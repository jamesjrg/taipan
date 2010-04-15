using System;
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

        private readonly int MainLoopTick;

        public EconomicPlayer()
        {
            Console.WriteLine("Reading server connection settings from config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Util.configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            AppSettings = ConfigurationManager.AppSettings;
            if (AppSettings.Count == 0)
                throw new ApplicationException("Flagrant error attempting to read appSettings from config file");
            MainLoopTick = Convert.ToInt32(AppSettings["MainLoopTick"]);

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
            try
            {
                try
                {
                    Console.WriteLine("Running");
                    while (Run() == true)
                    {
                        System.Threading.Thread.Sleep(MainLoopTick);
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
