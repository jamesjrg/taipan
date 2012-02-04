using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;

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
        protected readonly int MAIN_LOOP_TICK;

        public EconomicPlayer()
        {
            Console.WriteLine("Reading server connection settings from config file");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Globals.CONFIG_FILE;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            
            //this line worked for a while, and then stopped working? This config API totally sucks, I've wasted ludicrous amounts of time fighting it
            //AppSettings = ConfigurationManager.AppSettings;

            /* alternative approach: could instead use KeyValueConfigurationCollection everywhere, but it doesn't support hash lookups and also requires adding assembly references everywhere, lets just copy to a nice easy NameValueCollection */
            foreach (KeyValueConfigurationElement keyValueElement in config.AppSettings.Settings)
                AppSettings.Add(keyValueElement.Key, keyValueElement.Value);
            
            if (AppSettings.Count == 0)
                throw new ApplicationException("Flagrant error attempting to read appSettings from config file");
            MAIN_LOOP_TICK = Convert.ToInt32(AppSettings["MainLoopTick"]);
            CurrencyAccuracy = "F" + Convert.ToInt32(AppSettings["CurrencyAccuracy"]);
            TickVolatility = Convert.ToDecimal(AppSettings["TickVolatility"]);
            MoveContractAdvertiseTime = Convert.ToInt32(AppSettings["MoveContractAdvertiseTime"]);

            Globals.FUEL_RATE = Convert.ToDecimal(AppSettings["FuelRate"]);
            Globals.SHIPPING_COMPANY_RATE = Convert.ToDecimal(AppSettings["ShippingCompanyRate"]);
            Globals.FREIGHTER_SPEED = Convert.ToInt32(AppSettings["FreighterSpeed"]);

            ServersSection serversSection = config.GetSection("servers") as ServersSection;
            if (serversSection == null)
                throw new ApplicationException("Couldn't find server connection settings in config file " + Globals.CONFIG_FILE);
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
                System.Threading.Thread.Sleep(MAIN_LOOP_TICK);
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

            Util.SetConsoleTitle(title + " (ID: " + myID + ")");

            return myID;
        }        
    }    
}
