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
    public partial class Vendor_CCR : PropertiesViewModelBase
    {
        public string vendor_code { get; set; }
        public string vendor_ename { get; set; }
        public int Vendor_LeadTime { get; set; }
        public int Vendor_PaymentTerms { get; set; }
        public string DATAAREAID { get; set; }
    }
}
