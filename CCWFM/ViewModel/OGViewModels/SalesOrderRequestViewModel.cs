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
using CCWFM.WarehouseService;
using GalaSoft.MvvmLight.Command;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using System.Collections.Specialized;

namespace CCWFM.ViewModel.OGViewModels
{
    #region Model

    public class TblSalesOrderHeaderRequestModel : CRUDManagerService.PropertiesViewModelBase
    {
        private ObservableCollection<TblMarkupTranProdViewModel> _markUpTransList;
    
        public ObservableCollection<TblMarkupTranProdViewModel> MarkUpTransList
        {
            get { return _markUpTransList ?? (_markUpTransList = new ObservableCollection<TblMarkupTranProdViewModel>()); }
            set
            {
                _markUpTransList = value;
                RaisePropertyChanged("MarkUpTransList");
            }
        }

        DateTime? DeliveryDateField;
        public DateTime? DeliveryDate
        {
            get
            {
                return DeliveryDateField ?? (DeliveryDateField = DateTime.Now);
            }
            set
            {
                if ((DeliveryDateField.Equals(value) != true))
                {
                    DeliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }

        DateTime? _TransDate;
        public DateTime? TransDate
        {
            get
            {
                return _TransDate ?? (_TransDate = DateTime.Now);
            }
            set
            {
                if ((_TransDate.Equals(value) != true))
                {
                    _TransDate = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }

        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private string AxMethodOfPaymentCodeField;

        private string AxTermOfPaymentCodeField;

        private string CodeField;

        private int? CreatedByField;

        private DateTime? CreationDateField;

        private int EntityAccountField;

        private int IserialField;

        private int? LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private DateTime? ShippingDateField;

        private bool StatusField;

        private int? TblCurrencyField;

        private CRUDManagerService.GenericTable TblCurrency1Field;

        private int TblJournalAccountTypeField;

        private ObservableCollection<TblSalesIssueHeaderModel> TblSalesIssueHeadersField;

        private int? TblSalesPersonField;

        private CRUDManagerService.GenericTable TblSalesPerson1Field;

        private int? TblWarehouseField;

        private CRUDManagerService.TblWarehouse WareHousePerRowField;

        public string AxMethodOfPaymentCode
        {
            get
            {
                return this.AxMethodOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(this.AxMethodOfPaymentCodeField, value) != true))
                {
                    this.AxMethodOfPaymentCodeField = value;
                    RaisePropertyChanged("AxMethodOfPaymentCode");
                }
            }
        }

        public string AxTermOfPaymentCode
        {
            get
            {
                return this.AxTermOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(this.AxTermOfPaymentCodeField, value) != true))
                {
                    this.AxTermOfPaymentCodeField = value;
                    RaisePropertyChanged("AxTermOfPaymentCode");
                }
            }
        }

        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                if ((ReferenceEquals(this.CodeField, value) != true))
                {
                    this.CodeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public int? CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {
                if ((this.CreatedByField.Equals(value) != true))
                {
                    this.CreatedByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return this.CreationDateField;
            }
            set
            {
                if ((this.CreationDateField.Equals(value) != true))
                {
                    this.CreationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public int EntityAccount
        {
            get
            {
                return this.EntityAccountField;
            }
            set
            {
                if ((this.EntityAccountField.Equals(value) != true))
                {
                    this.EntityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int? LastUpdatedBy
        {
            get
            {
                return this.LastUpdatedByField;
            }
            set
            {
                if ((this.LastUpdatedByField.Equals(value) != true))
                {
                    this.LastUpdatedByField = value;
                    RaisePropertyChanged("LastUpdatedBy");
                }
            }
        }

        public DateTime? LastUpdatedDate
        {
            get
            {
                return this.LastUpdatedDateField;
            }
            set
            {
                if ((this.LastUpdatedDateField.Equals(value) != true))
                {
                    this.LastUpdatedDateField = value;
                    RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }

        public DateTime? ShippingDate
        {
            get
            {
                return ShippingDateField ?? (ShippingDateField = DateTime.Now);
            }
            set
            {
                if ((ShippingDateField.Equals(value) != true))
                {
                    ShippingDateField = value;
                    RaisePropertyChanged("ShippingDate");
                }
            }
        }

        public bool Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                if ((this.StatusField.Equals(value) != true))
                {
                    this.StatusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public int? TblCurrency
        {
            get
            {
                return this.TblCurrencyField;
            }
            set
            {
                if ((this.TblCurrencyField.Equals(value) != true))
                {
                    this.TblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public CRUDManagerService.GenericTable CurrencyPerRow
        {
            get
            {
                return this.TblCurrency1Field;
            }
            set
            {
                if ((ReferenceEquals(this.TblCurrency1Field, value) != true))
                {
                    this.TblCurrency1Field = value;
                    RaisePropertyChanged("CurrencyPerRow");
                }
            }
        }

        public int TblJournalAccountType
        {
            get
            {
                return this.TblJournalAccountTypeField;
            }
            set
            {
                if ((this.TblJournalAccountTypeField.Equals(value) != true))
                {
                    this.TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }


        public ObservableCollection<TblSalesIssueHeaderModel> RecHeaderList
        {
            get
            {
                return this.TblSalesIssueHeadersField??(TblSalesIssueHeadersField= new ObservableCollection<TblSalesIssueHeaderModel>());
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesIssueHeadersField, value) != true))
                {
                    this.TblSalesIssueHeadersField = value;
                    RaisePropertyChanged("RecHeaderList");
                }
            }
        }

        ObservableCollection<TblSalesOrderDetailRequestModel> DetailsListField;
        public ObservableCollection<TblSalesOrderDetailRequestModel> DetailsList
        {
            get
            {
                return this.DetailsListField ?? (DetailsListField = new ObservableCollection<TblSalesOrderDetailRequestModel>());
            }
            set
            {
                if ((ReferenceEquals(this.DetailsListField, value) != true))
                {
                    this.DetailsListField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }


        public int? TblSalesPerson
        {
            get
            {
                return this.TblSalesPersonField;
            }
            set
            {
                if ((this.TblSalesPersonField.Equals(value) != true))
                {
                    this.TblSalesPersonField = value;
                    RaisePropertyChanged("TblSalesPerson");
                }
            }
        }


        public CRUDManagerService.GenericTable SalesPersonPerRow
        {
            get
            {
                return this.TblSalesPerson1Field;
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesPerson1Field, value) != true))
                {
                    this.TblSalesPerson1Field = value;
                    RaisePropertyChanged("SalesPersonPerRow");
                }
            }
        }


        public int? TblWarehouse
        {
            get
            {
                return this.TblWarehouseField;
            }
            set
            {
                if ((this.TblWarehouseField.Equals(value) != true))
                {
                    this.TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }


        public CRUDManagerService.TblWarehouse WareHousePerRow
        {
            get
            {
                return this.WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(this.WareHousePerRowField, value) != true))
                {
                    this.WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }

        private GlService.Entity _EntityPerRow;

        public GlService.Entity EntityPerRow
        {
            get { return _EntityPerRow; }
            set
            {
                _EntityPerRow = value; RaisePropertyChanged("EntityPerRow");
                EntityAccount = _EntityPerRow.Iserial;
            }
        }


        private GlService.GenericTable _JournalAccountTypePerRow;

        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _JournalAccountTypePerRow; }
            set
            {
                _JournalAccountTypePerRow = value; RaisePropertyChanged("JournalAccountTypePerRow");
                TblJournalAccountType = _JournalAccountTypePerRow.Iserial;
            }
        }
    }

    public class TblSalesOrderDetailRequestModel : CRUDManagerService.PropertiesViewModelBase
    {
        private decimal _received;

        public decimal Received
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

        private Web.DataLayer.ItemDimensionSearchModel itemTransfer;

        public virtual Web.DataLayer.ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new Web.DataLayer.ItemDimensionSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemTransfer, value) != true))
                {
                    itemTransfer = value;
                }
            }
        }
        private decimal? BasicPriceField;

        private DateTime? DeliveryDateField;

        private int IserialField;

        private decimal? PriceField;

        private decimal? QtyField;

        private decimal? ReceiveNowField;

        private DateTime? ShippingDateField;

        private int TblItemDimField;

        private int? TblSalesOrderHeaderRequestField;

        private string UnitField;


        public decimal? BasicPrice
        {
            get
            {
                return this.BasicPriceField;
            }
            set
            {
                if ((this.BasicPriceField.Equals(value) != true))
                {
                    this.BasicPriceField = value;
                    RaisePropertyChanged("BasicPrice");
                }
            }
        }


        public DateTime? DeliveryDate
        {
            get
            {
                return DeliveryDateField;
            }
            set
            {
                if ((DeliveryDateField.Equals(value) != true))
                {
                    DeliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public decimal? Price
        {
            get
            {
                return this.PriceField;
            }
            set
            {
                if ((this.PriceField.Equals(value) != true))
                {
                    this.PriceField = value;
                    Total = Qty * Price;
                    RaisePropertyChanged("Price");
                }
            }
        }


        public decimal? Qty
        {
            get
            {
                return this.QtyField;
            }
            set
            {
                if ((this.QtyField.Equals(value) != true))
                {
                    this.QtyField = value;
                    Total = Qty * Price;
                    RaisePropertyChanged("Qty");
                }
            }
        }


        public decimal? ReceiveNow
        {
            get
            {
                return this.ReceiveNowField;
            }
            set
            {
                if ((this.ReceiveNowField.Equals(value) != true))
                {
                    this.ReceiveNowField = value;
                    RaisePropertyChanged("ReceiveNow");
                }
            }
        }

        private decimal? _Total;

        public decimal? Total
        {
            get { return _Total; }
            set { _Total = value; RaisePropertyChanged("Total"); }
        }


        public DateTime? ShippingDate
        {
            get
            {
                return ShippingDateField;
            }
            set
            {
                if ((ShippingDateField.Equals(value) != true))
                {
                    ShippingDateField = value;
                    RaisePropertyChanged("ShippingDate");
                }
            }
        }


        public int TblItemDim
        {
            get
            {
                return this.TblItemDimField;
            }
            set
            {
                if ((this.TblItemDimField.Equals(value) != true))
                {
                    this.TblItemDimField = value;
                    RaisePropertyChanged("TblItemDim");
                }
            }
        }

        public int? TblSalesOrderHeaderRequest
        {
            get
            {
                return this.TblSalesOrderHeaderRequestField;
            }
            set
            {
                if ((this.TblSalesOrderHeaderRequestField.Equals(value) != true))
                {
                    this.TblSalesOrderHeaderRequestField = value;
                    RaisePropertyChanged("TblSalesOrderHeaderRequest");
                }
            }
        }

        public string Unit
        {
            get
            {
                return this.UnitField;
            }
            set
            {
                if ((ReferenceEquals(this.UnitField, value) != true))
                {
                    this.UnitField = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }
    }

    public class TblSalesIssueHeaderModel : CRUDManagerService.PropertiesViewModelBase
    {

        private int CreatedByField;

        private DateTime? CreationDateField;

        private string DocCodeField;

        private DateTime? DocDateField;

        private int EntityAccountField;

        private int IserialField;

        private int LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private string NotesField;

        private string RefNoField;

        private int TblInventTypeField;

        private int TblJournalAccountTypeField;

        private System.Collections.ObjectModel.ObservableCollection<TblSalesIssueDetailModel> TblSalesIssueDetailsField;

        private int? TblSalesOrderHeaderRequestField;

        private CCWFM.WarehouseService.TblSalesOrderHeaderRequest TblSalesOrderHeaderRequest1Field;


        private System.Collections.ObjectModel.ObservableCollection<CCWFM.WarehouseService.TblSalesOrderRequestInvoiceDetail> TblSalesOrderRequestInvoiceDetailsField;

        private int? TblWarehouseField;

        private CCWFM.WarehouseService.TblWarehouse WareHousePerRowField;

        public int CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {

                this.CreatedByField = value;
                RaisePropertyChanged("CreatedBy");

            }
        }


        public DateTime? CreationDate
        {
            get
            {
                return this.CreationDateField;
            }
            set
            {
                if ((this.CreationDateField.Equals(value) != true))
                {
                    this.CreationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }


        public string DocCode
        {
            get
            {
                return this.DocCodeField;
            }
            set
            {
                if ((ReferenceEquals(this.DocCodeField, value) != true))
                {
                    this.DocCodeField = value;
                    RaisePropertyChanged("DocCode");
                }
            }
        }


        public DateTime? DocDate
        {
            get
            {
                return this.DocDateField;
            }
            set
            {
                if ((this.DocDateField.Equals(value) != true))
                {
                    this.DocDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }


        public int EntityAccount
        {
            get
            {
                return this.EntityAccountField;
            }
            set
            {
                if ((this.EntityAccountField.Equals(value) != true))
                {
                    this.EntityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public int LastUpdatedBy
        {
            get
            {
                return this.LastUpdatedByField;
            }
            set
            {

                this.LastUpdatedByField = value;
                RaisePropertyChanged("LastUpdatedBy");

            }
        }


        public DateTime? LastUpdatedDate
        {
            get
            {
                return this.LastUpdatedDateField;
            }
            set
            {
                if ((this.LastUpdatedDateField.Equals(value) != true))
                {
                    this.LastUpdatedDateField = value;
                    RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }


        public string Notes
        {
            get
            {
                return this.NotesField;
            }
            set
            {
                if ((ReferenceEquals(this.NotesField, value) != true))
                {
                    this.NotesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }


        public string RefNo
        {
            get
            {
                return this.RefNoField;
            }
            set
            {
                if ((ReferenceEquals(this.RefNoField, value) != true))
                {
                    this.RefNoField = value;
                    RaisePropertyChanged("RefNo");
                }
            }
        }


        public int TblInventType
        {
            get
            {
                return this.TblInventTypeField;
            }
            set
            {
                if ((this.TblInventTypeField.Equals(value) != true))
                {
                    this.TblInventTypeField = value;
                    RaisePropertyChanged("TblInventType");
                }
            }
        }


        public int TblJournalAccountType
        {
            get
            {
                return this.TblJournalAccountTypeField;
            }
            set
            {
                if ((this.TblJournalAccountTypeField.Equals(value) != true))
                {
                    this.TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }


        public System.Collections.ObjectModel.ObservableCollection<TblSalesIssueDetailModel> TblSalesIssueDetails
        {
            get
            {
                return this.TblSalesIssueDetailsField;
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesIssueDetailsField, value) != true))
                {
                    this.TblSalesIssueDetailsField = value;
                    RaisePropertyChanged("TblSalesIssueDetails");
                }
            }
        }


        public int? TblSalesOrderHeaderRequest
        {
            get
            {
                return this.TblSalesOrderHeaderRequestField;
            }
            set
            {
                if ((this.TblSalesOrderHeaderRequestField.Equals(value) != true))
                {
                    this.TblSalesOrderHeaderRequestField = value;
                    RaisePropertyChanged("TblSalesOrderHeaderRequest");
                }
            }
        }


        public CCWFM.WarehouseService.TblSalesOrderHeaderRequest TblSalesOrderHeaderRequest1
        {
            get
            {
                return this.TblSalesOrderHeaderRequest1Field;
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesOrderHeaderRequest1Field, value) != true))
                {
                    this.TblSalesOrderHeaderRequest1Field = value;
                    RaisePropertyChanged("TblSalesOrderHeaderRequest1");
                }
            }
        }



        public System.Collections.ObjectModel.ObservableCollection<CCWFM.WarehouseService.TblSalesOrderRequestInvoiceDetail> TblSalesOrderRequestInvoiceDetails
        {
            get
            {
                return this.TblSalesOrderRequestInvoiceDetailsField;
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesOrderRequestInvoiceDetailsField, value) != true))
                {
                    this.TblSalesOrderRequestInvoiceDetailsField = value;
                    RaisePropertyChanged("TblSalesOrderRequestInvoiceDetails");
                }
            }
        }


        public int? TblWarehouse
        {
            get
            {
                return this.TblWarehouseField;
            }
            set
            {
                if ((this.TblWarehouseField.Equals(value) != true))
                {
                    this.TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }


        public CCWFM.WarehouseService.TblWarehouse WareHousePerRow
        {
            get
            {
                return this.WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(this.WareHousePerRowField, value) != true))
                {
                    this.WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }


    }

    public partial class TblSalesIssueDetailModel : CRUDManagerService.PropertiesViewModelBase
    {

        private int IserialField;

        private decimal? PriceField;

        private decimal? QtyField;

        private int? TblSalesIssueHeaderField;


        private int? TblSalesOrderDetailRequestField;

        private CCWFM.WarehouseService.TblSalesOrderDetailRequest TblSalesOrderDetailRequest1Field;


        private int TypeField;


        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public decimal? Price
        {
            get
            {
                return this.PriceField;
            }
            set
            {
                if ((this.PriceField.Equals(value) != true))
                {
                    this.PriceField = value;
                    RaisePropertyChanged("Price");
                }
            }
        }


        public decimal? Qty
        {
            get
            {
                return this.QtyField;
            }
            set
            {
                if ((this.QtyField.Equals(value) != true))
                {
                    this.QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }


        public int? TblSalesIssueHeader
        {
            get
            {
                return this.TblSalesIssueHeaderField;
            }
            set
            {
                if ((this.TblSalesIssueHeaderField.Equals(value) != true))
                {
                    this.TblSalesIssueHeaderField = value;
                    RaisePropertyChanged("TblSalesIssueHeader");
                }
            }
        }


        public int? TblSalesOrderDetailRequest
        {
            get
            {
                return this.TblSalesOrderDetailRequestField;
            }
            set
            {
                if ((this.TblSalesOrderDetailRequestField.Equals(value) != true))
                {
                    this.TblSalesOrderDetailRequestField = value;
                    RaisePropertyChanged("TblSalesOrderDetailRequest");
                }
            }
        }


        public CCWFM.WarehouseService.TblSalesOrderDetailRequest TblSalesOrderDetailRequest1
        {
            get
            {
                return this.TblSalesOrderDetailRequest1Field;
            }
            set
            {
                if ((ReferenceEquals(this.TblSalesOrderDetailRequest1Field, value) != true))
                {
                    this.TblSalesOrderDetailRequest1Field = value;
                    RaisePropertyChanged("TblSalesOrderDetailRequest1");
                }
            }
        }


        public int Type
        {
            get
            {
                return this.TypeField;
            }
            set
            {
                if ((this.TypeField.Equals(value) != true))
                {
                    this.TypeField = value;
                    RaisePropertyChanged("Type");
                }
            }
        }
    }

    #endregion

    public class SalesOrderRequestViewModel : ViewModelStructuredBase
    {
        public SalesOrderRequestViewModel() : base(PermissionItemName.SalesOrderRequest)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                OpenItemSearch = new RelayCommand(() =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(SelectedMainRow.WareHousePerRow.Code))
                        {
                            var vm = new ItemDimensionSearchViewModel();
                            vm.WarehouseCode = SelectedMainRow.WareHousePerRow.Code;
                            vm.SiteIserial = SelectedMainRow.WareHousePerRow.TblSite;
                            vm.AppliedSearchResultList.CollectionChanged += (s, e) =>
                            {
                                // هنا هبدا اعبى الى جاى من السيرش
                                foreach (var item in vm.AppliedSearchResultList)
                                {
                                    var temp = SelectedMainRow.DetailsList.FirstOrDefault(td => td.TblItemDim == item.ItemDimFromIserial);
                                    if (item.AvailableQuantity < item.TransferredQuantity)
                                        continue;
                                    if (temp == null)// مش موجود
                                    {
                                        var transferDetail = new TblSalesOrderDetailRequestModel()
                                        {
                                            TblSalesOrderHeaderRequest = SelectedMainRow.Iserial,
                                            TblItemDim = item.ItemDimFromIserial,
                                            ItemTransfer = item,
                                            Qty = item.TransferredQuantity,

                                        };
                                        SelectedMainRow.DetailsList.Add(transferDetail);
                                        SelectedDetailRow = transferDetail;
                                    }
                                    else// لو موجود هحدث الكمية
                                    {

                                        temp.Qty = item.TransferredQuantity;
                                    }
                                }
                            };
                            var childWindowSeach = new ItemDimensionSearchChildWindow(vm);
                            childWindowSeach.Show();
                            childWindowSeach.IsTransfer = false;
                            childWindowSeach.QuantityTitle = strings.Qty;
                            vm.Title = strings.TransferItem;
                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouseFrom);
                    }
                    catch (Exception ex) { throw ex; }
                });


                PurchasePlanClient.GetTransferCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TransferHeader();
                        newrow.InjectFrom(row);
                        TransferList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                
                };
                PurchasePlanClient.GetTransferDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    { 
                        var newrow = new TblSalesOrderDetailRequestModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }                 
                    Loading = false;              
                };

                FromTransfer = new RelayCommand(() =>
                {
                    SearchTransfer();
                    //var child = new 

                });
                MainRowList = new ObservableCollection<TblSalesOrderHeaderRequestModel>();

                var calculationClient = new CRUDManagerService.CRUD_ManagerServiceClient();
                calculationClient.GetGenericCompleted += (s, sv) =>
                {
                    SalesPersonList = sv.Result;
                };
                calculationClient.GetGenericAsync("TblSalesPerson", "%%", "%%", "%%", "Iserial", "ASC");


                //PurchasePlanClient.SearchSalesOrderDetailCompleted += (s, sv) =>
                //{
                //    DetailsList.Clear();
                //    foreach (var row in sv.Result)
                //    {
                //        var newrow = new TblSalesOrderDetailViewModel { ColorPerRow = new CRUDManagerService.TblColor() };
                //        if (row.TblColor != null) newrow.ColorPerRow.InjectFrom(row.TblColor);

                //        newrow.InjectFrom(row);

                //        DetailsList.Add(newrow);
                //    }
                //};



                var journalAccountTypeClient = new GlService.GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var currencyClient = new GlService.GlServiceClient();

                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = new ObservableCollection<CRUDManagerService.GenericTable>();
                    foreach (var item in sv.Result)
                    {
                        CurrencyList.Add(new CRUDManagerService.GenericTable().InjectFrom(item) as CRUDManagerService.GenericTable);
                    }

                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                Client.GetTblWarehouseAsync(0, int.MaxValue, "it.Iserial", null, null);

                Client.GetTblWarehouseCompleted += (s, sv) =>
                 {
                     WareHouseList = sv.Result;

                 };

                Client.GetVendPayModeAsync("CCR");
                Client.GetVendPayModeCompleted += (s, sv) =>
                {
                    VendPayModeList = sv.Result;
                };
                Client.GetAxPaymentTermAsync("CCR");
                Client.GetAxPaymentTermCompleted += (s, sv) =>
                {
                    PaymTerm = sv.Result;
                };
                Client.GetAllWarehousesByCompanyNameAsync("CCm");
                PurchasePlanClient.GetTblSalesOrderHeaderRequestCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesOrderHeaderRequestModel();
                        newrow.InjectFrom(row);
                        newrow.EntityPerRow = new GlService.Entity();
                        newrow.EntityPerRow.InjectFrom(sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType));
                        newrow.WareHousePerRow = new CRUDManagerService.TblWarehouse();
                        newrow.WareHousePerRow.InjectFrom(row.TblWarehouse1);
                        newrow.SalesPersonPerRow = new CRUDManagerService.GenericTable();
                        newrow.SalesPersonPerRow.InjectFrom(row.TblSalesPerson1);
                        newrow.JournalAccountTypePerRow = new GlService.GenericTable();
                        newrow.JournalAccountTypePerRow.InjectFrom(JournalAccountTypeList.FirstOrDefault(w => w.Iserial == row.TblJournalAccountType));
                        newrow.CurrencyPerRow = new CRUDManagerService.GenericTable().InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == newrow.TblCurrency)) as CRUDManagerService.GenericTable;
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                };
                PurchasePlanClient.GetTblSalesIssueHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesIssueHeaderModel();
                        newrow.InjectFrom(row);
                        newrow.WareHousePerRow = row.TblWarehouse1;
                        SelectedMainRow.RecHeaderList.Add(newrow);
                    }

                    Loading = false;
                };
                PurchasePlanClient.GetTblSalesIssueDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesIssueDetailModel();
                        newrow.InjectFrom(row);
                        SelectedSubDetailRow.TblSalesIssueDetails.Add(newrow);
                    }

                    Loading = false;
                };
                PurchasePlanClient.GetTblSalesOrderDetailRequestCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesOrderDetailRequestModel
                        {
                            //       ColorPerRow = new CRUDManagerService.TblColor()
                        };
                        newrow.InjectFrom(row);
                        //  newrow.Received = sv.purchaseRec.Where(w => w.Key == row.Iserial).Sum(w => w.Value ?? 0);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                };
                PurchasePlanClient.DeleteTblSalesIssueHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.RecHeaderList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.RecHeaderList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblSalesIssueDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedSubDetailRow.TblSalesIssueDetails.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedSubDetailRow.TblSalesIssueDetails.Remove(oldrow);
                };
                PurchasePlanClient.UpdateOrInsertTblSalesOrderHeaderRequestCompleted += (s, x) =>
                {
                    if (SelectedMainRow != null) SelectedMainRow.InjectFrom(x.Result);
                    MessageBox.Show(strings.SavedMessage);
                    if (SelectedMainRow.DetailsList!=null)
                    {
                        SelectedMainRow.DetailsList.Clear();
                    }
                   
                    GetDetailData();
                };

                PurchasePlanClient.UpdateOrInsertTblSalesOrderDetailRequestCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                PurchasePlanClient.DeleteTblSalesOrderHeaderRequestCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                PurchasePlanClient.DeleteTblSalesOrderDetailRequestCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
               };            
            }
        }


        #region Properties

        WarehouseServiceClient PurchasePlanClient = new WarehouseService.WarehouseServiceClient();

        private CRUDManagerService.ItemsDto _itemPerRow;

        public CRUDManagerService.ItemsDto ItemPerRow
        {
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
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


        #endregion

        #region Commands

        RelayCommand openItemSearch;
        public RelayCommand OpenItemSearch
        {
            get { return openItemSearch; }
            set { openItemSearch = value; RaisePropertyChanged(nameof(OpenItemSearch)); }
        }

        RelayCommand _FromTransfer;
        public RelayCommand FromTransfer
        {
            get { return _FromTransfer; }
            set { _FromTransfer = value; RaisePropertyChanged(nameof(FromTransfer)); }
        }

                

        RelayCommand approveTransfer;
        public RelayCommand ApproveTransfer
        {
            get { return approveTransfer; }
            set { approveTransfer = value; RaisePropertyChanged(nameof(ApproveTransfer)); }
        }

        RelayCommand<object> deleteTransferDetail;
        public RelayCommand<object> DeleteTransferDetail
        {
            get { return deleteTransferDetail; }
            set { deleteTransferDetail = value; RaisePropertyChanged(nameof(DeleteTransferDetail)); }
        }

        #endregion

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            PurchasePlanClient.GetTblSalesOrderHeaderRequestAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
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
                        PurchasePlanClient.DeleteTblSalesOrderHeaderRequestAsync((TblSalesOrderHeaderRequest)new TblSalesOrderHeaderRequest().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblSalesOrderHeaderRequestModel());

            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
            {
                if (checkLastRow && SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(
                        SelectedDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, SelectedDetailRow = new TblSalesOrderDetailRequestModel
                {
                    TblSalesOrderHeaderRequest = SelectedMainRow.Iserial
                });
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
                    var saveRow = new TblSalesOrderHeaderRequest();
                    saveRow.InjectFrom(SelectedMainRow);
                    var detailListToSave = new ObservableCollection<TblSalesOrderDetailRequest>();
                    foreach (var item in SelectedMainRow.DetailsList)
                    {
                        var detailrow = new TblSalesOrderDetailRequest();
                        detailrow.InjectFrom(item);
                        saveRow.TblSalesOrderDetailRequests = new ObservableCollection<TblSalesOrderDetailRequest>();

                        detailListToSave.Add(detailrow);
                    }
                    saveRow.TblSalesOrderDetailRequests = detailListToSave;
                    saveRow.Code = "tt";

                    PurchasePlanClient.UpdateOrInsertTblSalesOrderHeaderRequestAsync(saveRow, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);


                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                PurchasePlanClient.GetTblSalesOrderDetailRequestAsync(SelectedMainRow.DetailsList.Count, int.MaxValue,
                    SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
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
                        if (row.Iserial != 0)
                        {
                            PurchasePlanClient.DeleteTblSalesOrderDetailRequestAsync((TblSalesOrderDetailRequest)new TblSalesOrderDetailRequest().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
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
                    var rowToSave = new TblSalesIssueDetail();
                    if (SelectedDetailRow != null)
                    {
                        SelectedRecDetailRow.TblSalesOrderDetailRequest = SelectedDetailRow.Iserial;
                    }

                    SelectedRecDetailRow.TblSalesIssueHeader = SelectedSubDetailRow.Iserial;
                    rowToSave.InjectFrom(SelectedRecDetailRow);
                    PurchasePlanClient.UpdateOrInsertTblSalesIssueDetailAsync(rowToSave, SelectedSubDetailRow.TblSalesIssueDetails.IndexOf(SelectedRecDetailRow), LoggedUserInfo.WFM_UserName);
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
                        PurchasePlanClient.DeleteTblSalesIssueDetailAsync((TblSalesIssueDetail)new TblSalesIssueDetail().InjectFrom(row), SelectedSubDetailRow.TblSalesIssueDetails.IndexOf(row));
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
                        PurchasePlanClient.DeleteTblSalesIssueHeaderAsync((TblSalesIssueHeader)new TblSalesIssueHeader().InjectFrom(row), SelectedMainRow.RecHeaderList.IndexOf(row));
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
                    var rowToSave = new WarehouseService.TblSalesIssueHeader();
                    SelectedSubDetailRow.TblSalesOrderHeaderRequest = SelectedMainRow.Iserial;


                    rowToSave.TblJournalAccountType = SelectedMainRow.TblJournalAccountType;
                    rowToSave.EntityAccount = SelectedMainRow.EntityAccount;


                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    rowToSave.TblWarehouse = SelectedMainRow.TblWarehouse;
                    PurchasePlanClient.UpdateOrInsertTblSalesIssueHeaderAsync(rowToSave, SelectedMainRow.RecHeaderList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.Iserial);
                }
            }
        }

        public void GetRecHeader()
        {
            if (SelectedMainRow != null)
            {
                PurchasePlanClient.GetTblSalesIssueHeaderAsync(SelectedMainRow.RecHeaderList.Count, PageSize,
                    SelectedMainRow.Iserial,0,0, DetailSortBy, DetailSubFilter, DetailSubValuesObjects);
            }
        }

        public void GetRecDetail()
        {
            if (SelectedSubDetailRow != null)
            {
                PurchasePlanClient.GetTblSalesIssueDetailAsync(SelectedSubDetailRow.TblSalesIssueDetails.Count, PageSize,
                    SelectedSubDetailRow.Iserial, DetailSortBy, RecDetailFilter, RecDetailValuesObjects);
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
                    var rowToSave = new TblSalesOrderDetailRequest();
                    //   SelectedDetailRow.TblSalesOrderHeaderRequest = SelectedMainRow.Iserial;
                    rowToSave.InjectFrom(SelectedDetailRow);
                    PurchasePlanClient.UpdateOrInsertTblSalesOrderDetailRequestAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        public void SearchHeader()
        {
            //MainRowList.Clear();
            //    var child = new SalesOrderRequestSearchChild(this);
            //child.Show();
        }

        public void DeleteOrder()
        {
            PurchasePlanClient.DeleteTblSalesOrderHeaderRequestAsync((TblSalesOrderHeaderRequest)new TblSalesOrderHeaderRequest().InjectFrom(SelectedMainRow), MainRowList.IndexOf(SelectedMainRow));
        }

        public void SeachPendingPurchase()
        {
            //   PurchasePlanClient.SearchSalesOrderDetailAsync(SelectedMainRow.Vendor, ItemPerRow.Code, FromDate, ToDate);

        }

        public void ReceivePurchase(bool receiveAll)
        {
            var newrow = new TblSalesIssueHeader();
            newrow.InjectFrom(SelectedSubDetailRow);
            newrow.TblJournalAccountType = SelectedMainRow.TblJournalAccountType;
            newrow.EntityAccount = SelectedMainRow.EntityAccount;


            newrow.TblSalesIssueDetails = new ObservableCollection<TblSalesIssueDetail>();
            if (receiveAll)
            {
                foreach (var item in SelectedMainRow.DetailsList.Where(w => w.Received <= w.Qty))
                {
                    item.ReceiveNow = item.Qty - item.Received;
                }
            }

            foreach (var item in SelectedMainRow.DetailsList.Where(x => x.ReceiveNow != 0))
            {
                var newdetailrow = new TblSalesIssueDetail
                {
                    Qty = item.ReceiveNow,
                    TblSalesOrderDetailRequest = item.Iserial,
                    Type = 0,
                    Price = item.Price,


                };
                newrow.TblSalesIssueDetails.Add(newdetailrow);
            }
            newrow.TblWarehouse = SelectedMainRow.TblWarehouse;

            PurchasePlanClient.UpdateOrInsertTblSalesIssueHeaderAsync(newrow, 0, LoggedUserInfo.Iserial);
        }

        public void AddNewMarkUpRow(bool checkLastRow, bool header)
        {
            if (header)
            {
                var currentRowIndex = (SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow));

                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                        new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }
                if (AllowAdd != true)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }
                var newrow = new TblMarkupTranProdViewModel
                {
                    Type = 0,
                    TblRecInv = SelectedMainRow.Iserial,
                    MiscValueType = 1
                };

             
                SelectedMainRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
                SelectedMarkupRow = newrow;
            }
            //else
            //{
            //    var currentRowIndex = (SelectedDetailRow.MarkUpTransList.IndexOf(SelectedMarkupRow));

            //    if (checkLastRow)
            //    {
            //        var valiationCollection = new List<ValidationResult>();

            //        var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
            //            new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

            //        if (!isvalid)
            //        {
            //            return;
            //        }
            //    }
            //    if (AllowAdd != true)
            //    {
            //        MessageBox.Show(strings.AllowAddMsg);
            //        return;
            //    }
            //    var newrow = new TblMarkupTranProdViewModel { Type = 0, TblRecInv = SelectedDetailRow.Iserial };
            //    SelectedDetailRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
            //    SelectedMarkupRow = newrow;
            //}
        }

        public void SaveMarkupRow()
        {
            if (SelectedMarkupRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                    new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMarkupRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblSalesOrderRequestInvoiceHeaderMarkupTransProd();
                    saveRow.InjectFrom(SelectedMarkupRow);
                    //saveRow.RouteCardInvoiceHeader = SelectedMainRow.Iserial;
                    
                    //saveRow.TblMarkup1 = null;

                    //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                    if (!Loading)
                    {
                        Loading = true;

                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                            PurchasePlanClient.UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProdsAsync(saveRow, save, SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
                      LoggedUserInfo.DatabasEname);
                     
                    }
                }
            }
        }

        public void SaveMarkupRowOldRow(TblMarkupTranProdViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblSalesOrderRequestInvoiceHeaderMarkupTransProd();
                    saveRow.InjectFrom(oldRow);
                    if (!Loading)
                    {
                        Loading = true;
                        if (oldRow.Type == 0)
                        {
                            PurchasePlanClient.UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProdsAsync(saveRow, save,
                                SelectedMainRow.MarkUpTransList.IndexOf(oldRow));
                        }
                        //else
                        //{
                        //    Glclient.UpdateOrInsertTblMarkupTransAsync(saveRow, save,
                        //        SelectedDetailRow.MarkUpTransList.IndexOf(oldRow),
                        //        LoggedUserInfo.DatabasEname);
                        //}
                    }
                }
            }
        }



