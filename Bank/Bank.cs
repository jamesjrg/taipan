using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using Db = TaiPan.Common.Db;
using TaiPan.Common;

namespace TaiPan.Bank
{
    class Bank : TaiPan.Common.EconomicPlayer
    {
        private SqlConnection dbConn;
        private Client fxPoller;

        protected override void Init(string[] args)
        {
            Console.Title = "Bank";

            dbConn = Db.GetDbConnectionRW();
            dbConn.Open();

            fxPoller = new Client(serverConfigs["FXServer-BankBroadcast"]);
            Thread thread = new Thread(fxPoller.MainLoop);
            thread.Start();
        }

        protected override bool Run()
        {
            while (fxPoller.messages.Count != 0)
                Console.WriteLine(fxPoller.messages.Dequeue());
            return true;
        }

        private void DbTest()
        {
            string select = "SELECT Name FROM Company";
            SqlCommand cmd = new SqlCommand(select, dbConn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("{0}", reader.GetString(0));
            reader.Close();

            string update = "UPDATE Currency SET USDValue = 10 WHERE ID = 1";
            cmd = new SqlCommand(update, dbConn);
            cmd.ExecuteNonQuery();
        }

        protected override void Dispose(bool disposing)
        {
            Db.CloseConn(dbConn);
            fxPoller.Dispose();
        }
    }
}
