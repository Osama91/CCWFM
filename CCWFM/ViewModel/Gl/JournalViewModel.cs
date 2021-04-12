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
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class JournalViewModel : ViewModelBase
    {
        public JournalViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Journal.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblJournalViewModel>();
                SelectedMainRow = new TblJournalViewModel();

                var currencyClient = new GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var journalTypeClient = new GlServiceClient();
                journalTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalTypeList = sv.Result;
                };
                journalTypeClient.GetGenericAsync("TblJournalType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                Glclient.GetTblJournalCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblJournalViewModel();
                        newrow.InjectFrom(row);

                        newrow.JournalAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        newrow.JournalTypePerRow = new GenericTable();
                        newrow.JournalTypePerRow.InjectFrom(row.TblJournalType1);
                        newrow.CurrencyPerRow = new GenericTable();
                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);

                        newrow.HeaderSequencePerRow = row.TblSequence;
                        newrow.DetailSequencePerRow = row.TblSequence1;
                        newrow.OffsetEntityAccountPerRow =
                sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType && x.Iserial == row.Entity);

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
                        ExportGrid.ExportExcel("Journal");
                    }
                };

                Glclient.UpdateOrInsertTblJournalsCompleted += (s, ev) =>
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
                Glclient.DeleteTblJournalCompleted += (s, ev) =>
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
            Glclient.GetTblJournalAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
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
            var newrow = new TblJournalViewModel();
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
                    var saveRow = new TblJournal();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblJournalsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblJournalViewModel oldRow)
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
                    var saveRow = new TblJournal();
                    saveRow.InjectFrom(oldRow);
                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblJournalsAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblJournalAsync((TblJournal)new TblJournal().InjectFrom(row),
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

        private SortableCollectionView<TblJournalViewModel> _mainRowList;

        public SortableCollectionView<TblJournalViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<GenericTable> _journalTypeList;

        public ObservableCollection<GenericTable> JournalTypeList
        {
            get { return _journalTypeList; }
            set { _journalTypeList = value; RaisePropertyChanged("JournalTypeList"); }
        }

        private ObservableCollection<TblJournalViewModel> _selectedMainRows;

        public ObservableCollection<TblJournalViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblJournalViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblJournalViewModel _selectedMainRow;

        public TblJournalViewModel SelectedMainRow
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

    public class TblJournalViewModel : GenericViewModel
    {
        private string _report;

        public string Report
        {
            get { return _report; }
            set { _report = value; RaisePropertyChanged("Report"); }
        }

        private int? _tblCurrencyField;

        private int? _tblJournalAccountTypeField;

        private int? _tblJournalTypeField;

        private int? _headerSequenceField;

        private int? _detailSequenceField;

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

        private Entity _offsetEntityAccountPerRow;

        public Entity OffsetEntityAccountPerRow
        {
            get { return _offsetEntityAccountPerRow; }
            set
            {
                if ((ReferenceEquals(_offsetEntityAccountPerRow, value) != true))
                {
                    _offsetEntityAccountPerRow = value;
                    RaisePropertyChanged("OffsetEntityAccountPerRow");
                    if (OffsetEntityAccountPerRow != null) Entity = OffsetEntityAccountPerRow.Iserial;
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSequence")]
        public int? HeaderSequence
        {
            get
            {
                return _headerSequenceField;
            }
            set
            {
                if ((_headerSequenceField.Equals(value) != true))
                {
                    _headerSequenceField = value;
                    RaisePropertyChanged("HeaderSequence");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSequence")]
        public int? DetailSequence
        {
            get
            {
                return _detailSequenceField;
            }
            set
            {
                if ((_detailSequenceField.Equals(value) != true))
                {
                    _detailSequenceField = value;
                    RaisePropertyChanged("DetailSequence");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalType")]
        public int? TblJournalType
        {
            get
            {
                return _tblJournalTypeField;
            }
            set
            {
                if ((_tblJournalTypeField.Equals(value) != true))
                {
                    _tblJournalTypeField = value;
                    RaisePropertyChanged("TblJournalType");
                }
            }
        }

        private GenericTable _journalAccountTypePerRow;

        private GenericTable _journalTypePerRow;

        private TblSequence _headerSequencePerRowField;

        private TblSequence _detailSequencePerRowField;

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

        public GenericTable JournalTypePerRow
        {
            get
            {
                return _journalTypePerRow;
            }
            set
            {
                if ((ReferenceEquals(_journalTypePerRow, value) != true))
                {
                    _journalTypePerRow = value;
                    RaisePropertyChanged("JournalTypePerRow");
                    TblJournalType = JournalTypePerRow.Iserial;
                }
            }
        }

        public TblSequence HeaderSequencePerRow
        {
            get
            {
                return _headerSequencePerRowField;
            }
            set
            {
                if ((ReferenceEquals(_headerSequencePerRowField, value) != true))
                {
                    _headerSequencePerRowField = value;
                    RaisePropertyChanged("HeaderSequencePerRow");
                    HeaderSequence = HeaderSequencePerRow.Iserial;
                }
            }
        }

        public TblSequence DetailSequencePerRow
        {
            get
            {
                return _detailSequencePerRowField;
            }
            set
            {
                if ((ReferenceEquals(_detailSequencePerRowField, value) != true))
                {
                    _detailSequencePerRowField = value;
                    RaisePropertyChanged("DetailSequencePerRow");
                    DetailSequence = DetailSequencePerRow.Iserial;
                }
            }
        }
    }

    #endregion ViewModels
}