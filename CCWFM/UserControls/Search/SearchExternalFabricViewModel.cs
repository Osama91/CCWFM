using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchExternalFabricViewModel : ViewModelBase
    {
        public SearchExternalFabricViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblSupplierFabric>();
                SelectedMainRow = new TblSupplierFabric();

                Client.GetTblSupplierFabricCompleted += (s, sv) =>
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
            Client.GetTblSupplierFabricAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<TblSupplierFabric> _mainRowList;

        public SortableCollectionView<TblSupplierFabric> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblSupplierFabric _selectedMainRow;

        public TblSupplierFabric SelectedMainRow
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