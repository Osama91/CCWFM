using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Vendor> GetVendors(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string dataaraeid, int itemid, string type,string season)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<Vendor> query;
                if (itemid == 0)
                {
                    if (filter != null)
                    {
                        filter = filter + " and it.DATAAREAID ==(@DATAAREAID0)";
                        valuesObjects.Add("DATAAREAID0", dataaraeid);
                        var parameterCollection = ConvertToParamters(valuesObjects);

                        fullCount = context.Vendors.Where(filter, parameterCollection.ToArray()).Count();
                        query =
                            context.Vendors.Where(filter, parameterCollection.ToArray())
                                .OrderBy(x => x.vendor_code)
                                .Skip(skip)
                                .Take(take);
                    }
                    else
                    {
                        fullCount = context.Vendors.Count(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower());
                        query =
                            context.Vendors.Where(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower())
                                .OrderBy(x => x.vendor_code)
                                .Skip(skip)
                                .Take(take);
                    }
                }
                else
                {
                    var vendors = context.TblTradeAgreementDetails.Include("TblTradeAgreementHeader1").Where(x => x.ItemCode == itemid && x.ItemType == type&& x.TblTradeAgreementHeader1.TblTradeAgreementTransaction1.TblLkpSeason1.ShortCode== season).Select(w => w.TblTradeAgreementHeader1.Vendor);
                    fullCount = 0;
                    query = context.Vendors.Where(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower() && vendors.Contains(x.vendor_code));
                }
                return query.ToList();

            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Vendor> SearchVendors(string dataaraeid, string searchTerm)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.Vendors.Where(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower()
                         && x.vendor_code.ToLower().StartsWith(searchTerm.Trim().ToLower())
                         || x.vendor_ename.ToLower().StartsWith(searchTerm.Trim().ToLower())
                         ).OrderBy(x => x.vendor_code).Take(30).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TBLsupplier> GetTblRetailSupplier(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, out List<Vendor> AxVendors,string company)
        {
            IQueryable<TBLsupplier> query;
            var glCodes = new ObservableCollection<string>();
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TBLsuppliers.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TBLsuppliers.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TBLsuppliers.Count();
                    query = context.TBLsuppliers.OrderBy(sort).Skip(skip).Take(take);
                }
                foreach (var variable in query.Select(x => x.glcode).ToList())
                {
                    glCodes.Add(variable);
                }

                using (var model = new WorkFlowManagerDBEntities())
                {
                    AxVendors = model.Vendors.Where(x => glCodes.Contains(x.vendor_code) && x.DATAAREAID == "ccr").ToList();
                }

                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TBLsupplier> GetTblSupplier(string searchTerm)
        {
            using (var context = new ccnewEntities())
            {
                return context.TBLsuppliers.Where(x =>
                             x.Code.ToLower().StartsWith(searchTerm.Trim().ToLower()) || x.Ename.ToLower().StartsWith(searchTerm.Trim().ToLower()) || x.Aname.ToLower().StartsWith(searchTerm.Trim().ToLower())).Take(30).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStore> SearchRetailStores(string searchTerm)
        {
            using (var context = new ccnewEntities())
            {
                return context.TblStores.Where(x =>
                             x.code.ToLower().StartsWith(searchTerm.Trim().ToLower()) || x.ENAME.ToLower().StartsWith(searchTerm.Trim().ToLower()) || x.aname.ToLower().StartsWith(searchTerm.Trim().ToLower())).Take(30).ToList();
            }
        }

        [OperationContract]
        public List<VENDGROUP> GetVendorGroups()
        {
            using (var context = new ax2009_ccEntities())
            {
                return context.VENDGROUPs.Where(x => x.DATAAREAID == "Ccm").ToList();
            }
        }
    }
}