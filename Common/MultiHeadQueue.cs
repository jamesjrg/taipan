using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    /// <summary>
    /// A data structure a bit like a normal thread safe queue, except it manages a list of "subscribers", each of whom maintain a seperate head index into the queue - dequeueing only advances the callers head, not everyone else's. Also, rather than dequeueing a single item at a time, the DequeueAll method dequeues all remaining values (for the calling subscriber).
    /// </summary>
    class MultiHeadQueue
    {
        private readonly int size;
        private readonly object syncRoot;

        private string[] buffer;
        private Dictionary<int, Subscriber> subscribers;

        //position for next enqueue
        private int tail;

        //values for new subscribers
        private int newHead;
        private int newCount;

        private class Subscriber
        {
            //position for next dequeue for each subscriber. -1 means out of sync.
            public int head;
            //number of elements yet to be dequeued for each subscriber
            public int count;

            public Subscriber(int head, int count)
            {
                this.head = head;
                this.count = count;
            }
        }

        public MultiHeadQueue(int size)
        {
            this.size = size;
            syncRoot = new object();
            buffer = new string[size];
            subscribers = new Dictionary<int, Subscriber>();
            newHead = 0;
            newCount = 0;
            tail = 0;
        }

        public int Subscribe(int myId)
        {
            lock (syncRoot)
            {
                if (subscribers.ContainsKey(myId))
                    throw new ApplicationException("FLAGRANT ERROR: Subscribe request for id already subscribed");

                subscribers.Add(myId, new Subscriber(newHead, newCount));
            }
            return 0;
        }

        public void Unsubscribe(int myId)
        {
            lock (syncRoot)
            {
                if (!subscribers.ContainsKey(myId))
                    throw new ApplicationException("FLAGRANT ERROR: Unsubscribe request for id not subscribed");
                subscribers.Remove(myId);
            }
        }

        public void Enqueue(string value)
        {
            lock (syncRoot)
            {
                buffer[tail] = value;
                tail = (tail + 1) % size;
                foreach (var key in subscribers.Keys)
                {
                    subscribers[key].count++;
                    if (subscribers[key].count == size)
                        subscribers[key].count = -1;
                }

                if (newCount < size)
                {
                    newCount++;
                }
                else
                    newHead = tail;
            }
        }

        public string[] DequeueAll(int myId)
        {
            if (subscribers[myId].count == -1)
                throw new ApplicationException("FLAGRANT ERROR: Worker thread with id " + myId + " has become to out of sync with data queue");
 
            string[] values;

            lock (syncRoot)
            {
                values = new string[subscribers[myId].count];

                int pos = subscribers[myId].head;
                for (int i = 0; i < subscribers[myId].count; i++)
                {
                    values[i] = buffer[pos];
                    pos = (pos + 1) % size;                    
                }
                subscribers[myId].head = pos;
                subscribers[myId].count = 0;
            }
            return values;
        }
    }
}
    