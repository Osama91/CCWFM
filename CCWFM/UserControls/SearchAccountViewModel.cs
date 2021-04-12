using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchAccountViewModel : ViewModelBase
    {
        public bool ChildOnly;

        public SearchAccountViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblAccount>();
                SelectedMainRow = new TblAccount();

                Glclient.GetTblAccountCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblAccountAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, ChildOnly);
        }

        #region Prop

        private SortableCollectionView<TblAccount> _mainRowList;

        public SortableCollectionView<TblAccount> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblAccount> _selectedMainRows;

        public ObservableCollection<TblAccount> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAccount>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblAccount _selectedMainRow;

        public TblAccount SelectedMainRow
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