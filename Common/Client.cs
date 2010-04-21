using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections.Specialized;

namespace TaiPan.Common
{
    public class Client : TCPConnection, IDisposable
    {
        private readonly int ClientRetryTime;

        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader sr;

        public Client(Common.ServerConfig config, NameValueCollection appSettings):
            base(appSettings)
        {
            ClientRetryTime = Convert.ToInt32(appSettings["ClientRetryTime"]);
            
            tcpClient = AttemptTCPConnect(config);
            ns = tcpClient.GetStream();
            sr = new StreamReader(ns);

            Thread thread = new Thread(MainLoop);
            thread.Start();
        }

        public void Dispose()
        {
            Util.CloseTcpClient(tcpClient);
        }

        private void MainLoop()
        {
            while (true)
            {
                try
                {
                    while (ns.DataAvailable)
                        incoming.Enqueue(sr.ReadLine());
                    Thread.Sleep(TCP_THREAD_TICK);
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
                    Console.WriteLine("Attempting to connect to " + config.name + " at " + config.address + ":" + config.port);
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
                        Thread.Sleep(ClientRetryTime);
                }
            }

            throw new ApplicationException("What the hell?");
        }
    }
}
