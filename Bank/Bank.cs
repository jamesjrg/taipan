using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using TaiPan.Common;

namespace TaiPan.Bank
{
    /// <summary>
    /// Singleton class for Bank process
    /// </summary>
    class Bank : TaiPan.Common.EconomicPlayer
    {
        private DbConn dbConn;

        private Client fxPoller;
        private Client commodPoller;
        private Client stockPoller;

        public Bank(string[] args)
        {
            Console.Title = "Bank";

            dbConn = new DbConn(false);

            fxPoller = new Client(ServerConfigs["FXServer-BankBroadcast"], AppSettings);
            Thread thread1 = new Thread(fxPoller.MainLoop);
            thread1.Start();

            commodPoller = new Client(ServerConfigs["FateAndGuessWork-BankCommodBroadcast"], AppSettings);
            Thread thread2 = new Thread(commodPoller.MainLoop);
            thread2.Start();

            stockPoller = new Client(ServerConfigs["FateAndGuessWork-BankStockBroadcast"], AppSettings);
            Thread thread3 = new Thread(stockPoller.MainLoop);
            thread3.Start();
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
