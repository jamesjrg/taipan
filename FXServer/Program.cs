using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.FXServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FXServer fx = new FXServer();
            fx.Go(args);
        }
    }
}