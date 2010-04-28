using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AlgoService
{
    public class AlgoService : IAlgoService
    {
        private List<DataStructure> structures = new List<DataStructure>();

        public SortReturn Sort(string type, int[] data)
        {
            switch (type)
            {
                case "insertion":
                    return SortAlgs.InsertionSort(data);
                case "merge":
                    return SortAlgs.MergeSort(data);
                case "heap":
                    return SortAlgs.HeapSort(data);
                case "quick":
                    return SortAlgs.QuickSort(data);
                case "randomizedquick":
                    return SortAlgs.RandomizedQuickSort(data);
                default:
                    throw new FaultException("sort type not recognised");
            }
        }

        public SortReturn CountingSort(int[] data, int maxPossible)
        {
            return SortAlgs.CountingSort(data, maxPossible);
        }

        public StructureReturn CreateStructure(string type, int[] data)
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
