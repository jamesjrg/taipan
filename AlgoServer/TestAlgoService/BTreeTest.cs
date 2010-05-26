using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            int id = 0; // TODO: Initialize to an appropriate value
            bool truncate = false; // TODO: Initialize to an appropriate value
            BTree target = new BTree(id, truncate);
            Assert.Inconclusive("TODO: Implement code to verify target");
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
            int id = 0; // TODO: Initialize to an appropriate value
            bool truncate = false; // TODO: Initialize to an appropriate value
            BTree target = new BTree(id, truncate); // TODO: Initialize to an appropriate value
            int k = 0; // TODO: Initialize to an appropriate value
            target.Insert(k);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
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
    }
}
