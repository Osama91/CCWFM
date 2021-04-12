using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCostDimSetupHeader> GetTblCostDimSetupHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostDimSetupHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblCostDimSetupHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblCostDimSetupHeaders.Include("TblJournalAccountType1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblCostDimSetupHeaders.Count();
                    query = context.TblCostDimSetupHeaders.Include("TblJournalAccountType1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostDimSetupHeader UpdateOrInsertTblCostDimSetupHeader(TblCostDimSetupHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    context.TblCostDimSetupHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblCostDimSetupHeaders
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
        private int DeleteTblCostDimSetupHeader(TblCostDimSetupHeader row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCostDimSetupHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblCostDimSetupDetail> GetTblCostDimSetupDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostDimSetupDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblCostDimSetupHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblCostDimSetupDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblCostDimSetupDetails.Include("TblCostCenterOption1").Include("TblCostCenterType1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblCostDimSetupDetails.Count(v => v.TblCostDimSetupHeader == groupId);
                    query = context.TblCostDimSetupDetails.Include("TblCostCenterOption1").Include("TblCostCenterType1").OrderBy(sort).Where(v => v.TblCostDimSetupHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostDimSetupDetail UpdateOrInsertTblCostDimSetupDetail(TblCostDimSetupDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    context.TblCostDimSetupDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblCostDimSetupDetails
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
        private int DeleteTblCostDimSetupDetail(TblCostDimSetupDetail row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCostDimSetupDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}