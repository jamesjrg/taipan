using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    /// <summary>
    /// Just a thread safe wrapper around a generic queue. Also adds a threadsafe DequeueAll method.
    /// </summary>
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

        public List<T> DequeueAll()
        {
            List<T> ret;
            lock (internalQueue)
            {
                ret = new List<T>(internalQueue.Count);

                for (int i = 0; i < internalQueue.Count; i++)
                {
                    ret.Add(internalQueue.Dequeue());
                }                
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
