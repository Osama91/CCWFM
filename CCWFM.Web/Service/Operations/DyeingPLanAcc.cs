using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<EstimatedDyedMain_Result> GetEstimatedDyeingListAcc(string brand, int brandSection, string season, DateTime? fromDate, DateTime? toDate, out List<DyeingColorQuantitiesRequired_Result> ColorsValues)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var getDyeingList = (from s in entities.EstimatedDyedMainAcc(brand, brandSection, season, fromDate, toDate)
                                     orderby s.SalesOrderID
                                     select s).ToList();

                ColorsValues = (from s in entities.DyeingColorQuantitiesRequiredacc(brand, brandSection, season, fromDate, toDate)
                                select s).ToList();
                return getDyeingList;
            }
        }

        [OperationContract]
        public List<TblDyeingPlanACC> GetDyeingListAcc(int dyeingHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var getDyeingList = (from s in entities.TblDyeingPlanACCs
                                                         .Include("TblDyeingPlanDetailsAccs")
                                     where s.DyeingHeader == dyeingHeaderIserial
                                     select s).ToList();

                return getDyeingList;
            }
        }

        [OperationContract]
        public List<TblDyeingPlanHeaderAcc> GetDyeingHeaderListAcc(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingPlanHeaderAcc> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblDyeingPlanHeaderAccs.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblDyeingPlanHeaderAccs.Include("TblLkpSeason1").Include("TblLkpBrandSection1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblDyeingPlanHeaderAccs.Count();
                    query = context.TblDyeingPlanHeaderAccs.Include("TblLkpSeason1").Include("TblLkpBrandSection1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblDyeingSummaryAcc> GetDyeingSummaryAcc(int headerIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.TblDyeingSummaryAccs.Where(x => x.DyeingHeader == headerIserial).ToList();
            }
        }

        [OperationContract]
        public TblDyeingPlanHeaderAcc SaveDyeingHeaderListAcc(TblDyeingPlanHeaderAcc header, List<TblDyeingPlanACC> mainDetails, List<TblDyeingSummaryAcc> summaryList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                #region PlanWithoutSummary

                if (header.Iserial != 0)
                {
                    var h = entities.TblDyeingPlanHeaderAccs.SingleOrDefault(x => x.Iserial == header.Iserial);

                    h.DocNo = header.DocNo;
                    h.Location = header.Location;
                    h.Brand = header.Brand;
                    h.TblLkpSeason = header.TblLkpSeason;
                    h.TransactionDate = header.TransactionDate;
                }
                else
                {
                    entities.AddToTblDyeingPlanHeaderAccs(header);
                }

                foreach (var dyeingPlanRow in mainDetails)
                {
                    if (dyeingPlanRow.Iserial == 0)
                    {
                        dyeingPlanRow.DyeingHeader = header.Iserial;
                        entities.AddToTblDyeingPlanACCs(dyeingPlanRow);
                    }
                    else
                    {
                        var mainDetailRow = (from d in entities.TblDyeingPlanACCs
                                             where d.DyeingHeader == header.Iserial &&
                                             d.Iserial == dyeingPlanRow.Iserial
                                             select d).SingleOrDefault();

                        if (mainDetailRow != null)
                        {
                            // MainDetailRow.TransactionDate = item.TransactionDate;
                        }
                        else
                        {
                            entities.AddToTblDyeingPlanACCs(dyeingPlanRow);
                        }
                    }
                    entities.SaveChanges();

                    if (dyeingPlanRow.DyeingHeader != 0)
                    {
                        foreach (var dyeingDetailsPlanRow in dyeingPlanRow.TblDyeingPlanDetailsAccs.ToList())
                        {
                            var dyeingOrderDetails = (from d in entities.TblDyeingPlanDetailsAccs
                                                      where d.Iserial == dyeingDetailsPlanRow.Iserial
                                                      && d.FabricColorName == dyeingDetailsPlanRow.FabricColorName
                                                      select d).SingleOrDefault();
                            if (dyeingOrderDetails != null)
                            {
                                dyeingOrderDetails.BatchNo = dyeingDetailsPlanRow.BatchNo;
                                dyeingOrderDetails.FabricColorValue = dyeingDetailsPlanRow.FabricColorValue;
                                dyeingOrderDetails.BatchNoCreated = dyeingDetailsPlanRow.BatchNoCreated;
                                dyeingOrderDetails.OldColor = dyeingDetailsPlanRow.OldColor;
                            }
                            else
                            {
                                dyeingDetailsPlanRow.TblDyeingPlanACC = null;
                                entities.AddToTblDyeingPlanDetailsAccs(dyeingDetailsPlanRow);
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
                        var querySummaryrow = (from d in entities.TblDyeingSummaryAccs
                                               where d.Iserial == summaryRow.Iserial
                                               select d).SingleOrDefault();

                        if (querySummaryrow != null)
                        {
                            querySummaryrow.Color = summaryRow.Color;
                            querySummaryrow.DyedFabric = summaryRow.DyedFabric;
                            querySummaryrow.Unit = summaryRow.Unit;
                            querySummaryrow.CalculatedTotalQty = summaryRow.CalculatedTotalQty;
                            querySummaryrow.FabricCode = summaryRow.FabricCode;
                            querySummaryrow.Vendor = summaryRow.Vendor;
                        }
                    }
                    else
                    {
                        summaryRow.TblDyeingPlanHeaderAcc = null;
                        summaryRow.DyeingHeader = header.Iserial;
                        entities.AddToTblDyeingSummaryAccs(summaryRow);
                    }
                    entities.SaveChanges();
                    foreach (var planLotsMasterRow in summaryRow.TblDyeingPlanLotsMasterAccs)
                    {
                        if (planLotsMasterRow.Iserial != 0)
                        {
                            var queryPlanLotsMasterRow = (from d in entities.TblDyeingPlanLotsMasterAccs
                                                          where d.Iserial == planLotsMasterRow.Iserial
                                                          select d).SingleOrDefault();

                            queryPlanLotsMasterRow.FabricCode = planLotsMasterRow.FabricCode;
                            queryPlanLotsMasterRow.FabricLot = planLotsMasterRow.FabricLot;
                            queryPlanLotsMasterRow.AvaliableQuantity = planLotsMasterRow.AvaliableQuantity;
                            queryPlanLotsMasterRow.Config = planLotsMasterRow.Config;
                            queryPlanLotsMasterRow.RequiredQuantity = planLotsMasterRow.RequiredQuantity;
                            queryPlanLotsMasterRow.Unit = planLotsMasterRow.Unit;
                        }
                        else
                        {
                            planLotsMasterRow.TblDyeingSummaryAcc = null;
                            planLotsMasterRow.DyeingsSummaryPlanIserial = summaryRow.Iserial;
                            entities.AddToTblDyeingPlanLotsMasterAccs(planLotsMasterRow);
                        }
                        entities.SaveChanges();

                        foreach (var planLotDetailsRow in planLotsMasterRow.TblDyeingPlanLotsDetailsAccs)
                        {
                            if (planLotDetailsRow.Iserial != 0)
                            {
                                var queryPlanLotsDetailsRow = (from d in entities.TblDyeingPlanLotsDetailsAccs
                                                               where d.Iserial == planLotDetailsRow.Iserial
                                                               select d).SingleOrDefault();

                                queryPlanLotsDetailsRow.DeliveryDate = planLotDetailsRow.DeliveryDate;
                                queryPlanLotsDetailsRow.RequiredQuantity = planLotDetailsRow.RequiredQuantity;
                                queryPlanLotsDetailsRow.SalesOrder = planLotDetailsRow.SalesOrder;
                                queryPlanLotsDetailsRow.Saved = planLotDetailsRow.Saved;
                            }
                            else
                            {
                                planLotDetailsRow.TblDyeingPlanLotsMasterAcc = null;
                                planLotDetailsRow.FabricLotMasterIserial = planLotsMasterRow.Iserial;
                                entities.AddToTblDyeingPlanLotsDetailsAccs(planLotDetailsRow);
                            }
                        }
                    }
                }

                entities.SaveChanges();

                #endregion SummarySaving

                return header;
            }
        }

        //[OperationContract]
        //public List<DyeingAxService> GetAxSummaryServicesAcc()
        //{
        //    using (var entities = new WorkFlowManagerDBEntities())
        //    {
        //        return entities.DyeingAxServices.ToList();
        //    }
        //}

        //[OperationContract]
        //public List<DyeingSummaryService> GetSavedSummaryServicesAcc(int iserial)
        //{
        //    using (var entities = new WorkFlowManagerDBEntities())
        //    {
        //        return entities.DyeingSummaryServices.Where(x => x.SummaryRowIserial == iserial).ToList();
        //    }
        //}

        //[OperationContract]
        //public void SaveSummaryServicesAcc(int iserial, string serviceCode, string serviceName, string notes, bool Checked)
        //{
        //    using (var entities = new WorkFlowManagerDBEntities())
        //    {
        //        if (Checked)
        //        {
        //            var dyeingSummaryServices = entities.DyeingSummaryServices.SingleOrDefault(x => x.SummaryRowIserial == iserial && x.ServiceCode == serviceCode);

        //            var summaryServiceNewRow = new DyeingSummaryService();

        //            if (dyeingSummaryServices == null)
        //            {
        //                summaryServiceNewRow.ServiceCode = serviceCode;
        //                summaryServiceNewRow.ServiceName = serviceName;
        //                summaryServiceNewRow.SummaryRowIserial = iserial;
        //                summaryServiceNewRow.Notes = notes;
        //                entities.AddToDyeingSummaryServices(summaryServiceNewRow);
        //            }
        //            else
        //            {
        //                summaryServiceNewRow = dyeingSummaryServices;
        //                summaryServiceNewRow.ServiceCode = serviceCode;
        //                summaryServiceNewRow.ServiceName = serviceName;
        //                summaryServiceNewRow.SummaryRowIserial = iserial;
        //                summaryServiceNewRow.Notes = notes;
        //            }
        //        }
        //        else
        //        {
        //            var dyeingSummaryServices = entities.DyeingSummaryServices.SingleOrDefault(x => x.SummaryRowIserial == iserial && x.ServiceCode == serviceCode);
        //            if (dyeingSummaryServices != null)
        //            {
        //                entities.DeleteObject(dyeingSummaryServices);
        //            }
        //        }
        //        entities.SaveChanges();
        //    }
        //}

        [OperationContract]
        public void DeleteDyeingRowAcc(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var dyeingRow = (from s in entities.TblDyeingPlanACCs
                                 where s.Iserial == iserial
                                 select s).SingleOrDefault();

                entities.TblDyeingPlanACCs.DeleteObject(dyeingRow);
                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void GenerateDyeingOrdersAcc(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var planHeader = entities.TblDyeingPlanHeaderAccs.SingleOrDefault(x => x.Iserial == iserial);
                planHeader.PlanGenerated = true;

                var dyeingSummaryList = entities.TblDyeingSummaryAccs.Where(x => x.DyeingHeader == planHeader.Iserial).ToList();
                var vendorList = dyeingSummaryList.Select(x => x.Vendor).Distinct().ToList();

                foreach (var item in vendorList)
                {
                    var dyeingHeaderRow = new TblDyeingOrdersHeaderAcc { DocPlan = planHeader.DocNo, Vendor = item };

                    entities.AddToTblDyeingOrdersHeaderAccs(dyeingHeaderRow);
                    entities.SaveChanges();

                    const int transId = 1;
                    var mainDetails = new TblDyeingOrdersMainDetailsACC
                    {
                        DyeingProductionOrder = dyeingHeaderRow.DyeingProductionOrder,
                        WareHouse = planHeader.Location,
                        TransactionType = 0,
                        TransId = transId,
                        Posted = false
                    };
                    entities.AddToTblDyeingOrdersMainDetailsACCs(mainDetails);
                    entities.SaveChanges();

                    var summarylist = dyeingSummaryList.Where(x => x.Vendor == item).ToList();
                    var sub = new ObservableCollection<TblDyeingOrdersDetailsAcc>();
                    foreach (var dyeingObject in summarylist)
                    {
                        sub.Add(new TblDyeingOrdersDetailsAcc
                        {
                            Size = dyeingObject.Size,
                            Color = dyeingObject.Color,

                            DyedFabric = dyeingObject.DyedFabric,
                            Unit = dyeingObject.Unit,
                            CalculatedTotalQty = Convert.ToSingle(dyeingObject.CalculatedTotalQty),
                            FabricCode = dyeingObject.FabricCode,
                            DyeingProductionOrder = dyeingHeaderRow.DyeingProductionOrder,
                            TransactionType = mainDetails.TransactionType,
                            TransId = mainDetails.TransId,
                        });
                    }

                    foreach (var orderDetails in sub)
                    {
                        entities.AddToTblDyeingOrdersDetailsAccs(orderDetails);
                        entities.SaveChanges();
                    }
                }
            }
        }
    }
}