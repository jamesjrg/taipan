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
        SortReturn TimeSort(string type, int[] data, int iterations);

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

        [DataMember]
        public long time = -1;

        [DataMember]
        public int[] sortedData;
    }
}
