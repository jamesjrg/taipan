using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TaiPan.Common.NetContract
{
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
        Buy = 5,
        Future = 6,
        AdvertiseMove = 7,
        ConfirmMove = 8,

        //Shipping Company
        Departure = 9,
        Arrival = 10,
        AcceptMove = 11,

        //Bank
        BuyConfirm = 12,
        FutureSettle = 13,

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
        public MovingMsg(int portID, int transactionID, DateTime time)
        {
            this.portID = portID;
            this.transactionID = transactionID;
            this.time = time;
        }

        public int portID;
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
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
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
            //XXX this could maybe be a dict, but then couldn't be static
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
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                tw.Close();
            }

            return ret;
        }
    }
}

