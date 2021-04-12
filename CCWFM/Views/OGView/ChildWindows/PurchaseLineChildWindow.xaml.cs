using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseLineChildWindow
    {
        public PurchaseLineChildWindow(ReservationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            var viewModel = (ReservationViewModel)DataContext;        
         //   _ViewModel.GetFabricInspectionOrderDetail();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

