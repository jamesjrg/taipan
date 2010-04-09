using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared = TaiPan.SharedLib.SharedLib;

namespace TaiPan.ShippingCompany
{
    class Program
    {
        internal static int myID;

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
            myID = Shared.SetID("ShippingCompany", args);
        }
    }
}
