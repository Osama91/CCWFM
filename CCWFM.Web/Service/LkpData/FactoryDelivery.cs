using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        [OperationContract]
        private List<TblFactoryDelivery> GetTblFactoryDelivery(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
             
                IQueryable<TblFactoryDelivery> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblFactoryDeliveries.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblFactoryDeliveries.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblFactoryDeliveries.Count();
                    query = context.TblFactoryDeliveries.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblFactoryDelivery UpdateOrInsertTblFactoryDelivery(TblFactoryDelivery newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblFactoryDeliveries.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblFactoryDeliveries
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblFactoryDelivery(TblFactoryDelivery row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblFactoryDeliveries
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}