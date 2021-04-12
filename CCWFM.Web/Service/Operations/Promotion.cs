using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Data.Entity;
namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        //public List<TblStore> SearchByStoreNameAndCode(string code, string stroename)

        //{
        //    using (var db = new ccnewEntities(GetSqlConnectionString(database)))
        //    {
        //    }

        //}

        //[OperationContract]

        ////[OperationContract]

        ////public List<TblStore> serachbycodestore(string code)

        ////{
        ////    using (var db = new ccnewEntities(GetSqlConnectionString(database)))
        ////    {
        ////        var tbl = db.TblStores.Where(x=>x.code==code).ToList();

        ////        return tbl;
        ////    }

        //}
        //(T => T.Date > Fromdate && T.Date < ToDate)
        [OperationContract]
        public TblPromoHeader GetPrmotionRange(int fromcode, int tocode, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var Tbl = db.TblPromoHeaders.FirstOrDefault(m => m.FromCode <= fromcode && (m.ToCode >= fromcode));
                return Tbl;
            }
        }

        [OperationContract]
        public List<TblStore> SearchforsStoreName(string storename, string code, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                db.Connection.Open();
                List<TblStore> u =
                    db.TblStores.Where(z => (z.ENAME.StartsWith(storename) || storename == null || storename == "") && z.Type == 4 && (z.code.StartsWith(code) || code == null || code == "")
                            ).ToList();

                return u;
            }
        }

        [OperationContract]
        public List<TblPromoHeader> SearchTblPromoHeaderAll(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPromoHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblPromoHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblPromoHeaders.Include(w=>w.TBLEVENTUALHEADER1).Include(w => w.TblPromoCriterias).Include("TblPromoCriterias.TblBrandPromoDetails").Include("TblPromoCriterias.TblStorePromoDetails")
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblPromoHeaders.Count();
                    query = entity.TblPromoHeaders.Include(w => w.TBLEVENTUALHEADER1).Include(w => w.TblPromoCriterias).Include("TblPromoCriterias.TblBrandPromoDetails").Include("TblPromoCriterias.TblStorePromoDetails")
                        .OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public TblPromoDetail UpdateAndInsertTblPromoDetail(TblPromoDetail newRow, bool save, int index, out int outindex, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                outindex = index;
                if (save)
                {
                    db.TblPromoDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in db.TblPromoDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, db);
                    }
                }
                db.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPromoDetail(TblPromoDetail row, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in db.TblPromoDetails
                              where e.Glserial == row.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null) db.DeleteObject(oldRow);

                db.SaveChanges();
            }
            return row.Glserial;
        }

        [OperationContract]
        public List<TblPromoDetail> GetDetail(int skip, int take, int glSerial, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPromoDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.Glserial ==(@Group0)";
                    valuesObjects.Add("Group0", glSerial);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblPromoDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPromoDetails.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPromoDetails.Count(x => x.Glserial == glSerial);
                    query = context.TblPromoDetails.Where(x => x.Glserial == glSerial).OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }
  
        [OperationContract]
        public List<TBLEVENTUALHEADER> GetevData(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TBLEVENTUALHEADER> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TBLEVENTUALHEADERs.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TBLEVENTUALHEADERs.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TBLEVENTUALHEADERs.Count();
                    query = context.TBLEVENTUALHEADERs.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        public TblPromoHeader SearchTblPromoHeader(int glserial, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var Tbl = db.TblPromoHeaders.SingleOrDefault(m => m.GlSerial == glserial);

                return Tbl;
            }
        }

        [OperationContract]
        private TblPromoHeader UpdateOrInsertTblPromoHeader(TblPromoHeader newRow, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblPromoHeaders
                              where e.GlSerial == newRow.GlSerial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);

                    foreach (var item in newRow.TblPromoCriterias)
                    {
                        UpdateOrInsertTblPromoCriteria(item,context);
                    }                 
                }
                else
                {
                    newRow.CreateDate = DateTime.Now;
                    context.TblPromoHeaders.AddObject(newRow);
                }

                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPromoHeader(TblPromoHeader row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblPromoHeaders
                              where e.GlSerial == row.GlSerial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
                return row.GlSerial;
            }
        }

        // skip : bt3dy awl 20 row btsbhom
        // take 3dd al rows al ana 3aeyzha
        //fullcount 3shan ageeb 3dd al rows
    }
}