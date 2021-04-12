using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;
using Lite.ExcelLibrary.SpreadSheet;
using System.Linq;
using System;

namespace CCWFM.Views.Gl
{
    public partial class SalaryApproval
    {
        private readonly SalaryApprovalViewModel _viewModel;

        public SalaryApproval()
        {
            InitializeComponent();
            _viewModel = (SalaryApprovalViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.SelectedMainRows.Clear();
            //foreach (var row in MainGrid.SelectedItems)
            //{
            //    _viewModel.SelectedMainRows.Add(row as TblSalaryApprovalHeaderModel);
            //}
            //_viewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            //if (_viewModel.ExportGrid == MainGrid)
            //{
            //    _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
            //}
            //else
            //{
            //    _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
            //}
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

                //foreach (var variable in e.RemovedItems)
                //{
                //    _viewModel.SaveOldRow(variable as TblSalaryApprovalHeaderModel);
                //}
            }
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Down)
            //{
            //    var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
            //    if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
            //    {
            //        _viewModel.AddNewMainRow(true);
            //    }
            //}
            //else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            //{
            //    _viewModel.SelectedMainRows.Clear();
            //    foreach (var row in MainGrid.SelectedItems)
            //    {
            //        _viewModel.SelectedMainRows.Add(row as TblSalaryApprovalHeaderModel);
            //    }

            //    _viewModel.DeleteMainRow();
            //}
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
                    //_viewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblSalaryApprovalDetailModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            //_viewModel.SaveDetailRow();
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (var variable in e.RemovedItems)
            //{
            //    _viewModel.SaveOldDetailRow(variable as TblSalaryApprovalDetailModel);
            //}
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

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var salaryList = new ObservableCollection<BankDepositService.TblSalaryApprovalHeader>();
            var salaryDetaillist = new ObservableCollection<BankDepositService.TblSalaryApprovalDetail>();
            var oFile = new OpenFileDialog { Filter = "Excel (*.xls)|*.xls" };

            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();

                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];

                var shop = 0;
                var emp = 0;
                var code = 0;
                var Net = 0;
                var Month = 0;
                var Year = 0;
                var SalaryType = 0;
                var duedate = 0;
                for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "shop":
                            shop = j;
                            break;
                        case "empcode":
                            emp = j;
                            break;
                        case "code":
                            code = j;
                            break;
                        case "net":
                            Net = j;
                            break;
                        case "salarytype":
                            SalaryType = j;
                            break;
                        case "month":
                            Month = j;
                            break;
                        case "year":
                            Year = j;
                            break;
                        case "duedate":
                            duedate = j;
                            break;
                    }
                }

                for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
                {
                    var newemp = new BankDepositService.TblSalaryApprovalHeader();
                    var newDetail = new BankDepositService.TblSalaryApprovalDetail();
                    var salaryTypeStr = sheet.Cells[i, SalaryType].StringValue.ToUpper().Trim();
                    int monthStr = 0;
                    var CodeStr = sheet.Cells[i, code].StringValue.ToUpper().Trim();


                    if (string.IsNullOrEmpty(CodeStr) || string.IsNullOrEmpty(salaryTypeStr))
                    {
                        continue;
                    }

                    if (!int.TryParse(sheet.Cells[i, Month].StringValue.ToUpper().Trim(), out monthStr))
                    {
                        continue;
                    }

                    //if (!int.TryParse(sheet.Cells[i, emp].StringValue.ToUpper().Trim(), out employee))
                    //{
                    //    continue;
                    //}

                    var yearStr = Convert.ToInt32(sheet.Cells[i, Year].StringValue.ToUpper().Trim());
                    var DueDateValue = sheet.Cells[i, duedate].DateTimeValue;
                    var shopStr = sheet.Cells[i, shop].StringValue.ToUpper().Trim();
                    if (!salaryList.Any(x => x.TblStore1.code.ToUpper().Trim() == shopStr
                    && x.TblSalaryType1.EName.ToUpper().Trim() == salaryTypeStr
                    && x.Code.ToUpper().Trim() == CodeStr
                    && x.Month == monthStr && x.Year == yearStr))
                    {
                        if (string.IsNullOrEmpty(CodeStr) || string.IsNullOrEmpty(salaryTypeStr))
                        {
                            continue;
                        }
                        newemp.TblSalaryApprovalDetails = new ObservableCollection<BankDepositService.TblSalaryApprovalDetail>();
                        newemp.TblSalaryType1 = new BankDepositService.TblSalaryType();
                        newemp.TblSalaryType1.EName = salaryTypeStr;
                        newemp.TblSalaryType1.Code = salaryTypeStr;
                        newemp.TblSalaryType1.AName = salaryTypeStr;
                        newemp.TblStore1 = new BankDepositService.TblStore();
                        newemp.TblStore1.code = shopStr;
                        newemp.TblStore1.ENAME = shopStr;
                        newemp.TblStore1.aname = shopStr;
                        newemp.Code = CodeStr;
                        newemp.Month = monthStr;
                        newemp.Year = yearStr;
                        newemp.DueDate = DueDateValue;
                        salaryList.Add(newemp);
                    }
                    newDetail.Salary = Convert.ToDecimal(sheet.Cells[i, Net].StringValue.ToUpper().Trim());
                    int employee = 0;
                    if (!int.TryParse(sheet.Cells[i, emp].StringValue.ToUpper().Trim(),out employee))
                    {
                        continue;
                    }
                    newDetail.TblEmployee = employee;
                    salaryList.FirstOrDefault(w => w.TblStore1.code == shopStr && w.TblSalaryType1.EName == salaryTypeStr
                    && w.Month == monthStr && w.Year == yearStr
                    ).TblSalaryApprovalDetails.Add(newDetail);
                }

                _viewModel.InsertImportedSalaryApprovals(salaryList);
            }
        }
    }
}