using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Views.PrintPreviews;

namespace CCWFM.Views.OGView
{
    public partial class FabricDefects
    {
        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();

        private readonly FabricDefectsViewModel _viewModel = new FabricDefectsViewModel();

        #region FormModesSettings

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            _viewModel.TransactionDetails.Clear();
            _viewModel.TransactionHeader = new TblFabricInspectionHeaderViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtntnAddNewMainOrderDetails.IsEnabled = true;

                    _viewModel.TransactionHeader.Enabled = true;

                    break;

                case FormMode.Standby:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    BtnSaveOrder.IsEnabled = true;

                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = true;

                    break;

                case FormMode.Update:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = true;

                    break;

                case FormMode.Read:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;

                    BtnSaveOrder.IsEnabled = true;

                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r =
            MessageBox
            .Show("You are about to delete a Fabric Defects Order permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _viewModel.DeleteOrder();
            }
        }

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FabricInspectionReport();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TransactionHeaderList.Clear();
            _viewModel.SearchHeader();
            var childWindowSeach = new FabricInspectionOrderSearchResultsChild(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
           
            _viewModel.SaveOrder();
        }

        private void btnSaveOrderTemp_Click(object sender, RoutedEventArgs e)
        {

            _viewModel.SaveOrderTemp();
        }
        #endregion FormModesSettings

        public FabricDefects()
        {
            InitializeComponent();
            DataContext = _viewModel;
           // _webService.CreateAxBarcodeAsync(115, 1, LoggedUserInfo.Iserial, 0);
            SwitchFormMode(FormMode.Add);
            _webService.PrintingFabricDefectsCompleted += webService_PrintingFabricDefectsCompleted;
            _viewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
        }

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        private void PurchaseOrderAC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PurchaseOrderAc.SelectedIndex != -1)
            {
                var da = PurchaseOrderAc.SelectedItem as CRUD_ManagerServicePurchaseOrderDto;
                if (da != null) _viewModel.TransactionHeader.VendorProperty = da.VendorCode;
            }
        }

        private void btnPostToAx_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateAxBarcode();

            BtnPrintBarcode.IsEnabled = true;
        }

        private void webService_PrintingFabricDefectsCompleted(object sender, PrintingFabricDefectsCompletedEventArgs e)
        {
            var printingPage = new BarcodePrintPreview(e.DefectsList.ToList(), e.Result, 1, (LoggedUserInfo.BarcodeSettingHeader.Code), true);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            printingPage.Show();
        }

        private void btnPrintBarcode_Click(object sender, RoutedEventArgs e)
        {
            _webService.PrintingFabricDefectsAsync(int.Parse(TxtIserial.Text));
        }

        private void btnAddNewMainOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.OrderLineList.Clear();
            _viewModel.GetOrderInfo();
            var childWindows = new FabricDefectsLineCreationChildWindow(_viewModel);
            childWindows.Show();
        }

        private void BtnDeleteInspectionRowClick(object sender, RoutedEventArgs e)
        {
            var delete = (sender) as Button;
            if (delete == null) return;
            var x = delete.DataContext as TblFabricInspectionDetailViewModel;
            _viewModel.DeleteInspectionLine(x);
        }

        //private void JournalComplete_Populating(object sender, PopulatingEventArgs e)
        //{
        //    var autoComplete = sender as AutoCompleteBox;
        //    e.Cancel = true;
        //    _viewModel.SearchForAxTransaction(autoComplete.Text);
        //}

        //private void JournalComplete_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var autoComplete = sender as AutoCompleteBox;

        //    if (_viewModel != null)
        //        _viewModel.PurchaseTransactionsCompleted += (s, r) => autoComplete.PopulateComplete();
        //}

        private void TxtDefect_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.SelectAll();
        }
    }
}