using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchStoresViewModel : ViewModelBase
    {
        public SearchStoresViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<StoreForAllCompany>();
                SelectedMainRow = new StoreForAllCompany();

                Client.GetStoresForAllCompanyCompleted += (s, sv) =>
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

            Client.GetStoresForAllCompanyAsync(MainRowList.Count, PageSize, organization, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<StoreForAllCompany> _mainRowList;

        public SortableCollectionView<StoreForAllCompany> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private StoreForAllCompany _selectedMainRow;

        public StoreForAllCompany SelectedMainRow
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