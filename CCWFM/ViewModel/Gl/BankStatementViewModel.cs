using CCWFM.BankStatService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Models.Gl;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.Gl;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;
using GalaSoft.MvvmLight.Command;
using Lite.ExcelLibrary.SpreadSheet;
using Omu.ValueInjecter.Silverlight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.ViewModel.Gl
{
    public class BankStatHeader : TblBankStatHeader
    {
        public BankStatHeader()
        {
            DocDate = DateTime.Now;
            ReconcileDate = DateTime.Now;
            TblCurrency = -1;
            CreatedBy = LoggedUserInfo.Iserial;
            CreationDate = DateTime.Now;
        }
        [Range(1, long.MaxValue)]
        public new decimal TotalBalance
        {
            get { return TblBankStatDetails.Sum(d => d.Amount); }
        }
        ObservableCollection<BankStatDetail> bankStatDetails;
        public new ObservableCollection<BankStatDetail> TblBankStatDetails
        {
            get { return bankStatDetails ?? (bankStatDetails = new ObservableCollection<BankStatDetail>()); }
            set { bankStatDetails = value; RaisePropertyChanged(nameof(TblBankStatDetails)); }
        }
    }
    public class BankStatDetail : TblBankStatDetail
    {
        public BankStatDetail()
        {
            DocDate = DateTime.Now;
        }
        [Range(1, long.MaxValue)]
        public new decimal Amount
        {
            get { return base.Amount; }
            set { base.Amount = value; RaisePropertyChanged(nameof(BankStatHeader.TotalBalance)); }
        }
    }
    public class BankStatementViewModel : ViewModelStructuredBase
    {
        BankStatServiceClient BankStatClient = Helpers.Services.Instance.GetBankStatServiceClient();
        public BankStatementViewModel() : base(PermissionItemName.BankStatement)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                ApproveBankStat = new RelayCommand(() => {
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    SelectedMainRow.Approved = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                        if (NewCommand.CanExecute(null))
                            NewCommand.Execute(null);
                }, () => CheckCanApprove());
                DeleteBankStatDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblBankStatDetails.Remove(SelectedDetailRow);
                            if (SelectedMainRow.TblBankStatDetails.Count == 0)
                            {
                                AddNewDetailRow(false);
                            }
                        }
                        else
                            DeleteDetailRow();
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                }, (o) => {
                    return SelectedMainRow != null && !SelectedMainRow.Approved;
                });
                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    if (SelectedMainRow.TblBankStatDetails.Count < PageSize)
                    {
                        return;
                    }
                    if (SelectedMainRow.TblBankStatDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    {
                        GetDetailData();
                    }
                });
          
                NewDetail = new RelayCommand<object>((o) =>
                {
                    var e = o as SelectionChangedEventArgs;
                    if (((KeyEventArgs)(o)).Key == Key.Down)
                    {
                        AddNewDetailRow(false);
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                });
                this.PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "BankStatApprove") != null)
                    {
                        CanApprove = true;
                    }
                };
                this.GetCustomePermissions(PermissionItemName.BankStatement.ToString());

                MainRowList = new ObservableCollection<BankStatHeader>();
                AddNewMainRow(false);
                
                BankStatClient.GetBankStatHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new BankStatHeader();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (SearchWindow != null)
                    {
                        SearchWindow.FullCount = sv.fullCount;
                        SearchWindow.Loading = false;
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                BankStatClient.GetBankStatDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new BankStatDetail();
                        newrow.InjectFrom(row);
                        //newrow.TblBankTransactionType1 = BankTransactionTypeList.FirstOrDefault(btt => btt.Iserial == newrow.TblBankTransactionType);
                        SelectedMainRow.TblBankStatDetails.Add(newrow);
                    }
                    if (!SelectedMainRow.TblBankStatDetails.Any())
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                BankStatClient.UpdateOrInsertBankStatHeaderCompleted += (s, x) =>
                {
                    BankStatHeader savedRow = null;
                    if (x.outindex >= 0)
                        savedRow = MainRowList.ElementAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == savedRow.TblBank);
                        savedRow.TblCurrency1 = CurrencyList.FirstOrDefault(c => c.Iserial == savedRow.TblCurrency);
                        savedRow.TblBankStatDetails.Clear();
                        foreach (var item in x.Result.TblBankStatDetails)
                        {
                            var detailTemp = new BankStatDetail();
                            detailTemp.InjectFrom(item);
                            savedRow.TblBankStatDetails.Add(detailTemp);
                        }
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    DeleteCommand.RaiseCanExecuteChanged();
                    ApproveBankStat.RaiseCanExecuteChanged();
                    DeleteBankStatDetail.RaiseCanExecuteChanged();
                    IsNewChanged();
                };
                BankStatClient.UpdateOrInsertBankStatDetailCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.TblBankStatDetails.ElementAt(x.outindex);
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblBankTransactionType1 = BankTransactionTypeList.FirstOrDefault(bt => bt.Iserial == savedRow.TblBankTransactionType);
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                BankStatClient.DeleteBankStatHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                BankStatClient.DeleteBankStatDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblBankStatDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblBankStatDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };

                BankStatClient.IsBankStatHeaderHasMatchedRowsCompleted += (s, e) =>
                {
                    if (SelectedMainRow.Iserial == e.Iserial)
                        hasMatched = e.Result;
                    else hasMatched = false;
                    RaisePropertyChanged(nameof(IsReadOnly));
                };

                BankStatClient.GetLookUpBankTransactionTypeCompleted += (s, e) =>
                {
                    BankTransactionTypeList = e.Result;
                };
                BankStatClient.GetLookUpBankCompleted += (s, e) =>
                {
                    BankList = e.Result;
                };
                BankStatClient.GetLookUpCurrencyCompleted += (s, e) =>
                {
                    CurrencyList = e.Result;
                };

                BankStatClient.InsertImportedItemsCompleted += (s, e) =>
                {
                        for (int i = 0; i < RemainningImportList.Count; i = i + step)
                        {
                            bool approve = (i + step >= RemainningImportList.Count);//هل دى اخر لفة
                            var temp = new ObservableCollection<ImportedBankStatement>(RemainningImportList.Skip(i).Take(step));
                            BankStatClient.InsertRemainingImportedItemsAsync(e.Result, temp, LoggedUserInfo.DatabasEname);// First Time
                            requestes++;
                        }
                        ImportHeaderIserial = e.Result;
                        Loading = false;
                };
                BankStatClient.InsertRemainingImportedItemsCompleted += (s, e) =>
                {
                    requestes--;// على اساس ان الريكويست اسرع من الريسبونس

                    foreach (var item in e.Result)
                    {
                        error += item + "\r\n";
                    }
                    if (e.Error != null)
                    {
                        requestes = -1;
                        throw e.Error;
                    }
                    else if (requestes == 0)// كده ده اخر واحد
                    {
                        if (string.IsNullOrWhiteSpace(error))
                        {
                            MessageBox.Show("Import Completed Succesfully");
                            //هجيب الريكورد الى اتحفظ ده اعرضه بقى
                            BankStatClient.GetBankStatHeaderByIserialAsync(ImportHeaderIserial, LoggedUserInfo.DatabasEname);
                            ImportHeaderIserial = -1;
                            return;
                        }
                        BankStatClient.DeleteBankStatByIserialAsync(ImportHeaderIserial, LoggedUserInfo.DatabasEname);
                        ImportHeaderIserial = -1;
                        if (MessageBox.Show("Import Completed, Do you want to view logs?", "Info", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            new LogView(error).Show();
                            error = "";
                        }
                    }
                };
                BankStatClient.GetBankStatHeaderByIserialCompleted += (s, e) =>
                {
                    SelectedMainRow.InjectFrom(e.Result);
                    SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                    SelectedMainRow.TblCurrency1 = CurrencyList.FirstOrDefault(c => c.Iserial == SelectedMainRow.TblCurrency);
                    if (SelectedMainRow.TblBankStatDetails != null)
                        SelectedMainRow.TblBankStatDetails = new ObservableCollection<BankStatDetail>();
                    RaisePropertyChanged(nameof(SelectedMainRow));
                    DeleteCommand.RaiseCanExecuteChanged();
                    ApproveBankStat.RaiseCanExecuteChanged();
                    DeleteBankStatDetail.RaiseCanExecuteChanged();
                    IsNewChanged();
                    GetDetailData();
                };
                BankStatClient.InsertExcelFileDateCompleted += (s, e) => {
                    //if (e.Error != null) MessageBox.Show(e.Error.Message);
                };
                ImportFromExcelCommand = new RelayCommand(() => {
                    if (!ValidHeaderData()) { return; }
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Excel Files (*.xls)|*.xls";
                    if (ofd.ShowDialog() == true)
                    {
                        var importedList = new ObservableCollection<ImportedBankStatement>();
                        var fs = ofd.File.OpenRead();

                        var book = Workbook.Open(fs);
                        var sheet = book.Worksheets[0];

                        var dataList = new ObservableCollection<Models.Excel.CellModel>();
                        for (int i = sheet.Cells.FirstColIndex; i < sheet.Cells.LastColIndex + 1; i++)
                        {
                            for (int j = sheet.Cells.FirstRowIndex; j < sheet.Cells.LastRowIndex + 1; j++)
                            {
                                var cellModel = new Models.Excel.CellModel()
                                {
                                    Column = i,
                                    Row = j,
                                    Value = sheet.Cells[j, i].StringValue.ToUpper(),
                                };
                                dataList.Add(cellModel);
                                if (i == 1)
                                {
                                    var t = sheet.Cells[j, i].StringValue.ToUpper().Split('-');
                                    if (t.Length == 3)
                                    {
                                        cellModel.Day = t[0];
                                        cellModel.Mounth = t[1];
                                        cellModel.Year = t[2];
                                    }
                                }
                            }
                        }
                        TblBankStatHeader headerRow = new TblBankStatHeader();
                        headerRow.InjectFrom(SelectedMainRow);
                        BankStatClient.InsertExcelFileDateAsync(headerRow, dataList, LoggedUserInfo.DatabasEname);// new ObservableCollection<Models.Excel.CellModel>(dataList.Take(300)));
                        //int docDateIndex = 0, transactionTypeIndex = 0, descriptionIndex = 0, amountIndex = 0, chequeNoIndex = 0, depositNoIndex = 0;
                        //FillList(importedList, sheet, ref docDateIndex, ref transactionTypeIndex, ref descriptionIndex, ref amountIndex, ref chequeNoIndex, ref depositNoIndex);
                        //InsertImportedDetail(importedList);
                    }
                });
                MatchCommand = new RelayCommand(() => {
                    if (SelectedMainRow.Iserial <= 0)
                    {
                        MessageBox.Show("Bank Statement must save first");
                        return;
                    }
                    var matchingView = new BankStatementMatchView();
                    var matchingViewModel = new BankStatementMatchViewModel();
                    matchingViewModel.HeaderRow = SelectedMainRow;
                    matchingView.DataContext = matchingViewModel;
                    matchingView.Show();
                });

                GetComboData();
                GetMaindata();
            }
        }

        private static void FillList(ObservableCollection<ImportedBankStatement> importedList, Worksheet sheet, ref int docDateIndex, ref int transactionTypeIndex, ref int descriptionIndex, ref int amountIndex, ref int chequeNoIndex, ref int depositNoIndex)
        {
            for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
            {
                switch (sheet.Cells[0, j].StringValue.ToLower())
                {
                    case "docdate":
                        docDateIndex = j;
                        break;
                    case "transactiontype":
                        transactionTypeIndex = j;
                        break;
                    case "description":
                        descriptionIndex = j;
                        break;
                    case "amount":
                        amountIndex = j;
                        break;
                    case "chequeno":
                        chequeNoIndex = j;
                        break;
                    case "depositno":
                        depositNoIndex = j;
                        break;
                }
            }
            for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
            {
                var newemp = new ImportedBankStatement();
                if (sheet.Cells[i, docDateIndex].Value != null)
                {
                    var docDate = sheet.Cells[i, docDateIndex].Value.ToString().ToUpper().Trim();
                    newemp.DocDate = Convert.ToDateTime(docDate);
                }
                if (sheet.Cells[i, transactionTypeIndex].Value != null)
                {
                    var transactionType = sheet.Cells[i, transactionTypeIndex].Value.ToString().ToUpper().Trim();
                    newemp.TransactionType = transactionType;
                }
                if (sheet.Cells[i, descriptionIndex].Value != null)
                {
                    var description = sheet.Cells[i, descriptionIndex].Value.ToString().ToUpper().Trim();
                    newemp.Description = description;
                }
                if (sheet.Cells[i, amountIndex].Value != null)
                {
                    var amount = sheet.Cells[i, amountIndex].Value.ToString().ToUpper().Trim();
                    newemp.Amount = Convert.ToDecimal(amount);
                }
                if (sheet.Cells[i, chequeNoIndex].Value != null)
                {
                    var chequeNo = sheet.Cells[i, chequeNoIndex].Value.ToString().ToUpper().Trim();
                    newemp.ChequeNo = Convert.ToInt64(chequeNo);
                }
                if (sheet.Cells[i, depositNoIndex].Value != null)
                {
                    var depositNo = sheet.Cells[i, depositNoIndex].Value.ToString().ToUpper().Trim();
                    newemp.DepositNo = depositNo;
                }
                importedList.Add(newemp);
            }
        }

        private bool hasMatched;
        int step = 300, requestes = 0, ImportHeaderIserial = -1;
        string error = "";
        ObservableCollection<ImportedBankStatement> RemainningImportList;

        #region Methods

        private void InsertImportedDetail(ObservableCollection<ImportedBankStatement> importedList)
        {
            TblBankStatHeader headerRow = new TblBankStatHeader();
            headerRow.InjectFrom(SelectedMainRow);
         
            if (ValidData())
            {
                var temp = new ObservableCollection<ImportedBankStatement>(importedList.Skip(0).Take(0));
                BankStatClient.InsertImportedItemsAsync(headerRow, temp, LoggedUserInfo.DatabasEname);// BankStat Header

                RemainningImportList = new ObservableCollection<ImportedBankStatement>(
                    importedList.Skip(0));

            }

        }
        private void ValidateDetailRow(BankStatDetail bankStatDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblBankStatDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblBankStatDetails.Add(bankStatDetail);
            }
            else
            {
                SelectedMainRow.TblBankStatDetails.Add(bankStatDetail);
            }
            SelectedDetailRow = bankStatDetail;
            RaisePropertyChanged(nameof(Total));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }       
        private bool CheckCanApprove()
        {
            return CanApprove && !SelectedMainRow.Approved && !SelectedMainRow.MatchApproved;
        }
        public bool ValidHeaderData()
        {
            if (SelectedMainRow.TblBank <= 0)
            {
                MessageBox.Show(strings.ReqBankAccountNo);
                return false;
            }
            if (SelectedMainRow.TblCurrency < 0)
            {
                MessageBox.Show(strings.ReqCurrency);
                return false;
            }
            if (SelectedMainRow.TblCurrency != SelectedMainRow.TblBank1.TblCurrency)
            {
                MessageBox.Show("Statement currency and account currency must be the same.");
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.Approved && SelectedMainRow.TblBankStatDetails.Any(td => 0 == td.Amount))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            if ((SelectedMainRow.Approved && SelectedMainRow.Iserial <= 0 &&
                SelectedMainRow.TblBankStatDetails.Any(td => td.Amount <= 0)))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
    
        #endregion

        #region Operations

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            BankStatClient.GetBankStatHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    BankStatClient.DeleteBankStatHeaderAsync((TblBankStatHeader)new
                        TblBankStatHeader().InjectFrom(SelectedMainRow),
                        LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                SelectedMainRow = new BankStatHeader()
                {
                    TblBankStatDetails = new ObservableCollection<BankStatDetail>()
                };
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                AddNewDetailRow(false);
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
              
                if (isvalid)
                {
                    var saveRow = new TblBankStatHeader()
                    {
                        DocDate = DateTime.Now,
                        CreationDate = DateTime.Now,
                    };
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblBankStatDetails = new ObservableCollection<TblBankStatDetail>();
                    foreach (var item in SelectedMainRow.TblBankStatDetails)
                    {
                        var detailTemp = new TblBankStatDetail();
                        detailTemp.InjectFrom(item);
                        saveRow.TblBankStatDetails.Add(detailTemp);
                    }

                    var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    if (mainRowIndex < 0)
                    {
                        MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    }
                    BankStatClient.UpdateOrInsertBankStatHeaderAsync(saveRow, mainRowIndex, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
                BankStatClient.GetBankStatDetailAsync(SelectedMainRow.TblBankStatDetails.Count, PageSize, SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    BankStatClient.DeleteBankStatDetailAsync((TblBankStatDetail)new TblBankStatDetail().InjectFrom(SelectedDetailRow),
                                                LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblBankStatDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblBankStatDetails.Count - 1))
            {
                if (checkLastRow && SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(
                        SelectedDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.TblBankStatDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new BankStatDetail
                {
                    TblBankStatHeader = SelectedMainRow.Iserial
                });
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new TblBankStatDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    BankStatClient.UpdateOrInsertBankStatDetailAsync(rowToSave, SelectedMainRow.TblBankStatDetails.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            BankStatClient.GetLookUpBankTransactionTypeAsync(LoggedUserInfo.DatabasEname);
            BankStatClient.GetLookUpBankAsync(LoggedUserInfo.DatabasEname);
            BankStatClient.GetLookUpCurrencyAsync(LoggedUserInfo.DatabasEname);
        }

        #endregion

        #region Properties

        private ObservableCollection<BankStatHeader> _mainRowList;
        public ObservableCollection<BankStatHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }
        private ObservableCollection<BankStatHeader> _selectedMainRows;
        public ObservableCollection<BankStatHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<BankStatHeader>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private BankStatHeader _selectedMainRow;
        public BankStatHeader SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new BankStatHeader()
                {
                    TblBankStatDetails = new ObservableCollection<BankStatDetail>()
                });
            }
            set
            {
                if (value != null)
                    BankStatClient.IsBankStatHeaderHasMatchedRowsAsync(value.Iserial, LoggedUserInfo.DatabasEname);
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                ApproveBankStat.RaiseCanExecuteChanged();
                DeleteBankStatDetail.RaiseCanExecuteChanged();
                IsNewChanged();
                GetDetailData();
            }
        }
        private BankStatDetail _selectedDetailRow;
        public BankStatDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new BankStatDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }

        #region Combo Data

        ObservableCollection<TblBankTransactionType> _bankTransactionTypeList = new ObservableCollection<TblBankTransactionType>();
        public ObservableCollection<TblBankTransactionType> BankTransactionTypeList
        {
            get { return _bankTransactionTypeList; }
            set { _bankTransactionTypeList = value; RaisePropertyChanged(nameof(BankTransactionTypeList)); }
        }
        ObservableCollection<TblCurrencyTest> _currencyList = new ObservableCollection<TblCurrencyTest>();
        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged(nameof(CurrencyList)); }
        }

        ObservableCollection<TblBank> _bankList = new ObservableCollection<TblBank>();
        public ObservableCollection<TblBank> BankList
        {
            get { return _bankList ?? (_bankList = new ObservableCollection<TblBank>()); }
            set { _bankList = value; RaisePropertyChanged(nameof(BankList)); }
        }
        #endregion

        public virtual bool IsReadOnly
        {
            get
            {
                return (SelectedMainRow != null && SelectedMainRow.Iserial > 0 &&
                  SelectedMainRow.Approved) || hasMatched;
            }
        }
        public virtual bool IsHeaderHasDetails
        {
            get { return SelectedMainRow.TblBankStatDetails.Any(d => d.TblBankTransactionType > 0) || IsReadOnly; }
        }
        private bool canApprove;
        public bool CanApprove
        {
            get { return canApprove; }
            set { canApprove = value; RaisePropertyChanged(nameof(CanApprove)); ApproveBankStat.RaiseCanExecuteChanged(); }
        }
        public override bool IsNew
        {
            get { return SelectedMainRow.Iserial <= 0; }//base.IsNew && 
            set { base.IsNew = value; }
        }
        public decimal Total
        {
            get { return SelectedMainRow.TblBankStatDetails.Sum(td => td.Amount); }
        }
     
        #endregion

        #region Commands
        
        RelayCommand approveBankStat;
        public RelayCommand ApproveBankStat
        {
            get { return approveBankStat; }
            set { approveBankStat = value; RaisePropertyChanged(nameof(ApproveBankStat)); }
        }

        RelayCommand<object> deleteBankStatDetail;
        public RelayCommand<object> DeleteBankStatDetail
        {
            get { return deleteBankStatDetail; }
            set { deleteBankStatDetail = value; RaisePropertyChanged(nameof(DeleteBankStatDetail)); }
        }
        
        RelayCommand<object> detailSelectionChanged;
        public RelayCommand<object> NewDetail
        {
            get { return detailSelectionChanged; }
            set { detailSelectionChanged = value; RaisePropertyChanged(nameof(NewDetail)); }
        }
        
        RelayCommand<object> loadingDetailRows;
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }

        RelayCommand importFromExcelCommand;
        public RelayCommand ImportFromExcelCommand
        {
            get { return importFromExcelCommand; }
            set { importFromExcelCommand = value; RaisePropertyChanged(nameof(ImportFromExcelCommand)); }
        }

        RelayCommand matchCommand;
        public RelayCommand MatchCommand
        {
            get { return matchCommand; }
            set { matchCommand = value; RaisePropertyChanged(nameof(MatchCommand)); }
        }
     
        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
            RaisePropertyChanged(nameof(IsReadOnly));
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData() && ValidDetailData();
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<BankStatHeader> vm =
                new GenericSearchViewModel<BankStatHeader>() { Title = "Bank Statement Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) => {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header = strings.Currency,
                        PropertyPath= string.Format("{0}.{1}", nameof(BankStatHeader.TblCurrency1),nameof(TblCurrencyTest.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(BankStatHeader.TblCurrency1),nameof(TblCurrencyTest.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.BankAccount,
                        PropertyPath= string.Format("{0}.{1}", nameof(BankStatHeader.TblBank1),nameof(TblBank.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(BankStatHeader.TblBank1),nameof(TblBank.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Date,
                        PropertyPath=nameof(BankStatHeader.DocDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Approved,
                        PropertyPath=nameof(BankStatHeader.Approved),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ApproveDate,
                        PropertyPath=nameof(BankStatHeader.ApproveDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                };
        }
        public override void DeleteRecord()
        {
            this.DeleteMainRow();
            AddNewMainRow(false);
            base.DeleteRecord();
        }
        public override bool CanDeleteRecord()
        {
            return SelectedMainRow.Iserial > 0 &&
            SelectedMainRow.Approved == false;
        }
        public override void Cancel()
        {
            MainRowList.Clear();
            SelectedMainRows.Clear();
            AddNewMainRow(false);
            RaisePropertyChanged(nameof(IsReadOnly));
            base.Cancel();
        }
        public override void Print()
        {
            base.Print();
            var rVM = new GenericReportViewModel();
            var para = new ObservableCollection<string>() { SelectedMainRow.Iserial.ToString() };
            para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
            para.Add(LoggedUserInfo.DatabasEname);
            //rVM.GenerateReport("BankStatDocument", para);
        }

        #endregion
    }
}