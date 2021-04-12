using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<tblcurrencydailyexchange> GetTblCurrencyDailyExchange(int skip, int take, int year, int TblCurrency, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount,string comany)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(comany)))
            {
                IQueryable<tblcurrencydailyexchange> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblCurrency ==(@TblCurrency0)";
                    valuesObjects.Add("TblCurrency0", TblCurrency);

                    filter = filter + " and it ==(@year0)";
                    valuesObjects.Add("year0", year);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.tblcurrencydailyexchanges.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tblcurrencydailyexchanges.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tblcurrencydailyexchanges.Count(x => x.DocDate.Value.Year == year && x.TblCurrency == TblCurrency);
                    query = context.tblcurrencydailyexchanges.OrderBy(sort).Where((x => x.DocDate.Value.Year == year && x.TblCurrency == TblCurrency)).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblCurrencyTest> GetTblCurrencyTest(string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCurrencyTest> query;

                query = context.TblCurrencyTests;

                return query.ToList();
            }
        }

        [OperationContract]
        private tblcurrencydailyexchange UpdateOrInsertTblCurrencyDailyExchange(tblcurrencydailyexchange newRow, bool save, int index, out int outindex,string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    context.tblcurrencydailyexchanges.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.tblcurrencydailyexchanges
                                  where e.votglserial == newRow.votglserial
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
        private int DeleteTblCurrencyDailyExchange(tblcurrencydailyexchange row,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.tblcurrencydailyexchanges
                              where e.votglserial == row.votglserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.votglserial;
        }

        [OperationContract]
        private List<tblcurrencydailyexchange> GenerateCurrencyDailyExchange(DateTime fromDate, DateTime toDate, float rate, int currency,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var listOfRows = new List<tblcurrencydailyexchange>();
                foreach (DateTime day in EachDay(fromDate, toDate))
                {
                    var oldRow = (from e in context.tblcurrencydailyexchanges
                                  where e.DocDate == day && e.TblCurrency == currency
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        oldRow.ExchRate = rate;
                        listOfRows.Add(oldRow);
                    }
                    else
                    {
                        var newrow = new tblcurrencydailyexchange
                        {
                            DocDate = day,
                            ExchRate = rate,
                            TblCurrency = currency
                        };
                        listOfRows.Add(newrow);
                        context.tblcurrencydailyexchanges.AddObject(newrow);
                    }
                }

                context.SaveChanges();
                return listOfRows;
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}