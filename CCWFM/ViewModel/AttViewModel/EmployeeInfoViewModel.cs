using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using System.Windows;

namespace CCWFM.ViewModel.AttViewModel
{
    public class TblEmployeeInfoViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private string _adateField;

        private string _approvalField;

        private string _descriptionField;

        private string _employeeGuidField;

        private string _empnameField;

        private string _enrollNumberField;

        private string _excuseTypeField;

        private string _fromDateField;

        private TimeSpan? _fromTimeField;

        private string _intimeField;

        private int _iserialField;

        private string _managerEmailField;

        private string _managerGuidField;

        private string _missionTypeField;

        private string _reasonField;

        private string _statusField;

        private TimeSpan? _toTimeField;

        private string _vacationTypeField;

        private string _outtimeField;

        private string _toDateField;

        [ReadOnly(true)]
        public string Adate
        {
            get
            {
                return _adateField;
            }
            set
            {
                if ((ReferenceEquals(_adateField, value) != true))
                {
                    _adateField = value;
                    RaisePropertyChanged("Adate");
                }
            }
        }

        [ReadOnly(true)]
        public string Approval
        {
            get
            {
                return _approvalField;
            }
            set
            {
                if ((ReferenceEquals(_approvalField, value) != true))
                {
                    _approvalField = value;
                    RaisePropertyChanged("Approval");
                }
            }
        }

        [ReadOnly(true)]
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

        [ReadOnly(true)]
        public string EmployeeGuid
        {
            get
            {
                return _employeeGuidField;
            }
            set
            {
                if ((ReferenceEquals(_employeeGuidField, value) != true))
                {
                    _employeeGuidField = value;
                    RaisePropertyChanged("EmployeeGuid");
                }
            }
        }

        [ReadOnly(true)]
        public string Empname
        {
            get
            {
                return _empnameField;
            }
            set
            {
                if ((ReferenceEquals(_empnameField, value) != true))
                {
                    _empnameField = value;
                    RaisePropertyChanged("Empname");
                }
            }
        }

        [ReadOnly(true)]
        public string EnrollNumber
        {
            get
            {
                return _enrollNumberField;
            }
            set
            {
                if ((ReferenceEquals(_enrollNumberField, value) != true))
                {
                    _enrollNumberField = value;
                    RaisePropertyChanged("EnrollNumber");
                }
            }
        }

        [ReadOnly(true)]
        public string ExcuseType
        {
            get
            {
                return _excuseTypeField;
            }
            set
            {
                if ((ReferenceEquals(_excuseTypeField, value) != true))
                {
                    _excuseTypeField = value;
                    RaisePropertyChanged("ExcuseType");
                }
            }
        }

        [ReadOnly(true)]
        public string FromDate
        {
            get
            {
                return _fromDateField;
            }
            set
            {
                if ((ReferenceEquals(_fromDateField, value) != true))
                {
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        [ReadOnly(true)]
        public TimeSpan? FromTime
        {
            get
            {
                return _fromTimeField;
            }
            set
            {
                if ((_fromTimeField.Equals(value) != true))
                {
                    _fromTimeField = value;
                    RaisePropertyChanged("FromTime");
                }
            }
        }

        [ReadOnly(true)]
        public string Intime
        {
            get
            {
                return _intimeField;
            }
            set
            {
                if ((ReferenceEquals(_intimeField, value) != true))
                {
                    _intimeField = value;
                    RaisePropertyChanged("Intime");
                }
            }
        }

        [ReadOnly(true)]
        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [ReadOnly(true)]
        public string ManagerEmail
        {
            get
            {
                return _managerEmailField;
            }
            set
            {
                if ((ReferenceEquals(_managerEmailField, value) != true))
                {
                    _managerEmailField = value;
                    RaisePropertyChanged("ManagerEmail");
                }
            }
        }

        [ReadOnly(true)]
        public string ManagerGuid
        {
            get
            {
                return _managerGuidField;
            }
            set
            {
                if ((ReferenceEquals(_managerGuidField, value) != true))
                {
                    _managerGuidField = value;
                    RaisePropertyChanged("ManagerGuid");
                }
            }
        }

        [ReadOnly(true)]
        public string MissionType
        {
            get
            {
                return _missionTypeField;
            }
            set
            {
                if ((ReferenceEquals(_missionTypeField, value) != true))
                {
                    _missionTypeField = value;
                    RaisePropertyChanged("MissionType");
                }
            }
        }

        [ReadOnly(true)]
        public string Reason
        {
            get
            {
                return _reasonField;
            }
            set
            {
                if ((ReferenceEquals(_reasonField, value) != true))
                {
                    _reasonField = value;
                    RaisePropertyChanged("Reason");
                }
            }
        }

        [ReadOnly(true)]
        public string Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                if ((ReferenceEquals(_statusField, value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        [ReadOnly(true)]
        public TimeSpan? ToTime
        {
            get
            {
                return _toTimeField;
            }
            set
            {
                if ((_toTimeField.Equals(value) != true))
                {
                    _toTimeField = value;
                    RaisePropertyChanged("ToTime");
                }
            }
        }

        [ReadOnly(true)]
        public string VacationType
        {
            get
            {
                return _vacationTypeField;
            }
            set
            {
                if ((ReferenceEquals(_vacationTypeField, value) != true))
                {
                    _vacationTypeField = value;
                    RaisePropertyChanged("VacationType");
                }
            }
        }

        [ReadOnly(true)]
        public string outtime
        {
            get
            {
                return _outtimeField;
            }
            set
            {
                if ((ReferenceEquals(_outtimeField, value) != true))
                {
                    _outtimeField = value;
                    RaisePropertyChanged("outtime");
                }
            }
        }

        [ReadOnly(true)]
        public string toDate
        {
            get
            {
                return _toDateField;
            }
            set
            {
                if ((ReferenceEquals(_toDateField, value) != true))
                {
                    _toDateField = value;
                    RaisePropertyChanged("toDate");
                }
            }
        }
    }

    public class EmployeeInfoViewModel : ViewModelBase
    {
        AttService.AttServiceClient AttService = new AttService.AttServiceClient();

        public EmployeeInfoViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblEmployeeInfoViewModel>();
                SelectedMainRows = new ObservableCollection<TblEmployeeInfoViewModel>();

                AttService.GetTblEmloyeeInfoCompleted += (x, y) =>
                {
                    foreach (var row in y.Result)
                    {
                        var newrow = new TblEmployeeInfoViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                };
            }
        }

        public void GetMaindata()
        {
            AttService.GetTblEmloyeeInfoAsync(Code);
        }

        private ObservableCollection<TblEmployeeInfoViewModel> _mainRowList;

        public ObservableCollection<TblEmployeeInfoViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblEmployeeInfoViewModel> _selectedMainRows;

        public ObservableCollection<TblEmployeeInfoViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblEmployeeInfoViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }
    }
}