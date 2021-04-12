using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using _Proxy = CCWFM.CRUDManagerService;
using _Proxy2 = CCWFM.LoginService;

namespace CCWFM.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public bool IsDesignTime
        {
            get
            {
                return (Application.Current == null) ||
                       (Application.Current.GetType() == typeof(Application));
            }
        }

        #region [ Basic Permissions ]

      
        private bool _allowAdd;

        private bool _allowDelete;

        private bool _allowUpdate;

        public bool AllowAdd
        {
            get { return _allowAdd; }
            set { _allowAdd = value; RaisePropertyChanged("AllowAdd"); }
        }

        public bool AllowDelete
        {
            get { return _allowDelete; }
            set { _allowDelete = value; RaisePropertyChanged("AllowDelete"); }
        }

        public bool AllowUpdate
        {
            get { return _allowUpdate; }
            set { _allowUpdate = value; RaisePropertyChanged("AllowUpdate"); }
        }

        #endregion [ Basic Permissions ]

        #region [ Properties ]

        #region CustomDataGridProperties

        public string DataGridName { get; set; }

        public Dictionary<string, object> ValuesObjects { get; set; }

        public Dictionary<string, object> DetailValuesObjects { get; set; }

        public Dictionary<string, object> DetailSubValuesObjects { get; set; }

        public Dictionary<string, object> DetailsValuesObjects { get; set; }

        public bool Export { get; set; }

        public bool AllowExport
        {
            get { return _allowExport; }
            set { _allowExport = value; RaisePropertyChanged("AllowExport"); }
        }

        public DataGrid ExportGrid { get; set; }

        private int _fullcount;

        public int FullCount
        {
            get { return _fullcount; }
            set { _fullcount = value; RaisePropertyChanged("FullCount"); }
        }

        private int _detailfullcount;

        public int DetailFullCount
        {
            get { return _detailfullcount; }
            set { _detailfullcount = value; RaisePropertyChanged("DetailFullCount"); }
        }

        private int _detailSubfullcount;

        public int DetailSubFullCount
        {
            get { return _detailSubfullcount; }
            set { _detailSubfullcount = value; RaisePropertyChanged("DetailSubFullCount"); }
        }

        public string SortBy { get; set; }

        public string DetailSortBy { get; set; }

        public string DetailSubSortBy { get; set; }

        public string Filter { get; set; }

        public string DetailFilter { get; set; }

        public string DetailSubFilter { get; set; }

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("Code"); }
        }



        public bool Loading
        {
            get { return _loading; }
            set { _loading = value; RaisePropertyChanged("Loading"); }
        }

        public int PageSize { get; set; }

        #endregion CustomDataGridProperties

        private ObservableCollection<CustomePermissionsMapper> _permissionsMapper;

        public ObservableCollection<CustomePermissionsMapper> PermissionsMapper
        {
            get { return _permissionsMapper ?? (_permissionsMapper = new ObservableCollection<CustomePermissionsMapper>()); }
            set { _permissionsMapper = value; RaisePropertyChanged("PermissionsMapper"); }
        }

        private _Proxy2.TblAuthJobPermission _permissions;

        public _Proxy2.TblAuthJobPermission Permissions
        {
            get { return _permissions; }
            set { _permissions = value; RaisePropertyChanged("Permissions"); }
        }

        private ObservableCollection<_Proxy.TblAuthPermission> _customePermissions;

        public ObservableCollection<_Proxy.TblAuthPermission> CustomePermissions
        {
            get { return _customePermissions ?? (_customePermissions = new ObservableCollection<_Proxy.TblAuthPermission>()); }
            set { _customePermissions = value; RaisePropertyChanged("CustomePermissions"); }
        }
        private GlServiceClient _glclient;

        public GlServiceClient Glclient
        {
            get
            {
                return _glclient ?? (_glclient = new GlServiceClient());
            }
            set { _glclient = value; RaisePropertyChanged("Glclient"); }
        }


        private _Proxy.CRUD_ManagerServiceClient _client;

        public _Proxy.CRUD_ManagerServiceClient Client
        {
            get
            {
                return _client ?? (
                    _client = new _Proxy.CRUD_ManagerServiceClient()
                    );
            }

            set { _client = value; RaisePropertyChanged("Client"); }
        }

       

        public EventHandler PremCompleted;

        private bool _isPermissionLoaded;
        private bool _loading;
        private bool _allowExport;

        public bool IsPermissionsLoaded
        {
            get { return _isPermissionLoaded; }
            set { _isPermissionLoaded = value; RaisePropertyChanged("IsPermissionsLoaded"); }
        }

        #endregion [ Properties ]

        ///There an issue around where this constructor is called many times
        ///whith no intention to do so!

        #region [ Contstructor(s) ]

        public ViewModelBase()
        {
            //MessageBox.Show("This is a call from base");
            //_client = new _Proxy.CRUD_ManagerServiceClient();

            Loading = false;
            PageSize = 20;
        }

        #endregion [ Contstructor(s) ]

        #region [ Basic permission Initiation ]

        protected void GetItemPermissions(string itemName)
        {
            Client.GetItemsPermissionsCompleted += (s, ev) =>
            {
                if (ev.Error != null) return;
                Permissions = LoggedUserInfo.WFM_UserJobPermissions.FirstOrDefault(x => x.TblPermission == ev.Result);

                if (Permissions != null)
                {
                    AllowAdd = Permissions.AllowNew != null && (bool)Permissions.AllowNew;
                    AllowUpdate = Permissions.AllowUpdate != null && (bool)Permissions.AllowUpdate;
                    AllowDelete = Permissions.AllowDelete != null && (bool)Permissions.AllowDelete;
                }
                else
                {
                    AllowAdd = false;
                    AllowUpdate = false;
                    AllowDelete = false;
                }
                AfterItemPermissionsCompleted();
            };
            Client.GetItemsPermissionsAsync(itemName);
        }
        public virtual void AfterItemPermissionsCompleted() { }

        public virtual void InitiatePermissionsMapper()
        {
            CustomePermissions = new ObservableCollection<_Proxy.TblAuthPermission>();
            CustomePermissions.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_Proxy.TblAuthPermission item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (_Proxy.TblAuthPermission item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }
            };
            PermissionsMapper = new ObservableCollection<CustomePermissionsMapper>();
            PermissionsMapper.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (CustomePermissionsMapper item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (CustomePermissionsMapper item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }
            };
        }

        protected virtual void GetCustomePermissions(string parentItemName)
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetItemsPermissionsByParentCompleted += (s, e) =>
            {
                var temp = (from z in e.Result
                            where (from x in LoggedUserInfo.WFM_UserJobPermissions
                                   select x.TblPermission).Contains(z.Iserial)
                            select z).ToList();

                foreach (var item in temp)
                {
                    if (!CustomePermissions.Contains(item))
                    {
                        CustomePermissions.Add(item);
                    }
                }
                foreach (var item in PermissionsMapper)
                {
                    var c = CustomePermissions.SingleOrDefault(x => x.Code == item.PermissionKey);

                    item.PermissionValue = c != null;
                }
                var handler = PremCompleted;
                if (handler != null) handler(this, EventArgs.Empty);
            };

            client.GetItemsPermissionsByParentAsync(parentItemName);
        }

        #endregion [ Basic permission Initiation ]
    }

    public class CustomePermissionsMapper : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private string _permissionKey;

        public string PermissionKey
        {
            get { return _permissionKey; }
            set { _permissionKey = value; RaisePropertyChanged("PermissionKey"); }
        }

        private bool _permissionsValue;

        public bool PermissionValue
        {
            get { return _permissionsValue; }
            set { _permissionsValue = value; RaisePropertyChanged("PermissionValue"); }
        }
    }
}