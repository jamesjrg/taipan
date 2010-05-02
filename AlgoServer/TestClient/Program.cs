using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("a");
            AlgoServiceClient service = new AlgoServiceClient();
            Console.WriteLine("a");
            /*int result;
            bool specified;
            service.Search(1, true, 1, true, out result, out specified);*/
            int result = service.Search(1, 2);
            Console.WriteLine("a");
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
