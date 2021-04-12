using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblSizeGroup> GetTblSizeGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSizeGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSizeGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSizeGroups.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSizeGroups.Count();
                    query = context.TblSizeGroups.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }


        [OperationContract]
        private List<TblSizeGroupLink> FamilyCategory_GetTblSizeGroupLink(string brand, string brandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int sec = int.Parse(brandSection);
                var query = context.TblSizeGroupLinks.Include("TblSizeGroup1").Where(x => x.TblLkpBrandSection == sec && x.TblBrand == brand);

                return query.ToList();
            }
        }
        [OperationContract]
        private int SaveBrandSectionFamilySizeGroup(int Brand, int Section, int Family, int SizeGroup)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var checkItem = context.TblBrandSectionFamilySizegroups.FirstOrDefault(a => a.TblBrand == Brand
                    && a.TblFamily == Family
                    && a.TblSizeGroup == SizeGroup);
                    if (checkItem != null) // return 2 if object exist
                        return 2;
                    context.TblBrandSectionFamilySizegroups.AddObject(new TblBrandSectionFamilySizegroup()
                    {
                        TblBrand = Brand,
                        TblLkpBrandSection = Section,
                        TblFamily = Family,
                        TblSizeGroup = SizeGroup
                    });
                    context.SaveChanges();
                    return 1;
                }
                catch { return 0; }
            }
        }
        [OperationContract]
        private List<tbl_lkp_FabricDesignes> FamilyCategory_GetTblFabricDesign(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tbl_lkp_FabricDesignes.Where(a => context.tblActiveDesignCode.Any(x => x.Code == a.Code));

                fullCount = query.Count();
                return query.ToList();
            }
        }


        [OperationContract]
        private List<TblStyleStatu> FamilyCategory_GetTblStyleStatus(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleStatus.Where(a => context.tblActiveStyleStatus.Any(x => x.Code == a.Code));

                fullCount = query.Count();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSizeGroup UpdateOrInsertTblSizeGroup(TblSizeGroup newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSizeGroups
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblSizeGroups.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSizeGroup(TblSizeGroup row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSizeGroups
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSize> GetTblSizeCode(int skip, int take, int groupId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSizes.OrderBy(x => x.Id).Where(v => v.TblSizeGroup == groupId).Skip(skip).Take(take);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSize UpdateOrInsertTblSizeCode(TblSize newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var sizeGroupInStyles = context.TblStyles.Any(w => w.TblSizeGroup == newRow.TblSizeGroup);
                if (sizeGroupInStyles)
                {
                    return newRow;
                }

                var oldRow = (from e in context.TblSizes
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblSizes.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblSize DeleteTblSizeCode(TblSize row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSizes
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}