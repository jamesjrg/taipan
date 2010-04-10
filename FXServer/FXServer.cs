using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using CommonLib = TaiPan.Common.Util;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TaiPan.FXServer
{
    class FXServer : TaiPan.Common.EconomicPlayer
    {
        private TcpListener tcpListener;
        List<TcpClient> tcpClients = new List<TcpClient>();
        
        protected override void Init(string[] args)
        {
            Console.Title = "FXServer";

            Thread thread = new Thread(Listen);
            thread.Start();
        }

        protected override bool Run()
        {
            return true;
        }

        protected override void Shutdown()
        {
            foreach (TcpClient client in tcpClients)
            {
                CommonLib.CloseTcpClient(client);
            }
            tcpListener.Stop();
        }

        public void Listen()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            Int32 port = 6100;
            tcpListener = new TcpListener(localAddr, port);
            tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Server accepted new connection");
                tcpClients.Add(tcpClient);
                ThreadPool.QueueUserWorkItem(Broadcast, tcpClient);
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
                    sw.WriteLine("Important info");
                    sw.Flush();
                    Console.WriteLine("Important info");
                    Thread.Sleep(SLEEPTIME);
                }
                catch (Exception e)
                {
                    if (!(e is IOException || e is ObjectDisposedException))
                        throw;

                    Console.WriteLine("Client connection lost, still waiting for new connections");
                    sw.Close();
                    ns.Close();
                    break;
                }
            }
        }
    }
}
