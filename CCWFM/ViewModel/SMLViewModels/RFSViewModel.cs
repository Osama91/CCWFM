using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Defaults;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public sealed class RFSViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private string _brandCode;
        private Brand _brandProp;
        private ObservableCollection<Brand> _brands;

        //private _proxy.CRUD_ManagerServiceClient _client = new _proxy.CRUD_ManagerServiceClient();
        private string _docNum;

        private ObjectMode _formMode;
        private PermissionItemName _formName;
        private bool _isCodeExists;
        private bool _isLoading;
        private bool _isSearchingMode;
        private ObjectStatus _objstatus;
        private ObservableCollection<RFSSubHeader> _rfqHeaderList;
        private ObservableCollection<GenericViewModel> _rfqItems;
        private RFSSubHeader _rfqSelectedHeader;
        private ObservableCollection<GenericViewModel> _rfqServices;
        private string _seasonCode;
        private TblLkpSeason _seasonProp;
        private ObservableCollection<TblLkpSeason> _seasons;
        private string _supplierId;
        private Vendor _supplierProp;
        private int? _transId;
        private ObservableCollection<Vendor> _vendors;
        #endregion [ Private Fields ]

        #region [ Public Properties ]

        private bool _allfollowupApproved;

        private ObservableCollection<PurchaseOrderHeaderViewModel> _deletedPurchHeaders;

        private ObservableCollection<RFSSubHeader> _rfqHeaderDeletedList;

        public bool AllFollowupApproved
        {
            get { return _allfollowupApproved; }
            set
            {
                _allfollowupApproved = value;
                RaisePropertyChanged("AllFollowupApproved");
            }
        }

        [Required]
        public string BrandCode
        {
            get { return _brandCode; }
            set { _brandCode = value; RaisePropertyChanged("BrandCode"); }
        }

        [Required]
        public Brand BrandProp
        {
            get { return _brandProp; }
            set
            {
                if (!ReferenceEquals(_brandProp, value))
                {
                    _brandProp = value;
                    FillColors();
                }
                RaisePropertyChanged("BrandProp");
            }
        }

        public ObservableCollection<Brand> Brands
        {
            get { return _brands; }
            set { _brands = value; RaisePropertyChanged("Brands"); }
        }

        public ObservableCollection<PurchaseOrderHeaderViewModel> DeletedPurchHeaders
        {
            get { return _deletedPurchHeaders ?? (_deletedPurchHeaders = new ObservableCollection<PurchaseOrderHeaderViewModel>()); }
            set { _deletedPurchHeaders = value; RaisePropertyChanged("DeletedPurchHeaders"); }
        }

        public string DocNum
        {
            get { return _docNum; }
            set { _docNum = value; RaisePropertyChanged("DocNum"); }
        }

        public ObjectMode FormMode
        {
            get { return _formMode; }
            set
            {
                _formMode = value;
                RaisePropertyChanged("");
            }
        }

        public PermissionItemName FormName
        {
            get { return _formName; }
            set { _formName = value; RaisePropertyChanged("FormName"); }
        }

        public bool IsCodeExists
        {
            get { return _isCodeExists; }
            set { _isCodeExists = value; RaisePropertyChanged("IsCodeExists"); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; RaisePropertyChanged("IsLoaded"); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChanged("IsLoading"); }
        }

        public bool IsSearchingMode
        {
            get { return _isSearchingMode; }
            set { _isSearchingMode = value; RaisePropertyChanged("IsSearchingMode"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objstatus; }
            set { _objstatus = value; RaisePropertyChanged("Status"); }
        }

        public ObservableCollection<RFSSubHeader> RFQHeaderDeletedList
        {
            get { return _rfqHeaderDeletedList ?? (_rfqHeaderDeletedList = new ObservableCollection<RFSSubHeader>()); }
            set { _rfqHeaderDeletedList = value; RaisePropertyChanged("RFQHeaderDeletedList"); }
        }

        public ObservableCollection<RFSSubHeader> RFQHeaderList
        {
            get { return _rfqHeaderList; }
            set { _rfqHeaderList = value; RaisePropertyChanged("RFQHeaderList"); }
        }

        public ObservableCollection<GenericViewModel> RFQItems
        {
            get { return _rfqItems; }
            set { _rfqItems = value; RaisePropertyChanged("RFQItems"); }
        }

        public RFSSubHeader RFQSelectedHeader
        {
            get { return _rfqSelectedHeader; }
            set { _rfqSelectedHeader = value; RaisePropertyChanged("RFQSelectedHeader"); }
        }

        public ObservableCollection<GenericViewModel> RFQServices
        {
            get { return _rfqServices; }
            set { _rfqServices = value; RaisePropertyChanged("RFQServices"); }
        }

        [Required]
        public string SeasonCode
        {
            get { return _seasonCode; }
            set { _seasonCode = value; RaisePropertyChanged("SeasonCode"); }
        }

        [Required]
        public TblLkpSeason SeasonProp
        {
            get { return _seasonProp; }
            set
            {
                if (!ReferenceEquals(_seasonProp, value))
                {
                    _seasonProp = value;
                    FillColors();
                }
                RaisePropertyChanged("SeasonProp");
            }
        }

        public ObservableCollection<TblLkpSeason> Seasons
        {
            get { return _seasons; }
            set { _seasons = value; RaisePropertyChanged("Seasons"); }
        }

        public ObservableCollection<SizesWithGroups> SizesCollection
        {
            get { return _sizesCollection; }
            set { _sizesCollection = value; RaisePropertyChanged("SizesCollection"); }
        }

        [Required]
        public string SupplierID
        {
            get { return _supplierId; }
            set { _supplierId = value; RaisePropertyChanged("SupplierID"); }
        }

        [Required]
        public Vendor SupplierProp
        {
            get { return _supplierProp; }
            set { _supplierProp = value; RaisePropertyChanged("SupplierProp"); }
        }

        public int? TransID
        {
            get { return _transId; }
            set { _transId = value; RaisePropertyChanged("TransID"); }
        }

        public ObservableCollection<Vendor> Vendors
        {
            get { return _vendors; }
            set { _vendors = value; RaisePropertyChanged("Vendors"); }
        }

        #endregion [ Public Properties ]

        #region [ Commands ]

        private CommandsExecuter _addNewPurchCommand;
        private CommandsExecuter _addNewsubHeader;
        private CommandsExecuter _cancelCommand;
        private CommandsExecuter _changeobjStatusCommand;
        private CommandsExecuter _checkCodeCommand;
        private CommandsExecuter _deleteCostFollowUpCommand;
        private CommandsExecuter _deleteFollowUpCommand;
        private CommandsExecuter _deleteItemsCommand;
        private CommandsExecuter _deletePurchaseOrderCommand;
        private CommandsExecuter _deleteRfq;
        private CommandsExecuter _deleteServiceCommand;
        private CommandsExecuter _deleteSubHeaderCommand;
        private bool _isLoaded;
        private CommandsExecuter _loadCodeCommand;
        private CommandsExecuter _newRfqCommand;
        private CommandsExecuter _savePurchaseOrderCommand;
        private CommandsExecuter _saveRfqCommand;
        private CommandsExecuter _searchRfqCommand;
        private ObservableCollection<SizesWithGroups> _sizesCollection;
        private CommandsExecuter _toggleSearchCommand;
        private CommandsExecuter _viewReportCommand;

        public CommandsExecuter AddNewPurchCommand
        {
            get { return _addNewPurchCommand; }
            set
            {
                _addNewPurchCommand = value;
                RaisePropertyChanged("AddNewPurchCommand");
            }
        }

        public CommandsExecuter AddNewSubHeaderCommand
        {
            get { return _addNewsubHeader; }
            set { _addNewsubHeader = value; RaisePropertyChanged("AddNewSubHeaderCommand"); }
        }

        public CommandsExecuter CancelCommand
        {
            get { return _cancelCommand; }
            set { _cancelCommand = value; RaisePropertyChanged("CancelCommand"); }
        }

        public CommandsExecuter ChangeObjStatusCommand
        {
            get { return _changeobjStatusCommand; }
            set { _changeobjStatusCommand = value; RaisePropertyChanged("ChangeObjStatusCommand"); }
        }

        public CommandsExecuter CheckCodeCommand
        {
            get { return _checkCodeCommand; }
            set { _checkCodeCommand = value; RaisePropertyChanged("CheckCodeCommand"); }
        }

        public CommandsExecuter DeleteCostFollowUpCommand
        {
            get { return _deleteCostFollowUpCommand; }
            set { _deleteCostFollowUpCommand = value; RaisePropertyChanged("DeleteCostFollowUpCommand"); }
        }

        public CommandsExecuter DeleteFollowUpCommand
        {
            get { return _deleteFollowUpCommand; }
            set { _deleteFollowUpCommand = value; RaisePropertyChanged("DeleteFollowUpCommand"); }
        }

        public CommandsExecuter DeleteItemsCommand
        {
            get { return _deleteItemsCommand; }
            set { _deleteItemsCommand = value; RaisePropertyChanged("DeleteItemsCommand"); }
        }

        public CommandsExecuter DeletePurchaseOrderCommand
        {
            get { return _deletePurchaseOrderCommand; }
            set
            {
                _deletePurchaseOrderCommand = value;
                RaisePropertyChanged("DeletePurchaseOrderCommand");
            }
        }

        public CommandsExecuter DeleteRFQCommand
        {
            get { return _deleteRfq; }
            set { _deleteRfq = value; RaisePropertyChanged("DeleteRFQ"); }
        }

        public CommandsExecuter DeleteServiceCommand
        {
            get { return _deleteServiceCommand; }
            set { _deleteServiceCommand = value; RaisePropertyChanged("DeleteServiceCommand"); }
        }

        public CommandsExecuter DeleteSubHeaderCommand
        {
            get { return _deleteSubHeaderCommand; }
            set { _deleteSubHeaderCommand = value; RaisePropertyChanged("DeleteSubHeaderCommand"); }
        }

        public CommandsExecuter LoadCodeCommand
        {
            get { return _loadCodeCommand; }
            set { _loadCodeCommand = value; RaisePropertyChanged("LoadCodeCommand"); }
        }

        public CommandsExecuter NewRFQCommand
        {
            get { return _newRfqCommand; }
            set { _newRfqCommand = value; RaisePropertyChanged("NewRFQCommand"); }
        }

        public CommandsExecuter SavePurchaseOrderCommand
        {
            get { return _savePurchaseOrderCommand; }
            set
            {
                _savePurchaseOrderCommand = value;
                RaisePropertyChanged("SavePurchaseOrderCommand");
            }
        }

        public CommandsExecuter SaveRFQCommand
        {
            get { return _saveRfqCommand; }
            set { _saveRfqCommand = value; RaisePropertyChanged("SaveRFQCommand"); }
        }

        public CommandsExecuter SearchRFQCommand
        {
            get { return _searchRfqCommand; }
            set { _searchRfqCommand = value; RaisePropertyChanged("SearchRFQCommand"); }
        }

        public CommandsExecuter ToggleSearchCommand
        {
            get { return _toggleSearchCommand; }
            set { _toggleSearchCommand = value; RaisePropertyChanged("ToggleSearchCommand"); }
        }

        public CommandsExecuter ViewReportCommand
        {
            get
            {
                return _viewReportCommand
                       ??
                       (_viewReportCommand = new CommandsExecuter(ViewReport)
                       {
                           IsEnabled = true
                       });
            }
        }

        #endregion [ Commands ]

        #region [ Constructor(s) ]

        public RFSViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();
                InitiateCommands();
                // FormName = PermissionItemName.RFQForm;
                InitiatePermissionsMapper();
                ManagePermissions();
                ManageCustomePermissions();
                FormMode = new ObjectMode();
                FormMode = ObjectMode.StandBy;
                InitializeObjStatus();
                InitiateCollections();
                InitializeServiceEvents();
                FillBasicCollections();
            }
        }

        #endregion [ Constructor(s) ]

        #region [ Initiators ]

        private void InitializeObjStatus()
        {
            ObjStatus = new ObjectStatus { IsChanged = false, IsMarkedForDeletion = false };
        }

        private void InitializeServiceEvents()
        {
            Client.GetRFSCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error != null) return;
                    RFQGlobalLkps.ColorsList = new ObservableCollection<TblColor>(e.colorsList);
                    //RFQGlobalLkps.StatisSizesCollection = new ObservableCollection<_proxy.SizesWithGroups>();
                    if (e.Result != null)
                    {
                        RFSMVM_Mapper.MapToViewModel(e.Result, this);
                        IsCodeExists = false;
                        IsSearchingMode = false;
                        foreach (var item in RFQHeaderList)
                        {
                            item.ObjStatus.IsSavedDBItem = true;
                            item.ObjStatus.IsNew = false;
                        }
                        IsLoaded = true;
                    }
                    else if (e.Result == null)
                    {
                        MessageBox.Show("Document number does not exist");
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            };

            Client.GetTblColorLinkCompleted += (s, e) =>
            {
                if (e.Result == null || e.Error != null) return;
                RFQGlobalLkps.ColorsList = new ObservableCollection<TblColor>();
                RFQGlobalLkps.ColorsList.Clear();
                foreach (var item in e.Result)
                {
                    RFQGlobalLkps.ColorsList.Add(item.TblColor1);
                }
            };

            Client.GetStyleSizesFromRetailCompleted += (s, e) =>
            {
                if (e.Error != null && e.Result == null) return;
                //var res = e.Result;

                RFQGlobalLkps.StatisSizesCollection.Clear();
                foreach (var siz in e.Result)
                {
                    RFQGlobalLkps.StatisSizesCollection.Add(siz);
                }
            };

            Client.UpdateOrInsertRFSDetailServicesCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        //MessageBox.Show("Document Number is: [ " + e.Result + " ]", strings.Save, MessageBoxButton.OK);
                        // DocNum = e.Result;
                        ResetViewModel(ObjectMode.StandBy);
                    }
                    else
                    {
                        //MessageBox.Show(strings.FailSavingMessage, strings.Save, MessageBoxButton.OK);
                        throw e.Error;
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            };
        }

        private void InitiateCollections()
        {
            RFQHeaderList = new ObservableCollection<RFSSubHeader>();
            RFQHeaderList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFSSubHeader item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                        var item1 = item;
                        item.DeleteSubHeader += (s1, e1) =>
                        {
                            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
                            if (res == MessageBoxResult.Cancel) return;
                            if (item1.ObjStatus.IsSavedDBItem)
                                RFQHeaderDeletedList.Add(item1);
                            RFQHeaderList.Remove(item1);
                        };
                    }

                if (e.OldItems != null)
                    foreach (RFSSubHeader item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Vendors = new ObservableCollection<Vendor>();
            Vendors.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (Vendor item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (Vendor item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Brands = new ObservableCollection<Brand>();
            Brands.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (Brand item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (Brand item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Seasons = new ObservableCollection<TblLkpSeason>();
            Seasons.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (TblLkpSeason item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (TblLkpSeason item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            RFQItems = new ObservableCollection<GenericViewModel>();
            RFQServices = new ObservableCollection<GenericViewModel>();
            RFQItems.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (GenericViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (GenericViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };
            RFQServices.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (GenericViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (GenericViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            FillGenericCollection("tbl_lkp_RFQItems", RFQItems);
            FillGenericCollection("tbl_lkp_RFQIServices", RFQServices);
            RFQGlobalLkps.CurrenciesList = new ObservableCollection<GenericViewModel>();
            RFQGlobalLkps.CostTypeList = new ObservableCollection<GenericViewModel>();
            FillGenericCollection("tbl_lkp_CostTypes", RFQGlobalLkps.CostTypeList);
            FillGenericCollection("tbl_lkp_Currency", RFQGlobalLkps.CurrenciesList);
            RFQGlobalLkps.FollowupStatusesList = new ObservableCollection<GenericViewModel>();
            RFQGlobalLkps.FollowupStatusesList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (GenericViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (GenericViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };
            FillGenericCollection("tbl_lkp_RFQStatus", RFQGlobalLkps.FollowupStatusesList);

            RFQGlobalLkps.StatisSizesCollection = new ObservableCollection<SizesWithGroups>();
            RFQGlobalLkps.StatisSizesCollection.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (SizesWithGroups item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems == null) return;
                foreach (SizesWithGroups item in e.OldItems)
                {
                    item.PropertyChanged
                        -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                }
            };
        }

        private void InitiateCommands()
        {
            AddNewSubHeaderCommand = new CommandsExecuter(AddNewDetail) { IsEnabled = true };

            ChangeObjStatusCommand = new CommandsExecuter(ChangeSelectedSubDetailsObjState) { IsEnabled = true };

            DeleteSubHeaderCommand = new CommandsExecuter(DeleteSubHeader) { IsEnabled = true };

            DeleteItemsCommand = new CommandsExecuter(DeleteItems) { IsEnabled = true };

            DeleteServiceCommand = new CommandsExecuter(DeleteService) { IsEnabled = true };

            SaveRFQCommand = new CommandsExecuter(SaveRfq) { IsEnabled = true };

            SearchRFQCommand = new CommandsExecuter(SearchRFQ) { IsEnabled = true };

            LoadCodeCommand = new CommandsExecuter(LoadRfq) { IsEnabled = true };

            CheckCodeCommand = new CommandsExecuter(CheckRFQDocCode) { IsEnabled = true };

            ToggleSearchCommand = new CommandsExecuter(ToggleInFormSearch) { IsEnabled = true };

            CancelCommand = new CommandsExecuter(Cancel) { IsEnabled = true };

            NewRFQCommand = new CommandsExecuter(NewRfq) { IsEnabled = true };
        }

        #endregion [ Initiators ]

        #region [ Commands bound methods ]

        /// <summary>
        /// Adding New Request For Qutation Sub Header.
        /// </summary>
        private void AddNewDetail()
        {
            if (RFQSelectedHeader != null)
            {
                if ((RFQHeaderList.IndexOf(RFQSelectedHeader)) == (RFQHeaderList.Count - 1))
                {
                    if (!RFQHeaderList[(RFQHeaderList.Count - 1)].ObjStatus.IsEmpty)
                    {
                        RFQHeaderList.Add(new RFSSubHeader(RFQItems, RFQServices) { ParentObj = this });
                        RFQSelectedHeader = RFQHeaderList[(RFQHeaderList.Count - 1)];
                        if (TransID != null)
                            RFQSelectedHeader.MainHeaderTransID = TransID;
                        RFQSelectedHeader.StyleSelected += (s, e) => InitiateAllStyleSizes();
                    }
                }
            }
            else
            {
                RFQHeaderList.Add(new RFSSubHeader(RFQItems, RFQServices) { ParentObj = this });
                RFQSelectedHeader = RFQHeaderList[(RFQHeaderList.Count - 1)];
                if (TransID != null)
                    RFQSelectedHeader.MainHeaderTransID = TransID;
            }
            RFQSelectedHeader.ObjStatus.IsEmpty = true;
            RFQSelectedHeader.ObjStatus.IsNew = true;
            RFQSelectedHeader.StyleSelected += (s, e) => InitiateAllStyleSizes();
        }

        /// <summary>
        /// Resetting Form mode to Standby
        /// </summary>
        private void Cancel()
        {
            ResetViewModel(ObjectMode.StandBy);
        }

        /// <summary>
        /// Change Sub Header Status on data change.
        /// </summary>
        private void ChangeSelectedSubDetailsObjState()
        {
            if (RFQSelectedHeader == null) return;

            if (RFQSelectedHeader.ObjStatus.IsEmpty)
                RFQSelectedHeader.ObjStatus.IsEmpty = false;

            if (RFQSelectedHeader.ObjStatus.IsSavedDBItem)
                RFQSelectedHeader.ObjStatus.IsChanged = true;
        }

        /// <summary>
        /// Checkes if the trans code exists, but not used right now
        /// </summary>
        private void CheckRFQDocCode()
        {
            //if (!string.IsNullOrEmpty(this.DocNum))
            //{
            //    _client.CheckDocNumAsync(this.DocNum.Trim());
            //}
        }

        private void DeleteItems()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQSelectedHeader.HeaderItems.Remove(RFQSelectedHeader.HeaderItemsSelectedItem);
        }

        private void DeleteService()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQSelectedHeader.HeaderServices.Remove(RFQSelectedHeader.HeaderSelectedService);
        }

        private void DeleteSubHeader()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQHeaderList.Remove(RFQSelectedHeader);
        }

        private void InitiateAllStyleSizes()
        {
            Client.GetStyleSizesFromRetailAsync
                    (
                        new ObservableCollection<string>
                            (
                                RFQHeaderList.Select(x => x.Style).ToList()
                            )
                    );
        }

        private void LoadRfq()
        {
            if ((IsLoaded || (FormMode == ObjectMode.StandBy)) && !IsSearchingMode) return;
            if (IsLoading) return;
            if (string.IsNullOrEmpty(DocNum)) return;
            IsLoading = true;
            Client.GetRFSAsync(DocNum.Trim());
        }

        private void NewRfq()
        {
            ResetViewModel(ObjectMode.NewObject);
        }

        private void SaveRfq()
        {
            try
            {
                switch (FormMode)
                {
                    case ObjectMode.NewObject:
                        SaveAllNew();
                        break;

                    case ObjectMode.LoadedFromDb:
                        Update();
                        break;

                    case ObjectMode.MarkedForDeletion:
                        Delete();
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(strings.FailSavingMessage, strings.Save, MessageBoxButton.OK);
            }
        }

        private void SearchRFQ()
        {
        }

        private void ToggleInFormSearch()
        {
            if (FormMode == ObjectMode.StandBy)
                IsSearchingMode = !IsSearchingMode;
            else
                IsSearchingMode = false;
        }

        private void ViewReport()
        {
            if (!ObjStatus.IsSavedDBItem) return;
            var reportName = "RFQReport";

            if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            { reportName = "RFQReport"; }
            var temp = DocNum;
            var para = new ObservableCollection<string> { temp };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        #endregion [ Commands bound methods ]

        #region [ Private Methods: Internal Logic ]

        public void FillColors()
        {
            var chk = BrandProp != null && SeasonProp != null;
            if (chk)
                Client.GetTblColorLinkAsync(0, 100, BrandProp.Brand_Code, 1, SeasonProp.Iserial, "it.TblBrand", null, null);
        }

        private void Delete()
        {
        }

        private void ResetViewModel(ObjectMode objMode)
        {
            FormMode = objMode;
            InitiateCollections();
            FillBasicCollections();
            TransID = null;
            ObjStatus = new ObjectStatus { IsNew = true };
            SupplierID = null;
            BrandCode = null;
            SeasonCode = null;
            SeasonProp = null;
            BrandProp = null;
            SupplierProp = null;
            DocNum = null;
            IsSearchingMode = false;
            AllFollowupApproved = false;
            IsLoaded = objMode == ObjectMode.NewObject || objMode == ObjectMode.LoadedFromDb;
        }

        private void SaveAllNew()
        {
            if (AllowAdd)
            {
                IsLoading = true;
                Client.UpdateOrInsertRFSCompleted += (s, e) =>
                {
                    try
                    {
                        if (e.Error == null)
                        {
                            MessageBox.Show("Document Number is: [ " + e.Result.DocNumber + " ]", strings.Save, MessageBoxButton.OK);
                            DocNum = e.Result.DocNumber;
                            ResetViewModel(ObjectMode.StandBy);
                        }
                        else
                        {
                            //MessageBox.Show(strings.FailSavingMessage, strings.Save, MessageBoxButton.OK);
                            throw e.Error;
                        }
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                };
                Client.UpdateOrInsertRFSAsync(RFSMVM_Mapper.MapToModel(this, true));
            }
            else
            {
                MessageBox.Show("You do not have the permission to add a new item!");
            }
        }

        private void Update()
        {
            if (AllowUpdate)
            {
                IsLoading = true;
                var addedSubHeaderList = new ObservableCollection<TblRFSDetail>();
                var addedSubHeaderItems = new ObservableCollection<TblRFSDetailItem>();
                var addedSubHeaderServices = new ObservableCollection<TblRFSDetailService>();

                var updatedSubHeaderList = new ObservableCollection<TblRFSDetail>();
                var updatedSubHeaderItems = new ObservableCollection<TblRFSDetailItem>();
                var updatedSubHeaderServices = new ObservableCollection<TblRFSDetailService>();

                var deletedSubHeaderList = new ObservableCollection<TblRFSDetail>();
                var deletedSubHeaderItems = new ObservableCollection<TblRFSDetailItem>();
                var deletedSubHeaderServices = new ObservableCollection<TblRFSDetailService>();

                foreach (var item in RFQHeaderDeletedList)
                {
                    deletedSubHeaderList.Add(RFSMVM_Mapper.MapToModel(item));
                }

                foreach (var item in RFQHeaderList)
                {
                    if (item.ObjStatus.IsNew)
                        addedSubHeaderList.Add(RFSMVM_Mapper.MapToModel(item));
                    else if (item.ObjStatus.IsSavedDBItem)
                        updatedSubHeaderList.Add(RFSMVM_Mapper.MapToModel(item));

                    foreach (var itms in item.HeaderItems)
                    {
                        if (itms.ObjStatus.IsNew)
                            addedSubHeaderItems.Add(RFSMVM_Mapper.MapToModel(itms));
                        else if (item.ObjStatus.IsSavedDBItem)
                            updatedSubHeaderItems.Add(RFSMVM_Mapper.MapToModel(itms));
                    }

                    foreach (var itms in item.HeaderItemsDeletedList)
                    {
                        deletedSubHeaderItems.Add(RFSMVM_Mapper.MapToModel(itms));
                    }

                    foreach (var srvs in item.HeaderServices)
                    {
                        if (srvs.ObjStatus.IsNew)
                            addedSubHeaderServices.Add(RFSMVM_Mapper.MapToModel(srvs));
                        else if (item.ObjStatus.IsSavedDBItem)
                            updatedSubHeaderServices.Add(RFSMVM_Mapper.MapToModel(srvs));
                    }

                    foreach (var srvs in item.HeaderServicesDeletedList)
                    {
                        deletedSubHeaderServices.Add(RFSMVM_Mapper.MapToModel(srvs));
                    }
                }

                try
                {
                    /////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////
                    //Client.UpdateRFQAsync(RFQMVM_Mapper.MapToModel(this, true));
                    Client.UpdateOrInsertRFSDetailAsync(updatedSubHeaderList, addedSubHeaderList, deletedSubHeaderList);
                    Client.UpdateOrInsertRFSDetailItemsAsync(updatedSubHeaderItems, addedSubHeaderItems, deletedSubHeaderItems);
                    Client.UpdateOrInsertRFSDetailServicesAsync(updatedSubHeaderServices, addedSubHeaderServices, deletedSubHeaderServices);
                    /////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////
                }
                catch (Exception)
                {
                    MessageBox.Show(strings.FailSavingMessage, strings.Save, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("You do not have the update permission");
            }
        }

        #endregion [ Private Methods: Internal Logic ]

        #region [ Private methods: Data Fillers ]

        private static void FillGenericCollection(string tablEname, ObservableCollection<GenericViewModel> objectToFill)
        {
            var serviceClient = new CRUD_ManagerServiceClient();
            serviceClient.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            serviceClient.GetGenericCompleted += (s, ev) =>
            {
                int i = 0;
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
            serviceClient.CloseAsync();
        }

        private void FillBasicCollections()
        {
            var serviceClient = new CRUD_ManagerServiceClient();
            serviceClient.GetVendorsCompleted += (a, b) =>
            {
                try
                {
                    foreach (var item in b.Result)
                    {
                        Vendors.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            };
        
            serviceClient.GetAllBrandsCompleted += (s, e) =>
            {
                try
                {
                    foreach (var item in e.Result)
                    {
                        Brands.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            };
            serviceClient.GetAllBrandsAsync(LoggedUserInfo.Iserial);

            serviceClient.GetAllSeasonsCompleted += (s, e) =>
            {
                foreach (var item in e.Result)
                {
                    Seasons.Add(item);
                }
            };
            serviceClient.GetAllSeasonsAsync();
        }

        #endregion [ Private methods: Data Fillers ]

        #region [ Permissions Handlers ]

        public override void InitiatePermissionsMapper()
        {
            base.InitiatePermissionsMapper();
        }

        private void ManageCustomePermissions()
        {
            GetCustomePermissions(FormName.ToString());
        }

        private void ManagePermissions()
        {
            GetItemPermissions(FormName.ToString());
        }

        #endregion [ Permissions Handlers ]
    }
}