using System;
using System.Collections.Generic;
using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;
using System.Data.SqlClient;

namespace TaiPan.Trader
{
    /// <summary>
    /// Singleton class for Trader process
    /// </summary>
    class Trader : EconomicPlayer
    {
        private int myID;

        private DbConn dbConn;

        private Server shippingServer;

        private Client bankClient;
        private Client fateClient;

        private Dictionary<string, int> portDistances;

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
            public WarehousedGood(int transactionID, int portID, int commodityID, int quantity, DateTime saleTime)
            {
                this.transactionID = transactionID;
                this.portID = portID;
                this.commodityID = commodityID;
                this.quantity = quantity;
                this.saleTime = saleTime;
            }

            public int transactionID;
            public int portID;
            public int commodityID;
            public int quantity;
            public DateTime saleTime;           
        }

        public Trader(string[] args, bool testing = false)
        {
            myID = SetID("Trader", args);

            dbConn = new DbConn();
            portDistances = GetPortDistancesLookup(dbConn);

            var conf = ServerConfigs["Trader-Shipping"];
            conf.port = conf.port + (myID - 1);

            if (!testing)
            {
                shippingServer = new Server(conf, AppSettings, false);
                bankClient = new Client(ServerConfigs["Bank-Trader"], AppSettings, myID, false);
                fateClient = new Client(ServerConfigs["FateAndGuesswork-Trader"], AppSettings, myID, true);
            }
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
            foreach (var good in warehousedGoods)
            {   
                var salePorts = new List<Tuple<int, decimal, decimal>>();

                SqlCommand cmd = new SqlCommand(
@"select pcp.PortId, pcp.LocalPrice, Currency.USDValue from PortCommodityPrice pcp
join Port p on pcp.PortID = p.ID
join Country on p.CountryID = Country.ID
join Currency on Country.CurrencyID = Currency.ID
where CommodityId = @CID");
                cmd.Parameters.AddWithValue("@CID", good.commodityID);
                SqlDataReader reader = dbConn.ExecuteQuery(cmd);

                while (reader.Read())
                {
                    int portID = reader.GetInt32(0);
                    decimal localPrice = reader.GetDecimal (1);
                    decimal USDValue = reader.GetDecimal(2);
                    salePorts.Add(new Tuple<int, decimal, decimal>(portID, localPrice, USDValue));
                }
                reader.Close();
            
                int bestPort = 0;
                decimal bestProfit = 0;

                foreach (var port in salePorts)
                {
                    //XXX currently selling to the port good is being stored at is not possible
                    if (good.portID == port.Item1)
                        continue;

                    decimal profit = (good.quantity * port.Item2 * port.Item3)
                        - SHIPPING_COMPANY_RATE *
                        portDistances[good.portID + "," + port.Item1];

                    if (profit > bestProfit) 
                    {
                        bestPort = port.Item1;
                        bestProfit = profit;
                    }
                }

                moveContracts.Add(new MoveContractMsg(good.portID, bestPort, good.transactionID));               
            }

            warehousedGoods.Clear();
        }

        //just add to warehousedGoods and leave for DecideSales to deal with
        private void BuyConfirmed(BankConfirmMsg msg)
        {
            warehousedGoods.Add(new WarehousedGood(msg.transactionID, msg.portID, msg.commodID, msg.quantity, DateTime.Now));
        }

        //just add to warehousedGoods and leave for DecideSales to deal with
        private void FutureSettled(BankConfirmMsg msg)
        {
            warehousedGoods.Add(new WarehousedGood(msg.transactionID, msg.portID, msg.commodID, msg.quantity, DateTime.Now));
        }

        private void MoveAccepted(int companyID, MoveContractMsg msg)
        {
            /*if contract hasn't yet been taken, send confirmation and remove from list of untaken contracts and from warehoused goods */

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
            futureRequests.Add(new FutureMsg(msg.portID, msg.commodID, msg.quantity, msg.time));
        }

        private void ShortageForecast(ForecastMsg msg)
        {
            buyRequests.Add(new BuyMsg(msg.portID, msg.commodID, msg.quantity));
        }        
    }
}
