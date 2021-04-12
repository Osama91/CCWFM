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
    public class CostCenterViewModel : ViewModelBase
    {
        public CostCenterViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.CostCenter.ToString());
                Glclient = new GlServiceClient();
                var costDimSetupTypeClient = new GlServiceClient();
                costDimSetupTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                costDimSetupTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                MainRowList = new SortableCollectionView<TblCostCenterViewModel>();
                SelectedMainRow = new TblCostCenterViewModel();

                Glclient.GetTblCostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostCenterViewModel();
                        newrow.InjectFrom(row);
                        newrow.CostCenterTypePerRow = new GenericTable();
                        if (row.TblCostCenterType1 != null)
                            newrow.CostCenterTypePerRow.InjectFrom(row.TblCostCenterType1);
                        newrow.CostCenterPerRow = row.TblCostCenter2;

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

                Glclient.UpdateOrInsertTblCostCentersCompleted += (s, ev) =>
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
                Glclient.DeleteTblCostCenterCompleted += (s, ev) =>
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
            Glclient.GetTblCostCenterAsync(MainRowList.Count, PageSize, null, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname, 0, 0, 0);
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
            var newrow = new TblCostCenterViewModel();
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
                    var saveRow = new TblCostCenter();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblCostCentersAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblCostCenterViewModel oldRow)
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
                    var saveRow = new TblCostCenter();
                    saveRow.InjectFrom(oldRow);
                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblCostCentersAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblCostCenterAsync((TblCostCenter)new TblCostCenter().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private SortableCollectionView<TblCostCenterViewModel> _mainRowList;

        public SortableCollectionView<TblCostCenterViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
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

        private ObservableCollection<TblCostCenterViewModel> _selectedMainRows;

        public ObservableCollection<TblCostCenterViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostCenterViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblCostCenterViewModel _selectedMainRow;

        public TblCostCenterViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblCostCenterViewModel : GenericViewModel
    {
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
                    if (CostCenterTypePerRow != null)
                    {
                        if (CostCenterTypePerRow.Iserial != 0)
                        {
                            TblCostCenterType = CostCenterTypePerRow.Iserial;
                        }
                    }
                }
            }
        }

        private int _level;

        public int CostCenterLevel
        {
            get { return _level; }
            set { _level = value; RaisePropertyChanged("CostCenterLevel"); }
        }

        private int _type;

        public int Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged("Type"); }
        }

        private int? _parent;

        public int? Parent
        {
            get { return _parent; }
            set { _parent = value; RaisePropertyChanged("Parent"); }
        }

        private TblCostCenter _costCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _costCenterPerRow; }
            set
            {
                _costCenterPerRow = value; RaisePropertyChanged("CostCenterPerRow");
                if (CostCenterPerRow != null) Parent = CostCenterPerRow.Iserial;
            }
        }
    }

    #endregion ViewModels
}