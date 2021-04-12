using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblVisaMachine> GetTblVisaMachine(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company,
            out List<Entity> EntityAccounts)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblVisaMachine> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblVisaMachines.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblVisaMachines.Include(nameof(TblVisaMachine.TblBank1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblVisaMachines.Count();
                    query = entity.TblVisaMachines.Include(nameof(TblVisaMachine.TblBank1)).OrderBy(sort).Skip(skip).Take(take);
                }
                List<int> intList = query.Select(x => x.EntityAccount).ToList();
                //TblJournalAccountType = 15 -- Expenses
                EntityAccounts = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && x.TblJournalAccountType == 15).ToList();
                return query.ToList();
            }
        }
        [OperationContract]
        private TblVisaMachine UpdateOrInsertTblVisaMachine(TblVisaMachine newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblVisaMachines.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblVisaMachines
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
        private int DeleteTblVisaMachine(TblVisaMachine row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblVisaMachines
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblBank> GetVisaMachineBanks(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblBanks
                             select e).ToList();
                return query;

            }
        }
    }
}