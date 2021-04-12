using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

// ReSharper disable once CheckNamespace
namespace CCWFM.Web.Service
{
// ReSharper disable once InconsistentNaming
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblWarehouse> GetTblWarehouse(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblWarehouse> query;
                if (filter != null)
                {                    
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblWarehouses.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblWarehouses.Include("TblSite1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblWarehouses.Count();
                    query = context.TblWarehouses.Include("TblSite1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

     
        [OperationContract]
        private TblWarehouse UpdateOrInsertTblWarehouse(TblWarehouse newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblWarehouses.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblWarehouses
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
        private int DeleteTblWarehouse(TblWarehouse row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblWarehouses
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }      
    }
}