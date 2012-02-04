using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TaiPan.Common
{
    public class DBUtil
    {
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
                string otherPortsStr = String.Join(",", otherPorts);

                var cmd = new SqlCommand(
@"select p.ID, o.ID, p.Location.STDistance(o.Location)
from Port p, Port o
where p.ID = @PortID");
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
