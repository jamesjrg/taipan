using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AlgoService
{
    class BTree
    {
        private string filenamePrefix;

        private const int MIN_DEGREE = 4;
        private const int MIN_KEYS = MIN_DEGREE - 1;
        private const int MAX_KEYS = 2 * MIN_DEGREE - 1;
        private const int MAX_CHILDREN = 2 * MIN_DEGREE;

        private Node root;

        public class Node
        {
            public Node()
            {
                keys = new int[MAX_KEYS];
                children = new Node[MAX_CHILDREN];
            }

            public int count;
            public int[] keys;
            public Node[] children;
            public bool leaf;
        }

        public class NodeIndexPair
        {
            public NodeIndexPair(Node node, int index)
            {
                this.node = node;
                this.index = index;
            }

            public Node node;
            public int index;
        }

        public BTree(int id, bool truncate)
        {
            filenamePrefix = "btree" + id + "-";
            
            if (truncate)
            {
                DirectoryInfo dir = new DirectoryInfo(".");
                FileInfo[] myfiles = dir.GetFiles(filenamePrefix + "*");
                foreach (FileInfo f in myfiles)
                    f.Delete();
            }

            root = null;
        }

        public NodeIndexPair SearchRoot(int k)
        {
            //empty tree?
            if (root == null)
                return null;

            return Search(root, k);
        }

        public NodeIndexPair Search(Node x, int k)
        {
            int i = 0;
            while (i != x.count && k > x.keys[i])
                i++;

            if (i != x.count && k == x.keys[i])
                return new NodeIndexPair(x, i);
            else if (x.leaf)
                return null;
            else
            {
                Node child = DiskReadNode(x, i);
                return Search(x.children[i], k);
            }
        }

        private Node DiskReadNode(Node parent, int index)
        {
            //ClassToSerialize c = new ClassToSerialize();
            //File f = new File("temp.dat");
            //Stream s = f.Open(FileMode.Open);
            //BinaryFormatter b = new BinaryFormatter();
            //c = (ClassToSerialize)b.Deserialize(s);
            //Console.WriteLine(c.name);

            return new Node();
        }

        private void DiskWriteNode()
        {
            //if (File.Exists(path))
            //    File.Delete(path);

            //File.Create(path);

            //ClassToSerialize c = new ClassToSerialize();
            //File f = new File("temp.dat");
            //Stream s = f.Open(FileMode.Create);
            //BinaryFormatter b = new BinaryFormatter();
            //b.Serialize(s, c);
            //s.Close();
        }
    }
}
