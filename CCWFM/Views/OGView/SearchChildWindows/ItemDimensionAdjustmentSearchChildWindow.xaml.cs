using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class ItemDimensionAdjustmentSearchChildWindow
    {
        bool hasCost = false;
        private ItemDimensionAdjustmentSearchViewModel _viewModel;
        public ItemDimensionAdjustmentSearchChildWindow(
            ItemDimensionAdjustmentSearchViewModel viewModel, bool hasCost = false)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            this.hasCost = hasCost;
        }

        private void Grid_OnOnFilter(object sender, FilterEvent e)
        {
            //_viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            //_viewModel.GetMaindata();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SearchResultList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SearchResultList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetInspectionWarehouseRows();
            }
        }

        private void SelectedItems_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = ((DataGrid)sender);
            grid.Columns.FirstOrDefault(c =>
                c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(Cost))
                .Visibility = hasCost ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

