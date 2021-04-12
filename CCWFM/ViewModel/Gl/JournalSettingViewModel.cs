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
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.Gl
{
    public class JournalSettingViewModel : ViewModelBase
    {
        public JournalSettingViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.JournalSetting.ToString());
                Glclient = new GlServiceClient();

                MainRowList = new ObservableCollection<TblJournalSettingViewModel>();
                SelectedMainRow = new TblJournalSettingViewModel();

                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);


                Glclient.GetTblJournalSettingCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblJournalSettingViewModel();
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
                    if (Export)
                    {
                        Export = false;
                        AllowExport = true;
                        //ExportGrid.ExportExcel("Account");
                    }
                };

                Glclient.UpdateOrInsertTblJournalSettingsCompleted += (s, ev) =>
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
                Glclient.DeleteTblJournalSettingCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    foreach (var item in ev.Result.ToList())
                    {
                        var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == item);
                        if (oldrow != null) MainRowList.Remove(oldrow);
                    }

                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.GetTblJournalSettingEntityCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblJournalSettingEntityViewModel();
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);

                        newrow.JournalPerRow = new TblJournal();
                        newrow.JournalPerRow.InjectFrom(row.TblJournal1);
                        newrow.EntityPerRow =
                      sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                               x.Iserial == row.EntityAccount && x.scope==row.Scope);
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }

                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count(x => x.Iserial >= 0) == 0)
                    {
                        AddNewDetailRow(false);
                    }

                    if (Export)
                    {
                        AllowExport = true;
                        Export = false;
                    }
                };

                Glclient.UpdateOrInsertTblJournalSettingEntitysCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;

                };

                Glclient.DeleteTblJournalSettingEntityCompleted += (s, ev) =>
                {
                    Loading = false;

                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    foreach (var item in ev.Result.ToList())
                    {
                        var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == item);
                        if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    }
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                var glRuleTypeClient = new GlServiceClient();
                glRuleTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                glRuleTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                Glclient.GetTblJournalSettingCostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblJournalSettingCostCenterViewModel();
                        if (row.TblCostCenter1 != null)
                        {
                            newrow.CostCenterPerRow = new TblCostCenter();
                            newrow.CostCenterPerRow.InjectFrom(row.TblCostCenter1);

                            if (row.TblCostCenter1.TblCostCenterType1 != null)
                            {
                                newrow.CostCenterTypePerRow.InjectFrom(row.TblCostCenter1.TblCostCenterType1);
                            }
                        }


                        newrow.InjectFrom(row);

                        SelectedMainRow.SubDetailList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;

                    if (SelectedMainRow.SubDetailList.Any() &&
                        (SelectedSubDetailRow == null))
                    {
                        SelectedSubDetailRow = SelectedMainRow.SubDetailList.FirstOrDefault();
                    }

                    if (DetailSubFullCount == 0 && SelectedMainRow.SubDetailList.Count(x => x.Iserial >= 0) == 0)
                    {
                        AddNewSubDetailRow(false);
                    }
                    if (ExportGrid != null)
                    {
                        ExportGrid.ScrollIntoView(SelectedSubDetailRow, ExportGrid.Columns[1]);
                        ExportGrid.CurrentColumn = ExportGrid.Columns[1];
                        ExportGrid.Focus();
                    }

                };

                Glclient.UpdateOrInsertTblJournalSettingCostCentersCompleted += (s, x) =>
                 {

                     var savedRow = SelectedMainRow.SubDetailList.ElementAt(x.outindex);
                     if (savedRow != null) savedRow.InjectFrom(x.Result);
                     Loading = false;
                 };

                Glclient.DeleteTblJournalSettingCostCenterCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    foreach (var item in ev.Result.ToList())
                    {
                        var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == item);
                        if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    }
                    if (!SelectedMainRow.SubDetailList.Any())
                    {
                        AddNewSubDetailRow(false);
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
            Glclient.GetTblJournalSettingAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetTblJournalSettingAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblJournalSettingViewModel();
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
                    var saveRow = new TblJournalSetting();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblJournalSettingsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblJournalSettingViewModel oldRow)
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
                    var saveRow = new TblJournalSetting();
                    saveRow.InjectFrom(oldRow);
                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblJournalSettingsAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                    var ListInt = SelectedMainRows.Select(w => w.Iserial).ToList();
                    var ObsInt = new ObservableCollection<int>();
                    foreach (var item in ListInt)
                    {
                        ObsInt.Add(item);
                    }
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;

                    }
                    Glclient.DeleteTblJournalSettingAsync(ObsInt, LoggedUserInfo.DatabasEname);
                    foreach (var row in SelectedMainRows.Where(w => w.Iserial == 0).ToList())
                    {
                        MainRowList.Remove(row);
                    }
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
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
                Glclient.GetTblJournalSettingEntityAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetFullDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            Export = true;
            if (SelectedMainRow != null)
                Glclient.GetTblJournalSettingEntityAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
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
                    var ListInt = SelectedDetailRows.Select(w => w.Iserial).ToList();
                    var ObsInt = new ObservableCollection<int>();
                    foreach (var item in ListInt)
                    {
                        ObsInt.Add(item);
                    }
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;

                    }
                    Glclient.DeleteTblJournalSettingEntityAsync(ObsInt, LoggedUserInfo.DatabasEname);
                    foreach (var row in SelectedDetailRows.Where(w => w.Iserial == 0).ToList())
                    {
                        SelectedMainRow.DetailsList.Remove(row);
                    }
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (SelectedDetailRow == null)
            {
                currentRowIndex = 0;
            }
            if (checkLastRow)
            {
                var lastrow = SelectedMainRow.DetailsList.ElementAtOrDefault(currentRowIndex);
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(lastrow,
                    new ValidationContext(lastrow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowUpdate != true)
            {

                return;
            }


            var newrow = new TblJournalSettingEntityViewModel
            {

                TblJournalSetting = SelectedMainRow.Iserial,

            };

            if (SelectedMainRow.DetailsList.Any(x => x.Iserial > -1))
            {
                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            }
            else
            {
                if (currentRowIndex == -1)
                {
                    currentRowIndex = 0;
                }
                SelectedMainRow.DetailsList.Insert(currentRowIndex, newrow);
            }

            SelectedDetailRow = newrow;
            if (ExportGrid != null)
            {
                ExportGrid.BeginEdit();
            }
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
                    if (SelectedDetailRow.TblJournalSetting == 0)
                    {
                        SelectedDetailRow.TblJournalSetting = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true)
                    {

                        return;
                    }
                    var rowToSave = new TblJournalSettingEntity();
                    rowToSave.InjectFrom(SelectedDetailRow);


                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblJournalSettingEntitysAsync(rowToSave, save,
                            SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname,
                            LoggedUserInfo.Iserial);
                    }
                }
            }
        }

        public void SaveOldDetailRow(TblJournalSettingEntityViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (oldRow.TblJournalSetting == 0)
                    {
                        oldRow.TblJournalSetting = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true)
                    {

                        return;
                    }
                    var rowToSave = new TblJournalSettingEntity();
                    rowToSave.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        if (SelectedMainRow != null)
                        {
                            if (SelectedMainRow.DetailsList.IndexOf(oldRow) != -1)
                            {
                                Loading = true;
                                Glclient.UpdateOrInsertTblJournalSettingEntitysAsync(rowToSave, save,
                                SelectedMainRow.DetailsList.IndexOf(oldRow), LoggedUserInfo.DatabasEname,
                                LoggedUserInfo.Iserial);
                            }
                        }

                    }
                }
            }
        }


        public void GetSubDetailData()
        {
            if (DetailSubSortBy == null)
                DetailSubSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblJournalSettingCostCenterAsync(SelectedMainRow.SubDetailList.Count, int.MaxValue, SelectedMainRow.Iserial,
                    DetailSubSortBy, DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteSubDetailRow()
        {
            if (SelectedSubDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
      MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    var ListInt = SelectedSubDetailRows.Select(w => w.Iserial).ToList();
                    var ObsInt = new ObservableCollection<int>();
                    foreach (var item in ListInt)
                    {
                        ObsInt.Add(item);
                    }
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;

                    }
                    Glclient.DeleteTblJournalSettingCostCenterAsync(ObsInt, LoggedUserInfo.DatabasEname);
                    foreach (var row in SelectedSubDetailRows.Where(w => w.Iserial == 0).ToList())
                    {
                        SelectedMainRow.SubDetailList.Remove(row);
                    }
                    if (!SelectedMainRow.SubDetailList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                }
            }
        }

        public void AddNewSubDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.SubDetailList.IndexOf(SelectedSubDetailRow));
            if (checkLastRow)
            {
                var lastrow = SelectedMainRow.SubDetailList.ElementAtOrDefault(currentRowIndex);
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(lastrow,
                    new ValidationContext(lastrow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            var newrow = new TblJournalSettingCostCenterViewModel
            {
                TblJournalSetting = SelectedMainRow.Iserial,
            };

            SelectedMainRow.SubDetailList.Insert(currentRowIndex + 1, newrow);
            SelectedSubDetailRow = newrow;
            if (ExportGrid != null)
            {
                ExportGrid.BeginEdit();
            }
        }

        public void SaveSubDetailRow()
        {
            if (SelectedSubDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSubDetailRow,
                    new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedSubDetailRow.Iserial == 0;
                    if (SelectedSubDetailRow.TblJournalSetting == 0)
                    {
                        SelectedSubDetailRow.TblJournalSetting = SelectedDetailRow.Iserial;
                    }


                    if (AllowUpdate != true)
                    {

                        return;
                    }


                    var rowToSave = new TblJournalSettingCostCenter();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblJournalSettingCostCentersAsync(rowToSave, save,
                            SelectedMainRow.SubDetailList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }



        #region Prop   

        private ObservableCollection<TblJournalSettingViewModel> _mainRowList;

        public ObservableCollection<TblJournalSettingViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblJournalSettingViewModel> _selectedMainRows;

        public ObservableCollection<TblJournalSettingViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblJournalSettingViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private TblJournalSettingViewModel _selectedMainRow;

        public TblJournalSettingViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblJournalSettingEntityViewModel _selectedDetailRow;

        public TblJournalSettingEntityViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblJournalSettingEntityViewModel> _selectedDetailRows;

        public ObservableCollection<TblJournalSettingEntityViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblJournalSettingEntityViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private ObservableCollection<TblJournalSettingCostCenterViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblJournalSettingCostCenterViewModel> SelectedSubDetailRows
        {
            get { return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblJournalSettingCostCenterViewModel>()); }
            set
            {
                _selectedSubDetailRows = value;
                RaisePropertyChanged("SelectedSubDetailRows");
            }
        }

        private TblJournalSettingCostCenterViewModel _selectedSubDetailRow;

        public TblJournalSettingCostCenterViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set
            {
                _selectedSubDetailRow = value;
                RaisePropertyChanged("SelectedSubDetailRow");
            }
        }
        private ObservableCollection<GenericTable> _costCenterTypeList;

        public ObservableCollection<GenericTable> CostCenterTypeList
        {
            get { return _costCenterTypeList; }
            set
            {
                _costCenterTypeList = value;
                RaisePropertyChanged("CostCenterTypeList");
            }
        }
        #endregion Prop
    }

    #region ViewModels

    public class TblJournalSettingViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblJournalSettingCostCenterViewModel> _SubdetailsList;

        public ObservableCollection<TblJournalSettingCostCenterViewModel> SubDetailList
        {
            get { return _SubdetailsList ?? (_SubdetailsList = new ObservableCollection<TblJournalSettingCostCenterViewModel>()); }
            set
            {
                _SubdetailsList = value;
                RaisePropertyChanged("SubDetailList");
            }
        }

        private ObservableCollection<TblJournalSettingEntityViewModel> _detailsList;
        public ObservableCollection<TblJournalSettingEntityViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblJournalSettingEntityViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; RaisePropertyChanged("Description"); }
        }

        private int _iserialField;

        private string _aname;

        private string _ename;

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get { return _aname; }
            set
            {
                if ((ReferenceEquals(_aname, value) != true))
                {
                    if (value != null) _aname = value.Trim();
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get { return _ename; }
            set
            {
                if ((ReferenceEquals(_ename, value) != true))
                {
                    if (value != null) _ename = value.Trim();
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
                }
            }
        }
    }

    public class TblJournalSettingEntityViewModel : PropertiesViewModelBase
    {

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
        private bool? CrField;

        private bool DrField;

        private int? EntityAccountField;

        private int IserialField;

        private int? TblJournalField;

        private TblJournal _journalPerRow;

        private int? TblJournalAccountTypeField;

        private GenericTable _journalAccountTypePerRow;


        private int? TblJournalSettingField;


        public bool? Cr
        {
            get
            {
                return this.CrField;
            }
            set
            {
                if ((this.CrField.Equals(value) != true))
                {
                    this.CrField = value;
                    this.RaisePropertyChanged("Cr");
                }
            }
        }
        public bool Dr
        {
            get
            {
                return this.DrField;
            }
            set
            {
                if ((this.DrField.Equals(value) != true))
                {
                    this.DrField = value;
                    this.RaisePropertyChanged("Dr");
                }
            }
        }
        [Required]
        public int? EntityAccount
        {
            get
            {
                return this.EntityAccountField;
            }
            set
            {
                if ((this.EntityAccountField.Equals(value) != true))
                {
                    this.EntityAccountField = value;
                    this.RaisePropertyChanged("EntityAccount");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }
        [Required]
        public int? TblJournal
        {
            get
            {
                return this.TblJournalField;
            }
            set
            {
                if ((this.TblJournalField.Equals(value) != true))
                {
                    this.TblJournalField = value;
                    this.RaisePropertyChanged("TblJournal");
                }
            }
        }

        public TblJournal JournalPerRow
        {
            get { return _journalPerRow; }
            set
            {
                _journalPerRow = value;
                RaisePropertyChanged("JournalPerRow");

                if (JournalPerRow != null)
                {
                    TblJournal = _journalPerRow.Iserial;

                }
            }
        }
        [Required]
        public int? TblJournalAccountType
        {
            get
            {
                return this.TblJournalAccountTypeField;
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


        public int? TblJournalSetting
        {
            get
            {
                return this.TblJournalSettingField;
            }
            set
            {
                if ((this.TblJournalSettingField.Equals(value) != true))
                {
                    this.TblJournalSettingField = value;
                    this.RaisePropertyChanged("TblJournalSetting");
                }
            }
        }

    }

    public class TblJournalSettingCostCenterViewModel : PropertiesViewModelBase
    {
        private GenericTable _costCenterTypePerRow;

        public GenericTable CostCenterTypePerRow
        {
            get { return _costCenterTypePerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterTypePerRow, value) != true))
                {
                    _costCenterTypePerRow = value;
                    RaisePropertyChanged("CostCenterTypePerRow");
                    if (CostCenterTypePerRow != null) TblCostCenterType = CostCenterTypePerRow.Iserial;
                }
            }
        }

        private int? _tblCostCenterType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenterType")]
        public int? TblCostCenterType
        {
            get { return _tblCostCenterType; }
            set { _tblCostCenterType = value; RaisePropertyChanged("TblCostCenterType"); }
        }


        private int IserialField;

        private int? TblCostCenterField;

        private int? TblJournalSettingField;

        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }
        [Required]
        public int? TblCostCenter
        {
            get
            {
                return this.TblCostCenterField;
            }
            set
            {
                if ((this.TblCostCenterField.Equals(value) != true))
                {
                    this.TblCostCenterField = value;
                    this.RaisePropertyChanged("TblCostCenter");
                }
            }
        }

        private TblCostCenter _costCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _costCenterPerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterPerRow, value) != true))
                {
                    _costCenterPerRow = value;
                    RaisePropertyChanged("CostCenterPerRow");
                    if (_costCenterPerRow != null)
                    {
                        TblCostCenter = _costCenterPerRow.Iserial;
                    }
                }
            }
        }

        [Required]
        public int? TblJournalSetting
        {
            get
            {
                return this.TblJournalSettingField;
            }
            set
            {
                if ((this.TblJournalSettingField.Equals(value) != true))
                {
                    this.TblJournalSettingField = value;
                    this.RaisePropertyChanged("TblJournalSetting");
                }
            }
        }
    }
    #endregion ViewModels
}