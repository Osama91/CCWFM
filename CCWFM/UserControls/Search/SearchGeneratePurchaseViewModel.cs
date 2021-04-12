using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;
using CCWFM.PurchasePlanService;

namespace CCWFM.UserControls.Search
{
    public class SearchGeneratePurchaseViewModel : ViewModelBase
    {
        PurchasePlanClient PurchasePlanClient = new PurchasePlanClient();

        public SearchGeneratePurchaseViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblGeneratePurchaseHeader>();
                SelectedMainRow = new TblGeneratePurchaseHeader();

                PurchasePlanClient.GetTblGeneratePurchaseHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            PurchasePlanClient.GetTblGeneratePurchaseHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<TblGeneratePurchaseHeader> _mainRowList;

        public SortableCollectionView<TblGeneratePurchaseHeader> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblGeneratePurchaseHeader _selectedMainRow;

        public TblGeneratePurchaseHeader SelectedMainRow
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