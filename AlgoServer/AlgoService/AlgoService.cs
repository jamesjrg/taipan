using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using System.Diagnostics;

using AlgoLib;

namespace AlgoService
{
    public class AlgoService : IAlgoService
    {
        private List<DataStructure> structures = new List<DataStructure>();
        delegate void SortDel(int[] data);

        public SortReturn TimeSort(string type, int[] data, int iterations)
        {
            SortDel del;
            
            switch (type)
            {
                case "insertion":
                    del = SortAlgs.InsertionSort;
                    break;
                case "merge":
                    del = SortAlgs.MergeSort;
                    break;
                case "heap":
                    del = SortAlgs.HeapSort;
                    break;
                case "quick":
                    del = SortAlgs.QuickSort;
                    break;
                case "randomizedquick":
                    del = SortAlgs.RandomizedQuickSort;
                    break;
                case "counting":
                    del = SortAlgs.CountingSort;
                    break;
                default:
                    throw new FaultException("sort type not recognised");
            }

            //don't sort a pre-sorted array!
            int[] argData = new int[data.Length];
            Array.Copy(data, argData, data.Length);
            
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i != iterations; ++i)
            {
                del.Invoke(argData);
                Array.Copy(data, argData, data.Length);
            }
            sw.Stop();

            return new SortReturn(sw.ElapsedMilliseconds, data);
        }

        public int CreateStructure(string type, int[] data)
        {            
            try
            {
                int id = 1;
                return id;
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public void DeleteStructure(int id)
        {
            try
            {
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public int Search(int id, int key)
        {
            try
            {
                return 1;
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
