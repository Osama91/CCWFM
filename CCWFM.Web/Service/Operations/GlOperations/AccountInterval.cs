using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblAccountInterval> GetTblAccountInterval(int skip, int take, int tblAccount, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblAccountInterval> query;
                if (filter != null)
                {
                    filter = filter + " and it.tblAccount ==(@tblAccount0)";
                    valuesObjects.Add("tblAccount0", tblAccount);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblAccountIntervals.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblAccountIntervals.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblAccountIntervals.Count(x => x.TblAccount == tblAccount);
                    query = entity.TblAccountIntervals.OrderBy(sort).Where(x => x.TblAccount == tblAccount).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblAccountInterval UpdateOrInsertTblAccountIntervals(TblAccountInterval newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblAccountIntervals.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblAccountIntervals
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
        private int DeleteTblAccountInterval(TblAccountInterval row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblAccountIntervals
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}