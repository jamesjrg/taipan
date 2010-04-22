using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Collections.Specialized;

namespace TaiPan.Common
{
    public class Server : TCPConnection, IDisposable
    {
        private TcpListener tcpListener;
        private List<TcpClient> clients = new List<TcpClient>();
        private bool broadcastOnly;

        //note incoming has public access, outgoing does not
        protected MultiHeadQueue outgoing;

        public Server(Common.ServerConfig config, NameValueCollection appSettings, bool broadcastOnly):
            base(appSettings)
        {
            this.broadcastOnly = broadcastOnly;
            outgoing = new MultiHeadQueue(OUTGOING_QUEUE_SIZE);

            Thread thread = new Thread(ListenForNew);
            thread.Start(config);
        }

        public void Dispose()
        {
            foreach (TcpClient client in clients)
            {
                Util.CloseTcpClient(client);
            }
            tcpListener.Stop();
        }

        public void Send(string message)
        {
            outgoing.Enqueue(message);
        }

        //unused argument, have to mirror function used by server broadcast threads.
        //this is messy, should refactor, but then hard to make BroadcastThread code shared
        override protected string[] OutgoingDequeueAll(int clientID)
        {
            return outgoing.DequeueAll(clientID);
        }

        private void ListenForNew(object o)
        {
            ServerConfig config = (ServerConfig)o;
            IPAddress localAddr = IPAddress.Parse(config.address);
            tcpListener = new TcpListener(localAddr, config.port);
            Console.WriteLine("Server starting on " + config.address + ":" + config.port);
            tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                NetworkStream ns = tcpClient.GetStream();

                Console.WriteLine("Server accepted new connection");
                clients.Add(tcpClient);

                //get id
                byte[] buffer = new byte[4];
                Console.WriteLine("Waiting for id");
                ns.Read(buffer, 0, 4);
                int clientID = BitConverter.ToInt32(buffer, 0);
                Console.WriteLine("Got id: {0}", clientID);

                outgoing.Subscribe(clientID);

                BroadcastThreadState broadcastState = new BroadcastThreadState(clientID, ns);
                ThreadPool.QueueUserWorkItem(BroadcastThread, broadcastState);
                if (!broadcastOnly)
                    ThreadPool.QueueUserWorkItem(ReceiveThread, ns);
            }
        }
    }
}
