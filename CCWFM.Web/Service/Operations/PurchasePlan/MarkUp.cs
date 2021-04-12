using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.PurchasePlan
{
    public partial class PurchasePlan
    {
        [OperationContract]
        private List<TblMarkupGroupProd> GetTblMarkupGroupProd(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblMarkupGroupProd> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = entity.TblMarkupGroupProds.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblMarkupGroupProds
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblMarkupGroupProds.Count();
                    query = entity.TblMarkupGroupProds
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblMarkupGroupProd UpdateOrInsertTblMarkupGroupProds(TblMarkupGroupProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblMarkupGroupProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkupGroupProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkupGroupProd(TblMarkupGroupProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblMarkupGroupProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblMarkup> GetTblMarkupProd(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblMarkup> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = entity.TblMarkups.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblMarkups.Include("TblMarkupGroup1")
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblMarkups.Count();
                    query = entity.TblMarkups.Include("TblMarkupGroup1")
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblMarkupProd UpdateOrInsertTblMarkupProd(TblMarkupProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;

            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblMarkupProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkupProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkupProd(TblMarkupProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblMarkupProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblMarkupTransProd> GetTblTblMarkupTransProd(int type, int tblRecInv, string company, out List<Model.Entity> entityList)
        {

            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblMarkupTransProd> query = entity.TblMarkupTransProds.Where(x => x.TblRecInv == tblRecInv && x.Type == type);

                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var context = new ccnewEntities(Operations.SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblMarkupTransProd UpdateOrInsertTblMarkupTransProds(TblMarkupTransProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;

            using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var entityrow = entity.Entities.Any(w => w.Iserial == newRow.TblMarkupProd && w.scope==0 && w.TblJournalAccountType == 9 && w.AccountIserial!=0);

                if (!entityrow)
                {
                    newRow.Iserial = -1;
                    return newRow;
                }

            }

            using (var entity = new WorkFlowManagerDBEntities())
            {
                newRow.TblMarkupProd1 = null;
                if (save)
                {
                    entity.TblMarkupTransProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkupTransProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkupTransProds(TblMarkupTransProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblMarkupTransProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
        [OperationContract]
        private List<TblDyeingOrderInvoiceMarkupTransProd> DyeingOrderInvoiceMarkupTransProd(int type, int RouteCardInvoiceHeader, string company, out List<Model.Entity> entityList)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingOrderInvoiceMarkupTransProd> query = entity.TblDyeingOrderInvoiceMarkupTransProds.Where(x => x.TblDyeingOrderInvoiceHeader == RouteCardInvoiceHeader && x.Type == type);
                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var context = new ccnewEntities(Operations.SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<RouteCardInvoiceMarkupTransProd> GetTblRouteCardInvoiceMarkupTransProd(int type, int RouteCardInvoiceHeader, string company, out List<Model.Entity> entityList)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<RouteCardInvoiceMarkupTransProd> query = entity.RouteCardInvoiceMarkupTransProds.Where(x => x.RouteCardInvoiceHeader == RouteCardInvoiceHeader && x.Type == type);

                List<int> intList = query.Select(x => x.EntityAccount).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                using (var context = new ccnewEntities(Operations.SharedOperation.GetSqlConnectionString(company)))
                {
                    entityList = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                    var listOfStyles = query.Select(x => x.Iserial);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private RouteCardInvoiceMarkupTransProd UpdateOrInsertRouteCardInvoiceMarkupTransProds(RouteCardInvoiceMarkupTransProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var entityrow = entity.Entities.Any(w => w.Iserial == newRow.TblMarkupProd && w.scope == 0 && w.TblJournalAccountType == 9 && w.AccountIserial != 0);

                if (!entityrow)
                {
                    newRow.Iserial = -1;
                    return newRow;
                }

            }

            using (var entity = new WorkFlowManagerDBEntities())
            {
                newRow.TblMarkupProd1 = null;
                if (save)
                {
                    entity.RouteCardInvoiceMarkupTransProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.RouteCardInvoiceMarkupTransProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteRouteCardInvoiceMarkupTransProds(RouteCardInvoiceMarkupTransProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.RouteCardInvoiceMarkupTransProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    
        [OperationContract]
        private TblDyeingOrderInvoiceMarkupTransProd UpdateOrInsertDyeingOrderInvoiceMarkupTransProds(TblDyeingOrderInvoiceMarkupTransProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var entityrow = entity.Entities.Any(w => w.Iserial == newRow.TblMarkupProd && w.scope == 0 && w.TblJournalAccountType == 9 && w.AccountIserial != 0);

                if (!entityrow)
                {
                    newRow.Iserial = -1;
                    return newRow;
                }

            }
            using (var entity = new WorkFlowManagerDBEntities())
            {
                newRow.TblMarkupProd1 = null;
                if (save)
                {
                    entity.TblDyeingOrderInvoiceMarkupTransProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblDyeingOrderInvoiceMarkupTransProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteDyeingOrderInvoiceMarkupTransProds(TblDyeingOrderInvoiceMarkupTransProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblDyeingOrderInvoiceMarkupTransProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }    
    }
}