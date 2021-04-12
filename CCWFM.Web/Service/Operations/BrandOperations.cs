using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Brand> GetAllBrands(int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var brand = context.TblUserBrandSections
                  .Where(x => x.TblAuthUser == userIserial).Select(x => x.BrandCode).Distinct();

                return context.Brands.Where(x => brand.Contains(x.Brand_Code) || userIserial == 0).OrderBy(x => x.Brand_Code).ToList();
            }
        }

        [OperationContract]
        public List<TblItemDownLoadDef> GetBrands(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects, out int fullCount,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblItemDownLoadDef> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblItemDownLoadDefs.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblItemDownLoadDefs.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblItemDownLoadDefs.Count();
                    query = context.TblItemDownLoadDefs.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }
    }
}