        #region Prop     

        private ObservableCollection<TblMarkupProd> _markupList;

        public ObservableCollection<TblMarkupProd> MarkupList
        {
            get { return _markupList; }
            set { _markupList = value; RaisePropertyChanged("MarkupList"); }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _CurrencyList;

        public ObservableCollection<CRUDManagerService.GenericTable> CurrencyList
        {
            get { return _CurrencyList; }
            set { _CurrencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _SalesPersonList;

        public ObservableCollection<CRUDManagerService.GenericTable> SalesPersonList
        {
            get { return _SalesPersonList; }
            set { _SalesPersonList = value; RaisePropertyChanged("SalesPersonList"); }
        }

        private ObservableCollection<CRUDManagerService.TblWarehouse> _wareHouseList;

        public ObservableCollection<CRUDManagerService.TblWarehouse> WareHouseList
        {
            get { return _wareHouseList ?? (_wareHouseList = new ObservableCollection<CRUDManagerService.TblWarehouse>()); }
            set { _wareHouseList = value; RaisePropertyChanged("WareHouseList"); }
        }

        private ObservableCollection<CRUDManagerService.VENDPAYMMODETABLE> _vendPayMode;

        public ObservableCollection<CRUDManagerService.VENDPAYMMODETABLE> VendPayModeList
        {
            get { return _vendPayMode ?? (_vendPayMode = new ObservableCollection<CRUDManagerService.VENDPAYMMODETABLE>()); }
            set { _vendPayMode = value; RaisePropertyChanged("VendPayModeList"); }
        }

        private ObservableCollection<CRUDManagerService.PAYMTERM> _paymTerm;

        public ObservableCollection<CRUDManagerService.PAYMTERM> PaymTerm
        {
            get { return _paymTerm ?? (_paymTerm = new ObservableCollection<CRUDManagerService.PAYMTERM>()); }
            set { _paymTerm = value; RaisePropertyChanged("PaymTerm"); }
        }

        private ObservableCollection<CRUDManagerService.TblLkpSeason> _seasonList;

        public ObservableCollection<CRUDManagerService.TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<CRUDManagerService.TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private ObservableCollection<TblSalesOrderHeaderRequestModel> _mainRowList;

        public ObservableCollection<TblSalesOrderHeaderRequestModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSalesOrderHeaderRequestModel> _selectedMainRows;

        public ObservableCollection<TblSalesOrderHeaderRequestModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSalesOrderHeaderRequestModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblSalesOrderHeaderRequestModel _selectedMainRow;

        public TblSalesOrderHeaderRequestModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblSalesOrderHeaderRequestModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }
        private TransferHeader _SelectedTransferRow;

        public TransferHeader SelectedTransferRow
        {
            get { return _SelectedTransferRow; }
            set { _SelectedTransferRow = value;RaisePropertyChanged("SelectedTransferRow"); }
        }

        private TblSalesOrderDetailRequestModel _selectedDetailRow;

        public TblSalesOrderDetailRequestModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblSalesOrderDetailRequestModel> _selectedDetailRows;

        public ObservableCollection<TblSalesOrderDetailRequestModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSalesOrderDetailRequestModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        public string RecDetailFilter { get; set; }

        public Dictionary<string, object> RecDetailValuesObjects { get; set; }

        private TblSalesIssueHeaderModel _selectedSubDetailRow;

        public TblSalesIssueHeaderModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow ?? (_selectedSubDetailRow = new TblSalesIssueHeaderModel()); }
            set { _selectedSubDetailRow = value; RaisePropertyChanged("SelectedSubDetailRow"); }
        }

