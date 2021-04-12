using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using System.Collections.Specialized;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.ViewModel.OGViewModels
{
    #region Models

    public class RouteCardInvoiceHeaderViewModel : PropertiesViewModelBase
    {
        private GenericTable _currencyPerRow;

        public GenericTable CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (CurrencyPerRow != null) TblCurrency = CurrencyPerRow.Iserial;
            }
        }
        private int? _tblCurrencyField;

        public int? TblCurrency
        {
            get
            {
                return _tblCurrencyField;
            }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }
        private bool _visPosted;

        public bool VisPosted
        {
            get { return _visPosted; }
            set { _visPosted = value; RaisePropertyChanged("VisPosted"); }
        }
        private ObservableCollection<TblMarkupTranProdViewModel> _markUpTransList;

        public ObservableCollection<TblMarkupTranProdViewModel> MarkUpTransList
        {
            get { return _markUpTransList ?? (_markUpTransList = new ObservableCollection<TblMarkupTranProdViewModel>()); }
            set
            {
                _markUpTransList = value;
                RaisePropertyChanged("MarkUpTransList");
            }
        }

        private int _miscValueType;

        public int MiscValueType
        {
            get { return _miscValueType; }
            set { _miscValueType = value; RaisePropertyChanged("MiscValueType"); }
        }

        private double? _miscValueField;
        public double? MiscValue
        {
            get
            {
                return _miscValueField;
            }
            set
            {
                if ((_miscValueField.Equals(value) != true))
                {
                    _miscValueField = value;
                    RaisePropertyChanged("MiscValue");
                }
            }
        }

        private Vendor _vendor;

        public Vendor VendorPerRow
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                if (value != null)
                {
                    Vendor = value.vendor_code;
                    RaisePropertyChanged("VendorPerRow");
                }
            }
        }

        public RouteCardInvoiceHeaderViewModel()
        {
            DocDate = DateTime.Now;
        }

        private DateTime? _docDateField;

        private string _vendorField;

        private int _iserialField;

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
        public DateTime? DocDate
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

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        private string _SupplierInv;

        public string SupplierInv
        {
            get { return _SupplierInv; }
            set { _SupplierInv = value; RaisePropertyChanged("SupplierInv"); }
        }



        private int _status;

        public int Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private int _tblUser;

        public int TblUser
        {
            get { return _tblUser; }
            set { _tblUser = value; RaisePropertyChanged("TblUser"); }
        }

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("Code"); }
        }


        private int _transId;

        // ReSharper disable once InconsistentNaming
        public int TransID
        {
            get { return _transId; }
            set { _transId = value; RaisePropertyChanged("TransID"); }
        }
    }

    public class RouteCardInvoiceDetailViewModel : PropertiesViewModelBase
    {
        public RouteCardInvoiceDetailViewModel()
        {
            Selected = true;
        }


        private GenericTable _RouteGroupPerRow;

        public GenericTable RouteGroupPerRow
        {
            get { return _RouteGroupPerRow; }
            set { _RouteGroupPerRow = value; RaisePropertyChanged("RouteGroupPerRow"); }
        }


        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; RaisePropertyChanged("Selected"); }
        }

        private double? _costField;

        private int _iserialField;

        private int _qtyField;

        private int? _routeCardHeaderField;

        private int? _routeCardInvoiceHeaderField;

        private int _tblColorField;

        private TblColor _tblColor1Field;

        private int _tblSalesOrderField;

        private int _transIdField;

        public double? Cost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
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

        [ReadOnly(true)]
        public int Qty
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

        [ReadOnly(true)]
        public int? RouteCardHeader
        {
            get
            {
                return _routeCardHeaderField;
            }
            set
            {
                if ((_routeCardHeaderField.Equals(value) != true))
                {
                    _routeCardHeaderField = value;
                    RaisePropertyChanged("RouteCardHeader");
                }
            }
        }

        [ReadOnly(true)]
        public int? RouteCardInvoiceHeader
        {
            get
            {
                return _routeCardInvoiceHeaderField;
            }
            set
            {
                if ((_routeCardInvoiceHeaderField.Equals(value) != true))
                {
                    _routeCardInvoiceHeaderField = value;
                    RaisePropertyChanged("RouteCardInvoiceHeader");
                }
            }
        }

        [ReadOnly(true)]
        public int TblColor
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

        [ReadOnly(true)]
        public TblColor ColorPerRow
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
                    RaisePropertyChanged("ColorPerRow");
                }
            }
        }

        [ReadOnly(true)]
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
        private TblSalesOrder _salesOrderPerRow;
        [ReadOnly(true)]
        public TblSalesOrder SalesOrderPerRow
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

        [ReadOnly(true)]
        public int TransID
        {
            get
            {
                return _transIdField;
            }
            set
            {
                _transIdField = value;
                RaisePropertyChanged("TransID");
            }
        }

        string _SupplierInv;
        [ReadOnly(true)]
        public string SupplierInv
        {
            get
            {
                return _SupplierInv;
            }
            set
            {
                _SupplierInv = value;
                RaisePropertyChanged("SupplierInv");
            }
        }
    }

    #endregion Models

    public class RouteCardInvoiceViewModel : ViewModelStructuredBase
    {
        #region Prop


        private DateTime? _fromDate;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? FromDate
        {
            get
            {
                return _fromDate ?? (_fromDate = new DateTime(DateTime.Now.Year, 1, 1));
            }
            set
            {
                if ((_fromDate.Equals(value) != true))
                {
                    _fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        private DateTime? _toDate;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? ToDate
        {
            get
            {
                return _toDate ?? (_toDate = DateTime.Now);
            }
            set
            {
                if ((_toDate.Equals(value) != true))
                {
                    _toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        private TblSalesOrder _salesOrderPerRow;

        public TblSalesOrder SalesOrderPerRow
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

        private int _salesOrderField;
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


        private string _Referance;

        public string Referance
        {
            get { return _Referance; }
            set { _Referance = value; RaisePropertyChanged("Referance"); }
        }

        private int _transId;

        // ReSharper disable once InconsistentNaming
        public int TransID
        {
            get { return _transId; }
            set { _transId = value; RaisePropertyChanged("TransID"); }
        }

        private string _supplierInv;

        public string SupplierInv
        {
            get { return _supplierInv; }
            set { _supplierInv = value; RaisePropertyChanged("SupplierInv"); }
        }

        private TblMarkupTranProdViewModel _selectedMarkup;

        public TblMarkupTranProdViewModel SelectedMarkupRow
        {
            get { return _selectedMarkup; }
            set
            {
                _selectedMarkup = value;
                RaisePropertyChanged("SelectedMarkupRow");
            }
        }

        private ObservableCollection<GenericTable> _miscValueType;

        public ObservableCollection<GenericTable> MiscValueTypeList
        {
            get { return _miscValueType; }
            set { _miscValueType = value; RaisePropertyChanged("MiscValueTypeList"); }
        }

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<TblMarkupProd> _markupList;

        public ObservableCollection<TblMarkupProd> MarkupList
        {
            get { return _markupList; }
            set { _markupList = value; RaisePropertyChanged("MarkupList"); }
        }
        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<TblMarkupTranProdViewModel> _SelectedMarkupRows;

        public ObservableCollection<TblMarkupTranProdViewModel> SelectedMarkupRows
        {
            get { return _SelectedMarkupRows ?? (_SelectedMarkupRows = new ObservableCollection<TblMarkupTranProdViewModel>()); }
            set
            {
                _SelectedMarkupRows = value;
                RaisePropertyChanged("SelectedMarkupRows");
            }
        }

        private RouteCardInvoiceHeaderViewModel _SelectedMainRow;

        public RouteCardInvoiceHeaderViewModel SelectedMainRow
        {
            get
            {
                return _SelectedMainRow;
            }
            set
            {
                if ((ReferenceEquals(_SelectedMainRow, value) != true))
                {
                    _SelectedMainRow = value;
                    RaisePropertyChanged("SelectedMainRow");
                }
            }
        }

        private ObservableCollection<RouteCardInvoiceHeaderViewModel> _MainRowList;

        public ObservableCollection<RouteCardInvoiceHeaderViewModel> MainRowList
        {
            get
            {
                return _MainRowList ?? (_MainRowList = new ObservableCollection<RouteCardInvoiceHeaderViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_MainRowList, value) != true))
                {
                    _MainRowList = value;
                    RaisePropertyChanged("MainRowList");
                }
            }
        }

        private ObservableCollection<RouteCardInvoiceDetailViewModel> _transactionDetails;

        public ObservableCollection<RouteCardInvoiceDetailViewModel> TransactionDetails
        {
            get
            {
                return _transactionDetails ?? (_transactionDetails = new ObservableCollection<RouteCardInvoiceDetailViewModel>());
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

        public bool OpenMisc { get; private set; }

        #endregion Prop

        //public event EventHandler SubmitSearchAction;

        //public event EventHandler PurchaseTransactionsCompleted;

        //protected virtual void OnPurchaseTransactionsPopulatingCompleted()
        //{
        //    var handler = PurchaseTransactionsCompleted;
        //    if (handler != null) handler(this, EventArgs.Empty);
        //}

        public RouteCardService.RouteCardServiceClient RouteCardService = new RouteCardService.RouteCardServiceClient();

        public PurchasePlanService.PurchasePlanClient PurchasePlanClient = new PurchasePlanService.PurchasePlanClient();


        public RouteCardInvoiceViewModel() : base(PermissionItemName.RouteCardInvoice)
        {
            GetItemPermissions(PermissionItemName.RouteCardInvoice.ToString());
            RouteCardService.PostRouteCardInvoiceCompleted += (s, sv) =>
            {
                if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);
                SelectedMainRow.VisPosted = false;
                MessageBox.Show("Posted Completed");
            };
            RouteCardService.DeleteRouteCardInvoiceHeaderCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                }
                var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
            };
            MiscValueTypeList = new ObservableCollection<GenericTable>
                {
                    new GenericTable {Iserial = 0, Code = "%", Ename = "%", Aname = "%"},
                    new GenericTable {Iserial = 1, Code = "Value", Ename = "Value", Aname = "Value"}
                };

            var currencyClient = new GlService.GlServiceClient();
            currencyClient.GetGenericCompleted += (s, sv) =>
            {
                CurrencyList = new ObservableCollection<CRUDManagerService.GenericTable>();
                foreach (var item in sv.Result)
                {
                    CurrencyList.Add(new CRUDManagerService.GenericTable().InjectFrom(item) as CRUDManagerService.GenericTable);
                }
            };
            currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
            

            var journalAccountTypeClient = new GlService.GlServiceClient();
            journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
            {
                JournalAccountTypeList = sv.Result;
            };
            journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);



            PurchasePlanClient.GetTblMarkupProdAsync(0, int.MaxValue, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);

            PurchasePlanClient.GetTblMarkupProdCompleted += (s, sv) =>
            {
                MarkupList = new ObservableCollection<CRUDManagerService.TblMarkupProd>();
                foreach (var item in sv.Result)
                {
                    MarkupList.Add(new TblMarkupProd().InjectFrom(item) as CRUDManagerService.TblMarkupProd);
                }
            };

      

            SelectedMainRow = new RouteCardInvoiceHeaderViewModel { DocDate = DateTime.Now };

            RouteCardService.UpdateOrInsertRouteCardInvoiceHeaderCompleted += (s, sv) =>
            {

                if (sv.Error != null)
                {
                    MessageBox.Show(sv.Error.Message);
                }
                try
                {
                    SelectedMainRow.InjectFrom(sv.Result);

                    if (SelectedMainRow.Status == 0)
                    {
                        SelectedMainRow.VisPosted = true;
                    }
                    else
                    {
                        SelectedMainRow.VisPosted = false;
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
                Loading = false;
                if (OpenMisc)
                {
                    new MarkupTransProdChildWindow(this).Show();
                    OpenMisc = false;
                }

            };

            PurchasePlanClient.UpdateOrInsertRouteCardInvoiceMarkupTransProdsCompleted += (s, x) =>
            {
                var markup = new TblMarkupProd();
                try
                {
                    var row = SelectedMainRow.MarkUpTransList.ElementAt(x.outindex);
                    if (row != null)
                    {
                        markup = row.TblMarkupProd1;
                    }
                    if (x.Result.Iserial == -1)
                    {
                        MessageBox.Show("This Markup Is Not Linked To Account");
                        row.TblMarkupProd1 = null;
                    }
                    else
                    {
                        SelectedMainRow.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);
                        if (row != null) row.TblMarkupProd1 = markup;
                    }

                }
                catch (Exception)
                {
                }

                Loading = false;
            };

            RouteCardService.GetRouteCardInvoiceHeaderCompleted += (s, sv) =>
            {
                Loading = false;
                MainRowList.Clear();
                foreach (var variable in sv.Result.ToList())
                {

                    var newrow = new RouteCardInvoiceHeaderViewModel();
                    newrow.InjectFrom(variable);
                    newrow.CurrencyPerRow = new GenericTable();
                    newrow.CurrencyPerRow.InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == variable.TblCurrency));
                    newrow.TblCurrency = variable.TblCurrency;

                    var vendortemp = new Vendor().InjectFrom(sv.Vendors.FirstOrDefault(w => w.vendor_code == newrow.Vendor)) as Vendor;
                    newrow.VendorPerRow = vendortemp;
                    if (newrow.Status == 0)
                    {
                        newrow.VisPosted = true;
                    }
                    MainRowList.Add(newrow);
                }
            };

            RouteCardService.GetRouteCardInvoiceDetailCompleted += (s, sv) =>
            {
                foreach (var variable in sv.Result.ToList())
                {
                    var newrow = new RouteCardInvoiceDetailViewModel();
                    newrow.InjectFrom(variable);
                    newrow.SalesOrderPerRow = new TblSalesOrder();
                    newrow.ColorPerRow = new TblColor();
                    newrow.SalesOrderPerRow.SalesOrderCode = variable.TblSalesOrder1.SalesOrderCode;
                    newrow.ColorPerRow.Code = variable.TblColor1.Code;
                    newrow.RouteGroupPerRow = new GenericTable();
                    newrow.RouteGroupPerRow.InjectFrom(variable.RouteCardHeader1.TblRouteGroup);

                    TransactionDetails.Add(newrow);
                }
            };
            RouteCardService.SearchRouteCardInvoiceCompleted += (s, sv) =>
            {

                foreach (var variable in sv.Result.ToList())
                {
                    var newrow = new RouteCardInvoiceDetailViewModel();
                    newrow.InjectFrom(variable);
                    newrow.SalesOrderPerRow = new TblSalesOrder();
                    newrow.ColorPerRow = new TblColor();
                    newrow.SalesOrderPerRow.SalesOrderCode = variable.SalesOrderCode;
                    newrow.SalesOrderPerRow.Iserial = variable.TblSalesOrder;
                    newrow.TblSalesOrder = variable.TblSalesOrder;

                    newrow.ColorPerRow.Code = variable.ColorCode;
                    newrow.RouteGroupPerRow = new GenericTable();
                    newrow.RouteGroupPerRow.Ename = variable.RouteGroupString;

                    TransactionDetails.Add(newrow);
                }
            };
            PurchasePlanClient.GetTblRouteCardInvoiceMarkupTransProdCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblMarkupTranProdViewModel();
                    newrow.InjectFrom(row);
                    newrow.CurrencyPerRow = new GenericTable();
                    newrow.CurrencyPerRow.InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == row.TblCurrency));
                    newrow.TblMarkupProd1 = new TblMarkupProd();
                    newrow.TblMarkupProd1.InjectFrom(MarkupList.FirstOrDefault(w => w.Iserial == row.TblMarkupProd));
                    newrow.TblMarkupProd = row.TblMarkupProd;
                    newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == newrow.TblJournalAccountType);
                    newrow.EntityPerRow = new GlService.Entity().InjectFrom(sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType)) as GlService.Entity;
                    newrow.TblJournalAccountType = row.TblJournalAccountType;
                    newrow.EntityAccount = row.EntityAccount;
                    SelectedMainRow.MarkUpTransList.Add(newrow);
                }
                Loading = false;
                if (SelectedMainRow.MarkUpTransList.Count == 0)
                {
                    AddNewMarkUpRow(false, true);
                }
            };
        }

        public void GetMarkUpdata(bool header)
        {
            SelectedMainRow.MarkUpTransList.Clear();
            PurchasePlanClient.GetTblRouteCardInvoiceMarkupTransProdAsync(0, SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
            Loading = true;
        }


        public void DeleteMainRow()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                RouteCardService.DeleteRouteCardInvoiceHeaderAsync((RouteCardService.RouteCardInvoiceHeader)new RouteCardService.RouteCardInvoiceHeader().InjectFrom(SelectedMainRow) as RouteCardService.RouteCardInvoiceHeader);
            }
        }

        public void DeleteMarkupRow(bool header)
        {

            if (SelectedMarkupRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMarkupRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }

                            PurchasePlanClient.DeleteRouteCardInvoiceMarkupTransProdsAsync((PurchasePlanService.RouteCardInvoiceMarkupTransProd)new PurchasePlanService.RouteCardInvoiceMarkupTransProd().InjectFrom(row), SelectedMainRow.MarkUpTransList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.MarkUpTransList.Remove(row);
                            if (!SelectedMainRow.MarkUpTransList.Any())
                            {
                                AddNewMarkUpRow(false, header);
                            }
                        }
                    }
                }
            }
        }

        public void SaveMarkupRow()
        {
            if (SelectedMarkupRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                    new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMarkupRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new PurchasePlanService.RouteCardInvoiceMarkupTransProd();
                    saveRow.InjectFrom(SelectedMarkupRow);
                    saveRow.RouteCardInvoiceHeader = SelectedMainRow.Iserial;

                    if (!Loading)
                    {
                        Loading = true;

                        PurchasePlanClient.UpdateOrInsertRouteCardInvoiceMarkupTransProdsAsync(saveRow, save, SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
                      LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveMarkupRowOldRow(TblMarkupTranProdViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new PurchasePlanService.RouteCardInvoiceMarkupTransProd();
                    saveRow.InjectFrom(oldRow);
                    saveRow.RouteCardInvoiceHeader = SelectedMainRow.Iserial;
                    if (!Loading)
                    {
                        Loading = true;

                        PurchasePlanClient.UpdateOrInsertRouteCardInvoiceMarkupTransProdsAsync(saveRow, save,
                                SelectedMainRow.MarkUpTransList.IndexOf(oldRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void AddNewMarkUpRow(bool checkLastRow, bool header)
        {
            var currentRowIndex = (SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                    new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblMarkupTranProdViewModel
            {
                Type = 0,
                RouteCardInvoiceHeader = SelectedMainRow.Iserial,
                MiscValueType = 1
            };

            SelectedMainRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
            SelectedMarkupRow = newrow;
        }

        public void GetOrderInfo()
        {
            if (FromDate != null)
                if (ToDate != null)
                    RouteCardService.SearchRouteCardInvoiceAsync(SelectedMainRow.Vendor, (DateTime)FromDate, (DateTime)ToDate, TransID, TblSalesOrder, SupplierInv);
        }




        public override void Search()
        {
            MainRowList.Clear();
            SearchHeader();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<RouteCardInvoiceHeaderViewModel> vm =
                new GenericSearchViewModel<RouteCardInvoiceHeaderViewModel>() { Title = "Route Card Invoice" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                GetDetailData();
                // RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) =>
            {
                DetailFilter = vm.Filter;
               DetailValuesObjects = vm.ValuesObjects;
                SearchHeader();
            },
            (o) =>
            {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header="Code",
                        PropertyPath=nameof(RouteCardInvoiceHeader.Code),
                    },
                     new SearchColumnModel()
                    {
                        Header=strings.Vendor,
                        PropertyPath= string.Format("{0}",nameof(RouteCardInvoiceHeader.Vendor)),
                        FilterPropertyPath=string.Format("{0}",nameof(RouteCardInvoiceHeader.Vendor)),
                    },
                    new SearchColumnModel()
                    {
                       Header=strings.Vendor,
                              PropertyPath= string.Format("{0}.{1}", nameof(RouteCardInvoiceHeaderViewModel.VendorPerRow),nameof(Vendor.vendor_ename)),
                        FilterPropertyPath=string.Format("{0}.{1}", nameof(RouteCardInvoiceHeaderViewModel.VendorPerRow),nameof(Vendor.vendor_ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.DocDate,
                        PropertyPath=nameof(RouteCardInvoiceHeaderViewModel.DocDate),
                         StringFormat="{0:dd/MM/yyyy}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Description,
                        PropertyPath=nameof(RouteCardInvoiceHeaderViewModel.Description),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.SupplierInvoice,
                        PropertyPath=nameof(RouteCardInvoiceHeaderViewModel.SupplierInv),
                    },
                };
        }


        public void SearchHeader()
        {
            RouteCardService.GetRouteCardInvoiceHeaderAsync(MainRowList.Count, PageSize, "it.Iserial desc", DetailFilter, DetailValuesObjects);
        }

        public void SaveOrder(bool openMisc = false)
        {
            OpenMisc = openMisc;
            var valiationCollectionHeader = new List<ValidationResult>();
            var isvalidHeader = Validator.TryValidateObject(SelectedMainRow,
                new ValidationContext(SelectedMainRow, null, null), valiationCollectionHeader, true);

            var details = new ObservableCollection<RouteCardService.RouteCardInvoiceDetail>();
            var isvalid = false;
            foreach (var item in TransactionDetails)
            {
                var valiationCollection = new List<ValidationResult>(); isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);
                if (isvalid == false)
                {
                    return;
                }

                details.Add((RouteCardService.RouteCardInvoiceDetail)new RouteCardService.RouteCardInvoiceDetail().InjectFrom(item));
            }
            var newrow = new RouteCardService.RouteCardInvoiceHeader();
            newrow.InjectFrom(SelectedMainRow);

            newrow.RouteCardInvoiceDetails = details;
            if (isvalid && isvalidHeader)
            {
                if (Loading == false)
                {
                    Loading = true;
                    RouteCardService.UpdateOrInsertRouteCardInvoiceHeaderAsync(newrow, true, 0);
                }
            }
            else
            {
                MessageBox.Show("Data Is NOt Valid");
            }
        }

        public void GetDetailData()
        {
            RouteCardService.GetRouteCardInvoiceDetailAsync(0, int.MaxValue, SelectedMainRow.Iserial, "it.Iserial", null, null);
        }

        public void FabricInspectionReport()
        {
            var reportName = "RouteCardInvoice";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "RouteCardInvoice"; }

            var para = new ObservableCollection<string> { SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        public void Post()
        {
            var saveRow = new RouteCardService.RouteCardInvoiceHeader();
            saveRow.InjectFrom(SelectedMainRow);
            RouteCardService.PostRouteCardInvoiceAsync(saveRow, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
        }
    }
}