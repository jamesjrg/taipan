using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TaiPan.Common.NetContract
{
    //manually specify the numbers to ease debugging of net messages
    public enum NetMsgType
    {
        //FXServer
        Currency = 0,
        //FateAndGW
        Stock = 1,
        Commodity = 2,
        Surplus = 3,
        Shortage = 4,

        //Trader
        Buy = 10,
        LocalSale = 11,
        Future = 12,
        AdvertiseMove = 13,
        ConfirmMove = 14,

        //Shipping Company
        Departure = 20,
        Arrival = 21,
        AcceptMove = 22,

        //Bank
        BuyConfirm = 30,
        FutureSettle = 31,

        //Shared
        DebugTimer = 40
    }

    public class DeserializedMsg
    {
        public DeserializedMsg(NetMsgType type, object data)
        {
            this.type = type;
            this.data = data;
        }

        public NetMsgType type;
        public object data;
    }

    public class CurrencyMsgItem
    {
        public CurrencyMsgItem() { }
        public CurrencyMsgItem(int id, decimal USDValue)
        {
            this.id = id;
            this.USDValue = USDValue;
        }

        public int id;
        public decimal USDValue;
    }

    public class CurrencyMsg
    {
        public CurrencyMsg() {}
        
        public CurrencyMsgItem[] items;
        public DateTime time;
    }

    public class StockMsgItem
    {
        public StockMsgItem() { }
        public StockMsgItem(int companyID, decimal price)
        {
            this.companyID = companyID;
            this.price = price;
        }

        public int companyID;
        public decimal price;
    }

    public class StockMsg
    {
        public StockMsg() {}
        
        public StockMsgItem[] items;
        public DateTime time;
    }

    public class CommodityMsgItem
    {
        public CommodityMsgItem() { }
        public CommodityMsgItem(int portID, int commodID, decimal localPrice)
        {
            this.portID = portID;
            this.commodID = commodID;
            this.localPrice = localPrice;
        }

        public int portID;
        public int commodID;
        public decimal localPrice;
    }

    public class CommodityMsg
    {
        public CommodityMsg() {}
        
        public CommodityMsgItem[] items;
        public DateTime time;
    }

    public class ForecastMsg
    {
        public ForecastMsg() {}
        public ForecastMsg(int portID, int commodID, int quantity, DateTime time)
        {
            this.portID = portID;
            this.commodID = commodID;
            this.quantity = quantity;
            this.time = time;
        }

        public int portID;
        public int commodID;
        public int quantity;
        public DateTime time;
    }

    public class BuyMsg
    {
        public BuyMsg() { }
        public BuyMsg(int portID, int commodID, int quantity)
        {
            this.portID = portID;
            this.commodID = commodID;
            this.quantity = quantity;
        }

        public int portID;
        public int commodID;
        public int quantity;
    }

    public class LocalSaleMsg
    {
        public LocalSaleMsg() { }
        public LocalSaleMsg(int transactionID)
        {
            this.transactionID = transactionID;
        }

        public int transactionID;
    }

    public class FutureMsg
    {
        public FutureMsg() {}
        public FutureMsg(int portID, int commodID, int quantity, DateTime time)
        {
            this.portID = portID;
            this.commodID = commodID;
            this.quantity = quantity;
            this.time = time;
        }

        public int portID;
        public int commodID;
        public int quantity;
        public DateTime time;
    }

    public class MoveContractMsg
    {
        public MoveContractMsg() { }
        public MoveContractMsg(int departureID, int destID, int transactionID)
        {
            this.departureID = departureID;
            this.destID = destID;
            this.transactionID = transactionID;
        }

        public int departureID;
        public int destID;
        public int transactionID;
    }

    public class MovingMsg
    {
        public MovingMsg() { }
        public MovingMsg(int departPortID, int destPortID, int transactionID, DateTime time)
        {
            this.departPortID = departPortID;
            this.destPortID = destPortID;
            this.transactionID = transactionID;
            this.time = time;
        }

        public int departPortID;
        public int destPortID;
        public int transactionID;
        public DateTime time;
    }

    public class BankConfirmMsg
    {
        public BankConfirmMsg() { }
        public BankConfirmMsg(int portID, int commodID, int quantity, int transactionID, decimal localPrice)
        {
            this.portID = portID;
            this.commodID = commodID;
            this.quantity = quantity;
            this.transactionID = transactionID;
            this.localPrice = localPrice;
        }

        public int portID;
        public int commodID;
        public int quantity;
        public int transactionID;
        public decimal localPrice;
    }

    public class DebugTimerMsg
    {
        public DebugTimerMsg() { }
        public int[] times;
    }

    public class NetContract
    {
        public static void Deserialize(string str, out NetMsgType type, out object data)
        {
            type = NetContract.GetNetMsgTypeFromStr(str);
            data = NetContract.DeserializeData(type, str.Substring(2));
        }

        public static string Serialize(NetMsgType type, object obj)
        {
            Type classType = GetClassFromNetMsgType(type);

            TextWriter tw = new StringWriter();
            XmlSerializer sr = new XmlSerializer(classType);
            string cereal = "";

            try
            {
                sr.Serialize(tw, obj);
                cereal = tw.ToString();
            }
            catch (SerializationException e)
            {
                throw new Exception("Failed to serialize. Reason: " + e.Message);
            }
            finally
            {
                tw.Close();
            }

            //Add message type to front of string, padded to two figures, and a :: end of message delimiter
            return String.Format("{0,2}{1}::", (int)type, cereal);
        }

        private static NetMsgType GetNetMsgTypeFromStr(string str)
        {
            return (NetMsgType)(Int32.Parse(str.Substring(0, 2)));
        }

        private static Type GetClassFromNetMsgType(NetMsgType type)
        {
            switch (type)
            {
                //FXServer
                case NetMsgType.Currency:
                    return typeof(CurrencyMsg);
                //FateAndGW
                case NetMsgType.Stock:
                    return typeof(StockMsg);
                case NetMsgType.Commodity:
                    return typeof(CommodityMsg);
                case NetMsgType.Surplus:
                case NetMsgType.Shortage:
                    return typeof(ForecastMsg);

                //trader
                case NetMsgType.Buy:
                    return typeof(BuyMsg);
                case NetMsgType.LocalSale:
                    return typeof(LocalSaleMsg);
                case NetMsgType.Future:
                    return typeof(FutureMsg);
                case NetMsgType.AdvertiseMove:
                case NetMsgType.ConfirmMove:
                    return typeof(MoveContractMsg);

                //shippingcompany
                case NetMsgType.Departure:
                case NetMsgType.Arrival:
                    return typeof(MovingMsg);
                case NetMsgType.AcceptMove:
                    return typeof(MoveContractMsg);

                //bank
                case NetMsgType.BuyConfirm:
                case NetMsgType.FutureSettle:
                    return typeof(BankConfirmMsg);

                //shared
                case NetMsgType.DebugTimer:
                    return typeof(DebugTimerMsg);

                default:
                    throw new TaiPanException("GetClassFromNetMsgType received unknown type");
            }
        }

        private static object DeserializeData(NetMsgType type, string data)
        {
            Type classType = GetClassFromNetMsgType(type);
            object ret = System.Activator.CreateInstance(classType);

            TextReader tw = new StringReader(data);
            try
            {
                XmlSerializer sr = new XmlSerializer(classType);
                ret = sr.Deserialize(tw);
            }
            catch (SerializationException e)
            {
                throw new Exception("Failed to deserialize. Reason: " + e.Message);
            }
            finally
            {
                tw.Close();
            }

            return ret;
        }
    }
}

