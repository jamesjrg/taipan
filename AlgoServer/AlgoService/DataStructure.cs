﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    interface DataStructure
    {
        void Insert(int key, int val);
        void Delete(int key);
        int Search(int key);
    }
}
