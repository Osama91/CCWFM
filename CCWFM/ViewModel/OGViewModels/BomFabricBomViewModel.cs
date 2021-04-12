using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblBomFabricBomViewModelViewModel : PropertiesViewModelBase
    {
        private double _rowIndexField;

        public double RowIndex
        {
            get
            {
                return _rowIndexField;
            }
            set
            {
                if ((_rowIndexField.Equals(value) != true))
                {
                    _rowIndexField = value;
                    RaisePropertyChanged("RowIndex");
                }
            }
        }

        private int? _tblOperationField;

        private TblRouteGroup _route;

        public TblRouteGroup RouteGroupPerRow
        {
            get { return _route; }
            set { _route = value; RaisePropertyChanged("RouteGroupPerRow"); }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqOperation")]
        public int? TblOperation
        {
            get
            {
                return _tblOperationField;
            }
            set
            {
                if ((_tblOperationField.Equals(value) != true))
                {
                    _tblOperationField = value;
                    RaisePropertyChanged("TblOperation");
                }
            }
        }

        private int _baseFabricField;

        private int _iserialField;

        private int? _itemField;

        private string _itemTypeField;

        private double _materialUsageField;

        private ItemsDto _itemPerRow;

        public ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
                if (ItemPerRow != null)
                {
                    ItemType = ItemPerRow.ItemGroup;
                    Item = ItemPerRow.Iserial;
                }
            }
        }

        public int Bom
        {
            get
            {
                return _baseFabricField;
            }
            set
            {
                if ((_baseFabricField.Equals(value) != true))
                {
                    _baseFabricField = value;
                    RaisePropertyChanged("Bom");
                }
            }
        }

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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqItem")]
        public int? Item
        {
            get
            {
                return _itemField;
            }
            set
            {
                if ((_itemField.Equals(value) != true))
                {
                    _itemField = value;
                    RaisePropertyChanged("Item");
                }
            }
        }

        [ReadOnly(true)]
        public string ItemType
        {
            get
            {
                return _itemTypeField;
            }
            set
            {
                if ((ReferenceEquals(_itemTypeField, value) != true))
                {
                    _itemTypeField = value;
                    RaisePropertyChanged("ItemType");
                }
            }
        }

        public double MaterialUsage
        {
            get
            {
                return _materialUsageField;
            }
            set
            {
                if ((_materialUsageField.Equals(value) != true))
                {
                    _materialUsageField = value;
                    RaisePropertyChanged("MaterialUsage");
                }
            }
        }
    }

    public class BomFabricBomViewModel : ViewModelBase
    {
        private readonly BomViewModel _fabricViewModel;

        public BomFabricBomViewModel(BomViewModel viewModel)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                _fabricViewModel = viewModel;
                MainRowList = new SortableCollectionView<TblBomFabricBomViewModelViewModel>();
                SelectedMainRow = new TblBomFabricBomViewModelViewModel();

                Client.GetTblRouteGroupAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblRouteGroupCompleted += (s, sv) =>
                {
                    RouteGroupList = sv.Result;
                };
                Client.GetBomFabricBomCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBomFabricBomViewModelViewModel { ItemPerRow = new ItemsDto() };

                        var ItemRow =
                            sv.FabricServiceList.FirstOrDefault(
                                x => x.Iserial == row.Item && x.ItemGroup == row.ItemType);

                        newrow.ItemPerRow.Code = ItemRow.Code;
                        newrow.ItemPerRow.Name = ItemRow.Name;
                        newrow.ItemPerRow.Unit = ItemRow.Unit;
                        newrow.ItemPerRow.ItemGroup = ItemRow.ItemGroup;

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

                Client.UpdateOrInsertBomFabricBomCompleted += (s, x) =>
                {
                    var savedRow = (TblBomFabricBomViewModelViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteBomFabricBomCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
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
                    SortBy = SortBy + "it." + sortDesc.PropertyName + (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetBomFabricBomAsync(MainRowList.Count, PageSize, _fabricViewModel.Iserial, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteBomFabricBomAsync(
                                (BomFabricBom)new BomFabricBom().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblBomFabricBomViewModelViewModel { Bom = _fabricViewModel.Iserial });
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
                    var saveRow = new BomFabricBom();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertBomFabricBomAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblBomFabricBomViewModelViewModel> _mainRowList;

        public SortableCollectionView<TblBomFabricBomViewModelViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblRouteGroup> _routeGroupList;

        public ObservableCollection<TblRouteGroup> RouteGroupList
        {
            get { return _routeGroupList; }
            set { _routeGroupList = value; RaisePropertyChanged("RouteGroupList"); }
        }

        private ObservableCollection<TblBomFabricBomViewModelViewModel> _selectedMainRows;

        public ObservableCollection<TblBomFabricBomViewModelViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBomFabricBomViewModelViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBomFabricBomViewModelViewModel _selectedMainRow;

        public TblBomFabricBomViewModelViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}