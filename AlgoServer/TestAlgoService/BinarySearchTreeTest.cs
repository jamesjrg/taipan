using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestAlgoService
{
    [TestClass()]
    public class BinarySearchTreeTest
    {
        private static List<TreeAssertData> assertData = new List<TreeAssertData>();

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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            assertData = Util.CreateTreeAssertData();
        }

        public BinarySearchTree BuildTree(TreeAssertData data)
        {
            BinarySearchTree tree = new BinarySearchTree();

            foreach (var key in data.input)
                tree.Insert(key);

            return tree;
        }

        [TestMethod()]
        public void PredecessorTest()
        {
            foreach (var data in assertData)
            {
                BinarySearchTree target = BuildTree(data);

                foreach (var pair in data.predecessorTests)
                {
                    //BinarySearchTree.Node actual = target.Predecessor(x);
                }                
            }
        }

        [TestMethod()]
        public void SuccessorTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            BinarySearchTree.Node x = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.Successor(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void SearchTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int k = 0; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node expected = null; // TODO: Initialize to an appropriate value
            BinarySearchTree.Node actual;
            actual = target.Search(k);
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
            actual = target.Minimum(x);
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
            actual = target.Maximum(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void TreeInsertTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int key = 0; // TODO: Initialize to an appropriate value
            target.Insert(key);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void DeleteTest()
        {
            BinarySearchTree target = new BinarySearchTree(); // TODO: Initialize to an appropriate value
            int key = 0;
            BinarySearchTree.Node delNode = target.Search(key);
            target.Delete(delNode);
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
