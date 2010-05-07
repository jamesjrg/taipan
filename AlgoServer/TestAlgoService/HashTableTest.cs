using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestAlgoService
{
    [TestClass()]
    public class HashTableTest
    {
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


        [TestMethod()]
        public void SearchTest()
        {
            HashTable target = new HashTable();
            int key = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Search(key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void InsertTest()
        {
            HashTable target = new HashTable(); // TODO: Initialize to an appropriate value
            int key = 0; // TODO: Initialize to an appropriate value
            int val = 0; // TODO: Initialize to an appropriate value
            target.Insert(key, val);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        [DeploymentItem("AlgoService.dll")]
        public void HashTest()
        {
            HashTable_Accessor target = new HashTable_Accessor(); // TODO: Initialize to an appropriate value
            int key = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Hash(key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void DeleteTest()
        {
            HashTable target = new HashTable(); // TODO: Initialize to an appropriate value
            int key = 0; // TODO: Initialize to an appropriate value
            target.Delete(key);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
