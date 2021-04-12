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
    public class SearchChequeViewModel : ViewModelBase
    {
        public int Bank { get; set; }

        public SearchChequeViewModel()
        {
            if (!IsDesignTime)
            {
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblBankCheque>();
                SelectedMainRow = new TblBankCheque();
                Glclient.CreateChequeCompleted += (s, sv) => GetMaindata();
                Glclient.GetTblBankChequeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }
                    if (Bank != 0 && ValuesObjects != null && (!MainRowList.Any() && ValuesObjects.ContainsKey("Cheque0")))
                    {
                        var res = MessageBox.Show("This Cheque is Not Found Do You Want To Create It ?", "Create Cheque",
                   MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            Glclient.CreateChequeAsync(Bank, (long)ValuesObjects.FirstOrDefault(w => w.Key == "Cheque0").Value, 1, LoggedUserInfo.DatabasEname);
                        }
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblBankChequeAsync(MainRowList.Count, PageSize, Bank, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname, ChequeStatusPerRow.Iserial);
        }

        public GenericTable ChequeStatusPerRow { get; set; }

        #region Prop

        private SortableCollectionView<TblBankCheque> _mainRowList;

        public SortableCollectionView<TblBankCheque> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblBankCheque> _selectedMainRows;

        public ObservableCollection<TblBankCheque> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBankCheque>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }
        private TblBankCheque _selectedMainRow;

        public TblBankCheque SelectedMainRow
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