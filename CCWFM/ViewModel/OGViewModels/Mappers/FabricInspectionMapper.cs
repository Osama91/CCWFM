using System;
using System.Linq;
using CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.OGViewModels.Mappers
{
    public static class FabricInspectionMapper
    {
        public static OrderLineListViewModel MapToOrderLine(CRUD_ManagerServicePurchaseOrderDetailDto row)
        {
            return new OrderLineListViewModel
            {
                Color_Code = row.FabricColor,
                Fabric_Code = row.ItemId,
                Fabric_Ename = row.ItemName,
                PURCHUNIT = row.Unit,
                TotalQty = row.Quantity,
                //VENDACCOUNT = row.VENDACCOUNT,
                NoOfRolls = 1,
                BatchNo = row.BatchNo,
                LineNum = row.LineNumber,
                UnitPrice = row.UnitPrice,
                Warehouse = row.Warehouse,
                Location = row.Location,
                Site = row.Site,
            };
        }

        public static OrderLineListViewModel MapToOrderLine(InventoryReservedJournalsDetail row)
        {
            return new OrderLineListViewModel
            {
                // Color_Code = Row.Color_Code,
                Fabric_Code = row.ITEMID,
                // Fabric_Ename = Row.Fabric_Ename,
                PURCHUNIT = row.Unit,
                //    TotalQty = row.QTY,
                //  VENDACCOUNT = Row.VENDACCOUNT,
                NoOfRolls = 1,
                // BatchNo = Row.INVENTBATCHID,
                LineNum = (row.LINENUM),
            };
        }

        public static TblFabricInspectionHeaderViewModel VwMapToFabricInspectionHeader(Tbl_fabricInspectionHeader row)
        {
            string batchno = "";
            var fabricColor = "";
            var n = row.Tbl_fabricInspectionDetail.FirstOrDefault();
            if (n != null)
            {
                 batchno = n.BatchNo;
                fabricColor = n.ColorCode;
            }
            return new TblFabricInspectionHeaderViewModel
            {
                BatchNo = batchno,
                ColorCode= fabricColor,
                Brand = row.Brand,
                Iserial = row.Iserial,
                Order = row.TransOrder,
                PostedToAx = row.PostedToAx,
                Season = row.Season,
                TransactionType = row.TransactionType,
                TransDate = row.TransDate,
                VendorProperty = row.Vendor,
                Notes = row.Notes,
                JournalPerRow = new CRUD_ManagerServicePurchaseOrderDto
                {
                    JournalId = row.TransOrder,
                    VendorCode = row.Vendor,
                },
                FabricCode = row.FabricCode,

            };
        }

        public static Tbl_fabricInspectionHeader DbMapToFabricInspectionHeader(TblFabricInspectionHeaderViewModel row)
        {
            return new Tbl_fabricInspectionHeader
            {
                Brand = row.Brand,
                Iserial = row.Iserial,
                TransOrder = row.Order,
                PostedToAx = row.PostedToAx,
                Season = row.Season,
                TransactionType = row.TransactionType,
                TransDate = Convert.ToDateTime(row.TransDate),
                Vendor = row.VendorProperty,
                VendorSubtraction = row.VendorSubtraction,
                Notes=row.Notes
            };
        }
    }
}