using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.Common
{
    public struct ServerConfig
    {
        public ServerConfig(string name, string address, int port)
        {
            this.name = name;
            this.address = address;
            this.port = port;
        }

        public string name;
        public string address;
        public int port;
    }
}
