using System.Collections.ObjectModel;
using System.ComponentModel;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class RouteFreeIssueViewModel : ViewModelBase
    {
        public RouteFreeIssueViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<RouteCardFabricViewModel>();
                SalesOrderList = new SortableCollectionView<TblSalesOrder>();
            }
        }

        #region Prop

        private SortableCollectionView<RouteCardFabricViewModel> _mainRowList;

        public SortableCollectionView<RouteCardFabricViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private RouteCardFabricViewModel _selectedMainRow;

        public RouteCardFabricViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private SortableCollectionView<TblSalesOrder> _salesOrderList;

        public SortableCollectionView<TblSalesOrder> SalesOrderList
        {
            get { return _salesOrderList; }
            set { _salesOrderList = value; RaisePropertyChanged("SalesOrderList"); }
        }

        private ObservableCollection<V_Warehouse> _warehouseList;

        public ObservableCollection<V_Warehouse> WarehouseList
        {
            get { return _warehouseList; }
            set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
        }

        #endregion Prop
    }
}