using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<tbl_lkp_FabricStructure> GetTblFabricStructure(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_lkp_FabricStructure> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.tbl_lkp_FabricStructure.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tbl_lkp_FabricStructure.Include("tbl_FabricCategories1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_lkp_FabricStructure.Count();
                    query = context.tbl_lkp_FabricStructure.Include("tbl_FabricCategories1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private tbl_lkp_FabricStructure UpdateOrInsertTblFabricStructure(tbl_lkp_FabricStructure newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.tbl_lkp_FabricStructure.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.tbl_lkp_FabricStructure
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
        private int DeleteTblFabricStructure(tbl_lkp_FabricStructure row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tbl_lkp_FabricStructure
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

    }
}