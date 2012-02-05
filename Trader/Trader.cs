using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

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

        private TraderLogic logic;

        private List<FutureMsg> futureRequests = new List<FutureMsg>();
        private List<BuyMsg> buyRequests = new List<BuyMsg>();
        private List<MoveContractMsg> moveContracts = new List<MoveContractMsg>();
        private List<MoveContractMsg> unconfirmedContracts = new List<MoveContractMsg>();
        private List<MoveConfirmInfo> moveConfirms = new List<MoveConfirmInfo>();

        private List<LocalSaleMsg> localSales = new List<LocalSaleMsg>();

        private const int SURPLUS_LEEWAY = 2;

        private class MoveConfirmInfo
        {
            public MoveConfirmInfo(MoveContractMsg msg, int targetCompany)
            {
                this.msg = msg;
                this.targetCompany = targetCompany;
            }

            public MoveContractMsg msg;
            public int targetCompany;
        }

        public Trader(string[] args)
        {
            myID = SetID("Trader", args);

            logic = new TraderLogic();

            var conf = ServerConfigs["Trader-Shipping"];
            conf.port = conf.port + (myID - 1);

            shippingServer = new Server(conf, AppSettings, false);
            bankClient = new Client(ServerConfigs["Bank-Trader"], AppSettings, myID, false);
            fateClient = new Client(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, myID, true);
        }

        protected override bool Run()
        {
            List<DeserializedMsg> bankIncoming = bankClient.IncomingDeserializeAll();
            foreach (var msg in bankIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.BuyConfirm:
                        BuyConfirmed((BankConfirmMsg)(msg.data));
                        break;
                    case NetMsgType.FutureSettle:
                        FutureSettled((BankConfirmMsg)(msg.data));
                        break;
                    default:
                        throw new ApplicationException("bankClient received wrong type of net message");
                }
            }

            List<DeserializedMsg> fateIncoming = fateClient.IncomingDeserializeAll();
            foreach (var msg in fateIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Surplus:
                        SurplusForecast((ForecastMsg)(msg.data));
                        break;
                    case NetMsgType.Shortage:
                        ShortageForecast((ForecastMsg)(msg.data));
                        break;
#if true
                    case NetMsgType.DebugTimer:
                        DebugTimerMsg debugMsg = (DebugTimerMsg)msg.data;
                        debugMsg.times = debugMsg.times.Concat(new int[] { Environment.TickCount }).ToArray();
                        bankClient.Send(NetContract.Serialize(NetMsgType.DebugTimer, debugMsg));
                        break;
#endif

                    default:
                        throw new ApplicationException("fateClient received wrong type of net message: " + msg.type);
                }
            }

            foreach (var client in shippingServer.clients)
            {
                List<DeserializedMsg> shippingIncoming = shippingServer.IncomingDeserializeAll(client.id);
                foreach (var msg in shippingIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.AcceptMove:
                            MoveAccepted(client.id, (MoveContractMsg)(msg.data));
                            break;                    
                        default:
                            throw new ApplicationException("shippingServer received wrong type of net message");
                    }
                }
            }

            logic.DecideSales(moveContracts, localSales);

            foreach (var msg in futureRequests)
                bankClient.Send(NetContract.Serialize(NetMsgType.Future, msg));
            futureRequests.Clear();
            
            foreach (var msg in buyRequests)
                bankClient.Send(NetContract.Serialize(NetMsgType.Buy, msg));
            buyRequests.Clear();

            foreach (var msg in moveContracts)
            {
                shippingServer.Send(NetContract.Serialize(NetMsgType.AdvertiseMove, msg));
                unconfirmedContracts.Add(msg);
            }
            moveContracts.Clear();

            foreach (var info in moveConfirms)
                shippingServer.Send(NetContract.Serialize(NetMsgType.ConfirmMove, info.msg), info.targetCompany);
            moveConfirms.Clear();

            foreach (var msg in localSales)
                bankClient.Send(NetContract.Serialize(NetMsgType.LocalSale, msg));
            localSales.Clear();

            return true; 
        }

        //just add to warehousedGoods and leave for DecideSales to deal with
        private void BuyConfirmed(BankConfirmMsg msg)
        {
            logic.AddGood(msg.transactionID, msg.portID, msg.commodID, msg.quantity);
        }

        //just add to warehousedGoods and leave for DecideSales to deal with
        private void FutureSettled(BankConfirmMsg msg)
        {
            logic.AddGood(msg.transactionID, msg.portID, msg.commodID, msg.quantity);
        }

        private void MoveAccepted(int companyID, MoveContractMsg msg)
        {
            /*if contract hasn't yet been taken, send confirmation and remove from list of untaken contracts */

            int unconfirmedIndex = unconfirmedContracts.FindIndex(
                element => element.transactionID == msg.transactionID);

            if (unconfirmedIndex != -1)
            {
                moveConfirms.Add(new MoveConfirmInfo(msg, companyID));
                unconfirmedContracts.RemoveAt(unconfirmedIndex);
            }
        }

        private void SurplusForecast(ForecastMsg msg)
        {
            if (msg.time < DateTime.Now.AddSeconds(SURPLUS_LEEWAY))
                Console.WriteLine("Ignoring out of date surplus forecast");
            else
                futureRequests.Add(new FutureMsg(msg.portID, msg.commodID, msg.quantity, msg.time));
        }

        private void ShortageForecast(ForecastMsg msg)
        {
            if (msg.time < DateTime.Now.AddSeconds(SURPLUS_LEEWAY))
                Console.WriteLine("Ignoring out of date surplus forecast");
            else
                buyRequests.Add(new BuyMsg(msg.portID, msg.commodID, msg.quantity));
        }        
    }
}
