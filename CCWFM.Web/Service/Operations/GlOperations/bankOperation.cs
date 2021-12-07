using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblBank> GetTblBank(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBank> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblBanks.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblBanks.Include(nameof(TblBank.TblBankGroup1)).Include(nameof(TblBank.TblCurrency1)).Include(nameof(TblBank.TblBankAccountType1))
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblBanks.Count();
                    query = entity.TblBanks.Include(nameof(TblBank.TblBankGroup1)).Include(nameof(TblBank.TblCurrency1)).Include(nameof(TblBank.TblBankAccountType1))
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblBank UpdateOrInsertTblBanks(TblBank newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblBanks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblBanks
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
        private int DeleteTblBank(TblBank row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblBanks
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();



                if (query != null)
                {

                    var Ledger = entity.TblLedgerMainDetails.Where(w => w.EntityAccount == query.Iserial);
                    if (Ledger.Any())
                    {
                        throw new System.Exception("Cannot delete Because There is a transaction Exisit");
                    }

                    entity.DeleteObject(query);
                }

                entity.SaveChanges();
            }
            return row.Iserial;
        }


        public List<TblBank> GetBanks(int skip, int take, string sort, string filter,
          Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBank> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblBanks.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblBanks.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblBanks.Count();
                    query = context.TblBanks.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

    }
}