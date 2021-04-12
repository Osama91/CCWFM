using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using GenericTable = CCWFM.GlService.GenericTable;
using TblCurrencyTest = CCWFM.GlService.TblCurrencyTest;

namespace CCWFM.ViewModel.Gl
{
    public class DepreciationViewModel : ViewModelBase
    {
        public DepreciationViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions("Depreciation");
                GetCustomePermissions("Depreciation");
                Glclient = new GlServiceClient();
                Glclient.GetTblRetailCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Glclient.GetTblRetailCurrencyAsync(0, int.MaxValue, "It.Iserial", null, null,
                 LoggedUserInfo.DatabasEname);
                var depreciationMethodClient = new GlServiceClient();
                depreciationMethodClient.GetGenericCompleted += (s, sv) =>
                {
                    DepreciationMethodList = sv.Result;
                };
                depreciationMethodClient.GetGenericAsync("TblDepreciationMethod", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblDepreciationTransactionHeaderViewModel>();
                SelectedMainRow = new TblDepreciationTransactionHeaderViewModel();
                Glclient.GetTblDepreciationTransactionHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblDepreciationTransactionHeaderViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                };

                Glclient.GetTblDepreciationTransactionDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        SelectedMainRow.DetailList.Add(row);
                    }

                    Loading = false;
                };

                //Glclient.GetLockupFromPreTransactionCompleted += (s, sv) =>
                //{
                //    foreach (var row in sv.Result)
                //    {
                //        if (SelectedMainRow.DetailsList.All(x => x.TblDepreciationTransactionDetail1 != row.Iserial))
                //        {
                //            var newrow = new TblDepreciationTransactionDetailViewModel();
                //            newrow.InjectFrom(row);
                //            newrow.ChequePerRow = new TblBankCheque();
                //            newrow.Iserial = 0;
                //            newrow.TblDepreciationTransactionHeader = 0;
                //            newrow.TblDepreciationTransactionDetail1 = row.Iserial;
                //            if (row.TblBankCheque1 != null) newrow.ChequePerRow.InjectFrom(row.TblBankCheque1);
                //            newrow.TblJournalAccountTypePerRow = new GenericTable();
                //            if (row.TblJournalAccountType != null)
                //                newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                //            newrow.TblJournalAccountType1PerRow = new GenericTable();
                //            if (row.TblJournalAccountType1 != null)
                //                newrow.TblJournalAccountType1PerRow.InjectFrom(row.TblJournalAccountType1);
                //            newrow.EntityDetail1TblJournalAccountType = row.EntityDetail1TblJournalAccountType;
                //            newrow.EntityDetail2TblJournalAccountType = row.EntityDetail2TblJournalAccountType;
                //            newrow.EntityPerRow =
                //        sv.entityList.FirstOrDefault(
                //             x => x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType
                //             && x.Iserial == row.EntityDetail1);

                //            newrow.OffsetEntityPerRow =
                //                 sv.entityList.FirstOrDefault(
                //                    x => x.TblJournalAccountType == row.EntityDetail2TblJournalAccountType
                //                    && x.Iserial == row.EntityDetail2);
                //            newrow.Saved = true;

                //            SelectedMainRow.DetailsList.Add(newrow);
                //        }
                //    }

                //    Loading = false;

                //    SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                //};

                Glclient.UpdateOrInsertTblDepreciationTransactionHeaderCompleted += (s, ev) =>
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
                Glclient.DeleteTblDepreciationTransactionHeaderCompleted += (s, ev) =>
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
                Glclient.GetTblLedgerDetailForDepreciationCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerMainDetailViewModel { CurrencyPerRow = new TblCurrencyTest() };
                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        if (row.TblBankTransactionType1 != null)
                        {
                            newrow.BankTransactionTypePerRow = new GenericTable();
                            newrow.BankTransactionTypePerRow.InjectFrom(row.TblBankTransactionType1);
                        }
                        newrow.MethodOfPaymentPerRow = row.TblMethodOfPayment1;
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        newrow.OffsetAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        if (row.TblJournalAccountType2 != null)
                            newrow.OffsetAccountTypePerRow.InjectFrom(row.TblJournalAccountType2);
                        newrow.EntityPerRow =
                            sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                     x.Iserial == row.EntityAccount);
                        newrow.OffsetEntityPerRow =
                           sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.OffsetAccountType && x.Iserial == row.OffsetEntityAccount);
                        newrow.InjectFrom(row);
                        if (newrow.DrOrCr == true)
                        {
                            if (row.Amount != null) newrow.DrAmount = row.Amount;
                        }
                        else
                        {
                            if (row.Amount != null) newrow.CrAmount = row.Amount;
                        }
                        if (row.TblBankCheque1 != null) newrow.ChequePerRow = row.TblBankCheque1;
                        newrow.TransactionExists = sv.TransactionExist.FirstOrDefault(x => x.Key == newrow.Iserial).Value;

                        if (row.TblAccount != null)
                        {
                            newrow.AccountPerRow = new TblAccount
                            {
                                Code = row.TblAccount.Code,
                                Iserial = row.TblAccount.Iserial,
                                Ename = row.TblAccount.Ename,
                                Aname = row.TblAccount.Aname
                            };
                        }

                        TblLedgerDetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblDepreciationTransactionHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void GetDetailData()
        {
            SelectedMainRow.DetailList.Clear();
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblDepreciationTransactionDetailAsync(SelectedMainRow.Iserial,
                LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblDepreciationTransactionHeaderViewModel
            {
                CreatedBy = LoggedUserInfo.Iserial,
                TransDate = DateTime.Now,
            };

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow(bool approved)
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblDepreciationTransactionHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblDepreciationTransactionHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial, approved, LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblDepreciationTransactionHeaderAsync((TblDepreciationTransactionHeader)new TblDepreciationTransactionHeader().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        internal void GetLedgerDetail()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;

            Glclient.GetTblLedgerDetailForDepreciationAsync(0, 100, true, DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private ObservableCollection<TblCurrencyTest> _currencyList;

        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private SortableCollectionView<TblDepreciationTransactionHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblDepreciationTransactionHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblDepreciationTransactionHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblDepreciationTransactionHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblDepreciationTransactionHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblDepreciationTransactionHeaderViewModel _selectedMainRow;

        public TblDepreciationTransactionHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<GenericTable> _depreciationMethodList;

        public ObservableCollection<GenericTable> DepreciationMethodList
        {
            get { return _depreciationMethodList; }
            set
            {
                _depreciationMethodList = value;
                RaisePropertyChanged("DepreciationMethodList");
            }
        }

        private ObservableCollection<TblLedgerMainDetailViewModel> _tblLedgerDetailsList;

        public ObservableCollection<TblLedgerMainDetailViewModel> TblLedgerDetailsList
        {
            get { return _tblLedgerDetailsList ?? (_tblLedgerDetailsList = new ObservableCollection<TblLedgerMainDetailViewModel>()); }
            set { _tblLedgerDetailsList = value; RaisePropertyChanged("TblLedgerDetailsList"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblDepreciationTransactionHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        public TblDepreciationTransactionHeaderViewModel()
        {
            DepreciationFactor = 1;
        }

        private ObservableCollection<TblDepreciationTransactionDetail> _detailList;

        public ObservableCollection<TblDepreciationTransactionDetail> DetailList
        {
            get { return _detailList ?? (_detailList = new ObservableCollection<TblDepreciationTransactionDetail>()); }
            set { _detailList = value; RaisePropertyChanged("DetailList"); }
        }

        private DateTime? _approveDateField;

        private bool _approvedField;

        private int? _approvedByField;

        private double? _bookValueField;

        private string _codeField;

        private int? _createdByField;

        private DateTime? _creationDateField;

        private double? _depreciationLifeField;

        private int _iserialField;

        private double? _salvageValueField;

        private DateTime? _startDateField;

        private int? _statusField;

        private int? _tblCurrencyField;

        private int? _tblDepreciationMethodField;

        private GenericTable _tblDepreciationMethod1Field;

        private int? _tblLedgerDetailField;

        private TblLedgerMainDetailViewModel _tblLedgerDetail1Field;

        private DateTime? _transDateField;

        public DateTime? ApproveDate
        {
            get
            {
                return _approveDateField;
            }
            set
            {
                if ((_approveDateField.Equals(value) != true))
                {
                    _approveDateField = value;
                    RaisePropertyChanged("ApproveDate");
                }
            }
        }

        public bool Approved
        {
            get
            {
                return _approvedField;
            }
            set
            {
                if ((_approvedField.Equals(value) != true))
                {
                    _approvedField = value;
                    RaisePropertyChanged("Approved");
                }
            }
        }

        public int? ApprovedBy
        {
            get
            {
                return _approvedByField;
            }
            set
            {
                if ((_approvedByField.Equals(value) != true))
                {
                    _approvedByField = value;
                    RaisePropertyChanged("ApprovedBy");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBookValue")]
        public double? BookValue
        {
            get
            {
                return _bookValueField;
            }
            set
            {
                if ((_bookValueField.Equals(value) != true))
                {
                    _bookValueField = value;
                    RaisePropertyChanged("BookValue");
                }
            }
        }

        public string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public int? CreatedBy
        {
            get
            {
                return _createdByField;
            }
            set
            {
                if ((_createdByField.Equals(value) != true))
                {
                    _createdByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDepreciationLife")]
        public double? DepreciationLife
        {
            get
            {
                return _depreciationLifeField;
            }
            set
            {
                if ((_depreciationLifeField.Equals(value) != true))
                {
                    _depreciationLifeField = value;
                    RaisePropertyChanged("DepreciationLife");
                }
            }
        }

        private double? _depreciationFactor;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDepreciationFactor")]
        public double? DepreciationFactor
        {
            get
            {
                return _depreciationFactor;
            }
            set
            {
                _depreciationFactor = value;
                RaisePropertyChanged("DepreciationFactor");
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSalvageValue")]
        public double? SalvageValue
        {
            get
            {
                return _salvageValueField;
            }
            set
            {
                if ((_salvageValueField.Equals(value) != true))
                {
                    _salvageValueField = value;
                    RaisePropertyChanged("SalvageValue");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStartDate")]
        public DateTime? StartDate
        {
            get
            {
                return _startDateField;
            }
            set
            {
                if ((_startDateField.Equals(value) != true))
                {
                    _startDateField = value;
                    RaisePropertyChanged("StartDate");
                }
            }
        }

        public int? Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                if ((_statusField.Equals(value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public int? TblCurrency
        {
            get
            {
                return _tblCurrencyField;
            }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDepreciationMethod")]
        public int? TblDepreciationMethod
        {
            get
            {
                return _tblDepreciationMethodField;
            }
            set
            {
                if ((_tblDepreciationMethodField.Equals(value) != true))
                {
                    _tblDepreciationMethodField = value;
                    RaisePropertyChanged("TblDepreciationMethod");
                }
            }
        }

        public GenericTable DepreciationMethodPerRow
        {
            get
            {
                return _tblDepreciationMethod1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblDepreciationMethod1Field, value) != true))
                {
                    _tblDepreciationMethod1Field = value;
                    RaisePropertyChanged("DepreciationMethodPerRow");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournal")]
        public int? TblLedgerMainDetail
        {
            get
            {
                return _tblLedgerDetailField;
            }
            set
            {
                if ((_tblLedgerDetailField.Equals(value) != true))
                {
                    _tblLedgerDetailField = value;
                    RaisePropertyChanged("TblLedgerMainDetail");
                }
            }
        }

        public TblLedgerMainDetailViewModel LedgerDetailPerRow
        {
            get
            {
                return _tblLedgerDetail1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblLedgerDetail1Field, value) != true))
                {
                    _tblLedgerDetail1Field = value;
                    RaisePropertyChanged("LedgerDetailPerRow");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }
    }

    #endregion ViewModels
}