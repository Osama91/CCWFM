using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

//using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public TblRFSHeader GetRFS(string rfsNum, out List<TblColor> colorsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var tmp = context.TblRFSHeaders
                    .Include("TblRFSDetails.TblRFSDetailServices")
                    .Include("TblRFSDetails.TblRFSDetailItems")
                    .FirstOrDefault(x => x.DocNumber == rfsNum);
                if (tmp != null)
                {
                    colorsList = null;
                    // colorsList = GetColorsByBrandSeason(tmp.BrandCode, tmp.SeasonCode);
                    return tmp;
                }
            }
            colorsList = new List<TblColor>();
            return null;
        }

        [OperationContract]
        public TblRFSHeader UpdateOrInsertRFS(TblRFSHeader newRow)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRFSHeaders
                              where e.TransID == newRow.TransID
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    try
                    {
                        var max = context.TblRFSHeaders.Select(x => x.DocNumber).Cast<int>().Max(x => x);
                        newRow.DocNumber = (max + 1).ToString();
                    }
                    catch
                    {
                        newRow.DocNumber = "0";
                    }

                    context.TblRFSHeaders.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        public List<TblRFSDetail> UpdateOrInsertRFSDetail(List<TblRFSDetail> newRows, List<TblRFSDetail> deletedRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in newRows)
                {
                    var oldRow = (from e in context.TblRFSDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.TblRFSDetails.AddObject(newRow);
                    }
                }
                foreach (var TblRFSDetail in deletedRows)
                {
                    context.DeleteObject(TblRFSDetail);
                }
                context.SaveChanges();
            }
            return newRows;
        }

        [OperationContract]
        public List<TblRFSDetailItem> UpdateOrInsertRFSDetailItems(List<TblRFSDetailItem> newRows, List<TblRFSDetailItem> deletedRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in newRows)
                {
                    var oldRow = (from e in context.TblRFSDetailItems
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.TblRFSDetailItems.AddObject(newRow);
                    }
                }
                foreach (var TblRFSDetail in deletedRows)
                {
                    context.DeleteObject(TblRFSDetail);
                }
                context.SaveChanges();
            }
            return newRows;
        }

        [OperationContract]
        public List<TblRFSDetailService> UpdateOrInsertRFSDetailServices(List<TblRFSDetailService> newRows, List<TblRFSDetailService> deletedRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in newRows)
                {
                    var oldRow = (from e in context.TblRFSDetailServices
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        context.TblRFSDetailServices.AddObject(newRow);
                    }
                }
                foreach (var TblRFSDetail in deletedRows)
                {
                    context.DeleteObject(TblRFSDetail);
                }
                context.SaveChanges();
            }
            return newRows;
        }
    }
}