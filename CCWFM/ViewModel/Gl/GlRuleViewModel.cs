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
    public class GlRuleViewModel : ViewModelBase
    {
        public GlRuleViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.GlRule.ToString());
                Glclient = new GlServiceClient();

                Glclient.GetGlInventoryGroupAsync(LoggedUserInfo.DatabasEname);

                Glclient.GetGlInventoryGroupCompleted += (s, sv) =>
                {
                    GlInventoryGroupList = sv.Result;
                };
                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                    JournalAccountTypeListFilter = new ObservableCollection<GenericTable>(sv.Result.Where(x => x.Iserial == 0 || x.Iserial == 8));
                    var newrow = JournalAccountTypeListFilter.FirstOrDefault(x => x.Iserial == 0);
                    newrow.Code = "Group";
                    newrow.Ename = "Group";
                    newrow.Aname = "Group";
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var glRuleTypeClient = new GlServiceClient();
                glRuleTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                glRuleTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                var tblCostCenterOptionClient = new GlServiceClient();
                tblCostCenterOptionClient.GetGenericCompleted += (s, sv) => { CostCenterOptionList = sv.Result; };
                tblCostCenterOptionClient.GetGenericAsync("tblCostCenterOption", "%%", "%%", "%%", "Iserial", "ASC",

                    LoggedUserInfo.DatabasEname);

                var tblCostAllocationMethodClient = new GlServiceClient();
                tblCostAllocationMethodClient.GetGenericCompleted += (s, sv) => { CostAllocationMethodList = sv.Result; };
                tblCostAllocationMethodClient.GetGenericAsync("TblCostAllocationMethod", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblGlRuleHeaderViewModel>();
                SelectedMainRow = new TblGlRuleHeaderViewModel();

                Glclient.GetTblGlRuleHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlRuleHeaderViewModel();
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

                Glclient.UpdateOrInsertTblGlRuleHeadersCompleted += (s, ev) =>
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
                Glclient.DeleteTblGlRuleHeaderCompleted += (s, ev) =>
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

                Glclient.GetTblGlRuleMainDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlRuleMainDetailViewModel();

                        newrow.EntityAccountPerRow = new Entity();

                        newrow.EntityAccountPerRow =
                            sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                              x.Iserial == row.EntityAccount);

                        if (row.TblJournalAccountType1 != null)
                        {
                            newrow.JournalAccountTypePerRow = new GenericTable();
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        }
                        if (row.TblCostCenterType1 != null)
                        {
                            newrow.CostCenterTypePerRow = new GenericTable();
                            newrow.CostCenterTypePerRow.InjectFrom(row.TblCostCenterType1);
                        }
                        if (row.TblCostCenterOption1 != null)
                        {
                            newrow.CostCenterOptionPerRow = new GenericTable();
                            newrow.CostCenterOptionPerRow.InjectFrom(row.TblCostCenterOption1);
                        }
                        if (row.TblCostAllocationMethod1 != null)
                        {
                            newrow.CostAllocationMethodPerRow = new GenericTable();
                            newrow.CostAllocationMethodPerRow.InjectFrom(row.TblCostAllocationMethod1);
                        }
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblGlRuleMainDetailsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedDetailRow.DetailsList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                };
                Glclient.DeleteTblGlRuleMainDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.GetTblGlRuleDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlRuleDetailViewModel
                        {
                            CostCenterPerRow = row.TblCostCenter1,
                            TblCostCenterType = SelectedDetailRow.TblCostCenterType,
                            CostCenterTypePerRow = SelectedDetailRow.CostCenterTypePerRow,
                            JournalAccountTypePerRow = JournalAccountTypeListFilter.FirstOrDefault(x => x.Iserial == row.TblJournalAccountType)
                        };

                        if (row.TblJournalAccountType != null)
                        {
                            newrow.GlInventoryGroupPerRow =
                                GlInventoryGroupList.FirstOrDefault(
                                    x =>
                                        x.TblJournalAccountType == row.TblJournalAccountType &&
                                        x.Iserial == row.GlInventoryGroup);
                        }

                        newrow.InjectFrom(row);

                        SelectedDetailRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;

                    if (SelectedDetailRow.DetailsList.Any() &&
                        (SelectedSubDetailRow == null))
                    {
                        SelectedSubDetailRow = SelectedDetailRow.DetailsList.FirstOrDefault();
                    }

                    if (DetailSubFullCount == 0 && SelectedDetailRow.DetailsList.Count == 0)
                    {
                        AddNewSubDetailRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblGlRuleDetailsCompleted += (s, x) =>
                {
                    var savedRow = (TblGlRuleDetailViewModel)SelectedDetailRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblGlRuleDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    Loading = false;
                    var oldrow = SelectedDetailRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.DetailsList.Remove(oldrow);
                    if (!SelectedDetailRow.DetailsList.Any())
                    {
                        AddNewSubDetailRow(false);
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
            Glclient.GetTblGlRuleHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblGlRuleHeaderViewModel();
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
                    var saveRow = new TblGlRuleHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblGlRuleHeadersAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                        LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldRow(TblGlRuleHeaderViewModel oldRow)
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
                    var saveRow = new TblGlRuleHeader();
                    saveRow.InjectFrom(oldRow);

                    Glclient.UpdateOrInsertTblGlRuleHeadersAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblGlRuleHeaderAsync((TblGlRuleHeader)new TblGlRuleHeader().InjectFrom(row), 0, LoggedUserInfo.DatabasEname);
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
                Glclient.GetTblGlRuleMainDetailAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
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
                            Glclient.DeleteTblGlRuleMainDetailAsync(
                                (TblGlRuleMainDetail)new TblGlRuleMainDetail().InjectFrom(row), 1,
                                 LoggedUserInfo.DatabasEname);
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

            var newrow = new TblGlRuleMainDetailViewModel { TblGlRuleHeader = SelectedMainRow.Iserial };

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
                    if (SelectedDetailRow.TblGlRuleHeader == 0)
                    {
                        SelectedDetailRow.TblGlRuleHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblGlRuleMainDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertTblGlRuleMainDetailsAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldDetailRow(TblGlRuleMainDetailViewModel oldRow)
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
                    var saveRow = new TblGlRuleMainDetail();
                    saveRow.InjectFrom(oldRow);

                    if (SelectedMainRow != null && SelectedMainRow.DetailsList != null)
                        Glclient.UpdateOrInsertTblGlRuleMainDetailsAsync(saveRow, save, SelectedMainRow.DetailsList.IndexOf(oldRow),
                            LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void GetSubDetailData()
        {
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblGlRuleDetailAsync(SelectedDetailRow.DetailsList.Count, PageSize, SelectedDetailRow.Iserial,
                    "it.Iserial", DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteSubDetailRow()
        {
            if (SelectedSubDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedSubDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteTblGlRuleDetailAsync(
                                (TblGlRuleDetail)new TblGlRuleDetail().InjectFrom(row), 1,
                                 LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedDetailRow.DetailsList.Remove(SelectedSubDetailRow);
                            if (!SelectedDetailRow.DetailsList.Any())
                            {
                                AddNewSubDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewSubDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSubDetailRow,
                    new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

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

            var newrow = new TblGlRuleDetailViewModel
            {
                TblGlRuleMainDetail = SelectedDetailRow.Iserial,
                TblCostCenterType = SelectedDetailRow.TblCostCenterType,
                CostCenterTypePerRow = new GenericTable
                {
                    Iserial = (int)SelectedDetailRow.TblCostCenterType,
                }
            };

            SelectedDetailRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedSubDetailRow = newrow;
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
                    if (SelectedSubDetailRow.TblGlRuleMainDetail == 0)
                    {
                        SelectedSubDetailRow.TblGlRuleMainDetail = SelectedDetailRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblGlRuleDetail();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    if (Loading == false)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblGlRuleDetailsAsync(rowToSave, save,
                            SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldSubDetailRow(TblGlRuleDetailViewModel oldRow)
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
                    if (Loading == false)
                    {
                        Loading = true;
                        var saveRow = new TblGlRuleDetail();
                        saveRow.InjectFrom(oldRow);

                        Glclient.UpdateOrInsertTblGlRuleDetailsAsync(saveRow, save, SelectedDetailRow.DetailsList.IndexOf(oldRow),
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        #region Prop

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set
            {
                _journalAccountTypeList = value;
                RaisePropertyChanged("JournalAccountTypeList");
            }
        }

        private ObservableCollection<GenericTable> _journalAccountTypeListFilter;

        public ObservableCollection<GenericTable> JournalAccountTypeListFilter
        {
            get { return _journalAccountTypeListFilter; }
            set
            {
                _journalAccountTypeListFilter = value;
                RaisePropertyChanged("JournalAccountTypeListFilter");
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

        private ObservableCollection<GenericTable> _costCenterOptionList;

        public ObservableCollection<GenericTable> CostCenterOptionList
        {
            get { return _costCenterOptionList; }
            set { _costCenterOptionList = value; RaisePropertyChanged("CostCenterOptionList"); }
        }

        private ObservableCollection<GlInventoryGroup_Result> _glInventoryGroupList;

        public ObservableCollection<GlInventoryGroup_Result> GlInventoryGroupList
        {
            get { return _glInventoryGroupList; }
            set { _glInventoryGroupList = value; RaisePropertyChanged("GlInventoryGroupList"); }
        }

        private ObservableCollection<GlInventoryGroup_Result> _glInventoryGroupListFiltered;

        public ObservableCollection<GlInventoryGroup_Result> GlInventoryGroupListFiltered
        {
            get { return _glInventoryGroupListFiltered; }
            set { _glInventoryGroupListFiltered = value; RaisePropertyChanged("GlInventoryGroupListFiltered"); }
        }

        private ObservableCollection<GenericTable> _costAllocationMethodList;

        public ObservableCollection<GenericTable> CostAllocationMethodList
        {
            get { return _costAllocationMethodList; }
            set { _costAllocationMethodList = value; RaisePropertyChanged("CostAllocationMethodList"); }
        }

        private SortableCollectionView<TblGlRuleHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblGlRuleHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblGlRuleHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblGlRuleHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlRuleHeaderViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblGlRuleHeaderViewModel _selectedMainRow;

        public TblGlRuleHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblGlRuleMainDetailViewModel _selectedDetailRow;

        public TblGlRuleMainDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblGlRuleMainDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblGlRuleMainDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblGlRuleMainDetailViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private ObservableCollection<TblGlRuleDetailViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblGlRuleDetailViewModel> SelectedSubDetailRows
        {
            get { return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblGlRuleDetailViewModel>()); }
            set
            {
                _selectedSubDetailRows = value;
                RaisePropertyChanged("SelectedSubDetailRows");
            }
        }

        private TblGlRuleDetailViewModel _selectedSubDetailRow;

        public TblGlRuleDetailViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set
            {
                _selectedSubDetailRow = value;
                RaisePropertyChanged("SelectedSubDetailRow");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblGlRuleHeaderViewModel : GenericViewModel
    {
        private SortableCollectionView<TblGlRuleMainDetailViewModel> _detailLst;

        public SortableCollectionView<TblGlRuleMainDetailViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new SortableCollectionView<TblGlRuleMainDetailViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }
    }

    public class TblGlRuleMainDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private SortableCollectionView<TblGlRuleDetailViewModel> _detailLst;

        public SortableCollectionView<TblGlRuleDetailViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new SortableCollectionView<TblGlRuleDetailViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private int _tblGlRuleHeader;

        public int TblGlRuleHeader
        {
            get { return _tblGlRuleHeader; }
            set { _tblGlRuleHeader = value; RaisePropertyChanged("TblGlRuleHeader"); }
        }

        private int? _tblCostCenterOption;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenterOption")]
        public int? TblCostCenterOption
        {
            get { return _tblCostCenterOption; }
            set { _tblCostCenterOption = value; RaisePropertyChanged("TblCostCenterOption"); }
        }

        private GenericTable _costCenterOptionPerRow;

        public GenericTable CostCenterOptionPerRow
        {
            get { return _costCenterOptionPerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterOptionPerRow, value) != true))
                {
                    _costCenterOptionPerRow = value;
                    RaisePropertyChanged("CostCenterOptionPerRow");
                    if (CostCenterOptionPerRow != null) TblCostCenterOption = CostCenterOptionPerRow.Iserial;
                }
            }
        }

        private int? _tblJournalAccountType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountType; }
            set { _tblJournalAccountType = value; RaisePropertyChanged("TblJournalAccountType"); }
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
                    RaisePropertyChanged("JournalAccountTypePerRow");
                    if (JournalAccountTypePerRow != null) TblJournalAccountType = JournalAccountTypePerRow.Iserial;
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

        private int? _tblCostAllocationMethod;

        public int? TblCostAllocationMethod
        {
            get { return _tblCostAllocationMethod; }
            set { _tblCostAllocationMethod = value; RaisePropertyChanged("TblCostAllocationMethod"); }
        }

        private GenericTable _costAllocationMethodPerRow;

        public GenericTable CostAllocationMethodPerRow
        {
            get { return _costAllocationMethodPerRow; }
            set
            {
                if ((ReferenceEquals(_costAllocationMethodPerRow, value) != true))
                {
                    _costAllocationMethodPerRow = value;
                    RaisePropertyChanged("CostAllocationMethodPerRow");
                    if (CostAllocationMethodPerRow != null) TblCostAllocationMethod = CostAllocationMethodPerRow.Iserial;
                }
            }
        }

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

        private Entity _tblEntityPerRow;

        public Entity EntityAccountPerRow
        {
            get { return _tblEntityPerRow; }
            set
            {
                if ((ReferenceEquals(_tblEntityPerRow, value) != true))
                {
                    _tblEntityPerRow = value;
                    RaisePropertyChanged("EntityAccountPerRow");

                    if (EntityAccountPerRow != null) EntityAccount = EntityAccountPerRow.Iserial;
                }
            }
        }

        private int? _entityAccountField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAccount")]
        public int? EntityAccount
        {
            get { return _entityAccountField; }
            set
            {
                if ((_entityAccountField.Equals(value) != true))
                {
                    _entityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }
    }

    public class TblGlRuleDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private double _ratio;

        public double Ratio
        {
            get { return _ratio; }
            set { _ratio = value; RaisePropertyChanged("Ratio"); }
        }

        private int? _tblGlRuleMainDetail;

        public int? TblGlRuleMainDetail
        {
            get { return _tblGlRuleMainDetail; }
            set { _tblGlRuleMainDetail = value; RaisePropertyChanged("TblGlRuleMainDetail"); }
        }

        private int? _tblCostCenter;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenter")]
        public int? TblCostCenter
        {
            get { return _tblCostCenter; }
            set { _tblCostCenter = value; }
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

        private int? _tblCostCenterType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenterType")]
        public int? TblCostCenterType
        {
            get { return _tblCostCenterType; }
            set { _tblCostCenterType = value; RaisePropertyChanged("TblCostCenterType"); }
        }

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

        private GlInventoryGroup_Result _glInventoryGroupPerRow;

        public GlInventoryGroup_Result GlInventoryGroupPerRow
        {
            get { return _glInventoryGroupPerRow; }
            set
            {
                _glInventoryGroupPerRow = value;
                RaisePropertyChanged("GlInventoryGroupPerRow");

                if (GlInventoryGroupPerRow != null)
                {
                    if (GlInventoryGroupPerRow.Iserial != 0)
                    {
                        GlInventoryGroup = _glInventoryGroupPerRow.Iserial;
                        TblJournalAccountType =
                            _glInventoryGroupPerRow.TblJournalAccountType;
                    }
                }
            }
        }

        private int _glInventoryGroup;

        public int GlInventoryGroup
        {
            get { return _glInventoryGroup; }
            set { _glInventoryGroup = value; RaisePropertyChanged("GlInventoryGroup"); }
        }

        private int _tblJournalAccountType;

        public int TblJournalAccountType
        {
            get { return _tblJournalAccountType; }
            set { _tblJournalAccountType = value; RaisePropertyChanged("TblJournalAccountType"); }
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
    }

    #endregion ViewModels
}