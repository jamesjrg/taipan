using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using TaiPan.Common;

namespace TaiPan.Bank
{
    class Bank : TaiPan.Common.EconomicPlayer
    {
        private DbConn dbConn;
        private Client fxPoller;

        public Bank(string[] args)
        {
            Console.Title = "Bank";

            dbConn = new DbConn(false);

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
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT Name FROM Company");
            while (reader.Read())
                Console.WriteLine("{0}", reader.GetString(0));
            reader.Close();

            dbConn.ExecuteNonQuery("UPDATE Currency SET USDValue = 10 WHERE ID = 1");
        }        
    }
}
