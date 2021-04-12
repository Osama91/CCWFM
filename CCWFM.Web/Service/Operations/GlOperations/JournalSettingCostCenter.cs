using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblJournalSettingCostCenter> GetTblJournalSettingCostCenter(int skip, int take,int TblJournalSetting, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var CostCenter = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblJournalSettingCostCenter> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblJournalSetting ==(@Group0)";
                    valuesObjects.Add("Group0", TblJournalSetting);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = CostCenter.TblJournalSettingCostCenters.Where(filter, parameterCollection.ToArray()).Count();
                    query = CostCenter.TblJournalSettingCostCenters
                        .Include(nameof(TblJournalSettingCostCenter.TblCostCenter1))
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = CostCenter.TblJournalSettingCostCenters.Count(w=>w.TblJournalSetting== TblJournalSetting);
                    query = CostCenter.TblJournalSettingCostCenters
                        .Include(nameof(TblJournalSettingCostCenter.TblCostCenter1))
                        .OrderBy(sort).Where(w => w.TblJournalSetting == TblJournalSetting).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblJournalSettingCostCenter UpdateOrInsertTblJournalSettingCostCenters(TblJournalSettingCostCenter newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var CostCenter = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    CostCenter.TblJournalSettingCostCenters.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in CostCenter.TblJournalSettingCostCenters
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, CostCenter);
                    }
                }
                CostCenter.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private List<int> DeleteTblJournalSettingCostCenter(List<int> ListOfRows, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                foreach (var row in ListOfRows)
                {
                    var query = (from e in entity.TblJournalSettingCostCenters
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