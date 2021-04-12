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
        private List<TblLkpBrandSectionLink> GetTblBrandSectionLink(string brand, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblLkpBrandSectionLinks.Include("TblLkpBrandSection1")
                    .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                    .Where(x => x.TblLkpBrandSection1.TblUserBrandSections.Any(e => e.BrandCode == brand)
                           && x.TblLkpBrandSection1.TblBrandSectionPermissions.Any(e => e.BrandCode == brand)
                           && x.TblBrand == brand
                           && x.TblLkpBrandSection1.TblUserBrandSections.Any(e => (e.TblAuthUser == userIserial || userIserial == 0) && e.BrandCode == brand));
                return query.ToList();
            }
        }
        
        [OperationContract]
        private List<TblLkpDirectionLink> GetTblDirectionFilterLink(string brand, List<TblLkpBrandSection> _brandSections)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var query = (from x in context.TblLkpDirectionLinks.Include("TblLkpDirection1").ToList()
                             where x.TblBrand == brand select x).ToList();
                query = (from s in query
                         where _brandSections.Any(es => (es.Iserial == s.TblLkpBrandSection))
                         select s).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStyleCategoryLink> GetTblStyleCategoryFilterLink(string brand, List<TblLkpBrandSection> _brandSections, List<TblLkpDirection> _directions)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblStyleCategoryLinks.Include("TblStyleCategory1").ToList()
                             where x.TblBrand == brand
                             select x).ToList();

                query = (from s in query
                         where _brandSections.Any(es => (es.Iserial == s.TblLkpBrandSection))
                         select s).ToList();

                query = (from s in query
                         where _directions.Any(es => (es.Iserial == s.TblLkpDirection))
                         select s).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblFamilyCategoryLink> GetTblFamilyCategoryFilterLink(string brand, List<TblLkpBrandSection> _brandSections, List<TblLkpDirection> _directions, List<TblStyleCategory> _styleCategories)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblFamilyCategoryLinks.Include("TblFamily1").ToList()
                             where x.TblBrand == brand
                             select x).ToList();

                query = (from s in query
                         where _brandSections.Any(es => (es.Iserial == s.TblLkpBrandSection))
                         select s).ToList();

                query = (from s in query
                         where _directions.Any(es => (es.Iserial == s.TblLkpDirection))
                         select s).ToList();

                query = (from s in query
                         where _styleCategories.Any(es => (es.Iserial == s.TblStyleCategory))
                         select s).ToList();

                return query.ToList();
            }
        }


        [OperationContract]
        private List<TblSubFamilyCategoryLink> GetTblSubFamilyCategoryFilterLink(string brand, List<TblLkpBrandSection> _brandSections, List<TblLkpDirection> _directions,
            List<TblStyleCategory> _styleCategories, List<TblFamily> _families)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblSubFamilyCategoryLinks.Include("TblSubFamily1").ToList()
                             where x.TblBrand == brand
                             select x).ToList();

                query = (from s in query
                         where _brandSections.Any(es => (es.Iserial == s.TblLkpBrandSection))
                         select s).ToList();

                query = (from s in query
                         where _directions.Any(es => (es.Iserial == s.TblLkpDirection))
                         select s).ToList();

                query = (from s in query
                         where _styleCategories.Any(es => (es.Iserial == s.TblStyleCategory))
                         select s).ToList();

                query = (from s in query
                         where _families.Any(es => (es.Iserial == s.TblFamily))
                         select s).ToList();

                return query.ToList();
            }
        }
    }
}