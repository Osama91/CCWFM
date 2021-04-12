using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchRetailSeasonViewModel : ViewModelBase
    {
        public SearchRetailSeasonViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblSeason>();
                SelectedMainRow = new TblSeason();

                Client.GetTblRetailSeasonCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblRetailSeasonAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<TblSeason> _mainRowList;

        public SortableCollectionView<TblSeason> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblSeason _selectedMainRow;

        public TblSeason SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop
    }
}