using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblStore> SeasonForStoreByUser(string storename, string code, string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                db.Connection.Open();
                var u =
                    db.TblStores.Where(z => (z.ENAME.StartsWith(storename) || storename == null || storename == "") && z.Type == 4 && (z.code.StartsWith(code) || code == null || code == "")).ToList();

                return u;
            }
        }

        [OperationContract]
        public List<TblStore> GetStores(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects, out int fullCount,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblStore> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblStores.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblStores.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblStores.Count();
                    query = context.TblStores.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private List<StoreForAllCompany> GetStoresForAllCompany(int skip, int take, string organization, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<StoreForAllCompany> query;
                if (filter != null)
                {
                    if (organization != null)
                    {
                        filter = filter + " and it.Organization ==(@Group0)";
                        valuesObjects.Add("Group0", organization);
                    }
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.StoreForAllCompanies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.StoreForAllCompanies.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.StoreForAllCompanies.Count(x => x.Organization == organization || organization == null);
                    query = context.StoreForAllCompanies.OrderBy(sort).Where(x => x.Organization == organization || organization == null).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

    }
}