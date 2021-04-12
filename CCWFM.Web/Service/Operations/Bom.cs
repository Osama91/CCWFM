using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using System.IO;
using System.Drawing;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<BOM> GetBom(int salesOrder, out List<Vendor> vendorList, out List<ItemsDto> itemsList, out List<int> salesOrdersPendingCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.BOMs.Include("BOM_CalcMethod").Include("TblBOMSizes").Include("TblBOMStyleColors.TblColor").Include("TblBOMStyleColors.TblColor1").Include("TblBOMStyleColors.TblColor2").Include("TblPurchaseOrderDetailBreakDowns.TblPurchaseOrderDetail1.TblPurchaseRequestLinks").Where(x => x.TblSalesOrder == salesOrder);

                var fabricsIserial = query.Where(x => x.BOM_FabricType != "Accessories").Select(x => x.BOM_Fabric);

                var accIserial = query.Where(x => x.BOM_FabricType == "Accessories").Select(x => x.BOM_Fabric);
                var listOfStyles = query.Select(x => x.Iserial);
                salesOrdersPendingCount = context.TblBOMSizeEstimateds.Where(x => listOfStyles.Any(l => x.Bom == l)).Select(x => x.Bom).ToList();

                var vendorListStrings = query.Select(x => x.Vendor);
                vendorList = context.Vendors.Where(x => vendorListStrings.Any(l => x.vendor_code == l)).ToList();

                var itemsquery = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                  where (fabricsIserial.Any(l => x.Iserial == l))
                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).Take(20).ToList();
                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(20).ToList());
                itemsList = itemsquery;
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TechPackBOM> GetTechPackBom(int tblStyle, out List<Vendor> vendorList, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TechPackBOMs
                                        .Include("BOM_CalcMethod")
                                        .Include("BOM_FabricType1")
                                        .Include("TblTechPackBOMSizes")
                                        .Include("TblTechPackBOMStyleColors.TblColor")
                                        .Include("TblTechPackBOMStyleColors.TblColor1")
                                        .Include("TblTechPackBOMStyleColors.TblColor2")
                                        .Where(x => x.TblStyle == tblStyle);

                var fabricsIserial = query.Where(x => x.BOM_FabricType != "Accessories").Select(x => x.BOM_Fabric);

                var accIserial = query.Where(x => x.BOM_FabricType == "Accessories").Select(x => x.BOM_Fabric);
                var listOfStyles = query.Select(x => x.Iserial);


                var vendorListStrings = query.Select(x => x.Vendor);
                vendorList = context.Vendors.Where(x => vendorListStrings.Any(l => x.vendor_code == l)).ToList();

                var itemsquery = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                  where (fabricsIserial.Any(l => x.Iserial == l))
                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).Take(20).ToList();
                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(20).ToList());
                itemsList = itemsquery;
                var I = query.ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private bool ValidateBom(int Iserial, out bool RoutesBug, out bool SizesBug)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var salesorder = context.TblSalesOrders.Include("BOMs.TblBOMSizes").FirstOrDefault(x => x.Iserial == Iserial);

                if (salesorder == null)
                {
                    SizesBug = false;
                    RoutesBug = false;
                    return false;
                }

                if (salesorder.BOMs.Any(x => x.BOM_FabricRout == null))
                {
                    RoutesBug = true;
                    SizesBug = true;
                    return false;
                }

                var Invalid = salesorder.BOMs.Where(t => t.BOM_FabricType == "Accessories" && t.TblBOMSizes.Any(e => e.FabricSize == null));
                if (salesorder.BOMs.Where(t => t.BOM_FabricType == "Accessories").Any(x => x.TblBOMSizes.Any(e => e.FabricSize == null)))
                {
                    SizesBug = true;
                    RoutesBug = true;
                    return false;
                }
                RoutesBug = true;
                SizesBug = true;



                return true;
            }
        }

        [OperationContract]
        private List<BOM> UpdateOrInsertBom(List<BOM> newBomList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (newBomList.Any())
                {
                    var firstBom = newBomList.FirstOrDefault(x => x.BOM_IsMainFabric && x.BOM_FabricType != "Accessories");

                    if (firstBom != null)
                    {
                        var salesOrder = context.TblSalesOrders.FirstOrDefault(x => x.Iserial == firstBom.TblSalesOrder);
                        salesOrder.TblFactoryGroup = firstBom.TblSalesOrder1.TblFactoryGroup;
                        salesOrder.TblComplexityGroup = firstBom.TblSalesOrder1.TblComplexityGroup;
                        salesOrder.TblSalesPerson = firstBom.TblSalesOrder1.TblSalesPerson;
                        var style = context.TblStyles.FirstOrDefault(x => x.Iserial == salesOrder.TblStyle);
                        if (firstBom.BOM_Fabric != null) style.tbl_FabricAttriputes = (int)firstBom.BOM_Fabric;
                    }
                }

                foreach (var row in newBomList)
                {
                    if (row.BOM_FabricType == "Accessories")
                    {
                        row.BOM_IsMainFabric = false;
                    }

                    row.TblSalesOrder1 = null;
                    if (row.Iserial != 0)
                    {
                        var oldRow = (from e in context.BOMs
                                      where e.Iserial == row.Iserial
                                      select e).SingleOrDefault();

                        GenericUpdate(oldRow, row, context);

                        foreach (var newSizeRow in row.TblBOMSizes.ToList())
                        {
                            newSizeRow.BOM1 = null;
                            if (newSizeRow.Iserial != 0)
                            {
                                var oldSizeRow = (from e in context.TblBOMSizes
                                                  where e.Iserial == newSizeRow.Iserial
                                                  select e).SingleOrDefault();
                                GenericUpdate(oldSizeRow, newSizeRow, context);
                            }
                            else
                            {
                                newSizeRow.Bom = oldRow.Iserial;
                                context.TblBOMSizes.AddObject(newSizeRow);
                            }
                        }

                        foreach (var newBomStyleColorRow in row.TblBOMStyleColors.ToList())
                        {
                            newBomStyleColorRow.TblColor = null;
                            newBomStyleColorRow.TblColor1 = null;
                            newBomStyleColorRow.TblColor2 = null;
                            newBomStyleColorRow.BOM1 = null;
                            newBomStyleColorRow.Bom = oldRow.Iserial;
                            if (newBomStyleColorRow.Iserial != 0)
                            {
                                var oldColoreRow = (from e in context.TblBOMStyleColors
                                                    where e.Iserial == newBomStyleColorRow.Iserial
                                                    select e).SingleOrDefault();
                                GenericUpdate(oldColoreRow, newBomStyleColorRow, context);
                            }
                            else
                            {
                                context.TblBOMStyleColors.AddObject(newBomStyleColorRow);
                            }
                        }
                    }
                    else
                    {
                        context.BOMs.AddObject(row);
                    }
                }

                context.SaveChanges();
                var iserials = newBomList.Select(x => x.Iserial);
                var query = context.BOMs.Include("TblBOMSizes").Include("TblBOMStyleColors.TblColor").Include("TblBOMStyleColors.TblColor1").Include("TblBOMStyleColors.TblColor2").Where(x => iserials.Contains(x.Iserial));

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TechPackBOM> UpdateOrInsertTechPackBom(List<TechPackBOM> newBomList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (newBomList.Any())
                {
                    var firstBom = newBomList.FirstOrDefault(x => x.BOM_IsMainFabric && x.BOM_FabricType != "Accessories");

                    if (firstBom != null)
                    {
                        //var salesOrder = context.TblSalesOrders.FirstOrDefault(x => x.Iserial == firstBom.TblSalesOrder);
                        //salesOrder.TblFactoryGroup = firstBom.TblSalesOrder1.TblFactoryGroup;
                        //salesOrder.TblSalesPerson = firstBom.TblSalesOrder1.TblSalesPerson;
                        //var style = context.TblStyles.FirstOrDefault(x => x.Iserial == salesOrder.TblStyle);
                        //if (firstBom.BOM_Fabric != null) style.tbl_FabricAttriputes = (int)firstBom.BOM_Fabric;
                    }
                }

                foreach (var row in newBomList)
                {
                    if (row.BOM_FabricType == "Accessories")
                    {
                        row.BOM_IsMainFabric = false;
                    }

                    if (row.Iserial != 0)
                    {
                        var oldRow = (from e in context.TechPackBOMs
                                      where e.Iserial == row.Iserial
                                      select e).SingleOrDefault();
                        string GallaryLink = "";
                        if (!string.IsNullOrEmpty(oldRow.GallaryLink))
                        {
                            GallaryLink = oldRow.GallaryLink.ToString();
                        }
                        row.CalcMethod = 1; //Static Value
                        GenericUpdate(oldRow, row, context);
                        //Save TechPackBomTrimImage
                        if (row.ImageThumb != null)
                        {
                            SaveImageToPath(row.ImageName, row.ImageThumb);
                            oldRow.GallaryLink = string.Format(@"D:\TechPackBomTrimImages\" + row.ImageName);
                        }
                        else
                        {
                            oldRow.GallaryLink = GallaryLink;
                        }

                        oldRow.ImageThumb = null;

                        foreach (var newSizeRow in row.TblTechPackBOMSizes.ToList())
                        {
                            newSizeRow.TechPackBOM1 = null;
                            if (newSizeRow.Iserial != 0)
                            {
                                var oldSizeRow = (from e in context.TblTechPackBOMSizes
                                                  where e.Iserial == newSizeRow.Iserial
                                                  select e).SingleOrDefault();
                                GenericUpdate(oldSizeRow, newSizeRow, context);
                            }
                            else
                            {
                                newSizeRow.TechPackBOM = oldRow.Iserial;
                                context.TblTechPackBOMSizes.AddObject(newSizeRow);
                            }
                        }

                        foreach (var newBomStyleColorRow in row.TblTechPackBOMStyleColors.ToList())
                        {
                            newBomStyleColorRow.TblColor = null;
                            newBomStyleColorRow.TblColor1 = null;
                            newBomStyleColorRow.TblColor2 = null;
                            newBomStyleColorRow.TechPackBOM1 = null;
                            newBomStyleColorRow.TechPackBOM = oldRow.Iserial;
                            if (newBomStyleColorRow.Iserial != 0)
                            {
                                var oldColoreRow = (from e in context.TblTechPackBOMStyleColors
                                                    where e.Iserial == newBomStyleColorRow.Iserial
                                                    select e).SingleOrDefault();
                                GenericUpdate(oldColoreRow, newBomStyleColorRow, context);
                            }
                            else
                            {
                                context.TblTechPackBOMStyleColors.AddObject(newBomStyleColorRow);
                            }
                        }
                        if (string.IsNullOrEmpty(oldRow.GallaryLink))
                            oldRow.GallaryLink = "";
                    }
                    else
                    {
                        //Save TechPackBomTrimImage
                        if (row.ImageThumb != null)
                        {
                            SaveImageToPath(row.ImageName, row.ImageThumb);
                            row.GallaryLink = string.Format(@"D:\TechPackBomTrimImages\" + row.ImageName);
                        }
                        row.ImageThumb = null;
                        row.CalcMethod = 1; //Static Value
                        if (string.IsNullOrEmpty(row.GallaryLink))
                            row.GallaryLink = "";
                        context.TechPackBOMs.AddObject(row);
                    }
                }

                context.SaveChanges();
                var iserials = newBomList.Select(x => x.Iserial);
                var query = context.TechPackBOMs.Include("TblTechPackBOMSizes").Include("TblTechPackBOMStyleColors.TblColor").Include("TblTechPackBOMStyleColors.TblColor1").Include("TblTechPackBOMStyleColors.TblColor2").Where(x => iserials.Contains(x.Iserial));

                return query.ToList();
            }
        }

        [OperationContract]
        private int DeleteBom(BOM row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.BOMs
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int DeleteTechPackBom(TechPackBOM row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TechPackBOMs
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private bool PostBomToAx(TblSalesOrder row, int userIserial)
        {
            var credential = new NetworkCredential("bcproxy", "around1");
            var success = false;
            try
            {
                if (SharedOperation.UseAx())
                {


                    var ax = new Axapta();//Ready To be Dependent from Ax;
                    TblAuthUser userToLogin;
                    using (var model = new WorkFlowManagerDBEntities())
                    {
                        userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                    }
                    ax.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                    AxaptaObject import = ax.CreateAxaptaObject("NRunIntegration1");
                    var retval = import.Call("run", row.SalesOrderCode);
                    if (retval.ToString() == "")
                    {
                        success = true;
                    }
                    ax.Logoff();
                }
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var temp = context.TblSalesOrders.Include("TblSalesOrderColors.TblSalesOrderSizeRatios").FirstOrDefault(x => x.SalesOrderCode == row.SalesOrderCode && x.SalesOrderType == 2);
                    temp.Status = 1;
                    foreach (var VARIABLE in temp.TblSalesOrderColors)
                    {
                        VARIABLE.ManualCalculationForProduction = true;
                        VARIABLE.TotalForProduction = VARIABLE.Total;
                        var Min =
                         VARIABLE.TblSalesOrderSizeRatios.Where(x => x.ProductionPerSize > 0)
                             .OrderBy(x => x.ProductionPerSize)
                             .FirstOrDefault()
                             .ProductionPerSize;
                        foreach (var roww in VARIABLE.TblSalesOrderSizeRatios)
                        {
                            roww.ProductionPerSizeForProduction = roww.ProductionPerSize;

                            if (VARIABLE.Total > 0 && roww.ProductionPerSize > 0)
                            {
                                if (roww.ProductionPerSize == Min)
                                {
                                    roww.RatioForProduction = 1;
                                }
                                else
                                {
                                    roww.RatioForProduction = (double)roww.ProductionPerSize / Min;
                                }
                            }
                            else
                            {
                                roww.RatioForProduction = 0;
                            }
                        }
                    }

                    temp.IsPostedOnAxapta = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            return success;
        }

        private void CreateEstimatedBom(TblSalesOrder row)
        {
            using (var model = new WorkFlowManagerDBEntities())
            {
                var bom = model.BOMs.Include("BomCosts").Where(w => w.TblSalesOrder == row.Iserial);
                foreach (var bomLine in bom.ToList())
                {
                    float price = 0;
                    // decimal exrate = 0;
                    int? TransAgreementIserial = null;
                    var currency = "";
                    var temp = model.TblTradeAgreementDetails.Where(x => x.ItemCode == bomLine.BOM_Fabric && x.ItemType == bomLine.BOM_FabricType && (x.TblTradeAgreementHeader1.Vendor == bomLine.Vendor || bomLine.Vendor == null));
                    if (temp.Any())
                    {
                        price = temp.FirstOrDefault().Price;
                        currency = temp.FirstOrDefault().CurrencyCode;
                        TransAgreementIserial = temp.FirstOrDefault().Iserial;
                        //  exrate = GetExRateByCurrency(temp.FirstOrDefault().CurrencyCode, "CCM");
                    }

                    var newrow = new BOMEstimated
                    {
                        BOM_DateAdded = bomLine.BOM_DateAdded,
                        BOM_Fabric = bomLine.BOM_Fabric,
                        BOM_FabricType = bomLine.BOM_FabricType,
                        BOM_FabricRout = bomLine.BOM_FabricRout,
                        Dyed = bomLine.Dyed,
                        BOM_IsLocalProduction = bomLine.BOM_IsLocalProduction,
                        TblSalesOrder = bomLine.TblSalesOrder,
                        BOM_IsMainFabric = bomLine.BOM_IsMainFabric,
                        BOM_Notes = bomLine.BOM_Notes,
                        CalcMethod = bomLine.CalcMethod,
                        IsSupplierMaterial = bomLine.IsSupplierMaterial,
                        Bom = bomLine.Iserial
                    };

                    model.BOMEstimateds.AddObject(newrow);
                    model.SaveChanges();
                    var bomsizes = model.TblBOMSizes.Where(w => w.Bom == bomLine.Iserial);
                    foreach (var bomsize in bomsizes.ToList())
                    {
                        var newSize = new TblBOMSizeEstimated
                        {
                            Bom = newrow.Iserial,
                            FabricSize = bomsize.FabricSize,
                            MaterialUsage = bomsize.MaterialUsage,
                            SizeFabricRatio = bomsize.SizeFabricRatio,
                            StyleSize = bomsize.StyleSize,
                        };

                        model.TblBOMSizeEstimateds.AddObject(newSize);
                    }
                    var bomColors = model.TblBOMStyleColors.Where(w => w.Bom == bomLine.Iserial);
                    foreach (var bomColor in bomColors.ToList())
                    {
                        if (temp.Any(x => x.TblColor == bomColor.FabricColor))
                        {
                            price = temp.FirstOrDefault().Price;
                            currency = temp.FirstOrDefault().CurrencyCode;
                            TransAgreementIserial = temp.FirstOrDefault().Iserial;
                        }
                        var newColors = new TblBOMStyleColorEstimated
                        {
                            Bom = newrow.Iserial,
                            StyleColor = bomColor.StyleColor,
                            DyedColor = bomColor.DyedColor,
                            FabricColor = bomColor.FabricColor,
                        };
                        newColors.Price = price;
                        newColors.CurrencyCode = currency;
                        newColors.TblTradeAgreementDetail = TransAgreementIserial;

                        model.TblBOMStyleColorEstimateds.AddObject(newColors);
                    }

                    foreach (var bomCost in bomLine.BomCosts)
                    {
                        var newCost = new BomCostEstimated
                        {
                            BOM_Fabric = bomCost.BOM_Fabric,
                            BOM_FabricType = bomCost.BOM_FabricType,
                            Bom = newrow.Iserial,
                            FabricColor = bomCost.FabricColor,
                            StyleColor = bomCost.StyleColor,
                            StyleSize = bomCost.StyleSize,
                            FabricSize = bomCost.FabricSize,
                            MaterialUsage = bomCost.MaterialUsage,
                        };
                        model.BomCostEstimateds.AddObject(newCost);
                    }
                }
                model.SaveChanges();
            }
        }

        [OperationContract]
        private List<TblFabricBom> GetTblFabricBom(int skip, int take, int baseFabric, string sort, string filter, Dictionary<string, object> valuesObjects, out List<FabricServiceSearch> FabricServiceList, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblFabricBom> query;
                if (filter != null)
                {
                    filter = filter + " and it.BaseFabric ==(@BaseFabric0)";
                    valuesObjects.Add("BaseFabric0", baseFabric);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblFabricBoms.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblFabricBoms.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblFabricBoms.Count(x => x.BaseFabric == baseFabric);
                    query = context.TblFabricBoms.OrderBy(sort).Where(x => x.BaseFabric == baseFabric).Skip(skip).Take(take);
                }
                var iserials = query.Select(x => x.Item);
                FabricServiceList = context.FabricServiceSearches.Where(x => iserials.Any(l => x.Iserial == l)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblFabricBom UpdateOrInsertTblFabricBom(TblFabricBom newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblFabricBoms.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblFabricBoms
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblFabricBom(TblFabricBom row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblFabricBoms
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<BomFabricBom> GetBomFabricBom(int skip, int take, int baseFabric, string sort, string filter, Dictionary<string, object> valuesObjects, out List<FabricServiceSearch> FabricServiceList, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<BomFabricBom> query;
                if (filter != null)
                {
                    filter = filter + " and it.Bom ==(@Bom0)";
                    valuesObjects.Add("Bom0", baseFabric);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.BomFabricBoms.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.BomFabricBoms.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.BomFabricBoms.Count(x => x.Bom == baseFabric);
                    var row = context.BOMs.FirstOrDefault(x => x.Iserial == baseFabric);
                    if (fullCount == 0)
                    {
                        if (row.BOM_Fabric != 0)
                        {
                            if (row.BOM_FabricType != "Accessories")
                            {
                                var fabric = context.tbl_FabricAttriputes.FirstOrDefault(x => x.Iserial == row.BOM_Fabric);
                                var fabricBom = context.TblFabricBoms.Where(x => x.BaseFabric == fabric.Iserial).ToList();
                                foreach (var variable in fabricBom)
                                {
                                    var bomfabricBom = new BomFabricBom
                                    {
                                        Bom = row.Iserial,
                                        Item = variable.Item,
                                        ItemType = variable.ItemType,
                                        MaterialUsage = variable.MaterialUsage
                                    };
                                    context.BomFabricBoms.AddObject(bomfabricBom);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    query = context.BomFabricBoms.OrderBy(sort).Where(x => x.Bom == baseFabric).Skip(skip).Take(take);
                }
                var iserials = query.Select(x => x.Item);
                FabricServiceList = context.FabricServiceSearches.Where(x => iserials.Any(l => x.Iserial == l)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private BomFabricBom UpdateOrInsertBomFabricBom(BomFabricBom newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.BomFabricBoms.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.BomFabricBoms
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteBomFabricBom(BomFabricBom row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.BomFabricBoms
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void RemoveFromPurchasePlan(int bomIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var breakDowns = (from e in context.TblPurchaseOrderDetailBreakDowns.Include("TblPurchaseOrderDetail1")
                                  where e.Bom == bomIserial
                                  select e).ToList();
                foreach (var variable in breakDowns.Select(w => w.TblPurchaseOrderDetail))
                {
                    double? qty = 0;
                    foreach (var row in breakDowns.Where(x => x.TblPurchaseOrderDetail == variable))
                    {
                        qty = qty + row.Qty;
                        context.TblPurchaseOrderDetailBreakDowns.DeleteObject(row);
                    }
                    var purchaseline = context.TblPurchaseOrderDetails.Include("TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1").FirstOrDefault(w => w.Iserial == variable);

                    if (purchaseline != null)
                    {

                        purchaseline.Qty = purchaseline.Qty - qty;
                        purchaseline.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.Modified = true;
                    }
                }
                context.SaveChanges();
            }
        }

        public void SaveImageToPath(string ImageName, byte[] image)
        {
            int ThumbW = 0;
            int ThumbH = 0;
            GetNewDimenision(image, out ThumbW, out ThumbH);
            byte[] ImageThum = MakeThumbnail(image, ThumbW, ThumbH);
            try
            {
                //Delete File IF Exist
                if (File.Exists(@"D:\TechPackBomTrimImages\" + ImageName))
                {
                    File.Delete(@"D:\TechPackBomTrimImages\" + ImageName);
                }
                File.WriteAllBytes("D:\\TechPackBomTrimImages\\" + ImageName, image);
                File.WriteAllBytes("D:\\TechPackBomTrimImages\\" + "Thumb-" + ImageName, ImageThum);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        public byte[] MakeThumbnail(byte[] myImage, int thumbWidth, int thumbHeight)
        {

            using (MemoryStream ms = new MemoryStream())
            using (Image thumbnail = Image.FromStream(new MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }


        void GetNewDimenision(byte[] byteArrayIn, out int newWidth, out int newHeight)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            double scaleX;
            double scaleY;
            //int newWidth;
            //int newHeight;

            if (returnImage.Height > 100)
            {
                scaleY = Convert.ToDouble(100) / returnImage.Height;
                newHeight = 100;
            }
            else
            {
                scaleY = 1;
                newHeight = returnImage.Height;
            }

            if (returnImage.Width > 100)
            {
                scaleX = Convert.ToDouble(100) / returnImage.Width;
                newWidth = 100;
            }
            else
            {
                scaleX = 1;
                newWidth = returnImage.Width;
            }

            double scale = 1;
            if (scaleX < scaleY)
            {
                scale = scaleX;
                newHeight = Convert.ToInt32(returnImage.Height * scale);
            }
            else if (scaleY != 1)
            {
                scale = scaleY;
                newWidth = Convert.ToInt32(returnImage.Width * scale);
            }
        }
    }
}
