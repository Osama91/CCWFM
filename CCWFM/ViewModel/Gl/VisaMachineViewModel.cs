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
using CCWFM.Helpers.Utilities;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class VisaMachineViewModel : ViewModelBase
    {
        public VisaMachineViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.VisaMachineForm.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblVisaMachineViewModel>();
                SelectedMainRow = new TblVisaMachineViewModel();


                if (MainRowList.Any() && (SelectedMainRow == null))
                {
                    SelectedMainRow = MainRowList.FirstOrDefault();
                }

                Glclient.GetTblVisaMachineCompleted  += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblVisaMachineViewModel();
                        newrow.TblbankPerRow = new TblBank();
                        newrow.InjectFrom(row);

                        if (row.TblBank1 != null) newrow.TblbankPerRow.InjectFrom(row.TblBank1);

                        newrow.JournalAccountTypePerRow = new GlService.GenericTable();
                        if (newrow.TblJournalAccountType.HasValue)
                        {
                            newrow.JournalAccountTypePerRow =
                                JournalAccountTypeList.FirstOrDefault(jAT => jAT.Iserial == newrow.TblJournalAccountType.Value);
                        }

                        var entity = sv.EntityAccounts.FirstOrDefault(x => x.TblJournalAccountType == newrow.TblJournalAccountType &&
                                                         x.Iserial == row.EntityAccount);
                        if (entity != null)
                        {
                            newrow.EntityPerRow.InjectFrom(entity); newrow.RaiseEntityChanged();
                        }


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
                        ExportGrid.ExportExcel("VisaMachines");
                    }
                };

                Glclient.UpdateOrInsertTblVisaMachineCompleted  += (s, ev) =>
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
                Glclient.DeleteTblVisaMachineCompleted  += (s, ev) =>
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

                Glclient.GetVisaMachineBanksCompleted  += (s, sv) =>
                {
                    BankList = sv.Result;
                };

                Glclient.GetVisaMachineBanksAsync(LoggedUserInfo.DatabasEname);

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
            Glclient.GetTblVisaMachineAsync (MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,LoggedUserInfo.DatabasEname);
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
            var newrow = new TblVisaMachineViewModel();
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
                    var saveRow = new TblVisaMachine();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblVisaMachineAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblVisaMachineAsync ((TblVisaMachine)new TblVisaMachine().InjectFrom(row),
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

        private SortableCollectionView<TblVisaMachineViewModel> _mainRowList;

        public SortableCollectionView<TblVisaMachineViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblVisaMachineViewModel> _selectedMainRows;

        public ObservableCollection<TblVisaMachineViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblVisaMachineViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblVisaMachineViewModel _selectedMainRow;

        public TblVisaMachineViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private ObservableCollection<TblBank> _bankList;

        public ObservableCollection<TblBank> BankList
        {
            get { return _bankList; }
            set { _bankList = value; RaisePropertyChanged("BankList"); }
        }

        ObservableCollection<GlService.GenericTable> _journalAccountTypeList = null;
        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get
            {
                return _journalAccountTypeList ?? (_journalAccountTypeList =
                  new ObservableCollection<GlService.GenericTable>() {
                      new GlService.GenericTable(){
                        Iserial = 15,
                        Code = "Expenses",
                        Ename = "Expenses",
                        Aname = "Expenses",
                    }});
            }
            set { _journalAccountTypeList = value; RaisePropertyChanged(nameof(JournalAccountTypeList)); }
        }


        #endregion Prop
    }

    #region ViewModels

    public class TblVisaMachineViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserialField;
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

        private string _codeField;

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

        private string _machineIdField;

        public string MachineId
        {
            get
            {
                return _machineIdField;
            }
            set
            {
                if ((ReferenceEquals(_machineIdField, value) != true))
                {
                    _machineIdField = value;
                    RaisePropertyChanged("MachineId");
                }
            }
        }


        private int _tblbankField;

        public int TblBank
        {
            get
            {
                return _tblbankField;
            }
            set
            {
                if ((_tblbankField.Equals(value) != true))
                {
                    _tblbankField = value;
                    RaisePropertyChanged("TblBank");
                }
            }
        }


        private decimal _discountPercentField;
        public decimal DiscountPercent
        {
            get
            {
                return _discountPercentField;
            }
            set
            {
                if ((_discountPercentField.Equals(value) != true))
                {
                    _discountPercentField = value;
                    RaisePropertyChanged("DiscountPercent");
                }
            }
        }

        private int _entityAccountField;
        public int EntityAccount
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

        GlService.Entity _entityPerRow = new GlService.Entity();
        public GlService.Entity EntityPerRow
        {
            get { return _entityPerRow ?? (_entityPerRow = new GlService.Entity()); }
            set
            {
                _entityPerRow = value;
                RaiseEntityChanged();
                if (value != null)
                {
                    EntityAccount = value.Iserial;
                    //TblJournalAccountType = value.TblJournalAccountType;
                    //
                }
            }
        }

        private System.Nullable<int> TblJournalAccountTypeField;
        public System.Nullable<int> TblJournalAccountType
        {
            get
            {
                //Expenses
                return 15;
            }
            set
            {
                if ((this.TblJournalAccountTypeField.Equals(value) != true))
                {
                    this.TblJournalAccountTypeField = value;
                    this.RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        private TblBank _tblbankPerRow;

        public TblBank TblbankPerRow
        {
            get { return _tblbankPerRow; }
            set
            {
                if ((ReferenceEquals(_tblbankPerRow, value) != true))
                {
                    _tblbankPerRow = value;
                    RaisePropertyChanged("TblbankPerRow");
                    if (_tblbankPerRow != null)
                    {
                        if (_tblbankPerRow.Iserial != 0)
                        {
                            TblBank = _tblbankPerRow.Iserial;
                        }
                    }
                }
            }
        }

        GlService.GenericTable _journalAccountTypePerRow;
        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if (_journalAccountTypePerRow == null || !_journalAccountTypePerRow.Equals(value))
                    EntityPerRow = null;
                _journalAccountTypePerRow = value;
                RaisePropertyChanged(nameof(JournalAccountTypePerRow));
                if (JournalAccountTypePerRow != null && JournalAccountTypePerRow.Iserial > 0)
                { RaiseEntityChanged();}
            }
        }

        internal void RaiseEntityChanged()
        {
            RaisePropertyChanged(nameof(EntityPerRow));
        }

    }

    #endregion ViewModels
}
