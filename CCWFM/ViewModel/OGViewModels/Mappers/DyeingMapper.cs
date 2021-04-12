using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.OGViewModels.Mappers
{
    public static class DyeingMapper
    {
        public static TblDyeingOrderHeaderViewModel VwMapToDyeingorderHeader(TblDyeingOrdersHeader row)
        {
            return new TblDyeingOrderHeaderViewModel
            {
                DocPlan = row.DocPlan,
                DyeingProductionOrder = row.DyeingProductionOrder,
                TransactionDate = row.TransactionDate,
                Vendor = row.Vendor,
                VendorPerRow = new Vendor { vendor_code = row.Vendor }
            };
        }

        public static TblDyeingOrdersMainDetailViewModel VwMapToDyeingorderMainDetail(TblDyeingOrdersMainDetail row, Dictionary<string, double?> colorPrices)
        {
            var orderDetails = new ObservableCollection<TblDyeingOrderDetailsViewModel>();
            foreach (var item in row.TblDyeingOrdersDetails)
            {

               
              var price=  colorPrices.FirstOrDefault(w => w.Key == item.Color).Value;
                orderDetails.Add(new TblDyeingOrderDetailsViewModel
                {
                    DefaultPrice = price,
                    BatchNo = item.BatchNo,
                    CalculatedTotalQty = Math.Round(item.CalculatedTotalQty, 3),
                    Color = item.Color,
                    FabricCode = item.FabricCode,
                    Iserial = item.Iserial,
                    DyedFabric = item.DyedFabric,
                    DyeingClass = item.DyeingClass,
                    
                    Unit = item.Unit,
                    TransactionType = item.TransactionType,
                    DyeingProductionOrder = item.DyeingProductionOrder,
                    TransId = item.TransId,
                    EstimatedDeliveryDate = item.EstimatedDeliveryDate,
                });
            }

            return new TblDyeingOrdersMainDetailViewModel
            {
                Iserial=row.Iserial,
                TransactionType = row.TransactionType,
                DyeingProductionOrder = row.DyeingProductionOrder,
                TransactionDate = row.TransactionDate,
                TransId = row.TransId,
                WareHouse = row.WareHouse,
                Saved = true,
                GotTransid = true,
                VendorTransaction = row.VendorTransaction,
                TblDyeingOrderDetails = orderDetails,
                Posted = row.Posted
            };
        }

        public static TblDyeingOrdersMainDetail DbMapToDyeingorderMainDetail(TblDyeingOrdersMainDetailViewModel row)
        {
            var orderDetails = new ObservableCollection<TblDyeingOrdersDetail>();
            foreach (var item in row.TblDyeingOrderDetails)
            {
                orderDetails.Add(new TblDyeingOrdersDetail
                {
                    BatchNo = item.BatchNo,
                    
                    CalculatedTotalQty = Math.Round(item.CalculatedTotalQty, 3),
                    Color = item.Color,
                    FabricCode = item.FabricCode,
                    Iserial = item.Iserial,
                    DyedFabric = item.DyedFabric,
                    DyeingClass = item.DyeingClass,
                    Unit = item.Unit,
                    TransactionType = item.TransactionType,
                    DyeingProductionOrder = item.DyeingProductionOrder,
                    TransId = item.TransId,

                    EstimatedDeliveryDate = item.EstimatedDeliveryDate
                });
            }

            return new TblDyeingOrdersMainDetail
            {
                Iserial = row.Iserial,
                TransactionType = row.TransactionType,
                DyeingProductionOrder = row.DyeingProductionOrder,
                WareHouse = row.WareHouse,
                TransactionDate = row.TransactionDate,
                TransId = row.TransId,
                VendorTransaction = row.VendorTransaction,
                TblDyeingOrdersDetails = orderDetails,
                Posted = row.Posted
            };
        }
    }
}