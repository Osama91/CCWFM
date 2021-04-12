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
    public class TblExcuseViewModel : PropertiesViewModelBase
    {
        private SolidColorBrush _backgroundColor;

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; RaisePropertyChanged("BackgroundColor"); }
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

        private Visibility _isNotExtraExcuse;

        public Visibility IsNotExtraExcuse
        {
            get { return _isNotExtraExcuse; }
            set { _isNotExtraExcuse = value; RaisePropertyChanged("IsNotExtraExcuse"); }
        }

        private string _cspexcuseidField;

        private string _emplidField;

        private int? _fromTimeField;

        private int _iserialField;

        private int _statusField;

        private int? _toTimeField;

        private DateTime? _transDateField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqExcuseType")]
        public string CSPEXCUSEID
        {
            get { return _cspexcuseidField; }
            set
            {
                if ((ReferenceEquals(_cspexcuseidField, value) != true))
                {
                    _cspexcuseidField = value;
                    RaisePropertyChanged("CSPEXCUSEID");
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFromTime")]
        public int? FromTime
        {
            get { return _fromTimeField; }
            set
            {
                if ((_fromTimeField.Equals(value) != true))
                {
                    if (ToTime != null && FromTime != null)
                    {
                        if (value > ToTime)
                        {
                            value = ToTime;
                        }
                    }
                    _fromTimeField = value;
                    RaisePropertyChanged("FromTime");
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

        private Visibility _excuseStatusVisibility;

        public Visibility ExcuseStatusVisibility
        {
            get { return _excuseStatusVisibility; }
            set { _excuseStatusVisibility = value; RaisePropertyChanged("ExcuseStatusVisibility"); }
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
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqToTime")]
        public int? ToTime
        {
            get { return _toTimeField; }
            set
            {
                if ((_toTimeField.Equals(value) != true))
                {
                    if (ToTime != null && FromTime != null)
                    {
                        if (value < FromTime)
                        {
                            value = FromTime;
                        }
                    }
                    _toTimeField = value;
                    RaisePropertyChanged("ToTime");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? TransDate
        {
            get { return _transDateField; }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
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

        private bool _containMultipleTransaction;

        public bool ContainMultipleTransaction
        {
            get { return _containMultipleTransaction; }
            set { _containMultipleTransaction = value; RaisePropertyChanged("ContainMultipleTransaction"); }
        }
    }

    public class ExcuseViewModel : ViewModelBase
    {

        AttServiceClient AttService = new AttServiceClient();

        public ExcuseViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblExcuseViewModel>();
                SelectedMainRows = new ObservableCollection<TblExcuseViewModel>();
                AttService.ExcuseTypeCompleted += (x, y) =>
                {
                    CcExcuse = y.Result;
                };
                AttService.ExcuseTypeAsync();
                AttService.UpdateAndInsertTblExcuseCompleted += (x, y) =>
                {
                    var savedRow = (TblExcuseViewModel)MainRowList.GetItemAt(y.outindex);

                    if (savedRow != null) savedRow.InjectFrom(y.Result);
                };
                AttService.GetTblExcusesCompleted += (x, y) =>
                {
                    foreach (var row in y.Result)
                    {
                        var newrow = new TblExcuseViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                };
                AttService.DeleteTblExcuseCompleted += (s, ev) =>
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
                            AttService.DeleteTblExcuseAsync(
                                (TblExcuse)new TblExcuse().InjectFrom(row), MainRowList.IndexOf(row));
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
                    var saveRow = new TblExcuse();
                    saveRow.InjectFrom(SelectedMainRow);

                    //    AttService.UpdateAndInsertTblExcuseAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
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

                MainRowList.Insert(currentRowIndex + 1, new TblExcuseViewModel

                {
                    Emplid = LoggedUserInfo.Code
                });
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            AttService.GetTblExcusesAsync(MainRowList.Count, PageSize, LoggedUserInfo.Code, SortBy, Filter, ValuesObjects);
        }

        private SortableCollectionView<TblExcuseViewModel> _mainRowList;

        public SortableCollectionView<TblExcuseViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblExcuseViewModel> _selectedMainRows;

        public ObservableCollection<TblExcuseViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblExcuseViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblExcuseViewModel _selectedMainRow;

        public TblExcuseViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
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
    }
}