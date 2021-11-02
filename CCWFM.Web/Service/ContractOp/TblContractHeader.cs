using System.Runtime.Serialization;

namespace CCWFM.Web.Model
{
    public partial class TblContractHeader
    {
        TBLsupplier supplier;
        [DataMember]
        public TBLsupplier Supplier
        {
            get { return supplier; }
            set { supplier = value; }
        }
        TblCurrencyTest currency;
        [DataMember]
        public TblCurrencyTest Currency
        {
            get { return currency; }
            set { currency = value; }
        }
    }
    public partial class TblContractDetail
    {
        //public decimal Total
        //{
        //    get
        //    {
        //        decimal result = 0;
        //        switch (TblContractHeader1.TblRetailOrderProductionType)
        //        {
        //            case 1:
        //                result = Qty * Cost;
        //                break;
        //            case 2:
        //                result = Qty * (Cost - AccCost);
        //                break;
        //            case 3:
        //                result = Qty * (Cost - AccCost - FabricCost);
        //                break;
        //            default:
        //                result = Qty * Cost;
        //                break;
        //        }
        //        return result;
        //    }
        //}
    }
}