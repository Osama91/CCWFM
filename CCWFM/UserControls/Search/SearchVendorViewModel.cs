using System.ComponentModel;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchVendorViewModel : ViewModelBase
    {
        public SearchVendorViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<Vendor>();
                SelectedMainRow = new Vendor();

                Client.GetVendorsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    //FullCount = sv.fullCount;
                };

                // GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetVendorsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, "ccm", ItemId, ItemType,SeasonPerRow); //, Filter, ValuesObjects);
        }

        #region Prop

        private int _itemId;

        public int ItemId
        {
            get { return _itemId; }
            set { _itemId = value; RaisePropertyChanged("ItemId"); }
        }

        private string _itemType;

        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; RaisePropertyChanged("ItemType"); }
        }
        private string _SeasonPerRow;

        public string SeasonPerRow
        {
            get { return _SeasonPerRow; }
            set { _SeasonPerRow = value; RaisePropertyChanged("SeasonPerRow"); }
        }

        private SortableCollectionView<Vendor> _mainRowList;

        public SortableCollectionView<Vendor> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private Vendor _selectedMainRow;

        public Vendor SelectedMainRow
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
}