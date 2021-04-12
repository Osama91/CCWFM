using System;
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
        public List<TblBrandBudgetHeader> GetTblBrandBudgetHeader(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, int TransactionType, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblBrandBudgetHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TransactionType ==(@TransactionType0)";
                    valuesObjects.Add("TransactionType0", TransactionType);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblBrandBudgetHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblBrandBudgetHeaders.Include("TblLkpBrandSection1").Include("TblLkpSeason1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblBrandBudgetHeaders.Count(x => x.TransactionType == TransactionType);
                    query = context.TblBrandBudgetHeaders.Include("TblLkpBrandSection1").Include("TblLkpSeason1").OrderBy(sort).Where(v => v.TransactionType == TransactionType).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblBrandBudgetDetail> GetTblBrandBudgetDetail(int skip, int take, int glSerial, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblBrandBudgetDetail> query;
             


                if (filter != null)
                {
                    filter = filter + " and it.TblBrandBudgetHeader ==(@Group0)";
                    valuesObjects.Add("Group0", glSerial);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblBrandBudgetDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblBrandBudgetDetails.Include("TblFamily1").Include("TblSalesOrderColorTheme1").Include("TblFactoryGroup1").Include("TblLkpDirection1").Include("TblStyleCategory1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblBrandBudgetDetails.Count(x => x.Iserial == glSerial);
                    query = context.TblBrandBudgetDetails.Include("TblFamily1").Include("TblSalesOrderColorTheme1").Include("TblFactoryGroup1").Include("TblLkpDirection1").Include("TblStyleCategory1").Where(x => x.TblBrandBudgetHeader == glSerial).OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblBrandBudgetHeader UpdateOrInsertTblBrandBudgetHeader(TblBrandBudgetHeader newRow, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblBrandBudgetHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var temp = GenericUpdate(oldRow, newRow, context);

                    if (temp.Any())
                    {
                        newRow.LastUpdatedDate = DateTime.Now;
                        newRow.LastUpdatedBy = userIserial;
                    }
                }
                else
                {
                    newRow.LastUpdatedDate = DateTime.Now;
                    newRow.CreatedBy = userIserial;
                    context.TblBrandBudgetHeaders.AddObject(newRow);
                }

                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblBrandBudgetHeader(TblBrandBudgetHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblBrandBudgetHeaders
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
        private int DeleteTblBrandBudgetDetail(TblBrandBudgetDetail row)
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in db.TblBrandBudgetDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) db.DeleteObject(oldRow);

                db.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        public TblBrandBudgetDetail UpdateOrInsertTblBrandBudgetDetail(TblBrandBudgetDetail newRow, bool save, int index, out int outindex, int userIserial)
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                outindex = index;
                if (save)
                {
                    db.TblBrandBudgetDetails.AddObject(newRow);
                    db.SaveChanges();
                    if (newRow.TblBrandBudgetHeader1 != null)
                    {
                        newRow.TblBrandBudgetHeader1.LastUpdatedDate = DateTime.Now;
                        newRow.TblBrandBudgetHeader1.CreatedBy = userIserial;
                    }
                    else
                    {
                        var header =
                            db.TblBrandBudgetHeaders.FirstOrDefault(x => x.Iserial == newRow.TblBrandBudgetHeader);
                        if (header != null)
                        {
                            header.LastUpdatedDate = DateTime.Now;
                            header.LastUpdatedBy = userIserial;
                        }
                    }
                }
                else
                {
                    var oldRow = (from e in db.TblBrandBudgetDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        var temp = GenericUpdate(oldRow, newRow, db);
                        if (temp.Any())
                        {
                            var header =
                         db.TblBrandBudgetHeaders.FirstOrDefault(x => x.Iserial == newRow.TblBrandBudgetHeader);
                            if (header != null)
                            {
                                header.LastUpdatedDate = DateTime.Now;
                                header.LastUpdatedBy = userIserial;
                            }
                        }
                    }
                }
                db.SaveChanges();
                return newRow;
            }
        }



        [OperationContract]
        public TblBrandBudgetDetail SaveBrandBudget(TblBrandBudgetDetail details, int userIserial)
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                var ExistingDetail = db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial);
                if (ExistingDetail == null)
                {
                    db.TblBrandBudgetDetails.AddObject(details);
                    db.SaveChanges();
                }
                else
                {
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).TblLkpDirection = details.TblLkpDirection;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).TblStyleCategory = details.TblStyleCategory;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).TblFamily = details.TblFamily;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).TblSalesOrderColorTheme = details.TblSalesOrderColorTheme;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).PaymentDate = details.PaymentDate;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).Amount = details.Amount;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).Qty = details.Qty;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).NoOfStyles = details.NoOfStyles;
                    db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial).NoOfStyles = details.NoOfStyles;
                    db.SaveChanges();
                }
                return db.TblBrandBudgetDetails.FirstOrDefault(x => x.Iserial == details.Iserial);
            }

        }
    }
}

