using System;
using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels.Mappers
{
    public class ReservationMappers
    {
        public static TblReservationMainDetailsViewModel MaptoViewModel(OrderLineListViewModel row)
        {
            return new TblReservationMainDetailsViewModel
            {
                Batchno = row.BatchNo,
                FabricName = row.Fabric_Ename,
                LineNum = row.LineNum,
                Fabric = row.Fabric_Code,
                FabricUnit = row.PURCHUNIT,
                FabricColor = row.Color_Code,
                Warehouse = row.Warehouse,
                Location = row.Location,
                Site = row.Site,

                Qty = row.TotalQty
            };
        }

        public static CRUD_ManagerServicePurchaseOrderDetailDto MaptoViewModel(TblReservationMainDetailsViewModel rows, bool Temp)
        {
            return new CRUD_ManagerServicePurchaseOrderDetailDto
            {
                BatchNo = rows.Batchno,
                LineNumber = rows.LineNum,
                ItemId = rows.Fabric,
                Unit = rows.FabricUnit,
                FabricColor = rows.FabricColor,
                Warehouse = rows.Warehouse,
                Location = rows.Location,
                Site = rows.Site,
            };
        }

        public static Tbl_ReservationHeader MaptoViewModel(TblReservationHeaderViewModel resRow)
        {
            return new Tbl_ReservationHeader
            {
                AxRouteCardFabricsJournalId = resRow.AxRouteCardFabricsJournalId,
                DocDate = resRow.DocDate,
                Iserial = resRow.Iserial,
                IsPosted = resRow.IsPosted,
                ReservationType = resRow.ReservationType,
                TransOrder = resRow.TransOrder,
                DocNo = "tEST",
            };
        }

        public static TblReservationHeaderViewModel MaptoViewModel(Tbl_ReservationHeader resRow)
        {
            var row = (TblReservationHeaderViewModel)new TblReservationHeaderViewModel().InjectFrom(resRow);

            row.JournalPerRow = new CRUD_ManagerServicePurchaseOrderDto
            {
                JournalId = row.TransOrder,
                //VendorCode = resRow.
            };
            return row;
        }

        public static Tbl_ReservationMainDetails MaptoViewModel(TblReservationMainDetailsViewModel resRow)
        {
            var detailsList = new ObservableCollection<Tbl_ReservationDetails>();

            foreach (var row in resRow.ReservationDetailsViewModelList)
            {
                var newDetail = new Tbl_ReservationDetails();
                GenericMapper.InjectFromObCollection(newDetail.Tbl_ReservationRec, row.ReservationListRec);
                newDetail.InjectFrom(row);
                detailsList.Add(newDetail);
            }

            var newRow = new Tbl_ReservationMainDetails();
            newRow.InjectFrom(resRow);
            newRow.Tbl_ReservationDetails = detailsList;

            return newRow;
        }

        public static TblReservationMainDetailsViewModel MaptoViewModel(Tbl_ReservationMainDetails resRow, ObservableCollection<Fabric_UnitID> mainFabricList)
        {
            var detailsList = new ObservableCollection<TblReservationDetailsViewModel>();

            foreach (var row in resRow.Tbl_ReservationDetails)
            {
                var newDetail = new TblReservationDetailsViewModel();
                GenericMapper.InjectFromObCollection(newDetail.ReservationListRec, row.Tbl_ReservationRec);
                newDetail.InjectFrom(row);
                detailsList.Add(newDetail);
            }

            var newRow = new TblReservationMainDetailsViewModel();
            newRow.InjectFrom(resRow);
            newRow.ReservationDetailsViewModelList = detailsList;
            newRow.RemQtyTemp = newRow.Qty - newRow.ReservationDetailsViewModelList.Sum(x => x.IntialQty);
            newRow.RemQty = newRow.RemQtyTemp;
            //  newRow.FabricName = mainFabricList.FirstOrDefault(x => x.Fabric_Code == newRow.Fabric).Fabric_Ename;
            return newRow;
        }

        internal static TblReservationMainDetailsViewModel MaptoViewModel(Tbl_ReservationMainDetails resRow, ObservableCollection<Fabric_UnitID> mainFabricList, ObservableCollection<GetItemOnhand_Result> onHandList)
        {
            var detailsList = new ObservableCollection<TblReservationDetailsViewModel>();

            foreach (var row in resRow.Tbl_ReservationDetails)
            {
                var newDetail = new TblReservationDetailsViewModel();
                GenericMapper.InjectFromObCollection(newDetail.ReservationListRec, row.Tbl_ReservationRec);
                newDetail.InjectFrom(row);
                detailsList.Add(newDetail);
            }

            double onhand = 0;
            try
            {
                onhand = Convert.ToDouble(onHandList.FirstOrDefault(w => w.FabricCode == resRow.Fabric && w.CONFIGID == resRow.FabricColor && w.TOBATCH == resRow.Batchno).Qty);

            }
            catch (Exception)
            {

                onhand= 0;
            }
    
            var newRow = new TblReservationMainDetailsViewModel();
            newRow.InjectFrom(resRow);
            newRow.ReservationDetailsViewModelList = detailsList;
            newRow.RemQtyTemp = newRow.Qty - newRow.ReservationDetailsViewModelList.Sum(x => x.IntialQty);
            newRow.RemQty = newRow.RemQtyTemp;
            newRow.OnHandQty = onhand;
            //  newRow.FabricName = mainFabricList.FirstOrDefault(x => x.Fabric_Code == newRow.Fabric).Fabric_Ename;
            return newRow;
        }
    }
}