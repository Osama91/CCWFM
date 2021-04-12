using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;


namespace CCWFM.Views.StylePages
{
    public partial class SeasonalMasterListPage
    {
        private readonly ViewModel.OGViewModels.SeasonalMasterListViewModel _viewModel;

        public SeasonalMasterListPage()
        {
            InitializeComponent();
            _viewModel = (LayoutRoot.DataContext as ViewModel.OGViewModels.SeasonalMasterListViewModel);
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblSeasonalMasterListViewModel);
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
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

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

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        

            if (_viewModel.MainRowList.Count < 20)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 >= e.Row.GetIndex() || _viewModel.Loading) return;
            _viewModel.Loading = true;
            _viewModel.GetMaindata();
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
                    _viewModel.SelectedMainRows.Add(row as TblSeasonalMasterListViewModel);
                }

                _viewModel.DeleteMainRow();
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


        //private void StyleComplete_Populating(object sender, PopulatingEventArgs e)
        //{
        //    var autoComplete = sender as AutoCompleteBox;
        //    e.Cancel = true;
        //    _viewModel.SearchStyle(autoComplete.Text);
      
        //}

        //private void StyleComplete_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var autoComplete = sender as AutoCompleteBox;

        //    if (_viewModel != null)
        //        _viewModel.StyleCompletedCompleted  += (s, r) => autoComplete.PopulateComplete();
    
        //}

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PreviewReport();
        }
    }
}