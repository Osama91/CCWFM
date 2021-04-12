using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class RFQFollowUpViewModel : ViewModelBase
    {
        #region [ Private fields ]

        private DateTime? _actualDelivaryDate;
        private DateTime? _estDelivaryDate;
        private string _notes;
        private string _sizeCode;
        private _Proxy.TblSize _sizeProperty;
        private ObservableCollection<_Proxy.SizesWithGroups> _sizesCollection;
        private int? _statusID;
        private string _style;
        private int? _transID;
        #endregion [ Private fields ]

        #region [ Events ]

        public event EventHandler DeleteFollowup;

        public event EventHandler ResampleAction;

        public virtual void OnDeleteFollowup()
        {
            var handler = DeleteFollowup;
            if (handler == null) return;
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.Cancel) return;
            handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Public properties ]

        private bool _isApproved;

        private int? _Iserial;

        private bool _isRejected;

        private ObjectStatus _objStatus;

        private ObservableCollection<GenericViewModel> _statusList;

        private GenericViewModel _statusProp;

        public DateTime? ActualDelivaryDate
        {
            get { return _actualDelivaryDate; }
            set { _actualDelivaryDate = value; RaisePropertyChanged("ActualDelivaryDate"); }
        }

        public DateTime? EstimatedDelivaryDate
        {
            get { return _estDelivaryDate; }
            set { _estDelivaryDate = value; RaisePropertyChanged("EstimatedDelivaryDate"); }
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

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set { _objStatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        public int? ParentID
        {
            get { return _transID; }
            set { _transID = value; RaisePropertyChanged("TransID"); }
        }

        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _sizeCode = value; RaisePropertyChanged("SizeCode");
                }
            }
        }

        public _Proxy.TblSize SizeProperty
        {
            get { return _sizeProperty; }
            set { _sizeProperty = value; RaisePropertyChanged("SizeProperty"); }
        }

        public ObservableCollection<_Proxy.SizesWithGroups> SizesCollection
        {
            get { return _sizesCollection; }
            set { _sizesCollection = value; RaisePropertyChanged("SizesCollection"); }
        }

        public int? StatusID
        {
            get { return _statusID; }
            set { _statusID = value; RaisePropertyChanged("StatusID"); }
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
                _statusProp = value; RaisePropertyChanged("StatusProp");
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

        #endregion [ Public properties ]

        #region [ Constructor(S) ]

        public RFQFollowUpViewModel()
        {
            ObjStatus = new ObjectStatus { IsNew = true };
            InitializeDates();
            InitializeCollections();
            InitializeCommands();
            StatusID = 4;
        }

        #endregion [ Constructor(S) ]

        #region [ Initiators ]

        private void InitializeCollections()
        {
            SizesCollection = new ObservableCollection<_Proxy.SizesWithGroups>();
            SizesCollection.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (_Proxy.SizesWithGroups item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (_Proxy.SizesWithGroups item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

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
        }

        private void InitializeCommands()
        {
        }

        private void InitializeDates()
        {
            ActualDelivaryDate = DateTime.Now;
            EstimatedDelivaryDate = DateTime.Now;
        }

        #endregion [ Initiators ]

        #region [ Commands ]

        private CommandsExecuter _deleteFollowupCommand;

        public CommandsExecuter DeleteFollowupCommand
        {
            get { return _deleteFollowupCommand ?? (_deleteFollowupCommand = new CommandsExecuter(Delete) { IsEnabled = true }); }
        }

        #endregion [ Commands ]

        #region [ Commands bound method ]

        private void Delete()
        {
            OnDeleteFollowup();
        }

        #endregion [ Commands bound method ]
    }
}