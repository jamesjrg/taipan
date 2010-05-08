using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    public class RBTree
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

        //iterative method as opposed to recursive
        //I have made it always start from root
        public Node Search(int k)
        {
            Node x = root;

            while (x != nil && k != x.key)
            {
                if (k < x.key)
                    x = x.left;
                else
                    x = x.right;
            }
            return x;
        }

        public Node Successor(Node x)
        {
            if (x.right != nil)
                return Minimum(x.right);

            Node y = x.parent;
            while (y != nil && x == y.right)
            {
                x = y;
                y = y.parent;
            }
            return y;
        }

        public Node Predecessor(Node x)
        {
            if (x.left != nil)
                return Maximum(x.left);

            Node y = x.parent;
            while (y != nil && x == y.left)
            {
                x = y;
                y = y.parent;
            }
            return y;
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

        public void Insert(int key)
        {
            Node newNode = new Node(key);

            Node x = root;
            Node y = nil;

            while (x != nil)
            {
                y = x;
                if (newNode.key < x.key)
                    x = x.left;
                else
                    x = x.right;
            }

            newNode.parent = y;
            if (y.parent == nil)
                root = newNode;
            else if (newNode.key < y.key)
                y.left = newNode;
            else
                y.right = newNode;

            newNode.left = nil;
            newNode.right = nil;

            InsertFixup(newNode);
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
                y = Minimum(z.right);
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

        public Node Minimum(Node x)
        {
            while (x.left != nil)
                x = x.left;
            return x;
        }

        public Node Maximum(Node x)
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
                    if (w.color == Color.Red)
                    {
                        w.color = Color.Black;
                        x.parent.color = Color.Red;
                        LeftRotate(x.parent);
                        w = x.parent.right;
                    }
                    if (w.left.color == Color.Black && w.right.color == Color.Black)
                    {
                        w.color = Color.Red;
                        x = x.parent;
                    }
                    else
                    {
                        if (w.right.color == Color.Black)
                        {
                            w.left.color = Color.Black;
                            w.color = Color.Red;
                            RightRotate(w);
                            w = x.parent.right;
                        }
                        w.color = x.parent.color;
                        x.parent.color = Color.Black;
                        w.right.color = Color.Black;
                        LeftRotate(x.parent);
                        x = root;
                    }
                }
                //exactly the same as above but with "right" and "left" exchanged
                else
                {
                    Node w = x.parent.left;
                    if (w.color == Color.Red)
                    {
                        w.color = Color.Black;
                        x.parent.color = Color.Red;
                        RightRotate(x.parent);
                        w = x.parent.left;
                    }
                    if (w.right.color == Color.Black && w.left.color == Color.Black)
                    {
                        w.color = Color.Red;
                        x = x.parent;
                    }
                    else
                    {
                        if (w.left.color == Color.Black)
                        {
                            w.right.color = Color.Black;
                            w.color = Color.Red;
                            LeftRotate(w);
                            w = x.parent.left;
                        }
                        w.color = x.parent.color;
                        x.parent.color = Color.Black;
                        w.left.color = Color.Black;
                        RightRotate(x.parent);
                        x = root;
                    }
                }
            }

            x.color = Color.Black;
        }
    }
}
