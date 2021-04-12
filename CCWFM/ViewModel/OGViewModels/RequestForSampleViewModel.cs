using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblRequestForSampleViewModel : PropertiesViewModelBase
    {
        private TblRequestForSampleStatu _requestForSampleStatusPerRow;

        public TblRequestForSampleStatu RequestForSampleStatusPerRow
        {
            get { return _requestForSampleStatusPerRow; }
            set
            {
                _requestForSampleStatusPerRow = value; RaisePropertyChanged("RequestForSampleStatusPerRow");

                if (RequestForSampleStatusPerRow.Iserial != 0)
                {
                    TblRequestForSampleStatus = RequestForSampleStatusPerRow.Iserial;
                }
            }
        }

        private DateTime? _creationDateField;

        private string _descriptionField;

        private DateTime? _estimatedDeliveryDateField;

        private byte[] _imageField;

        private string _imagePathField;

        private int _iserialField;

        private string _sizeField;

        private int _tblRequestForSampleStatus;

        private int? _tblColorField;

        private int _tblStyleField;

        private int? _tblSupplierField;

        private ObservableCollection<TBLsupplier> _supplier;

        public ObservableCollection<TBLsupplier> Suppliers
        {
            get { return _supplier; }
            set { _supplier = value; RaisePropertyChanged("Suppliers"); }
        }

        private string _serialNoField;

        public string SerialNo
        {
            get
            {
                return _serialNoField;
            }
            set
            {
                if ((ReferenceEquals(_serialNoField, value) != true))
                {
                    _serialNoField = value;
                    RaisePropertyChanged("SerialNo");
                }
            }
        }

        [ReadOnly(true)]
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
                    if (value == null)
                    {
                        value = DateTime.Now;
                    }
                    _creationDateField = value;

                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private string _code;

        [ReadOnly(true)]
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                if ((ReferenceEquals(_code, value) != true))
                {
                    _code = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
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
                }
            }
        }

        public byte[] Image
        {
            get
            {
                return _imageField;
            }
            set
            {
                if ((ReferenceEquals(_imageField, value) != true))
                {
                    _imageField = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePathField;
            }
            set
            {
                if ((ReferenceEquals(_imagePathField, value) != true))
                {
                    _imagePathField = value;
                    RaisePropertyChanged("ImagePath");
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSize")]
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

        public int TblRequestForSampleStatus
        {
            get
            {
                return _tblRequestForSampleStatus;
            }
            set
            {
                if ((_tblRequestForSampleStatus.Equals(value) != true))
                {
                    _tblRequestForSampleStatus = value;
                    RaisePropertyChanged("TblRequestForSampleStatus");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqColor")]
        public int? TblColor
        {
            get
            {
                return _tblColorField;
            }
            set
            {
                if ((_tblColorField.Equals(value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public int TblStyle
        {
            get
            {
                return _tblStyleField;
            }
            set
            {
                if ((_tblStyleField.Equals(value) != true))
                {
                    _tblStyleField = value;
                    RaisePropertyChanged("TblStyle");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSupplier")]
        public int? TblSupplier
        {
            get
            {
                return _tblSupplierField;
            }
            set
            {
                if ((_tblSupplierField.Equals(value) != true))
                {
                    _tblSupplierField = value;
                    RaisePropertyChanged("TblSupplier");
                }
            }
        }

        private SortableCollectionView<TblRequestForSampleItemViewModel> _tblSubProductGroupField;

        public SortableCollectionView<TblRequestForSampleItemViewModel> DetailsList
        {
            get
            {
                return _tblSubProductGroupField ?? (_tblSubProductGroupField = new SortableCollectionView<TblRequestForSampleItemViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblSubProductGroupField, value) != true))
                {
                    _tblSubProductGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        private SortableCollectionView<TblRequestForSampleServiceViewModel> _subDetailsList;

        public SortableCollectionView<TblRequestForSampleServiceViewModel> SubDetailsList
        {
            get
            {
                return _subDetailsList ?? (_subDetailsList = new SortableCollectionView<TblRequestForSampleServiceViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_subDetailsList, value) != true))
                {
                    _subDetailsList = value;
                    RaisePropertyChanged("SubDetailsList");
                }
            }
        }

        private SortableCollectionView<TblRequestForSampleEventViewModel> _subEventList;

        public SortableCollectionView<TblRequestForSampleEventViewModel> SubEventList
        {
            get
            {
                return _subEventList ?? (_subEventList = new SortableCollectionView<TblRequestForSampleEventViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_subEventList, value) != true))
                {
                    _subEventList = value;
                    RaisePropertyChanged("SubEventList");
                }
            }
        }

        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                if (value != null)
                {
                    _colorPerRow = value;
                    RaisePropertyChanged("ColorPerRow");
                }
            }
        }

        private TblSize _sizePerRow;

        public TblSize SizePerRow
        {
            get { return _sizePerRow; }
            set
            {
                if (value != null)
                {
                    _sizePerRow = value;
                    RaisePropertyChanged("SizePerRow");
                }
            }
        }

        private TBLsupplier _supplierPerRow;

        public TBLsupplier SupplierPerRow
        {
            get
            {
                return _supplierPerRow;
            }
            set
            {
                if ((ReferenceEquals(_supplierPerRow, value) != true))
                {
                    _supplierPerRow = value;
                    RaisePropertyChanged("SupplierPerRow");
                    if (SupplierPerRow != null) TblSupplier = SupplierPerRow.Iserial;
                }
            }
        }
    }

    public class TblRequestForSampleItemViewModel : PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblRequestForSampleItemViewModel()
        {
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;
                if (ItemPerRow.IsSizeIncluded == true)
                {
                    ItemPerRow.SizesList.Add(sv.Result.CombinationList.FirstOrDefault().Size);
                    Size = sv.Result.CombinationList.FirstOrDefault().Size;
                }
            };
        }

        private string _descriptionField;

        private string _fabricTypeField;

        private byte[] _imageField;

        private string _imagePathField;

        private int _iserialField;

        private int? _itemField;

        private string _nameField;

        private string _sizeField;

        private int? _tblColorField;

        private int? _tblRequestForSampleField;

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private bool _freeItem;

        public bool FreeItem
        {
            get { return _freeItem; }
            set { _freeItem = value; RaisePropertyChanged("FreeItem"); }
        }

        private ObservableCollection<ItemsDto> _items;

        public ObservableCollection<ItemsDto> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if ((ReferenceEquals(_items, value) != true))
                {
                    _items = value;
                    RaisePropertyChanged("Items");
                }
            }
        }

        private string _uoMidField;

        [ReadOnly(true)]
        public string Unit
        {
            get
            {
                return _uoMidField;
            }
            set
            {
                if ((ReferenceEquals(_uoMidField, value) != true))
                {
                    _uoMidField = value;
                    RaisePropertyChanged("Unit");
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
                if (ItemPerRow != null)
                {
                    Unit = ItemPerRow.Unit;
                    FabricType = ItemPerRow.ItemGroup;
                    Name = ItemPerRow.Name;
                    Item = ItemPerRow.Iserial;
                    if (ItemPerRow.ItemGroup != null) IsAcc = ItemPerRow.ItemGroup.Contains("Acc");
                    if (IsAcc)
                    {
                        _client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                    TblColor = null;
                    ColorPerRow = null;
                    Size = null;
                }
            }
        }

        private bool _isAcc;

        [ReadOnly(true)]
        public bool IsAcc
        {
            get { return _isAcc; }
            set
            {
                _isAcc = value; RaisePropertyChanged("IsAcc");
            }
        }

        [ReadOnly(true)]
        public string FabricType
        {
            get
            {
                return _fabricTypeField;
            }
            set
            {
                if ((ReferenceEquals(_fabricTypeField, value) != true))
                {
                    _fabricTypeField = value;
                    RaisePropertyChanged("FabricType");
                }
            }
        }

        public byte[] Image
        {
            get
            {
                return _imageField;
            }
            set
            {
                if ((ReferenceEquals(_imageField, value) != true))
                {
                    _imageField = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePathField;
            }
            set
            {
                if ((ReferenceEquals(_imagePathField, value) != true))
                {
                    _imagePathField = value;
                    RaisePropertyChanged("ImagePath");
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

        public int? Item
        {
            get
            {
                return _itemField;
            }
            set
            {
                if ((_itemField.Equals(value) != true))
                {
                    _itemField = value;
                    RaisePropertyChanged("Item");
                }
            }
        }

        [ReadOnly(true)]
        public string Name
        {
            get
            {
                return _nameField;
            }
            set
            {
                if ((ReferenceEquals(_nameField, value) != true))
                {
                    _nameField = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public string Size
        {
            get
            {
                return _sizeField;
            }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true) && (value != null))
                {
                    _sizeField = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        public int? TblColor
        {
            get
            {
                return _tblColorField;
            }
            set
            {
                if ((_tblColorField.Equals(value) != true) && (value != null))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public int? TblRequestForSample
        {
            get
            {
                return _tblRequestForSampleField;
            }
            set
            {
                if ((_tblRequestForSampleField.Equals(value) != true))
                {
                    _tblRequestForSampleField = value;
                    RaisePropertyChanged("TblRequestForSample");
                }
            }
        }

        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                if (value != null)
                {
                    _colorPerRow = value;
                    RaisePropertyChanged("ColorPerRow");
                    if (ItemPerRow != null && ItemPerRow.CombinationList != null)
                        ItemPerRow.SizesList = new ObservableCollection<string>(ItemPerRow.CombinationList.Where(x => x.Configuration == ColorPerRow.Code).Select(x => x.Size).Distinct());
                }
            }
        }

        private TblSize _sizePerRow;

        public TblSize SizePerRow
        {
            get { return _sizePerRow; }
            set
            {
                if (value != null)
                {
                    _sizePerRow = value;
                    RaisePropertyChanged("SizePerRow");
                }
            }
        }
    }

    public class TblRequestForSampleServiceViewModel : ViewModelBase
    {
        private byte[] _imageField;

        private string _imagePathField;

        private int _iserialField;

        private string _notesField;

        private int? _tblRequestForSampleField;

        private int? _tblServiceField;

        public byte[] Image
        {
            get
            {
                return _imageField;
            }
            set
            {
                if ((ReferenceEquals(_imageField, value) != true))
                {
                    _imageField = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePathField;
            }
            set
            {
                if ((ReferenceEquals(_imagePathField, value) != true))
                {
                    _imagePathField = value;
                    RaisePropertyChanged("ImagePath");
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

        public int? TblRequestForSample
        {
            get
            {
                return _tblRequestForSampleField;
            }
            set
            {
                if ((_tblRequestForSampleField.Equals(value) != true))
                {
                    _tblRequestForSampleField = value;
                    RaisePropertyChanged("TblRequestForSample");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqService")]
        public int? TblService
        {
            get
            {
                return _tblServiceField;
            }
            set
            {
                if ((_tblServiceField.Equals(value) != true))
                {
                    _tblServiceField = value;
                    RaisePropertyChanged("TblService");
                }
            }
        }

        private TblService _servicePerRow;

        public TblService ServicePerRow
        {
            get { return _servicePerRow; }
            set { _servicePerRow = value; RaisePropertyChanged("ServicePerRow"); }
        }
    }

    public class TblRequestForSampleEventViewModel : PropertiesViewModelBase
    {
        private bool _locked;

        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; RaisePropertyChanged("Locked"); }
        }

        private TblSalesOrderOperationViewModel _routePerRow;

        public TblSalesOrderOperationViewModel RoutePerRow
        {
            get { return _routePerRow; }
            set
            {
                _routePerRow = value; RaisePropertyChanged("RoutePerRow");
                if (RoutePerRow != null) TblRouteGroup = RoutePerRow.TblOperation;
            }
        }

        private int? _bomFabricRoutField;

        public int? TblRouteGroup
        {
            get
            {
                return _bomFabricRoutField;
            }
            set
            {
                if ((_bomFabricRoutField.Equals(value) != true))
                {
                    _bomFabricRoutField = value;
                    RaisePropertyChanged("TblRouteGroup");
                }
            }
        }

        private DateTime? _requestDate;

        private int _iserialField;

        private string _notesField;

        private int _tblRequestForSampleField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqRequestDate")]
        public DateTime? RequestDate
        {
            get
            {
                return _requestDate;
            }
            set
            {
                if ((_requestDate.Equals(value) != true))
                {
                    _requestDate = value;
                    RaisePropertyChanged("RequestDate");
                }
            }
        }

        private DateTime? _deliveryDate;

        public DateTime? DeliveryDate
        {
            get
            {
                return _deliveryDate;
            }
            set
            {
                if ((_deliveryDate.Equals(value) != true))
                {
                    _deliveryDate = value;
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

        private int? _approvedBy;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqApprovedBy")]
        public int? ApprovedBy
        {
            get
            {
                return _approvedBy;
            }
            set
            {
                if ((_approvedBy.Equals(value) != true))
                {
                    _approvedBy = value;
                    RaisePropertyChanged("ApprovedBy");
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

        public int TblSalesOrder
        {
            get
            {
                return _tblRequestForSampleField;
            }
            set
            {
                if ((_tblRequestForSampleField.Equals(value) != true))
                {
                    _tblRequestForSampleField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

        private int? _tblRequestForSampleStatus;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStatus")]
        public int? TblRequestForSampleStatus
        {
            get
            {
                return _tblRequestForSampleStatus;
            }
            set
            {
                if ((_tblRequestForSampleStatus.Equals(value) != true))
                {
                    _tblRequestForSampleStatus = value;
                    RaisePropertyChanged("TblRequestForSampleStatus");
                }
            }
        }

        private TblRequestForSampleStatu _requestForSampleStatusPerRow;

        public TblRequestForSampleStatu RequestForSampleStatusPerRow
        {
            get { return _requestForSampleStatusPerRow; }
            set { _requestForSampleStatusPerRow = value; RaisePropertyChanged("RequestForSampleStatusPerRow"); }
        }

        private TblAuthUser _userPerRow;

        public TblAuthUser UserPerRow
        {
            get { return _userPerRow; }
            set { _userPerRow = value; RaisePropertyChanged("UserPerRow"); }
        }
    }

    #endregion ViewModels

    public class RequestForSampleViewModel : ViewModelBase
    {
        public event EventHandler ItemCompletedCompleted;

        public event EventHandler SupplierItemCompleted;

        public RequestForSampleViewModel(StyleHeaderViewModel styleViewModel)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.RequestForSampleForm.ToString());
                GetCustomePermissions(PermissionItemName.RequestForSampleForm.ToString());
                Client.GetTblRequestForSampleStatusCompleted += (s, sv) =>
                {
                    RequestForSampleStatusList = sv.Result;
                };

                Client.ViewReportCompleted += (s, sv) => Client.SendMailReportAsync(LoggedUserInfo.Code, "RequestForSample", Subject, Body);

                Client.GetTblRequestForSampleStatusAsync();

                Style = styleViewModel;

                MainRowList = new SortableCollectionView<TblRequestForSampleViewModel>();
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;
                SelectedMainRow = new TblRequestForSampleViewModel();
                SelectedDetailRow = new TblRequestForSampleItemViewModel();
                SelectedSubDetailRow = new TblRequestForSampleServiceViewModel();
                SelectedRequestForSampleEvent = new TblRequestForSampleEventViewModel();

                

                //Client.GetAllUsersAsync(0, int.MaxValue, "it.Iserial", null, null);
                //Client.GetAllUsersCompleted += (s, sv) =>
                //{
                //    UsersList = sv.Result;
                //};
                Client.GetTblColorLinkAsync(0, int.MaxValue, Style.SelectedMainRow.Brand, (int)Style.SelectedMainRow.TblLkpBrandSection, (int)Style.SelectedMainRow.TblLkpSeason, "it.TblColor", null, null);
                Client.GetTblColorLinkCompleted += (s, sv) =>
                {
                    ColorList = new ObservableCollection<TblColor>();
                    foreach (var row in sv.Result)
                    {
                        ColorList.Add(row.TblColor1);
                    }
                };

                Client.GetTblServiceAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblServiceCompleted += (s, sv) =>
                {
                    ServicesList = sv.Result;
                };

                Client.GetTblSizeCodeAsync(0, int.MaxValue, (int)Style.SelectedMainRow.TblSizeGroup);

                Client.GetTblSizeCodeCompleted += (s, sv) =>
                {
                    Sizes = sv.Result;
                };
                Client.GetTblRequestForSampleCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRequestForSampleViewModel();
                        newrow.InjectFrom(row);
                        newrow.ColorPerRow = row.TblColor1;
                        newrow.SupplierPerRow = sv.SupplierList.FirstOrDefault(x => x.Iserial == row.TblSupplier);
                        newrow.RequestForSampleStatusPerRow = new TblRequestForSampleStatu();
                        newrow.RequestForSampleStatusPerRow.InjectFrom(row.TblRequestForSampleStatu);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblRequestForSampleItemCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRequestForSampleItemViewModel
                        {
                            ColorPerRow = row.TblColor1,
                            ItemPerRow =
                                sv.itemsList.SingleOrDefault(x => x.Iserial == row.Item && x.ItemGroup == row.FabricType)
                        };
                        newrow.InjectFrom(row);

                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Client.UpdateOrInsertTblRequestForSampleCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblRequestForSampleViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.RequestForSampleStatusPerRow = new TblRequestForSampleStatu();

                        savedRow.RequestForSampleStatusPerRow.InjectFrom(RequestForSampleStatusList.FirstOrDefault(w => w.Iserial == savedRow.TblRequestForSampleStatus));
                    }
                };

                Client.UpdateOrInsertTblRequestForSampleItemCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblRequestForSampleItemViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    //if (x.Result.TblSalaryRelation1 != null)
                    //{
                    //    var headerIserial = x.Result.TblSalaryRelation;

                    //    SelectedMainRow.Iserial = headerIserial;
                    //}
                };

                Client.GetTblRequestForSampleServiceCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRequestForSampleServiceViewModel();
                        newrow.InjectFrom(row);
                        newrow.ServicePerRow = row.TblService1;
                        SelectedMainRow.SubDetailsList.Add(newrow);
                    }

                    Loading = false;
                    if (DetailSubFullCount == 0 && SelectedMainRow.SubDetailsList.Count == 0)
                    {
                        AddNewSubDetailRow(false);
                    }
                };

                Client.UpdateOrInsertTblRequestForSampleServiceCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblRequestForSampleServiceViewModel)SelectedMainRow.SubDetailsList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.DeleteTblRequestForSampleServiceCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.SubDetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.SubDetailsList.Remove(oldrow);
                };

                Client.GetTblRequestForSampleEventCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRequestForSampleEventViewModel();
                        newrow.InjectFrom(row);
                        newrow.RequestForSampleStatusPerRow = RequestForSampleStatusList.FirstOrDefault(x => x.Iserial == row.TblRequestForSampleStatus);
                        newrow.UserPerRow = row.TblAuthUser;

                        SelectedMainRow.SubEventList.Add(newrow);
                    }
                    if (SelectedMainRow.SubEventList.Any())
                    {
                        var lastRow = SelectedMainRow.SubEventList.OrderBy(x => x.Iserial).LastOrDefault();
                        SelectedRequestForSampleEvent = lastRow;
                    }

                    Loading = false;
                    if (SelectedMainRow != null && SelectedMainRow.SubEventList.Count == 0)
                    {
                        AddNewSubEventRow(false);
                    }
                };

                Client.UpdateOrInsertTblRequestForSampleEventCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblRequestForSampleEventViewModel)SelectedMainRow.SubEventList.GetItemAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        var lastRow = SelectedMainRow.SubEventList.OrderBy(w => w.Iserial).LastOrDefault();
                        SelectedMainRow.RequestForSampleStatusPerRow = lastRow.RequestForSampleStatusPerRow;
                    }
                };

                Client.DeleteTblRequestForSampleEventCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.SubEventList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.SubEventList.Remove(oldrow);
                };

                Client.DeleteTblRequestForSampleCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblRequestForSampleItemCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                GetMaindata();
            }
        }

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblRequestForSampleViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblRequestForSampleViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "TblSupplier":
                    SelectedMainRow.Code = Style.SelectedMainRow.StyleCode + "-" + SelectedMainRow.SupplierPerRow.Code;
                    break;
            }
        }

        private ObservableCollection<TblService> _servicesList;

        public ObservableCollection<TblService> ServicesList
        {
            get { return _servicesList; }
            set { _servicesList = value; RaisePropertyChanged("ServicesList"); }
        }

        private ObservableCollection<TblAuthUser> _usersList;

        public ObservableCollection<TblAuthUser> UsersList
        {
            get { return _usersList; }
            set { _usersList = value; RaisePropertyChanged("UsersList"); }
        }

        protected virtual void OnItemPopulatingCompleted()
        {
            var handler = ItemCompletedCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnSupplierCompleted()
        {
            var handler = SupplierItemCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblRequestForSampleAsync(MainRowList.Count, PageSize, Style.SelectedMainRow.Iserial, SortBy, Filter, ValuesObjects);
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    Loading = true;
                    foreach (var row in SelectedMainRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblRequestForSampleAsync(
                                (TblRequestForSample)new TblRequestForSample().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = SelectedMainRow != null && Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var lastRow = SelectedMainRow.SubEventList.OrderBy(x => x.Iserial).LastOrDefault();

            if (lastRow != null && LoggedUserInfo.Iserial != lastRow.ApprovedBy)
            {
                MessageBox.Show("Cannot Edit This Sample Because The Last Event Wasn't Made By You");
                return;
            }

            var newrow = new TblRequestForSampleViewModel
                {
                    CreationDate = DateTime.Now,
                    TblStyle = Style.SelectedMainRow.Iserial,
                    //       RequestForSampleStatusPerRow = RequestForSampleStatusList.FirstOrDefault(x => x.Code == "Pending")
                };

            MainRowList.Insert(currentRowIndex + 1, newrow);

            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblRequestForSample();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblRequestForSampleAsync(saveRow, LoggedUserInfo.Iserial, save, MainRowList.IndexOf(SelectedMainRow));
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        public void GetDetailData()
        {
            Loading = true;
            if (SelectedMainRow != null)
                Client.GetTblRequestForSampleItemAsync(SelectedMainRow.Iserial);
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    Loading = true;
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblRequestForSampleItemAsync((TblRequestForSampleItem)new TblRequestForSampleItem().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = SelectedDetailRow != null && Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);
                if (SelectedDetailRow.FreeItem && string.IsNullOrWhiteSpace(SelectedDetailRow.Description))
                {
                    isvalid = false;
                }

                if (!SelectedDetailRow.FreeItem && SelectedDetailRow.Item == null)
                {
                    isvalid = false;
                }
                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblRequestForSampleItemViewModel
                {
                    TblRequestForSample = SelectedMainRow.Iserial
                };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);

            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);
                if (SelectedDetailRow.FreeItem && string.IsNullOrWhiteSpace(SelectedDetailRow.Description))
                {
                    isvalid = false;
                }

                if (!SelectedDetailRow.FreeItem && SelectedDetailRow.Item == null)
                {
                    isvalid = false;
                }
                if (isvalid)
                {
                    Loading = true;
                    var save = SelectedDetailRow.Iserial == 0;
                    var rowToSave = new TblRequestForSampleItem();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblRequestForSampleItemAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        public void GetSubDetailData()
        {
            Loading = true;
            if (SelectedMainRow != null)
                Client.GetTblRequestForSampleServiceAsync(SelectedMainRow.Iserial);
        }

        public void DeleteSubDetailRow()
        {
            if (SelectedSubDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    Loading = true;
                    foreach (var row in SelectedSubDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblRequestForSampleServiceAsync((TblRequestForSampleService)new TblRequestForSampleService().InjectFrom(row), SelectedMainRow.SubDetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.SubDetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewSubDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.SubDetailsList.IndexOf(SelectedSubDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = SelectedSubDetailRow != null && Validator.TryValidateObject(SelectedSubDetailRow, new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblRequestForSampleServiceViewModel
            {
                TblRequestForSample = SelectedMainRow.Iserial
            };

            SelectedMainRow.SubDetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedSubDetailRow = newrow;
        }

        public void SaveSubDetailRow()
        {
            if (SelectedSubDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSubDetailRow, new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    Loading = true;
                    var save = SelectedSubDetailRow.Iserial == 0;
                    var rowToSave = new TblRequestForSampleService();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    Client.UpdateOrInsertTblRequestForSampleServiceAsync(rowToSave, save, SelectedMainRow.SubDetailsList.IndexOf(SelectedSubDetailRow));
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        public void GetRequestForSampleEventData()
        {
            Loading = true;
            if (SelectedMainRow != null)
                Client.GetTblRequestForSampleEventAsync(SelectedMainRow.Iserial);
        }

        public void DeleteRequestForSampleEventRow()
        {
            if (SelectedRequestForSampleEvents != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    Loading = true;
                    foreach (var row in SelectedRequestForSampleEvents)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblRequestForSampleEventAsync((TblRequestForSampleEvent)new TblRequestForSampleEvent().InjectFrom(row), SelectedMainRow.SubEventList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.SubEventList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewSubEventRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.SubEventList.IndexOf(SelectedRequestForSampleEvent));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = SelectedRequestForSampleEvent != null && Validator.TryValidateObject(SelectedRequestForSampleEvent, new ValidationContext(SelectedRequestForSampleEvent, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow =
                 new TblRequestForSampleEventViewModel
                {
                    TblSalesOrder = SelectedMainRow.Iserial,
                    UserPerRow = new TblAuthUser
                    {
                        Iserial = LoggedUserInfo.Iserial
                    ,
                        Code = LoggedUserInfo.Code
                    ,
                        UserName = LoggedUserInfo.WFM_UserName
                    }

                    ,// UsersList.FirstOrDefault(x => x.Code == LoggedUserInfo.Code),
                    ApprovedBy = LoggedUserInfo.Iserial
                };
            SelectedMainRow.SubEventList.Insert(currentRowIndex + 1, newrow);
            SelectedRequestForSampleEvent = newrow;
        }

        public void SaveSubEventRow()
        {
            if (SelectedRequestForSampleEvent != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedRequestForSampleEvent, new ValidationContext(SelectedRequestForSampleEvent, null, null), valiationCollection, true);

                if (isvalid)
                {
                    Loading = true;
                    var save = SelectedRequestForSampleEvent.Iserial == 0;
                    var rowToSave = new TblRequestForSampleEvent();
                    rowToSave.InjectFrom(SelectedRequestForSampleEvent);
                    if (!save)
                    {
                        if (LoggedUserInfo.Iserial != SelectedRequestForSampleEvent.ApprovedBy)
                        {
                            MessageBox.Show("Cannot Edit Transaction Made By Another User");
                        }
                    }
                    Client.UpdateOrInsertTblRequestForSampleEventAsync(rowToSave, save, SelectedMainRow.SubEventList.IndexOf(SelectedRequestForSampleEvent));
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblRequestForSampleViewModel> _mainRowList;

        public SortableCollectionView<TblRequestForSampleViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblRequestForSampleViewModel> _selectedMainRows;

        public ObservableCollection<TblRequestForSampleViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblRequestForSampleViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblRequestForSampleViewModel _selectedMainRow;

        public TblRequestForSampleViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblRequestForSampleItemViewModel _selectedDetailRow;

        public TblRequestForSampleItemViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblRequestForSampleStatu> _requestForSampleStatusList;

        public ObservableCollection<TblRequestForSampleStatu> RequestForSampleStatusList
        {
            get { return _requestForSampleStatusList ?? (_requestForSampleStatusList = new ObservableCollection<TblRequestForSampleStatu>()); }
            set { _requestForSampleStatusList = value; RaisePropertyChanged("RequestForSampleStatusList"); }
        }

        private ObservableCollection<TblRequestForSampleItemViewModel> _selectedDetailRows;

        public ObservableCollection<TblRequestForSampleItemViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblRequestForSampleItemViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private TblRequestForSampleServiceViewModel _selectedSubDetailRow;

        public TblRequestForSampleServiceViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set { _selectedSubDetailRow = value; RaisePropertyChanged("SelectedSubDetailRow"); }
        }

        private ObservableCollection<TblRequestForSampleServiceViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblRequestForSampleServiceViewModel> SelectedSubDetailRows
        {
            get
            {
                return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblRequestForSampleServiceViewModel>());
            }
            set { _selectedSubDetailRows = value; RaisePropertyChanged("SelectedSubDetailRows"); }
        }

        private TblRequestForSampleEventViewModel _selectedRequestForSampleEvent;

        public TblRequestForSampleEventViewModel SelectedRequestForSampleEvent
        {
            get { return _selectedRequestForSampleEvent; }
            set { _selectedRequestForSampleEvent = value; RaisePropertyChanged("SelectedRequestForSampleEvent"); }
        }

        private ObservableCollection<TblRequestForSampleEventViewModel> _selectedRequestForSampleEvents;

        public ObservableCollection<TblRequestForSampleEventViewModel> SelectedRequestForSampleEvents
        {
            get
            {
                return _selectedRequestForSampleEvents ?? (_selectedRequestForSampleEvents = new ObservableCollection<TblRequestForSampleEventViewModel>());
            }
            set { _selectedRequestForSampleEvents = value; RaisePropertyChanged("SelectedRequestForSampleEvents"); }
        }

        private StyleHeaderViewModel _style;

        private ObservableCollection<TblSize> _sizes;

        public ObservableCollection<TblSize> Sizes
        {
            get { return _sizes; }
            set { _sizes = value; RaisePropertyChanged("Sizes"); }
        }

        private ObservableCollection<TblColor> _colorList;

        public ObservableCollection<TblColor> ColorList
        {
            get { return _colorList; }
            set { _colorList = value; RaisePropertyChanged("ColorList"); }
        }

        public StyleHeaderViewModel Style
        {
            get { return _style; }
            set { _style = value; RaisePropertyChanged("Style"); }
        }

        #endregion Prop

        internal void SearchForFabric(string fabric)
        {
            //      Client.GetItemWithUnitAndItemGroupAsync(fabric);
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool RequestForSampleOpen { get; set; }

        public void SendMail(ObservableCollection<string> para, string subject, string body)
        {
            Client.ViewReportAsync("RequestForSample", para);
            Subject = subject;
            Body = body;
        }
    }
}