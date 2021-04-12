using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class Family
    {
        private readonly FamilyViewModel _viewModel;

        public Family()
        {
            InitializeComponent();
            _viewModel = (FamilyViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblFamilyViewModel);
            }
            _viewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ExportGrid == MainGrid)
            {
                _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
            }
            else
            {
                _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
            }
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
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblFamilyViewModel);
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
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid.CommitEdit();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.SelectedMainRow.DetailsList.Count < _viewModel.DetailFullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetDetailData();
            }
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblSubFamilyViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedDetailRow != null && _viewModel.SelectedDetailRow.Iserial != 0)
            {
                DetailGrid.ExportExcel("SubFamily");
            }
            else
            {
                MainGrid.ExportExcel("Family");
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Report();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }
    }
}