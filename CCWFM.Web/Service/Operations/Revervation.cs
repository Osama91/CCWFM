using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using System.Transactions;
using LinqKit;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<Tbl_ReservationMainDetails> GetReservationMainDetails(int resHeader, out List<Fabric_UnitID> mainFabricList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var reservationMainDetails = (from i in entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails.Tbl_ReservationRec")
                                              where i.Tbl_ReservationHeader == resHeader
                                              select i).ToList();
                var fabriclist = reservationMainDetails.Select(x => x.Fabric).Distinct();

                mainFabricList = entities.Fabric_UnitID.Where(x => fabriclist.Any(l => x.Fabric_Code == l)).ToList();

                return reservationMainDetails.ToList();
            }
        }

        [OperationContract]
        public List<Tbl_ReservationMainDetails> GetReservationMainDetailByFabric(string filter,
            Dictionary<string, object> valuesObjects, bool GetOnhand,
            int tblSalesOrder, int type, string Fabric, List<string> FabricColor, out List<GetItemOnhand_Result> OnHandList, out List<Fabric_UnitID> mainFabricList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesordercode = entities.TblSalesOrders.FirstOrDefault(w => w.Iserial == tblSalesOrder).SalesOrderCode;
                if (valuesObjects.Any())
                {
                    filter = filter + " and it.Fabric LIKE(@Code0)";
                }
                else
                {
                    filter = filter + "it.Fabric LIKE(@Code0)";
                }

                valuesObjects.Add("Code0", Fabric);
                var parameterCollection = ConvertToParamters(valuesObjects);

                var reservationMainDetail = entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails.Tbl_ReservationRec").Where(filter, parameterCollection.ToArray());


                //(from i in entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails.Tbl_ReservationRec")
                var reservationMainDetails = reservationMainDetail.AsExpandable().Where(i => FabricColor.Contains(i.FabricColor)).ToList();

                foreach (var item in reservationMainDetails.Select(w => w.FabricColor).Distinct().ToList())
                {
                    if (entities.Tbl_ReservationDetails.Any(w => w.SalesOrder == salesordercode && w.Tbl_ReservationMainDetails1.Fabric == Fabric && w.Tbl_ReservationMainDetails1.FabricColor == item))
                    {
                        var rowsToRemove = reservationMainDetails.Where(w => w.FabricColor == item).ToList();
                        foreach (var rowToRemove in rowsToRemove)
                        {
                            reservationMainDetails.Remove(rowToRemove);
                        }
                    }
                }

                var fabriclist = reservationMainDetails.Select(x => x.Fabric).Distinct();
                var listOnHand = new List<GetItemOnhand_Result>();
                if (GetOnhand)
                {
                    using (var model = new ax2009_ccEntities())
                    {
                        foreach (var item in reservationMainDetails.GroupBy(x => new { x.Fabric, x.Batchno, x.FabricColor, x.Location }))
                        {
                            listOnHand.AddRange(model.GetItemOnhand(item.Key.Fabric, item.Key.Batchno, item.Key.FabricColor, DateTime.Now, item.Key.Location));
                        }

                    }
                    OnHandList = listOnHand;
                }
                else
                {
                    OnHandList = null;
                }

                mainFabricList = entities.Fabric_UnitID.Where(x => fabriclist.Any(l => x.Fabric_Code == l)).ToList();

                return reservationMainDetails.ToList();
            }
        }

        [OperationContract]
        public void DeleteReservationOrder(Tbl_ReservationHeader reservationHeader, int userIserial)
        {
            var axapta = new Axapta();//Ready To be Dependent from Ax
            if (SharedOperation.UseAx())
            {



                var credential = new NetworkCredential("bcproxy", "around1");
                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
            }
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var headerRow =
                    entities.Tbl_ReservationHeader.SingleOrDefault(x => x.Iserial == reservationHeader.Iserial);
                var rows = entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails").Include("Tbl_ReservationHeader1").Where(x => x.Tbl_ReservationHeader == reservationHeader.Iserial);

                foreach (var mainRow in rows)
                {
                    foreach (var detailsRow in mainRow.Tbl_ReservationDetails.ToList())
                    {
                        entities.DeleteObject(detailsRow);
                        if (SharedOperation.UseAx())
                        {

                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

                            if (detailsRow.AxPicklingListJournal != null)
                            {
                                importNew.Call("deletejournal", 0, detailsRow.AxPicklingListJournal, 0);
                            }
                        }
                    }
                    entities.DeleteObject(mainRow);
                }

                entities.DeleteObject(headerRow);
                entities.SaveChanges();
            }
            if (SharedOperation.UseAx())
            {
                axapta.Logoff();
            }
        }

        [OperationContract]
        public List<Tbl_ReservationMainDetails> SaveReservation(Tbl_ReservationHeader reservationHeader, List<Tbl_ReservationMainDetails> resMainDetails, string transactionGuid, out bool ErrorExists, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var errors = new List<string>();
                var mainRowWithGuid = new Dictionary<string, Tbl_ReservationMainDetails>();
                foreach (var item in resMainDetails)
                {
                    var guidCheck = Guid.NewGuid().ToString();
                    mainRowWithGuid.Add(guidCheck, item);
                }

                if (errors.Count != 0)
                {
                    ErrorExists = true;

                    return mainRowWithGuid.Where(x => errors.Any(e => x.Key == e)).Select(s => s.Value).ToList();
                }
                else
                {
                    ErrorExists = false;
                    if (reservationHeader.Iserial == 0)
                    {
                        reservationHeader.IsPosted = false;
                        reservationHeader.DocNo = "TEST";
                        entities.AddObject("Tbl_ReservationHeader", reservationHeader);
                        entities.SaveChanges();
                    }
                    foreach (var item in resMainDetails)
                    {
                        if (item.Iserial != 0)
                        {
                            foreach (var items in item.Tbl_ReservationDetails.ToList())
                            {
                                var reservationDetails = (from d in entities.Tbl_ReservationDetails
                                                          where d.Iserial == items.Iserial
                                                          select d).SingleOrDefault();
                                if (reservationDetails != null)
                                {
                                    //     items.AxPicklingListJournal = PickingListFromReservation(items, reservationHeader, transactionGuid, userIserial, true, false);
                                    GenericUpdate(reservationDetails, items, entities);
                                }
                                else
                                {
                                    items.Tbl_ReservationMainDetails1 = null;
                                    items.Tbl_ReservationMainDetails = item.Iserial;

                                    entities.AddObject("Tbl_ReservationDetails", items);
                                    //     items.AxPicklingListJournal = PickingListFromReservation(items, reservationHeader, transactionGuid, userIserial, true, false);
                                }
                            }
                        }
                        else
                        {
                            item.Tbl_ReservationHeader = reservationHeader.Iserial;
                            entities.AddObject("Tbl_ReservationMainDetails", item);
                            entities.SaveChanges();

                            foreach (var items in item.Tbl_ReservationDetails.ToList())
                            {
                                items.Tbl_ReservationMainDetails = item.Iserial;
                                //      items.AxPicklingListJournal = PickingListFromReservation(items, reservationHeader, transactionGuid, userIserial, true, false);
                            }
                        }
                    }

                    entities.SaveChanges();
                    return resMainDetails;
                }
            }
        }


        [OperationContract]
        public List<Tbl_ReservationDetails> SaveReservationDetails(List<Tbl_ReservationDetails> resMainDetails, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                foreach (var item in resMainDetails)
                {
                    var reservationDetails = (from d in entities.Tbl_ReservationDetails
                                              where d.Iserial == item.Iserial
                                              select d).SingleOrDefault();
                    if (reservationDetails != null)
                    {
                        GenericUpdate(reservationDetails, item, entities);
                    }
                    else
                    {
                        item.Tbl_ReservationMainDetails1 = null;
                        // item.Tbl_ReservationMainDetails = item.Iserial;
                        entities.AddObject("Tbl_ReservationDetails", item);
                    }
                }

                entities.SaveChanges();
                return resMainDetails;
            }
        }

        [OperationContract]
        private List<Tbl_ReservationHeader> GetReservationHeaderList(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<Tbl_ReservationHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.Tbl_ReservationHeader.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.Tbl_ReservationHeader.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.Tbl_ReservationHeader.Count();
                    query = context.Tbl_ReservationHeader.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<SalesOrderDto> GetPurchaseOrderSalesOrders(PurchaseOrderDetailDto searchedItems)
        {
            using (var workFlowEntities = new WorkFlowManagerDBEntities())
            {
                var fabricquery =
                    workFlowEntities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(x => x.FabricID == searchedItems.ItemId);
                var itemiserial = 0;
                if (fabricquery == null)
                {
                    var accQuery = workFlowEntities.tbl_AccessoryAttributesHeader.FirstOrDefault(x => x.Code == searchedItems.ItemId);
                    if (accQuery != null)
                    {
                        itemiserial = accQuery.Iserial;
                    }
                }
                else
                {
                    itemiserial = fabricquery.Iserial;
                }
                var reservedSalesOrders =
                    workFlowEntities.Tbl_ReservationDetails
                        .Where(
                            x =>
                                x.Tbl_ReservationMainDetails1.Fabric == searchedItems.ItemId &&
                                x.Tbl_ReservationMainDetails1.FabricColor == searchedItems.FabricColor)
                        .Select(w => new SalesOrderDto { SalesOrder = w.SalesOrder, SalesOrderColor = w.SalesOrderColor });

                var salesOrders = (from bom in workFlowEntities.BOMs.Include("TblBOMStyleColors.TblColor").Include("TblBOMSizes").Include("TblSalesOrder1.TblStyle1").Include("TblSalesOrder1.TblSalesOrderColors.TblColor1")
                                   where bom.BOM_Fabric == itemiserial && bom.TblBOMStyleColors.Any(w => w.TblColor.Code == searchedItems.FabricColor)
                                   && bom.TblSalesOrder1.SalesOrderType == 2 && bom.TblSalesOrder1.TblStyle1.TblLkpSeason == searchedItems.TblSeason
                                   && bom.TblSalesOrder1.TblStyle1.Brand == searchedItems.Brand && bom.TblSalesOrder1.TblStyle1.TblLkpBrandSection == searchedItems.TblBrandSection
                                   select bom);
                var querylist = new List<SalesOrderDto>();
                foreach (var bom in salesOrders.ToList())
                {
                    double? Temp = 0;
                    if (bom.BOM_FabricType != "Accessories" && fabricquery.tbl_lkp_UoM.Code != "Meter")
                    {
                        Temp = bom.TblBOMSizes.Max(x => x.MaterialUsage) *
                                  ((fabricquery.WeightPerSquarMeterAsRawMax / 1000 * fabricquery.WidthAsRawMax / 100));
                    }
                    else
                    {
                        Temp = bom.TblBOMSizes.Max(x => x.MaterialUsage);
                    }

                    foreach (var row in bom.TblBOMStyleColors.Where(x => x.TblColor != null && x.TblColor.Code.ToLower() == searchedItems.FabricColor.ToLower()))
                    {
                        if (!reservedSalesOrders.Any(x => x.SalesOrderColor == row.TblColor1.Code && x.SalesOrder == row.BOM1.TblSalesOrder1.SalesOrderCode))
                        {
                            querylist.Add(new SalesOrderDto
                            {
                                SalesOrder = row.BOM1.TblSalesOrder1.SalesOrderCode,
                                SalesOrderColor = row.TblColor1.Code,
                                IntialQty =
                                   (float)(bom.TblSalesOrder1.TblSalesOrderColors.Where(x => x.TblColor1.Code.ToLower() == row.TblColor1.Code.ToLower()).Sum(x => x.Total) * Temp)
                            });
                        }
                    }
                }
                return querylist;
            }
        }

        [OperationContract]
        public List<SalesOrderDto> GetSalesOrderReservation(string fabric, string FabricColor, int tblsalesorder)
        {
            using (var workFlowEntities = new WorkFlowManagerDBEntities())
            {
                var fabricquery =
                    workFlowEntities.tbl_FabricAttriputes.Include("tbl_lkp_UoM").FirstOrDefault(x => x.FabricID == fabric);
                var itemiserial = 0;

                itemiserial = fabricquery.Iserial;

                var reservedSalesOrders =
                    workFlowEntities.Tbl_ReservationDetails
                        .Where(
                            x =>
                                x.Tbl_ReservationMainDetails1.Fabric == fabric &&
                                x.Tbl_ReservationMainDetails1.FabricColor == FabricColor)
                        .Select(w => new SalesOrderDto { SalesOrder = w.SalesOrder, SalesOrderColor = w.SalesOrderColor });

                var salesOrders = (from bom in workFlowEntities.BOMs.Include("TblBOMStyleColors.TblColor").Include("TblBOMSizes").Include("TblSalesOrder1.TblStyle1").Include("TblSalesOrder1.TblSalesOrderColors.TblColor1")
                                   where bom.BOM_Fabric == itemiserial && bom.TblBOMStyleColors.Any(w => w.TblColor.Code == FabricColor)
                                   && bom.TblSalesOrder1.SalesOrderType == 2 && bom.BOM_Fabric == itemiserial && bom.TblSalesOrder == tblsalesorder
                                   && (bom.TblSalesOrder1.Status == 0 || bom.TblSalesOrder1.Status == 1)
                                   select bom);
                var querylist = new List<SalesOrderDto>();
                foreach (var bom in salesOrders.ToList())
                {
                    double? Temp = 0;
                    if (bom.BOM_FabricType != "Accessories" && fabricquery.tbl_lkp_UoM.Code != "Meter")
                    {
                        Temp = bom.TblBOMSizes.Max(x => x.MaterialUsage) *
                                  ((fabricquery.WeightPerSquarMeterAsRawMax / 1000 * fabricquery.WidthAsRawMax / 100));
                    }
                    else
                    {
                        Temp = bom.TblBOMSizes.Max(x => x.MaterialUsage);
                    }

                    foreach (var row in bom.TblBOMStyleColors.Where(x => x.TblColor != null && x.TblColor.Code.ToLower() == FabricColor.ToLower()))
                    {
                        if (!reservedSalesOrders.Any(x => x.SalesOrderColor == row.TblColor1.Code && x.SalesOrder == row.BOM1.TblSalesOrder1.SalesOrderCode))
                        {
                            querylist.Add(new SalesOrderDto
                            {
                                SalesOrder = row.BOM1.TblSalesOrder1.SalesOrderCode,
                                SalesOrderColor = row.TblColor1.Code,
                                IntialQty =
                                   (float)(bom.TblSalesOrder1.TblSalesOrderColors.Where(x => x.TblColor1.Code.ToLower() == row.TblColor1.Code.ToLower()).Sum(x => x.Total) * Temp)
                            });
                        }
                    }
                }
                return querylist;
            }

        }


        [OperationContract]
        public List<Tbl_fabricInspectionDetail> GetResInspectionList(decimal lineNumber, string order)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabricinspection = (from i in entities.Tbl_fabricInspectionDetail
                                        where i.LineNum == lineNumber && i.Tbl_fabricInspectionHeader1.TransOrder == order
                                        select i).ToList();
                return fabricinspection;
            }
        }

        //public string PickingListFromReservation(Tbl_ReservationDetails item, Tbl_ReservationHeader header, string transactionGuid, int userIserial, bool create, bool NoBatch)
        //{
        //    var axapta = new Axapta();
        //    try
        //    {
        //        const string tableName = "AutoPICKING";
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
        //        PURCHTABLE purchaseOrder;
        //        using (var axEntity = new ax2009_ccEntities())
        //        {
        //            purchaseOrder = axEntity.PURCHTABLEs.SingleOrDefault(x => x.PURCHID == header.TransOrder);
        //        }

        //        using (var entities = new WorkFlowManagerDBEntities())
        //        {
        //            int SalesOrderType = (int)Enums.SalesOrderType.SalesOrderPo;
        //            var styleHeaderSalesOrder =
        //                entities.TblSalesOrders.FirstOrDefault(x => x.SalesOrderCode == item.SalesOrder && x.SalesOrderType == SalesOrderType);
        //            if (styleHeaderSalesOrder != null)
        //            {
        //                var style = entities.TblStyles.FirstOrDefault(x => x.Iserial == styleHeaderSalesOrder.TblStyle).StyleCode;

        //                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);

        //                axaptaRecord.Clear();
        //                axaptaRecord.InitValue();
        //                axaptaRecord.set_Field("TransactionGuid", transactionGuid);
        //                axaptaRecord.set_Field("DATAAREAID", "Ccm");
        //                axaptaRecord.set_Field("SALESORDER", item.SalesOrder);
        //                axaptaRecord.set_Field("FABRICID", item.Tbl_ReservationMainDetails1.Fabric);
        //                axaptaRecord.set_Field("FABRIC_COLOR", item.Tbl_ReservationMainDetails1.FabricColor);
        //                axaptaRecord.set_Field("STYLEID", style);
        //                axaptaRecord.set_Field("STYLECOLOR", item.SalesOrderColor);
        //                axaptaRecord.set_Field("FABRICSITEID", item.Tbl_ReservationMainDetails1.Site);
        //                axaptaRecord.set_Field("FABRICLOCATION", item.Tbl_ReservationMainDetails1.Location);
        //                axaptaRecord.set_Field("FABRICWAREHOUSES", item.Tbl_ReservationMainDetails1.Warehouse);
        //                if (!NoBatch)
        //                {
        //                    axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Tbl_ReservationMainDetails1.Batchno);
        //                }

        //                axaptaRecord.set_Field("TRANSDATE", header.DocDate);
        //                axaptaRecord.set_Field("QTY", item.FinalQty > 0 ? item.FinalQty : item.IntialQty);
        //                axaptaRecord.set_Field("VENDOR", purchaseOrder.ORDERACCOUNT);
        //                axaptaRecord.set_Field("WORKFLOWJOURID", item.Iserial);
        //                axaptaRecord.Insert();
        //            }

        //            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

        //            if (item.AxPicklingListJournal != null)
        //            {
        //                importNew.Call("deletejournal", 0, item.AxPicklingListJournal, 0);
        //            }
        //            //CreatePicking(int  TransId,Int JournalType,int PostOrNo,int Validate)
        //            if (create)
        //            {
        //                var retval = (string)importNew.Call("CreatePicking", item.Iserial, 0, 0, 0);
        //                if (retval.Contains("No Enough AvailPhysical"))
        //                {
        //                    throw new System.ArgumentException(retval);
        //                }
        //                ClearAxTable(tableName, axapta, transactionGuid);
        //                return (string)retval;
        //            }

        //            ClearAxTable(tableName, axapta, transactionGuid);
        //            return (string)"";
        //        }
        //    }
        //    finally
        //    {
        //        axapta.Logoff();
        //    }
        //}

        //public string CheckPickingListFromReservation(Tbl_ReservationDetails item, Tbl_ReservationHeader header, string transactionGuid, int userIserial)
        //{
        //    var axapta = new Axapta();
        //    try
        //    {
        //        const string tableName = "AutoPICKING";
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
        //        PURCHTABLE purchaseOrder;
        //        using (var axEntity = new ax2009_ccEntities())
        //        {
        //            purchaseOrder = axEntity.PURCHTABLEs.SingleOrDefault(x => x.PURCHID == header.TransOrder);
        //        }

        //        using (var entities = new WorkFlowManagerDBEntities())
        //        {
        //            var styleHeaderSalesOrder =
        //                entities.TblSalesOrders.SingleOrDefault(x => x.SalesOrderCode == item.SalesOrder && x.SalesOrderType == 2);
        //            if (styleHeaderSalesOrder != null)
        //            {
        //                var styleCode = entities.TblStyles.FirstOrDefault(x => x.Iserial == styleHeaderSalesOrder.TblStyle).StyleCode;
        //                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);

        //                axaptaRecord.Clear();
        //                axaptaRecord.InitValue();
        //                axaptaRecord.set_Field("TransactionGuid", transactionGuid);
        //                axaptaRecord.set_Field("DATAAREAID", "Ccm");
        //                axaptaRecord.set_Field("SALESORDER", item.SalesOrder);
        //                axaptaRecord.set_Field("FABRICID", item.Tbl_ReservationMainDetails1.Fabric);
        //                axaptaRecord.set_Field("FABRIC_COLOR", item.Tbl_ReservationMainDetails1.FabricColor);
        //                axaptaRecord.set_Field("STYLEID", styleCode);
        //                axaptaRecord.set_Field("STYLECOLOR", item.SalesOrderColor);
        //                axaptaRecord.set_Field("FABRICSITEID", item.Tbl_ReservationMainDetails1.Site);
        //                axaptaRecord.set_Field("FABRICLOCATION", item.Tbl_ReservationMainDetails1.Location);
        //                axaptaRecord.set_Field("FABRICWAREHOUSES", item.Tbl_ReservationMainDetails1.Warehouse);
        //                axaptaRecord.set_Field("FABRICBATCHNUMBER", item.Tbl_ReservationMainDetails1.Batchno);
        //                axaptaRecord.set_Field("TRANSDATE", header.DocDate);
        //                axaptaRecord.set_Field("QTY", item.FinalQty > 0 ? item.FinalQty : item.IntialQty);
        //                axaptaRecord.set_Field("VENDOR", purchaseOrder.ORDERACCOUNT);

        //                axaptaRecord.Insert();
        //            }

        //            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

        //            var retval = (string)importNew.Call("CheckPicking", transactionGuid);

        //            ClearAxTable(tableName, axapta, transactionGuid);
        //            return (string)retval;
        //        }
        //    }
        //    finally
        //    {
        //        axapta.Logoff();
        //    }
        //}

        [OperationContract]
        public List<decimal> ReservationLineNum(string purchaseOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabricinspection = (from i in entities.Tbl_ReservationMainDetails
                                        where i.Tbl_ReservationHeader1.TransOrder == purchaseOrder
                                        select i.LineNum).ToList();
                return fabricinspection;
            }
        }

        [OperationContract]
        public void GenerateReservationFromPlan(Tbl_ReservationHeader header)
        {
            if (header.Iserial == 0)
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    using (var scope = new TransactionScope())
                    {
                        header.IsPosted = false;
                        header.DocNo = "TEST";
                        context.AddObject("Tbl_ReservationHeader", header);
                        context.SaveChanges();

                        var valuesObjects = new Dictionary<string, object>();
                        var filter = "it.PURCHID ==(@PURCHID0)";
                        valuesObjects.Add("PURCHID0", header.TransOrder);
                        filter = filter + " and it.DATAAREAID ==(@DATAAREAID0)";
                        valuesObjects.Add("DATAAREAID0", "ccm");
                        var parameterCollection = ConvertToParamters(valuesObjects);
                        var querytemp = context.PurchlineInventDims.Where(filter, parameterCollection.ToArray());
                        foreach (var p in querytemp)
                        {
                            var newrow = new Tbl_ReservationMainDetails
                            {
                                Fabric = p.ITEMID,
                                FabricUnit = p.PURCHUNIT,
                                FabricColor = p.CONFIGID,
                                Batchno = p.INVENTBATCHID,
                                Qty = (float)p.PURCHQTY,
                                Inspected = false,
                                LineNum = p.LINENUM,
                                Location = p.WMSLOCATIONID,
                                Warehouse = p.INVENTLOCATIONID,
                                Site = p.INVENTSITEID,
                                Tbl_ReservationHeader = header.Iserial,
                            };

                            var purchaseOrderDetails = from x in context.TblPurchaseOrderDetails.Include(
                                "TblPurchaseOrderDetailBreakDowns.BOM1.TblSalesOrder1")
                                                       let purid = ("p" + x.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.Code + "_" +
                                                           x.TblPurchaseOrderHeader1.Vendor).ToLower()
                                                       where x.ItemId == p.ITEMID && x.TblColor.Code == p.CONFIGID
                                                       && p.PURCHID == (purid)
                                                       select x
                            ;
                            //var purchaseOrderDetails =
                            //    context.TblPurchaseOrderDetails.Include(
                            //        "TblPurchaseOrderDetailBreakDowns.BOM1.TblSalesOrder1")
                            //        .Where(
                            //            x =>
                            //                x.ItemId == p.ITEMID && x.TblColor.Code == p.CONFIGID &&
                            //                'P' + x.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.Code + '_' +
                            //                x.TblPurchaseOrderHeader1.Vendor == p.PURCHID).ToList();
                            var reservationDetailList = new EntityCollection<Tbl_ReservationDetails>();

                            foreach (var purchaseLine in purchaseOrderDetails.ToList())

                                foreach (var row in purchaseLine.TblPurchaseOrderDetailBreakDowns)
                                {
                                    var percentage = row.Qty / purchaseLine.Qty;

                                    float salesorderQty = (float)(percentage * newrow.Qty);

                                    var newdetailrow = new Tbl_ReservationDetails
                                    {
                                        FinalQty = salesorderQty,
                                        IntialQty = salesorderQty,
                                        Markered = false,
                                        AxPicklingListJournal = "",
                                        SalesOrderColor = p.CONFIGID,
                                        SalesOrder = row.BOM1.TblSalesOrder1.SalesOrderCode,
                                    };
                                    reservationDetailList.Add(newdetailrow);
                                }
                            newrow.Tbl_ReservationDetails = reservationDetailList;

                            context.Tbl_ReservationMainDetails.AddObject(newrow);
                        }
                        context.SaveChanges();
                        scope.Complete();
                    }
                }
            }

        }

        [OperationContract]
        private int DeleteReservationDetails(int iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.Tbl_ReservationDetails
                              where e.Iserial == iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return iserial;
        }

    }
}