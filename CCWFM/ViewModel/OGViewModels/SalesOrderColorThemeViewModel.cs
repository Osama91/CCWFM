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
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblSalesOrderColorThemeViewModel : GenericViewModel
    {
        private DateTime? _deliveryDateField;

        private string _tblBrandField;

        private int _tblLkpBrandSectionField;

        private int _tblLkpSeasonField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
        public DateTime? DeliveryDate
        {
            get
            {
                return _deliveryDateField;
            }
            set
            {
                if ((_deliveryDateField.Equals(value) != true))
                {
                    _deliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }

        private DateTime? _shopDeliveryDate;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
        public DateTime? ShopDeliveryDate
        {
            get
            {
                return _shopDeliveryDate;
            }
            set
            {
                if ((_shopDeliveryDate.Equals(value) != true))
                {
                    _shopDeliveryDate = value;
                    RaisePropertyChanged("ShopDeliveryDate");
                }
            }
        }

        public string TblBrand
        {
            get
            {
                return _tblBrandField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandField, value) != true))
                {
                    _tblBrandField = value;
                    RaisePropertyChanged("TblBrand");
                }
            }
        }

        public int TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSectionField;
            }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        public int TblLkpSeason
        {
            get
            {
                return _tblLkpSeasonField;
            }
            set
            {
                if ((_tblLkpSeasonField.Equals(value) != true))
                {
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }
    }

    #endregion ViewModels

    public class SalesOrderColorThemeViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        public SalesOrderColorThemeViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.ColorThemesForm.ToString());

                lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                {
                    BrandSectionList.Clear();
                    foreach (var row in sv.Result)
                    {
                        BrandSectionList.Add(new GenericTable().InjectFrom(row.TblLkpBrandSection1) as GenericTable);
                    }
                };
                var seasonClient = new CRUD_ManagerServiceClient();
                seasonClient.GetGenericCompleted += (s, sv) =>
                {
                    SeasonList = sv.Result;
                };
                seasonClient.GetGenericAsync("TblLkpSeason", "%%", "%%", "%%", "Iserial", "ASC");

                MainRowList = new SortableCollectionView<TblSalesOrderColorThemeViewModel>();
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;
                SelectedMainRow = new TblSalesOrderColorThemeViewModel();
                

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                Client.GetTblSalesOrderColorThemeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalesOrderColorThemeViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
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
                        ExportGrid.ExportExcel("Style");
                    }
                };

                Client.UpdateOrInsertTblSalesOrderColorThemeCompleted += (s, x) =>
                {
                    var savedRow = (TblSalesOrderColorThemeViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.DeleteTblSalesOrderColorThemeCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                //   Client.GetSeasonalMasterListNotLinkedToSalesorder
            }
        }

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSalesOrderColorThemeViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSalesOrderColorThemeViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblSalesOrderColorThemeAsync(MainRowList.Count, PageSize, Season, Brand, BrandSection, SortBy, Filter, ValuesObjects);
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
                            Loading = true;
                            Client.DeleteTblSalesOrderColorThemeAsync(
                                (TblSalesOrderColorTheme)new TblSalesOrderColorTheme().InjectFrom(row), MainRowList.IndexOf(row));
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

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
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

                MainRowList.Insert(currentRowIndex + 1, new TblSalesOrderColorThemeViewModel());

                var newrow = new TblSalesOrderColorThemeViewModel();
                MainRowList.Insert(currentRowIndex + 1, newrow);
                SelectedMainRow = newrow;
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
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }
                    var saveRow = new TblSalesOrderColorTheme();
                    saveRow.InjectFrom(SelectedMainRow);

                    saveRow.TblBrand = Brand;
                    saveRow.TblLkpBrandSection = BrandSection;
                    saveRow.TblLkpSeason = Season;
                    Client.UpdateOrInsertTblSalesOrderColorThemeAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Props

        private int _season;

        public int Season
        {
            get { return _season; }
            set
            {
                _season = value;
                RaisePropertyChanged("Season");
                Getdata();
            }
        }

        private string _brand;

        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                RaisePropertyChanged("Brand");
                Getdata();
                if (Brand != null) lkpClient.GetTblBrandSectionLinkAsync(Brand,  LoggedUserInfo.Iserial);
            }
        }

        private int _brandSection;

        public int BrandSection
        {
            get { return _brandSection; }
            set
            {
                _brandSection = value;
                RaisePropertyChanged("BrandSection");
                Getdata();
            }
        }

        private void Getdata()
        {
            if (Season != 0 && Brand != null && BrandSection != 0)
            {
                MainRowList.Clear();
                GetMaindata();
            }
        }

        private ObservableCollection<GenericTable> _brandSectionList;

        public ObservableCollection<GenericTable> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<GenericTable>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<GenericTable> _seasonList;

        public ObservableCollection<GenericTable> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<GenericTable>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private SortableCollectionView<TblSalesOrderColorThemeViewModel> _mainRowList;

        public SortableCollectionView<TblSalesOrderColorThemeViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSalesOrderColorThemeViewModel> _selectedMainRows;

        public ObservableCollection<TblSalesOrderColorThemeViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSalesOrderColorThemeViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblSalesOrderColorThemeViewModel _selectedMainRow;

        public TblSalesOrderColorThemeViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Props
    }
}