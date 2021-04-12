using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblCurrency> GetTblCurrency(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblCurrency> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblCurrencies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblCurrencies.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblCurrencies.Count();
                    query = context.TblCurrencies.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCurrency UpdateOrInsertTblCurrency(TblCurrency newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblCurrencies.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblCurrencies
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
        private int DeleteTblCurrency(TblCurrency row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblCurrencies
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<CURRENCY> GetAxCurrency(string dataarea, out List<EXCHRATE> exrates)
        {
            using (var context = new ax2009_ccEntities())
            {
                var oldRow = (from e in context.CURRENCies
                              where e.DATAAREAID == dataarea
                              select e).ToList();

                exrates = (from e in context.EXCHRATES
                           where e.DATAAREAID == dataarea
                           select e).ToList();

                return oldRow;
            }
        }

        private decimal GetExRateByCurrency(string currency, string dataArea)
        {
            using (var context = new ax2009_ccEntities())
            {
                var result = context.EXCHRATES.Where(e => e.DATAAREAID == dataArea && e.CURRENCYCODE == currency).OrderByDescending(x => x.TODATE).FirstOrDefault();
                return result.EXCHRATE1;
            }
        }

    }
}