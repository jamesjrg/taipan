using System;
using System.Collections.Generic;
using System.Threading;

using TaiPan.Common;

namespace TaiPan.Trader
{
    /// <summary>
    /// Singleton class for Trader process
    /// </summary>
    class Trader : TaiPan.Common.EconomicPlayer
    {
        private int myID;
        private Client fatePoller;

        public Trader(string[] args)
        {
            myID = SetID("Trader", args);

            fatePoller = new Client(ServerConfigs["FateAndGuessWork-TraderBroadcast"], AppSettings);
            Thread thread = new Thread(fatePoller.MainLoop);
            thread.Start();
        }

        protected override bool Run()
        {
            if (fatePoller.messages.Count != 0)
                Console.WriteLine(fatePoller.messages.Dequeue());
            return true; 
        }
    }
}
