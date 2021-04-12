using CCWFM.Web.DataLayer;
using System.Runtime.Serialization;

namespace CCWFM.Web.Model
{
    public partial class TblAdjustmentTempDetail
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
        decimal oldQuantity, newQuantity;
        [DataMember]
        public decimal OldQuantity
        {
            get { return oldQuantity; }
            set
            {
                oldQuantity = value;
                //CountedQuantity = OldQuantity + NewQuantity;
                //ItemAdjustment.CountedQuantity = OldQuantity + NewQuantity;
            }
        }
        [DataMember]
        public decimal NewQuantity
        {
            get { return newQuantity; }
            set
            {
                newQuantity = value;
                //CountedQuantity = OldQuantity + NewQuantity;
                //ItemAdjustment.CountedQuantity = OldQuantity + NewQuantity;
            }
        }
    }
}