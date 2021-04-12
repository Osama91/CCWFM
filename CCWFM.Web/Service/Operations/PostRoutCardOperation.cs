using System;
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
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PostRoutCardToAx(int routeCardHeaderIserial, int postPostOrNo) // posted=1
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //int _TransID, string _WorkStationID, string _OperationID, int JournalType
                var routeHeaderRow = context
                    .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

                var operation = context.WF_RouteGroup.Where(x => x.iSerial == routeHeaderRow.RoutGroupID).Select(x => x.Code).SingleOrDefault();

                var workStation = context.WF_Route.Where(x => x.iSerial == routeHeaderRow.RoutID).Select(x => x.Code).SingleOrDefault();

                var detailsObjToPost = context
                 .RealRoutCards
                 .Where(x => x.TransID == routeHeaderRow.TransID
                         && x.Operation == operation
                         && x.WorkStation == workStation).ToList();

                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");
                axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);

                AxaptaRecord salesRecord = axapta.CreateAxaptaRecord("SalesLine"), invent = axapta.CreateAxaptaRecord("InventDim");

                const string tableName = "AutoRoute";
                //List<string> _TempinventDimIDList = new List<string>();
                //_TempinventDimIDList.Add("00008851_086");
                //_TempinventDimIDList.Add("00012748_086");
                //_TempinventDimIDList.Add("00008851_086");
                bool posted = false;

                try
                {
                    if (detailsObjToPost.Count > 0)
                    {
                        foreach (var item in detailsObjToPost)
                        {
                            axapta.ExecuteStmt("select * from %1 "
                                + "JOIN %2"
                                + " where %1.InventDimId == %2.InventDimId"
                                + " && %2.ConfigID =='" + item.Color + "'"
                                + " && %1.SalesId == '" + item.SalesOrder + "'", salesRecord, invent);
                            var inventDim = salesRecord.get_Field("InventDimID").ToString();

                            var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            axaptaRecord.set_Field("TransId", routeCardHeaderIserial);
                            if (item.StyleHeader != null) axaptaRecord.set_Field("ItemID", item.StyleHeader);
                            if (item.SalesOrder != null) axaptaRecord.set_Field("SalesId", item.SalesOrder);
                            if (item.Color != null) axaptaRecord.set_Field("Colour", item.Color);
                            if (item.WorkStation != null) axaptaRecord.set_Field("Machine", item.WorkStation);
                            if (item.Qty != null) axaptaRecord.set_Field("Qty", Convert.ToDecimal(item.Qty.ToString()));
                            if (item.Operation != null) axaptaRecord.set_Field("Operation", item.Operation);
                            if (item.DocDate != null) axaptaRecord.set_Field("DocDate", item.DocDate);
                            axaptaRecord.set_Field("WhareHouse", "");
                            axaptaRecord.set_Field("Site", "");
                            if (inventDim != null) axaptaRecord.set_Field("InventDimID", inventDim);
                            axaptaRecord.set_Field("JournalType", 5);
                            if ((routeHeaderRow.RouteType == 5))
                            {
                                axaptaRecord.set_Field("BatchId", "1");
                                axaptaRecord.set_Field("JournalType", 5);
                            }

                            axaptaRecord.Insert();
                        }
                        var import = axapta.CreateAxaptaObject("CreateProductionJournals");

                        var retval = import.Call("CreateRouteJournal", routeCardHeaderIserial, workStation, operation, 5, postPostOrNo);

                        if (retval.ToString() == "0")
                        {
                            throw new Exception("Error While Posting To AX");
                        }
                        else
                        {
                            routeHeaderRow.AxRouteCardJournalId = retval.ToString();

                            if (postPostOrNo == 1)
                            {
                                posted = true;
                            }
                            routeHeaderRow.IsPosted = posted;
                        }

                        ClearAxTable(tableName, axapta);

                        context.SaveChanges();
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

                if (routeHeaderRow.RouteType == 3) // Report AS a Finish
                {
                    PostReportAsAFinish(routeHeaderRow.TransID, 3, routeCardHeaderIserial, postPostOrNo);
                }

                if (routeHeaderRow.Direction == 0)
                {
                    PurchaseRouteServicesToAx(routeHeaderRow, postPostOrNo);
                }

                PickingList(routeCardHeaderIserial, postPostOrNo);
            }
        }

        private void ClearAxTable(string tableName, Axapta axapta)
        {
            var axRecord = axapta.CreateAxaptaRecord(tableName);
            axRecord.Clear();
            axRecord.InitValue();

            using (axRecord = axapta.CreateAxaptaRecord(tableName))
            {
                // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.

                string query = "select forupdate * from %1";                
                             //    " %1.WORKFLOWJOURID ==  " + iserial + "";
                axRecord.ExecuteStmt(query);
                // If the record is found then delete the record.
                if (axRecord.Found)
                {
                    // Start a transaction that can be committed.
                    axapta.TTSBegin();
                    axRecord.Delete();
                    // Commit the transaction.
                    axapta.TTSCommit();
                    axRecord.Next();
                }
            }
        }

        private void PostReportAsAFinish(int transId, int journalType, int routeCardHeaderIserial, int postPostOrNo)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var routeCardHeaderRow = context
                    .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

                var chainSetupList = context.tblChainSetups.Where(x => x.sGridHeaderEName == "Route Card").ToList();

                var routeCardDetailList = context.RouteCardDetails.Where(x => x.RoutGroupID == routeCardHeaderRow.RoutGroupID
                    && x.Trans_TransactionHeader == transId && x.Direction == routeCardHeaderRow.Direction).ToList();
                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");
                axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);
                const string tableName = "AutoRoute";
                AxaptaRecord salesRecord = axapta.CreateAxaptaRecord("SalesLine"), invent = axapta.CreateAxaptaRecord("InventDim");

                int i = 0;
                try
                {
                    if (routeCardDetailList.Count > 0)
                    {
                        foreach (var item in routeCardDetailList)
                        {
                            string warehouse = null;
                            if (item.Degree == "1st")
                            {
                                warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse1st").sSetupValue;
                            }
                            else if (item.Degree == "2nd")
                            {
                                warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse2st").sSetupValue;
                            }
                            else if (item.Degree == "3rd")
                            {
                                warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse3rd").sSetupValue;
                            }

                            var site = context.GetLocations.Where(x => x.INVENTLOCATIONID == warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();

                            #region MyRegion

                            axapta.ExecuteStmt("select * from %1 "
                                + "JOIN %2"
                                + " where %1.InventDimId == %2.InventDimId"
                                + " && %2.ConfigID =='" + item.Color + "'"
                                + " && %1.SalesId == '" + item.SalesOrder + "'", salesRecord, invent);
                            var inventDim = salesRecord.get_Field("InventDimID").ToString();

                            #endregion MyRegion

                            var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            axaptaRecord.set_Field("TransId", routeCardHeaderRow.Iserial);
                            if (item.Style != null) axaptaRecord.set_Field("ItemID", item.Style);
                            if (item.SalesOrder != null) axaptaRecord.set_Field("SalesId", item.SalesOrder);
                            if (item.Color != null) axaptaRecord.set_Field("Colour", item.Color);
                            //  AxaptaRecord.set_Field("Machine", _WorkStationID);
                            if (item.SizeQuantity != null) axaptaRecord.set_Field("Qty", Convert.ToDecimal(item.SizeQuantity));
                            //   AxaptaRecord.set_Field("Operation", _OperationID);
                            if (routeCardHeaderRow.DocDate != null) axaptaRecord.set_Field("DocDate", routeCardHeaderRow.DocDate);
                            if (warehouse != null) axaptaRecord.set_Field("WhareHouse", warehouse);
                            if (warehouse != null) axaptaRecord.set_Field("Location", warehouse);
                            if (site != null) axaptaRecord.set_Field("Site", site);
                            axaptaRecord.set_Field("InventDimID", inventDim);
                            axaptaRecord.set_Field("JournalType", journalType);
                            axaptaRecord.set_Field("BatchId", "N/a");
                            if (item.Size != null) axaptaRecord.set_Field("SreialId", item.Size);

                            if (i < 3)
                            {
                                i++;
                            }
                            axaptaRecord.Insert();
                        }
                        var import = axapta.CreateAxaptaObject("CreateProductionJournals");

                        var retval = import.Call("CreateReportJournal", routeCardHeaderRow.Iserial, journalType, true, postPostOrNo);

                        if (retval.ToString() == "0")
                        {
                            throw new Exception("Error While Posting To AX");
                        }
                        else
                        {
                            routeCardHeaderRow.AxReportAsAFinishedJournalId = retval.ToString();
                        }

                        ClearAxTable(tableName, axapta);

                        context.SaveChanges();
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

        private void PickingList(int routeCardHeaderIserial, int postPostOrNo)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var routeHeaderRow = entities
                    .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

                var pickingList = entities.RouteCardFabrics.Where(x => x.RouteCardHeaderIserial == routeHeaderRow.Iserial).ToList();

                try
                {
                    if (pickingList.Count() != 0)
                    {
                        var axapta = new Axapta();
                        var credential = new NetworkCredential("bcproxy", "around1");
                        axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);
                        const string tableName = "AutoPICKING";

                        foreach (var item in pickingList)
                        {
                            var site = entities.GetLocations.Where(x => x.INVENTLOCATIONID == item.Warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();

                            var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();
                            axaptaRecord.set_Field("DATAAREAID", "CCM");
                            if (item != null)
                            {
                                if (item.SalesOrder != null) axaptaRecord.set_Field("SALESORDER", item.SalesOrder);
                                if (item.ItemId != null) axaptaRecord.set_Field("FABRICID", item.ItemId);
                                if (item.FabricColor != null) axaptaRecord.set_Field("FABRIC_COLOR", item.FabricColor);
                                if (item.Style != null) axaptaRecord.set_Field("STYLEID", item.Style);
                                if (item.StyleColor != null) axaptaRecord.set_Field("STYLECOLOR", item.StyleColor);
                                if (site != null) axaptaRecord.set_Field("FABRICSITEID", site);
                                if (item.Warehouse != null)
                                {
                                    axaptaRecord.set_Field("FABRICLOCATION", item.Warehouse);
                                    axaptaRecord.set_Field("FABRICWAREHOUSES", item.Warehouse);
                                }
                                if (item.Barcode != null) axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Barcode.ToString());
                                axaptaRecord.set_Field("TRANSDATE", routeHeaderRow.DocDate);
                                axaptaRecord.set_Field("QTY", item.Qty);
                            }
                            axaptaRecord.set_Field("VENDOR", routeHeaderRow.Vendor);
                            axaptaRecord.set_Field("WORKFLOWJOURID", routeHeaderRow.Iserial);

                            axaptaRecord.Insert();
                        }

                        var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                        var retval = importNew.Call("CreatePicking", routeHeaderRow.Iserial, 0, postPostOrNo);
                        routeHeaderRow.AxRouteCardFabricsJournalId = retval.ToString();

                        ClearAxTable(tableName, axapta);

                        entities.SaveChanges();
                        axapta.Logoff();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void DeleteAXroute(RouteCardHeader header)
        {
            var deleteOrReverse = 0;
            if (header.IsPosted == true)
            {
                deleteOrReverse = 1;
            }

            using (var axapta = new Axapta())
            {
                var credential = new NetworkCredential("bcproxy", "around1");
                axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);

                var import = axapta.CreateAxaptaObject("CreateProductionJournals");

                string journal = null;
                switch (header.RouteType)
                {
                    case 3:
                        journal = header.AxReportAsAFinishedJournalId;
                        break;

                    case 5:
                        journal = header.AxRouteCardJournalId;
                        break;

                    case 0:
                        journal = header.AxRouteCardFabricsJournalId;
                        break;
                }

                if (journal != null) import.Call("deletejournal", header.RouteType, journal, deleteOrReverse);
                if (header.AxRouteCardFabricsJournalId != null) import.Call("deletejournal", 0, header.AxRouteCardFabricsJournalId, deleteOrReverse);
                axapta.Logoff();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PurchaseRouteServicesToAx(RouteCardHeader headerObjToPost, int postPostOrNo)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var detailsObjToPost = context.RouteCardFabrics.Where(x => x.RouteCardHeaderIserial == headerObjToPost.Iserial);

                using (var axapta = new Axapta())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");

                    axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccm", null, null, null);

                    var inventTable = axapta.CreateAxaptaRecord("InventDim");
                    try
                    {
                        var purchId = "Rc_ " + headerObjToPost.Iserial.ToString();
                        var tableName = "PurchTable";
                        var purchTableRecord = axapta.CreateAxaptaRecord(tableName);
                        purchTableRecord.Clear();
                        purchTableRecord.InitValue();

                        purchTableRecord.set_Field("PurchId", purchId);
                        purchTableRecord.set_Field("DeliveryDate", headerObjToPost.DeliveryDate);
                        //   axaptaRecord.set_Field("PurchId", _PurchID);

                        var header = axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.Vendor) as AxaptaRecord;
                        purchTableRecord.Call("initFromVendTable", header);

                        purchTableRecord.Insert();

                        tableName = "PurchLine";
                        foreach (var item in detailsObjToPost)
                        {
                            var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            inventTable.Clear();
                            inventTable.set_Field("InventLocationId", item.Warehouse);
                            if (item.FabricColor != null) inventTable.set_Field("InventColorId", item.FabricColor);
                            if (item.Size != null) inventTable.set_Field("InventSizeId", item.Size);

                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                            var producationOrder = importNew.Call("GetProdIdFromSalesorderAndColor", item.Style, item.StyleColor, item.SalesOrder);

                            if (producationOrder == null || (string)producationOrder == "")
                            {
                                producationOrder = "Free";
                            }

                            var config = importNew.Call("CreateConfig", item.ItemId, "Free");
                            var batch = importNew.Call("CreateBatch", item.ItemId, producationOrder);
                            inventTable.set_Field("configId", "Free");
                            inventTable.set_Field("inventBatchId", producationOrder);
                            inventTable = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventTable) as AxaptaRecord;

                            if (inventTable != null)
                            {
                                var tempx = inventTable.get_Field("inventDimId").ToString();
                                axaptaRecord.set_Field("InventDimId", tempx);
                            }

                            axaptaRecord.set_Field("ItemId", item.ItemId);
                            axaptaRecord.set_Field("ItemId", item.ItemId);
                            axaptaRecord.set_Field("purchId", purchId);
                            axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(item.Qty.ToString()));
                            //  axaptaRecord.set_Field("PurchPrice", item.Qty);
                            axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(item.Qty.ToString()));
                            axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(item.Qty.ToString()));
                            axaptaRecord.Call("createLine", true, true, true, true, true, true);
                        }
                        //No errors occured, Commit!
                        //Axapta.TTSCommit();

                        if (postPostOrNo == 1)
                        {
                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                            importNew.Call("PostPurchaseOrder", purchId, headerObjToPost.DocDate);
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
    }
}