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

        private List<CommodityInfo> commodityInfos = new List<CommodityInfo>();
        private CommodityMsg commodityMsg = new CommodityMsg();
        private StockMsg stocks = new StockMsg();

        private Random random = new Random();
        private StatsLib.StatsLib stats = new StatsLib.StatsLib();

        private class CommodityInfo
        {
            public CommodityInfo(int portID, int commodID, decimal localPrice, int surplusProb, int shortageProb)
            {
                this.portID = portID;
                this.commodID = commodID;                
                this.localPrice = localPrice;
                this.surplusProb = surplusProb;
                this.shortageProb = shortageProb;
            }

            public int portID;
            public int commodID;
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
                int portID = reader.GetInt32(0);
                int commodID = reader.GetInt32(1);
                decimal localPrice = reader.GetDecimal(2);
                int shortageProb = reader.GetInt32(3);
                int surplusProb = reader.GetInt32(4);
                commodityInfos.Add(new CommodityInfo(portID, commodID, localPrice, shortageProb, surplusProb));
            }
            reader.Close();

            commodityMsg.items = new CommodityMsgItem[commodityInfos.Count];
            for (int i = 0; i != commodityInfos.Count; ++i )
                commodityMsg.items[i] = new CommodityMsgItem(commodityInfos[i].portID, commodityInfos[i].commodID, commodityInfos[i].localPrice);

            Console.WriteLine("Reading stocks from db");
            List<StockMsgItem> tmpList = new List<StockMsgItem>();
            reader = dbConn.ExecuteQuery("SELECT CompanyID, USDStockPrice from ShippingCompany ORDER BY CompanyID ASC");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                decimal price = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", id, price);
                tmpList.Add(new StockMsgItem(id, price));
            }
            stocks.items = tmpList.ToArray();
            reader.Close();

            //close db conn
            dbConn.Dispose();

            bankServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, true);
            traderServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, true);
        }

        protected override bool Run()
        {
            DecideCommodPrices();
            DecideStockPrices();
            DecideCommodPriceJumps();

            bankServer.Send(NetContract.Serialize(NetMsgType.Commodity, commodityMsg));
            bankServer.Send(NetContract.Serialize(NetMsgType.Stock, stocks));

            return true;
        }

        private void DecideCommodPrices()
        {
            commodityMsg.time = DateTime.Now;
            for (int i = 0; i != commodityInfos.Count; ++i)
            {
                decimal nextVal = stats.GBMSequence(commodityInfos[i].localPrice, TickVolatility, 1)[0];
                commodityInfos[i].localPrice = nextVal;
                commodityMsg.items[i].localPrice = nextVal;
            }
        }

        private void DecideStockPrices()
        {
            stocks.time = DateTime.Now;
            for (int i = 0; i != stocks.items.Length; ++i)
            {
                decimal nextVal = stats.GBMSequence(stocks.items[i].price, TickVolatility, 1)[0];
                stocks.items[i].price = nextVal;
            }
        }

        private void DecideCommodPriceJumps()
        {
            
        }
    }
}
