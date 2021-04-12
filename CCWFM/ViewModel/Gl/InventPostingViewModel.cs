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
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class InventPostingViewModel : ViewModelBase
    {
        public InventPostingViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.InventPosting.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblInventPostingViewModel>();
                SelectedMainRow = new TblInventPostingViewModel();
                Glclient.GetGlItemGroupAsync(LoggedUserInfo.DatabasEname);
                Glclient.GetGlItemGroupCompleted += (s, sv) =>
                {
                    ItemGroupList = sv.Result;
                };
                Glclient.GetTblInventPostingTypeAsync(LoggedUserInfo.DatabasEname);
                Glclient.GetTblInventPostingTypeCompleted += (s, sv) =>
                {
                    InventPostingTypeList = sv.Result;
                    Items = new ObservableCollection<TblInventAccountType>(InventPostingTypeList.Where(x => x.TabName == "SalesOrder"));
                };
                Glclient.GetTblInventPostingCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblInventPostingViewModel
                        {
                            AccountPerRow = row.TblAccount1,
                            ItemGroupPerRow = ItemGroupList.FirstOrDefault(x => x.Iserial == row.ItemScopeRelation)
                        };
                        newrow.InjectFrom(row);
                        newrow.JournalAccountTypePerRow = JournalAccountTypePerRow;
                        if (sv.entityList != null && sv.entityList.Any(x => x.scope == row.SupCustScope && x.Iserial == row.SupCustRelation))
                        {
                            newrow.EntityAccountPerRow =
                      sv.entityList.First(x => x.scope == row.SupCustScope && x.Iserial == row.SupCustRelation);
                        }

                        MainRowList.Add(newrow);
                    }

                    Loading = false;

                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblInventPostingsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                };
                Glclient.DeleteTblInventPostingCompleted += (s, ev) =>
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
            }
        }

        private ObservableCollection<TblInventAccountType> _items;

        public ObservableCollection<TblInventAccountType> Items
        {
            get { return _items; }
            set { _items = value; RaisePropertyChanged("Items"); }
        }

        private ObservableCollection<Entity> _itemGroupList;

        public ObservableCollection<Entity> ItemGroupList
        {
            get { return _itemGroupList; }
            set { _itemGroupList = value; RaisePropertyChanged("ItemGroupList"); }
        }

        public ObservableCollection<TblInventAccountType> InventPostingTypeList
        {
            get { return _inventPostingTypeList; }
            set { _inventPostingTypeList = value; RaisePropertyChanged("InventPostingTypeList"); }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            MainRowList.Clear();
            Glclient.GetTblInventPostingAsync(TblInventAccountType,
                LoggedUserInfo.DatabasEname, JournalAccountTypePerRow.Iserial);
        }

        public int TblInventAccountType
        {
            get { return _tblInventAccountType; }
            set { _tblInventAccountType = value; RaisePropertyChanged("TblInventAccountType"); }
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
            var newrow = new TblInventPostingViewModel
            {
                TblInventAccountType = TblInventAccountType,
                JournalAccountTypePerRow = JournalAccountTypePerRow
            };
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
                    var saveRow = new TblInventPosting();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblInventPostingsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
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
                            Glclient.DeleteTblInventPostingAsync((TblInventPosting)new TblInventPosting().InjectFrom(row),
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

        private GenericTable _genericTable;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _genericTable ?? (_genericTable = new GenericTable { Iserial = 1 }); }
            set { _genericTable = value; RaisePropertyChanged("JournalAccountTypePerRow"); }
        }

        private SortableCollectionView<TblInventPostingViewModel> _mainRowList;

        public SortableCollectionView<TblInventPostingViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblInventPostingViewModel>()); }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblInventPostingViewModel> _selectedMainRows;

        public ObservableCollection<TblInventPostingViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblInventPostingViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblInventPostingViewModel _selectedMainRow;
        private int _tblInventAccountType;
        private ObservableCollection<TblInventAccountType> _inventPostingTypeList;

        public TblInventPostingViewModel SelectedMainRow
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

    public class TblInventPostingViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private GenericTable _genericTable;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _genericTable ?? (_genericTable = new GenericTable { Iserial = 1 }); }
            set { _genericTable = value; RaisePropertyChanged("JournalAccountTypePerRow"); }
        }

        private Entity _itemGroupPerRow;

        public Entity ItemGroupPerRow
        {
            get { return _itemGroupPerRow; }
            set { _itemGroupPerRow = value; RaisePropertyChanged("ItemGroupPerRow"); }
        }

        private Entity _entityAccountPerRow;

        public Entity EntityAccountPerRow
        {
            get { return _entityAccountPerRow; }
            set
            {
                _entityAccountPerRow = value; RaisePropertyChanged("EntityAccountPerRow");
                if (EntityAccountPerRow != null) SupCustRelation = EntityAccountPerRow.Iserial;
            }
        }

        private int _iserialField;

        private int? _itemScopeRelationField;

        private int? _supCustRelationField;

        private int _supCustScopeField;

        private int? _tblAccountField;

        private TblAccount _tblAccount1Field;

        private int? _tblInventAccountTypeField;

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

        [Required]
        public int? ItemScopeRelation
        {
            get
            {
                return _itemScopeRelationField;
            }
            set
            {
                if ((_itemScopeRelationField.Equals(value) != true))
                {
                    _itemScopeRelationField = value;
                    RaisePropertyChanged("ItemScopeRelation");
                }
            }
        }

        public int? SupCustRelation
        {
            get
            {
                return _supCustRelationField;
            }
            set
            {
                if ((_supCustRelationField.Equals(value) != true))
                {
                    _supCustRelationField = value;
                    RaisePropertyChanged("SupCustRelation");
                }
            }
        }

        [Required]
        public int SupCustScope
        {
            get
            {
                return _supCustScopeField;
            }
            set
            {
                if ((_supCustScopeField.Equals(value) != true))
                {
                    _supCustScopeField = value;
                    RaisePropertyChanged("SupCustScope");
                    ScopePerRow = new GenericTable
                    {
                        Iserial = SupCustScope,
                        Aname = "",
                        Code = "",
                        Ename = ""
                    };
                }
            }
        }

        private GenericTable _scopePerRow;

        public GenericTable ScopePerRow
        {
            get { return _scopePerRow; }
            set { _scopePerRow = value; RaisePropertyChanged("ScopePerRow"); }
        }

        [Required]
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

        public TblAccount AccountPerRow
        {
            get
            {
                return _tblAccount1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblAccount1Field, value) != true))
                {
                    _tblAccount1Field = value;
                    RaisePropertyChanged("AccountPerRow");
                    TblAccount = AccountPerRow.Iserial;
                }
            }
        }

        [Required]
        public int? TblInventAccountType
        {
            get
            {
                return _tblInventAccountTypeField;
            }
            set
            {
                if ((_tblInventAccountTypeField.Equals(value) != true))
                {
                    _tblInventAccountTypeField = value;
                    RaisePropertyChanged("TblInventAccountType");
                }
            }
        }
    }

    #endregion ViewModels
}