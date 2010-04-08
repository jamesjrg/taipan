using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using Shared = TaiPan.SharedLib.SharedLib;

namespace TaiPan.Bank
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection conn = Shared.GetDbConnection())
            {
                conn.Open();
                // Do something useful
                conn.Close();
                Console.WriteLine("Closed database connection");
            }
            Shared.ConsolePause();
        }        
    }
}
