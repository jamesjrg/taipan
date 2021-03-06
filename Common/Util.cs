﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

namespace TaiPan.Common
{
    public class Util
    {
        public static void ConsolePause()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static void CloseTcpClient(TcpClient client)
        {
            if (client != null && client.Connected)
                client.Close();
        }

        public static void SetConsoleTitle(string title)
        {
            //will fail if in a unit test
            try
            {
                Console.Title = title;
            }
            catch (Exception e)
            {
            }
        }        
    }
}
