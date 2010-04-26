using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;
using System.Transactions;

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
                    default:
                        throw new ApplicationException("fxClient received wrong type of net message");
                }
            }

            List<DeserializedMsg> fateIncoming = fateClient.IncomingDeserializeAll();
            foreach (var msg in fateIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Commodity:
                        updateCommodity((CommodityMsg)(msg.data));
                        break;
                    case NetMsgType.Stock:
                        updateStock((StockMsg)(msg.data));
                        break;
                    default:
                        throw new ApplicationException("fateIncoming received wrong type of net message");
                }
            }

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
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption
.Required))
            {
                foreach (var item in msg.items)
                {
                    List<SqlParameter> pars = new List<SqlParameter>();
                    pars.Add(new SqlParameter("@CurrencyID", item.id));
                    pars.Add(new SqlParameter("@ValueDate", msg.time));
                    pars.Add(new SqlParameter("@USDValue", item.USDValue));

                    dbConn.StoredProc("procCurrencyUpdate", pars);
                }
                scope.Complete();
            }
        }

        private void updateCommodity(CommodityMsg msg)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption
.Required))
            {
                foreach (var item in msg.items)
                {
                    List<SqlParameter> pars = new List<SqlParameter>();
                    pars.Add(new SqlParameter("@PortID", item.portID));
                    pars.Add(new SqlParameter("@CommodityID", item.commodID));
                    pars.Add(new SqlParameter("@ValueDate", msg.time));
                    pars.Add(new SqlParameter("@LocalPrice", item.localPrice));

                    dbConn.StoredProc("procPortComodUpdate", pars);
                }
                scope.Complete();
            }
        }

        private void updateStock(StockMsg msg)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption
.Required))
            {
                foreach (var item in msg.items)
                {
                    List<SqlParameter> pars = new List<SqlParameter>();
                    pars.Add(new SqlParameter("@CompanyID", item.companyID));
                    pars.Add(new SqlParameter("@PriceDate", msg.time));
                    pars.Add(new SqlParameter("@USDStockPrice", item.price));

                    dbConn.StoredProc("procStockUpdate", pars);
                }
                scope.Complete();
            }
        }
    }
}
