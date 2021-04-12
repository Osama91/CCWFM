using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.Defaults;
using CCWFM.ViewModel;
using CCWFM.ViewModel.PurchaseOrderViewModel;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModels.PurchaseOrderViewModels
{
    /************************************************************/

    public class PurchaseOrderHeaderViewModel : ViewModelBase
    {
        #region[ Data Members ]
        private int? _transId;

        public int? TransID
        {
            get { return _transId; }
            set
            {
                _transId = value;
                RaisePropertyChanged("TransID");
            }
        }

        private string _vendorCode;

        public string VendorCode
        {
            get { return _vendorCode; }
            set
            {
                _vendorCode = value;
                RaisePropertyChanged("VendorCode");
            }
        }

        private _Proxy.Vendor _vendor;

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

        private DateTime? _recieveDate;

        public DateTime? RecieveDate
        {
            get { return _recieveDate; }
            set
            {
                _recieveDate = value;
                RaisePropertyChanged("RecieveDate");
            }
        }

        private DateTime? _delivaryDate;

        public DateTime? DelivaryDate
        {
            get { return _delivaryDate; }
            set { _delivaryDate = value; RaisePropertyChanged("DelivaryDate"); }
        }

        private bool? _isPosted;

        public bool? IsPosted
        {
            get { return _isPosted; }
            set
            {
                _isPosted = value;
                RaisePropertyChanged("IsPosted");
            }
        }

        private DateTime _creationDate;

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private DateTime _lastUpdateDate;

        public DateTime LastUpdateDate
        {
            get { return _lastUpdateDate; }
            set { _lastUpdateDate = value; RaisePropertyChanged("LastUpdateDate"); }
        }

        private ObservableCollection<PurchasOrderDetailsViewModel> _purchaseOrderDetails;

        public ObservableCollection<PurchasOrderDetailsViewModel> PurchaseOrderDetails
        {
            get { return _purchaseOrderDetails; }
            set { _purchaseOrderDetails = value; RaisePropertyChanged("PurchaseOrderDetails"); }
        }

        private ObservableCollection<_Proxy.Vendor> _vendors;

        public ObservableCollection<_Proxy.Vendor> Vendors
        {
            get { return _vendors; }
            set
            {
                _vendors = value;
                RaisePropertyChanged("Vendors");
            }
        }

        private int _grandTotal;

        public int GrandTotal
        {
            get { return _grandTotal; }
            set { _grandTotal = value; RaisePropertyChanged("GrandTotal"); }
        }

        private _Proxy.V_Warehouse _wareHouseItem;

        public _Proxy.V_Warehouse WareHouseItem
        {
            get { return _wareHouseItem; }
            set { _wareHouseItem = value; RaisePropertyChanged("WareHouseItem"); }
        }

        private ObservableCollection<_Proxy.V_Warehouse> _warehousesList;

        public ObservableCollection<_Proxy.V_Warehouse> WarehousesList
        {
            get { return _warehousesList; }
            set { _warehousesList = value; RaisePropertyChanged("WarehousesList"); }
        }

        #endregion

        #region [ Constructors ]

        public PurchaseOrderHeaderViewModel()
        {
            LoadViewModel();
        }

        private void Vendors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (_Proxy.Vendor item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (_Proxy.Vendor item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// what is the table name ?
        /// </summary>
        /// <param name="header"></param>
        public PurchaseOrderHeaderViewModel(_Proxy.tbl_PurchaseOrderHeader header)
        {
            LoadViewModel(header);
        }

        public void LoadViewModel()
        {
            Vendors = new ObservableCollection<_Proxy.Vendor>();
            Vendors.CollectionChanged += Vendors_CollectionChanged;
            WarehousesList = new ObservableCollection<_Proxy.V_Warehouse>();
            WarehousesList.CollectionChanged += WarehousesList_CollectionChanged;
            CreationDate = DateTime.Now;
            var client = new _Proxy.CRUD_ManagerServiceClient();
            
            client.GetVendorsCompleted += (a, b) =>
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

            client.GetAllWarehousesByCompanyNameCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        WarehousesList.Add(item);
                    }
                };
            client.GetAllWarehousesByCompanyNameAsync("CCR");
            PurchaseOrderDetails = new ObservableCollection<PurchasOrderDetailsViewModel>();
            client.CloseAsync();
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

        public void LoadViewModel(_Proxy.tbl_PurchaseOrderHeader header)
        {
            TransID = header.TransID;
            VendorCode = header.Vendor;
            Vendor = Vendors.FirstOrDefault(x => x.vendor_code == VendorCode);
            IsPosted = header.IsPosted;
            WareHouseItem = WarehousesList.FirstOrDefault(x => x.WarehouseID == header.WareHouseID);
            try
            {
                DelivaryDate = header.DelivaryDate;
                CreationDate = (DateTime)header.CreationDate;
            }
            catch (Exception)
            {
            }

            //_client.CloseAsync();
        }

        #endregion

        #region [ CRUD ]

        public void SaveRouteCard(string saveMode)
        {
            if (saveMode.ToLower() == "add")
            {
                try
                {
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
            else
            {
                try
                {
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
        }

        #endregion
    }
}