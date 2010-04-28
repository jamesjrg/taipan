using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    interface DataStructure
    {
        public void Insert(int key);
        public void Delete(int key);
        public int Search(int key);
    }
}
