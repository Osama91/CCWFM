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
    public partial class Inspection : PropertiesViewModelBase
    {
        public int Iserial { get; set; }
        public int Tbl_fabricInspectionHeader { get; set; }
        public string Fabric_Code { get; set; }
        public string ColorCode { get; set; }
        public string BatchNo { get; set; }
        public Nullable<int> ProductCategory { get; set; }
        public int RollNo { get; set; }
        public string Unit { get; set; }
        public float ConsPerPC { get; set; }
        public float NoofPCs { get; set; }
        public float StoreRollQty { get; set; }
        public Nullable<float> M2WeightGm { get; set; }
        public Nullable<float> RollWMT { get; set; }
        public float NetRollWMT { get; set; }
        public float RemainingReservationRollQty { get; set; }
        public double RemainingMarkerRollQty { get; set; }
        public decimal LineNum { get; set; }
        public short Degree { get; set; }
        public string FinishedLocation { get; set; }
        public string FinishedWarehouse { get; set; }
        public string FinishedSite { get; set; }
        public float TotalLineNumQty { get; set; }
        public float UnitPrice { get; set; }
        public string OrgLocation { get; set; }
        public Nullable<float> qtyPerKilo { get; set; }
        public int markerIserial { get; set; }
        public string RollBatch { get; set; }
        public int MarkerTransactionHeader { get; set; }
    }
}
