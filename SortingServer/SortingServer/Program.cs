using System;
using System.Collections.Generic;
using System.ServiceModel;
using SortingService;

namespace SortingServer
{
    class Program
    {
        internal static ServiceHost myServiceHost = null;
        
        internal static void StartService()
        {
            myServiceHost = new ServiceHost(typeof(SortingService.SortingService));
            myServiceHost.Open();
        }
        internal static void StopService()
        {
            if (myServiceHost.State != CommunicationState.Closed)
                myServiceHost.Close();
        }
        static void Main()
        {
            StartService();
            Console.WriteLine("Server is running. Press return to exit");
            Console.ReadLine();
            StopService();
        }
    }
}
