using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AccessoriesViewModel;
using CCWFM.Views.OGView.ChildWindows;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.AccessoryTools
{
    public partial class AccessoryGroup
    {
        private readonly AccGroupViewModel _viewModel;

        public AccessoryGroup()
        {
            InitializeComponent();
            _viewModel = (AccGroupViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblLkpAccessoryGroupViewModel);
            }
            _viewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(false);
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                _viewModel.SelectedMainRow.DetailsList.Clear();

                _viewModel.GetDetailData();
            }
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewMainRow(true);
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblLkpAccessoryGroupViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            var counter = 0;
            _viewModel.DetailFilter = null;

            _viewModel.DetailValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
                var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
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

                _viewModel.DetailValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.DetailFilter = _viewModel.DetailFilter + " and ";
                }

                _viewModel.DetailFilter = _viewModel.DetailFilter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetDetailData();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewDetailRow(true);
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblAccessoriesSubGroupViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetDetailData();
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void Sizes_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GetDetailSubData();
            var childwindows = new AccSubGroupSizes(_viewModel);
            childwindows.Show();
        }
    }
}