﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Collections.Specialized;

namespace TaiPan.Common
{
    /// <summary>
    /// Singleton base class for all of the interacting C# processes
    /// </summary>
    public abstract class EconomicPlayer
    {
        protected NameValueCollection AppSettings = new NameValueCollection();
        protected Dictionary<string, ServerConfig> ServerConfigs = new Dictionary<string, ServerConfig>();
        protected readonly string CurrencyAccuracy;
        protected readonly decimal TickVolatility;
        protected readonly int MoveContractAdvertiseTime;

        private readonly int MainLoopTick;
        
        public EconomicPlayer()
        {
            Console.WriteLine("Reading server connection settings from config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Util.configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            
            //this line worked for a while, and then stopped working? This config API totally sucks, I've wasted ludicrous amounts of time fighting it
            //AppSettings = ConfigurationManager.AppSettings;

            /* alternative approach: could instead use KeyValueConfigurationCollection everywhere, but it doesn't support hash lookups and also requires adding assembly references everywhere, lets just copy to a nice easy NameValueCollection */
            foreach (KeyValueConfigurationElement keyValueElement in config.AppSettings.Settings)
                AppSettings.Add(keyValueElement.Key, keyValueElement.Value);
            
            if (AppSettings.Count == 0)
                throw new ApplicationException("Flagrant error attempting to read appSettings from config file");
            MainLoopTick = Convert.ToInt32(AppSettings["MainLoopTick"]);
            CurrencyAccuracy = "F" + Convert.ToInt32(AppSettings["CurrencyAccuracy"]);
            TickVolatility = Convert.ToDecimal(AppSettings["TickVolatility"]);
            MoveContractAdvertiseTime = Convert.ToInt32(AppSettings["MoveContractAdvertiseTime"]);

            ServersSection serversSection = config.GetSection("servers") as ServersSection;
            if (serversSection == null)
                throw new ApplicationException("Couldn't find server connection settings in config file " + Util.configFile);
            ServersCollection servers = serversSection.Servers;
            foreach (ServerElement server in servers)
                ServerConfigs.Add(server.Name, new ServerConfig(server.Name, server.Address, server.Port));
        }

        //no dispose here or in derived classes because when this class is no longer needed, the whole program is ending

        public void Go()
        {
#if DEBUG
            ActualGo();
            Util.ConsolePause();
#else
            try
            {
                ActualGo();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Util.ConsolePause();
            }
#endif
        }

        private void ActualGo()
        {
            Console.WriteLine("Running");
            while (Run() == true)
            {
                System.Threading.Thread.Sleep(MainLoopTick);
            }

            Console.WriteLine("Shutdown");
            //could add shutdown code here, but as everything is managed code or resources under the control of System classes, rely on everything to clear up after itself when program ends. This isn't C++.
        }

        protected abstract bool Run();

        protected int SetID(string title, string[] args)
        {
            int myID;

            try
            {
                myID = Int32.Parse(args[0]);
            }
            catch (Exception)
            {
                throw new ApplicationException("ID must be given as command line argument");
            }

            Console.Title = title + " (ID: " + myID + ")";

            return myID;
        }        
    }    
}
