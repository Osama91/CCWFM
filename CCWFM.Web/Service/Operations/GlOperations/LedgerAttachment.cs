using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblLedgerHeaderAttachment> GetTblLedgerHeaderAttachment(int LedgerHeader)
        {
            using (var context = new ccnewEntities())
            {
                try
                {
                    return
                        context.TblLedgerHeaderAttachments.Include("TblLedgerHeader1").Where(x => x.TblLedgerHeader == LedgerHeader).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

     

        [OperationContract]
        private List<TblLedgerHeaderAttachment> UpdateOrInsertTblLedgerHeaderAttachment(List<TblLedgerHeaderAttachment> attachmentList)
        {
            using (var context = new ccnewEntities())
            {
                foreach (var newRow in attachmentList)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        context.TblLedgerHeaderAttachments.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblLedgerHeaderAttachments
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
        private int DeleteTblLedgerHeaderAttachment(TblLedgerHeaderAttachment row)
        {
            using (var context = new ccnewEntities())
            {
                var oldRow = (from e in context.TblLedgerHeaderAttachments
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}