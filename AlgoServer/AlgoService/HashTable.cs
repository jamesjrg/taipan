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
        /*I have chosen to use the system linked list to store the actual values rather than implementing my own linked list. This makes the whole thing ridiculously inefficient as 
        ultimately I could just use the system Dictionary class, but that's not the exercise*/
        private List<int>[] data;

        private const int START_SIZE = 32;

        public void Insert(int key)
        {
            //check if there is space, if not resize
            //if 
            int index = Hash(key);
            data[index].Add(key);
        }

        public void Delete(int key)
        {
        }

        public int Search(int key)
        {
            return 1;
        }

        private void Resize()
        {
            //make new array
            List<int>[] newData = new List<int>[data.Length * 2];
            //init each element to a list, this is expensive and if this were C you wouldn't have to bother as everything would just be pointers (I imagine there is some sort of workaround, but anyway)
            for (int i = 0; i != newData.Length; ++i)
                newData[i] = new List<int>();

            //rehash all old elements and put into new array
            foreach (var el in data)
            {
                //
            }

            //and leave the garbage collector to free the old data
            data = newData;
        }

        //Multiplication method
        private int Hash(int key)
        {
            uint ukey = (uint)key;

            /*relatively "simple" version would be to multiply k by 2654435769 { == ((sqrt(5) - 1)/2 ) * 2^32 } to create a 64 bit value and then take the p most significant bits of the lower order word of the product as the hash (see page 264 of algorithms book) 
             * 
            However rather than faffing with bit manipulations myself I just copied a variation of this idea from the internet, "Robert Jenkins' 32 bit Mix Function", which still uses a 64 bit value created using the magic number 2654435769, but seems to do something a bit crazy to mix up the bits. I'm a software developer, not a mathematician:*/

            ukey += (ukey << 12);
            ukey ^= (ukey >> 22);
            ukey += (ukey << 4);
            ukey ^= (ukey >> 9);
            ukey += (ukey << 10);
            ukey ^= (ukey >> 2);
            ukey += (ukey << 7);
            ukey ^= (ukey >> 12);
	        
            ukey = (ukey >> 3) * 2654435761;

            ukey = ukey % (uint)data.Length;

	        return (int)ukey;
        }

    }
}
