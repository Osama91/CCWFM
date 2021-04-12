using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StoreIntialOrderChildWindow
    {
        private readonly SeasonalMasterListViewModel _viewModel;

        public StoreIntialOrderChildWindow(SeasonalMasterListViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.GetDetaildata();
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedDetailRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedDetailRows.Add(row as TblStoreIntialOrderViewModel);
            }
            _viewModel.DeleteDetailRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewDetailRow(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailList.Clear();

            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetDetaildata();
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailList.Count - 2 >= e.Row.GetIndex() || _viewModel.Loading) return;
            _viewModel.Loading = true;
            _viewModel.GetDetaildata();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblStoreIntialOrderViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }
    }
}