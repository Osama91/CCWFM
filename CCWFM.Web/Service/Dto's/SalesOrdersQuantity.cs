using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [DataContract]
        public class SalesOrderDto
        {
            [DataMember]
            public float FinalQty { get; set; }

            [DataMember]
            public float IntialQty { get; set; }

            [DataMember]
            public string SalesOrder { get; set; }

            [DataMember]
            public string SalesOrderColor { get; set; }
        }
    }
}