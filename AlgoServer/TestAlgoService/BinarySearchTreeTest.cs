using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestAlgoService
{
    [TestClass()]
    public class BinarySearchTreeTest
    {
        private static List<AssertData> assertData = new List<AssertData>();

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private class AssertData
        {
            public AssertData(int[] input)
            {
                this.input = input;
            }

            public BinarySearchTree BuildTree()
            {
                BinarySearchTree tree = new BinarySearchTree();

                foreach (var key in input)
                    tree.TreeInsert(key);

                return tree;
            }

            private int[] input;
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        [TestMethod()]
        public void TreeSuccessorTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            BinarySearchTree.Node x = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.TreeSuccessor(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreeSearchTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int k = 0; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.TreeSearch(k);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreePredecessorTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            BinarySearchTree.Node x = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.TreePredecessor(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreeMinimumTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            BinarySearchTree.Node x = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.TreeMinimum(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreeMaximumTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            BinarySearchTree.Node x = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.TreeMaximum(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreeInsertTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int key = 0; // TODO: Initialize to an appropriate value
            target.TreeInsert(key);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void TreeDeleteTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int key = 0;
            BinarySearchTree.Node delNode = target.TreeSearch(key);
            target.TreeDelete(delNode);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TransplantTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int key1 = 0;
            int key2 = 0;
            BinarySearchTree.Node tNode1 = target.TreeSearch(key1);
            BinarySearchTree.Node tNode2 = target.TreeSearch(key2);
            target.Transplant(tNode1, tNode2);

            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void InorderTreeWalkRoot()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            List<BinarySearchTree.Node> expected = null; // TODO: Initialize to an appropriate value
            List<BinarySearchTree.Node> actual;
            actual = target.InorderTreeWalkRoot();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
