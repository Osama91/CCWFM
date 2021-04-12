using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using System.ComponentModel;

namespace CCWFM.ViewModel.Gl
{
    public class ShopAreaViewModel : ViewModelBase
    {
        public ShopAreaViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.ShopArea.ToString());
                Glclient = new GlServiceClient();

                var costDimSetupTypeClient = new GlServiceClient();
                costDimSetupTypeClient.GetGenericCompleted += (s, sv) => { BrandList = sv.Result; };
                costDimSetupTypeClient.GetGenericAsync("TblItemDownLoadDef", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetRetailChainSetupByCodeAsync("ShopAreaCostCenterType", LoggedUserInfo.DatabasEname);

                Glclient.GetRetailChainSetupByCodeCompleted += (s, sv) =>
                {
                    int costcentertype = Convert.ToInt32( sv.Result.sSetupValue);
                    Glclient.GetTblCostCenterAsync(MainRowList.Count, PageSize, costcentertype, "it.Iserial", null, null,
                 LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial, 0, 0);
                };
                Glclient.GetTblCostCenterCompleted += (s, sv) =>
                {
                    CostCenterList = sv.Result;
                };
                    MainRowList = new ObservableCollection<TblshopareaModel>();
                SelectedMainRow = new TblshopareaModel();

                Glclient.GetshopareaCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblshopareaModel();
                        newrow.InjectFrom(row);
                        var storeTempRow = sv.StoreList.FirstOrDefault(w => w.code == row.shopcode);
                        if (storeTempRow!=null)
                        {
                            newrow.StorePerRow = new CRUDManagerService.StoreForAllCompany()
                            {
                                Code = storeTempRow.code,
                                Aname = storeTempRow.aname,
                                Ename = storeTempRow.ENAME,
                            };
                        }
                     
                        newrow.BrandPerRow = new GenericTable();

                        var BrandTempRow = sv.BrandList.FirstOrDefault(w => w.Code == row.Brand);
                        newrow.BrandPerRow = new GenericTable()
                        {
                            Code = BrandTempRow.Code,
                            Aname = BrandTempRow.Aname,
                            Ename = BrandTempRow.Ename,
                        };
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                    if (Export)
                    {
                        Export = false;
                        AllowExport = true;
                        //ExportGrid.ExportExcel("Account");
                    }
                };

                Glclient.UpdateOrInsertshopareaCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteshopareaCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.GetTblCostCenterShopAreaCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostCenterShopAreaModel();
                        newrow.InjectFrom(row);
                        var storeTempRow = CostCenterList.FirstOrDefault(w => w.Iserial == row.TblCostCenter);
                        if (storeTempRow != null)
                        {
                            newrow.CostCenterPerRow= new TblCostCenter()
                            {
                                Code = storeTempRow.Code,
                                Aname = storeTempRow.Aname,
                                Ename = storeTempRow.Ename,
                                TblCostCenterType= storeTempRow.TblCostCenterType
                            };
                        }                    
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                 
                };


                Glclient.UpdateOrInsertTblCostCenterShopAreasCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedMainRow.DetailsList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblCostCenterShopAreaCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };




                GetMaindata();
            }
        }
        #region Header
        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetshopareaAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetshopareaAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblshopareaModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }
        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new shoparea();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertshopareaAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblshopareaModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new shoparea();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertshopareaAsync(saveRow, save, MainRowList.IndexOf(oldRow),
                         LoggedUserInfo.DatabasEname);
                    }
                }
            }
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
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Glclient.DeleteshopareaAsync((shoparea)new shoparea().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
                        }
                    }
                }
            }
        }

        #endregion






        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblCostCenterShopAreaAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
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
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteTblCostCenterShopAreaAsync(
                                (TblCostCenterShopArea)new TblCostCenterShopArea().InjectFrom(row),
                                SelectedMainRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            var newrow = new TblCostCenterShopAreaModel { shoparea = SelectedMainRow.Iserial };

            newrow.JournalAccountType = new GenericTable()
            {
                Iserial = CostCenterList.FirstOrDefault().TblCostCenterType??0,
                Ename="",
                Aname="",
                Code="",

            };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    if (SelectedDetailRow.shoparea == 0)
                    {
                        SelectedDetailRow.shoparea = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblCostCenterShopArea();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertTblCostCenterShopAreasAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void ShowLinkChildWindow()
        {
            GetDetailData();
            var child = new Views.Gl.ChildWindow.CostCenterShopArea(this);
            child.Show();
        }


        #region Prop

        private ObservableCollection<TblshopareaModel> _mainRowList;

        public ObservableCollection<TblshopareaModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }
        private ObservableCollection<TblCostCenter> _CostCenterList;

        public ObservableCollection<TblCostCenter> CostCenterList
        {
            get { return _CostCenterList; }
            set { _CostCenterList = value; RaisePropertyChanged("CostCenterList"); }
        }


        private ObservableCollection<TblshopareaModel> _selectedMainRows;

        public ObservableCollection<TblshopareaModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblshopareaModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _BrandList;

        public ObservableCollection<GenericTable> BrandList
        {
            get { return _BrandList; }
            set { _BrandList = value; RaisePropertyChanged("BrandList"); }
        }

        private TblshopareaModel _selectedMainRow;

        public TblshopareaModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblCostCenterShopAreaModel _selectedDetailRow;

        public TblCostCenterShopAreaModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblCostCenterShopAreaModel> _selectedDetailRows;

        public ObservableCollection<TblCostCenterShopAreaModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblCostCenterShopAreaModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region Models

    public class TblshopareaModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblCostCenterShopAreaModel> _detailsList;

        public ObservableCollection<TblCostCenterShopAreaModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblCostCenterShopAreaModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }


        private CRUDManagerService.StoreForAllCompany _StorePerRow;

        public CRUDManagerService.StoreForAllCompany StorePerRow
        {
            get { return _StorePerRow; }
            set
            {
                _StorePerRow = value; RaisePropertyChanged("StorePerRow");
                shopcode = _StorePerRow.Code;
                ShopName = _StorePerRow.Ename;
            }
        }
        private GenericTable _BrandPerRow;

        public GenericTable BrandPerRow
        {
            get { return _BrandPerRow; }
            set
            {
                _BrandPerRow = value; RaisePropertyChanged("BrandPerRow");
                Brand = BrandPerRow.Code;
            }
        }

        private string BrandField;

        private int IserialField;

        private decimal? RatioField;

        private string ShopNameField;

        private decimal? areaField;

        private decimal? areabField;

        private string shopcodeField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public string Brand
        {
            get
            {
                return BrandField;
            }
            set
            {
                if ((ReferenceEquals(BrandField, value) != true))
                {
                    BrandField = value;
                    RaisePropertyChanged("Brand");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public decimal? Ratio
        {
            get
            {
                return RatioField;
            }
            set
            {
                if ((RatioField.Equals(value) != true))
                {
                    RatioField = value;
                    RaisePropertyChanged("Ratio");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStore")]
        [ReadOnly(true)]
        public string ShopName
        {
            get
            {
                return ShopNameField;
            }
            set
            {
                if ((ReferenceEquals(ShopNameField, value) != true))
                {
                    ShopNameField = value;
                    RaisePropertyChanged("ShopName");
                }
            }
        }


        public decimal? area
        {
            get
            {
                return areaField;
            }
            set
            {
                if ((areaField.Equals(value) != true))
                {
                    areaField = value;
                    RaisePropertyChanged("area");
                }
            }
        }


        public decimal? areab
        {
            get
            {
                return areabField;
            }
            set
            {
                if ((areabField.Equals(value) != true))
                {
                    areabField = value;
                    RaisePropertyChanged("areab");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStore")]
        public string shopcode
        {
            get
            {
                return shopcodeField;
            }
            set
            {
                if ((ReferenceEquals(shopcodeField, value) != true))
                {
                    shopcodeField = value;
                    RaisePropertyChanged("shopcode");
                }
            }
        }


    }

    public class TblCostCenterShopAreaModel : PropertiesViewModelBase
    {

        private int _TblCostCenterType;

        public int TblCostCenterType
        {
            get { return _TblCostCenterType; }
            set { _TblCostCenterType = value; RaisePropertyChanged("TblCostCenterType"); }
        }

        private GenericTable _JournalAccountType;

        public GenericTable JournalAccountType
        {
            get { return _JournalAccountType; }
            set
            {
                if ((ReferenceEquals(_JournalAccountType, value) != true))
                {
                    _JournalAccountType = value;
                    RaisePropertyChanged("JournalAccountType");
                    if (JournalAccountType != null) TblCostCenterType = JournalAccountType.Iserial;
                }
            }
        }

        private TblCostCenter _costCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _costCenterPerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterPerRow, value) != true))
                {
                    _costCenterPerRow = value;
                    RaisePropertyChanged("CostCenterPerRow");
                    if (_costCenterPerRow != null)
                    {
                        TblCostCenter = _costCenterPerRow.Iserial;
                        
                    }
                    else
                    {
                        
                    }
                }
            }
        }


        private int IserialField;

        private int? TblCostCenterField;

        private int shopareaField;

        
        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenter")]
        public int? TblCostCenter
        {
            get
            {
                return TblCostCenterField;
            }
            set
            {
                if ((TblCostCenterField.Equals(value) != true))
                {
                    TblCostCenterField = value;
                    RaisePropertyChanged("TblCostCenter");
                }
            }
        }

        
        public int shoparea
        {
            get
            {
                return shopareaField;
            }
            set
            {
                if ((shopareaField.Equals(value) != true))
                {
                    shopareaField = value;
                    RaisePropertyChanged("shoparea");
                }
            }
        }
    }

    #endregion Models
}