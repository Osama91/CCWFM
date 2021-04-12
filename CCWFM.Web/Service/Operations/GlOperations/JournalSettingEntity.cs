using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblJournalSettingEntity> GetTblJournalSettingEntity(int skip, int take, int TblJournalSetting, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<Entity> entityList, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblJournalSettingEntity> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblJournalSetting ==(@Group0)";
                    valuesObjects.Add("Group0", TblJournalSetting);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblJournalSettingEntities.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblJournalSettingEntities
                        .Include(nameof(TblJournalSettingEntity.TblJournalAccountType1))
                        .Include(nameof(TblJournalSettingEntity.TblJournal1))
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblJournalSettingEntities.Count(w=>w.TblJournalSetting== TblJournalSetting);
                    query = entity.TblJournalSettingEntities.Include(nameof(TblJournalSettingEntity.TblJournalAccountType1))
                        .Include(nameof(TblJournalSettingEntity.TblJournal1))
                        .OrderBy(sort).Where(w => w.TblJournalSetting == TblJournalSetting).Skip(skip).Take(take);
                }


                List<int?> intList = query.Select(x => x.EntityAccount).ToList();
                
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
            
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblJournalSettingEntity UpdateOrInsertTblJournalSettingEntitys(TblJournalSettingEntity newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblJournalSettingEntities.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblJournalSettingEntities
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
        private List<int> DeleteTblJournalSettingEntity(List<int> ListOfRows, string company)
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