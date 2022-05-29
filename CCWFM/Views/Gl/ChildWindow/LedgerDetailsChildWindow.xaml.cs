using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.Gl;
using Lite.ExcelLibrary.SpreadSheet;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class LedgerDetailChildWindow
    {
        private readonly LedgerHeaderViewModel _viewModel;


        public LedgerDetailChildWindow(LedgerHeaderViewModel viewModel,bool posted)
        {
            InitializeComponent();
            if (posted)
            {
                DetailGrid.IsReadOnly = true;
            }
           

            viewModel.PostedMode = posted;
            TxtBalanceJournalValue.Text = "0";
            TxtBalancePerVoucherValue.Text = "0";
            TxtCreditJournalValue.Text = "0";
            TxtCreditPerVoucherValue.Text = "0";
            TxtDebitJournalValue.Text = "0";
            TxtDebitPerVouchervalue.Text = "0";
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.GetDetailData(posted);
            if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithApproval") != null || _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithoutApproval") != null)
            {
                BtnPost.Visibility = Visibility.Visible;
            }
            if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "LedgerApprove") != null)
            {
                BtnApprove.Visibility = Visibility.Visible;
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            CalcTotal();

            _viewModel.SaveDetailRow();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true,false);
                    DetailGrid.BeginEdit();
                }
            }
            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add((TblLedgerMainDetailViewModel)row);
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

        public void CalcTotal()
        {
            var credittotalPosted = _viewModel.SelectedMainRow.DetailsList.Sum(x => x.CrAmount * (decimal)x.ExchangeRate) +
                        _viewModel.SelectedMainRow.DetailsList.Where(x => x.OffsetEntityAccount != null)
                            .Sum(x => x.DrAmount * (decimal)x.ExchangeRate) ?? 0;
            if (TxtCreditJournalValue.Text != String.Format("{0:0.00}", credittotalPosted))
            {
                string value = String.Format("{0:0.00}", credittotalPosted);
                TxtCreditJournalValue.Text = value;
            }
            var debittotalPosted = _viewModel.SelectedMainRow.DetailsList.Sum(x => x.DrAmount * (decimal)x.ExchangeRate) +
                             _viewModel.SelectedMainRow.DetailsList.Where(x => x.OffsetEntityAccount != null)
                                 .Sum(x => x.CrAmount * (decimal)x.ExchangeRate) ?? 0;
            if (TxtDebitJournalValue.Text != String.Format("{0:0.00}", debittotalPosted))
            {
                TxtDebitJournalValue.Text = String.Format("{0:0.00}", debittotalPosted);
            }

            if (TxtBalanceJournalValue.Text != String.Format("{0:0.00}", (debittotalPosted - credittotalPosted)))
            {
                TxtBalanceJournalValue.Text = String.Format("{0:0.00}", (debittotalPosted - credittotalPosted));
            }
            if (_viewModel.SelectedDetailRow != null)
            {
                var crAmount = _viewModel.SelectedDetailRow.CrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                if (_viewModel.SelectedDetailRow.OffsetEntityAccount != null)
                {
                    crAmount = crAmount + _viewModel.SelectedDetailRow.DrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                }
                var drAmount = _viewModel.SelectedDetailRow.DrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                if (_viewModel.SelectedDetailRow.OffsetEntityAccount != null)
                {
                    drAmount = drAmount + _viewModel.SelectedDetailRow.CrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                }

                if (TxtCreditPerVoucherValue.Text != String.Format("{0:0.00}", (crAmount)))
                {
                    TxtCreditPerVoucherValue.Text = String.Format("{0:0.00}", (crAmount));
                }
                if (TxtDebitPerVouchervalue.Text != String.Format("{0:0.00}", drAmount))
                {
                    TxtDebitPerVouchervalue.Text = String.Format("{0:0.00}", drAmount);
                }
                if (TxtBalancePerVoucherValue.Text != String.Format("{0:0.00}", (drAmount - crAmount)))
                {
                    TxtBalancePerVoucherValue.Text = String.Format("{0:0.00}", (drAmount - crAmount));
                }
                if ((debittotalPosted - credittotalPosted) < (decimal)0.01 && (credittotalPosted - debittotalPosted) < (decimal)0.01)
                {
                    _viewModel.SelectedMainRow.balanced = true;
                }
                else
                {
                    _viewModel.SelectedMainRow.balanced = false;
                }
            }

           
                var credittotal = _viewModel.SelectedMainRow.DetailsList.Sum(x => x.CrAmount * (decimal)x.ExchangeRate) +
                                    _viewModel.SelectedMainRow.DetailsList.Where(x => x.OffsetEntityAccount != null)
                                        .Sum(x => x.DrAmount * (decimal)x.ExchangeRate) ?? 0;
                if (TxtCreditJournalValue.Text != String.Format("{0:0.00}", credittotal))
                {
                    string value = String.Format("{0:0.00}", credittotal);
                    TxtCreditJournalValue.Text = value;
                }
                var debittotal = _viewModel.SelectedMainRow.DetailsList.Sum(x => x.DrAmount * (decimal)x.ExchangeRate) +
                                 _viewModel.SelectedMainRow.DetailsList.Where(x => x.OffsetEntityAccount != null)
                                     .Sum(x => x.CrAmount * (decimal)x.ExchangeRate) ?? 0;
                if (TxtDebitJournalValue.Text != String.Format("{0:0.00}", debittotal))
                {
                    TxtDebitJournalValue.Text = String.Format("{0:0.00}", debittotal);
                }

                if (TxtBalanceJournalValue.Text != String.Format("{0:0.00}", (debittotal - credittotal)))
                {
                    TxtBalanceJournalValue.Text = String.Format("{0:0.00}", (debittotal - credittotal));
                }
                if (_viewModel.SelectedDetailRow != null)
                {
                    var crAmount = _viewModel.SelectedDetailRow.CrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                    if (_viewModel.SelectedDetailRow.OffsetEntityAccount != null)
                    {
                        crAmount = crAmount + _viewModel.SelectedDetailRow.DrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                    }
                    var drAmount = _viewModel.SelectedDetailRow.DrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                    if (_viewModel.SelectedDetailRow.OffsetEntityAccount != null)
                    {
                        drAmount = drAmount + _viewModel.SelectedDetailRow.CrAmount * (decimal)_viewModel.SelectedDetailRow.ExchangeRate ?? 0;
                    }

                    if (TxtCreditPerVoucherValue.Text != String.Format("{0:0.00}", (crAmount)))
                    {
                        TxtCreditPerVoucherValue.Text = String.Format("{0:0.00}", (crAmount));
                    }
                    if (TxtDebitPerVouchervalue.Text != String.Format("{0:0.00}", drAmount))
                    {
                        TxtDebitPerVouchervalue.Text = String.Format("{0:0.00}", drAmount);
                    }
                    if (TxtBalancePerVoucherValue.Text != String.Format("{0:0.00}", (drAmount - crAmount)))
                    {
                        TxtBalancePerVoucherValue.Text = String.Format("{0:0.00}", (drAmount - crAmount));
                    }
                    if ((debittotal - credittotal) < (decimal)0.01 && (credittotal - debittotal) < (decimal)0.01)
                    {
                        _viewModel.SelectedMainRow.balanced = true;
                    }
                    else
                    {
                        _viewModel.SelectedMainRow.balanced = false;
                    }
                }

         }

        private void DetailGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedMainRow.balanced == false || _viewModel.SelectedMainRow.DetailsList.Sum(w => w.CrAmount) == 0
                      || _viewModel.SelectedMainRow.DetailsList.Sum(w => w.DrAmount) == 0
                      )
            {
                return;

            }
            if (_viewModel.SelectedMainRow.TblJournalLink != null || _viewModel.SelectedMainRow.Posted || _viewModel.SelectedMainRow.Approved)
            {
                e.Cancel = true;
            }

            if (_viewModel.SelectedDetailRow.Iserial == -1 || _viewModel.SelectedDetailRow.TransactionExists)
            {
                e.Cancel = true;
            }
            if (!_viewModel.AllowUpdate && !_viewModel.AddMode && _viewModel.SelectedDetailRow.Iserial != 0)
            {

                e.Cancel = true;

            }
        }

        private void TblPeriodLineDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TblLedgerMainDetailViewModel oldrow = null;
            if (e.RemovedItems.Count != 0)
            {
                oldrow = e.RemovedItems[0] as TblLedgerMainDetailViewModel;
            }

            if (oldrow != null && ((oldrow.CrAmount == 0 || oldrow.CrAmount == null) && (oldrow.DrAmount == 0 || oldrow.DrAmount == null)))
            {
                if (!_viewModel.AllowUpdate)
                {
                    _viewModel.SelectedDetailRow = oldrow;
                    return;
                }

            }
            if (_viewModel.SelectedDetailRow != null && !(_viewModel.SelectedMainRow.DetailsList.Count == 1 && _viewModel.SelectedDetailRow.CrAmount == 0 && _viewModel.SelectedDetailRow.DrAmount == 0))
            {
                CalcTotal();
            }

            if (!_viewModel.PostedMode) { 

            _viewModel.SaveDetailRow();

            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveOldDetailRow(variable as TblLedgerMainDetailViewModel);
            }
            }
        }

        private void CostCenter_OnClick(object sender, RoutedEventArgs e)
        {
            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(_viewModel.SelectedDetailRow,
                new ValidationContext(_viewModel.SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                MessageBox.Show("Data Is Not Valid");
            }
            else
            {
                _viewModel.SelectedDetailRow.DetailsList.Clear();
                CalcTotal();
                _viewModel.SaveDetailRow();
                var child = new LedgerDetailCostCenterChildWindow(_viewModel);
                child.Show();
            }
        }

        private void DetailGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = (DataGrid)sender;
        }

        private void CmdCurrency_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmd = sender as ComboBox;
            if (cmd != null)
            {
                if (cmd.SelectedValue != null)
                {
                    var value = (int)cmd.SelectedValue;
                    if (_viewModel.DefaultCurrencyIserial == value)
                    {
                        _viewModel.SelectedDetailRow.ExchangeRateEnabled = false;
                    }
                    else
                    {
                        _viewModel.SelectedDetailRow.ExchangeRateEnabled = true;
                    }
                }
            }
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void DataFormStyle_AddingNewItem(object sender, DataFormAddingNewItemEventArgs e)
        {
            e.Cancel = true;
            _viewModel.AddNewDetailRow(true, false);
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            DetailGrid.ExportExcel("Journal Detail");
            _viewModel.AllowExport = false;
        }

        private void BtnGetallData_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GetFullDetailData();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1, false);
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedDetailRows.Clear();
            foreach (var row in DetailGrid.SelectedItems)
            {
                _viewModel.SelectedDetailRows.Add((TblLedgerMainDetailViewModel)row);
            }
            _viewModel.DeleteDetailRow();
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            CalcTotal();
            _viewModel.SaveDetailRow();
        }

        private void BtnPost_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Post();
        }

        private void BtnApprove_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Approve();
        }

        private void TxtDr_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            try
            {
                if (!string.IsNullOrEmpty(txt.Text) && Convert.ToDecimal(txt.Text) != 0)
                {
                    _viewModel.SelectedDetailRow.CrAmount = 0;
                    _viewModel.SelectedDetailRow.DrOrCr = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void TxtCr_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            try
            {
                if (!string.IsNullOrEmpty(txt.Text) && Convert.ToDecimal(txt.Text) != 0)
                {
                    _viewModel.SelectedDetailRow.DrAmount = 0;
                    _viewModel.SelectedDetailRow.DrOrCr = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private void TxtDr_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.SelectAll();
        }

        private void TxtCr_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.SelectAll();
        }


        private void BtnCalcTotal_Click(object sender, RoutedEventArgs e)
        {
          _viewModel.PosTLedgerTotal();
        }
            


        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var ledgerMainList = new ObservableCollection<TblLedgerMainDetail1>();
            var oFile = new OpenFileDialog { Filter = "Excel (*.xls)|*.xls" };
            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();
                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];
                var description = 0;
                var currency = 0;
                var journalcccounttype = 0;
                var enity = 0;
                var account = 0;
                var debit = 0;
                var credit = 0;
                var costcentertype = 0;
                var costcenter = 0;
                var PaymentRef = 0;
                var Rate = 0;
                for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "description":
                            description = j;
                            break;

                        case "currency":
                            currency = j;
                            break;

                        case "journalaccounttype":
                            journalcccounttype = j;
                            break;

                        case "enity":
                            enity = j;
                            break;

                        case "account":
                            account = j;
                            break;

                        case "debit":
                            debit = j;
                            break;

                        case "credit":
                            credit = j;
                            break;

                        case "costcentertype":
                            costcentertype = j;
                            break;

                        case "costcenter":
                            costcenter = j;
                            break;
                        case "paymentref":
                            PaymentRef = j;
                            break;

                        case "rate":
                            Rate = j;
                            break;                            

                    }
                }

                for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
                {
                    var newemp = new TblLedgerMainDetail1();
                    decimal strAmount = 0;
                    if (sheet.Cells[i, debit].StringValue != ""&& sheet.Cells[i, debit].StringValue!="0")
                    {
                        newemp.DrOrCr = true;
                        try
                        {
                            strAmount = Convert.ToDecimal(sheet.Cells[i, debit].StringValue ?? "0");
                        }
                        catch (Exception)
                        {

                            strAmount = 0;
                        }
                     
                    }
                    else
                    {
                        newemp.DrOrCr = false;
                        try
                        {
                            strAmount = Convert.ToDecimal(sheet.Cells[i, credit].StringValue ?? "0");
                        }
                        catch (Exception)
                        {

                            strAmount = 0;
                        }
                       
                    }
                    newemp.Amount = strAmount;
                    if (sheet.Cells[i, Rate].StringValue != "")
                    {                    
                        strAmount = Convert.ToDecimal(sheet.Cells[i, Rate].StringValue ?? "0");
                    }
                    else
                    {                     
                        strAmount = Convert.ToDecimal(sheet.Cells[i, Rate].StringValue ?? "0");
                    }

                    newemp.ExchangeRate = (double)strAmount;
                    newemp.TblLedgerHeader1 = _viewModel.SelectedMainRow.Iserial;
                    var Description = sheet.Cells[i, description].Value.ToString().ToUpper().Trim();
                    newemp.Description = Description;
                    newemp.TblCurrency1 = new TblCurrencyTest
                    {
                        Ename = sheet.Cells[i, currency].StringValue.ToUpper().Trim(),
                        Code = sheet.Cells[i, currency].StringValue.ToUpper().Trim(),
                        Aname = sheet.Cells[i, currency].StringValue.ToUpper().Trim(),
                        Iserial = 0
                    };
                    newemp.TblJournalAccountType1 = new TblJournalAccountType
                    {
                        Ename = sheet.Cells[i, journalcccounttype].StringValue.ToUpper().Trim(),
                        Code = sheet.Cells[i, journalcccounttype].StringValue.ToUpper().Trim(),
                        Aname = sheet.Cells[i, journalcccounttype].StringValue.ToUpper().Trim(),
                        Iserial = 0
                    };
                    newemp.TblJournalAccountType2 = new TblJournalAccountType
                    {
                        Code = sheet.Cells[i, enity].StringValue.ToUpper().Trim(),
                        Ename = sheet.Cells[i, enity].StringValue.ToUpper().Trim(),
                        Aname = sheet.Cells[i, enity].StringValue.ToUpper().Trim(),
                        Iserial = 0
                    };
                    newemp.TblMethodOfPayment1 = new TblMethodOfPayment
                    {
                        Code = sheet.Cells[i, account].StringValue.ToUpper().Trim(),
                        Aname = sheet.Cells[i, account].StringValue.ToUpper().Trim(),
                        Ename = sheet.Cells[i, account].StringValue.ToUpper().Trim(),
                        Iserial = 0
                    };
                    newemp.PaymentRef = sheet.Cells[i, PaymentRef].StringValue.ToUpper().Trim();
                    newemp.TblLedgerDetail1CostCenter= new ObservableCollection<TblLedgerDetail1CostCenter>();
                    if (!string.IsNullOrWhiteSpace(sheet.Cells[i, costcentertype].StringValue.ToUpper().Trim()))
                    {
                        newemp.TblLedgerDetail1CostCenter.Add(new TblLedgerDetail1CostCenter
                        {
                            TblCostCenterType1 = new TblCostCenterType
                            {
                                Ename = sheet.Cells[i, costcentertype].StringValue.ToUpper().Trim(),
                                Aname = sheet.Cells[i, costcentertype].StringValue.ToUpper().Trim(),
                                Code = sheet.Cells[i, costcentertype].StringValue.ToUpper().Trim()
                            },
                            TblCostCenter1 = new TblCostCenter
                            {
                                Ename = sheet.Cells[i, costcenter].StringValue.ToUpper().Trim(),
                                Aname = sheet.Cells[i, costcenter].StringValue.ToUpper().Trim(),
                                Code = sheet.Cells[i, costcenter].StringValue.ToUpper().Trim()
                            }
                        });
                    }
                    ledgerMainList.Add(newemp);
                }
                _viewModel.InsertImportedLedgerMainDetail(ledgerMainList);
            }
        }

        private void LayoutRoot_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                CostCenter_OnClick(null, null);
            }
        }

        private void ChildWindowsOverride_Closed(object sender, EventArgs e)
        {

        }
    }
}