using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblDepositViewModel : PropertiesViewModelBase
    {
        private int _tbluser;

        public int TblUser
        {
            get { return _tbluser; }
            set
            {
                _tbluser = value;
                RaisePropertyChanged("Tbluser");
            }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private CRUD_ManagerServiceClient Client = new CRUD_ManagerServiceClient();

        public TblDepositViewModel()
        {
            Client.BankDepositFromDateCompleted += (x, y) =>
            {
                Loading = false;
                FromDate = Convert.ToDateTime(y.Result);
                Enabled = y.Enabled;
            };

            _docdateField = DateTime.Now;
            FromDate = DateTime.Now;
        }

        private int _glserial;

        public int Glserial
        {
            get { return _glserial; }
            set { _glserial = value; RaisePropertyChanged("Glserial"); }
        }

        private int _amountField;
        private bool _inter;

        public bool inter
        {
            get { return _inter; }
            set
            {
                _inter = value; RaisePropertyChanged("inter");

                if (!inter)
                {
                    //Enabled = false;
                }
            }
        }

        private string _descriptionField;
        private DateTime _docdateField;

        private DateTime? _fromDateField;

        private string _iserialField;

        private int _tblStoreField;

        private DateTime? _toDateField;

        [Required(ErrorMessage = "ReqAmount")]
        public int Amount
        {
            get
            {
                return _amountField;
            }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
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

        public DateTime Docdate
        {
            get
            {
                return _docdateField;
            }
            set
            {
                if ((_docdateField.Equals(value) != true))
                {
                    _docdateField = value;
                    RaisePropertyChanged("Docdate");
                }
            }
        }

        public DateTime? FromDate
        {
            get
            {
                return _fromDateField;
            }
            set
            {
                if ((_fromDateField.Equals(value) != true))
                {
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public string Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((ReferenceEquals(_iserialField, value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [Required(ErrorMessage = "ReqStore")]
        public int TblStore
        {
            get
            {
                return _tblStoreField;
            }
            set
            {
                if ((_tblStoreField.Equals(value) != true))
                {
                    _tblStoreField = value;
                    RaisePropertyChanged("TblStore");
                }
            }
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow ?? (_storePerRow = new TblStore()); }
            set
            {
                _storePerRow = value;
                RaisePropertyChanged("StorePerRow");
                if (StorePerRow != null)
                {
                    TblStore = StorePerRow.iserial;

                    if (StorePerRow.code != null)
                    {
                        Client.BankDepositAmountCompleted += (x, b) =>
                        {
                            Amount = Convert.ToInt32(b.Result);
                        };

                        date();
                        if (StorePerRow.code != null)
                            Loading = true;
                        if (ToDate != null) Client.BankDepositAmountAsync((DateTime)FromDate, (DateTime)ToDate, StorePerRow.code,LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void date()
        {
            Loading = true;
            Client.BankDepositFromDateAsync(StorePerRow.code,LoggedUserInfo.DatabasEname);
        }

        [Required(ErrorMessage = "ReqDate")]
        public DateTime? ToDate
        {
            get
            {
                return _toDateField;
            }
            set
            {
                if ((_toDateField.Equals(value) != true))
                {
                    if (value < FromDate)
                    {
                        value = FromDate;
                    }
                    _toDateField = value;
                    RaisePropertyChanged("ToDate");
                    if (StorePerRow.code != null)
                        Loading = true;
                    if (ToDate != null) Client.BankDepositAmountAsync((DateTime)FromDate, (DateTime)ToDate, StorePerRow.code,LoggedUserInfo.DatabasEname);
                }
            }
        }
    }

    public class DepositViewModel : ViewModelBase
    {
        public DepositViewModel()
        {
            GetItemPermissions("BankDepositForm");

            TransactionHeader = new TblDepositViewModel();
            if (LoggedUserInfo.Store != null) TransactionHeader.StorePerRow = new TblStore().InjectFrom(LoggedUserInfo.Store) as TblStore;
            if (LoggedUserInfo.Store != null) TransactionHeader.TblUser = LoggedUserInfo.Iserial;
            MainRowList = new SortableCollectionView<TblBankDeposit>();
            StoreList = new SortableCollectionView<TblStore>();

            Client.SearchforsStoreNameCompleted += (m, k) =>
            {
                Loading = false;
                StoreList = k.Result;

                //OnSupplierCompleted();
            };

            Client.BankdigitCompleted += (w, n) =>
            {
                Loading = false;
                TransactionHeader.Iserial = n.Result;
            };

            //   Client.SearchBysStorEnameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores));
            Client.SearchTblBankDepositCompleted += (y, v) =>
            {
                foreach (var row in v.Result)
                {
                    Loading = false;
                    var newrow = new TblBankDeposit();
                    newrow.InjectFrom(row);

                    MainRowList.Add(newrow);
                }
            };

            Client.SearchByStoreCodeCompleted += (x, y) =>
            {
                Loading = false;
                TransactionHeader.StorePerRow = y.Result;
            };

            Client.UpdateOrInsertTblBankDepositCompleted += (s, x) =>
            {
                if (x.Error == null)
                {
                    TransactionHeader.InjectFrom(x.Result);
                    MessageBox.Show("Saved Successfully");
                    Print();
                }
                else
                {
                    MessageBox.Show(x.Error.Message);
                }
                //   Loading = false;

                //TransactionHeader.InjectFrom(x.Result);
            };

            Client.DeleteTblBankDepositCompleted += (w, k) =>
            {
                Loading = false;
                TransactionHeader = new TblDepositViewModel();
            };
        }

        protected virtual void OnSupplierCompleted()
        {
            var handler = SupplierItemCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        //internal void SearchForStorEname(string StorEname,string code)
        //{
        //    Client.SearchBysStorEnameAsync(StorEname, new ObservableCollection<int>(LoggedUserInfo.AllowedStores));

        //}

        //public void date()
        //{
        //    Client.DateAsync(TransactionHeader.StorePerRow.code);
        //}

        //public void Amount()
        //{
        //    Client.AmountAsync(TransactionHeader.FromDate,TransactionHeader.ToDate,TransactionHeader.StorePerRow.code);
        //}

        public void Bank()
        {
            if (TransactionHeader.Glserial == 0)
            {
                if (TransactionHeader.StorePerRow != null && TransactionHeader.StorePerRow.iserial != 0)
                {
                    Loading = true;
                    Client.BankdigitAsync(TransactionHeader.StorePerRow.iserial, false,LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SearchByStoreCode()
        {
            Loading = true;
            Client.SearchByStoreCodeAsync(TransactionHeader.StorePerRow.code, new ObservableCollection<int>(LoggedUserInfo.AllowedStores),LoggedUserInfo.DatabasEname);
        }

        public void GetMaindata()
        {
            Loading = true;
            Client.SearchTblBankDepositAsync(serial, StorEname, ddate, Code, new ObservableCollection<int>(LoggedUserInfo.AllowedStores),LoggedUserInfo.DatabasEname);
        }

        public void UpdateAndInsert()
        {
            var valiationCollection = new List<ValidationResult>();
            bool isvalid = Validator.TryValidateObject(TransactionHeader, new ValidationContext(TransactionHeader, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblBankDeposit();
                data.InjectFrom(TransactionHeader);

                bool save = TransactionHeader.Glserial == 0;

                if (save)
                {
                    if (AllowAdd)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblBankDepositAsync(data,LoggedUserInfo.DatabasEname);
                    }

                    else
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                    }
                }
                else
                {
                    if (AllowUpdate)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblBankDepositAsync(data,LoggedUserInfo.DatabasEname);
                    }

                    else
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                    }
                }
            }
        }

        public void Delete()
        {
            var data = new TblBankDeposit();
            data.InjectFrom(TransactionHeader);

            if (AllowDelete)
            {
                Loading = true;
                Client.DeleteTblBankDepositAsync(data,LoggedUserInfo.DatabasEname);
            }
            else
            {
                MessageBox.Show("You are Not Allowed to Delete");
            }
        }

        internal void SearchForStorEname()
        {
            if (LoggedUserInfo.AllowedStores != null)
            {
                Loading = true;
                Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, StorEname, Code,LoggedUserInfo.DatabasEname);
            }
        }

        private void Print()
        {
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            if (LoggedUserInfo.DatabasEname.ToLower() == "ccnew")
            {
                if (currentUi.DisplayName != "العربية")
                {
                    var myUri = new Uri(HtmlPage.Document.DocumentUri, string.Format("report.aspx?Iserial=" + TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri, "_Blank");
                }

                else
                {
                    var myUri2 = new Uri(HtmlPage.Document.DocumentUri, string.Format("report2.aspx?Iserial=" + TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri2, "_Blank");
                }
            }
            else
            {
                if (currentUi.DisplayName != "العربية")
                {
                    var myUri3 = new Uri(HtmlPage.Document.DocumentUri, string.Format("swreport.aspx?Iserial=" + TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri3, "_Blank");
                }

                else
                {
                    var myUri4 = new Uri(HtmlPage.Document.DocumentUri, string.Format("Bankdepositarabicsw.aspx?Iserial=" + TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri4, "_Blank");
                }
            }
        }

        private string _serial;

        public string serial
        {
            get { return _serial; }
            set
            {
                _serial = value;

                RaisePropertyChanged("serial");
            }
        }

        private DateTime? _ddate;

        public DateTime? ddate
        {
            get { return _ddate; }
            set
            {
                _ddate = value;

                RaisePropertyChanged("ddate");
            }
        }

        private string _storEname;

        public string StorEname
        {
            get { return _storEname; }
            set
            {
                _storEname = value;

                RaisePropertyChanged("StorEname");
            }
        }

        private string _code;

        public new string Code
        {
            get { return _code; }
            set
            {
                _code = value;

                RaisePropertyChanged("Code");
            }
        }

        private ObservableCollection<TblStore> _storeList;

        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set
            {
                _storeList = value;
                RaisePropertyChanged("StoreList");
            }
        }

        public event EventHandler SupplierItemCompleted;

        private SortableCollectionView<TblBankDeposit> _mainRowList;

        public SortableCollectionView<TblBankDeposit> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblBankDeposit _selectedMainRow;

        public TblBankDeposit SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblDepositViewModel _transactionHeader;

        public TblDepositViewModel TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                _transactionHeader = value;
                RaisePropertyChanged("TransactionHeader");
            }
        }
    }
}