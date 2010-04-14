using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.DomesticCompany
{
    class Program
    {
        static void Main(string[] args)
        {
            DomesticCompany company = new DomesticCompany(args);
            company.Go();
        }
    }
}
