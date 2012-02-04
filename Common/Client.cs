using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;
using System.Collections.Specialized;
using TaiPan.Common.NetContract;

namespace TaiPan.Common
{
    public class Client : TCPConnection, IDisposable
    {
        private readonly int ClientRetryTime;

        private TcpClient tcpClient;
        private NetworkStream ns;

        protected SyncQueue<string> incoming = new SyncQueue<string>();
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

            TCPThreadState TCPState = new TCPThreadState(myID, ns);

            Thread receiveThread = new Thread(ReceiveThread);
            receiveThread.Start(TCPState);

            if (!receiveOnly)
            {
                Thread broadcastThread = new Thread(BroadcastThread);
                broadcastThread.Start(TCPState);
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

        public List<DeserializedMsg> IncomingDeserializeAll()
        {
            List<String> strings = incoming.DequeueAll();
            return base.SerializeList(strings);
        }
        
        override protected void IncomingEnqueue(int clientID, string msg)
        {
            incoming.Enqueue(msg);
        }

        override protected List<string> OutgoingDequeueAll(int clientID)
        {
            return outgoing.DequeueAll();
        }

        private TcpClient AttemptTCPConnect(Common.ServerConfig config)
        {
            int attempts = 20;
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
