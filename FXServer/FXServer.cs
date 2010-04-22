using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.FXServer
{
    /// <summary>
    /// Singleton class for FXServer process
    /// </summary>
    class FXServer : EconomicPlayer
    {
        private Server bankListener;
        private List<CurrencyMsg> currencies = new List<CurrencyMsg>();
        private Random random = new Random();

        public FXServer(string[] args)
        {
            Console.Title = "FXServer";

            DbConn dbConn = new DbConn();
            Console.WriteLine("Reading currencies from db");
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT ShortName, USDValue FROM Currency ORDER BY ID ASC");
            while (reader.Read()) {
                string shortName = reader.GetString(0);
                decimal USDValue = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", shortName, USDValue);
                currencies.Add(new CurrencyMsg(shortName, USDValue));
            }
            reader.Close();
            dbConn.Dispose();

            bankListener = new Server(ServerConfigs["FXServer-Bank"], AppSettings);
        }

        protected override bool Run()
        {
            DecidePrices();

            foreach (CurrencyMsg currency in currencies)
                bankListener.Send(NetContract.Serialize(NetMsgType.Currency, currency));
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
