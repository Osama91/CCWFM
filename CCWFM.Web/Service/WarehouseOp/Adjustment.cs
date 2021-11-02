using CCWFM.Models.Inv;
using CCWFM.Models.Items;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;

namespace CCWFM.Web.Service.WarehouseOp
{
    public partial class WarehouseService
    {
        [OperationContract]
        private List<TblWarehouse> GetLookUpWarehouseForAdjustment(int UserIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblWarehouse> query;
                query = context.TblWarehouses.Include(nameof(TblWarehouse.TblSite1)).Where(w =>
                    w.TblAuthWarehouses.Any(aw => aw.AuthUserIserial == UserIserial &&
                    aw.PermissionType == (short)AuthWarehouseType.Adjustment));

                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblAdjustmentHeader> GetAdjustment(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects,
          int userIserial, bool openningBalance, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var defaultQuery = context.TblAdjustmentHeaders.Include(nameof(
                    TblAdjustmentHeader.TblWarehouse)).Where(a => a.IsOpeningBalance == openningBalance);
                IQueryable<TblAdjustmentHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = defaultQuery.Where(filter, parameterCollection.ToArray()).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType == (short)AuthWarehouseType.Adjustment &&
                        (aw.WarehouseIserial == tr.WarehouseIserial))).Count();
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType == (short)AuthWarehouseType.Adjustment &&
                        (aw.WarehouseIserial == tr.WarehouseIserial)));
                }
                else
                {
                    fullCount = defaultQuery.Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType == (short)AuthWarehouseType.Adjustment &&
                        (aw.WarehouseIserial == tr.WarehouseIserial ))).Count();
                    query = defaultQuery.OrderBy(sort).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType == (short)AuthWarehouseType.Adjustment &&
                        (aw.WarehouseIserial == tr.WarehouseIserial)));
                }
                return query.Skip(skip).Take(take).ToList();
            }
        }
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthWarehouse> GetUserAsignedWarehousesForAdjustment(int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthWarehouses.Where(aw =>
                aw.AuthUserIserial == userIserial && aw.PermissionType == (short)AuthWarehouseType.Adjustment).ToList();
            }
        }
        [OperationContract]
        private TblAdjustmentHeader UpdateOrInsertAdjustmentHeader(TblAdjustmentHeader newRow, int index, int userIserial, out int outindex)// )
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblAdjustmentHeaders.Include(nameof(TblAdjustmentHeader.TblAdjustmentDetails)).Include(nameof(
                    TblAdjustmentHeader.TblWarehouse)).FirstOrDefault(th => th.Iserial == newRow.Iserial);
                    var tempwarhouse = newRow.TblWarehouse;
                    newRow.TblWarehouse = null;
                    foreach (var item in newRow.TblAdjustmentDetails)
                    {
                        item.AvailableQuantity = item.ItemAdjustment.AvailableQuantity;
                        item.DifferenceQuantity = item.ItemAdjustment.DifferenceQuantity;
                        item.CountedQuantity = item.ItemAdjustment.CountedQuantity;
                        item.Cost = item.ItemAdjustment.Cost;
                    }
                    if (oldRow != null)// الهيدر موجود قبل كده
                    {                      
                        //// هحذف الى اتحذف
                        //foreach (var item in oldRow.TblAdjustmentDetails)
                        //{
                        //    if (!newRow.TblAdjustmentDetails.Any(td => td.Iserial == item.Iserial))// مش موجود فى الجديد يبقى اتحذف
                        //        DeleteAdjustmentDetail(item);
                        //}
                        foreach (var item in newRow.TblAdjustmentDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp, headeriserial;//item.ItemAdjustment
                            headeriserial = item.AdjustmentHeaderIserial;
                            item.TblAdjustmentHeader = null;
                            item.AdjustmentHeaderIserial = headeriserial;
                            UpdateOrInsertAdjustmentDetail(item, 1, out temp);
                            item.TblAdjustmentHeader = newRow;
                        }
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        var result = SharedOperation.GenericUpdate(oldRow, newRow, context);

                        if (result.Count() > 0)
                        {
                            newRow.LastChangeUser = userIserial;
                            newRow.LastChangeDate = DateTime.Now;
                        }
                    }
                    else// الهيدر ده جديد
                    {
                        var warehouse = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.WarehouseIserial);
                        var seq = warehouse.AdjustIn;
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seq);
                        newRow.Code = SharedOperation.HandelSequence(seqRow);

                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastChangeDate = DateTime.Now;
                        newRow.LastChangeUser = userIserial;

                        context.TblAdjustmentHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                    //if (newRow.EntityState != System.Data.EntityState.Detached && newRow.EntityState != System.Data.EntityState.Added)
                    //    context.Detach(newRow);
                    //if (newRow.TblWarehouse == null)
                    //    newRow.TblWarehouse = context.TblWarehouses.AsNoTracking().FirstOrDefault(w => w.Iserial == newRow.WarehouseIserial);// tempwarhouse;
                    foreach (var item in newRow.TblAdjustmentDetails)
                    {
                        GetAdjustmentItemDetails(context, item);
                    }
                }
                catch (Exception ex) { throw ex; }
                return newRow;
            }
        }
        /// <summary>
        /// لازم يكون معاه الهيدر بتاعه 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private static void GetAdjustmentItemDetails(WorkFlowManagerDBEntities context, TblAdjustmentDetail item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.ItemDimIserial);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemAdjustment.ItemId = itemDimFromResult.ItemIserial;
            item.ItemAdjustment.ItemCode = context.FabricAccSearches.FirstOrDefault(fas =>
            fas.Iserial == itemDimFromResult.ItemIserial && fas.ItemGroup == itemDimFromResult.ItemType).Code;
            item.ItemAdjustment.ItemName = itemDimFromResult.ItemName;
            item.ItemAdjustment.ItemType = itemDimFromResult.ItemType;
            item.ItemAdjustment.DifferenceQuantity = item.DifferenceQuantity;
            item.ItemAdjustment.AvailableQuantity = item.AvailableQuantity;
            item.ItemAdjustment.CountedQuantity = item.CountedQuantity;

            item.ItemAdjustment.ItemDimFromIserial = item.ItemDimIserial;
            item.ItemAdjustment.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemAdjustment.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemAdjustment.SizeFrom = itemDimFromResult.Size;
            item.ItemAdjustment.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemAdjustment.SiteFromIserial = itemDimFromResult.SiteIserial;
            item.ItemAdjustment.Cost = item.Cost;

            //string warehouseCode = context.TblWarehouses.FirstOrDefault(tw =>
            //tw.Iserial == item.TblAdjustmentHeader.WarehouseIserial).Code;
            //var itemdimfrom = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.ItemDimIserial);
            //string itemCode = context.FabricAccSearches.FirstOrDefault(fas =>
            //    fas.Iserial == itemdimfrom.ItemIserial && fas.ItemGroup == itemdimfrom.ItemType).Code;
            //string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimfrom.TblColor).Code;
            //item.ItemAdjustment.AvailableQuantity = WarehouseQuantities.GetAvilableQuantity(
            //     warehouseCode, itemCode, colorCode, itemdimfrom.Size, itemdimfrom.BatchNo);
        }
        [OperationContract]
        private int DeleteAdjustment(TblAdjustmentHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = context.TblAdjustmentHeaders
                    .FirstOrDefault(e => e.Iserial == row.Iserial);
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
        [OperationContract]
        private List<TblAdjustmentDetail> GetAdjustmentDetail(int skip, int take, int headerId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblAdjustmentDetails.Include(nameof(TblAdjustmentDetail.TblAdjustmentHeader)).Include(
                    string.Format("{0}.{1}", nameof(TblAdjustmentDetail.TblAdjustmentHeader), nameof(TblAdjustmentHeader.TblWarehouse)))
                    .Where(v => v.AdjustmentHeaderIserial == headerId).OrderBy(
                    x => x.Iserial).Skip(skip).Take(take);
                var result = query.ToList();
                foreach (var item in result)
                {
                    GetAdjustmentItemDetails(context, item);
                }
                return result;
            }
        }
        [OperationContract]
        private TblAdjustmentDetail UpdateOrInsertAdjustmentDetail(TblAdjustmentDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAdjustmentDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    SharedOperation.GenericUpdate(oldRow, newRow, context);                    
                }
                else
                {
                    context.TblAdjustmentDetails.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }
        [OperationContract]
        private TblAdjustmentDetail DeleteAdjustmentDetail(TblAdjustmentDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAdjustmentDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
        [OperationContract]
        private List<DataLayer.ItemDimensionAdjustmentSearchModel> GetItemDimensionsOrCreate(List<DataLayer.ItemDimensionAdjustmentSearchModel> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var item in itemsList)
                {
                    item.ItemDimFromIserial = context.FindOrCreateItemDim(item.ItemId, item.ItemType, item.ColorFromId,
                            item.SizeFrom, item.BatchNoFrom, item.SiteFromIserial).FirstOrDefault().Iserial;
                }
                return itemsList;
            }
        }
        /// <summary>
        /// Form TblInventTrans by date
        /// </summary>
        /// <param name="warehouseCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="colorIserial"></param>
        /// <param name="size"></param>
        /// <param name="rollBatch"></param>
        /// <returns></returns>
        [OperationContract]
        private List<DataLayer.ItemDimensionAdjustmentSearchModel> GetItemDimensionsQuantitiesByDate(
            out int fullCount, string warehouseCode, int? itemIserial, string itemType,
           int? colorIserial, string size, string rollBatch, DateTime docDate)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                // هنا ده جزء الـ Relations
                IQueryable<TblInventTran> temp = entities.TblInventTrans.Include(nameof(TblInventTran.TblInventType1))
                    .Include(nameof(TblInventTran.TblWarehouse1)).Include(nameof(TblInventTran.TblItemDim1))
                    .Include(string.Format("{0}.{1}", nameof(TblInventTran.TblItemDim1), nameof(TblItemDim.TblColor1)));
                // ده الشروط
                var predicate = PredicateBuilder.True<TblInventTran>();

                predicate = predicate.And(i => i.TblWarehouse1.Code == warehouseCode);
                predicate = predicate.And(i => i.TblItemDim1.ItemIserial == itemIserial);
                predicate = predicate.And(i => i.TblItemDim1.ItemType == itemType);
                predicate = predicate.And(i => i.DocDate <= docDate);
                if (colorIserial.HasValue)
                    predicate = predicate.And(i => i.TblItemDim1.TblColor == colorIserial);
                if (!string.IsNullOrWhiteSpace(size))
                    predicate = predicate.And(i => i.TblItemDim1.Size == size);
                if (!string.IsNullOrWhiteSpace(rollBatch))
                    predicate = predicate.And(i => i.TblItemDim1.BatchNo == rollBatch);
                // هجيب كل الى محتاجه علشان جزئية الكمية والـ Nature
                var query = temp.AsExpandable().Where(predicate).Select(it =>
                          new DataLayer.ItemDimensionAdjustmentSearchModel()
                          {
                              ItemId = it.TblItemDim1.ItemIserial,
                              ItemName = entities.FabricAccSearches.FirstOrDefault(i =>
                                     i.Iserial == itemIserial && i.ItemGroup == itemType) == null ? "" : entities.FabricAccSearches.FirstOrDefault(i =>
                                     i.Iserial == itemIserial && i.ItemGroup == itemType).Name,
                              ItemCode = entities.FabricAccSearches.FirstOrDefault(i =>
                                     i.Iserial == itemIserial && i.ItemGroup == itemType).Code,
                              ItemType = itemType ?? "",
                              AvailableQuantity = it.Qty * it.TblInventType1.Nature,
                              DifferenceQuantity = 0,
                              CountedQuantity = 0,
                              SiteFromIserial = it.TblItemDim1.TblSite,
                              BatchNoFrom = it.TblItemDim1.BatchNo ?? "",
                              SizeFrom = it.TblItemDim1.Size ?? "",
                              ColorFromCode = it.TblItemDim1.TblColor1.Ename,
                              ColorFromId = it.TblItemDim1.TblColor,
                              IsAcc = itemType.ToLower().Contains("acc"),
                              ItemDimFromIserial = it.TblItemDim,
                              TransferredQuantity = 0,
                              Cost = 0,// هو هيدخلها بنفسه
                          }
                          // هعمل جروب باى بيه
                   ).GroupBy(i => new {
                       i.ItemId,
                       i.ItemCode,
                       i.ItemDimFromIserial,
                       i.SiteFromIserial,
                       i.BatchNoFrom,
                       i.SizeFrom,
                       i.ColorFromCode,
                       i.ColorFromId,
                   });
             
                fullCount = query.Count();
                var result = new List<DataLayer.ItemDimensionAdjustmentSearchModel>();
                if (fullCount > 0)
                {
                    // هنا هرجع اجيب الى محتاجه واعمل المجموع واشغل الاستعلام
                    var tempQ = query.Select(it =>
                          new DataLayer.ItemDimensionAdjustmentSearchModel()
                          {
                              ItemId = it.Key.ItemId,
                              ItemName = (entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType) == null ? "" :
                                  entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType).Name),
                              ItemCode = it.Key.ItemCode,
                              ItemType = itemType ?? "",
                              AvailableQuantity = it.Sum(i => (i.AvailableQuantity)),
                              DifferenceQuantity = 0,
                              CountedQuantity = 0,
                              SiteFromIserial = it.Key.SiteFromIserial,
                              BatchNoFrom = it.Key.BatchNoFrom ?? "",
                              SizeFrom = it.Key.SizeFrom ?? "",
                              ColorFromCode = it.Key.ColorFromCode,
                              ColorFromId = it.Key.ColorFromId,
                              IsAcc = itemType.ToLower().Contains("acc"),
                              ItemDimFromIserial = it.Key.ItemDimFromIserial,
                              TransferredQuantity = 0,
                              Cost = 0,
                          }
                   );
                    // هجيب الداتا                
                    result = tempQ.ToList();
                }
                else if (itemIserial.HasValue && colorIserial.HasValue &&
                    !string.IsNullOrWhiteSpace(itemType) && ((itemType.ToLower().Contains("acc") &&
                    !string.IsNullOrWhiteSpace(size)) || (!itemType.ToLower().Contains("acc") &&
                    !string.IsNullOrWhiteSpace(rollBatch))))
                {
                    var siteIserial = entities.TblWarehouses.FirstOrDefault(w => w.Code == warehouseCode).TblSite;
                    var itemdim = entities.FindOrCreateItemDim(itemIserial, itemType, colorIserial, size, rollBatch, siteIserial).FirstOrDefault();
                    var color = entities.TblColors.FirstOrDefault(c => c.Iserial == itemdim.TblColor);
                    result = new List<DataLayer.ItemDimensionAdjustmentSearchModel>()
                            {
                                new DataLayer.ItemDimensionAdjustmentSearchModel() {
                              ItemId = itemdim.ItemIserial,
                              ItemName = (entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType) == null ? "" :
                                  entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType).Name),
                              ItemCode = (entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType) == null ? "" :
                                  entities.FabricAccSearches.FirstOrDefault(i =>
                                  i.Iserial == itemIserial && i.ItemGroup == itemType).Code),
                              ItemType = itemdim.ItemType,
                                    AvailableQuantity = 0,
                              DifferenceQuantity = 0,
                              CountedQuantity = 0,
                              SiteFromIserial = itemdim.TblSite,
                              BatchNoFrom = itemdim.BatchNo,
                              SizeFrom = itemdim.Size,
                              ColorFrom=new DataLayer.TblColor() {
                                  Iserial = itemdim.TblColor,
                                  Code=color.Code,
                                  Ename=color.Ename,
                                  Aname=color.Aname,
                              },
                                    ColorFromCode = color.Code,
                              ColorFromId = itemdim.TblColor,
                              IsAcc = itemdim.ItemType.ToLower().Contains("acc"),
                                    ItemDimFromIserial = itemdim.Iserial,
                                    TransferredQuantity = 0,
                        Cost = 0,
                            }
                            };
                }
                return result;
            }
        }
        [OperationContract]
        private DataLayer.ItemDimensionAdjustmentSearchModel GetItemDimensionQuantitiesByDate(
            string warehouseCode, int itemDimIserial, DateTime docDate)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var temp = entities.TblInventTrans.Include(
                   nameof(TblInventTran.TblWarehouse1)).Include(nameof(TblInventTran.TblItemDim1))
                   .Include(string.Format("{0}.{1}", nameof(TblInventTran.TblItemDim1), nameof(TblItemDim.TblColor1)));

                var query = temp.AsExpandable().Where(i => i.TblWarehouse1.Code == warehouseCode &&
                i.TblItemDim == itemDimIserial && i.DocDate <= docDate).GroupBy(i => new
                {
                    i.Iserial,
                    i.TblWarehouse,
                    i.TblWarehouse1,
                    i.TblItemDim,
                    i.TblItemDim1,
                }).FirstOrDefault();
                var result = new DataLayer.ItemDimensionAdjustmentSearchModel();
                if (query != null)
                {
                    result = new DataLayer.ItemDimensionAdjustmentSearchModel()
                    {
                        ItemId = query.Key.TblItemDim1.ItemIserial,
                        ItemName = entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == query.Key.TblItemDim1.ItemIserial && i.ItemGroup == query.Key.TblItemDim1.ItemType) == null ? "" : entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == query.Key.TblItemDim1.ItemIserial && i.ItemGroup == query.Key.TblItemDim1.ItemType).Name,
                        ItemType = query.Key.TblItemDim1.ItemType ?? "",
                        ItemCode = entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == query.Key.TblItemDim1.ItemIserial && i.ItemGroup == query.Key.TblItemDim1.ItemType).Code,
                        AvailableQuantity = query.Sum(i => i.Qty),
                        DifferenceQuantity = 0,
                        CountedQuantity = 0,
                        SiteFromIserial = query.Key.Iserial,
                        BatchNoFrom = query.Key.TblItemDim1.BatchNo ?? "",
                        SizeFrom = query.Key.TblItemDim1.Size ?? "",
                        ColorFromId = query.Key.TblItemDim1.TblColor,
                        IsAcc = query.Key.TblItemDim1.ItemType.ToLower().Contains("acc"),
                        ItemDimFromIserial = query.Key.TblItemDim,
                        TransferredQuantity = 0,
                        Cost = 0,

                        Vendor = (entities.Vendor_new.FirstOrDefault(v => entities.TblTradeAgreementHeaders.Any(tah =>
                           tah.Vendor == v.Vendor_Code && tah.TblTradeAgreementDetails.Any(tad => tad.ItemCode == query.Key.TblItemDim1.ItemIserial))) == null ? "" : entities.Vendor_new.FirstOrDefault(v => entities.TblTradeAgreementHeaders.Any(tah =>
                           tah.Vendor == v.Vendor_Code && tah.TblTradeAgreementDetails.Any(tad => tad.ItemCode == query.Key.TblItemDim1.ItemIserial))).Vendor_Ename)
                    };
                    result.ColorFromCode = entities.TblColors.FirstOrDefault(c => c.Iserial == query.Key.TblItemDim1.TblColor).Code;
                }
                return result;
            }
        }
  
        #region Import
        [OperationContract]
        private int InsertImportedItems(TblAdjustmentHeader header,
            List<ImportedItemDimensionModel> importedList,bool Counting,out bool IsCounting)
        {
            IsCounting = Counting;            
            List<string> errors = new List<string>();
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;
                try
                {
                    header.Approved = false;
                    foreach (var item in importedList)
                    {
                        TblAdjustmentDetail detail = PrepareDetail(entities, header, item, errors);
                        if (detail != null)
                            header.TblAdjustmentDetails.Add(detail);
                    }
                    var warehouseRec = entities.TblWarehouses.FirstOrDefault(w => w.Iserial == header.WarehouseIserial);
                    var seq = warehouseRec.AdjustIn;
                    var seqRow = entities.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seq);
                    header.Code = SharedOperation.HandelSequence(seqRow);
                    header.TblWarehouse = null;

                    header.CreationDate = DateTime.Now;
                    header.LastChangeDate = DateTime.Now;
                    header.LastChangeUser = header.CreatedBy;

                    entities.TblAdjustmentHeaders.AddObject(header);
                    entities.SaveChanges();
                    return header.Iserial;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        
        [OperationContract]
        private List<string> InsertRemainingImportedItems(int headerIserial,List<ImportedItemDimensionModel> importedList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var header = entities.TblAdjustmentHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
                List<string> errors = new List<string>();
                try
                {
                    foreach (var item in importedList)
                    {
                        TblAdjustmentDetail detail = PrepareDetail(entities, header, item, errors);
                        if (detail != null)
                            // Add detail
                            header.TblAdjustmentDetails.Add(detail);
                    }
                    entities.SaveChanges();
                    //UpdateHeaderQuantities(entities, header);
                    return errors;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static TblAdjustmentDetail PrepareDetail(WorkFlowManagerDBEntities entities, TblAdjustmentHeader header,
            ImportedItemDimensionModel item, List<string> errors)
        {
            string itemInfo = string.Format("Item code:{0}, Color code:{1}, Batch:{2}, Size:{3}, Quantity:{4}, Cost:{5}"
                , item.ItemCode, item.Color, item.BatchNo, item.Size, item.Qty, item.Cost);
            var items = entities.FabricAccSearches.Where(i => i.Code == item.ItemCode&& i.ItemGroup!= "FP").ToList();
            if (items.Count != 1)
            {
                if (items.Count == 0)
                    errors.Add(string.Format("{1} -->> Item Code not Found. More info -->> {0}",
                        itemInfo, DateTime.Now));
                else
                    errors.Add(string.Format("{1} -->> Item Code found for more than one item. More info -->> {0}",
                        itemInfo, DateTime.Now));
                return null;
            }
            var colors = entities.TblColors.Where(i => i.Code == item.Color && i.TblLkpColorGroup != 24);
            if (items.FirstOrDefault().ItemGroup.ToLower().Contains("acc"))
            {
                colors = entities.TblColors.Where(i => i.Code == item.Color && i.TblLkpColorGroup == 24);
            }
            if (colors.Count() != 1)
            {
                if (colors.Count() == 0)
                    errors.Add(string.Format("{1} -->> Color Code not Found. More info -->> {0}",
                        itemInfo, DateTime.Now));
                else
                    errors.Add(string.Format("{1} -->> Color Code found for more than one item. More info -->> {0}",
                        itemInfo, DateTime.Now));
                return null;
            }
            if (string.IsNullOrWhiteSpace(item.Size) && string.IsNullOrWhiteSpace(item.BatchNo))
            {
                errors.Add(string.Format("{1} -->> Every item must have size or batch no. More info -->> {0}",
                    itemInfo, DateTime.Now)); return null;
            }
            // Find Or create ItemDim
            var warehouse = entities.TblWarehouses.FirstOrDefault(s =>
                   s.Iserial == header.WarehouseIserial);
            var iserial = entities.FindOrCreateItemDim(items.FirstOrDefault().Iserial,
                items.FirstOrDefault().ItemGroup, colors.FirstOrDefault().Iserial,
                item.Size, item.BatchNo, warehouse.TblSite).FirstOrDefault().Iserial;

            var detail = new TblAdjustmentDetail()
            {
                ItemDimIserial = iserial,
                AvailableQuantity = 0,// الـ StoredProcedure هتملاه
                CountedQuantity = item.Qty,
                DifferenceQuantity = 0,// الـ StoredProcedure هتملاه
                Cost=item.Cost,
            };
            detail.ItemAdjustment.AvailableQuantity = 0;
            detail.ItemAdjustment.CountedQuantity = item.Qty;
            detail.ItemAdjustment.DifferenceQuantity = 0;
            detail.ItemAdjustment.Cost = item.Cost;
            return detail;
        }      
      
        [OperationContract]
        private void DeleteAdjustmentByIserial(int headerIserial, int CountingHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var temp = entities.TblAdjustmentHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
                var countingTemp = entities.TblAdjustmentHeaders.FirstOrDefault(ah => ah.Iserial == CountingHeaderIserial);
                if (temp != null)
                {
                    entities.DeleteObject(temp);
                }
                if (countingTemp != null)
                {
                    entities.DeleteObject(countingTemp);
                }
                entities.SaveChanges();
            }
        }
        [OperationContract]
        private void ApproveAdjustmentByIserial(int headerIserial, int userIserial, int CountingHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;
                var temp = entities.TblAdjustmentHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
                var countingTemp= entities.TblAdjustmentHeaders.FirstOrDefault(ah => ah.Iserial == CountingHeaderIserial);
                if (temp != null && countingTemp != null)
                    entities.FillAdjustmentDetailQuantities(temp.Iserial, countingTemp.Iserial, true);
                else if(temp!=null)
                    entities.FillAdjustmentDetailQuantities(temp.Iserial, -1, false);
                if (temp != null)
                {
                    temp.Approved = true;
                    temp.ApproveDate = DateTime.Now;
                    temp.ApprovedBy = userIserial;
                }
                if (countingTemp != null)
                {
                    countingTemp.Approved = true;
                    countingTemp.ApproveDate = DateTime.Now;
                    countingTemp.ApprovedBy = userIserial;
                }
                entities.SaveChanges();
            }
        }
        #endregion

        #region Temp
      
        /// <summary>
        /// لازم يكون معاه الهيدر بتاعه 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private static void GetAdjustmentTempItemDetails(WorkFlowManagerDBEntities context, TblAdjustmentTempDetail item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.ItemDimIserial);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemAdjustment.ItemId = itemDimFromResult.ItemIserial;
            item.ItemAdjustment.ItemName = itemDimFromResult.ItemName;
            item.ItemAdjustment.ItemType = itemDimFromResult.ItemType;
            item.ItemAdjustment.ItemCode = context.FabricAccSearches.FirstOrDefault(fas =>
            fas.Iserial == itemDimFromResult.ItemIserial && fas.ItemGroup == itemDimFromResult.ItemType).Code;
            item.ItemAdjustment.DifferenceQuantity = item.DifferenceQuantity;
            item.ItemAdjustment.AvailableQuantity = item.AvailableQuantity;
            item.ItemAdjustment.CountedQuantity = item.CountedQuantity;
            item.ItemAdjustment.Cost = item.Cost;

            item.ItemAdjustment.ItemDimFromIserial = item.ItemDimIserial;
            item.ItemAdjustment.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemAdjustment.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemAdjustment.SizeFrom = itemDimFromResult.Size;
            item.ItemAdjustment.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemAdjustment.SiteFromIserial = itemDimFromResult.SiteIserial;
            var vendor = context.Vendor_new.FirstOrDefault(v => context.TblTradeAgreementHeaders.Any(tah =>
                          tah.Vendor == v.Vendor_Code && tah.TblTradeAgreementDetails.Any(tad => tad.ItemCode == itemDimFromResult.ItemIserial)));
            item.ItemAdjustment.Vendor = (vendor == null ? "" : vendor.Vendor_Ename);
        }
        [OperationContract]
        private List<TblAdjustmentTempDetail> GetAdjustmentTempDetails(int skip, int take, int headerId,
            DataLayer.TblAdjustmentTempDetailSearch searchRow = null)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblAdjustmentTempDetails.Include(nameof(TblAdjustmentTempDetail.TblAdjustmentHeader)).Include(
                    string.Format("{0}.{1}", nameof(TblAdjustmentTempDetail.TblAdjustmentHeader), nameof(TblAdjustmentHeader.TblWarehouse)));

                var predicate = PredicateBuilder.True<TblAdjustmentTempDetail>();
                if (searchRow != null)
                {
                    if (!string.IsNullOrWhiteSpace(searchRow.ItemDimIserialStr))
                        predicate = predicate.And(i => i.ItemDimIserial == searchRow.ItemDimIserial);
                    if (!string.IsNullOrWhiteSpace(searchRow.ItemCode))
                    {
                        var item = context.FabricAccSearches.Where(fas => fas.Code == searchRow.ItemCode);
                        if (item != null)
                            predicate = predicate.And(i =>
                            item.Any(fa => fa.Iserial == i.TblItemDim.ItemIserial) &&
                            item.Any(fa => fa.ItemGroup == i.TblItemDim.ItemType));
                    }
                    if (!string.IsNullOrWhiteSpace(searchRow.ItemName))
                    {
                        var item = context.FabricAccSearches.Where(fas => fas.Name.Contains(searchRow.ItemName));
                        if (item != null)
                            predicate = predicate.And(i =>
                            item.Any(fa => fa.Iserial == i.TblItemDim.ItemIserial) &&
                            item.Any(fa => fa.ItemGroup == i.TblItemDim.ItemType));
                    }
                    if (!string.IsNullOrWhiteSpace(searchRow.ColorCode))
                        predicate = predicate.And(i => i.TblItemDim.TblColor1.Code == searchRow.ColorCode);
                    if (!string.IsNullOrWhiteSpace(searchRow.Size))
                        predicate = predicate.And(i => i.TblItemDim.Size == searchRow.Size);
                    if (!string.IsNullOrWhiteSpace(searchRow.BatchNo))
                        predicate = predicate.And(i => i.TblItemDim.BatchNo == searchRow.BatchNo);
                    if (!string.IsNullOrWhiteSpace(searchRow.AvailableQuantityStr))
                        predicate = predicate.And(i => i.AvailableQuantity == searchRow.AvailableQuantity);
                    if (!string.IsNullOrWhiteSpace(searchRow.CountedQuantityStr))
                        predicate = predicate.And(i => i.CountedQuantity == searchRow.CountedQuantity);
                    if (!string.IsNullOrWhiteSpace(searchRow.DifferenceQuantityStr))
                        predicate = predicate.And(i => i.DifferenceQuantity == searchRow.DifferenceQuantity);
                }

                var result = query.AsExpandable().Where(predicate).Where(v => v.AdjustmentHeaderIserial == headerId)
                    .OrderBy(x => x.Iserial).Skip(skip).Take(take).ToList();
                foreach (var item in result)
                {
                    GetAdjustmentTempItemDetails(context, item);
                    item.OldQuantity = item.CountedQuantity;
                }
                return result;
            }
        }
        [OperationContract]
        private TblAdjustmentTempDetail GetAdjustmentTempDetail(int headerId,int ItemDimIserail)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                bool isNew = false;
                decimal oldQty = 0;
                var result = context.TblAdjustmentTempDetails.Include(nameof(TblAdjustmentTempDetail.TblAdjustmentHeader)).Include(
                    string.Format("{0}.{1}",nameof(TblAdjustmentTempDetail.TblAdjustmentHeader),nameof(TblAdjustmentHeader.TblWarehouse)))
                    .FirstOrDefault(v => v.AdjustmentHeaderIserial == headerId && v.ItemDimIserial == ItemDimIserail);
                if (result == null)
                {
                    isNew = true;
                    result = new TblAdjustmentTempDetail() { AdjustmentHeaderIserial = headerId, ItemDimIserial = ItemDimIserail };
                    context.TblAdjustmentTempDetails.AddObject(result);
                    context.SaveChanges();
                    result = context.TblAdjustmentTempDetails.Include(nameof(TblAdjustmentTempDetail.TblAdjustmentHeader)).Include(
                        string.Format("{0}.{1}", nameof(TblAdjustmentTempDetail.TblAdjustmentHeader), nameof(TblAdjustmentHeader.TblWarehouse)))
                        .FirstOrDefault(v => v.AdjustmentHeaderIserial == headerId && v.ItemDimIserial == ItemDimIserail);
                }
                else
                    oldQty = result.CountedQuantity;
                if (isNew)
                    result.ItemAdjustment = GetItemDimensionQuantitiesByDate(result.TblAdjustmentHeader.TblWarehouse.Code, ItemDimIserail, result.TblAdjustmentHeader.DocDate);
                else
                    result.OldQuantity = oldQty;
                result.AvailableQuantity = result.ItemAdjustment.AvailableQuantity;
                result.DifferenceQuantity = result.ItemAdjustment.DifferenceQuantity;
                result.CountedQuantity = result.ItemAdjustment.CountedQuantity;
              
                context.SaveChanges();
                return result;
            }
        }
        [OperationContract]
        private TblAdjustmentTempDetail UpdateOrInsertAdjustmentTempDetail(TblAdjustmentTempDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = (from e in context.TblAdjustmentTempDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.TblAdjustmentTempDetails.AddObject(newRow);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex) { }
                return newRow;
            }
        }
        [OperationContract]
        private TblAdjustmentTempDetail DeleteAdjustmentTempDetail(TblAdjustmentTempDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAdjustmentTempDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
        [OperationContract]
        private int ApproveAdjustmentTempDetail(int headerIserail)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.ApproveTempAdjustmentDetail(headerIserail);
            }
        }
       
        #endregion
    }
}