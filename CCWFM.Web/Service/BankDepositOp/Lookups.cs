using CCWFM.Web.Model;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace CCWFM.Web.Service.BankDepositOp
{
    public partial class BankDepositService
    {
        //[PrincipalPermission(SecurityAction.Demand)]
        [OperationContract]
        private List<TblCashDepositType> GetLookUpCashDepositType( string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                IQueryable<TblCashDepositType> query;
                query = context.TblCashDepositTypes.Include(nameof(TblCashDepositType.TblSequence1));

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblBank> GetLookUpBank( string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                IQueryable<TblBank> query;
                query = context.TblBanks.Where(b => b.TblBankGroup1.IsBank).OrderBy(b => b.Ename);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStore> GetLookUpStore(string company, bool active = false)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                IQueryable<TblStore> query;
                query = context.TblStores
                    .Where(s => s.Type != 2 && (s.TblStoreCommission.IsActive == active || !active))
                    .OrderBy(s => s.ENAME);

                return query.ToList();
            }
        }
        [OperationContract]
        private List<TblTenderType> GetLookUpTenderTypes(int TypeIserial, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                IQueryable<TblTenderType> query;
                query = context.TblTenderTypes.Where(T =>
                T.TblCashDepositTypes.Any(dt => dt.Iserial == TypeIserial));

                return query.ToList();
            }
        }
        
    }
}