using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

namespace TaiPan.Common
{
    public enum PriceJumpType
    {
        Surplus = 0,
        Shortage = 1
    }

    public class DisconnectedException : ApplicationException
    {
    }

    public struct ServerConfig
    {
        public ServerConfig(string name, string address, int port)
        {
            this.name = name;
            this.address = address;
            this.port = port;
        }

        public string name;
        public string address;
        public int port;
    }

    public class Util
    {
        public const string configFile = "Common.config";

        public static void ConsolePause()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static void CloseTcpClient(TcpClient client)
        {
            if (client != null && client.Connected)
                client.Close();
        }
    }
}
