using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.ServiceModel;
using _Model = CCWFM.Web.Model;

// ReSharper disable once CheckNamespace
namespace CCWFM.Web.Service
{
    // ReSharper disable once InconsistentNaming
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PurchaseOrderDto> GetPurchaseOrderJournals(int skip, int take, string dataArea, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, int type = 0)
        {
            if (valuesObjects == null)
            {
                valuesObjects = new Dictionary<string, object>();
            }
            if (SharedOperation.UseAx())
            {
                if (type != 1)
                {
                    if (filter == null)
                    {
                        filter = "it.DATAAREAID ==(@DATAAREAID0)";
                    }
                    else
                    {
                        filter = filter + " and it.DATAAREAID ==(@DATAAREAID0)";
                    }

                    valuesObjects.Add("DATAAREAID0", dataArea);
                }
            }


            var parameterCollection = ConvertToParamters(valuesObjects);
            var searchedItems = new List<PurchaseOrderDto>();
            fullCount = 0;
            if (SharedOperation.UseAx())
            {

                using (var context = new _Model.ax2009_ccEntities())
                {
                    if (type == 0)
                    {


                        fullCount = context.PURCHTABLEs.Where(filter, parameterCollection.ToArray()).Count();
                        searchedItems = (from p in context.PURCHTABLEs.Where(filter, parameterCollection.ToArray()).OrderBy(p => p.DATAAREAID).Skip(skip).Take(take)
                                         select new PurchaseOrderDto
                                         {
                                             VendorCode = p.ORDERACCOUNT,
                                             VendorName = p.PURCHNAME,
                                             CreatedDate = p.CREATEDDATETIME,
                                             DataArea = p.DATAAREAID,
                                             JournalId = p.PURCHID,
                                             Status = p.PURCHSTATUS,
                                         }).OrderByDescending(x => x.CreatedDate).Take(30).ToList();
                    }

                    if (type == 2)
                    {
                        fullCount = context.INVENTJOURNALTABLEs.Where(filter, parameterCollection.ToArray()).Count();
                        searchedItems = (from p in context.INVENTJOURNALTABLEs
                                             .Where(filter, parameterCollection.ToArray()).OrderBy(p => p.DATAAREAID).Skip(skip).Take(take)
                                         select new PurchaseOrderDto
                                         {
                                             VendorCode = "",
                                             VendorName = "",
                                             DataArea = p.DATAAREAID,
                                             JournalId = p.JOURNALID,
                                         }).OrderByDescending(x => x.DataArea).Take(30).ToList();
                    }
                }
            }
            else {

                using (var context = new _Model.WorkFlowManagerDBEntities())
                {
                    if (type == 0)
                    {
                        if (filter == null)
                        { 
                        fullCount = context.TblPurchaseOrderHeaderRequests.Count();
                        searchedItems = (from p in context.TblPurchaseOrderHeaderRequests.OrderByDescending(w=>w.Iserial).Skip(skip).Take(take)
                                         select new PurchaseOrderDto
                                         {
                                             VendorCode = p.Vendor,
                                             VendorName = p.Vendor,
                                             CreatedDate = p.CreationDate?? DateTime.Now,
                                             DataArea = "ccm",
                                             JournalId = p.Code,
                                             Status = 1,
                                         }).OrderByDescending(x => x.CreatedDate).Take(30).ToList();
                        }
                        else{
                            fullCount = context.TblPurchaseOrderHeaderRequests.Where(filter, parameterCollection.ToArray()).Count();
                            searchedItems = (from p in context.TblPurchaseOrderHeaderRequests.Where(filter, parameterCollection.ToArray()).OrderByDescending(w => w.Iserial).Skip(skip).Take(take)
                                             select new PurchaseOrderDto
                                             {
                                                 VendorCode = p.Vendor,
                                                 VendorName = p.Vendor,
                                                 CreatedDate = p.CreationDate ?? DateTime.Now,
                                                 DataArea = "ccm",
                                                 JournalId = p.Code,
                                                 Status = 1,
                                             }).OrderByDescending(x => x.CreatedDate).Take(30).ToList();
                        }
                    }

                    //if (type == 2)
                    //{
                    //    fullCount = context.INVENTJOURNALTABLEs.Where(filter, parameterCollection.ToArray()).Count();
                    //    searchedItems = (from p in context.INVENTJOURNALTABLEs
                    //                         .Where(filter, parameterCollection.ToArray()).OrderBy(p => p.DATAAREAID).Skip(skip).Take(take)
                    //                     select new PurchaseOrderDto
                    //                     {
                    //                         VendorCode = "",
                    //                         VendorName = "",
                    //                         DataArea = p.DATAAREAID,
                    //                         JournalId = p.JOURNALID,
                    //                     }).OrderByDescending(x => x.DataArea).Take(30).ToList();
                    //}
                }

            }

            if (type == 1)
            {
                using (var context = new _Model.WorkFlowManagerDBEntities())
                {
                    if (filter == null)
                    {
                        fullCount = context.TblDyeingOrdersHeaders.Count();
                        searchedItems = (from p in context.TblDyeingOrdersHeaders.OrderByDescending(p => p.DocPlan).Skip(skip).Take(take).Where(w => w.TblDyeingOrdersMainDetails.Any(r => r.TransactionType == 0))
                                         select new PurchaseOrderDto
                                         {
                                             VendorCode = p.Vendor,
                                             VendorName = p.Vendor,
                                             DataArea = "ccm",
                                             JournalId = p.DocPlan,
                                         }).Take(30).ToList();
                    }
                    else
                    {
                        fullCount = context.TblDyeingOrdersHeaders.Where(filter, parameterCollection.ToArray()).Count();
                        searchedItems = (from p in context.TblDyeingOrdersHeaders
                            .Where(filter, parameterCollection.ToArray()).GroupBy(x => new { x.DocPlan, x.Vendor }).OrderByDescending(p => p.Key.DocPlan).Skip(skip).Take(take)
                                         select new PurchaseOrderDto
                                         {
                                             VendorCode = p.Key.Vendor,
                                             VendorName = p.Key.Vendor,
                                             DataArea = "ccm",
                                             JournalId = p.Key.DocPlan,
                                         }).Take(30).ToList();
                    }
                }
            }

            return searchedItems;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PurchaseOrderDetailDto> GetPurchaseOrderLines(int skip, int take, string dataArea, string journal, IEnumerable<decimal> lineNumbers, string sort, string filter, Dictionary<string, object> valuesObjects)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (valuesObjects == null)
                {
                    valuesObjects = new Dictionary<string, object>();
                }

                if (filter == null)
                {
                    filter = "it.PURCHID ==(@PURCHID0)";
                }
                else
                {
                    filter = filter + " and it.PURCHID ==(@PURCHID0)";
                }

                valuesObjects.Add("PURCHID0", journal);
                //filter = filter + " and it.DATAAREAID ==(@DATAAREAID0)";
                //valuesObjects.Add("DATAAREAID0", dataArea);
                var parameterCollection = ConvertToParamters(valuesObjects);
                var querytemp = context.PurchlineInventDims.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
               var  query = querytemp.Select(p => new PurchaseOrderDetailDto
                {
                    ItemName = p.NAME,
                    ItemId = p.ITEMID,
                    Unit = p.PURCHUNIT,
                    FabricColor = p.CONFIGID,
                    BatchNo = p.INVENTBATCHID,
                    Quantity = (float)p.PURCHQTY,
                    UnitPrice = (float)p.PRICEUNIT,
                    TotalPrice = (float)p.LINEAMOUNT,
                    LineNumber = p.LINENUM,
                    Location = p.WMSLOCATIONID,
                    Warehouse = p.INVENTLOCATIONID,
                    Site = p.INVENTSITEID,
                });

                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PurchaseOrderDetailDto> GetTransferInventDimLines(int skip, int take, string dataArea, string journal, IEnumerable<decimal> lineNumbers, string sort, string filter, Dictionary<string, object> valuesObjects)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                // if (filter != null)
                //{
                if (valuesObjects == null)
                {
                    valuesObjects = new Dictionary<string, object>();
                }

                if (filter == null)
                {
                    filter = "it.PURCHID ==(@PURCHID0)";
                }
                else
                {
                    filter = filter + " and it.PURCHID ==(@PURCHID0)";
                }

                valuesObjects.Add("PURCHID0", journal);
                filter = filter + " and it.DATAAREAID ==(@DATAAREAID0)";
                valuesObjects.Add("DATAAREAID0", dataArea);
                //int counter = 0;

                var parameterCollection = ConvertToParamters(valuesObjects);
                var querytemp = context.TransferInventDims.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                var query = querytemp.Select(p => new PurchaseOrderDetailDto
                {
                    ItemName = p.NAME,
                    ItemId = p.ITEMID,
                    Unit = p.PURCHUNIT,
                    FabricColor = p.CONFIGID,
                    BatchNo = p.INVENTBATCHID,
                    Quantity = (float)p.PURCHQTY,
                    UnitPrice = (float)p.PRICEUNIT,
                    TotalPrice = (float)p.LINEAMOUNT,
                    LineNumber = p.LINENUM,
                    Location = p.WMSLOCATIONID,
                    Warehouse = p.INVENTLOCATIONID,
                    Site = p.INVENTSITEID,
                });

                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PurchaseOrderDetailDto> GetRecivedDyedOrders(int skip, int take, string journal, IEnumerable<decimal> lineNumbers, string sort, string filter, Dictionary<string, object> valuesObjects)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (valuesObjects == null)
                {
                    valuesObjects = new Dictionary<string, object>();
                }

                if (filter == null)
                {
                    filter = "it.TblDyeingOrdersMainDetail.TblDyeingOrdersHeader.DocPlan ==(@PURCHID0)";
                }
                else
                {
                    filter = filter + " and it.TblDyeingOrdersMainDetail.TblDyeingOrdersHeader.DocPlan ==(@PURCHID0)";
                }

                filter = filter + " and it.TransactionType ==(@TransactionType0)";
                valuesObjects.Add("PURCHID0", journal);
                valuesObjects.Add("TransactionType0", 1);               
                var parameterCollection = ConvertToParamters(valuesObjects);
                var querytemp = context.TblDyeingOrdersDetails.Include("TblDyeingOrdersMainDetail").Where(filter, parameterCollection.ToArray());
                var query = querytemp.Select(p => new PurchaseOrderDetailDto
                {
                    ItemName = p.DyedFabric,
                    ItemId = p.DyedFabric,
                    Unit = p.Unit,
                    FabricColor = p.Color,
                    BatchNo = SqlFunctions.StringConvert((double)p.BatchNo).Trim(),
                    Quantity = (float)p.CalculatedTotalQty,
                    UnitPrice = (float)0,
                    TotalPrice = (float)0,
                    LineNumber = p.Iserial,
                    Location = p.TblDyeingOrdersMainDetail.WareHouse,
                    Warehouse = p.TblDyeingOrdersMainDetail.WareHouse,
                    Site = context.V_Warehouse.FirstOrDefault(w => w.WarehouseID == p.TblDyeingOrdersMainDetail.WareHouse).SiteId,
                });

                return query.ToList();
            }
        }
    }
}