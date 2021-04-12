using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<shoparea> Getshoparea(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<TblStore> StoreList, out List<TblItemDownLoadDef> BrandList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<shoparea> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.shopareas.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.shopareas.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.shopareas.Count();
                    query = entity.shopareas.OrderBy(sort).Skip(skip).Take(take);
                }
                List<string> ListCodes = query.Select(x => x.shopcode).ToList();
                List<string> ListBrand = query.Select(x => x.Brand).ToList();
                StoreList = entity.TblStores.Where(x => ListCodes.Contains(x.code)).ToList();
                BrandList = entity.TblItemDownLoadDefs.Where(x => ListBrand.Contains(x.Code)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private shoparea UpdateOrInsertshoparea(shoparea newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.shopareas.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.shopareas
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int Deleteshoparea(shoparea row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.shopareas
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
        [OperationContract]
        private List<TblCostCenterShopArea> GetTblCostCenterShopArea(int skip, int take, int shoparea, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostCenterShopArea> query;
                if (filter != null)
                {
                    filter = filter + " and it.shoparea ==(@shoparea0)";
                    valuesObjects.Add("shoparea0", shoparea);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCostCenterShopAreas.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCostCenterShopAreas.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCostCenterShopAreas.Count(x => x.shoparea == shoparea);
                    query = entity.TblCostCenterShopAreas.OrderBy(sort).Where(x => x.shoparea == shoparea).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostCenterShopArea UpdateOrInsertTblCostCenterShopAreas(TblCostCenterShopArea newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblCostCenterShopAreas.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCostCenterShopAreas
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblCostCenterShopArea(TblCostCenterShopArea row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCostCenterShopAreas
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}