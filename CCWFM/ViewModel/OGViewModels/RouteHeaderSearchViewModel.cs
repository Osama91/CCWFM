using System.Collections.ObjectModel;
using System.ComponentModel;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid;
using System.Windows;
using System.Linq;

namespace CCWFM.ViewModel.OGViewModels
{
    public class RouteHeaderSearchViewModel : ViewModelBase
    {
        RouteCardService.RouteCardServiceClient RouteCardService = new RouteCardService.RouteCardServiceClient();

        public RouteHeaderSearchViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<RouteCardService.RouteCardHeader>();
                SelectedMainRow = new RouteCardHeader();

                RouteCardService.GetRouteCardHeadersCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        VendorList = sv.Vendors;
                        MainRowList.Add(row);
                    }
                    Loading = false;
                };

                RouteCardService.GetRouteCardHeadersBeforeInspectionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }
                    Sizes = sv.sizes;
                    Loading = false;
                };
            }
        }

        public ObservableCollection<RouteCardService.TblSize> Sizes { get; set; }

        public void GetMaindata(bool inspection)
        {
            if (inspection)
            {


                if (SalesOrderPerRow != null)
                {
                    Loading = true;
                    RouteCardService.GetRouteCardHeadersBeforeInspectionAsync(MainRowList.Count, PageSize, SalesOrderPerRow.Iserial, ColorPerRow.Iserial,
                                LoggedUserInfo.Iserial);
                }
            }
            else
            {
                if (SalesOrderPerRow != null)
                {
                    Loading = true;
                    RouteCardService.GetRouteCardHeadersAsync(MainRowList.Count, PageSize, SalesOrderPerRow.Iserial, ColorPerRow.Iserial,
                            LoggedUserInfo.Iserial);
                }
            }
        }

        #region Prop

        private TblSalesOrder _salesOrderPerRow;

        public TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set { _salesOrderPerRow = value; RaisePropertyChanged("SalesOrderPerRow"); }
        }

        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow ?? (_colorPerRow = new TblColor()); }
            set { _colorPerRow = value; RaisePropertyChanged("ColorPerRow"); }
        }

        private ObservableCollection<RouteCardService.RouteCardHeader> _mainRowList;

        public ObservableCollection<RouteCardService.RouteCardHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private RouteCardHeader _selectedMainRow;

        public RouteCardHeader SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        public ObservableCollection<RouteCardService.Vendor> VendorList { get; private set; }

        #endregion Prop
    }
}