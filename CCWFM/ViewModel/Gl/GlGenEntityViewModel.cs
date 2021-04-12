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
    public class GlGenEntityViewModel : ViewModelBase
    {
        public GlGenEntityViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.GlGenEntity.ToString());
                Glclient = new GlServiceClient();

                Glclient.GetTblJournalAccountTypeCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                Glclient.GetTblJournalAccountTypeAsync(true, LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblGlGenEntityGroupViewModel>();
                SelectedMainRow = new TblGlGenEntityGroupViewModel();

                Glclient.GetTblGlGenEntityGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlGenEntityGroupViewModel();
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

                Glclient.UpdateOrInsertTblGlGenEntityGroupCompleted += (s, ev) =>
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
                Glclient.DeleteTblGlGenEntityGroupCompleted += (s, ev) =>
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

                Glclient.GetTblGlGenEntityCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlGenEntityViewModel();

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

                Glclient.UpdateOrInsertTblGlGenEntityCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedMainRow.DetailsList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                };
                Glclient.DeleteTblGlGenEntityCompleted += (s, ev) =>
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
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblGlGenEntityGroupAsync(MainRowList.Count, PageSize, MainJournalAccoutType.Iserial, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblGlGenEntityGroupViewModel();
            newrow.TblJournalAccountType = MainJournalAccoutType.Iserial;
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (!Loading)
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
                        var saveRow = new TblGlGenEntityGroup();
                        saveRow.InjectFrom(SelectedMainRow);
                        Loading = true;
                        Glclient.UpdateOrInsertTblGlGenEntityGroupAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblGlGenEntityGroupViewModel oldRow)
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
                    var saveRow = new TblGlGenEntityGroup();
                    saveRow.InjectFrom(oldRow);

                    Glclient.UpdateOrInsertTblGlGenEntityGroupAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblGlGenEntityGroupAsync((TblGlGenEntityGroup)new TblGlGenEntityGroup().InjectFrom(row), 0, LoggedUserInfo.DatabasEname);
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
                Glclient.GetTblGlGenEntityAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial, MainJournalAccoutType.Iserial,
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
                            Glclient.DeleteTblGlGenEntityAsync(
                                (TblGlGenEntity)new TblGlGenEntity().InjectFrom(row), 1,
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

            var newrow = new TblGlGenEntityViewModel
            {
                TblGlGenEntityGroup = SelectedMainRow.Iserial,
                TblJournalAccountType = MainJournalAccoutType.Iserial
            };
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
                    if (SelectedDetailRow.TblGlGenEntityGroup == 0)
                    {
                        SelectedDetailRow.TblGlGenEntityGroup = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblGlGenEntity();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertTblGlGenEntityAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldDetailRow(TblGlGenEntityViewModel oldRow)
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
                    var saveRow = new TblGlGenEntity();
                    saveRow.InjectFrom(oldRow);

                    if (SelectedMainRow != null && SelectedMainRow.DetailsList != null)
                        Glclient.UpdateOrInsertTblGlGenEntityAsync(saveRow, save, SelectedMainRow.DetailsList.IndexOf(oldRow),
                            LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop

        private ObservableCollection<TblJournalAccountType> _journalAccountTypeList;

        public ObservableCollection<TblJournalAccountType> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set
            {
                _journalAccountTypeList = value;
                RaisePropertyChanged("JournalAccountTypeList");
            }
        }

        private SortableCollectionView<TblGlGenEntityGroupViewModel> _mainRowList;

        public SortableCollectionView<TblGlGenEntityGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblGlGenEntityGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblGlGenEntityGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlGenEntityGroupViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblGlGenEntityGroupViewModel _selectedMainRow;

        public TblGlGenEntityGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblGlGenEntityViewModel _selectedDetailRow;

        public TblGlGenEntityViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblGlGenEntityViewModel> _selectedDetailRows;

        public ObservableCollection<TblGlGenEntityViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblGlGenEntityViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private TblJournalAccountType _mainJournalAccoutType;

        public TblJournalAccountType MainJournalAccoutType
        {
            get { return _mainJournalAccoutType; }
            set
            {
                _mainJournalAccoutType = value;
                RaisePropertyChanged("MainJournalAccoutType");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblGlGenEntityGroupViewModel : GenericViewModel
    {
        private SortableCollectionView<TblGlGenEntityViewModel> _detailLst;

        public SortableCollectionView<TblGlGenEntityViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new SortableCollectionView<TblGlGenEntityViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private int? _tblJournalAccountType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountType; }
            set { _tblJournalAccountType = value; RaisePropertyChanged("TblJournalAccountType"); }
        }
    }

    public class TblGlGenEntityViewModel : GenericViewModel
    {
        private int _tblGlGenEntityGroup;

        public int TblGlGenEntityGroup
        {
            get { return _tblGlGenEntityGroup; }
            set { _tblGlGenEntityGroup = value; RaisePropertyChanged("TblGlGenEntityGroup"); }
        }

        private int? _tblJournalAccountType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountType; }
            set { _tblJournalAccountType = value; RaisePropertyChanged("TblJournalAccountType"); }
        }
    }

    #endregion ViewModels
}