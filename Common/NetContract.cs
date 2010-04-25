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

    public class CurrencyMsg
    {
        public CurrencyMsg() { }
        public CurrencyMsg(int id, decimal USDValue)
        {
            this.id = id;
            this.USDValue = USDValue;
        }

        public int id;
        public decimal USDValue;
    }

    public class StockMsg
    {
        public StockMsg() { }
        public StockMsg(int companyId, decimal price)
        {
            this.companyId = companyId;
            this.price = price;
        }

        public int companyId;
        public decimal price;
    }

    public class CommodityMsg
    {
        public CommodityMsg() {}
        public CommodityMsg(int portId, int commodId, decimal localPrice)
        {
            this.portId = portId;
            this.commodId = commodId;
            this.localPrice = localPrice;
        }

        public int portId;
        public int commodId;
        public decimal localPrice;
    }

    public class SurplusMsg
    {
        public SurplusMsg()
        {
        }
    }

    public class ShortageMsg
    {
        public ShortageMsg()
        {
        }
    }

    public class BuyMsg
    {
        public BuyMsg()
        {
        }
    }

    public class FutureMsg
    {
        public FutureMsg()
        {
        }
    }

    public class AdvertiseMoveMsg
    {
        public AdvertiseMoveMsg()
        {
        }
    }

    public class ConfirmMoveMsg
    {
        public ConfirmMoveMsg()
        {
        }
    }

    public class DepartureMsg
    {
        public DepartureMsg()
        {
        }
    }

    public class ArrivalMsg
    {
        public ArrivalMsg()
        {
        }
    }

    public class AcceptMoveMsg
    {
        public AcceptMoveMsg()
        {
        }
    }

    public class BuyConfirmMsg
    {
        public BuyConfirmMsg()
        {
        }
    }

    public class FutureSettleMsg
    {
        public FutureSettleMsg()
        {
        }
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
                    return typeof(SurplusMsg);
                case NetMsgType.Shortage:
                    return typeof(ShortageMsg);

                //trader
                case NetMsgType.Buy:
                    return typeof(BuyMsg);
                case NetMsgType.Future:
                    return typeof(FutureMsg);
                case NetMsgType.AdvertiseMove:
                    return typeof(AdvertiseMoveMsg);
                case NetMsgType.ConfirmMove:
                    return typeof(ConfirmMoveMsg);

                //shippingcompany
                case NetMsgType.Departure:
                    return typeof(DepartureMsg);
                case NetMsgType.Arrival:
                    return typeof(ArrivalMsg);
                case NetMsgType.AcceptMove:
                    return typeof(AcceptMoveMsg);

                //bank
                case NetMsgType.BuyConfirm:
                    return typeof(BuyConfirmMsg);
                case NetMsgType.FutureSettle:
                    return typeof(FutureSettleMsg);
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

