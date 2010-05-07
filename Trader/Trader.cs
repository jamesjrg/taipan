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
        private List<MoveContractMsg> moveContracts = new List<MoveContractMsg>();
        private List<MoveContractMsg> unconfirmedContracts = new List<MoveContractMsg>();
        private List<MoveConfirmInfo> moveConfirms = new List<MoveConfirmInfo>();

        private List<WarehousedGood> warehousedGoods = new List<WarehousedGood>();

        private class MoveConfirmInfo
        {
            public MoveConfirmInfo(MoveContractMsg msg, int targetCompany)
            {
                this.msg = msg;
                this.targetCompany = targetCompany;
            }

            public MoveContractMsg msg;
            int targetCompany;
        }

        private class WarehousedGood
        {
            public WarehousedGood(int transactionID, int portID, int commodityID, DateTime saletime)
            {
                this.transactionID = transactionID;
                this.portID = portID;
                this.commodityID = commodityID;
                this.saleTime = saleTime;
            }

            public int transactionID;
            public int portID;
            public int commodityID;
            public DateTime saleTime;            
        }

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
                    default:
                        throw new ApplicationException("fateClient received wrong type of net message");
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

            DecideSales();

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
                shippingServer.Send(NetContract.Serialize(NetMsgType.ConfirmMove, info.msg));
            moveConfirms.Clear();

            return true; 
        }

        private void DecideSales()
        {
            //warehousedGoods
            //moveContracts.Add(new MoveContractMsg(msg.portID, destID, msg.transactionID));
        }

        private void BuyConfirmed(BankConfirmMsg msg)
        {
            //warehousedGoods
        }

        //just add to warehousedGoods and leave for DecideSales to deal with
        private void FutureSettled(BankConfirmMsg msg)
        {
            warehousedGoods.Add(new WarehousedGood(msg.transactionID, msg.portID, msg.commodID, DateTime.Now));
        }

        private void MoveAccepted(int companyID, MoveContractMsg msg)
        {
            //XXX search unconfirmedContracts to check it hasn't already beeen accepted
            moveConfirms.Add(new MoveConfirmInfo(msg, companyID));
            //XXX remove from unconfirmedContracts
        }

        private void SurplusForecast(ForecastMsg msg)
        {
            futureRequests.Add(new FutureMsg(msg.portID, msg.commodID, 1, msg.time));
        }

        private void ShortageForecast(ForecastMsg msg)
        {
        }        
    }
}
