using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class EmpWeeklyDayOffViewModel : ViewModelBase
    {
        public EmpWeeklyDayOffViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                WeekList = new List<string>();

                GetItemPermissions(PermissionItemName.EmpWeeklyDayOff.ToString());

                Client.UpdateOrInsertTblEmpWeeklyDayOffCompleted += (s, x) =>
                {
                    var savedRow = (TblEmpWeeklyDayOffViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblEmpWeeklyDayOffCompleted += (s, ev) =>
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

                foreach (var variable in GetDates(2015, 10))
                {
                    WeekList.Add(variable.DayOfWeek.ToString());
                }
                GetMaindata();

                Client.GetTblEmpWeeklyDayOffCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblEmpWeeklyDayOffViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }

                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }

                    if (Export)
                    {
                        Export = false;

                        //var handler = ExportCompleted;
                        //if (handler != null) handler(this, EventArgs.Empty);
                        //ExportGrid.ExportExcel("Style");
                    }
                };
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblEmpWeeklyDayOffAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public static List<DateTime> GetDates(int year, int weekNumber)
        {
            var start = new DateTime(year, 1, 1);
            start = start.AddDays(-((int)start.DayOfWeek));
            start = start.AddDays(7 * (weekNumber - 1));
            var listNum = Enumerable.Range(0, 7).Select(num => start.AddDays(num)).Where(x => x.Year == year).ToList();
            return listNum;
        }

        private SortableCollectionView<TblEmpWeeklyDayOffViewModel> _mainRowList;

        public SortableCollectionView<TblEmpWeeklyDayOffViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblEmpWeeklyDayOffViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private List<string> _weekList;

        public List<string> WeekList
        {
            get { return _weekList; }
            set { _weekList = value; RaisePropertyChanged("WeekList"); }
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
                            MessageBox.Show(strings.AllowAddMsg);
                            return;
                        }
                    }
                    else
                    {
                        if (AllowUpdate != true)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }
                    }
                    var saveRow = new TblEmpWeeklyDayOff();

                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblEmpWeeklyDayOffAsync(saveRow, save, 0, LoggedUserInfo.Iserial);
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
                            Loading = true;
                            Client.DeleteTblEmpWeeklyDayOffAsync(
                                (TblEmpWeeklyDayOff)new TblEmpWeeklyDayOff().InjectFrom(row), MainRowList.IndexOf(row));
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

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
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
            var newrow = new TblEmpWeeklyDayOffViewModel();

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        #region Prop

        private TblEmpWeeklyDayOffViewModel _selectedMainRow;

        public TblEmpWeeklyDayOffViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblEmpWeeklyDayOffViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblEmpWeeklyDayOffViewModel> _selectedMainRows;

        public ObservableCollection<TblEmpWeeklyDayOffViewModel> SelectedMainRows
        {
            get
            {
                return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblEmpWeeklyDayOffViewModel>());
            }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        #endregion Prop
    }

    public class TblEmpWeeklyDayOffViewModel : PropertiesViewModelBase
    {
        private int? _creationByField;

        private DateTime? _creationDateField;

        private string _empField;

        private int _iserialField;

        private string _dayOffField;

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
            set { _empPerRow = value; RaisePropertyChanged("EmpPerRow"); }
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

        public string DayOff
        {
            get
            {
                return _dayOffField;
            }
            set
            {
                if ((ReferenceEquals(_dayOffField, value) != true))
                {
                    _dayOffField = value;
                    RaisePropertyChanged("DayOff");
                }
            }
        }

    }
}