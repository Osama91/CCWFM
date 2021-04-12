using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class DyeingOrderInvoiceSearchResultsChild
    {
        public DyeingOrderInvoiceSearchResultsChild(DyeingOrderInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DyeingOrderInvoiceViewModel)DataContext;
            viewModel.SelectedMainRow = DgResults.SelectedItem as TblDyeingOrderInvoiceHeaderViewModel;
            viewModel.GetDetailData();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (DyeingOrderInvoiceViewModel)DataContext;
            viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.DetailFilter = filter;
            viewModel.DetailValuesObjects = valueObjecttemp;
            viewModel.SearchHeader();
        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (DyeingOrderInvoiceViewModel)DataContext;
 
            if (viewModel.MainRowList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.SearchHeader();
            }
        }
    }
}