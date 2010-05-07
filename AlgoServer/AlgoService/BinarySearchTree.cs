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
            public Node(int key)
            {
                this.parent = null;
                this.left = null;
                this.right = null;

                this.key = key;
            }

            public Node parent;
            public Node left;
            public Node right;

            public int key;
        }

        private Node root;

        public BinarySearchTree()
        {
            root = null;
        }

        public List<Node> InorderTreeWalkRoot()
        {
            List<Node> keys = new List<Node>();
            InorderTreeWalk(root, keys);
            return keys;
        }

        public List<Node> InorderTreeWalk(Node x, List<Node> keys)
        {
            if (x != null)
            {
                keys.AddRange(InorderTreeWalk(x.left, keys));
                keys.Add(x);
                keys.AddRange(InorderTreeWalk(x.right, keys));
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

        public Node TreeMinimum(Node x)
        {
            while (x.left != null)
                x = x.left;
            return x;
        }

        public Node TreeMaximum(Node x)
        {
            while (x.right != null)
                x = x.right;
            return x;
        }

        public Node TreeSuccessor(Node x)
        {
            if (x.right != null)
                return TreeMinimum(x.right);

            Node y = x.parent;
            while (y != null && x == y.right)
            {
                x = y;
                y = y.parent;
            }
            return y;
        }

        public Node TreePredecessor(Node x)
        {
            if (x.left != null)
                return TreeMaximum(x.left);

            Node y = x.parent;            
            while (y != null && x == y.left)
            {
                x = y;
                y = y.parent;
            }
            return y;
        }

        public void TreeInsert(int key)
        {
            Node newNode = new Node(key);

            Node x = root;
            Node y = null;

            while (x != null)
            {
                y = x;
                if (newNode.key < x.key)
                    x = x.left;
                else
                    x = x.right;
            }
            
            newNode.parent = y;
            //if tree empty
            if (y == null)
                root = newNode;
            else if (newNode.key < y.key)
                y.left = newNode;
            else
                y.right = newNode;
        }

        public void Transplant(Node u, Node v)
        {
            if (u.parent == null)
                root = v;
            else if (u == u.parent.left)
                u.parent.left = v;
            else
                u.parent.right = v;

            if (v != null)
                v.parent = u.parent;
        }

        public void TreeDelete(Node z)
        {
            if (z.left == null)
                Transplant(z, z.right);
            else if (z.right == null)
                Transplant(z, z.left);
            else
            {
                Node y = TreeMinimum(z.right);
                if (y.parent != z)
                {
                    Transplant(y, y.right);
                    y.right = z.right;
                    y.right.parent = y;
                }

                Transplant(z, y);
                y.left = z.left;
                y.left.parent = y;
            }
        }
    }
}
