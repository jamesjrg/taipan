using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    class RBTree
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
            //0 is black, 1 is red
            public bool color;
        }

        private Node root;

        public void LeftRotate(Node x)
        {
            Node y = x.right;
            x.right = y.left;

            if (y.left != null)
                y.left.parent = x;
            y.parent = x.parent;

            if (x.parent == null)
                root = y;
            else if (x == x.parent.left)
                x.parent.left = y;
            else
                x.parent.right = y;

            y.left = x;
            x.parent = y;
        }

        public void RightRotate(Node y)
        {
            Node x = y.left;
            y.left = x.right;

            if (x.right != null)
                x.right.parent = y;
            x.parent = y.parent;

            if (y.parent == null)
                root = x;
            else if (y == y.parent.left)
                y.parent.left = x;
            else
                y.parent.right = x;

            x.right = y;
            y.parent = x;
        }

        public void Insert(Node z)
        {
            Node x = root;
            Node y = null;

            while (x != null)
            {
            }
            //XXX
        }

        public void InsertFixup(Node z)
        {
        }

        public void Transplant(Node u, Node v)
        {
        }

        public void Delete(Node z)
        {
        }

        public void DeleteFixup(Node z)
        {
        }
    }
}
