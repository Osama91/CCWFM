using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.AccessoriesViewModel
{
    public class AccessoriesViewModel : ViewModelBase
    {
        public AccessoriesViewModel()
        {
            SelectedMainRow = new AccessoryAttributesViewModel();
            AccDetailsList = new ObservableCollection<AccessoryAttributesDetailsViewModel>();            
            AccessoryTypesList = new ObservableCollection<GenericViewModel>();
            AccessoryGroupList = new ObservableCollection<GenericViewModel>();
            BrandList = new ObservableCollection<_Proxy.Brand>();
            UoMList = new ObservableCollection<GenericViewModel>();
            NewDetailsCommand = new CommandsExecuter(NewDetails) { IsEnabled = true };
            SaveAccCommand = new CommandsExecuter(SaveAccessories) { IsEnabled = true };
            FillGenericCollection("tbl_lkp_AccessoryItemType", AccessoryTypesList);
            FillGenericCollection("tbl_lkp_AccessoryGroup", AccessoryGroupList);
            FillGenericCollection("tbl_lkp_UoM", UoMList);

            Client.DeleteAccDetailCompleted += (s, sv) =>
            {
                if (sv.Error != null)
                {
                    throw sv.Error;
                }

                if (sv.Result != 0)
                {
                    var oldrow = AccDetailsList.FirstOrDefault(x => x.Iserial == sv.Result);
                    if (oldrow != null) AccDetailsList.Remove(oldrow);
                }
                else
                {
                    MessageBox.Show("This Comination Has Transaction !!! It Cannot Be Deleted");
                }
            };

            Client.GetAllBrandsCompleted += (s, sv) =>
            {
                BrandList = sv.Result;
            };
            Client.GetAllBrandsAsync(0);
            SelectedMainRow.InitiatePermissionsMapper();
            Client.GetAllAccessoriesHeaderCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    if (e.Result == null) return;
                    foreach (var row in e.Result)
                    {
                        var newrow = new AccessoryAttributesViewModel
                        {
                            ObjStatus = { IsSavedDBItem = true, IsNew = false }
                        };
                        newrow.InjectFrom(row);
                        newrow.BrandProperty = BrandList.SingleOrDefault(x => x.Brand_Code == newrow.Brand);
                        newrow.AccessoryGroupProperty =
                        AccessoryGroupList.SingleOrDefault(x => x.Iserial == newrow.AccGroup);
                        newrow.AccessorySubGroupProperty = row.tbl_AccessoriesSubGroup;
                        newrow.AccSubGrouplist.Add(row.tbl_AccessoriesSubGroup);
                        newrow.UoMProperty = UoMList.SingleOrDefault(x => x.Iserial == newrow.UoMID);
                        newrow.AccessoryTypesProperty = AccessoryTypesList.SingleOrDefault(x => x.Iserial == newrow.ItemType);
                        newrow.LatestColor= e.TransactionExist.FirstOrDefault(x => x.Key == newrow.Code).Value+1;
                        MainRowList.Add(newrow);
                    }
                }
                else
                {
                    var err = new ErrorWindow(e.Error);
                    err.Show();
                }
            };

            Client.GetAllAccessoriesAttributesDetailsCompleted += (s1, e1) =>
            {
                foreach (var item in e1.Result)
                {
                    var newItem = new AccessoryAttributesDetailsViewModel
                    {
                        ObjStatus = { IsSavedDBItem = true },
                    };

                    newItem.InjectFrom(item);

                    AccDetailsList.Add(newItem);
                }
                Loading = false;
            };
        }

        private void FillGenericCollection(string tablEname, ObservableCollection<GenericViewModel> objectToFill)
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();

            client.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            client.GetGenericCompleted += (s, ev) =>
            {
                var i = 0;
                foreach (var item in ev.Result)
                {
                    objectToFill.Add(new GenericViewModel
                    {
                        Iserial = item.Iserial,
                        Code = item.Code,
                        Aname = item.Aname,
                        Ename = item.Ename
                    });
                    objectToFill[i].Status.IsChanged = false;
                    objectToFill[i].Status.IsNew = false;
                    objectToFill[i].Status.IsSavedDBItem = true;
                    i++;
                }
            };
        }

        public void LoadAccessoryDetail()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Client.GetAllAccessoriesAttributesDetailsAsync(AccDetailsList.Count, PageSize, SelectedMainRow.FullCode, DetailSortBy, DetailFilter, DetailValuesObjects);
            SubmitSearchAction.Invoke(this, null);
        }

        private void NewDetails()
        {

            try
            {
                SelectedMainRow.LatestColor = SelectedMainRow.LatestColor + 1;
                //SelectedMainRow.LatestColor ;
                var temp = new AccessoryAttributesDetailsViewModel
                {
                    Code = SelectedMainRow.FullCode,
                    Descreption = SelectedMainRow.Descreption,
                    Configuration = SelectedMainRow.LatestColor.ToString(),
                    Size = SelectedMainRow.AccSizeProperty != null ? SelectedMainRow.AccSizeProperty.TblAccSize1.SizeCode : null,
                    ObjStatus = { IsNew = true }
                };
                AccDetailsList.Add(temp);
                
                SelectedDetail = temp;
            }
            catch
            {
                var err = new ErrorWindow("Error Adding Details"
                    , "Please make sure that there is enough data in the header section!");
                err.Show();
            }
        }

        private void SaveAccessories()
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            var headerRow = new _Proxy.tbl_AccessoryAttributesHeader();
            
            headerRow.InjectFrom(SelectedMainRow);

            var detailsList = new ObservableCollection<_Proxy.tbl_AccessoryAttributesDetails>();
            
            GenericMapper.InjectFromObCollection(detailsList, AccDetailsList);


            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(SelectedMainRow,
                new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                MessageBox.Show("Data IS not Valid");
                return;
            }


            if (SelectedMainRow.ObjStatus.IsNew)
            {
                client.AddAllNewAccessoriesAttributesCompleted += (s, sv)
                    =>
                {
                    MessageBox.Show(sv.Error == null ? strings.SavedMessage : strings.FailSavingMessage);
                    SelectedMainRow.InjectFrom(sv.Result);
                    SelectedMainRow.ObjStatus.IsNew = false;
                    SelectedMainRow.ObjStatus.IsSavedDBItem = true;

                    AccDetailsList = new SortableCollectionView<AccessoryAttributesDetailsViewModel>();
                   

                    foreach (var row in sv.Result.tbl_AccessoryAttributesDetails)
                    {
                        var newrow = new AccessoryAttributesDetailsViewModel
                        {
                            ObjStatus = { IsNew = false, IsSavedDBItem = true }
                        };
                        newrow.InjectFrom(row);
                        AccDetailsList.Add(newrow);
                    }
                };
                //  if (AllowAdd)
                //        {
                client
                    .AddAllNewAccessoriesAttributesAsync
                    (headerRow
                    , detailsList, SelectedMainRow.IsSizeIncludedInHeader, LoggedUserInfo.Iserial);
                //       }
                //      else
                //   {
                //         MessageBox.Show("You do not have the permission to add a new item!");
                //    }
            }
            else if (SelectedMainRow.ObjStatus.IsSavedDBItem)
            {
                client.UpdateAccessoriesAttributesCompleted += (s, e) =>
                {
                    MessageBox.Show(e.Error == null ? strings.SavedMessage : strings.FailSavingMessage);
                    SelectedMainRow.InjectFrom(e.Result);
                    SelectedMainRow.ObjStatus.IsNew = false;
                    SelectedMainRow.ObjStatus.IsSavedDBItem = true;

                    AccDetailsList = new SortableCollectionView<AccessoryAttributesDetailsViewModel>();
                    
                    foreach (var row in e.Result.tbl_AccessoryAttributesDetails)
                    {
                        var newrow = new AccessoryAttributesDetailsViewModel
                        {
                            ObjStatus = { IsNew = false, IsSavedDBItem = true }
                        };
                        newrow.InjectFrom(row);
                        AccDetailsList.Add(newrow);
                    }
                }
                ;
                //     if (AllowUpdate)
                //     {
                var detailsNewList = new ObservableCollection<_Proxy.tbl_AccessoryAttributesDetails>();
                GenericMapper.InjectFromObCollection(detailsNewList, AccDetailsList.Where(x => x.ObjStatus.IsNew));

                var detailsUpdatedList = new ObservableCollection<_Proxy.tbl_AccessoryAttributesDetails>();
                GenericMapper.InjectFromObCollection(detailsUpdatedList, AccDetailsList.Where(x => x.ObjStatus.IsSavedDBItem));

                client
                    .UpdateAccessoriesAttributesAsync
                    (headerRow,
                        detailsNewList,
                        detailsUpdatedList, LoggedUserInfo.Iserial);
                //   }
                //    else
                //    {
                //        MessageBox.Show("You do not have the update permission");
                //   }
            }
        }

    
        internal void SearchHeader()
        {
            MainRowList.Clear();
            GetAllAcc();
            var childWindowSeach = new AccessorySearcResultChildWindow(this);
            childWindowSeach.Show();
        }

        public void GetAllAcc()
        {
            if (SortBy == null)
                SortBy = "it.Code";

            Client.GetAllAccessoriesHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Props

        public event EventHandler SubmitSearchAction;

        private ObservableCollection<AccessoryAttributesDetailsViewModel> _accDetailsList;
        private ObservableCollection<GenericViewModel> _accessoryGroupList;
        private ObservableCollection<GenericViewModel> _accessoryTypesList;

        private ObservableCollection<_Proxy.Brand> _brandList;

        private CommandsExecuter _checkCodeCommand;
        private CommandsExecuter _deleteDetailCommand;
        private CommandsExecuter _newDetailsCommand;
        private CommandsExecuter _saveAccCommand;
        private AccessoryAttributesDetailsViewModel _selectedDetail;
        private ObservableCollection<GenericViewModel> _uoMList;
        private SortableCollectionView<AccessoryAttributesViewModel> _mainRowList;

        public SortableCollectionView<AccessoryAttributesViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<AccessoryAttributesViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private AccessoryAttributesViewModel _selectedMainRow;

        public AccessoryAttributesViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private bool _isEnabled;

        public bool Enabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; RaisePropertyChanged("Enabled"); }
        }

        public ObservableCollection<AccessoryAttributesDetailsViewModel> AccDetailsList
        {
            get { return _accDetailsList; }
            set { _accDetailsList = value; RaisePropertyChanged("AccDetailsList"); }
        }

        public ObservableCollection<GenericViewModel> AccessoryGroupList
        {
            get { return _accessoryGroupList; }
            set { _accessoryGroupList = value; RaisePropertyChanged("AccessoryGroupList"); }
        }

        public ObservableCollection<GenericViewModel> AccessoryTypesList
        {
            get { return _accessoryTypesList; }
            set { _accessoryTypesList = value; RaisePropertyChanged("AccessoryTypesList"); }
        }

        public ObservableCollection<_Proxy.Brand> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        public AccessoryAttributesDetailsViewModel SelectedDetail
        {
            get { return _selectedDetail; }
            set { _selectedDetail = value; RaisePropertyChanged("SelectedDetail"); }
        }

        public ObservableCollection<GenericViewModel> UoMList
        {
            get { return _uoMList; }
            set { _uoMList = value; RaisePropertyChanged("UoMList"); }
        }

        private CommandsExecuter _loadCodeCommand;

        public CommandsExecuter CheckCodeCommand
        {
            get { return _checkCodeCommand; }
            set { _checkCodeCommand = value; RaisePropertyChanged("CheckCodeCommand"); }
        }

        public CommandsExecuter DeleteDetailCommand
        {
            get { return _deleteDetailCommand; }
            set { _deleteDetailCommand = value; RaisePropertyChanged("DeleteDetailCommand"); }
        }

        public CommandsExecuter LoadCodeCommand
        {
            get { return _loadCodeCommand; }
            set { _loadCodeCommand = value; RaisePropertyChanged("LoadCodeCommand"); }
        }

        public CommandsExecuter NewDetailsCommand
        {
            get { return _newDetailsCommand; }
            set { _newDetailsCommand = value; RaisePropertyChanged("NewDetailsCommand"); }
        }

        public CommandsExecuter SaveAccCommand
        {
            get { return _saveAccCommand; }
            set { _saveAccCommand = value; RaisePropertyChanged("SaveAccCommand"); }
        }

        #endregion Props

        public void DeletedAccDetail()
        {
            if (SelectedDetail.Iserial != 0)
            {
                Client.DeleteAccDetailAsync((_Proxy.tbl_AccessoryAttributesDetails)new _Proxy.tbl_AccessoryAttributesDetails().InjectFrom(SelectedDetail), LoggedUserInfo.Iserial);
            }
            else
            {
                AccDetailsList.Remove(SelectedDetail);
            }
        }
    }

    public class AccessoryAttributesDetailsViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        #region [ Private Fields ]

        private CommandsExecuter _browseImageCommand;
        private CommandsExecuter _deleteSelfCommand;
        private ObjectStatus _objstatus;
        private byte[] _accImageField;

        private string _codeField;

        private string _configurationField;

        private string _descreptionField;

        private string _notesField;

        private string _sizeField;
        #endregion [ Private Fields ]

        #region [ Public Properties ]

        [Display(ResourceType = typeof(strings), Name = "FabImg")]
        public byte[] AccImage
        {
            get
            {
                return _accImageField;
            }
            set
            {
                if ((ReferenceEquals(_accImageField, value) != true))
                {
                    _accImageField = value;
                    RaisePropertyChanged("AccImage");
                }
            }
        }

        public CommandsExecuter BrowseImageCommand
        {
            get { return _browseImageCommand; }
            set { _browseImageCommand = value; RaisePropertyChanged("BrowseImageCommand"); }
        }

        [Display(ResourceType = typeof(strings), Name = "Code")]
        public string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        [Required]
        [Display(ResourceType = typeof(strings), Name = "AccsConfig")]
        public string Configuration
        {
            get
            {
                return _configurationField;
            }
            set
            {
                if ((ReferenceEquals(_configurationField, value) != true))
                {
                    _configurationField = value.ToUpper(CultureInfo.InvariantCulture);
                    RaisePropertyChanged("Configuration");
                }
            }
        }

        public CommandsExecuter DeleteSelfCommand
        {
            get { return _deleteSelfCommand; }
            set { _deleteSelfCommand = value; RaisePropertyChanged("DeleteSelfCommand"); }
        }

        [Display(ResourceType = typeof(strings), Name = "Description")]
        public string Descreption
        {
            get
            {
                return _descreptionField;
            }
            set
            {
                if ((ReferenceEquals(_descreptionField, value) != true))
                {
                    _descreptionField = value;
                    RaisePropertyChanged("Descreption");
                }
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Notes")]
        public string Notes
        {
            get
            {
                return _notesField;
            }
            set
            {
                if ((ReferenceEquals(_notesField, value) != true))
                {
                    _notesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objstatus; }
            set { _objstatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        [Required]
        [Display(ResourceType = typeof(strings), Name = "AccsSize")]
        public string Size
        {
            get { return _sizeField; }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true))
                {
                    if (value != null)
                    {
                        _sizeField = value.Trim().ToUpperInvariant();
                    }

                    RaisePropertyChanged("Size");
                }
            }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public AccessoryAttributesDetailsViewModel()
        {
            InitializeCommands();
            ObjStatus = new ObjectStatus();
        }

        #endregion [ Constructor(s) ]

        #region [ Private Methods ]

        private void BrowseImage()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                if (dlg.Files.Any(x => x.Length > 1048576))
                {
                    new ErrorWindow("Error Uploading Image"
                        , "I have detected that the uploaded Image exceeds the allowed size which is 1 Megabyte...\nPlease note that any image larger than 1 MB will not be added to the galary!");
                }
                if (dlg.Files.ToList().Count > 0)
                {
                    if (dlg.File.Length <= 1048576)
                    {
                        var reader = dlg.File.OpenRead();
                        var byteArray = new byte[reader.Length];
                        reader.Read(byteArray, 0, Convert.ToInt32(reader.Length));
                        AccImage = byteArray;
                    }
                }
            }
        }

        private void InitializeCommands()
        {
            BrowseImageCommand = new CommandsExecuter(BrowseImage) { IsEnabled = true };
        }

        #endregion [ Private Methods ]
    }

    public class AccessoryAttributesViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private ObservableCollection<_Proxy.tbl_AccessoriesSubGroup> _accSubGrouplist;

        private string _codePrefix;
        private GenericViewModel _accessoryGroupProperty;
        private _Proxy.tbl_AccessoriesSubGroup _accessorySubGroupProperty;
        private GenericViewModel _accessoryTypesProperty;
        private _Proxy.Brand _brandProperty;
        private string _fullCode;
        private bool _isCodeExists;
        private GenericViewModel _uoMField;
        private ObjectStatus _objstatus;
        private string _accessoryBoxNumberField;
        private int? _accGroup1Field;
        private int? _accGroup2Field;
        private int? _accGroup3Field;
        private int? _accGroup4Field;
        private int _accGroupField;
        private int? _accSubGroupField;
        private string _brandField;
        private string _codeField;
        private string _descreptionField;
        private int? _itemTypeField;
        private DataTemplate _listDataTemplate;
        private int? _uoMidField;
        #endregion [ Private Fields ]

        #region [ public Properties ]

        public ObservableCollection<_Proxy.tbl_AccessoriesSubGroup> AccSubGrouplist
        {
            get { return _accSubGrouplist; }
            set { _accSubGrouplist = value; RaisePropertyChanged("AccSubGrouplist"); }
        }

        private PermissionItemName _formName;

        public PermissionItemName FormName
        {
            get { return _formName; }
            set { _formName = value; RaisePropertyChanged("FormName"); }
        }

        public bool IsSizeIncludedInHeader
        {
            get { return _isSizeIncludedInHeader; }
            set { _isSizeIncludedInHeader = value; RaisePropertyChanged("IsSizeIncludedInHeader"); }
        }

        public string AccessoryBoxNumber
        {
            get
            {
                return _accessoryBoxNumberField;
            }
            set
            {
                if ((ReferenceEquals(_accessoryBoxNumberField, value) != true))
                {
                    _accessoryBoxNumberField = value;
                    RaisePropertyChanged("AccessoryBoxNumber");
                }
            }
        }

        public ObservableCollection<_Proxy.TblAccSubGroupSizeLink> AccSizesList
        {
            get { return _accSizesList; }
            set { _accSizesList = value; RaisePropertyChanged("AccSizesList"); }
        }

        public _Proxy.TblAccSubGroupSizeLink AccSizeProperty
        {
            get { return _accSizeProperty; }
            set
            {
                _accSizeProperty = value;
                RaisePropertyChanged("AccSizeProperty");
                TblSize = AccSizeProperty.TblAccSize;
                UpdateCodePrefix();
            }
        }

        public GenericViewModel AccessoryGroupProperty
        {
            get { return _accessoryGroupProperty; }
            set
            {
                _accessoryGroupProperty = value;
                RaisePropertyChanged("AccessoryGroupProperty");
                if (!ObjStatus.IsSavedDBItem)
                {
                    FillSubGroupCollection();

                    UpdateCodePrefix();
                }
                AccGroup = value.Iserial;
            }
        }

        public _Proxy.tbl_AccessoriesSubGroup AccessorySubGroupProperty
        {
            get { return _accessorySubGroupProperty; }
            set
            {
                _accessorySubGroupProperty = value;
                RaisePropertyChanged("AccessorySubGroupProperty");
                if (value != null)
                {
                    AccSubGroup = value.Iserial;
                    if (!ObjStatus.IsSavedDBItem)
                    {
                        UpdateCodePrefix();
                        FillSubGroupSizes();
                    }
                }
            }
        }

        public GenericViewModel AccessoryTypesProperty
        {
            get { return _accessoryTypesProperty; }
            set
            {
                _accessoryTypesProperty = value;
                RaisePropertyChanged("AccessoryTypesProperty");
                //this.ItemType = value.Iserial;
            }
        }

        public int AccGroup
        {
            get
            {
                return _accGroupField;
            }
            set
            {
                if ((_accGroupField.Equals(value) != true))
                {
                    _accGroupField = value;
                    RaisePropertyChanged("AccGroup");
                }
            }
        }

        public int? AccGroup1
        {
            get
            {
                return _accGroup1Field;
            }
            set
            {
                if ((_accGroup1Field.Equals(value) != true))
                {
                    _accGroup1Field = value;
                    RaisePropertyChanged("AccGroup1");
                }
            }
        }

        public int? AccGroup2
        {
            get
            {
                return _accGroup2Field;
            }
            set
            {
                if ((_accGroup2Field.Equals(value) != true))
                {
                    _accGroup2Field = value;
                    RaisePropertyChanged("AccGroup2");
                }
            }
        }

        public int? AccGroup3
        {
            get
            {
                return _accGroup3Field;
            }
            set
            {
                if ((_accGroup3Field.Equals(value) != true))
                {
                    _accGroup3Field = value;
                    RaisePropertyChanged("AccGroup3");
                }
            }
        }

        public int? AccGroup4
        {
            get
            {
                return _accGroup4Field;
            }
            set
            {
                if ((_accGroup4Field.Equals(value) != true))
                {
                    _accGroup4Field = value;
                    RaisePropertyChanged("AccGroup4");
                }
            }
        }

        public int? AccSubGroup
        {
            get
            {
                return _accSubGroupField;
            }
            set
            {
                if ((_accSubGroupField.Equals(value) != true))
                {
                    _accSubGroupField = value;
                    RaisePropertyChanged("AccSubGroup");
                    UpdateCodePrefix();
                }
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public string Brand
        {
            get
            {
                return _brandField;
            }
            set
            {
                if ((ReferenceEquals(_brandField, value) != true))
                {
                    _brandField = value;
                    RaisePropertyChanged("Brand");
                }
            }
        }

        public _Proxy.Brand BrandProperty
        {
            get { return _brandProperty; }
            set
            {
                _brandProperty = value;
                RaisePropertyChanged("BrandProperty");
                if (value != null)
                {
                    UpdateCodePrefix();
                    Brand = value.Brand_Code;
                }
            }
        }

        public new string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                    FullCode = CodePrefix + value;
                }
            }
        }

        public string CodePrefix
        {
            get { return _codePrefix; }
            set
            {
                if (_codePrefix != value)
                {
                    _codePrefix = value;
                    RaisePropertyChanged("CodePrefix");
                    FullCode = value + Code;
                }
            }
        }

        public string Descreption
        {
            get
            {
                return _descreptionField;
            }
            set
            {
                if ((ReferenceEquals(_descreptionField, value) != true))
                {
                    _descreptionField = value;
                    RaisePropertyChanged("Descreption");
                }
            }
        }

        public string FullCode
        {
            get { return _fullCode; }
            set
            {
                if (ReferenceEquals(_fullCode, value) == false)
                {
                    _fullCode = value;
                    RaisePropertyChanged("FullCode");
                }
            }
        }

        public bool IsCodeExists
        {
            get { return _isCodeExists; }
            set { _isCodeExists = value; RaisePropertyChanged("IsCodeExists"); }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqItemType")]
        public int? ItemType
        {
            get
            {
                return _itemTypeField;
            }
            set
            {
                if ((_itemTypeField.Equals(value) != true))
                {
                    _itemTypeField = value;
                    RaisePropertyChanged("ItemType");
                }
            }
        }

        public DataTemplate ListDataTemplate
        {
            get { return _listDataTemplate; }
            set { _listDataTemplate = value; RaisePropertyChanged("ListDataTemplate"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objstatus ?? (_objstatus = new ObjectStatus()); }
            set
            {
                _objstatus = value; RaisePropertyChanged("ObjStatus");
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqUnit")]
        public int? UoMID
        {
            get
            {
                return _uoMidField;
            }
            set
            {
                if ((_uoMidField.Equals(value) != true))
                {
                    _uoMidField = value;
                    RaisePropertyChanged("UoMID");
                }
            }
        }

        private int? _tblSize;

        public int? TblSize
        {
            get { return _tblSize; }
            set { _tblSize = value; RaisePropertyChanged("TblSize"); }
        }

        public GenericViewModel UoMProperty
        {
            get { return _uoMField; }
            set
            {
                _uoMField = value;
                RaisePropertyChanged("UoMProperty");
                if (_uoMField != null)
                {
                    UoMID = value.Iserial;
                }
            }
        }

        public int LatestColor { get; set; }

        #endregion [ public Properties ]

        private ObservableCollection<_Proxy.TblAccSubGroupSizeLink> _accSizesList;

        private _Proxy.TblAccSubGroupSizeLink _accSizeProperty;
        private bool _isSizeIncludedInHeader;

        public AccessoryAttributesViewModel()
        {
            InitializeObject();
            ObjStatus.IsNew = true;
            ObjStatus.IsSavedDBItem = false;
        }

        private void InitializeCollections()
        {
            AccSizesList = new ObservableCollection<_Proxy.TblAccSubGroupSizeLink>();
        }

        private void InitializeObject()
        {
            AccSubGrouplist = new ObservableCollection<_Proxy.tbl_AccessoriesSubGroup>();
            FormName = PermissionItemName.AccessoriesCodingForm;

            InitializeCollections();
        }

        private void UpdateCodePrefix()
        {
            if (!ObjStatus.IsSavedDBItem)
            {
                var tempSubGroup = "";
                if (IsSizeIncludedInHeader)
                {
                    if (AccessorySubGroupProperty != null)
                    {
                        tempSubGroup = AccessorySubGroupProperty.SubCode;
                    }
                    Code =
                        (tempSubGroup
                         + (BrandProperty != null
                             ? (BrandProperty.Brand_Code.ToUpper() != "FR"
                                 ? BrandProperty.Brand_Code
                                 : "")
                             : "")
                         + (AccSizeProperty != null ? (AccSizeProperty.TblAccSize1.SizeCode) : "")).ToUpper();
                }
                else
                {
                    if (AccessorySubGroupProperty != null)
                    {
                        tempSubGroup = AccessorySubGroupProperty.SubCode;
                    }
                    Code =
                        (tempSubGroup
                             + (BrandProperty != null
                             ? (BrandProperty.Brand_Code.ToUpper() != "FR"
                                 ? BrandProperty.Brand_Code
                                 : "")
                             : "")).ToUpper();
                }
            }
        }

        private void FillSubGroupSizes()
        {
            try
            {
                var client = new _Proxy.CRUD_ManagerServiceClient();
                client.GetSuptGroupSizesCompleted += (s, e) =>
                {
                    if (e.Result != null)
                    {
                        AccSizesList.Clear();
                        foreach (var item in e.Result)
                        {
                            AccSizesList.Add(item);
                            RaisePropertyChanged("AccSizesList");
                        }
                        IsSizeIncludedInHeader = true;
                    }
                    else
                    {
                        IsSizeIncludedInHeader = false;
                    }
                };
                if (AccSubGroup != null) client.GetSuptGroupSizesAsync((int)AccSubGroup);
            }
            catch (Exception ex)
            {
                var err = new ErrorWindow(ex);
                err.Show();
            }
        }

        private void FillSubGroupCollection()
        {
            try
            {
                AccSubGrouplist.Clear();
                var client = new _Proxy.CRUD_ManagerServiceClient();

                client.GetAccSubGroupByGroupCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        AccSubGrouplist.Add(item);
                        RaisePropertyChanged("AccSubGrouplist");
                    }
                };
                client.GetAccSubGroupByGroupAsync(AccessoryGroupProperty.Iserial);
            }
            catch (Exception ex)
            {
                var err = new ErrorWindow(ex);
                err.Show();
            }
        }

        #region [ Permissions Handlers ]

        private void ManagePermissions()
        {
            GetItemPermissions(FormName.ToString());
            ManageCustomePermissions();
        }

        private void ManageCustomePermissions()
        {
            GetCustomePermissions(FormName.ToString());
        }

        public override void InitiatePermissionsMapper()
        {
            base.InitiatePermissionsMapper();
            PermissionsMapper.Add(new CustomePermissionsMapper
            {
                PermissionKey = "FAutoPostToAX",
                PermissionValue = false
            });
            ManagePermissions();
        }

        #endregion [ Permissions Handlers ]

        //#region [ Command(S) bound methods ]

        ////private void CheckCode()
        ////{
        ////    Client.CheckIfAccessoryExistsCompleted += (s, e) =>
        ////    {
        ////        if (e.Error == null)
        ////        {
        ////            IsCodeExists = e.Result;
        ////        }
        ////    };
        ////    Client.CheckIfAccessoryExistsAsync(FullCode);
        ////}

        //#endregion [ Command(S) bound methods ]
    }
}