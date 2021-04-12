using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TblBrandStoreTargetHeaderViewModel : PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblBrandStoreTargetHeaderViewModel()
        {
            MainRowList = new SortableCollectionView<TblBrandStoreTargetDetailViewModel>();

            MainRowList.CollectionChanged += MainRowList_CollectionChanged;
            _client.GetTblBrandStoreTargetHeaderCompleted += (s, sv) =>
            {
                Glserial = 0;
                MainRowList.Clear();
                if (sv.Result != null)
                {
                
                foreach (var tblBrandStoreTargetDetail in sv.Result.TblBrandStoreTargetDetails.OrderBy(x => x.TblStore1.code))
                    {
                        Glserial = tblBrandStoreTargetDetail.Glserial;
                        var row =
                            new TblBrandStoreTargetDetailViewModel().InjectFrom(tblBrandStoreTargetDetail) as
                                TblBrandStoreTargetDetailViewModel;
                        row.StorePerRow = new StoresForBrand
                        {
                            Iserial = tblBrandStoreTargetDetail.TblStore1.iserial,
                            Code = tblBrandStoreTargetDetail.TblStore1.code,
                            Ename = tblBrandStoreTargetDetail.TblStore1.ENAME,
                            Aname = tblBrandStoreTargetDetail.TblStore1.aname
                        };

                        MainRowList.Add(row);
                    }
                    Glserial = sv.Result.Glserial;
                }
                foreach (var variable in sv.Stores.OrderBy(x => x.Code))
                {
                    if (MainRowList.FirstOrDefault(x => x.Tblstore == variable.Iserial) == null)
                    {
                        MainRowList.Add(new TblBrandStoreTargetDetailViewModel
                        {
                            Glserial = Glserial,
                            StorePerRow = variable,
                            Qty = 1
                        });
                    }
                }

                TotalAmount = MainRowList.Sum(x => x.Amount);

                TotalQty = MainRowList.Sum(x => x.Qty);
            };

        }

        private void MainRowList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblBrandStoreTargetDetailViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblBrandStoreTargetDetailViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "Amount")
            {
                TotalAmount = MainRowList.Sum(x => x.Amount);
            }
            if (e.PropertyName == "Qty")
            {
                TotalQty = MainRowList.Sum(x => x.Qty);
            }
        }

        private decimal? _totalamount;

        public decimal? TotalAmount
        {
            get { return _totalamount; }
            set { _totalamount = value; RaisePropertyChanged("TotalAmount"); }
        }

        private float? _totalQty;

        public float? TotalQty
        {
            get { return _totalQty; }
            set { _totalQty = value; RaisePropertyChanged("TotalQty"); }
        }

        private int? _monthField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMonth")]
        public int? Month
        {
            get
            {
                return _monthField;
            }
            set
            {
                if ((_monthField.Equals(value) != true))
                {
                    _monthField = value;
                    RaisePropertyChanged("Month");
                    GetBrandStoreHeader();
                }
            }
        }

        private int _brandAmount;


        public int BrandAmount
        {
            get
            {
                return _brandAmount;
            }
            set
            {
                if ((_brandAmount.Equals(value) != true))
                {
                    _brandAmount = value;
                    RaisePropertyChanged("BrandAmount");
                    
                }
            }
        }

        private void GetBrandStoreHeader()
        {
            if (Month != null)
                if (TblItemDownLoadDef != null)
                    if (Year != null)
                        if (ItemDownloadDefPerRow.Code != null)
                            _client.GetTblBrandStoreTargetHeaderAsync((int)Month, (int)TblItemDownLoadDef, (int)Year, ItemDownloadDefPerRow.Code,LoggedUserInfo.DatabasEname);
        }

        private int _glserialField;

        private int? _tblItemDownLoadDefField;

        private TblItemDownLoadDef _tblItemDownLoadDef1Field;
        private SortableCollectionView<TblBrandStoreTargetDetailViewModel> _mainRowList;

        public SortableCollectionView<TblBrandStoreTargetDetailViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private int? _yearField;
        private bool _enabled;

        public int Glserial
        {
            get
            {
                return _glserialField;
            }
            set
            {
                if ((_glserialField.Equals(value) != true))
                {
                    _glserialField = value;
                    RaisePropertyChanged("Glserial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public int? TblItemDownLoadDef
        {
            get
            {
                return _tblItemDownLoadDefField;
            }
            set
            {
                if ((_tblItemDownLoadDefField.Equals(value) != true))
                {
                    _tblItemDownLoadDefField = value;
                    RaisePropertyChanged("TblItemDownLoadDef");
                }
            }
        }

        public TblItemDownLoadDef ItemDownloadDefPerRow
        {
            get
            {
                return _tblItemDownLoadDef1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblItemDownLoadDef1Field, value) != true))
                {
                    _tblItemDownLoadDef1Field = value;
                    RaisePropertyChanged("ItemDownloadDefPerRow");
                    if (ItemDownloadDefPerRow != null)
                    {
                        TblItemDownLoadDef = ItemDownloadDefPerRow.iserial;
                        GetBrandStoreHeader();
                    }
                }
            }
        }

        public int? Year
        {
            get
            {
                return _yearField;
            }
            set
            {
                if ((_yearField.Equals(value) != true))
                {
                    _yearField = value;
                    RaisePropertyChanged("Year");
                    GetBrandStoreHeader();
                }
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }
    }

    public class TblBrandStoreTargetDetailViewModel : PropertiesViewModelBase
    {
        public TblBrandStoreTargetDetailViewModel()
        {
            Qty = 1;
        }

        private decimal? _amountField;

        private int _glserialField;

        private int _iserialField;

        private float? _qtyField;

        private TblBrandStoreTargetHeader _tblBrandStoreTargetHeaderField;

        private StoresForBrand _tblStore1Field;

        private int? _tblstoreField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAmount")]
        public decimal? Amount
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

        public int Glserial
        {
            get
            {
                return _glserialField;
            }
            set
            {
                if ((_glserialField.Equals(value) != true))
                {
                    _glserialField = value;
                    RaisePropertyChanged("Glserial");
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqQty")]
        public float? Qty
        {
            get
            {
                return _qtyField;
            }
            set
            {
                if ((_qtyField.Equals(value) != true))
                {
                    _qtyField = value ?? 1;

                    if (_qtyField == 0)
                    {
                        _qtyField = 1;
                    }
                    RaisePropertyChanged("Qty");
                }
            }
        }

        public TblBrandStoreTargetHeader BrandStoreTargetHeaderPerRow
        {
            get
            {
                return _tblBrandStoreTargetHeaderField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandStoreTargetHeaderField, value) != true))
                {
                    _tblBrandStoreTargetHeaderField = value;
                    RaisePropertyChanged("BrandStoreTargetHeaderPerRow");
                }
            }
        }

        public StoresForBrand StorePerRow
        {
            get
            {
                return _tblStore1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblStore1Field, value) != true))
                {
                    _tblStore1Field = value;
                    RaisePropertyChanged("StorePerRow");
                    if (StorePerRow != null)
                    {
                        Tblstore = StorePerRow.Iserial;
                    }
                }
            }
        }

        public int? Tblstore
        {
            get
            {
                return _tblstoreField;
            }
            set
            {
                if ((_tblstoreField.Equals(value) != true))
                {
                    _tblstoreField = value;
                    RaisePropertyChanged("Tblstore");
                }
            }
        }
    }

    public class BrandStoreTargetViewModel : ViewModelBase
    {
        public BrandStoreTargetViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                YearList = new List<int>();
                for (var i = 0; i < 10; i++)
                {
                    const int yearNumber = 2014;
                    YearList.Add(yearNumber + i);
                }
                TransactionHeader.Year = DateTime.Now.Year;
                GetItemPermissions(PermissionItemName.BrandStoreTarget.ToString());
                Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                SelectedMainRow = new TblBrandStoreTargetDetailViewModel();

                Client.UpdateOrInsertTblBrandStoreTargetDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblBrandStoreTargetDetailViewModel)TransactionHeader.MainRowList.GetItemAt(x.outindex);

                    TransactionHeader.Glserial = x.Result.Glserial;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
                Client.DeleteTblBrandStoreTargetDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = TransactionHeader.MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) TransactionHeader.MainRowList.Remove(oldrow);
                };
            }
        }

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
                            Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                            Client.DeleteTblBrandStoreTargetDetailAsync(
                                (TblBrandStoreTargetDetail)new TblBrandStoreTargetDetail().InjectFrom(row),LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            TransactionHeader.MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (TransactionHeader.MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (TransactionHeader.MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                TransactionHeader.MainRowList.Insert(currentRowIndex + 1, new TblBrandStoreTargetDetailViewModel
                {
                    Glserial = TransactionHeader.Glserial,
                    Qty = 1,
                    Amount = 0
                });
            }
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
                    var saveRow = new TblBrandStoreTargetDetail();
                    saveRow.InjectFrom(SelectedMainRow);
                    if (TransactionHeader.Glserial == 0)
                    {
                        saveRow.TblBrandStoreTargetHeader = new TblBrandStoreTargetHeader();
                        saveRow.TblBrandStoreTargetHeader.InjectFrom(TransactionHeader);
                    }
                    else
                    {
                        saveRow.Glserial = TransactionHeader.Glserial;
                    }
                    Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                    Client.UpdateOrInsertTblBrandStoreTargetDetailAsync(saveRow, save, TransactionHeader.MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop

        private ObservableCollection<TblBrandStoreTargetDetailViewModel> _selectedMainRows;

        public ObservableCollection<TblBrandStoreTargetDetailViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBrandStoreTargetDetailViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBrandStoreTargetDetailViewModel _selectedMainRow;

        public TblBrandStoreTargetDetailViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblBrandStoreTargetHeaderViewModel _transactionHeader;

        public TblBrandStoreTargetHeaderViewModel TransactionHeader
        {
            get { return _transactionHeader ?? (_transactionHeader = new TblBrandStoreTargetHeaderViewModel()); }
            set { _transactionHeader = value; RaisePropertyChanged("TransactionHeader"); }
        }

        private List<int> _yearList;

        public List<int> YearList
        {
            get { return _yearList; }
            set { _yearList = value; RaisePropertyChanged("YearList"); }
        }

        #endregion Prop
    }
}