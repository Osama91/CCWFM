using System.ComponentModel;
using System.Windows;
using CCWFM.ViewModel.OGViewModels;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PendingPurchaseOrderChildWindows
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private readonly PurchaseOrderRequestViewModel _viewModel;

        public PendingPurchaseOrderChildWindows(PurchaseOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            LayoutRoot.DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var listOfDate = new ObservableCollection<TblPurchaseOrderDetailViewModel>();
            foreach (var item in DgResults.SelectedItems)
            {
                listOfDate.Add(item as TblPurchaseOrderDetailViewModel);
            }
            foreach (var item in listOfDate.GroupBy(w => new { w.ItemId, w.FabricColor,  w.Size, w.BatchNo, w.ItemType, w.Unit, w.IsAcc }))
            {
                var newrow = new TblPurchaseOrderDetailRequestModel()
                {
                    BatchNo = item.Key.BatchNo,
                    ColorPerRow = item.FirstOrDefault().ColorPerRow,
                    Qty = item.Sum(w => w.Qty),
                    Price = item.Sum(w => w.Qty * w.Price) / item.Sum(w => w.Qty)??0,
                    ItemType = item.Key.ItemType,
                    Received = 0,
                    ReceiveNow = 0,
                    BasicPrice = item.Sum(w => w.Qty * w.Price) / item.Sum(w => w.Qty)??0,
                    BomQty = 0,
                    DeliveryDate = _viewModel.SelectedMainRow.DeliveryDate,
                    ShippingDate = _viewModel.SelectedMainRow.ShippingDate,
                    Size = item.Key.Size,
                    FabricColor = item.Key.FabricColor,
                    Unit = item.Key.Unit,
                    IsAcc = item.Key.IsAcc,
                    ItemPerRow = item.FirstOrDefault().ItemPerRow,
                    ItemId = item.Key.ItemId,
                };
                newrow.TblPurchaseRequestLink = new ObservableCollection<PurchasePlanService.TblPurchaseRequestLink>();
                foreach (var PurchaseRequestLink in _viewModel.DetailsList.Where(w=>w.ItemId== item.Key.ItemId&& w.FabricColor == item.Key.FabricColor&& w.Size == item.Key.Size&& w.BatchNo == item.Key.BatchNo&& w.ItemType == item.Key.ItemType))
                {
                    newrow.TblPurchaseRequestLink.Add(new PurchasePlanService.TblPurchaseRequestLink()
                    {
                        TblPurchaseOrderDetail = PurchaseRequestLink.Iserial,
                    });
                }            
                newrow.Iserial = 0;
                newrow.DeliveryDate = _viewModel.SelectedMainRow.DeliveryDate;
                newrow.ShippingDate = _viewModel.SelectedMainRow.ShippingDate;

                if (newrow.TblPurchaseRequestLink.Any())
                {
                    newrow.Enabled = false;
                }
                else
                {
                    newrow.Enabled = true;
                }

                if (!_viewModel.SelectedMainRow.DetailsList.Any(w => w.ItemId == item.Key.ItemId && w.FabricColor == item.Key.FabricColor && w.Size == item.Key.Size && w.BatchNo == item.Key.BatchNo && w.ItemType == item.Key.ItemType))
                {
                    _viewModel.SelectedMainRow.DetailsList.Add(newrow);
                }
            
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SeachPendingPurchase();
        }

        private void DgResults_RowEditEnded(object sender, System.Windows.Controls.DataGridRowEditEndedEventArgs e)
        {
            var row = DgResults.SelectedItem as TblPurchaseOrderDetailViewModel;

            _viewModel.PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailCanceledAsync(row.Iserial, row.Canceled);
        }

        private void DgResults_KeyDown(object sender, KeyEventArgs e)
        {
            //_viewModel.SelectedMainRows.Clear();

            if (e.Key == Key.Delete){

                var listOfDate = new ObservableCollection<TblPurchaseOrderDetailViewModel>();            
                foreach (var item in DgResults.SelectedItems)
                {
                    listOfDate.Add(item as TblPurchaseOrderDetailViewModel);
                }

                foreach (var row in listOfDate)
                {
                    _viewModel.DetailsList.Remove(row as TblPurchaseOrderDetailViewModel);
                }
            }


            //_viewModel.DeleteMainRow();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

            _viewModel.TblLkpSeason = null;
            _viewModel.Brand = null;
            _viewModel.TblLkpBrandSection = null;
            _viewModel.ItemPerRow = null;
            _viewModel.ColorPerRow = null;
            _viewModel.TblColor = 0;
            _viewModel.FromDate=null;
            _viewModel.ToDate =null;
            _viewModel.DetailsList.Clear();

        }
    }
}