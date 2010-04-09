using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared = TaiPan.SharedLib.SharedLib;

namespace TaiPan.Trader
{
    class Program : TaiPan.SharedLib.EconomicPlayer
    {
        static void Main(string[] args)
        {
            try
            {
                Init(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Shared.ConsolePause();
            }
        }

        static void Init(string[] args)
        {
            myID = Shared.SetID("Trader", args);
        }
    }
}
