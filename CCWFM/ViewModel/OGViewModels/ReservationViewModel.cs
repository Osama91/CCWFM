using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels.Mappers;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblReservationHeaderViewModel : PropertiesViewModelBase
    {
        private Transactions _typePerRow;

        public Transactions TypePerRow
        {
            get
            {
                return _typePerRow;
            }
            set
            {
                if ((ReferenceEquals(_typePerRow, value) != true))
                {
                    _typePerRow = value;
                    RaisePropertyChanged("TypePerRow");
                }
            }
        }
        private string _axRouteCardFabricsJournalIdField;

        private DateTime _docDateField;

        private bool _isPostedField;

        private int _iserialField;

        private int _reservationTypeField;

        private string _transOrderField;

        private string _docno;

        public string DocNo
        {
            get { return _docno; }
            set { _docno = value; RaisePropertyChanged("DocNo"); }
        }

        private CRUD_ManagerServicePurchaseOrderDto _journalPerRow;

        public CRUD_ManagerServicePurchaseOrderDto JournalPerRow
        {
            get { return _journalPerRow; }
            set
            {
                _journalPerRow = value; RaisePropertyChanged("JournalPerRow");
                if (JournalPerRow != null) TransOrder = JournalPerRow.JournalId;
            }
        }

        public string TransOrder
        {
            get { return _transOrderField ?? (_transOrderField = ""); }
            set
            {
                if ((ReferenceEquals(_transOrderField, value) != true))
                {
                    _transOrderField = value;
                    RaisePropertyChanged("TransOrder");
                    TransactionMainDetails.Clear();
                }
            }
        }

        public string AxRouteCardFabricsJournalId
        {
            get
            {
                return _axRouteCardFabricsJournalIdField;
            }
            set
            {
                if ((ReferenceEquals(_axRouteCardFabricsJournalIdField, value) != true))
                {
                    _axRouteCardFabricsJournalIdField = value;
                    RaisePropertyChanged("AxRouteCardFabricsJournalId");
                }
            }
        }

        public DateTime DocDate
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

        public bool IsPosted
        {
            get
            {
                return _isPostedField;
            }
            set
            {
                if ((_isPostedField.Equals(value) != true))
                {
                    _isPostedField = value;
                    RaisePropertyChanged("IsPosted");
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

        public int ReservationType
        {
            get
            {
                return _reservationTypeField;
            }
            set
            {
                if ((_reservationTypeField.Equals(value) != true))
                {
                    _reservationTypeField = value;
                    RaisePropertyChanged("ReservationType");
                }
            }
        }

        private ObservableCollection<TblReservationMainDetailsViewModel> _transactionMainDetails;

        public ObservableCollection<TblReservationMainDetailsViewModel> TransactionMainDetails
        {
            get
            {
                return _transactionMainDetails ?? (_transactionMainDetails = new ObservableCollection<TblReservationMainDetailsViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_transactionMainDetails, value) != true))
                {
                    _transactionMainDetails = value;
                    RaisePropertyChanged("TransactionMainDetails");
                }
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private int _transactionTypeField;

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
                    ReservationType = value;
                    RaisePropertyChanged("TransactionType");
                }
            }
        }

    }

    public class TblReservationMainDetailsViewModel : PropertiesViewModelBase
    {
        public TblReservationMainDetailsViewModel()
        {
            _errorVisbility = Visibility.Collapsed;
            ReservationDetailsViewModelList = new ObservableCollection<TblReservationDetailsViewModel>();
            InspectionRollList = new ObservableCollection<Tbl_fabricInspectionDetail>();
        }

        private bool _inspectedField;

        public bool Inspected
        {
            get { return _inspectedField; }
            set
            {
                _inspectedField = value;
                RaisePropertyChanged("inspected");
            }
        }

        private string _batchnoField;

        private string _fabricField;

        private string _fabricColorField;

        private string _fabricUnitField;

        private int _iserialField;

        private string _locationField;

        private string _siteField;

        private int _tblReservationHeaderField;

        private string _warehouseField;

        private string _fabricname;

        public string FabricName
        {
            get { return _fabricname; }
            set { _fabricname = value; RaisePropertyChanged("FabricName"); }
        }

        public string Batchno
        {
            get
            {
                return _batchnoField;
            }
            set
            {
                if ((ReferenceEquals(_batchnoField, value) != true))
                {
                    _batchnoField = value;
                    RaisePropertyChanged("Batchno");
                }
            }
        }

        public string Fabric
        {
            get
            {
                return _fabricField;
            }
            set
            {
                if ((ReferenceEquals(_fabricField, value) != true))
                {
                    _fabricField = value;
                    RaisePropertyChanged("Fabric");
                }
            }
        }

        public string FabricColor
        {
            get
            {
                return _fabricColorField;
            }
            set
            {
                if ((ReferenceEquals(_fabricColorField, value) != true))
                {
                    _fabricColorField = value;
                    RaisePropertyChanged("FabricColor");
                }
            }
        }

        public string FabricUnit
        {
            get
            {
                return _fabricUnitField;
            }
            set
            {
                if ((ReferenceEquals(_fabricUnitField, value) != true))
                {
                    _fabricUnitField = value;
                    RaisePropertyChanged("FabricUnit");
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

        public string Location
        {
            get
            {
                return _locationField;
            }
            set
            {
                if ((ReferenceEquals(_locationField, value) != true))
                {
                    _locationField = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        public string Site
        {
            get
            {
                return _siteField;
            }
            set
            {
                if ((ReferenceEquals(_siteField, value) != true))
                {
                    _siteField = value;
                    RaisePropertyChanged("Site");
                }
            }
        }

        public int Tbl_ReservationHeader
        {
            get
            {
                return _tblReservationHeaderField;
            }
            set
            {
                if ((_tblReservationHeaderField.Equals(value) != true))
                {
                    _tblReservationHeaderField = value;
                    RaisePropertyChanged("Tbl_ReservationHeader");
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

        private decimal _lineNum;

        public decimal LineNum
        {
            get
            {
                return _lineNum;
            }
            set
            {
                if ((_lineNum.Equals(value) != true))
                {
                    _lineNum = value;
                    RaisePropertyChanged("LineNum");
                }
            }
        }

        private float _qty;

        public float Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                if (_remQty == 0)
                {
                    _remQty = _qty;
                    _remQtyTemp = _qty;
                }

                RaisePropertyChanged("Qty");
            }
        }

        private float _remQty;

        public float RemQty
        {
            get { return _remQty; }
            set
            {
                _remQty = value;
                RaisePropertyChanged("RemQty");
            }
        }

        private float _remQtyTemp;

        public float RemQtyTemp
        {
            get { return _remQtyTemp; }
            set
            {
                _remQtyTemp = value;
                RaisePropertyChanged("RemQtyTemp");
            }
        }
        private double _OnHandQty;

        public double OnHandQty
        {
            get { return _OnHandQty; }
            set { _OnHandQty = value; }
        }


        private Visibility _errorVisbility;

        public Visibility ErrorVisbility
        {
            get { return _errorVisbility; }
            set
            {
                if (_errorVisbility != value)
                {
                    _errorVisbility = value; RaisePropertyChanged("ErrorVisbility");
                }
            }
        }

        private ObservableCollection<TblReservationDetailsViewModel> _reservationDetailsViewModelList;

        public ObservableCollection<TblReservationDetailsViewModel> ReservationDetailsViewModelList
        {
            get { return _reservationDetailsViewModelList; }
            set { _reservationDetailsViewModelList = value; RaisePropertyChanged("ReservationDetailsViewModelList"); }
        }

        private ObservableCollection<Tbl_fabricInspectionDetail> _inspectionRollList;

        public ObservableCollection<Tbl_fabricInspectionDetail> InspectionRollList
        {
            get { return _inspectionRollList; }
            set { _inspectionRollList = value; RaisePropertyChanged("InspectionRollList"); }
        }
    }

    public class TblReservationDetailsViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblReservationRecViewModel> _reservationListRec;

        public ObservableCollection<TblReservationRecViewModel> ReservationListRec
        {
            get { return _reservationListRec; }
            set { _reservationListRec = value; RaisePropertyChanged("ReservationListRec"); }
        }

        public TblReservationDetailsViewModel()
        {
            ReservationListRec = new ObservableCollection<TblReservationRecViewModel>();
        }

        private float _finalQtyField;

        private float _intialQtyField;

        private int _iserialField;

        private string _salesOrderField;

        private string _salesOrderColorField;

        private int _tblReservationMainDetailsField;

        private string _axPicklingListJournal;

        private bool _markered;

        public bool Markered
        {
            get { return _markered; }
            set { _markered = value; RaisePropertyChanged("Markered"); }
        }

        public string AxPicklingListJournal
        {
            get { return _axPicklingListJournal; }
            set { _axPicklingListJournal = value; RaisePropertyChanged("AxPicklingListJournal"); }
        }

        public float FinalQty
        {
            get { return _finalQtyField; }
            set
            {
                if ((_finalQtyField.Equals(value) != true))
                {
                    _finalQtyField = value;
                    RaisePropertyChanged("FinalQty");
                }
            }
        }

        public float IntialQty
        {
            get { return _intialQtyField; }
            set
            {
                if ((_intialQtyField.Equals(value) != true))
                {
                    _intialQtyField = value;
                    RaisePropertyChanged("IntialQty");
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

        public string SalesOrder
        {
            get { return _salesOrderField; }
            set
            {
                if ((ReferenceEquals(_salesOrderField, value) != true))
                {
                    _salesOrderField = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }

        public string SalesOrderColor
        {
            get { return _salesOrderColorField; }
            set
            {
                if ((ReferenceEquals(_salesOrderColorField, value) != true))
                {
                    _salesOrderColorField = value;
                    RaisePropertyChanged("SalesOrderColor");
                }
            }
        }

        public int Tbl_ReservationMainDetails
        {
            get { return _tblReservationMainDetailsField; }
            set
            {
                if ((_tblReservationMainDetailsField.Equals(value) != true))
                {
                    _tblReservationMainDetailsField = value;
                    RaisePropertyChanged("Tbl_ReservationMainDetails");
                }
            }
        }
    }

    public class TblReservationRecViewModel : PropertiesViewModelBase
    {
        private string _batchNoField;

        private int _iserialField;

        private string _itemField;

        private string _itemColorField;

        private string _locationField;

        private float _qtyField;

        private int _rollNoField;

        private string _siteField;

        private int? _tblFabricInspectionDetailsField;

        private int _tblReservationDetailsField;

        private string _warehouseField;

        public string BatchNo
        {
            get
            {
                return _batchNoField;
            }
            set
            {
                if ((ReferenceEquals(_batchNoField, value) != true))
                {
                    _batchNoField = value;
                    RaisePropertyChanged("BatchNo");
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

        public string Item
        {
            get
            {
                return _itemField;
            }
            set
            {
                if ((ReferenceEquals(_itemField, value) != true))
                {
                    _itemField = value;
                    RaisePropertyChanged("Item");
                }
            }
        }

        public int RollNo
        {
            get
            {
                return _rollNoField;
            }
            set
            {
                if ((_rollNoField.Equals(value) != true))
                {
                    _rollNoField = value;
                    RaisePropertyChanged("RollNo");
                }
            }
        }

        public string ItemColor
        {
            get
            {
                return _itemColorField;
            }
            set
            {
                if ((ReferenceEquals(_itemColorField, value) != true))
                {
                    _itemColorField = value;
                    RaisePropertyChanged("ItemColor");
                }
            }
        }

        public string Location
        {
            get
            {
                return _locationField;
            }
            set
            {
                if ((ReferenceEquals(_locationField, value) != true))
                {
                    _locationField = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        public float Qty
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

        public string Site
        {
            get
            {
                return _siteField;
            }
            set
            {
                if ((ReferenceEquals(_siteField, value) != true))
                {
                    _siteField = value;
                    RaisePropertyChanged("Site");
                }
            }
        }

        public int? Tbl_FabricInspectionDetails
        {
            get
            {
                return _tblFabricInspectionDetailsField;
            }
            set
            {
                if ((_tblFabricInspectionDetailsField.Equals(value) != true))
                {
                    _tblFabricInspectionDetailsField = value;
                    RaisePropertyChanged("Tbl_FabricInspectionDetails");
                }
            }
        }

        public int Tbl_ReservationDetails
        {
            get
            {
                return _tblReservationDetailsField;
            }
            set
            {
                if ((_tblReservationDetailsField.Equals(value) != true))
                {
                    _tblReservationDetailsField = value;
                    RaisePropertyChanged("Tbl_ReservationDetails");
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
    }

    public class ReservationViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public EventHandler SubmitSearchAction;

        private ObservableCollection<Transactions> _type;

        public ObservableCollection<Transactions> Types
        {
            get
            {
                return _type;
            }
            set
            {
                if ((ReferenceEquals(_type, value) != true))
                {
                    _type = value;
                    RaisePropertyChanged("Types");
                }
            }
        }
        public string TransactionGuid { get; set; }

        private int _section;

        public int Section
        {
            get { return _section; }
            set
            {
                _section = value;
                RaisePropertyChanged("Section");
            }
        }
        private int _season;

        public int TblLkpSeason
        {
            get { return _season; }
            set
            {
                _season = value;
                RaisePropertyChanged("TblLkpSeason");
            }
        }
        private string _brand;


        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;

                if (Brand != null) lkpClient.GetTblBrandSectionLinkAsync(Brand, 0);
                RaisePropertyChanged("Brand");
            }
        }

        private TblLkpBrandSection _sectionPerRow;
        public TblLkpBrandSection SectionPerRow
        {
            get
            {
                return _sectionPerRow;
            }
            set
            {
                if ((ReferenceEquals(_sectionPerRow, value) != true))
                {
                    _sectionPerRow = value;
                    RaisePropertyChanged("SectionPerRow");
                }
            }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        //private TblStyle _stylePerRow;

        //public TblStyle StylePerRow
        //{
        //    get { return _stylePerRow; }
        //    set
        //    {
        //        _stylePerRow = value; RaisePropertyChanged("StylePerRow");

        //        if (_stylePerRow != null)
        //        {
        //            SearchSalesOrder();
        //        }
        //    }
        //}

        private ObservableCollection<OrderLineListViewModel> _orderLineList;

        public ObservableCollection<OrderLineListViewModel> OrderLineList
        {
            get
            {
                return _orderLineList ?? (_orderLineList = new ObservableCollection<OrderLineListViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_orderLineList, value) != true))
                {
                    _orderLineList = value;
                    RaisePropertyChanged("OrderLineList");
                }
            }
        }

        private string _fabricToSearch;

        public string FabricToSearch
        {
            get { return _fabricToSearch; }
            set { _fabricToSearch = value; RaisePropertyChanged("FabricToSearch"); }
        }

        private bool _fabricToSearchVis;

        public bool FabricToSearchVis
        {
            get { return _fabricToSearchVis; }
            set
            {
                _fabricToSearchVis = value;
                if (!value)
                {
                    FabricToSearch = null;
                }
                RaisePropertyChanged("FabricToSearchVis");
            }
        }

        internal void InitializeCompleted()
        {
            if (!DesignerProperties.IsInDesignTool)
            {

                Types = new ObservableCollection<Transactions>();
                if (LoggedUserInfo.CurrLang == 0)
                {
                    Types.Add(new Transactions { TransactionId = 0, TransactionName = "أمر الشراء" });
                    Types.Add(new Transactions { TransactionId = 1, TransactionName = "صباغة" });
                    Types.Add(new Transactions { TransactionId = 2, TransactionName = "Transfer" });
                }
                else
                {
                    Types.Add(new Transactions { TransactionId = 0, TransactionName = "Purshase Order" });
                    Types.Add(new Transactions { TransactionId = 1, TransactionName = "Dyeing" });
                    Types.Add(new Transactions { TransactionId = 2, TransactionName = "Transfer" });
                }
                Client = new CRUD_ManagerServiceClient();
                GetItemPermissions(PermissionItemName.Reservation.ToString());
                ObservableCollection<GetItemOnhand_Result> OnHandList = new ObservableCollection<GetItemOnhand_Result>();
                Client.GetReservationMainDetailByFabricCompleted += (s, f) =>
                {
                    if (f.OnHandList!=null)
                    {
                        OnHandList = f.OnHandList;
                    }
                    TransactionHeader = new TblReservationHeaderViewModel();
                    foreach (var variable in f.Result)
                    {                     
                        TransactionHeader.TransactionMainDetails.Add(ReservationMappers.MaptoViewModel(variable, f.mainFabricList, OnHandList));
                    }
                    Loading = false;
                };
                Client.GetAllSeasonsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SeasonList.All(x => x.Iserial != row.Iserial))
                        {
                            SeasonList.Add(new TblLkpSeason().InjectFrom(row) as TblLkpSeason);
                        }
                    }
                };
                Client.GetAllSeasonsAsync();
                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandsList.Clear();
                    foreach (var variable in sv.Result.OrderBy(x => x.Brand_Code))
                    {
                        BrandsList.Add(variable);
                    }
                };
                Client.GetAllBrandsAsync(0);
                TransactionGuid = Guid.NewGuid().ToString();
                TransactionHeader = new TblReservationHeaderViewModel { DocDate = DateTime.Now.Date };
                SelectedMainDetails = new TblReservationMainDetailsViewModel();

                Client.GetReservationHeaderListCompleted += (d, i) =>
                {
                    foreach (var item in i.Result)
                    {
                        TransactionHeaderList.Add(ReservationMappers.MaptoViewModel(item));
                    }
                };

                lkpClient.GetTblBrandSectionLinkCompleted += (x, sv) =>
                {
                    BrandSectionList.Clear();
                    foreach (var row in sv.Result)
                    {
                        BrandSectionList.Add(row.TblLkpBrandSection1);
                    }
                };
                SalesorderList = new ObservableCollection<CRUD_ManagerServiceSalesOrderDto>();

                SalesorderList.CollectionChanged += SalesorderList_CollectionChanged;

                Client.GetPurchaseOrderSalesOrdersCompleted += (d, s) =>
                {
                    foreach (var variable in s.Result)
                    {
                        if (!SalesorderList.Any(
                            x =>
                                x.SalesOrder.Contains(variable.SalesOrder) &&
                                x.SalesOrderColor.Contains(variable.SalesOrderColor)) && !SelectedMainDetails.ReservationDetailsViewModelList.Any(x => x.SalesOrder.Contains(variable.SalesOrder) &&
                                x.SalesOrderColor.Contains(variable.SalesOrderColor)))
                        {
                            SalesorderList.Add(variable);
                        }
                    }
                };

                Client.GetPurchaseOrderLinesCompleted += (s, f) =>
                {
                    foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                    {
                        OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                    }
                    Loading = false;
                };


                Client.GetRecivedDyedOrdersCompleted += (s, f) =>
                {
                    foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                    {
                        OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                    }
                };
                Client.GetTransferInventDimLinesCompleted += (s, f) =>
                {
                    foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                    {
                        OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                    }
                };
                Client.GetReservationMainDetailsCompleted += (s, f) =>
                {
                    foreach (var variable in f.Result)
                    {
                        if (variable.Inspected)
                        {
                            Client.GetResInspectionListAsync(variable.LineNum, TransactionHeader.TransOrder);
                        }
                        //f.mainFabricList;
                        TransactionHeader.TransactionMainDetails.Add(ReservationMappers.MaptoViewModel(variable, f.mainFabricList));
                    }
                };

                Client.SaveReservationCompleted += (a, f) =>
                {
                    if (f.ErrorExists)
                    {
                        var visList = TransactionHeader.TransactionMainDetails.Where(x => f.Result.All(s => s.Fabric == x.Fabric && s.FabricColor == x.FabricColor));

                        foreach (var item in TransactionHeader.TransactionMainDetails)
                        {
                            item.ErrorVisbility = visList.Contains(item) ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        var tblFabricInspectionDetail = f.Result.FirstOrDefault();
                        if (tblFabricInspectionDetail != null)
                            TransactionHeader.Iserial = tblFabricInspectionDetail.Tbl_ReservationHeader;
                        TransactionHeader.TransactionMainDetails.Clear();
                        foreach (var item in f.Result)
                        {
                            TransactionHeader.TransactionMainDetails.Add((TblReservationMainDetailsViewModel)new TblReservationMainDetailsViewModel().InjectFrom(item));
                        }
                        var currentUi = Thread.CurrentThread.CurrentUICulture;
                        if (currentUi.DisplayName == "العربية")
                        {
                            MessageBox.Show("Saved");
                        }
                        else
                        {
                            MessageBox.Show("Saved");
                        }
                    }
                };


                Client.GenerateReservationFromPlanCompleted += (s, sv) => Client.GetReservationMainDetailsAsync(TransactionHeader.Iserial);
                Client.GetSalesOrderReservationCompleted += (s, sv) =>
                {
                    GenericMapper.InjectFromObCollection(SelectedMainDetails.ReservationDetailsViewModelList, sv.Result);
                    foreach (var item in (SelectedMainDetails.ReservationDetailsViewModelList))
                    {
                        item.Tbl_ReservationMainDetails = SelectedMainDetails.Iserial;

                    }

                };
                Client.ReservationLineNumCompleted += (s, p) =>
                {
                    var lines = new ObservableCollection<decimal>(p.Result);

                    if (TransactionHeader.TransactionMainDetails.Count() != 0)
                    {
                        foreach (var variable in TransactionHeader.TransactionMainDetails.Select(x => x.LineNum))
                        {
                            lines.Add(variable);
                        }
                    }
                    if (SortBy == null)
                    {
                        SortBy = "it.PURCHQTY";
                    }
                    if (TransactionHeader.TransactionType == 0)
                    {
                        Client.GetPurchaseOrderLinesAsync(OrderLineList.Count, PageSize, "ccm", TransactionHeader.TransOrder, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
                    }
                    if (TransactionHeader.TransactionType == 1)
                    {
                        Client.GetRecivedDyedOrdersAsync(OrderLineList.Count, PageSize, TransactionHeader.TransOrder, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
                    }
                    else if (TransactionHeader.TransactionType == 2)
                    {
                        Client.GetTransferInventDimLinesAsync(OrderLineList.Count, PageSize, "ccm", TransactionHeader.TransOrder, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
                    }
                };
            }

        }
        public ReservationViewModel()
        {
            InitializeCompleted();
        }

        public ReservationViewModel(StyleHeaderViewModel styleViewModel)
        {
            this.styleViewModel = styleViewModel;

            InitializeCompleted();
            GetOnhand = true;
            GetReservationMainDetail(styleViewModel);
        }

        private void SalesorderList_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CRUD_ManagerServiceSalesOrderDto item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                    item_PropertyChanged(item, null);
                }

            if (e.OldItems != null)
                foreach (CRUD_ManagerServiceSalesOrderDto item in e.OldItems)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                    item_PropertyChanged(item, null);
                }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SelectedMainDetails.RemQtyTemp = SelectedMainDetails.RemQty - SalesorderList.Sum(x => x.IntialQty);
            //SelectedMainDetails.RemQtyTemp =  SalesorderList.Sum(x => x.IntialQty);
            if (e != null && e.PropertyName != null) RaisePropertyChanged(e.PropertyName);
        }

        private ObservableCollection<TblLkpSeason> _seasonNews;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get
            {
                return _seasonNews ?? (_seasonNews = new ObservableCollection<TblLkpSeason>());
            }
            set
            {
                if ((ReferenceEquals(_seasonNews, value) != true))
                {
                    _seasonNews = value;
                    RaisePropertyChanged("SeasonList");
                }
            }
        }

        private ObservableCollection<Brand> _brandsList;

        public ObservableCollection<Brand> BrandsList
        {
            get
            {
                return _brandsList ?? (_brandsList = new ObservableCollection<Brand>());
            }
            set
            {
                if ((ReferenceEquals(_brandsList, value) != true))
                {
                    _brandsList = value;
                    RaisePropertyChanged("BrandsList");
                }
            }
        }

        private ObservableCollection<CRUD_ManagerServiceSalesOrderDto> _salesorderList;

        public ObservableCollection<CRUD_ManagerServiceSalesOrderDto> SalesorderList
        {
            get
            {
                return _salesorderList ?? (_salesorderList = new ObservableCollection<CRUD_ManagerServiceSalesOrderDto>());
            }
            set
            {
                if ((ReferenceEquals(_salesorderList, value) != true))
                {
                    _salesorderList = value;
                    RaisePropertyChanged("SalesorderList");
                }
            }
        }

        private TblReservationHeaderViewModel _transactionHeader;

        public TblReservationHeaderViewModel TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                if ((ReferenceEquals(_transactionHeader, value) != true))
                {
                    _transactionHeader = value;
                    RaisePropertyChanged("TransactionHeader");
                }
            }
        }

        private ObservableCollection<TblReservationHeaderViewModel> _transactionHeaderList;

        public ObservableCollection<TblReservationHeaderViewModel> TransactionHeaderList
        {
            get
            {
                return _transactionHeaderList ?? (_transactionHeaderList = new ObservableCollection<TblReservationHeaderViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_transactionHeaderList, value) != true))
                {
                    _transactionHeaderList = value;
                    RaisePropertyChanged("TransactionHeaderList");
                }
            }
        }

        private TblReservationMainDetailsViewModel _selectedMainDetails;

        public TblReservationMainDetailsViewModel SelectedMainDetails
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

        private StyleHeaderViewModel _styleviewModel;

        public StyleHeaderViewModel styleViewModel
        {
            get { return _styleviewModel; }
            set { _styleviewModel = value; RaisePropertyChanged("styleViewModel"); }
        }


        private TblReservationDetailsViewModel _selectedDetails;

        public TblReservationDetailsViewModel SelectedDetails
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

        private float _QtyReq;

        public float QtyReq
        {
            get { return _QtyReq; }
            set { _QtyReq = value; RaisePropertyChanged("QtyReq"); }
        }



        internal void SearchHeader()
        {
            Client.GetReservationHeaderListAsync(TransactionHeaderList.Count, PageSize, "it.Iserial", DetailFilter, DetailValuesObjects);
        }

        internal void GetOrderInfo()
        {
            if (!string.IsNullOrWhiteSpace(TransactionHeader.TransOrder))
            {
                Client.ReservationLineNumAsync(TransactionHeader.TransOrder);
            }
        }

        internal void SaveOrder()
        {
            var mainDetailsListToSave = new ObservableCollection<Tbl_ReservationMainDetails>();

            foreach (var row in TransactionHeader.TransactionMainDetails)
            {
                mainDetailsListToSave.Add(ReservationMappers.MaptoViewModel(row));
            }
            Client.SaveReservationAsync(ReservationMappers.MaptoViewModel(TransactionHeader), mainDetailsListToSave, TransactionGuid, LoggedUserInfo.Iserial);
        }

        internal void saveReservationForStyle()
        {
            if (SelectedMainDetails.Qty - SelectedMainDetails.ReservationDetailsViewModelList.Sum(w => w.IntialQty) >= 0)
            {
                ObservableCollection<Tbl_ReservationDetails> newlist = new ObservableCollection<Tbl_ReservationDetails>();

                GenericMapper.InjectFromObCollection(newlist, SelectedMainDetails.ReservationDetailsViewModelList);
                Client.SaveReservationDetailsAsync(newlist, LoggedUserInfo.Iserial);

            }
            else
            {
                MessageBox.Show("Qty Exceed Limit " + SelectedMainDetails.Qty + "");
            }

        }

        internal void GetReservationMainDetail()
        {
            SubmitSearchAction.Invoke(null, null);
            Client.GetReservationMainDetailsAsync(TransactionHeader.Iserial);
        }

        bool GetOnhand;
        internal void GetReservationMainDetail(StyleHeaderViewModel StyleHeaderViewModel)
        {
            if (ValuesObjects == null)
            {
                ValuesObjects = new System.Collections.Generic.Dictionary<string, object>();
            }
            if (StyleHeaderViewModel.SelectedBomRow != null)
            {
                if (StyleHeaderViewModel.SelectedBomRow.ItemPerRow != null)
                {
                    ObservableCollection<string> temp = new ObservableCollection<string>();
                    foreach (var item in this.styleViewModel.SelectedBomRow.BomStyleColors.Where(w=>w.TblColor!=null).Select(w => w.TblColor.Code).Distinct())
                    {
                        temp.Add(item);
                    }
                    Loading = true;
                    Client.GetReservationMainDetailByFabricAsync(Filter,ValuesObjects, GetOnhand, StyleHeaderViewModel.SelectedDetailRow.Iserial, -1, StyleHeaderViewModel.SelectedBomRow.ItemPerRow.Code, temp);
                    GetOnhand = false;
                }
            }
        }
        internal void SearchSalesOrder()
        {
            var row = ReservationMappers.MaptoViewModel(SelectedMainDetails, true);
            row.Brand = Brand;
            row.TblBrandSection = Section;
            row.TblSeason = TblLkpSeason;

            Client.GetPurchaseOrderSalesOrdersAsync(row);
        }

        internal bool CheckFabricLineQty()
        {
            return !(SelectedMainDetails.RemQtyTemp < 0);
        }

        internal void DeleteOrder()
        {

            Client.DeleteReservationOrderAsync(ReservationMappers.MaptoViewModel(TransactionHeader), LoggedUserInfo.Iserial);
        }

        internal void ReservationRec()
        {
            var childWindows = new ReservationRecChildWindow(this);
            childWindows.Show();
        }

        internal void LinkToPlan()
        {
            Client.GenerateReservationFromPlanAsync(ReservationMappers.MaptoViewModel(TransactionHeader));
        }

        internal void getQtyRequired()
        {
            Client.GetSalesOrderReservationAsync(styleViewModel.SelectedBomRow.ItemPerRow.Code, SelectedMainDetails.FabricColor, styleViewModel.SelectedDetailRow.Iserial);
        }
    }
}