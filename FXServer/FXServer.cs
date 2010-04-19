using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using TaiPan.Common;

namespace TaiPan.FXServer
{
    /// <summary>
    /// Singleton class for FXServer process
    /// </summary>
    class FXServer : TaiPan.Common.EconomicPlayer
    {
        private TaiPan.Common.Server server;
        private List<Currency> currencies = new List<Currency>();
        private Random random = new Random();

        private class Currency
        {
            public Currency(string shortName, decimal USDValue)
            {
                this.shortName = shortName;
                this.USDValue = USDValue;
            }

            public string shortName;
            public decimal USDValue;
        }

        public FXServer(string[] args)
        {
            Console.Title = "FXServer";

            DbConn dbConn = new DbConn();
            Console.WriteLine("Reading currencies from db");
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT ShortName, USDValue FROM Currency ORDER BY ID DESC");
            while (reader.Read()) {
                string shortName = reader.GetString(0);
                decimal USDValue = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", shortName, USDValue);
                currencies.Add(new Currency(shortName, USDValue));
            }
            reader.Close();
            dbConn.Dispose();

            server = new TaiPan.Common.Server(ServerConfigs["FXServer-BankBroadcast"], AppSettings);
        }

        protected override bool Run()
        {
            DecidePrices();

            foreach (Currency currency in currencies)
                server.Send(currency.shortName + ',' + currency.USDValue);
            return true;
        }

        private void DecidePrices()
        {
            for (int i = 0; i != currencies.Count; ++i)
            {
                //nextdouble between 0 and 1.0
                decimal newPrice = currencies[i].USDValue + (decimal)random.NextDouble() - 0.5m;
                if (newPrice > 0)
                    currencies[i].USDValue = newPrice;
            }
        }
    }
}
