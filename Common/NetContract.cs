using System;
using System.Collections.Generic;
using System.Text;

namespace TaiPan.Common.NetContract
{
    public enum NetMsgType
    {
        TraderToBankBuy = 0,
        TraderToBankFuture = 1,
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
            return NetMsgType.TraderToBankBuy;
        }

        public static string EncodeMessage(NetMsgType type, object data)
        {
            string ret = "";
            return ret;
        }

        public static BuyMsg DecodeBuy(string msg)
        {
            return new BuyMsg();
        }

        public static FutureMsg DecodeFuture(string msg)
        {
            return new FutureMsg();
        }
    }
}

