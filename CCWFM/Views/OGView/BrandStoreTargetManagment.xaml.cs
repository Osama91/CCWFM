using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class BrandStoreTargetManagment
    {
        private readonly BrandStoreTargetManagmentViewModel _viewModel;

        public BrandStoreTargetManagment()
        {
            InitializeComponent();
            _viewModel = (BrandStoreTargetManagmentViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

     

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
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

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
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

        private void MainGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedMainRow != null && _viewModel.SelectedMainRow.Glserial != 0)
            {
                foreach (var variable in MainGrid.Columns)
                {
                    variable.IsReadOnly = true;
                }
                MainGrid.Columns.LastOrDefault().IsReadOnly = false;
            }
            else
            {
                foreach (var variable in MainGrid.Columns)
                {
                    variable.IsReadOnly = false;
                }
            }
        }
    }
}