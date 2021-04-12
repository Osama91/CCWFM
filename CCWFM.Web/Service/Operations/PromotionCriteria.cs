using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Globalization;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblPromoCriteria> SearchTblPromoCriteria(int? Glserial, DateTime? Date, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var Tbl = db.TblPromoCriterias.Include("TblBrandPromoDetails").Include("TblStorePromoDetails").Where(m =>
                   (m.Glserial == Glserial || Glserial == null || Glserial == 0)
                    && (EntityFunctions.TruncateTime(m.DateFrom) <= Date && EntityFunctions.TruncateTime(m.DateTo) >= Date || Date == null)
                    ).ToList();

                return Tbl;
            }
        }

        [OperationContract]
        public List<TblStore> SearchStores(List<int> Storeiserial, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var u =
                    db.TblStores.Where(z =>
                             Storeiserial.Any(l => z.iserial == l)).ToList();

                return u;
            }
        }

        [OperationContract]
        public List<TblStorePromoDetail> GetStoresDetails(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblStorePromoDetail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblStorePromoDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblStorePromoDetails.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblStorePromoDetails.Count();
                    query = context.TblStorePromoDetails.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblBrandPromoDetail> GeTblBrandPromoDetails(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBrandPromoDetail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblBrandPromoDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblBrandPromoDetails.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblBrandPromoDetails.Count();
                    query = context.TblBrandPromoDetails.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }




        [OperationContract]
        public void GetCustomerCriteria(int Glserial, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                db.CommandTimeout = 0;
                var Criteria = db.TblPromoCriterias.FirstOrDefault(w => w.TblPromoHeader == Glserial);
                var fromCodeInt = 1;
                if (db.TblPromoDetails.Any())
                {
                    var numbers = db.TblPromoDetails.Select(x => x.Code).ToList();

                    int temp;

                    fromCodeInt = numbers.Select(n => int.TryParse(n, out temp) ? temp : 0).Max();
                }
                var Counter = 0;
                if (Criteria.NotActive)
                {
                    var brands = db.TblBrandPromoDetails.Where(w => w.TblPromoCriteria == Criteria.Glserial).Select(w => w.Brands).ToList();
                    var stores = db.TblStorePromoDetails.Where(w => w.TblPromoCriteria == Criteria.Glserial).Select(w => w.TblStores);
                    string combindedbrands = string.Join("|", brands.ToArray());
                    string combindedstores = string.Join("|", stores.ToArray());
                    var selectTop = Criteria.SelectTop ?? 0;
                    if (selectTop == 0)
                    {
                        selectTop = int.MaxValue;
                    }
                    var query = db.NotActiveCustomerCriteria(selectTop, combindedstores, combindedbrands, Criteria.DateFrom, Criteria.DateTo).ToList();

                    var random = new Random();
                    foreach (var item in query)
                    {
                        Counter++;
                        var codeNo = fromCodeInt + Counter;
                        var Millisecond = DateTime.Now.Millisecond;
                        var randomNumber = random.Next(10000, 90000) + Millisecond;

                        var newrow = (new TblPromoDetail
                        {
                            Code = codeNo.ToString(CultureInfo.InvariantCulture),
                            PIN = randomNumber.ToString(CultureInfo.InvariantCulture),
                            MobileNo = item,
                            Glserial = Criteria.TblPromoHeader ?? 0,
                            TblPromoHeader = Criteria.TblPromoHeader,
                        }
                        );
                        db.TblPromoDetails.AddObject(newrow);
                    }

                }
                else
                {
                    var query = db.CustomerCriteria(Glserial).ToList();
                    //var TransactionHeader = db.TblPromoCriterias.Include(w => w.TblPromoHeader1).FirstOrDefault(w => w.Glserial == Glserial).TblPromoHeader1;
                    var difference = query.Count();
                    var random = new Random();
                    foreach (var item in query)
                    {
                        Counter++;
                        var codeNo = fromCodeInt + Counter;
                        var Millisecond = DateTime.Now.Millisecond;
                        var randomNumber = random.Next(10000, 90000) + Millisecond;
                        var newrow = (new TblPromoDetail
                        {
                            Code = codeNo.ToString(CultureInfo.InvariantCulture),
                            PIN = randomNumber.ToString(CultureInfo.InvariantCulture),
                            MobileNo = item.customerMobile,
                            Glserial = Criteria.TblPromoHeader ?? 0,
                            TblPromoHeader = Criteria.TblPromoHeader,
                        }
                        );
                        db.TblPromoDetails.AddObject(newrow);
                    }
                }
                db.SaveChanges();

            }
        }

        [OperationContract]
        private TblPromoCriteria UpdateOrInsertTblPromoCriteria(TblPromoCriteria newRow, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblPromoCriterias
                              where e.Glserial == newRow.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                    var oldStores = (from e in context.TblStorePromoDetails
                                     where e.Glserial == newRow.Glserial
                                     select e);

                    foreach (var oldstore in oldStores)
                    {
                        context.DeleteObject(oldstore);
                    }

                    foreach (var storeRow in newRow.TblStorePromoDetails.ToList())
                    {
                        storeRow.TblPromoCriteria1 = null;
                        storeRow.TblPromoCriteria = newRow.Glserial;
                        context.TblStorePromoDetails.AddObject(storeRow);
                    }

                    var oldbrands = (from e in context.TblBrandPromoDetails
                                     where e.Glserial == newRow.Glserial
                                     select e);

                    foreach (var oldbrand in oldbrands)
                    {
                        context.DeleteObject(oldbrand);
                    }

                    foreach (var Brandrow in newRow.TblBrandPromoDetails.ToList())
                    {
                        Brandrow.TblPromoCriteria1 = null;
                        Brandrow.TblPromoCriteria = newRow.Glserial;
                        context.TblBrandPromoDetails.AddObject(Brandrow);
                    }
                }
                //else
                //{
                //    context.TblPromoCriterias.AddObject(newRow);
                //}

                context.SaveChanges();

                return newRow;
            }
        }

        private TblPromoCriteria UpdateOrInsertTblPromoCriteria(TblPromoCriteria newRow, ccnewEntities context)
        {

            var oldRow = (from e in context.TblPromoCriterias
                          where e.Glserial == newRow.Glserial
                          select e).SingleOrDefault();
            if (oldRow != null)
            {
                GenericUpdate(oldRow, newRow, context);
                var oldStores = (from e in context.TblStorePromoDetails
                                 where e.Glserial == newRow.Glserial
                                 select e);

                foreach (var oldstore in oldStores)
                {
                    context.DeleteObject(oldstore);
                }

                foreach (var storeRow in newRow.TblStorePromoDetails.ToList())
                {
                    storeRow.TblPromoCriteria1 = null;
                    storeRow.TblPromoCriteria = newRow.Glserial;
                    context.TblStorePromoDetails.AddObject(storeRow);
                }

                var oldbrands = (from e in context.TblBrandPromoDetails
                                 where e.Glserial == newRow.Glserial
                                 select e);

                foreach (var oldbrand in oldbrands)
                {
                    context.DeleteObject(oldbrand);
                }

                foreach (var Brandrow in newRow.TblBrandPromoDetails.ToList())
                {
                    Brandrow.TblPromoCriteria1 = null;
                    Brandrow.TblPromoCriteria = newRow.Glserial;
                    context.TblBrandPromoDetails.AddObject(Brandrow);
                }
            }
            return newRow;

        }
        [OperationContract]
        private int DeleteTblPromoCriteria(TblPromoCriteria row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblPromoCriterias
                              where e.Glserial == row.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
                return row.Glserial;
            }
        }
    }
}