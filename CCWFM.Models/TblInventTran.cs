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
    using System;
    using System.Collections.Generic;using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    
    
    //[DataContract]
    public partial class TblInventTran : PropertiesViewModelBase
    {
        public int Iserial { get; set; }
        public Nullable<int> TblInventType { get; set; }
        public Nullable<System.DateTime> DocDate { get; set; }
        public decimal Qty { get; set; }
        public Nullable<decimal> ItemCost { get; set; }
        public Nullable<int> VotGlSerial { get; set; }
        public int TblWarehouse { get; set; }
        public int TblItemDim { get; set; }
        public Nullable<decimal> AvgCost { get; set; }
        public Nullable<decimal> LastWarehouseQty { get; set; }
        public string CurrencyCode { get; set; }
    
        public TblInventType TblInventType1 { get; set; }
        public TblItemDim TblItemDim1 { get; set; }
        public TblWarehouse TblWarehouse1 { get; set; }
    }
}
