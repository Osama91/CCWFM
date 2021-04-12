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

    public class TblPurchaseBudgetHeaderViewModel : GenericViewModel
    {
        private SortableCollectionView<TblPurchaseBudgetDetailViewModel> _tblPurchaseBudgetHeaderField;

        public SortableCollectionView<TblPurchaseBudgetDetailViewModel> DetailsList
        {
            get
            {
                return _tblPurchaseBudgetHeaderField ?? (_tblPurchaseBudgetHeaderField = new SortableCollectionView<TblPurchaseBudgetDetailViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblPurchaseBudgetHeaderField, value) != true))
                {
                    _tblPurchaseBudgetHeaderField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }
    }

    public class TblPurchaseBudgetDetailViewModel : PropertiesViewModelBase
    {
        private int _tblPurchaseBudgetHeader;
        private int _iserialField;

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

        public int TblPurchaseBudgetHeader
        {
            get
            {
                return _tblPurchaseBudgetHeader;
            }
            set
            {
                if ((Equals(_tblPurchaseBudgetHeader, value) != true))
                {
                    _tblPurchaseBudgetHeader = value;
                    RaisePropertyChanged("TblPurchaseBudgetHeader");
                }
            }
        }

        private int _amount;

        public int Amount
        {
            get { return _amount; }
            set { _amount = value;RaisePropertyChanged("Amount"); }
        }
        
        private string _brandField;

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

                    var brandSectionClient = new LkpData.LkpDataClient();
                    brandSectionClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                    };
                    if (Brand != null) brandSectionClient.GetTblBrandSectionLinkAsync(Brand, LoggedUserInfo.Iserial);
                }
            }
        }

        private int? _tblLkpBrandSection;

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

        private TblLkpBrandSection _sectionPerRow;

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

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }



        private TblLkpSeason _seasonPerRow;

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

        private int? _tblLkpSeasonField;

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



}


#endregion ViewModels

public class PurchaseBudgetViewModel : ViewModelBase
    {
        public PurchaseBudgetViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblPurchaseBudgetHeaderViewModel>();
                SelectedMainRow = new TblPurchaseBudgetHeaderViewModel();
                SelectedDetailRow = new TblPurchaseBudgetDetailViewModel();

                Client.GetTblPurchaseBudgetHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseBudgetHeaderViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblPurchaseBudgetDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPurchaseBudgetDetailViewModel();
                        newrow.InjectFrom(row);
                        newrow.SectionPerRow = row.TblLkpBrandSection1;
                        newrow.SeasonPerRow = row.TblLkpSeason1;
                     
                        SelectedMainRow.DetailsList.Add(newrow);
                     
                    }
                    if (sv.Result.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                };
                Client.UpdateOrInsertTblPurchaseBudgetHeaderCompleted += (s, x) =>
                {
                    var savedRow = MainRowList.ElementAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblPurchaseBudgetDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblPurchaseBudgetDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
               

                Client.DeleteTblPurchaseBudgetHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblPurchaseBudgetDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };
        

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandList = sv.Result;
                };

                Client.GetAllSeasonsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result.Where(w=>w.IsMaster==true))
                    {
                        if (SeasonList.All(x => x.Iserial != row.Iserial))
                        {
                            SeasonList.Add(new TblLkpSeason().InjectFrom(row) as TblLkpSeason);
                        }
                    }
                };
                Client.GetAllSeasonsAsync();
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Client.GetTblPurchaseBudgetHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                        Client.DeleteTblPurchaseBudgetHeaderAsync(
                            (TblPurchaseBudgetHeader)new TblPurchaseBudgetHeader().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblPurchaseBudgetHeaderViewModel());
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
                    var saveRow = new TblPurchaseBudgetHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblPurchaseBudgetHeaderAsync(saveRow, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                Client.GetTblPurchaseBudgetDetailAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailValuesObjects);
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
                        Client.DeleteTblPurchaseBudgetDetailAsync((TblPurchaseBudgetDetail)new TblPurchaseBudgetDetail().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
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

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblPurchaseBudgetDetailViewModel
                {
                    TblPurchaseBudgetHeader = SelectedMainRow.Iserial
                });
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
                    var rowToSave = new TblPurchaseBudgetDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblPurchaseBudgetDetailAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }
    

        #region Prop

 
        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<TblLkpSeason> _seasonList;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private ObservableCollection<TblPurchaseBudgetHeaderViewModel> _mainRowList;

        public ObservableCollection<TblPurchaseBudgetHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPurchaseBudgetHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblPurchaseBudgetHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPurchaseBudgetHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPurchaseBudgetHeaderViewModel _selectedMainRow;

        public TblPurchaseBudgetHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblPurchaseBudgetDetailViewModel _selectedDetailRow;

        public TblPurchaseBudgetDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblPurchaseBudgetDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPurchaseBudgetDetailViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPurchaseBudgetDetailViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}