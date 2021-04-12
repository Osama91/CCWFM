using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.FabricToolsViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblFabricBomViewModel : PropertiesViewModelBase
    {
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

        public int BaseFabric
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
                    RaisePropertyChanged("BaseFabric");
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

    public class FabricBomViewModel : ViewModelBase
    {
        private readonly FabricSetupsViewModel _fabricViewModel;

        public FabricBomViewModel(FabricSetupsViewModel viewModel)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                _fabricViewModel = viewModel;
                MainRowList = new SortableCollectionView<TblFabricBomViewModel>();
                SelectedMainRow = new TblFabricBomViewModel();

                Client.GetTblFabricBomCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblFabricBomViewModel { ItemPerRow = new ItemsDto() };

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

                Client.UpdateOrInsertTblFabricBomCompleted += (s, x) =>
                {
                    var savedRow = (TblFabricBomViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblFabricBomCompleted += (s, ev) =>
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

        public FabricBomViewModel(FabricSetupsWFViewModel viewModel)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                _fabricViewModel.InjectFrom(viewModel);
                MainRowList = new SortableCollectionView<TblFabricBomViewModel>();
                SelectedMainRow = new TblFabricBomViewModel();

                Client.GetTblFabricBomCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblFabricBomViewModel { ItemPerRow = new ItemsDto() };

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

                Client.UpdateOrInsertTblFabricBomCompleted += (s, x) =>
                {
                    var savedRow = (TblFabricBomViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblFabricBomCompleted += (s, ev) =>
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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblFabricBomAsync(MainRowList.Count, PageSize, _fabricViewModel.Iserial, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblFabricBomAsync(
                                (TblFabricBom)new TblFabricBom().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblFabricBomViewModel { BaseFabric = _fabricViewModel.Iserial });
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
                    var saveRow = new TblFabricBom();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblFabricBomAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblFabricBomViewModel> _mainRowList;

        public SortableCollectionView<TblFabricBomViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblFabricBomViewModel> _selectedMainRows;

        public ObservableCollection<TblFabricBomViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblFabricBomViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblFabricBomViewModel _selectedMainRow;

        public TblFabricBomViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}