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
    public partial class TblSalesOrderRequestInvoiceHeader : PropertiesViewModelBase
    {
        public TblSalesOrderRequestInvoiceHeader()
        {
            this.TblSalesOrderRequestInvoiceDetails = new Collection<TblSalesOrderRequestInvoiceDetail>();
        }
    
        public int Iserial { get; set; }
        public string Code { get; set; }
        public int TblJournalAccountType { get; set; }
        public int EntityAccount { get; set; }
        public Nullable<System.DateTime> DocDate { get; set; }
        public string Description { get; set; }
        public Nullable<double> Misc { get; set; }
        public Nullable<double> MiscWithoutItemEffect { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string SupplierInv { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
    
        public Collection<TblSalesOrderRequestInvoiceDetail> TblSalesOrderRequestInvoiceDetails { get; set; }
    }
}
