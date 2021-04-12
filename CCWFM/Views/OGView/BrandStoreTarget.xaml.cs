using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class BrandStoreTarget
    {
        private readonly BrandStoreTargetViewModel _viewModel;

        public BrandStoreTarget()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as BrandStoreTargetViewModel;
            DataContext = _viewModel;
        }



        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.ExportExcel("StoresTarget");
        }
    }
}