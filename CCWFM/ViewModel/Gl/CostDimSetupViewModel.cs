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
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class CostDimSetupViewModel : ViewModelBase
    {
        public CostDimSetupViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.CostDimSetup.ToString());
                Glclient = new GlServiceClient();
                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) => { JournalAccountTypeList = sv.Result; };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var costDimSetupTypeClient = new GlServiceClient();
                costDimSetupTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                costDimSetupTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                var tblCostCenterOptionClient = new GlServiceClient();
                tblCostCenterOptionClient.GetGenericCompleted += (s, sv) => { CostCenterOptionList = sv.Result; };
                tblCostCenterOptionClient.GetGenericAsync("tblCostCenterOption", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblCostDimSetupHeaderViewModel>();
                SelectedMainRow = new TblCostDimSetupHeaderViewModel();

                Glclient.GetTblCostDimSetupHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostDimSetupHeaderViewModel();
                        newrow.InjectFrom(row);
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
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
                        ExportGrid.ExportExcel("CostDimSetup");
                    }
                };

                Glclient.UpdateOrInsertTblCostDimSetupHeaderCompleted += (s, ev) =>
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
                Glclient.DeleteTblCostDimSetupHeaderCompleted += (s, ev) =>
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

                Glclient.GetTblCostDimSetupDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostDimSetupDetailViewModel();
                        newrow.CostCenterOptionPerRow = new GenericTable();
                        newrow.CostCenterOptionPerRow.InjectFrom(row.TblCostCenterOption1);
                        newrow.CostCenterTypePerRow = new GenericTable();
                        newrow.CostCenterTypePerRow.InjectFrom(row.TblCostCenterType1);

                        newrow.InjectFrom(row);

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
                };

                Glclient.UpdateOrInsertTblCostDimSetupDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblCostDimSetupDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblCostDimSetupDetailCompleted += (s, ev) =>
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
            Glclient.GetTblCostDimSetupHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblCostDimSetupHeaderViewModel();
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
                    var saveRow = new TblCostDimSetupHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblCostDimSetupHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
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
                            Glclient.DeleteTblCostDimSetupHeaderAsync((TblCostDimSetupHeader)new TblCostDimSetupHeader().InjectFrom(row), LoggedUserInfo.DatabasEname);
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
                Glclient.GetTblCostDimSetupDetailAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
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
                            Glclient.DeleteTblCostDimSetupDetailAsync(
                                (TblCostDimSetupDetail)new TblCostDimSetupDetail().InjectFrom(row),
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

            var newrow = new TblCostDimSetupDetailViewModel { TblCostDimSetupHeader = SelectedMainRow.Iserial };

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
                    if (SelectedDetailRow.TblCostDimSetupHeader == 0)
                    {
                        SelectedDetailRow.TblCostDimSetupHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblCostDimSetupDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertTblCostDimSetupDetailAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
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

        private SortableCollectionView<TblCostDimSetupHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblCostDimSetupHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblCostDimSetupHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblCostDimSetupHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostDimSetupHeaderViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblCostDimSetupHeaderViewModel _selectedMainRow;

        public TblCostDimSetupHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblCostDimSetupDetailViewModel _selectedDetailRow;

        public TblCostDimSetupDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblCostDimSetupDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblCostDimSetupDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblCostDimSetupDetailViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblCostDimSetupHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private SortableCollectionView<TblCostDimSetupDetailViewModel> _detailLst;

        public SortableCollectionView<TblCostDimSetupDetailViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new SortableCollectionView<TblCostDimSetupDetailViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private int? _tblJournalAccountTypeField;

        private GenericTable _tblJournalAccountType;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _tblJournalAccountType; }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountType, value) != true))
                {
                    _tblJournalAccountType = value;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                    if (JournalAccountTypePerRow != null) TblJournalAccountType = JournalAccountTypePerRow.Iserial;
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountTypeField; }
            set
            {
                if ((_tblJournalAccountTypeField.Equals(value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }
    }

    public class TblCostDimSetupDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _tblCostDimHeader;

        public int TblCostDimSetupHeader
        {
            get { return _tblCostDimHeader; }
            set { _tblCostDimHeader = value; RaisePropertyChanged("TblCostDimSetupHeader"); }
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