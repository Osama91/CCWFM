using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.RequestForQutation;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class RFQCostFollowUp : ViewModelBase
    {
        #region [ Private Fields ]

        private DateTime? _actualDelivaryDate;
        private decimal? _cost;
        private string _currency;
        private DateTime? _estimatedDelivaryDate;
        private decimal? _exchangeRate;
        private bool _isApproved;
        private int? _Iserial;
        private bool _isRejected;
        private decimal? _localCost;
        private string _notes;

        private int? _parentID;
        private int? _status;
        private ObservableCollection<GenericViewModel> _statusList;
        private GenericViewModel _statusProp;
        private string _style;
        #endregion [ Private Fields ]

        #region [ Events ]

        public event EventHandler DeleteCostingFollowup;

        public event EventHandler ResampleAction;

        public virtual void OnDeleteCostingFollowup()
        {
            var handler = DeleteCostingFollowup;
            if (handler == null) return;
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.Cancel) return;
            handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Public Properties ]

        private decimal? _additionalCost;
        private ObservableCollection<tbl_RFQ_AdditionalCost> _additionalCostList;
        private List<GenericViewModel> _currencies;

        private RFQSubHeader _parent;

        public DateTime? ActualDelivaryDate
        {
            get { return _actualDelivaryDate; }
            set { _actualDelivaryDate = value; RaisePropertyChanged("ActualDelivaryDate"); }
        }

        public decimal? AdditionalCost
        {
            get { return _additionalCost; }
            set { _additionalCost = value; RaisePropertyChanged("AdditionalCost"); }
        }

        public ObservableCollection<tbl_RFQ_AdditionalCost> AdditionalCostList
        {
            get { return _additionalCostList ?? (_additionalCostList = new ObservableCollection<CRUDManagerService.tbl_RFQ_AdditionalCost>()); }
            set { _additionalCostList = value; RaisePropertyChanged("AdditionalCostList"); }
        }

        public ObservableCollection<TblColor> ColorsList
        {
            get { return _colorsList; }
            set
            {
                _colorsList = value;
                RaisePropertyChanged("ColorsList");
            }
        }

        public decimal? Cost
        {
            get { return _cost; }
            set
            {
                if (_cost == value || value == null) return;
                _cost = value;
                LocalCost = decimal.Parse(((ExchangeRate ?? 0) * value).ToString());
                RaisePropertyChanged("Cost");
            }
        }

        public List<GenericViewModel> Currencies
        {
            get { return _currencies ?? (_currencies = new List<GenericViewModel>(RFQGlobalLkps.CurrenciesList)); }
            set { _currencies = value; RaisePropertyChanged("Currencies"); }
        }

        public string Currency
        {
            get { return _currency; }
            set { _currency = value; RaisePropertyChanged("Currency"); }
        }

        public DateTime? EstimatedDelivaryDate
        {
            get { return _estimatedDelivaryDate; }
            set { _estimatedDelivaryDate = value; RaisePropertyChanged("EstimatedDelivaryDate"); }
        }

        public decimal? ExchangeRate
        {
            get { return _exchangeRate; }
            set
            {
                if (_exchangeRate != value || value != null)
                {
                    _exchangeRate = value;
                    LocalCost = decimal.Parse(((Cost ?? 0) * value).ToString());
                    RaisePropertyChanged("ExchangeRate");
                }
            }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
            set
            {
                _isApproved = value;
                RaisePropertyChanged("IsApproved");
            }
        }

        public int? Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        public bool IsRejected
        {
            get { return _isRejected; }
            set { _isRejected = value; RaisePropertyChanged("IsRejected"); }
        }

        public decimal? LocalCost
        {
            get { return _localCost; }
            set
            {
                if (_localCost == value) return;
                _localCost = value; RaisePropertyChanged("LocalCost");
            }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        public RFQSubHeader Parent
        {
            get { return _parent; }
            set { _parent = value; RaisePropertyChanged("Parent"); }
        }

        public int? ParentID
        {
            get { return _parentID; }
            set { _parentID = value; RaisePropertyChanged("ParentID"); }
        }

        public int? Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        public ObservableCollection<GenericViewModel> StatusList
        {
            get { return _statusList; }
            set { _statusList = value; RaisePropertyChanged("StatusList"); }
        }

        public GenericViewModel StatusProp
        {
            get { return _statusProp; }
            set
            {
                _statusProp = value;
                RaisePropertyChanged("StatusProp");
                switch (value.Iserial)
                {
                    case 1:
                        IsRejected = true;
                        IsApproved = false;
                        break;

                    case 3:
                        if (ResampleAction != null)
                        {
                            ResampleAction(this, new EventArgs());
                        }
                        break;

                    case 2:
                        IsRejected = false;
                        IsApproved = true;
                        break;
                }
            }
        }

        public string Style
        {
            get { return _style; }
            set { _style = value; RaisePropertyChanged("Style"); }
        }

        public string StyleColor
        {
            get { return _styleColor; }
            set
            {
                if (_styleColor == value) return;

                _styleColor = value;
                RaisePropertyChanged("SalesOrderColor");
            }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public RFQCostFollowUp()
        {
            InitializeObject();
            initializeCommands();
            InitializeDates();
            Status = 4;
        }

        #endregion [ Constructor(s) ]

        #region [ Commands ]

        private ObservableCollection<TblColor> _colorsList;
        private CommandsExecuter _deleteCostFollowup;

        private CommandsExecuter _showCostingDetailsCommand;
        private string _styleColor;

        public CommandsExecuter DeleteCostFollowup
        {
            get { return _deleteCostFollowup ?? (_deleteCostFollowup = new CommandsExecuter(Delete) { IsEnabled = true }); }
        }

        public CommandsExecuter ShowCostingDetailsCommand
        {
            get { return _showCostingDetailsCommand ?? (_showCostingDetailsCommand = new CommandsExecuter(ShowCostingDetails) { IsEnabled = true }); }
        }

        #endregion [ Commands ]

        #region [ Initialization ]

        private void initializeCommands()
        {
        }

        private void InitializeDates()
        {
            ActualDelivaryDate = DateTime.Now;
            EstimatedDelivaryDate = DateTime.Now;
        }

        private void InitializeObject()
        {
            StatusList = new ObservableCollection<GenericViewModel>();
            StatusList.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach (GenericViewModel item in e.NewItems)
                        {
                            item.PropertyChanged
                                += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                        }

                    if (e.OldItems != null)
                        foreach (GenericViewModel item in e.OldItems)
                        {
                            item.PropertyChanged
                                -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                        }
                };

            foreach (var item in RFQGlobalLkps.FollowupStatusesList)
            {
                StatusList.Add(item);
            }
            ColorsList = new ObservableCollection<TblColor>();
            ColorsList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (TblColor item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (TblColor item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            foreach (var item in RFQGlobalLkps.ColorsList)
            {
                ColorsList.Add(item);
            }
        }

        #endregion [ Initialization ]

        #region [ Command's bound methods ]

        private void Delete()
        {
            OnDeleteCostingFollowup();
        }

        private void ShowCostingDetails()
        {
            var temp = new RFQCostingChild(AdditionalCostList ?? (new ObservableCollection<tbl_RFQ_AdditionalCost>()));
            temp.SubmitCosts += (s, e) =>
                {
                    AdditionalCostList = e.AdditionalCostList;
                    AdditionalCost = Parent.Qty == 0 ? 0 : (e.TotalocalValue / Parent.Qty);
                };
            temp.Show();
        }

        #endregion [ Command's bound methods ]
    }
}