using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblRequestForSampleItem> GetTblRequestForSampleItem(int tblRequestForSample, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblRequestForSampleItems.Include("TblColor1").Where(x => x.TblRequestForSample == tblRequestForSample).ToList();
                var fabricsIserial = query.Where(x => x.FabricType != "Accessories").Select(x => x.Item);
                var accIserial = query.Where(x => x.FabricType == "Accessories").Select(x => x.Item);
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
                                  }).Take(20).ToList();

                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader
                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(20).ToList());
                itemsList = itemsquery;

                return query;
            }
        }

        [OperationContract]
        private TblRequestForSampleItem UpdateOrInsertTblRequestForSampleItem(TblRequestForSampleItem newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblRequestForSampleItems.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblRequestForSampleItems
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
        private int DeleteTblRequestForSampleItem(TblRequestForSampleItem row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRequestForSampleItems
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}