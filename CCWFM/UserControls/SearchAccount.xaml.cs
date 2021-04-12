using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls
{
    public partial class SearchAccount
    {
        private readonly SearchAccountUserControl _userControl;
        private readonly SearchAccountViewModel _viewModel = new SearchAccountViewModel();

        public SearchAccount(SearchAccountUserControl searchAccountUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.ChildOnly = searchAccountUserControl.ChildOnlyPerRow;
            _viewModel.GetMaindata();

            _userControl = searchAccountUserControl;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _userControl.SearchPerRow = MainGrid.SelectedItem as TblAccount;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            _userControl.SearchPerRow = null;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                _viewModel.MainRowList.Count < _viewModel.FullCount)
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

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _userControl.SearchPerRow = MainGrid.SelectedItem as TblAccount;
            DialogResult = true;
        }
    }
}