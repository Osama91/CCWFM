using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblIncomeStatmentDesignHeader> GetTblIncomeStatmentDesignHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblIncomeStatmentDesignHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblIncomeStatmentDesignHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblIncomeStatmentDesignHeaders.
                        Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblIncomeStatmentDesignHeaders.Count();
                    query = entity.TblIncomeStatmentDesignHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIncomeStatmentDesignHeader UpdateOrInsertTblIncomeStatmentDesignHeader(TblIncomeStatmentDesignHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblIncomeStatmentDesignHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblIncomeStatmentDesignHeaders
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
        private int DeleteTblIncomeStatmentDesignHeader(TblIncomeStatmentDesignHeader row, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblIncomeStatmentDesignHeaders.SingleOrDefault(e => e.Iserial == row.Iserial);
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblIncomeStatmentDesignDetail> GetTblIncomeStatmentDesignDetail(int skip, int take, int incomeStatmentDesignHeaderId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblIncomeStatmentDesignDetail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    filter = filter + " and it.TblIncomeStatmentDesignHeader ==(@TblIncomeStatmentDesignHeader0)";
                    valuesObjects.Add("TblIncomeStatmentDesignHeader0", incomeStatmentDesignHeaderId);
                    fullCount = entity.TblIncomeStatmentDesignDetails.Where(filter, parameterCollection).Count();
                    query = entity.TblIncomeStatmentDesignDetails.OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblIncomeStatmentDesignDetails.Count(v => v.TblIncomeStatmentDesignHeader == incomeStatmentDesignHeaderId);
                    query = entity.TblIncomeStatmentDesignDetails.Where(x => x.TblIncomeStatmentDesignHeader == incomeStatmentDesignHeaderId).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIncomeStatmentDesignDetail UpdateOrInsertIncomeStatmentDesignDetail(TblIncomeStatmentDesignDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblIncomeStatmentDesignDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblIncomeStatmentDesignDetails
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
        private int DeleteIncomeStatmentDesignDetail(TblIncomeStatmentDesignDetail row, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblIncomeStatmentDesignDetails.SingleOrDefault(e => e.Iserial == row.Iserial);
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}