using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.AttService;

namespace CCWFM.ViewModel.AttViewModel
{
    public class TblVacationViewModel : CRUDManagerService.PropertiesViewModelBase
    {
        private string _cspvacationidField;

        private int _daysField;

        private string _descriptionField;

        private string _emplidField;

        private DateTime? _fromDateField;

        private int _iserialField;

        private int _statusField;

        private DateTime? _toDateField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqVacationType")]
        public string CSPVACATIONID
        {
            get { return _cspvacationidField; }
            set
            {
                if ((ReferenceEquals(_cspvacationidField, value) != true))
                {
                    _cspvacationidField = value;
                    RaisePropertyChanged("CSPVACATIONID");
                }
            }
        }

        [ReadOnly(true)]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDays")]
        public int DAYS
        {
            get { return _daysField; }
            set
            {
                if ((_daysField.Equals(value) != true))
                {
                    _daysField = value;
                    RaisePropertyChanged("DAYS");
                }
            }
        }

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDescription")]
        public string Description
        {
            get { return _descriptionField; }
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
            get { return _emplidField; }
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
            get { return _fromDateField; }
            set
            {
                if ((_fromDateField.Equals(value) != true))
                {
                    if (ToDate != null && FromDate != null)
                    {
                        if (value > ToDate)
                        {
                            value = ToDate;
                        }
                    }
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                    if (ToDate != null && FromDate != null)
                    {
                        var tempDays = ToDate.Value.Subtract(FromDate.Value);
                        DAYS = tempDays.Days;
                    }
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        private Visibility _vacationStatusVisibility;

        public Visibility VacationStatusVisibility
        {
            get { return _vacationStatusVisibility; }
            set { _vacationStatusVisibility = value; RaisePropertyChanged("VacationStatusVisibility"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

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
                        if (Status == 2)
                        {
                            BackgroundColor = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
            }
        }

        private SolidColorBrush _backgroundColor;

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; RaisePropertyChanged("BackgroundColor"); }
        }

        private bool _loading;

        public bool Loaded
        {
            get { return _loading; }
            set { _loading = value; RaisePropertyChanged("Loaded"); }
        }

        private VacationType _vacationPerRow;

        public VacationType VacationPerRow
        {
            get { return _vacationPerRow; }
            set { _vacationPerRow = value; RaisePropertyChanged("VacationPerRow"); }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToDate")]
        public DateTime? ToDate
        {
            get { return _toDateField; }
            set
            {
                if ((_toDateField.Equals(value) != true))
                {
                    if (ToDate != null && FromDate != null)
                    {
                        if (value < FromDate)
                        {
                            value = FromDate;
                        }
                    }
                    _toDateField = value;
                    RaisePropertyChanged("ToDate");
                    if (ToDate != null && FromDate != null)
                    {
                        var tempDays = ToDate.Value.Subtract(FromDate.Value);
                        DAYS = tempDays.Days;
                    }
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
    }

    public class VacationViewModel : ViewModelBase
    {
        AttServiceClient AttService = new AttServiceClient();

        public VacationViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblVacationViewModel>();
                SelectedMainRows = new ObservableCollection<TblVacationViewModel>();

                AttService.VacationTypeCompleted += (x, y) =>
                {
                    CcVacation = y.Result;
                };

                AttService.UpdateAndInsertTblVacationCompleted += (x, y) =>
                {
                    var savedRow = (TblVacationViewModel)MainRowList.GetItemAt(y.outindex);

                    if (savedRow != null) savedRow.InjectFrom(y.Result);
                };

                AttService.GetTblVacationCompleted += (x, y) =>
                {
                    foreach (var row in y.Result)
                    {
                        var newrow = new TblVacationViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                };
                AttService.DeleteTblVacationCompleted += (s, ev) =>
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
                            AttService.DeleteTblVacationAsync(
                                (TblVacation)new TblVacation().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void SaveMainRowExc()
        {
            if (SelectedMainRows != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRows, new ValidationContext(SelectedMainRows, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    SelectedMainRow.Emplid = LoggedUserInfo.Code;
                    var saveRow = new AttService.TblVacation();
                    saveRow.InjectFrom(SelectedMainRow);

                    AttService.UpdateAndInsertTblVacationAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
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
                MainRowList.Insert(currentRowIndex + 1, new TblVacationViewModel
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

        private SortableCollectionView<TblVacationViewModel> _mainRowList;

        public SortableCollectionView<TblVacationViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblVacationViewModel> _selectedMainRows;

        public ObservableCollection<TblVacationViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblVacationViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblVacationViewModel _selectedMainRow;

        public TblVacationViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
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
    }
}