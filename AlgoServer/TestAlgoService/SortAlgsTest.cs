using AlgoService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
namespace TestAlgoService
{
    [TestClass()]
    public class SortAlgsTest
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

        private class AssertData
        {
            public AssertData(int[] input, int[] expected)
            {
                this.input = input;
                this.expected = expected;
            }

            public int[] InputClone()
            {
                return (int[])input.Clone();
            }

            private int[] input;
            public int[] expected;
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
            int[] input;
            int[] expected;

            //empty
            assertData.Add(new AssertData(new int[0], new int[0]));

            //single number
            assertData.Add(new AssertData(new int[1] { 1 }, new int[1] { 1 }));

            //already sorted
            input = new int[]{1,2,3,4,5};
            expected = new int[]{1,2,3,4,5};
            assertData.Add(new AssertData(input, expected));

            //backwards
            input = new int[] { 5, 4, 3, 2, 1 };
            expected = new int[] { 1, 2, 3, 4, 5 };
            assertData.Add(new AssertData(input, expected));

            //lots of mixed up numbers
            input = new int[] { 2, 3, 43, 52, 31, 2, 453, 5363, 35, 13, 354, 293, 2394, 283, 2134, 33, 05, 385, 283, 9, 0, 3, 44, 2203, 2203, 2203 };
            expected = new int[] { 0, 2, 2, 3, 3, 5, 9, 13, 31, 33, 35, 43, 44, 52, 283, 283, 293, 354, 385, 453,
2134, 2203, 2203, 2203, 2394, 5363 };
            assertData.Add(new AssertData(input, expected));

            //could do negative numbers?
        }

        private bool ArraysEqual(int[] a1, int[] a2)
        {
            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i != a1.Length; ++i)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }

        private string ArrayToString(int[] array)
        {
            string[] tmp = new string[array.Length];
            for (int i = 0; i != array.Length; ++i)
                tmp[i] = array[i].ToString();
            return String.Join(", ", tmp);

        }

        [TestMethod()]
        public void InsertionSortTest()
        {
            foreach (var data in assertData)
            {
                int[] theArray = (int[])data.InputClone();
                SortAlgs.InsertionSort(theArray);
                Assert.IsTrue(ArraysEqual(data.expected, theArray));
            }
        }

        [TestMethod()]
        public void MergeSortTest()
        {
            foreach (var data in assertData)
            {
                int[] theArray = (int[])data.InputClone();
                SortAlgs.MergeSort(theArray);
                Assert.IsTrue(ArraysEqual(data.expected, theArray), ArrayToString(data.expected));
            }
        }

        [TestMethod()]
        public void HeapSortTest()
        {
            foreach (var data in assertData)
            {
                int[] theArray = (int[])data.InputClone();
                SortAlgs.HeapSort(theArray);
                Assert.IsTrue(ArraysEqual(data.expected, theArray), ArrayToString(data.expected));
            }
        }

        [TestMethod()]
        public void QuickSortTest()
        {
            foreach (var data in assertData)
            {
                int[] theArray = (int[])data.InputClone();
                SortAlgs.QuickSort(theArray);
                Assert.IsTrue(ArraysEqual(data.expected, theArray), ArrayToString(data.expected));
            }
        }

        [TestMethod()]
        public void RandomizedQuickSortTest()
        {
            foreach (var data in assertData)
            {
                int[] theArray = (int[])data.InputClone();
                SortAlgs.RandomizedQuickSort(theArray);
                Assert.IsTrue(ArraysEqual(data.expected, theArray), ArrayToString(data.expected));
            }
        }
    }
}
