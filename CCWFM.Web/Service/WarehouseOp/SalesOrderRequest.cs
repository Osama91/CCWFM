using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Service.Operations;
using System;
using LinqKit;
using System.Linq.Dynamic;
using CCWFM.Web.Model;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using CCWFM.Models.Inv;

namespace CCWFM.Web.Service.WarehouseOp
{
    public partial class WarehouseService
    {

        [OperationContract]
        private List<TblTransferHeader> GetTransferByWarehouseTo(int skip,int take,int WarehouseTo,int item,string ItemType,string Code,
         int userIserial, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var predicate = PredicateBuilder.True<TblTransferHeader>();
                if (WarehouseTo != 0)
                {
                    predicate = predicate.And(i => i.WarehouseFrom == WarehouseTo);
                }
                if (item != 0)
                {
                    predicate = predicate.And(i => i.TblTransferDetails.Any(w => w.TblItemDim1.Iserial == item && w.TblItemDim1.ItemType == ItemType));
                }

                if (!string.IsNullOrWhiteSpace(Code))
                {
                    predicate = predicate.And(i => i.CodeFrom == Code);
                }


                var defaultQuery = context.TblTransferHeaders.Include(nameof(
                    TblTransferHeader.TblWarehouseFrom)).Include(nameof(
                        TblTransferHeader.TblWarehouseTo)).Where(predicate);
                IQueryable<TblTransferHeader> query;

              
                
                //predicate = predicate.And(i => i.TblItemDim1.ItemType == ItemType);
                //predicate = predicate.And(i => i.DocDate <= docDate);

                fullCount = defaultQuery.Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo))).Count();
                    query = defaultQuery.Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo)));
                
                return query.Skip(skip).Take(take).ToList();
            }
        }


        [OperationContract]
        private List<TblSalesOrderHeaderRequest> GetTblSalesOrderHeaderRequest(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out int fullCount, out List<Model.Entity> entityList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderHeaderRequest> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrderHeaderRequests.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderHeaderRequests.Include("TblSalesPerson1").Include("TblWarehouse1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblSalesOrderHeaderRequests.Count();
                    query = context.TblSalesOrderHeaderRequests.Include("TblSalesPerson1").Include("TblWarehouse1").OrderBy(sort).Skip(skip).Take(take);
                }

                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var entity = new Model.ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderHeaderRequest UpdateOrInsertTblSalesOrderHeaderRequest(TblSalesOrderHeaderRequest newRow, int index, int userIserial, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblSalesOrderHeaderRequests.Include(nameof(TblSalesOrderHeaderRequest.TblSalesOrderDetailRequests)).FirstOrDefault(th => th.Iserial == newRow.Iserial);

                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;

                        // هحذف الى اتحذف
                        foreach (var item in oldRow.TblSalesOrderDetailRequests)
                        {
                            if (!newRow.TblSalesOrderDetailRequests.Any(td => td.Iserial == item.Iserial))// مش موجود فى الجديد يبقى اتحذف
                                DeleteTblSalesOrderDetailRequest(item);
                        }

                        foreach (var item in newRow.TblSalesOrderDetailRequests.ToArray())
                        {
                            item.TblSalesOrderHeaderRequest = newRow.Iserial;
                            // هشوف بقى الى اتعدل والجديد
                            int temp;//item.ItemAdjustment                       
                            if (item.Iserial!=0)
                            {
                                UpdateOrInsertTblSalesOrderDetailRequest(item, 1, out temp);
                            }
                            else
                            {
                                context.TblSalesOrderDetailRequests.AddObject(item);
                            }
                           
                        }
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                        //GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {
                        var warehouse = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.TblWarehouse);
                        var seq = warehouse.Po;
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seq);
                        newRow.Code = SharedOperation.HandelSequence(seqRow);

                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;

                        context.TblSalesOrderHeaderRequests.AddObject(newRow);
                    }
                    context.SaveChanges();
                    foreach (var item in newRow.TblSalesOrderDetailRequests)
                    {
                        GetTblSalesOrderDetailRequestDetails(context, item);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderHeaderRequest(TblSalesOrderHeaderRequest row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderHeaderRequests
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesOrderHeaderRequests.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSalesOrderDetailRequest> GetTblSalesOrderDetailRequest(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderDetailRequest> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblSalesOrderHeaderRequest=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrderDetailRequests.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderDetailRequests.Include(nameof(TblSalesOrderDetailRequest.TblSalesOrderHeaderRequest1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesOrderDetailRequests.Count(x => x.TblSalesOrderHeaderRequest == groupId);
                    query = context.TblSalesOrderDetailRequests.Include(nameof(TblSalesOrderDetailRequest.TblSalesOrderHeaderRequest1)).OrderBy(sort).Where(v => v.TblSalesOrderHeaderRequest == groupId).Skip(skip).Take(take);
                }
                var result = query.ToList();
                foreach (var item in result)
                {
                    GetTblSalesOrderDetailRequestDetails(context, item);
                }

                return result;
            }
        }


        private static void GetTblSalesOrderDetailRequestDetails(WorkFlowManagerDBEntities context, TblSalesOrderDetailRequest item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.TblItemDim);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemTransfer.ItemId = itemDimFromResult.ItemIserial;
            item.ItemTransfer.ItemName = itemDimFromResult.ItemName;
            item.ItemTransfer.ItemType = itemDimFromResult.ItemType;
            item.ItemTransfer.TransferredQuantity = item.Qty ?? 0;

            item.ItemTransfer.ItemDimFromIserial = item.TblItemDim;
            item.ItemTransfer.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemTransfer.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemTransfer.SizeFrom = itemDimFromResult.Size;
            item.ItemTransfer.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemTransfer.SiteFromIserial = itemDimFromResult.SiteIserial;

            string warehouseCode = context.TblWarehouses.FirstOrDefault(tw =>
            tw.Iserial == item.TblSalesOrderHeaderRequest1.TblWarehouse).Code;
            var itemdimfrom = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.TblItemDim);
            string itemCode = context.FabricAccSearches.FirstOrDefault(fas =>
                fas.Iserial == itemdimfrom.ItemIserial && fas.ItemGroup == itemdimfrom.ItemType).Code;
            string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimfrom.TblColor).Code;
            item.ItemTransfer.AvailableQuantity = WarehouseQuantities.GetAvilableQuantity(
                 warehouseCode, itemCode, colorCode, itemdimfrom.Size, itemdimfrom.BatchNo);
        }

        [OperationContract]
        private TblSalesOrderDetailRequest UpdateOrInsertTblSalesOrderDetailRequest(TblSalesOrderDetailRequest newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderDetailRequests
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {

                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {

                    context.TblSalesOrderDetailRequests.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderDetailRequest(TblSalesOrderDetailRequest row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderDetailRequests
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesOrderDetailRequests.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSalesIssueHeader> GetTblSalesIssueHeader(int skip, int take, int groupId, int journalaccounttype, int EntityAccount, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesIssueHeader> query;

                var list = context.TblSalesOrderRequestInvoiceDetails.Select(w => w.TblSalesIssueHeader).ToList();
                if (filter != null)
                {
                    if (groupId != 0)
                    {
                        filter = filter + " and it.TblSalesOrderHeaderRequest=(@group)";
                        valuesObjects.Add("group", groupId);
                    }

                    if (journalaccounttype != 0)
                    {
                        filter = filter + " and it.TblJournalAccountType=(@TblJournalAccountType)";
                        valuesObjects.Add("TblJournalAccountType", journalaccounttype);

                        foreach (var item in list)
                        {
                            filter = filter + " and it.iserial !=(@IserialTemp" + item + ")";
                            valuesObjects.Add("IserialTemp" + item + "", item);
                        }
                    }
                    if (EntityAccount != 0)
                    {
                        filter = filter + " and it.EntityAccount=(@EntityAccount)";
                        valuesObjects.Add("EntityAccount", EntityAccount);

                    }


                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblSalesIssueHeaders.Include(nameof(TblSalesIssueHeader.TblWarehouse1)).Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesIssueHeaders.Include(nameof(TblSalesIssueHeader.TblWarehouse1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesIssueHeaders.Include(nameof(TblSalesIssueHeader.TblWarehouse1)).Count(
                        x =>
                        !list.Contains(x.Iserial)
                        &&

                        (x.TblSalesOrderHeaderRequest == groupId || groupId == 0) && (x.TblJournalAccountType == journalaccounttype || journalaccounttype == 0


                    ) && (x.EntityAccount == EntityAccount || EntityAccount == 0));
                    query = context.TblSalesIssueHeaders.Include(nameof(TblSalesIssueHeader.TblWarehouse1)).OrderBy(sort).Where(x =>
                       !list.Contains(x.Iserial)
                        &&
                    (x.TblSalesOrderHeaderRequest == groupId || groupId == 0) && (x.TblJournalAccountType == journalaccounttype || journalaccounttype == 0) && (x.EntityAccount == EntityAccount || EntityAccount == 0)).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblSalesIssueDetail> GetTblSalesIssueDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesIssueDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblSalesIssueHeader=(@group)";
                    valuesObjects.Add("group", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesIssueDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesIssueDetails.Include("TblSalesOrderDetailRequest1.TblItemDim1.TblColor1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesIssueDetails.Count(x => x.TblSalesIssueHeader == groupId);
                    query = context.TblSalesIssueDetails.Include("TblPurchaseOrderDetailRequest1.TblItemDim1.TblColor1").OrderBy(sort).Where(v => v.TblSalesIssueHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        //[OperationContract]
        //private List<TblPurchaseOrderDetail> SearchPurchaseOrderDetail(string Vendor, string ItemID, DateTime? FromDate, DateTime? ToDate)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        IQueryable<TblPurchaseOrderDetail> query;

        //        query = context.TblPurchaseOrderDetails.Include("TblColor").Where(v =>
        //        (!context.TblPurchaseOrderDetailRequests.Any(e => e.TblPurchaseOrderDetail == v.Iserial)) &&
        //        v.Qty > 0 && v.TblPurchaseOrderHeader1.TblPurchaseHeaderType == 0 && v.TblPurchaseOrderHeader1.Vendor == Vendor && (v.ItemId == ItemID || ItemID == null) && (v.TblPurchaseOrderHeader1.TransDate >= FromDate || FromDate == null) && (v.TblPurchaseOrderHeader1.TransDate <= ToDate || ToDate == null));
        //        return query.ToList();
        //    }
        //}


        [OperationContract]
        private TblSalesIssueHeader UpdateOrInsertTblSalesIssueHeader(TblSalesIssueHeader newRow, int index, out int outindex, int userIserial)
        {
            outindex = index;
            var totallist = new List<TblSalesIssueDetail>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                var newrowRet = new TblSalesIssueHeader();
                var oldRow = (from e in context.TblSalesIssueHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.CreatedBy = oldRow.CreatedBy;
                    newRow.CreationDate = oldRow.CreationDate;
                    newRow.LastUpdatedBy = userIserial;
                    newRow.LastUpdatedDate = DateTime.Now;
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    var warehouse = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.TblWarehouse);

                    foreach (var VARIABLE in newRow.TblSalesIssueDetails)
                    {
                        totallist.Add(VARIABLE);
                    }
                    newRow.TblSalesIssueDetails.Clear();

                    if (totallist.Any(w => w.Qty > 0))
                    {
                        foreach (var variable in totallist.Where(x => x.Qty > 0))
                        {
                            newRow.TblSalesIssueDetails.Add(variable);
                        }

                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == warehouse.Sales);
                        newRow.DocCode = SharedOperation.HandelSequence(seqRow);
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.TblInventType = 7;

                        context.TblSalesIssueHeaders.AddObject(newRow);
                    }
                    if (totallist.Any(w => w.Qty < 0))
                    {
                        newrowRet = new TblSalesIssueHeader
                        {
                            DocDate = newRow.DocDate,
                            RefNo = newRow.RefNo,
                            Notes = newRow.Notes,
                            TblWarehouse = newRow.TblWarehouse,
                            TblJournalAccountType = newRow.TblJournalAccountType,
                            EntityAccount = newRow.EntityAccount,
                            TblSalesOrderHeaderRequest = newRow.TblSalesOrderHeaderRequest,
                            Iserial = 0,
                        };
                        newrowRet.TblSalesIssueDetails.Clear();
                        foreach (var variable in totallist.Where(x => x.Qty < 0))
                        {
                            newrowRet.TblSalesIssueDetails.Add(variable);
                        }

                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == warehouse.RetSales);
                        newrowRet.DocCode = SharedOperation.HandelSequence(seqRow);
                        newrowRet.CreatedBy = userIserial;
                        newrowRet.CreationDate = DateTime.Now;
                        newrowRet.TblInventType = 7;
                        context.TblSalesIssueHeaders.AddObject(newrowRet);
                    }
                }
                context.SaveChanges();
                if (totallist.Any(w => w.Qty > 0))
                {
                    //  PoPlanPurchase(newRow.Iserial, userIserial);
                    var header =
                        context.TblSalesIssueHeaders
                            .Include("TblSalesIssueDetails.TblSalesOrderDetailRequest1")
                            .FirstOrDefault(w => w.Iserial == newRow.Iserial);
                    //    ReceivepackingSlip(header, userIserial);
                }
                if (totallist.Any(w => w.Qty < 0))
                {
                    var header = context.TblSalesIssueHeaders.Include("TblSalesIssueDetails.TblSalesOrderDetailRequest1").FirstOrDefault(w => w.Iserial == newrowRet.Iserial);
                    //      ReceivepackingSlip(header, userIserial);
                }
                if (totallist.Any(w => w.Qty > 0))
                {
                    var purchaserec = context.TblSalesIssueHeaders.Include("TblSalesIssueDetails").FirstOrDefault(w => w.Iserial == newRow.Iserial);

                    foreach (var item in purchaserec.TblSalesIssueDetails.ToList())
                    {
                        //var purchaseOrderDetailRequest = context.TblPurchaseOrderDetailRequests.FirstOrDefault(w => w.Iserial == item.TblPurchaseOrderDetailRequest);
                        //purchaseOrderDetailRequest.BatchNo = "";
                        //purchaseOrderDetailRequest.ReceiveNow = 0;
                    }
                }
                if (totallist.Any(w => w.Qty < 0))
                {
                    var purchaserec = context.TblSalesIssueHeaders.Include("TblSalesIssueDetails").FirstOrDefault(w => w.Iserial == newrowRet.Iserial);
                    foreach (var item in purchaserec.TblSalesIssueDetails.ToList())
                    {
                        //var purchaseOrderDetailRequest = context.TblPurchaseOrderDetailRequests.FirstOrDefault(w => w.Iserial == item.TblPurchaseOrderDetailRequest);
                        //purchaseOrderDetailRequest.BatchNo = "";
                        //purchaseOrderDetailRequest.ReceiveNow = 0;
                    }
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesIssueHeader(TblSalesIssueHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesIssueHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesIssueHeaders.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblSalesIssueDetail UpdateOrInsertTblSalesIssueDetail(TblSalesIssueDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesIssueDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var purchaseorderdetailIserial = newRow.TblSalesOrderDetailRequest;
                    newRow.TblSalesOrderDetailRequest1 = null;
                    newRow.TblSalesOrderDetailRequest = purchaseorderdetailIserial;
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblSalesIssueDetails.AddObject(newRow);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesIssueDetail(TblSalesIssueDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesIssueDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesIssueDetails.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSalesOrderRequestInvoiceHeader> GetTblSalesOrderRequestInvoiceHeader(int skip, int take, int TblSalesOrderHeaderRequest, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out int fullCount, out List<Model.Entity> entityList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (TblSalesOrderHeaderRequest != 0)
                {
                    filter = filter + " and it.TblSalesOrderHeaderRequest=(@group)";
                    valuesObjects.Add("group", TblSalesOrderHeaderRequest);
                }


                IQueryable<TblSalesOrderRequestInvoiceHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrderRequestInvoiceHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderRequestInvoiceHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblSalesOrderRequestInvoiceHeaders.Count(x => x.TblSalesOrderHeaderRequest == TblSalesOrderHeaderRequest || TblSalesOrderHeaderRequest == 0);
                    query = context.TblSalesOrderRequestInvoiceHeaders.OrderBy(sort).Where(v => v.TblSalesOrderHeaderRequest == TblSalesOrderHeaderRequest || TblSalesOrderHeaderRequest == 0).Skip(skip).Take(take);

                }

                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderRequestInvoiceHeader UpdateOrInsertTblSalesOrderRequestInvoiceHeader(TblSalesOrderRequestInvoiceHeader newRow, int index, int userIserial, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblSalesOrderRequestInvoiceHeaders.Include(nameof(TblSalesOrderRequestInvoiceHeader.TblSalesOrderRequestInvoiceDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);

                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;

                        // هحذف الى اتحذف
                        foreach (var item in oldRow.TblSalesOrderRequestInvoiceDetails)
                        {
                            if (!newRow.TblSalesOrderRequestInvoiceDetails.Any(td => td.Iserial == item.Iserial))// مش موجود فى الجديد يبقى اتحذف
                                DeleteTblSalesOrderRequestInvoiceDetail(item);
                        }
                        foreach (var item in newRow.TblSalesOrderRequestInvoiceDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp, headeriserial;//item.ItemAdjustment
                            headeriserial = item.TblSalesOrderRequestInvoiceHeader ?? 0;
                            item.TblSalesOrderRequestInvoiceHeader = null;
                            item.TblSalesOrderRequestInvoiceHeader = headeriserial;
                            UpdateOrInsertTblSalesOrderRequestInvoiceDetail(item, 1, out temp);
                        }
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {
                        var seqCode = SharedOperation.GetChainSetup("SalesOrderInvoiceSequence");
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Code == seqCode);
                        newRow.Code = SharedOperation.HandelSequence(seqRow);
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;
                        //  context.TblSalesOrderRequestInvoiceHeaders.Detach(newRow);
                        context.TblSalesOrderRequestInvoiceHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                    foreach (var item in newRow.TblSalesOrderRequestInvoiceDetails)
                    {
                        //       GetTblSalesOrderDetailRequestDetails(context, item);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderRequestInvoiceHeader(TblSalesOrderRequestInvoiceHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderRequestInvoiceHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesOrderRequestInvoiceHeaders.DeleteObject(oldRow);

                context.SaveChanges();
            }
            
            return row.Iserial;
            
        }
        

        [OperationContract]
        private List<TblSalesOrderRequestInvoiceDetail> GetTblSalesOrderRequestInvoiceDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out int fullCount, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderRequestInvoiceDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblSalesOrderRequestInvoiceHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblSalesOrderRequestInvoiceDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderRequestInvoiceDetails.Include("TblItemDim1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblSalesOrderRequestInvoiceDetails.Count(w => w.TblSalesOrderRequestInvoiceHeader == groupId);
                    query = context.TblSalesOrderRequestInvoiceDetails.Include("TblItemDim1").OrderBy(sort).Where(v => v.TblSalesOrderRequestInvoiceHeader == groupId).Skip(skip).Take(take);
                }

                itemsList = (from w in context.FabricAccSearches
                       .Where(w => query.Any(e => e.TblItemDim1.ItemType == w.ItemGroup && e.TblItemDim1.ItemIserial == w.Iserial))
                             select new ItemsDto()
                             {
                                 Iserial = w.Iserial,
                                 Name = w.Name,
                                 ItemGroup = w.ItemGroup,
                                 Code = w.Code,
                             }).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderRequestInvoiceDetail UpdateOrInsertTblSalesOrderRequestInvoiceDetail(TblSalesOrderRequestInvoiceDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblSalesOrderRequestInvoiceDetails.FirstOrDefault(th => th.Iserial == newRow.Iserial);

                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                        //GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {

                        context.TblSalesOrderRequestInvoiceDetails.AddObject(newRow);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderRequestInvoiceDetail(TblSalesOrderRequestInvoiceDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderRequestInvoiceDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblSalesOrderRequestInvoiceDetails.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSalesIssueHeader> GetSalesIssueHeaderPending(int JournalAccountType, int EntityAccount, DateTime? fromDate, DateTime? toDate)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var list = entity.TblSalesOrderRequestInvoiceDetails.Select(w => w.TblSalesIssueHeader).ToList();
                var query =
                    entity.TblSalesIssueHeaders
                        .Where(x => x.EntityAccount == EntityAccount && x.TblJournalAccountType == JournalAccountType &&
                        (x.DocDate >= fromDate || fromDate == null)
                        && !list.Contains(x.Iserial)
                        && (x.DocDate <= toDate || toDate == null)
                        );
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderRequestInvoiceHeader SearchSalesOrderRequestInvoice(TblSalesOrderRequestInvoiceHeader newrow, int userIserial, List<int> SalesIssueHeaderList)
        {
            var result = new List<TblSalesOrderRequestInvoiceDetail>();

            using (var context = new WorkFlowManagerDBEntities())
            {
                int temp = 0;
                UpdateOrInsertTblSalesOrderRequestInvoiceHeader(newrow, 0, userIserial, out temp);
                var query = (from p in context.TblSalesIssueDetails.Include("TblSalesOrderDetailRequest1.TblItemDim1.TblColor1")
                    .Where(w => SalesIssueHeaderList.Any(e => e == w.TblSalesIssueHeader)
                        &&
                        !context.TblSalesOrderRequestInvoiceDetails.Any(e => e.TblSalesIssueHeader == w.TblSalesIssueHeader)
                        )
                    .GroupBy(e =>
                            new
                            {
                                e.TblSalesOrderDetailRequest1.TblItemDim1,
                                e.TblSalesIssueHeader
                            })
                             select new
                             {
                                 Qty = p.Sum(v => v.Qty),
                                 TblItemDim1 = p.Key.TblItemDim1,
                                 Misc = 0,
                                 Price = p.FirstOrDefault().Price,
                                 TblSalesIssueHeader = p.Key.TblSalesIssueHeader,
                                 TblSalesOrderRequestInvoiceHeader = newrow.Iserial,
                             }).ToList();
                foreach (var item in query)
                {
                    result.Add(new TblSalesOrderRequestInvoiceDetail()
                    {
                        Qty = item.Qty,
                        TblItemDim1 = item.TblItemDim1,
                        Misc = 0,
                        Price = item.Price,
                        TblSalesIssueHeader = item.TblSalesIssueHeader,
                        TblSalesOrderRequestInvoiceHeader = newrow.Iserial,
                    });

                }
                //  newrow.TblSalesOrderRequestInvoiceDetails = new System.Data.Objects.DataClasses.EntityCollection<TblSalesOrderRequestInvoiceDetail>();
                foreach (var item in result.ToList())
                {
                    context.TblSalesOrderRequestInvoiceDetails.AddObject(item);
                }
                context.SaveChanges();
            }

            return newrow;
        }

        [OperationContract]
        private List<TblSalesOrderRequestInvoiceHeaderMarkupTransProd> GetTblSalesOrderRequestInvoiceMarkupTransProd(int type, int TblSalesOrderRequestInvoiceHeader,string company, out List<Model.Entity> entityList)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderRequestInvoiceHeaderMarkupTransProd> query = entity.TblSalesOrderRequestInvoiceHeaderMarkupTransProds.Where(x => x.TblSalesOrderRequestInvoiceHeader == TblSalesOrderRequestInvoiceHeader && x.Type == type);


                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderRequestInvoiceHeaderMarkupTransProd UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProds(TblSalesOrderRequestInvoiceHeaderMarkupTransProd newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                
                if (save)
                {
                    entity.TblSalesOrderRequestInvoiceHeaderMarkupTransProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblSalesOrderRequestInvoiceHeaderMarkupTransProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderRequestInvoiceMarkupTransProds(TblSalesOrderRequestInvoiceHeaderMarkupTransProd row)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblSalesOrderRequestInvoiceHeaderMarkupTransProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblSalesOrderRequestInvoiceHeader PostTblSalesOrderRequestInvoiceHeader(TblSalesOrderRequestInvoiceHeader row, int user, string company)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                var glserive = new Operations.GlOperations.GlService();
                short? lang = 0;
                using (var entity = new WorkFlowManagerDBEntities())
                {
                    var firstOrDefault = entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user);
                    if (firstOrDefault != null)
                        lang = firstOrDefault.CurrLang;

                    entity.CommandTimeout = 0;
                    var query = entity.TblSalesOrderRequestInvoiceHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Sales TransNo " + row.Code;
                    var markuptrans =
                        entity.TblSalesOrderRequestInvoiceHeaderMarkupTransProds
                            .Where(x => x.TblSalesOrderRequestInvoiceHeader == row.Iserial && x.Type == 0);

                    double cost =
                         (double)entity.TblSalesOrderRequestInvoiceDetails.Where(x => x.TblSalesOrderRequestInvoiceHeader == row.Iserial).Sum(w => w.Price * w.Qty);

                    double totalWithItemEffect = 0;
                    double totalWithoutItemEffect = 0;
                    foreach (var variable in markuptrans)
                    {
                        var markupRow=  glserive.getMarkUpByIserial(variable.TblMarkupProd, company);
                        if (markupRow.ItemEffect == false)
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithoutItemEffect = (double)(totalWithoutItemEffect + (cost * (variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithoutItemEffect = (double)(totalWithoutItemEffect + variable.MiscValue);
                            }
                        }
                        else
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithItemEffect = (double)(totalWithItemEffect + (cost * (variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithItemEffect = (double)(totalWithItemEffect + variable.MiscValue);
                            }
                        }
                    }
                    if (query != null)
                    {
                        query.MiscWithoutItemEffect = totalWithoutItemEffect;
                        query.Misc = totalWithItemEffect;
                        if (totalWithItemEffect != 0)
                        {
                            var queryDetail =
                                entity.TblSalesOrderRequestInvoiceDetails.Where(x => x.TblSalesOrderRequestInvoiceHeader == row.Iserial).ToList();
                            foreach (var variable in queryDetail)
                            {
                                variable.Misc = (decimal)(((double)variable.Price / cost) * totalWithItemEffect);
                            }
                        }
                        entity.SaveChanges();
                        if (query != null)
                        {
                            query.Status = 1;
                            query.CreatedBy = user;
                            query.PostDate = DateTime.Now;
                            using (var db = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                            {
                                // var ledgercode = entity.tblChainSetupTests.FirstOrDefault(x => x.sSetupValue == "GLSalesInventory").sSetupValue;
                                var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlSalesInvoice").sSetupValue;

                                int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderProdRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = desc,
                                    DocDate = row.DocDate,
                                    TblJournal = journalint,
                                    TblTransactionType = 11,
                                    TblJournalLink = query.Iserial
                                };
                                int temp;
                                
                                glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);

                                var sqlParam = new List<SqlParameter>{
                                new SqlParameter
                                {
                                    ParameterName = "Iserial",
                                    Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
                                    SqlDbType = SqlDbType.NVarChar
                                },};

                                var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlSalesInvoicePostingToGl  @Iserial",
                                 sqlParam.ToArray()).ToList();


                                #region MarkUp

                                foreach (var rr in markuptrans)
                                {
                                    var markupRow = glserive.getMarkUpByIserial(rr.TblMarkupProd, company);
                                    var currencyrow = db.TblCurrencyTests.First(w => w.Iserial == rr.TblCurrency);


                                    var glAccount =
                                        db.Entities.FirstOrDefault(
                                            x => x.Iserial == rr.TblMarkupProd && x.TblJournalAccountType == 9).AccountIserial;
                                    var vendorAccountMarkUp =
                                        db.Entities.FirstOrDefault(
                                            x => x.Iserial == rr.EntityAccount && x.TblJournalAccountType == rr.TblJournalAccountType);

                                    var drorCr = true;
                                    double? total = 0;
                                    if (rr.MiscValueType == 0)
                                    {
                                        total = (total + cost * (rr.MiscValue / 100)) * markupRow.TblMarkupGroup1.Direction;
                                    }
                                    else
                                    {
                                        total = (total + rr.MiscValue) * markupRow.TblMarkupGroup1.Direction;
                                    }
                                    if (total > 0)
                                    {
                                        drorCr = false;
                                    }
                                    var markupdes = markupRow.Ename + query.Code;
                                    if (lang == 0)
                                    {
                                        markupdes = markupRow.Aname + query.Code;
                                    }
                                    decimal totalModified = (decimal)total;
                                    if (total < 0)
                                    {
                                        totalModified = (decimal)(total * -1);
                                    }
                                    var markupVendorDiscount = new TblLedgerMainDetail();

                                    markupVendorDiscount = new TblLedgerMainDetail
                                    {
                                        Amount = totalModified,
                                        Description = markupdes,
                                        ExchangeRate = currencyrow.ExchangeRate,
                                        TblCurrency = rr.TblCurrency,
                                        TransDate = row.DocDate,
                                        TblJournalAccountType = 0,
                                        EntityAccount = glAccount,
                                        GlAccount = glAccount,
                                        TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                        PaymentRef = query.Code,
                                        DrOrCr = !drorCr
                                    };


                                    glserive.UpdateOrInsertTblLedgerMainDetails(markupVendorDiscount, true, 000, out temp, company,
                                         user);

                                    //            CreateTblLedgerDetailCostCenter(company, (decimal)total, markupVendorDiscount, storeCostcenter);

                                    if (glAccount != 0)
                                    {
                                        var markupVendor = new TblLedgerMainDetail
                                        {
                                            Amount = totalModified,
                                            Description = markupdes,
                                            ExchangeRate = currencyrow.ExchangeRate,
                                            TblCurrency = rr.TblCurrency,
                                            TransDate = row.DocDate,
                                            TblJournalAccountType = query.TblJournalAccountType,
                                            EntityAccount = vendorAccountMarkUp.Iserial,
                                            GlAccount = vendorAccountMarkUp.AccountIserial,
                                            TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                            PaymentRef = query.Code,
                                            DrOrCr = drorCr
                                        };

                                        glserive.UpdateOrInsertTblLedgerMainDetails(markupVendor, true, 000, out temp, company, user);

                                        //CreateTblLedgerDetailCostCenter(company, (decimal)total, markupVendor, storeCostcenter);

                                        //foreach (var variable in list)
                                        //{
                                        //    var costcenter = new TblGlRuleDetail();
                                        //    costcenter = glserive.FindCostCenterByType(costcenter, 0, (int)variable.GroupName,
                                        //        company);

                                        //    var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                                        //    {
                                        //        Ratio = 0,
                                        //        TblLedgerMainDetail = markupVendor.Iserial,
                                        //        Amount = (double)markupVendor.Amount * variable.CostPercentage,
                                        //        TblCostCenter = costcenter.TblCostCenter,
                                        //        TblCostCenterType = costcenter.TblCostCenter1.TblCostCenterType,
                                        //    };
                                        //    glserive.UpdateOrInsertTblLedgerDetailCostCenters(markupVendorLedgerCostCenter, true, 000,
                                        //         out temp, company);
                                        //}
                                    }
                                }

                                #endregion MarkUp

                                foreach (var rr in list.GroupBy(x => x.GroupName))
                                {
                                    glserive.PostInvPurchaseAndTax(query, newLedgerHeaderProdRow, rr, company, user, list, desc);
                                }
                                glserive.CorrectLedgerHeaderRouding(newLedgerHeaderProdRow.Iserial, company, user);
                            }

                            entity.SaveChanges();
                            scope.Complete();
                        }
                        return query;
                    }
                    return null;
                }
            }
        }
    }
}