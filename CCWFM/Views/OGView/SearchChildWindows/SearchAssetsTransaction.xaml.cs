using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchAssetsTransaction
    {
        private readonly AssetsTransactionViewModel _assetsTransactionViewModel;
        private readonly SearchAssetsTransactionViewModel _viewModel;

        public SearchAssetsTransaction(AssetsTransactionViewModel assetsTransactionViewModel)
        {
            InitializeComponent();
            _assetsTransactionViewModel = assetsTransactionViewModel;
            _viewModel = (SearchAssetsTransactionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _assetsTransactionViewModel.SubmitSearch(MainGrid.SelectedItem as TblAssetsTransactionViewModel);
        }
        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }
    }
}