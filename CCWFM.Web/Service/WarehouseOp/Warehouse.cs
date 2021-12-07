using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Linq.Dynamic;
using CCWFM.Web.Service.Operations;
using CCWFM.Web.Model;
using LinqKit;
using Omu.ValueInjecter;
using System;

namespace CCWFM.Web.Service.WarehouseOp
{
    public partial class WarehouseService
    {
        [OperationContract]
        private List<TblWarehouse> GetLookUpWarehouse()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblWarehouse> query;
                query = context.TblWarehouses.Include(nameof(TblWarehouse.TblSite1));

                return query.ToList();
            }
        }
        
        [OperationContract]
        private TblWarehouse UpdateOrInsertTblWarehouse(TblWarehouse newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblWarehouses.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblWarehouses
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblWarehouse(TblWarehouse row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblWarehouses
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
        
        [OperationContract]
        public List<DataLayer.ItemDimensionAdjustmentSearchModel> GetAccWarehouseRows(string warehouseCode,
        string itemType, int? itemIserial, int? colorIserial, string size, string rollBatch)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var predicate = PredicateBuilder.True<TblStock>();

                predicate = predicate.And(i => i.TblWarehouse1.Code == warehouseCode);
                if (!string.IsNullOrWhiteSpace(itemType))
                    predicate = predicate.And(i => i.TblItemDim1.ItemType == itemType);
                if (itemIserial.HasValue)
                    predicate = predicate.And(i => i.TblItemDim1.ItemIserial == itemIserial);
                if (colorIserial.HasValue)
                    predicate = predicate.And(i => i.TblItemDim1.TblColor == colorIserial);
                if (!string.IsNullOrWhiteSpace(size))
                    predicate = predicate.And(i => i.TblItemDim1.Size == size);
                if (!string.IsNullOrWhiteSpace(rollBatch))
                    predicate = predicate.And(i => i.TblItemDim1.BatchNo == rollBatch);

                var query = entities.TblStocks.Include(nameof(TblInventTran.TblItemDim1))
                   .Include(string.Format("{0}.{1}", nameof(TblInventTran.TblItemDim1), nameof(TblItemDim.TblColor1)))
                    .AsExpandable().Where(predicate).Select(s =>
                new DataLayer.ItemDimensionAdjustmentSearchModel()
                {
                    ItemId = s.TblItemDim1.ItemIserial,
                    ItemName = (entities.FabricAccSearches.FirstOrDefault(i =>
                        i.Iserial == s.TblItemDim1.ItemIserial && i.ItemGroup == s.TblItemDim1.ItemType) == null ? "" :
                                    entities.FabricAccSearches.FirstOrDefault(i =>
                                    i.Iserial == s.TblItemDim1.ItemIserial && i.ItemGroup == s.TblItemDim1.ItemType).Name),
                    ItemType = s.TblItemDim1.ItemType ?? "",
                    ItemCode = entities.FabricAccSearches.FirstOrDefault(i =>
                          i.Iserial == s.TblItemDim1.ItemIserial && i.ItemGroup == s.TblItemDim1.ItemType).Code,
                    AvailableQuantity = s.Qty,
                    PendingQuantity = s.PendingQty,
                    DifferenceQuantity = 0,
                    CountedQuantity = 0,
                    SiteFromIserial = s.TblItemDim1.TblSite,
                    BatchNoFrom = s.TblItemDim1.BatchNo ?? "",
                    SizeFrom = s.TblItemDim1.Size ?? "",
                    ColorFromId = s.TblItemDim1.TblColor,
                    ColorFrom = new DataLayer.TblColor(),
                    ColorFromCode = s.TblItemDim1.TblColor1.Code,
                    IsAcc = s.TblItemDim1.ItemType.ToLower().Contains("acc"),
                    ItemDimFromIserial = s.TblItemDim,
                    TransferredQuantity = 0,
                    SiteToIserial = s.TblItemDim1.TblSite,
                    BatchNoTo = s.TblItemDim1.BatchNo ?? "",
                    SizeTo = s.TblItemDim1.Size ?? "",
                    ColorToId = s.TblItemDim1.TblColor,
                    ColorPerRow = new DataLayer.TblColor(),
                    ItemDimToIserial = s.TblItemDim,
                });
                var result = query.ToList();
                if (result.Count > 0)
                    foreach (var item in result)
                    {
                        var t = entities.TblColors.FirstOrDefault(c => c.Iserial == item.ColorToId);
                        if (t != null)
                            item.ColorPerRow.InjectFrom(t);
                        var f = entities.TblColors.FirstOrDefault(c => c.Iserial == item.ColorFromId);
                        if (f != null)
                            item.ColorFrom.InjectFrom(f);
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
                            }
                            };
                }
                return result;
            }
        }
       
        /// <summary>
        /// For fabric only
        /// </summary>
        /// <param name="warehouseCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="colorCode"></param>
        /// <param name="size"></param>
        /// <param name="rollBatch"></param>
        /// <returns></returns>
        [OperationContract]
        private List<DataLayer.ItemDimensionSearchModel> GetInspectionWarehouseRows(int skip, int take,
            string sort, out int fullCount, string warehouseCode, string itemType, string itemCode,
            string colorCode, string size, string rollBatch)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;

                var SelectedItem =
                     entities.FabricAccSearches.Where(fas => fas.Code == itemCode).ToList();


                var lists = SelectedItem.Select(w => w.Iserial).ToList();

                var predicate = PredicateBuilder.True<TblStock>();

                predicate = predicate.And(i => i.TblWarehouse1.Code == warehouseCode);
                if (!string.IsNullOrWhiteSpace(itemType))

                    predicate = predicate.And(i => i.TblItemDim1.ItemType == itemType);
                if (!string.IsNullOrWhiteSpace(itemCode))
                    predicate = predicate.And(i => lists.Contains( i.TblItemDim1.ItemIserial) );
                //predicate = predicate.And(i => i.Fabric_Code == itemCode);
                if (!string.IsNullOrWhiteSpace(colorCode))
                    predicate = predicate.And(i => i.TblItemDim1.TblColor1.Code == colorCode);
                //if (!string.IsNullOrWhiteSpace(size))
                //    predicate = predicate.And(i => i.size == colorCode);
                if (!string.IsNullOrWhiteSpace(rollBatch))
                    predicate = predicate.And(i => i.TblItemDim1.BatchNo == rollBatch);

                predicate=predicate.And(e => e.Qty > 0);

                IQueryable<TblStock> query = entities.TblStocks.Include("TblWarehouse1").Include("TblItemDim1.TblColor1").AsExpandable().Where(predicate);



                //var predicate = PredicateBuilder.True<InspectionsRoute>();

                //predicate = predicate.And(i => i.FinishedWarehouse == warehouseCode);
                //if (!string.IsNullOrWhiteSpace(itemType))
                //    predicate = predicate.And(i => entities.FabricAccSearches.Any(fas => fas.ItemGroup == itemType && i.Fabric_Code == fas.Code));
                //if (!string.IsNullOrWhiteSpace(itemCode))
                //    predicate = predicate.And(i => i.Fabric_Code == itemCode);
                //if (!string.IsNullOrWhiteSpace(colorCode))
                //    predicate = predicate.And(i => i.ColorCode == colorCode);
                ////if (!string.IsNullOrWhiteSpace(size))
                ////    predicate = predicate.And(i => i.size == colorCode);
                //if (!string.IsNullOrWhiteSpace(rollBatch))
                //    predicate = predicate.And(i => i.RollBatch == rollBatch);

                //IQueryable<InspectionsRoute> query = entities.InspectionsRoutes.AsExpandable().Where(predicate);

                fullCount = query.Count();
                query = query.OrderBy(sort).Skip(skip).Take(take);

                var result = query.ToList().Select(iR => new DataLayer.ItemDimensionSearchModel()
                {
                    ItemId = GetSelectedItem(iR, SelectedItem).Iserial,
                    ItemName = GetSelectedItem(iR, SelectedItem).Name,
                    ItemType = GetSelectedItem(iR, SelectedItem).ItemGroup,
                    ItemCode = GetSelectedItem(iR, SelectedItem).Code,
                    AvailableQuantity = Convert.ToDecimal(iR.Qty),
                    TransferredQuantity = 0,

                    BatchNoFrom = iR.TblItemDim1.BatchNo ?? "",
                    BatchNoTo = iR.TblItemDim1.BatchNo ?? "",
                    SizeFrom = size ?? "",
                    SizeTo = size ?? "",
                    ColorFromCode = iR.TblItemDim1.TblColor1.Code,
                    ColorFromId = iR.TblItemDim1.TblColor,
                    ColorToId = iR.TblItemDim1.TblColor,
                    SiteFromIserial = iR.TblWarehouse1.TblSite??0,
                    SiteToIserial = iR.TblWarehouse1.TblSite ?? 0,

                    IsAcc = iR.TblItemDim1.ItemType.ToLower().Contains("acc"),
                }).ToList();
                foreach (var item in result)
                {
                    if (item.ColorFrom == null) item.ColorFrom = new DataLayer.TblColor();
                    var fromColor = entities.TblColors.FirstOrDefault(c => c.Iserial == item.ColorFromId);
                    item.ColorFrom.InjectFrom(fromColor);
                    if (item.ColorPerRow == null) item.ColorPerRow = new DataLayer.TblColor();
                    var toColor = entities.TblColors.FirstOrDefault(c => c.Iserial == item.ColorToId);
                    item.ColorPerRow.InjectFrom(toColor);
                }
                return result.ToList();
            }
        }

        private static FabricAccSearch GetSelectedItem(TblStock iR, List<FabricAccSearch> SelectedItem)
        {
            return SelectedItem.FirstOrDefault(w => w.ItemGroup == iR.TblItemDim1.ItemType);
        }
    }
}