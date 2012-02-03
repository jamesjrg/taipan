using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.Trader
{
    class TraderLogic
    {
        private class WarehousedGood
        {
            public WarehousedGood(int transactionID, int portID, int commodityID, int quantity, DateTime buyTime)
            {
                this.transactionID = transactionID;
                this.portID = portID;
                this.commodityID = commodityID;
                this.quantity = quantity;
                this.buyTime = buyTime;
            }

            public int transactionID;
            public int portID;
            public int commodityID;
            public int quantity;
            public DateTime buyTime;
        }

        private DbConn dbConn;
        private List<WarehousedGood> warehousedGoods = new List<WarehousedGood>();

        private decimal SHIPPING_COMPANY_RATE;

        public TraderLogic(decimal SHIPPING_COMPANY_RATE)
        {
            dbConn = new DbConn();
            this.SHIPPING_COMPANY_RATE = SHIPPING_COMPANY_RATE;
        }

        public void DecideSales(List<MoveContractMsg> moveContracts, List<LocalSaleMsg> localSales)
        {
            foreach (var good in warehousedGoods)
            {
                var salePorts = new List<Tuple<int, decimal, decimal>>();

                SqlCommand cmd = new SqlCommand(
@"select pcp.PortId, pcp.LocalPrice, Currency.USDValue from PortCommodityPrice pcp
join Port p on pcp.PortID = p.ID
join Country on p.CountryID = Country.ID
join Currency on Country.CurrencyID = Currency.ID
where CommodityId = @CID");
                cmd.Parameters.AddWithValue("@CID", good.commodityID);
                SqlDataReader reader = dbConn.ExecuteQuery(cmd);

                while (reader.Read())
                {
                    int portID = reader.GetInt32(0);
                    decimal localPrice = reader.GetDecimal(1);
                    decimal USDValue = reader.GetDecimal(2);
                    salePorts.Add(new Tuple<int, decimal, decimal>(portID, localPrice, USDValue));
                }
                reader.Close();

                int bestPort = 0;
                decimal bestProfit = 0;

                SqlCommand shippingRateCmd = new SqlCommand("select dbo.funcShippingCost(@Port1, @Port2, @ShippingRate, @Quantity)");

                foreach (var port in salePorts)
                {
                    decimal shippingCost;

                    if (good.portID == port.Item1)
                        shippingCost = 0;
                    else
                    {
                        shippingRateCmd.Parameters.Clear();
                        shippingRateCmd.Parameters.AddWithValue("@Port1", good.portID);
                        shippingRateCmd.Parameters.AddWithValue("@Port2", port.Item1);
                        shippingRateCmd.Parameters.AddWithValue("@ShippingRate", SHIPPING_COMPANY_RATE);
                        shippingRateCmd.Parameters.AddWithValue("@Quantity", good.quantity);
                        shippingCost = (decimal)dbConn.ExecuteScalar(shippingRateCmd);
                    }

                    decimal profit = (good.quantity * port.Item2 * port.Item3) - shippingCost;

                    if (profit > bestProfit)
                    {
                        bestPort = port.Item1;
                        bestProfit = profit;
                    }
                }

                if (good.portID == bestPort)
                    localSales.Add(new LocalSaleMsg(good.transactionID));
                else
                    moveContracts.Add(new MoveContractMsg(good.portID, bestPort, good.transactionID));
            }

            warehousedGoods.Clear();
        }

        public void AddGood(int transactionID, int portID, int commodID, int quantity)
        {
            warehousedGoods.Add(new WarehousedGood(transactionID, portID, commodID, quantity, DateTime.Now));
        }
    }
}
