using System.Windows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class RouteCardInvoice
    {   
        private readonly RouteCardInvoiceViewModel _viewModel = new RouteCardInvoiceViewModel();

        #region FormModesSettings

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            _viewModel.TransactionDetails.Clear();
            _viewModel.SelectedMainRow = new RouteCardInvoiceHeaderViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            BtnDeleteOrder.IsEnabled = false;
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnSaveOrder.IsEnabled = true;
                    break;

                case FormMode.Standby:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;                    
                    BtnDeleteOrder.Visibility = Visibility.Visible;
                    BtnDeleteOrder.IsEnabled = true;
                    break;

                case FormMode.Update:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    break;

                case FormMode.Read:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
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

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {           
            _viewModel.SaveOrder();
        }

        #endregion FormModesSettings

        public RouteCardInvoice()
        {
            InitializeComponent();
            DataContext = _viewModel;
            SwitchFormMode(FormMode.Add);
        }
        
        private void btnAddNewMainOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.TransactionDetails.Clear();
            _viewModel.GetOrderInfo();       
        }

        private void BtnMisc_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedMainRow.Iserial==0)
            {
                _viewModel.SaveOrder(true);
            }
            else
            {
                new MarkupTransProdChildWindow(_viewModel).Show();
            }                     
        }

        private void BtnPost_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Post();
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteMainRow();
            ResetMode();
        }
    }
}