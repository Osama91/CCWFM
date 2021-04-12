using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ColorsToLinkChildWindow
    {
        public ColorsToLinkChildWindow(ColorLinkViewModel viewModel)
        {
            InitializeComponent();
            viewModel.GetDetaildata();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ColorLinkViewModel;
            if (viewModel != null)
            {
                viewModel.MainSelectedRows = new ObservableCollection<TblColor>();
                foreach (var row in MainGrid.SelectedItems)
                {
                    viewModel.MainSelectedRows.Add((TblColor)row);
                }
                viewModel.AddToColorLink();
            }

            DialogResult = true;
        }

        private void MainGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = DataContext as ColorLinkViewModel;
            if (viewModel != null && viewModel.ColorsList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel != null && (viewModel.ColorsList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading))
            {
                viewModel.Loading = true;
                viewModel.GetDetaildata();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            var viewModel = DataContext as ColorLinkViewModel;
            if (viewModel != null)
            {
                viewModel.ColorsList.Clear();
                string filter;
                Dictionary<string, object> valueObjecttemp;
                GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
                viewModel.DetailFilter = filter;
                viewModel.DetailValuesObjects = valueObjecttemp;
                viewModel.GetDetaildata();
            }
        }
    }
}