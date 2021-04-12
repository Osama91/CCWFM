using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblGeneratePurchaseHeaderCurrency> TblGeneratePurchaseHeaderCurrency(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblGeneratePurchaseHeaderCurrency> query;
                if (filter != null)
                {                   
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblGeneratePurchaseHeaderCurrencies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblGeneratePurchaseHeaderCurrencies.Include("TblGeneratePurchaseHeader1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblGeneratePurchaseHeaderCurrencies.Count();
                    query = context.TblGeneratePurchaseHeaderCurrencies.Include("TblGeneratePurchaseHeader1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

   
        [OperationContract]
        private TblGeneratePurchaseHeaderCurrency UpdateOrInsertTblGeneratePurchaseHeaderCurrency(TblGeneratePurchaseHeaderCurrency newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblGeneratePurchaseHeaderCurrencies.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblGeneratePurchaseHeaderCurrencies
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
        private int DeleteTblGeneratePurchaseHeaderCurrency(TblGeneratePurchaseHeaderCurrency row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblGeneratePurchaseHeaderCurrencies
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}