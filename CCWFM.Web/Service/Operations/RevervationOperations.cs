//using System.Collections.Generic;
//using System.Data.Objects;
//using System.Linq;
//using System.Net;
//using System.ServiceModel;
//using CCWFM.Web.Model;
//using Microsoft.Dynamics.BusinessConnectorNet;

//namespace CCWFM.Web.Service
//{
//    public partial class CRUD_ManagerService
//    {
//        [OperationContract]
//        public List<Tbl_ReservationMainDetails> GetReservationMainDetails(int resHeader)
//        {
//            using (var entities = new WorkFlowManagerDBEntities())
//            {
//                var fabricinspection = (from i in entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails")
//                                        where i.Tbl_ReservationHeader == resHeader
//                                        select i).ToList();
//                return fabricinspection.ToList();
//            }
//        }

//        [OperationContract]
//        public List<Tbl_ReservationMainDetails> SaveReservation(Tbl_ReservationHeader reservationHeader, List<Tbl_ReservationMainDetails> resMainDetails)
//        {
//            using (var entities = new WorkFlowManagerDBEntities())
//            {
//                if (reservationHeader.Iserial == 0)
//                {
//                    reservationHeader.IsPosted = false;
//                    reservationHeader.DocNo = "TEST";
//                    entities.AddObject("Tbl_ReservationHeader", reservationHeader);
//                    entities.SaveChanges();
//                }
//                //else
//                //{
//                //    var headerOld = (from h in entities.Tbl_ReservationHeader
//                //                     where (h.Iserial == reservationHeader.Iserial)
//                //                     select h).SingleOrDefault();

//                //    //entities.ObjectStateManager.
//                //}
//                foreach (var item in resMainDetails)
//                {
//                    if (item.Iserial != 0)
//                    {
//                        foreach (var items in item.Tbl_ReservationDetails.ToList())
//                        {
//                            var reservationDetails = (from d in entities.Tbl_ReservationDetails
//                                                      where d.Iserial == items.Iserial
//                                                      select d).SingleOrDefault();
//                            if (reservationDetails != null)
//                            {
//                                items.AxPicklingListJournal = PickingListFromReservation(items, reservationHeader);
//                                GenericUpdate(reservationDetails, items, entities);
//                            }
//                            else
//                            {                            
//                                items.Tbl_ReservationMainDetails = item.Iserial;
//                                entities.AddObject("Tbl_ReservationDetails", items);
//                                items.AxPicklingListJournal = PickingListFromReservation(items, reservationHeader);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        item.Tbl_ReservationHeader = reservationHeader.Iserial;
//                        entities.AddObject("Tbl_ReservationMainDetails", item);
//                    }                  
//                }

//                entities.SaveChanges();
//                return resMainDetails;
//            }
//        }

//        private void GenericUpdate<T>(T oldValues, T newValues, ObjectContext entities)
//        {
//            var orgRow = entities.ObjectStateManager.GetObjectStateEntry(oldValues);
//            orgRow.ApplyCurrentValues(newValues);
//          //  var modifiedProperties = orgRow.GetModifiedProperties();
//            entities.SaveChanges();
//        }

//        [OperationContract]
//        private List<Tbl_ReservationHeader> GetReservationHeaderList(Tbl_ReservationHeader header)
//        {
//            using (var entities = new WorkFlowManagerDBEntities())
//            {
//                var headerList = (from d in entities.Tbl_ReservationHeader
//                                  where (d.TransOrder == header.TransOrder || header.TransOrder == "")
//                                           && (d.DocDate == header.DocDate || header.DocDate == null)
//                                  select d).ToList();
//                return headerList;
//            }
//        }

//        [OperationContract]
//        [TransactionFlow(TransactionFlowOption.Allowed)]
//        public List<SalesOrderDto> GetPurchaseOrderSalesOrders(string season, string brand, string style, PurchaseOrderDetailDto searchedItems)
//        {
//            using (var workFlowEntities = new WorkFlowManagerDBEntities())
//            {
//                workFlowEntities.BOMs.MergeOption = MergeOption.NoTracking;
//                workFlowEntities.BOM_StyleColorInfo.MergeOption = MergeOption.NoTracking;
//                workFlowEntities.StyleHeader_SalesOrder_Colors.MergeOption = MergeOption.NoTracking;
//                workFlowEntities.StyleHeader_SalesOrder.MergeOption = MergeOption.NoTracking;
//                workFlowEntities.BOMs.MergeOption = MergeOption.NoTracking;

