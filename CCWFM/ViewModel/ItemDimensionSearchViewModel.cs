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
    public class ItemDimensionSearchModel : Web.DataLayer.ItemDimensionSearchModel
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
    public class ItemDimensionSearchViewModel : ViewModelBase
    {
        string fromTitle;
        public string FromTitle
        {
            set { fromTitle = value; RaisePropertyChanged(nameof(FromTitle)); }
            get { return fromTitle; }
        }
        string toTitle;
        public string ToTitle
        {
            set { toTitle = value; RaisePropertyChanged(nameof(ToTitle)); }
            get { return toTitle; }
        }
        public WarehouseService.WarehouseServiceClient WarehouseClient = new WarehouseService.WarehouseServiceClient();
        bool isWorking = false;
        string warehouseCode, warehouseToCode, title;
        int? siteIserial;
        public string WarehouseCode
        {
            get { return warehouseCode; }
            set
            {
                warehouseCode = value;
                RaisePropertyChanged(nameof(WarehouseCode));
                WarehouseClient.GetInspectionWarehouseRowsCompleted += (s, e) =>
                {
                    if (e.Result.Count <= 0)
                        MessageBox.Show(strings.NoDataFound);
                    foreach (var iR in e.Result)
                    {
                        // هشوف لو موجود مش هحطه
                        if (!SearchResultList.Any(sr => ItemPerRow.Code == iR.ItemCode &&//مفيش size علشان ده فابريك
                        ItemPerRow.ItemGroup == sr.ItemType && iR.BatchNoFrom == sr.BatchNoFrom &&
                         iR.ColorFromId == sr.ColorFromId))
                        {                          
                            if (iR.PendingQuantity > 0)
                                iR.TransferredQuantity = iR.AvailableQuantity;
                            else
                                iR.TransferredQuantity = iR.AvailableQuantity + iR.PendingQuantity;
                            var temp = new ItemDimensionSearchModel();
                            temp.InjectFrom(iR);
                            temp.ColorFrom.InjectFrom(iR.ColorFrom);
                            if(ColorPerRow != null)
                                temp.ColorPerRow.InjectFrom(ColorPerRow);
                            if (iR.ColorPerRow != null)
                                temp.ColorPerRow.InjectFrom(iR.ColorPerRow);
                            temp.ItemPerRow = ItemPerRow;
                            SearchResultList.Add(temp);
                        }
                    }
                    Loading = false;
                    FullCount = e.fullCount;
                };
                SearchCommand = new RelayCommand((o) =>
                {
                    // هنا بقى هعبى ليست الى بتحط فى الجريد
                    SearchResultList.Clear();
                    GetInspectionWarehouseRows();
                });
            }
        }
        public string WarehouseToCode
        {
            get { return warehouseToCode; }
            set
            {
                warehouseToCode = value;
                RaisePropertyChanged(nameof(WarehouseToCode));
            }
        }
        public int? SiteIserial
        {
            get { return siteIserial; }
            set { siteIserial = value; RaisePropertyChanged(nameof(SiteIserial)); }
        }

        public virtual void GetInspectionWarehouseRows()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            int? colorIserial = null;
            if (ColorPerRow != null) colorIserial = ColorPerRow.Iserial;
            int? iserial = null;
            if (ItemPerRow.Iserial > 0) iserial = ItemPerRow.Iserial;
            if ((ItemPerRow.ItemGroup != null && (ItemPerRow.ItemGroup.ToLower().Contains("acc") ||
                ItemPerRow.ItemGroup.ToLower().Contains("fp"))) ||
                (SelectedItemType != null && (SelectedItemType.ToLower().Contains("acc") ||
                SelectedItemType.ToLower().Contains("fp"))))
            {
                WarehouseClient.GetAccWarehouseRowsAsync(WarehouseCode, SelectedItemType, iserial
                    , colorIserial, SelectedSize, SelectedBatchNo);
            }
            else
                WarehouseClient.GetInspectionWarehouseRowsAsync(SearchResultList.Count, PageSize, SortBy, WarehouseCode, SelectedItemType,
                    ItemPerRow.Code, (ColorPerRow != null ? ColorPerRow.Code : ""), SelectedSize, SelectedBatchNo);
        }
        public ItemDimensionSearchViewModel()
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
            Client.InspectionRouteCompleted += (o, e) =>
            {
                foreach (var batchNo in e.Result.Select(x => x.RollBatch))
                {
                    if (!BatchNoList.Contains(batchNo))
                    {
                        BatchNoList.Add(batchNo);
                    }
                }
            };
            // Client.SearchF
            WarehouseClient.GetAccWarehouseRowsCompleted += (s, e) =>
            {
                if (e.Result.Count <= 0)
                    MessageBox.Show(strings.NoDataFound);
                foreach (var iR in e.Result)
                {
                    // هشوف لو موجود مش هحطه
                    if (!SearchResultList.Any(sr => sr.ItemDimFromIserial == iR.ItemDimFromIserial))
                    {
                        var qtemp = iR.AvailableQuantity;
                        iR.AvailableQuantity = 0;
                        // iR.CountedQuantity = 0;
                        iR.AvailableQuantity = qtemp;
                        if (iR.PendingQuantity > 0)
                            iR.TransferredQuantity = iR.AvailableQuantity;
                        else
                            iR.TransferredQuantity = iR.AvailableQuantity + iR.PendingQuantity;
                        ItemDimensionSearchModel item = new ItemDimensionSearchModel();
                        item.InjectFrom(iR);
                        if (item.ItemPerRow==null)
                            item.ItemPerRow = new CRUDManagerService.ItemsDto();
                        if (ItemPerRow.Iserial > 0)
                            item.ItemPerRow.InjectFrom(ItemPerRow);
                        //else
                        //    Client.AccWithConfigAndSizeAsync(new CRUDManagerService.ItemsDto()
                        //    {
                        //        Iserial = item.ItemId,
                        //        ItemGroup = item.ItemType,
                        //        Code=item.ItemCode,
                        //    });
                        if (item.ItemPerRow.AccConfigList == null) item.ItemPerRow.AccConfigList = new ObservableCollection<CRUDManagerService.TblColor>();
                        var colorRow = item.ItemPerRow.AccConfigList.FirstOrDefault(c => c.Iserial == iR.ColorPerRow.Iserial);
                        if (colorRow == null)
                        {
                            colorRow = new CRUDManagerService.TblColor()
                            {
                                Iserial = item.ColorFromId,
                                Code = item.ColorFromCode
                            };
                        }
                        item.ColorPerRow = colorRow;

                        SearchResultList.Add(item);
                    }
                }
                Loading = false;
            };
            WarehouseClient.GetItemDimensionsOrCreteForTransferCompleted += (s, e) =>
            {
                SelectedSearchResultList.Clear();
                foreach (var item in e.Result)
                {
                    ItemDimensionSearchModel temp = new ItemDimensionSearchModel();
                    temp.InjectFrom(item);
                    temp.ItemPerRow = ItemPerRow;
                    temp.ColorPerRow.InjectFrom(item.ColorPerRow);
                    SelectedSearchResultList.Add(temp);
                }
                IsWorking = false;
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
                var view = (o as ItemDimensionSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية     
                        ApplySelectedItem(view);
                }
            });
            OkCommand = new RelayCommand((o) =>
            {
                var view = (o as ItemDimensionSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية لو مكانش راح
                    //وهقفل الشاشة فى الاخر
                    ApplySelectedItem(view);
                    view.DialogResult = true;
                    view.Close();
                }
            });
            LoadingRow = new GalaSoft.MvvmLight.Command.RelayCommand<object>((o) =>
            {
                var e = o as System.Windows.Controls.DataGridRowEventArgs;
                if (SearchResultList.Count < PageSize)
                {
                    return;
                }
                if (SearchResultList.Count - 2 < e.Row.GetIndex() && !Loading)
                {
                    GetInspectionWarehouseRows();
                }
            });
            DetailSelectionChanged = new GalaSoft.MvvmLight.Command.RelayCommand<object>(
                  (o) => {
                      var e = o as System.Windows.Controls.SelectionChangedEventArgs;
                      var temp = new Web.DataLayer.ItemDimensionSearchModel();
                      if (e.AddedItems != null && e.AddedItems.Count > 0)
                      {
                          temp.InjectFrom(e.AddedItems[0]);
                          WarehouseClient.GetItemToQuantitiesAsync(temp, WarehouseToCode);
                      }
                  });
            WarehouseClient.GetItemToQuantitiesCompleted += (s, e) =>
            {
                var item = SearchResultList.FirstOrDefault(rl =>
                  rl.ItemId == e.Result.ItemId && rl.ItemType == e.Result.ItemType &&
                  rl.ItemDimFromIserial == e.Result.ItemDimFromIserial &&
                  rl.ColorFromId == e.Result.ColorFromId &&
                  rl.SizeFrom == e.Result.SizeFrom && rl.BatchNoFrom == e.Result.BatchNoFrom &&
                  rl.ColorToId == e.Result.ColorToId && rl.SizeTo == e.Result.SizeTo &&
                  rl.BatchNoTo == e.Result.BatchNoTo);
                if (item != null)
                {
                    item.ItemDimToIserial = e.Result.ItemDimToIserial;
                    item.AvailableToQuantity = e.Result.AvailableToQuantity;
                    item.PendingToQuantity = e.Result.PendingToQuantity;
                }
                else
                {
                    //item.AvailableToQuantity = 0;
                    //item.PendingToQuantity = 0;
                }
            };

            WarehouseClient.GetItemGroupCompleted += (s, e) =>
            {
                foreach (var item in e.Result)
                {
                    ItemTypes.Add(item);
                }
            };
            WarehouseClient.GetItemGroupAsync();
        }
        private void SetSelectedFromFields(TblItemDim itemDimFrom, ItemDimensionSearchModel itemTo)
        {
            itemTo.ItemDimFromIserial = itemDimFrom.Iserial;
            itemTo.ColorFromId = itemDimFrom.TblColor;
            itemTo.SizeFrom = itemDimFrom.Size;
            itemTo.BatchNoFrom = itemDimFrom.BatchNo;
            itemTo.SiteFromIserial = itemDimFrom.TblSite;
        }
        private void SetSelectedToFields(TblItemDim itemDimFrom, ItemDimensionSearchModel itemTo)
        {
            itemTo.ItemDimToIserial = itemDimFrom.Iserial;
            itemTo.ColorToId = itemDimFrom.TblColor;
            itemTo.SizeTo = itemDimFrom.Size;
            itemTo.BatchNoTo = itemDimFrom.BatchNo;
            itemTo.SiteToIserial = itemDimFrom.TblSite;
        }
        /// <summary>
        /// هاخد السجلات وارجعها للشاشة الاصلية
        /// </summary>
        /// <param name="selectedItems"></param>
        internal void ApplySelectedItem(ItemDimensionSearchChildWindow view)
        {
            ObservableCollection<Web.DataLayer.ItemDimensionSearchModel> temp =
                new ObservableCollection<Web.DataLayer.ItemDimensionSearchModel>();
            IsWorking = true;
            var selectedItems = SearchResultList.Where(t => t.TransferredQuantity > 0 && (
            (t.AvailableQuantity >= t.TransferredQuantity && t.PendingQuantity >= 0) ||
            ((t.AvailableQuantity + t.PendingQuantity) >= t.TransferredQuantity && t.PendingQuantity < 0)));
            if (selectedItems.Count() <= 0) { MessageBox.Show(strings.CheckQuantities); return; }
            foreach (var item in selectedItems)
            {
                item.SiteFromIserial = SiteIserial.HasValue ? SiteIserial.Value : 1;
                item.SiteToIserial = SiteIserial.HasValue ? SiteIserial.Value : 1;

                Web.DataLayer.ItemDimensionSearchModel tempItem = new Web.DataLayer.ItemDimensionSearchModel();
                tempItem.InjectFrom(item);
                tempItem.ColorPerRow.InjectFrom(item.ColorPerRow);
                temp.Add(tempItem);
            }
            WarehouseClient.GetItemDimensionsOrCreteForTransferAsync(temp, WarehouseToCode);
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
                    if (IsAcc)
                    {
                        Client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                    else
                    {
                        // الوان الفابريك جاية من كنترول البحث
                    }
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

        private ObservableCollection<ItemDimensionSearchModel> _appliedSearchResultList;
        public ObservableCollection<ItemDimensionSearchModel> AppliedSearchResultList
        {
            get { return _appliedSearchResultList ?? (_appliedSearchResultList = new ObservableCollection<ItemDimensionSearchModel>()); }
            set { _appliedSearchResultList = value; RaisePropertyChanged(nameof(AppliedSearchResultList)); }
        }

        private ObservableCollection<ItemDimensionSearchModel> _selectedSearchResultList;
        public ObservableCollection<ItemDimensionSearchModel> SelectedSearchResultList
        {
            get { return _selectedSearchResultList ?? (_selectedSearchResultList = new ObservableCollection<ItemDimensionSearchModel>()); }
            set
            {
                if ((ReferenceEquals(_selectedSearchResultList, value) != true))
                {
                    _selectedSearchResultList = value;
                    RaisePropertyChanged(nameof(SelectedSearchResultList));
                }
            }
        }
     
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(nameof(Title)); }
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
        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                if (!value)//&& IsWorkingFrom == 0 && IsWorkingTo == 0//بعد كده هضيف الى عملته فى الشاشة الرئيسية
                {// بترجع من السيرش تمام بس بتضيف مرتيين
                    AppliedSearchResultList.Clear();
                    foreach (var item in SelectedSearchResultList)
                    {
                        item.ItemPerRow = ItemPerRow;
                        AppliedSearchResultList.Add(item);
                    }
                }
                isWorking = value;
            }
        }
        
        #endregion

    }
}