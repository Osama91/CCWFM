using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class BankDepositViewModel : ViewModelBase
    {
        public BankDepositViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.BankDepositApproval.ToString());

                Client.GetTblBankDepositCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBankDepositViewModel();

                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };
            }
        }

        public void GetMainData()
        {
            Client.GetTblBankDepositAsync(0, int.MaxValue, StorePerRow.iserial, null, null, null, LoggedUserInfo.DatabasEname);
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var saveRow = new TblBankDeposit();

                saveRow.InjectFrom(SelectedMainRow);
                if (saveRow.Status != null) Client.ApproveBankDepositAsync(saveRow, (int)saveRow.Status, LoggedUserInfo.DatabasEname);
            }
        }

        #region Prop

        private TblBankDepositViewModel _selectedMainRow;

        public TblBankDepositViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblBankDepositViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow; }
            set
            {
                _storePerRow = value; RaisePropertyChanged("StorePerRow");
                GetMainData();
            }
        }

        private ObservableCollection<TblBankDepositViewModel> _mainRowList;

        public ObservableCollection<TblBankDepositViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new ObservableCollection<TblBankDepositViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        #endregion Prop

        public void Report()
        {
            const string reportName = "BankDepositReport";

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }
    }

    public class TblBankDepositViewModel : PropertiesViewModelBase
    {
        private int _amountField;

        private string _descriptionField;

        private DateTime _docdateField;

        private DateTime _fromDateField;

        private int _glserialField;

        private string _iserialField;

        private int? _overShortField;

        private int? _statusField;

        private bool _statusBool;

        public bool StatusBool
        {
            get { return _statusBool; }
            set
            {
                _statusBool = value; RaisePropertyChanged("StatusBool");
                if (StatusBool)
                {
                    Status = 1;
                }
                else
                {
                    Status = 0;
                }
            }
        }

        private int _tblStoreField;

        private TblStore _tblStore1Field;

        private int? _tblUserField;

        private DateTime _toDateField;

        public int Amount
        {
            get
            {
                return _amountField;
            }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public DateTime Docdate
        {
            get
            {
                return _docdateField;
            }
            set
            {
                if ((_docdateField.Equals(value) != true))
                {
                    _docdateField = value;
                    RaisePropertyChanged("Docdate");
                }
            }
        }

        public DateTime FromDate
        {
            get
            {
                return _fromDateField;
            }
            set
            {
                if ((_fromDateField.Equals(value) != true))
                {
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public int Glserial
        {
            get
            {
                return _glserialField;
            }
            set
            {
                if ((_glserialField.Equals(value) != true))
                {
                    _glserialField = value;
                    RaisePropertyChanged("Glserial");
                }
            }
        }

        public string Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((ReferenceEquals(_iserialField, value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int? OverShort
        {
            get
            {
                return _overShortField;
            }
            set
            {
                if ((_overShortField.Equals(value) != true))
                {
                    _overShortField = value;
                    RaisePropertyChanged("OverShort");
                }
            }
        }

        public int? Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                if ((_statusField.Equals(value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public int TblStore
        {
            get
            {
                return _tblStoreField;
            }
            set
            {
                if ((_tblStoreField.Equals(value) != true))
                {
                    _tblStoreField = value;
                    RaisePropertyChanged("TblStore");
                }
            }
        }

        public TblStore TblStore1
        {
            get
            {
                return _tblStore1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblStore1Field, value) != true))
                {
                    _tblStore1Field = value;
                    RaisePropertyChanged("TblStore1");
                }
            }
        }

        public int? TblUser
        {
            get
            {
                return _tblUserField;
            }
            set
            {
                if ((_tblUserField.Equals(value) != true))
                {
                    _tblUserField = value;
                    RaisePropertyChanged("TblUser");
                }
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _toDateField;
            }
            set
            {
                if ((_toDateField.Equals(value) != true))
                {
                    _toDateField = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
    }
}