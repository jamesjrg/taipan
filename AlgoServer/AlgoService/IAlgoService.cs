using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net.Security;

namespace AlgoService
{
    //disable encrypted replies
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface IAlgoService
    {
        [OperationContract]
        SortReturn Sort(string type, int[] data);

        [OperationContract]
        SortReturn CountingSort(int[] data, int maxPossible);

        [OperationContract]
        int CreateStructure(string type, int[] data);

        [OperationContract]
        void DeleteStructure(int id);

        [OperationContract]
        int Search(int id, int key);
    }

    [DataContract]
    public class SortReturn
    {
        public SortReturn(long time, int[] sortedData)
        {
            this.time = time;
            this.sortedData = sortedData;
        }

        public long time = -1;
        public int[] sortedData;
    }
}
