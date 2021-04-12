using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchSalesOrderChild
    {
        private readonly SearchSalesOrderViewModel _viewModel = new SearchSalesOrderViewModel();
        private readonly SearchSalesOrder _userControl;

        public SearchSalesOrderChild(SearchSalesOrder searchEmployeeUserControl)
        {
            InitializeComponent();
            if (searchEmployeeUserControl.StatusPerRow != null)
            {
                _viewModel.Status = searchEmployeeUserControl.StatusPerRow.Iserial;
            }
            else
            {
                _viewModel.Status = 1;
            }
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as TblSalesOrder;
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
            int counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

            foreach (ColumnFilterControl f in e.FiltersPredicate)
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
                object myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                        break;
                }

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.Filter = _viewModel.Filter + " and ";
                }

                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }

        private void BtnOperation_OnClick(object sender, RoutedEventArgs e)
        {
            var Client = new CRUD_ManagerServiceClient();
            if (MainGrid.SelectedIndex != -1)
            {
                var b = (Button)sender;
                var gt = b.TransformToVisual(Application.Current.RootVisual);
                var p = gt.Transform(new Point(0, b.ActualHeight));

                MyPopup.HorizontalOffset = p.X;
                MyPopup.VerticalOffset = 100;

                MyPopup.IsOpen = true;
                var salesorder = MainGrid.SelectedItem as TblSalesOrder;
                //_viewModel.Operation(salesorder.Iserial);
                Client.GetTblSalesOrderOperationAsync(salesorder.Iserial);

                Client.GetTblSalesOrderOperationCompleted += (s, sv) =>
                {
                    SalesOrderOperationGrid.ItemsSource = sv.Result;
                };
            }
        }

        private void MainGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
    }
}