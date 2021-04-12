using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblTransferMsg> GetTblTransferMsg(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, string code, out int fullCount)
        {
            using (var model = new TimeAttEntities())
            {
                var emp = model.EmployeesViews.FirstOrDefault(x => x.Emplid == code);

                using (var context = new WorkFlowManagerDBEntities())
                {
                    IQueryable<TblTransferMsg> query;
                    if (filter != null)
                    {
                        filter = filter + " and it.Company ==(@Company0)";
                        valuesObjects.Add("Company0", emp.Company);
                        filter = filter + " and it.Store ==(@Store0)";
                        valuesObjects.Add("Store0", emp.Store);
                        var parameterCollection = ConvertToParamters(valuesObjects);

                        fullCount = context.TblTransferMsgs.Where(filter, parameterCollection.ToArray()).Count();
                        query = context.TblTransferMsgs.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                    }
                    else
                    {
                        fullCount = context.TblTransferMsgs.Count(x => x.Company == emp.Company && x.Store == emp.Store);
                        query = context.TblTransferMsgs.Where(x => x.Company == emp.Company && x.Store == emp.Store).OrderBy(sort).Skip(skip).Take(take);
                    }
                    return query.ToList();
                }
            }
        }

        [OperationContract]
        private TblTransferMsg UpdateOrInsertTblTransferMsg(TblTransferMsg newRow, bool save, int index,
            out int outindex, string code)
        {
            outindex = index;
            using (var model = new TimeAttEntities())
            {
                var emp = model.EmployeesViews.FirstOrDefault(x => x.Emplid == code);
                newRow.Company = emp.Company;
                newRow.Store = emp.Store;
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (save)
                    {
                        context.TblTransferMsgs.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblTransferMsgs
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
        }

        [OperationContract]
        private int DeleteTblTransferMsg(TblTransferMsg row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblTransferMsgs
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}