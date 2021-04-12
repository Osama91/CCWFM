//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
//using CCWFM.CRUDManagerService;
//using CCWFM.Helpers.Enums;
//using CCWFM.Views.DataSettingsForms;
//using CCWFM.Views.Promotions_View;
//using Omu.ValueInjecter.Silverlight;
//using Os.Controls.DataGrid;

//namespace CCWFM.ViewModel.PromotionViewModel
//{
//    //public class TblDownLoadDefentionViewModel:ViewModelBase
//    //{
//    //    private int _Iserial;

//    //    public int Iserial
//    //    {
//    //        get { return _Iserial; }
//    //        set { _Iserial = value; RaisePropertyChanged("Iserial");}
//    //    }

//    //    private string _aname;

//    //    public string Aname
//    //    {
//    //        get { return _aname; }
//    //        set
//    //        {
//    //            _aname = value;

//    //            RaisePropertyChanged("Aname");
//    //        }
//    //    }

//    //    private string _code;

//    //    public string Code
//    //    {
//    //        get { return _code; }
//    //        set
//    //        {
//    //            _code = value;

//    //            RaisePropertyChanged("Code");
//    //        }
//    //    }
//    //    private bool _chek;

//    //    public bool Chek
//    //    {
//    //        get { return _chek; }
//    //        set
//    //        {
//    //            _chek = value;

//    //            RaisePropertyChanged("Chek");
//    //        }
//    //    }

//    //}

//    //public class TblStoreViewModel : ViewModelBase
//    //{
//    //    private int _Iserial;

//    //    public int Iserial
//    //    {
//    //        get { return _Iserial; }
//    //        set { _Iserial = value; RaisePropertyChanged("Iserial"); }
//    //    }

//    //    private string _aname;

//    //    public string aname
//    //    {
//    //        get { return _aname; }
//    //        set
//    //        {
//    //            _aname = value;

//    //            RaisePropertyChanged("aname");
//    //        }
//    //    }

//    //    private string _code;

//    //    public string code
//    //    {
//    //        get { return _code; }
//    //        set
//    //        {
//    //            _code = value;

//    //            RaisePropertyChanged("code");
//    //        }
//    //    }

//    //    private bool _chek;

//    //    public bool Chek
//    //    {
//    //        get { return _chek; }
//    //        set
//    //        {
//    //            _chek = value;

//    //            RaisePropertyChanged("Chek");
//    //        }
//    //    }
//    //}

//    //public class TblPromoCriteriaViewModel:ViewModelBase
//    //{
//    //    CRUD_ManagerServiceClient client= new CRUD_ManagerServiceClient();
//    //    public TblPromoCriteriaViewModel()
//    //    {
//    //    }

//    //    private bool _Enabled;

//    //    public bool Enabled
//    //    {
//    //        get { return _Enabled; }
//    //        set
//    //        {
//    //            _Enabled = value;

//    //            RaisePropertyChanged("Enabled");
//    //        }
//    //    }

//    //    private System.Nullable<System.DateTime> DateFromField;

//    //    private System.Nullable<System.DateTime> DateToField;

//    //    private string DescriptionField;

//    //    private int GlserialField;

//    //    private string NoOfVisitFromField;

//    //    private string NoOfVisitToField;

//    //    private string SalesAmountFromField;

//    //    private string SalesAmountToField;

//    //    private System.Nullable<int> SelectTopField;

//    //    private string TblIteMDownLoadDefField;

//    //    public System.Nullable<System.DateTime> DateFrom
//    //    {
//    //        get
//    //        {
//    //            return this.DateFromField;
//    //        }
//    //        set
//    //        {
//    //            if ((this.DateFromField.Equals(value) != true))
//    //            {
//    //                this.DateFromField = value;
//    //                this.RaisePropertyChanged("DateFrom");
//    //            }
//    //        }
//    //    }

//    //    public System.Nullable<System.DateTime> DateTo
//    //    {
//    //        get
//    //        {
//    //            return this.DateToField;
//    //        }
//    //        set
//    //        {
//    //            if ((this.DateToField.Equals(value) != true))
//    //            {
//    //                this.DateToField = value;
//    //                this.RaisePropertyChanged("DateTo");
//    //            }
//    //        }
//    //    }

//    //    public string Description
//    //    {
//    //        get
//    //        {
//    //            return this.DescriptionField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.DescriptionField, value) != true))
//    //            {
//    //                this.DescriptionField = value;
//    //                this.RaisePropertyChanged("Description");
//    //            }
//    //        }
//    //    }

//    //    public int Glserial
//    //    {
//    //        get
//    //        {
//    //            return this.GlserialField;
//    //        }
//    //        set
//    //        {
//    //            if ((this.GlserialField.Equals(value) != true))
//    //            {
//    //                this.GlserialField = value;
//    //                this.RaisePropertyChanged("Glserial");
//    //            }
//    //        }
//    //    }

