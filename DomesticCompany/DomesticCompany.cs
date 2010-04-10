using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonLib = TaiPan.Common.Util;

namespace TaiPan.DomesticCompany
{
    class DomesticCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        protected override void Init(string[] args)
        {
            myID = SetID("DomesticCompany", args);
        }

        protected override bool Run()
        {
            return true;
        }

        protected override void Shutdown()
        {
        }
    }
}
