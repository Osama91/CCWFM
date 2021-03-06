using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchFabricServiceChild
    {
        private readonly SearchFabricServiceViewModel _viewModel = new SearchFabricServiceViewModel();
        private readonly SearchFabricService _userControl;

        public CRUD_ManagerServiceClient Client = new CRUD_ManagerServiceClient();

        public SearchFabricServiceChild(SearchFabricService searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
            Client.FabricInventSumWithBatchesCompleted += (s, sv) =>
            {
                AvaliableQtyGrid.ItemsSource = sv.Result;
            };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as ItemsDto;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }

        private void BtnAvaliableQty_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));

            MyPopup.HorizontalOffset = p.X;
            MyPopup.VerticalOffset = 100;

            MyPopup.IsOpen = true;
            var row = MainGrid.SelectedItem as ItemsDto;
            Client.FabricInventSumWithBatchesAsync(null, row.Code, "Ccm",null,null,null);
        }

        private void MainGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
    }
}