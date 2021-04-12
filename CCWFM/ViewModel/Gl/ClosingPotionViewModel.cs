using System;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;

namespace CCWFM.ViewModel.Gl
{
    public class ClosingPostingViewModel : ViewModelBase
    {
        public ClosingPostingViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.ClosingPosting.ToString());
                Glclient = new GlServiceClient();

                Glclient.GetPostingTimesAsync(LoggedUserInfo.DatabasEname);
                Glclient.GetPostingTimesCompleted += (s, sv) =>
                {
                    AdjustDate = sv.adjustDate;
                    TransferDate = sv.transferDate;
                    SalesDate = sv.Result;
                    ExpensesDate =
                    sv.expensesDate;
                };
                Glclient.GlPostCompleted += (s, sv) =>
                {
                    if (sv.Result == 1)
                    {
                        MessageBox.Show("Done");
                    }
                    else
                    {
                        var res =
                            MessageBox.Show("Some Transactions Already Exsists Are You Sure You Want To Override ? ?",
                                "Delete",
                                MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            Glclient.GlPostAsync(FromDate, ToDate, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname,
                                Sales, Transfer, Adjustment, Expenses,Depreciation, Commission, true,CostCenter);
                        }
                    }
                };
                FromDate = DateTime.Now;
                ToDate = DateTime.Now;
                Sales =Commission= true;
            }
        }
        private bool _CostCenter;
        public bool CostCenter
        {
            get { return _CostCenter; }
            set { _CostCenter = value; RaisePropertyChanged("CostCenter"); }
        }

        private string _period;

        public string Period
        {
            get { return _period; }
            set { _period = value; RaisePropertyChanged("Period"); }
        }

        private TblAccount _accountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _accountPerRow; }
            set { _accountPerRow = value; RaisePropertyChanged("AccountPerRow"); }
        }

        private DateTime? _transferDate;

        public DateTime? TransferDate
        {
            get { return _transferDate; }
            set { _transferDate = value; RaisePropertyChanged("TransferDate"); }
        }

        private DateTime? _adjustDate;

        public DateTime? AdjustDate
        {
            get { return _adjustDate; }
            set { _adjustDate = value; RaisePropertyChanged("AdjustDate"); }
        }

        private DateTime? _salesDate;

        public DateTime? SalesDate
        {
            get { return _salesDate; }
            set { _salesDate = value; RaisePropertyChanged("SalesDate"); }
        }

        private DateTime? _expensesDate;

        public DateTime? ExpensesDate
        {
            get { return _expensesDate; }
            set { _expensesDate = value; RaisePropertyChanged("ExpensesDate"); }
        }

        #region Prop

        private DateTime _fromDate;

        public DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChanged("FromDate"); }
        }

        private DateTime _toDate;

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; RaisePropertyChanged("ToDate"); }
        }

        private bool _adjustment;

        public bool Adjustment
        {
            get { return _adjustment; }
            set { _adjustment = value; RaisePropertyChanged("Adjustment"); }
        }

        private bool _expenses;

        public bool Expenses
        {
            get { return _expenses; }
            set { _expenses = value; RaisePropertyChanged("Expenses"); }
        }

        private bool _depreciation;

        public bool Depreciation
        {
            get { return _depreciation; }
            set { _depreciation = value;RaisePropertyChanged("Depreciation");}
        }

        private bool _Commission;

        public bool Commission
        {
            get { return _Commission; }
            set { _Commission = value; RaisePropertyChanged("Commission"); }
        }


        private bool _transfer;

        public bool Transfer
        {
            get { return _transfer; }
            set { _transfer = value; RaisePropertyChanged("Transfer"); }
        }

        private bool _sales;

        public bool Sales
        {
            get { return _sales; }
            set { _sales = value; RaisePropertyChanged("Sales"); }
        }

        #endregion Prop

        public void Post()
        {
            Glclient.GlPostAsync(FromDate, ToDate, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, Sales, Transfer, Adjustment, Expenses, Depreciation, Commission, false,CostCenter);
        }
    }
}