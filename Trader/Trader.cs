using System;
using System.Collections.Generic;
using System.Threading;

using TaiPan.Common;

namespace TaiPan.Trader
{
    /// <summary>
    /// Singleton class for Trader process
    /// </summary>
    class Trader : EconomicPlayer
    {
        private int myID;

        private Server shippingListener;

        private Client bankPoller;
        private Client fatePoller;

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

            var conf = ServerConfigs["Trader-Shipping"];
            conf.port = conf.port + (myID - 1);
            shippingListener = new Server(conf, AppSettings);

            bankPoller = new Client(ServerConfigs["Bank-Trader"], AppSettings);

            fatePoller = new Client(ServerConfigs["FateAndGuesswork-Trader"], AppSettings);
        }

        protected override bool Run()
        {
            while (fatePoller.incoming.Count != 0)
                Console.WriteLine(fatePoller.incoming.Dequeue());
            return true; 
        }
    }
}
