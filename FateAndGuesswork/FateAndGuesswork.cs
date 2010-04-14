using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonLib = TaiPan.Common.Util;

namespace TaiPan.FateAndGuesswork
{
    class FateAndGuesswork : TaiPan.Common.EconomicPlayer
    {
        private TaiPan.Common.Server server;

        public FateAndGuesswork(string[] args)
        {
            Console.Title = "FateAndGuesswork";

            server = new TaiPan.Common.Server(serverConfigs["FateAndGuessWork-DCBroadcast"]);
        }

        protected override bool Run()
        {
            server.Send("Blah");
            return true;
        }
    }
}
