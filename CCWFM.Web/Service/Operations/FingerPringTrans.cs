using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<FingerPrintTransaction> GetFingerPrintTransaction(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<EmployeesView> empList, out List<StoreForAllCompany> storesAllCompanies)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<FingerPrintTransaction> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.FingerPrintTransactions.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.FingerPrintTransactions.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.FingerPrintTransactions.Count();
                    query = context.FingerPrintTransactions.OrderBy(sort).Skip(skip).Take(take);
                }

                var emp = query.Select(x => x.UserId);
                empList = context.EmployeesViews.Where(x => emp.Contains(x.Emplid)).ToList();
                var stores = query.Select(x => x.StoreCode).ToList();

                using (var model = new WorkFlowManagerDBEntities())
                {
                    storesAllCompanies = model.StoreForAllCompanies.Where(w => stores.Contains(w.Code)).ToList();
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private FingerPrintTransaction UpdateOrInsertFingerPrintTransaction(FingerPrintTransaction newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.FingerPrintTransactions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.FingerPrintTransactions
                                  where e.GlSerial == newRow.GlSerial
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
        private int DeleteFingerPrintTransaction(FingerPrintTransaction row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.FingerPrintTransactions
                              where e.GlSerial == row.GlSerial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.GlSerial;
        }
    }
}