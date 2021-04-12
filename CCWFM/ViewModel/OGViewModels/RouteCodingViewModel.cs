using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblRouteGroupViewModel : GenericViewModel
    {
        public CRUD_ManagerServiceClient Client = new CRUD_ManagerServiceClient();

        public TblRouteGroupViewModel()
        {
            DetailsList = new SortableCollectionView<TblRouteViewModel>();
            DetailsList.CollectionChanged += DetailsList_CollectionChanged;

            Client.GetAxRoutListeCompleted += (s, sv) =>
            {
                foreach (var detailRow in DetailsList)
                {
                    foreach (var row in sv.Result.Where(x => x.WRKCTRGROUPID.ToString(CultureInfo.InvariantCulture) == AXRoutLinkCode.ToString(CultureInfo.InvariantCulture)))
                    {
                        if (detailRow.AxRoutesList.All(x => x.WRKCTRID != row.WRKCTRID))
                        {
                            detailRow.AxRoutesList.Add(row);
                        }
                    }
                }
            };
        }

        private void DetailsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblRouteViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblRouteViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "VendorSerial")
            {
                foreach (var vendor in DetailsList.Select(x => x.VendorSerial).Distinct())
                {
                    Client.GetAxRoutListeAsync("");
                }
            }
        }

        private SortableCollectionView<TblRouteViewModel> _tblSubProductGroupField;

        public SortableCollectionView<TblRouteViewModel> DetailsList
        {
            get
            {
                return _tblSubProductGroupField ?? (_tblSubProductGroupField = new SortableCollectionView<TblRouteViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblSubProductGroupField, value) != true))
                {
                    _tblSubProductGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        private WRKCTRTABLE _routeGroupPerRow;

        public WRKCTRTABLE RouteGroupPerRow
        {
            get { return _routeGroupPerRow; }
            set { _routeGroupPerRow = value; RaisePropertyChanged("RouteGroupPerRow"); }
        }

        private int? _tblService;

        public int? TblService
        {
            get { return _tblService; }
            set { _tblService = value; RaisePropertyChanged("TblService"); }
        }

        private string _axRoutLinkCodeField;

        public string AXRoutLinkCode
        {
            get
            {
                return _axRoutLinkCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axRoutLinkCodeField, value) != true))
                {
                    _axRoutLinkCodeField = value;
                    RaisePropertyChanged("AXRoutLinkCode");
                }
            }
        }

        private TblService _servicePerRow;

        public TblService ServicePerRow
        {
            get { return _servicePerRow; }
            set { _servicePerRow = value; RaisePropertyChanged("ServicePerRow"); }
        }
    }

    public class TblRouteViewModel : GenericViewModel
    {
        private TblService _servicePerRow;

        public TblService ServicePerRow
        {
            get { return _servicePerRow; }
            set { _servicePerRow = value; RaisePropertyChanged("ServicePerRow"); }
        }

        private ObservableCollection<WRKCTRTABLE> _axRouteList;

        public ObservableCollection<WRKCTRTABLE> AxRoutesList
        {
            get { return _axRouteList ?? (_axRouteList = new ObservableCollection<WRKCTRTABLE>()); }
            set { _axRouteList = value; RaisePropertyChanged("AxRoutesList"); }
        }

        private int _tblRouteGroupField;

        public int TblRouteGroup
        {
            get
            {
                return _tblRouteGroupField;
            }
            set
            {
                if ((_tblRouteGroupField.Equals(value) != true))
                {
                    _tblRouteGroupField = value;
                    RaisePropertyChanged("TblRouteGroup");
                }
            }
        }

        private string _axRoutLinkCodeField;

        private int? _routeCapacityField;

        private int? _timePerCapacity;

        private string _timeUnitField;

        private string _vendorSerialField;

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value; RaisePropertyChanged("VendorPerRow");
                if (value != null) VendorSerial = value.vendor_code;
            }
        }

        private int? _tblService;

        public int? TblService
        {
            get { return _tblService; }
            set { _tblService = value; RaisePropertyChanged("TblService"); }
        }

        private WRKCTRTABLE _routePerRow;

        public WRKCTRTABLE RoutePerRow
        {
            get { return _routePerRow; }
            set { _routePerRow = value; RaisePropertyChanged("RoutePerRow"); }
        }

        public string AXRoutLinkCode
        {
            get
            {
                return _axRoutLinkCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axRoutLinkCodeField, value) != true))
                {
                    _axRoutLinkCodeField = value;
                    RaisePropertyChanged("AXRoutLinkCode");
                }
            }
        }

        public int? RouteCapacity
        {
            get
            {
                return _routeCapacityField;
            }
            set
            {
                if ((_routeCapacityField.Equals(value) != true))
                {
                    _routeCapacityField = value;
                    RaisePropertyChanged("RouteCapacity");
                }
            }
        }

        public int? TimePerCapacity
        {
            get
            {
                return _timePerCapacity;
            }
            set
            {
                if ((_timePerCapacity.Equals(value) != true))
                {
                    _timePerCapacity = value;
                    RaisePropertyChanged("TimePerCapacity");
                }
            }
        }

        public string TimeUnit
        {
            get
            {
                return _timeUnitField;
            }
            set
            {
                if ((ReferenceEquals(_timeUnitField, value) != true))
                {
                    _timeUnitField = value;
                    RaisePropertyChanged("TimeUnit");
                }
            }
        }

        public string VendorSerial
        {
            get
            {
                return _vendorSerialField;
            }
            set
            {
                if ((ReferenceEquals(_vendorSerialField, value) != true))
                {
                    _vendorSerialField = value;
                    RaisePropertyChanged("VendorSerial");
                }
            }
        }
    }

    #endregion ViewModels

    public class RouteCodingViewModel : ViewModelBase
    {
        public RouteCodingViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetAxRouteGroupAsync();
                Client.GetAxRouteGroupCompleted += (s, sv) =>
                {
                    AxRouteGroupList = sv.Result;
                };

                MainRowList = new SortableCollectionView<TblRouteGroupViewModel>();
                SelectedMainRow = new TblRouteGroupViewModel();
                SelectedDetailRow = new TblRouteViewModel();
                
                SelectedMainRow.DetailsList.OnRefresh += DetailsList_OnRefresh;

                Client.GetTblServiceCompleted += (s, sv) =>
                {
                    ServiceList = sv.Result;
                };

                Client.GetTblServiceAsync(0, int.MaxValue, "it.Iserial", null, null);
                Client.GetTblRouteGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRouteGroupViewModel();
                        newrow.InjectFrom(row);

                        newrow.ServicePerRow = row.TblService1;

                        newrow.RouteGroupPerRow = new WRKCTRTABLE
                        {
                            NAME = newrow.AXRoutLinkCode,
                            WRKCTRGROUPID = newrow.AXRoutLinkCode,
                        };

                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblRouteCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRouteViewModel();
                        newrow.InjectFrom(row);
                        newrow.VendorPerRow = new Vendor
                        {
                            vendor_code = row.VendorSerial,
                            vendor_ename = row.VendorSerial
                        };
                        newrow.RoutePerRow = new WRKCTRTABLE
                        {
                            NAME = newrow.AXRoutLinkCode,
                            WRKCTRGROUPID = newrow.AXRoutLinkCode,
                        };

                        if (newrow.AxRoutesList.All(x => x.WRKCTRGROUPID != newrow.RoutePerRow.WRKCTRGROUPID))
                        {
                            newrow.AxRoutesList.Add(newrow.RoutePerRow);
                        }
                        newrow.ServicePerRow = row.TblService1;
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };
                Client.UpdateOrInsertTblRouteGroupCompleted += (s, x) =>
                {
                    var savedRow = (TblRouteGroupViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblRouteCompleted += (s, x) =>
                {
                    var savedRow = (TblRouteViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    //if (x.Result.TblSalaryRelation1 != null)
                    //{
                    //    var headerIserial = x.Result.TblSalaryRelation;

                    //    SelectedMainRow.Iserial = headerIserial;
                    //}
                };

                Client.DeleteTblRouteGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblRouteCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Client.GetTblRouteGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblRouteGroupAsync(
                                (TblRouteGroup)new TblRouteGroup().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblRouteGroupViewModel());
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
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblRouteGroup();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblRouteGroupAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            if (SelectedMainRow != null)
                Client.GetTblRouteAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        private void DetailsList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                SelectedMainRow.DetailsList.Clear();
                DetailSortBy = null;
                foreach (var sortDesc in SelectedMainRow.DetailsList.SortDescriptions)
                {
                    DetailSortBy = DetailSortBy + "it." + sortDesc.PropertyName + (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                if (SelectedMainRow != null)
                {
                    GetDetailData();
                }
            }
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblRouteAsync((TblRoute)new TblRoute().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblRouteViewModel
                        {
                            TblRouteGroup = SelectedMainRow.Iserial
                        });
                    }
                }
                else
                {
                    SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblRouteViewModel
                    {
                        TblRouteGroup = SelectedMainRow.Iserial
                    });
                }
            }
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    var rowToSave = new TblRoute();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblRouteAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        #region Prop

        private ObservableCollection<WRKCTRTABLE> _axRouteGroupList;

        public ObservableCollection<WRKCTRTABLE> AxRouteGroupList
        {
            get { return _axRouteGroupList; }
            set { _axRouteGroupList = value; RaisePropertyChanged("AxRouteGroupList"); }
        }

        private ObservableCollection<TblService> _serviceList;

        public ObservableCollection<TblService> ServiceList
        {
            get { return _serviceList; }
            set { _serviceList = value; RaisePropertyChanged("ServiceList"); }
        }

        private SortableCollectionView<TblRouteGroupViewModel> _mainRowList;

        public SortableCollectionView<TblRouteGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblRouteGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblRouteGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblRouteGroupViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblRouteGroupViewModel _selectedMainRow;

        public TblRouteGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblRouteViewModel _selectedDetailRow;

        public TblRouteViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblRouteViewModel> _selectedDetailRows;

        public ObservableCollection<TblRouteViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblRouteViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}