//    //    public string NoOfVisitFrom
//    //    {
//    //        get
//    //        {
//    //            return this.NoOfVisitFromField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.NoOfVisitFromField, value) != true))
//    //            {
//    //                this.NoOfVisitFromField = value;
//    //                this.RaisePropertyChanged("NoOfVisitFrom");
//    //            }
//    //        }
//    //    }

//    //    public string NoOfVisitTo
//    //    {
//    //        get
//    //        {
//    //            return this.NoOfVisitToField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.NoOfVisitToField, value) != true))
//    //            {
//    //                //if (value < NoOfVisitFrom)
//    //                //{
//    //                //    value = NoOfVisitFrom;
//    //                //}

//    //                this.NoOfVisitToField = value;
//    //                this.RaisePropertyChanged("NoOfVisitTo");
//    //            }
//    //        }
//    //    }

//    //    public string SalesAmountFrom
//    //    {
//    //        get
//    //        {
//    //            return this.SalesAmountFromField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.SalesAmountFromField, value) != true))
//    //            {
//    //                this.SalesAmountFromField = value;
//    //                this.RaisePropertyChanged("SalesAmountFrom");
//    //            }
//    //        }
//    //    }

//    //    public string SalesAmountTo
//    //    {

//    //        get
//    //        {
//    //            return this.SalesAmountToField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.SalesAmountToField, value) != true))
//    //            {
//    //                this.SalesAmountToField = value;
//    //                this.RaisePropertyChanged("SalesAmountTo");
//    //            }
//    //        }
//    //    }

//    //    public System.Nullable<int> SelectTop
//    //    {
//    //        get
//    //        {
//    //            return this.SelectTopField;
//    //        }
//    //        set
//    //        {
//    //            if ((this.SelectTopField.Equals(value) != true))
//    //            {
//    //                this.SelectTopField = value;
//    //                this.RaisePropertyChanged("SelectTop");
//    //            }
//    //        }
//    //    }

//    //    public string TblIteMDownLoadDef
//    //    {
//    //        get
//    //        {
//    //            return this.TblIteMDownLoadDefField;
//    //        }
//    //        set
//    //        {
//    //            if ((object.ReferenceEquals(this.TblIteMDownLoadDefField, value) != true))
//    //            {
//    //                this.TblIteMDownLoadDefField = value;
//    //                this.RaisePropertyChanged("TblIteMDownLoadDef");
//    //            }
//    //        }
//    //    }  private TblItemDownLoadDef _itemperrow;
//    //    public TblItemDownLoadDef Itemperrow
//    //    {
//    //        get
//    //        {
//    //            return _itemperrow;
//    //        }
//    //        set
//    //        {
//    //            if ((ReferenceEquals(_itemperrow, value) != true))
//    //            {
//    //                _itemperrow = value;
//    //                this.RaisePropertyChanged("Itemperrow");
//    //                if (Itemperrow != null) TblIteMDownLoadDef = Itemperrow.Aname;
//    //            }
//    //        }

//    //    }

//    //}

//    public class PromoCriteriaViewModel : ViewModelBase
//    {
//        public PromoCriteriaViewModel()
//        {
//            GetItemPermissions(PermissionItemName.PromotionForm.ToString());
//            GetCustomePermissions(PermissionItemName.PromotionForm.ToString());
//            SelectedDetailRowsCriteria = new SortableCollectionView<TblItemDownLoadDef>();
//            TransactionHeaderCriteria = new TblPromoCriteriaViewModel();
//            MainRowListCriteria = new SortableCollectionView<TblPromoCriteria>();
//            Brands = new ObservableCollection<TblDownLoadDefentionViewModel>();
//            StoreList=new ObservableCollection<TblStore>();
//            Stores= new ObservableCollection<TblStoreViewModel>();
//            MobileNo = new ObservableCollection<TblPromoDetailViewModel>();
//            Client.GetStoresDetailsCompleted += (s, sv) =>
//            {
//                  foreach (var row in sv.Result)
//                  {
//                      var tblStoreViewModel = Stores.FirstOrDefault(x => x.Iserial == row.TblStores);
//                      if (tblStoreViewModel != null)
//                          tblStoreViewModel.Chek = true;
//                  }

//                Loading = false;

//            };

//            Client.GetCustomerCriteriaCompleted += (s, sv) =>
//            {
//                foreach (var ITEM in sv.Result)
//                {
//                 MobileNo.Add(new TblPromoDetailViewModel()

//                 {
//                     MobileNo = ITEM.customerMobile,

