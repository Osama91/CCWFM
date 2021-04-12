using CCWFM.Web.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace CCWFM.Web.Service.Operations.PurchasePlan
{
    public partial class PurchasePlan
    {
        [OperationContract]
        private List<TblGeneratePurchaseHeader> GetTblGeneratePurchaseHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblGeneratePurchaseHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblGeneratePurchaseHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblGeneratePurchaseHeaders.Include("TblLkpBrandSection1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblGeneratePurchaseHeaders.Count();
                    query = context.TblGeneratePurchaseHeaders.Include("TblLkpBrandSection1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        public void GeneratePurchaseOrderFromPurchasePLan(TblGeneratePurchaseHeader searchedItems, bool dyed, List<int> salesOrdersList)
        {
            using (var workFlowEntities = new WorkFlowManagerDBEntities())
            {
                //var factoryDelivery =
                //    workFlowEntities.TblFactoryDeliveries.FirstOrDefault(x => x.Iserial == searchedItems.TblFactoryDelivery);
                string purchaseType = null;
                string fabrictype = null;
                var tblPurchaseType = workFlowEntities.TblPurchaseTypes.FirstOrDefault(x => x.Iserial == searchedItems.TblPurchaseType);
                if (tblPurchaseType != null)
                {
                    purchaseType = tblPurchaseType.Ename;

                    if (purchaseType.StartsWith("Acc"))
                    {
                        purchaseType = null;
                        fabrictype = "Accessories";
                    }
                    else
                    {
                        purchaseType = "Accessories";
                    }
                }
                List<BOM> salesOrders;
                List<TblPurchaseOrderDetailBreakDown> listofBreakDown = new List<TblPurchaseOrderDetailBreakDown>();
                if (salesOrdersList.Any())
                {
                    salesOrders = (from bom in workFlowEntities.BOMs.Include("TblBOMStyleColors.TblColor").Include("TblBOMSizes").Include("TblSalesOrder1.TblFactoryGroup1").Include("TblSalesOrder1.TblStyle1").Include("TblSalesOrder1.TblSalesOrderColors.TblColor1").Include("TblSalesOrder1.TblSalesOrderColors.TblSalesOrderSizeRatios")
                                   where (bom.BOM_FabricType != purchaseType || purchaseType == null) && bom.OnStock == false
                                        && (bom.BOM_FabricType == fabrictype || fabrictype == null)
                                && salesOrdersList.Contains(bom.TblSalesOrder)
                                   //  && !workFlowEntities.TblPurchaseOrderDetailBreakDowns.Any(e => e.Bom == bom.Iserial &&bom.TblBOMStyleColors.Any(w=>w.Iserial==e.TblBOMStyleColor)&&bom.TblBOMSizes.Any(w=>w.Iserial==e.TblBOMSize))
                                   && bom.TblSalesOrder1.SalesOrderType == 2 && bom.Dyed == dyed && bom.OnStock == false
                                   select bom).ToList();

                    var BomIserial = salesOrders.Select(w => w.Iserial);
                    listofBreakDown = workFlowEntities.TblPurchaseOrderDetailBreakDowns.Where(w => BomIserial.Contains(w.Bom)).ToList();

                }
                else
                {
                    salesOrders = (from bom in workFlowEntities.BOMs.Include("TblBOMStyleColors.TblColor").Include("TblBOMSizes").Include("TblSalesOrder1.TblFactoryGroup1").Include("TblSalesOrder1.TblStyle1").Include("TblSalesOrder1.TblSalesOrderColors.TblColor1").Include("TblSalesOrder1.TblSalesOrderColors.TblSalesOrderSizeRatios")
                                   where (bom.BOM_FabricType != purchaseType || purchaseType == null)
                                        && (bom.BOM_FabricType == fabrictype || fabrictype == null)
                                && bom.TblSalesOrder1.SalesOrderType == 2 && bom.Dyed == dyed && bom.OnStock == false
                                && (bom.TblSalesOrder1.TblStyle1.Brand == searchedItems.Brand || searchedItems.Brand == null)
                                && (bom.TblSalesOrder1.TblStyle1.TblLkpSeason == searchedItems.TblLkpSeason || searchedItems.TblLkpSeason == null)
                                && (bom.TblSalesOrder1.TblStyle1.TblLkpBrandSection == searchedItems.TblLkpBrandSection || searchedItems.TblLkpBrandSection == null)
                               // && (bom.TblSalesOrder1.DeliveryDate >= factoryDelivery.FromDate && bom.TblSalesOrder1.DeliveryDate <= factoryDelivery.ToDate)
                                   //   && !workFlowEntities.TblPurchaseOrderDetailBreakDowns.Any(e => e.Bom == bom.Iserial && bom.TblBOMStyleColors.Any(w => w.Iserial == e.TblBOMStyleColor) && bom.TblBOMSizes.Any(w => w.Iserial == e.TblBOMSize))
                                   //&& bom.TblSalesOrder1.Status == 0
                                   && bom.TblSalesOrder1.Status < 2 && bom.TblSalesOrder1.ReadyProduct!= true
                                   select bom).ToList();
                    var BomIserial = salesOrders.Select(w => w.Iserial);
                    listofBreakDown = workFlowEntities.TblPurchaseOrderDetailBreakDowns.Where(w => BomIserial.Contains(w.Bom)).ToList();
                }
                var querylist = new List<TblPurchaseOrderDetail>();
                foreach (var bom in salesOrders.ToList())
                {
                    var fabricquery = new tbl_FabricAttriputes();
                    var acc = new tbl_AccessoryAttributesHeader();
                    if (bom.BOM_FabricType != "Accessories")
                    {
                        fabricquery =
                      workFlowEntities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(x => x.Iserial == bom.BOM_Fabric);
                    }
                    else
                    {
                        acc = workFlowEntities.tbl_AccessoryAttributesHeader.Include("tbl_lkp_UoM").FirstOrDefault(x => x.Iserial == bom.BOM_Fabric);
                    }

                    double? temp = 0;
                    if (bom.BOM_FabricType != "Accessories" && fabricquery.tbl_lkp_UoM.Code == "Kg")
                    {
                        if (fabricquery.FabricID.Contains("D"))
                        {
                            temp = bom.TblBOMSizes.Max(x => x.MaterialUsage) *
                              ((fabricquery.WeightPerSquarMeterAfterWashMax / 1000 * fabricquery.DyedFabricWidthMax / 100));
                        }
                        else
                        {
                            temp = bom.TblBOMSizes.Max(x => x.MaterialUsage) *
                              ((fabricquery.WeightPerSquarMeterAsRawMax / 1000 * fabricquery.WidthAsRawMax / 100));
                        }
                    }
                    else
                    {
                        temp = bom.TblBOMSizes.Max(x => x.MaterialUsage);
                    }

                    foreach (var row in bom.TblBOMStyleColors.Where(w => w.FabricColor != null && bom.TblSalesOrder1.TblSalesOrderColors.Any(e => e.TblColor == w.StyleColor)))
                    {



                        var currency = "EGP";
                        var vendor = "";
                        var leadtime = 0;
                        var leadtimecalc = 0;
                        if (bom.TblSalesOrder1.TblFactoryGroup1 != null)
                        {
                            leadtimecalc = bom.TblSalesOrder1.TblFactoryGroup1.ProductionDays;
                        }
                        float basicPrice = 0;

                        var price = SharedOperation.GetTradeAgrementPrice(searchedItems.TblLkpSeason??0, (int)bom.BOM_Fabric, bom.BOM_FabricType, bom.Vendor, row.FabricColor ?? 0, searchedItems.TransDate, out currency, out vendor, out leadtime, out basicPrice);
                        var deliverytime = bom.TblSalesOrder1.DeliveryDate.Value.AddDays((leadtimecalc));
                        if (bom.BOM_FabricType != "Accessories")
                        {
                            foreach (var variable in bom.TblBOMSizes.Where(x => x.MaterialUsage != 0).ToList())
                            {
                                // Create New Link 
                                if (listofBreakDown.Where(w => w.TblBOMStyleColor == row.Iserial && w.TblBOMSize == variable.Iserial && bom.Iserial == w.Bom).Count() == 0)
                                {
                                    var tblPurchaseOrderDetailBreakDown = new TblPurchaseOrderDetailBreakDown
                                    {
                                        Bom = bom.Iserial,
                                        TblBOMStyleColor = row.Iserial,
                                        TblBOMSize = variable.Iserial
                                    };
                                    try
                                    {
                                        tblPurchaseOrderDetailBreakDown.Qty =
                                     (float)
                                         (bom.TblSalesOrder1.TblSalesOrderColors.FirstOrDefault(
                                             x => x.TblColor == row.StyleColor)
                                             .TblSalesOrderSizeRatios.Where(
                                                 w => variable.StyleSize == w.Size)
                                             .Sum(x => x.ProductionPerSize) * temp);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception("SalesOrder no. " + bom.TblSalesOrder1.SalesOrderCode);
                                    }

                                    //tblPurchaseOrderDetailBreakDown.Qty =
                                    //(float)
                                    //    (bom.TblSalesOrder1.TblSalesOrderColors.Where(
                                    //        x => x.TblColor == row.StyleColor)
                                    //        .Sum(x => x.Total) * temp);
                                    tblPurchaseOrderDetailBreakDown.Qty = tblPurchaseOrderDetailBreakDown.Qty
                                        + (tblPurchaseOrderDetailBreakDown.Qty * (bom.BomHorizontalShrinkage / 100)) +
                                        (tblPurchaseOrderDetailBreakDown.Qty * (bom.BomVerticalShrinkage / 100))
                                        ;
                                    querylist.Add(new TblPurchaseOrderDetail
                                    {
                                        Canceled = false,
                                        ItemId = fabricquery.FabricID,
                                        FabricColor = row.FabricColor,
                                        Qty = tblPurchaseOrderDetailBreakDown.Qty,
                                        RemaningQty = tblPurchaseOrderDetailBreakDown.Qty,
                                        ItemType = bom.BOM_FabricType,
                                        Unit = fabricquery.tbl_lkp_UoM.Code,
                                        Price = price,
                                        BasicPrice = basicPrice,
                                        TblPurchaseOrderHeader1 = new TblPurchaseOrderHeader { CurrencyCode = currency, Vendor = vendor, DeliveryDate = deliverytime },

                                        TblPurchaseOrderDetailBreakDowns = new EntityCollection<TblPurchaseOrderDetailBreakDown> { tblPurchaseOrderDetailBreakDown },
                                    });
                                }
                            }
                        }
                        else
                        {
                            foreach (var variable in bom.TblBOMSizes.Where(x => x.MaterialUsage != 0).ToList())
                            {
                                // Create New Link 
                                if (listofBreakDown.Where(w => w.TblBOMStyleColor == row.Iserial && w.TblBOMSize == variable.Iserial && bom.Iserial == w.Bom).Count() == 0)
                                {

                                    var tblPurchaseOrderDetailBreakDown = new TblPurchaseOrderDetailBreakDown
                                    {
                                        Bom = bom.Iserial,
                                        TblBOMStyleColor = row.Iserial,
                                        TblBOMSize = variable.Iserial,
                                    };
                                    try
                                    {
                                        tblPurchaseOrderDetailBreakDown.Qty =
                                     (float)
                                         (bom.TblSalesOrder1.TblSalesOrderColors.FirstOrDefault(
                                             x => x.TblColor == row.StyleColor)
                                             .TblSalesOrderSizeRatios.Where(
                                                 w => variable.StyleSize == w.Size)
                                             .Sum(x => x.ProductionPerSize) * variable.MaterialUsage);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception("SalesOrder no. " + bom.TblSalesOrder1.SalesOrderCode);
                                    }

                                    querylist.Add(new TblPurchaseOrderDetail
                                    {
                                        Canceled = false,
                                        ItemId = acc.Code,
                                        FabricColor = row.FabricColor,
                                        Qty = tblPurchaseOrderDetailBreakDown.Qty,
                                        RemaningQty = tblPurchaseOrderDetailBreakDown.Qty,
                                        Size = variable.FabricSize,
                                        ItemType = bom.BOM_FabricType,
                                        Unit = acc.tbl_lkp_UoM.Code,
                                        Price = price,
                                        BasicPrice = basicPrice,
                                        TblPurchaseOrderHeader1 = new TblPurchaseOrderHeader { CurrencyCode = currency, Vendor = vendor, DeliveryDate = deliverytime },

                                        TblPurchaseOrderDetailBreakDowns = new EntityCollection<TblPurchaseOrderDetailBreakDown> { tblPurchaseOrderDetailBreakDown },
                                    });
                                }
                            }
                        }
                    }
                }

                foreach (var row in querylist.Where(x => x.FabricColor != null).GroupBy(w => new { w.ItemId, w.FabricColor, w.Size, w.TblPurchaseOrderHeader1.Vendor, w.TblPurchaseOrderHeader1.CurrencyCode }))
                {
                    var transactiontype = 0;
                    if (dyed)
                    {
                        transactiontype = 1;
                    }
                    var deliverydate =
                        querylist.Where(
                            x =>
                                x.TblPurchaseOrderHeader1.Vendor == row.Key.Vendor &&
                                x.TblPurchaseOrderHeader1.CurrencyCode == row.Key.CurrencyCode).OrderBy(x => x.TblPurchaseOrderHeader1.DeliveryDate).FirstOrDefault().TblPurchaseOrderHeader1.DeliveryDate;

                    var purchaseOrderHeader = new TblPurchaseOrderHeader();
                    var temprow = workFlowEntities.TblPurchaseOrderHeaders.FirstOrDefault(
                          x => x.DeliveryDate == deliverydate && x.Vendor == row.Key.Vendor && x.CurrencyCode == row.Key.CurrencyCode && x.TblGeneratePurchaseHeader == searchedItems.Iserial && x.TblPurchaseHeaderType == transactiontype);
                    var temprowDyed = workFlowEntities.TblPurchaseOrderHeaders.FirstOrDefault(
                         x => x.DeliveryDate == deliverydate && x.Vendor == row.Key.Vendor && x.CurrencyCode == row.Key.CurrencyCode && x.TblGeneratePurchaseHeader == searchedItems.Iserial && x.TblPurchaseHeaderType == 0);
                    var dyedpurchaseOrder = new TblPurchaseOrderHeader();
                    if (temprow == null)
                    {
                        purchaseOrderHeader.Vendor = row.Key.Vendor;
                        purchaseOrderHeader.DeliveryDate = deliverydate;
                        purchaseOrderHeader.CurrencyCode = row.Key.CurrencyCode;

                        purchaseOrderHeader.TransDate = searchedItems.TransDate;
                        purchaseOrderHeader.TblGeneratePurchaseHeader = searchedItems.Iserial;
                        purchaseOrderHeader.TblPurchaseHeaderType = transactiontype;

                        workFlowEntities.TblPurchaseOrderHeaders.AddObject(purchaseOrderHeader);
                        workFlowEntities.SaveChanges();
                    }
                    else
                    {
                        purchaseOrderHeader = temprow;
                    }
                    if (temprowDyed == null)
                    {
                        if (dyed)
                        {
                            dyedpurchaseOrder.Vendor = row.Key.Vendor;
                            dyedpurchaseOrder.DeliveryDate = deliverydate;
                            dyedpurchaseOrder.CurrencyCode = row.Key.CurrencyCode;
                            dyedpurchaseOrder.TransDate = searchedItems.TransDate;
                            dyedpurchaseOrder.TblGeneratePurchaseHeader = searchedItems.Iserial;
                            dyedpurchaseOrder.TblPurchaseHeaderType = 0;
                            dyedpurchaseOrder.TblPurchaseHeader = purchaseOrderHeader.Iserial;
                            workFlowEntities.TblPurchaseOrderHeaders.AddObject(dyedpurchaseOrder);
                            workFlowEntities.SaveChanges();
                        }
                    }
                    else
                    {
                        dyedpurchaseOrder = temprowDyed;
                    }

                    var tblPurchaseOrderDetailBreakDowns = new EntityCollection<TblPurchaseOrderDetailBreakDown>();
                    foreach (var variable in row.Select(x => x.TblPurchaseOrderDetailBreakDowns))
                    {
                        foreach (var VARIABLE in variable)
                        {
                            var tblPurchaseOrderDetailBreakDown = VARIABLE;
                            if (variable != null)
                            {
                                tblPurchaseOrderDetailBreakDowns.Add(new TblPurchaseOrderDetailBreakDown
                                {
                                    Bom = tblPurchaseOrderDetailBreakDown.Bom,
                                    Qty = tblPurchaseOrderDetailBreakDown.Qty,
                                    TblBOMSize = tblPurchaseOrderDetailBreakDown.TblBOMSize,
                                    TblBOMStyleColor = tblPurchaseOrderDetailBreakDown.TblBOMStyleColor,
                                });
                            }
                        }
                    }
                    var newrow = new TblPurchaseOrderDetail
                    {
                        ItemId = row.Key.ItemId,
                        FabricColor = row.Key.FabricColor,
                        Qty = row.Sum(q => q.Qty)
                        ,
                        Canceled = false,
                        RemaningQty = row.Sum(q => q.Qty),
                        Size = row.FirstOrDefault().Size,
                        ItemType = row.FirstOrDefault().ItemType,
                        Unit = row.FirstOrDefault().Unit,
                        TblPurchaseOrderDetailBreakDowns = tblPurchaseOrderDetailBreakDowns,
                        TblPurchaseOrderHeader = purchaseOrderHeader.Iserial,
                        Price = row.FirstOrDefault().Price,
                        BasicPrice = row.FirstOrDefault().BasicPrice,
                    };

                    workFlowEntities.TblPurchaseOrderDetails.AddObject(newrow);

                    workFlowEntities.SaveChanges();
                    if (dyed)
                    {
                        foreach (var breakDowns in newrow.TblPurchaseOrderDetailBreakDowns)
                        {
                            var bom = workFlowEntities.BOMs.Include("BomFabricBoms").Where(x => x.BOM_FabricType != "Service").First(x => x.Iserial == breakDowns.Bom);

                            foreach (var bomFabricBom in bom.BomFabricBoms.Where(x => x.ItemType != "Service"))
                            {
                                var fabrictemp =
                                    workFlowEntities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(
                                        x => x.Iserial == bomFabricBom.Item);
                                var newrowqq = new TblPurchaseOrderDetail
                                {
                                    Canceled = false,
                                    ItemId = fabrictemp.FabricID,
                                    FabricColor = row.Key.FabricColor,
                                    Qty =
                                      row.Sum(q => q.Qty) * bomFabricBom.MaterialUsage,
                                    RemaningQty = row.Sum(q => q.Qty) * bomFabricBom.MaterialUsage,
                                    Size = row.FirstOrDefault().Size,
                                    ItemType = bomFabricBom.ItemType,
                                    Unit = fabrictemp.tbl_lkp_UoM.Code,
                                    TblPurchaseOrderHeader = dyedpurchaseOrder.Iserial,
                                    Price = row.FirstOrDefault().Price,
                                    BasicPrice = row.FirstOrDefault().BasicPrice
                                };
                                workFlowEntities.TblPurchaseOrderDetails.AddObject(newrowqq);

                                workFlowEntities.SaveChanges();
                            }
                        }
                    }
                }
            }
            if (!dyed)
            {
                GeneratePurchaseOrderFromPurchasePLan(searchedItems, true, salesOrdersList);
            }
            //return querylist;
        }

        [OperationContract]
        public void GeneratePurchaseOrderFromUnpannedPurchase(int iserial)
        {
            using (var workFlowEntities = new WorkFlowManagerDBEntities())
            {
                var planheader = workFlowEntities.TblGeneratePurchaseHeaders.Include("TblPurchaseOrderHeaders.TblPurchaseOrderDetails").FirstOrDefault(x => x.Iserial == iserial);
                string purchaseType = null;
                string fabrictype = null;
                var tblPurchaseType = workFlowEntities.TblPurchaseTypes.FirstOrDefault(x => x.Iserial == planheader.TblPurchaseType);
                if (tblPurchaseType != null)
                {
                    purchaseType = tblPurchaseType.Ename;

                    if (purchaseType.StartsWith("Acc"))
                    {
                        purchaseType = null;
                        fabrictype = "Accessories";
                    }
                    else
                    {
                        fabrictype = null;
                        purchaseType = "Accessories";
                    }
                }

                foreach (var purchaseOrderHeader in planheader.TblPurchaseOrderHeaders)
                {
                    foreach (var purchaseOrderDetail in purchaseOrderHeader.TblPurchaseOrderDetails)
                    {
                        var fabric = 0;

                        if (fabrictype == "Accessories")
                        {
                            fabric =
                                workFlowEntities.tbl_AccessoryAttributesHeader.FirstOrDefault(
                                    x => x.Code == purchaseOrderDetail.ItemId).Iserial;
                        }
                        else
                        {
                            fabric =
                                workFlowEntities.tbl_FabricAttriputes.FirstOrDefault(
                                    x => x.FabricID == purchaseOrderDetail.ItemId).Iserial;
                        }

                        var salesOrders = (from bom in workFlowEntities.BOMs.Include("TblBOMStyleColors.TblColor").Include("TblBOMSizes").Include("TblSalesOrder1.TblStyle1").Include("TblSalesOrder1.TblSalesOrderColors.TblColor1").Include("TblSalesOrder1.TblSalesOrderColors.TblSalesOrderSizeRatios")
                                           where (bom.BOM_FabricType != purchaseType || purchaseType == null)
                                           && (bom.BOM_FabricType == fabrictype || fabrictype == null)
                                           && bom.TblSalesOrder1.SalesOrderType == 2
                                           && (bom.TblSalesOrder1.TblStyle1.Brand == planheader.Brand || planheader.Brand == null)
                                           && (bom.TblSalesOrder1.TblStyle1.TblLkpSeason == planheader.TblLkpSeason || planheader.TblLkpSeason == null)
                                           && (bom.TblSalesOrder1.TblStyle1.TblLkpBrandSection == planheader.TblLkpBrandSection || planheader.TblLkpBrandSection == null)
                                           && (!workFlowEntities.TblPurchaseOrderDetailBreakDowns.Include("TblPurchaseOrderDetail1").Where(x => x.TblPurchaseOrderDetail1.FabricColor == purchaseOrderDetail.FabricColor).Select(x => x.Bom).Contains(bom.Iserial))
                                           && bom.BOM_Fabric == fabric && bom.TblBOMStyleColors.Select(x => x.FabricColor).Contains(purchaseOrderDetail.FabricColor)
                                           select bom);

                        foreach (var bom in salesOrders.ToList())
                        {
                            var fabricquery = new tbl_FabricAttriputes();
                            //     var acc = new tbl_AccessoryAttributesHeader();
                            if (bom.BOM_FabricType != "Accessories")
                            {
                                fabricquery =
                              workFlowEntities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(x => x.Iserial == bom.BOM_Fabric);
                            }
                            //else
                            //{
                            //    acc = workFlowEntities.tbl_AccessoryAttributesHeader.Include("tbl_lkp_UoM").FirstOrDefault(x => x.Iserial == bom.BOM_Fabric);
                            //}

                            double? temp = 0;
                            if (bom.BOM_FabricType != "Accessories" && fabricquery.tbl_lkp_UoM.Code != "Meter")
                            {
                                temp = bom.TblBOMSizes.Max(x => x.MaterialUsage) *
                                          ((fabricquery.WeightPerSquarMeterAsRawMax / 1000 * fabricquery.WidthAsRawMax / 100));
                            }
                            else
                            {
                                temp = bom.TblBOMSizes.Max(x => x.MaterialUsage);
                            }

                            foreach (var row in bom.TblBOMStyleColors.Where(w => w.FabricColor != null && w.FabricColor == purchaseOrderDetail.FabricColor))
                            {
                                if (bom.BOM_FabricType != "Accessories")
                                {
                                    purchaseOrderDetail.RemaningQty = purchaseOrderDetail.RemaningQty - (float)
                                        (bom.TblSalesOrder1.TblSalesOrderColors.Where(
                                            x =>
                                                x.TblColor1.Code.ToLower() ==
                                                row.TblColor1.Code.ToLower())
                                            .Sum(x => x.Total) * temp);

                                    var tblPurchaseOrderDetailBreakDown = new TblPurchaseOrderDetailBreakDown
                                    {
                                        Bom = bom.Iserial,
                                        TblPurchaseOrderDetail = purchaseOrderDetail.Iserial,
                                        Qty = (bom.TblSalesOrder1.TblSalesOrderColors.Where(
                                            x =>
                                                x.TblColor1.Code.ToLower() ==
                                                row.TblColor1.Code.ToLower())
                                            .Sum(x => x.Total) * temp)
                                    };
                                    workFlowEntities.TblPurchaseOrderDetailBreakDowns.AddObject(tblPurchaseOrderDetailBreakDown);
                                }
                                else
                                {
                                    foreach (var variable in bom.TblBOMSizes.GroupBy(w => w.FabricSize))
                                    {
                                        purchaseOrderDetail.RemaningQty = purchaseOrderDetail.RemaningQty -
                                                                          (float)
                                                                              (bom.TblSalesOrder1.TblSalesOrderColors
                                                                                  .FirstOrDefault(
                                                                                      x =>
                                                                                          x.TblColor1.Code.ToLower() ==
                                                                                          row.TblColor1.Code.ToLower())
                                                                                  .TblSalesOrderSizeRatios.Where(
                                                                                      w =>
                                                                                          variable.Select(
                                                                                              x => x.StyleSize)
                                                                                              .Contains(w.Size))
                                                                                  .Sum(x => x.ProductionPerSize) * temp);

                                        var tblPurchaseOrderDetailBreakDown = new TblPurchaseOrderDetailBreakDown
                                        {
                                            Bom = bom.Iserial,
                                            TblPurchaseOrderDetail = purchaseOrderDetail.Iserial,
                                            Qty = (float)
                                                                              (bom.TblSalesOrder1.TblSalesOrderColors
                                                                                  .FirstOrDefault(
                                                                                      x =>
                                                                                          x.TblColor1.Code.ToLower() ==
                                                                                          row.TblColor1.Code.ToLower())
                                                                                  .TblSalesOrderSizeRatios.Where(
                                                                                      w =>
                                                                                          variable.Select(
                                                                                              x => x.StyleSize)
                                                                                              .Contains(w.Size))
                                                                                  .Sum(x => x.ProductionPerSize) * temp)
                                        };
                                        workFlowEntities.TblPurchaseOrderDetailBreakDowns.AddObject(tblPurchaseOrderDetailBreakDown);
                                    }
                                }
                            }
                        }
                        workFlowEntities.SaveChanges();
                    }
                }
            }

            //return querylist;
        }

        [OperationContract]
        public void RecalcAllModifiedPlans()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var plans = context.TblGeneratePurchaseHeaders.Where(w => w.Modified == true).ToList();
                int temp;
                foreach (var tblGeneratePurchaseHeader in plans)
                {
                    RecalculateTblGeneratePurchaseHeader(tblGeneratePurchaseHeader, 0, out temp, null);
                }
            }
        }

        [OperationContract]
        private TblGeneratePurchaseHeader RecalculateTblGeneratePurchaseHeader(TblGeneratePurchaseHeader newRow,
            int index, out int outindex, List<int> SalesOrders)
        {
            outindex = 0;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (!SalesOrders.Any() || SalesOrders == null)
                {
                    var temp = (from tblSalesOrders in context.TblSalesOrders
                                join boMs in context.BOMs on new { tblSalesOrders.Iserial } equals new { Iserial = boMs.TblSalesOrder }
                                join tblPurchaseOrderDetailBreakDowns in context.TblPurchaseOrderDetailBreakDowns on new { boMs.Iserial } equals new { Iserial = tblPurchaseOrderDetailBreakDowns.Bom }
                                join tblPurchaseOrderDetails in context.TblPurchaseOrderDetails on new { iserial = tblPurchaseOrderDetailBreakDowns.TblPurchaseOrderDetail } equals new { iserial = tblPurchaseOrderDetails.Iserial }
                                join tblpurchaseheader in context.TblPurchaseOrderHeaders on new { iserial = (int)tblPurchaseOrderDetails.TblPurchaseOrderHeader } equals new { iserial = tblpurchaseheader.Iserial }
                                join tblGeneratePurchaseHeader in context.TblGeneratePurchaseHeaders on new { iserial = (int)tblpurchaseheader.TblGeneratePurchaseHeader } equals new { iserial = tblGeneratePurchaseHeader.Iserial }
                                where tblGeneratePurchaseHeader.Iserial == newRow.Iserial
                                select new
                                {
                                    SalesOrder = tblSalesOrders.SalesOrderCode,
                                    tblSalesOrders.Iserial
                                }).Distinct().ToList();

                    foreach (var variable in temp)
                    {
                        SalesOrders.Add(variable.Iserial);
                    }
                }
                var code = "";
                var oldRow = (from e in context.TblPurchaseOrderHeaders.Include("TblGeneratePurchaseHeader1").Include("TblPurchaseOrderDetails.TblPurchaseRequestLinks")
                              where e.TblGeneratePurchaseHeader == newRow.Iserial
                              select e).ToList();
                var tblPurchaseOrderHeader = oldRow.FirstOrDefault();
                if (tblPurchaseOrderHeader != null)
                {
                    code = tblPurchaseOrderHeader.TblGeneratePurchaseHeader1.Code;

                    foreach (var VARIABLE in oldRow)
                    {
                        var PurchaeOrderDetailIserials = VARIABLE.TblPurchaseOrderDetails.Select(w => w.Iserial).ToList();
                        if (context.TblPurchaseRequestLinks.Any(w => PurchaeOrderDetailIserials.Contains(w.TblPurchaseOrderDetail)))
                        {
                            foreach (var item in VARIABLE.TblPurchaseOrderDetails.ToList())
                            {
                                if (!item.TblPurchaseRequestLinks.Any())
                                {
                                    context.TblPurchaseOrderDetails.DeleteObject(item);
                                }
                            }

                        }
                        else
                        {
                            context.TblPurchaseOrderHeaders.DeleteObject(VARIABLE);
                        }
                    }
                }
                context.SaveChanges();
                return UpdateOrInsertTblGeneratePurchaseHeader(newRow, index, out outindex, SalesOrders, code);
            }
        }

        [OperationContract]
        private TblGeneratePurchaseHeader UpdateOrInsertTblGeneratePurchaseHeader(TblGeneratePurchaseHeader newRow, int index, out int outindex, List<int> SalesOrders, string code = "")
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblGeneratePurchaseHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    var oldRowa = (from e in context.TblGeneratePurchaseHeaders
                                   where e.Brand == newRow.Brand && e.TblLkpSeason == newRow.TblLkpSeason
                                   select e).Count();
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        newRow.Code = newRow.Code + "-" + (oldRowa + 1);
                    }
                    else
                    {
                        newRow.Code = code;
                    }

                    if (context.TblGeneratePurchaseHeaders.Any(w => w.Code == newRow.Code))
                    {
                        newRow.Code = newRow.Code + "-" + (oldRowa + 2);
                    }

                    if (context.TblGeneratePurchaseHeaders.Any(w => w.Code == newRow.Code))
                    {
                        newRow.Code = newRow.Code + "-" + (oldRowa + 3);
                    }

                    if (context.TblGeneratePurchaseHeaders.Any(w => w.Code == newRow.Code))
                    {
                        newRow.Code = newRow.Code + "-" + (oldRowa + 4);
                    }

                    var tblSeasonCurrency = context.TblSeasonCurrencies.Where(
                        w => w.TblLkpSeason == newRow.TblLkpSeason).ToList();
                    newRow.TblGeneratePurchaseHeaderCurrencies = new EntityCollection<TblGeneratePurchaseHeaderCurrency>();

                    foreach (var item in tblSeasonCurrency)
                    {
                        var rate = item.ExRate;

                        var row = new TblGeneratePurchaseHeaderCurrency
                        {
                            CurrencyCode = item.CurrencyCode,
                            CustomerExRate = rate,
                            VendorExRate = rate,
                        };

                        newRow.TblGeneratePurchaseHeaderCurrencies.Add(row);

                    }
                    context.TblGeneratePurchaseHeaders.AddObject(newRow);
                }

                context.SaveChanges();
                if (newRow.TblPlanType == 0)
                {
                    GeneratePurchaseOrderFromPurchasePLan(newRow, false, SalesOrders);
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblGeneratePurchaseHeader(TblGeneratePurchaseHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblGeneratePurchaseHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPurchaseOrderHeader> GetTblPurchaseOrderHeader(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out Dictionary<string, int> SalesorderList, out List<TblPurchaseRequestLink> PurchaseRequestLink)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseOrderHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGeneratePurchaseHeader =(@group)";
                    valuesObjects.Add("group", groupId);
                    //filter = filter + " and it.TblPurchaseHeaderType =(@ddd)";
                    //valuesObjects.Add("group", 0);

                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseOrderHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseOrderHeaders.Include("TblPurchaseOrderHeader1").Include("TblPurchaseHeaderType1").Include("TblGeneratePurchaseHeader1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblPurchaseOrderHeaders.Count(
                            x => x.TblGeneratePurchaseHeader == groupId && x.TblPurchaseHeader == null);// && x.TblPurchaseHeaderType == 0);
                    query = context.TblPurchaseOrderHeaders.Include("TblPurchaseOrderHeader1").Include("TblPurchaseHeaderType1").Include("TblGeneratePurchaseHeader1").OrderBy(sort).Where(v => v.TblGeneratePurchaseHeader == groupId && v.TblPurchaseHeader == null).Skip(skip).Take(take);
                }

                var temp = (from tblSalesOrders in context.TblSalesOrders
                            join boMs in context.BOMs on new { tblSalesOrders.Iserial } equals new { Iserial = boMs.TblSalesOrder }
                            join tblPurchaseOrderDetailBreakDowns in context.TblPurchaseOrderDetailBreakDowns on new { boMs.Iserial } equals new { Iserial = tblPurchaseOrderDetailBreakDowns.Bom }
                            join tblPurchaseOrderDetails in context.TblPurchaseOrderDetails on new { iserial = tblPurchaseOrderDetailBreakDowns.TblPurchaseOrderDetail } equals new { iserial = tblPurchaseOrderDetails.Iserial }
                            join tblpurchaseheader in context.TblPurchaseOrderHeaders on new { iserial = (int)tblPurchaseOrderDetails.TblPurchaseOrderHeader } equals new { iserial = tblpurchaseheader.Iserial }
                            join tblGeneratePurchaseHeader in context.TblGeneratePurchaseHeaders on new { iserial = (int)tblpurchaseheader.TblGeneratePurchaseHeader } equals new { iserial = tblGeneratePurchaseHeader.Iserial }
                            where tblGeneratePurchaseHeader.Iserial == groupId
                            select new
                            {
                                SalesOrder = tblSalesOrders.SalesOrderCode,
                                tblSalesOrders.Iserial
                            }).Distinct().ToList();
                SalesorderList = new Dictionary<string, int>();
                foreach (var variable in temp)
                {
                    var temprow = new Dictionary<string, int>();
                    temprow.Add(variable.SalesOrder, variable.Iserial);
                    SalesorderList.Add(variable.SalesOrder, variable.Iserial);
                }

                var purchaseOrderIserial = query.Select(w => w.Iserial).ToList();
                var PurchaeOrderDetailIserials = context.TblPurchaseOrderDetails.Where(w => purchaseOrderIserial.Contains(w.TblPurchaseOrderHeader ?? 0)).Select(w => w.Iserial).ToList();
                if (context.TblPurchaseRequestLinks.Any(w => PurchaeOrderDetailIserials.Contains(w.TblPurchaseOrderDetail)))
                {
                    PurchaseRequestLink = context.TblPurchaseRequestLinks.Include("TblPurchaseOrderDetail1").Where(w => PurchaeOrderDetailIserials.Contains(w.TblPurchaseOrderDetail)).ToList();

                }
                else
                {
                    PurchaseRequestLink = null;
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblPurchaseOrderHeader UpdateOrInsertTblPurchaseOrderHeader(TblPurchaseOrderHeader newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblPurchaseOrderHeaders.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderHeader(TblPurchaseOrderHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPurchaseOrderDetail> GetTblPurchaseOrderDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)//, out Dictionary<int?, double?> purchaseRec)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseOrderDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPurchaseOrderHeader=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseOrderDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseOrderDetails.Include("TblColor").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseOrderDetails.Count(x => x.TblPurchaseOrderHeader == groupId);
                    query = context.TblPurchaseOrderDetails.Include("TblColor").OrderBy(sort).Where(v => v.TblPurchaseOrderHeader == groupId).Skip(skip).Take(take);
                }
                var listofPurchase = query.Select(x => x.Iserial);


                //purchaseRec = context.TblPurchaseReceiveDetails.Where(
                //    x => listofPurchase.Any(l => x.TblPurchaseOrderDetail == l)).GroupBy(x => x.TblPurchaseOrderDetail).ToDictionary(t => t.Key, t => t.Sum(w => w.Qty));
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblPurchaseOrderDetailBreakDown> GetTblPurchaseOrderDetailBreakDown(int groupId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblPurchaseOrderDetailBreakDowns.Include("BOM1.TblSalesOrder1")
                    .Where(v => v.TblPurchaseOrderDetail == groupId);

                return query.ToList();
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderDetailBreakDown(TblPurchaseOrderDetailBreakDown row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetailBreakDowns
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var oldqty = context.TblPurchaseOrderDetails.FirstOrDefault(x => x.Iserial == oldRow.TblPurchaseOrderDetail);

                    if (oldqty != null) oldqty.RemaningQty = oldqty.RemaningQty + oldRow.Qty;

                    context.DeleteObject(oldRow);
                }
                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblPurchaseOrderDetail UpdateOrInsertTblPurchaseOrderDetail(TblPurchaseOrderDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    newRow.RemaningQty = newRow.Qty;
                    context.TblPurchaseOrderDetails.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private void UpdateOrInsertTblPurchaseOrderDetailCanceled(int Iserial, bool canceled)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetails
                              where e.Iserial == Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    oldRow.Canceled = canceled;
                }
                context.SaveChanges();
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderDetail(TblPurchaseOrderDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void CollectPlans(int basePlan, int modifiedPlan)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderHeaders
                              where e.TblGeneratePurchaseHeader == modifiedPlan
                              select e).ToList();

                foreach (var tblPurchaseOrderHeader in oldRow)
                {
                    tblPurchaseOrderHeader.TblGeneratePurchaseHeader = basePlan;
                }
                context.SaveChanges();
            }
        }
    }
}