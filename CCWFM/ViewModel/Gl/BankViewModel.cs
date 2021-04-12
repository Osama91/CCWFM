using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class BankViewModel : ViewModelBase
    {
        public BankViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Bank.ToString());
                Glclient = new GlServiceClient();
                var currencyClient = new GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var BankAccountTypeClient = new GlServiceClient();
                BankAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    BankAccountTypeList = sv.Result;
                };
                BankAccountTypeClient.GetGenericAsync("TblBankAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);


                var bankClient = new GlServiceClient();
                bankClient.GetGenericCompleted += (s, sv) =>
                {
                    BankGroupList = sv.Result;
                };
                bankClient.GetGenericAsync("TblBankGroup", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblBankViewModel>();
                SelectedMainRow = new TblBankViewModel();

                Glclient.GetTblBankChequeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBankChequeViewModel();
                        newrow.InjectFrom(row);

                        if (row.TblCurrency1 != null) newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);

                        if (row.TblBank1 != null) newrow.BankPerRow.InjectFrom(row.TblBank1);
                        if (row.TblJournalAccountType1 != null) newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        newrow.EntityPerRow =
                                      sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                               x.Iserial == row.EntityAccount);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                }
                ;
                Glclient.GetTblBankCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBankViewModel();
                        newrow.CurrencyPerRow = new GenericTable();
                        newrow.BankAccountTypePerRow = new GenericTable();
                        newrow.BankGroupPerRow = new GenericTable();
                        newrow.InjectFrom(row);
                        
                        if (row.TblCurrency1 != null) newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);

                     
                        if (row.TblBankAccountType1 != null) newrow.BankAccountTypePerRow.InjectFrom(row.TblBankAccountType1);

            
                        if (row.TblBankGroup1 != null) newrow.BankGroupPerRow.InjectFrom(row.TblBankGroup1);

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
                };

                Glclient.UpdateOrInsertTblBanksCompleted += (s, ev) =>
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
                Glclient.DeleteTblBankCompleted += (s, ev) =>
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
                Glclient.CreateChequeCompleted += (s, sv) => MessageBox.Show("Cheques Generated");
                Glclient.DeleteTblBankChequeCompleted += (s, sv) =>
                {
                    MessageBox.Show("Cheques Deleted");
                    SelectedMainRow.DetailsList.Clear();
                    GetDetailRow();
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblBankAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetDetailRow()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblBankChequeAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial, SortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname, 4);
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
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblBankViewModel();
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
                    var saveRow = new TblBank();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblBanksAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblBankViewModel oldRow)
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
                    var saveRow = new TblBank();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblBanksAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblBankAsync((TblBank)new TblBank().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        #region Prop

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _BankAccountTypeList;

        public ObservableCollection<GenericTable> BankAccountTypeList
        {
            get { return _BankAccountTypeList; }
            set { _BankAccountTypeList = value; RaisePropertyChanged("BankAccountTypeList"); }
        }

        private ObservableCollection<GenericTable> _bankList;

        public ObservableCollection<GenericTable> BankGroupList
        {
            get { return _bankList; }
            set { _bankList = value; RaisePropertyChanged("BankGroupList"); }
        }

        private SortableCollectionView<TblBankViewModel> _mainRowList;

        public SortableCollectionView<TblBankViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblBankViewModel> _selectedMainRows;

        public ObservableCollection<TblBankViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBankViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBankViewModel _selectedMainRow;

        public TblBankViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }


        private TblBankChequeViewModel _SelectedDetailRow;

        public TblBankChequeViewModel SelectedDetailRow
        {
            get { return _SelectedDetailRow; }
            set { _SelectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        
        #endregion Prop

        public void CreateCheck(long from, int to)
        {
            Glclient.CreateChequeAsync(SelectedMainRow.Iserial, from, to, LoggedUserInfo.DatabasEname);
        }

        public void DeleteCheck(int from, int to)
        {
            Glclient.DeleteTblBankChequeAsync(SelectedMainRow.Iserial, from, to, LoggedUserInfo.DatabasEname);
        }
    }

    #region ViewModels

    public class TblBankViewModel : GenericViewModel
    {
        private ObservableCollection<TblBankChequeViewModel> _detailLst;

        public ObservableCollection<TblBankChequeViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new ObservableCollection<TblBankChequeViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private GenericTable _tblcurrency;

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
        private GenericTable _BankAccountTypePerRow;

        public GenericTable BankAccountTypePerRow
        {
            get { return _BankAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_BankAccountTypePerRow, value) != true))
                {
                    _BankAccountTypePerRow = value;
                    RaisePropertyChanged("BankAccountTypePerRow");
                    if (_BankAccountTypePerRow!=null)
                    {
                        if (_BankAccountTypePerRow.Iserial!=0)
                        {
                            TblBankAccountType = _BankAccountTypePerRow.Iserial;
                        }
                       
                    }
                  
                }
            }
        }



        private int? _TblBankAccountType;
        
        public int? TblBankAccountType
        {
            get
            {
                return _TblBankAccountType;
            }
            set
            {
                if ((_TblBankAccountType.Equals(value) != true))
                {
                    _TblBankAccountType = value;
                    RaisePropertyChanged("TblBankAccountType");
                }
            }
        }

        private GenericTable _bankGroupPerRow;

        public GenericTable BankGroupPerRow
        {
            get { return _bankGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_bankGroupPerRow, value) != true))
                {
                    _bankGroupPerRow = value;
                    RaisePropertyChanged("BankGroupPerRow");
                    if (BankGroupPerRow != null)
                    {
                        if (BankGroupPerRow.Iserial != 0)
                        {
                            TblBankGroup = BankGroupPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private string _bankAccountNoField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBankAccountNo")]
        public string BankAccountNo
        {
            get
            {
                return _bankAccountNoField;
            }
            set
            {
                if ((ReferenceEquals(_bankAccountNoField, value) != true))
                {
                    if (value != null) _bankAccountNoField = value.Trim();
                    RaisePropertyChanged("BankAccountNo");
                }
            }
        }

        private int? _tblTblBankGroupField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBankGroup")]
        public int? TblBankGroup
        {
            get
            {
                return _tblTblBankGroupField;
            }
            set
            {
                if ((_tblTblBankGroupField.Equals(value) != true))
                {
                    _tblTblBankGroupField = value;
                    RaisePropertyChanged("TblBankGroup");
                }
            }
        }

        private int? _tblCurrencyField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
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
    }

    public class TblBankChequeViewModel : Web.DataLayer.PropertiesViewModelBase
    {
    
        private double _amountField;

        private long _chequeField;

        private int? _entityAccountField;

        private int _iserialField;

        private int _statusField;

        private int _tblBankField;

        private TblBank _tblBank1Field;

        private int _tblCurrencyField;

        private TblCurrencyTest _tblCurrency1Field;

        private int? _tblJournalAccountTypeField;

        private TblJournalAccountType _tblJournalAccountType1Field;

        private DateTime? _transDateField;

        public double Amount
        {
            get
            {
                return _amountField;
            }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public long Cheque
        {
            get
            {
                return _chequeField;
            }
            set
            {
               
                    _chequeField = value;
                    RaisePropertyChanged("Cheque");
               
            }
        }

        public int? EntityAccount
        {
            get
            {
                return _entityAccountField;
            }
            set
            {
                if ((_entityAccountField.Equals(value) != true))
                {
                    _entityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
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

        public int Status
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

        public int TblBank
        {
            get
            {
                return _tblBankField;
            }
            set
            {
                if ((_tblBankField.Equals(value) != true))
                {
                    _tblBankField = value;
                    RaisePropertyChanged("TblBank");
                }
            }
        }

        public TblBank BankPerRow
        {
            get
            {
                return _tblBank1Field ?? (_tblBank1Field = new TblBank());
            }
            set
            {
                if ((ReferenceEquals(_tblBank1Field, value) != true))
                {
                    _tblBank1Field = value;
                    RaisePropertyChanged("BankPerRow");
                }
            }
        }

        public int TblCurrency
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

        public TblCurrencyTest CurrencyPerRow
        {
            get
            {
                return _tblCurrency1Field ?? (_tblCurrency1Field = new TblCurrencyTest());
            }
            set
            {
                if ((ReferenceEquals(_tblCurrency1Field, value) != true))
                {
                    _tblCurrency1Field = value;
                    RaisePropertyChanged("CurrencyPerRow");
                }
            }
        }

        public int? TblJournalAccountType
        {
            get
            {
                return _tblJournalAccountTypeField;
            }
            set
            {
                if ((_tblJournalAccountTypeField.Equals(value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        public TblJournalAccountType JournalAccountTypePerRow
        {
            get
            {
                return _tblJournalAccountType1Field ?? (_tblJournalAccountType1Field = new TblJournalAccountType());
            }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountType1Field, value) != true))
                {
                    _tblJournalAccountType1Field = value;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                }
            }
        }

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

        private string _PayTo;

        public string PayTo
        {
            get { return _PayTo; }
            set { _PayTo = value;RaisePropertyChanged("PayTo"); }
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
                        //   DrOrCr = null;
                        //if (EntityPerRow.AccountMustDrOrCr == 0)
                        //{
                        //    DrOrCr = null;
                        //}

                        //if (EntityPerRow.AccountMustDrOrCr == 1)
                        //{
                        //    DrOrCr = true;
                        //}
                        //if (EntityPerRow.AccountMustDrOrCr == 2)
                        //{
                        //    DrOrCr = false;
                        //}
                    }
                    if (EntityPerRow != null)
                    {
                        EntityAccount = EntityPerRow.Iserial;
                    }
                }
            }
        }
    }

    #endregion ViewModels
}