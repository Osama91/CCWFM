//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CCWFM.Web.DataLayer
{
    using System;using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    
    //[DataContract]
    public partial class DyeingColorQuantitiesRequiredacc_Result : PropertiesViewModelBase
    {
        public string DyeingClass { get; set; }
        public string SalesOrderID { get; set; }
        public string Fabric_Code { get; set; }
        public string Fabric_Ename { get; set; }
        public string BOM_FabricType { get; set; }
        public string styleheader { get; set; }
        public string unitid { get; set; }
        public string Brand_Ename { get; set; }
        public string FabricColor { get; set; }
        public string Season_Name { get; set; }
        public string OldColor { get; set; }
        public Nullable<double> Total { get; set; }
        public string Size { get; set; }
    }

    public partial class RecInvContractValidation_Result : PropertiesViewModelBase
    {

        public string Style { get; set; }
        public string ColorCode { get; set; }
        public float Quantity { get; set; }
        public float ContractQty { get; set; }
        public float ContractCost { get; set; }
        public float Difference { get; set; }

    }
}
