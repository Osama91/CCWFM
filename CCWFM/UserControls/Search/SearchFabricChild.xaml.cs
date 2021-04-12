using System.Collections.Generic;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchFabricChild
    {
        private readonly SearchFabricViewModel _viewModel = new SearchFabricViewModel();
        private readonly SearchFabric _userControl;

        public SearchFabricChild(SearchFabric searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as tbl_FabricAttriputes;
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