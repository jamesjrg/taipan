using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using TaiPan.Common;
using TaiPan.Common.NetContract;
using TaiPan.StatsLib;

namespace TaiPan.FXServer
{
    /// <summary>
    /// Singleton class for FXServer process
    /// </summary>
    class FXServer : EconomicPlayer
    {
        private Server bankServer;
        private CurrencyMsg currencies = new CurrencyMsg();
        private int USDID;

        private StatsLib.StatsLib stats = new StatsLib.StatsLib();
        private Random random = new Random();

        public FXServer(string[] args)
        {
            Console.Title = "FXServer";

            DbConn dbConn = new DbConn();

            Console.WriteLine("Reading currencies from db");
            List<CurrencyMsgItem> tmpList = new List<CurrencyMsgItem>();
            SqlDataReader reader = dbConn.ExecuteQuery(new SqlCommand("SELECT ID, ShortName, USDValue FROM Currency ORDER BY ID ASC"));
            while (reader.Read()) {
                int id = reader.GetInt32(0);
                string shortName = reader.GetString(1);
                decimal USDValue = reader.GetDecimal(2);
                Console.WriteLine("{0}: {1}", id, USDValue);
                tmpList.Add(new CurrencyMsgItem(id, USDValue));

                //we don't make the price of USD fluctuate relative to USD...
                if (shortName == "USD")
                    USDID = id;
            }
            reader.Close();
            currencies.items = tmpList.ToArray();

            dbConn.Dispose();

            bankServer = new Server(ServerConfigs["FXServer-Bank"], AppSettings, true);
        }

        protected override bool Run()
        {
            DecidePrices();

            bankServer.Send(NetContract.Serialize(NetMsgType.Currency, currencies));

            return true;
        }

        private void DecidePrices()
        {
            currencies.time = DateTime.Now;
            for (int i = 0; i != currencies.items.Length; ++i)
            {
                if (currencies.items[i].id == USDID)
                    continue;

                decimal nextVal =stats.GBMSequence(currencies.items[i].USDValue, TickVolatility, 1)[0];
                currencies.items[i].USDValue = nextVal;
            }
        }
    }
}
