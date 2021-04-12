using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class GeneratePurchase
    {
        private readonly GeneratePurchaseViewModel _viewModel;

        public GeneratePurchase()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as GeneratePurchaseViewModel;
            DataContext = _viewModel;            
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
            _viewModel.SelectedMainRow = new TblGeneratePurchaseHeaderModel();
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
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;

                    ClearScreen();
                    break;

                case FormMode.Search:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;

                    break;

                case FormMode.Update:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Read:

                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    BtnSaveOrder.IsEnabled = false;
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
            _viewModel.GenerateSalesOrdersList.Clear();
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
            var reportName = "GeneratedPurchasePlan";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "GeneratedPurchasePlan"; }

            var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void PurchaseGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.PurchaseOrderList.IndexOf(_viewModel.SelectedPurchaseRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.PurchaseOrderList.Count - 1))
                {
                    if (_viewModel.SelectedMainRow.TblPlanType == 0)
                    {
                        _viewModel.AddNewDetailRow(true);
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
            }
        }

        private void PurchaseGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SavePurchaseRow();
        }

        private void Purchase2Grid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedPurchaseRow.PurchaseOrderHeaderList.IndexOf(_viewModel.SelectedPurchase2Row));
                if (currentRowIndex == (_viewModel.SelectedPurchaseRow.PurchaseOrderHeaderList.Count - 1))
                {
                    if (_viewModel.SelectedMainRow.TblPlanType == 0)
                    {
                        _viewModel.AddNewDetail2Row(true);
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
            }
        }

        private void Purchase2Grid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SavePurchase2Row();
        }

        private void BtnPurchaseDetail_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedPurchaseRow.DetailsList.Clear();
            var child = new PurchaseOrderDetailChild(_viewModel, 1);
            child.Show();
        }

        private void BtnPurchaseDetail2_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedPurchase2Row.DetailsList.Clear();
            var child = new PurchaseOrderDetailChild(_viewModel, 2);
            child.Show();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateLink();
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = sender as TabControl;
            if (tab != null && tab.SelectedIndex == 0)
            {
            }
            else
            {
                if (_viewModel.SelectedPurchaseRow.PurchaseOrderHeaderList != null)
                    _viewModel.SelectedPurchase2Row = _viewModel.SelectedPurchaseRow.PurchaseOrderHeaderList.FirstOrDefault();
            }
        }

        private void BtnSalesOrders_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new GenratePurchaseSalesOrderChildWindow(_viewModel);
            child.Show();
        }

        private void btnReCalc_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow(true);       
        }

        private void BtnMergePlans_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.MergePlans();
        }

        private void BtnApproveMergePlans_OnClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnCurrencies_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new GeneratePurchaseHeaderCurrencies(new GeneratePurchaseHeaderCurrenciesViewModel(_viewModel.SelectedMainRow.Iserial));
            child.Show();
        }

        private void PurchaseGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedPurchaseRow.Status)
            {
                e.Cancel = true;
            }
        }
    }
}