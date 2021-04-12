using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCurrencyTest> GetTblRetailCurrency(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCurrencyTest> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCurrencyTests.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblCurrencyTests
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblCurrencyTests.Where(wde => wde.Aname.Trim() != "").Count();
                    query = entity.TblCurrencyTests
                        .OrderBy(sort).Where(wde => wde.Aname.Trim() != "").Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblChainSetting GetRetailChainSetting(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return entity.TblChainSettings.FirstOrDefault();
            }
        }


        [OperationContract]
        public tblChainSetupTest GetRetailChainSetupByCode(string code,string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return entity.tblChainSetupTests.FirstOrDefault(w=>w.sGlobalSettingCode== code);
            }
        }
    }
}