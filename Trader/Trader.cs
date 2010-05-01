using System;
using System.Collections.Generic;
using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.Trader
{
    /// <summary>
    /// Singleton class for Trader process
    /// </summary>
    class Trader : EconomicPlayer
    {
        private int myID;

        private Server shippingServer;

        private Client bankClient;
        private Client fateClient;

        private List<FutureMsg> futureRequests = new List<FutureMsg>();
        private List<BuyMsg> buyRequests = new List<BuyMsg>();

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
            shippingServer = new Server(conf, AppSettings, false);

            bankClient = new Client(ServerConfigs["Bank-Trader"], AppSettings, myID, false);

            fateClient = new Client(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, myID, true);
        }

        protected override bool Run()
        {
            while (fateClient.incoming.Count != 0)
                Console.WriteLine(fateClient.incoming.Dequeue());

            foreach (var msg in futureRequests)
                bankClient.Send(NetContract.Serialize(NetMsgType.Future, msg));
            futureRequests.Clear();
            return true;

            foreach (var msg in buyRequests)
                bankClient.Send(NetContract.Serialize(NetMsgType.Buy, msg));
            buyRequests.Clear();
            return true; 
        }
    }
}
