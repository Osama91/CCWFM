using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblGlRuleJob> GetTblGlRuleJob(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<TblAuthJob> userList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlRuleJob> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlRuleJobs.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlRuleJobs.Include("TblGlRuleHeader1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlRuleJobs.Count();
                    query = entity.TblGlRuleJobs.Include("TblGlRuleHeader1").OrderBy(sort).Skip(skip).Take(take);
                }
                var listofIserials = query.Select(x => x.TblJob).Distinct().ToArray();
                using (var context = new WorkFlowManagerDBEntities())
                {
                    userList = listofIserials.Any() ? context.TblAuthJobs.Where(x => listofIserials.Any(l => x.Iserial == l)).ToList() : null;
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlRuleJob UpdateOrInsertTblGlRuleJobs(TblGlRuleJob newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlRuleJobs.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlRuleJobs
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
        private int DeleteTblGlRuleJob(TblGlRuleJob row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlRuleJobs
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}