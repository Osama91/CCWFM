using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class ColorLinkViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public ColorLinkViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandsList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                var seasonClient = new CRUD_ManagerServiceClient();
                seasonClient.GetGenericCompleted += (s, sv) =>
                {
                    SeasonList = sv.Result;
                };
                seasonClient.GetGenericAsync("TblLkpSeason", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetTblColorCompleted += (s, sv) =>
                {
                    // ColorsList.Clear();
                    //  GenericMapper.InjectFromObCollection(ColorsList, sv.Result);

                    foreach (var row in sv.Result)
                    {
                        ColorsList.Add(new TblColor().InjectFrom(row) as TblColor);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                };
                Client.GetTblColorLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        //if (!MainRowList.Contains(row.TblColor1))
                        //{
                        TblColor col = new TblColor();
                        col.Iserial = row.TblColor1.Iserial;
                        col.Code = row.TblColor1.Code;
                        col.Ename = row.TblColor1.Ename;
                        col.Aname = row.TblColor1.Aname;
                        col.PantonCode = row.PantonCode;

                        MainRowList.Add(col);
                        // }
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };

                lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                {
                    if (Copy)
                    {
                        BrandSectionListCopy.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionListCopy.Add(row.TblLkpBrandSection1);
                        }
                        Copy = false;
                    }
                    else
                    {
                        BrandSectionList.Clear();
                        foreach (var row in sv.Result)
                        {
                            BrandSectionList.Add(row.TblLkpBrandSection1);
                        }
                    }
                    Loading = false;
                };

                Client.UpdateOrDeleteTblColorLinkCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                    }
                    else
                    {
                        MainRowList.Clear();
                        GetMaindata();
                    }
                    Loading = false;
                };

                Client.CopyColorLinkCompleted += (s, sv) =>
                {
                    Loading = false;
                };
            }
        }

        public void GetDetaildata()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";

            Loading = true;
            Client.GetTblColorAsync(ColorsList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.TblColor";
            Loading = true;
            Client.GetTblColorLinkAsync(MainRowList.Count, PageSize, BrandCode, BrandSection, Season, SortBy, Filter, ValuesObjects);
        }

        public void AddToColorLink()
        {
            foreach (var row in MainSelectedRows)
            {
                Loading = true;
                var newrow = new TblColorLink
                {
                    TblBrand = BrandCode,
                    TblColor = row.Iserial,
                    TblLkpSeason = Season,
                    TblLkpBrandSection = BrandSection
                };

                Client.UpdateOrDeleteTblColorLinkAsync(
                   newrow, true, 0);
            }
        }

        public void UpdateColorLinkPanton()
        {
            foreach (var row in UpdatedPantonRows)
            {
                Loading = true;
                var newrow = new TblColorLink
                {
                    TblBrand = BrandCode,
                    TblColor = row.Iserial,
                    TblLkpSeason = Season,
                    TblLkpBrandSection = BrandSection,
                    PantonCode = row.PantonCode
                };

                Client.UpdateColorLinkPantonAsync(
                   newrow, true, 0);
            }
        }

        public void CopyColorLink()
        {
            var newrow = new TblColorLink
            {
                TblBrand = BrandCode,
                //TblColor = MainRowList.FirstOrDefault().Iserial,
                TblLkpSeason = Season,
                TblLkpBrandSection = BrandSection
            };
            Loading = true;
            Client.CopyColorLinkAsync(newrow, BrandCodeCopy, BrandSectionCopy, SeasonCopy);
        }

        internal void DeleteColorLink()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        Loading = true;
                        var newrow = new TblColorLink
                        {
                            TblBrand = BrandCode,
                            TblColor = row.Iserial,
                            TblLkpSeason = Season,
                            TblLkpBrandSection = BrandSection
                        };

                        Client.UpdateOrDeleteTblColorLinkAsync(
                           newrow, false, MainRowList.IndexOf(row));
                    }
                }
            }
        }

        #region Prop

        private ObservableCollection<GenericTable> _seasonList;

        public ObservableCollection<GenericTable> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<GenericTable>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }

        private ObservableCollection<Brand> _brandsList;

        public ObservableCollection<Brand> BrandsList
        {
            get { return _brandsList; }
            set { _brandsList = value; RaisePropertyChanged("BrandsList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionListCopy;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionListCopy
        {
            get { return _brandSectionListCopy ?? (_brandSectionListCopy = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionListCopy = value; RaisePropertyChanged("BrandSectionListCopy"); }
        }

        public bool Copy { get; set; }

        private string _brandCode;

        public string BrandCode
        {
            get { return _brandCode; }
            set
            {
                _brandCode = value;

                if (_brandCode != null)
                {
                    RaisePropertyChanged("BrandCode");

                    lkpClient.GetTblBrandSectionLinkAsync(BrandCode,  LoggedUserInfo.Iserial);
                    if (BrandCode != null && BrandSection != 0 && _season != 0)
                    {
                        MainRowList.Clear();
                        GetMaindata();
                    }
                }
            }
        }

        private int _brandSection;

        public int BrandSection
        {
            get { return _brandSection; }
            set
            {
                _brandSection = value; RaisePropertyChanged("BrandSection");
                if (BrandCode != null && BrandSection != 0 && _season != 0)
                {
                    MainRowList.Clear();
                    GetMaindata();
                }
            }
        }

        private int _season;

        public int Season
        {
            get { return _season; }
            set
            {
                _season = value; RaisePropertyChanged("Season");
                if (BrandCode != null && BrandSection != 0 && _season != 0)
                {
                    MainRowList.Clear();
                    GetMaindata();
                }
            }
        }

        private string _brandCodeCopy;

        public string BrandCodeCopy
        {
            get { return _brandCodeCopy; }
            set
            {
                _brandCodeCopy = value;

                if (_brandCodeCopy != null)
                {
                    RaisePropertyChanged("BrandCodeCopy");
                    Copy = true;
                    lkpClient.GetTblBrandSectionLinkAsync(BrandCodeCopy,  LoggedUserInfo.Iserial);
                    //if (BrandCodeCopy != null && BrandSectionCopy != 0 && _season != 0)
                    //{
                    //    MainRowList.Clear();
                    //    GetMaindata();
                    //}
                }
            }
        }

        private int _brandSectionCopy;

        public int BrandSectionCopy
        {
            get { return _brandSectionCopy; }
            set
            {
                _brandSectionCopy = value; RaisePropertyChanged("BrandSectionCopy");
                //if (BrandCodeCopy != null && BrandSectionCopy != 0 && _season != 0)
                //{
                //    MainRowList.Clear();
                //    GetMaindata();
                //}
            }
        }

        private int _seasonCopy;

        public int SeasonCopy
        {
            get { return _seasonCopy; }
            set
            {
                _seasonCopy = value; RaisePropertyChanged("SeasonCopy");
            }
        }

        private SortableCollectionView<TblColor> _colorsList;

        public SortableCollectionView<TblColor> ColorsList
        {
            get { return _colorsList ?? (_colorsList = new SortableCollectionView<TblColor>()); }
            set { _colorsList = value; RaisePropertyChanged("ColorsList"); }
        }

        private ObservableCollection<TblColor> _mainSelectedRows;

        public ObservableCollection<TblColor> MainSelectedRows
        {
            get { return _mainSelectedRows ?? (_mainSelectedRows = new ObservableCollection<TblColor>()); }
            set { _mainSelectedRows = value; RaisePropertyChanged("MainSelectedRows"); }
        }

        private ObservableCollection<TblColor> _updatedPantonRows;

        public ObservableCollection<TblColor> UpdatedPantonRows
        {
            get { return _updatedPantonRows ?? (_updatedPantonRows = new ObservableCollection<TblColor>()); }
            set { _updatedPantonRows = value; RaisePropertyChanged("UpdatedPantonRows"); }
        }


        private SortableCollectionView<TblColor> _colorList;

        public SortableCollectionView<TblColor> MainRowList
        {
            get { return _colorList ?? (_colorList = new SortableCollectionView<TblColor>()); }
            set { _colorList = value; RaisePropertyChanged("MainRowList"); }
        }


        private ObservableCollection<TblColor> _selectedMainRows;

        public ObservableCollection<TblColor> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblColor>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        #endregion Prop
    }

}