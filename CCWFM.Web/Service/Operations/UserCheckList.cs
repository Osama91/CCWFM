using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblUserCheckList> GetTblUserCheckList(int tblauthUser)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblUserCheckLists.Include("TblCheckListGroup1").Where(x => x.TblAuthUser == tblauthUser);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblUserCheckList UpdateOrInsertTblUserCheckList(TblUserCheckList newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblUserCheckLists.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblUserCheckLists
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
        private int DeleteTblUserCheckList(TblUserCheckList row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblUserCheckLists
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}