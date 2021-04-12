using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.ViewModel.GenericViewModels;
using System.Collections.Specialized;

namespace CCWFM.ViewModel.PromotionViewModel
{
    public class TypesViewModel
    {
        public int No { get; set; }

        public string Name { get; set; }
    }

    public class RangeViewModel : PropertiesViewModelBase
    {
        private short? _NoRange;

        public short? NoRange
        {
            get { return _NoRange; }
            set
            {
                _NoRange = value;
                RaisePropertyChanged("TypeInfo");
            }
        }


        public string NameRange { get; set; }
    }

    public class TblPromHeaderViewModel : PropertiesViewModelBase
    {
        public TblPromHeaderViewModel()
        {
            DetailsList = new ObservableCollection<TblPromoDetailViewModel>();
            Type = 1;
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

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                RaisePropertyChanged("Enabled");
            }
        }

        private short? _activeField;

        private DateTime? _createDateField;

        private string _descriptionField;

        private double? _disCountField;

        private int _fromCodeField;

        private DateTime? _fromDateField;

        private int _glSerialField;

        private string _msgField;

        private DateTime? _toDateField;

        private int? _tblEventualHeaderField;

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
                _activeField = value;
                RaisePropertyChanged("Active");
            }
        }

        public DateTime? CreateDate
        {
            get
            {
                return _createDateField;
            }
            set
            {
                if ((_createDateField.Equals(value) != true))
                {
                    _createDateField = value;
                    RaisePropertyChanged("CreateDate");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
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

        public double? DisCount
        {
            get
            {
                return _disCountField;
            }
            set
            {
                if ((_disCountField.Equals(value) != true))
                {
                    _disCountField = value;
                    RaisePropertyChanged("DisCount");
                }
            }
        }

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromCode")]
        public int FromCode
        {
            get
            {
                return _fromCodeField;
            }
            set
            {
                if ((Equals(_fromCodeField, value) != true))
                {
                    _fromCodeField = value;
                    RaisePropertyChanged("FromCode");
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

        public int GlSerial
        {
            get
            {
                return _glSerialField;
            }
            set
            {
                if ((_glSerialField.Equals(value) != true))
                {
                    _glSerialField = value;
                    RaisePropertyChanged("GlSerial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "Message")]
        public string Msg
        {
            get
            {
                return _msgField;
            }
            set
            {
                if ((ReferenceEquals(_msgField, value) != true))
                {
                    _msgField = value;
                    RaisePropertyChanged("Msg");
                }
            }
        }


        public DateTime? TODate
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
                    RaisePropertyChanged("TODate");
                }
            }
        }

        public int? TblEventualHeader
        {
            get
            {
                return _tblEventualHeaderField;
            }
            set
            {
                if ((_tblEventualHeaderField.Equals(value) != true))
                {
                    _tblEventualHeaderField = value;
                    RaisePropertyChanged("TblEventualHeader");
                }
            }
        }

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToCode")]
        public int
            ToCode
        {
            get
            {
                return _toCodeField;
            }
            set
            {
                if ((Equals(_toCodeField, value) != true))
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

                _eventperrow = value;
                RaisePropertyChanged("EventPerrow");
                if (EventPerrow != null) TblEventualHeader = EventPerrow.ISERIAL;

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

        private ObservableCollection<TblPromoDetailViewModel> _detailslist;

        public ObservableCollection<TblPromoDetailViewModel> DetailsList
        {
            get { return _detailslist; }
            set
            {
                _detailslist = value;

                RaisePropertyChanged("DetailsList");
            }
        }
    }

    public class TblDownLoadDefentionViewModel : ViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _aname;

        public string Aname
        {
            get { return _aname; }
            set
            {
                _aname = value;

                RaisePropertyChanged("Aname");
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

        private bool _chek;

        public bool Chek
        {
            get { return _chek; }
            set
            {
                _chek = value;

                RaisePropertyChanged("Chek");
            }
        }
    }

    public class TblStoreViewModel : ViewModelBase
    {
        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _aname;

        public string aname
        {
            get { return _aname; }
            set
            {
                _aname = value;

                RaisePropertyChanged("aname");
            }
        }

        private string _code;

        public string code
        {
            get { return _code; }
            set
            {
                _code = value;

                RaisePropertyChanged("code");
            }
        }

        private bool _chek;

        public bool Chek
        {
            get { return _chek; }
            set
            {
                _chek = value;

                RaisePropertyChanged("Chek");
            }
        }
    }

    public class TblPromoCriteriaViewModel : ViewModelBase
    {
        //private CRUD_ManagerServiceClient client = new CRUD_ManagerServiceClient();

        public TblPromoCriteriaViewModel()
        {
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                RaisePropertyChanged("Enabled");
            }
        }
        private bool _NotActive;

        public bool NotActive
        {
            get { return _NotActive; }
            set { _NotActive = value; RaisePropertyChanged("NotActive"); }
        }

        private DateTime? _dateFromField;

        private DateTime? _dateToField;

        private string _descriptionField;

        private int _glserialField;

        private int _noOfVisitFromField;

        private int _noOfVisitToField;

        private int _salesAmountFromField;

        private int _salesAmountToField;

        private int? _selectTopField;

        private string _tblIteMDownLoadDefField;

        private int _tblPromoHeader;

        public int TblPromoHeader
        {
            get { return _tblPromoHeader; }
            set
            {
                _tblPromoHeader = value;

                RaisePropertyChanged("TblPromoHeader");
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromDate")]
        public DateTime? DateFrom
        {
            get
            {
                return _dateFromField;
            }
            set
            {
                if ((_dateFromField.Equals(value) != true))
                {
                    _dateFromField = value;
                    RaisePropertyChanged("DateFrom");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToDate")]
        public DateTime? DateTo
        {
            get
            {
                return _dateToField;
            }
            set
            {
                if ((_dateToField.Equals(value) != true))
                {
                    if (value < DateFrom)
                    {
                        value = DateFrom;
                    }
                    _dateToField = value;
                    RaisePropertyChanged("DateTo");
                }
            }
        }

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqNoOfVisitsFrom")]
        public int NoOfVisitFrom
        {
            get
            {
                return _noOfVisitFromField;
            }
            set
            {
                if ((Equals(_noOfVisitFromField, value) != true))
                {
                    _noOfVisitFromField = value;
                    RaisePropertyChanged("NoOfVisitFrom");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqNoOfVisitsTo")]
        public int NoOfVisitTo
        {
            get
            {
                return _noOfVisitToField;
            }
            set
            {
                if ((Equals(_noOfVisitToField, value) != true))
                {
                    if (value < NoOfVisitFrom)
                    {
                        value = NoOfVisitFrom;
                    }

                    _noOfVisitToField = value;
                    RaisePropertyChanged("NoOfVisitTo");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSalesAmountFrom")]
        public int SalesAmountFrom
        {
            get
            {
                return _salesAmountFromField;
            }
            set
            {
                if ((Equals(_salesAmountFromField, value) != true))
                {
                    _salesAmountFromField = value;
                    RaisePropertyChanged("SalesAmountFrom");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSalesAmountTo")]
        public int SalesAmountTo
        {
            get
            {
                return _salesAmountToField;
            }
            set
            {
                if ((Equals(_salesAmountToField, value) != true))
                {
                    if (value < Convert.ToInt32(SalesAmountFrom))
                    {
                        value = Convert.ToInt32(SalesAmountFrom);
                    }
                    _salesAmountToField = value;
                    RaisePropertyChanged("SalesAmountTo");
                }
            }
        }

        public int? SelectTop
        {
            get
            {
                return _selectTopField;
            }
            set
            {
                if ((_selectTopField.Equals(value) != true))
                {
                    _selectTopField = value;
                    RaisePropertyChanged("SelectTop");
                }
            }
        }

        public string TblIteMDownLoadDef
        {
            get
            {
                return _tblIteMDownLoadDefField;
            }
            set
            {
                if ((ReferenceEquals(_tblIteMDownLoadDefField, value) != true))
                {
                    _tblIteMDownLoadDefField = value;
                    RaisePropertyChanged("TblIteMDownLoadDef");
                }
            }
        }
        private TblItemDownLoadDef _itemperrow;

        public TblItemDownLoadDef Itemperrow
        {
            get
            {
                return _itemperrow;
            }
            set
            {
                if ((ReferenceEquals(_itemperrow, value) != true))
                {
                    _itemperrow = value;
                    RaisePropertyChanged("Itemperrow");
                    if (Itemperrow != null) TblIteMDownLoadDef = Itemperrow.Aname;
                }
            }
        }
    }

    public class PromHeaderViewModel : ViewModelStructuredBase
    {
        public PromHeaderViewModel() : base(PermissionItemName.PromotionForm)

        {
            GetItemPermissions(PermissionItemName.PromotionForm.ToString());
            GetCustomePermissions(PermissionItemName.PromotionForm.ToString());
            SelectedDetailRows = new ObservableCollection<TblPromoDetailViewModel>();
            TransactionHeader = new TblPromHeaderViewModel();
            MainRowList = new ObservableCollection<TblPromHeaderViewModel>();
            TypesList = new ObservableCollection<TypesViewModel>();
            RangeList = new ObservableCollection<RangeViewModel>();
            DetailList = new ObservableCollection<TBLEVENTUALHEADER>();
            TypesList.Add(new TypesViewModel
            {
                No = 1,
                Name = "Event"
            });

            TypesList.Add(new TypesViewModel
            {
                No = 2,
                Name = "Discount %"
            });
            TypesList.Add(new TypesViewModel
            {
                No = 3,
                Name = "Discount Amount"
            });

            RangeList.Add(new RangeViewModel
            {
                NoRange = 0,
                NameRange = "Pending"
            });

            RangeList.Add(new RangeViewModel
            {
                NoRange = 1,
                NameRange = "Active"
            });
            RangeList.Add(new RangeViewModel
            {
                NoRange = 2,
                NameRange = "Closed"
            });

            Client.GetPrmotionRangeCompleted += (s, sv) =>
            {
                //if (sv.Result != null)
                //{
                //    MessageBox.Show("This Range Is Already Exist");
                //}
                //else
                //{
                //    var fromCodeInt = Convert.ToInt32(TransactionHeader.FromCode);
                //    var difference = Convert.ToInt32(TransactionHeader.ToCode) - Convert.ToInt32(TransactionHeader.FromCode);
                //    var random = new Random();
                //    for (var x = 0; x < difference; x++)
                //    {
                //        var codeNo = fromCodeInt + x;

                //        var randomNumber = random.Next(0, 9999);
                //        TransactionHeader.DetailsList.Add(new TblPromoDetailViewModel
                //        {
                //            Code = codeNo.ToString(CultureInfo.InvariantCulture),
                //            PIN = randomNumber.ToString(CultureInfo.InvariantCulture),
                //            SelecteIndex = x + 1
                //        }
                //        );
                //    }
                //}

                //for (var i = 0; i < TransactionHeader.DetailsList.Count; i++)
                //{
                //    var codeNo = +i;
                //}
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
            Client.GetDetailCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblPromoDetailViewModel();
                    newrow.InjectFrom(row);
                    TransactionHeader.DetailsList.Add(newrow);
                    newrow.SelecteIndex = TransactionHeader.DetailsList.IndexOf(newrow) + 1;
                }

                if (ExcelSheetprop)
                {
                    var sDialog = new SaveFileDialog();
                    sDialog.Filter = "Excel Files(*.xls)|*.xls";
                    if (sDialog.ShowDialog() == true)
                    {
                        // create an instance of excel workbook
                        var workbook = new Workbook();
                        // create a worksheet object
                        var worksheet = new Worksheet("Friends");
                        // write data in worksheet cells
                        for (var i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        {
                            worksheet.Cells[i, 2] = new Cell(TransactionHeader.Msg + "  Expiry:" + TransactionHeader.TODate.ToString() + "   Code:" + TransactionHeader.DetailsList.ElementAt(i).Code + "  Pin:" + TransactionHeader.DetailsList.ElementAt(i).PIN);
                        }
                        for (var i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        {
                            worksheet.Cells[1 + i, 0] = new Cell("MobileNo");
                            worksheet.Cells[1 + i, 1] = new Cell(TransactionHeader.DetailsList.ElementAt(i).MobileNo);
                        }

                        //worksheet.Cells[0, 0] = new Cell("Msg");
                        //worksheet.Cells[0, 1] = new Cell(TransactionHeader.Msg);
                        //worksheet.Cells[0, 2] = new Cell("Expiry");
                        //worksheet.Cells[0, 3] = new Cell(TransactionHeader.TODate.ToString());

                        //for (int i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        //{
                        //    worksheet.Cells[1 + i, 0] = new Cell("MobileNo");
                        //    worksheet.Cells[1 + i, 1] = new Cell(TransactionHeader.DetailsList.ElementAt(i).MobileNo);

                        //}

                        //for (int i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        //{
                        //    worksheet.Cells[1 + i, 2] = new Cell("Code");
                        //    worksheet.Cells[1 + i, 3] = new Cell(TransactionHeader.DetailsList.ElementAt(i).Code);

                        //}
                        //for (int i = 0; TransactionHeader.DetailsList.Count() > i; i++)
                        //{
                        //    worksheet.Cells[1 + i, 4] = new Cell("PIN");
                        //    worksheet.Cells[1 + i, 5] = new Cell(TransactionHeader.DetailsList.ElementAt(i).PIN);

                        //}
                        workbook.Worksheets.Add(worksheet);
                        var sFile = sDialog.OpenFile();

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

                    var newrow = new TblPromHeaderViewModel();
                    newrow.InjectFrom(row);
                    newrow.EventPerrow = row.TBLEVENTUALHEADER1;
                    if (row.TblPromoCriterias.Any())
                    {
                        var savedCriteria = row.TblPromoCriterias.FirstOrDefault();
                        TransactionHeaderCriteria.InjectFrom(savedCriteria);

                        foreach (var Storerow in savedCriteria.TblStorePromoDetails)
                        {
                            var tblStoreViewModel = Stores.FirstOrDefault(x => x.Iserial == Storerow.TblStores);
                            if (tblStoreViewModel != null)
                                tblStoreViewModel.Chek = true;
                        }

                        foreach (var Brandrow in savedCriteria.TblBrandPromoDetails)
                        {
                            var tblBrandViewModel = Brands.FirstOrDefault(x => x.Iserial == Brandrow.Brands);
                            if (tblBrandViewModel != null)
                                tblBrandViewModel.Chek = true;
                        }

                    }

                    //_viewModel.TransactionHeaderCriteria.Clear();
                    //  _viewModel.TransactionHeader.StorePerRow = _viewModel.SelectedMainRow.TblStore1;
                    ////  _viewModel.GetTblPromoDetail();
                    //GetBrandsDetial();
                    //GetStoresDetail();
                    MainRowList.Add(newrow);
                }
                Loading = false;
            };

            Client.UpdateOrInsertTblPromoHeaderCompleted += (s, x) =>
            {
                if (x.Error == null)
                {
                    TransactionHeader.InjectFrom(x.Result);
                    Client.GetCustomerCriteriaAsync(TransactionHeader.GlSerial, LoggedUserInfo.DatabasEname);
                    //  SaveMainRowExc();
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
                TransactionHeader = new TblPromHeaderViewModel();
            };

            //////////////////////////////criteria
            SelectedDetailRowsCriteria = new ObservableCollection<TblItemDownLoadDef>();
            TransactionHeaderCriteria = new TblPromoCriteriaViewModel();
            MainRowListCriteria = new ObservableCollection<TblPromoCriteria>();
            Brands = new ObservableCollection<TblDownLoadDefentionViewModel>();
            StoreList = new ObservableCollection<TblStore>();
            Stores = new ObservableCollection<TblStoreViewModel>();
            MobileNo = new ObservableCollection<TblPromoDetailViewModel>();
            Client.GetStoresDetailsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var tblStoreViewModel = Stores.FirstOrDefault(x => x.Iserial == row.TblStores);
                    if (tblStoreViewModel != null)
                        tblStoreViewModel.Chek = true;
                }

                Loading = false;
            };

            Client.GetCustomerCriteriaCompleted += (s, sv) =>
            {
                GetTblPromoDetail();
                //for (int i = 0; i < sv.Result.Count; i++)
                //{
                //    try
                //    {
                //        var row = TransactionHeader.DetailsList.ElementAt(i);

                //        if (row != null)
                //        {
                //            row.MobileNo = sv.Result.ElementAt(i).customerMobile;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }

                //    Loading = false;

                //}
            };
            Client.GeTblBrandPromoDetailsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var tblitemdefention = Brands.FirstOrDefault(x => x.Iserial == row.Brands);
                    if (tblitemdefention != null)
                        tblitemdefention.Chek = true;
                }

                DetailFullCount = sv.fullCount;
                Loading = false;
            };

            Client.GetBrandsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblDownLoadDefentionViewModel();
                    newrow.InjectFrom(row);
                    newrow.Iserial = row.iserial;
                    Brands.Add(newrow);
                }

                DetailFullCount = sv.fullCount;
                Loading = false;
            };

            Client.GetStoresCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new TblStoreViewModel();
                    newrow.InjectFrom(row);
                    newrow.Iserial = row.iserial;
                    Stores.Add(newrow);
                }

                DetailFullCount = sv.fullCount;
                Loading = false;
            };
            Client.SearchTblPromoCriteriaCompleted += (y, v) =>
            {
                foreach (var row in v.Result)
                {
                    Loading = false;
                    var newrow = new TblPromoCriteria();
                    newrow.InjectFrom(row);

                    MainRowListCriteria.Add(newrow);
                }
                Loading = false;
            };

            Client.UpdateOrInsertTblPromoCriteriaCompleted += (s, x) =>
            {
                if (x.Error == null)
                {
                    TransactionHeaderCriteria.InjectFrom(x.Result);
                    //SaveMainRowExc();
                    MessageBox.Show("Saved Successfully");


                }
                else
                {
                    MessageBox.Show(x.Error.Message);
                }
                Loading = false;
                //   Loading = false;

                //TransactionHeaderCriteria.InjectFrom(x.Result);
            };

            Client.DeleteTblPromoCriteriaCompleted += (w, k) =>
            {
                Loading = false;
                TransactionHeaderCriteria = new TblPromoCriteriaViewModel();
            };
            GetBrands();
            GetStores();
        }

        #region

        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TblPromHeaderViewModel> vm =
                new GenericSearchViewModel<TblPromHeaderViewModel>() { Title = "Transfer Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (vm.ResultItemsList[e.NewStartingIndex] != null)
                    {
                        TransactionHeader = vm.ResultItemsList[e.NewStartingIndex];
                        TransactionHeader.Enabled = true;
                        TransactionHeader.DetailsList.Clear();

                        GetTblPromoDetail();
                    }

                }
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
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            //                       < sdk:DataGridTextColumn dataGrid:DataGridColumnHelper.HeaderBinding = "{Binding Description, Source={StaticResource LocalizedStrings}}"  Binding = "{Binding Description,Mode=TwoWay}" Width = "100" />

            return new ObservableCollection<SearchColumnModel>()
                {
                 new SearchColumnModel()
                    {
                        Header=strings.Iserial,
                        PropertyPath=nameof(TblPromHeaderViewModel.GlSerial),
                    },
                  new SearchColumnModel()
                    {
                        Header=strings.FromDate,
                        PropertyPath=nameof(TblPromHeaderViewModel.FromDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ToDate,
                        PropertyPath=nameof(TblPromHeaderViewModel.TODate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                new SearchColumnModel()
                    {
                        Header=strings.FromCode,
                        PropertyPath=nameof(TblPromHeaderViewModel.FromCode),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ToCode,
                        PropertyPath=nameof(TblPromHeaderViewModel.ToCode),
                    },
                     new SearchColumnModel()
                    {
                        Header=strings.ToCode,
                        PropertyPath=nameof(TblPromHeaderViewModel.ToCode),
                    },
                      new SearchColumnModel()
                    {
                        Header=strings.Description,
                        PropertyPath=nameof(TblPromHeaderViewModel.Description),
                    },
                    //new SearchColumnModel()
                    //{
                    //    Header=strings.WarehouseFrom,
                    //    PropertyPath= string.Format("{0}.{1}", nameof(TblPromHeaderViewModel.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                    //    FilterPropertyPath=string.Format("{0}.{1}",nameof(TblPromHeaderViewModel.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                    //},
                    //new SearchColumnModel()
                    //{
                    //    Header="Code To",
                    //    PropertyPath=nameof(TblPromHeaderViewModel.CodeTo),
                    //},
                    //new SearchColumnModel()
                    //{
                    //    Header=strings.WarehouseTo,
                    //    PropertyPath= string.Format("{0}.{1}", nameof(TblPromHeaderViewModel.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                    //    FilterPropertyPath=string.Format("{0}.{1}",nameof(TblPromHeaderViewModel.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                    //},
                    //new SearchColumnModel()
                    //{
                    //    Header=strings.Date,
                    //    PropertyPath=nameof(TblPromHeaderViewModel.DocDate),
                    //    StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    //},
                    //new SearchColumnModel()
                    //{
                    //    Header=strings.Approved,
                    //    PropertyPath=nameof(TblPromHeaderViewModel.Approved),
                    //},
                    //new SearchColumnModel()
                    //{
                    //    Header=strings.ApproveDate,
                    //    PropertyPath=nameof(TblPromHeaderViewModel.ApproveDate),
                    //    StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    //},
                };
        }
        public void GetPromoRange()
        {
            Client.GetPrmotionRangeAsync(TransactionHeader.FromCode, TransactionHeader.ToCode, LoggedUserInfo.DatabasEname);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.GlSerial";
            Loading = true;
            Client.SearchTblPromoHeaderAllAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);

            //Loading = true;

            //Client.SearchTblPromoHeaderAllAsync(GlSerial, Date, FromCode, ToCode, Code, StorEname, LoggedUserInfo.DatabasEname);
        }

        public void UpdateAndInsert()
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(TransactionHeader, new ValidationContext(TransactionHeader, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblPromoHeader();
                data.InjectFrom(TransactionHeader);
                data.TblPromoCriterias = new ObservableCollection<TblPromoCriteria>();
                data.TblPromoCriterias.Add(UpdateAndInsertCriteria());
                var save = TransactionHeader.GlSerial == 0;
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
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
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
        }

        public void SaveMainRowExc()
        {
            if (TransactionHeader.DetailsList != null)
            {
                foreach (var item in TransactionHeader.DetailsList)
                {
                    var save = item.Glserial == 0;
                    var saveRow = new TblPromoDetail();
                    saveRow.InjectFrom(item);
                    saveRow.Iserial = TransactionHeader.GlSerial;

                    if (save)
                    {
                        if (AllowAdd)
                        {
                            Loading = true;
                            Client.UpdateAndInsertTblPromoDetailAsync(saveRow, save, TransactionHeader.DetailsList.IndexOf(item), LoggedUserInfo.DatabasEname);
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
                            Client.UpdateAndInsertTblPromoDetailAsync(saveRow, save, TransactionHeader.DetailsList.IndexOf(item), LoggedUserInfo.DatabasEname);
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

                TransactionHeader.DetailsList.Insert(currentRowIndex + 1, new TblPromoDetailViewModel());
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

        private ObservableCollection<TblPromoDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPromoDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows; }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private ObservableCollection<TBLEVENTUALHEADER> _detailList;

        public ObservableCollection<TBLEVENTUALHEADER> DetailList
        {
            get { return _detailList; }
            set
            {
                _detailList = value;
                RaisePropertyChanged("DetailList");
            }
        }

        private TblPromHeaderViewModel _selectedMainRow;

        public TblPromHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }
        private ObservableCollection<TblPromHeaderViewModel> _mainRowList;

        public ObservableCollection<TblPromHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TypesViewModel> _typesList;

        public ObservableCollection<TypesViewModel> TypesList
        {
            get { return _typesList; }
            set
            {
                _typesList = value;
                RaisePropertyChanged("TypesList");
            }
        }

        private ObservableCollection<RangeViewModel> _rangeList;

        public ObservableCollection<RangeViewModel> RangeList
        {
            get { return _rangeList; }
            set
            {
                _rangeList = value;
                RaisePropertyChanged("RangeList");
            }
        }

        private TblPromoDetailViewModel _selectedDetailRow;

        public TblPromoDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private TblPromHeaderViewModel _transactionHeader;

        public TblPromHeaderViewModel TransactionHeader
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
                SortBy = "it.GlIserial";

            Client.GetDetailAsync(TransactionHeader.DetailsList.Count, PageSize, TransactionHeader.GlSerial, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        internal void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetevDataAsync(DetailList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #endregion
        /// //////////////////////////////////////////////////criteria
        #region

        internal TblPromoCriteria UpdateAndInsertCriteria()
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(TransactionHeaderCriteria, new ValidationContext(TransactionHeaderCriteria, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblPromoCriteria();
                data.InjectFrom(TransactionHeaderCriteria);
                data.TblStorePromoDetails = new ObservableCollection<TblStorePromoDetail>();
                data.TblBrandPromoDetails = new ObservableCollection<TblBrandPromoDetail>();
                foreach (var item in Stores.Where(x => x.Chek))
                {
                    data.TblStorePromoDetails.Add(new TblStorePromoDetail
                    {
                        TblStores = item.Iserial
                    });
                }

                foreach (var item in Brands.Where(x => x.Chek))
                {
                    data.TblBrandPromoDetails.Add(new TblBrandPromoDetail
                    {
                        Brands = item.Iserial
                    });
                }
                var save = TransactionHeaderCriteria.Glserial == 0;
                if (save)
                {
                    if (AllowAdd)
                    {
                        //Loading = true;
                        data.TblPromoHeader = TransactionHeader.GlSerial;
                        return data;
                        //Client.UpdateOrInsertTblPromoCriteriaAsync(data);
                    }

                    else
                    {
                        MessageBox.Show("You are Not Allowed to Add");
                        return null;
                    }
                }
                else
                {
                    if (AllowUpdate)
                    {
                        //Loading = true;
                        data.TblPromoHeader = TransactionHeader.GlSerial;
                        return data;
                        //Client.UpdateOrInsertTblPromoCriteriaAsync(data);
                    }

                    else
                    {
                        MessageBox.Show("You are Not Allowed to Update");
                        return null;
                    }
                }
                //Loading = true;
            }
            return null;
        }

        public void GetMaindataCriteria()
        {
            Loading = true;

            Client.SearchTblPromoCriteriaAsync(GlserialCriteria, DateCriteria, LoggedUserInfo.DatabasEname);
        }

        public void GetBrands()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetBrandsAsync(Brands.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetBrandsDetial()
        {
            Client.GeTblBrandPromoDetailsAsync(0, PageSize, "it.Glserial", null, null, LoggedUserInfo.DatabasEname);
        }

        public void GetStoresDetail()
        {
            Client.GetStoresDetailsAsync(0, PageSize, "it.Glserial", null, null, LoggedUserInfo.DatabasEname);
        }

        public void GetStores()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Client.GetStoresAsync(Stores.Count, PageSize, "it.Iserial", DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteCriteria()
        {
            var data = new TblPromoCriteria();
            data.InjectFrom(TransactionHeaderCriteria);

            if (AllowDelete)
            {
                Loading = true;
                Client.DeleteTblPromoCriteriaAsync(data, LoggedUserInfo.DatabasEname);
            }
            else
            {
                MessageBox.Show("You are Not Allowed to Delete");
            }
        }

        private int _glserialCriteria;

        public int GlserialCriteria
        {
            get { return _glserialCriteria; }
            set
            {
                _glserialCriteria = value;

                RaisePropertyChanged("GlserialCriteria");
            }
        }

        private string _brand;

        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                RaisePropertyChanged("Brand");
            }
        }

        private DateTime? _fromdateCriteria;

        public DateTime? FromDateCriteria
        {
            get { return _fromdateCriteria; }
            set
            {
                _fromdateCriteria = value;
                RaisePropertyChanged("FromDateCriteria");
            }
        }

        private DateTime? _todateCriteria;

        public DateTime? ToDateCriteria
        {
            get { return _todateCriteria; }
            set
            {
                _todateCriteria = value;
                RaisePropertyChanged("ToDateCriteria");
            }
        }

        private DateTime? _dateCriteria;

        public DateTime? DateCriteria
        {
            get { return _dateCriteria; }
            set
            {
                _dateCriteria = value;
                RaisePropertyChanged("DateCriteria");
            }
        }

        private TblPromoCriteriaViewModel _transactionHeaderCriteria;

        public TblPromoCriteriaViewModel TransactionHeaderCriteria
        {
            get { return _transactionHeaderCriteria; }
            set
            {
                _transactionHeaderCriteria = value;
                RaisePropertyChanged("TransactionHeaderCriteria");
            }
        }

        //private ObservableCollection<CustomerCriteria_Result> _filterList;

        //public ObservableCollection<CustomerCriteria_Result> FilterList
        //{
        //    get { return _filterList; }
        //    set
        //    {
        //        _filterList = value;
        //        RaisePropertyChanged("FilterList");
        //    }
        //}

        private ObservableCollection<TblPromoDetailViewModel> _mobileNo;

        public ObservableCollection<TblPromoDetailViewModel> MobileNo
        {
            get { return _mobileNo; }
            set
            {
                _mobileNo = value;
                RaisePropertyChanged("MobileNo");
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

        private ObservableCollection<TblItemDownLoadDef> _selectedDetailRowsCriteria;

        public ObservableCollection<TblItemDownLoadDef> SelectedDetailRowsCriteria
        {
            get { return _selectedDetailRowsCriteria; }
            set
            {
                _selectedDetailRowsCriteria = value;
                RaisePropertyChanged("SelectedDetailRowsCriteria");
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

        private TblPromoCriteria _selectedMainRowCriteriaCriteria;

        public TblPromoCriteria SelectedMainRowCriteria
        {
            get { return _selectedMainRowCriteriaCriteria; }
            set
            {
                _selectedMainRowCriteriaCriteria = value;
                RaisePropertyChanged("SelectedMainRowCriteria");
            }
        }

        private ObservableCollection<TblPromoCriteria> _mainRowListCriteria;

        public ObservableCollection<TblPromoCriteria> MainRowListCriteria
        {
            get { return _mainRowListCriteria; }
            set
            {
                _mainRowListCriteria = value;
                RaisePropertyChanged("MainRowListCriteria");
            }
        }

        private ObservableCollection<TblDownLoadDefentionViewModel> _brands;

        public ObservableCollection<TblDownLoadDefentionViewModel> Brands
        {
            get { return _brands; }
            set
            {
                _brands = value;
                RaisePropertyChanged("Brands");
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

        #endregion
    }
}