using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl
{
    public partial class JournalSetting
    {
        private readonly JournalSettingViewModel _viewModel;

        public JournalSetting()
        {
            InitializeComponent();
            _viewModel = (JournalSettingViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteMainRow();
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
            else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add((TblJournalSettingViewModel)row);
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
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void MainGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = sender as DataGrid;
        }

        private void MainGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldRow(variable as TblJournalSettingViewModel);
            }
            _viewModel.GetDetailData();
            _viewModel.GetSubDetailData();
        }


        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
          
            _viewModel.SaveDetailRow();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                    DetailGrid.BeginEdit();
                }
            }
            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add((TblJournalSettingEntityViewModel)row);
                }

                _viewModel.DeleteDetailRow();
            }
            else if (e.Key == Key.Tab)
            {
                if (DetailGrid.CurrentColumn != null)
                {
                    int index = DetailGrid.Columns.IndexOf(DetailGrid.CurrentColumn);
                    if (index == DetailGrid.Columns.Count - 1)
                    {
                        var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                        if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                        {
                        }
                    }
                }
            }
        }    
        private void DetailGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = (DataGrid)sender;
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TblJournalSettingEntityViewModel oldrow = null;
            if (e.RemovedItems.Count != 0)
            {
                oldrow = e.RemovedItems[0] as TblJournalSettingEntityViewModel;
            }
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldDetailRow(variable as TblJournalSettingEntityViewModel);
            }
        }


        private void SubDetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
        }

        private void SubDetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.SubDetailList.IndexOf(_viewModel.SelectedSubDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.SubDetailList.Count - 1))
                {
                    _viewModel.AddNewSubDetailRow(true);
                    DetailGrid.BeginEdit();
                }
            }
            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedSubDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedSubDetailRows.Add((TblJournalSettingCostCenterViewModel)row);
                }

                _viewModel.DeleteSubDetailRow();
            }
            else if (e.Key == Key.Tab)
            {
                if (DetailGrid.CurrentColumn != null)
                {
                    var index = DetailGrid.Columns.IndexOf(DetailGrid.CurrentColumn);
                    if (index == DetailGrid.Columns.Count - 1)
                    {
                        var currentRowIndex = (_viewModel.SelectedMainRow.SubDetailList.IndexOf(_viewModel.SelectedSubDetailRow));
                        if (currentRowIndex == (_viewModel.SelectedMainRow.SubDetailList.Count - 1))
                        {
                            _viewModel.AddNewSubDetailRow(true);
                            DetailGrid.BeginEdit();
                        }
                    }
                }
            }
        }

        private void SubDetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
            DetailGrid.BeginEdit();

        }
    }
}