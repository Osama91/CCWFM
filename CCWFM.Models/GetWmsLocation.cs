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
    public partial class GetWmsLocation : PropertiesViewModelBase
    {
        public int POSITION { get; set; }
        public int LEVEL_ { get; set; }
        public int RACK { get; set; }
        public string INVENTLOCATIONID { get; set; }
        public string WMSLOCATIONID { get; set; }
        public string CHECKTEXT { get; set; }
        public int SORTCODE { get; set; }
        public int MANUALSORTCODE { get; set; }
        public int MANUALNAME { get; set; }
        public string AISLEID { get; set; }
        public int LOCATIONTYPE { get; set; }
        public decimal HEIGHT { get; set; }
        public decimal WIDTH { get; set; }
        public decimal DEPTH { get; set; }
        public decimal VOLUME { get; set; }
        public string INPUTLOCATION { get; set; }
        public decimal ABSOLUTEHEIGHT { get; set; }
        public string DATAAREAID { get; set; }
        public int RECVERSION { get; set; }
        public long RECID { get; set; }
        public string PALLETTYPEGROUPID { get; set; }
        public string STOREAREAID { get; set; }
        public int MAXPALLETCOUNT { get; set; }
        public string INPUTBLOCKINGCAUSEID { get; set; }
        public string OUTPUTBLOCKINGCAUSEID { get; set; }
        public string PICKINGAREAID { get; set; }
        public string VENDID { get; set; }
        public string CUSTID { get; set; }
        public string INSPECTIONLOC1 { get; set; }
        public string INSPECTIONLOC2 { get; set; }
        public string INSPECTIONLOC3 { get; set; }
    }
}
