using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    [DataContract]
    public class ItemsDto
    {
        [DataMember]
        public int Iserial { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Config { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string Batch { get; set; }

        [DataMember]
        [ReadOnly(true)]
        public string Desc { get; set; }

        [DataMember]
        public byte[] Image { get; set; }

        [DataMember]
        public bool? IsSizeIncluded { get; set; }

        [DataMember]
        public List<ImageDto> Images { get; set; }

        [DataMember]
        [ReadOnly(true)]
        public string Name { get; set; }

        [DataMember]
        [ReadOnly(true)]
        public string ItemGroup { get; set; }

        [DataMember]
        [ReadOnly(true)]
        public string Unit { get; set; }

        [DataMember]
        public IEnumerable<tbl_AccessoryAttributesDetails> CombinationList { get; set; }

        [DataMember]
        public IEnumerable<string> SizesList { get; set; }

        [DataMember]
        public List<TblColor> AccConfigList { get; set; }
    }

    [DataContract]
    public class GlPosting
    {
        [DataMember]
        public int GroupIserial { get; set; }

        [DataMember]
        public int StoreIserial { get; set; }

        [DataMember]
        public decimal NetSales { get; set; }

        [DataMember]
        public int PaymentMethod { get; set; }

        [DataMember]
        public int ItemIserial { get; set; }

        [DataMember]
        public int CustIserial { get; set; }

        [DataMember]
        public int AccountTemp { get; set; }

        [DataMember]
        public decimal CostCenterAmount { get; set; }

        [DataMember]
        public decimal DrAmount { get; set; }

        [DataMember]
        public decimal CrAmount { get; set; }

        [DataMember]
        public int TblCostCenter { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public TblAccount AccountPerRow { get; set; }

        [DataMember]
        public string Brand { get; set; }
    }

    [DataContract]
    public class MarkerPostToRouteProcedure
    {
        [DataMember]
        public int sizeqtyReal { get; set; }

        [DataMember]
        public string meterpersizecode { get; set; }
    }

    [DataContract]
    public class Users
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string SID { get; set; }
        [DataMember]
        public bool isMapped { get; set; }
    }


}