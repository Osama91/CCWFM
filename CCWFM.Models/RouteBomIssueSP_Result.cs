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
    public partial class RouteBomIssueSP_Result : PropertiesViewModelBase
    {
        public Nullable<long> LineNumber { get; set; }
        public string SalesOrderID { get; set; }
        public string ItemId { get; set; }
        public string BOM_FabricType { get; set; }
        public string StyleCode { get; set; }
        public int OperationIserial { get; set; }
        public string Operation { get; set; }
        public string StyleColor { get; set; }
        public int StyleColorCode { get; set; }
        public string ItemType { get; set; }
        public string Style { get; set; }
        public string UnitID { get; set; }
        public string Brand_Ename { get; set; }
        public string FabricColor { get; set; }
        public string Season_Name { get; set; }
        public string StyleSize { get; set; }
        public string FabricSize { get; set; }
        public double MaterialUsage { get; set; }
        public Nullable<double> Total { get; set; }
        public int FabricColorIserial { get; set; }
        public int SalesOrderIserial { get; set; }
        public string Notes { get; set; }
        public Nullable<float> CostPerUnit { get; set; }
        public Nullable<bool> Dyed { get; set; }
        public bool BOM_IsLocalProduction { get; set; }
        public Nullable<int> DyedColor { get; set; }
        public string DyedColorCode { get; set; }
    }
}
