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
        private List<MoveContractMsg> movesWishes = new List<MoveContractMsg>();

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
                            MoveAdvertised((MoveContractMsg)(msg.data));
                            break;
                        case NetMsgType.ConfirmMove:
                            MoveConfirmed((MoveContractMsg)(msg.data));
                            break;
                        default:
                            throw new ApplicationException("fxClient received wrong type of net message");
                    }
                }
            }

            //foreach (var moveWish in moveWishes)
            //    traderClient.Send(NetContract.Serialize(NetMsgType, moveWish));

            foreach (var departure in departures)
                bankClient.Send(NetContract.Serialize(NetMsgType.Departure, departure));
            
            foreach (var arrival in arrivals)
                bankClient.Send(NetContract.Serialize(NetMsgType.Arrival, arrival));
            
            return true;
        }

        private void DecideArrivals()
        {
            arrivals.Add(new MovingMsg(1, DateTime.Now));
        }

        private void MoveAdvertised(MoveContractMsg msg)
        {
            movesWishes.Add(new MoveContractMsg(msg.warehouseID));
        }

        private void MoveConfirmed(MoveContractMsg msg)
        {
            departures.Add(new MovingMsg(1, DateTime.Now));
            shipsInProgress.Add(new ShipInProgress(1, DateTime.Now.AddSeconds(5)));
        }
    }
}
