using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Omu.ValueInjecter;
using System;
using CCWFM.Models.Inv;
using CCWFM.Web.Service.Operations;
using System.Linq.Dynamic;
using CCWFM.Web.Model;
using LinqKit;

namespace CCWFM.Web.Service.WarehouseOp
{
    public partial class WarehouseService
    {
        [OperationContract]
        private List<TblTransferHeader> GetTransfer(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects,
          int userIserial, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var defaultQuery = context.TblTransferHeaders.Include(nameof(
                    TblTransferHeader.TblWarehouseFrom)).Include(nameof(
                        TblTransferHeader.TblWarehouseTo));
                IQueryable<TblTransferHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = defaultQuery.Where(filter, parameterCollection.ToArray()).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType >(short) AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo))).Count();
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo)));
                }
                else
                {
                    fullCount = defaultQuery.Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo))).Count();
                    query = defaultQuery.OrderBy(sort).Where(tr =>
                        context.TblAuthWarehouses.Any(aw =>
                        aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom &&
                        (aw.WarehouseIserial == tr.WarehouseFrom ||
                        aw.WarehouseIserial == tr.WarehouseTo)));
                }
                return query.Skip(skip).Take(take).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthWarehouse> GetUserAsignedWarehousesForTransfer(int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthWarehouses.Where(aw =>
                aw.AuthUserIserial == userIserial && aw.PermissionType > (short)AuthWarehouseType.None &&
                        aw.PermissionType <= (short)AuthWarehouseType.TransferToFrom).ToList();
            }
        }
        [OperationContract]
        private List<TblWarehouse> GetLookUpWarehouseForTransferFrom(int UserIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblWarehouse> query;
                query = context.TblWarehouses.Include(nameof(TblWarehouse.TblSite1)).Where(w =>
                    w.TblAuthWarehouses.Any(aw => aw.AuthUserIserial == UserIserial &&(
                    aw.PermissionType == (short)AuthWarehouseType.TransferFrom ||
                    aw.PermissionType == (short)AuthWarehouseType.TransferToFrom)));
                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblWarehouse> GetLookUpWarehouseForTransferTo(int UserIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblWarehouse> query;
                query = context.TblWarehouses.Include(nameof(TblWarehouse.TblSite1)).Where(w =>
                    w.TblAuthWarehouses.Any(aw => aw.AuthUserIserial == UserIserial &&(
                    aw.PermissionType == (short)AuthWarehouseType.TransferTo ||
                    aw.PermissionType == (short)AuthWarehouseType.TransferToFrom)));
                return query.ToList();
            }
        }
        [OperationContract]
        private TblTransferHeader UpdateOrInsertTransferHeader(TblTransferHeader newRow,int index, int userIserial, out int outindex)// )
        {
            outindex = index;           
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var oldRow = context.TblTransferHeaders.Include(nameof(TblTransferHeader.TblTransferDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);
                    newRow.TblWarehouseFrom = null;
                    newRow.TblWarehouseTo = null;
                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        newRow.LastChangeUser = userIserial;
                        newRow.LastChangeDate = DateTime.Now;
                        if (!oldRow.Approved && newRow.Approved)// كده لسه معموله ابروف
                        {
                            var warehouseTo = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.WarehouseTo);
                            var seqTo = warehouseTo.TransferIn;
                            var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seqTo);
                            newRow.CodeTo = SharedOperation.HandelSequence(seqRow);
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        //// هحذف الى اتحذف
                        //foreach (var item in oldRow.TblTransferDetails)
                        //{
                        //    if (!newRow.TblTransferDetails.Any(td => td.Iserial == item.Iserial))// مش موجود فى الجديد يبقى اتحذف
                        //        DeleteTransferDetail(item);
                        //}
                        foreach (var item in newRow.TblTransferDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp,headeriserial;//item.ItemTransfer
                            headeriserial = item.TransferHeader;
                            item.TblTransferHeader = null;
                            item.TransferHeader = headeriserial;
                            UpdateOrInsertTransferDetail(item, 1, out temp);
                            item.TblTransferHeader = newRow;
                        }
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {
                        var warehouseFrom = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.WarehouseFrom);
                        var seq = warehouseFrom.TransferOut;
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seq);
                        newRow.CodeFrom = SharedOperation.HandelSequence(seqRow);
                        if (newRow.Approved)// كده معموله ابروف
                        {
                            var warehouseTo = context.TblWarehouses.FirstOrDefault(w => w.Iserial == newRow.WarehouseTo);
                            var seqTo = warehouseTo.TransferIn;
                            var seqRowTo = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seqTo);
                            newRow.CodeTo = SharedOperation.HandelSequence(seqRowTo);
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        else
                            newRow.CodeTo = newRow.CodeFrom;
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastChangeDate = DateTime.Now;
                        newRow.LastChangeUser = userIserial;

                        context.TblTransferHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                    foreach (var item in newRow.TblTransferDetails)
                    {
                        GetTransferItemDetails(context, item);
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
        private static void GetTransferItemDetails(WorkFlowManagerDBEntities context, TblTransferDetail item)
        {
            // From
            var tempFrom = context.GetItemDimDetails(item.ItemDimFrom);
            var itemDimFromResult = tempFrom.FirstOrDefault();

            item.ItemTransfer.ItemId = itemDimFromResult.ItemIserial;
            item.ItemTransfer.ItemName = itemDimFromResult.ItemName;
            item.ItemTransfer.ItemType = itemDimFromResult.ItemType;
            item.ItemTransfer.ItemCode = context.FabricAccSearches.FirstOrDefault(fas =>
                fas.Iserial == itemDimFromResult.ItemIserial && fas.ItemGroup == itemDimFromResult.ItemType).Code;
            item.ItemTransfer.TransferredQuantity = Convert.ToDecimal(item.Quantity);

            item.ItemTransfer.ItemDimFromIserial = item.ItemDimFrom;
            item.ItemTransfer.ColorFromId = itemDimFromResult.ColorIserial;
            item.ItemTransfer.ColorFrom.InjectFrom(context.TblColors.FirstOrDefault(c => c.Iserial == itemDimFromResult.ColorIserial));
            item.ItemTransfer.ColorFromCode = itemDimFromResult.ColorCode;
            item.ItemTransfer.SizeFrom = itemDimFromResult.Size;
            item.ItemTransfer.BatchNoFrom = itemDimFromResult.BatchNo;
            item.ItemTransfer.SiteFromIserial = itemDimFromResult.SiteIserial;

            // TO
            var tempTo = context.GetItemDimDetails(item.ItemDimTo);
            var itemDimToResult = tempTo.FirstOrDefault();

            item.ItemTransfer.ItemDimToIserial = item.ItemDimTo;
            item.ItemTransfer.ColorToId = itemDimToResult.ColorIserial;
            item.ItemTransfer.ColorPerRow.InjectFrom(context.TblColors.FirstOrDefault(c => c.Iserial == itemDimToResult.ColorIserial));
            item.ItemTransfer.SizeTo = itemDimToResult.Size;
            item.ItemTransfer.BatchNoTo = itemDimToResult.BatchNo;
            item.ItemTransfer.SiteToIserial = itemDimToResult.SiteIserial;

            // From
            string warehouseCodeFrom = context.TblWarehouses.FirstOrDefault(tw =>
            tw.Iserial == item.TblTransferHeader.WarehouseFrom).Code;
            var itemdimFrom = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.ItemDimFrom);
            item.ItemTransfer.PendingQuantity = GetItemPendingByWarehouse(context, warehouseCodeFrom, itemdimFrom.Iserial);
            if (item.ItemTransfer.ItemType.ToLower().Contains("acc")||item.ItemTransfer.ItemType.ToLower().Contains("fp"))// ده كده اكسيسورى
                item.ItemTransfer.AvailableQuantity = GetItemQuantityByWarehouse(context, warehouseCodeFrom, itemdimFrom.Iserial);
            else// كده قماش
            {
                string itemFromCode = context.FabricAccSearches.FirstOrDefault(fas =>
                    fas.Iserial == itemdimFrom.ItemIserial && fas.ItemGroup == itemdimFrom.ItemType).Code;
                string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimFrom.TblColor).Code;
                item.ItemTransfer.AvailableQuantity = WarehouseQuantities.GetAvilableQuantity(
                     warehouseCodeFrom, itemFromCode, colorCode, itemdimFrom.Size, itemdimFrom.BatchNo);
            }


            // To
            string warehouseCodeTo = context.TblWarehouses.FirstOrDefault(tw =>
            tw.Iserial == item.TblTransferHeader.WarehouseTo).Code;
            var itemdimTo = context.TblItemDims.FirstOrDefault(id => id.Iserial == item.ItemDimTo);
            item.ItemTransfer.PendingToQuantity = GetItemPendingByWarehouse(context, warehouseCodeTo, itemdimTo.Iserial);
            if (item.ItemTransfer.ItemType.ToLower().Contains("acc")||item.ItemTransfer.ItemType.ToLower().Contains("fp"))// ده كده اكسيسورى
                item.ItemTransfer.AvailableToQuantity = GetItemQuantityByWarehouse(context, warehouseCodeTo, itemdimTo.Iserial);
            else// كده قماش
            {
                string itemToCode = context.FabricAccSearches.FirstOrDefault(fas =>
                    fas.Iserial == itemdimTo.ItemIserial && fas.ItemGroup == itemdimTo.ItemType).Code;
                string colorCode = context.TblColors.FirstOrDefault(c => c.Iserial == itemdimTo.TblColor).Code;
                item.ItemTransfer.AvailableToQuantity = WarehouseQuantities.GetAvilableQuantity(
                     warehouseCodeTo, itemToCode, colorCode, itemdimTo.Size, itemdimTo.BatchNo);
            }
        }

        private static decimal GetItemQuantityByWarehouse(WorkFlowManagerDBEntities context, string warehouseCode, int itemDimIserial)
        {
            return context.TblStocks.Where(s => s.TblWarehouse1.Code == warehouseCode &&
                             s.TblItemDim == itemDimIserial).Select(s => s.Qty).DefaultIfEmpty(0).FirstOrDefault();
        }

        private static decimal GetItemPendingByWarehouse(WorkFlowManagerDBEntities context, string warehouseCode, int itemDimIserial)
        {
            return context.TblStocks.Where(s => s.TblWarehouse1.Code == warehouseCode &&
                          s.TblItemDim == itemDimIserial).Select(s => s.PendingQty).DefaultIfEmpty(0).FirstOrDefault();
        }

        [OperationContract]
        private int DeleteTransfer(TblTransferHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblTransferHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblTransferDetail> GetTransferDetail(int skip, int take, int headerId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblTransferDetails.Include(nameof(TblTransferDetail.TblTransferHeader
                    )).Where(v => v.TransferHeader == headerId).OrderBy(
                    x => x.Iserial).Skip(skip).Take(take);
                var result = query.ToList();
                foreach (var item in result)
                {
                    GetTransferItemDetails(context, item);
                }
                return result;
            }
        }

        [OperationContract]
        private TblTransferDetail UpdateOrInsertTransferDetail(TblTransferDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblTransferDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                   
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblTransferDetails.AddObject(newRow);
                }
                
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblTransferDetail DeleteTransferDetail(TblTransferDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblTransferDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private DataLayer.ItemDimensionSearchModel GetItemDimensionQuantities(
           string warehouseFromCode, string warehouseToCode, int itemDimIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var temp = entities.TblStocks.Include(
                   nameof(TblStock.TblWarehouse1)).Include(nameof(TblStock.TblItemDim1))
                   .Include(string.Format("{0}.{1}", nameof(TblStock.TblItemDim1), nameof(TblItemDim.TblColor1)));

                var itemFrom = temp.AsExpandable().FirstOrDefault(i => i.TblWarehouse1.Code == warehouseFromCode &&
                i.TblItemDim == itemDimIserial);
                var itemTo = temp.AsExpandable().FirstOrDefault(i => i.TblWarehouse1.Code == warehouseToCode &&
                i.TblItemDim == itemDimIserial);
                if (itemTo == null) itemTo = new TblStock();
                var result = new DataLayer.ItemDimensionSearchModel();
                if (itemFrom != null && itemTo != null)
                {
                    result = new DataLayer.ItemDimensionSearchModel()
                    {
                        ItemId = itemFrom.TblItemDim1.ItemIserial,
                        ItemName = entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == itemFrom.TblItemDim1.ItemIserial && i.ItemGroup == itemFrom.TblItemDim1.ItemType) == null ? "" : entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == itemFrom.TblItemDim1.ItemIserial && i.ItemGroup == itemFrom.TblItemDim1.ItemType).Name,
                        ItemType = itemFrom.TblItemDim1.ItemType ?? "",
                        ItemCode = entities.FabricAccSearches.FirstOrDefault(i =>
                               i.Iserial == itemFrom.TblItemDim1.ItemIserial && i.ItemGroup == itemFrom.TblItemDim1.ItemType).Code,
                        AvailableQuantity = itemFrom.Qty,
                        PendingQuantity = itemFrom.PendingQty,
                        AvailableToQuantity = itemTo.Qty,// لانه نفس الصنف
                        PendingToQuantity = itemTo.PendingQty,// لانه نفس الصنف
                        SiteFromIserial = itemFrom.TblItemDim1.TblSite,
                        BatchNoFrom = itemFrom.TblItemDim1.BatchNo ?? "",
                        SizeFrom = itemFrom.TblItemDim1.Size ?? "",
                        ColorFromId = itemFrom.TblItemDim1.TblColor,
                        SiteToIserial = itemFrom.TblItemDim1.TblSite,//itemTo
                        BatchNoTo = itemFrom.TblItemDim1.BatchNo ?? "",//itemTo
                        SizeTo = itemFrom.TblItemDim1.Size ?? "",//itemTo
                        ColorToId = itemFrom.TblItemDim1.TblColor,//itemTo
                        IsAcc = itemFrom.TblItemDim1.ItemType.ToLower().Contains("acc"),
                        ItemDimFromIserial = itemFrom.TblItemDim,
                        ItemDimToIserial = itemFrom.TblItemDim,//itemTo
                        TransferredQuantity = 0,
                    };
                    result.ColorFromCode = entities.TblColors.FirstOrDefault(c => c.Iserial == itemFrom.TblItemDim1.TblColor).Code;
                    result.ColorFrom.InjectFrom(itemFrom.TblItemDim1.TblColor1);//itemFrom
                    result.ColorPerRow.InjectFrom(itemFrom.TblItemDim1.TblColor1);//itemTo
                }
                return result;
            }
        }

        [OperationContract]
        private DataLayer.ItemDimensionSearchModel GetItemToQuantities(
            DataLayer.ItemDimensionSearchModel item, string warehouseCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.FindOrCreateItemDim(item.ItemId, item.ItemType, 
                    item.ColorToId, item.SizeTo, item.BatchNoTo, item.SiteToIserial);
                var itemdim = temp.FirstOrDefault();
                item.ItemDimToIserial = itemdim.Iserial;
                item.AvailableToQuantity = GetItemQuantityByWarehouse(context, warehouseCode, item.ItemDimToIserial);
                item.PendingToQuantity = GetItemPendingByWarehouse(context, warehouseCode, item.ItemDimToIserial);
                return item;
            }
        }
    }
}