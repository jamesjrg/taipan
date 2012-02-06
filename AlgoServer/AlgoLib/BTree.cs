using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AlgoLib
{
    /// <summary>
    /// A B-Tree
    /// Notes:
    /// a) root node isn't necessarily at any particular index - gets shifted to second-to-last position when root node needs to be split
    /// b) Btree root and Node children "pointers" are actually int pseudo pointers for disk seeking
    /// c) First node on disk isn't a real node, instead it stores btree metadata - root location etc
    /// d) For reading/writing nodes directly from/to file the Node class is a value-type struct, though this means you have to do some manual faffing with passing things specifically by reference etc
    /// e) key cannot be "0", as that is currently used to mean "empty" (though this could easily be changed)
    /// 
    /// </summary>
    public class BTree: IDisposable
    {
        private string filename;
        private FileStream fs;

        public const int MIN_DEGREE = 4;
        public const int MIN_KEYS = MIN_DEGREE - 1;
        public const int MAX_KEYS = 2 * MIN_DEGREE - 1;
        public const int MAX_CHILDREN = 2 * MIN_DEGREE;

        private const int NIL_POINTER = -1;

        private readonly int NODE_SIZE;
        private int LastNodeIndex;

        //root node always kept in memory
        private Node RootNode;
        private int RootIndex;        

        private bool isDisposed = false;

        //a struct that can be serialized
        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {
            public int count;
            public bool leaf;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_KEYS)]
            public int[] keys;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHILDREN)]
            public int[] children;

            //Don't use the default constructor! User-defined parameterless constructors aren't allowed for value types by C#, for some reason
            public static Node NewNode()
            {
                var newNode = new Node();
                newNode.keys = new int[MAX_KEYS];
                newNode.children = new int[MAX_CHILDREN];
                return newNode;
            }

            public Node Clone()
            {
                var newNode = NewNode();
                newNode.count = count;
                newNode.leaf = leaf;
                keys.CopyTo(newNode.keys, 0);
                children.CopyTo(newNode.children, 0);

                return newNode;
            }

            public void AppendToStrB(StringBuilder builder, int index)
            {
                builder.Append("I:" + index + ":");
                builder.Append("K:");

                if (count == NIL_POINTER)
                    builder.Append("deleted");
                else
                {
                    for (int i = 0; i != this.count; i++)
                        builder.Append(keys[i] + ",");
                    builder.Remove(builder.Length - 1, 1);
                }
                
                builder.Append(";");
            }
        }

        public class NodeIndexPair
        {
            //note passed by value, will create copy of node
            public NodeIndexPair(Node node, int index)
            {
                this.node = node;
                this.index = index;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                node.AppendToStrB(builder, index);
                return builder.ToString();
            }

            public Node node;
            public int index;
        }

        public BTree(int id, bool newOrTruncate)
        {
            filename = "btree" + id;

            NODE_SIZE = Marshal.SizeOf(RootNode);

            if (newOrTruncate)
            {
                fs = File.Open(filename, FileMode.Create, FileAccess.ReadWrite);
                RootIndex = NIL_POINTER;
                LastNodeIndex = NIL_POINTER;
            }
            else
            {
                //read file
                fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
                //read in special node zero data
                Node specialNode = DiskReadNode(0);
                LastNodeIndex = specialNode.children[0];
                RootIndex = specialNode.children[1];

                //if not empty, read in root node
                if (RootIndex != NIL_POINTER)
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
                RootNode.children[0] = LastNodeIndex;
                RootNode.children[1] = RootIndex;

                DiskWriteNode(ref RootNode, 0);

                fs.Close();
            }
            isDisposed = true;
        }

        //For testing
        public string Dump()
        {
            StringBuilder builder = new StringBuilder();

            if (RootIndex == -1)
                return "";

            for (int i = 0; i != LastNodeIndex + 1; ++i)
            {
                Node node = DiskReadNode(i);
                node.AppendToStrB(builder, i);
                builder.Append("  ");
            }
            builder.Remove(builder.Length - 2, 2);
            
            return builder.ToString();
        }

        public void Insert(int k)
        {
            //empty tree?
            if (RootIndex == NIL_POINTER)
            {
                RootNode = Node.NewNode();
                RootIndex = 0;
                LastNodeIndex = 0;
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
                    Node oldRoot = RootNode.Clone();

                    //new root node
                    LastNodeIndex++;
                    RootIndex = LastNodeIndex;
                    RootNode.leaf = false;
                    RootNode.count = 0;
                    RootNode.children[0] = oldRootIndex;
                    
                    //splitting first child of new root, i.e. the old root
                    SplitChildNode(ref RootNode, RootIndex, ref oldRoot, oldRootIndex, 0);
                }
            }
            InsertNonFull(ref RootNode, RootIndex, k);
        }

        private void InsertNonFull(ref Node node, int nodeIndex, int k)
        {
            int i = node.count - 1;

            if (node.leaf)
            {
                while (i >= 0 && k < node.keys[i])
                {
                    node.keys[i + 1] = node.keys[i];
                    i--;
                }
                node.keys[i + 1] = k;
                node.count++;
                DiskWriteNode(ref node, nodeIndex);
            }
            else
            {                
                while (i >= 0 && k < node.keys[i])
                    i--;

                i = i + 1;
                int childIndex = node.children[i];

                Node child = DiskReadNode(childIndex);
                if (child.count == MAX_KEYS)
                {
                    SplitChildNode(ref node, nodeIndex, ref child, childIndex, i);
                    if (k > node.keys[i])
                        i++;
                }

                InsertNonFull(ref child, childIndex, k);
            } 
        }

        private void SplitChildNode(ref Node parent, int parentIndex, ref Node child, int childIndex, int whichChild)
        {
            Node newRight = Node.NewNode();
            newRight.leaf = child.leaf;
            newRight.count = MIN_KEYS;

            //new right hand node copies MIN_KEYS largest keys from child
            for (int j = 0; j != MIN_KEYS; ++j)
                newRight.keys[j] = child.keys[j + MIN_DEGREE];
             child.count = MIN_KEYS;
             
            //if not a leaf node, then new right hand node copies MIN_DEGREE largest children from child
            if (!child.leaf)
            {
                for (int j = 0; j != MIN_DEGREE; ++j)
                    newRight.children[j] = child.children[j + MIN_DEGREE];
            }            

            LastNodeIndex++;
            //LastNodeIndex now has the index of newRight

            //push up into parent node
            for (int j = parent.count; j != whichChild; --j)
                parent.children[j + 1] = parent.children[j];
            //LastNodeIndex is newRight pseudo-pointer
            parent.children[whichChild + 1] = LastNodeIndex;
            
            for (int j = parent.count - 1; j != whichChild - 1; --j)
                parent.keys[j + 1] = parent.keys[j];
            parent.keys[whichChild] = child.keys[MIN_DEGREE - 1];

            parent.count += 1;

            //write left child
            DiskWriteNode(ref child, childIndex);

            //write new right node
            DiskWriteNode(ref newRight, LastNodeIndex);

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
            while (i != node.count && k > node.keys[i])
                i++;

            if (i != node.count && k == node.keys[i])
                return new NodeIndexPair(node, i);
            else if (node.leaf)
                return null;
            else
            {
                Node child = DiskReadNode(node.children[i]);
                return Search(ref child, k);
            }
        }

        public void Delete(int k)
        {
            //empty tree?
            if (RootIndex == NIL_POINTER)
                throw new Exception("Called delete on empty tree");
                
            Delete(ref RootNode, RootIndex, k);
        }

        //walk down tree from root searching for k
        private void Delete(ref Node node, int nodeIndex, int k)
        {
            int i = 0;
            while (i != node.count && k > node.keys[i])
                i++;

            if (i != node.count && k == node.keys[i])
            {
                if (node.leaf)
                {
                    //delete by shuffling keys leftwards
                    while (i != node.count - 1)
                    {
                        node.keys[i] = node.keys[i + 1];
                        i++;
                    }
                    node.count--;
                    DiskWriteNode(ref node, nodeIndex);
                }
                else
                {
                    Node predecessor = DiskReadNode(node.children[i]);
                    
                    //replace k by predecessor of k, then recursively delete predecessor of k
                    if (predecessor.keys >= MIN_DEGREE)
                    {
                        int kPrime = predecessor.keys[predecessor.count - 1];
                        node.keys[i] = kPrime;
                        Delete(predecessor, i, kPrime);
                    }
                    else
                    {
                        int successorIndex = i + 1;
                        Node successor = DiskReadNode(node.children[successorIndex]);
                        
                        //symmetrically, replace k by successor of k, then recursively delete successor of k
                        if (successor.keys >= MIN_DEGREE)
                        {
                            int kPrime = successor.keys[0];
                            node.keys[i] = kPrime;
                            Delete(successor, successorIndex, kPrime);
                        }
                        //merge succeeding node into preceding node, along with k (freeing succeeding node from disk), then recursively delete k from new merged child
                        else                        
                        {
                            //xxx
                            //DiskDeleteNode(index);
                        }
                    }
                }                
            }
            else if (node.leaf)
                throw new Exception("Key not found: " + k);
            else
            {
                Node child = DiskReadNode(node.children[i]);
                
                if (child.count < MIN_DEGREE)
                {
                    //xxx give it one of current node's keys.
                    
                    //if root node will now become empty
                    if (node == RootNode && node.count == 0)
                    {
                        //delete root and replace it with its first child
                        //DiskDeleteNode(index);
                        //xxx possibly special behaviour if deleting final key in tree - i.e. change rootindex and lastnodeindex to null constant
                    }
                }
                
                Delete(ref child, nodeIndex, k);
            }
        }

        private Node DiskReadNode(int index)
        {
            byte[] arr = new byte[NODE_SIZE];
            Node node = Node.NewNode();

            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Read(arr, 0, NODE_SIZE);

            GCHandle pinnedArr = GCHandle.Alloc(arr, GCHandleType.Pinned);
            node = (Node)Marshal.PtrToStructure(
                pinnedArr.AddrOfPinnedObject(),
                typeof(Node));
            pinnedArr.Free();

            return node;

            //for testing
            //UTF8Encoding temp = new UTF8Encoding(true);
            //while (fs.Read(b, 0, b.Length) > 0)
            //    Console.WriteLine(temp.GetString(b));
        }

        private void DiskWriteNode(ref Node node, int index)
        {
            byte[] arr = new byte[NODE_SIZE];
            IntPtr ptr = Marshal.AllocHGlobal(NODE_SIZE);
            Marshal.StructureToPtr(node, ptr, false);
            Marshal.Copy(ptr, arr, 0, NODE_SIZE);
            Marshal.FreeHGlobal(ptr);

            fs.Seek(index * NODE_SIZE, SeekOrigin.Begin);
            fs.Write(arr, 0, NODE_SIZE);
        }
        
        //For the time being, if btree delete results in deletion of node, then write that node's position on disk as invalid data.
        //more advanced would be some sort of manual or automatic garbage collection that shifts data to fill in these gaps, and then truncates the now unused space at the end of the file.
        private void DiskDeleteNode(int index)
        {
            Node nullNode = Node.NewNode();
            nullNode.count = NIL_POINTER;
            DiskWriteNode(Node, index);
        }
    }
}

