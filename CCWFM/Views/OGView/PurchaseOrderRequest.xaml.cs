using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using Os.Controls.DataGrid.Events;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView
{
    public partial class PurchaseOrderRequest
    {
        private readonly PurchaseOrderRequestViewModel _viewModel;

        private void BtnFree_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.AllowAddFree)
            {
                var childWindow = new RouteFreeIssueChildWindow(_viewModel);
                childWindow.Show();
            }
            else
            {
                MessageBox.Show("You Don't Have Permission Required To Add Items");
            }
        }

        public PurchaseOrderRequest()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as PurchaseOrderRequestViewModel;
            DataContext = _viewModel;

            _viewModel.PremCompleted += (s, sv)=> {
                BtnApprove.Visibility = _viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "Approval") != null ? Visibility.Visible : Visibility.Collapsed;         
                _viewModel.AllowAddFree= _viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "Allow Free Orders") != null ? true : false;
            };

        }

        #region FormModesSettings

        public enum FormMode
        {
            Standby,
            Search,
            Add,
            Update,
            Read
        }

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            _viewModel.SelectedMainRow = new TblPurchaseOrderHeaderRequestModel();

            if (_viewModel.PaymTerm.Any())
            {
                _viewModel.SelectedMainRow.AxTermOfPaymentCode = _viewModel.PaymTerm.FirstOrDefault().PAYMTERMID;
            }

            if (_viewModel.VendPayModeList.Any())
            {
                _viewModel.SelectedMainRow.AxMethodOfPaymentCode = _viewModel.VendPayModeList.FirstOrDefault().PAYMMODE;
            }
            if (_viewModel.CurrencyList.Any())
            {
                _viewModel.SelectedMainRow.TblCurrency = _viewModel.CurrencyList.FirstOrDefault().Iserial;
            }
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    _viewModel.SelectedMainRow.Enabled = true;
                    break;
                case FormMode.Standby:
                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    //BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;
                    ClearScreen();
                    break;
                case FormMode.Search:
                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    //BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                    break;
                case FormMode.Update:
                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    //BtnSaveOrder.IsEnabled = false;
                    break;
                case FormMode.Read:
                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    //BtnSaveOrder.IsEnabled = false;
                    //   BtnEditOrder.IsEnabled = true;
                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            _viewModel.AddNewDetailRow(false);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r =
            MessageBox
            .Show("You are about to delete a GeneratePurchase Order permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _viewModel.DeleteOrder();
            }
        }

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancelOrder.IsEnabled = false;
            BtnCancelOrder.Visibility = Visibility.Collapsed;
            BtnShowSearchOrder.IsChecked = false;
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
            _viewModel.DetailsList.Clear();
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SearchHeader();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        #endregion FormModesSettings

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "PurchaseOrderDoc";
            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "PurchaseOrderDoc"; }
            var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void PurchaseGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in PurchaseGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblPurchaseOrderDetailRequestModel);
                }
                _viewModel.DeleteDetailRow();
            }
            else if (e.Key == Key.Down)
            {
                _viewModel.AddNewDetailRow(true);
            }
        }

        private void PurchaseGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void BtnPurchaseDetail_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
        }


        private void btnReCalc_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void PurchaseGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {          
        }

        private void BtnPlannedOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Vendor = _viewModel.SelectedMainRow.Vendor;
            _viewModel.VendorPerRow = _viewModel.SelectedMainRow.VendorPerRow;
            PendingPurchaseOrderChildWindows child = new PendingPurchaseOrderChildWindows(_viewModel);
            child.Show();
        }


        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var viewModel = (PurchaseOrderRequestViewModel)DataContext;
                if (TaPurchase != null)
                {

                    if (TaPurchase.SelectedIndex == 1)
                    {
                        viewModel.GetRecHeader();
                    }
                }
            }
            catch { }
        }

        private void RecGrid_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SelectedMainRow.RecHeaderList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.DetailSubFilter = filter;
            viewModel.DetailSubValuesObjects = valueObjecttemp;
            viewModel.GetRecHeader();
        }

        private void RecGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            if (viewModel.SelectedMainRow.RecHeaderList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.SelectedMainRow.RecHeaderList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.GetRecHeader();
            }

        }

        private void RecGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            if (e.Key == Key.Delete)
            {
                viewModel.SelectedRecHeaders.Clear();
                foreach (var row in RecGrid.SelectedItems)
                {
                    viewModel.SelectedRecHeaders.Add(row as TblPurchaseReceiveHeaderModel);
                }

                viewModel.DeleteRecHeader();
            }
        }

        private void RecGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SaveRecHeaderRow();
        }

        private void RecGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {     
        }

        private void btnPrintPurchaseOrder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            const string reportName = "RecievePo";

            var para = new ObservableCollection<string>
            {
                viewModel.SelectedSubDetailRow.Iserial.ToString(CultureInfo.InvariantCulture),
                LoggedUserInfo.WFM_UserName
            };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            var child = new PurchaseReceiveChildWindow(DataContext as PurchaseOrderRequestViewModel);
            child.Show();
        }

        private void btnReceiveDetail_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.GetRecDetail();
            var child = new PurchaseReceiveDetailChildWindow(DataContext as PurchaseOrderRequestViewModel);
            child.Show();
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        { 
            var newrow = new TblPurchaseOrderDetailRequestModel();
            newrow.InjectFrom(_viewModel.SelectedDetailRow);
            newrow.TblPurchaseRequestLink = new ObservableCollection<PurchasePlanService.TblPurchaseRequestLink>();
            newrow.Iserial = 0;
            foreach (var PurchaseRequestLink in _viewModel.SelectedDetailRow.TblPurchaseRequestLink)
            {
                newrow.TblPurchaseRequestLink.Add(new PurchasePlanService.TblPurchaseRequestLink()
                {
                    TblPurchaseOrderDetail = PurchaseRequestLink.TblPurchaseOrderDetail,
                });
            }
            int currenctindex = _viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow) + 1;
            _viewModel.SelectedMainRow.DetailsList.Insert(currenctindex, newrow);

        }
     
        private void ckhApprove_Checked(object sender, RoutedEventArgs e)
        {
        
        }

        
  private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
              _viewModel.Approve();
        }
        private void BtnPaymentOrder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;            
            var child = new PurchaseOrderRequestPaymentChild(DataContext as PurchaseOrderRequestViewModel);
            child.Show();
        }

        private void BtnGetPlanItems_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetPlanFabric();
        }

      

        private void PlansVendorUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetPlanFabric();

        }

        private void txtPlanCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                _viewModel.GetPlanVendors();
            }
        }

        private void txtPlanCode_LostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.GetPlanVendors();
        }

     
    }
}