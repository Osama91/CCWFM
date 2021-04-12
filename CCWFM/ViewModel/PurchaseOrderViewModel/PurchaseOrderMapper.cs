using CCWFM.ViewModels.PurchaseOrderViewModels;
using _proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.PurchaseOrderViewModel
{
    public static class PurchaseOrderMapper
    {
        public static _proxy.tbl_PurchaseOrderHeader MapToModelObject(PurchaseOrderHeaderViewModel objectToBeMapped)
        {
            var temp = new _proxy.tbl_PurchaseOrderHeader
            {
                WareHouseID = objectToBeMapped.WareHouseItem.WarehouseID,
                Vendor = objectToBeMapped.VendorCode,
                RecieveDate = objectToBeMapped.RecieveDate,
                CreationDate = objectToBeMapped.CreationDate,
                DelivaryDate = objectToBeMapped.DelivaryDate
            };

            if (objectToBeMapped.TransID != null) temp.TransID = (int)objectToBeMapped.TransID;
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