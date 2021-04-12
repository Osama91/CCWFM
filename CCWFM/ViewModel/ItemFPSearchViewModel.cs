using CCWFM.Models.LocalizationHelpers;
using CCWFM.ViewModel.OGViewModels.ControlsOverride;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.WarehouseService;
using Omu.ValueInjecter.Silverlight;
using SilverlightCommands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CCWFM.ViewModel
{
    public class ItemDimensionFPSearchViewModel : Web.DataLayer.ItemDimensionSearchModel
    {
        // هعمل التعديلات
        CRUDManagerService.ItemsDto itemRow;
        public CRUDManagerService.ItemsDto ItemPerRow
        {
            get { return itemRow; }
            set { itemRow = value; RaisePropertyChanged(nameof(ItemPerRow)); }
        }
        CRUDManagerService.TblColor colorTo;
        public new CRUDManagerService.TblColor ColorPerRow
        {
            get { return colorTo ?? (colorTo = new CRUDManagerService.TblColor()); }
            set
            {
                colorTo = value; RaisePropertyChanged(nameof(ColorPerRow));
                if (ItemPerRow == null || colorTo == null) return;
                ColorToId = colorTo.Iserial;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                if (IsAcc)//كده ده اكسسيسورى
                {
                    foreach (var size in ItemPerRow.CombinationList.Where(
                                 x => (colorTo != null && colorTo.Code == x.Configuration)).Select(x => x.Size))
                    {
                        if (!ItemPerRow.SizesList.Contains(size))
                        {
                            ItemPerRow.SizesList.Add(size);
                        }
                    }
                }
            }
        }
    }
    public class ItemFPSearchViewModel : ViewModelBase
    {
        public ItemFPSearchViewModel()
        {
            Client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                bool isSearchItem = false;
                var itemSearchModel = SearchResultList.FirstOrDefault(i => i.ItemId == sv.Result.Iserial && i.ItemType == sv.Result.ItemGroup);
                CRUDManagerService.ItemsDto item;
                if (itemSearchModel != null)
                {
                    item = itemSearchModel.ItemPerRow ?? (itemSearchModel.ItemPerRow = new CRUDManagerService.ItemsDto());
                    isSearchItem = true;
                }
                else
                    item = ItemPerRow;

                item.AccConfigList = sv.Result.AccConfigList;
                item.SizesList = new ObservableCollection<string>();
                item.CombinationList = sv.Result.CombinationList;

                var tblAccessoryAttributesDetails = sv.Result.CombinationList.FirstOrDefault();
                if (tblAccessoryAttributesDetails != null)
                    item.SizesList.Add(tblAccessoryAttributesDetails.Size);
                if (item.CombinationList != null)
                {
                    item.SizesList = new ObservableCollection<string>();
                }
                if (isSearchItem)
                {
                    if (item.AccConfigList == null) item.AccConfigList = new ObservableCollection<CRUDManagerService.TblColor>();
                    itemSearchModel.ColorPerRow = item.AccConfigList.FirstOrDefault(c => c.Iserial == itemSearchModel.ColorToId);
                }
            };
            CancelCommand = new RelayCommand((o) => // هقفل وارجع فولس يعرف انه كنسل
            {
                var view = (o as ChildWindowsOverride);
                if (view != null)
                {
                    view.DialogResult = false;
                    view.Close();
                }
            });
            ApplyCommand = new RelayCommand((o) =>
            {
                var view = (o as ItemFPSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية     
                        ApplySelectedItem(view);
                }
            });
            OkCommand = new RelayCommand((o) =>
            {
                var view = (o as ItemFPSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية لو مكانش راح
                    //وهقفل الشاشة فى الاخر
                    try
                    {
                        var item = view.DataContext as ItemFPSearchViewModel;
                        ItemDimensionFPSearchViewModel newSearchData = new ItemDimensionFPSearchViewModel();
                        newSearchData.ItemPerRow = item.ItemPerRow;
                        newSearchData.ColorPerRow = item.ColorPerRow;
                        item.FPAppliedSearchResultList.Add(newSearchData);
                        view.DialogResult = true;
                        view.Close();
                    } catch { }
                }
            });
            LoadingRow = new GalaSoft.MvvmLight.Command.RelayCommand<object>((o) =>
            {
                //var e = o as System.Windows.Controls.DataGridRowEventArgs;
                //if (SearchResultList.Count < PageSize)
                //{
                //    return;
                //}
                //if (SearchResultList.Count - 2 < e.Row.GetIndex() && !Loading)
                //{
                //    GetInspectionWarehouseRows();
                //}
            });
           
        }
        private void SetSelectedFromFields(TblItemDim itemDimFrom, ItemDimensionFPSearchViewModel itemTo)
        {
            itemTo.ItemDimFromIserial = itemDimFrom.Iserial;
            itemTo.ColorFromId = itemDimFrom.TblColor;
            itemTo.SizeFrom = itemDimFrom.Size;
            itemTo.BatchNoFrom = itemDimFrom.BatchNo;
            itemTo.SiteFromIserial = itemDimFrom.TblSite;
        }
        private void SetSelectedToFields(TblItemDim itemDimFrom, ItemDimensionFPSearchViewModel itemTo)
        {
            itemTo.ItemDimToIserial = itemDimFrom.Iserial;
            itemTo.ColorToId = itemDimFrom.TblColor;
            itemTo.SizeTo = itemDimFrom.Size;
            itemTo.BatchNoTo = itemDimFrom.BatchNo;
            itemTo.SiteToIserial = itemDimFrom.TblSite;
        }

        internal void ApplySelectedItem(ItemFPSearchChildWindow view)
        {
            ObservableCollection<ItemFPSearchViewModel> temp =  new ObservableCollection<ItemFPSearchViewModel>();
            IsWorking = true;
        }

        #region Properties
        bool isAcc = true;
        public bool IsAcc
        {
            get { return isAcc; }
            set { isAcc = value; RaisePropertyChanged(nameof(IsAcc)); }
        }

        private CRUDManagerService.ItemsDto _selectedItem;
        public CRUDManagerService.ItemsDto ItemPerRow
        {
            get { return _selectedItem ?? (_selectedItem = new CRUDManagerService.ItemsDto()); }
            set
            {
                _selectedItem = value; RaisePropertyChanged(nameof(ItemPerRow));
                if (ItemPerRow != null)
                {
                    IsAcc = false;
                    SearchResultList.Clear();
                    if (ItemPerRow.ItemGroup != null)
                    {
                        SelectedItemType = ItemPerRow.ItemGroup;
                        IsAcc = ItemPerRow.ItemGroup.Contains("Acc");
                    }
                    //if (IsAcc)
                    {
                        Client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                    //else
                    //{
                    //    // الوان الفابريك جاية من كنترول البحث
                    //}
                }
            }
        }

        private CRUDManagerService.TblColor _selectedColor;
        public CRUDManagerService.TblColor ColorPerRow
        {
            get { return _selectedColor; }
            set
            {
                if (value == null) return;
                _selectedColor = value; RaisePropertyChanged(nameof(ColorPerRow));
                ItemPerRow.SizesList = ItemPerRow.SizesList ?? new ObservableCollection<string>();
                ItemPerRow.SizesList.Clear();
                BatchNoList.Clear();
                if (IsAcc)//كده ده اكسسيسورى
                {
                    foreach (var size in ItemPerRow.CombinationList.Where(
                                 x => (_selectedColor != null && _selectedColor.Code == x.Configuration)).Select(x => x.Size))
                    {
                        if (!ItemPerRow.SizesList.Contains(size))
                        {
                            ItemPerRow.SizesList.Add(size);
                        }
                    }
                }
                else// كده قماش هنروح نجيب بقى رقم الباتش 
                {
                    Client.InspectionRouteAsync(ItemPerRow.Code, ColorPerRow.Iserial);
                }
            }
        }

        private string _selectedBatchNo;
        public string SelectedBatchNo
        {
            get { return _selectedBatchNo; }
            set { _selectedBatchNo = value; RaisePropertyChanged(nameof(SelectedBatchNo)); }
        }
        private ObservableCollection<string> _batchNoList;
        public ObservableCollection<string> BatchNoList
        {
            get { return _batchNoList ?? (_batchNoList = new ObservableCollection<string>()); }
            set { _batchNoList = value; RaisePropertyChanged(nameof(BatchNoList)); }
        }

        private string selectedItemType;
        public string SelectedItemType
        {
            get { return selectedItemType; }
            set
            {
                if (ItemPerRow != null && ItemPerRow.ItemGroup != value)
                { selectedItemType = ItemPerRow.ItemGroup; }
                else { selectedItemType = value; }
                RaisePropertyChanged(nameof(SelectedItemType));
            }
        }
        private ObservableCollection<string> itemTypes;
        public ObservableCollection<string> ItemTypes
        {
            get { return itemTypes ?? (itemTypes = new ObservableCollection<string>()); }
            set { itemTypes = value; RaisePropertyChanged(nameof(ItemTypes)); }
        }
        private string _selectedSize;
        public string SelectedSize
        {
            get { return _selectedSize; }
            set { _selectedSize = value; RaisePropertyChanged(nameof(SelectedSize)); }
        }
        private ObservableCollection<string> _sizeList;
        public ObservableCollection<string> SizeList
        {
            get { return _sizeList ?? (_sizeList = new ObservableCollection<string>()); }
            set { _sizeList = value; RaisePropertyChanged(nameof(SizeList)); }
        }

        private ObservableCollection<ItemDimensionSearchModel> _searchResultList;
        public ObservableCollection<ItemDimensionSearchModel> SearchResultList
        {
            get { return _searchResultList ?? (_searchResultList = new ObservableCollection<ItemDimensionSearchModel>()); }
            set { _searchResultList = value; RaisePropertyChanged(nameof(SearchResultList)); }
        }

        private ObservableCollection<ItemDimensionFPSearchViewModel> _fpappliedSearchResultList;
        public ObservableCollection<ItemDimensionFPSearchViewModel> FPAppliedSearchResultList
        {
            get { return _fpappliedSearchResultList ?? (_fpappliedSearchResultList = new ObservableCollection<ItemDimensionFPSearchViewModel>()); }
            set { _fpappliedSearchResultList = value; RaisePropertyChanged(nameof(_fpappliedSearchResultList)); }
        }


        #endregion

        #region Commands

        RelayCommand okCommand;
        public RelayCommand OkCommand
        {
            get { return okCommand; }
            set { okCommand = value; RaisePropertyChanged(nameof(OkCommand)); }
        }
        RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; RaisePropertyChanged(nameof(CancelCommand)); }
        }
        RelayCommand applyCommand;
        public RelayCommand ApplyCommand
        {
            get { return applyCommand; }
            set { applyCommand = value; RaisePropertyChanged(nameof(ApplyCommand)); }
        }
        RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { return searchCommand; }
            set { searchCommand = value; RaisePropertyChanged(nameof(SearchCommand)); }
        }

        GalaSoft.MvvmLight.Command.RelayCommand<object> detailSelectionChanged;
        public GalaSoft.MvvmLight.Command.RelayCommand<object> DetailSelectionChanged
        {
            get { return detailSelectionChanged; }
            set { detailSelectionChanged = value; RaisePropertyChanged(nameof(DetailSelectionChanged)); }
        }
        GalaSoft.MvvmLight.Command.RelayCommand<object> loadingRow;
        public GalaSoft.MvvmLight.Command.RelayCommand<object> LoadingRow
        {
            get { return loadingRow; }
            set { loadingRow = value; RaisePropertyChanged(nameof(LoadingRow)); }
        }
        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set { isWorking = value;}
        }
        
        #endregion

    }
}