using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class AccountViewModel : ViewModelBase
    {
        public AccountViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Account.ToString());
                Glclient = new GlServiceClient();
                var currencyClient = new GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) => { CurrencyList = sv.Result; };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var accountTypeClient = new GlServiceClient();
                accountTypeClient.GetGenericCompleted += (s, sv) => { AccountTypeList = sv.Result; };
                accountTypeClient.GetGenericAsync("TblAccountType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);
                MainRowList = new SortableCollectionView<TblAccountViewModel>();
                SelectedMainRow = new TblAccountViewModel();

                Glclient.GetTblAccountCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccountViewModel();
                        newrow.CurrencyPerRow = new GenericTable();
                        newrow.AccountTypePerRow = new GenericTable();
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        newrow.InjectFrom(row);

                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);

                        newrow.AccountTypePerRow.InjectFrom(row.TblAccountType1);

                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        newrow.EntityPerRow =
                         sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                  x.Iserial == row.EntityAccount);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                    if (Export)
                    {
                        Export = false;
                        AllowExport = true;
                        //ExportGrid.ExportExcel("Account");
                    }
                };

                Glclient.UpdateOrInsertTblAccountsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblAccountCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.GetTblAccountIntervalCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccountIntervalViewModel();

                        newrow.InjectFrom(row);

                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;

                    if (SelectedMainRow.DetailsList.Any() &&
                        (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }

                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblAccountIntervalsCompleted += (s, x) =>
                {
                    var savedRow = (TblAccountIntervalViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblAccountIntervalCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    Loading = false;
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                GetMaindata();
            }
        }



        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblAccountAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, false);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetTblAccountAsync(MainRowList.Count, int.MaxValue, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, false);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblAccountViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblAccount();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblAccountsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                                              LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblAccountViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblAccount();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblAccountsAsync(saveRow, save, MainRowList.IndexOf(oldRow),
                                              LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Glclient.DeleteTblAccountAsync((TblAccount)new TblAccount().InjectFrom(row),
                                MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblAccountIntervalAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteTblAccountIntervalAsync(
                                (TblAccountInterval)new TblAccountInterval().InjectFrom(row),
                                SelectedMainRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            var newrow = new TblAccountIntervalViewModel { TblAccount = SelectedMainRow.Iserial };

            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    if (SelectedDetailRow.TblAccount == 0)
                    {
                        SelectedDetailRow.TblAccount = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblAccountInterval();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertTblAccountIntervalsAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set
            {
                _currencyList = value;
                RaisePropertyChanged("CurrencyList");
            }
        }

        private ObservableCollection<GenericTable> _accountTypeList;

        public ObservableCollection<GenericTable> AccountTypeList
        {
            get { return _accountTypeList; }
            set
            {
                _accountTypeList = value;
                RaisePropertyChanged("AccountTypeList");
            }
        }

        private SortableCollectionView<TblAccountViewModel> _mainRowList;

        public SortableCollectionView<TblAccountViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblAccountViewModel> _selectedMainRows;

        public ObservableCollection<TblAccountViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAccountViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private TblAccountViewModel _selectedMainRow;

        public TblAccountViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblAccountIntervalViewModel _selectedDetailRow;

        public TblAccountIntervalViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblAccountIntervalViewModel> _selectedDetailRows;

        public ObservableCollection<TblAccountIntervalViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (SelectedDetailRows = new ObservableCollection<TblAccountIntervalViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblAccountViewModel : GenericViewModel
    {
        private ObservableCollection<TblAccountIntervalViewModel> _detailLst;

        public ObservableCollection<TblAccountIntervalViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new ObservableCollection<TblAccountIntervalViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private Entity _entityPerRow;
        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                if ((ReferenceEquals(_entityPerRow, value) != true))
                {
                    _entityPerRow = value;
                    RaisePropertyChanged("EntityPerRow");

                    if (EntityPerRow != null)
                    {
                        EntityAccount = EntityPerRow.Iserial;
                    }
                }
            }
        }

        private int _entityAccount;

        public int EntityAccount
        {
            get { return _entityAccount; }
            set { _entityAccount = value; RaisePropertyChanged("EntityAccount"); }
        }


        private bool _expensesFlag;

        public bool ExpensesFlag
        {
            get { return _expensesFlag; }
            set { _expensesFlag = value; RaisePropertyChanged("ExpensesFlag"); }
        }

        private int _accountCursorField;
        private int _accountMustDrOrCrField;
        private int _balanceControlField;
        private bool _boldField;
        private int _closedBackGroupPostField;
        private int _colLevelField;
        private int _colNoField;
        private bool _invertSignField;
        private bool _italicsField;
        private bool _lineAboveField;
        private bool _lineBelowField;
        private bool _lockedField;
        private int? _parentCodeField;
        private bool _profitAndLossField;
        private GenericTable _tblAccountType;
        private int _tblAccountTypeField;
        private int _tblCostCenterField;
        private GenericTable _tblcurrency;
        private int _tblCurrencyField;
        private bool _underlineField;
        private int? _tblJournalAccountTypeField;
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountTypeField; }
            set
            {
                if ((_tblJournalAccountTypeField.Equals(value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }
        private GenericTable _journalAccountTypePerRow;
        public GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    if (JournalAccountTypePerRow != null) TblJournalAccountType = JournalAccountTypePerRow.Iserial;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                }
            }
        }

        private string _accountLevel;

        public string AccountLevel
        {
            get { return _accountLevel; }
            set { _accountLevel = value; RaisePropertyChanged("AccountLevel"); }
        }

        public GenericTable CurrencyPerRow
        {
            get { return _tblcurrency; }
            set
            {
                if ((ReferenceEquals(_tblcurrency, value) != true))
                {
                    _tblcurrency = value;
                    RaisePropertyChanged("CurrencyPerRow");
                    TblCurrency = CurrencyPerRow.Iserial;
                }
            }
        }

        public GenericTable AccountTypePerRow
        {
            get { return _tblAccountType; }
            set
            {
                if ((ReferenceEquals(_tblAccountType, value) != true))
                {
                    _tblAccountType = value;
                    RaisePropertyChanged("AccountTypePerRow");
                    TblAccountType = AccountTypePerRow.Iserial;
                }
            }
        }

        public int AccountCursor
        {
            get { return _accountCursorField; }
            set
            {
                if ((_accountCursorField.Equals(value) != true))
                {
                    _accountCursorField = value;
                    RaisePropertyChanged("AccountCursor");
                }
            }
        }

        public int AccountMustDrOrCr
        {
            get { return _accountMustDrOrCrField; }
            set
            {
                if ((_accountMustDrOrCrField.Equals(value) != true))
                {
                    _accountMustDrOrCrField = value;
                    RaisePropertyChanged("AccountMustDrOrCr");
                }
            }
        }

        public int BalanceControl
        {
            get { return _balanceControlField; }
            set
            {
                if ((_balanceControlField.Equals(value) != true))
                {
                    _balanceControlField = value;
                    RaisePropertyChanged("BalanceControl");
                }
            }
        }

        public bool Bold
        {
            get { return _boldField; }
            set
            {
                if ((_boldField.Equals(value) != true))
                {
                    _boldField = value;
                    RaisePropertyChanged("Bold");
                }
            }
        }

        public int ClosedBackGroupPost
        {
            get { return _closedBackGroupPostField; }
            set
            {
                if ((_closedBackGroupPostField.Equals(value) != true))
                {
                    _closedBackGroupPostField = value;
                    RaisePropertyChanged("ClosedBackGroupPost");
                }
            }
        }

        public int ColLevel
        {
            get { return _colLevelField; }
            set
            {
                if ((_colLevelField.Equals(value) != true))
                {
                    _colLevelField = value;
                    RaisePropertyChanged("ColLevel");
                }
            }
        }

        public int ColNo
        {
            get { return _colNoField; }
            set
            {
                if ((_colNoField.Equals(value) != true))
                {
                    _colNoField = value;
                    RaisePropertyChanged("ColNo");
                }
            }
        }

        public bool InvertSign
        {
            get { return _invertSignField; }
            set
            {
                if ((_invertSignField.Equals(value) != true))
                {
                    _invertSignField = value;
                    RaisePropertyChanged("InvertSign");
                }
            }
        }

        public bool Italics
        {
            get { return _italicsField; }
            set
            {
                if ((_italicsField.Equals(value) != true))
                {
                    _italicsField = value;
                    RaisePropertyChanged("Italics");
                }
            }
        }

        public bool LineAbove
        {
            get { return _lineAboveField; }
            set
            {
                if ((_lineAboveField.Equals(value) != true))
                {
                    _lineAboveField = value;
                    RaisePropertyChanged("LineAbove");
                }
            }
        }

        public bool LineBelow
        {
            get { return _lineBelowField; }
            set
            {
                if ((_lineBelowField.Equals(value) != true))
                {
                    _lineBelowField = value;
                    RaisePropertyChanged("LineBelow");
                }
            }
        }

        public bool Locked
        {
            get { return _lockedField; }
            set
            {
                if ((_lockedField.Equals(value) != true))
                {
                    _lockedField = value;
                    RaisePropertyChanged("Locked");
                }
            }
        }

        public int? ParentCode
        {
            get { return _parentCodeField; }
            set
            {
                if ((_parentCodeField.Equals(value) != true))
                {
                    _parentCodeField = value;
                    RaisePropertyChanged("ParentCode");
                }
            }
        }

        public bool ProfitAndLoss
        {
            get { return _profitAndLossField; }
            set
            {
                if ((_profitAndLossField.Equals(value) != true))
                {
                    _profitAndLossField = value;
                    RaisePropertyChanged("ProfitAndLoss");
                }
            }
        }

        public int TblAccountType
        {
            get { return _tblAccountTypeField; }
            set
            {
                if ((_tblAccountTypeField.Equals(value) != true))
                {
                    _tblAccountTypeField = value;
                    RaisePropertyChanged("TblAccountType");
                }
            }
        }

        public int TblCostCenter
        {
            get { return _tblCostCenterField; }
            set
            {
                if ((_tblCostCenterField.Equals(value) != true))
                {
                    _tblCostCenterField = value;
                    RaisePropertyChanged("TblCostCenter");
                }
            }
        }

        public int TblCurrency
        {
            get { return _tblCurrencyField; }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public bool Underline
        {
            get { return _underlineField; }
            set
            {
                if ((_underlineField.Equals(value) != true))
                {
                    _underlineField = value;
                    RaisePropertyChanged("Underline");
                }
            }
        }
    }

    public class TblAccountIntervalViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private string _fromRangeField;

        private bool _invertSignField;

        private int _iserialField;

        private int _tblAccountField;

        private string _toRangeField;

        [Required]
        public string FromRange
        {
            get
            {
                return _fromRangeField;
            }
            set
            {
                if ((ReferenceEquals(_fromRangeField, value) != true))
                {
                    _fromRangeField = value;
                    RaisePropertyChanged("FromRange");
                }
            }
        }

        public bool InvertSign
        {
            get
            {
                return _invertSignField;
            }
            set
            {
                if ((_invertSignField.Equals(value) != true))
                {
                    _invertSignField = value;
                    RaisePropertyChanged("InvertSign");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int TblAccount
        {
            get
            {
                return _tblAccountField;
            }
            set
            {
                if ((_tblAccountField.Equals(value) != true))
                {
                    _tblAccountField = value;
                    RaisePropertyChanged("TblAccount");
                }
            }
        }

        [Required]
        public string ToRange
        {
            get
            {
                return _toRangeField;
            }
            set
            {
                if ((ReferenceEquals(_toRangeField, value) != true))
                {
                    _toRangeField = value;
                    RaisePropertyChanged("ToRange");
                }
            }
        }
    }

    #endregion ViewModels
}