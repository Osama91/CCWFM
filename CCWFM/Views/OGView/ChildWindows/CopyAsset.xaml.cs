using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class CopyAsset
    {
        public CopyAsset(AssetsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           DialogResult = true;
            var viewModel = DataContext as AssetsViewModel;
            if (viewModel != null) viewModel.CopyAsset();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

