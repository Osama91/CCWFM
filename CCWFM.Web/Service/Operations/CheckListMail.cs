using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblCheckListMail> GetTblCheckListMail(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<Employee> EmpList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblCheckListMail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblCheckListMails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblCheckListMails.Include("TblCheckListGroup1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblCheckListMails.Count();
                    query = context.TblCheckListMails.Include("TblCheckListGroup1").OrderBy(sort).Skip(skip).Take(take);
                }
                var emp = query.Select(x => x.Emp);
                EmpList = context.Employees.Where(x => emp.Contains(x.EMPLID)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCheckListMail UpdateOrInsertTblCheckListMail(TblCheckListMail newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblCheckListMails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblCheckListMails
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
        private int DeleteTblCheckListMail(TblCheckListMail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblCheckListMails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}