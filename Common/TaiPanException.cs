using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    public class TaiPanException : ApplicationException
    {
        public TaiPanException(string msg) :
            base("FLAGRANT ERROR: " + msg) { }
    }
}
