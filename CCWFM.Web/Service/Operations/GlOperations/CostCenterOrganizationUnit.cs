using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {

        [OperationContract]
        private List<PayrollOrganizationUnit> GetPayrollOrganizationUnit()
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {

                return  entity.PayrollOrganizationUnits.ToList();
            }
        }
        [OperationContract]
        private List<TblCostCenterOrganizationUnit> GetTblCostCenterOrganizationUnit(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostCenterOrganizationUnit> query;
                if (filter != null)
                {
                    
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCostCenterOrganizationUnits.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCostCenterOrganizationUnits.Include("TblCostCenter1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCostCenterOrganizationUnits.Count();
                    query = entity.TblCostCenterOrganizationUnits.Include("TblCostCenter1").OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostCenterOrganizationUnit UpdateOrInsertTblCostCenterOrganizationUnits(TblCostCenterOrganizationUnit newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblCostCenterOrganizationUnits.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCostCenterOrganizationUnits
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
        private int DeleteTblCostCenterOrganizationUnit(TblCostCenterOrganizationUnit row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCostCenterOrganizationUnits
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}