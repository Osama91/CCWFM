using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.AuthService;
using CCWFM.Web.DataLayer;
using GalaSoft.MvvmLight.Command;

namespace CCWFM.ViewModel.OGViewModels
{
    public class UserJobsViewModel : ViewModelBase
    {
        AuthService.AuthServiceClient AuthClient;
        public UserJobsViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                AuthClient = Helpers.Services.Instance.GetAuthServiceClient();
                Jobs = new ObservableCollection<TblAuthJob>();
                BarcodeList = new ObservableCollection<BarCodeSettingsList>();
                DetailList = new ObservableCollection<EmployeesView>();

                AuthClient.GetBarcodeDisplaySettingsHeaderCompleted += (s, sv) =>
                {
                    foreach (var item in sv.Result)
                    {
                        BarcodeList.Add(MapToBarcode(item));
                    }
                };
                AuthClient.GetBarcodeDisplaySettingsHeaderAsync();
                Client.GetGenericAsync("TblCompany", "%%", "%%", "%%", "Iserial", "ASC");
                Client.GetGenericCompleted += (s, sv) =>
                {
                    CompanyList = sv.Result;
                };
                AuthClient.SaveUserCompleted += (s, sv) =>
                {
                    var savedRow = MainRowList.ElementAt(sv.outindex);
                    if (savedRow != null)
                    {
                        savedRow.Iserial = sv.Result.Iserial;
                        MessageBox.Show("Saved Successfully");
                    }
                    else if (sv.Result.Ename == "ERROR_OCCURED")
                    {
                        MessageBox.Show("Error Occured, User NOT saved");
                    }
                };
                _axUsers = new ObservableCollection<AuthService.User>();
                Language = new List<Language>
                {
                    new Language {CurrLan = 0, Lang = "Arabic"},
                    new Language {CurrLan = 1, Lang = "English"}
                };

