using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonLib = TaiPan.Common.Util;

namespace TaiPan.ShippingCompany
{
    class ShippingCompany : TaiPan.Common.EconomicPlayer
    {
        private int myID;

        public ShippingCompany(string[] args)
        {
            myID = SetID("ShippingCompany", args);
        }        

        protected override bool Run()
        {
            return true;
        }
    }
}
