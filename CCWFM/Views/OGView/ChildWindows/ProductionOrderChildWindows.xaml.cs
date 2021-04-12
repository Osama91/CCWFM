using System.Windows;
using CCWFM.ViewModel.OGViewModels;
using System.Windows.Input;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ProductionOrderChildWindows
    {
        ProductionOrderViewModel _viewModel;
        public ProductionOrderChildWindows(ProductionOrderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void ProductionFabricGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedProductionOrderFabrics.Clear();
                foreach (var row in ProductionFabricGrid.SelectedItems)
                {
                    _viewModel.SelectedProductionOrderFabrics.Add(row as TblProductionOrderFabricModel);
                }

                _viewModel.DeleteProductionOrderFabric();
            }
        }

   

        private void ProdService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedProductionOrderServices.Clear();
                foreach (var row in ProdService.SelectedItems)
                {
                    _viewModel.SelectedProductionOrderServices.Add(row as TblProductionOrderServiceModel);
                }

                _viewModel.DeleteProductionOrderFabric();
            }
            else if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.TblProductionOrderServices.IndexOf(_viewModel.SelectedProductionOrderService));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.TblProductionOrderServices.Count - 1))
                {
                    _viewModel.AddNewServiceRow(true);
                }
            }
        }

    private void ChildWindowsOverride_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.SaveProductionOrderFabrics();
            _viewModel.SaveProductionOrderServices();
        }

        private void ProdTransGrid_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedDetailRow.Posted)
            {
                e.Cancel = true;
            }
        }
    }
}

