using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchEntityViewModel : ViewModelBase
    {
        public SearchEntityViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<Entity>();
                SelectedMainRow = new Entity();

                    

                Glclient.GetEntityCompleted += (s, sv) =>
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

            if (TblJournalAccountType != null)
            {
                Loading = true;
                Glclient.GetEntityAsync(MainRowList.Count, PageSize, (int)TblJournalAccountType, Scope, SortBy, Filter,
                    ValuesObjects, LoggedUserInfo.DatabasEname,LoggedUserInfo.Iserial, PreventPerRow);
            }
        }

        #region Prop

        private int? _tblJournalAccountType;

        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountType; }
            set
            {
                _tblJournalAccountType = value;
                RaisePropertyChanged("TblJournalAccountType");
            }
        }

        private int _scope;

        public int Scope
        {
            get { return _scope; }
            set { _scope = value; RaisePropertyChanged("Scope"); }
        }

        private bool _preventPerRow;

        public bool PreventPerRow
        {
            get { return _preventPerRow; }
            set { _preventPerRow = value; RaisePropertyChanged("PreventPerRow"); }
        }

        private SortableCollectionView<Entity> _mainRowList;

        public SortableCollectionView<Entity> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<Entity> _selectedMainRows;

        public ObservableCollection<Entity> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<Entity>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private Entity _selectedMainRow;

        public Entity SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        public bool getOnlyWithAccount { get; set; }

        #endregion Prop
    }
}