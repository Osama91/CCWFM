using System.Collections.Generic;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class DyeingOrderSearchResultsChild
    {
        public DyeingOrderViewModel ViewModel;

        public DyeingOrderSearchResultsChild(DyeingOrderViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            ViewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DyeingOrderHeader = dgDyeingOrderResults.SelectedItem as TblDyeingOrderHeaderViewModel;
            ViewModel.GetDyeingOrderMainDetail();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgDyeingOrderResults_OnOnFilter(object sender, FilterEvent e)
        {
            ViewModel.DyeingOrderHeaderList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            ViewModel.Filter = filter;
            ViewModel.ValuesObjects = valueObjecttemp;
            ViewModel.SearchHeader();
        }
    }
}