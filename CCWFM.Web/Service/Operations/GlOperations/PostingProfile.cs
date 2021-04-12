using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblPostingProfileHeader> GetTblPostingProfileHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPostingProfileHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblPostingProfileHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblPostingProfileHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblPostingProfileHeaders.Count();
                    query = entity.TblPostingProfileHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblPostingProfileDetail> GetTblPostingProfileDetail(int skip, int take, int PostingProfileHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPostingProfileDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPostingProfileHeader ==(@Group0)";
                    valuesObjects.Add("Group0", PostingProfileHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblPostingProfileDetails.Where(filter, parameterCollection).Count();
                    query = entity.TblPostingProfileDetails.Include("TblAccount1").Include("TblJournalAccountType").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblPostingProfileDetails.Count(v => v.TblPostingProfileHeader == PostingProfileHeader);
                    query = entity.TblPostingProfileDetails.Include("TblAccount1").Include("TblJournalAccountType").OrderBy(sort).Where(v => v.TblPostingProfileHeader == PostingProfileHeader).Skip(skip).Take(take);
                }
                List<int> intList = query.Select(x => x.Entity).ToList();

                List<int> intTypeList = query.Select(x => x.Type).ToList();

                List<int?> intScopeList = query.Select(x => x.Scope).ToList();

                entityList = entity.Entities.Where(x => intScopeList.Contains(x.scope) && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPostingProfileHeader UpdateOrInsertTblPostingProfileHeaders(TblPostingProfileHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblPostingProfileHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblPostingProfileHeaders
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
        private TblPostingProfileDetail UpdateOrInsertTblPostingProfileDetails(TblPostingProfileDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblPostingProfileDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblPostingProfileDetails
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
        private int DeleteTblPostingProfileHeader(TblPostingProfileHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblPostingProfileHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int DeleteTblPostingProfileDetail(TblPostingProfileDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblPostingProfileDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}