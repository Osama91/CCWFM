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
    public partial class TblSalesOrderRequestMarkupTransProd : PropertiesViewModelBase
    {
        public int Iserial { get; set; }
        public int TblMarkupProd { get; set; }
        public int Type { get; set; }
        public int TblSalesOrderHeaderRequest { get; set; }
        public int TblCurrency { get; set; }
        public bool VendorEffect { get; set; }
        public Nullable<double> MiscValue { get; set; }
        public int MiscValueType { get; set; }
        public int TblJournalAccountType { get; set; }
        public int EntityAccount { get; set; }
    }
}