using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Data;
using CCWFM.Web.Service.Operations;
using System.Data.SqlClient;
using System.Globalization;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        [OperationContract]
        private List<TblProductionOrderHeader> GetTblProductionOrderHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblProductionOrderHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderHeaders.Include("TblItemDim1.TblColor1").Include("TblWarehouse1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblProductionOrderHeaders.Count();
                    query = context.TblProductionOrderHeaders.Include("TblItemDim1.TblColor1").Include("TblWarehouse1").OrderBy(sort).Skip(skip).Take(take);
                }

                //var codes = query.Select(w => w.Vendor).ToList();
                //Vendors = context.Vendors.Where(x => codes.Contains(x.vendor_code)).ToList();
                //var result = query.ToList();
                //foreach (var item in result)
                //{
                //    GetTblSalesOrderDetailRequestDetails(context, item);
                //}
                return query.ToList();
            }
        }

        private static void GetTblSalesOrderDetailRequestDetails(WorkFlowManagerDBEntities context, TblProductionOrderHeader item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.TblItemDim);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemTransfer.ItemId = itemDimFromResult.ItemIserial;
            item.ItemTransfer.ItemCode = context.FabricAccSearches.FirstOrDefault(fas =>
          fas.Iserial == itemDimFromResult.ItemIserial && fas.ItemGroup == itemDimFromResult.ItemType).Code;
            item.ItemTransfer.ItemName = itemDimFromResult.ItemName;
            item.ItemTransfer.ItemType = itemDimFromResult.ItemType;
            item.ItemTransfer.TransferredQuantity = item.Qty ?? 0;

            item.ItemTransfer.ItemDimFromIserial = item.TblItemDim ?? 0;
            item.ItemTransfer.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemTransfer.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemTransfer.SizeFrom = itemDimFromResult.Size;
            item.ItemTransfer.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemTransfer.SiteFromIserial = itemDimFromResult.SiteIserial;

            //string warehouseCode = context.TblWarehouses.FirstOrDefault(tw =>
            //tw.Iserial == item.TblWarehouse).Code;
            //var itemdimfrom = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.TblItemDim);
            //string itemCode = context.FabricAccSearches.FirstOrDefault(fas =>
            //    fas.Iserial == itemdimfrom.ItemIserial && fas.ItemGroup == itemdimfrom.ItemType).Code;
            //string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimfrom.TblColor).Code;
            //item.ItemTransfer.AvailableQuantity = WarehouseQuantities.GetAvilableQuantity(
            //     warehouseCode, itemCode, colorCode, itemdimfrom.Size, itemdimfrom.BatchNo);
        }

        private static void GetTblSalesOrderDetailRequestDetails(WorkFlowManagerDBEntities context, TblProductionOrderFabric item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.TblItemDim);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemTransfer.ItemId = itemDimFromResult.ItemIserial;
            item.ItemTransfer.ItemCode = context.FabricAccSearches.FirstOrDefault(fas =>
          fas.Iserial == itemDimFromResult.ItemIserial && fas.ItemGroup == itemDimFromResult.ItemType).Code;
            item.ItemTransfer.ItemName = itemDimFromResult.ItemName;
            item.ItemTransfer.ItemType = itemDimFromResult.ItemType;
            item.ItemTransfer.TransferredQuantity = item.Qty ?? 0;

            item.ItemTransfer.ItemDimFromIserial = item.TblItemDim ?? 0;
            item.ItemTransfer.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemTransfer.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemTransfer.SizeFrom = itemDimFromResult.Size;
            item.ItemTransfer.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemTransfer.SiteFromIserial = itemDimFromResult.SiteIserial;

            //string warehouseCode = context.TblWarehouses.FirstOrDefault(tw =>
            //tw.Iserial == item.TblWarehouse).Code;
            //var itemdimfrom = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.TblItemDim);
            //string itemCode = context.FabricAccSearches.FirstOrDefault(fas =>
            //    fas.Iserial == itemdimfrom.ItemIserial && fas.ItemGroup == itemdimfrom.ItemType).Code;
            //string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimfrom.TblColor).Code;
            //item.ItemTransfer.AvailableQuantity = WarehouseQuantities.GetAvilableQuantity(
            //     warehouseCode, itemCode, colorCode, itemdimfrom.Size, itemdimfrom.BatchNo);
        }


        //[OperationContract]
        //private List<TblProductionOrderTransactionDto> SearchDyeingOrderInvoice(string vendor, DateTime? fromDate, DateTime? toDate, int transId, string supplierInvoice)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {

        //        var predicate = PredicateBuilder.True<DyeingOrderDetailsService>();

        //        if (!string.IsNullOrEmpty(supplierInvoice))
        //            predicate = predicate.And(i => i.TblDyeingOrdersDetail.SupplierInv == supplierInvoice);


        //        if (!string.IsNullOrEmpty(vendor))
        //            predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.Vendor == vendor);

        //        if (fromDate != null)
        //            predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.TransactionDate >= fromDate);
        //        if (toDate != null)
        //            predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.TransactionDate <= toDate);
        //        predicate = predicate.And(i => i.TblDyeingOrdersDetail.TransactionType == 1);
        //        if (transId != 0)
        //            predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersMainDetail.Iserial == transId);

        //        predicate = predicate.And(i => i.TblDyeingOrdersDetail.TransactionType == 1 &&
        //         !context.TblProductionOrderTransactions.Any(e => e.TblDyeingOrdersMainDetails == i.TblDyeingOrdersDetail.Iserial));
        //        var query = (from p in context.DyeingOrderDetailsServices.AsExpandable()
        //            .Where(predicate)
        //            .GroupBy(e =>
        //                    new
        //                    {
        //                        e.TblDyeingOrdersDetail.TblDyeingOrdersMainDetail.Iserial,
        //                        e.TblDyeingOrdersDetail.Color,
        //                        e.ServiceCode,
        //                        e.ServiceName,
        //                        e.TblDyeingOrdersDetail.SupplierInv,
        //                        e.TblDyeingOrdersDetail.BatchNo,
        //                        e.TblDyeingOrdersDetail.DyedFabric
        //                    })
        //                     select new TblProductionOrderTransactionDto
        //                     {
        //                         Qty = p.Sum(v => v.TblDyeingOrdersDetail.CalculatedTotalQty),
        //                         TblDyeingOrdersMainDetails = p.Key.Iserial,
        //                         TblProductionOrderHeader = 0,
        //                         TblColor = context.TblColors.FirstOrDefault(w => w.Code == p.Key.Color && w.TblLkpColorGroup != 24).Iserial,
        //                         ColorCode = p.Key.Color,
        //                         TblService = context.TblServices.FirstOrDefault(w => w.Code == p.Key.ServiceCode).Iserial,
        //                         ServiceCode = p.Key.ServiceCode,
        //                         TransID = p.Key.Iserial,
        //                         Cost = p.Sum(v => v.Qty),
        //                         ServiceName = p.Key.ServiceName,
        //                         SupplierInv = p.Key.SupplierInv,
        //                         BatchNo = p.Key.BatchNo,
        //                         DyedFabric = p.Key.DyedFabric
        //                     }).ToList();
        //        return query;
        //    }
        //}

        [OperationContract]
        private TblProductionOrderHeader UpdateOrInsertTblProductionOrderHeader(TblProductionOrderHeader newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var seqCode = SharedOperation.GetChainSetup("GlProduction");
                    var seqProd = context.TblSequenceProductions.FirstOrDefault(w => w.Code == seqCode);
                    newRow.DocCode = SharedOperation.HandelSequence(seqProd);
                    newRow.CreatedBy = user;
                    newRow.CreationDate = DateTime.Now;
                    context.TblProductionOrderHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblProductionOrderHeaders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        newRow.CreatedBy = oldRow.CreatedBy;
                        newRow.CreationDate = oldRow.CreationDate;
                        newRow.LastUpdatedBy = user;
                        newRow.LastUpdatedDate = DateTime.Now;
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblProductionOrderHeader(TblProductionOrderHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblProductionOrderTransaction> GetTblProductionOrderTransaction(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderTransaction> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblProductionOrderHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblProductionOrderTransactions.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderTransactions.Include(nameof(TblProductionOrderTransaction.TblWarehouse1)).Include(nameof(TblProductionOrderTransaction.TblProductionOrderTransactionType1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblProductionOrderTransactions.Count(v => v.TblProductionOrderHeader == groupId);
                    query = context.TblProductionOrderTransactions.Include(nameof(TblProductionOrderTransaction.TblWarehouse1)).Include((nameof(TblProductionOrderTransaction.TblProductionOrderTransactionType1))).OrderBy(sort).Where(v => v.TblProductionOrderHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblProductionOrderTransaction UpdateOrInsertTblProductionOrderTransaction(TblProductionOrderTransaction newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var seqCode = "";// SharedOperation.GetChainSetup("GlProduction");
                    if (newRow.TblProductionOrderTransactionType== 1)
                    {
                        seqCode = SharedOperation.GetChainSetup("GlProductionIssue");
                    }
                    else
                    {
                        seqCode = SharedOperation.GetChainSetup("GlProductionRec");
                    }
                        
                    var seqProd = context.TblSequenceProductions.FirstOrDefault(w => w.Code == seqCode);
                    newRow.DocCode = SharedOperation.HandelSequence(seqProd);

                    newRow.CreatedBy = user;
                    newRow.CreationDate = DateTime.Now;
                    context.TblProductionOrderTransactions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblProductionOrderTransactions
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        newRow.CreatedBy = oldRow.CreatedBy;
                        newRow.CreationDate = oldRow.CreationDate;
                        newRow.LastUpdatedBy = user;
                        newRow.LastUpdatedDate = DateTime.Now;
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblProductionOrderTransaction(TblProductionOrderTransaction row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderTransactions
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }



        [OperationContract]
        private List<TblProductionOrderFabric> GetTblProductionOrderFabric(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderFabric> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblProductionOrderTransaction ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblProductionOrderFabrics.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderFabrics.Include(nameof(TblProductionOrderFabric.TblItemDim1)).Include(nameof(TblProductionOrderFabric.TblWarehouse1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblProductionOrderFabrics.Count(v => v.TblProductionOrderTransaction == groupId);
                    query = context.TblProductionOrderFabrics.Include((nameof(TblProductionOrderFabric.TblItemDim1))).Include(nameof(TblProductionOrderFabric.TblWarehouse1)).OrderBy(sort).Where(v => v.TblProductionOrderTransaction == groupId).Skip(skip).Take(take);
                }

                var result = query.ToList();
                foreach (var item in result)
                {
                    GetTblSalesOrderDetailRequestDetails(context, item);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblProductionOrderFabric UpdateOrInsertTblProductionOrderFabric(TblProductionOrderFabric newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {

                    context.TblProductionOrderFabrics.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblProductionOrderFabrics
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {

                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblProductionOrderFabric(TblProductionOrderFabric row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderFabrics
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }


        [OperationContract]
        private List<TblProductionOrderService> GetTblProductionOrderService(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out List<Entity> entityList, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderService> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblProductionOrderTransaction ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblProductionOrderServices.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderServices.Include(nameof(TblProductionOrderService.TblService1)).Include(nameof(TblProductionOrderService.TblColor1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblProductionOrderServices.Count(v => v.TblProductionOrderTransaction == groupId);
                    query = context.TblProductionOrderServices.Include(nameof(TblProductionOrderService.TblService1)).Include(nameof(TblProductionOrderService.TblColor1)).OrderBy(sort).Where(v => v.TblProductionOrderTransaction == groupId).Skip(skip).Take(take);
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
        private TblProductionOrderService UpdateOrInsertTblProductionOrderService(TblProductionOrderService newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblProductionOrderServices.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblProductionOrderServices
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblProductionOrderService(TblProductionOrderService row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderServices
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }


        [OperationContract]
        private List<TblProductionOrderInvoiceHeader> GetTblProductionOrderInvoiceHeader(int skip, int take, int TblProductionOrderHeader, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out int fullCount, out List<Model.Entity> entityList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (TblProductionOrderHeader != 0)
                {
                    filter = filter + " and it.TblProductionOrderHeader=(@group)";
                    valuesObjects.Add("group", TblProductionOrderHeader);
                }


                IQueryable<TblProductionOrderInvoiceHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblProductionOrderInvoiceHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderInvoiceHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblProductionOrderInvoiceHeaders.Count(x => x.TblProductionOrderHeader == TblProductionOrderHeader || TblProductionOrderHeader == 0);
                    query = context.TblProductionOrderInvoiceHeaders.OrderBy(sort).Where(v => v.TblProductionOrderHeader == TblProductionOrderHeader || TblProductionOrderHeader == 0).Skip(skip).Take(take);

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
        private TblProductionOrderInvoiceHeader UpdateOrInsertTblProductionOrderInvoiceHeader(TblProductionOrderInvoiceHeader newRow, int index, int userIserial, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblProductionOrderInvoiceHeaders.Include(nameof(TblProductionOrderInvoiceHeader.TblProductionOrderInvoiceDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);

                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;

                        // هحذف الى اتحذف
                        foreach (var item in oldRow.TblProductionOrderInvoiceDetails)
                        {
                            if (!newRow.TblProductionOrderInvoiceDetails.Any(td => td.Iserial == item.Iserial))// مش موجود فى الجديد يبقى اتحذف
                                DeleteTblProductionOrderInvoiceDetail(item);
                        }
                        foreach (var item in newRow.TblProductionOrderInvoiceDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp, headeriserial;//item.ItemAdjustment
                            headeriserial = item.TblProductionOrderInvoiceHeader ?? 0;
                            item.TblProductionOrderInvoiceHeader = null;
                            item.TblProductionOrderInvoiceHeader = headeriserial;
                            UpdateOrInsertTblProductionOrderInvoiceDetail(item, 1, out temp);
                        }
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {
                        var seqCode = SharedOperation.GetChainSetup("ProductionOrderInvoiceSequence");
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Code == seqCode);
                        newRow.Code = SharedOperation.HandelSequence(seqRow);
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastUpdatedBy = userIserial;
                        newRow.LastUpdatedDate = DateTime.Now;
                        //  context.TblProductionOrderInvoiceHeaders.Detach(newRow);
                        context.TblProductionOrderInvoiceHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                    foreach (var item in newRow.TblProductionOrderInvoiceDetails)
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
        private int DeleteTblProductionOrderInvoiceHeader(TblProductionOrderInvoiceHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderInvoiceHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblProductionOrderInvoiceHeaders.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }


        [OperationContract]
        private List<TblProductionOrderInvoiceDetail> GetTblProductionOrderInvoiceDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, string company, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderInvoiceDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblProductionOrderInvoiceHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblProductionOrderInvoiceDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblProductionOrderInvoiceDetails.Include("TblService1").Include("TblColor1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount =
                        context.TblProductionOrderInvoiceDetails.Count(w => w.TblProductionOrderInvoiceHeader == groupId);
                    query = context.TblProductionOrderInvoiceDetails.Include("TblService1").Include("TblColor1").OrderBy(sort).Where(v => v.TblProductionOrderInvoiceHeader == groupId).Skip(skip).Take(take);
                }



                return query.ToList();
            }
        }

        [OperationContract]
        private TblProductionOrderInvoiceDetail UpdateOrInsertTblProductionOrderInvoiceDetail(TblProductionOrderInvoiceDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblProductionOrderInvoiceDetails.FirstOrDefault(th => th.Iserial == newRow.Iserial);

                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                        //GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {

                        context.TblProductionOrderInvoiceDetails.AddObject(newRow);
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
        private int DeleteTblProductionOrderInvoiceDetail(TblProductionOrderInvoiceDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblProductionOrderInvoiceDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.TblProductionOrderInvoiceDetails.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblProductionOrderService> GetProductionOrderServicePending(int JournalAccountType, int EntityAccount,string code, DateTime? fromDate, DateTime? toDate)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var list = entity.TblProductionOrderInvoiceDetails.Select(w => w.TblProductionOrderTransaction).ToList();
                var query =
                    entity.TblProductionOrderServices.Include(nameof(TblProductionOrderService.TblService1)).Include(nameof(TblProductionOrderService.TblColor1))
                        .Where(x => x.EntityAccount == EntityAccount && x.TblJournalAccountType == JournalAccountType &&
                        (x.TblProductionOrderTransaction1.DocDate >= fromDate || fromDate == null)
                        && !list.Contains(x.TblProductionOrderTransaction) &&                         
                        x.TblProductionOrderTransaction1.DocCode== code|| code==null
                        && (x.TblProductionOrderTransaction1.DocDate <= toDate || toDate == null));
                return query.ToList();
            }
        }

        [OperationContract]
        private TblProductionOrderInvoiceHeader SearchProductionOrderInvoice(TblProductionOrderInvoiceHeader newrow, int userIserial, List<int> SalesIssueHeaderList)
        {
            var result = new List<TblProductionOrderInvoiceDetail>();

            using (var context = new WorkFlowManagerDBEntities())
            {
                int temp = 0;
                UpdateOrInsertTblProductionOrderInvoiceHeader(newrow, 0, userIserial, out temp);
                var query = (from p in context.TblProductionOrderServices.Include("TblService1").Include("TblColor1")
                    .Where(w => SalesIssueHeaderList.Any(e => e == w.TblProductionOrderTransaction)
                        &&
                        !context.TblProductionOrderInvoiceDetails.Any(e => e.TblProductionOrderTransaction == w.TblProductionOrderTransaction)
                        )
                    .GroupBy(e =>
                            new
                            {
                                e.TblService1,
                                e.TblColor1,
                                e.TblProductionOrderTransaction
                            })
                             select new
                             {
                                 Qty = p.Sum(v => v.Qty),
                                 TblColor1 = p.Key.TblColor1,
                                 TblService1 = p.Key.TblService1,
                                 Misc = 0,
                                 Price = p.FirstOrDefault().Cost,
                                 TblProductionOrderTransaction = p.Key.TblProductionOrderTransaction,
                                 TblProductionOrderInvoiceHeader = newrow.Iserial,
                             }).ToList();
                foreach (var item in query)
                {
                    result.Add(new TblProductionOrderInvoiceDetail()
                    {
                        Qty = item.Qty,
                        TblService1 = item.TblService1,
                        Misc = 0,
                        Price = item.Price,
                        TblProductionOrderTransaction = item.TblProductionOrderTransaction,
                        TblProductionOrderInvoiceHeader = newrow.Iserial,
                    });

                }
                //  newrow.TblProductionOrderInvoiceDetails = new System.Data.Objects.DataClasses.EntityCollection<TblProductionOrderInvoiceDetail>();
                foreach (var item in result.ToList())
                {
                    context.TblProductionOrderInvoiceDetails.AddObject(item);
                }
                context.SaveChanges();
            }

            return newrow;
        }

        [OperationContract]
        private List<TblProductionOrderInvoiceHeaderMarkupTransProd> GetTblProductionOrderInvoiceMarkupTransProd(int type, int TblProductionOrderInvoiceHeader, string company, out List<Model.Entity> entityList)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblProductionOrderInvoiceHeaderMarkupTransProd> query = entity.TblProductionOrderInvoiceHeaderMarkupTransProds.Where(x => x.TblProductionOrderInvoiceHeader == TblProductionOrderInvoiceHeader && x.Type == type);


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
        private TblProductionOrderInvoiceHeaderMarkupTransProd UpdateOrInsertTblProductionOrderInvoiceMarkupTransProds(TblProductionOrderInvoiceHeaderMarkupTransProd newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {

                if (save)
                {
                    entity.TblProductionOrderInvoiceHeaderMarkupTransProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblProductionOrderInvoiceHeaderMarkupTransProds
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
        private int DeleteTblProductionOrderInvoiceMarkupTransProds(TblProductionOrderInvoiceHeaderMarkupTransProd row)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblProductionOrderInvoiceHeaderMarkupTransProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblProductionOrderInvoiceHeader PostTblProductionOrderInvoiceHeader(TblProductionOrderInvoiceHeader row, int user, string company)
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
                    var query = entity.TblProductionOrderInvoiceHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Production TransNo " + row.Code;
                    var markuptrans =
                        entity.TblProductionOrderInvoiceHeaderMarkupTransProds
                            .Where(x => x.TblProductionOrderInvoiceHeader == row.Iserial && x.Type == 0);

                    double cost =
                         (double)entity.TblProductionOrderInvoiceDetails.Where(x => x.TblProductionOrderInvoiceHeader == row.Iserial).Sum(w => w.Price * w.Qty);

                    double totalWithItemEffect = 0;
                    double totalWithoutItemEffect = 0;
                    foreach (var variable in markuptrans)
                    {
                        var markupRow = glserive.getMarkUpByIserial(variable.TblMarkupProd, company);
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
                                entity.TblProductionOrderInvoiceDetails.Where(x => x.TblProductionOrderInvoiceHeader == row.Iserial).ToList();
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
                                var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlProductionInvoice").sSetupValue;

                                int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;
                                int temp;
                                var newLedgerHeaderProdRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = desc,
                                    DocDate = row.DocDate,
                                    TblJournal = journalint,
                                    TblTransactionType = 12,
                                    TblJournalLink = query.Iserial
                                };
                                

                                glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);

                                var sqlParam = new List<SqlParameter>{
                                new SqlParameter
                                {
                                    ParameterName = "Iserial",
                                    Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
                                    SqlDbType = SqlDbType.NVarChar
                                },};

                                var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlProductionOrderPostingToGl  @Iserial",
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


        //[OperationContract]
        //private TblProductionOrderHeader PostDyeingOrderInvoice(TblProductionOrderHeader row, int user, string company)
        //{
        //    using (var scope = new TransactionScope())
        //    {
        //        short? lang = 0;
        //        using (var entity = new WorkFlowManagerDBEntities())
        //        {
        //            var firstOrDefault = entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user);
        //            if (firstOrDefault != null)
        //                lang = firstOrDefault.CurrLang;

        //            entity.CommandTimeout = 0;
        //            var query = entity.TblProductionOrderHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

        //            string desc = "Dyeing TransNo " + row.Code;


        //            var markuptrans =
        //                entity.TblDyeingOrderInvoiceMarkupTransProds.Include("TblMarkupProd1.TblMarkupGroupProd1")
        //                    .Where(x => x.TblProductionOrderHeader == row.Iserial && x.Type == 0);

        //            var cost =
        //                entity.TblProductionOrderTransactions.Where(x => x.TblProductionOrderHeader == row.Iserial).Sum(w => w.Cost * w.Qty);

        //            double totalWithItemEffect = 0;
        //            double totalWithoutItemEffect = 0;
        //            foreach (var variable in markuptrans)
        //            {
        //                if (variable.TblMarkupProd1.ItemEffect == false)
        //                {
        //                    if (variable.MiscValueType == 0)
        //                    {
        //                        totalWithoutItemEffect = (double)(totalWithoutItemEffect + (cost * (variable.MiscValue / 100)));
        //                    }
        //                    else
        //                    {
        //                        totalWithoutItemEffect = (double)(totalWithoutItemEffect + variable.MiscValue);
        //                    }
        //                }
        //                else
        //                {
        //                    if (variable.MiscValueType == 0)
        //                    {
        //                        totalWithItemEffect = (double)(totalWithItemEffect + (cost * (variable.MiscValue / 100)));
        //                    }
        //                    else
        //                    {
        //                        totalWithItemEffect = (double)(totalWithItemEffect + variable.MiscValue);
        //                    }
        //                }
        //            }
        //            if (query != null)
        //            {
        //                query.MiscWithoutItemEffect = totalWithoutItemEffect;
        //                query.Misc = totalWithItemEffect;
        //                if (totalWithItemEffect != 0)
        //                {
        //                    var queryDetail =
        //                        entity.TblProductionOrderTransactions.Where(x => x.TblProductionOrderHeader == row.Iserial).ToList();
        //                    foreach (var variable in queryDetail)
        //                    {
        //                        variable.Misc = (variable.Cost / cost) * totalWithItemEffect;
        //                    }
        //                }
        //                entity.SaveChanges();
        //                if (query != null)
        //                {
        //                    query.Status = 1;
        //                    query.TblUser = user;
        //                    query.PostDate = DateTime.Now;
        //                    using (var db = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
        //                    {
        //                        var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlDyeingInvoice").sSetupValue;

        //                        int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

        //                        var newLedgerHeaderProdRow = new TblLedgerHeader
        //                        {
        //                            CreatedBy = user,
        //                            CreationDate = DateTime.Now,
        //                            Description = desc,
        //                            DocDate = row.DocDate,
        //                            TblJournal = journalint,
        //                            TblTransactionType = 10,
        //                            TblJournalLink = query.Iserial
        //                        };
        //                        int temp;
        //                        var glserive = new GlService();
        //                        glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);


        //                        var sqlParam = new List<SqlParameter>
        //                    {

        //                        new SqlParameter
        //                        {
        //                            ParameterName = "Iserial",
        //                            Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
        //                            SqlDbType = SqlDbType.NVarChar
        //                        },
        //                    };

        //                        var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlDyeingOrderInvoicePostingToGl  @Iserial",
        //                         sqlParam.ToArray()).ToList();
        //                        #region MarkUp

        //                        foreach (var rr in markuptrans)
        //                        {
        //                            var currencyrow = db.TblCurrencyTests.First(w => w.Iserial == rr.TblCurrency);
        //                            var glAccount =
        //                                db.Entities.FirstOrDefault(
        //                                    x => x.Iserial == rr.TblMarkupProd && x.scope == 0 && x.TblJournalAccountType == 9).AccountIserial;
        //                            var vendorAccountMarkUp =
        //                                db.Entities.FirstOrDefault(
        //                                    x => x.Iserial == rr.EntityAccount && x.scope == 0 && x.TblJournalAccountType == rr.TblJournalAccountType);

        //                            var drorCr = true;
        //                            double? total = 0;
        //                            if (rr.MiscValueType == 0)
        //                            {
        //                                total = (total + cost * (rr.MiscValue / 100)) * rr.TblMarkupProd1.TblMarkupGroupProd1.Direction;
        //                            }
        //                            else
        //                            {
        //                                total = (total + rr.MiscValue) * rr.TblMarkupProd1.TblMarkupGroupProd1.Direction;
        //                            }
        //                            if (total > 0)
        //                            {
        //                                drorCr = false;
        //                            }
        //                            var markupdes = rr.TblMarkupProd1.Ename + query.Code;
        //                            if (lang == 0)
        //                            {
        //                                markupdes = rr.TblMarkupProd1.Aname + query.Code;
        //                            }
        //                            decimal totalModified = (decimal)total;
        //                            if (total < 0)
        //                            {
        //                                totalModified = (decimal)(total * -1);
        //                            }
        //                            var markupVendorDiscount = new TblLedgerMainDetail();

        //                            markupVendorDiscount = new TblLedgerMainDetail
        //                            {
        //                                Amount = totalModified,
        //                                Description = markupdes,
        //                                ExchangeRate = currencyrow.ExchangeRate,
        //                                TblCurrency = rr.TblCurrency,
        //                                TransDate = row.DocDate,
        //                                TblJournalAccountType = 0,
        //                                EntityAccount = glAccount,
        //                                GlAccount = glAccount,
        //                                TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
        //                                PaymentRef = query.Code,
        //                                DrOrCr = !drorCr
        //                            };

        //                            glserive.UpdateOrInsertTblLedgerMainDetails(markupVendorDiscount, true, 000, out temp, company,
        //                                 user);

        //                            if (glAccount != 0)
        //                            {
        //                                var markupVendor = new TblLedgerMainDetail
        //                                {
        //                                    Amount = totalModified,
        //                                    Description = markupdes,
        //                                    ExchangeRate = currencyrow.ExchangeRate,
        //                                    TblCurrency = rr.TblCurrency,
        //                                    TransDate = row.DocDate,
        //                                    TblJournalAccountType = rr.TblJournalAccountType,
        //                                    EntityAccount = vendorAccountMarkUp.Iserial,
        //                                    GlAccount = vendorAccountMarkUp.AccountIserial,
        //                                    TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
        //                                    PaymentRef = query.Code,
        //                                    DrOrCr = drorCr
        //                                };

        //                                glserive.UpdateOrInsertTblLedgerMainDetails(markupVendor, true, 000, out temp, company, user);
        //                            }
        //                        }

        //                        #endregion MarkUp

        //                        foreach (var rr in list.GroupBy(x => x.GroupName))
        //                        {
        //                            glserive.PostInvPurchaseAndTax(query, newLedgerHeaderProdRow, rr, company, user, list, desc);
        //                        }
        //                        glserive.CorrectLedgerHeaderRouding(newLedgerHeaderProdRow.Iserial, company, user);
        //                    }

        //                    entity.SaveChanges();
        //                    scope.Complete();
        //                }
        //                return query;
        //            }
        //            return null;
        //        }
        //    }
        //}
    }
}