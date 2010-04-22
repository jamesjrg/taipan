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

        private Client bankClient;
        private List<Client> traderClients = new List<Client>();

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

            bankClient = new Client(ServerConfigs["Bank-Shipping"], AppSettings, myID, false);

            var conf = ServerConfigs["Trader-Shipping"];
            for (int i = 0; i != nTraders; ++i)
            {
                traderClients.Add(new Client(conf, AppSettings, myID, false));
                conf.port += 1;
            }
        }        

        protected override bool Run()
        {
            return true;
        }
    }
}
