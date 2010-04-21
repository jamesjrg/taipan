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
        public StructureReturn CreateStructure(string type, List<int> data)
        {
            StructureReturn ret = new StructureReturn();
            ret.Id = 0;
            ret.Time = 0;
            return ret;
        }

        public StructureReturn TimeStructureToArray(int structureId)
        {
            StructureReturn ret = new StructureReturn();
            ret.Id = 0;
            ret.Time = 0;
            return ret;
        }

        public bool DeleteStructure(int structureId)
        {
            return true;
        }

        public int TimeSort(string type, List<int> data)
        {
            return 0;
        }
    }
}
