using TaiPan.Bank;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using TaiPan.Common.NetContract;

namespace TestTaiPan
{   
    [TestClass()]
    public class BankDBLogicTest : TestTaiPanBase
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
        public void ShipDepartedTest()
        {
            BankDBLogic target = new BankDBLogic();

            int traderID = 1;
            int commodID = 5;
            int shippingCoID = 6;
            int departPortID = 2;
            int destPortID = 3;
            int quantity = 10;
            decimal amount = 10;
            DateTime time = DateTime.Now;

            int transID = target.InsertCommodityTransaction(traderID, commodID, departPortID, quantity, amount);
            target.ShipDeparted(shippingCoID, new MovingMsg(departPortID, destPortID, transID, time));

            //check CommodityTransport
            var dataset = conn.FilledDataSet("SELECT ShippingCompanyID, CommodityTransactionID, DepartureTime FROM CommodityTransport");
            object[] expectedRow = { 6, transID, time };
            Assert.AreEqual(1, dataset.Tables[0].Rows.Count);
            AssertSeqEqual(expectedRow, dataset.Tables[0].Rows[0].ItemArray);

            //xxx check CommodityTransaction updated
            dataset = conn.FilledDataSet("SELECT ID, SalePortID FROM CommodityTransaction");
            object[] expectedTransportRow = { transID, destPortID };
            Assert.AreEqual(1, dataset.Tables[0].Rows.Count);
            AssertSeqEqual(expectedTransportRow, dataset.Tables[0].Rows[0].ItemArray);
        }

        [TestMethod()]
        public void ShipArrivedTest()
        {
            BankDBLogic target = new BankDBLogic();
            //target.ShipArrived(companyID, msg);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
