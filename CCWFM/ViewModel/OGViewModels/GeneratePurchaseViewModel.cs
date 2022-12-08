using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.PurchasePlanService;
using CCWFM.CRUDManagerService;
using System.Windows.Media;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblPurchaseReceiveDetailModel : PurchasePlanService.PropertiesViewModelBase
    {

        private string _batchNo;

        public string BatchNo
        {
            get { return _batchNo; }
            set { _batchNo = value; RaisePropertyChanged("BatchNo"); }
        }



        private double? _costField;

        private int _iserialField;

        private double? _oldCostField;

        private double? _qtyField;

        private int? _tblPurchaseOrderDetailField;

        private int? _tblPurchaseReceiveHeaderField;
        private PurchasePlanService.TblPurchaseOrderDetailRequest _myVar;

        public PurchasePlanService.TblPurchaseOrderDetailRequest TblPurchaseOrderDetailRequest1
        {
            get { return _myVar; }
            set { _myVar = value; RaisePropertyChanged("TblPurchaseOrderDetailRequest1"); }
        }

        

        public double? Cost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public double? OldCost
        {
            get
            {
                return _oldCostField;
            }
            set
            {
                if ((_oldCostField.Equals(value) != true))
                {
                    _oldCostField = value;
                    RaisePropertyChanged("OldCost");
                }
            }
        }
        
        public double? Qty
        {
            get
            {
                return _qtyField;
            }
            set
            {
                if ((_qtyField.Equals(value) != true))
                {
                    _qtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        public int? TblPurchaseOrderDetail
        {
            get
            {
                return _tblPurchaseOrderDetailField;
            }
            set
            {
                if ((_tblPurchaseOrderDetailField.Equals(value) != true))
                {
                    _tblPurchaseOrderDetailField = value;
                    RaisePropertyChanged("TblPurchaseOrderDetail");
                }
            }
        }

        public int? TblPurchaseReceiveHeader
        {
            get
            {
                return _tblPurchaseReceiveHeaderField;
            }
            set
            {
                if ((_tblPurchaseReceiveHeaderField.Equals(value) != true))
                {
                    _tblPurchaseReceiveHeaderField = value;
                    RaisePropertyChanged("TblPurchaseReceiveHeader");
                }
            }
        }
    }

    public class TblPurchaseReceiveHeaderModel : PurchasePlanService.PropertiesViewModelBase
    {
        private CRUDManagerService.TblWarehouse _warehousePerRow;

        public CRUDManagerService.TblWarehouse WarehousePerRow
        {
            get { return _warehousePerRow; }
            set
            {
                _warehousePerRow = value; RaisePropertyChanged("WarehousePerRow");
                
            }
        }

        private string _createdByField;

        private DateTime? _creationDateField;

        private string _docCodeField;

        private DateTime? _docDateField;

        private int _iserialField;

        private string _lastUpdatedByField;

        private DateTime? _lastUpdatedDateField;

        private string _notesField;

        private string _refNoField;

        private int? _tblPurchaseOrderHeaderField;

        private ObservableCollection<PurchasePlanService.TblPurchaseReceiveDetail> _tblPurchaseReceiveDetailsField;

        private string _vendorField;

        public string CreatedBy
        {
            get
            {
                return _createdByField;
            }
            set
            {
                if ((ReferenceEquals(_createdByField, value) != true))
                {
                    _createdByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public string DocCode
        {
            get
            {
                return _docCodeField;
            }
            set
            {
                if ((ReferenceEquals(_docCodeField, value) != true))
                {
                    _docCodeField = value;
                    RaisePropertyChanged("DocCode");
                }
            }
        }

        public DateTime? DocDate
        {
            get
            {
                return _docDateField;
            }
            set
            {
                if ((_docDateField.Equals(value) != true))
                {
                    _docDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string LastUpdatedBy
        {
            get
            {
                return _lastUpdatedByField;
            }
            set
            {
                if ((ReferenceEquals(_lastUpdatedByField, value) != true))
                {
                    _lastUpdatedByField = value;
                    RaisePropertyChanged("LastUpdatedBy");
                }
            }
        }

        public DateTime? LastUpdatedDate
        {
            get
            {
                return _lastUpdatedDateField;
            }
            set
            {
                if ((_lastUpdatedDateField.Equals(value) != true))
                {
                    _lastUpdatedDateField = value;
                    RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }

        public string Notes
        {
            get
            {
                return _notesField;
            }
            set
            {
                if ((ReferenceEquals(_notesField, value) != true))
                {
                    _notesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public string RefNo
        {
            get
            {
                return _refNoField;
            }
            set
            {
                if ((ReferenceEquals(_refNoField, value) != true))
                {
                    _refNoField = value;
                    RaisePropertyChanged("RefNo");
                }
            }
        }

        public int TblInventType { get; set; }

        private int _tblWarehouse;

        public int TblWarehouse
        {
            get { return _tblWarehouse; }
            set { _tblWarehouse = value; RaisePropertyChanged("TblWarehouse"); }
        }

        public int? TblPurchaseOrderHeaderRequest
        {
            get
            {
                return _tblPurchaseOrderHeaderField;
            }
            set
            {
                if ((_tblPurchaseOrderHeaderField.Equals(value) != true))
                {
                    _tblPurchaseOrderHeaderField = value;
                    RaisePropertyChanged("TblPurchaseOrderHeaderRequest");
                }
            }
        }

        private ObservableCollection<TblPurchaseReceiveDetailModel> _detailList;

        public ObservableCollection<TblPurchaseReceiveDetailModel> DetailList
        {
            get
            {
                return _detailList ?? (_detailList = new ObservableCollection<TblPurchaseReceiveDetailModel>());
            }
            set
            {
                if ((ReferenceEquals(_detailList, value) != true))
                {
                    _detailList = value;
                    RaisePropertyChanged("DetailList");
                }
            }
        }

        public ObservableCollection<PurchasePlanService.TblPurchaseReceiveDetail> TblPurchaseReceiveDetails
        {
            get
            {
                return _tblPurchaseReceiveDetailsField;
            }
            set
            {
                if ((ReferenceEquals(_tblPurchaseReceiveDetailsField, value) != true))
                {
                    _tblPurchaseReceiveDetailsField = value;
                    RaisePropertyChanged("TblPurchaseReceiveDetails");
                }
            }
        }

        public string Vendor
        {
            get
            {
                return _vendorField;
            }
            set
            {
                if ((ReferenceEquals(_vendorField, value) != true))
                {
                    _vendorField = value;
                    RaisePropertyChanged("Vendor");
                }
            }
        }
    }

    public class TblGeneratePurchaseHeaderModel : PurchasePlanService.PropertiesViewModelBase
    {
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("Code"); }
        }

        private int _tblPurchaseType;

        public int TblPurchaseType
        {
            get { return _tblPurchaseType; }
            set { _tblPurchaseType = value; RaisePropertyChanged("TblPurchaseType"); }
        }

        private int? _tblFactoryDelivery;

        public int? TblFactoryDelivery
        {
            get { return _tblFactoryDelivery; }
            set { _tblFactoryDelivery = value; RaisePropertyChanged("TblFactoryDelivery"); }
        }

        private int _tblPlanType;

        public int TblPlanType
        {
            get { return _tblPlanType; }
            set { _tblPlanType = value; RaisePropertyChanged("TblPlanType"); }
        }

        private GenericTable _purchaseTypePerRow;

        public GenericTable PurchaseTypePerRow
        {
            get { return _purchaseTypePerRow; }
            set { _purchaseTypePerRow = value; RaisePropertyChanged("PurchaseTypePerRow"); }
        }

        private string _axMethodOfPaymentCodeField;

        private string _axTermOfPaymentCodeField;

        private string _brandField;

        private DateTime? _deliveryDateField;

        private int _iserialField;

        private DateTime? _shippingDateField;

        private int _statusField;

        private int? _tblLkpSeasonField;

        private DateTime? _transDateField;

        private string _warehouseField;

        public string AxMethodOfPaymentCode
        {
            get
            {
                return _axMethodOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axMethodOfPaymentCodeField, value) != true))
                {
                    _axMethodOfPaymentCodeField = value;
                    RaisePropertyChanged("AxMethodOfPaymentCode");
                }
            }
        }

        public string AxTermOfPaymentCode
        {
            get
            {
                return _axTermOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axTermOfPaymentCodeField, value) != true))
                {
                    _axTermOfPaymentCodeField = value;
                    RaisePropertyChanged("AxTermOfPaymentCode");
                }
            }
        }

        public string Brand
        {
            get
            {
                return _brandField;
            }
            set
            {
                if ((ReferenceEquals(_brandField, value) != true))
                {
                    _brandField = value;
                    RaisePropertyChanged("Brand");

                    var brandSectionClient = new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                    };
                    if (Brand != null) brandSectionClient.GetTblBrandSectionLinkAsync(Brand, LoggedUserInfo.Iserial);

                    if (SectionPerRow != null)
                        if (SeasonPerRow != null)
                            if (Brand != null) Code = Brand + "-" + SectionPerRow.Code + "-" + SeasonPerRow.Ename;
                }
            }
        }

        private int? _tblLkpBrandSection;

        public int? TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSection;
            }
            set
            {
                if ((_tblLkpBrandSection.Equals(value) != true))
                {
                    if (_tblLkpBrandSection == value)
                    {
                        return;
                    }

                    _tblLkpBrandSection = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        private CRUDManagerService.TblLkpBrandSection _sectionPerRow;

        public CRUDManagerService.TblLkpBrandSection SectionPerRow
        {
            get
            {
                return _sectionPerRow ?? (_sectionPerRow = new CRUDManagerService.TblLkpBrandSection()); ;
            }
            set
            {
                if ((ReferenceEquals(_sectionPerRow, value) != true))
                {
                    _sectionPerRow = value;
                    RaisePropertyChanged("SectionPerRow");
                    if (SectionPerRow != null)
                        if (SeasonPerRow != null)
                            if (Brand != null) Code = Brand + "-" + SectionPerRow.Code + "-" + SeasonPerRow.Ename;
                }
            }
        }
        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        public DateTime? DeliveryDate
        {
            get
            {
                return _deliveryDateField;
            }
            set
            {
                if ((_deliveryDateField.Equals(value) != true))
                {
                    _deliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public DateTime? ShippingDate
        {
            get
            {
                return _shippingDateField;
            }
            set
            {
                if ((_shippingDateField.Equals(value) != true))
                {
                    _shippingDateField = value;
                    RaisePropertyChanged("ShippingDate");
                }
            }
        }

        public int Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                if ((_statusField.Equals(value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        

        private CRUDManagerService.TblLkpSeason _seasonPerRow;

        public CRUDManagerService.TblLkpSeason SeasonPerRow
        {
            get
            {
                return _seasonPerRow;
            }
            set
            {
                if ((ReferenceEquals(_seasonPerRow, value) != true))
                {
                    _seasonPerRow = value;
                    RaisePropertyChanged("SeasonPerRow");
                    if (SectionPerRow != null)
                        if (SeasonPerRow != null)
                            if (Brand != null) Code = Brand + "-" + SectionPerRow.Code + "-" + SeasonPerRow.Ename;
                }
            }
        }

        public int? TblLkpSeason
        {
            get
            {
                return _tblLkpSeasonField;
            }
            set
            {
                if ((_tblLkpSeasonField.Equals(value) != true))
                {
                    if (_tblLkpSeasonField == value)
                    {
                        return;
                    }
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }

        public string Warehouse
        {
            get
            {
                return _warehouseField;
            }
            set
            {
                if ((ReferenceEquals(_warehouseField, value) != true))
                {
                    _warehouseField = value;
                    RaisePropertyChanged("Warehouse");
                }
            }
        }

        private SortableCollectionView<TblPurchaseOrderHeaderModel> _purchaseOrderList;

        public SortableCollectionView<TblPurchaseOrderHeaderModel> PurchaseOrderList
        {
            get { return _purchaseOrderList ?? (_purchaseOrderList = new SortableCollectionView<TblPurchaseOrderHeaderModel>()); }
            set { _purchaseOrderList = value; RaisePropertyChanged("PurchaseOrderList"); }
        }
    }

    public class TblPurchaseOrderHeaderModel : PurchasePlanService.PropertiesViewModelBase
    {

        private SolidColorBrush _foreground;

        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; RaisePropertyChanged("Foreground"); }
        }
        private bool _GotPurchaseOrderRequest;

        public bool GotPurchaseOrderRequest
        {
            get { return _GotPurchaseOrderRequest; }
            set { _GotPurchaseOrderRequest = value; RaisePropertyChanged("GotPurchaseOrderRequest");
                if (GotPurchaseOrderRequest)
                {
                    Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Foreground = new SolidColorBrush(Colors.Black);
                }

               
            }
        }

        private ObservableCollection<TblPurchaseReceiveHeaderModel> _recHeaderList;

        public ObservableCollection<TblPurchaseReceiveHeaderModel> RecHeaderList
        {
            get { return _recHeaderList ?? (_recHeaderList = new ObservableCollection<TblPurchaseReceiveHeaderModel>()); }
            set
            {
                _recHeaderList = value;
                RaisePropertyChanged("RecHeaderList");
            }
        }
        private CRUDManagerService.TblWarehouse _warehousePerRow;

        public CRUDManagerService.TblWarehouse WarehousePerRow
        {
            get { return _warehousePerRow; }
            set
            {
                _warehousePerRow = value; RaisePropertyChanged("WarehousePerRow");
                

            }
        }
        private bool _status;

        public bool Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private string _axPurchase;

        public string AxPurchase
        {
            get { return _axPurchase; }
            set { _axPurchase= value; RaisePropertyChanged("AxPurchase"); }
        }

        private string _currencyCode;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
        public string CurrencyCode
        {
            get
            {
                return _currencyCode;
            }
            set
            {
                _currencyCode = value;
                RaisePropertyChanged("CurrencyCode");
            }
        }

        private SortableCollectionView<TblPurchaseOrderHeaderModel> _purchaseOrderHeaderList;

        public SortableCollectionView<TblPurchaseOrderHeaderModel> PurchaseOrderHeaderList
        {
            get { return _purchaseOrderHeaderList; }
            set { _purchaseOrderHeaderList = value; RaisePropertyChanged("PurchaseOrderHeaderList"); }
        }

        private CRUDManagerService.Vendor _vendorPerRowVendor;

        public CRUDManagerService.Vendor VendorPerRow
        {
            get { return _vendorPerRowVendor; }
            set
            {
                _vendorPerRowVendor = value; RaisePropertyChanged("VendorPerRow");
                if (_vendorPerRowVendor != null && _vendorPerRowVendor.vendor_code != null)
                {
                    Vendor = _vendorPerRowVendor.vendor_code;
                }
            }
        }

        private string _axMethodOfPaymentCodeField;

        private string _axTermOfPaymentCodeField;

        private DateTime? _deliveryDateField;

        private int _iserialField;

        private DateTime? _shippingDateField;

        private int? _tblGeneratePurchaseHeaderField;

        private DateTime? _transDateField;

        private string _vendorField;

        public string AxMethodOfPaymentCode
        {
            get
            {
                return _axMethodOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axMethodOfPaymentCodeField, value) != true))
                {
                    _axMethodOfPaymentCodeField = value;
                    RaisePropertyChanged("AxMethodOfPaymentCode");
                }
            }
        }

        public string AxTermOfPaymentCode
        {
            get
            {
                return _axTermOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axTermOfPaymentCodeField, value) != true))
                {
                    _axTermOfPaymentCodeField = value;
                    RaisePropertyChanged("AxTermOfPaymentCode");
                }
            }
        }

        public DateTime? DeliveryDate
        {
            get
            {
                return _deliveryDateField;
            }
            set
            {
                if ((_deliveryDateField.Equals(value) != true))
                {
                    _deliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public DateTime? ShippingDate
        {
            get
            {
                return _shippingDateField;
            }
            set
            {
                if ((_shippingDateField.Equals(value) != true))
                {
                    _shippingDateField = value;
                    RaisePropertyChanged("ShippingDate");
                }
            }
        }

        public int? TblGeneratePurchaseHeader
        {
            get
            {
                return _tblGeneratePurchaseHeaderField;
            }
            set
            {
                if ((_tblGeneratePurchaseHeaderField.Equals(value) != true))
                {
                    _tblGeneratePurchaseHeaderField = value;
                    RaisePropertyChanged("TblGeneratePurchaseHeader");
                }
            }
        }

        public DateTime? TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }

        public string Vendor
        {
            get
            {
                return _vendorField;
            }
            set
            {
                if ((ReferenceEquals(_vendorField, value) != true))
                {
                    _vendorField = value;
                    RaisePropertyChanged("Vendor");
                }
            }
        }

        private string _warehouse;

        public string Warehouse
        {
            get { return _warehouse; }
            set { _warehouse = value; RaisePropertyChanged("Warehouse"); }
        }

        private int? _tblPurchaseHeader;

        public int? TblPurchaseHeader
        {
            get { return _tblPurchaseHeader; }
            set { _tblPurchaseHeader = value; RaisePropertyChanged("TblPurchaseHeader"); }
        }

        private int? _tblPurchaseHeaderType;

        public int? TblPurchaseHeaderType
        {
            get { return _tblPurchaseHeaderType; }
            set { _tblPurchaseHeaderType = value; RaisePropertyChanged("TblPurchaseHeaderType"); }
        }

        private GenericTable _purchaseHeaderTypePerRow;

        public GenericTable PurchaseHeaderTypePerRow
        {
            get { return _purchaseHeaderTypePerRow; }
            set { _purchaseHeaderTypePerRow = value; RaisePropertyChanged("PurchaseHeaderTypePerRow"); }
        }

        private SortableCollectionView<TblPurchaseOrderDetailViewModel> _detailsList;

        public SortableCollectionView<TblPurchaseOrderDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new SortableCollectionView<TblPurchaseOrderDetailViewModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailsList"); }
        }
    }

    public class TblPurchaseOrderDetailViewModel : PurchasePlanService.PropertiesViewModelBase
    {
        private SolidColorBrush _foreground;

        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; RaisePropertyChanged("Foreground"); }
        }
        private bool _GotPurchaseOrderRequest;

        public bool GotPurchaseOrderRequest
        {
            get { return _GotPurchaseOrderRequest; }
            set
            {
                _GotPurchaseOrderRequest = value; RaisePropertyChanged("GotPurchaseOrderRequest");
                if (GotPurchaseOrderRequest)
                {
                    Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private bool _Canceled;

        public bool Canceled
        {
            get { return _Canceled; }
            set { _Canceled = value;RaisePropertyChanged("Canceled"); }
        }
        CCWFM.PurchasePlanService.TblLkpBrandSection TblLkpBrandSection1Field;
        public CCWFM.PurchasePlanService.TblLkpBrandSection TblLkpBrandSection1
        {
            get
            {
                return this.TblLkpBrandSection1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblLkpBrandSection1Field, value) != true))
                {
                    this.TblLkpBrandSection1Field = value;
                    this.RaisePropertyChanged("TblLkpBrandSection1");
                }
            }
        }
        PurchasePlanService.TblLkpSeason TblLkpSeason1Field;
        public PurchasePlanService.TblLkpSeason TblLkpSeason1
        {
            get
            {
                return this.TblLkpSeason1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblLkpSeason1Field, value) != true))
                {
                    this.TblLkpSeason1Field = value;
                    this.RaisePropertyChanged("TblLkpSeason1");
                }
            }
        }

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblPurchaseOrderDetailViewModel()
        {
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;

                if (ItemPerRow.CombinationList != null)
                {
                    var sizes =
                        ItemPerRow.CombinationList.Where(
                            x => ColorPerRow != null && (x.Configuration == ColorPerRow.Code));

                    var distinctsize = sizes.Select(x => x.Size);
                    foreach (var size in distinctsize)
                    {
                        if (!ItemPerRow.SizesList.Contains(size))
                        {
                            ItemPerRow.SizesList.Add(size);
                        }
                    }
                }
            };
        }

        private List<PurchasePlanService.TblPurchaseOrderDetailBreakDown> _purchaseOrderDetailBreakDownList;

        public List<PurchasePlanService.TblPurchaseOrderDetailBreakDown> PurchaseOrderDetailBreakDownList
        {
            get { return _purchaseOrderDetailBreakDownList; }
            set { _purchaseOrderDetailBreakDownList = value; RaisePropertyChanged("PurchaseOrderDetailBreakDownList"); }
        }

        private string _batchNo;

        public string BatchNo
        {
            get { return _batchNo; }
            set { _batchNo = value; RaisePropertyChanged("BatchNo"); }
        }


        private CRUDManagerService.TblColor _colorPerRow;

        public CRUDManagerService.TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                _colorPerRow = value; RaisePropertyChanged("ColorPerRow");
                if (_colorPerRow != null)
                {
                    FabricColor = _colorPerRow.Iserial;
                    if (IsAcc)
                    {
                        ItemPerRow.SizesList.Clear();
                        var sizes =
                            ItemPerRow.CombinationList.Where(
                                x => (x.Configuration == ColorPerRow.Code));

                        var distinctsize = sizes.Select(x => x.Size);
                        foreach (var size in distinctsize)
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

        //private ObservableCollection<CRUDManagerService.TblColor> _colors;

        //public ObservableCollection<CRUDManagerService.TblColor> Colors
        //{
        //    get { return _colors; }
        //    set { _colors = value; RaisePropertyChanged("Colors"); }
        //}

        private ObservableCollection<ItemsDto> _item;

        public ObservableCollection<ItemsDto> Items
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Items"); }
        }

        private bool _acc;

        public bool IsAcc
        {
            get { return _acc; }
            set { _acc = value; RaisePropertyChanged("IsAcc"); }
        }

        private ItemsDto _itemPerRow;

        public ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
                if (_itemPerRow != null)
                {
                    ItemId = ItemPerRow.Code;
                    ItemType = ItemPerRow.ItemGroup;
                    IsAcc = ItemPerRow.ItemGroup.StartsWith("Acc");
                    Unit = ItemPerRow.Unit;
                    if (IsAcc)
                    {
                        _client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                }
            }
        }

        private double? _basicPrice;
        public double? BasicPrice
        {
            get
            {
                return _basicPrice;
            }
            set
            {
                if ((_basicPrice.Equals(value) != true))
                {
                    _basicPrice = value;
                    RaisePropertyChanged("BasicPrice");
                }
            }
        }
        private double? _priceField;

        public double? Price
        {
            get
            {
                return _priceField;
            }
            set
            {
                if ((_priceField.Equals(value) != true))
                {
                    _priceField = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        private double? _remaningQty;

        public double? RemaningQty
        {
            get
            {
                return _remaningQty;
            }
            set
            {
                if ((_remaningQty.Equals(value) != true))
                {
                    _remaningQty = value;
                    RaisePropertyChanged("RemaningQty");
                }
            }
        }

        private double? _bomQty;

        public double? BomQty
        {
            get
            {
                return _bomQty;
            }
            set
            {
                if ((_bomQty.Equals(value) != true))
                {
                    _bomQty = value;
                    RaisePropertyChanged("BomQty");
                }
            }
        }

        private int? _fabricColorField;

        private int _iserialField;

        private string _itemIdField;

        private double _qtyField;

        private string _sizeField;

        private int _tblPurchaseOrderHeader;

        private string _unitField;

        [ReadOnly(true)]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqColor")]
        public int? FabricColor
        {
            get
            {
                return _fabricColorField;
            }
            set
            {
                if ((_fabricColorField.Equals(value) != true))
                {
                    _fabricColorField = value;
                    RaisePropertyChanged("FabricColor");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [ReadOnly(true)]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqItem")]
        public string ItemId
        {
            get
            {
                return _itemIdField;
            }
            set
            {
                if ((ReferenceEquals(_itemIdField, value) != true))
                {
                    _itemIdField = value;
                    RaisePropertyChanged("ItemId");
                }
            }
        }

        private string _itemType;

        [ReadOnly(true)]
        public string ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                if ((ReferenceEquals(_itemType, value) != true))
                {
                    _itemType = value;
                    RaisePropertyChanged("ItemType");
                }
            }
        }

        public double Qty
        {
            get
            {
                return _qtyField;
            }
            set
            {
                if ((_qtyField.Equals(value) != true))
                {
                    _qtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        [ReadOnly(true)]
        public string Size
        {
            get
            {
                return _sizeField;
            }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true))
                {
                    _sizeField = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        public int TblPurchaseOrderHeader
        {
            get
            {
                return _tblPurchaseOrderHeader;
            }
            set
            {
                if ((_tblPurchaseOrderHeader.Equals(value) != true))
                {
                    _tblPurchaseOrderHeader = value;
                    RaisePropertyChanged("TblPurchaseOrderHeader");
                }
            }
        }

        [ReadOnly(true)]
        public string Unit
        {
            get
            {
                return _unitField;
            }
            set
            {
                if ((ReferenceEquals(_unitField, value) != true))
                {
                    _unitField = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }

        private double _receiveNow;

        public double ReceiveNow
        {
            get
            {
                return _receiveNow;
            }
            set
            {
                if ((_receiveNow.Equals(value) != true))
                {
                    _receiveNow = value;
                    RaisePropertyChanged("ReceiveNow");
                }
            }
        }

        private double _received;

        public double Received
        {
            get
            {
                return _received;
            }
            set
            {
                if ((_received.Equals(value) != true))
                {
                    _received = value;
                    RaisePropertyChanged("Received");
                }
            }
        }

        private string _Brand;

        public string Brand
        {
            get { return _Brand; }
            set { _Brand = value; RaisePropertyChanged("Brand"); }
        }

    }

    public class TblGeneratePurchaseSalesOrdersViewModel : PurchasePlanService.PropertiesViewModelBase
    {
        private CRUDManagerService.TblSalesOrder _salesOrderPerRowOrder;

        public CRUDManagerService.TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRowOrder; }
            set { _salesOrderPerRowOrder = value; RaisePropertyChanged("SalesOrderPerRow"); }
        }

        private GenericTable _statusPerRow;

        public GenericTable StatusPerRow
        {
            get
            {
                return _statusPerRow ?? (_statusPerRow = new GenericTable
                {
                    Iserial = 3
                });
            }
            set { _statusPerRow = value; RaisePropertyChanged("StatusPerRow"); }
        }
    }

    #endregion ViewModels

    public class GeneratePurchaseViewModel : ViewModelBase
    {
        PurchasePlanClient PurchasePlanClient = new PurchasePlanClient();

        public GeneratePurchaseViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblGeneratePurchaseHeaderModel>();
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;

                Client.GetTblCurrencyAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblCurrencyCompleted += (s, sv) =>
                {
                    AxCurrencyList = sv.Result;
                };

                PurchasePlanClient.GetTblGeneratePurchaseHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGeneratePurchaseHeaderModel();
                        if (PurchaseTypeList.FirstOrDefault(x => x.Iserial == row.TblPurchaseType) != null)
                        {
                            newrow.PurchaseTypePerRow = PurchaseTypeList.FirstOrDefault(x => x.Iserial == row.TblPurchaseType);
                        }
                        if (SeasonList.FirstOrDefault(x => x.Iserial == row.TblLkpSeason) != null)
                        {
                            newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == row.TblLkpSeason);
                        }
                        if (row.TblLkpBrandSection1 != null)
                        {
                            newrow.SectionPerRow.InjectFrom( row.TblLkpBrandSection1);
                        }

                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };
                Client.GetTblWarehouseAsync(0, int.MaxValue, "it.Iserial", null, null);

                Client.GetTblWarehouseCompleted += (s, sv) =>
                 {
                     WareHouseList= sv.Result;                    
                 };
                              
                //Client.GetVendPayModeAsync("CCR");
                //Client.GetVendPayModeCompleted += (s, sv) =>
                //{
                //    VendPayModeList = sv.Result;
                //};
                //Client.GetAxPaymentTermAsync("CCR");
                //Client.GetAxPaymentTermCompleted += (s, sv) =>
                //{
                //    PaymTerm = sv.Result;
                //};
                Client.GetAllWarehousesByCompanyNameAsync("CCm");
                Client.GetGenericCompleted += (s, sv) =>
                {
                    PurchaseTypeList = sv.Result;
                };
                Client.GetGenericAsync("TblPurchaseType", "%%", "%%", "%%", "Iserial", "ASC");

                var tblFactoryDelivery = new CRUDManagerService.CRUD_ManagerServiceClient();

                tblFactoryDelivery.GetGenericCompleted += (s, sv) =>
                {
                    TblFactoryDeliveryList = sv.Result;
                };
                tblFactoryDelivery.GetGenericAsync("TblFactoryDelivery", "%%", "%%", "%%", "Iserial", "ASC");

                var tblPlanTypeList = new CRUDManagerService.CRUD_ManagerServiceClient();

                tblPlanTypeList.GetGenericCompleted += (s, sv) =>
                {
                    PlanTypeList = sv.Result;
                };

                tblPlanTypeList.GetGenericAsync("TblPlanType", "%%", "%%", "%%", "Iserial", "ASC");


                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList = sv.Result;
                };

                Client.GetAllSeasonsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SeasonList.All(x => x.Iserial != row.Iserial))
                        {
                            SeasonList.Add(new CRUDManagerService.TblLkpSeason().InjectFrom(row) as CRUDManagerService.TblLkpSeason);
                        }
                    }
                };
                Client.GetAllSeasonsAsync();
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
              
                PurchasePlanClient.GetTblPurchaseOrderDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderDetailViewModel { ColorPerRow = new CRUDManagerService.TblColor() };
                        if (row.TblColor != null) newrow.ColorPerRow.InjectFrom( row.TblColor);
                        newrow.GotPurchaseOrderRequest = false;
                        if (PurchaseRequestLink != null)
                        {
                            if (PurchaseRequestLink.Any(w => w.TblPurchaseOrderDetail == row.Iserial))
                            {

                                newrow.GotPurchaseOrderRequest = true;
                            }
                            else
                            {
                                newrow.GotPurchaseOrderRequest = false;
                            }
                        }

                        newrow.InjectFrom(row);                 
                        SelectedPurchaseRow.DetailsList.Add(newrow);
                    }

                    if (!SelectedPurchaseRow.DetailsList.Any())
                    {
                        AddNewPurchaseDetail(false);
                    }
                    Loading = false;
                };
               
                PurchasePlanClient.UpdateOrInsertTblGeneratePurchaseHeaderCompleted += (s, x) =>
                {                    
                    var savedRow = (TblGeneratePurchaseHeaderModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.Code = x.Result.Code;
                        savedRow.Iserial = x.Result.Iserial;
                    }
                    MessageBox.Show(strings.SavedMessage);
                    GetMaindata();
                };

                PurchasePlanClient.RecalculateTblGeneratePurchaseHeaderCompleted += (s, x) =>
                {
                    var savedRow = (TblGeneratePurchaseHeaderModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.Code = x.Result.Code;
                        savedRow.Iserial = x.Result.Iserial;                      
                    }
                    MessageBox.Show(strings.SavedMessage);
                    GetMaindata();
                };

                PurchasePlanClient.GetTblPurchaseOrderHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderHeaderModel();
                        newrow.InjectFrom(row);
                        newrow.PurchaseHeaderTypePerRow = new GenericTable();
                        if (row.TblPurchaseHeaderType1 != null)
                        {
                            newrow.PurchaseHeaderTypePerRow.InjectFrom(row.TblPurchaseHeaderType1);
                        }
                        PurchaseRequestLink = sv.PurchaseRequestLink;
                        newrow.GotPurchaseOrderRequest = false;
                        if (sv.PurchaseRequestLink != null)
                        {   if (sv.PurchaseRequestLink.Any(w => w.TblPurchaseOrderDetail1.TblPurchaseOrderHeader == row.Iserial))
                            {
                                newrow.GotPurchaseOrderRequest = true;
                            }
                            else
                            {
                                newrow.GotPurchaseOrderRequest = false;
                            }
                        }

                        newrow.PurchaseOrderHeaderList = new SortableCollectionView<TblPurchaseOrderHeaderModel>();
                        foreach (var purchaseRow in row.TblPurchaseOrderHeader1)
                        {
                            var roow = new TblPurchaseOrderHeaderModel();
                            roow.InjectFrom(purchaseRow);
                            roow.PurchaseHeaderTypePerRow = new GenericTable();
                            if (purchaseRow.TblPurchaseHeaderType1 != null)
                                roow.PurchaseHeaderTypePerRow.InjectFrom(purchaseRow.TblPurchaseHeaderType1);
                            newrow.PurchaseOrderHeaderList.Add(roow);
                        }

                        SelectedMainRow.PurchaseOrderList.Add(newrow);
                    }
                    Loading = false;

                    if (!SelectedMainRow.PurchaseOrderList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                    foreach (var variable in sv.SalesorderList.ToList())
                    {
                        GenerateSalesOrdersList.Add(new TblGeneratePurchaseSalesOrdersViewModel
                        {
                            SalesOrderPerRow = new CRUDManagerService.TblSalesOrder
                            {
                                SalesOrderCode = variable.Key,
                                Iserial = variable.Value
                            }
                        });
                    }
                };

                PurchasePlanClient.UpdateOrInsertTblPurchaseOrderHeaderCompleted += (s, x) =>
                {
                    var savedRow = (TblPurchaseOrderHeaderModel)SelectedMainRow.PurchaseOrderList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                PurchasePlanClient.DeleteTblPurchaseOrderHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.PurchaseOrderList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.PurchaseOrderList.Remove(oldrow);
                };

                PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblPurchaseOrderDetailViewModel)SelectedPurchaseRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                PurchasePlanClient.DeleteTblGeneratePurchaseHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblPurchaseOrderDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedPurchaseRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedPurchaseRow.DetailsList.Remove(oldrow);
                };

                PurchasePlanClient.GetTblPurchaseOrderDetailBreakDownCompleted += (s, sv) =>
                {
                    SelectedDetailRow.PurchaseOrderDetailBreakDownList = new List<PurchasePlanService.TblPurchaseOrderDetailBreakDown>();
                    foreach (var variable in sv.Result)
                    {
                        SelectedDetailRow.PurchaseOrderDetailBreakDownList.Add(variable);
                    }
                };

                PurchasePlanClient.GeneratePurchaseOrderFromUnpannedPurchaseCompleted += (s, sv) => MessageBox.Show("Link Generated Successfully");

                PurchasePlanClient.DeleteTblPurchaseOrderDetailBreakDownCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedDetailRow.PurchaseOrderDetailBreakDownList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.PurchaseOrderDetailBreakDownList.Remove(oldrow);
                };
            }
        }

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblGeneratePurchaseHeaderModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblGeneratePurchaseHeaderModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == "Brand")
            {
                if (SelectedMainRow.Brand != null)
                {
                }
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc";
            PurchasePlanClient.GetTblGeneratePurchaseHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        PurchasePlanClient.DeleteTblGeneratePurchaseHeaderAsync((PurchasePlanService.TblGeneratePurchaseHeader)new PurchasePlanService.TblGeneratePurchaseHeader().InjectFrom(row), MainRowList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new TblGeneratePurchaseHeaderModel());
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.PurchaseOrderList.IndexOf(SelectedPurchaseRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.PurchaseOrderList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedPurchaseRow, new ValidationContext(SelectedPurchaseRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.PurchaseOrderList.Insert(currentRowIndex + 1, new TblPurchaseOrderHeaderModel { TblGeneratePurchaseHeader = SelectedMainRow.Iserial }
                    );
            }
        }

        public void AddNewDetail2Row(bool checkLastRow)
        {
            var currentRowIndex = (SelectedPurchaseRow.PurchaseOrderHeaderList.IndexOf(SelectedPurchase2Row));
            if (!checkLastRow || currentRowIndex == (SelectedPurchaseRow.PurchaseOrderHeaderList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedPurchase2Row, new ValidationContext(SelectedPurchase2Row, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedPurchaseRow.PurchaseOrderHeaderList.Insert(currentRowIndex + 1, new TblPurchaseOrderHeaderModel { TblGeneratePurchaseHeader = SelectedMainRow.Iserial }
                    );
            }
        }

        public void AddNewPurchaseDetail(bool checkLastRow)
        {
            var currentRowIndex = (SelectedPurchaseRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedPurchaseRow.DetailsList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedPurchaseRow.DetailsList.Insert(currentRowIndex + 1, new TblPurchaseOrderDetailViewModel
                {
                    TblPurchaseOrderHeader = SelectedPurchaseRow.Iserial,
                });
            }
        }

        public void AddNewSalesOrder(bool checkLastRow)
        {
            var currentRowIndex = (GenerateSalesOrdersList.IndexOf(SelectedSalesOrder));
            if (!checkLastRow || currentRowIndex == (GenerateSalesOrdersList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedSalesOrder, new ValidationContext(SelectedSalesOrder, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                GenerateSalesOrdersList.Insert(currentRowIndex + 1, new TblGeneratePurchaseSalesOrdersViewModel());
            }
        }

        public void SaveMainRow(bool recalc = false)
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var saveRow = new PurchasePlanService.TblGeneratePurchaseHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    if (recalc)
                    {
                        PurchasePlanClient.RecalculateTblGeneratePurchaseHeaderAsync(saveRow, MainRowList.IndexOf(SelectedMainRow), new ObservableCollection<int>(GenerateSalesOrdersList.Select(w => w.SalesOrderPerRow.Iserial)));
                    }
                    else
                    {
                        PurchasePlanClient.UpdateOrInsertTblGeneratePurchaseHeaderAsync(saveRow, MainRowList.IndexOf(SelectedMainRow), new ObservableCollection<int>(GenerateSalesOrdersList.Select(w => w.SalesOrderPerRow.Iserial)), "");
                    }
                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial desc";
                PurchasePlanClient.GetTblPurchaseOrderDetailAsync(SelectedPurchaseRow.DetailsList.Count, int.MaxValue,
                    SelectedPurchaseRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
            }
        }

        public void GetDetail2Data()
        {
            if (SelectedMainRow != null)
            {
                var service = new PurchasePlanClient();
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                service.GetTblPurchaseOrderDetailAsync(SelectedPurchase2Row.DetailsList.Count, int.MaxValue,
                    SelectedPurchase2Row.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);

                service.GetTblPurchaseOrderDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderDetailViewModel { ColorPerRow = new CRUDManagerService.TblColor() };
                        if (row.TblColor != null) newrow.ColorPerRow.InjectFrom( row.TblColor);

                        newrow.InjectFrom(row);
                        SelectedPurchase2Row.DetailsList.Add(newrow);
                    }
                    if (!SelectedPurchase2Row.DetailsList.Any())
                    {
                        AddNewPurchaseDetail(false);
                    }
                    Loading = false;
                };
            }
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        PurchasePlanClient.DeleteTblPurchaseOrderDetailAsync((PurchasePlanService.TblPurchaseOrderDetail)new PurchasePlanService.TblPurchaseOrderDetail().InjectFrom(row), SelectedPurchaseRow.DetailsList.IndexOf(row));
                    }
                }
            }
        }
       
        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseOrderDetail();
                    SelectedDetailRow.TblPurchaseOrderHeader = SelectedPurchaseRow.Iserial;
                    rowToSave.InjectFrom(SelectedDetailRow);
                    PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailAsync(rowToSave, SelectedPurchaseRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        public void SaveDetail2Row()
        {
            if (SelectedDetail2Row != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetail2Row, new ValidationContext(SelectedDetail2Row, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseOrderDetail();
                    SelectedDetail2Row.TblPurchaseOrderHeader = SelectedPurchase2Row.Iserial;
                    rowToSave.InjectFrom(SelectedDetail2Row);
                    PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailAsync(rowToSave, SelectedPurchase2Row.DetailsList.IndexOf(SelectedDetail2Row));
                }
            }
        }

        internal void SavePurchase2Row()
        {
            if (SelectedPurchase2Row != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedPurchase2Row, new ValidationContext(SelectedPurchase2Row, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var service = new PurchasePlanClient();
                    var rowToSave = new PurchasePlanService.TblPurchaseOrderHeader();
                    SelectedPurchase2Row.TblGeneratePurchaseHeader = SelectedMainRow.Iserial;
                    SelectedPurchase2Row.TransDate = SelectedMainRow.TransDate;
                    rowToSave.InjectFrom(SelectedPurchase2Row);
                    service.UpdateOrInsertTblPurchaseOrderHeaderAsync(rowToSave, SelectedPurchaseRow.PurchaseOrderHeaderList.IndexOf(SelectedPurchase2Row));
                    service.UpdateOrInsertTblPurchaseOrderHeaderCompleted += (s, sv) =>
                    {
                        var savedRow = (TblPurchaseOrderHeaderModel)SelectedPurchaseRow.PurchaseOrderHeaderList.GetItemAt(sv.outindex);

                        if (savedRow != null) savedRow.InjectFrom(sv.Result);
                    };
                }
            }
        }

        public void SavePurchaseRow()
        {
            if (SelectedPurchaseRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedPurchaseRow, new ValidationContext(SelectedPurchaseRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseOrderHeader();
                    SelectedPurchaseRow.TblGeneratePurchaseHeader = SelectedMainRow.Iserial;
                    SelectedPurchaseRow.TransDate = SelectedMainRow.TransDate;
                    rowToSave.InjectFrom(SelectedPurchaseRow);
                    PurchasePlanClient.UpdateOrInsertTblPurchaseOrderHeaderAsync(rowToSave, SelectedMainRow.PurchaseOrderList.IndexOf(SelectedPurchaseRow));
                }
            }
        }

        public void SearchHeader()
        {
            MainRowList.Clear();
            var child = new GeneratePurchaseChild(this);
            child.Show();
        }  

        public void DeleteOrder()
        {
            PurchasePlanClient.DeleteTblGeneratePurchaseHeaderAsync((PurchasePlanService.TblGeneratePurchaseHeader)new PurchasePlanService.TblGeneratePurchaseHeader().InjectFrom(SelectedMainRow), MainRowList.IndexOf(SelectedMainRow));
        }

        public void DeleteLink()
        {
            if (SelectedPurchaseOrderDetailBreakDowns != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedPurchaseOrderDetailBreakDowns)
                    {
                        PurchasePlanClient.DeleteTblPurchaseOrderDetailBreakDownAsync((PurchasePlanService.TblPurchaseOrderDetailBreakDown)new PurchasePlanService.TblPurchaseOrderDetailBreakDown().InjectFrom(row), SelectedDetailRow.PurchaseOrderDetailBreakDownList.IndexOf(row));
                    }
                }
            }
        }

        public void GetPurchaseBom()
        {
            PurchasePlanClient.GetTblPurchaseOrderDetailBreakDownAsync(SelectedDetailRow.Iserial);
        }

        public void GetPurchaseHeader()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            PurchasePlanClient.GetTblPurchaseOrderHeaderAsync(SelectedMainRow.PurchaseOrderList.Count, PageSize, SelectedMainRow.Iserial, SortBy, null, null);
        }


        #region Prop     
        private ObservableCollection<CRUDManagerService.TblWarehouse> _wareHouseList;

        public ObservableCollection<CRUDManagerService.TblWarehouse> WareHouseList
        {
            get { return _wareHouseList; }
            set { _wareHouseList = value; RaisePropertyChanged("WareHouseList"); }
        }

        private ObservableCollection<TblGeneratePurchaseSalesOrdersViewModel> _generateSalesOrdersList;

        public ObservableCollection<TblGeneratePurchaseSalesOrdersViewModel> GenerateSalesOrdersList
        {
            get { return _generateSalesOrdersList ?? (_generateSalesOrdersList = new ObservableCollection<TblGeneratePurchaseSalesOrdersViewModel>()); }
            set { _generateSalesOrdersList = value; RaisePropertyChanged("GenerateSalesOrdersList"); }
        }

        private ObservableCollection<GenericTable> _purchaseTypeList;

        public ObservableCollection<GenericTable> PurchaseTypeList
        {
            get { return _purchaseTypeList; }
            set { _purchaseTypeList = value; RaisePropertyChanged("PurchaseTypeList"); }
        }

        private ObservableCollection<GenericTable> _tblFactoryDeliveryList;

        public ObservableCollection<GenericTable> TblFactoryDeliveryList
        {
            get { return _tblFactoryDeliveryList; }
            set { _tblFactoryDeliveryList = value; RaisePropertyChanged("TblFactoryDeliveryList"); }
        }

        private ObservableCollection<CRUDManagerService.TblCurrency> _axCurrencyList;

        public ObservableCollection<CRUDManagerService.TblCurrency> AxCurrencyList
        {
            get { return _axCurrencyList; }
            set { _axCurrencyList = value; RaisePropertyChanged("AxCurrencyList"); }
        }

        private ObservableCollection<GenericTable> _planTypeList;

        public ObservableCollection<GenericTable> PlanTypeList
        {
            get { return _planTypeList; }
            set { _planTypeList = value; RaisePropertyChanged("PlanTypeList"); }
        }

        private ObservableCollection<VENDPAYMMODETABLE> _vendPayMode;

        public ObservableCollection<VENDPAYMMODETABLE> VendPayModeList
        {
            get { return _vendPayMode ?? (_vendPayMode = new ObservableCollection<VENDPAYMMODETABLE>()); }
            set { _vendPayMode = value; RaisePropertyChanged("VendPayModeList"); }
        }

        private ObservableCollection<PAYMTERM> _paymTerm;

        public ObservableCollection<PAYMTERM> PaymTerm
        {
            get { return _paymTerm ?? (_paymTerm = new ObservableCollection<PAYMTERM>()); }
            set { _paymTerm = value; RaisePropertyChanged("PaymTerm"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<CRUDManagerService.TblLkpSeason> _seasonList;

        public ObservableCollection<CRUDManagerService.TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<CRUDManagerService.TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private SortableCollectionView<TblGeneratePurchaseHeaderModel> _mainRowList;

        public SortableCollectionView<TblGeneratePurchaseHeaderModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGeneratePurchaseHeaderModel> _selectedMainRows;

        public ObservableCollection<TblGeneratePurchaseHeaderModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGeneratePurchaseHeaderModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblGeneratePurchaseHeaderModel _selectedMainRow;

        public TblGeneratePurchaseHeaderModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblGeneratePurchaseHeaderModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblGeneratePurchaseSalesOrdersViewModel _selectedSalesOrder;

        public TblGeneratePurchaseSalesOrdersViewModel SelectedSalesOrder
        {
            get { return _selectedSalesOrder ?? (_selectedSalesOrder = new TblGeneratePurchaseSalesOrdersViewModel()); }
            set { _selectedSalesOrder = value; RaisePropertyChanged("SelectedSalesOrder"); }
        }

        private TblPurchaseOrderHeaderModel _selectedPurchaseRow;

        public TblPurchaseOrderHeaderModel SelectedPurchaseRow
        {
            get { return _selectedPurchaseRow ?? (_selectedPurchaseRow = new TblPurchaseOrderHeaderModel()); }
            set { _selectedPurchaseRow = value; RaisePropertyChanged("SelectedPurchaseRow"); }
        }

        private TblPurchaseOrderHeaderModel _selectedPurchase2Row;

        public TblPurchaseOrderHeaderModel SelectedPurchase2Row
        {
            get { return _selectedPurchase2Row ?? (_selectedPurchase2Row = new TblPurchaseOrderHeaderModel()); }
            set { _selectedPurchase2Row = value; RaisePropertyChanged("SelectedPurchase2Row"); }
        }

        private TblPurchaseOrderDetailViewModel _selectedDetailRow;

        public TblPurchaseOrderDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private TblPurchaseOrderDetailViewModel _selectedDetail2Row;

        public TblPurchaseOrderDetailViewModel SelectedDetail2Row
        {
            get { return _selectedDetail2Row; }
            set { _selectedDetail2Row = value; RaisePropertyChanged("SelectedDetail2Row"); }
        }

        private ObservableCollection<TblPurchaseOrderDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPurchaseOrderDetailViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPurchaseOrderDetailViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        public string RecDetailFilter { get; set; }

        public Dictionary<string, object> RecDetailValuesObjects { get; set; }

     


        private ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown> _selectedPurchaseOrderDetailBreakDowns;

        public ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown> SelectedPurchaseOrderDetailBreakDowns
        {
            get
            {
                return _selectedPurchaseOrderDetailBreakDowns ?? (_selectedPurchaseOrderDetailBreakDowns = new ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown>());
            }
            set { _selectedPurchaseOrderDetailBreakDowns = value; RaisePropertyChanged("SelectedPurchaseOrderDetailBreakDowns"); }
        }

        public ObservableCollection<PurchasePlanService.TblPurchaseRequestLink> PurchaseRequestLink { get; private set; }

        #endregion Prop

        public void GenerateLink()
        {
            PurchasePlanClient.GeneratePurchaseOrderFromUnpannedPurchaseAsync(SelectedMainRow.Iserial);
        }

        public void MergePlans()
        {
            MainRowList.Clear();
            var child = new GeneratePurchaseChild(this, true);
            child.Show();
        }

        public void ApplyMergePlans(int iserial)
        {
            PurchasePlanClient.CollectPlansAsync(SelectedMainRow.Iserial, iserial);
        }
    }
}