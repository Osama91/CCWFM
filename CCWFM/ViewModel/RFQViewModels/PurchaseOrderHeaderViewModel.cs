using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class PurchaseOrderHeaderViewModel : ViewModelBase
    {
        #region [ Private members ]

        private DateTime _creationDate;
        private DateTime? _delivaryDate;
        private DateTime? _docDate;
        private int _grandTotal;
        private bool _isPosted;
        private DateTime _lastUpdateDate;
        private ObservableCollection<PurchasOrderDetailsViewModel> _purchaseOrderDetails;
        private int? _transID;

        private _Proxy.Vendor _vendor;

        private string _vendorCode;

        private ObservableCollection<_Proxy.Vendor> _vendors;

        private _Proxy.V_Warehouse _wareHouseItem;

        private ObservableCollection<_Proxy.V_Warehouse> _warehousesList;

        #endregion [ Private members ]

        #region [ public Members ]

        private ObjectStatus _objStatus;

        private RFQSubHeader _parentRfq;

        private ObservableCollection<PurchasOrderDetailsViewModel> _purchaseOrderDeletedDetails;

        private PurchasOrderDetailsViewModel _selectedPurchDetails;

        private string _warehouseCode;

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        public DateTime? DelivaryDate
        {
            get { return _delivaryDate; }
            set { _delivaryDate = value; RaisePropertyChanged("DelivaryDate"); }
        }

        public DateTime? DocDate
        {
            get { return _docDate; }
            set
            {
                _docDate = value;
                RaisePropertyChanged("DocDate");
            }
        }

        public int GrandTotal
        {
            get { return _grandTotal; }
            set { _grandTotal = value; RaisePropertyChanged("GrandTotal"); }
        }

        public bool IsPosted
        {
            get { return _isPosted; }
            set
            {
                _isPosted = value;
                RaisePropertyChanged("IsPosted");
            }
        }

        public DateTime LastUpdateDate
        {
            get { return _lastUpdateDate; }
            set { _lastUpdateDate = value; RaisePropertyChanged("LastUpdateDate"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set
            {
                _objStatus = value;
                RaisePropertyChanged("ObjStatus");
            }
        }

        public int? ParentId
        {
            get { return _arentId; }
            set
            {
                _arentId = value;
                RaisePropertyChanged("ParentId");
            }
        }

        public RFQSubHeader ParentRFQ
        {
            get { return _parentRfq; }
            set { _parentRfq = value; RaisePropertyChanged("ParentRFQ"); }
        }

        public ObservableCollection<PurchasOrderDetailsViewModel> PurchaseOrderDeletedDetails
        {
            get { return _purchaseOrderDeletedDetails ?? (_purchaseOrderDeletedDetails = new ObservableCollection<PurchasOrderDetailsViewModel>()); }
            set { _purchaseOrderDeletedDetails = value; RaisePropertyChanged("PurchaseOrderDeletedDetails"); }
        }

        public ObservableCollection<PurchasOrderDetailsViewModel> PurchaseOrderDetails
        {
            get { return _purchaseOrderDetails; }
            set { _purchaseOrderDetails = value; RaisePropertyChanged("PurchaseOrderDetails"); }
        }

        public string PurchId
        {
            get { return _purchId; }
            set
            {
                _purchId = value;
                RaisePropertyChanged("PurchId");
            }
        }

        public PurchasOrderDetailsViewModel SelectedPurchDetail
        {
            get { return _selectedPurchDetails; }
            set
            {
                _selectedPurchDetails = value;
                RaisePropertyChanged("SelectedPurchDetail");
            }
        }

        public int? TransID
        {
            get { return _transID; }
            set
            {
                _transID = value;
                RaisePropertyChanged("TransID");
            }
        }

        public _Proxy.Vendor Vendor
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                VendorCode = value.vendor_code;
                RaisePropertyChanged("Vendor");
            }
        }

        public string VendorCode
        {
            get { return _vendorCode; }
            set
            {
                _vendorCode = value;
                RaisePropertyChanged("VendorCode");
            }
        }

        public ObservableCollection<_Proxy.Vendor> Vendors
        {
            get { return _vendors; }
            set
            {
                _vendors = value;
                RaisePropertyChanged("Vendors");
            }
        }

        public _Proxy.V_Warehouse WareHouseItem
        {
            get { return _wareHouseItem; }
            set { _wareHouseItem = value; RaisePropertyChanged("WareHouseItem"); }
        }

        public ObservableCollection<_Proxy.V_Warehouse> WarehousesList
        {
            get { return _warehousesList; }
            set { _warehousesList = value; RaisePropertyChanged("WarehousesList"); }
        }

        public string WarHouseCode
        {
            get { return _warehouseCode; }
            set
            {
                _warehouseCode = value;
                RaisePropertyChanged("WarHouseCode");
            }
        }

        #endregion [ public Members ]

        #region [ Constructors ]

        public PurchaseOrderHeaderViewModel()
        {
            LoadViewModel();
        }

        #endregion [ Constructors ]

        #region [ Commands ]

        private int? _arentId;
        private CommandsExecuter _deletePoHeaderCommand;
        private CommandsExecuter _postToAxCommand;

        private string _purchId;

        private CommandsExecuter _repeatStyleCommand;

        public CommandsExecuter DeletePoHeaderCommand
        {
            get { return _deletePoHeaderCommand ?? (_deletePoHeaderCommand = new CommandsExecuter(DeletePOHeader) { IsEnabled = true }); }
        }

        public CommandsExecuter PostToAxCommand
        {
            get { return _postToAxCommand ?? (_postToAxCommand = new CommandsExecuter(PostPOheaderToax) { IsEnabled = true }); }
        }

        public CommandsExecuter RepeatStyleCommand
        {
            get { return _repeatStyleCommand ?? (_repeatStyleCommand = new CommandsExecuter(RepeatStyle) { IsEnabled = true }); }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void DeletePOHeader()
        {
        }

        private void PostPOheaderToax()
        {
            if (!ObjStatus.IsSavedDBItem)
            {
                MessageBox.Show("You cannot POST this before saving!");
                return;
            }
            var param = RFQMVM_Mapper.MapToModel(this);
            Client.PostPoToAxAsync(param);
        }

        private void RepeatStyle()
        {
            //if (SelectedPurchDetail == null) return;
            //var temp = new PurchasOrderDetailsViewModel
            //    {
            //        ColorsList = SelectedPurchDetail.ColorsList,
            //        StyleHeader = SelectedPurchDetail.StyleHeader,
            //        Price = SelectedPurchDetail.Price,
            //        RowTotal = SelectedPurchDetail.RowTotal
            //    };
            //var i = 0;
            //foreach (var sze in SelectedPurchDetail.PurchaseOrderSizes.Where(x => x.IsTextBoxEnabled))
            //{
            //    temp.PurchaseOrderSizes[i].SizeCode = sze.SizeCode;
            //    temp.PurchaseOrderSizes[i].IsTextBoxEnabled = true;
            //    i++;
            //}
            //PurchaseOrderDetails.Insert((PurchaseOrderDetails.IndexOf(SelectedPurchDetail) + 1),
            //    temp);
        }

        #endregion [ Commands bound methods ]

        #region [ Internal Logic ]

        public void LoadViewModel()
        {
            ObjStatus = new ObjectStatus { IsNew = true };
            Client = new _Proxy.CRUD_ManagerServiceClient();
            WarehousesList = new ObservableCollection<_Proxy.V_Warehouse>();
            WarehousesList.CollectionChanged += WarehousesList_CollectionChanged;
            CreationDate = DateTime.Now;
            Client.GetAllWarehousesByCompanyNameCompleted += (s, e) =>
            {
                foreach (var item in e.Result)
                {
                    WarehousesList.Add(item);
                }
            };
            Client.GetAllWarehousesByCompanyNameAsync("CCR");
            PurchaseOrderDetails = new ObservableCollection<PurchasOrderDetailsViewModel>();

            PurchaseOrderDetails.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (PurchasOrderDetailsViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) =>
                            {
                                RaisePropertyChanged(e1.PropertyName);
                                if (e1.PropertyName == "RowTotal")
                                    GrandTotal = PurchaseOrderDetails.Sum(x => x.RowTotal);
                            };

                        var item1 = item;
                        item.DeletePurchLine += (ss, ee) =>
                        {
                            var res = MessageBox.Show("Delete?", "", MessageBoxButton.OKCancel);
                            if (res == MessageBoxResult.Cancel) return;
                            PurchaseOrderDetails.Remove(item1);
                        };
                    }

                if (e.OldItems != null)
                    foreach (PurchasOrderDetailsViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                        var item1 = item;
                        item.DeletePurchLine -= ((ss, ee) =>
                        {
                            if (item1.ObjStatus.IsSavedDBItem)
                                PurchaseOrderDeletedDetails.Add(item1);
                            PurchaseOrderDetails.Remove(item1);
                        });
                    }
            };

            Client.PostPoToAxCompleted += (s, e)
                =>
            {
                MessageBox
                    .Show(e.Error == null ? "PO Successfully Posted" : "PO Was not posted");
                IsPosted = e.Error == null;
            }

    ;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void WarehousesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (_Proxy.V_Warehouse item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (_Proxy.V_Warehouse item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        #endregion [ Internal Logic ]
    }
}