using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using System.Collections.ObjectModel;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls.Search
{
    public class SearchSupplierViewModel : ViewModelBase
    {
        public SearchSupplierViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new ObservableCollection<TBLsupplier>();
                SelectedMainRow = new TBLsupplier();

                Client.GetTblRetailSupplierCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var axvendor = sv.AxVendors.FirstOrDefault(x => x.vendor_code == row.glcode);
                        if (axvendor != null)
                        {
                            row.Address1 = axvendor.Termsofpayment;
                            row.Address2 = axvendor.Methodofpayment;
                        }
                        else
                        {
                            row.Address1 = row.Address2 = null;
                        }
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
            Client.GetTblRetailSupplierAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private ObservableCollection<TBLsupplier> _mainRowList;

        public ObservableCollection<TBLsupplier> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TBLsupplier _selectedMainRow;

        public TBLsupplier SelectedMainRow
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