using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblVariableTermManualFactoryViewModel : PropertiesViewModelBase
    {
        private TblSalaryTerm _tblSalaryTermPerRow;

        public TblSalaryTerm SalaryTerPerRow
        {
            get { return _tblSalaryTermPerRow; }
            set
            {
                _tblSalaryTermPerRow = value;
                if (_tblSalaryTermPerRow != null) TermId = _tblSalaryTermPerRow.Iserial;
                RaisePropertyChanged("SalaryTerPerRow");
            }
        }

        private EmployeesView _emp;

        public EmployeesView EmpPerRow
        {
            get { return _emp; }
            set
            {
                _emp = value;
                RaisePropertyChanged("EmpPerRow");
            }
        }

        private double _daysField;

        private string _emplidField;

        private int _iserialField;

        private int? _termIdField;

        private DateTime? _transDateField;

        private int _statusField;

        public double Days
        {
            get
            {
                return _daysField;
            }
            set
            {
                if ((_daysField.Equals(value) != true))
                {
                    _daysField = value;
                    RaisePropertyChanged("Days");
                }
            }
        }

        private double _hours;
        public double Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                if ((_hours.Equals(value) != true))
                {
                    _hours = value;
                    RaisePropertyChanged("Hours");
                }
            }
        }


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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSalaryTerm")]
        public int? TermId
        {
            get
            {
                return _termIdField;
            }
            set
            {
                if ((_termIdField.Equals(value) != true))
                {
                    _termIdField = value;
                    RaisePropertyChanged("TermId");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
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

        public int status
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
                    RaisePropertyChanged("status");
                }
            }
        }

        private string _position;

        public string Position
        {
            get { return _position; }
            set { _position = value; RaisePropertyChanged("Position"); }
        }

        private string _transportation;

        public string Transportation
        {
            get { return _transportation; }
            set { _transportation = value; RaisePropertyChanged("Transportation"); }
        }

    }

    public class VariableTermManualFactoryViewModel : ViewModelBase
    {
        public VariableTermManualFactoryViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                PositionAllowed = true;
                TransportationLineAllow = Visibility.Visible;
                MainRowList = new SortableCollectionView<TblVariableTermManualFactoryViewModel>();
                SelectedMainRow = new TblVariableTermManualFactoryViewModel();
                GetPoisition(LoggedUserInfo.Code);
                GetEmpTransportationLine(LoggedUserInfo.Code);
                Client.GetEmpPositionCompleted += (s, sv) =>
                {
                    PositionList = sv.Result.ToList();
                    if (PositionList.Count() == 1)
                    {
                        SelectedMainRow.Position = PositionList.FirstOrDefault();
                        Position = PositionList.FirstOrDefault();
                    }
                };
                Client.GetEmpTransportationLineCompleted += (s, sv) =>
                {
                    TransportationList = sv.Result.ToList();
                    if (TransportationList.Count() == 1)
                    {
                        SelectedMainRow.Transportation = TransportationList.FirstOrDefault();
                        Transportation = TransportationList.FirstOrDefault();
                    }
                };

                Client.GetTblVariableTermManualCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblVariableTermManualFactoryViewModel
                        {
                            EmpPerRow = new EmployeesView(),
                            SalaryTerPerRow = new TblSalaryTerm()
                        };

                        newrow.EmpPerRow = EmpList.FirstOrDefault(w => w.Emplid == row.Emplid);
                        newrow.SalaryTerPerRow = SalaryTermList.FirstOrDefault(w => w.Iserial == row.TermId);
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;

                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                StoreVisibility = LoggedUserInfo.Company.Code == "HQ" ? Visibility.Collapsed : Visibility.Visible;
                PositionVisibility = LoggedUserInfo.Company.Code != "HQ" ? Visibility.Collapsed : Visibility.Visible;

                Client.UpdateOrInsertTblVariableTermManualCompleted += (s, x) =>
                {
                    var savedRow = (TblVariableTermManualFactoryViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblVariableTermManualCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.GetEmpByAttOperatorCompleted += (s, sv) =>
                {
                    MainRowList.Clear();
                    foreach (var emp in sv.Result)
                    {
                        MainRowList.Add(new TblVariableTermManualFactoryViewModel
                        {
                            EmpPerRow = emp,
                            Emplid = emp.Emplid,
                            Position = emp.Position,
                            Transportation = emp.TransportationLine,
                            TransDate = DayDate,
                            SalaryTerPerRow = SalaryTerPerRow,
                            Hours = Hours,
                            Days = Days
                        });
                    }

                    Loading = false;
                };

                Client.GetTblSalaryTermAsync();

                Client.GetTblSalaryTermCompleted += (s, sv) =>
                {
                    SalaryTermList = sv.Result;
                };
                Client.GetEmpTablebyStoreAndCompanyCompleted += (s, sv) =>
                {
                    EmpList = new ObservableCollection<EmployeesView>();
                    EmpList = sv.Result;
                    Loading = false;

                    Client.GetTblVariableTermManualAsync(new ObservableCollection<string>(EmpList.Select(x => x.Emplid)));
                };
            }
        }

        public void GetPoisition(string code)
        {
            Client.GetEmpPositionAsync(code);
        }

        public void GetEmpTransportationLine(string code)
        {
            Client.GetEmpTransportationLineAsync(code);
        }

        public void GetEmpByStore()
        {
            if (LoggedUserInfo.Company.Code == "HQ")
            {
                Loading = true;
                string transportationTemp = null;
                string positionTemp = null;
                if (SelectedMainRow != null)
                {
                    if (PositionAllowed)
                    {
                        if (!string.IsNullOrWhiteSpace(SelectedMainRow.Position))
                        {
                            positionTemp = SelectedMainRow.Position;
                        }
                    }
                    if (TransportationAllowed)
                    {
                        if (!string.IsNullOrWhiteSpace(SelectedMainRow.Transportation))
                        {
                            transportationTemp = SelectedMainRow.Transportation;
                        }
                    }
                    Client.GetEmpByAttOperatorAsync(positionTemp, transportationTemp, LoggedUserInfo.Code);
                }
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
                            Client.DeleteTblVariableTermManualAsync(
                                (TblVariableTermManual)new TblVariableTermManual().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
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

                MainRowList.Insert(currentRowIndex + 1, new TblVariableTermManualFactoryViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (MainRowList != null)
            {
                foreach (var variable in MainRowList)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(variable, new ValidationContext(variable, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = variable.Iserial == 0;
                        var saveRow = new TblVariableTermManual();
                        saveRow.InjectFrom(variable);
                        Client.UpdateOrInsertTblVariableTermManualAsync(saveRow, save, MainRowList.IndexOf(variable));
                    }
                }
            }
        }

        #region Prop

        private DateTime? _dayDateTime;

        public DateTime? DayDate
        {
            get { return _dayDateTime; }
            set
            {
                _dayDateTime = value; RaisePropertyChanged("DayDate");
            }
        }

        private Visibility _attFileStatusVisibility;

        public Visibility AttFileStatusVisibility
        {
            get { return _attFileStatusVisibility; }
            set { _attFileStatusVisibility = value; RaisePropertyChanged("AttFileStatusVisibility"); }
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

        private Visibility _storeVisibility;

        public Visibility StoreVisibility
        {
            get { return _storeVisibility; }
            set { _storeVisibility = value; RaisePropertyChanged("StoreVisibility"); }
        }

        private Visibility _transportationLineAllow;

        public Visibility TransportationLineAllow
        {
            get { return _transportationLineAllow; }
            set { _transportationLineAllow = value; RaisePropertyChanged("TransportationLineAllow"); }
        }

        private Visibility _positionVisibility;

        public Visibility PositionVisibility
        {
            get { return _positionVisibility; }
            set { _positionVisibility = value; RaisePropertyChanged("PositionVisibility"); }
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow ?? (_storePerRow = new TblStore()); }
            set
            {
                _storePerRow = value;
                RaisePropertyChanged("StorePerRow");
                if (StorePerRow.code != null)
                    Client.GetEmpTablebyStoreAndCompanyAsync(LoggedUserInfo.DatabasEname, StorePerRow.code);
            }
        }

        private TblSalaryTerm _tblSalaryTermPerRow;

        public TblSalaryTerm SalaryTerPerRow
        {
            get { return _tblSalaryTermPerRow; }
            set
            {
                _tblSalaryTermPerRow = value;
                if (_tblSalaryTermPerRow != null) TermId = _tblSalaryTermPerRow.Iserial;
                RaisePropertyChanged("SalaryTerPerRow");
            }
        }
        private int _termId;

        public int TermId
        {
            get { return _termId; }
            set { _termId = value;RaisePropertyChanged("TermId"); }
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

        private ObservableCollection<TblSalaryTerm> _salaryTermList;

        public ObservableCollection<TblSalaryTerm> SalaryTermList
        {
            get { return _salaryTermList; }
            set
            {
                _salaryTermList = value;
                RaisePropertyChanged("SalaryTermList");
            }
        }

        private ObservableCollection<EmployeesView> _emp;

        public ObservableCollection<EmployeesView> EmpList
        {
            get { return _emp; }
            set
            {
                _emp = value;
                RaisePropertyChanged("EmpList");
            }
        }

        private SortableCollectionView<TblVariableTermManualFactoryViewModel> _mainRowList;

        public SortableCollectionView<TblVariableTermManualFactoryViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblVariableTermManualFactoryViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblVariableTermManualFactoryViewModel> _selectedMainRows;

        public ObservableCollection<TblVariableTermManualFactoryViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblVariableTermManualFactoryViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblVariableTermManualFactoryViewModel _selectedMainRow;

        public TblVariableTermManualFactoryViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private double _daysField;
        public double Days
        {
            get
            {
                return _daysField;
            }
            set
            {
                if ((_daysField.Equals(value) != true))
                {
                    _daysField = value;
                    RaisePropertyChanged("Days");
                }
            }
        }

        private double _hours;
        public double Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                if ((_hours.Equals(value) != true))
                {
                    _hours = value;
                    RaisePropertyChanged("Hours");
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
                SelectedMainRow.Transportation = Transportation;
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
                SelectedMainRow.Position = Position;
                GetEmpByStore();
            }
        }

        #endregion Prop
    }
}