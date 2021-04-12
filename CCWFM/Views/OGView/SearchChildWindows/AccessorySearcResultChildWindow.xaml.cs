using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AccessoriesViewModel;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class AccessorySearcResultChildWindow
    {
        public AccessorySearcResultChildWindow(AccessoriesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AccessoriesViewModel;
            viewModel.SelectedMainRow = (AccessoryAttributesViewModel) MainGrid.SelectedItem;

            viewModel.LoadAccessoryDetail();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = DataContext as AccessoriesViewModel;
            if (viewModel.MainRowList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.MainRowList.Count != viewModel.FullCount)
            {
                viewModel.GetAllAcc();
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            var viewModel = DataContext as AccessoriesViewModel;
            viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.Filter = filter;
            viewModel.ValuesObjects = valueObjecttemp;
            viewModel.GetAllAcc();
        }
    }
}