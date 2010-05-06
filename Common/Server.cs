using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Collections.Specialized;
using TaiPan.Common.NetContract;

namespace TaiPan.Common
{
    public class Server : TCPConnection, IDisposable
    {
        private TcpListener tcpListener;
        public List<ClientConnection> clients = new List<ClientConnection>();
        private bool broadcastOnly;

        protected Dictionary<int, SyncQueue<string>> incoming = new Dictionary<int, SyncQueue<string>>();
        protected MultiHeadQueue outgoing;

        public class ClientConnection
        {
            public ClientConnection(int id, TcpClient client)
            {
                this.id = id;
                this.client = client;
            }

            public TcpClient client;
            public int id;
        }

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
            foreach (ClientConnection client in clients)
            {
                Util.CloseTcpClient(client.client);
            }
            tcpListener.Stop();
        }

        public void Send(string message)
        {
            outgoing.Enqueue(message, 0);
        }

        public void Send(string message, int id)
        {
            outgoing.Enqueue(message, id);
        }

        public List<int> GetClientIDs()
        {
            List<int> IDs = new List<int>();
            foreach (var client in clients)
                IDs.Add(client.id);

            return IDs;
        }

        public List<DeserializedMsg> IncomingDeserializeAll(int clientID)
        {
            List<String> strings = incoming[clientID].DequeueAll();
            return base.SerializeList(strings);
        }

        override protected void IncomingEnqueue(int clientID, string msg)
        {
            incoming[clientID].Enqueue(msg);
        }

        override protected List<string> OutgoingDequeueAll(int clientID)
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
                //get id
                byte[] buffer = new byte[4];
                Console.WriteLine("Waiting for id");
                ns.Read(buffer, 0, 4);
                int clientID = BitConverter.ToInt32(buffer, 0);
                Console.WriteLine("Got id: {0}", clientID);

                clients.Add(new ClientConnection(clientID, tcpClient));
                outgoing.Subscribe(clientID);
                
                TCPThreadState TCPState = new TCPThreadState(clientID, ns);
                ThreadPool.QueueUserWorkItem(BroadcastThread, TCPState);
                if (!broadcastOnly)
                {
                    ThreadPool.QueueUserWorkItem(ReceiveThread, TCPState);
                    incoming[clientID] = new SyncQueue<string>();
                }
            }
        }
    }
}
