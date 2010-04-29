using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using System.Diagnostics;

namespace AlgoService
{
    public class AlgoService : IAlgoService
    {
        private List<DataStructure> structures = new List<DataStructure>();

        public SortReturn Sort(string type, int[] data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            switch (type)
            {
                case "insertion":
                    SortAlgs.InsertionSort(data);
                    break;
                case "merge":
                    SortAlgs.MergeSort(data);
                    break;
                case "heap":
                    SortAlgs.HeapSort(data);
                    break;
                case "quick":
                    SortAlgs.QuickSort(data);
                    break;
                case "randomizedquick":
                    SortAlgs.RandomizedQuickSort(data);
                    break;
                default:
                    throw new FaultException("sort type not recognised");
            }
            sw.Stop();

            return new SortReturn(sw.ElapsedMilliseconds, data);
        }

        public SortReturn CountingSort(int[] data, int maxPossible)
        {
            Stopwatch sw = Stopwatch.StartNew();
            SortAlgs.CountingSort(data, maxPossible);
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
