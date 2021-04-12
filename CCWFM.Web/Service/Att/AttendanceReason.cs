using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Att
{
    public partial class AttService
    {
        [OperationContract]
        private List<TblAttendanceFileReason> GetTblAttendanceFileReason(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<TblAttendanceFileReason> query;
                if (filter != null)
                {
                    var parameterCollection = Operations.SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblAttendanceFileReasons.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAttendanceFileReasons.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAttendanceFileReasons.Count();
                    query = context.TblAttendanceFileReasons.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAttendanceFileReason UpdateOrInsertTblAttendanceFileReason(TblAttendanceFileReason newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.TblAttendanceFileReasons.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblAttendanceFileReasons
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        Operations.SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblAttendanceFileReason(TblAttendanceFileReason row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblAttendanceFileReasons
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}