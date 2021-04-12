using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblJournal> GetTblJournal(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, int user, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);

                var JournalList = new List<int>();

                if (SettingExist)
                {
                    JournalList = entity.TblJournalSettingEntities.Where(w =>
                    w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user)).Select(w => w.TblJournal ?? 0).ToList();
                }
                IQueryable<TblJournal> query;
                if (filter != null)
                {
                    if (JournalList.Any())
                    {
                        foreach (var variable in JournalList)
                        {
                            filter = filter + " and it.Iserial ==(@Iserial" + variable + ")";
                            valuesObjects.Add("Iserial" + variable, variable);
                        }
                    }

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblJournals.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblJournals
                         .Include("TblCurrency1").Include("TblSequence").Include("TblSequence1").Include("TblJournalType1").Include("TblJournalAccountType1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    if (JournalList.Any())
                    {
                        fullCount = entity.TblJournals.Count(w => JournalList.Contains(w.Iserial));
                        query = entity.TblJournals.Include("TblCurrency1").Include("TblSequence").Include("TblSequence1").Include("TblJournalType1").Include("TblJournalAccountType1")
                            .OrderBy(sort).Where(w => JournalList.Contains(w.Iserial)).Skip(skip).Take(take);                    
                    }
                    else
                    {
                        fullCount = entity.TblJournals.Count();
                        query = entity.TblJournals.Include("TblCurrency1").Include("TblSequence").Include("TblSequence1").Include("TblJournalType1").Include("TblJournalAccountType1")
                            .OrderBy(sort).Skip(skip).Take(take);
                    }

                }
                List<int?> intList = query.Select(x => x.Entity).ToList();

                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblJournal FindJournalByCode(string code, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblJournals
             .FirstOrDefault(v => v.Code == code);

                return query;
            }
        }

        [OperationContract]
        private TblJournal UpdateOrInsertTblJournals(TblJournal newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblJournals.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblJournals
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
        private int DeleteTblJournal(TblJournal row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblJournals
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}