                AuthClient.GetEmpTableCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        DetailList.Add(row);
                    }
                    DetailFullCount = sv.fullCount;
                    Loading = false;
                };

                MainRowList = new ObservableCollection<TblUserViewModel>();
                AuthClient.GetAllJobsAsync();
                AuthClient.GetAllJobsCompleted += (s, j) =>
                {
                    foreach (var item in j.Result)
                    {
                        Jobs.Add(item);
                    }
                };

                AuthClient.GetAxUserAsync();
                AuthClient.GetAxUserCompleted += (s, j) =>
                {
                    foreach (var item in j.Result)
                    {
                        AxUsers.Add(item);
                    }
                };

                AuthClient.GetAllUsersCompleted += (s, ds) =>
                {
                    foreach (var item in ds.Result)
                    {
                        var newRow = new TblUserViewModel();
                        newRow.InjectFrom(item);
                        newRow.CompanyPerRow = new CRUDManagerService.GenericTable();
                        newRow.SecondaryCompanyPerRow = new CRUDManagerService.GenericTable();
                        newRow.PositionPerRow = new CRUDManagerService.GenericTable();
                        newRow.BarcodePerRow = new BarCodeSettingsList();
                        if (item.TblCompany1 != null) newRow.CompanyPerRow.InjectFrom(item.TblCompany1);
                        if (item.TblCompany2 != null) newRow.SecondaryCompanyPerRow.InjectFrom(item.TblCompany2);
                        if (item.TblPosition1 != null) newRow.PositionPerRow.InjectFrom(item.TblPosition1);
                        newRow.BarcodePerRow = BarcodeList.FirstOrDefault(x => x.Iserial == newRow.PrintingCode);
                        MainRowList.Add(newRow);
                        newRow.JobPerRow = item.TblAuthJob;
                    }
                    Loading = false;
                };
                GetMaindata();

                AuthClient.GetLookUpAuthWarehouseTypesCompleted += (s, e) =>
                {
                    PermissionTypes = e.Result;
                };
                AuthClient.GetLookUpAuthWarehouseTypesAsync();
                AuthClient.GetAuthWarehousesCompleted += (s, e) =>
                {
                    WarehouseAuths = e.Result;
                };
                AuthClient.GetAuthJournalSettingCompleted += (s, e) =>
                {
                    JournalAuth = e.Result;
                };
                SavePermissions = new RelayCommand(() =>
                {
                    if (SelectedPermissionType != null && SelectedMainRow != null)
                        AuthClient.SaveAuthWarehousesAsync(SelectedMainRow.Iserial,
                        SelectedPermissionType.Iserial, WarehouseAuths);
                });


                SaveJournalAuth = new RelayCommand(() =>
                {
                    if (SelectedMainRow != null)
                    {
                        string comp = "";
                        if (SelectedMainRow.SecondaryCompanyPerRow != null)
                        {
                            comp = SelectedMainRow.SecondaryCompanyPerRow.Code;
                        }
                        if (comp == "")
                        {
                            if (SelectedMainRow.CompanyPerRow != null)
                            {
                                comp = SelectedMainRow.CompanyPerRow.Code;
                            }

                        }
                        AuthClient.SaveAuthJournalSettingAsync(SelectedMainRow.Iserial, comp, JournalAuth);
                    }
                });
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Ename";

            Loading = true;
            AuthClient.GetAllUsersAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            AuthClient.DeleteUserAsync(row.Iserial);
                            Loading = true;
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
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
                var newrow = new TblUserViewModel();
                MainRowList.Insert(currentRowIndex + 1, newrow);
                SelectedMainRow = newrow;
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
                    var saveRow = new AuthService.TblAuthUser();
                    saveRow.InjectFrom(SelectedMainRow);
                    AuthClient.SaveUserAsync(saveRow, MainRowList.IndexOf(SelectedMainRow));
                    Loading = true;
                }
            }
        }

        public void ApplyUser(AuthService.User axUser)
        {
            SelectedMainRow.AxName = axUser.NAME;
            SelectedMainRow.AxId = axUser.ID;
            SelectedMainRow.User_Domain = axUser.User_Domain;
            SelectedMainRow.User_Win_Login = axUser.User_Win_Login;
        }

        public void ApplyUser()
        {
            SelectedMainRow.Code = SelectedDetailRow.Emplid;
            SelectedMainRow.Aname = SelectedDetailRow.Name;
            SelectedMainRow.Ename = SelectedDetailRow.Name;
            SelectedMainRow.UserName = SelectedDetailRow.Emplid;
            SelectedMainRow.UserPassword = SelectedDetailRow.Emplid;
            //	SelectedMainRow.PositionPerRow = PositionList.SingleOrDefault(x => x.Ename == SelectedDetailRow.Position);
            if (SelectedMainRow.PositionPerRow != null) SelectedMainRow.TblPosition = SelectedMainRow.PositionPerRow.Iserial;
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Emplid";

            AuthClient.GetEmpTableAsync(DetailList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        private BarCodeSettingsList MapToBarcode(BarcodeDisplaySettingsHeader barCode)
        {
            return new BarCodeSettingsList
            {
                PrintingCode = barCode.Code,
                Iserial = barCode.Iserial,
            };
        }

        #region Props

        private ObservableCollection<BarCodeSettingsList> _barcodeList;
        public ObservableCollection<BarCodeSettingsList> BarcodeList
        {
            get { return _barcodeList; }
            set
            {
                _barcodeList = value;
                RaisePropertyChanged("BarcodeList");
            }
        }

        private ObservableCollection<TblUserViewModel> _mainRowList;
        public ObservableCollection<TblUserViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<AuthService.User> _axUsers;
        public ObservableCollection<AuthService.User> AxUsers
        {
            get { return _axUsers; }
            set
            {
                _axUsers = value;
                RaisePropertyChanged("AxUsers");
            }
        }

        private ObservableCollection<TblAuthJob> _jobs;
        public ObservableCollection<TblAuthJob> Jobs
        {
            get { return _jobs; }
            set
            {
                _jobs = value;
                RaisePropertyChanged("Jobs");
            }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _companyList;
        public ObservableCollection<CRUDManagerService.GenericTable> CompanyList
        {
            get { return _companyList; }
            set
            {
                _companyList = value;
                RaisePropertyChanged("CompanyList");
            }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _positionList;
        public ObservableCollection<CRUDManagerService.GenericTable> PositionList
        {
            get { return _positionList; }
            set
            {
                _positionList = value;
                RaisePropertyChanged("PositionList");
            }
        }

        private List<Language> _lan;
        public List<Language> Language
        {
            get { return _lan; }
            set
            {
                _lan = value;
                RaisePropertyChanged("Language");
            }
        }

        private ObservableCollection<EmployeesView> _detailList;
        public ObservableCollection<EmployeesView> DetailList
        {
            get { return _detailList; }
            set
            {
                _detailList = value;
                RaisePropertyChanged("DetailList");
            }
        }

        private TblUserViewModel _selectedMainRow;
        public TblUserViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                if (SelectedPermissionType != null && SelectedMainRow != null)
                {
                    AuthClient.GetAuthWarehousesAsync(SelectedMainRow.Iserial, SelectedPermissionType.Iserial);
                }

                if (SelectedMainRow.CompanyPerRow != null)
                {
                    string comp = "";

                    if (SelectedMainRow.SecondaryCompanyPerRow != null)
                    {
                        if (SelectedMainRow.SecondaryCompanyPerRow.Code != null)
                        {
                            comp = SelectedMainRow.SecondaryCompanyPerRow.Code;
                        }
                    }


                    if (comp == "")
                    {
                        comp = SelectedMainRow.CompanyPerRow.Code;
                    }
                    if (comp != "HQ")
                    {
                        AuthClient.GetAuthJournalSettingAsync(SelectedMainRow.Iserial, comp);
                    }

                }
            }
        }

        private ObservableCollection<TblUserViewModel> _selectedMainRows;
        public ObservableCollection<TblUserViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblUserViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private EmployeesView _selectedDetailRow;
        public EmployeesView SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<SearchForStore.TblStoreViewModel> _storeList;
        public ObservableCollection<SearchForStore.TblStoreViewModel> StoreList
        {
            get { return _storeList ?? (_storeList = new ObservableCollection<SearchForStore.TblStoreViewModel>()); }
            set
            {
                _storeList = value;
                RaisePropertyChanged("StoreList");
            }
        }


        private TblAuthWarehousePermissionType selectedPermissionType;
        public TblAuthWarehousePermissionType SelectedPermissionType
        {
            get { return selectedPermissionType; }
            set
            {
                selectedPermissionType = value; RaisePropertyChanged(nameof(SelectedPermissionType));
                if (SelectedPermissionType != null && SelectedMainRow != null)
                    AuthClient.GetAuthWarehousesAsync(SelectedMainRow.Iserial, SelectedPermissionType.Iserial);
            }
        }

        private ObservableCollection<TblAuthWarehousePermissionType> permissionTypes;
        public ObservableCollection<TblAuthWarehousePermissionType> PermissionTypes
        {
            get { return permissionTypes; }
            set { permissionTypes = value; RaisePropertyChanged(nameof(PermissionTypes)); }
        }


        private ObservableCollection<Models.Inv.AuthWarehouseModel> warehouseAuths;
        public ObservableCollection<Models.Inv.AuthWarehouseModel> WarehouseAuths
        {
            get { return warehouseAuths; }
            set { warehouseAuths = value; RaisePropertyChanged(nameof(WarehouseAuths)); }
        }

        private ObservableCollection<Models.Inv.AuthWarehouseModel> _JournalAuth;
        public ObservableCollection<Models.Inv.AuthWarehouseModel> JournalAuth
        {
            get { return _JournalAuth; }
            set { _JournalAuth = value; RaisePropertyChanged(nameof(JournalAuth)); }
        }


        #endregion Props

        RelayCommand savePermissions;
        public RelayCommand SavePermissions
        {
            get { return savePermissions; }
            set { savePermissions = value; RaisePropertyChanged(nameof(SavePermissions)); }
        }

        RelayCommand _SaveJournalAuth;
        public RelayCommand SaveJournalAuth
        {
            get { return _SaveJournalAuth; }
            set { _SaveJournalAuth = value; RaisePropertyChanged(nameof(SaveJournalAuth)); }
        }


    }

    #region Model

    public class BarCodeSettingsList : CRUDManagerService.PropertiesViewModelBase
    {
        private int _iserialField;

        private string _printingCodeField;

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string PrintingCode
        {
            get { return _printingCodeField; }
            set
            {
                if ((ReferenceEquals(_printingCodeField, value) != true))
                {
                    _printingCodeField = value;
                    RaisePropertyChanged("PrintingCode");
                }
            }
        }
    }

    public class TblUserViewModel : GenericViewModel
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

        private int? _regNoField;

        private int? _tblJobField;

        private string _tel1Field;

        private string _tel2Field;

        private string _userNameField;

        private string _userPasswordField;

        private string _userDomainField;

        private string _userWinLoginField;

        private int? _wfmDomainidField;

        private int? _printingCodeField;

        private TblAuthJob _tblAuthJobField;

        private BarCodeSettingsList _barcodePerRow;

        public BarCodeSettingsList BarcodePerRow
        {
            get { return _barcodePerRow; }
            set { _barcodePerRow = value; RaisePropertyChanged("BarcodePerRow"); }
        }

        public int? Active
        {
            get { return _activeField; }
            set
            {
                if ((_activeField.Equals(value) != true))
                {
                    _activeField = value;
                    RaisePropertyChanged("Active");
                }
            }
        }

        private CRUDManagerService.GenericTable _positionPerRow;

        public CRUDManagerService.GenericTable PositionPerRow
        {
            get { return _positionPerRow; }
            set { _positionPerRow = value; RaisePropertyChanged("PositionPerRow"); }
        }

        private CRUDManagerService.GenericTable _companyPerRow;

        public CRUDManagerService.GenericTable CompanyPerRow
        {
            get { return _companyPerRow; }
            set { _companyPerRow = value; RaisePropertyChanged("CompanyPerRow"); }
        }

        private CRUDManagerService.GenericTable _SecondarycompanyPerRow;

        public CRUDManagerService.GenericTable SecondaryCompanyPerRow
        {
            get { return _SecondarycompanyPerRow; }
            set { _SecondarycompanyPerRow = value; RaisePropertyChanged("SecondaryCompanyPerRow"); }
        }


        private int? _tblposition;

        public int? TblPosition
        {
            get { return _tblposition; }
            set
            {
                if ((_tblposition.Equals(value) != true))
                {
                    _tblposition = value;
                    RaisePropertyChanged("TblPosition");
                }
            }
        }

        private int? _tblCompany;

        public int? TblCompany
        {
            get { return _tblCompany; }
            set
            {
                if ((_tblCompany.Equals(value) != true))
                {
                    _tblCompany = value;
                    RaisePropertyChanged("TblCompany");
                }
            }
        }

        private int? _tblCompanySecondary;

        public int? TblCompanySecondary
        {
            get { return _tblCompanySecondary; }
            set
            {
                if ((_tblCompanySecondary.Equals(value) != true))
                {
                    _tblCompanySecondary = value;
                    RaisePropertyChanged("TblCompanySecondary");
                }
            }
        }

        public int ActiveStore
        {
            get { return _activeStoreField; }
            set
            {
                if ((Equals(_activeStoreField, value) != true))
                {
                    _activeStoreField = value;
                    RaisePropertyChanged("ActiveStore");
                }
            }
        }

        public string Address
        {
            get { return _addressField; }
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
            get { return _allowedStoresField; }
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
            get { return _axIdField; }
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
            get { return _axNameField; }
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
            get { return _commentField; }
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
            get { return _currLangField; }
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
            get { return _isEnabledField; }
            set
            {
                if ((_isEnabledField.Equals(value) != true))
                {
                    _isEnabledField = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public int? RegNo
        {
            get { return _regNoField; }
            set
            {
                if ((_regNoField.Equals(value) != true))
                {
                    _regNoField = value;
                    RaisePropertyChanged("RegNo");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJob")]
        public int? TblJob
        {
            get { return _tblJobField; }
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
            get { return _tel1Field; }
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
            get { return _tel2Field; }
            set
            {
                if ((ReferenceEquals(_tel2Field, value) != true))
                {
                    _tel2Field = value;
                    RaisePropertyChanged("Tel2");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqUserName")]
        public string UserName
        {
            get { return _userNameField; }
            set
            {
                if ((ReferenceEquals(_userNameField, value) != true))
                {
                    _userNameField = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqUserPassword")]
        public string UserPassword
        {
            get { return _userPasswordField; }
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
            get { return _userDomainField; }
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
            get { return _userWinLoginField; }
            set
            {
                if ((ReferenceEquals(_userWinLoginField, value) != true))
                {
                    _userWinLoginField = value;
                    RaisePropertyChanged("User_Win_Login");
                }
            }
        }

        public int? WFM_DOMAINID
        {
            get { return _wfmDomainidField; }
            set
            {
                if ((_wfmDomainidField.Equals(value) != true))
                {
                    _wfmDomainidField = value;
                    RaisePropertyChanged("WFM_DOMAINID");
                }
            }
        }

        public int? PrintingCode
        {
            get { return _printingCodeField; }
            set
            {
                if ((ReferenceEquals(_printingCodeField, value) != true))
                {
                    _printingCodeField = value;
                    RaisePropertyChanged("PrintingCode");
                }
            }
        }

        public TblAuthJob JobPerRow
        {
            get { return _tblAuthJobField; }
            set
            {
                if ((ReferenceEquals(_tblAuthJobField, value) != true))
                {
                    _tblAuthJobField = value;
                    RaisePropertyChanged("JobPerRow");
                }
            }
        }
    }

    public class Language : PropertiesViewModelBase
    {
        private string _language;

        private short? _currLan;

        public string Lang
        {
            get { return _language; }
            set
            {
                if ((ReferenceEquals(_language, value) != true))
                {
                    _language = value;
                    RaisePropertyChanged("Lang");
                }
            }
        }

        public short? CurrLan
        {
            get { return _currLan; }
            set
            {
                if ((ReferenceEquals(_currLan, value) != true))
                {
                    _currLan = value;
                    RaisePropertyChanged("CurrLan");
                }
            }
        }
    }

    public class AxUsers : PropertiesViewModelBase
    {
        private string _companyField;

        private string _idField;

        private string _nameField;

        private string _userDomainField;

        private string _userWinLoginField;

        public string COMPANY
        {
            get { return _companyField; }
            set
            {
                if ((ReferenceEquals(_companyField, value) != true))
                {
                    _companyField = value;
                    RaisePropertyChanged("COMPANY");
                }
            }
        }

        public string ID
        {
            get { return _idField; }
            set
            {
                if ((ReferenceEquals(_idField, value) != true))
                {
                    _idField = value;
                    RaisePropertyChanged("ID");
                }
            }
        }

        public string NAME
        {
            get { return _nameField; }
            set
            {
                if ((ReferenceEquals(_nameField, value) != true))
                {
                    _nameField = value;
                    RaisePropertyChanged("NAME");
                }
            }
        }

        public string User_Domain
        {
            get { return _userDomainField; }
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
            get { return _userWinLoginField; }
            set
            {
                if ((ReferenceEquals(_userWinLoginField, value) != true))
                {
                    _userWinLoginField = value;
                    RaisePropertyChanged("User_Win_Login");
                }
            }
        }
    }

    #endregion Model
}