using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class CopyColors 
    {
        public CopyColors(ColorLinkViewModel viewModel)
        {
            InitializeComponent();
            LayoutRoot.DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = LayoutRoot.DataContext as ColorLinkViewModel;
            if (viewModel != null) viewModel.CopyColorLink();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = false;
        }
    }
}

