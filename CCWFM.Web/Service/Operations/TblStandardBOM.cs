using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblStandardBomHeader> GetTblStandardBOMHeader(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblStandardBomHeader> query;
                if (filter != null)
                {
                    var brandTempQuery = context.TblUserBrandSections.Where(x => x.TblAuthUser == userIserial).ToList();
                    var brandSectionQuery = brandTempQuery.Select(x => x.TblLkpBrandSection).Distinct();
                    var brandQuery = brandTempQuery.Select(x => x.BrandCode).Distinct();

                    if (brandQuery.Any())
                    {
                        filter = filter + " and (";
                    }

                    int brandcount = 0;
                    foreach (var brand in brandQuery)
                    {
                        brandcount++;
                        if (brand == brandQuery.FirstOrDefault())
                        {
                            filter = filter + " it.Brand ==(@b)";
                            valuesObjects.Add("b", brand);
                        }
                        if (brand != brandQuery.FirstOrDefault() && brand != brandQuery.LastOrDefault())
                        {
                            filter = filter + " or it.Brand ==(@b" + brandcount + " )";
                            valuesObjects.Add("b" + brandcount + " ", brand);
                        }
                        if (brand == brandQuery.LastOrDefault() && brand != brandQuery.FirstOrDefault())
                        {
                            filter = filter + " or it.Brand ==(@bl)";
                            valuesObjects.Add("bl", brand);
                        }
                        if (brand == brandQuery.LastOrDefault())
                        {
                            filter = filter + ")";
                        }
                    }

                    if (brandSectionQuery.Any())
                    {
                        filter = filter + " and (";
                    }
                    int brandSectioncount = 0;
                    foreach (var brandSection in brandSectionQuery)
                    {
                        brandSectioncount++;
                        if (brandSection == brandSectionQuery.FirstOrDefault())
                        {
                            filter = filter + " it.TblLkpBrandSection ==(@bs)";
                            valuesObjects.Add("bs", brandSection);
                        }
                        if (brandSection != brandSectionQuery.FirstOrDefault() &&
                            brandSection != brandSectionQuery.LastOrDefault())
                        {
                            filter = filter + " or it.TblLkpBrandSection ==(@bs" + brandSectioncount + " )";
                            valuesObjects.Add("bs" + brandcount + " ", brandSection);
                        }
                        if (brandSection == brandSectionQuery.LastOrDefault() &&
                            brandSection != brandSectionQuery.FirstOrDefault())
                        {
                            filter = filter + " or it.TblLkpBrandSection ==(@bsl)";
                            valuesObjects.Add("bsl", brandSection);
                        }
                        if (brandSection == brandSectionQuery.LastOrDefault())
                        {
                            filter = filter + ")";
                        }
                    }
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblStandardBomHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        context.TblStandardBomHeaders.Include("TblLkpBrandSection1").Include("TblFactoryGroup1").Include("TblComplexityGroup1")
                            .Include("TblLkpSeason1")
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount =
                        context.TblStandardBomHeaders.Count(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand));
                    query = context.TblStandardBomHeaders.Include("TblLkpBrandSection1").Include("TblLkpSeason1").Include("TblFactoryGroup1").Include("TblComplexityGroup1")
                        .OrderBy(sort)
                        .Where(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand))
                        .Skip(skip)
                        .Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblStandardBomHeader UpdateOrInsertTblStandardBOMHeader(TblStandardBomHeader newRow, bool save,
            int index, out int outindex, string UserName)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblStandardBomHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblStandardBomHeaders
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
        private TblStandardBomHeader DeleteTblStandardBOMHeader(TblStandardBomHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStandardBomHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();

                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private List<TblStandardBOM> GetTblStandardBOM(int salesOrder, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStandardBOMs.Include("TblColor1").Include("BOM_CalcMethod").Where(x => x.TblStandardBomHeader == salesOrder);

                var fabricsIserial = query.Where(x => x.BOM_FabricType != "Accessories").Select(x => x.BOM_Fabric);

                var accIserial = query.Where(x => x.BOM_FabricType == "Accessories").Select(x => x.BOM_Fabric);
                //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                var itemsquery = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                  where (fabricsIserial.Any(l => x.Iserial == l))

                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).Take(100).ToList();

                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories"
                                         ,
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,

                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(100).ToList());
                itemsList = itemsquery;
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStandardBOM> UpdateOrInsertTblStandardBOM(List<TblStandardBOM> newTblStandardBOMList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var row in newTblStandardBOMList)
                {
                    if (row.Iserial != 0)
                    {
                        var oldRow = (from e in context.TblStandardBOMs
                                      where e.Iserial == row.Iserial
                                      select e).SingleOrDefault();

                        GenericUpdate(oldRow, row, context);
                    }
                    else
                    {
                        context.TblStandardBOMs.AddObject(row);
                    }
                }

                context.SaveChanges();
                return newTblStandardBOMList;
            }
        }

        [OperationContract]
        private int DeleteTblStandardBOM(TblStandardBOM row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStandardBOMs
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}