using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.PrintPreviews;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.Helpers.Utilities;
using System.IO;

namespace CCWFM.ViewModel.OGViewModels
{
    #region Models

    public class TblStyleTNAHeaderViewModel : PropertiesViewModelBase
    {
        int TblStyleTnaStatusField;
        public int TblStyleTNAStatus
        {
            get
            {
                return TblStyleTnaStatusField;
            }
            set
            {
                if ((this.TblStyleTnaStatusField.Equals(value) != true))
                {
                    this.TblStyleTnaStatusField = value;
                    this.RaisePropertyChanged("TblStyleTNAStatus");
                }
            }
        }
        // CCWFM.ProductionService.TblStyleTNAStatusDetail
        private ObservableCollection<TblStyleTNAStatusDetailModel> _StyleTNAStatusDetailList;

        public ObservableCollection<TblStyleTNAStatusDetailModel> StyleTNAStatusDetailList
        {
            get { return _StyleTNAStatusDetailList ?? (_StyleTNAStatusDetailList = new ObservableCollection<TblStyleTNAStatusDetailModel>()); }
            set { _StyleTNAStatusDetailList = value; RaisePropertyChanged("StyleTNAStatusDetailList"); }
        }
        private ObservableCollection<TblStyleTNAColorDetailModel> _styleTNAColorDetailList;

        public ObservableCollection<TblStyleTNAColorDetailModel> StyleTNAColorDetailList
        {
            get { return _styleTNAColorDetailList ?? (_styleTNAColorDetailList = new ObservableCollection<TblStyleTNAColorDetailModel>()); }
            set { _styleTNAColorDetailList = value; RaisePropertyChanged("StyleTNAColorDetailList"); }
        }

        private double _TargetCostPrice;
        [Range(1, int.MaxValue)]
        public double TargetCostPrice
        {
            get { return _TargetCostPrice; }
            set
            {
                _TargetCostPrice = value; RaisePropertyChanged("TargetCostPrice");
                LocalCost = (ExchangeRate * (decimal)value);
            }
        }
        private double _FabricCost;

        public double FabricCost
        {
            get { return _FabricCost; }
            set { _FabricCost = value; RaisePropertyChanged("FabricCost"); }
        }
        private double _AccCost;

        public double AccCost
        {
            get { return _AccCost; }
            set { _AccCost = value; RaisePropertyChanged("AccCost"); }
        }

        private double _OperationCost;

        public double OperationCost
        {
            get { return _OperationCost; }
            set { _OperationCost = value; RaisePropertyChanged("OperationCost"); }
        }
        private int? _TblRetailOrderProductionType;
        [Range(1, int.MaxValue)]
        public int? TblRetailOrderProductionType
        {
            get { return _TblRetailOrderProductionType; }
            set
            {
                _TblRetailOrderProductionType = value;
                RaisePropertyChanged("TblRetailOrderProductionType");
                FabricCostEnabled = true;
                AccCostEnabled = true;
                if (TblRetailOrderProductionType == 1)
                {
                    FabricCostEnabled = false;
                    AccCostEnabled = false;
                }
                if (TblRetailOrderProductionType == 2)
                {
                    FabricCostEnabled = false;
                    AccCostEnabled = true;
                }
                if (TblRetailOrderProductionType == 3)
                {
                    FabricCostEnabled = true;
                    AccCostEnabled = false;
                }
            }
        }

        private bool _FabricCostEnabled;

        public bool FabricCostEnabled
        {
            get { return _FabricCostEnabled; }
            set { _FabricCostEnabled = value; RaisePropertyChanged("FabricCostEnabled"); }
        }

        private bool _AccCostEnabled;

