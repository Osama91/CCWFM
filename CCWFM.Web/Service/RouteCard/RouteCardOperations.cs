using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Models.Inv;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.RouteCard
{
    // ReSharper disable once CheckNamespace
    public partial class RouteCardService
    {
        [OperationContract]
        public List<Tbl_TransactionType> GetTransactionTypes()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.Tbl_TransactionType.ToList();
            }
        }

        [OperationContract]
        private List<TblAuthWarehouse> GetLookUpWarehousePermissionType(int UserIserial, List<AuthWarehouseType> TypeList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                List<short> TypeListShort = new List<short>();
                foreach (var item in TypeList)
                {
                    TypeListShort.Add((short)item);
                }
                IQueryable<TblAuthWarehouse> query;


                query = context.TblAuthWarehouses.Include(nameof(TblAuthWarehouse.TblWarehouse)).Where(w =>
               w.AuthUserIserial == UserIserial && TypeListShort.Contains(w.PermissionType));


                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblRoute> GetRoutes(int? skip, int? take, int routeGroupId, string vendorCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (skip != null)
                    return context.TblRoutes.Where(x => x.VendorSerial == vendorCode && x.TblRouteGroup == routeGroupId).Skip((int)skip).Take((int)take).ToList();
                else
                    return context.TblRoutes.Where(x => x.VendorSerial == vendorCode && x.TblRouteGroup == routeGroupId).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblRouteGroup> GetRoutGroups(int? skip, int? take)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (skip != null)
                    return context.TblRouteGroups.Skip((int)skip).Take((int)take).ToList();
                else
                    return context.TblRouteGroups.ToList();
            }
        }

        [OperationContract]
        public List<RouteCardHeader> GetRouteCardHeaders(int skip, int take, int tblsalesorder, int color, int user, out List<Vendor> Vendors)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<RouteCardHeader> query;

                if (color == 0)
                {
                    query = context.RouteCardHeaders
                        .Include("TblAuthUser1").Include("TblAuthUser")
                        .Include("Tbl_TransactionType").Include("TblRouteDirection")
                        .Include("TblRouteGroup").Include("TblRoute").Include("RouteCardFabrics.TblSalesOrder1")
                        .Include("RouteCardFabrics.TblColor").Include("RouteCardFabrics.TblColor1")
                        .Include("RouteCardFabrics.TblColor2")
                        .Where(x => x.RouteCardDetails.Any(w => w.TblSalesOrder == tblsalesorder) || x.RouteCardFabrics.Any(w => w.TblSalesOrder == tblsalesorder)).OrderByDescending(w => w.Iserial).Skip(skip).Take(take);
                }
                else
                {
                    query = context.RouteCardHeaders.Include("TblAuthUser1")
                            .Include("TblAuthUser")
                            .Include("Tbl_TransactionType")
                            .Include("TblRouteDirection")
                            .Include("TblRouteGroup")
                            .Include("TblRoute")
                            .Include("RouteCardFabrics.TblSalesOrder1")
                            .Include("RouteCardFabrics.TblColor")
                            .Include("RouteCardFabrics.TblColor1")
                            .Include("RouteCardFabrics.TblColor2")
                            .Where(x => x.RouteCardDetails.Any(w => w.TblSalesOrder == tblsalesorder && w.TblColor == color) || x.RouteCardFabrics.Any(w => w.TblSalesOrder == tblsalesorder && w.StyleColor == color)).OrderByDescending(w => w.Iserial).Skip(skip).Take(take);
                }
                var Codes = query.Select(w => w.Vendor).ToList();
                Vendors = context.Vendors.Where(x => Codes.Contains(x.vendor_code)).ToList();
                var result = query.ToList();
                return result;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<RouteCardHeader> GetRouteCardHeadersBeforeInspection(int skip, int take, int tblsalesorder, int color, int user, out List<TblSize> sizes)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<RouteCardHeader> query;

                if (color == 0)
                {
                    query = context.RouteCardHeaders
                        .Include("TblAuthUser1").Include("TblAuthUser")
                        .Include("Tbl_TransactionType").Include("TblRouteDirection")
                        .Include("TblRouteGroup").Include("TblRoute").Include("RouteCardDetails.TblSalesOrder1.TblStyle1")
                        .Include("RouteCardDetails.TblColor1")

                        .Where(x => (x.RouteCardDetails.Any(w => w.TblSalesOrder == tblsalesorder) || x.RouteCardFabrics.Any(w => w.TblSalesOrder == tblsalesorder)) && x.tblTransactionType == 8).OrderByDescending(w => w.Iserial).Skip(skip).Take(take);
                }
                else
                {
                    query = context.RouteCardHeaders.Include("TblAuthUser1")
                            .Include("TblAuthUser")
                            .Include("Tbl_TransactionType")
                            .Include("TblRouteDirection")
                            .Include("TblRouteGroup")
                            .Include("TblRoute")
                            .Include("RouteCardDetails.TblSalesOrder1.TblStyle1")
                        .Include("RouteCardDetails.TblColor1")
                            .Where(
                                x =>
                                    (x.RouteCardDetails.Any(w => w.TblSalesOrder == tblsalesorder && w.TblColor == color) || x.RouteCardFabrics.Any(w => w.TblSalesOrder == tblsalesorder && w.StyleColor == color)) && x.tblTransactionType == 8).OrderByDescending(w => w.Iserial).Skip(skip).Take(take);
                }

                if (query.Any())
                {
                    var listofint = new List<int>();

                    foreach (var VARIABLE in query.FirstOrDefault().RouteCardDetails)
                    {
                        listofint.Add(VARIABLE.TblSalesOrder1.TblStyle1.TblSizeGroup);
                    }
                    sizes = context.TblSizes.Where(x => listofint.Contains(x.TblSizeGroup)).ToList();
                }
                else
                {
                    sizes = null;
                }
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<RouteCardDetail> GetRouteCardDetails(int? skip, int? take, int cardHeaderId, int direction, int routeGroupId, out List<TblSize> sizes)
        {
            var temp = new List<RouteCardDetail>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (skip != null)
                {
                    temp = context.RouteCardDetails.Include("TblColor1").Include("TblSalesOrder1.TblStyle1").Where(x => x.RouteCardHeaderIserial == cardHeaderId).OrderBy(x => x.SizeQuantity).Skip((int)skip).Take((int)take).ToList();
                }
                else
                {
                    temp = context.RouteCardDetails.Include("TblColor1").Include("TblSalesOrder1.TblStyle1").Where(x => x.RouteCardHeaderIserial == cardHeaderId).OrderBy(x => x.SizeQuantity).ToList();
                }

                if (temp.Any())
                {
                    var sizegroups = temp.Select(w => w.TblSalesOrder1.TblStyle1.TblSizeGroup).Distinct().ToList();
                    sizes = context.TblSizes.Where(x => sizegroups.Contains(x.TblSizeGroup)).ToList();
                }
                else
                {
                    sizes = null;
                }
            }
            return temp;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public RouteCardHeader AddRoutCard(RouteCardHeader header, List<RouteCardDetail> details, out List<RouteCardDetail> savedDetails, int postToAx, string transactionGuid, int userIserial)
        {
            header.Createdby = userIserial;
            header.LastUpdatedDate = DateTime.Now;

            using (var context = new WorkFlowManagerDBEntities())
            {
                if (header.RouteCardFabrics.Any())
                {
                    var MaxPackingTransid = context.RouteCardHeaders.Where(w => w.tblTransactionType == header.tblTransactionType).Max(w => w.PackingTransID);
                    header.PackingTransID = MaxPackingTransid + 1;
                }
                if (header.tblTransactionType == 5)
                {
                    var job = Operations.SharedOperation.GetUserJob(userIserial, "");
                    var routeIssue = context.TblAuthJobPermissions.FirstOrDefault(x => x.TblPermission == 225 && x.Tbljob == job);

                    header.RouteIncluded = routeIssue != null;
                }
                else
                {
                    header.RouteIncluded = false;
                }
                try
                {
                    if (header.tblTransactionType == 1)
                    {
                        foreach (var routeCardFabricRow in header.RouteCardFabrics)
                        {
                            routeCardFabricRow.RemainingQty = routeCardFabricRow.Qty;
                        }
                    }
                    else if (header.tblTransactionType == 2 || header.tblTransactionType == 3)
                    {
                        foreach (var routeCardFabricRow in header.RouteCardFabrics)
                        {
                            routeCardFabricRow.OldIserial = routeCardFabricRow.Iserial;
                            routeCardFabricRow.RouteCardHeaderIserial = header.Iserial;
                            routeCardFabricRow.RemainingQty = null;
                            var oldrow = context.RouteCardFabrics.SingleOrDefault(x => x.Iserial == routeCardFabricRow.OldIserial);
                            if (oldrow != null) oldrow.RemainingQty = oldrow.RemainingQty - routeCardFabricRow.Qty;
                        }
                    }

                    else if (header.tblTransactionType == 4 || header.tblTransactionType == 6)// return from PickingLists
                    {
                        foreach (var routeCardFabricRow in header.RouteCardFabrics.ToList())
                        {
                            routeCardFabricRow.OldIserial = routeCardFabricRow.Iserial;


                            routeCardFabricRow.RemainingQty = null;
                            routeCardFabricRow.RouteCardHeaderIserial = header.Iserial;
                            var oldrow = context.RouteCardFabrics.SingleOrDefault(x => x.Iserial == routeCardFabricRow.OldIserial);
                            oldrow.RemainingQty = oldrow.RemainingQty - routeCardFabricRow.Qty;
                            if (header.tblTransactionType == 4)
                            {
                                var transferRow = context.RouteCardFabrics.SingleOrDefault(x => x.Iserial == oldrow.OldIserial);
                                if (transferRow != null)
                                    transferRow.RemainingQty = transferRow.RemainingQty + routeCardFabricRow.Qty;
                            }
                        }
                    }
                    else if (header.tblTransactionType == 5)
                    {
                        foreach (var routeCardFabricRow in header.RouteCardFabrics.ToList())
                        {
                            routeCardFabricRow.OldIserial = routeCardFabricRow.Iserial;
                        }
                    }

                    context.RouteCardHeaders.AddObject(header);
                    context.SaveChanges();
                    foreach (var item in details)
                    {
                        item.RouteCardHeaderIserial = header.Iserial;
                        context.RouteCardDetails.AddObject(item);
                    }
                    context.SaveChanges();

                    savedDetails = details;
                    if (postToAx == 1)
                    {
                        PostRoutCardToAx(header.Iserial, postToAx, transactionGuid, userIserial);
                    }

                    return header;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [OperationContract]
        public List<RouteCardFabric> GetRemRouteQuantity(string vendor, int routeGroup, int route, int transactionType, int transId, string salesorder, string fabricCode,int Skip,int Take)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var details = context.RouteCardFabrics.Include("TblColor").Include("TblColor1").Include("TblSalesOrder1").Include("RouteCardHeader").Where(x => (x.RemainingQty > 0 || x.RemainingQty == null)
                  && x.TblSalesOrder1.SalesOrderCode.Contains(salesorder)
                    && x.RouteCardHeader.RoutGroupID == routeGroup && x.RouteCardHeader.RoutID == route && x.RouteCardHeader.Vendor == vendor && x.ItemId.StartsWith(fabricCode) && x.TblSalesOrder1.SalesOrderCode.StartsWith(salesorder))
                    .OrderByDescending(x=>x.Iserial).Skip(Skip).Take(Take).ToList();

                return details;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public RouteCardHeader UpdateRoutCard(RouteCardHeader header, List<RouteCardDetail> details, int postOrNo, string transactionGuid, int userIserial)
        {
            header.UpdatedBy = userIserial;
            header.LastUpdatedDate = DateTime.Now;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (header.tblTransactionType == 5)
                {
                    var job = Operations.SharedOperation.GetUserJob(userIserial, "");
                    var routeIssue = context.TblAuthJobPermissions.FirstOrDefault(x => x.TblPermission == 225 && x.Tbljob == job);
                    if (routeIssue == null)
                    {
                        header.RouteIncluded = false;
                    }
                    else
                    {
                        header.RouteIncluded = true;
                    }
                }
                else
                {
                    header.RouteIncluded = false;
                }
                try
                {
                    var rch = (from x in context.RouteCardHeaders
                               where x.Iserial == header.Iserial
                               select x).FirstOrDefault();
                    header.Createdby = rch.Createdby;
                    header.PackingTransID = rch.PackingTransID;
                    var det = context.RouteCardDetails.Where(x => x.RouteCardHeaderIserial == header.Iserial);
                    foreach (var item in det)
                    {
                        context.DeleteObject(item);
                    }
                    //try
                    //{
                    //    DeleteAXroute(rch, userIserial);
                    //}
                    //catch (Exception)
                    //{                                            
                    //}             
                    try
                    {
                        context.DeleteObject(rch);

                        context.AddObject("RouteCardHeaders", header);

                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                            //context.DeleteObject(rch);
                            var code = GetMaxRouteCardTransactionID(header.RoutGroupID, header.Direction, header.tblTransactionType);
                            header.TransID = code + 1;
                            context.AddObject("RouteCardHeaders", header);
                            context.SaveChanges();
                    }

                    foreach (var item in details)
                    {
                        item.RouteCardHeaderIserial = header.Iserial;

                        context.RouteCardDetails.AddObject(item);
                    }

                    foreach (var item in details.Where(w => w.Price != 0).GroupBy(w => w.TblSalesOrder))
                    {
                        var salesOrderOperation = context.TblSalesOrderOperations.FirstOrDefault(w => w.TblSalesOrder == item.Key && w.TblRouteGroup == header.TblRouteGroup);
                        if (salesOrderOperation != null)
                        {
                            if (salesOrderOperation.OprCost != 0)
                            {
                                salesOrderOperation.OprCost = Convert.ToSingle(item.Max(w => w.Price));
                            }
                        }
                    }

                    context.SaveChanges();
                    if (postOrNo == 1)
                    {
                        PostRoutCardToAx(header.Iserial, postOrNo, transactionGuid, userIserial);
                    }

                    return header;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteRoutCard(int routeHeader, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var rch = (context.RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeHeader));

                var det = context.RouteCardDetails.Where(x => x.Direction == rch.Direction && x.RoutGroupID == rch.RoutGroupID
                                                              && x.Trans_TransactionHeader == rch.TransID);
                foreach (var item in det)
                {
                    context.DeleteObject(item);
                }
                if (SharedOperation.UseAx())
                {
                    DeleteAXroute(rch, userIserial);
                }
                    context.DeleteObject(rch);
                context.SaveChanges();
            }
        }

        [OperationContract]
        public void RouteCardFabric(int iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var rch = (context.RouteCardFabrics.SingleOrDefault(x => x.Iserial == iserial));
                context.DeleteObject(rch);
                context.SaveChanges();
            }
        }

        [OperationContract]
        public int GetMaxRouteCardTransactionID(int routeGroupId, int direction, int tblTransactionType)
        {
            return Operations.SharedOperation.GetMaxRouteCardTransactionID(routeGroupId, direction, tblTransactionType);
        }

        [OperationContract]
        public List<RouteBomIssueSP_Result> GetRouteBomIssue(string salesOrder, string style, ObservableCollection<string> styleColor, int operation, int direction, ObservableCollection<string> degree)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.CommandTimeout = 0;

                var routegroup = context.TblRouteGroups.Include("TblService1").FirstOrDefault(x => x.Iserial == operation);
                context.RouteBomIssues.MergeOption = MergeOption.NoTracking;

                var mainQuery = context.RouteBomIssueSP(salesOrder).ToList();


                var a = mainQuery.Where(x => x.SalesOrderID == salesOrder && x.Style == style
                    && styleColor.Contains(x.StyleColor) && (x.OperationIserial == operation) && x.BOM_IsLocalProduction != true
                    && (x.ItemType == "Accessories" && direction == 1)).ToList();
                if (direction == 0)
                {
                    if (routegroup.SubFabricProcess == true)
                    {
                        var tempquery =
                            context.RouteCardFabrics.Where(
                                x =>
                                    x.TblSalesOrder1.SalesOrderCode == salesOrder &&
                                    x.RouteCardHeader.RoutGroupID == routegroup.Iserial);

                        var rrrr = mainQuery.Where(x => x.SalesOrderID == salesOrder && x.Style == style
                                                                   && x.StyleColor.Contains(x.StyleColor) && x.Dyed == true
                                                                  && (x.ItemType == "Accessories" && direction == 0)).ToList();
                        var tradeagreement = context.TblTradeAgreementDetails.Where(
                                x => (x.ItemCode == routegroup.TblService && x.ItemType == "Service")).Min(x => x.Price);
                        var tblsalesorderbom = context.BOMs.Include("TblBOMSizes")
                                .Include("TblBOMStyleColors.Tblcolor")
                                .Include("TblBOMStyleColors.Tblcolor1")
                                .Include("TblBOMStyleColors.Tblcolor2")
                                .Include("TblSalesOrder1.TblSalesOrderColors")
                                .Include("TblSalesOrder1.TblStyle1")
                                .Where(x => x.Dyed == true && x.BOM_FabricType == "Accessories" && x.TblSalesOrder1.SalesOrderCode == salesOrder
                                         && x.TblSalesOrder1.Status == 1 && x.TblSalesOrder1.SalesOrderType == 2).ToList();

                        if (routegroup.SubFabricProcess == true)
                        {
                            foreach (var variable in rrrr)
                            {
                                var total = tempquery.Where(x => x.FabricColor == variable.FabricColorIserial && x.NewFabricColor == variable.DyedColor && x.ItemId == variable.ItemId && x.StyleColor == variable.StyleColorCode).Sum(x => x.Qty);
                                variable.Total = total;

                                if (!a.Any(x => x.FabricColorIserial == variable.FabricColorIserial && x.DyedColor == variable.DyedColor && x.ItemId == variable.ItemId && x.StyleColorCode == variable.StyleColorCode))
                                {
                                    a.Add(variable);
                                }
                            }
                        }
                        foreach (var bom in tblsalesorderbom)
                        {
                            double? total = 0;
                            foreach (var variable in bom.TblBOMStyleColors.Where(x => x.DyedColor != null && styleColor.Contains(x.TblColor1.Code)))
                            {
                                if (bom.BOM_FabricType.StartsWith("Acc"))
                                {
                                    var fabric =
                         context.tbl_AccessoryAttributesHeader.FirstOrDefault(x => x.Iserial == bom.BOM_Fabric)
                             .Code;
                                    total = tempquery.Where(x => x.FabricColor == variable.FabricColor && x.NewFabricColor == variable.DyedColor && x.ItemId == fabric && x.StyleColor == variable.StyleColor).Sum(x => x.Qty);
                                }
                                else
                                {
                                    var fabric =
                              context.tbl_FabricAttriputes.FirstOrDefault(x => x.Iserial == bom.BOM_Fabric)
                                  .FabricID;
                                    total = tempquery.Where(x => x.FabricColor == variable.FabricColor && x.NewFabricColor == variable.DyedColor && x.ItemId == fabric && x.StyleColor == variable.StyleColor).Sum(x => x.Qty);
                                }

                                a.Add(new RouteBomIssueSP_Result()
                                {
                                    BOM_FabricType = "Service",
                                    Brand_Ename = bom.TblSalesOrder1.TblStyle1.Brand,
                                    FabricColor = variable.TblColor.Code,
                                    FabricColorIserial = variable.TblColor.Iserial,
                                    UnitID = "KG",
                                    StyleColor = variable.TblColor1.Code,
                                    StyleColorCode = variable.TblColor1.Iserial,
                                    DyedColor = variable.TblColor2.Iserial,
                                    DyedColorCode = variable.TblColor2.Code,
                                    ItemType = "Service",
                                    ItemId = routegroup.TblService1.Code,
                                    CostPerUnit = tradeagreement,
                                    MaterialUsage = bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                    Total = total,
                                    SalesOrderID = salesOrder,
                                    SalesOrderIserial = bom.TblSalesOrder,
                                    Dyed = true,
                                    Style = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                    StyleCode = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                });
                            }
                        }
                    }
                }
                else
                {
                    var rrrr = mainQuery.Where(x => x.SalesOrderID == salesOrder && x.Style == style
                                                                 && styleColor.Contains(x.StyleColor) && (x.Dyed == true || x.BOM_IsLocalProduction == true)
                                                                 && (x.ItemType == "Accessories" && direction == 1)).ToList();
                    if (rrrr.Any(x => x.Dyed == true))
                    {
                        var service = context.TblServices.FirstOrDefault(x => x.Iserial == 14);
                        var tradeagreement =
                            context.TblTradeAgreementDetails.Where(
                                x => (x.ItemCode == service.Iserial && x.ItemType == "Service")).Min(x => x.Price);
                        var tblsalesorderbom = context.BOMs.Include("TblBOMSizes")
                                .Include("TblBOMStyleColors.TblColor")
                                .Include("TblBOMStyleColors.TblColor1")
                                .Include("TblBOMStyleColors.TblColor2")
                                .Include("TblSalesOrder1.TblSalesOrderColors")
                                .Include("TblSalesOrder1.TblStyle1")
                                .Where(x => x.Dyed == true && x.BOM_FabricType == "Accessories" &&
                                        x.TblSalesOrder1.SalesOrderCode == salesOrder &&
                                        x.TblSalesOrder1.Status == 1 &&
                                        x.TblSalesOrder1.SalesOrderType == 2).ToList();
                        if (routegroup.SubFabricProcess == true)
                        {
                            a.AddRange(rrrr);
                        }
                        if (routegroup.SubFabricProcess == false)
                        {
                            foreach (var bom in tblsalesorderbom)
                            {
                                foreach (var variable in bom.TblBOMStyleColors.Where(x => x.DyedColor != null && styleColor.Contains(x.TblColor1.Code)))
                                {
                                    a.Add(new RouteBomIssueSP_Result()
                                    {
                                        BOM_FabricType = "Service",
                                        Brand_Ename = bom.TblSalesOrder1.TblStyle1.Brand,
                                        FabricColor = variable.TblColor.Code,
                                        FabricColorIserial = variable.TblColor.Iserial,
                                        StyleColor = variable.TblColor1.Code,
                                        StyleColorCode = variable.TblColor1.Iserial,
                                        DyedColor = variable.TblColor2.Iserial,
                                        DyedColorCode = variable.TblColor2.Code,
                                        ItemType = "Service",
                                        ItemId = service.Code,
                                        CostPerUnit = tradeagreement,
                                        MaterialUsage = bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                        Total = bom.TblSalesOrder1.TblSalesOrderColors.FirstOrDefault(x => x.TblColor == variable.StyleColor).Total *
                                            bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                        UnitID = "KG",
                                        SalesOrderID = salesOrder,
                                        SalesOrderIserial = bom.TblSalesOrder,
                                        Dyed = true,
                                        Style = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                        StyleCode = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                    });
                                }
                            }
                        }
                    }
                    else if (rrrr.Any(x => x.BOM_IsLocalProduction))
                    {
                        var service = routegroup.TblService1;
                        var tradeagreement =
                            context.TblTradeAgreementDetails.Where(
                                x => (x.ItemCode == service.Iserial && x.ItemType == "Service")).Min(x => x.Price);
                        var tblsalesorderbom = context.BOMs.Include("TblBOMSizes").Include("BomFabricBoms")
                                .Include("TblBOMStyleColors.TblColor")
                                .Include("TblBOMStyleColors.TblColor1")
                                .Include("TblBOMStyleColors.TblColor2")
                                .Include("TblSalesOrder1.TblSalesOrderColors")
                                .Include("TblSalesOrder1.TblStyle1")
                                .Where(x => x.BOM_IsLocalProduction && x.BOM_FabricType == "Accessories" &&
                                        x.TblSalesOrder1.SalesOrderCode == salesOrder &&
                                        x.TblSalesOrder1.Status == 1 &&
                                        x.TblSalesOrder1.SalesOrderType == 2).ToList();

                        foreach (var bom in tblsalesorderbom)
                        {
                            foreach (var variable in bom.TblBOMStyleColors.Where(x => styleColor.Contains(x.TblColor1.Code)))
                            {
                                a.Add(new RouteBomIssueSP_Result()
                                {
                                    BOM_FabricType = "Service",
                                    Brand_Ename = bom.TblSalesOrder1.TblStyle1.Brand,
                                    FabricColor = variable.TblColor.Code,
                                    FabricColorIserial = variable.TblColor.Iserial,
                                    StyleColor = variable.TblColor1.Code,
                                    StyleColorCode = variable.TblColor1.Iserial,

                                    ItemType = "Service",
                                    ItemId = service.Code,
                                    CostPerUnit = tradeagreement,
                                    MaterialUsage = bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                    Total = bom.TblSalesOrder1.TblSalesOrderColors.FirstOrDefault(x => x.TblColor == variable.StyleColor).Total *
                                        bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                    UnitID = "KG",
                                    SalesOrderID = salesOrder,
                                    SalesOrderIserial = bom.TblSalesOrder,

                                    Style = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                    StyleCode = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                });

                                foreach (var VARIABLE in bom.BomFabricBoms.Where(x => x.BOM_FabricRout == operation))
                                {
                                    var item =
                                        context.FabricAccSearches.FirstOrDefault(
                                            x => x.Iserial == VARIABLE.Item && x.ItemGroup == VARIABLE.ItemType);

                                    a.Add(new RouteBomIssueSP_Result()
                                    {
                                        BOM_FabricType = VARIABLE.ItemType,
                                        Brand_Ename = bom.TblSalesOrder1.TblStyle1.Brand,
                                        FabricColor = variable.TblColor.Code,
                                        FabricColorIserial = variable.TblColor.Iserial,
                                        StyleColor = variable.TblColor1.Code,
                                        StyleColorCode = variable.TblColor1.Iserial,
                                        ItemType = VARIABLE.ItemType,
                                        ItemId = item.Code,
                                        CostPerUnit = tradeagreement,
                                        MaterialUsage = bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                        Total = bom.TblSalesOrder1.TblSalesOrderColors.FirstOrDefault(x => x.TblColor == variable.StyleColor).Total *
                                        bom.TblBOMSizes.Min(w => w.MaterialUsage),
                                        UnitID = item.Unit,
                                        SalesOrderID = salesOrder,
                                        SalesOrderIserial = bom.TblSalesOrder,
                                        Style = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                        StyleCode = bom.TblSalesOrder1.TblStyle1.StyleCode,
                                    });
                                }
                            }
                        }
                    }
                }
                return a;
            }
        }
    }
}