using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl
{
    public partial class PositionRoute
    {
        private readonly PositionRouteViewModel _viewModel;

        public PositionRoute()
        {
            InitializeComponent();
            _viewModel = (PositionRouteViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblPositionRouteHeaderViewModel);
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

                foreach (var variable in e.RemovedItems)
                {
                    _viewModel.SaveOldRow(variable as TblPositionRouteHeaderViewModel);
                }
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
            else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblPositionRouteHeaderViewModel);
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
            else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblPositionRouteViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldDetailRow(variable as TblPositionRouteViewModel);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.MainRowList != null)
            {
                _viewModel.MainRowList.Clear();
            }
            if (_viewModel.SelectedMainRow != null) _viewModel.SelectedMainRow.DetailsList.Clear();
            _viewModel.GetMaindata();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            const string reportName = "GlGenericEntity";
            var para = new ObservableCollection<string>();

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void MainGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = (DataGrid)sender;
        }
    }
}