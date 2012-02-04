using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.Bank
{
    class BankDBLogic
    {
        private DbConn dbConn;

        public BankDBLogic()
        {
            dbConn = new DbConn(false);

            //truncate some of the tables in the database which don't make sense if not cleared between
            //runs
            dbConn.ExecuteNonQuery("delete from CommodityTransport");
            dbConn.ExecuteNonQuery("delete from CommodityTransaction");
            dbConn.ExecuteNonQuery("delete from FuturesContract;");
        }

        //only public so it can be used as utility by testing code
        public int InsertCommodityTransaction(int traderID, int commodID, int portID, int quantity, decimal amount, int futuresID = 0)
        {
            SqlCommand insertCmd = new SqlCommand(
@"insert into CommodityTransaction
(TraderID, CommodityID, BuyPortID, FuturesContractID, Quantity, PurchasePrice)
VALUES (@traderID, @commodID, @portID, @futuresID, @quantity, @amount);
SELECT ID FROM CommodityTransaction WHERE ID = @@IDENTITY");
            insertCmd.Parameters.AddWithValue("@traderID", traderID);
            insertCmd.Parameters.AddWithValue("@commodID", commodID);
            insertCmd.Parameters.AddWithValue("@portID", portID);
            if (futuresID == 0)
                insertCmd.Parameters.AddWithValue("@futuresID", DBNull.Value);
            else
                insertCmd.Parameters.AddWithValue("@futuresID", futuresID);
            insertCmd.Parameters.AddWithValue("@quantity", quantity);
            insertCmd.Parameters.AddWithValue("@amount", amount);

            return (int)dbConn.ExecuteScalar(insertCmd);
        }

        public void FutureSettlements(List<Bank.ConfirmInfo> settledFutures)
        {
            //find settled futures, insert new records into CommodityTransaction, and make a list of messages to be sent to traders
            //use a data set to store data and then loop through it, can't interleave reads and writes whilst a reader is open
            var dataSet = dbConn.FilledDataSet(
@"SELECT ID, TraderID, CommodityID, PortID, LocalPrice, Quantity
FROM FuturesContract
WHERE ActualSetTime is null AND SettlementTime < GETDATE()");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                int futureID = row.Field<int>("ID");
                int traderID = row.Field<int>("TraderID");
                int commodID = row.Field<int>("CommodityID");
                int portID = row.Field<int>("PortID");
                decimal localPrice = row.Field<decimal>("LocalPrice");
                int quantity = row.Field<int>("Quantity");
                decimal amount = localPrice * quantity;

                int transID = InsertCommodityTransaction(traderID, commodID, portID, quantity, amount, futureID);

                //debit trader's account
                SqlCommand debitCmd = new SqlCommand("procSubtractBalance");
                debitCmd.Parameters.AddWithValue("@CompanyID", traderID);
                debitCmd.Parameters.AddWithValue("@PortID", portID);
                debitCmd.Parameters.AddWithValue("@Amount", amount);
                dbConn.StoredProc(debitCmd);

                settledFutures.Add(new Bank.ConfirmInfo(traderID, portID, commodID, quantity, transID, localPrice));

                var settleCmd = new SqlCommand("UPDATE FuturesContract set ActualSetTime = GETDATE() where ID = @FID");
                settleCmd.Parameters.AddWithValue("FID", futureID);
                dbConn.ExecuteNonQuery(settleCmd);
            }
        }

        public void UpdateCurrency(CurrencyMsg msg)
        {
            foreach (var item in msg.items)
            {
                var cmd = new SqlCommand("procCurrencyUpdate");
                cmd.Parameters.AddWithValue("@CurrencyID", item.id);
                cmd.Parameters.AddWithValue("@ValueDate", msg.time);
                cmd.Parameters.AddWithValue("@USDValue", item.USDValue);
                dbConn.StoredProc(cmd);
            }
        }

        public void UpdateCommodity(CommodityMsg msg)
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
        }

        public void UpdateStock(StockMsg msg)
        {   
            foreach (var item in msg.items)
            {
                SqlCommand cmd = new SqlCommand("procStockUpdate");
                cmd.Parameters.AddWithValue("@CompanyID", item.companyID);
                cmd.Parameters.AddWithValue("@PriceDate", msg.time);
                cmd.Parameters.AddWithValue("@USDStockPrice", item.price);
                dbConn.StoredProc(cmd);
            }
        }

        public void EnactFuture(int traderID, FutureMsg msg)
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

        public void EnactBuy(int traderID, BuyMsg msg, List<Bank.ConfirmInfo> confirmedBuys)
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

            int transID = InsertCommodityTransaction(traderID, msg.commodID, msg.portID, msg.quantity, amount);

            //last, add msg to confirmedBuys
            confirmedBuys.Add(new Bank.ConfirmInfo(traderID, msg.portID, msg.commodID, msg.quantity, transID, amount));
        }

        public void EnactLocalSale(LocalSaleMsg msg)
        {
            var saleCmd = new SqlCommand("procLocalSale");
            saleCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            dbConn.StoredProc(saleCmd);
        }

        public void ShipDeparted(int companyID, MovingMsg msg)
        {
            var shipDepartedCmd = new SqlCommand("procShipDeparted");
            shipDepartedCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            shipDepartedCmd.Parameters.AddWithValue("@ShippingCompanyID", companyID);
            shipDepartedCmd.Parameters.AddWithValue("@DepartTime", msg.time);
            shipDepartedCmd.Parameters.AddWithValue("@DestPort", msg.destPortID);
            dbConn.StoredProc(shipDepartedCmd);
        }

        public void ShipArrived(int companyID, MovingMsg msg)
        {
            var shipArrivedCmd = new SqlCommand("procShipArrived");
            shipArrivedCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            shipArrivedCmd.Parameters.AddWithValue("@ShippingCompanyID", companyID);
            shipArrivedCmd.Parameters.AddWithValue("@ArrivalTime", msg.time);
            shipArrivedCmd.Parameters.AddWithValue("@DepartPort", msg.departPortID);
            shipArrivedCmd.Parameters.AddWithValue("@ArrivalPort", msg.destPortID);
            shipArrivedCmd.Parameters.AddWithValue("@ShippingCompanyRate", Globals.SHIPPING_COMPANY_RATE);
            shipArrivedCmd.Parameters.AddWithValue("@FuelRate", Globals.FUEL_RATE);
            dbConn.StoredProc(shipArrivedCmd);

            var saleCmd = new SqlCommand("procCommoditySale");
            saleCmd.Parameters.AddWithValue("@CommodityTransactionID", msg.transactionID);
            saleCmd.Parameters.AddWithValue("@SalePortID", msg.destPortID);
            dbConn.StoredProc(saleCmd);
        }
    }
}
