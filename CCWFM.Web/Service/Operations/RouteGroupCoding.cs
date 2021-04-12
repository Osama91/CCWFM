using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblRouteGroup> GetTblRouteGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRouteGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblRouteGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblRouteGroups.Include("TblService1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblRouteGroups.Count();
                    query = context.TblRouteGroups.Include("TblService1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblRouteGroup UpdateOrInsertTblRouteGroup(TblRouteGroup newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblRouteGroups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblRouteGroups
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
        private int DeleteTblRouteGroup(TblRouteGroup row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRouteGroups
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblRoute> GetTblRoute(int skip, int take, int routeGroup, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRoute> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRouteGroup ==(@Group0)";
                    valuesObjects.Add("Group0", routeGroup);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblRoutes.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblRoutes.Include("TblService1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblRoutes.Count(x => x.TblRouteGroup == routeGroup);
                    query = context.TblRoutes.Include("TblService1").OrderBy(sort).Where(x => x.TblRouteGroup == routeGroup).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblRoute UpdateOrInsertTblRoute(TblRoute newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblRoutes.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblRoutes
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
        private int DeleteTblRoute(TblRoute row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRoutes
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<WRKCTRTABLE> GetAxRoutListe(string vendor)
        {
            using (var context = new ax2009_ccEntities())
            {
                return context.WRKCTRTABLEs.Where(x => x.VENDID == vendor && x.DATAAREAID == "Ccm" && x.ISGROUP == 0).ToList();
            }
        }

        [OperationContract]
        private List<WRKCTRTABLE> GetAxRouteGroup()
        {
            using (var context = new ax2009_ccEntities())
            {
                return context.WRKCTRTABLEs.Where(x => x.DATAAREAID == "Ccm" && x.ISGROUP == 1).ToList();
            }
        }
    }
}