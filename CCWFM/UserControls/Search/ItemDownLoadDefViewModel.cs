using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class ItemDownLoadDefViewModel : ViewModelBase
    {
        public ItemDownLoadDefViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblItemDownLoadDef>();
                SelectedMainRow = new TblItemDownLoadDef();

            

                Client.GetBrandsCompleted += (s, sv) =>
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

            Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
            Client.GetBrandsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private SortableCollectionView<TblItemDownLoadDef> _mainRowList;

        public SortableCollectionView<TblItemDownLoadDef> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblItemDownLoadDef _selectedMainRow;

        public TblItemDownLoadDef SelectedMainRow
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