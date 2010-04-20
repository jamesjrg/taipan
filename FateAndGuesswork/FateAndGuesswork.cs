using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaiPan.Common;
using System.Data.SqlClient;

namespace TaiPan.FateAndGuesswork
{
    /// <summary>
    /// Singleton class for FateAndGuesswork process
    /// </summary>
    class FateAndGuesswork : TaiPan.Common.EconomicPlayer
    {
        private Server bankBroadcast;
        private Server traderBroadcast;

        private List<CommodityPrice> commodityPrices = new List<CommodityPrice>();
        private List<Stock> stocks = new List<Stock>();
        private Random random = new Random();

        private class CommodityPrice
        {
            public CommodityPrice(int portId, int commodId, decimal localPrice, int surplusProb, int shortageProb)
            {
                this.portId = portId;
                this.commodId = commodId;                
                this.localPrice = localPrice;
                this.surplusProb = surplusProb;
                this.shortageProb = shortageProb;
            }

            public int portId;
            public int commodId;
            public decimal localPrice;
            public int surplusProb;
            public int shortageProb;
        }

        private class Stock
        {
            public Stock(int companyId, decimal price)
            {
                this.companyId = companyId;
                this.price = price;
            }

            public int companyId;
            public decimal price;
        }

        private class PriceJump
        {
            public PriceJump(string traderId, PriceJumpType type, string port, string commod, int quantity, DateTime when)
            {
                this.traderId = traderId;
                this.type = type;
                this.port = port;
                this.commod = commod;
                this.when = when;
                this.quantity = quantity;
            }

            public string traderId;
            public PriceJumpType type;
            string port;
            string commod;
            int quantity;
            DateTime when;
        }

        public FateAndGuesswork(string[] args)
        {
            Console.Title = "FateAndGuesswork";

            DbConn dbConn = new DbConn();

            Console.WriteLine("Reading commodity prices from db");           
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT PortID, CommodityID, LocalPrice, ShortageProb, SurplusProb FROM PortCommodityPrice ORDER BY PortID ASC");
            while (reader.Read())
            {
                int portId = reader.GetInt32(0);
                int commodId = reader.GetInt32(1);
                decimal localPrice = reader.GetDecimal(2);
                int shortageProb = reader.GetInt32(3);
                int surplusProb = reader.GetInt32(4);
                commodityPrices.Add(new CommodityPrice(portId, commodId, localPrice, shortageProb, surplusProb));
            }

            Console.WriteLine("Reading stocks from db");
            reader = dbConn.ExecuteQuery("SELECT CompanyID, USDStockPrice from ShippingCompany ORDER BY CompanyID ASC");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                decimal price = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", id, price);
                stocks.Add(new Stock(id, price));
            }

            //close db conn
            reader.Close();
            dbConn.Dispose();

            bankBroadcast = new TaiPan.Common.Server(ServerConfigs["FateAndGuessWork-BankBroadcast"], AppSettings);
            traderBroadcast = new TaiPan.Common.Server(ServerConfigs["FateAndGuessWork-TraderBroadcast"], AppSettings);
        }

        protected override bool Run()
        {
            DecideCommodPrices();
            //DecideCommodPriceJumps();
            //DecideStockPrices();

            foreach (Port port in ports)
            {
                foreach (Commodity commod in port.commodityPrices)
                    bankBroadcast.Send("commodity," + port.name + ',' + commod.name + ',' + commod.localPrice);
                foreach (Stock stock in stocks)
                    bankBroadcast.Send("stock," + stock.name + ',' + stock.price);
            }
            return true;
        }

        private void DecideCommodPrices()
        {
            for (int i = 0; i != ports.Count; ++i)
            {
                for (int j = 0; j != ports[i].commodityPrices.Count; ++j)
                {
                    decimal newPrice = ports[i].commodityPrices[j].localPrice + (decimal)random.NextDouble() - 0.5m;
                    if (newPrice > 0)
                        ports[i].commodityPrices[j].localPrice = newPrice;
                }
            }
        }
    }
}
