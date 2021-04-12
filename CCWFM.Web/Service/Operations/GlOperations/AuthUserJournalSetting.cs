using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblAuthUserJournalSetting> GetTblAuthUserJournalSetting(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblAuthUserJournalSetting> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblAuthUserJournalSettings.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblAuthUserJournalSettings
                        .Include(nameof(TblAuthUserJournalSetting.TblJournalSetting1))
                        
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblAuthUserJournalSettings.Count();
                    query = entity.TblAuthUserJournalSettings.Include(nameof(TblAuthUserJournalSetting.TblJournalSetting1))
                        
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAuthUserJournalSetting UpdateOrInsertTblAuthUserJournalSettings(TblAuthUserJournalSetting newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblAuthUserJournalSettings.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblAuthUserJournalSettings
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
        private List<int> DeleteTblAuthUserJournalSetting(List<int> ListOfRows, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                foreach (var row in ListOfRows)
                {
                    var query = (from e in entity.TblAuthUserJournalSettings
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