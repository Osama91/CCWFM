using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.RequestForQutation;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class RFQSubHeader : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler DeleteSubHeader;

        public virtual void OnDeleteSubHeader()
        {
            var handler = DeleteSubHeader;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Private fields ]

        private RFQCostFollowUp _costFollowUpItem;

        private ObservableCollection<RFQCostFollowUp> _costFollowUpList;

        private DateTime _delivaryDate;

        private string _desc;

        private RFQFollowUpViewModel _followUpItem;

        private ObservableCollection<RFQFollowUpViewModel> _followUpItemsList;

        private byte[] _headerImage;

        private ObservableCollection<RFQHeaderItem> _headerItems;

        private RFQHeaderItem _headerItemsSelectedItem;

        private RFQHeaderServices _headerSelectedService;

        private ObservableCollection<RFQHeaderServices> _headerServices;

        private bool _isFollowUpReady;

        private bool? _isSampleAttached;

        private string _mainFabDesc;

        private string _notes;

        private ObjectStatus _objStatus;

        private int _qty;

        private string _style;

        private string _subFabDesc;

        public event EventHandler StyleSelected;

        protected virtual void OnStyleSelected()
        {
            var handler = StyleSelected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Private fields ]

        #region [ Public Properties ]

        private ObservableCollection<RFQCostFollowUp> _costFollowUpDeleted;
        private bool _followupsApproves;

        private ObservableCollection<RFQFollowUpViewModel> _followupUpDeletedList;

        private ObservableCollection<RFQHeaderItem> _headerItemsDeletedList;

        private ObservableCollection<RFQHeaderServices> _headerServicesDeletedList;

        public ObservableCollection<RFQCostFollowUp> CostFollowupDeleted
        {
            get
            {
                return _costFollowUpDeleted
                    ??
                    (
                        _costFollowUpDeleted = new ObservableCollection<RFQCostFollowUp>()
                    );
            }
            set
            {
                _costFollowUpDeleted = value;
                RaisePropertyChanged("CostFollowupDeleted");
            }
        }

        public RFQCostFollowUp CostFollowUpItem
        {
            get { return _costFollowUpItem; }
            set { _costFollowUpItem = value; RaisePropertyChanged("CostFollowUpItem"); }
        }

        public ObservableCollection<RFQCostFollowUp> CostFollowUpList
        {
            get { return _costFollowUpList; }
            set { _costFollowUpList = value; RaisePropertyChanged("CostFollowUpList"); }
        }

        public DateTime DelivaryDate
        {
            get { return _delivaryDate; }
            set { _delivaryDate = value; RaisePropertyChanged("DelivaryDate"); }
        }

        public string Desc
        {
            get { return _desc; }
            set { _desc = value; RaisePropertyChanged("Desc"); }
        }

        public ObservableCollection<RFQFollowUpViewModel> FollowupDeletedList
        {
            get { return _followupUpDeletedList ?? (_followupUpDeletedList = new ObservableCollection<RFQFollowUpViewModel>()); }
            set { _followupUpDeletedList = value; RaisePropertyChanged("FollowupDeletedList"); }
        }

        public RFQFollowUpViewModel FollowUpItem
        {
            get { return _followUpItem; }
            set { _followUpItem = value; RaisePropertyChanged("FollowUpItem"); }
        }

        public ObservableCollection<RFQFollowUpViewModel> FollowUpList
        {
            get { return _followUpItemsList; }
            set { _followUpItemsList = value; RaisePropertyChanged("FollowUpList"); }
        }

        public bool FollowupsApproved
        {
            get { return _followupsApproves; }
            set
            {
                _followupsApproves = value;
                RaisePropertyChanged("FollowupsApproved");
            }
        }

        public byte[] HeaderImage
        {
            get { return _headerImage; }
            set { _headerImage = value; RaisePropertyChanged("HeaderImage"); }
        }

        public ObservableCollection<RFQHeaderItem> HeaderItems
        {
            get { return _headerItems; }
            set { _headerItems = value; RaisePropertyChanged("HeaderItems"); }
        }

        public ObservableCollection<RFQHeaderItem> HeaderItemsDeletedList
        {
            get { return _headerItemsDeletedList ?? (_headerItemsDeletedList = new ObservableCollection<RFQHeaderItem>()); }
            set { _headerItemsDeletedList = value; RaisePropertyChanged("HeaderItemsDeletedList"); }
        }

        public RFQHeaderItem HeaderItemsSelectedItem
        {
            get { return _headerItemsSelectedItem; }
            set { _headerItemsSelectedItem = value; RaisePropertyChanged("HeaderItemsSelectedItem"); }
        }

        public RFQHeaderServices HeaderSelectedService
        {
            get { return _headerSelectedService; }
            set { _headerSelectedService = value; RaisePropertyChanged("HeaderSelectedService"); }
        }

        public ObservableCollection<RFQHeaderServices> HeaderServices
        {
            get { return _headerServices; }
            set { _headerServices = value; RaisePropertyChanged("HeaderServices"); }
        }

        public ObservableCollection<RFQHeaderServices> HeaderServicesDeletedList
        {
            get { return _headerServicesDeletedList ?? (_headerServicesDeletedList = new ObservableCollection<RFQHeaderServices>()); }
            set { _headerServicesDeletedList = value; RaisePropertyChanged("HeaderServicesDeletedList"); }
        }

        public bool IncludedInPurchase
        {
            get { return _includedInPurchase; }
            set
            {
                _includedInPurchase = value;
                RaisePropertyChanged("IncludedInPurchase");
            }
        }

        public bool IsFollowUpReady
        {
            get { return _isFollowUpReady; }
            set { _isFollowUpReady = value; RaisePropertyChanged("IsFollowUpReady"); }
        }

        public bool? IsSampleAttached
        {
            get { return _isSampleAttached; }
            set { _isSampleAttached = value; RaisePropertyChanged("IsSampleAttached"); }
        }

        public string MainFabDesc
        {
            get { return _mainFabDesc; }
            set { _mainFabDesc = value; RaisePropertyChanged("MainFabDesc"); }
        }

        public int? MainHeaderTransID
        {
            get { return _mainHeaderTransID; }
            set { _mainHeaderTransID = value; RaisePropertyChanged("MainHeaderTransID"); }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set { _objStatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        public PurchaseOrderHeaderViewModel PurchaseOrderProp
        {
            get { return _purchaseOrderField; }
            set
            {
                _purchaseOrderField = value;
                RaisePropertyChanged("PurchaseOrderProp`");
            }
        }

        [Required]
        public int Qty
        {
            get { return _qty; }
            set { _qty = value; RaisePropertyChanged("Qty"); }
        }

        public ObservableCollection<GenericViewModel> RFQItems
        {
            get { return _rfqItems; }
            set { _rfqItems = value; RaisePropertyChanged("RFQItems"); }
        }

        public ObservableCollection<GenericViewModel> RFQServices
        {
            get { return _rfqServices; }
            set { _rfqServices = value; RaisePropertyChanged("RFQServices"); }
        }

        public string Style
        {
            get { return _style; }
            set { _style = value; RaisePropertyChanged("Style"); }
        }

        public string SubFabDesc
        {
            get { return _subFabDesc; }
            set { _subFabDesc = value; RaisePropertyChanged("SubFabDesc"); }
        }

        public int? SubHeaderSerial
        {
            get { return _subHeaderSerial; }
            set { _subHeaderSerial = value; RaisePropertyChanged("SubHeaderSerial"); }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public RFQSubHeader(IEnumerable<GenericViewModel> itemsList, IEnumerable<GenericViewModel> servicesList)
        {
            InitializeObjStatus();
            InitializeCommands();
            InitializeCollections();
            InitializeDates();
            foreach (var item in itemsList)
            {
                RFQItems.Add(item);
            }

            foreach (var item in servicesList)
            {
                RFQServices.Add(item);
            }
        }

        #endregion [ Constructor(s) ]

        #region [ Initiators ]

        private void InitializeCollections()
        {
            HeaderItems = new ObservableCollection<RFQHeaderItem>();
            HeaderItems.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach (RFQHeaderItem item in e.NewItems)
                        {
                            item.PropertyChanged
                                += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                            var item1 = item;
                            item.DeleteDetailItem += (ss, ee) =>
                            {
                                if (item1.ObjStatus.IsSavedDBItem)
                                    HeaderItemsDeletedList.Add(item1);
                                HeaderItems.Remove(item1);
                            };
                        }

                    if (e.OldItems != null)
                        foreach (RFQHeaderItem item in e.OldItems)
                        {
                            item.PropertyChanged
                                -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                        }
                };

            HeaderServices = new ObservableCollection<RFQHeaderServices>();
            HeaderServices.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFQHeaderServices item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                        //DeleteService
                        var item1 = item;
                        item.DeleteService += (ss, ee) =>
                        {
                            if (item1.ObjStatus.IsSavedDBItem)
                                HeaderServicesDeletedList.Add(item1);
                            HeaderServices.Remove(item1);
                        };
                    }

                if (e.OldItems != null)
                    foreach (RFQHeaderServices item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            FollowUpList = new ObservableCollection<RFQFollowUpViewModel>();
            FollowUpList.CollectionChanged += (s, e) =>
            {
                FollowupsApproved = FollowUpList.Any(x => x.IsApproved) &&
                                                        CostFollowUpList.Any(x => x.IsApproved);
                if (e.NewItems != null)
                    foreach (RFQFollowUpViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) =>
                            {
                                RaisePropertyChanged(e1.PropertyName);
                                FollowupsApproved = FollowUpList.Any(x => x.IsApproved) &&
                                                    CostFollowUpList.Any(x => x.IsApproved);
                            };
                        var item1 = item;
                        item.ResampleAction += (s1, e1) =>
                        {
                            FollowUpList
                                .Insert(FollowUpList.IndexOf((s1 as RFQFollowUpViewModel)) + 1
                                    , new RFQFollowUpViewModel() { Style = Style, ParentID = SubHeaderSerial });
                            FollowUpList[FollowUpList.IndexOf((s1 as RFQFollowUpViewModel)) + 1].StatusID = 4;

                            FollowUpList[FollowUpList.IndexOf((s1 as RFQFollowUpViewModel)) + 1].SizeCode
                                =
                            FollowUpList[FollowUpList.IndexOf((s1 as RFQFollowUpViewModel))].SizeCode;

                            var upViewModel = s1 as RFQFollowUpViewModel;
                            if (upViewModel != null && upViewModel.ObjStatus.IsNew)
                            {
                                item1.ObjStatus.IsEmpty = true;
                                item1.ObjStatus.IsNew = true;
                            }
                            else
                            {
                                var followUpViewModel = s1 as RFQFollowUpViewModel;
                                if (followUpViewModel == null || !followUpViewModel.ObjStatus.IsSavedDBItem) return;
                                var rfqFollowUpViewModel = s1 as RFQFollowUpViewModel;
                                rfqFollowUpViewModel.ObjStatus.IsChanged = true;
                            }
                        };

                        //DeleteFollowup
                        item.DeleteFollowup += (ss, ee) =>
                        {
                            if (item1.ObjStatus.IsSavedDBItem)
                                FollowupDeletedList.Add(item1);
                            FollowUpList.Remove(item1);
                        };
                    }

                if (e.OldItems != null)
                    foreach (RFQFollowUpViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));

                        item.ResampleAction -= new EventHandler((s1, e1) =>
                        {
                        });
                    }
            };

            CostFollowUpList = new ObservableCollection<RFQCostFollowUp>();
            CostFollowUpList.CollectionChanged += (s, e) =>
            {
                FollowupsApproved = FollowUpList.Any(x => x.IsApproved) &&
                                                        CostFollowUpList.Any(x => x.IsApproved);
                if (e.NewItems != null)
                    //foreach (RFQCostFollowUp item in e.NewItems)
                    //{
                    //    item.PropertyChanged
                    //        += (s1, e1) =>
                    //        {
                    //            RaisePropertyChanged(e1.PropertyName);
                    //            FollowupsApproved = FollowUpList.Any(x => x.IsApproved) &&
                    //                                CostFollowUpList.Any(x => x.IsApproved);
                    //        };
                    //    var item1 = item;
                    //    item.ResampleAction += ((s1, e1) =>
                    //    {
                    //        CostFollowUpList
                    //            .Insert(CostFollowUpList.IndexOf((s1 as RFQCostFollowUp)) + 1
                    //            , new RFQCostFollowUp() { Style = Style, ParentID = SubHeaderSerial });
                    //        CostFollowUpList[CostFollowUpList.IndexOf((s1 as RFQCostFollowUp)) + 1].Status = 4;
                    //        var rfqCostFollowUp = s1 as RFQCostFollowUp;
                    //        if (rfqCostFollowUp != null && rfqCostFollowUp.ObjStatus.IsNew)
                    //        {
                    //            item1.ObjStatus.IsEmpty = true;
                    //            item1.ObjStatus.IsNew = true;
                    //        }
                    //        else
                    //        {
                    //            var costFollowUp = s1 as RFQCostFollowUp;
                    //            if (costFollowUp != null && costFollowUp.ObjStatus.IsSavedDBItem)
                    //            {
                    //                (s1 as RFQCostFollowUp).ObjStatus.IsChanged = true;
                    //            }
                    //        }
                    //    });
                    //    //DeleteCostingFollowup
                    //    item.DeleteCostingFollowup += (ss, ee) =>
                    //    {
                    //        if (item1.ObjStatus.IsSavedDBItem)
                    //            CostFollowupDeleted.Add(item1);
                    //        CostFollowUpList.Remove(item1);
                    //    };
                    //}

                    if (e.OldItems != null)
                        foreach (RFQCostFollowUp item in e.OldItems)
                        {
                            item.PropertyChanged
                                -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                            item.ResampleAction -= new EventHandler((s1, e1) =>
                            {
                            });
                        }
            };

            RFQItems = new ObservableCollection<GenericViewModel>();

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
                            -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            RFQServices = new ObservableCollection<GenericViewModel>();

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
                            -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };
        }

        private void InitializeCommands()
        {
            AddImageCommand = new CommandsExecuter(AddNewImage) { IsEnabled = true };

            AddNewItemCommand = new CommandsExecuter(AddNewItem) { IsEnabled = true };

            AddNewServiceCommand = new CommandsExecuter(AddNewServices) { IsEnabled = true };

            AddFollowupCommand = new CommandsExecuter(AddFollowUp) { IsEnabled = true };

            AddCostFollowUpCommand = new CommandsExecuter(AddCostFollowUp) { IsEnabled = true };

            GetStyleCommand = new CommandsExecuter(GetStyleData) { IsEnabled = true };

            CheckRowStatusCommand = new CommandsExecuter(CheckRowStatus) { IsEnabled = true };

            ChangeCostFollowObjStatusCommand = new CommandsExecuter(ChangeSelectedItemDetailsObjState)
            {
                IsEnabled = false
            };

            ChangeFollowObjStatusCommand = new CommandsExecuter(ChangeSelectedFollowUpObjState) { IsEnabled = true };

            ChangeServiceObjStatusCommand = new CommandsExecuter(ChangeSelectedServiceObjState) { IsEnabled = true };

            ChangeItemObjStatusCommand = new CommandsExecuter(ChangeSelectedItemDetailsObjState) { IsEnabled = true };
        }

        private void InitializeDates()
        {
            DelivaryDate = DateTime.Now;
        }

        private void InitializeObjStatus()
        {
            if (ObjStatus == null)
            {
                ObjStatus = new ObjectStatus { IsChanged = false, IsMarkedForDeletion = false };
            }
        }

        #endregion [ Initiators ]

        #region [ Commands bound methods ]

        private void AddCostFollowUp()
        {
            if (CostFollowUpList.Any(x => x.IsRejected)) return;
            if (CostFollowUpItem != null)
            {
                if ((CostFollowUpList.IndexOf(CostFollowUpItem)) != (CostFollowUpList.Count - 1)) return;
                //    if (CostFollowUpList[CostFollowUpList.Count - 1].ObjStatus.IsEmpty) return;
                CostFollowUpList.Add(new RFQCostFollowUp { Style = Style, ParentID = SubHeaderSerial });
                //    CostFollowUpItem = CostFollowUpList[CostFollowUpList.Count - 1];
                //    CostFollowUpItem.ObjStatus.IsEmpty = true;
                //    CostFollowUpItem.ObjStatus.IsNew = true;
                CostFollowUpItem.Parent = this;
            }
            else
            {
                CostFollowUpList.Add(new RFQCostFollowUp { Style = Style, ParentID = SubHeaderSerial });
                CostFollowUpItem = CostFollowUpList[CostFollowUpList.Count - 1];
                //    CostFollowUpItem.ObjStatus.IsEmpty = true;
                //   CostFollowUpItem.ObjStatus.IsNew = true;
                CostFollowUpItem.Parent = this;
            }
        }

        private void AddFollowUp()
        {
            if (FollowUpList.Any(x => x.IsRejected)) return;
            if (FollowUpItem != null)
            {
                if ((FollowUpList.IndexOf(FollowUpItem)) != (FollowUpList.Count - 1)) return;
                if (FollowUpList[FollowUpList.Count - 1].ObjStatus.IsEmpty) return;
                FollowUpList.Add(new RFQFollowUpViewModel { Style = Style, ParentID = SubHeaderSerial });
                FollowUpItem = FollowUpList[FollowUpList.Count - 1];
                FollowUpItem.ObjStatus.IsEmpty = true;
                FollowUpItem.ObjStatus.IsNew = true;
                FollowUpItem.SizesCollection.Clear();
                foreach (var size in RFQGlobalLkps.StatisSizesCollection.Where(x => x.StyleCode == Style))
                {
                    FollowUpItem.SizesCollection.Add(size);
                }
            }
            else
            {
                FollowUpList.Add(new RFQFollowUpViewModel { Style = Style, ParentID = SubHeaderSerial });
                FollowUpItem = FollowUpList[FollowUpList.Count - 1];
                FollowUpItem.ObjStatus.IsEmpty = true;
                FollowUpItem.ObjStatus.IsNew = true;
                FollowUpItem.SizesCollection.Clear();
                foreach (var size in RFQGlobalLkps.StatisSizesCollection.Where(x => x.StyleCode == Style))
                {
                    FollowUpItem.SizesCollection.Add(size);
                }
            }
        }

        private void AddNewImage()
        {
            byte[] temp;
            GlobalMethods.BrowseImage(out temp);
            HeaderImage = temp;
        }

        private void AddNewItem()
        {
            HeaderItems.Add(new RFQHeaderItem(RFQItems));
            HeaderItemsSelectedItem = HeaderItems[HeaderItems.Count - 1];
            if (SubHeaderSerial != null)
                HeaderItemsSelectedItem.ParentID = SubHeaderSerial;
        }

        private void AddNewServices()
        {
            HeaderServices.Add(new RFQHeaderServices(RFQServices));
            HeaderSelectedService = HeaderServices[HeaderServices.Count - 1];
            HeaderSelectedService.ParentID = SubHeaderSerial;
        }

        private void ChangeSelectedFollowUpObjState()
        {
            if (FollowUpItem.ObjStatus.IsEmpty)
                FollowUpItem.ObjStatus.IsEmpty = false;

            if (FollowUpItem.ObjStatus.IsSavedDBItem)
                FollowUpItem.ObjStatus.IsChanged = true;
        }

        private void ChangeSelectedItemDetailsObjState()
        {
            if (HeaderItemsSelectedItem.ObjStatus.IsEmpty)
                HeaderItemsSelectedItem.ObjStatus.IsEmpty = false;

            if (HeaderItemsSelectedItem.ObjStatus.IsSavedDBItem)
                HeaderItemsSelectedItem.ObjStatus.IsChanged = true;
        }

        private void ChangeSelectedServiceObjState()
        {
            if (HeaderSelectedService.ObjStatus.IsEmpty)
                HeaderSelectedService.ObjStatus.IsEmpty = false;

            if (HeaderSelectedService.ObjStatus.IsSavedDBItem)
                HeaderSelectedService.ObjStatus.IsChanged = true;
        }

        private void CheckRowStatus()
        {
            CostFollowUpItem.IsRejected = CostFollowUpItem.StatusProp.Iserial == 1;
        }

        private void GetStyleData()
        {
            var styleGetterChild = new RFQGetStyleChild();
            styleGetterChild.SubmitActions += (s, e) =>
                {
                    Style = e.Style;
                    Desc = e.Description;
                    ObjStatus.IsEmpty = false;
                    OnStyleSelected();
                };
            styleGetterChild.Show();
        }

        #endregion [ Commands bound methods ]

        #region [ Commands ]

        private CommandsExecuter _addCostFollowUpCommand;
        private CommandsExecuter _addFollowUpCommand;
        private CommandsExecuter _addImageCommand;
        private CommandsExecuter _addNewItem;
        private CommandsExecuter _addNewService;
        private CommandsExecuter _changeFollowObjStatusCommand;
        private CommandsExecuter _changeItemObjStatusCommand;
        private CommandsExecuter _changeObjStatusCommand;
        private CommandsExecuter _changeServiceObjStatusCommand;
        private CommandsExecuter _checkRowStatusCommand;
        private CommandsExecuter _deletSubHeaderCommand;
        private CommandsExecuter _getStyleCommand;
        private bool _includedInPurchase;
        private int? _mainHeaderTransID;
        private PurchaseOrderHeaderViewModel _purchaseOrderField;
        private ObservableCollection<GenericViewModel> _rfqItems;
        private ObservableCollection<GenericViewModel> _rfqServices;
        private int? _subHeaderSerial;

        public CommandsExecuter AddCostFollowUpCommand
        {
            get { return _addCostFollowUpCommand; }
            set { _addCostFollowUpCommand = value; RaisePropertyChanged("AddCostFollowUpCommand"); }
        }

        public CommandsExecuter AddFollowupCommand
        {
            get { return _addFollowUpCommand; }
            set { _addFollowUpCommand = value; RaisePropertyChanged("AddFollowupCommand"); }
        }

        public CommandsExecuter AddImageCommand
        {
            get { return _addImageCommand; }
            set { _addImageCommand = value; RaisePropertyChanged("AddImageCommand"); }
        }

        public CommandsExecuter AddNewItemCommand
        {
            get { return _addNewItem; }
            set { _addNewItem = value; RaisePropertyChanged("AddNewItem"); }
        }

        public CommandsExecuter AddNewServiceCommand
        {
            get { return _addNewService; }
            set { _addNewService = value; RaisePropertyChanged("AddNewService"); }
        }

        public CommandsExecuter ChangeCostFollowObjStatusCommand
        {
            get { return _changeObjStatusCommand; }
            set { _changeObjStatusCommand = value; RaisePropertyChanged("ChangeObjStatusCommand"); }
        }

        public CommandsExecuter ChangeFollowObjStatusCommand
        {
            get { return _changeFollowObjStatusCommand; }
            set { _changeFollowObjStatusCommand = value; RaisePropertyChanged("ChangeFollowObjStatusCommand"); }
        }

        public CommandsExecuter ChangeItemObjStatusCommand
        {
            get { return _changeItemObjStatusCommand; }
            set { _changeItemObjStatusCommand = value; RaisePropertyChanged("ChangeItemObjStatusCommand"); }
        }

        public CommandsExecuter ChangeServiceObjStatusCommand
        {
            get { return _changeServiceObjStatusCommand; }
            set { _changeServiceObjStatusCommand = value; RaisePropertyChanged("ChangeServiceObjStatusCommand"); }
        }

        public CommandsExecuter CheckRowStatusCommand
        {
            get { return _checkRowStatusCommand; }
            set { _checkRowStatusCommand = value; RaisePropertyChanged("CheckRowStatusCommand"); }
        }

        public CommandsExecuter DeleteSubHederCommand
        {
            get
            {
                return _deletSubHeaderCommand
                    ?? (_deletSubHeaderCommand = new CommandsExecuter(OnDeleteSubHeader) { IsEnabled = true });
            }
        }

        public CommandsExecuter GetStyleCommand
        {
            get { return _getStyleCommand; }
            set { _getStyleCommand = value; RaisePropertyChanged("GetStyleCommand"); }
        }

        #endregion [ Commands ]
    }
}