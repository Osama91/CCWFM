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
    public partial class ProductionInventDim : PropertiesViewModelBase
    {
        public string PURCHID { get; set; }
        public string DATAAREAID { get; set; }
        public Nullable<System.DateTime> CREATEDDATETIME { get; set; }
        public string Name { get; set; }
        public string ITEMID { get; set; }
        public string PURCHUNIT { get; set; }
        public string CONFIGID { get; set; }
        public int INVENTBATCHID { get; set; }
        public double PURCHQTY { get; set; }
        public int PRICEUNIT { get; set; }
        public int LINEAMOUNT { get; set; }
        public int LINENUM { get; set; }
        public string WMSLOCATIONID { get; set; }
        public string INVENTLOCATIONID { get; set; }
        public string INVENTSITEID { get; set; }
    }
}
