using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    /// <summary>
    /// An implementation of a HashTable. Uses chaining to deal with hash collisions.
    /// </summary>
    class HashTable : DataStructure
    {
        private struct Element
        {
            public Element(int key, int val)
            {
                this.key = key;
                this.val = val;
            }

            public int key;
            public int val;
        }

        /*I have chosen to use the system linked list to store the actual values rather than implementing my own linked list. This makes the whole thing ridiculously inefficient as 
        ultimately I could just use the system Dictionary class, but that's not the exercise*/
        private List<Element>[] data;

        //this is neccessary for a correct implementation of the multiplicative hash function, looks like the vast majority of people in the internet get this wrong
        private int lg_size = 5;
        private int size = 2 ^ 5;
        private int currentCount = 0;

        public HashTable()
        {
            data = new List<Element>[size];
        }

        public void Insert(int key, int val)
        {
            //XXX quite possibly we should first check to see whether the key/value combination is already in the map - currently it is possible to have duplicates

            //check if map is getting overloaded (just defined as number of elements equal to number of chains, which possibly isn't the best heuristic), if not resize
            currentCount++;
            if (currentCount > size)
                Resize();

            int index = Hash(key);
            data[index].Add(new Element(key,val));
        }

        public void Delete(int key)
        {
            //XXX finish me
        }

        public int Search(int key)
        {
            int index = Hash(key);
            var chain = data[index];

            foreach (Element el in chain)
            {
                if (el.key == key)
                    return el.val;
            }

            throw new Exception("element not found");
        }

        private void Resize()
        {
            //make new array
            List<Element>[] newData = new List<Element>[data.Length * 2];
            lg_size += 1;

            //init each element to a list, this is expensive and if this were C you wouldn't have to bother as everything would just be pointers (I imagine there is some sort of workaround, but anyway)
            for (int i = 0; i != newData.Length; ++i)
                newData[i] = new List<Element>();

            //rehash all old elements and put into new array
            foreach (var chain in data)
            {
                foreach (var el in chain)
                {
                    int index = Hash(el.key);
                    newData[index].Add(new Element(el.key, el.val));
                }
            }

            //and leave the garbage collector to free the old data
            data = newData;
        }

        /*knuth's method: multiply k by 2654435769 { == ((sqrt(5) - 1)/2 ) * 2^32 } to create a 64 bit value and then take the p most significant bits of the lower order word of the product as the hash (see page 264 of algorithms book).
         * 
         * The vast majority of attempts to implement this algorithm with integer arithmetic found with google are incorrect - taking significant bits is not the same as modulo, and the bit shift has to depend on the lg of the array size. It gets even more confusing if you use open addressing, as then you need to do things like double hashing, which is even harder to get right. This version of single hashing using knuth's method, found at http://www.cs.hmc.edu/~geoff/classes/hmc.cs070.200101/homework10/hashfuncs.html looks correct to me */
        private int Hash(int key)
        {
            uint s = 2654435761;
            uint x = (uint)key * s;
            uint hash = x >> (32 - lg_size);
            
            return (int)hash;
        }

    }
}
