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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;


    //[DataContract]
    public partial class TblAdjustmentDetail : PropertiesViewModelBase
    {

        private int iserial;
        //[DataMember]
        public int Iserial
        {
            get { return iserial; }
            set
            {
                iserial = value; RaisePropertyChanged(nameof(Iserial));
            }
        }

        private int adjustmentHeaderIserial;
        //[DataMember]
        public int AdjustmentHeaderIserial
        {
            get { return adjustmentHeaderIserial; }
            set
            {
                adjustmentHeaderIserial = value; RaisePropertyChanged(nameof(AdjustmentHeaderIserial));
            }
        }

        private int itemDimIserial;
        //[DataMember]
        public int ItemDimIserial
        {
            get { return itemDimIserial; }
            set
            {
                itemDimIserial = value; RaisePropertyChanged(nameof(ItemDimIserial));
            }
        }

        private decimal availableQuantity;
        //[DataMember]
        public decimal AvailableQuantity
        {
            get { return availableQuantity; }
            set
            {
                availableQuantity = value; RaisePropertyChanged(nameof(AvailableQuantity));
            }
        }

        private decimal countedQuantity;
        //[DataMember]
        public decimal CountedQuantity
        {
            get { return countedQuantity; }
            set
            {
                countedQuantity = value; RaisePropertyChanged(nameof(CountedQuantity));
            }
        }

        private decimal differenceQuantity;
        //[DataMember]
        public decimal DifferenceQuantity
        {
            get { return differenceQuantity; }
            set
            {
                differenceQuantity = value; RaisePropertyChanged(nameof(DifferenceQuantity));
            }
        }


        private TblAdjustmentHeader tblAdjustmentHeader;
        //[DataMember]
        public virtual TblAdjustmentHeader TblAdjustmentHeader
        {
            get { return tblAdjustmentHeader ?? (TblAdjustmentHeader = new TblAdjustmentHeader()); }
            set
            {
                if ((ReferenceEquals(tblAdjustmentHeader, value) != true))
                {
                    tblAdjustmentHeader = value; RaisePropertyChanged(nameof(TblAdjustmentHeader));
                }
            }
        }

        private TblItemDim tblItemDim;
        //[DataMember]        
        public virtual TblItemDim TblItemDim
        {
            get { return tblItemDim ?? (TblItemDim = new TblItemDim()); }
            set
            {
                if ((ReferenceEquals(tblItemDim, value) != true))
                {
                    tblItemDim = value; RaisePropertyChanged(nameof(TblItemDim));
                }
            }
        }
        
        private ItemDimensionAdjustmentSearchModel itemAdjustment;
        public virtual ItemDimensionAdjustmentSearchModel ItemAdjustment
        {
            get { return itemAdjustment ?? (ItemAdjustment = new ItemDimensionAdjustmentSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemAdjustment, value) != true))
                {
                    itemAdjustment = value;
                }
            }
        }
    }
}
