//using CCWFM.ViewModels.PurchaseOrderViewModels;
using _proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class PurchaseOrderMapper
    {
        public static _proxy.tbl_PurchaseOrderHeader MapToModelObject(PurchaseOrderHeaderViewModel objectToBeMapped)
        {
            var temp = new _proxy.tbl_PurchaseOrderHeader();
            if (objectToBeMapped.TransID != null) temp.TransID = (int)objectToBeMapped.TransID;
            temp.WareHouseID = objectToBeMapped.WareHouseItem.WarehouseID;
            temp.Vendor = objectToBeMapped.VendorCode;
            temp.RecieveDate = objectToBeMapped.DocDate;
            temp.CreationDate = objectToBeMapped.CreationDate;
            temp.DelivaryDate = objectToBeMapped.DelivaryDate;
            return temp;
        }

        public static _proxy.tbl_PurchaseOrderDetails MapToModelObject(PurchasOrderDetailsViewModel objectToBeMapped)
        {
            return null;
        }

        public static PurchaseOrderHeaderViewModel MapToViewModelObject(_proxy.tbl_PurchaseOrderHeader objectToBeMapped)
        {
            return null;
        }


    }
}