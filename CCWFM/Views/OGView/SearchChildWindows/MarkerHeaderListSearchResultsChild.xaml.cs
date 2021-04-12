using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class MarkerHeaderListSearchResultsChild
    {
        public MarkerHeaderListSearchResultsChild(MarkerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MarkerViewModel;
            if (viewModel != null)
            {
                viewModel.MarkerHeader = DgResults.SelectedItem as MarkerHeaderListViewModel;
                if (viewModel.MarkerHeader != null)
                    viewModel.MarkerHeader.VendorPerRow = new Vendor {vendor_code = viewModel.MarkerHeader.VendorCode};
                viewModel.FillallMarkers();
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MarkerViewModel;
            if (viewModel != null) viewModel.SearchHeader();
        }
    }
}