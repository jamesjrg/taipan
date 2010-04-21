using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AlgoService
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
    [ServiceContract]
    public interface IAlgoService
    {
        [OperationContract]
        StructureReturn CreateStructure(string type, List<int> data);
    }

    [DataContract]
    public class StructureReturn
    {
        int id = -1;
        int time = -1;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public int Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
