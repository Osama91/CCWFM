using CCWFM.Web.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CCWFM.Web.Model
{
    public partial class TblProductionOrderFabric
    {
        private ItemDimensionSearchModel itemTransfer;
        [DataMember]
        public virtual ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new ItemDimensionSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemTransfer, value) != true))
                {
                    itemTransfer = value;
                }
            }
        }
    }
}