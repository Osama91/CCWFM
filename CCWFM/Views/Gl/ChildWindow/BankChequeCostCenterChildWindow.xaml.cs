using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class BankChequeCostCenterChildWindow
    {
        private readonly GlChequeTransactionViewModel _viewModel;

        public BankChequeCostCenterChildWindow(GlChequeTransactionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.GetSubDetailData();
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.DetailsList.IndexOf(_viewModel.SelectedSubDetailRow));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.DetailsList.Count - 1))
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
                    _viewModel.SelectedSubDetailRows.Add((TblBankChequeCostCenterViewModel)row);
                }

                _viewModel.DeleteSubDetailRow();
            }
            //else if (e.Key == Key.Tab)
            //{
            //    if (DetailGrid.CurrentColumn != null)
            //    {
            //        var index = DetailGrid.Columns.IndexOf(DetailGrid.CurrentColumn);
            //        if (index == DetailGrid.Columns.Count - 1)
            //        {
            //            var currentRowIndex = (_viewModel.SelectedDetailRow.DetailsList.IndexOf(_viewModel.SelectedSubDetailRow));
            //            if (currentRowIndex == (_viewModel.SelectedDetailRow.DetailsList.Count - 1))
            //            {
            //                _viewModel.AddNewSubDetailRow(true);
            //                DetailGrid.BeginEdit();
            //            }
            //        }
            //    }
            //}
        }

        private void TblPeriodLineDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
            DetailGrid.BeginEdit();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedDetailRow != null)
            {
                var amount = _viewModel.SelectedDetailRow.Amount;

                if (_viewModel.SelectedDetailRow.Amount == 0)
                {
                    amount = _viewModel.SelectedDetailRow.Amount;
                }
                var cmb = sender as ComboBox;
                if (_viewModel.SelectedSubDetailRow != null && (cmb.SelectedValue != null && _viewModel.SelectedSubDetailRow.Iserial == 0))
                {
                    var value = (int)cmb.SelectedValue;
                    var types = _viewModel.SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType != value && x.TblCostCenterType != null).Select(x => x.TblCostCenterType).Distinct();

                    foreach (var type in types)
                    {
                        if (
                            _viewModel.SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == type).Sum(x => x.Amount) !=
                            (double)amount)
                        {
                            MessageBox.Show("The Previous Type Settings Is Not Correct");
                            _viewModel.SelectedSubDetailRow.CostCenterTypePerRow = null;
                            _viewModel.SelectedSubDetailRow.TblCostCenterType = null;
                            return;
                        }
                    }
                }
            }
        }

        private void DetailGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = (DataGrid)sender;
            _viewModel.ExportGrid.BeginEdit();

        }

        private void TxtRatio_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;

            if (txt != null && txt.Text != null)
            {
                if (_viewModel.SelectedDetailRow.Amount > 0)
                {
                    _viewModel.SelectedSubDetailRow.Amount = (double)_viewModel.SelectedDetailRow.Amount * (Convert.ToDouble(txt.Text)) / 100;
                }
                else
                {
                    _viewModel.SelectedSubDetailRow.Amount = (double)(_viewModel.SelectedDetailRow.Amount * (Convert.ToDouble(txt.Text)) / 100);
                }
            }
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void DetailGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            DetailGrid.SelectedIndex = 0;
            DetailGrid.BeginEdit();
        }
    }
}