using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;

using Smo = Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

using TaiPan.Common;
using System.IO;

namespace TestTaiPan
{
    [DeploymentItem("Common.config")]
    [DeploymentItem("create_schema.sql")]
    [DeploymentItem("insert_data.sql")]
    public abstract class TestTaiPanBase
    {
        static protected DbConn conn;
        static protected Dictionary<string, int> commodIDs = new Dictionary<string, int>();
        static protected Dictionary<string, int> portIDs = new Dictionary<string, int>();

        protected static void SetupForTests()
        {
            SetGlobals();
            
            DbConn.testDB = true;
            conn = new DbConn(false);

            //reset database
            RunSQLScript("create_schema.sql");
            RunSQLScript("insert_data.sql");

            //save some ids in a dictionary for more readable tests
            SaveTestingIDs();

            //set up some fixed values for PortCommodityPrice:

            //first set value to 0 everywhere
            conn.ExecuteNonQuery("update PortCommodityPrice set LocalPrice = 0");

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
                conn.ExecuteNonQuery(cmd);
            }
        }

        private static void SetGlobals()
        {
            Globals.FUEL_RATE = 0.8M;
            Globals.SHIPPING_COMPANY_RATE = 0.0001M;
            Globals.FREIGHTER_SPEED = 200000;
        }

        private static void SaveTestingIDs()
        {
            SqlCommand portIDCmd = new SqlCommand("select ID from Port where Name = @PName");
            string[] portNames = { "Felixstowe", "Bahía Blanca", "Sydney" };
            foreach (var name in portNames)
            {
                portIDCmd.Parameters.Clear();
                portIDCmd.Parameters.AddWithValue("PName", name);
                int id = (int)conn.ExecuteScalar(portIDCmd);
                portIDs.Add(name, id);
            }

            SqlCommand commodIDCmd = new SqlCommand("select ID from Commodity where Name = @CName");
            string[] commodNames = { "Citrus fruit", "Iron ore" };
            foreach (var name in commodNames)
            {
                commodIDCmd.Parameters.Clear();
                commodIDCmd.Parameters.AddWithValue("CName", name);
                int id = (int)conn.ExecuteScalar(commodIDCmd);
                commodIDs.Add(name, id);
            }
        }

        private static void RunSQLScript(string fname)
        {
            FileInfo file = new FileInfo(fname);
            using (StreamReader sr = file.OpenText())
            {
                string script = file.OpenText().ReadToEnd();
                Smo.Server server = new Smo.Server(new ServerConnection(conn.UnderlyingConnection()));
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

        protected void AssertSeqEqual<T>(List<T> seq1, List<T> seq2)
        {
            //Visual Studio doesn't like multiline error messages, bah
            if (!seq1.SequenceEqual(seq2, new GenericObjectComparer<T>()))
                throw new AssertFailedException("Sequences not equal: " + ListToStr(seq1) + " and " + ListToStr(seq2));
            return;
        }
    }
}
