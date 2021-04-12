using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class FabricSetupsWFViewModel : ViewModelBase
    {
        #region Private Fields

        private int? _yarnStatusId;
        //private ObservableCollection<_Proxy.AccConfiguration> _accConfigList;
        //private _Proxy.AccConfiguration _accConfigProperty;
        //private ObservableCollection<_Proxy.AccSizeConfig> _accSizeConfigList;
        //private _Proxy.AccSizeConfig _accSizeConfigProperty;
        private bool _canPostToAx;
        private ObservableCollection<ContentCompositionViewModel> _contentCompositionList;
        private ObservableCollection<GenericViewModel> _contentsList;
        private GenericViewModel _dyingClassField;
        private ObservableCollection<GenericViewModel> _dyinngClassList;
        private ObservableCollection<GenericViewModel> _fabricCategoryList;
        private GenericViewModel _fabricCategoryObj;
        private string _fabricDescriptionByLang;
        private ObservableCollection<GenericViewModel> _fabricDesignsList;
        private GenericViewModel _fabricFinishesField;
        private ObservableCollection<GenericViewModel> _fabricFinishesList;
        private GenericViewModel _fabricMaterialsField;
        private ObservableCollection<GenericViewModel> _fabricMaterialsList;
        private ObservableCollection<GenericViewModel> _fabricStatus;
        private GenericViewModel _fabricStructureField;
        private ObservableCollection<GenericViewModel> _fabricStructureList;
        private GenericViewModel _fabricTypesField;
        private ObservableCollection<GenericViewModel> _fabricTypesList;
        private GenericViewModel _gaugesField;
        private GenericViewModel _yarnStatusProperty;
        private ObservableCollection<GenericViewModel> _gaugesList;
        private bool? _isPartialDetails;
        private bool? _tubularWidth;
        private ObjectStatus _objstatus;
        private GenericViewModel _statusField;
        private int? _statusId;
        private ObservableCollection<GenericViewModel> _statusList;
        private string _supplierRef;
        private GenericViewModel _threadNumberField;
        private ObservableCollection<GenericViewModel> _threadNumbersList;
        private double _totalPercentage;
        private GenericViewModel _uoMField;
        private ObservableCollection<GenericViewModel> _uoMList;
        private GenericViewModel _yarnCountField;
        private ObservableCollection<GenericViewModel> _yarnCountList;
        private GenericViewModel _yarnFinishesField;
        private ObservableCollection<GenericViewModel> _yarnFinishesList;
        private GenericViewModel _contentsField;
        private double? _dyedFabricWidthField;
        private double? _dyedFabricWidthMaxField;
        private int? _dyingClassificationIdField;
        private double? _expectedDyingLossMarginField;
        private int _fabricCategoryIdField;
        private string _fabricDescriptionArField;
        private string _fabricDescriptionField;
        private GenericViewModel _fabricDesignsField;
        private int? _fabricDesignsIdField;
        private int? _fabricFinishesIdField;
        private string _fabricIdField;
        private int? _fabricMaterialsIdField;
        private int? _fabricStructuresIdField;
        private int? _fabricTypesIdField;
        private int? _gaugesIdField;
        private double? _verticalShrinkageField;
        private double? _horizontalShrinkageField;

        private double? _verticalShrinkageMaxField;
        private double? _horizontalShrinkageMaxField;
        private ObservableCollection<GenericViewModel> _inshesList;
        private GenericViewModel _inshesProperty;
        private string _inshesValue;
        private string _notesField;
        private int? _threadNumbersIdField;
        private double? _twistField;
        private int? _uoMidField;

        private double? _weightPerSquarMeterAfterWashField;
        private double? _weightPerSquarMeterAfterWashMaxField;
        private double? _weightPerSquarMeterAsRawField;
        private double? _weightPerSquarMeterAsRawMaxField;
        private double? _weightPerSquarMeterBeforWashField;
        private double? _weightPerSquarMeterBeforWashMaxField;
        private double? _widthAsRawField;
        private double? _widthAsRawMaxField;
        private int? _yarnCountIdField;
        private int? _yarnFinishesIdField;
        private int? _yarnSourceId;
        private ObservableCollection<GenericViewModel> _yarnSourceList;
        private ObservableCollection<GenericViewModel> _yarnStatusList;

        private GenericViewModel _yarnSourceProperty;
        private DateTime? _noteUpdatedDate;
        private bool? _colored;

        #endregion Private Fields

        #region Public Properties

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        public DateTime? NoteUpdatedDate
        {
            get { return _noteUpdatedDate; }
            set { _noteUpdatedDate = value; RaisePropertyChanged("NoteUpdatedDate"); }
        }

        private PermissionItemName _formName;

        public PermissionItemName FormName
        {
            get { return _formName; }
            set { _formName = value; RaisePropertyChanged("FormName"); }
        }

        public int? YarnStatusID
        {
            get { return _yarnStatusId; }
            set { _yarnStatusId = value; RaisePropertyChanged("YarnStatusID"); }
        }

        private bool _isLoadingForDyedFlag;

        public bool IsLoadingForDyedFlag
        {
            get { return _isLoadingForDyedFlag; }
            set { _isLoadingForDyedFlag = value; }
        }

        //public ObservableCollection<_Proxy.AccConfiguration> AccConfigList
        //{
        //    get { return _accConfigList; }
        //    set { _accConfigList = value; RaisePropertyChanged("AccConfigList"); }
        //}

        //public _Proxy.AccConfiguration AccConfigProperty
        //{
        //    get { return _accConfigProperty; }
        //    set { _accConfigProperty = value; RaisePropertyChanged("AccConfigProperty"); }
        //}

        //public ObservableCollection<_Proxy.AccSizeConfig> AccSizeConfigList
        //{
        //    get { return _accSizeConfigList; }
        //    set { _accSizeConfigList = value; RaisePropertyChanged("AccSizeConfigList"); }
        //}

        //public _Proxy.AccSizeConfig AccSizeConfigProperty
        //{
        //    get { return _accSizeConfigProperty; }
        //    set { _accSizeConfigProperty = value; RaisePropertyChanged("AccSizeConfigProperty"); }
        //}

        public bool CanPostToAX
        {
            get { return _canPostToAx; }
            set { _canPostToAx = value; RaisePropertyChanged("CanPostToAX"); }
        }

        public ObservableCollection<ContentCompositionViewModel> ContentCompositionList
        {
            get { return _contentCompositionList; }
            set
            {
                _contentCompositionList = value;
                RaisePropertyChanged("ContentCompositionList");
                ManageContentsList();
                UpdateFabricDescreption();
            }
        }

        public ObservableCollection<GenericViewModel> YarnStatusList
        {
            get { return _yarnStatusList; }
            set { _yarnStatusList = value; RaisePropertyChanged("YarnStatusList"); }
        }

        public ObservableCollection<GenericViewModel> ContentsList
        {
            get { return _contentsList; }
            set { _contentsList = value; RaisePropertyChanged("ContentsList"); }
        }

        public GenericViewModel ContentsProperty
        {
            get { return _contentsField; }
            set
            {
                _contentsField = value;
                RaisePropertyChanged("ContentsProperty");
            }
        }

        public double? DyedFabricWidth
        {
            get
            {
                return _dyedFabricWidthField;
            }
            set
            {
                if ((_dyedFabricWidthField.Equals(value) != true))
                {
                    _dyedFabricWidthField = value;
                    RaisePropertyChanged("DyedFabricWidth");
                }
            }
        }

        public double? DyedFabricWidthMax
        {
            get
            {
                return _dyedFabricWidthMaxField;
            }
            set
            {
                if ((_dyedFabricWidthMaxField.Equals(value) != true))
                {
                    _dyedFabricWidthMaxField = value;
                    RaisePropertyChanged("DyedFabricWidthMax");
                }
            }
        }

        public int? DyingClassificationID
        {
            get
            {
                return _dyingClassificationIdField;
            }
            set
            {
                if ((_dyingClassificationIdField.Equals(value) != true))
                {
                    _dyingClassificationIdField = value;
                    RaisePropertyChanged("DyingClassificationID");
                }
            }
        }

        public ObservableCollection<GenericViewModel> DyingClassList
        {
            get { return _dyinngClassList; }
            set { _dyinngClassList = value; RaisePropertyChanged("DyingClassList"); }
        }

        public GenericViewModel YarnStatusProperty
        {
            get { return _yarnStatusProperty; }
            set
            {
                _yarnStatusProperty = value;
                RaisePropertyChanged("YarnStatusProperty");
                UpdateFabricDescreption();
            }
        }

        public GenericViewModel DyingClassProperty
        {
            get { return _dyingClassField; }
            set { _dyingClassField = value; RaisePropertyChanged("DyingClassProperty"); }
        }

        public double? ExpectedDyingLossMargin
        {
            get
            {
                return _expectedDyingLossMarginField;
            }
            set
            {
                if ((_expectedDyingLossMarginField.Equals(value) != true))
                {
                    _expectedDyingLossMarginField = value;
                    RaisePropertyChanged("ExpectedDyingLossMargin");
                }
            }
        }

        public int FabricCategoryID
        {
            get { return _fabricCategoryIdField; }
            set
            {
                if (_fabricCategoryIdField != value)
                {
                    _fabricCategoryIdField = value;
                    RaisePropertyChanged("FabricCategoryID");
                    UpdateFabricDescreption();
                    YarnStatusID = 1;
                    Colored = false;
                    IsPartialDetails = false;

                    if (value == 1 || value == 2)
                    {
                        UoMProperty = UoMList.FirstOrDefault(x => x.Iserial == 1);
                        FabricMaterialsProp = value == 2 ? FabricMaterialsList.FirstOrDefault(x => x.Iserial == 1) : null;
                    }
                }
            }
        }

        public ObservableCollection<GenericViewModel> FabricCategoryList
        {
            get { return _fabricCategoryList; }
            set { _fabricCategoryList = value; RaisePropertyChanged("FabricCategoryList"); }
        }

        public GenericViewModel FabricCategoryObj
        {
            get { return _fabricCategoryObj; }
            set { _fabricCategoryObj = value; RaisePropertyChanged("FabricCategoryObj"); }
        }

        public string FabricDescription
        {
            get
            {
                return _fabricDescriptionField;
            }
            set
            {
                if ((ReferenceEquals(_fabricDescriptionField, value) != true))
                {
                    _fabricDescriptionField = value;
                    RaisePropertyChanged("FabricDescription");
                }
            }
        }

        public string FabricDescriptionAR
        {
            get
            {
                return _fabricDescriptionArField;
            }
            set
            {
                if ((ReferenceEquals(_fabricDescriptionArField, value) != true))
                {
                    _fabricDescriptionArField = value;
                    RaisePropertyChanged("FabricDescriptionAR");
                }
            }
        }

        public string FabricDescriptionByLang
        {
            get { return _fabricDescriptionByLang; }
            set
            {
                if (_fabricDescriptionByLang != value)
                {
                    _fabricDescriptionByLang = value;
                    RaisePropertyChanged("FabricDescriptionByLang");
                }
            }
        }

        public int? FabricDesignsID
        {
            get
            {
                return _fabricDesignsIdField;
            }
            set
            {
                if ((_fabricDesignsIdField.Equals(value) != true))
                {
                    _fabricDesignsIdField = value;
                    RaisePropertyChanged("FabricDesignsID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> FabricDesignsList
        {
            get { return _fabricDesignsList; }
            set
            {
                _fabricDesignsList = value;
                RaisePropertyChanged("FabricDesignsList");
            }
        }

        public GenericViewModel FabricDesignsProperty
        {
            get { return _fabricDesignsField; }
            set
            {
                _fabricDesignsField = value;
                RaisePropertyChanged("FabricDesignsProperty");
                UpdateFabricDescreption();
            }
        }

        public int? FabricFinishesID
        {
            get
            {
                return _fabricFinishesIdField;
            }
            set
            {
                if ((_fabricFinishesIdField.Equals(value) != true))
                {
                    _fabricFinishesIdField = value;
                    RaisePropertyChanged("FabricFinishesID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> FabricFinishesList
        {
            get { return _fabricFinishesList; }
            set { _fabricFinishesList = value; RaisePropertyChanged("FabricFinishesList"); }
        }

        public GenericViewModel FabricFinishesProperty
        {
            get { return _fabricFinishesField; }
            set
            {
                _fabricFinishesField = value;
                RaisePropertyChanged("FabricFinishesProperty");
                UpdateFabricDescreption();
            }
        }

        public string FabricID
        {
            get
            {
                return _fabricIdField;
            }
            set
            {
                if ((ReferenceEquals(_fabricIdField, value) != true))
                {
                    _fabricIdField = value;
                    RaisePropertyChanged("FabricID");
                }
            }
        }

        public int? FabricMaterialsID
        {
            get
            {
                return _fabricMaterialsIdField;
            }
            set
            {
                if ((_fabricMaterialsIdField.Equals(value) != true))
                {
                    _fabricMaterialsIdField = value;
                    RaisePropertyChanged("FabricMaterialsID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> FabricMaterialsList
        {
            get { return _fabricMaterialsList; }
            set { _fabricMaterialsList = value; RaisePropertyChanged("FabricMaterialsList"); }
        }

        public GenericViewModel FabricMaterialsProp
        {
            get { return _fabricMaterialsField; }
            set
            {
                _fabricMaterialsField = value;
                RaisePropertyChanged("FabricMaterialsProp");
                UpdateFabricDescreption();
            }
        }

        public ObservableCollection<GenericViewModel> FabricStatus
        {
            get { return _fabricStatus; }
            set { _fabricStatus = value; RaisePropertyChanged("FabricStatus"); }
        }

        public ObservableCollection<GenericViewModel> FabricStructureList
        {
            get { return _fabricStructureList; }
            set { _fabricStructureList = value; RaisePropertyChanged("FabricStructureList"); }
        }

        public GenericViewModel FabricStructureProperty
        {
            get { return _fabricStructureField; }
            set
            {
                _fabricStructureField = value;
                RaisePropertyChanged("FabricStructureProperty");
                UpdateFabricDescreption();
            }
        }

        public int? FabricStructuresID
        {
            get
            {
                return _fabricStructuresIdField;
            }
            set
            {
                if ((_fabricStructuresIdField.Equals(value) != true))
                {
                    _fabricStructuresIdField = value;
                    RaisePropertyChanged("FabricStructuresID");
                    FabricStructureProperty = FabricStructureList.FirstOrDefault(x => x.Iserial == value);
                    UpdateFabricDescreption();
                }
            }
        }

        public int? FabricTypesID
        {
            get
            {
                return _fabricTypesIdField;
            }
            set
            {
                if ((_fabricTypesIdField.Equals(value) != true))
                {
                    _fabricTypesIdField = value;
                    RaisePropertyChanged("FabricTypesID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> FabricTypesList
        {
            get { return _fabricTypesList; }
            set { _fabricTypesList = value; RaisePropertyChanged("FabricTypesList"); }
        }

        public GenericViewModel FabricTypesProperty
        {
            get { return _fabricTypesField; }
            set
            {
                _fabricTypesField = value;
                RaisePropertyChanged("FabricTypesProperty");
                UpdateFabricDescreption();
            }
        }

        public int? GaugesID
        {
            get
            {
                return _gaugesIdField;
            }
            set
            {
                if ((_gaugesIdField.Equals(value) != true))
                {
                    _gaugesIdField = value;
                    RaisePropertyChanged("GaugesID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> GaugesList
        {
            get { return _gaugesList; }
            set { _gaugesList = value; RaisePropertyChanged("GaugesList"); }
        }

        public GenericViewModel GaugesProperty
        {
            get { return _gaugesField; }
            set
            {
                _gaugesField = value;
                RaisePropertyChanged("GaugesProperty");
                UpdateFabricDescreption();
            }
        }

        public double? HorizontalShrinkage
        {
            get
            {
                return _horizontalShrinkageField;
            }
            set
            {
                if ((_horizontalShrinkageField.Equals(value) != true))
                {
                    _horizontalShrinkageField = value;
                    RaisePropertyChanged("HorizontalShrinkage");
                }
            }
        }

        public double? HorizontalShrinkageMax
        {
            get
            {
                return _horizontalShrinkageMaxField;
            }
            set
            {
                if ((_horizontalShrinkageMaxField.Equals(value) != true))
                {
                    _horizontalShrinkageMaxField = value;
                    RaisePropertyChanged("HorizontalShrinkageMax");
                }
            }
        }

        public double? VerticalShrinkageMax
        {
            get
            {
                return _verticalShrinkageMaxField;
            }
            set
            {
                if ((_verticalShrinkageMaxField.Equals(value) != true))
                {
                    _verticalShrinkageMaxField = value;
                    RaisePropertyChanged("VerticalShrinkageMax");
                }
            }
        }

        public ObservableCollection<GenericViewModel> InshesList
        {
            get { return _inshesList; }
            set { _inshesList = value; RaisePropertyChanged("InshesList"); }
        }

        public GenericViewModel InshesProperty
        {
            get { return _inshesProperty; }
            set { _inshesProperty = value; RaisePropertyChanged("InshesProperty"); }
        }

        public string InshesValue
        {
            get { return _inshesValue; }
            set { _inshesValue = value; RaisePropertyChanged("InshesValue"); }
        }

        public bool? IsPartialDetails
        {
            get { return _isPartialDetails; }
            set
            {
                _isPartialDetails = value;
                if (_isPartialDetails == true)
                {
                    DyingClassProperty = DyingClassList.FirstOrDefault(x => x.Iserial == 3);
                }
                RaisePropertyChanged("IsPartialDetails");
            }
        }

        public bool? Colored
        {
            get { return _colored; }
            set { _colored = value; RaisePropertyChanged("Colored"); }
        }

        public bool? TubularWidth
        {
            get { return _tubularWidth; }
            set { _tubularWidth = value; RaisePropertyChanged("TubularWidth"); }
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

        public ObjectStatus ObjStatus
        {
            get { return _objstatus; }
            set { _objstatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        public int? StatusID
        {
            get { return _statusId; }
            set
            {
                _statusId = value;
                RaisePropertyChanged("StatusID");
                StatusProperty = StatusList.SingleOrDefault(x => x.Iserial == value);
            }
        }

        public ObservableCollection<GenericViewModel> StatusList
        {
            get { return _statusList; }
            set { _statusList = value; RaisePropertyChanged("StatusList"); }
        }

        public GenericViewModel StatusProperty
        {
            get { return _statusField; }
            set
            {
                _statusField = value;
                RaisePropertyChanged("StatusProperty");
            }
        }

        public string SupplierRef
        {
            get { return _supplierRef; }
            set { _supplierRef = value; RaisePropertyChanged("SupplierRef"); }
        }

        public GenericViewModel ThreadNumberProperty
        {
            get { return _threadNumberField; }
            set
            {
                _threadNumberField = value;
                RaisePropertyChanged("ThreadNumberProperty");
                UpdateFabricDescreption();
            }
        }

        public int? ThreadNumbersID
        {
            get
            {
                return _threadNumbersIdField;
            }
            set
            {
                if ((_threadNumbersIdField.Equals(value) != true))
                {
                    _threadNumbersIdField = value;
                    RaisePropertyChanged("ThreadNumbersID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> ThreadNumbersList
        {
            get { return _threadNumbersList; }
            set
            {
                _threadNumbersList = value;
                RaisePropertyChanged("ThreadNumbersList");
            }
        }

        public double TotalPercentage
        {
            get { return _totalPercentage; }
            set
            {
                _totalPercentage = value;
                RaisePropertyChanged("TotalPercentage");
                UpdateFabricDescreption();
            }
        }

        public double? Twist
        {
            get
            {
                return _twistField;
            }
            set
            {
                if ((_twistField.Equals(value) != true))
                {
                    _twistField = value;
                    RaisePropertyChanged("Twist");
                }
            }
        }

        public int? UoMID
        {
            get
            {
                return _uoMidField;
            }
            set
            {
                if ((_uoMidField.Equals(value) != true))
                {
                    _uoMidField = value;
                    RaisePropertyChanged("UoMID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> UoMList
        {
            get { return _uoMList; }
            set { _uoMList = value; RaisePropertyChanged("UoMList"); }
        }

        public GenericViewModel UoMProperty
        {
            get { return _uoMField; }
            set
            {
                _uoMField = value;
                RaisePropertyChanged("UoMProperty");
                UpdateFabricDescreption();
            }
        }

        public double? VerticalShrinkage
        {
            get
            {
                return _verticalShrinkageField;
            }
            set
            {
                if ((_verticalShrinkageField.Equals(value) != true))
                {
                    _verticalShrinkageField = value;
                    RaisePropertyChanged("VerticalShrinkage");
                }
            }
        }

        public double? WeightPerSquarMeterAfterWash
        {
            get
            {
                return _weightPerSquarMeterAfterWashField;
            }
            set
            {
                if ((_weightPerSquarMeterAfterWashField.Equals(value) != true))
                {
                    _weightPerSquarMeterAfterWashField = value;
                    RaisePropertyChanged("WeightPerSquarMeterAfterWash");
                }
            }
        }

        public double? WeightPerSquarMeterAfterWashMax
        {
            get
            {
                return _weightPerSquarMeterAfterWashMaxField;
            }
            set
            {
                if ((_weightPerSquarMeterAfterWashMaxField.Equals(value) != true))
                {
                    _weightPerSquarMeterAfterWashMaxField = value;
                    RaisePropertyChanged("WeightPerSquarMeterAfterWashMax");
                }
            }
        }

        public double? WeightPerSquarMeterAsRaw
        {
            get
            {
                return _weightPerSquarMeterAsRawField;
            }
            set
            {
                if ((_weightPerSquarMeterAsRawField.Equals(value) != true))
                {
                    _weightPerSquarMeterAsRawField = value;
                    RaisePropertyChanged("WeightPerSquarMeterAsRaw");
                }
            }
        }

        public double? WeightPerSquarMeterAsRawMax
        {
            get
            {
                return _weightPerSquarMeterAsRawMaxField;
            }
            set
            {
                if ((_weightPerSquarMeterAsRawMaxField.Equals(value) != true))
                {
                    _weightPerSquarMeterAsRawMaxField = value;
                    RaisePropertyChanged("WeightPerSquarMeterAsRawMax");
                }
            }
        }

        public double? WeightPerSquarMeterBeforWash
        {
            get
            {
                return _weightPerSquarMeterBeforWashField;
            }
            set
            {
                if ((_weightPerSquarMeterBeforWashField.Equals(value) != true))
                {
                    _weightPerSquarMeterBeforWashField = value;
                    RaisePropertyChanged("WeightPerSquarMeterBeforWash");
                }
            }
        }

        public double? WeightPerSquarMeterBeforWashMax
        {
            get
            {
                return _weightPerSquarMeterBeforWashMaxField;
            }
            set
            {
                if ((_weightPerSquarMeterBeforWashMaxField.Equals(value) != true))
                {
                    _weightPerSquarMeterBeforWashMaxField = value;
                    RaisePropertyChanged("WeightPerSquarMeterBeforWashMax");
                }
            }
        }

        public double? WidthAsRaw
        {
            get
            {
                return _widthAsRawField;
            }
            set
            {
                if ((_widthAsRawField.Equals(value) != true))
                {
                    _widthAsRawField = value;
                    RaisePropertyChanged("WidthAsRaw");
                }
            }
        }

        public double? WidthAsRawMax
        {
            get
            {
                return _widthAsRawMaxField;
            }
            set
            {
                if ((_widthAsRawMaxField.Equals(value) != true))
                {
                    _widthAsRawMaxField = value;
                    RaisePropertyChanged("WidthAsRawMax");
                }
            }
        }

        public int? YarnCountID
        {
            get
            {
                return _yarnCountIdField;
            }
            set
            {
                if ((_yarnCountIdField.Equals(value) != true))
                {
                    _yarnCountIdField = value;
                    RaisePropertyChanged("YarnCountID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> YarnCountList
        {
            get { return _yarnCountList; }
            set { _yarnCountList = value; RaisePropertyChanged("YarnCountList"); }
        }

        public GenericViewModel YarnCountProperty
        {
            get { return _yarnCountField; }
            set
            {
                _yarnCountField = value;
                RaisePropertyChanged("YarnCountProperty");
                UpdateFabricDescreption();
            }
        }

        public int? YarnFinishesID
        {
            get
            {
                return _yarnFinishesIdField;
            }
            set
            {
                if ((_yarnFinishesIdField.Equals(value) != true))
                {
                    _yarnFinishesIdField = value;
                    RaisePropertyChanged("YarnFinishesID");
                    UpdateFabricDescreption();
                }
            }
        }

        public ObservableCollection<GenericViewModel> YarnFinishesList
        {
            get { return _yarnFinishesList; }
            set { _yarnFinishesList = value; RaisePropertyChanged("YarnFinishesList"); }
        }

        public GenericViewModel YarnFinishesProperty
        {
            get { return _yarnFinishesField; }
            set
            {
                _yarnFinishesField = value;
                RaisePropertyChanged("YarnFinishesProperty");
                UpdateFabricDescreption();
            }
        }

        public int? YarnSourceID
        {
            get { return _yarnSourceId; }
            set { _yarnSourceId = value; RaisePropertyChanged("YarnSourceID"); }
        }

        public ObservableCollection<GenericViewModel> YarnSourceList
        {
            get { return _yarnSourceList; }
            set { _yarnSourceList = value; RaisePropertyChanged("YarnSourceList"); }
        }

        public GenericViewModel YarnSourcePropert
        {
            get { return _yarnSourceProperty; }
            set
            {
                _yarnSourceProperty = value;
                RaisePropertyChanged("YarnSourcePropert");
                UpdateFabricDescreption();
            }
        }

        #endregion Public Properties

        #region [ Constructors ]

        public FabricSetupsWFViewModel()
        {
            InitializeObject();
            ObjStatus = new ObjectStatus
            {
                IsNew = true,
                IsSavedDBItem = false,
                IsChanged = false,
                IsMarkedForDeletion = false,
                IsReadyForSaving = false,
                IsLoading = false
            };
            CanPostToAX = false;
        }

        public FabricSetupsWFViewModel(string fabAttrCode, int fabCategoryId, bool isLoadingForDyed)
        {
            InitializeObject();
            ObjStatus = new ObjectStatus
            {
                IsNew = isLoadingForDyed,
                IsSavedDBItem = !isLoadingForDyed,
                IsChanged = false,
                IsMarkedForDeletion = false,
                IsReadyForSaving = false,
                IsLoading = true
            };

            Client.GetFabAttributesCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    if (e.Result == null) return;
                    FabricSetupsWFViewModelMapper.MapToViewModelObject(this, e.Result);
                    CanPostToAX = StatusID != 2;
                    FabricCategoryID =
                        isLoadingForDyed ?
                            (FabricCategoryID == 3 ? 5
                                : FabricCategoryID == 2 ? 4 : FabricCategoryID)
                            : FabricCategoryID;
                    FabricID = FabricID;

                    Client.GetWFFabContentCompositionsCompleted += (s1, e1) =>
                      {
                          foreach (var item in e1.Result)
                          {
                              ContentCompositionList
                                  .Add(FabricSetupsWFViewModelMapper.MapToViewModelObject(item));
                          }

                          TotalPercentage = ContentCompositionList.Sum(x => x.ContentPercentage);

                          ObjStatus.IsLoading = false;
                          IsLoadingForDyedFlag = isLoadingForDyed;
                          UpdateFabricDescreption();
                      };
                    Client.GetWFFabContentCompositionsAsync(fabAttrCode, fabCategoryId);
                }
                else
                {
                    throw e.Error;
                }
            };
            Client.GetFabAttributesAsync(fabAttrCode, fabCategoryId);
        }

        #endregion [ Constructors ]

        #region [ Private Methods ]

        private void CheckObjectForSaving()
        {
            switch (FabricCategoryID)
            {
                case 1:
                    CheckYarnDataForSaving();
                    break;

                case 2:
                    CheckRawFabricDataForSaving();
                    break;

                case 3:
                    CheckFabricDataForSaving();
                    break;

                case 4:
                case 5:
                    CheckDyingFabricDataForSaving();
                    break;

                default:
                    ObjStatus.IsReadyForSaving = false;
                    break;
            }
        }

        private void CheckDyingFabricDataForSaving()
        {
            ObjStatus.IsReadyForSaving
                =
                (
                    FabricCategoryID != 0
                    &&
                    FabricFinishesID != null
                    &&
                    FabricDesignsID != null
                    &&
                    !string.IsNullOrEmpty(FabricDescription.Trim())
                    &&
                   (WeightPerSquarMeterAfterWash != null || UoMID == 1)
                    &&
                   (WeightPerSquarMeterBeforWash != null || UoMID == 1)
                    &&
                    DyedFabricWidth != null
                    &&
                    HorizontalShrinkage != null
                    &&
                    VerticalShrinkage != null
                    &&
                       HorizontalShrinkageMax != null
                    &&
                    VerticalShrinkageMax != null
                    &&
                    Twist != null
                    );
        }

        private void CheckFabricDataForSaving()
        {
            if (IsPartialDetails == false || IsPartialDetails == null)
            {
                ObjStatus.IsReadyForSaving
                    =
                    (
                        FabricCategoryID != 0
                        &&
                        FabricMaterialsID != null
                        &&
                        FabricStructuresID != null
                        &&
                        ContentCompositionList.Count > 0
                        &&
                        ContentCompositionList.Sum(x => x.ContentPercentage) == 100
                        &&
                        UoMID != null
                        &&
                        !string.IsNullOrEmpty(FabricDescription.Trim())
                        &&
                        WidthAsRaw != null
                        &&
                        FabricDesignsID != null
                        &&
                        ExpectedDyingLossMargin != null
                        &&
                        (WeightPerSquarMeterAsRaw != null || UoMID == 1)
                        &&
                         (WeightPerSquarMeterAsRawMax != null || UoMID == 1)
                       &&
                    //   && should check if only m
                        !string.IsNullOrEmpty(SupplierRef)
                        );
            }
            else
            {
                ObjStatus.IsReadyForSaving = (
                       FabricCategoryID != 0
                        &&
                        FabricMaterialsID != null
                        &&
                        FabricStructuresID != null
                        &&
                        ContentCompositionList.Count > 0
                        &&
                        ContentCompositionList.Sum(x => x.ContentPercentage) == 100
                        &&
                        UoMID != null
                        &&
                    !string.IsNullOrEmpty(SupplierRef)
                    );
            }
        }

        private void CheckRawFabricDataForSaving()
        {
            ObjStatus.IsReadyForSaving
                =
                (
                    FabricCategoryID != 0
                    &&
                    FabricMaterialsID != null
                    &&
                    FabricStructuresID != null
                    &&
                    ContentCompositionList.Count > 0
                    &&
                    ContentCompositionList.Sum(x => x.ContentPercentage) == 100
                    &&
                    YarnCountID != null
                    &&
                    ThreadNumbersID != null
                    &&
                    UoMID != null
                    &&
                    !string.IsNullOrEmpty(FabricDescription.Trim())
                    &&
                    WidthAsRaw != null
                    &&
                      WidthAsRawMax != null
                    &&
                    ExpectedDyingLossMargin != null
                    &&

                    WeightPerSquarMeterAsRaw != null
                     &&
                   WeightPerSquarMeterAsRawMax != null
                   &&
                   DyingClassificationID != null
                    );
        }

        private void CheckYarnDataForSaving()
        {
            ObjStatus.IsReadyForSaving
                =
                (
                    FabricCategoryID != 0
                    &&
                    ContentCompositionList.Count > 0
                    &&
                    ContentCompositionList.Sum(x => x.ContentPercentage) == 100
                    &&
                    YarnCountID != null
                    &&
                    ThreadNumbersID != null
                    &&
                    UoMID != null
                    &&
                    YarnFinishesID != null
                    &&
                    !string.IsNullOrEmpty(FabricDescription.Trim())
                    && YarnSourceID != null
                   );
        }

        private static void FillGenericCollection(string tablEname, ObservableCollection<GenericViewModel> objectToFill)
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            client.GetGenericCompleted += (s, ev) =>
            {
                var i = 0;
                foreach (var item in ev.Result)
                {
                    objectToFill.Add(new GenericViewModel()
                    {
                        Iserial = item.Iserial,
                        Code = item.Code,
                        Aname = item.Aname,
                        Ename = item.Ename
                    });
                    objectToFill[i].Status.IsChanged = false;
                    objectToFill[i].Status.IsNew = false;
                    objectToFill[i].Status.IsSavedDBItem = true;
                    i++;
                }
            };
            client.CloseAsync();
        }

        private void InitializeCollections()
        {
            FabricTypesList = new ObservableCollection<GenericViewModel>();
            FabricTypesList.CollectionChanged += GenericViewModel_CollectionChanged;

            InshesList = new ObservableCollection<GenericViewModel>();
            InshesList.CollectionChanged += GenericViewModel_CollectionChanged;

            FabricCategoryList = new ObservableCollection<GenericViewModel>();
            FabricCategoryList.CollectionChanged += GenericViewModel_CollectionChanged;

            ContentsList = new ObservableCollection<GenericViewModel>();
            ContentsList.CollectionChanged += GenericViewModel_CollectionChanged;

            FabricDesignsList = new ObservableCollection<GenericViewModel>();
            FabricDesignsList.CollectionChanged += GenericViewModel_CollectionChanged;

            FabricFinishesList = new ObservableCollection<GenericViewModel>();
            FabricFinishesList.CollectionChanged += GenericViewModel_CollectionChanged;

            FabricMaterialsList = new ObservableCollection<GenericViewModel>();
            FabricMaterialsList.CollectionChanged += GenericViewModel_CollectionChanged;

            FabricStructureList = new ObservableCollection<GenericViewModel>();
            FabricStructureList.CollectionChanged += GenericViewModel_CollectionChanged;

            GaugesList = new ObservableCollection<GenericViewModel>();
            GaugesList.CollectionChanged += GenericViewModel_CollectionChanged;

            StatusList = new ObservableCollection<GenericViewModel>();
            StatusList.CollectionChanged += GenericViewModel_CollectionChanged;

            ThreadNumbersList = new ObservableCollection<GenericViewModel>();
            ThreadNumbersList.CollectionChanged += GenericViewModel_CollectionChanged;

            UoMList = new ObservableCollection<GenericViewModel>();
            UoMList.CollectionChanged += GenericViewModel_CollectionChanged;

            YarnCountList = new ObservableCollection<GenericViewModel>();
            YarnCountList.CollectionChanged += GenericViewModel_CollectionChanged;

            YarnFinishesList = new ObservableCollection<GenericViewModel>();
            YarnFinishesList.CollectionChanged += GenericViewModel_CollectionChanged;

            ContentCompositionList = new ObservableCollection<ContentCompositionViewModel>();
            ContentCompositionList.CollectionChanged += ContentCompositionList_CollectionChanged;

            DyingClassList = new ObservableCollection<GenericViewModel>();
            DyingClassList.CollectionChanged += GenericViewModel_CollectionChanged;

            YarnSourceList = new ObservableCollection<GenericViewModel>();
            YarnSourceList.CollectionChanged += GenericViewModel_CollectionChanged;

            YarnStatusList = new ObservableCollection<GenericViewModel>();
            YarnStatusList.CollectionChanged += GenericViewModel_CollectionChanged;
        }

        private void InitializeObject()
        {
            FormName = PermissionItemName.FabSetupForm;
            InitiatePermissionsMapper();
            ManagePermissions();
            ManageCustomePermissions();

            IsPartialDetails = false;
            TubularWidth = false;
            InitializeCollections();
            FillGenericCollection("tbl_FabricCategories", FabricCategoryList);
            FillGenericCollection("tbl_lkp_FabricTypes", FabricTypesList);
            FillGenericCollection("tbl_lkp_FabricDesignes", FabricDesignsList);
            FillGenericCollection("tbl_lkp_FabricFinish", FabricFinishesList);
            FillGenericCollection("tbl_lkp_FabricMaterials", FabricMaterialsList);
            FillGenericCollection("tbl_lkp_FabricStructure", FabricStructureList);
            FillGenericCollection("tbl_lkp_Gauges", GaugesList);
            FillGenericCollection("tbl_lkp_ThreadNumbers", ThreadNumbersList);
            FillGenericCollection("tbl_lkp_UoM", UoMList);
            FillGenericCollection("tbl_lkp_YarnCount", YarnCountList);
            FillGenericCollection("tbl_lkp_YarnFinish", YarnFinishesList);
            FillGenericCollection("tbl_lkp_DyingClassification", DyingClassList);
            FillGenericCollection("tbl_lkp_Status", StatusList);
            FillGenericCollection("tbl_lkp_YarnSource", YarnSourceList);
            FillGenericCollection("tbl_lkp_Inch", InshesList);
            FillGenericCollection("tbl_lkp_YarnStatus", YarnStatusList);

            Client.InsertFabItemCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    var err = new ErrorWindow("There was an error Sending item to AX", e.Error.Message.ToLower().Replace(" (inventtable). ", ".\n").Trim());
                    err.Show();
                }
                else
                {
                    MessageBox.Show("Item successfully Posted to ax!", "Fabric Setups Tool", MessageBoxButton.OK);
                    StatusID = 2;
                    CanPostToAX = false;
                }
            };
        }

        private void ManageContentsList()
        {
            ContentCompositionViewModel.SelectedContents = new ObservableCollection<int>();
            foreach (var item in ContentCompositionList.Select(x => x.FabContentID))
            {
                ContentCompositionViewModel.SelectedContents.Add(item);
            }

            foreach (var item in ContentCompositionList)
            {
                foreach (var subItem in ContentCompositionViewModel.SelectedContents)
                {
                    if (item.FabContentID != subItem)
                    {
                        item.ContentsList
                        .Remove((
                                    item.ContentsList.SingleOrDefault(x => x.Iserial == subItem)
                               ));
                    }
                }
            }
        }

        private void UpdateFabricDescreption()
        {
            #region if full details

            try
            {
                if (ObjStatus != null && !ObjStatus.IsLoading && FabricCategoryID != 0)
                {
                    var temp = "";
                    var tempAr = "";
                    switch (FabricCategoryID)
                    {
                        case 2:
                        case 3:

                            foreach (var item in ContentCompositionList)
                            {
                                temp += (item.ContentPercentage != 0 && item.FabContentID != 0)
                                    ? item.ContentPercentage + "% "
                                    + item.FabContent.Ename + " " : "";

                                tempAr += (item.ContentPercentage != 0 && item.FabContentID != 0)
                                    ? item.ContentPercentage + "% "
                                    + item.FabContent.Aname + " " : "";
                            }

                            FabricDescription =
                                (FabricStructuresID != null ? FabricStructureProperty.Code : "xxx")
                                + " "
                                + temp
                                + " "
                                + (YarnCountID != null ? YarnCountProperty.Ename : "0")
                                + "/"
                                + (ThreadNumbersID != null ? ThreadNumberProperty.Ename : "0")
                                + "-"
                                + (GaugesID != null ? GaugesProperty.Ename : "0");

                            FabricDescriptionAR =
                                (FabricStructuresID != null ? FabricStructureProperty.Aname : "xxx")
                                + " "
                                + tempAr
                                + " "
                                + (YarnCountID != null ? YarnCountProperty.Aname : "0")
                                + "/"
                                + (ThreadNumbersID != null ? ThreadNumberProperty.Aname : "0")
                                + "-"
                                + (GaugesID != null ? GaugesProperty.Aname : "0");
                            if (FabricCategoryID == 3)
                            {
                                FabricDescription = FabricDescription + "-"
                           + (FabricDesignsID != null ? FabricDesignsProperty.Ename : "0");

                                FabricDescriptionAR = FabricDescriptionAR + "-"
                           + (FabricDesignsID != null ? FabricDesignsProperty.Aname : "0");
                            }

                            break;

                        case 4:
                        case 5:

                            if (IsLoadingForDyedFlag)
                            {
                                var tempDescription = CultureInfo.CurrentCulture.Name.ToLower().Contains("en") ? FabricDescription + " Dyed" : FabricDescriptionAR + "مصبوغ ";

                                if (FabricDescriptionByLang != tempDescription)
                                {
                                    FabricDescriptionByLang = CultureInfo.CurrentCulture.Name.ToLower().Contains("en") ? FabricDescription + " Dyed" : FabricDescriptionAR + "مصبوغ ";
                                }
                            }
                            break;

                        case 1:
                            temp = "";
                            tempAr = "";
                            foreach (var item in ContentCompositionList)
                            {
                                temp += (item.ContentPercentage != 0 && item.FabContentID != 0)
                                    ? item.ContentPercentage.ToString() + "% "
                                    + item.FabContent.Ename + " " : "";

                                tempAr += (item.ContentPercentage != 0 && item.FabContentID != 0)
                                    ? item.ContentPercentage.ToString() + "% "
                                    + item.FabContent.Aname + " " : "";
                            }
                            string chanetString = "";
                            //_YarnStatusID
                            if (YarnStatusID != null && YarnStatusID != 0)
                            {
                                chanetString = chanetString + YarnStatusProperty.Code;
                            }
                            else
                            {
                                chanetString = "";
                            }
                            FabricDescription =
                                (YarnFinishesID != null ? YarnFinishesProperty.Code : "")
                                + " "
                                + temp
                                + " "
                                + (YarnCountID != null ? YarnCountProperty.Code : "0")
                                + "/"
                                + (ThreadNumbersID != null ? ThreadNumberProperty.Code : "0")
                                + (YarnSourceID != null ? YarnSourcePropert.Code : "0")
                             + chanetString;

                            FabricDescriptionAR =
                                (YarnFinishesID != null ? YarnFinishesProperty.Code : "")
                                + " "
                                + temp
                                + " "
                                + (YarnCountID != null ? YarnCountProperty.Code : "0")
                                + "/"
                                + (ThreadNumbersID != null ? ThreadNumberProperty.Code : "0")
                                  + (YarnSourceID != null ? YarnSourcePropert.Code : "0")
                                  + chanetString;

                            break;
                    }
                    if (!IsLoadingForDyedFlag)
                    {
                        FabricDescriptionByLang = CultureInfo.CurrentCulture.Name.ToLower().Contains("en") ? FabricDescription : FabricDescriptionAR;
                    }
                }
            }
            catch
            {
            }

            #endregion if full details
        }

        #endregion [ Private Methods ]

        #region [ Public Methods ]

        public void DeleteFabricAttr()
        {
            Client.DeleteFabAttributesCompleted += (s, e) =>
            {
            };
            Client.DeleteFabAttributesAsync(FabricSetupsWFViewModelMapper.MapToModelObject(this), LoggedUserInfo.Iserial);
        }

        public void InsertFabricAttr()
        {
            CheckObjectForSaving();
            if (ObjStatus.IsReadyForSaving)
            {
                //var _client = new _Proxy.CRUD_ManagerServiceClient();

                Client.AddFabAttributesCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        FabricID = e.Result;
                        var temp = new ObservableCollection<_Proxy.WF_FabricContentsComposition>();
                        foreach (var item in ContentCompositionList)
                        {
                            temp.Add
                                (FabricSetupsWFViewModelMapper.MapToModelObject
                                (item, FabricCategoryID, FabricID));
                        }
                        Client.AddFabWFCompositionsCompleted += (s2, e2) =>
                        {
                            MessageBox.Show("Data is saved successfully!",
                                "Fabric Generation Tool", MessageBoxButton.OK);
                            var customePermissionsMapper = PermissionsMapper.SingleOrDefault(x => x.PermissionKey == "FAutoPostToAX");
                            if (customePermissionsMapper != null && customePermissionsMapper.PermissionValue)
                            {
                                InserToAx();
                                //MessageBox.Show("Auto AX Posting Is Enabled");
                            }
                        };
                        Client.AddFabWFCompositionsAsync(temp);
                    }
                    else if (e.Error.Message.Contains("FS-SIEX-2"))
                    {
                        var res =
                        MessageBox.Show(strings.SameFabricConfirmationMessage, strings.Save, MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            // call the same method again, but with the option "Override check" set to true
                            Client.AddFabAttributesAsync(FabricSetupsWFViewModelMapper.MapToModelObject(this), FabricCategoryID, true);
                        }
                        else
                        {
                            var err = new ErrorWindow(e.Error);
                            err.Show();
                        }
                    }
                    else
                    {
                        var err = new ErrorWindow(e.Error);
                        err.Show();
                    }
                };
                if (FabricCategoryID == 5 || FabricCategoryID == 4)
                {
                    FabricDescription = FabricDescription + "Dyed";
                    FabricDescriptionAR = FabricDescriptionAR + "مصبوغ ";
                }

                Client.AddFabAttributesAsync(FabricSetupsWFViewModelMapper.MapToModelObject(this), FabricCategoryID, true);
            }
            else
            {
                MessageBox.Show("Data is not valid for saving, please revise!", "Fabric Generation Tool", MessageBoxButton.OK);
            }
        }

        public void InserToAx()
        {
            //_Proxy.CRUD_ManagerServiceClient _client = new _Proxy.CRUD_ManagerServiceClient();

            var fabCategoryTemp =
                FabricCategoryID == 3 ? "FINISHED"
                :
                FabricCategoryID == 2 ? "Knited"
                :
                FabricCategoryID == 4 ? "DyingKnited"
                :
                FabricCategoryID == 5 ? "DyingWoven"
                :
                "YARN";
            Client
                .InsertFabItemAsync
                (FabricSetupsWFViewModelMapper
                .MapToModelObject(this), fabCategoryTemp,
                FabricMaterialsProp != null ? FabricMaterialsProp.Ename : null, LoggedUserInfo.Iserial);
            //Client.CloseAsync();
        }

        public void UpdateFabricAttr()
        {
            CheckObjectForSaving();
            if (ObjStatus.IsReadyForSaving)
            {
                Client.UpdateWFFabAttributesCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        var inventTransExists = e.Result;
                        if (!inventTransExists)
                        {
                            var temp = new ObservableCollection<_Proxy.WF_FabricContentsComposition>();
                            foreach (var item in ContentCompositionList)
                            {
                                temp.Add
                                    (FabricSetupsWFViewModelMapper.MapToModelObject
                                    (item, FabricCategoryID, FabricID));
                            }
                            Client.AddFabWFCompositionsCompleted += (s2, e2) => MessageBox.Show("Item successfully updated!");
                            Client.AddFabWFCompositionsAsync(temp);
                        }
                        else
                        {
                            MessageBox.Show("Transactions Exisits on that item Only Notes Can be Updated");
                        }
                    }
                    else
                    {
                        throw e.Error;
                    }
                };
                Client.UpdateWFFabAttributesAsync(FabricSetupsWFViewModelMapper.MapToModelObject(this), LoggedUserInfo.Iserial);
            }
            else
            {
                MessageBox.Show("Data is not valid for saving, please revise!", "Fabric Generation Tool", MessageBoxButton.OK);
            }
        }

        #endregion [ Public Methods ]

        #region [ Event Handlers ]

        private void ContentCompositionList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ContentCompositionViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (ContentCompositionViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void GenericViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (GenericViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (GenericViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "ContentPercentage")
            {
                TotalPercentage = ContentCompositionList.Sum(x => x.ContentPercentage);
            }
            if (e.PropertyName == "FabContentID")
            {
                ManageContentsList();
            }
        }

        #endregion [ Event Handlers ]

        #region [ Permissions Handlers ]

        private void ManagePermissions()
        {
            GetItemPermissions(FormName.ToString());
        }

        private void ManageCustomePermissions()
        {
            GetCustomePermissions(FormName.ToString());
        }

        public override void InitiatePermissionsMapper()
        {
            base.InitiatePermissionsMapper();
            PermissionsMapper.Add(new CustomePermissionsMapper()
            {
                PermissionKey = "FAutoPostToAX",
                PermissionValue = false
            });
        }

        #endregion [ Permissions Handlers ]
    }
}