using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblSalesOrderOperation> GetTblSalesOrderOperation(int salesOrder, out List<TblRouteGroupBomBase> routeBaseGroup)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSalesOrderOperations.Include("TblSalesOrderOperationDetails.TblColor1").Include("TblRouteGroup").Where(x => x.TblSalesOrder == salesOrder);
                if (query.Any())
                {
                    routeBaseGroup = null;
                }
                else
                {
                    routeBaseGroup = context.TblRouteGroupBomBases.Include("TblRouteGroup1").ToList();
                }
                
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderOperation UpdateOrInsertTblSalesOrderOperation(TblSalesOrderOperation newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    foreach (var variable in newRow.TblSalesOrderOperationDetails.ToList())
                    {
                        variable.TblColor1 = null;
                    }

                    context.TblSalesOrderOperations.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSalesOrderOperations
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);

                        foreach (var newDetailrow in newRow.TblSalesOrderOperationDetails.ToList())
                        {
                            newDetailrow.TblColor1 = null;
                            newDetailrow.TblSalesOrderOperation1 = null;
                            newDetailrow.TblSalesOrderOperation = newRow.Iserial;
                            var oldDetailRow = (from e in context.TblSalesOrderOperationDetails
                                                where e.Iserial == newDetailrow.Iserial
                                                select e).SingleOrDefault();
                            if (oldDetailRow != null)
                            {
                                GenericUpdate(oldDetailRow, newDetailrow, context);
                            }
                            else
                            {
                                context.TblSalesOrderOperationDetails.AddObject(newDetailrow);
                            }
                        }
                    }
                }
                context.SaveChanges();
                var row = (from e in context.TblSalesOrderOperations.Include("TblSalesOrderOperationDetails")
                           where e.Iserial == newRow.Iserial
                           select e).SingleOrDefault();
                return row;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderOperation(TblSalesOrderOperation row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderOperations
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int DeleteTblSalesOrderOperationDetail(TblSalesOrderOperationDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderOperationDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}