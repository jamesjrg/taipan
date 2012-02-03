using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Data.SqlClient;

namespace TaiPan.Common
{
    public enum PriceJumpType
    {
        Surplus = 0,
        Shortage = 1
    }

    public class TaiPanException : ApplicationException
    {
        public TaiPanException(string msg) :
            base("FLAGRANT ERROR: " + msg) {}
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

        public static void SetConsoleTitle(string title)
        {
            //will fail if in a unit test
            try
            {
                Console.Title = title;
            }
            catch (Exception e)
            {
            }
        }

        public static Dictionary<string, int> GetPortDistancesLookup(DbConn dbConn)
        {
            var ret = new Dictionary<string, int>();
            int nPorts = (int)dbConn.ExecuteScalar("select count (*) from Port");

            List<int> ports = new List<int>();
            for (int i = 1; i != nPorts + 1; ++i)
                ports.Add(i);

            foreach (int port in ports)
            {
                var otherPorts = new List<int>(ports);
                otherPorts.Remove(port);
                string otherPortsStr = String.Join(",", otherPorts);

                //XXX should do this without string interpolation, though in .NET there is no simple method
                var cmd = new SqlCommand(String.Format(
@"select p.ID, o.ID, p.Location.STDistance(o.Location)
from Port p, Port o where p.ID = @PortID and o.ID in ({0})", otherPortsStr));
                cmd.Parameters.AddWithValue("@PortID", port);
                var reader = dbConn.ExecuteQuery(cmd);
                while (reader.Read())
                {
                    int pid = reader.GetInt32(0);
                    int oid = reader.GetInt32(1);
                    double distance = reader.GetDouble(2);
                    ret[pid + "," + oid] = (int)distance;
                }
                reader.Close();
            }

            return ret;
        }
    }
}
