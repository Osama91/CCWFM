using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchCostCenterViewModel : ViewModelBase
    {
        public SearchCostCenterViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblCostCenter>();
                SelectedMainRow = new TblCostCenter();

                Glclient.GetTblCostCenterCompleted += (s, sv) =>
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
            Glclient.GetTblCostCenterAsync(MainRowList.Count, PageSize, CostCenterType, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial, EntityAccount, EntityAccountType);
        }

        #region Prop

        private SortableCollectionView<TblCostCenter> _mainRowList;

        public SortableCollectionView<TblCostCenter> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblCostCenter> _selectedMainRows;

        public ObservableCollection<TblCostCenter> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostCenter>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblCostCenter _selectedMainRow;

        public TblCostCenter SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private int? _costCenterType;

        public int? CostCenterType
        {
            get { return _costCenterType; }
            set
            {
                _costCenterType = value;
                RaisePropertyChanged("CostCenterType");
            }
        }

        private int _entityAccount;

        public int EntityAccount
        {
            get { return _entityAccount; }
            set
            {
                _entityAccount = value;
                RaisePropertyChanged("EntityAccount");
            }
        }

        private int _journalAccountType;

        public int JournalAccountType
        {
            get { return _journalAccountType; }
            set
            {
                _journalAccountType = value;
                RaisePropertyChanged("JournalAccountType");
            }
        }

        private int _entityAccountType;

        public int EntityAccountType
        {
            get { return _entityAccountType; }
            set
            {
                _entityAccountType = value;
                RaisePropertyChanged("EntityAccountType");
            }
        }

        #endregion Prop
    }
}