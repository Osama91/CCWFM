using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblAssetRetail> GetTblAssetRetail(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblAssetRetail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblAssetRetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblAssetRetails.Include("TblAssetGroup1").Include("TblAccount1").Include("TblAccount2").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblAssetRetails.Count();
                    query = entity.TblAssetRetails.Include("TblAssetGroup1").Include("TblAccount1").Include("TblAccount2").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAssetRetail UpdateOrInsertTblAssetRetail(TblAssetRetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblAssetRetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblAssetRetails
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
        private int DeleteTblAssetRetail(TblAssetRetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblAssetRetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null)
                {

                    var Ledger = entity.TblLedgerMainDetails.Where(w => w.EntityAccount == query.Iserial&& w.TblJournalAccountType== 5);
                    if (Ledger.Any())
                    {
                        throw new System.Exception("Cannot delete Because There is a transaction Exisit");
                    }

                    entity.DeleteObject(query);
                }

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblAssetGroup> GetTblAssetGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblAssetGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblAssetGroups.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblAssetGroups.Include("TblAccount1").Include("TblAccount2").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblAssetGroups.Count();
                    query = entity.TblAssetGroups.Include("TblAccount1").Include("TblAccount2").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAssetGroup UpdateOrInsertTblAssetGroup(TblAssetGroup newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblAssetGroups.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblAssetGroups
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
        private int DeleteTblAssetGroup(TblAssetGroup row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblAssetGroups
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}