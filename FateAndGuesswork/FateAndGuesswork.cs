using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonLib = TaiPan.Common.Util;

namespace TaiPan.FateAndGuesswork
{
    class FateAndGuesswork : TaiPan.Common.EconomicPlayer
    {
        protected override void Init(string[] args)
        {
            Console.Title = "FateAndGuesswork";
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
