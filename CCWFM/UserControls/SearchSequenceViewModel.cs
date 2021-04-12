using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchSequenceViewModel : ViewModelBase
    {
        public SearchSequenceViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblSequence>();
                SelectedMainRow = new TblSequence();

                Glclient.GetTblSequenceCompleted += (s, sv) =>
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
            Glclient.GetTblSequenceAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private SortableCollectionView<TblSequence> _mainRowList;

        public SortableCollectionView<TblSequence> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblSequence> _selectedMainRows;

        public ObservableCollection<TblSequence> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSequence>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblSequence _selectedMainRow;

        public TblSequence SelectedMainRow
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