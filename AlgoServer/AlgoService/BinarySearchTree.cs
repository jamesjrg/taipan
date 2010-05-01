using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    class BinarySearchTree
    {
        public class Node
        {
            public Node parent;
            public Node left;
            public Node right;

            public int key;
        }

        private Node root;

        //i.e. inorder tree walk
        public List<Node> GetKeys()
        {
            List<Node> keys = new List<Node>();
            GetKeys(root, keys);
            return keys;
        }

        public List<Node> GetKeys(Node x, List<Node> keys)
        {
            if (x != null)
            {
                keys.AddRange(GetKeys(x.left, keys));
                keys.Add(x);
                keys.AddRange(GetKeys(x.right, keys));
            }

            return keys;
        }

        //iterative method as opposed to recursive
        //I have made it always start from root
        public Node TreeSearch(int k)
        {
            Node x = root;

            while (x != null && k != x.key)
            {
                if (k < x.key)
                    x = x.left;
                else 
                    x = x.right;
            }
            return x;
        }

        public Node TreeMinimum()
        {
            Node x = root;
            while (x.left != null)
                x = x.left;
            return x;
        }

        public Node TreeMaximum()
        {
            Node x = root;
            while (x.right != null)
                x = x.right;
            return x;
        }

        public int TreeSuccessor()
        {
            return 0;
        }

        public int TreePredecessor()
        {
            return 0;
        }

        public int TreeInsert()
        {
            return 0;
        }

        public int Transplant()
        {
            return 0;
        }

        public int TreeDelete()
        {
            return 0;
        }

    }
}
