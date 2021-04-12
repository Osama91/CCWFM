using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
//using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System.Data.Objects;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        [OperationContract]
        private int DeleteTblUsersApprovalStatus(TblUsersApprovalStatu row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblUsersApprovalStatus
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblApprovalStatu> GetTblUsersApprovalStatus(int user)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                
               var query = context.TblUsersApprovalStatus.Include("TblApprovalStatu")
                    .Where(x => x.TblAuthUser == user).Select(w=>w.TblApprovalStatu).ToList();
                if (!query.Any())
                {
                    query = context.TblApprovalStatus.ToList();
                }             
                return query;
            }
        }



        [OperationContract]
        private TblUsersApprovalStatu UpdateOrInsertTblUsersApprovalStatus(TblUsersApprovalStatu newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (save)
                    {
                        context.TblUsersApprovalStatus.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblUsersApprovalStatus
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            SharedOperation.GenericUpdate(oldRow, newRow, context);
                        }
                    }
                    context.SaveChanges();
                }
                return newRow;
            }
            catch (Exception)
            {
                return newRow;
            }
        }
    }
}