using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<INVENTTABLE> GetAxItems(string ItemGroup, string DataArea)
        {
            using (var context = new ax2009_ccEntities())
            {
                var item = (from i in context.INVENTTABLEs.Where(x => x.ITEMGROUPID != "Fp" && x.DATAAREAID == DataArea
                    && (x.ITEMGROUPID == ItemGroup || ItemGroup == null))
                            select i).ToList();
                return item;
            }
        }

        [OperationContract]
        public List<CONFIGTABLE> GetAxConfigTable(string Item, string DataArea)
        {
            using (var context = new ax2009_ccEntities())
            {
                var Config = (from c in context.CONFIGTABLEs.Where(x => x.ITEMID == Item && x.DATAAREAID == DataArea)
                              select c).ToList();
                return Config;
            }
        }

        [OperationContract]
        private List<viewstyle> Getviewstyle(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, string company,int brand, out int fullCount)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<viewstyle> query;
                if (filter != null)
                {
                    filter = filter + " and it.ItemStoreGroup ==(@style0) ";
                    valuesObjects.Add("style0", brand);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.viewstyles.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.viewstyles.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.viewstyles.Count(w=>w.ItemStoreGroup==brand);
                    query = context.viewstyles.OrderBy(sort).Where(w => w.ItemStoreGroup == brand).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }
    }
}