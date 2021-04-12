using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Dto_s;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        private string GetMaxSalesorder(int style, int salesOrderType)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNoString = "";
                try
                {
                    var serialNo =
             context.TblSalesOrders.Where(
                 x => x.TblStyle == style && x.SalesOrderType == salesOrderType)
                 .Select(x => x.SerialNo).Cast<int?>().Max();

                    if (serialNo != null)
                    {
                        serialNo++;

                        var serial = (int)serialNo;
                        serialNoString = serial.ToString("000");
                    }
                    else
                    {
                        serialNoString = "001";
                    }
                }
                catch (Exception)
                {
                    serialNoString = "001";
                }

                return serialNoString;
            }
        }


        [OperationContract]
        private List<TblSalesOrder> GetTblSalesOrder(int skip, int take, int style, int salesOrderType, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<TBLsupplier> SupplierList, out List<int> ContainColors)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrder> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblStyle ==(@style0) and it.SalesOrderType==(@SalesOrderType0)";
                    valuesObjects.Add("style0", style);
                    valuesObjects.Add("SalesOrderType0", salesOrderType);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrders.Include("TblStyleTNAHeader1.TblLkpSeason1").Include("TblRequestForSample1").Include("TblApprovalStatu").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesOrders.Count(x => x.TblStyle == style && x.SalesOrderType == salesOrderType);
                    query = context.TblSalesOrders.Include("TblStyleTNAHeader1.TblLkpSeason1").Include("TblRequestForSample1").Include("TblApprovalStatu").OrderBy(sort).Where(x => x.TblStyle == style && x.SalesOrderType == salesOrderType).Skip(skip).Take(take);
                }
                var listOfSuppliers = query.Select(x => x.TblSupplier).Where(x => x > 0).Distinct().ToArray();
                var listOfTblSupplierContract = query.Select(x => x.TblSupplierContract).Where(x => x > 0).Distinct().ToArray();

                var listofIserials = query.Select(x => x.Iserial).Distinct().ToArray();
                using (var entity = new ccnewEntities())
                {
                    entity.TBLsuppliers.MergeOption = MergeOption.NoTracking;
                    SupplierList = listOfSuppliers.Any() ? entity.TBLsuppliers.Where(x => listOfSuppliers.Any(l => x.Iserial == l)).ToList() : null;
                    if (listOfTblSupplierContract.Any())
                    {
                        SupplierList.AddRange(entity.TBLsuppliers.Where(x => listOfTblSupplierContract.Any(l => x.Iserial == l)).ToList());
                    }
                }
                ContainColors = context.TblSalesOrderColors.Where(x => listofIserials.Contains(x.TblSalesOrder)).Select(x => x.TblSalesOrder).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        public TblSalesOrder UpdateOrInsertTblSalesOrder(TblSalesOrder newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var styleCode = context.TblStyles.FirstOrDefault(x => x.Iserial == newRow.TblStyle);
                var serial = GetMaxSalesorder(newRow.TblStyle, newRow.SalesOrderType);

                if (save)
                {
                    //if (newRow.SalesOrderType== (int)Enums.SalesOrderType.RetailPo)
                    //{
                    //    var brandsectionlink = context.TblLkpBrandSectionLinks.FirstOrDefault(w => w.TblBrand == styleCode.Brand && w.TblLkpBrandSection == styleCode.TblLkpBrandSection);
                    //    if (brandsectionlink != null)
                    //    {
                    //        if (brandsectionlink.ContractRequired && newRow.TblContractHeader == null)
                    //        {
                    //            throw new InvalidOperationException("Contract Required");

                    //        }
                    //    }
                    //}
                    if (newRow.TblSupplier==0)
                    {
                        newRow.TblSupplier = 10000021;
                    }
                    if (newRow.Iserial!=0)
                    {
                        newRow.Iserial = 0;
                    }
                 

                    //if (newRow.SalesOrderCode != null && newRow.SalesOrderCode.Length <= styleCode.Trim().Length + 4)
                    //{
                    //    newRow.SalesOrderCode = styleCode.Trim() + "-" + serial;

                    //}
                    //else
                    //{
                    //    if (newRow.SalesOrderCode != null && newRow.SalesOrderCode.Length <= styleCode.Trim().Length + 4)
                    //    {
                    //        newRow.SalesOrderCode = styleCode.Trim() + "-" + serial;

                    //        if (newRow.SalesOrderType == (decimal)Enums.SalesOrderType.AdvancedSampleRequest)
                    //        {
                    //            newRow.SalesOrderCode = styleCode.Trim() + "-" + "S" + "-" + serial;
                    //        }
                    //    }
                    //}

                    if (newRow.SalesOrderType == (decimal)Enums.SalesOrderType.AdvancedSampleRequest)
                    {
                        newRow.SalesOrderCode = styleCode.StyleCode.Trim() + "-" + "S" + "-" + serial;
                    }
                    else
                    {
                        if (newRow.SalesOrderCode == null)
                        {
                            newRow.SalesOrderCode = styleCode.StyleCode.Trim() + "-" + serial;
                        }
                    }

                    newRow.CreationDate = DateTime.Now;
                    newRow.LastUpdatedBy = null;
                    newRow.LastUpdatedDate = null;
                    newRow.SerialNo = serial;
                    newRow.TblStyle1 = null;

                    context.TblSalesOrders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSalesOrders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        var updatedStrings = GenericUpdate(oldRow, newRow, context);
                        //if (updatedStrings.Count() > 1 || (updatedStrings.Count() == 1 && updatedStrings.FirstOrDefault() != "LastUpdatedDate"))
                        //{
                        //    string code = newRow.SalesOrderCode;
                        //    newRow.SalesOrderCode = code + serial;
                        //    newRow.LastUpdatedDate = DateTime.Now;
                        //    newRow.LastUpdatedBy = LoggedUserInfo.WfmUserName;
                        //    GenericUpdate(oldRow, newRow, context);
                        //}
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblSalesOrder DeleteTblSalesOrder(TblSalesOrder row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private List<TblSalesOrderColor> GetTblSalesOrderColor(int salesorder)
        {
            
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSalesOrderColors.Include("TblSalesOrderSizeRatios").Include("TblColor1").Include("TblSalesOrderColorTheme1").Where(x => x.TblSalesOrder == salesorder).OrderBy(x => x.TblColor);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderColor UpdateOrInsertTblSalesOrderColor(TblSalesOrderColor newRow, bool save, int index, out int outindex)
        {
                
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {                    
                    context.TblSalesOrderColors.AddObject(newRow);
                    // var SalesOrder= context.TblSalesOrders.FirstOrDefault(x => x.Iserial == newRow.TblSalesOrder);
                    // var PaymentScheduleDetail = context.TblPaymentScheduleDetails.Where(x => x.tbl.TblPaymentSchedule == SalesOrder.TblPaymentSchedule);
                }
                else
                {
                    var oldRow = (from e in context.TblSalesOrderColors.Include("TblColor1").Include("TblSalesOrder1").Include("TblSalesOrder1.TblStyle1.TblLkpSeason1")
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                        foreach (var row in newRow.TblSalesOrderSizeRatios.ToList())
                        {
                            var oldColorRow = (from e in context.TblSalesOrderSizeRatios
                                               where e.Iserial == row.Iserial || (e.Size == row.Size && e.TblSalesOrderColor == row.TblSalesOrderColor)
                                               select e).SingleOrDefault();
                            if (oldColorRow != null)
                            {
                                row.TblSalesOrderColor = newRow.Iserial;

                                row.TblSalesOrderColor1 = null;
                                GenericUpdate(oldColorRow, row, context);

                                if (oldRow.TblSalesOrder1.Status == 1 && oldRow.TblSalesOrder1.SalesOrderType == 1)
                                {
                                    var group4 = FindOrCreate("TblGroup4",
                                                    new GenericTable
                                                    {
                                                        Iserial = 0,
                                                        Aname = oldRow.TblSalesOrder1.TblStyle1.TblLkpSeason1.Aname,
                                                        Code = oldRow.TblSalesOrder1.TblStyle1.TblLkpSeason1.Code,
                                                        Ename = oldRow.TblSalesOrder1.TblStyle1.TblLkpSeason1.Ename
                                                    });
                                    using (var ccnewcontext = new ccnewEntities())
                                    {
                                        var brandSectionLink =
                                        context.TblLkpBrandSectionLinks.FirstOrDefault(x => x.TblBrand == oldRow.TblSalesOrder1.TblStyle1.Brand && x.TblLkpBrandSection == oldRow.TblSalesOrder1.TblStyle1.TblLkpBrandSection);

                                        var tblitemdownloadDef = ccnewcontext.TblItemDownLoadDefs.FirstOrDefault(
                                         x => x.Code == brandSectionLink.TblItemDownLoadDef);

                                        var salesordertheme =
                                            context.TblSalesOrderColorThemes.FirstOrDefault(
                                                x => x.Iserial == newRow.TblSalesOrderColorTheme);

                                        var tblstylecolorgroup1 =
                                            ccnewcontext.TblStyleColorGroup1.FirstOrDefault(
                                                x =>
                                                    x.CODE == salesordertheme.Code &&
                                                    x.TblGroup4 == group4 &&
                                                    tblitemdownloadDef.iserial == x.TblItemDownloadDef);
                                        if (tblstylecolorgroup1 == null)
                                        {
                                            tblstylecolorgroup1 = new TblStyleColorGroup1
                                            {
                                                ISERIAL = salesordertheme.Iserial,
                                                CODE = salesordertheme.Code,
                                                ANAME = salesordertheme.Aname,
                                                ENAME = salesordertheme.Ename,
                                                TblGroup4 = group4,
                                                TblItemDownloadDef = tblitemdownloadDef.iserial,
                                            };
                                            ccnewcontext.TblStyleColorGroup1.AddObject(tblstylecolorgroup1);
                                            ccnewcontext.SaveChanges();
                                        }
                                        var retailcolor = FindOrCreate("tblcolor",
                                         new GenericTable
                                         {
                                             Iserial = 0,
                                             Aname = oldRow.TblColor1.Aname,
                                             Code = oldRow.TblColor1.Code,
                                             Ename = oldRow.TblColor1.Ename
                                         });
                                        var tblStyleColorGroupLinks = ccnewcontext.TblStyleColorGroupLinks.FirstOrDefault(x => x.Style == oldRow.TblSalesOrder1.TblStyle1.RefStyleCode && x.TblColor == retailcolor);

                                        if (tblStyleColorGroupLinks == null)
                                        {
                                            tblStyleColorGroupLinks = new TblStyleColorGroupLink
                                            {
                                                Style = oldRow.TblSalesOrder1.TblStyle1.RefStyleCode,
                                                TblColor = retailcolor,
                                                TblStyleColorGroup1 = tblstylecolorgroup1.ISERIAL,
                                                Notes = newRow.Notes
                                            };
                                            ccnewcontext.TblStyleColorGroupLinks.AddObject(tblStyleColorGroupLinks);
                                            ccnewcontext.SaveChanges();
                                        }
                                        else
                                        {
                                            tblStyleColorGroupLinks.TblStyleColorGroup1 = tblstylecolorgroup1.ISERIAL;
                                            tblStyleColorGroupLinks.Notes = newRow.Notes;
                                        }
                                        ccnewcontext.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                row.TblSalesOrderColor = newRow.Iserial;

                                row.TblSalesOrderColor1 = null;
                                context.TblSalesOrderSizeRatios.AddObject(row);
                            }
                        }
                    }
                }

                context.SaveChanges();
                return newRow;
            }
        }



        [OperationContract]
        private int DeleteTblSalesOrderColor(TblSalesOrderColor row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderColors
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblSalesOrder> GetSalesOrders(int? skip, int? take, int styleHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (skip != null && take != null)
                    return context.TblSalesOrders
                        .Where(x => x.TblStyle == styleHeader)
                        .OrderByDescending(x => x.CreationDate)
                        .Skip((int)skip)
                        .Take((int)take)
                        .ToList();
                return context.TblSalesOrders
                    .Where(x => x.TblStyle == styleHeader)
                    .OrderByDescending(x => x.CreationDate)
                    .ToList();
            }
        }

        [OperationContract]
        private List<TblSalesOrder> GetPoNotLinkToSalesorder(int style,out DateTime? TNADeliveryDate)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var poList = context.TblSalesOrders.Where(x => x.TblStyle == style
                                                                         && x.SalesOrderType == 2 && x.PoRef != null).Select(x => x.PoRef).ToList().Cast<int>();

                var salesOrdersNotLinked = context.TblSalesOrders.Include("TblSalesOrderOperations").Include("BOMs").Include("BOMs.TblBOMSizes").Include("BOMs.TblBOMStyleColors").Include("TblSalesOrderColors").Include("TblSalesOrderColors.TblSalesOrderSizeRatios")
                    .Where(x => x.TblStyle == style && x.SalesOrderType == 1
                    && (poList.All(p => x.Iserial != p))).ToList();

                TNADeliveryDate = null;
                if (salesOrdersNotLinked.Count <= 0)
                {
                     TNADeliveryDate = context.TblSeasonalMasterLists.Where(c => c.TblStyle == style).Min(x => x.DelivaryDate).Value;
                }
                return salesOrdersNotLinked;
            }
        }

        [OperationContract]
        private List<TblSalesOrder> GetRfqNotLinkToPo(int style)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                const int retailPoStatus = (int)Enums.SalesOrderType.RetailPo;
                const int rfqStatus = (int)Enums.SalesOrderType.RFQ;
                var poList = context.TblSalesOrders.Where(x => x.TblStyle == style && x.SalesOrderType == retailPoStatus && x.PoRef != null).Select(x => x.PoRef).ToList().Cast<int>();
                var salesOrdersNotLinked = context.TblSalesOrders.Include("TblSalesOrderOperations").Include("BOMs").Include("BOMs.TblBOMSizes").Include("BOMs.TblBOMStyleColors").Include("TblSalesOrderColors").Include("TblSalesOrderColors.TblSalesOrderSizeRatios")
                    .Where(x => x.TblStyle == style && x.SalesOrderType == rfqStatus
                    && (poList.All(p => x.Iserial != p))).ToList();

                return salesOrdersNotLinked;
            }
        }

        [OperationContract]
        private TblSalesOrder GetFullSalesOrderData(int iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var salesOrdersNotLinked =
                    context.TblSalesOrders.Include("TblSalesOrderOperations").Include("BOMs").Include("BOMs.TblBOMSizes").Include("BOMs.TblBOMStyleColors").Include("TblSalesOrderColors").Include("TblSalesOrderColors.TblSalesOrderSizeRatios").FirstOrDefault(x => x.Iserial == iserial);
                return salesOrdersNotLinked;
            }
        }

        [OperationContract]
        private List<TblSalesOrder> GetSalesOrderForCopyBom(int skip, int take, int salesOrderType, string sort, string filter,
        Dictionary<string, object> valuesObjects, out int fullCount, out List<TBLsupplier> SupplierList, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrder> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrders.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        context.TblSalesOrders.Include("TblSalesOrderOperations.TblRouteGroup").Include("BOMs.BOM_CalcMethod").Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount = context.TblSalesOrders.Count(x => x.SalesOrderType == salesOrderType && x.Status == 1);
                    query =
                        context.TblSalesOrders.Include("TblSalesOrderOperations.TblRouteGroup").Include("BOMs.BOM_CalcMethod").OrderBy(sort)

                            .Skip(skip)
                            .Take(take);
                }
                var listOfSuppliers = query.Select(x => x.TblSupplier).Where(x => x > 0).Distinct().ToArray();

                using (var entity = new ccnewEntities())
                {
                    entity.TBLsuppliers.MergeOption = MergeOption.NoTracking;
                    SupplierList = listOfSuppliers.Any()
                        ? entity.TBLsuppliers.Where(x => listOfSuppliers.Any(l => x.Iserial == l)).ToList()
                        : null;
                }
                var itemsquery = new List<ItemsDto>();
                foreach (var bomQuery in query.ToList())
                {
                    var fabricsIserial = bomQuery.BOMs.Where(x => x.BOM_FabricType != "Accessories").Select(x => x.BOM_Fabric);

                    var accIserial = bomQuery.BOMs.Where(x => x.BOM_FabricType == "Accessories").Select(x => x.BOM_Fabric);
                    //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                    itemsquery.AddRange((from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                         where (fabricsIserial.Any(l => x.Iserial == l))

                                         select new ItemsDto
                                         {
                                             Iserial = x.Iserial,
                                             Code = x.FabricID,
                                             Name = x.FabricDescription,
                                             ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                             Unit = x.tbl_lkp_UoM.Ename
                                         }).Take(20).ToList());

                    itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

                                         where (accIserial.Any(l => x.Iserial == l))
                                         select new ItemsDto
                                         {
                                             Iserial = x.Iserial,
                                             Code = x.Code,
                                             Name = x.Descreption,
                                             ItemGroup = "Accessories"
                                             ,
                                             IsSizeIncluded = x.IsSizeIncludedInHeader,

                                             Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                         }).Take(20).ToList());
                }

                itemsList = itemsquery;
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblSeasonalMasterList> GetSeasonalMasterListNotLinkedToSalesorder(int salesOrder)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //               fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));

                var style = context.TblSalesOrders.FirstOrDefault(x => x.Iserial == salesOrder);
                var salesOrderColorsList = context.TblSalesOrderColors.Where(x => x.TblSalesOrder1.TblStyle1.Iserial == style.TblStyle && x.TblSalesOrder1.IsPlannedOrder == true && x.TblSalesOrder1.SalesOrderType == style.SalesOrderType);

                var salesOrdersNotLinked = context.TblSeasonalMasterLists.Include("TblStyle1.TblStyleTNAHeaders").Include("TblSeasonalMasterListDetails").Include("TblColor1").Include("TblSalesOrderColorTheme1").Where(x => x.TblStyle == style.TblStyle && salesOrderColorsList.All(c => c.TblColor != x.TblColor));

                return salesOrdersNotLinked.ToList();
            }
        }



        private List<TblSeasonalMasterList> GetSeasonalMasterListNotLinkedToSalesorderByStyle(int style, int salesordertype)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //               fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));

                var salesOrderColorsList = context.TblSalesOrderColors.Where(x => x.TblSalesOrder1.TblStyle1.Iserial == style && x.TblSalesOrder1.IsPlannedOrder == true && x.TblSalesOrder1.SalesOrderType == salesordertype);

                var salesOrdersNotLinked = context.TblSeasonalMasterLists.Include("TblStyle1.TblStyleTNAHeaders").Include("TblSeasonalMasterListDetails").Include("TblColor1").Include("TblSalesOrderColorTheme1").Where(x => x.TblStyle == style && salesOrderColorsList.All(c => c.TblColor != x.TblColor));

                return salesOrdersNotLinked.ToList();
            }
        }
        [OperationContract]
        private List<TblSeasonalMasterList> GetSeasonalMasterListNotLinkedToStyleTnaByStyle(int style)
        {
            using (var context = new WorkFlowManagerDBEntities())
            { 
                var salesOrderColorsList = context.TblStyleTNAColorDetails.Where(x => x.TblStyleTNAHeader1.TblStyle1.Iserial == style );

                var salesOrdersNotLinked = context.TblSeasonalMasterLists.Include("TblStyle1.TblStyleTNAHeaders").Include("TblSeasonalMasterListDetails").Include("TblColor1").Include("TblSalesOrderColorTheme1").Where(x => x.TblStyle == style && salesOrderColorsList.All(c => c.TblColor != x.TblColor));

                return salesOrdersNotLinked.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblSalesOrderColor> GetSalesOrderColors(int? skip, int? take, int route, int salesOrder, int operation, int direction, bool CuttingQty, int userIserial, out bool MarkerExist, out List<RouteCardDetail> RouteCardDetailList, out float Cost)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                Cost = 0;
                MarkerExist = false;
                if (operation != 0)
                {
                    var routeIncluded = true;
                    var job = GetUserJob(userIserial, "");
                    var routeIssue = context.TblAuthJobPermissions.FirstOrDefault(x => x.TblPermission == 225 && x.Tbljob == job);

                    if (routeIssue == null)
                    {
                        routeIncluded = false;
                    }
                    else
                    {
                        routeIncluded = true;
                    }

                    var salesOrderOperations = context.TblSalesOrderOperations.Include("TblSalesOrder1").Where(x => x.TblSalesOrder == salesOrder).ToList();
                    var salescode = salesOrderOperations.FirstOrDefault().TblSalesOrder1.SalesOrderCode;
                    MarkerExist =
                        context.tbl_MarkerDetail.Any(
                        x => x.SalesOrder == salescode);
                    try
                    {
                        var lastRowIndex = salesOrderOperations.First(x => x.TblOperation == operation);
                        Cost = lastRowIndex.OprCost;
                        if (route == 5)
                        {
                            if (direction == 1)
                            {
                                direction = 0;
                            }
                            else
                            {
                                direction = 1;
                            }
                        }
                        else
                        {
                            direction = 1;
                        }

                        double? index = salesOrderOperations.First(x => x.TblOperation == operation).RowIndex;

                        if (direction == 1 && route == 5)
                        {
                            var temprow = salesOrderOperations.OrderByDescending(x => x.RowIndex).FirstOrDefault(x => x.RowIndex < index);
                            if (temprow != null)
                            {
                                lastRowIndex = temprow;
                            }
                        }
                        if (route == 5)
                        {
                            RouteCardDetailList = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == lastRowIndex.TblOperation && x.RouteCardHeader.Direction == direction && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                            var colorlist = RouteCardDetailList.Select(x => x.TblColor).Distinct();
                            var routecardOut = new List<RouteCardDetail>();
                            var routecardIn = new List<RouteCardDetail>();
                            if (lastRowIndex.TblOperation == operation)
                            {
                                if (!CuttingQty)
                                {
                                    routecardIn = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == lastRowIndex.TblOperation && x.RouteCardHeader.Direction == 0 && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                                }
                                else
                                {
                                    var operationtemp =
                                        context.BOMs.Where(x => x.TblSalesOrder == salesOrder)
                                            .FirstOrDefault(x => x.BOM_IsMainFabric)
                                            .BOM_FabricRout;

                                    routecardIn = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == operationtemp && x.Direction == 1 && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                                }
                                routecardOut = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == lastRowIndex.TblOperation && x.RouteCardHeader.RouteType == 5 && x.Direction == 1 && x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                            }
                            else
                            {
                                routecardOut =
                                     context.RouteCardDetails.Where(
                                         x =>
                                             x.RoutGroupID == operation && x.Direction == 0 && x.RouteCardHeader.RouteType == 5 &&
                                             x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true)
                                         .OrderByDescending(x => x.RouteCardHeader.DocDate)
                                         .ToList();

                                if (!CuttingQty)
                                {
                                    routecardIn = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == lastRowIndex.TblOperation && x.Direction == 1 && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                                }
                                else
                                {
                                    var operationtemp =
                                        context.BOMs.Where(x => x.TblSalesOrder == salesOrder)
                                            .FirstOrDefault(x => x.BOM_IsMainFabric)
                                            .BOM_FabricRout;

                                    routecardIn = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == operationtemp && x.Direction == 1 && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder && x.RouteCardHeader.RouteIncluded == true).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                                }
                            }
                            foreach (var color in colorlist)
                            {
                                foreach (var variable in RouteCardDetailList.Where(x => x.TblColor == color))
                                {
                                    if (routeIncluded)
                                    {
                                        variable.SizeQuantity =
                                     routecardIn.Where(x => x.Size == variable.Size && x.TblColor == color && x.Degree == variable.Degree).Sum(x => x.SizeQuantity) -
                                     routecardOut.Where(x => x.Size == variable.Size && x.TblColor == color && x.Degree == variable.Degree).Sum(x => x.SizeQuantity);
                                    }
                                    else
                                    {
                                        variable.SizeQuantity =
                                            routecardIn.Where(
                                                x =>
                                                    x.Size == variable.Size && x.TblColor == color &&
                                                    x.Degree == variable.Degree).Sum(x => x.SizeQuantity);
                                    }
                                }
                            }
                        }
                        else
                        {
                            RouteCardDetailList = context.RouteCardDetails.Where(x => x.RouteCardHeader.RoutGroupID == operation && x.RouteCardHeader.Direction == direction && x.RouteCardHeader.RouteType == 5 && x.TblSalesOrder == salesOrder).OrderByDescending(x => x.RouteCardHeader.DocDate).ToList();
                        }
                    }
                    catch (Exception)
                    {
                        RouteCardDetailList = null;
                    }
                }
                else
                {
                    RouteCardDetailList = null;
                    Cost = 0;
                }

                if (skip != null && take != null)
                    return context.TblSalesOrderColors.Include("TblColor1").Include("TblSalesOrderSizeRatios").Include("TblSalesOrder1.TblStyle1")
                        .Where(x => x.TblSalesOrder == salesOrder)
                        .OrderBy(x => x.Iserial)
                        .Skip((int)skip)
                        .Take((int)take)
                        .ToList();
                return context.TblSalesOrderColors.Include("TblColor1").Include("TblSalesOrderSizeRatios").Include("TblSalesOrder1.TblStyle1")
                    .Where(x => x.TblSalesOrder == salesOrder)
                    .OrderBy(x => x.Iserial)
                    .ToList();
            }
        }

        [OperationContract]
        public List<TblRequestForSample> GetSamplesRequestBySupplier(int tblStyle, int tblSupplier)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblRequestForSamples.Where(x => x.TblStyle == tblStyle && x.TblSupplier == tblSupplier && x.TblRequestForSampleStatu.LastEvent == true).ToList();
            }
        }

        [OperationContract]
        private List<TblSalesOrderNote> GetTblSalesOrderNotes(int skip, int take, int TblSalesOrder, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderNote> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblSalesOrder ==(@TblSalesOrder0)";
                    valuesObjects.Add("TblSalesOrder0", TblSalesOrder);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblSalesOrderNotes.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderNotes.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesOrderNotes.Count(x => x.TblSalesOrder == TblSalesOrder);
                    query = context.TblSalesOrderNotes.OrderBy(sort).Where(x => x.TblSalesOrder == TblSalesOrder).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderNote UpdateOrInsertTblSalesOrderNotes(TblSalesOrderNote newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {

                if (save)
                {

                    context.TblSalesOrderNotes.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSalesOrderNotes
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        var updatedStrings = GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblSalesOrderNote DeleteTblSalesOrderNotes(TblSalesOrderNote row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderNotes
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}