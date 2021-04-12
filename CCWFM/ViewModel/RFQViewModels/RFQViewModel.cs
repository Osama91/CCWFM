using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Defaults;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using _proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.RFQViewModels
{
    public sealed class RFQViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private string _brandCode;
        private _proxy.Brand _brandProp;
        private ObservableCollection<_proxy.Brand> _brands;

        //private _proxy.CRUD_ManagerServiceClient _client = new _proxy.CRUD_ManagerServiceClient();
        private string _docNum;

        private ObjectMode _formMode;
        private PermissionItemName _formName;
        private bool _isCodeExists;
        private bool _isLoading;
        private bool _isSearchingMode;
        private ObjectStatus _objstatus;
        private PurchaseOrderHeaderViewModel _purchaseHeaderSelectedItem;
        private ObservableCollection<PurchaseOrderHeaderViewModel> _purchHeaders;
        private ObservableCollection<RFQSubHeader> _rfqHeaderList;
        private ObservableCollection<GenericViewModel> _rfqItems;
        private RFQSubHeader _rfqSelectedHeader;
        private ObservableCollection<GenericViewModel> _rfqServices;
        private string _seasonCode;
        private _proxy.TblLkpSeason _seasonProp;
        private ObservableCollection<_proxy.TblLkpSeason> _seasons;
        private string _supplierId;
        private _proxy.Vendor _supplierProp;
        private int? _transId;
        private ObservableCollection<_proxy.Vendor> _vendors;
        #endregion [ Private Fields ]

        #region [ Public Properties ]

        private bool _AllfollowupApproved;

        private ObservableCollection<PurchaseOrderHeaderViewModel> _deletedPurchHeaders;

        private ObservableCollection<RFQSubHeader> _rfqHeaderDeletedList;

        public bool AllFollowupApproved
        {
            get { return _AllfollowupApproved; }
            set
            {
                _AllfollowupApproved = value;
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
        public _proxy.Brand BrandProp
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

        public ObservableCollection<_proxy.Brand> Brands
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

        public PurchaseOrderHeaderViewModel PurchaseHeaderSelectedItem
        {
            get { return _purchaseHeaderSelectedItem; }
            set { _purchaseHeaderSelectedItem = value; RaisePropertyChanged("PurchaseHeaderSelectedItem"); }
        }

        public ObservableCollection<PurchaseOrderHeaderViewModel> PurchHeaders
        {
            get { return _purchHeaders; }
            set
            {
                _purchHeaders = value;
                RaisePropertyChanged("PurchHeaders");
            }
        }

        public ObservableCollection<RFQSubHeader> RFQHeaderDeletedList
        {
            get { return _rfqHeaderDeletedList ?? (_rfqHeaderDeletedList = new ObservableCollection<RFQSubHeader>()); }
            set { _rfqHeaderDeletedList = value; RaisePropertyChanged("RFQHeaderDeletedList"); }
        }

        public ObservableCollection<RFQSubHeader> RFQHeaderList
        {
            get { return _rfqHeaderList; }
            set { _rfqHeaderList = value; RaisePropertyChanged("RFQHeaderList"); }
        }

        public ObservableCollection<GenericViewModel> RFQItems
        {
            get { return _rfqItems; }
            set { _rfqItems = value; RaisePropertyChanged("RFQItems"); }
        }

        public RFQSubHeader RFQSelectedHeader
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
        public _proxy.TblLkpSeason SeasonProp
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

        public ObservableCollection<_proxy.TblLkpSeason> Seasons
        {
            get { return _seasons; }
            set { _seasons = value; RaisePropertyChanged("Seasons"); }
        }

        public ObservableCollection<_proxy.SizesWithGroups> SizesCollection
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
        public _proxy.Vendor SupplierProp
        {
            get { return _supplierProp; }
            set { _supplierProp = value; RaisePropertyChanged("SupplierProp"); }
        }

        public int? TransID
        {
            get { return _transId; }
            set { _transId = value; RaisePropertyChanged("TransID"); }
        }

        public ObservableCollection<_proxy.Vendor> Vendors
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
        private CommandsExecuter _deleteRFQ;
        private CommandsExecuter _deleteServiceCommand;
        private CommandsExecuter _deleteSubHeaderCommand;
        private bool _isLoaded;
        private CommandsExecuter _loadCodeCommand;
        private CommandsExecuter _newRFQCommand;
        private CommandsExecuter _savePurchaseOrderCommand;
        private CommandsExecuter _saveRFQCommand;
        private CommandsExecuter _searchRFQCommand;
        private ObservableCollection<_proxy.SizesWithGroups> _sizesCollection;
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
            get { return _deleteRFQ; }
            set { _deleteRFQ = value; RaisePropertyChanged("DeleteRFQ"); }
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
            get { return _newRFQCommand; }
            set { _newRFQCommand = value; RaisePropertyChanged("NewRFQCommand"); }
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
            get { return _saveRFQCommand; }
            set { _saveRFQCommand = value; RaisePropertyChanged("SaveRFQCommand"); }
        }

        public CommandsExecuter SearchRFQCommand
        {
            get { return _searchRFQCommand; }
            set { _searchRFQCommand = value; RaisePropertyChanged("SearchRFQCommand"); }
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

        public RFQViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new _proxy.CRUD_ManagerServiceClient();
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
            Client.UpdateRFQCompleted += (s, e) =>
            {
            };
            Client.UpdateRFQSubHeaderCompleted += (s, e) =>
            {
            };
            Client.UpdateRFQSubHeaderItemsCompleted += (s, e) =>
            {
            };

            Client.UpdateRFQSubHeaderServicesCompleted += (s, e) =>
            {
            };

            Client.UpdateRFQFollowUpsCompleted += (s, e) =>
            {
            };
            Client.UpdateRFQCostFollowUpsCompleted += (s, e) =>
            {
            };
            Client.UpdatePurchHeaderCompleted += (s, e) =>
            {
            };
            Client.UpdatePurchlineAdditionalCostsCompleted += (s, e) =>
            {
            };
            Client.UpdateAdditionalCostsCompleted += (s, e) =>
            {
            };
            Client.UpdatePurchDetailsCompleted += (s, e) =>
            {
                MessageBox.Show(e.Error == null ? strings.SavedMessage : strings.FailSavingMessage
                    , strings.Save, MessageBoxButton.OK);
                IsLoading = false;
            };

            Client.DeleteRFQCompleted += (s, e) =>
            {
                if (e.Result)
                {
                    MessageBox.Show("RFQ Was deleted!");
                    ResetViewModel(ObjectMode.StandBy);
                }
                else
                {
                    MessageBox.Show("RFQ Was Not Deleted!");
                }
            };

            Client.GetRFQCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error != null) return;
                    RFQGlobalLkps.ColorsList = new ObservableCollection<_proxy.TblColor>(e.colorsList);
                    //RFQGlobalLkps.StatisSizesCollection = new ObservableCollection<_proxy.SizesWithGroups>();
                    if (e.Result != null)
                    {
                        RFQMVM_Mapper.MapToViewModel(e.Result, this);
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
                        MessageBox.Show("Document nuumber does not exist");
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            };

            //Client.GetColorsByBrandSeasonCompleted += (s, e) =>
            //{
            //    if (e.Result == null || e.Error != null) return;
            //    RFQGlobalLkps.ColorsList = new ObservableCollection<_proxy.TblColor>();
            //    RFQGlobalLkps.ColorsList.Clear();
            //    foreach (var item in e.Result)
            //    {
            //        RFQGlobalLkps.ColorsList.Add(item);
            //    }
            //};

            Client.GetStyleSizesFromRetailCompleted += (s, e) =>
            {
                if (e.Error != null && e.Result == null) return;
                var res = e.Result;

                RFQGlobalLkps.StatisSizesCollection.Clear();
                foreach (var siz in e.Result)
                {
                    RFQGlobalLkps.StatisSizesCollection.Add(siz);
                }
                if (PurchaseHeaderSelectedItem == null) return;
                if (PurchaseHeaderSelectedItem.PurchaseOrderDetails == null) return;
                foreach (var detail in PurchaseHeaderSelectedItem.PurchaseOrderDetails)
                {
                    var detail1 = detail;
                    var i = 0;
                    foreach (var item in res.Where(x => x.StyleCode == detail1.StyleHeader))
                    {
                        var temp = detail.PurchaseOrderSizes[i];
                        var tempSizeInfo = item;
                        temp.SizeCode = tempSizeInfo.SizeCode;
                        temp.SizeGroup = tempSizeInfo.SizeGroup;
                        temp.IsTextBoxEnabled = true;
                        i++;
                    }
                }
                PurchaseHeaderSelectedItem.SelectedPurchDetail = PurchaseHeaderSelectedItem.PurchaseOrderDetails[0];
            };
        }

        private void InitiateCollections()
        {
            RFQHeaderList = new ObservableCollection<RFQSubHeader>();
            RFQHeaderList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFQSubHeader item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) =>
                            {
                                RaisePropertyChanged(e1.PropertyName);
                                AllFollowupApproved = RFQHeaderList.Any(x => x.FollowupsApproved);
                            };
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
                    foreach (RFQSubHeader item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Vendors = new ObservableCollection<_proxy.Vendor>();
            Vendors.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_proxy.Vendor item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (_proxy.Vendor item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Brands = new ObservableCollection<_proxy.Brand>();
            Brands.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_proxy.Brand item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (_proxy.Brand item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            Seasons = new ObservableCollection<_proxy.TblLkpSeason>();
            Seasons.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_proxy.TblLkpSeason item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (_proxy.TblLkpSeason item in e.OldItems)
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

            PurchHeaders = new ObservableCollection<PurchaseOrderHeaderViewModel>();
            PurchHeaders.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (PurchaseOrderHeaderViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems == null) return;
                foreach (PurchaseOrderHeaderViewModel item in e.OldItems)
                {
                    item.PropertyChanged
                        -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                }
            };

            RFQGlobalLkps.StatisSizesCollection = new ObservableCollection<_proxy.SizesWithGroups>();
            RFQGlobalLkps.StatisSizesCollection.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_proxy.SizesWithGroups item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems == null) return;
                foreach (_proxy.SizesWithGroups item in e.OldItems)
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

            DeleteFollowUpCommand = new CommandsExecuter(DeleteFollowUpLine) { IsEnabled = true };

            DeleteCostFollowUpCommand = new CommandsExecuter(DeleteCostFollowUp) { IsEnabled = true };

            DeleteSubHeaderCommand = new CommandsExecuter(DeleteSubHeader) { IsEnabled = true };

            DeleteItemsCommand = new CommandsExecuter(DeleteItems) { IsEnabled = true };

            DeleteServiceCommand = new CommandsExecuter(DeleteService) { IsEnabled = true };

            SaveRFQCommand = new CommandsExecuter(SaveRfq) { IsEnabled = true };

            SearchRFQCommand = new CommandsExecuter(SearchRFQ) { IsEnabled = true };

            LoadCodeCommand = new CommandsExecuter(LoadRfq) { IsEnabled = true };

            CheckCodeCommand = new CommandsExecuter(CheckRFQDocCode) { IsEnabled = true };

            ToggleSearchCommand = new CommandsExecuter(ToggleInFormSearch) { IsEnabled = true };

            DeleteRFQCommand = new CommandsExecuter(DeleteRfq) { IsEnabled = true };

            CancelCommand = new CommandsExecuter(Cancel) { IsEnabled = true };

            NewRFQCommand = new CommandsExecuter(NewRfq) { IsEnabled = true };

            AddNewPurchCommand = new CommandsExecuter(AddPurchaseOrder) { IsEnabled = true };
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
                        RFQHeaderList.Add(new RFQSubHeader(RFQItems, RFQServices));
                        RFQSelectedHeader = RFQHeaderList[(RFQHeaderList.Count - 1)];
                        if (TransID != null)
                            RFQSelectedHeader.MainHeaderTransID = TransID;
                        RFQSelectedHeader.StyleSelected += (s, e) => InitiateAllStyleSizes();
                    }
                }
            }
            else
            {
                RFQHeaderList.Add(new RFQSubHeader(RFQItems, RFQServices));
                RFQSelectedHeader = RFQHeaderList[(RFQHeaderList.Count - 1)];
                if (TransID != null)
                    RFQSelectedHeader.MainHeaderTransID = TransID;
            }
            RFQSelectedHeader.ObjStatus.IsEmpty = true;
            RFQSelectedHeader.ObjStatus.IsNew = true;
            RFQSelectedHeader.StyleSelected += (s, e) => InitiateAllStyleSizes();
        }

        private void AddPurchaseOrder()
        {
            if (PurchHeaders == null) return;
            var temp = new PurchaseOrderCreationChildViewModel();
            foreach (var rfqSubHeader in RFQHeaderList.Where(x => x.FollowupsApproved))
            {
                temp.RFQHeaderList.Add(rfqSubHeader);
            }
            temp.SubmitSelectedStyles += (s, e) =>
            {
                PurchHeaders.Add(new PurchaseOrderHeaderViewModel
                {
                    Vendor = SupplierProp
                });
                PurchaseHeaderSelectedItem = PurchHeaders[PurchHeaders.Count - 1];
                PurchaseHeaderSelectedItem.ParentId = TransID;
                var purchaseOrderCreationChildViewModel = s as PurchaseOrderCreationChildViewModel;
                if (purchaseOrderCreationChildViewModel == null) return;
                foreach (var item in purchaseOrderCreationChildViewModel.SelectedStyles)
                {
                    foreach (var purchStyleClr in item.CostFollowUpList.Where(x => x.IsApproved))
                    {
                        PurchaseHeaderSelectedItem.PurchaseOrderDetails.Add(new PurchasOrderDetailsViewModel
                                    {
                                        StyleHeader = item.Style,
                                        ParentRfqSub = item,
                                        RowTotal = item.Qty,
                                        StyleColor = purchStyleClr.StyleColor,
                                        AdditionalCostList = new ObservableCollection<_proxy.tbl_PurchaseOrder_AdditionalCost>
                                            (
                                                purchStyleClr.AdditionalCostList.Select(x => new _proxy.tbl_PurchaseOrder_AdditionalCost
                                                    {
                                                        CostType = x.CostType,
                                                        CostValue = x.CostValue,
                                                        Currency = x.Currency,
                                                        ExchangeRate = (float)x.ExchangeRate,
                                                        LocalValue = x.LocalValue
                                                    })
                                            ),
                                        AdditionalCost = (decimal)purchStyleClr.AdditionalCost,
                                        Price = (decimal)purchStyleClr.LocalCost
                                    });
                    }
                }
                InitiateStyleSizesForPurch();
            };
            temp.InitiateSearch();
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

        /// <summary>
        /// Removes an entry from the cost followup list
        /// </summary>
        private void DeleteCostFollowUp()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQSelectedHeader.CostFollowUpList.Remove(RFQSelectedHeader.CostFollowUpItem);
        }

        private void DeleteFollowUpLine()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQSelectedHeader.FollowUpList.Remove(RFQSelectedHeader.FollowUpItem);
        }

        private void DeleteItems()
        {
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK) return;
            RFQSelectedHeader.HeaderItems.Remove(RFQSelectedHeader.HeaderItemsSelectedItem);
        }

        private void DeleteRfq()
        {
            if (AllowDelete)
            {
                var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
                if (res != MessageBoxResult.OK)
                {
                }
                else
                {
                    Client.DeleteRFQAsync(RFQMVM_Mapper.MapToModel(this, true));
                }
            }
            else
            {
                MessageBox.Show("You do not have the DELETE permission");
            }
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

        private void InitiateStyleSizesForPurch()
        {
            Client.GetStyleSizesFromRetailAsync
                    (
                        new ObservableCollection<string>
                            (
                                PurchaseHeaderSelectedItem.PurchaseOrderDetails.Select(x => x.StyleHeader).ToList()
                            )
                    );
        }

        private void LoadRfq()
        {
            if ((IsLoaded || (FormMode == ObjectMode.StandBy)) && !IsSearchingMode) return;
            if (IsLoading) return;
            if (string.IsNullOrEmpty(DocNum)) return;
            IsLoading = true;
            Client.GetRFQAsync(DocNum.Trim());
        }

        private void NewRfq()
        {
            ResetViewModel(ObjectMode.NewObject);
        }

        private void SaveRfq()
        {
            try
            {
                if (!PurchHeaders.All(
                    x => x.PurchaseOrderDetails.All(d => d.ObjStatus.IsReadyForSaving))) return;
                //if (!ObjStatus.IsReadyForSaving) return;
                //if(!PurchHeaders.All(
                //    x => x.ObjStatus.IsReadyForSaving && x.PurchaseOrderDetails.All(d => d.ObjStatus.IsReadyForSaving)))return;
                //if (!RFQHeaderList.All(
                //    x => x.ObjStatus.IsReadyForSaving
                //         && x.CostFollowUpList.All(d => d.ObjStatus.IsReadyForSaving)
                //         && x.FollowUpList.All(d => d.ObjStatus.IsReadyForSaving)
                //         && x.HeaderItems.All(d => d.ObjStatus.IsReadyForSaving))) return;
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
                Client.GetTblColorLinkAsync(0, 1000, BrandProp.Brand_Code, 1, SeasonProp.Iserial, "it.Iserial", null, null);
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
                Client.SaveAllNewRFQCompleted += (s, e) =>
                {
                    try
                    {
                        if (e.Error == null)
                        {
                            MessageBox.Show("Document Number is: [ " + e.Result + " ]", strings.Save, MessageBoxButton.OK);
                            DocNum = e.Result;
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
                Client.SaveAllNewRFQAsync(RFQMVM_Mapper.MapToModel(this, true));
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
                var addedSubHeaderList = new ObservableCollection<_proxy.tbl_RFQDetail>();
                var addedSubHeaderItems = new ObservableCollection<_proxy.tbl_RFQDetailItem>();
                var addedSubHeaderServices = new ObservableCollection<_proxy.tbl_RFQDetailService>();
                var addedFollowUps = new ObservableCollection<_proxy.tbl_RFQFollowup>();
                var addedPurchaseHeaders = new ObservableCollection<_proxy.tbl_PurchaseOrderHeader>();
                var addedPurchaseDetails = new ObservableCollection<_proxy.tbl_PurchaseOrderDetails>();
                var additionalCostLists = new ObservableCollection<_proxy.tbl_RFQ_AdditionalCost>();
                var additionalPurchLineCostLists = new ObservableCollection<_proxy.tbl_PurchaseOrder_AdditionalCost>();

                var updatedSubHeaderList = new ObservableCollection<_proxy.tbl_RFQDetail>();
                var updatedSubHeaderItems = new ObservableCollection<_proxy.tbl_RFQDetailItem>();
                var updatedSubHeaderServices = new ObservableCollection<_proxy.tbl_RFQDetailService>();
                var updatedFollowUps = new ObservableCollection<_proxy.tbl_RFQFollowup>();
                var updatedPurchaseHeaders = new ObservableCollection<_proxy.tbl_PurchaseOrderHeader>();
                var updatedPurchaseDetails = new ObservableCollection<_proxy.tbl_PurchaseOrderDetails>();

                var deletedSubHeaderList = new ObservableCollection<_proxy.tbl_RFQDetail>();
                var deletedSubHeaderItems = new ObservableCollection<_proxy.tbl_RFQDetailItem>();
                var deletedSubHeaderServices = new ObservableCollection<_proxy.tbl_RFQDetailService>();
                var deletedFollowUps = new ObservableCollection<_proxy.tbl_RFQFollowup>();
                var deletedPurchaseHeaders = new ObservableCollection<_proxy.tbl_PurchaseOrderHeader>();
                var deletedPurchaseDetails = new ObservableCollection<_proxy.tbl_PurchaseOrderDetails>();

                foreach (var item in RFQHeaderDeletedList)
                {
                    deletedSubHeaderList.Add(RFQMVM_Mapper.MapToModel(item));
                }

                foreach (var item in RFQHeaderList)
                {
                    if (item.ObjStatus.IsNew)
                        addedSubHeaderList.Add(RFQMVM_Mapper.MapToModel(item));
                    else if (item.ObjStatus.IsSavedDBItem)
                        updatedSubHeaderList.Add(RFQMVM_Mapper.MapToModel(item));

                    foreach (var itms in item.HeaderItems)
                    {
                        if (itms.ObjStatus.IsNew)
                            addedSubHeaderItems.Add(RFQMVM_Mapper.MapToModel(itms));
                        else if (item.ObjStatus.IsSavedDBItem)
                            updatedSubHeaderItems.Add(RFQMVM_Mapper.MapToModel(itms));
                    }

                    foreach (var itms in item.HeaderItemsDeletedList)
                    {
                        deletedSubHeaderItems.Add(RFQMVM_Mapper.MapToModel(itms));
                    }

                    foreach (var srvs in item.HeaderServices)
                    {
                        if (srvs.ObjStatus.IsNew)
                            addedSubHeaderServices.Add(RFQMVM_Mapper.MapToModel(srvs));
                        else if (item.ObjStatus.IsSavedDBItem)
                            updatedSubHeaderServices.Add(RFQMVM_Mapper.MapToModel(srvs));
                    }

                    foreach (var srvs in item.HeaderServicesDeletedList)
                    {
                        deletedSubHeaderServices.Add(RFQMVM_Mapper.MapToModel(srvs));
                    }

                    foreach (var followUp in item.FollowUpList)
                    {
                        if (followUp.ObjStatus.IsNew)
                            addedFollowUps.Add(RFQMVM_Mapper.MapToModel(followUp));
                        else if (item.ObjStatus.IsSavedDBItem)
                            updatedFollowUps.Add(RFQMVM_Mapper.MapToModel(followUp));
                    }

                    foreach (var followUp in item.FollowupDeletedList)
                    {
                        deletedFollowUps.Add(RFQMVM_Mapper.MapToModel(followUp));
                    }

                    foreach (var costfollowUp in item.CostFollowUpList)
                    {
                        foreach (var citem in costfollowUp.AdditionalCostList)
                        {
                            if (costfollowUp.Iserial != null) citem.Tbl_RFQFollowup = (int)costfollowUp.Iserial;
                            additionalCostLists.Add(citem);
                        }

                        //if (costfollowUp.ObjStatus.IsNew)
                        //    addedFollowUps.Add(RFQMVM_Mapper.MapToModel(costfollowUp));
                        //else if (item.ObjStatus.IsSavedDBItem)
                        //    updatedFollowUps.Add(RFQMVM_Mapper.MapToModel(costfollowUp));
                    }

                    foreach (var costfollowUp in item.CostFollowupDeleted)
                    {
                        deletedFollowUps.Add(RFQMVM_Mapper.MapToModel(costfollowUp));
                    }
                }

                foreach (var purchHeader in PurchHeaders)
                {
                    if (purchHeader.ObjStatus.IsNew)
                        addedPurchaseHeaders.Add(RFQMVM_Mapper.MapToModel(purchHeader, (int)TransID));
                    else if (purchHeader.ObjStatus.IsSavedDBItem)
                    {
                        updatedPurchaseHeaders.Add(RFQMVM_Mapper.MapToModel(purchHeader, (int)TransID));

                        foreach (var purchDetail in purchHeader.PurchaseOrderDetails)
                        {
                            if (purchDetail.ObjStatus.IsNew)
                                addedPurchaseDetails.Add(RFQMVM_Mapper.MapToModel(purchDetail));
                            else if (purchDetail.ObjStatus.IsSavedDBItem)
                            {
                                foreach (var additionalCost in purchDetail.AdditionalCostList)
                                {
                                    additionalPurchLineCostLists.Add(additionalCost);
                                }
                                updatedPurchaseDetails.Add(RFQMVM_Mapper.MapToModel(purchDetail));
                            }
                        }

                        foreach (var podetails in purchHeader.PurchaseOrderDeletedDetails.Where(podetails => podetails.ObjStatus.IsSavedDBItem))
                        {
                            deletedPurchaseDetails.Add(RFQMVM_Mapper.MapToModel(podetails));
                        }
                    }
                }

                try
                {
                    /////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////
                    //Client.UpdateRFQAsync(RFQMVM_Mapper.MapToModel(this, true));
                    Client.UpdateRFQSubHeaderAsync(updatedSubHeaderList, addedSubHeaderList, deletedSubHeaderList);
                    Client.UpdateRFQSubHeaderItemsAsync(updatedSubHeaderItems, addedSubHeaderItems, deletedSubHeaderItems);
                    Client.UpdateRFQSubHeaderServicesAsync(updatedSubHeaderServices, addedSubHeaderServices, deletedSubHeaderServices);
                    Client.UpdateRFQFollowUpsAsync(updatedFollowUps, addedFollowUps, deletedFollowUps);
                    Client.UpdateAdditionalCostsAsync(additionalCostLists);
                    Client.UpdatePurchlineAdditionalCostsAsync(additionalPurchLineCostLists);
                    Client.UpdatePurchHeaderAsync(addedPurchaseHeaders, updatedPurchaseHeaders, deletedPurchaseHeaders, BrandCode, SeasonCode);
                    Client.UpdatePurchDetailsAsync(addedPurchaseDetails, updatedPurchaseDetails, deletedPurchaseDetails);
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
            var serviceClient = new _proxy.CRUD_ManagerServiceClient();
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
            var serviceClient = new _proxy.CRUD_ManagerServiceClient();
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