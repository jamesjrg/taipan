using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TaiPan.Common
{
    public abstract class TCPConnection
    {
        protected readonly int OUTGOING_QUEUE_SIZE;
        protected readonly int TCP_THREAD_TICK;

        //note incoming has public access, outgoing does not
        public SyncQueue<string> incoming = new SyncQueue<string>();
        //outgoing in multiheadqueue for server, syncqueue for client

        public TCPConnection(NameValueCollection appSettings)
        {
            OUTGOING_QUEUE_SIZE = Convert.ToInt32(appSettings["MultiHeadQueueSize"]);
            TCP_THREAD_TICK = Convert.ToInt32(appSettings["TCPThreadTick"]);
        }

        protected class BroadcastThreadState
        {
            public BroadcastThreadState(int clientID, NetworkStream ns)
            {
                this.clientID = clientID;
                this.ns = ns;
            }

            public int clientID;
            public NetworkStream ns;
        }

        protected void BroadcastThread(object state)
        {
            BroadcastThreadState castedState = (BroadcastThreadState)state;
            NetworkStream ns = castedState.ns; ;
            StreamWriter sw = new StreamWriter(ns);

            while (true)
            {
                try
                {
                    string[] messagesCopy = OutgoingDequeueAll(castedState.clientID);
                    if (messagesCopy.Length != 0)
                    {
                        foreach (string msg in messagesCopy)
                        {
                            Console.WriteLine("Sending: " + msg);
                            sw.WriteLine(msg);
                        }
                        sw.Flush();
                    }
                    Thread.Sleep(TCP_THREAD_TICK);
                }
                catch (Exception e)
                {
                    if (!(e is IOException || e is ObjectDisposedException))
                        throw;

                    Console.WriteLine("A connection has been lost");
                    sw.Close();
                    ns.Close();
                    break;
                }
            }
        }

        protected void ReceiveThread(object state)
        {
            NetworkStream ns = (NetworkStream)state;
            StreamReader sr = new StreamReader(ns);
            while (true)
            {
                while (ns.DataAvailable)
                    incoming.Enqueue(sr.ReadLine());
                Thread.Sleep(TCP_THREAD_TICK);
            }
        }

        abstract protected string[] OutgoingDequeueAll(int clientID);
    }
}
