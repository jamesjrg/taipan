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

    public class CurrencyMsg
    {
        public CurrencyMsg() { }

        public CurrencyMsg(string shortName, decimal USDValue)
        {
            this.shortName = shortName;
            this.USDValue = USDValue;
        }

        public string shortName;
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
        public static NetMsgType GetNetMsgTypeFromStr(string msg)
        {
            return (NetMsgType)(Int32.Parse(msg.Substring(0,1)));
        }

        public static Type GetClassFromNetMsgType(NetMsgType type)
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

        public static string Serialize(NetMsgType type, object obj)
        {
            Type classType = GetClassFromNetMsgType(type);

            TextWriter tw = new StringWriter();
            XmlSerializer sr = new XmlSerializer(classType);
            string ret = "";

            try 
            {
                sr.Serialize(tw, obj);
                ret = tw.ToString();
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

            //Add message type to front of string
            return type + ret;
        }

        public static object Deserialize(NetMsgType type, string msg)
        {
            string data = msg.Substring(1);
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

