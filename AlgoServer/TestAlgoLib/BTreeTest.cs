using AlgoLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestAlgoService
{
    [TestClass()]
    public class BTreeTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        [TestMethod()]
        public void BTreeConstructorTest()
        {
            //check it can be closed and reopened again
            int id = 1;
            int key = 2;

            BTree target = new BTree(id, true);
            target.Insert(key);
            target.Dispose();
            target = new BTree(id, false);
            Assert.AreEqual("I:0:K:3", target.Search(key).ToString());

            //ensure truncation works
            target = new BTree(id, true);
            Assert.AreEqual(null, target.Search(key));
        }

        [TestMethod()]
        public void DeleteTest()
        {
            int id = 0; // TODO: Initialize to an appropriate value
            bool truncate = false; // TODO: Initialize to an appropriate value
            BTree target = new BTree(id, truncate); // TODO: Initialize to an appropriate value
            int k = 0; // TODO: Initialize to an appropriate value
            target.Delete(k);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void InsertTest()
        {
            int id = 1;
            int firstKey = 1;

            BTree target = new BTree(id, true);

            //simple
            target.Insert(firstKey);
            Assert.AreEqual("I:0:K:1;", target.Search(firstKey).ToString());

            //max keys in a single node
            for (int i = 1; i != BTree.MAX_KEYS; ++i)
                target.Insert(i + 1);
            Assert.AreEqual("I:0:K:1,2,3,4,5,6,7;", target.Dump());

            //and one more
            target.Insert(BTree.MAX_KEYS + 1);
            Assert.AreEqual("I:0:K:1,2,3;  I:1:K:4;  I:2:K:5,6,7,8;", target.Dump());
        }

        [TestMethod()]
        public void SearchTest()
        {
            int id = 0; // TODO: Initialize to an appropriate value
            bool truncate = false; // TODO: Initialize to an appropriate value
            BTree target = new BTree(id, truncate); // TODO: Initialize to an appropriate value
            int k = 0; // TODO: Initialize to an appropriate value
            BTree.NodeIndexPair expected = null; // TODO: Initialize to an appropriate value
            BTree.NodeIndexPair actual;
            actual = target.Search(k);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BTree Constructor
        ///</summary>
        [TestMethod()]
        public void BTreeConstructorTest1()
        {
            int id = 0; // TODO: Initialize to an appropriate value
            bool newOrTruncate = false; // TODO: Initialize to an appropriate value
            BTree target = new BTree(id, newOrTruncate);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
