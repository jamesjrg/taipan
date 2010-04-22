using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    /// <summary>
    /// A data structure a bit like a normal thread safe queue, except:
    /// a) it manages a list of "subscribers", each of whom maintain a seperate head index into the queue - dequeueing only advances the callers head, not everyone else's
    /// b) it is possible for enqueued values to be added for all subscribers, or just one subscriber
    /// c) rather than dequeueing a single item at a time, the DequeueAll method dequeues all remaining values (for the calling subscriber).
    /// </summary>
    public class MultiHeadQueue
    {
        private readonly int size;
        private readonly object syncRoot;

        private struct Item
        {
            public int targetID;
            public string data;

            public Item(int targetID, string data)
            {
                this.targetID = targetID;
                this.data = data;
            }
        }

        private Item[] buffer;
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
            buffer = new Item[size];
            subscribers = new Dictionary<int, Subscriber>();
            newHead = 0;
            newCount = 0;
            tail = 0;
        }

        public int Subscribe(int myID)
        {
            lock (syncRoot)
            {
                if (subscribers.ContainsKey(myID))
                    throw new TaiPanException("Subscribe request for id already subscribed");

                subscribers.Add(myID, new Subscriber(newHead, newCount));
            }
            return 0;
        }

        public void Unsubscribe(int myID)
        {
            lock (syncRoot)
            {
                if (!subscribers.ContainsKey(myID))
                    throw new TaiPanException("Unsubscribe request for id not subscribed");
                subscribers.Remove(myID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetID">0 means to everyone</param>
        public void Enqueue(string value, int targetID)
        {
            lock (syncRoot)
            {
                buffer[tail].data = value;
                buffer[tail].targetID = targetID;
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

        public List<string> DequeueAll(int myID)
        {
            if (subscribers[myID].count == -1)
                throw new TaiPanException("Worker thread with id " + myID + " has become to out of sync with data queue");

            List<string> values;

            lock (syncRoot)
            {
                values = new List<string>();

                int pos = subscribers[myID].head;
                for (int i = 0; i < subscribers[myID].count; i++)
                {
                    if (buffer[pos].targetID == myID || buffer[pos].targetID == 0)
                        values.Add(buffer[pos].data);
                    pos = (pos + 1) % size;          
                }
                subscribers[myID].head = pos;
                subscribers[myID].count = 0;
            }
            return values;
        }
    }
}
    