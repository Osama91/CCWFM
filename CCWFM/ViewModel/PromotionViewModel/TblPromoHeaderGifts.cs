using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Lite.ExcelLibrary.SpreadSheet;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.ViewModel.PromotionViewModel
{
    public class TypesDocViewModel
    {
        public int No { get; set; }

        public string Name { get; set; }
    }

    public class TblPromoHeaderGifts : ViewModelBase
    {
        public TblPromoHeaderGifts()
        {
            DetailsList = new SortableCollectionView<TblPromoDetailViewModel>();
            Type = 1;
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow; }
            set
            {
                _storePerRow = value;
                RaisePropertyChanged("StorePerRow");
                if (StorePerRow != null) TblStore = StorePerRow.iserial;
            }
        }

        private int _tblstore;

        public int TblStore
        {
            get { return _tblstore; }
            set
            {
                _tblstore = value;

                RaisePropertyChanged("TblStore");
            }
        }

        private bool _typeInfo;

        public bool TypeInfo
        {
            get { return _typeInfo; }
            set
            {
                _typeInfo = value;

                RaisePropertyChanged("TypeInfo");
            }
        }

        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;

                RaisePropertyChanged("Enabled");
            }
        }

        private short? _activeField;

        private DateTime? CreateDateField;

        private string DescriptionField;

        private double? DisCountField;

        private int FromCodeField;

        private DateTime? FromDateField;

        private int GlSerialField;

        private string MsgField;

        private DateTime? TODateField;

        private int? TblEventualHeaderField;

        private int _toCodeField;

        private int? _typeField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqActive")]
        public short? Active
        {
            get
            {
                return _activeField;
            }
            set
            {
                if ((_activeField.Equals(value) != true))
                {
                    _activeField = value;
                    RaisePropertyChanged("Active");
                }
            }
        }

        public DateTime? CreateDate
        {
            get
            {
                return CreateDateField;
            }
            set
            {
                if ((CreateDateField.Equals(value) != true))
                {
                    CreateDateField = value;
                    RaisePropertyChanged("CreateDate");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
        public string Description
        {
            get
            {
                return DescriptionField;
            }
            set
            {
                if ((ReferenceEquals(DescriptionField, value) != true))
                {
                    DescriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public double? DisCount
        {
            get
            {
                return DisCountField;
            }
            set
            {
                if ((DisCountField.Equals(value) != true))
                {
                    DisCountField = value;
                    RaisePropertyChanged("DisCount");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromCode")]
        public int FromCode
        {
            get
            {
                return FromCodeField;
            }
            set
            {
                if ((ReferenceEquals(FromCodeField, value) != true))
                {
                    FromCodeField = value;
                    RaisePropertyChanged("FromCode");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromDate")]
        public DateTime? FromDate
        {
            get
            {
                return FromDateField;
            }
            set
            {
                if ((FromDateField.Equals(value) != true))
                {
                    FromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public int GlSerial
        {
            get
            {
                return GlSerialField;
            }
            set
            {
                if ((GlSerialField.Equals(value) != true))
                {
                    GlSerialField = value;
                    RaisePropertyChanged("GlSerial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMessage")]
        public string Msg
        {
            get
            {
                return MsgField;
            }
            set
            {
                if ((ReferenceEquals(MsgField, value) != true))
                {
                    MsgField = value;
                    RaisePropertyChanged("Msg");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToDate")]
        public DateTime? TODate
        {
            get
            {
                return TODateField;
            }
            set
            {
                if ((TODateField.Equals(value) != true))
                {
                    {
                        if (value < FromDate)
                        {
                            value = FromDate;
                        }
                        value = FromDate;
                    }
                    TODateField = value;
                    RaisePropertyChanged("TODate");
                }
            }
        }

        public int? TblEventualHeader
        {
            get
            {
                return TblEventualHeaderField;
            }
            set
            {
                if ((TblEventualHeaderField.Equals(value) != true))
                {
                    TblEventualHeaderField = value;
                    RaisePropertyChanged("TblEventualHeader");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToCode")]
        public int
            ToCode
        {
            get
            {
                return _toCodeField;
            }
            set
            {
                if ((ReferenceEquals(_toCodeField, value) != true))
                {
                    _toCodeField = value;
                    RaisePropertyChanged("ToCode");
                }
            }
        }

        private TBLEVENTUALHEADER _eventperrow;

        public TBLEVENTUALHEADER EventPerrow
        {
            get
            {
                return _eventperrow;
            }
            set
            {
                if ((ReferenceEquals(_eventperrow, value) != true))
                {
                    _eventperrow = value;
                    this.RaisePropertyChanged("EventPerrow");
                    if (EventPerrow != null) TblEventualHeader = EventPerrow.ISERIAL;
                }
            }
        }

        public int? Type
        {
            get
            {
                return _typeField;
            }
            set
            {
                if ((_typeField.Equals(value) != true))
                {
                    _typeField = value;
                    RaisePropertyChanged("Type");
                    TypeInfo = Type == 1;
                }
            }
        }

        private SortableCollectionView<TblPromoDetailViewModel> _detailslist;

        public SortableCollectionView<TblPromoDetailViewModel> DetailsList
        {
            get { return _detailslist; }
            set
            {
                _detailslist = value;

                RaisePropertyChanged("DetailsList");
            }
        }
    }

    public class PromoHeaderGifts : ViewModelBase
    {
        public PromoHeaderGifts()
        {
            GetItemPermissions(PermissionItemName.PromotionForm.ToString());
            GetCustomePermissions(PermissionItemName.PromotionForm.ToString());
            SelectedDetailRows = new ObservableCollection<TblPromoDetailViewModel>();
            TransactionHeader = new TblPromoHeaderGifts();
            SelectedMainRow = new TblPromoHeaderGifts();
            MainRowList = new SortableCollectionView<TblPromoHeaderGifts>();
            TypesListDoc = new SortableCollectionView<TypesDocViewModel>();
            RangeList = new SortableCollectionView<RangeViewModel>();
            DetailList = new SortableCollectionView<TBLEVENTUALHEADER>();
            StoreList = new SortableCollectionView<TblStore>();
            Stores = new ObservableCollection<TblStoreViewModel>();
            TypesListDoc.Add(new TypesDocViewModel()
            {
                No = 0,
                Name = "Gifts"
            });

            Client.SearchforsStoreNameCompleted += (M, K) =>
            {
                Loading = false;
                StoreList = K.Result;

                //OnSupplierCompleted();
            };

            Client.GetPrmotionRangeCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    MessageBox.Show("This Range Is Already Exist");
                }
                else
                {
                    var FromCodeInt = Convert.ToInt32(TransactionHeader.FromCode);
                    var Difference = Convert.ToInt32(TransactionHeader.ToCode) - Convert.ToInt32(TransactionHeader.FromCode);
                    var random = new Random();
                    for (var x = 0; x < Difference; x++)
                    {
                        var codeNo = FromCodeInt + x;
                        var randomNumber = random.Next(0, 9999);
                        TransactionHeader.DetailsList.Add(new TblPromoDetailViewModel()
                        {
                            Code = codeNo.ToString(),
                            PIN = randomNumber.ToString(),
                            SelecteIndex = x + 1
                        }
                     );
                    }
                }

                for (var i = 0; i < TransactionHeader.DetailsList.Count; i++)
                {
                    var codeNo = +i;
                }
            };
            Client.DeleteTblPromoDetailCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    throw ev.Error;
                }

                var oldrow = MainRowList.FirstOrDefault(x => x.GlSerial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
            };
            Client.UpdateAndInsertTblPromoDetailCompleted += (x, y) =>
            {
                var savedRow = (TblPromoDetailViewModel)TransactionHeader.DetailsList.GetItemAt(y.outindex);

                if (savedRow != null) savedRow.InjectFrom(y.Result);
            };

            Client.GetStoresCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblStoreViewModel();
                    newrow.InjectFrom(row);

                    Stores.Add(newrow);
                }

                DetailFullCount = sv.fullCount;
                Loading = false;
            };
            Client.GetDetailCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblPromoDetailViewModel();
                    newrow.InjectFrom(row);
                    TransactionHeader.DetailsList.Add(newrow);
                    newrow.SelecteIndex = TransactionHeader.DetailsList.IndexOf(newrow) + 1;
                }

                if (ExcelSheetprop == true)
                {
                    SaveFileDialog sDialog = new SaveFileDialog();
                    sDialog.Filter = "Excel Files(*.xls)|*.xls";
                    if (sDialog.ShowDialog() == true)
                    {
                        // create an instance of excel workbook
                        Workbook workbook = new Workbook();
                        // create a worksheet object
                        Worksheet worksheet = new Worksheet("Friends");
                        // write data in worksheet cells
                        for (int i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        {
                            worksheet.Cells[i, 2] = new Cell(TransactionHeader.Msg + "  Expiry:" + TransactionHeader.TODate.ToString() + "   Code:" + TransactionHeader.DetailsList.ElementAt(i).Code + "  Pin:" + TransactionHeader.DetailsList.ElementAt(i).PIN);
                        }
                        for (int i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        {
                            worksheet.Cells[1 + i, 0] = new Cell("MobileNo");
                            worksheet.Cells[1 + i, 1] = new Cell(TransactionHeader.DetailsList.ElementAt(i).MobileNo);
                        }

                        workbook.Worksheets.Add(worksheet);
                        Stream sFile = sDialog.OpenFile();

                        // save method needs a stream object to write an excel file.
                        workbook.Save(sFile);
                    }
                }
                Loading = false;
                ExcelSheetprop = false;
            };

            Client.GetevDataCompleted += (s, sv) =>
            {
                DetailList.Clear();
                foreach (var row in sv.Result)
                {
                    DetailList.Add(row);
                }
                DetailFullCount = sv.fullCount;
                Loading = false;
            };

            Client.SearchTblPromoHeaderAllCompleted += (y, v) =>
            {
                foreach (var row in v.Result)
                {
                    Loading = false;

                    // غيرت حاجة هنا ممكن تكون غلط
                    var newrow = new TblPromoHeaderGifts();
                    newrow.InjectFrom(row);
                    newrow.StorePerRow = row.TblStore1;
                    MainRowList.Add(newrow);
                }
            };

            Client.UpdateOrInsertTblPromoHeaderCompleted += (s, x) =>
            {
                if (x.Error == null)
                {
                    TransactionHeader.InjectFrom(x.Result);
                    SaveMainRowExc();
                    MessageBox.Show("Saved Successfully");
                }
                else
                {
                    MessageBox.Show(x.Error.Message);
                }
                Loading = false;

                //TransactionHeader.InjectFrom(x.Result);
            };
            Client.DeleteTblPromoHeaderCompleted += (w, k) =>
            {
                Loading = false;
                TransactionHeader = new TblPromoHeaderGifts();
            };
        }

        internal void SearchForStorEname()
        {
            //string StorEname,string code

            Loading = true;
            Client.SearchforsStoreNameAsync(StorEname, Code, LoggedUserInfo.DatabasEname);
        }

        public void GetPromoRange()
        {
            Client.GetPrmotionRangeAsync(TransactionHeader.FromCode, TransactionHeader.ToCode, LoggedUserInfo.DatabasEname);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.SearchTblPromoHeaderAllAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void UpdateAndInsert()
        {
            var valiationCollection = new List<ValidationResult>();
            bool isvalid = Validator.TryValidateObject(TransactionHeader, new ValidationContext(TransactionHeader, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblPromoHeader();
                data.InjectFrom(TransactionHeader);
                bool save = TransactionHeader.GlSerial == 0;
                if (save)
                {
                    if (AllowAdd)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblPromoHeaderAsync(data, LoggedUserInfo.DatabasEname);
                    }

                    else
                    {
                        MessageBox.Show("You are Not Allowed to Add");
                    }
                }
                else
                {
                    if (AllowUpdate)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblPromoHeaderAsync(data, LoggedUserInfo.DatabasEname);
                    }

                    else
                    {
                        MessageBox.Show("You are Not Allowed to Update");
                    }
                }
            }
        }

        public void Delete()
        {
            var data = new TblPromoHeader();
            data.InjectFrom(TransactionHeader);

            if (AllowDelete)
            {
                Loading = true;
                Client.DeleteTblPromoHeaderAsync(data, LoggedUserInfo.DatabasEname);
            }
            else
            {
                MessageBox.Show("You are Not Allowed to Delete");
            }
        }

        public void GetStores()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetStoresAsync(Stores.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void SaveMainRowExc()
        {
            if (TransactionHeader.DetailsList != null)
            {
                foreach (var Item in TransactionHeader.DetailsList)
                {
                    var save = Item.Glserial == 0;
                    var saveRow = new TblPromoDetail();
                    saveRow.InjectFrom(Item);
                    saveRow.Glserial = TransactionHeader.GlSerial;

                    if (save)
                    {
                        if (AllowAdd)
                        {
                            Loading = true;
                            Client.UpdateAndInsertTblPromoDetailAsync(saveRow, save, TransactionHeader.DetailsList.IndexOf(Item), LoggedUserInfo.DatabasEname);
                        }

                        else
                        {
                            MessageBox.Show("You are Not Allowed to Add");
                        }
                    }
                    else
                    {
                        if (AllowUpdate)
                        {
                            Loading = true;
                            Client.UpdateAndInsertTblPromoDetailAsync(saveRow, save, TransactionHeader.DetailsList.IndexOf(Item), LoggedUserInfo.DatabasEname);
                        }

                        else
                        {
                            MessageBox.Show("You are Not Allowed to Update");
                        }
                    }
                }
                //var valiationCollection = new List<ValidationResult>();

                // var isvalid = Validator.TryValidateObject(TransactionHeader.DetailsList, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                //if (isvalid)
                //{
                //    var save = SelectedDetailRow.Iserial == 0;

                //}
            }
        }

        public void ExcelSheet()
        {
            Client.GetDetailAsync(TransactionHeader.DetailsList.Count, 30000, TransactionHeader.GlSerial, SortBy, null, null, LoggedUserInfo.DatabasEname);
            ExcelSheetprop = true;
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (TransactionHeader.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (TransactionHeader.DetailsList.Count - 1))
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

                TransactionHeader.DetailsList.Insert(currentRowIndex + 1, new TblPromoDetailViewModel()

                {
                });
            }
        }

        public void DeleteMainRow()
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
                            if (AllowDelete)
                            {
                                Loading = true;
                                Client.DeleteTblPromoDetailAsync(
                             (TblPromoDetail)new TblPromoDetail().InjectFrom(row), LoggedUserInfo.DatabasEname);
                            }
                            else
                            {
                                MessageBox.Show("You are Not Allowed to Delete");
                            }
                        }
                        else
                        {
                            TransactionHeader.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        private bool _excelSheetprop;

        public bool ExcelSheetprop
        {
            get { return _excelSheetprop; }
            set
            {
                _excelSheetprop = value;

                RaisePropertyChanged("ExcelSheetprop");
            }
        }

        private ObservableCollection<TblStoreViewModel> _stores;

        public ObservableCollection<TblStoreViewModel> Stores
        {
            get { return _stores; }
            set
            {
                _stores = value;
                RaisePropertyChanged("Stores");
            }
        }

        private int _glserial;

        public int GlSerial
        {
            get { return _glserial; }
            set
            {
                _glserial = value;

                RaisePropertyChanged("GlSerial");
            }
        }

        private DateTime? _date;

        public DateTime? Date
        {
            get { return _date; }
            set
            {
                _date = value;

                RaisePropertyChanged("Date");
            }
        }

        private int? _fromCode;

        public int? FromCode
        {
            get { return _fromCode; }
            set
            {
                _fromCode = value;

                RaisePropertyChanged("FromCode");
            }
        }

        private int? _tocode;

        public int? ToCode
        {
            get { return _tocode; }
            set
            {
                _tocode = value;

                RaisePropertyChanged("ToCode");
            }
        }

        private SortableCollectionView<TypesDocViewModel> _typesListdoc;

        public SortableCollectionView<TypesDocViewModel> TypesListDoc
        {
            get { return _typesListdoc; }
            set
            {
                _typesListdoc = value;
                RaisePropertyChanged("TypesListDoc");
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

        private ObservableCollection<TblPromoDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPromoDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows; }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private SortableCollectionView<TBLEVENTUALHEADER> _detailList;

        public SortableCollectionView<TBLEVENTUALHEADER> DetailList
        {
            get { return _detailList; }
            set
            {
                _detailList = value;
                RaisePropertyChanged("DetailList");
            }
        }

        private TblPromoHeaderGifts _selectedMainRow;

        public TblPromoHeaderGifts SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }



        private SortableCollectionView<TblPromoHeaderGifts> _mainRowList;

        public SortableCollectionView<TblPromoHeaderGifts> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private SortableCollectionView<TypesViewModel> _typesList;

        public SortableCollectionView<TypesViewModel> TypesList
        {
            get { return _typesList; }
            set
            {
                _typesList = value;
                RaisePropertyChanged("TypesList");
            }
        }

        private SortableCollectionView<RangeViewModel> _rangeList;

        public SortableCollectionView<RangeViewModel> RangeList
        {
            get { return _rangeList; }
            set
            {
                _rangeList = value;
                RaisePropertyChanged("RangeList");
            }
        }

        private TblPromoDetailViewModel _SelectedDetailRow;

        public TblPromoDetailViewModel SelectedDetailRow
        {
            get { return _SelectedDetailRow; }
            set
            {
                _SelectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private TblPromoHeaderGifts _transactionHeader;

        public TblPromoHeaderGifts TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                _transactionHeader = value;
                RaisePropertyChanged("TransactionHeader");
            }
        }

        internal void GetTblPromoDetail()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Client.GetDetailAsync(TransactionHeader.DetailsList.Count, PageSize, TransactionHeader.GlSerial, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        internal void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetevDataAsync(DetailList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }
    }
}