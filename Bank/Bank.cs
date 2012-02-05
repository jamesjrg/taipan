using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using TaiPan.Common;
using TaiPan.Common.NetContract;

namespace TaiPan.Bank
{
    /// <summary>
    /// Singleton class for Bank process
    /// </summary>
    class Bank : TaiPan.Common.EconomicPlayer
    {
        public class ConfirmInfo
        {
            public ConfirmInfo(int traderID, int portID, int commodID, int quantity, int transactionID, decimal localPrice)
            {
                this.traderID = traderID;
                this.msg = new BankConfirmMsg(portID, commodID, quantity, transactionID, localPrice);
            }

            public BankConfirmMsg msg;
            public int traderID;
        }

        private BankDBLogic dbLogic;

        private Server traderServer;
        private Server shippingServer;

        private Client fxClient;
        private Client fateClient;

        private List<ConfirmInfo> confirmedBuys = new List<ConfirmInfo>();
        private List<ConfirmInfo> settledFutures = new List<ConfirmInfo>();

        public Bank(string[] args)
        {
            Util.SetConsoleTitle("Bank");

            dbLogic = new BankDBLogic();

            traderServer = new Server(ServerConfigs["Bank-Trader"], AppSettings, false);
            shippingServer = new Server(ServerConfigs["Bank-Shipping"], AppSettings, false);

            fxClient = new Client(ServerConfigs["FXServer-Bank"], AppSettings, 1, true);
            fateClient = new Client(ServerConfigs["FateAndGuesswork-Bank"], AppSettings, 1, true);
        }

        protected override bool Run()
        {
            dbLogic.FutureSettlements(settledFutures);

            List<DeserializedMsg> fxIncoming = fxClient.IncomingDeserializeAll();
            foreach (var msg in fxIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Currency:
                        dbLogic.UpdateCurrency((CurrencyMsg)(msg.data));
                        break;                    
                    default:
                        throw new ApplicationException("fxClient received wrong type of net message: " + msg.type);
                }
            }

            List<DeserializedMsg> fateIncoming = fateClient.IncomingDeserializeAll();
            foreach (var msg in fateIncoming)
            {
                switch (msg.type)
                {
                    case NetMsgType.Commodity:
                        dbLogic.UpdateCommodity((CommodityMsg)(msg.data));
                        break;
                    case NetMsgType.Stock:
                        dbLogic.UpdateStock((StockMsg)(msg.data));
                        break;
                    default:
                        throw new ApplicationException("fateIncoming received wrong type of net message: " + msg.type);
                }
            }

            foreach (var client in traderServer.clients)
            {
                List<DeserializedMsg> traderIncoming = traderServer.IncomingDeserializeAll(client.id);
                foreach (var msg in traderIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.Buy:
                            dbLogic.EnactBuy(client.id, (BuyMsg)msg.data, confirmedBuys);
                            break;
                        case NetMsgType.LocalSale:
                            dbLogic.EnactLocalSale((LocalSaleMsg)msg.data);
                            break;
                        case NetMsgType.Future:
                            dbLogic.EnactFuture(client.id, (FutureMsg)msg.data);
                            break;
#if true
                        case NetMsgType.DebugTimer:
                            DebugTimerMsg debugMsg = (DebugTimerMsg)msg.data;
                            debugMsg.times = debugMsg.times.Concat(new int[] { Environment.TickCount }).ToArray();
                            Console.WriteLine(String.Join(", ",
                                debugMsg.times.Select((x, i) => {
                                    if (i == 0)
                                        return 0;
                                    else
                                        return x - debugMsg.times[i-1];
                                })));
                            break;
#endif

                        default:
                            throw new ApplicationException("traderServer received wrong type of net message: " + msg.type);
                    }
                }
            }

            foreach (var client in shippingServer.clients)
            {
                List<DeserializedMsg> shippingIncoming = shippingServer.IncomingDeserializeAll(client.id);
                foreach (var msg in shippingIncoming)
                {
                    switch (msg.type)
                    {
                        case NetMsgType.Departure:
                            dbLogic.ShipDeparted(client.id, (MovingMsg)msg.data);
                            break;
                        case NetMsgType.Arrival:
                            dbLogic.ShipArrived(client.id, (MovingMsg)msg.data);
                            break;
                        default:
                            throw new ApplicationException("traderServer received wrong type of net message: " + msg.type);
                    }
                }
            }

            foreach (var info in confirmedBuys)
                traderServer.Send(NetContract.Serialize(NetMsgType.BuyConfirm, info.msg), info.traderID);
            confirmedBuys.Clear();

            foreach (var info in settledFutures)
                traderServer.Send(NetContract.Serialize(NetMsgType.FutureSettle, info.msg), info.traderID);
            settledFutures.Clear();

            return true;
        }           
    }
}
