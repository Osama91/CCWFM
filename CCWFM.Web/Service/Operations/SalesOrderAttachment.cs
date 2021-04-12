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
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblSalesOrderAttachment> GetTblSalesOrderAttachment(int SalesOrder)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    return
                        context.TblSalesOrderAttachments.Include("TblSalesOrder1").Where(x => x.TblSalesOrder == SalesOrder).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        public int MaxIserialOfSalesOrderForAttachment(out string attachmentPath)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNo =
          context.TblSalesOrderAttachments
              .Select(x => x.Iserial).Cast<int?>().Max();

                attachmentPath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "AttachmentsFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }
                return 1;
            }
        }

        [OperationContract]
        private List<TblSalesOrderAttachment> UpdateOrInsertTblSalesOrderAttachment(List<TblSalesOrderAttachment> attachmentList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in attachmentList)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        context.TblSalesOrderAttachments.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblSalesOrderAttachments
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            var changedItems = GenericUpdate(oldRow, newRow, context);
                            if (changedItems.Count() > 1)
                            {
                                newRow.LastUpdatedDate = DateTime.Now;
                                GenericUpdate(oldRow, newRow, context);
                            }
                        }
                    }
                }

                context.SaveChanges();
                return attachmentList;
            }
        }

        [OperationContract]
        private int DeleteTblSalesOrderAttachment(TblSalesOrderAttachment row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderAttachments
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}