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
        public List<TblGlobalRetailBusinessBudget> GetTblGlobalRetailBusinessBudget(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, int TransactionType, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblGlobalRetailBusinessBudget> query;
                if (filter != null)
                {
                    filter = filter + " and it.TransactionType ==(@TransactionType0)";
                    valuesObjects.Add("TransactionType0", TransactionType);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblGlobalRetailBusinessBudgets.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblGlobalRetailBusinessBudgets.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblGlobalRetailBusinessBudgets.Count(x => x.TransactionType == TransactionType);
                    query = context.TblGlobalRetailBusinessBudgets.OrderBy(sort).Where(v => v.TransactionType == TransactionType).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblBudgetItem> GetTblBudgetItem()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblBudgetItems.Include("TblBudgetType1").ToList();
            }
        }

        [OperationContract]
        public List<TblGlobalRetailBusinessBudgetMainDetail> GetTblGlobalRetailBusinessBudgetMainDetail(int skip, int take, int glSerial, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblGlobalRetailBusinessBudgetMainDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlobalRetailBusinessBudget ==(@Group0)";
                    valuesObjects.Add("Group0", glSerial);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblGlobalRetailBusinessBudgetMainDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblGlobalRetailBusinessBudgetMainDetails.Include("TblLkpBrandSection1").Include("TblBudgetItem1").Include("TblLkpSeason1").Include("TblGlobalRetailBusinessBudgetDetails").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblGlobalRetailBusinessBudgetMainDetails.Count(x => x.Iserial == glSerial);
                    query = context.TblGlobalRetailBusinessBudgetMainDetails.Include("TblLkpBrandSection1").Include("TblBudgetItem1").Include("TblLkpSeason1").Include("TblGlobalRetailBusinessBudgetDetails").Where(x => x.TblGlobalRetailBusinessBudget == glSerial).OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlobalRetailBusinessBudget UpdateOrInsertTblGlobalRetailBusinessBudget(TblGlobalRetailBusinessBudget newRow, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblGlobalRetailBusinessBudgets
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblGlobalRetailBusinessBudgets.AddObject(newRow);
                }

                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblGlobalRetailBusinessBudget(TblGlobalRetailBusinessBudget row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblGlobalRetailBusinessBudgets
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
                return row.Iserial;
            }
        }

        [OperationContract]
        private int DeleteTblGlobalRetailBusinessBudgetMainDetail(TblGlobalRetailBusinessBudgetMainDetail row)
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in db.TblGlobalRetailBusinessBudgetMainDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) db.DeleteObject(oldRow);

                db.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        public TblGlobalRetailBusinessBudgetMainDetail UpdateOrInsertTblGlobalRetailBusinessBudgetMainDetail(TblGlobalRetailBusinessBudgetMainDetail newRow, bool save, int index, out int outindex, int userIserial)
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                outindex = index;
                if (save)
                {
                    if (newRow.TblGlobalRetailBusinessBudget1 != null)
                    {
                        var headerRow = newRow.TblGlobalRetailBusinessBudget1;
                        var oldRow = (from e in db.TblGlobalRetailBusinessBudgets
                                      where e.Year == headerRow.Year && e.TransDate == headerRow.TransDate && headerRow.TransactionType == e.TransactionType
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            newRow.TblGlobalRetailBusinessBudget = oldRow.Iserial;
                            newRow.TblGlobalRetailBusinessBudget1 = null;
                        }
                    }
                    db.TblGlobalRetailBusinessBudgetMainDetails.AddObject(newRow);
                    db.SaveChanges();
                }
                else
                {
                    var oldRow = (from e in db.TblGlobalRetailBusinessBudgetMainDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, db);
                        foreach (var detailRow in newRow.TblGlobalRetailBusinessBudgetDetails)
                        {
                            var detailOldRow = (from e in db.TblGlobalRetailBusinessBudgetDetails
                                                where e.Iserial == detailRow.Iserial
                                                select e).SingleOrDefault();
                            if (detailOldRow != null)
                            {
                                GenericUpdate(detailOldRow, detailRow, db);
                            }
                            else
                            {
                                detailRow.TblGlobalRetailBusinessBudgetMainDetail1 = null;
                                detailRow.TblGlobalRetailBusinessBudgetMainDetail = newRow.Iserial;
                                db.TblGlobalRetailBusinessBudgetDetails.AddObject(detailRow);
                            }
                        }
                    }
                }
                db.SaveChanges();
                return newRow;
            }
        }
    }
}