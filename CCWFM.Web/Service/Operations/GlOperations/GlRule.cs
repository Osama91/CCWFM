using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblGlRuleHeader> GetTblGlRuleHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlRuleHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlRuleHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlRuleHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlRuleHeaders.Count();
                    query = entity.TblGlRuleHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlRuleHeader UpdateOrInsertTblGlRuleHeaders(TblGlRuleHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlRuleHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlRuleHeaders
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
        private int DeleteTblGlRuleHeader(TblGlRuleHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlRuleHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlRuleMainDetail> GetTblGlRuleMainDetail(int skip, int take, int ruleHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlRuleMainDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlRuleHeader ==(@ruleHeader0)";
                    valuesObjects.Add("ruleHeader0", ruleHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlRuleMainDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlRuleMainDetails.Include("TblJournalAccountType1").Include("TblCostCenterType1").Include("TblCostCenterOption1").Include("TblCostAllocationMethod1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlRuleMainDetails.Count(w => w.TblGlRuleHeader == ruleHeader);
                    query = entity.TblGlRuleMainDetails.Include("TblJournalAccountType1").Include("TblCostCenterType1").Include("TblCostCenterOption1").Include("TblCostAllocationMethod1").OrderBy(sort).Where(x => x.TblGlRuleHeader == ruleHeader).Skip(skip).Take(take);
                }

                List<int?> intList = query.Select(x => x.EntityAccount).ToList();

                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlRuleMainDetail UpdateOrInsertTblGlRuleMainDetails(TblGlRuleMainDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlRuleMainDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlRuleMainDetails
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
        private int DeleteTblGlRuleMainDetail(TblGlRuleMainDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlRuleMainDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlRuleDetail> GetTblGlRuleDetail(int skip, int take, int ruleMainDetail, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlRuleDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlRuleMainDetail ==(@ruleMainDetail0)";
                    valuesObjects.Add("ruleMainDetail0", ruleMainDetail);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlRuleDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlRuleDetails.Include("TblCostCenter1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlRuleDetails.Count(x => x.TblGlRuleMainDetail == ruleMainDetail);
                    query = entity.TblGlRuleDetails.Include("TblCostCenter1").OrderBy(sort).Where(x => x.TblGlRuleMainDetail == ruleMainDetail).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlRuleDetail UpdateOrInsertTblGlRuleDetails(TblGlRuleDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlRuleDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlRuleDetails
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
        private int DeleteTblGlRuleDetail(TblGlRuleDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlRuleDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblGlRuleJob GetGlRuleByUser(int user, string company)
        {
            var job = 0;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                job = (int)entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user).TblJob;
            }

            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlRuleJobs.Include("TblGlRuleHeader1.TblGlRuleMainDetails.TblGlRuleDetails.TblCostCenter1")
                             where e.TblJob == job
                             select e).SingleOrDefault();
                if (query != null)
                {
                    return query;
                }
                var newquery = (from e in entity.TblGlRuleJobs.Include("TblGlRuleHeader1.TblGlRuleMainDetails.TblGlRuleDetails.TblCostCenter1")
                                where e.TblJob == null
                                select e).SingleOrDefault();
                return newquery;
            }
        }
    }
}