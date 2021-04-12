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
    public partial class SalaryTerms
    {
       

        public SalaryTerms()
        {
            InitializeComponent();
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

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            //var salaryList = new ObservableCollection<BankDepositService.TblSalaryApprovalHeader>();
            //var salaryDetaillist = new ObservableCollection<BankDepositService.TblSalaryApprovalDetail>();
            var oFile = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };

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
                    //var newemp = new BankDepositService.TblSalaryApprovalHeader();
                    //var newDetail = new BankDepositService.TblSalaryApprovalDetail();
                    //var salaryTypeStr = sheet.Cells[i, SalaryType].StringValue.ToUpper().Trim();
                    //int monthStr = 0;
                    //var CodeStr = sheet.Cells[i, code].StringValue.ToUpper().Trim();


                    //if (string.IsNullOrEmpty(CodeStr) || string.IsNullOrEmpty(salaryTypeStr))
                    //{
                    //    continue;
                    //}

                    //if (!int.TryParse(sheet.Cells[i, Month].StringValue.ToUpper().Trim(), out monthStr))
                    //{
                    //    continue;
                    //}

                    //if (!int.TryParse(sheet.Cells[i, emp].StringValue.ToUpper().Trim(), out employee))
                    //{
                    //    continue;
                    //}

                    //var yearStr = Convert.ToInt32(sheet.Cells[i, Year].StringValue.ToUpper().Trim());
                    //var DueDateValue = sheet.Cells[i, duedate].DateTimeValue;
                    //var shopStr = sheet.Cells[i, shop].StringValue.ToUpper().Trim();
                    //if (!salaryList.Any(x => x.TblStore1.code.ToUpper().Trim() == shopStr
                    //&& x.TblSalaryType1.EName.ToUpper().Trim() == salaryTypeStr
                    //&& x.Code.ToUpper().Trim() == CodeStr
                    //&& x.Month == monthStr && x.Year == yearStr))
                    //{
                    //    if (string.IsNullOrEmpty(CodeStr) || string.IsNullOrEmpty(salaryTypeStr))
                    //    {
                    //        continue;
                    //    }
                    //    newemp.TblSalaryApprovalDetails = new ObservableCollection<BankDepositService.TblSalaryApprovalDetail>();
                    //    newemp.TblSalaryType1 = new BankDepositService.TblSalaryType();
                    //    newemp.TblSalaryType1.EName = salaryTypeStr;
                    //    newemp.TblSalaryType1.Code = salaryTypeStr;
                    //    newemp.TblSalaryType1.AName = salaryTypeStr;
                    //    newemp.TblStore1 = new BankDepositService.TblStore();
                    //    newemp.TblStore1.code = shopStr;
                    //    newemp.TblStore1.ENAME = shopStr;
                    //    newemp.TblStore1.aname = shopStr;
                    //    newemp.Code = CodeStr;
                    //    newemp.Month = monthStr;
                    //    newemp.Year = yearStr;
                    //    newemp.DueDate = DueDateValue;
                    //    salaryList.Add(newemp);
                    //}
                    //newDetail.Salary = Convert.ToDecimal(sheet.Cells[i, Net].StringValue.ToUpper().Trim());
                    //int employee = 0;
                    //if (!int.TryParse(sheet.Cells[i, emp].StringValue.ToUpper().Trim(),out employee))
                    //{
                    //    continue;
                    //}
                    //newDetail.TblEmployee = employee;
                    //salaryList.FirstOrDefault(w => w.TblStore1.code == shopStr && w.TblSalaryType1.EName == salaryTypeStr
                    //&& w.Month == monthStr && w.Year == yearStr
                    //).TblSalaryApprovalDetails.Add(newDetail);
                }

              //  _viewModel.InsertImportedSalaryApprovals(salaryList);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}