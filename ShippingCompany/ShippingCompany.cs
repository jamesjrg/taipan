using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.ShippingCompany
{
    /// <summary>
    /// Singleton class for ShippingCompany process
    /// </summary>
    class ShippingCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        private Client bankClient;
        private Dictionary<int, Client> traderClients = new Dictionary<int, Client>();

        private List<MovingMsg> departures = new List<MovingMsg>();
        private List<MovingMsg> arrivals = new List<MovingMsg>();

        private List<ShipInProgress> shipsInProgress = new List<ShipInProgress>();
        private Dictionary<int, List<MoveContractMsg>> moveWishes = new Dictionary<int, List<MoveContractMsg>>();

        private class ShipInProgress
        {
            public ShipInProgress(int warehouseID, DateTime plannedArrivalTime)
            {
                this.warehouseID = warehouseID;
                this.plannedArrivalTime = plannedArrivalTime;
            }

            public int warehouseID;
            public DateTime plannedArrivalTime;
        }


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
                traderClients[conf.port] = new Client(conf, AppSettings, myID, false);
                moveWishes[conf.port] = new List<MoveContractMsg>();
                conf.port += 1;
            }
        }        

        protected override bool Run()
        {
            DecideArrivals();

            foreach (var trader in traderClients)
            {
                List<DeserializedMsg> traderIncoming = trader.Value.IncomingDeserializeAll();
                foreach (var msg in traderIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.AdvertiseMove:
                            MoveAdvertised(trader.Key, (MoveContractMsg)msg.data);
                            break;
                        case NetMsgType.ConfirmMove:
                            MoveConfirmed(trader.Key, (MoveContractMsg)msg.data);
                            break;
                        default:
                            throw new ApplicationException("fxClient received wrong type of net message");
                    }
                }
            }

            foreach (var keyValPair in moveWishes)
            {
                traderClients[keyValPair.Key].Send(NetContract.Serialize(NetMsgType.AcceptMove, keyValPair.Value));
                moveWishes[keyValPair.Key].Clear();
            }

            foreach (var departure in departures)
                bankClient.Send(NetContract.Serialize(NetMsgType.Departure, departure));
            
            foreach (var arrival in arrivals)
                bankClient.Send(NetContract.Serialize(NetMsgType.Arrival, arrival));
            
            return true;
        }

        private void DecideArrivals()
        {
            var shipsInProgressCopy = new List<ShipInProgress>(shipsInProgress);
            foreach (var ship in shipsInProgressCopy)
            {
                arrivals.Add(new MovingMsg(1, DateTime.Now));
                shipsInProgress.Remove(ship);
            }
        }

        private void MoveAdvertised(int traderPort, MoveContractMsg msg)
        {
            moveWishes[traderPort].Add(new MoveContractMsg(msg.warehouseID));
        }

        private void MoveConfirmed(int traderPort, MoveContractMsg msg)
        {
            departures.Add(new MovingMsg(1, DateTime.Now));
            shipsInProgress.Add(new ShipInProgress(1, DateTime.Now.AddSeconds(5)));
        }
    }
}
