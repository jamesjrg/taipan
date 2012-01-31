﻿using TaiPan.Trader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaiPan.Common.NetContract;
using System.Collections.Generic;
using TaiPan.Common;
using System.IO;
using Smo = Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;

namespace TestTaiPan
{
    [TestClass()]
    [DeploymentItem("Common.config")]
    [DeploymentItem("create_schema.sql")]
    [DeploymentItem("insert_data.sql")]
    public class TraderTest
    {
        static DbConn _conn;
        static Dictionary<string,int> commodIDs = new Dictionary<string,int>();
        static Dictionary<string,int> portIDs = new Dictionary<string,int>();

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
            _conn = new DbConn(false);

            //reset database
            RunSQLScript("create_schema.sql");
            RunSQLScript("insert_data.sql");

            //set up some fixed values for PortCommodityPrice:

            //first set value to 0 everywhere
            _conn.ExecuteNonQuery("update PortCommodityPrice set LocalPrice = 0");

            //now some chosen values
            var values = new List<Tuple<int, int, int>>();
            values.Add(new Tuple<int, int, int>(10, portIDs["Felixstowe"], commodIDs["Citrus fruit"]));
            values.Add(new Tuple<int, int, int>(3, portIDs["Felixstowe"], commodIDs["Iron ore"]));
            values.Add(new Tuple<int, int, int>(7, portIDs["Bahía Blanca"], commodIDs["Citrus fruit"]));
            values.Add(new Tuple<int, int, int>(5, portIDs["Bahía Blanca"], commodIDs["Iron ore"]));
            values.Add(new Tuple<int, int, int>(3, portIDs["Sydney"], commodIDs["Citrus fruit"]));
            values.Add(new Tuple<int, int, int>(4, portIDs["Sydney"], commodIDs["Iron ore"]));

            SqlCommand cmd = new SqlCommand("update PortCommodityPrice set LocalPrice = @LPrice where PortID = @PID and CommodityID = @CID");
            foreach (var val in values)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("LPrice", val.Item1);
                cmd.Parameters.AddWithValue("PID", val.Item2);
                cmd.Parameters.AddWithValue("CID", val.Item3);
                _conn.ExecuteNonQuery(cmd);
            }

            //save some ids for reuse in tests

            SqlCommand portIDCmd = new SqlCommand("select ID from Port where Name = @PName");
            string[] portNames = {"Felixstowe",  "Bahía Blanca", "Sydney"};
            foreach (var name in portNames)
            {
                portIDCmd.Parameters.Clear();
                portIDCmd.Parameters.AddWithValue("PName", name);
                int id = (int)_conn.ExecuteScalar(portIDCmd);
                portIDs.Add(name, id);
            }

            SqlCommand commodIDCmd = new SqlCommand("select ID from Commodity where Name = @CName");
            string[] commodNames = {"Citrus fruit",  "Iron ore"};
            foreach (var name in commodNames)
            {
                commodIDCmd.Parameters.Clear();
                commodIDCmd.Parameters.AddWithValue("CName", name);
                int id = (int)_conn.ExecuteScalar(commodIDCmd);
                commodIDs.Add(name, id);
            }
        }

        private static void RunSQLScript(string fname)
        {
            FileInfo file = new FileInfo(fname);
            using (StreamReader sr = file.OpenText())
            {
                string script = file.OpenText().ReadToEnd();
                Smo.Server server = new Smo.Server(new ServerConnection(_conn.UnderlyingConnection()));
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        [TestMethod()]
        [DeploymentItem("Trader.exe")]
        public void DecideSalesTest()
        {
            var args = new string[] {"1"};
            Trader_Accessor target = new Trader_Accessor(args);
            target.warehousedGoods.Add(new Trader_Accessor.WarehousedGood(1, portIDs["Felixstowe"], commodIDs["Citrus fruit"], 10, DateTime.Now));
            target.warehousedGoods.Add(new Trader_Accessor.WarehousedGood(2, portIDs["Felixstowe"], commodIDs["Iron ore"], 10, DateTime.Now));
            target.DecideSales();

            var expected = new List<MoveContractMsg>();
            expected.Add(new MoveContractMsg(portIDs["Felixstowe"], portIDs["Sydney"], 1));
            expected.Add(new MoveContractMsg(portIDs["Felixstowe"], portIDs["Sydney"], 2));
            Assert.IsTrue(target.moveContracts.SequenceEqual(expected));
        }
    }
}