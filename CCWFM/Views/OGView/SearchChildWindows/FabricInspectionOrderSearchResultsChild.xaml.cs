using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class FabricInspectionOrderSearchResultsChild
    {
        public FabricInspectionOrderSearchResultsChild(FabricDefectsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (FabricDefectsViewModel)DataContext;
            viewModel.TransactionHeader = DgResults.SelectedItem as TblFabricInspectionHeaderViewModel;
            viewModel.GetFabricInspectionOrderDetail();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (FabricDefectsViewModel)DataContext;
            viewModel.TransactionHeaderList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.DetailFilter = filter;
            viewModel.DetailValuesObjects = valueObjecttemp;
            viewModel.SearchHeader();
        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (FabricDefectsViewModel)DataContext;
 
            if (viewModel.TransactionHeaderList.Count <= viewModel.PageSize)
            {
                return;
            }
            if (viewModel.TransactionHeaderList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.SearchHeader();
            }



        }
    }
}