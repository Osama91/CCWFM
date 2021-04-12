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
    public class TblBrandBudgetsHeaderViewModel : PropertiesViewModelBase
    {
        public TblBrandBudgetsHeaderViewModel()
        {
            DetailsList = new SortableCollectionView<TblBrandBudgetDetailViewModel>
            {
                new TblBrandBudgetDetailViewModel()
            };
            AddTotalRow();
            TransDate = DateTime.Now;
        }

        public void AddTotalRow()
        {
            DetailsList.Add(new TblBrandBudgetDetailViewModel { Iserial = -1 });
        }

        private DateTime? _transDate;

        public DateTime? TransDate
        {
            get { return _transDate; }
            set { _transDate = value; RaisePropertyChanged("TransDate"); }
        }

        private DateTime? _lastUpdatedDate;

        public DateTime? LastUpdatedDate
        {
            get { return _lastUpdatedDate; }
            set { _lastUpdatedDate = value; RaisePropertyChanged("LastUpdatedDate"); }
        }

        private int? _createdBy;

        public int? CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; RaisePropertyChanged("CreatedBy"); }
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

        private int? _lastUpdatedBy;

        public int? LastUpdatedBy
        {
            get { return _lastUpdatedBy; }
            set { _lastUpdatedBy = value; RaisePropertyChanged("LastUpdatedBy"); }
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

        private SortableCollectionView<TblBrandBudgetDetailViewModel> _detailslist;

        public SortableCollectionView<TblBrandBudgetDetailViewModel> DetailsList
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

        private double _amountField;

        private string _brandField;

        private int _iserialField; 

        private int? _tblLkpBrandSectionField;

        private int? _tblLkpDirectionField;


        private int? _tblLkpSeasonField;

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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDirection")]
        public int? TblLkpDirection
        {
            get { return _tblLkpDirectionField; }
            set
            {
                if ((_tblLkpDirectionField.Equals(value) != true))
                {
                    _tblLkpDirectionField = value;
                    RaisePropertyChanged("TblLkpDirection");
                }
            }
        }

        private ObservableCollection<TblFamily> _familyList;

        public ObservableCollection<TblFamily> FamilyList
        {
            get { return _familyList ?? (_familyList = new ObservableCollection<TblFamily>()); }
            set
            {
                _familyList = value;
                RaisePropertyChanged("FamilyList");
            }
        }

        private ObservableCollection<TblLkpDirection> _directionList;

        public ObservableCollection<TblLkpDirection> DirectionList
        {
            get { return _directionList ?? (_directionList = new ObservableCollection<TblLkpDirection>()); }
            set
            {
                _directionList = value;
                RaisePropertyChanged("DirectionList");
            }
        }

        private ObservableCollection<TblStyleCategory> _styleCategoryList;

        public ObservableCollection<TblStyleCategory> StyleCategoryList
        {
            get { return _styleCategoryList ?? (_styleCategoryList = new ObservableCollection<TblStyleCategory>()); }
            set
            {
                _styleCategoryList = value;
                RaisePropertyChanged("StyleCategoryList");
            }
        }


        private ObservableCollection<TblSalesOrderColorTheme> _themeList;

        public ObservableCollection<TblSalesOrderColorTheme> ThemeList
        {
            get { return _themeList ?? (_themeList = new ObservableCollection<TblSalesOrderColorTheme>()); }
            set
            {
                _themeList = value;
                RaisePropertyChanged("ThemeList");
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

        public int TransactionType { get; set; }
    }

    public class TblBrandBudgetDetailViewModel : PropertiesViewModelBase
    {
        private TblFamily _familyPerRow;

        public TblFamily FamilyPerRow
        {
            get { return _familyPerRow; }
            set
            {
                _familyPerRow = value;
                RaisePropertyChanged("FamilyPerRow");
            }
        }

        private TblLkpDirection _directionPerRow;

        public TblLkpDirection DirectionPerRow
        {
            get { return _directionPerRow; }
            set
            {
                _directionPerRow = value;
                RaisePropertyChanged("DirectionPerRow");
            }
        }


        private TblStyleCategory _styleCategoryPerRow;

        public TblStyleCategory StyleCategoryPerRow
        {
            get { return _styleCategoryPerRow; }
            set
            {
                _styleCategoryPerRow = value;
                RaisePropertyChanged("StyleCategoryPerRow");
            }
        }

        private int _iserialField;

        private int _tblBrandBudgetHeaderField;

        private int? _tblFamilyField; 

        private int? _tblLkpDirectionField;

        private int? _tblStyleCategoryField;

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

        private double _qtyField;

        public double Qty
        {
            get { return _qtyField; }
            set
            {
                if ((_qtyField.Equals(value) != true))
                {
                    _qtyField = value;

                    RaisePropertyChanged("Qty");
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

        private int _noOfStyles;

        public int NoOfStyles
        {
            get { return _noOfStyles; }
            set { _noOfStyles = value; RaisePropertyChanged("NoOfStyles"); }
        }

        private int _noOfColors;

        public int NoOfColors
        {
            get { return _noOfColors; }
            set { _noOfColors = value; RaisePropertyChanged("NoOfColors"); }
        }

        private int? _tblFactoryGroup;

        public int? TblFactoryGroup
        {
            get { return _tblFactoryGroup; }
            set { _tblFactoryGroup = value; RaisePropertyChanged("TblFactoryGroup"); }
        }

        private int? _tblSalesOrderColorTheme;

        public int? TblSalesOrderColorTheme
        {
            get { return _tblSalesOrderColorTheme; }
            set { _tblSalesOrderColorTheme = value; RaisePropertyChanged("TblSalesOrderColorTheme"); }
        }

        private TblSalesOrderColorTheme _themePerRow;

        public TblSalesOrderColorTheme ThemePerRow
        {
            get { return _themePerRow; }
            set
            {
                _themePerRow = value; RaisePropertyChanged("ThemePerRow");
                if (ThemePerRow != null)
                {
                    PaymentDate = ThemePerRow.DeliveryDate;
                }
            }
        }

        private GenericTable _factoryGroupPerRow;

        public GenericTable FactoryGroupPerRow
        {
            get { return _factoryGroupPerRow; }
            set { _factoryGroupPerRow = value; RaisePropertyChanged("FactoryGroupPerRow"); }
        }

        private DateTime? _paymentDate;

        public DateTime? PaymentDate
        {
            get { return _paymentDate; }
            set { _paymentDate = value; RaisePropertyChanged("PaymentDate"); }
        }

        public int TblBrandBudgetHeader
        {
            get { return _tblBrandBudgetHeaderField; }
            set
            {
                if ((_tblBrandBudgetHeaderField.Equals(value) != true))
                {
                    _tblBrandBudgetHeaderField = value;
                    RaisePropertyChanged("TblBrandBudgetHeader");
                }
            }
        }
        
        public int? TblFamily
        {
            get { return _tblFamilyField; }
            set
            {
                if ((_tblFamilyField.Equals(value) != true))
                {
                    _tblFamilyField = value;
                    RaisePropertyChanged("TblFamily");
                }
            }
        }

        public int? TblLkpDirection
        {
            get { return _tblLkpDirectionField; }
            set
            {
                if ((_tblLkpDirectionField.Equals(value) != true))
                {
                    _tblLkpDirectionField = value;
                    RaisePropertyChanged("TblLkpDirection");
                }
            }
        }

        public int? TblStyleCategory
        {
            get { return _tblStyleCategoryField; }
            set
            {
                if ((_tblStyleCategoryField.Equals(value) != true))
                {
                    _tblStyleCategoryField = value;
                    RaisePropertyChanged("TblStyleCategory");
                }
            }
        }
    }

    public class BrandBudgetViewModel : ViewModelBase
    {
        public int TransactionType = 0;
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        public BrandBudgetViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                TransactionHeader = new TblBrandBudgetsHeaderViewModel();
                GetItemPermissions(PermissionItemName.BrandBudget.ToString());

                MainRowList = new SortableCollectionView<TblBrandBudgetsHeaderViewModel>();
                SelectedDetailRows = new ObservableCollection<TblBrandBudgetDetailViewModel>();
                SelectedDetailRow = new TblBrandBudgetDetailViewModel();
                TransactionHeader.PropertyChanged += (sender, sv) =>
                {
                    if (sv.PropertyName == "Brand")
                    {
                        Brandsection.Clear();
                        lkpClient.GetTblBrandSectionLinkAsync(TransactionHeader.Brand, LoggedUserInfo.Iserial);
                    }
                    else if (sv.PropertyName == "TblLkpBrandSection")
                    {
                        // GetFamilyLink();
                        GetDirectionLink();
                    }

                    else if (sv.PropertyName == "TblLkpDirection")
                    {
                        GetStyleCategoryLink();
                    }


                    DataGridName = "MainGrid";
                };

                var factorGroupClient = new CRUD_ManagerServiceClient();
                factorGroupClient.GetGenericCompleted += (s, sv) =>
                {
                    FactoryGroupList = sv.Result;
                };
                factorGroupClient.GetGenericAsync("TblFactoryGroup", "%%", "%%", "%%", "Iserial", "ASC");

                //lkpClient.GetTblFamilyLinkCompleted += (s, sv) =>
                //{
                //    foreach (var row in sv.Result)
                //    {
                //        if (TransactionHeader.FamilyList != null && (TransactionHeader != null && TransactionHeader.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial)))
                //        {
                //            TransactionHeader.FamilyList.Add( new TblFamily().InjectFrom( row.TblFamily1) as TblFamily);
                //        }
                //    }
                //};

                lkpClient.FamilyCategory_GetTblFamilyCategoryLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (TransactionHeader.FamilyList != null && (TransactionHeader != null && TransactionHeader.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial)))
                        {
                            TransactionHeader.FamilyList.Add(new TblFamily().InjectFrom(row.TblFamily1) as TblFamily);
                        }
                    }
                };


                lkpClient.GetTblDirectionLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (TransactionHeader.DirectionList != null && (TransactionHeader != null && TransactionHeader.DirectionList.All(x => x.Iserial != row.TblLkpDirection1.Iserial)))
                        {
                            TransactionHeader.DirectionList.Add(new TblLkpDirection().InjectFrom(row.TblLkpDirection1) as TblLkpDirection);
                        }
                    }
                };
                
                lkpClient.FamilyCategory_GetTblCategoryLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (TransactionHeader.StyleCategoryList != null && (TransactionHeader != null && TransactionHeader.StyleCategoryList.All(x => x.Iserial != row.TblStyleCategory1.Iserial)))
                        {
                            TransactionHeader.StyleCategoryList.Add(new TblStyleCategory().InjectFrom(row.TblStyleCategory1) as TblStyleCategory);
                        }
                    }
                };


                Client.GetTblSalesOrderColorThemeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (TransactionHeader.ThemeList != null && (TransactionHeader != null && TransactionHeader.ThemeList.All(x => x.Iserial != row.Iserial)))
                        {
                            TransactionHeader.ThemeList.Add(row);
                        }
                    }
                };
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

                lkpClient.GetTblBrandSectionLinkCompleted += (s, ev) =>
                {
                    foreach (var row in ev.Result)
                    {
                        if (Brandsection.All(x => x.Iserial != row.TblLkpBrandSection))
                        {
                            Brandsection.Add(row.TblLkpBrandSection1);
                        }
                    }
                };

                Client.DeleteTblBrandBudgetDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) TransactionHeader.DetailsList.Remove(oldrow);
                };

                Client.UpdateOrInsertTblBrandBudgetDetailCompleted += (x, y) =>
                {
                    var savedRow = (TblBrandBudgetDetailViewModel)TransactionHeader.DetailsList.GetItemAt(y.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(y.Result);
                        if (TransactionHeader.Iserial == 0)
                        {
                            TransactionHeader.Iserial = y.Result.TblBrandBudgetHeader;
                        }
                    }
                };

                Client.GetTblBrandBudgetDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBrandBudgetDetailViewModel();
                        newrow.InjectFrom(row);

                        if (row.TblFamily1 != null)
                        {
                            newrow.FamilyPerRow = row.TblFamily1;

                            if (TransactionHeader != null && TransactionHeader.FamilyList.All(x => x.Iserial != row.TblFamily1.Iserial))
                            {
                                TransactionHeader.FamilyList.Add(newrow.FamilyPerRow);
                            }
                        }

                        if (row.TblLkpDirection1 != null)
                        {
                            newrow.DirectionPerRow = row.TblLkpDirection1;

                            if (TransactionHeader != null && TransactionHeader.DirectionList.All(x => x.Iserial != row.TblLkpDirection1.Iserial))
                            {
                                TransactionHeader.DirectionList.Add(newrow.DirectionPerRow);
                            }
                        }

                        if (row.TblStyleCategory1 != null)
                        {
                            newrow.StyleCategoryPerRow = row.TblStyleCategory1;

                            if (TransactionHeader != null && TransactionHeader.StyleCategoryList.All(x => x.Iserial != row.TblStyleCategory1.Iserial))
                            {
                                TransactionHeader.StyleCategoryList.Add(newrow.StyleCategoryPerRow);
                            }
                        }
                        if (row.TblSalesOrderColorTheme1 != null)
                        {
                            newrow.ThemePerRow = row.TblSalesOrderColorTheme1;
                            if (TransactionHeader != null && TransactionHeader.ThemeList.All(x => x.Iserial != row.TblSalesOrderColorTheme))
                            {
                                TransactionHeader.ThemeList.Add(newrow.ThemePerRow);
                            }
                        }

                        newrow.FactoryGroupPerRow = new GenericTable();
                        if (row.TblFactoryGroup1 != null)
                        {
                            newrow.FactoryGroupPerRow.InjectFrom(row.TblFactoryGroup1);
                        }
                        TransactionHeader.DetailsList.Add(newrow);
                    }
                    TransactionHeader.AddTotalRow();
                };

                Client.GetTblBrandBudgetHeaderCompleted += (y, v) =>
                {
                    foreach (var row in v.Result)
                    {
                        Loading = false;
                        var newrow = new TblBrandBudgetsHeaderViewModel();
                        newrow.InjectFrom(row);
                        newrow.BrandSectionPerRow = row.TblLkpBrandSection1;
                        newrow.SeasonPerRow = row.TblLkpSeason1;
                        MainRowList.Add(newrow);
                    }
                };

                Client.UpdateOrInsertTblBrandBudgetHeaderCompleted += (s, x) =>
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
                Client.DeleteTblBrandBudgetHeaderCompleted += (w, k) =>
                {
                    Loading = false;
                    TransactionHeader.InjectFrom(new TblBrandBudgetsHeaderViewModel());
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

                if (TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.Amount) > TransactionHeader.Amount)
                {
                    MessageBox.Show("Amount of the Detail Exceeded The Brand Amount");
                    SelectedDetailRow.Amount = 0;
                    isvalid = false;
                }

                //if (TransactionType == 1)
                //{
                //    if (TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Count(x => x.TblFamily == SelectedDetailRow.TblFamily) > 1)
                //    {
                //        MessageBox.Show("Cannot Duplicate Family");
                //        SelectedDetailRow.FamilyPerRow = null;
                //        SelectedDetailRow.TblFamily = null;
                //        isvalid = false;
                //    }
                //}

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
            var newrow = new TblBrandBudgetDetailViewModel
            {
                TblBrandBudgetHeader = TransactionHeader.Iserial
            };
            TransactionHeader.DetailsList.Insert(currentRowIndex + 1, newrow);

            SelectedDetailRow = newrow;
        }

        internal void GetFamilyLink()
        {
            if (TransactionHeader.Brand != null && TransactionHeader.TblLkpBrandSection != null && SelectedDetailRow.TblLkpDirection !=null && SelectedDetailRow.TblStyleCategory !=null)
            {
                lkpClient.FamilyCategory_GetTblFamilyCategoryLinkAsync(TransactionHeader.Brand, (int)TransactionHeader.TblLkpBrandSection, (int)SelectedDetailRow.TblLkpDirection,(int)SelectedDetailRow.TblStyleCategory);
            }
        }

        internal void GetDirectionLink()
        {
            if (TransactionHeader.Brand != null && TransactionHeader.TblLkpBrandSection != null)
            {
                lkpClient.GetTblDirectionLinkAsync(TransactionHeader.Brand, (int)TransactionHeader.TblLkpBrandSection);
            }
        }

        internal void GetStyleCategoryLink()
        {
            if (TransactionHeader.Brand != null && TransactionHeader.TblLkpBrandSection != null &&  SelectedDetailRow.TblLkpDirection != null)
            {
                lkpClient.FamilyCategory_GetTblCategoryLinkAsync(TransactionHeader.Brand, (int)TransactionHeader.TblLkpBrandSection,(int) SelectedDetailRow.TblLkpDirection);
            }
        }

        public void GetTheme()
        {
            if (TransactionHeader.Brand != null && TransactionHeader.TblLkpBrandSection != null && TransactionHeader.TblLkpSeason != null)
            {
                Client.GetTblSalesOrderColorThemeAsync(0, int.MaxValue, (int)TransactionHeader.TblLkpSeason, TransactionHeader.Brand, (int)TransactionHeader.TblLkpBrandSection, "it.Iserial", null, null);
            }
        }

        public void GetMaindata()
        {
            Loading = true;
            if (SortBy == null)
            {
                SortBy = "it.Iserial";
            }
            Client.GetTblBrandBudgetHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, TransactionType);
        }

        internal void GetDetailData()
        {
            Loading = true;
            if (DetailSortBy == null)
            {
                DetailSortBy = "it.Iserial";
            }
            Client.GetTblBrandBudgetDetailAsync(TransactionHeader.DetailsList.Count(x => x.Iserial != 0), PageSize, TransactionHeader.Iserial, DetailSortBy,
                DetailFilter, DetailValuesObjects);
        }

        public void SaveMainRow()
        {
            var valiationCollection = new List<ValidationResult>();
            bool isvalid = Validator.TryValidateObject(TransactionHeader,
                new ValidationContext(TransactionHeader, null, null), valiationCollection, true);
            if (isvalid)
            {
                var data = new TblBrandBudgetHeader();
                data.InjectFrom(TransactionHeader);
                data.TransactionType = TransactionType;
                bool save = TransactionHeader.Iserial == 0;
                if (save)
                {
                    if (AllowAdd)
                    {
                        Loading = true;
                        Client.UpdateOrInsertTblBrandBudgetHeaderAsync(data, LoggedUserInfo.Iserial);
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
                        Client.UpdateOrInsertTblBrandBudgetHeaderAsync(data, LoggedUserInfo.Iserial);
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
                if (TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.Amount) > TransactionHeader.Amount)
                {
                    MessageBox.Show("Amount of the Detail Exceeded The Brand Amount");
                    row.Amount = 0;
                    isvalid = false;
                }
                
                //if (TransactionType == 1)
                //{
                //    if (TransactionHeader.DetailsList.Count(x => x.TblFamily == row.TblFamily) > 1)
                //    {
                //        MessageBox.Show("Cannot Duplicate Family");
                //        row.FamilyPerRow = null;
                //        row.TblFamily = null;
                //        isvalid = false;
                //    }
                //}

                if (isvalid && isvalidHeader)
                {
                    var data = new TblBrandBudgetDetail();
                    data.InjectFrom(row);
                    row.TblBrandBudgetHeader = TransactionHeader.Iserial;
                    if (row.TblBrandBudgetHeader == 0)
                    {
                        data.TblBrandBudgetHeader1 =
                            (TblBrandBudgetHeader)new TblBrandBudgetHeader().InjectFrom(TransactionHeader);
                    }
                    bool save = row.Iserial == 0;
                    if (save)
                    {
                        if (AllowAdd)
                        {
                            Loading = true;
                            Client.UpdateOrInsertTblBrandBudgetDetailAsync(data, true,
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
                            Client.UpdateOrInsertTblBrandBudgetDetailAsync(data, false,
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
                            Client.DeleteTblBrandBudgetHeaderAsync(
                                new TblBrandBudgetHeader().InjectFrom(TransactionHeader) as TblBrandBudgetHeader);
                        }
                        else
                        {
                            MessageBox.Show("You are Not Allowed to Delete");
                        }
                    }
                    else
                    {
                        TransactionHeader.InjectFrom(new TblBrandBudgetsHeaderViewModel());
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
                                Client.DeleteTblBrandBudgetDetailAsync(
                                    (TblBrandBudgetDetail)new TblBrandBudgetDetail().InjectFrom(row),
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

        private ObservableCollection<TblBrandBudgetDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblBrandBudgetDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows; }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private TblBrandBudgetDetailViewModel _selectedDetailRow;

        public TblBrandBudgetDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private SortableCollectionView<TblBrandBudgetsHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblBrandBudgetsHeaderViewModel> MainRowList
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

        private ObservableCollection<GenericTable> _factoryGroupList;

        public ObservableCollection<GenericTable> FactoryGroupList
        {
            get { return _factoryGroupList; }
            set { _factoryGroupList = value; RaisePropertyChanged("FactoryGroupList"); }
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

        private TblBrandBudgetsHeaderViewModel _transactionHeader;

        public TblBrandBudgetsHeaderViewModel TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                _transactionHeader = value;
                RaisePropertyChanged("TransactionHeader");
            }
        }
    }
}