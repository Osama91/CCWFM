using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls.Search
{
    public class SearchPurchaseOrdersViewModel : ViewModelBase
    {
        public SearchPurchaseOrdersViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<CRUD_ManagerServicePurchaseOrderDto>();
                SelectedMainRow = new CRUD_ManagerServicePurchaseOrderDto();

                Client.GetPurchaseOrderJournalsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                };
            }
        }
    

        public void GetMaindata()
        {
            SortBy = "it.DATAAREAID";
            Loading = true;
            if (Type == 2)
            {
                if (Filter != null)
                {
                    Filter = Filter.Replace("it.PURCHID", "it.JOURNALID");
                }
            }
            if (Type == 1)
            {
                if (Filter != null)
                {
                    Filter = Filter.Replace("it.PURCHID", "it.Docplan");
                }
            }
            Client.GetPurchaseOrderJournalsAsync(MainRowList.Count, PageSize, "ccm", SortBy, Filter, ValuesObjects, Type);
        }

        #region Prop

        private int _tblJournalAccountType;

        public int Type
        {
            get { return _tblJournalAccountType; }
            set
            {
                _tblJournalAccountType = value;
                RaisePropertyChanged("Type");
            }
        }

        private SortableCollectionView<CRUD_ManagerServicePurchaseOrderDto> _mainRowList;

        public SortableCollectionView<CRUD_ManagerServicePurchaseOrderDto> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private CRUD_ManagerServicePurchaseOrderDto _selectedMainRow;

        public CRUD_ManagerServicePurchaseOrderDto SelectedMainRow
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