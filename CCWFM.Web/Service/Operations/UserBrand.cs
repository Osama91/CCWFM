using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblUserBrandSection> GetTblUserBrandSection(string brand, int TblAuthUser)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblUserBrandSections.Include("TblLkpBrandSection1").Where(x => x.BrandCode == brand && x.TblAuthUser == TblAuthUser);
                return query.ToList();
            }
        }

        [OperationContract]
        private TblUserBrandSection UpdateOrDeleteTblUserBrandSection(TblUserBrandSection newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblUserBrandSections.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblUserBrandSections
                                  where e.TblLkpBrandSection == newRow.TblLkpBrandSection
                                  && e.BrandCode == newRow.BrandCode && e.TblAuthUser == newRow.TblAuthUser

                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private List<TblUserBrandSectionPermission> GetTblUserBrandSectionPermission(int UserBrandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblUserBrandSectionPermissions.Include("TblAuthPermission1")
                    .Where(x => x.TblUserBrandSection == UserBrandSection);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblUserBrandSectionPermission UpdateOrDeleteTblUserBrandSectionPermission(TblUserBrandSectionPermission newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblUserBrandSectionPermissions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblUserBrandSectionPermissions
                                  where e.TblAuthPermission == newRow.TblAuthPermission && e.TblUserBrandSection == newRow.TblUserBrandSection

                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

    }
}