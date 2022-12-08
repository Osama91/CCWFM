using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.PurchasePlanService;
using CCWFM.CRUDManagerService;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Helpers.Enums;

namespace CCWFM.ViewModel.OGViewModels
{
    #region Models

    public class TblPurchaseOrderHeaderRequestModel : PurchasePlanService.PropertiesViewModelBase
    {

        public TblPurchaseOrderHeaderRequestModel()
        {
            PaymentList.CollectionChanged += PaymentList_CollectionChanged;


        }

        private void PaymentList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TotalPayments = PaymentList.Sum(w => w.Amount);
        }

        public void sumTotal()
        {
            TotalPayments = PaymentList.Sum(w => w.Amount);
        }
        private DateTime _ApprovalDate;

        public DateTime ApprovalDate
        {
            get { return _ApprovalDate; }
            set { _ApprovalDate = value; RaisePropertyChanged("ApprovalDate"); }
        }

        private int _ApprovedBy;

        public int ApprovedBy
        {
            get { return _ApprovedBy; }
            set { _ApprovedBy = value; RaisePropertyChanged("ApprovedBy"); }
        }

        private DateTime _CreationDate;

        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private int _CreatedBy;

        public int CreatedBy
        {
            get { return _CreatedBy; }
            set { _CreatedBy = value; RaisePropertyChanged("CreatedBy"); }
        }


        private int? _TblGeneratePurchaseHeader;

        public int? TblGeneratePurchaseHeader
        {
            get { return _TblGeneratePurchaseHeader; }
            set { _TblGeneratePurchaseHeader = value; RaisePropertyChanged("TblGeneratePurchaseHeader"); }
        }
        private ObservableCollection<PaymentSettingModel> _PaymentSettings;

        public ObservableCollection<PaymentSettingModel> PaymentSettings
        {
            get { return _PaymentSettings ?? (_PaymentSettings = new ObservableCollection<PaymentSettingModel>()); }
            set { _PaymentSettings = value; RaisePropertyChanged("PaymentSettings"); }
        }

        private bool? _PendingInvoice;

        public bool? PendingInvoice
        {
            get { return _PendingInvoice; }
            set { _PendingInvoice = value; RaisePropertyChanged("PendingInvoice"); }
        }
        private bool? _PendingReceive;

        public bool? PendingReceive
        {
            get { return _PendingReceive; }
            set
            {
                _PendingReceive = value; RaisePropertyChanged("PendingReceive");
            }
        }

        private string _RefNo;

        public string RefNo
        {
            get { return _RefNo; }
            set { _RefNo = value; RaisePropertyChanged("RefNo"); }
        }
        
