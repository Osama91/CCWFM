using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchGroupsViewModel : ViewModelBase
    {
        public SearchGroupsViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblGROUP1>();
                SelectedMainRow = new TblGROUP1();

                Client.GetRetailGroupsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                };

                //GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetRetailGroupsAsync(MainRowList.Count, PageSize, TableName, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<TblGROUP1> _mainRowList;

        public SortableCollectionView<TblGROUP1> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        public string TableName { get; set; }

        private TblGROUP1 _selectedMainRow;

        public TblGROUP1 SelectedMainRow
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