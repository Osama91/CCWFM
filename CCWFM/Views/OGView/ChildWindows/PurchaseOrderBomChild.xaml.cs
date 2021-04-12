using System.Windows;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.PurchasePlanService;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseOrderBomChild
    {
        public PurchaseOrderBomChild(GeneratePurchaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.GetPurchaseBom();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;

            if (e.Key == Key.Delete)
            {
                viewModel.SelectedPurchaseOrderDetailBreakDowns.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    viewModel.SelectedPurchaseOrderDetailBreakDowns.Add(row as TblPurchaseOrderDetailBreakDown);
                }
                viewModel.DeleteLink();
            }
        }
    }
}