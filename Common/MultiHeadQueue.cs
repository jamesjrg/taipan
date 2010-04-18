using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    class MultiHeadQueue
    {
        private readonly int size;
        private readonly object syncRoot;

        private string[] buffer;

        //position for next dequeue for each subscriber. -1 means out of sync.
        private Dictionary<int, int> heads;

        //position for next enqueue
        private int tail;

        //number of elements yet to be dequeued for each subscriber
        private Dictionary<int, int> counts;

        public MultiHeadQueue(int size)
        {
            this.size = size;
            syncRoot = new object();
            buffer = new string[size];
            heads = new Dictionary<int, int>();
            tail = 0;
            counts = new Dictionary<int, int>();            
        }

        public int Subscribe()
        {
            lock (syncRoot)
            {
            }
            return 0;
        }

        public void Unsubscribe()
        {
            lock (syncRoot)
            {
            }
        }

        public void Enqueue(string value)
        {
            lock (syncRoot)
            {
                buffer[tail] = value;
                tail = (tail + 1) % size;
                foreach (var entry in counts.Values)
                {
                    counts[entry]++;
                    if (counts[entry] == size)
                        counts[entry] = -1;
                }
            }
        }

        public string[] DequeueAll(int myId)
        {
            if (counts[myId] == -1)
                throw new ApplicationException("FLAGRANT ERROR: Worker thread with id " + myId + " has become to out of sync with data queue");
 
            string[] values;

            lock (syncRoot)
            {
                values = new string[counts[myId]];
                int pos = heads[myId];
                for (int i = 0; i < size; i++)
                {
                    values[i] = buffer[pos];
                    pos = (pos + 1) % size;                    
                }
                heads[myId] = pos;
            }
            return values;
        }
    }
}
    