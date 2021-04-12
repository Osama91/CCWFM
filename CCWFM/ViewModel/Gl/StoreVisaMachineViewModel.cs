using CCWFM.BankDepositService;
using CCWFM.Helpers;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Models;
using CCWFM.Models.LocalizationHelpers;
using GalaSoft.MvvmLight.Command;
using Omu.ValueInjecter.Silverlight;
using Syncfusion.Data.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.ViewModel.Gl
{
    // الحساب مش بيسيف فى الداتابيز
    public class StoreVisaMachineViewModel : ViewModelStructuredBase
    {
        BankDepositServiceClient BankDepositClient = Services.Instance.GetBankDepositServiceClient();
        public StoreVisaMachineViewModel() : base(PermissionItemName.StoreVisaMachine)
        {
            if (!DesignerProperties.IsInDesignTool)
            {   
                MainRowList = new ObservableCollection<StoreVisaMachine>();

                BankDepositClient.GetVisaMachineCompleted += (s, sv) =>
                {
                    MainRowList.Clear();
                    if (sv.Result.Count > 0)
                    {
                        var storeVisaMachine = sv.Result.FirstOrDefault();
                        DiscountPercent = storeVisaMachine.DiscountPercent;
                        EntityPerRow = new GlService.Entity()
                        {
                            Iserial = storeVisaMachine.EntityAccount,
                            TblJournalAccountType = JournalAccountTypePerRow.Iserial,
                            Code = storeVisaMachine.EntityCode,
                        };
                    }
                    else
                    {
                        DiscountPercent = 0;
                        EntityPerRow = new GlService.Entity();
                    }
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }
                    Loading = false;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                BankDepositClient.UpdateOrInsertVisaMachineCompleted += (s, x) =>
                {
                    if (x.Error != null)
                    {
                        MessageBox.Show(Helper.GetInnerExceptionMessage(x.Error));
                    }
                    Loading = false;
                    MainRowList.Clear();
                    foreach (var item in x.Result)
                    {
                        var savedRow = MainRowList.FirstOrDefault(r => r.StoreVisaMachineIserial == item.StoreVisaMachineIserial);
                        if (savedRow != null)
                        {
                            savedRow.InjectFrom(item);
                            BankRec = BankList.FirstOrDefault(c => c.Iserial == savedRow.BankIserial);
                        }
                        else
                            MainRowList.Add(item);
                    }
                    if (x.Result.Count > 0) MessageBox.Show(strings.SavedMessage);
                    DeleteCommand.RaiseCanExecuteChanged();
                    IsNewChanged();

                };

                #region Delete

                DeleteDetail = new RelayCommand<object>((o) => {
                    if (((KeyEventArgs)(o)).Key == Key.Delete)
                    {
                        if (SelectedMainRow.StoreVisaMachineIserial <= 0 || SelectedMainRow.VisaMachineIserial <= 0)
                        {
                            MainRowList.Remove(SelectedMainRow);
                            if (MainRowList.Count == 0)
                            {
                                AddNewMainRow(false);
                            }
                        }
                        else
                            DeleteMainRow();
                    }
                });
                BankDepositClient.DeleteVisaMachineCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.StoreVisaMachineIserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                #endregion

                NewVisaCommand = new RelayCommand<object>((o) => {
                    if (((KeyEventArgs)(o)).Key == Key.Down)
                    {
                        NewCommand.Execute(null);
                    }
                });

                BankDepositClient.GetLookUpStoreCompleted += (s, e) =>
                {
                    StoreList = e.Result;
                };
                BankDepositClient.GetLookUpBankCompleted += (s, e) =>
                {
                    BankList = e.Result;
                };
                
                GetComboData();
                AddNewMainRow(false);
            }
        }
        
        #region Methods
        
        public bool ValidHeaderData()
        {
            if (BankRec == null)
            {
                MessageBox.Show(strings.ReqBankAccountNo);
                return false;
            }
            if (MainRowList.Any(r => string.IsNullOrWhiteSpace(r.MachineId)))
            {
                MessageBox.Show(strings.ReqMachineId);
                return false;
            }
            var duplicatedMachineId = MainRowList.GroupBy(r => new { r.MachineId }).FirstOrDefault(x => x.Count() > 1);
            if (duplicatedMachineId!=null)
            {
                MessageBox.Show(string.Format("Machine Id cannot be duplicated {0}", duplicatedMachineId.Key.MachineId));
                return false;
            }
            if (MainRowList.GroupBy(r => new { r.StoreIserial, r.IsDefault }).Where(r =>
            r.Key.IsDefault).Select(r => r.Count()).Any(r => r > 1))
            {
                MessageBox.Show("Store Can have one Default Machine");
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
            BankDepositClient.GetVisaMachineAsync(BankRec.Iserial, LoggedUserInfo.DatabasEname);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    BankDepositClient.DeleteVisaMachineAsync(SelectedMainRow, LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            if (BankRec == null)
                return;
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                SelectedMainRow = new StoreVisaMachine() { BankIserial = BankRec.Iserial, DiscountPercent = DiscountPercent, EntityAccount = EntityPerRow.Iserial };
                MainRowList.Add(SelectedMainRow);
            }
        }
        public void SaveMainRow()
        {
            if (MainRowList.Count > 0)
            {
                var saveRows = new ObservableCollection<StoreVisaMachine>();
                foreach (var item in MainRowList)
                {
                    item.DiscountPercent = DiscountPercent;
                    item.EntityAccount = EntityPerRow.Iserial;
                    saveRows.Add(item);
                }

                BankDepositClient.UpdateOrInsertVisaMachineAsync(saveRows, LoggedUserInfo.DatabasEname);
                Loading = true;
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            BankDepositClient.GetLookUpBankAsync(LoggedUserInfo.DatabasEname);
            BankDepositClient.GetLookUpStoreAsync(LoggedUserInfo.DatabasEname, true);
        }

        #endregion

        #region Properties

        private ObservableCollection<StoreVisaMachine> _mainRowList;
        public ObservableCollection<StoreVisaMachine> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }

        private ObservableCollection<StoreVisaMachine> _selectedMainRows;
        public ObservableCollection<StoreVisaMachine> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<StoreVisaMachine>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private StoreVisaMachine _selectedMainRow;
        public StoreVisaMachine SelectedMainRow
        {
            get
            {
                return _selectedMainRow;
            }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                IsNewChanged();
            }
        }
        
        #region Combo Data

        ObservableCollection<TblStore> _storeList = new ObservableCollection<TblStore>();
        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set { _storeList = value; RaisePropertyChanged(nameof(StoreList)); }
        }
     
        ObservableCollection<TblBank> _bankList = new ObservableCollection<TblBank>();
        public ObservableCollection<TblBank> BankList
        {
            get { return _bankList ?? (_bankList = new ObservableCollection<TblBank>()); }
            set { _bankList = value; RaisePropertyChanged(nameof(BankList)); }
        }

        #endregion

        TblBank bankRec;
        public TblBank BankRec
        {
            set
            {
                bankRec = value; RaisePropertyChanged(nameof(BankRec));
                if (value != null) GetMaindata();
            }
            get { return bankRec; }
        }

        decimal discountPercent;
        public decimal DiscountPercent
        {
            get { return discountPercent; }
            set
            {
                discountPercent = value; RaisePropertyChanged(nameof(DiscountPercent));
                foreach (var item in MainRowList)
                {
                    item.DiscountPercent = discountPercent;
                }
            }
        }


        GlService.Entity _entityAccount = new GlService.Entity();
        public GlService.Entity EntityPerRow
        {
            get { return _entityAccount ?? (_entityAccount = new GlService.Entity()); }
            set
            {
                _entityAccount = value; RaisePropertyChanged(nameof(EntityPerRow));
                foreach (var item in MainRowList)
                {
                    item.EntityAccount = _entityAccount.Iserial;
                }
            }
        }

        GlService.GenericTable _journalAccountTypePerRow;
        public GlService.GenericTable JournalAccountTypePerRow
        {
            get
            {
                return _journalAccountTypePerRow ?? (_journalAccountTypePerRow =
                    new GlService.GenericTable()
                    {
                        Iserial = 15,
                        Code = "Expenses",
                        Ename = "Expenses",
                        Aname = "Expenses",
                    });
            }
        }
        GlService.GenericTable scopePerRow = new GlService.GenericTable();
        public GlService.GenericTable ScopePerRow
        {
            get
            {
                return scopePerRow ?? (scopePerRow =
                    new GlService.GenericTable()
                    {
                        Iserial = 0,
                        Code = "0",
                        Ename = "Expenses",
                        Aname = "Expenses",
                    });
            }
        }
        #endregion

        #region Commands

        RelayCommand<object> deleteDetail;
        public RelayCommand<object> DeleteDetail
        {
            get { return deleteDetail; }
            set { deleteDetail = value; RaisePropertyChanged(nameof(DeleteDetail)); }
        }

        RelayCommand<object> newVisaCommand;
        public RelayCommand<object> NewVisaCommand
        {
            get { return newVisaCommand; }
            set { newVisaCommand = value; RaisePropertyChanged(nameof(NewVisaCommand)); }
        }

        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
        }       
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData();
        }
        public override void Search()
        {
           
        }   
        public override void DeleteRecord()
        {
            this.DeleteMainRow();
            AddNewMainRow(false);
            base.DeleteRecord();
        }      
        public override void Cancel()
        {
            MainRowList.Clear();
            SelectedMainRows.Clear();
            AddNewMainRow(false);
            base.Cancel();
        }
        public override void Print()
        {
            base.Print();
            var rVM = new GenericReportViewModel();
            //var para = new ObservableCollection<string>() { SelectedMainRow.Iserial.ToString() };
            //para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
            //para.Add(LoggedUserInfo.DatabasEname);
            //rVM.GenerateReport("CashDepositeDocument", para);
        }

        #endregion
    }
}