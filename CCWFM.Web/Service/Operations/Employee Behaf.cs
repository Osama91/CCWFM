using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
   {
           [OperationContract]
        public List<BehalfFiltered> GetBehalfFiltereds(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, string empCode, out int fullCount)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<BehalfFiltered> query;
                if (filter != null)
                {
                    filter = filter + " and it.ManagerId ==(@EmpCode)";
                    valuesObjects.Add("EmpCode0", empCode);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.BehalfFiltereds.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.BehalfFiltereds.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.BehalfFiltereds.Count(x => x.ManagerId == empCode);
                    query = context.BehalfFiltereds.OrderBy(sort).Where(v => v.ManagerId == empCode).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        
        [OperationContract]
        public List<EmployeesView> GetAllEmp()
        {
            using (var db = new TimeAttEntities())
            {
                var q = db.EmployeesViews.ToList();

                return q;
            }
        }

        [OperationContract]
        private List<TblEmployeeBehalf> GetTblEmployeeBehalf(ObservableCollection<string> empList)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<TblEmployeeBehalf> query = context.TblEmployeeBehalves.Where(z => empList.Contains(z.Emplid));

                return query.ToList();
            }
        }

        [OperationContract]
        private TblEmployeeBehalf UpdateOrInsertTblEmployeeBehalf(TblEmployeeBehalf newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.TblEmployeeBehalves.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblEmployeeBehalves
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
        private int DeleteTblEmployeeBehalf(TblEmployeeBehalf row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblEmployeeBehalves
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}