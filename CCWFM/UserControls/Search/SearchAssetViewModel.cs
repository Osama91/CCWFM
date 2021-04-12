using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchAssetViewModel : ViewModelBase
    {
        public SearchAssetViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblAssetsViewModel>();
                SelectedMainRow = new TblAssetsViewModel();

                Client.GetTblAssetsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAssetsViewModel();
                        if (row.TblAssetsType1 != null) newrow.AssetTypePerRow.InjectFrom(row.TblAssetsType1);
                        if (row.TblHardDisk1 != null) newrow.HardDiskPerRow.InjectFrom(row.TblHardDisk1);
                        if (row.TblMemory1 != null) newrow.MemoryPerRow.InjectFrom(row.TblMemory1);
                        if (row.TblProcessor1 != null) newrow.ProcessorPerRow.InjectFrom(row.TblProcessor1);
                        newrow.Pending = sv.PendingAssets.Any(x => x == row.Iserial);
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
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
                SortBy = "it.Iserial desc";
            Loading = true;
            Client.GetTblAssetsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, true);
        }

        #region Prop

        private SortableCollectionView<TblAssetsViewModel> _mainRowList;

        public SortableCollectionView<TblAssetsViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblAssetsViewModel _selectedMainRow;

        public TblAssetsViewModel SelectedMainRow
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