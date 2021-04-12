using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [DataContract]
        public class AxItem
        {
            [DataMember]
            public string ItemId { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string ItemGroup { get; set; }

            [DataMember]
            public string InventoryUnit { get; set; }
        }

        [OperationContract]
        public List<AxItem> SearchedAxItems(string dataArea, string itemGroup, string itemId, string itemDesc)
        {
            using (var context = new ax2009_ccEntities())
            {
                context.INVENTTABLEs.MergeOption = System.Data.Objects.MergeOption.NoTracking;

                var searchedItems = (from i in context.INVENTTABLEMODULEs

                                     join x in context.INVENTTABLEs
                                       on new { i.ITEMID, i.DATAAREAID } equals new { x.ITEMID, x.DATAAREAID }
                                     where x.DATAAREAID == dataArea && x.ITEMGROUPID.Contains(itemGroup) &&
                                     x.ITEMID.Contains(itemId) && i.MODULETYPE == 0
                                     && x.ITEMNAME.Contains(itemDesc)
                                     select new AxItem()
                                         {
                                             ItemId = x.ITEMID,
                                             ItemGroup = x.ITEMGROUPID,
                                             Description = x.ITEMNAME,
                                             InventoryUnit = i.UNITID,
                                         }).ToList();

                return searchedItems;
            }
        }

        [OperationContract]
        public List<INVENTDIM> SearchedAxItemsDetails(string dataArea, string itemId)
        {
            using (var context = new ax2009_ccEntities())
            {
                context.INVENTSUMs.MergeOption = System.Data.Objects.MergeOption.NoTracking;

                context.INVENTDIMs.MergeOption = System.Data.Objects.MergeOption.NoTracking;

                var searchedItems = context.INVENTSUMs.Where(x => x.DATAAREAID == dataArea
                    && x.ITEMID == itemId).Select(x => x.INVENTDIMID);

                var details = context.INVENTDIMs.Where(x => x.DATAAREAID == dataArea
                    && searchedItems.Any(s => s == x.INVENTDIMID)).ToList();

                return details;
            }
        }
    }
}