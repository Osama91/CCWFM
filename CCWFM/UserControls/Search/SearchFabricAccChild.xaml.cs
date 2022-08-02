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
    public partial class SearchFabricAccChild
    {
        private readonly SearchFabricAccViewModel _viewModel;
        private readonly SearchFabricAcc _userControl;

        public CRUD_ManagerServiceClient Client = new CRUD_ManagerServiceClient();

        public SearchFabricAccChild(SearchFabricAcc searchEmployeeUserControl)
        {
            InitializeComponent();
            _userControl = searchEmployeeUserControl;
            _viewModel = new SearchFabricAccViewModel(_userControl);
            DataContext = _viewModel;
           
            
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
            _userControl.SearchPerRow = null;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata(_userControl);
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
            _viewModel.GetMaindata(_userControl);
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