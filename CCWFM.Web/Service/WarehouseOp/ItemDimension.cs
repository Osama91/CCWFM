using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Service.Operations;
using System.Linq.Dynamic;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.WarehouseOp
{
    public partial class WarehouseService
    {
        [OperationContract]
        private List<DataLayer.ItemDimensionSearchModel> GetItemDimensionsOrCreteForTransfer(
            List<DataLayer.ItemDimensionSearchModel> itemsList, string WarehouseToCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var item in itemsList)
                {
                    TblItemDim tempFrom;
                    // From
                    try
                    {
                        if (item.ItemType != null && (item.ItemType.ToLower().Contains("acc") || item.ItemType.ToLower().Contains("fp")))// Accessory
                            tempFrom = context.TblItemDims.FirstOrDefault(
                                id => id.ItemIserial == item.ItemId && id.ItemType == item.ItemType &&
                            id.TblColor == item.ColorFromId && id.Size == item.SizeFrom && id.TblSite == item.SiteFromIserial);
                        else// Fabric
                            tempFrom = context.TblItemDims.FirstOrDefault(
                                id => id.ItemIserial == item.ItemId && id.ItemType == item.ItemType
                          && id.TblColor == item.ColorFromId && id.BatchNo == item.BatchNoFrom && id.TblSite == item.SiteFromIserial);
                        if (tempFrom == null)// Not Found
                        {
                            tempFrom = new TblItemDim()
                            {
                                Iserial = 0,
                                ItemIserial = item.ItemId,
                                ItemType = item.ItemType,
                                TblColor = item.ColorFromId,
                                Size = item.SizeFrom,
                                BatchNo = item.BatchNoFrom,
                                TblSite = item.SiteFromIserial
                            };
                            context.TblItemDims.AddObject(tempFrom);
                            context.SaveChanges();
                        }
                        item.ItemDimFromIserial = tempFrom.Iserial;
                    }
                    catch (System.Exception ex) { throw ex; }

                    // To
                    try
                    {
                        TblItemDim tempTo;
                        if (item.ItemType != null && (item.ItemType.ToLower().Contains("acc") || item.ItemType.ToLower().Contains("fp")))// Accessory
                            tempTo = context.TblItemDims.FirstOrDefault(
                                id => id.ItemIserial == item.ItemId && id.ItemType == item.ItemType &&
                            id.TblColor == item.ColorToId && id.Size == item.SizeTo && id.TblSite == item.SiteToIserial);
                        else// Fabric
                            tempTo = context.TblItemDims.FirstOrDefault(
                                id => id.ItemIserial == item.ItemId && id.ItemType == item.ItemType
                          && id.TblColor == item.ColorToId && id.BatchNo == item.BatchNoTo && id.TblSite == item.SiteToIserial);
                        if (tempTo == null)// Not Found
                        {
                            tempTo = new TblItemDim()
                            {
                                Iserial = 0,
                                ItemIserial = item.ItemId,
                                ItemType = item.ItemType,
                                TblColor = item.ColorToId,
                                Size = item.SizeTo,
                                BatchNo = item.BatchNoTo,
                                TblSite = item.SiteToIserial
                            };
                            context.TblItemDims.AddObject(tempTo);
                            context.SaveChanges();
                        }
                        item.ItemDimToIserial = tempTo.Iserial;
                        item.AvailableToQuantity = GetItemQuantityByWarehouse(context, WarehouseToCode, tempTo.Iserial);
                        item.PendingToQuantity = GetItemPendingByWarehouse(context, WarehouseToCode, tempTo.Iserial);
                    }
                    catch (System.Exception ex) { throw ex; }
                }
                return itemsList;
            }
        }

        [OperationContract]
        private List<TblItemDim> GetItemDimension(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblItemDim> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblItemDims.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblItemDims.Where(
                        filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblItemDims.Count();
                    query = context.TblItemDims.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblItemDim UpdateOrInsertItemDimension(TblItemDim newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblItemDims
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblItemDims.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private List<string> GetItemGroup()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                List<string> result = new List<string>();
                result = context.FabricAccSearches.Select(fas => fas.ItemGroup).Distinct().ToList();
                return result;
            }
        }

    }
}