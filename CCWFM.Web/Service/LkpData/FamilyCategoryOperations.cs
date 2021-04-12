using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using CCWFM.Web.Service.Operations;
using System;
using System.Data.Objects.SqlClient;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        [OperationContract]
        private List<TblLkpDirection> FamilyCategory_GetTblDirection(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                fullCount = context.TblLkpDirections.Count();
                IQueryable<TblLkpDirectionLink> query;
                //if (filter != null)
                //{
                //    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                //    fullCount = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).Count();
                //    query = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                //}
                //else
                //{
                //    fullCount = context.TblLkpDirections.Count();
                //    query = context.TblLkpDirections.OrderBy(sort).Skip(skip).Take(take);
                //}
                return context.TblLkpDirections.ToList();

            }
        }

        [OperationContract]
        private List<TblStyleCategory> FamilyCategory_GetTblStyleCategory(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                fullCount = context.TblLkpDirections.Count();
                IQueryable<TblStyleCategory> query;
                //if (filter != null)
                //{
                //    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                //    fullCount = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).Count();
                //    query = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                //}
                //else
                //{
                //    fullCount = context.TblLkpDirections.Count();
                //    query = context.TblLkpDirections.OrderBy(sort).Skip(skip).Take(take);
                //}
                return context.TblStyleCategories.ToList();

            }
        }

        [OperationContract]
        private List<TblFamily> FamilyCategory_GetTblFamily(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {

                // fullCount = context.TblFamilies.Count();
                //IQueryable<TblStyleCategory> query;
                //if (filter != null)
                //{
                //    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                //    fullCount = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).Count();
                //    query = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                //}
                //else
                //{
                //    fullCount = context.TblLkpDirections.Count();
                //    query = context.TblLkpDirections.OrderBy(sort).Skip(skip).Take(take);
                //}

                // Workflows.Where(w => w.Activities.Any())
                //context.TblFamilies.Where(w => w.Code.Any(context.tblActiveFamilyCode.a)




                var query = context.TblFamilies.Where(a => context.tblActiveFamilyCodes.Any(x => x.code == a.Code));

                //var query = (from activefamilies in context.tblActiveFamilyCode
                //             join families in context.TblFamilies on families.Code equals activefamilies.code
                //             select families);

                fullCount = query.Count();
                return query.ToList();

            }
        }

        [OperationContract]
        private List<TblSubFamily> FamilyCategory_GetTblSubFamily(int Family)
        {
            //using (var context = new WorkFlowManagerDBEntities())
            //{
            //    fullCount = context.TblSubFamilies.Count();
            //    IQueryable<TblStyleCategory> query;
            //    //if (filter != null)
            //    //{
            //    //    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

            //    //    fullCount = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).Count();
            //    //    query = context.TblLkpDirections.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
            //    //}
            //    //else
            //    //{
            //    //    fullCount = context.TblLkpDirections.Count();
            //    //    query = context.TblLkpDirections.OrderBy(sort).Skip(skip).Take(take);
            //    //}
            //    return context.TblSubFamilies.ToList();

            //}

            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSubFamilies.Where(x => x.TblFamily == Family);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblLkpDirectionLink> FamilyCategory_GetTblDirectionLink(string brand, int brandSection)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblLkpDirectionLinks.Include("TblLkpDirection1").Where(x => x.TblLkpBrandSection == brandSection && x.TblBrand == brand);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStyleCategoryLink> FamilyCategory_GetTblCategoryLink(string brand, int brandSection, int direction)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleCategoryLinks.Include("TblStyleCategory1").Where(x => x.TblLkpBrandSection ==
                brandSection && x.TblBrand == brand && x.TblLkpDirection == direction);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblSalesOrderColorTheme> FamilyCategory_GetTblThemeLink(string tblBrand,int tblBrandSection,   int tblLKPSeason)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSalesOrderColorThemes.Where(x=> x.TblBrand == tblBrand && x.TblLkpBrandSection == tblBrandSection
                                                                     && x.TblLkpSeason == tblLKPSeason)
                                                                       .OrderBy(x=>x.Iserial);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblFamilyCategoryLink> FamilyCategory_GetTblFamilyCategoryLink(string brand, int brandSection, int direction, int category)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblFamilyCategoryLinks.Include("TblFamily1").Where(x => x.TblLkpBrandSection == brandSection
                && x.TblBrand == brand && x.TblLkpDirection == direction && x.TblStyleCategory == category);
                
                var newItems = (from s in query
                                where context.tblActiveFamilyCodes.Any(es => (es.code == s.TblFamily1.Code))
                                select s).ToList();

                return newItems.ToList();
            }
        }

        [OperationContract]
        private List<TblSubFamilyCategoryLink> FamilyCategory_GetTblSubFamilyCategoryLink(string brand, int brandSection, int direction, int category, int Family)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblSubFamilyCategoryLinks.Include("TblSubFamily1").Where(x => x.TblLkpBrandSection == brandSection
                && x.TblBrand == brand && x.TblLkpDirection == direction && x.TblStyleCategory == category
                && x.TblFamily == Family);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<string> FamilyCategory_GetRepeatedStyles(string brand, string brandSection, string family, string season)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                List<string> AvaliableStyles = new List<string>();
                int bs = int.Parse(brandSection);
                int f = int.Parse(family);
                int se = int.Parse(season);

                var query = context.TblStyles.Where(x => x.Brand == brand && x.TblLkpBrandSection == bs && x.TblFamily == f && x.TblLkpSeason == se).ToList().Where(c => Convert.ToInt32(c.SerialNo) < 11).ToList();
                // if (query != null)
                // {
                if (query.Count > 0)
                {
                    for (int i = 1; i < 11; i++)
                    {

                        if (query.Where(x => Convert.ToInt32(x.SerialNo) == i).FirstOrDefault() == null)
                        {
                            AvaliableStyles.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 11; i++)
                    {
                        AvaliableStyles.Add(i.ToString());
                    }
                }
                // }
                return AvaliableStyles;
            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblLkpBrandSectionLink(string brand, int section, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblLkpBrandSectionLink newRow = new TblLkpBrandSectionLink();
                    newRow.TblBrand = brand;
                    newRow.TblLkpBrandSection = section;
                    context.TblLkpBrandSectionLinks.AddObject(newRow);
                }
                else
                {
                    //Delete From TblLkpBrandSectionLink
                    var oldBrandSectionRow = (from e in context.TblLkpBrandSectionLinks
                                              where e.TblLkpBrandSection == section
                                              && e.TblBrand == brand
                                              select e).SingleOrDefault();

                    if (oldBrandSectionRow != null) context.DeleteObject(oldBrandSectionRow);
                    context.SaveChanges();

                    //Delete From TblLkpdirectionLink
                    var oldDirectionRow = (from e in context.TblLkpDirectionLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand
                                           select e);

                    foreach (var row in oldDirectionRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();

                    //Delete From TblLkpstyleCategoryLink
                    var oldCategoryRow = (from e in context.TblStyleCategoryLinks
                                          where e.TblLkpBrandSection == section
                                          && e.TblBrand == brand
                                          select e);

                    foreach (var row in oldCategoryRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldFamilyRow = (from e in context.TblFamilyCategoryLinks
                                        where e.TblLkpBrandSection == section
                                        && e.TblBrand == brand
                                        select e);

                    foreach (var row in oldFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldSubFamilyRow = (from e in context.TblSubFamilyCategoryLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand
                                           select e);
                    foreach (var row in oldSubFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();
                }
                return context.SaveChanges();
            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblLkpDirectionLink(string brand, int section, int direction, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblLkpDirectionLink newRow = new TblLkpDirectionLink();
                    newRow.TblBrand = brand;
                    newRow.TblLkpBrandSection = section;
                    newRow.TblLkpDirection = direction;
                    context.TblLkpDirectionLinks.AddObject(newRow);
                }
                else
                {

                    //Delete From TblLkpdirectionLink
                    var oldDirectionRow = (from e in context.TblLkpDirectionLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand && e.TblLkpDirection == direction
                                           select e).SingleOrDefault();

                    if (oldDirectionRow != null) context.DeleteObject(oldDirectionRow);
                    context.SaveChanges();

                    //Delete From TblLkpstyleCategoryLink
                    var oldCategoryRow = (from e in context.TblStyleCategoryLinks
                                          where e.TblLkpBrandSection == section
                                          && e.TblBrand == brand && e.TblLkpDirection == direction
                                          select e);

                    foreach (var row in oldCategoryRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldFamilyRow = (from e in context.TblFamilyCategoryLinks
                                        where e.TblLkpBrandSection == section
                                        && e.TblBrand == brand && e.TblLkpDirection == direction
                                        select e);

                    foreach (var row in oldFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }

                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldSubFamilyRow = (from e in context.TblSubFamilyCategoryLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand && e.TblLkpDirection == direction
                                           select e);

                    foreach (var row in oldSubFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();
                }
                return context.SaveChanges();
            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblStyleCategoryLink(string brand, int section, int direction, int category, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblStyleCategoryLink newRow = new TblStyleCategoryLink();
                    newRow.TblBrand = brand;
                    newRow.TblLkpBrandSection = section;
                    newRow.TblLkpDirection = direction;
                    newRow.TblStyleCategory = category;
                    context.TblStyleCategoryLinks.AddObject(newRow);
                }
                else
                {

                    //Delete From TblLkpstyleCategoryLink
                    var oldCategoryRow = (from e in context.TblStyleCategoryLinks
                                          where e.TblLkpBrandSection == section
                                          && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category
                                          select e).SingleOrDefault();

                    if (oldCategoryRow != null) context.DeleteObject(oldCategoryRow);
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldFamilyRow = (from e in context.TblFamilyCategoryLinks
                                        where e.TblLkpBrandSection == section
                                        && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category
                                        select e);

                    foreach (var row in oldFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldSubFamilyRow = (from e in context.TblSubFamilyCategoryLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category
                                           select e);

                    foreach (var row in oldSubFamilyRow)
                    {
                        if (row != null) context.DeleteObject(row);
                    }
                    context.SaveChanges();
                }
                return context.SaveChanges();

            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblFamilyCategoryLink(string brand, int section, int direction, int category, int family, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblFamilyCategoryLink newRow = new TblFamilyCategoryLink();
                    newRow.TblBrand = brand;
                    newRow.TblLkpBrandSection = section;
                    newRow.TblLkpDirection = direction;
                    newRow.TblStyleCategory = category;
                    newRow.TblFamily = family;
                    context.TblFamilyCategoryLinks.AddObject(newRow);
                }
                else
                {
                    //Delete From TblLkpFamilyCategoryLink
                    var oldFamilyRow = (from e in context.TblFamilyCategoryLinks
                                        where e.TblLkpBrandSection == section
                                        && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category && e.TblFamily == family
                                        select e).SingleOrDefault();

                    if (oldFamilyRow != null) context.DeleteObject(oldFamilyRow);
                    context.SaveChanges();

                    //Delete From TblLkpFamilyCategoryLink
                    var oldSubFamilyRow = (from e in context.TblSubFamilyCategoryLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category && e.TblFamily == family
                                           select e);


                    if (oldSubFamilyRow != null)
                    {
                        foreach (var row in oldSubFamilyRow)
                        {
                            context.DeleteObject(row);
                        }

                    }

                    context.SaveChanges();
                }
                return context.SaveChanges();
            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblSubFamilyCategoryLink(string brand, int section, int direction, int category, int family, int Subfamily, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblSubFamilyCategoryLink newRow = new TblSubFamilyCategoryLink();
                    newRow.TblBrand = brand;
                    newRow.TblLkpBrandSection = section;
                    newRow.TblLkpDirection = direction;
                    newRow.TblStyleCategory = category;
                    newRow.TblFamily = family;
                    newRow.TblSubFamily = Subfamily;
                    context.TblSubFamilyCategoryLinks.AddObject(newRow);
                }
                else
                {

                    //Delete From TblLkpFamilyCategoryLink
                    var oldSubFamilyRow = (from e in context.TblSubFamilyCategoryLinks
                                           where e.TblLkpBrandSection == section
                                           && e.TblBrand == brand && e.TblLkpDirection == direction && e.TblStyleCategory == category
                                           && e.TblFamily == family && e.TblSubFamily == Subfamily
                                           select e).SingleOrDefault();
                    if (oldSubFamilyRow != null) context.DeleteObject(oldSubFamilyRow);

                    context.SaveChanges();
                }
                return context.SaveChanges();
            }
        }


        //[OperationContract]
        //private TblFamilyLink UpdateOrDeleteTblFamilyLink(TblFamilyLink newRow, bool save, int index, out int outindex)
        //{
        //    outindex = index;
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        if (save)
        //        {
        //            context.TblFamilyLinks.AddObject(newRow);
        //        }
        //        else
        //        {
        //            var oldRow = (from e in context.TblFamilyLinks
        //                          where e.TblFamily == newRow.TblFamily
        //                          && e.TblBrand == newRow.TblBrand
        //                          && e.TblLkpBrandSection == newRow.TblLkpBrandSection
        //                          select e).SingleOrDefault();
        //            if (oldRow != null) context.DeleteObject(oldRow);
        //        }
        //        context.SaveChanges();
        //        return newRow;
        //    }
        //}

        [OperationContract]
        private List<TblStyleTNA> FamilyCategory_GetTblStyleTNA()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleTNAs.OrderBy(X => X.OrderNo);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStyleTNARoute> FamilyCategory_GetTNARouteByStyle(int tblstyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleTNARoutes.Include("TblStyleTNA1").Where(x => x.TblStyle == tblstyle);
                return query.ToList();
            }
        }

        [OperationContract]
        private int FamilyCategory_UpdateOrDeleteTblStyleTNARoute(int tblstyle, int TblStyleTNA, bool save)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    TblStyleTNARoute newRow = new TblStyleTNARoute();
                    newRow.TblStyle = tblstyle;
                    newRow.TblStyleTNA = TblStyleTNA;
                    context.TblStyleTNARoutes.AddObject(newRow);
                    return context.SaveChanges();
                }
                else
                {

                    //Delete From TblStyleTNARoute
                    var oldStyleTNARouteRow = (from e in context.TblStyleTNARoutes
                                          where e.TblStyle == tblstyle
                                          && e.TblStyleTNA == TblStyleTNA
                                            select e).SingleOrDefault();

                    if (oldStyleTNARouteRow != null) context.DeleteObject(oldStyleTNARouteRow);
                   return context.SaveChanges();
                }
            }
        }
    }
}