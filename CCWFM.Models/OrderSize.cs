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
    public partial class OrderSize : PropertiesViewModelBase
    {
        public string SalesOrder { get; set; }
        public string StyleCode { get; set; }
        public string StyleColor { get; set; }
        public string SizeCode { get; set; }
        public int QuantityPerSize { get; set; }
    }
}
