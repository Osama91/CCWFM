using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Printing;
using CCWFM.CRUDManagerService;
using CCWFM.GlService;
using CCWFM.UserControls;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.PrintPreviews
{
    public partial class BarcodePrintPreview
    {
        static public List<DefectsData> DefectsDataList = new List<DefectsData>();
        private CRUD_ManagerServiceClient webService = new CRUD_ManagerServiceClient();

        private readonly bool PrintingFlag;

        public BarcodePrintPreview(List<DefectsData> DefectsData, ObservableCollection<PrintingFabricDefect> PrintingFabricDefects, int Operation, string PrintingCode, bool Print)
        {
            InitializeComponent();
            PrintingFlag = Print;
            DefectsDataList = DefectsData;

            PrintingList = new ObservableCollection<PrintingFabricDefectslist>();
            LayoutSettings = new ObservableCollection<BarcodeLayoutSettings>();
            if (Print)
            {
                webService.BarCodePrintLayoutOperationAsync(Operation, PrintingCode);
                webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        LayoutSettings.Add(MapToLayoutSettings(item));
                    }

                    foreach (var item in PrintingFabricDefects)
                    {
                        PrintingList.Add(MapToPrintingFabricDefects(item));
                    }
                    LoadThings();
                    DataContext = LayoutSettings.OrderByDescending(x => x.PageHeightProperty).FirstOrDefault();
                };
            }
            else
            {
                webService.BarCodePrintLayoutOperationAsync(Operation, PrintingCode);
                webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        LayoutSettings.Add(MapToLayoutSettings(item));
                    }

                    foreach (var item in PrintingFabricDefects)
                    {
                        PrintingList.Add(MapToPrintingFabricDefects(item));
                    }
                    LoadThings();
                    DataContext = LayoutSettings.OrderByDescending(x => x.PageHeightProperty).FirstOrDefault();
                    DialogResult = true;
                    Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
                };
            }
        }

        public BarcodePrintPreview(TblSalesOrderColorViewModel salesOrderColor, TblStyleViewModel tblStyle, int operation, string printingCode, bool Print)
        {
            InitializeComponent();
            PrintingFlag = Print;
            PrintingList = new ObservableCollection<PrintingFabricDefectslist>();
            LayoutSettings = new ObservableCollection<BarcodeLayoutSettings>();
            if (Print)
            {
                webService.BarCodePrintLayoutOperationAsync(operation, printingCode);
                webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        LayoutSettings.Add(MapToLayoutSettings(item));
                    }
                    DataContext = LayoutSettings.OrderByDescending(x => x.PageHeightProperty).FirstOrDefault();
                    LoadThingsPoBarcode(salesOrderColor, tblStyle);
                    DialogResult = true;
                    Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
                };
            }
        }

        public BarcodePrintPreview(TblAssetsViewModel asset, int operation, string printingCode, bool Print)
        {
            InitializeComponent();
            PrintingFlag = Print;
            PrintingList = new ObservableCollection<PrintingFabricDefectslist>();
            LayoutSettings = new ObservableCollection<BarcodeLayoutSettings>();
            if (Print)
            {
                webService.BarCodePrintLayoutOperationAsync(operation, printingCode);
                webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        LayoutSettings.Add(MapToLayoutSettings(item));
                    }
                    DataContext = LayoutSettings.OrderByDescending(x => x.PageHeightProperty).FirstOrDefault();
                    LoadThingsPoBarcode(asset);
                    DialogResult = true;
                    Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
                };
            }
        }

        public BarcodePrintPreview(bankchequePrint_Result bankCheque, int operation, string printingCode, bool Print)
        {
            InitializeComponent();
            PrintingFlag = Print;
            PrintingList = new ObservableCollection<PrintingFabricDefectslist>();
            LayoutSettings = new ObservableCollection<BarcodeLayoutSettings>();
            if (Print)
            {
                webService.BarCodePrintLayoutOperationAsync(operation, printingCode);
                webService.BarCodePrintLayoutOperationCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        LayoutSettings.Add(MapToLayoutSettings(item));
                    }
                    DataContext = LayoutSettings.OrderByDescending(x => x.PageHeightProperty).FirstOrDefault();
                    LoadThingsPoBarcode(bankCheque);
                    DialogResult = true;
                    Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
                };
            }
        }
        private void LoadThingsPoBarcode(bankchequePrint_Result bankcheque)
        {
            DisplayBarcodeUsercontrol = new ObservableCollection<DisplayingBarcodeToPrintUserControl>();

            var dateLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 14);
            var payToLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 15);
            var amountLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 16);
            var amountStringLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 18);


            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(bankcheque.Date.ToString(), dateLayout, LayoutSettings, null));
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(bankcheque.PayTo, payToLayout, LayoutSettings, null));
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(bankcheque.Amount.ToString(CultureInfo.InvariantCulture), amountLayout, LayoutSettings, null));
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(bankcheque.AmountString.ToString(CultureInfo.InvariantCulture), amountStringLayout, LayoutSettings, null));
            

            var newcan = new Canvas();
            foreach (var barcodes in DisplayBarcodeUsercontrol)
            {
                if (PrintingFlag)
                {
                    newcan.Children.Add(barcodes);
                }
                if (!PrintingFlag)
                {
                    CanvasPreview.Children.Add(barcodes);
                }
            }

            if (PrintingFlag)
            {
                print(newcan);
                //     CanvasList.Add(Newcan);
            }
        }
        private void LoadThingsPoBarcode(TblAssetsViewModel asset)
        {
            DisplayBarcodeUsercontrol = new ObservableCollection<DisplayingBarcodeToPrintUserControl>();

            var assetLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 13);
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(asset.Code, assetLayout, LayoutSettings, null));
            var newcan = new Canvas();
            foreach (var barcodes in DisplayBarcodeUsercontrol)
            {
                if (PrintingFlag)
                {
                    newcan.Children.Add(barcodes);
                }
                if (!PrintingFlag)
                {
                    CanvasPreview.Children.Add(barcodes);
                }
            }

            if (PrintingFlag)
            {
                print(newcan);
                //     CanvasList.Add(Newcan);
            }
        }

        private void LoadThingsPoBarcode(TblSalesOrderColorViewModel salesOrderColor, TblStyleViewModel tblStyle)
        {
            DisplayBarcodeUsercontrol = new ObservableCollection<DisplayingBarcodeToPrintUserControl>();

            var styleColorSizeLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 10);
            var colorThemeLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 11);
            var deliveryDateLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 12);
            var sizeStr = "";
            foreach (var size in salesOrderColor.SalesOrderSizeRatiosList.Where(x => x.ProductionPerSize > 0))
            {
                sizeStr = sizeStr + "_" + size.Size;
            }

            var styleColorSizesStr = tblStyle.StyleCode + "_" +
                                        salesOrderColor.ColorPerRow.Code + "_"
                //+ salesOrderColor.SalesOrderSizeRatiosList.Where(x => x.ProductionPerSize > 0).Min(x => x.ProductionPerSize) 
                                        + 1
                                        + sizeStr;

            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(styleColorSizesStr, styleColorSizeLayout, LayoutSettings, null));
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(salesOrderColor.ThemePerRow.Ename, colorThemeLayout, LayoutSettings, null));
            DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(salesOrderColor.DeliveryDate.Value.Date.ToShortDateString(), deliveryDateLayout, LayoutSettings, null));

            var newcan = new Canvas();
            foreach (var barcodes in DisplayBarcodeUsercontrol)
            {
                if (PrintingFlag)
                {
                    newcan.Children.Add(barcodes);
                }
                if (!PrintingFlag)
                {
                    CanvasPreview.Children.Add(barcodes);
                }
            }

            if (PrintingFlag)
            {
                print(newcan);
                //     CanvasList.Add(Newcan);
            }
        }

        private void LoadThings()
        {
            foreach (var item in PrintingList)
            {
                DisplayBarcodeUsercontrol = new ObservableCollection<DisplayingBarcodeToPrintUserControl>();
                foreach (var layoutItems in LayoutSettings.Where(x => x.printingPropertiesIserial == 0))
                {
                    DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl("", layoutItems, LayoutSettings, null));
                }

                var fabricCodeLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 1);
                var barCodeLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 2);
                var fabricDecriptionLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 3);
                var rollQtyLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 5);
                var rollWmtLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 4);
                var degreeLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 7);
                var fabricColorLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 8);
                var batchLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 9); // For Dyeing Only
                var weightLayout = LayoutSettings.SingleOrDefault(x => x.printingPropertiesIserial == 14);
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.Fabric_Code, fabricCodeLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.FabricDecription, fabricDecriptionLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.NetRollQty, rollQtyLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.RollWMT.ToString(), rollWmtLayout, LayoutSettings, DefectsDataList.Where(x => x.Iserial == item.RecordNumber).ToList()));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.BarCode, barCodeLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.M2WeightGm.ToString(), weightLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.Degree.ToString(), degreeLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.ColorCode, fabricColorLayout, LayoutSettings, null));
                DisplayBarcodeUsercontrol.Add(new DisplayingBarcodeToPrintUserControl(item.BatchNo, batchLayout, LayoutSettings, null));

                var newcan = new Canvas();
                foreach (var barcodes in DisplayBarcodeUsercontrol)
                {
                    if (PrintingFlag)
                    {
                        newcan.Children.Add(barcodes);
                    }
                    if (!PrintingFlag)
                    {
                        CanvasPreview.Children.Add(barcodes);
                    }
                }

                if (PrintingFlag)
                {
                    print(newcan);
                    //     CanvasList.Add(Newcan);
                }
            }

            if (PrintingFlag)
            {
                //   ListPrint.ItemsSource = DisplayBarcodeUsercontrol;
                //ListPrint.DataContext = 1;
            }
        }

        private void print(Canvas NewCanvas)
        {
            var Settings = new PrinterFallbackSettings();
            //   ListPrint.Margin = new Thickness(Convert.ToDouble(FabricDefects.BarcodeSettings.LeftMargin), Convert.ToDouble(FabricDefects.BarcodeSettings.UpperMargin), Convert.ToDouble(FabricDefects.BarcodeSettings.RightMargin), Convert.ToDouble(FabricDefects.BarcodeSettings.BottomMargin));
            var printDocument = new PrintDocument();
            printDocument.Print("SLPrintDemo document", Settings, true);
            printDocument.PrintPage += (s, sv) =>
            {
                //   uiElement.RenderSize.Height = 5;
                //  uiElement.DesiredSize.Width = 5;

                sv.PageVisual = NewCanvas;
            };
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private ObservableCollection<DisplayingBarcodeToPrintUserControl> _displayBarcodeUsercontrol;

        public ObservableCollection<DisplayingBarcodeToPrintUserControl> DisplayBarcodeUsercontrol
        {
            get
            {
                return _displayBarcodeUsercontrol;
            }
            set
            {
                if ((ReferenceEquals(_displayBarcodeUsercontrol, value) != true))
                {
                    _displayBarcodeUsercontrol = value;
                    RaisePropertyChanged("DisplayBarcodeUsercontrol");
                }
            }
        }

        private ObservableCollection<BarcodeLayoutSettings> _layoutSettings;

        public ObservableCollection<BarcodeLayoutSettings> LayoutSettings
        {
            get
            {
                return _layoutSettings;
            }
            set
            {
                if ((ReferenceEquals(_layoutSettings, value) != true))
                {
                    _layoutSettings = value;
                    RaisePropertyChanged("LayoutSettings");
                }
            }
        }

        private ObservableCollection<PrintingFabricDefectslist> _printingList;

        public ObservableCollection<PrintingFabricDefectslist> PrintingList
        {
            get
            {
                return _printingList;
            }
            set
            {
                if ((ReferenceEquals(_printingList, value) != true))
                {
                    _printingList = value;
                    RaisePropertyChanged("PrintingList");
                }
            }
        }

        private BarcodeLayoutSettings MapToLayoutSettings(BarCodePrintLayout _Object)
        {
            return new BarcodeLayoutSettings
            {
                PropertyName = _Object.PropertyName,
                PropertyNameArabic = _Object.PropertyNameArabic,
                PropertyType = _Object.PropertyType,
                FontFamily = _Object.FontFamily,
                FontSize = _Object.FontSize,
                ItalicProperty = _Object.ItalicProperty,
                CanvasLeft = _Object.CanvasLeft,
                CanvasTop = _Object.CanvasTop,
                printingPropertiesIserial = _Object.printingPropertiesIserial,
                Iserial = _Object.Iserial,
                BoldProperty = _Object.BoldProperty,
                BarcodeHeight = _Object.BarcodeHeight,
                BarcodeWidth = _Object.BarcodeWidth,
                Code = _Object.Code,
                PrintingBarcodeFormate = _Object.PrintingBarcodeFormate,
                BarcodeOperation = _Object.BarcodeOperation,
                PageHeightProperty = _Object.PageHeight,
                PageWidthProperty = _Object.PageWidth,
                PageSizeUnitProperty = _Object.PageSizeUnit,
                PageWidthSizeUnitProperty = _Object.PageWidth.ToString() + " " + _Object.PageSizeUnit,
                PageHeightSizeUnitProperty = _Object.PageHeight.ToString() + " " + _Object.PageSizeUnit,
            };
        }

        private PrintingFabricDefectslist MapToPrintingFabricDefects(PrintingFabricDefect _Object)
        {
            var currentUi = Thread.CurrentThread.CurrentUICulture;

            if (currentUi.DisplayName == "العربية")
            {
                return new PrintingFabricDefectslist
                {
                    Fabric_Code = _Object.Fabric_Code,
                    BarCode = _Object.BATCHID,
                    FabricDecription = _Object.FabricDescriptionAR,
                    Iserial = _Object.Tbl_fabricInspectionHeader,
                    RecordNumber = _Object.Iserial,
                    RollWMT = _Object.RollWMT,
                    M2WeightGm = _Object.M2WeightGm,
                    NetRollQty = _Object.NetRollQty,
                    BatchNo = _Object.Batchno,
                    Degree = _Object.Degree,
                    ColorCode = _Object.ColorCode,
                };
            }
            return new PrintingFabricDefectslist
            {
                Fabric_Code = _Object.Fabric_Code,
                BarCode = _Object.BATCHID,
                FabricDecription = _Object.FabricDescription,
                Iserial = _Object.Tbl_fabricInspectionHeader,
                RecordNumber = _Object.Iserial,
                RollWMT = _Object.RollWMT,
                NetRollQty = _Object.NetRollQty,
                BatchNo = _Object.Batchno,
                M2WeightGm = _Object.M2WeightGm,
                Degree = _Object.Degree,
                ColorCode = _Object.ColorCode,
            };
        }
    }

    public class BarcodeLayoutSettings : Web.DataLayer.PropertiesViewModelBase
    {
        private double _barcodeHeightField;

        private int _barcodeOperationField;

        private double _barcodeWidthField;

        private bool? _boldPropertyField;

        private double _canvasLeftField;

        private double _canvasTopField;

        private string _codeField;

        private string _fontFamilyField;

        private double? _fontSizeField;

        private bool? _italicPropertyField;

        private string _printingBarcodeFormateField;

        private string _propertyNameField;

        private string _propertyTypeField;

        private int _IserialField;

        private int? _printingPropertiesIserialField;

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

        public bool? BoldProperty
        {
            get
            {
                return _boldPropertyField;
            }
            set
            {
                if ((_boldPropertyField.Equals(value) != true))
                {
                    _boldPropertyField = value;
                    RaisePropertyChanged("BoldProperty");
                }
            }
        }

        public double CanvasLeft
        {
            get
            {
                return _canvasLeftField;
            }
            set
            {
                if ((_canvasLeftField.Equals(value) != true))
                {
                    _canvasLeftField = value;
                    RaisePropertyChanged("CanvasLeft");
                }
            }
        }

        public double CanvasTop
        {
            get
            {
                return _canvasTopField;
            }
            set
            {
                if ((_canvasTopField.Equals(value) != true))
                {
                    _canvasTopField = value;
                    RaisePropertyChanged("CanvasTop");
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

        public string FontFamily
        {
            get
            {
                return _fontFamilyField;
            }
            set
            {
                if ((ReferenceEquals(_fontFamilyField, value) != true))
                {
                    _fontFamilyField = value;
                    RaisePropertyChanged("FontFamily");
                }
            }
        }

        public double? FontSize
        {
            get
            {
                return _fontSizeField;
            }
            set
            {
                if ((_fontSizeField.Equals(value) != true))
                {
                    _fontSizeField = value;
                    RaisePropertyChanged("FontSize");
                }
            }
        }

        public bool? ItalicProperty
        {
            get
            {
                return _italicPropertyField;
            }
            set
            {
                if ((_italicPropertyField.Equals(value) != true))
                {
                    _italicPropertyField = value;
                    RaisePropertyChanged("ItalicProperty");
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

        private string _propertyValueField;

        public string PropertyValue
        {
            get
            {
                return _propertyValueField;
            }
            set
            {
                if ((ReferenceEquals(_propertyValueField, value) != true))
                {
                    _propertyValueField = value;
                    RaisePropertyChanged("PropertyValue");
                }
            }
        }

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
                return _IserialField;
            }
            set
            {
                if ((_IserialField.Equals(value) != true))
                {
                    _IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int? printingPropertiesIserial
        {
            get
            {
                return _printingPropertiesIserialField;
            }
            set
            {
                if ((_printingPropertiesIserialField.Equals(value) != true))
                {
                    _printingPropertiesIserialField = value;
                    RaisePropertyChanged("printingPropertiesIserial");
                }
            }
        }

        private double? _pageHeightField;

        private string _pageSizeUnitField;

        private double? _pageWidthField;

        public double? PageHeightProperty
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
                    RaisePropertyChanged("PageHeightProperty");
                }
            }
        }

        public string PageSizeUnitProperty
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
                    RaisePropertyChanged("PageSizeUnitProperty");
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

        public double? PageWidthProperty
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
                    RaisePropertyChanged("PageWidthProperty");
                }
            }
        }
    }

    public class PrintingFabricDefectslist : Web.DataLayer.PropertiesViewModelBase
    {
        private string _batchno;

        public string BatchNo
        {
            get { return _batchno; }
            set
            {
                _batchno = value;
                RaisePropertyChanged("BatchNo");
            }
        }

        private short _degree;

        public short Degree
        {
            get { return _degree; }
            set
            {
                _degree = value;
                RaisePropertyChanged("Degree");
            }
        }

        private string _colorCode;

        public string ColorCode
        {
            get { return _colorCode; }
            set
            {
                _colorCode = value;
                RaisePropertyChanged("ColorCode");
            }
        }

        private string _barcodeField;

        private string _fabricDescriptionField;

        private string _fabricCodeField;

        private int _iserialField;

        private string _netRollQtyField;

        private int _recordNumberField;

        private float? _rollWmtField;

        public string BarCode
        {
            get
            {
                return _barcodeField;
            }
            set
            {
                if ((ReferenceEquals(_barcodeField, value) != true))
                {
                    _barcodeField = value;
                    RaisePropertyChanged("BarCode");
                }
            }
        }

        public string FabricDecription
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
                    RaisePropertyChanged("FabricDecription");
                }
            }
        }

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

        public string NetRollQty
        {
            get
            {
                return _netRollQtyField;
            }
            set
            {
                if ((ReferenceEquals(_netRollQtyField, value) != true))
                {
                    _netRollQtyField = value;
                    RaisePropertyChanged("NetRollQty");
                }
            }
        }

        public int RecordNumber
        {
            get
            {
                return _recordNumberField;
            }
            set
            {
                if ((_recordNumberField.Equals(value) != true))
                {
                    _recordNumberField = value;
                    RaisePropertyChanged("RecordNumber");
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

        private float? _m2WeightGm;

        public float? M2WeightGm
        {
            get { return _m2WeightGm; }
            set { _m2WeightGm = value; RaisePropertyChanged("M2WeightGm"); }
        }

    }
}