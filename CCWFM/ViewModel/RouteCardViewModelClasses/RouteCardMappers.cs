using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.RouteCardViewModelClasses
{
    public static class RouteCardMappers
    {
        public static RouteCardFabricViewModel MapToViewModel(RouteCardService.RouteCardFabric item) // GetRouteCardFabric
        {
            //var style = new TblStyle();
            //if (item.TblSalesOrder1.TblStyle1!=null)
            //{
            //    style.InjectFrom(item.TblSalesOrder1.TblStyle1);
            //}
       
            var row = new RouteCardFabricViewModel
            {

                TransID = item.RouteCardHeader.TransID,
                DocDate = item.RouteCardHeader.DocDate,
                RemainingQty = item.RemainingQty,
                OldIserial = item.OldIserial,
                ItemId = item.ItemId,
                Warehouse = item.Warehouse,
// ReSharper disable once PossibleInvalidOperationException
                FabricColor = (int)item.FabricColor,
                Qty = item.Qty,
                TblSalesOrder = item.TblSalesOrder,
                Size = item.Size,
                Unit = item.Unit,
                StyleColor = item.StyleColor,
                Site = item.Site,
                Location = item.Location,
                Batch = item.Batch,
                Barcode = item.Barcode,
                Iserial = item.Iserial,
                StyleColorPerRow = new TblColor { Iserial=item.TblColor1.Iserial,Code=item.TblColor1.Code,Ename=item.TblColor1.Ename },
                FabricColorPerRow = new TblColor { Iserial = item.TblColor.Iserial, Code = item.TblColor.Code, Ename = item.TblColor.Ename },
                SalesOrderPerRow =new TblSalesOrder {Iserial=item.TblSalesOrder, SalesOrderCode=item.TblSalesOrder1.SalesOrderCode,SalesOrderType= item.TblSalesOrder1.SalesOrderType,TblStyle=item.TblSalesOrder1.TblStyle,
                //TblStyle1= style
                },
                RouteCardHeaderIserial = item.RouteCardHeaderIserial,
                IsFree = item.IsFree,
                Notes = item.Notes,
                NewFabricColor = item.NewFabricColor,
                CostPerUnit = item.CostPerUnit,
                ItemGroup = item.ItemGroup,
                TblItemDim = item.TblItemDim
            };
            if (item.TblColor2!=null)
            {
                row.NewColorPerRow = new TblColor { Iserial = item.TblColor2.Iserial, Code = item.TblColor2.Code, Ename = item.TblColor2.Ename };
            }
           
          
            return row;
        }

        public static RouteCardFabricViewModel MapToViewModel(RouteCardService.RouteBomIssueSP_Result item, string warehouse, ObservableCollection<V_Warehouse> warehouseList)
        {
            return new RouteCardFabricViewModel
            {
                ItemId = item.ItemId,
                Warehouse = warehouse,
                FabricColor = item.FabricColorIserial,
                MaterialUsage = item.MaterialUsage,
                TblSalesOrder = item.SalesOrderIserial,
                Size = item.FabricSize,
                Unit = item.UnitID,
                StyleColor = item.StyleColorCode,
                Location = warehouse,
                Site = warehouseList.FirstOrDefault(w => w.WarehouseID == warehouse).SiteId,
                CostPerUnit = item.CostPerUnit,
                FabricColorPerRow = new CRUDManagerService.TblColor
                {
                    Iserial = item.FabricColorIserial,
                    Code = item.FabricColor,
                    Ename = item.FabricColor,
                    Aname = item.FabricColor,
                },
                StyleColorPerRow = new CRUDManagerService.TblColor
                {
                    Iserial = item.StyleColorCode,
                    Code = item.StyleColor,
                    Ename = item.StyleColor,
                    Aname = item.StyleColor,
                },
                NewColorPerRow = new CRUDManagerService.TblColor
                {
                    Iserial = item.DyedColor ?? 0,
                    Code = item.DyedColorCode,
                    Ename = item.DyedColorCode,
                    Aname = item.DyedColorCode,
                },
                SalesOrderPerRow = new CRUDManagerService.TblSalesOrder
                {
                    Iserial = item.SalesOrderIserial,
                    SalesOrderCode = item.SalesOrderID
                },
                Qty = item.Total,
                Notes = item.Notes,
                NewFabricColor = item.DyedColor,
                ItemGroup = item.BOM_FabricType,
            };
        }

        public static RouteCardService.RouteCardFabric MapToViewModel(RouteCardFabricViewModel item) // SavingRouteCardFabric
        {
            return new RouteCardService.RouteCardFabric
            {
                RemainingQty = item.RemainingQty,
                OldIserial = item.OldIserial,
                ItemId = item.ItemId,
                Warehouse = item.Warehouse,
                FabricColor = item.FabricColor,
                Qty = item.Qty,
                Size = item.Size,
                Unit = item.Unit,
                StyleColor = item.StyleColor,
                Site = item.Site,
                Location = item.Location,
                Barcode = item.Barcode,
                Batch = item.Batch,
                Iserial = item.Iserial,
                TblSalesOrder = item.TblSalesOrder,
                RouteCardHeaderIserial = item.RouteCardHeaderIserial,
                IsFree = item.IsFree,
                CostPerUnit = item.CostPerUnit,
                Notes = item.Notes,
                NewFabricColor = item.NewFabricColor,
                ItemGroup=item.ItemGroup,
                TblItemDim=item.TblItemDim
            };
        }

        public static void MapToViewModel(InspectionsFullDimention item, RouteCardFabricViewModel row)
        {
            row.ItemId = item.Fabric_Code;
            row.Warehouse = item.FinishedWarehouse;
            //  row.FabricColor = item.TOCONFIG;
            row.Qty = item.RemainingMarkerRollQty;
            //  // row.SalesOrder = item.SalesOrder;
            //Size = item.Size,
            //row.Unit = item.;
            ////   row.StyleColor = item.StyleColorCode;
            row.Site = item.FinishedSite;
            row.Location = item.FinishedWarehouse;
            row.Barcode = item.RollBatch;
            row.Batch = item.BatchNo;
            row.ItemGroup = item.ItemGroup;
                //row.TblItemDim = item.TblItemDim
        }

        public static void MapToViewModel(RouteCardService.RouteCardHeader item, RouteCardHeaderViewModel row)
        {
            var routeCardFabrics = new ObservableCollection<RouteCardFabricViewModel>();
            foreach (var det in item.RouteCardFabrics)
            {
                routeCardFabrics.Add(MapToViewModel(det));
            }
            row.SupplierInv = item.SupplierInv;
            row.RouteCardFabricViewModelList = routeCardFabrics;
            row.TransactionType = item.tblTransactionType;
            row.RouteTypeInt = item.RouteType;
            row.MarkerIserial = item.MarkerTransaction;
            row.IsPosted = item.IsPosted;
            row.Iserial = item.Iserial;
            row.RoutGroupID = item.RoutGroupID;
            if (item.RoutID != null) row.RoutID = (int)item.RoutID;
            row.RoutGroupItem = item.TblRouteGroup;
            row.RoutItem = item.TblRoute;
            row.VendorCode = item.Vendor;
            row.VendorPerRow = new Vendor { vendor_code = item.Vendor };
            row.Direction = item.Direction;
            row.TransID = item.TransID;
            row.DelivaryDate = item.DeliveryDate;
            row.DocDate = item.DocDate;
            row.RouteDirectionPerRow = item.TblRouteDirection;
            row.LastTransaction = item.Iserial;
            row.LinkIserial = item.LinkIserial;
            row.Notes = item.Notes;
            row.ProductionResidue = item.ProductionResidue;
        }

        public static void MapToViewModel(RouteCardHeaderViewModel item, RouteCardService.RouteCardHeader row)
        {
            var routeCardFabrics = new ObservableCollection<RouteCardService.RouteCardFabric>();
            foreach (var det in item.RouteCardFabricViewModelList)
            {
                routeCardFabrics.Add(MapToViewModel(det));
            }
            row.SupplierInv = item.SupplierInv;
            row.RouteCardFabrics = routeCardFabrics;
            row.tblTransactionType = item.TransactionType;
            row.RouteType = item.RouteTypeInt;
            row.MarkerTransaction = item.MarkerIserial;
            row.IsPosted = item.IsPosted;
            row.Iserial = item.Iserial;
            row.RoutGroupID = item.RoutGroupID;
            row.RoutID = item.RoutID;
            row.Vendor = item.VendorCode;
            row.Direction = item.Direction;
            if (item.TransID != null) row.TransID = (int)item.TransID;
            row.DeliveryDate = item.DelivaryDate;
            row.DocDate = item.DocDate;
            item.RouteDirectionPerRow = row.TblRouteDirection;
            row.LinkIserial = item.LinkIserial;
            row.Notes = item.Notes;
            row.ProductionResidue = item.ProductionResidue;
        }
    }
}