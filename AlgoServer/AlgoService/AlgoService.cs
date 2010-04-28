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
        public SortReturn Sort(string type, int[] data)
        {
            switch (type)
            {
                case "insertion":
                    return SortAlgs.InsertionSort(data);
                default:
                    throw new FaultException("sort type not recognised");
            }
        }

        public StructureReturn CreateStructure(string type, int[] data)
        {
            StructureReturn ret = new StructureReturn();
            ret.id = 0;
            ret.time = 0;
            return ret;
        }

        public bool DeleteStructure(int structureId)
        {
            return true;
        }
    }
}
