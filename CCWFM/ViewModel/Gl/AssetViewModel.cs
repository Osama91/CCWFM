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
    public class AssetViewModel : ViewModelBase
    {
        public AssetViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Asset.ToString());
                Glclient = new GlServiceClient();

                var AssetClient = new GlServiceClient();
                AssetClient.GetGenericCompleted += (s, sv) =>
                {
                    AssetGroupList = sv.Result;
                };
                AssetClient.GetGenericAsync("TblAssetGroup", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblAssetViewModel>();
                SelectedMainRow = new TblAssetViewModel();

                Glclient.GetTblAssetRetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAssetViewModel();
                        newrow.InjectFrom(row);
                        newrow.AssetGroupPerRow = new GenericTable();
                        if (row.TblAssetGroup1 != null) newrow.AssetGroupPerRow.InjectFrom(row.TblAssetGroup1);

                        newrow.AccountPerRow = new TblAccount();
                        if (row.TblAccount1 != null) newrow.AccountPerRow = row.TblAccount1;

                        newrow.SumAccountPerRow = new TblAccount();
                        if (row.TblAccount2 != null) newrow.SumAccountPerRow = row.TblAccount2;

                        
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

                Glclient.UpdateOrInsertTblAssetRetailCompleted += (s, ev) =>
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
                Glclient.DeleteTblAssetRetailCompleted += (s, ev) =>
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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblAssetRetailAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetTblAssetRetailAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblAssetViewModel();
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
                    var saveRow = new TblAssetRetail();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblAssetRetailAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblAssetViewModel oldRow)
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
                    var saveRow = new TblAssetRetail();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblAssetRetailAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblAssetRetailAsync((TblAssetRetail)new TblAssetRetail().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        private ObservableCollection<GenericTable> _assetList;

        public ObservableCollection<GenericTable> AssetGroupList
        {
            get { return _assetList; }
            set { _assetList = value; RaisePropertyChanged("AssetGroupList"); }
        }

        private SortableCollectionView<TblAssetViewModel> _mainRowList;

        public SortableCollectionView<TblAssetViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblAssetViewModel> _selectedMainRows;

        public ObservableCollection<TblAssetViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAssetViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblAssetViewModel _selectedMainRow;

        public TblAssetViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblAssetViewModel : GenericViewModel
    {
        private int? _tblAccount;

        public int? TblAccount
        {
            get { return _tblAccount; }
            set { _tblAccount = value; RaisePropertyChanged("TblAccount"); }
        }

        private int? _sumAccount;

        public int? SumAccount
        {
            get { return _sumAccount; }
            set { _sumAccount = value; RaisePropertyChanged("SumAccount"); }
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
                    if (_accountPerRow != null)
                    {
                        if (_accountPerRow.Iserial != 0)
                        {
                            TblAccount = _accountPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private TblAccount _sumAccountPerRow;

        public TblAccount SumAccountPerRow
        {
            get { return _sumAccountPerRow; }
            set
            {
                if ((ReferenceEquals(_sumAccountPerRow, value) != true))
                {
                    _sumAccountPerRow = value;
                    RaisePropertyChanged("SumAccountPerRow");
                    if (_sumAccountPerRow != null)
                    {
                        if (_sumAccountPerRow.Iserial != 0)
                        {
                            SumAccount = _sumAccountPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private GenericTable _assetGroupPerRow;

        public GenericTable AssetGroupPerRow
        {
            get { return _assetGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_assetGroupPerRow, value) != true))
                {
                    _assetGroupPerRow = value;
                    RaisePropertyChanged("AssetGroupPerRow");
                    if (AssetGroupPerRow != null)
                    {
                        if (AssetGroupPerRow.Iserial != 0)
                        {
                            TblAssetGroup = AssetGroupPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private int? _tblTblAssetGroupField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAssetGroup")]
        public int? TblAssetGroup
        {
            get
            {
                return _tblTblAssetGroupField;
            }
            set
            {
                if ((_tblTblAssetGroupField.Equals(value) != true))
                {
                    _tblTblAssetGroupField = value;
                    RaisePropertyChanged("TblAssetGroup");
                }
            }
        }
    }

    #endregion ViewModels
}