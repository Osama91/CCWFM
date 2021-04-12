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
        private List<TblCheckListTransaction> GetTblCheckListTransaction(int user)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var now = DateTime.Now.Date;

                var query = context.TblCheckListTransactions.Where(x => x.TblAuthUser == user && x.TransDate == now);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblCheckListTransaction> GetTblCheckListTransactionForManager(string code, DateTime date)
        {
            var test = GetEmpByAttOperator(null, null, code);
            using (var context = new WorkFlowManagerDBEntities())
            {
                var listofIserial = new List<int>();
                foreach (var employeesView in test)
                {
                    if (code != employeesView.Emplid)
                    {
                        var user = context.TblAuthUsers.FirstOrDefault(x => x.Code == employeesView.Emplid);
                        if (user != null) listofIserial.Add(user.Iserial);
                    }
                }
                var query = context.TblCheckListTransactions.Include("TblCheckListItem1.TblCheckListLinks").Where(x => listofIserial.Contains(x.TblAuthUser) && x.TransDate == date);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblCheckListTransaction UpdateOrDeleteTblCheckListTransaction(TblCheckListTransaction newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            var date = newRow.TransDate.Date;

            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblCheckListTransactions
                              where e.TblAuthUser == newRow.TblAuthUser
                              && e.TblCheckListItem == newRow.TblCheckListItem
                              && e.TransDate == date
                              select e).SingleOrDefault();
                if (oldRow == null)
                {
                    newRow.TblCheckListItem1 = null;
                    newRow.TblAuthUser1 = null;
                    context.TblCheckListTransactions.AddObject(newRow);
                }
                else
                {
                    newRow.Iserial = oldRow.Iserial;
                    GenericUpdate(oldRow, newRow, context);
                }

                context.SaveChanges();
                return newRow;
            }
        }
    }
}