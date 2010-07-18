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

        private Dictionary<string, int> portDistances;

        private class ShipInProgress
        {
            public ShipInProgress(int departureID, int destID, int transactionID, DateTime plannedArrivalTime)
            {
                this.departureID = departureID;
                this.destID = destID;
                this.transactionID = transactionID;
                this.plannedArrivalTime = plannedArrivalTime;
            }

            public int departureID;
            public int destID;
            public int transactionID;
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

            DbConn dbConn = new DbConn();
            portDistances = GetPortDistancesLookup(dbConn);
            dbConn.Dispose();

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
                foreach (var msg in keyValPair.Value)
                    traderClients[keyValPair.Key].Send(NetContract.Serialize(NetMsgType.AcceptMove, msg));
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
                if (ship.plannedArrivalTime <= DateTime.Now)
                {
                    //sending departure portID, though never actually needed
                    arrivals.Add(new MovingMsg(ship.departureID, ship.destID, ship.transactionID, DateTime.Now));
                    shipsInProgress.Remove(ship);
                }
            }
        }

        private void MoveAdvertised(int traderPort, MoveContractMsg msg)
        {
            moveWishes[traderPort].Add(new MoveContractMsg(msg.departureID, msg.destID, msg.transactionID));
        }

        private void MoveConfirmed(int traderPort, MoveContractMsg msg)
        {
            int distance = portDistances[msg.departureID + "," + msg.destID];
            int time = distance / FREIGHTER_SPEED;
            DateTime plannedArrivalTime = DateTime.Now.AddSeconds(time);

            departures.Add(new MovingMsg(msg.departureID, msg.destID, msg.transactionID, DateTime.Now));
            shipsInProgress.Add(new ShipInProgress(msg.departureID, msg.destID, msg.transactionID, plannedArrivalTime));
        }
    }
}
