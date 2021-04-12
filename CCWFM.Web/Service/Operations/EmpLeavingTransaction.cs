using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<EmpLeavingTransaction> GetEmpLeavingTransaction(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<EmpLeavingTransaction> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.EmpLeavingTransactions.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.EmpLeavingTransactions.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.EmpLeavingTransactions.Count();
                    query = context.EmpLeavingTransactions.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private EmpLeavingTransaction UpdateOrInsertEmpLeavingTransaction(EmpLeavingTransaction newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    newRow.CreationBy = user;
                    newRow.CreationDate = DateTime.Now;
                    context.EmpLeavingTransactions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.EmpLeavingTransactions
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteEmpLeavingTransaction(EmpLeavingTransaction row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.EmpLeavingTransactions
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}