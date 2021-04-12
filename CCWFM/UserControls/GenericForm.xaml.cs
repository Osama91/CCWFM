using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using Lite.ExcelLibrary.SpreadSheet;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls
{
    public partial class GenericForm
    {
        private readonly GenericViewModelCollection _viewModel;

        public GenericForm(string tablEname, PermissionItemName userJobsForm)
        {
            InitializeComponent();
            _viewModel = new GenericViewModelCollection(tablEname, userJobsForm);

            DataContext = _viewModel;
        }

        public GenericForm(string tablEname, string userJobsForm, PermissionItemName factoryGroupForm)
        {
            InitializeComponent();
            _viewModel = new GenericViewModelCollection(tablEname, factoryGroupForm);

            DataContext = _viewModel;
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(false);
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (GenericViewModel row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row);
            }

            _viewModel.Delete();
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
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
                foreach (GenericViewModel row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row);
                }

                _viewModel.Delete();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.Code = _viewModel.Aname = _viewModel.Ename = null;
            foreach (ColumnFilterControl f in e.FiltersPredicate)
            {
                _viewModel.Filter = null;
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.Contains:
                        _viewModel.Filter = "%" + f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.EndsWith:
                        _viewModel.Filter = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        _viewModel.Filter = f.FilterText + "%";
                        break;
                }
                switch (f.FilterColumnInfo.PropertyPath)
                {
                    case "Code":
                        _viewModel.Code = _viewModel.Filter;
                        break;

                    case "Ename":
                        _viewModel.Ename = _viewModel.Filter;
                        break;

                    case "AName":
                        _viewModel.Aname = _viewModel.Filter;
                        break;
                }
            }
            if (!_viewModel.Loading)
            {
                _viewModel.GetMaindata();
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            
            var salesOrderlist = new ObservableCollection<GenericViewModel>();
            var oFile = new OpenFileDialog();
            oFile.Filter = "Excel (*.xls)|*.xls";

            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();

                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];

                var code = 0;
                var aname = 0;
                var ename = 0;
                for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "Code":
                            code = j;
                            break;

                        case "Aname":
                            aname = j;
                            break;

                        case "Ename":
                            ename = j;
                            break;

                    
                    }

                    for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex; i++)
                    {

                        var newemp = new GenericViewModel();
                        
                        newemp.Code = sheet.Cells[i, code].StringValue.Trim();
                        newemp.Aname = sheet.Cells[i, aname].StringValue.Trim();
                        newemp.Ename = sheet.Cells[i, ename].StringValue.Trim();

                     
                    

                        salesOrderlist.Add(newemp);
                    }
                }
            }

            _viewModel.SaveImported(salesOrderlist);

        }
    }
}