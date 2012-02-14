using System;
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
        SqlConnection _conn;

        //set to true by unit test code, makes all connections use seperate test database
        static public bool testDB = false;

        public DbConn()
        {
            Open(true);
        }

        public DbConn(bool readOnly)
        {
            Open(readOnly);
        }

        public void Dispose()
        {
            Close();
        }

        public SqlDataReader ExecuteQuery(SqlCommand cmd)
        {
            cmd.Connection = _conn;
            return cmd.ExecuteReader();
        }

        public SqlDataReader ExecuteQuery(string query)
        {
            return ExecuteQuery(new SqlCommand(query));
        }

        public void ExecuteNonQuery(SqlCommand cmd)
        {
            cmd.Connection = _conn;
            cmd.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string query)
        {
            ExecuteNonQuery(new SqlCommand(query));
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            cmd.Connection = _conn;
            return cmd.ExecuteScalar();
        }

        public object ExecuteScalar(string query)
        {
            return ExecuteScalar(new SqlCommand(query));
        }

        public void StoredProc(SqlCommand cmd)
        {
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public DataSet FilledDataSet(string query)
        {
            var adaptor = new SqlDataAdapter(query, _conn);
            var dataSet = new DataSet();
            adaptor.Fill(dataSet);
            return dataSet;
        }

        public void Open(bool readOnly)
        {
            string connName = "taipan-rw";
            if (readOnly)
                connName = "taipan-r";
            if (testDB)
                connName += "-test";

            Console.WriteLine("Reading config file for database connection");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Globals.CONFIG_FILE;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settings = config.ConnectionStrings.ConnectionStrings[connName];
            if (settings == null)
                throw new ApplicationException("Couldn't find connection string \"" + connName + "\" in config file " + Globals.CONFIG_FILE);

            Console.WriteLine("Connecting to database with " + connName);
            _conn = new SqlConnection(settings.ConnectionString);
            _conn.Open();
            Console.WriteLine("Connected to database");
        }

        public void Close()
        {
            if (_conn.State != System.Data.ConnectionState.Closed)
                _conn.Close();
        }

        //for unit tests
        public SqlConnection UnderlyingConnection()
        {
            return _conn;
        }
    }
}
