using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.ServiceModel;

using Shared = TaiPan.SharedLib.SharedLib;

namespace TaiPan.Bank
{
    class Program
    {   
        internal static ServiceHost myServiceHost = null;

        static void Main(string[] args)
        {        
            try
            {
                Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Shared.ConsolePause();
            }
        }

        static void Init()
        {
            Console.Title = "Bank";

            using (SqlConnection conn = Shared.GetDbConnectionRW())
            {
                conn.Open();
                string select = "SELECT Name FROM Company";
                SqlCommand cmd = new SqlCommand(select, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine("{0}",
                reader.GetString(0));
                reader.Close();

                string update = "UPDATE Currency SET USDValue = 10 WHERE ID = 1";
                cmd = new SqlCommand(update, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
