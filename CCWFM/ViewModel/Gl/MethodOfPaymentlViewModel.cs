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
    public class MethodOfPaymentViewModel : ViewModelBase
    {
        public MethodOfPaymentViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.MethodOfPayment.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblMethodOfPaymentViewModel>();
                SelectedMainRow = new TblMethodOfPaymentViewModel();

                var bankTransactionTypeClient = new GlServiceClient();
                bankTransactionTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    BankTransactionTypeList = sv.Result;
                };
                bankTransactionTypeClient.GetGenericAsync("TblBankTransactionType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetTblMethodOfPaymentCompleted += (s, sv) =>
              {
                  foreach (var row in sv.Result)
                  {
                      var newrow = new TblMethodOfPaymentViewModel();
                      if (row.TblBankTransactionType1 != null)
                      {
                          newrow.BankTransactionTypePerRow = new GenericTable();
                          newrow.BankTransactionTypePerRow.InjectFrom(row.TblBankTransactionType1);
                      }
                      if (row.TblJournalAccountType1 != null)
                      {
                          newrow.JournalAccountTypePerRow = new GenericTable();
                          newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                      }
                      newrow.EntityAccountPerRow =
              sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType && x.Iserial == row.Entity);
                      newrow.AccountPerRow = row.TblAccount;
                      newrow.InjectFrom(row);
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

                Glclient.UpdateOrInsertTblMethodOfPaymentsCompleted += (s, ev) =>
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
                };
                Glclient.DeleteTblMethodOfPaymentCompleted += (s, ev) =>
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

                GetMaindata();
            }
        }

        private void MainRowList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                MainRowList.Clear();
                SortBy = null;
                foreach (var sortDesc in MainRowList.SortDescriptions)
                {
                    SortBy = SortBy + "it." + sortDesc.PropertyName +
                             (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblMethodOfPaymentAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
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
            var newrow = new TblMethodOfPaymentViewModel();
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
                    var saveRow = new TblMethodOfPayment();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblMethodOfPaymentsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                        LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldRow(TblMethodOfPaymentViewModel oldRow)
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
                    var saveRow = new TblMethodOfPayment();
                    saveRow.InjectFrom(oldRow);

                    Glclient.UpdateOrInsertTblMethodOfPaymentsAsync(saveRow, save, MainRowList.IndexOf(oldRow),
                        LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblMethodOfPaymentAsync((TblMethodOfPayment)new TblMethodOfPayment().InjectFrom(row),
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

        #region Prop

        private SortableCollectionView<TblMethodOfPaymentViewModel> _mainRowList;

        public SortableCollectionView<TblMethodOfPaymentViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<GenericTable> _bankTransactionTypeList;

        public ObservableCollection<GenericTable> BankTransactionTypeList
        {
            get { return _bankTransactionTypeList; }
            set { _bankTransactionTypeList = value; RaisePropertyChanged("BankTransactionTypeList"); }
        }

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<TblMethodOfPaymentViewModel> _selectedMainRows;

        public ObservableCollection<TblMethodOfPaymentViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblMethodOfPaymentViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblMethodOfPaymentViewModel _selectedMainRow;

        public TblMethodOfPaymentViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblMethodOfPaymentViewModel : GenericViewModel
    {
        private int? _tblBankTransactionType;

        private int? _tblJournalAccountTypeField;

        public int? TblBankTransactionType
        {
            get
            {
                return _tblBankTransactionType;
            }
            set
            {
                if ((_tblBankTransactionType.Equals(value) != true))
                {
                    _tblBankTransactionType = value;
                    RaisePropertyChanged("TblBankTransactionType");
                }
            }
        }

        private Entity _entityAccountPerRow;

        public Entity EntityAccountPerRow
        {
            get { return _entityAccountPerRow; }
            set
            {
                if ((ReferenceEquals(_entityAccountPerRow, value) != true))
                {
                    _entityAccountPerRow = value;
                    RaisePropertyChanged("EntityAccountPerRow");
                    Entity = EntityAccountPerRow.Iserial;
                }
            }
        }

        private TblAccount _accountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _accountPerRow; }
            set
            {
                if ((ReferenceEquals(_accountPerRow, value) != true))
                {
                    _accountPerRow = value;
                    RaisePropertyChanged("AccountPerRow");
                    if (AccountPerRow != null) BridgingAccount = AccountPerRow.Iserial;
                }
            }
        }

        private bool _bridgingPosting;

        public bool BridgingPosting
        {
            get { return _bridgingPosting; }
            set { _bridgingPosting = value; RaisePropertyChanged("BridgingPosting"); }
        }

        private int? _bridgingAccountAccountField;

        public int? BridgingAccount
        {
            get { return _bridgingAccountAccountField; }
            set
            {
                if ((_bridgingAccountAccountField.Equals(value) != true))
                {
                    _bridgingAccountAccountField = value;
                    RaisePropertyChanged("BridgingAccount");
                }
            }
        }

        private int? _entityAccountField;

        public int? Entity
        {
            get { return _entityAccountField; }
            set
            {
                if ((_entityAccountField.Equals(value) != true))
                {
                    _entityAccountField = value;
                    RaisePropertyChanged("Entity");
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

        private GenericTable _journalAccountTypePerRow;

        private GenericTable _bankTransactionTypePerRow;

        public GenericTable BankTransactionTypePerRow
        {
            get { return _bankTransactionTypePerRow; }
            set
            {
                if ((ReferenceEquals(_bankTransactionTypePerRow, value) != true))
                {
                    _bankTransactionTypePerRow = value;
                    RaisePropertyChanged("BankTransactionTypePerRow");
                    if (BankTransactionTypePerRow != null) TblBankTransactionType = BankTransactionTypePerRow.Iserial;
                }
            }
        }

        public GenericTable JournalAccountTypePerRow
        {
            get
            {
                return _journalAccountTypePerRow;
            }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                    TblJournalAccountType = JournalAccountTypePerRow.Iserial;
                }
            }
        }
    }

    #endregion ViewModels
}