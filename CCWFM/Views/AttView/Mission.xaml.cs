using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AttViewModel;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.AttView
{
    public partial class Mission
    {
        private readonly MissionViewModel _viewModel;

        public Mission()
        {
            InitializeComponent();

            _viewModel = (MissionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
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
                    _viewModel.SelectedMainRows.Add(row as TblMissionViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void AddBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedItem != null);
        }

        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblMissionViewModel);
            }
            _viewModel.DeleteMainRow();
        }
    }
}