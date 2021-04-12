using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblJournalSetting> GetTblJournalSetting(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblJournalSetting> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblJournalSettings.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblJournalSettings
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblJournalSettings.Count();
                    query = entity.TblJournalSettings
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblJournalAccountType> GetJournalAccountTypeSetting(int user, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var listOfInt = entity.TblAuthUserJournalSettings.Where(w => w.TblAuthUser == user).Select(w => w.TblJournalSetting).Distinct().ToList();
                var query =new  List<TblJournalAccountType>();
                if (listOfInt.Any())
                {
                     query = entity.TblJournalSettingEntities.Include("TblJournalAccountType1").Where(w =>w.TblJournalAccountType !=null&& listOfInt.Contains(w.TblJournalSetting ?? 0)).Select(t => t.TblJournalAccountType1).Distinct().ToList();

                }
                else
                {
                    query = entity.TblJournalAccountTypes.ToList();
                }
                if (!query.Any())
                {
                    query = entity.TblJournalAccountTypes.ToList();
                }
                return query;
            }
        }


        [OperationContract]
        private TblJournalSetting UpdateOrInsertTblJournalSettings(TblJournalSetting newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblJournalSettings.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblJournalSettings
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
        private List<int> DeleteTblJournalSetting(List<int> ListOfRows, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                foreach (var row in ListOfRows)
                {
                    var query = (from e in entity.TblJournalSettingEntities
                                 where e.Iserial == row
                                 select e).SingleOrDefault();
                    if (query != null)
                    {
                        entity.DeleteObject(query);
                    }
                }
                entity.SaveChanges();
            }
            return ListOfRows.ToList();
        }
    }
}