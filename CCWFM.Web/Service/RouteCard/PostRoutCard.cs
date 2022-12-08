using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using CCWFM.Web.Service.Operations;
using System.Collections.Generic;
using CCWFM.Web.Service.WarehouseOp;

namespace CCWFM.Web.Service.RouteCard
{
    public partial class RouteCardService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PostRoutCardToAx(int routeCardHeaderIserial, int postPostOrNo, string transactionGuid, int userIserial) // posted=1
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var routeHeaderRow = context
                    .RouteCardHeaders.Include("TblRouteGroup").Include("RouteCardDetails").Include("Tbl_TransactionType").SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);


                // ReSharper disable once PossibleNullReferenceException
                var job = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial).TblJob;
                // ReSharper disable once PossibleNullReferenceException
                var perm = context.TblAuthPermissions.FirstOrDefault(x => x.Code == "RouteIssue").Iserial;
                var jobperm = context.TblAuthJobPermissions.Where(x => x.TblPermission == perm && x.Tbljob == job);

                if (jobperm.Any() && routeHeaderRow.Direction == 1 && routeHeaderRow.RouteType == 5 && routeHeaderRow.tblTransactionType == 5 && (routeHeaderRow.TblRouteGroup.SubFabricProcess == false || routeHeaderRow.TblRouteGroup.SubFabricProcess == null))
                {
                    //var operation = context.TblRouteGroups.Where(x => x.Iserial == routeHeaderRow.RoutGroupID).Select(x => x.AXRoutLinkCode).SingleOrDefault();

                    //var workStation = context.TblRoutes.Where(x => x.Iserial == routeHeaderRow.RoutID).Select(x => x.AXRoutLinkCode).SingleOrDefault();



                    var posted = false;

                    try
                    {
                       
                        routeHeaderRow.AxRouteCardJournalId = "111";//retval.ToString();

                        if (postPostOrNo == 1)
                        {
                            posted = true;
                        }
                        routeHeaderRow.IsPosted = posted;
                        routeHeaderRow.PostedDate = DateTime.Now;
                        routeHeaderRow.Createdby = userIserial;
                        //ClearAxTable(tableName, axapta, transactionGuid);

                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        //axapta.Logoff();
                    }
                }
                else
                {
                    if (postPostOrNo == 1)
                    {
                        if (routeHeaderRow != null && routeHeaderRow.RouteType == 3) // Report AS a Finish
                        {
                            PostReportAsAFinish(routeHeaderRow.TransID, 3, routeCardHeaderIserial, postPostOrNo,
                                transactionGuid, userIserial);


                        }
                        else if (routeHeaderRow.RouteType == 9) //EndPo
                        {
                            EndPo(routeHeaderRow.TransID, 3, routeCardHeaderIserial, postPostOrNo, transactionGuid,
                                userIserial);
                        }
                        //if (SharedOperation.UseAx())
                        //{
                        //    if (routeHeaderRow.RouteType == 6) // Recieve SalesOrder
                        //    {
                        //        DeliverSalesOrder(routeHeaderRow.TransID, 6, routeCardHeaderIserial, postPostOrNo,
                        //            transactionGuid, userIserial);
                        //    }
                        //}
                    }
                }
                if (postPostOrNo == 1)
                {
                    //if (SharedOperation.UseAx())
                    //{


                    //    if (routeHeaderRow.tblTransactionType == 2 && routeHeaderRow.TblRouteGroup.SubFabricProcess == true && routeHeaderRow.Direction == 0)
                    //    {
                    //        PurchaseRouteServicesToAx(routeHeaderRow, postPostOrNo, userIserial);
                    //    }
                    //}
                    if (routeHeaderRow.tblTransactionType == 5 || routeHeaderRow.tblTransactionType == 6 || routeHeaderRow.tblTransactionType == 11)
                    {
                        PickingList(routeCardHeaderIserial, postPostOrNo, transactionGuid, userIserial);
                    }
                    else if (routeHeaderRow.tblTransactionType == 1 || routeHeaderRow.tblTransactionType == 2)
                    {
                        TransferRoute(routeCardHeaderIserial, postPostOrNo, transactionGuid, userIserial);
                    }

                    else if (routeHeaderRow.tblTransactionType == 7)
                    {
                        PurchaseRouteItemByNegativeToAx(routeHeaderRow, postPostOrNo, userIserial);
                    }
                    else if (routeHeaderRow.tblTransactionType == 8)
                    {
                        //if (SharedOperation.UseAx())
                        //{
                        //    using (var axapta = new Axapta())//Ready To be Dependent from Ax
                        //    {
                        //        var credential = new NetworkCredential("bcproxy", "around1");

                        //        TblAuthUser userToLogin;

                        //        userToLogin = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                        //        var offsetAccount =
                        //            context.tblChainSetups.FirstOrDefault(
                        //                x => x.sGlobalSettingCode == "VendorOffsetAccount").sSetupValue;
                        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm",
                        //            null, null,
                        //            null);
                        //        var importNew = axapta.CreateAxaptaObject("Raj_LedgerJournal");
                        //        var price = routeHeaderRow.RouteCardDetails.GroupBy(w => w.ObjectIndex)
                        //               .Sum(x => x.Sum(e => e.SizeQuantity) * x.FirstOrDefault().Price);
                        //        string voucher = importNew.Call("PostingJournal", "مشتريات", "EGP", routeHeaderRow.Vendor, price, offsetAccount, routeHeaderRow.Tbl_TransactionType.Aname + "/" + routeHeaderRow.Iserial.ToString()) as string;

                        //        routeHeaderRow.AxRouteCardJournalId = voucher;
                        //        context.SaveChanges();
                        //    }
                        //}
                        //else
                        //{
                            routeHeaderRow.AxRouteCardJournalId = "1111";
                        routeHeaderRow.IsPosted = true;
                        routeHeaderRow.PostedDate = DateTime.Now;
                        context.SaveChanges();

                        //}

                    }
                    else if (routeHeaderRow.tblTransactionType == 9)
                    {
                 //       if (SharedOperation.UseAx())
                 //       {
                 //           using (var axapta = new Axapta())//Ready To be Dependent from Ax
                 //           {
                 //               var credential = new NetworkCredential("bcproxy", "around1");

                 //               TblAuthUser userToLogin;

                 //               userToLogin = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                 //               var offsetAccount =
                 //                   context.tblChainSetups.FirstOrDefault(
                 //                       x => x.sGlobalSettingCode == "VendorOffsetAccount").sSetupValue;
                 //               axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm",
                 //                   null, null,
                 //                   null);
                 //               var importNew = axapta.CreateAxaptaObject("Raj_LedgerJournal");
                 //               var price = routeHeaderRow.RouteCardDetails.GroupBy(w => w.ObjectIndex)
                 //                   .Sum(x => x.Sum(e => e.SizeQuantity) * x.FirstOrDefault().Price);
                 //               var routeHeaderRowOld = context
                 //.RouteCardHeaders.Include("TblRouteGroup").Include("RouteCardDetails").SingleOrDefault(x => x.Iserial == routeHeaderRow.LinkIserial);

                 //               var priceOld = routeHeaderRowOld.RouteCardDetails.GroupBy(w => w.ObjectIndex)
                 //                  .Sum(x => x.Sum(e => e.SizeQuantity) * x.FirstOrDefault().Price);
                 //               var voucher = importNew.Call("PostingJournal", "مشتريات", "EGP", routeHeaderRow.Vendor, price - priceOld,
                 //                   offsetAccount, routeHeaderRow.Tbl_TransactionType.Aname + "/" + routeHeaderRow.Iserial.ToString()) as string;

                 //               routeHeaderRow.AxRouteCardJournalId = voucher;
                 //               context.SaveChanges();
                 //           }
                 //       }
                 //       else
                 //       {
                            routeHeaderRow.AxRouteCardJournalId = "1111";
                        routeHeaderRow.PostedDate = DateTime.Now;
                        routeHeaderRow.IsPosted = true;
                            context.SaveChanges();

                        //}
                    }
                    //else if (routeHeaderRow.tblTransactionType == 10)
                    //{
                    //    if (SharedOperation.UseAx())
                    //    {
                    //        SalesOrderToSell(routeHeaderRow.TransID, 0, routeCardHeaderIserial, 0, "0", userIserial);
                    //    }
                    //}
                }
            }
        }

        private void PostReportAsAFinish(int transId, int journalType, int routeCardHeaderIserial, int postPostOrNo, string TransactionGuid, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var routeCardHeaderRow = context
                    .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);
                routeCardHeaderRow.AxReportAsAFinishedJournalId = "111";
                routeCardHeaderRow.IsPosted = true;
                routeCardHeaderRow.PostedDate = DateTime.Now;
                routeCardHeaderRow.Createdby = userIserial;

                context.SaveChanges();
            }

        }

        //private void DeliverSalesOrder(int transId, int journalType, int routeCardHeaderIserial, int postPostOrNo,
        //    string TransactionGuid, int userIserial)
        //{
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        var routeCardHeaderRow = context
        //            .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

        //        var chainSetupList = context.tblChainSetups.Where(x => x.sGridHeaderEName == "Route Card").ToList();

        //        var routeCardDetailList = context.RouteCardDetails.Include("TblSalesOrder1.TblStyle1")
        //            .Include("TblColor1")
        //            .Where(x => x.RoutGroupID == routeCardHeaderRow.RoutGroupID
        //                        && x.Trans_TransactionHeader == transId && x.Direction == routeCardHeaderRow.Direction &&
        //                        x.Degree == "1st")
        //            .GroupBy(p => new
        //            {
        //                p.TblSalesOrder1.SalesOrderCode,
        //                p.TblSalesOrder1.TblStyle1.StyleCode,
        //                Color = p.TblColor1.Code,
        //                p.Degree
        //            }).Select(lst => new { Salesorder = lst.Key.SalesOrderCode, lst.Key.StyleCode, lst.Key.Color, lst.Key.Degree, SizeQuantity = lst.Sum(x => x.SizeQuantity) })
        //                       .ToList();



        //        var axapta = new Axapta();//Ready To be Dependent from Ax
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
        //        axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");
        //        const string tableName = "AutoRoute";
        //        AxaptaRecord salesRecord = axapta.CreateAxaptaRecord("SalesLine"), invent = axapta.CreateAxaptaRecord("InventDim");

        //        var i = 0;
        //        try
        //        {
        //            foreach (var item in routeCardDetailList)
        //            {
        //                string warehouse = null;

        //                warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse1st").sSetupValue;

        //                var site = context.GetLocations.Where(x => x.INVENTLOCATIONID == warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();

        //                #region MyRegion

        //                axapta.ExecuteStmt("select * from %1 "
        //                    + "JOIN %2"
        //                    + " where %1.InventDimId == %2.InventDimId"
        //                    + " && %2.ConfigID =='" + item.Color + "'"
        //                    + " && %1.SalesId == '" + item.Salesorder + "'", salesRecord, invent);
        //                var inventDim = salesRecord.get_Field("InventDimID").ToString();

        //                #endregion MyRegion

        //                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
        //                axaptaRecord.Clear();
        //                axaptaRecord.InitValue();
        //                axaptaRecord.set_Field("TransactionGuid", TransactionGuid);
        //                axaptaRecord.set_Field("TransId", routeCardHeaderRow.Iserial);
        //                if (item.StyleCode != null) axaptaRecord.set_Field("ItemID", item.StyleCode);
        //                if (item.Salesorder != null) axaptaRecord.set_Field("SalesId", item.Salesorder);
        //                if (item.Color != null) axaptaRecord.set_Field("Colour", item.Color);

        //                if (item.SizeQuantity != null) axaptaRecord.set_Field("Qty", Convert.ToDecimal(item.SizeQuantity));

        //                if (routeCardHeaderRow.DocDate != null) axaptaRecord.set_Field("DocDate", routeCardHeaderRow.DocDate);
        //                if (warehouse != null) axaptaRecord.set_Field("WhareHouse", warehouse);
        //                if (warehouse != null) axaptaRecord.set_Field("Location", warehouse);
        //                if (site != null) axaptaRecord.set_Field("Site", site);
        //                axaptaRecord.set_Field("InventDimID", inventDim);
        //                axaptaRecord.set_Field("JournalType", journalType);
        //                axaptaRecord.set_Field("BatchId", "N/a");

        //                if (i < 3)
        //                {
        //                    i++;
        //                }
        //                axaptaRecord.Insert();
        //            }
        //            var import = axapta.CreateAxaptaObject("CreateProductionJournals");

        //            var retval = import.Call("SalesFormLetter", routeCardHeaderRow.Iserial, journalType, true, postPostOrNo);

        //            //        ClearAxTable(tableName, axapta, TransactionGuid);

        //            context.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            //There was some errors, Abort transaction and Raise error!
        //            //Axapta.TTSAbort();
        //            throw new Exception(ex.Message);
        //        }
        //        finally
        //        {
        //            //Finally logoff the Axapta Session
        //            axapta.Logoff();
        //        }
        //    }
        //}

    //    private void SalesOrderToSell(int transId, int journalType, int routeCardHeaderIserial, int postPostOrNo, string TransactionGuid, int userIserial)
    //    {
    //        using (var context = new WorkFlowManagerDBEntities())
    //        {
    //            var routeCardHeaderRow = context
    //                .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

    //            var chainSetupList = context.tblChainSetups.Where(x => x.sGridHeaderEName == "Route Card").ToList();

    //            var routeCardDetailList = context.RouteCardDetails.Include("TblSalesOrder1.TblStyle1")
    //                .Include("TblColor1")
    //                .Where(x => x.RoutGroupID == routeCardHeaderRow.RoutGroupID
    //                            && x.Trans_TransactionHeader == transId && x.Direction == routeCardHeaderRow.Direction &&
    //                            x.Degree == "1st")
    //                .GroupBy(p => new
    //                {
    //                    p.TblSalesOrder1.SalesOrderCode,
    //                    p.TblSalesOrder1.TblStyle1.StyleCode,
    //                    Color = p.TblColor1.Code,
    //                    p.Degree,
    //                    p.Price,
    //                    p.Size
    //                })
    //.Select(lst => new { Salesorder = lst.Key.SalesOrderCode, lst.Key.StyleCode, lst.Key.Color, lst.Key.Degree, SizeQuantity = lst.Sum(x => x.SizeQuantity), lst.Key.Size, lst.Key.Price })
    //                           .ToList();
    //            var axapta = new Axapta();//Ready To be Dependent from Ax
    //            var credential = new NetworkCredential("bcproxy", "around1");
    //            TblAuthUser userToLogin;
    //            using (var model = new WorkFlowManagerDBEntities())
    //            {
    //                userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
    //            }
    //            if (userToLogin != null)
    //                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
    //            axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");

    //            AxaptaObject salesHeader = axapta.CreateAxaptaObject("AxSalesTable");

    //            salesHeader.Call("parmCustAccount", "010");
    //            salesHeader.Call("parmCustInvoiceId", "010");

    //            salesHeader.Call("save");

    //            string salesid = salesHeader.Call("parmSalesId") as string;

    //            try
    //            {
    //                foreach (var item in routeCardDetailList)
    //                {
    //                    string warehouse = null;
    //                    if (item.Degree == "1st")
    //                    {
    //                        warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse1st").sSetupValue;
    //                    }
    //                    else if (item.Degree == "2nd")
    //                    {
    //                        warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse2st").sSetupValue;
    //                    }
    //                    else if (item.Degree == "3rd")
    //                    {
    //                        warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse3rd").sSetupValue;
    //                    }

    //                    var site = context.GetLocations.Where(x => x.INVENTLOCATIONID == warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();

    //                    #region MyRegion

    //                    var inventdim = axapta.CreateAxaptaRecord("InventDim");
    //                    inventdim.Clear();
    //                    inventdim.set_Field("InventLocationId", warehouse);
    //                    inventdim.set_Field("ConfigID", item.Color);
    //                    inventdim.set_Field("InventSizeId", item.Size);
    //                    inventdim.set_Field("InventSiteId", site);
    //                    inventdim = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventdim) as AxaptaRecord;
    //                    //axapta.ExecuteStmt("select * from %1 "
    //                    //    + "JOIN %2"
    //                    //    + " where %1.InventDimId == %2.InventDimId"
    //                    //    + " && %2.ConfigID =='" + item.Color + "'"
    //                    //       + " && %2.InventSizeId =='" + item.Size + "'"
    //                    //       + " && %2.InventLocationId =='" + warehouse + "'"
    //                    //       + " && %2.InventSiteId =='" + site + "'"
    //                    //    + " && %1.SalesId == '" + item.Salesorder + "'", salesRecord, invent);
    //                    //var inventDim = salesRecord.get_Field("InventDimID").ToString();

    //                    var inventdimstr = inventdim.get_Field("InventDimID").ToString();
    //                    //var SalesId = salesHeader.get_Field("InventDimID").ToString();

    //                    #endregion MyRegion

    //                    var import = axapta.CreateAxaptaObject("CreateProductionJournals");

    //                    //CreateSalesOrder(CustAccount custAccount,Description custDescription,SalesId   newSalesId,inventDimId inventDimId,SalesQty SalesQty,SalesPrice SalesPrice)
    //                    var retval = import.Call("CreateSalesOrder", "010", "", item.StyleCode, inventdimstr, item.SizeQuantity, item.Price, salesid);
    //                }

    //                //        ClearAxTable(tableName, axapta, TransactionGuid);

    //                context.SaveChanges();
    //            }
    //            catch (Exception ex)
    //            {
    //                //There was some errors, Abort transaction and Raise error!
    //                //Axapta.TTSAbort();
    //                throw new Exception(ex.Message);
    //            }
    //            finally
    //            {
    //                //Finally logoff the Axapta Session
    //                axapta.Logoff();
    //            }
    //        }
    //    }

        [OperationContract]
        private void EndPo(int transId, int journalType, int routeCardHeaderIserial, int postPostOrNo, string TransactionGuid, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var routeCardHeaderRow = context.RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

                var routeCardDetailList = context.RouteCardDetails.Include("TblSalesOrder1.TblStyle1").Include("TblColor1").Where(x => x.RoutGroupID == routeCardHeaderRow.RoutGroupID
         && x.Trans_TransactionHeader == transId && x.Direction == routeCardHeaderRow.Direction).ToList();
                foreach (var item in routeCardDetailList.GroupBy(x => new { x.TblSalesOrder }))
                {

                    if (SharedOperation.isSample(item.FirstOrDefault().TblSalesOrder1.SalesOrderCode))
                    {
                        SharedOperation.CalcEstimatedBom(item.FirstOrDefault().TblSalesOrder1);
                    }
                }
                //if (SharedOperation.UseAx())
                //{

                //    var chainSetupList = context.tblChainSetups.Where(x => x.sGridHeaderEName == "Route Card").ToList();

                //    var axapta = new Axapta();//Ready To be Dependent from Ax
                //    var credential = new NetworkCredential("bcproxy", "around1");
                //    TblAuthUser userToLogin;
                //    using (var model = new WorkFlowManagerDBEntities())
                //    {
                //        userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                //    }
                //    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                //    axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");
                //    const string tableName = "AutoRoute";
                //    AxaptaRecord salesRecord = axapta.CreateAxaptaRecord("SalesLine"), invent = axapta.CreateAxaptaRecord("InventDim");

                //    var i = 0;
                //    try
                //    {
                //        if (routeCardDetailList.Count > 0)
                //        {
                //            foreach (var item in routeCardDetailList.GroupBy(x => new { x.TblSalesOrder, x.TblColor }))
                //            {
                //                string warehouse = null;

                //                warehouse = chainSetupList.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultFPWarehouse1st").sSetupValue;

                //                var site = context.GetLocations.Where(x => x.INVENTLOCATIONID == warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();

                //                #region MyRegion

                //                axapta.ExecuteStmt("select * from %1 "
                //                    + "JOIN %2"
                //                    + " where %1.InventDimId == %2.InventDimId"
                //                    + " && %2.ConfigID =='" + item.FirstOrDefault().TblColor1.Code + "'"
                //                    + " && %1.SalesId == '" + item.FirstOrDefault().TblSalesOrder1.SalesOrderCode + "'", salesRecord, invent);
                //                var inventDim = salesRecord.get_Field("InventDimID").ToString();

                //                #endregion MyRegion

                //                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                //                axaptaRecord.Clear();
                //                axaptaRecord.InitValue();
                //                axaptaRecord.set_Field("TransactionGuid", TransactionGuid);
                //                axaptaRecord.set_Field("TransId", routeCardHeaderIserial);

                //                if (item.FirstOrDefault().TblSalesOrder1.TblStyle1.StyleCode != null) axaptaRecord.set_Field("itemID", item.FirstOrDefault().TblSalesOrder1.TblStyle1.StyleCode);
                //                if (item.FirstOrDefault().TblSalesOrder1.SalesOrderCode != null) axaptaRecord.set_Field("SalesId", item.FirstOrDefault().TblSalesOrder1.SalesOrderCode);
                //                if (item.FirstOrDefault().TblColor1.Code != null) axaptaRecord.set_Field("Colour", item.FirstOrDefault().TblColor1.Code);
                //                //  AxaptaRecord.set_Field("Machine", _WorkStationID);
                //                if (item.FirstOrDefault().SizeQuantity != null) axaptaRecord.set_Field("Qty", Convert.ToDecimal(item.FirstOrDefault().SizeQuantity));
                //                //   AxaptaRecord.set_Field("Operation", _OperationID);
                //                axaptaRecord.set_Field("DocDate", DateTime.Now);
                //                if (warehouse != null) axaptaRecord.set_Field("WhareHouse", warehouse);
                //                if (warehouse != null) axaptaRecord.set_Field("Location", warehouse);
                //                if (site != null) axaptaRecord.set_Field("Site", site);
                //                axaptaRecord.set_Field("InventDimID", inventDim);
                //                axaptaRecord.set_Field("JournalType", 3);
                //                axaptaRecord.set_Field("BatchId", "N/a");
                //                if (item.FirstOrDefault().Size != null) axaptaRecord.set_Field("SreialId", item.FirstOrDefault().Size);

                //                if (i < 3)
                //                {
                //                    i++;
                //                }
                //                axaptaRecord.Insert();
                //            }
                //            var import = axapta.CreateAxaptaObject("CreateProductionJournals");

                //            var retval = import.Call("EndPo", routeCardHeaderIserial, 3, true, 0);

                //            Operations.SharedOperation.ClearAxTable(tableName, axapta, TransactionGuid);
                //            routeCardHeaderRow.IsPosted = true;
                //            routeCardHeaderRow.PostedDate = DateTime.Now;
                //            routeCardHeaderRow.Createdby = userIserial;
                //            context.SaveChanges();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        //There was some errors, Abort transaction and Raise error!
                //        //Axapta.TTSAbort();
                //        throw new Exception(ex.Message);
                //    }
                //    finally
                //    {
                //        //Finally logoff the Axapta Session
                //        axapta.Logoff();
                //    }
                //}
                //else
                //{
                    routeCardHeaderRow.IsPosted = true;
                    routeCardHeaderRow.PostedDate = DateTime.Now;
                    routeCardHeaderRow.Createdby = userIserial;
                    context.SaveChanges();
                //}
            }
        }

        private void PickingList(int routeCardHeaderIserial, int postPostOrNo, string transactionGuid, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var routeHeaderRow = entities
                    .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);

                //var pickingList = entities.RouteCardFabrics.Include("TblSalesOrder1.TblStyle1").Where(x => x.RouteCardHeaderIserial == routeHeaderRow.Iserial).ToList();

                try
                {
                    //if (pickingList.Count() != 0)
                    //{
                        //if (SharedOperation.UseAx())
                        //{
           //                 var transactionTypeRow = entities
           //.Tbl_TransactionType.SingleOrDefault(x => x.Iserial == routeHeaderRow.tblTransactionType);


           //                 var axapta = new Axapta();//Ready To be Dependent from Ax
           //                 var credential = new NetworkCredential("bcproxy", "around1");
           //                 TblAuthUser userToLogin;
           //                 using (var model = new WorkFlowManagerDBEntities())
           //                 {
           //                     userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
           //                 }
           //                 axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
           //                 axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");
           //                 const string tableName = "AutoPICKING";
           //                 var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

           //                 foreach (var item in pickingList)
           //                 {
           //                     var site = entities.GetLocations.Where(x => x.INVENTLOCATIONID == item.Warehouse).Select(x => x.INVENTSITEID).FirstOrDefault();
           //                     var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
           //                     axaptaRecord.Clear();
           //                     axaptaRecord.InitValue();
           //                     axaptaRecord.set_Field("DATAAREAID", "Ccm");
           //                     if (item != null)
           //                     {
           //                         axaptaRecord.set_Field("TransactionGuid", transactionGuid);
           //                         if (item.TblSalesOrder1.SalesOrderType == 4)
           //                         {
           //                             if (item.TblSalesOrder1.SalesOrderCode != null) axaptaRecord.set_Field("SALESORDER", "SALES-001");
           //                         }
           //                         else
           //                         {
           //                             if (item.TblSalesOrder1.SalesOrderCode != null) axaptaRecord.set_Field("SALESORDER", item.TblSalesOrder1.SalesOrderCode);
           //                         }

           //                         if (item.ItemId != null) axaptaRecord.set_Field("FABRICID", item.ItemId);
           //                         if (item.FabricColor != null) axaptaRecord.set_Field("FABRIC_COLOR", entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);

           //                         if (item.TblSalesOrder1.SalesOrderType == 4)
           //                         {
           //                             if (item.TblSalesOrder1.TblStyle1.StyleCode != null) axaptaRecord.set_Field("STYLEID", "SALES");
           //                         }
           //                         else
           //                         {
           //                             if (item.TblSalesOrder1.TblStyle1.StyleCode != null) axaptaRecord.set_Field("STYLEID", item.TblSalesOrder1.TblStyle1.StyleCode);
           //                         }
           //                         if (item.TblSalesOrder1.SalesOrderType == 4)
           //                         {
           //                             //   if (item.TblSalesOrder1.TblStyle1.StyleCode != null) axaptaRecord.set_Field("STYLEID", "SALES");
           //                             axaptaRecord.set_Field("STYLECOLOR", "BL1");
           //                         }
           //                         else
           //                         {
           //                             if (item.StyleColor != 0) axaptaRecord.set_Field("STYLECOLOR", entities.TblColors.FirstOrDefault(x => x.Iserial == item.StyleColor).Code);
           //                         }

           //                         if (site != null) axaptaRecord.set_Field("FABRICSITEID", site);
           //                         if (item.Size != null) axaptaRecord.set_Field("InventSizeID", item.Size);
           //                         try
           //                         {
           //                             importNew.Call("CreateConfig", item.ItemId.ToString(CultureInfo.InvariantCulture), entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);
           //                         }
           //                         catch (Exception)
           //                         {
           //                         }
           //                         if (item.Warehouse != null)
           //                         {
           //                             axaptaRecord.set_Field("FABRICLOCATION", item.Warehouse);
           //                             axaptaRecord.set_Field("FABRICWAREHOUSES", item.Warehouse);
           //                         }
           //                         if (item.Barcode != null)
           //                         {
           //                             axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Barcode);
           //                         }
           //                         else
           //                         {
           //                             if (item.Batch != null) axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Batch);
           //                         }

           //                         axaptaRecord.set_Field("TRANSDATE", routeHeaderRow.DocDate);
           //                         axaptaRecord.set_Field("QTY", item.Qty);
           //                     }
           //                     axaptaRecord.set_Field("VENDOR", routeHeaderRow.Vendor);
           //                     axaptaRecord.set_Field("WORKFLOWJOURID", routeHeaderRow.Iserial);

           //                     axaptaRecord.Insert();
           //                 }

           //                 //  CreatePicking(int  TransId,Int JournalType,int PostOrNo,int Validate)
           //                 string retval = "";
           //                 if (transactionTypeRow.Nature == "+")
           //                 {

           //                     retval = importNew.Call("CreatePicking", routeHeaderRow.Iserial, 1, postPostOrNo, 0) as string;
           //                 }
           //                 else
           //                 {
           //                     retval = importNew.Call("CreatePicking", routeHeaderRow.Iserial, 0, postPostOrNo, 0) as string;
           //                 }
           //                 routeHeaderRow.AxRouteCardFabricsJournalId = retval;
           //                 Operations.SharedOperation.ClearAxTable(tableName, axapta, transactionGuid);
           //                 axapta.Logoff();
                        //}
                        //else
                        //{
                            routeHeaderRow.AxRouteCardFabricsJournalId = "111";
                    routeHeaderRow.PostedDate = DateTime.Now;
                    routeHeaderRow.IsPosted = true;
                    //}
                    entities.SaveChanges();
                    //}
                }
                catch (Exception ex)
                {
                    var result= entities.RouteQuantities(routeHeaderRow.Iserial.ToString());

                    var NewMsg = "";
                    foreach (var item in result)
                    {
                        NewMsg = NewMsg + item.itemid + "_" + item.Code + "_" + item.SIZE + "_" + item.BatchNo + "_  " + "Transaction Qty =" + item.TransactionQty.ToString() + "Exceed Stock Qty=" + item.StockQty + System.Environment.NewLine;
                    }

                    throw new Exception(NewMsg);
                }
            }
        }

        [OperationContract]
        public void TransferRoute(int routeCardHeaderIserial, int postPostOrNo, string transactionGuid, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var header = entities
            .RouteCardHeaders.SingleOrDefault(x => x.Iserial == routeCardHeaderIserial);
                //if (SharedOperation.UseAx())
                //{


                //    var mainDetail = entities.RouteCardFabrics.Include("TblSalesOrder1.TblStyle1").Where(x => x.RouteCardHeaderIserial == header.Iserial).ToList();

                //    var vendorWmsLocation = entities.GetWmsLocations.SingleOrDefault(x => x.VENDID == header.Vendor);

                //    var vendorLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                //    var axapta = new Axapta();//Ready To be Dependent from Ax
                //    var credential = new NetworkCredential("bcproxy", "around1");

                //    TblAuthUser userToLogin;
                //    using (var model = new WorkFlowManagerDBEntities())
                //    {
                //        userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                //    }
                //    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                //    axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");
                //    var tableName = "PRODCONNECTION";

                //    foreach (var item in mainDetail)
                //    {
                //        try
                //        {
                //            var locationLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == item.Warehouse);
                //            AxaptaRecord AxaptaRecord;

                //            AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                //            AxaptaRecord.Clear();
                //            AxaptaRecord.InitValue();

                //            if (header.tblTransactionType == 1)
                //            {
                //                //Transfer To Vendor's Location

                //                AxaptaRecord.set_Field("TRANSID", item.RouteCardHeaderIserial);
                //                AxaptaRecord.set_Field("RAWID", item.ItemId);
                //                AxaptaRecord.set_Field("DyedItem", item.ItemId);
                //                AxaptaRecord.set_Field("RAWQTY", item.Qty);
                //                AxaptaRecord.set_Field("DyedQty", item.Qty);
                //                AxaptaRecord.set_Field("UNITID", item.Unit);
                //                if (locationLoc != null) AxaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                //                AxaptaRecord.set_Field("FROMLOCATION", item.Warehouse);
                //                AxaptaRecord.set_Field("FROMWAREHOUSE", item.Warehouse);
                //                AxaptaRecord.set_Field("FROMBATCH", item.Size ?? "Free");
                //                AxaptaRecord.set_Field("FROMCONFIG", entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);
                //                if (vendorLoc != null) AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                //                if (vendorWmsLocation != null)
                //                {
                //                    AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                //                    AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                //                }
                //                AxaptaRecord.set_Field("TOBATCH", item.Size ?? item.Barcode ?? "Free");
                //                AxaptaRecord.set_Field("TOCONFIG", entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);
                //                AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                //                AxaptaRecord.set_Field("JOURNALLINKID", routeCardHeaderIserial);
                //                AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                //                AxaptaRecord.Insert();
                //            }
                //            else if (header.tblTransactionType == 2)
                //            {
                //                var service = entities.TblServices.FirstOrDefault(x => x.Code == item.ItemId);
                //                if (service == null)
                //                {
                //                    AxaptaRecord.set_Field("TRANSID", item.RouteCardHeaderIserial);
                //                    AxaptaRecord.set_Field("DyedItem", item.ItemId);
                //                    AxaptaRecord.set_Field("RAWID", item.ItemId);
                //                    AxaptaRecord.set_Field("DyedQty", item.Qty);
                //                    AxaptaRecord.set_Field("RAWQTY", item.Qty);
                //                    AxaptaRecord.set_Field("UNITID", item.Unit);
                //                    AxaptaRecord.set_Field("FROMSITE", vendorLoc.INVENTSITEID);
                //                    AxaptaRecord.set_Field("FROMLOCATION", vendorWmsLocation.WMSLOCATIONID);
                //                    AxaptaRecord.set_Field("FROMWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                //                    AxaptaRecord.set_Field("FROMBATCH", item.Size);
                //                    AxaptaRecord.set_Field("FROMCONFIG",
                //                        entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);
                //                    AxaptaRecord.set_Field("TOSITE", locationLoc.INVENTSITEID);
                //                    AxaptaRecord.set_Field("TOLOCATION", item.Warehouse);
                //                    AxaptaRecord.set_Field("TOWAREHOUSE", item.Warehouse);
                //                    AxaptaRecord.set_Field("TOBATCH", item.Size);
                //                    AxaptaRecord.set_Field("TOCONFIG",
                //                        entities.TblColors.FirstOrDefault(x => x.Iserial == item.FabricColor).Code);
                //                    AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                //                    AxaptaRecord.set_Field("JOURNALLINKID", routeCardHeaderIserial);
                //                    AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                //                    AxaptaRecord.Insert();
                //                }
                //            }
                //        }
                //        catch (Exception)
                //        {
                //        }
                //    }
                //    var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                //    try
                //    {
                //        var transfer = import.Call("run", routeCardHeaderIserial, routeCardHeaderIserial, 0, "Name", 1, header.DocDate.Value.ToUniversalTime());
                //        header.AxRouteCardFabricsJournalId = transfer.ToString();
                //        header.IsPosted = true;
                //        header.PostedDate = DateTime.Now;
                //        header.Createdby = userIserial;
                //        entities.SaveChanges();
                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }

                //    Operations.SharedOperation.ClearAxTable("PRODCONNECTION", axapta, transactionGuid);
                //    axapta.Logoff();
                //}
                //else
                //{
                    header.AxRouteCardFabricsJournalId = "1111";
                    header.IsPosted = true;
                    header.PostedDate = DateTime.Now;
                    header.Createdby = userIserial;

                try
                {

                    entities.SaveChanges();
                }
                catch (Exception)
                {

                    var result = entities.RouteQuantities(header.Iserial.ToString());

                    var NewMsg = "";
                    foreach (var item in result)
                    {
                        NewMsg = NewMsg + item.itemid + "_" + item.Code + "_" + item.SIZE + "_" + item.BatchNo + "_  " + "Transaction Qty =" + item.TransactionQty.ToString() + "Exceed Stock Qty=" + item.StockQty + System.Environment.NewLine;
                    }

                    throw new Exception(NewMsg);
                    throw;
                }





        

                //}
            }
        }

        //private void DeleteAXroute(RouteCardHeader header, int userIserial)
        //{
        //    var deleteOrReverse = 0;
        //    if (header.IsPosted == true)
        //    {
        //        deleteOrReverse = 1;
        //    }

        //    using (var axapta = new Axapta())//Ready To be Dependent from Ax
        //    {
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

        //        var import = axapta.CreateAxaptaObject("CreateProductionJournals");

        //        string journal = null;
        //        switch (header.RouteType)
        //        {
        //            case 3:
        //                journal = header.AxReportAsAFinishedJournalId;
        //                break;

        //            case 5:
        //                journal = header.AxRouteCardJournalId;
        //                break;

        //            case 0:
        //                journal = header.AxRouteCardFabricsJournalId;
        //                break;
        //        }

        //        if (journal != null) import.Call("deletejournal", header.RouteType, journal, deleteOrReverse);
        //        if (header.AxRouteCardFabricsJournalId != null) import.Call("deletejournal", 0, header.AxRouteCardFabricsJournalId, deleteOrReverse);
        //        axapta.Logoff();
        //    }
        //}

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PurchaseRouteServicesToAx(RouteCardHeader headerObjToPost, int postPostOrNo, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var detailsObjToPost = context.RouteCardFabrics.Include("TblSalesOrder1.TblStyle1").Where(x => x.RouteCardHeaderIserial == headerObjToPost.Iserial);
                var codes = detailsObjToPost.Select(x => x.ItemId);
                var serviceCode = context.TblServices.Where(x => codes.Contains(x.Code));
                if (!serviceCode.Any())
                {
                    return;
                }
                //if (SharedOperation.UseAx())
                //{


                //    using (var axapta = new Axapta())//Ready To be Dependent from Ax
                //    {
                //        var credential = new NetworkCredential("bcproxy", "around1");

                //        TblAuthUser userToLogin;

                //        userToLogin = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);

                //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

                //        var inventTable = axapta.CreateAxaptaRecord("InventDim");
                //        try
                //        {
                //            string random = Guid.NewGuid().ToString().Substring(0, 3);
                //            var purchId = "Rc_ " + detailsObjToPost.FirstOrDefault().TblSalesOrder1.TblStyle1.StyleCode + random;

                //            var tableName = "PurchTable";
                //            var purchTableRecord = axapta.CreateAxaptaRecord(tableName);
                //            purchTableRecord.Clear();
                //            purchTableRecord.InitValue();

                //            purchTableRecord.set_Field("PurchId", purchId);
                //            purchTableRecord.set_Field("DeliveryDate", headerObjToPost.DeliveryDate ?? headerObjToPost.DocDate);
                //            //   axaptaRecord.set_Field("PurchId", _PurchID);

                //            var header = axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.Vendor) as AxaptaRecord;
                //            purchTableRecord.Call("initFromVendTable", header);

                //            purchTableRecord.Insert();

                //            tableName = "PurchLine";
                //            foreach (var item in detailsObjToPost)
                //            {
                //                var service = context.TblServices.FirstOrDefault(x => x.Code == item.ItemId);
                //                if (service != null)
                //                {
                //                    var firstOrDefault = context.TblColors.FirstOrDefault(x => x.Iserial == item.NewFabricColor);
                //                    var colorcode = "Free";
                //                    if (firstOrDefault != null)
                //                    {
                //                        colorcode =
                //                           firstOrDefault.Code;
                //                    }
                //                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                //                    axaptaRecord.Clear();
                //                    axaptaRecord.InitValue();

                //                    inventTable.Clear();
                //                    inventTable.set_Field("InventLocationId", item.Warehouse);
                //                    inventTable.set_Field("wMSLocationId", item.Warehouse);
                //                    inventTable.set_Field("InventSiteId", item.Site);
                //                    if (item.FabricColor != null)
                //                        inventTable.set_Field("InventColorId",
                //                            colorcode);
                //                    if (item.Size != null) inventTable.set_Field("InventSizeId", item.Size);

                //                    var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                //                    var producationOrder = importNew.Call("GetProdIdFromSalesorderAndColor",
                //                        item.TblSalesOrder1.TblStyle1.StyleCode,
                //                        context.TblColors.FirstOrDefault(x => x.Iserial == item.StyleColor).Code,
                //                        item.TblSalesOrder1.SalesOrderCode);

                //                    if (producationOrder == null || (string)producationOrder == "")
                //                    {
                //                        producationOrder = "Free";
                //                    }
                //                    try
                //                    {
                //                        importNew.Call("CreateBatch", item.ItemId, producationOrder);
                //                        importNew.Call("CreateConfig", item.ItemId.ToString(CultureInfo.InvariantCulture), colorcode);
                //                    }
                //                    catch (Exception)
                //                    {
                //                    }
                //                    inventTable.set_Field("configId", colorcode);
                //                    inventTable.set_Field("inventBatchId", producationOrder);
                //                    inventTable =
                //                        axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventTable) as
                //                            AxaptaRecord;

                //                    if (inventTable != null)
                //                    {
                //                        var tempx = inventTable.get_Field("inventDimId").ToString();
                //                        axaptaRecord.set_Field("InventDimId", tempx);
                //                    }

                //                    axaptaRecord.set_Field("ItemId", item.ItemId);
                //                    axaptaRecord.set_Field("purchId", purchId);
                //                    axaptaRecord.set_Field("PurchUnit", item.Unit);
                //                    axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(item.Qty.ToString()));
                //                    axaptaRecord.set_Field("PurchPrice", item.CostPerUnit);
                //                    axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(item.Qty.ToString()));
                //                    axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(item.Qty * item.CostPerUnit));
                //                    axaptaRecord.Call("createLine", true, true, false, true, true, false);
                //                }
                //            }

                //            //No errors occured, Commit!
                //            //Axapta.TTSCommit();

                //            if (postPostOrNo == 1)
                //            {
                //                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                //                importNew.Call("PostPurchaseOrder", purchId, headerObjToPost.DocDate);
                //            }
                //        }

                //        catch (Exception ex)
                //        {
                //            throw new Exception(ex.Message);
                //        }
                //        finally
                //        {
                //            //Finally logoff the Axapta Session
                //            axapta.Logoff();
                //        }
                //    }
                //}
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PurchaseRouteItemByNegativeToAx(RouteCardHeader header, int postPostOrNo, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.CommandTimeout = 0;
                var headerObjToPost = context.RouteCardHeaders.Include("RouteCardFabrics").SingleOrDefault(x => x.Iserial == header.Iserial);

                if (headerObjToPost.ProductionResidue)
                {
                    var WarehouseCodes = headerObjToPost.RouteCardFabrics.Select(w => w.Warehouse).Distinct().ToList();

                    var warehouses = context.TblWarehouses.Where(w => WarehouseCodes.Contains(w.Code));


                    foreach (var warehouse in warehouses)
                    {


                        var adjustmenHeader = new TblAdjustmentHeader()
                        {
                            DocDate = headerObjToPost.DocDate ?? DateTime.Now,
                            CreationDate = DateTime.Now,
                            WarehouseIserial = warehouse.Iserial,
                            CountReference = headerObjToPost.Iserial.ToString(),


                        };
                        adjustmenHeader.TblAdjustmentDetails = new System.Data.Objects.DataClasses.EntityCollection<TblAdjustmentDetail>();
                        var routeList = headerObjToPost.RouteCardFabrics.Where(w => w.Warehouse == warehouse.Code).ToList();

                        var NewAdjustList = new List<TblAdjustmentDetail>();
                        foreach (var RouteDetailList in routeList)
                        {
                            var detail = new TblAdjustmentDetail();
                            detail.ItemAdjustment.DifferenceQuantity = Convert.ToDecimal(RouteDetailList.Qty);
                            var fabric = context.FabricAccSearches.FirstOrDefault(w => w.ItemGroup == RouteDetailList.ItemGroup && w.Code == RouteDetailList.ItemId);
                            detail.ItemDimIserial = context.FindOrCreateItemDim(fabric.Iserial, RouteDetailList.ItemGroup, RouteDetailList.FabricColor,
                    RouteDetailList.Size, RouteDetailList.Batch, warehouse.TblSite).FirstOrDefault().Iserial;

                            NewAdjustList.Add(detail);
                        }

                        var GroupedList=   NewAdjustList.GroupBy(w => w.ItemDimIserial).Select(e => new TblAdjustmentDetail {
                            ItemAdjustment = new DataLayer.ItemDimensionAdjustmentSearchModel() {
                                DifferenceQuantity = e.Sum(w => w.ItemAdjustment.DifferenceQuantity)
                            },
                            ItemDimIserial=e.Key
                        } );

                        foreach (var item in GroupedList)
                        {
                            adjustmenHeader.TblAdjustmentDetails.Add(item);
                        }
                      
                      WarehouseService srv = new WarehouseService();
                        int index = 0;
                        srv.UpdateOrInsertAdjustmentHeader(adjustmenHeader, 0, userIserial, out index);
                        srv.ApproveAdjustmentByIserial(adjustmenHeader.Iserial, userIserial, 0);
                    }
                }

                headerObjToPost.AxRouteCardFabricsJournalId = "111";
                headerObjToPost.IsPosted = true;
                headerObjToPost.PostedDate = DateTime.Now;
                try
                {


                    context.SaveChanges();

                }
                catch (Exception ex)
                {
                    var result = context.RouteQuantities(headerObjToPost.Iserial.ToString());


                    var NewMsg = "";
                    foreach (var item in result)
                    {
                        NewMsg = NewMsg + item.itemid + "_" + item.Code + "_" + item.SIZE + "_" + item.BatchNo + "_  " + "Transaction Qty =" + item.TransactionQty.ToString() + "Exceed Stock Qty=" + item.StockQty + System.Environment.NewLine;
                    }

                    throw new Exception(NewMsg);
                }

            }
        }

        [OperationContract]
        public decimal? GetAxItemPrice(int TransactionType,string ItemGroup, string fabricCode, string batch,string Size, string color, string inventlocationid)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
               // var 
                var tt = context.view_GetItemPrice.Where(w=>w.LastStoreAvgCost!=null).OrderByDescending(w => w.DocDate).FirstOrDefault(w => w.Code == fabricCode &&w.Size== Size && w.ColorCode == color && w.BatchNo == batch
                //&& w.WarehouseCode == inventlocationid
                );
                //(fabricCode, DateTime.Now, batch, color, inventlocationid).FirstOrDefault();

                if (tt != null)
                {
                    //Transfer To Vendor Sales
                    //بيع
                    if (TransactionType == 10 || TransactionType == 7)
                    {
                        if (tt.ProfitMarginPercentage > 0)
                        {
                          var Percentage=  tt.ProfitMarginPercentage / 100.00;
                            var percentageValues =  tt.LastStoreAvgCost*Convert.ToDecimal(Percentage);
                            return tt.LastStoreAvgCost+ percentageValues;
                        }
                    }
                    else {
                        return tt.LastStoreAvgCost;

                    }
                }

                return 0;
            }
    
        }
    }
}