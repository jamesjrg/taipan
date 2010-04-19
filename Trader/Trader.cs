using System;
using System.Collections.Generic;
using System.Threading;

using TaiPan.Common;

namespace TaiPan.Trader
{
    /// <summary>
    /// Singleton class for Trader process
    /// </summary>
    class Trader : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        private TaiPan.Common.Server bankBroadcast;
        private TaiPan.Common.Server shippingBroadcast;

        private Client fatePoller;
        private List<Client> shippingPollers = new List<Client>();

        public Trader(string[] args)
        {
            myID = SetID("Trader", args);

            int nShipping;
            try
            {
                nShipping = Int32.Parse(args[1]);
            }
            catch (Exception)
            {
                throw new ApplicationException("Requires 2 command line arguments: first is id, second is number of shipping companies");
            }

            var conf = ServerConfigs["Trader-BankBroadcast"];
            conf.port = conf.port + (myID - 1);
            bankBroadcast = new Server(conf, AppSettings);
            conf = ServerConfigs["Trader-ShippingBroadcast"];
            conf.port = conf.port + (myID - 1);
            shippingBroadcast = new Server(conf, AppSettings);

            fatePoller = new Client(ServerConfigs["FateAndGuessWork-TraderBroadcast"], AppSettings);
            shippingPollers.Add(new Client(ServerConfigs["Shipping-TraderBroadcast"], AppSettings));
        }

        protected override bool Run()
        {
            if (fatePoller.messages.Count != 0)
                Console.WriteLine(fatePoller.messages.Dequeue());
            return true; 
        }
    }
}
