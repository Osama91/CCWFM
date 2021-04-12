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
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class PostingProfileHeaderViewModel : ViewModelBase
    {
        public PostingProfileHeaderViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.PostingProfile.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblPostingProfileHeaderViewModel>();
                SelectedMainRow = new TblPostingProfileHeaderViewModel();

                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetTblPostingProfileHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPostingProfileHeaderViewModel();
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

                Glclient.UpdateOrInsertTblPostingProfileHeadersCompleted += (s, ev) =>
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
                Glclient.DeleteTblPostingProfileHeaderCompleted += (s, ev) =>
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

                Glclient.GetTblPostingProfileDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPostingProfileDetailViewModel
                        {
                            AccountPerRow = row.TblAccount1,
                            JournalAccountTypePerRow = new GenericTable()
                        };
                        newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);

                        newrow.InjectFrom(row);
                        newrow.EntityAccountPerRow =
                            sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == newrow.Type && x.scope == newrow.Scope &&
                                                              x.Iserial == newrow.Entity);
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

                    if (Export)
                    {
                        Export = false;
                        ExportGrid.ExportExcel("Users");
                    }
                };

                Glclient.UpdateOrInsertTblPostingProfileDetailsCompleted += (s, x) =>
                {
                    var savedRow = (TblPostingProfileDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblPostingProfileDetailCompleted += (s, ev) =>
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
            Glclient.GetTblPostingProfileHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblPostingProfileHeaderViewModel();
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
                    var saveRow = new TblPostingProfileHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblPostingProfileHeadersAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                        LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldRow(TblPostingProfileHeaderViewModel oldRow)
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
                    var saveRow = new TblPostingProfileHeader();
                    saveRow.InjectFrom(oldRow);

                    Glclient.UpdateOrInsertTblPostingProfileHeadersAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblPostingProfileHeaderAsync((TblPostingProfileHeader)new TblPostingProfileHeader().InjectFrom(row),
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
                Glclient.GetTblPostingProfileDetailAsync(SelectedMainRow.DetailsList.Count(x => x.Iserial != 0), PageSize, SelectedMainRow.Iserial,
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
                            Glclient.DeleteTblPostingProfileDetailAsync(
                                (TblPostingProfileDetail)new TblPostingProfileDetail().InjectFrom(row),
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

            var newrow = new TblPostingProfileDetailViewModel { TblPostingProfileHeader = SelectedMainRow.Iserial, JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault() };

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
                    if (SelectedDetailRow.TblPostingProfileHeader == 0)
                    {
                        SelectedDetailRow.TblPostingProfileHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblPostingProfileDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblPostingProfileDetailsAsync(rowToSave, save,
                          SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldDetailRow(TblPostingProfileDetailViewModel oldRow)
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
                    var saveRow = new TblPostingProfileDetail();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblPostingProfileDetailsAsync(saveRow, save, SelectedMainRow.DetailsList.IndexOf(oldRow),
                      LoggedUserInfo.DatabasEname);
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

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private SortableCollectionView<TblPostingProfileHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblPostingProfileHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblPostingProfileHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblPostingProfileHeaderViewModel> SelectedMainRows
        {
            get
            {
                return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPostingProfileHeaderViewModel>());
            }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblPostingProfileHeaderViewModel _selectedMainRow;

        public TblPostingProfileHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblPostingProfileDetailViewModel _selectedDetailRow;

        public TblPostingProfileDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblPostingProfileDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPostingProfileDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPostingProfileDetailViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblPostingProfileHeaderViewModel : GenericViewModel
    {
        private SortableCollectionView<TblPostingProfileDetailViewModel> _detailsList;

        public SortableCollectionView<TblPostingProfileDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new SortableCollectionView<TblPostingProfileDetailViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }
    }

    public class TblPostingProfileDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private int? _entityAccountField;
        private GenericTable _journalAccountTypePerRow;

        private int? _tblJournalAccountTypeField;
        private int _tblPostingProfileHeader;
        private Entity _tblEntityPerRow;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    if (JournalAccountTypePerRow != null) Type = JournalAccountTypePerRow.Iserial;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                }
            }
        }

        public Entity EntityAccountPerRow
        {
            get { return _tblEntityPerRow; }
            set
            {
                if ((ReferenceEquals(_tblEntityPerRow, value) != true))
                {
                    _tblEntityPerRow = value;
                    RaisePropertyChanged("EntityAccountPerRow");
                    if (EntityAccountPerRow != null) Entity = EntityAccountPerRow.Iserial;
                }
            }
        }

        public int TblPostingProfileHeader
        {
            get { return _tblPostingProfileHeader; }
            set
            {
                _tblPostingProfileHeader = value;
                RaisePropertyChanged("TblPostingProfileHeader");
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? Type
        {
            get { return _tblJournalAccountTypeField; }
            set
            {
                if ((_tblJournalAccountTypeField.Equals(value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEntity")]
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

        private int? _tblAccountField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAccount")]
        public int? TblAccount
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
                    TblAccount = _accountPerRow.Iserial;
                }
            }
        }

        private int _scope;

        public int Scope
        {
            get { return _scope; }
            set
            {
                _scope = value; RaisePropertyChanged("Scope");
                ScopePerRow = new GenericTable
                {
                    Iserial = Scope,
                    Aname = "",
                    Code = "",
                    Ename = ""
                };
            }
        }

        private GenericTable _scopePerRow;

        public GenericTable ScopePerRow
        {
            get { return _scopePerRow; }
            set { _scopePerRow = value; RaisePropertyChanged("ScopePerRow"); }
        }

    }

    #endregion ViewModels
}