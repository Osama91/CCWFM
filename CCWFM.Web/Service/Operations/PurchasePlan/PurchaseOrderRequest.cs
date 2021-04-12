using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using LinqKit;
using System.Data.Objects;

namespace CCWFM.Web.Service.Operations.PurchasePlan
{
    public partial class PurchasePlan
    {
        [OperationContract]
        private List<ViewPurchaseOrderHeaderRequest> GetTblPurchaseOrderHeaderRequest(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<Vendor> vendorList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.ViewPurchaseOrderHeaderRequests.MergeOption = MergeOption.NoTracking;
                IQueryable<ViewPurchaseOrderHeaderRequest> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.ViewPurchaseOrderHeaderRequests.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.ViewPurchaseOrderHeaderRequests.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.ViewPurchaseOrderHeaderRequests.Count();
                    query = context.ViewPurchaseOrderHeaderRequests.OrderBy(sort).Skip(skip).Take(take);
                }


                var vendorListStrings = query.Select(x => x.Vendor);
                vendorList = context.Vendors.Where(x => vendorListStrings.Any(l => x.vendor_code == l)).ToList();


                return query.OrderByDescending(w => w.Iserial).ToList();
            }
        }

        [OperationContract]
        private TblPurchaseOrderHeaderRequest UpdateOrInsertTblPurchaseOrderHeaderRequest(TblPurchaseOrderHeaderRequest newRow, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var warehouse = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.TblWarehouse).Po;

                var seq = context.TblSequenceProductions.FirstOrDefault(w => w.Iserial == warehouse);

                var oldRow = (from e in context.TblPurchaseOrderHeaderRequests
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.CreatedBy = oldRow.CreatedBy;
                    newRow.CreationDate = oldRow.CreationDate;
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    newRow.CreationDate = DateTime.Now;
                    newRow.CreatedBy = user;
                    newRow.Code = SharedOperation.HandelSequence(seq);
                    context.TblPurchaseOrderHeaderRequests.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderHeaderRequest(TblPurchaseOrderHeaderRequest row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderHeaderRequests
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPurchaseOrderDetailRequest> GetTblPurchaseOrderDetailRequest(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out Dictionary<int?, double?> purchaseRec)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseOrderDetailRequest> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPurchaseOrderHeaderRequest=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseOrderDetailRequests.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseOrderDetailRequests.Include("TblPurchaseRequestLinks").Include("TblColor").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseOrderDetailRequests.Count(x => x.TblPurchaseOrderHeaderRequest == groupId);
                    query = context.TblPurchaseOrderDetailRequests.Include("TblPurchaseRequestLinks").Include("TblColor").OrderBy(sort).Where(v => v.TblPurchaseOrderHeaderRequest == groupId).Skip(skip).Take(take);
                }
                var listofPurchase = query.Select(x => x.Iserial);
                purchaseRec = context.TblPurchaseReceiveDetails.Where(x => listofPurchase.Any(l => x.TblPurchaseOrderDetailRequest == l)).GroupBy(x => x.TblPurchaseOrderDetailRequest).ToDictionary(t => t.Key, t => t.Sum(w => w.Qty));
                return query.ToList();
            }
        }
       
        //Get By Plan Code
        [OperationContract]
        private List<TblPurchaseOrderHeader> GetTblPurchaseOrderDetailRequestByPlanCode(string planCode,string vendorCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblPurchaseOrderHeaders.Include("TblPurchaseOrderDetails").Include("TblPurchaseOrderDetails.TBLCOLOR")
                    .Include("TblGeneratePurchaseHeader1").Where(X => X.TblGeneratePurchaseHeader1.Code == planCode && X.Vendor == vendorCode);
                if (query != null)
                {
                    return query.ToList();
                }
                else
                { return null; }
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Vendor> GetVendorsByPlanCode(string planCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var vendors = context.TblPurchaseOrderHeaders
                    .Include("TblGeneratePurchaseHeader1").Where(c => c.TblGeneratePurchaseHeader1.Code == planCode).Select(v=>v.Vendor).ToList();
                var query= context.Vendors.Where(x => vendors.Contains(x.vendor_code));
                return query.ToList();

            }
        }

        [OperationContract]
        private TblPurchaseOrderDetailRequest UpdateOrInsertTblPurchaseOrderDetailRequest(TblPurchaseOrderDetailRequest newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetailRequests
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    newRow.RemaningQty = newRow.Qty;
                    context.TblPurchaseOrderDetailRequests.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderDetailRequest(TblPurchaseOrderDetailRequest row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseOrderDetailRequests
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPurchaseReceiveHeader> GetTblPurchaseReceiveHeader(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseReceiveHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPurchaseOrderHeaderRequest=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblPurchaseReceiveHeaders.Include(nameof(TblPurchaseReceiveHeader.TblWarehouse1)).Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseReceiveHeaders.Include(nameof(TblPurchaseReceiveHeader.TblWarehouse1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseReceiveHeaders.Include(nameof(TblPurchaseReceiveHeader.TblWarehouse1)).Count(x => x.TblPurchaseOrderHeaderRequest == groupId);
                    query = context.TblPurchaseReceiveHeaders.Include(nameof(TblPurchaseReceiveHeader.TblWarehouse1)).OrderBy(sort).Where(v => v.TblPurchaseOrderHeaderRequest == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblPurchaseReceiveDetail> GetTblPurchaseReceiveDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseReceiveDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPurchaseReceiveHeader=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseReceiveDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseReceiveDetails.Include("TblPurchaseOrderDetailRequest1.TblColor").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseReceiveDetails.Count(x => x.TblPurchaseReceiveHeader == groupId);
                    query = context.TblPurchaseReceiveDetails.Include("TblPurchaseOrderDetailRequest1.TblColor").OrderBy(sort).Where(v => v.TblPurchaseReceiveHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblPurchaseOrderDetail> SearchPurchaseOrderDetail(string Vendor, string ItemID, int Color, string brand, int? BrandSection, int? Season, DateTime? FromDate, DateTime? ToDate, string CurrencyCode, bool StyleWithCostPrice)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var predicate = PredicateBuilder.True<TblPurchaseOrderDetail>();
                if (!string.IsNullOrEmpty(brand))
                    predicate = predicate.And(i => i.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.Brand == brand);
                if (BrandSection != null && BrandSection != 0)
                    predicate = predicate.And(i => i.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpBrandSection == BrandSection);
                if (Season != null && Season != 0)
                    predicate = predicate.And(i => i.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpSeason == Season);
                if (!string.IsNullOrEmpty(ItemID))
                    predicate = predicate.And(v => v.ItemId == ItemID);
                if (FromDate != null)
                    predicate = predicate.And(v => v.TblPurchaseOrderHeader1.TransDate >= FromDate);
                if (ToDate != null)
                    predicate = predicate.And(v => v.TblPurchaseOrderHeader1.TransDate <= ToDate);
                if (Color != 0)
                {
                    predicate = predicate.And(v => v.FabricColor == Color);
                }

                predicate = predicate.And(i => i.TblPurchaseOrderHeader1.TblPurchaseHeaderType == 0);
                predicate = predicate.And(v => (!context.TblPurchaseOrderDetailRequests.All(e => e.TblPurchaseRequestLinks.All(r => r.TblPurchaseOrderDetailRequest == v.Iserial))));
                predicate = predicate.And(v => v.Canceled == false);
                predicate = predicate.And(v => v.TblPurchaseOrderHeader1.CurrencyCode == CurrencyCode);
                predicate = predicate.And(v => v.Qty > 0);
                predicate = predicate.And(v => v.TblPurchaseOrderHeader1.Vendor == Vendor);
                if (StyleWithCostPrice)
                {
                    predicate = predicate.And(v => v.TblPurchaseOrderDetailBreakDowns.All(w => w.BOM1.TblSalesOrder1.TblStyle1.TargetCostPrice != 0));
                }

                IQueryable<TblPurchaseOrderDetail> query;
                query = context.TblPurchaseOrderDetails.Include("TblColor").Include("TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpBrandSection1").Include("TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpSeason1").AsExpandable().Where(predicate);
                // query= query.Where(v => (!context.TblPurchaseOrderDetailRequests.Any(e => e.TblPurchaseRequestLinks.Any(r => r.TblPurchaseOrderDetailRequest == v.Iserial))));
                return query.ToList();
            }
        }


        [OperationContract]
        private TblPurchaseReceiveHeader UpdateOrInsertTblPurchaseReceiveHeader(TblPurchaseReceiveHeader newRow, int index, out int outindex, string createdBy, int userIserial)
        {
            outindex = index;
            var totallist = new List<TblPurchaseReceiveDetail>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                var newrowRet = new TblPurchaseReceiveHeader();
                var oldRow = (from e in context.TblPurchaseReceiveHeaders.Include("TblPurchaseReceiveDetails")
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.CreatedBy = oldRow.CreatedBy;
                    newRow.CreationDate = oldRow.CreationDate;
                    newRow.LastUpdatedBy = createdBy;
                    newRow.LastUpdatedDate = DateTime.Now;
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                    var OlDheader = context.TblPurchaseOrderHeaderRequests.FirstOrDefault(w => w.Iserial == oldRow.TblPurchaseOrderHeaderRequest);
                    if (OlDheader.AxPurchase == null)
                    {
                        foreach (var VARIABLE in oldRow.TblPurchaseReceiveDetails)
                        {
                            totallist.Add(VARIABLE);
                        }
                    }
                }
                else
                {
                    var warehouse = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.TblWarehouse);

                    foreach (var VARIABLE in newRow.TblPurchaseReceiveDetails)
                    {
                        totallist.Add(VARIABLE);
                    }
                    newRow.TblPurchaseReceiveDetails.Clear();

                    if (totallist.Any(w => w.Qty > 0))
                    {
                        foreach (var variable in totallist.Where(x => x.Qty > 0))
                        {
                            newRow.TblPurchaseReceiveDetails.Add(variable);
                        }

                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == warehouse.Receive);
                        newRow.DocCode = SharedOperation.HandelSequence(seqRow);
                        newRow.CreatedBy = createdBy;
                        newRow.CreationDate = DateTime.Now;
                        newRow.TblInventType = 1;

                        context.TblPurchaseReceiveHeaders.AddObject(newRow);
                    }
                    if (totallist.Any(w => w.Qty < 0))
                    {
                        newrowRet = new TblPurchaseReceiveHeader
                        {
                            DocDate = newRow.DocDate,
                            RefNo = newRow.RefNo,
                            Notes = newRow.Notes,
                            TblWarehouse = newRow.TblWarehouse,
                            Vendor = newRow.Vendor,
                            TblPurchaseOrderHeaderRequest = newRow.TblPurchaseOrderHeaderRequest,
                            Iserial = 0,

                        };
                        newrowRet.TblPurchaseReceiveDetails.Clear();
                        foreach (var variable in totallist.Where(x => x.Qty < 0))
                        {
                            newrowRet.TblPurchaseReceiveDetails.Add(variable);
                        }

                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == warehouse.RetReceive);
                        newrowRet.DocCode = SharedOperation.HandelSequence(seqRow);
                        newrowRet.CreatedBy = createdBy;
                        newrowRet.CreationDate = DateTime.Now;
                        newrowRet.TblInventType = 2;
                        context.TblPurchaseReceiveHeaders.AddObject(newrowRet);
                    }
                }
                context.SaveChanges();

                //if (true)
                //{

                //}

                //var ListOfRec = (from e in context.TblPurchaseReceiveDetails
                //                 where e.TblPurchaseReceiveHeader == newRow.Iserial
                //                 select e).ToList();
                //newRow.TblPurchaseReceiveDetails = new System.Data.Objects.DataClasses.EntityCollection<TblPurchaseReceiveDetail>();
                //foreach (var item in ListOfRec)
                //{

                //    newRow.TblPurchaseReceiveDetails.Add(item);
                //}

                //foreach (var VARIABLE in newRow.TblPurchaseReceiveDetails)
                //{
                //    totallist.Add(VARIABLE);
                //}
                if (totallist.Any(w => w.Qty > 0))
                {
                    PoPlanPurchase(newRow.Iserial, userIserial);

                    ReceivepackingSlip(newRow.Iserial, userIserial);
                }
                if (totallist.Any(w => w.Qty < 0))
                {
                    PoPlanPurchase(newrowRet.Iserial, userIserial);
                    //var header = context.TblPurchaseReceiveHeaders.Include("TblPurchaseOrderHeaderRequest1").Include("TblPurchaseReceiveDetails.TblPurchaseOrderDetailRequest1").FirstOrDefault(w => w.Iserial == newrowRet.Iserial);
                    ReceivepackingSlip(newrowRet.Iserial, userIserial);
                }
                if (totallist.Any(w => w.Qty > 0))
                {
                    var purchaserec = context.TblPurchaseReceiveHeaders.Include("TblPurchaseReceiveDetails").FirstOrDefault(w => w.Iserial == newRow.Iserial);

                    foreach (var item in purchaserec.TblPurchaseReceiveDetails.ToList())
                    {
                        var purchaseOrderDetailRequest = context.TblPurchaseOrderDetailRequests.FirstOrDefault(w => w.Iserial == item.TblPurchaseOrderDetailRequest);
                        purchaseOrderDetailRequest.BatchNo = "";
                        purchaseOrderDetailRequest.ReceiveNow = 0;
                    }
                }
                if (totallist.Any(w => w.Qty < 0))
                {
                    var purchaserec = context.TblPurchaseReceiveHeaders.Include("TblPurchaseReceiveDetails").FirstOrDefault(w => w.Iserial == newrowRet.Iserial);
                    foreach (var item in purchaserec.TblPurchaseReceiveDetails.ToList())
                    {
                        var purchaseOrderDetailRequest = context.TblPurchaseOrderDetailRequests.FirstOrDefault(w => w.Iserial == item.TblPurchaseOrderDetailRequest);
                        purchaseOrderDetailRequest.BatchNo = "";
                        purchaseOrderDetailRequest.ReceiveNow = 0;
                    }
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseReceiveHeader(TblPurchaseReceiveHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseReceiveHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblPurchaseReceiveDetail UpdateOrInsertTblPurchaseReceiveDetail(TblPurchaseReceiveDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseReceiveDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var purchaseorderdetailIserial = newRow.TblPurchaseOrderDetailRequest;
                    newRow.TblPurchaseOrderDetailRequest1 = null;
                    newRow.TblPurchaseOrderDetailRequest = purchaseorderdetailIserial;
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblPurchaseReceiveDetails.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseReceiveDetail(TblPurchaseReceiveDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseReceiveDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        private void ReceivepackingSlip(int Iserial, int userIserial)
        {

            var axapta = new Axapta();
            var transactionGuid = Guid.NewGuid().ToString();
            var credential = new NetworkCredential("bcproxy", "around1");
            var header = new TblPurchaseReceiveHeader();
            using (var model = new WorkFlowManagerDBEntities())
            {
                header =
                    model.TblPurchaseReceiveHeaders.Include("TblPurchaseOrderHeaderRequest1")
                        .Include("TblPurchaseReceiveDetails.TblPurchaseOrderDetailRequest1")
                        .FirstOrDefault(w => w.Iserial == Iserial);

                TblAuthUser userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);

                var warehouse = model.TblWarehouses.FirstOrDefault(w => w.Iserial == header.TblWarehouse);
                if (userToLogin != null)
                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                const string tableName = "AutoPICKING";
                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                foreach (var variable in header.TblPurchaseReceiveDetails)
                {
                    var color = model.TblColors.FirstOrDefault(e => e.Iserial == variable.TblPurchaseOrderDetailRequest1.FabricColor).Code;

                    var purchdim = model.PurchlineInventDimWithoutRecs.Where(w => w.ITEMID == variable.TblPurchaseOrderDetailRequest1.ItemId
                                                                   && w.CONFIGID == color && w.PURCHID == header.TblPurchaseOrderHeaderRequest1.AxPurchase);

                    if (variable.TblPurchaseOrderDetailRequest1.BatchNo != null)
                    {
                        purchdim = purchdim.Where(x => x.INVENTBATCHID == variable.TblPurchaseOrderDetailRequest1.BatchNo);
                    }
                    if (variable.TblPurchaseOrderDetailRequest1.Size != null)
                    {
                        purchdim = purchdim.Where(x => x.INVENTSIZEID == variable.TblPurchaseOrderDetailRequest1.Size);
                    }
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();
                    axaptaRecord.set_Field("DATAAREAID", "Ccm");
                    axaptaRecord.set_Field("TransactionGuid", transactionGuid);
                    axaptaRecord.set_Field("FABRICLOCATION", warehouse.Code);
                    axaptaRecord.set_Field("QTY", Convert.ToDecimal(variable.Qty));
                    axaptaRecord.set_Field("WORKFLOWJOURID", header.Iserial);
                    axaptaRecord.set_Field("LineNum", purchdim.OrderByDescending(w => w.LINENUM).FirstOrDefault().LINENUM);
                    axaptaRecord.Insert();
                }
            }
            CreatePackingSlip(header.DocCode + "_" + header.RefNo, header.TblPurchaseOrderHeaderRequest1.AxPurchase, transactionGuid, userIserial);
        }


        public void PoPlanPurchase(int PurchaseReceiveHeader, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var PurchaseOrderDetails = context.TblPurchaseReceiveDetails.Include("TblPurchaseOrderDetailRequest1").Where(w => w.TblPurchaseReceiveHeader == PurchaseReceiveHeader).ToList();

                var iserial = PurchaseOrderDetails.FirstOrDefault().TblPurchaseOrderDetailRequest1.TblPurchaseOrderHeaderRequest;

                var headerObjToPost = context.TblPurchaseOrderHeaderRequests.Include("TblWarehouse1.TblSite1").Include("TblPurchaseOrderDetailRequests.TblColor").SingleOrDefault(x => x.Iserial == iserial);
                var axapta = new Axapta();

                var credential = new NetworkCredential("bcproxy", "around1");
                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                const string dataarea = "CCM";

                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, dataarea, null, null, null);
                axapta.TTSBegin();

                try
                {
                    var tableName = "PurchTable";
                    var purchId = "P" + "_" + headerObjToPost.Vendor + "_" + headerObjToPost.Iserial;

                    if (headerObjToPost.AxPurchase != purchId)
                    {
                        var inventDim = axapta.CreateAxaptaRecord("InventDim");
                        var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();
                        axaptaRecord.set_Field("PurchId", purchId);
                        if (headerObjToPost.DeliveryDate != null)
                        {
                            axaptaRecord.set_Field("DeliveryDate", headerObjToPost.DeliveryDate);
                        }

                        var header = axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.Vendor) as AxaptaRecord;
                        axaptaRecord.Call("initFromVendTable", header);

                        axaptaRecord.Insert();

                    }

                    tableName = "PurchLine";
                    foreach (var item in PurchaseOrderDetails)
                    {
                        
                        PurchaseFabricLinesToAx(headerObjToPost, item, purchId, userIserial, axapta);
                    }
                    axapta.TTSCommit();
                    headerObjToPost.AxPurchase = purchId;
                    context.SaveChanges();

                    //No errors occured, Commit!
                    //Axapta.TTSCommit();
                }
                catch (Exception ex)
                {
                    //There was some errors, Abort transaction and Raise error!
                    //Axapta.TTSAbort();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //Finally logoff the Axapta Session
                    axapta.Logoff();
                }
            }
        }
        public decimal PurchaseFabricLinesToAx(TblPurchaseOrderHeaderRequest headerObjToPost, TblPurchaseReceiveDetail item, string purchaseOrder, int userIserial, Axapta axapta)
        {
            try
            {
                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                var lineNum = importNew.Call("CreatePurchaseLinesSize", purchaseOrder, item.TblPurchaseOrderDetailRequest1.ItemId, item.Qty, item.BatchNo ?? "",
                    item.TblPurchaseOrderDetailRequest1.TblColor.Code, headerObjToPost.TblWarehouse1.Code, item.TblPurchaseOrderDetailRequest1.Price, headerObjToPost.TblWarehouse1.Code, headerObjToPost.TblWarehouse1.TblSite1.Code, item.TblPurchaseOrderDetailRequest1.Size ?? "");

                return Convert.ToDecimal(lineNum);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void CreatePackingSlip(string packingSlipId, string purchId, string transactionGuid, int userIserial)
        {
            var axapta = new Axapta();
            var credential = new NetworkCredential("bcproxy", "around1");
            TblAuthUser userToLogin;
            using (var model = new WorkFlowManagerDBEntities())
            {
                userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
            }
            if (userToLogin != null)
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

            importNew.Call("PostPurchFormLetter", packingSlipId, purchId, transactionGuid);

            axapta.Logoff();
        }
    }
}