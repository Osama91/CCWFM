using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels.Mappers;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class MarkerHeaderListViewModel : ViewModelBase
    {

        RouteCardService.RouteCardServiceClient RouteCardService = new RouteCardService.RouteCardServiceClient();

        public MarkerHeaderListViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {

                Client = new CRUD_ManagerServiceClient();
                RouteGroups = new ObservableCollection<RouteCardService.TblRouteGroup>();
                Routes = new ObservableCollection<RouteCardService.TblRoute>();
                MarkerListViewModelList = new ObservableCollection<MarkerListViewModel>();
                MarkerDetailsPagedCollectionView = new PagedCollectionView(MarkerListViewModelList);
                MarkerListViewModelList.Add(new MarkerListViewModel());
                RouteCardService.GetRoutGroupsAsync(null, null);
                RouteCardService.GetRoutGroupsCompleted += (a, b) =>
                {
                    try
                    {
                        foreach (RouteCardService.TblRouteGroup item in b.Result)
                        {
                            RouteGroups.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        var err = new ErrorWindow(ex);
                        err.Show();
                    }
                };
            }
        }

        #region Properties

        private int _iserialField;
        private PagedCollectionView _markerDetailsPagedCollectionView;
        private ObservableCollection<MarkerListViewModel> _markerListViewModelList;
        private bool? _posted;
        private int _routGroupId;
        private RouteCardService.TblRouteGroup _routGroupItem;
        private int _routId;
        private ObservableCollection<RouteCardService.TblRouteGroup> _routeGroups;
        private RouteCardService.TblRoute _routeItem;
        private ObservableCollection<RouteCardService.TblRoute> _routes;

        private int _status;
        private int _tblMarkerType;

        private DateTime? _transactionDateField;
        private string _vendorCode;
        private Vendor _vendorPerRow;

        private string _wareHousesField;

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public DateTime? TransDate
        {
            get { return _transactionDateField; }
            set
            {
                if ((_transactionDateField.Equals(value) != true))
                {
                    _transactionDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }

        public string WareHouses
        {
            get { return _wareHousesField; }
            set
            {
                if ((ReferenceEquals(_wareHousesField, value) != true))
                {
                    _wareHousesField = value;
                    RaisePropertyChanged("WareHouses");
                }
            }
        }

        public string VendorCode
        {
            get { return _vendorCode; }
            set
            {
                _vendorCode = value;
                RaisePropertyChanged("VendorCode");
                GetRoutes();
            }
        }

        public int TblMarkerType
        {
            get { return _tblMarkerType; }
            set
            {
                _tblMarkerType = value;
                RaisePropertyChanged("TblMarkerType");
            }
        }

        public PagedCollectionView MarkerDetailsPagedCollectionView
        {
            get { return _markerDetailsPagedCollectionView; }
            set
            {
                _markerDetailsPagedCollectionView = value;
                RaisePropertyChanged("MarkerDetailsPagedCollectionView");
            }
        }

        public ObservableCollection<MarkerListViewModel> MarkerListViewModelList
        {
            get { return _markerListViewModelList; }
            set
            {
                _markerListViewModelList = value;
                RaisePropertyChanged("MarkerListViewModelList");
            }
        }

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value;
                RaisePropertyChanged("VendorPerRow");
                if (VendorPerRow != null)
                {
                    VendorCode = VendorPerRow.vendor_code;
                }
            }
        }

        public ObservableCollection<RouteCardService.TblRouteGroup> RouteGroups
        {
            get { return _routeGroups; }
            set
            {
                _routeGroups = value;
                RaisePropertyChanged("RouteGroups");
            }
        }

        public ObservableCollection<RouteCardService.TblRoute> Routes
        {
            get { return _routes; }
            set
            {
                _routes = value;
                RaisePropertyChanged("Routes");
            }
        }

        // ReSharper disable once InconsistentNaming
        public int RoutGroupID
        {
            get { return _routGroupId; }
            set
            {
                _routGroupId = value;
                RaisePropertyChanged("RoutGroupID");
                GetRoutes();
                RoutGroupItem = RouteGroups.FirstOrDefault(x => x.Iserial == value);
            }
        }

        // ReSharper disable once InconsistentNaming
        public int RoutID
        {
            get { return _routId; }
            set
            {
                _routId = value;
                RaisePropertyChanged("RoutID");
            }
        }

        public RouteCardService.TblRouteGroup RoutGroupItem
        {
            get { return _routGroupItem; }
            set
            {
                _routGroupItem = value;
                RaisePropertyChanged("RoutGroupItem");
            }
        }

        public RouteCardService.TblRoute RoutItem
        {
            get { return _routeItem; }
            set
            {
                _routeItem = value;
                RaisePropertyChanged("RoutItem");
            }
        }

        public bool? Posted
        {
            get { return _posted; }
            set
            {
                _posted = value;
                RaisePropertyChanged("Posted");
            }
        }

        #endregion Properties

        private void GetRoutes()
        {
            if (VendorCode != null && RoutGroupID > -1)
            {
                var serviceClient = new RouteCardService.RouteCardServiceClient();
                serviceClient.GetRoutesAsync(null, null, RoutGroupID, VendorCode);

                serviceClient.GetRoutesCompleted += (s, ev) =>
                {
                    Routes.Clear();
                    foreach (RouteCardService.TblRoute item in ev.Result)
                    {
                        Routes.Add(item);
                    }
                    RoutItem = Routes.FirstOrDefault(x => x.Iserial == RoutID);
                };
                serviceClient.CloseAsync();
            }
        }
    }

    public class MarkerListViewModel : ViewModelBase
    {
        public MarkerListViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MeterPerSizeList = new ObservableCollection<MeterPerSize>();
                CuttingOrderlist = new ObservableCollection<CuttingOrderViewModel>();
                MeterPerSizeList.CollectionChanged += MeterPerSizeList_CollectionChanged;
                _webService.SizeCodeCompleted += (s, o) =>
                {
                    MeterPerSizeList.Clear();
                    foreach (TblSize variable in o.Result)
                    {
                        MeterPerSizeList.Add(MarkerMapper.MapToMeterPerSize(variable));
                    }
                };

                _webService.MeterPerSizeCompleted += (x, n) =>
                {
                    foreach (tbl_MarkerDetailMeterPerSize variable in n.Result)
                    {
                        MeterPerSizeList.Add(MarkerMapper.MapToMeterPerSize(variable));
                    }
                };
                _webService.GetTotalSizeConsumtionCompleted += (s, sv) =>
                {
                    foreach (CRUD_ManagerServiceSalesOrderDto variable in sv.Result)
                    {
                        TotalReq = variable.IntialQty;
                    }
                };
                _webService.MarkerSalesOrderDetailsCompleted += (s, o) =>
                {
                    FabricsList = new ObservableCollection<MarkerSalesOrderDetail>();
                    FabricColorList = new ObservableCollection<MarkerSalesOrderDetail>();
                    StyleDetailsList = new ObservableCollection<MarkerSalesOrderDetail>();
                    MarkerSalesOrderDetails = new ObservableCollection<MarkerSalesOrderDetail>();
                    StyleNo = o.Result.Select(x => x.StyleCode).FirstOrDefault();
                    SizeRange = o.Result.Select(x => x.StyleHeader_SizeGroup).FirstOrDefault();
                    foreach (MarkerSalesOrderDetail item in o.Result)
                    {
                        if (StyleDetailsList.Count(x => x.StyleColorCode == item.StyleColorCode) == 0)
                        {
                            StyleDetailsList.Add(new MarkerSalesOrderDetail().InjectFrom(item) as MarkerSalesOrderDetail);
                        }

                        if (FabricCode != null)
                        {
                            if (item.StyleColorIserial == StyleColorCode && item.FabricCode == FabricCode)
                            {
                                if (
                                    FabricColorList.Where(
                                        x => x.FabricCode == item.FabricCode && x.StyleColorCode == item.StyleColorCode)
                                        .Count(x => x.FabricColorCode == item.FabricColorCode) == 0)
                                {
                                    FabricColorList.Add(
                                        new MarkerSalesOrderDetail().InjectFrom(item) as MarkerSalesOrderDetail);
                                }
                            }
                            if (item.StyleColorIserial == StyleColorCode)
                            {
                                FabricsList.Add(new MarkerSalesOrderDetail().InjectFrom(item) as MarkerSalesOrderDetail);
                            }
                        }

                        MarkerSalesOrderDetails.Add(item);
                    }
                    if (StyleColorCode != 0)
                    {
                        if (StyleDetailsList.FirstOrDefault(x => x.StyleColorIserial == StyleColorCode)!=null)
                        {
                            StyleColorPerRow = StyleDetailsList.FirstOrDefault(x => x.StyleColorIserial == StyleColorCode);
                        }
                        
                    }

                    if (FabricColorCode != 0)
                    {
                        if (FabricColorList.FirstOrDefault(x => x.FabricColorIserial == FabricColorCode)!=null)
                        {

                            FabricColorPerRow = FabricColorList.FirstOrDefault(x => x.FabricColorIserial == FabricColorCode);
                        }

                        
                        //TotalReq = (double)FabricColorPerRow.Total;
                    }
                };
            }
        }

        private void MeterPerSizeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (MeterPerSize item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (MeterPerSize item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == "MeterPerSizeValue")
            {
                IEnumerable<string> sizes =
                    MeterPerSizeList.Where(x => x.MeterPerSizeValue > 0).Select(x => x.MeterPerSizeCode);

                _webService.GetTotalSizeConsumtionAsync(SalesOrder, new ObservableCollection<string>(sizes), FabricCode,
                    FabricColorCode, StyleColorCode);
            }
        }

        private void GetSalesOrderInfo()
        {
            _webService.MarkerSalesOrderDetailsAsync(SalesOrder);
        }

        #region Properties

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();
        private int _status;

        private string _batchNoField;

        private double _cloretteCmPerPcField;
        private ObservableCollection<CuttingOrderViewModel> _cuttingOrderList;

        private string _fabricCodeField;

        private int _fabricColorCodeField;
        private ObservableCollection<MarkerSalesOrderDetail> _fabricColorList;

        private string _fabricColorNameField;
        private MarkerSalesOrderDetail _fabricColorPerRow;
        private ObservableCollection<MarkerSalesOrderDetail> _fabricsList;

        private int _iserialField;

        private double _markerLField;

        private string _markerNoField;
        private ObservableCollection<MarkerSalesOrderDetail> _markerSalesOrderDetails;

        private int _markerTransactionHeaderField;

        private double _markerWField;
        private ObservableCollection<MeterPerSize> _meterPerSizeList;

        private string _salesOrderField;
        private TblSalesOrder _salesOrderPerRow;
        private ObservableCollection<CuttingOrderViewModel> _savedCuttingOrderlist;

        private string _sizeRangeField;

        private int _styleColorCodeField;

        private string _styleColorNameField;
        private MarkerSalesOrderDetail _styleColorPerRow;
        private ObservableCollection<MarkerSalesOrderDetail> _styleDetailsList;

        private string _styleNoField;
        private double _totalReq;

        public ObservableCollection<CuttingOrderViewModel> CuttingOrderlist
        {
            get { return _cuttingOrderList; }
            set
            {
                if ((ReferenceEquals(_cuttingOrderList, value) != true))
                {
                    _cuttingOrderList = value;
                    RaisePropertyChanged("CuttingOrderlist");
                }
            }
        }

        public ObservableCollection<CuttingOrderViewModel> SavedCuttingOrderlist
        {
            get
            {
                return _savedCuttingOrderlist ??
                       (_savedCuttingOrderlist = new ObservableCollection<CuttingOrderViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_savedCuttingOrderlist, value) != true))
                {
                    _savedCuttingOrderlist = value;
                    RaisePropertyChanged("SavedCuttingOrderlist");
                }
            }
        }

        public TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set
            {
                _salesOrderPerRow = value;
                RaisePropertyChanged("SalesOrderPerRow");
                if (SalesOrderPerRow != null) SalesOrder = SalesOrderPerRow.SalesOrderCode;
            }
        }

        public ObservableCollection<MeterPerSize> MeterPerSizeList
        {
            get { return _meterPerSizeList; }
            set
            {
                if ((ReferenceEquals(_meterPerSizeList, value) != true))
                {
                    _meterPerSizeList = value;
                    RaisePropertyChanged("MeterPerSizeList");
                }
            }
        }

        public ObservableCollection<MarkerSalesOrderDetail> MarkerSalesOrderDetails
        {
            get { return _markerSalesOrderDetails; }
            set
            {
                _markerSalesOrderDetails = value;
                RaisePropertyChanged("MarkerSalesOrderDetails");
            }
        }

        public ObservableCollection<MarkerSalesOrderDetail> FabricsList
        {
            get { return _fabricsList; }
            set
            {
                _fabricsList = value;
                RaisePropertyChanged("FabricsList");
            }
        }

        public ObservableCollection<MarkerSalesOrderDetail> FabricColorList
        {
            get { return _fabricColorList; }
            set
            {
                _fabricColorList = value;
                RaisePropertyChanged("FabricColorList");
            }
        }

        public ObservableCollection<MarkerSalesOrderDetail> StyleDetailsList
        {
            get { return _styleDetailsList; }
            set
            {
                _styleDetailsList = value;
                RaisePropertyChanged("StyleDetailsList");
            }
        }

        public MarkerSalesOrderDetail FabricColorPerRow
        {
            get { return _fabricColorPerRow; }
            set
            {
                _fabricColorPerRow = value;
                RaisePropertyChanged("FabricColorPerRow");
            }
        }

        public MarkerSalesOrderDetail StyleColorPerRow
        {
            get { return _styleColorPerRow; }
            set
            {
                _styleColorPerRow = value;
                RaisePropertyChanged("StyleColorPerRow");
                if (_styleColorPerRow != null)
                {
                    if (MarkerNo == null)
                    {
                        MarkerNo = _styleColorPerRow.StyleCode;
                    }
                }
            }
        }

        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public string SizeRange
        {
            get { return _sizeRangeField; }
            set
            {
                if ((ReferenceEquals(_sizeRangeField, value) != true))
                {
                    _sizeRangeField = value;
                    if (value != null && Iserial == 0)
                    {
                        _webService.SizeCodeAsync(_sizeRangeField);
                    }

                    RaisePropertyChanged("SizeRange");
                }
            }
        }

        public int StyleColorCode
        {
            get { return _styleColorCodeField; }
            set
            {
                if ((Equals(_styleColorCodeField, value) != true))
                {
                    _styleColorCodeField = value;
                    RaisePropertyChanged("StyleColorCode");

                    if (StyleColorCode != 0)
                    {
                        if (MarkerSalesOrderDetails != null)
                        {
                            foreach (
                                MarkerSalesOrderDetail item in
                                    MarkerSalesOrderDetails.Where(x => x.StyleColorIserial == StyleColorCode))
                            {
                                if (FabricsList.Count(x => x.FabricCode == item.FabricCode) == 0)
                                {
                                    FabricsList.Add(
                                        new MarkerSalesOrderDetail().InjectFrom(item) as MarkerSalesOrderDetail);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string FabricCode
        {
            get { return _fabricCodeField; }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");

                    if (MarkerSalesOrderDetails != null && !string.IsNullOrEmpty(FabricCode))
                    {
                        foreach (
                            MarkerSalesOrderDetail item in
                                MarkerSalesOrderDetails.Where(
                                    x => x.StyleColorIserial == StyleColorCode && x.FabricCode == FabricCode))
                        {
                            if (FabricColorList.Count(x => x.FabricColorCode == item.FabricColorCode) == 0)
                            {
                                FabricColorList.Add(
                                    new MarkerSalesOrderDetail().InjectFrom(item) as MarkerSalesOrderDetail);
                            }
                        }
                    }
                }
            }
        }

        public string StyleColorName
        {
            get { return _styleColorNameField; }
            set
            {
                if ((ReferenceEquals(_styleColorNameField, value) != true))
                {
                    _styleColorNameField = value;
                    RaisePropertyChanged("StyleColorName");
                }
            }
        }

        public string StyleNo
        {
            get { return _styleNoField; }
            set
            {
                if ((ReferenceEquals(_styleNoField, value) != true))
                {
                    _styleNoField = value;
                    RaisePropertyChanged("StyleNo");
                }
            }
        }

        public double TotalReq
        {
            get { return _totalReq; }
            set
            {
                _totalReq = value;
                RaisePropertyChanged("TotalReq");
            }
        }

        public string BatchNo
        {
            get { return _batchNoField; }
            set
            {
                if ((ReferenceEquals(_batchNoField, value) != true))
                {
                    _batchNoField = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }

        public double CloretteCmPerPc
        {
            get { return _cloretteCmPerPcField; }
            set
            {
                if ((_cloretteCmPerPcField.Equals(value) != true))
                {
                    _cloretteCmPerPcField = value;
                    RaisePropertyChanged("CloretteCmPerPc");
                }
            }
        }

        public string FabricColorName
        {
            get { return _fabricColorNameField; }
            set
            {
                if ((ReferenceEquals(_fabricColorNameField, value) != true))
                {
                    _fabricColorNameField = value;
                    RaisePropertyChanged("FabricColorName");
                }
            }
        }

        public int FabricColorCode
        {
            get { return _fabricColorCodeField; }
            set
            {
                if ((Equals(_fabricColorCodeField, value) != true))
                {
                    _fabricColorCodeField = value;
                    RaisePropertyChanged("FabricColorCode");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;

                    if (Iserial != 0)
                    {
                        _webService.MeterPerSizeAsync(Iserial);
                    }
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public double MarkerL
        {
            get { return _markerLField; }
            set
            {
                if ((_markerLField.Equals(value) != true))
                {
                    _markerLField = value;
                    RaisePropertyChanged("MarkerL");
                }
            }
        }

        public string MarkerNo
        {
            get { return _markerNoField; }
            set
            {
                if ((ReferenceEquals(_markerNoField, value) != true))
                {
                    _markerNoField = value;
                    RaisePropertyChanged("MarkerNo");
                }
            }
        }

        public int MarkerTransactionHeader
        {
            get { return _markerTransactionHeaderField; }
            set
            {
                if ((_markerTransactionHeaderField.Equals(value) != true))
                {
                    _markerTransactionHeaderField = value;
                    RaisePropertyChanged("MarkerTransactionHeader");
                }
            }
        }

        public double MarkerW
        {
            get { return _markerWField; }
            set
            {
                if ((_markerWField.Equals(value) != true))
                {
                    _markerWField = value;
                    RaisePropertyChanged("MarkerW");
                }
            }
        }

        public string SalesOrder
        {
            get { return _salesOrderField; }
            set
            {
                if ((ReferenceEquals(_salesOrderField, value) != true))
                {
                    _salesOrderField = value;
                    if (_salesOrderField != null)
                    {
                        GetSalesOrderInfo();
                    }
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }

        #endregion Properties
    }

    public class FabricsViewModel : PropertiesViewModelBase
    {
        private string _fabricCodeField;

        private string _fabricNameField;

        private string _salesOrderFiled;

        public string FabricCode
        {
            get { return _fabricCodeField; }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");
                }
            }
        }

        public string FabricName
        {
            get { return _fabricNameField; }
            set
            {
                if ((ReferenceEquals(_fabricNameField, value) != true))
                {
                    _fabricNameField = value;
                    RaisePropertyChanged("FabricName");
                }
            }
        }

        public string SalesOrder
        {
            get { return _salesOrderFiled; }
            set
            {
                if ((ReferenceEquals(_salesOrderFiled, value) != true))
                {
                    _salesOrderFiled = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }
    }

    public class FabricsColorsViewModel : PropertiesViewModelBase
    {
        private string _fabricColorCodeField;

        private string _fabricColorNameField;

        private string _salesOrderFileds;

        public string FabricColorCode
        {
            get { return _fabricColorCodeField; }
            set
            {
                if ((ReferenceEquals(_fabricColorCodeField, value) != true))
                {
                    _fabricColorCodeField = value;
                    RaisePropertyChanged("FabricColorCode");
                }
            }
        }

        public string FabricColorName
        {
            get { return _fabricColorNameField; }
            set
            {
                if ((ReferenceEquals(_fabricColorNameField, value) != true))
                {
                    _fabricColorNameField = value;
                    RaisePropertyChanged("FabricColorName");
                }
            }
        }

        public string SalesOrder
        {
            get { return _salesOrderFileds; }
            set
            {
                if ((ReferenceEquals(_salesOrderFileds, value) != true))
                {
                    _salesOrderFileds = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }
    }

    public class TblMarkerTempViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblMarkerTempViewModel> _detailsList;

        private int _iserialField;

        private string _markerNoField;
        private int _noOfLayers;
        private int _noOfLayersOrg;

        private double? _productionField;

        private double? _ratioField;

        private double? _remField;

        private string _sizeField;
        private Visibility _sizeVisibile;

        private TblColor _tblColor1Field;
        private int? _tblColorField;

        public ObservableCollection<TblMarkerTempViewModel> DetailsList
        {
            get { return _detailsList; }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string MarkerNo
        {
            get { return _markerNoField; }
            set
            {
                if ((ReferenceEquals(_markerNoField, value) != true))
                {
                    _markerNoField = value;
                    RaisePropertyChanged("MarkerNo");
                }
            }
        }

        public double? Production
        {
            get { return _productionField; }
            set
            {
                if ((_productionField.Equals(value) != true))
                {
                    _productionField = value;
                    RaisePropertyChanged("Production");
                }
            }
        }

        public double? Ratio
        {
            get { return _ratioField; }
            set
            {
                if ((_ratioField.Equals(value) != true))
                {
                    _ratioField = value;
                    RaisePropertyChanged("Ratio");
                }
            }
        }

        public double? Rem
        {
            get { return _remField; }
            set
            {
                if ((_remField.Equals(value) != true))
                {
                    _remField = value;
                    RaisePropertyChanged("Rem");
                }
            }
        }

        public string Size
        {
            get { return _sizeField; }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true))
                {
                    _sizeField = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        public int? TblColor
        {
            get { return _tblColorField; }
            set
            {
                if ((_tblColorField.Equals(value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public TblColor TblColor1
        {
            get { return _tblColor1Field; }
            set
            {
                if ((ReferenceEquals(_tblColor1Field, value) != true))
                {
                    _tblColor1Field = value;
                    RaisePropertyChanged("TblColor1");
                }
            }
        }

        public Visibility SizeVisible
        {
            get { return _sizeVisibile; }
            set
            {
                _sizeVisibile = value;
                RaisePropertyChanged("SizeVisible");
            }
        }

        public int NoOfLayers
        {
            get { return _noOfLayers; }
            set
            {
                _noOfLayers = value;
                RaisePropertyChanged("NoOfLayers");
            }
        }

        public int NoOfLayersOrg
        {
            get { return _noOfLayersOrg; }
            set
            {
                _noOfLayersOrg = value;
                RaisePropertyChanged("NoOfLayersOrg");
            }
        }
    }

    public class StyleViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<MeterPerSize> _meterPerSizes;
        private int _qty;
        private string _salesOrderFiled;
        private string _stylEnameField;

        private string _styleCodeField;

        private string _styleColorCodeField;

        private string _styleColorNameField;

        private string _styleHeaderSizeGroupField;

        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                RaisePropertyChanged("Qty");
            }
        }

        public ObservableCollection<MeterPerSize> DetailsList
        {
            get { return _meterPerSizes; }
            set
            {
                _meterPerSizes = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        public string StyleCode
        {
            get { return _styleCodeField; }
            set
            {
                if ((ReferenceEquals(_styleCodeField, value) != true))
                {
                    _styleCodeField = value;
                    RaisePropertyChanged("StyleCode");
                }
            }
        }

        public string StyleColorCode
        {
            get { return _styleColorCodeField; }
            set
            {
                if ((ReferenceEquals(_styleColorCodeField, value) != true))
                {
                    _styleColorCodeField = value;
                    RaisePropertyChanged("StyleColorCode");
                }
            }
        }

        public string StyleColorName
        {
            get { return _styleColorNameField; }
            set
            {
                if ((ReferenceEquals(_styleColorNameField, value) != true))
                {
                    _styleColorNameField = value;
                    RaisePropertyChanged("StyleColorName");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public string StyleHeader_SizeGroup
        {
            get { return _styleHeaderSizeGroupField; }
            set
            {
                if ((ReferenceEquals(_styleHeaderSizeGroupField, value) != true))
                {
                    _styleHeaderSizeGroupField = value;
                    RaisePropertyChanged("StyleHeader_SizeGroup");
                }
            }
        }

        public string StylEname
        {
            get { return _stylEnameField; }
            set
            {
                if ((ReferenceEquals(_stylEnameField, value) != true))
                {
                    _stylEnameField = value;
                    RaisePropertyChanged("StylEname");
                }
            }
        }

        public string SalesOrder
        {
            get { return _salesOrderFiled; }
            set
            {
                if ((ReferenceEquals(_salesOrderFiled, value) != true))
                {
                    _salesOrderFiled = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }
    }

    public class MarkerViewModel : ViewModelBase
    {

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();
        private ObservableCollection<TblSalesOrderColor> _salesOrderColor;
        private TblSalesOrder _salesOrderPerRow;

        public MarkerViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                CuttingSelectionItems = new List<string> { "C", "M" };
                GetItemPermissions(PermissionItemName.MarkerDetailsForm.ToString());
                _webService.GetSalesOrderColorsCompleted += (s, sv) => { SalesOrderColor = sv.Result; };

                _webService.GetGenericCompleted += (s, sv) =>
                {
                    MarkerTypeList = sv.Result;
                };

                _webService.GetGenericAsync("TblMarkerType", "%%", "%%", "%%", "Iserial", "ASC");

                WareHouseList = new ObservableCollection<TblWarehouse>();
                _webService.GetTblWarehouseAsync(0, int.MaxValue, "it.Iserial", null, null);
                _webService.GetTblWarehouseCompleted += (s, w) =>
                {
                    WareHouseList = w.Result;
                };

                _webService.InspectionCompleted += (s, ev) =>
                {
                    SelectedMarker.CuttingOrderlist.Clear();
                    foreach (Inspection item in ev.Result)
                    {
                        SelectedMarker.CuttingOrderlist.Add(MarkerMapper.MapToCuttingOrder(item, SelectedMarker.Iserial));
                    }
                };

                SalesOrderList = new ObservableCollection<string>();

                MarkerHeader = new MarkerHeaderListViewModel { TransDate = DateTime.Today };

                ChaingSetupMethod.GetSettings("Marker", _webService);

                _webService.GetChainSetupCompleted += (a, s) =>
                {
                    foreach (tblChainSetup item in s.Result)
                    {
                        if (item.sGlobalSettingCode == "DefaultWorkstation")
                        {
                            MarkerHeader.RoutID = int.Parse(item.sSetupValue);
                        }
                    }
                };
                _webService.SaveTransactionHeaderCompleted += (s, w) =>
                {
                    MarkerHeader = MarkerMapper.MapToViewModel(w.Result, MarkerHeader.RouteGroups);

                    FillallMarkers();
                };

                _webService.GetMarkerTransactionHeadersListCompleted += (d, i) =>
                {
                    foreach (tbl_MarkerTransactionHeader item in i.Result)
                    {
                        MarkerHeaderList.Add(MarkerMapper.MapToViewModel(item, MarkerHeader.RouteGroups));
                    }
                };

                _webService.SavingCuttingOrderCompleted += (s, sv) =>
                {
                    Tbl_Wf_CuttingOrder row = sv.Result;
                    CuttingOrderViewModel savedRow = SelectedMarker.SavedCuttingOrderlist.FirstOrDefault(c =>
                        c.MarkerIserial == row.MarkerIserial && c.InspectionIserial == row.InspectionIserial);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(row);
                        //SelectedMarker.SavedCuttingOrderlist.FirstOrDefault(c => c.MarkerHeaderIserial == row.MarkerHeaderTransaction&& c.MarkerIserial == row.MarkerIserial&& c.InspectionIserial == row.InspectionIserial) = MarkerMapper.MapToCuttingOrder(row);
                    }

                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                };
                _webService.InsertPickingListCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                    else
                    {
                        MessageBox.Show("Route Created");
                    }
                };

                Client.SaveMarkerTempCompleted += (s, sv) => MessageBox.Show(sv.Result == 0
                    ? "Fabric Qty Is Less Than Required ! please Modify the No Of Layers"
                    : "Done");
            }
        }
        private ObservableCollection<GenericTable> _markerTypeList;
        public ObservableCollection<GenericTable> MarkerTypeList
        {
            get { return _markerTypeList; }
            set { _markerTypeList = value; RaisePropertyChanged("MarkerTypeList"); }
        }

        public ObservableCollection<TblSalesOrderColor> SalesOrderColor
        {
            get { return _salesOrderColor; }
            set
            {
                _salesOrderColor = value;
                RaisePropertyChanged("SalesOrderColor");
            }
        }

        public List<string> CuttingSelectionItems { get; set; }

        public TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set
            {
                _salesOrderPerRow = value;
                RaisePropertyChanged("SalesOrderPerRow");
            }
        }

        public event EventHandler SubmitSearchAction;

        public void SearchHeader()
        {
            if (MarkerHeaderList != null) MarkerHeaderList.Clear();

            if (SalesOrderPerRow != null) _webService.GetMarkerTransactionHeadersListAsync(SalesOrderPerRow.Iserial);
        }

        public void SaveCuttingOrder(CuttingOrderViewModel order)
        {
            var cuttingOrder = new Tbl_Wf_CuttingOrder();
            if (order != null)
            {
                cuttingOrder.InjectFrom(order);
                cuttingOrder.MarkerHeaderTransaction = MarkerHeader.Iserial;
                cuttingOrder.MarkerIserial = order.MarkerIserial;
                cuttingOrder.RollUnit = order.RollUnit;

                if (!string.IsNullOrWhiteSpace(cuttingOrder.CuttingSelection) && cuttingOrder.RollAssignedQty > 0)
                {
                    _webService.SavingCuttingOrderAsync(cuttingOrder);
                }
                else
                {
                    MessageBox.Show("Data Is Not Valid For Saving");
                }
            }
        }

        public void FillCuttingOrder()
        {
            _webService.InspectionAsync(SelectedMarker.Iserial);
        }

        public void DeleteMarkerDetails(MarkerListViewModel markerLine)
        {
            _webService.DeleteMarkerAsync(markerLine.Iserial);

            _webService.DeleteMarkerCompleted += (m, s) => MarkerHeader.MarkerListViewModelList.Remove(markerLine);
        }

        public void PickingList()
        {
            _webService.InsertPickingListAsync(MarkerHeader.Iserial, LoggedUserInfo.Iserial);
        }

        public void GetSalesOrderColor()
        {
            _webService.GetSalesOrderColorsAsync(0, int.MaxValue, 0, SalesOrderIserial, 0, 0, false, LoggedUserInfo.Iserial);
        }

        public void SavingHeader()
        {
            tbl_MarkerTransactionHeader temp = MarkerMapper.MapToViewModel(MarkerHeader);
            if (temp.tbl_MarkerDetail.Where(x => x.MarkerNo != null).Select(x => x.SalesOrder).Distinct().Count() > 1)
            {
                MessageBox.Show("More Than 1 SalesOrder in that transaction Please Review The Data");

                return;
            }

            _webService.SaveTransactionHeaderAsync(temp);
        }

        public void DeleteTransaction()
        {
            _webService.DeleteMarkerTransactionAsync(MarkerHeader.Iserial);
        }

        public void FillallMarkers()
        {
            _webService.MarkerDetailsCompleted += (s, m) =>
            {
                MarkerHeader.MarkerListViewModelList.Clear();
                SalesOrderIserial = m.SalesOrder.Iserial;
                foreach (tbl_MarkerDetail item in m.Result)
                {
                    MarkerHeader.MarkerListViewModelList.Add(MarkerMapper.MapToMarkerDetail(item));
                }
                MarkerHeader.MarkerListViewModelList.Add(new MarkerListViewModel());
                SubmitSearchAction.Invoke(this, null);
            };
            _webService.MarkerDetailsAsync(MarkerHeader.Iserial);
        }

        public void DeleteCutting(CuttingOrderViewModel row)
        {
            var newrow = new Tbl_Wf_CuttingOrder();
            newrow.InjectFrom(row);
            _webService.DeleteCuttingOrderAsync(newrow);
        }

        public void SaveCuttingTemp()
        {
            var firstrow = CalcList.FirstOrDefault(x => x.MarkerNo == "Production Ratio");
            var temp = new ObservableCollection<TblMarkerTemp>();
            foreach (var parent in CalcList.Where(x => x.MarkerNo != "Production Ratio"))
            {
                foreach (var row in parent.DetailsList)
                {
                    if (firstrow != null)
                        temp.Add(new TblMarkerTemp
                        {
                            TblColor = firstrow.TblColor,
                            MarkerNo = parent.MarkerNo,
                            Size = row.Size,
                            Rem = row.Rem,
                            Ratio = row.Ratio,
                            Production = row.Production,
                            NoOfLayersOrg = parent.NoOfLayersOrg,
                            NoOfLayers = parent.NoOfLayers,
                            MarkerTransactionHeader = MarkerHeader.Iserial
                        });
                }
            }

            Client.SaveMarkerTempAsync(temp);
        }

        #region Setting the Properties and Fields For The Marker Details Table

        private ObservableCollection<TblMarkerTempViewModel> _calcList;
        private MarkerHeaderListViewModel _markerHeader;
        private ObservableCollection<MarkerHeaderListViewModel> _markerHeaderList;
        private ObservableCollection<string> _salesOrderList;

        private int _salesorderIserial;
        private MarkerListViewModel _selectedMarker;
        private ObservableCollection<TblWarehouse> _wareHouseList;
        private ObservableCollection<WareHouseDto> _wareHouseWithOnHandList;


        public ObservableCollection<WareHouseDto> WarehouseWithOnHandList
        {
            get { return _wareHouseWithOnHandList; }
            set
            {
                _wareHouseWithOnHandList = value;
                RaisePropertyChanged("WarehouseWithOnHandList");
            }
        }

        public ObservableCollection<TblWarehouse> WareHouseList
        {
            get { return _wareHouseList; }
            set
            {
                _wareHouseList = value;
                RaisePropertyChanged("WareHouseList");
            }
        }

        public MarkerHeaderListViewModel MarkerHeader
        {
            get { return _markerHeader; }
            set
            {
                _markerHeader = value;
                RaisePropertyChanged("MarkerHeader");
            }
        }

        public ObservableCollection<MarkerHeaderListViewModel> MarkerHeaderList
        {
            get
            {
                return _markerHeaderList ?? (_markerHeaderList = new ObservableCollection<MarkerHeaderListViewModel>());
            }
            set
            {
                _markerHeaderList = value;
                RaisePropertyChanged("MarkerHeaderList");
            }
        }

        public ObservableCollection<TblMarkerTempViewModel> CalcList
        {
            get { return _calcList ?? (_calcList = new ObservableCollection<TblMarkerTempViewModel>()); }
            set
            {
                _calcList = value;
                RaisePropertyChanged("CalcList");
            }
        }

        public MarkerListViewModel SelectedMarker
        {
            get { return _selectedMarker; }
            set
            {
                _selectedMarker = value;
                RaisePropertyChanged("SelectedMarker");
            }
        }


        public int SalesOrderIserial
        {
            get { return _salesorderIserial; }
            set
            {
                _salesorderIserial = value;
                RaisePropertyChanged("SalesOrderIserial");
            }
        }

        public ObservableCollection<string> SalesOrderList
        {
            get { return _salesOrderList; }
            set
            {
                _salesOrderList = value;
                RaisePropertyChanged("SalesOrder");
            }
        }

        #endregion Setting the Properties and Fields For The Marker Details Table
    }

    public class SizeInfo : PropertiesViewModelBase
    {
        private int? _sizeCodeIdField;
        private string _sizeCodeSizeCodeField;

        public int? SizeCodeId
        {
            get { return _sizeCodeIdField; }
            set
            {
                if ((_sizeCodeIdField.Equals(value) != true))
                {
                    _sizeCodeIdField = value;
                    RaisePropertyChanged("SizeCode_IdBate5");
                }
            }
        }

        public string SizeCodeSizeCode
        {
            get { return _sizeCodeSizeCodeField; }
            set
            {
                if ((ReferenceEquals(_sizeCodeSizeCodeField, value) != true))
                {
                    _sizeCodeSizeCodeField = value;
                    RaisePropertyChanged("SizeCode_SizeCode");
                }
            }
        }
    }

    public class MeterPerSize : PropertiesViewModelBase
    {
        private int _iserialField;

        private string _meterPerSizeCodeField;

        private double _meterPerSizeValueField;
        private Visibility _sizeVisibile;

        private int _sizecodeId;

        public int SizecodeId
        {
            get { return _sizecodeId; }
            set
            {
                if ((_sizecodeId.Equals(value) != true))
                {
                    _sizecodeId = value;
                    RaisePropertyChanged("SizecodeId");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string MeterPerSizeCode
        {
            get { return _meterPerSizeCodeField; }
            set
            {
                if ((ReferenceEquals(_meterPerSizeCodeField, value) != true))
                {
                    _meterPerSizeCodeField = value;
                    RaisePropertyChanged("MeterPerSizeCode");
                }
            }
        }

        public double MeterPerSizeValue
        {
            get { return _meterPerSizeValueField; }
            set
            {
                if ((_meterPerSizeValueField.Equals(value) != true))
                {
                    _meterPerSizeValueField = value;
                    RaisePropertyChanged("MeterPerSizeValue");
                }
            }
        }

        public Visibility SizeVisible
        {
            get { return _sizeVisibile; }
            set
            {
                _sizeVisibile = value;
                RaisePropertyChanged("SizeVisible");
            }
        }
    }

    public class CuttingOrderViewModel : PropertiesViewModelBase
    {

        private string _wareHouse;

        public string Warehouse
        {
            get { return _wareHouse; }
            set { _wareHouse = value; RaisePropertyChanged("Warehouse"); }
        }

        private string _site;

        public string Site
        {
            get { return _site; }
            set { _site = value; RaisePropertyChanged("Site"); }
        }


        private string _barcode;
        private string _batchNoCutttingField;

        private string _colorCodeCuttingOrderField;
        private float _consPerpcField;
        private string _cuttingSelectionField;

        private string _fabricCodeField;
        private int _inspectionIserialField;
        private float? _m2WeightGmField;
        private int _markerHeaderIserialField;
        private int _markerIserialField;

        private float? _netRollWmtField;
        private float _noOfpcsField;

        private int? _productCategoryField;
        private double _rollAssignedQtyField;

        private int _rollNoField;
        private string _rollUnitField;

        private float? _rollWMtField;

        private float _storeRollQtyField;

        private string _unitField;

        public int MarkerIserial
        {
            get { return _markerIserialField; }
            set
            {
                if ((Equals(_markerIserialField, value) != true))
                {
                    _markerIserialField = value;
                    RaisePropertyChanged("MarkerIserial");
                }
            }
        }

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                if ((ReferenceEquals(_barcode, value) != true))
                {
                    _barcode = value;
                    RaisePropertyChanged("Barcode");
                }
            }
        }

        public int MarkerHeaderIserial
        {
            get { return _markerHeaderIserialField; }
            set
            {
                if ((Equals(_markerHeaderIserialField, value) != true))
                {
                    _markerHeaderIserialField = value;
                    RaisePropertyChanged("MarkerHeaderIserial");
                }
            }
        }

        public string BatchNoCuttting
        {
            get { return _batchNoCutttingField; }
            set
            {
                if ((ReferenceEquals(_batchNoCutttingField, value) != true))
                {
                    _batchNoCutttingField = value;
                    RaisePropertyChanged("BatchNoCutttingField");
                }
            }
        }

        public string ColorCodeCuttingOrder
        {
            get { return _colorCodeCuttingOrderField; }
            set
            {
                if ((ReferenceEquals(_colorCodeCuttingOrderField, value) != true))
                {
                    _colorCodeCuttingOrderField = value;
                    RaisePropertyChanged("ColorCode");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public string Fabric_Code
        {
            get { return _fabricCodeField; }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("Fabric_Code");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public float? NetRollWMT
        {
            get { return _netRollWmtField; }
            set
            {
                if ((_netRollWmtField.Equals(value) != true))
                {
                    _netRollWmtField = value;
                    RaisePropertyChanged("NetRollWMT");
                }
            }
        }

        public int? ProductCategory
        {
            get { return _productCategoryField; }
            set
            {
                if ((ReferenceEquals(_productCategoryField, value) != true))
                {
                    _productCategoryField = value;
                    RaisePropertyChanged("ProductCategory");
                }
            }
        }

        public int RollNo
        {
            get { return _rollNoField; }
            set
            {
                if ((Equals(_rollNoField, value) != true))
                {
                    _rollNoField = value;
                    RaisePropertyChanged("RollNo");
                }
            }
        }

        public float? RollWMt
        {
            get { return _rollWMtField; }
            set
            {
                if ((_rollWMtField.Equals(value) != true))
                {
                    _rollWMtField = value;
                    RaisePropertyChanged("RollWMt");
                }
            }
        }

        public float StoreRollQty
        {
            get { return _storeRollQtyField; }
            set
            {
                if ((_storeRollQtyField.Equals(value) != true))
                {
                    _storeRollQtyField = value;
                    RaisePropertyChanged("StoreRollQty");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public float consPerpc
        {
            get { return _consPerpcField; }
            set
            {
                if ((_consPerpcField.Equals(value) != true))
                {
                    _consPerpcField = value;
                    RaisePropertyChanged("consPerpc");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public float? m2WeightGm
        {
            get { return _m2WeightGmField; }
            set
            {
                if ((_m2WeightGmField.Equals(value) != true))
                {
                    _m2WeightGmField = value;
                    RaisePropertyChanged("m2WeightGm");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public float noOfpcs
        {
            get { return _noOfpcsField; }
            set
            {
                if ((_noOfpcsField.Equals(value) != true))
                {
                    _noOfpcsField = value;
                    RaisePropertyChanged("noOfpcs");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public string unit
        {
            get { return _unitField; }
            set
            {
                if ((ReferenceEquals(_unitField, value) != true))
                {
                    _unitField = value;
                    RaisePropertyChanged("unit");
                }
            }
        }

        public int InspectionIserial
        {
            get { return _inspectionIserialField; }
            set
            {
                if ((_inspectionIserialField.Equals(value) != true))
                {
                    _inspectionIserialField = value;
                    RaisePropertyChanged("InspectionIserial");
                }
            }
        }

        public double RollAssignedQty
        {
            get { return _rollAssignedQtyField; }
            set
            {
                if ((_rollAssignedQtyField.Equals(value) != true))
                {
                    _rollAssignedQtyField = value;
                    RaisePropertyChanged("RollAssignedQty");
                }
            }
        }

        public string RollUnit
        {
            get { return _rollUnitField; }
            set
            {
                if ((ReferenceEquals(_rollUnitField, value) != true))
                {
                    _rollUnitField = value;
                    RaisePropertyChanged("RollUnit");
                }
            }
        }

        public string CuttingSelection
        {
            get { return _cuttingSelectionField; }
            set
            {
                if ((ReferenceEquals(_cuttingSelectionField, value) != true))
                {
                    _cuttingSelectionField = value;
                    RaisePropertyChanged("CuttingSelection");
                }
            }
        }
    }
}