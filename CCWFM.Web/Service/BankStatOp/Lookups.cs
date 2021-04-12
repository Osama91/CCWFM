using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace CCWFM.Web.Service.BankStatOp
{
    public partial class BankStatService
    {
        [OperationContract]
        private List<TblBankTransactionType> GetLookUpBankTransactionType(string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblBankTransactionType> query;
                query = context.TblBankTransactionTypes;

                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblBank> GetLookUpBank(string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblBank> query;
                query = context.TblBanks;

                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblCurrencyTest> GetLookUpCurrency(string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblCurrencyTest> query;
                query = context.TblCurrencyTests;

                return query.ToList();
            }
        }
    }
}