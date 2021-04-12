using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Views.OGView;

namespace CCWFM.ViewModel.OGViewModels
{
    public class PrintingBarcodePropertiesList : PropertiesViewModelBase
    {
        private int? _operationField;

        private string _propertyNameField;

        public int? Operation
        {
            get
            {
                return _operationField;
            }
            set
            {
                if ((_operationField.Equals(value) != true))
                {
                    _operationField = value;
                    RaisePropertyChanged("Operation");
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyNameField;
            }
            set
            {
                if ((ReferenceEquals(_propertyNameField, value) != true))
                {
                    _propertyNameField = value;
                    RaisePropertyChanged("PropertyName");
                }
            }
        }

        private string _propertyNameArabicField;

        public string PropertyNameArabic
        {
            get
            {
                return _propertyNameArabicField;
            }
            set
            {
                if ((ReferenceEquals(_propertyNameArabicField, value) != true))
                {
                    _propertyNameArabicField = value;
                    RaisePropertyChanged("PropertyNameArabic");
                }
            }
        }

        private string _propertyTypeField;

        private int _iserialField;

        public string PropertyType
        {
            get
            {
                return _propertyTypeField;
            }
            set
            {
                if ((ReferenceEquals(_propertyTypeField, value) != true))
                {
                    _propertyTypeField = value;
                    RaisePropertyChanged("PropertyType");
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

    public class BarcodeSettingsHeader : PropertiesViewModelBase
    {
        private double _barcodeHeightField;

        private int _barcodeOperationField;

        private double _barcodeWidthField;

        private string _codeField;

        private double? _pageHeightField;

        private string _pageSizeUnitField;

        private double? _pageWidthField;

        private string _printingBarcodeFormateField;

        private int _iserialField;

        public double BarcodeHeight
        {
            get
            {
                return _barcodeHeightField;
            }
            set
            {
                if ((_barcodeHeightField.Equals(value) != true))
                {
                    _barcodeHeightField = value;
                    RaisePropertyChanged("BarcodeHeight");
                }
            }
        }

        public int BarcodeOperation
        {
            get
            {
                return _barcodeOperationField;
            }
            set
            {
                if ((_barcodeOperationField.Equals(value) != true))
                {
                    _barcodeOperationField = value;
                    RaisePropertyChanged("BarcodeOperation");
                }
            }
        }

        public double BarcodeWidth
        {
            get
            {
                return _barcodeWidthField;
            }
            set
            {
                if ((_barcodeWidthField.Equals(value) != true))
                {
                    _barcodeWidthField = value;
                    RaisePropertyChanged("BarcodeWidth");
                }
            }
        }

        public string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public double? PageHeight
        {
            get
            {
                return _pageHeightField;
            }
            set
            {
                if ((_pageHeightField.Equals(value) != true))
                {
                    _pageHeightField = value;
                    RaisePropertyChanged("PageHeight");
                }
            }
        }

        public string PageSizeUnit
        {
            get
            {
                return _pageSizeUnitField;
            }
            set
            {
                if ((ReferenceEquals(_pageSizeUnitField, value) != true))
                {
                    _pageSizeUnitField = value;
                    RaisePropertyChanged("PageSizeUnit");
                }
            }
        }

        public double? PageWidth
        {
            get
            {
                return _pageWidthField;
            }
            set
            {
                if ((_pageWidthField.Equals(value) != true))
                {
                    _pageWidthField = value;
                    RaisePropertyChanged("PageWidth");
                }
            }
        }

        private string _pageWidthSizeUnitProperty;

        public string PageWidthSizeUnitProperty
        {
            get
            {
                return _pageWidthSizeUnitProperty;
            }
            set
            {
                if ((ReferenceEquals(_pageWidthSizeUnitProperty, value) != true))
                {
                    _pageWidthSizeUnitProperty = value;
                    RaisePropertyChanged("PageWidthSizeUnitProperty");
                }
            }
        }

        private string _pageHeightSizeUnitProperty;

        public string PageHeightSizeUnitProperty
        {
            get
            {
                return _pageHeightSizeUnitProperty;
            }
            set
            {
                if ((ReferenceEquals(_pageHeightSizeUnitProperty, value) != true))
                {
                    _pageHeightSizeUnitProperty = value;
                    RaisePropertyChanged("PageHeightSizeUnitProperty");
                }
            }
        }

        public string PrintingBarcodeFormate
        {
            get
            {
                return _printingBarcodeFormateField;
            }
            set
            {
                if ((ReferenceEquals(_printingBarcodeFormateField, value) != true))
                {
                    _printingBarcodeFormateField = value;
                    RaisePropertyChanged("PrintingBarcodeFormate");
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

    public class BarcodeSettingsUiViewModel : ViewModelBase
    {
        public event EventHandler SubmitClicked;

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();

        private ObservableCollection<PrintingBarcodePropertiesList> _printingBarcodePropertiesList;

        public ObservableCollection<PrintingBarcodePropertiesList> PrintingBarcodePropertiesList
        {
            get
            {
                return _printingBarcodePropertiesList;
            }
            set
            {
                _printingBarcodePropertiesList = value;
                RaisePropertyChanged("PrintingBarcodePropertiesList");
            }
        }

        private List<PrintingBarcodePropertiesList> _printingBarcodePropertiesListPerHeader;

        public List<PrintingBarcodePropertiesList> PrintingBarcodePropertiesListPerHeader
        {
            get
            {
                return _printingBarcodePropertiesListPerHeader;
            }
            set
            {
                _printingBarcodePropertiesListPerHeader = value;
                RaisePropertyChanged("PrintingBarcodePropertiesListPerHeader");
            }
        }

        private ObservableCollection<string> _barcodeFormate;

        public ObservableCollection<string> BarcodeFormate
        {
            get
            {
                return _barcodeFormate;
            }
            set
            {
                _barcodeFormate = value;
                RaisePropertyChanged("BarcodeFormate");
            }
        }

        private ObservableCollection<BarcodeSettingsHeader> _barcodeSettingHeaderList;

        public ObservableCollection<BarcodeSettingsHeader> BarcodeSettingHeaderList
        {
            get
            {
                return _barcodeSettingHeaderList;
            }
            set
            {
                _barcodeSettingHeaderList = value;
                RaisePropertyChanged("BarcodeSettingHeaderList");
            }
        }

        private ObservableCollection<GenericBarcodeTemplate> _genericBarcodeTemplate;

        public ObservableCollection<GenericBarcodeTemplate> GenericBarcodeTemplate
        {
            get
            {
                return _genericBarcodeTemplate;
            }
            set
            {
                _genericBarcodeTemplate = value;
                RaisePropertyChanged("GenericBarcodeTemplate");
            }
        }

        private GenericBarcodeTemplate _selectedTemplate;

        public GenericBarcodeTemplate SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                _selectedTemplate = value;
                RaisePropertyChanged("SelectedTemplate");
            }
        }

        private ObservableCollection<tbl_lkp_BarcodeOperations> _barcodeOperationsList;

        public ObservableCollection<tbl_lkp_BarcodeOperations> BarcodeOperationsList
        {
            get
            {
                return _barcodeOperationsList;
            }
            set
            {
                _barcodeOperationsList = value;
                RaisePropertyChanged("BarcodeOperationsList");
            }
        }

        public BarcodeSettingsUiViewModel()
        {
            PrintingBarcodePropertiesList = new ObservableCollection<PrintingBarcodePropertiesList>();
            _webService.GetBarcodePropertyAsync();

            BarcodeOperationsList = new ObservableCollection<tbl_lkp_BarcodeOperations>();
            _webService.GetBarcodeOperationsLkpAsync();
            _webService.GetBarcodeOperationsLkpCompleted += (s, sv) =>
            {
                foreach (var item in sv.Result)
                {
                    BarcodeOperationsList.Add(item);
                }
                GetData();
            };

            var values = Enum.GetValues(typeof(BarcodeFormatEnum));
            BarcodeFormate = new ObservableCollection<string>();
            foreach (BarcodeFormatEnum item in values)
            {
                BarcodeFormate.Add(item.ToString());
            }

            _webService.GetBarcodeDisplaySettingsHeaderCompleted += (s, sv) =>
            {
                foreach (var item in sv.Result)
                {
                    BarcodeSettingHeaderList.Add(MapTOBarcodeSettingsHeader(item));
                }
            };

            _webService.GetBarcodePropertyCompleted += (s, sv) =>
            {
                foreach (var item in sv.Result)
                {
                    PrintingBarcodePropertiesList.Add(MapTopBarcodeProperties(item));
                }
            };

            _webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
            {
                if (sv.Result.Count == 0)
                {
                    foreach (var item in PrintingBarcodePropertiesListPerHeader)
                    {
                        GenericBarcodeTemplate.Add(MapToBarcodeSettingsDetailsDefault(item));
                    }
                }
                else
                {
                    foreach (var item in sv.Result)
                    {
                        GenericBarcodeTemplate.Add(MapToBarcodeSettingsDetails(item));
                    }
                }
                SubmitClicked(this, new EventArgs());
            };
        }

        private PrintingBarcodePropertiesList MapTopBarcodeProperties(PrintingBarcodeProperty _Object)
        {
            return new PrintingBarcodePropertiesList
            {
                Iserial = _Object.Iserial,
                Operation = _Object.Operation,
                PropertyName = _Object.PropertyName,
                PropertyType = _Object.PropertyType,
                PropertyNameArabic = _Object.PropertyNameArabic
            };
        }

        public void GetData()
        {
            BarcodeSettingHeaderList = new ObservableCollection<BarcodeSettingsHeader>();
            _webService.GetBarcodeDisplaySettingsHeaderAsync();
        }

        private BarcodeSettingsHeader MapTOBarcodeSettingsHeader(BarcodeDisplaySettingsHeader _Object)
        {
            return new BarcodeSettingsHeader
            {
                Iserial = _Object.Iserial,
                BarcodeHeight = _Object.BarcodeHeight,
                BarcodeWidth = _Object.BarcodeWidth,
                Code = _Object.Code,
                PrintingBarcodeFormate = _Object.PrintingBarcodeFormate,
                BarcodeOperation = _Object.BarcodeOperation,
                PageHeight = _Object.PageHeight,
                PageWidth = _Object.PageWidth,
                PageSizeUnit = _Object.PageSizeUnit,
                PageWidthSizeUnitProperty = _Object.PageWidth + " " + _Object.PageSizeUnit,
                PageHeightSizeUnitProperty = _Object.PageHeight + " " + _Object.PageSizeUnit,
            };
        }

        private GenericBarcodeTemplate MapToBarcodeSettingsDetailsDefault(PrintingBarcodePropertiesList _Object)
        {
            return new GenericBarcodeTemplate(this)
            {
                PropertyNameArabic = _Object.PropertyNameArabic,
                PropertyName = _Object.PropertyName,
                PropertyType = _Object.PropertyType,
                BarcodePropertiesIserial = _Object.Iserial,
                FontSizeProp = 12,
                BoldProperty = false,
                ItalicProperty = false,
                FontFamilyProp = "Arial",
                CanvasLeftPropperty = 0,
                CanvasTopPropperty = 0
            };
        }

        private GenericBarcodeTemplate MapToBarcodeSettingsDetails(BarCodePrintLayout _Object)
        {
            return new GenericBarcodeTemplate(this)
            {
                Code = _Object.Code,
                PropertyName = _Object.PropertyName,
                PropertyType = _Object.PropertyType,
                PropertyNameArabic = _Object.PropertyNameArabic,
                BarcodePropertiesIserial = _Object.printingPropertiesIserial,
                FontSizeProp = _Object.FontSize,
                BoldProperty = _Object.BoldProperty,
                ItalicProperty = _Object.ItalicProperty,
                FontFamilyProp = _Object.FontFamily,
                CanvasLeftPropperty = _Object.CanvasLeft,
                CanvasTopPropperty = _Object.CanvasTop,
            };
        }

        public void GetDetails(BarcodeSettingsHeader headerobject)
        {
            SelectedTemplate = new GenericBarcodeTemplate(this);

            PrintingBarcodePropertiesListPerHeader = new List<PrintingBarcodePropertiesList>();

            PrintingBarcodePropertiesListPerHeader = PrintingBarcodePropertiesList.Where(x => x.Operation == headerobject.BarcodeOperation).ToList();

            GenericBarcodeTemplate = new ObservableCollection<GenericBarcodeTemplate>();
            GenericBarcodeTemplate.CollectionChanged += GenericBarcodeTemplate_CollectionChanged;
            _webService.BarCodePrintLayoutOperationAsync(headerobject.BarcodeOperation, headerobject.Code);
        }

        private void GenericBarcodeTemplate_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (GenericBarcodeTemplate item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (GenericBarcodeTemplate item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "Itemsvisiblity")
            {
                if (GenericBarcodeTemplate.FirstOrDefault(x => x.Itemsvisiblity == Visibility.Visible) != null)
                {
                    SelectedTemplate = GenericBarcodeTemplate.FirstOrDefault(x => x.Itemsvisiblity == Visibility.Visible);
                }
            }
        }

        public void SaveBarcodeDisplaySettingsHeader(BarcodeSettingsHeader headerobject)
        {
            _webService.UpdateAndSaveBarcodeDisplaySettingsHeaderAsync(headerobject.Iserial, headerobject.BarcodeOperation, headerobject.PrintingBarcodeFormate, headerobject.BarcodeWidth, headerobject.BarcodeHeight, headerobject.Code, Convert.ToDouble(headerobject.PageWidth), Convert.ToDouble(headerobject.PageHeight), headerobject.PageSizeUnit);
        }

        public void DeleteBarcodeDisplaySettingsHeader(BarcodeSettingsHeader headerobject)
        {
            _webService.DeleteBarcodeDisplaySettingsHeaderAsync(headerobject.Iserial);
        }

        public void SaveDetails(BarcodeSettingsHeader headerobject)
        {
            foreach (var item in GenericBarcodeTemplate)
            {
                _webService.UpdateAndSaveBarcodeDisplaySettingsDetailsAsync(headerobject.Iserial, item.FontSizeProp, item.BoldProperty, item.ItalicProperty, item.CanvasTopPropperty, item.CanvasLeftPropperty, item.FontFamilyProp, Convert.ToInt32(item.BarcodePropertiesIserial), item.PropertyName, item.PropertyNameArabic, item.PropertyType);
            }
        }
    }
}