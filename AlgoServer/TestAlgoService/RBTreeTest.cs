using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace TestAlgoService
{
    
    
    /// <summary>
    ///This is a test class for RBTreeTest and is intended
    ///to contain all RBTreeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RBTreeTest
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

        public RBTree BuildTree(TreeAssertData data)
        {
            RBTree tree = new RBTree();

            foreach (var key in data.input)
                tree.Insert(key);

            return tree;
        }

        [TestMethod()]
        public void MinimumTest()
        {
            RBTree target = new RBTree(); // TODO: Initialize to an appropriate value
            RBTree.Node x = null; // TODO: Initialize to an appropriate value
            RBTree.Node expected = null; // TODO: Initialize to an appropriate value
            RBTree.Node actual;
            actual = target.Minimum(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void MaximumTest()
        {
            RBTree target = new RBTree(); // TODO: Initialize to an appropriate value
            RBTree.Node x = null; // TODO: Initialize to an appropriate value
            RBTree.Node expected = null; // TODO: Initialize to an appropriate value
            RBTree.Node actual;
            actual = target.Maximum(x);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void PredecessorTest()
        {
            foreach (var data in assertData)
            {
                RBTree target = BuildTree(data);

                foreach (var pair in data.predecessorTests)
                {
                    //BinarySearchTree.Node actual = target.Predecessor(x);
                }
            }
        }

        [TestMethod()]
        public void SuccessorTest()
        {
            foreach (var data in assertData)
            {
                RBTree target = BuildTree(data);

                foreach (var pair in data.predecessorTests)
                {
                    //BinarySearchTree.Node actual = target.TreePredecessor(x);
                }
            }
        }

        [TestMethod()]
        public void InsertTest()
        {
            
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void DeleteTest()
        {
            RBTree target = new RBTree(); // TODO: Initialize to an appropriate value
            RBTree.Node z = null; // TODO: Initialize to an appropriate value
            target.Delete(z);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
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
    }
}
