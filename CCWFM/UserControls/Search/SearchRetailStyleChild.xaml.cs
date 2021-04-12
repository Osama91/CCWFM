using System;
using System.Collections.Generic;
using System.Windows;
using CCWFM.CRUDManagerService;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchRetailStyleChild
    {
        private readonly SearchRetailStyleViewModel _viewModel = new SearchRetailStyleViewModel();
        private readonly SearchRetailStyle _userControl;

        public SearchRetailStyleChild(SearchRetailStyle searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;

            _viewModel.GetMaindata(_userControl.Brand.Iserial);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as viewstyle;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata(_userControl.Brand.Iserial);
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
            if (!_viewModel.Loading)
            {
                _viewModel.GetMaindata(_userControl.Brand.Iserial);
            }
        }

        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }
    }
}