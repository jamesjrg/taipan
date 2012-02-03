using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;
using System.Transactions;
using System.Data;

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

        private Dictionary<string, int> portDistances;

        private class ConfirmInfo
        {
            public ConfirmInfo(int traderID, int portID, int commodID, int quantity, int transactionID, decimal localPrice)
            {
                this.traderID = traderID;
                this.msg = new BankConfirmMsg(portID, commodID, quantity, transactionID, localPrice);
            }

            public BankConfirmMsg msg;
            public int traderID;
        }

        public Bank(string[] args)
        {
            Util.SetConsoleTitle("Bank");

            dbConn = new DbConn(false);

            //truncate some of the tables in the database which don't make sense if not cleared between
            //runs
            dbConn.ExecuteNonQuery("delete from CommodityTransport");
            dbConn.ExecuteNonQuery("delete from CommodityTransaction");
            dbConn.ExecuteNonQuery("delete from FuturesContract;");

            portDistances = Util.GetPortDistancesLookup(dbConn);

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
            //find settled futures, and add to list which will be messaged to traders
            SqlDataReader reader = dbConn.ExecuteQuery(@"SELECT ID, TraderID, CommodityID, PortID, LocalPrice, Quantity FROM FuturesContract WHERE ActualSetTime is null AND SettlementTime < GETDATE()");
            while (reader.Read())
            {
                int futureID = reader.GetInt32(0);
                int traderID = reader.GetInt32(1);
                int commodID = reader.GetInt32(2);
                int portID = reader.GetInt32(3);
                decimal localPrice = reader.GetDecimal(4);
                int quantity = reader.GetInt32(5);

                settledFutures.Add(new ConfirmInfo(traderID, portID, commodID, quantity, futureID, localPrice));
            }
            reader.Close();

            if (settledFutures.Count > 0)
            {
                StringBuilder futureIDs = new StringBuilder();
                foreach (var future in settledFutures)
                    futureIDs.Append(future.msg.transactionID + ",");
                //remove trailing comma
                futureIDs.Remove(futureIDs.Length - 1, 1);

                //XXX should do this without string interpolation, though in .NET there is no simple method
                dbConn.ExecuteNonQuery(String.Format("UPDATE FuturesContract set ActualSetTime = GETDATE() where ID in ({0})", futureIDs.ToString()));
                
                //debit trader's account
                foreach (var future in settledFutures)
                {
                    SqlCommand cmd2 = new SqlCommand("procSubtractBalance");
                    cmd2.Parameters.AddWithValue("@CompanyID", future.traderID);
                    cmd2.Parameters.AddWithValue("@PortID", future.msg.portID);
                    cmd2.Parameters.AddWithValue("@Amount", future.msg.localPrice * future.msg.quantity);
                    dbConn.StoredProc(cmd2);
                }
            }
        }

        private void UpdateCurrency(CurrencyMsg msg)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption
