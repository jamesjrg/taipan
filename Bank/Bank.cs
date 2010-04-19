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
        private Client fatePoller;
        private List<Client> traderPollers = new List<Client>();
        private List<Client> shippingPollers = new List<Client>();

        public Bank(string[] args)
        {
            Console.Title = "Bank";

            int nTraders, nShipping;
            try
            {
                nTraders = Int32.Parse(args[0]);
                nShipping = Int32.Parse(args[1]);
            }
            catch (Exception)
            {
                throw new ApplicationException("Requires 2 command line arguments: first is number of traders, second is number of shipping companies");
            }

            dbConn = new DbConn(false);

            fxPoller = new Client(ServerConfigs["FXServer-BankBroadcast"], AppSettings);
            fatePoller = new Client(ServerConfigs["FateAndGuessWork-BankBroadcast"], AppSettings);
            traderPollers.Add(new Client(ServerConfigs["Trader-BankBroadcast"], AppSettings));
            shippingPollers.Add(new Client(ServerConfigs["Shipping-BankBroadcast"], AppSettings));
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
