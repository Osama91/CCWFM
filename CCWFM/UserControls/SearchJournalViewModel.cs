using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchJournalViewModel : ViewModelBase
    {
        public SearchJournalViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblJournal>();
                SelectedMainRow = new TblJournal();

                Glclient.GetTblJournalCompleted += (s, sv) =>
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
            Glclient.GetTblJournalAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
        }

        #region Prop

        private SortableCollectionView<TblJournal> _mainRowList;

        public SortableCollectionView<TblJournal> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblJournal> _selectedMainRows;

        public ObservableCollection<TblJournal> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblJournal>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblJournal _selectedMainRow;

        public TblJournal SelectedMainRow
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