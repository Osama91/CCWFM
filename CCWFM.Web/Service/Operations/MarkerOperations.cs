using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using System.Linq.Dynamic;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<tbl_MarkerTransactionHeader> GetMarkerTransactionHeadersList(int tblsalesorder)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                // ReSharper disable once PossibleNullReferenceException
                var salesorder = context.TblSalesOrders.FirstOrDefault(w => w.Iserial == tblsalesorder).SalesOrderCode;
                var query = context.tbl_MarkerTransactionHeader.Include("TblRoute").Include("TblRouteGroup").Include("TblMarkerType1").Where(x => x.tbl_MarkerDetail.FirstOrDefault().SalesOrder == salesorder);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<SalesOrderStyle> SalesOrderStyle()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesOrderStyle = (from b in entities.SalesOrderStyles
                                       select b).ToList();
                return salesOrderStyle;
            }
        }

        [OperationContract]
        private List<MarkerSalesOrderDetail> MarkerSalesOrderDetails(string salesOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesOrderFabric = (from b in entities.MarkerSalesOrderDetails
                                        where b.SalesOrder == salesOrder
                                        select b).Distinct().ToList();
                return salesOrderFabric;
            }
        }

        [OperationContract]
        private List<SalesOrderDto> GetTotalSizeConsumtion(string salesOrder, List<string> sizes, string fabric, int fabricColor, int StyleColor)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesOrders = (from bom in entities.TblSalesOrderColors.Include("TblSalesOrderSizeRatios")
                                   where bom.TblColor == StyleColor
                                   && bom.TblSalesOrder1.SalesOrderType == 2
                                    && bom.TblSalesOrder1.SalesOrderCode == salesOrder
                            && bom.TblSalesOrder1.Status == 1
                                   select bom);
                var querylist = new List<SalesOrderDto>();
                double? temp = 0;
                foreach (var bom in salesOrders.ToList())
                {
                    foreach (var variable in sizes)
                    {
                        var salesordercolors = bom;

                        if (salesordercolors != null)

                            foreach (var bomsize in salesordercolors.TblSalesOrderSizeRatios.Where(x => x.Size == variable))
                            {
                                temp = temp + (bomsize.ProductionPerSize);
                            }
                    }
                }
                querylist.Add(new SalesOrderDto
                {
                    IntialQty = (float)temp
                });
                return querylist.ToList();
            }
        }

        [OperationContract]
        private List<SalesOrderFabric> SalesOrderFabric(string salesOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesOrderFabric = (from b in entities.SalesOrderFabrics
                                        where b.SalesOrder == salesOrder
                                        select b).Distinct().ToList();
                return salesOrderFabric;
            }
        }

        [OperationContract]
        private List<SalesOrderFabricsColor> SalesOrderFabricsColor()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var salesOrderFabricsColor = (from b in entities.SalesOrderFabricsColors
                                              select b).Distinct().ToList();
                return salesOrderFabricsColor;
            }
        }

        [OperationContract]
        private List<tbl_MarkerDetail> MarkerDetails(int headerTransactionIserial, out TblSalesOrder SalesOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var markerDetails = (from m in entities.tbl_MarkerDetail.Include("TblColor").Include("TblColor1").Include("Tbl_Wf_CuttingOrder")
                                     where m.MarkerTransactionHeader == headerTransactionIserial
                                     select m).ToList();
                var sales = markerDetails.FirstOrDefault().SalesOrder;
                SalesOrder = entities.TblSalesOrders.FirstOrDefault(x => x.SalesOrderCode == sales && x.SalesOrderType == 2);
                return markerDetails;
            }
        }

        [OperationContract]
        private List<TblSalesOrder> SearchSalesOrder(int skip, int take, int salesOrderType, int status, string sort, string filter, Dictionary<string, object> valuesObjects)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrder> query;
                // if (filter != null)
                //{
                if (valuesObjects == null)
                {
                    valuesObjects = new Dictionary<string, object>();
                }

                if (filter == null)
                {
                    filter = "it.salesOrderType ==(@salesOrderType0) or (it.salesOrderType ==(@salesOrderType1) and it.status==0)";
                }
                else
                {
                    filter = filter + " and (it.salesOrderType ==(@salesOrderType0)  or (it.salesOrderType ==(@salesOrderType1) and it.status==0))";
                }

                valuesObjects.Add("salesOrderType0", salesOrderType);
                valuesObjects.Add("salesOrderType1", 4);
                if (status != 3)
                {
                    filter = filter + " and (it.status ==(@status0) or it.salesOrderType ==(@salesOrderType1))";
                    valuesObjects.Add("status0", status);
                }

                var parameterCollection = ConvertToParamters(valuesObjects);
                query = context.TblSalesOrders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblSalesOrderColor> SearchSalesOrderColor(int skip, int take, int salesOrderType, int status, string sort, string filter, Dictionary<string, object> valuesObjects)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSalesOrderColor> query;
                // if (filter != null)
                //{
                if (valuesObjects == null)
                {
                    valuesObjects = new Dictionary<string, object>();
                }

                if (filter == null)
                {
                    filter = "it.TblSalesOrder1.salesOrderType ==(@salesOrderType0)";
                }
                else
                {
                    filter = filter + " and it.TblSalesOrder1.status ==(@status0)";
                }

                valuesObjects.Add("salesOrderType0", salesOrderType);
                filter = filter + " and it.TblSalesOrder1.status ==(@status0)";
                valuesObjects.Add("status0", status);
                var parameterCollection = ConvertToParamters(valuesObjects);
                query = context.TblSalesOrderColors.Include("TblColor1").Include("TblSalesOrder1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblSize> SizeCode(string sizeCode)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var sizeCodeList = (from s in entities.TblSizes
                                    where s.TblSizeGroup1.Code == sizeCode
                                    orderby s.Id
                                    select s);
                return sizeCodeList.ToList();
            }
        }

        [OperationContract]
        private tbl_MarkerTransactionHeader SaveTransactionHeader(tbl_MarkerTransactionHeader markerHeaderHeader)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (markerHeaderHeader.Iserial == 0)
                {
                    entities.tbl_MarkerTransactionHeader.AddObject(markerHeaderHeader);
                }
                else
                {
                    var transactionHeaderQuery = (from h in entities.tbl_MarkerTransactionHeader
                                                  where h.Iserial == markerHeaderHeader.Iserial
                                                  select h).SingleOrDefault();

                    if (transactionHeaderQuery != null)
                    {
                        GenericUpdate(transactionHeaderQuery, markerHeaderHeader, entities);
                    }
                    foreach (var variable in markerHeaderHeader.tbl_MarkerDetail.ToList())
                    {
                        var markerDetail = (from h in entities.tbl_MarkerDetail
                                            where h.Iserial == variable.Iserial
                                            select h).SingleOrDefault();
                        if (markerDetail != null)
                        {
                            GenericUpdate(markerDetail, variable, entities);

                            foreach (var variableDetail in variable.tbl_MarkerDetailMeterPerSize.ToList())
                            {
                                var tblMarkerDetailMeterPerSize = (from h in entities.tbl_MarkerDetailMeterPerSize
                                                                   where h.Iserial == variableDetail.Iserial
                                                                   && h.MeterPerSizeCode == variableDetail.MeterPerSizeCode
                                                                   select h).SingleOrDefault();
                                if (tblMarkerDetailMeterPerSize != null)
                                {
                                    GenericUpdate(tblMarkerDetailMeterPerSize, variableDetail, entities);
                                }
                                else
                                {
                                    variableDetail.Iserial = variable.Iserial;
                                    variableDetail.tbl_MarkerDetail = null;
                                    entities.tbl_MarkerDetailMeterPerSize.AddObject(variableDetail);
                                }
                            }
                        }
                        else
                        {
                            variable.MarkerTransactionHeader = markerHeaderHeader.Iserial;
                            variable.tbl_MarkerTransactionHeader = null;
                            entities.tbl_MarkerDetail.AddObject(variable);
                        }
                    }
                }

                entities.SaveChanges();
                return markerHeaderHeader;
            }
        }

        [OperationContract]
        private Tbl_Wf_CuttingOrder SavingCuttingOrder(Tbl_Wf_CuttingOrder order)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var cuttingOrderQuery = (from c in entities.Tbl_Wf_CuttingOrder
                                         where c.MarkerHeaderTransaction == order.MarkerHeaderTransaction
                                     && c.MarkerIserial == order.MarkerIserial
                                     && c.InspectionIserial == order.InspectionIserial
                                         select c).SingleOrDefault();

                var inspectionLine = entities.Tbl_fabricInspectionDetail.SingleOrDefault(x => x.Iserial == order.InspectionIserial);

                var reservationLines = entities.Tbl_ReservationRec.Include("Tbl_ReservationDetails1").Where(x => x.Tbl_FabricInspectionDetails == order.InspectionIserial);

                float rem = 0;
                if (cuttingOrderQuery != null)
                {
                    if (inspectionLine != null)
                    {
                        rem = inspectionLine.RemainingMarkerRollQty - Convert.ToSingle(order.RollAssignedQty) + Convert.ToSingle(cuttingOrderQuery.RollAssignedQty);
                        inspectionLine.RemainingMarkerRollQty = rem;

                        if (rem < 0)
                        {
                            return cuttingOrderQuery;
                        }
                    }
                    inspectionLine.RemainingMarkerRollQty = (float)(rem + order.RollAssignedQty);
                    entities.DeleteObject(cuttingOrderQuery);
                    entities.SaveChanges();
                }
                if (inspectionLine != null)
                {
                    rem = inspectionLine.RemainingMarkerRollQty - Convert.ToSingle(order.RollAssignedQty);
                    inspectionLine.RemainingMarkerRollQty = rem;

                    entities.Tbl_Wf_CuttingOrder.AddObject(order);

                    var marker = entities.tbl_MarkerDetail.Include("TblColor").Include("TblColor1").FirstOrDefault(w => w.Iserial == order.MarkerIserial);
                    var inspectionLineRow = entities.FabricInspectionPurchaseOrders.SingleOrDefault(x => x.Iserial == order.InspectionIserial);
                    var reservationDetail =
                        entities.Tbl_ReservationDetails.FirstOrDefault(w => w.SalesOrder == marker.SalesOrder
                                                                            && w.SalesOrderColor == marker.TblColor.Code &&
                                                                            inspectionLine.Fabric_Code ==
                                                                            w.Tbl_ReservationMainDetails1.Fabric &&
                                                                            w.Tbl_ReservationMainDetails1.FabricColor ==
                                                                            inspectionLine.ColorCode);
                    if (!reservationLines.Any())
                    {
                        if (reservationDetail != null)
                        {
                            if (inspectionLineRow != null)
                            {
                                var newrow = new Tbl_ReservationRec
                                {
                                    Site = inspectionLine.FinishedSite,
                                    Item = inspectionLine.Fabric_Code,
                                    ItemColor = inspectionLine.ColorCode,
                                    RollNo = inspectionLine.RollNo,
                                    BatchNo = inspectionLineRow.BARCODE,
                                    Location = inspectionLine.FinishedWarehouse,
                                    Warehouse = inspectionLine.FinishedWarehouse,
                                    Qty = (float)order.RollAssignedQty,
                                    Tbl_FabricInspectionDetails = inspectionLine.Iserial,
                                    Tbl_ReservationDetails = reservationDetail.Iserial
                                };
                                entities.Tbl_ReservationRec.AddObject(newrow);
                            }
                        }
                    }
                    else
                    {
                        //foreach (var resLine in reservationLines)

                        //    if (resLine.Tbl_ReservationDetails1.Markered != true)
                        //    {
                        //        var rowtodelete =
                        //            entities.Tbl_ReservationDetails.FirstOrDefault(
                        //                x => x.Iserial == resLine.Tbl_ReservationDetails1.Iserial);

                        //        entities.Tbl_ReservationDetails.DeleteObject(rowtodelete);
                        //    }
                    }
                    entities.SaveChanges();
                    foreach (var resLine in reservationLines)
                    {
                        if (marker != null && (resLine.Tbl_ReservationDetails1.SalesOrder == marker.SalesOrder && resLine.Tbl_ReservationDetails1.SalesOrderColor == marker.TblColor.Code))
                        {
                            resLine.Qty = (float)order.RollAssignedQty;
                            resLine.Tbl_ReservationDetails1.Markered = true;
                        }
                    }
                }
                entities.SaveChanges();
            }
            return order;
        }

        [OperationContract]
        private Tbl_Wf_CuttingOrder DeleteCuttingOrder(Tbl_Wf_CuttingOrder order)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var cuttingOrderQuery = (from c in entities.Tbl_Wf_CuttingOrder.Include("tbl_MarkerDetail.TblColor")
                                         where c.MarkerIserial == order.MarkerIserial
                                               && c.InspectionIserial == order.InspectionIserial
                                         select c).FirstOrDefault();
                if (cuttingOrderQuery != null)
                {
                    //var inspection =
                    //    entities.Tbl_fabricInspectionDetail.FirstOrDefault(x => x.Iserial == order.InspectionIserial);

                    //if (inspection != null)
                    //{
                    //    inspection.RemainingMarkerRollQty = (float)(inspection.RemainingMarkerRollQty +
                    //                                                     cuttingOrderQuery.RollAssignedQty);
                    //}
                    var reservationLines = entities.Tbl_ReservationRec.Include("Tbl_ReservationDetails1").FirstOrDefault(x => x.Tbl_FabricInspectionDetails == order.InspectionIserial
                        && x.Tbl_ReservationDetails1.SalesOrder == cuttingOrderQuery.tbl_MarkerDetail.SalesOrder && x.Tbl_ReservationDetails1.SalesOrderColor == cuttingOrderQuery.tbl_MarkerDetail.TblColor.Code);
                    if (reservationLines != null) entities.DeleteObject(reservationLines);
                    entities.DeleteObject(cuttingOrderQuery);
                    entities.SaveChanges();
                    return order;
                }
            }
            return null;
        }

        [OperationContract]
        private List<Inspection> Inspection(int markerIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;
                var inspectionList = (from i in entities.Inspections
                                      where i.markerIserial == markerIserial
                                      select i).ToList();
                return inspectionList;
            }
        }

        [OperationContract]
        private List<InspectionsRoute> InspectionRoute(string fabricCode, int fabricColor)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;

                var color = entities.TblColors.Where(w => w.Iserial == fabricColor).FirstOrDefault().Code;

                var list = entities.ExecuteStoreQuery<InspectionsRoute>("SELECT * FROM [InspectionsRoute] where Fabric_Code = '" + fabricCode + "' and ColorCode = '" + color + "' ").ToList();
                
                return list;
            }
        }

        [OperationContract]
        private List<InspectionsFullDimention> InspectionFullDim(string rollBatch)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var inspectionList = (from i in entities.InspectionsFullDimentions
                                      where i.RollBatch == rollBatch
                                      select i).ToList();
                return inspectionList;
            }
        }

        [OperationContract]
        private List<tbl_MarkerDetailMeterPerSize> MeterPerSize(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var markerHeader = (from ms in entities.tbl_MarkerDetailMeterPerSize
                                    where ms.Iserial == iserial
                                    orderby ms.SizeCode_Id
                                    select ms).ToList();
                return markerHeader;
            }
        }

        [OperationContract]
        private void InsertPickingList(int markerHeadersIserial, int userIserial)
        {
            GenerateRouteFromMarker(markerHeadersIserial, userIserial);
        }

        [OperationContract]
        private void GenerateRouteFromMarker(int iserial, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var h = (from head in entities.tbl_MarkerTransactionHeader
                         where head.Iserial == iserial
                         select head).SingleOrDefault();
                h.Posted = true;

                var markerDetailQuery = (from f in entities.tbl_MarkerDetail.Include("Tbl_Wf_CuttingOrder")
                                         where f.MarkerTransactionHeader == iserial
                                         && f.Tbl_Wf_CuttingOrder.Count() != 0
                                         select f).ToList();

                var salesorder = markerDetailQuery.FirstOrDefault().SalesOrder;
                var salesOrderIserial = entities.TblSalesOrders.FirstOrDefault(w => w.SalesOrderCode == salesorder && w.SalesOrderType == 2).Iserial;
                var bomline = entities.BOMs.FirstOrDefault(w => w.TblSalesOrder == salesOrderIserial && w.BOM_IsMainFabric == true);
                if (bomline==null)
                {
                    throw new InvalidDataException("Please Check Main fabric");
                }
                var routeHeaderOld = (from head in entities.RouteCardHeaders
                                      where head.MarkerTransaction == iserial
                                      select head).SingleOrDefault();

                if (routeHeaderOld != null && routeHeaderOld.AxRouteCardFabricsJournalId != null)
                {
                    return;
                }
                else
                {
                    if (routeHeaderOld != null) entities.RouteCardHeaders.DeleteObject(routeHeaderOld);
                }
                var transactionType = 5;

                //var singleOrDefault = entities.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "DefaultVendor");

                //if (singleOrDefault != null)
                //{
                //    var defaultVendor = singleOrDefault.sSetupValue;

                //    transactionType = h != null && defaultVendor == h.Vendor ? 5:5;
                //}
                if (bomline.BOM_FabricRout==null)
                {
                    throw new InvalidDataException("Please Check Main fabric Route Group");
                }
                h.Operation = (int)bomline.BOM_FabricRout;
                if (h != null)
                {
                    var routeHeader = new RouteCardHeader
                    {
                        tblTransactionType = transactionType,
                        DocDate = h.TransDate,
                        Direction = 0,
                        MarkerTransaction = h.Iserial,
                        RoutGroupID = h.Operation,
                        Vendor = h.Vendor,
                        RoutID = h.Workstation,
                        IsPosted = false,
                        TransID = Operations.SharedOperation.GetMaxRouteCardTransactionID(h.Operation, 0, transactionType) + 1,
                        RouteType = 5,
                        DeliveryDate = h.TransDate
                    };

                    entities.AddToRouteCardHeaders(routeHeader);
                    entities.SaveChanges();
                    foreach (var d in markerDetailQuery)
                    {
                        foreach (var item in d.Tbl_Wf_CuttingOrder)
                        {
                            var fabricaccsearch = entities.FabricAccSearches.FirstOrDefault(w => w.Code == d.FabricCode);
                            var total = GetNetRollFromTotal(item.Barcode, item.RollAssignedQty);
                            var inspection =
                                entities.Tbl_fabricInspectionDetail.FirstOrDefault(x => x.Iserial == item.InspectionIserial);

                            var site = entities.GetLocations.Where(x => x.INVENTLOCATIONID == inspection.FinishedWarehouse).Select(x => x.INVENTSITEID).FirstOrDefault();
                            var routeFabric = new RouteCardFabric
                            {
                                ItemGroup = fabricaccsearch.ItemGroup,
                                TblSalesOrder = salesOrderIserial,
                                Barcode = item.Barcode,
                                Batch = inspection.BatchNo,
                                FabricColor = d.FabricColorCode,
                                ItemId = d.FabricCode,
                                Location = inspection.FinishedWarehouse,
                                Warehouse = inspection.FinishedWarehouse,
                                Qty = total,
                                RemainingQty = total,
                                RouteCardHeaderIserial = routeHeader.Iserial,
                                Site = site,
                                StyleColor = d.StyleColorCode,
                                Unit = item.RollUnit,
                                Size = inspection.BatchNo,
                            };

                            entities.AddObject("RouteCardFabrics", routeFabric);
                            entities.SaveChanges();
                        }
                    }
                    var discStyleColors =
                       markerDetailQuery.GroupBy(x => x.StyleColorCode).Select(x => x.Key).Distinct();
                    var salesordercode = markerDetailQuery.FirstOrDefault().SalesOrder;
                    var salesOrderIserials = entities.TblSalesOrders.FirstOrDefault(w => w.SalesOrderCode == salesordercode && w.SalesOrderType == 2).Iserial;
                    var salesOrder = entities.TblSalesOrders.Include("TblStyle1.TblSizeGroup1.TblSizes").FirstOrDefault(w => w.Iserial == salesOrderIserials);

                    foreach (var variable in discStyleColors)
                    {
                        var objectIndex = Guid.NewGuid().ToString("D");

                        var rr = new WorkFlowManagerDBEntities().markerPostToRouteProcedure(iserial, variable).ToList();
                        var warehousecode = GetChainSetupBycode("DefaultFPWarehouse1st");

                        var warehouseIserial = entities.TblWarehouses.FirstOrDefault(w => w.Code == warehousecode).Iserial;
                        if (rr.Any())
                        {
                            foreach (var row in rr)
                            {
                                var newrow = new RouteCardDetail
                                {
                                    TblColor = variable,
                                    Degree = "1st",
                                    TblSalesOrder = salesOrderIserials,
                                    Trans_TransactionHeader = routeHeader.TransID,
                                    Size = row.meterpersizecode,
                                    SizeQuantity = (int?)row.sizeQtyReal,
                                    RoutGroupID = h.Operation,
                                    Direction = 0,
                                    ObjectIndex = objectIndex,
                                    RouteCardHeaderIserial = routeHeader.Iserial,
                                    TblWarehouse = warehouseIserial
                                };
                                entities.RouteCardDetails.AddObject(newrow);
                            }
                        }
                        else
                        {
                            var objectIndexnew = Guid.NewGuid().ToString("D");
                            foreach (var VARIABLE in salesOrder.TblStyle1.TblSizeGroup1.TblSizes)
                            {
                                var newrow = new RouteCardDetail
                                {
                                    TblColor = variable,
                                    Degree = "1st",
                                    TblSalesOrder = salesOrderIserials,
                                    Trans_TransactionHeader = routeHeader.TransID,
                                    Size = VARIABLE.SizeCode,
                                    SizeQuantity = 0,
                                    RoutGroupID = h.Operation,
                                    Direction = 0,
                                    ObjectIndex = objectIndexnew,
                                    RouteCardHeaderIserial = routeHeader.Iserial,
                                    TblWarehouse = warehouseIserial
                                };

                                entities.RouteCardDetails.AddObject(newrow);
                            }
                        }
                    }
                }
                entities.SaveChanges();

                if (h.Status == 0)
                {
                    // SenderMarkerMail("MarkerMail", "Marker", "", h.Iserial);
                }
            }
        }

        [OperationContract]
        public void SenderMarkerMail(string reportName, string subject, string body, int markerHeader)
        {
            string deviceInfo = null;
            var extension = String.Empty;
            var mimeType = String.Empty;
            var encoding = String.Empty;
            Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] warnings = null;
            string[] streamIDs = null;
            string historyId = null;
            var rsExec = new ReportExecutionService();
            rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            using (var context = new WorkFlowManagerDBEntities())
            {
                if (string.IsNullOrEmpty(ReportServer))
                {
                    ReportServer = context.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "ReportServer").sSetupValue;
                }
            }

            rsExec.Url = ReportServer + "/ReportExecution2005.asmx";

            // Load the report
            var execInfo = rsExec.LoadReport("/Report/" + reportName, historyId);
            var para = new ObservableCollection<string> { markerHeader.ToString() };
            var parameters = new ParameterValue[para.Count];
            foreach (var row in para)
            {
                var index = para.IndexOf(row);
                parameters[0] = new ParameterValue();
                parameters[index].Value = row;
                parameters[index].Name = execInfo.Parameters[index].Name;

                // paramters) { Name = , Value = row } }, "en-us");
            }
            rsExec.SetExecutionParameters(parameters, "en-us");

            // get pdf of report
            var results = rsExec.Render("PDF", deviceInfo,
            out extension, out encoding,
            out mimeType, out warnings, out streamIDs);

            //Walla...almost no code, it's easy to manage and your done.

            //Take the bytes and add as an attachment to a MailMessage(SMTP):

            var attach = new Attachment(new MemoryStream(results),
                String.Format("{0}.pdf", reportName));

            string emailFrom;
            var emailTo = new List<string>();
            using (var model = new WorkFlowManagerDBEntities())
            {
                emailFrom = "sadatdesign@cc-egypt.com";

                var brandsectionMail = model.TblMarkerMails;

                foreach (var variable in brandsectionMail.Select(x => x.Emp))
                {
                    emailTo.Add(model.Employees.FirstOrDefault(x => x.EMPLID == variable).Email);
                }
            }
            SendEmail(attach, emailFrom, emailTo, subject, body);
        }

        private double GetNetRollFromTotal(string Barcode, double rollAssignedQty)
        {
            double total = 0;
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var barcode = entities.FabricInspectionPurchaseOrders.FirstOrDefault(x => x.BARCODE == Barcode);

                if (barcode != null)
                {
                    total = (double)((rollAssignedQty / barcode.NetRollQty) * barcode.StoreRollQty);
                }
            }
            return total;
        }

        [OperationContract]
        private int DeleteMarker(int iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var markerQuery = (from m in entities.tbl_MarkerDetail
                                   where m.Iserial == iserial
                                   select m).SingleOrDefault();
                if (markerQuery != null)
                {
                    entities.DeleteObject(markerQuery);
                }

                entities.SaveChanges();
                return iserial;
            }
        }

        [OperationContract]
        private void DeleteMarkerTransaction(int iserialHeader)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var headerTransaction = (from h in entities.tbl_MarkerTransactionHeader
                                         where h.Iserial == iserialHeader
                                         select h).SingleOrDefault();

                if (headerTransaction != null)
                {
                    entities.DeleteObject(headerTransaction);
                }

                entities.SaveChanges();
            }
        }

        [OperationContract]
        private int SaveMarkerTemp(List<TblMarkerTemp> markerList)
        {
            int returnStatus = 0;
            var markerheaderIserial = markerList.FirstOrDefault().MarkerTransactionHeader;
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var markerheader =
                    entities.tbl_MarkerTransactionHeader.FirstOrDefault(w => w.Iserial == markerheaderIserial);
                var color = (int)markerList.FirstOrDefault().TblColor;

                foreach (var variable in markerList)
                {
                    var markertemp =
                        entities.TblMarkerTemps.FirstOrDefault(
                            x =>
                                x.MarkerNo == variable.MarkerNo && x.MarkerTransactionHeader == variable.MarkerTransactionHeader && x.TblColor == variable.TblColor &&
                                x.Size == variable.Size);
                    if (markertemp == null)
                    {
                        entities.TblMarkerTemps.AddObject(variable);
                    }
                    else
                    {
                        variable.Iserial = markertemp.Iserial;
                        GenericUpdate(markertemp, variable, entities);
                    }
                }
                entities.SaveChanges();

                if (markerheaderIserial != 0)
                {
                    if (!entities.Tbl_Wf_CuttingOrder.Any(x => x.MarkerHeaderTransaction == markerheaderIserial))
                    {
                        var markerNoList =
                            entities.tbl_MarkerDetail.Where(x => x.MarkerTransactionHeader == markerheaderIserial)
                                .Select(w => w.MarkerNo)
                                .Distinct();
                        var statusList = new List<int>();
                        foreach (var variable in markerNoList)
                        {
                            var markerRow =
                                entities.tbl_MarkerDetail.Where(
                                    x =>
                                        x.MarkerTransactionHeader == markerheaderIserial && x.StyleColorCode == color &&
                                        x.MarkerNo == variable).ToList();

                            foreach (var marker in markerRow)
                            {
                                int status = 0;
                                CalcMarker(markerheader, marker, entities, out status);
                                marker.Status = status;
                                statusList.Add(status);
                            }
                        }
                        if (statusList.Any(w => w == 0))
                        {
                            markerheader.Status = 0;
                        }
                        else
                        {
                            markerheader.Status = 1;
                            returnStatus = 1;
                        }
                        var noOfLayers = entities.TblMarkerTemps.Where(
                   x => x.MarkerTransactionHeader == markerheader.Iserial
                        );
                        if (noOfLayers.Any(w => w.NoOfLayersOrg > w.NoOfLayers))
                        {
                            markerheader.Status = 0;
                        }

                        entities.SaveChanges();
                    }
                }
            }
            return returnStatus;
        }

        private void CalcMarker(tbl_MarkerTransactionHeader markerheader, tbl_MarkerDetail row, WorkFlowManagerDBEntities entities, out int status)
        {
            var noOfLayers = entities.TblMarkerTemps.FirstOrDefault(
                     x => x.MarkerNo == row.MarkerNo && x.MarkerTransactionHeader == row.MarkerTransactionHeader
                         && x.TblColor == row.StyleColorCode).NoOfLayers;
            var totalReqFabric = noOfLayers * row.MarkerL;
            var inspections = entities.Inspections.Where(x => x.markerIserial == row.Iserial).ToList();

            var totalAvaFabric = inspections.Sum(x => x.RemainingMarkerRollQty);
            if (inspections.FirstOrDefault().Unit == "Kgg")
            {
                totalAvaFabric = (float)inspections.Sum(x => x.qtyPerKilo);
            }
            row.Status = 0;
            if (totalReqFabric <= totalAvaFabric)
            {
                row.Status = 1;

                status = 1;
                var listOfInspections =
                    inspections.Where(w => totalReqFabric < w.RemainingMarkerRollQty);
                Inspection choosedRoll;
                if (listOfInspections.Any())
                {
                    choosedRoll = listOfInspections.OrderBy(x => x.RemainingMarkerRollQty).FirstOrDefault();
                    double rollassigned = 0;

                    switch (markerheader.TblMarkerType)
                    {
                        case 0:
                            if (inspections.FirstOrDefault().Unit == "Kgg")
                            {
                                rollassigned =
                                    (double)(totalReqFabric * (choosedRoll.RollWMT * (choosedRoll.M2WeightGm / 100)));
                            }
                            else
                            {
                                rollassigned = (double)totalReqFabric;
                            }
                            break;

                        case 1:
                            rollassigned = choosedRoll.RemainingMarkerRollQty;
                            break;
                    }
                    var cuttingRow = new Tbl_Wf_CuttingOrder
                    {
                        MarkerHeaderTransaction = row.MarkerTransactionHeader,
                        InspectionIserial = choosedRoll.Iserial,
                        RollUnit = choosedRoll.Unit,
                        MarkerIserial = choosedRoll.markerIserial,
                        RollAssignedQty = rollassigned,
                        CuttingSelection = "C",
                        Barcode = choosedRoll.RollBatch,
                        TblMarkerType = 0,
                    };

                    SavingCuttingOrder(cuttingRow);
                }
                else
                {
                    var listOfMarkerList = new List<Inspection>();
                    var remaining = totalReqFabric;

                    foreach (var variable in inspections.OrderByDescending(x => x.RemainingMarkerRollQty))
                    {
                        if (remaining > 0)
                        {
                            var remlist = inspections.Where(x => listOfMarkerList.Any(s => s.RollBatch != x.RollBatch));
                            var templist =
                                remlist.Where(x => remaining < x.RemainingMarkerRollQty);

                            if (inspections.FirstOrDefault().Unit == "Kgg")
                            {
                                //templist =
                                //inspections
                                //    .Where(x => !listOfMarkerList.Contains(x) && remaining < x.qtyPerKilo);
                            }
                            if (templist.Any())
                            {
                                choosedRoll = templist.OrderBy(x => x.RemainingMarkerRollQty).FirstOrDefault();
                                if (inspections.FirstOrDefault().Unit == "Kgg")
                                {
                                    choosedRoll = templist.OrderBy(x => x.qtyPerKilo).FirstOrDefault();
                                }
                            }
                            else
                            {
                                choosedRoll = variable;
                            }

                            listOfMarkerList.Add(choosedRoll);

                            double rollassigned = 0;

                            switch (markerheader.TblMarkerType)
                            {
                                case 0:
                                    if (templist.Any())
                                    {
                                        rollassigned = (double)remaining;
                                        if (inspections.FirstOrDefault().Unit == "Kgg")
                                        {
                                            rollassigned = (double)(rollassigned *
                                                                     (choosedRoll.RollWMT * (choosedRoll.M2WeightGm / 100)));
                                        }
                                    }

                                    else
                                    {
                                        rollassigned = choosedRoll.RemainingMarkerRollQty;
                                    }
                                    break;

                                case 1:
                                    rollassigned = choosedRoll.RemainingMarkerRollQty;
                                    break;
                            }
                            remaining = remaining - rollassigned;
                            var cuttingRow = new Tbl_Wf_CuttingOrder
                            {
                                MarkerHeaderTransaction = row.MarkerTransactionHeader,
                                InspectionIserial = choosedRoll.Iserial,
                                RollUnit = choosedRoll.Unit,
                                MarkerIserial = choosedRoll.markerIserial,
                                RollAssignedQty = rollassigned,
                                CuttingSelection = "C",
                                Barcode = choosedRoll.RollBatch,
                                TblMarkerType = 0,
                            };

                            SavingCuttingOrder(cuttingRow);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                status = 0;
            }
        }



    }
}