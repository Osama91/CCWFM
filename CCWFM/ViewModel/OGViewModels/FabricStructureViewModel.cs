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

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblFabricStructureViewModel : GenericViewModel
    {
        private GenericTable _fabriccategoryPerRow;

        public GenericTable FabricCategoryPerRow
        {
            get { return _fabriccategoryPerRow; }
            set { _fabriccategoryPerRow = value; RaisePropertyChanged("FabricCategoryPerRow"); }
        }

        private int? _tblFabricCategories;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFabricCategory")]
        public int? tbl_FabricCategories
        {
            get
            {
                return _tblFabricCategories;
            }
            set
            {
                if ((_tblFabricCategories.Equals(value) != true))
                {
                    _tblFabricCategories = value;
                    RaisePropertyChanged("tbl_FabricCategories");
                }
            }
        }
    }

    public class FabricStructureViewModel : ViewModelBase
    {
        public FabricStructureViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblFabricStructureViewModel>();
                SelectedMainRow = new TblFabricStructureViewModel();

                Client.GetGenericCompleted += (s, sv) =>
                {
                    FabricCategoryList = sv.Result;
                };

                Client.GetGenericAsync("tbl_FabricCategories", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetTblFabricStructureCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblFabricStructureViewModel { FabricCategoryPerRow = new GenericTable() };
                        if (row.tbl_FabricCategories != null)
                            newrow.FabricCategoryPerRow.InjectFrom(row.tbl_FabricCategories1);
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertTblFabricStructureCompleted += (s, x) =>
                {
                    var savedRow = (TblFabricStructureViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblFabricStructureCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Client.GetTblFabricStructureAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblFabricStructureAsync(
                                (tbl_lkp_FabricStructure)new tbl_lkp_FabricStructure().InjectFrom(row), MainRowList.IndexOf(row));
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
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
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

                MainRowList.Insert(currentRowIndex + 1, new TblFabricStructureViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new tbl_lkp_FabricStructure();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblFabricStructureAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblFabricStructureViewModel> _mainRowList;

        public SortableCollectionView<TblFabricStructureViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblFabricStructureViewModel> _selectedMainRows;

        public ObservableCollection<TblFabricStructureViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblFabricStructureViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblFabricStructureViewModel _selectedMainRow;
        private ObservableCollection<GenericTable> _fabricCategoryList;

        public TblFabricStructureViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        public ObservableCollection<GenericTable> FabricCategoryList
        {
            get { return _fabricCategoryList; }
            set { _fabricCategoryList = value; RaisePropertyChanged("FabricCategoryList"); }
        }

        #endregion Prop
    }
}