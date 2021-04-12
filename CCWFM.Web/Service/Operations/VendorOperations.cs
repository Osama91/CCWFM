using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

using _Model = CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        /// <summary>
        /// Pass null to all parameters to get all vendors
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Vendor> GetVendors(int? skip, int? take, string dataaraeid)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (skip != null && take != null)
                    return context.Vendors.Where(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower()).OrderBy(x => x.vendor_code).Skip((int)skip).Take((int)take).ToList();
                else
                    return context.Vendors.Where(x => x.DATAAREAID.ToLower() == dataaraeid.ToLower()).OrderBy(x => x.vendor_code).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<Warehouse> GetWareHouses()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.WareHouses.ToList();
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<V_Warehouse> GetAllWarehousesByCompanyName(string company)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.V_Warehouse.Where(x => x.DataAreaID.ToLower() == company.ToLower()).ToList();
            }
        }

    }
}