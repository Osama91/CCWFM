using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCashDepositType> GetTblCashDepositType(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCashDepositType> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCashDepositTypes.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCashDepositTypes.Include(nameof(TblCashDepositType.TblSequence1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCashDepositTypes.Count();
                    query = entity.TblCashDepositTypes.Include(nameof(TblCashDepositType.TblSequence1)).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }
        [OperationContract]
        private TblCashDepositType UpdateOrInsertTblCashDepositType(TblCashDepositType newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    int MaxIserial = entity.TblCashDepositTypes.Max(x => x.Iserial);
                    newRow.Iserial = MaxIserial + 1;
                    entity.TblCashDepositTypes.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCashDepositTypes
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
        private int DeleteTblCashDepositTypes(TblCashDepositType row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCashDepositTypes
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSequence> GetCashDepositeSequence(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblSequences
                             select e).ToList();
                return query;

            }
        }
    }
}