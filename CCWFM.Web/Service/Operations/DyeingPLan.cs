using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<EstimatedDyedMain_Result> GetEstimatedDyeingList(string brand, int brandSection, string season, DateTime? fromDate, DateTime? toDate, out List<DyeingColorQuantitiesRequired_Result> ColorsValues, out int BatchNo)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var getDyeingList = (from s in entities.EstimatedDyedMain(brand, brandSection, season, fromDate, toDate)
                                     orderby s.SalesOrderID
                                     select s).ToList();

                ColorsValues = (from s in entities.DyeingColorQuantitiesRequired(brand, brandSection, season, fromDate, toDate)
                                select s).ToList();
                try
                {
                    BatchNo = (from b in entities.TblDyeingPlanDetails
                               select b.BatchNoCreated).Max();
                }
                catch (Exception)
                {
                    BatchNo = 0;
                }
                return getDyeingList;
            }
        }

        [OperationContract]
        public List<BomFabricBom> GetBomfabricDyeingService(List<string> salesOrder, string fabric, string color, out List<TblService> ServiceList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var bomList =
                    entities.BomFabricBoms.Include("BOM1.TblSalesOrder1").Include("BOM1.BomFabricBoms")
                        .Where(x => salesOrder.Contains(x.BOM1.TblSalesOrder1.SalesOrderCode) && x.BOM1.TblSalesOrder1.SalesOrderType == 2);

                var itemIserial = bomList.Select(x => x.Item);
                ServiceList = entities.TblServices.Where(x => itemIserial.Contains(x.Iserial)).ToList();

                return bomList.ToList();
            }
        }

        [OperationContract]
        public List<EstimatedDyedMain_Result> GetEstimatedDyeingListFromProduction(int iserial, out List<DyeingColorQuantitiesRequired_Result> ColorsValues, out int BatchNo, out List<TblPurchaseOrderDetail> purchaseDetailList, out List<TblService> Services)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                Services = new List<TblService>();
                var production = entities.TblPurchaseOrderHeaders.Include("TblPurchaseOrderDetails.TblPurchaseOrderDetailBreakDowns.BOM1.BomFabricBoms").Include("TblPurchaseOrderDetails.TblPurchaseOrderDetailBreakDowns.BOM1.TblSalesOrder1.TblStyle1").Include("TblPurchaseOrderDetails.TblColor").Where(x => x.TblGeneratePurchaseHeader == iserial && x.TblPurchaseHeaderType == 1);
                var dyeinglist = new List<EstimatedDyedMain_Result>();
                purchaseDetailList = new List<TblPurchaseOrderDetail>();
                ColorsValues = new List<DyeingColorQuantitiesRequired_Result>();
                foreach (var tblPurchaseOrderHeader in production)
                {
                    foreach (var purchaseDetail in tblPurchaseOrderHeader.TblPurchaseOrderDetails)
                    {
                        purchaseDetailList.Add(purchaseDetail);                       
                        foreach (var purchaseDetailBreakDown in purchaseDetail.TblPurchaseOrderDetailBreakDowns)
                        {
                            var firstrow = 0;
                            try
                            {
                                firstrow = purchaseDetailBreakDown.BOM1.BomFabricBoms.FirstOrDefault(x => x.ItemType != "Service").Item;                      
                            }
                            catch (Exception)
                            {
                                throw new Exception("Sales Order:" + purchaseDetailBreakDown.BOM1.TblSalesOrder1.SalesOrderCode + " fabric :" + purchaseDetail.ItemId + " Needs To Be Revised ");
                            }

                            var rowFabric = entities.tbl_FabricAttriputes.FirstOrDefault(w => w.Iserial == firstrow);
                            var listofiserials = purchaseDetailBreakDown.BOM1.BomFabricBoms.Where(x => x.ItemType == "Service")
                                .Select(x => x.Item);
                            Services.AddRange(entities.TblServices.Where(x => listofiserials.Contains(x.Iserial)).ToList());
                            var fabric = entities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(x => x.FabricID == purchaseDetail.ItemId);
                            var dyeingclass = entities.tbl_lkp_DyingClassification.FirstOrDefault(
                                x => x.Iserial == fabric.DyingClassificationID);
                            if (dyeingclass != null)
                            {
                                if (rowFabric != null)
                                {
                                    if (fabric != null)
                                    {
                                        var newdyeingRow = new EstimatedDyedMain_Result
                                        {
                                            DyedGroupAname = dyeingclass.Aname,
                                            DyedGroupEname = dyeingclass.Ename,
                                            DyeingClass = dyeingclass.Iserial.ToString(),
                                            FabricCode = rowFabric.FabricID,
                                            FabricName = rowFabric.FabricDescription,
                                            Unit = fabric.tbl_lkp_UoM.Code,
                                            SalesOrderID = purchaseDetailBreakDown.BOM1.TblSalesOrder1.SalesOrderCode,
                                            Style = purchaseDetailBreakDown.BOM1.TblSalesOrder1.TblStyle1.StyleCode,
                                            DyedCode = fabric.FabricID,
                                            DyedName = fabric.FabricDescription,
                                        };
                                        if (!dyeinglist.Contains(newdyeingRow))
                                        {
                                            dyeinglist.Add(newdyeingRow);
                                        }
                                    }
                                }
                            }

                            if (dyeingclass != null)
                                if (fabric != null)
                                    ColorsValues.Add(new DyeingColorQuantitiesRequired_Result
                                    {
                                        BOM_FabricType = "Knitted",
                                        Brand_Ename = "tt",
                                        DyeingClass = dyeingclass.Iserial.ToString(),
                                        SalesOrderID = purchaseDetailBreakDown.BOM1.TblSalesOrder1.SalesOrderCode,
                                        styleheader = purchaseDetailBreakDown.BOM1.TblSalesOrder1.TblStyle1.StyleCode,
                                        Fabric_Code = fabric.FabricID,
                                        Fabric_Ename = fabric.FabricDescription,
                                        FabricColor = purchaseDetail.TblColor.Code,
                                        OldColor = "",
                                        Season_Name = "tt",
                                        Size = "",
                                        unitid = fabric.tbl_lkp_UoM.Code,
                                        Total = purchaseDetailBreakDown.Qty
                                    });
                        }
                    }
                }
                try
                {
                    BatchNo = (from b in entities.TblDyeingPlanDetails
                               select b.BatchNoCreated).Max();
                }
                catch (Exception)
                {
                    BatchNo = 0;
                }
                return dyeinglist;
            }
        }

        [OperationContract]
        public List<tbl_lkp_DyingClassification> GetDyeingClassificaiton()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.tbl_lkp_DyingClassification.ToList();
            }
        }

        [OperationContract]
        public List<TblDyeingPlan> GetDyeingList(int dyeingHeaderIserial, out int BatchNo)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var getDyeingList = (from s in entities.TblDyeingPlans
                                                         .Include("TblDyeingPlanDetails")
                                     where s.DyeingHeader == dyeingHeaderIserial
                                     select s).ToList();

                try
                {
                    BatchNo = (from b in entities.TblDyeingPlanDetails
                               select b.BatchNoCreated).Max();
                }
                catch (Exception)
                {
                    BatchNo = 0;
                }

                return getDyeingList;
            }
        }

        [OperationContract]
        public List<TblDyeingPlanHeader> GetDyeingHeaderList(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingPlanHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblDyeingPlanHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblDyeingPlanHeaders.Include("TblDyeingType1").Include("TblLkpSeason1").Include("TblLkpBrandSection1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblDyeingPlanHeaders.Count();
                    query = context.TblDyeingPlanHeaders.Include("TblDyeingType1").Include("TblLkpSeason1").Include("TblLkpBrandSection1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblDyeingSummary> GetDyeingSummary(int headerIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.TblDyeingSummaries.Include("TblDyeingPlanLotsMasters").Where(x => x.DyeingHeader == headerIserial).ToList();
            }
        }

        [OperationContract]
        public TblDyeingPlanHeader SaveDyeingHeaderList(TblDyeingPlanHeader header, List<TblDyeingPlan> mainDetails, List<TblDyeingSummary> summaryList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                #region PlanWithoutSummary

                if (header.Iserial != 0)
                {
                    var h = entities.TblDyeingPlanHeaders.SingleOrDefault(x => x.Iserial == header.Iserial);

                    if (h != null)
                    {
                        h.DocNo = header.DocNo;
                        h.Location = header.Location;
                        h.Brand = header.Brand;
                        h.TblLkpSeason = header.TblLkpSeason;
                        h.TransactionDate = header.TransactionDate;
                        h.TblDyeingType = header.TblDyeingType;
                    }
                }
                else
                {
                    entities.AddToTblDyeingPlanHeaders(header);
                }

                foreach (var dyeingPlanRow in mainDetails)
                {                  
                    if (dyeingPlanRow.Iserial == 0)
                    {
                        dyeingPlanRow.DyeingHeader = header.Iserial;
                        entities.AddToTblDyeingPlans(dyeingPlanRow);
                    }
                    else
                    {
                        var mainDetailRow = (from d in entities.TblDyeingPlans
                                             where d.DyeingHeader == header.Iserial &&
                                             d.Iserial == dyeingPlanRow.Iserial
                                             select d).SingleOrDefault();

                        if (mainDetailRow != null)
                        {
                            // MainDetailRow.TransactionDate = item.TransactionDate;
                        }
                        else
                        {
                            entities.AddToTblDyeingPlans(dyeingPlanRow);
                        }
                    }
                    entities.SaveChanges();

                    if (dyeingPlanRow.DyeingHeader != 0)
                    {
                        foreach (var dyeingDetailsPlanRow in dyeingPlanRow.TblDyeingPlanDetails.ToList())
                        {
                            var dyeingOrderDetails = (from d in entities.TblDyeingPlanDetails
                                                      where d.Iserial == dyeingDetailsPlanRow.Iserial
                                                      && d.FabricColorName == dyeingDetailsPlanRow.FabricColorName
                                                      select d).SingleOrDefault();
                            if (dyeingOrderDetails != null)
                            {
                                dyeingOrderDetails.BatchNo = dyeingDetailsPlanRow.BatchNo;
                                dyeingOrderDetails.FabricColorValue = dyeingDetailsPlanRow.FabricColorValue;
                                dyeingOrderDetails.BatchNoCreated = dyeingDetailsPlanRow.BatchNoCreated;
                            }
                            else
                            {
                                dyeingDetailsPlanRow.TblDyeingPlan = null;
                                entities.AddToTblDyeingPlanDetails(dyeingDetailsPlanRow);
                            }
                            entities.SaveChanges();
                        }
                    }
                }

                #endregion PlanWithoutSummary

                #region SummarySaving

                foreach (var summaryRow in summaryList)
                {
                    if (summaryRow.Iserial != 0)
                    {
                        var querySummaryrow = (from d in entities.TblDyeingSummaries
                                               where d.Iserial == summaryRow.Iserial
                                               select d).SingleOrDefault();

                        if (querySummaryrow != null)
                        {
                            querySummaryrow.BatchNo = summaryRow.BatchNo;
                            querySummaryrow.CalculatedTotalQty = summaryRow.CalculatedTotalQty;
                            querySummaryrow.Color = summaryRow.Color;
                            querySummaryrow.DyeingClass = summaryRow.DyeingClass;
                            querySummaryrow.DyedFabric = summaryRow.DyedFabric;
                            querySummaryrow.Unit = summaryRow.Unit;

                            querySummaryrow.FabricCode = summaryRow.FabricCode;
                            querySummaryrow.Vendor = summaryRow.Vendor;
                        }
                    }
                    else
                    {
                        summaryRow.TblDyeingPlanHeader = null;
                        summaryRow.DyeingHeader = header.Iserial;
                        entities.AddToTblDyeingSummaries(summaryRow);
                    }
                    entities.SaveChanges();
                    foreach (var planLotsMasterRow in summaryRow.TblDyeingPlanLotsMasters)
                    {
                        if (planLotsMasterRow.Iserial != 0)
                        {
                            var queryPlanLotsMasterRow = (from d in entities.TblDyeingPlanLotsMasters
                                                          where d.Iserial == planLotsMasterRow.Iserial
                                                          select d).SingleOrDefault();

                            if (queryPlanLotsMasterRow != null)
                            {
                                queryPlanLotsMasterRow.FabricCode = planLotsMasterRow.FabricCode;
                                queryPlanLotsMasterRow.FromColor = planLotsMasterRow.FromColor;
                                queryPlanLotsMasterRow.FabricLot = planLotsMasterRow.FabricLot;
                                queryPlanLotsMasterRow.AvaliableQuantity = planLotsMasterRow.AvaliableQuantity;
                                queryPlanLotsMasterRow.Config = planLotsMasterRow.Config;
                                queryPlanLotsMasterRow.RequiredQuantity = planLotsMasterRow.RequiredQuantity;
                                queryPlanLotsMasterRow.Unit = planLotsMasterRow.Unit;
                            }
                        }
                        else
                        {
                            planLotsMasterRow.TblDyeingSummary = null;
                            planLotsMasterRow.DyeingsSummaryPlanIserial = summaryRow.Iserial;
                            entities.AddToTblDyeingPlanLotsMasters(planLotsMasterRow);
                        }
                        entities.SaveChanges();

                        foreach (var planLotDetailsRow in planLotsMasterRow.TblDyeingPlanLotsDetails)
                        {
                            if (planLotDetailsRow.Iserial != 0)
                            {
                                var queryPlanLotsDetailsRow = (from d in entities.TblDyeingPlanLotsDetails
                                                               where d.Iserial == planLotDetailsRow.Iserial
                                                               select d).SingleOrDefault();

                                if (queryPlanLotsDetailsRow != null)
                                {
                                    queryPlanLotsDetailsRow.DeliveryDate = planLotDetailsRow.DeliveryDate;
                                    queryPlanLotsDetailsRow.RequiredQuantity = planLotDetailsRow.RequiredQuantity;
                                    queryPlanLotsDetailsRow.SalesOrder = planLotDetailsRow.SalesOrder;
                                    queryPlanLotsDetailsRow.Saved = planLotDetailsRow.Saved;
                                }
                            }
                            else
                            {
                                planLotDetailsRow.TblDyeingPlanLotsMaster = null;
                                planLotDetailsRow.FabricLotMasterIserial = planLotsMasterRow.Iserial;
                                entities.AddToTblDyeingPlanLotsDetails(planLotDetailsRow);
                            }
                        }
                    }
                }

                entities.SaveChanges();

                #endregion SummarySaving

                return header;
            }
        }

        [OperationContract]
        public List<FabricsDyedGroup> GetFabricDyedList()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.FabricsDyedGroups.ToList();
            }
        }

        [OperationContract]
        public List<DyeingAxService> GetAxSummaryServices()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.DyeingAxServices.ToList();
            }
        }

        [OperationContract]
        public List<DyeingSummaryService> GetSavedSummaryServices(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.DyeingSummaryServices.Where(x => x.SummaryRowIserial == iserial).ToList();
            }
        }

        [OperationContract]
        public void SaveSummaryServices(int iserial, string serviceCode, string serviceName, string notes, bool Checked, double qty)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (Checked)
                {
                    var dyeingSummaryServices = entities.DyeingSummaryServices.SingleOrDefault(x => x.SummaryRowIserial == iserial && x.ServiceCode == serviceCode);

                    var summaryServiceNewRow = new DyeingSummaryService();

                    if (dyeingSummaryServices == null)
                    {
                        summaryServiceNewRow.ServiceCode = serviceCode;
                        summaryServiceNewRow.ServiceName = serviceName;
                        summaryServiceNewRow.SummaryRowIserial = iserial;
                        summaryServiceNewRow.Notes = notes;
                        summaryServiceNewRow.Qty = qty;
                        entities.AddToDyeingSummaryServices(summaryServiceNewRow);
                    }
                    else
                    {
                        summaryServiceNewRow = dyeingSummaryServices;
                        summaryServiceNewRow.ServiceCode = serviceCode;
                        summaryServiceNewRow.ServiceName = serviceName;
                        summaryServiceNewRow.SummaryRowIserial = iserial;
                        summaryServiceNewRow.Notes = notes;
                        summaryServiceNewRow.Qty = qty;
                    }
                }
                else
                {
                    var dyeingSummaryServices = entities.DyeingSummaryServices.SingleOrDefault(x => x.SummaryRowIserial == iserial && x.ServiceCode == serviceCode);
                    if (dyeingSummaryServices != null)
                    {
                        entities.DeleteObject(dyeingSummaryServices);
                    }
                }
                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingRow(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var dyeingRow = (from s in entities.TblDyeingPlans
                                 where s.Iserial == iserial
                                 select s).SingleOrDefault();

                var dyeingRowDetails = (from s in entities.TblDyeingPlanDetails
                                        where s.Iserial == iserial
                                        select s);

                foreach (var item in dyeingRowDetails)
                {
                    entities.TblDyeingPlanDetails.DeleteObject(item);
                }
                entities.TblDyeingPlans.DeleteObject(dyeingRow);
                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingPlanHeader(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var dyeingRow = (from s in entities.TblDyeingPlanHeaders
                                 where s.Iserial == iserial
                                 select s).SingleOrDefault();

                if (dyeingRow != null) entities.TblDyeingPlanHeaders.DeleteObject(dyeingRow);
                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void GenerateDyeingOrders(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var planHeader = entities.TblDyeingPlanHeaders.SingleOrDefault(x => x.Iserial == iserial);
                if (planHeader != null)
                {
                    planHeader.PlanGenerated = true;

                    var dyeingSummaryList = entities.TblDyeingSummaries.Include("DyeingSummaryServices").Where(x => x.DyeingHeader == planHeader.Iserial).ToList();
                    var vendorList = dyeingSummaryList.Select(x => x.Vendor).Distinct().ToList();

                    foreach (var item in vendorList)
                    {
                        var dyeingHeaderRow = new TblDyeingOrdersHeader { DocPlan = planHeader.DocNo, Vendor = item, TransactionDate = planHeader.TransactionDate, };

                        entities.AddToTblDyeingOrdersHeaders(dyeingHeaderRow);
                        entities.SaveChanges();

                        const int transId = 1;
                        var mainDetails = new TblDyeingOrdersMainDetail
                        {
                            DyeingProductionOrder = dyeingHeaderRow.DyeingProductionOrder,
                            WareHouse = planHeader.Location,
                            TransactionType = 0,
                            TransId = transId,
                            Posted = false
                        };
                        entities.AddToTblDyeingOrdersMainDetails(mainDetails);
                        entities.SaveChanges();

                        var summarylist = dyeingSummaryList.Where(x => x.Vendor == item).ToList();
                        var sub = new ObservableCollection<TblDyeingOrdersDetail>();
                        foreach (var dyeingObject in summarylist)
                        {
                            var orderDetailsServices = new ObservableCollection<DyeingOrderDetailsService>();
                            foreach (var service in dyeingObject.DyeingSummaryServices)
                            {
                                orderDetailsServices.Add(new DyeingOrderDetailsService
                                {
                                    Notes = service.Notes,
                                    ServiceCode = service.ServiceCode,
                                    ServiceName = service.ServiceName,
                                    Qty = service.Qty
                                });
                            }
                            var entityCollection = new EntityCollection<DyeingOrderDetailsService>();

                            foreach (var entity in orderDetailsServices)
                            {
                                entityCollection.Add(entity);
                            }

                            sub.Add(new TblDyeingOrdersDetail
                            {
                                BatchNo = dyeingObject.BatchNo,
                                Color = dyeingObject.Color,
                                DyeingClass = dyeingObject.DyeingClass,
                                DyedFabric = dyeingObject.DyedFabric,
                                Unit = dyeingObject.Unit,
                                CalculatedTotalQty = Convert.ToSingle(dyeingObject.CalculatedTotalQty),
                                FabricCode = dyeingObject.FabricCode,
                                DyeingProductionOrder = dyeingHeaderRow.DyeingProductionOrder,
                                TransactionType = mainDetails.TransactionType,
                                TransId = mainDetails.TransId,
                                DyeingOrderDetailsServices = entityCollection,
                            });
                        }

                        foreach (var orderDetails in sub)
                        {
                            entities.AddToTblDyeingOrdersDetails(orderDetails);
                            entities.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}