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
    public partial class OrderColor : PropertiesViewModelBase
    {
        public string StyleCode { get; set; }
        public string SalesOrder { get; set; }
        public string StyleColor { get; set; }
        public int StyleColorTotal { get; set; }
        public string ItemGroup { get; set; }
        public string Season { get; set; }
        public string StyleBrand { get; set; }
        public string StyleName { get; set; }
        public string productGroup { get; set; }
        public double SalesPrice { get; set; }
        public string Color_Name { get; set; }
        public Nullable<System.DateTime> DelivaryDate { get; set; }
        public string CustomerCode { get; set; }
    }
}
