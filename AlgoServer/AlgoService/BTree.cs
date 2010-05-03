using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AlgoService
{
    class BTree
    {
        private FileStream fs;
        private string filename;
        private const int BRANCH_FACTOR = 8;

        BTree(int id, bool truncate)
        {
            filename = "btree" + id;
            
            if (truncate)
                CreateFile();

            fs = File.Open("btree", FileMode.Open, FileAccess.ReadWrite);
        }

        private void CreateFile()
        {
            if (File.Exists(filename))
                File.Delete(filename);

            File.Create(filename);
        }

        private void ReadFile()
        {
            byte[] b = new byte[1024];
            UTF8Encoding temp = new UTF8Encoding(true);
            while (fs.Read(b,0,b.Length) > 0)
            {
                Console.WriteLine(temp.GetString(b));
            }
        }

        private void WriteFile()
        {
            byte[] info = new UTF8Encoding(true).GetBytes("monkeys");
            fs.Write(info, 0, info.Length);
        }
    }
}
