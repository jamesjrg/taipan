using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Bank
{
    class Program : TaiPan.EconomicPlayer.Program
    {
        static void Main(string[] args)
        {
            ConnectToDb();
            EndMain();
        }        
    }
}
