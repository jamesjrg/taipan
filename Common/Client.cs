using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TaiPan.Common
{
    public class Client: IDisposable
    {
        public Queue<string> messages = new Queue<string>();
        
        private const int RETRY_TIME = 500;
        private const int SLEEP_TIME = 500;

        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader sr;

        public Client(Common.ServerConfig config)
        {
            tcpClient = AttemptTCPConnect(config);
            ns = tcpClient.GetStream();
            sr = new StreamReader(ns);
        }

        public void Dispose()
        {
            Util.CloseTcpClient(tcpClient);
        }

        public void MainLoop()
        {
            while (true)
            {
                try
                {
                    while (ns.DataAvailable)
                        messages.Enqueue(sr.ReadLine());
                    Thread.Sleep(SLEEP_TIME);
                }
                catch (Exception e)
                {
                    if (!(e is IOException))
                        throw;

                    Console.WriteLine("Disconnected from server");
                }
            }
        }

        private TcpClient AttemptTCPConnect(Common.ServerConfig config)
        {
            int attempts = 5;
            for (int i = 0; i != attempts; ++i)
            {
                if (i == 0)
                    Console.WriteLine("Attempting to connect to " + config.name);
                else
                    Console.WriteLine("Connection attempt " + (i + 1) + " of " + attempts);
                try
                {
                    TcpClient client = new TcpClient(config.address, config.port);
                    Console.WriteLine("Client connected");
                    return client;
                }
                catch (Exception e)
                {
                    if (i == attempts - 1)
                        throw e;
                    else
                        Thread.Sleep(RETRY_TIME);
                }
            }

            throw new ApplicationException("What the hell?");
        }
    }
}
