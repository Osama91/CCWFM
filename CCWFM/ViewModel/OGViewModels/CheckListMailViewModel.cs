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
    public class TblCheckListMailViewModel : PropertiesViewModelBase
    {
        private int _iserialField;

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

        private string _emp;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEmployee")]
        public string Emp
        {
            get
            {
                return _emp;
            }
            set
            {
                if ((ReferenceEquals(_emp, value) != true))
                {
                    _emp = value;
                    RaisePropertyChanged("Emp");
                }
            }
        }

        private int? _tblCheckListGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCheckListGroup")]
        public int? TblCheckListGroup
        {
            get { return _tblCheckListGroup; }
            set { _tblCheckListGroup = value; RaisePropertyChanged("TblCheckListGroup"); }
        }

        private GenericTable _checkListGroupPerRow;

        public GenericTable CheckListGroupPerRow
        {
            get { return _checkListGroupPerRow; }
            set
            {
                _checkListGroupPerRow = value; RaisePropertyChanged("CheckListGroupPerRow");
                if (CheckListGroupPerRow != null)
                {
                    TblCheckListGroup = CheckListGroupPerRow.Iserial;
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
                if (EmpPerRow != null) Emp = EmpPerRow.Emplid;
            }
        }
    }

    public class CheckListMailViewModel : ViewModelBase
    {
        public CheckListMailViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.CheckListMail.ToString());
                MainRowList = new SortableCollectionView<TblCheckListMailViewModel>();
                SelectedMainRow = new TblCheckListMailViewModel();
                CheckListGroupList = new ObservableCollection<GenericTable>();

                Client.GetGenericAsync("TblCheckListGroup", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetGenericCompleted += (s, sv) =>
                {
                    CheckListGroupList = sv.Result;
                };
                Client.GetTblCheckListMailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCheckListMailViewModel();
                        var newEmp = sv.EmpList.FirstOrDefault(x => x.EMPLID == newrow.Emp);

                        if (newEmp != null)
                            newrow.EmpPerRow = new EmployeesView
                            {
                                Emplid = newEmp.EMPLID,
                                Name = newEmp.name
                            };

                        newrow.CheckListGroupPerRow = new GenericTable();
                        newrow.CheckListGroupPerRow.InjectFrom(row.TblCheckListGroup1);
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertTblCheckListMailCompleted += (s, x) =>
                {
                    var savedRow = (TblCheckListMailViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblCheckListMailCompleted += (s, ev) =>
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

        private void MainRowList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                MainRowList.Clear();
                SortBy = null;
                foreach (var sortDesc in MainRowList.SortDescriptions)
                {
                    SortBy = SortBy + "it." + sortDesc.PropertyName + (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblCheckListMailAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Code);
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
                            Client.DeleteTblCheckListMailAsync(
                                (TblCheckListMail)new TblCheckListMail().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblCheckListMailViewModel());
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
                    var saveRow = new TblCheckListMail();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblCheckListMailAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Code);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblCheckListMailViewModel> _mainRowList;

        public SortableCollectionView<TblCheckListMailViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblCheckListMailViewModel> _selectedMainRows;

        public ObservableCollection<TblCheckListMailViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCheckListMailViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblCheckListMailViewModel _selectedMainRow;

        public TblCheckListMailViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<GenericTable> _tblCheckListGroup;

        public ObservableCollection<GenericTable> CheckListGroupList
        {
            get { return _tblCheckListGroup; }
            set { _tblCheckListGroup = value; RaisePropertyChanged("CheckListGroupList"); }
        }

        #endregion Prop
    }
}