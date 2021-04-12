using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using CCWFM.BankDepositService;
using CCWFM.AuthService;
using System.ComponentModel;

namespace CCWFM.ViewModel.Gl
{
    public class SalaryApprovalViewModel : ViewModelBase
    {
        BankDepositServiceClient BankDepositClient = Helpers.Services.Instance.GetBankDepositServiceClient();
        public SalaryApprovalViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.SalaryApproval.ToString());
                MainRowList = new ObservableCollection<TblSalaryApprovalHeaderModel>();
                SelectedMainRow = new TblSalaryApprovalHeaderModel();

                BankDepositClient.GetSalaryApprovalHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSalaryApprovalHeaderModel();
                        newrow.SalaryTypePerRow = row.TblSalaryType1;
                        newrow.TblStore1 = row.TblStore1;
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                BankDepositClient.DeleteSalaryApprovalDetailCompleted += (s, ev) => {

                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };
                BankDepositClient.UpdateOrInsertSalaryApprovalHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                                
                BankDepositClient.GetSalaryApprovalDetailCompleted += (s, sv) =>
                 {
                     foreach (var row in sv.Result)
                     {
                         var newrow = new TblSalaryApprovalDetailModel();
                         newrow.EmpPerRow = sv.EmpList.FirstOrDefault(w => w.iserial == row.TblEmployee);
                         newrow.InjectFrom(row);
                         SelectedMainRow.DetailsList.Add(newrow);
                     }

                     Loading = false;
                     DetailFullCount = sv.fullCount;
                     if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                     {
                         SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                     }
                     if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                     {
                         AddNewDetailRow(false);
                     }
                 };

