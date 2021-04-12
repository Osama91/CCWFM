using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        [OperationContract]
        private List<TblLkpBrandSectionLink> GetTblBrandSectionLink(string brand, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblLkpBrandSectionLinks.Include("TblLkpBrandSection1")
                    .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                    .Where(x => x.TblLkpBrandSection1.TblUserBrandSections.Any(e => e.BrandCode == brand)
                           && x.TblLkpBrandSection1.TblBrandSectionPermissions.Any(e => e.BrandCode == brand)
                           && x.TblBrand == brand
                           && x.TblLkpBrandSection1.TblUserBrandSections.Any(e => (e.TblAuthUser == userIserial||userIserial==0) && e.BrandCode == brand));
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblLkpBrandSectionLink> GetTblAllBrandSectionLink(string brand)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblLkpBrandSectionLinks.Include("TblLkpBrandSection1")
                    .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                    .Where(x =>x.TblBrand == brand);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblLkpBrandSectionLink UpdateOrDeleteTblLkpBrandSectionLink(TblLkpBrandSectionLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblLkpBrandSectionLinks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblLkpBrandSectionLinks
                                  where e.TblLkpBrandSection == newRow.TblLkpBrandSection
                                  && e.TblBrand == newRow.TblBrand

                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        public List<TblFamilyLink> GetTblFamilyLink(string brand, int brandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblFamilyLinks.Include("TblFamily1").Where(x => x.TblLkpBrandSection == brandSection && x.TblBrand == brand).OrderBy(w=>w.TblFamily1.Ename);

                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblFamilyCategoryLink> GetTblFamilyCategoryLink(string brand, int brandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblFamilyCategoryLinks.Include("TblFamily1").Where(x => x.TblLkpBrandSection == brandSection && x.TblBrand == brand).OrderBy(w => w.TblFamily1.Ename);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblFamilyLink UpdateOrDeleteTblFamilyLink(TblFamilyLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblFamilyLinks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblFamilyLinks
                                  where e.TblFamily == newRow.TblFamily
                                  && e.TblBrand == newRow.TblBrand
                                  && e.TblLkpBrandSection == newRow.TblLkpBrandSection
                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        internal List<TblSubFamilyLink> GetTblSubFamilyLink(int tblFamily, string brand, int tblBrandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSubFamilyLinks.Include("TblSubFamily1").Where(x => x.TblFamily == tblFamily
                 && x.TblBrand == brand && x.TblLkpBrandSection == tblBrandSection).OrderBy(w => w.TblSubFamily1.Ename);

                return query.ToList();
            }
        }

        [OperationContract]
        internal List<TblSubFamilyCategoryLink> GetTblSubFamilyCategoryLink(int tblFamily, string brand, int tblBrandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSubFamilyCategoryLinks.Include("TblSubFamily1").Where(x => x.TblFamily == tblFamily
                 && x.TblBrand == brand && x.TblLkpBrandSection == tblBrandSection).OrderBy(w => w.TblSubFamily1.Ename);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSubFamilyLink UpdateOrDeleteTblSubFamilyLink(TblSubFamilyLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var row = new TblSubFamilyLink
                    {
                        TblBrand = newRow.TblBrand,
                        TblFamily = newRow.TblFamily,
                        TblLkpBrandSection = newRow.TblLkpBrandSection,
                        TblSubFamily = newRow.TblSubFamily
                    };

                    context.TblSubFamilyLinks.AddObject(row);
                }
                else
                {
                    var oldRow = (from x in context.TblSubFamilyLinks
                                  where x.TblFamily == newRow.TblFamily
                    && x.TblSubFamily == newRow.TblSubFamily && x.TblBrand == newRow.TblBrand && x.TblLkpBrandSection == newRow.TblLkpBrandSection
                                  select x).SingleOrDefault();

                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblLkpDirectionLink UpdateOrDeleteTblDirectionLink(TblLkpDirectionLink newRow, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblLkpDirectionLinks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblLkpDirectionLinks
                                  where e.TblLkpBrandSection == newRow.TblLkpBrandSection
                                  && e.TblBrand == newRow.TblBrand
                                  && e.TblLkpDirection == newRow.TblLkpDirection

                                  select e).SingleOrDefault();
                    if (oldRow != null) context.DeleteObject(oldRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private List<TblLkpDirectionLink> GetTblDirectionLink(string brand, int brandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblLkpDirectionLinks.Include("TblLkpDirection1").Where(x => x.TblLkpBrandSection == brandSection && x.TblBrand == brand);

                return query.ToList();
            }
        }

        [OperationContract]
        private bool GetHasDBImage(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var firstOrDefault = context.TblStyleImages.FirstOrDefault(x => x.TblStyle == Iserial && x.DefaultImage);
                if (firstOrDefault != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}