        private TblSalesIssueDetailModel _selectedRecDetailRow;

        public TblSalesIssueDetailModel SelectedRecDetailRow
        {
            get { return _selectedRecDetailRow ?? (_selectedRecDetailRow = new TblSalesIssueDetailModel()); }
            set { _selectedRecDetailRow = value; RaisePropertyChanged("SelectedRecDetailRow"); }
        }

        private ObservableCollection<TblSalesIssueHeaderModel> _selectedRecHeaders;

        public ObservableCollection<TblSalesIssueHeaderModel> SelectedRecHeaders
        {
            get { return _selectedRecHeaders ?? (_selectedRecHeaders = new ObservableCollection<TblSalesIssueHeaderModel>()); }
            set { _selectedRecHeaders = value; RaisePropertyChanged("SelectedRecHeaders"); }
        }

        private ObservableCollection<TblSalesIssueDetailModel> _selectedRecDetails;

        public ObservableCollection<TblSalesIssueDetailModel> SelectedRecDetails
        {
            get { return _selectedRecDetails ?? (_selectedRecDetails = new ObservableCollection<TblSalesIssueDetailModel>()); }
            set { _selectedRecDetails = value; RaisePropertyChanged("SelectedRecDetails"); }
        }

   
        private TblMarkupTranProdViewModel _selectedMarkup;

