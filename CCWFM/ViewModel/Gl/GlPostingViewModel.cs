using System;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;

namespace CCWFM.ViewModel.Gl
{
    public class GlPostingViewModel : ViewModelBase
    {
        public GlPostingViewModel()
        {
            
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.GlPosting.ToString());
                Glclient = new GlServiceClient();
                Glclient.GetPostingTimesAsync(LoggedUserInfo.DatabasEname);
                Glclient.GetPostingTimesCompleted += (s, sv) =>
                {
                    AdjustDate = sv.adjustDate;
                    TransferDate = sv.transferDate;
                    SalesDate = sv.Result;
                    ExpensesDate =sv.expensesDate;
                    DepreciationDate = sv.depreciationDate;
                    CommissionDate = sv.commissionDate;
                };
                Glclient.CalcCostCenterCompleted += (s, sv) =>
                {
                    Loading = false;

                    if (sv.Error==null)
                    {
                        Count = sv.Result;
                        if (Count != 0)
                        {
                            Glclient.CalcCostCenterAsync(LoggedUserInfo.DatabasEname);
                        }
                        if (Count == 0)
                        {
                            MessageBox.Show("Done");
                        }
                    }
                    else
                    {
                        MessageBox.Show(sv.Error.Message);

                    }
                
                };
                    Glclient.GlPostCompleted += (s, sv) =>
                {
                    Loading = false;
                    if (sv.Error == null)
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
                                Loading = true;
                                Glclient.GlPostAsync(FromDate, ToDate, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname,
                                    Sales, Transfer, Adjustment, Expenses, Depreciation, Commission, true, CostCenter);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(sv.Error.Message);

                    }
                };
                FromDate = DateTime.Now;
                ToDate = DateTime.Now;
                Sales  = true;
            }
        }
        #region Prop
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

        private DateTime? _depreciationDate;

        public DateTime? DepreciationDate
        {
            get { return _depreciationDate; }
            set { _depreciationDate = value; RaisePropertyChanged("depreciationDate"); }
        }

        private DateTime? _CommissionDate;

        public DateTime? CommissionDate
        {
            get { return _CommissionDate; }
            set { _CommissionDate = value; RaisePropertyChanged("CommissionDate"); }
        }


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

        private bool _depreciation;
        public bool Depreciation
        {
            get { return _depreciation; }
            set { _depreciation = value; RaisePropertyChanged("Depreciation"); }
        }
        private bool _CostCenter;
        public bool CostCenter
        {
            get { return _CostCenter; }
            set { _CostCenter = value; RaisePropertyChanged("CostCenter"); }
        }

        private bool _Commission;

        public bool Commission
        {
            get { return _Commission; }
            set { _Commission = value; RaisePropertyChanged("Commission"); }
        }
        private int _count;

        public int Count
        {
            get { return _count; }
            set { _count = value; RaisePropertyChanged("Count"); }
        }



        #endregion Prop




        public void Post()
        {
            Loading = true;
            if (CostCenter)
            {
                Glclient.CalcCostCenterAsync(LoggedUserInfo.DatabasEname);
            }

                

            Glclient.GlPostAsync(FromDate, ToDate, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, Sales, Transfer, Adjustment, Expenses, Depreciation,Commission, false, CostCenter);
        }
    }
}