using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared = TaiPan.SharedLib.SharedLib;

namespace TaiPan.FateAndGuesswork
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Shared.ConsolePause();
            }
        }

        static void Init()
        {
            Console.Title = "FateAndGuesswork";
        }
    }
}
