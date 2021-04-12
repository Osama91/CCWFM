using System.Collections.Generic;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchVendorChild
    {
        private readonly SearchVendorViewModel _viewModel = new SearchVendorViewModel();
        private readonly SearchVendor _userControl;

        public SearchVendorChild(SearchVendor searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
            if (_userControl.ItemPerRow != null)
            {
                _viewModel.ItemId = _userControl.ItemPerRow.Iserial;
                _viewModel.ItemType = _userControl.ItemPerRow.ItemGroup;
                _viewModel.SeasonPerRow = _userControl.SeasonPerRow;

            }
            _viewModel.GetMaindata();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as Vendor;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            _userControl.SearchPerRow = null;
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