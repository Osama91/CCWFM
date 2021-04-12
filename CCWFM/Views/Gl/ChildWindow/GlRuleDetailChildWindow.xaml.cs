using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class GlRuleDetailChildWindow
    {
        private readonly GlRuleViewModel _viewModel;

        public GlRuleDetailChildWindow(GlRuleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            viewModel.GetSubDetailData();
        }

        private void DetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedDetailRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailSubFilter = filter;
            _viewModel.DetailSubValuesObjects = valueObjecttemp;

            _viewModel.GetSubDetailData();
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedDetailRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedDetailRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetSubDetailData();
            }
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.DetailsList.IndexOf(_viewModel.SelectedSubDetailRow));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewSubDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedSubDetailRows.Clear();
                foreach (var row in TblPeriodLineDataGrid.SelectedItems)
                {
                    _viewModel.SelectedSubDetailRows.Add((TblGlRuleDetailViewModel)row);
                }

                _viewModel.DeleteSubDetailRow();
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var cmb = sender as ComboBox;
            if (cmb != null)
            {
                var value = (int) cmb.SelectedValue;
                _viewModel.GlInventoryGroupListFiltered = new ObservableCollection<GlInventoryGroup_Result>(_viewModel.GlInventoryGroupList.Where(x => x.TblJournalAccountType == value).OrderBy(x=>x.Ename));
            }
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldSubDetailRow(variable as TblGlRuleDetailViewModel);
            }
        }

    }
}