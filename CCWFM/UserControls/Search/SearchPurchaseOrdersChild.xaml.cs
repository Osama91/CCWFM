using System.Collections.Generic;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchPurchaseOrdersChild
    {
        private readonly SearchPurchaseOrdersViewModel _viewModel = new SearchPurchaseOrdersViewModel();
        private readonly SearchPurchaseOrders _userControl;

        public SearchPurchaseOrdersChild(SearchPurchaseOrders searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
            if (_userControl.JournalType != null)
            {
                _viewModel.Type = _userControl.JournalType.TransactionId;
            }
            else
            {
                _viewModel.Type = 0;
            }

            _viewModel.GetMaindata();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as CRUD_ManagerServicePurchaseOrderDto;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }
    }
}