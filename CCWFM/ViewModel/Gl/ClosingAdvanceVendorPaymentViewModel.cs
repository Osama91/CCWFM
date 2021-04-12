using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using GenericTable = CCWFM.GlService.GenericTable;
using TblBankCheque = CCWFM.GlService.TblBankCheque;
using System.Linq;

namespace CCWFM.ViewModel.Gl
{
    public class ClosingAdvanceVendorPaymentViewModel : ViewModelBase
    {
        public ClosingAdvanceVendorPaymentViewModel()
        {
            if (!IsDesignTime)
            {
                PostDate = DateTime.Now;
                TblJournalAccountTypePerRow = new GenericTable() { Iserial = 2 };
                Glclient = new GlServiceClient();
                MainRowList = new ObservableCollection<TblGlChequeTransactionDetailViewModel>();
                Glclient.GetTblChequeTransactionDetailNotLinkedCompleted += (s, sv) =>
                    {
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblGlChequeTransactionDetailViewModel();
                            newrow.InjectFrom(row);
                            newrow.ChequePerRow = new TblBankCheque();

                            if (row.TblBankCheque1 != null) newrow.ChequePerRow.InjectFrom(row.TblBankCheque1);
                            newrow.TblJournalAccountTypePerRow = new GenericTable();
                            if (row.TblJournalAccountType != null)
                                newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                            newrow.TblJournalAccountType1PerRow = new GenericTable();
                            if (row.TblJournalAccountType1 != null)
                                newrow.TblJournalAccountType1PerRow.InjectFrom(row.TblJournalAccountType1);

                            newrow.EntityDetail1TblJournalAccountType = row.EntityDetail1TblJournalAccountType;
                            newrow.EntityDetail2TblJournalAccountType = row.EntityDetail2TblJournalAccountType;
                            newrow.Saved = true;
                            MainRowList.Add(newrow);
                        }
                        Loading = false;
                    };
                Glclient.UpdateorInsertClosingAdvanceVendorPaymentsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.Clear();

                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };

            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc";
            Loading = true;
            Glclient.GetTblChequeTransactionDetailNotLinkedAsync(Vendor, LoggedUserInfo.DatabasEname);
        }


        public void SaveMainRow()
        {

            Loading = true;
            var observableCollectionIserials = new ObservableCollection<int>();
            foreach (var item in MainRowList.Where(w => w.Saved==true).Select(w => w.Iserial).ToList())
            {
                observableCollectionIserials.Add(item);
            }
            Glclient.UpdateorInsertClosingAdvanceVendorPaymentsAsync(PostDate,observableCollectionIserials, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
        }

        #region Prop
        private string _Vendor;

        public string Vendor
        {
            get { return _Vendor; }
            set { _Vendor = value; RaisePropertyChanged("Vendor"); }
        }

        private DateTime _PostDate;

        public DateTime PostDate
        {
            get { return _PostDate; }
            set { _PostDate = value; RaisePropertyChanged("PostDate"); }
        }

        private ObservableCollection<TblGlChequeTransactionDetailViewModel> _MainRowList;

        public ObservableCollection<TblGlChequeTransactionDetailViewModel> MainRowList
        {
            get { return _MainRowList ?? (_MainRowList = new ObservableCollection<TblGlChequeTransactionDetailViewModel>()); }
            set { _MainRowList = value; RaisePropertyChanged("MainRowList"); }
        }
        private GenericTable _TblJournalAccountTypePerRow;

        public GenericTable TblJournalAccountTypePerRow
        {
            get { return _TblJournalAccountTypePerRow ?? (_TblJournalAccountTypePerRow = new GenericTable()); }
            set { _TblJournalAccountTypePerRow = value; RaisePropertyChanged("TblJournalAccountTypePerRow"); }
        }

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                _entityPerRow = value; RaisePropertyChanged("EntityPerRow");
                if (EntityPerRow != null) Vendor = EntityPerRow.Code;
            }
        }


        #endregion Prop
    }

}