﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Smo = Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;

using TaiPan.Trader;
using TaiPan.Common;
using TaiPan.Common.NetContract;
using System.Collections;
using System.Reflection;

namespace TestTaiPan
{   
    [TestClass()]
    [DeploymentItem("Common.config")]
    [DeploymentItem("create_schema.sql")]
    [DeploymentItem("insert_data.sql")]
    public class TraderLogicTest
    {
        static DbConn _conn;
        static Dictionary<string, int> commodIDs = new Dictionary<string, int>();
        static Dictionary<string, int> portIDs = new Dictionary<string, int>();

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
            DbConn.testDB = true;
            _conn = new DbConn(false);

            //reset database
            RunSQLScript("create_schema.sql");
            RunSQLScript("insert_data.sql");

            //save some ids in a dictionary for more readable tests
            SaveTestingIDs();

            //set up some fixed values for PortCommodityPrice:

            //first set value to 0 everywhere
            _conn.ExecuteNonQuery("update PortCommodityPrice set LocalPrice = 0");

            //now some chosen values
            //note price is in local money, and Argentian peso is $0.25562, GBP is $1.5321, AUD is $0.9277
            var values = new List<Tuple<int, int, int>>();
            values.Add(new Tuple<int, int, int>(10, portIDs["Felixstowe"], commodIDs["Citrus fruit"]));
            values.Add(new Tuple<int, int, int>(4, portIDs["Felixstowe"], commodIDs["Iron ore"]));
            values.Add(new Tuple<int, int, int>(28, portIDs["Bahía Blanca"], commodIDs["Citrus fruit"]));
            values.Add(new Tuple<int, int, int>(25, portIDs["Bahía Blanca"], commodIDs["Iron ore"]));
            values.Add(new Tuple<int, int, int>(10, portIDs["Sydney"], commodIDs["Citrus fruit"]));
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
        }

        static void SaveTestingIDs()
        {
            SqlCommand portIDCmd = new SqlCommand("select ID from Port where Name = @PName");
            string[] portNames = { "Felixstowe", "Bahía Blanca", "Sydney" };
            foreach (var name in portNames)
            {
                portIDCmd.Parameters.Clear();
                portIDCmd.Parameters.AddWithValue("PName", name);
                int id = (int)_conn.ExecuteScalar(portIDCmd);
                portIDs.Add(name, id);
            }

            SqlCommand commodIDCmd = new SqlCommand("select ID from Commodity where Name = @CName");
            string[] commodNames = { "Citrus fruit", "Iron ore" };
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

        //xxx not sure if all the reflective generics in the following methods are considered good form in the .NET world or not
        private string ListToStr<T>(List<T> seq)
        {
            return string.Join(", ", seq.Select(
                x =>
                {
                    Type myType = x.GetType();
                    var fieldInfo = myType.GetFields();

                    string str = "[";
                    foreach (var field in fieldInfo)
                    {
                        str += field.GetValue(x).ToString() + ";";
                    }

                    return str + "]";
                }));
        }

        class GenericObjectComparer<T> : EqualityComparer<T>
        {
            public override bool Equals(T x, T y)
            {
                Type myType = x.GetType();
                var fieldInfo = myType.GetFields();

                foreach (var field in fieldInfo)
                {
                    if (!field.GetValue(x).Equals(field.GetValue(y)))
                        return false;
                }

                return true;
            }

            //xxx meh why I am forced to implement this
            public override int GetHashCode(T x)
            {
                throw new Exception("not implemented");
            }
        }
        
        private void AssertSeqEqual<T>(List<T> seq1, List<T> seq2)
        {
            //Visual Studio doesn't like multiline error messages, bah
            if (!seq1.SequenceEqual(seq2, new GenericObjectComparer<T>()))
                throw new AssertFailedException("Sequences not equal: " + ListToStr(seq1) + " and " + ListToStr(seq2));
            return;
        }

        [TestMethod()]
        public void DecideSalesTest()
        {
            Decimal SHIPPING_COMPANY_RATE = 0.0001M;
            TraderLogic target = new TraderLogic(SHIPPING_COMPANY_RATE);
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
