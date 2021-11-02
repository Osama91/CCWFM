using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchBanksViewModel : ViewModelBase
    {
        public SearchBanksViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<TblBank>();
                SelectedMainRow = new TblBank();
                
                Client.GetBanksCompleted += (s, sv) =>
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
                SortBy = "it.Organization";
            Loading = true;
            string organization = null;
            if (LoggedUserInfo.Company.Code == "CR")
            {
                organization = "CR";
            }
            else if (LoggedUserInfo.Company.Code == "CCNEW")
            {
                organization = "CR";
            }
            else if (LoggedUserInfo.Company.Code == "SW")
            {
                organization = "SW";
            }

            Client.GetBanksAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private SortableCollectionView<TblBank> _mainRowList;

        public SortableCollectionView<TblBank> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblBank _selectedMainRow;

        public TblBank SelectedMainRow
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