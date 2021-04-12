using System.Runtime.Serialization;

namespace CCWFM.Web.Service.Dto_s
{
    [DataContract]
    public class WareHouseDto
    {
        [DataMember]
        public string WareHouseCode { get; set; }

        [DataMember]
        public string WareHouseName { get; set; }
    }
}