using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;

namespace CCWFM.UserControls.Search
{
    public class SearchSalesOrderColorViewModel : ViewModelBase
    {
        public SearchSalesOrderColorViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new ObservableCollection<TblSalesOrderColor>();
                SelectedMainRow = new TblSalesOrderColor();

                Client.SearchSalesOrderColorCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            if (!MainRowList.Contains(row))
                            {
                                MainRowList.Add(row);
                            }
                        }
                    }

                    Loading = false;
                    //   FullCount = sv.fullCount;
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.SearchSalesOrderColorAsync(MainRowList.Count, PageSize, 2, 1, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private ObservableCollection<TblSalesOrderColor> _mainRowList;

        public ObservableCollection<TblSalesOrderColor> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblSalesOrderColor _selectedMainRow;

        public TblSalesOrderColor SelectedMainRow
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