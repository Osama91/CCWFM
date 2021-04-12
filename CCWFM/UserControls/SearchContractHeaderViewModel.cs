using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;

namespace CCWFM.UserControls
{
    public class SearchContractHeaderViewModel : ViewModelBase
    {
        public int TblJournalAccountType { get; set; }
        public int EntityAccount { get; set; }

        public SearchContractHeaderViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new ObservableCollection<TblContractHeader>();
                SelectedMainRow = new TblContractHeader();
                
                Glclient.GetTblContractHeaderForChequeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result.OrderByDescending(w=>w.Iserial))
                    {
                        MainRowList.Add(row);
                    }                 
                    Loading = false;
                    
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblContractHeaderForChequeAsync(TblJournalAccountType,EntityAccount,
                LoggedUserInfo.DatabasEname);
        }        

        #region Prop

        private ObservableCollection<TblContractHeader> _mainRowList;

        public ObservableCollection<TblContractHeader> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblContractHeader> _selectedMainRows;

        public ObservableCollection<TblContractHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblContractHeader>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }
        private TblContractHeader _selectedMainRow;

        public TblContractHeader SelectedMainRow
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