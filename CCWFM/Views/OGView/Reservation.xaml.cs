using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class Reservation
    {
        private readonly ReservationViewModel _viewModel = new ReservationViewModel();

        public Reservation()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
            SwitchFormMode(FormMode.Add);
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
            BtnAddNewOrder.IsChecked = false;

            _viewModel.TransactionHeader = new TblReservationHeaderViewModel { DocDate = DateTime.Now.Date };
        }

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    _viewModel.TransactionHeader.Enabled = true;

                    break;

                case FormMode.Standby:
                    _viewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    ClearScreen();
                    break;

                case FormMode.Search:
                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    break;

                case FormMode.Update:

                    _viewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;
                    break;

                case FormMode.Read:

                    _viewModel.TransactionHeader.Enabled = false;

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

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
        }

        private FormMode _formMode;

        public FormMode _FormMode
        {
            get { return _formMode; }
            set { _formMode = value; }
        }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TransactionHeaderList.Clear();
            _viewModel.SearchHeader();
            var childWindowSeach = new ReservationOrderSearchResultsChild(_viewModel);
            childWindowSeach.Show();

            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        #endregion FormModesSettings

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveOrder();
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox
            .Show("You are about to delete Reservation Order permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _viewModel.DeleteOrder();
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "Reservation";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "Reservation"; }
            
            var para = new ObservableCollection<string> { _viewModel.TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void BtnAddNewMainRow_OnClick(object sender, RoutedEventArgs e)
        {

            _viewModel.OrderLineList.Clear();
            _viewModel.GetOrderInfo();
            var childWindows = new FabricDefectsLineCreationChildWindow(_viewModel);
            childWindows.Show();

            //_viewModel.TransactionMainDetails.Add(new TblReservationMainDetailsViewModel());
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BtnAddNewDetailsRow_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SalesorderList.Clear();
            new ReservationSalesOrdersChildWindow(_viewModel).Show();
            //_viewModel.SelectedMainDetails.ReservationDetailsViewModelList.Add(new TblReservationDetailsViewModel());
        }

        private void btnDeleteRow_Click(object sender, MouseButtonEventArgs e)
        {
            var deleteBtn = sender as Image;
          
            if (_viewModel.SelectedMainDetails.Inspected)
            {
                MessageBox.Show("Cannot Delete Inspected Row ");
            }
            else
            {
                var rowtoDelete = (TblReservationDetailsViewModel) deleteBtn.DataContext;
                _viewModel.Client.DeleteReservationDetailsAsync(rowtoDelete.Iserial);
                _viewModel.SelectedMainDetails.ReservationDetailsViewModelList.Remove(rowtoDelete);
                _viewModel.SelectedMainDetails.RemQty = _viewModel.SelectedMainDetails.RemQty+ rowtoDelete.IntialQty;
                _viewModel.SelectedMainDetails.RemQtyTemp = _viewModel.SelectedMainDetails.RemQtyTemp + rowtoDelete.IntialQty;
                
            }
        }

        private void btnResRec_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ReservationRec();
        }      
        private void DgResMainDetails_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.TransactionHeader.TransactionMainDetails.Clear();
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.Filter = _viewModel.Filter + " and ";
                }

                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetOrderInfo();
        }

        private void DgResMainDetails_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.TransactionHeader.TransactionMainDetails.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.TransactionHeader.TransactionMainDetails.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetOrderInfo();
            }
        }

        private void BtnGenerateFromReservation_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.LinkToPlan();
        }
    }
}