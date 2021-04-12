using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchRetailStyleViewModel : ViewModelBase
    {
        public SearchRetailStyleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<viewstyle>();
                SelectedMainRow = new viewstyle();


                Client.GetviewstyleCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    
                };

            }
        }


        public void GetMaindata(int brand)
        {
            if (SortBy == null)
                SortBy = "it.Code";
            Loading = true;
            Client.GetviewstyleAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,LoggedUserInfo.DatabasEname,brand);
        }

        #region Prop

        private SortableCollectionView<viewstyle> _mainRowList;

        public SortableCollectionView<viewstyle> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private viewstyle _selectedMainRow;

        public viewstyle SelectedMainRow
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