﻿using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestAlgoService
{
    
    
    /// <summary>
    ///This is a test class for SortAlgsTest and is intended
    ///to contain all SortAlgsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SortAlgsTest
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
        ///A test for MergeSort
        ///</summary>
        [TestMethod()]
        public void MergeSortTest()
        {
            int[] data = null; // TODO: Initialize to an appropriate value
            int[] expected = null; // TODO: Initialize to an appropriate value
            SortReturn actual;
            actual = SortAlgs.MergeSort(data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for QuickSort
        ///</summary>
        [TestMethod()]
        public void QuickSortTest()
        {
            int[] data = null; // TODO: Initialize to an appropriate value
            int[] expected = null; // TODO: Initialize to an appropriate value
            SortReturn actual;
            actual = SortAlgs.QuickSort(data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
