using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;    
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        [OperationContract]
        private List<TblStyleTNAStatu> GetTblStyleTnaStatus()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblStyleTNAStatus.ToList();
            }
        }
        [OperationContract]
        private TblStyleTNAStatusDetail UpdateOrInsertTblStyleTNAStatusDetail(TblStyleTNAStatusDetail newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                //   context.Connection.ConnectionTimeout = 0;
                var oldRow = (from e in context.TblStyleTNAStatusDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblStyleTNAStatusDetails.AddObject(newRow);
                }
                var styleTNA = context.TblStyleTNAHeaders.Include(nameof(TblStyleTNAHeader.TblStyle1)).Include(nameof(TblStyleTNAHeader.TblStyleTNAColorDetails)).FirstOrDefault(x => x.Iserial == newRow.TblStyleTNAHeader);
                styleTNA.TblStyleTNAStatus = newRow.TblStyleTnaStatus;
                if (styleTNA.TblStyleTNAStatus == 1)
                {
                    GenerateSalesOrderFromTna(styleTNA);
                }
                context.SaveChanges();

                return newRow;
            }
        }

        public void GenerateSalesOrderFromTna(TblStyleTNAHeader StyleTna)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                CRUD_ManagerService srv = new CRUD_ManagerService();
                srv.generateSalesOrderColorFromMasterList(context, StyleTna);
                context.SaveChanges();

            }
        }
        [OperationContract]
        private List<TblStyleTNAStatusDetail> GetTblStyleTNAStatusDetail(int StyleTna)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblStyleTNAStatusDetails.Include("TblStyleTNAStatu").Include("TblAuthUser1").Where(x => x.TblStyleTNAHeader == StyleTna).ToList();
            }
        }
    }
}