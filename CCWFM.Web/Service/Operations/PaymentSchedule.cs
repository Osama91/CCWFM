using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblPaymentSchedule> GetTblPaymentSchedule(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPaymentSchedule> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblPaymentSchedules.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPaymentSchedules.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPaymentSchedules.Count();
                    query = context.TblPaymentSchedules.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPaymentSchedule UpdateOrInsertTblPaymentSchedule(TblPaymentSchedule newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    foreach (var row in newRow.TblPaymentScheduleDetails.ToList())
                    {
                        row.TblPaymentScheduleSetting = null;
                    }
                    context.TblPaymentSchedules.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblPaymentSchedules
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }

                    foreach (var row in newRow.TblPaymentScheduleDetails.ToList())
                    {
                        var oldDetailRow = (from e in context.TblPaymentScheduleDetails
                                            where e.Iserial == row.Iserial
                                            select e).SingleOrDefault();
                        if (oldDetailRow != null)
                        {
                            GenericUpdate(oldDetailRow, row, context);
                        }
                        else
                        {
                            row.TblPaymentSchedule1 = null;
                            row.TblPaymentScheduleSetting = null;
                            context.TblPaymentScheduleDetails.AddObject(row);
                        }
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPaymentSchedule(TblPaymentSchedule row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblPaymentSchedules
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPaymentScheduleDetail> GetTblPaymentScheduleDetail(int skip, int take, int paymentSchedule, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPaymentScheduleDetail> query;
                if (filter != null)
                {
                    //it.TblLkpSeason==(@TblLkpSeason0)
                    filter = filter + " and it.TblPaymentSchedule ==(@Group0)";
                    valuesObjects.Add("Group0", paymentSchedule);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    //  var tempQuery = context.TblPaymentScheduleDetail.Where(x => x.TblFamily == groupId);
                    fullCount = context.TblPaymentScheduleDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblPaymentScheduleDetails.Include("TblPaymentScheduleSetting").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblPaymentScheduleDetails.Count(v => v.TblPaymentSchedule == paymentSchedule);
                    query = context.TblPaymentScheduleDetails.Include("TblPaymentScheduleSetting").OrderBy(sort).Where(v => v.TblPaymentSchedule == paymentSchedule).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        //[OperationContract]
        //private List<VENDPAYMMODETABLE> GetVendPayMode(string dataArea)
        //{
        //    using (var context = new ax2009_ccEntities())
        //    {
        //        return context.VENDPAYMMODETABLEs.Where(x => x.DATAAREAID == dataArea).ToList();
        //    }
        //}

        //[OperationContract]
        //private List<PAYMTERM> GetAxPaymentTerm(string dataArea)
        //{
        //    using (var context = new ax2009_ccEntities())
        //    {
        //        return context.PAYMTERMs.Where(x => x.DATAAREAID == dataArea).ToList();
        //    }
        //}
    }
}