        public bool AccCostEnabled
        {
            get { return _AccCostEnabled; }
            set { _AccCostEnabled = value; RaisePropertyChanged("AccCostEnabled"); }
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
                    if (SupplierPerRow != null)
                    {
                        TblSupplier = SupplierPerRow.Iserial;
                    }
                }
            }
        }

        private int _tblSupplier;
        [Range(1, int.MaxValue)]
        public int TblSupplier
        {
            get
            {
                return _tblSupplier;
            }
            set
            {
                if ((Equals(_tblSupplier, value) != true))
                {
                    _tblSupplier = value;
                    RaisePropertyChanged("TblSupplier");
                }
            }
        }

        private ObservableCollection<TblStyleTNADetailViewModel> _DetailList;

        public ObservableCollection<TblStyleTNADetailViewModel> DetailList
        {
            get { return _DetailList ?? (_DetailList = new ObservableCollection<TblStyleTNADetailViewModel>()); }
            set { _DetailList = value; RaisePropertyChanged("DetailList"); }
        }

        private TblTechPackDesignDetailViewModel _SelectedTechPackDesignDetailRow;

        public TblTechPackDesignDetailViewModel SelectedTechPackDesignDetailRow
        {
            get { return _SelectedTechPackDesignDetailRow ?? (_SelectedTechPackDesignDetailRow = new TblTechPackDesignDetailViewModel()); }
            set { _SelectedTechPackDesignDetailRow = value; RaisePropertyChanged("SelectedTechPackDesignDetailRow"); }
        }


        private ObservableCollection<TblTechPackDesignDetailViewModel> _TechPackDesignDetailList;

        public ObservableCollection<TblTechPackDesignDetailViewModel> TechPackDesignDetailList
        {
            get { return _TechPackDesignDetailList ?? (_TechPackDesignDetailList = new ObservableCollection<TblTechPackDesignDetailViewModel>()); }
            set { _TechPackDesignDetailList = value; RaisePropertyChanged("TechPackDesignDetailList"); }
        }

        private int CreatedByField;

        private DateTime? CreationDateField;

        private int IserialField;

        private int? LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private int? TblLkpSeasonField;

        private CCWFM.CRUDManagerService.TblLkpSeason TblLkpSeason1Field;

        private int? TblStyleField;





        public int CreatedBy
        {
            get
            {
                return CreatedByField;
            }
            set
            {
                if ((CreatedByField.Equals(value) != true))
                {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
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
                    this.RaisePropertyChanged("CreationDate");
                }
            }
        }

        private DateTime? _DeliveryDate;
        [Required]
        public DateTime? DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; RaisePropertyChanged("DeliveryDate"); }
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
                    this.RaisePropertyChanged("Iserial");
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
                    this.RaisePropertyChanged("LastUpdatedBy");
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
                    this.RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }

        public int? TblLkpSeason
        {
            get
            {
                return this.TblLkpSeasonField;
            }
            set
            {
                if ((this.TblLkpSeasonField.Equals(value) != true))
                {
                    this.TblLkpSeasonField = value;
                    this.RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        public CCWFM.CRUDManagerService.TblLkpSeason SeasonPerRow
        {
            get
            {
                return TblLkpSeason1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblLkpSeason1Field, value) != true))
                {
                    this.TblLkpSeason1Field = value;
                    this.RaisePropertyChanged("SeasonPerRow");
                }
            }
        }

        public int? TblStyle
        {
            get
            {
                return TblStyleField;
            }
            set
            {
                if ((TblStyleField.Equals(value) != true))
                {
                    TblStyleField = value;
                    RaisePropertyChanged("TblStyle");
                }
            }
        }


        private TblCurrency _currencyPerRow;

        public TblCurrency CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (CurrencyPerRow != null)
                {
                    if (CurrencyPerRow.LocalCurrency == true)
                    {
                        ExchangeRate = 1;
                    }
                    TblCurrency = CurrencyPerRow.Iserial;

                    //    Client.ConvertCurrencyAsync(_currencyPerRow.Code, "EGP", 1);
                }

                //string url = string.Format("http://rate-exchange.appspot.com/currency?from={0}&to={1}", CurrencyPerRow.Code, "EGP");
                //WebClient client = new WebClient();
                //string rates = client.DownloadStringAsync(url);
                //client.DownloadStringCompleted += (s, sv) =>
                //{
                //};
                //Rate rate = new JavaScriptSerializer().Deserialize<Rate>(rates);
                //double converted_amount = amount * rate.rate;
                //string message = ddlFrom.SelectedItem.Value + ": " + amount + "\\n";
                //message += ddlTo.SelectedItem.Value + ": " + converted_amount + "\\n";
                //message += "Rate: 1 " + ddlFrom.SelectedItem.Value + " = " + rate.rate + " " + ddlTo.SelectedItem.Value;
            }
        }

        private int? _tblCurrency;

        public int? TblCurrency
        {
            get { return _tblCurrency; }
            set { _tblCurrency = value; RaisePropertyChanged("TblCurrency"); }
        }

        private decimal? _additionalCostField;

        public decimal? AdditionalCost
        {
            get
            {
                return _additionalCostField;
            }
            set
            {
                if ((_additionalCostField.Equals(value) != true))
                {
                    _additionalCostField = value;
                    RaisePropertyChanged("AdditionalCost");
                }
            }
        }

        private decimal? _exchangeRateField;

        public decimal? ExchangeRate
        {
            get
            {
                return _exchangeRateField;
            }
            set
            {
                if ((_exchangeRateField.Equals(value) != true))
                {
                    _exchangeRateField = value;
                    RaisePropertyChanged("ExchangeRate");
                    LocalCost = (decimal)TargetCostPrice * value;
                }
            }
        }

        private decimal? _localCostField;

        [ReadOnly(true)]
        public decimal? LocalCost
        {
            get
            {
                return _localCostField;
            }
            set
            {
                if ((_localCostField.Equals(value) != true))
                {
                    _localCostField = value;
                    RaisePropertyChanged("LocalCost");
                }
            }
        }

    }

    public class TblStyleTNADetailViewModel : PropertiesViewModelBase
    {
        private DateTime? ActualDeliveryDateField;

        private int CreatedByField;

        private DateTime? CreationDateField;

        private DateTime? ExpectedDeliveryDateField;

        private int IserialField;

        private int? LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private string NotesField;

        private int? TblStyleTNAField;

        private CCWFM.ProductionService.TblStyleTNA TblStyleTNA1Field;

        private int? TblStyleTNAHeaderField;

        public DateTime? ActualDeliveryDate
        {
            get
            {
                return ActualDeliveryDateField;
            }
            set
            {
                if ((ActualDeliveryDateField.Equals(value) != true))
                {
                    this.ActualDeliveryDateField = value;
                    this.RaisePropertyChanged("ActualDeliveryDate");
                }
            }
        }
        public int CreatedBy
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
                    this.RaisePropertyChanged("CreatedBy");
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
                    this.RaisePropertyChanged("CreationDate");
                }
            }
        }
        //[Required]
        public DateTime? ExpectedDeliveryDate
        {
            get
            {
                return this.ExpectedDeliveryDateField;
            }
            set
            {
                if ((this.ExpectedDeliveryDateField.Equals(value) != true))
                {
                    this.ExpectedDeliveryDateField = value;
                    this.RaisePropertyChanged("ExpectedDeliveryDate");
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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        public int? LastUpdatedBy
        {
            get
            {
                return LastUpdatedByField;
            }
            set
            {
                if ((this.LastUpdatedByField.Equals(value) != true))
                {
                    this.LastUpdatedByField = value;
                    this.RaisePropertyChanged("LastUpdatedBy");
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
                    this.RaisePropertyChanged("LastUpdatedDate");
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
                if ((object.ReferenceEquals(this.NotesField, value) != true))
                {
                    this.NotesField = value;
                    this.RaisePropertyChanged("Notes");
                }
            }
        }

        [Required]

        public int? TblStyleTNA
        {
            get
            {
                return this.TblStyleTNAField;
            }
            set
            {
                if ((this.TblStyleTNAField.Equals(value) != true))
                {
                    this.TblStyleTNAField = value;
                    this.RaisePropertyChanged("TblStyleTNA");
                }
            }
        }


        private int? _TblStyleTNARouteStatus;

        public int? TblStyleTNARouteStatus
        {
            get
            {
                return this._TblStyleTNARouteStatus;
            }
            set
            {
                if ((this._TblStyleTNARouteStatus.Equals(value) != true))
                {
                    this._TblStyleTNARouteStatus = value;
                    this.RaisePropertyChanged("TblStyleTNARouteStatus");
                }
            }
        }

        public CCWFM.ProductionService.TblStyleTNA StyleTNAPerRow
        {
            get
            {
                return this.TblStyleTNA1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblStyleTNA1Field, value) != true))
                {
                    this.TblStyleTNA1Field = value;
                    this.RaisePropertyChanged("StyleTNAPerRow");
                }
            }
        }

        private CCWFM.LkpData.TblStyleTNARouteStatu _StyleTNAStatusPerRow;

        public CCWFM.LkpData.TblStyleTNARouteStatu StyleTNAStatusPerRow
        {
            get
            {
                return this._StyleTNAStatusPerRow;
            }
            set
            {
                if ((object.ReferenceEquals(this._StyleTNAStatusPerRow, value) != true))
                {
                    this._StyleTNAStatusPerRow = value;
                    this.RaisePropertyChanged("StyleTNAStatusPerRow");
                }
            }
        }
        public int? TblStyleTNAHeader
        {
            get
            {
                return this.TblStyleTNAHeaderField;
            }
            set
            {
                if ((this.TblStyleTNAHeaderField.Equals(value) != true))
                {
                    this.TblStyleTNAHeaderField = value;
                    this.RaisePropertyChanged("TblStyleTNAHeader");
                }
            }
        }


        private ObservableCollection<CCWFM.ProductionService.TblStyleTNADetailAttachment> _TblStyleTNADetailAttachment;
        public ObservableCollection<CCWFM.ProductionService.TblStyleTNADetailAttachment> TblStyleTNADetailAttachment
        {
            get
            {
                return this._TblStyleTNADetailAttachment;
            }
            set
            {
                if ((object.ReferenceEquals(this._TblStyleTNADetailAttachment, value) != true))
                {
                    this._TblStyleTNADetailAttachment = value;
                    this.RaisePropertyChanged("TblStyleTNADetailAttachment");
                }
            }
        }
    }

    public class TblTechPackDesignDetailViewModel : PropertiesViewModelBase
    {

        private string _galarylink;
        public string galarylink
        {
            get; set;
        }

        private string _imageName;
        public string ImageName
        {
            get
            {
                return this._imageName;
            }
            set
            {
                this._imageName = value;
                this.RaisePropertyChanged("ImageName");
            }
        }

        public byte[] ImageThumb { get; set; }

        private int IserialField;
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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        private int _tblTechPackHeaderField;
        public int tblTechPackHeader
        {
            get
            {
                return this._tblTechPackHeaderField;
            }
            set
            {
                if ((this._tblTechPackHeaderField.Equals(value) != true))
                {
                    this._tblTechPackHeaderField = value;
                    this.RaisePropertyChanged("tblTechPackHeader");
                }
            }
        }


        private int _tblStyle;
        public int tblStyle
        {
            get
            {
                return this._tblStyle;
            }
            set
            {
                if ((this._tblStyle.Equals(value) != true))
                {
                    this._tblStyle = value;
                    this.RaisePropertyChanged("tblStyle");
                }
            }
        }
        private int _tBLTechPackStatus;
        public int TBLTechPackStatus
        {
            get
            {
                return this._tBLTechPackStatus;
            }
            set
            {
                if ((this._tBLTechPackStatus.Equals(value) != true))
                {
                    this._tBLTechPackStatus = value;
                    this.RaisePropertyChanged("TBLTechPackStatus");
                }
            }
        }

        private string DescriptionField;
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true))
                {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

        private int? TblTechPackPartField;

        public int? TblTechPackPart
        {
            get
            {
                return this.TblTechPackPartField;
            }
            set
            {
                if ((this.TblTechPackPartField.Equals(value) != true))
                {
                    this.TblTechPackPartField = value;
                    this.RaisePropertyChanged("TblTechPackPart");
                }
            }
        }
        private CCWFM.LkpData.tblTechPackPart TblTechPackPart1Field;

        public CCWFM.LkpData.tblTechPackPart TechPackPartPerRow
        {
            get
            {
                return this.TblTechPackPart1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblTechPackPart1Field, value) != true))
                {
                    this.TblTechPackPart1Field = value;
                    this.RaisePropertyChanged("TechPackPartPerRow");
                }
            }
        }

    }

    public class TblStyleSpecDetailsViewModel : PropertiesViewModelBase
    {
        private int IserialField;
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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        private int _tblStyle;
        public int tblStyle
        {
            get
            {
                return this._tblStyle;
            }
            set
            {
                if ((this._tblStyle.Equals(value) != true))
                {
                    this._tblStyle = value;
                    this.RaisePropertyChanged("tblStyle");
                }
            }
        }

        private string DescriptionField;
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true))
                {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

        private int? TblStyleSpecTypesField;

        public int? TblStyleSpecTypes
        {
            get
            {
                return this.TblStyleSpecTypesField;
            }
            set
            {
                if ((this.TblStyleSpecTypesField.Equals(value) != true))
                {
                    this.TblStyleSpecTypesField = value;
                    this.RaisePropertyChanged("TblStyleSpecTypes");
                }
            }
        }

        private CCWFM.LkpData.TblStyleSpecType TblStyleSpecType1Field;
        
        public CCWFM.LkpData.TblStyleSpecType TblStyleSpecTypePerRow
        {
            get
            {
                return this.TblStyleSpecType1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblStyleSpecType1Field, value) != true))
                {
                    this.TblStyleSpecType1Field = value;
                    this.RaisePropertyChanged("TblStyleSpecTypePerRow");
                }
            }
        }

        private ObservableCollection<CCWFM.LkpData.tblStyleSpecDetailAttachment> _tblStyleSpecDetailAttachment;
        public ObservableCollection<CCWFM.LkpData.tblStyleSpecDetailAttachment> TblStyleSpecDetailAttachment
        {
            get
            {
                return this._tblStyleSpecDetailAttachment;
            }
            set
            {
                if ((object.ReferenceEquals(this._tblStyleSpecDetailAttachment, value) != true))
                {
                    this._tblStyleSpecDetailAttachment = value;
                    this.RaisePropertyChanged("TblStyleSpecDetailAttachment");
                }
            }
        }
    }

    public class TblTechPackDetailViewModel : PropertiesViewModelBase
    {


        /*
          [Iserial] [int] IDENTITY(1,1) NOT NULL,
          [tblTechPackHeader] [int] NULL,
          [tblTechPackPart] [int] NULL,
          [Description] [varchar](50) NULL,
          [galaryLink] [varchar](50) NULL,
          [tblUser] [int] NULL,
       */

        private int IserialField;
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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        private int tblTechPackHeaderField;
        public int tblTechPackHeader
        {
            get
            {
                return this.tblTechPackHeaderField;
            }
            set
            {
                if ((this.tblTechPackHeaderField.Equals(value) != true))
                {
                    this.tblTechPackHeaderField = value;
                    this.RaisePropertyChanged("TBLTechPackHeader");
                }
            }
        }

        private string DescriptionField;
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true))
                {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

        private CCWFM.LkpData.tblTechPackPart tblTechPackPartField;
        public CCWFM.LkpData.tblTechPackPart TechPackPartPerRow
        {
            get
            {
                return this.tblTechPackPartField;
            }
            set
            {
                if ((object.ReferenceEquals(this.tblTechPackPartField, value) != true))
                {
                    this.tblTechPackPartField = value;
                    this.RaisePropertyChanged("TechPackPartPerRow");
                }
            }
        }
    }

    public class TblStyleTNAColorDetailModel : PropertiesViewModelBase
    {
        private DateTime? _DeliveryDate;
        [Required]
        public DateTime? DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; RaisePropertyChanged("DeliveryDate"); }
        }
        private TblSeasonalMasterList _seasonalMasterListPerRow;

        public TblSeasonalMasterList SeasonalMasterPerRow
        {
            get { return _seasonalMasterListPerRow; }
            set
            {
                _seasonalMasterListPerRow = value;

                RaisePropertyChanged("SeasonalMasterPerRow");

            }
        }
        private System.Nullable<int> TblColorField;

        private TblColor _ColorPerRow;

        private int? TblStyleTNAHeaderField;
        int IserialField;
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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqColor")]
        public int? TblColor
        {
            get
            {
                return this.TblColorField;
            }
            set
            {
                if ((this.TblColorField.Equals(value) != true))
                {
                    this.TblColorField = value;
                    this.RaisePropertyChanged("TblColor");
                }
            }
        }

        private int _Qty;
        [ReadOnly(true)]
        public int Qty
        {
            get { return _Qty; }
            set { _Qty = value; RaisePropertyChanged("Qty"); }
        }

        public TblColor ColorPerRow
        {
            get
            {
                return _ColorPerRow;
            }
            set
            {
                if ((ReferenceEquals(this._ColorPerRow, value) != true))
                {
                    this._ColorPerRow = value;
                    this.RaisePropertyChanged("ColorPerRow");
                }
            }
        }


        public System.Nullable<int> TblStyleTNAHeader
        {
            get
            {
                return this.TblStyleTNAHeaderField;
            }
            set
            {
                if ((this.TblStyleTNAHeaderField.Equals(value) != true))
                {
                    this.TblStyleTNAHeaderField = value;
                    this.RaisePropertyChanged("TblStyleTNAHeader");
                }
            }
        }
        private decimal? _costField;

        public decimal? Cost
        {
            get { return _costField; }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");

                }
            }
        }


        private TblCurrency _currencyPerRow;

        public TblCurrency CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (CurrencyPerRow != null)
                {
                    if (CurrencyPerRow.LocalCurrency == true)
                    {
                        ExchangeRate = 1;
                    }
                    TblCurrency = CurrencyPerRow.Iserial;

                    //    Client.ConvertCurrencyAsync(_currencyPerRow.Code, "EGP", 1);
                }

                //string url = string.Format("http://rate-exchange.appspot.com/currency?from={0}&to={1}", CurrencyPerRow.Code, "EGP");
                //WebClient client = new WebClient();
                //string rates = client.DownloadStringAsync(url);
                //client.DownloadStringCompleted += (s, sv) =>
                //{
                //};
                //Rate rate = new JavaScriptSerializer().Deserialize<Rate>(rates);
                //double converted_amount = amount * rate.rate;
                //string message = ddlFrom.SelectedItem.Value + ": " + amount + "\\n";
                //message += ddlTo.SelectedItem.Value + ": " + converted_amount + "\\n";
                //message += "Rate: 1 " + ddlFrom.SelectedItem.Value + " = " + rate.rate + " " + ddlTo.SelectedItem.Value;
            }
        }

        private int? _tblCurrency;

        public int? TblCurrency
        {
            get { return _tblCurrency; }
            set { _tblCurrency = value; RaisePropertyChanged("TblCurrency"); }
        }

        private decimal? _additionalCostField;

        public decimal? AdditionalCost
        {
            get
            {
                return _additionalCostField;
            }
            set
            {
                if ((_additionalCostField.Equals(value) != true))
                {
                    _additionalCostField = value;
                    RaisePropertyChanged("AdditionalCost");
                }
            }
        }

        private decimal? _exchangeRateField;

        public decimal? ExchangeRate
        {
            get
            {
                return _exchangeRateField;
            }
            set
            {
                if ((_exchangeRateField.Equals(value) != true))
                {
                    _exchangeRateField = value;
                    RaisePropertyChanged("ExchangeRate");
                    LocalCost = Cost * value;
                }
            }
        }

        private decimal? _localCostField;

        [ReadOnly(true)]
        public decimal? LocalCost
        {
            get
            {
                return _localCostField;
            }
            set
            {
                if ((_localCostField.Equals(value) != true))
                {
                    _localCostField = value;
                    RaisePropertyChanged("LocalCost");
                }
            }
        }

        private double _FabricCost;

        public double FabricCost
        {
            get { return _FabricCost; }
            set { _FabricCost = value; RaisePropertyChanged("FabricCost"); }
        }
        private double _AccCost;

        public double AccCost
        {
            get { return _AccCost; }
            set { _AccCost = value; RaisePropertyChanged("AccCost"); }
        }

        private double _OperationCost;

        public double OperationCost
        {
            get { return _OperationCost; }
            set { _OperationCost = value; RaisePropertyChanged("OperationCost"); }
        }

    }

    public class TblStyleViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblStyleSpecDetailsViewModel> _StyleSpecDetailsList;

        public ObservableCollection<TblStyleSpecDetailsViewModel> StyleSpecDetailsList
        {
            get { return _StyleSpecDetailsList ?? (_StyleSpecDetailsList = new ObservableCollection<TblStyleSpecDetailsViewModel>()); }
            set { _StyleSpecDetailsList = value; RaisePropertyChanged("StyleSpecDetailsList"); }
        }


        private TblStyleSpecDetailsViewModel _StyleSpecDetailPerRow;

        public TblStyleSpecDetailsViewModel StyleSpecDetailPerRow
        {
            get { return _StyleSpecDetailPerRow ?? (_StyleSpecDetailPerRow = new TblStyleSpecDetailsViewModel()); }
            set { _StyleSpecDetailPerRow = value; RaisePropertyChanged("StyleSpecDetailPerRow"); }
        }

        private LkpData.TblStyleSpecType _StyleSpecTypePerRow;

        public LkpData.TblStyleSpecType StyleSpecTypePerRow
        {
            get { return _StyleSpecTypePerRow ?? (_StyleSpecTypePerRow = new LkpData.TblStyleSpecType()); }
            set { _StyleSpecTypePerRow = value; RaisePropertyChanged("StyleSpecTypePerRow"); }
        }

        private ObservableCollection<TblColor> _selectedStyleSeasonalMaterColors;
        public ObservableCollection<TblColor> SelectedStyleSeasonalMaterColors
        {
            get { return _selectedStyleSeasonalMaterColors; }
            set { _selectedStyleSeasonalMaterColors = value; RaisePropertyChanged("SelectedStyleSeasonalMaterColors"); }
        }

        public Brush Approved
        {
            get
            {
                return this.Canceled == true ?
                  new SolidColorBrush(Colors.Red) : null;
            }
        }
        private string _LastTnaStatus;

        public string LastTnaStatus
        {
            get { return _LastTnaStatus; }
            set { _LastTnaStatus = value; RaisePropertyChanged("LastTnaStatus"); }
        }

        private string _Fabric1;

        public string Fabric1
        {
            get { return _Fabric1; }
            set { _Fabric1 = value; RaisePropertyChanged("Fabric1"); }
        }

        private LkpData.tblTechPackHeader _tblTechPackHeader;

        public LkpData.tblTechPackHeader tblTechPackHeader
        {
            get { return _tblTechPackHeader; }
            set { _tblTechPackHeader = value; RaisePropertyChanged("tblTechPackHeader"); }
        }


        private LkpData.tblTechPackBOMComment _tblTechPackBOMComment;

        public LkpData.tblTechPackBOMComment tblTechPackBOMComment
        {
            get { return _tblTechPackBOMComment; }
            set { _tblTechPackBOMComment = value; RaisePropertyChanged("tblTechPackBOMComment"); }
        }

        private string _Fabric2;

        public string Fabric2
        {
            get { return _Fabric2; }
            set { _Fabric2 = value; RaisePropertyChanged("Fabric2"); }
        }

        private ObservableCollection<TblColor> _SeasonalMasterListColorsList;

        public ObservableCollection<TblColor> SeasonalMasterListColorsList
        {
            get { return _SeasonalMasterListColorsList; }
            set { _SeasonalMasterListColorsList = value; RaisePropertyChanged("SeasonalMasterListColorsList"); }
        }

        private int _TnaCreatedFrom;

        public int TnaCreatedFrom
        {
            get { return _TnaCreatedFrom; }
            set { _TnaCreatedFrom = value; RaisePropertyChanged("TnaCreatedFrom"); }
        }

        private DateTime? _TNACreationDate;

        public DateTime? TNACreationDate
        {
            get { return _TNACreationDate; }
            set
            {

                _TNACreationDate = value;
                if (value != null)
                {
                    TimeSpan difference = DateTime.Now.Date - _TNACreationDate.Value.Date;
                    int days = (int)difference.TotalDays;
                    TnaCreatedFrom = days;
                }
                RaisePropertyChanged("TNACreationDate");
            }
        }
        private bool _RequestTna;

        public bool RequestTna
        {
            get { return _RequestTna; }
            set { _RequestTna = value; RaisePropertyChanged("RequestTna"); }
        }

        private bool _canceled;

        public bool Canceled
        {
            get { return _canceled; }
            set { _canceled = value; RaisePropertyChanged("Canceled"); }
        }
        private ObservableCollection<TblStyleTNAHeaderViewModel> _StyleTnaList;

        public ObservableCollection<TblStyleTNAHeaderViewModel> StyleTnaList
        {
            get { return _StyleTnaList ?? (_StyleTnaList = new ObservableCollection<TblStyleTNAHeaderViewModel>()); }
            set { _StyleTnaList = value; RaisePropertyChanged("StyleTnaList"); }
        }

        private ObservableCollection<TblTechPackDetailViewModel> _TechPackDesignDetailList;

        public ObservableCollection<TblTechPackDetailViewModel> TechPackDesignDetailList
        {
            get { return _TechPackDesignDetailList ?? (_TechPackDesignDetailList = new ObservableCollection<TblTechPackDetailViewModel>()); }
            set { _TechPackDesignDetailList = value; RaisePropertyChanged("TechPackDesignDetailList"); }
        }
        

      

        private ObservableCollection<LkpData.TblFamily> _familyList;

        public ObservableCollection<LkpData.TblFamily> FamilyList
        {
            get { return _familyList ?? (_familyList = new ObservableCollection<LkpData.TblFamily>()); }
            set { _familyList = value; RaisePropertyChanged("FamilyList"); }
        }

        public bool TransactionExists { get; set; }

        public bool RetialPoTransactionExist { get; set; }

        private bool _familyUpdate;

        public bool familyUpdate
        {
            get { return _familyUpdate; }
            set { _familyUpdate = value; RaisePropertyChanged("familyUpdate"); }
        }

        private bool _brandUpdate;

        public bool brandUpdate
        {
            get { return _brandUpdate; }
            set { _brandUpdate = value; RaisePropertyChanged("brandUpdate"); }
        }

        private bool _brandSectionUpdate;

        public bool brandSectionUpdate
        {
            get { return _brandSectionUpdate; }
            set { _brandSectionUpdate = value; RaisePropertyChanged("brandSectionUpdate"); }
        }

        private bool _seasonUpdate;

        public bool seasonUpdate
        {
            get { return _seasonUpdate; }
            set { _seasonUpdate = value; RaisePropertyChanged("seasonUpdate"); }
        }

        private ObservableCollection<TblSupplierFabric> _supplierFabrics;

        public ObservableCollection<TblSupplierFabric> SupplierFabrics
        {
            get { return _supplierFabrics; }
            set { _supplierFabrics = value; RaisePropertyChanged("SupplierFabrics"); }
        }

        private TblSupplierFabric _supplierFabricPerRow;

        public TblSupplierFabric SupplierFabricPerRow
        {
            get { return _supplierFabricPerRow; }
            set
            {
                _supplierFabricPerRow = value; RaisePropertyChanged("SupplierFabricPerRow");
                if (_supplierFabricPerRow != null)
                {
                    TblSupplierFabric = SupplierFabricPerRow.Iserial;
                }
            }
        }

        private int? _tblStyleCategory;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCategory")]
        public int? TblStyleCategory
        {
            get
            {
                return _tblStyleCategory;
            }
            set
            {
                if ((_tblStyleCategory.Equals(value) != true))
                {
                    _tblStyleCategory = value;
                    RaisePropertyChanged("TblStyleCategory");
                }
            }
        }

        private int? _tblGenericFabric;
        public int? TblGenericFabric
        {
            get
            {
                return _tblGenericFabric;
            }
            set
            {
                if ((_tblGenericFabric.Equals(value) != true))
                {
                    _tblGenericFabric = value;
                    RaisePropertyChanged("TblGenericFabric");
                }
            }
        }


        private TblStyleFabricComposition _compositionPerRow;

        public TblStyleFabricComposition StyleFabricCompositionPerRow
        {
            get { return _compositionPerRow ?? (_compositionPerRow = new TblStyleFabricComposition()); }
            set { _compositionPerRow = value; RaisePropertyChanged("StyleFabricCompositionPerRow"); }
        }

        private int? _tblStyleFabricComposition;

        public int? TblStyleFabricComposition
        {
            get { return _tblStyleFabricComposition; }
            set { _tblStyleFabricComposition = value; RaisePropertyChanged("TblStyleFabricComposition"); }
        }

        private int? _tblSupplierFabric;

        public int? TblSupplierFabric
        {
            get
            {
                return _tblSupplierFabric;
            }
            set
            {
                if ((_tblSupplierFabric.Equals(value) != true))
                {
                    _tblSupplierFabric = value;
                    RaisePropertyChanged("TblSupplierFabric");
                }
            }
        }

        private int? _tblStyleStatus;

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCategory")]

        public int? TblStyleStatus
        {
            get
            {
                return _tblStyleStatus;
            }
            set
            {
                if ((_tblStyleStatus.Equals(value) != true))
                {
                    _tblStyleStatus = value;
                    RaisePropertyChanged("TblStyleStatus");
                }
            }
        }

        private ObservableCollection<GenericTable> _directionList;

        public ObservableCollection<GenericTable> DirectionList
        {
            get { return _directionList ?? (_directionList = new ObservableCollection<GenericTable>()); }
            set { _directionList = value; RaisePropertyChanged("DirectionList"); }
        }

        private ObservableCollection<GenericTable> _styleThemeList;
        public ObservableCollection<GenericTable> StyleThemeList
        {
            get { return _styleThemeList ?? (_styleThemeList = new ObservableCollection<GenericTable>()); }
            set { _styleThemeList = value; RaisePropertyChanged("StyleThemeList"); }
        }

        private int? _tblGroup1Field;

        private int _tblGroup4Field;

        private int _tblGroup6Field;

        private int _tblGroup7Field;

        private int _tblGroup8Field;

        public int? TblGroup1
        {
            get
            {
                return _tblGroup1Field;
            }
            set
            {
                if ((_tblGroup1Field.Equals(value) != true))
                {
                    _tblGroup1Field = value;
                    RaisePropertyChanged("TblGroup1");
                }
            }
        }

        public int TblGroup4
        {
            get
            {
                return _tblGroup4Field;
            }
            set
            {
                if ((_tblGroup4Field.Equals(value) != true))
                {
                    _tblGroup4Field = value;
                    RaisePropertyChanged("TblGroup4");
                }
            }
        }

        public int TblGroup6
        {
            get
            {
                return _tblGroup6Field;
            }
            set
            {
                if ((_tblGroup6Field.Equals(value) != true))
                {
                    _tblGroup6Field = value;
                    RaisePropertyChanged("TblGroup6");
                }
            }
        }

        public int TblGroup7
        {
            get
            {
                return _tblGroup7Field;
            }
            set
            {
                if ((_tblGroup7Field.Equals(value) != true))
                {
                    _tblGroup7Field = value;
                    RaisePropertyChanged("TblGroup7");
                }
            }
        }

        public int TblGroup8
        {
            get
            {
                return _tblGroup8Field;
            }
            set
            {
                if ((_tblGroup8Field.Equals(value) != true))
                {
                    _tblGroup8Field = value;
                    RaisePropertyChanged("TblGroup8");
                }
            }
        }

        private TblGROUP1 _group1PerRow;

        public TblGROUP1 Group1PerRow
        {
            get { return _group1PerRow; }
            set
            {
                _group1PerRow = value; RaisePropertyChanged("Group1PerRow");
                if (_group1PerRow != null)
                {
                    TblGroup1 = _group1PerRow.ISERIAL;
                }
            }
        }

        private TblGROUP1 _group4PerRow;

        public TblGROUP1 Group4PerRow
        {
            get { return _group4PerRow; }
            set
            {
                _group4PerRow = value; RaisePropertyChanged("Group4PerRow");
                if (_group4PerRow != null)
                {
                    TblGroup4 = _group4PerRow.ISERIAL;
                }
            }
        }

        private TblGROUP1 _group6PerRow;

        public TblGROUP1 Group6PerRow
        {
            get { return _group6PerRow; }
            set
            {
                _group6PerRow = value; RaisePropertyChanged("Group6PerRow");
                if (_group6PerRow != null)
                {
                    TblGroup6 = _group6PerRow.ISERIAL;
                }
            }
        }

        private TblGROUP1 _group7PerRow;

        public TblGROUP1 Group7PerRow
        {
            get { return _group7PerRow; }
            set
            {
                _group7PerRow = value; RaisePropertyChanged("Group7PerRow");
                if (_group7PerRow != null)
                {
                    TblGroup7 = _group7PerRow.ISERIAL;
                }
            }
        }

        private TblGROUP1 _group8PerRow;

        public TblGROUP1 Group8PerRow
        {
            get { return _group8PerRow; }
            set
            {
                _group8PerRow = value; RaisePropertyChanged("Group8PerRow");
                if (_group8PerRow != null)
                {
                    TblGroup8 = _group8PerRow.ISERIAL;
                }
            }
        }

        private string _brandField;

        private string _createdByField;

        private DateTime? _creationDateField;

        private string _descriptionField;

        private string _lastUpdatedByField;

        private DateTime? _lastUpdatedDateField;

        private string _notesField;

        private string _refStyleCodeField;

      

        private string _GenericFabricNotes;
        public string GenericFabricNotes
        {
            get
            {
                return _GenericFabricNotes;
            }
            set { _GenericFabricNotes = value; RaisePropertyChanged("GenericFabricNotes"); }
        }

       

        private int? _tblLkpBrandSection;

        private string _serialField;

        private int? _sizeGroupField;

        private int? _statusField;

        private string _styleCodeField;

        private string _styleTNAMaxCostField;
        private string _styleTNADeliveryDateField;

        private double _retailtargetCostPriceField;
        private double _barcodePrice;
        private double _targetCostPriceField;
        private double _cctargetCostPriceField;
        private int? _tblLkpDirectionField;

        private int? _tblLkpSeasonField;

        private GenericTable _directionPerRow;

        private TblLkpSeason _seasonPerRow;

        private TblSizeGroup _sizeGroupPerRow;

      

        private LkpData.TblLkpBrandSection _sectionPerRow;

        private GenericTable _designPerRow;

        private int? _tblFamilyField;

        private LkpData.TblFamily _familyPerRow;

        private int? _tblSubFamilyField;

        private LkpData.TblSubFamily _subFamilyPerRow;

        private int? _tblFabricAttriputes;

        public int? tbl_FabricAttriputes
        {
            get { return _tblFabricAttriputes; }
            set
            {
                if ((_tblFabricAttriputes.Equals(value) != true))
                {
                    _tblFabricAttriputes = value; RaisePropertyChanged("tbl_FabricAttriputes");
                }
            }
        }

        private string _externalStyleCode;

        public string ExternalStyleCode
        {
            get { return _externalStyleCode; }
            set
            {
                if (value != null) _externalStyleCode = value.Trim();
                RaisePropertyChanged("ExternalStyleCode");
            }
        }

        private string _supplierRef;

        public string SupplierRef
        {
            get { return _supplierRef; }
            set
            {
                if (value != null) _supplierRef = value.Trim();
                RaisePropertyChanged("SupplierRef");
            }
        }

        private ObservableCollection<TblSalesOrderViewModel> _detailsList;

        public ObservableCollection<TblSalesOrderViewModel> DetailsList
        {
            get { return _detailsList ?? (DetailsList = new ObservableCollection<TblSalesOrderViewModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailsList"); }
        }

        private ObservableCollection<TblSalesOrderViewModel> _tempDetailsList;

        public ObservableCollection<TblSalesOrderViewModel> TempDetailsList
        {
            get { return _tempDetailsList ?? (_tempDetailsList = new ObservableCollection<TblSalesOrderViewModel>()); }
            set { _tempDetailsList = value; RaisePropertyChanged("TempDetailsList"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set
            {
                _iserial = value; RaisePropertyChanged("Iserial");
            }
        }

        private int? _tblLkpFabricDesignes;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDesign")]
        public int? tbl_lkp_FabricDesignes
        {
            get { return _tblLkpFabricDesignes; }
            set
            {
                if ((_tblLkpFabricDesignes.Equals(value) != true))
                {
                    _tblLkpFabricDesignes = value; RaisePropertyChanged("tbl_lkp_FabricDesignes");
                    UpdatedStyleCode();
                }
            }
        }

        private string _theme_Code;
        public string Theme_Code
        {
            get
            {
                return _theme_Code;
            }
            set
            {
                if ((ReferenceEquals(_theme_Code, value) != true))
                {
                    if (value != null) _theme_Code = value.Trim();
                    RaisePropertyChanged("Theme_Code");
                    UpdatedStyleCode();
                }
            }
        }



        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
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
                    if (value != null) _brandField = value.Trim();
                    RaisePropertyChanged("Brand");
                    UpdatedStyleCode();

                }
            }
        }

        private bool? _manualCode;
        public bool? ManualCode
        {

            get { return _manualCode; }
            set
            {
                _manualCode = value; RaisePropertyChanged("ManualCode");
            }
        }

        [ReadOnly(true)]
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
        public string Description
        {
            get
            {
                return _descriptionField = ToTitleCase(_descriptionField);
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    if (value != null)
                    {
                        string temp = value.Trim().ToUpperInvariant();
                        _descriptionField = ToTitleCase(temp);
                    }
                }

                RaisePropertyChanged("Description");
            }
        }

        public string ToTitleCase(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            var result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }

        private string _additionalDescription;

        public string AdditionalDescription
        {
            get
            {
                return _additionalDescription = ToTitleCase(_additionalDescription);
            }
            set
            {
                if ((ReferenceEquals(_additionalDescription, value) != true))
                {
                    if (_additionalDescription == value)
                    {
                        return;
                    }
                    if (value != null)
                    {
                        string temp = value.Trim().ToUpperInvariant();
                        _additionalDescription = ToTitleCase(temp);
                    }
                    RaisePropertyChanged("AdditionalDescription");
                    UpdatedStyleCode();
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
                    if (value != null) _notesField = value.Trim();
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public string RefStyleCode
        {
            get
            {
                return _refStyleCodeField;
            }
            set
            {
                if ((ReferenceEquals(_serialField, value) != true))
                {
                    if (value != null) _refStyleCodeField = value.Trim().ToUpperInvariant();

                    _refStyleCodeField = _refStyleCodeField.Replace(" ", "");

                    RaisePropertyChanged("RefStyleCode");
                }
            }
        }

        private string _foreignFabric;

        public string ForeignFabric
        {
            get
            {
                return _foreignFabric;
            }
            set
            {
                if ((ReferenceEquals(_foreignFabric, value) != true))
                {
                    if (value != null) _foreignFabric = value.Trim();
                    RaisePropertyChanged("ForeignFabric");
                }
            }
        }

        private string _foreignFabricDescription;

        public string ForeignFabricDescription
        {
            get
            {
                return _foreignFabricDescription;
            }
            set
            {
                if ((ReferenceEquals(_foreignFabricDescription, value) != true))
                {
                    if (value != null) _foreignFabricDescription = value.Trim();
                    RaisePropertyChanged("ForeignFabricDescription");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrandSection")]
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
                    DirectionList.Clear();

                    _tblLkpBrandSection = value;
                    RaisePropertyChanged("TblLkpBrandSection");

                    UpdatedStyleCode();
                }
            }
        }

        private int? _tblThemeField;

        public int? tblTheme
        {
            get
            {
                return _tblThemeField;
            }
            //set
            //{
            //    if ((_tblThemeField.Equals(value) != true))
            //    {
            //        if (_tblThemeField == value)
            //        {
            //            return;
            //        }

            //         StyleThemeList.Clear();
            //        _tblThemeField = value;
            //        RaisePropertyChanged("tblTheme");
            //    }
            //}
            set
            {
                if ((Equals(_tblThemeField, value) != true))
                {
                    _tblThemeField = value;
                    RaisePropertyChanged("tblTheme");
                }
            }
        }

        public string SerialNo
        {
            get
            {
                return _serialField;
            }
            set
            {
                if ((ReferenceEquals(_serialField, value) != true))
                {
                    _serialField = value;
                    RaisePropertyChanged("SerialNo");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSizeGroup")]
        public int? TblSizeGroup
        {
            get
            {
                return _sizeGroupField;
            }
            set
            {
                if ((Equals(_sizeGroupField, value) != true))
                {
                    _sizeGroupField = value;
                    RaisePropertyChanged("TblSizeGroup");
                }
            }
        }

        public int? Status
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

        public string StyleCode
        {
            get
            {
                return _styleCodeField;
            }
            set
            {
                if ((ReferenceEquals(_styleCodeField, value) != true))
                {
                    if (value != null) _styleCodeField = value.Trim().ToUpperInvariant();
                    RaisePropertyChanged("StyleCode");
                }
            }
        }

        public string StyleTNAMaxCost
        {
            get
            {
                return _styleTNAMaxCostField;
            }
            set
            {
                if ((ReferenceEquals(_styleTNAMaxCostField, value) != true))
                {
                    if (value != null) _styleTNAMaxCostField = value.Trim().ToUpperInvariant();
                    RaisePropertyChanged("StyleTNAMaxCost");
                }
            }
        }
        private string _styleRetailMaxDeliveryDateField;
        public string StyleRetailMaxDeliveryDate
        {
            get
            {
                return _styleRetailMaxDeliveryDateField;
            }
            set
            {
                if ((ReferenceEquals(_styleRetailMaxDeliveryDateField, value) != true))
                {
                    if (value != null) _styleRetailMaxDeliveryDateField = value.Trim().ToUpperInvariant();
                    RaisePropertyChanged("StyleRetailMaxDeliveryDate");
                }
            }
        }

        private string _styleTNASupplierNameField;

        public string StyleTNASupplierName
        {
            get
            {
                return _styleTNASupplierNameField;
            }
            set
            {
                if ((ReferenceEquals(_styleTNASupplierNameField, value) != true))
                {
                    if (value != null) _styleTNASupplierNameField = value.Trim().ToUpperInvariant();
                    RaisePropertyChanged("StyleTNASupplierName");
                }
            }
        }
        public string StyleTNADeliveryDate
        {
            get
            {
                return _styleTNADeliveryDateField;
            }
            set
            {
                if ((ReferenceEquals(_styleTNADeliveryDateField, value) != true))
                {
                    if (value != null) _styleTNADeliveryDateField = value.Trim().ToUpperInvariant();
                    RaisePropertyChanged("StyleTNADeliveryDate");
                }
            }
        }

        public double RetailTargetCostPrice
        {
            get
            {
                return _retailtargetCostPriceField;
            }
            set
            {
                if ((_retailtargetCostPriceField.Equals(value) != true))
                {
                    _retailtargetCostPriceField = value;
                    RaisePropertyChanged("RetailTargetCostPrice");
                    ProfitMargin = RetailTargetCostPrice / TargetCostPrice;
                }
            }
        }

        public double BarcodePrice
        {
            get
            {
                return _barcodePrice;
            }
            set
            {
                if ((_barcodePrice.Equals(value) != true))
                {
                    _barcodePrice = value;
                    RaisePropertyChanged("BarcodePrice");
                }
            }
        }

        public double TargetCostPrice
        {
            get
            {
                return _targetCostPriceField;
            }
            set
            {
                if ((_targetCostPriceField.Equals(value) != true))
                {
                    _targetCostPriceField = value;

                    RaisePropertyChanged("TargetCostPrice");
                    ProfitMargin = RetailTargetCostPrice / TargetCostPrice;
                }
            }
        }

        public double CCTargetCostPrice
        {
            get
            {
                return _cctargetCostPriceField;
            }
            set
            {
                if ((_cctargetCostPriceField.Equals(value) != true))
                {
                    _cctargetCostPriceField = value;

                    RaisePropertyChanged("CCTargetCostPrice");
                }
            }
        }

        private double _profitMargin;

        public double ProfitMargin
        {
            get
            {
                return _profitMargin;
            }
            set
            {
                if ((_profitMargin.Equals(value) != true))
                {
                    _profitMargin = value;
                    RaisePropertyChanged("ProfitMargin");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDirection")]
        public int? TblLkpDirection
        {
            get
            {
                return _tblLkpDirectionField;
            }
            set
            {
                if ((_tblLkpDirectionField.Equals(value) != true))
                {
                    if (_tblLkpDirectionField == value)
                    {
                        return;
                    }
                    _tblLkpDirectionField = value;
                    RaisePropertyChanged("TblLkpDirection");
                    UpdatedStyleCode();
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSeason")]
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
                    UpdatedStyleCode();
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFamily")]
        public int? TblFamily
        {
            get
            {
                return _tblFamilyField;
            }
            set
            {
                if ((_tblFamilyField.Equals(value) != true))
                {
                    if (_tblFamilyField == value)
                    {
                        return;
                    }
                    _tblFamilyField = value;
                    RaisePropertyChanged("TblFamily");
                    SubFamilyList.Clear();
                }
            }
        }

        private GenericTable _styleSpecGenericFabricPerRow;

        public GenericTable StyleSpecGenericFabricPerRow
        {
            get { return _styleSpecGenericFabricPerRow; }
            set { _styleSpecGenericFabricPerRow = value; RaisePropertyChanged("StyleSpecGenericFabricPerRow"); }
        }

        private GenericTable _styleCategoryPerRow;

        public GenericTable StyleCategoryPerRow
        {
            get { return _styleCategoryPerRow; }
            set { _styleCategoryPerRow = value; RaisePropertyChanged("StyleCategoryPerRow"); }
        }

        private GenericTable _styleTechPackStausPerRow;

        public GenericTable StyleTechPackStausPerRow
        {
            get { return _styleTechPackStausPerRow; }
            set { _styleTechPackStausPerRow = value; RaisePropertyChanged("StyleTechPackStausPerRow"); }
        }

        private tbl_FabricAttriputes _fabricPerRow;

        public tbl_FabricAttriputes FabricPerRow
        {
            get
            {
                return _fabricPerRow;
            }
            set
            {
                if ((ReferenceEquals(_fabricPerRow, value) != true))
                {
                    _fabricPerRow = value;
                    RaisePropertyChanged("FabricPerRow");
                    if (_fabricPerRow != null)
                    {
                        tbl_FabricAttriputes = _fabricPerRow.Iserial;
                    }
                }
            }
        }

        public LkpData.TblLkpBrandSection SectionPerRow
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

        public GenericTable DesignPerRow
        {
            get
            {
                return _designPerRow;
            }
            set
            {
                if ((ReferenceEquals(_designPerRow, value) != true))
                {
                    _designPerRow = value;
                    RaisePropertyChanged("DesignPerRow");
                }
            }
        }

        public GenericTable DirectionPerRow
        {
            get
            {
                return _directionPerRow;
            }
            set
            {
                if ((ReferenceEquals(_directionPerRow, value) != true))
                {
                    _directionPerRow = value;
                    RaisePropertyChanged("DirectionPerRow");
                }
            }
        }

        private GenericTable _styleThemePerRow;
        public GenericTable StyleThemePerRow
        {
            get
            {
                return _styleThemePerRow;
            }
            set
            {
                if ((ReferenceEquals(_styleThemePerRow, value) != true))
                {
                    _styleThemePerRow = value;
                    RaisePropertyChanged("StyleThemePerRow");
                }
            }
        }

        public TblLkpSeason SeasonPerRow
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
                }
            }
        }

        public TblSizeGroup SizeGroupPerRow
        {
            get
            {
                return _sizeGroupPerRow;
            }
            set
            {
                if ((ReferenceEquals(_sizeGroupPerRow, value) != true))
                {
                    _sizeGroupPerRow = value;
                    RaisePropertyChanged("SizeGroupPerRow");
                }
            }
        }

        public LkpData.TblFamily FamilyPerRow
        {
            get
            {
                return _familyPerRow;
            }
            set
            {
                if ((ReferenceEquals(_familyPerRow, value) != true))
                {
                    _familyPerRow = value;
                    RaisePropertyChanged("FamilyPerRow");
                }
            }
        }

        public LkpData.TblSubFamily SubFamilyPerRow
        {
            get
            {
                return _subFamilyPerRow;
            }
            set
            {
                if ((ReferenceEquals(_subFamilyPerRow, value) != true))
                {
                    _subFamilyPerRow = value;
                    RaisePropertyChanged("SubFamilyPerRow");
                }
            }
        }

        private ObservableCollection<LkpData.TblSubFamily> _subFamilyList;

        public ObservableCollection<LkpData.TblSubFamily> SubFamilyList
        {
            get { return _subFamilyList ?? (_subFamilyList = new ObservableCollection<LkpData.TblSubFamily>()); }
            set { _subFamilyList = value; RaisePropertyChanged("SubFamilyList"); }
        }

        private List<string> _repeatedStyleList;

        public List<string> RepeatedStyleList
        {
            get { return _repeatedStyleList ?? (_repeatedStyleList = new List<string>()); }
            set { _repeatedStyleList = value; RaisePropertyChanged("RepeatedStyleList"); }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSubFamily")]
        public int? TblSubFamily
        {
            get
            {
                return _tblSubFamilyField;
            }
            set
            {
                if ((_tblSubFamilyField.Equals(value) != true))
                {
                    if (_tblSubFamilyField == value)
                    {
                        return;
                    }
                    _tblSubFamilyField = value;
                    RaisePropertyChanged("TblSubFamily");
                    UpdatedStyleCode();
                }
            }
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

        private int _salesOrderToApprove;

        public int SalesOrderToApproveCount
        {
            get { return _salesOrderToApprove; }
            set { _salesOrderToApprove = value; RaisePropertyChanged("SalesOrderToApproveCount"); }
        }

        private byte[] _styleImage;

        public byte[] StyleImage
        {
            get { return _styleImage; }
            set { _styleImage = value; RaisePropertyChanged("StyleImage"); }
        }


        private string _galarylink;
        public string galarylink
        {
            get { return _galarylink; }
            set { _galarylink = value; RaisePropertyChanged("galarylink"); }
        }

        private string _ImageName;
        public string ImageName
        {
            get { return _ImageName; }
            set { _ImageName = value; RaisePropertyChanged("ImageName"); }
        }

        private void UpdatedStyleCode()
        {
            if (Finished)
            {
                if (SeasonPerRow != null && Brand != null && SectionPerRow != null && TblSubFamily != null &&
                    TblLkpDirection != null && SubFamilyPerRow != null && DirectionPerRow != null)
                {
                    Status = 100;
                    //   const char addedChar = '0';

                    //   StyleCode = Brand.PadLeft(2, addedChar) + SectionPerRow.Code.PadLeft(1, addedChar) +
                    //             SeasonPerRow.Code.PadLeft(3, addedChar) + DirectionPerRow.Code.PadLeft(1, addedChar) + SubFamilyPerRow.Code.PadLeft(4, addedChar);
                    if (Iserial != 0)
                    {
                        //   StyleCode = StyleCode + SerialNo;
                    }
                    if (tbl_lkp_FabricDesignes != null)
                    {
                        Description = DirectionPerRow.Aname + " " + FamilyPerRow.Ename + " " + SubFamilyPerRow.Ename + " " + DesignPerRow.Ename;
                        if (!string.IsNullOrWhiteSpace(AdditionalDescription))
                        {
                            Description = FamilyPerRow.Ename + " " + AdditionalDescription;
                        }
                        if (Description.Length > 50)
                        {
                            Description = Description.Remove(50);
                        }
                    }
                }
            }
        }

        public bool Finished { get; set; }
    }

    public class TblSalesOrderViewModel : PropertiesViewModelBase
    {
        private GenericTable _RetailOrderProductionTypePerRow;

        public GenericTable RetailOrderProductionTypePerRow
        {
            get { return _RetailOrderProductionTypePerRow; }
            set { _RetailOrderProductionTypePerRow = value; RaisePropertyChanged("RetailOrderProductionTypePerRow"); }
        }

        private double _FabricCost;

        public double FabricCost
        {
            get { return _FabricCost; }
            set { _FabricCost = value; RaisePropertyChanged("FabricCost"); }
        }
        private double _AccCost;

        public double AccCost
        {
            get { return _AccCost; }
            set { _AccCost = value; RaisePropertyChanged("AccCost"); }
        }

        private double _OperationCost;

        public double OperationCost
        {
            get { return _OperationCost; }
            set { _OperationCost = value; RaisePropertyChanged("OperationCost"); }
        }
        private int? _TblRetailOrderProductionType;

        public int? TblRetailOrderProductionType
        {
            get { return _TblRetailOrderProductionType; }
            set { _TblRetailOrderProductionType = value; RaisePropertyChanged("TblRetailOrderProductionType"); }
        }

        private ObservableCollection<TblSalesOrderNotesModel> _SalesOrderNotesList;

        public ObservableCollection<TblSalesOrderNotesModel> SalesOrderNotesList
        {
            get { return _SalesOrderNotesList ?? (_SalesOrderNotesList = new ObservableCollection<TblSalesOrderNotesModel>()); }
            set { _SalesOrderNotesList = value; RaisePropertyChanged("SalesOrderNotesList"); }
        }

        private int _TblSalesPerson;

        public int TblSalesPerson
        {
            get { return _TblSalesPerson; }
            set { _TblSalesPerson = value; RaisePropertyChanged("TblSalesPerson"); }
        }

        private int? _TblStyleTNAHeader;

        public int? TblStyleTNAHeader
        {
            get { return _TblStyleTNAHeader; }
            set { _TblStyleTNAHeader = value; RaisePropertyChanged("TblStyleTNAHeader"); }
        }

        private TblStyleTNAHeaderViewModel _StyleTnaHeaderForRow;

        public TblStyleTNAHeaderViewModel StyleTnaHeaderForRow
        {
            get { return _StyleTnaHeaderForRow; }
            set { _StyleTnaHeaderForRow = value; RaisePropertyChanged("StyleTnaHeaderForRow"); }
        }


        public TblSalesOrderViewModel()
        {
            SalesOrderColorList = new ObservableCollection<TblSalesOrderColorViewModel>();
            SalesOrderColorList.CollectionChanged += SalesOrderColorList_CollectionChanged;
        }

        private bool _isSample;

        public bool IsSample
        {
            get { return _isSample; }
            set { _isSample = value; RaisePropertyChanged("IsSample"); }
        }

        private bool _readyProduct;

        public bool ReadyProduct
        {
            get { return _readyProduct; }
            set { _readyProduct = value; RaisePropertyChanged("ReadyProduct"); }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        private string _additionalNotes;

        public string AdditionalNotes
        {
            get { return _additionalNotes; }
            set { _additionalNotes = value; RaisePropertyChanged("AdditionalNotes"); }
        }
        private ObservableCollection<TblRequestForSampleEventViewModel> _subEventList;

        public ObservableCollection<TblRequestForSampleEventViewModel> SubEventList
        {
            get
            {
                return _subEventList ?? (_subEventList = new ObservableCollection<TblRequestForSampleEventViewModel>());
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

        public ObservableCollection<TblSeasonalMasterList> SeasonalMasterList
        {
            get
            {
                return _seasonalMasterList ??
                    (_seasonalMasterList = new ObservableCollection<TblSeasonalMasterList>());
            }
            set { _seasonalMasterList = value; RaisePropertyChanged("SeasonalMasterList"); }
        }

        private void SalesOrderColorList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSalesOrderColorViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSalesOrderColorViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "ThemePerRow":

                    if (SalesOrderColorList.Count(x => x.DeliveryDate >= DeliveryDate) > 1)
                    {
                        //MessageBox.Show("Color Delivery Date can't Be Before The Delivery Date Of the PO");
                    }
                    break;
            }
        }

        private ObservableCollection<TblRequestForSample> _sampleRequest;

        public ObservableCollection<TblRequestForSample> SampleRequestList
        {
            get { return _sampleRequest ?? (_sampleRequest = new ObservableCollection<TblRequestForSample>()); }
            set { _sampleRequest = value; RaisePropertyChanged("SampleRequestList"); }
        }

        private TblRequestForSample _requestforSamplePerRow;

        public TblRequestForSample RequestforSamplePerRow
        {
            get { return _requestforSamplePerRow; }
            set { _requestforSamplePerRow = value; RaisePropertyChanged("RequestforSamplePerRow"); }
        }

        public bool ContainColors { get; set; }

        private ApprovalOptions _approvalOptions;

        public ApprovalOptions ApprovalOptions
        {
            get { return _approvalOptions; }
            set { _approvalOptions = value; RaisePropertyChanged("ApprovalOptions"); }
        }

        private TblApprovalStatu _approvalStatus;

        public TblApprovalStatu ApprovalStatusPerRow
        {
            get { return _approvalStatus; }
            set
            {
                _approvalStatus = value;
                RaisePropertyChanged("ApprovalStatusPerRow");
            }
        }

        private bool _isPlannedOrderField;

        public bool IsPlannedOrder
        {
            get
            {
                return _isPlannedOrderField;
            }
            set
            {
                if ((_isPlannedOrderField.Equals(value) != true))
                {
                    _isPlannedOrderField = value;
                    RaisePropertyChanged("IsPlannedOrder");
                }
            }
        }

        private string _createdByField;

        private DateTime? _creationDateField;

        private string _customerField;

        private bool? _isPostedOnAxaptaField;

        private int _iserialField;

        private string _lastUpdatedByField;

        private DateTime? _lastUpdatedDateField;

        private string _serialNoField;

        private int _statusField;

        private int _tblStyleField;

        private int? _tblFactoryGroup;

        public int? TblFactoryGroup
        {
            get { return _tblFactoryGroup; }
            set { _tblFactoryGroup = value; RaisePropertyChanged("TblFactoryGroup"); }
        }

        private int? _tblComplexityGroup;

        public int? TblComplexityGroup
        {
            get { return _tblComplexityGroup; }
            set { _tblComplexityGroup = value; RaisePropertyChanged("TblComplexityGroup"); }
        }
        private ObservableCollection<TblSalesOrderOperationViewModel> _salesOrderOperationList;

        public ObservableCollection<TblSalesOrderOperationViewModel> SalesOrderOperationList
        {
            get { return _salesOrderOperationList ?? (_salesOrderOperationList = new ObservableCollection<TblSalesOrderOperationViewModel>()); }
            set { _salesOrderOperationList = value; RaisePropertyChanged("SalesOrderOperationList"); }
        }

        private ObservableCollection<BomViewModel> _bomList;

        public ObservableCollection<BomViewModel> BomList
        {
            get { return _bomList ?? (_bomList = new ObservableCollection<BomViewModel>()); }
            set { _bomList = value; RaisePropertyChanged("BomList"); }
        }

        private ObservableCollection<TechPackBomViewModel> _techPackbomList;

        public ObservableCollection<TechPackBomViewModel> TechPackBomList
        {
            get { return _techPackbomList ?? (_techPackbomList = new ObservableCollection<TechPackBomViewModel>()); }
            set { _techPackbomList = value; RaisePropertyChanged("TechPackBomList"); }
        }

        private int? _tblRequestForSample;

        public int? TblRequestForSample
        {
            get { return _tblRequestForSample; }
            set { _tblRequestForSample = value; RaisePropertyChanged("TblRequestForSample"); }
        }

        [ReadOnly(true)]
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
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        private int? _poRef;

        public int? PoRef
        {
            get { return _poRef; }
            set { _poRef = value; RaisePropertyChanged("PoRef"); }
        }

        private int _tblSupplier;

        public int TblSupplier
        {
            get
            {
                return _tblSupplier;
            }
            set
            {
                if ((Equals(_tblSupplier, value) != true))
                {
                    _tblSupplier = value;
                    RaisePropertyChanged("TblSupplier");
                }
            }
        }

        [ReadOnly(true)]
        public string Customer
        {
            get
            {
                return _customerField;
            }
            set
            {
                if ((ReferenceEquals(_customerField, value) != true))
                {
                    _customerField = value;
                    RaisePropertyChanged("Customer");
                }
            }
        }

        private ObservableCollection<TBLsupplier> _supplier;

        public ObservableCollection<TBLsupplier> Suppliers
        {
            get { return _supplier; }
            set { _supplier = value; RaisePropertyChanged("Suppliers"); }
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
                    if (SupplierPerRow != null)
                    {
                        TblSupplier = SupplierPerRow.Iserial;
                    }
                }
            }
        }

        public bool? IsPostedOnAxapta
        {
            get
            {
                return _isPostedOnAxaptaField;
            }
            set
            {
                if ((_isPostedOnAxaptaField.Equals(value) != true))
                {
                    _isPostedOnAxaptaField = value;
                    RaisePropertyChanged("IsPostedOnAxapta");
                }
            }
        }

        private int _salesOrderType;

        public int SalesOrderType
        {
            get
            {
                return _salesOrderType;
            }
            set
            {
                if ((_salesOrderType.Equals(value) != true))
                {
                    _salesOrderType = value;
                    RaisePropertyChanged("SalesOrderType");
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

        [ReadOnly(true)]
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

        private ObservableCollection<TblSalesOrderColorViewModel> _salesOrderColors;

        public ObservableCollection<TblSalesOrderColorViewModel> SalesOrderColorList
        {
            get { return _salesOrderColors; }
            set { _salesOrderColors = value; RaisePropertyChanged("SalesOrderColorList"); }
        }

        private string _salesOrderCode;

        [ReadOnly(true)]
        public string SalesOrderCode
        {
            get { return _salesOrderCode; }
            set { _salesOrderCode = value; RaisePropertyChanged("SalesOrderCode"); }
        }

        private DateTime? _delivaryDateField;
        private ObservableCollection<TblSeasonalMasterList> _seasonalMasterList;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
        public DateTime? DeliveryDate
        {
            get
            {
                return _delivaryDateField;
            }
            set
            {
                if ((_delivaryDateField.Equals(value) != true))
                {
                    _delivaryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }


        private bool _isRetailApproved;

        [ReadOnly(true)]
        public bool IsRetailApproved
        {
            get { return _isRetailApproved; }
            set { _isRetailApproved = value; RaisePropertyChanged("IsRetailApproved"); }
        }

        private bool _isTechnicalApproved;

        [ReadOnly(true)]
        public bool IsTechnicalApproved
        {
            get { return _isTechnicalApproved; }
            set { _isTechnicalApproved = value; RaisePropertyChanged("IsTechnicalApproved"); }
        }

        private bool _isFinancialApproved;

        [ReadOnly(true)]
        public bool IsFinancialApproved
        {
            get { return _isFinancialApproved; }
            set { _isFinancialApproved = value; RaisePropertyChanged("IsFinancialApproved"); }
        }
    }

    public class TblTechPackBOMCopyViewModel : PropertiesViewModelBase
    {

    }
    public class TblSalesOrderColorViewModel : PropertiesViewModelBase
    {
        public TblSalesOrderColorViewModel()
        {
            SalesOrderSizeRatiosList = new ObservableCollection<TblSalesOrderSizeRatio>();
            SalesOrderSizeRatiosList.CollectionChanged += SalesOrderSizeRatiosList_CollectionChanged;
        }

        private void SalesOrderSizeRatiosList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSalesOrderSizeRatio item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSalesOrderSizeRatio item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "ProductionPerSize":
                    if (ManualCalculation)
                    {
                        // ReSharper disable once RedundantCheckBeforeAssignment
                        if (Total != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                        {
                            Total = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize);
                        }

                        foreach (var row in SalesOrderSizeRatiosList)
                        {
                            if (Total > 0 && row.ProductionPerSize > 0)
                            {
                                var result = ((double)row.ProductionPerSize / Total);

                                // ReSharper disable once RedundantCheckBeforeAssignment
                                // ReSharper disable once CompareOfFloatsByEqualityOperator
                                if (row.Ratio != Math.Round(result, 2) * 10)
                                {
                                    row.Ratio = Math.Round(result, 2) * 10;
                                }
                            }
                        }
                    }
                    break;

                case "ProductionPerSizeForProduction":
                    if (ManualCalculationForProduction)
                    {
                        if (TotalForProduction != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction))
                        {
                            TotalForProduction = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction);
                        }
                        try
                        {
                            var min = SalesOrderSizeRatiosList.Where(x => x.ProductionPerSizeForProduction > 0)
                              .OrderBy(x => x.ProductionPerSizeForProduction)
                              .FirstOrDefault()
                              .ProductionPerSizeForProduction;

                            foreach (var row in SalesOrderSizeRatiosList)
                            {
                                if (TotalForProduction > 0 && row.ProductionPerSizeForProduction > 0)
                                {
                                    if (row.ProductionPerSizeForProduction == min)
                                    {
                                        row.RatioForProduction = 1;
                                    }
                                    else
                                    {
                                        row.RatioForProduction = (double)row.ProductionPerSizeForProduction / min;
                                    }
                                }
                                else
                                {
                                    row.RatioForProduction = 0;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;

                case "Ratio":

                    if (!ManualCalculation)
                    {
                        foreach (var row in SalesOrderSizeRatiosList)
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (row != null && row.Ratio == 0)
                            {
                                row.ProductionPerSize = 0;
                            }
                            if (row != null && (Total > 0 && row.Ratio > 0))
                            {
                                var sum = SalesOrderSizeRatiosList.Sum(x => x.Ratio);

                                if (row.ProductionPerSize != Convert.ToInt32(Total * (row.Ratio / sum)))
                                {
                                    row.ProductionPerSize = Convert.ToInt32(Total * (row.Ratio / sum));
                                }
                            }
                        }
                        if (Total != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                        {
                            if (Total > 50)
                            {


                                var Largest = SalesOrderSizeRatiosList.OrderByDescending(w => w.Ratio).FirstOrDefault();
                                if (Total < SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                                {
                                    Largest.ProductionPerSize = Largest.ProductionPerSize - (SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize) - Total);
                                }
                                else if (Total > SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                                {
                                    Largest.ProductionPerSize = Largest.ProductionPerSize + (Total - SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize));

                                }
                            }
                            else
                            {
                                Total = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize);
                            }
                        }
                    }

                    break;

                case "RatioForProduction":

                    if (!ManualCalculationForProduction)
                    {
                        foreach (var row in SalesOrderSizeRatiosList)
                        {
                            if (row.RatioForProduction == 0)
                            {
                                row.ProductionPerSizeForProduction = 0;
                            }
                            if (TotalForProduction > 0 && row.RatioForProduction > 0)
                            {
                                var sum = SalesOrderSizeRatiosList.Sum(x => x.RatioForProduction);

                                if (row.ProductionPerSizeForProduction != Convert.ToInt32(TotalForProduction * (row.RatioForProduction / sum)))
                                {
                                    row.ProductionPerSizeForProduction = Convert.ToInt32(TotalForProduction * (row.RatioForProduction / sum));
                                }
                            }
                        }
                        if (TotalForProduction != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction))
                        {
                            TotalForProduction = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction);
                        }
                    }

                    break;
            }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        private bool _RequestForCancelApprove;

        public bool RequestForCancelApprove
        {
            get { return _RequestForCancelApprove; }
            set { _RequestForCancelApprove = value; RaisePropertyChanged("RequestForCancelApprove"); }
        }


        private decimal? _costField;

        public decimal? Cost
        {
            get { return _costField; }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
                    LocalCost = ExchangeRate * value;
                }
            }
        }

        private TblCurrency _currencyPerRow;

        public TblCurrency CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (CurrencyPerRow != null)
                {
                    if (CurrencyPerRow.LocalCurrency == true)
                    {
                        ExchangeRate = 1;
                    }
                    TblCurrency = CurrencyPerRow.Iserial;

                    //    Client.ConvertCurrencyAsync(_currencyPerRow.Code, "EGP", 1);
                }

                //string url = string.Format("http://rate-exchange.appspot.com/currency?from={0}&to={1}", CurrencyPerRow.Code, "EGP");
                //WebClient client = new WebClient();
                //string rates = client.DownloadStringAsync(url);
                //client.DownloadStringCompleted += (s, sv) =>
                //{
                //};
                //Rate rate = new JavaScriptSerializer().Deserialize<Rate>(rates);
                //double converted_amount = amount * rate.rate;
                //string message = ddlFrom.SelectedItem.Value + ": " + amount + "\\n";
                //message += ddlTo.SelectedItem.Value + ": " + converted_amount + "\\n";
                //message += "Rate: 1 " + ddlFrom.SelectedItem.Value + " = " + rate.rate + " " + ddlTo.SelectedItem.Value;
            }
        }

        private int? _tblCurrency;

        public int? TblCurrency
        {
            get { return _tblCurrency; }
            set { _tblCurrency = value; RaisePropertyChanged("TblCurrency"); }
        }

        private decimal? _additionalCostField;

        public decimal? AdditionalCost
        {
            get
            {
                return _additionalCostField;
            }
            set
            {
                if ((_additionalCostField.Equals(value) != true))
                {
                    _additionalCostField = value;
                    RaisePropertyChanged("AdditionalCost");
                }
            }
        }

        private decimal? _exchangeRateField;

        public decimal? ExchangeRate
        {
            get
            {
                return _exchangeRateField;
            }
            set
            {
                if ((_exchangeRateField.Equals(value) != true))
                {
                    _exchangeRateField = value;
                    RaisePropertyChanged("ExchangeRate");
                    LocalCost = Cost * value;
                }
            }
        }

        private decimal? _localCostField;

        [ReadOnly(true)]
        public decimal? LocalCost
        {
            get
            {
                return _localCostField;
            }
            set
            {
                if ((_localCostField.Equals(value) != true))
                {
                    _localCostField = value;
                    RaisePropertyChanged("LocalCost");
                }
            }
        }

        private DateTime? _delivaryDateField;

        private int _iserialField;

        private bool _manualCalculationField;
        private bool _manualCalculationForProduction;
        private int? _tblColorField;

        private int _tblSalesOrderField;

        private TblSalesOrderColorTheme _tblSalesOrderColorThemeField;

        private int? _tblSalesOrerColorThemeField;

        private int _totalField;

        private int _totalForProductionField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
        public DateTime? DeliveryDate
        {
            get
            {
                return _delivaryDateField;
            }
            set
            {
                if ((_delivaryDateField.Equals(value) != true))
                {
                    _delivaryDateField = value;
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

        public bool ManualCalculationForProduction
        {
            get
            {
                return _manualCalculationForProduction;
            }
            set
            {
                if ((_manualCalculationForProduction.Equals(value) != true))
                {
                    _manualCalculationForProduction = value;
                    RaisePropertyChanged("ManualCalculationForProduction");
                }
            }
        }

        public bool ManualCalculation
        {
            get
            {
                return _manualCalculationField;
            }
            set
            {
                if ((_manualCalculationField.Equals(value) != true))
                {
                    _manualCalculationField = value;
                    RaisePropertyChanged("ManualCalculation");
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
                if ((Equals(_tblColorField, value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public int TblSalesOrder
        {
            get
            {
                return _tblSalesOrderField;
            }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

        public TblSalesOrderColorTheme ThemePerRow
        {
            get
            {
                return _tblSalesOrderColorThemeField;
            }
            set
            {
                if ((ReferenceEquals(_tblSalesOrderColorThemeField, value) != true))
                {
                    _tblSalesOrderColorThemeField = value;
                    RaisePropertyChanged("ThemePerRow");
                    if (TblSalesOrderColorTheme != null && _tblSalesOrerColorThemeField != 0 && Iserial == 0)
                    {
                        if (ThemePerRow != null) DeliveryDate = ThemePerRow.DeliveryDate;
                    }
                }
            }
        }

        private ObservableCollection<TblSalesOrderSizeRatio> _tblSalesOrderSizeRatiosField;

        public ObservableCollection<TblSalesOrderSizeRatio> SalesOrderSizeRatiosList
        {
            get
            {
                return _tblSalesOrderSizeRatiosField;
            }
            set
            {
                if ((ReferenceEquals(_tblSalesOrderSizeRatiosField, value) != true))
                {
                    _tblSalesOrderSizeRatiosField = value;
                    RaisePropertyChanged("SalesOrderSizeRatiosList");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqTheme")]
        public int? TblSalesOrderColorTheme
        {
            get
            {
                return _tblSalesOrerColorThemeField;
            }
            set
            {
                if ((_tblSalesOrerColorThemeField.Equals(value) != true))
                {
                    _tblSalesOrerColorThemeField = value;
                    RaisePropertyChanged("TblSalesOrderColorTheme");
                }
            }
        }

        public int TotalForProduction
        {
            get
            {
                return _totalForProductionField;
            }
            set
            {
                if ((_totalForProductionField.Equals(value) != true))
                {
                    _totalForProductionField = value;
                    RaisePropertyChanged("TotalForProduction");
                    if (!ManualCalculationForProduction)
                    {
                        foreach (var row in SalesOrderSizeRatiosList)
                        {
                            if (TotalForProduction > 0 && row.RatioForProduction > 0)
                            {
                                var sum = SalesOrderSizeRatiosList.Sum(x => x.RatioForProduction);
                                if (row.ProductionPerSize != Convert.ToInt32(TotalForProduction * (row.RatioForProduction / sum)))
                                {
                                    row.ProductionPerSize = Convert.ToInt32(TotalForProduction * (row.RatioForProduction / sum));
                                }
                            }
                        }
                        if (SalesOrderSizeRatiosList.Any())
                        {
                            if (TotalForProduction != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction))
                            {
                                TotalForProduction = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSizeForProduction);
                            }

                            RaisePropertyChanged("TotalForProduction");
                        }
                    }
                }
            }
        }

        public int Total
        {
            get
            {
                return _totalField;
            }
            set
            {
                if ((_totalField.Equals(value) != true))
                {
                    _totalField = value;
                    RaisePropertyChanged("Total");
                    if (!ManualCalculation)
                    {
                        foreach (var row in SalesOrderSizeRatiosList)
                        {
                            if (Total > 0 && row.Ratio > 0)
                            {
                                var sum = SalesOrderSizeRatiosList.Sum(x => x.Ratio);
                                if (row.ProductionPerSize != Convert.ToInt32(Total * (row.Ratio / sum)))
                                {
                                    row.ProductionPerSize = Convert.ToInt32(Total * (row.Ratio / sum));
                                }
                            }
                        }
                        if (SalesOrderSizeRatiosList.Any())
                        {
                            if (Total != SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                            {
                                if (Total > 50)
                                {


                                    var Largest = SalesOrderSizeRatiosList.OrderByDescending(w => w.Ratio).FirstOrDefault();
                                    if (Total < SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                                    {
                                        Largest.ProductionPerSize = Largest.ProductionPerSize - (SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize) - Total);
                                    }
                                    else if (Total > SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize))
                                    {
                                        Largest.ProductionPerSize = Largest.ProductionPerSize + (Total - SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize));

                                    }
                                }
                                else
                                {
                                    Total = SalesOrderSizeRatiosList.Sum(x => x.ProductionPerSize);
                                }
                            }

                            RaisePropertyChanged("Total");
                        }
                    }
                }
            }
        }

        private TblSeasonalMasterList _seasonalMasterListPerRow;

        public TblSeasonalMasterList SeasonalMasterPerRow
        {
            get { return _seasonalMasterListPerRow; }
            set
            {
                _seasonalMasterListPerRow = value;

                RaisePropertyChanged("SeasonalMasterPerRow");
                if (_seasonalMasterListPerRow != null)
                {
                    SalesOrderSizeRatiosList.Clear();
                    if (SeasonalMasterPerRow.TblSeasonalMasterListDetails != null)
                    {
                        foreach (var tblSeasonalMasterListDetail in SeasonalMasterPerRow.TblSeasonalMasterListDetails)
                        {
                            tblSeasonalMasterListDetail.Iserial = 0;
                            tblSeasonalMasterListDetail.EntityKey = null;
                        }
                        GenericMapper.InjectFromObCollection(SalesOrderSizeRatiosList, SeasonalMasterPerRow.TblSeasonalMasterListDetails);
                    }

                    if (_seasonalMasterListPerRow.Qty != 0)
                    {
                        Total = _seasonalMasterListPerRow.Qty;
                    }
                    TblSalesOrderColorTheme = _seasonalMasterListPerRow.TblSalesOrderColorTheme;
                    DeliveryDate = _seasonalMasterListPerRow.DelivaryDate;
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
    }

    public class TblSalesOrderOperationViewModel : PropertiesViewModelBase
    {
        private int _iserialField;

        private float _oprCostField;

        private int _tblSalesOrderField;

        private double _rowIndexField;

        [ReadOnly(true)]
        public double RowIndex
        {
            get
            {
                return _rowIndexField;
            }
            set
            {
                if ((_rowIndexField.Equals(value) != true))
                {
                    _rowIndexField = value;
                    RaisePropertyChanged("RowIndex");
                }
            }
        }

        private int? _tblOperationField;

        private TblRouteGroup _route;

        public TblRouteGroup RouteGroupPerRow
        {
            get { return _route; }
            set { _route = value; RaisePropertyChanged("RouteGroupPerRow"); }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqOperation")]
        public int? TblOperation
        {
            get
            {
                return _tblOperationField;
            }
            set
            {
                if ((_tblOperationField.Equals(value) != true))
                {
                    _tblOperationField = value;
                    RaisePropertyChanged("TblOperation");
                }
            }
        }

        public float OprCost
        {
            get
            {
                return _oprCostField;
            }
            set
            {
                if ((_oprCostField.Equals(value) != true))
                {
                    _oprCostField = value;
                    RaisePropertyChanged("OprCost");

                    if (TblSalesOrderOperationDetailList != null)
                        foreach (var variable in TblSalesOrderOperationDetailList)
                        {
                            if (variable.OprCost == 0)
                            {
                                variable.OprCost = OprCost;
                            }
                        }
                }
            }
        }

        private ObservableCollection<TblSalesOrderOperationDetail> _tblSalesOrderOperationDetailList;

        public ObservableCollection<TblSalesOrderOperationDetail> TblSalesOrderOperationDetailList
        {
            get { return _tblSalesOrderOperationDetailList; }
            set { _tblSalesOrderOperationDetailList = value; RaisePropertyChanged("TblSalesOrderOperationDetailList"); }
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

        public int TblSalesOrder
        {
            get
            {
                return _tblSalesOrderField;
            }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }
    }

    public class BomStyleColorViewModel : PropertiesViewModelBase
    {
        private int? _fabricColorField;

        private int _iserialField;

        private int _styleColorField;

        private TblColor _tblColorField;

        private TblColor _tblColor1Field;

        private TblColor _tblColor2Field;

        private int _bomField;

        public int Bom
        {
            get
            {
                return _bomField;
            }
            set
            {
                if ((_bomField.Equals(value) != true))
                {
                    _bomField = value;
                    RaisePropertyChanged("Bom");
                }
            }
        }

        private int? _dummyColorField;

        public int? DummyColor
        {
            get
            {
                return _dummyColorField;
            }
            set
            {
                if ((_dummyColorField.Equals(value) != true))
                {
                    _dummyColorField = value;
                    RaisePropertyChanged("DummyColor");
                }
            }
        }

        private int? _dyedColor;

        public int? DyedColor
        {
            get
            {
                return _dyedColor;
            }
            set
            {
                if ((_dyedColor.Equals(value) != true))
                {
                    _dyedColor = value;
                    RaisePropertyChanged("DyedColor");
                }
            }
        }

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

        public TblColor TblColor
        {
            get
            {
                return _tblColorField;
            }
            set
            {
                if ((ReferenceEquals(_tblColorField, value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public TblColor TblColor1
        {
            get
            {
                return _tblColor1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblColor1Field, value) != true))
                {
                    _tblColor1Field = value;
                    RaisePropertyChanged("TblColor1");
                }
            }
        }

        public TblColor TblColor2
        {
            get
            {
                return _tblColor2Field;
            }
            set
            {
                if ((ReferenceEquals(_tblColor2Field, value) != true))
                {
                    _tblColor2Field = value;
                    RaisePropertyChanged("TblColor2");
                }
            }
        }
    }

    public class BomViewModel : PropertiesViewModelBase
    {
        
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public BomViewModel()
        {
            isBom = true;
            BomSizes = new ObservableCollection<TblBOMSize>();
            BomSizes.CollectionChanged += BomSizes_CollectionChanged;
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;

                var tblAccessoryAttributesDetails = sv.Result.CombinationList.FirstOrDefault();
                if (tblAccessoryAttributesDetails != null)
                    ItemPerRow.SizesList.Add(tblAccessoryAttributesDetails.Size);
                //if (BomSizes != null)
                //    foreach (var tblBomSize in BomSizes)
                //    {
                //        tblBomSize.FabricSize = ItemPerRow.SizesList.FirstOrDefault();
                //    }

                if (BomStyleColors != null)
                    foreach (var variable in BomStyleColors)
                    {
                        variable.TblColor = variable.TblColor;
                        variable.StyleColor = variable.StyleColor;
                        variable.FabricColor = variable.FabricColor;
                        variable.DummyColor = variable.DummyColor;
                    }

                if (BomSizes != null)
                    foreach (var variable in BomSizes)
                    {
                        variable.FabricSize = variable.FabricSize;
                    }

                if (ItemPerRow.CombinationList != null)
                {
                    ItemPerRow.SizesList = new ObservableCollection<string>();

                    if (BomStyleColors != null)
                    {
                        var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                        var sizes =
                            ItemPerRow.CombinationList.Where(
                                x => (color != null && color.Contains(x.Configuration)));

                        if (sizes != null)
                        {
                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    var sizes =
                        ItemPerRow.CombinationList.Where(
                            x => (color != null && color.Contains(x.Configuration)));

                    if (sizes != null)
                    {
                        var distinctsize = sizes.Select(x => x.Size);

                        foreach (var size in distinctsize)
                        {
                            var row = sizes.Count(x => x.Size == size);

                            if (row == color.Count())
                            {
                                if (!ItemPerRow.SizesList.Contains(size))
                                {
                                    ItemPerRow.SizesList.Add(size);
                                }
                            }
                        }
                    }
                }
            };
        }


        private bool _isBom;

        public bool isBom
        {
            get { return _isBom; }
            set { _isBom = value; RaisePropertyChanged("isBom"); }
        }

        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set { _colorPerRow = value; RaisePropertyChanged("ColorPerRow"); }
        }

        private int _tblColor;

        public int TblColor
        {
            get { return _tblColor; }
            set
            {
                _tblColor = value; RaisePropertyChanged("TblColor");
                ColorChangedStandardBom();
            }
        }

        private string _fabricSize;

        public string FabricSize
        {
            get { return _fabricSize; }
            set { _fabricSize = value; RaisePropertyChanged("FabricSize"); }
        }

        private void BomSizes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblBOMSize item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }

            if (e.OldItems != null)
                foreach (TblBOMSize item in e.OldItems)
                {
                    if (item == BomSizes.FirstOrDefault())
                    {
                        if (item != null) item.PropertyChanged += item_PropertyChanged;
                    }
                }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "FabricSize":

                    if (BomSizes != null)
                        foreach (var row in BomSizes)
                        {
                            if (string.IsNullOrWhiteSpace(row.FabricSize))
                            {
                                var firstOrDefault = BomSizes.FirstOrDefault();
                                if (firstOrDefault != null)
                                    row.FabricSize = firstOrDefault.FabricSize;
                            }
                        }

                    break;
            }
        }

        private bool _gotPlan;
        
        public bool GotPlan
        {
            get { return _gotPlan; }
            set
            {
                _gotPlan = value; RaisePropertyChanged("GotPlan");
                if (GotPlan)
                {
                    Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private bool _GotPurchaseRequest;

        public bool GotPurchaseRequest
        {
            get { return _GotPurchaseRequest; }
            set
            {
                _GotPurchaseRequest = value; RaisePropertyChanged("GotPurchaseRequest");
                if (GotPurchaseRequest)
                {
                    Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private bool _onStock;

        public bool OnStock
        {
            get { return _onStock; }
            set { _onStock = value; RaisePropertyChanged("OnStock"); }
        }

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value;
                if (value != null)
                {
                    Vendor = value.vendor_code;
                    RaisePropertyChanged("VendorPerRow");
                }
            }
        }

        private string _vendor;

        public string Vendor
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                RaisePropertyChanged("Vendor");
            }
        }

        private SolidColorBrush _foreground;

        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; RaisePropertyChanged("Foreground"); }
        }

        private GenericTable _bomCalcMethodField;

        private GenericTable _bomFabricTypePerRowField;


        private DateTime? _bomDateAddedField;

        private int _bomFabricField;

        private int? _bomFabricRoutField;

        private string _bomFabricTypeField;



        private bool _bomIsLocalProductionField;

        private bool _bomIsMainFabricField;

        private string _bomNotesField;

        private int _calcMethodField;




        private int _iserialField;

        private ObservableCollection<TblBOMSize> _bomSizes;

        private ObservableCollection<BomStyleColorViewModel> _bomStyleColors;

        private int _tblSalesOrderField;

        private string _uoMidField;

        private bool _isSupplierMaterialField;

        public bool IsSupplierMaterial
        {
            get
            {
                return _isSupplierMaterialField;
            }
            set
            {
                if ((_isSupplierMaterialField.Equals(value) != true))
                {
                    _isSupplierMaterialField = value;
                    RaisePropertyChanged("IsSupplierMaterial");
                }
            }
        }

        private TblSalesOrderOperationViewModel _routePerRow;

        public TblSalesOrderOperationViewModel RoutePerRow
        {
            get { return _routePerRow; }
            set
            {
                _routePerRow = value; RaisePropertyChanged("RoutePerRow");
                if (RoutePerRow != null) BOM_FabricRout = RoutePerRow.TblOperation;
            }
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
                    BOM_FabricType = ItemPerRow.ItemGroup;
                    ItemName = ItemPerRow.Name;
                    BOM_Fabric = ItemPerRow.Iserial;
                    if (ItemPerRow.ItemGroup != null) IsAcc = ItemPerRow.ItemGroup.Contains("Acc");
                    if (IsAcc)
                    {
                        _client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
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

        private bool _dyed;

        public bool Dyed
        {
            get { return _dyed; }
            set { _dyed = value; RaisePropertyChanged("Dyed"); }
        }

        private IEnumerable<string> _configrationList;

        public IEnumerable<string> ConfigrationList
        {
            get { return _configrationList; }
            set { _configrationList = value; RaisePropertyChanged("ConfigrationList"); }
        }

        private List<string> _sizesList;

        public List<string> SizesList
        {
            get { return _sizesList; }
            set { _sizesList = value; RaisePropertyChanged("SizesList"); }
        }

        private string _itemName;

        [ReadOnly(true)]
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if ((ReferenceEquals(_itemName, value) != true))
                {
                    _itemName = value;
                    RaisePropertyChanged("ItemName");
                }
            }
        }

        public GenericTable BOM_CalcMethodPerRow
        {
            get
            {
                return _bomCalcMethodField;
            }
            set
            {
                if ((ReferenceEquals(_bomCalcMethodField, value) != true))
                {
                    _bomCalcMethodField = value;
                    RaisePropertyChanged("BOM_CalcMethodPerRow");
                    //if (BOM_CalcMethodPerRow != null) CalcMethod = BOM_CalcMethodPerRow.Iserial;
                    //if (_calcMethodField == 1)
                    //{
                    //    if (BomStyleColors != null)
                    //        foreach (var row in BomStyleColors)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                row.DummyColor = BomStyleColors.FirstOrDefault().DummyColor;
                    //            }
                    //            else
                    //            {
                    //                row.FabricColor = BomStyleColors.FirstOrDefault().FabricColor;
                    //            }
                    //        }
                    //    ColorEnabled = false;
                    //}
                    //else if (_calcMethodField == 2)
                    //{
                    //    ColorEnabled = true;
                    //    if (BomStyleColors != null)
                    //        foreach (var row in BomStyleColors)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                if (ItemPerRow.AccConfigList != null)
                    //                {
                    //                    var firstOrDefault = ItemPerRow.AccConfigList.FirstOrDefault(x => row.TblColor1 != null && x.Code == row.TblColor1.Code);
                    //                    if (firstOrDefault != null)
                    //                        row.DummyColor = firstOrDefault.Iserial;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                row.FabricColor = row.StyleColor;
                    //            }
                    //        }
                    //}
                    //else if (_calcMethodField == 3)
                    //{
                    //    ColorEnabled = true;
                    //    if (BomSizes != null)
                    //        foreach (var row in BomSizes)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                if (ItemPerRow.SizesList.Contains(row.StyleSize))
                    //                {
                    //                    row.FabricSize = row.StyleSize;
                    //                }

                    //                //        row.DummyColor = BomStyleColors.FirstOrDefault().StyleColor;
                    //            }
                    //        }
                    //}
                }
            }
        }


        public GenericTable BOM_FabricTypePerRow
        {
            get
            {
                return _bomFabricTypePerRowField;
            }
            set
            {
                if ((ReferenceEquals(_bomFabricTypePerRowField, value) != true))
                {
                    _bomFabricTypePerRowField = value;
                    RaisePropertyChanged("BOM_FabricTypePerRow");
                }
            }
        }

        public DateTime? BOM_DateAdded
        {
            get
            {
                return _bomDateAddedField;
            }
            set
            {
                if ((_bomDateAddedField.Equals(value) != true))
                {
                    _bomDateAddedField = value;
                    RaisePropertyChanged("BOM_DateAdded");
                }
            }
        }

        public int BOM_Fabric
        {
            get
            {
                return _bomFabricField;
            }
            set
            {
                if ((Equals(_bomFabricField, value) != true))
                {
                    _bomFabricField = value;
                    RaisePropertyChanged("BOM_Fabric");
                }
            }
        }

        public int? BOM_FabricRout
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
                    RaisePropertyChanged("BOM_FabricRout");
                }
            }
        }

        [ReadOnly(true)]
        public string BOM_FabricType
        {
            get
            {
                return _bomFabricTypeField;
            }
            set
            {
                if ((ReferenceEquals(_bomFabricTypeField, value) != true))
                {
                    _bomFabricTypeField = value;
                    RaisePropertyChanged("BOM_FabricType");
                }
            }
        }


        public bool BOM_IsLocalProduction
        {
            get
            {
                return _bomIsLocalProductionField;
            }
            set
            {
                if ((_bomIsLocalProductionField.Equals(value) != true))
                {
                    _bomIsLocalProductionField = value;
                    RaisePropertyChanged("BOM_IsLocalProduction");
                }
            }
        }

        [ReadOnly(true)]
        public bool BOM_IsMainFabric
        {
            get
            {
                return _bomIsMainFabricField;
            }
            set
            {
                if ((_bomIsMainFabricField.Equals(value) != true))
                {
                    _bomIsMainFabricField = value;
                    RaisePropertyChanged("BOM_IsMainFabric");
                }
            }
        }

        public string BOM_Notes
        {
            get
            {
                return _bomNotesField;
            }
            set
            {
                if ((ReferenceEquals(_bomNotesField, value) != true))
                {
                    _bomNotesField = value;
                    RaisePropertyChanged("BOM_Notes");
                }
            }
        }

        private bool _colorEnabled;

        public bool ColorEnabled
        {
            get { return _colorEnabled; }
            set { _colorEnabled = value; RaisePropertyChanged("ColorEnabled"); }
        }

        [Required]
        [Range(1, int.MaxValue)]
        public int CalcMethod
        {
            get
            {
                return _calcMethodField;
            }
            set
            {
                if ((_calcMethodField.Equals(value) != true))
                {
                    _calcMethodField = value;
                    RaisePropertyChanged("CalcMethod");
                    if (CalcMethod == 4 && !IsAcc)
                    {
                        if (BomStyleColors != null)
                            foreach (var row in BomStyleColors)
                            {
                                row.FabricColor = row.StyleColor;
                                row.TblColor = row.TblColor1;
                            }
                    }
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

        public ObservableCollection<TblBOMSize> BomSizes
        {
            get
            {
                return _bomSizes;
            }
            set
            {
                if ((ReferenceEquals(_bomSizes, value) != true))
                {
                    _bomSizes = value;
                    RaisePropertyChanged("BomSizes");
                }
            }
        }

        public void ColorChangedStandardBom()
        {
            if (IsAcc)
            {
                if (ItemPerRow.IsSizeIncluded == false || ItemPerRow.IsSizeIncluded == null)
                {
                    //  TblBOMStyleColor tblBomStyleColor;
                    if (ItemPerRow.CombinationList != null)
                    {
                        ItemPerRow.SizesList = new ObservableCollection<string>();

                        if (BomStyleColors != null)
                        {
                            var color =
                      ItemPerRow.AccConfigList.Where(
                          x => TblColor == (x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
                                {
                                    if (!ItemPerRow.SizesList.Contains(size))
                                    {
                                        ItemPerRow.SizesList.Add(size);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var color =
                   ItemPerRow.AccConfigList.Where(
                       x => TblColor == (x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    var color =
                ItemPerRow.AccConfigList.Where(
                    x => TblColor == (x.Iserial)).Select(q => q.Code);

                    var sizes =
                       ItemPerRow.CombinationList.Where(
                           x => (color != null && color.Contains(x.Configuration)));

                    //var color =
                    //     ItemPerRow.AccConfigList.Where(
                    //         x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    //var sizes =
                    //   ItemPerRow.CombinationList.Where(
                    //       x => (color != null && color.Contains(x.Configuration)));
                    var distinctsize = sizes.Select(x => x.Size);

                    foreach (var size in distinctsize)
                    {
                        var row = sizes.Count(x => x.Size == size);

                        if (row == color.Count())
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

        public void ColorChanged()
        {
            if (CalcMethod == 1)
            {
                if (BomStyleColors != null)
                    foreach (var row in BomStyleColors)
                    {
                        if (IsAcc)
                        {
                            row.DummyColor = BomStyleColors.FirstOrDefault().DummyColor;
                            row.TblColor = BomStyleColors.FirstOrDefault().TblColor;
                        }
                        else
                        {
                            row.FabricColor = BomStyleColors.FirstOrDefault().FabricColor;
                            row.TblColor = BomStyleColors.FirstOrDefault().TblColor;
                        }
                    }
            }
            else if (CalcMethod == 4 && !IsAcc)
            {
                if (BomStyleColors != null)
                    foreach (var row in BomStyleColors)
                    {
                        row.FabricColor = row.StyleColor;
                        row.TblColor = row.TblColor1;
                    }
            }

            if (IsAcc)
            {
                if (ItemPerRow.IsSizeIncluded == false || ItemPerRow.IsSizeIncluded == null)
                {
                    //  TblBOMStyleColor tblBomStyleColor;
                    if (ItemPerRow.CombinationList != null)
                    {
                        ItemPerRow.SizesList = new ObservableCollection<string>();

                        if (BomStyleColors != null)
                        {
                            var color =
                            ItemPerRow.AccConfigList.Where(
                                x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
                                {
                                    if (!ItemPerRow.SizesList.Contains(size))
                                    {
                                        ItemPerRow.SizesList.Add(size);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var color =
                           ItemPerRow.AccConfigList.Where(
                               x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    //    var color =
                    //ItemPerRow.AccConfigList.Where(
                    //    x => TblColor==(x.Iserial)).Select(q => q.Code);

                    //    var sizes =
                    //       ItemPerRow.CombinationList.Where(
                    //           x => (color != null && color.Contains(x.Configuration)));

                    var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    var sizes =
                       ItemPerRow.CombinationList.Where(
                           x => (color != null && color.Contains(x.Configuration)));
                    var distinctsize = sizes.Select(x => x.Size);

                    foreach (var size in distinctsize)
                    {
                        var row = sizes.Count(x => x.Size == size);

                        if (row == color.Count())
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

        public ObservableCollection<BomStyleColorViewModel> BomStyleColors
        {
            get
            {
                return _bomStyleColors;
            }
            set
            {
                _bomStyleColors = value;

                RaisePropertyChanged("BomStyleColors");
            }
        }

        public int TblSalesOrder
        {
            get
            {
                return _tblSalesOrderField;
            }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

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

        public int TblStandardBomHeader { get; set; }

        private double _bomHorizontalShrinkage;

        public double BomHorizontalShrinkage
        {
            get { return _bomHorizontalShrinkage; }
            set { _bomHorizontalShrinkage = value; RaisePropertyChanged("BomHorizontalShrinkage"); }
        }

        private double _bomVerticalShrinkage;

        public double BomVerticalShrinkage
        {
            get { return _bomVerticalShrinkage; }
            set { _bomVerticalShrinkage = value; RaisePropertyChanged("BomVerticalShrinkage"); }
        }

        private double _rowIndexField;

        public double BomRowIndex
        {
            get
            {
                return _rowIndexField;
            }
            set
            {
                if ((_rowIndexField.Equals(value) != true))
                {
                    _rowIndexField = value;
                    RaisePropertyChanged("BomRowIndex");
                }
            }
        }
    }

    public class TechPackBomStyleColorViewModel : PropertiesViewModelBase
    {
        private int? _fabricColorField;

        private int _iserialField;

        private int _styleColorField;

        private TblColor _tblColorField;

        private TblColor _tblColor1Field;

        private TblColor _tblColor2Field;

        private int _bomField;

        public int Bom
        {
            get
            {
                return _bomField;
            }
            set
            {
                if ((_bomField.Equals(value) != true))
                {
                    _bomField = value;
                    RaisePropertyChanged("Bom");
                }
            }
        }

        private int? _dummyColorField;

        public int? DummyColor
        {
            get
            {
                return _dummyColorField;
            }
            set
            {
                if ((_dummyColorField.Equals(value) != true))
                {
                    _dummyColorField = value;
                    RaisePropertyChanged("DummyColor");
                }
            }
        }

        private int? _dyedColor;

        public int? DyedColor
        {
            get
            {
                return _dyedColor;
            }
            set
            {
                if ((_dyedColor.Equals(value) != true))
                {
                    _dyedColor = value;
                    RaisePropertyChanged("DyedColor");
                }
            }
        }

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

        public TblColor TblColor
        {
            get
            {
                return _tblColorField;
            }
            set
            {
                if ((ReferenceEquals(_tblColorField, value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        public TblColor TblColor1
        {
            get
            {
                return _tblColor1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblColor1Field, value) != true))
                {
                    _tblColor1Field = value;
                    RaisePropertyChanged("TblColor1");
                }
            }
        }

        public TblColor TblColor2
        {
            get
            {
                return _tblColor2Field;
            }
            set
            {
                if ((ReferenceEquals(_tblColor2Field, value) != true))
                {
                    _tblColor2Field = value;
                    RaisePropertyChanged("TblColor2");
                }
            }
        }
    }

    public class TechPackBomViewModel : PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TechPackBomViewModel()
        {
            BomSizes = new ObservableCollection<TblTechPackBOMSize>();
            BomSizes.CollectionChanged += BomSizes_CollectionChanged;
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;

                var tblAccessoryAttributesDetails = sv.Result.CombinationList.FirstOrDefault();
                if (tblAccessoryAttributesDetails != null)
                    ItemPerRow.SizesList.Add(tblAccessoryAttributesDetails.Size);
                //if (BomSizes != null)
                //    foreach (var tblBomSize in BomSizes)
                //    {
                //        tblBomSize.FabricSize = ItemPerRow.SizesList.FirstOrDefault();
                //    }

                if (BomStyleColors != null)
                    foreach (var variable in BomStyleColors)
                    {
                        variable.TblColor = variable.TblColor;
                        variable.StyleColor = variable.StyleColor;
                        variable.FabricColor = variable.FabricColor;
                        variable.DummyColor = variable.DummyColor;
                    }

                if (BomSizes != null)
                    foreach (var variable in BomSizes)
                    {
                        variable.FabricSize = variable.FabricSize;
                    }

                if (ItemPerRow.CombinationList != null)
                {
                    ItemPerRow.SizesList = new ObservableCollection<string>();

                    if (BomStyleColors != null)
                    {
                        var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                        var sizes =
                            ItemPerRow.CombinationList.Where(
                                x => (color != null && color.Contains(x.Configuration)));

                        if (sizes != null)
                        {
                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    var sizes =
                        ItemPerRow.CombinationList.Where(
                            x => (color != null && color.Contains(x.Configuration)));

                    if (sizes != null)
                    {
                        var distinctsize = sizes.Select(x => x.Size);

                        foreach (var size in distinctsize)
                        {
                            var row = sizes.Count(x => x.Size == size);

                            if (row == color.Count())
                            {
                                if (!ItemPerRow.SizesList.Contains(size))
                                {
                                    ItemPerRow.SizesList.Add(size);
                                }
                            }
                        }
                    }
                }
            };
        }

        private string _galarylink;
        public string galarylink
        {
            get; set;
        }

        private string _imageName;
        public string ImageName
        {
            get
            {
                return this._imageName;
            }
            set
            {
                this._imageName = value;
                this.RaisePropertyChanged("ImageName");
            }
        }

        public byte[] ImageThumb { get; set; }


        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set { _colorPerRow = value; RaisePropertyChanged("ColorPerRow"); }
        }

        private int _tblColor;

        public int TblColor
        {
            get { return _tblColor; }
            set
            {
                _tblColor = value; RaisePropertyChanged("TblColor");
                ColorChangedStandardBom();
            }
        }

        private string _fabricSize;

        public string FabricSize
        {
            get { return _fabricSize; }
            set { _fabricSize = value; RaisePropertyChanged("FabricSize"); }
        }

        private void BomSizes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblTechPackBOMSize item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }

            if (e.OldItems != null)
                foreach (TblTechPackBOMSize item in e.OldItems)
                {
                    if (item == BomSizes.FirstOrDefault())
                    {
                        if (item != null) item.PropertyChanged += item_PropertyChanged;
                    }
                }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "FabricSize":

                    if (BomSizes != null)
                        foreach (var row in BomSizes)
                        {
                            if (string.IsNullOrWhiteSpace(row.FabricSize))
                            {
                                var firstOrDefault = BomSizes.FirstOrDefault();
                                if (firstOrDefault != null)
                                    row.FabricSize = firstOrDefault.FabricSize;
                            }
                        }

                    break;
            }
        }

        private bool _gotPlan;

        public bool GotPlan
        {
            get { return _gotPlan; }
            set
            {
                _gotPlan = value; RaisePropertyChanged("GotPlan");
                if (GotPlan)
                {
                    Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private bool _GotPurchaseRequest;

        public bool GotPurchaseRequest
        {
            get { return _GotPurchaseRequest; }
            set
            {
                _GotPurchaseRequest = value; RaisePropertyChanged("GotPurchaseRequest");
                if (GotPurchaseRequest)
                {
                    Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }


        private bool _onStock;

        public bool OnStock
        {
            get { return _onStock; }
            set { _onStock = value; RaisePropertyChanged("OnStock"); }
        }

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value;
                if (value != null)
                {
                    Vendor = value.vendor_code;
                    RaisePropertyChanged("VendorPerRow");
                }
            }
        }

        private string _vendor;

        public string Vendor
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                RaisePropertyChanged("Vendor");
            }
        }

        private SolidColorBrush _foreground;

        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; RaisePropertyChanged("Foreground"); }
        }

        private GenericTable _bomCalcMethodField;

        private DateTime? _bomDateAddedField;

        private int _bomFabricField;

        private int? _bomFabricRoutField;

        private string _bomFabricTypeField;

        private byte[] _bomImageField;

        private bool _bomIsLocalProductionField;

        private bool _bomIsMainFabricField;

        private string _bomNotesField;

        private int _calcMethodField;
        private int _FabricTypeField;

        private int _iserialField;

        private ObservableCollection<TblTechPackBOMSize> _bomSizes;

        private ObservableCollection<TechPackBomStyleColorViewModel> _bomStyleColors;

        private int _tblSalesOrderField;

        private string _uoMidField;

        private bool _isSupplierMaterialField;

        public bool IsSupplierMaterial
        {
            get
            {
                return _isSupplierMaterialField;
            }
            set
            {
                if ((_isSupplierMaterialField.Equals(value) != true))
                {
                    _isSupplierMaterialField = value;
                    RaisePropertyChanged("IsSupplierMaterial");
                }
            }
        }

        private TblSalesOrderOperationViewModel _routePerRow;

        public TblSalesOrderOperationViewModel RoutePerRow
        {
            get { return _routePerRow; }
            set
            {
                _routePerRow = value; RaisePropertyChanged("RoutePerRow");
                if (RoutePerRow != null) BOM_FabricRout = RoutePerRow.TblOperation;
            }
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
                    BOM_FabricType = ItemPerRow.ItemGroup;
                    ItemName = ItemPerRow.Name;
                    BOM_Fabric = ItemPerRow.Iserial;
                    if (ItemPerRow.ItemGroup != null) IsAcc = ItemPerRow.ItemGroup.Contains("Acc");
                    if (IsAcc)
                    {
                        _client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
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

        private bool _dyed;

        public bool Dyed
        {
            get { return _dyed; }
            set { _dyed = value; RaisePropertyChanged("Dyed"); }
        }

        private IEnumerable<string> _configrationList;

        public IEnumerable<string> ConfigrationList
        {
            get { return _configrationList; }
            set { _configrationList = value; RaisePropertyChanged("ConfigrationList"); }
        }

        private List<string> _sizesList;

        public List<string> SizesList
        {
            get { return _sizesList; }
            set { _sizesList = value; RaisePropertyChanged("SizesList"); }
        }

        private string _itemName;

        [ReadOnly(true)]
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if ((ReferenceEquals(_itemName, value) != true))
                {
                    _itemName = value;
                    RaisePropertyChanged("ItemName");
                }
            }
        }

        private GenericTable _bomFabricTypePerRowField;

        public GenericTable BOM_CalcMethodPerRow
        {
            get
            {
                return _bomCalcMethodField;
            }
            set
            {
                if ((ReferenceEquals(_bomCalcMethodField, value) != true))
                {
                    _bomCalcMethodField = value;
                    RaisePropertyChanged("BOM_CalcMethodPerRow");
                    //if (BOM_CalcMethodPerRow != null) CalcMethod = BOM_CalcMethodPerRow.Iserial;
                    //if (_calcMethodField == 1)
                    //{
                    //    if (BomStyleColors != null)
                    //        foreach (var row in BomStyleColors)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                row.DummyColor = BomStyleColors.FirstOrDefault().DummyColor;
                    //            }
                    //            else
                    //            {
                    //                row.FabricColor = BomStyleColors.FirstOrDefault().FabricColor;
                    //            }
                    //        }
                    //    ColorEnabled = false;
                    //}
                    //else if (_calcMethodField == 2)
                    //{
                    //    ColorEnabled = true;
                    //    if (BomStyleColors != null)
                    //        foreach (var row in BomStyleColors)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                if (ItemPerRow.AccConfigList != null)
                    //                {
                    //                    var firstOrDefault = ItemPerRow.AccConfigList.FirstOrDefault(x => row.TblColor1 != null && x.Code == row.TblColor1.Code);
                    //                    if (firstOrDefault != null)
                    //                        row.DummyColor = firstOrDefault.Iserial;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                row.FabricColor = row.StyleColor;
                    //            }
                    //        }
                    //}
                    //else if (_calcMethodField == 3)
                    //{
                    //    ColorEnabled = true;
                    //    if (BomSizes != null)
                    //        foreach (var row in BomSizes)
                    //        {
                    //            if (IsAcc)
                    //            {
                    //                if (ItemPerRow.SizesList.Contains(row.StyleSize))
                    //                {
                    //                    row.FabricSize = row.StyleSize;
                    //                }

                    //                //        row.DummyColor = BomStyleColors.FirstOrDefault().StyleColor;
                    //            }
                    //        }
                    //}
                }
            }
        }

        public GenericTable BOM_FabricTypePerRow
        {
            get
            {
                return _bomFabricTypePerRowField;
            }
            set
            {
                if ((ReferenceEquals(_bomFabricTypePerRowField, value) != true))
                {
                    _bomFabricTypePerRowField = value;
                    RaisePropertyChanged("BOM_FabricTypePerRow");
                }
            }
        }

        public DateTime? BOM_DateAdded
        {
            get
            {
                return _bomDateAddedField;
            }
            set
            {
                if ((_bomDateAddedField.Equals(value) != true))
                {
                    _bomDateAddedField = value;
                    RaisePropertyChanged("BOM_DateAdded");
                }
            }
        }

        public int BOM_Fabric
        {
            get
            {
                return _bomFabricField;
            }
            set
            {
                if ((Equals(_bomFabricField, value) != true))
                {
                    _bomFabricField = value;
                    RaisePropertyChanged("BOM_Fabric");
                }
            }
        }

        public int? BOM_FabricRout
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
                    RaisePropertyChanged("BOM_FabricRout");
                }
            }
        }

        [ReadOnly(true)]
        public string BOM_FabricType
        {
            get
            {
                return _bomFabricTypeField;
            }
            set
            {
                if ((ReferenceEquals(_bomFabricTypeField, value) != true))
                {
                    _bomFabricTypeField = value;
                    RaisePropertyChanged("BOM_FabricType");
                }
            }
        }

        public byte[] BOM_Image
        {
            get
            {
                return _bomImageField;
            }
            set
            {
                if ((ReferenceEquals(_bomImageField, value) != true))
                {
                    _bomImageField = value;
                    RaisePropertyChanged("BOM_Image");
                }
            }
        }

        public bool BOM_IsLocalProduction
        {
            get
            {
                return _bomIsLocalProductionField;
            }
            set
            {
                if ((_bomIsLocalProductionField.Equals(value) != true))
                {
                    _bomIsLocalProductionField = value;
                    RaisePropertyChanged("BOM_IsLocalProduction");
                }
            }
        }

        [ReadOnly(true)]
        public bool BOM_IsMainFabric
        {
            get
            {
                return _bomIsMainFabricField;
            }
            set
            {
                if ((_bomIsMainFabricField.Equals(value) != true))
                {
                    _bomIsMainFabricField = value;
                    RaisePropertyChanged("BOM_IsMainFabric");
                }
            }
        }

        public string BOM_Notes
        {
            get
            {
                return _bomNotesField;
            }
            set
            {
                if ((ReferenceEquals(_bomNotesField, value) != true))
                {
                    _bomNotesField = value;
                    RaisePropertyChanged("BOM_Notes");
                }
            }
        }

        private string _supplierColorCodeField;
        public string SupplierColorCode
        {
            get
            {
                return _supplierColorCodeField;
            }
            set
            {
                if ((ReferenceEquals(_supplierColorCodeField, value) != true))
                {
                    _supplierColorCodeField = value;
                    RaisePropertyChanged("SupplierColorCode");
                }
            }
        }

        private string _supplierCodeField;
        public string SupplierCode
        {
            get
            {
                return _supplierCodeField;
            }
            set
            {
                if ((ReferenceEquals(_supplierCodeField, value) != true))
                {
                    _supplierCodeField = value;
                    RaisePropertyChanged("SupplierCode");
                }
            }
        }
        private string _placementField;
        public string Placement
        {
            get
            {
                return _placementField;
            }
            set
            {
                if ((ReferenceEquals(_placementField, value) != true))
                {
                    _placementField = value;
                    RaisePropertyChanged("Placement");
                }
            }
        }
        private string _compositionField;
        public string Composition
        {
            get
            {
                return _compositionField;
            }
            set
            {
                if ((ReferenceEquals(_compositionField, value) != true))
                {
                    _compositionField = value;
                    RaisePropertyChanged("Composition");
                }
            }
        }

        private string _consumptionField;
        public string Consumption
        {
            get
            {
                return _consumptionField;
            }
            set
            {
                if ((ReferenceEquals(_compositionField, value) != true))
                {
                    _consumptionField = value;
                    RaisePropertyChanged("Consumption");
                }
            }
        }


        private string _widthField;
        public string Width
        {
            get
            {
                return _widthField;
            }
            set
            {
                if ((ReferenceEquals(_widthField, value) != true))
                {
                    _widthField = value;
                    RaisePropertyChanged("Width");
                }
            }
        }

        private string _weightField;
        public string Weight
        {
            get
            {
                return _weightField;
            }
            set
            {
                if ((ReferenceEquals(_weightField, value) != true))
                {
                    _weightField = value;
                    RaisePropertyChanged("Weight");
                }
            }
        }

        private string _fabric_DescriptionField;
        public string Fabric_Description
        {
            get
            {
                return _fabric_DescriptionField;
            }
            set
            {
                if ((ReferenceEquals(_fabric_DescriptionField, value) != true))
                {
                    _fabric_DescriptionField = value;
                    RaisePropertyChanged("Fabric_Description");
                }
            }
        }


        private bool _colorEnabled;

        public bool ColorEnabled
        {
            get { return _colorEnabled; }
            set { _colorEnabled = value; RaisePropertyChanged("ColorEnabled"); }
        }

        //[Required]
        //[Range(1, int.MaxValue)]
        public int CalcMethod
        {
            get
            {
                return _calcMethodField;
            }
            set
            {
                _calcMethodField = value;

                //if ((_calcMethodField.Equals(value) != true))
                //{
                //    _calcMethodField = value;
                //    RaisePropertyChanged("CalcMethod");
                //    if (CalcMethod == 4 && !IsAcc)
                //    {
                //        if (BomStyleColors != null)
                //            foreach (var row in BomStyleColors)
                //            {
                //                row.FabricColor = row.StyleColor;
                //                row.TblColor = row.TblColor1;
                //            }
                //    }
                //}
            }
        }


        [Required]
        [Range(1, int.MaxValue)]
        public int FabricType
        {
            get
            {
                return _FabricTypeField;
            }
            set
            {
                _FabricTypeField = value;
                RaisePropertyChanged("FabricType");
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

        public ObservableCollection<TblTechPackBOMSize> BomSizes
        {
            get
            {
                return _bomSizes;
            }
            set
            {
                if ((ReferenceEquals(_bomSizes, value) != true))
                {
                    _bomSizes = value;
                    RaisePropertyChanged("BomSizes");
                }
            }
        }

        public void ColorChangedStandardBom()
        {
            if (IsAcc)
            {
                if (ItemPerRow.IsSizeIncluded == false || ItemPerRow.IsSizeIncluded == null)
                {
                    //  TblBOMStyleColor tblBomStyleColor;
                    if (ItemPerRow.CombinationList != null)
                    {
                        ItemPerRow.SizesList = new ObservableCollection<string>();

                        if (BomStyleColors != null)
                        {
                            var color =
                      ItemPerRow.AccConfigList.Where(
                          x => TblColor == (x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
                                {
                                    if (!ItemPerRow.SizesList.Contains(size))
                                    {
                                        ItemPerRow.SizesList.Add(size);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var color =
                   ItemPerRow.AccConfigList.Where(
                       x => TblColor == (x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    var color =
                ItemPerRow.AccConfigList.Where(
                    x => TblColor == (x.Iserial)).Select(q => q.Code);

                    var sizes =
                       ItemPerRow.CombinationList.Where(
                           x => (color != null && color.Contains(x.Configuration)));

                    //var color =
                    //     ItemPerRow.AccConfigList.Where(
                    //         x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    //var sizes =
                    //   ItemPerRow.CombinationList.Where(
                    //       x => (color != null && color.Contains(x.Configuration)));
                    var distinctsize = sizes.Select(x => x.Size);

                    foreach (var size in distinctsize)
                    {
                        var row = sizes.Count(x => x.Size == size);

                        if (row == color.Count())
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

        public void ColorChanged()
        {
            if (CalcMethod == 1)
            {
                if (BomStyleColors != null)
                    foreach (var row in BomStyleColors)
                    {
                        if (IsAcc)
                        {
                            row.DummyColor = BomStyleColors.FirstOrDefault().DummyColor;
                            row.TblColor = BomStyleColors.FirstOrDefault().TblColor;
                        }
                        else
                        {
                            row.FabricColor = BomStyleColors.FirstOrDefault().FabricColor;
                            row.TblColor = BomStyleColors.FirstOrDefault().TblColor;
                        }
                    }
            }
            else if (CalcMethod == 4 && !IsAcc)
            {
                if (BomStyleColors != null)
                    foreach (var row in BomStyleColors)
                    {
                        row.FabricColor = row.StyleColor;
                        row.TblColor = row.TblColor1;
                    }
            }

            if (IsAcc)
            {
                if (ItemPerRow.IsSizeIncluded == false || ItemPerRow.IsSizeIncluded == null)
                {
                    //  TblBOMStyleColor tblBomStyleColor;
                    if (ItemPerRow.CombinationList != null)
                    {
                        ItemPerRow.SizesList = new ObservableCollection<string>();

                        if (BomStyleColors != null)
                        {
                            var color =
                            ItemPerRow.AccConfigList.Where(
                                x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
                                {
                                    if (!ItemPerRow.SizesList.Contains(size))
                                    {
                                        ItemPerRow.SizesList.Add(size);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var color =
                           ItemPerRow.AccConfigList.Where(
                               x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                            var sizes =
                               ItemPerRow.CombinationList.Where(
                                   x => (color != null && color.Contains(x.Configuration)));

                            var distinctsize = sizes.Select(x => x.Size);

                            foreach (var size in distinctsize)
                            {
                                var row = sizes.Count(x => x.Size == size);

                                if (row == color.Count())
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
                else
                {
                    //    var color =
                    //ItemPerRow.AccConfigList.Where(
                    //    x => TblColor==(x.Iserial)).Select(q => q.Code);

                    //    var sizes =
                    //       ItemPerRow.CombinationList.Where(
                    //           x => (color != null && color.Contains(x.Configuration)));

                    var color =
                         ItemPerRow.AccConfigList.Where(
                             x => BomStyleColors.Select(w => w.DummyColor).Contains(x.Iserial)).Select(q => q.Code);

                    var sizes =
                       ItemPerRow.CombinationList.Where(
                           x => (color != null && color.Contains(x.Configuration)));
                    var distinctsize = sizes.Select(x => x.Size);

                    foreach (var size in distinctsize)
                    {
                        var row = sizes.Count(x => x.Size == size);

                        if (row == color.Count())
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

        public ObservableCollection<TechPackBomStyleColorViewModel> BomStyleColors
        {
            get
            {
                return _bomStyleColors;
            }
            set
            {
                _bomStyleColors = value;

                RaisePropertyChanged("BomStyleColors");
            }
        }

        public int TblSalesOrder
        {
            get
            {
                return _tblSalesOrderField;
            }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

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

        public int TblStandardBomHeader { get; set; }

        private double _bomHorizontalShrinkage;

        public double BomHorizontalShrinkage
        {
            get { return _bomHorizontalShrinkage; }
            set { _bomHorizontalShrinkage = value; RaisePropertyChanged("BomHorizontalShrinkage"); }
        }

        private double _bomVerticalShrinkage;

        public double BomVerticalShrinkage
        {
            get { return _bomVerticalShrinkage; }
            set { _bomVerticalShrinkage = value; RaisePropertyChanged("BomVerticalShrinkage"); }
        }

        private double _rowIndexField;

        public double BomRowIndex
        {
            get
            {
                return _rowIndexField;
            }
            set
            {
                if ((_rowIndexField.Equals(value) != true))
                {
                    _rowIndexField = value;
                    RaisePropertyChanged("BomRowIndex");
                }
            }
        }
    }

    public class TblSalesOrderNotesModel : PropertiesViewModelBase
    {
        private int IserialField;

        private string NotesField;

        private int? TblSalesOrderField;
        private int? TblSalesOrderNotesTypeField;

        private GenericTable TblSalesOrderNotesType1Field;

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
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        public string Notes
        {
            get
            {
                return NotesField;
            }
            set
            {
                if ((ReferenceEquals(NotesField, value) != true))
                {
                    NotesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public int? TblSalesOrder
        {
            get
            {
                return this.TblSalesOrderField;
            }
            set
            {
                if ((this.TblSalesOrderField.Equals(value) != true))
                {
                    this.TblSalesOrderField = value;
                    this.RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSalesOrderNotesType")]
        public int? TblSalesOrderNotesType
        {
            get
            {
                return TblSalesOrderNotesTypeField;
            }
            set
            {
                if ((this.TblSalesOrderNotesTypeField.Equals(value) != true))
                {
                    this.TblSalesOrderNotesTypeField = value;
                    this.RaisePropertyChanged("TblSalesOrderNotesType");
                }
            }
        }


        public GenericTable SalesOrderNotesTypePerRow
        {
            get
            {
                return this.TblSalesOrderNotesType1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblSalesOrderNotesType1Field, value) != true))
                {
                    this.TblSalesOrderNotesType1Field = value;
                    this.RaisePropertyChanged("SalesOrderNotesTypePerRow");
                }
            }
        }
    }

    public class TNAStyleRouteViewModel
    {
        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public bool Checked { get; set; }
    }

    #endregion Models

    public class StyleHeaderViewModel : ViewModelBase
    {

        public int ViewModelAddMode = 0;
        ProductionService.ProductionServiceClient ProductionClient = new ProductionService.ProductionServiceClient();

        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        private bool _LimitedUpdateStyleTna;

        public bool LimitedUpdateStyleTna
        {
            get { return _LimitedUpdateStyleTna; }
            set { _LimitedUpdateStyleTna = value; RaisePropertyChanged("LimitedUpdateStyleTna"); }
        }

        private bool _LimitedAddStyleTNA;

        public bool LimitedAddStyleTNA
        {
            get { return _LimitedAddStyleTNA; }
            set { _LimitedAddStyleTNA = value; RaisePropertyChanged("LimitedAddStyleTNA"); }
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public SalesOrderType SalesOrderType { get; set; }

        private bool _seasonApprovedUpdate;

        public bool SeasonApprovedUpdate
        {
            get { return _seasonApprovedUpdate; }
            set { _seasonApprovedUpdate = value; RaisePropertyChanged("SeasonApprovedUpdate"); }
        }

        private TblLkpSeason _SeasonPerRow;

        public TblLkpSeason SeasonPerRow
        {
            get { return _SeasonPerRow; }
            set { _SeasonPerRow = value; RaisePropertyChanged("SeasonPerRow"); }
        }

        public List<LkpData.TBLsupplier> Suppliers;

        public StyleHeaderViewModel(SalesOrderType salesOrderType)
        {
            SalesOrderType = salesOrderType;
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.StyleCodingForm.ToString());
                GetCustomePermissions(PermissionItemName.StyleCodingForm.ToString());
                PremCompleted += (s, sv) =>
                {
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "SeasonApprovedUpdate") != null)
                    {
                        SeasonApprovedUpdate = true;
                    }
                    else
                    {
                        SeasonApprovedUpdate = false;
                    }
                };
                GetSeason();
                GetSizeGroup();
                // GetTblStyleTheme();
                GetDesign();
                GetTblStyleCategory();
                GetTblStyleSpecGenericFabric();
                GetTBLTechPackStatus();
                GetStyleTechPackParts();
                GetStyleTNARouteStatus();
                GetTblStyleStatus(0);
                var calculationClient = new CRUD_ManagerServiceClient();
                calculationClient.GetGenericCompleted += (s, sv) =>
                {
                    BomCalcMethodList = sv.Result;
                };
                calculationClient.GetGenericAsync("BOM_CalcMethod", "%%", "%%", "%%", "Iserial", "ASC");
                ///////////////
                var FabricTypeClient = new CRUD_ManagerServiceClient();
                FabricTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    FabricTypeList = sv.Result;
                };
                FabricTypeClient.GetGenericAsync("BOM_FabricType", "%%", "%%", "%%", "Iserial", "ASC");

                var TblSalesOrderNotesTypeClient = new CRUD_ManagerServiceClient();
                TblSalesOrderNotesTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    SalesOrderNotesTypeList = sv.Result;
                };
                TblSalesOrderNotesTypeClient.GetGenericAsync("TblSalesOrderNotesType", "%%", "%%", "%%", "Iserial", "ASC");


                var SalesPersonClient = new CRUD_ManagerServiceClient();
                SalesPersonClient.GetGenericCompleted += (s, sv) =>
                {
                    SalesPersonList = sv.Result;
                };
                SalesPersonClient.GetGenericAsync("TblSalesPerson", "%%", "%%", "%%", "Iserial", "ASC");
                var RetailOrderProductionTypeClient = new CRUD_ManagerServiceClient();
                RetailOrderProductionTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    RetailOrderProductionTypeList = sv.Result;
                };
                RetailOrderProductionTypeClient.GetGenericAsync("TblRetailOrderProductionType", "%%", "%%", "%%", "Iserial", "ASC");


                Client.GetTblStyleFabricCompositionAsync(0, int.MaxValue, "it.Iserial", null, null);

                Client.GetTblStyleFabricCompositionCompleted += (s, sv) =>
                {
                    StyleFabricCompositionList = sv.Result;
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
                Client.ValidateBomCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                        return;
                    }
                    if (sv.Result)
                    {
                        var childWindow = new ApprovalChildWindow(this);
                        childWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Bom Is Not Valid");
                    }
                };

                Client.InsertImportedSalesOrderCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show("Completed");
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                };
                Client.RemoveFromPurchasePlanCompleted += (s, sv) => MessageBox.Show("Completed");
                ProductionClient.StyleTnaCountCompleted += (s, sv) =>
                {
                    StyleTnaPerRow = sv.Result.FirstOrDefault();
                };
                Client.InsertImportedStylesCompleted += (s, sv) =>
                {
                    if (sv.Error == null)
                    {
                        MessageBox.Show("Completed");
                    }
                    else
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                };

                //Client.InsertImportedStylesCompleted += (s, sv) =>
                //{
                //    if (sv.Error != null)
                //    {
                //    }
                //};
                Client.GetFullSalesOrderDataCompleted += (s, sv) =>
                {
                    SelectedPoToLink = sv.Result;
                    SaveGeneratedSalesOrder();
                    MessageBox.Show("SalesOrder Generated");
                };
                Client.ViewReportCompleted += (s, sv) => Client.SendMailReportAsync(LoggedUserInfo.Code, "AdvancedRequestForSample", Subject, Body);

                Client.DeleteTblSalesOrderOperationDetailCompleted += (s, sv) =>
                {
                    try
                    {
                        Loading = false;
                        var row = SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.FirstOrDefault(x => x.Iserial == sv.Result);
                        SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.Remove(row);
                    }
                    catch (Exception)
                    {
                    }
                };
                Client.GetSamplesRequestBySupplierCompleted += (s, sv) =>
                {
                    SelectedDetailRow.SampleRequestList = sv.Result;
                    if (SelectedDetailRow.TblRequestForSample != null)
                    {
                        SelectedDetailRow.RequestforSamplePerRow = SelectedDetailRow.SampleRequestList.FirstOrDefault(x => x.Iserial == SelectedDetailRow.TblRequestForSample);
                    }
                };
                lkpClient.GetTblSubFamilyLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SelectedMainRow.SubFamilyList.All(x => x.Iserial != row.TblSubFamily1.Iserial))
                        {
                            SelectedMainRow.SubFamilyList.Add(row.TblSubFamily1);
                        }
                    }
                };
                Client.GetSeasonalMasterListNotLinkedToSalesorderCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        SelectedDetailRow.SeasonalMasterList = sv.Result;
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblSalesOrderColorViewModel
                            {
                                TblSalesOrder = SelectedDetailRow.Iserial,
                                TblColor = row.TblColor,
                                ColorPerRow = row.TblColor1,
                                TblSalesOrderColorTheme = row.TblSalesOrderColorTheme,
                                ManualCalculation = row.ManualCalculation
                            };

                            newrow.SalesOrderSizeRatiosList.Clear();
                            if (row.TblSeasonalMasterListDetails != null)
                            {
                                foreach (var tblSeasonalMasterListDetail in row.TblSeasonalMasterListDetails)
                                {
                                    tblSeasonalMasterListDetail.Iserial = 0;
                                    tblSeasonalMasterListDetail.EntityKey = null;
                                }
                            }

                            //Map with the NEW Stitch System Ratios with Sizes 

                            ObservableCollection<TblSeasonalMasterListDetail> data = new ObservableCollection<TblSeasonalMasterListDetail>();

                            if (row.TblSeasonalMasterListDetails.Count() != 0)
                            {
                                foreach (var item in Sizes.OrderBy(x => x.Id))
                                {
                                    TblSeasonalMasterListDetail d = new TblSeasonalMasterListDetail();
                                    d = row.TblSeasonalMasterListDetails.Where(x => x.Size == item.SizeCode).FirstOrDefault();
                                    if(d != null)
                                    data.Add(d);
                                }
                            }


                            //GenericMapper.InjectFromObCollection(newrow.SalesOrderSizeRatiosList, row.TblSeasonalMasterListDetails);
                            GenericMapper.InjectFromObCollection(newrow.SalesOrderSizeRatiosList, data);

                            var brandRow = BrandList.SingleOrDefault(x => x.Brand_Code == SelectedMainRow.Brand);

                            var SelectedSalesOrderTna = SelectedMainRow.StyleTnaList.FirstOrDefault(w => w.Iserial == SelectedDetailRow.TblStyleTNAHeader);
                            if (brandRow.RequireTNA && SelectedSalesOrderTna != null)
                            {
                                if (SelectedSalesOrderTna.StyleTNAColorDetailList.Any(w => w.TblColor == newrow.TblColor))
                                {
                                    newrow.Cost = SelectedSalesOrderTna.StyleTNAColorDetailList.FirstOrDefault(w => w.TblColor == newrow.TblColor).Cost;
                                }
                                else
                                {
                                    newrow.Cost = (decimal?)SelectedSalesOrderTna.TargetCostPrice;
                                }

                            }
                            else
                            {
                                newrow.Cost = (decimal?)SelectedMainRow.TargetCostPrice;
                            }
                            newrow.CurrencyPerRow = CurrencyList.First(x => x.LocalCurrency == true);
                            newrow.Total = row.Qty;
                            newrow.TblSalesOrderColorTheme = row.TblSalesOrderColorTheme;
                            newrow.ThemePerRow = row.TblSalesOrderColorTheme1;
                            newrow.DeliveryDate = row.DelivaryDate;
                            if (SelectedDetailRow.SalesOrderColorList.Count(x => x.TblSalesOrderColorTheme == row.TblSalesOrderColorTheme && x.TblColor == row.TblColor) == 0)
                            {
                                SelectedDetailRow.SalesOrderColorList.Add(newrow);
                            }
                        }
                    }
                };
                Client.GetSeasonalMasterListNotLinkedToStyleTnaByStyleCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        //SelectedDetailRow.SeasonalMasterList = sv.Result;
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblStyleTNAColorDetailModel();
                            newrow.Cost = Convert.ToDecimal(SelectedTnaRow.TargetCostPrice);

                            newrow.AccCost = SelectedTnaRow.AccCost;
                            newrow.FabricCost = SelectedTnaRow.FabricCost;
                            newrow.OperationCost = SelectedTnaRow.OperationCost;
                            newrow.DeliveryDate = SelectedTnaRow.DeliveryDate;
                            newrow.TblCurrency = SelectedTnaRow.TblCurrency;
                            newrow.CurrencyPerRow = SelectedTnaRow.CurrencyPerRow;
                            newrow.ColorPerRow = row.TblColor1;
                            newrow.TblColor = row.TblColor;
                            newrow.ExchangeRate = SelectedTnaRow.ExchangeRate;
                            newrow.LocalCost = SelectedTnaRow.LocalCost;
                            var currentRowIndex = (SelectedTnaRow.StyleTNAColorDetailList.IndexOf(SelectedStyleTNAColorDetailRow));
                            SelectedTnaRow.StyleTNAColorDetailList.Insert(currentRowIndex + 1, newrow);
                            newrow.Qty = row.Qty;
                            SelectedStyleTNAColorDetailRow = newrow;
                            if (SelectedTnaRow.StyleTNAColorDetailList.Count(x => x.TblColor == row.TblColor) == 0)
                            {
                                SelectedTnaRow.StyleTNAColorDetailList.Add(newrow);
                            }
                        }
                    }
                };

                lkpClient.GetTblDirectionLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SelectedMainRow.DirectionList.All(x => x.Iserial != row.TblLkpDirection1.Iserial))
                        {
                            SelectedMainRow.DirectionList.Add(new GenericTable().InjectFrom(row.TblLkpDirection1) as GenericTable);
                        }
                    }

                    SelectedMainRow.DirectionPerRow = SelectedMainRow.DirectionList.SingleOrDefault(x => x.Iserial == SelectedMainRow.TblLkpDirection);

                };

                lkpClient.FamilyCategory_GetTblThemeLinkCompleted += (s, sv) =>
                {

                    foreach (var row in sv.Result)
                    {
                        if (SelectedMainRow.StyleThemeList.All(x => x.Iserial != row.Iserial))
                        {
                            SelectedMainRow.StyleThemeList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                        }
                    }

                    SelectedMainRow.StyleThemePerRow = SelectedMainRow.StyleThemeList.SingleOrDefault(x => x.Iserial == SelectedMainRow.tblTheme);

                };

                Client.GetTblRequestForSampleStatusCompleted += (s, sv) =>
                {
                    RequestForSampleStatusList = sv.Result;
                };
                Client.GetTblRequestForSampleStatusAsync();
                Client.GetTblCurrencyAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                var uomClient = new CRUD_ManagerServiceClient();
                uomClient.GetGenericCompleted += (s, sv) =>
                {
                    UomList = sv.Result;
                };
                uomClient.GetGenericAsync("tbl_lkp_UoM", "%%", "%%", "%%", "Iserial", "ASC");

                MainRowList = new ObservableCollection<TblStyleViewModel>();
                SelectedMainRow = new TblStyleViewModel();
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;
                Client.GetTblSizeGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SizeGroupList.All(x => x.Iserial != row.Iserial))
                        {
                            SizeGroupList.Add(row);
                        }
                    }
                };
                Client.GetStyleMainImageCompleted += (s, sv) =>
                {
                    if (sv.Result == null)
                    {
                        try
                        {
                            LkpData.LkpDataClient _lkpclient = new LkpData.LkpDataClient();
                            _lkpclient.InsertStyleImageFromFolderAsync(SelectedMainRow.Iserial, SelectedMainRow.StyleCode);
                            _lkpclient.InsertStyleImageFromFolderCompleted += _lkpclient_InsertStyleImageFromFolderCompleted;

                            #region Old_Image_Save
                            /*
                           if (false)
                           {
                               //var TemplateUri = new Uri(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", SelectedMainRow.StyleCode));
                               //var stream = Application.GetResourceStream(TemplateUri).Stream;

                               //GetImagePath();
                               FileStream file = new FileStream(@"D:\StylePicture\ORMP20TS-030BA.jpg", FileMode.Open);

                               // StreamResourceInfo gifContentInfo = Application.GetResourceStream(new Uri(@"F:\ORMP20TS-030BA.jpg", UriKind.Relative));
                               //FileStream file = new FileStream(@"D:\ORMP20TS-030BA.jpg", FileMode.Open);
                               //BitmapImage newImage = new BitmapImage { UriSource = new Uri("/CCWFM;component/StyleImages/UANTR002DM.jpg", UriKind.Relative)};
                               //Stream gifStream = gifContentInfo.Stream;
                               // string imagePath =  string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", SelectedMainRow.StyleCode);
                               //string imagePath = string.Format(@"/CCWFM;component/Images/{0}.jpg", SelectedMainRow.StyleCode);

                               //MessageBox.Show(imagePath);
                               //FileStream file = new FileStream(imagePath, FileMode.Open);
                               WriteableBitmap wb;
                               wb = ImageHelper.GetImageSource(file, 100, 100);
                               byte[] buffer;
                               using (var source = JpgEncoder.Encode(wb, 50))
                               {
                                   var bufferSize = Convert.ToInt32(source.Length);
                                   buffer = new byte[bufferSize];
                                   source.Read(buffer, 0, bufferSize);
                                   source.Close();
                               }
                               SelectedMainRow.StyleImage = buffer;
                               //Saving Image To DB
                               SaveStyleImageToDB(buffer);
                           }
                            */
                            #endregion


                        }

                        catch (Exception ex)
                        {
                            SelectedMainRow.StyleImage = null;
                        }
                    }
                    else
                    {
                        SelectedMainRow.StyleImage = sv.Result;
                    }
                };

                var factorGroupClient = new CRUD_ManagerServiceClient();
                factorGroupClient.GetGenericCompleted += (s, sv) =>
                {
                    FactoryGroupList = sv.Result;
                };
                factorGroupClient.GetGenericAsync("TblFactoryGroup", "%%", "%%", "%%", "Iserial", "ASC");

                var complexityGroupClient = new CRUD_ManagerServiceClient();
                complexityGroupClient.GetGenericCompleted += (s, sv) =>
                {
                    ComplexityGroupList = sv.Result;
                };
                complexityGroupClient.GetGenericAsync("TblComplexityGroup", "%%", "%%", "%%", "Iserial", "ASC");

                StartTime = DateTime.Now;
                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList.Clear();
                    BrandFilterList.Clear();
                    foreach (var variable in sv.Result.OrderBy(x => x.Brand_Code))
                    {
                        BrandList.Add(variable);
                        BrandFilterList.Add(variable);
                    }
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                Client.GetTblColorLinkCompleted += (s, sv) =>
                {
                    ColorList = new ObservableCollection<TblColor>();
                    foreach (var row in sv.Result)
                    {
                        ColorList.Add(row.TblColor1);
                    }
                };

                Client.GetTblSalesOrderColorThemeCompleted += (s, sv) =>
                {
                    ThemesList = sv.Result;
                };

                Client.GetTblSizeCodeCompleted += (s, sv) =>
                {
                    Loading = false;
                    Sizes = sv.Result;
                    if (SeasonalMasterListCompleted)
                    {
                        var childWindow = new SeasonalMasterListChildWindow(this);

                        childWindow.Show();
                        SeasonalMasterListCompleted = false;
                    }
                };
                Client.GetPendingStyleCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            
                            var newrow = new TblStyleViewModel();
                            
                            newrow.InjectFrom(row);
                            if (!MainRowListTemp.Contains(newrow))
                            {
                                newrow.SeasonPerRow = new TblLkpSeason();
                                newrow.SizeGroupPerRow = new TblSizeGroup();
                                newrow.StyleCategoryPerRow = new GenericTable();
                                newrow.StyleSpecGenericFabricPerRow = new GenericTable();
                                if (SizeGroupList.All(x => x.Iserial != row.TblSizeGroup1.Iserial))
                                {
                                    SizeGroupList.Add(row.TblSizeGroup1);
                                }
                                if (row.TblStyleStatu != null)
                                {
                                    if (StyleStatusList.All(x => x.Iserial != row.TblStyleStatus))
                                    {
                                        StyleStatusList.Add(
                                            new GenericTable().InjectFrom(row.TblStyleStatu) as GenericTable);
                                    }
                                }

                                if (row.TblStyleCategory1 != null)
                                {
                                    newrow.StyleCategoryPerRow.InjectFrom(row.TblStyleCategory1);

                                    if (StyleCategoryList.All(x => x.Iserial != row.TblStyleCategory))
                                    {
                                        StyleCategoryList.Add(
                                            new GenericTable().InjectFrom(row.TblStyleCategory1) as GenericTable);
                                    }
                                }

                                if (row.TblGenericFabric1 != null)
                                {
                                    newrow.StyleSpecGenericFabricPerRow.InjectFrom(row.TblGenericFabric1);

                                    if (StyleSpecGenericFabricList.All(x => x.Iserial != row.TblGenericFabric))
                                    {
                                        StyleSpecGenericFabricList.Add(
                                            new GenericTable().InjectFrom(row.TblGenericFabric1) as GenericTable);
                                    }
                                }

                                if (row.TblStyleFabricComposition1 != null)
                                {
                                    newrow.StyleFabricCompositionPerRow.InjectFrom(row.TblStyleFabricComposition1);

                                    if (StyleFabricCompositionList.All(x => x.Iserial != row.TblStyleFabricComposition))
                                    {
                                        StyleFabricCompositionList.Add(row.TblStyleFabricComposition1);
                                    }
                                }

                                if (BrandSectionList.All(x => x.Iserial != row.TblLkpBrandSection1.Iserial))
                                {
                                    BrandSectionList.Add(new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection);
                                }
                                if (SeasonList.All(x => x.Iserial != row.TblLkpSeason))
                                {
                                    SeasonList.Add(new TblLkpSeason().InjectFrom(row.TblLkpSeason1) as TblLkpSeason);
                                }

                                newrow.DirectionList.Add(
                                    new GenericTable().InjectFrom(row.TblLkpDirection1) as GenericTable);

                                if (row.tblTheme != null)
                                    newrow.StyleThemeList.Add(
                                        new GenericTable().InjectFrom(row.TblSalesOrderColorTheme) as GenericTable);

                                if (DesignList.All(x => x.Iserial != row.tbl_lkp_FabricDesignes))
                                {
                                    DesignList.Add(
                                        new GenericTable().InjectFrom(row.tbl_lkp_FabricDesignes1) as GenericTable);
                                }
                                newrow.SectionPerRow = new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection;
                                newrow.FamilyPerRow = new LkpData.TblFamily().InjectFrom(row.TblFamily1) as LkpData.TblFamily;
                                newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == newrow.TblLkpSeason);
                                newrow.SizeGroupPerRow = row.TblSizeGroup1;
                                newrow.DesignPerRow =
                                    DesignList.FirstOrDefault(x => x.Iserial == newrow.tbl_lkp_FabricDesignes);
                                newrow.SupplierFabricPerRow = row.TblSupplierFabric1;
                                newrow.SubFamilyPerRow = new LkpData.TblSubFamily().InjectFrom(row.TblSubFamily1) as LkpData.TblSubFamily;
                                newrow.DirectionPerRow = new GenericTable();
                                newrow.DirectionPerRow.InjectFrom(row.TblLkpDirection1);
                            }

                            MainRowListTemp.Add(newrow);
                        }
                        var child = new PendingStyles(this);

                        child.Show();
                    }
                    else
                    {
                        MessageBox.Show("No Styles");
                    }
                };
                //GetStyleTNALockup();
                Client.GetTblStyleCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        StartTime = DateTime.Now;
                        foreach (var row in sv.Result)
                        {

                            var newrow = new TblStyleViewModel();
                            newrow.InjectFrom(row);

                            if (row.TblSeasonalMasterLists.Count() > 0)
                            {
                                //Get tblSeasonalMasterList 
                                var newrowSeasonalMasterList = row.TblSeasonalMasterLists.Max(x => x.DelivaryDate);
                                // newrowSeasonalMasterList.InjectFrom(row.TblSeasonalMasterLists.Max(x => x.DelivaryDate));
                                newrow.StyleRetailMaxDeliveryDate = newrowSeasonalMasterList == null? "": newrowSeasonalMasterList.Value.ToShortDateString();
                            }


                            if (row.TblStyleTNAHeaders.Count() > 0)
                            {
                                var newrowTNAHeader = new TblStyleTNAHeaderViewModel();
                                newrowTNAHeader.InjectFrom(row.TblStyleTNAHeaders.FirstOrDefault());

                                //Get Supplier Name
                                if (Suppliers != null)
                                    newrow.StyleTNASupplierName = Suppliers.FirstOrDefault(x => x.Iserial == newrowTNAHeader.TblSupplier).Ename;
                                else
                                    newrow.StyleTNASupplierName = "";
                                var TblStyleTNAColorDetails = new List<TblStyleTNAColorDetail>();

                                if (row.TblStyleTNAHeaders.FirstOrDefault().TblStyleTNAColorDetails.ToList().Count > 0)
                                {
                                    //foreach (var item in row.TblStyleTNAHeaders.FirstOrDefault().TblStyleTNAColorDetails.ToList())
                                    //{
                                    //    if (item.LocalCost.Value > MaxLocalCost)
                                    //    {
                                    //        MaxLocalCost = item.LocalCost;
                                    //    }
                                    //}

                                    var MaxItem = row.TblStyleTNAHeaders.FirstOrDefault().TblStyleTNAColorDetails.ToList().Max(x => x.LocalCost);
                                    var MinItem = row.TblStyleTNAHeaders.FirstOrDefault().TblStyleTNAColorDetails.ToList().Min(x => x.LocalCost);
                                    var MaxDeliveryDate = row.TblStyleTNAHeaders.FirstOrDefault().TblStyleTNAColorDetails.ToList().Max(x => x.DeliveryDate);

                                    if (MaxItem != null)
                                    {
                                        if (MaxItem != MinItem)
                                        {
                                            newrow.StyleTNAMaxCost = MaxItem.ToString() + "*";
                                            newrow.StyleTNADeliveryDate = MaxDeliveryDate.Value.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {

                                            newrow.StyleTNAMaxCost = MaxItem.ToString();
                                            newrow.StyleTNADeliveryDate = MaxDeliveryDate.Value.ToString("dd/MM/yyyy");
                                        }
                                    }
                                    else
                                    {

                                        newrow.StyleTNAMaxCost = newrowTNAHeader.LocalCost.ToString();
                                        newrow.StyleTNADeliveryDate = newrowTNAHeader.DeliveryDate.Value.ToString("dd/MM/yyyy");
                                    }
                                }
                                else
                                {
                                    newrow.StyleTNAMaxCost = newrowTNAHeader.LocalCost.ToString();
                                    newrow.StyleTNADeliveryDate = newrowTNAHeader.DeliveryDate != null ? newrowTNAHeader.DeliveryDate.Value.ToString("dd/MM/yyyy") : null;
                                }
                            }
                            else
                            {
                                newrow.StyleTNAMaxCost = "NAN";
                                newrow.StyleTNADeliveryDate = "";
                            }

                            if (sv.LastTnaStatus.Any(w => w.Key == row.Iserial))
                            {
                                newrow.LastTnaStatus = sv.LastTnaStatus.FirstOrDefault(w => w.Key == row.Iserial).Value;
                            }

                            if (!MainRowList.Contains(newrow))
                            {
                                var fabric = sv.mainFabricList.FirstOrDefault(x => x.Iserial == newrow.tbl_FabricAttriputes);

                                if (fabric != null)
                                {
                                    newrow.FabricPerRow = fabric;
                                }

                                if (fabric != null)
                                {
                                    newrow.FabricPerRow = fabric;
                                }
                                newrow.SeasonPerRow = new TblLkpSeason();
                                newrow.SizeGroupPerRow = new TblSizeGroup();
                                newrow.StyleCategoryPerRow = new GenericTable();
                                newrow.StyleSpecGenericFabricPerRow = new GenericTable();

                                if (SizeGroupList.All(x => x.Iserial != row.TblSizeGroup1.Iserial))
                                {
                                    SizeGroupList.Add(row.TblSizeGroup1);
                                }
                                if (row.TblStyleStatu != null)
                                {
                                    if (StyleStatusList.All(x => x.Iserial != row.TblStyleStatus))
                                    {
                                        StyleStatusList.Add(
                                            new GenericTable().InjectFrom(row.TblStyleStatu) as GenericTable);
                                    }
                                }

                                if (row.TblStyleCategory1 != null)
                                {
                                    newrow.StyleCategoryPerRow.InjectFrom(row.TblStyleCategory1);

                                    if (StyleCategoryList.All(x => x.Iserial != row.TblStyleCategory))
                                    {
                                        StyleCategoryList.Add(
                                            new GenericTable().InjectFrom(row.TblStyleCategory1) as GenericTable);
                                    }
                                }

                                if (row.TblGenericFabric1 != null)
                                {
                                    newrow.StyleCategoryPerRow.InjectFrom(row.TblGenericFabric1);

                                    if (StyleSpecGenericFabricList.All(x => x.Iserial != row.TblGenericFabric))
                                    {
                                        StyleSpecGenericFabricList.Add(
                                            new GenericTable().InjectFrom(row.TblGenericFabric1) as GenericTable);
                                    }
                                }

                                if (row.TblStyleFabricComposition1 != null)
                                {
                                    newrow.StyleFabricCompositionPerRow.InjectFrom(row.TblStyleFabricComposition1);

                                    if (StyleFabricCompositionList.All(x => x.Iserial != row.TblStyleFabricComposition))
                                    {
                                        StyleFabricCompositionList.Add(row.TblStyleFabricComposition1);
                                    }
                                }
                                //FamilyList.Clear();

                                if (newrow.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial))
                                {
                                    newrow.FamilyList.Add(new LkpData.TblFamily().InjectFrom(row.TblFamily1) as LkpData.TblFamily);
                                }

                                if (newrow.SubFamilyList.All(x => x.Iserial != row.TblSubFamily1.Iserial))
                                {
                                    newrow.SubFamilyList.Add(new LkpData.TblSubFamily().InjectFrom(row.TblSubFamily1) as LkpData.TblSubFamily);
                                }

                                if (BrandSectionList.All(x => x.Iserial != row.TblLkpBrandSection1.Iserial))
                                {
                                    BrandSectionList.Add(new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection);
                                }
                                if (SeasonList.All(x => x.Iserial != row.TblLkpSeason))
                                {
                                    SeasonList.Add(new TblLkpSeason().InjectFrom(row.TblLkpSeason1) as TblLkpSeason);
                                }

                                newrow.DirectionList.Add(new GenericTable().InjectFrom(row.TblLkpDirection1) as GenericTable);

                                if (row.tblTheme != null && newrow.StyleThemeList.All(x => x.Iserial != row.tblTheme))
                                {
                                    newrow.StyleThemeList.Add(new GenericTable().InjectFrom(row.TblSalesOrderColorTheme) as GenericTable);
                                }

                                if (DesignList.All(x => x.Iserial != row.tbl_lkp_FabricDesignes))
                                {
                                    DesignList.Add(
                                        new GenericTable().InjectFrom(row.tbl_lkp_FabricDesignes1) as GenericTable);
                                }

                                newrow.SalesOrderToApproveCount =
                                    sv.salesOrdersPendingCount.FirstOrDefault(x => x.Key == newrow.Iserial).Value;

                                newrow.TransactionExists = sv.TransactionExist.FirstOrDefault(x => x.Key == newrow.Iserial).Value;
                                newrow.RetialPoTransactionExist = sv.RetialPoTransactionExist.FirstOrDefault(x => x.Key == newrow.Iserial).Value;
                                newrow.SectionPerRow = new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection;
                                newrow.FamilyPerRow = new LkpData.TblFamily().InjectFrom(row.TblFamily1) as LkpData.TblFamily;

                                if (sv.group1List != null)
                                {
                                    newrow.Group1PerRow = sv.group1List.FirstOrDefault(x => x.ISERIAL == newrow.TblGroup1);
                                }

                                if (sv.group4List != null)
                                {
                                    var grouprow = sv.group4List.FirstOrDefault(x => x.ISERIAL == newrow.TblGroup4);
                                    newrow.Group4PerRow = new TblGROUP1();
                                    if (grouprow != null)
                                    {
                                        newrow.Group4PerRow.InjectFrom(grouprow);
                                    }
                                }

                                if (sv.group6List != null)
                                {
                                    var grouprow = sv.group6List.FirstOrDefault(x => x.ISERIAL == newrow.TblGroup6);
                                    newrow.Group6PerRow = new TblGROUP1();
                                    if (grouprow != null)
                                    {
                                        newrow.Group6PerRow.InjectFrom(grouprow);
                                    }
                                }

                                if (sv.group7List != null)
                                {
                                    var grouprow = sv.group7List.FirstOrDefault(x => x.ISERIAL == newrow.TblGroup7);
                                    newrow.Group7PerRow = new TblGROUP1();
                                    if (grouprow != null)
                                    {
                                        newrow.Group7PerRow.InjectFrom(grouprow);
                                    }
                                }

                                if (sv.group8List != null)
                                {
                                    var grouprow = sv.group8List.FirstOrDefault(x => x.ISERIAL == newrow.TblGroup8);
                                    newrow.Group8PerRow = new TblGROUP1();
                                    if (grouprow != null)
                                    {
                                        newrow.Group8PerRow.InjectFrom(grouprow);
                                    }
                                }

                                newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == newrow.TblLkpSeason);
                                newrow.SizeGroupPerRow = row.TblSizeGroup1;
                                newrow.DesignPerRow =
                                    DesignList.FirstOrDefault(x => x.Iserial == newrow.tbl_lkp_FabricDesignes);
                                newrow.SupplierFabricPerRow = row.TblSupplierFabric1;
                                newrow.SubFamilyPerRow = new LkpData.TblSubFamily().InjectFrom(row.TblSubFamily1) as LkpData.TblSubFamily;
                                newrow.DirectionPerRow = new GenericTable();
                                newrow.DirectionPerRow.InjectFrom(row.TblLkpDirection1);
                                if (row.tblTheme != null)
                                {
                                    newrow.StyleThemePerRow = new GenericTable();
                                    newrow.StyleThemePerRow.InjectFrom(row.TblSalesOrderColorTheme);
                                }

                                //TNA Difference 

                                MainRowList.Add(newrow);
                                newrow.Finished = true;
                            }
                        }

                        EndTime = DateTime.Now;
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }

                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }

                    if (Export)
                    {
                        Export = false;

                        var handler = ExportCompleted;
                        if (handler != null) handler(this, EventArgs.Empty);
                        //ExportGrid.ExportExcel("Style");
                    }
                };

               

                Client.UpdateOrInsertTblStyleCompleted += (s, sv) =>
                {
                    if (sv.Error != null && sv.Error.Message.StartsWith("Sales Order"))
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                    else
                    {
                        try
                        {
                            var savedRow = MainRowList.ElementAt(sv.outindex);

                            if (savedRow != null)
                            {

                                savedRow.Iserial = (sv.Result.Iserial);
                                savedRow.StyleCode = sv.Result.StyleCode;
                                savedRow.SerialNo = sv.Result.SerialNo;
                                savedRow.RefStyleCode = sv.Result.RefStyleCode;
                                savedRow.SupplierRef = sv.Result.SupplierRef;
                                savedRow.CreationDate = sv.Result.CreationDate;
                            }
                        }
                        catch (Exception)
                        {
                            if (sv.outindex == -3)
                            {
                                MainRowList.Clear();
                                GetMaindata();
                            }

                        }
                        //Save Style Image
                        UploadStyleImage();
                    }
                    Loading = false;
                };

                Client.DeleteTblStyleCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.StyleCode == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    Loading = false;
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                SelectedDetailRow = new TblSalesOrderViewModel();

                Client.GetTblSalesOrderCompleted += (s, sv) =>
                {
                    Loading = false;

                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesOrderViewModel { ApprovalStatusPerRow = row.TblApprovalStatu };
                        if (sv.SupplierList != null)
                        {
                            newrow.SupplierPerRow = sv.SupplierList.FirstOrDefault(x => x.Iserial == row.TblSupplier);
                        }

                        newrow.InjectFrom(row);

                        if (sv.ContainColors.Contains(row.Iserial))
                        {
                            newrow.ContainColors = true;
                        }

                        newrow.RequestforSamplePerRow = row.TblRequestForSample1;

                        if (!newrow.SampleRequestList.Contains(newrow.RequestforSamplePerRow))
                        {
                            newrow.SampleRequestList.Add(newrow.RequestforSamplePerRow);
                        }

                        if (row.TblRetailOrderProductionType != null)
                        {
                            newrow.RetailOrderProductionTypePerRow = RetailOrderProductionTypeList.FirstOrDefault(w => w.Iserial == row.TblRetailOrderProductionType);
                        }
                        if (row.TblStyleTNAHeader1 != null)
                        {
                            newrow.StyleTnaHeaderForRow = new TblStyleTNAHeaderViewModel().InjectFrom(row.TblStyleTNAHeader1) as TblStyleTNAHeaderViewModel;
                            newrow.StyleTnaHeaderForRow.SeasonPerRow = row.TblStyleTNAHeader1.TblLkpSeason1;
                        }
                        SelectedMainRow.DetailsList.Add(newrow);
                        SelectedMainRow.TempDetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;

                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                    else if (SelectedMainRow.DetailsList.Count <= PageSize)
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                        GetSalesOrderColors();
                        GetSalesOrderOperations();
                    }
                    if (Export)
                    {
                        Export = false;
                        var handler = ExportCompleted;
                        if (handler != null) handler(this, EventArgs.Empty);
                        //    ExportGrid.ExportExcel("Po");
                    }
                };
                Client.GetTblSalesOrderNotesCompleted += (s, sv) =>
                {
                    Loading = false;

                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesOrderNotesModel();
                        newrow.SalesOrderNotesTypePerRow = SalesOrderNotesTypeList.FirstOrDefault(w => w.Iserial == row.TblSalesOrderNotesType);

                        newrow.InjectFrom(row);

                        SelectedDetailRow.SalesOrderNotesList.Add(newrow);
                    }
                    Loading = false;


                    if (SelectedDetailRow.SalesOrderNotesList.Count == 0)
                    {

                        AddNewSalesOrderNotes(false);

                    }
                };
                Client.UpdateOrInsertTblSalesOrderCompleted += (s, x) =>
                {
                    Loading = false;
                    if (x.Error != null)
                    {
                        MessageBox.Show(x.Error.Message);
                    }
                    else
                    {
                        if (x.outindex == -2)
                        {
                            GetDetailData();
                            return;
                        }

                        try
                        {
                            var savedRow = SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                            if (savedRow != null) savedRow.InjectFrom(x.Result);
                        }
                        catch (Exception)
                        {
                        }
                        SaveMainRow();
                    }

                };

                Client.DeleteTblSalesOrderCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);

                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                Client.DeleteBomCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error != null)
                    {
                        throw sv.Error;
                    }

                    var oldrow = SelectedDetailRow.BomList.FirstOrDefault(x => x.Iserial == sv.Result);
                    if (oldrow != null) SelectedDetailRow.BomList.Remove(oldrow);
                };

                Client.DeleteTechPackBomCompleted += (s, sv) =>
               {
                   Loading = false;
                   if (sv.Error != null)
                   {
                       throw sv.Error;
                   }

                   var oldrow = SelectedDetailRow.TechPackBomList.FirstOrDefault(x => x.Iserial == sv.Result);
                   if (oldrow != null) SelectedDetailRow.TechPackBomList.Remove(oldrow);
               };
                Client.GetTblSalesOrderColorCompleted += (s, sv) =>
                {
                    Loading = false;
                    SelectedDetailRow.SalesOrderColorList.Clear();
                    foreach (var row in sv.Result)
                    {
                            var newrow = new TblSalesOrderColorViewModel
                            {
                                TblSalesOrder = row.TblSalesOrder,
                                SeasonalMasterPerRow = new TblSeasonalMasterList { TblColor1 = row.TblColor1 },
                                ColorPerRow = row.TblColor1,
                                ThemePerRow = row.TblSalesOrderColorTheme1,
                                CurrencyPerRow = CurrencyList.SingleOrDefault(x => x.Iserial == row.TblCurrency),
                                Notes = row.Notes,
                                RequestForCancelApprove = row.RequestForCancel == 1 ? true : false,
                            };

                             newrow.InjectFrom(row);

                            GenericMapper.InjectFromObCollection(newrow.SalesOrderSizeRatiosList, row.TblSalesOrderSizeRatios);

                            SelectedDetailRow.SalesOrderColorList.Add(newrow);
                            SelectedDetailRow.ContainColors = true;

                     
                    }
                };

                Client.GetTblRouteGroupAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblRouteGroupCompleted += (s, sv) =>
                {
                    RouteGroupList = sv.Result;
                };
                Client.GetTblSalesOrderOperationCompleted += (s, sv) =>
                {
                    SelectedDetailRow.SalesOrderOperationList.Clear();
                    foreach (var row in sv.Result.OrderBy(w => w.RowIndex))
                    {
                        var newrow = new TblSalesOrderOperationViewModel();
                        newrow.InjectFrom(row);
                        newrow.RouteGroupPerRow = row.TblRouteGroup;
                        newrow.TblSalesOrderOperationDetailList = row.TblSalesOrderOperationDetails;
                        SelectedDetailRow.SalesOrderOperationList.Add(newrow);

                        if (!newrow.TblSalesOrderOperationDetailList.Any())
                        {
                            newrow.TblSalesOrderOperationDetailList.Add(new TblSalesOrderOperationDetail());
                        }
                    }

                    if (sv.routeBaseGroup != null)
                        foreach (var variable in sv.routeBaseGroup)
                        {
                            AddNewSalesOrderOperation(false, variable.TblRouteGroup, variable.TblRouteGroup1);
                        }
                    if (SelectedDetailRow.SalesOrderOperationList.Count == 0)
                    {
                        AddNewSalesOrderOperation(false);
                    }
                };

                Client.UpdateOrInsertTblSalesOrderOperationCompleted += (s, sv) =>
                {
                    if (SelectedDetailRow != null)
                    {
                        if (SelectedDetailRow.SalesOrderOperationList.Any())
                        {
                            var savedRow = SelectedDetailRow.SalesOrderOperationList.ElementAt(sv.outindex);
                            if (savedRow != null)
                            {
                                savedRow.InjectFrom(sv.Result);
                                savedRow.TblSalesOrderOperationDetailList.Clear();
                                foreach (var row in sv.Result.TblSalesOrderOperationDetails)
                                {
                                    row.TblColor1 = ColorList.FirstOrDefault(x => x.Iserial == row.TblColor);
                                    savedRow.TblSalesOrderOperationDetailList.Add(row);
                                }
                            }
                        }
                        Loading = false;
                    }
                };

                Client.GetBomCompleted += (s, sv) =>
                {
                    SelectedDetailRow.BomList.Clear();
                    foreach (var row in sv.Result)
                    {
                        foreach (var styleColor in row.TblBOMStyleColors)
                        {
                            styleColor.DummyColor = styleColor.FabricColor;
                        }

                        var newrow = new BomViewModel();

                        if (row.BOM_CalcMethod != null)
                            newrow.BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable; // row.BOM_CalcMethod;

                        if (row.BOM_FabricType != null)
                            newrow.BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType) as GenericTable; // row.BOM_CalcMethod;

                        newrow.BomStyleColors = new ObservableCollection<BomStyleColorViewModel>();

                        newrow.BomStyleColors.Clear();

                        foreach (var item in SelectedDetailRow.SalesOrderColorList)
                        {
                            if (item.TblColor != null)
                            {
                                var newbomcolor = new BomStyleColorViewModel
                                {
                                    StyleColor = (int)item.TblColor,
                                    TblColor1 = item.ColorPerRow,
                                    Bom = SelectedBomRow.Iserial
                                };

                                foreach (var variable in row.TblBOMStyleColors.Where(x => x.StyleColor == item.TblColor))
                                {
                                    newbomcolor.InjectFrom(variable);
                                }
                                newrow.BomStyleColors.Add(newbomcolor);
                            }
                        }

                        GenericMapper.InjectFromObCollection(newrow.BomSizes, row.TblBOMSizes);

                        var sizesToAdd = from c in Sizes where !(from o in newrow.BomSizes select o.StyleSize).Contains(c.SizeCode) select c.SizeCode;

                        foreach (var VARIABLE in sizesToAdd)
                        {
                            newrow.BomSizes.Add(new TblBOMSize
                            {
                                StyleSize = VARIABLE,
                                MaterialUsage = 1
                            });
                        }
                        newrow.InjectFrom(row);

                        newrow.GotPurchaseRequest = false;
                        if (row.TblPurchaseOrderDetailBreakDowns.Any())
                        {
                            newrow.GotPlan = true;
                            if (row.TblPurchaseOrderDetailBreakDowns != null)
                            {
                                if (row.TblPurchaseOrderDetailBreakDowns.FirstOrDefault().TblPurchaseOrderDetail1.TblPurchaseRequestLinks != null)
                                {
                                    if (row.TblPurchaseOrderDetailBreakDowns.FirstOrDefault().TblPurchaseOrderDetail1.TblPurchaseRequestLinks.Any())
                                    {
                                        newrow.GotPurchaseRequest = true;
                                    }
                                    else
                                    {
                                        newrow.GotPurchaseRequest = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            newrow.GotPlan = false;
                        }
                        newrow.RoutePerRow =
                            SelectedDetailRow.SalesOrderOperationList.FirstOrDefault(x => x.TblOperation == row.BOM_FabricRout);
                        newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.BOM_Fabric && x.ItemGroup == row.BOM_FabricType);
                        newrow.VendorPerRow = sv.vendorList.FirstOrDefault(w => w.vendor_code == row.Vendor);
                        foreach (var colorRow in newrow.BomStyleColors)
                        {
                            colorRow.DummyColor = colorRow.DummyColor;
                        }

                        SelectedDetailRow.BomList.Add(newrow);
                    }
                    if (SelectedDetailRow.BomList.Count == 0)
                    {
                        AddBom(false);
                    }
                };

                Client.GetTechPackBomCompleted += (s, sv) =>
                {
                    SelectedDetailRow.TechPackBomList.Clear();
                    foreach (var row in sv.Result)
                    {
                        foreach (var styleColor in row.TblTechPackBOMStyleColors)
                        {
                            styleColor.DummyColor = styleColor.FabricColor;
                        }

                        var newrow = new TechPackBomViewModel();

                        if (row.BOM_CalcMethod != null)
                            newrow.BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable; // row.BOM_CalcMethod;
                                                                                                                             //  if(row.FabricType !=null)
                        newrow.BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType1) as GenericTable;
                        newrow.BomStyleColors = new ObservableCollection<TechPackBomStyleColorViewModel>();

                        newrow.BomStyleColors.Clear();

                        // foreach (var item in SelectedDetailRow.SalesOrderColorList)
                        foreach (var item in SelectedMainRow.SelectedStyleSeasonalMaterColors)
                        {
                            if (item != null)
                            {
                                var newbomcolor = new TechPackBomStyleColorViewModel
                                {
                                    StyleColor = item.Iserial,
                                    TblColor1 = (TblColor)item,
                                    Bom = SelectedTechPackBomRow.Iserial
                                };

                                foreach (var variable in row.TblTechPackBOMStyleColors.Where(x => x.StyleColor == item.Iserial))
                                {
                                    newbomcolor.InjectFrom(variable);
                                }
                                newrow.BomStyleColors.Add(newbomcolor);
                            }
                        }

                        GenericMapper.InjectFromObCollection(newrow.BomSizes, row.TblTechPackBOMSizes);

                        var sizesToAdd = from c in Sizes where !(from o in newrow.BomSizes select o.StyleSize).Contains(c.SizeCode) select c.SizeCode;

                        foreach (var VARIABLE in sizesToAdd)
                        {
                            newrow.BomSizes.Add(new TblTechPackBOMSize
                            {
                                StyleSize = VARIABLE,
                                MaterialUsage = 1
                            });
                        }
                        newrow.InjectFrom(row);

                        newrow.GotPurchaseRequest = false;

                        //if (row.TblPurchaseOrderDetailBreakDowns.Any())
                        //{
                        //    newrow.GotPlan = true;
                        //    if (row.TblPurchaseOrderDetailBreakDowns != null)
                        //    {
                        //        if (row.TblPurchaseOrderDetailBreakDowns.FirstOrDefault().TblPurchaseOrderDetail1.TblPurchaseRequestLinks != null)
                        //        {
                        //            if (row.TblPurchaseOrderDetailBreakDowns.FirstOrDefault().TblPurchaseOrderDetail1.TblPurchaseRequestLinks.Any())
                        //            {
                        //                newrow.GotPurchaseRequest = true;
                        //            }
                        //            else
                        //            {
                        //                newrow.GotPurchaseRequest = false;
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        {
                            newrow.GotPlan = false;
                        }
                        //newrow.RoutePerRow =  SelectedDetailRow.SalesOrderOperationList.FirstOrDefault(x => x.TblOperation == row.BOM_FabricRout);
                        newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.BOM_Fabric && x.ItemGroup == row.BOM_FabricType);
                        newrow.VendorPerRow = sv.vendorList.FirstOrDefault(w => w.vendor_code == row.Vendor);
                        foreach (var colorRow in newrow.BomStyleColors)
                        {
                            colorRow.DummyColor = colorRow.DummyColor;
                        }

                        SelectedDetailRow.TechPackBomList.Add(newrow);
                    }

                    if (SelectedDetailRow.TechPackBomList.Count == 0)
                    {
                        AddTechPackBom(false);
                    }
                };

                Client.SearchSupplierFabricCompleted += (s, sv) =>
                {
                    if (SelectedMainRow != null) SelectedMainRow.SupplierFabrics = sv.Result;

                    OnSupplierFabricsCompleted();
                };

                Client.GetRetailTblChainSettingAsync();

                Client.GetRetailTblChainSettingCompleted += (s, sv) =>
                {
                    RetailTblChainSetting = sv.Result;
                };

                Client.GetFinishedFabricsCompleted += (s, sv) =>
                {
                    SelectedMainRow.Items = sv.Result;
                    OnMainFabricCompleted();
                };
                Client.UpdateOrInsertTblSalesOrderColorCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                        return;
                    }
                    if (SelectedDetailRow != null)
                    {
                        var savedRow = SelectedDetailRow.SalesOrderColorList.ElementAt(sv.outindex);
                        if (savedRow != null)
                        {
                            savedRow.InjectFrom(sv.Result);
                            if (savedRow == SelectedDetailRow.SalesOrderColorList.LastOrDefault())
                            {
                                MessageBox.Show("Saved Successfully");
                            }
                        }
                        SelectedDetailRow.ContainColors = true;
                    }

                };

                Client.UpdateOrInsertTblSalesOrderNotesCompleted += (s, sv) =>
                {
                    if (SelectedDetailRow != null)
                    {
                        var savedRow = SelectedDetailRow.SalesOrderNotesList.ElementAt(sv.outindex);
                        if (savedRow != null)
                        {
                            savedRow.InjectFrom(sv.Result);

                        }
                    }
                    Loading = false;
                };
                Client.DeleteTblSalesOrderOperationCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }

                    if (SelectedDetailRow.SalesOrderOperationList != null)
                    {
                        var oldrow = SelectedDetailRow.SalesOrderOperationList.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (oldrow != null) SelectedDetailRow.SalesOrderOperationList.Remove(oldrow);
                    }
                    Loading = false;
                };

                Client.GetRfqNotLinkToPoCompleted += (s, sv) =>
                {
                    PoNotLinkdList = sv.Result;
                    var childWindow = new GenerateSalesOrderFromPo(this);
                    childWindow.Show();
                };
                Client.GetPoNotLinkToSalesorderCompleted += (s, sv) =>
                {
                    if (sv.Result.ToList().Count > 0)
                    {
                        PoNotLinkdList = sv.Result;
                        var childWindow = new GenerateSalesOrderFromPo(this);
                        childWindow.Show();
                    }
                    else
                    {
                        CreateSalesOrderfromMasterList(sv.TNADeliveryDate);
                    }
                };
                Client.DeleteTblSalesOrderColorCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }

                    if (SelectedDetailRow.SalesOrderColorList != null)
                    {
                        var oldrow = SelectedDetailRow.SalesOrderColorList.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (oldrow != null) SelectedDetailRow.SalesOrderColorList.Remove(oldrow);
                    }
                    Loading = false;
                };
                Client.DeleteTblSalesOrderNotesCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }

                    if (SelectedDetailRow.SalesOrderNotesList != null)
                    {
                        var oldrow = SelectedDetailRow.SalesOrderNotesList.FirstOrDefault(x => x.Iserial == sv.Result.Iserial);
                        if (oldrow != null) SelectedDetailRow.SalesOrderNotesList.Remove(oldrow);
                    }
                    Loading = false;
                };

                Client.UpdateOrInsertBomCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                    Loading = false;
                    GetSalesOrderBom();
                };

                Client.PostBomToAxCompleted += (s, sv) => MessageBox.Show("Successfully Posted");

                Client.GetSalesOrderForCopyBomCompleted += (s, sv) =>
                {
                    foreach (var salesOrderRow in sv.Result)
                    {
                        var newrowSalesOrder = new TblSalesOrderViewModel { ApprovalStatusPerRow = salesOrderRow.TblApprovalStatu };
                        if (sv.SupplierList != null)
                        {
                            newrowSalesOrder.SupplierPerRow = sv.SupplierList.FirstOrDefault(x => x.Iserial == salesOrderRow.TblSupplier);
                        }
                        newrowSalesOrder.InjectFrom(salesOrderRow);

                        foreach (var row in salesOrderRow.TblSalesOrderOperations)
                        {
                            var newrow = new TblSalesOrderOperationViewModel();
                            newrow.InjectFrom(row);
                            newrow.RouteGroupPerRow = row.TblRouteGroup;

                            newrowSalesOrder.SalesOrderOperationList.Add(newrow);
                        }
                        foreach (var row in salesOrderRow.BOMs)
                        {
                            var styleColorList = new ObservableCollection<TblBOMStyleColor>();

                            var bomSizesTemp = new ObservableCollection<TblBOMSize>();

                            foreach (var item in Sizes)
                            {
                                bomSizesTemp.Add(new TblBOMSize
                                {
                                    StyleSize = item.SizeCode,
                                    MaterialUsage = 1
                                });
                            }

                            var newrow = new BomViewModel();
                            if (row.BOM_CalcMethod != null)
                                newrow.BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable; // row.BOM_CalcMethod;

                            if (row.BOM_FabricType != null)
                                newrow.BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType) as GenericTable; // row.BOM_CalcMethod;

                            GenericMapper.InjectFromObCollection(newrow.BomSizes, bomSizesTemp);
                            newrow.BomStyleColors = new ObservableCollection<BomStyleColorViewModel>();
                            GenericMapper.InjectFromObCollection(newrow.BomStyleColors, styleColorList);
                            newrow.InjectFrom(row);
                            newrow.RoutePerRow =
                                newrowSalesOrder.SalesOrderOperationList.FirstOrDefault(x => x.TblOperation == row.BOM_FabricRout);
                            newrow.ItemPerRow = sv.itemsList.FirstOrDefault(x => x.Iserial == row.BOM_Fabric);
                            newrowSalesOrder.BomList.Add(newrow);
                            //
                        }
                        CopybomList.Add(newrowSalesOrder);
                    }
                    Loading = false;
                };
                Client.GetTblStandardBOMHeaderCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblStandardBomHeaderViewModel();

                            newrow.InjectFrom(row);
                            if (!StandardBomList.Contains(newrow))
                            {
                                newrow.SeasonPerRow = new TblLkpSeason();
                                newrow.FactoryGroupPerRow = new GenericTable();
                                newrow.ComplexityGroupPerRow = new GenericTable();

                                if (BrandSectionList.All(x => x.Iserial != row.TblLkpBrandSection1.Iserial))
                                {
                                    BrandSectionList.Add(new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection);
                                }
                                if (SeasonList.All(x => x.Iserial != row.TblLkpSeason))
                                {
                                    SeasonList.Add(new TblLkpSeason().InjectFrom(row.TblLkpSeason1) as TblLkpSeason);
                                }

                                newrow.FactoryGroupPerRow = new GenericTable().InjectFrom(row.TblFactoryGroup1) as GenericTable;

                                if(row.TblComplexityGroup1 != null)
                                newrow.ComplexityGroupPerRow = new GenericTable().InjectFrom(row.TblComplexityGroup1) as GenericTable;

                                newrow.SectionPerRow = row.TblLkpBrandSection1;
                                newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == newrow.TblLkpSeason);

                                StandardBomList.Add(newrow);
                            }
                        }
                    }
                    Loading = false;
                };

                Client.GetTblStandardBOMCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var styleColorList = new ObservableCollection<BomStyleColorViewModel>();
                        foreach (var item in SelectedDetailRow.SalesOrderColorList)
                        {
                            if (item.TblColor != null)
                                styleColorList.Add(new BomStyleColorViewModel
                                {
                                    StyleColor = (int)item.TblColor,
                                    TblColor1 = item.ColorPerRow,
                                    FabricColor = row.TblColor,
                                    TblColor = row.TblColor1
                                });
                        }
                        var bomSizesTemp = new ObservableCollection<TblBOMSize>();

                        foreach (var item in Sizes)
                        {
                            bomSizesTemp.Add(new TblBOMSize
                            {
                                StyleSize = item.SizeCode,
                                MaterialUsage = 1,
                                FabricSize = row.FabricSize
                            });
                        }

                        var newrow = new BomViewModel
                        {
                            BomStyleColors = styleColorList,
                            TblSalesOrder = SelectedDetailRow.Iserial,
                            BOM_IsMainFabric = false,
                            IsAcc = false,
                            BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable,
                            BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType) as GenericTable
                        };
                        row.Iserial = 0;
                        newrow.InjectFrom(row);
                        newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.BOM_Fabric);
                        GenericMapper.InjectFromObCollection(newrow.BomSizes, bomSizesTemp);
                        SelectedDetailRow.BomList.Add(newrow);
                    }
                };
                Client.GetTblRequestForSampleEventCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRequestForSampleEventViewModel();
                        newrow.InjectFrom(row);
                        newrow.RequestForSampleStatusPerRow = RequestForSampleStatusList.FirstOrDefault(x => x.Iserial == row.TblRequestForSampleStatus);
                        newrow.UserPerRow = row.TblAuthUser;
                        newrow.RoutePerRow =
             SelectedDetailRow.SalesOrderOperationList.FirstOrDefault(x => x.TblOperation == row.TblRouteGroup);
                        SelectedDetailRow.SubEventList.Add(newrow);
                    }
                    if (SelectedDetailRow.SubEventList.Any())
                    {
                        var lastRow = SelectedDetailRow.SubEventList.OrderBy(x => x.Iserial).LastOrDefault();
                        SelectedRequestForSampleEvent = lastRow;
                    }

                    Loading = false;
                    if (SelectedMainRow != null && SelectedDetailRow.SubEventList.Count == 0)
                    {
                        AddNewSubEventRow(false);
                    }
                };

                Client.UpdateOrInsertTblRequestForSampleEventCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = SelectedDetailRow.SubEventList.ElementAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        var lastRow = SelectedDetailRow.SubEventList.OrderBy(w => w.Iserial).LastOrDefault();
                    }
                };

                Client.DeleteTblRequestForSampleEventCompleted += (s, ev) =>
                {
                    Loading = false;
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedDetailRow.SubEventList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.SubEventList.Remove(oldrow);
                };


                ProductionClient.GetTblStyleTNAHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {

                        var newrow = new TblStyleTNAHeaderViewModel();
                        if (sv.SupplierList != null)
                        {
                            newrow.SupplierPerRow = new TBLsupplier().InjectFrom(sv.SupplierList.FirstOrDefault(x => x.Iserial == row.TblSupplier)) as TBLsupplier;
                        }
                        newrow.SeasonPerRow = SeasonList.FirstOrDefault(w => w.Iserial == row.TblLkpSeason);
                        newrow.CurrencyPerRow = CurrencyList.FirstOrDefault(x => x.Iserial == row.TblCurrency);

                        newrow.InjectFrom(row);


                        //Add TNA_DETAIL
                        //foreach (var TnaDetail in row.TblStyleTNADetails)
                        //{
                        //    var detailRow = new TblStyleTNADetailViewModel();
                        //    detailRow.InjectFrom(TnaDetail);
                        //    newrow.DetailList.Add(detailRow);
                        //}  

                        foreach (var DetailRowrow in row.TblStyleTNAColorDetails)
                        {
                            var newrowDetail = new TblStyleTNAColorDetailModel();
                            newrowDetail.ColorPerRow = new TblColor().InjectFrom(DetailRowrow.TblColor1) as TblColor;
                            newrowDetail.CurrencyPerRow = CurrencyList.FirstOrDefault(x => x.Iserial == DetailRowrow.TblCurrency);
                            newrowDetail.InjectFrom(DetailRowrow);

                            newrow.StyleTNAColorDetailList.Add(newrowDetail);
                        }
                       

                        if (SelectedMainRow.StyleTnaList.FirstOrDefault(w => w.Iserial == row.Iserial) == null)
                        {

                            if (SelectedTnaRow != newrow)
                            {
                                SelectedTnaRow = newrow;
                                SelectedTnaRow.DetailList.Clear();

                                foreach (var TnaDetail in row.TblStyleTNADetails)
                                {
                                    var detailRow = new TblStyleTNADetailViewModel();

                                    if (TnaDetail.TblStyleTNA != null && StyleTNAList.Count() > 0)
                                        detailRow.StyleTNAPerRow = new ProductionService.TblStyleTNA().InjectFrom(StyleTNAList.FirstOrDefault(w => w.Iserial == TnaDetail.TblStyleTNA)) as ProductionService.TblStyleTNA;
                                    if (TnaDetail.TblStyleTNARouteStatus != null && StyleTNAStatusList.Count() > 0)
                                        detailRow.StyleTNAStatusPerRow = new LkpData.TblStyleTNARouteStatu().InjectFrom(StyleTNAStatusList.FirstOrDefault(w => w.Iserial == TnaDetail.TblStyleTNARouteStatus)) as LkpData.TblStyleTNARouteStatu;

                                    detailRow.InjectFrom(TnaDetail);
                                   
                                    //newrow.DetailList.Add(detailRow);
                                   
                                    SelectedTnaRow.DetailList.Add(detailRow);
                                }

                            }

                            //Add TNA_DETAIL
                            //foreach (var TnaDetail in row.TblStyleTNADetails)
                            //{
                            //    var detailRow = new TblStyleTNADetailViewModel();
                            //    detailRow.InjectFrom(TnaDetail);
                            //    newrow.DetailList.Add(detailRow);
                            //}  

                            SelectedMainRow.StyleTnaList.Add(newrow);


                        }
                    }
                    Loading = false;
                    if (!SelectedMainRow.StyleTnaList.Any())
                    {
                        
                        AddNewStyleTNARow(false);
                    }

                };

                ProductionClient.GetTblStyleTNADetailCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                        SelectedTnaRow.DetailList.Clear();
                    foreach (var row in sv.Result.OrderBy(w => w.ExpectedDeliveryDate).ToList())
                    {
                        var newrow = new TblStyleTNADetailViewModel();

                        newrow.InjectFrom(row);
                        try
                        {
                            newrow.StyleTNAPerRow = new ProductionService.TblStyleTNA().InjectFrom(StyleTNAList.FirstOrDefault(w => w.Iserial == row.TblStyleTNA)) as ProductionService.TblStyleTNA;
                            if(row.TblStyleTNARouteStatus != null)
                            newrow.StyleTNAStatusPerRow = new LkpData.TblStyleTNARouteStatu().InjectFrom(StyleTNAStatusList.FirstOrDefault(w => w.Iserial == row.TblStyleTNARouteStatus)) as LkpData.TblStyleTNARouteStatu;

                            SelectedTnaRow.DetailList.Add(newrow);
                        }
                        catch { }

                    }
                    Loading = false;

                    CopyOfSelectedTnaRow = new TblStyleTNAHeaderViewModel();

                    CopyOfSelectedTnaRow.InjectFrom(SelectedTnaRow);

                    if (SelectedTnaRow.DetailList.Count == 0)
                    {
                        //AddNewTNADetailRow();
                    }
                };

                ProductionClient.UpdateOrInsertTblStyleTNAHeaderCompleted += (s, x) =>
                {
                    if (x.Error != null)
                    {
                        MessageBox.Show(x.Error.Message);
                    }
                    SelectedTnaRow.InjectFrom(x.Result);
                    SelectedTnaRow.DetailList.Clear();
                    foreach (var row in x.Result.TblStyleTNADetails)
                    {
                        var newrow = new TblStyleTNADetailViewModel();

                        newrow.InjectFrom(row);
                        newrow.StyleTNAPerRow = new ProductionService.TblStyleTNA().InjectFrom(StyleTNAList.FirstOrDefault(w => w.Iserial == row.TblStyleTNA)) as ProductionService.TblStyleTNA;
                        if(row.TblStyleTNARouteStatus != null)
                        newrow.StyleTNAStatusPerRow = new LkpData.TblStyleTNARouteStatu().InjectFrom(StyleTNAStatusList.FirstOrDefault(w => w.Iserial == row.TblStyleTNARouteStatus)) as LkpData.TblStyleTNARouteStatu;
                        SelectedTnaRow.DetailList.Add(newrow);
                    }

                    foreach (var row in x.Result.TblStyleTNAColorDetails)
                    {
                        var newrow = new TblStyleTNAColorDetailModel();
                        newrow.InjectFrom(row);
                        newrow.ColorPerRow = new TblColor().InjectFrom(row.TblColor1) as TblColor;
                        SelectedTnaRow.StyleTNAColorDetailList.Add(newrow);
                    }
                    Loading = false;
                    GetStyleTNAdata();
                    StyleTnaCount();
                };

                ProductionClient.RequestStyleTnaCompleted += (s, sv) =>
                {
                    SelectedMainRow.TNACreationDate = sv.Result;
                };

                Client.GetSmlCompleted += (s, sv) =>
                {
                    Loading = false;
                    SelectedMainRow.SeasonalMasterListColorsList = new ObservableCollection<TblColor>();

                    foreach (var item in sv.Result)
                    {
                        SelectedMainRow.SeasonalMasterListColorsList.Add(item.TblColor1);
                    }
                };
            }
            else
            {

                MainRowList = new ObservableCollection<TblStyleViewModel>();
                MainRowList.Add(new TblStyleViewModel() { StyleCode = "hwhw" });
            }
        }

        private void CreateSalesOrderfromMasterList(DateTime? DeliveryDate)
        {
            try
            {
                var brandRow = BrandList.SingleOrDefault(x => x.Brand_Code == SelectedMainRow.Brand);
                var TnaList = SelectedMainRow.DetailsList.Select(w => w.TblStyleTNAHeader);
                if (brandRow.RequireTNA && SelectedMainRow.DetailsList.Count == 0 && SalesOrderType.RetailPo == SalesOrderType)
                {
                    return;
                }

                var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                var lastTna = SelectedMainRow.StyleTnaList.Where(w => !TnaList.Contains(w.Iserial)).OrderByDescending(w => w.Iserial).FirstOrDefault();
                if (brandRow != null)
                {
                    var newRow = new TblSalesOrderViewModel
                    {
                        CreatedBy = LoggedUserInfo.WFM_UserName,
                        Customer = brandRow.CustomerCode,
                        TblStyle = SelectedMainRow.Iserial,
                        SalesOrderType = (int)SalesOrderType,
                        IsPlannedOrder = true,
                        DeliveryDate = DeliveryDate
                    };
                    if (brandRow.RequireTNA && SalesOrderType.RetailPo == SalesOrderType)
                    {
                        if (lastTna != null)
                        {
                            newRow.TblStyleTNAHeader = lastTna.Iserial;
                            newRow.DeliveryDate = lastTna.DeliveryDate;
                            newRow.TblSupplier = lastTna.TblSupplier;
                            newRow.SupplierPerRow = lastTna.SupplierPerRow;
                            newRow.StyleTnaHeaderForRow = lastTna;
                            newRow.AccCost = lastTna.AccCost;
                            newRow.FabricCost = lastTna.FabricCost;
                            newRow.OperationCost = lastTna.OperationCost;
                            newRow.TblRetailOrderProductionType = lastTna.TblRetailOrderProductionType;

                        }

                        newRow.RetailOrderProductionTypePerRow = RetailOrderProductionTypeList.FirstOrDefault(w => w.Iserial == newRow.TblRetailOrderProductionType);

                    }
                    if (newRow.SalesOrderType == (int)SalesOrderType.AdvancedSampleRequest)
                    {
                        newRow.Status = 1;
                    }
                    SelectedMainRow.DetailsList.Clear();
                    SelectedMainRow.DetailsList.Insert(0, newRow);
                    SelectedDetailRow = newRow;
                    SaveDetailRow();
                }
            } catch { }
        }

        private void _lkpclient_InsertStyleImageFromFolderCompleted(object sender, LkpData.InsertStyleImageFromFolderCompletedEventArgs e)
        {
            //Reload Image After Saving To DB
            if (e.Result == true)
            {
                GetStyleImage();
            }
        }

        private string GetImagePath()
        {
            string Path = "";
            if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", SelectedMainRow.StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", SelectedMainRow.StyleCode);
            }
            else if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.png", SelectedMainRow.StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.png", SelectedMainRow.StyleCode);
            }
            else if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpeg", SelectedMainRow.StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpeg", SelectedMainRow.StyleCode);
            }
            return Path;
        }

        private void SaveStyleImageToDB(byte[] buffer)
        {
            try
            {
                CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
                _client.MaxIserialOfStyleAsync();
                _client.MaxIserialOfStyleCompleted += (s, sv) =>
                {
                    string FolderPath = "Uploads" + "/" + sv.imagePath;
                    string folderName = FolderPath + "/" + SelectedMainRow.SeasonPerRow.Ename + "_" + SelectedMainRow.Brand + "_" +
                                         SelectedMainRow.SectionPerRow.Ename;
                    //var isvalid = false;

                    ObservableCollection<TblStyleImage> StyleImageList = new ObservableCollection<TblStyleImage>();
                    TblStyleImage tblStyleImage = new TblStyleImage();
                    tblStyleImage.Iserial = 0;
                    tblStyleImage.TblStyle = SelectedMainRow.Iserial;
                    tblStyleImage.IsPrintable = false;
                    tblStyleImage.IsActive = true;
                    tblStyleImage.ImagePath = folderName;
                    tblStyleImage.ImagePathThumbnail = buffer;
                    tblStyleImage.DefaultImage = true;
                    tblStyleImage.OrginalFileName = SelectedMainRow.StyleCode + ".jpg";

                    StyleImageList.Add(tblStyleImage);
                    // isvalid = Validator.TryValidateObject(tblStyleImageViewModel, new ValidationContext(tblStyleImageViewModel, null, null), null, true);
                    if (true)
                    {
                        _client.UpdateOrInsertTblStyleImageAsync(StyleImageList);
                        _client.UpdateOrInsertTblStyleImageCompleted += _client_UpdateOrInsertTblStyleImageCompleted;
                    }
                };

            }
            catch { }
        }

        private void _client_UpdateOrInsertTblStyleImageCompleted(object sender, UpdateOrInsertTblStyleImageCompletedEventArgs e)
        {

        }

        public void GetStyleTNAdata()
        {
            /*
            try
            {
                SelectedMainRow.StyleTnaList.Clear();
                Loading = true;
                ProductionClient.GetTblStyleTNAHeaderAsync(SelectedMainRow.Iserial, SeasonPerRow.Iserial);
            } catch { }
            */
        }

        public void GetStyleTNAdataWithoutSeason()
        {
            SelectedMainRow.StyleTnaList.Clear();

            Loading = true;
            ProductionClient.GetTblStyleTNAHeaderAsync(SelectedMainRow.Iserial, -1);
        }

        public void getSml()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;

            Client.GetSmlAsync(0, int.MaxValue, SelectedMainRow.Iserial, SortBy, null, null);
        }

        public void GetTNADetaildata()
        {
            SelectedTnaRow.DetailList.Clear();
            if (SelectedTnaRow.Iserial == 0)
            {
               // AddNewTNADetailRow();
            }
            else
            {
                Loading = true;
                ProductionClient.GetTblStyleTNADetailAsync(SelectedTnaRow.Iserial);
            }

        }

        //public void GetTNADetailColor()
        //{
        //    SelectedTnaRow.StyleTNAColorDetailList.Clear();
        //    Loading = true;
        //    ProductionClient.GetTblStyleTNAColorDetailAsync(SelectedTnaRow.Iserial);
        //}

        public void AddNewStyleTNARow(bool checkLastRow)
        {
            var newrow = new TblStyleTNAHeaderViewModel
            {
                TblStyle = SelectedMainRow.Iserial,
                SeasonPerRow = SelectedMainRow.SeasonPerRow,
                TblLkpSeason = SelectedMainRow.TblLkpSeason,
                TargetCostPrice = SelectedMainRow.TargetCostPrice,
                CurrencyPerRow = CurrencyList.FirstOrDefault(w => w.LocalCurrency == true),
                TblRetailOrderProductionType = RetailOrderProductionTypeList.FirstOrDefault().Iserial

            };
            // SelectedMainRow.StyleTnaList.Insert(currentRowIndex + 1, newrow);
            SelectedTnaRow = newrow;
            if (SelectedTnaRow.Iserial != 0)
            {
                AddNewTNADetailRow();
            }
            // GetTNADetaildata();
        }

        public void AddNewStyleTNAColorDetailRow(bool checkLastRow)
        {

            if (!SelectedTnaRow.StyleTNAColorDetailList.Any())
            {
                getPendingTnaColors();
            }
        }

        public void AddNewTNADetailRow(TblStyleTNADetailViewModel row = null)
        {
            if (SelectedTnaRow != null)
            {
                if (row == null)
                {
                    if (!SelectedTnaRow.DetailList.Any())
                    {
                        foreach (var item in StyleTNAList)
                        {
                            var newrow = new TblStyleTNADetailViewModel
                            {
                                TblStyleTNAHeader = SelectedMainRow.Iserial,
                                TblStyleTNA = item.Iserial,
                                StyleTNAPerRow = item
                            };
                            SelectedTnaRow.DetailList.Add(newrow);
                        }
                    }
                }
                else
                {
                    try
                    {
                        StyleTNAListTemp.Clear();
                        //Validate Max_Repeated TNA
                        //foreach (var item in StyleTNAList)
                        //{
                        //    int _CurrentCount = SelectedTnaRow.DetailList.Where(x => x.StyleTNAPerRow.Iserial == item.Iserial).Count();
                        //    if (_CurrentCount < item.MaxRepeated)
                        //    {
                        //        StyleTNAListTemp.Add(item);
                                
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    var index = SelectedTnaRow.DetailList.IndexOf(row);
                    var newrow = new TblStyleTNADetailViewModel
                    {
                        TblStyleTNAHeader = SelectedMainRow.Iserial,
                        ExpectedDeliveryDate = null,
                    };
                    SelectedTnaRow.DetailList.Add(newrow);
                }
            }
        }

        public void SaveStyleTNARow()
        {
            /*
            var save = SelectedTnaRow.Iserial == 0;
            if (SelectedTnaRow != null)
            {
                if (!(CustomePermissions.Any(w => w.Code == "UpdateStyleTNAActualDates")))
                {
                    //if (SelectedTnaRow.TblStyleTNAStatus == 1 || SelectedTnaRow.TblStyleTNAStatus == 5)
                    //{
                    //    MessageBox.Show("Cannot Edit Approved Or Canceled TNA");
                    //    return;
                    //}

                    if (save)
                    {
                        if (!(CustomePermissions.Any(w => w.Code == "AddStyleTNA") || CustomePermissions.Any(w => w.Code == "LimitedAddStyleTNA")))
                        {
                            MessageBox.Show(strings.AllowAddMsg);
                            return;
                        }
                    }
                    else
                    {
                        if (!(CustomePermissions.Any(w => w.Code == "LimitedAddStyleTNA")))
                        {
                            //if (SelectedTnaRow.TblStyleTNAStatus == 1 || SelectedTnaRow.TblStyleTNAStatus == 5)
                            //{
                            //    MessageBox.Show(strings.AllowUpdateMsg);
                            //    return;
                            //}

                        }

                    }

                    //if (CustomePermissions.Any(w => w.Code == "LimitedAddStyleTNA"))
                    //{
                    //    foreach (var item in SelectedTnaRow.DetailList)
                    //    {
                    //        if (item.ActualDeliveryDate != null)
                    //        {
                    //            MessageBox.Show(strings.AllowAddMsg);
                    //            return;
                    //        }

                    //    }
                    //}
                }
                else
                {

                }
                SelectedTnaRow.SeasonPerRow = SeasonPerRow;
                SelectedTnaRow.TblLkpSeason = SeasonPerRow.Iserial;
                SelectedTnaRow.TblStyle = SelectedMainRow.Iserial;
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedTnaRow, new ValidationContext(SelectedTnaRow, null, null), valiationCollection, true);
                if (isvalid && !Loading)//&& !SelectedMainRow.LockedRow)
                {

                    var saveRow = new ProductionService.TblStyleTNAHeader();
                    saveRow.InjectFrom(SelectedTnaRow);
                    saveRow.TblStyleTNADetails = new ObservableCollection<ProductionService.TblStyleTNADetail>();
                    saveRow.TblStyleTNAColorDetails = new ObservableCollection<ProductionService.TblStyleTNAColorDetail>();
                    foreach (var item in SelectedTnaRow.DetailList)
                    {
                        isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);
                        if (!isvalid)
                        {
                            // MessageBox.Show("Data Is Not Valid");
                            // return;
                        }
                        else
                        {
                            //Add Attachment
                            ProductionService.TblStyleTNADetail de = (new ProductionService.TblStyleTNADetail().InjectFrom(item)) as ProductionService.TblStyleTNADetail;
                            de.TblStyleTNADetailAttachments = new ObservableCollection<ProductionService.TblStyleTNADetailAttachment>();
                            if(item.TblStyleTNADetailAttachment != null)
                            {
                                foreach (var File in item.TblStyleTNADetailAttachment)
                                {
                                    ProductionService.TblStyleTNADetailAttachment fl = (new ProductionService.TblStyleTNADetailAttachment().InjectFrom(File)) as ProductionService.TblStyleTNADetailAttachment;
                                    de.TblStyleTNADetailAttachments.Add(fl);
                                }
                            }
                            saveRow.TblStyleTNADetails.Add(de);
                            
                        }
                    }

                    foreach (var item in SelectedTnaRow.StyleTNAColorDetailList)
                    {
                        isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);
                        if (!isvalid)
                        {
                            //MessageBox.Show("Data Is Not Valid");
                            //return;

                        }
                        else
                        {
                            saveRow.TblStyleTNAColorDetails.Add(new ProductionService.TblStyleTNAColorDetail().InjectFrom(item) as ProductionService.TblStyleTNAColorDetail);
                        }

                    }

                    Loading = true;
                    CopyOfSelectedTnaRow = null;
                    ProductionClient.UpdateOrInsertTblStyleTNAHeaderAsync(saveRow, save, 0, LoggedUserInfo.Iserial);

                }
                else
                {
                    MessageBox.Show("Data Is Not Valid");
                    return;
                }
            }
            */
        }

        public void GetStyleTNALockup()
        {
           /*
            ProductionClient.GetTblStyleLookupCompleted += (s, sv) =>
            {
                //
                StyleTNAListTemp.Clear();
                StyleTNAList.Clear();
                foreach (var row in sv.Result)
                {
                    if (StyleTNAList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleTNAList.Add(row);
                    }

                    if (StyleTNAListTemp.All(x => x.Iserial != row.Iserial))
                    {
                        StyleTNAListTemp.Add(row);
                    }
                }
                StyleTNAListFilterList = new ObservableCollection<ColumnFilterControl.CollectionTemp>();
                foreach (var item in StyleTNAList)
                {

                    StyleTNAListFilterList.Add(new ColumnFilterControl.CollectionTemp()
                    {
                        ItemValue = item.Iserial.ToString(),
                        Name = item.Ename
                    });

                }
                StyleTNAListFilterList = StyleTNAListFilterList;

                SeasonPerRow = SelectedMainRow.SeasonPerRow;
                GetStyleTNAdata();

            };
            ProductionClient.GetTblStyleLookupAsync(SelectedMainRow.Iserial);
            */
        }

        private ObservableCollection<ProductionService.TblStyleTNA> _StyleTNA;

        public ObservableCollection<ProductionService.TblStyleTNA> StyleTNAList
        {
            get { return _StyleTNA ?? (_StyleTNA = new ObservableCollection<ProductionService.TblStyleTNA>()); }
            set { _StyleTNA = value; RaisePropertyChanged("StyleTNAList"); }
        }

        private ObservableCollection<ProductionService.TblStyleTNA> _StyleTNATemp;

        public ObservableCollection<ProductionService.TblStyleTNA> StyleTNAListTemp
        {
            get { return _StyleTNATemp ?? (_StyleTNATemp = new ObservableCollection<ProductionService.TblStyleTNA>()); }
            set { _StyleTNATemp = value; RaisePropertyChanged("StyleTNAListTemp"); }
        }


        private ObservableCollection<LkpData.TblStyleTNARouteStatu> _StyleTNAStatusList;

        public ObservableCollection<LkpData.TblStyleTNARouteStatu> StyleTNAStatusList
        {
            get { return _StyleTNAStatusList ?? (_StyleTNAStatusList = new ObservableCollection<LkpData.TblStyleTNARouteStatu>()); }
            set { _StyleTNAStatusList = value; RaisePropertyChanged("StyleTNAStatusList"); }
        }

        
        private ObservableCollection<LkpData.TblStyleSpecType> _StyleSpecTypesList;

        public ObservableCollection<LkpData.TblStyleSpecType> StyleSpecTypesList
        {
            get { return _StyleSpecTypesList ?? (_StyleSpecTypesList = new ObservableCollection<LkpData.TblStyleSpecType>()); }
            set { _StyleSpecTypesList = value; RaisePropertyChanged("StyleSpecTypesList"); }
        }


        private ObservableCollection<LkpData.tblTechPackPart> _StyleTechPackPart;

        public ObservableCollection<LkpData.tblTechPackPart> StyleTechPackPartList
        {
            get { return _StyleTechPackPart ?? (_StyleTechPackPart = new ObservableCollection<LkpData.tblTechPackPart>()); }
            set { _StyleTechPackPart = value; RaisePropertyChanged("StyleTechPackPartList"); }
        }

        private List<LkpData.tblTechPackPart> _StyleTechPackPartTemp;

        public List<LkpData.tblTechPackPart> StyleTechPackPartListTemp
        {
            get { return _StyleTechPackPartTemp ?? (_StyleTechPackPartTemp = new List<LkpData.tblTechPackPart>()); }
            set { _StyleTechPackPartTemp = value; RaisePropertyChanged("StyleTechPackPartListTemp"); }
        }

        private ObservableCollection<TblStyleFabricComposition> _styleFabricCompositionList;

        public ObservableCollection<TblStyleFabricComposition> StyleFabricCompositionList
        {
            get { return _styleFabricCompositionList; }
            set { _styleFabricCompositionList = value; RaisePropertyChanged("StyleFabricCompositionList"); }
        }

        private void DetailsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSalesOrderViewModel item in e.NewItems)
                    item.PropertyChanged += Rfqitem_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSalesOrderViewModel item in e.OldItems)
                    item.PropertyChanged -= Rfqitem_PropertyChanged;
        }

        private void Rfqitem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == "TblSupplier")
            {
                if (SelectedDetailRow.SalesOrderType == (int)SalesOrderType.Rfq || SelectedDetailRow.SalesOrderType == (int)SalesOrderType.AdvancedSampleRequest)
                {
                    Client.GetSamplesRequestBySupplierAsync(SelectedDetailRow.TblStyle, SelectedDetailRow.TblSupplier);
                }
            }
        }

        #region GettingData

        public void GetSeasonalMasterList()
        {
            Client.GetSeasonalMasterListNotLinkedToSalesorderAsync(SelectedDetailRow.Iserial);
        }

        public void getPendingTnaColors()
        {
            Client.GetSeasonalMasterListNotLinkedToStyleTnaByStyleAsync(SelectedMainRow.Iserial);
        }

        public void GetDirection()
        {
            if (SelectedMainRow.Brand != null && SelectedMainRow.TblLkpBrandSection != null)
            {
                lkpClient.GetTblDirectionLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection);
            }
        }

        public void GetStyleTheme()
        {
            if (SelectedMainRow.Brand != null && SelectedMainRow.TblLkpBrandSection != null && SelectedMainRow.TblLkpSeason != null)
            {
                lkpClient.FamilyCategory_GetTblThemeLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, (int)SelectedMainRow.TblLkpSeason);
            }
        }

        public void GetSubFamily()
        {
            if (SelectedMainRow.TblFamily != null && SelectedMainRow.Brand != null && SelectedMainRow.TblLkpBrandSection != null)
            {
                lkpClient.GetTblSubFamilyLinkAsync((int)SelectedMainRow.TblFamily, SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection);
            }
        }

        public void GetSeason()
        {
            var seasonClient = new CRUD_ManagerServiceClient();
            seasonClient.GetAllSeasonsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (SeasonList.All(x => x.Iserial != row.Iserial))
                    {
                        SeasonList.Add(new TblLkpSeason().InjectFrom(row) as TblLkpSeason);
                    }
                }
            };
            seasonClient.GetAllSeasonsAsync();
        }

        public void GetDesign()
        {
            var designClient = new CRUD_ManagerServiceClient();
            //designClient.GetGenericCompleted += (s, sv) =>
            //{
            //    foreach (var row in sv.Result)
            //    {
            //        if (DesignList.All(x => x.Iserial != row.Iserial))
            //        {
            //            DesignList.Add(new GenericTable().InjectFrom(row) as GenericTable);
            //        }
            //    }
            //};
            //designClient.GetGenericAsync("tbl_lkp_FabricDesignes", "%%", "%%", "%%", "Iserial", "ASC");

            designClient.FamilyCategory_GetTblFabricDesignCompleted += (s, sv) =>
             {
                 foreach (var row in sv.Result)
                 {
                     if (DesignList.All(x => x.Iserial != row.Iserial))
                     {
                         DesignList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                     }
                 }
             };
            designClient.FamilyCategory_GetTblFabricDesignAsync(DesignList.Count, 1000, "", "", null);
        }

        public void GetSizeGroup()
        {
            Client.GetTblSizeGroupAsync(0, int.MaxValue, "it.Iserial", null, null);
        }

        //public void GetTblStyleStatus()
        //{
        //    var styleStatusClient = new CRUD_ManagerServiceClient();
        //    styleStatusClient.GetGenericCompleted += (s, sv) =>
        //    {
        //        foreach (var row in sv.Result)
        //        {
        //            if (StyleStatusList.All(x => x.Iserial != row.Iserial))
        //            {
        //                StyleStatusList.Add(new GenericTable().InjectFrom(row) as GenericTable);
        //            }
        //        }
        //    };
        //    styleStatusClient.GetGenericAsync("TblStyleStatus", "%%", "%%", "%%", "Iserial", "ASC");
        //}

        public void UploadStyleImage() {
            lkpClient.UploadStyleImageAsync(SelectedMainRow.Iserial, SelectedMainRow.galarylink, SelectedMainRow.StyleImage);
            lkpClient.UploadStyleImageCompleted += (s, sv) => { };
        }

        public void GetTblStyleStatus(int AddMode = 0)
        {
            var styleStatusClient = new CRUD_ManagerServiceClient();
            //styleStatusClient.GetGenericCompleted += (s, sv) =>
            //{
            //    foreach (var row in sv.Result)
            //    {
            //        if (StyleStatusList.All(x => x.Iserial != row.Iserial))
            //        {
            //            StyleStatusList.Add(new GenericTable().InjectFrom(row) as GenericTable);
            //        }
            //    }
            //};
            //styleStatusClient.GetGenericAsync("TblStyleStatus", "%%", "%%", "%%", "Iserial", "ASC");


            styleStatusClient.FamilyCategory_GetTblStyleStatusCompleted += (s, sv) =>
            {
                if (AddMode == 1)
                    StyleStatusList.Clear();
                foreach (var row in sv.Result)
                {
                    if (StyleStatusList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleStatusList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    }
                }
            };
            styleStatusClient.FamilyCategory_GetTblStyleStatusAsync(StyleStatusList.Count, 1000, "", "", null);
        }

        public void GetTblStyleCategory()
        {
            var styleStatusClient = new CRUD_ManagerServiceClient();
            styleStatusClient.GetGenericCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (StyleCategoryList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleCategoryList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    }
                }
            };
            styleStatusClient.GetGenericAsync("TblStyleCategory", "%%", "%%", "%%", "Iserial", "ASC");
        }

        public void GetTblStyleSpecGenericFabric()
        {
            var styleSpecGenericFabricClient = new CRUD_ManagerServiceClient();
            styleSpecGenericFabricClient.GetGenericCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (StyleSpecGenericFabricList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleSpecGenericFabricList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    }
                }
            };
            styleSpecGenericFabricClient.GetGenericAsync("TblGenericFabric", "%%", "%%", "%%", "Iserial", "ASC");
        }

        public void GetTBLTechPackStatus()
        {
            var styleTechPackStatusClient = new CRUD_ManagerServiceClient();
            styleTechPackStatusClient.GetGenericCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (StyleTechPackStatusList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleTechPackStatusList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    }
                }
            };
            styleTechPackStatusClient.GetGenericAsync("TBLTechPackStatus", "%%", "%%", "%%", "Iserial", "ASC");
        }

        public void GetTblStyleTheme()
        {
            var styleThemeClient = new CRUD_ManagerServiceClient();
            styleThemeClient.GetGenericCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (SelectedMainRow.StyleThemeList.All(x => x.Iserial != row.Iserial))
                    {
                        SelectedMainRow.StyleThemeList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    }
                }
            };
            styleThemeClient.GetGenericAsync("TblSalesOrderColorTheme", "%%", "%%", "%%", "Iserial", "ASC");
        }

        internal void GetFamilyLink()
        {
            if (SelectedMainRow.Brand != null && SelectedMainRow.TblLkpBrandSection != null)
            {
                var familyLinkClientClient = new LkpData.LkpDataClient();
                familyLinkClientClient.GetTblFamilyLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection);
                familyLinkClientClient.GetTblFamilyLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SelectedMainRow.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial))
                        {
                            SelectedMainRow.FamilyList.Add(row.TblFamily1);
                        }
                    }
                };
            }
        }

        internal void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblStyleAsync(MainRowList.Count, 
                PageSize, (int)SalesOrderType, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial,
                StyletnaOption, BrandFilter,
                SelectedBrandSectionFilterList, SelectedDirectionFilterList,
                SelectedStyleCategoryFilterList, SelectedFamilyFilterList,
                SelectedSubFamilyFilterList);

        }

        public void GetMainTempData()
        {
            MainRowListTemp.Clear();

            Client.GetPendingStyleAsync(LoggedUserInfo.Iserial, (int)SalesOrderType, PenStatus);
        }

        internal void CopyProduct()
        {
            var newRow = new TblStyle();
            newRow.InjectFrom(SelectedMainRow);

            newRow.Iserial = 0;
            newRow.StyleCode = "";
            newRow.RefStyleCode = "";


            Loading = true;
            Client.UpdateOrInsertTblStyleAsync(newRow, true, -3, LoggedUserInfo.WFM_UserName, "", "");


        }

        public void GetMaindataFull(DataGrid mainGrid)
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Export = true;
            Loading = true;
            ExportGrid = mainGrid;
            Client.GetTblStyleAsync(MainRowList.Count, int.MaxValue, (int)SalesOrderType, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial, StyletnaOption,
                BrandFilter,
                SelectedBrandSectionFilterList, SelectedDirectionFilterList,
                SelectedStyleCategoryFilterList, SelectedFamilyFilterList,
                SelectedSubFamilyFilterList);
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetTblSalesOrderAsync(SelectedMainRow.DetailsList.Count(x => x.Iserial != 0), PageSize, SelectedMainRow.Iserial, (int)SalesOrderType, DetailSortBy, DetailFilter, DetailValuesObjects);
            Loading = true;
        }

        public void GetDetaildataFull(DataGrid detaildatagrid)
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Export = true;
            Loading = true;
            ExportGrid = detaildatagrid;
            Client.GetTblSalesOrderAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial, (int)SalesOrderType, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        public void GetSalesOrderLookups()
        {
            if (SelectedMainRow != null)
            {
                Loading = true;
                SelectedMainRow.DetailsList.Clear();
                Client.GetTblColorLinkAsync(0, int.MaxValue, SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, (int)SelectedMainRow.TblLkpSeason, "it.TblColor", null, null);
                Client.GetTblSalesOrderColorThemeAsync(0, int.MaxValue, (int)SelectedMainRow.TblLkpSeason, SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, "it.Iserial", null, null);
                if (SelectedMainRow.TblSizeGroup != null)
                    Client.GetTblSizeCodeAsync(0, int.MaxValue, (int)SelectedMainRow.TblSizeGroup);

                if (SeasonalMasterListCompleted)
                {
                }
                else
                {
                    GetDetailData();
                }
                GetStyleTNAdataWithoutSeason();
            }
        }

        public void GetTechPackLookUp()
        {
            try
            {
                if (SelectedMainRow.TblSizeGroup != null)
                    Client.GetTblSizeCodeAsync(0, int.MaxValue, (int)SelectedMainRow.TblSizeGroup);

                Client.GetTblColorLinkAsync(0, int.MaxValue, SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, (int)SelectedMainRow.TblLkpSeason, "it.TblColor", null, null);
            }
            catch { }
        }


        public void GetSalesOrderColors()
        {
            if (SelectedDetailRow != null) Client.GetTblSalesOrderColorAsync(SelectedDetailRow.Iserial);
        }

        public void GetSalesOrderNotes()
        {
            SelectedDetailRow.SalesOrderNotesList.Clear();
            if (SelectedDetailRow != null) Client.GetTblSalesOrderNotesAsync(0, int.MaxValue, SelectedDetailRow.Iserial, "it.Iserial", null, null);
        }

        public void GetSalesOrderOperations()
        {
            if (SelectedDetailRow != null) Client.GetTblSalesOrderOperationAsync(SelectedDetailRow.Iserial);
        }

        internal void GetSalesOrderBom()
        {
            if (SelectedDetailRow != null) Client.GetBomAsync(SelectedDetailRow.Iserial);
        }

        internal void GetTechPackBom()
        {
            if (SelectedMainRow != null) Client.GetTechPackBomAsync(SelectedMainRow.Iserial);
        }

        internal void GetStyleImage()
        {
            if (SelectedMainRow != null) Client.GetStyleMainImageAsync(SelectedMainRow.Iserial);
        }

        #endregion GettingData

        #region FilterData

        private string _brandFilterField;
        public string BrandFilter
        {
            get
            {
                return _brandFilterField;
            }
            set
            {
                if ((ReferenceEquals(_brandFilterField, value) != true))
                {
                    if (value != null) _brandFilterField = value.Trim();
                    RaisePropertyChanged("BrandFilter");
                    // UpdatedStyleCode();
                }
            }
        }

        ObservableCollection<TblLkpBrandSection> _brandSectionFilterList = new ObservableCollection<TblLkpBrandSection>();
        public ObservableCollection<TblLkpBrandSection> BrandSectionFilterList
        {
            get { return _brandSectionFilterList; }
            set { _brandSectionFilterList = value; RaisePropertyChanged(nameof(BrandSectionFilterList)); }
        }

        ObservableCollection<TblLkpBrandSection> _selectedBrandSectionFilterList = new ObservableCollection<TblLkpBrandSection>();
        public ObservableCollection<TblLkpBrandSection> SelectedBrandSectionFilterList
        {
            get { return _selectedBrandSectionFilterList; }
            set { _selectedBrandSectionFilterList = value; RaisePropertyChanged(nameof(SelectedBrandSectionFilterList)); }
        }


        ObservableCollection<TblLkpDirection> _directionFilterList = new ObservableCollection<TblLkpDirection>();
        public ObservableCollection<TblLkpDirection> DirectionFilterList
        {
            get { return _directionFilterList; }
            set { _directionFilterList = value; RaisePropertyChanged(nameof(DirectionFilterList)); }
        }

        ObservableCollection<TblLkpDirection> _selectedDirectionFilterList = new ObservableCollection<TblLkpDirection>();
        public ObservableCollection<TblLkpDirection> SelectedDirectionFilterList
        {
            get { return _selectedDirectionFilterList; }
            set { _selectedDirectionFilterList = value; RaisePropertyChanged(nameof(SelectedDirectionFilterList)); }
        }

        ObservableCollection<TblStyleCategory> _styleCategoryFilterList = new ObservableCollection<TblStyleCategory>();
        public ObservableCollection<TblStyleCategory> StyleCategoryFilterList
        {
            get { return _styleCategoryFilterList; }
            set { _styleCategoryFilterList = value; RaisePropertyChanged(nameof(StyleCategoryFilterList)); }
        }

        ObservableCollection<TblStyleCategory> _selectedStyleCategoryFilterList = new ObservableCollection<TblStyleCategory>();
        public ObservableCollection<TblStyleCategory> SelectedStyleCategoryFilterList
        {
            get { return _selectedStyleCategoryFilterList; }
            set { _selectedStyleCategoryFilterList = value; RaisePropertyChanged(nameof(SelectedDirectionFilterList)); }
        }
        ObservableCollection<TblFamily> _familyFilterList = new ObservableCollection<TblFamily>();
        public ObservableCollection<TblFamily> FamilyFilterList
        {
            get { return _familyFilterList; }
            set { _familyFilterList = value; RaisePropertyChanged(nameof(FamilyFilterList)); }
        }

        ObservableCollection<TblFamily> _selectedfamilyFilterList = new ObservableCollection<TblFamily>();
        public ObservableCollection<TblFamily> SelectedFamilyFilterList
        {
            get { return _selectedfamilyFilterList; }
            set { _selectedfamilyFilterList = value; RaisePropertyChanged(nameof(SelectedFamilyFilterList)); }
        }

        ObservableCollection<TblSubFamily> _subfamilyFilterList = new ObservableCollection<TblSubFamily>();
        public ObservableCollection<TblSubFamily> SubFamilyFilterList
        {
            get { return _subfamilyFilterList; }
            set { _subfamilyFilterList = value; RaisePropertyChanged(nameof(SubFamilyFilterList)); }
        }

        ObservableCollection<TblSubFamily> _selectedsubfamilyFilterList = new ObservableCollection<TblSubFamily>();
        public ObservableCollection<TblSubFamily> SelectedSubFamilyFilterList
        {
            get { return _selectedsubfamilyFilterList; }
            set { _selectedsubfamilyFilterList = value; RaisePropertyChanged(nameof(SelectedSubFamilyFilterList)); }
        }

        #endregion
        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblStyleViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblStyleViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == "Brand")
            {
                if (SelectedMainRow.Brand != null)
                {
                    var brandSectionClient = new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);

                        }
                        BrandSectionListLink = sv.Result;
                    };
                    brandSectionClient.GetTblBrandSectionLinkAsync(SelectedMainRow.Brand, LoggedUserInfo.Iserial);
                }
                SelectedMainRow.FamilyList.Clear();

                GetFamilyLink();
            }
            if (e.PropertyName == "TblLkpBrandSection")
            {
                // GetFamilyLink();
            }
        }

        #region AutoCompleteEvents

        private void OnSupplierFabricsCompleted()
        {
            var handler = SupplierFabricsCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnMainFabricCompleted()
        {
            var handler = FabricItemCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion AutoCompleteEvents

        #region AddingRows

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (ViewModelAddMode != 1)
                {
                    if (!isvalid)
                    {
                        return;
                    }
                }
            }

            var firstOrDefault = BrandList.FirstOrDefault();

            var newrow = new TblStyleViewModel
            {
                CreatedBy = LoggedUserInfo.WFM_UserName,
                Finished = true,
                StyleCode = "Code"
            };
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
            if (firstOrDefault != null)
            {
                SelectedMainRow.FamilyList.Clear();

                newrow.Brand = firstOrDefault.Brand_Code;
                newrow.ManualCode = firstOrDefault.ManualCode;
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var brandRow = BrandList.SingleOrDefault(x => x.Brand_Code == SelectedMainRow.Brand);
            var TnaList = SelectedMainRow.DetailsList.Select(w => w.TblStyleTNAHeader);
            if (brandRow.RequireTNA && SelectedMainRow.DetailsList.Count == 0 && SalesOrderType.RetailPo == SalesOrderType)
            {
                return;
            }

            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }


            var lastTna = SelectedMainRow.StyleTnaList.Where(w => !TnaList.Contains(w.Iserial)).OrderByDescending(w => w.Iserial).FirstOrDefault();
            if (brandRow != null)
            {
                var newRow = new TblSalesOrderViewModel
                {
                    CreatedBy = LoggedUserInfo.WFM_UserName,
                    Customer = brandRow.CustomerCode,
                    TblStyle = SelectedMainRow.Iserial,
                    SalesOrderType = (int)SalesOrderType,
                    IsPlannedOrder = true,

                };
                if (brandRow.RequireTNA && SalesOrderType.RetailPo == SalesOrderType)
                {
                    if (lastTna != null)
                    {
                        newRow.TblStyleTNAHeader = lastTna.Iserial;
                        newRow.DeliveryDate = lastTna.DeliveryDate;
                        newRow.TblSupplier = lastTna.TblSupplier;
                        newRow.SupplierPerRow = lastTna.SupplierPerRow;
                        newRow.StyleTnaHeaderForRow = lastTna;
                        newRow.AccCost = lastTna.AccCost;
                        newRow.FabricCost = lastTna.FabricCost;
                        newRow.OperationCost = lastTna.OperationCost;
                        newRow.TblRetailOrderProductionType = lastTna.TblRetailOrderProductionType;

                    }

                    newRow.RetailOrderProductionTypePerRow = RetailOrderProductionTypeList.FirstOrDefault(w => w.Iserial == newRow.TblRetailOrderProductionType);

                }
                if (newRow.SalesOrderType == (int)SalesOrderType.AdvancedSampleRequest)
                {
                    newRow.Status = 1;
                }
                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newRow);

                SelectedDetailRow = newRow;
            }
        }

        public void AddBom(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.BomList.IndexOf(SelectedBomRow));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedBomRow, new ValidationContext(SelectedBomRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var styleColorList = new ObservableCollection<BomStyleColorViewModel>();
            if (SelectedDetailRow.SalesOrderColorList != null)
                foreach (var item in SelectedDetailRow.SalesOrderColorList)
                {
                    if (item.TblColor != null)
                        styleColorList.Add(new BomStyleColorViewModel
                        {
                            StyleColor = (int)item.TblColor,
                            TblColor1 = item.ColorPerRow
                        });
                }
            var bomSizesTemp = new ObservableCollection<TblBOMSize>();

            foreach (var item in Sizes)
            {
                bomSizesTemp.Add(new TblBOMSize
                {
                    StyleSize = item.SizeCode,
                    MaterialUsage = 1
                });
            }
            var mainFabric = SelectedDetailRow.BomList.Count == 0;
            if (styleColorList.Any())
            {
                //double rowindex = 10;
                //if (SelectedDetailRow.BomList.Count > 0)
                //{
                //    rowindex = (SelectedBomRow.BomRowIndex +
                //        SelectedDetailRow.BomList.ElementAt(currentRowIndex + 1).BomRowIndex) / 2;

                //}
                var newrow = new BomViewModel
                {
                    BomStyleColors = styleColorList,
                    TblSalesOrder = SelectedDetailRow.Iserial,
                    BOM_IsMainFabric = mainFabric,
                    IsAcc = false,
                    // BomRowIndex = rowindex,
                };

                GenericMapper.InjectFromObCollection(newrow.BomSizes, bomSizesTemp);

                SelectedDetailRow.BomList.Insert(currentRowIndex + 1, newrow);

                SelectedBomRow = newrow;
            }
        }

        public void AddTechPackBom(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.TechPackBomList.IndexOf(SelectedTechPackBomRow));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedTechPackBomRow, new ValidationContext(SelectedTechPackBomRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var styleColorList = new ObservableCollection<TechPackBomStyleColorViewModel>();
            if (SelectedMainRow.SelectedStyleSeasonalMaterColors != null)
                foreach (var item in SelectedMainRow.SelectedStyleSeasonalMaterColors)
                {
                    if (item != null)
                        styleColorList.Add(new TechPackBomStyleColorViewModel
                        {
                            StyleColor = item.Iserial,
                            TblColor1 = item
                        });
                }

                var bomSizesTemp = new ObservableCollection<TblTechPackBOMSize>();
                if (Sizes != null)
                foreach (var item in Sizes)
                {
                    bomSizesTemp.Add(new TblTechPackBOMSize
                    {
                        StyleSize = item.SizeCode,
                        MaterialUsage = 1
                    });
                }
                var mainFabric = SelectedDetailRow.TechPackBomList.Count == 0;
                var newrow = new TechPackBomViewModel
                {
                    BomStyleColors = styleColorList,
                    TblSalesOrder = 0,
                    BOM_IsMainFabric = mainFabric,
                    IsAcc = false
                };

                GenericMapper.InjectFromObCollection(newrow.BomSizes, bomSizesTemp);
                SelectedDetailRow.TechPackBomList.Insert(currentRowIndex + 1, newrow);
                SelectedTechPackBomRow = newrow;
            
        }

        public void AddNewSalesOrderColor(bool checkLastRow)
        {

            var currentRowIndex = (SelectedDetailRow.SalesOrderColorList.IndexOf(SelectedSalesOrderColorRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSalesOrderColorRow, new ValidationContext(SelectedSalesOrderColorRow, null, null), valiationCollection, true);

                if (SelectedDetailRow.SalesOrderColorList.Count(x => x.TblColor == SelectedSalesOrderColorRow.TblColor) > 1)
                {
                    MessageBox.Show("Cannot Duplicate Color");
                    return;
                }
                if (!isvalid)
                {
                    return;
                }
            }
            var sizeRatioList = new ObservableCollection<TblSalesOrderSizeRatio>();
            foreach (var sizeCode in Sizes.OrderBy(x => x.Id))
            {
                sizeRatioList.Add(new TblSalesOrderSizeRatio
                {
                    Size = sizeCode.SizeCode,
                });
            }
            DateTime? lastDeliveryDate = null;

            if (currentRowIndex != -1)
            {
                lastDeliveryDate = SelectedSalesOrderColorRow.DeliveryDate;
            }

            var newrow = new TblSalesOrderColorViewModel();
            GenericMapper.InjectFromObCollection(newrow.SalesOrderSizeRatiosList, sizeRatioList);
            newrow.DeliveryDate = lastDeliveryDate;
            newrow.TblSalesOrder = SelectedDetailRow.Iserial;

            SelectedDetailRow.SalesOrderColorList.Insert(currentRowIndex + 1, newrow);
            SelectedSalesOrderColorRow = newrow;
        }
        public void AddNewSalesOrderNotes(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.SalesOrderNotesList.IndexOf(SelectedSalesOrderNotesRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSalesOrderNotesRow, new ValidationContext(SelectedSalesOrderNotesRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var newrow = new TblSalesOrderNotesModel();
            newrow.TblSalesOrder = SelectedDetailRow.Iserial;

            SelectedDetailRow.SalesOrderNotesList.Insert(currentRowIndex + 1, newrow);
            SelectedSalesOrderNotesRow = newrow;
        }

        public void AddNewSalesOrderOperation(bool checkLastRow, int tblroutegroup = 0, TblRouteGroup routeGroupSelected = null)
        {
            var currentRowIndex = (SelectedDetailRow.SalesOrderOperationList.IndexOf(SelectedSalesOrderOperation));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSalesOrderOperation, new ValidationContext(SelectedSalesOrderOperation, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            double rowindex = 10;
            if (SelectedDetailRow.SalesOrderOperationList.Count > 0 && SelectedSalesOrderOperation != null)
            {
                if (SelectedSalesOrderOperation != SelectedDetailRow.SalesOrderOperationList.LastOrDefault())
                {
                    rowindex = (SelectedSalesOrderOperation.RowIndex +
                        SelectedDetailRow.SalesOrderOperationList.ElementAt(currentRowIndex + 1).RowIndex) / 2;
                }
                else
                {
                    rowindex = SelectedSalesOrderOperation.RowIndex + 10;
                }
            }
            else if (SelectedDetailRow.SalesOrderOperationList.Count < 0 && SelectedSalesOrderOperation == null)
            {
                rowindex = SelectedDetailRow.SalesOrderOperationList.LastOrDefault().RowIndex + 10;
            }

            var operationDetails = new ObservableCollection<TblSalesOrderOperationDetail>();

            foreach (var variable in SelectedDetailRow.SalesOrderColorList)
            {
                var newoperationDetails = new TblSalesOrderOperationDetail
                {
                    TblColor = variable.TblColor,
                    TblColor1 = variable.ColorPerRow,
                    OprCost = 0
                };
                operationDetails.Add(newoperationDetails);
            }

            var newrow = new TblSalesOrderOperationViewModel
            {
                TblSalesOrder = SelectedDetailRow.Iserial,
                RowIndex = rowindex,
                TblSalesOrderOperationDetailList = operationDetails
            };
            if (tblroutegroup != 0)
            {
                newrow.TblOperation = tblroutegroup;
                newrow.RouteGroupPerRow = routeGroupSelected;
            }
            SelectedDetailRow.SalesOrderOperationList.Insert(currentRowIndex + 1, newrow);
            SelectedSalesOrderOperation = newrow;
        }

        #endregion AddingRows

        #region DeleteRow

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
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Client.DeleteTblStyleAsync(
                                (TblStyle)new TblStyle().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
                        }
                    }
                }
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
                            Client.DeleteTblSalesOrderAsync(
                                (TblSalesOrder)new TblSalesOrder().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));

                            Loading = true;
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        internal void DeleteSalesOrderColor()
        {
            //if (Debugger.IsAttached)
            //{
            //}
            //else if (SelectedDetailRow.Status == 1)
            //{
            //    MessageBox.Show("Cannot edit because this order is approved");
            //    return;
            //}

            if (SelectedSalesOrderColorRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedSalesOrderColorRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblSalesOrderColorAsync(
           (TblSalesOrderColor)new TblSalesOrderColor().InjectFrom(row), SelectedDetailRow.SalesOrderColorList.IndexOf(row));
                        }
                        else
                        {
                            if (SelectedDetailRow.SalesOrderColorList.Any(x => x.Iserial == row.Iserial))
                            {
                                SelectedDetailRow.SalesOrderColorList.Remove(row);
                            }
                        }
                    }
                }
            }
        }


        internal void DeleteSalesOrderNotes()
        {
            if (SelectedSalesOrderNotesRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedSalesOrderNotesRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblSalesOrderNotesAsync(
           (TblSalesOrderNote)new TblSalesOrderNote().InjectFrom(row), SelectedDetailRow.SalesOrderNotesList.IndexOf(row));
                        }
                        else
                        {
                            if (SelectedDetailRow.SalesOrderNotesList.Any(x => x.Iserial == row.Iserial))
                            {
                                SelectedDetailRow.SalesOrderNotesList.Remove(row);
                            }
                        }
                    }
                }
            }
        }


        internal void DeleteSalesOrderOperations()
        {
            if (SelectedSalesOrderOperations != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedSalesOrderOperations)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblSalesOrderOperationAsync(
                                (TblSalesOrderOperation)new TblSalesOrderOperation().InjectFrom(row), SelectedDetailRow.SalesOrderOperationList.IndexOf(row));
                        }
                        else
                        {
                            SelectedDetailRow.SalesOrderOperationList.Remove(row);
                        }
                    }
                }
            }
        }

        internal void DeleteBom()
        {
            if (SelectedBomRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedBomRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteBomAsync(
                                (BOM)new BOM().InjectFrom(row), SelectedDetailRow.BomList.IndexOf(row));
                        }
                        else
                        {
                            SelectedDetailRow.BomList.Remove(row);
                        }
                    }
                }
            }
        }

        internal void DeleteTechPackBom()
        {
            if (SelectedTechPackBomRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedTechPackBomRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTechPackBomAsync((TechPackBOM)new TechPackBOM().InjectFrom(row), SelectedDetailRow.TechPackBomList.IndexOf(row));
                        }
                        else
                        {
                            SelectedDetailRow.TechPackBomList.Remove(row);
                        }
                    }
                }
            }
        }

        #endregion DeleteRow

        #region Search

        internal void SearchSupplierFabric(string fabric)
        {
            Client.SearchSupplierFabricAsync(fabric);
        }

        internal void SearchForStyleFabric(string fabric)
        {
            Client.GetFinishedFabricsAsync(fabric);
        }

        public void SearchForOperation(bool mainOperation)
        {
            var child = new BomOperations(this, mainOperation);
            child.Show();
        }

        #endregion Search

        #region Save

        public void SaveMainRow(string repeatedStyle = "")
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    //BrandList.FirstOrDefault(w => w.Brand_Code == SelectedMainRow.Brand).
                    //if (;
                    //{

                    //}
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblStyle();
                   
                    saveRow.InjectFrom(SelectedMainRow);
                    
                    Loading = true;
                    
                    //string StyleTheme = "";
                    //if (!string.IsNullOrEmpty(SelectedMainRow.Theme_Code))
                    //{
                    //    StyleTheme = SelectedMainRow.Theme_Code;
                    //    if (StyleTheme.Length > 3)
                    //        StyleTheme = StyleTheme.Substring(0, 3);
                    //}

                    if (string.IsNullOrEmpty(repeatedStyle))
                        Client.UpdateOrInsertTblStyleAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.WFM_UserName, "", "");
                    else
                        Client.UpdateOrInsertTblStyleAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.WFM_UserName, repeatedStyle, "");
                }
            }
        }

        public void SaveGeneratedSalesOrder()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var salesordertype = 0;
            if (SelectedPoToLink.SalesOrderType == 1 || SelectedPoToLink.SalesOrderType == 4)
            {
                salesordertype = 2;
            }
            else if (SelectedPoToLink.SalesOrderType == 3)
            {
                salesordertype = 1;
            }

            SelectedPoToLink.EntityKey = null;
            SelectedPoToLink.SalesOrderType = salesordertype;
            SelectedPoToLink.PoRef = SelectedPoToLink.Iserial;
            SelectedPoToLink.Iserial = 0;
            SelectedPoToLink.CreatedBy = LoggedUserInfo.WFM_UserName;
            SelectedPoToLink.IsPostedOnAxapta = false;
            SelectedPoToLink.Status = 0;
            SelectedPoToLink.IsFinancialApproved = false;
            SelectedPoToLink.IsRetailApproved = false;
            SelectedPoToLink.IsTechnicalApproved = false;
            foreach (var bomLine in SelectedPoToLink.BOMs)
            {
                bomLine.TblSalesOrder1 = null;
                if (bomLine.IsSupplierMaterial)
                {
                    SelectedPoToLink.BOMs.Remove(bomLine);
                }
                bomLine.IsSupplierMaterial = false;
                bomLine.Iserial = 0;
                foreach (var sizeBom in bomLine.TblBOMSizes)
                {
                    sizeBom.BOM1 = null;
                    sizeBom.Iserial = 0;
                    sizeBom.Bom = 0;
                }
            }

            foreach (var row in SelectedPoToLink.TblSalesOrderColors)
            {
                row.Iserial = 0;
                row.TblSalesOrder = 0;
                row.TblSalesOrder1 = null;
                foreach (var sizeRow in row.TblSalesOrderSizeRatios)
                {
                    sizeRow.TblSalesOrderColor1 = null;
                    sizeRow.TblSalesOrderColor = 0;
                    sizeRow.Iserial = 0;
                }
            }
            var newRow = new TblSalesOrder();
            if (SelectedPoToLink.SalesOrderType == 4)
            {
                newRow.IsSample = true;
            }
            newRow.InjectFrom(SelectedPoToLink);
            newRow.BOMs = new ObservableCollection<BOM>();

            foreach (var bomNewRow in SelectedPoToLink.BOMs)
            {
                var newBom = new BOM();

                newBom.InjectFrom(bomNewRow);
                newBom.TblBOMSizes = new ObservableCollection<TblBOMSize>();
                newBom.TblBOMStyleColors = new ObservableCollection<TblBOMStyleColor>();
                foreach (var variable in bomNewRow.TblBOMSizes.ToList())
                {
                    newBom.TblBOMSizes.Add(new TblBOMSize().InjectFrom(variable) as TblBOMSize);
                }
                foreach (var variable in bomNewRow.TblBOMStyleColors.ToList())
                {
                    newBom.TblBOMStyleColors.Add(new TblBOMStyleColor().InjectFrom(variable) as TblBOMStyleColor);
                }

                newRow.BOMs.Add(newBom);
            }

            newRow.TblSalesOrderColors = new ObservableCollection<TblSalesOrderColor>();
            foreach (var sizeColorNewRow in SelectedPoToLink.TblSalesOrderColors.Where(w => w.Canceled == false).ToList())
            {
                var salesOrderColorNewRow = new TblSalesOrderColor();
                var sizeRatioList =
        new ObservableCollection<TblSalesOrderSizeRatio>();
                GenericMapper.InjectFromObCollection(sizeRatioList, sizeColorNewRow.TblSalesOrderSizeRatios);

                salesOrderColorNewRow.InjectFrom(sizeColorNewRow);
                salesOrderColorNewRow.TblSalesOrderSizeRatios = sizeRatioList;
                newRow.TblSalesOrderColors.Add(salesOrderColorNewRow);
            }

            Client.UpdateOrInsertTblSalesOrderAsync(newRow, true, -2);

            //else
            //{
            //    SelectedPoToLink.EntityKey = null;
            //    SelectedPoToLink.SalesOrderType = 1;
            //    SelectedPoToLink.PoRef = SelectedPoToLink.Iserial;
            //    SelectedPoToLink.Iserial = 0;
            //    SelectedPoToLink.CreatedBy = LoggedUserInfo.WFM_UserName;
            //    SelectedPoToLink.IsPlannedOrder = false;
            //    SelectedPoToLink.IsPostedOnAxapta = false;
            //    SelectedPoToLink.Status = 0;

            //    foreach (var bomLine in SelectedPoToLink.BOMs)
            //    {
            //        bomLine.TblSalesOrder1 = null;
            //        if (bomLine.IsSupplierMaterial)
            //        {
            //            SelectedPoToLink.BOMs.Remove(bomLine);
            //        }

            //        bomLine.IsSupplierMaterial = false;
            //        bomLine.Iserial = 0;

            //        foreach (var sizeBom in bomLine.TblBOMSizes)
            //        {
            //            sizeBom.BOM1 = null;
            //            sizeBom.Iserial = 0;
            //            sizeBom.Bom = 0;
            //        }
            //    }

            //    foreach (var row in SelectedPoToLink.TblSalesOrderColors)
            //    {
            //        row.Iserial = 0;
            //        row.TblSalesOrder = 0;
            //        row.TblSalesOrder1 = null;
            //        foreach (var sizeRow in row.TblSalesOrderSizeRatios)
            //        {
            //            sizeRow.TblSalesOrderColor1 = null;
            //            sizeRow.TblSalesOrderColor = 0;
            //            sizeRow.Iserial = 0;
            //        }
            //    }
            //    var newRow = new TblSalesOrder();

            //    var newBom = new BOM();
            //    newRow.BOMs = new ObservableCollection<BOM>();
            //    //GenericMapper.InjectFromObCollection(newRow.BOMs, SelectedPoToLink.BOMs);

            //    foreach (var bomNewRow in SelectedPoToLink.BOMs)
            //    {
            //        GenericMapper.InjectFromObCollection(newBom.TblBOMSizes, bomNewRow.TblBOMSizes);
            //        GenericMapper.InjectFromObCollection(newBom.TblBOMStyleColors, bomNewRow.TblBOMStyleColors);
            //        newBom.InjectFrom(bomNewRow);
            //        newRow.BOMs.Add(newBom);
            //    }

            //    foreach (var sizeColorNewRow in SelectedPoToLink.TblSalesOrderColors)
            //    {
            //        var sizeRatioList =
            //        new ObservableCollection<TblSalesOrderSizeRatio>();
            //        var salesOrderColorNewRow = new TblSalesOrderColor();
            //        GenericMapper.InjectFromObCollection(sizeRatioList, sizeColorNewRow.TblSalesOrderSizeRatios);
            //        salesOrderColorNewRow.TblSalesOrderSizeRatios = sizeRatioList;
            //        salesOrderColorNewRow.InjectFrom(sizeColorNewRow);
            //        newRow.TblSalesOrderColors = new ObservableCollection<TblSalesOrderColor> { salesOrderColorNewRow };
            //    }

            //    newRow.InjectFrom(SelectedPoToLink);
            //    Client.UpdateOrInsertTblSalesOrderAsync(newRow, true, -2);
            //}
        }

        public void SaveDetailRow()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    var saveRow = new TblSalesOrder();
                    saveRow.InjectFrom(SelectedDetailRow);
                    //if (SelectedDetailRow.SalesOrderType == 1)
                    //{
                    //    if (SelectedMainRow.RetailTargetCostPrice < 1)
                    //    {
                    //        MessageBox.Show("Retail Price Can not Be 0");
                    //        return;
                    //    }

                    //    if (SelectedMainRow.TargetCostPrice < 1)
                    //    {
                    //        MessageBox.Show("Target Cost Can not Be 0");
                    //        return;
                    //    }
                    //}

                    Client.UpdateOrInsertTblSalesOrderAsync(saveRow, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        public void SaveMainList()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            foreach (var row in SelectedDetailRow.SalesOrderColorList)
            {
                if (row != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(row, new ValidationContext(row, null, null),
                        valiationCollection, true);

                    if (SelectedDetailRow.SalesOrderColorList.Count(x => x.TblColor == SelectedSalesOrderColorRow.TblColor) > 1)
                    {
                        MessageBox.Show("Cannot Duplicate Color");
                        return;
                    }

                    if (SelectedDetailRow.SalesOrderColorList.Any(x => x.Cost == 0 || x.Cost == null) && SelectedDetailRow.SalesOrderType != (int)SalesOrderType.SalesOrderPo)
                    {
                        MessageBox.Show("Cannot Save With 0 Cost");
                        return;
                    }

                    if (isvalid && !Loading) //&& !SelectedMainRow.LockedRow)
                    {
                        var save = row.Iserial == 0;
                        var saveRow = new TblSalesOrderColor();
                        saveRow.InjectFrom(row);
                        saveRow.TblSalesOrderSizeRatios = row.SalesOrderSizeRatiosList;
                        if (row == SelectedDetailRow.SalesOrderColorList.LastOrDefault())
                        {
                            Loading = true;
                        }
                        if (row.LocalCost == null || row.LocalCost == 0)
                        {
                            row.LocalCost = row.Cost * row.ExchangeRate;
                        }

                        Client.UpdateOrInsertTblSalesOrderColorAsync(saveRow, save,
                            SelectedDetailRow.SalesOrderColorList.IndexOf(row));
                    }
                }
            }
        }

        internal void SalesOrderColors()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (SelectedDetailRow != null)
            {
                if (SelectedDetailRow.SalesOrderColorList.Count(x => x.TblColor == SelectedSalesOrderColorRow.TblColor) > 1)
                {
                    MessageBox.Show("Cannot Duplicate Color");
                    return;
                }

                if (SelectedDetailRow.SalesOrderColorList.Any(x => x.Cost == 0 || x.Cost == null))
                {
                    MessageBox.Show("Cannot Save With 0 Cost");
                    return;
                }

                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSalesOrderColorRow, new ValidationContext(SelectedSalesOrderColorRow, null, null),
                    valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
                var newColorToSave = new TblSalesOrderColor();
                var save = SelectedSalesOrderColorRow.Iserial == 0;
                newColorToSave.InjectFrom(SelectedSalesOrderColorRow);
                newColorToSave.TblSalesOrderSizeRatios = SelectedSalesOrderColorRow.SalesOrderSizeRatiosList;
                Loading = true;
                if (newColorToSave.LocalCost == null || newColorToSave.LocalCost == 0)
                {
                    newColorToSave.LocalCost = newColorToSave.Cost * newColorToSave.ExchangeRate;
                }
                Client.UpdateOrInsertTblSalesOrderColorAsync(newColorToSave, save,
                    SelectedDetailRow.SalesOrderColorList.IndexOf(SelectedSalesOrderColorRow));
            }
        }

        internal void SalesOrderNotes()
        {
            if (SelectedDetailRow != null)
            {

                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSalesOrderNotesRow, new ValidationContext(SelectedSalesOrderNotesRow, null, null),
                    valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
                var newNotesToSave = new TblSalesOrderNote();
                var save = SelectedSalesOrderNotesRow.Iserial == 0;
                newNotesToSave.InjectFrom(SelectedSalesOrderNotesRow);

                Loading = true;
                Client.UpdateOrInsertTblSalesOrderNotesAsync(newNotesToSave, save,
                    SelectedDetailRow.SalesOrderNotesList.IndexOf(SelectedSalesOrderNotesRow));
            }
        }

        public void SaveSalesOrderOperations()
        {
            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(SelectedSalesOrderOperation,
                new ValidationContext(SelectedSalesOrderOperation, null, null), valiationCollection, true);
            if (isvalid)
            {
                var operations = new List<int>();
                foreach (var row in SelectedDetailRow.SalesOrderOperationList.Where(x => x.TblOperation != null))
                {
                    if (row.TblOperation != null) operations.Add((int)row.TblOperation);
                }
                var operationCountFlag = operations.Distinct().Count() ==
                                         SelectedDetailRow.SalesOrderOperationList.Count;
                if (operationCountFlag)
                {
                    var save = SelectedSalesOrderOperation.Iserial == 0;
                    var saverow = new TblSalesOrderOperation();
                    saverow.InjectFrom(SelectedSalesOrderOperation);
                    saverow.TblSalesOrderOperationDetails = SelectedSalesOrderOperation.TblSalesOrderOperationDetailList;

                    Client.UpdateOrInsertTblSalesOrderOperationAsync(saverow, save, SelectedDetailRow.SalesOrderOperationList.IndexOf(SelectedSalesOrderOperation));

                    return;
                }
            }

            MessageBox.Show("Data Is Not Valid");
        }

        public void SaveSalesOrderOperationsList()
        {
            var operations = new List<int>();
            foreach (var row in SelectedDetailRow.SalesOrderOperationList)
            {
                operations.Add((int)row.TblOperation);
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(row,
                    new ValidationContext(row, null, null), valiationCollection, true);
                if (!isvalid)
                {
                    return;
                }
            }
            var operationCountFlag = operations.Distinct().Count() ==
                                     SelectedDetailRow.SalesOrderOperationList.Count;
            if (operationCountFlag)
            {
                foreach (var row in SelectedDetailRow.SalesOrderOperationList)
                {
                    var save = row.Iserial == 0;
                    var saverow = new TblSalesOrderOperation();
                    saverow.InjectFrom(row);
                    saverow.TblSalesOrderOperationDetails = row.TblSalesOrderOperationDetailList;

                    Client.UpdateOrInsertTblSalesOrderOperationAsync(saverow, save, SelectedDetailRow.SalesOrderOperationList.IndexOf(row));
                }
                return;
            }

            MessageBox.Show("Data Is Not Valid");
        }

        internal void SaveBom()
        {
            var bomListToSave = new ObservableCollection<BOM>();
            foreach (var row in SelectedDetailRow.BomList)
            {
                var newrow = new BOM();
                newrow.InjectFrom(row);
                newrow.TblBOMSizes = row.BomSizes;
                newrow.TblBOMStyleColors = new ObservableCollection<TblBOMStyleColor>();
                GenericMapper.InjectFromObCollection(newrow.TblBOMStyleColors, row.BomStyleColors);

                newrow.TblSalesOrder1 = new TblSalesOrder();
                newrow.TblSalesOrder1.InjectFrom(SelectedDetailRow);
                if (row.IsAcc)
                {
                    foreach (var colorRow in newrow.TblBOMStyleColors)
                    {
                        colorRow.FabricColor = colorRow.DummyColor;
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                else
                {
                    foreach (var colorRow in newrow.TblBOMStyleColors)
                    {
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                if (!row.IsSupplierMaterial)
                {
                    if (row.ItemPerRow != null)
                    {
                    }
                }
                bomListToSave.Add(newrow);
            }

            Client.UpdateOrInsertBomAsync(bomListToSave, LoggedUserInfo.Iserial);
        }

        public void SaveBomRow()
        {
            var row = SelectedBomRow;

            var bomListToSave = new ObservableCollection<BOM>();

            var newrow = new BOM();
            newrow.InjectFrom(row);
            newrow.TblBOMSizes = row.BomSizes;

            newrow.TblBOMStyleColors = new ObservableCollection<TblBOMStyleColor>();
            if (row.BomStyleColors != null)
            {
                GenericMapper.InjectFromObCollection(newrow.TblBOMStyleColors, row.BomStyleColors);
                newrow.TblSalesOrder1 = new TblSalesOrder();
                newrow.TblSalesOrder1.InjectFrom(SelectedDetailRow);
                if (row.IsAcc)
                {
                    foreach (var colorRow in newrow.TblBOMStyleColors)
                    {
                        colorRow.FabricColor = colorRow.DummyColor;
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                else
                {
                    foreach (var colorRow in newrow.TblBOMStyleColors)
                    {
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                if (!row.IsSupplierMaterial)
                {
                    if (row.ItemPerRow != null)
                    {
                    }
                }
                bomListToSave.Add(newrow);

                var service = new CRUD_ManagerServiceClient();
                if (Loading)
                {
                    return;
                }
                Loading = true;
                service.UpdateOrInsertBomAsync(bomListToSave, LoggedUserInfo.Iserial);
                var index = SelectedDetailRow.BomList.IndexOf(SelectedBomRow);
                service.UpdateOrInsertBomCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }
                    Loading = false;
                    var rowww = SelectedDetailRow.BomList.ElementAt(index);
                    rowww.InjectFrom(sv.Result.FirstOrDefault());
                    rowww.BomStyleColors.Clear();
                    rowww.BomSizes.Clear();
                    var firstOrDefault = sv.Result.FirstOrDefault();
                    var colors = SelectedDetailRow.SalesOrderColorList.Select(e => e.TblColor);
                    if (firstOrDefault != null)
                    {
                        GenericMapper.InjectFromObCollection(rowww.BomStyleColors, firstOrDefault.TblBOMStyleColors.Where(w => colors.Contains(w.StyleColor)));
                        GenericMapper.InjectFromObCollection(rowww.BomSizes, firstOrDefault.TblBOMSizes);
                    }
                    Loading = false;
                };
            }
        }

        internal void SaveTechPackBom()
        {
            var bomListToSave = new ObservableCollection<TechPackBOM>();
            foreach (var row in SelectedDetailRow.TechPackBomList)
            {
                var newrow = new TechPackBOM();
                newrow.InjectFrom(row);
                newrow.TblTechPackBOMSizes = row.BomSizes;

                newrow.TblTechPackBOMStyleColors = new ObservableCollection<TblTechPackBOMStyleColor>();
                GenericMapper.InjectFromObCollection(newrow.TblTechPackBOMStyleColors, row.BomStyleColors);

                newrow.TblStyle = SelectedMainRow.Iserial;

                if (row.IsAcc)
                {
                    foreach (var colorRow in newrow.TblTechPackBOMStyleColors)
                    {
                        colorRow.FabricColor = colorRow.DummyColor;
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                else
                {
                    foreach (var colorRow in newrow.TblTechPackBOMStyleColors)
                    {
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                if (!row.IsSupplierMaterial)
                {
                    if (row.ItemPerRow != null)
                    {
                    }
                }
                bomListToSave.Add(newrow);
            }

            Client.UpdateOrInsertTechPackBomAsync(bomListToSave);
        }

        #endregion Save

        internal void GenerateSalesOrder()
        {
            Client.GetPoNotLinkToSalesorderAsync(SelectedMainRow.Iserial);
        }

        internal void GeneratePo()
        {
            Client.GetRfqNotLinkToPoAsync(SelectedMainRow.Iserial);
        }

        internal void PostToAx()
        {
            Client.PostBomToAxAsync(new TblSalesOrder().InjectFrom(SelectedDetailRow) as TblSalesOrder, LoggedUserInfo.Iserial);
        }

        internal void PrintBarcode(TblSalesOrderColorViewModel row)
        {
            var printingPage = new BarcodePrintPreview(row, SelectedMainRow, 2, (LoggedUserInfo.BarcodeSettingHeader.Code), true);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            printingPage.Show();
        }

        internal void Report()
        {
            var reportChild = new ReportsChildWindow(PermissionItemName.StyleCodingForm.ToString(), SelectedMainRow.Iserial);
            reportChild.Show();
        }

        internal void ReportSalesOrder()
        {
            var reportChild = new ReportsChildWindow("SalesOrder", SelectedDetailRow.Iserial);
            reportChild.Show();
        }

        internal void CalcRatio()
        {
            foreach (var tblSeasonalMasterListViewModel in SelectedDetailRow.SalesOrderColorList)
            {
                foreach (var tblSeasonalMasterListDetail in tblSeasonalMasterListViewModel.SalesOrderSizeRatiosList)
                {
                    tblSeasonalMasterListDetail.Ratio =
                    SelectedSalesOrderColorRow.SalesOrderSizeRatiosList.FirstOrDefault(
                                x => x.Size == tblSeasonalMasterListDetail.Size)
                            .Ratio;
                }
            }

            SaveMainList();
        }

        internal void GetStandardBom()
        {
            Client.GetTblStandardBOMHeaderAsync(StandardBomList.Count, PageSize, "it.Iserial", null, null, LoggedUserInfo.Iserial);
            Loading = true;
        }

        internal void GenerateBomFromStandard(TblStandardBomHeaderViewModel row)
        {
            if (row != null) Client.GetTblStandardBOMAsync(row.Iserial);
        }

        internal void GenerateSalesOrderFromSample()
        {
            Client.GetFullSalesOrderDataAsync(SelectedDetailRow.Iserial);
        }

        //internal void PrintSalesOrder()
        //{
        //    const string reportName = "Po";

        //    var paramters = new ObservableCollection<string>();

        //    if (SelectedDetailRow != null)
        //    {
        //        paramters.Add(SelectedDetailRow.Iserial.ToString(CultureInfo.InvariantCulture));
        //    }

        //    var reportViewmodel = new GenericReportViewModel();
        //    reportViewmodel.GenerateReport(reportName, paramters);
        //}

        internal void GetRequestForSampleEventData()
        {
            Loading = true;
            if (SelectedDetailRow != null)
            {
                SelectedDetailRow.SubEventList.Clear();
                Client.GetTblRequestForSampleEventAsync(SelectedDetailRow.Iserial);
            }
        }

        internal void DeleteRequestForSampleEventRow()
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
                            Client.DeleteTblRequestForSampleEventAsync((TblRequestForSampleEvent)new TblRequestForSampleEvent().InjectFrom(row), SelectedDetailRow.SubEventList.IndexOf(row));
                        }
                        else
                        {
                            SelectedDetailRow.SubEventList.Remove(row);
                        }
                    }
                }
            }
        }

        internal void AddNewSubEventRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.SubEventList.IndexOf(SelectedRequestForSampleEvent));

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
                     TblSalesOrder = SelectedDetailRow.Iserial,
                     UserPerRow = new TblAuthUser
                     {
                         Iserial = LoggedUserInfo.Iserial,
                         Code = LoggedUserInfo.Code,
                         UserName = LoggedUserInfo.WFM_UserName,
                         Ename = LoggedUserInfo.WFM_UserName
                     }

                     ,// UsersList.FirstOrDefault(x => x.Code == LoggedUserInfo.Code),
                     ApprovedBy = LoggedUserInfo.Iserial
                 };
            SelectedDetailRow.SubEventList.Insert(currentRowIndex + 1, newrow);
            SelectedRequestForSampleEvent = newrow;
        }

        internal void SaveSubEventRow()
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
                    Client.UpdateOrInsertTblRequestForSampleEventAsync(rowToSave, save, SelectedDetailRow.SubEventList.IndexOf(SelectedRequestForSampleEvent));
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        #region Props
        public string Subject { get; set; }

        public string Body { get; set; }

        private TblRequestForSampleEventViewModel _selectedRequestForSampleEvent;

        public TblRequestForSampleEventViewModel SelectedRequestForSampleEvent
        {
            get { return _selectedRequestForSampleEvent ?? (_selectedRequestForSampleEvent = new TblRequestForSampleEventViewModel()); }
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

        public ObservableCollection<TblCurrency> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _complexityGroupList;

        public ObservableCollection<GenericTable> ComplexityGroupList
        {
            get { return _complexityGroupList; }
            set { _complexityGroupList = value; RaisePropertyChanged("ComplexityGroupList"); }
        }

        private ObservableCollection<GenericTable> _factoryGroupList;

        public ObservableCollection<GenericTable> FactoryGroupList
        {
            get { return _factoryGroupList; }
            set { _factoryGroupList = value; RaisePropertyChanged("FactoryGroupList"); }
        }

        public event EventHandler ItemCompletedCompleted;

        public event EventHandler FabricItemCompleted;

        public event EventHandler SupplierItemCompleted;

        public event EventHandler ExportCompleted;

        public event EventHandler SupplierFabricsCompleted;

        private ObservableCollection<GenericTable> _bomCalcMethodList;

        public ObservableCollection<GenericTable> BomCalcMethodList
        {
            get { return _bomCalcMethodList; }
            set { _bomCalcMethodList = value; RaisePropertyChanged("BomCalcMethodList"); }
        }

        private ObservableCollection<GenericTable> _bomFabricTypeList;

        public ObservableCollection<GenericTable> FabricTypeList
        {
            get { return _bomFabricTypeList; }
            set { _bomFabricTypeList = value; RaisePropertyChanged("FabricTypeList"); }
        }

        private ObservableCollection<GenericTable> _RetailOrderProductionTypeList;

        public ObservableCollection<GenericTable> RetailOrderProductionTypeList
        {
            get { return _RetailOrderProductionTypeList; }
            set { _RetailOrderProductionTypeList = value; RaisePropertyChanged("RetailOrderProductionTypeList"); }
        }

        private ObservableCollection<GenericTable> _SalesPersonList;

        public ObservableCollection<GenericTable> SalesPersonList
        {
            get { return _SalesPersonList; }
            set { _SalesPersonList = value; RaisePropertyChanged("SalesPersonList"); }
        }


        private ObservableCollection<TblRouteGroup> _routeGroupList;

        public ObservableCollection<TblRouteGroup> RouteGroupList
        {
            get { return _routeGroupList; }
            set { _routeGroupList = value; RaisePropertyChanged("RouteGroupList"); }
        }

        private ObservableCollection<GenericTable> _uomList;

        public ObservableCollection<GenericTable> UomList
        {
            get { return _uomList; }
            set { _uomList = value; RaisePropertyChanged("UomList"); }
        }

        private ObservableCollection<TblSalesOrderColorTheme> _themesList;

        public ObservableCollection<TblSalesOrderColorTheme> ThemesList
        {
            get { return _themesList ?? (_themesList = new ObservableCollection<TblSalesOrderColorTheme>()); }
            set { _themesList = value; RaisePropertyChanged("ThemesList"); }
        }
        private ObservableCollection<LkpData.TblLkpBrandSectionLink> _brandSectionListLink;

        public ObservableCollection<LkpData.TblLkpBrandSectionLink> BrandSectionListLink
        {
            get { return _brandSectionListLink ?? (_brandSectionListLink = new ObservableCollection<LkpData.TblLkpBrandSectionLink>()); }
            set { _brandSectionListLink = value; RaisePropertyChanged("BrandSectionListLink"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

       

        private ObservableCollection<Brand> _brandFilterList;

        public ObservableCollection<Brand> BrandFilterList
        {
            get { return _brandFilterList ?? (_brandFilterList = new ObservableCollection<Brand>()); }
            set { _brandFilterList = value; RaisePropertyChanged("BrandFilterList"); }
        }


        private ObservableCollection<TblSizeGroup> _sizeGroup;

        public ObservableCollection<TblSizeGroup> SizeGroupList
        {
            get { return _sizeGroup ?? (_sizeGroup = new ObservableCollection<TblSizeGroup>()); }
            set { _sizeGroup = value; RaisePropertyChanged("SizeGroupList"); }
        }

        private ObservableCollection<TblLkpSeason> _seasonList;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private ObservableCollection<TblLkpSeason> _seasonListPerRow;

        public ObservableCollection<TblLkpSeason> SeasonListPerRow
        {
            get { return _seasonListPerRow ?? (_seasonListPerRow = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonListPerRow = value; RaisePropertyChanged("SeasonListPerRow"); }
        }
        private ObservableCollection<GenericTable> _SalesOrderNotesTypeList;

        public ObservableCollection<GenericTable> SalesOrderNotesTypeList
        {
            get { return _SalesOrderNotesTypeList ?? (_SalesOrderNotesTypeList = new ObservableCollection<GenericTable>()); }
            set { _SalesOrderNotesTypeList = value; RaisePropertyChanged("SalesOrderNotesTypeList"); }
        }

        private ObservableCollection<GenericTable> _designList;

        public ObservableCollection<GenericTable> DesignList
        {
            get { return _designList ?? (_designList = new ObservableCollection<GenericTable>()); }
            set { _designList = value; RaisePropertyChanged("DesignList"); }
        }

        private ObservableCollection<TblSize> _sizes;

        public ObservableCollection<TblSize> Sizes
        {
            get { return _sizes; }
            set { _sizes = value; RaisePropertyChanged("Sizes"); }
        }

        private ObservableCollection<TblStyleViewModel> _mainRowList;

        public ObservableCollection<TblStyleViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private int _penStatus;

        public int PenStatus
        {
            get { return _penStatus; }
            set { _penStatus = value; RaisePropertyChanged("PenStatus"); }
        }

        private ObservableCollection<TblStyleViewModel> _mainRowListTemp;

        public ObservableCollection<TblStyleViewModel> MainRowListTemp
        {
            get { return _mainRowListTemp ?? (_mainRowListTemp = new ObservableCollection<TblStyleViewModel>()); }
            set { _mainRowListTemp = value; RaisePropertyChanged("MainRowListTemp"); }
        }

        private ObservableCollection<TblStandardBomHeaderViewModel> _standardBomList;

        public ObservableCollection<TblStandardBomHeaderViewModel> StandardBomList
        {
            get { return _standardBomList ?? (_standardBomList = new ObservableCollection<TblStandardBomHeaderViewModel>()); }
            set { _standardBomList = value; RaisePropertyChanged("StandardBomList"); }
        }

        private ObservableCollection<TblSalesOrderViewModel> _copyBomList;

        public ObservableCollection<TblSalesOrderViewModel> CopybomList
        {
            get { return _copyBomList ?? (_copyBomList = new ObservableCollection<TblSalesOrderViewModel>()); }
            set { _copyBomList = value; RaisePropertyChanged("CopybomList"); }
        }

        //private ObservableCollection<TblTechPackBOMCopyViewModel> _copyTechPackbomList;

        //public ObservableCollection<TblTechPackBOMCopyViewModel> CopyTechPackbomList
        //{
        //    get { return _copyTechPackbomList ?? (_copyTechPackbomList = new ObservableCollection<TblTechPackBOMCopyViewModel>()); }
        //    set { _copyTechPackbomList = value; RaisePropertyChanged("CopyTechPackbomList"); }
        //}

        private ObservableCollection<LkpData.TblStyle> _copyTechPackbomList;

        public ObservableCollection<LkpData.TblStyle> CopyTechPackbomList
        {
            get { return _copyTechPackbomList ?? (_copyTechPackbomList = new ObservableCollection<LkpData.TblStyle>()); }
            set { _copyTechPackbomList = value; RaisePropertyChanged("CopyTechPackbomList"); }
        }

        private ObservableCollection<TblStyleViewModel> _selectedMainRows;

        public ObservableCollection<TblStyleViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStyleViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private BomViewModel _selectedBomRow;

        public BomViewModel SelectedBomRow
        {
            get { return _selectedBomRow ?? (_selectedBomRow = new BomViewModel()); }
            set { _selectedBomRow = value; RaisePropertyChanged("SelectedBomRow"); }
        }

        private TechPackBomViewModel _selectedTechPackBomRow;

        public TechPackBomViewModel SelectedTechPackBomRow
        {
            get { return _selectedTechPackBomRow ?? (_selectedTechPackBomRow = new TechPackBomViewModel()); }
            set { _selectedTechPackBomRow = value; RaisePropertyChanged("SelectedTechPackBomRow"); }
        }

        private ObservableCollection<BomViewModel> _selectedBomRows;

        public ObservableCollection<BomViewModel> SelectedBomRows
        {
            get { return _selectedBomRows ?? (_selectedBomRows = new ObservableCollection<BomViewModel>()); }
            set { _selectedBomRows = value; RaisePropertyChanged("SelectedBomRows"); }
        }

        private ObservableCollection<TechPackBomViewModel> _selectedTechPackBomRows;

        public ObservableCollection<TechPackBomViewModel> SelectedTechPackBomRows
        {
            get { return _selectedTechPackBomRows ?? (_selectedTechPackBomRows = new ObservableCollection<TechPackBomViewModel>()); }
            set { _selectedTechPackBomRows = value; RaisePropertyChanged("SelectedTechPackBomRows"); }
        }

        private TblStyleViewModel _selectedMainRow;

        public TblStyleViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblStyleViewModel()); }
            set
            {
                _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow");
                SelectedMainRow.DetailsList.CollectionChanged += DetailsList_CollectionChanged;
            }
        }

        private TblStyleTNAHeaderViewModel _SelectedTnaRow;

        public TblStyleTNAHeaderViewModel SelectedTnaRow
        {
            get { return _SelectedTnaRow ?? (_SelectedTnaRow = new TblStyleTNAHeaderViewModel()); }
            set
            {
                _SelectedTnaRow = value; RaisePropertyChanged("SelectedTnaRow");
                if (!CustomePermissions.Any(w => w.Code == "UpdateStyleTNA"))
                {
                    if (CustomePermissions.Any(w => w.Code == "LimitedUpdateStyleTNA"))
                    {
                        LimitedUpdateStyleTna = true;
                    }
                }
                if (value != null)
                {
                    GetTNADetaildata();
                }

            }
        }
        private TblStyleTNAHeaderViewModel _CopyOfSelectedTnaRow;

        public TblStyleTNAHeaderViewModel CopyOfSelectedTnaRow
        {
            get { return _CopyOfSelectedTnaRow ?? (_CopyOfSelectedTnaRow = new TblStyleTNAHeaderViewModel()); }
            set
            {
                _CopyOfSelectedTnaRow = value; RaisePropertyChanged("CopyOfSelectedTnaRow");
            }
        }
        private ObservableCollection<ColumnFilterControl.CollectionTemp> _StyleTNAListFilterList;

        public ObservableCollection<ColumnFilterControl.CollectionTemp> StyleTNAListFilterList
        {
            get { return _StyleTNAListFilterList; }
            set { _StyleTNAListFilterList = value; RaisePropertyChanged("StyleTNAListFilterList"); }
        }

        private TblStyleTNAColorDetailModel _TblStyleTNAColorDetailModel;

        public TblStyleTNAColorDetailModel SelectedStyleTNAColorDetailRow
        {
            get { return _TblStyleTNAColorDetailModel ?? (_TblStyleTNAColorDetailModel = new TblStyleTNAColorDetailModel()); }
            set
            {
                _TblStyleTNAColorDetailModel = value; RaisePropertyChanged("SelectedStyleTNAColorDetailRow");
            }
        }

        private ObservableCollection<TblRequestForSampleStatu> _requestForSampleStatusList;

        public ObservableCollection<TblRequestForSampleStatu> RequestForSampleStatusList
        {
            get { return _requestForSampleStatusList ?? (_requestForSampleStatusList = new ObservableCollection<TblRequestForSampleStatu>()); }
            set { _requestForSampleStatusList = value; RaisePropertyChanged("RequestForSampleStatusList"); }
        }

        private ObservableCollection<TblColor> _colorList;

        public ObservableCollection<TblColor> ColorList
        {
            get { return _colorList ?? (_colorList = new ObservableCollection<TblColor>()); }
            set { _colorList = value; RaisePropertyChanged("ColorList"); }
        }

        private TblSalesOrderViewModel _selectedDetailRow;

        public TblSalesOrderViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new TblSalesOrderViewModel()); }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }


        private ObservableCollection<TblSalesOrderViewModel> _selectedDetailRows;

        public ObservableCollection<TblSalesOrderViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSalesOrderViewModel>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private ObservableCollection<TblSalesOrderOperationViewModel> _selectedSalesOrderOperations;

        public ObservableCollection<TblSalesOrderOperationViewModel> SelectedSalesOrderOperations
        {
            get { return _selectedSalesOrderOperations ?? (_selectedSalesOrderOperations = new ObservableCollection<TblSalesOrderOperationViewModel>()); }
            set { _selectedSalesOrderOperations = value; RaisePropertyChanged("SelectedSalesOrderOperations"); }
        }

        private TblSalesOrderOperationViewModel _selectedSalesOrderOperation;

        public TblSalesOrderOperationViewModel SelectedSalesOrderOperation
        {
            get { return _selectedSalesOrderOperation ?? (_selectedSalesOrderOperation = new TblSalesOrderOperationViewModel()); }
            set { _selectedSalesOrderOperation = value; RaisePropertyChanged("SelectedSalesOrderOperation"); }
        }

        private TblSalesOrderColorViewModel _selectSalesOrderColorRow;

        public TblSalesOrderColorViewModel SelectedSalesOrderColorRow
        {
            get { return _selectSalesOrderColorRow ?? (_selectSalesOrderColorRow = new TblSalesOrderColorViewModel()); }
            set { _selectSalesOrderColorRow = value; RaisePropertyChanged("SelectedSalesOrderColorRow"); }
        }

        private TblSalesOrderNotesModel _selectSalesOrderNotesRow;

        public TblSalesOrderNotesModel SelectedSalesOrderNotesRow
        {
            get { return _selectSalesOrderNotesRow ?? (_selectSalesOrderNotesRow = new TblSalesOrderNotesModel()); }
            set { _selectSalesOrderNotesRow = value; RaisePropertyChanged("SelectedSalesOrderNotesRow"); }
        }


        private TblSalesOrderColorViewModel _selectedDetailRowColorRow;

        public TblSalesOrderColorViewModel SelectedDetailRowColorRow
        {
            get { return _selectedDetailRowColorRow ?? (_selectedDetailRowColorRow = new TblSalesOrderColorViewModel()); }
            set { _selectedDetailRowColorRow = value; RaisePropertyChanged("SelectedDetailRowColorRow"); }
        }

        private ObservableCollection<TblSalesOrder> _poNotLinkdList;

        public ObservableCollection<TblSalesOrder> PoNotLinkdList
        {
            get { return _poNotLinkdList; }
            set { _poNotLinkdList = value; RaisePropertyChanged("PoNotLinkdList"); }
        }

        private TblSalesOrder _selectedPoToLink;

        public TblSalesOrder SelectedPoToLink
        {
            get { return _selectedPoToLink ?? (_selectedPoToLink = new TblSalesOrder()); }
            set { _selectedPoToLink = value; RaisePropertyChanged("SelectedPoToLink"); }
        }

        private ProductionService.StyleTnaCount_Result _StyleTnaCountList;

        public ProductionService.StyleTnaCount_Result StyleTnaPerRow
        {
            get { return _StyleTnaCountList; }
            set { _StyleTnaCountList = value; RaisePropertyChanged("StyleTnaPerRow"); }
        }

        private ObservableCollection<TblSalesOrderColorViewModel> _selectedSalesOrderColorRows;

        public ObservableCollection<TblSalesOrderColorViewModel> SelectedSalesOrderColorRows
        {
            get { return _selectedSalesOrderColorRows ?? (_selectedSalesOrderColorRows = new ObservableCollection<TblSalesOrderColorViewModel>()); }
            set { _selectedSalesOrderColorRows = value; RaisePropertyChanged("SelectedSalesOrderColorRows"); }
        }


        private ObservableCollection<TblSalesOrderNotesModel> _selectedSalesOrderNotesRows;

        public ObservableCollection<TblSalesOrderNotesModel> SelectedSalesOrderNotesRows
        {
            get { return _selectedSalesOrderNotesRows ?? (_selectedSalesOrderNotesRows = new ObservableCollection<TblSalesOrderNotesModel>()); }
            set { _selectedSalesOrderNotesRows = value; RaisePropertyChanged("SelectedSalesOrderNotesRows"); }
        }


        private TblChainSetting _retailTblChainSetting;

        public TblChainSetting RetailTblChainSetting
        {
            get { return _retailTblChainSetting; }
            set { _retailTblChainSetting = value; RaisePropertyChanged("RetailTblChainSetting"); }
        }

        private ObservableCollection<TblPaymentSchedule> _paymentScheduleList;

        public ObservableCollection<TblPaymentSchedule> PaymentScheduleList
        {
            get { return _paymentScheduleList; }
            set { _paymentScheduleList = value; RaisePropertyChanged("PaymentScheduleList"); }
        }

        private ObservableCollection<TblCurrency> _currencyList;
        private ObservableCollection<GenericTable> _styleStatusList;

        public ObservableCollection<GenericTable> StyleStatusList
        {
            get { return _styleStatusList ?? (_styleStatusList = new ObservableCollection<GenericTable>()); }
            set { _styleStatusList = value; RaisePropertyChanged("StyleStatusList"); }
        }

        private ObservableCollection<GenericTable> _styleCategoryList;
        internal string StyletnaOption;

        public ObservableCollection<GenericTable> StyleCategoryList
        {
            get { return _styleCategoryList ?? (_styleCategoryList = new ObservableCollection<GenericTable>()); }
            set { _styleCategoryList = value; RaisePropertyChanged("StyleCategoryList"); }
        }

        private ObservableCollection<GenericTable> _styleSpecGenericFabricList;

        public ObservableCollection<GenericTable> StyleSpecGenericFabricList
        {
            get { return _styleSpecGenericFabricList ?? (_styleSpecGenericFabricList = new ObservableCollection<GenericTable>()); }
            set { _styleSpecGenericFabricList = value; RaisePropertyChanged("StyleSpecGenericFabricList"); }
        }

        private ObservableCollection<GenericTable> _styleTechPackStatusList;
        public ObservableCollection<GenericTable> StyleTechPackStatusList
        {
            get { return _styleTechPackStatusList ?? (_styleTechPackStatusList = new ObservableCollection<GenericTable>()); }
            set { _styleTechPackStatusList = value; RaisePropertyChanged("StyleTechPackStatusList"); }
        }

        private LkpData.TBLTechPackStatu _selectedTechPackStatusperRow;
        public LkpData.TBLTechPackStatu SelectedTechPackStatusperRow
        {
            get { return _selectedTechPackStatusperRow ?? (_selectedTechPackStatusperRow = new LkpData.TBLTechPackStatu()); }
            set { _selectedTechPackStatusperRow = value; RaisePropertyChanged("SelectedTechPackStatusperRow"); }
        }

        public bool SeasonalMasterListCompleted { get; set; }

        #endregion Props

        internal void CopyBom()
        {
            Client.GetSalesOrderForCopyBomAsync(CopybomList.Count, PageSize, SelectedDetailRow.SalesOrderType, "it.Iserial", DetailSubFilter, DetailSubValuesObjects);
        }

        internal void SalesOperationDetail(TblSalesOrderOperationDetail row)
        {
            if (row.Iserial != 0)
            {
                if (AllowDelete != true)
                {
                    MessageBox.Show(strings.AllowDeleteMsg);
                    return;
                }
                Loading = true;
                Client.DeleteTblSalesOrderOperationDetailAsync(row);
            }
            else
            {
                SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.Remove(row);
                if (!SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.Any())
                {
                    SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.Add(new TblSalesOrderOperationDetail());
                }
            }
        }

        internal void SendMail(ObservableCollection<string> para, string subject, string body)
        {
            Client.ViewReportAsync("AdvancedRequestForSample", para);
            Subject = subject;
            Body = body;
        }

        internal void ValidateBom()
        {
            if (!_selectedDetailRow.SalesOrderOperationList.Any())
            {
                if (_selectedDetailRow.SalesOrderOperationList.Any(x => x.OprCost == 0))
                {
                    MessageBox.Show("Operations Can't Be 0 Cost ");
                    return;
                }
                MessageBox.Show("No Operations");
                return;
            }

            Client.ValidateBomAsync(SelectedDetailRow.Iserial);
        }

        internal void InsertImportedStyles(ObservableCollection<TblStyle> stylelist)
        {
            Client.InsertImportedStylesAsync(stylelist, null, LoggedUserInfo.WFM_UserName);
        }

        internal void InsertImportedSalesOrder(ObservableCollection<TblSalesOrder> salesOrderlist)
        {
            Client.InsertImportedSalesOrderAsync(salesOrderlist, LoggedUserInfo.WFM_UserName);
        }

        internal void RemoveFromPlan(BomViewModel selectedBomRow)
        {
            Client.RemoveFromPurchasePlanAsync(selectedBomRow.Iserial);
        }

        internal void StyleTnaCount()
        {
            ProductionClient.StyleTnaCountAsync(LoggedUserInfo.Iserial);
        }

        internal void RequestTna(bool requireTna)
        {
            if (!Loading && SelectedMainRow.Iserial != 0)
            {
                ProductionClient.RequestStyleTnaAsync(SelectedMainRow.Iserial, requireTna);
            }

        }

        internal void CheckTnaChanged()
        {
            try
            {
                if (SelectedTnaRow != null && SelectedTnaRow != new TblStyleTNAHeaderViewModel())
                {
                    var temp = GlobalMethods.GetDifferingProperties(CopyOfSelectedTnaRow, SelectedTnaRow);
                    var test = temp;
                }

            }
            catch (Exception ex)
            {
                var tempeerere = ex;
            }
        }

        internal void GetTblStyleCategoryLink(string TblLkpDirection)
        {
            StyleCategoryList.Clear();
            if (SelectedMainRow.Brand != null && SelectedMainRow.TblLkpBrandSection != null && !string.IsNullOrEmpty(TblLkpDirection))
            {

                var StyleCategoryLinkClientClient = new LkpData.LkpDataClient();
                StyleCategoryLinkClientClient.FamilyCategory_GetTblCategoryLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, int.Parse(TblLkpDirection));
                StyleCategoryLinkClientClient.FamilyCategory_GetTblCategoryLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        //if (StyleCategoryList.All(x => x.Iserial != row.TblStyleCategory1.Iserial))
                        //{
                        StyleCategoryList.Add(new GenericTable().InjectFrom(row.TblStyleCategory1) as GenericTable);
                        //}
                    }

                    //if (StyleCategoryList.All(x => x.Iserial != row.Iserial))
                    //{
                    //    StyleCategoryList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                    //}
                };
            }
        }

        internal void GetTblStyleThemeLink()
        {

            if (SelectedMainRow.Brand != null && SelectedMainRow.TblLkpSeason != null && SelectedMainRow.TblLkpBrandSection != null)
            {
                SelectedMainRow.StyleThemeList.Clear();
                var stylecategorylinkclientclient = new LkpData.LkpDataClient();
                stylecategorylinkclientclient.FamilyCategory_GetTblThemeLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, (int)SelectedMainRow.TblLkpSeason);
                stylecategorylinkclientclient.FamilyCategory_GetTblThemeLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        //if (stylecategorylist.all(x => x.iserial != row.tblstylecategory1.iserial))
                        //{
                        SelectedMainRow.StyleThemeList.Add(new GenericTable().InjectFrom(row) as GenericTable);
                        //}
                    }

                    //if (stylecategorylist.all(x => x.iserial != row.iserial))
                    //{
                    //    stylecategorylist.add(new generictable().injectfrom(row) as generictable);
                    //}
                };
            }
        }

        internal void GetTblFamilyLinkByCategory(string direction, string category)
        {
            var StyleCategoryLinkClientClient = new LkpData.LkpDataClient();
            SelectedMainRow.FamilyList.Clear();

            lkpClient.FamilyCategory_GetTblFamilyCategoryLinkCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (SelectedMainRow.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial))
                    {
                        SelectedMainRow.FamilyList.Add(row.TblFamily1);
                    }
                }
            };

            lkpClient.FamilyCategory_GetTblFamilyCategoryLinkAsync(SelectedMainRow.Brand, (int)SelectedMainRow.TblLkpBrandSection, int.Parse(direction), int.Parse(category));
        }

        internal void GetTblSubFamilyLinkByCategory(string direction, string category, string Family)
        {
            var StyleCategoryLinkClientClient = new LkpData.LkpDataClient();
            SelectedMainRow.SubFamilyList.Clear();

            lkpClient.FamilyCategory_GetTblSubFamilyCategoryLinkCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (SelectedMainRow.SubFamilyList.All(x => x.Iserial != row.TblSubFamily1.Iserial))
                    {
                        SelectedMainRow.SubFamilyList.Add(row.TblSubFamily1);
                    }
                }
            };

            lkpClient.FamilyCategory_GetTblSubFamilyCategoryLinkAsync(SelectedMainRow.Brand,
                (int)SelectedMainRow.TblLkpBrandSection, int.Parse(direction),
                int.Parse(category), int.Parse(Family));
        }

        internal void GetRepeatedStyles(string brand, string section, string family)
        {
            //lkpClient.FamilyCategory_GetRepeatedStylesCompleted += (s, sv) =>
            //{
            //    SelectedMainRow.RepeatedStyleList.Clear();
            //    foreach (var row in sv.Result)
            //    {
            //          SelectedMainRow.RepeatedStyleList.Add(row);
            //    }
            //};

            //lkpClient.FamilyCategory_GetRepeatedStylesAsync(brand, section, family);
        }

        internal void GetTblSizeGroupLinkByBrandSection(string brand, string section)
        {
            CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
            _client.FamilyCategory_GetTblSizeGroupLinkCompleted += (s, sv) =>
            {
                SizeGroupList.Clear();
                foreach (var row in sv.Result)
                {
                    SizeGroupList.Add(row.TblSizeGroup1);
                }
            };
            _client.FamilyCategory_GetTblSizeGroupLinkAsync(brand, section);
        }

        internal void GetStyleTNARoute()
        {

        }

        internal void GetTblSupplier()
        {
            lkpClient.GetTBLsupplierAsync(LoggedUserInfo.DatabasEname);
            lkpClient.GetTBLsupplierCompleted += (s, sv) =>
            {
                if (sv.Result.Count() > 0)
                {
                    Suppliers = sv.Result.ToList();
                }
            };
        }

        internal void GetStyleTechPackParts()
        {
            lkpClient.GetTBLTechPackPartsAsync();
            lkpClient.GetTBLTechPackPartsCompleted += (s, sv) =>
            {

                StyleTechPackPartList.Clear();
                foreach (var row in sv.Result)
                {
                    if (StyleTechPackPartList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleTechPackPartList.Add(row);
                    }
                }
            };
        }

        internal void GetStyleSpecTypes()
        {
            lkpClient.GetTblStyleSpecTypesAsync();
            lkpClient.GetTblStyleSpecTypesCompleted += (s, sv) =>
            {
                StyleSpecTypesList.Clear();
                foreach (var row in sv.Result)
                {
                    if (StyleSpecTypesList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleSpecTypesList.Add(row);
                    }
                }
                GetStyleSpecDetail();
            };
        }

        

        internal void GetStyleTNARouteStatus()
        {
            lkpClient.GetTblStyleTNARouteStatusAsync();
            lkpClient.GetTblStyleTNARouteStatusCompleted += (s, sv) =>
            {
                StyleTNAStatusList.Clear();
                foreach (var row in sv.Result)
                {
                    if (StyleTNAStatusList.All(x => x.Iserial != row.Iserial))
                    {
                        StyleTNAStatusList.Add(row);
                    }
                }
            };
        }

        internal void GetTechPackDetailDesignData()
        {
            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
            _client.GetTBLTechPackDetailsAsync(SelectedMainRow.Iserial);

            _client.GetTBLTechPackDetailsCompleted += (s, sv) =>
            {
                SelectedTnaRow.TechPackDesignDetailList.Clear();
                SelectedMainRow.tblTechPackHeader = sv.tblTechPackHeader;
                foreach (var row in sv.Result.OrderBy(w => w.Iserial).ToList())
                {
                    var newrow = new TblTechPackDesignDetailViewModel();
                    newrow.InjectFrom(row);
                    try
                    {

                        LkpData.tblTechPackPart lk = StyleTechPackPartListTemp.Where(x => x.Iserial == newrow.TblTechPackPart).FirstOrDefault();
                        StyleTechPackPartListTemp.Remove(lk);
                        newrow.InjectFrom(row);
                        newrow.TechPackPartPerRow = new LkpData.tblTechPackPart().InjectFrom(StyleTechPackPartList.FirstOrDefault(w => w.Iserial == row.tblTechPackPart)) as LkpData.tblTechPackPart;
                        SelectedTnaRow.TechPackDesignDetailList.Add(newrow);
                    }
                    catch { }

                }
                Loading = false;
                if (SelectedTnaRow.TechPackDesignDetailList.Count == 0)
                {
                    AddNewTechPackDetailRow();
                }
            };
        }
        
        public void AddNewTechPackDetailRow(TblTechPackDesignDetailViewModel row = null)
        {

            StyleTechPackPartListTemp = StyleTechPackPartList.ToList();

            //(from s in StyleTechPackPartList
            //                             where !SelectedTnaRow.TechPackDesignDetailList.Any(es => (es.TechPackPartPerRow.Iserial == s.Iserial )
            //                             )
            //                             select s).ToList();
            
            //if (SelectedTnaRow != null)
            {
                    {
                    var index = SelectedTnaRow.TechPackDesignDetailList.IndexOf(row);
                    {
                        //Remaining TechPack Parts
                        //StyleTechPackPartList.
                        var newrow = new TblTechPackDesignDetailViewModel
                        {
                            Description = "",
                        };
                        SelectedTnaRow.TechPackDesignDetailList.Add(newrow);
                    }
                }
            }
        }

        public void AddNewStyleSpecDetailRow(TblStyleSpecDetailsViewModel row = null)
        {
            var index = SelectedMainRow.StyleSpecDetailsList.IndexOf(row);
            var newrow = new TblStyleSpecDetailsViewModel
            {
                tblStyle = SelectedMainRow.Iserial,
                Description = "",
            };
            SelectedMainRow.StyleSpecDetailsList.Add(newrow);
        }

        internal void SaveTechPackBomRow()
        {
            var row = SelectedTechPackBomRow;

            var bomListToSave = new ObservableCollection<TechPackBOM>();

            var newrow = new TechPackBOM();

            newrow.InjectFrom(row);
            newrow.TblTechPackBOMSizes = row.BomSizes;
            newrow.TblStyle = SelectedMainRow.Iserial;

            newrow.TblTechPackBOMStyleColors = new ObservableCollection<TblTechPackBOMStyleColor>();
            if (row.BomStyleColors != null)
            {
                GenericMapper.InjectFromObCollection(newrow.TblTechPackBOMStyleColors, row.BomStyleColors);
                newrow.TblStyle = SelectedMainRow.Iserial;

                if (row.IsAcc)
                {
                    foreach (var colorRow in newrow.TblTechPackBOMStyleColors)
                    {
                        colorRow.FabricColor = colorRow.DummyColor;
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                else
                {
                    foreach (var colorRow in newrow.TblTechPackBOMStyleColors)
                    {
                        colorRow.TblColor = null;
                        colorRow.TblColor1 = null;
                    }
                }
                if (!row.IsSupplierMaterial)
                {
                    if (row.ItemPerRow != null)
                    {
                    }
                }
                bomListToSave.Add(newrow);

                var service = new CRUD_ManagerServiceClient();
                //if (Loading)
                //{
                //    return;
                //}
                //Loading = true;

                service.UpdateOrInsertTechPackBomAsync(bomListToSave);

                var index = SelectedDetailRow.TechPackBomList.IndexOf(SelectedTechPackBomRow);

                service.UpdateOrInsertTechPackBomCompleted += (s, sv) =>
                {
                    try
                    {
                        if (sv.Error != null)
                        {
                            MessageBox.Show(sv.Error.Message);
                        }
                        Loading = false;
                        var rowww = SelectedDetailRow.TechPackBomList.ElementAt(index);
                        rowww.InjectFrom(sv.Result.FirstOrDefault());
                        rowww.BomStyleColors.Clear();
                        rowww.BomSizes.Clear();
                        var firstOrDefault = sv.Result.FirstOrDefault();
                        var colors = SelectedMainRow.SelectedStyleSeasonalMaterColors.Select(x => x.Iserial);
                        if (firstOrDefault != null)
                        {
                            GenericMapper.InjectFromObCollection(rowww.BomStyleColors, firstOrDefault.TblTechPackBOMStyleColors.Where(w => colors.Contains(w.StyleColor)));
                            GenericMapper.InjectFromObCollection(rowww.BomSizes, firstOrDefault.TblTechPackBOMSizes);
                        }

                        Loading = false;

                    } catch { }
                    
                   
                };
            }
        }

        internal void SaveTechPackDesignDetailRow(int? TechPackStatus, string Fit = "", string ORGFabric = "")
        {
            #region oldInsert
            //New Header
            //if (SelectedMainRow.tblTechPackHeader != null)
            //{
            //    lkpClient.InsertOrUpdateTBLTechPackHeaderAsync(SelectedMainRow.tblTechPackHeader);
            //}
            //else
            //{
            //    LkpData.tblTechPackHeader _newRow = new LkpData.tblTechPackHeader();
            //    _newRow.CreationDate = DateTime.Now;
            //    _newRow.tblUser = LoggedUserInfo.Iserial;
            //    _newRow.tblStyle = SelectedMainRow.Iserial;
            //    lkpClient.InsertOrUpdateTBLTechPackHeaderAsync(_newRow);
            //}

            //lkpClient.InsertOrUpdateTBLTechPackHeaderCompleted += (s, sv) =>
            //{
            //    try
            //    {
            //        SelectedMainRow.tblTechPackHeader = sv.Result;
            //        var newTechPackDesignDetailRows = new ObservableCollection<LkpData.tblTechPackDetail>();
            //        foreach (var item in SelectedTnaRow.TechPackDesignDetailList)
            //        {
            //            try
            //            {
            //                var newrow = new LkpData.tblTechPackDetail();
            //                newrow.Iserial = item.Iserial;
            //                newrow.tblTechPackPart = item.TechPackPartPerRow.Iserial;
            //                newrow.Description = item.Description;
            //                newrow.galaryLink = item.galarylink;
            //                newrow.ImageName = item.ImageName;
            //                newrow.ImageThumb = item.ImageThumb;
            //                newrow.tblUser = LoggedUserInfo.Iserial;
            //                if (SelectedMainRow.tblTechPackHeader != null) newrow.tblTechPackHeader = SelectedMainRow.tblTechPackHeader.Iserial;
            //                if (!string.IsNullOrEmpty(newrow.ImageName))
            //                newTechPackDesignDetailRows.Add(newrow);
            //            } catch { }
            //        }
            //        if(newTechPackDesignDetailRows.Count > 0)
            //        lkpClient.InsertOrUpdateTBLTechPackDetailAsync(newTechPackDesignDetailRows);
            //        lkpClient.InsertOrUpdateTBLTechPackDetailCompleted += (s1, sv1) =>
            //        {
            //            GetTechPackDetailDesignData();
            //        };
            //    }
            //    catch { }
            // };
            #endregion

            LkpData.tblTechPackHeader _newTechPackHeaderRow = new LkpData.tblTechPackHeader();
            //Get_New_Header
            if (SelectedMainRow.tblTechPackHeader == null)
            {
                _newTechPackHeaderRow.CreationDate = DateTime.Now;
                _newTechPackHeaderRow.tblUser = LoggedUserInfo.Iserial;
                _newTechPackHeaderRow.tblStyle = SelectedMainRow.Iserial;
                _newTechPackHeaderRow.Description = SelectedMainRow.AdditionalDescription;
                _newTechPackHeaderRow.FIT = Fit;
                _newTechPackHeaderRow.ORGFabric = ORGFabric;
                _newTechPackHeaderRow.TBLTechPackStatus = TechPackStatus;
            }
            else
            {
                _newTechPackHeaderRow.tblUser = SelectedMainRow.tblTechPackHeader.tblUser;
                _newTechPackHeaderRow.CreationDate = SelectedMainRow.tblTechPackHeader.CreationDate;
                _newTechPackHeaderRow.tblUser = LoggedUserInfo.Iserial;
                _newTechPackHeaderRow.Iserial = SelectedMainRow.tblTechPackHeader.Iserial;
                _newTechPackHeaderRow.tblStyle = SelectedMainRow.Iserial;
                _newTechPackHeaderRow.Description = SelectedMainRow.AdditionalDescription;
                _newTechPackHeaderRow.FIT = SelectedMainRow.tblTechPackHeader.FIT;
                _newTechPackHeaderRow.ORGFabric = SelectedMainRow.tblTechPackHeader.ORGFabric;
                _newTechPackHeaderRow.TBLTechPackStatus = SelectedMainRow.tblTechPackHeader.TBLTechPackStatus;
            }
            //Get_Details
            try
            {
                var newTechPackDesignDetailRows = new ObservableCollection<LkpData.tblTechPackDetail>();
                foreach (var item in SelectedTnaRow.TechPackDesignDetailList)
                {
                    try
                    {
                        var newrow = new LkpData.tblTechPackDetail();
                        newrow.Iserial = item.Iserial;
                        newrow.tblTechPackPart = item.TechPackPartPerRow.Iserial;
                        newrow.Description = item.Description;
                        newrow.galaryLink = item.galarylink;
                        newrow.ImageName = item.ImageName;
                        newrow.ImageThumb = item.ImageThumb;
                        newrow.tblUser = LoggedUserInfo.Iserial;
                        if (SelectedMainRow.tblTechPackHeader != null) newrow.tblTechPackHeader = SelectedMainRow.tblTechPackHeader.Iserial;
                        // if (!string.IsNullOrEmpty(newrow.ImageName))
                        newTechPackDesignDetailRows.Add(newrow);
                    }
                    catch { }
                }
                if (newTechPackDesignDetailRows.Count > 0)
                    lkpClient.InsertOrUpdateTBLTechPackDetailAsync(_newTechPackHeaderRow, newTechPackDesignDetailRows);
                lkpClient.InsertOrUpdateTBLTechPackDetailCompleted += (s1, sv1) =>
                {
                    GetTechPackDetailDesignData();
                };
            }
            catch { }

        }

        internal void SaveTechPackDesignDetailAllRows()
        {
            //foreach (var item in SelectedTnaRow.TechPackDesignDetailList)
            //{
            //    var newrow = new LkpData.tblTechPackDetail();
            //    newrow.Iserial = item.Iserial;
            //    newrow.tblTechPackPart = item.TechPackPartPerRow.Iserial;
            //    newrow.Description = item.Description;
            //    newrow.galaryLink = item.galarylink;
            //    newrow.ImageName = item.ImageName;
            //    newrow.tblUser = LoggedUserInfo.Iserial;
            //    if(SelectedMainRow.tblTechPackHeader != null)
            //     newrow.tblTechPackHeader =  SelectedMainRow.tblTechPackHeader.Iserial;
            //    lkpClient.InsertOrUpdateTBLTechPackDetailAsync(newrow,item.Imagestream);
            //}

            //lkpClient.InsertOrUpdateTBLTechPackDetailCompleted += (s, sv) =>
            //{
            //    GetTechPackDetailDesignData();
            //};
        }

        internal void SaveTechPackDesignHeaderRow(int? TechPackStatus)
        {
            if (SelectedMainRow.tblTechPackHeader != null)
            {
                SelectedMainRow.tblTechPackHeader.TBLTechPackStatus = TechPackStatus;
                lkpClient.InsertOrUpdateTBLTechPackHeaderAsync(SelectedMainRow.tblTechPackHeader);
            }
            else
            {
                LkpData.tblTechPackHeader _newRow = new LkpData.tblTechPackHeader();
                _newRow.CreationDate = DateTime.Now;
                _newRow.TBLTechPackStatus = TechPackStatus;
                _newRow.tblUser = LoggedUserInfo.Iserial;
                _newRow.tblStyle = SelectedMainRow.Iserial;
                lkpClient.InsertOrUpdateTBLTechPackHeaderAsync(_newRow);
            }

            lkpClient.InsertOrUpdateTBLTechPackHeaderCompleted += (s, sv) =>
            {
                SelectedMainRow.tblTechPackHeader = sv.Result;
                SaveTechPackDesignDetailAllRows();
            };
        }

        internal void GetTechPackBomComments()
        {
            if (SelectedMainRow != null) lkpClient.GetTBLTechPackBOMCommentAsync(SelectedMainRow.Iserial);
            lkpClient.GetTBLTechPackBOMCommentCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    SelectedMainRow.tblTechPackBOMComment = sv.Result;
                }
                else
                {
                    SelectedMainRow.tblTechPackBOMComment = new LkpData.tblTechPackBOMComment();
                    SelectedMainRow.tblTechPackBOMComment.tblStyle = SelectedMainRow.Iserial;
                }
            };
        }

        internal void UpdateOrInsertTechPackBOMComment()
        {
            lkpClient.UpdateOrInsertTechPackBOMCommentAsync(SelectedMainRow.tblTechPackBOMComment);
            lkpClient.UpdateOrInsertTechPackBOMCommentCompleted += (s, sv) =>
            {
                SelectedMainRow.tblTechPackBOMComment = sv.Result;

            };
        }

        internal void DeleteTechPackDesingDetailRows(List<TblTechPackDesignDetailViewModel> _selectedTechPackDesignDetails)
        {
            try
            {
                foreach (var item in _selectedTechPackDesignDetails)
                {
                    if (item.Iserial != 0)
                    {
                        lkpClient.DeleteTechPackDesignDetailsAsync(item.Iserial);
                    }
                }
            }
            catch { }
        }

        internal void GetTechBackBomCopyList()
        {
            lkpClient.GetCurrentTechPackBOMStylesAsync(CopyTechPackbomList.Count, PageSize, "it.Iserial", DetailSubFilter, DetailSubValuesObjects);
            lkpClient.GetCurrentTechPackBOMStylesCompleted += (s, sv) =>
            {
                CopyTechPackbomList.Clear();
                if (sv.Result != null && sv.Result.Count > 0)
                {
                    foreach (var item in sv.Result)
                    {
                        CopyTechPackbomList.Add(item);
                    }
                }
            };
        }

        internal void GetFilterBrandSections()
        {
            BrandSectionFilterList.Clear();
            DirectionFilterList.Clear();
            StyleCategoryFilterList.Clear();
            FamilyFilterList.Clear();
            SubFamilyFilterList.Clear();

            CRUDManagerService.CRUD_ManagerServiceClient _Client = new CRUDManagerService.CRUD_ManagerServiceClient();
            _Client.GetTblBrandSectionLinkAsync(BrandFilter, LoggedUserInfo.Iserial);
            _Client.GetTblBrandSectionLinkCompleted += (s, e) =>
            {

                foreach (var row in e.Result)
                {
                    if (!BrandSectionFilterList.Any(w => w.Iserial == row.TblLkpBrandSection))
                    {
                        TblLkpBrandSection temp = new TblLkpBrandSection();
                        temp.InjectFrom(row.TblLkpBrandSection1);
                        BrandSectionFilterList.Add(temp);
                    }
                }
                MainRowList.Clear();
                GetMaindata();
            };
        }

        internal void GetDirectionFilter()
        {
            try
            {
                SubFamilyFilterList.Clear();
                FamilyFilterList.Clear();
                StyleCategoryFilterList.Clear();
                DirectionFilterList.Clear();

                CRUDManagerService.CRUD_ManagerServiceClient _Client = new CRUDManagerService.CRUD_ManagerServiceClient();
                _Client.GetTblDirectionFilterLinkAsync(BrandFilter, SelectedBrandSectionFilterList);
                _Client.GetTblDirectionFilterLinkCompleted += (s, e) =>
                {
                    foreach (var row in e.Result)
                    {
                        if (!DirectionFilterList.Any(w => w.Iserial == row.TblLkpDirection))
                        {
                            TblLkpDirection temp = new TblLkpDirection();
                            temp.InjectFrom(row.TblLkpDirection1);
                            DirectionFilterList.Add(temp);
                        }
                    }
                    MainRowList.Clear();
                    GetMaindata();
                };
            }
            catch { }
        }

        internal void GetStyleCategoryFilter()
        {
            try
            {
                SubFamilyFilterList.Clear();
                FamilyFilterList.Clear();
                StyleCategoryFilterList.Clear();

                CRUDManagerService.CRUD_ManagerServiceClient _Client = new CRUDManagerService.CRUD_ManagerServiceClient();
                _Client.GetTblStyleCategoryFilterLinkAsync(BrandFilter, SelectedBrandSectionFilterList, SelectedDirectionFilterList);
                _Client.GetTblStyleCategoryFilterLinkCompleted += (s, e) =>
                {

                    foreach (var row in e.Result)
                    {
                        if (!StyleCategoryFilterList.Any(w => w.Iserial == row.TblStyleCategory))
                        {
                            TblStyleCategory temp = new TblStyleCategory();
                            temp.InjectFrom(row.TblStyleCategory1);
                            StyleCategoryFilterList.Add(temp);
                        }
                    }
                    MainRowList.Clear();
                    GetMaindata();
                };
            }
            catch { }
        }

        internal void GetFamilyFilter()
        {
            try
            {
                SubFamilyFilterList.Clear();
                FamilyFilterList.Clear();

                CRUDManagerService.CRUD_ManagerServiceClient _Client = new CRUDManagerService.CRUD_ManagerServiceClient();
                _Client.GetTblFamilyCategoryFilterLinkAsync(BrandFilter, SelectedBrandSectionFilterList, SelectedDirectionFilterList, SelectedStyleCategoryFilterList);
                _Client.GetTblFamilyCategoryFilterLinkCompleted += (s, e) =>
                {

                    foreach (var row in e.Result)
                    {
                        if (!FamilyFilterList.Any(w => w.Iserial == row.TblFamily))
                        {
                            TblFamily temp = new TblFamily();
                            temp.InjectFrom(row.TblFamily1);
                            FamilyFilterList.Add(temp);
                        }
                    }
                    MainRowList.Clear();
                    GetMaindata();
                };
            }
            catch { }
        }
        internal void GetSubFamilyFilter()
        {
            try
            {
                SubFamilyFilterList.Clear();
                CRUDManagerService.CRUD_ManagerServiceClient _Client = new CRUDManagerService.CRUD_ManagerServiceClient();
                _Client.GetTblSubFamilyCategoryFilterLinkAsync(BrandFilter, SelectedBrandSectionFilterList, SelectedDirectionFilterList, SelectedStyleCategoryFilterList, SelectedFamilyFilterList);
                _Client.GetTblSubFamilyCategoryFilterLinkCompleted += (s, e) =>
                {

                    foreach (var row in e.Result)
                    {
                        if (!SubFamilyFilterList.Any(w => w.Iserial == row.TblSubFamily))
                        {
                            TblSubFamily temp = new TblSubFamily();
                            temp.InjectFrom(row.TblSubFamily1);
                            SubFamilyFilterList.Add(temp);
                        }
                    }
                    MainRowList.Clear();
                    GetMaindata();
                };

            }
            catch { }

        }

        internal void ClearStyleGridFilters()
        {
            BrandFilter = "";

            BrandSectionFilterList.Clear();
            DirectionFilterList.Clear();
            StyleCategoryFilterList.Clear();
            FamilyFilterList.Clear();
            SubFamilyFilterList.Clear();

            SelectedBrandSectionFilterList.Clear();
            SelectedDirectionFilterList.Clear();
            SelectedStyleCategoryFilterList.Clear();
            SelectedFamilyFilterList.Clear();
            SelectedSubFamilyFilterList.Clear();
            MainRowList.Clear();
            GetMaindata();
        }

        internal void CopyTechPackBomRow(bool checkCurrentRow)
        {
            var currentRowIndex = (SelectedDetailRow.TechPackBomList.IndexOf(SelectedTechPackBomRow));
            if (checkCurrentRow)
            {
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedTechPackBomRow, new ValidationContext(SelectedTechPackBomRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
                
               //Inject Selected Row
               //newrow = new TechPackBomViewModel().InjectFrom(SelectedTechPackBomRow) as TechPackBomViewModel;
                
                var styleColorList = new ObservableCollection<TechPackBomStyleColorViewModel>();
                if (SelectedMainRow.SelectedStyleSeasonalMaterColors != null)
                foreach (var item in SelectedMainRow.SelectedStyleSeasonalMaterColors)
                {
                    if (item != null)
                        styleColorList.Add(new TechPackBomStyleColorViewModel
                        {
                            StyleColor = item.Iserial,
                            TblColor1 = item
                        });
                }

                var mainFabric = SelectedDetailRow.TechPackBomList.Count == 0;
            var newrow = new TechPackBomViewModel
            {
                BomStyleColors = styleColorList,
                TblSalesOrder = 0,
                BOM_IsMainFabric = mainFabric,
                IsAcc = false,
                galarylink = "",
                ImageName = "",
                BOM_FabricType = SelectedTechPackBomRow.BOM_FabricType,
                BOM_Notes = SelectedTechPackBomRow.BOM_Notes,
                BOM_Fabric = SelectedTechPackBomRow.BOM_Fabric,
                BOM_FabricTypePerRow = SelectedTechPackBomRow.BOM_FabricTypePerRow,
                SupplierCode = SelectedTechPackBomRow.SupplierCode,
                SupplierColorCode = SelectedTechPackBomRow.SupplierColorCode,
                Placement = SelectedTechPackBomRow.Placement,
                Fabric_Description = SelectedTechPackBomRow.Fabric_Description,
                Composition = SelectedTechPackBomRow.Composition,
                Consumption = SelectedTechPackBomRow.Consumption,
                Width = SelectedTechPackBomRow.Width,
                Weight = SelectedTechPackBomRow.Weight,
                Unit = SelectedTechPackBomRow.Unit,
                FabricType = SelectedTechPackBomRow.FabricType,
                //Vendor = SelectedTechPackBomRow.Vendor,
                //VendorPerRow = SelectedTechPackBomRow.VendorPerRow,
                CalcMethod= SelectedTechPackBomRow.CalcMethod,
                Dyed = SelectedTechPackBomRow.Dyed,
                BOM_IsLocalProduction = SelectedTechPackBomRow.BOM_IsLocalProduction,
                //Items = SelectedTechPackBomRow.Items,
                //ItemPerRow = SelectedTechPackBomRow.ItemPerRow,
                OnStock = SelectedTechPackBomRow.OnStock
            };

                var bomSizesTemp = new ObservableCollection<TblTechPackBOMSize>();
                if (Sizes != null)
                foreach (var item in Sizes)
                {
                    bomSizesTemp.Add(new TblTechPackBOMSize
                    {
                        StyleSize = item.SizeCode,
                        MaterialUsage = 1
                    });
                }
                GenericMapper.InjectFromObCollection(newrow.BomSizes, bomSizesTemp);
                SelectedDetailRow.TechPackBomList.Insert(SelectedDetailRow.TechPackBomList.Count(), newrow);
                SelectedTechPackBomRow = newrow;
                SaveTechPackBomRow();
        }

        internal void RemoveApprovedSalesOrder()
        {
             StyleService.StyleServiceClient _client = new StyleService.StyleServiceClient();
            _client.RemoveSalesOrderApprovalAsync(SelectedDetailRow.SalesOrderCode);
            _client.RemoveSalesOrderApprovalCompleted += (s, sv) => 
            {
                if(sv.Result > 1)
                MessageBox.Show("Process Complete");
            };
           
        }

        internal void GetStyleSpecDetail()
        {
            lkpClient.GetStyleSpecDetailsAsync(SelectedMainRow.Iserial);
            lkpClient.GetStyleSpecDetailsCompleted += (s, sv) => 
            {
                SelectedMainRow.StyleSpecDetailsList.Clear();
                foreach (var row in sv.Result.OrderBy(w => w.Iserial).ToList())
                {
                    var newrow = new TblStyleSpecDetailsViewModel();
                    newrow.InjectFrom(row);
                    try
                    {
                        newrow.InjectFrom(row);
                        newrow.TblStyleSpecTypePerRow = new LkpData.TblStyleSpecType().InjectFrom(StyleSpecTypesList .FirstOrDefault(w => w.Iserial == row.TblStyleSpecTypes)) as LkpData.TblStyleSpecType;
                        SelectedMainRow.StyleSpecDetailsList.Add(newrow);
                    }
                    catch { }

                }

                if (SelectedMainRow.StyleSpecDetailsList.Count == 0)
                {
                    var newrow = new TblStyleSpecDetailsViewModel
                    {
                        tblStyle = SelectedMainRow.Iserial,
                        Description = "",
                    };
                    SelectedMainRow.StyleSpecDetailsList.Add(newrow);
                }
            };
        }

        internal void SaveStyleSpecDetails()
        {
            ObservableCollection<LkpData.TblStyleSpecDetail> _StyleSpecdetails = new ObservableCollection<LkpData.TblStyleSpecDetail>();
            foreach (var item in SelectedMainRow.StyleSpecDetailsList)
            {
                LkpData.TblStyleSpecDetail _detail = new LkpData.TblStyleSpecDetail();
                _detail.Iserial = item.Iserial;
                _detail.TblStyle = SelectedMainRow.Iserial;
                _detail.Description = item.Description;
                _detail.TblStyleSpecTypes = item.TblStyleSpecTypes;
                if(item.TblStyleSpecDetailAttachment != null)
                {
                    _detail.tblStyleSpecDetailAttachments = item.TblStyleSpecDetailAttachment;
                }
                _StyleSpecdetails.Add(_detail);
            }
            lkpClient.UpdateOrInsertStyleSpecDetailsAsync(_StyleSpecdetails);
            lkpClient.UpdateOrInsertStyleSpecDetailsCompleted += (s, sv) => {

                SelectedMainRow.StyleSpecDetailsList.Clear();
                foreach (var row in sv.Result.OrderBy(w => w.Iserial).ToList())
                {
                    var newrow = new TblStyleSpecDetailsViewModel();
                    newrow.InjectFrom(row);
                    try
                    {
                        newrow.InjectFrom(row);
                        newrow.TblStyleSpecTypePerRow = new LkpData.TblStyleSpecType().InjectFrom(StyleSpecTypesList.FirstOrDefault(w => w.Iserial == row.TblStyleSpecTypes)) as LkpData.TblStyleSpecType;
                        SelectedMainRow.StyleSpecDetailsList.Add(newrow);
                    }
                    catch { }

                }

                if (SelectedMainRow.StyleSpecDetailsList.Count == 0)
                {
                    var newrow = new TblStyleSpecDetailsViewModel
                    {
                        tblStyle = SelectedMainRow.Iserial,
                        Description = "",
                    };
                    SelectedMainRow.StyleSpecDetailsList.Add(newrow);
                }
            };
        }

        internal void DeleteStyleSpecDetailsRows(List<TblStyleSpecDetailsViewModel> _selectedStyleSpecDetails)
        {
            foreach (var item in _selectedStyleSpecDetails)
            {
                if(item.Iserial > 0)
                lkpClient.DeleteStyleSpecDetailRowAsync(item.Iserial, item.tblStyle);
                lkpClient.DeleteStyleSpecDetailRowCompleted += (s, sv) => { };
            }
        }
    }
}