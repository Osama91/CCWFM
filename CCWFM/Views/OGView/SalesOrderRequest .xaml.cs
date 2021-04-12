using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;
using System.Collections.Generic;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.ChildWindow;

namespace CCWFM.Views.OGView
{
    public partial class SalesOrderRequest
    {
        private readonly SalesOrderRequestViewModel _viewModel;

        public SalesOrderRequest()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as SalesOrderRequestViewModel;
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
            _viewModel.SelectedMainRow = new TblSalesOrderHeaderRequestModel();
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
            _viewModel.AddNewMainRow(false);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r =
            MessageBox
            .Show("You are about to delete a GenerateSalesOrder Order permanently!!\nPlease note that this action cannot be undone!"
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
      //      _viewModel.DetailsList.Clear();
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Search();
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
            var reportName = "SalesOrderDoc";
            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "SalesOrderDoc"; }
            var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void SalesOrderGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {                
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in SalesOrderGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblSalesOrderDetailRequestModel);
                }
                _viewModel.DeleteDetailRow();
            }
        }

        private void SalesOrderGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }
                
        private void BtnSalesOrderDetail_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            //    var child = new SalesOrderDetailChild(_viewModel, 1);
            //   child.Show();
        }


        private void btnReCalc_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void SalesOrderGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
        }

        private void BtnPlannedOrder_Click(object sender, RoutedEventArgs e)
        {
         //   RecieveSalesIssueChildWindow child = new RecieveSalesIssueChildWindow(_viewModel);
        //    child.Show();
        }
     

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (SalesOrderRequestViewModel)DataContext;
            if (TaSalesOrder != null)
            {
                if (TaSalesOrder.SelectedIndex == 1)
                {
                   // viewModel.GetRecHeader();
                }
            }
        }        

        private void RecGrid_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (SalesOrderRequestViewModel)DataContext;           
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
            var viewModel = (SalesOrderRequestViewModel)DataContext;

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
            var viewModel = (SalesOrderRequestViewModel)DataContext;

            if (e.Key == Key.Delete)
            {
                viewModel.SelectedRecHeaders.Clear();
                foreach (var row in RecGrid.SelectedItems)
                {
                    viewModel.SelectedRecHeaders.Add(row as TblSalesIssueHeaderModel);
                }

                viewModel.DeleteRecHeader();
            }


        }

        private void RecGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var viewModel = (SalesOrderRequestViewModel)DataContext;         
                viewModel.SaveRecHeaderRow();            
        }
    
        private void RecGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //            
        }

        private void btnPrintSalesOrder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SalesOrderRequestViewModel)DataContext;
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
            var child = new PurchaseReceiveChildWindow(DataContext as SalesOrderRequestViewModel);
            child.Show();
        }

        private void btnReceiveDetail_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SalesOrderRequestViewModel)DataContext;
            //viewModel.GetRecDetail();
            //var child = new SalesOrderReceiveDetailChildWindow(DataContext as SalesOrderRequestViewModel);
            //child.Show();
        }

        private void btnPrintPurchaseOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtItemDim_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}