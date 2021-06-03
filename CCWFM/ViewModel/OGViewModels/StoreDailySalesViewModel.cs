using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.LkpData;

namespace CCWFM.ViewModel.OGViewModels
{
    public class CalliopeDailySales  
    {
        public CalliopeDailySales()
        {
            //LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
            //lkpClient.GetUserDefaultStoreAsync(LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, (int)LoggedUserInfo.ActiveStore);
            //lkpClient.GetUserDefaultStoreCompleted += (s, sv) =>
            //{
            //    TblStore = sv.Result.iserial;
            //    StoreName = sv.Result.ENAME;
            //};
        }

        private int _Iserial;
        public int Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; }
        }

        private int _tblStore;
        public int TblStore
        {
            get { return _tblStore; }
            set { _tblStore = value; }
        }
        private int _qty;
        public int Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }


        private decimal _salesAmount;
        public decimal SalesAmount
        {
            get { return _salesAmount; }
            set { _salesAmount = value; }
        }

        private string _storeName;
        public string StoreName
        {
            get { return _storeName; }
            set { _storeName = value; }

        }

        private DateTime? _transDate;
        public DateTime? TransDate
        {
            get { return _transDate ?? DateTime.Now; }
            set { _transDate = value; }

        }

    }

    public class StoreDailySalesViewModel : ViewModelBase
    {
        private string _activeStoreName;
        public string ActiveStoreName
        {
            get { return _activeStoreName; }
            set { _activeStoreName = value; }
        }
         
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        private CalliopeDailySales _selectedMainRow;
        public CalliopeDailySales SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new CalliopeDailySales());
               
            }
            set
            {
                _selectedMainRow = value;
            }
        }

        public StoreDailySalesViewModel()
        {
            GetUserDefaultStore();
        }

        private void GetUserDefaultStore()
        {
            LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
            lkpClient.GetUserDefaultStoreAsync(LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, LoggedUserInfo.AllowedStores.First());
            lkpClient.GetUserDefaultStoreCompleted += (s, sv) =>
            {
                ActiveStoreName = sv.Result.ENAME.ToString();
                SelectedMainRow.TblStore = sv.Result.iserial;
                SelectedMainRow.StoreName = sv.Result.ENAME;
            };
        }

        internal void SaveMainRow()
        {

            if(SelectedMainRow.SalesAmount > 0 && SelectedMainRow.Qty > 0)
            {
                tblCalliopeStoresDailySale newRow = new tblCalliopeStoresDailySale()
                {
                    SalesAmount = SelectedMainRow.SalesAmount,
                    TblStore = SelectedMainRow.TblStore,
                    TransDate = (DateTime)SelectedMainRow.TransDate,
                    Qty = SelectedMainRow.Qty,
                };
                lkpClient.SaveStoreDailySalesAsync(LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, newRow);
                lkpClient.SaveStoreDailySalesCompleted += (s, sv) =>
                {
                    if (sv.Result != null && sv.Result.Iserial > 0)
                    {
                        MessageBox.Show("Data Saved Successfully");
                    }
                };
            }else
            {
                MessageBox.Show("Invalid Qty or Amount ");
            }


        }
    }
}
