using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;
using System.Collections.Specialized;

namespace TaiPan.Common
{
    public class Client : TCPConnection, IDisposable
    {
        private readonly int ClientRetryTime;

        private TcpClient tcpClient;
        private NetworkStream ns;

        //note incoming has public access, outgoing does not
        protected SyncQueue<string> outgoing = new SyncQueue<string>();

        public Client(Common.ServerConfig config, NameValueCollection appSettings, int myID, bool receiveOnly):
            base(appSettings)
        {
            ClientRetryTime = Convert.ToInt32(appSettings["ClientRetryTime"]);
            
            tcpClient = AttemptTCPConnect(config);
            ns = tcpClient.GetStream();

            //give them our id
            Console.WriteLine("Sending id: {0}", myID);
            byte[] buffer = BitConverter.GetBytes(myID);
            ns.Write(buffer, 0, 4);
            Console.WriteLine("Sent id");

            Thread receiveThread = new Thread(ReceiveThread);
            receiveThread.Start(ns);

            if (!receiveOnly)
            {
                BroadcastThreadState broadcastState = new BroadcastThreadState(1, ns);
                Thread broadcastThread = new Thread(BroadcastThread);
                broadcastThread.Start(broadcastState);
            }
        }

        public void Dispose()
        {
            Util.CloseTcpClient(tcpClient);
        }

        public void Send(string message)
        {
            outgoing.Enqueue(message);
        }

        override protected string[] OutgoingDequeueAll(int clientID)
        {
            return outgoing.DequeueAll();
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

            throw new TaiPanException("What the hell?");
        }
    }
}
