using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class EmpLeavingTransactionViewModel : ViewModelBase
    {
        public EmpLeavingTransactionViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.EmpLeavingTransactionForm.ToString());
                GetCustomePermissions(PermissionItemName.EmpLeavingTransactionForm.ToString());
                PremCompleted += (s, sv) => AddNewMainRow(false);
                Client.UpdateOrInsertEmpLeavingTransactionCompleted += (s, x) =>
                {
                    var savedRow = (TblEmpLeavingTransactionViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };
                Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, null, null,LoggedUserInfo.DatabasEname);

                Client.SearchBysStoreNameCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        Client.GetEmpTablebyStoreAndCompanyAsync(LoggedUserInfo.DatabasEname, variable.code);
                        Store = variable.code;
                    }
                };
                Client.DeleteEmpLeavingTransactionCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };
                EmpList = new ObservableCollection<EmployeesView>();
                Client.GetEmpTablebyStoreAndCompanyCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        EmpList.Add(variable);
                    }
                };
            }
        }

        private ObservableCollection<EmployeesView> _empList;

        public ObservableCollection<EmployeesView> EmpList
        {
            get { return _empList; }
            set { _empList = value; RaisePropertyChanged("EmpList"); }
        }

        public string Store { get; set; }

        private SortableCollectionView<TblEmpLeavingTransactionViewModel> _mainRowList;

        public SortableCollectionView<TblEmpLeavingTransactionViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblEmpLeavingTransactionViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
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
                    if (save)
                    {
                        if (AllowAdd != true)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (AllowUpdate != true)
                        {
                            return;
                        }
                    }
                    if (!Loading)
                    {
                        var saveRow = new EmpLeavingTransaction();

                        saveRow.InjectFrom(SelectedMainRow);
                        Loading = true;
                        Client.UpdateOrInsertEmpLeavingTransactionAsync(saveRow, save, 0, LoggedUserInfo.Iserial);
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                return;
            }
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblEmpLeavingTransactionViewModel { Store = Store };
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        #region Prop

        private TblEmpLeavingTransactionViewModel _selectedMainRow;

        public TblEmpLeavingTransactionViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblEmpLeavingTransactionViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    public class TblEmpLeavingTransactionViewModel : PropertiesViewModelBase
    {
        public TblEmpLeavingTransactionViewModel()
        {
            TransDate = DateTime.Now;
        }

        private int? _creationByField;

        private DateTime? _creationDateField;

        private string _empField;

        private int _iserialField;

        private string _reasonField;

        private string _storeField;

        private DateTime _transDateField;

        public int? CreationBy
        {
            get
            {
                return _creationByField;
            }
            set
            {
                if ((_creationByField.Equals(value) != true))
                {
                    _creationByField = value;
                    RaisePropertyChanged("CreationBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        [Required]
        public string Emp
        {
            get
            {
                return _empField;
            }
            set
            {
                if ((ReferenceEquals(_empField, value) != true))
                {
                    _empField = value;
                    RaisePropertyChanged("Emp");
                }
            }
        }

        private EmployeesView _empPerRow;

        public EmployeesView EmpPerRow
        {
            get { return _empPerRow; }
            set
            {
                _empPerRow = value; RaisePropertyChanged("EmpPerRow");
                if (EmpPerRow.Emplid != null) Emp = EmpPerRow.Emplid;
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

        [Required]
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

        public string Store
        {
            get
            {
                return _storeField;
            }
            set
            {
                if ((ReferenceEquals(_storeField, value) != true))
                {
                    _storeField = value;
                    RaisePropertyChanged("Store");
                }
            }
        }

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
}