using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
//using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        [OperationContract]
        private int DeleteTblUsersStyleTNAStatus(TblUsersStyleTNAStatu row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblUsersStyleTNAStatus
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStyleTNAStatu> GetTblUsersStyleTNAStatus(int user)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                
               var query = context.TblUsersStyleTNAStatus.Include("TblStyleTNAStatu")
                    .Where(x => x.TblAuthUser == user).Select(w=>w.TblStyleTNAStatu).ToList();
                if (!query.Any())
                {
                    query = context.TblStyleTNAStatus.ToList();
                }             
                return query;
            }
        }



        [OperationContract]
        private TblUsersStyleTNAStatu UpdateOrInsertTblUsersStyleTNAStatus(TblUsersStyleTNAStatu newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (save)
                    {
                        context.TblUsersStyleTNAStatus.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblUsersStyleTNAStatus
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