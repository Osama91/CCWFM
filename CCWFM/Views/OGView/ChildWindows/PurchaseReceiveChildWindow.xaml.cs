using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseReceiveChildWindow
    {
        readonly PurchaseOrderRequestViewModel _viewModel;
        readonly SalesOrderRequestViewModel _viewModel2;
        public PurchaseReceiveChildWindow(PurchaseOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
            LayoutRoot.DataContext = viewModel;
            
        }
        public PurchaseReceiveChildWindow(SalesOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel2 = viewModel;
            DataContext = viewModel;
            LayoutRoot.DataContext = viewModel;

        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel!=null)
            {
                _viewModel.SelectedSubDetailRow.TblPurchaseOrderHeaderRequest = _viewModel.SelectedMainRow.Iserial;
                _viewModel.ReceivePurchase(CmbQty.SelectedIndex == 1);
            }

            if (_viewModel2 != null)
            {
                _viewModel2.SelectedSubDetailRow.TblSalesOrderHeaderRequest = _viewModel2.SelectedMainRow.Iserial;
                _viewModel2.ReceivePurchase(CmbQty.SelectedIndex == 1);
            }

            DialogResult = false;
        }          
    }
}