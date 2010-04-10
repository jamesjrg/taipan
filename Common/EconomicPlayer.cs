using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;

namespace TaiPan.Common
{
    public abstract class EconomicPlayer
    {
        protected const int SLEEPTIME = 500;

        public void Go(string[] args)
        {
            try
            {
                try
                {
                    Console.WriteLine("Initialising");
                    Init(args);

                    Console.WriteLine("Running");
                    while (Run() == true)
                    {
                        System.Threading.Thread.Sleep(SLEEPTIME);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Console.WriteLine("Shutdown");
                Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Util.ConsolePause();
            }
        }

        protected abstract void Init(string[] args);
        protected abstract bool Run();
        protected abstract void Shutdown();

        protected int SetID(string title, string[] args)
        {
            int myID;

            try
            {
                myID = Int32.Parse(args[0]);
            }
            catch (Exception)
            {
                throw new ApplicationException("ID must be given as command line argument");
            }

            Console.Title = title + " " + myID;

            return myID;
        }

        public static TcpClient AttemptTCPConnect(string host, int port, string desc)
        {
            int attempts = 5;
            for (int i = 0; i != attempts; ++i)
            {
                if (i == 0)
                    Console.WriteLine("Attempting to connect to " + desc);
                else
                    Console.WriteLine("Connection attempt " + (i + 1) + " of " + attempts);
                try
                {
                    TcpClient client = new TcpClient(host, port);
                    Console.WriteLine("Client connected");
                    return client;
                }
                catch (Exception e)
                {
                    if (i == attempts - 1)
                        throw e;
                    else
                        Thread.Sleep(SLEEPTIME);
                }
            }

            throw new ApplicationException("What the hell?");
        }
    }    
}
