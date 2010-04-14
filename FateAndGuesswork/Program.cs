using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.FateAndGuesswork
{
    class Program
    {
        static void Main(string[] args)
        {
            FateAndGuesswork fate = new FateAndGuesswork(args);
            fate.Go();
        }
    }
}