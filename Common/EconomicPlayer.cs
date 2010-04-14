﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

namespace TaiPan.Common
{
    public abstract class EconomicPlayer
    {
        protected const int SLEEP_TIME = 10;
        protected Dictionary<string, ServerConfig> serverConfigs = new Dictionary<string, ServerConfig>();

        public EconomicPlayer()
        {
            ReadConfig();
        }

        //no dispose here or in derived classes because when this class is no longer needed, the whole program is ending

        private void ReadConfig()
        {
            Console.WriteLine("Reading server connection settings from config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Util.configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ServersSection serversSection = config.GetSection("servers") as ServersSection;
            if (serversSection == null)
                throw new ApplicationException("Couldn't find server connection settings in config file " + Util.configFile);

            ServersCollection servers = serversSection.Servers;

            foreach (ServerElement server in servers)
                serverConfigs.Add(server.Name, new ServerConfig(server.Name, server.Address, server.Port));
        }

        public void Go()
        {
            try
            {
                try
                {
                    Console.WriteLine("Running");
                    while (Run() == true)
                    {
                        System.Threading.Thread.Sleep(SLEEP_TIME);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Console.WriteLine("Shutdown");
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

            Console.Title = title + " " + myID;

            return myID;
        }        
    }    
}