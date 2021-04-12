using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        /*
           int TblCashDepositHeader,string no, string month,string year,string User,string company
        */
        [OperationContract]
        private TblCashDepositHeader ReveseTBLCashDepoisteHeader
                                      (int TblCashDepositHeader, string no, string month,string year,string User,string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                //entity.sp_ReverceGLTransactionData(TblCashDepositHeader, "TblCashDepositHeader", no, month, year, User, 9);
                ////var query = entity.TblGLTransactionReverses.FirstOrDefault(x => x.TblTransactionHeader == TblCashDepositHeader);
                //if (query != null)
                //{
                //    var reversed = entity.TblCashDepositHeaders.FirstOrDefault(x => x.Iserial == query.TblTransactionHeaderReverse);
                //    return reversed;
                //}
                //else
                //{
                    return null;
                //}
            }
        }
    }
}

