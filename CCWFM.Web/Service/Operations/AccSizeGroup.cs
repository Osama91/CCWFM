using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblAccSizeGroup> GetTblAccSizeGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblAccSizeGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblAccSizeGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAccSizeGroups.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAccSizeGroups.Count();
                    query = context.TblAccSizeGroups.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAccSizeGroup UpdateOrInsertTblAccSizeGroup(TblAccSizeGroup newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAccSizeGroups
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblAccSizeGroups.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblAccSizeGroup(TblAccSizeGroup row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAccSizeGroups
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblAccSize> GetTblAccSizeCode(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblAccSize> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblAccSizeGroup ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblAccSizes.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAccSizes.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAccSizes.Count(x => x.TblAccSizeGroup == groupId);
                    query = context.TblAccSizes.OrderBy(sort).Where(x => x.TblAccSizeGroup == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAccSize UpdateOrInsertTblAccSizeCode(TblAccSize newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAccSizes
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblAccSizes.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblAccSize DeleteTblAccSizeCode(TblAccSize row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAccSizes
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }  
    }
}