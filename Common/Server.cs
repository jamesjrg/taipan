using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using CommonLib = TaiPan.Common.Util;
using System.Collections.Specialized;

namespace TaiPan.Common
{
    public class Server
    {
        /*
         * WARNING WARNING WARNING shared between threads
         * 
        */
        public List<string> messages = new List<string>();

        private TcpListener tcpListener;
        private List<TcpClient> tcpClients = new List<TcpClient>();
        private List<int> clientIds;
        
        private readonly int ServerLoopTick;

        public Server(Common.ServerConfig config, NameValueCollection appSettings)
        {
            ServerLoopTick = Convert.ToInt32(appSettings["ServerLoopTick"]);

            Thread thread = new Thread(Listen);
            thread.Start(config);
        }

        public void Dispose()
        {
            foreach (TcpClient client in tcpClients)
            {
                CommonLib.CloseTcpClient(client);
            }
            tcpListener.Stop();
        }

        public void Listen(object o)
        {
            ServerConfig config = (ServerConfig)o;
            IPAddress localAddr = IPAddress.Parse(config.address);
            tcpListener = new TcpListener(localAddr, config.port);
            tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Server accepted new connection");
                tcpClients.Add(tcpClient);
                ThreadPool.QueueUserWorkItem(Broadcast, tcpClient);
            }
        }

        public void Send(string message)
        {
            lock (messages)
            {
                messages.Add(message);

                if (messages.Count > 10)
                    messages.RemoveRange(0, 5);
            }            
        }

        public void Broadcast(object state)
        {   
            TcpClient tcpClient = (TcpClient)state;
            NetworkStream ns = tcpClient.GetStream();
            StreamWriter sw = new StreamWriter(ns);

            while (true)
            {
                try
                {
                    lock (messages)
                    {
                        if (messages.Count != 0)
                        {
                            foreach (string msg in messages)
                            {
                                Console.WriteLine("Sending: " + msg);
                                sw.WriteLine(msg);
                            }
                            sw.Flush();
                        }
                    }
                    Thread.Sleep(ServerLoopTick);
                }
                catch (Exception e)
                {
                    if (!(e is IOException || e is ObjectDisposedException))
                        throw;

                    Console.WriteLine("A client connection has been lost");
                    sw.Close();
                    ns.Close();
                    break;
                }
            }
        }
    }
}
