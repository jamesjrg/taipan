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
        Currency = 0,
        Buy = 1,
        Future = 2,
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
            switch (type)
            {
                case NetMsgType.Currency:
                    return typeof(CurrencyMsg);
                case NetMsgType.Buy:
                    return typeof(BuyMsg);
                case NetMsgType.Future:
                    return typeof(FutureMsg);
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

