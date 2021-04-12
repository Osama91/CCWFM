using System;
using System.Windows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class SalesOrderRequestInvoice
    {   
        private readonly SalesOrderRequestInvoiceViewModel _viewModel = new SalesOrderRequestInvoiceViewModel();
        public SalesOrderRequestInvoice()
        {
            InitializeComponent();
            DataContext = _viewModel;
           SwitchFormMode(FormMode.Add);     
        }        
        #region FormModesSettings
        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            _viewModel.TransactionDetails.Clear();
            _viewModel.TransactionHeader = new TblSalesOrderRequestInvoiceHeaderModel();
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

                    BtnSaveOrder.IsEnabled = false;

                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Update:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Read:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;

                    BtnSaveOrder.IsEnabled = false;

                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Update;
            SwitchFormMode(FormMode);
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
        }
        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FabricInspectionReport();
        }
        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
        }
        public FormMode FormMode { get; set; }
        private void ResetMode()
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
        }
        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Search();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        #endregion FormModesSettings
        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveOrder();
        }
        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }    
        private void BtnMisc_OnClick(object sender, RoutedEventArgs e)
        {
            new MarkupTransProdChildWindow(_viewModel).Show();
        }
        private void BtnPost_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Post();
        }
        private void btnAddNewMainOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.RecieveHeaderList.Clear();
            _viewModel.RecieveHeaderChoosedList.Clear();            
            var child = new ChildWindow.RecieveSalesIssueChildWindow(_viewModel);
            child.Show();
        }
    }
}