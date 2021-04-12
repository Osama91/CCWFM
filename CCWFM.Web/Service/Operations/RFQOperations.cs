using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;

//using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        public static int? NullableTryParseInt32(string text)
        {
            int value;
            return int.TryParse(text, out value) ? (int?)value : null;
        }

        public static string SubstractString(string text)
        {
            return text.Split('-')[2];
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool CheckDocNum(string _DocNum)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var c = context.tbl_RFQHeader.Count(x => x.DocNumber == _DocNum);

                return c == 1;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeletePurchDetail(int detailSerial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = (from x in context.tbl_PurchaseOrderDetails
                            where x.Iserial == detailSerial
                            select x).SingleOrDefault();

                context.DeleteObject(temp);
                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool DeleteRFQ(tbl_RFQHeader ObjectToBeDeleted)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var temp = context.RFQByDocNum(ObjectToBeDeleted.DocNumber);
                    context.DeleteObject(temp);
                    context.SaveChanges();
                    return true;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblCurrency> GerCurreneis()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.TblCurrencies;
                return temp.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_lkp_CostTypes> GetCostTypes()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.tbl_lkp_CostTypes;
                return temp.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_RFQHeader GetRFQ(string docNum, out List<TblColor> colorsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var temp = context.RFQByDocNum(docNum);
                    colorsList = new List<TblColor>();
                    int test;
                    foreach (var row in GetTblColorLink(0, 50, temp.BrandCode, 1, 1, "it.TblBrand", null, null, out test))
                    {
                        colorsList.Add(row.TblColor1);
                    }

                    //     colorsList = GetTblColorLink(temp.BrandCode,0, temp.TransID);
                    return temp;
                }
                catch
                {
                    throw new Exception("Document number does not exist");
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public string SaveAllNewRFQ(tbl_RFQHeader rfqObj)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                var temp = "";

                try
                {
                    var first = contex.sp_GetMaxRFQDocNum().FirstOrDefault();
                    if (first != null) temp = (int.Parse(first) + 1).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    temp = 0.ToString(CultureInfo.InvariantCulture);
                }
                rfqObj.DocNumber = temp;

                int c;
                try
                {
                    c = (from a in
                             (from x in contex.tbl_PurchaseOrderHeader
                              where x.PurchaseID.StartsWith(rfqObj.BrandCode + "-" + rfqObj.SeasonCode)
                              select x.PurchaseID).ToList()
                         let str = SubstractString(a)
                         let value = NullableTryParseInt32(str)
                         select value ?? 0).Max();
                }
                catch
                {
                    c = 000000;
                }
                foreach (var purchaseOrderHeader in rfqObj.tbl_PurchaseOrderHeader)
                {
                    var purchNum = "";
                    var newC = (c + 1);
                    if (newC < 10)
                        purchNum = "00000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 10 && newC < 100)
                        purchNum = "0000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 100 && newC < 1000)
                        purchNum = "000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 1000 && newC < 10000)
                        purchNum = "00" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 10000 && newC < 100000)
                        purchNum = "0" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 100000 && newC < 1000000)
                        purchNum = newC.ToString(CultureInfo.InvariantCulture);
                    purchaseOrderHeader.PurchaseID =
                        (rfqObj.BrandCode + "-" + rfqObj.SeasonCode + "-" +
                         (string.IsNullOrEmpty(purchNum) ? "000000" : purchNum))
                        ;
                    c++;
                }
                contex.tbl_RFQHeader.AddObject(rfqObj);
                contex.SaveChanges();
                return rfqObj.DocNumber;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<V_AXStyles> SearchAXStyles(string StyleCode, out List<SizesWithGroups> sizesWithGroups)
        {
            //using (var context = new WorkFlowManagerDBEntities())
            //{
            //    try
            //    {
            //        if (!StyleCode.Contains(":"))
            //        {
            //            var ret = context.V_AXStyles
            //                                    .Where(x => x.StyleCode.ToLower().StartsWith(StyleCode.ToLower())
            //                                     && !context.tbl_RFQDetail.Select(s => s.StyleCode).Contains(x.StyleCode)).Take(20);

            //            var styleCodes = ret.Select(x => x.StyleCode).ToList();

            //            sizesWithGroups = context.v_RetailItemsWithSizeGroup
            //                    .Where(x => styleCodes.Contains(x.Style))
            //                    .Select(sg => new SizesWithGroups { SizeCode = sg.SizeCode, SizeGroup = sg.SizeGroupCode, StyleCode = sg.Style })
            //                    .Distinct().ToList();

            //            return ret.ToList();
            //        }
            //        else
            //        {
            //            StyleCode = StyleCode.Split(':')[1];
            //            var ret = context.V_AXStyles
            //                    .Where(x => x.StyleCode.ToLower().StartsWith(StyleCode.ToLower())
            //                     && !context.TblSeasonalMasterLists.Select(s => s.StyleCode).Contains(x.StyleCode)).Take(20);

            //            var styleCodes = ret.Select(x => x.StyleCode).ToList();

            //            sizesWithGroups = context.v_RetailItemsWithSizeGroup
            //                    .Where(x => styleCodes.Contains(x.Style))
            //                    .Select(sg => new SizesWithGroups { SizeCode = sg.SizeCode, SizeGroup = sg.SizeGroupCode, StyleCode = sg.Style })
            //                    .Distinct().ToList();

            //            return ret.ToList();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
            sizesWithGroups = null;
            return null;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<ItemsDto> SearchForRfqItems(string searchTerm, string descSearchTerm, string source)
        {
            WorkFlowManagerDBEntities context;
            using (context = new WorkFlowManagerDBEntities())
            {
                context.CommandTimeout = 120;
                ax2009_ccEntities axContext;
                using (axContext = new ax2009_ccEntities())
                {
                    axContext.CommandTimeout = 120;

                    if (source.ToLower().Contains("ac"))
                    {
                        #region accessories

                        var temp =
                            (from i in axContext.INVENTTABLEMODULEs

                             join x in axContext.INVENTTABLEs
                                 on new { i.ITEMID, i.DATAAREAID } equals new { x.ITEMID, x.DATAAREAID }
                             where i.MODULETYPE == 0
                                   && x.ITEMGROUPID.ToUpper() == "ACCESSORIES"
                                   && i.DATAAREAID == "Ccm"
                                   && (string.IsNullOrEmpty(searchTerm) || x.ITEMID.ToUpper()
                                       .StartsWith(searchTerm.Trim().ToUpper()))
                                   && (string.IsNullOrEmpty(descSearchTerm) || x.ITEMNAME.ToUpper()
                                       .Contains(descSearchTerm.Trim().ToUpper()))
                             join y in axContext.INVENTDIMCOMBINATIONs
                                 on new { i.ITEMID, i.DATAAREAID } equals new { y.ITEMID, y.DATAAREAID }
                             join z in axContext.INVENTDIMs
                                 on y.DATAAREAID equals z.DATAAREAID
                             where z.INVENTDIMID == y.INVENTDIMID
                             select new ItemsDto
                             {
                                 Code = x.ITEMID,
                                 Size = z.INVENTSIZEID,
                                 Config = z.CONFIGID,
                                 Batch = z.INVENTBATCHID,
                                 ItemGroup = "ACCESSORIES",
                                 Name = x.ITEMNAME,
                                 Desc = x.ITEMNAME
                             }).Take(20).ToList();
                        temp.ForEach(
                           x =>
                               x.Images =
                                   context.tbl_AccessoryAttributesDetails.Where(f => f.Code == x.Code && f.Configuration == x.Config && f.Size == x.Size)
                                       .Select(f => new ImageDto { Image = f.AccImage }).DefaultIfEmpty().ToList());
                        temp.ForEach(
                            x =>
                                x.Image =
                                    context.tbl_AccessoryAttributesDetails.Where(
                                        f => f.Code == x.Code && f.Configuration == x.Config && f.Size == x.Size)
                                        .Select(f => f.AccImage).FirstOrDefault());
                        var ret = temp;
                        return ret;

                        #endregion accessories
                    }
                    if (source.ToLower().Contains("fab"))
                    {
                        #region Fabric

                        var temp =
                            (from i in axContext.INVENTTABLEMODULEs

                             join x in axContext.INVENTTABLEs
                                 on new { i.ITEMID, i.DATAAREAID } equals new { x.ITEMID, x.DATAAREAID }
                             where i.MODULETYPE == 0
                                   && x.ITEMGROUPID.ToUpper() != "ACCESSORIES"
                                   && i.DATAAREAID == "Ccm"
                                   && (string.IsNullOrEmpty(searchTerm) || x.ITEMID.ToUpper()
                                       .StartsWith(searchTerm.Trim().ToUpper()))
                                   && (string.IsNullOrEmpty(descSearchTerm) || x.ITEMNAME.ToUpper()
                                       .Contains(descSearchTerm.Trim().ToUpper()))
                             join y in axContext.INVENTDIMCOMBINATIONs
                                 on new { i.ITEMID, i.DATAAREAID } equals new { y.ITEMID, y.DATAAREAID }
                             join z in axContext.INVENTDIMs
                                 on y.DATAAREAID equals z.DATAAREAID
                             where z.INVENTDIMID == y.INVENTDIMID
                             select new ItemsDto
                             {
                                 Code = x.ITEMID,
                                 Size = z.INVENTSIZEID,
                                 Config = z.CONFIGID,
                                 Batch = z.INVENTBATCHID,
                                 ItemGroup = "Fabric",
                                 Name = x.ITEMNAME,
                                 Desc = x.ITEMNAME
                             }).Take(20).ToList();

                        temp.ForEach(
                            x =>
                                x.Images =
                                    context.tbl_FabricImage.Where(f => f.FabricCode == x.Code)
                                        .Select(f => new ImageDto { Image = f.FabImage }).DefaultIfEmpty().ToList());
                        temp.ForEach(x => x.Image = context.tbl_FabricImage.Where(f => f.FabricCode == x.Code)
                                       .Select(f => f.FabImage).FirstOrDefault());
                        var ret = temp;
                        return ret;

                        #endregion Fabric
                    }
                    if (source.ToLower().Contains("gen"))
                    {
                        #region Generic

                        var temp = context.Fabric_new
                            .Where(x => (string.IsNullOrEmpty(searchTerm) || x.Fabric_Code.ToUpper()
                                          .StartsWith(searchTerm.Trim().ToUpper()))
                                      && (string.IsNullOrEmpty(descSearchTerm) || x.Fabric_Ename.ToUpper()
                                          .Contains(descSearchTerm.Trim().ToUpper())) &&
                                        !x.Fabric_FabricType.ToLower().Contains("oper"))
                            .Select(x => new ItemsDto
                            {
                                Code = x.Fabric_Code,
                                Name = x.Fabric_Ename,
                                ItemGroup = "Generic"
                            });
                        return temp.ToList();

                        #endregion Generic
                    }
                }
                return new List<ItemsDto>();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateAdditionalCosts(List<tbl_RFQ_AdditionalCost> rfqObjToUpdate)
        {
            try
            {
                using (var contex = new WorkFlowManagerDBEntities())
                {
                    foreach (var item in rfqObjToUpdate.Select(x => x.Tbl_RFQFollowup).Distinct())
                    {
                        var item1 = item;
                        var temp = (from x in rfqObjToUpdate
                                    let chk = CheckAdditionalCosts(x)
                                    where x.Tbl_RFQFollowup == item1
                                    && !chk
                                    select x);

                        foreach (var tempd in temp.ToList())
                        {
                            contex.tbl_RFQ_AdditionalCost.AddObject(tempd);
                        }

                        var tempdlh = (from x in contex.tbl_RFQ_AdditionalCost
                                           .Where(z => z.Tbl_RFQFollowup == item).ToList()
                                       where rfqObjToUpdate.Any(z => z.Iserial != x.Iserial)
                                       select x);

                        foreach (var tempdl in tempdlh)
                        {
                            contex.DeleteObject(tempdl);
                        }
                    }
                    foreach (var item in rfqObjToUpdate)
                    {
                        var temp = (from x in contex.tbl_RFQ_AdditionalCost
                                    where x.Iserial == item.Iserial
                                    select x).SingleOrDefault();

                        if (temp != null)
                            GenerUpdate(temp, item, contex);
                    }
                    contex.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdatePurchDetails(List<tbl_PurchaseOrderDetails> purchToAdd,
            List<tbl_PurchaseOrderDetails> purchToUpdate, List<tbl_PurchaseOrderDetails> purchToDelete)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var orderDetail in purchToAdd)
                {
                    context.tbl_PurchaseOrderDetails.AddObject(orderDetail);
                }

                foreach (var orderDetail in purchToUpdate)
                {
                    var temp = (from x in context.tbl_PurchaseOrderDetails
                                where x.Iserial == orderDetail.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, orderDetail, context);
                }

                foreach (var purh in purchToDelete)
                {
                    context.DeleteObject(purh);
                }
                try
                {
                    var saveChanges = context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdatePurchHeader(List<tbl_PurchaseOrderHeader> purchToAdd,
            List<tbl_PurchaseOrderHeader> purchToUpdate, List<tbl_PurchaseOrderHeader> purchToDelete, string brandCode, string seasonCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var orderHeader in purchToAdd)
                {
                    int c;
                    try
                    {
                        c = (from a in
                                 (from x in context.tbl_PurchaseOrderHeader
                                  where x.PurchaseID.StartsWith(brandCode + "-" + seasonCode)
                                  select x.PurchaseID).ToList()
                             let str = SubstractString(a)
                             let value = NullableTryParseInt32(str)
                             select value ?? 0).Max();
                    }
                    catch
                    {
                        c = 0;
                    }
                    var purchNum = "";
                    var newC = (c + 1);
                    if (newC < 10)
                        purchNum = "00000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 10 && newC < 100)
                        purchNum = "0000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 100 && newC < 1000)
                        purchNum = "000" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 1000 && newC < 10000)
                        purchNum = "00" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 10000 && newC < 100000)
                        purchNum = "0" + newC.ToString(CultureInfo.InvariantCulture);
                    if (newC > 100000 && newC < 1000000)
                        purchNum = newC.ToString(CultureInfo.InvariantCulture);
                    orderHeader.PurchaseID =
                        (brandCode + "-" + seasonCode + "-" + (string.IsNullOrEmpty(purchNum) ? "000000" : purchNum))
                        ;
                    context.tbl_PurchaseOrderHeader.AddObject(orderHeader);
                }

                foreach (var orderHeader in purchToUpdate)
                {
                    var temp = (from x in context.tbl_PurchaseOrderHeader
                                where x.TransID == orderHeader.TransID
                                select x).SingleOrDefault();

                    GenerUpdate(temp, orderHeader, context);
                }
                var saveChanges = context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdatePurchlineAdditionalCosts(List<tbl_PurchaseOrder_AdditionalCost> rfqObjToUpdate)
        {
            try
            {
                using (var contex = new WorkFlowManagerDBEntities())
                {
                    foreach (var item in rfqObjToUpdate.Select(x => x.ParentPurchLineSerial).Distinct())
                    {
                        var item1 = item;
                        var temp = (from x in rfqObjToUpdate
                                    let chk = CheckAdditionalCosts(x)
                                    where x.ParentPurchLineSerial == item1
                                    && !chk
                                    select x);

                        foreach (var tempd in temp.ToList())
                        {
                            contex.tbl_PurchaseOrder_AdditionalCost.AddObject(tempd);
                        }

                        var tempdlh = (from x in contex.tbl_PurchaseOrder_AdditionalCost
                                           .Where(z => z.ParentPurchLineSerial == item).ToList()
                                       where rfqObjToUpdate.Any(z => z.Iserial != x.Iserial)
                                       select x);

                        foreach (var tempdl in tempdlh)
                        {
                            contex.DeleteObject(tempdl);
                        }
                    }
                    foreach (var item in rfqObjToUpdate)
                    {
                        var temp = (from x in contex.tbl_PurchaseOrder_AdditionalCost
                                    where x.Iserial == item.Iserial
                                    select x).SingleOrDefault();

                        if (temp != null)
                            GenerUpdate(temp, item, contex);
                    }
                    contex.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQ(tbl_RFQHeader _RFQObj)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                var temp = (from x in contex.tbl_RFQHeader
                            where x.TransID == _RFQObj.TransID
                            select x).SingleOrDefault();

                if (temp != null)
                {
                    GenerUpdate(temp, _RFQObj, contex);
                }
                contex.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQCostFollowUps(List<tbl_RFQFollowup> _RFQObjToUpdate, List<tbl_RFQFollowup> _RFQObjToADD, List<tbl_RFQFollowup> _RFQObjToDelete)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                foreach (var item in _RFQObjToADD)
                {
                    contex.tbl_RFQFollowup.AddObject(item);
                }
                foreach (var item in _RFQObjToUpdate)
                {
                    var temp = (from x in contex.tbl_RFQFollowup
                                where x.Iserial == item.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, item, contex);
                }
                foreach (var temp in _RFQObjToDelete.Select(item => (from x in contex.tbl_RFQFollowup
                                                                     where x.Iserial == item.Iserial
                                                                     select x).SingleOrDefault()))
                {
                    contex.DeleteObject(temp);
                }
                contex.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQFollowUps(List<tbl_RFQFollowup> _RFQObjToUpdate, List<tbl_RFQFollowup> _RFQObjToADD, List<tbl_RFQFollowup> _RFQObjToDelete)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                foreach (var item in _RFQObjToADD)
                {
                    contex.tbl_RFQFollowup.AddObject(item);
                }
                foreach (var item in _RFQObjToUpdate)
                {
                    var temp = (from x in contex.tbl_RFQFollowup
                                where x.Iserial == item.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, item, contex);
                }
                foreach (var temp in _RFQObjToDelete.Select(item => (from x in contex.tbl_RFQFollowup
                                                                     where x.Iserial == item.Iserial
                                                                     select x).SingleOrDefault()))
                {
                    contex.DeleteObject(temp);
                }
                contex.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQSubHeader(List<tbl_RFQDetail> _RFQObjToUpdate, List<tbl_RFQDetail> _RFQObjToADD, List<tbl_RFQDetail> _RFQObjToDelete)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                foreach (var item in _RFQObjToADD)
                {
                    contex.tbl_RFQDetail.AddObject(item);
                }
                foreach (var item in _RFQObjToUpdate)
                {
                    var temp = (from x in contex.tbl_RFQDetail
                                where x.SubHeaderID == item.SubHeaderID
                                select x).SingleOrDefault();

                    GenerUpdate(temp, item, contex);
                }
                foreach (var temp in _RFQObjToDelete.Select(item => (from x in contex.tbl_RFQDetail
                                                                     where x.SubHeaderID == item.SubHeaderID
                                                                     select x).SingleOrDefault()))
                {
                    contex.DeleteObject(temp);
                }
                contex.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQSubHeaderItems(List<tbl_RFQDetailItem> _RFQObjToUpdate,
            List<tbl_RFQDetailItem> _RFQObjToADD, List<tbl_RFQDetailItem> _RFQObjToDelete)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                foreach (var item in _RFQObjToADD)
                {
                    contex.tbl_RFQDetailItem.AddObject(item);
                }
                foreach (var item in _RFQObjToUpdate)
                {
                    var temp = (from x in contex.tbl_RFQDetailItem
                                where x.Iserial == item.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, item, contex);
                }
                foreach (var temp in _RFQObjToDelete.Select(item => (from x in contex.tbl_RFQDetailItem
                                                                     where x.Iserial == item.Iserial
                                                                     select x).SingleOrDefault()))
                {
                    contex.DeleteObject(temp);
                }
                contex.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateRFQSubHeaderServices(List<tbl_RFQDetailService> rfqObjToUpdate,
            List<tbl_RFQDetailService> rfqObjToAdd, List<tbl_RFQDetailService> rfqObjToDelete)
        {
            using (var contex = new WorkFlowManagerDBEntities())
            {
                foreach (var item in rfqObjToAdd)
                {
                    contex.tbl_RFQDetailService.AddObject(item);
                }
                foreach (var item in rfqObjToUpdate)
                {
                    var temp = (from x in contex.tbl_RFQDetailService
                                where x.Iserial == item.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, item, contex);
                }
                foreach (var temp in rfqObjToDelete.Select(item => (from x in contex.tbl_RFQDetailService
                                                                    where x.Iserial == item.Iserial
                                                                    select x).SingleOrDefault()))
                {
                    contex.DeleteObject(temp);
                }
                contex.SaveChanges();
            }
        }

        private static void GenerUpdate<T>(T oldValues, T newValues, ObjectContext entities)
        {
            var orgRow = entities.ObjectStateManager.GetObjectStateEntry(oldValues);
            orgRow.ApplyCurrentValues(newValues);
            entities.SaveChanges();
        }

        private bool CheckAdditionalCosts(tbl_PurchaseOrder_AdditionalCost x)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_PurchaseOrder_AdditionalCost.Any(c => c.Iserial == x.Iserial);
            }
        }

        private bool CheckAdditionalCosts(tbl_RFQ_AdditionalCost x)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_RFQ_AdditionalCost.Any(c => c.Iserial == x.Iserial);
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private void PostPoToAx(tbl_PurchaseOrderHeader paramHeaderObjToPost)
        {
            var context = new WorkFlowManagerDBEntities();
            var headerObjToPost =
                context.tbl_PurchaseOrderHeader.Include("tbl_PurchaseOrderDetails.tbl_PurchaseOrderSizeDetails")
                    .FirstOrDefault(x => x.TransID == paramHeaderObjToPost.TransID);

            var axapta = new Axapta();

            var credential = new NetworkCredential("bcproxy", "around1");
            try
            {
                axapta.LogonAs("ahmed.gamal", "ccasual.loc", credential, "ccr", null, null, null);
            }
            catch (Exception)
            {
                throw new Exception("There was a problem logging to ax");
            }
            try
            {
                axapta.TTSBegin();
                var inventDimTable = axapta.CreateAxaptaRecord("InventDim");
                var inventColorTable = axapta.CreateAxaptaRecord("InventColor");
                var inventDimCombination = axapta.CreateAxaptaRecord("InventDimCombination");

                var axaptaRecord = axapta.CreateAxaptaRecord("PurchTable");
                axaptaRecord.Clear();
                axaptaRecord.InitValue();
                var purchId = headerObjToPost.PurchaseID;
                axaptaRecord.set_Field("PurchId", purchId);

                var header =
                    axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.Vendor) as AxaptaRecord;
                axaptaRecord.Call("initFromVendTable", header);

                axaptaRecord.Insert();
                context.tbl_PurchaseOrderDetails.MergeOption = MergeOption.NoTracking;
                context.V_Warehouse.MergeOption = MergeOption.NoTracking;
                var detailHeadersFull = headerObjToPost.tbl_PurchaseOrderDetails;

                foreach (var ditem in detailHeadersFull)
                {
                    foreach (var sdItem in ditem.tbl_PurchaseOrderSizeDetails)
                    {
                        var itemId = ditem.StyleCode;

                        axaptaRecord = axapta.CreateAxaptaRecord("PurchLine");
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        inventDimTable.Clear();
                        inventDimTable.set_Field("InventLocationId", headerObjToPost.WareHouseID);

                        var warehouse =
                            context.V_Warehouse.SingleOrDefault(
                                x => x.DataAreaID == "ccr" && x.WarehouseID == headerObjToPost.WareHouseID);
                        if (warehouse !=
                            null)
                            inventDimTable.set_Field("InventSiteId", warehouse.SiteId);

                        inventColorTable.set_Field("ItemID", ditem.StyleCode);
                        inventColorTable.set_Field("InventColorId", ditem.Color);
                        var clr =
                            (bool)
                                axapta.CallStaticRecordMethod("InventColor", "checkExist", ditem.StyleCode, ditem.Color);
                        if (!clr)
                            inventColorTable.Insert();
                        else
                            inventColorTable.Clear();

                        try
                        {
                            inventDimCombination.set_Field("ItemID", ditem.StyleCode);
                            inventDimCombination.set_Field("InventSizeId", sdItem.Size);
                            inventDimCombination.set_Field("InventColorId", ditem.Color);
                            inventDimCombination.Insert();
                        }
                        catch
                        {
                        }

                        inventDimTable.set_Field("InventColorId", ditem.Color);
                        inventDimTable.set_Field("InventSizeId", sdItem.Size);

                        inventDimTable =
                            axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventDimTable) as
                                AxaptaRecord;

                        var tempx = inventDimTable.get_Field("inventDimId").ToString();
                        axaptaRecord.set_Field("InventDimId", tempx);

                        if (ditem.DelivaryDate != null)
                            axaptaRecord.set_Field("DeliveryDate", ditem.DelivaryDate);

                        axaptaRecord.set_Field("ItemId", itemId);
                        axaptaRecord.set_Field("purchId", purchId);
                        axaptaRecord.set_Field("PurchUnit", "Pcs");
                        axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(sdItem.Qty.ToString()));
                        axaptaRecord.set_Field("PurchPrice", Convert.ToDecimal(ditem.PurchasePrice));
                        axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(sdItem.Qty.ToString()));
                        axaptaRecord.set_Field("LineAmount",
                            Convert.ToDecimal((sdItem.Qty * ditem.PurchasePrice).ToString()));

                        axaptaRecord.Call("createLine", true, true, false, true, true, false);
                    }

                    //TO DO: Add Posting TO Retail Plus Logic Here!

                    //-////////////////////////////////////////////
                    var retailContext = new ccnewEntities();
                    //var retailPoHeader = new TblPOHeader
                    //{
                    //    Code = headerObjToPost.PurchaseID,
                    //    tblstore = headerObjToPost.WareHouseID,
                    //     tblseason = headerObjToPost.tbl_RFQHeader.SeasonCode,

                    //};
                    var retailPoMainDetail = new TblPOMainDetail();
                    var retailPoDetail = new TblPODetail();

                    axapta.TTSCommit();
                    headerObjToPost.IsPosted = true;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                axapta.TTSAbort();
                throw;
            }
            finally
            {
                axapta.Logoff();
                axapta.Dispose();
            }
        }
    }
}

#region [ Eeager Loading Extension Methods for RFQ Operations ]

public static class Extensions
{
    public static IQueryable<tbl_RFQHeader> CompleteRFQ(this WorkFlowManagerDBEntities context)
    {
        return context.tbl_RFQHeader
            .Include("tbl_RFQDetail.tbl_RFQDetailItem")
            .Include("tbl_RFQDetail.tbl_RFQDetailService")
            .Include("tbl_RFQDetail.tbl_RFQFollowup.tbl_RFQ_AdditionalCost")
            .Include("tbl_PurchaseOrderHeader.tbl_PurchaseOrderDetails.tbl_PurchaseOrderSizeDetails")
            .Include("tbl_PurchaseOrderHeader.tbl_PurchaseOrderDetails.tbl_PurchaseOrder_AdditionalCost");
    }

    public static tbl_RFQHeader RFQByDocNum(this WorkFlowManagerDBEntities context, string _DocNumber)
    {
        try
        {
            return context.tbl_RFQHeader
                    .Include("tbl_RFQDetail.tbl_RFQDetailItem")
                    .Include("tbl_RFQDetail.tbl_RFQDetailService")
                    .Include("tbl_PurchaseOrderHeader.tbl_PurchaseOrderDetails.tbl_PurchaseOrderSizeDetails")
                    .Include("tbl_PurchaseOrderHeader.tbl_PurchaseOrderDetails.tbl_PurchaseOrder_AdditionalCost")
                    .Include("tbl_RFQDetail.tbl_RFQFollowup.tbl_RFQ_AdditionalCost")
                    .FirstOrDefault(c => c.DocNumber == _DocNumber);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

#endregion [ Eeager Loading Extension Methods for RFQ Operations ]