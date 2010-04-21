using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    public class SyncQueue<T>
    {
        private Queue<T> internalQueue = new Queue<T>();

        public void Enqueue(T item)
        {
            lock (internalQueue)
            {
                internalQueue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T ret;
            lock (internalQueue)
            {
                ret = internalQueue.Dequeue();
            }
            return ret;
        }

        public int Count
        {
            get {
                int ret;
                lock (internalQueue)
                {
                    ret = internalQueue.Count;
                }
                return ret;
            }
        }
    }
}
