using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblJournalAccountType> GetTblJournalAccountType(bool manual, string company)
        {
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblJournalAccountType> query = entityGroup.TblJournalAccountTypes.Where(x => x.Manual == manual);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblGlGenEntityGroup> GetTblGlGenEntityGroup(int skip, int take, int tblJournalAccountType, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlGenEntityGroup> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblJournalAccountType ==(@Group0)";
                    valuesObjects.Add("Group0", tblJournalAccountType);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entityGroup.TblGlGenEntityGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = entityGroup.TblGlGenEntityGroups.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entityGroup.TblGlGenEntityGroups.Count(x => x.TblJournalAccountType == tblJournalAccountType);
                    query = entityGroup.TblGlGenEntityGroups.OrderBy(sort).Where(v => v.TblJournalAccountType == tblJournalAccountType).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlGenEntityGroup UpdateOrInsertTblGlGenEntityGroup(TblGlGenEntityGroup newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {                    
                    entityGroup.TblGlGenEntityGroups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entityGroup.TblGlGenEntityGroups
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entityGroup);
                    }
                }
                entityGroup.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblGlGenEntityGroup(TblGlGenEntityGroup row, int index, string company)
        {
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entityGroup.TblGlGenEntityGroups
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null)
                {

                    var Ledger = entityGroup.TblLedgerMainDetails.Where(w => w.EntityAccount == query.Iserial);
                    if (Ledger.Any())
                    {
                        throw new System.Exception("Cannot delete Because There is a transaction Exisit");
                    }

                    entityGroup.DeleteObject(query);
                }

                entityGroup.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlGenEntity> GetTblGlGenEntity(int skip, int take, int group, int tblJournalAccountType, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlGenEntity> query;
                if (filter != null)
                {
                    filter = filter + " and it.tblJournalAccountType ==(@Group0)";
                    valuesObjects.Add("Group0", tblJournalAccountType);
                    filter = filter + " and it.TblGlGenEntityGroup ==(@Group0)";
                    valuesObjects.Add("Group0", group);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlGenEntities.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlGenEntities.Include("TblGlGenEntityGroup1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlGenEntities.Count(x => x.TblGlGenEntityGroup == group && x.TblJournalAccountType == tblJournalAccountType);
                    query = entity.TblGlGenEntities.Include("TblGlGenEntityGroup1").OrderBy(sort).Where(v => v.TblGlGenEntityGroup == group && v.TblJournalAccountType == tblJournalAccountType).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlGenEntity UpdateOrInsertTblGlGenEntity(TblGlGenEntity newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlGenEntities.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlGenEntities
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
        private int DeleteTblGlGenEntity(TblGlGenEntity row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlGenEntities
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}