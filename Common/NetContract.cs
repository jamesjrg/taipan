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
        Buy = 0,
        Future = 1,
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
        public static NetMsgType GetNetMsgType(string msg)
        {
            return (NetMsgType)(Int32.Parse(msg.Substring(0,1)));
        }

        public static string Serialize(NetMsgType type, object data)
        {
            string ret = Serialize(data);
            return type + ret;
        }

        private static string Serialize(object obj)
        {
            TextWriter tw = new StringWriter();
            XmlSerializer sr = new XmlSerializer(typeof(BuyMsg));
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

            return "";
        }

        public static BuyMsg DeserializeBuy(string msg)
        {
            string data = msg.Substring(1);
            // Declare the hashtable reference.
            BuyMsg ret = null;

            // Open the file containing the data that you want to deserialize.
            TextReader tw = new StringReader(data);
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(BuyMsg));

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                ret = (BuyMsg)sr.Deserialize(tw);
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

        public static FutureMsg DeserializeFuture(string msg)
        {
            return new FutureMsg();
        }
    }
}

