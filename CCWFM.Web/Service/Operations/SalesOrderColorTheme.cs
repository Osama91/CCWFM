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
        private List<TblSalesOrderColorTheme> GetTblSalesOrderColorTheme(int skip, int take, int season, string brand, int brandSection, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderColorTheme> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLkpSeason ==(@Seaso0)";
                    filter = filter + " and it.TblBrand ==(@brand0)";
                    filter = filter + " and it.TblLkpBrandSection ==(@TblLkpBrandSection0)";
                    valuesObjects.Add("Seaso0", season);
                    valuesObjects.Add("brand0", brand);
                    valuesObjects.Add("TblLkpBrandSection0", brandSection);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblSalesOrderColorThemes.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalesOrderColorThemes.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalesOrderColorThemes.Count(x => x.TblBrand == brand && x.TblLkpSeason == season && x.TblLkpBrandSection == brandSection);
                    query = context.TblSalesOrderColorThemes.Where(x => x.TblBrand == brand && x.TblLkpSeason == season && x.TblLkpBrandSection == brandSection).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalesOrderColorTheme UpdateOrInsertTblSalesOrderColorTheme(TblSalesOrderColorTheme newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblSalesOrderColorThemes.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSalesOrderColorThemes
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
            }

            return newRow;
        }

        [OperationContract]
        private TblSalesOrderColorTheme DeleteTblSalesOrderColorTheme(TblSalesOrderColorTheme row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSalesOrderColorThemes
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}