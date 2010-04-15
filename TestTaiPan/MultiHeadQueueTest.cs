using TaiPan.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestTaiPan
{
    
    
    /// <summary>
    ///This is a test class for MultithreadQueueTest and is intended
    ///to contain all MultithreadQueueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultiHeadQueueTest
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


        /// <summary>
        ///A test for Dequeue
        ///</summary>
        [TestMethod()]
        public void DequeueAllTest()
        {
            int size = 0; // TODO: Initialize to an appropriate value
            MultiHeadQueue target = new MultiHeadQueue(size); // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            int myId = 1;
            actual = target.DequeueAll(myId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MultithreadQueue Constructor
        ///</summary>
        [TestMethod()]
        public void MultithreadQueueConstructorTest()
        {
            int size = 0; // TODO: Initialize to an appropriate value
            MultiHeadQueue target = new MultiHeadQueue(size);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Enqueue
        ///</summary>
        [TestMethod()]
        public void EnqueueTest()
        {
            int size = 0; // TODO: Initialize to an appropriate value
            MultiHeadQueue target = new MultiHeadQueue(size); // TODO: Initialize to an appropriate value
            target.Enqueue();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