                BankDepositClient.ImportSalaryApprovalCompleted += (s, sv) =>
                 {
                     GetMaindata();
                 };
                BankDepositClient.UpdateOrInsertSalaryApprovalDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedMainRow.DetailsList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc";
            Loading = true;
            BankDepositClient.GetSalaryApprovalHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial,
                LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
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
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblSalaryApprovalHeaderModel();

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (!Loading)
            {
                if (SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow,
                        new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = SelectedMainRow.Iserial == 0;
                        if (AllowUpdate != true && !save)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }
                        var saveRow = new BankDepositService.TblSalaryApprovalHeader();
                        saveRow.InjectFrom(SelectedMainRow);
                        Loading = true;
                        BankDepositClient.UpdateOrInsertSalaryApprovalHeaderAsync(saveRow, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial,
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblSalaryApprovalHeaderModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new BankDepositService.TblSalaryApprovalHeader();
                    saveRow.InjectFrom(oldRow);

                    BankDepositClient.UpdateOrInsertSalaryApprovalHeaderAsync(saveRow, MainRowList.IndexOf(oldRow), LoggedUserInfo.Iserial,
                        LoggedUserInfo.DatabasEname);
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
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            //BankDepositClient.DeleteTblSalaryApproval((TblSalaryApprovalHeader)new TblSalaryApprovalHeader().InjectFrom(row), 0, LoggedUserInfo.DatabasEname);
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

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                BankDepositClient.GetSalaryApprovalDetailAsync(SelectedMainRow.DetailsList.Count, PageSize,SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            BankDepositClient.DeleteSalaryApprovalDetailAsync(
                                (BankDepositService.TblSalaryApprovalDetail)new BankDepositService.TblSalaryApprovalDetail().InjectFrom(row), 1,
                                 LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

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

            var newrow = new TblSalaryApprovalDetailModel
            {
                TblSalaryApprovalHeader = SelectedMainRow.Iserial,

            };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        internal void InsertImportedSalaryApprovals(ObservableCollection<BankDepositService.TblSalaryApprovalHeader> salaryList)
        {
            BankDepositClient.ImportSalaryApprovalAsync(salaryList, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
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
                    if (SelectedDetailRow.TblSalaryApprovalHeader == 0)
                    {
                        SelectedDetailRow.TblSalaryApprovalHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new BankDepositService.TblSalaryApprovalDetail();
                    
                    rowToSave.InjectFrom(SelectedDetailRow);

                    BankDepositClient.UpdateOrInsertSalaryApprovalDetailAsync(rowToSave, LoggedUserInfo.Iserial,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void SaveOldDetailRow(TblSalaryApprovalDetailModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new BankDepositService.TblSalaryApprovalDetail();
                    saveRow.InjectFrom(oldRow);

                    if (SelectedMainRow != null && SelectedMainRow.DetailsList != null)
                        BankDepositClient.UpdateOrInsertSalaryApprovalDetailAsync(saveRow, LoggedUserInfo.Iserial, SelectedMainRow.DetailsList.IndexOf(oldRow),
                            LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop
        private ObservableCollection<TblSalaryApprovalHeaderModel> _mainRowList;

        public ObservableCollection<TblSalaryApprovalHeaderModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblSalaryApprovalHeaderModel> _selectedMainRows;

        public ObservableCollection<TblSalaryApprovalHeaderModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSalaryApprovalHeaderModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblSalaryApprovalHeaderModel _selectedMainRow;

        public TblSalaryApprovalHeaderModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblSalaryApprovalDetailModel _selectedDetailRow;

        public TblSalaryApprovalDetailModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblSalaryApprovalDetailModel> _selectedDetailRows;

        public ObservableCollection<TblSalaryApprovalDetailModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSalaryApprovalDetailModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }


        #endregion Prop
    }

    #region ViewModels

    public class TblSalaryApprovalHeaderModel : Web.DataLayer.PropertiesViewModelBase
    {
        private DateTime _DueDate;

        public DateTime DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value;RaisePropertyChanged("DueDate"); }
        }

        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { _Code = value;RaisePropertyChanged("Code"); }
        }

        private BankDepositService.TblStore _TblStore1;

        public BankDepositService.TblStore TblStore1
        {
            get { return _TblStore1; }
            set { _TblStore1 = value;RaisePropertyChanged("TblStore1"); }
        }


        private BankDepositService.TblSalaryType _SalaryTypePerRow;

        public BankDepositService.TblSalaryType SalaryTypePerRow
        {
            get { return _SalaryTypePerRow; }
            set { _SalaryTypePerRow = value;RaisePropertyChanged("SalaryTypePerRow"); }
        }

        private int _TblEmployee;

        public int TblEmployee
        {
            get { return _TblEmployee; }
            set { _TblEmployee = value;RaisePropertyChanged("TblEmployee"); }
        }


        private EmployeesView _empPerRow;

        [ReadOnly(true)]
        public EmployeesView EmpPerRow
        {
            get
            {
                return _empPerRow;
            }
            set
            {
                if ((ReferenceEquals(_empPerRow, value) != true))
                {
                    _empPerRow = value;                 
                    RaisePropertyChanged("EmpPerRow");
                }
            }
        }

        private ObservableCollection<TblSalaryApprovalDetailModel> _detailLst;

        public ObservableCollection<TblSalaryApprovalDetailModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new ObservableCollection<TblSalaryApprovalDetailModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }
        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value;RaisePropertyChanged("Iserial"); }
        }
         
        private int _TblSalaryType;

        public int TblSalaryType
        {
            get { return _TblSalaryType; }
            set { _TblSalaryType = value; RaisePropertyChanged("TblSalaryType"); }
        }
        private int _year;

        public int Year
        {
            get { return _year; }
            set { _year = value; RaisePropertyChanged("Year"); }
        }

        private int _month;

        public int Month
        {
            get { return _month; }
            set { _month = value; RaisePropertyChanged("Month"); }
        }

        private DateTime _VotDate;

        public DateTime VotDate
        {
            get { return _VotDate; }
            set { _VotDate = value; RaisePropertyChanged("VotDate"); }
        }
        private int _TblStore;

        public int TblStore
        {
            get { return _TblStore; }
            set { _TblStore = value; RaisePropertyChanged("TblStore"); }
        }
        private DateTime _CreationDate;

        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private int? _TblUser;

        public int? TblUser
        {
            get { return _TblUser; }
            set { _TblUser = value; RaisePropertyChanged("TblUser"); }
        }
        private bool _HQApproved;

        public bool HQApproved
        {
            get { return _HQApproved; }
            set { _HQApproved = value; RaisePropertyChanged("HQApproved"); }
        }

        private bool _StoreApproved;

        public bool StoreApproved
        {
            get { return _StoreApproved; }
            set { _StoreApproved = value; RaisePropertyChanged("StoreApproved"); }
        }
    }

    public class TblSalaryApprovalDetailModel : Web.DataLayer.PropertiesViewModelBase
    {

        private TblEmployee _EmpPerRow;

        public TblEmployee EmpPerRow
        {
            get { return _EmpPerRow; }
            set { _EmpPerRow = value;RaisePropertyChanged("EmpPerRow"); }
        }

        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private int _TblSalaryApprovalHeader;

        public int TblSalaryApprovalHeader
        {
            get { return _TblSalaryApprovalHeader; }
            set { _TblSalaryApprovalHeader = value; RaisePropertyChanged("TblSalaryApprovalHeader"); }
        }
        private int _TblEmployee;

        public int TblEmployee
        {
            get { return _TblEmployee; }
            set { _TblEmployee = value; RaisePropertyChanged("TblEmployee"); }
        }

        private decimal _Salary;

        public decimal Salary
        {
            get { return _Salary; }
            set { _Salary = value;RaisePropertyChanged("Salary"); }
        }

        private bool _Approved;

        public bool Approved
        {
            get { return _Approved; }
            set { _Approved = value;RaisePropertyChanged("Approved"); }
        }

        private DateTime _CreationDate;

        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value;RaisePropertyChanged("CreationDate"); }
        }


        //private int? _tblJournalAccountType;

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        //public int? TblJournalAccountType
        //{
        //    get { return _tblJournalAccountType; }
        //    set { _tblJournalAccountType = value; RaisePropertyChanged("TblJournalAccountType"); }
        //}
    }

    #endregion ViewModels
}