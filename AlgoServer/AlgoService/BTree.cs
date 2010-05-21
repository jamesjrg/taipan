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
        private int Root;
        private int NumNodes;

        private bool isDisposed = false;

        Node CurrentNode;
        private int CurrentNodeIndex;

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

                Root = NIL_POINTER;
                NumNodes = 0;
            }
            else
            {
                //read file
                fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
                //read in special node zero data
                DiskReadNode(0);
                NumNodes = CurrentNode.children[0];
                Root = CurrentNode.children[1];
            }
        }

        public ~BTree()
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
                
                //write data to special node 0
                CurrentNode.children[0] = NumNodes;
                CurrentNode.children[1] = Root;
                DiskWriteNode(CurrentNode, 0);
            }
            isDisposed = true;
        }

        public void Insert(int k)
        {
            //empty tree?
            if (Root == NIL_POINTER)
            {
                //xxx
                //leaf = true
            }
            else
            {
                //full root? If so, create a new root node
                DiskReadNode(Root);
                if (CurrentNode.count == MAX_KEYS)
                {         
                    //XXX maybe should be able to do this without creating an extra node object
                    Node s = new Node();
                    s.leaf = false;
                    s.count = 0;
                    s.children[0] = Root;
                    NumNodes++;
                    Root = NumNodes;

                    //write old root node to disk, new root node is now CurrentNode
                    DiskWriteNode(CurrentNode, CurrentNodeIndex);

                    CurrentNode = s;
                    
                    SplitNode();
                    //XXX assuming current node is still s for insert nonfull?
                }
            }
            InsertNonFull(k);
        }

        private void InsertNonFull(int k)
        {
            int i = CurrentNode.count;
            if (CurrentNode.leaf)
            {
                while (i >= 0 && k < CurrentNode.keys[i])
                {
                    CurrentNode.keys[i + 1] = CurrentNode.keys[i];
                    i--;
                }
                CurrentNode.keys[i + 1] = k;
                CurrentNode.count++;
                DiskWriteNode(CurrentNode, CurrentNodeIndex);
            }
            else
            {
                while (i >= 0 && k < CurrentNode.keys[i])
                    i--;

                i = i + 1;
                DiskReadNode(CurrentNode.children[i]);
                if (CurrentNode.count == MAX_KEYS)
                {
                    SplitNode();
                    //XXX
                    //reread our orig node into memory?
                    //if (k > parentnode.keys[i])
                    //i = i + 1;
                    //XXX assuming current node is still s for insert nonfull?
                }
                InsertNonFull(k);
            } 
        }

        //split CurrentNode, not a child of that node as in Cormen
        private void SplitNode()
        {
            Node newRight = new Node();
            newRight.leaf = CurrentNode.leaf;
            newRight.count = MIN_KEYS;

            for (int j = 0; j != MIN_KEYS; ++j)
                newRight.keys[j] = CurrentNode.keys[j];
            if (!CurrentNode.leaf)
            {
                for (int j = 0; j != MIN_DEGREE; ++j)
                    newRight.children[j] = CurrentNode.children[j + MIN_DEGREE];
            }
            CurrentNode.count = MIN_KEYS;

            //write current node (left child)
            DiskWriteNode(CurrentNode, CurrentNodeIndex);

            //write new right node
            NumNodes++;
            DiskWriteNode(newRight, NumNodes);

            //XXX need to push up into parent node
            //XXX read in parent node
            //for (j = parent...
            //  parent.children...
            //parent.children...
            //for (j = parent...
            //  parent.keys...
            //parent.keys...
            //x.count =

            //write parent node
            DiskWriteNode(
            //xxx need to put currentnode back to where it was originally?
        }

        public NodeIndexPair SearchRoot(int k)
        {
            //empty tree?
            if (Root == NIL_POINTER)
                return null;

            DiskReadNode(Root);
            return Search(k);
        }

        public NodeIndexPair Search(int k)
        {
            int i = 0;
            fixed (int* ptr = CurrentNode.keys)
            {
                while (i != CurrentNode.count && k > ptr[i])
                    i++;

                if (i != CurrentNode.count && k == ptr[i])
                    return new NodeIndexPair(CurrentNode, i);
                else if (CurrentNode.leaf)
                    return null;
                else
                {
                    DiskReadNode(i);
                    return Search(k);
                }
            }
        }

        public void Delete(int k)
        {
            //walk down tree from root searching for k
            //before recursively calling delete on any child of root, check that child has at least min keys + 1. if not, give it one of current nodes keys. if currentnode is root and this leaves root with 0 keys, then delete root and replace it with its first child
            
            //on reaching k:
            //if given node is a leaf
                //can now delete key
            //if given node is an internal node
                //if child that precedes key has at least t keys, then recursively delete predecessor of k, and move that predecessor to k's place
                //else if child that succeeds key has at least t keys, try the same with successor-containing node
                //else merge succeeding node into preceding node, along with k (freeing succeeding node from disk), then recursively delete k from new merged child
        }

        private void DiskReadNode(int index)
        {
            byte[] arr = new byte[NODE_SIZE];
            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Read(arr, 0, NODE_SIZE);

            GCHandle pinnedArr = GCHandle.Alloc(arr, GCHandleType.Pinned);
            CurrentNode = (Node)Marshal.PtrToStructure(
                pinnedArr.AddrOfPinnedObject(),
                typeof(Node));
            pinnedArr.Free();
            CurrentNodeIndex = index;

            //for testing?
            //UTF8Encoding temp = new UTF8Encoding(true);
            //while (fs.Read(b, 0, b.Length) > 0)
            //    Console.WriteLine(temp.GetString(b));
        }

        //write CurrentNode to disk file
        private void DiskWriteNode(Node theNode, int index)
        {
            //I'm assuming just sizeof(Node) is fine, I mean come on
            //int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[NODE_SIZE];
            IntPtr ptr = Marshal.AllocHGlobal(NODE_SIZE);
            Marshal.StructureToPtr(theNode, ptr, true);
            Marshal.Copy(ptr, arr, 0, NODE_SIZE);
            Marshal.FreeHGlobal(ptr);

            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Write(arr, 0, NODE_SIZE);
        }
    }
}