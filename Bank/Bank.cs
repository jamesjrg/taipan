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

        private Server traderServer;
        private Server shippingServer;

        private Client fxClient;
        private Client fateClient;

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

            traderServer = new Server(ServerConfigs["Bank-Trader"], AppSettings, false);
            shippingServer = new Server(ServerConfigs["Bank-Shipping"], AppSettings, false);

            fxClient = new Client(ServerConfigs["FXServer-Bank"], AppSettings, 1, true);
            fateClient = new Client(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, 1, true);
        }

        protected override bool Run()
        {
            List<DeserializedMsg> fxIncoming = fxClient.IncomingDeserializeAll();
            foreach (var msg in fxIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Currency:
                        updateCurrency((CurrencyMsg)(msg.data));
                        break;
                    case NetMsgType.Commodity:
                        updateCommodity((CommodityMsg)(msg.data));
                        break;
                    case NetMsgType.Stock:
                        updateStock((StockMsg)(msg.data));
                        break;
                    default:
                        throw new ApplicationException("fxClient received wrong type of net message");
                }
            }

            List<DeserializedMsg> fateIncoming = fateClient.IncomingDeserializeAll();

            List<DeserializedMsg> traderIncoming = traderServer.IncomingDeserializeAll();
            foreach (var msg in traderIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Buy:                   
                        break;
                    case NetMsgType.Future:
                        break;
                    default:
                        throw new ApplicationException("traderServer received wrong type of net message");
                }
            }

            while (shippingServer.incoming.Count != 0)
                Console.WriteLine(shippingServer.incoming.Dequeue());

            return true;
        }

        private void updateCurrency(CurrencyMsg msg)
        {
            dbConn.ExecuteNonQuery(String.Format("UPDATE Currency SET USDValue = {0} WHERE ID = {1}", msg.USDValue, msg.id));
        }

        private void updateCommodity(CommodityMsg msg)
        {
            dbConn.ExecuteNonQuery(String.Format("UPDATE PortCommodityPrice SET LocalPrice = {0} WHERE PortID = {1} and CommodityID = {2}", msg.localPrice, msg.portId, msg.commodId));
        }

        private void updateStock(StockMsg msg)
        {
            dbConn.ExecuteNonQuery(String.Format("UPDATE ShippingCompany SET USDStockPrice = {0} WHERE CompanyID = {1}", msg.price, msg.companyId));
        }
    }
}
