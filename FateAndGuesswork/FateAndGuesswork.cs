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
        private List<ForecastInfo> shortages = new List<ForecastInfo>();
        private List<ForecastInfo> surpluses = new List<ForecastInfo>();
        
        private readonly int nPorts;
        private readonly int nCommodities;

        private Random random = new Random();
        private StatsLib.StatsLib stats = new StatsLib.StatsLib();

        private DateTime lastTick = new DateTime(0);

        //for forecasted price jumps
        const int MIN_QUANTITY = 100;
        const int MAX_QUANTITY = 500;

        //in ticks
        const int MIN_FORECAST_OFFSET_IN_TICKS = 5;
        const int MAX_FORECAST_OFFSET_IN_TICKS = 10;

        //in seconds
        readonly int MIN_FORECAST_OFFSET;
        readonly int MAX_FORECAST_OFFSET;

        private class ForecastInfo
        {
            public ForecastInfo(int traderID, int portID, int commodID, int quantity, DateTime time)
            {
                this.traderID = traderID;
                this.msg = new ForecastMsg(portID, commodID, quantity, time);
            }

            public ForecastMsg msg;
            public int traderID;
        }

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

            nCommodities = (int)dbConn.ExecuteScalar("select count (*) from Commodity");

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

            nPorts = (int)dbConn.ExecuteScalar("select count (*) from Port");
            
            //close db conn
            dbConn.Dispose();

            MIN_FORECAST_OFFSET = MIN_FORECAST_OFFSET_IN_TICKS * MAIN_LOOP_TICK;
            MAX_FORECAST_OFFSET = MAX_FORECAST_OFFSET_IN_TICKS * MAIN_LOOP_TICK;

            bankServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, true);
            traderServer = new TaiPan.Common.Server(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, true);
        }

        protected override bool Run()
        {
            //don't want multiple ticks with the exact same timestamp on the db entries, though assuming tick is at least one second this shouldn't happen anyway
            if (lastTick == DateTime.Now)
                return true;
            else
                lastTick = DateTime.Now;

            DecideCommodPrices();
            DecideStockPrices();
            DecideCommodPriceJumps();

            bankServer.Send(NetContract.Serialize(NetMsgType.Commodity, commodityMsg));
            bankServer.Send(NetContract.Serialize(NetMsgType.Stock, stocks));

            foreach (var info in shortages)
                traderServer.Send(NetContract.Serialize(NetMsgType.Shortage, info.msg), info.traderID);
            shortages.Clear();

            foreach (var info in surpluses)
                traderServer.Send(NetContract.Serialize(NetMsgType.Surplus, info.msg), info.traderID);
            surpluses.Clear();

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
            var clientIDs = traderServer.GetClientIDs();

            if (clientIDs.Count == 0)
                return;

            int rand = random.Next(0, clientIDs.Count);
            int targetTrader = clientIDs[rand];

            int portID = random.Next(1, nPorts + 1);
            int commodID = random.Next(1, nCommodities + 1);

            int quantity = random.Next(MIN_QUANTITY, MAX_QUANTITY);
            
            int offset = random.Next(MIN_FORECAST_OFFSET, MAX_FORECAST_OFFSET + 1);
            DateTime time = DateTime.Now.AddMilliseconds(offset);

            if (random.Next(0, 2) == 0)
                shortages.Add(new ForecastInfo(targetTrader, portID, commodID, quantity, time));
            else
                surpluses.Add(new ForecastInfo(targetTrader, portID, commodID, quantity, time));
        }
    }
}