//                 }
//                     );
//                }
//            };
//            Client.GeTblBrandPromoDetailsCompleted += (s, sv) =>
//            {
//                foreach (var row in sv.Result)
//                {
//                    var tblitemdefention = Stores.FirstOrDefault(x => x.Iserial == row.Brands);
//                    if (tblitemdefention != null)
//                        tblitemdefention.Chek = true;
//                }

//                DetailFullCount = sv.fullCount;
//                Loading = false;
//            };

//            Client.GetBrandsCompleted += (s, sv) =>
//            {
//                foreach (var row in sv.Result)
//                {
//                    var newrow = new TblDownLoadDefentionViewModel();
//                    newrow.InjectFrom(row);

//                    Brands.Add(newrow);
//                }

//                DetailFullCount = sv.fullCount;
//                Loading = false;
//            };

//            Client.GetStoresCompleted += (s, sv) =>
//            {
//                foreach (var row in sv.Result)
//                {
//                    var newrow = new TblStoreViewModel();
//                    newrow.InjectFrom(row);

//                    Stores.Add(newrow);
//                }

//                DetailFullCount = sv.fullCount;
//                Loading = false;
//            };
//            Client.SearchTblPromoCriteriaCompleted += (y, v) =>
//            {
//                foreach (var row in v.Result)
//                {
//                    Loading = false;
//                    var newrow = new TblPromoCriteria();
//                    newrow.InjectFrom(row);

//                    MainRowListCriteria.Add(newrow);
//                }
//                Loading = false;
//            };

//            Client.UpdateOrInsertTblPromoCriteriaCompleted += (s, x) =>
//            {
//                if (x.Error == null)
//                {
//                    TransactionHeaderCriteria.InjectFrom(x.Result);
//                    //SaveMainRowExc();
//                    MessageBox.Show("Saved Successfully");
//                    Client.GetCustomerCriteriaAsync(TransactionHeaderCriteria.Glserial);
//                }
//                else
//                {
//                    MessageBox.Show(x.Error.Message);
//                }
//                //   Loading = false;

//                //TransactionHeaderCriteria.InjectFrom(x.Result);
//            };

//            Client.DeleteTblPromoCriteriaCompleted += (w, k) =>
//            {
//                Loading = false;
//                TransactionHeaderCriteria = new TblPromoCriteriaViewModel();
//            };
//            GetBrands();
//            GetStores();

//            //Client.SalesAmountCompleted += (x, b) =>
//            //{
//            //    DetailFilter = Convert.ToString(b.Result);
//            //};
//        }

//        //public void GetFilterdDate()
//        //{
//        //Client.SalesAmountAsync(FromDate,ToDate,Brand);

//        //}

//        public void UpdateAndInsertCriteria()
//        {
//            var valiationCollection = new List<ValidationResult>();
//            bool isvalid = Validator.TryValidateObject(TransactionHeaderCriteria, new ValidationContext(TransactionHeaderCriteria, null, null), valiationCollection, true);
//            if (isvalid)
//            {
//                var data = new TblPromoCriteria();
//                data.InjectFrom(TransactionHeaderCriteria);
//                data.TblStorePromoDetails = new ObservableCollection<TblStorePromoDetail>();
//                data.TblBrandPromoDetails = new ObservableCollection<TblBrandPromoDetail>();
//                foreach (var item in Stores.Where(x=>x.Chek))
//                {
//                    data.TblStorePromoDetails.Add( new TblStorePromoDetail()
//                    {
//                        TblStores = item.Iserial
//                    });

//                }

//                foreach (var item in Brands.Where(x => x.Chek))
//                {
//                    data.TblBrandPromoDetails.Add(new TblBrandPromoDetail()
//                    {
//                        Brands = item.Iserial
//                    });

//                }
//                bool save = TransactionHeaderCriteria.Glserial == 0;
//                if (save)
//                {
//                    if (AllowAdd)
//                    {
//                        Loading = true;
//                        Client.UpdateOrInsertTblPromoCriteriaAsync(data);

//                    }

//                    else
//                    {
//                        MessageBox.Show("You are Not Allowed to Add");

//                    }

//                }
//                else
//                {
//                    if (AllowUpdate)
//                    {
//                        Loading = true;
//                        Client.UpdateOrInsertTblPromoCriteriaAsync(data);
//                    }

//                    else
//                    {
//                        MessageBox.Show("You are Not Allowed to Update");

//                    }
//                }
//                Loading = true;

//            }
//        }

//        public void GetMaindataCriteria()
//        {
//            Loading = true;

//            Client.SearchTblPromoCriteriaAsync(GlserialCriteria, FromDateCriteria);
//        }

//        public void GetBrands()
//        {
//            if (DetailSortBy == null)
//                DetailSortBy = "it.Iserial";

//            Client.GetBrandsAsync(Brands.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
//        }
//        public void GetBrandsDetial()
//        {
//            Client.GeTblBrandPromoDetailsAsync(0, PageSize, "it.Glserial", null, null);
//        }
//        public void GetStoresDetail()
//        {
//            Client.GetStoresDetailsAsync(0, PageSize, "it.Glserial", null, null);
//        }

