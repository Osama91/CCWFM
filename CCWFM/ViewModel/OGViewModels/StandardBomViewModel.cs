using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblStandardBomHeaderViewModel : PropertiesViewModelBase
    {
        private string _brandField;

        private int? _tblLkpBrandSection;

        private ObservableCollection<BomViewModel> _bomList;

        public ObservableCollection<BomViewModel> BomList
        {
            get { return _bomList ?? (_bomList = new ObservableCollection<BomViewModel>()); }
            set { _bomList = value; RaisePropertyChanged("BomList"); }
        }

        private int? _tblFactoryGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFactoryGroup")]
        public int? TblFactoryGroup
        {
            get { return _tblFactoryGroup; }
            set { _tblFactoryGroup = value; RaisePropertyChanged("TblFactoryGroup"); }
        }

        private GenericTable _factoryGroupPerRow;

        public GenericTable FactoryGroupPerRow
        {
            get { return _factoryGroupPerRow; }
            set { _factoryGroupPerRow = value; RaisePropertyChanged("FactoryGroupPerRow"); }
        }

        private int? _tblComplexityGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqComplexityGroup")]
        public int? TblComplexityGroup
        {
            get { return _tblComplexityGroup; }
            set { _tblComplexityGroup = value; RaisePropertyChanged("TblComplexityGroup"); }
        }

        private GenericTable _complexityGroupPerRow;

        public GenericTable ComplexityGroupPerRow
        {
            get { return _complexityGroupPerRow; }
            set { _complexityGroupPerRow = value; RaisePropertyChanged("ComplexityGroupPerRow"); }
        }

        private int? _tblLkpSeasonField;

        private TblLkpSeason _seasonPerRow;

        private TblLkpBrandSection _sectionPerRow;

        private SortableCollectionView<BomViewModel> _detailsList;

        public SortableCollectionView<BomViewModel> DetailsList
        {
            get { return _detailsList ?? (DetailsList = new SortableCollectionView<BomViewModel>()); }
            set { _detailsList = value; RaisePropertyChanged("DetailsList"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set
            {
                _iserial = value; RaisePropertyChanged("Iserial");
            }
        }

        private string _anameField;

        private string _codeField;

        private string _enameField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get
            {
                return _anameField;
            }
            set
            {
                if ((ReferenceEquals(_anameField, value) != true))
                {
                    _anameField = value;
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get
            {
                return _enameField;
            }
            set
            {
                if ((ReferenceEquals(_enameField, value) != true))
                {
                    _enameField = value;
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public string Brand
        {
            get
            {
                return _brandField;
            }
            set
            {
                if ((ReferenceEquals(_brandField, value) != true))
                {
                    _brandField = value;
                    RaisePropertyChanged("Brand");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrandSection")]
        public int? TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSection;
            }
            set
            {
                if ((_tblLkpBrandSection.Equals(value) != true))
                {
                    if (_tblLkpBrandSection == value)
                    {
                        return;
                    }

                    _tblLkpBrandSection = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSeason")]
        public int? TblLkpSeason
        {
            get
            {
                return _tblLkpSeasonField;
            }
            set
            {
                if ((_tblLkpSeasonField.Equals(value) != true))
                {
                    if (_tblLkpSeasonField == value)
                    {
                        return;
                    }
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        public TblLkpBrandSection SectionPerRow
        {
            get
            {
                return _sectionPerRow;
            }
            set
            {
                if ((ReferenceEquals(_sectionPerRow, value) != true))
                {
                    _sectionPerRow = value;
                    RaisePropertyChanged("SectionPerRow");
                }
            }
        }

        public TblLkpSeason SeasonPerRow
        {
            get
            {
                return _seasonPerRow;
            }
            set
            {
                if ((ReferenceEquals(_seasonPerRow, value) != true))
                {
                    _seasonPerRow = value;
                    RaisePropertyChanged("SeasonPerRow");
                }
            }
        }
    }

    #endregion ViewModels

    public class StandardBomViewModel : ViewModelBase
    {
        public StandardBomViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.StandardBomForm.ToString());
                GetSeason();
                var calculationClient = new CRUD_ManagerServiceClient();
                calculationClient.GetGenericCompleted += (s, sv) =>
                {
                    BomCalcMethodList = sv.Result;
                };
                calculationClient.GetGenericAsync("BOM_CalcMethod", "%%", "%%", "%%", "Iserial", "ASC");
                //Client.GetTblPaymentScheduleCompleted += (s, sv) =>
                //{
                //    PaymentScheduleList = sv.Result;
                //};
                //Client.GetTblPaymentScheduleAsync(0, int.MaxValue, "it.Iserial", null, null);
                var uomClient = new CRUD_ManagerServiceClient();
                uomClient.GetGenericCompleted += (s, sv) =>
                {
                    UomList = sv.Result;
                };
                uomClient.GetGenericAsync("tbl_lkp_UoM", "%%", "%%", "%%", "Iserial", "ASC");

                MainRowList = new SortableCollectionView<TblStandardBomHeaderViewModel>();
                SelectedMainRow = new TblStandardBomHeaderViewModel();
                // 
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;

                var factorGroupClient = new CRUD_ManagerServiceClient();
                factorGroupClient.GetGenericCompleted += (s, sv) =>
                {
                    FactoryGroupList = sv.Result;
                };
                factorGroupClient.GetGenericAsync("TblFactoryGroup", "%%", "%%", "%%", "Iserial", "ASC");

                var complixtyGroupClient = new CRUD_ManagerServiceClient();
                complixtyGroupClient.GetGenericCompleted += (s, sv) =>
                {
                    //complix GroupList = sv.Result;
                };
                factorGroupClient.GetGenericAsync("TblFactoryGroup", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                Client.GetTblStandardBOMHeaderCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblStandardBomHeaderViewModel();

                            newrow.InjectFrom(row);
                            if (!MainRowList.Contains(newrow))
                            {
                                newrow.SeasonPerRow = new TblLkpSeason();
                                newrow.FactoryGroupPerRow = new GenericTable();
                                newrow.ComplexityGroupPerRow = new GenericTable();
                                if (BrandSectionList.All(x => x.Iserial != row.TblLkpBrandSection1.Iserial))
                                {
                                    BrandSectionList.Add(new LkpData.TblLkpBrandSection().InjectFrom( row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection);
                                }
                                if (SeasonList.All(x => x.Iserial != row.TblLkpSeason))
                                {
                                    SeasonList.Add(new TblLkpSeason().InjectFrom(row.TblLkpSeason1) as TblLkpSeason);
                                }
                                newrow.FactoryGroupPerRow = new GenericTable().InjectFrom(row.TblFactoryGroup1) as GenericTable;
                                if(row.TblComplexityGroup1 != null)
                                newrow.ComplexityGroupPerRow = new GenericTable().InjectFrom(row.TblComplexityGroup1) as GenericTable;
                                newrow.SectionPerRow = row.TblLkpBrandSection1;

                                newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == newrow.TblLkpSeason);

                                MainRowList.Add(newrow);
                            }
                        }
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
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

                        var handler = ExportCompleted;
                        if (handler != null) handler(this, EventArgs.Empty);
                        //ExportGrid.ExportExcel("Style");
                    }
                };
                GetMaindata();
                Client.UpdateOrInsertTblStandardBOMHeaderCompleted += (s, sv) =>
                {
                    var savedRow = MainRowList.ElementAt(sv.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(sv.Result);
                    }

                    Loading = false;
                };

                Client.DeleteTblStandardBOMHeaderCompleted += (s, ev) =>
                {
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    Loading = false;
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.DeleteTblStandardBOMCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error != null)
                    {
                        throw sv.Error;
                    }

                    var oldrow = SelectedMainRow.BomList.FirstOrDefault(x => x.Iserial == sv.Result);
                    if (oldrow != null) SelectedMainRow.BomList.Remove(oldrow);
                };

                Client.GetTblStandardBOMCompleted += (s, sv) =>
                {
                    SelectedMainRow.BomList.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new BomViewModel
                        {
                            BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable,
                            BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType) as GenericTable

                        };

                        newrow.ColorPerRow = new TblColor();
                        newrow.ColorPerRow = row.TblColor1;
                        
                        newrow.InjectFrom(row);
                        newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.BOM_Fabric);

                        SelectedMainRow.BomList.Add(newrow);
                    }
                    if (SelectedMainRow.BomList.Count == 0)
                    {
                        AddBom(false);
                    }
                };

                Client.UpdateOrInsertTblStandardBOMCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                        return;
                    }
                    
                    GetSalesOrderBom();
                };
            }
        }

        #region GettingData

        public void GetSeason()
        {
            var seasonClient = new CRUD_ManagerServiceClient();
            seasonClient.GetAllSeasonsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    if (SeasonList.All(x => x.Iserial != row.Iserial))
                    {
                        SeasonList.Add(new TblLkpSeason().InjectFrom(row) as TblLkpSeason);
                    }
                }
            };
            seasonClient.GetAllSeasonsAsync();
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Client.GetTblStandardBOMHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial);
            Loading = true;
        }

        public void GetMaindataFull(DataGrid mainGrid)
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Export = true;
            Loading = true;
            ExportGrid = mainGrid;
            Client.GetTblStandardBOMHeaderAsync(MainRowList.Count, int.MaxValue, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial);
        }

        internal void GetSalesOrderBom()
        {
            if (SelectedMainRow != null) Client.GetTblStandardBOMAsync(SelectedMainRow.Iserial);
        }

        #endregion GettingData

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblStandardBomHeaderViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblStandardBomHeaderViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == "Brand")
            {
                if (SelectedMainRow.Brand != null)
                {
                    var brandSectionClient = new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                    };
                    brandSectionClient.GetTblBrandSectionLinkAsync(SelectedMainRow.Brand, LoggedUserInfo.Iserial);
                }
            }
        }

        #region AddingRows

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
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

            var firstOrDefault = BrandList.FirstOrDefault();

            var newrow = new TblStandardBomHeaderViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
            if (firstOrDefault != null)
            {
                newrow.Brand = firstOrDefault.Brand_Code;
            }
        }

        public void AddBom(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.BomList.IndexOf(SelectedBomRow));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedBomRow, new ValidationContext(SelectedBomRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var newrow = new BomViewModel
            {
                TblStandardBomHeader = SelectedMainRow.Iserial,

                IsAcc = false
            };

            SelectedMainRow.BomList.Insert(currentRowIndex + 1, newrow);

            SelectedBomRow = newrow;
        }

        #endregion AddingRows

        #region DeleteRow

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
                            Loading = true;
                            Client.DeleteTblStandardBOMHeaderAsync(
                                (TblStandardBomHeader)new TblStandardBomHeader().InjectFrom(row), MainRowList.IndexOf(row));
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

        internal void DeleteBom()
        {
            if (SelectedBomRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedBomRows)
                    {
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblStandardBOMAsync(
                                (TblStandardBOM)new TblStandardBOM().InjectFrom(row), SelectedMainRow.BomList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.BomList.Remove(row);
                        }
                    }
                }
            }
        }

        #endregion DeleteRow

        #region Save

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }

                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblStandardBomHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Loading = true;
                    Client.UpdateOrInsertTblStandardBOMHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.WFM_UserName);
                }
            }
        }

        internal void SaveBom()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(SelectedBomRow, new ValidationContext(SelectedBomRow, null, null), valiationCollection, true);
            if (isvalid)
            {
                var bomListToSave = new ObservableCollection<TblStandardBOM>();
                foreach (var row in SelectedMainRow.BomList)
                {

                    var newrow = new TblStandardBOM();
                    newrow.InjectFrom(row);

                    bomListToSave.Add(newrow);
                }
                Client.UpdateOrInsertTblStandardBOMAsync(bomListToSave);
            }
           
        }

        #endregion Save

        private ObservableCollection<GenericTable> _factoryGroupList;

        public ObservableCollection<GenericTable> FactoryGroupList
        {
            get { return _factoryGroupList; }
            set { _factoryGroupList = value; RaisePropertyChanged("FactoryGroupList"); }
        }

        private ObservableCollection<GenericTable> _complexityGroupList;

        public ObservableCollection<GenericTable> ComplexityGroupList
        {
            get { return _complexityGroupList; }
            set { _complexityGroupList = value; RaisePropertyChanged("ComplexityGroupList"); }
        }

        public event EventHandler ExportCompleted;

        private ObservableCollection<GenericTable> _bomCalcMethodList;

        public ObservableCollection<GenericTable> BomCalcMethodList
        {
            get { return _bomCalcMethodList; }
            set { _bomCalcMethodList = value; RaisePropertyChanged("BomCalcMethodList"); }
        }

        private ObservableCollection<GenericTable> _uomList;

        public ObservableCollection<GenericTable> UomList
        {
            get { return _uomList; }
            set { _uomList = value; RaisePropertyChanged("UomList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<TblLkpSeason> _seasonList;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private ObservableCollection<TblStandardBomHeaderViewModel> _mainRowList;

        public ObservableCollection<TblStandardBomHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblStandardBomHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblStandardBomHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStandardBomHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private BomViewModel _selectedBomRow;

        public BomViewModel SelectedBomRow
        {
            get { return _selectedBomRow ?? (_selectedBomRow = new BomViewModel()); }
            set { _selectedBomRow = value; RaisePropertyChanged("SelectedBomRow"); }
        }

        private ObservableCollection<BomViewModel> _selectedBomRows;

        public ObservableCollection<BomViewModel> SelectedBomRows
        {
            get { return _selectedBomRows ?? (_selectedBomRows = new ObservableCollection<BomViewModel>()); }
            set { _selectedBomRows = value; RaisePropertyChanged("SelectedBomRows"); }
        }

        private TblStandardBomHeaderViewModel _selectedMainRow;

        public TblStandardBomHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblStandardBomHeaderViewModel()); }
            set
            {
                _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow");
            }
        }

        //internal void SaveBomRow()
        //{
        //    var row = SelectedBomRow;
        //    if (AllowUpdate != true)
        //    {
        //        MessageBox.Show(strings.AllowAddMsg);
        //        return;
        //    }

        //    var bomListToSave = new ObservableCollection<BOM>();

        //    var newrow = new BOM();
        //    newrow.InjectFrom(row);
        //    newrow.TblBOMSizes = row.BomSizes;
        //    newrow.TblBOMStyleColors = new ObservableCollection<TblBOMStyleColor>();
        //    if (row.BomStyleColors != null) newrow.TblBOMStyleColors.InjectFrom(row.BomStyleColors);

        //    if (row.IsAcc)
        //    {
        //        if (newrow.TblBOMStyleColors != null)
        //            foreach (var colorRow in newrow.TblBOMStyleColors)
        //            {
        //                colorRow.FabricColor = colorRow.DummyColor;
        //                colorRow.TblColor = null;
        //                colorRow.TblColor1 = null;
        //            }
        //    }
        //    else
        //    {
        //        if (newrow.TblBOMStyleColors != null)
        //            foreach (var colorRow in newrow.TblBOMStyleColors)
        //            {
        //                colorRow.TblColor = null;
        //                colorRow.TblColor1 = null;
        //            }
        //    }
        //    if (!row.IsSupplierMaterial)
        //    {
        //        if (row.ItemPerRow != null)
        //        {
        //        }
        //    }
        //    bomListToSave.Add(newrow);

        //    var service = new CRUD_ManagerServiceClient();
        //    int index = SelectedMainRow.BomList.IndexOf(SelectedBomRow);
        //    service.UpdateOrInsertBomCompleted += (s, sv) =>
        //    {
        //        if (sv.Error != null)
        //        {
        //            MessageBox.Show(sv.Error.Message);
        //        }
        //        Loading = false;
        //        var rowww = SelectedMainRow.BomList.ElementAt(index);
        //        rowww.InjectFrom(sv.Result.FirstOrDefault());
        //    };

        //    service.UpdateOrInsertBomAsync(bomListToSave);
        //}
    }
}