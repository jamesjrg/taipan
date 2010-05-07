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
    /// </summary>
    unsafe class BTree
    {
        private string filename;
        private FileStream fs;

        private const int MIN_DEGREE = 4;
        private const int MIN_KEYS = MIN_DEGREE - 1;
        private const int MAX_KEYS = 2 * MIN_DEGREE - 1;
        private const int MAX_CHILDREN = 2 * MIN_DEGREE;

        private readonly int NODE_SIZE;

        //pseudo pointer, i.e. we seek disk to find nodes rather than storing them in memory
        private int root;

        Node currentNode;

        //a struct with fixed length arrays so it is easy to serialize
        public struct Node
        {
            public int count;
            public fixed int keys[MAX_KEYS];
            //pseudo pointer, i.e. we seek disk to find nodes rather than storing them in memory
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

                //XXX root = something;
            }
            else
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);

                //XXX root = something
            }
        }

        public NodeIndexPair SearchRoot(int k)
        {
            //empty tree?
            if (root == -1)
                return null;

            DiskReadNode(root);
            return Search(k);
        }

        public NodeIndexPair Search(int k)
        {
            int i = 0;
            fixed (int* ptr = currentNode.keys)
            {
                while (i != currentNode.count && k > ptr[i])
                    i++;

                if (i != currentNode.count && k == ptr[i])
                    return new NodeIndexPair(currentNode, i);
                else if (currentNode.leaf)
                    return null;
                else
                {
                    DiskReadNode(i);
                    return Search(k);
                }
            }
        }

        private void DiskReadNode(int index)
        {
            //byte[] b = new byte[1024];
            //UTF8Encoding temp = new UTF8Encoding(true);
            //while (fs.Read(b, 0, b.Length) > 0)
            //{
            //    Console.WriteLine(temp.GetString(b));
            //}

            currentNode = new Node();
        }

        private void DiskWriteNode()
        {
            //I'm assuming just sizeof(Node) is fine, I mean come on
            //int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[NODE_SIZE];
            IntPtr ptr = Marshal.AllocHGlobal(NODE_SIZE);
            Marshal.StructureToPtr(currentNode, ptr, true);
            Marshal.Copy(ptr, arr, 0, NODE_SIZE);
            Marshal.FreeHGlobal(ptr);

            //fs.Write(info, 0, info.Length);
        }
    }
}