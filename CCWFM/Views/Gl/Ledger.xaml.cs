using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;


namespace CCWFM.Views.Gl
{
    public partial class Ledger
    {
        private readonly LedgerHeaderViewModel _viewModel;
        private int _counter;

        public Ledger()
        {
            InitializeComponent();
            _viewModel = (LedgerHeaderViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            _viewModel.PremCompleted += (s, sv) =>
            {
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithApproval") != null || _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithoutApproval") != null)
                {
                    MainGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "Posted").Visibility = Visibility.Visible;
                }
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerApprove") != null)
                {
                    MainGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "Approved").Visibility =
                        Visibility.Visible;
                }
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "ReverseGLTransaction") != null)
                {
                    BtnReverse.Visibility = Visibility.Visible;
                }
            };
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle.SelectedIndex == 0)
            {
                _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add((TblLedgerHeaderViewModel)row);
            }
            _viewModel.DeleteMainRow(true);
            }
            else
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGridUnposted.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add((TblLedgerHeaderViewModel)row);
                }
                _viewModel.DeleteMainRow(false);
            }
             }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle.SelectedIndex == 0)
            {
                _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1,true);
            }
            else {
                _viewModel.AddNewMainRow(MainGridUnposted.SelectedIndex != -1,false);
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            var grid = sender as OsGrid;
            if (grid.Name == "MainGrid")
            {

                _viewModel.MainRowList.Clear();
            }
            else
            {

                _viewModel.MainRowListUnPosted.Clear();
            }
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            if (grid.Name == "MainGrid")
            {
                _viewModel.GetMaindata(true);
            }
            else {
                _viewModel.GetMaindata(false);

            }
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {

            var grid = sender as OsGrid;

            if (grid.Name == "MainGrid")
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                    if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                    {
                        _viewModel.AddNewMainRow(true, true);
                        MainGrid.BeginEdit();
                    }
                }
            }
            else
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel.MainRowListUnPosted.IndexOf(_viewModel.SelectedMainRow));
                    if (currentRowIndex == (_viewModel.MainRowListUnPosted.Count - 1))
                    {
                        _viewModel.AddNewMainRow(true, false);
                        MainGridUnposted.BeginEdit();
                    }
                }

            }

            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                if (grid.Name == "MainGrid")
                {
                    _viewModel.SelectedMainRows.Clear();
                    foreach (var row in MainGrid.SelectedItems)
                    {
                        _viewModel.SelectedMainRows.Add((TblLedgerHeaderViewModel)row);
                    }
                    _viewModel.DeleteMainRow(true);
                 }
                else {
                    _viewModel.SelectedMainRows.Clear();
                    foreach (var row in MainGridUnposted.SelectedItems)
                    {
                        _viewModel.SelectedMainRows.Add((TblLedgerHeaderViewModel)row);
                    }
                    _viewModel.DeleteMainRow(false);
                }
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {

                if (grid.Name == "MainGrid")
                {
                    if (MainGrid.CurrentColumn != null)
                    {
                        int index = MainGrid.Columns.IndexOf(MainGrid.CurrentColumn);
                        if (index == MainGrid.Columns.Count - 1)
                        {
                            var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                            if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                            {
                            }
                        }
                    }
                }
                else
                {
                    if (MainGridUnposted.CurrentColumn != null)
                    {
                        int index = MainGridUnposted.Columns.IndexOf(MainGridUnposted.CurrentColumn);
                        if (index == MainGrid.Columns.Count - 1)
                        {
                            var currentRowIndex = (_viewModel.MainRowListUnPosted.IndexOf(_viewModel.SelectedMainRow));
                            if (currentRowIndex == (_viewModel.MainRowListUnPosted.Count - 1))
                            {
                            }
                        }
                    }
                }
            }
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            var grid = sender as OsGrid;
            if (grid.Name == "MainGrid")
            {
                if (_viewModel.MainRowList.Count < _viewModel.PageSize)
                {
                    return;
                }
                if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                    _viewModel.MainRowList.Count < _viewModel.FullCount)
                {
                    _viewModel.GetMaindata(true);
                }
            }
            else
            {
                if (_viewModel.MainRowListUnPosted.Count < _viewModel.PageSize)
                {
                    return;
                }
                if (_viewModel.MainRowListUnPosted.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                    _viewModel.MainRowListUnPosted.Count < _viewModel.FullCount)
                {
                    _viewModel.GetMaindata(false);
                }

            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var grid = sender as OsGrid;
            if (grid.Name == "MainGrid")
            {
                _viewModel.SaveMainRow(true);
            }
            else
            {
                _viewModel.SaveMainRow(false);

            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid.CommitEdit();
        }

        private void BtnReverse_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.Reverse();
        }


        

        private void MainGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldRow(variable as TblLedgerHeaderViewModel);
            }
        }

        private void MainGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = sender as DataGrid;
            var ww = MainGrid.FilterHeaders.FirstOrDefault(x => (string)x.Column.Header == "balanced");
            if (ww != null)
            {
                if (!ww.Collection.Select(x => x.Name).Contains("Balanced"))
                {
                    ww.Collection.Clear();
                    ww.Collection.Add(new ColumnFilterControl.CollectionTemp { ItemValue = "True", Name = "Balanced" });
                    ww.Collection.Add(new ColumnFilterControl.CollectionTemp { ItemValue = "False", Name = "UnBalanced" });
                }
            }
        }

        private void BtnDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var name=btn.Name;

            var valiationCollection = new List<ValidationResult>();

        
                var isvalid = Validator.TryValidateObject(_viewModel.SelectedMainRow,
                    new ValidationContext(_viewModel.SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    MessageBox.Show("Data Is Not Valid");
                }
                else
                {
                    //_viewModel.SaveMainRow(true);
                    _viewModel.SelectedMainRow.DetailsList.Clear();


                if (name == "BtnDetails")
                {
                    _viewModel.SelectedMainRow.DetailsList.Clear();
                    var child = new LedgerDetailChildWindow(_viewModel, true);
                    child.Show();

                }
                else {
                    _viewModel.SaveMainRow(false);
                    _viewModel.SelectedMainRow.DetailsList.Clear();
                    var child = new LedgerDetailChildWindow(_viewModel, false);
                    child.Show();
                }
            }

            
         

        }

        private void MainGrid_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = (DataGrid)sender;
        }

        private void MainGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedMainRow.TblJournalLink != null || _viewModel.SelectedMainRow.Posted || _viewModel.SelectedMainRow.Approved)
            {
                e.Cancel = true;
            }
            if (!_viewModel.AllowUpdate && _viewModel.SelectedMainRow.Iserial != 0)
            {
                e.Cancel = true;

            }
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PrintLedgerHeader(_viewModel.SelectedMainRow.Iserial);

        }

        private void Post_OnChecked(object sender, RoutedEventArgs e)
        {
         //   _viewModel.Post();
        }

        private void Approve_OnChecked(object sender, RoutedEventArgs e)
        {
            _viewModel.Approve();
        }

        private void LayoutRoot_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                BtnDetails_OnClick(null, null);
            }
        }

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;

            if (datepicker != null && (datepicker.Text.Length < 6 || _counter < 6))
            {
                try
                {
                    DateTime datetime = Convert.ToDateTime(datepicker.Text + "-" + DateTime.Now.Year);
                    datepicker.SelectedDate = datetime;
                }
                catch (Exception)
                {
                }
            }
        }

        private void DatePicker_KeyDown(object sender, KeyEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker != null && datepicker.Text != null)
            {
                _counter = datepicker.Text.Length;
                if (e.Key == Key.Enter)
                {
                    if (datepicker.Text.Length < 6)
                    {
                        try
                        {
                            DateTime datetime = Convert.ToDateTime(datepicker.Text + "-" + DateTime.Now.Year);
                            datepicker.SelectedDate = datetime;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private void BtnSalesOrderAttachment_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            if (btn != null)
            {
                var childWindow = new LedgerAttachmentChildWindow(btn.DataContext as TblLedgerHeaderViewModel);
                childWindow.Show();
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var Dp = sender as DatePicker;
            if (_viewModel.PeriodLines == null)
            {
                return;
            }
            if (Dp.SelectedDate != null)
            {

                if (_viewModel.PeriodLines.Any())
                {
                    var selectedperiod = _viewModel.PeriodLines.FirstOrDefault(w => w.FromDate <= Dp.SelectedDate.Value && w.ToDate >= Dp.SelectedDate.Value);

                    if (selectedperiod != null)
                    {
                        if (selectedperiod.Locked)
                        {
                            MessageBox.Show("This Period is Locked ");
                            Dp.SelectedDate = DateTime.Now;
                        }
                    }
                }
            }
            // PeriodLines
        }

        private void TabPosted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}