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
        public List<TblDyeingOrdersMainDetailsACC> GetDyeingOrdersMainDetailsAcc(int dyeingProductionOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var dyeingOrdersMainDetail = (from s in entities.TblDyeingOrdersMainDetailsACCs.Include("TblDyeingOrdersDetailsAccs")
                                              where s.DyeingProductionOrder == dyeingProductionOrder
                                              select s).ToList();
                return dyeingOrdersMainDetail;
            }
        }

        [OperationContract]
        public List<TblDyeingOrdersHeaderAcc> DyeingOrderHeaderSearchListAcc(TblDyeingOrdersHeaderAcc header)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var d = (from h in entities.TblDyeingOrdersHeaderAccs
                         where h.DocPlan.Contains(header.DocPlan)
                         && (h.TransactionDate == header.TransactionDate || header.TransactionDate == null)
                         && h.Vendor.Contains(header.Vendor)
                         select h).ToList();

                return d;
            }
        }

        [OperationContract]
        public List<DyeingOrderDetailsServicesAcc> GetSavedDyeingOrderServicesAcc(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.DyeingOrderDetailsServicesAccs.Where(x => x.DyeingOrdersDetailsInt == iserial).ToList();
            }
        }

        [OperationContract]
        public List<TblDyeingOrdersMainDetailsACC> SaveDyeingOrderAcc(TblDyeingOrdersHeaderAcc header, List<TblDyeingOrdersMainDetailsACC> Maindetails)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (header.DyeingProductionOrder != 0)
                {
                    var h = entities.TblDyeingOrdersHeaderAccs.SingleOrDefault(x => x.DyeingProductionOrder == header.DyeingProductionOrder);

                    if (h != null)
                    {
                        h.DocPlan = header.DocPlan;
                        h.TransactionDate = header.TransactionDate;
                        h.Vendor = header.Vendor;
                    }
                }
                else
                {
                    entities.AddToTblDyeingOrdersHeaderAccs(header);
                }
                entities.SaveChanges();

                foreach (var item in Maindetails)
                {
                    if (item.DyeingProductionOrder == 0)
                    {
                        item.DyeingProductionOrder = header.DyeingProductionOrder;
                        entities.AddToTblDyeingOrdersMainDetailsACCs(item);
                    }
                    else
                    {
                        var mainDetailRow = (from d in entities.TblDyeingOrdersMainDetailsACCs
                                             where d.TransId == item.TransId && d.TransactionType == item.TransactionType
                                             && d.DyeingProductionOrder == item.DyeingProductionOrder
                                             select d).SingleOrDefault();

                        if (mainDetailRow != null)
                        {
                            GenericUpdate(mainDetailRow, item, entities);
                        }
                        else
                        {
                            entities.AddToTblDyeingOrdersMainDetailsACCs(item);
                        }
                    }
                    entities.SaveChanges();

                    if (item.DyeingProductionOrder != 0)
                    {
                        foreach (var items in item.TblDyeingOrdersDetailsAccs.ToList())
                        {
                            var dyeingOrderDetails = (from d in entities.TblDyeingOrdersDetailsAccs
                                                      where d.Iserial == items.Iserial && d.TransactionType == item.TransactionType
                                                      select d).SingleOrDefault();
                            if (dyeingOrderDetails != null)
                            {
                                GenericUpdate(dyeingOrderDetails, items, entities);
                            }
                            else
                            {
                                items.TblDyeingOrdersMainDetailsACC = null;
                                entities.AddToTblDyeingOrdersDetailsAccs(items);
                            }
                            entities.SaveChanges();
                        }
                    }
                }
            }
            return Maindetails;
        }

        [OperationContract]
        public void DeleteDyeingOrderAcc(int dyeingHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var detailsList = entities.TblDyeingOrdersDetailsAccs.Include("DyeingOrderDetailsServicesAccs").Where(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                foreach (var item in detailsList)
                {
                    foreach (var service in item.DyeingOrderDetailsServicesAccs.ToList())
                    {
                        entities.DeleteObject(service);
                    }

                    entities.DeleteObject(item);
                }

                var mainDetailsList = entities.TblDyeingOrdersMainDetailsACCs.Where(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                foreach (var item in mainDetailsList)
                {
                    entities.DeleteObject(item);
                }

                var header = entities.TblDyeingOrdersHeaderAccs.SingleOrDefault(x => x.DyeingProductionOrder == dyeingHeaderIserial);

                entities.DeleteObject(header);

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingMainDetailsAcc(TblDyeingOrdersMainDetailsACC maindetails)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var detailsList = entities.TblDyeingOrdersDetailsAccs.Include("DyeingOrderDetailsServicesAccs").Where(x => x.DyeingProductionOrder == maindetails.DyeingProductionOrder
                    && x.TransactionType == maindetails.TransactionType && x.TransId == maindetails.TransId);

                foreach (var item in detailsList)
                {
                    foreach (var service in item.DyeingOrderDetailsServicesAccs.ToList())
                    {
                        entities.DeleteObject(service);
                    }

                    entities.DeleteObject(item);
                }

                var mainDet = entities.TblDyeingOrdersMainDetailsACCs.SingleOrDefault(x => x.DyeingProductionOrder == maindetails.DyeingProductionOrder
                                                                                       && x.TransactionType == maindetails.TransactionType && x.TransId == maindetails.TransId);

                entities.DeleteObject(mainDet);

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void DeleteDyeingDetailsAcc(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var details = entities.TblDyeingOrdersDetailsAccs.Include("DyeingOrderDetailsServicesAccs").SingleOrDefault(x => x.Iserial == iserial);

                if (details != null)
                {
                    foreach (var service in details.DyeingOrderDetailsServicesAccs.ToList())
                    {
                        entities.DeleteObject(service);
                    }
                    entities.DeleteObject(details);
                }

                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void ProducationConnectionAcc(int dyeingProductionOrder, int transId, int transactionType, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var header = entities.TblDyeingOrdersHeaderAccs.SingleOrDefault(x => x.DyeingProductionOrder == dyeingProductionOrder);
                var mainDetail = entities.TblDyeingOrdersMainDetailsACCs.Include("TblDyeingOrdersDetailsAccs.DyeingOrderDetailsServicesAccs").SingleOrDefault(x => x.DyeingProductionOrder == dyeingProductionOrder
                                                                                          && x.TransId == transId && x.TransactionType == transactionType);
                var locationLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == mainDetail.WareHouse);

                var vendorWmsLocation = entities.GetWmsLocations.SingleOrDefault(x => x.VENDID == header.Vendor);

                var vendorLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");

                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                var tableName = "PRODCONNECTION";
                var transactionGuid = Guid.NewGuid().ToString();
                foreach (var item in mainDetail.TblDyeingOrdersDetailsAccs)
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

                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.FabricCode);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMWAREHOUSE", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMBATCH", "Free");
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            //AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();

                            //  //   ProductionOrder

                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();

                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(1));
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            //  AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            //   AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();

                            ////     PickingList
                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();
                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", item.FabricCode);
                            AxaptaRecord.set_Field("DYEDITEM", item.DyedFabric);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(2));
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            //AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            //AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();
                        }

                        #endregion TransactionType0  Transfer To Vendor's Location And ProductionOrder And PickingList

                        #region TransactionType3 ReportAsFinished

                        else if (item.TransactionType == 1)
                        {
                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();

                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
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
                            // AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            // AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("JOURNALLINKID", transId);
                            AxaptaRecord.set_Field("TOWAREHOUSE", mainDetail.WareHouse);
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);

                            AxaptaRecord.Insert();
                        }

                        #endregion TransactionType3 ReportAsFinished
                    }
                    catch (Exception)
                    {
                    }
                }
                var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                try
                {
                    if (transactionType == 0)
                    {
                        //public ProdJournalId run(int transId,int journalId,int WhatToDo,str JourName,int PostorNo)
                        var Production = import.Call("run", dyeingProductionOrder, transId, 1, "", 1);
                        var Transfer = import.Call("run", dyeingProductionOrder, transId, 0, "Name", 1);
                        var PickingList = import.Call("run", dyeingProductionOrder, transId, 2, "Name", 1);
                        PurchaseDyeingServicesToAxAcc(header, mainDetail, 0, userIserial, transactionGuid);
                    }
                    else if (transactionType == 1)
                    {
                        var ReportAsFinished = import.Call("run", dyeingProductionOrder, transId, 3, "Name", 1);
                    }

                    mainDetail.Posted = true;
                    entities.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }

                Operations.SharedOperation.ClearAxTable("PRODCONNECTION", axapta, transactionGuid);
                axapta.Logoff();
            }
        }

        public void PurchaseDyeingServicesToAxAcc(TblDyeingOrdersHeaderAcc objToPost, TblDyeingOrdersMainDetailsACC headerObjToPost, int postPostOrNo, int userIserial, string transactionGuid)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                using (var axapta = new Axapta())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");

                    TblAuthUser userToLogin;

                    userToLogin = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);

                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

                    var inventTable = axapta.CreateAxaptaRecord("InventDim");

                    try
                    {
                        var vendorWmsLocation = context.GetWmsLocations.SingleOrDefault(x => x.VENDID == objToPost.Vendor);

                        var vendorLoc =
                            context.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                        var purchId = "Rc_ " + headerObjToPost.DyeingProductionOrder.ToString() + headerObjToPost.TransId.ToString() + headerObjToPost.TransactionType;
                        var tableName = "PurchTable";
                        var purchTableRecord = axapta.CreateAxaptaRecord(tableName);
                        purchTableRecord.Clear();
                        purchTableRecord.InitValue();

                        purchTableRecord.set_Field("PurchId", purchId);
                        purchTableRecord.set_Field("DeliveryDate", headerObjToPost.TblDyeingOrdersDetailsAccs.OrderByDescending(x => x.EstimatedDeliveryDate).FirstOrDefault().EstimatedDeliveryDate ?? DateTime.Now);

                        var headerax = axapta.CallStaticRecordMethod("VendTable", "find", objToPost.Vendor) as AxaptaRecord;
                        purchTableRecord.Call("initFromVendTable", headerax);

                        purchTableRecord.Insert();

                        foreach (var item in headerObjToPost.TblDyeingOrdersDetailsAccs)
                        {
                            tableName = "PurchLine";
                            foreach (var servicerow in item.DyeingOrderDetailsServicesAccs)
                            {
                                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                                axaptaRecord.Clear();
                                axaptaRecord.InitValue();

                                inventTable.Clear();
                                inventTable.set_Field("InventLocationId", vendorLoc.INVENTLOCATIONID);
                                if (item.Color != null) inventTable.set_Field("InventColorId", item.Color);

                                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                                var producationOrder = "Free";
                                using (var model = new ax2009_ccEntities())
                                {
                                    //  var batch = item.BatchNo.ToString();

                                    var firstOrDefault = model.PRODCONNECTIONs.FirstOrDefault(
                                        x =>
                                            x.DYEDITEM == item.DyedFabric && x.TRANSID == item.DyeingProductionOrder
                                           && x.FROMCONFIG == item.Color &&
                                            // x.FROMBATCH == batch &&
                                           x.JOURNALLINKID == item.TransId);
                                    if (firstOrDefault != null)
                                        producationOrder =
                                            firstOrDefault.PRODID;
                                }
                                if (producationOrder == null || producationOrder == "")
                                {
                                    producationOrder = "Free";
                                }

                                importNew.Call("CreateConfig", servicerow.ServiceCode, item.Color);
                                importNew.Call("CreateBatch", servicerow.ServiceCode, producationOrder);
                                inventTable.set_Field("configId", item.Color);
                                inventTable.set_Field("inventBatchId", producationOrder);
                                inventTable = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventTable) as AxaptaRecord;

                                if (inventTable != null)
                                {
                                    var tempx = inventTable.get_Field("inventDimId").ToString();
                                    axaptaRecord.set_Field("InventDimId", tempx);
                                }

                                axaptaRecord.set_Field("ItemId", servicerow.ServiceCode);
                                axaptaRecord.set_Field("purchId", purchId);
                                axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(item.CalculatedTotalQty.ToString()));
                                decimal price = 1;

                                //context.TblTradeAgreementDetails.Where(x => x.ItemCode == servicerow.ServiceCode && x.TblTradeAgreementHeader1.Vendor == objToPost.Vendor);
                                axaptaRecord.set_Field("PurchPrice", Convert.ToDecimal(price));
                                axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(item.CalculatedTotalQty.ToString()));
                                axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(item.CalculatedTotalQty.ToString()) * price);
                                axaptaRecord.Call("createLine", true, true, true, true, true, false);
                            }
                            //No errors occured, Commit!
                            //Axapta.TTSCommit();

                            if (postPostOrNo == 1)
                            {
                                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                                importNew.Call("PostPurchaseOrder", purchId, objToPost.TransactionDate ?? DateTime.Now);
                                PickingListForAxServicesAcc(objToPost, headerObjToPost, postPostOrNo, userIserial,
                                    transactionGuid);
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        //There was some errors, Abort transaction and Raise error!
                        //Axapta.TTSAbort();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        //Finally logoff the Axapta Session
                        axapta.Logoff();
                    }
                }
            }
        }

        private void PickingListForAxServicesAcc(TblDyeingOrdersHeaderAcc objToPost, TblDyeingOrdersMainDetailsACC headerObjToPost,
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
                    foreach (var item in headerObjToPost.TblDyeingOrdersDetailsAccs)
                    {
                        foreach (var service in item.DyeingOrderDetailsServicesAccs)
                        {
                            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            AxaptaRecord.Clear();
                            AxaptaRecord.InitValue();
                            AxaptaRecord.set_Field("TRANSID", item.DyeingProductionOrder);
                            AxaptaRecord.set_Field("RAWID", service.ServiceCode);
                            AxaptaRecord.set_Field("DYEDITEM", service.ServiceCode);
                            AxaptaRecord.set_Field("RAWQTY", item.CalculatedTotalQty);
                            AxaptaRecord.set_Field("DYEDQTY", item.CalculatedTotalQty);
                            //AxaptaRecord.set_Field("UNITID", item.Unit);
                            AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                            AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(2));
                            AxaptaRecord.set_Field("FROMCONFIG", item.Color);
                            AxaptaRecord.set_Field("TOCONFIG", item.Color);
                            // AxaptaRecord.set_Field("FROMBATCH", item.BatchNo.ToString());
                            //AxaptaRecord.set_Field("TOBATCH", item.BatchNo.ToString());
                            AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                            AxaptaRecord.set_Field("JOURNALLINKID", headerObjToPost.TransId);
                            AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                            AxaptaRecord.Insert();
                        }
                    }
                    //public ProdJournalId run(int transId,int journalId,int WhatToDo,str JourName,int PostorNo)
                    var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                    var PickingList = import.Call("run", headerObjToPost.DyeingProductionOrder, headerObjToPost.TransId, 2, "Name", postPostOrNo);

                    axapta.Logoff();
                }

                catch (Exception)
                {
                    axapta.Logoff();
                    throw;
                }
            }
        }
    }
}