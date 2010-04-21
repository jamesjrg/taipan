using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace TaiPan.Common
{
    public class TCPConnection
    {
        private readonly int MESSAGE_QUEUE_SIZE;
        protected readonly int TCP_THREAD_TICK;

        //note incoming has public access, outgoing does not
        public SyncQueue<string> incoming = new SyncQueue<string>();
        protected MultiHeadQueue outgoing;

        public TCPConnection(NameValueCollection appSettings)
        {
            MESSAGE_QUEUE_SIZE = Convert.ToInt32(appSettings["MultiHeadQueueSize"]);
            TCP_THREAD_TICK = Convert.ToInt32(appSettings["TCPThreadTick"]);
            
            outgoing = new MultiHeadQueue(MESSAGE_QUEUE_SIZE);
        }
    }
}