        public TblMarkupTranProdViewModel SelectedMarkupRow
        {
            get { return _selectedMarkup; }
            set
            {
                _selectedMarkup = value;
                RaisePropertyChanged("SelectedMarkupRow");
            }
        }

        private ObservableCollection<TransferHeader> _TransferList;
        public ObservableCollection<TransferHeader> TransferList
        {
            get { return _TransferList??(_TransferList= new ObservableCollection<TransferHeader>()); }
            set { _TransferList = value; RaisePropertyChanged(nameof(TransferList)); }
        }

        public string TransferFilter { get; private set; }
        public Dictionary<string, object> TransferValuesObjects { get; private set; }

        #endregion Prop    
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TblSalesOrderHeaderRequestModel> vm =
                new GenericSearchViewModel<TblSalesOrderHeaderRequestModel>() { Title = "Transfer Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                GetDetailData();
                // RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) =>
            {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) =>
            {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }

        public void GetTransfer()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            PurchasePlanClient.GetTransferAsync(TransferList.Count, PageSize, TransferFilter, SortBy , TransferValuesObjects, LoggedUserInfo.Iserial);
        }
        public void GetTransferDetails()
        {
            if (SelectedTransferRow != null)
                PurchasePlanClient.GetTransferDetailAsync(0, int.MaxValue, SelectedTransferRow.Iserial);
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header="Code",
                        PropertyPath=nameof(TblSalesOrderHeaderRequestModel.Code),
                    },
                     new SearchColumnModel()
                    {
                        Header=strings.WareHouse,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblSalesOrderHeaderRequestModel.WareHousePerRow),nameof(TblWarehouse.Code)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblSalesOrderHeaderRequest.TblWarehouse1),nameof(TblWarehouse.Code)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WareHouse,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblSalesOrderHeaderRequestModel.WareHousePerRow),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblSalesOrderHeaderRequest.TblWarehouse1),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.DeliveryDate,
                        PropertyPath=nameof(TblSalesOrderHeaderRequestModel.DeliveryDate),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.TransDate,
                        PropertyPath=nameof(TblSalesOrderHeaderRequestModel.TransDate),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ShippingDate,
                        PropertyPath=nameof(TblSalesOrderHeaderRequestModel.ShippingDate),
                    },
                };
        }

        public  void SearchTransfer()
        {
            TransferList.Clear();
            GetTransfer();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchTransferModel());
            GenericSearchViewModel<TransferHeader> vm =
                new GenericSearchViewModel<TransferHeader>() { Title = "Transfer Search" };
            vm.FilteredItemsList = TransferList;
            vm.ItemsList = TransferList;
            vm.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedTransferRow = vm.ResultItemsList[e.NewStartingIndex];
               // RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
                TransferFilter = vm.Filter;
                TransferValuesObjects = vm.ValuesObjects;
                GetTransfer();
            },
            (o) => {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchTransferModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header="Code From",
                        PropertyPath=nameof(TransferHeader.CodeFrom),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WarehouseFrom,
                        PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header="Code To",
                        PropertyPath=nameof(TransferHeader.CodeTo),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WarehouseTo,
                        PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Date,
                        PropertyPath=nameof(TransferHeader.DocDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Approved,
                        PropertyPath=nameof(TransferHeader.Approved),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ApproveDate,
                        PropertyPath=nameof(TransferHeader.ApproveDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                };
        }




    }
}