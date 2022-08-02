using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        private class FirstDegreeQty
        {
            public decimal LineNum { get; set; }

            public float Qty { get; set; }

            public float TotalLineNumQty { get; set; }

            public string Location { get; set; }
        }

        [OperationContract]
        public List<Tbl_fabricInspectionDetail> fabricInspectionDetail(int inspectionHeader)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabricinspection = (from i in entities.Tbl_fabricInspectionDetail.Include("Tbl_fabricInspectionDetailDefects")
                                        where i.Tbl_fabricInspectionHeader == inspectionHeader
                                        select i).ToList();
                return fabricinspection.ToList();
            }
        }

        #region AutoComplete

        [OperationContract]
        public List<Brand> Brand()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var brand = (from b in entities.Brands
                             join c in entities.TblColorLinks
                             on b.Brand_Code equals c.TblBrand
                             select b).Distinct().ToList();
                return brand;
            }
        }

        [OperationContract]
        public List<FabricB> Fabric()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabric = (from s in entities.FabricBs
                              where s.Fabric_FabricType != "OPERATION"
                              select s).ToList();
                return fabric;
            }
        }

        #endregion AutoComplete

        [OperationContract]
        public List<tbl_WF_Defects> Defects()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var defects = (from d in entities.tbl_WF_Defects
                               select d).ToList();
                return defects;
            }
        }

        [OperationContract]
        private void DeleteInspectionRow(int Iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var detailsQuery = (from s in entities.Tbl_fabricInspectionDetail
                                    where s.Iserial == Iserial
                                    select s).SingleOrDefault();

                entities.Tbl_fabricInspectionDetail.DeleteObject(detailsQuery);

                entities.SaveChanges();
            }
        }

        [OperationContract]
        private void DeleteFabricInspection(int Iserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var headerQuery = (from h in entities.Tbl_fabricInspectionHeader
                                   where h.Iserial == Iserial
                                   select h).SingleOrDefault();

                if (headerQuery != null) entities.Tbl_fabricInspectionHeader.DeleteObject(headerQuery);
                entities.SaveChanges();
            }
        }

        [OperationContract]
        public void UpdateInspectionOrder(Tbl_fabricInspectionHeader transactionHeader, List<Tbl_fabricInspectionDetail> details, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var TransHeader = entities.Tbl_fabricInspectionHeader.FirstOrDefault(wde => wde.Iserial == transactionHeader.Iserial);
                if (TransHeader!=null)
                {
                    TransHeader.Notes = transactionHeader.Notes;
                }

                if (TransHeader.PostedToAx == true) 
                {
                    throw new Exception("you cannot Modify Posted Trasaction");
                }
               
                foreach (var item in details.ToList())
                {
                    var OldRow = entities.Tbl_fabricInspectionDetail.FirstOrDefault(w => w.Iserial == item.Iserial);
                    if (entities.Tbl_Wf_CuttingOrder.Any(w=>w.InspectionIserial== OldRow.Iserial))
                    {
                        throw new  Exception("Marker Exisit");
                    }
                    if (OldRow != null)
                    {
                        OldRow.M2WeightGm = item.M2WeightGm;
                        OldRow.RollWMT = item.RollWMT;
                        OldRow.NetRollWMT = item.NetRollWMT;
                        OldRow.ConsPerPC = item.ConsPerPC;
                        OldRow.NoofPCs = item.NoofPCs;
                        OldRow.StoreRollQty = item.StoreRollQty;
                        OldRow.RemainingMarkerRollQty = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);
                        OldRow.RemainingReservationRollQty = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);
                        OldRow.QtyInspected = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);

                        foreach (var deftsItes in item.Tbl_fabricInspectionDetailDefects)
                        {
                            var OldDefectRow = entities.Tbl_fabricInspectionDetailDefects.FirstOrDefault(w => w.Iserial == deftsItes.Iserial && w.DefectIserial == deftsItes.DefectIserial && w.Tbl_fabricInspectionDetail == item.Iserial);
                            if(OldDefectRow!=null)
                            OldDefectRow.DefectValue = deftsItes.DefectValue;
                        }
                    }
                }
               
                entities.SaveChanges();
                CreateAxBarcode(transactionHeader.Iserial, 1, userIserial, transactionHeader.TransactionType);
            }
        }

        [OperationContract]
        public List<Tbl_fabricInspectionDetail> SaveInspectionOrder(Tbl_fabricInspectionHeader transactionHeader, List<Tbl_fabricInspectionDetail> details, int userIserial)
        {
            double fabricIssueQty = 0;
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabrics = details.Select(x => x.Fabric_Code).Distinct();
                transactionHeader.FabricCode = fabrics.FirstOrDefault();
                //foreach (var fabric in fabrics)
                //{
                //    var reservationList = entities.Tbl_ReservationMainDetails.Include("Tbl_ReservationDetails").Where(x => x.Tbl_ReservationHeader1.TransOrder ==
                //          transactionHeader.TransOrder && x.Fabric == fabric).ToList();

                //    foreach (var reservation in reservationList)
                //    {
                //        foreach (var reservationDetail in reservation.Tbl_ReservationDetails.ToList())
                //        {
                //            var salesorder = entities.TblSalesOrders.FirstOrDefault(x => x.SalesOrderCode == reservationDetail.SalesOrder && x.SalesOrderType == 2).Iserial;
                //            var fabriccolor =
                //                entities.TblColors.FirstOrDefault(
                //                    x => x.Code == reservation.FabricColor && x.TblLkpColorGroup != 24).Iserial;

                //            var sum = entities.RouteCardFabrics.Where(x => x.TblSalesOrder == salesorder && x.IsFree && x.ItemId == reservation.Fabric
                //                                                           && x.FabricColor == fabriccolor).Sum(x => x.Qty);
                //            if (sum != null)
                //                fabricIssueQty =
                //                    (double)sum;
                //        }
                //    }
                //}
            }

            var firstDegreeQty = details.Where(x => x.Degree == 1).GroupBy(x => x.LineNum)
                .Select(t => new FirstDegreeQty
                {
                    LineNum = t.Key,
                    Qty = t.Sum(w => w.StoreRollQty),
                    TotalLineNumQty = (float)(t.FirstOrDefault().TotalLineNumQty - fabricIssueQty),
                    Location = t.FirstOrDefault().FinishedWarehouse,
                });
            int maxroll = 0;

            var transactionGuid = Guid.NewGuid().ToString();
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var firstdetail = details.FirstOrDefault();
                if (entities.Tbl_fabricInspectionDetail.Any(x => x.Fabric_Code == firstdetail.Fabric_Code && x.ColorCode == firstdetail.ColorCode))
                {
                    maxroll =
                        entities.Tbl_fabricInspectionDetail
                            .Where(
                                x => x.Fabric_Code == firstdetail.Fabric_Code && x.ColorCode == firstdetail.ColorCode).OrderByDescending(x => x.RollNo)
                            .Max(x => x.RollNo);
                }
                if (transactionHeader.Iserial == 0)
                {
                    transactionHeader.PostedToAx = false;
                    entities.AddToTbl_fabricInspectionHeader(transactionHeader);
                    entities.SaveChanges();
                }
                else
                {
                    var fabricInspectionHeaderOldRow = (from h in entities.Tbl_fabricInspectionHeader
                                                        where (h.Iserial == transactionHeader.Iserial)
                                                        select h).SingleOrDefault();

                    transactionHeader.PostedToAx = false;
                    GenericUpdate(fabricInspectionHeaderOldRow, transactionHeader, entities);
                }
                foreach (var item in details)
                {
                    if (item.Iserial != 0 && item.Tbl_fabricInspectionHeader != 0)
                    {
                        var detailRowOld = (from f in entities.Tbl_fabricInspectionDetail
                                            where f.Iserial == item.Iserial && f.Tbl_fabricInspectionHeader == item.Tbl_fabricInspectionHeader
                                            select f).SingleOrDefault();
                        GenericUpdate(detailRowOld, item, entities);
                    }
                    else
                    {
                        string locTemp = null;
                        switch (item.Degree)
                        {
                            case 1:
                                //    locTemp = wmsLoc.INSPECTIONLOC1;
                                break;

                            case 2:
                                locTemp = entities.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "FabricWarehouse2ndDCode").sSetupValue;
                                break;

                            case 3:
                                locTemp = entities.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "FabricWarehouse3rdDCode").sSetupValue;
                                break;
                        }
                        if (item.Degree != 1)
                        {
                            item.FinishedLocation = locTemp;
                            item.FinishedWarehouse = locTemp;
                        }
                        else
                        {
                            item.FinishedLocation = item.FinishedWarehouse;
                            item.FinishedWarehouse = item.FinishedWarehouse;
                        }

                        item.RollNo = item.RollNo + maxroll;
                        // type equal purchase order
                        if (item.Degree != 1 && transactionHeader.TransactionType == 0)
                        {
                           // InsertpackingSlipData(item.LineNum, item.FinishedLocation, transactionGuid,
                           //item.StoreRollQty * -1, transactionHeader.Iserial, userIserial);
                            //var line = PurchaseFabricLinesToAx(item, transactionHeader.TransOrder, userIserial);
                            //item.LineNum = line;
                            //CreateOrUpdateMiscCharge(transactionHeader.TransOrder, item.LineNum, item.UnitPrice * -1, true, userIserial);
                            //InsertpackingSlipData(item.LineNum, item.FinishedLocation, transactionGuid,
                            //item.StoreRollQty, transactionHeader.Iserial, userIserial);
                        }

                        item.RemainingMarkerRollQty = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);
                        item.RemainingReservationRollQty = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);
                        item.QtyInspected = item.StoreRollQty - (item.Tbl_fabricInspectionDetailDefects.Sum(x => x.DefectValue)) * (item.ConsPerPC / item.NoofPCs);
                        item.Tbl_fabricInspectionHeader1 = null;
                        entities.AddToTbl_fabricInspectionDetail(item);
                        item.Tbl_fabricInspectionHeader = transactionHeader.Iserial;
                    }

                    entities.SaveChanges();
                    foreach (var items in item.Tbl_fabricInspectionDetailDefects.ToList())
                    {
                        var fabricInspectionDetailDefectsQuery = (from d in entities.Tbl_fabricInspectionDetailDefects
                                                                  where d.DefectIserial == items.DefectIserial
                                                                  && d.Tbl_fabricInspectionDetail == item.Iserial
                                                                  select d).SingleOrDefault();
                        if (fabricInspectionDetailDefectsQuery != null)
                        {
                            fabricInspectionDetailDefectsQuery.DefectValue = items.DefectValue;
                        }
                        entities.SaveChanges();
                    }
                }

                //CreatePackingSlip(String.Format("Insp_{0}_1", transactionHeader.Iserial), transactionHeader.TransOrder, transactionGuid, userIserial);

                transactionGuid = Guid.NewGuid().ToString();
                if (transactionHeader.TransactionType == 0)
                {
                    foreach (var f in firstDegreeQty)
                    {
                        var firstQty = f.Qty;
                        // var secondQty = notFirstDegreeQty.SingleOrDefault(x => x.Key == f.LineNum).Value;

                        var total = firstQty - f.TotalLineNumQty;// - secondQty;
                        if (total != 0)
                        {
                            InsertpackingSlipData(f.LineNum, f.Location, transactionGuid,
                              total, transactionHeader.Iserial, userIserial);
                        }
                    }
                }
                //if (SharedOperation.UseAx())
                //{
                //    if (transactionHeader.VendorSubtraction)
                //    {
                //        CreatePackingSlip(String.Format("Insp_{0}_2", transactionHeader.Iserial),
                //                transactionHeader.TransOrder, transactionGuid, userIserial);
                //    }
                //}
                CreateAxBarcode(transactionHeader.Iserial, 1, userIserial, transactionHeader.TransactionType);
                return details;
            }
        }

        [OperationContract]
        public void InsertInspectionHeaderTemp(Tbl_fabricInspectionHeader transactionHeader, List<Tbl_fabricInspectionDetail> details, int userIserial)
        {
            var transactionGuid = Guid.NewGuid().ToString();
            //CreatePackingSlip(String.Format("Insp_{0}_1", transactionHeader.Iserial), transactionHeader.TransOrder, transactionGuid, userIserial);

            var firstDegreeQty = details.Where(x => x.Degree == 1).GroupBy(x => x.LineNum)
               .Select(t => new FirstDegreeQty
               {
                   LineNum = t.Key,
                   Qty = t.Sum(w => w.StoreRollQty),
                   TotalLineNumQty = (float)(t.FirstOrDefault().TotalLineNumQty),
                   Location = t.FirstOrDefault().FinishedWarehouse,
               });
            if (transactionHeader.TransactionType == 0)
            {
                foreach (var f in firstDegreeQty)
                {
                    var firstQty = f.Qty;
                    // var secondQty = notFirstDegreeQty.SingleOrDefault(x => x.Key == f.LineNum).Value;

                    var total = firstQty - f.TotalLineNumQty;// - secondQty;
                    if (total != 0)
                    {
                        InsertpackingSlipData(f.LineNum, f.Location, transactionGuid,
                          total, transactionHeader.Iserial, userIserial);
                    }
                }
            }
            //foreach (var item in details)
            //{
            //    //if (item.Degree != 1 && transactionHeader.TransactionType == 0)
            //    //{
            //    //    InsertpackingSlipData(item.LineNum, item.FinishedLocation, transactionGuid,
            //    //        item.StoreRollQty*-1, transactionHeader.Iserial, userIserial);
            //    //    var line = PurchaseFabricLinesToAx(item, transactionHeader.TransOrder, userIserial);
            //    //    item.LineNum = line;
            //    //    CreateOrUpdateMiscCharge(transactionHeader.TransOrder, item.LineNum, item.UnitPrice*-1, true,
            //    //        userIserial);
            //    //    InsertpackingSlipData(item.LineNum, item.FinishedLocation, transactionGuid,
            //    //        item.StoreRollQty, transactionHeader.Iserial, userIserial);
            //    //}
            //}

            //if (transactionHeader.VendorSubtraction)
            //{
            //    CreatePackingSlip(String.Format("Insp_{0}_2", transactionHeader.Iserial),
            //            transactionHeader.TransOrder, transactionGuid, userIserial);
            //}
            CreateAxBarcode(transactionHeader.Iserial, 1, userIserial, transactionHeader.TransactionType);
        }

        [OperationContract]
        private List<Tbl_fabricInspectionHeader> FabricInspectionHeaderList(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
        //    CreateAxBarcode(21124, 1, 1180, 0);
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<Tbl_fabricInspectionHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.Tbl_fabricInspectionHeader.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.Tbl_fabricInspectionHeader.Include("Tbl_fabricInspectionDetail").Where(filter, parameterCollection.ToArray()).OrderBy(sort);
                }
                else
                {
                    fullCount = context.Tbl_fabricInspectionHeader.Count();
                    query = context.Tbl_fabricInspectionHeader.Include("Tbl_fabricInspectionDetail").OrderBy(sort);
                }
                string batchno = "";
                string fabricColor = "";
                if (valuesObjects != null)
                {
                    if (valuesObjects.Any(x => x.Key == "BatchNo0"))
                    {
                        batchno = valuesObjects.FirstOrDefault(x => x.Key == "BatchNo0").Value.ToString().ToLower();
                    }

                    if (valuesObjects.Any(x => x.Key == "ColorCode0"))
                    {
                        fabricColor = valuesObjects.FirstOrDefault(x => x.Key == "ColorCode0").Value.ToString().ToLower();
                    }
                }
                if (!string.IsNullOrWhiteSpace(batchno)|| !string.IsNullOrWhiteSpace(fabricColor))
                {
                    query = query.Where(x => x.Tbl_fabricInspectionDetail.Any(w => w.BatchNo.ToLower().Contains(batchno)&&w.ColorCode.ToLower().Contains(fabricColor)));
                }
                return query.Skip(skip).Take(take).ToList();
            }
        }

        [OperationContract]
        public List<Tbl_fabricInspectionDetail> PurchaseInspectionLineNum(string purchaseOrder)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var fabricinspection = (from i in entities.Tbl_fabricInspectionDetail
                                        where i.Tbl_fabricInspectionHeader1.TransOrder == purchaseOrder
                                        select i).ToList();
                return fabricinspection;
            }
        }

        [OperationContract]
        public List<FabricInventDimDetail> FabricInventDimDetails(string dataArea, string purchaseOrder, ObservableCollection<decimal> lineNumbers)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.FabricInventDimDetails.MergeOption = MergeOption.NoTracking;

                var temp = from x in entities.FabricInventDimDetails
                           where (x.PURCHID == purchaseOrder
                           && x.DATAAREAID == dataArea)
                           select x;

                var fabricInventList = temp;
                if (lineNumbers != null)
                {
                    fabricInventList = temp.Where(x => lineNumbers.Any(l => x.LINENUM != l));
                }

                return fabricInventList.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public string CreateAxBarcode(int Iserial, int post, int userIserial, int transactionType)
        {

            var axapta = new Axapta();

            if (SharedOperation.UseAx())
            {


                var credential = new NetworkCredential("bcproxy", "around1");
                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }

                if (userToLogin != null)
                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
            }
           
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.CommandTimeout = 0;
                if (!SharedOperation.UseAx())
                { 
                    entities.CreateRollBarcode(Iserial);
                }
                if (transactionType == 0)
                {
                    entities.FabricInspectionPurchaseOrders.MergeOption = MergeOption.NoTracking;
                    var listToPost = (from s in entities.FabricInspectionPurchaseOrders
                                      where s.HeaderIserial == Iserial
                                      orderby s.LINENUM
                                      select s).ToList();

                    var fabricInspectionPurchaseOrder = listToPost.FirstOrDefault();
                    var recList = new List<Tbl_ReservationRec>();
                    if (fabricInspectionPurchaseOrder != null)
                    {
                        var transOrder = fabricInspectionPurchaseOrder.TransOrder;

                        var distinctList = listToPost.GroupBy(x => new { x.Fabric_Code, x.ColorCode }).Select(w => new SalesOrderDto
                        {
                            SalesOrder = w.Key.Fabric_Code,
                            SalesOrderColor = w.Key.ColorCode
                        });
                        var resDetails = (from r in entities.Tbl_ReservationDetails.Include("Tbl_ReservationMainDetails1")
                                          where r.Tbl_ReservationMainDetails1.Tbl_ReservationHeader1.TransOrder == transOrder
                                          orderby r.IntialQty descending
                                          select r);

                        var listToPostTemp = listToPost;
                        foreach (var row in resDetails)
                        {
                            if (distinctList.Any(x => x.SalesOrder == row.SalesOrder && x.SalesOrderColor == row.SalesOrderColor))
                            {
                                row.Tbl_ReservationMainDetails1.Inspected = true;
                                var qtyReq = row.IntialQty;
                                float finalModified;
                                var totalRoll =
                                    listToPostTemp.Where(
                                        x =>
                                            x.Fabric_Code == row.Tbl_ReservationMainDetails1.Fabric &&
                                            x.BatchNo == row.Tbl_ReservationMainDetails1.Batchno
                                            && x.ColorCode == row.Tbl_ReservationMainDetails1.FabricColor &&
                                            x.LINENUM == row.Tbl_ReservationMainDetails1.LineNum && x.RemainingReservationRollQty > 0).Sum(x => x.RemainingReservationRollQty);
                                var percentage = (double)(totalRoll / qtyReq);
                                var maxPercentage =
                                    entities.tblChainSetups.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "MaxReservationTolerancePercentage").sSetupValue;

                                var minPercentage =
                                   entities.tblChainSetups.FirstOrDefault(
                                       x => x.sGlobalSettingCode == "MinReservationTolerancePercentage").sSetupValue;
                                var dblmaxPercentage = Convert.ToDouble(maxPercentage);
                                var dblminPercentage = Convert.ToDouble(minPercentage);
                                if (percentage < dblmaxPercentage && percentage > dblminPercentage)
                                {
                                    finalModified = (float)(qtyReq * percentage);
                                    qtyReq = (float)(qtyReq * percentage);
                                }
                                else
                                {
                                    finalModified = qtyReq * 1;
                                    qtyReq = qtyReq * 1;
                                }

                                if (percentage < dblminPercentage)
                                {
                                    //       entities.Tbl_ReservationDetails.DeleteObject(row);
                                    //        entities.SaveChanges();
                                }
                                // if percentage =>1 then decrease from the rolls normally
                                //else qtyreq *percentage
                                foreach (var inspectionRow in listToPostTemp.Where(x => x.Fabric_Code == row.Tbl_ReservationMainDetails1.Fabric && x.BatchNo == row.Tbl_ReservationMainDetails1.Batchno
                                                                                   && x.ColorCode == row.Tbl_ReservationMainDetails1.FabricColor && x.LINENUM == row.Tbl_ReservationMainDetails1.LineNum && x.RemainingReservationRollQty > 0).ToList())
                                {
                                    //var percentage = (inspectionRow.NetRollQty / inspectionRow.StoreRollQty);

                                    var singleOrDefault = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == inspectionRow.FinishedWarehouse);
                                    if (singleOrDefault != null)
                                    {
                                        var siteFinishedWarehouse = singleOrDefault.INVENTSITEID;

                                        //  finalModified = finalModified + (float)(qtyReq * percentage);
                                        if (qtyReq > 0)
                                        {
                                            if (qtyReq >= inspectionRow.RemainingReservationRollQty)
                                            {
                                                qtyReq = qtyReq - inspectionRow.RemainingReservationRollQty;
                                            }
                                            else
                                            {
                                                inspectionRow.RemainingReservationRollQty = qtyReq;
                                                qtyReq = 0;
                                            }
                                            var reservationRec = new Tbl_ReservationRec
                                            {
                                                RollNo = inspectionRow.RollNo,
                                                Tbl_FabricInspectionDetails = inspectionRow.Iserial,
                                                Item = inspectionRow.Fabric_Code,
                                                ItemColor = inspectionRow.ColorCode,
                                                Location = inspectionRow.FinishedWarehouse,
                                                Warehouse = inspectionRow.FinishedWarehouse,
                                                Site = siteFinishedWarehouse,
                                                Qty = inspectionRow.RemainingReservationRollQty,
                                                Tbl_ReservationDetails = row.Iserial,
                                            };

                                            recList.Add(reservationRec);
                                            entities.Tbl_ReservationRec.AddObject(reservationRec);
                                            inspectionRow.RemainingReservationRollQty = inspectionRow.RemainingReservationRollQty - qtyReq;
                                        }
                                    }
                                }
                                row.FinalQty = finalModified;
                            }
                        }
                    }
                    var retval = "";
                    if (SharedOperation.UseAx())
                    {
                        foreach (var item in listToPost)
                        {
                            var siteFinishedWarehouse = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == item.FinishedWarehouse);
                            var axRecord = axapta.CreateAxaptaRecord("CLEBarcodeBCreate");
                            axRecord.Clear();
                            axRecord.InitValue();
                            axRecord.set_Field("REFLINE", Convert.ToSingle(item.Iserial));
                            axRecord.set_Field("ROLLENUM", item.RollNo);
                            axRecord.set_Field("REFDATE", item.TransDate);
                            axRecord.set_Field("REFID", item.TransOrder);
                            axRecord.set_Field("ITEMID", item.Fabric_Code);
                            axRecord.set_Field("ROLEQTY", Convert.ToSingle(item.StoreRollQty));
                            axRecord.set_Field("INSPECTIONNUM", item.HeaderIserial);
                            axRecord.set_Field("REFTYPE", item.TransactionType);
                            if (siteFinishedWarehouse != null)
                            {
                                axRecord.set_Field("FROMSITE", siteFinishedWarehouse.INVENTSITEID);
                                axRecord.set_Field("TOSITE", siteFinishedWarehouse.INVENTSITEID);
                            }
                            axRecord.set_Field("FROMLOCATION", item.FinishedWarehouse);
                            axRecord.set_Field("TOLOCATION", item.FinishedWarehouse);
                            axRecord.set_Field("FROMCONFIG", item.ColorCode);
                            axRecord.set_Field("TOCONFIG", item.ColorCode);
                            axRecord.set_Field("FROMBATCH", item.BatchNo);
                            axRecord.set_Field("TOBATCH", item.BatchNo);
                            axRecord.set_Field("TOWAREHOUSE", item.FinishedWarehouse);
                            axRecord.set_Field("FROMWAREHOUSE", item.FinishedWarehouse);
                            axRecord.set_Field("DATAAREAID", "Ccm");
                            axRecord.Insert();
                        }


                        var import = axapta.CreateAxaptaObject("CLEAllTransfer");
                        retval = import.Call("run", Iserial, post).ToString();
                    }

                    var fabricInspectionHeader = (from s in entities.Tbl_fabricInspectionHeader
                                                  where s.Iserial == Iserial
                                                  select s).SingleOrDefault();
                    if (post == 1)
                    {
                        if (fabricInspectionHeader != null) fabricInspectionHeader.PostedToAx = true;
                    }
                    if (fabricInspectionHeader != null) fabricInspectionHeader.AxTransferTransaction = retval;

                    foreach (var resRecRow in recList)
                    {
                        resRecRow.BatchNo = GetBarcodeFromAx(resRecRow.Tbl_FabricInspectionDetails);
                        var inspectionRow = entities.Tbl_fabricInspectionDetail.SingleOrDefault(x => x.Iserial == resRecRow.Tbl_FabricInspectionDetails);
                        if (inspectionRow != null)
                            inspectionRow.RemainingReservationRollQty = inspectionRow.RemainingReservationRollQty -
                                                                        resRecRow.Qty;
                    }
                }
                else if (transactionType == 2 || transactionType == 1)
                {
                    entities.FabricInspectionPurchaseOrders.MergeOption = MergeOption.NoTracking;
                    var listToPost = (from s in entities.FabricInspectionJournals
                                      where s.HeaderIserial == Iserial
                                      orderby s.LINENUM
                                      select s).ToList();
                    var fabricInspectionPurchaseOrder = listToPost.FirstOrDefault();
                    var recList = new List<Tbl_ReservationRec>();
                    if (fabricInspectionPurchaseOrder != null)
                    {
                        var transOrder = fabricInspectionPurchaseOrder.TransOrder;
                        var distinctList = listToPost.GroupBy(x => new { x.Fabric_Code, x.ColorCode }).Select(w => new SalesOrderDto
                        {
                            SalesOrder = w.Key.Fabric_Code,
                            SalesOrderColor = w.Key.ColorCode
                        });
                        var resDetails = (from r in entities.Tbl_ReservationDetails.Include("Tbl_ReservationMainDetails1")
                                          where r.Tbl_ReservationMainDetails1.Tbl_ReservationHeader1.TransOrder == transOrder
                                          orderby r.IntialQty descending
                                          select r);
                        var listToPostTemp = listToPost;
                        foreach (var row in resDetails)
                        {
                            if (distinctList.Any(x => x.SalesOrder == row.SalesOrder && x.SalesOrderColor == row.SalesOrderColor))
                            {
                                row.Tbl_ReservationMainDetails1.Inspected = true;
                                var qtyReq = row.IntialQty;
                                float finalModified;
                                var totalRoll =
                                    listToPostTemp.Where(
                                        x =>
                                            x.Fabric_Code == row.Tbl_ReservationMainDetails1.Fabric &&
                                            x.BatchNo == row.Tbl_ReservationMainDetails1.Batchno
                                            && x.ColorCode == row.Tbl_ReservationMainDetails1.FabricColor &&
                                            x.LINENUM == row.Tbl_ReservationMainDetails1.LineNum && x.RemainingReservationRollQty > 0).Sum(x => x.RemainingReservationRollQty);
                                var percentage = (double)(totalRoll / qtyReq);
                                var maxPercentage =
                                    entities.tblChainSetups.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "MaxReservationTolerancePercentage").sSetupValue;
                                var minPercentage =
                                   entities.tblChainSetups.FirstOrDefault(
                                       x => x.sGlobalSettingCode == "MinReservationTolerancePercentage").sSetupValue;
                                var dblmaxPercentage = Convert.ToDouble(maxPercentage);
                                var dblminPercentage = Convert.ToDouble(minPercentage);
                                if (percentage < dblmaxPercentage && percentage > dblminPercentage)
                                {
                                    finalModified = (float)(qtyReq * percentage);
                                    qtyReq = (float)(qtyReq * percentage);
                                }
                                else
                                {
                                    finalModified = qtyReq * 1;
                                    qtyReq = qtyReq * 1;
                                }

                                if (percentage < dblminPercentage)
                                {
                                    //       entities.Tbl_ReservationDetails.DeleteObject(row);
                                    //        entities.SaveChanges();
                                }
                                // if percentage =>1 then decrease from the rolls normally
                                //else qtyreq *percentage
                                foreach (var inspectionRow in listToPostTemp.Where(x => x.Fabric_Code == row.Tbl_ReservationMainDetails1.Fabric && x.BatchNo == row.Tbl_ReservationMainDetails1.Batchno
                                                                                   && x.ColorCode == row.Tbl_ReservationMainDetails1.FabricColor && x.LINENUM == row.Tbl_ReservationMainDetails1.LineNum && x.RemainingReservationRollQty > 0).ToList())
                                {
                                    //var percentage = (inspectionRow.NetRollQty / inspectionRow.StoreRollQty);

                                    var singleOrDefault = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == inspectionRow.FinishedWarehouse);
                                    if (singleOrDefault != null)
                                    {
                                        var siteFinishedWarehouse = singleOrDefault.INVENTSITEID;

                                        //  finalModified = finalModified + (float)(qtyReq * percentage);
                                        if (qtyReq > 0)
                                        {
                                            if (qtyReq >= inspectionRow.RemainingReservationRollQty)
                                            {
                                                qtyReq = qtyReq - inspectionRow.RemainingReservationRollQty;
                                            }
                                            else
                                            {
                                                inspectionRow.RemainingReservationRollQty = qtyReq;
                                                qtyReq = 0;
                                            }
                                            var reservationRec = new Tbl_ReservationRec
                                            {
                                                RollNo = inspectionRow.RollNo,
                                                Tbl_FabricInspectionDetails = inspectionRow.Iserial,
                                                Item = inspectionRow.Fabric_Code,
                                                ItemColor = inspectionRow.ColorCode,
                                                Location = inspectionRow.FinishedWarehouse,
                                                Warehouse = inspectionRow.FinishedWarehouse,
                                                Site = siteFinishedWarehouse,
                                                Qty = inspectionRow.RemainingReservationRollQty,
                                                Tbl_ReservationDetails = row.Iserial,
                                            };

                                            recList.Add(reservationRec);
                                            entities.Tbl_ReservationRec.AddObject(reservationRec);
                                            inspectionRow.RemainingReservationRollQty = inspectionRow.RemainingReservationRollQty - qtyReq;
                                        }
                                    }
                                }
                                row.FinalQty = finalModified;
                            }
                        }
                    }

                    foreach (var item in listToPost)
                    {
                        var siteFinishedWarehouse = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == item.FinishedWarehouse);

                        var axRecord = axapta.CreateAxaptaRecord("CLEBarcodeBCreate");
                        axRecord.Clear();
                        axRecord.InitValue();
                        axRecord.set_Field("REFLINE", Convert.ToSingle(item.Iserial));
                        axRecord.set_Field("ROLLENUM", item.RollNo);
                        axRecord.set_Field("REFDATE", item.TransDate);
                        axRecord.set_Field("REFID", item.TransOrder);
                        axRecord.set_Field("ITEMID", item.Fabric_Code);
                        axRecord.set_Field("ROLEQTY", Convert.ToSingle(item.StoreRollQty));
                        axRecord.set_Field("INSPECTIONNUM", item.HeaderIserial);
                        axRecord.set_Field("REFTYPE", item.TransactionType);
                        if (siteFinishedWarehouse != null)
                        {
                            axRecord.set_Field("FROMSITE", siteFinishedWarehouse.INVENTSITEID);
                            axRecord.set_Field("TOSITE", siteFinishedWarehouse.INVENTSITEID);
                        }
                        axRecord.set_Field("FROMLOCATION", item.OrgLocation.Trim());
                        axRecord.set_Field("TOLOCATION", item.FinishedWarehouse.Trim());
                        axRecord.set_Field("FROMCONFIG", item.ColorCode.Trim());
                        axRecord.set_Field("TOCONFIG", item.ColorCode.Trim());
                        axRecord.set_Field("FROMBATCH", item.BatchNo.Trim());
                        axRecord.set_Field("TOBATCH", item.BatchNo.Trim());
                        axRecord.set_Field("TOWAREHOUSE", item.FinishedWarehouse.Trim());
                        axRecord.set_Field("FROMWAREHOUSE", item.OrgLocation.Trim());
                        axRecord.set_Field("DATAAREAID", "Ccm");
                        axRecord.Insert();
                    }
                    var import = axapta.CreateAxaptaObject("CLEAllTransfer");
                    //int cc = Iserial;
                    // Post = 0;
                    var retval = import.Call("run", Iserial, post);
                    var fabricInspectionHeader = (from s in entities.Tbl_fabricInspectionHeader
                                                  where s.Iserial == Iserial
                                                  select s).SingleOrDefault();
                    if (post == 1)
                    {
                        if (fabricInspectionHeader != null) fabricInspectionHeader.PostedToAx = true;
                    }
                    if (fabricInspectionHeader != null) fabricInspectionHeader.AxTransferTransaction = retval.ToString();

                    foreach (var resRecRow in recList)
                    {
                        resRecRow.BatchNo = GetBarcodeFromAx(resRecRow.Tbl_FabricInspectionDetails);

                        var inspectionRow = entities.Tbl_fabricInspectionDetail.SingleOrDefault(x => x.Iserial == resRecRow.Tbl_FabricInspectionDetails);
                        if (inspectionRow != null)
                            inspectionRow.RemainingReservationRollQty = inspectionRow.RemainingReservationRollQty - resRecRow.Qty;
                    }
                }
                entities.SaveChanges();

                if (SharedOperation.UseAx())
                {
                    axapta.Logoff();
                    entities.CreateRollBarcode(Iserial);
                }
            }
            return "Posted Sucessfully Done";
        }

  

        [OperationContract]
        public List<string> InventoryReservedJournalsDetail(string dataArea)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var inventoryReservedJournal = entities.InventoryReservedJournalsDetails.Where(
                x => x.DATAAREAID == dataArea).Select(x => x.JOURNALID).Distinct() //&& X.JOURNALID == Journalid && X.ITEMID == FabricCode)
                .ToList();
                return inventoryReservedJournal;
            }
        }

        [OperationContract]
        public string GetBarcodeFromAx(int? inspectionRow)
        {
            if (SharedOperation.UseAx())
            {
                using (var entities = new ax2009_ccEntities())
                {
                    entities.CLEBARCODEBCREATEs.MergeOption = MergeOption.NoTracking;
                    return
                        entities.CLEBARCODEBCREATEs.FirstOrDefault(x => x.DATAAREAID == "ccm" &&
                          x.REFLINE == inspectionRow).BARCODE;
                }


            }
            else {
                using (var workflow = new WorkFlowManagerDBEntities())
                {
                    return workflow.TblRollBarcodes.Where(wde => wde.Tbl_fabricInspectionDetail == inspectionRow).FirstOrDefault().Barcode;
                }
            }
        }

        //private void CreatePackingSlip(string packingSlipId, string purchId, string transactionGuid, int userIserial)
        //{
        //    if (SharedOperation.UseAx())
        //    {
        //        var axapta = new Axapta();
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        if (userToLogin != null)
        //            axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

        //        var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");

        //        importNew.Call("PostPurchFormLetter", packingSlipId, purchId, transactionGuid);

        //        axapta.Logoff();
        //    }
        //}

        //private void CreateOrUpdateMiscCharge(string purchId, decimal lineNum, float value, bool createOk, int userIserial)
        //{
        //    if (SharedOperation.UseAx())
        //    {
        //        var axapta = new Axapta();
        //        var credential = new NetworkCredential("bcproxy", "around1");
        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        if (userToLogin != null)
        //        {
        //            axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
        //        }
        //        var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
        //        //(packingSlipId packingSlipId,PurchId PurchId,str 200 TransactionGuid)
        //        importNew.Call("CreateOrUpdateMiscCharge", purchId, lineNum, value, createOk);
        //        axapta.Logoff();
        //    }
        //}

        private void InsertpackingSlipData(decimal lineNum, string location, string transactionGuid, float qty, int fabricInspectionHeader, int userIserial)
        {
            if (SharedOperation.UseAx())
            {
                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");
                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                if (userToLogin != null)
                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                const string tableName = "AutoPICKING";
                var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                axaptaRecord.Clear();
                axaptaRecord.InitValue();
                axaptaRecord.set_Field("DATAAREAID", "Ccm");
                axaptaRecord.set_Field("TransactionGuid", transactionGuid);
                axaptaRecord.set_Field("FABRICLOCATION", location);
                axaptaRecord.set_Field("QTY", qty);
                axaptaRecord.set_Field("WORKFLOWJOURID", fabricInspectionHeader);
                axaptaRecord.set_Field("LineNum", lineNum);
                axaptaRecord.Insert();
            }
        }

        //public decimal PurchaseFabricLinesToAx(Tbl_fabricInspectionDetail item, string purchaseOrder, int userIserial)
        //{
        //    using (var axapta = new Axapta())
        //    {
        //        var credential = new NetworkCredential("bcproxy", "around1");

        //        TblAuthUser userToLogin;
        //        using (var model = new WorkFlowManagerDBEntities())
        //        {
        //            userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
        //        }
        //        axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

        //        var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
        //        var lineNum = importNew.Call("CreatePurchaseLines", purchaseOrder, item.Fabric_Code, item.StoreRollQty, item.BatchNo,
        //            item.ColorCode, item.FinishedWarehouse, item.UnitPrice, item.FinishedLocation, item.FinishedSite);

        //        axapta.Logoff();
        //        return Convert.ToDecimal(lineNum);
        //    }
        //}

        [OperationContract]
        public List<InventoryReservedJournalsDetail> InventoryReservedJournalsDetailPerJournal(string dataArea, string journalid, IEnumerable<decimal> lineNumbers)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var inventoryReservedJournal = entities.InventoryReservedJournalsDetails.Where(
                x => x.DATAAREAID == dataArea && x.JOURNALID == journalid
                    && lineNumbers.Any(s => s != x.LINENUM))
                .ToList();
                return inventoryReservedJournal;
            }
        }

        #region Printing

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PrintingFabricDefect> PrintingFabricDefects(int Iserial, out List<DefectsData> DefectsList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.PrintingFabricDefects.MergeOption = MergeOption.NoTracking;
                entities.DefectsDatas.MergeOption = MergeOption.NoTracking;
                var printingList = from s in entities.PrintingFabricDefects
                                   where s.Tbl_fabricInspectionHeader == Iserial
                                   select s;

                DefectsList = (from d in entities.DefectsDatas
                               where d.HeaderIserial == Iserial
                               select d).ToList();

                return printingList.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public PrintingFabricDefect PrintingFabricDefectsAll(out List<DefectsData> DefectsList)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                entities.PrintingFabricDefects.MergeOption = MergeOption.NoTracking;
                var tblFabricInspectionHeader = (from s in entities.PrintingFabricDefects
                                                 select s.Iserial).FirstOrDefault();
                var printingList = (from s in entities.PrintingFabricDefects
                                    where s.Tbl_fabricInspectionHeader == tblFabricInspectionHeader
                                    select s).FirstOrDefault();

                entities.DefectsDatas.MergeOption = MergeOption.NoTracking;
                var Iserial = printingList.Iserial;
                DefectsList = (from d in entities.DefectsDatas
                               where d.HeaderIserial == Iserial
                               select d).ToList();

                return printingList;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<BarCodePrintLayout> BarCodePrintLayoutOperation(int operation, string code)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var printingListLayout = (from s in entities.BarCodePrintLayouts
                                          where s.BarcodeOperation == operation
                                                && s.Code == code
                                          select s).ToList();

                return printingListLayout.ToList();
            }
        }

        #endregion Printing
    }
}