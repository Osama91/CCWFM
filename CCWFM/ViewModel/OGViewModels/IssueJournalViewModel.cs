using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblIssueJournalHeaderViewModel : PropertiesViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value; RaisePropertyChanged("VendorPerRow");
                if (value != null) Vendor = value.vendor_code;
            }
        }

        private string _codeField;

        private string _createdByField;

        private DateTime? _creationDateField;

        private DateTime? _dateField;

        private string _vendorField;

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

        public string CreatedBy
        {
            get
            {
                return _createdByField;
            }
            set
            {
                if ((ReferenceEquals(_createdByField, value) != true))
                {
                    _createdByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public DateTime? Date
        {
            get
            {
                return _dateField;
            }
            set
            {
                if ((_dateField.Equals(value) != true))
                {
                    _dateField = value;
                    RaisePropertyChanged("Date");
                }
            }
        }

        public string Vendor
        {
            get
            {
                return _vendorField;
            }
            set
            {
                if ((ReferenceEquals(_vendorField, value) != true))
                {
                    _vendorField = value;
                    RaisePropertyChanged("Vendor");
                }
            }
        }

        private ObservableCollection<TblIssueJournalDetailViewModel> _detailsList;

        public ObservableCollection<TblIssueJournalDetailViewModel> DetailList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblIssueJournalDetailViewModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailList"); }
        }

        private ObservableCollection<TblIssueJournalReceiveHeaderViewModel> _subDetailList;

        public ObservableCollection<TblIssueJournalReceiveHeaderViewModel> SubDetailList
        {
            get { return _subDetailList ?? (_subDetailList = new ObservableCollection<TblIssueJournalReceiveHeaderViewModel>()); }
            set { _subDetailList = value; RaisePropertyChanged("SubDetailList"); }
        }
    }

    public class TblIssueJournalDetailViewModel : PropertiesViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public TblIssueJournalDetailViewModel()
        {
            _client.AccWithConfigAndSizeCompleted += (s, sv) =>
            {
                ItemPerRow.AccConfigList = sv.Result.AccConfigList;
                ItemPerRow.SizesList = new ObservableCollection<string>();
                ItemPerRow.CombinationList = sv.Result.CombinationList;

                if (ItemPerRow.CombinationList != null)
                {
                    var sizes =
                        ItemPerRow.CombinationList.Where(
                            x => ColorPerRow != null && (x.Configuration == ColorPerRow.Code));

                    var distinctsize = sizes.Select(x => x.Size);
                    foreach (var size in distinctsize)
                    {
                        if (!ItemPerRow.SizesList.Contains(size))
                        {
                            ItemPerRow.SizesList.Add(size);
                        }
                    }
                }
            };
        }

        private double _qty;

        public double Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                if ((_qty.Equals(value) != true))
                {
                    _qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        private double _remQty;

        public double RemQty
        {
            get { return _remQty; }
            set { _remQty = value; RaisePropertyChanged("RemQty"); }
        }

        private int? _tblColor;

        public int? TblColor
        {
            get { return _tblColor; }
            set { _tblColor = value; RaisePropertyChanged("TblColor"); }
        }

        private TblColor _colorPerRow;

        public TblColor ColorPerRow
        {
            get { return _colorPerRow; }
            set
            {
                _colorPerRow = value; RaisePropertyChanged("ColorPerRow");
                if (_colorPerRow != null)
                {
                    TblColor = _colorPerRow.Iserial;
                    if (IsAcc)
                    {
                        ItemPerRow.SizesList.Clear();
                        var sizes =
                            ItemPerRow.CombinationList.Where(
                                x => (x.Configuration == ColorPerRow.Code));

                        var distinctsize = sizes.Select(x => x.Size);
                        foreach (var size in distinctsize)
                        {
                            if (!ItemPerRow.SizesList.Contains(size))
                            {
                                ItemPerRow.SizesList.Add(size);
                            }
                        }
                    }
                }
            }
        }

        private ItemsDto _itemPerRow;

        public ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
                if (_itemPerRow != null)
                {
                    ItemCode = ItemPerRow.Iserial;
                    ItemType = ItemPerRow.ItemGroup;
                    IsAcc = ItemPerRow.ItemGroup.StartsWith("Acc");
                    if (IsAcc)
                    {
                        _client.AccWithConfigAndSizeAsync(ItemPerRow);
                    }
                }
            }
        }

        public bool IsAcc { get; set; }

        private string _itemType;

        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; RaisePropertyChanged("ItemType"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _batchNoField;

        private string _configrationField;

        private double _costField;

        private int _itemCodeField;

        private string _locationField;

        private string _siteField;

        private string _sizeField;

        private int? _tblIssueJournalHeaderField;

        public string BatchNo
        {
            get
            {
                return _batchNoField;
            }
            set
            {
                if ((ReferenceEquals(_batchNoField, value) != true))
                {
                    _batchNoField = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }

        public string Configration
        {
            get
            {
                return _configrationField;
            }
            set
            {
                if ((ReferenceEquals(_configrationField, value) != true))
                {
                    _configrationField = value;
                    RaisePropertyChanged("Configration");
                }
            }
        }

        public double Cost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }

        public int ItemCode
        {
            get
            {
                return _itemCodeField;
            }
            set
            {
                _itemCodeField = value;
                RaisePropertyChanged("ItemCode");
            }
        }

        public string Location
        {
            get
            {
                return _locationField;
            }
            set
            {
                if ((ReferenceEquals(_locationField, value) != true))
                {
                    _locationField = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        public string Site
        {
            get
            {
                return _siteField;
            }
            set
            {
                if ((ReferenceEquals(_siteField, value) != true))
                {
                    _siteField = value;
                    RaisePropertyChanged("Site");
                }
            }
        }

        public string Size
        {
            get
            {
                return _sizeField;
            }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true))
                {
                    _sizeField = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        public int? TblIssueJournalHeader
        {
            get
            {
                return _tblIssueJournalHeaderField;
            }
            set
            {
                if ((_tblIssueJournalHeaderField.Equals(value) != true))
                {
                    _tblIssueJournalHeaderField = value;
                    RaisePropertyChanged("TblIssueJournalHeader");
                }
            }
        }
    }

    public class TblIssueJournalReceiveHeaderViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblIssueJournalReceiveDetailViewModel> _detailsList;

        public ObservableCollection<TblIssueJournalReceiveDetailViewModel> DetailList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblIssueJournalReceiveDetailViewModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailList"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _codeField;

        private string _createdByField;

        private DateTime? _creationDateField;

        private DateTime? _dateField;

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

        public string CreatedBy
        {
            get
            {
                return _createdByField;
            }
            set
            {
                if ((ReferenceEquals(_createdByField, value) != true))
                {
                    _createdByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public DateTime? Date
        {
            get
            {
                return _dateField;
            }
            set
            {
                if ((_dateField.Equals(value) != true))
                {
                    _dateField = value;
                    RaisePropertyChanged("Date");
                }
            }
        }

        private int _tblIssueJournalHeader;

        public int TblIssueJournalHeader
        {
            get { return _tblIssueJournalHeader; }
            set { _tblIssueJournalHeader = value; RaisePropertyChanged("TblIssueJournalHeader"); }
        }
    }

    public class TblIssueJournalReceiveDetailViewModel : TblIssueJournalDetailViewModel
    {
        private int _tblIssueJournalDetail;

        public int TblIssueJournalDetail
        {
            get { return _tblIssueJournalDetail; }
            set { _tblIssueJournalDetail = value; RaisePropertyChanged("TblIssueJournalDetail"); }
        }

        private double _costField;

        public double NewCost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("NewCost");
                }
            }
        }

        private double _qty;

        public double NewQty
        {
            get
            {
                return _qty;
            }
            set
            {
                if ((_qty.Equals(value) != true))
                {
                    _qty = value;
                    RaisePropertyChanged("NewQty");
                }
            }
        }

        private string _newLocation;

        public string NewLocation
        {
            get { return _newLocation; }
            set { _newLocation = value; RaisePropertyChanged("NewLocation"); }
        }

        private int _tblIssueJournalHeader;

        public int TblIssueJournalReceiveHeader
        {
            get { return _tblIssueJournalHeader; }
            set { _tblIssueJournalHeader = value; RaisePropertyChanged("TblIssueJournalReceiveHeader"); }
        }
    }

    public class IssueJournalViewModel : ViewModelBase
    {
        public IssueJournalViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.IssueJournalForm.ToString());
                MainRowList = new SortableCollectionView<TblIssueJournalHeaderViewModel>();
                SelectedMainRow = new TblIssueJournalHeaderViewModel();
                SelectedRecRow = new TblIssueJournalReceiveHeaderViewModel();
                Client.GetWarehouseswithOnHandAsync();
                Client.GetWarehouseswithOnHandCompleted += (l, c) =>
                {
                    WarehouseWithOnHandList = c.Result;
                };
                Client.GetTblIssueJournalHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblIssueJournalHeaderViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };
                Client.GetTblIssueJournalReceiveHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblIssueJournalReceiveHeaderViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.SubDetailList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;
                };
                Client.GetTblIssueJournalDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblIssueJournalDetailViewModel();
                        newrow.ColorPerRow = row.TblColor1;
                        newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.ItemCode && x.ItemGroup == row.ItemType);
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                };
                Client.UpdateOrInsertTblIssueJournalHeaderCompleted += (s, x) =>
                {
                    var savedRow = (TblIssueJournalHeaderViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
                Client.DeleteTblIssueJournalHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    Loading = false;
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.UpdateOrInsertTblIssueJournalDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblIssueJournalDetailViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblIssueJournalHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void GetDetaildata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblIssueJournalDetailAsync(SelectedMainRow.DetailList.Count, PageSize, SelectedMainRow.Iserial, SortBy, DetailFilter, DetailValuesObjects);
        }

        public void GetSubDetaildata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblIssueJournalReceiveHeaderAsync(SelectedMainRow.SubDetailList.Count, PageSize, SelectedMainRow.Iserial, SortBy, DetailSubFilter, DetailSubValuesObjects);
        }

        public void GetRecDetailData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblIssueJournalReceiveDetailAsync(0, int.MaxValue, SelectedRecRow.Iserial, SortBy, DetailSubFilter, DetailSubValuesObjects);
        }

        public void SaveRecRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var detailsl = new ObservableCollection<TblIssueJournalReceiveDetail>();
            if (SelectedRecRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedRecRow, new ValidationContext(SelectedRecRow, null, null), valiationCollection, true);

                foreach (var variable in SelectedRecRow.DetailList)
                {
                    var valiationCollectiondetail = new List<ValidationResult>();

                    var isvalidDetails = Validator.TryValidateObject(variable, new ValidationContext(variable, null, null), valiationCollectiondetail, true);

                    if (isvalidDetails)
                    {
                        detailsl.Add(new TblIssueJournalReceiveDetail().InjectFrom(variable) as TblIssueJournalReceiveDetail);
                    }
                    else
                    {
                        return;
                    }
                }

                if (isvalid)
                {
                    var save = SelectedRecRow.Iserial == 0;
                    SelectedRecRow.TblIssueJournalHeader = SelectedMainRow.Iserial;
                    var saveRow = new TblIssueJournalReceiveHeader();
                    saveRow.InjectFrom(SelectedRecRow);
                    saveRow.TblIssueJournalReceiveDetails = detailsl;
                    Loading = true;
                    Client.UpdateOrInsertTblIssueJournalReceiveHeaderAsync(saveRow, save, SelectedMainRow.SubDetailList.IndexOf(SelectedRecRow));
                }
            }
        }

        public void SaveMainRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var detailsl = new ObservableCollection<TblIssueJournalDetail>();
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                foreach (var variable in SelectedMainRow.DetailList)
                {
                    var valiationCollectiondetail = new List<ValidationResult>();

                    var isvalidDetails = Validator.TryValidateObject(variable, new ValidationContext(variable, null, null), valiationCollectiondetail, true);

                    if (isvalidDetails)
                    {
                        variable.RemQty = variable.Qty;
                        detailsl.Add(new TblIssueJournalDetail().InjectFrom(variable) as TblIssueJournalDetail);
                    }
                    else
                    {
                        return;
                    }
                }

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblIssueJournalHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblIssueJournalDetails = detailsl;
                    Loading = true;
                    Client.UpdateOrInsertTblIssueJournalHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailList.IndexOf(SelectedDetailRow));

            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblIssueJournalDetailViewModel();
            SelectedMainRow.DetailList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        public void AddNewRecRow()
        {
            foreach (var variable in SelectedMainRow.DetailList)
            {
                var newrow = new TblIssueJournalReceiveDetailViewModel();

                newrow.InjectFrom(variable);
                newrow.TblIssueJournalDetail = variable.Iserial;
                newrow.Iserial = 0;
                newrow.NewCost = newrow.Cost;
                newrow.NewLocation = newrow.NewLocation;
                newrow.NewQty = newrow.Qty;

                newrow.TblIssueJournalDetail = variable.Iserial;
                SelectedRecRow.DetailList.Add(newrow);
            }
        }

        #region Prop

        private ObservableCollection<WareHouseDto> _warehouseWithOnhandList;

        public ObservableCollection<WareHouseDto> WarehouseWithOnHandList
        {
            get
            {
                return _warehouseWithOnhandList;
            }
            set
            {
                if ((ReferenceEquals(_warehouseWithOnhandList, value) != true))
                {
                    _warehouseWithOnhandList = value;
                    RaisePropertyChanged("WarehouseWithOnHandList");
                }
            }
        }

        private SortableCollectionView<TblIssueJournalHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblIssueJournalHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }
     
        private TblIssueJournalHeaderViewModel _selectedMainRow;

        public TblIssueJournalHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblIssueJournalReceiveHeaderViewModel _selectedRecRow;

        public TblIssueJournalReceiveHeaderViewModel SelectedRecRow
        {
            get { return _selectedRecRow; }
            set { _selectedRecRow = value; RaisePropertyChanged("SelectedRecRow"); }
        }

        private TblIssueJournalDetailViewModel _selectedDetailRow;

        public TblIssueJournalDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop
    }
}