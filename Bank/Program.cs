using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Bank
{
    class Program
    {   
        static void Main(string[] args)
        {
            Bank bank = new Bank();
            bank.Go(args);
        }       
    }
}
