using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public class TblGlobalRetailBusinessBudgetViewModel : PropertiesViewModelBase
    {
        public TblGlobalRetailBusinessBudgetViewModel()
        {
            var names = new[]
                {
                    "Jan", "Feb", "Mar",
                    "Apr", "May", "Jun",
                    "Jul", "Aug", "Sep",
                    "Oct", "Nov", "Dec"
                };
            MonthTotallist = new ObservableCollection<GlobalRetailBusinessBudgetViewModel.MonthTotal>();
            for (int i = 1; i < names.Count() + 1; i++)
            {
                MonthTotallist.Add(new GlobalRetailBusinessBudgetViewModel.MonthTotal
                {
                    Month = i,
                    Amount = 0
                });
            }
            DetailsList = new SortableCollectionView<TblGlobalRetailBusinessBudgetMainDetailViewModel>();

            DetailsList.CollectionChanged += DetailsList_CollectionChanged;
            DetailsList.Add(new TblGlobalRetailBusinessBudgetMainDetailViewModel());
            AddTotalRow();

            TransDate = DateTime.Now;
            Year = DateTime.Now.Year;
        }

        public void AddTotalRow()
        {
            DetailsList.Add(new TblGlobalRetailBusinessBudgetMainDetailViewModel { Iserial = -1 });
        }

        private ObservableCollection<GlobalRetailBusinessBudgetViewModel.MonthTotal> _monthTotallistTotal;

        public ObservableCollection<GlobalRetailBusinessBudgetViewModel.MonthTotal> MonthTotallist
        {
            get { return _monthTotallistTotal; }
            set { _monthTotallistTotal = value; RaisePropertyChanged("MonthTotallist"); }
        }

        private void DetailsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblGlobalRetailBusinessBudgetMainDetailViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblGlobalRetailBusinessBudgetMainDetailViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "TotalAmount")
            {
                var row = DetailsList.FirstOrDefault(x => x.Iserial == -1).TotalAmount = DetailsList.Where(x => x.Iserial != -1).Sum(x => x.TotalAmount);
            }
        }

        private DateTime? _transDate;

        public DateTime? TransDate
        {
            get { return _transDate; }
            set
            {
                _transDate = value;
                RaisePropertyChanged("TransDate");
            }
        }

        private SortableCollectionView<TblGlobalRetailBusinessBudgetMainDetailViewModel> _detailslist;

        public SortableCollectionView<TblGlobalRetailBusinessBudgetMainDetailViewModel> DetailsList
        {
            get { return _detailslist; }
            set
            {
                _detailslist = value;

                RaisePropertyChanged("DetailsList");
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                RaisePropertyChanged("Enabled");
            }
        }

        private int _iserialField;

        private int? _year;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public int? Year
        {
            get { return _year; }
            set
            {
                _year = value;
                RaisePropertyChanged("Year");
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int TransactionType { get; set; }
    }

    public class TblGlobalRetailBusinessBudgetMainDetailViewModel : PropertiesViewModelBase
    {
        private LkpData.LkpDataClient Client = new LkpData.LkpDataClient();
        public TblGlobalRetailBusinessBudgetMainDetailViewModel()
        {
            Client.GetTblBrandSectionLinkCompleted += (s, ev) =>
            {
                foreach (var row in ev.Result)
                {
                    if (Brandsection.All(x => x.Iserial != row.TblLkpBrandSection))
                    {
                        Brandsection.Add(row.TblLkpBrandSection1);
                    }
                }
            };
            var monthList = new Dictionary<int, string>();
            var names = new[]
                {
                    "Jan", "Feb", "Mar",
                    "Apr", "May", "Jun",
                    "Jul", "Aug", "Sep",
                    "Oct", "Nov", "Dec"
                };

            for (int i = 1; i < names.Count() + 1; i++)
            {
                monthList.Add(i, names[i - 1]);
            }

            DetailsList = new SortableCollectionView<TblGlobalRetailBusinessBudgetDetailViewModel>();
            foreach (var variable in monthList)
            {
                DetailsList.Add(new TblGlobalRetailBusinessBudgetDetailViewModel
                {
                    Month = variable.Key,
                    TblGlobalRetailBusinessBudgetMainDetail = Iserial,
                });
            }
        }

        private double _totalAmountField;

        public double TotalAmount
        {
            get { return _totalAmountField; }
            set
            {
                if ((_totalAmountField.Equals(value) != true))
                {
                    _totalAmountField = value;
                    RaisePropertyChanged("TotalAmount");
                    //foreach (var item in MonthTotallist)
                    //{
                    //    var Total = 0;
                    //    foreach (var detail in DetailsList)
                    //    {
                    //        Total = Total + detail.DetailsList.Where(x => x.Month == item.Month).Sum(x => x.Month);
                    //    }

                    //    item.Amount = Total;
                    //}
                }
            }
        }

        private SortableCollectionView<TblGlobalRetailBusinessBudgetDetailViewModel> _detailslist;

        public SortableCollectionView<TblGlobalRetailBusinessBudgetDetailViewModel> DetailsList
        {
            get { return _detailslist; }
            set
            {
                _detailslist = value;

                RaisePropertyChanged("DetailsList");
            }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandsection;

        public ObservableCollection<LkpData.TblLkpBrandSection> Brandsection
        {
            get { return _brandsection ?? (_brandsection = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set
            {
                if ((ReferenceEquals(_brandsection, value) != true))
                {
                    _brandsection = value;
                    RaisePropertyChanged("Brandsection");
                }
            }
        }

        private TblBudgetItem _budgetItemPerRow;

        public TblBudgetItem BudgetItemPerRow
        {
            get { return _budgetItemPerRow; }
            set
            {
                _budgetItemPerRow = value;
                RaisePropertyChanged("BudgetItemPerRow");
            }
        }

        private int? _tblBudgetItem;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBudgetItem")]
        public int? TblBudgetItem
        {
            get { return _tblBudgetItem; }
            set
            {
                _tblBudgetItem = value;
                RaisePropertyChanged("TblBudgetItem");
            }
        }

        private string _brandField;

        private int? _tblLkpBrandSectionField;

        private int? _tblLkpSeasonField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public string Brand
        {
            get { return _brandField; }
            set
            {
                if ((ReferenceEquals(_brandField, value) != true))
                {
                    _brandField = value;
                    RaisePropertyChanged("Brand");
                    Brandsection.Clear();
                    Client.GetTblBrandSectionLinkAsync(Brand, LoggedUserInfo.Iserial);
                }
            }
        }

        private TblLkpBrandSection _brandsectionPerRow;

        public TblLkpBrandSection BrandSectionPerRow
        {
            get { return _brandsectionPerRow; }
            set
            {
                _brandsectionPerRow = value;
                RaisePropertyChanged("BrandSectionPerRow");
            }
        }

        private TblLkpSeason _seasonPerRow;

        public TblLkpSeason SeasonPerRow
        {
            get { return _seasonPerRow; }
            set
            {
                _seasonPerRow = value;
                RaisePropertyChanged("SeasonPerRow");
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrandSection")]
        public int? TblLkpBrandSection
        {
            get { return _tblLkpBrandSectionField; }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSeason")]
        public int? TblLkpSeason
        {
            get { return _tblLkpSeasonField; }
            set
            {
                if ((_tblLkpSeasonField.Equals(value) != true))
                {
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        private int _iserialField;

        private int _tblGlobalRetailBusinessBudget;

        private double _amountField;

        public double Amount
        {
            get { return _amountField; }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int TblGlobalRetailBusinessBudget
        {
            get { return _tblGlobalRetailBusinessBudget; }
            set
            {
                if ((_tblGlobalRetailBusinessBudget.Equals(value) != true))
                {
                    _tblGlobalRetailBusinessBudget = value;
                    RaisePropertyChanged("TblGlobalRetailBusinessBudget");
                }
            }
        }
    }

    public class TblGlobalRetailBusinessBudgetDetailViewModel : PropertiesViewModelBase
    {
        private double _amountField;

        private int _iserialField;

        private int _monthField;

        private int _tblGlobalRetailBusinessBudgetMainDetailField;

        public double Amount
        {
            get { return _amountField; }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int Month
        {
            get { return _monthField; }
            set
            {
                if ((_monthField.Equals(value) != true))
                {
                    _monthField = value;
                    RaisePropertyChanged("Month");
                }
            }
        }

        public int TblGlobalRetailBusinessBudgetMainDetail
        {
            get { return _tblGlobalRetailBusinessBudgetMainDetailField; }
            set
            {
                if ((_tblGlobalRetailBusinessBudgetMainDetailField.Equals(value) != true))
                {
                    _tblGlobalRetailBusinessBudgetMainDetailField = value;
                    RaisePropertyChanged("TblGlobalRetailBusinessBudgetMainDetail");
                }
            }
        }
    }

    public class GlobalRetailBusinessBudgetViewModel : ViewModelBase
    {
        public int TransactionType = 0;

        public GlobalRetailBusinessBudgetViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetTblBudgetItemAsync();
                Client.GetTblBudgetItemCompleted += (s, sv) =>
                {
                    BrandBudgetList = sv.Result;
                };
                YearList = new List<int>();
                MonthList = new Dictionary<int, string>();
                for (var i = 0; i < 10; i++)
                {
                    const int yearNumber = 2014;
                    YearList.Add(yearNumber + i);
                }
                var names = new[]
                {
                    "Jan", "Feb", "Mar",
                    "Apr", "May", "Jun",
                    "Jul", "Aug", "Sep",
                    "Oct", "Nov", "Dec"
                };
                MonthTotallist = new ObservableCollection<MonthTotal>();
                for (int i = 1; i < names.Count() + 1; i++)
                {
                    MonthList.Add(i, names[i - 1]);
                    MonthTotallist.Add(new MonthTotal
                    {
                        Month = i,
                        Amount = 0
                    });
                }
                TransactionHeader = new TblGlobalRetailBusinessBudgetViewModel();
                GetItemPermissions(PermissionItemName.GlobalRetailBusinessBudget.ToString());

                MainRowList = new SortableCollectionView<TblGlobalRetailBusinessBudgetViewModel>();
                SelectedDetailRows = new ObservableCollection<TblGlobalRetailBusinessBudgetMainDetailViewModel>();
                SelectedDetailRow = new TblGlobalRetailBusinessBudgetMainDetailViewModel();

                Client.GetAllBrandsCompleted += (s, ev) =>
                {
                    Brands = ev.Result;
                };

                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                Client.GetAllSeasonsCompleted += (s, ev) =>
                {
                    Seasons = ev.Result;
                };
                Client.GetAllSeasonsAsync();

                Client.DeleteTblGlobalRetailBusinessBudgetMainDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) TransactionHeader.DetailsList.Remove(oldrow);
                };

                Client.UpdateOrInsertTblGlobalRetailBusinessBudgetMainDetailCompleted += (x, y) =>
                {
                    var savedRow =
                        (TblGlobalRetailBusinessBudgetMainDetailViewModel)
                            TransactionHeader.DetailsList.GetItemAt(y.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(y.Result);
                        if (TransactionHeader.Iserial == 0)
                        {
                            TransactionHeader.Iserial = y.Result.TblGlobalRetailBusinessBudget;
                        }
                        foreach (var variable in savedRow.DetailsList)
                        {
                            var newRow = y.Result.TblGlobalRetailBusinessBudgetDetails.FirstOrDefault(w => w.Month == variable.Month);
                            if (newRow != null)
                            {
                                variable.Iserial = newRow.Iserial;
                                variable.Month = newRow.Month;
                                variable.TblGlobalRetailBusinessBudgetMainDetail =
                                    newRow.TblGlobalRetailBusinessBudgetMainDetail;
                            }
                        }
                    }
                };

                Client.GetTblGlobalRetailBusinessBudgetMainDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlobalRetailBusinessBudgetMainDetailViewModel();
                        newrow.InjectFrom(row);
                        newrow.BrandSectionPerRow = row.TblLkpBrandSection1;
                        newrow.SeasonPerRow = row.TblLkpSeason1;
                        newrow.BudgetItemPerRow = row.TblBudgetItem1;
                        foreach (var variable in row.TblGlobalRetailBusinessBudgetDetails)
                        {
                            var oldRow = newrow.DetailsList.FirstOrDefault(w => w.Month == variable.Month);
                            if (oldRow != null)
                            {
                                oldRow.Iserial = variable.Iserial;
                                oldRow.Month = variable.Month;
                                oldRow.Amount = variable.Amount;
                                oldRow.TblGlobalRetailBusinessBudgetMainDetail = variable.TblGlobalRetailBusinessBudgetMainDetail;
                            }
                        }
                        newrow.TotalAmount = newrow.DetailsList.Sum(x => x.Amount);
                        TransactionHeader.DetailsList.Add(newrow);
                    }
                    TransactionHeader.AddTotalRow();
                    foreach (var row in TransactionHeader.MonthTotallist)
                    {
                        var total = 0;
                        foreach (var variable in TransactionHeader.DetailsList.Where(x => x.Iserial != 1))
                        {
                            total = (int)(total + variable.DetailsList.FirstOrDefault(x => x.Month == row.Month).Amount);
                        }

                        TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == -1)
                            .DetailsList.FirstOrDefault(x => x.Month == row.Month)
                            .Amount = total;
                    }

                    TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == -1).TotalAmount =
                        TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.TotalAmount);
                };

                Client.GetTblGlobalRetailBusinessBudgetCompleted += (y, v) =>
                {
                    foreach (var row in v.Result)
                    {
                        Loading = false;
                        var newrow = new TblGlobalRetailBusinessBudgetViewModel();
                        newrow.InjectFrom(row);

                        MainRowList.Add(newrow);
                    }
                    FullCount = v.fullCount;
                };

                Client.UpdateOrInsertTblGlobalRetailBusinessBudgetCompleted += (s, x) =>
                {
                    if (x.Error == null)
                    {
                        TransactionHeader.InjectFrom(x.Result);
                    }
                    else
                    {
                        MessageBox.Show(x.Error.Message);
                    }
                    Loading = false;

                    //TransactionHeader.InjectFrom(x.Result);
                };
                Client.DeleteTblGlobalRetailBusinessBudgetCompleted += (w, k) =>
                {
                    Loading = false;
                    TransactionHeader.InjectFrom(new TblGlobalRetailBusinessBudgetViewModel());
                };
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (TransactionHeader.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = SelectedDetailRow != null &&
                              Validator.TryValidateObject(SelectedDetailRow,
                                  new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);
                if (!isvalid)
                {
                    return;
                }

                if (
                    TransactionHeader.DetailsList.Count(
                        x =>
                            x.Brand == SelectedDetailRow.Brand &&
                            x.TblLkpBrandSection == SelectedDetailRow.TblLkpBrandSection &&
                            x.TblLkpSeason == SelectedDetailRow.TblLkpSeason &&
                            SelectedDetailRow.TblBudgetItem == x.TblBudgetItem) > 1)
                {
                    MessageBox.Show("Cannot Duplicate The Full Criteria");
                    SelectedDetailRow.BrandSectionPerRow = null;
                    SelectedDetailRow.SeasonPerRow = null;
                    SelectedDetailRow.BudgetItemPerRow = null;

                    isvalid = false;
                }

                if (!isvalid)
                {
                    return;
                }
                if (AllowAdd != true)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }
            }
            var newrow = new TblGlobalRetailBusinessBudgetMainDetailViewModel
            {
                TblGlobalRetailBusinessBudget = TransactionHeader.Iserial
            };
            TransactionHeader.DetailsList.Insert(currentRowIndex + 1, newrow);

            SelectedDetailRow = newrow;
        }

        public void GetMaindata()
        {
            Loading = true;
            if (SortBy == null)
            {
                SortBy = "it.Iserial";
            }
            Client.GetTblGlobalRetailBusinessBudgetAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, TransactionType);
        }

        internal void GetDetailData()
        {
            SubmitSearchAction.Invoke(this, null);
            Loading = true;
            if (DetailSortBy == null)
            {
                DetailSortBy = "it.Iserial";
            }
            Client.GetTblGlobalRetailBusinessBudgetMainDetailAsync(
                TransactionHeader.DetailsList.Count(x => x.Iserial != 0), PageSize, TransactionHeader.Iserial,
                DetailSortBy,
                DetailFilter, DetailValuesObjects);
        }

        public void SaveMainRow()
        {
            var valiationCollection = new List<ValidationResult>();
            bool isvalid = Validator.TryValidateObject(TransactionHeader,
                new ValidationContext(TransactionHeader, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblGlobalRetailBusinessBudget();
                data.InjectFrom(TransactionHeader);
                bool save = TransactionHeader.Iserial == 0;
                data.TransactionType = TransactionType;
                if (save)
                {
                    if (AllowAdd)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblGlobalRetailBusinessBudgetAsync(data, LoggedUserInfo.Iserial);
                    }

                    else
                    {
                        MessageBox.Show("You are Not Allowed to Add");
                    }
                }
                else
                {
                    if (AllowUpdate)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblGlobalRetailBusinessBudgetAsync(data, LoggedUserInfo.Iserial);
                    }
                    else
                    {
                        MessageBox.Show("You are Not Allowed to Update");
                    }
                }
            }
        }

        public void SaveDetailRow()
        {
            foreach (var row in TransactionHeader.DetailsList.Where(x => x.Iserial != -1))
            {
                var valiationCollection = new List<ValidationResult>();
                bool isvalid = Validator.TryValidateObject(row,
                    new ValidationContext(row, null, null), valiationCollection, true);
                var valiationCollectionHeader = new List<ValidationResult>();
                bool isvalidHeader = Validator.TryValidateObject(TransactionHeader,
                    new ValidationContext(TransactionHeader, null, null), valiationCollectionHeader, true);
                TransactionHeader.TransactionType = TransactionType;
                if (
                    TransactionHeader.DetailsList.Count(
                        x =>
                            x.Brand == row.Brand &&
                            x.TblLkpBrandSection == row.TblLkpBrandSection &&
                            x.TblLkpSeason == row.TblLkpSeason &&
                            row.TblBudgetItem == x.TblBudgetItem) > 1)
                {
                    MessageBox.Show("Cannot Duplicate The Full Criteria");
                    row.BrandSectionPerRow = null;
                    row.SeasonPerRow = null;
                    row.BudgetItemPerRow = null;

                    isvalid = false;
                }

                if (isvalid && isvalidHeader)
                {
                    var data = new TblGlobalRetailBusinessBudgetMainDetail();
                    data.InjectFrom(row);
                    row.TblGlobalRetailBusinessBudget = TransactionHeader.Iserial;
                    if (row.TblGlobalRetailBusinessBudget == 0)
                    {
                        data.TblGlobalRetailBusinessBudget1 =
                            (TblGlobalRetailBusinessBudget)
                                new TblGlobalRetailBusinessBudget().InjectFrom(TransactionHeader);
                    }
                    data.TblGlobalRetailBusinessBudgetDetails = new ObservableCollection<TblGlobalRetailBusinessBudgetDetail>();

                    GenericMapper.InjectFromObCollection(data.TblGlobalRetailBusinessBudgetDetails,
                        row.DetailsList);

                    bool save = row.Iserial == 0;
                    if (save)
                    {
                        if (AllowAdd)
                        {
                            Loading = true;
                            Client.UpdateOrInsertTblGlobalRetailBusinessBudgetMainDetailAsync(data, true,
                                TransactionHeader.DetailsList.IndexOf(row), LoggedUserInfo.Iserial);
                        }

                        else
                        {
                            MessageBox.Show("You are Not Allowed to Add");
                        }
                    }
                    else
                    {
                        if (AllowUpdate)
                        {
                            Loading = true;
                            Client.UpdateOrInsertTblGlobalRetailBusinessBudgetMainDetailAsync(data, false,
                                TransactionHeader.DetailsList.IndexOf(row), LoggedUserInfo.Iserial);
                        }
                        else
                        {
                            MessageBox.Show("You are Not Allowed to Update");
                        }
                    }
                }
            }
        }

        public void DeleteMainRow()
        {
            if (TransactionHeader != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    if (TransactionHeader.Iserial != 0)
                    {
                        if (AllowDelete)
                        {
                            Loading = true;
                            Client.DeleteTblGlobalRetailBusinessBudgetAsync(
                                new TblGlobalRetailBusinessBudget().InjectFrom(TransactionHeader) as
                                    TblGlobalRetailBusinessBudget);
                        }
                        else
                        {
                            MessageBox.Show("You are Not Allowed to Delete");
                        }
                    }
                    else
                    {
                        TransactionHeader.InjectFrom(new TblGlobalRetailBusinessBudgetViewModel());
                        TransactionHeader.DetailsList.Clear();
                    }
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
                    Loading = true;
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete)
                            {
                                Loading = true;
                                Client.DeleteTblGlobalRetailBusinessBudgetMainDetailAsync(
                                    (TblGlobalRetailBusinessBudgetMainDetail)
                                        new TblGlobalRetailBusinessBudgetMainDetail().InjectFrom(row),
                                    TransactionHeader.DetailsList.IndexOf(row));
                            }
                            else
                            {
                                MessageBox.Show("You are Not Allowed to Delete");
                            }
                        }
                        else
                        {
                            TransactionHeader.DetailsList.Remove(row);
                            if (!TransactionHeader.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<TblGlobalRetailBusinessBudgetMainDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblGlobalRetailBusinessBudgetMainDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows; }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private ObservableCollection<TblBudgetItem> _brandBudgtList;

        public ObservableCollection<TblBudgetItem> BrandBudgetList
        {
            get { return _brandBudgtList; }
            set { _brandBudgtList = value; RaisePropertyChanged("BrandBudgetList"); }
        }

        private TblGlobalRetailBusinessBudgetMainDetailViewModel _selectedDetailRow;

        public TblGlobalRetailBusinessBudgetMainDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private List<int> _yearList;

        public List<int> YearList
        {
            get { return _yearList; }
            set { _yearList = value; RaisePropertyChanged("YearList"); }
        }

        private Dictionary<int, string> _monthList;

        public Dictionary<int, string> MonthList
        {
            get { return _monthList; }
            set { _monthList = value; RaisePropertyChanged("MonthList"); }
        }

        private ObservableCollection<MonthTotal> _monthTotal;

        public ObservableCollection<MonthTotal> MonthTotallist
        {
            get { return _monthTotal; }
            set { _monthTotal = value; RaisePropertyChanged("MonthTotallist"); }
        }

        public class MonthTotal : PropertiesViewModelBase
        {
            private int _month;

            public int Month
            {
                get { return _month; }
                set { _month = value; RaisePropertyChanged("Month"); }
            }

            private int _amount;

            public int Amount
            {
                get { return _amount; }
                set { _amount = value; RaisePropertyChanged("Amount"); }
            }
        }

        private SortableCollectionView<TblGlobalRetailBusinessBudgetViewModel> _mainRowList;

        public SortableCollectionView<TblGlobalRetailBusinessBudgetViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<Brand> _brands;

        public ObservableCollection<Brand> Brands
        {
            get { return _brands; }
            set
            {
                if ((ReferenceEquals(_brands, value) != true))
                {
                    _brands = value;
                    RaisePropertyChanged("Brands");
                }
            }
        }

        private ObservableCollection<TblLkpSeason> _seasons;

        public ObservableCollection<TblLkpSeason> Seasons
        {
            get { return _seasons; }
            set
            {
                if ((ReferenceEquals(_seasons, value) != true))
                {
                    _seasons = value;
                    RaisePropertyChanged("Seasons");
                }
            }
        }

        private TblGlobalRetailBusinessBudgetViewModel _transactionHeader;

        public TblGlobalRetailBusinessBudgetViewModel TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                _transactionHeader = value;
                RaisePropertyChanged("TransactionHeader");
            }
        }

        public event EventHandler SubmitSearchAction;
    }
}