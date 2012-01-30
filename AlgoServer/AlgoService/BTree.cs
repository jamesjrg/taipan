using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AlgoService
{
    /// <summary>
    /// A B-Tree. Very much unsafe code, lots of pointers and byte-level file manipulation.
    /// Notes:
    /// a) root node isn't necessarily at any particular index - gets shifted right when root node needs to be split
    /// b) Btree root and Node children "pointers" are actually int pseudo pointers for disk seeking
    /// c) First node on disk isn't a real node, instead it stores btree metadata - root location etc
    /// d) For reading/writing nodes directly from/to file the Node class is a value-type struct
    /// with a fixed length array, but this makes for ridiculously messy code. I should have just used C++ or Python for this, C# is terrible for this sort of thing
    /// 
    /// </summary>
    unsafe public class BTree: IDisposable
    {
        private string filename;
        private FileStream fs;

        private const int MIN_DEGREE = 4;
        private const int MIN_KEYS = MIN_DEGREE - 1;
        private const int MAX_KEYS = 2 * MIN_DEGREE - 1;
        private const int MAX_CHILDREN = 2 * MIN_DEGREE;

        private const int NIL_POINTER = -1;

        private readonly int NODE_SIZE;
        private int NumNodes;

        //root node always kept in memory
        private Node RootNode;
        private int RootIndex;        

        private bool isDisposed = false;

        //a struct with fixed length arrays so it is easy to serialize
        public struct Node
        {
            public int count;
            public fixed int keys[MAX_KEYS];
            public fixed int children[MAX_CHILDREN];
            public bool leaf;
        }

        public class NodeIndexPair
        {
            //note passed by value, will create copy of node
            public NodeIndexPair(Node node, int index)
            {
                this.node = node;
                this.index = index;
            }

            public Node node;
            public int index;
        }

        unsafe public BTree(int id, bool truncate)
        {
            filename = "btree" + id;

            NODE_SIZE = sizeof(Node);

            if (truncate)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);

                RootIndex = NIL_POINTER;
                NumNodes = 0;
            }
            else
            {
                //read file
                fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
                //read in special node zero data
                Node specialNode = DiskReadNode(0);
                NumNodes = specialNode.children[0];
                RootIndex = specialNode.children[1];

                //if not empty, read in root node
                if (NumNodes > 0)
                    RootNode = DiskReadNode(RootIndex);
            }
        }

        ~BTree()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {}//no managed objects
                
                //write data to special node 0, use the root node as a temp var
                fixed (int* ptr = RootNode.children)
                {
                    ptr[0] = NumNodes;
                    ptr[1] = RootIndex;
                }
                DiskWriteNode(ref RootNode, 0);
            }
            isDisposed = true;
        }

        /// <summary>
        /// For testing
        /// </summary>
        public string Dump()
        {
            //XXX
            return "";
        }

        public void Insert(int k)
        {
            //empty tree?
            if (RootIndex == NIL_POINTER)
            {
                RootNode = new Node();
                RootNode.leaf = true;
                RootNode.count = 0;

                //will be written to disk in InsertNonFull
            }
            else
            {
                //full root? If so, create a new root node
                if (RootNode.count == MAX_KEYS)
                {         
                    //old root node
                    int oldRootIndex = RootIndex;
                    Node oldRoot = RootNode;

                    //new root node
                    NumNodes++;
                    RootIndex = NumNodes;
                    RootNode.leaf = false;
                    RootNode.count = 0;
                    fixed (int* ptr = RootNode.children)
                    {
                        ptr[0] = RootIndex;
                    }
                    
                    //splitting first child of new root, i.e. the old root
                    SplitChildNode(ref RootNode, RootIndex, ref oldRoot, oldRootIndex, 0);
                }
            }
            InsertNonFull(ref RootNode, RootIndex, k);
        }

        private void InsertNonFull(ref Node node, int nodeIndex, int k)
        {
            int i = node.count;
            if (node.leaf)
            {
                fixed (int* ptr = node.keys)
                {
                    while (i >= 0 && k < ptr[i])
                    {
                        ptr[i + 1] = ptr[i];
                        i--;
                    }
                    ptr[i + 1] = k;
                }
                node.count++;
                DiskWriteNode(ref node, nodeIndex);
            }
            else
            {
                fixed (int* ptr = node.keys)
                {
                    while (i >= 0 && k < ptr[i])
                        i--;

                    i = i + 1;
                    fixed (int* childrenPtr = node.children)
                    {
                        int childIndex = childrenPtr[i];

                        Node child = DiskReadNode(childIndex);
                        if (child.count == MAX_KEYS)
                        {
                            SplitChildNode(ref node, nodeIndex, ref child, childIndex, i);
                            if (k > ptr[i])
                                i++;
                        }

                        InsertNonFull(ref child, childIndex, k);
                    }                    
                }                
            } 
        }

        private void SplitChildNode(ref Node parent, int parentIndex, ref Node child, int childIndex, int whichChild)
        {
            Node newRight = new Node();
            newRight.leaf = child.leaf;
            newRight.count = MIN_KEYS;

            fixed (int* childKeys = child.keys)
            {
                for (int j = 0; j != MIN_KEYS; ++j)
                    newRight.keys[j] = childKeys[j];
            }
            if (!child.leaf)
            {
                fixed (int* childChildren = child.children)
                {
                    for (int j = 0; j != MIN_DEGREE; ++j)
                        newRight.children[j] = childChildren[j + MIN_DEGREE];
                }
            }
            child.count = MIN_KEYS;

            NumNodes++;
            //NumNodes now has the index of newRight

            //push up into parent node
            fixed (int* parentChildren = parent.children)
            {
                for (int j = parent.count; j != whichChild; --j)
                    parentChildren[j + 1] = parentChildren[j];
                //NumNodes is newRight pseudo-pointer
                parentChildren[whichChild + 1] = NumNodes;
            }

            fixed (int* parentKeys = parent.keys)
            {
                for (int j = parent.count - 1; j != whichChild - 1; --j)
                    parentKeys[j + 1] = parentKeys[j];
                fixed (int* childKeys = child.keys)
                    parentKeys[whichChild] = childKeys[MIN_DEGREE];
            }

            parent.count += 1;

            //write left child
            DiskWriteNode(ref child, childIndex);

            //write new right node
            DiskWriteNode(ref newRight, NumNodes);

            //write parent node
            DiskWriteNode(ref parent, parentIndex);
        }

        public NodeIndexPair Search(int k)
        {
            //empty tree?
            if (RootIndex == NIL_POINTER)
                return null;

            return Search(ref RootNode, k);
        }

        private NodeIndexPair Search(ref Node node, int k)
        {
            int i = 0;
            fixed (int* ptr = node.keys)
            {
                while (i != node.count && k > ptr[i])
                    i++;

                if (i != node.count && k == ptr[i])
                    return new NodeIndexPair(node, i);
                else if (node.leaf)
                    return null;
                else
                {
                    Node tmp = DiskReadNode(i);
                    return Search(ref tmp, k);
                }
            }
        }

        public void Delete(int k)
        {
            //empty tree?
            if (RootIndex == NIL_POINTER)
                throw new Exception("empty tree");

            Delete(ref RootNode, RootIndex);
        }

        private void Delete(ref Node node, int nodeIndex)
        {
            //walk down tree from root searching for k
            //before recursively calling delete on any child, check that child has at least min keys + 1. if not, give it one of current nodes keys. if currentnode is root and this leaves root with 0 keys, then delete root and replace it with its first child

            //on reaching k:
            //if given node is a leaf
            //can now delete key
            //if given node is an internal node
            //if child that precedes key has at least t keys, then recursively delete predecessor of k, and move that predecessor to k's place
            //else if child that succeeds key has at least t keys, try the same with successor-containing node
            //else merge succeeding node into preceding node, along with k (freeing succeeding node from disk), then recursively delete k from new merged child
        }

        private Node DiskReadNode(int index)
        {
            byte[] arr = new byte[NODE_SIZE];
            Node node = new Node();

            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Read(arr, 0, NODE_SIZE);

            GCHandle pinnedArr = GCHandle.Alloc(arr, GCHandleType.Pinned);
            node = (Node)Marshal.PtrToStructure(
                pinnedArr.AddrOfPinnedObject(),
                typeof(Node));
            pinnedArr.Free();

            return node;

            //for testing?
            //UTF8Encoding temp = new UTF8Encoding(true);
            //while (fs.Read(b, 0, b.Length) > 0)
            //    Console.WriteLine(temp.GetString(b));
        }

        //write CurrentNode to disk file
        private void DiskWriteNode(ref Node node, int index)
        {
            //I'm assuming just sizeof(Node) is fine, I mean come on
            //int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[NODE_SIZE];
            IntPtr ptr = Marshal.AllocHGlobal(NODE_SIZE);
            Marshal.StructureToPtr(node, ptr, true);
            Marshal.Copy(ptr, arr, 0, NODE_SIZE);
            Marshal.FreeHGlobal(ptr);

            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Write(arr, 0, NODE_SIZE);
        }
    }
}