using TaiPan.Trader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaiPan.Common.NetContract;
using System.Collections.Generic;
using TaiPan.Common;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace TestTaiPan
{
    
    
    /// <summary>
    ///This is a test class for TraderTest and is intended
    ///to contain all TraderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TraderTest
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
        public static void MyClassInitialize(TestContext testContext)
        {
            DbConn.testDB = true;
            var conn = new DbConn(false);
            //xxx

            FileInfo file = new FileInfo(@"xxxx path to create_schema.sql.sql");
            using (StreamReader sr = file.OpenText())
            {
                string script = file.OpenText().ReadToEnd();
                Server server = new Server(new ServerConnection(conn.UnderlyingConnection()));
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            //xxx
            sqlcmd -U taipan-rw -P fakepass -d TaiPan -S (local) -i insert_data.sql
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //xxx
            truncate all database tables
        }

        [TestMethod()]
        [DeploymentItem("Trader.exe")]
        public void DecideSalesTest()
        {
            var args = new string[] {"1"};
            Trader_Accessor target = new Trader_Accessor(args);
            //XXX
            target.warehousedGoods.Add(new Trader_Accessor.WarehousedGood(transactionID, portID, commodityID, quantity, saleTime);
            target.DecideSales();

            //XXX
            var expected = new List<MoveContractMsg>();
            expected.Add(new MoveContractMsg(departureID, destID, transactionID);
            Assert.IsTrue(target.moveContracts.SequenceEqual(expected));
            Assert.Inconclusive("TODO");
        }
    }
}
