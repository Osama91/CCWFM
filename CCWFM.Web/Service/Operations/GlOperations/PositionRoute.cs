using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Data.Entity;
namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblPositionRouteHeader> GetTblPositionRouteHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPositionRouteHeader> query;
                if (filter != null)
                {               

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entityGroup.TblPositionRouteHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entityGroup.TblPositionRouteHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entityGroup.TblPositionRouteHeaders.Count();
                    query = entityGroup.TblPositionRouteHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPositionRouteHeader UpdateOrInsertTblPositionRouteHeader(TblPositionRouteHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {                    
                    entityGroup.TblPositionRouteHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entityGroup.TblPositionRouteHeaders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entityGroup);
                    }
                }
                entityGroup.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPositionRouteHeader(TblPositionRouteHeader row, int index, string company)
        {
            using (var entityGroup = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entityGroup.TblPositionRouteHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null)
                {                
                    entityGroup.DeleteObject(query);
                }

                entityGroup.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblPositionRoute> GetTblPositionRoute(int skip, int take, int group, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<EmployeesView> EmpList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblPositionRoute> query;
                if (filter != null)
                {                 
                    filter = filter + " and it.TblPositionRouteHeader ==(@Group0)";
                    valuesObjects.Add("Group0", group);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblPositionRoutes.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblPositionRoutes.Include(t => t.TblStore).Include(t=>t.TblStore1).Include("TblPositionRouteHeader1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblPositionRoutes.Count(x => x.TblPositionRouteHeader == group);
                    query = entity.TblPositionRoutes.Include(t => t.TblStore).Include(t => t.TblStore1).Include("TblPositionRouteHeader1").OrderBy(sort).Where(v => v.TblPositionRouteHeader == group).Skip(skip).Take(take);
                }

                using (var context = new TimeAttEntities())
                {
                    List<string> intList = query.Select(x => x.Emplid).ToList();                    
                    EmpList = context.EmployeesViews.Where(x =>  intList.Contains(x.Emplid) ).ToList();                    
                }
              
                return query.ToList();
            }
        }

        [OperationContract]
        private TblPositionRoute UpdateOrInsertTblPositionRoute(TblPositionRoute newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblPositionRoutes.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblPositionRoutes
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblPositionRoute(TblPositionRoute row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblPositionRoutes
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}