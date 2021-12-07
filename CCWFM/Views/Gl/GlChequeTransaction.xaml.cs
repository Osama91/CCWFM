using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using CCWFM.Views.PrintPreviews;
using Omu.ValueInjecter.Silverlight;
using Lite.ExcelLibrary.SpreadSheet;
using System;

namespace CCWFM.Views.Gl
{
    public partial class GlChequeTransaction
    {
        private readonly GlChequeTransactionViewModel _viewModel;
        private readonly TblGlChequeTypeSetting _setting;

        public GlChequeTransaction(TblGlChequeTypeSetting setting, ObservableCollection<Entity> entityList)
        {
            InitializeComponent();
            _setting = setting;
            _viewModel = new GlChequeTransactionViewModel(setting, entityList);
            DataContext = _viewModel;
            MyPopup.DataContext = _viewModel;
            _viewModel.PremCompleted += (s, sv) =>
            {
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "Approval") != null)
                {
                    BtnApprove.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnApprove.Visibility = Visibility.Collapsed;
                }

                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "DefaultBank") != null)
                {
                    _viewModel.Glclient.GetRetailChainSetupByCodeAsync("DefaultBank", LoggedUserInfo.DatabasEname);
                    _viewModel.Glclient.GetRetailChainSetupByCodeCompleted += (hhfs, tv) =>
                    {
                        _viewModel.DefaultBank = new GenericTable
                        {
                            Iserial =
                            Convert.ToInt32(tv.Result.sSetupValue)
                        };
                    };                    
                }

            };
            SwitchFormMode(FormMode.Add);

            if (setting.Iserial == 2)
            {
                // ReSharper disable once PossibleNullReferenceException
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "CostCenter").Visibility =
                       Visibility.Visible;
            }
            if (setting.Iserial == 13)
            {
                // ReSharper disable once PossibleNullReferenceException
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "TblBank").Visibility =
                       Visibility.Visible;
                
            }
            if (setting.ChequeLockupFilterOnChequeType != 0 && setting.ChequeLockupFilterOnChequeType != null)
            {
                BtnAddDetails.Visibility = Visibility.Visible;
                // DetailGrid.IsReadOnly = true;
                DetailGridReadOnly();
            }
            else
            {
                BtnAddDetails.Visibility = Visibility.Collapsed;
                DetailGrid.IsReadOnly = false;
              
            }
            if (!setting.UseEntityDetail1)
            {
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "EntityDetail1").Visibility =
                       Visibility.Collapsed;
            }

            if (!setting.UseEntityDetail2)
            {
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "EntityDetail2").Visibility =
                    Visibility.Collapsed;
            }
            if (!setting.Payable)
            {
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "TblBankCheque").Visibility =
                    Visibility.Collapsed;
            }
            else
            {
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "ChequeNo").Visibility =
                  Visibility.Collapsed;
            }

            if (!setting.UseBankCollectionDate)
            {
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "BankCollectionDate").Visibility =
               Visibility.Collapsed;
            }
            try
            {
                ExcelBehavior.EnableForGrid(DetailGrid);
            } catch { }
        }

        public FormMode FormMode { get; set; }

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
                if (!_viewModel.SelectedMainRow.Approved)
                {
                    _viewModel.SelectedDetailRows.Clear();
                    foreach (var row in DetailGrid.SelectedItems)
                    {
                        _viewModel.SelectedDetailRows.Add((TblGlChequeTransactionDetailViewModel)row);
                    }
                    _viewModel.DeleteDetailRow();
                }
            }
            try { } catch { }
        }

        private void ResetMode()
        {
            FormMode = FormMode.Standby;
            SwitchFormMode(FormMode);
        }

        private void ClearScreen()
        {
            _viewModel.SelectedMainRow = null;
            _viewModel.AddNewMainRow(false);
            BtnAddNewCard.IsChecked = false;
        }

        private void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.Visibility = Visibility.Collapsed;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnCancel.IsEnabled = true;
                    break;

                case FormMode.Standby:
                    BtnAddNewCard.IsEnabled = true;
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = true;
                    BtnSave.Visibility = Visibility.Collapsed;
                    BtnShowSearch.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtnAddNewCard.IsEnabled = false;
                    BtnAddNewCard.Visibility = Visibility.Collapsed;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = false;
                    break;

                case FormMode.Update:
                    BtnSave.Visibility = Visibility.Visible;
                    break;

                case FormMode.Read:
                    BtnSearch.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new GlChequeTransactionChildWindow(_viewModel);
            childWindowSeach.Show();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.SelectedMainRow.Approved)
            {
                var r =
                    MessageBox
                        .Show(
                            "You are about to delete The Transaction permanently!!\nPlease note that this action cannot be undone!"
                            , "Delete", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK)
                {
                    _viewModel.DeleteMainRow();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnDeleteCard.Visibility = Visibility.Collapsed;
            BtnDeleteCard.IsEnabled = false;
            BtnShowSearch.IsChecked = false;
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            var para = new ObservableCollection<string>
            {
                _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture),
                LoggedUserInfo.Ip + LoggedUserInfo.Port,
                LoggedUserInfo.DatabasEname
            };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(_setting.Report, para);
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;

            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));
            MyPopup.HorizontalOffset = p.X;
            MyPopup.VerticalOffset = p.Y + 60;
            MyPopup.IsOpen = true;
        }

        private void BtnApprove_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow(true);
        }

        private void btnDuplicate_Click(object sender, RoutedEventArgs e)
        {
            var newrow = new TblGlChequeTransactionDetailViewModel();
            newrow.InjectFrom(_viewModel.SelectedDetailRow);
            newrow.ChequePerRow = new TblBankCheque();

            newrow.Iserial = 0;
            _viewModel.SelectedMainRow.DetailsList.Add(newrow);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetLockupFromPreTransaction();
            MyPopup.IsOpen = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }

        private void CostCenter_OnClick(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            var temp = btn.DataContext;

            _viewModel.SelectedDetailRow = temp as TblGlChequeTransactionDetailViewModel;
            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(_viewModel.SelectedDetailRow,
                new ValidationContext(_viewModel.SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                MessageBox.Show("Data Is Not Valid");
            }
            else
            {
                var child = new BankChequeCostCenterChildWindow(_viewModel);
                child.Show();
            }
        }

        private void BtnprintCheque_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Glclient.PrintCheckAsync((int)_viewModel.SelectedDetailRow.TblBankCheque, LoggedUserInfo.DatabasEname);
            _viewModel.Glclient.PrintCheckCompleted += (s, sv) =>
            {
                var printingPage = new BarcodePrintPreview(sv.Result, 4, (_viewModel.SelectedDetailRow.ChequePerRow.TblBank1.Code), true);
                var currentUi = Thread.CurrentThread.CurrentUICulture;
                printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                printingPage.Show();
            };

        }

        private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.calcTotal();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var stylelist = new List<TblGlChequeTransactionDetailViewModel>();
            var oFile = new OpenFileDialog { Filter = "Excel (*.xls)|*.xls" };

            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();

                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];

                var JournalAccountType = 0;
                var EntityCode = 0;
                var Payto = 0;
                var Amount = 0;
                var DueDate = 0;
                var Cheque = 0;
                var Description = 0;
                for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "journalaccounttype":
                            JournalAccountType = j;
                            break;

                        case "entitycode":
                            EntityCode = j;
                            break;

                        case "payto":
                            Payto = j;
                            break;

                        case "amount":
                            Amount = j;
                            break;

                        case "duedate":
                            DueDate = j;
                            break;

                        case "cheque":
                            Cheque = j;
                            break;

                        case "description":
                            Description = j;
                            break;

                    }
                };
                for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
                {

                    var journalAccountPerRow = _viewModel.TblJournalAccountTypeList.SingleOrDefault(x => x.Ename.ToLower() == sheet.Cells[i, JournalAccountType].StringValue.ToLower().Trim());
                    var newemp = new TblGlChequeTransactionDetailViewModel();
                    newemp.TblJournalAccountTypePerRow = journalAccountPerRow;
                    newemp.EntityDetail1TblJournalAccountType = journalAccountPerRow.Iserial;
                    newemp.EntityCode = sheet.Cells[i, EntityCode].StringValue.ToUpper().Trim();
                    newemp.PayTo = sheet.Cells[i, Payto].StringValue.ToUpper().Trim();
                    newemp.Amount = Convert.ToDouble(sheet.Cells[i, Amount].StringValue.ToUpper().Trim());
                    newemp.DueDate = sheet.Cells[i, DueDate].DateTimeValue;
                    newemp.ChequeNo = sheet.Cells[i, Cheque].StringValue.ToUpper().Trim();
                    newemp.Description = sheet.Cells[i, Description].StringValue.ToUpper().Trim();

                    stylelist.Add(newemp);
                }
                _viewModel.GenerateDetailListFromExcel(stylelist);

            }
        }

        private void DpTransDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
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
        }

        private void DetailGridReadOnly()
        {
            /*
                TblJournalAccountType1.Ename
                EntityDetail1
                EntityDetail2
                DueDate
                BankCollectionDate
                ChequeNo
                TblBankCheque
                TblBank
                PayTo
                BankName
                Description
                Amount
                CostCenter
                TblBankCheque
                DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "TblBank").IsReadOnly =
                     false;
             */

            foreach (var item in DetailGrid.Columns)
            {
                if (item.SortMemberPath == "TblBank")
                {
                    item.IsReadOnly = false;
                } else
                {
                    item.IsReadOnly = true;
                }
            }
        }
    }

}