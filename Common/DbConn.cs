﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace TaiPan.Common
{
    public class DbConn
    {
        private SqlConnection conn;

        public DbConn()
        {
            Init(true);
        }

        public DbConn(bool readOnly)
        {
            Init(readOnly);
        }

        public void Dispose()
        {
            Close();
        }

        public SqlDataReader ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            return cmd.ExecuteReader();
        }

        public void ExecuteNonQuery(string stmt)
        {
            SqlCommand cmd = new SqlCommand(stmt, conn);
            cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            return cmd.ExecuteScalar();
        }

        public void StoredProc(string stmt, List<SqlParameter> parameters)
        {
            SqlCommand cmd = new SqlCommand(stmt, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var par in parameters)
                cmd.Parameters.Add(par);
            cmd.ExecuteNonQuery();
        }

        private void Init(bool readOnly)
        {
            string connName = "taipan-rw";
            if (readOnly)
                connName = "taipan-r";

            Console.WriteLine("Reading config file for database connection");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Util.configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settings = config.ConnectionStrings.ConnectionStrings[connName];
            if (settings == null)
                throw new ApplicationException("Couldn't find connection string \"" + connName + "\" in config file " + Util.configFile);

            Console.WriteLine("Connecting to database with " + connName);
            conn = new SqlConnection(settings.ConnectionString);
            conn.Open();
            Console.WriteLine("Connected to database");
        }

        public void Close()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
        }        
    }
}
