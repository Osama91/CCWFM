using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblRequestForSampleService> GetTblRequestForSampleService(int tblRequestForSample)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblRequestForSampleServices.Include("TblService1").Where(x => x.TblRequestForSample == tblRequestForSample).ToList();
            }
        }

        [OperationContract]
        private TblRequestForSampleService UpdateOrInsertTblRequestForSampleService(TblRequestForSampleService newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblRequestForSampleServices.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblRequestForSampleServices
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
        private int DeleteTblRequestForSampleService(TblRequestForSampleService row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRequestForSampleServices
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}