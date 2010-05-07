using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    class RBTree
    {
        public enum Color
        {
            Black = 0,
            Red = 1
        }

        public class Node
        {
            //sentinel
            public Node()
            {
                this.key = -1;
                this.color = Color.Black;
            }

            public Node(int key)
            {
                this.key = key;
                this.color = Color.Red;
            }

            public Node parent;
            public Node left;
            public Node right;

            public int key;
            public Color color;
        }

        private Node root;
        private Node nil;

        public RBTree()
        {
            nil = new Node();
            root = nil;
        }

        public void LeftRotate(Node x)
        {
            Node y = x.right;
            x.right = y.left;

            if (y.left != nil)
                y.left.parent = x;
            y.parent = x.parent;

            if (x.parent == nil)
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

            if (x.right != nil)
                x.right.parent = y;
            x.parent = y.parent;

            if (y.parent == nil)
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
            Node y = nil;

            while (x != nil)
            {
                y = x;
                if (z.key < x.key)
                    x = x.left;
                else
                    x = x.right;
            }

            z.parent = y;
            if (y.parent == nil)
                root = z;
            else if (z.key < y.key)
                y.left = z;
            else
                y.right = z;

            z.left = nil;
            z.right = nil;

            InsertFixup(z);
        }

        public void InsertFixup(Node z)
        {
            while (z.parent.color == Color.Red)
            {
                if (z.parent == z.parent.parent.left)
                {
                    Node y = z.parent.parent.right;
                    if (y.color == Color.Red)
                    {
                        z.parent.color = Color.Black;
                        y.color = Color.Black;
                        z.parent.parent.color = Color.Red;
                        z = z.parent.parent;
                    }
                    else
                    {
                        if (z == z.parent.right)
                        {
                            z = z.parent;
                            LeftRotate(z);
                        }

                        z.parent.color = Color.Black;
                        z.parent.parent.color = Color.Red;
                        RightRotate(z.parent.parent);
                    }
                }
                //exactly the same as above but with "right" and "left" exchanged
                else
                {
                    Node y = z.parent.parent.left;
                    if (y.color == Color.Red)
                    {
                        z.parent.color = Color.Black;
                        y.color = Color.Black;
                        z.parent.parent.color = Color.Red;
                        z = z.parent.parent;
                    }
                    else
                    {
                        if (z == z.parent.left)
                        {
                            z = z.parent;
                            RightRotate(z);
                        }

                        z.parent.color = Color.Black;
                        z.parent.parent.color = Color.Red;
                        LeftRotate(z.parent.parent);
                    }
                }
            }
        }

        public void Transplant(Node u, Node v)
        {
            if (u.parent == nil)
                root = v;
            else if (u == u.parent.left)
                u.parent.left = v;
            else
                u.parent.right = v;
            v.parent = u.parent;
        }

        public void Delete(Node z)
        {
            Node x;
            Node y = z;
            Color yOrigColor = y.color;
            if (z.left == nil)
            {
                x = z.right;
                Transplant(z, z.right);
            }
            else if (z.right == nil)
            {
                x = z.left;
                Transplant(z, z.left);
            }
            else
            {
                y = TreeMinimum(z.right);
                yOrigColor = y.color;
                x = y.right;

                if (y.parent == z)
                    x.parent = y;
                else
                {
                    Transplant(y, y.right);
                    y.right = z.right;
                    y.right.parent = y;
                }

                Transplant(z, y);
                y.left = z.left;
                y.left.parent = y;
                y.color = z.color;
            }

            if (yOrigColor == Color.Black)
                DeleteFixup(x);
        }

        public Node TreeMinimum(Node x)
        {
            while (x.left != nil)
                x = x.left;
            return x;
        }

        public Node TreeMaximum(Node x)
        {
            while (x.right != nil)
                x = x.right;
            return x;
        }

        public void DeleteFixup(Node x)
        {
            while (x != root && x.color == Color.Black)
            {
                if (x == x.parent.left)
                {
                    Node w = x.parent.right;



                    //XXX
                }
                //exactly the same as above but with "right" and "left" exchanged
                else
                {
                    //XXX
                }
            }

            x.color = Color.Black;
        }
    }
}
