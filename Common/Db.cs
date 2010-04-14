﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace TaiPan.Common
{
    public class Db
    {
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

            Console.WriteLine("Reading config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Util.configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settings = config.ConnectionStrings.ConnectionStrings[connName];
            if (settings == null)
                throw new ApplicationException("Couldn't find connection string \"" + connName + "\" in config file " + Util.configFile);

            Console.WriteLine("Connecting to database with " + connName);
            SqlConnection conn = new SqlConnection(settings.ConnectionString);
            Console.WriteLine("Connected to database");

            return conn;
        }

        public static void CloseConn(SqlConnection dbConn)
        {
            if (dbConn.State != System.Data.ConnectionState.Closed)
                dbConn.Close();
        }
    }
}
