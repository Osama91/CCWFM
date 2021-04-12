using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchAssets
    {
        private readonly AssetsViewModel _assetsViewModel;
        private readonly SearchAssetsViewModel _viewModel;
        public SearchAssets(AssetsViewModel assetsViewModel)
        {
            InitializeComponent();
            _assetsViewModel = assetsViewModel;
            _viewModel = (SearchAssetsViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }
        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }
        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {

            _viewModel.MainRowList.Clear();
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

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

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            _assetsViewModel.SubmitSearch(MainGrid.SelectedItem as TblAssetsViewModel);
            
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }
    }
}

