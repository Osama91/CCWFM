using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblEmpWeeklyDayOff> GetTblEmpWeeklyDayOff(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, out List<EmployeesView> EmpList)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<TblEmpWeeklyDayOff> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblEmpWeeklyDayOffs.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblEmpWeeklyDayOffs.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblEmpWeeklyDayOffs.Count();
                    query = context.TblEmpWeeklyDayOffs.OrderBy(sort).Skip(skip).Take(take);
                }

                var querylist = query.Select(x => x.Emp);

                EmpList =
              context.EmployeesViews.Where(x => querylist.Any(l => x.Emplid == l)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblEmpWeeklyDayOff UpdateOrInsertTblEmpWeeklyDayOff(TblEmpWeeklyDayOff newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    newRow.CreationBy = user;
                    newRow.CreationDate = DateTime.Now;
                    context.TblEmpWeeklyDayOffs.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblEmpWeeklyDayOffs
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
        private int DeleteTblEmpWeeklyDayOff(TblEmpWeeklyDayOff row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblEmpWeeklyDayOffs
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}