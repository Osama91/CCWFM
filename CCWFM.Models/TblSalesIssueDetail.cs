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
    public partial class TblSalesIssueDetail : PropertiesViewModelBase
    {
        public int Iserial { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> TblSalesIssueHeader { get; set; }
        public Nullable<int> TblSalesOrderDetailRequest { get; set; }
        public int Type { get; set; }
    
        public TblSalesIssueHeader TblSalesIssueHeader1 { get; set; }
        public TblSalesOrderDetailRequest TblSalesOrderDetailRequest1 { get; set; }
    }
}
