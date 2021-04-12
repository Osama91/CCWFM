using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblMethodOfPayment> GetTblMethodOfPayment(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblMethodOfPayment> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblMethodOfPayments.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblMethodOfPayments.Include("TblBankTransactionType1").Include("TblJournalAccountType1").Include("TblAccount").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblMethodOfPayments.Count();
                    query = entity.TblMethodOfPayments.Include("TblBankTransactionType1").Include("TblJournalAccountType1").Include("TblAccount")
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                List<int?> intList = query.Select(x => x.Entity).ToList();
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblMethodOfPayment UpdateOrInsertTblMethodOfPayments(TblMethodOfPayment newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblMethodOfPayments.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMethodOfPayments
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
        private int DeleteTblMethodOfPayment(TblMethodOfPayment row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblMethodOfPayments
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);
                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}