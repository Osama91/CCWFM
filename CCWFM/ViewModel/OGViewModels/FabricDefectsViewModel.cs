using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.OGViewModels.Mappers;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class Degrees : PropertiesViewModelBase
    {
        private short _degreeNum;

        public short DegreeNum
        {
            get
            {
                return _degreeNum;
            }
            set
            {
                if ((_degreeNum.Equals(value) != true))
                {
                    _degreeNum = value;
                    RaisePropertyChanged("DegreeNum");
                }
            }
        }

        private string _degreEname;

        public string DegreEname
        {
            get
            {
                return _degreEname;
            }
            set
            {
                if ((ReferenceEquals(_degreEname, value) != true))
                {
                    _degreEname = value;
                    RaisePropertyChanged("DegreEname");
                }
            }
        }
    }

    public class OrderLineListViewModel : PropertiesViewModelBase
    {
        private string _colorCodeField;

        private string _colorNameField;

        private DateTime? _createdDateField;

        private string _fabricCodeField;

        private string _fabricEnameField;

        private string _inventbatchidField;

        private string _purchidField;

        private decimal _purchqtyField;

        private string _purchunitField;

        private float _totalQtyField;

        private string _vendaccountField;

        private decimal _lineNum;

        [ReadOnly(true)]
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

        [ReadOnly(true)]
        public string Color_Code
        {
            get
            {
                return _colorCodeField;
            }
            set
            {
                if ((ReferenceEquals(_colorCodeField, value) != true))
                {
                    _colorCodeField = value;
                    RaisePropertyChanged("Color_Code");
                }
            }
        }

        [ReadOnly(true)]
        public string Color_Name
        {
            get
            {
                return _colorNameField;
            }
            set
            {
                if ((ReferenceEquals(_colorNameField, value) != true))
                {
                    _colorNameField = value;
                    RaisePropertyChanged("Color_Name");
                }
            }
        }

        private int _noOfRolls;

        public int NoOfRolls
        {
            get
            {
                return _noOfRolls;
            }
            set
            {
                if ((_noOfRolls.Equals(value) != true))
                {
                    _noOfRolls = value;
                    RaisePropertyChanged("NoOfRolls");
                }
            }
        }

        private string _batchno;

        [ReadOnly(true)]
        public string BatchNo
        {
            get
            {
                return _batchno;
            }
            set
            {
                if ((ReferenceEquals(_batchno, value) != true))
                {
                    _batchno = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }

        [ReadOnly(true)]
        public DateTime? CreatedDate
        {
            get
            {
                return _createdDateField;
            }
            set
            {
                if ((_createdDateField.Equals(value) != true))
                {
                    _createdDateField = value;
                    RaisePropertyChanged("CreatedDate");
                }
            }
        }

        [ReadOnly(true)]
        public string Fabric_Code
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
                    RaisePropertyChanged("Fabric_Code");
                }
            }
        }

        [ReadOnly(true)]
        public string Fabric_Ename
        {
            get
            {
                return _fabricEnameField;
            }
            set
            {
                if ((ReferenceEquals(_fabricEnameField, value) != true))
                {
                    _fabricEnameField = value;
                    RaisePropertyChanged("Fabric_Ename");
                }
            }
        }

        [ReadOnly(true)]
        public string INVENTBATCHID
        {
            get
            {
                return _inventbatchidField;
            }
            set
            {
                if ((ReferenceEquals(_inventbatchidField, value) != true))
                {
                    _inventbatchidField = value;
                    RaisePropertyChanged("INVENTBATCHID");
                }
            }
        }

        [ReadOnly(true)]
        public string PURCHID
        {
            get
            {
                return _purchidField;
            }
            set
            {
                if ((ReferenceEquals(_purchidField, value) != true))
                {
                    _purchidField = value;
                    RaisePropertyChanged("PURCHID");
                }
            }
        }

        [ReadOnly(true)]
        public decimal PURCHQTY
        {
            get
            {
                return _purchqtyField;
            }
            set
            {
                if ((_purchqtyField.Equals(value) != true))
                {
                    _purchqtyField = value;
                    RaisePropertyChanged("PURCHQTY");
                }
            }
        }

        [ReadOnly(true)]
        public string PURCHUNIT
        {
            get
            {
                return _purchunitField;
            }
            set
            {
                if ((ReferenceEquals(_purchunitField, value) != true))
                {
                    _purchunitField = value;
                    RaisePropertyChanged("PURCHUNIT");
                }
            }
        }

        [ReadOnly(true)]
        public float TotalQty
        {
            get
            {
                return _totalQtyField;
            }
            set
            {
                if ((_totalQtyField.Equals(value) != true))
                {
                    _totalQtyField = value;
                    RaisePropertyChanged("TotalQty");
                }
            }
        }

        [ReadOnly(true)]
        public string VENDACCOUNT
        {
            get
            {
                return _vendaccountField;
            }
            set
            {
                if ((ReferenceEquals(_vendaccountField, value) != true))
                {
                    _vendaccountField = value;
                    RaisePropertyChanged("VENDACCOUNT");
                }
            }
        }

        private string _warehouse;

        [ReadOnly(true)]
        public string Warehouse
        {
            get { return _warehouse; }
            set
            {
                _warehouse = value;
                RaisePropertyChanged("Warehouse");
            }
        }

        private string _location;

        [ReadOnly(true)]
        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged("Location");
            }
        }

        private string _site;

        [ReadOnly(true)]
        public string Site
        {
            get { return _site; }
            set
            {
                _site = value;
                RaisePropertyChanged("Site");
            }
        }

        private float _unitPrice;

        [ReadOnly(true)]
        public float UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value; RaisePropertyChanged("UnitPrice");
                }
            }
        }
    }

    public class TblFabricInspectionHeaderViewModel : PropertiesViewModelBase
    {
        public TblFabricInspectionHeaderViewModel()
        {
            TransDate = DateTime.Now;
        }

        private string _Notes;

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; RaisePropertyChanged("Notes"); }
        }

        private float _totalQty;

        public float TotalQty
        {
            get { return _totalQty; }
            set { _totalQty = value; RaisePropertyChanged("TotalQty"); }
        }

        private string _fabricCode;

        public string FabricCode
        {
            get { return _fabricCode; }
            set { _fabricCode = value; RaisePropertyChanged("FabricCode"); }
        }
        private string _BatchNo;

        public string BatchNo
        {
            get { return _BatchNo; }
            set { _BatchNo = value; RaisePropertyChanged("BatchNo"); }
        }

        private string _ColorCode;

        public string ColorCode
        {
            get { return _ColorCode; }
            set { _ColorCode = value; RaisePropertyChanged("ColorCode"); }
        }
        


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

        private bool _vendorSubtraction;

        public bool VendorSubtraction
        {
            get { return _vendorSubtraction; }
            set { _vendorSubtraction = value; RaisePropertyChanged("VendorSubtraction"); }
        }

        private string _brandField;

        private bool? _postedToAxField;

        private string _orderField;

        private string _seasonField;

        private DateTime _transDateField;

        private int _transactionTypeField;

        private string _vendorField;

        private int _iserialField;

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
                }
            }
        }

        public bool? PostedToAx
        {
            get
            {
                return _postedToAxField;
            }
            set
            {
                if ((_postedToAxField.Equals(value) != true))
                {
                    _postedToAxField = value;
                    RaisePropertyChanged("PostedToAx");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqOrder")]
        public string Order
        {
            get
            {
                return _orderField;
            }
            set
            {
                if ((ReferenceEquals(_orderField, value) != true))
                {
                    _orderField = value;
                    RaisePropertyChanged("Order");
                }
            }
        }

        public string Season
        {
            get
            {
                return _seasonField;
            }
            set
            {
                if ((ReferenceEquals(_seasonField, value) != true))
                {
                    _seasonField = value;
                    RaisePropertyChanged("Season");
                }
            }
        }

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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime TransDate
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

        private CRUD_ManagerServicePurchaseOrderDto _journalPerRow;

        public CRUD_ManagerServicePurchaseOrderDto JournalPerRow
        {
            get { return _journalPerRow; }
            set
            {
                _journalPerRow = value; RaisePropertyChanged("JournalPerRow");
                if (JournalPerRow != null) Order = JournalPerRow.JournalId;
                if (JournalPerRow != null) VendorProperty = JournalPerRow.VendorCode;
            }
        }

        #region VisibilityPropertiesAndMethods

        private Visibility _includDefects;

        public Visibility IncludDefects
        {
            get { return _includDefects; }
            set { _includDefects = value; RaisePropertyChanged("IncludDefects"); }
        }

        private Visibility _purVis;

        public Visibility PurVis
        {
            get { return _purVis; }
            set { _purVis = value; RaisePropertyChanged("PurVis"); }
        }

        private Visibility _dyeingVis;

        public Visibility DyeingVis
        {
            get { return _dyeingVis; }
            set { _dyeingVis = value; RaisePropertyChanged("DyeingVis"); }
        }

        private Visibility _opBalanceVis;

        public Visibility OpBalanceVis
        {
            get { return _opBalanceVis; }
            set { _opBalanceVis = value; RaisePropertyChanged("OpBalanceVis"); }
        }

        private void VisModes()
        {
            switch (TransactionType)
            {
                case 0:
                    PurVis = Visibility.Visible;
                    DyeingVis = Visibility.Collapsed;
                    OpBalanceVis = Visibility.Collapsed;
                    IncludDefects = Visibility.Visible;
                    break;

                case 1:

                    PurVis = Visibility.Visible;
                    DyeingVis = Visibility.Collapsed;
                    OpBalanceVis = Visibility.Collapsed;
                    IncludDefects = Visibility.Visible;
                    break;

                case 2:

                    PurVis = Visibility.Visible;
                    DyeingVis = Visibility.Collapsed;
                    OpBalanceVis = Visibility.Collapsed;
                    IncludDefects = Visibility.Visible;
                    break;
            }
        }

        #endregion VisibilityPropertiesAndMethods

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

                    VisModes();

                    RaisePropertyChanged("TransactionType");
                }
            }
        }

        public string VendorProperty
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
                    RaisePropertyChanged("VendorProperty");
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
    }

    public class TblFabricInspectionDetailViewModel : PropertiesViewModelBase
    {

        private bool _IsOpen;

        public bool IsOpen
        {
            get { return _IsOpen; }
            set { _IsOpen = value; RaisePropertyChanged("IsOpen"); }
        }

        public TblFabricInspectionDetailViewModel()
        {
            DetailsViewModel = new ObservableCollection<TblFabricInspectionDetailDefectsViewModel>();
        }

        private string _orgLocation;

        public string OrgLocation
        {
            get { return _orgLocation; }
            set { _orgLocation = value; RaisePropertyChanged("OrgLocation"); }
        }

        private short? _degree;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDegree")]
        public short? Degree
        {
            get
            {
                return _degree;
            }
            set
            {
                if ((_degree.Equals(value) != true))
                {
                    _degree = value;

                    RaisePropertyChanged("Degree");
                }
            }
        }

        private decimal _lineNum;

        [ReadOnly(true)]
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

        private string _batchNoField;

        private string _colorCodeField;

        private float _consPerPcField;

        private string _fabricCodeField;

        private int _tblFabricInspectionHeader;

        private float? _m2WeightGmField;

        private float _netRollWmtField;

        private float _noofPCsField;

        private int _iserial;

        private float _remainingRollQtyField;

        private int _rollNoField;

        private float? _rollWmtField;

        private float _storeRollQtyField;

        private string _unitField;

        private string _finishedWarehouse;

        [ReadOnly(true)]
        public string FinishedWarehouse
        {
            get
            {
                return _finishedWarehouse;
            }
            set
            {
                if ((ReferenceEquals(_finishedWarehouse, value) != true))
                {
                    _finishedWarehouse = value;
                    RaisePropertyChanged("FinishedWarehouse");
                }
            }
        }

        private string _finishedLocation;

        [ReadOnly(true)]
        public string FinishedLocation
        {
            get { return _finishedLocation; }
            set
            {
                if (_finishedLocation != value)
                {
                    _finishedLocation = value; RaisePropertyChanged("FinishedLocation");
                }
            }
        }

        private string _finishedSite;

        [ReadOnly(true)]
        public string FinishedSite
        {
            get { return _finishedSite; }
            set
            {
                if (_finishedSite != value)
                {
                    _finishedSite = value; RaisePropertyChanged("FinishedSite");
                }
            }
        }

        [ReadOnly(true)]
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

        [ReadOnly(true)]
        public string ColorCode
        {
            get
            {
                return _colorCodeField;
            }
            set
            {
                if ((ReferenceEquals(_colorCodeField, value) != true))
                {
                    _colorCodeField = value;
                    RaisePropertyChanged("ColorCode");
                }
            }
        }

        public float ConsPerPC
        {
            get
            {
                return _consPerPcField;
            }
            set
            {
                if ((_consPerPcField.Equals(value) != true))
                {
                    _consPerPcField = value;
                    RaisePropertyChanged("ConsPerPC");
                }
            }
        }

        [ReadOnly(true)]
        public string Fabric_Code
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
                    RaisePropertyChanged("Fabric_Code");
                }
            }
        }

        [ReadOnly(true)]
        public int Tbl_fabricInspectionHeader
        {
            get
            {
                return _tblFabricInspectionHeader;
            }
            set
            {
                if ((_tblFabricInspectionHeader.Equals(value) != true))
                {
                    _tblFabricInspectionHeader = value;
                    RaisePropertyChanged("Tbl_fabricInspectionHeader");
                }
            }
        }

        public float? M2WeightGm
        {
            get
            {
                return _m2WeightGmField;
            }
            set
            {
                if ((_m2WeightGmField.Equals(value) != true))
                {
                    _m2WeightGmField = value;
                    RaisePropertyChanged("M2WeightGm");
                }
            }
        }

        public float NetRollWMT
        {
            get
            {
                return _netRollWmtField;
            }
            set
            {
                if ((_netRollWmtField.Equals(value) != true))
                {
                    _netRollWmtField = value;
                    RaisePropertyChanged("NetRollWMT");
                }
            }
        }

        public float NoofPCs
        {
            get
            {
                return _noofPCsField;
            }
            set
            {
                if ((_noofPCsField.Equals(value) != true))
                {
                    _noofPCsField = value;
                    RaisePropertyChanged("NoofPCs");
                }
            }
        }

        [ReadOnly(true)]
        public int Iserial
        {
            get
            {
                return _iserial;
            }
            set
            {
                if ((_iserial.Equals(value) != true))
                {
                    _iserial = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [ReadOnly(true)]
        public float RemainingRollQty
        {
            get
            {
                return _remainingRollQtyField;
            }
            set
            {
                if ((_remainingRollQtyField.Equals(value) != true))
                {
                    _remainingRollQtyField = value;
                    RaisePropertyChanged("RemainingRollQty");
                }
            }
        }

        [ReadOnly(true)]
        public int RollNo
        {
            get
            {
                return _rollNoField;
            }
            set
            {
                if ((Equals(_rollNoField, value) != true))
                {
                    _rollNoField = value;
                    RaisePropertyChanged("RollNo");
                }
            }
        }

        public float? RollWMT
        {
            get
            {
                return _rollWmtField;
            }
            set
            {
                if ((_rollWmtField.Equals(value) != true))
                {
                    _rollWmtField = value;
                    RaisePropertyChanged("RollWMT");
                }
            }
        }

        public float StoreRollQty
        {
            get
            {
                return _storeRollQtyField;
            }
            set
            {
                if ((_storeRollQtyField.Equals(value) != true))
                {
                    _storeRollQtyField = value;
                    RaisePropertyChanged("StoreRollQty");
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

        private float _unitPrice;

        public float UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value; RaisePropertyChanged("UnitPrice");
                }
            }
        }

        private ObservableCollection<TblFabricInspectionDetailDefectsViewModel> _detailsViewModel;

        public ObservableCollection<TblFabricInspectionDetailDefectsViewModel> DetailsViewModel
        {
            get
            {
                return _detailsViewModel;
            }
            set
            {
                if ((ReferenceEquals(_detailsViewModel, value) != true))
                {
                    _detailsViewModel = value;
                    RaisePropertyChanged("DetailsViewModel");
                }
            }
        }

        private float _totalLineNumQty;

        [ReadOnly(true)]
        public float TotalLineNumQty
        {
            get { return _totalLineNumQty; }
            set
            {
                if (_totalLineNumQty != value)
                {
                    _totalLineNumQty = value; RaisePropertyChanged("TotalLineNumQty");
                }
            }
        }
    }

    public class TblFabricInspectionDetailDefectsViewModel : PropertiesViewModelBase
    {
        private int _counter;

        public int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                if ((_counter.Equals(value) != true))
                {
                    _counter = value;
                    RaisePropertyChanged("Counter");
                }
            }
        }

        private int _defectIserialField;

        private int _defectValueField;

        private int? _tblFabricInspectionDetail;

        private int _iserialField;

        public int DefectIserial
        {
            get
            {
                return _defectIserialField;
            }
            set
            {
                if ((_defectIserialField.Equals(value) != true))
                {
                    _defectIserialField = value;
                    RaisePropertyChanged("DefectIserial");
                }
            }
        }

        public int DefectValue
        {
            get
            {
                return _defectValueField;
            }
            set
            {
                if ((_defectValueField.Equals(value) != true))
                {
                    _defectValueField = value;
                    RaisePropertyChanged("DefectValue");
                }
            }
        }

        public int? Tbl_fabricInspectionDetail
        {
            get
            {
                return _tblFabricInspectionDetail;
            }
            set
            {
                if ((_tblFabricInspectionDetail.Equals(value) != true))
                {
                    _tblFabricInspectionDetail = value;
                    RaisePropertyChanged("Tbl_fabricInspectionDetail");
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
    }

    #endregion ViewModels

    public class FabricDefectsViewModel : ViewModelBase
    {
        #region Prop

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

        private string _fabricToSearch;

        public string FabricToSearch
        {
            get { return _fabricToSearch; }
            set { _fabricToSearch = value; RaisePropertyChanged("FabricToSearch"); }
        }

        private ObservableCollection<WMSLOCATION> _wmslocation;

        public ObservableCollection<WMSLOCATION> WmsLocationList
        {
            get { return _wmslocation; }
            set
            {
                if (_wmslocation != value)
                {
                    _wmslocation = value; RaisePropertyChanged("WmsLocationList");
                }
            }
        }

        private ObservableCollection<Degrees> _degreesList;

        public ObservableCollection<Degrees> DegreesList
        {
            get
            {
                return _degreesList;
            }
            set
            {
                if ((ReferenceEquals(_degreesList, value) != true))
                {
                    _degreesList = value;
                    RaisePropertyChanged("DegreesList");
                }
            }
        }

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

        private ObservableCollection<TblFamily> _familyList;

        public ObservableCollection<TblFamily> FamilyList
        {
            get
            {
                return _familyList;
            }
            set
            {
                if ((ReferenceEquals(_familyList, value) != true))
                {
                    _familyList = value;
                    RaisePropertyChanged("FamilyList");
                }
            }
        }

        private ObservableCollection<tbl_WF_Defects> _defectsList;

        public ObservableCollection<tbl_WF_Defects> DefectsList
        {
            get
            {
                return _defectsList;
            }
            set
            {
                if ((ReferenceEquals(_defectsList, value) != true))
                {
                    _defectsList = value;
                    RaisePropertyChanged("DefectsList");
                }
            }
        }

        private ObservableCollection<InventoryReservedJournalsDetail> _inventoryResJournals;

        public ObservableCollection<InventoryReservedJournalsDetail> InventoryResJournals
        {
            get
            {
                return _inventoryResJournals;
            }
            set
            {
                if ((ReferenceEquals(_inventoryResJournals, value) != true))
                {
                    _inventoryResJournals = value;
                    RaisePropertyChanged("InventoryResJournals");
                }
            }
        }

        private IEnumerable<string> _journalList;

        public IEnumerable<string> JournalList
        {
            get
            {
                return _journalList;
            }
            set
            {
                if ((ReferenceEquals(_journalList, value) != true))
                {
                    _journalList = value;
                    RaisePropertyChanged("JournalList");
                }
            }
        }

        private ObservableCollection<CRUD_ManagerServicePurchaseOrderDto> _purchaseOrderJournalList;

        public ObservableCollection<CRUD_ManagerServicePurchaseOrderDto> PurchaseOrderJournalList
        {
            get
            {
                return _purchaseOrderJournalList;
            }
            set
            {
                if ((ReferenceEquals(_purchaseOrderJournalList, value) != true))
                {
                    _purchaseOrderJournalList = value;
                    RaisePropertyChanged("PurchaseOrderJournalList");
                }
            }
        }

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

        private TblFabricInspectionHeaderViewModel _transactionHeader;

        public TblFabricInspectionHeaderViewModel TransactionHeader
        {
            get
            {
                return _transactionHeader;
            }
            set
            {
                if ((ReferenceEquals(_transactionHeader, value) != true))
                {
                    _transactionHeader = value;
                    RaisePropertyChanged("TransactionHeader");
                }
            }
        }

        private ObservableCollection<TblFabricInspectionHeaderViewModel> _transactionHeaderList;

        public ObservableCollection<TblFabricInspectionHeaderViewModel> TransactionHeaderList
        {
            get
            {
                return _transactionHeaderList ?? (_transactionHeaderList = new ObservableCollection<TblFabricInspectionHeaderViewModel>());
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

        private ObservableCollection<TblFabricInspectionDetailViewModel> _transactionDetails;

        public ObservableCollection<TblFabricInspectionDetailViewModel> TransactionDetails
        {
            get
            {
                return _transactionDetails;
            }
            set
            {
                if ((ReferenceEquals(_transactionDetails, value) != true))
                {
                    _transactionDetails = value;
                    RaisePropertyChanged("TransactionDetails");
                }
            }
        }

        #endregion Prop

        public event EventHandler SubmitSearchAction;

        public event EventHandler PurchaseTransactionsCompleted;

        protected virtual void OnPurchaseTransactionsPopulatingCompleted()
        {
            var handler = PurchaseTransactionsCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();

        public FabricDefectsViewModel()
        {
            GetItemPermissions(PermissionItemName.FabricInspectionForm.ToString());
            WmsLocationList = new ObservableCollection<WMSLOCATION>();
            _webService.GetWmsLocationCompleted += (s, v) =>
            {
                var row = v.Result;

                if (!WmsLocationList.Contains(row))
                {
                    WmsLocationList.Add(row);
                }
            };

            DefectsList = new ObservableCollection<tbl_WF_Defects>();
            TransactionHeader = new TblFabricInspectionHeaderViewModel { TransDate = DateTime.Now };

            Types = new ObservableCollection<Transactions>();

            _webService.GetPurchaseOrderLinesCompleted += (s, f) =>
            {
                foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                {
                    if (LinesInspection.Any(x => x.LineNum == item.LineNumber && x.Fabric_Code == item.ItemId && x.ColorCode == item.FabricColor))
                    {
                        item.Quantity = item.Quantity - LinesInspection.Where(x => x.LineNum == item.LineNumber && x.Fabric_Code == item.ItemId && x.ColorCode == item.FabricColor)
                            .Sum(x => x.StoreRollQty);
                    }
                    OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                }
            };


            _webService.GetRecivedDyedOrdersCompleted += (s, f) =>
            {
                foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                {
                    if (LinesInspection.Any(x => x.LineNum == item.LineNumber && x.Fabric_Code == item.ItemId && x.ColorCode == item.FabricColor))
                    {
                        item.Quantity = item.Quantity - LinesInspection.Where(x => x.LineNum == item.LineNumber && x.Fabric_Code == item.ItemId && x.ColorCode == item.FabricColor)
                            .Sum(x => x.StoreRollQty);
                    }
                    OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                }
            };
            _webService.GetTransferInventDimLinesCompleted += (s, f) =>
            {
                foreach (var item in f.Result.Where(item => !OrderLineList.Contains(FabricInspectionMapper.MapToOrderLine(item))))
                {
                    if (LinesInspection.Any(x => x.LineNum == item.LineNumber))
                    {
                        item.Quantity = item.Quantity - LinesInspection.Where(x => x.LineNum == item.LineNumber && x.Fabric_Code == item.ItemId)
                            .Sum(x => x.StoreRollQty);
                    }
                    OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                }
            };
            DegreesList = new ObservableCollection<Degrees>();

            string firstDegree;
            string secondDegree;
            string thirdDegree;
            if (LoggedUserInfo.CurrLang == 0)
            {
                firstDegree = "اولى";
                secondDegree = "ثانيه";
                thirdDegree = "الثالثة";
            }
            else
            {
                firstDegree = "1st";
                secondDegree = "2nd";
                thirdDegree = "3rd";
            }

            DegreesList.Add(new Degrees
            {
                DegreeNum = 1,
                DegreEname = firstDegree,
            });
            DegreesList.Add(new Degrees
            {
                DegreeNum = 2,
                DegreEname = secondDegree,
            });
            DegreesList.Add(new Degrees
            {
                DegreeNum = 3,
                DegreEname = thirdDegree,
            });

            _webService.SaveInspectionOrderCompleted += (s, sv) =>
            {
                Loading = false;
                var tblFabricInspectionDetail = sv.Result.FirstOrDefault();
                if (tblFabricInspectionDetail != null)
                    TransactionHeader.Iserial = tblFabricInspectionDetail.Tbl_fabricInspectionHeader;
                TransactionDetails = new ObservableCollection<TblFabricInspectionDetailViewModel>();
                TransactionDetails.CollectionChanged += new NotifyCollectionChangedEventHandler(TransactionDetails_CollectionChanged);
                foreach (var item in sv.Result)
                {
                    var test = new TblFabricInspectionDetailViewModel();
                    GenericMapper.InjectFromObCollection(test.DetailsViewModel, item.Tbl_fabricInspectionDetailDefects);
                    TransactionDetails.Add((TblFabricInspectionDetailViewModel)test.InjectFrom(item));
                }
                MessageBox.Show("Saved");
            };

            _webService.UpdateInspectionOrderCompleted += (s, sv) => { Loading = false;
                MessageBox.Show("Saved"); };
            _webService.InsertInspectionHeaderTempCompleted += (s, sv) =>
            {
                Loading = false;
                MessageBox.Show("Saved");
            };


            _webService.DeleteFabricInspectionCompleted += (d, s) =>
                {
                    TransactionHeader = new TblFabricInspectionHeaderViewModel();
                    TransactionDetails = new ObservableCollection<TblFabricInspectionDetailViewModel>();
                    TransactionDetails.CollectionChanged += new NotifyCollectionChangedEventHandler(TransactionDetails_CollectionChanged);
                    MessageBox.Show("Deleted");
                };

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

            _webService.FabricInspectionHeaderListCompleted += (d, i) =>
            {
                Loading = false;
                foreach (var item in i.Result)
                {
                    TransactionHeaderList.Add(FabricInspectionMapper.VwMapToFabricInspectionHeader(item));
                }
            };

            TransactionDetails = new ObservableCollection<TblFabricInspectionDetailViewModel>();
            TransactionDetails.CollectionChanged += new NotifyCollectionChangedEventHandler(TransactionDetails_CollectionChanged);
            _webService.fabricInspectionDetailCompleted += (d, s) =>
                {
                    TransactionDetails = new ObservableCollection<TblFabricInspectionDetailViewModel>();
                    TransactionDetails.CollectionChanged += new NotifyCollectionChangedEventHandler(TransactionDetails_CollectionChanged);
                    foreach (var item in s.Result)
                    {
                        var test = new TblFabricInspectionDetailViewModel();
                        GenericMapper.InjectFromObCollection(test.DetailsViewModel, item.Tbl_fabricInspectionDetailDefects);
                        TransactionDetails.Add((TblFabricInspectionDetailViewModel)test.InjectFrom(item));
                    }
                };

            _webService.DefectsAsync();
            _webService.DefectsCompleted += (d, s) =>
                {
                    foreach (var item in s.Result)
                    {
                        DefectsList.Add(item);
                    }
                };

            _webService.GetPurchaseOrderJournalsCompleted += (d, p) =>
            {
                PurchaseOrderJournalList = p.Result;
                OnPurchaseTransactionsPopulatingCompleted();
            };

            _webService.InventoryReservedJournalsDetailAsync("ccm");
            _webService.InventoryReservedJournalsDetailCompleted += (a, d) =>
                {
                    JournalList = d.Result;
                };

            _webService.InventoryReservedJournalsDetailPerJournalCompleted += (a, d) =>
            {
                InventoryResJournals = d.Result;

                foreach (var item in InventoryResJournals)
                {
                    OrderLineList.Add(FabricInspectionMapper.MapToOrderLine(item));
                }

                var childWindows = new FabricDefectsLineCreationChildWindow(this);
                childWindows.Show();
            };

            _webService.PurchaseInspectionLineNumCompleted += (a, p) =>
          {
              LinesInspection = new ObservableCollection<Tbl_fabricInspectionDetail>(p.Result);

              var lines = new ObservableCollection<decimal>(p.Result.Select(x => x.LineNum));

              if (SortBy == null)
              {
                  SortBy = "it.PURCHQTY";
              }
              if (TransactionHeader.TransactionType == 0)
              {
                  _webService.GetPurchaseOrderLinesAsync(OrderLineList.Count, PageSize, "ccm", TransactionHeader.Order, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
              }
              if (TransactionHeader.TransactionType == 1)
              {
                  _webService.GetRecivedDyedOrdersAsync(OrderLineList.Count, PageSize, TransactionHeader.Order, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
              }
              else if (TransactionHeader.TransactionType == 2)
              {
                  _webService.GetTransferInventDimLinesAsync(OrderLineList.Count, PageSize, "ccm", TransactionHeader.Order, lines, "it.PURCHQTY desc", Filter, ValuesObjects);
              }
          };
        }

        private void TransactionDetails_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblFabricInspectionDetailViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblFabricInspectionDetailViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "StoreRollQty")
            {
                TransactionHeader.TotalQty = TransactionDetails.Sum(x => x.StoreRollQty);
            }
        }

        private ObservableCollection<Tbl_fabricInspectionDetail> _linesInspection;

        public ObservableCollection<Tbl_fabricInspectionDetail> LinesInspection
        {
            get { return _linesInspection; }
            set { _linesInspection = value; RaisePropertyChanged("LinesInspection"); }
        }

        public void GetOrderInfo()
        {
            switch (TransactionHeader.TransactionType)
            {
                case 0:

                    if (!string.IsNullOrWhiteSpace(TransactionHeader.Order))
                    {
                        _webService.PurchaseInspectionLineNumAsync(TransactionHeader.Order);
                    }
                    break;

                case 1:

                    if (!string.IsNullOrWhiteSpace(TransactionHeader.Order))
                    {
                        _webService.PurchaseInspectionLineNumAsync(TransactionHeader.Order);
                    }
                    break;

                case 2:

                    if (!string.IsNullOrWhiteSpace(TransactionHeader.Order))
                    {
                        _webService.PurchaseInspectionLineNumAsync(TransactionHeader.Order);
                    }
                    break;
            }
        }

        public void DeleteOrder()
        {
            _webService.DeleteFabricInspectionAsync(TransactionHeader.Iserial);
        }

        public void SearchHeader()
        {
            _webService.FabricInspectionHeaderListAsync(TransactionHeaderList.Count, PageSize, "it.Iserial", DetailFilter, DetailValuesObjects);
        }


        public void SaveOrderTemp()
        {
            var valiationCollectionHeader = new List<ValidationResult>();
            var isvalidHeader = Validator.TryValidateObject(TransactionHeader,
                new ValidationContext(TransactionHeader, null, null), valiationCollectionHeader, true);

            var details = new ObservableCollection<Tbl_fabricInspectionDetail>();
            var isvalid = false;
            foreach (var item in TransactionDetails)
            {
                var valiationCollection = new List<ValidationResult>();
                isvalid = Validator.TryValidateObject(item,
                   new ValidationContext(item, null, null), valiationCollection, true);
                if (isvalid == false)
                {
                    return;
                }
                var test = new Tbl_fabricInspectionDetail
                {
                    Tbl_fabricInspectionDetailDefects = new ObservableCollection<Tbl_fabricInspectionDetailDefects>(),
                };
                GenericMapper.InjectFromObCollection(test.Tbl_fabricInspectionDetailDefects, item.DetailsViewModel);
                details.Add((Tbl_fabricInspectionDetail)test.InjectFrom(item));
            }
            if (isvalid && isvalidHeader)
            {

                if (Loading == false)
                {
                    Loading = true;
                    _webService.InsertInspectionHeaderTempAsync(FabricInspectionMapper.DbMapToFabricInspectionHeader(TransactionHeader), details, LoggedUserInfo.Iserial);
                }
            }
            else
            {
                MessageBox.Show("Data Is NOt Valid");
            }
        }

        public void SaveOrder()
        {
            var valiationCollectionHeader = new List<ValidationResult>();
            var isvalidHeader = Validator.TryValidateObject(TransactionHeader,
                new ValidationContext(TransactionHeader, null, null), valiationCollectionHeader, true);

            var details = new ObservableCollection<Tbl_fabricInspectionDetail>();
            var isvalid = false;
            foreach (var item in TransactionDetails)
            {
                var valiationCollection = new List<ValidationResult>(); isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);
                if (isvalid == false)
                {
                    return;
                }
                var test = new Tbl_fabricInspectionDetail
                {
                    Tbl_fabricInspectionDetailDefects = new ObservableCollection<Tbl_fabricInspectionDetailDefects>(),
                };
                GenericMapper.InjectFromObCollection(test.Tbl_fabricInspectionDetailDefects, item.DetailsViewModel);
                details.Add((Tbl_fabricInspectionDetail)test.InjectFrom(item));
            }
            if (isvalid && isvalidHeader)
            {
                if (details.Sum(x => x.StoreRollQty) > details.FirstOrDefault().TotalLineNumQty)
                {
                    MessageBox.Show("Rolls Qty Exceed Po Qty");
                    return;
                }
                if (Loading == false)
                {
                    Loading = true;
                    if (TransactionHeader.Iserial==0)
                    {
                        _webService.SaveInspectionOrderAsync(FabricInspectionMapper.DbMapToFabricInspectionHeader(TransactionHeader), details, LoggedUserInfo.Iserial);
                    }
                    else
                    {
                        _webService.UpdateInspectionOrderAsync(FabricInspectionMapper.DbMapToFabricInspectionHeader(TransactionHeader), details, LoggedUserInfo.Iserial);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Data Is NOt Valid");
            }
        }

        public void GetFabricInspectionOrderDetail()
        {
            _webService.fabricInspectionDetailAsync(TransactionHeader.Iserial);
            SubmitSearchAction.Invoke(this, null);
        }

        public void DeleteInspectionLine(TblFabricInspectionDetailViewModel row)
        {
            if (row.Iserial != 0)
            {
                //_webService.DeleteInspectionRowAsync(row.Iserial);
            }
            TransactionDetails.Remove(row);
        }

        internal void CreateAxBarcode()
        {
            _webService.CreateAxBarcodeAsync(TransactionHeader.Iserial, 1, LoggedUserInfo.Iserial, TransactionHeader.TransactionType);
        }

        internal void FabricInspectionReport()
        {
            var reportName = "FabricInspection";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "FabricInspectionar"; }

            var para = new ObservableCollection<string> { TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }
    }
}