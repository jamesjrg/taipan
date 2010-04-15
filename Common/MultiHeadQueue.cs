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
        private Dictionary<int, int> heads;
        private int tail;
        private int count;

        public MultiHeadQueue(int size)
        {
            this.size = size;
            syncRoot = new object();
            buffer = new string[size];
            count = 0;
            heads = new Dictionary<int, int>();
            tail = 0;
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
                count++;
            }
        }

        public string[] DequeueAll(int myId)
        {
            string[] values;

            lock (syncRoot)
            {
                values = new string[10];
                int pos = heads[myId];
                for (int i = 0; i < size; i++)
                {
                    values[i] = buffer[pos];
                    pos = (pos + 1) % size;
                }
            }
            return values;
        }
    }
}
