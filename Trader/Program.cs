using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Trader
{
    class Program
    {
        static void Main(string[] args)
        {
            Trader trader = new Trader(args);
            trader.Go();
        }
    }
}
