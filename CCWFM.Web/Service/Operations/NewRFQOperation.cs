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
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tblNewRFQMainHeader GetNewRfq(string docNumber)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var tmp = context.tblNewRFQMainHeaders
                    .Include("tblNewRFQDetails.tblNewRFQSizeDetails")
                    .FirstOrDefault(x => x.DocNumber == docNumber);
                return tmp;
            }
        }

        [OperationContract]
        public tblNewRFQMainHeader UpdateOrInsertNewRfq(tblNewRFQMainHeader newRow)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tblNewRFQMainHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    try
                    {
                        var max = context.tblNewRFQMainHeaders.Select(x => x.DocNumber).Cast<int>().Max(x => x);
                        newRow.DocNumber = (max + 1).ToString();
                    }
                    catch
                    {
                        newRow.DocNumber = "0";
                    }

                    context.tblNewRFQMainHeaders.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        public List<tblNewRFQDetail> UpdateOrInsertNewRfqDetail(List<tblNewRFQDetail> newRows, List<tblNewRFQDetail> deletedRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in newRows)
                {
                    var oldRow = (from e in context.tblNewRFQDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.tblNewRFQDetails.AddObject(newRow);
                    }
                }
                foreach (var TblRFSDetail in deletedRows)
                {
                    context.DeleteObject(TblRFSDetail);
                }
                context.SaveChanges();
            }
            return newRows;
        }

        [OperationContract]
        public List<tblNewRFQSizeDetail> UpdateOrInsertNewRfqSizeDetail(List<tblNewRFQSizeDetail> newRows, List<tblNewRFQSizeDetail> deletedRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in newRows)
                {
                    var oldRow = (from e in context.tblNewRFQSizeDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.tblNewRFQSizeDetails.AddObject(newRow);
                    }
                }
                foreach (var TblRFSDetail in deletedRows)
                {
                    context.DeleteObject(TblRFSDetail);
                }
                context.SaveChanges();
            }
            return newRows;
        }

        [OperationContract]
        public List<SMLDTO> SearchSMLStyles(string searchTerm)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //var temp = context.TblSeasonalMasterLists.Include("TblSeasonalMasterListDetails")
                //    .Where
                //        (x =>
                //            x.StyleCode.ToUpper().StartsWith(searchTerm.Trim().ToUpper())
                //            && x.StatusID == 2// 2 means finished
                //        ).ToList().Select(x=> new SMLDTO
                //        {
                //            BrandCode = x.BrandCode,
                //            SeasonCode = x.SeasonCode,
                //            ColorCode = x.ColorCode,
                //            StyleCode = x.StyleCode,
                //            Desc = x.Description,
                //            SMLSerial = x.Iserial,
                //            SizeRange = x.SizeRange,
                //            IsSampleRequested = context.TblRFSDetails.Any(d=>d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode),
                //            RFSSerial = context.TblRFSDetails
                //                        .Any(d => d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode)
                //                    ? context.TblRFSDetails
                //                                .FirstOrDefault(d => d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode)
                //                                .TblRFSHeader
                //                    : null,
                //            SmlSizes = new List<SMLSizesDTO>(x.tblSeasonalMasterListDetails
                //                .Select(s=> new SMLSizesDTO
                //                {
                //                    SizeCode = s.Size,
                //                    Ratio = (float)(s.Ratio ?? 0)
                //                })),
                //            Qty = (x.Qty ?? 0)
                //        });
                //return temp.ToList();
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateNewPurchDetails(List<tblNewRFQPurchLine> purchToAdd,
            List<tblNewRFQPurchLine> purchToUpdate, List<tblNewRFQPurchLine> purchToDelete)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var orderDetail in purchToAdd)
                {
                    context.tblNewRFQPurchLines.AddObject(orderDetail);
                }

                foreach (var orderDetail in purchToUpdate)
                {
                    var temp = (from x in context.tblNewRFQPurchLines
                                where x.Iserial == orderDetail.Iserial
                                select x).SingleOrDefault();

                    GenerUpdate(temp, orderDetail, context);
                }

                foreach (var purh in purchToDelete)
                {
                    context.DeleteObject(purchToDelete);
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
        public void UpdateNewPurchHeader(List<tblNewRFQPurchaseOrderHeader> purchToAdd,
            List<tblNewRFQPurchaseOrderHeader> purchToUpdate, List<tblNewRFQPurchaseOrderHeader> purchToDelete, string brandCode, string seasonCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var orderHeader in purchToAdd)
                {
                    int c;
                    try
                    {
                        c = (from a in
                                 (from x in context.tblNewRFQPurchaseOrderHeaders
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
                    context.tblNewRFQPurchaseOrderHeaders.AddObject(orderHeader);
                }

                foreach (var orderHeader in purchToUpdate)
                {
                    var temp = (from x in context.tblNewRFQPurchaseOrderHeaders
                                where x.TransID == orderHeader.TransID
                                select x).SingleOrDefault();

                    GenerUpdate(temp, orderHeader, context);
                }
                var saveChanges = context.SaveChanges();
            }
        }

        //[OperationContract]
        //[TransactionFlow(TransactionFlowOption.Allowed)]
        //public tblNewRFQPurchaseOrderHeader GetPurchaseOrderHeader()
        //{
        //}

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<SMLDTO> SearchRFQItemsFORPO(string searchTerm, int? skip, int? take)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.tblNewRFQDetails.Include("tblNewRFQSizeDetails")
                    .Where
                        (x =>
                            (x.StyleCode.ToUpper().StartsWith(searchTerm.Trim().ToUpper()) || string.IsNullOrEmpty(searchTerm))
                        ).Skip(skip ?? 0).Take(take ?? 20).ToList()
                        .Select(x => new SMLDTO
                        {
                            BrandCode = x.tblNewRFQMainHeader.BrandCode,
                            SeasonCode = x.tblNewRFQMainHeader.SeasonCode,
                            ColorCode = x.ColorCode,
                            StyleCode = x.StyleCode,
                            SMLSerial = x.Iserial,
                            Cost = x.Cost,
                            LandedCost = x.LandedCost,
                            ExchangeRate = x.ExchangeRate,
                            IsSampleRequested = context.TblRFSDetails.Any(d => d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode),
                            RFSSerial = context.TblRFSDetails
                                        .Any(d => d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode)
                                    ? context.TblRFSDetails
                                                .FirstOrDefault(d => d.StyleCode == x.StyleCode && d.ColorCode == x.ColorCode)
                                                .tblRFSHeader
                                    : null,
                            SmlSizes = new List<SMLSizesDTO>(x.tblNewRFQSizeDetails
                                .Select(s => new SMLSizesDTO
                                {
                                    SizeCode = s.SizeCode,
                                    Ratio = (float)(s.SizeRatio ?? 0)
                                })),
                            Qty = (x.Qty ?? 0)
                        });
                return temp.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private void PostNewPoToAx(tblNewRFQPurchaseOrderHeader paramHeaderObjToPost)
        {
            var context = new WorkFlowManagerDBEntities();
            var headerObjToPost =
                context.tblNewRFQPurchaseOrderHeaders.Include("tblNewRFQPurchLines.tbl_PurchaseOrderSizeDetails")
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
                var detailHeadersFull = headerObjToPost.tblNewRFQPurchLines;

                foreach (var ditem in detailHeadersFull)
                {
                    foreach (var sdItem in ditem.tblNewRFQPurchLineSizes)
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
                        inventColorTable.set_Field("InventColorId", ditem.ColorCode);
                        var clr =
                            (bool)
                                axapta.CallStaticRecordMethod("InventColor", "checkExist", ditem.StyleCode, ditem.ColorCode);
                        if (!clr)
                            inventColorTable.Insert();
                        else
                            inventColorTable.Clear();

                        try
                        {
                            inventDimCombination.set_Field("ItemID", ditem.StyleCode);
                            inventDimCombination.set_Field("InventSizeId", sdItem.Size);
                            inventDimCombination.set_Field("InventColorId", ditem.ColorCode);
                            inventDimCombination.Insert();
                        }
                        catch
                        {
                        }

                        inventDimTable.set_Field("InventColorId", ditem.ColorCode);
                        inventDimTable.set_Field("InventSizeId", sdItem.Size);

                        inventDimTable =
                            axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventDimTable) as
                                AxaptaRecord;

                        var tempx = inventDimTable.get_Field("inventDimId").ToString();
                        axaptaRecord.set_Field("InventDimId", tempx);

                        if (ditem.DeliveryDate != null)
                            axaptaRecord.set_Field("DeliveryDate", ditem.DeliveryDate);

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