using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace TaiPan.SharedLib
{
    public class SharedLib
    {
        public static void ConsolePause()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static SqlConnection GetDbConnection()
        {
            return GetDbConnection(true);
        }

        public static SqlConnection GetDbConnection(bool readOnly)
        {
            string connName = "taipan-rw";
            if (readOnly)
                connName = "taipan-r";

            Console.WriteLine("Reading config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = "SharedLib.config";
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settings = config.ConnectionStrings.ConnectionStrings[connName];
            if (settings == null)
                throw new ApplicationException("Couldn't find connection string in config file: " + connName);

            Console.WriteLine("Connecting to database with " + connName);
            SqlConnection conn = new SqlConnection(settings.ConnectionString);

            return conn;            
        }
    }
}
