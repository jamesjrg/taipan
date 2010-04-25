using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.FateAndGuesswork
{
    /// <summary>
    /// Singleton class for FateAndGuesswork process
    /// </summary>
    class FateAndGuesswork : TaiPan.Common.EconomicPlayer
    {
        private Server bankServer;
        private Server traderServer;

        private List<CommodityPrice> commodityPrices = new List<CommodityPrice>();
        private List<CommodityMsg> commodityMsgs = new List<CommodityMsg>();
        private List<StockMsg> stocks = new List<StockMsg>();

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
            reader.Close();

            Console.WriteLine("Reading stocks from db");
            reader = dbConn.ExecuteQuery("SELECT CompanyID, USDStockPrice from ShippingCompany ORDER BY CompanyID ASC");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                decimal price = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", id, price);
                stocks.Add(new StockMsg(id, price));
            }

            //close db conn
            reader.Close();
            dbConn.Dispose();

            bankServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, true);
            traderServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, true);
        }

        protected override bool Run()
        {
            DecideCommodPrices();
            DecideStockPrices();
            DecideCommodPriceJumps();

            foreach (CommodityMsg commod in commodityMsgs)
                bankServer.Send(NetContract.Serialize(NetMsgType.Commodity, commod));
            foreach (StockMsg stock in stocks)
                bankServer.Send(NetContract.Serialize(NetMsgType.Stock, stock));

            return true;
        }

        private void DecideCommodPrices()
        {
            commodityMsgs.Clear();

            for (int i = 0; i != commodityPrices.Count; ++i)
            {
                decimal nextVal = StatsLib.StatsLib.GBMSequence(commodityPrices[i].localPrice, TickVolatility, 1)[0];
                commodityPrices[i].localPrice = nextVal;
                commodityMsgs.Add(new CommodityMsg(commodityPrices[i].portId, commodityPrices[i].commodId, commodityPrices[i].localPrice));
            }
        }

        private void DecideStockPrices()
        {
            for (int i = 0; i != stocks.Count; ++i)
            {
                decimal nextVal = StatsLib.StatsLib.GBMSequence(stocks[i].price, TickVolatility, 1)[0];
                stocks[i].price = nextVal;
            }
        }

        private void DecideCommodPriceJumps()
        {
            
        }
    }
}
