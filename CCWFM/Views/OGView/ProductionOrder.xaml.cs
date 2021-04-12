using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class ProductionOrder
    {
        private readonly ProductionOrderViewModel _viewModel;

        public ProductionOrder()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as ProductionOrderViewModel;
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
            _viewModel.SelectedMainRow = new TblProductionOrderHeaderModel();
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
            //_viewModel.AddNewMainRow(false);
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
            var reportName = "ProductionDoc";
            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "ProductionDoc"; }
            var para = new ObservableCollection<string> { LoggedUserInfo.Iserial.ToString(), _viewModel.SelectedDetailRow.Iserial.ToString(CultureInfo.InvariantCulture) };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void ProdTransGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in ProdTransGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblProductionOrderTransactionModel);
                }
                _viewModel.DeleteDetailRow();
            }
        }

        private void ProdTransGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void btnReCalc_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void btnPrintSalesOrder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (ProductionOrderViewModel)DataContext;
            const string reportName = "RecievePo";

            var para = new ObservableCollection<string>
            {
                viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture),
                LoggedUserInfo.WFM_UserName
            };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }


        private void ProdTransGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void ProdTransGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void ProdTransGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in ProdTransGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblProductionOrderTransactionModel);
                }

                _viewModel.DeleteDetailRow();
            }
            else if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }
        }

        private void ProdTransGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedDetailRow.Posted || _viewModel.SelectedDetailRow.Iserial != 0)
            {
                e.Cancel = true;
            }
        }
    }
}