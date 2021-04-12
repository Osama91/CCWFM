using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.AttViewModel
{
    public class TblEmployeeBehalfViewModel : PropertiesViewModelBase
    {
        private string _attOperatorId;

        [ReadOnly(true)]
        public string AttOperatorId
        {
            get { return _attOperatorId; }
            set
            {
                _attOperatorId = value;

                RaisePropertyChanged("AttOperatorId");
            }
        }

        private string _emplidField;

        private int _iserialField;

        private string _managerIdField;

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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqManager")]
        [ReadOnly(true)]
        public string ManagerId
        {
            get
            {
                return _managerIdField;
            }
            set
            {
                if ((ReferenceEquals(_managerIdField, value) != true))
                {
                    _managerIdField = value;
                    RaisePropertyChanged("ManagerId");
                }
            }
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

        private EmployeesView _operatorPerRow;

        public EmployeesView OperatorPerRow
        {
            get
            {
                return _operatorPerRow;
            }
            set
            {
                if ((ReferenceEquals(_operatorPerRow, value) != true))
                {
                    _operatorPerRow = value;
                    RaisePropertyChanged("OperatorPerRow");
                }
            }
        }

        private EmployeesView _managerPerRow;

        public EmployeesView ManagerPerRow
        {
            get
            {
                return _managerPerRow;
            }
            set
            {
                if ((ReferenceEquals(_managerPerRow, value) != true))
                {
                    _managerPerRow = value;
                    RaisePropertyChanged("ManagerPerRow");
                }
            }
        }
    }

    public class EmployeeBehalfViewModel : ViewModelBase
    {
        private readonly CRUD_ManagerServiceClient _empClient = new CRUD_ManagerServiceClient();
        public ObservableCollection<EmployeesView> EmpViewList = new ObservableCollection<EmployeesView>();

        public EmployeeBehalfViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.EmployeeBehalfForm.ToString());
                //   GetCustomePermissions(PermissionItemName.EmployeeBehalf.ToString());
                MainRowList = new SortableCollectionView<TblEmployeeBehalfViewModel>();
                SelectedMainRows = new ObservableCollection<TblEmployeeBehalfViewModel>();
                DetailList = new SortableCollectionView<EmployeesView>();

                _empClient.GetEmpTableCompleted += (x, y) =>
                {
                    foreach (var item in y.Result)
                    {
                        
                        MainRowList.Add(new TblEmployeeBehalfViewModel
                        {
                            Emplid = item.Emplid,
                            EmpPerRow = item
                        }
                            );
                        EmpViewList.Add(item);
                    }
                    Client.GetTblEmployeeBehalfAsync(new ObservableCollection<string>(EmpViewList.Select(e => e.Emplid)));
                    FullCount = y.fullCount;
                    Loading = false;
                };

                Client.GetEmpTableCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        DetailList.Add(row);
                    }
                    DetailFullCount = sv.fullCount;
                    Loading = false;
                };

                Client.UpdateOrInsertTblEmployeeBehalfCompleted += (x, y) =>
                {
                    var savedRow = (TblEmployeeBehalfViewModel)MainRowList.GetItemAt(y.outindex);
                    Loading = false;
                    if (savedRow != null) savedRow.InjectFrom(y.Result);
                };

                Client.GetTblEmployeeBehalfCompleted += (x, y) =>
                {
                    foreach (var row in y.Result)
                    {
                        var tempRow = MainRowList.SingleOrDefault(w => w.Emplid == row.Emplid);
                        if (tempRow != null)
                        {
                            tempRow.ManagerId = row.ManagerId;
                            tempRow.AttOperatorId = row.AttOperatorId;
                            tempRow.Iserial = row.Iserial;
                            tempRow.ManagerPerRow = EmpViewList.FirstOrDefault(w => w.Emplid == row.ManagerId);
                            tempRow.EmpPerRow = EmpViewList.FirstOrDefault(w => w.Emplid == row.Emplid);
                            tempRow.OperatorPerRow = EmpViewList.FirstOrDefault(w => w.Emplid == row.AttOperatorId);
                        }
                    }
                    Loading = false;
                };
                Client.DeleteTblEmployeeBehalfCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    Loading = false;
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                GetMaindata();
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Emplid";

            Loading = true;
            Client.GetEmpTableAsync(DetailList.Count, PageSize, DetailSortBy, DetailFilter, DetailValuesObjects);
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
                            if (AllowDelete)
                            {
                                Loading = true;
                                Client.DeleteTblEmployeeBehalfAsync(
                                    (TblEmployeeBehalf)new TblEmployeeBehalf().InjectFrom(row), MainRowList.IndexOf(row));
                            }
                            else
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                            }
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

                    var saveRow = new TblEmployeeBehalf();
                    saveRow.InjectFrom(SelectedMainRow);
                    if (save)
                    {
                        if (AllowAdd)
                        {
                            Loading = true;
                            Client.UpdateOrInsertTblEmployeeBehalfAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                        }
                        else
                        {
                            MessageBox.Show(strings.AllowAddMsg);
                        }
                    }
                    else
                    {
                        if (AllowUpdate)
                        {
                            Loading = true;
                            Client.UpdateOrInsertTblEmployeeBehalfAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                        }
                        else
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                        }
                    }
                }
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Emplid";

            Loading = true;
            _empClient.GetEmpTableAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        private SortableCollectionView<EmployeesView> _detailList;

        public SortableCollectionView<EmployeesView> DetailList
        {
            get { return _detailList; }
            set
            {
                _detailList = value;
                RaisePropertyChanged("DetailList");
            }
        }

        private SortableCollectionView<TblEmployeeBehalfViewModel> _mainRowList;

        public SortableCollectionView<TblEmployeeBehalfViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblEmployeeBehalfViewModel> _selectedMainRows;

        public ObservableCollection<TblEmployeeBehalfViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblEmployeeBehalfViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblEmployeeBehalfViewModel _selectedMainRow;

        public TblEmployeeBehalfViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }
    }
}