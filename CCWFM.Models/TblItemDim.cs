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
    public partial class TblItemDim : PropertiesViewModelBase
    {
        public TblItemDim()
        {
            this.TblAdjustmentDetails = new Collection<TblAdjustmentDetail>();
            this.TblTransferDetails = new Collection<TblTransferDetail>();
            this.TblTransferDetails1 = new Collection<TblTransferDetail>();
            this.TblInventTrans = new Collection<TblInventTran>();
            this.TblSalesOrderDetailRequests = new Collection<TblSalesOrderDetailRequest>();
            this.TblSalesOrderRequestInvoiceDetails = new Collection<TblSalesOrderRequestInvoiceDetail>();
        }
    
        public int Iserial { get; set; }
        public int ItemIserial { get; set; }
        public string ItemType { get; set; }
        public int TblColor { get; set; }
        public string Size { get; set; }
        public string BatchNo { get; set; }
        public int TblSite { get; set; }
    
        public Collection<TblAdjustmentDetail> TblAdjustmentDetails { get; set; }
        public TblColor TblColor1 { get; set; }
        public TblSite TblSite1 { get; set; }
        public Collection<TblTransferDetail> TblTransferDetails { get; set; }
        public Collection<TblTransferDetail> TblTransferDetails1 { get; set; }
        public Collection<TblInventTran> TblInventTrans { get; set; }
        public Collection<TblSalesOrderDetailRequest> TblSalesOrderDetailRequests { get; set; }
        public Collection<TblSalesOrderRequestInvoiceDetail> TblSalesOrderRequestInvoiceDetails { get; set; }
    }
}