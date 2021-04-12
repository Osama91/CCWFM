using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Dto_s;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public WMSLOCATION GetWmsLocation(string warehouse, string location, string vendor)
        {
            using (var context = new ax2009_ccEntities())
            {
                try
                {
                    return
                        context.WMSLOCATIONs.SingleOrDefault(x => x.INVENTLOCATIONID == warehouse && (x.WMSLOCATIONID == location && location != null)
                                                                  && (x.VENDID == vendor && vendor != null));
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
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

        //DyeingPlan
        //Marker

        [OperationContract]
        public List<WareHouseDto> GetWarehouseswithOnHand()
        {
            using (var entities = new ax2009_ccEntities())
            {
                var query = from invents in entities.INVENTSUMs
                            join dim in entities.INVENTDIMs on new { invents.INVENTDIMID, invents.DATAAREAID }
                                equals new { dim.INVENTDIMID, dim.DATAAREAID }
                            join loc in entities.INVENTLOCATIONs on new { dim.INVENTLOCATIONID, dim.DATAAREAID }
                                equals new { loc.INVENTLOCATIONID, loc.DATAAREAID }
                            where invents.DATAAREAID == "CCm" && invents.AVAILPHYSICAL > 0
                            select new WareHouseDto
                    {
                        WareHouseCode = loc.INVENTLOCATIONID,
                        WareHouseName = loc.NAME,
                    };
                return query.Distinct().ToList();
            }
        }

        //DyeingPlan
        [OperationContract]
        public List<FabricStorage_Result> GetLocationDetails(string location)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.FabricStorage(location).ToList();
            }
        }
    }
}