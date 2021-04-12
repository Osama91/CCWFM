using CCWFM.Models.LocalizationHelpers;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Web.DataLayer;
using Omu.ValueInjecter.Silverlight;
using SilverlightCommands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CCWFM.ViewModel
{
    public class ItemDimensionAdjustmentSearchViewModel : ItemDimensionSearchViewModel
    {
        public new WarehouseService.WarehouseServiceClient WarehouseClient = Helpers.Services.Instance.GetWarehouseServiceClient();
        public ItemDimensionAdjustmentSearchViewModel()
        {
            WarehouseClient.GetItemDimensionsQuantitiesByDateCompleted += (s, e) =>
            {
                if (e.Result.Count <= 0)
                    MessageBox.Show(strings.NoDataFound);
                foreach (var iR in e.Result)
                {
                    if (iR.ColorFrom == null)
                    {
                        iR.ColorFrom = new TblColor()
                        {
                            Iserial = iR.ColorFromId,
                            Code = iR.ColorFromCode
                        };
                    }
                    // هشوف لو موجود مش هحطه
                    if (!SearchResultList.Any(sr => sr.ItemDimFromIserial == iR.ItemDimFromIserial))
                    {
                        var qtemp = iR.AvailableQuantity;
                        iR.AvailableQuantity = 0;
                        iR.CountedQuantity = 0;
                        iR.AvailableQuantity = qtemp;
                        SearchResultList.Add(iR);
                    }
                }
                Loading = false;
                FullCount = e.fullCount;
            };
            WarehouseClient.GetItemDimensionsOrCreateCompleted += (s, e) =>
            {
                SelectedSearchResultList.Clear();
                foreach (var item in e.Result)
                {
                    SelectedSearchResultList.Add(item);
                }
                IsWorking = false;
            };
            WarehouseClient.GetAccWarehouseRowsCompleted += (s, e) =>
            {
                //اخر حاجة راجع بيند الكميات وبحث التسوية مش بيحسب الكميات غالبا بيند
                if (e.Result.Count <= 0)
                    MessageBox.Show(strings.NoDataFound);
                foreach (var iR in e.Result)
                {
                    // هشوف لو موجود مش هحطه
                    if (!SearchResultList.Any(sr => sr.ItemDimFromIserial == iR.ItemDimFromIserial))
                    {
                        var qtemp = iR.AvailableQuantity;
                        iR.AvailableQuantity = 0;
                        iR.CountedQuantity = 0;
                        iR.AvailableQuantity = qtemp;
                        SearchResultList.Add(iR);
                    }
                }
                Loading = false;
            };
            ApplyCommand = new RelayCommand((o) =>
            {
                var view = (o as ItemDimensionAdjustmentSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية     
                    ApplySelectedItem(view);
                }
            });
            OkCommand = new RelayCommand((o) =>
            {
                var view = (o as ItemDimensionAdjustmentSearchChildWindow);
                if (view != null)
                {
                    //هنا هاخد الى مختاره فى الجريد للشاشة الاصلية لو مكانش راح
                    //وهقفل الشاشة فى الاخر
                    ApplySelectedItem(view);
                    view.DialogResult = true;
                    view.Close();
                }
            });
        }
        /// <summary>
        /// هاخد السجلات وارجعها للشاشة الاصلية
        /// </summary>
        /// <param name="selectedItems"></param>
        internal void ApplySelectedItem(ItemDimensionAdjustmentSearchChildWindow view)
        {
            ObservableCollection<ItemDimensionAdjustmentSearchModel> temp =
                new ObservableCollection<ItemDimensionAdjustmentSearchModel>();
            IsWorking = true;
            foreach (var item in this.SearchResultList.Where(t => t.DifferenceQuantity != 0))
            {
                item.SiteFromIserial = SiteIserial.HasValue ? SiteIserial.Value : 1;
                item.SiteToIserial = SiteIserial.HasValue ? SiteIserial.Value : 1;
                
                temp.Add(item);
            }
            WarehouseClient.GetItemDimensionsOrCreateAsync(temp);
        }
        DateTime docDate;
        public DateTime DocDate
        {
            get { return docDate; }
            set { docDate = value; RaisePropertyChanged(nameof(DocDate)); }
        }
        public override void GetInspectionWarehouseRows()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            int? colorIserial = null;
            if (ColorPerRow != null) colorIserial = ColorPerRow.Iserial;
            int? iserial = null;
            if (ItemPerRow.Iserial > 0) iserial = ItemPerRow.Iserial;
            if (ItemPerRow.ItemGroup != null && (ItemPerRow.ItemGroup.ToLower().Contains("acc") ||
                ItemPerRow.ItemGroup.ToLower().Contains("fp")) ||
                (SelectedItemType != null && (SelectedItemType.ToLower().Contains("acc") ||
                SelectedItemType.ToLower().Contains("fp"))))
            {
                WarehouseClient.GetAccWarehouseRowsAsync(WarehouseCode, SelectedItemType, iserial
                    , colorIserial, SelectedSize, SelectedBatchNo);
            }
            else
                WarehouseClient.GetItemDimensionsQuantitiesByDateAsync(
                    WarehouseCode, iserial, ItemPerRow.ItemGroup, colorIserial,
                    SelectedSize, SelectedBatchNo, DocDate);
        }
        private ObservableCollection<ItemDimensionAdjustmentSearchModel> _searchResultList;
        public new ObservableCollection<ItemDimensionAdjustmentSearchModel> SearchResultList
        {
            get { return _searchResultList ?? (_searchResultList = new ObservableCollection<ItemDimensionAdjustmentSearchModel>()); }
            set { _searchResultList = value; RaisePropertyChanged(nameof(SearchResultList)); }
        }
        private ObservableCollection<ItemDimensionAdjustmentSearchModel> _appliedSearchResultList;
        public new ObservableCollection<ItemDimensionAdjustmentSearchModel> AppliedSearchResultList
        {
            get { return _appliedSearchResultList ?? (_appliedSearchResultList = new ObservableCollection<ItemDimensionAdjustmentSearchModel>()); }
            set { _appliedSearchResultList = value; RaisePropertyChanged(nameof(AppliedSearchResultList)); }
        }

        private ObservableCollection<ItemDimensionAdjustmentSearchModel> _selectedSearchResultList;
        public new ObservableCollection<ItemDimensionAdjustmentSearchModel> SelectedSearchResultList
        {
            get { return _selectedSearchResultList ?? (_selectedSearchResultList = new ObservableCollection<ItemDimensionAdjustmentSearchModel>()); }
            set
            {
                if ((ReferenceEquals(_selectedSearchResultList, value) != true))
                {
                    _selectedSearchResultList = value;
                    RaisePropertyChanged(nameof(SelectedSearchResultList));
                }
            }
        }

        public new bool IsWorking
        {
            get { return base.IsWorking; }
            set
            {
                if (!value)//&& IsWorkingFrom == 0 && IsWorkingTo == 0//بعد كده هضيف الى عملته فى الشاشة الرئيسية
                {
                    AppliedSearchResultList.Clear();
                    foreach (var item in SelectedSearchResultList)
                    {
                        AppliedSearchResultList.Add(item);
                    }
                }
                base.IsWorking = value;
            }
        }

    }
}
