using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblRequestForSampleEvent> GetTblRequestForSampleEvent(int requestForSample)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblRequestForSampleEvents.Include("TblAuthUser")
                    .Where(x => x.TblSalesOrder == requestForSample);
                return query.ToList();
            }
        }

        [OperationContract]
        private TblRequestForSampleEvent UpdateOrInsertTblRequestForSampleEvent(TblRequestForSampleEvent newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                //var requestForSample = context.TblRequestForSamples.FirstOrDefault(x => x.Iserial == newRow.TblSalesOrder);
                //if (requestForSample != null)
                //    requestForSample.TblRequestForSampleStatus = newRow.TblRequestForSampleStatus;
                if (save)
                {
                    context.TblRequestForSampleEvents.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblRequestForSampleEvents
                                  where e.Iserial == newRow.Iserial
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
        private int DeleteTblRequestForSampleEvent(TblRequestForSampleEvent row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRequestForSampleEvents
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}