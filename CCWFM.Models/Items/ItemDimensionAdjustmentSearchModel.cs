using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCWFM.Web.DataLayer
{
    public class ItemDimensionAdjustmentSearchModel : ItemDimensionSearchModel
    {

        decimal countedQuantity, differenceQuantity, cost;
        public decimal CountedQuantity
        {
            get { return countedQuantity; }
            set
            {
                countedQuantity = value;
                differenceQuantity = countedQuantity - AvailableQuantity;
                RaisePropertyChanged(nameof(DifferenceQuantity));
                RaisePropertyChanged(nameof(CountedQuantity));
            }
        }
        public decimal Cost
        {
            get { return cost; }
            set { cost = value; RaisePropertyChanged(nameof(Cost)); }
        }

        public decimal DifferenceQuantity
        {
            get { return differenceQuantity; }
            set
            {
                differenceQuantity = value;
                countedQuantity = AvailableQuantity + differenceQuantity;
                RaisePropertyChanged(nameof(DifferenceQuantity));
                RaisePropertyChanged(nameof(CountedQuantity));
            }
        }

        string vendor = "";
        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; RaisePropertyChanged(nameof(Vendor)); }
        }
    }
}
