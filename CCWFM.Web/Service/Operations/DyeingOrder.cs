using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblDyeingOrdersMainDetail> GetDyeingOrdersMainDetails(int dyeingProductionOrder, out Dictionary<string, double?> ColorPrices)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var dyeingOrdersMainDetail = (from s in entities.TblDyeingOrdersMainDetails.Include("TblDyeingOrdersDetails")
                                              where s.DyeingProductionOrder == dyeingProductionOrder
                                              select s).ToList();
                var temp = dyeingOrdersMainDetail.Select(w => w.TblDyeingOrdersDetails).ToList();
                var listofColors = new List<string>();
                foreach (var VARIABLE in temp)
                {
                    listofColors.AddRange(VARIABLE.Select(w => w.Color));
                }
                ColorPrices = entities.TblColors.Where(w => listofColors.Contains(w.Code)).GroupBy(x => x.Code).ToDictionary(t => t.Key, t => t.FirstOrDefault().DefaultPrice);
                return dyeingOrdersMainDetail;
            }
        }

        [OperationContract]
        public List<TblDyeingOrdersHeader> DyeingOrderHeaderSearchList(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingOrdersHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblDyeingOrdersHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblDyeingOrdersHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblDyeingOrdersHeaders.Count();
                    query = context.TblDyeingOrdersHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<DyeingOrderDetailsService> GetSavedDyeingOrderServices(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.DyeingOrderDetailsServices.Where(x => x.DyeingOrdersDetailsInt == iserial).ToList();
            }
        }

        [OperationContract]
        public List<TblDyeingOrdersMainDetail> SaveDyeingOrder(TblDyeingOrdersHeader header, List<TblDyeingOrdersMainDetail> Maindetails)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (header.DyeingProductionOrder != 0)
                {
                    var h = entities.TblDyeingOrdersHeaders.SingleOrDefault(x => x.DyeingProductionOrder == header.DyeingProductionOrder);

                    if (h != null)
                    {
                        h.DocPlan = header.DocPlan;
                        h.TransactionDate = header.TransactionDate;
                        h.Vendor = header.Vendor;
                    }
                }
                else
                {
                    entities.AddToTblDyeingOrdersHeaders(header);
                }
                entities.SaveChanges();

                foreach (var item in Maindetails)
                {
                    if (item.DyeingProductionOrder == 0)
                    {
                        item.DyeingProductionOrder = header.DyeingProductionOrder;
                        entities.AddToTblDyeingOrdersMainDetails(item);
                    }
                    else
                    {
                        var mainDetailRow = (from d in entities.TblDyeingOrdersMainDetails
                                             where d.TransId == item.TransId && d.TransactionType == item.TransactionType
                                             && d.DyeingProductionOrder == item.DyeingProductionOrder
                                             select d).SingleOrDefault();

                        if (mainDetailRow != null)
                        {
                            GenericUpdate(mainDetailRow, item, entities);
                        }
                        else
                        {
                            entities.AddToTblDyeingOrdersMainDetails(item);
                        }
                    }
                    entities.SaveChanges();

                    if (item.DyeingProductionOrder != 0)
                    {
                        foreach (var items in item.TblDyeingOrdersDetails.ToList())
                        {
                            var dyeingOrderDetails = (from d in entities.TblDyeingOrdersDetails
                                                      where d.Iserial == items.Iserial && d.TransactionType == item.TransactionType
                                                      select d).SingleOrDefault();
                            if (dyeingOrderDetails != null)
                            {
                                GenericUpdate(dyeingOrderDetails, items, entities);
                            }
                            else
                            {
                                items.TblDyeingOrdersMainDetail = null;
                                entities.AddToTblDyeingOrdersDetails(items);
                            }
                            entities.SaveChanges();
                        }
                    }
                }
            }
            return Maindetails;
        }

        [OperationContract]
        public void DeleteDyeingOrder(int dyeingHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var detailsList = entities.TblDyeingOrdersDetails.Include("DyeingOrderDetailsServices").Where(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                foreach (var item in detailsList)
                {
                    foreach (var service in item.DyeingOrderDetailsServices.ToList())
                    {
                        entities.DeleteObject(service);
                    }

                    entities.DeleteObject(item);
                }

                var mainDetailsList = entities.TblDyeingOrdersMainDetails.Where(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                foreach (var item in mainDetailsList)
                {
                    entities.DeleteObject(item);
                }

                var header = entities.TblDyeingOrdersHeaders.SingleOrDefault(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                entities.DeleteObject(header);

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingMainDetails(TblDyeingOrdersMainDetail maindetails)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var detailsList = entities.TblDyeingOrdersDetails.Include("DyeingOrderDetailsServices").Where(x => x.DyeingProductionOrder == maindetails.DyeingProductionOrder
                    && x.TransactionType == maindetails.TransactionType && x.TransId == maindetails.TransId);

                foreach (var item in detailsList)
                {
                    foreach (var service in item.DyeingOrderDetailsServices.ToList())
                    {
                        entities.DeleteObject(service);
                    }

                    entities.DeleteObject(item);
                }

                var mainDet = entities.TblDyeingOrdersMainDetails.SingleOrDefault(x => x.DyeingProductionOrder == maindetails.DyeingProductionOrder
                                                                                       && x.TransactionType == maindetails.TransactionType && x.TransId == maindetails.TransId);

                entities.DeleteObject(mainDet);

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingDetails(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var details = entities.TblDyeingOrdersDetails.Include("DyeingOrderDetailsServices").SingleOrDefault(x => x.Iserial == iserial);

                if (details != null)
                {
                    foreach (var service in details.DyeingOrderDetailsServices.ToList())
                    {
                        entities.DeleteObject(service);
                    }
                    entities.DeleteObject(details);
                }

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void ProducationConnection(int dyeingProductionOrder, int transId, int transactionType, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var header = entities.TblDyeingOrdersHeaders.FirstOrDefault(x => x.DyeingProductionOrder == dyeingProductionOrder);
                var mainDetail = entities.TblDyeingOrdersMainDetails.Include("TblDyeingOrdersDetails.DyeingOrderDetailsServices").FirstOrDefault(x => x.DyeingProductionOrder == dyeingProductionOrder
                                                                                          && x.TransId == transId && x.TransactionType == transactionType);
                var locationLoc = entities.GetLocations.FirstOrDefault(x => x.INVENTLOCATIONID == mainDetail.WareHouse);
                var vendorWmsLocation = entities.GetWmsLocations.FirstOrDefault(x => x.VENDID == header.Vendor);
                var vendorLoc = entities.GetLocations.FirstOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");

                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);
                }
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                var tableName = "PRODCONNECTION";
                var transactionGuid = Guid.NewGuid().ToString();
                foreach (var item in mainDetail.TblDyeingOrdersDetails)
                {
                    try
                    {
                        AxaptaRecord AxaptaRecord;
                        #region TransactionType0  Transfer To Vendor's Location And ProductionOrder And PickingList

                        AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        AxaptaRecord.Clear();
                        AxaptaRecord.InitValue();

                        if (item.TransactionType == 0)
                        {
                            //Transfer To Vendor's Location

                            var lotmaster = entities.TblDyeingSummaries.Include("TblDyeingPlanLotsMasters").FirstOrDefault(
                                  x =>
                                      x.FabricCode == item.FabricCode && x.BatchNo == item.BatchNo &&
                                      item.Color == x.Color).TblDyeingPlanLotsMasters.FirstOrDefault();
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.FabricCode);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMWAREHOUSE", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMBATCH", lotmaster.FabricLot);
                            AxaptaRecord.set_Field("FROMCONFIG", lotmaster.FromColor);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOBATCH", lotmaster.FabricLot);
                            AxaptaRecord.set_Field("TOCONFIG", lotmaster.FromColor);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();

                            //  //   ProductionOrder

                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();

                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.FabricCode);
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(1));
                            AxaptaRecord.set_Field("FROMCONFIG", lotmaster.FromColor);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();

                            //////     PickingList
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.FabricCode);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMWAREHOUSE", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMBATCH", lotmaster.FabricLot);
                            AxaptaRecord.set_Field("FROMCONFIG", lotmaster.FromColor);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(2));
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.Insert();
                        }

                        #endregion TransactionType0  Transfer To Vendor's Location And ProductionOrder And PickingList

                        #region TransactionType1 ReportAsFinished

                        else if (item.TransactionType == 1)
                        {
                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();

                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.DyedFabric);
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", locationLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(3));
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TOWAREHOUSE", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);

                            AxaptaRecord.Insert();
                        }

                        #endregion TransactionType1 ReportAsFinished
                    }
                    catch (Exception ex)
                    {
                    }
                }

                mainDetail.Posted = true;
                entities.SaveChanges();
                var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                if (mainDetail.TransactionDate == null)
                {
                    mainDetail.TransactionDate = DateTime.UtcNow;
                }
                try
                {
                    if (transactionType == 0)
                    {
                        var Production = import.Call("run", dyeingProductionOrder, transId, 1, "", 0, mainDetail.TransactionDate.Value.ToUniversalTime());
                        var Transfer = import.Call("run", dyeingProductionOrder, transId, 0, "Name", 1, mainDetail.TransactionDate.Value.ToUniversalTime());
                        var PickingList = import.Call("run", dyeingProductionOrder, transId, 2, "Name", 1, mainDetail.TransactionDate.Value.ToUniversalTime());
                    }
                    else if (transactionType == 1)
                    {
                        PurchaseDyeingServicesToAx(header, mainDetail, 1, userIserial, transactionGuid);
                        var ReportAsFinished = import.Call("run", dyeingProductionOrder, transId, 3, "Name", 1, mainDetail.TransactionDate.Value.ToUniversalTime());
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                //         ClearAxTable("PRODCONNECTION", axapta, transactionGuid);
                axapta.Logoff();
            }
        }
        private string GenerateNewPurchase(string Purchaseid)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var Exist = context.FabricPurchaseOrders.Any(x => x.PURCHID == Purchaseid);

                if (Exist)
                {
                    GenerateNewPurchase(Purchaseid + "1");
                }
                else
                {
                    return Purchaseid;
                }
                return Purchaseid;
            }
        }

        public void PurchaseDyeingServicesToAx(TblDyeingOrdersHeader objToPost, TblDyeingOrdersMainDetail headerObjToPost, int postPostOrNo, int userIserial, string transactionGuid)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                using (var axapta = new Axapta())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");

                    TblAuthUser userToLogin = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);

                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

                    var inventTable = axapta.CreateAxaptaRecord("InventDim");

                    try
                    {
                        var vendorWmsLocation = context.GetWmsLocations.FirstOrDefault(x => x.VENDID == objToPost.Vendor);

                        var vendorLoc =
                            context.GetLocations.FirstOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                        var purchId = "Rc_ " + objToPost.DocPlan.ToString() + objToPost.DyeingProductionOrder + headerObjToPost.TransId.ToString() + headerObjToPost.TransactionType;
                        var tableName = "PurchTable";
                        purchId = GenerateNewPurchase(purchId);

                        var purchTableRecord = axapta.CreateAxaptaRecord(tableName);
                        purchTableRecord.Clear();
                        purchTableRecord.InitValue();

                        purchTableRecord.set_Field("PurchId", purchId);
                        purchTableRecord.set_Field("DeliveryDate", headerObjToPost.TblDyeingOrdersDetails.OrderByDescending(x => x.EstimatedDeliveryDate).FirstOrDefault().EstimatedDeliveryDate ?? DateTime.Now);

                        var headerax = axapta.CallStaticRecordMethod("VendTable", "find", objToPost.Vendor) as AxaptaRecord;
                        purchTableRecord.Call("initFromVendTable", headerax);

                        purchTableRecord.Insert();

                        foreach (var item in headerObjToPost.TblDyeingOrdersDetails)
                        {
                            tableName = "PurchLine";
                            foreach (var servicerow in item.DyeingOrderDetailsServices)
                            {
                                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                                axaptaRecord.Clear();
                                axaptaRecord.InitValue();

                                inventTable.Clear();
                                inventTable.set_Field("InventLocationId", vendorLoc.INVENTLOCATIONID);
                                inventTable.set_Field("wMSLocationId", vendorWmsLocation.WMSLOCATIONID);
                                if (item.Color != null) inventTable.set_Field("InventColorId", item.Color);

                                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                                var producationOrder = "Free";
                                using (var model = new ax2009_ccEntities())
                                {
                                    var firstOrDefault = model.PRODCONNECTIONs.FirstOrDefault(
                                        x =>
                                            x.DYEDITEM == item.DyedFabric && x.TRANSID == item.DyeingProductionOrder
                                           && x.TOCONFIG == item.Color && x.PRODID != "");
                                    if (firstOrDefault != null)
                                        producationOrder =
                                            firstOrDefault.PRODID;
                                }
                                if (producationOrder == null || (string)producationOrder == "")
                                {
                                    producationOrder = "Free";
                                }

                                importNew.Call("CreateConfig", servicerow.ServiceCode, item.Color);
                                importNew.Call("CreateBatch", servicerow.ServiceCode, producationOrder);
                                inventTable.set_Field("configId", item.Color);
                                inventTable.set_Field("inventBatchId", producationOrder);
                                inventTable.set_Field("INVENTSITEID", vendorLoc.INVENTSITEID);
                                inventTable = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventTable) as AxaptaRecord;

                                if (inventTable != null)
                                {
                                    var tempx = inventTable.get_Field("inventDimId").ToString();
                                    axaptaRecord.set_Field("InventDimId", tempx);
                                }
                                axaptaRecord.set_Field("PurchUnit", "Kg");

                                axaptaRecord.set_Field("ItemId", servicerow.ServiceCode);
                                axaptaRecord.set_Field("purchId", purchId);
                                axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(item.CalculatedTotalQty.ToString()));
                                decimal price = 1;
                                price = (decimal)servicerow.Qty;
                                axaptaRecord.set_Field("PurchPrice", Convert.ToDecimal(price));
                                axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(item.CalculatedTotalQty.ToString()));
                                axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(price * (decimal)item.CalculatedTotalQty));
                                axaptaRecord.Call("createLine", true, true, false, true, true, false);
                            }
                        }
                        if (postPostOrNo == 1)
                        {
                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                            importNew.Call("PostPurchaseOrder", purchId, objToPost.TransactionDate ?? DateTime.Now);
                            PickingListForAxServices(objToPost, headerObjToPost, postPostOrNo, userIserial,
                                transactionGuid);
                        }
                    }

                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        axapta.Logoff();
                    }
                }
            }
        }

        private void PickingListForAxServices(TblDyeingOrdersHeader objToPost, TblDyeingOrdersMainDetail headerObjToPost,
            int postPostOrNo, int userIserial, string transactionGuid)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var axapta = new Axapta();
                AxaptaRecord AxaptaRecord;
                var credential = new NetworkCredential("bcproxy", "around1");
                var tableName = "PRODCONNECTION";
                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

                var vendorWmsLocation = entities.GetWmsLocations.SingleOrDefault(x => x.VENDID == objToPost.Vendor);

                var vendorLoc =
                    entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);

                try
                {
                    foreach (var item in headerObjToPost.TblDyeingOrdersDetails)
                    {
                        foreach (var service in item.DyeingOrderDetailsServices)
                        {
                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();
                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", service.ServiceCode);
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", "Kg");
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(6));
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("JOURNALLINKID", headerObjToPost.TransId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();
                        }
                    }
                    if (headerObjToPost.TransactionDate == null)
                    {
                        headerObjToPost.TransactionDate = DateTime.UtcNow;
                    }
                    //public ProdJournalId run(int transId,int journalId,int WhatToDo,str JourName,int PostorNo)
                    var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                    var PickingList = import.Call("run", headerObjToPost.DyeingProductionOrder, headerObjToPost.TransId, 6, "Name", postPostOrNo, headerObjToPost.TransactionDate.Value.ToUniversalTime());

                    axapta.Logoff();
                }

                catch (Exception)
                {
                    axapta.Logoff();
                    throw;
                }
            }
        }

        [OperationContract]
        public List<FabricInventSumWithBatch> FabricInventSumWithBatches(string wareHouse, string fabricCode, string company, string batch, string color, string size)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.FabricInventSumWithBatches.Where(x => (x.INVENTLOCATIONID == wareHouse || wareHouse == null) && (x.INVENTBATCHID == batch || batch == null) && (x.CONFIGID == color || color == null) && (x.INVENTSIZEID == size || size == null) && (x.FabricCode == fabricCode || fabricCode == null)
                    && x.DATAAREAID == company.ToLower()).OrderByDescending(w => w.AVAILPHYSICAL).ToList();
            }
        }

        [OperationContract]
        public void SaveDyeingOrderDetailsServices(int iserial, string serviceCode, string serviceName, string notes, bool Checked, double qty)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (Checked)
                {
                    var dyeingSummaryServices = entities.DyeingOrderDetailsServices.SingleOrDefault(x => x.DyeingOrdersDetailsInt == iserial && x.ServiceCode == serviceCode);

                    var summaryServiceNewRow = new DyeingOrderDetailsService();

                    if (dyeingSummaryServices == null)
                    {
                        summaryServiceNewRow.ServiceCode = serviceCode;
                        summaryServiceNewRow.ServiceName = serviceName;
                        summaryServiceNewRow.DyeingOrdersDetailsInt = iserial;
                        summaryServiceNewRow.Notes = notes;
                        summaryServiceNewRow.Qty = qty;
                        entities.AddToDyeingOrderDetailsServices(summaryServiceNewRow);
                    }
                    else
                    {
                        summaryServiceNewRow.Notes = notes;
                        summaryServiceNewRow.Qty = qty;
                    }
                }
                else
                {
                    var dyeingSummaryServices = entities.DyeingOrderDetailsServices.SingleOrDefault(x => x.DyeingOrdersDetailsInt == iserial && x.ServiceCode == serviceCode);
                    if (dyeingSummaryServices != null)
                    {
                        entities.DeleteObject(dyeingSummaryServices);
                    }
                }
                entities.SaveChanges();
            }
        }
    }
}