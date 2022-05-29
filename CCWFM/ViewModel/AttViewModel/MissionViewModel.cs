using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.AttService;

namespace CCWFM.ViewModel.AttViewModel
{
    public class TblMissionViewModel : PropertiesViewModelBase
    {
        private SolidColorBrush _backgroundColor;

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; RaisePropertyChanged("BackgroundColor"); }
        }

        private string _cspmissionidField;

        private string _descriptionField;

        private string _emplidField;

        private DateTime? _fromDateField;

        private int? _fromTimeField;

        private int _iserialField;

        private int _statusField;

        private DateTime? _toDateField;

        private int? _toTimeField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMissionType")]
        public string CSPMISSIONID
        {
            get
            {
                return _cspmissionidField;
            }
            set
            {
                if ((ReferenceEquals(_cspmissionidField, value) != true))
                {
                    _cspmissionidField = value;
                    RaisePropertyChanged("CSPMISSIONID");
                    if (LoggedUserInfo.Code == "0140" && !string.IsNullOrEmpty(_cspmissionidField))
                    {
                        FromTime = 28800;
                        ToTime = 57600;
                    }
                    if (LoggedUserInfo.Code == "1002" && !string.IsNullOrEmpty(_cspmissionidField))
                    {
                        FromTime = 28800;
                        ToTime = 57600;
                    }
                }
            }
        }

        private bool _containMultipleTransaction;

        public bool ContainMultipleTransaction
        {
            get { return _containMultipleTransaction; }
            set { _containMultipleTransaction = value; RaisePropertyChanged("ContainMultipleTransaction"); }
        }

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromDate")]
        public DateTime? FromDate
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromTime")]
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

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private Visibility _missionStatusVisibility;

        public Visibility MissionStatusVisibility
        {
            get { return _missionStatusVisibility; }
            set { _missionStatusVisibility = value; RaisePropertyChanged("MissionStatusVisibility"); }
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToDate")]
        public DateTime? ToDate
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToTime")]
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

        private bool _loading;

        public bool Loaded
        {
            get { return _loading; }
            set { _loading = value; RaisePropertyChanged("Loaded"); }
        }

        private Visibility _isNotExtraMission;

        public Visibility IsNotExtraMission
        {
            get { return _isNotExtraMission; }
            set { _isNotExtraMission = value; RaisePropertyChanged("IsNotExtraMission"); }
        }
    }

    public class MissionViewModel : ViewModelBase
    {  
        AttServiceClient AttService = new AttServiceClient();


        public MissionViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblMissionViewModel>();
                SelectedMainRows = new ObservableCollection<TblMissionViewModel>();

                AttService.MissionTypeCompleted += (x, y) =>
                {
                    Ccmission = y.Result;
                };
                AttService.MissionTypeAsync();

                AttService.UpdateAndInsertTblMissionCompleted += (x, y) =>
                {
                    var savedRow = (TblMissionViewModel)MainRowList.GetItemAt(y.outindex);

                    if (savedRow != null) savedRow.InjectFrom(y.Result);
                };

                AttService.GetTblMissionCompleted += (x, y) =>
                {
                    foreach (var row in y.Result)
                    {
                        var newrow = new TblMissionViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                };
                AttService.DeleteTblMissionCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        if (row.Iserial != 0)
                        {
                            AttService.DeleteTblMissionAsync(
                                (TblMission)new TblMission().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRows != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRows, new ValidationContext(SelectedMainRows, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    SelectedMainRow.Emplid = LoggedUserInfo.Code;
                    var saveRow = new TblMission();
                    saveRow.InjectFrom(SelectedMainRow);

                    AttService.UpdateAndInsertTblMissionAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new TblMissionViewModel
                {
                    Emplid = LoggedUserInfo.Code
                });
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            AttService.GetTblMissionAsync(MainRowList.Count, PageSize, LoggedUserInfo.Code, SortBy, Filter, ValuesObjects);
        }

        private SortableCollectionView<TblMissionViewModel> _mainRowList;

        public SortableCollectionView<TblMissionViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblMissionViewModel> _selectedMainRows;

        public ObservableCollection<TblMissionViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblMissionViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblMissionViewModel _selectedMainRow;

        public TblMissionViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
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
    }
}