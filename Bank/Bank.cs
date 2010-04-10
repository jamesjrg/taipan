using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using Util = TaiPan.Common.Util;
using Db = TaiPan.Common.Db;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TaiPan.Bank
{
    class Bank : TaiPan.Common.EconomicPlayer
    {
        private SqlConnection conn;
        private TcpClient fxPoller;
        private NetworkStream ns;
        private StreamReader sr;

        protected override void Init(string[] args)
        {
            Console.Title = "Bank";

            conn = Db.GetDbConnectionRW();
            conn.Open();

            fxPoller = AttemptTCPConnect("localhost", 6100, "FXServer");
            
            ns = fxPoller.GetStream();
            sr = new StreamReader(ns);

            Thread thread = new Thread(FXPoll);
            thread.Start();
        }

        protected override bool Run()
        {   
            return true;
        }

        private void DbTest()
        {
            string select = "SELECT Name FROM Company";
            SqlCommand cmd = new SqlCommand(select, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("{0}",
            reader.GetString(0));
            reader.Close();

            string update = "UPDATE Currency SET USDValue = 10 WHERE ID = 1";
            cmd = new SqlCommand(update, conn);
            cmd.ExecuteNonQuery();
        }

        protected override void Shutdown()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
            Util.CloseTcpClient(fxPoller);
        }

        private void FXPoll()
        {
            while (true)
            {
                int result = sr.Read();
                Console.WriteLine((char)result);
            }
        }
    }
}
