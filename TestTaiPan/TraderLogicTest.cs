﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

using TaiPan.Trader;
using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TestTaiPan
{   
    [TestClass()]
    public class TraderLogicTest : TestTaiPanBase
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

        [ClassInitialize()]
        public static void TestClassInitialize(TestContext testContext)
        {
            SetupForTests();   
        }

        [TestMethod()]
        public void DecideSalesTest()
        {
            TraderLogic target = new TraderLogic();
            target.AddGood(1, portIDs["Sydney"], commodIDs["Iron ore"], 10);
            target.AddGood(2, portIDs["Felixstowe"], commodIDs["Citrus fruit"], 10);

            var moveContracts = new List<MoveContractMsg>();
            var localSales = new List<LocalSaleMsg>();
            
            target.DecideSales(moveContracts, localSales);

            var expectedMoves = new List<MoveContractMsg>();
            var expectedLocalSales = new List<LocalSaleMsg>();
            expectedMoves.Add(new MoveContractMsg(portIDs["Sydney"], portIDs["Bahía Blanca"], 1));
            expectedLocalSales.Add(new LocalSaleMsg(2));

            AssertSeqEqual(moveContracts, expectedMoves);
            AssertSeqEqual(localSales, expectedLocalSales);
        }
    }
}
