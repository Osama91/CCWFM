using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCostCenterRouteGroup> GetTblCostCenterRouteGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostCenterRouteGroup> query;
                if (filter != null)
                {
                    
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCostCenterRouteGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCostCenterRouteGroups.Include("TblCostCenter1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCostCenterRouteGroups.Count();
                    query = entity.TblCostCenterRouteGroups.Include("TblCostCenter1").OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostCenterRouteGroup UpdateOrInsertTblCostCenterRouteGroups(TblCostCenterRouteGroup newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblCostCenterRouteGroups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCostCenterRouteGroups
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
        private int DeleteTblCostCenterRouteGroup(TblCostCenterRouteGroup row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCostCenterRouteGroups
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}