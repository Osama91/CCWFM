using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_FabricAttriputes> GetFabricWithUnitAndDyeingClass(string fabric)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return
                    context.tbl_FabricAttriputes.Where(
                        x => x.FabricDescription.StartsWith(fabric) || x.FabricDescriptionAR.StartsWith(fabric) || x.FabricID.StartsWith(fabric)).Take(50)
                        .ToList();
            }
        }

        [OperationContract]
        public List<FabricAccSearch> GetItemWithUnitAndItemGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<FabricAccSearch> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.FabricAccSearches.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.FabricAccSearches.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.FabricAccSearches.Count();
                    query = context.FabricAccSearches.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        public List<FabricServiceSearch> GetFabricServiceSearch(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<FabricServiceSearch> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.FabricServiceSearches.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.FabricServiceSearches.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.FabricServiceSearches.Count();
                    query = context.FabricServiceSearches.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private ItemsDto AccWithConfigAndSize(ItemsDto item)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var comList =
                    (from x in context.tbl_AccessoryAttributesDetails
                     where (x.Code == (item.Code))
                     select x).ToList();

                var criteria = comList.Select(x => x.Configuration).ToList();
                //var result_tag = (from i in _ctx.Items
                //                  where criteria.Contains(i.ID)
                //                  select i).ToList();
                //var result = from i in result_tag
                //             join s in itemScores on i.ID equals s._id
                //             orderby s._score descending
                //             select new ItemSearchResult(i, s._score);

                var accList =
                   (from c in context.TblColors
                        ///    join x in comList on c.Code equals x.Configuration

                    where (c.TblLkpColorGroup == 24 && criteria.Contains(c.Code))
                    orderby c.Code
                    select (c)).ToList();

                item.CombinationList = comList;
                item.AccConfigList = accList;
                return item;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<ItemsDto> GetFinishedFabrics(string fabric)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                             where (
                          x.FabricDescription.StartsWith(fabric) || x.FabricDescriptionAR.StartsWith(fabric)
                          || x.FabricID.StartsWith(fabric) || x.tbl_lkp_UoM.Code.StartsWith(fabric) || x.tbl_lkp_FabricMaterials.Code.StartsWith(fabric))
                             select new ItemsDto
                             {
                                 Iserial = x.Iserial,
                                 Code = x.FabricID,
                                 Name = x.FabricDescription,
                                 ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                 Unit = x.tbl_lkp_UoM.Ename
                             }).Take(20).ToList();
                return query;
            }
        }

        [OperationContract]
        private List<tbl_FabricAttriputes> GetTblFabricAttriputes(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_FabricAttriputes> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.tbl_FabricAttriputes.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tbl_FabricAttriputes.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_FabricAttriputes.Count();
                    query = context.tbl_FabricAttriputes.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        private void CreateTblItemCombined(string code, int tblColor, string batch, string size)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblItemCombineds.Where(
                           x => x.Code == code && x.TblColor == tblColor && x.Batch == batch && x.Size == size);
                if (!query.Any())
                {
                    var row = new TblItemCombined { Code = code, TblColor = tblColor, Batch = batch, Size = size };

                    context.TblItemCombineds.AddObject(row);
                    context.SaveChanges();
                }
            }
        }
    }
}