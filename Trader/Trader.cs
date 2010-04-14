using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonLib = TaiPan.Common.Util;

namespace TaiPan.Trader
{
    class Trader : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        protected override void Init(string[] args)
        {
            myID = SetID("Trader", args);
        }

        protected override bool Run()
        {
            return true;
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
