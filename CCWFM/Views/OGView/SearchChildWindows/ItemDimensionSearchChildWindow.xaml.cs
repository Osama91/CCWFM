using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class ItemDimensionSearchChildWindow 
    {
        private ItemDimensionSearchViewModel _viewModel;
        public ItemDimensionSearchChildWindow(ItemDimensionSearchViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Grid_OnOnFilter(object sender, FilterEvent e)
        {
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
        }

        bool isTransfer = true;
        public bool IsTransfer
        {
            get { return isTransfer; }
            set
            {
                isTransfer = value;
                SelectedItems.Columns.FirstOrDefault(c =>
                c.SortMemberPath == "To").Visibility
                = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        string quantityTitle;
        public string QuantityTitle
        {
            get { return quantityTitle; }
            set { quantityTitle = value; 
                SelectedItems.Columns.FirstOrDefault(c =>
                c.SortMemberPath == "Quantity").Header = value; }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems.SelectedItem != null)
            {
                var temp = new Web.DataLayer.ItemDimensionSearchModel();
                temp.InjectFrom(SelectedItems.SelectedItem);
                _viewModel.WarehouseClient.GetItemToQuantitiesAsync(temp, _viewModel.WarehouseToCode);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedItems.SelectedItem != null)
            {
                var temp = new Web.DataLayer.ItemDimensionSearchModel();
                temp.InjectFrom(SelectedItems.SelectedItem);
                _viewModel.WarehouseClient.GetItemToQuantitiesAsync(temp, _viewModel.WarehouseToCode);
            }
        }

        private void SearchColor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedItems.SelectedItem != null)
            {
                var temp = new Web.DataLayer.ItemDimensionSearchModel();
                temp.InjectFrom(SelectedItems.SelectedItem);
                _viewModel.WarehouseClient.GetItemToQuantitiesAsync(temp, _viewModel.WarehouseToCode);
            }
        }
    }
}

