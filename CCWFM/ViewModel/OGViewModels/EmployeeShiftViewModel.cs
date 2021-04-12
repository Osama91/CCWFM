using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.AttViewModel;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.AttService;

namespace CCWFM.ViewModel.OGViewModels
{
    public class DayClass : PropertiesViewModelBase
    {
        private DateTime? _date;

        public DateTime? Date
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged("Date"); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }
    }

    public class TblEmployeeshiftViewModel : PropertiesViewModelBase
    {
        private int? _approvedby;

        public int? ApprovedBy
        {
            get { return _approvedby; }
            set { _approvedby = value; RaisePropertyChanged("ApprovedBy"); }
        }

        private int _createdBy;

        public int CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; RaisePropertyChanged("CreatedBy"); }
        }

        private DateTime? _creationDate;

        public DateTime? CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private DateTime? _approvedDate;

        public DateTime? ApprovedDate
        {
            get { return _approvedDate; }
            set { _approvedDate = value; RaisePropertyChanged("ApprovedDate"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private bool _disableManualAttendance;

        public bool DisableManualAttendance
        {
            get { return _disableManualAttendance; }
            set { _disableManualAttendance = value; RaisePropertyChanged("DisableManualAttendance"); }
        }

        private bool _loading;

        public bool Loaded
        {
            get { return _loading; }
            set { _loading = value; RaisePropertyChanged("Loaded"); }
        }

        private int? _maxdayOffPerWeek;

        public int? MaxdayOffPerWeek
        {
            get { return _maxdayOffPerWeek; }
            set { _maxdayOffPerWeek = value; RaisePropertyChanged("MaxdayOffPerWeek"); }
        }

        private string _dayOff;

        public string DayOff
        {
            get { return _dayOff; }
            set { _dayOff = value; RaisePropertyChanged("DayOff"); }
        }

        public ObservableCollection<TblVacationDetail> RemainingVacations { get; set; }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow ?? (_storePerRow = new TblStore()); }
            set
            {
                _storePerRow = value;
                RaisePropertyChanged("StorePerRow");
                if (StorePerRow != null)
                {
                    TblStore = StorePerRow.iserial;
                }
            }
        }

        private ObservableCollection<VacationType> _ccVacation;

        public ObservableCollection<VacationType> CcVacation
        {
            get
            {
                return _ccVacation;
            }
            set
            {
                if ((ReferenceEquals(_ccVacation, value) != true))
                {
                    _ccVacation = value;
                    RaisePropertyChanged("CcVacation");
                }
            }
        }

        private int? _dayField;

        private string _empIdField;

        private int _iserialField;

        private int? _tblShiftField;

        private int _tblStoreField;

        private DateTime? _transDateField;

        private int? _weekField;

        public int? Day
        {
            get
            {
                return _dayField;
            }
            set
            {
                if ((_dayField.Equals(value) != true))
                {
                    _dayField = value;
                    RaisePropertyChanged("Day");
                }
            }
        }

        private int _statusField;

        public int Status
        {
            get { return _statusField; }
            set
            {
                if ((_statusField.Equals(value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                    if (Loaded)
                    {
                        if (Status == 1)
                        {
                            ApprovedBy = LoggedUserInfo.Iserial;
                            ApprovedDate = DateTime.Now;
                        }
                    }
                }
            }
        }

        public string EmpId
        {
            get
            {
                return _empIdField;
            }
            set
            {
                if ((ReferenceEquals(_empIdField, value) != true))
                {
                    _empIdField = value;
                    RaisePropertyChanged("EmpId");
                }
            }
        }

        private string _periodId;

        public string PeriodId
        {
            get
            {
                return _periodId;
            }
            set
            {
                if ((ReferenceEquals(_periodId, value) != true))
                {
                    _periodId = value.ToUpper();
                    RaisePropertyChanged("PeriodId");
                }
            }
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if ((ReferenceEquals(_name, value) != true))
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

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

        public int? TblEmployeeShiftLookup
        {
            get
            {
                return _tblShiftField;
            }
            set
            {
                _tblShiftField = value;
                RaisePropertyChanged("TblEmployeeShiftLookup");
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

        public DateTime? TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }

        private string _position;

        public string Position
        {
            get { return _position; }
            set
            {
                _position = value; RaisePropertyChanged("Position");
            }
        }

        private int? _yearField;

        public int? Year
        {
            get
            {
                return _yearField;
            }
            set
            {
                if ((_yearField.Equals(value) != true))
                {
                    _yearField = value;
                    RaisePropertyChanged("Year");
                }
            }
        }

        public int? Week
        {
            get
            {
                return _weekField;
            }
            set
            {
                if ((_weekField.Equals(value) != true))
                {
                    _weekField = value;
                    RaisePropertyChanged("Week");
                }
            }
        }

        private ObservableCollection<TblEmployeeshiftViewModel> _selectedMainRows;

        public ObservableCollection<TblEmployeeshiftViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblEmployeeshiftViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private ObservableCollection<TblVacationViewModel> _selectedVacations;

        public ObservableCollection<TblVacationViewModel> SelectedVacations
        {
            get { return _selectedVacations ?? (_selectedVacations = new ObservableCollection<TblVacationViewModel>()); }
            set
            {
                _selectedVacations = value;
                RaisePropertyChanged("SelectedVacations");
            }
        }

        private ObservableCollection<TblVacationViewModel> _selectedCopyVacations;

        public ObservableCollection<TblVacationViewModel> SelectedCopyVacations
        {
            get { return _selectedCopyVacations ?? (_selectedCopyVacations = new ObservableCollection<TblVacationViewModel>()); }
            set
            {
                _selectedCopyVacations = value;
                RaisePropertyChanged("SelectedCopyVacations");
            }
        }

        private ObservableCollection<TblExcuseViewModel> _selectedExcusess;

        public ObservableCollection<TblExcuseViewModel> SelectedExcuses
        {
            get { return _selectedExcusess ?? (_selectedExcusess = new ObservableCollection<TblExcuseViewModel>()); }
            set
            {
                _selectedExcusess = value;
                RaisePropertyChanged("SelectedExcuses");
            }
        }

        private ObservableCollection<TblMissionViewModel> _selectedMissions;

        public ObservableCollection<TblMissionViewModel> SelectedMissions
        {
            get { return _selectedMissions ?? (_selectedMissions = new ObservableCollection<TblMissionViewModel>()); }
            set
            {
                _selectedMissions = value;
                RaisePropertyChanged("SelectedMissions");
            }
        }

        private ObservableCollection<TblAttendanceFileViewModel> _selectedAtendancefile;

        public ObservableCollection<TblAttendanceFileViewModel> SelectedAttendanceFile
        {
            get { return _selectedAtendancefile ?? (_selectedAtendancefile = new ObservableCollection<TblAttendanceFileViewModel>()); }
            set
            {
                _selectedAtendancefile = value;
                RaisePropertyChanged("SelectedAttendanceFile");
            }
        }

        private Dictionary<decimal, int> _excuseCount;

        public Dictionary<decimal, int> ExcuseCount
        {
            get { return _excuseCount ?? (_excuseCount = new Dictionary<decimal, int>()); }
            set { _excuseCount = value; RaisePropertyChanged("ExcuseCount"); }
        }

        public string Transportation { get; set; }
    }

    public class TblAttendanceFileViewModel : PropertiesViewModelBase
    {


        public string Transportation { get; set; }
        private int? _approvedby;

        public int? ApprovedBy
        {
            get { return _approvedby; }
            set { _approvedby = value; RaisePropertyChanged("ApprovedBy"); }
        }

        private int _createdBy;

        public int CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; RaisePropertyChanged("CreatedBy"); }
        }

        private DateTime? _creationDate;

        public DateTime? CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private DateTime? _approvedDate;

        public DateTime? ApprovedDate
        {
            get { return _approvedDate; }
            set { _approvedDate = value; RaisePropertyChanged("ApprovedDate"); }
        }

        private int _tblAttendanceFileReason;

        public int TblAttendanceFileReason
        {
            get { return _tblAttendanceFileReason; }
            set { _tblAttendanceFileReason = value; RaisePropertyChanged("TblAttendanceFileReason"); }
        }

        private Visibility _attStatusVisibility;

        public Visibility AttStatusVisibility
        {
            get { return _attStatusVisibility; }
            set { _attStatusVisibility = value; RaisePropertyChanged("AttStatusVisibility"); }
        }

        private TblAttendanceFileReason _reasonPerRow;

        public TblAttendanceFileReason ReasonPerRow
        {
            get { return _reasonPerRow; }
            set
            {
                _reasonPerRow = value; RaisePropertyChanged("ReasonPerRow");
                if (ReasonPerRow != null)
                {
                    TblAttendanceFileReason = ReasonPerRow.Iserial;
                }
            }
        }

        private string _description;

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
        public string Description
        {
            get { return _description; }
            set
            {
                if ((ReferenceEquals(_description, value) != true))
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if ((ReferenceEquals(_name, value) != true))
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string _emplidField;

        private int? _fromTimeField;

        private int _iserialField;

        private int _statusField;

        private int? _toTimeField;

        private DateTime _transDateField;

        public string Emplid
        {
            get
            {
                return _emplidField;
            }
            set
            {
                if ((ReferenceEquals(_emplidField, value) != true))
                {
                    _emplidField = value;
                    RaisePropertyChanged("Emplid");
                }
            }
        }



        public int? FromTime
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

        private bool _loading;

        public bool Loaded
        {
            get { return _loading; }
            set { _loading = value; RaisePropertyChanged("Loaded"); }
        }

        public int Status
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
                    if (Loaded)
                    {
                        if (Status == 1)
                        {
                            ApprovedBy = LoggedUserInfo.Iserial;
                            ApprovedDate = DateTime.Now;
                        }
                    }
                }
            }
        }

        public int? ToTime
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

        private int? _orginalFromTime;

        [ReadOnly(true)]
        public int? OrginalFromTime
        {
            get
            {
                return _orginalFromTime;
            }
            set
            {
                if ((_orginalFromTime.Equals(value) != true))
                {
                    _orginalFromTime = value;
                    RaisePropertyChanged("OrginalFromTime");
                }
            }
        }

        private int? _orginalToTime;

        [ReadOnly(true)]
        public int? OrginalInTime
        {
            get
            {
                return _orginalToTime;
            }
            set
            {
                if ((_orginalToTime.Equals(value) != true))
                {
                    _orginalToTime = value;
                    RaisePropertyChanged("OrginalInTime");
                }
            }
        }

        ////private int? _OrginalInTime;
        ////public int? OrginalInTime
        ////{
        ////    get
        ////    {
        ////        return _OrginalInTime;
        ////    }
        ////    set
        ////    {
        ////        if ((_OrginalInTime.Equals(value) != true))
        ////        {
        ////            _OrginalInTime = value;
        ////            RaisePropertyChanged("OrginalInTime");
        ////        }
        ////    }
        ////}

        [ReadOnly(true)]
        public DateTime TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }
    }

    public class EmployeeShiftViewModel : ViewModelBase
    {
        #region Properties
        AttServiceClient AttService = new AttServiceClient();


        public List<DateTime> DaysToadd { get; set; }

        private bool _isVactionWindowClosedField;
        public bool IsVactionWindowClosed
        {
            get
            {
                return _isVactionWindowClosedField;
            }
            set
            {
                 _isVactionWindowClosedField = value;
                 RaisePropertyChanged("IsVactionWindowClosed");
            }
        }


        private int DaysToWork { get; set; }
        private int DaysToSee { get; set; }
        private int? _toTimeField;
        public int? ToTime
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

        private int? _fromTimeField;
        public int? FromTime
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
        #endregion
        public EmployeeShiftViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                PositionAllowed = true;
                TransportationLineAllow = Visibility.Collapsed;
                TransactionHeader = new TblEmployeeshiftViewModel();

                TransactionHeader.SelectedAttendanceFile.CollectionChanged += SelectedAttendanceFile_CollectionChanged;

                DaysToadd = new List<DateTime>();

                DetailList = new SortableCollectionView<EmployeesView>();

                GetItemPermissions(PermissionItemName.EmployeeShiftForm.ToString());
                GetCustomePermissions(PermissionItemName.EmployeeShiftForm.ToString());

                Client.GetChainSetupAsync(PermissionItemName.EmployeeShiftForm.ToString());

                Client.GetChainSetupCompleted += (s, sv) =>
                {
                    var firstOrDefault = sv.Result.FirstOrDefault(x => x.sGlobalSettingCode == "DayToWorkOn");
                    if (firstOrDefault != null) DaysToWork = Convert.ToInt32(firstOrDefault.sSetupValue);


                    var daysToSeeInt = sv.Result.FirstOrDefault(x => x.sGlobalSettingCode == "DaysToSee");
                    if (daysToSeeInt != null)
                        DaysToSee = Convert.ToInt32(daysToSeeInt.sSetupValue);

                    DayDate = sv.Today;
                    Year = sv.Today.Year;
                };

                AttService.GetEmpVacationCompleted += (s, sv) =>
                {
                    var vacationsToAdd = CcVacation.Select(x => x.CSPVACATIONID.ToLower());
                    var row = sv.Result.Where(x => vacationsToAdd.Contains(x.CSPVACATIONID.ToLower()));

                    if (row != null)
                        if (SelectedMainRow != null)
                            SelectedMainRow.RemainingVacations = new ObservableCollection<TblVacationDetail>(row);
                };

                AttService.ExcuseCountCompleted += (s, sv) =>
                {
                    if (SelectedMainRowCopy != null)
                    {
                        SelectedMainRowCopy.ExcuseCount.Clear();
                        foreach (var variable in sv.Result)
                        {
                            SelectedMainRowCopy.ExcuseCount.Add(variable.Key, variable.Value);
                        }
                    }
                };
                ExcuseVisibility = Visibility.Collapsed;
                MissionVisibility = Visibility.Collapsed;
                VacationVisibility = Visibility.Collapsed;
                AttFileVisibility = Visibility.Collapsed;
                ExcuseStatusVisibility = Visibility.Collapsed;
                MissionStatusVisibility = Visibility.Collapsed;
                VacationStatusVisibility = Visibility.Collapsed;
                PendingVisibility = Visibility.Collapsed;
                ExcuseStatusSelfVisibility = Visibility.Collapsed;
                MissionStatusSelfVisibility = Visibility.Collapsed;
                VacationStatusSelfVisibility = Visibility.Collapsed;
                AttStatusSelfVisibility = Visibility.Collapsed;
                AttFileStatusVisibility = Visibility.Collapsed;
                EmployeeShiftStatusVisibility = Visibility.Collapsed;
                EmployeeShiftVisibility = Visibility.Collapsed;

                EmpShiftAllowDelete = Visibility.Collapsed;
                AttFileAllowDelete = Visibility.Collapsed;
                MissionAllowDelete = Visibility.Collapsed;
                VacationAllowDelete = Visibility.Collapsed;
                ExcuseAllowDelete = Visibility.Collapsed;

                StoreVisibility = LoggedUserInfo.Company.Code == "HQ" ? Visibility.Collapsed : Visibility.Visible;
                PositionVisibility = LoggedUserInfo.Company.Code != "HQ" ? Visibility.Collapsed : Visibility.Visible;
                GetDays();
                YearList = new List<int>();
                if (StoreVisibility== Visibility.Collapsed)
                {
                    GetAllShiftLookup();
                }
                Client.GetEmpPositionCompleted += (s, sv) =>
                {
                    PositionList = sv.Result.ToList();
                    if (PositionList.Count() == 1)
                    {
                        TransactionHeader.Position = PositionList.FirstOrDefault();
                        Position = PositionList.FirstOrDefault();
                    }
                };
                Client.GetEmpTransportationLineCompleted += (s, sv) =>
                {
                    TransportationList = sv.Result.ToList();
                    if (TransportationList.Count() == 1)
                    {
                        TransactionHeader.Transportation = TransportationList.FirstOrDefault();
                        Transportation = TransportationList.FirstOrDefault();
                    }
                };

                AttService.GetTblExcuseRuleAsync(0, int.MaxValue, "it.Iserial", null, null);
                AttService.GetTblExcuseRuleCompleted += (s, sv) =>
                {
                    ExcuseRuleList = sv.Result;
                };
                for (var i = 0; i < 10; i++)
                {
                    const int yearNumber = 2014;
                    YearList.Add(yearNumber + i);
                }

                if (LoggedUserInfo.Store != null)
                    TransactionHeader.StorePerRow =  new TblStore().InjectFrom(LoggedUserInfo.Store) as TblStore;  
                StoreList = new SortableCollectionView<TblStore>();

                if (LoggedUserInfo.AllowedStores != null && LoggedUserInfo.Company.Code != "HQ")
                    Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, null, null, LoggedUserInfo.DatabasEname);

                Client.SearchBysStoreNameCompleted += (s, sv) =>
                {
                    StoreList = sv.Result;
                };

                Client.GetEmpTableCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        DetailList.Add(row);
                    }

                    DetailFullCount = sv.fullCount;
                    Loading = false;
                };

                AttService.VacationTypeCompleted += (s, y) =>
                {
                   
                    if (CustomePermissions.Any(w=>w.Code== "ShowHiddenVacations"))
                    {
                        CcVacation = y.Result;
                    }
                    else
                    {
                        var newObservableCollection = new ObservableCollection<VacationType>();
                        foreach (var item in y.Result.Where(w => w.HIDDEN == 0).ToList())
                        {
                            newObservableCollection.Add(item);

                        }
                        
                        CcVacation = newObservableCollection;
                    }


                    if (y.Result.Any())
                    {
                        CcVacation.Insert(0, new VacationType
                        {
                            CSPVACATIONID = "",
                            NAME = "",
                            CSPPERIODID = y.Result.First().CSPPERIODID
                        });
                    }

                };

                AttService.GetTblAttendanceFileReasonAsync(0, int.MaxValue, "it.Iserial", null, null);
                AttService.GetTblAttendanceFileReasonCompleted += (s, sv) =>
                {
                    AttendanceFileReasonList = sv.Result;
                };
                AttService.ExcuseTypeCompleted += (x, y) =>
                {
                    CcExcuse = y.Result;
                    CcExcuse.Insert(0, new ExcuseType
                    {
                        CSPEXCUSEID = "",
                        NAME = ""
                    });
                };
                AttService.ExcuseTypeAsync();

                AttService.MissionTypeCompleted += (x, y) =>
                {
                    Ccmission = y.Result;
                    Ccmission.Insert(0, new MissionType
                    {
                        CSPMISSIONID = "",
                        Name = "",
                    });
                };
                AttService.MissionTypeAsync();

                //AttService.GetTblEmployeeShiftStoreAsync
                //
                Client.GetTblEmployeeShiftLookupCompleted += (s, sv) =>
                 {
                     var Shifts = new ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup>();
                     Shifts = sv.Result;
                     if (LoggedUserInfo.Code == "0140")
                     {
                         EmployeeShiftLookUp = new ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup>(Shifts.Where(x => x.Shift == "DayOff"));
                     }
                     else
                     {
                         EmployeeShiftLookUp = Shifts;
                     }
                 };
                AttService.GetTblEmployeeShiftStoreCompleted += (s, sv) =>
                 {
                     var Shifts = new ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup>();
                     foreach (var item in sv.Result.Select(w => w.TblEmployeeShiftLookup1).ToList())
                     {
                         Shifts.Add(new CRUDManagerService.TblEmployeeShiftLookup().InjectFrom(item) as CRUDManagerService.TblEmployeeShiftLookup);
                     }
                     if (LoggedUserInfo.Code == "0140")
                     {
                         EmployeeShiftLookUp = new ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup>(Shifts.Where(x => x.Shift == "DayOff"));
                     }
                     else
                     {
                         EmployeeShiftLookUp = Shifts;
                     }
                 };

                Client.UpdateAndInsertTblEmployeeShiftCompleted += (s, sv) =>
                {

                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                        Loading = false;
                        return;
                    }
                    var savedRow = TransactionHeader.SelectedMainRows.ElementAt(sv.outindex);

                    if (savedRow != null) savedRow.SelectedMainRows.FirstOrDefault(x => x.TransDate == sv.Result.TransDate).InjectFrom(sv.Result);
                    Changed = false;
                };

                AttService.UpdateAndInsertTblExcuseCompleted += (s, sv) =>
                {
                    if (sv.Error == null)
                    {
                        var savedRow = TransactionHeader.SelectedMainRows.ElementAt(sv.outindex);
                        var row = savedRow.SelectedExcuses.FirstOrDefault(x => x.TransDate == sv.Result.TransDate && x.FromTime == sv.Result.FromTime);
                        if (sv.Result.Status != 5)
                        {
                            if (row != null) row.InjectFrom(sv.Result);
                        }
                        else if (sv.Result.Status == 6)
                        {
                            MessageBox.Show("Excuse exceed max allowed time");

                        }
                        else
                        {
                            if (row != null)
                            {
                                row.CSPEXCUSEID = null;
                                row.FromTime = null;
                                row.ToTime = null;
                            }
                            MessageBox.Show("Excuses exceed Limit");
                        }
                    }
                    else {
                        MessageBox.Show(sv.Error.Message);
                        Loading = false;
                        return;
                    }
                    Changed = false;
                };

                AttService.UpdateAndInsertTblMissionCompleted += (s, sv) =>
                {
                    var savedRow = TransactionHeader.SelectedMainRows.ElementAt(sv.outindex);

                    if (savedRow != null)
                        if (savedRow.SelectedMissions != null)
                        {
                            var Gridrow = savedRow.SelectedMissions.FirstOrDefault(x => x.FromDate == sv.Result.FromDate && x.FromTime == sv.Result.FromTime);

                            if (Gridrow != null) Gridrow.InjectFrom(sv.Result);

                            if (Gridrow != null)
                            {
                                foreach (var row in TransactionHeader.SelectedMainRows.Where(x => x.EmpId == Gridrow.Emplid))
                                {
                                    var rowToInject =
                                        row.SelectedMissions.FirstOrDefault(x => x.FromDate == Gridrow.FromDate && x.Status == 0);
                                    if (rowToInject != null && rowToInject.ContainMultipleTransaction)
                                    {

                                        if (Gridrow.FromTime != rowToInject.FromTime)
                                        {
                                            Gridrow.InjectFrom(rowToInject);
                                            Gridrow.IsNotExtraMission = Visibility.Visible;
                                            rowToInject.FromTime = sv.Result.FromTime;
                                            rowToInject.ToTime = sv.Result.ToTime;
                                            rowToInject.Status = sv.Result.Status;
                                        }
                                    }
                                }
                            }
                        }
                    Changed = false;
                };

                AttService.UpdateAndInsertTblVacationCompleted += (s, sv) =>
                {
                    var savedRow = TransactionHeader.SelectedMainRows.ElementAt(sv.outindex);
                    var row = savedRow.SelectedVacations.FirstOrDefault(x => x.FromDate == sv.Result.FromDate);
                    if (sv.Result.Iserial != 0)
                    {
                        if (savedRow.SelectedVacations != null)
                        {
                            if (row != null)
                            {
                                row.Iserial = sv.Result.Iserial;
                                row.CSPVACATIONID = row.CSPVACATIONID;
                            }
                        }
                    }
                    else
                    {
                        if (savedRow.SelectedVacations != null)
                            if (row != null)
                                row.CSPVACATIONID =
                                    null;
                        MessageBox.Show("Vacations  exceed Limit");
                    }
                    Changed = false;
                };
                AttService.UpdateAndInsertTblAttendanceFileCompleted += (s, sv) =>
                {
                    var savedRow = TransactionHeader.SelectedAttendanceFile.ElementAt(sv.outindex);


                    if (sv.Result.Status != 5)
                    {
                        if (savedRow != null) savedRow.InjectFrom(sv.Result);
                    }

                    else
                    {
                        if (savedRow != null)
                        {

                            savedRow.FromTime = null;
                            savedRow.ToTime = null;
                        }
                        MessageBox.Show("Attendance Modification exceed Limit");
                    }

                    Changed = false;
                };
                AttService.GetTblAttendanceFileForEmpCompleted += (s, sv) =>
                {
                    TransactionHeader.SelectedAttendanceFile.Clear();

                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        foreach (var day in DaysToadd)
                        {
                            var newrow = new TblAttendanceFileViewModel
                            {
                                Emplid = emp.EmpId,
                                Name = emp.Name,
                                TransDate = day.Date,
                                Status = AttFileStatusVisibility == Visibility.Visible ? 1 : 0,
                               // ReasonPerRow = AttendanceFileReasonList.LastOrDefault(),
                                Transportation = emp.Transportation,
                            };

                            if (emp.EmpId == LoggedUserInfo.Code)
                            {
                                newrow.Status = 1;
                                newrow.AttStatusVisibility = Visibility.Visible;
                            }
                            else
                            {
                                newrow.Status = AttFileStatusVisibility == Visibility.Visible ? 1 : 0;
                                newrow.AttStatusVisibility = AttFileStatusVisibility;
                            }
                            newrow.Loaded = true;
                            TransactionHeader.SelectedAttendanceFile.Add(newrow);
                        }
                    }

                    //foreach (var savedrow in sv.axAttendanceFileList.OrderBy(x => x.CSPDIRECTION))
                    //{
                    //    var rowToInject = TransactionHeader.SelectedAttendanceFile.FirstOrDefault(x => x.Emplid == savedrow.EMPLID && x.TransDate.Date == savedrow.TRANSDATE.Date);

                    //    if (rowToInject != null)
                    //    {
                    //        if (savedrow.CSPDIRECTION == 0)
                    //        {
                    //            rowToInject.OrginalFromTime = savedrow.TIME;
                    //        }

                    //        else if (savedrow.CSPDIRECTION == 1)
                    //        {
                    //            if (rowToInject.OrginalFromTime != savedrow.TIME)
                    //            {

                    //                rowToInject.OrginalInTime = savedrow.TIME;


                    //            }
                    //        }
                    //    }
                    //}
                    //foreach (var savedrow in sv.PayrollAttendanceFile)
                    //{
                    //    var rowToInject = TransactionHeader.SelectedAttendanceFile.FirstOrDefault(x => x.Emplid == savedrow.TblEmployee1.EmpId && x.TransDate.Date == savedrow.TransDate.Date);

                    //    if (rowToInject != null)
                    //    {
                    //        if (savedrow.FromTime!=0)
                    //        {
                    //            rowToInject.OrginalFromTime = savedrow.FromTime;

                    //        }
                    //        if (savedrow.ToTime != 0)
                    //        {
                    //            rowToInject.OrginalInTime = savedrow.ToTime;

                    //        }


                    //    }
                    //}


                    foreach (var savedrow in sv.Result)
                    {
                        foreach (var row in TransactionHeader.SelectedAttendanceFile.Where(x => x.Emplid == savedrow.Emplid))
                        {
                            if (row.TransDate == savedrow.TransDate)
                            {
                                row.InjectFrom(savedrow);

                                row.OrginalInTime = savedrow.OrginalInTime;
                                row.ReasonPerRow = savedrow.TblAttendanceFileReason1;
                            }
                        }
                    }
                    if (Pending)
                    {
                        if (sv.Result.Any())
                        {
                            foreach (var row in TransactionHeader.SelectedAttendanceFile.Where(
                                x => !sv.Result.Select(w => w.Emplid).Contains(x.Emplid)).ToList())
                            {
                                TransactionHeader.SelectedAttendanceFile.Remove(row);
                            }
                        }
                        else
                        {
                            TransactionHeader.SelectedAttendanceFile.Clear();
                        }
                    }
                    Loading = false;
                    Changed = false;
                };
                Client.GetTblEmployeeShiftByStoreCompleted += (s, sv) =>
                {
                    foreach (var row in TransactionHeader.SelectedMainRows)
                    {
                        row.SelectedMainRows.Clear();
                    }
                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        if (DaysToadd.Count < 7 && DaysToadd.OrderBy(x => x.Date).FirstOrDefault().Day == 1)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedMainRows.Add(new TblEmployeeshiftViewModel
                                {
                                    Enabled = false,
                                    Week = TransactionHeader.Week,
                                    Year = Year,
                                    TblStore = TransactionHeader.TblStore,
                                    EmpId = emp.EmpId,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0,
                                    Position = emp.Position,
                                    DayOff = emp.DayOff
                                });
                            }
                        }

                        foreach (var day in DaysToadd)
                        {
                            var enabled = true;


                            if (!NoDayLimitApproval)
                            {
                                if (DaysToSee <= DateTime.Now.Day)
                                {
                                    if (DayDate != null && day.AddDays(-DateTime.Now.Day + 1).Date > day.Date)
                                    {
                                        enabled = false;
                                    }
                                }
                                else
                                {
                                    if (DayDate != null && day.AddDays(-DateTime.Now.Day - 30).Date > day.Date)
                                    {
                                        enabled = false;
                                    }
                                }

                            }
                            if (!NoDayLimit)
                            {
                                if (DayDate != null && DateTime.Now.AddDays(-DaysToWork).Date > day.Date)
                                {
                                    enabled = false;
                                }
                            }

                            var rowww = new TblEmployeeshiftViewModel
                            {
                                Enabled = enabled,
                                Day = day.Day,
                                Week = TransactionHeader.Week,
                                Year = Year,
                                TblStore = TransactionHeader.TblStore,
                                TransDate = day.Date,
                                EmpId = emp.EmpId,
                                Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0,
                                DayOff = emp.DayOff
                            };
                            emp.SelectedMainRows.Add(rowww);
                            if (day.DayOfWeek.ToString() == emp.DayOff)
                            {
                                rowww.Enabled = false;
                                rowww.TblEmployeeShiftLookup = EmployeeShiftLookUp.FirstOrDefault(x => x.Shift == "DayOff").Iserial;
                                selectedEmployeeShift = rowww;
                                SaveRow(AttSaveTypes.Shift);
                            }
                            emp.SelectedMainRows.CollectionChanged += SelectedMainRows_CollectionChanged;
                        }
                        if (DaysToadd.Count < 7)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedMainRows.Add(new TblEmployeeshiftViewModel
                                {
                                    Enabled = false,

                                    Week = TransactionHeader.Week,
                                    Year = Year,
                                    TblStore = TransactionHeader.TblStore,

                                    EmpId = emp.EmpId,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0,
                                    DayOff = emp.DayOff
                                });
                            }
                        }
                    }
                    foreach (var savedrow in sv.Result)
                    {
                        foreach (var row in TransactionHeader.SelectedMainRows.Where(x => x.EmpId == savedrow.EmpId))
                        {
                            var rowToInject =
                                row.SelectedMainRows.SingleOrDefault(x => x.TransDate == savedrow.TransDate);

                            if (rowToInject != null) rowToInject.InjectFrom(savedrow);
                        }
                    }
                    if (Pending)
                    {
                        if (sv.Result.Any())
                        {
                            foreach (var row in TransactionHeader.SelectedMainRows.Where(
                                x => !sv.Result.Select(w => w.EmpId).Contains(x.EmpId)).ToList())
                            {
                                TransactionHeader.SelectedMainRows.Remove(row);
                            }
                        }
                        else
                        {
                            TransactionHeader.SelectedMainRows.Clear();
                        }
                    }
                    Loading = false;
                    Changed = false;
                };
                AttService.GetTblExcusesForStoresCompleted += (s, sv) =>
                {
                    foreach (var row in TransactionHeader.SelectedMainRows)
                    {
                        row.SelectedExcuses.Clear();
                    }
                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        if (DaysToadd.Count < 7 && DaysToadd.OrderBy(x => x.Date).FirstOrDefault().Day == 1)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedExcuses.Add(new TblExcuseViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                    }
                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        foreach (var day in DaysToadd.Where(x => true))
                        {
                            var enabled = true;
                            if (!NoDayLimitApproval)
                            {
                                if (DaysToSee <= DateTime.Now.Day)
                                {
                                    if (DateTime.Now.Month != day.Date.Month)
                                    {
                                        enabled = false;
                                    }
                                }
                                else
                                {
                                    //if (day.AddDays(-DateTime.Now.Day - 30).Date > day.Date)
                                    //{
                                    //    enabled = false;
                                    //}

                                    // if (DateTime.Now.Date.AddDays(-DaysToSee - 30).Date > day.Date)

                                    // if (DateTime.Now.Date.AddDays( -DateTime.Now.Day - DateTime.DaysInMonth(day.Year, DateTime.Now.Month -1)).Date >= day.Date)
                                    if (DateTime.Now.Date.AddDays(-DateTime.Now.Day - DateTime.DaysInMonth(day.Year, DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1)).Date >= day.Date)
                                    {
                                        enabled = false;
                                    }
                                }

                            }
                            if (!NoDayLimit)
                            {
                                if (DayDate != null && DateTime.Now.AddDays(-DaysToWork).Date > day.Date)
                                {
                                    enabled = false;
                                }
                            }
                          
                            var newexe = new TblExcuseViewModel
                            {
                                Enabled = enabled,
                                Emplid = emp.EmpId,
                                TransDate = day.Date,
                                IsNotExtraExcuse = ExcuseStatusSelfVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed
                            };
                            if (emp.EmpId == LoggedUserInfo.Code)
                            {
                                newexe.Status = ExcuseStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                newexe.ExcuseStatusVisibility = ExcuseStatusSelfVisibility;
                            }
                            else
                            {
                                newexe.Status = ExcuseStatusVisibility == Visibility.Visible ? 1 : 0;
                                newexe.ExcuseStatusVisibility = ExcuseStatusVisibility;
                            }
                            newexe.Loaded = true;
                            emp.SelectedExcuses.Add(newexe);
                            emp.SelectedExcuses.CollectionChanged += SelectedExcuses_CollectionChanged;
                        }
                    }
                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        if (DaysToadd.Count < 7)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedExcuses.Add(new TblExcuseViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                    }
                    foreach (var savedrow in sv.Result)
                    {
                        foreach (var row in TransactionHeader.SelectedMainRows.Where(x => x.EmpId == savedrow.Emplid))
                        {
                            var rowToInject =
                                row.SelectedExcuses.FirstOrDefault(x => x.TransDate == savedrow.TransDate);
                            if (rowToInject != null)
                            {
                                rowToInject.Loaded = false;

                                if (rowToInject.FromTime == null)
                                {
                                    rowToInject.InjectFrom(savedrow);
                                }

                                rowToInject.Loaded = true;
                                if (rowToInject.Status == 2)
                                {
                                    rowToInject.Enabled = false;
                                    rowToInject.BackgroundColor = new SolidColorBrush(Colors.Red);
                                }

                                if (sv.Result.Count(x => x.TransDate == rowToInject.TransDate) > 1)
                                {
                                    if (row.SelectedExcuses.Count(x => x.TransDate == savedrow.TransDate && x.FromTime == savedrow.FromTime) == 0)
                                    {
                                        var newexe = new TblExcuseViewModel
                                        {
                                            Enabled = rowToInject.Enabled,
                                            Emplid = rowToInject.Emplid,
                                            TransDate = rowToInject.TransDate,
                                            IsNotExtraExcuse = Visibility.Collapsed
                                        };
                                        if (rowToInject.Emplid == LoggedUserInfo.Code)
                                        {
                                            newexe.Status = ExcuseStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                            newexe.ExcuseStatusVisibility = ExcuseStatusSelfVisibility;
                                        }
                                        else
                                        {
                                            newexe.Status = ExcuseStatusVisibility == Visibility.Visible ? 1 : 0;
                                            newexe.ExcuseStatusVisibility = ExcuseStatusVisibility;
                                        }
                                        newexe.InjectFrom(savedrow);
                                        row.SelectedExcuses.Add(newexe);

                                        foreach (var excuseRow in row.SelectedExcuses)
                                        {
                                            excuseRow.ContainMultipleTransaction = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Pending)
                    {
                        if (sv.Result.Any())
                        {
                            foreach (var row in TransactionHeader.SelectedMainRows.Where(
                                x => !sv.Result.Select(w => w.Emplid).Contains(x.EmpId)).ToList())
                            {
                                TransactionHeader.SelectedMainRows.Remove(row);
                            }
                        }
                        else
                        {
                            TransactionHeader.SelectedMainRows.Clear();
                        }
                    }
                    Loading = false;
                    Changed = false;
                };

                AttService.GetTblVacationForStoresCompleted += (s, sv) =>
                {
                    foreach (var row in TransactionHeader.SelectedMainRows)
                    {
                        row.SelectedVacations.Clear();
                        row.SelectedCopyVacations.Clear();
                    }

                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        if (DaysToadd.Count < 7 && DaysToadd.OrderBy(x => x.Date).FirstOrDefault().Day == 1)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedVacations.Add(new TblVacationViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                        try
                        {
                            emp.CcVacation = new ObservableCollection<VacationType>(CcVacation.Where(x => x.CSPPERIODID == emp.PeriodId));
                        }
                        catch (Exception)
                        {
                        }

                        foreach (var day in DaysToadd.Where(x => x.Date != null))
                        {
                            var enabled = true;
                            if (!NoDayLimitApproval)
                            {
                                if (DaysToSee <= DateTime.Now.Day)
                                {
                                    if (DateTime.Now.Month != day.Date.Month)
                                    {
                                        enabled = false;
                                    }
                                }
                                else
                                {
                                    //if (DayDate != null && day.AddDays(-DateTime.Now.Day - 30).Date > day.Date)
                                    //{
                                    //    enabled = false;
                                    //}
                                  //  if (DateTime.Now.Month==1 )
                                  //  { }

                                    if (DateTime.Now.Date.AddDays(-DateTime.Now.Day - DateTime.DaysInMonth(day.Year, DateTime.Now.Month ==1 ? 12 : DateTime.Now.Month - 1)).Date >= day.Date)
                                    {
                                        enabled = false;
                                    }

                                }

                            }
                            if (!NoDayLimit)
                            {
                                if (DayDate != null && DateTime.Now.AddDays(-DaysToWork).Date > day.Date)
                                {
                                    enabled = false;
                                }
                            }
                            var newvac = new TblVacationViewModel
                            {
                                Enabled = enabled,
                                Emplid = emp.EmpId,
                                FromDate = day.Date,
                                ToDate = day.Date,
                                Status = VacationStatusVisibility == Visibility.Visible ? 1 : 0
                            };
                            if (emp.EmpId == LoggedUserInfo.Code)
                            {
                                newvac.Status = VacationStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                newvac.VacationStatusVisibility = VacationStatusSelfVisibility;
                            }
                            else
                            {
                                newvac.Status = VacationStatusVisibility == Visibility.Visible ? 1 : 0;
                                newvac.VacationStatusVisibility = VacationStatusVisibility;
                            }
                            newvac.Loaded = true;
                            emp.SelectedVacations.Add(newvac);

                            var newvacCopy = new TblVacationViewModel
                            {
                                Enabled = enabled,
                                Emplid = emp.EmpId,
                                FromDate = day.Date,
                                ToDate = day.Date,
                            };
                            if (emp.EmpId == LoggedUserInfo.Code)
                            {
                                newvacCopy.Status = VacationStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                newvacCopy.VacationStatusVisibility = VacationStatusSelfVisibility;
                            }
                            else
                            {
                                newvacCopy.Status = VacationStatusVisibility == Visibility.Visible ? 1 : 0;
                                newvacCopy.VacationStatusVisibility = VacationStatusVisibility;
                            }
                            newvacCopy.Loaded = true;
                            emp.SelectedCopyVacations.Add(newvacCopy);
                            emp.SelectedVacations.CollectionChanged += SelectedVacations_CollectionChanged;
                        }
                        if (DaysToadd.Count < 7)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedVacations.Add(new TblVacationViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                    }

                    foreach (var savedrow in sv.Result)
                    {
                        foreach (var row in TransactionHeader.SelectedMainRows.Where(x => x.EmpId == savedrow.Emplid))
                        {
                            var rowToInject =
                                row.SelectedVacations.SingleOrDefault(x => x.FromDate == savedrow.FromDate);

                            if (rowToInject != null)
                            {
                                rowToInject.Loaded = false;
                                rowToInject.InjectFrom(savedrow);
                                if (rowToInject.Status == 2)
                                {
                                    rowToInject.Enabled = false;
                                    rowToInject.BackgroundColor = new SolidColorBrush(Colors.Red);
                                }
                                rowToInject.VacationPerRow =
                                    CcVacation.SingleOrDefault(x => x.CSPVACATIONID == rowToInject.CSPVACATIONID);
                                rowToInject.Loaded = true;
                            }
                            var rowToInjectCpy =
                                row.SelectedCopyVacations.SingleOrDefault(x => x.FromDate == savedrow.FromDate);

                            if (rowToInjectCpy != null)
                            {
                                rowToInjectCpy.Loaded = false;
                                rowToInjectCpy.InjectFrom(savedrow);
                                if (rowToInjectCpy.Status == 2)
                                {
                                    rowToInjectCpy.Enabled = false;
                                    if (rowToInject != null)
                                        rowToInject.BackgroundColor = new SolidColorBrush(Colors.Red);
                                }
                                rowToInjectCpy.VacationPerRow =
                                    CcVacation.SingleOrDefault(x => x.CSPVACATIONID == rowToInjectCpy.CSPVACATIONID);
                                rowToInjectCpy.Loaded = true;
                            }
                        }
                    }

                    if (Pending)
                    {
                        if (sv.Result.Any())
                        {
                            foreach (var row in TransactionHeader.SelectedMainRows.Where(
                                x => !sv.Result.Select(w => w.Emplid).Contains(x.EmpId)).ToList())
                            {
                                TransactionHeader.SelectedMainRows.Remove(row);
                            }
                        }
                        else
                        {
                            TransactionHeader.SelectedMainRows.Clear();
                        }
                    }
                    Loading = false;
                    Changed = false;
                };

                AttService.GetTblMissionForStoresCompleted += (s, sv) =>
                {
                    foreach (var row in TransactionHeader.SelectedMainRows)
                    {
                        row.SelectedMissions.Clear();
                    }

                    foreach (var emp in TransactionHeader.SelectedMainRows)
                    {
                        if (DaysToadd.Count < 7 && DaysToadd.OrderBy(x => x.Date).FirstOrDefault().Day == 1)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedMissions.Add(new TblMissionViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                        foreach (var day in DaysToadd.Where(x => x.Date != null))
                        {
                            var enabled = true;
                            if (!NoDayLimitApproval)
                            {
                                if (DaysToSee <= DateTime.Now.Day)
                                {
                                    if (DateTime.Now.Month != day.Date.Month)
                                    {
                                        enabled = false;
                                    }
                                }
                                else
                                {
                                    //if (day != null && day.AddDays(-DateTime.Now.Day - 30).Date > day.Date)
                                    //{
                                    //    enabled = false;
                                    //}
                                    //if (DateTime.Now.Date.AddDays(-DateTime.Now.Day - DateTime.DaysInMonth(day.Year, DateTime.Now.Month - 1)).Date >= day.Date)
                                    if (DateTime.Now.Date.AddDays(-DateTime.Now.Day - DateTime.DaysInMonth(day.Year, DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1)).Date >= day.Date)
                                    {
                                        enabled = false;
                                    }
                                }

                            }
                            if (!NoDayLimit)
                            {
                                if (DayDate != null && DateTime.Now.AddDays(-DaysToWork).Date > day.Date)
                                {
                                    enabled = false;
                                }
                            }
                            var newmission = new TblMissionViewModel
                            {
                                Enabled = enabled,
                                Emplid = emp.EmpId,
                                FromDate = day.Date,
                                ToDate = day.Date,
                                Status = MissionStatusVisibility == Visibility.Visible ? 1 : 0,

                            };
                            if (emp.EmpId == LoggedUserInfo.Code)
                            {
                                newmission.Status = MissionStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                newmission.MissionStatusVisibility = MissionStatusSelfVisibility;
                                newmission.IsNotExtraMission = Visibility.Visible;
                            }
                            else
                            {
                                newmission.Status = MissionStatusVisibility == Visibility.Visible ? 1 : 0;
                                newmission.MissionStatusVisibility = MissionStatusVisibility;
                                newmission.IsNotExtraMission = Visibility.Visible;
                            }

                            newmission.Loaded = true;

                            emp.SelectedMissions.Add(newmission);
                            emp.SelectedMissions.CollectionChanged += SelectedMissions_CollectionChanged;
                        }
                        if (DaysToadd.Count < 7)
                        {
                            for (var i = DaysToadd.Count(); i < 7; i++)
                            {
                                emp.SelectedMissions.Add(new TblMissionViewModel
                                {
                                    Enabled = false,
                                    Status = EmployeeShiftStatusVisibility == Visibility.Visible ? 1 : 0
                                });
                            }
                        }
                    }

                    foreach (var savedrow in sv.Result.OrderBy(w => w.Status))
                    {
                        foreach (var row in TransactionHeader.SelectedMainRows.Where(x => x.EmpId == savedrow.Emplid))
                        {
                            var rowToInject =
                                row.SelectedMissions.FirstOrDefault(x => x.FromDate == savedrow.FromDate);

                            if (rowToInject != null)
                            {
                                rowToInject.Loaded = false;
                                if (rowToInject.FromTime == null)
                                {
                                    rowToInject.InjectFrom(savedrow);
                                }
                                if (rowToInject.Status == 2)
                                {
                                    rowToInject.Enabled = false;
                                    rowToInject.BackgroundColor = new SolidColorBrush(Colors.Red);
                                }

                                rowToInject.Loaded = true;
                                if (sv.Result.Count(x => x.FromDate == rowToInject.FromDate) > 1)
                                {
                                    if (row.SelectedMissions.Count(x => x.FromDate == savedrow.FromDate && x.FromTime == savedrow.FromTime) == 0)
                                    {
                                        var newexe = new TblMissionViewModel
                                        {
                                            Enabled = rowToInject.Enabled,
                                            Emplid = rowToInject.Emplid,
                                            FromDate = rowToInject.FromDate,
                                            IsNotExtraMission = Visibility.Collapsed
                                        };
                                        //if ((sv.Result.Count(x => x.FromDate == rowToInject.FromDate&& x.Emplid==rowToInject.Emplid)>1)
                                        //{

                                        //}
                                        if (rowToInject.Emplid == LoggedUserInfo.Code)
                                        {
                                            newexe.Status = MissionStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                                            newexe.MissionStatusVisibility = MissionStatusSelfVisibility;
                                        }
                                        else
                                        {
                                            newexe.Status = MissionStatusVisibility == Visibility.Visible ? 1 : 0;
                                            newexe.MissionStatusVisibility = MissionStatusVisibility;
                                        }
                                        newexe.InjectFrom(savedrow);
                                        if (newexe.Status == 2)
                                        {
                                            newexe.Enabled = false;
                                            newexe.BackgroundColor = new SolidColorBrush(Colors.Red);
                                        }
                                        row.SelectedMissions.Add(newexe);

                                        foreach (var missionRow in row.SelectedMissions.Where(w => w.FromDate == savedrow.FromDate))
                                        {
                                            missionRow.ContainMultipleTransaction = true;
                                            missionRow.BackgroundColor = new SolidColorBrush(Colors.Yellow);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Pending)
                    {
                        if (sv.Result.Any())
                        {
                            foreach (var row in TransactionHeader.SelectedMainRows.Where(
                                x => !sv.Result.Select(w => w.Emplid).Contains(x.EmpId)).ToList())
                            {
                                TransactionHeader.SelectedMainRows.Remove(row);
                            }
                        }
                        else
                        {
                            TransactionHeader.SelectedMainRows.Clear();
                        }
                    }
                    Loading = false;
                    Changed = false;
                };

                Client.GetEmpByAttOperatorCompleted += (s, sv) =>
                {
                    if (sv.Error!=null)
                    {
                        MessageBox.Show(sv.Error.Message);
                        Loading = false;
                        return;
                    }
                    var periodList = new List<string>();
                    TransactionHeader.SelectedMainRows.Clear();
                    TransactionHeader.SelectedAttendanceFile.Clear();
                    if (TransactionHeader.Week != null)
                        if (TransactionHeader.Year != null)
                            DaysToadd = GetDates((int)TransactionHeader.Year, (int)TransactionHeader.Week);
                    var c = 0;

                    foreach (var emp in sv.Result)
                    {
                        if (!periodList.Contains(emp.Period))
                        {
                            periodList.Add(emp.Period);
                        }

                        TransactionHeader.SelectedMainRows.Add(new TblEmployeeshiftViewModel
                        {
                            EmpId = emp.Emplid,
                            Name = emp.Name,
                            PeriodId = emp.Period,
                            Position = emp.Position,
                            MaxdayOffPerWeek = emp.MaxdayOffPerWeek,
                            Transportation = emp.TransportationLine
                        });
                    }
                    foreach (var variable in periodList)
                    {
                        if (DayDate != null)
                            AttService.VacationTypeAsync(variable, TransactionHeader.Year ?? DayDate.Value.Year, LoggedUserInfo.Code);
                    }
                    foreach (var dayClass in Days)
                    {
                        dayClass.Date = null;
                    }
                    if (DaysToadd != null)
                        foreach (var day in DaysToadd)
                        {
                            var firstOrDefault = Days.FirstOrDefault(x => x.Name == day.DayOfWeek.ToString());
                            if (firstOrDefault != null)
                                firstOrDefault.Date = day.Date;
                            c++;
                        }

                    Loading = false;
                    LoadTab();
                };
                Client.GetEmpTablebyStoreAndCompanyCompleted += (s, sv) =>
                {


                    var periodList = new List<string>();
                    if (TransactionHeader.Year != null)
                    {
                        if (TransactionHeader.Week != null)
                            DaysToadd = GetDates((int)TransactionHeader.Year, (int)TransactionHeader.Week);

                        var c = 0;
                        foreach (var dayClass in Days)
                        {
                            dayClass.Date = null;
                        }
                        foreach (var day in DaysToadd)
                        {
                            var firstOrDefault = Days.FirstOrDefault(x => x.Name == day.DayOfWeek.ToString());
                            if (firstOrDefault != null)
                                firstOrDefault.Date = day.Date;
                            c++;
                        }
                        TransactionHeader.SelectedMainRows.Clear();
                        TransactionHeader.SelectedAttendanceFile.Clear();
                        foreach (var emp in sv.Result)
                        {
                            if (!periodList.Contains(emp.Period))
                            {
                                periodList.Add(emp.Period);
                            }

                            TransactionHeader.SelectedMainRows.Add(new TblEmployeeshiftViewModel
                            {
                                EmpId = emp.Emplid,
                                Name = emp.Name,
                                PeriodId = emp.Period,
                                Position = emp.Position,
                                MaxdayOffPerWeek = emp.MaxdayOffPerWeek,
                                DayOff = emp.DayOff
                            });
                        }
                        Loading = false;
                        foreach (var variable in periodList)
                        {
                            Loading = true;
                            AttService.VacationTypeAsync(variable, (int)TransactionHeader.Year, LoggedUserInfo.Code);
                        }
                    }
                    LoadTab();
                };

                PremCompleted += (s, sv) =>
                {
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "EmployeeShiftTabView") != null)
                    {
                        EmployeeShiftVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "ExcuseTabView") != null)
                    {
                        ExcuseVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "MissionTabView") != null)
                    {
                        MissionVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "VacationTabView") != null)
                    {
                        VacationVisibility = Visibility.Visible;
                    }
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "AttFileTabView") != null)
                    {
                        AttFileVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "EmployeeShiftPost") != null)
                    {
                        EmployeeShiftStatusVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "ExcusePost") != null)
                    {
                        ExcuseStatusVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "PendingVis") != null)
                    {
                        PendingVisibility = Visibility.Visible;

                        Pending = true;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "MissionPost") != null)
                    {
                        MissionStatusVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "VacationPost") != null)
                    {
                        VacationStatusVisibility = Visibility.Visible;
                    }
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "MissionSelfPost") != null)
                    {
                        MissionStatusSelfVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "ExcuseSelfPost") != null)
                    {
                        ExcuseStatusSelfVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "VacationSelfPost") != null)
                    {
                        VacationStatusSelfVisibility = Visibility.Visible;
                    }

                    if (CustomePermissions.SingleOrDefault(x => x.Code == "TransportationLineAllow") != null)
                    {
                        TransportationLineAllow = Visibility.Visible;
                    }
                };

                AttService.DeleteTblMissionCompleted += (s, sv) =>
                {
                    Loading = false;
                    foreach (var variable in TransactionHeader.SelectedMainRows)
                    {
                        var row = variable.SelectedMissions.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (row != null)
                        {
                            row.FromTime = row.ToTime = null;
                            row.CSPMISSIONID = null;
                            return;
                        }
                    }
                };

                AttService.DeleteTblExcuseCompleted += (s, sv) =>
                {
                    Loading = false;
                    foreach (var variable in TransactionHeader.SelectedMainRows)
                    {
                        var row = variable.SelectedExcuses.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (row != null)
                        {
                            row.FromTime = row.ToTime = null;
                            row.CSPEXCUSEID = null;
                            row.Iserial = 0;
                            return;
                        }
                    }
                };

                AttService.DeleteTblVacationCompleted += (s, sv) =>
                {
                    Loading = false;
                    foreach (var variable in TransactionHeader.SelectedMainRows)
                    {
                        var row = variable.SelectedVacations.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (row != null)
                        {
                            row.CSPVACATIONID = null;
                            row.Iserial = 0;
                            row.Status = 0;
                            return;
                        }
                    }
                };
                Client.DeleteTblEmployeeShiftCompleted += (s, sv) =>
                {
                    Loading = false;
                    foreach (var variable in TransactionHeader.SelectedMainRows)
                    {
                        var row = variable.SelectedMainRows.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (row != null)
                        {
                            row.TblEmployeeShiftLookup = null;
                            row.Iserial = 0;
                            return;
                        }
                    }
                };
                AttService.DeleteTblAttendanceFileCompleted += (s, sv) =>
                {
                    Loading = false;
                    foreach (var variable in TransactionHeader.SelectedMainRows)
                    {
                        var row = variable.SelectedAttendanceFile.FirstOrDefault(x => x.Iserial == sv.Result);
                        if (row == null) continue;
                        row.FromTime = row.ToTime = null;
                        row.Iserial = 0;
                        return;
                    }
                };
            }
        }

        public void GetShiftLookup()
        {
            AttService.GetTblEmployeeShiftStoreAsync(TransactionHeader.TblStore,LoggedUserInfo.DatabasEname);
        }

        public void GetAllShiftLookup() {
            Client.GetTblEmployeeShiftLookupAsync();
        }
        public void LoadTab()
        {
            if (CustomePermissions.SingleOrDefault(x => x.Code == "EmployeeShiftTabView") != null)
            {
                if (SelectedTab == "Shift" || SelectedTab == null)
                {
                    GetShift();
                    return;
                }
            }

            if (CustomePermissions.SingleOrDefault(x => x.Code == "ExcuseTabView") != null)
            {
                if (SelectedTab == "Excuse" || SelectedTab == null)
                {
                    GetExcuses();
                    return;
                }
            }

            if (CustomePermissions.SingleOrDefault(x => x.Code == "MissionTabView") != null)
            {
                if (SelectedTab == "Mission" || SelectedTab == null)
                {
                    GetMission();
                    return;
                }
            }

            if (CustomePermissions.SingleOrDefault(x => x.Code == "VacationTabView") != null)
            {
                if (SelectedTab == "Vacation" || SelectedTab == null)
                {
                    GetVacation();
                    return;
                }
            }
            if (CustomePermissions.SingleOrDefault(x => x.Code == "AttFileTabView") != null)
            {
                if (SelectedTab == "Attendance" || SelectedTab == null)
                {
                    GetAttFile();
                }
            }
        }

        public void GetEmpVacation()
        {
            if(SelectedMainRow !=null)
             AttService.GetEmpVacationAsync(SelectedMainRow.EmpId);
        }

        public void GetExcuseCount(decimal month,DateTime TransDate)
        {
            AttService.ExcuseCountAsync(SelectedMainRowCopy.EmpId, SelectedMainRowCopy.PeriodId, month, TransDate);
        }

        public void GetPoisition(string code)
        {
            Client.GetEmpPositionAsync(code);
        }

        public void GetEmpTransportationLine(string code)
        {
            Client.GetEmpTransportationLineAsync(code);
        }

        private void SelectedMainRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblEmployeeshiftViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblEmployeeshiftViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void SelectedVacations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblVacationViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblVacationViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            Changed = true;
        }

        private void SelectedMissions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblMissionViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblMissionViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void SelectedExcuses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblExcuseViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblExcuseViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void SelectedAttendanceFile_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblAttendanceFileViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblAttendanceFileViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        public void GetShift()
        {
            if (EmployeeShiftVisibility == Visibility.Visible)
            {
                Loading = true;
                if (TransactionHeader.Week != null)
                    Client.GetTblEmployeeShiftByStoreAsync((int)TransactionHeader.Week, new ObservableCollection<string>(TransactionHeader.SelectedMainRows.Select(e => e.EmpId)), Pending);
            }
        }

        public void GetExcuses()
        {
            if (ExcuseVisibility == Visibility.Visible)
            {
                Loading = true;
                AttService.GetTblExcusesForStoresAsync(new ObservableCollection<string>(TransactionHeader.SelectedMainRows.Select(e => e.EmpId)), new ObservableCollection<DateTime>(DaysToadd), Pending);
            }
        }

        public void GetVacation()
        {
            if (VacationVisibility == Visibility.Visible)
            {
                Loading = true;
                AttService.GetTblVacationForStoresAsync(new ObservableCollection<string>(TransactionHeader.SelectedMainRows.Select(e => e.EmpId)), new ObservableCollection<DateTime>(DaysToadd), Pending);
            }
        }

        public void GetMission()
        {
            if (MissionVisibility == Visibility.Visible)
            {
                Loading = true;
                AttService.GetTblMissionForStoresAsync(new ObservableCollection<string>(TransactionHeader.SelectedMainRows.Select(e => e.EmpId)), new ObservableCollection<DateTime>(DaysToadd), Pending);
            }
        }

        public void GetAttFile()
        {
            if (AttFileVisibility == Visibility.Visible)
            {
                Loading = true;
                AttService.GetTblAttendanceFileForEmpAsync(new ObservableCollection<string>(TransactionHeader.SelectedMainRows.Select(e => e.EmpId)), new ObservableCollection<DateTime>(DaysToadd), Pending);
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Emplid";

            Loading = true;
            Client.GetEmpTableAsync(DetailList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        public static List<DateTime> GetDates(int year, int weekNumber)
        {
            var start = new DateTime(year, 1, 1);
            start = start.AddDays(-((int)start.DayOfWeek));
            start = start.AddDays(7 * (weekNumber - 1));
            var listNum = Enumerable.Range(0, 7).Select(num => start.AddDays(num)).Where(x => x.Year == year).ToList();
            return listNum;
        }

        public void GetEmpByStore()
        {
            if (SelectedTab != null)
            {
                SelectedMainRowCopy = null;
                if (LoggedUserInfo.Company.Code == "HQ")
                {
                    Loading = true;
                    string transportationTemp = null;
                    string positionTemp = null;
                    if (TransactionHeader != null)
                    {

                        if (PositionAllowed)
                        {
                            if (!string.IsNullOrWhiteSpace(TransactionHeader.Position))
                            {
                                positionTemp = TransactionHeader.Position;
                            }
                        }
                        if (TransportationAllowed)
                        {
                            if (!string.IsNullOrWhiteSpace(TransactionHeader.Transportation))
                            {
                                transportationTemp = TransactionHeader.Transportation;
                            }
                        }
                        Client.GetEmpByAttOperatorAsync(positionTemp, transportationTemp, Code);
                    }
                }
                else
                {
                    if (TransactionHeader != null)
                    {

                        Loading = true;
                        Client.GetEmpTablebyStoreAndCompanyAsync(LoggedUserInfo.DatabasEname,
                            TransactionHeader.StorePerRow.code);
                    }
                }
            }
        }

        public int GetWeeksInYear(int year)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var date1 = new DateTime(year, 12, 31);
            var cal = dfi.Calendar;

            return cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                                dfi.FirstDayOfWeek);
        }

        private void GetDays()
        {
            var info = CultureInfo.CurrentUICulture.DateTimeFormat;
            Days = new List<DayClass>();
            for (var i = 0; i < info.DayNames.Count(); i++)
            {
                Days.Add(new DayClass
                {
                    Name = info.DayNames[i]
                }
                    );
            }
        }

        public enum AttSaveTypes
        {
            Shift,
            Excuse,
            Vacation,
            Mission,
            AttFile,
        }

        //public void SaveList(AttSaveTypes attSave)
        //{
        //    switch (attSave)
        //    {
        //        case AttSaveTypes.Excuse:
        //            var excuseToSave = new ObservableCollection<TblExcuse>();

        //            foreach (var row in TransactionHeader.SelectedMainRows)
        //            {
        //                foreach (var newrow in row.SelectedExcuses)
        //                {
        //                    if (newrow.CSPEXCUSEID != null && !string.IsNullOrWhiteSpace(newrow.CSPEXCUSEID) && (newrow.FromTime != null && newrow.ToTime != null))
        //                    {
        //                        if (ExcuseAllowAdd && newrow.Iserial == 0)
        //                        {
        //                            excuseToSave.Add(new TblExcuse().InjectFrom(newrow) as TblExcuse);
        //                        }
        //                        if (ExcuseAllowUpdate && newrow.Iserial != 0)
        //                        {
        //                            excuseToSave.Add(new TblExcuse().InjectFrom(newrow) as TblExcuse);
        //                        }
        //                    }
        //                }
        //            }
        //            if (excuseToSave.Any())
        //            {
        //                Loading = true;

        //                Client.UpdateAndInsertTblExcuseListAsync(excuseToSave);
        //            }

        //            break;

        //        case AttSaveTypes.Vacation:
        //            var vacationToSave = new ObservableCollection<TblVacation>();

        //            foreach (var row in TransactionHeader.SelectedMainRows)
        //            {
        //                foreach (var newrow in row.SelectedVacations)
        //                {
        //                    if (newrow.CSPVACATIONID != null && !string.IsNullOrWhiteSpace(newrow.CSPVACATIONID))
        //                    {
        //                        if (VacationAllowAdd && newrow.Iserial == 0)
        //                        {
        //                            vacationToSave.Add(new TblVacation().InjectFrom(newrow) as TblVacation);
        //                        }
        //                        if (VacationAllowUpdate && newrow.Iserial != 0)
        //                        {
        //                            vacationToSave.Add(new TblVacation().InjectFrom(newrow) as TblVacation);
        //                        }
        //                    }
        //                }
        //            }
        //            if (vacationToSave.Any())
        //            {
        //                Loading = true;
        //                Client.UpdateAndInsertTblVacationListAsync(vacationToSave);
        //            }

        //            break;

        //        case AttSaveTypes.Mission:
        //            var missionToSave = new ObservableCollection<TblMission>();

        //            foreach (var row in TransactionHeader.SelectedMainRows)
        //            {
        //                foreach (var newrow in row.SelectedMissions)
        //                {
        //                    if (newrow.CSPMISSIONID != null && !string.IsNullOrWhiteSpace(newrow.CSPMISSIONID) && (newrow.FromTime != null && newrow.ToTime != null))
        //                    {
        //                        if (MissionAllowAdd && newrow.Iserial == 0)
        //                        {
        //                            missionToSave.Add(new TblMission().InjectFrom(newrow) as TblMission);
        //                        }
        //                        if (MissionAllowUpdate && newrow.Iserial != 0)
        //                        {
        //                            missionToSave.Add(new TblMission().InjectFrom(newrow) as TblMission);
        //                        }
        //                    }
        //                }
        //            }
        //            if (missionToSave.Any())
        //            {
        //                Loading = true;
        //                Client.UpdateAndInsertTblMissionListAsync(missionToSave);
        //            }

        //            break;

        //        case AttSaveTypes.AttFile:
        //            var listAttFileToSave = new ObservableCollection<TblAttendanceFile>();

        //            foreach (var row in TransactionHeader.SelectedAttendanceFile)
        //            {
        //                if (row.Emplid != null && (row.FromTime != null || row.ToTime != null))
        //                {
        //                    if (AttFileAllowAdd && row.Iserial == 0)
        //                    {
        //                        listAttFileToSave.Add(new TblAttendanceFile().InjectFrom(row) as TblAttendanceFile);
        //                    }
        //                    if (AttFileAllowUpdate && row.Iserial != 0)
        //                    {
        //                        listAttFileToSave.Add(new TblAttendanceFile().InjectFrom(row) as TblAttendanceFile);
        //                    }
        //                }
        //            }
        //            if (listAttFileToSave.Any())
        //            {
        //                Loading = true;
        //                Client.UpdateAndInsertTblAttendanceFileListAsync(listAttFileToSave);
        //            }

        //            break;

        //        case AttSaveTypes.Shift:
        //            var listToSave = new ObservableCollection<TblEmployeeShift>();

        //            foreach (var row in TransactionHeader.SelectedMainRows)
        //            {
        //                foreach (var newrow in row.SelectedMainRows)
        //                {
        //                    if (newrow.TblEmployeeShiftLookup != null)
        //                    {
        //                        if (EmpShiftAllowAdd && newrow.Iserial == 0)
        //                        {
        //                            listToSave.Add(new TblEmployeeShift().InjectFrom(newrow) as TblEmployeeShift);
        //                        }
        //                        if (EmpShiftAllowUpdate && newrow.Iserial != 0)
        //                        {
        //                            listToSave.Add(new TblEmployeeShift().InjectFrom(newrow) as TblEmployeeShift);
        //                        }
        //                    }
        //                }
        //            }

        //            if (listToSave.Any())
        //            {
        //                Loading = true;
        //                Client.UpdateAndInsertTblEmployeeShiftListAsync(listToSave);
        //            }

        //            break;
        //    }
        //}

        internal void SaveRow(AttSaveTypes attSaveTypes)
        {
            if (attSaveTypes == AttSaveTypes.AttFile)
            {
                if (SelectedAttRule != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedAttRule, new ValidationContext(SelectedAttRule, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = SelectedAttRule.Iserial == 0;

                        var saveRow = new TblAttendanceFile();

                        saveRow.InjectFrom(SelectedAttRule);
                        if ((saveRow.FromTime != null || saveRow.ToTime != null) && saveRow.TblAttendanceFileReason != null)
                        {
                           
                            AttService.UpdateAndInsertTblAttendanceFileAsync(saveRow, save, TransactionHeader.SelectedAttendanceFile.IndexOf(SelectedAttRule), LoggedUserInfo.Iserial);
                        }
                    }
                }
            }
            else if (attSaveTypes == AttSaveTypes.Excuse)
            {
                if (selectedExcuse != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(selectedExcuse, new ValidationContext(selectedExcuse, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = selectedExcuse.Iserial == 0;
                        if (save)
                        {
                            selectedExcuse.CreatedBy = LoggedUserInfo.Iserial;
                            if (DayDate != null) selectedExcuse.CreationDate = DayDate.Value;
                        }
                        var saveRow = new TblExcuse();
                        saveRow.InjectFrom(selectedExcuse);
                        var tempempindex =
                            TransactionHeader.SelectedMainRows.IndexOf(
                                TransactionHeader.SelectedMainRows.FirstOrDefault(x => x.EmpId == selectedExcuse.Emplid));
                        var tempemp = TransactionHeader.SelectedMainRows.ElementAt(tempempindex);

                        AttService.UpdateAndInsertTblExcuseAsync(saveRow, save, TransactionHeader.SelectedMainRows.IndexOf(TransactionHeader.SelectedMainRows.FirstOrDefault(x => x.EmpId == selectedExcuse.Emplid)), tempemp.PeriodId, selectedExcuse.TransDate.Value.Month, LoggedUserInfo.Iserial);
                    }
                }
            }

            else if (attSaveTypes == AttSaveTypes.Mission)
            {
                if (selectedMission != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(selectedMission, new ValidationContext(selectedMission, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = selectedMission.Iserial == 0;
                        if (save)
                        {
                            selectedMission.CreatedBy = LoggedUserInfo.Iserial;
                            if (DayDate != null) selectedMission.CreationDate = DayDate.Value;
                        }
                        var saveRow = new TblMission();
                        saveRow.InjectFrom(selectedMission);
                        AttService.UpdateAndInsertTblMissionAsync(saveRow, save, TransactionHeader.SelectedMainRows.IndexOf(TransactionHeader.SelectedMainRows.FirstOrDefault(x => x.EmpId == selectedMission.Emplid)), LoggedUserInfo.Iserial);
                    }
                }
            }

            else if (attSaveTypes == AttSaveTypes.Vacation)
            {
                if (selectedVacation != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(selectedVacation, new ValidationContext(selectedVacation, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = selectedVacation.Iserial == 0;
                        if (save)
                        {
                            selectedVacation.CreatedBy = LoggedUserInfo.Iserial;
                            if (DayDate != null) selectedVacation.CreationDate = DayDate.Value;
                        }
                        var saveRow = new TblVacation();
                        saveRow.InjectFrom(selectedVacation);
                        AttService.UpdateAndInsertTblVacationAsync(saveRow, save, TransactionHeader.SelectedMainRows.IndexOf(TransactionHeader.SelectedMainRows.FirstOrDefault(x => x.EmpId == selectedVacation.Emplid)), LoggedUserInfo.Iserial);
                    }
                }
            }
            else if (attSaveTypes == AttSaveTypes.Shift)
            {
                if (selectedEmployeeShift != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(selectedEmployeeShift, new ValidationContext(selectedEmployeeShift, null, null), valiationCollection, true);

                    if (isvalid && selectedEmployeeShift.TblEmployeeShiftLookup != null)
                    {
                        var saveRow = new CRUDManagerService.TblEmployeeShift();
                        saveRow.InjectFrom(selectedEmployeeShift);
                        Client.UpdateAndInsertTblEmployeeShiftAsync(saveRow, TransactionHeader.SelectedMainRows.IndexOf(TransactionHeader.SelectedMainRows.FirstOrDefault(x => x.EmpId == selectedEmployeeShift.EmpId)), LoggedUserInfo.Iserial);
                    }
                }
            }
        }

        private ObservableCollection<TblStore> _storeList;

        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set
            {
                _storeList = value;
                RaisePropertyChanged("StoreList");
            }
        }

        private Visibility _transportationLineAllow;

        public Visibility TransportationLineAllow
        {
            get { return _transportationLineAllow; }
            set { _transportationLineAllow = value; RaisePropertyChanged("TransportationLineAllow"); }
        }

        public ObservableCollection<TblAttendanceFileReason> AttendanceFileReasonList { get; set; }

        private DateTime? _dayDateTime;

        public DateTime? DayDate
        {
            get { return _dayDateTime; }
            set
            {
                _dayDateTime = value; RaisePropertyChanged("DayDate");
                if (DayDate != null)
                {
                    var week = GetIso8601WeekOfYear((DateTime)DayDate);
                    if (WeekList != null && WeekList.Contains(week))
                    {
                        Week = week;
                    }
                }
            }
        }

        private string _transportation;

        public string Transportation
        {
            get { return _transportation; }
            set
            {
                _transportation = value; RaisePropertyChanged("Transportation");
                TransactionHeader.Transportation = Transportation;
                GetEmpByStore();
            }
        }

        private bool _transportationAllowed;

        public bool TransportationAllowed
        {
            get { return _transportationAllowed; }
            set
            {
                _transportationAllowed = value; RaisePropertyChanged("TransportationAllowed");
                GetEmpByStore();
            }
        }

        private bool _positionAllowed;

        public bool PositionAllowed
        {
            get { return _positionAllowed; }
            set
            {
                _positionAllowed = value; RaisePropertyChanged("PositionAllowed");
                GetEmpByStore();
            }
        }

        private string _position;

        public string Position
        {
            get { return _position; }
            set
            {
                _position = value; RaisePropertyChanged("Position");
                TransactionHeader.Position = Position;
                GetEmpByStore();
            }
        }

        private SortableCollectionView<EmployeesView> _detailList;

        public SortableCollectionView<EmployeesView> DetailList
        {
            get { return _detailList; }
            set
            {
                _detailList = value;
                RaisePropertyChanged("DetailList");
            }
        }

        private TblExcuseViewModel _selectedExcuse;

        public TblExcuseViewModel selectedExcuse
        {
            get { return _selectedExcuse; }
            set
            {
                _selectedExcuse = value;
                RaisePropertyChanged("selectedExcuse");
            }
        }

        private TblMissionViewModel _selectedMission;

        public TblMissionViewModel selectedMission
        {
            get { return _selectedMission; }
            set
            {
                _selectedMission = value;
                RaisePropertyChanged("selectedMission");
            }
        }

        private TblEmployeeshiftViewModel _selectedEmployeeShift;

        public TblEmployeeshiftViewModel selectedEmployeeShift
        {
            get { return _selectedEmployeeShift; }
            set
            {
                _selectedEmployeeShift = value;
                RaisePropertyChanged("selectedEmployeeShift");
            }
        }

        private TblVacationViewModel _selectedVacation;

        public TblVacationViewModel selectedVacation
        {
            get { return _selectedVacation; }
            set
            {
                _selectedVacation = value;
                RaisePropertyChanged("selectedVacation");
            }
        }

        private int? _week;

        public int? Week
        {
            get
            {
                return _week;
            }
            set
            {
                if ((_week.Equals(value) != true))
                {
                    _week = value;
                    //
                    TransactionHeader.Week = Week;

                    GetEmpByStore();
                    RaisePropertyChanged("Week");
                }
            }
        }

        private int? _yearField;

        public int? Year
        {
            get
            {
                return _yearField;
            }
            set
            {
                _yearField = value;

                if (Year != null)
                {
                    TransactionHeader.Year = value;
                    WeekList = new ObservableCollection<int>();
                    if (DaysToWork != 0)
                    {
                        var temp = Math.Ceiling((double)DaysToWork / 7);
                        if (DayDate != null)
                        {
                            var x = (int)(GetIso8601WeekOfYear((DateTime)DayDate) - temp);

                            for (var i = 0; i < GetWeeksInYear((int)Year); i++)
                            {
                                if (i >= x || NoDayLimit)
                                {
                                    WeekList.Add(i + 1);
                                }
                            }
                        }
                        if (DayDate != null) Week = GetIso8601WeekOfYear((DateTime)DayDate);
                    }
                }
                RaisePropertyChanged("Year");
            }
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        private TblEmployeeshiftViewModel _transactionHeader;

        private ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup> _employeeShiftLook;

        private List<DayClass> _days;

        public List<DayClass> Days
        {
            get { return _days; }
            set { _days = value; RaisePropertyChanged("Days"); }
        }

        public ObservableCollection<CRUDManagerService.TblEmployeeShiftLookup> EmployeeShiftLookUp
        {
            get { return _employeeShiftLook; }
            set { _employeeShiftLook = value; RaisePropertyChanged("EmployeeShiftLookUp"); }
        }

        private List<int> _yearList;

        public List<int> YearList
        {
            get { return _yearList; }
            set { _yearList = value; RaisePropertyChanged("YearList"); }
        }

        private bool _noDayLimit;

        public bool NoDayLimit
        {
            get { return _noDayLimit; }
            set
            {
                _noDayLimit = value;
                if (NoDayLimit)
                {
                    if (DayDate != null) Year = DayDate.Value.Year;
                }
            }
        }

        public bool NoDayLimitApproval { get; set; }

       


        private ObservableCollection<int> _weekList;

        public ObservableCollection<int> WeekList
        {
            get { return _weekList; }
            set { _weekList = value; RaisePropertyChanged("WeekList"); }
        }

        public TblEmployeeshiftViewModel TransactionHeader
        {
            get { return _transactionHeader; }
            set
            {
                _transactionHeader = value;
                RaisePropertyChanged("TransactionHeader");
            }
        }

        private ObservableCollection<MissionType> _ccmission;

        public ObservableCollection<MissionType> Ccmission
        {
            get
            {
                return _ccmission;
            }
            set
            {
                if ((ReferenceEquals(_ccmission, value) != true))
                {
                    _ccmission = value;
                    RaisePropertyChanged("Ccmission");
                }
            }
        }

        private ObservableCollection<VacationType> _ccVacation;

        public ObservableCollection<VacationType> CcVacation
        {
            get
            {
                return _ccVacation;
            }
            set
            {
                if ((ReferenceEquals(_ccVacation, value) != true))
                {
                    _ccVacation = value;
                    RaisePropertyChanged("CcVacation");
                }
            }
        }

        private ObservableCollection<TblExcuseRule> _excuseRuleList;

        public ObservableCollection<TblExcuseRule> ExcuseRuleList
        {
            get { return _excuseRuleList; }
            set { _excuseRuleList = value; RaisePropertyChanged("ExcuseRuleList"); }
        }

        private ObservableCollection<ExcuseType> _ccExcuse;

        public ObservableCollection<ExcuseType> CcExcuse
        {
            get
            {
                return _ccExcuse;
            }
            set
            {
                if ((ReferenceEquals(_ccExcuse, value) != true))
                {
                    _ccExcuse = value;
                    RaisePropertyChanged("CcExcuse");
                }
            }
        }

        private List<string> _transportationList;

        public List<string> TransportationList
        {
            get { return _transportationList; }
            set { _transportationList = value; RaisePropertyChanged("TransportationList"); }
        }

        private List<string> _positionList;

        public List<string> PositionList
        {
            get { return _positionList; }
            set { _positionList = value; RaisePropertyChanged("PositionList"); }
        }

        private Visibility _employeeShiftVisibility;

        public Visibility EmployeeShiftVisibility
        {
            get { return _employeeShiftVisibility; }
            set { _employeeShiftVisibility = value; RaisePropertyChanged("EmployeeShiftVisibility"); }
        }

        private Visibility _excuseVisibility;

        public Visibility ExcuseVisibility
        {
            get { return _excuseVisibility; }
            set { _excuseVisibility = value; RaisePropertyChanged("ExcuseVisibility"); }
        }

        private Visibility _missionVisibility;

        public Visibility MissionVisibility
        {
            get { return _missionVisibility; }
            set { _missionVisibility = value; RaisePropertyChanged("MissionVisibility"); }
        }

        private Visibility _vacationVisibility;

        public Visibility VacationVisibility
        {
            get { return _vacationVisibility; }
            set { _vacationVisibility = value; RaisePropertyChanged("VacationVisibility"); }
        }

        private Visibility _employeeShiftStatusVisibility;

        public Visibility EmployeeShiftStatusVisibility
        {
            get { return _employeeShiftStatusVisibility; }
            set { _employeeShiftStatusVisibility = value; RaisePropertyChanged("EmployeeShiftStatusVisibility"); }
        }

        private Visibility _attFileVisibility;

        public Visibility AttFileVisibility
        {
            get { return _attFileVisibility; }
            set { _attFileVisibility = value; RaisePropertyChanged("AttFileVisibility"); }
        }

        private Visibility _excuseStatusVisibility;

        public Visibility ExcuseStatusVisibility
        {
            get { return _excuseStatusVisibility; }
            set { _excuseStatusVisibility = value; RaisePropertyChanged("ExcuseStatusVisibility"); }
        }

        private Visibility _missionStatusVisibility;

        public Visibility MissionStatusVisibility
        {
            get { return _missionStatusVisibility; }
            set { _missionStatusVisibility = value; RaisePropertyChanged("MissionStatusVisibility"); }
        }

        private Visibility _excuseStatusSelfVisibility;

        public Visibility ExcuseStatusSelfVisibility
        {
            get { return _excuseStatusSelfVisibility; }
            set { _excuseStatusSelfVisibility = value; RaisePropertyChanged("ExcuseStatusSelfVisibility"); }
        }

        private Visibility _missionStatusSelfVisibility;

        public Visibility MissionStatusSelfVisibility
        {
            get { return _missionStatusSelfVisibility; }
            set { _missionStatusSelfVisibility = value; RaisePropertyChanged("MissionStatusSelfVisibility"); }
        }

        private Visibility _vacationStatusSelfVisibility;

        public Visibility VacationStatusSelfVisibility
        {
            get { return _vacationStatusSelfVisibility; }
            set { _vacationStatusSelfVisibility = value; RaisePropertyChanged("VacationStatusSelfVisibility"); }
        }

        private Visibility _vacationStatusVisibility;

        public Visibility VacationStatusVisibility
        {
            get { return _vacationStatusVisibility; }
            set { _vacationStatusVisibility = value; RaisePropertyChanged("VacationStatusVisibility"); }
        }

        private Visibility _pendingVisibility;

        public Visibility PendingVisibility
        {
            get { return _pendingVisibility; }
            set { _pendingVisibility = value; RaisePropertyChanged("PendingVisibility"); }
        }

        private Visibility _attFileStatusVisibility;

        public Visibility AttFileStatusVisibility
        {
            get { return _attFileStatusVisibility; }
            set { _attFileStatusVisibility = value; RaisePropertyChanged("AttFileStatusVisibility"); }
        }

        private Visibility _attStatusSelfVisibility;

        public Visibility AttStatusSelfVisibility
        {
            get { return _attStatusSelfVisibility; }
            set { _attStatusSelfVisibility = value; RaisePropertyChanged("AttStatusSelfVisibility"); }
        }

        private Visibility _storeVisibility;

        public Visibility StoreVisibility
        {
            get { return _storeVisibility; }
            set { _storeVisibility = value; RaisePropertyChanged("StoreVisibility"); }
        }

        private Visibility _positionVisibility;

        public Visibility PositionVisibility
        {
            get { return _positionVisibility; }
            set { _positionVisibility = value; RaisePropertyChanged("PositionVisibility"); }
        }

        private TblEmployeeshiftViewModel _selectedMainRow;

        public TblEmployeeshiftViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private bool _pending;

        public bool Pending
        {
            get { return _pending; }
            set
            {
                _pending = value; RaisePropertyChanged("Pending");
                GetEmpByStore();
            }
        }

        private TblEmployeeshiftViewModel _selectedMainRowCopy;

        public TblEmployeeshiftViewModel SelectedMainRowCopy
        {
            get { return _selectedMainRowCopy; }
            set
            {
                _selectedMainRowCopy = value;
                RaisePropertyChanged("SelectedMainRowCopy");
            }
        }

        private TblAttendanceFileViewModel _selectedAttRule;

        public TblAttendanceFileViewModel SelectedAttRule
        {
            get { return _selectedAttRule; }
            set
            {
                _selectedAttRule = value;
                RaisePropertyChanged("SelectedAttRule");
            }
        }

        public bool Changed { get; set; }

        private bool _empShiftAllowAdd;

        private Visibility _empShiftAllowDelete;

        private bool _empShiftAllowUpdate;

        public bool EmpShiftAllowAdd
        {
            get { return _empShiftAllowAdd; }
            set { _empShiftAllowAdd = value; RaisePropertyChanged("EmpShiftAllowAdd"); }
        }

        public Visibility EmpShiftAllowDelete
        {
            get { return _empShiftAllowDelete; }
            set { _empShiftAllowDelete = value; RaisePropertyChanged("EmpShiftAllowDelete"); }
        }

        public bool EmpShiftAllowUpdate
        {
            get { return _empShiftAllowUpdate; }
            set { _empShiftAllowUpdate = value; RaisePropertyChanged("EmpShiftAllowUpdate"); }
        }

        private bool _vacationAllowAdd;

        private Visibility _vacationAllowDelete;

        private bool _vacationAllowUpdate;

        public bool VacationAllowAdd
        {
            get { return _vacationAllowAdd; }
            set { _vacationAllowAdd = value; RaisePropertyChanged("VacationAllowAdd"); }
        }

        public Visibility VacationAllowDelete
        {
            get { return _vacationAllowDelete; }
            set { _vacationAllowDelete = value; RaisePropertyChanged("VacationAllowDelete"); }
        }

        public bool VacationAllowUpdate
        {
            get { return _vacationAllowUpdate; }
            set { _vacationAllowUpdate = value; RaisePropertyChanged("VacationAllowUpdate"); }
        }

        private bool _excuseAllowAdd;

        private Visibility _excuseAllowDelete;

        private bool _excuseAllowUpdate;

        public bool ExcuseAllowAdd
        {
            get { return _excuseAllowAdd; }
            set { _excuseAllowAdd = value; RaisePropertyChanged("ExcuseAllowAdd"); }
        }

        public Visibility ExcuseAllowDelete
        {
            get { return _excuseAllowDelete; }
            set { _excuseAllowDelete = value; RaisePropertyChanged("ExcuseAllowDelete"); }
        }

        public bool ExcuseAllowUpdate
        {
            get { return _excuseAllowUpdate; }
            set { _excuseAllowUpdate = value; RaisePropertyChanged("ExcuseAllowUpdate"); }
        }

        private bool _missionAllowAdd;

        private Visibility _missionAllowDelete;

        private bool _missionAllowUpdate;

        public bool MissionAllowAdd
        {
            get { return _missionAllowAdd; }
            set { _missionAllowAdd = value; RaisePropertyChanged("MissionAllowAdd"); }
        }

        public Visibility MissionAllowDelete
        {
            get { return _missionAllowDelete; }
            set { _missionAllowDelete = value; RaisePropertyChanged("MissionAllowDelete"); }
        }

        public bool MissionAllowUpdate
        {
            get { return _missionAllowUpdate; }
            set { _missionAllowUpdate = value; RaisePropertyChanged("MissionAllowUpdate"); }
        }

        private bool _attFileAllowAdd;

        private Visibility _attFileAllowDelete;

        private bool _attFileAllowUpdate;

        public bool AttFileAllowAdd
        {
            get { return _attFileAllowAdd; }
            set { _attFileAllowAdd = value; RaisePropertyChanged("AttFileAllowAdd"); }
        }

        public Visibility AttFileAllowDelete
        {
            get { return _attFileAllowDelete; }
            set { _attFileAllowDelete = value; RaisePropertyChanged("AttFileAllowDelete"); }
        }

        public bool AttFileAllowUpdate
        {
            get { return _attFileAllowUpdate; }
            set { _attFileAllowUpdate = value; RaisePropertyChanged("AttFileAllowUpdate"); }
        }

        public string SelectedTab { get; set; }

        public void DeleteExcuse(TblExcuseViewModel row)
        {
            if (row.Iserial != 0)
            {
                if (row.ApprovedBy == LoggedUserInfo.Iserial || row.ApprovedBy == 0 || row.ApprovedBy == null || row.Status == 0)
                {
                    AttService.DeleteTblExcuseAsync(new TblExcuse().InjectFrom(row) as TblExcuse);
                }
                else
                {
                    MessageBox.Show("Cannot Delete A Record Approved By Another Person");
                }
            }
            else
            {
                row.FromTime = row.ToTime = null;
                row.CSPEXCUSEID = null;
            }
        }

        public void DeleteVacation(TblVacationViewModel row)
        {
            if (row.Iserial != 0)
            {
                if (row.ApprovedBy == LoggedUserInfo.Iserial || row.ApprovedBy == 0 || row.ApprovedBy == null || row.Status == 0)
                {
                    AttService.DeleteTblVacationAsync(new TblVacation().InjectFrom(row) as TblVacation);
                }
                else
                {
                    MessageBox.Show("Cannot Delete A Record Approved By Another Person");
                }
            }

            else
            {
                row.CSPVACATIONID = null;
            }
        }

        public void DeleteEmployeeShift(TblEmployeeshiftViewModel row)
        {
            if (row.Iserial != 0)
            {
                Client.DeleteTblEmployeeShiftAsync(new CRUDManagerService.TblEmployeeShift().InjectFrom(row) as CRUDManagerService.TblEmployeeShift);
            }
            else
            {
                row.TblEmployeeShiftLookup = null;
            }
        }

        public void DeleteMission(TblMissionViewModel row)
        {
            if (row.Iserial != 0)
            {
                if (row.ApprovedBy == LoggedUserInfo.Iserial || row.ApprovedBy == 0 || row.ApprovedBy == null || row.Status == 0)
                {
                    AttService.DeleteTblMissionAsync(new TblMission().InjectFrom(row) as TblMission);
                }
                else
                {
                    MessageBox.Show("Cannot Delete A Record Approved By Another Person");
                }
            }
            else
            {
                row.CSPMISSIONID = null;
                row.FromTime = row.ToTime = null;
            }
        }

        public void DeleteAttFile(TblAttendanceFileViewModel row)
        {
            if (row.Iserial != 0)
            {
                AttService.DeleteTblAttendanceFileAsync(new TblAttendanceFile().InjectFrom(row) as TblAttendanceFile);
            }
            else
            {
                row.FromTime = row.ToTime = null;
            }
        }
    }
}