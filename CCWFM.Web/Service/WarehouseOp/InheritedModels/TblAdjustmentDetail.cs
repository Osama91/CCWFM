using CCWFM.Web.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CCWFM.Web.Model
{
    public partial class TblAdjustmentDetail
    {
        private ItemDimensionAdjustmentSearchModel itemAdjustment;
        [DataMember]
        public virtual ItemDimensionAdjustmentSearchModel ItemAdjustment
        {
            get { return itemAdjustment ?? (itemAdjustment = new ItemDimensionAdjustmentSearchModel()); }
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