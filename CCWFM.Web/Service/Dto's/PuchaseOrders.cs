using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [DataContract]
        public class PurchaseOrderDto
        {
            [DataMember]
            public DateTime CreatedDate { get; set; }

            [DataMember]
            public string DataArea { get; set; }

            [DataMember]
            public string JournalId { get; set; }

            [DataMember]
            public string VendorCode { get; set; }

            [DataMember]
            public string VendorName { get; set; }

            [DataMember]
            public int Status { get; set; }
        }

        [DataContract]
        public class PurchaseOrderDetailDto
        {
            [DataMember]
            public  string Brand { get; set; }
            [DataMember]
            public int TblBrandSection { get; set; }
            [DataMember]
            public int TblSeason { get; set; }
            [DataMember]
            public string ItemId { get; set; }

            [DataMember]
            public float Quantity { get; set; }

            [DataMember]
            public decimal LineNumber { get; set; }

            [DataMember]
            public string Unit { get; set; }

            [DataMember]
            public float UnitPrice { get; set; }

            [DataMember]
            public float TotalPrice { get; set; }

            [DataMember]
            public string ItemName { get; set; }

            [DataMember]
            public string Location { get; set; }

            [DataMember]
            public string Warehouse { get; set; }

            [DataMember]
            public string FabricColor { get; set; }

            [DataMember]
            public string BatchNo { get; set; }

            [DataMember]
            public string Site { get; set; }

            [DataMember]
            public ObservableCollection<SalesOrderDto> SalesOrdersList { get; set; }
        }
    }
}