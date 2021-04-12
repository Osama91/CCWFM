using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using Os.Controls.DataGrid.Events;
using System.Linq;

namespace CCWFM.Views.OGView
{
    public partial class RecInvProd
    {
        private RecInvProductionViewModel _viewModel;

        public RecInvProd()
        {
            InitializeComponent();
            _viewModel = (RecInvProductionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            _viewModel = (RecInvProductionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            BtnEditOrder.IsChecked = false;

            _viewModel.MainRowList.Clear();
            _viewModel.SelectedMainRow = new TblRecInvHeaderProdViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            BtnDeleteOrder.IsEnabled = false;
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    break;

                case FormMode.Standby:

                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    ClearScreen();
                    break;

                case FormMode.Search:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnDeleteOrder.Visibility = Visibility.Visible;
                    BtnDeleteOrder.IsEnabled = true;

                    break;

                case FormMode.Update:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    break;

                case FormMode.Read:

                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;

                    break;
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "ProductionVendorInvoiceDoc";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "ProductionVendorInvoiceDoc"; }

            var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);

            //   BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteMainRow();

            ResetMode();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnAddNewOrder.IsChecked = false;
            BtnAddNewOrder.IsEnabled = true;

            BtnAddNewOrder.Visibility = Visibility.Visible;
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new RecHeaderProdChildWindow(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RecieveHeaderList.Clear();
            _viewModel.RecieveHeaderChoosedList.Clear();
            _viewModel.GetRecieveHeaderListData();
            var child = new RecieveHeaderProdChildWindow(_viewModel);
            child.Show();
        }

        #endregion FormModesSettings

        private void BtnMisc_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRow.MarkUpTransList.Clear();
            new MarkupTransProdChildWindow(_viewModel, true).Show();
        }


        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailsValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.SelectedMainRow.DetailsList.Count < _viewModel.DetailFullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetDetailData();
            }
        }

      

        private void BtnPost_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Post();
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }
    
        private void TxtCostStyle_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Focus();
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double test = 0;
            var txt = sender as TextBox;
            foreach (var item in _viewModel.SelectedMainRow.DetailsList.Where(w => w.TblCurrency == _viewModel.SelectedDetailRow.TblCurrency).ToList())
            {
                double.TryParse(txt.Text, out test);
                if (test != 0)
                {
                    item.ExchangeRate = test;
                }
            }

            _viewModel.SaveDetailRows();
        }
    }
}