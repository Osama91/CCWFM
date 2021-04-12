using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblCheckListLink> GetTblCheckListLink(int tblChecklistgroup)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblCheckListLinks.Include("TblCheckListDesignGroupHeader11").Include("TblCheckListDesignGroupHeader21").Include("TblCheckListGroup1").Include("TblCheckListItem1").Where(s => s.TblCheckListGroup == tblChecklistgroup);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblCheckListLink UpdateOrDeleteTblCheckListLink(TblCheckListLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblCheckListLinks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblCheckListLinks
                                  where e.TblCheckListGroup == newRow.TblCheckListGroup
                                  && e.TblCheckListItem == newRow.TblCheckListItem

                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }
    }
}