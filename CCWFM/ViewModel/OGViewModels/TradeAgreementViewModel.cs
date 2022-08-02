using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.Helpers;
using CCWFM.ProductionService;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows.Controls;
using Os.Controls.DataGrid.Events;
using CCWFM.Helpers.Utilities;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TradeAgreementTransaction : TblTradeAgreementTransaction
    {
        public TradeAgreementTransaction()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            DetailsList = new ObservableCollection<TradeAgreementDetailModel>();
        }
        public new DateTime ToDate
        {
            get { return base.ToDate; }
            set
            {
                if ((base.ToDate.Equals(value) != true))
                {
                    base.ToDate = value;
                    if (base.ToDate == null) { base.FromDate = DateTime.Now; }
                }
            }
        }

        private ObservableCollection<TradeAgreementDetailModel> _detail;
        public ObservableCollection<TradeAgreementDetailModel> DetailsList
        {
            get { return _detail; }
            set
            {
                if (_detail != value)
                {
                    _detail = value; RaisePropertyChanged(nameof(DetailsList));
                }
            }
        }

        public new DateTime FromDate
        {
            get { return base.FromDate; }
            set
            {
                if ((base.FromDate.Equals(value) != true))
                {
                    base.FromDate = value;
                    if (base.ToDate == null) { base.ToDate = DateTime.Now; }
                }
            }
        }
    }

    public class TradeAgreementDetailModel : GlService.PropertiesViewModelBase
    {
        CRUDManagerService.CRUD_ManagerServiceClient Client = new CRUDManagerService.CRUD_ManagerServiceClient();

        public TradeAgreementDetailModel()
        {
            Client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = new ObservableCollection<CRUDManagerService.TblColor>();
                foreach (var item in sv.Result.AccConfigList)
                {
                    ItemPerRow.AccConfigList.Add(item);
                }
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = new ObservableCollection<CRUDManagerService.tbl_AccessoryAttributesDetails>();
                foreach (var item in sv.Result.CombinationList)
                {
                    ItemPerRow.CombinationList.Add(item);
                }
                //ItemPerRow.CombinationList.InjectFrom(sv.Result.CombinationList);

                if (ItemPerRow.CombinationList != null)
                {
                    var sizes = ItemPerRow.CombinationList.Where(
                            x => ColorPerRow != null && (x.Configuration == ColorPerRow.Code));

                    var distinctsize = sizes.Select(x => x.Size);
                    foreach (var size in distinctsize)
                    {
                        if (!ItemPerRow.SizesList.Contains(size))
                        {
                            ItemPerRow.SizesList.Add(size);
                        }
                    }
                }
            };
        }

        private DateTime? _fromDate = DateTime.Now;
        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                if ((_fromDate.Equals(value) != true))
                {
                    _fromDate = value;
                    if (ToDate == null)
                    {
                        ToDate = DateTime.Now;
                    }
                    RaisePropertyChanged(nameof(FromDate));
                }
            }
        }

        private DateTime? _toDate = DateTime.Now;
        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                if ((_toDate.Equals(value) != true))
                {
                    _toDate = value;
                    if (ToDate == null)
                    {
                        ToDate = DateTime.Now;
                    }
                    RaisePropertyChanged(nameof(ToDate));
                }
            }
        }

        private CRUDManagerService.Vendor _vendorPerRow;
        public CRUDManagerService.Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value; RaisePropertyChanged(nameof(VendorPerRow));
                if (VendorPerRow != null) Vendor = VendorPerRow.vendor_code;
            }
        }

        private string _vendor;
        public string Vendor
        {
            get
            {
                return _vendor;
            }
            set
            {
                if ((ReferenceEquals(_vendor, value) != true))
                {
                    _vendor = value;
                    RaisePropertyChanged(nameof(Vendor));
                }
            }
        }

        private CRUDManagerService.GenericTable _vendorPurchaseGroupPerRow;
        public CRUDManagerService.GenericTable VendorPurchaseGroupPerRow
        {
            get { return _vendorPurchaseGroupPerRow; }
            set
            {
                _vendorPurchaseGroupPerRow = value; RaisePropertyChanged(nameof(VendorPurchaseGroupPerRow));
                if (_vendorPurchaseGroupPerRow != null)
                {
                    if (_vendorPurchaseGroupPerRow.Iserial != 0)
                    {
                        TblVendorPurchaseGroup = _vendorPurchaseGroupPerRow.Iserial;
                    }
                }
            }
        }

        private CRUDManagerService.TblColor _colorPerRow;
        public CRUDManagerService.TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                _colorPerRow = value; RaisePropertyChanged(nameof(ColorPerRow));
                if (_colorPerRow != null)
                {
                    TblColor = _colorPerRow.Iserial;
                    if (IsAcc)
                    {
                        ItemPerRow.SizesList.Clear();
                        var sizes =
                            ItemPerRow.CombinationList.Where(
                                x => (x.Configuration == ColorPerRow.Code));

                        var distinctsize = sizes.Select(x => x.Size);
                        foreach (var size in distinctsize)
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

        private ObservableCollection<CRUDManagerService.TblColor> _colors;
        public ObservableCollection<CRUDManagerService.TblColor> Colors
        {
            get { return _colors; }
            set { _colors = value; RaisePropertyChanged(nameof(Colors)); }
        }

        private ObservableCollection<CRUDManagerService.ItemsDto> _item;
        public ObservableCollection<CRUDManagerService.ItemsDto> Items
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged(nameof(Items)); }
        }

        private bool _acc;
        public bool IsAcc
        {
            get { return _acc; }
            set { _acc = value; RaisePropertyChanged(nameof(IsAcc)); }
        }

        private CRUDManagerService.ItemsDto _itemPerRow;
        public CRUDManagerService.ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged(nameof(ItemPerRow));
                if (_itemPerRow != null)
                {
                    ItemCode = ItemPerRow.Iserial;
                    ItemType = ItemPerRow.ItemGroup;
                    if (ItemPerRow.ItemGroup != null)
                        IsAcc = ItemPerRow.ItemGroup.StartsWith("Acc");
                    if (IsAcc)
                    {
                        var temp = new CRUDManagerService.ItemsDto();
                        temp.InjectFrom(ItemPerRow);
                        Client.AccWithConfigAndSizeAsync(temp);
                    }
                }
            }
        }

        private int _headerIserial;
        public int HeaderIserial
        {
            get { return _headerIserial; }
            set
            {
                if ((_Iserial.Equals(value) != true))
                {
                    _headerIserial = value;
                    RaisePropertyChanged(nameof(HeaderIserial));
                }
            }
        }

        private int _tradeAgreementTransactionIserial;
        public int TradeAgreementTransactionIserial
        {
            get
            {
                return _tradeAgreementTransactionIserial;
            }
            set
            {
                if ((_tradeAgreementTransactionIserial.Equals(value) != true))
                {
                    _tradeAgreementTransactionIserial = value;
                    RaisePropertyChanged(nameof(TradeAgreementTransactionIserial));
                }
            }
        }


        private string _supplierColorCode;
        public string SupplierColorCode
        {
            get { return _supplierColorCode; }
            set { _supplierColorCode = value; RaisePropertyChanged(nameof(SupplierColorCode)); }
        }

        private string _pantone;
        public string Pantone
        {
            get { return _pantone; }
            set { _pantone = value; RaisePropertyChanged(nameof(Pantone)); }
        }

        private int? _tblVendorPurchaseGroup;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = nameof(strings.ReqVendorPurchaseGroup))]
        public int? TblVendorPurchaseGroup
        {
            get { return _tblVendorPurchaseGroup; }
            set { _tblVendorPurchaseGroup = value; RaisePropertyChanged(nameof(TblVendorPurchaseGroup)); }
        }

        private int _createdBy;
        public int CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; RaisePropertyChanged(nameof(CreatedBy)); }
        }

        private int _lastUpdatedBy;
        public int LastUpdatedBy
        {
            get { return _lastUpdatedBy; }
            set { _lastUpdatedBy = value; RaisePropertyChanged(nameof(LastUpdatedBy)); }
        }

        private DateTime _lastUpdatedDate = DateTime.Now;
        public DateTime LastUpdatedDate
        {
            get { return _lastUpdatedDate; }
            set { _lastUpdatedDate = value; RaisePropertyChanged(nameof(LastUpdatedDate)); }
        }

        private DateTime _creationDate = DateTime.Now;
        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged(nameof(CreationDate)); }
        }

        private double _shippingPercentage;
        public double ShippingPercentage
        {
            get { return _shippingPercentage; }
            set
            {
                _shippingPercentage = value;
                RaisePropertyChanged(nameof(ShippingPercentage));
            }
        }

        private double _customsPercentage;
        public double CustomsPercentage
        {
            get { return _customsPercentage; }
            set { _customsPercentage = value; RaisePropertyChanged(nameof(CustomsPercentage)); }
        }

        private double _salesPercentage;
        public double SalesPercentage
        {
            get { return _salesPercentage; }
            set { _salesPercentage = value; RaisePropertyChanged(nameof(SalesPercentage)); }
        }

        private int? _color;
        public int? TblColor
        {
            get
            {
                return _color;
            }
            set
            {
                if ((Equals(_color, value) != true))
                {
                    _color = value;
                    RaisePropertyChanged(nameof(TblColor));
                }
            }
        }

        private string _itemType;
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; RaisePropertyChanged(nameof(ItemType)); }
        }

        private string _accSize;
        public string AccSize
        {
            get { return _accSize; }
            set { _accSize = value; RaisePropertyChanged(nameof(AccSize)); }
        }

        private int _Iserial;
        public int Iserial
        {
            get { return _Iserial; }
            set
            {
                if ((_Iserial.Equals(value) != true))
                {
                    _Iserial = value;
                    RaisePropertyChanged(nameof(Iserial));
                }
            }
        }

        private int _days;
        public int Days
        {
            get { return _days; }
            set { _days = value; RaisePropertyChanged(nameof(Days)); }
        }

        private int? _itemCode;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = nameof(strings.ReqItem))]
        public int? ItemCode
        {
            get
            {
                return _itemCode;
            }
            set
            {
                if ((Equals(_itemCode, value) != true))
                {
                    _itemCode = value;
                    // Client.GetAxConfigTableAsync(_itemCodeField, "ccm");
                    RaisePropertyChanged(nameof(ItemCode));
                }
            }
        }

        private string _currencyCode;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = nameof(strings.ReqCurrency))]
        public string CurrencyCode
        {
            get
            {
                return _currencyCode;
            }
            set
            {
                _currencyCode = value;
                RaisePropertyChanged(nameof(CurrencyCode));
            }
        }

        private float _price;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = nameof(strings.ReqPrice))]
        public float Price
        {
            get
            {
                return _price;
            }
            set
            {
                if ((_price.Equals(value) != true))
                {
                    _price = value;
                    RaisePropertyChanged(nameof(Price));
                }
            }
        }

        private string _VendorCurrencyCode;

        public string VendorCurrencyCode
        {
            get
            {
                return _VendorCurrencyCode;
            }
            set
            {
                _VendorCurrencyCode = value;
                RaisePropertyChanged(nameof(VendorCurrencyCode));
            }
        }

        private float _VendorPrice;

        public float VendorPrice
        {
            get
            {
                return _VendorPrice;
            }
            set
            {
                if ((_VendorPrice.Equals(value) != true))
                {
                    _VendorPrice = value;
                    RaisePropertyChanged(nameof(_VendorPrice));
                }
            }
        }

    }

    public class TradeAgreementViewModel : ViewModelStructuredBase
    {
        internal ProductionServiceClient productionServiceClient = Services.Instance.GetProductionServiceClient();
        Dictionary<string, object> valueObjecttemp = new Dictionary<string, object>();

        CRUDManagerService.CRUD_ManagerServiceClient calculationClient = Services.Instance.GetCRUD_ManagerServiceClient();

        public TradeAgreementViewModel() : base(PermissionItemName.TradeAgreementFabricView)
        {
            // ده هيحتاج تعديل
            productionServiceClient.GetTblTradeAgreementDetailListFabricViewCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TradeAgreementDetailModel();
                    newrow.TradeAgreementTransactionIserial = SelectedMainRow.Iserial;
                    newrow.HeaderIserial = row.TblTradeAgreementHeader;
                    newrow.ColorPerRow = new CRUDManagerService.TblColor();
                    if (row.TblColor1 != null)
                        newrow.ColorPerRow.InjectFrom(row.TblColor1);
                    newrow.ItemPerRow = new CRUDManagerService.ItemsDto();
                    newrow.ItemPerRow.InjectFrom(sv.itemsList.SingleOrDefault(x => x.Iserial == row.ItemCode && x.ItemGroup == row.ItemType));
                    newrow.ItemType = row.ItemType;
                    newrow.VendorPerRow = new CRUDManagerService.Vendor { vendor_code = row.TblTradeAgreementHeader1.Vendor };
                    newrow.VendorPurchaseGroupPerRow = new CRUDManagerService.GenericTable();
                    newrow.VendorPerRow.InjectFrom(sv.VendorList.FirstOrDefault(x => x.vendor_code == row.TblTradeAgreementHeader1.Vendor));
                    if (row.TblVendorPurchaseGroup1 != null)
                        newrow.VendorPurchaseGroupPerRow.InjectFrom(row.TblVendorPurchaseGroup1);
                    newrow.TradeAgreementTransactionIserial = row.TblTradeAgreementHeader1.TblTradeAgreementTransaction.Value;
                    newrow.HeaderIserial = row.TblTradeAgreementHeader;
                    newrow.InjectFrom(row);
                    SelectedMainRow.DetailsList.Insert(0, newrow);
                }

                Loading = false;

                if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                {
                    AddNewDetailRow(false);
                }
                if (Export)
                {
                    Export = false;
                    ExportGrid.ExportExcel("TradeAgreement");
                }
            }
            ;
            calculationClient.GetGenericCompleted += (s, sv) =>
            {
                VendorPurchaseGroupList = sv.Result;
            };

            Client.GetAxCurrencyAsync("CCM");
            Client.GetAxCurrencyCompleted += (s, sv) =>
            {
                AxCurrencyList = sv.Result;
            };
            Client.SearchForColorCompleted += (s, sv) =>
            {
                if (sv.Error != null) return;

                SelectedDetailRow.Colors = sv.Result;
            };

            var currentUi = Thread.CurrentThread.CurrentUICulture;
            productionServiceClient.GetTblTradeAgreementDetailListCompleted += (d, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TradeAgreementDetailModel();
                    newrow.InjectFrom(row);
                    newrow.HeaderIserial = row.TblTradeAgreementHeader;
                    newrow.TradeAgreementTransactionIserial = SelectedMainRow.Iserial;
                    newrow.ColorPerRow = new CRUDManagerService.TblColor();
                    if (row.TblColor1 != null)
                        newrow.ColorPerRow.InjectFrom(row.TblColor1);
                    newrow.TblColor = row.TblColor;
                    newrow.ItemPerRow = new CRUDManagerService.ItemsDto();
                    var itemsDto = sv.itemsList.SingleOrDefault(x => x.Iserial == row.ItemCode && x.ItemGroup == row.ItemType);
                    if (itemsDto != null)
                        newrow.ItemPerRow.InjectFrom(itemsDto);
                    newrow.ItemCode = row.ItemCode;
                    newrow.ItemType = row.ItemType;
                    newrow.FromDate = row.TblTradeAgreementHeader1.FromDate;
                    newrow.ToDate = row.TblTradeAgreementHeader1.ToDate;
                    newrow.VendorPurchaseGroupPerRow = VendorPurchaseGroupList.FirstOrDefault(vpg => vpg.Iserial == newrow.TblVendorPurchaseGroup);
                    newrow.VendorPerRow = new CRUDManagerService.Vendor();
                    newrow.VendorPerRow.InjectFrom(sv.vendorsList.FirstOrDefault(v => v.vendor_code == row.TblTradeAgreementHeader1.Vendor));
                    newrow.Vendor = row.TblTradeAgreementHeader1.Vendor;
                    SelectedMainRow.DetailsList.Add(newrow);
                }

                Loading = false;
                DetailFullCount = sv.fullCount;
                if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                {
                    AddNewDetailRow(false);
                }
            };

            productionServiceClient.UpdateOrInsertTblTradeAgreementDetailCompleted += (s, x) =>
            {
                var savedRow = SelectedMainRow.DetailsList.ElementAtOrDefault(x.outindex);

                if (savedRow != null) savedRow.InjectFrom(x.Result);
            };

            productionServiceClient.GetTblTradeAgreementTransactionCompleted += (d, s) =>
            {
                foreach (var variable in s.Result)
                {
                    var newrow = new TradeAgreementTransaction();
                    newrow.InjectFrom(variable);
                    MainRowList.Add(newrow);
                }
                Loading = false;
                FullCount = s.fullCount;
                if (SearchWindow != null)
                {
                    SearchWindow.FullCount = s.fullCount;
                    SearchWindow.Loading = false;
                }
                if (FullCount == 0 && MainRowList.Count == 0)
                {
                    AddNewMainRow(true);
                }
            };

            productionServiceClient.DeleteTblTradeAgreementDetailCompleted += (s, ev) =>
            {
                Loading = false;
                if (ev.Error != null)
                {
                    throw ev.Error;
                }

                var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);

                if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
            };

            productionServiceClient.SaveTradeAgreementCompleted += (s, sv) =>
            {
                if (SelectedMainRow == null)
                    SelectedMainRow = new TradeAgreementTransaction();
                SelectedMainRow.InjectFrom(sv.savedHeader);
                SelectedMainRow.TblLkpSeason = sv.savedHeader.TblLkpSeason;
                SelectedMainRow.TblLkpSeason1 = SeasonList.FirstOrDefault(r => r.Iserial == SelectedMainRow.TblLkpSeason);
                //SelectedMainRow.DetailsList.Clear();
                foreach (var item in SelectedMainRow.DetailsList)
                {
                    var newRow = sv.Result.FirstOrDefault(d => d.ItemCode == item.ItemCode && d.TblColor == item.TblColor &&
                    d.TblTradeAgreementHeader == item.HeaderIserial && d.AccSize == item.AccSize);
                    if (newRow != null)
                    {
                        item.Iserial = newRow.Iserial;
                        item.HeaderIserial = newRow.TblTradeAgreementHeader;
                        item.TradeAgreementTransactionIserial = newRow.TblTradeAgreementHeader1.TblTradeAgreementTransaction.Value;
                    }
                }
                RaisePropertyChanged(nameof(IsHeaderSaved));
                MessageBox.Show(strings.Saved);
                Loading = true;
            };
            productionServiceClient.DeleteTradeAgreementHeaderCompleted += (d, s) =>
            {
                SelectedMainRow = new TradeAgreementTransaction();

                if (currentUi.DisplayName == "العربية")
                {
                    MessageBox.Show("Deleted");
                }
                else
                {
                    MessageBox.Show("Deleted");
                }
            };
            // ده هيحتاج تعديل
            LoadingDetail = new RelayCommand<DataGridRowEventArgs>((e) =>
            {
                if (SelectedMainRow.DetailsList.Count < PageSize)
                {
                    return;
                }
                if (SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && SelectedMainRow.DetailsList.Count < DetailFullCount
                        && !Loading)
                {
                    string size = null, vendor = null, color = null, ItemType = null, ItemCode = null;
                    string key = "AccSize";
                    if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                    {
                        size = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                    }
                    key = "Vendor";
                    if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                    {
                        vendor = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                    }

                    key = "Color";
                    if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                    {
                        color = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                    }
                    key = "ItemType";
                    if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                    {
                        ItemType = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                    }
                    key = "ItemCode";
                    if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                    {
                        ItemCode = Convert.ToString(valueObjecttemp.First(w => w.Key.StartsWith(key)).Value);
                    }
                    productionServiceClient.GetTblTradeAgreementDetailListFabricViewAsync(
                        SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial, ItemCode,
                        color, size, vendor, ItemType);
                }
            });
            // ده هيحتاج تعديل
            FilterCommand = new RelayCommand<FilterEvent>((e) =>
            {
                SelectedMainRow.DetailsList.Clear();

                valueObjecttemp.Clear();

                foreach (var f in e.FiltersPredicate)
                {
                    valueObjecttemp.Add(f.FilterColumnInfo.PropertyPath, f.FilterText);
                }

                string size = null, vendor = null, color = null, ItemType = null, ItemCode = null;
                string key = "AccSize";
                if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                {
                    size = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                }
                key = "Vendor";
                if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                {
                    vendor = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                }

                key = "Color";
                if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                {
                    color = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                }
                key = "ItemType";
                if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                {
                    ItemType = (string)valueObjecttemp.First(w => w.Key.StartsWith(key)).Value;
                }
                key = "ItemCode";
                if (valueObjecttemp.Any(w => w.Key.StartsWith(key)))
                {
                    ItemCode = Convert.ToString(valueObjecttemp.First(w => w.Key.StartsWith(key)).Value);
                }
                productionServiceClient.GetTblTradeAgreementDetailListFabricViewAsync(
                    0, PageSize, SelectedMainRow.Iserial, ItemCode, color, size, vendor, ItemType);
            });
            NewDetail = new RelayCommand<KeyEventArgs>((e) =>
            {
                if (e.Key == Key.Down)
                {
                    //var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                    //if (currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
                    //{
                    //    AddNewDetailRow(true);
                    //}
                }
            });
            DeleteDetail = new RelayCommand<KeyEventArgs>((e) =>
            {
                if (e.Key == Key.Delete)
                {
                    var DetailGrid = e.OriginalSource as OsGrid;
                    SelectedDetailRows.Clear();
                    foreach (var row in DetailGrid.SelectedItems)
                    {
                        SelectedDetailRows.Add(row as TradeAgreementDetailModel);
                    }
                    DeleteDetailRow();
                }
            });
            SaveDetail = new RelayCommand<DataGridRowEditEndedEventArgs>((e) => { SaveDetailRow(); });
            Client.GetAllSeasonsCompleted += (s, e) =>
            {
                SeasonList.Clear();
                foreach (var row in e.Result)
                {
                    TblLkpSeason temp = new TblLkpSeason();
                    temp.InjectFrom(row);
                    SeasonList.Add(temp);
                }
            };

            AddNewMainRow(false);
            GetComboData();
            GetMaindata();
        }

        #region Methods
        private void ValidateDetailRow(TradeAgreementDetailModel tradeAgreementDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                SelectedMainRow.DetailsList.Add(tradeAgreementDetail);
            }
            else
            {
                SelectedMainRow.DetailsList.Add(tradeAgreementDetail);
            }
            SelectedDetailRow = tradeAgreementDetail;
        }
        public bool ValidHeaderData()
        {
            if (SelectedMainRow.TblLkpSeason == 0)
            {
                MessageBox.Show(strings.ReqSeason);
                return false;
            }
            if (SelectedMainRow.ToDate >= SelectedMainRow.FromDate)
            {
                MessageBox.Show("From date must be earlier than to date.");
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.DetailsList.Any(td => string.IsNullOrWhiteSpace(td.Vendor)))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
        #endregion

        #region Operations



        
        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            productionServiceClient.GetTblTradeAgreementTransactionAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }
        public void DeleteMainRow()
        {
            productionServiceClient.DeleteTradeAgreementHeaderAsync(SelectedMainRow.Iserial);
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                SelectedMainRow = new TradeAgreementTransaction();
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                AddNewDetailRow(false);
            }
        }
        public void SaveMainRow()
        {
            Loading = true;
            var saveTemp = (TblTradeAgreementTransaction)new TblTradeAgreementTransaction().InjectFrom(SelectedMainRow);
            saveTemp.TblTradeAgreementHeaders = new ObservableCollection<TblTradeAgreementHeader>();
            var details = SelectedMainRow.DetailsList.Where(d => d.TblVendorPurchaseGroup != null &&
                d.ItemCode != null && d.CurrencyCode != null);
            foreach (var item in details.GroupBy(d => new { d.Vendor }))
            {
                var firstDetail = item.FirstOrDefault();
                var TradeAgreementHeader = new TblTradeAgreementHeader()
                {
                    FromDate = SelectedMainRow.FromDate,
                    ToDate = SelectedMainRow.ToDate,
                    Vendor = item.Key.Vendor,
                    TblTradeAgreementTransaction = saveTemp.Iserial,
                };
                if (firstDetail != null) TradeAgreementHeader.Iserial = firstDetail.HeaderIserial;

                TradeAgreementHeader.TblTradeAgreementDetails =
                    new ObservableCollection<TblTradeAgreementDetail>();
                GenericMapper.InjectFromObCollection(TradeAgreementHeader.TblTradeAgreementDetails,
                    details.Where(d => d.Vendor == item.Key.Vendor));

                saveTemp.TblTradeAgreementHeaders.Add(TradeAgreementHeader);
            }
            productionServiceClient.SaveTradeAgreementAsync(saveTemp);
        }
        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            productionServiceClient.GetTblTradeAgreementDetailListAsync(
                SelectedMainRow.DetailsList.Count(x => x.Iserial != 0),
                PageSize, SelectedMainRow.Iserial, DetailSortBy,
                DetailFilter, DetailValuesObjects);
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
                            productionServiceClient.DeleteTblTradeAgreementDetailAsync(row.Iserial);

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
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex > -1)// == (SelectedMainRow.DetailsList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }
                var newrow = new TradeAgreementDetailModel() { FromDate = SelectedMainRow.FromDate, ToDate = SelectedMainRow.ToDate };
                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
                newrow.TradeAgreementTransactionIserial = SelectedMainRow.Iserial;

                SelectedDetailRow = newrow;
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
                    var rowToSave = new TblTradeAgreementDetail();

                    rowToSave.InjectFrom(SelectedDetailRow);
                    rowToSave.TblTradeAgreementHeader1 = new TblTradeAgreementHeader()
                    {
                        Iserial = SelectedDetailRow.HeaderIserial,
                        TblTradeAgreementTransaction = SelectedMainRow.Iserial,
                        FromDate = SelectedMainRow.FromDate,
                        ToDate = SelectedMainRow.ToDate,
                        Vendor = SelectedDetailRow.Vendor
                    };
                    productionServiceClient.UpdateOrInsertTblTradeAgreementDetailAsync(rowToSave,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.Iserial);
                }
            }
        }
        internal void SaveDetailRows()
        {
            foreach (var variable in SelectedMainRow.DetailsList.Where(d => d.ItemCode != null))
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(variable, new ValidationContext(variable, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new TblTradeAgreementDetail();

                    rowToSave.InjectFrom(SelectedDetailRow);
                    rowToSave.TblTradeAgreementHeader1 = new TblTradeAgreementHeader()
                    {
                        TblTradeAgreementTransaction1 = new TblTradeAgreementTransaction()
                    };
                    rowToSave.TblTradeAgreementHeader1.TblTradeAgreementTransaction1.InjectFrom(SelectedMainRow);
                    productionServiceClient.UpdateOrInsertTblTradeAgreementDetailAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.Iserial);
                }
                else
                {
                    MessageBox.Show("Data IS Not Valid");
                    return;
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            calculationClient.GetGenericAsync(nameof(TblVendorPurchaseGroup), "%%", "%%", "%%", "Iserial", "ASC");
            Client.GetAllSeasonsAsync();
        }

        #endregion

        #region Properties

        private ObservableCollection<CRUDManagerService.GenericTable> _vendorPurchaseGroupList;
        public ObservableCollection<CRUDManagerService.GenericTable> VendorPurchaseGroupList
        {
            get { return _vendorPurchaseGroupList; }
            set { _vendorPurchaseGroupList = value; RaisePropertyChanged("VendorPurchaseGroupList"); }
        }

        private TradeAgreementTransaction _selectedMainRow;
        public TradeAgreementTransaction SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new TradeAgreementTransaction());
            }
            set
            {
                if ((ReferenceEquals(_selectedMainRow, value) != true))
                {
                    _selectedMainRow = value;
                    RaisePropertyChanged(nameof(IsHeaderSaved));
                    RaisePropertyChanged(nameof(SelectedMainRow));
                    GetDetailData();
                }
            }
        }

        private SortableCollectionView<TradeAgreementTransaction> _mainRowList;
        public SortableCollectionView<TradeAgreementTransaction> MainRowList
        {
            get
            {
                return _mainRowList ?? (_mainRowList = new SortableCollectionView<TradeAgreementTransaction>());
            }
            set
            {
                if ((ReferenceEquals(_mainRowList, value) != true))
                {
                    _mainRowList = value;
                    RaisePropertyChanged("MainRowList");
                }
            }
        }

        private ObservableCollection<CRUDManagerService.CURRENCY> _axCurrencyList;
        public ObservableCollection<CRUDManagerService.CURRENCY> AxCurrencyList
        {
            get { return _axCurrencyList; }
            set { _axCurrencyList = value; RaisePropertyChanged("AxCurrencyList"); }
        }

        private TradeAgreementDetailModel _selectedDetail;
        public TradeAgreementDetailModel SelectedDetailRow
        {
            get { return _selectedDetail; }
            set
            {
                if (_selectedDetail != value)
                {
                    _selectedDetail = value; RaisePropertyChanged(nameof(SelectedDetailRow));
                }
            }
        }

        private ObservableCollection<TradeAgreementDetailModel> _selectedDetailRows;
        public ObservableCollection<TradeAgreementDetailModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ??
                       (_selectedDetailRows = new ObservableCollection<TradeAgreementDetailModel>());
            }
            set
            {
                if (_selectedDetailRows != value)
                {
                    _selectedDetailRows = value; RaisePropertyChanged(nameof(SelectedDetailRows));
                }
            }
        }

        #region Combo

        ObservableCollection<TblLkpSeason> _seasonList = new ObservableCollection<TblLkpSeason>();
        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged(nameof(SeasonList)); }
        }

        #endregion

        public virtual bool IsHeaderSaved
        {
            get { return SelectedMainRow.Iserial > 0; }
        }

        #endregion

        #region Commands

        RelayCommand<DataGridRowEventArgs> loadingDetail;
        public RelayCommand<DataGridRowEventArgs> LoadingDetail
        {
            get { return loadingDetail; }
            set { loadingDetail = value; RaisePropertyChanged(nameof(LoadingDetail)); }
        }

        RelayCommand<FilterEvent> filterCommand;
        public RelayCommand<FilterEvent> FilterCommand
        {
            get { return filterCommand; }
            set { filterCommand = value; RaisePropertyChanged(nameof(FilterCommand)); }
        }

        RelayCommand<KeyEventArgs> newDetail;
        public RelayCommand<KeyEventArgs> NewDetail
        {
            get { return newDetail; }
            set { newDetail = value; RaisePropertyChanged(nameof(NewDetail)); }
        }

        RelayCommand<KeyEventArgs> deleteDetail;
        public RelayCommand<KeyEventArgs> DeleteDetail
        {
            get { return deleteDetail; }
            set { deleteDetail = value; RaisePropertyChanged(nameof(DeleteDetail)); }
        }

        RelayCommand<DataGridRowEditEndedEventArgs> saveDetail;
        public RelayCommand<DataGridRowEditEndedEventArgs> SaveDetail
        {
            get { return saveDetail; }
            set { saveDetail = value; RaisePropertyChanged(nameof(SaveDetail)); }
        }

        #endregion

        #region override
        public override void NewRecord()
        {
            //AddNewMainRow(false);
            AddNewDetailRow(true);
            base.NewRecord();
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return true;
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TradeAgreementTransaction> vm =
                new GenericSearchViewModel<TradeAgreementTransaction>() { Title = "Trade Agreement Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
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


        public override void ExportMethod()
        {
         
            SelectedMainRow.DetailsList.Clear();
            if (SortBy == null)
                SortBy = "it.Iserial";

            Export = true;
                
            if (!Loading)
            {
                string size = null, vendor = null, color = null, ItemType = null, ItemCode = null;
                productionServiceClient.GetTblTradeAgreementDetailListFabricViewAsync(
                    SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial, ItemCode,
                    color, size, vendor, ItemType);
            }
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header=strings.Season,
                        PropertyPath= string.Format("{0}.{1}", nameof(TradeAgreementTransaction.TblLkpSeason1), nameof(TblLkpSeason.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}", nameof(TradeAgreementTransaction.TblLkpSeason1), nameof(TblLkpSeason.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.FromDate,
                        PropertyPath=nameof(TradeAgreementTransaction.FromDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ToDate,
                        PropertyPath=nameof(TradeAgreementTransaction.ToDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.ItemID,
                        PropertyPath = nameof(TblLkpSeason.Code),
                    },
                };
        }
        public override void DeleteRecord()
        {
            var r = MessageBox.Show(
                "You are about to delete a TradeAgreement Order permanently!!\nPlease note that this action cannot be undone!",
                "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                DeleteMainRow();
                AddNewMainRow(false);
            }
            base.DeleteRecord();
        }
        public override bool CanDeleteRecord()
        {
            return SelectedMainRow.Iserial > 0;
        }
        public override void Cancel()
        {
            AddNewMainRow(false);
            base.Cancel();
        }
        public override void Print()
        {
            base.Print();

            var rVM = new GenericReportViewModel();

            rVM.GenerateReport("TradeAgreement", new ObservableCollection<string>());
        }
        #endregion
    }
}