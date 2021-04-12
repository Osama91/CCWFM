using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels.Mappers
{
    public static class DyeingPlanMapper
    {
        public static TblDyeingPlanViewModel VmMapToDyeingPLanEstimated(EstimatedDyedMain_Result dyeingObject, List<DyeingColorQuantitiesRequired_Result> dyeingDetailsObject, int batchNo, ObservableCollection<ColorHeader> colorHeader)
        {
            var dyeingPLanDetail = new ObservableCollection<TblDyeingPlanDetailViewModel>();
            foreach (var variable in colorHeader.Distinct())
            {
                var list = dyeingDetailsObject.Where(x => x.FabricColor == variable.ColorName);
                if (list != null && list.Any())
                {
                    dyeingPLanDetail.Insert(colorHeader.IndexOf(variable), new TblDyeingPlanDetailViewModel
                        {
                            FabricColorName = list.FirstOrDefault().FabricColor,
                            FabricColorValue = Math.Round(Convert.ToSingle(list.Sum(x => x.Total)), 3),
                            BatchNo = batchNo = batchNo + 1,
                            SalesOrder = dyeingObject.SalesOrderID,
                            Size = list.FirstOrDefault().Size,
                            OldColor = list.FirstOrDefault().OldColor
                        });
                }
                else
                {
                    dyeingPLanDetail.Insert(colorHeader.IndexOf(variable), new TblDyeingPlanDetailViewModel
                    {
                        FabricColorName = "",
                        FabricColorValue = 0,
                        BatchNo = batchNo = 0,
                        SalesOrder = "",
                        OldColor = ""
                    });
                }
            }
            return new TblDyeingPlanViewModel
            {
                SalesOrder = dyeingObject.SalesOrderID,
                DyeingClass = int.Parse(dyeingObject.DyeingClass),
                Unit = dyeingObject.Unit,
                FabricCode = dyeingObject.FabricCode,
                Style = dyeingObject.Style,
                DetailsViewModel = dyeingPLanDetail,
                CalculatedTotalQty = dyeingPLanDetail.Sum(x => x.FabricColorValue),
                DyedFabric = dyeingObject.DyedCode,
            };
        }

        public static DyeingSummeryViewModel VmMapToDyeingSummary(TblDyeingSummary dyeingObject)
        {
            var list = new ObservableCollection<TblDyeingPlanLotsMasterViewModel>();
            foreach (var variable in dyeingObject.TblDyeingPlanLotsMasters)
            {
                list.Add((TblDyeingPlanLotsMasterViewModel)new TblDyeingPlanLotsMasterViewModel().InjectFrom(variable));
            }

            return new DyeingSummeryViewModel
            {
                Iserial = dyeingObject.Iserial,
                Batchno = dyeingObject.BatchNo,
                ColorName = dyeingObject.Color,
                DyeingClass = dyeingObject.DyeingClass,
                DyedFabric = dyeingObject.DyedFabric,
                DyeingHeader = dyeingObject.DyeingHeader,
                Unit = dyeingObject.Unit,
                CalculatedTotalQty = Convert.ToDouble(dyeingObject.CalculatedTotalQty),
                Vendor = dyeingObject.Vendor,
                FabricCode = dyeingObject.FabricCode,
                
                LotsMasterList = list,
            };
        }

        public static TblDyeingPlanViewModel VmMapToDyeingPLan(TblDyeingPlan dyeingObject)
        {
            var dyeingPLanDetail = new ObservableCollection<TblDyeingPlanDetailViewModel>();
            foreach (var item in dyeingObject.TblDyeingPlanDetails)
            {
                dyeingPLanDetail.Add(new TblDyeingPlanDetailViewModel
                {
                    FabricColorName = item.FabricColorName,
                    FabricColorValue = Math.Round(Convert.ToSingle(item.FabricColorValue), 3),
                    BatchNo = item.BatchNo,
                    Iserial = item.Iserial,
                    SalesOrder = dyeingObject.SalesOrder,
                    BatchNoCreated = item.BatchNoCreated,
                });
            }

            return new TblDyeingPlanViewModel
            {
                Iserial = dyeingObject.Iserial,
                DyeingClass = dyeingObject.DyeingClass,
                Unit = dyeingObject.Unit,
                SalesOrder = dyeingObject.SalesOrder,
                FabricCode = dyeingObject.FabricCode,
                Style = dyeingObject.Style,
                DyeingHeader = dyeingObject.DyeingHeader,
                CalculatedTotalQty = Math.Round(Convert.ToSingle(dyeingObject.CalculatedTotalQty), 3),
                DetailsViewModel = dyeingPLanDetail,
                DyedFabric = dyeingObject.DyedFabric,
            };
        }

        public static TblDyeingPlan DbMapToDyeingPLan(TblDyeingPlanViewModel dyeingObject)
        {
            var dyeingPLanDetail = new ObservableCollection<TblDyeingPlanDetail>();
            foreach (var item in dyeingObject.DetailsViewModel)
            {
                dyeingPLanDetail.Add(new TblDyeingPlanDetail
                {
                    FabricColorName = item.FabricColorName,
                    FabricColorValue = Math.Round(Convert.ToSingle(item.FabricColorValue), 3),
                    BatchNo = item.BatchNo,
                    Iserial = item.Iserial,
                    BatchNoCreated = item.BatchNoCreated,
                });
            }

            return new TblDyeingPlan
            {
                Iserial = dyeingObject.Iserial,
                DyeingClass = dyeingObject.DyeingClass,
                Unit = dyeingObject.Unit,
                FabricCode = dyeingObject.FabricCode,
                Style = dyeingObject.Style,
                DyeingHeader = dyeingObject.DyeingHeader,
                CalculatedTotalQty = Math.Round(dyeingObject.CalculatedTotalQty, 3),
                TblDyeingPlanDetails = dyeingPLanDetail,
                DyedFabric = dyeingObject.DyedFabric,
                SalesOrder = dyeingObject.SalesOrder,
            };
        }

        public static TblDyeingSummary DbMapToDyeingSummary(DyeingSummeryViewModel dyeingObject)
        {
            var lotsMasterlist = new ObservableCollection<TblDyeingPlanLotsMaster>();
            var lotDetailsList = new ObservableCollection<TblDyeingPlanLotsDetail>();

            foreach (var lotsMasterRow in dyeingObject.LotsMasterList.Where(x => x.FabricLot != null))
            {
                foreach (var lotsDetailsRow in lotsMasterRow.LotsDetailsList)
                {
                    lotDetailsList.Add(new TblDyeingPlanLotsDetail
                    {
                        DeliveryDate = lotsDetailsRow.DeliveryDate,
                        FabricLotMasterIserial = lotsMasterRow.Iserial,
                        Iserial = lotsDetailsRow.Iserial,
                        RequiredQuantity = lotsDetailsRow.RequiredQuantity,
                        SalesOrder = lotsDetailsRow.SalesOrder,
                        Saved = lotsDetailsRow.Saved,
                    });
                }

                lotsMasterlist.Add(new TblDyeingPlanLotsMaster
                {
                    AvaliableQuantity = lotsMasterRow.AvaliableQuantity,
                    Config = lotsMasterRow.Config,
                    FabricCode = lotsMasterRow.FabricCode,
                    FabricLot = lotsMasterRow.FabricLot,
                    FromColor = lotsMasterRow.FromColor,
                    Iserial = lotsMasterRow.Iserial,
                    DyeingsSummaryPlanIserial = dyeingObject.Iserial,
                    RequiredQuantity = lotsMasterRow.RequiredQuantity,
                    Unit = lotsMasterRow.Unit,
                    TblDyeingPlanLotsDetails = lotDetailsList,
                    
                });
            }
            return new TblDyeingSummary
            {
                Iserial = dyeingObject.Iserial,
                BatchNo = dyeingObject.Batchno,
                Color = dyeingObject.ColorName,
                DyeingClass = dyeingObject.DyeingClass,
                DyedFabric = dyeingObject.DyedFabric,
                DyeingHeader = dyeingObject.DyeingHeader,
                Unit = dyeingObject.Unit,
                CalculatedTotalQty = dyeingObject.CalculatedTotalQty,
                Vendor = dyeingObject.Vendor,
                FabricCode = dyeingObject.FabricCode,
                TblDyeingPlanLotsMasters = lotsMasterlist
            };
        }
    }
}