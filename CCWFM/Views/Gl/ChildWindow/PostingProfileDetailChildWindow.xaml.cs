using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class PostingProfileDetailChildWindow
    {
        private readonly PostingProfileHeaderViewModel _viewModel;

        public PostingProfileDetailChildWindow(PostingProfileHeaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.GetDetailData();
        }

        private void DetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailsValuesObjects = valueObjecttemp;

            _viewModel.GetDetailData();
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
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
            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in TblPeriodLineDataGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add((TblPostingProfileDetailViewModel)row);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldDetailRow(variable as TblPostingProfileDetailViewModel);
            }
        }
    }
}