using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;

namespace CCWFM.UserControls.Search
{
    public class SearchSalesOrderViewModel : ViewModelBase
    {
        public SearchSalesOrderViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new ObservableCollection<TblSalesOrder>();
                SelectedMainRow = new TblSalesOrder();
                Client.GetTblSalesOrderOperationCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                        }
                    }
                };
                Client.SearchSalesOrderCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            //var newrow = new TblSalesOrder();

                            //newrow.InjectFrom(row);
                            if (!MainRowList.Contains(row))
                            {
                                //var fabric = sv.mainFabricList.FirstOrDefault(x => x.Iserial == newrow.tbl_FabricAttriputes);

                                //if (fabric != null)
                                //{
                                //    newrow.FabricPerRow = fabric;
                                //}
                                //newrow.SeasonPerRow = new GenericTable();
                                //newrow.SizeGroupPerRow = new TblSizeGroup();

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
            Client.SearchSalesOrderAsync(MainRowList.Count, PageSize, 2, Status, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private ObservableCollection<TblSalesOrder> _mainRowList;

        public ObservableCollection<TblSalesOrder> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private int _Status;

        public int Status
        {
            get { return _Status; }
            set { _Status = value; RaisePropertyChanged("Status"); }
        }

        private TblSalesOrder _selectedMainRow;

        public TblSalesOrder SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop

        public void Operation(int iserial)
        {
        }
    }
}