        private string _Notes;

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; RaisePropertyChanged("Notes"); }
        }

        private string _PlanCode;

        public string PlanCode
        {
            get { return _PlanCode; }
            set { _PlanCode = value; RaisePropertyChanged("PlanCode"); }
        }

        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { _Code = value; RaisePropertyChanged("Code"); }
        }
        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; RaisePropertyChanged("Enabled"); }
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

        private ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel> _PaymentList;

        public ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel> PaymentList
        {
            get { return _PaymentList ?? (_PaymentList = new ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel>()); }
            set
            {
                _PaymentList = value;
                RaisePropertyChanged("PaymentList");
            }
        }
        private CRUDManagerService.TblWarehouse _warehousePerRow;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public CRUDManagerService.TblWarehouse WarehousePerRow
        {
            get { return _warehousePerRow; }
            set
            {
                _warehousePerRow = value; RaisePropertyChanged("WarehousePerRow");
                if (_warehousePerRow != null && _warehousePerRow.Iserial != 0)
                {
                    TblWarehouse = _warehousePerRow.Iserial;
                }
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
            set { _axPurchase = value; RaisePropertyChanged("AxPurchase"); }
        }

        private int _currencyCode;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
        public int TblCurrency
        {
            get
            {
                return _currencyCode;
            }
            set
            {
                _currencyCode = value;
                RaisePropertyChanged("TblCurrency");
            }
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

        public string _vendor_code;
        public string vendor_code
        {
            get
            {
                return _vendor_code;
            }
            set
            {
                if ((ReferenceEquals(_vendor_code, value) != true))
                {
                    _vendor_code = value;
                    RaisePropertyChanged("vendor_code");
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


        public DateTime? DeliveryDate
        {
            get
            {
                return _deliveryDateField ?? (_deliveryDateField = DateTime.Now);
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

        public DateTime? ShippingDate
        {
            get
            {
                return _shippingDateField ?? (_shippingDateField = DateTime.Now);
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

        public DateTime? TransDate
        {
            get
            {
                return _transDateField ?? (_transDateField = DateTime.Now);
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
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqVendor")]
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

        private int? _warehouse;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public int? TblWarehouse
        {
            get { return _warehouse; }
            set { _warehouse = value; RaisePropertyChanged("TblWarehouse"); }
        }


        private ObservableCollection<TblPurchaseOrderDetailRequestModel> _detailsList;

        public ObservableCollection<TblPurchaseOrderDetailRequestModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new SortableCollectionView<TblPurchaseOrderDetailRequestModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailsList"); }
        }

        private decimal _Total;

        public decimal Total
        {
            get { return _Total; }
            set
            {

                if (_Total != value)
                {
                    _Total = value;
                    RaisePropertyChanged("Total");
                    Total = decimal.Round(Total, 1);
                    Remaining = Total - TotalPayments;
                }

            }
        }

        private decimal _TotalPayments;

        public decimal TotalPayments
        {
            get { return _TotalPayments; }
            set
            {
                if (_TotalPayments != value)
                {
                    _TotalPayments = value; RaisePropertyChanged("TotalPayments");
                    TotalPayments = decimal.Round(TotalPayments, 1);
                    Remaining = Total - TotalPayments;
                }


            }
        }
        private decimal _Remaining;

        public decimal Remaining
        {
            get { return _Remaining; }
            set { _Remaining = value; RaisePropertyChanged("Remaining"); }
        }


    }

    public class TblPurchaseOrderDetailRequestModel : PurchasePlanService.PropertiesViewModelBase
    {



        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private PurchasePlanService.ItemDimensionSearchModel itemTransfer;

        public virtual PurchasePlanService.ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new PurchasePlanService.ItemDimensionSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemTransfer, value) != true))
                {
                    itemTransfer = value;
                }
            }
        }

        private int _TblPurchaseOrderHeaderRequest;

        public int TblPurchaseOrderHeaderRequest
        {
            get { return _TblPurchaseOrderHeaderRequest; }
            set { _TblPurchaseOrderHeaderRequest = value; RaisePropertyChanged("TblPurchaseOrderHeaderRequest"); }
        }

        private string _BatchNo;

        public string BatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; RaisePropertyChanged("BatchNo"); }
        }

        DateTime? _deliveryDateField;
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
        DateTime? _shippingDateField;
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

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblPurchaseOrderDetailRequestModel()
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

        private ObservableCollection<PurchasePlanService.TblPurchaseRequestLink> _TblPurchaseRequestLink;

        public ObservableCollection<PurchasePlanService.TblPurchaseRequestLink> TblPurchaseRequestLink
        {
            get { return _TblPurchaseRequestLink; }
            set { _TblPurchaseRequestLink = value; }
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

        private double _basicPrice;
        public double BasicPrice
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
        private double _priceField;

        public double Price
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
    }

    public class TblPurchaseOrderHeaderRequestPaymentModel : PurchasePlanService.PropertiesViewModelBase
    {
        private CRUDManagerService.TblPaymentScheduleSetting _PaymentSettingPerRow;

        public CRUDManagerService.TblPaymentScheduleSetting PaymentSettingPerRow
        {
            get { return _PaymentSettingPerRow; }
            set { _PaymentSettingPerRow = value; RaisePropertyChanged("PaymentSettingPerRow"); }
        }
        private int? _PaymentScheduleSetting;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPaymentScheduleSetting")]
        public int? TblPaymentScheduleSettings
        {
            get { return _PaymentScheduleSetting; }
            set { _PaymentScheduleSetting = value; RaisePropertyChanged("TblPaymentScheduleSettings"); }
        }


        private decimal AmountField;
        private string DescriptionField;
        private DateTime? DueDateField;
        private int IserialField;
        private int? StatusField;

        private int? TblPurchaseOrderHeaderRequestField;
        public decimal Amount
        {
            get
            {
                return AmountField;
            }
            set
            {
                if ((AmountField.Equals(value) != true))
                {
                    AmountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }
        public string Description
        {
            get
            {
                return DescriptionField;
            }
            set
            {
                if ((ReferenceEquals(this.DescriptionField, value) != true))
                {
                    DescriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? DueDate
        {
            get
            {
                return DueDateField;
            }
            set
            {
                if ((DueDateField.Equals(value) != true))
                {
                    DueDateField = value;
                    RaisePropertyChanged("DueDate");
                }
            }
        }
        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }
        public int? Status
        {
            get
            {
                return StatusField;
            }
            set
            {
                if ((StatusField.Equals(value) != true))
                {
                    this.StatusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }
        public int? TblPurchaseOrderHeaderRequest
        {
            get
            {
                return this.TblPurchaseOrderHeaderRequestField;
            }
            set
            {
                if ((this.TblPurchaseOrderHeaderRequestField.Equals(value) != true))
                {
                    this.TblPurchaseOrderHeaderRequestField = value;
                    RaisePropertyChanged("TblPurchaseOrderHeaderRequest");
                }
            }
        }
    }

    public class PaymentSettingModel : PurchasePlanService.PropertiesViewModelBase
    {
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; RaisePropertyChanged("Description"); }
        }

        private CRUDManagerService.TblPaymentScheduleSetting _PaymentSettingPerRow;

        public CRUDManagerService.TblPaymentScheduleSetting PaymentSettingPerRow
        {
            get { return _PaymentSettingPerRow; }
            set { _PaymentSettingPerRow = value; RaisePropertyChanged("PaymentSettingPerRow"); }
        }
        private int? _PaymentScheduleSetting;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPaymentScheduleSetting")]
        public int? PaymentScheduleSetting
        {
            get { return _PaymentScheduleSetting; }
            set { _PaymentScheduleSetting = value; RaisePropertyChanged("PaymentScheduleSetting"); }
        }


        private float _Percentage;

        public float Percentage
        {
            get { return _Percentage; }
            set { _Percentage = value; RaisePropertyChanged("Percentage"); }
        }
        private int _InstallmentCount;
        [Range(1, int.MaxValue)]
        public int InstallmentCount
        {
            get { return _InstallmentCount; }
            set { _InstallmentCount = value; RaisePropertyChanged("InstallmentCount"); }
        }

        private int _InstallmentInterval;
        [Range(1, int.MaxValue)]
        public int InstallmentInterval
        {
            get { return _InstallmentInterval; }
            set { _InstallmentInterval = value; RaisePropertyChanged("InstallmentInterval"); }
        }


        private int _StartingDays;

        public int StartingDays
        {
            get { return _StartingDays; }
            set { _StartingDays = value; RaisePropertyChanged("StartingDays"); }
        }
    }

    #endregion Models

    public class PurchaseOrderRequestViewModel : ViewModelBase
    {
        public PurchasePlanClient PurchasePlanClient = new PurchasePlanClient();

        private bool _StyleWithCostPrice;
        public bool StyleWithCostPrice
        {
            get { return _StyleWithCostPrice; }
            set { _StyleWithCostPrice = value; RaisePropertyChanged("StyleWithCostPrice"); }
        }

        private bool _AllowAddFree;
        public bool AllowAddFree
        {
            get { return _AllowAddFree; }
            set { _AllowAddFree = value; RaisePropertyChanged("AllowAddFree"); }
        }

        private bool _acc;
        public bool IsAcc
        {
            get { return _acc; }
            set { _acc = value; RaisePropertyChanged("IsAcc"); }
        }

        private int _TblColor;
        public int TblColor
        {
            get { return _TblColor; }
            set { _TblColor = value; RaisePropertyChanged("TblColor"); }
        }

        private CRUDManagerService.TblColor _colorPerRow;
        public CRUDManagerService.TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                if (value != null)
                {
                    _colorPerRow = value; RaisePropertyChanged("ColorPerRow");
                    if (_colorPerRow != null)
                    {
                        TblColor = _colorPerRow.Iserial;
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
                    //ItemCode = ItemPerRow.Iserial;
                    //ItemType = ItemPerRow.ItemGroup;
                    IsAcc = ItemPerRow.ItemGroup.StartsWith("Acc");
                    if (IsAcc)
                    {
                        Client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                }
            }
        }

        private DateTime? _From;
        public DateTime? FromDate
        {
            get { return _From; }
            set { _From = value; RaisePropertyChanged("FromDate"); }
        }

        private DateTime? _to;
        public DateTime? ToDate
        {
            get { return _to; }
            set { _to = value; RaisePropertyChanged("ToDate"); }
        }

        int? _tblLkpSeasonField;
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

        private string _Vendor;
        public string Vendor
        {
            get { return _Vendor; }
            set { _Vendor = value; RaisePropertyChanged("Vendor"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;
        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private string _Brand;
        public string Brand
        {
            get { return _Brand; }
            set
            {
                _Brand = value;
                if (Brand != null)
                {
                    var brandSectionClient = new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                    };
                    brandSectionClient.GetTblBrandSectionLinkAsync(Brand, 0);
                }
                RaisePropertyChanged("Brand");
            }
        }


        int? _tblLkpBrandSection;
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
                    _tblLkpBrandSection = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }


        public PurchaseOrderRequestViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.PurchaseOrderRequest.ToString());
                GetCustomePermissions(PermissionItemName.PurchaseOrderRequest.ToString());
                StyleWithCostPrice = true;
                PurchasePlanClient.GeneratePurchaseOrderHeaderRequestPaymentCompleted += (s, sv) =>
                {
                    GetPaymentData();
                };
                Client.AccWithConfigAndSizeCompleted += (s, sv) =>
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

                Client.GetTblPaymentScheduleSettingsAsync(0, 50, "it.Iserial", null, null);
                Client.GetTblPaymentScheduleSettingsCompleted += (s, sv) =>
                {
                    SchedulesSettingList = sv.Result;
                };
                MainRowList = new ObservableCollection<TblPurchaseOrderHeaderRequestModel>();

                PurchasePlanClient.SearchPurchaseOrderDetailCompleted += (s, sv) =>
                {
                    Loading = false;
                    DetailsList.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderDetailViewModel { ColorPerRow = new CRUDManagerService.TblColor() };
                        if (row.TblColor != null) newrow.ColorPerRow.InjectFrom(row.TblColor);
                        newrow.Brand = row.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.Brand;
                        if (row.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpSeason1 != null)
                        {
                            newrow.TblLkpSeason1 = row.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpSeason1;
                        }
                        if (row.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpBrandSection1 != null)
                        {
                            newrow.TblLkpBrandSection1 = row.TblPurchaseOrderHeader1.TblGeneratePurchaseHeader1.TblLkpBrandSection1;
                        }
                        newrow.InjectFrom(row);
                        DetailsList.Add(newrow);
                    }
                };
                var currencyClient = new GlService.GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = new ObservableCollection<GenericTable>();
                    foreach (var item in sv.Result)
                    {
                        if(item.Ename.Trim()!="")
                        CurrencyList.Add(new GenericTable().InjectFrom(item) as GenericTable);
                    }
                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                Client.GetTblWarehouseAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblWarehouseCompleted += (s, sv) =>
                 {
                     WareHouseList = sv.Result;
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
                var tblFactoryDelivery = new CRUD_ManagerServiceClient();
                tblFactoryDelivery.GetGenericCompleted += (s, sv) =>
                {
                    TblFactoryDeliveryList = sv.Result;
                };
                tblFactoryDelivery.GetGenericAsync("TblFactoryDelivery", "%%", "%%", "%%", "Iserial", "ASC");

                var tblPlanTypeList = new CRUD_ManagerServiceClient();

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
                Client.GetAllBrandsAsync(0);
                PurchasePlanClient.GetTblPurchaseOrderHeaderRequestCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderHeaderRequestModel();
                        newrow.InjectFrom(row);
                        newrow.WarehousePerRow = new CRUDManagerService.TblWarehouse();
                        var warehouserow = WareHouseList.FirstOrDefault(w => w.Iserial == row.TblWarehouse);
                        if (warehouserow != null)
                        {
                            newrow.WarehousePerRow.InjectFrom(warehouserow);
                        }
                        newrow.VendorPerRow = new CRUDManagerService.Vendor();
                        newrow.VendorPerRow.InjectFrom(sv.vendorList.FirstOrDefault(w => w.vendor_code == row.Vendor));
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                };


                PurchasePlanClient.GetTblPurchaseOrderHeaderRequestPaymentCompleted += (s, sv) =>
                {
                    SelectedMainRow.PaymentList.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderHeaderRequestPaymentModel();
                        newrow.PaymentSettingPerRow = SchedulesSettingList.FirstOrDefault(w => w.Iserial == row.TblPaymentScheduleSettings);
                        newrow.InjectFrom(row);
                        if (!SelectedMainRow.PaymentList.Any(w => w.Iserial == row.Iserial))
                        {
                            SelectedMainRow.PaymentList.Add(newrow);
                        }

                    }
                    Loading = false;
                };
                PurchasePlanClient.GetTblPurchaseReceiveHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseReceiveHeaderModel();
                        newrow.InjectFrom(row);
                        newrow.WarehousePerRow = new CRUDManagerService.TblWarehouse();
                        if (row.TblWarehouse1 != null)
                        {
                            newrow.WarehousePerRow.InjectFrom(row.TblWarehouse1);
                        }
                        SelectedMainRow.RecHeaderList.Add(newrow);
                    }
                    Loading = false;
                };
                PurchasePlanClient.GetTblPurchaseReceiveDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseReceiveDetailModel();
                        newrow.InjectFrom(row);
                        SelectedSubDetailRow.DetailList.Add(newrow);
                    }
                    Loading = false;
                };
                PurchasePlanClient.GetTblPurchaseOrderDetailRequestCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseOrderDetailRequestModel { ColorPerRow = new CRUDManagerService.TblColor() };
                        if (row.TblColor != null) newrow.ColorPerRow.InjectFrom(row.TblColor);
                        newrow.InjectFrom(row);
                        newrow.Received = sv.purchaseRec.Where(w => w.Key == row.Iserial).Sum(w => w.Value ?? 0);
                        newrow.TblPurchaseRequestLink = row.TblPurchaseRequestLinks;

                        if (newrow.TblPurchaseRequestLink.Any())
                        {
                            newrow.Enabled = false;
                        }
                        else
                        {
                            newrow.Enabled = true;
                        }

                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                };
                PurchasePlanClient.DeleteTblPurchaseReceiveHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.RecHeaderList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.RecHeaderList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblPurchaseReceiveDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedSubDetailRow.DetailList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedSubDetailRow.DetailList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblPurchaseOrderHeaderRequestPaymentCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.PaymentList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.PaymentList.Remove(oldrow);
                };

                PurchasePlanClient.UpdateOrInsertTblPurchaseOrderHeaderRequestCompleted += (s, x) =>
                {
                    if (SelectedMainRow != null) SelectedMainRow.InjectFrom(x.Result);
                    SelectedMainRow.DetailsList.Clear();
                    GetDetailData();
                };

                PurchasePlanClient.UpdateOrInsertTblPurchaseReceiveHeaderCompleted
                    += (s, x) =>
                    {
                        GetDetailData();
                    };

                PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailRequestCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                PurchasePlanClient.DeleteTblPurchaseOrderHeaderRequestCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblPurchaseOrderDetailRequestCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                PurchasePlanClient.GetTblPruchaseOrderTotalCompleted += (s, sv) =>
                {
                    SelectedMainRow.Total = Convert.ToDecimal(sv.Result);
                };

            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc";
            PurchasePlanClient.GetTblPurchaseOrderHeaderRequestAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void GetPaymentData()
        {
            GetOrderTotal();        
            PurchasePlanClient.GetTblPurchaseOrderHeaderRequestPaymentAsync(SelectedMainRow.Iserial);
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
                        PurchasePlanClient.DeleteTblPurchaseOrderHeaderRequestAsync((PurchasePlanService.TblPurchaseOrderHeaderRequest)new PurchasePlanService.TblPurchaseOrderHeaderRequest().InjectFrom(row), MainRowList.IndexOf(row));
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
                var newrow = new TblPurchaseOrderHeaderRequestModel();
                if (PaymTerm.Any())
                {
                    newrow.AxTermOfPaymentCode = PaymTerm.FirstOrDefault().PAYMTERMID;
                }
                else
                {
                    newrow.AxTermOfPaymentCode = "60d";
                }
                if (VendPayModeList.Any())
                {
                    newrow.AxMethodOfPaymentCode = VendPayModeList.FirstOrDefault().PAYMMODE;
                }
                else
                {
                    newrow.AxMethodOfPaymentCode = "cashMethod1";
                }
                if (CurrencyList.Any())
                {
                    newrow.TblCurrency = CurrencyList.FirstOrDefault().Iserial;
                }
                MainRowList.Insert(currentRowIndex + 1, newrow);
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
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

                if (AllowAddFree != true)
                {
                    return;
                }

                var newrow = new TblPurchaseOrderDetailRequestModel();
                newrow.Enabled = true;
                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var saveRow = new PurchasePlanService.TblPurchaseOrderHeaderRequest();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (SelectedMainRow.Iserial!=0)
                    {
                        if (AllowUpdate != true)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }
                    }

                    var detailListToSave = new ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailRequest>();
                    foreach (var item in SelectedMainRow.DetailsList)
                    {
                        var detailrow = new PurchasePlanService.TblPurchaseOrderDetailRequest();
                        var subDetailrow = new PurchasePlanService.TblPurchaseRequestLink();
                        detailrow.InjectFrom(item);
                        detailrow.TblPurchaseRequestLinks = new ObservableCollection<PurchasePlanService.TblPurchaseRequestLink>();
                        if (item.TblPurchaseRequestLink != null)
                        {
                            foreach (var LinkRow in item.TblPurchaseRequestLink)
                            {
                                detailrow.TblPurchaseRequestLinks.Add(LinkRow);
                            }
                        }

                        saveRow.TblPurchaseOrderDetailRequests = new ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailRequest>();
                        detailListToSave.Add(detailrow);
                    }
                    saveRow.TblPurchaseOrderDetailRequests = detailListToSave;
                    PurchasePlanClient.UpdateOrInsertTblPurchaseOrderHeaderRequestAsync(saveRow, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                PurchasePlanClient.GetTblPurchaseOrderDetailRequestAsync(SelectedMainRow.DetailsList.Count, int.MaxValue,
                    SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
            }
        }

        public void DeleteDetailRow()
        {
            if (SelectedMainRow.Status == true)
            {
                MessageBox.Show("Cannot Delete Approved Orders");
                return;
            }

            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            PurchasePlanClient.DeleteTblPurchaseOrderDetailRequestAsync(new PurchasePlanService.TblPurchaseOrderDetailRequest().InjectFrom(row) as PurchasePlanService.TblPurchaseOrderDetailRequest);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        public void SaveRecHeaderRow()
        {
            if (SelectedSubDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSubDetailRow, new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseReceiveHeader();
                    SelectedSubDetailRow.TblPurchaseOrderHeaderRequest = SelectedMainRow.Iserial;

                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    rowToSave.TblWarehouse = SelectedMainRow.TblWarehouse;
                    PurchasePlanClient.UpdateOrInsertTblPurchaseReceiveHeaderAsync(rowToSave, SelectedMainRow.RecHeaderList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.WFM_UserName, LoggedUserInfo.Iserial);
                }
            }
        }

        public void SaveRecDetailRow()
        {
            if (SelectedRecDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedRecDetailRow, new ValidationContext(SelectedRecDetailRow, null, null), valiationCollection, true);
                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseReceiveDetail();
                    if (SelectedDetailRow != null)
                    {
                        SelectedRecDetailRow.TblPurchaseOrderDetail = SelectedDetailRow.Iserial;
                    }

                    SelectedRecDetailRow.TblPurchaseReceiveHeader = SelectedSubDetailRow.Iserial;
                    rowToSave.InjectFrom(SelectedRecDetailRow);
                    PurchasePlanClient.UpdateOrInsertTblPurchaseReceiveDetailAsync(rowToSave, SelectedSubDetailRow.DetailList.IndexOf(SelectedRecDetailRow), LoggedUserInfo.WFM_UserName);
                }
            }
        }

        public void DeleteRecDetail()
        {
            if (_selectedRecDetails != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedRecDetails)
                    {
                        PurchasePlanClient.DeleteTblPurchaseReceiveDetailAsync((PurchasePlanService.TblPurchaseReceiveDetail)new PurchasePlanService.TblPurchaseReceiveDetail().InjectFrom(row), SelectedSubDetailRow.DetailList.IndexOf(row));
                    }
                }
            }
        }

        public void DeleteRecHeader()
        {
            if (SelectedRecHeaders != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedRecHeaders)
                    {
                        PurchasePlanClient.DeleteTblPurchaseReceiveHeaderAsync((PurchasePlanService.TblPurchaseReceiveHeader)new PurchasePlanService.TblPurchaseReceiveHeader().InjectFrom(row), SelectedMainRow.RecHeaderList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewPaymentRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.PaymentList.IndexOf(SelectedPaymentRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.PaymentList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedPaymentRow, new ValidationContext(SelectedPaymentRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }

                if (AllowAddFree != true)
                {
                    return;
                }

                var newrow = new TblPurchaseOrderHeaderRequestPaymentModel();

                SelectedMainRow.PaymentList.Insert(currentRowIndex + 1, newrow);
            }
        }

        public void DeletePaymentRow()
        {
            if (SelectedPaymentRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedPaymentRows)
                    {
                        PurchasePlanClient.DeleteTblPurchaseOrderHeaderRequestPaymentAsync((PurchasePlanService.TblPurchaseOrderHeaderRequestPayment)new PurchasePlanService.TblPurchaseOrderHeaderRequestPayment().InjectFrom(row), SelectedMainRow.PaymentList.IndexOf(row));
                    }
                }
            }
        }

        public void GetRecHeader()
        {
            if (SelectedMainRow != null)
            {
                PurchasePlanClient.GetTblPurchaseReceiveHeaderAsync(SelectedMainRow.RecHeaderList.Count, PageSize,
                    SelectedMainRow.Iserial, DetailSortBy, DetailSubFilter, DetailSubValuesObjects);
            }
        }

        public void GetRecDetail()
        {
            if (SelectedSubDetailRow != null)
            {
                PurchasePlanClient.GetTblPurchaseReceiveDetailAsync(SelectedSubDetailRow.DetailList.Count, PageSize,
                    SelectedSubDetailRow.Iserial, DetailSortBy, RecDetailFilter, RecDetailValuesObjects);
            }
        }

        public void SaveDetailRow()
        {
            if (SelectedMainRow.Iserial != 0)
            {
                if (SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var rowToSave = new PurchasePlanService.TblPurchaseOrderDetailRequest();
                        SelectedDetailRow.TblPurchaseOrderHeaderRequest = SelectedMainRow.Iserial;
                        rowToSave.InjectFrom(SelectedDetailRow);

                        if (rowToSave.Iserial != 0)
                        {
                            rowToSave.TblPurchaseRequestLinks = null;
                        }
                        else
                        {
                            var subDetailrow = new PurchasePlanService.TblPurchaseRequestLink();
                            rowToSave.TblPurchaseRequestLinks = new ObservableCollection<PurchasePlanService.TblPurchaseRequestLink>();
                            if (SelectedDetailRow.TblPurchaseRequestLink != null)
                            {
                                foreach (var LinkRow in SelectedDetailRow.TblPurchaseRequestLink)
                                {
                                    rowToSave.TblPurchaseRequestLinks.Add(LinkRow);
                                }
                            }
                        }
                        PurchasePlanClient.UpdateOrInsertTblPurchaseOrderDetailRequestAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                    }
                }
            }
        }

        public void SearchHeader()
        {
            MainRowList.Clear();
            var child = new PurchaseOrderRequestSearchChild(this);
            child.Show();
        }

        public void DeleteOrder()
        {

            if (SelectedMainRow.Status == true)
            {
                MessageBox.Show("Cannot Delete Approved Orders");
                return;
            }

            if (AllowDelete)
            {
                PurchasePlanClient.DeleteTblPurchaseOrderHeaderRequestAsync((PurchasePlanService.TblPurchaseOrderHeaderRequest)new PurchasePlanService.TblPurchaseOrderHeaderRequest().InjectFrom(SelectedMainRow), MainRowList.IndexOf(SelectedMainRow));
            }
            else
            {
                MessageBox.Show(strings.AllowDeleteMsg);
            }

        }

        public void SeachPendingPurchase()
        {
            Loading = true;
            var currecycode = CurrencyList.FirstOrDefault(w => w.Iserial == SelectedMainRow.TblCurrency).Code;
            if (ItemPerRow != null)
            {
                PurchasePlanClient.SearchPurchaseOrderDetailAsync(Vendor, ItemPerRow.Code, TblColor, Brand, TblLkpBrandSection, TblLkpSeason, FromDate, ToDate, currecycode, StyleWithCostPrice);
            }
            else
            {
                PurchasePlanClient.SearchPurchaseOrderDetailAsync(Vendor, "", TblColor, Brand, TblLkpBrandSection, TblLkpSeason, FromDate, ToDate, currecycode, StyleWithCostPrice);

            }

        }

        public void ReceivePurchase(bool receiveAll)
        {
            var newrow = new PurchasePlanService.TblPurchaseReceiveHeader();
            newrow.InjectFrom(SelectedSubDetailRow);
            newrow.TblPurchaseReceiveDetails = new ObservableCollection<PurchasePlanService.TblPurchaseReceiveDetail>();
            if (receiveAll)
            {
                foreach (var item in SelectedMainRow.DetailsList.ToList())//.Where(w => w.Received <= w.Qty))
                {
                    if (item.Received == 0)
                    {
                        item.ReceiveNow = item.Qty;
                    }
                    else if (item.Received < item.Qty)
                    {
                        item.ReceiveNow = item.Qty - item.Received;
                    }

                    if (item.Qty < 0)
                    {
                        if (item.Received > item.Qty)
                        {
                            item.ReceiveNow = item.Qty - item.Received;
                        }
                    }

                    if (item.Received == item.Qty)
                    {
                        item.ReceiveNow = 0;
                    }

                }
            }

            foreach (var item in SelectedMainRow.DetailsList.Where(x => x.ReceiveNow != 0))
            {
                var newdetailrow = new PurchasePlanService.TblPurchaseReceiveDetail
                {
                    Cost = item.Price,
                    OldCost = item.Price,
                    Qty = item.ReceiveNow,
                    BatchNo = item.BatchNo,
                    TblPurchaseOrderDetailRequest = item.Iserial
                };
                newrow.TblPurchaseReceiveDetails.Add(newdetailrow);
            }
            newrow.Vendor = SelectedMainRow.Vendor;
            newrow.TblWarehouse = SelectedMainRow.TblWarehouse;
            PurchasePlanClient.UpdateOrInsertTblPurchaseReceiveHeaderAsync(newrow, 0, LoggedUserInfo.WFM_UserName, LoggedUserInfo.Iserial);
        }

        void GetOrderTotal()
        {
            PurchasePlanClient.GetTblPruchaseOrderTotalAsync(SelectedMainRow.Iserial);
        }
        internal void Approve()
        {
            SelectedMainRow.Status = true;
            SelectedMainRow.ApprovalDate = DateTime.Now;
            SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
            SaveMainRow();
        }

        internal void SavePaymentRow()
        {

            ObservableCollection<PurchasePlanService.TblPurchaseOrderHeaderRequestPayment> ListToSave = new ObservableCollection<PurchasePlanService.TblPurchaseOrderHeaderRequestPayment>();
            foreach (var item in SelectedMainRow.PaymentList.ToList())
            {

                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new PurchasePlanService.TblPurchaseOrderHeaderRequestPayment();
                    item.TblPurchaseOrderHeaderRequest = SelectedMainRow.Iserial;
                    rowToSave.InjectFrom(item);
                    ListToSave.Add(rowToSave);

                }
                else
                {
                    MessageBox.Show("Data Is Not Valid");
                    return;
                }

            }
            var totalPayment = Convert.ToInt32(ListToSave.Sum(w => w.Amount));
            if (totalPayment == Convert.ToInt32(SelectedMainRow.Total))
            {
                PurchasePlanClient.UpdateOrInsertTblPurchaseOrderHeaderRequestPaymentsAsync(ListToSave);
            }
            else
            {
                MessageBox.Show("Payment amount should same as expected order payment");
            }


        }

        internal void GeneratePayment()
        {
            if (SelectedMainRow.PaymentList.Any(w => w.Iserial > 0))
            {
                MessageBox.Show("Please delete all the transactions to regenerate Payments");
                return;
            }
            if (SelectedMainRow.PaymentSettings.Any(w => w.PaymentScheduleSetting == null))
            {
                MessageBox.Show("Data Is Not Valid");
                return;
            }
            if (SelectedMainRow.PaymentSettings.Any())
            {
                if (SelectedMainRow.PaymentSettings.Sum(w => w.Percentage) == 100)
                {
                    float amount = Convert.ToSingle(SelectedMainRow.Total);
                    //Convert.ToSingle(SelectedMainRow.DetailsList.Sum(w => w.Qty * w.Price));
                    DateTime date = DateTime.Now;
                    foreach (var item in SelectedMainRow.PaymentSettings)
                    {
                        if (item.PaymentSettingPerRow.Code == "DownPayment")
                        {
                            date = SelectedMainRow.CreationDate;
                        }
                        else
                        {
                            date = SelectedMainRow.DeliveryDate ?? DateTime.Now;
                        }
                        date = date.AddDays(item.StartingDays);

                        var desc = item.Description;
                        if (string.IsNullOrWhiteSpace(desc))
                        {
                            desc = item.PaymentSettingPerRow.Code;
                        }
                        PurchasePlanClient.GeneratePurchaseOrderHeaderRequestPaymentAsync(SelectedMainRow.Iserial, date, item.Percentage, item.InstallmentCount, item.InstallmentInterval, amount, desc, item.PaymentScheduleSetting ?? 0);
                    }
                }
                else
                {
                    MessageBox.Show("Percentage Must be 100");
                }
            }
            else
            {
                MessageBox.Show("Data Is Not Valid");
            }

        }

        internal void GetPlanFabric()
        {
            if (!string.IsNullOrEmpty(SelectedMainRow.PlanCode) && !string.IsNullOrEmpty(SelectedMainRow.vendor_code))
            {
                PurchasePlanClient.GetTblPurchaseOrderDetailRequestByPlanCodeAsync(SelectedMainRow.PlanCode, SelectedMainRow.vendor_code);
                PurchasePlanClient.GetTblPurchaseOrderDetailRequestByPlanCodeCompleted += (s, sv) =>
                {
                    SelectedMainRow.DetailsList.Clear();
                    foreach (var item in sv.Result)
                    {
                        SelectedMainRow.TblGeneratePurchaseHeader = item.TblGeneratePurchaseHeader;
                        foreach (var itemDetails in item.TblPurchaseOrderDetails)
                        {
                            var newrow = new TblPurchaseOrderDetailRequestModel { ColorPerRow = new CRUDManagerService.TblColor() };
                            if (itemDetails.FabricColor != null) newrow.ColorPerRow.InjectFrom(itemDetails.TblColor);
                            newrow.DeliveryDate = item.DeliveryDate;
                            newrow.ShippingDate = item.ShippingDate;
                            newrow.InjectFrom(itemDetails);
                            newrow.Enabled = true;
                            SelectedMainRow.DetailsList.Add(newrow);


                            //newrow.Received = item.TblGeneratePurchaseHeader1.purchaseRec.Where(w => w.Key == row.Iserial).Sum(w => w.Value ?? 0);
                            //newrow.TblPurchaseRequestLink = row.TblPurchaseRequestLinks;
                            //if (newrow.TblPurchaseRequestLink.Any())
                            //{
                            //    newrow.Enabled = false;
                            //}
                            //else
                            //{
                            //    newrow.Enabled = true;
                            //}
                        }
                    }
                };
            }
        }

        internal void GetPlanVendors()
        {

            if (!string.IsNullOrEmpty(SelectedMainRow.PlanCode))
            {
                PurchasePlanClient.GetVendorsByPlanCodeAsync(SelectedMainRow.PlanCode);
                PurchasePlanClient.GetVendorsByPlanCodeCompleted += (s, sv) =>
                {
                    PlanVendorList.Clear();
                    var vendors = sv.Result;

                    foreach (var row in sv.Result)
                    {
                        if (PlanVendorList.All(x => x.vendor_code != row.vendor_code))
                        {
                            PlanVendorList.Add(row);
                        }
                    }
                };
            }
        }

        #region Prop     
        private ObservableCollection<GenericTable> _CurrencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _CurrencyList; }
            set { _CurrencyList = value; RaisePropertyChanged("CurrencyList"); }
        }



        private ObservableCollection<CRUDManagerService.TblWarehouse> _wareHouseList;

        public ObservableCollection<CRUDManagerService.TblWarehouse> WareHouseList
        {
            get { return _wareHouseList; }
            set { _wareHouseList = value; RaisePropertyChanged("WareHouseList"); }
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

        private ObservableCollection<PurchasePlanService.Vendor> _PlanVendorList;

        public ObservableCollection<PurchasePlanService.Vendor> PlanVendorList
        {
            get { return _PlanVendorList ?? (_PlanVendorList = new ObservableCollection<PurchasePlanService.Vendor>()); }
            set { _PlanVendorList = value; RaisePropertyChanged("PlanVendorList"); }
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

        private ObservableCollection<TblPurchaseOrderHeaderRequestModel> _mainRowList;

        public ObservableCollection<TblPurchaseOrderHeaderRequestModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPurchaseOrderHeaderRequestModel> _selectedMainRows;

        public ObservableCollection<TblPurchaseOrderHeaderRequestModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPurchaseOrderHeaderRequestModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPurchaseOrderHeaderRequestModel _selectedMainRow;

        public TblPurchaseOrderHeaderRequestModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblPurchaseOrderHeaderRequestModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }



        private TblPurchaseOrderHeaderRequestPaymentModel _SelectedPaymentRow;

        public TblPurchaseOrderHeaderRequestPaymentModel SelectedPaymentRow
        {
            get { return _SelectedPaymentRow ?? (_SelectedPaymentRow = new TblPurchaseOrderHeaderRequestPaymentModel()); }
            set { _SelectedPaymentRow = value; RaisePropertyChanged("SelectedPaymentRow"); }
        }





        private TblPurchaseOrderDetailRequestModel _selectedDetailRow;

        public TblPurchaseOrderDetailRequestModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblPurchaseOrderDetailRequestModel> _selectedDetailRows;

        public ObservableCollection<TblPurchaseOrderDetailRequestModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPurchaseOrderDetailRequestModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        public string RecDetailFilter { get; set; }

        public Dictionary<string, object> RecDetailValuesObjects { get; set; }

        public string PaymentDetailFilter { get; set; }

        public Dictionary<string, object> PaymentDetailValuesObjects { get; set; }
        private TblPurchaseReceiveHeaderModel _selectedSubDetailRow;

        public TblPurchaseReceiveHeaderModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow ?? (_selectedSubDetailRow = new TblPurchaseReceiveHeaderModel()); }
            set { _selectedSubDetailRow = value; RaisePropertyChanged("SelectedSubDetailRow"); }
        }

        private TblPurchaseReceiveDetailModel _selectedRecDetailRow;

        public TblPurchaseReceiveDetailModel SelectedRecDetailRow
        {
            get { return _selectedRecDetailRow ?? (_selectedRecDetailRow = new TblPurchaseReceiveDetailModel()); }
            set { _selectedRecDetailRow = value; RaisePropertyChanged("SelectedRecDetailRow"); }
        }

        private ObservableCollection<TblPurchaseReceiveHeaderModel> _selectedRecHeaders;

        public ObservableCollection<TblPurchaseReceiveHeaderModel> SelectedRecHeaders
        {
            get { return _selectedRecHeaders ?? (_selectedRecHeaders = new ObservableCollection<TblPurchaseReceiveHeaderModel>()); }
            set { _selectedRecHeaders = value; RaisePropertyChanged("SelectedRecHeaders"); }
        }
        private ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel> _SelectedPaymentRows;

        public ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel> SelectedPaymentRows
        {
            get { return _SelectedPaymentRows ?? (_SelectedPaymentRows = new ObservableCollection<TblPurchaseOrderHeaderRequestPaymentModel>()); }
            set { _SelectedPaymentRows = value; RaisePropertyChanged("SelectedPaymentRows"); }
        }



        private ObservableCollection<TblPurchaseReceiveDetailModel> _selectedRecDetails;

        public ObservableCollection<TblPurchaseReceiveDetailModel> SelectedRecDetails
        {
            get { return _selectedRecDetails ?? (_selectedRecDetails = new ObservableCollection<TblPurchaseReceiveDetailModel>()); }
            set { _selectedRecDetails = value; RaisePropertyChanged("SelectedRecDetails"); }
        }

        private ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown> _selectedPurchaseOrderDetailBreakDowns;

        public ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown> SelectedPurchaseOrderDetailBreakDowns
        {
            get
            {
                return _selectedPurchaseOrderDetailBreakDowns ?? (_selectedPurchaseOrderDetailBreakDowns = new ObservableCollection<PurchasePlanService.TblPurchaseOrderDetailBreakDown>());
            }
            set { _selectedPurchaseOrderDetailBreakDowns = value; RaisePropertyChanged("SelectedPurchaseOrderDetailBreakDowns"); }
        }

        private ObservableCollection<TblPurchaseOrderDetailViewModel> _DetailsList;

        public ObservableCollection<TblPurchaseOrderDetailViewModel> DetailsList
        {
            get
            {
                return _DetailsList ?? (_DetailsList = new ObservableCollection<TblPurchaseOrderDetailViewModel>());
            }
            set { _DetailsList = value; RaisePropertyChanged("DetailsList"); }
        }

        private ObservableCollection<CRUDManagerService.TblPaymentScheduleSetting> _SchedulesSettingList;

        public ObservableCollection<CRUDManagerService.TblPaymentScheduleSetting> SchedulesSettingList
        {
            get
            {
                return _SchedulesSettingList ?? (_SchedulesSettingList = new ObservableCollection<CRUDManagerService.TblPaymentScheduleSetting>());
            }
            set { _SchedulesSettingList = value; RaisePropertyChanged("SchedulesSettingList"); }
        }

        #endregion Prop    
    }
}