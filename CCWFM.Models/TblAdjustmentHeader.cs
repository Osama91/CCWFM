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
    public partial class TblAdjustmentHeader : PropertiesViewModelBase
    {
        public TblAdjustmentHeader()
        {
            this.TblAdjustmentDetails = new Collection<TblAdjustmentDetail>();
        }
    
        public int Iserial { get; set; }
        public int WarehouseIserial { get; set; }
        public string Code { get; set; }
        public string CountReference { get; set; }
        public System.DateTime DocDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool Approved { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<System.DateTime> LastChangeDate { get; set; }
        public Nullable<int> LastChangeUser { get; set; }
        public string Notes { get; set; }
    
        public Collection<TblAdjustmentDetail> TblAdjustmentDetails { get; set; }
        public TblAuthUser TblAuthUser { get; set; }
        public TblAuthUser TblAuthUser1 { get; set; }
        public TblWarehouse TblWarehouse { get; set; }
    }
}
