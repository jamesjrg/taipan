using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaiPan.Common;

namespace TaiPan.ShippingCompany
{
    /// <summary>
    /// Singleton class for ShippingCompany process
    /// </summary>
    class ShippingCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        private Client bankPoller;
        private List<Client> traderPollers = new List<Client>();

        public ShippingCompany(string[] args)
        {
            myID = SetID("ShippingCompany", args);

            int nTraders;
            try
            {
                nTraders = Int32.Parse(args[1]);
            }
            catch (Exception)
            {
                throw new ApplicationException("Requires 2 command line arguments: first is id, second is number of traders");
            }

            bankPoller = new Client(ServerConfigs["Bank-Shipping"], AppSettings);

            var conf = ServerConfigs["Trader-Shipping"];
            for (int i = 0; i != nTraders; ++i)
            {
                traderPollers.Add(new Client(conf, AppSettings));
                conf.port += 1;
            }
        }        

        protected override bool Run()
        {
            return true;
        }
    }
}
