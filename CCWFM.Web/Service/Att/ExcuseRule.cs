using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Att
{
    public partial class AttService
    {
        [OperationContract]
        private List<TblExcuseRule> GetTblExcuseRule(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<TblExcuseRule> query;
                if (filter != null)
                {
                    var parameterCollection = Operations.SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblExcuseRules.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblExcuseRules.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblExcuseRules.Count();
                    query = context.TblExcuseRules.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<CSPPERIOD> AxPeriods()
        {
            using (var db = new configurationEntities())
            {
                var q = db.CSPPERIODS.ToList();

                return q;
            }
        }

        [OperationContract]
        public List<CSPPERIODLINE> AxPeriodLines(string periodId, DateTime date)
        {
            using (var db = new configurationEntities())
            {
                var q = db.CSPPERIODLINES.Where(x => x.CSPPERIODID == periodId && x.FROMDATE <= date && x.TODATE >= date).ToList();

                return q;
            }
        }

        [OperationContract]
        private TblExcuseRule UpdateOrInsertTblExcuseRule(TblExcuseRule newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.TblExcuseRules.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblExcuseRules
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
        private int DeleteTblExcuseRule(TblExcuseRule row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblExcuseRules
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}