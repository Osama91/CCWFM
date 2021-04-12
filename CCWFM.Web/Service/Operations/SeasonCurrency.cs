using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblSeasonCurrency> GetTblSeasonCurrency(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSeasonCurrency> query;
                if (filter != null)
                {                   
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblSeasonCurrencies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSeasonCurrencies.Include("TblLkpSeason1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSeasonCurrencies.Count();
                    query = context.TblSeasonCurrencies.Include("TblLkpSeason1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

   
        [OperationContract]
        private TblSeasonCurrency UpdateOrInsertTblSeasonCurrency(TblSeasonCurrency newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblSeasonCurrencies.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSeasonCurrencies
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
        private int DeleteTblSeasonCurrency(TblSeasonCurrency row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSeasonCurrencies
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}