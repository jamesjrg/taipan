using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AlgoService
{
    class SortAlgs
    {
        public static SortReturn InsertionSort(int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            for (int j = 1; j < data.Length; ++j)
            {
                var key = data[j];
                var i = j - 1;

                while (i >= 0 && data[i] > key)
                {
                    data[i + 1] = data[i];
                    i -= 1;
                }
                data[i + 1] = key;
            }

            sw.Stop();
            return new SortReturn(sw.ElapsedMilliseconds, data);
        }


        public static SortReturn MergeSort(int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int[] ret = new int[data.Length];

            sw.Stop();
            return new SortReturn(sw.ElapsedMilliseconds, ret);
        }

        public static SortReturn HeapSort(int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int[] ret = new int[data.Length];

            sw.Stop();
            return new SortReturn(sw.ElapsedMilliseconds, ret);
        }

        public static SortReturn QuickSort(int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int[] ret = new int[data.Length];

            sw.Stop();
            return new SortReturn(sw.ElapsedMilliseconds, ret);
        }

        public static SortReturn QuickSortInPlace(int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();

            sw.Stop();
            return new SortReturn(sw.ElapsedMilliseconds, ret);
        }        
    }
}
