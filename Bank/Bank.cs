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

        private List<ConfirmInfo> confirmedBuys = new List<ConfirmInfo>();
        private List<ConfirmInfo> settledFutures = new List<ConfirmInfo>();

        private class ConfirmInfo
        {
            public ConfirmInfo(int traderID, int portID, int commodID, int quantity, int warehouseID)
            {
                this.traderID = traderID;
                this.msg = new BankConfirmMsg(portID, commodID, quantity, warehouseID);
            }

            public BankConfirmMsg msg;
            public int traderID;
        }

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

            //truncate some of the tables in the database which don't make sense if not cleared between
            //runs
                dbConn.ExecuteNonQuery("delete from CommodityTransport");
                dbConn.ExecuteNonQuery("delete from WarehousedCommodity");
                dbConn.ExecuteNonQuery("delete from FuturesContract;");

            traderServer = new Server(ServerConfigs["Bank-Trader"], AppSettings, false);
            shippingServer = new Server(ServerConfigs["Bank-Shipping"], AppSettings, false);

            fxClient = new Client(ServerConfigs["FXServer-Bank"], AppSettings, 1, true);
            fateClient = new Client(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, 1, true);
        }

        protected override bool Run()
        {
            FutureSettlements();

            List<DeserializedMsg> fxIncoming = fxClient.IncomingDeserializeAll();
            foreach (var msg in fxIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Currency:
                        UpdateCurrency((CurrencyMsg)(msg.data));
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
                        UpdateCommodity((CommodityMsg)(msg.data));
                        break;
                    case NetMsgType.Stock:
                        UpdateStock((StockMsg)(msg.data));
                        break;
                    default:
                        throw new ApplicationException("fateIncoming received wrong type of net message");
                }
            }

            foreach (var client in traderServer.clients)
            {
                List<DeserializedMsg> traderIncoming = traderServer.IncomingDeserializeAll(client.id);
                foreach (var msg in traderIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.Buy:
                            EnactBuy(client.id, (BuyMsg)msg.data);
                            break;
                        case NetMsgType.Future:
                            EnactFuture(client.id, (FutureMsg)msg.data);
                            break;
                        default:
                            throw new ApplicationException("traderServer received wrong type of net message");
                    }
                }
            }

            foreach (var client in shippingServer.clients)
            {
                List<DeserializedMsg> shippingIncoming = shippingServer.IncomingDeserializeAll(client.id);
                foreach (var msg in shippingIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.Departure:
                            ShipDeparted(client.id, (MovingMsg)msg.data);
                            break;
                        case NetMsgType.Arrival:
                            ShipArrived(client.id, (MovingMsg)msg.data);
                            break;
                        default:
                            throw new ApplicationException("traderServer received wrong type of net message");
                    }
                }
            }

            foreach (var info in confirmedBuys)
                traderServer.Send(NetContract.Serialize(NetMsgType.BuyConfirm, info.msg), info.traderID);
            confirmedBuys.Clear();

            foreach (var info in settledFutures)
                traderServer.Send(NetContract.Serialize(NetMsgType.FutureSettle, info.msg), info.traderID);
            settledFutures.Clear();

            return true;
        }

        private void FutureSettlements()
        {
        }

        private void UpdateCurrency(CurrencyMsg msg)
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

        private void UpdateCommodity(CommodityMsg msg)
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

        private void UpdateStock(StockMsg msg)
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

        private void EnactFuture(int traderID, FutureMsg msg)
        {
            dbConn.ExecuteNonQuery(String.Format(@"INSERT INTO dbo.FuturesContract
           (TraderID, CommodityID, PortID, LocalPrice, Quantity, PurchaseTime, SettlementTime)
     VALUES
     ({0}, {1}, {2},
(select LocalPrice from PortCommodityPrice where PortID = {2} and CommodityID = {1}),
{3}, '{4}', '{5}')", traderID, msg.commodID, msg.portID, msg.quantity, DateTime.Now, msg.time));
        }

        private void EnactBuy(int traderID, BuyMsg msg)
        {
            //throw new NotImplementedException();
        }

        private void ShipDeparted(int companyID, MovingMsg msg)
        {
        }

        private void ShipArrived(int companyID, MovingMsg msg)
        {
        }
    }
}
