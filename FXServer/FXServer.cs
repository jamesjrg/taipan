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
        private Random random = new Random();

        public FXServer(string[] args)
        {
            Console.Title = "FXServer";

            DbConn dbConn = new DbConn();

            Console.WriteLine("Reading currencies from db");
            List<CurrencyMsgItem> tmpList = new List<CurrencyMsgItem>();
            SqlDataReader reader = dbConn.ExecuteQuery("SELECT ID, USDValue FROM Currency ORDER BY ID ASC");
            while (reader.Read()) {
                int id = reader.GetInt32(0);
                decimal USDValue = reader.GetDecimal(1);
                Console.WriteLine("{0}: {1}", id, USDValue);
                tmpList.Add(new CurrencyMsgItem(id, USDValue));
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
                decimal nextVal = StatsLib.StatsLib.GBMSequence(currencies.items[i].USDValue, TickVolatility, 1)[0];
                currencies.items[i].USDValue = nextVal;
            }
        }
    }
}
