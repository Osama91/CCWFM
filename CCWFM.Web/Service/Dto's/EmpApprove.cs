using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [DataContract]
        public class EmpApprove
        {
            [DataMember]
            public int Iserial { get; set; }

            [DataMember]
            public string StatusName { get; set; }

            [DataMember]
            public string Name { get; set; }
        }
    }
}