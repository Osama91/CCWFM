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
    public partial class V_FabAttr : PropertiesViewModelBase
    {
        public string FabricID { get; set; }
        public int FabricCategoryID { get; set; }
        public string FabricCategoryName { get; set; }
        public string FabricDescription { get; set; }
        public string UoM { get; set; }
        public Nullable<double> ExpectedDyingLossMargin { get; set; }
        public Nullable<double> WidthAsRaw { get; set; }
        public Nullable<double> WeightPerSquarMeterAsRaw { get; set; }
        public Nullable<double> WeightPerSquarMeterBeforWash { get; set; }
        public Nullable<double> WeightPerSquarMeterAfterWash { get; set; }
        public Nullable<double> DyedFabricWidth { get; set; }
        public Nullable<double> HorizontalShrinkage { get; set; }
        public Nullable<double> VerticalShrinkage { get; set; }
        public Nullable<double> Twist { get; set; }
        public string Notes { get; set; }
    }
}