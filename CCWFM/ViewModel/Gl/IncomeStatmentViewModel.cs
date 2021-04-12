using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class IncomeStatmentViewModel : ViewModelBase
    {
        public IncomeStatmentViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.IncomeStatment.ToString());
                Glclient = new GlServiceClient();
                var glRuleTypeClient = new GlServiceClient();
                glRuleTypeClient.GetGenericCompleted += (s, sv) => {
                    CostCenterTypeList = sv.Result; };
                glRuleTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);             
                MainRowList = new SortableCollectionView<TblIncomeStatmentHeaderModel>();
                SelectedMainRow = new TblIncomeStatmentHeaderModel();
                DesignHeaderList = new ObservableCollection<TblIncomeStatmentDesignHeader>();

                Glclient.GetTblIncomeStatmentDesignHeaderAsync(0, int.MaxValue, "it.Iserial", null, null,
                    LoggedUserInfo.DatabasEname);
                Glclient.GetTblIncomeStatmentDesignHeaderCompleted += (s, sv) =>
                {
                    DesignHeaderList = sv.Result;                
                    if (DesignHeaderList.Count==1)
                    {
                        SelectedMainRow.DesignHeaderPerRow = DesignHeaderList.FirstOrDefault();                        
                    }                  
                };
                Glclient.GetIncomeStatmentDataCostCenterCompleted += (s, sv) =>
                 {
                     SelectedMainRow.DetailsList.Clear();
                     foreach (var variable in sv.DesignDetailList.OrderBy(x => x.RowOrder))
                     {
                         var vis = Visibility.Collapsed;
                         var itemVis = Visibility.Visible;
                         var doubleVis = Visibility.Collapsed;
                         if (variable.Type == "Double Seprator" || variable.Type == "Seprator")
                         {
                             if (variable.Type == "Seprator")
                             {
                                 vis = Visibility.Visible;
                             }
                             if (variable.Type == "Double Seprator")
                             {
                                 doubleVis = Visibility.Visible;
                             }
                             itemVis = Visibility.Collapsed;
                         }

                         var newrow = new TblIncomeStatmentDetailModel
                         {
                             Bold = variable.Bold,
                             Description = variable.Description,
                             RowOrder = variable.RowOrder,
                             Type = variable.Type,
                             IsSeparator = vis,
                             IsItem = itemVis,
                             IsDoubleSeparator = doubleVis,
                         };

                         if (variable.Type.StartsWith("Account"))
                         {
                             if (sv.Result != null)
                             {
                                 var oldrow = sv.Result.FirstOrDefault(x => x.AccountPerRow.Ename == variable.Description);
                                 if (oldrow != null)
                                 {
                                     newrow.CrAmount = oldrow.CrAmount;
                                     newrow.DrAmount = oldrow.DrAmount;
                                     newrow.Accountemp = oldrow.AccountTemp;
                                 }
                             }

                             if (variable.Type == "Account With Subs")
                             {
                                 if (sv.Result != null)
                                 {
                                     var oldrow = sv.Result.Where(x => SelectedMainRow.DetailsList.Select(w => w.Accountemp).Equals(x.AccountTemp)).ToList();

                                    // ReSharper disable once InconsistentNaming
                                    foreach (var VARIABLE in oldrow)
                                     {
                                         var newrowaa = new TblIncomeStatmentDetailModel
                                         {
                                             Bold = variable.Bold,
                                             Description = variable.Description,
                                             RowOrder = variable.RowOrder,
                                             Type = variable.Type,
                                             IsSeparator = vis,
                                             IsItem = itemVis,
                                             IsDoubleSeparator = doubleVis,
                                             CrAmount = VARIABLE.CrAmount,
                                             DrAmount = VARIABLE.DrAmount
                                         };
                                         SelectedMainRow.DetailsList.Add(newrowaa);
                                     }
                                 }
                             }
                         }
                         else if (variable.Type == "Total")
                         {
                             var crAmount =
                                 SelectedMainRow.DetailsList.Where(x => x.Type != "Total" && x.Type != "Account With Subs" && x.RowOrder < variable.RowOrder)
                                     .Sum(x => x.CrAmount);

                             var drAmount =
                                 SelectedMainRow.DetailsList.Where(x => x.Type != "Total" && x.Type != "Account With Subs" && x.RowOrder < variable.RowOrder)
                                     .Sum(x => x.DrAmount);

                             if (drAmount > crAmount)
                             {
                                 newrow.DrAmount = drAmount - crAmount;
                             }
                             else
                             {
                                 newrow.CrAmount = crAmount - drAmount;
                             }
                         }
                         else if (variable.Type == "Income Tax")
                         {
                             var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.RowOrder == variable.RowOrder - 1);

                             if (oldrow != null)
                             {
                                 var value = oldrow.CrAmount ?? 0;

                                 foreach (var taxrow in sv.IncomeTax.OrderBy(w => w.Iserial))
                                 {
                                     if (value >= taxrow.ToValue)
                                     {
                                         newrow.DrAmount = newrow.DrAmount ?? 0 +
                                       (taxrow.ToValue * (decimal)taxrow.TaxPres / 100);
                                         value = (decimal)(value - taxrow.ToValue);
                                     }
                                     else
                                     {
                                         newrow.DrAmount = newrow.DrAmount ?? 0 +
                                    (value * (decimal)taxrow.TaxPres / 100);
                                         value = 0;
                                     }
                                 }
                             }
                         }

                         SelectedMainRow.DetailsList.Add(newrow);

                         Loading = false;
                     }
                 };
                Glclient.GetIncomeStatmentDataCompleted += (s, sv) =>
                {
                    SelectedMainRow.DetailsList.Clear();
                    foreach (var variable in sv.DesignDetailList.OrderBy(x => x.RowOrder))
                    {
                        var vis = Visibility.Collapsed;
                        var itemVis = Visibility.Visible;
                        var doubleVis = Visibility.Collapsed;
                        if (variable.Type == "Double Seprator" || variable.Type == "Seprator")
                        {
                            if (variable.Type == "Seprator")
                            {
                                vis = Visibility.Visible;
                            }
                            if (variable.Type == "Double Seprator")
                            {
                                doubleVis = Visibility.Visible;
                            }
                            itemVis = Visibility.Collapsed;
                        }

                        var newrow = new TblIncomeStatmentDetailModel
                        {
                            Bold = variable.Bold,
                            Description = variable.Description,
                            RowOrder = variable.RowOrder,
                            Type = variable.Type,
                            IsSeparator = vis,
                            IsItem = itemVis,
                            IsDoubleSeparator = doubleVis,
                        };

                        if (variable.Type.StartsWith("Account"))
                        {
                            if (sv.Result != null)
                            {
                                var oldrow = sv.Result.FirstOrDefault(x => x.AccountPerRow.Ename == variable.Description);
                                if (oldrow != null)
                                {
                                    newrow.CrAmount = oldrow.CrAmount;
                                    newrow.DrAmount = oldrow.DrAmount;
                                    newrow.Accountemp = oldrow.AccountTemp;
                                }
                            }

                            if (variable.Type == "Account With Subs")
                            {
                                if (sv.Result != null)
                                {
                                    var oldrow = sv.Result.Where(x => SelectedMainRow.DetailsList.Select(w => w.Accountemp).Equals(x.AccountTemp)).ToList();

// ReSharper disable once InconsistentNaming
                                    foreach (var VARIABLE in oldrow)
                                    {
                                        var newrowaa = new TblIncomeStatmentDetailModel
                                        {
                                            Bold = variable.Bold,
                                            Description = variable.Description,
                                            RowOrder = variable.RowOrder,
                                            Type = variable.Type,
                                            IsSeparator = vis,
                                            IsItem = itemVis,
                                            IsDoubleSeparator = doubleVis,
                                            CrAmount = VARIABLE.CrAmount,
                                            DrAmount = VARIABLE.DrAmount
                                        };
                                        SelectedMainRow.DetailsList.Add(newrowaa);
                                    }
                                }
                            }
                        }
                        else if (variable.Type == "Total")
                        {
                            var crAmount =
                                SelectedMainRow.DetailsList.Where(x => x.Type != "Total" && x.Type != "Account With Subs" && x.RowOrder < variable.RowOrder)
                                    .Sum(x => x.CrAmount);

                            var drAmount =
                                SelectedMainRow.DetailsList.Where(x => x.Type != "Total" && x.Type != "Account With Subs" && x.RowOrder < variable.RowOrder)
                                    .Sum(x => x.DrAmount);

                            if (drAmount > crAmount)
                            {
                                newrow.DrAmount = drAmount - crAmount;
                            }
                            else
                            {
                                newrow.CrAmount = crAmount - drAmount;
                            }
                        }
                        else if (variable.Type == "Income Tax")
                        {
                            var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.RowOrder == variable.RowOrder - 1);

                            if (oldrow != null)
                            {
                                var value = oldrow.CrAmount ?? 0;
                                newrow.DrAmount = 0;
                                foreach (var taxrow in sv.IncomeTax.OrderBy(w => w.Iserial))
                                {
                                    if (value >= taxrow.ToValue)
                                    {
                                        newrow.DrAmount = newrow.DrAmount +(taxrow.ToValue * (decimal)taxrow.TaxPres / 100);
                                        value = (decimal)(value - taxrow.ToValue);
                                    }
                                    else
                                    {

                                        if (value >= taxrow.FromValue)
                                        {
                                            var taxvalue = (value * (decimal)taxrow.TaxPres / 100);
                                            newrow.DrAmount = newrow.DrAmount  + taxvalue;
                                            value = 0;

                                            //newrow.DrAmount = newrow.DrAmount ?? 0 + (taxrow.ToValue * (decimal)taxrow.TaxPres / 100);
                                            //value = (decimal)(value - taxrow.ToValue);
                                        }

                                      
                                    }
                                }
                            }
                        }

                        SelectedMainRow.DetailsList.Add(newrow);

                        Loading = false;
                    }
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow.FromDate != null)
                if (SelectedMainRow.ToDate != null)
                {
                    if (SelectedMainRow.TblCostCenter == 0)
                    {
                        Glclient.GetIncomeStatmentDataAsync((DateTime)SelectedMainRow.FromDate, (DateTime)SelectedMainRow.ToDate, SelectedMainRow.CostCenterType, SelectedMainRow.TblCostCenter, LoggedUserInfo.DatabasEname, SelectedMainRow.Code);
                    }
                    else
                    {
                        Glclient.GetIncomeStatmentDataCostCenterAsync((DateTime)SelectedMainRow.FromDate, (DateTime)SelectedMainRow.ToDate, SelectedMainRow.TblCostCenter, LoggedUserInfo.DatabasEname, SelectedMainRow.Code);

                    }
                }
        }

        public void Clear()
        {
            SelectedMainRow.TblCostCenter = 0;
            SelectedMainRow.CostCenterPerRow = new TblCostCenter();
            SelectedMainRow.CostCenterType = 0;            

        }
        #region Prop

        private ObservableCollection<TblIncomeStatmentHeaderModel> _mainRowList;

        public ObservableCollection<TblIncomeStatmentHeaderModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<GenericTable> _CostCenterTypeList;

        public ObservableCollection<GenericTable> CostCenterTypeList
        {
            get { return _CostCenterTypeList; }
            set { _CostCenterTypeList = value;RaisePropertyChanged("CostCenterTypeList"); }
        }


        private ObservableCollection<TblIncomeStatmentDesignHeader> _designHeader;

        public ObservableCollection<TblIncomeStatmentDesignHeader> DesignHeaderList
        {
            get { return _designHeader; }
            set { _designHeader = value; RaisePropertyChanged("DesignHeaderList"); }
        }

        private ObservableCollection<TblIncomeStatmentHeaderModel> _selectedMainRows;

        public ObservableCollection<TblIncomeStatmentHeaderModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblIncomeStatmentHeaderModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblIncomeStatmentHeaderModel _selectedMainRow;

        public TblIncomeStatmentHeaderModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblIncomeStatmentDetailModel _selectedDetailRow;

        public TblIncomeStatmentDetailModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblIncomeStatmentDetailModel> _selectedDetailRows;

        public ObservableCollection<TblIncomeStatmentDetailModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (SelectedDetailRows = new ObservableCollection<TblIncomeStatmentDetailModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region Models

    public class TblIncomeStatmentHeaderModel: ViewModelBase
    {
        public TblIncomeStatmentHeaderModel()
        {
            Glclient.GetTblCostCenterCompleted += (s, sv) =>
            {
                CostCenterList = sv.Result;
            };  
        }

        private ObservableCollection<TblCostCenter> _CostCenterList;

        public ObservableCollection<TblCostCenter> CostCenterList
        {
            get { return _CostCenterList ?? (_CostCenterList = new ObservableCollection<TblCostCenter>()); }
            set
            {
                _CostCenterList = value;
                RaisePropertyChanged("CostCenterList");
            }
        }
        private int _TblCostCenter;

        public int TblCostCenter
        {
            get { return _TblCostCenter; }
            set { _TblCostCenter = value; RaisePropertyChanged("TblCostCenter"); }
        }

        private TblCostCenter _CostCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _CostCenterPerRow; }
            set { _CostCenterPerRow = value; RaisePropertyChanged("CostCenterPerRow");
            }
        }

        private int _CostCenterType;

        public int CostCenterType
        {
            get { return _CostCenterType; }
            set { _CostCenterType = value; RaisePropertyChanged("CostCenterType");
                Glclient.GetTblCostCenterAsync(0,int.MaxValue, CostCenterType, "it.Iserial", null, null, LoggedUserInfo.DatabasEname, 0, 0,0);
            }
        }

        private ObservableCollection<TblIncomeStatmentDetailModel> _detailLst;

        public ObservableCollection<TblIncomeStatmentDetailModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new ObservableCollection<TblIncomeStatmentDetailModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private DateTime? _fromDate;

        public DateTime? FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChanged("FromDate"); }
        }

        private DateTime? _toDate;

        public DateTime? ToDate
        {
            get { return _toDate; }
            set { _toDate = value; RaisePropertyChanged("ToDate"); }
        }

        //private string _code;

        //public string Code
        //{
        //    get { return _code; }
        //    set { _code = value; RaisePropertyChanged("Code"); }
        //}

        private TblIncomeStatmentDesignHeader _designHeaderPerRow;

        public TblIncomeStatmentDesignHeader DesignHeaderPerRow
        {
            get { return _designHeaderPerRow; }
            set { _designHeaderPerRow = value; RaisePropertyChanged("DesignHeaderPerRow"); }
        }
    }

    public class TblIncomeStatmentDetailModel: Web.DataLayer.PropertiesViewModelBase
    {
        public int Accountemp { get; set; }

        private Visibility _isSeparator;

        public Visibility IsSeparator
        {
            get { return _isSeparator; }
            set { _isSeparator = value; RaisePropertyChanged("IsSeparator"); }
        }

        private Visibility _isDoubleSeparator;

        public Visibility IsDoubleSeparator
        {
            get { return _isDoubleSeparator; }
            set { _isDoubleSeparator = value; RaisePropertyChanged("IsDoubleSeparator"); }
        }

        private Visibility _isItem;

        public Visibility IsItem
        {
            get { return _isItem; }
            set { _isItem = value; RaisePropertyChanged("IsItem"); }
        }

        private string _description;

        [Required]
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        private int _rowOrder;

        public int RowOrder
        {
            get { return _rowOrder; }
            set { _rowOrder = value; RaisePropertyChanged("RowOrder"); }
        }

        private bool _bold;

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; RaisePropertyChanged("Bold"); }
        }

        private decimal? _drAmount;

        public decimal? DrAmount
        {
            get { return _drAmount; }
            set
            {
                _drAmount = value;
                RaisePropertyChanged("DrAmount");
            }
        }

        private decimal? _crAmount;

        public decimal? CrAmount
        {
            get { return _crAmount; }
            set
            {
                _crAmount = value;
                RaisePropertyChanged("CrAmount");
            }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged("Type"); }
        }
    }

    #endregion Models
}