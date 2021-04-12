using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.Views;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.RouteCardService;

namespace CCWFM.ViewModel.RouteCardViewModelClasses
{
    public class RouteCardViewModel : CRUDManagerService.PropertiesViewModelBase
    {
        private int _TblWarehouse;

        public int TblWarehouse
        {
            get { return _TblWarehouse; }
            set { _TblWarehouse = value; RaisePropertyChanged("TblWarehouse"); }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; RaisePropertyChanged("Price"); }
        }

        private int _routeCardHeaderIserial;

        public int RouteCardHeaderIserial
        {
            get { return _routeCardHeaderIserial; }
            set { _routeCardHeaderIserial = value; RaisePropertyChanged("RouteCardHeaderIserial"); }
        }

        private CRUDManagerService.TblColor _colorPerRow;

        public CRUDManagerService.TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set { _colorPerRow = value; RaisePropertyChanged("ColorPerRow"); }
        }

        private CRUDManagerService.TblSalesOrder _salesOrderPerRow;

        public CRUDManagerService.TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set { _salesOrderPerRow = value; RaisePropertyChanged("SalesOrderPerRow"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _objectIndex;

        public string ObjectIndex
        {
            get { return _objectIndex; }
            set
            {
                _objectIndex = value;
                RaisePropertyChanged("ObjectIndex");
            }
        }

        private string _size;

        public string Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("Size"); }
        }

        private int _tblSalesOrder;

        public int TblSalesOrder
        {
            get { return _tblSalesOrder; }
            set { _tblSalesOrder = value; RaisePropertyChanged("TblSalesOrder"); }
        }

        private int _tblColor;

        public int TblColor
        {
            get { return _tblColor; }
            set { _tblColor = value; RaisePropertyChanged("TblColor"); }
        }

        private int _routeGroupId;

        // ReSharper disable once InconsistentNaming
        public int RouteGroupID
        {
            get { return _routeGroupId; }
            set { _routeGroupId = value; RaisePropertyChanged("RouteGroupID"); }
        }

        private int _direction;

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; RaisePropertyChanged("Direction"); }
        }

        private int _parentTransId;

        // ReSharper disable once InconsistentNaming
        public int Trans_TransactionHeader
        {
            get { return _parentTransId; }
            set { _parentTransId = value; RaisePropertyChanged("Trans_TransactionHeader"); }
        }

        private ObservableCollection<RouteCardService.TblWarehouse1> _SelectedWarehouseList;

        public ObservableCollection<RouteCardService.TblWarehouse1> SelectedWarehouseList
        {
            get { return _SelectedWarehouseList ?? (_SelectedWarehouseList = new ObservableCollection<RouteCardService.TblWarehouse1>()); }
            set { _SelectedWarehouseList = value; RaisePropertyChanged("SelectedWarehouseList"); }
        }

        private string _degree;

        [Required]
        public string Degree
        {
            get { return _degree; }
            set
            {
                _degree = value;
                SelectedWarehouseList.Clear();
                if (WareHouseDegreeList != null)
                {


                    if (_degree == "1st")
                    {

                        foreach (var item in WareHouseDegreeList.Where(w => w.AuthUserIserial == LoggedUserInfo.Iserial && w.PermissionType == 5))
                        {
                            SelectedWarehouseList.Add(item.TblWarehouse);
                        }
                        SelectedWarehouseList = SelectedWarehouseList;


                    }
                    else if (_degree == "2nd")
                    {
                        foreach (var item in WareHouseDegreeList.Where(w => w.AuthUserIserial == LoggedUserInfo.Iserial && w.PermissionType == 6))
                        {
                            SelectedWarehouseList.Add(item.TblWarehouse);
                        }
                        SelectedWarehouseList = SelectedWarehouseList;
                    }
                    else if (_degree == "3rd")
                    {
                        foreach (var item in WareHouseDegreeList.Where(w => w.AuthUserIserial == LoggedUserInfo.Iserial && w.PermissionType == 7))
                        {
                            SelectedWarehouseList.Add(item.TblWarehouse);
                        }
                        SelectedWarehouseList = SelectedWarehouseList;
                    }
                    if (SelectedWarehouseList.Any())
                    {
                        TblWarehouse = SelectedWarehouseList.FirstOrDefault().Iserial;
                    }
                }

                RaisePropertyChanged("Degree");
            }
        }

        public ObservableCollection<string> Degrees { get; set; }

        private ObservableCollection<RoutCardSizeInfo> _routCardSizes;

        public ObservableCollection<RoutCardSizeInfo> RoutCardSizes
        {
            get
            {
                if (_routCardSizes == null)
                {
                    _routCardSizes = new ObservableCollection<RoutCardSizeInfo>();
                    RoutCardSizes.CollectionChanged += RoutCardSizes_CollectionChanged;
                }
                return _routCardSizes;
            }
            set
            {
                _routCardSizes = value;
                RaisePropertyChanged("RoutCardSizes");
            }
        }

        private int _sizeQuantity;

        public int SizeQuantity
        {
            get { return _sizeQuantity; }
            set { _sizeQuantity = value; RaisePropertyChanged("SizeQuantity"); }
        }

        private int _rowTotal;

        public int RowTotal
        {
            get { return _rowTotal; }
            set
            {
                _rowTotal = value;
                RaisePropertyChanged("RowTotal");
            }
        }

        private int _blocks;

        public int Blocks
        {
            get { return _blocks; }
            set { _blocks = value; RaisePropertyChanged("Blocks"); }
        }

        private string _notes;
        private ObservableCollection<RouteCardService.TblAuthWarehouse1> _WareHouseDegreeList;

        public ObservableCollection<RouteCardService.TblAuthWarehouse1> WareHouseDegreeList
        {
            get { return _WareHouseDegreeList; }
            set { _WareHouseDegreeList = value; RaisePropertyChanged("WareHouseDegreeList"); }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }



        public RouteCardViewModel()
        {
            Degrees = new ObservableCollection<string> { "1st", "2nd", "3rd" };
        }

        public void RoutCardSizes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (RoutCardSizeInfo item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (RoutCardSizeInfo item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RowTotal = RoutCardSizes.Sum(x => x.SizeConsumption);
        }
    }

    public class RoutCardSizeInfo : CRUDManagerService.PropertiesViewModelBase
    {
        private string _sizeCode;

        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                _sizeCode = value;
                RaisePropertyChanged("SizeCode");
            }
        }

        private int _sizeConsumption;

        public int SizeConsumption
        {
            get { return _sizeConsumption; }
            set
            {
                _sizeConsumption = value;
                RaisePropertyChanged("SizeConsumption");
            }
        }

        private bool _isTextBoxEnabled;

        public bool IsTextBoxEnabled
        {
            get { return _isTextBoxEnabled; }
            set
            {
                _isTextBoxEnabled = value;
                RaisePropertyChanged("IsTextBoxEnabled");
            }
        }
    }

    public class RouteCardFabricViewModel : CRUDManagerService.PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public RouteCardFabricViewModel()
        {
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;

                if (ItemPerRow.IsSizeIncluded == true)
                {
                    var tblAccessoryAttributesDetails = sv.Result.CombinationList.FirstOrDefault();
                    if (tblAccessoryAttributesDetails != null)
                        ItemPerRow.SizesList.Add(tblAccessoryAttributesDetails.Size);
                }
                if (ItemPerRow.CombinationList != null)
                {
                    ItemPerRow.SizesList.Clear();
                    foreach (var variable in ItemPerRow.CombinationList.Where(x => FabricColorPerRow != null && x.Configuration == FabricColorPerRow.Code).Select(x => x.Size))
                    {
                        ItemPerRow.SizesList.Add(variable);
                    }
                }
            };
        }
        private string _ItemGroup;

        public string ItemGroup
        {
            get { return _ItemGroup; }
            set { _ItemGroup = value; RaisePropertyChanged("ItemGroup"); }
        }

        private int? _TblItemDim;

        public int? TblItemDim
        {
            get { return _TblItemDim; }
            set { _TblItemDim = value; RaisePropertyChanged("TblItemDim"); }
        }


        private ItemsDto _itemPerRow;

        public ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
                if (ItemPerRow != null)
                {
                    if (ItemPerRow.Unit != null) Unit = ItemPerRow.Unit;
                    if (ItemPerRow.Code != null) ItemId = ItemPerRow.Code;
                    if (ItemPerRow.ItemGroup != null) IsAcc = ItemPerRow.ItemGroup.Contains("Acc");

                    _client.AccWithConfigAndSizeAsync(ItemPerRow);
                }
            }
        }

        private bool _isAcc;

        public bool IsAcc
        {
            get { return _isAcc; }
            set
            {
                _isAcc = value; RaisePropertyChanged("IsAcc");
            }
        }

        private SortableCollectionView<CRUDManagerService.TblColor> _colorList;

        public SortableCollectionView<CRUDManagerService.TblColor> ColorList
        {
            get { return _colorList ?? (_colorList = new SortableCollectionView<CRUDManagerService.TblColor>()); }
            set { _colorList = value; RaisePropertyChanged("ColorList"); }
        }

        private CRUDManagerService.TblColor _fabricColorPerRow;

        public CRUDManagerService.TblColor FabricColorPerRow
        {
            get { return _fabricColorPerRow; }
            set
            {
                _fabricColorPerRow = value; RaisePropertyChanged("FabricColorPerRow");
                if (FabricColorPerRow != null) FabricColor = FabricColorPerRow.Iserial;
                if (_fabricColorPerRow != null)
                {
                    if (ItemPerRow != null)
                    {
                        //  TblBOMStyleColor tblBomStyleColor;
                        if (ItemPerRow.CombinationList != null)
                        {
                            ItemPerRow.SizesList = new ObservableCollection<string>();
                            var sizes =
                                ItemPerRow.CombinationList.Where(
                                    x => (_fabricColorPerRow.Code == x.Configuration)).Select(x => x.Size).ToList();
                            ItemPerRow.SizesList = new ObservableCollection<string>(sizes);

                            Size = Size;
                        }
                    }
                }
            }
        }

        private CRUDManagerService.TblColor _styleColorPerRow;

        public CRUDManagerService.TblColor StyleColorPerRow
        {
            get { return _styleColorPerRow; }
            set
            {
                _styleColorPerRow = value; RaisePropertyChanged("StyleColorPerRow");
            }
        }

        private CRUDManagerService.TblColor _newColorPerRow;

        public CRUDManagerService.TblColor NewColorPerRow
        {
            get { return _newColorPerRow; }
            set
            {
                _newColorPerRow = value; RaisePropertyChanged("NewColorPerRow");
            }
        }

        private CRUDManagerService.TblSalesOrder _salesOrderPerRow;

        public CRUDManagerService.TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set
            {
                _salesOrderPerRow = value; RaisePropertyChanged("SalesOrderPerRow");
                if (_salesOrderPerRow != null)
                {
                    TblSalesOrder = SalesOrderPerRow.Iserial;
                }
            }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        private int? _transId;

        // ReSharper disable once InconsistentNaming
        public int? TransID
        {
            get { return _transId; }
            set
            {
                _transId = value;
                RaisePropertyChanged("TransID");
            }
        }

        private DateTime? _docDate;

        public DateTime? DocDate
        {
            get { return _docDate; }
            set { _docDate = value; RaisePropertyChanged("DocDate"); }
        }

        private string _barcodeField;

        private string _batchField;

        private int _fabricColorField;

        private int _iserialField;

        private string _itemIdField;

        private string _locationField;

        private double? _qtyField;

        private int _routeCardHeaderIserialField;

        private int _salesOrderField;

        private string _siteField;

        private string _sizeField;

        private int _styleColorField;

        private string _unitField;

        private string _warehouseField;

        private double? _remainingQty;

        public double? RemainingQty
        {
            get { return _remainingQty; }
            set { _remainingQty = value; RaisePropertyChanged("RemainingQty"); }
        }

        private int? _oldIserial;

        public int? OldIserial
        {
            get
            {
                return _oldIserial;
            }
            set
            {
                if ((_oldIserial.Equals(value) != true))
                {
                    _oldIserial = value;
                    RaisePropertyChanged("OldIserial");
                }
            }
        }

        public string Barcode
        {
            get
            {
                return _barcodeField;
            }
            set
            {
                _barcodeField = value;
                RaisePropertyChanged("Barcode");
            }
        }

        public string Batch
        {
            get
            {
                return _batchField;
            }
            set
            {
                if ((ReferenceEquals(_batchField, value) != true))
                {
                    _batchField = value;
                    RaisePropertyChanged("Batch");
                }
            }
        }

        public int FabricColor
        {
            get
            {
                return _fabricColorField;
            }
            set
            {
                if ((Equals(_fabricColorField, value) != true))
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
                    if (ItemPerRow == null || ItemPerRow.Code != ItemId)
                    {
                        ItemPerRow = new ItemsDto
                        {
                            Code = ItemId,
                        };
                    }
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

        public int RouteCardHeaderIserial
        {
            get
            {
                return _routeCardHeaderIserialField;
            }
            set
            {
                if ((_routeCardHeaderIserialField.Equals(value) != true))
                {
                    _routeCardHeaderIserialField = value;
                    RaisePropertyChanged("RouteCardHeaderIserial");
                }
            }
        }

        public int TblSalesOrder
        {
            get
            {
                return _salesOrderField;
            }
            set
            {
                if ((_salesOrderField.Equals(value) != true))
                {
                    _salesOrderField = value;
                    RaisePropertyChanged("SalesOrder");
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

        public int StyleColor
        {
            get
            {
                return _styleColorField;
            }
            set
            {
                if ((_styleColorField.Equals(value) != true))
                {
                    _styleColorField = value;
                    RaisePropertyChanged("StyleColor");
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

        private double _materialUsage;

        public double MaterialUsage
        {
            get
            {
                return _materialUsage;
            }
            set
            {
                if ((_materialUsage.Equals(value) != true))
                {
                    _materialUsage = value;
                    RaisePropertyChanged("MaterialUsage");
                }
            }
        }

        private bool _isFree;

        public bool IsFree
        {
            get { return _isFree; }
            set { _isFree = value; RaisePropertyChanged("_isFree"); }
        }

        private double? _costPerUnit;

        public double? CostPerUnit
        {
            get { return _costPerUnit; }
            set { _costPerUnit = value; RaisePropertyChanged("CostPerUnit"); }
        }

        private int? _newFabricColor;

        public int? NewFabricColor
        {
            get { return _newFabricColor; }
            set { _newFabricColor = value; RaisePropertyChanged("NewFabricColor"); }
        }
    }

    public class RouteTypes : CRUDManagerService.PropertiesViewModelBase
    {
        private int _routeTypeInt;

        public int RouteTypeInt
        {
            get { return _routeTypeInt; }
            set
            {
                _routeTypeInt = value;
                RaisePropertyChanged("RouteTypeInt");
            }
        }

        private string _routeTypeEname;

        public string RouteTypeEname
        {
            get { return _routeTypeEname; }
            set
            {
                _routeTypeEname = value;
                RaisePropertyChanged("RouteTypeEname");
            }
        }

        private string _routeTypeAname;

        public string RouteTypeAname
        {
            get { return _routeTypeAname; }
            set
            {
                _routeTypeAname = value;
                RaisePropertyChanged("RouteTypeAname");
            }
        }
    }

    public class RouteCardHeaderViewModel : ViewModelBase
    {
        #region[ Data Members ]

        public RouteCardService.RouteCardServiceClient RouteCardService = new RouteCardServiceClient();

        private int _PackingTransID;

        public int PackingTransID
        {
            get { return _PackingTransID; }
            set { _PackingTransID = value; RaisePropertyChanged("PackingTransID"); }
        }


        private string _SupplierInv;

        public string SupplierInv
        {
            get { return _SupplierInv; }
            set { _SupplierInv = value; RaisePropertyChanged("SupplierInv"); }
        }

        private DateTime _postedDate;

        public DateTime PostedDate
        {
            get { return _postedDate; }
            set { _postedDate = value; RaisePropertyChanged("PostedDate"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChanged("Enabled");
            }
        }


        private RouteCardService.TblRouteDirection _routeDirection;

        public RouteCardService.TblRouteDirection RouteDirectionPerRow
        {
            get { return _routeDirection; }
            set { _routeDirection = value; RaisePropertyChanged("RouteDirectionPerRow"); }
        }

        public string TransactionGuid { get; set; }

        private PermissionItemName _formName;

        public PermissionItemName FormName
        {
            get { return _formName; }
            set { _formName = value; RaisePropertyChanged("FormName"); }
        }

        private PagedCollectionView _searchPagedCollection;

        public PagedCollectionView SearchPagedCollection
        {
            get
            {
                return _searchPagedCollection;
            }
            set
            {
                _searchPagedCollection = value;
                RaisePropertyChanged("SearchPagedCollection");
            }
        }

        private PagedCollectionView _TempsearchPagedCollection;

        public PagedCollectionView TempSearchPagedCollection
        {
            get
            {
                return _TempsearchPagedCollection;
            }
            set
            {
                _TempsearchPagedCollection = value;
                RaisePropertyChanged("TempSearchPagedCollection");
            }
        }

        private Visibility _loadAvaVis;

        public Visibility LoadAvaVis
        {
            get { return _loadAvaVis; }
            set { _loadAvaVis = value; RaisePropertyChanged("LoadAvaVis"); }
        }

        private int? _linkIserial;

        public int? LinkIserial
        {
            get { return _linkIserial; }
            set { _linkIserial = value; RaisePropertyChanged("LinkIserial"); }
        }

        private int _transType;

        public int TransactionType
        {
            get { return _transType; }
            set
            {
                _transType = value;

                if (RouteTypeInt == 3 || RouteTypeInt == 9 || RouteTypeInt == 6)
                {
                    DirectionEnabled = false;
                    Direction = 0;
                }
                else
                {
                    if (TransactionType == 2)
                    {
                        DirectionEnabled = false;
                        Direction = 0;
                    }
                    if (TransactionType == 1 || TransactionType == 5 || TransactionType == 11)
                    {
                        LoadAvaVis = Visibility.Collapsed;
                        if (TransactionType == 1)
                        {
                            DirectionEnabled = false;
                            Direction = 1;
                        }
                        else
                        {
                            DirectionEnabled = true;
                        }
                    }
                    else
                    {
                        LoadAvaVis = Visibility.Visible;
                    }
                }

                RaisePropertyChanged("TransactionType");
            }
        }

        public string DefaultFinishedFabricWarehouse { get; set; }

        public string DefaultAccWarehouse { get; set; }

        public string DefaultDyeingAccWarehouse { get; set; }

        private ObservableCollection<RouteTypes> _routeTypes;

        public ObservableCollection<RouteTypes> RouteTypesList
        {
            get { return _routeTypes; }
            set
            {
                _routeTypes = value;
                RaisePropertyChanged("RouteTypesList");
            }
        }

        private ObservableCollection<RouteCardService.Tbl_TransactionType> _transactionTypes;

        public ObservableCollection<RouteCardService.Tbl_TransactionType> TransactionTypes
        {
            get { return _transactionTypes; }
            set
            {
                _transactionTypes = value;
                RaisePropertyChanged("TransactionTypes");
            }
        }

        private ObservableCollection<RouteCardFabricViewModel> _routeCardFabricViewModelList;

        public ObservableCollection<RouteCardFabricViewModel> RouteCardFabricViewModelList
        {
            get { return _routeCardFabricViewModelList; }
            set
            {
                _routeCardFabricViewModelList = value;
                RaisePropertyChanged("RouteCardFabricViewModelList");
            }
        }

        private RouteCardFabricViewModel _routeFabricSelectedRow;

        public RouteCardFabricViewModel RouteFabricSelectedRow
        {
            get { return _routeFabricSelectedRow; }
            set
            {
                _routeFabricSelectedRow = value;
                RaisePropertyChanged("RouteFabricSelectedRow");
            }
        }

        private ObservableCollection<RouteCardFabricViewModel> _routeCardFabricRemViewModelList;

        public ObservableCollection<RouteCardFabricViewModel> RouteCardFabricRemViewModelList
        {
            get { return _routeCardFabricRemViewModelList; }
            set
            {
                _routeCardFabricRemViewModelList = value;
                RaisePropertyChanged("RouteCardFabricRemViewModelList");
            }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set
            {
                _iserial = value;
                if (Iserial == 0 || (MarkerIserial != null))
                {
                    Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
                RaisePropertyChanged("Iserial");
            }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        private int _lastTransaction;

        public int LastTransaction
        {
            get { return _lastTransaction; }
            set
            {
                _lastTransaction = value;
                RaisePropertyChanged("LastTransaction");
            }
        }

        private bool _directionEnabled;

        public bool DirectionEnabled
        {
            get { return _directionEnabled; }
            set
            {
                _directionEnabled = value;
                RaisePropertyChanged("DirectionEnabled");
            }
        }

        private int? _markerIserial;

        public int? MarkerIserial
        {
            get { return _markerIserial; }
            set
            {
                _markerIserial = value;
                RaisePropertyChanged("MarkerIserial");
            }
        }

        private int? _transId;

        // ReSharper disable once InconsistentNaming
        public int? TransID
        {
            get { return _transId; }
            set
            {
                _transId = value;
                RaisePropertyChanged("TransID");
            }
        }

        private string _vendorCode;

        public string VendorCode
        {
            get { return _vendorCode; }
            set
            {
                _vendorCode = value;
                RaisePropertyChanged("VendorCode");
                GetRoutes();
                if (RouteTypeInt == 3 || RouteTypeInt == 9 || RouteTypeInt == 6)
                {
                    DirectionEnabled = false;
                    Direction = 0;
                }
                else
                {
                    if (TransactionType == 2)
                    {
                        DirectionEnabled = false;
                        Direction = 0;
                    }
                    if (TransactionType == 1 || TransactionType == 5)
                    {
                        LoadAvaVis = Visibility.Collapsed;
                        if (TransactionType == 1)
                        {
                            DirectionEnabled = false;
                            Direction = 1;
                        }
                        else
                        {
                            DirectionEnabled = true;
                        }
                    }
                    else
                    {
                        LoadAvaVis = Visibility.Visible;
                    }
                }
            }
        }

        private CRUDManagerService.Vendor _vendor;

        public CRUDManagerService.Vendor VendorPerRow
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                if (value != null)
                {
                    VendorCode = value.vendor_code;
                    RaisePropertyChanged("VendorPerRow");
                }
            }
        }

        private int _processId;

        // ReSharper disable once InconsistentNaming
        public int ProcessID
        {
            get { return _processId; }
            set
            { _processId = value; RaisePropertyChanged("ProcessID"); }
        }

        private int _routId;

        // ReSharper disable once InconsistentNaming
        public int RoutID
        {
            get { return _routId; }
            set
            {
                _routId = value; RaisePropertyChanged("RoutID");

                if (RouteTypeInt == 3 || RouteTypeInt == 9 || RouteTypeInt == 6)
                {
                    DirectionEnabled = false;
                    Direction = 0;
                }
                else
                {
                    if (TransactionType == 2)
                    {
                        DirectionEnabled = false;
                        Direction = 0;
                    }
                    if (TransactionType == 1 || TransactionType == 5)
                    {
                        LoadAvaVis = Visibility.Collapsed;
                        if (TransactionType == 1)
                        {
                            DirectionEnabled = false;
                            Direction = 1;
                        }
                        else
                        {
                            DirectionEnabled = true;
                        }
                    }
                    else
                    {
                        LoadAvaVis = Visibility.Visible;
                    }
                }
            }
        }

        private DateTime? _delivaryDate;

        public DateTime? DelivaryDate
        {
            get { return _delivaryDate; }
            set { _delivaryDate = value; RaisePropertyChanged("DelivaryDate"); }
        }

        private int _direction;

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; RaisePropertyChanged("Direction"); }
        }

        private int _routGroupId;

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

        private RouteCardService.TblRouteGroup _routGroupItem;

        public RouteCardService.TblRouteGroup RoutGroupItem
        {
            get { return _routGroupItem; }
            set { _routGroupItem = value; RaisePropertyChanged("RoutGroupItem"); }
        }

        private RouteCardService.TblRoute _routeItem;

        public RouteCardService.TblRoute RoutItem
        {
            get { return _routeItem; }
            set { _routeItem = value; RaisePropertyChanged("RoutItem"); }
        }

        private bool? _isPosted;

        public bool? IsPosted
        {
            get { return _isPosted; }
            set
            {
                _isPosted = value;
                RaisePropertyChanged("IsPosted");
            }
        }

        private int _routeTypeInt;

        public int RouteTypeInt
        {
            get { return _routeTypeInt; }
            set
            {
                _routeTypeInt = value;

                if (RouteTypeInt == 3 || RouteTypeInt == 9 || RouteTypeInt == 6)
                {
                    DirectionEnabled = false;
                    Direction = 0;
                }
                else
                {
                    if (TransactionType == 2)
                    {
                        DirectionEnabled = false;
                        Direction = 0;
                    }
                    if (TransactionType == 1 || TransactionType == 5)
                    {
                        LoadAvaVis = Visibility.Collapsed;
                        if (TransactionType == 1)
                        {
                            DirectionEnabled = false;
                            Direction = 1;
                        }
                    }
                    else
                    {
                        LoadAvaVis = Visibility.Visible;
                    }
                }
                RaisePropertyChanged("RouteTypeInt");
            }
        }

        private DateTime? _docDate;

        public DateTime? DocDate
        {
            get { return _docDate ?? (_docDate = DateTime.Now); }
            set { _docDate = value; RaisePropertyChanged("DocDate"); }
        }

        private DateTime _lastUpdateDate;

        public DateTime LastUpdateDate
        {
            get { return _lastUpdateDate; }
            set { _lastUpdateDate = value; RaisePropertyChanged("LastUpdateDate"); }
        }

        private ObservableCollection<RouteCardViewModel> _cardDetails;

        public ObservableCollection<RouteCardViewModel> RouteCardDetails
        {
            get { return _cardDetails; }
            set { _cardDetails = value; RaisePropertyChanged("RouteCardDetails"); }
        }



        private ObservableCollection<RouteCardService.AuthWarehouseType> _authwarehouse;

        public ObservableCollection<RouteCardService.AuthWarehouseType> authwarehouse
        {
            get { return _authwarehouse; }
            set { _authwarehouse = value; RaisePropertyChanged("authwarehouse"); }
        }

        private ObservableCollection<RouteBomIssueSP_Result> _routeBomIssue;

        public ObservableCollection<RouteBomIssueSP_Result> RouteBomIssue
        {
            get { return _routeBomIssue; }
            set
            {
                _routeBomIssue = value;
                RaisePropertyChanged("RouteBomIssue");
            }
        }

        private ObservableCollection<CRUDManagerService.Vendor> _vendors;

        public ObservableCollection<CRUDManagerService.Vendor> Vendors
        {
            get { return _vendors; }
            set
            {
                _vendors = value;
                RaisePropertyChanged("Vendors");
            }
        }

        private int _grandTotal;

        public int GrandTotal
        {
            get { return _grandTotal; }
            set { _grandTotal = value; RaisePropertyChanged("GrandTotal"); }
        }

        private ObservableCollection<RouteCardService.TblRouteGroup> _routeGroups;

        public ObservableCollection<RouteCardService.TblRouteGroup> RouteGroups
        {
            get { return _routeGroups; }
            set { _routeGroups = value; RaisePropertyChanged("RouteGroups"); }
        }

        private ObservableCollection<RouteCardService.TblRoute> _routes;

        public ObservableCollection<RouteCardService.TblRoute> Routes
        {
            get { return _routes; }
            set { _routes = value; RaisePropertyChanged("Routes"); }
        }

        private ObservableCollection<V_Warehouse> _warehouseList;

        public ObservableCollection<V_Warehouse> WarehouseList
        {
            get { return _warehouseList; }
            set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
        }

        public bool BoolCuttingQty { get; set; }

        public List<RouteCardService.RouteCardDetail> RouteCardDetailOperationList { get; set; }

        private ObservableCollection<RouteCardService.TblAuthWarehouse1> _WareHouseDegreeList;

        public ObservableCollection<RouteCardService.TblAuthWarehouse1> WareHouseDegreeList
        {
            get { return _WareHouseDegreeList; }
            set { _WareHouseDegreeList = value; RaisePropertyChanged("WareHouseDegreeList"); }
        }



        #endregion

        #region [ Constructors ]

        public RouteCardHeaderViewModel()
        {
            TransactionGuid = Guid.NewGuid().ToString();
            LoadViewModel();
        }

        public void ClearViewModel()
        {
            TransID = null;
            VendorCode = null;
            VendorCode = null;
            MarkerIserial = null;
            VendorCode = null;
            RoutGroupID = -1;
            RoutID = -1;
            IsPosted = false;
            Iserial = 0;
            DelivaryDate = null;
            DocDate = DateTime.Now;
            RouteCardDetails.Clear();
            RouteCardFabricViewModelList.Clear();
        }

        public void AddNewRowRouteCardCardFabric()
        {
            RouteCardFabricViewModelList.Add(new RouteCardFabricViewModel());
        }

        public void DeleteRouteCardFabric(RouteCardFabricViewModel row)
        {
            RouteCardFabricViewModelList.Remove(row);
        }

        public void LoadViewModel()
        {
            Client = new CRUD_ManagerServiceClient();
            TransactionTypes = new ObservableCollection<RouteCardService.Tbl_TransactionType>();
            authwarehouse = new ObservableCollection<AuthWarehouseType>();
            authwarehouse.Add(AuthWarehouseType.FirstDegreeWarehouse);
            authwarehouse.Add(AuthWarehouseType.SecondDegreeWarehouse);
            authwarehouse.Add(AuthWarehouseType.ThridDegreeWarehouse);
            RouteCardService.GetLookUpWarehousePermissionTypeAsync(LoggedUserInfo.Iserial, authwarehouse);
            RouteCardService.GetLookUpWarehousePermissionTypeCompleted += (s, sv) =>
            {
                WareHouseDegreeList = sv.Result;
            };

            //RouteCardServiceClient.
            RouteCardService.GetAxItemPriceCompleted += (s, sv) =>
            {
                var routeCardFabricViewModel = RouteCardFabricViewModelList.LastOrDefault();
                if (routeCardFabricViewModel != null)
                    routeCardFabricViewModel.CostPerUnit = (double?)sv.Result;
            };
            Client.InspectionFullDimCompleted += (a, d) =>
            {
                if (d.Result != null)
                {
                    foreach (var variable in d.Result)
                    {
                        RouteCardMappers.MapToViewModel(variable, RouteFabricSelectedRow);
                    }
                }
            };
            RouteCardService.GetTransactionTypesCompleted += (s, d) =>
            {
                TransactionTypes.Clear();
                foreach (var variable in d.Result)
                {
                    if (CustomePermissions.Any(x => x.Code == variable.Ename))
                    {
                        TransactionTypes.Add(variable);
                    }
                }
            };

            FormName = PermissionItemName.RouteCardForm;
            GetItemPermissions(PermissionItemName.RouteCardForm.ToString());
            GetCustomePermissions(PermissionItemName.RouteCardForm.ToString());
            PremCompleted += (s, sv) =>
            {
                BoolCuttingQty = CustomePermissions.FirstOrDefault(w => w.Code == "CuttingQty") != null;
                RouteCardService.GetTransactionTypesAsync();
            };
            RouteTypesList = new ObservableCollection<RouteTypes>
            {
                new RouteTypes {RouteTypeInt = 5, RouteTypeEname = "RouteCard", RouteTypeAname = ""},
                new RouteTypes {RouteTypeInt = 3, RouteTypeEname = "Report As A Finished", RouteTypeAname = ""},
                //new RouteTypes {RouteTypeInt = 6, RouteTypeEname = "شحن للعميل", RouteTypeAname = ""},
                new RouteTypes {RouteTypeInt = 9, RouteTypeEname = "End", RouteTypeAname = ""}
            };

            RouteCardFabricViewModelList = new ObservableCollection<RouteCardFabricViewModel>();

            ChaingSetupMethod.GetSettings("Common", Client);

            Client.GetChainSetupCompleted += (a, s) =>
            {
                foreach (var item in s.Result)
                {
                    if (item.sGlobalSettingCode == "DefaultAccWarehouse")
                    {
                        DefaultAccWarehouse = item.sSetupValue;
                    }
                    else if (item.sGlobalSettingCode == "DefaultFinishedFabricWarehouse")
                    {
                        DefaultFinishedFabricWarehouse = item.sSetupValue;
                    }
                    else if (item.sGlobalSettingCode == "DefaultDyeingAccWarehouse")
                    {
                        DefaultDyeingAccWarehouse = item.sSetupValue;
                    }
                }
            };

            RouteBomIssue = new ObservableCollection<RouteBomIssueSP_Result>();
            WarehouseList = new ObservableCollection<V_Warehouse>();
            Client.GetAllWarehousesByCompanyNameAsync("ccm");
            Client.GetAllWarehousesByCompanyNameCompleted += (a, b) =>
                {
                    foreach (var item in b.Result)
                    {
                        WarehouseList.Add(item);
                    }
                };

            DocDate = DateTime.Now;
            Vendors = new ObservableCollection<CRUDManagerService.Vendor>();
            RouteGroups = new ObservableCollection<RouteCardService.TblRouteGroup>();
            Routes = new ObservableCollection<RouteCardService.TblRoute>();

            RouteCardService.GetRouteBomIssueCompleted += (a, b) =>
            {
                var temp = new ObservableCollection<RouteBomIssueSP_Result>();
                foreach (var degree in RouteCardDetails.Select(x => x.Degree).Distinct())
                {
                    foreach (var item in b.Result)
                    {
                        var totalQuery = RouteCardDetails.Where(
                           x => x.SalesOrderPerRow.SalesOrderCode == item.SalesOrderID
                                && x.TblColor == item.StyleColorCode && x.Degree == degree).ToList();
                        var total = 0;

                        foreach (var routeCardViewModel in totalQuery)
                        {
                            total = routeCardViewModel.RoutCardSizes.Where(x => x.SizeCode == item.StyleSize).Sum(x => x.SizeConsumption);
                        }
                        if (Direction == 0 && RoutGroupItem.SubFabricProcess == true)
                        {
                        }
                        else
                        {
                            if (total > 0)
                            {
                                item.Total = item.MaterialUsage * total;
                            }
                            else
                            {
                                item.Total = 0;
                            }
                        }

                        temp.Add(new RouteBomIssueSP_Result().InjectFrom(item) as RouteBomIssueSP_Result);
                    }
                }
                var qqqq = temp.GroupBy(x => new { x.SalesOrderIserial, x.StyleColorCode, x.ItemId, x.FabricColor, x.FabricSize }).Select(x => new RouteBomIssueSP_Result
                {
                    SalesOrderID = x.FirstOrDefault().SalesOrderID,
                    SalesOrderIserial = x.Key.SalesOrderIserial,
                    LineNumber = x.FirstOrDefault().LineNumber,
                    ItemType = x.FirstOrDefault().ItemType,
                    ItemId = x.Key.ItemId,
                    StyleColor = x.FirstOrDefault().StyleColor,
                    StyleColorCode = x.Key.StyleColorCode,
                    Operation = x.FirstOrDefault().Operation,
                    OperationIserial = x.FirstOrDefault().OperationIserial,
                    BOM_FabricType = x.FirstOrDefault().BOM_FabricType,
                    Brand_Ename = x.FirstOrDefault().Brand_Ename,
                    FabricColor = x.FirstOrDefault().FabricColor,
                    FabricColorIserial = x.FirstOrDefault().FabricColorIserial,
                    FabricSize = x.FirstOrDefault().FabricSize,
                    Season_Name = x.FirstOrDefault().Season_Name,
                    MaterialUsage = x.FirstOrDefault().MaterialUsage,
                    Style = x.FirstOrDefault().Style,
                    StyleCode = x.FirstOrDefault().StyleCode,
                    UnitID = x.FirstOrDefault().UnitID,
                    Total = x.Sum(w => w.Total),
                    Notes = x.FirstOrDefault().Notes,
                    Dyed = x.FirstOrDefault().Dyed,
                    DyedColor = x.FirstOrDefault().DyedColor,
                    DyedColorCode = x.FirstOrDefault().DyedColorCode,
                    CostPerUnit = x.FirstOrDefault().CostPerUnit,
                    StyleSize = x.FirstOrDefault().StyleSize,
                });
                foreach (var item in qqqq)
                {
                    string warehouse = item.ItemType == "Accessories" ? DefaultAccWarehouse : DefaultFinishedFabricWarehouse;

                    if ((item.ItemType == "Accessories" || item.ItemType == "Service") && item.Dyed == true && Direction == 0)
                    {
                        warehouse = DefaultDyeingAccWarehouse;
                    }
                    if ((item.ItemType == "Accessories" || item.ItemType == "Service") && item.Dyed == true && Direction == 1)
                    {
                        warehouse = DefaultAccWarehouse;
                    }
                    RouteCardFabricViewModelList.Add(RouteCardMappers.MapToViewModel(item, warehouse, WarehouseList));
                }
            };

            RouteCardService.GetRoutGroupsAsync(null, null);
            RouteCardService.GetRoutGroupsCompleted += (a, b) =>
            {
                try
                {
                    foreach (var item in b.Result)
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
            RouteCardDetails = new ObservableCollection<RouteCardViewModel>();

            RouteCardService.GetRouteCardDetailsCompleted += Client_GetRouteCardDetailsCompleted;
        }


        public void calcTotal()
        {
            if (RouteCardDetails.Any())
            {
                GrandTotal = RouteCardDetails.Sum(x => x.RowTotal);
            }

        }
        public void LoadRouteFabric()
        {
            if (RouteCardFabricViewModelList.Count != 0) return;
            var distinctRoute = RouteCardDetails.GroupBy(x => new { x.Degree, x.SalesOrderPerRow, x.ColorPerRow, x.WareHouseDegreeList })
                .Select(x => new RouteCardViewModel { WareHouseDegreeList = x.Key.WareHouseDegreeList, SalesOrderPerRow = x.Key.SalesOrderPerRow, ColorPerRow = x.Key.ColorPerRow, Degree = x.Key.Degree });
            if (distinctRoute != null)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var colors = new ObservableCollection<string>(distinctRoute.Select(x => x.ColorPerRow.Code));
                // ReSharper disable once PossibleMultipleEnumeration
                var degree = new ObservableCollection<string>(distinctRoute.Select(x => x.Degree));
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var variable in distinctRoute.Select(x => x.SalesOrderPerRow).Distinct())
                {
                    RouteCardService.GetRouteBomIssueAsync(variable.SalesOrderCode, variable.TblStyle1.StyleCode, colors, RoutGroupID, Direction, degree);
                }
            }
        }

        public void LoadAvaliableQty()
        {
            var childWindow = new RouteCardFabricRemChildWindow(this,1);
            childWindow.Show();
        }

        public void LoadRouteDetails()
        {
            RouteCardService.GetRouteCardDetailsAsync(null, null, Iserial, Direction, RoutGroupID);
        }

        public void GetInfoFromBarcode(RouteCardFabricViewModel routeFabricSelectedRow)
        {
            if (routeFabricSelectedRow.Barcode != null)
            {
                RouteFabricSelectedRow = routeFabricSelectedRow;
                Client.InspectionFullDimAsync(routeFabricSelectedRow.Barcode);
            }
        }

        public void GetFreeStyles()
        {
            if (CustomePermissions.FirstOrDefault(w => w.Code == "FreeIssue") != null)
            {
                var childWindow = new RouteFreeIssueChildWindow(this);
                childWindow.Show();
            }
            else
            {
                MessageBox.Show("You Don't Have Permission Required To Add Items");
            }
        }

        #endregion

        #region [ Service Events ]

        private void Client_GetRouteCardDetailsCompleted(object sender, GetRouteCardDetailsCompletedEventArgs e)
        {
            try
            {
                RouteCardDetails.Clear();
                var objectindexlist = e.Result.Select(x => x.ObjectIndex).Distinct().ToList();

                foreach (var variable in objectindexlist)
                {
                    var savedrow = e.Result.FirstOrDefault(x => x.ObjectIndex == variable);

                    var temp = new RouteCardViewModel();
                    temp.WareHouseDegreeList = WareHouseDegreeList;
                    temp.InjectFrom(savedrow);
                    if (savedrow != null)
                    {
                        var color = new CRUDManagerService.TblColor();
                        color.InjectFrom(savedrow.TblColor1);
                        var TblSalesOrder = new CRUDManagerService.TblSalesOrder();

                        TblSalesOrder.InjectFrom(savedrow.TblSalesOrder1);
                        TblSalesOrder.TblStyle1 = new CRUDManagerService.TblStyle();
                        TblSalesOrder.TblStyle1.InjectFrom(savedrow.TblSalesOrder1.TblStyle1);
                        temp.ColorPerRow = color;
                        temp.SalesOrderPerRow = TblSalesOrder;
                    }

                    // ReSharper disable once InconsistentNaming
                    foreach (var VARIABLE in e.sizes.OrderBy(x => x.Id).Distinct())
                    {
                        foreach (var item in e.Result.OrderBy(x => x.Iserial).Where(x => x.ObjectIndex == variable && x.Size == VARIABLE.SizeCode).ToList())
                        {
                            if (item.SizeQuantity != null)
                            {
                                var newSize = new RoutCardSizeInfo
                                {
                                    SizeCode = item.Size,
                                    SizeConsumption = (int)item.SizeQuantity,
                                    IsTextBoxEnabled = true
                                };

                                if (temp.RoutCardSizes.All(x => x.SizeCode != item.Size))
                                {
                                    temp.RoutCardSizes.Add(newSize);
                                }
                            }
                        }
                    }

                    temp.RowTotal = temp.RoutCardSizes.Sum(x => x.SizeConsumption);

                    if (RouteCardDetails.All(x => x.ObjectIndex != variable))
                    {
                        RouteCardDetails.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorWindow(ex);
                err.Show();
            }
            calcTotal();
        }

        private void GetRoutes()
        {
            if (VendorCode != null && RoutGroupID > -1)
            {
                RouteCardService.GetRoutesAsync(null, null, RoutGroupID, VendorCode);

                RouteCardService.GetRoutesCompleted += (s, ev) =>
                    {
                        Routes.Clear();
                        foreach (var item in ev.Result)
                        {
                            Routes.Add(item);
                        }
                        RoutItem = Routes.FirstOrDefault(x => x.Iserial == RoutID);
                    };
            }
        }

        #endregion

        #region [ CRUD ]

        public void SaveRouteCard(RouteCardService.RouteCardHeader cardHeader, ObservableCollection<RouteCardService.RouteCardDetail> cardDetails, string saveMode, int postOrno)
        {
            if (saveMode.ToLower() == "add")
            {
                try
                {
                    RouteCardService.AddRoutCardCompleted += (s, e)
                        =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }
                        ClearViewModel();
                        LastTransaction = e.Result.Iserial;
                    };
                    RouteCardService.AddRoutCardAsync(cardHeader, cardDetails, postOrno, TransactionGuid, LoggedUserInfo.Iserial);
                    MessageBox.Show("Route Card successfully Saved!", "Save", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
            else
            {
                try
                {
                    RouteCardService.UpdateRoutCardCompleted += (s, e)
                        =>
                        {
                            if (e.Error != null)
                            {
                                throw e.Error;
                            }

                            LastTransaction = e.Result.Iserial;
                            ClearViewModel();
                        };
                    RouteCardService.UpdateRoutCardAsync(cardHeader, cardDetails, postOrno, TransactionGuid, LoggedUserInfo.Iserial);
                    MessageBox.Show("Route Card successfully Saved!", "Save", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
        }

        #endregion
    }
}