using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using TaiPan.Common;

namespace TaiPan.DomesticCompany
{
    /// <summary>
    /// Singleton class for DomesticCompany process
    /// </summary>
    class DomesticCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;
        private Client fatePoller;

        public DomesticCompany(string[] args)
        {
            myID = SetID("DomesticCompany", args);

            fatePoller = new Client(ServerConfigs["FateAndGuessWork-DCBroadcast"], AppSettings);
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