.Required))
            {
                foreach (var item in msg.items)
                {
                    var cmd = new SqlCommand("procCurrencyUpdate");
                    cmd.Parameters.AddWithValue("@CurrencyID", item.id);
                    cmd.Parameters.AddWithValue("@ValueDate", msg.time);
                    cmd.Parameters.AddWithValue("@USDValue", item.USDValue);
                    dbConn.StoredProc(cmd);
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
                    var cmd = new SqlCommand("procPortComodUpdate");
                    cmd.Parameters.AddWithValue("@PortID", item.portID);
                    cmd.Parameters.AddWithValue("@CommodityID", item.commodID);
                    cmd.Parameters.AddWithValue("@ValueDate", msg.time);
                    cmd.Parameters.AddWithValue("@LocalPrice", item.localPrice);
                    dbConn.StoredProc(cmd);
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
                    SqlCommand cmd = new SqlCommand("procStockUpdate");
                    cmd.Parameters.AddWithValue("@CompanyID", item.companyID);
                    cmd.Parameters.AddWithValue("@PriceDate", msg.time);
                    cmd.Parameters.AddWithValue("@USDStockPrice", item.price);
                    dbConn.StoredProc(cmd);
                }
                scope.Complete();
            }
        }

        private void EnactFuture(int traderID, FutureMsg msg)
        {
            var cmd = new SqlCommand(@"
INSERT INTO dbo.FuturesContract
(TraderID, CommodityID, PortID, LocalPrice, Quantity, PurchaseTime, SettlementTime)
VALUES
(@TraderID, @CommodID, @PortID,
(select LocalPrice from PortCommodityPrice where PortID = @PortID and CommodityID = @commodID),
@Quantity, GETDATE(), @Time)");
            cmd.Parameters.AddWithValue("@TraderID", traderID);
            cmd.Parameters.AddWithValue("@CommodID", msg.commodID);
            cmd.Parameters.AddWithValue("@PortID", msg.portID);
            cmd.Parameters.AddWithValue("@Quantity", msg.quantity);
            cmd.Parameters.AddWithValue("@Time", msg.time);
            dbConn.ExecuteNonQuery(cmd);
        }

        private void EnactBuy(int traderID, BuyMsg msg)
        {
            //first, debit trader's account
            SqlCommand amountCmd = new SqlCommand("select (LocalPrice * @quantity) from PortCommodityPrice where PortID = @portID and CommodityID = @commodID;");
            amountCmd.Parameters.AddWithValue("@quantity", msg.quantity);
            amountCmd.Parameters.AddWithValue("@portID", msg.portID);
            amountCmd.Parameters.AddWithValue("@commodID", msg.commodID);
            decimal amount = (decimal)dbConn.ExecuteScalar(amountCmd);

            SqlCommand subtractCmd = new SqlCommand("procSubtractBalance");
            subtractCmd.Parameters.AddWithValue("@CompanyID", traderID);
            subtractCmd.Parameters.AddWithValue("@PortID", msg.portID);
            subtractCmd.Parameters.AddWithValue("@Amount", amount);
            dbConn.StoredProc(subtractCmd);

            //next, create CommodityTransaction
            SqlCommand insertCmd = new SqlCommand(
@"insert into CommodityTransaction
(TraderID, CommodityID, BuyPortID, Quantity, PurchasePrice, PurchaseTime)
VALUES (@traderID, @commodID, @portID, @quantity, @amount, GETDATE());
SELECT ID FROM CommodityTransaction WHERE ID = @@IDENTITY");
            insertCmd.Parameters.AddWithValue("@traderID", traderID);
            insertCmd.Parameters.AddWithValue("@commodID", msg.commodID);
            insertCmd.Parameters.AddWithValue("@portID", msg.portID);
            insertCmd.Parameters.AddWithValue("@quantity", msg.quantity);
            insertCmd.Parameters.AddWithValue("@amount", amount);

            int transID = (int)dbConn.ExecuteScalar(insertCmd);

            //last, add msg to confirmedBuys
            confirmedBuys.Add(new ConfirmInfo(traderID, msg.portID, msg.commodID, msg.quantity, transID, amount));
        }

        private void ShipDeparted(int companyID, MovingMsg msg)
        {
            var cmd = new SqlCommand("insert into CommodityTransport (ShippingCompanyID, CommodityTransactionID, DepartureTime) VALUES (@CompanyID, @TransactionID, @Time)");
            cmd.Parameters.AddWithValue("@CompanyID", companyID);
            cmd.Parameters.AddWithValue("@TransactionID", msg.transactionID);
            cmd.Parameters.AddWithValue("@Time", msg.time);
            dbConn.ExecuteNonQuery(cmd);
        }

        private void ShipArrived(int companyID, MovingMsg msg)
        {
            //calculate fuel charge paid by shipping company and rate charged by shipping company to trader
            int distance = portDistances[msg.departPortID + "," + msg.destPortID];
            decimal fuelCost = FUEL_COST * distance;
            decimal shippingCompanyCharge = fuelCost * SHIPPING_COMPANY_RATE;

            var shipArrivedCmd = new SqlCommand("procShipArrived");
            shipArrivedCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            shipArrivedCmd.Parameters.AddWithValue("@ShippingCompanyID", companyID);
            shipArrivedCmd.Parameters.AddWithValue("@ArrivalTime", msg.time);
            shipArrivedCmd.Parameters.AddWithValue("@ShippingCompanyCharge", shippingCompanyCharge);
            shipArrivedCmd.Parameters.AddWithValue("@FuelCost", fuelCost);
            dbConn.StoredProc(shipArrivedCmd);

            var saleCmd = new SqlCommand("procCommoditySale");
            saleCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            saleCmd.Parameters.AddWithValue("@SalePortID", msg.destPortID);
            dbConn.StoredProc(saleCmd);
        }
    }
}
