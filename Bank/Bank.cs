using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.Bank
{
    /// <summary>
    /// Singleton class for Bank process
    /// </summary>
    class Bank : TaiPan.Common.EconomicPlayer
    {
        private DbConn dbConn;

        private Server traderBroadcast;

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

            traderBroadcast = new Server(ServerConfigs["Bank-TraderBroadcast"], AppSettings);

            fxPoller = new Client(ServerConfigs["FXServer-BankBroadcast"], AppSettings);
            fatePoller = new Client(ServerConfigs["FateAndGuessWork-BankBroadcast"], AppSettings);

            var conf = ServerConfigs["Trader-BankBroadcast"];
            for (int i = 0; i != nTraders; ++i)
            {
                traderPollers.Add(new Client(conf, AppSettings));
                conf.port += 1;
            }

            conf = ServerConfigs["Shipping-BankBroadcast"];
            for (int i = 0; i != nShipping; ++i)
            {
                traderPollers.Add(new Client(conf, AppSettings));
                conf.port += 1;
            }
        }

        protected override bool Run()
        {
            while (fxPoller.messages.Count != 0)
                Console.WriteLine(fxPoller.messages.Dequeue());
            while (fatePoller.messages.Count != 0)
                Console.WriteLine(fxPoller.messages.Dequeue());

            foreach (var traderPoller in traderPollers)
            {
                while (traderPoller.messages.Count != 0)
                {
                    string msg = traderPoller.messages.Dequeue();
                    switch (NetContract.GetNetMsgType(msg))
                    {
                        case NetMsgType.TraderToBankBuy:
                            NetContract.DecodeBuy(msg);
                            break;
                        case NetMsgType.TraderToBankFuture:
                            NetContract.DecodeFuture(msg);
                            break;
                        default:
                            throw new ApplicationException("traderPoller received wrong type of net message");
                    }
                    Console.WriteLine(msg);
                }                 
            }

            foreach (var shippingPoller in shippingPollers)
            {
                while (shippingPoller.messages.Count != 0)
                    Console.WriteLine(shippingPoller.messages.Dequeue());
            }

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
