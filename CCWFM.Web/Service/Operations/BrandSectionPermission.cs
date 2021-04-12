using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

// ReSharper disable once CheckNamespace
namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblBrandSectionPermission> GetTblBrandSectionPermission(string brand, int tblLkpBrandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblBrandSectionPermissions.Where(x => x.BrandCode == brand && x.TblLkpBrandSection == tblLkpBrandSection);
                return query.ToList();
            }
        }

        [OperationContract]
        private TblBrandSectionPermission UpdateOrInsertTblBrandSectionPermission(TblBrandSectionPermission newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblBrandSectionPermissions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblBrandSectionPermissions
                                  where e.Iserial == newRow.Iserial

                                  select e).SingleOrDefault();
                    if (oldRow != null) GenericUpdate(oldRow, newRow, context);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblBrandSectionPermission(TblBrandSectionPermission row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblBrandSectionPermissions
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}