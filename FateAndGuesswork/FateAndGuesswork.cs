using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaiPan.Common;

namespace TaiPan.FateAndGuesswork
{
    /// <summary>
    /// Singleton class for FateAndGuesswork process
    /// </summary>
    class FateAndGuesswork : TaiPan.Common.EconomicPlayer
    {
        private Server bankBroadcast;
        private Server traderBroadcast;

        private List<Port> ports = new List<Port>();
        private List<Stock> stocks = new List<Stock>();
        private Random random = new Random();

        private class Commodity
        {
            public Commodity(string name, decimal localPrice, int importProb, int exportProb)
            {
                this.name = name;
                this.localPrice = localPrice;
                this.importProb = importProb;
                this.exportProb = exportProb;
            }

            public string name;
            public decimal localPrice;
            public int importProb;
            public int exportProb;
        }

        private class Port
        {
            public Port(string name)
            {
                this.name = name;
                this.commodityPrices = new List<Commodity>();
            }

            public string name;
            public List<Commodity> commodityPrices;
        }

        private class Stock
        {
            public Stock(string name, decimal price)
            {
                this.name = name;
                this.price = price;
            }

            public string name;
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
