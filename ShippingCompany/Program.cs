using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.ShippingCompany
{
    class Program
    {
        static void Main(string[] args)
        {
            ShippingCompany company = new ShippingCompany(args);
            company.Go();
        }
    }
}
