using CCWFM.Models;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace CCWFM.Web.Service.BankDepositOp
{
    public partial class BankDepositService
    {
        [OperationContract]
        private List<TblStoreCommission> GetStoreCommission(string sort, string filter,
            Dictionary<string, object> valuesObjects, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                IQueryable<TblStoreCommission> query;               
                var defaultQuery = context.TblStoreCommissions;
                IEnumerable<TblStoreCommission> notSavedStores = new List<TblStoreCommission>();
                if (string.IsNullOrWhiteSpace(filter))
                {
                    query = defaultQuery.OrderBy(sort);
                    notSavedStores = context.TblStores
                   .Where(s => !query.Any(q => q.Tblstore == s.iserial) && s.Type != 2)
                   .OrderBy(s => s.code)
                   .Select(s => new
                   {
                       s.iserial,
                   }).ToList().Select(s => new TblStoreCommission
                   {
                       Tblstore = s.iserial,
                       IsActive = false,
                   });
                }
                else
                {
                    filter = filter.Replace("it.code", "it.Tblstore1.code").Replace("it.ENAME", "it.Tblstore1.ENAME");
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort);
                }
              
                var result = query.ToList();
                result.AddRange(notSavedStores);
                return result;
            }
        }
        
        [OperationContract]
        private List<TblStoreCommission> UpdateOrInsertStoreCommission(List<TblStoreCommission> newRows, string company)// int index, out int outindex,
        {
            //outindex = index;
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                try
                {
                    foreach (var item in newRows)
                    {
                        var oldRow = context.TblStoreCommissions.FirstOrDefault(th => th.Tblstore == item.Tblstore);

                        if (oldRow != null)
                        {
                            SharedOperation.GenericUpdate(oldRow, item, context);
                        }
                        else
                        {
                            item.TblStore1 = null;
                            context.TblStoreCommissions.AddObject(item);
                        }

                        context.SaveChanges();
                    }
                }
                catch (Exception ex) { throw Helper.GetInnerException(ex); }
                return newRows;
            }
        }

        //[OperationContract]
        //private int DeleteStoreCommission(TblStoreCommission row, string company)
        //{
        //    using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
        //    {
        //        var oldRow = context.TblStoreCommissions.SingleOrDefault(r => r.Tblstore == row.Tblstore);
        //        if (oldRow != null) context.DeleteObject(oldRow);

        //        context.SaveChanges();
        //    }
        //    return row.Tblstore;
        //}
        
    }
}