using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblMarkupGroup> GetTblMarkupGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblMarkupGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblMarkupGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblMarkupGroups
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblMarkupGroups.Count();
                    query = entity.TblMarkupGroups
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }





        [OperationContract]
        private TblMarkupGroup UpdateOrInsertTblMarkupGroups(TblMarkupGroup newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblMarkupGroups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkupGroups
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkupGroup(TblMarkupGroup row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblMarkupGroups
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblMarkup> GetTblMarkup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblMarkup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
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
        private TblMarkup UpdateOrInsertTblMarkups(TblMarkup newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblMarkups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkups
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkup(TblMarkup row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblMarkups
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();

                if (query != null)
                {

                    var Ledger = entity.TblLedgerMainDetails.Where(w => w.EntityAccount == query.Iserial);
                    if (Ledger.Any())
                    {
                        throw new System.Exception("Cannot delete Because There is a transaction Exisit");
                    }

                    entity.DeleteObject(query);
                }
            

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblMarkupTran> GetTblMarkupTrans(int type, int tblRecInv, string company,out decimal? TotalMisc, out List<Model.Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var  query = entity.TblMarkupTrans.Include("TblCurrency1").Include("TblMarkup1").Where(x => x.TblRecInv == tblRecInv && x.Type == type).ToList();

                List<int> intList = query.Select(x => x.EntityAccount??0).ToList();


                List<int> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                try
                {
                    TotalMisc = entity.TblRecInvMainDetails.Where(w => w.TblRecInvHeader == tblRecInv && w.Misc != 0).Sum(w => w.Qty * w.Misc);
                }
                catch (System.Exception)
                {

                    TotalMisc = 0;
                }
             
                return query;
            }
        }

        [OperationContract]
        private TblMarkupTran UpdateOrInsertTblMarkupTrans(TblMarkupTran newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                newRow.TblMarkup1 = null;
                if (save)
                {
                    entity.TblMarkupTrans.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblMarkupTrans
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblMarkupTrans(TblMarkupTran row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblMarkupTrans
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        public TblMarkup getMarkUpByIserial(int Iserial, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
               return entity.TblMarkups.Include("TblMarkupGroup1").FirstOrDefault(w => w.Iserial == Iserial);                
            }
        }
    }
}