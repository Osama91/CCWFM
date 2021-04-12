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
        public List<TblStyleAttachment> GetTblStyleAttachment(int style)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    return
                        context.TblStyleAttachments.Include("TblStyle1").Where(x => x.TblStyle == style).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        public int MaxIserialOfStyleForAttachment(out string attachmentPath)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNo =
          context.TblStyleAttachments
              .Select(x => x.Iserial).Cast<int?>().Max();

                attachmentPath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "AttachmentsFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }
                return 1;
            }
        }

        [OperationContract]
        private List<TblStyleAttachment> UpdateOrInsertTblStyleAttachment(List<TblStyleAttachment> attachmentList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in attachmentList)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        context.TblStyleAttachments.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblStyleAttachments
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
        private int DeleteTblStyleAttachment(TblStyleAttachment row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStyleAttachments
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}