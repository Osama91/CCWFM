using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels.Mappers;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblDyeingOrderHeaderViewModel : PropertiesViewModelBase
    {
        private bool _enabled;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if ((_enabled.Equals(value) != true))
                {
                    _enabled = value;
                    RaisePropertyChanged("Enabled");
                }
            }
        }

        private string _docPlanField;

        private int _dyeingProductionOrderField;

        private DateTime? _transactionDateField;

        private string _vendorField;

        public string DocPlan
        {
            get
            {
                return _docPlanField;
            }
            set
            {
                if ((ReferenceEquals(_docPlanField, value) != true))
                {
                    _docPlanField = value;
                    RaisePropertyChanged("DocPlan");
                }
            }
        }

        public int DyeingProductionOrder
        {
            get
            {
                return _dyeingProductionOrderField;
            }
            set
            {
                if ((_dyeingProductionOrderField.Equals(value) != true))
                {
                    _dyeingProductionOrderField = value;
                    RaisePropertyChanged("DyeingProductionOrder");
                }
            }
        }

        public DateTime? TransactionDate
        {
            get
            {
                return _transactionDateField;
            }
            set
            {
                if ((_transactionDateField.Equals(value) != true))
                {
                    _transactionDateField = value;
                    RaisePropertyChanged("TransactionDate");
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

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value; RaisePropertyChanged("VendorPerRow");
                if (value != null) Vendor = value.vendor_code;
            }
        }

        private TblDyeingOrdersMainDetailViewModel _tblDyeingOrdersMainDetails;

        public TblDyeingOrdersMainDetailViewModel TblDyeingOrdersMainDetails
        {
            get
            {
                return _tblDyeingOrdersMainDetails;
            }
            set
            {
                if ((ReferenceEquals(_tblDyeingOrdersMainDetails, value) != true))
                {
                    _tblDyeingOrdersMainDetails = value;
                    RaisePropertyChanged("TblDyeingOrdersMainDetails");
                }
            }
        }

        private TblDyeingOrderDetailsViewModel _tblDyeingOrderDetails;

        public TblDyeingOrderDetailsViewModel TblDyeingOrderDetails
        {
            get
            {
                return _tblDyeingOrderDetails;
            }
            set
            {
                if ((ReferenceEquals(_tblDyeingOrderDetails, value) != true))
                {
                    _tblDyeingOrderDetails = value;
                    RaisePropertyChanged("TblDyeingOrderDetails");
                }
            }
        }
    }

    public class TblDyeingOrdersMainDetailViewModel : PropertiesViewModelBase
    {
        public TblDyeingOrdersMainDetailViewModel()
        {
            TblDyeingOrderDetails = new ObservableCollection<TblDyeingOrderDetailsViewModel>();
            FabricInventSumWithBatchList = new ObservableCollection<FabricInventSumWithBatch>();
        }
        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private bool _saved;

        public bool Saved
        {
            get
            {
                return _saved;
            }
            set
            {
                if ((_saved.Equals(value) != true))
                {
                    _saved = value;
                    RaisePropertyChanged("Saved");
                }
            }
        }

        private string _vendorTransaction;

        public string VendorTransaction
        {
            get { return _vendorTransaction; }
            set { _vendorTransaction = value; RaisePropertyChanged("VendorTransaction"); }
        }

        private string _wareHouse;

        public string WareHouse
        {
            get
            {
                return _wareHouse;
            }
            set
            {
                if ((ReferenceEquals(_wareHouse, value) != true))
                {
                    _wareHouse = value;
                    RaisePropertyChanged("WareHouse");
                }
            }
        }

        private int _dyeingProductionOrderField;

        private DateTime? _transactionDateField;
        private int _transIdField;

        private int _transactionTypeField;

        public int DyeingProductionOrder
        {
            get
            {
                return _dyeingProductionOrderField;
            }
            set
            {
                if ((_dyeingProductionOrderField.Equals(value) != true))
                {
                    _dyeingProductionOrderField = value;
                    RaisePropertyChanged("DyeingProductionOrder");
                }
            }
        }

        public int TransId
        {
            get
            {
                return _transIdField;
            }
            set
            {
                if ((_transIdField.Equals(value) != true))
                {
                    _transIdField = value;
                    RaisePropertyChanged("TransId");
                }
            }
        }

        public DateTime? TransactionDate
        {
            get
            {
                return _transactionDateField;
            }
            set
            {
                if ((_transactionDateField.Equals(value) != true))
                {
                    _transactionDateField = value;
                    RaisePropertyChanged("TransactionDate");
                    if (_transactionDateField == DateTime.MinValue)
                    {
                        _transactionDateField = null;
                    }
                }
            }
        }

        public int TransactionType
        {
            get
            {
                return _transactionTypeField;
            }
            set
            {
                if ((_transactionTypeField.Equals(value) != true))
                {
                    _transactionTypeField = value;
                    RaisePropertyChanged("TransactionType");
                }
            }
        }

        private bool _gotTransid;

        public bool GotTransid
        {
            get
            {
                return _gotTransid;
            }
            set
            {
                if ((_gotTransid.Equals(value) != true))
                {
                    _gotTransid = value;
                    RaisePropertyChanged("GotTransid");
                }
            }
        }

        private ObservableCollection<TblDyeingOrderDetailsViewModel> _tblDyeingOrderDetails;

        public ObservableCollection<TblDyeingOrderDetailsViewModel> TblDyeingOrderDetails
        {
            get
            {
                return _tblDyeingOrderDetails;
            }
            set
            {
                if ((ReferenceEquals(_tblDyeingOrderDetails, value) != true))
                {
                    _tblDyeingOrderDetails = value;
                    RaisePropertyChanged("TblDyeingOrderDetails");
                }
            }
        }

        private ObservableCollection<FabricInventSumWithBatch> _fabricInventSumWithBatch;

        public ObservableCollection<FabricInventSumWithBatch> FabricInventSumWithBatchList
        {
            get
            {
                return _fabricInventSumWithBatch;
            }
            set
            {
                if ((ReferenceEquals(_fabricInventSumWithBatch, value) != true))
                {
                    _fabricInventSumWithBatch = value;
                    RaisePropertyChanged("FabricInventSumWithBatchList");
                }
            }
        }

        private bool _posted;

        public bool Posted
        {
            get { return _posted; }
            set { _posted = value; RaisePropertyChanged("Posted"); }
        }
    }

    public class TblDyeingOrderDetailsViewModel : PropertiesViewModelBase
    {
        private int _batchNoField;

        private double _calculatedTotalQtyField;

        private string _colorField;

        private string _dyedFabricField;

        private int _dyeingClassField;

        private int _dyeingProductionOrderField;

        private string _fabricCodeField;

        private int _iserialField;

        private int _transIdField;

        private int _transactionTypeField;

        private string _unitField;

        private double? _defaultPrice;

        public double? DefaultPrice
        {
            get { return _defaultPrice; }
            set { _defaultPrice = value; RaisePropertyChanged("DefaultPrice"); }
        }

        private DateTime? _estimatedDeliveryDateField;

        public DateTime? EstimatedDeliveryDate
        {
            get
            {
                return _estimatedDeliveryDateField;
            }
            set
            {
                if ((_estimatedDeliveryDateField.Equals(value) != true))
                {
                    _estimatedDeliveryDateField = value;
                    RaisePropertyChanged("EstimatedDeliveryDate");
                    if (_estimatedDeliveryDateField == DateTime.MinValue)
                    {
                        _estimatedDeliveryDateField = null;
                    }
                }
            }
        }

        public int BatchNo
        {
            get
            {
                return _batchNoField;
            }
            set
            {
                if ((_batchNoField.Equals(value) != true))
                {
                    _batchNoField = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }

        private string _oldColor;

        public string OldColor
        {
            get { return _oldColor; }
            set { _oldColor = value; RaisePropertyChanged("OldColor"); }
        }

        public double CalculatedTotalQty
        {
            get
            {
                return _calculatedTotalQtyField;
            }
            set
            {
                if ((_calculatedTotalQtyField.Equals(value) != true))
                {
                    _calculatedTotalQtyField = value;
                    RaisePropertyChanged("CalculatedTotalQty");
                }
            }
        }

        public string Color
        {
            get
            {
                return _colorField;
            }
            set
            {
                if ((ReferenceEquals(_colorField, value) != true))
                {
                    _colorField = value;
                    RaisePropertyChanged("Color");
                }
            }
        }

        public string DyedFabric
        {
            get
            {
                return _dyedFabricField;
            }
            set
            {
                if ((ReferenceEquals(_dyedFabricField, value) != true))
                {
                    _dyedFabricField = value;
                    RaisePropertyChanged("DyedFabric");
                }
            }
        }

        public int DyeingClass
        {
            get
            {
                return _dyeingClassField;
            }
            set
            {
                if ((_dyeingClassField.Equals(value) != true))
                {
                    _dyeingClassField = value;
                    RaisePropertyChanged("DyeingClass");
                }
            }
        }

        public int DyeingProductionOrder
        {
            get
            {
                return _dyeingProductionOrderField;
            }
            set
            {
                if ((_dyeingProductionOrderField.Equals(value) != true))
                {
                    _dyeingProductionOrderField = value;
                    RaisePropertyChanged("DyeingProductionOrder");
                }
            }
        }

        public string FabricCode
        {
            get
            {
                return _fabricCodeField;
            }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");
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

        public int TransId
        {
            get
            {
                return _transIdField;
            }
            set
            {
                if ((_transIdField.Equals(value) != true))
                {
                    _transIdField = value;
                    RaisePropertyChanged("TransId");
                }
            }
        }

        public int TransactionType
        {
            get
            {
                return _transactionTypeField;
            }
            set
            {
                if ((_transactionTypeField.Equals(value) != true))
                {
                    _transactionTypeField = value;
                    RaisePropertyChanged("TransactionType");
                }
            }
        }

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
    }

    public class Transactions : PropertiesViewModelBase
    {
        private int _transactionId;

        public int TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                if ((Equals(_transactionId, value) != true))
                {
                    _transactionId = value;
                    RaisePropertyChanged("TransactionId");
                }
            }
        }

        private string _transactionName;

        public string TransactionName
        {
            get
            {
                return _transactionName;
            }
            set
            {
                if ((ReferenceEquals(_transactionName, value) != true))
                {
                    _transactionName = value;
                    RaisePropertyChanged("TransactionName");
                }
            }
        }
    }

    #endregion	ViewModels

    public class DyeingOrderViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<Transactions> _transactionList;

        public ObservableCollection<Transactions> TransactionList
        {
            get
            {
                return _transactionList;
            }
            set
            {
                if ((ReferenceEquals(_transactionList, value) != true))
                {
                    _transactionList = value;
                    RaisePropertyChanged("TransactionList");
                }
            }
        }

        private TblDyeingOrderHeaderViewModel _dyeingOrderHeader;

        public TblDyeingOrderHeaderViewModel DyeingOrderHeader
        {
            get
            {
                return _dyeingOrderHeader;
            }
            set
            {
                if ((ReferenceEquals(_dyeingOrderHeader, value) != true))
                {
                    _dyeingOrderHeader = value;
                    RaisePropertyChanged("DyeingOrderHeader");
                }
            }
        }

        private ObservableCollection<TblDyeingOrderHeaderViewModel> _dyeingOrderHeaderList;

        public ObservableCollection<TblDyeingOrderHeaderViewModel> DyeingOrderHeaderList
        {
            get
            {
                return _dyeingOrderHeaderList;
            }
            set
            {
                if ((ReferenceEquals(_dyeingOrderHeaderList, value) != true))
                {
                    _dyeingOrderHeaderList = value;
                    RaisePropertyChanged("DyeingOrderHeaderList");
                }
            }
        }

        private ObservableCollection<TblDyeingOrdersMainDetailViewModel> _dyeingOrderMainDetail;

        public ObservableCollection<TblDyeingOrdersMainDetailViewModel> DyeingOrderMainDetailList
        {
            get
            {
                return _dyeingOrderMainDetail;
            }
            set
            {
                if ((ReferenceEquals(_dyeingOrderMainDetail, value) != true))
                {
                    _dyeingOrderMainDetail = value;
                    RaisePropertyChanged("DyeingOrderMainDetailList");
                }
            }
        }

        private ObservableCollection<DyeingAxService> _dyeingServiceSummaryList;

        public ObservableCollection<DyeingAxService> DyeingAxServiceSummaryList
        {
            get
            {
                return _dyeingServiceSummaryList;
            }
            set
            {
                if ((ReferenceEquals(_dyeingServiceSummaryList, value) != true))
                {
                    _dyeingServiceSummaryList = value;
                    RaisePropertyChanged("DyeingAxServiceSummaryList");
                }
            }
        }

        private TblDyeingOrdersMainDetailViewModel _selectedMainDetails;

        public TblDyeingOrdersMainDetailViewModel SelectedMainDetails
        {
            get
            {
                return _selectedMainDetails;
            }
            set
            {
                if ((ReferenceEquals(_selectedMainDetails, value) != true))
                {
                    _selectedMainDetails = value;
                    RaisePropertyChanged("SelectedMainDetails");
                }
            }
        }

        private TblDyeingOrderDetailsViewModel _selectedDetails;

        public TblDyeingOrderDetailsViewModel SelectedDetails
        {
            get
            {
                return _selectedDetails;
            }
            set
            {
                if ((ReferenceEquals(_selectedDetails, value) != true))
                {
                    _selectedDetails = value;
                    RaisePropertyChanged("SelectedDetails");
                }
            }
        }

        private TblDyeingOrderDetailsViewModel _recieveDyedFabricDetails;

        public TblDyeingOrderDetailsViewModel RecieveDyedFabricDetails
        {
            get
            {
                return _recieveDyedFabricDetails;
            }
            set
            {
                if ((ReferenceEquals(_recieveDyedFabricDetails, value) != true))
                {
                    _recieveDyedFabricDetails = value;
                    RaisePropertyChanged("RecieveDyedFabricDetails");
                }
            }
        }

        #endregion Properties

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();
        private ObservableCollection<DyeingSummaryServicesViewModel> _dyeingSummaryServices;

        public event EventHandler SubmitSearchAction;

        public DyeingOrderViewModel()
        {
            PageSize = 60;
            DyeingOrderHeaderList = new ObservableCollection<TblDyeingOrderHeaderViewModel>();
            DyeingOrderHeader = new TblDyeingOrderHeaderViewModel();
            SelectedMainDetails = new TblDyeingOrdersMainDetailViewModel();
            DyeingAxServiceSummaryList = new ObservableCollection<DyeingAxService>();
            _webService.GetAxSummaryServicesAsync();

            _webService.GetAxSummaryServicesCompleted += (d, s) =>
            {
                foreach (var item in s.Result)
                {
                    DyeingAxServiceSummaryList.Add(item);
                }
            };

            _webService.FabricInventSumWithBatchesCompleted += (s, x) =>
            {
                SelectedMainDetails.FabricInventSumWithBatchList = new ObservableCollection<FabricInventSumWithBatch>();

                SelectedMainDetails.FabricInventSumWithBatchList = x.Result;
            };
            _webService.GetSavedDyeingOrderServicesCompleted += (s, sv) =>
            {
                double price = 0;

                DyeingSummaryServices = new ObservableCollection<DyeingSummaryServicesViewModel>();
                foreach (var item in DyeingAxServiceSummaryList)
                {
                    if (item.ServiceCode == "سعر صباغة اللون")
                    {
                        price = SelectedDetails.DefaultPrice ?? 0;
                    }
                    else
                    {
                        price = item.DefaultPrice ?? 0;
                    }
                    DyeingSummaryServices.Add(new DyeingSummaryServicesViewModel
                    {
                        ServiceCode = item.ServiceCode,
                        ServicEname = item.ServiceName,
                        Qty = price
                    });
                }

                foreach (var row in sv.Result)
                {
                    var serviceRow = DyeingSummaryServices.SingleOrDefault(x => x.ServiceCode == row.ServiceCode);
                    if (serviceRow != null)
                    {
                        serviceRow.SummaryRowIserial = row.DyeingOrdersDetailsInt;
                        serviceRow.Notes = row.Notes;
                        serviceRow.ItemChecked = true;
                        if (row.Qty != null) serviceRow.Qty = (double)row.Qty;
                    }
                }
            };
            DyeingOrderMainDetailList = new ObservableCollection<TblDyeingOrdersMainDetailViewModel>();
            TransactionList = new ObservableCollection<Transactions>();
            DyeingOrderMainDetailList.CollectionChanged += DyeingOrderMainDetailList_CollectionChanged;

            if (CultureInfo.CurrentCulture.Name.ToLower().Contains("en"))
            {
                TransactionList.Add(new Transactions { TransactionId = 0, TransactionName = "Issue Fabric" });
                TransactionList.Add(new Transactions { TransactionId = 1, TransactionName = "Receive Dyed Fabric" });
                //TransactionList.Add(new Transactions { TransactionId = 2, TransactionName = "Receive Fabric" });
                //TransactionList.Add(new Transactions { TransactionId = 3, TransactionName = "Return Dyed Fabric" });
            }
            else
            {
                TransactionList.Add(new Transactions { TransactionId = 0, TransactionName = "صرف" });
                TransactionList.Add(new Transactions { TransactionId = 1, TransactionName = "إستلام قماش  مصبوغ" });
                //TransactionList.Add(new Transactions { TransactionId = 2, TransactionName = "إضافة" });
                //TransactionList.Add(new Transactions { TransactionId = 3, TransactionName = "إرجاع قماش  مصبوغ" });
            }

            _webService.DyeingOrderHeaderSearchListCompleted += (s, e) =>
                {
                    DyeingOrderHeaderList = new ObservableCollection<TblDyeingOrderHeaderViewModel>();
                    foreach (var item in e.Result)
                    {
                        DyeingOrderHeaderList.Add(DyeingMapper.VwMapToDyeingorderHeader(item));
                    }

                };

            _webService.GetDyeingOrdersMainDetailsCompleted += (s, e) =>
                {
                    DyeingOrderMainDetailList = new ObservableCollection<TblDyeingOrdersMainDetailViewModel>();
                    foreach (var item in e.Result)
                    {
                        
                        DyeingOrderMainDetailList.Add(DyeingMapper.VwMapToDyeingorderMainDetail(item, e.ColorPrices));
                    }
                    int maxTransId;
                    try
                    {
                        maxTransId = DyeingOrderMainDetailList.Where(x => x.TransactionType == SelectedMainDetails.TransactionType & x != SelectedMainDetails).Select(x => x.TransId).Max();
                        SelectedMainDetails.GotTransid = true;
                    }
                    catch (Exception)
                    {
                        maxTransId = 0;
                    }

                    if (SelectedMainDetails != null) SelectedMainDetails.TransId = maxTransId + 1;

                    DyeingOrderMainDetailList.CollectionChanged += DyeingOrderMainDetailList_CollectionChanged;
                };
        }

        public ObservableCollection<DyeingSummaryServicesViewModel> DyeingSummaryServices
        {
            get { return _dyeingSummaryServices; }
            set { _dyeingSummaryServices = value; RaisePropertyChanged("DyeingSummaryServices"); }
        }

        private void DyeingOrderMainDetailList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblDyeingOrdersMainDetailViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblDyeingOrdersMainDetailViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TransactionType")
            {
                try
                {
                    SelectedMainDetails.TransId =
                                   DyeingOrderMainDetailList.Where(w => w.TransactionType == SelectedMainDetails.TransactionType)
                                       .Max(w => w.TransId) + 1;
                }
                catch (Exception)
                {
                    SelectedMainDetails.TransId = 0;
                }
            }
        }

        public void PostMainDetail()
        {
            _webService.ProducationConnectionAsync(SelectedMainDetails.DyeingProductionOrder, SelectedMainDetails.TransId, SelectedMainDetails.TransactionType, LoggedUserInfo.Iserial);
        }

        public void ResetMode()
        {
            DyeingOrderHeader = new TblDyeingOrderHeaderViewModel();
            DyeingOrderMainDetailList.Clear();
        }

        public void GetDyeingOrderMainDetail()
        {
            _webService.GetDyeingOrdersMainDetailsAsync(DyeingOrderHeader.DyeingProductionOrder);
            SubmitSearchAction.Invoke(this, null);
        }

        public void SearchHeader()
        {
            if (SortBy == null)
                SortBy = "it.DyeingProductionOrder desc";
            _webService.DyeingOrderHeaderSearchListAsync(DyeingOrderHeaderList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void SaveOrder()
        {
            var header = (TblDyeingOrdersHeader)new TblDyeingOrdersHeader().InjectFrom(DyeingOrderHeader);
            var mainDetails = new ObservableCollection<TblDyeingOrdersMainDetail>();
            if (DyeingOrderMainDetailList != null)
            {
                foreach (var item in DyeingOrderMainDetailList)
                {
                    mainDetails.Add(DyeingMapper.DbMapToDyeingorderMainDetail(item));
                }
            }

            _webService.SaveDyeingOrderAsync(header, mainDetails);
        }

        public void DyeingOrderServices()
        {
            _webService.GetSavedDyeingOrderServicesAsync(SelectedDetails.Iserial);
            var childServices = new DyeingPLanServicesChildWindows(this, SelectedDetails.Iserial);
            childServices.Show();
        }

        public void GetFabricInventSumWithBatches()
        {
            if (SelectedMainDetails != null)
                _webService.FabricInventSumWithBatchesAsync(SelectedMainDetails.WareHouse, null, "ccm",null,null,null);
        }

        #region DeleteMethods

        public void DeleteOrder()
        {
            if (DyeingOrderHeader.DyeingProductionOrder != 0)
            {
                _webService.DeleteDyeingOrderAsync(DyeingOrderHeader.DyeingProductionOrder);
                DyeingOrderHeader = new TblDyeingOrderHeaderViewModel();
                DyeingOrderMainDetailList.Clear();
            }
        }

        public void DeleteMainDetailsOrder()
        {
            if (DyeingOrderHeader.DyeingProductionOrder != 0)
            {
                _webService.DeleteDyeingMainDetailsAsync(DyeingMapper.DbMapToDyeingorderMainDetail(SelectedMainDetails));
                DyeingOrderMainDetailList.Remove(SelectedMainDetails);
            }
        }

        public void DeleteDetailsOrder()
        {
            if (DyeingOrderHeader.DyeingProductionOrder != 0)
            {
                _webService.DeleteDyeingDetailsAsync(SelectedDetails.Iserial);
                SelectedMainDetails.TblDyeingOrderDetails.Remove(SelectedDetails);
            }
        }

        #endregion DeleteMethods

        public void SaveServices(int iserial)
        {
            foreach (var row in DyeingSummaryServices)
            {
                Client.SaveDyeingOrderDetailsServicesAsync(iserial, row.ServiceCode, row.ServicEname, row.Notes, row.ItemChecked, row.Qty);
            }
        }
    }
}