//                var salesOrders = (from bom in workFlowEntities.BOMs
//                                   join bs in workFlowEntities.BOM_StyleColorInfo
//                                   on bom.BOM_ID equals bs.BOM_StyleColorInfo_BOMID
//                                   join styleColor in workFlowEntities.StyleHeader_SalesOrder_Colors
//                                   on bs.BOM_StyleColorInfo_StyleColor equals styleColor.StyleHeader_Colors_ID
//                                   join styleSalesorder in workFlowEntities.StyleHeader_SalesOrder
//                                   on styleColor.StyleHeader_Colors_SalesOrder equals styleSalesorder.SalesOrderID
//                                   where bom.BOM_Fabric == searchedItems.ItemId && bs.BOM_StyleColorInfo_FabricColor == searchedItems.FabricColor
//                            && (styleSalesorder.SalesOrder_Season == season || season == null)
//                            && (styleSalesorder.SalesOrder_Brand == brand || brand == null)
//                            && (styleSalesorder.StyleHeader == style || style == null)
//                            && styleSalesorder.SalesOrderStatus == 5
//                                   select new SalesOrderDto
//                                   {
//                                       SalesOrder = styleColor.StyleHeader_Colors_SalesOrder,
//                                       SalesOrderColor = styleColor.StyleHeader_Colors_StyleColor,
//                                       IntialQty = styleColor.StyleHeader_Colors_Total
//                                   }
//                    );
//                return salesOrders.ToList();
//            }
//        }

//        public string PickingListFromReservation(Tbl_ReservationDetails item, Tbl_ReservationHeader header)
//        {
//            var axapta = new Axapta();
//            try
//            {
//                const string tableName = "AutoPICKING";
//                var credential = new NetworkCredential("bcproxy", "around1");
//                axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);
//                PURCHTABLE purchaseOrder;
//                using (var axEntity = new ax2009_ccEntities())
//                {
//                    purchaseOrder = axEntity.PURCHTABLEs.SingleOrDefault(x => x.PURCHID == header.TransOrder);
//                }

//                using (var entities = new WorkFlowManagerDBEntities())
//                {
//                    var styleHeaderSalesOrder =
//                        entities.StyleHeader_SalesOrder.SingleOrDefault(x => x.SalesOrderID == item.SalesOrder);
//                    if (styleHeaderSalesOrder != null)
//                    {
//                        var style = styleHeaderSalesOrder.StyleHeader;

//                        var axaptaRecord = axapta.CreateAxaptaRecord(tableName);

//                        axaptaRecord.Clear();
//                        axaptaRecord.InitValue();
//                        axaptaRecord.set_Field("DATAAREAID", "CCM");
//                        axaptaRecord.set_Field("SALESORDER", item.SalesOrder);
//                        axaptaRecord.set_Field("FABRICID", item.Tbl_ReservationMainDetails1.Fabric);
//                        axaptaRecord.set_Field("FABRIC_COLOR", item.Tbl_ReservationMainDetails1.FabricColor);
//                        axaptaRecord.set_Field("STYLEID", style);
//                        axaptaRecord.set_Field("STYLECOLOR", item.SalesOrderColor);
//                        axaptaRecord.set_Field("FABRICSITEID", item.Tbl_ReservationMainDetails1.Site);
//                        axaptaRecord.set_Field("FABRICLOCATION", item.Tbl_ReservationMainDetails1.Location);
//                        axaptaRecord.set_Field("FABRICWAREHOUSES", item.Tbl_ReservationMainDetails1.Warehouse);
//                        axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Tbl_ReservationMainDetails1.Batchno);
//                        axaptaRecord.set_Field("TRANSDATE", header.DocDate);
//                        axaptaRecord.set_Field("QTY", item.FinalQty > 0 ? item.FinalQty : item.IntialQty);
//                        axaptaRecord.set_Field("VENDOR", purchaseOrder.ORDERACCOUNT);
//                        axaptaRecord.set_Field("WORKFLOWJOURID", item.Iserial);
//                        axaptaRecord.Insert();
//                    }

//                    var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

//                    if (item.AxPicklingListJournal != null)
//                    {
//                        importNew.Call("deletejournal", 0, item.AxPicklingListJournal, 0);
//                    }
//                    var retval = importNew.Call("CreatePicking", item.Iserial, 0, 0);

//                    ClearAxTable(tableName, axapta);
//                    return (string)retval;
//                }
//            }
//            finally
//            {
//                axapta.Logoff();
//            }
//        }
//    }
//}