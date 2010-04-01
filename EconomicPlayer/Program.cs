using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace TaiPan.EconomicPlayer
{
    public class Program
    {
        static void Main(string[] args)
        {         
        }

        protected static void EndMain()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        protected static void ConnectToDb()
        {
            ConnectToDb(true);
        }

        protected static void ConnectToDb(bool readOnly)
        {
            String user = "taipan-rw";
            if (readOnly)
                String user = "taipan-r";
                
            Console.WriteLine("Connecting to database as " + user);
            string source = "server=DAPHNE-DURON\\SQLEXPRESS;" +
            "User Id="+user+";Password=fakepass;" +
            "database=taipan";
            SqlConnection conn = new SqlConnection(source);
            conn.Open();
            // Do something useful
            conn.Close();
            Console.WriteLine("Closed database connection");
        }
    }
}
