using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblColorLink> GetTblColorLink(int skip, int take, string brand, int brandSection, int season, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblColorLink> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblBrand LIKE(@Brand0)" + " and it.TblLkpBrandSection ==(@BrandSection0)" + " and it.TblLkpSeason ==(@season0)";
                    valuesObjects.Add("Brand0", brand);
                    valuesObjects.Add("BrandSection0", brandSection);
                    valuesObjects.Add("season0", season);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblColorLinks.Include("TblColor1").Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblColorLinks.Include("TblColor1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblColorLinks.Include("TblColor1").Count(x => x.TblBrand == brand && x.TblLkpBrandSection == brandSection && x.TblLkpSeason == season);
                    query = context.TblColorLinks.Include("TblColor1").Where(x => x.TblBrand == brand && x.TblLkpBrandSection == brandSection && x.TblLkpSeason == season).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblColorLink UpdateOrDeleteTblColorLink(TblColorLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblColorLinks.AddObject(newRow);
                }
                var oldRow = (from e in context.TblColorLinks
                              where e.TblLkpSeason == newRow.TblLkpSeason
                              && e.TblBrand == newRow.TblBrand
                              && e.TblLkpBrandSection == newRow.TblLkpBrandSection
                              && e.TblColor == newRow.TblColor
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblColorLink UpdateColorLinkPanton(TblColorLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblColorLinks
                              where e.TblLkpSeason == newRow.TblLkpSeason
                              && e.TblBrand == newRow.TblBrand
                              && e.TblLkpBrandSection == newRow.TblLkpBrandSection
                              && e.TblColor == newRow.TblColor
                              select e).SingleOrDefault();

                if (oldRow != null)
                {
                    oldRow.PantonCode = newRow.PantonCode;
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private void CopyColorLink(TblColorLink newRow, string brand, int brandsection, int season)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var colorList = (from e in context.TblColorLinks
                                 where e.TblLkpSeason == newRow.TblLkpSeason
                                       && e.TblLkpBrandSection == newRow.TblLkpBrandSection
                                       && e.TblBrand == newRow.TblBrand
                                 select e
                    ).ToList();

                foreach (var tblColorLink in colorList)
                {
                    var oldRow = (from e in context.TblColorLinks
                                  where e.TblLkpSeason == season
                                        && e.TblBrand == brand
                                        && e.TblLkpBrandSection == brandsection
                                        && e.TblColor == tblColorLink.TblColor
                                  select e).SingleOrDefault();
                    if (oldRow == null)
                    {
                        var row = new TblColorLink
                        {
                            TblBrand = brand,
                            TblLkpBrandSection = brandsection,
                            TblLkpSeason = season,
                            TblColor = tblColorLink.TblColor
                        };
                        context.TblColorLinks.AddObject(row);
                    }
                }
                context.SaveChanges();
            }
        }

        //[OperationContract]
        //private TblColorLink DeleteTblColorLink(TblColorLink newRow)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        var oldRow = (from e in context.TblColorLinks
        //                      where e.TblLkpSeason == newRow.TblLkpSeason
        //                      && e.TblLkpSeason == newRow.TblLkpSeason
        //                      && e.TblLkpBrandSection == newRow.TblLkpBrandSection
        //                      && e.TblColor == newRow.TblColor
        //                      select e).SingleOrDefault();
        //        if (oldRow != null) context.DeleteObject(oldRow);

        //        context.SaveChanges();
        //    }
        //    return newRow;
        //}

        //[OperationContract]
        //public List<TblColor> GetAllcolor(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        IQueryable<TblColor> query;
        //        if (filter != null)
        //        {
        //            var parameterCollection = ConvertToParamters(valuesObjects);

        //            fullCount = context.TblColors.Where(filter, parameterCollection.ToArray()).Count();
        //            query = context.TblColors.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
        //        }
        //        else
        //        {
        //            fullCount = context.TblColors.Count();
        //            query = context.TblColors.OrderBy(sort).Skip(skip).Take(take);
        //        }
        //        return query.ToList();
        //    }
        //}

        //[OperationContract]
        //private TblColor UpdateOrInsertTblColor(TblColor newRow, bool save, int index, out int outindex)
        //{
        //    outindex = index;
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        if (save)
        //        {
        //            context.TblColors.AddObject(newRow);
        //        }
        //        else
        //        {
        //            var oldRow = (from e in context.TblColors
        //                          where e.Iserial == newRow.Iserial
        //                          select e).SingleOrDefault();
        //            if (oldRow != null)
        //            {
        //                GenericUpdate(oldRow, newRow, context);
        //            }
        //        }

        //        context.SaveChanges();
        //        return newRow;
        //    }
        //}

        //[OperationContract]
        //private TblColor DeleteTblColor(TblColor newRow)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        var oldRow = (from e in context.TblColors
        //                      where e.Iserial == newRow.Iserial
        //                      select e).SingleOrDefault();
        //        if (oldRow != null) context.DeleteObject(oldRow);

        //        context.SaveChanges();
        //    }
        //    return newRow;
        //}
    }
}