//        public void GetStores()
//        {
//            if (DetailSortBy == null)
//                DetailSortBy = "it.Iserial";

//            Client.GetStoresAsync(Stores.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
//        }

//        public void DeleteCriteria()
//        {
//            var data = new TblPromoCriteria();
//            data.InjectFrom(TransactionHeaderCriteria);

//            if (AllowDelete)
//            {
//                Loading = true;
//                Client.DeleteTblPromoCriteriaAsync(data);

//            }
//            else
//            {
//                MessageBox.Show("You are Not Allowed to Delete");
//            }
//        }

//        private int _glserialCriteria;

//        public int GlserialCriteria
//          {
//              get { return _glserialCriteria; }
//              set
//              {
//                  _glserialCriteria = value;

//                  RaisePropertyChanged("GlserialCriteria");
//              }
//          }

//         private string _brand ;

//        public string Brand
//        {
//            get { return _brand; }
//            set
//            {
//                _brand = value;
//                RaisePropertyChanged("Brand");
//            }
//        }
//        private DateTime? _fromdateCriteria;

//            public DateTime? FromDateCriteria
//        {
//            get { return _fromdateCriteria; }
//            set
//            {
//                _fromdateCriteria = value;
//                RaisePropertyChanged("FromDateCriteria");
//            }
//        }

//            private DateTime _todateCriteria;

//            public DateTime ToDateCriteria
//        {
//            get { return _todateCriteria; }
//            set
//            {
//                _todateCriteria = value;
//                RaisePropertyChanged("ToDateCriteria");
//            }
//        }

//        private TblPromoCriteriaViewModel _TransactionHeaderCriteria;

//        public TblPromoCriteriaViewModel TransactionHeaderCriteria
//        {
//            get { return _TransactionHeaderCriteria; }
//            set
//            {
//                _TransactionHeaderCriteria = value;
//                RaisePropertyChanged("TransactionHeaderCriteria");
//            }
//        }

//        //private ObservableCollection<CustomerCriteria_Result> _filterList;

//        //public ObservableCollection<CustomerCriteria_Result> FilterList
//        //{
//        //    get { return _filterList; }
//        //    set
//        //    {
//        //        _filterList = value;
//        //        RaisePropertyChanged("FilterList");
//        //    }
//        //}

//        private ObservableCollection<TblPromoDetailViewModel> _mobileNo;

//        public ObservableCollection<TblPromoDetailViewModel> MobileNo
//        {
//            get { return _mobileNo; }
//            set
//            {
//                _mobileNo = value;
//                RaisePropertyChanged("MobileNo");
//            }
//        }

//        private ObservableCollection<TblStore> _storeList;

//        public ObservableCollection<TblStore> StoreList
//        {
//            get { return _storeList; }
//            set
//            {
//                _storeList = value;
//                RaisePropertyChanged("StoreList");
//            }
//        }

//        private SortableCollectionView<TblItemDownLoadDef> _SelectedDetailRowsCriteria;

//        public SortableCollectionView<TblItemDownLoadDef> SelectedDetailRowsCriteria
//        {
//            get { return _SelectedDetailRowsCriteria; }
//            set
//            {
//                _SelectedDetailRowsCriteria = value;
//                RaisePropertyChanged("SelectedDetailRowsCriteria");
//            }
//        }

//        private TblPromoCriteria _selectedMainRowCriteriaCriteria;

//        public TblPromoCriteria SelectedMainRowCriteria
//        {
//            get { return _selectedMainRowCriteriaCriteria; }
//            set
//            {
//                _selectedMainRowCriteriaCriteria = value;
//                RaisePropertyChanged("SelectedMainRowCriteria");
//            }
//        }

//        private SortableCollectionView<TblPromoCriteria> _MainRowListCriteria;

//        public SortableCollectionView<TblPromoCriteria> MainRowListCriteria
//        {
//            get { return _MainRowListCriteria; }
//            set
//            {
//                _MainRowListCriteria = value;
//                RaisePropertyChanged("MainRowListCriteria");
//            }
//        }
//        private ObservableCollection<TblDownLoadDefentionViewModel> _brands;

//        public ObservableCollection<TblDownLoadDefentionViewModel> Brands
//        {
//            get { return _brands; }
//            set
//            {
//                _brands = value;
//                RaisePropertyChanged("Brands");
//            }
//        }

//        private ObservableCollection<TblStoreViewModel> _stores;

//        public ObservableCollection<TblStoreViewModel> Stores
//        {
//            get { return _stores; }
//            set
//            {
//                _stores = value;
//                RaisePropertyChanged("Stores");
//            }
//        }

//    }
//}