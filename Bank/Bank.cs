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

        private Server traderListener;
        private Server shippingListener;

        private Client fxPoller;
        private Client fatePoller;

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

            traderListener = new Server(ServerConfigs["Bank-Trader"], AppSettings);
            shippingListener = new Server(ServerConfigs["Bank-Shipping"], AppSettings);

            fxPoller = new Client(ServerConfigs["FXServer-Bank"], AppSettings);
            fatePoller = new Client(ServerConfigs["FateAndGuesswork-Bank"], AppSettings);
        }

        protected override bool Run()
        {
            while (fxPoller.incoming.Count != 0)
                Console.WriteLine(fxPoller.incoming.Dequeue());
            while (fatePoller.incoming.Count != 0)
                Console.WriteLine(fatePoller.incoming.Dequeue());

            while (traderListener.incoming.Count != 0)
            {
                string msg = traderListener.incoming.Dequeue();
                NetMsgType type = NetContract.GetNetMsgTypeFromStr(msg);
                object data = NetContract.Deserialize(type, msg);
                switch (type)
                {
                    case NetMsgType.Buy:                        
                        break;
                    case NetMsgType.Future:
                        break;
                    default:
                        throw new ApplicationException("traderListener received wrong type of net message");
                }
                Console.WriteLine(msg);
            }

            while (shippingListener.incoming.Count != 0)
                Console.WriteLine(shippingListener.incoming.Dequeue());

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
