using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblUserBrandSectionPermissionViewModel : PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblUserBrandSectionPermissionViewModel()
        {
            _client.UpdateOrDeleteTblUserBrandSectionPermissionCompleted += (s, sv) => this.InjectFrom(sv.Result);
        }

        private bool _financialEnabledField;

        public bool FinancialEnabled
        {
            get
            {
                return _financialEnabledField;
            }
            set
            {
                if ((_financialEnabledField.Equals(value) != true))
                {
                    _financialEnabledField = value;
                    RaisePropertyChanged("FinancialEnabled");
                }
            }
        }

        private bool _retailEnabledField;

        public bool RetailEnabled
        {
            get
            {
                return _retailEnabledField;
            }
            set
            {
                if ((_retailEnabledField.Equals(value) != true))
                {
                    _retailEnabledField = value;
                    RaisePropertyChanged("RetailEnabled");
                }
            }
        }

        private bool _technicalEnabledField;

        public bool TechnicalEnabled
        {
            get
            {
                return _technicalEnabledField;
            }
            set
            {
                if ((_technicalEnabledField.Equals(value) != true))
                {
                    _technicalEnabledField = value;
                    RaisePropertyChanged("TechnicalEnabled");
                }
            }
        }

        private bool _financialField;

        public bool Financial
        {
            get
            {
                return _financialField;
            }
            set
            {
                if ((_financialField.Equals(value) != true))
                {
                    _financialField = value;
                    RaisePropertyChanged("Financial");
                }
            }
        }

        private bool _retailField;

        public bool Retail
        {
            get
            {
                return _retailField;
            }
            set
            {
                if ((_retailField.Equals(value) != true))
                {
                    _retailField = value;
                    RaisePropertyChanged("Retail");
                }
            }
        }

        private bool _technicalField;

        public bool Technical
        {
            get
            {
                return _technicalField;
            }
            set
            {
                if ((_technicalField.Equals(value) != true))
                {
                    _technicalField = value;
                    RaisePropertyChanged("Technical");
                }
            }
        }

        private int _tblAuthPermissionField;

        private int _tblUserBrandSectionField;

        private bool _checked;

        private string _ename;

        public string Ename
        {
            get { return _ename; }
            set { _ename = value; RaisePropertyChanged("Ename"); }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    _client.UpdateOrDeleteTblUserBrandSectionPermissionAsync(
                        (TblUserBrandSectionPermission)new TblUserBrandSectionPermission().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set
            {
                _updatedAllow = value;
                RaisePropertyChanged("UpdatedAllowed");
            }
        }

        public int TblAuthPermission
        {
            get { return _tblAuthPermissionField; }
            set
            {
                if ((_tblAuthPermissionField.Equals(value) != true))
                {
                    _tblAuthPermissionField = value;
                    RaisePropertyChanged("TblAuthPermission");
                }
            }
        }

        public int TblUserBrandSection
        {
            get { return _tblUserBrandSectionField; }
            set
            {
                if ((_tblUserBrandSectionField.Equals(value) != true))
                {
                    _tblUserBrandSectionField = value;
                    RaisePropertyChanged("TblUserBrandSection");
                }
            }
        }
    }

    public class TblUserBrandSectionViewModel : GenericViewModel
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        private string _brandCode;

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
                    _client.UpdateOrDeleteTblUserBrandSectionAsync(
                        (TblUserBrandSection)new TblUserBrandSection().InjectFrom(this), Checked, 0);
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

        public string BrandCode
        {
            get
            {
                return _brandCode;
            }
            set
            {
                if ((ReferenceEquals(_brandCode, value) != true))
                {
                    _brandCode = value;
                    RaisePropertyChanged("BrandCode");
                }
            }
        }

        private int _tblAuthUser;

        public int TblAuthUser
        {
            get
            {
                return _tblAuthUser;
            }
            set
            {
                if ((_tblAuthUser.Equals(value) != true))
                {
                    _tblAuthUser = value;
                    RaisePropertyChanged("TblAuthUser");
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

        private ObservableCollection<LkpData.TblBrandSectionPermission> _tblBrandSectionPermissions;

        public ObservableCollection<LkpData.TblBrandSectionPermission> TblBrandSectionPermissions
        {
            get { return _tblBrandSectionPermissions; }
            set { _tblBrandSectionPermissions = value; RaisePropertyChanged("TblBrandSectionPermissions"); }
        }
    }

    public class TblAuthUserViewModel : GenericViewModel
    {
        private int? _activeField;

        private int _activeStoreField;

        private string _addressField;

        private string _allowedStoresField;

        private string _axIdField;

        private string _axNameField;

        private string _commentField;

        private short? _currLangField;

        private bool? _isEnabledField;

        private int? _printingCodeField;

        private int? _regNoField;

        private int? _tblJobField;

        private string _tel1Field;

        private string _tel2Field;

        private string _userNameField;

        private string _userPasswordField;

        private string _userDomainField;

        private string _userWinLoginField;

        private int? _wfmDomainidField;

        public int? Active
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

        public int ActiveStore
        {
            get
            {
                return _activeStoreField;
            }
            set
            {
                if ((_activeStoreField.Equals(value) != true))
                {
                    _activeStoreField = value;
                    RaisePropertyChanged("ActiveStore");
                }
            }
        }

        public string Address
        {
            get
            {
                return _addressField;
            }
            set
            {
                if ((ReferenceEquals(_addressField, value) != true))
                {
                    _addressField = value;
                    RaisePropertyChanged("Address");
                }
            }
        }

        public string AllowedStores
        {
            get
            {
                return _allowedStoresField;
            }
            set
            {
                if ((ReferenceEquals(_allowedStoresField, value) != true))
                {
                    _allowedStoresField = value;
                    RaisePropertyChanged("AllowedStores");
                }
            }
        }

        public string AxId
        {
            get
            {
                return _axIdField;
            }
            set
            {
                if ((ReferenceEquals(_axIdField, value) != true))
                {
                    _axIdField = value;
                    RaisePropertyChanged("AxId");
                }
            }
        }

        public string AxName
        {
            get
            {
                return _axNameField;
            }
            set
            {
                if ((ReferenceEquals(_axNameField, value) != true))
                {
                    _axNameField = value;
                    RaisePropertyChanged("AxName");
                }
            }
        }

        public string Comment
        {
            get
            {
                return _commentField;
            }
            set
            {
                if ((ReferenceEquals(_commentField, value) != true))
                {
                    _commentField = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }

        public short? CurrLang
        {
            get
            {
                return _currLangField;
            }
            set
            {
                if ((_currLangField.Equals(value) != true))
                {
                    _currLangField = value;
                    RaisePropertyChanged("CurrLang");
                }
            }
        }

        public bool? IsEnabled
        {
            get
            {
                return _isEnabledField;
            }
            set
            {
                if ((_isEnabledField.Equals(value) != true))
                {
                    _isEnabledField = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public int? PrintingCode
        {
            get
            {
                return _printingCodeField;
            }
            set
            {
                if ((_printingCodeField.Equals(value) != true))
                {
                    _printingCodeField = value;
                    RaisePropertyChanged("PrintingCode");
                }
            }
        }

        public int? RegNo
        {
            get
            {
                return _regNoField;
            }
            set
            {
                if ((_regNoField.Equals(value) != true))
                {
                    _regNoField = value;
                    RaisePropertyChanged("RegNo");
                }
            }
        }

        public int? TblJob
        {
            get
            {
                return _tblJobField;
            }
            set
            {
                if ((_tblJobField.Equals(value) != true))
                {
                    _tblJobField = value;
                    RaisePropertyChanged("TblJob");
                }
            }
        }

        public string Tel1
        {
            get
            {
                return _tel1Field;
            }
            set
            {
                if ((ReferenceEquals(_tel1Field, value) != true))
                {
                    _tel1Field = value;
                    RaisePropertyChanged("Tel1");
                }
            }
        }

        public string Tel2
        {
            get
            {
                return _tel2Field;
            }
            set
            {
                if ((ReferenceEquals(_tel2Field, value) != true))
                {
                    _tel2Field = value;
                    RaisePropertyChanged("Tel2");
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userNameField;
            }
            set
            {
                if ((ReferenceEquals(_userNameField, value) != true))
                {
                    _userNameField = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        public string UserPassword
        {
            get
            {
                return _userPasswordField;
            }
            set
            {
                if ((ReferenceEquals(_userPasswordField, value) != true))
                {
                    _userPasswordField = value;
                    RaisePropertyChanged("UserPassword");
                }
            }
        }

        public string User_Domain
        {
            get
            {
                return _userDomainField;
            }
            set
            {
                if ((ReferenceEquals(_userDomainField, value) != true))
                {
                    _userDomainField = value;
                    RaisePropertyChanged("User_Domain");
                }
            }
        }

        public string User_Win_Login
        {
            get
            {
                return _userWinLoginField;
            }
            set
            {
                if ((ReferenceEquals(_userWinLoginField, value) != true))
                {
                    _userWinLoginField = value;
                    RaisePropertyChanged("User_Win_Login");
                }
            }
        }

        public int? WfmDomainid
        {
            get
            {
                return _wfmDomainidField;
            }
            set
            {
                if ((_wfmDomainidField.Equals(value) != true))
                {
                    _wfmDomainidField = value;
                    RaisePropertyChanged("WFM_DOMAINID");
                }
            }
        }
    }

    public class UserBrandViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public UserBrandViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;
            Client.GetAllBrandsAsync(0);

            Client.GetAllBrandsCompleted += (d, s) =>
            {
                BrandList = s.Result;
            };
            Client.GetAllUsersAsync(0, int.MaxValue, "it.Ename", null, null);
            Client.GetAllUsersCompleted += (d, s) =>
            {
                UsersList = s.Result;
                SelectedUser = s.Result.FirstOrDefault();
            };

            //Client.GetGenericCompleted += (s, sv) =>
            //{
            //    foreach (var row in sv.Result)
            //    {
            //        var newrow = new TblUserBrandSectionViewModel
            //        {
            //            Aname = row.Aname,
            //            Ename = row.Ename,
            //            Iserial = row.Iserial,
            //            Code = row.Code,
            //            TblLkpBrandSection = row.Iserial,
            //            BrandCode = SelectedBrand.Brand_Code,
            //            TblAuthUser = SelectedUser.Iserial,
            //        };
            //        BrandSectionList.Add(newrow);
            //    }
            //};

            lkpClient.GetTblAllBrandSectionLinkCompleted += (s, sv) =>
            {
                BrandSectionList = new ObservableCollection<TblUserBrandSectionViewModel>();
                foreach (var row in sv.Result)
                {
                    var newrow = new TblUserBrandSectionViewModel
                    {
                        Aname = row.TblLkpBrandSection1.Aname,
                        Ename = row.TblLkpBrandSection1.Ename,
                        TblLkpBrandSection = row.TblLkpBrandSection,
                        Code = row.TblLkpBrandSection1.Code,
                        BrandCode = row.TblBrand,
                        TblAuthUser = SelectedUser.Iserial,
                        TblBrandSectionPermissions = row.TblLkpBrandSection1.TblBrandSectionPermissions
                    };
                    BrandSectionList.Add(newrow);
                }
                GetBrandSectionPerUser();
            };

            Client.GetUserSpectialPermissionsAsync();
            Client.GetUserSpectialPermissionsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    PermissionList.Add(new TblUserBrandSectionPermissionViewModel
                    {
                        TblAuthPermission = row.Iserial,
                        Ename = row.Ename,
                    });
                }
            };

            Client.GetTblUserBrandSectionPermissionCompleted += (s, sv) =>
            {
                foreach (var row in PermissionList)
                {
                    row.UpdatedAllowed = false;
                    row.Retail = row.Technical = row.Financial = row.RetailEnabled = row.TechnicalEnabled = row.FinancialEnabled = row.Checked = false;
                    var brandSectionPermissions =
                        SelectedUserBrandSection.TblBrandSectionPermissions.SingleOrDefault(
                            x => x.TblAuthPermission == row.TblAuthPermission && x.BrandCode == SelectedBrand.Brand_Code);

                    var permissionRow = PermissionList.SingleOrDefault(x => x.TblAuthPermission == row.TblAuthPermission

                        );
                    if (permissionRow != null)
                    {
                        if (brandSectionPermissions != null)
                        {
                            permissionRow.FinancialEnabled = brandSectionPermissions.Financial;
                            permissionRow.TechnicalEnabled = brandSectionPermissions.Technical;
                            permissionRow.RetailEnabled = brandSectionPermissions.Retail;
                        }
                    }
                }
                foreach (var row in sv.Result)
                {
                    if (SelectedUserBrandSection != null)
                    {
                        var permissionRow = PermissionList.SingleOrDefault(x => x.TblAuthPermission == row.TblAuthPermission && SelectedUserBrandSection.Iserial == row.TblUserBrandSection
                            );
                        if (permissionRow != null)
                        {
                            permissionRow.Checked = true;
                            permissionRow.TblUserBrandSection = row.Iserial;
                            permissionRow.Retail = row.Retail;
                            permissionRow.Technical = row.Technical;
                            permissionRow.Financial = row.Financial;
                        }
                    }
                }
            };

            Client.GetTblUserBrandSectionCompleted += (s, sv) =>
            {
                foreach (var row in BrandSectionList)
                {
                    row.UpdatedAllowed = false;
                    row.Checked = false;
                }
                foreach (var row in sv.Result)
                {
                    var brandSectionRow = BrandSectionList.SingleOrDefault(x => x.TblLkpBrandSection == row.TblLkpBrandSection && x.TblAuthUser == row.TblAuthUser
                        );
                    if (brandSectionRow != null)
                    {
                        brandSectionRow.Iserial = row.Iserial;
                        brandSectionRow.Checked = true;
                    }
                }
            };
        }

        #region Prop

        private TblAuthUser _selectedUser;

        public TblAuthUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value; RaisePropertyChanged("SelectedUser");
                if (SelectedBrand != null) lkpClient.GetTblAllBrandSectionLinkAsync(SelectedBrand.Brand_Code,  LoggedUserInfo.Iserial);
                GetBrandSectionPerUser();
            }
        }

        private ObservableCollection<TblUserBrandSectionPermissionViewModel> _permissionList;

        public ObservableCollection<TblUserBrandSectionPermissionViewModel> PermissionList
        {
            get { return _permissionList ?? (_permissionList = new ObservableCollection<TblUserBrandSectionPermissionViewModel>()); }
            set { _permissionList = value; RaisePropertyChanged("PermissionList"); }
        }

        private ObservableCollection<TblAuthUser> _usersList;

        public ObservableCollection<TblAuthUser> UsersList
        {
            get { return _usersList ?? (_usersList = new ObservableCollection<TblAuthUser>()); }
            set { _usersList = value; RaisePropertyChanged("UsersList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<TblUserBrandSectionViewModel> _brandSectionList;

        public ObservableCollection<TblUserBrandSectionViewModel> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<TblUserBrandSectionViewModel>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private TblUserBrandSectionViewModel _selectedUserBrandSection;

        public TblUserBrandSectionViewModel SelectedUserBrandSection
        {
            get { return _selectedUserBrandSection; }
            set
            {
                _selectedUserBrandSection = value; RaisePropertyChanged("SelectedUserBrandSection");
                GetUserPermission();
            }
        }

        private Brand _selectedBrand;

        public Brand SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                _selectedBrand = value;

                if (_selectedBrand != null)
                {
                    RaisePropertyChanged("SelectedBrand");
                    lkpClient.GetTblAllBrandSectionLinkAsync(SelectedBrand.Brand_Code,  LoggedUserInfo.Iserial);
                }
            }
        }

        #endregion Prop

        internal void GetUserPermission()
        {
            if (SelectedUserBrandSection != null)
                Client.GetTblUserBrandSectionPermissionAsync(SelectedUserBrandSection.Iserial);
        }

        internal void GetBrandSectionPerUser()
        {
            if (SelectedBrand != null && SelectedUser != null)
                Client.GetTblUserBrandSectionAsync(SelectedBrand.Brand_Code, SelectedUser.Iserial);
        }
    }
}