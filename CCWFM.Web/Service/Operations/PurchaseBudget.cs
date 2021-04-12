using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblPurchaseBudgetHeader> GetTblPurchaseBudgetHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseBudgetHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseBudgetHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseBudgetHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseBudgetHeaders.Count();
                    query = context.TblPurchaseBudgetHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPurchaseBudgetHeader UpdateOrInsertTblPurchaseBudgetHeader(TblPurchaseBudgetHeader newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseBudgetHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblPurchaseBudgetHeaders.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseBudgetHeader(TblPurchaseBudgetHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseBudgetHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPurchaseBudgetDetail> GetTblPurchaseBudgetDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseBudgetDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblPurchaseBudgetHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblPurchaseBudgetDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPurchaseBudgetDetails.Include(nameof(TblPurchaseBudgetDetail.TblLkpSeason1)).Include("TblLkpBrandSection1").Include(nameof(TblPurchaseBudgetDetail.TblLkpSeason1)).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPurchaseBudgetDetails.Count(x => x.TblPurchaseBudgetHeader == groupId);
                    query = context.TblPurchaseBudgetDetails.Include(nameof(TblPurchaseBudgetDetail.TblLkpSeason1)).Include("TblLkpBrandSection1").Include(nameof(TblPurchaseBudgetDetail.TblLkpSeason1)).OrderBy(sort).Where(x => x.TblPurchaseBudgetHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPurchaseBudgetDetail UpdateOrInsertTblPurchaseBudgetDetail(TblPurchaseBudgetDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseBudgetDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblPurchaseBudgetDetails.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblPurchaseBudgetDetail DeleteTblPurchaseBudgetDetail(TblPurchaseBudgetDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPurchaseBudgetDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }   
    }
}