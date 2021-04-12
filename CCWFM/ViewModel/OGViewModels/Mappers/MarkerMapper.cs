using System;
using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels.Mappers
{
    public class MarkerMapper
    {
        public static MarkerHeaderListViewModel MapToViewModel(tbl_MarkerTransactionHeader markerObject,
            ObservableCollection<RouteCardService.TblRouteGroup> @group)
        {
            if (markerObject.Status == null)
            {
                markerObject.Status = 0;
            }
            var newRouteGroupitem = new RouteCardService.TblRouteGroup();
            if (markerObject.TblRouteGroup!= null)
            {
                newRouteGroupitem.InjectFrom(markerObject.TblRouteGroup);
            }
            var newRouteitem = new RouteCardService.TblRoute();

            if (markerObject.TblRoute!=null)
            {
                newRouteitem.InjectFrom(markerObject.TblRoute);
            }
            
            return new MarkerHeaderListViewModel
            {
                Iserial = markerObject.Iserial,
                RoutGroupID = markerObject.Operation,
                RoutID = markerObject.Workstation,
                RoutGroupItem = newRouteGroupitem,
                RoutItem = newRouteitem,
                VendorCode = markerObject.Vendor,
                TransDate = markerObject.TransDate,
                WareHouses = markerObject.WareHouses,
                RouteGroups = @group,
                TblMarkerType = markerObject.TblMarkerType??0,
                Status = (int)markerObject.Status,
                Posted = markerObject.Posted,
            };
        }

        public static tbl_MarkerTransactionHeader MapToViewModel(MarkerHeaderListViewModel markerObject)
        {
            var markerDetails = new ObservableCollection<tbl_MarkerDetail>();
            foreach (var item in markerObject.MarkerListViewModelList.Where(x => x.MarkerNo != null))
            {
                markerDetails.Add(MapToMarkerDetail(item));
            }
            return new tbl_MarkerTransactionHeader
            {
                Iserial = markerObject.Iserial,
                Operation = markerObject.RoutGroupID,
                Workstation = markerObject.RoutID,
                Vendor = markerObject.VendorCode,
                TransDate = Convert.ToDateTime(markerObject.TransDate),
                WareHouses = markerObject.WareHouses,
                tbl_MarkerDetail = markerDetails,
                TblMarkerType = markerObject.TblMarkerType,
                Status = markerObject.Status,
                Posted = markerObject.Posted,
            };
        }

        public static MarkerListViewModel MapToMarkerDetail(tbl_MarkerDetail markerObject)
        {
            var markerMeterPersizes = new ObservableCollection<MeterPerSize>();
            foreach (var meterPerSize in markerObject.tbl_MarkerDetailMeterPerSize)
            {
                markerMeterPersizes.Add(MapToMeterPerSize(meterPerSize));
            }
            var styleColorlist = new ObservableCollection<MarkerSalesOrderDetail>
            {
                new MarkerSalesOrderDetail
                {
                    StyleColorCode = markerObject.TblColor.Code,
                    StyleColorIserial = markerObject.TblColor.Iserial
                }
            };

            var fabricColorList = new ObservableCollection<MarkerSalesOrderDetail>
            {
                new MarkerSalesOrderDetail
                {
                    FabricColorCode = markerObject.TblColor1.Code,
                    FabricColorIserial = markerObject.TblColor1.Iserial
                }
            };
            var listOfCutting = new ObservableCollection<CuttingOrderViewModel>();
            foreach (var variable in markerObject.Tbl_Wf_CuttingOrder)
            {
                var newrow = new CuttingOrderViewModel().InjectFrom(variable) as CuttingOrderViewModel;
                listOfCutting.Add(newrow);
            }
            double totalreq = 0;
            if (markerObject.TotalReq != null)
            {
                totalreq = (double)markerObject.TotalReq;
            }
            if (markerObject.Status == null)
            {
                markerObject.Status = 0;
            }
            var row = new MarkerListViewModel
            {
                Iserial = markerObject.Iserial,
                BatchNo = markerObject.BatchNo,
                StyleColorCode = markerObject.StyleColorCode,
                FabricCode = markerObject.FabricCode,
                FabricColorCode = markerObject.FabricColorCode,
                FabricColorList = fabricColorList,
                StyleDetailsList = styleColorlist,
                MarkerL = markerObject.MarkerL,
                MarkerNo = markerObject.MarkerNo,
                MarkerW = markerObject.MarkerW,
                SizeRange = markerObject.SizeRange,
                StyleNo = markerObject.StyleNo,
                SalesOrder = markerObject.SalesOrder,
                MarkerTransactionHeader = markerObject.MarkerTransactionHeader,
                CloretteCmPerPc = markerObject.CloretteCmPerPc,
                Status = (int)markerObject.Status,
                SavedCuttingOrderlist = listOfCutting,
                TotalReq = totalreq
            };
            GenericMapper.InjectFromObCollection(row.MeterPerSizeList, markerMeterPersizes);

            return row;
        }

        public static tbl_MarkerDetail MapToMarkerDetail(MarkerListViewModel markerObject)
        {
            var markerMeterPersizes = new ObservableCollection<tbl_MarkerDetailMeterPerSize>();
            foreach (var meterPerSize in markerObject.MeterPerSizeList)
            {
                markerMeterPersizes.Add(MapToMeterPerSize(meterPerSize));
            }

            return new tbl_MarkerDetail
            {
                Iserial = markerObject.Iserial,
                BatchNo = markerObject.BatchNo,
                StyleColorCode = markerObject.StyleColorCode,
                FabricCode = markerObject.FabricCode,
                FabricColorCode = markerObject.FabricColorCode,
                MarkerL = markerObject.MarkerL,
                MarkerNo = markerObject.MarkerNo,
                MarkerW = markerObject.MarkerW,
                SizeRange = markerObject.SizeRange,
                StyleNo = markerObject.StyleNo,
                SalesOrder = markerObject.SalesOrder,
                MarkerTransactionHeader = markerObject.MarkerTransactionHeader,
                CloretteCmPerPc = markerObject.CloretteCmPerPc,

                tbl_MarkerDetailMeterPerSize = markerMeterPersizes,

                TotalReq = markerObject.TotalReq,
                Status = markerObject.Status
            };
        }

        public static MeterPerSize MapToMeterPerSize(TblSize sizeCode)
        {
            return new MeterPerSize
            {
                Iserial = 0,
                MeterPerSizeCode = sizeCode.SizeCode,
                MeterPerSizeValue = 0,
                SizecodeId = Convert.ToInt32(sizeCode.Id)
            };
        }

        public static MeterPerSize MapToMeterPerSize(tbl_MarkerDetailMeterPerSize meterPerSize)
        {
            return new MeterPerSize
            {
                Iserial = meterPerSize.Iserial,
                MeterPerSizeCode = meterPerSize.MeterPerSizeCode,
                MeterPerSizeValue = meterPerSize.MeterPerSizeValue,
                SizecodeId = meterPerSize.SizeCode_Id,
            };
        }

        public static tbl_MarkerDetailMeterPerSize MapToMeterPerSize(MeterPerSize meterPerSize)
        {
            return new tbl_MarkerDetailMeterPerSize
            {
                Iserial = meterPerSize.Iserial,
                MeterPerSizeCode = meterPerSize.MeterPerSizeCode,
                MeterPerSizeValue = meterPerSize.MeterPerSizeValue,
                SizeCode_Id = meterPerSize.SizecodeId
            };
        }

        public static CuttingOrderViewModel MapToCuttingOrder(Inspection inspectionObject, int markerIserial)
        {
            return new CuttingOrderViewModel
            {
                MarkerHeaderIserial = inspectionObject.MarkerTransactionHeader,
                Barcode = inspectionObject.RollBatch,
                InspectionIserial = inspectionObject.Iserial,
                BatchNoCuttting = inspectionObject.BatchNo,
                ColorCodeCuttingOrder = inspectionObject.ColorCode,
                Fabric_Code = inspectionObject.Fabric_Code,
                NetRollWMT = inspectionObject.NetRollWMT,
                ProductCategory = inspectionObject.ProductCategory,
                RollNo = inspectionObject.RollNo,
                RollWMt = inspectionObject.RollWMT,
                StoreRollQty = (float)inspectionObject.RemainingMarkerRollQty,
                consPerpc = inspectionObject.ConsPerPC,
                m2WeightGm = inspectionObject.M2WeightGm,
                noOfpcs = inspectionObject.NoofPCs,
                unit = inspectionObject.Unit,
                RollUnit = inspectionObject.Unit,
                MarkerIserial = markerIserial
            };
        }

        public static CuttingOrderViewModel MapToCuttingOrder(InspectionsRoute inspectionObject, int markerIserial)
        {
            return new CuttingOrderViewModel
            {
                MarkerHeaderIserial = inspectionObject.MarkerTransactionHeader,
                Barcode = inspectionObject.RollBatch,
                InspectionIserial = inspectionObject.Iserial,
                BatchNoCuttting = inspectionObject.BatchNo,
                ColorCodeCuttingOrder = inspectionObject.ColorCode,
                Fabric_Code = inspectionObject.Fabric_Code,
                NetRollWMT = inspectionObject.NetRollWMT,
                ProductCategory = inspectionObject.ProductCategory,
                RollNo = inspectionObject.RollNo,
                RollWMt = inspectionObject.RollWMT,
                StoreRollQty = inspectionObject.RemainingMarkerRollQty??0,
                consPerpc = inspectionObject.ConsPerPC,
                m2WeightGm = inspectionObject.M2WeightGm,
                noOfpcs = inspectionObject.NoofPCs,
                unit = inspectionObject.Unit,
                RollUnit = inspectionObject.Unit,
                MarkerIserial = markerIserial,
                Warehouse = inspectionObject.FinishedWarehouse,
                Site = inspectionObject.FinishedSite,
                
            };
        }

        public static Tbl_Wf_CuttingOrder MapToCuttingOrder(CuttingOrderViewModel order)
        {
            return new Tbl_Wf_CuttingOrder
            {
                MarkerHeaderTransaction = order.MarkerHeaderIserial,
                InspectionIserial = order.InspectionIserial,

                RollUnit = order.unit,
                MarkerIserial = order.MarkerIserial,
                RollAssignedQty = order.RollAssignedQty,
                CuttingSelection = order.CuttingSelection,
            };
        }

        public static CuttingOrderViewModel MapToCuttingOrder(Tbl_Wf_CuttingOrder order)
        {
            return new CuttingOrderViewModel
            {
                MarkerHeaderIserial = order.MarkerHeaderTransaction,
                InspectionIserial = order.InspectionIserial,
                unit = order.RollUnit,
                RollUnit = order.RollUnit,
                MarkerIserial = order.MarkerIserial,
                RollAssignedQty = order.RollAssignedQty,
                CuttingSelection = order.CuttingSelection,
            };
        }
    }
}