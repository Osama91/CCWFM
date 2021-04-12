using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCostCenterInterval> GetTblCostCenterInterval(int skip, int take, int tblCostCenter, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostCenterInterval> query;
                if (filter != null)
                {
                    filter = filter + " and it.tblCostCenter ==(@tblCostCenter0)";
                    valuesObjects.Add("tblCostCenter0", tblCostCenter);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCostCenterIntervals.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCostCenterIntervals.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCostCenterIntervals.Count(x => x.TblCostCenter == tblCostCenter);
                    query = entity.TblCostCenterIntervals.OrderBy(sort).Where(x => x.TblCostCenter == tblCostCenter).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostCenterInterval UpdateOrInsertTblCostCenterIntervals(TblCostCenterInterval newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblCostCenterIntervals.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCostCenterIntervals
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
        private int DeleteTblCostCenterInterval(TblCostCenterInterval row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCostCenterIntervals
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}