using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.Gl.ChildWindow;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.Gl
{
    public class PeriodGlViewModel : ViewModelBase
    {
        public PeriodGlViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.PeriodsGl.ToString());

                MainRowList = new ObservableCollection<TblPeriodViewModel>();
                SelectedMainRow = new TblPeriodViewModel();

                Glclient.UpdateOrInsertCspPeriodLineCompleted += (s, sv) =>
                 {
                     Loading = false;
                 };

                Glclient.GetTblPeriodCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPeriodViewModel();
                        newrow.InjectFrom(row);

                        if (row.TblAccount1 != null)
                        {
                            newrow.AccountPerRow = new TblAccount();
                            newrow.AccountPerRow = row.TblAccount1;
                        }

                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblperiodCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                };
                Glclient.DeleteTblPeriodCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Glclient.GetTblPeriodLineCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    SelectedMainRow.TblPeriodLineList = new ObservableCollection<TblPeriodLineViewModel>();
                    GenericMapper.InjectFromObCollection(SelectedMainRow.TblPeriodLineList, ev.Result);

                    var childWindow = new PeriodLinesChildWindow(this);
                    childWindow.Show();
                };
                Glclient.ClosePeriodCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    var Period = MainRowList.FirstOrDefault(w => w.Iserial == ev.Result.Iserial);
                    Period.InjectFrom(ev.Result);
                    MessageBox.Show("Post Completed");
                    //SelectedMainRow.TblPeriodLineList = new ObservableCollection<TblPeriodLineViewModel>();
                    //GenericMapper.InjectFromObCollection(SelectedMainRow.TblPeriodLineList, ev.Result);

                };


                Glclient.ClosePeriodlineCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    var PeriodLine = SelectedMainRow.TblPeriodLineList.FirstOrDefault(w => w.Iserial == ev.Result.Iserial);
                    PeriodLine.InjectFrom(ev.Result);
                    MessageBox.Show("Post Completed");
                    //SelectedMainRow.TblPeriodLineList = new ObservableCollection<TblPeriodLineViewModel>();
                    //GenericMapper.InjectFromObCollection(SelectedMainRow.TblPeriodLineList, ev.Result);

                };
                
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Glclient.GetTblPeriodAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblPeriodViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        internal void ClosePeriod(TblPeriodViewModel selectedrow)
        {
            Glclient.ClosePeriodAsync(selectedrow.Iserial, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
        }

        internal void ClosePeriodLine(TblPeriodLineViewModel selectedrow)
        {
            Glclient.ClosePeriodlineAsync(selectedrow.Iserial, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
        }

        

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var save = SelectedMainRow.Iserial == 0;

                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }

                if (AllowUpdate != true && !save)
                {
                    MessageBox.Show(strings.AllowUpdateMsg);
                    return;
                }
                var saveRow = new TblGlPeriod();
                saveRow.InjectFrom(SelectedMainRow);
                Glclient.UpdateOrInsertTblperiodAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }

                            Glclient.DeleteTblPeriodAsync((TblGlPeriod)new TblGlPeriod().InjectFrom(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void GeneratePeriodLines(DateTime fromDate, DateTime toDate)
        {
            Glclient.GeneratePeriodLinesAsync((TblGlPeriod)new TblGlPeriod().InjectFrom(SelectedMainRow), fromDate, toDate, LoggedUserInfo.DatabasEname);
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;                   
                    if (AllowUpdate != true )
                    {

                        return;
                    }
                    var rowToSave = new TblGlPeriodLine();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertCspPeriodLineAsync(rowToSave, save,
                            SelectedMainRow.TblPeriodLineList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname,
                            LoggedUserInfo.Iserial);
                    }
                }
            }
        }

        #region Prop

        private ObservableCollection<TblPeriodViewModel> _mainRowList;

        public ObservableCollection<TblPeriodViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPeriodViewModel> _selectedMainRows;

        public ObservableCollection<TblPeriodViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPeriodViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPeriodViewModel _selectedMainRow;

        public TblPeriodViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblPeriodLineViewModel _selectedDetailRow;

        public TblPeriodLineViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop

        internal void ShowPeriodLines()
        {
            if (SelectedMainRow.Iserial != 0)
            {
                Glclient.GetTblPeriodLineAsync(0, PageSize, SelectedMainRow.Iserial, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);
            }
            else
            {
                var childWindow = new GeneratePeroidLinesChildWindow(this);
                childWindow.Show();

                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                        MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                }
            }
        }
    }

    public class TblPeriodViewModel : GenericViewModel
    {

        private int? _TblAccount;

        public int? TblAccount
        {
            get { return _TblAccount; }
            set { _TblAccount = value; RaisePropertyChanged("TblAccount"); }
        }

        private TblAccount _AccountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _AccountPerRow; }
            set
            {
                _AccountPerRow = value; RaisePropertyChanged("AccountPerRow");
                TblAccount = _AccountPerRow.Iserial;
            }
        }

        private bool _closed;

        [ReadOnly(true)]
        public bool Closed
        {
            get { return _closed; }
            set { _closed = value; RaisePropertyChanged("Closed"); }
        }

        private DateTime? _closedDate;

        public DateTime? ClosedDate
        {
            get { return _closedDate; }
            set { _closedDate = value; RaisePropertyChanged("ClosedDate"); }
        }

        private bool _insuraneField;

        private int _periodUnitField;

        private bool _taxesField;

        private ObservableCollection<TblPeriodLineViewModel> _tblPeriodLinesField;

        public ObservableCollection<TblPeriodLineViewModel> TblPeriodLineList
        {
            get { return _tblPeriodLinesField; }
            set
            {
                _tblPeriodLinesField = value;
                RaisePropertyChanged("TblPeriodLineList");
            }
        }

        public bool Insurane
        {
            get { return _insuraneField; }
            set
            {
                if ((_insuraneField.Equals(value) != true))
                {
                    _insuraneField = value;
                    RaisePropertyChanged("Insurane");
                }
            }
        }

        public int PeriodUnit
        {
            get { return _periodUnitField; }
            set
            {
                if ((_periodUnitField.Equals(value) != true))
                {
                    _periodUnitField = value;
                    RaisePropertyChanged("PeriodUnit");
                }
            }
        }

        public bool Taxes
        {
            get { return _taxesField; }
            set
            {
                if ((_taxesField.Equals(value) != true))
                {
                    _taxesField = value;
                    RaisePropertyChanged("Taxes");
                }
            }
        }
    }

    public class TblPeriodLineViewModel : PropertiesViewModelBase
    {

        private int _iserialField;

        private string _aname;

        private string _ename;

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
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get { return _aname; }
            set
            {
                if ((ReferenceEquals(_aname, value) != true))
                {
                    if (value != null) _aname = value.Trim();
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get { return _ename; }
            set
            {
                if ((ReferenceEquals(_ename, value) != true))
                {
                    if (value != null) _ename = value.Trim();
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
                }
            }
        }

        private ObservableCollection<TblPeriodLineCalcViewModel> _detailsList;

        public ObservableCollection<TblPeriodLineCalcViewModel> DetailsList
        {
            get { return _detailsList; }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        private bool _approvedField;

        private bool _attCalculatedField;

        private bool _payrollCalculated;

        private DateTime? _fromDateField;

        private bool _lockedField;

        private int _tblPeriodField;

        private DateTime? _toDateField;

        private bool _dueField;

        public bool Approved
        {
            get { return _approvedField; }
            set
            {
                if ((_approvedField.Equals(value) != true))
                {
                    _approvedField = value;
                    RaisePropertyChanged("Approved");
                }
            }
        }

        public bool AttCalculated
        {
            get { return _attCalculatedField; }
            set
            {
                if ((_attCalculatedField.Equals(value) != true))
                {
                    _attCalculatedField = value;
                    RaisePropertyChanged("AttCalculated");
                }
            }
        }

        public bool PayrollCalculated
        {
            get { return _payrollCalculated; }
            set
            {
                if ((_payrollCalculated.Equals(value) != true))
                {
                    _payrollCalculated = value;
                    RaisePropertyChanged("PayrollCalculated");
                }
            }
        }

        public DateTime? FromDate
        {
            get { return _fromDateField; }
            set
            {
                if ((_fromDateField.Equals(value) != true))
                {
                    _fromDateField = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public bool Locked
        {
            get { return _lockedField; }
            set
            {
                if ((_lockedField.Equals(value) != true))
                {
                    _lockedField = value;
                    RaisePropertyChanged("Locked");
                }
            }
        }

        public int TblPeriod
        {
            get { return _tblPeriodField; }
            set
            {
                if ((_tblPeriodField.Equals(value) != true))
                {
                    _tblPeriodField = value;
                    RaisePropertyChanged("TblPeriod");
                }
            }
        }

        public DateTime? ToDate
        {
            get { return _toDateField; }
            set
            {
                if ((_toDateField.Equals(value) != true))
                {
                    _toDateField = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        public bool Due
        {
            get { return _dueField; }
            set
            {
                if ((_dueField.Equals(value) != true))
                {
                    _dueField = value;
                    RaisePropertyChanged("Due");
                }
            }
        }
    }

    public class TblPeriodLineCalcViewModel : GenericViewModel
    {
        private int _calcTypeField;

        private int _tblDepartmentField;

        private int _tblPeriodLineField;

        private TblGlPeriodLine _tblPeriodLine1Field;

        public int CalcType
        {
            get
            {
                return _calcTypeField;
            }
            set
            {
                if ((_calcTypeField.Equals(value) != true))
                {
                    _calcTypeField = value;
                    RaisePropertyChanged("CalcType");
                }
            }
        }

        public int TblDepartment
        {
            get
            {
                return _tblDepartmentField;
            }
            set
            {
                if ((_tblDepartmentField.Equals(value) != true))
                {
                    _tblDepartmentField = value;
                    RaisePropertyChanged("TblDepartment");
                }
            }
        }

        public int TblPeriodLine
        {
            get
            {
                return _tblPeriodLineField;
            }
            set
            {
                if ((_tblPeriodLineField.Equals(value) != true))
                {
                    _tblPeriodLineField = value;
                    RaisePropertyChanged("TblPeriodLine");
                }
            }
        }

        public TblGlPeriodLine PeriodLinePerRow
        {
            get
            {
                return _tblPeriodLine1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblPeriodLine1Field, value) != true))
                {
                    _tblPeriodLine1Field = value;
                    RaisePropertyChanged("PeriodLinePerRow");
                }
            }
        }
    }
}