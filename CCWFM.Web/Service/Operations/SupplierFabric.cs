using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblSupplierFabric> GetTblSupplierFabric(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSupplierFabric> query;
                if (filter != null)
                {
                    filter = filter + " and it.Code !=(@Group0)";
                    valuesObjects.Add("Group0", "N/A");
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSupplierFabrics.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSupplierFabrics.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSupplierFabrics.Count(x => x.Code != "N/A");
                    query = context.TblSupplierFabrics.OrderBy(sort).Where(x => x.Code != "N/A").Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblSupplierFabric> SearchSupplierFabric(string supplierFabric)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblSupplierFabrics
                             where x.Code.StartsWith(supplierFabric) || x.Ename.StartsWith(supplierFabric)
                                    || x.Aname.StartsWith(supplierFabric)
                             select x).Take(20).ToList();
                return query;
            }
        }

        [OperationContract]
        private TblSupplierFabric UpdateOrInsertTblSupplierFabric(TblSupplierFabric newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblSupplierFabrics.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSupplierFabrics
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
        private int DeleteTblSupplierFabric(TblSupplierFabric row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSupplierFabrics
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}