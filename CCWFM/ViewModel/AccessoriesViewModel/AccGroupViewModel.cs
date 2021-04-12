using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.AccessoriesViewModel
{
    #region ViewModels

    public class TblLkpAccessoryGroupViewModel : GenericViewModel
    {
        private SortableCollectionView<TblAccessoriesSubGroupViewModel> _tblAccessoriesSubGroupField;

        private int _tblLkpItemGroupTypeField;

        private tbl_lkp_ItemGroupType _tblLkpItemGroupType1Field;

        public SortableCollectionView<TblAccessoriesSubGroupViewModel> DetailsList
        {
            get
            {
                return _tblAccessoriesSubGroupField ?? (_tblAccessoriesSubGroupField = new SortableCollectionView<TblAccessoriesSubGroupViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblAccessoriesSubGroupField, value) != true))
                {
                    _tblAccessoriesSubGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        public int tbl_lkp_ItemGroupType
        {
            get
            {
                return _tblLkpItemGroupTypeField;
            }
            set
            {
                if ((_tblLkpItemGroupTypeField.Equals(value) != true))
                {
                    _tblLkpItemGroupTypeField = value;
                    RaisePropertyChanged("tbl_lkp_ItemGroupType");
                }
            }
        }

        public tbl_lkp_ItemGroupType ItemGroupTypePerRow
        {
            get
            {
                return _tblLkpItemGroupType1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblLkpItemGroupType1Field, value) != true))
                {
                    _tblLkpItemGroupType1Field = value;
                    RaisePropertyChanged("tbl_lkp_ItemGroupType1");
                }
            }
        }
    }

    public class TblAccessoriesSubGroupViewModel : GenericViewModel
    {
        private int _groupIdField;

        private string _subCodeField;

        private ObservableCollection<TblAccessorySizeViewModel> _tblAccessorySizesField;

        private GenericTable _accSizeGroupPerRow;

        public GenericTable AccSizeGroupPerRow
        {
            get { return _accSizeGroupPerRow; }
            set { _accSizeGroupPerRow = value; RaisePropertyChanged("AccSizeGroupPerRow"); }
        }

        private int? _tblAccSizeGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSizeGroup")]
        public int? TblAccSizeGroup
        {
            get
            {
                return _tblAccSizeGroup;
            }
            set
            {
                if ((_tblAccSizeGroup.Equals(value) != true))
                {
                    _tblAccSizeGroup = value;
                    RaisePropertyChanged("TblAccSizeGroup");
                }
            }
        }

        public int GroupID
        {
            get
            {
                return _groupIdField;
            }
            set
            {
                if ((_groupIdField.Equals(value) != true))
                {
                    _groupIdField = value;
                    RaisePropertyChanged("GroupID");
                }
            }
        }

        public string SubCode
        {
            get
            {
                return _subCodeField;
            }
            set
            {
                if ((ReferenceEquals(_subCodeField, value) != true))
                {
                    _subCodeField = value;
                    RaisePropertyChanged("SubCode");
                }
            }
        }

        public ObservableCollection<TblAccessorySizeViewModel> DetailsList
        {
            get
            {
                return _tblAccessorySizesField ?? (_tblAccessorySizesField = new SortableCollectionView<TblAccessorySizeViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblAccessorySizesField, value) != true))
                {
                    _tblAccessorySizesField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }
    }

    public class TblAccessorySizeViewModel : ViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        private int _accessorySubGroupIdField;

        private int _codeField;

        private TblAccSize _sizeperrowSize;

        public TblAccSize SizePerRow
        {
            get { return _sizeperrowSize ?? (_sizeperrowSize = new TblAccSize()); }
            set { _sizeperrowSize = value; RaisePropertyChanged("SizePerRow"); }
        }

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    _client.UpdateOrDeletedAccSubGroupSizeLinkAsync((TblAccSubGroupSizeLink)new TblAccSubGroupSizeLink().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        public int tbl_AccessoriesSubGroup
        {
            get
            {
                return _accessorySubGroupIdField;
            }
            set
            {
                if ((_accessorySubGroupIdField.Equals(value) != true))
                {
                    _accessorySubGroupIdField = value;
                    RaisePropertyChanged("tbl_AccessoriesSubGroup");
                }
            }
        }

        public int TblAccSize
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((Equals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("TblAccSize");
                }
            }
        }
    }

    #endregion ViewModels

    public class AccGroupViewModel : ViewModelBase
    {
        public AccGroupViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetAccItemGroupTypesCompleted += (s, sv) =>
                {
                    TblItemGroupTypeList = sv.Result;
                };
                Client.GetAccItemGroupTypesAsync();

                MainRowList = new SortableCollectionView<TblLkpAccessoryGroupViewModel>();
                SelectedMainRow = new TblLkpAccessoryGroupViewModel();
                SelectedDetailRow = new TblAccessoriesSubGroupViewModel();
                SelectedDetailSubRow = new TblAccessorySizeViewModel();

                var client = new CRUD_ManagerServiceClient();

                client.GetGenericAsync("TblAccSizeGroup", "%%", "%%", "%%", "Iserial", "ASC");

                client.GetGenericCompleted += (s, ev) =>
                {
                    AccSizeGroupList = ev.Result;
                };

                Client.GetTblAccessoriesGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLkpAccessoryGroupViewModel();
                        newrow.InjectFrom(row);
                        newrow.ItemGroupTypePerRow =
                        TblItemGroupTypeList.FirstOrDefault(x => x.Iserial == newrow.tbl_lkp_ItemGroupType);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblAccessoriesSubGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccessoriesSubGroupViewModel();
                        newrow.InjectFrom(row);
                        newrow.AccSizeGroupPerRow =
                        AccSizeGroupList.FirstOrDefault(x => x.Iserial == newrow.TblAccSizeGroup);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Client.GetTblAccSizeCodeCompleted += (s, sv) =>
                {
                    SelectedDetailRow.DetailsList.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccessorySizeViewModel();
                        newrow.SizePerRow.InjectFrom(row);
                        SelectedDetailRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    Client.GetTblAccSubGroupSizeLinkAsync(SelectedDetailRow.Iserial);
                };

                Client.GetTblAccSubGroupSizeLinkCompleted += (s, sv) =>
                {
                    foreach (var row in SelectedDetailRow.DetailsList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                    }
                    foreach (var row in sv.Result)
                    {
                        var subfamilyRow = SelectedDetailRow.DetailsList.SingleOrDefault(x =>
                            x.SizePerRow.Iserial == row.TblAccSize);
                        subfamilyRow.Checked = true;
                    }

                    Loading = false;
                };
                Client.UpdateOrInsertTblAccessoriesGroupCompleted += (s, x) =>
                {
                    var savedRow = (TblLkpAccessoryGroupViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblAccessoriesSubGroupCompleted += (s, x) =>
                {
                    var savedRow = (TblAccessoriesSubGroupViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    //if (x.Result.TblSalaryRelation1 != null)
                    //{
                    //    var headerIserial = x.Result.TblSalaryRelation;

                    //    SelectedMainRow.Iserial = headerIserial;
                    //}
                };
                Client.DeleteTblAccessoriesGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblAccessoriesSubGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Client.GetTblAccessoriesGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblAccessoriesGroupAsync(
                                (tbl_lkp_AccessoryGroup)new tbl_lkp_AccessoryGroup().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            int currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new TblLkpAccessoryGroupViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                }
                else
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new tbl_lkp_AccessoryGroup();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblAccessoriesGroupAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            if (SelectedMainRow != null)
                Client.GetTblAccessoriesSubGroupAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
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
                            Client.DeleteTblAccessoriesSubGroupAsync((tbl_AccessoriesSubGroup)new tbl_AccessoriesSubGroup().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            int currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblAccessoriesSubGroupViewModel()
                {
                    GroupID = SelectedMainRow.Iserial
                });
            }
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
                var save = SelectedDetailRow.Iserial == 0;

                var rowToSave = new tbl_AccessoriesSubGroup();
                rowToSave.InjectFrom(SelectedDetailRow);
                if (save && SelectedMainRow.Iserial == 0)
                {
                    rowToSave.tbl_lkp_AccessoryGroup = new tbl_lkp_AccessoryGroup();

                    rowToSave.tbl_lkp_AccessoryGroup.InjectFrom(SelectedMainRow);
                }
                Client.UpdateOrInsertTblAccessoriesSubGroupAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            }
        }

        public void GetDetailSubData()
        {
            if (SelectedDetailRow != null)
                Client.GetTblAccSizeCodeAsync(0, int.MaxValue, (int)SelectedDetailRow.TblAccSizeGroup,
                "it.Iserial", null, null);
        }

        #region Prop

        private ObservableCollection<tbl_lkp_ItemGroupType> _itemGroupType;

        public ObservableCollection<tbl_lkp_ItemGroupType> TblItemGroupTypeList
        {
            get { return _itemGroupType; }
            set { _itemGroupType = value; RaisePropertyChanged("TblItemGroupTypeList"); }
        }

        private ObservableCollection<GenericTable> _accSizeGroupList;

        public ObservableCollection<GenericTable> AccSizeGroupList
        {
            get { return _accSizeGroupList; }
            set { _accSizeGroupList = value; RaisePropertyChanged("AccSizeGroupList"); }
        }

        private SortableCollectionView<TblLkpAccessoryGroupViewModel> _mainRowList;

        public SortableCollectionView<TblLkpAccessoryGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblLkpAccessoryGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblLkpAccessoryGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblLkpAccessoryGroupViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblLkpAccessoryGroupViewModel _selectedMainRow;

        public TblLkpAccessoryGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblAccessoriesSubGroupViewModel _selectedDetailRow;

        public TblAccessoriesSubGroupViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblAccessoriesSubGroupViewModel> _selectedDetailRows;

        public ObservableCollection<TblAccessoriesSubGroupViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblAccessoriesSubGroupViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private TblAccessorySizeViewModel _selectedDetailSubRow;

        public TblAccessorySizeViewModel SelectedDetailSubRow
        {
            get { return _selectedDetailSubRow; }
            set { _selectedDetailSubRow = value; RaisePropertyChanged("SelectedDetailSubRow"); }
        }

        private ObservableCollection<TblAccessorySizeViewModel> _selectedDetailSubRows;

        public ObservableCollection<TblAccessorySizeViewModel> SelectedDetailSubRows
        {
            get { return _selectedDetailSubRows ?? (_selectedDetailSubRows = new ObservableCollection<TblAccessorySizeViewModel>()); }
            set { _selectedDetailSubRows = value; RaisePropertyChanged("SelectedDetailSubRows"); }
        }

        #endregion Prop
    }
}