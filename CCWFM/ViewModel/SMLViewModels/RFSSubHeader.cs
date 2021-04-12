using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;
using CCWFM.Views.StylePages;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class RFSSubHeader : ViewModelBase
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

        private DateTime _delivaryDate;

        private string _desc;

        private byte[] _headerImage;

        private ObservableCollection<RFSHeaderItem> _headerItems;

        private RFSHeaderItem _headerItemsSelectedItem;

        private RFSHeaderServices _headerSelectedService;

        private ObservableCollection<RFSHeaderServices> _headerServices;

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

        private RFSViewModel _parentObj;

        public RFSViewModel ParentObj
        {
            get { return _parentObj; }
            set
            {
                _parentObj = value;
                RaisePropertyChanged("ParentObj");
            }
        }

        private ObservableCollection<TblColor> _colorList;

        public ObservableCollection<TblColor> ColorList
        {
            get { return _colorList; }
            set
            {
                _colorList = value;
                RaisePropertyChanged("TblColor");
            }
        }

        private ObservableCollection<RFSHeaderItem> _headerItemsDeletedList;

        private ObservableCollection<RFSHeaderServices> _headerServicesDeletedList;

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

        public byte[] HeaderImage
        {
            get { return _headerImage; }
            set { _headerImage = value; RaisePropertyChanged("HeaderImage"); }
        }

        public ObservableCollection<RFSHeaderItem> HeaderItems
        {
            get { return _headerItems; }
            set { _headerItems = value; RaisePropertyChanged("HeaderItems"); }
        }

        public ObservableCollection<RFSHeaderItem> HeaderItemsDeletedList
        {
            get { return _headerItemsDeletedList ?? (_headerItemsDeletedList = new ObservableCollection<RFSHeaderItem>()); }
            set { _headerItemsDeletedList = value; RaisePropertyChanged("HeaderItemsDeletedList"); }
        }

        public RFSHeaderItem HeaderItemsSelectedItem
        {
            get { return _headerItemsSelectedItem; }
            set { _headerItemsSelectedItem = value; RaisePropertyChanged("HeaderItemsSelectedItem"); }
        }

        public RFSHeaderServices HeaderSelectedService
        {
            get { return _headerSelectedService; }
            set { _headerSelectedService = value; RaisePropertyChanged("HeaderSelectedService"); }
        }

        public ObservableCollection<RFSHeaderServices> HeaderServices
        {
            get { return _headerServices; }
            set { _headerServices = value; RaisePropertyChanged("HeaderServices"); }
        }

        public ObservableCollection<RFSHeaderServices> HeaderServicesDeletedList
        {
            get { return _headerServicesDeletedList ?? (_headerServicesDeletedList = new ObservableCollection<RFSHeaderServices>()); }
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

        public RFSSubHeader(IEnumerable<GenericViewModel> itemsList, IEnumerable<GenericViewModel> servicesList)
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
            ColorList = new ObservableCollection<TblColor>();
            ColorList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (TblColor item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (TblColor item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            foreach (var colorNew in RFQGlobalLkps.ColorsList)
            {
                ColorList.Add(colorNew);
            }
            HeaderItems = new ObservableCollection<RFSHeaderItem>();
            HeaderItems.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach (RFSHeaderItem item in e.NewItems)
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
                        foreach (RFSHeaderItem item in e.OldItems)
                        {
                            item.PropertyChanged
                                -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                        }
                };

            HeaderServices = new ObservableCollection<RFSHeaderServices>();
            HeaderServices.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFSHeaderServices item in e.NewItems)
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
                    foreach (RFSHeaderServices item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
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

            GetStyleCommand = new CommandsExecuter(GetStyleData) { IsEnabled = true };

            ChangeCostFollowObjStatusCommand = new CommandsExecuter(ChangeSelectedItemDetailsObjState)
            {
                IsEnabled = false
            };

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

        private void AddNewImage()
        {
            byte[] temp;
            GlobalMethods.BrowseImage(out temp);
            HeaderImage = temp;
        }

        private void AddNewItem()
        {
            HeaderItems.Add(new RFSHeaderItem(RFQItems));
            HeaderItemsSelectedItem = HeaderItems[HeaderItems.Count - 1];
            if (SubHeaderSerial != null)
                HeaderItemsSelectedItem.ParentID = SubHeaderSerial;
        }

        private void AddNewServices()
        {
            HeaderServices.Add(new RFSHeaderServices(RFQServices));
            HeaderSelectedService = HeaderServices[HeaderServices.Count - 1];
            HeaderSelectedService.ParentID = SubHeaderSerial;
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

        private void GetStyleData()
        {
            var styleGetterChild = new RFSGetStyleChild
                {
                    SeasonCode = ParentObj.SeasonCode,
                    BrandCode = ParentObj.BrandCode
                };
            styleGetterChild.SubmitActions += (s, e) =>
                {
                    Style = e.Style;
                    Desc = e.Description;
                    ColorCode = e.ColorCode;
                    SizeCodes = new List<string>(e.Sizes);
                    RaisePropertyChanged("SizeCodes");
                    ObjStatus.IsEmpty = false;
                    OnStyleSelected();
                };
            styleGetterChild.Show();
        }

        private List<string> _sizeCodes;

        public List<string> SizeCodes
        {
            get { return _sizeCodes; }
            set { _sizeCodes = value; RaisePropertyChanged("SizeCode"); }
        }

        private string _sizeCode;

        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                _sizeCode = value;
                RaisePropertyChanged("SizeCode");
            }
        }

        public string ColorCode
        {
            get { return _colorCode; }
            set
            {
                _colorCode = value;
                RaisePropertyChanged("ColorCode");
            }
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
        private string _colorCode;

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