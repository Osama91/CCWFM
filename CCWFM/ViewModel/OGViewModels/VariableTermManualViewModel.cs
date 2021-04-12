using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using LoginService = CCWFM.LoginService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblVariableTermManualViewModel : PropertiesViewModelBase
    {
        public TblVariableTermManualViewModel()
        {
            TransDate = DateTime.Now;
        }

        private TblSalaryTerm _tblSalaryTermPerRow;

        public TblSalaryTerm SalaryTerPerRow
        {
            get { return _tblSalaryTermPerRow; }
            set
            {
                _tblSalaryTermPerRow = value;
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
    }

    public class VariableTermManualViewModel : ViewModelBase
    {
        public VariableTermManualViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblVariableTermManualViewModel>();
                SelectedMainRow = new TblVariableTermManualViewModel();

                Client.GetTblVariableTermManualCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblVariableTermManualViewModel();
                        newrow.EmpPerRow = new EmployeesView();
                        newrow.SalaryTerPerRow = new TblSalaryTerm();

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

                Client.UpdateOrInsertTblVariableTermManualCompleted += (s, x) =>
                {
                    var savedRow = (TblVariableTermManualViewModel)MainRowList.GetItemAt(x.outindex);

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
                GetEmpByStore();
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

        public void GetEmpByStore()
        {
            if (LoggedUserInfo.Store != null)
                StorePerRow = new TblStore().InjectFrom(LoggedUserInfo.Store) as TblStore;
            StoreList = new SortableCollectionView<TblStore>();

            if (LoggedUserInfo.AllowedStores != null && LoggedUserInfo.Company.Code != "HQ")
                Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, null, null, LoggedUserInfo.DatabasEname);

            Client.SearchBysStoreNameCompleted += (s, sv) =>
            {
                StoreList = sv.Result;

                if (StoreList != null && StoreList.Count == 1)
                {
                    StorePerRow = StoreList.FirstOrDefault();
                }
            };

            Loading = true;
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

                MainRowList.Insert(currentRowIndex + 1, new TblVariableTermManualViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblVariableTermManual();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblVariableTermManualAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

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

        private SortableCollectionView<TblVariableTermManualViewModel> _mainRowList;

        public SortableCollectionView<TblVariableTermManualViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblVariableTermManualViewModel> _selectedMainRows;

        public ObservableCollection<TblVariableTermManualViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblVariableTermManualViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblVariableTermManualViewModel _selectedMainRow;

        public TblVariableTermManualViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}