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

        private TaiPan.Common.Server bankBroadcast;
        private TaiPan.Common.Server traderBroadcast;

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

            var conf = ServerConfigs["Shipping-BankBroadcast"];
            conf.port = conf.port + (myID - 1);
            bankBroadcast = new Server(conf, AppSettings);
            conf = ServerConfigs["Shipping-TraderBroadcast"];
            conf.port = conf.port + (myID - 1);
            traderBroadcast = new Server(conf, AppSettings);

            traderPollers.Add(new Client(ServerConfigs["Trader-ShippingBroadcast"], AppSettings));
        }        

        protected override bool Run()
        {
            return true;
        }
    }
}
