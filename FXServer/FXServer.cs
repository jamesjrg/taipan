using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using TaiPan.Common;

namespace TaiPan.FXServer
{
    internal class Currency
    {
        public Currency(string shortName, decimal USDValue)
        {
            this.shortName = shortName;
            this.USDValue = USDValue;
        }

        public string shortName;
        public decimal USDValue;
    }

    class FXServer : TaiPan.Common.EconomicPlayer
    {
        private TaiPan.Common.Server server;
        private SqlConnection dbConn;
        private List<Currency> currencies = new List<Currency>();
        private Random random = new Random();

        protected override void Init(string[] args)
        {
            Console.Title = "FXServer";

            dbConn = Db.GetDbConnectionRW();
            dbConn.Open();

            Console.WriteLine("Reading currencies from db");
            string select = "SELECT ShortName, USDValue FROM Currency";
            SqlCommand cmd = new SqlCommand(select, dbConn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                string shortName = reader.GetString(0);
                decimal USDValue = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", shortName, USDValue);
                currencies.Add(new Currency(shortName, USDValue));
            }
            reader.Close();

            server = new TaiPan.Common.Server(serverConfigs["FXServer-BankBroadcast"]);
        }

        protected override bool Run()
        {
            DecidePrices();

            foreach (Currency currency in currencies)
                server.Send(currency.shortName + currency.USDValue);
            return true;
        }

        private void DecidePrices()
        {
            for (int i = 0; i != currencies.Count; ++i)
            {
                //nextdouble between 0 and 1.0
                currencies[i].USDValue += (decimal)random.NextDouble() - 0.5m;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Db.CloseConn(dbConn);
            server.Dispose();            
        }
    }
}
