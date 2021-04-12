using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblFamilyLinkViewModel : TblFamilyViewModel
    {
        // ReSharper disable once InconsistentNaming
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        private string _tblBrandField;

        private int _tblFamilyField;

        private int _tblLkpBrandSectionField;

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    TblFamily = Iserial;
                    lkpClient.UpdateOrDeleteTblFamilyLinkAsync((LkpData.TblFamilyLink)new LkpData.TblFamilyLink().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        public string TblBrand
        {
            get
            {
                return _tblBrandField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandField, value) != true))
                {
                    _tblBrandField = value;
                    RaisePropertyChanged("TblBrand");
                }
            }
        }

        public int TblFamily
        {
            get
            {
                return _tblFamilyField;
            }
            set
            {
                if ((_tblFamilyField.Equals(value) != true))
                {
                    _tblFamilyField = value;
                    RaisePropertyChanged("TblFamily");
                }
            }
        }

        public int TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSectionField;
            }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        private ObservableCollection<TblSubFamilyLinkViewModel> _tblSubProductGroupField;

        public new ObservableCollection<TblSubFamilyLinkViewModel> DetailsList
        {
            get
            {
                return _tblSubProductGroupField ?? (_tblSubProductGroupField = new ObservableCollection<TblSubFamilyLinkViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblSubProductGroupField, value) != true))
                {
                    _tblSubProductGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }
    }

    public class TblSubFamilyLinkViewModel : TblSubFamily
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        private string _tblBrandField;

        private int _tblLkpBrandSectionField;

        private int _tblSubFamilyField;
        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    TblSubFamily = Iserial;
                    lkpClient.UpdateOrDeleteTblSubFamilyLinkAsync((LkpData.TblSubFamilyLink)new LkpData.TblSubFamilyLink().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        public string TblBrand
        {
            get
            {
                return _tblBrandField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandField, value) != true))
                {
                    _tblBrandField = value;
                    RaisePropertyChanged("TblBrand");
                }
            }
        }

        public int TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSectionField;
            }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        public int TblSubFamily
        {
            get
            {
                return _tblSubFamilyField;
            }
            set
            {
                if ((_tblSubFamilyField.Equals(value) != true))
                {
                    _tblSubFamilyField = value;
                    RaisePropertyChanged("TblSubFamily");
                }
            }
        }
    }

    public class TblLkpBrandSectionLinkViewModel : GenericViewModel
    {
        // ReSharper disable once InconsistentNaming
        //private readonly CRUD_ManagerServiceClient client = new CRUD_ManagerServiceClient();
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        private string _tblBrandField;

        private string _shortCode;

        public string ShortCode
        {
            get { return _shortCode; }
            set { _shortCode = value; RaisePropertyChanged("ShortCode"); }
        }

        private bool _useCategory;

        public bool UseCategory
        {
            get { return _useCategory; }
            set { _useCategory = value; RaisePropertyChanged("UseCategory"); }
        }

        private bool _useDirection;

        public bool UseDirection
        {
            get { return _useDirection; }
            set { _useDirection = value; RaisePropertyChanged("UseDirection"); }
        }

        private int _tblLkpBrandSectionField;

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    lkpClient.UpdateOrDeleteTblLkpBrandSectionLinkAsync(
                        (LkpData.TblLkpBrandSectionLink)new LkpData.TblLkpBrandSectionLink().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        public string TblBrand
        {
            get
            {
                return _tblBrandField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandField, value) != true))
                {
                    _tblBrandField = value;
                    RaisePropertyChanged("TblBrand");
                }
            }
        }

        private string _tblItemDownLoadDef;

        public string TblItemDownLoadDef
        {
            get
            {
                return _tblItemDownLoadDef;
            }
            set
            {
                if ((ReferenceEquals(_tblItemDownLoadDef, value) != true))
                {
                    _tblItemDownLoadDef = value;
                    RaisePropertyChanged("TblItemDownLoadDef");
                }
            }
        }

        public int TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSectionField;
            }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        private int _retailWarehouse;

        public int RetailWarehouse
        {
            get
            {
                return _retailWarehouse;
            }
            set
            {
                if ((_retailWarehouse.Equals(value) != true))
                {
                    _retailWarehouse = value;
                    RaisePropertyChanged("RetailWarehouse");
                }
            }
        }

        private TblStore _storesPerRow;

        public TblStore StoresPerRow
        {
            get { return _storesPerRow; }
            set
            {
                _storesPerRow = value; RaisePropertyChanged("StoresPerRow");

                if (_storesPerRow != null) RetailWarehouse = _storesPerRow.iserial;
            }
        }

        private ObservableCollection<TblStore> _stores;

        public ObservableCollection<TblStore> Stores
        {
            get { return _stores; }
            set { _stores = value; RaisePropertyChanged("Stores"); }
        }
    }

    #endregion ViewModels

    public class FamilyLinkViewModel : ViewModelBase
    {
        public event EventHandler StoreCompleted;
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public FamilyLinkViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                SelectedBrandSection = new TblLkpBrandSectionLinkViewModel();
                BrandSectionList = new ObservableCollection<TblLkpBrandSectionLinkViewModel>();
                MainRowList = new ObservableCollection<TblFamilyLinkViewModel>();
                SelectedMainRow = new TblFamilyLinkViewModel();
                SelectedDetailRow = new TblSubFamilyLinkViewModel();
                Client.SearchRetailStoresCompleted += (s, sv) =>
                {
                    SelectedBrandSection.Stores = sv.Result;
                    OnStoreCompleted();
                };

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandsList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
                Client.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLkpBrandSectionLinkViewModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code
                        };
                        BrandSectionList.Add(newrow);
                    }
                };
                Client.GetGenericAsync("TblLkpBrandSection", "%%", "%%", "%%", "Iserial", "ASC");

                lkpClient.GetTblFamilyLinkCompleted += (s, sv) =>
                {
                    MainSelectedRows.Clear();
                    MainSelectedRows = sv.Result;

                    foreach (var row in MainRowList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                    }
                    foreach (var row in MainSelectedRows)
                    {
                        var familyRow = MainRowList.SingleOrDefault(x =>
                            x.Iserial == row.TblFamily);
                        familyRow.Checked = true;
                    }

                    if (_brandCode != null && SelectedBrandSection != null && SelectedMainRow != null)
                    {
                        lkpClient.GetTblSubFamilyLinkAsync(SelectedMainRow.Iserial, BrandCode,
                            SelectedBrandSection.Iserial);
                    }
                };
                lkpClient.GetTblSubFamilyLinkCompleted += (s, sv) =>
                {
                    SubSelectedRows.Clear();
                    SubSelectedRows = sv.Result;

                    foreach (var row in SelectedMainRow.DetailsList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                    }
                    foreach (var row in SubSelectedRows)
                    {
                        var subfamilyRow = SelectedMainRow.DetailsList.SingleOrDefault(x =>
                            x.Iserial == row.TblSubFamily);
                        if (subfamilyRow != null) subfamilyRow.Checked = true;
                    }
                };
                lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                {
                    foreach (var row in BrandSectionList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                    }

                    foreach (var row in sv.Result)
                    {
                        var brandSectionRow = BrandSectionList.SingleOrDefault(x => x.Iserial == row.TblLkpBrandSection
                            );
                        if (brandSectionRow != null)
                        {
                            brandSectionRow.Checked = true;
                            brandSectionRow.ShortCode = row.ShortCode;
                            brandSectionRow.UseCategory = row.UseCategory;
                            brandSectionRow.UseDirection = row.UseDirection;
                        }
                    }
                };

                lkpClient.GetTblFamilyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblFamilyLinkViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };
                lkpClient.GetTblSubFamilyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSubFamilyLinkViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;

                    if (_brandCode != null && SelectedBrandSection != null && SelectedMainRow != null)
                    {
                        lkpClient.GetTblSubFamilyLinkAsync(SelectedMainRow.Iserial, BrandCode,
                            SelectedBrandSection.Iserial);
                    }
                };

                GetMaindata();
            }
        }

        protected virtual void OnStoreCompleted()
        {
            var handler = StoreCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            lkpClient.GetTblFamilyAsync(MainRowList.Count, 1000, SortBy, Filter, ValuesObjects);
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            if (SelectedMainRow != null)
                lkpClient.GetTblSubFamilyAsync(SelectedMainRow.DetailsList.Count, 1000, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        #region Prop

        private ObservableCollection<Brand> _brandsList;

        public ObservableCollection<Brand> BrandsList
        {
            get { return _brandsList; }
            set { _brandsList = value; RaisePropertyChanged("BrandsList"); }
        }

        private string _brandCode;

        public string BrandCode
        {
            get { return _brandCode; }
            set
            {
                _brandCode = value;

                if (_brandCode != null)
                {
                    RaisePropertyChanged("BrandCode");
                    if (SelectedBrandSection != null)
                    {
                        lkpClient.GetTblBrandSectionLinkAsync(BrandCode, LoggedUserInfo.Iserial);
                        lkpClient.GetTblFamilyLinkAsync(BrandCode, SelectedBrandSection.Iserial);
                    }
                }
            }
        }

        public void SectionChanged()
        {
            if (_brandCode != null)
            {
                lkpClient.GetTblFamilyLinkAsync(BrandCode, SelectedBrandSection.Iserial);
            }
        }

        private ObservableCollection<LkpData.TblFamilyLink> _mainSelectedRows;

        public ObservableCollection<LkpData.TblFamilyLink> MainSelectedRows
        {
            get { return _mainSelectedRows ?? (_mainSelectedRows = new ObservableCollection<LkpData.TblFamilyLink>()); }
            set { _mainSelectedRows = value; RaisePropertyChanged("MainSelectedRows"); }
        }

        private ObservableCollection<LkpData.TblSubFamilyLink> _subSelectedRows;

        public ObservableCollection<LkpData.TblSubFamilyLink> SubSelectedRows
        {
            get { return _subSelectedRows ?? (_subSelectedRows = new ObservableCollection<LkpData.TblSubFamilyLink>()); }
            set { _subSelectedRows = value; RaisePropertyChanged("subSelectedRows"); }
        }

        private ObservableCollection<TblLkpBrandSectionLinkViewModel> _brandSectionList;

        public ObservableCollection<TblLkpBrandSectionLinkViewModel> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<TblLkpBrandSectionLinkViewModel>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private TblLkpBrandSectionLinkViewModel _selectedBrandSection;

        public TblLkpBrandSectionLinkViewModel SelectedBrandSection
        {
            get { return _selectedBrandSection; }
            set { _selectedBrandSection = value; RaisePropertyChanged("SelectedBrandSection"); }
        }

        private ObservableCollection<TblFamilyLinkViewModel> _mainRowList;

        public ObservableCollection<TblFamilyLinkViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new ObservableCollection<TblFamilyLinkViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private TblFamilyLinkViewModel _selectedMainRow;

        public TblFamilyLinkViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblSubFamilyLinkViewModel _selectedDetailRow;

        public TblSubFamilyLinkViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop

        internal void SearchForStore(string store)
        {
            Client.SearchRetailStoresAsync(store);
        }
    }
}