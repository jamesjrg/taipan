using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using TaiPan.Common;

namespace TaiPan.DomesticCompany
{
    class DomesticCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;
        private Client fatePoller;

        protected override void Init(string[] args)
        {
            myID = SetID("DomesticCompany", args);

            fatePoller = new Client(serverConfigs["FateAndGuessWork-DCBroadcast"]);
            Thread thread = new Thread(fatePoller.MainLoop);
            thread.Start();
        }

        protected override bool Run()
        {
            if (fatePoller.messages.Count != 0)
                Console.WriteLine(fatePoller.messages.Dequeue());
            return true;
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
