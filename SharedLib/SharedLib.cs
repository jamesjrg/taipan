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
        public static int SetID(string title, string[] args)
        {
            int myID;

            try
            {
                myID = Int32.Parse(args[0]);
            }
            catch (Exception e)
            {
                throw new ApplicationException("ID must be given as command line argument");
            }

            Console.Title = title + " " + myID;

            return myID;
        }

        public static void ConsolePause()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static SqlConnection GetDbConnection()
        {
            return GetDbConnection(true);
        }

        public static SqlConnection GetDbConnectionRW()
        {
            return GetDbConnection(false);
        }

        private static SqlConnection GetDbConnection(bool readOnly)
        {
            string connName = "taipan-rw";
            if (readOnly)
                connName = "taipan-r";
            string configFile = "SharedLib.config";

            Console.WriteLine("Reading config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settings = config.ConnectionStrings.ConnectionStrings[connName];
            if (settings == null)
                throw new ApplicationException("Couldn't find connection string \"" + connName + "\" in config file " + configFile);

            Console.WriteLine("Connecting to database with " + connName);
            SqlConnection conn = new SqlConnection(settings.ConnectionString);

            return conn;            
        }
    }
}
