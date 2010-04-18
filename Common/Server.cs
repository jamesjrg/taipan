﻿using System;
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
        private const int MESSAGE_QUEUE_SIZE = 10;

        private MultiHeadQueue messages = new MultiHeadQueue(MESSAGE_QUEUE_SIZE);

        private TcpListener tcpListener;
        private List<ClientSubscriber> subscribers = new List<ClientSubscriber>();
        
        private readonly int ServerLoopTick;

        private class ClientSubscriber
        {
            public int myId;
            public TcpClient client;

            public ClientSubscriber(int myId, TcpClient client)
            {
                this.myId = myId;
                this.client = client;
            }
        }

        public Server(Common.ServerConfig config, NameValueCollection appSettings)
        {
            ServerLoopTick = Convert.ToInt32(appSettings["ServerLoopTick"]);

            Thread thread = new Thread(Listen);
            thread.Start(config);
        }

        public void Dispose()
        {
            foreach (ClientSubscriber subscriber in subscribers)
            {
                CommonLib.CloseTcpClient(subscriber.client);
            }
            tcpListener.Stop();
        }

        public void Listen(object o)
        {
            ServerConfig config = (ServerConfig)o;
            IPAddress localAddr = IPAddress.Parse(config.address);
            tcpListener = new TcpListener(localAddr, config.port);
            tcpListener.Start();
            int newSubscriberId = 1;
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Server accepted new connection");
                ClientSubscriber subscriber = new ClientSubscriber(newSubscriberId, tcpClient);
                subscribers.Add(subscriber);
                ThreadPool.QueueUserWorkItem(Broadcast, subscriber);
                newSubscriberId++;
            }
        }

        public void Send(string message)
        {
            messages.Enqueue(message);
        }

        public void Broadcast(object state)
        {
            ClientSubscriber subscriber = (ClientSubscriber)state;
            TcpClient tcpClient = subscriber.client;
            int myId = subscriber.myId;
            NetworkStream ns = tcpClient.GetStream();
            StreamWriter sw = new StreamWriter(ns);

            messages.Subscribe(myId);

            while (true)
            {
                try
                {
                    string[] messagesCopy = messages.DequeueAll(myId);
                    if (messagesCopy.Length != 0)
                    {
                        foreach (string msg in messagesCopy)
                        {
                            Console.WriteLine("Sending: " + msg);
                            sw.WriteLine(msg);
                        }
                        sw.Flush();
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
