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
        private Server bankServer;
        private List<CurrencyMsg> currencies = new List<CurrencyMsg>();
        private Random random = new Random();

        public FXServer(string[] args)
        {
            Console.Title = "FXServer";

            DbConn dbConn = new DbConn();
            Console.WriteLine("Reading currencies from db");
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT ID, USDValue FROM Currency ORDER BY ID ASC");
            while (reader.Read()) {
                int id = reader.GetInt32(0);
                decimal USDValue = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", id, USDValue);
                currencies.Add(new CurrencyMsg(id, USDValue));
            }
            reader.Close();
            dbConn.Dispose();

            bankServer = new Server(ServerConfigs["FXServer-Bank"], AppSettings, true);
        }

        protected override bool Run()
        {
            DecidePrices();

            foreach (CurrencyMsg currency in currencies)
                bankServer.Send(NetContract.Serialize(NetMsgType.Currency, currency));
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
