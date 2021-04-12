using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblSeasonViewModel : GenericViewModel
    {
        private SortableCollectionView<TblSeasonTrackViewModel> _tblSubProductGroupField;

        public SortableCollectionView<TblSeasonTrackViewModel> DetailsList
        {
            get
            {
                return _tblSubProductGroupField ?? (_tblSubProductGroupField = new SortableCollectionView<TblSeasonTrackViewModel>());
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

        private string _shortCode;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqShortCode")]
        public string ShortCode
        {
            get { return _shortCode; }
            set { _shortCode = value; RaisePropertyChanged("ShortCode"); }
        }

        private TblSeason _retailSeasonPerRow;

        public TblSeason RetailSeasonPerRow
        {
            get { return _retailSeasonPerRow; }
            set
            {
                _retailSeasonPerRow = value; RaisePropertyChanged("RetailSeasonPerRow");
                if (_retailSeasonPerRow != null)
                {
                    ShortCode = RetailSeasonPerRow.Code;
                }
            }
        }
    }

    public class TblSeasonTrackViewModel : PropertiesViewModelBase
    {
        private string _brandField;

        private DateTime? _fromDateField;

        private int _iserialField;

        private int? _tblLkpBrandSectionField;

        private int? _tblLkpSeasonField;

        private int? _tblSeasonTrackTypeField;

        private DateTime? _toDateField;

        private LkpData.TblLkpBrandSection _sectionPerrow;

        public LkpData.TblLkpBrandSection SectionPerRow
        {
            get
            {
                return _sectionPerrow;
            }
            set
            {
                if ((ReferenceEquals(_sectionPerrow, value) != true))
                {
                    _sectionPerrow = value;
                    RaisePropertyChanged("SectionPerRow");
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

                    var brandSectionClient =  new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                        if (TblLkpBrandSection != null && TblLkpBrandSection != 0)
                        {
                            SectionPerRow = BrandSectionList.FirstOrDefault(x => x.Iserial == TblLkpBrandSection);
                        }
                    };
                    brandSectionClient.GetTblBrandSectionLinkAsync(Brand, LoggedUserInfo.Iserial);
                }
            }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromDate")]
        public DateTime? FromDate
        {
            get
            {
                return _fromDateField;
            }
            set
            {
                if ((_fromDateField.Equals(value) != true))
                {
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
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
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSeasonTrackType")]
        public int? TblSeasonTrackType
        {
            get
            {
                return _tblSeasonTrackTypeField;
            }
            set
            {
                if ((_tblSeasonTrackTypeField.Equals(value) != true))
                {
                    _tblSeasonTrackTypeField = value;
                    RaisePropertyChanged("TblSeasonTrackType");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToDate")]
        public DateTime? ToDate
        {
            get
            {
                return _toDateField;
            }
            set
            {
                if ((_toDateField.Equals(value) != true))
                {
                    _toDateField = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
    }

    #endregion ViewModels

    public class SeasonViewModel : ViewModelBase
    {
        public SeasonViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetGenericCompleted += (s, sv) =>
                {
                    SeasonTrackTypeList = sv.Result;
                };

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                Client.GetGenericAsync("TblSeasonTrackType", "%%", "%%", "%%", "Iserial", "ASC");
                MainRowList = new SortableCollectionView<TblSeasonViewModel>();
                SelectedMainRow = new TblSeasonViewModel();
                SelectedDetailRow = new TblSeasonTrackViewModel();
                
                SelectedMainRow.DetailsList.OnRefresh += DetailsList_OnRefresh;

                Client.GetTblLkpSeasonCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSeasonViewModel();
                        newrow.InjectFrom(row);
                        newrow.RetailSeasonPerRow = new TblSeason { Code = newrow.ShortCode };
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblSeasonTrackCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSeasonTrackViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };
                Client.UpdateOrInsertTblLkpSeasonCompleted += (s, x) =>
                {
                    var savedRow = (TblSeasonViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblSeasonTrackCompleted += (s, x) =>
                {
                    var savedRow = (TblSeasonTrackViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    //if (x.Result.TblSalaryRelation1 != null)
                    //{
                    //    var headerIserial = x.Result.TblSalaryRelation;

                    //    SelectedMainRow.Iserial = headerIserial;
                    //}
                };

                Client.DeleteTblLkpSeasonCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblSeasonTrackCompleted += (s, ev) =>
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

            Client.GetTblLkpSeasonAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblLkpSeasonAsync(
                                (TblLkpSeason)new TblLkpSeason().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblSeasonViewModel());
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
                    var saveRow = new TblLkpSeason();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblLkpSeasonAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            if (SelectedMainRow != null)
                Client.GetTblSeasonTrackAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
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
                            Client.DeleteTblSeasonTrackAsync((TblSeasonTrack)new TblSeasonTrack().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
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
                        SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblSeasonTrackViewModel
                        {
                            TblLkpSeason = SelectedMainRow.Iserial
                        });
                    }
                }
                else
                {
                    SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblSeasonTrackViewModel
                    {
                        TblLkpSeason = SelectedMainRow.Iserial
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
                    var rowToSave = new TblSeasonTrack();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblSeasonTrackAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        #region Prop

        private ObservableCollection<GenericTable> _seasonTrackTypeList;

        public ObservableCollection<GenericTable> SeasonTrackTypeList
        {
            get { return _seasonTrackTypeList; }
            set { _seasonTrackTypeList = value; RaisePropertyChanged("SeasonTrackTypeList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private SortableCollectionView<TblSeasonViewModel> _mainRowList;

        public SortableCollectionView<TblSeasonViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSeasonViewModel> _selectedMainRows;

        public ObservableCollection<TblSeasonViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSeasonViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblSeasonViewModel _selectedMainRow;

        public TblSeasonViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblSeasonTrackViewModel _selectedDetailRow;

        public TblSeasonTrackViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblSeasonTrackViewModel> _selectedDetailRows;

        public ObservableCollection<TblSeasonTrackViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSeasonTrackViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}