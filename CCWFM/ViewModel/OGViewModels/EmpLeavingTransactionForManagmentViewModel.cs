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

namespace CCWFM.ViewModel.OGViewModels
{
    public class EmpLeavingTransactionForManagmentViewModel : ViewModelBase
    {
        public EmpLeavingTransactionForManagmentViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.EmpLeavingTransactionForManagment.ToString());
                MainRowList = new SortableCollectionView<TblEmpLeavingTransactionViewModel>();
                SelectedMainRow = new TblEmpLeavingTransactionViewModel();
                Client.GetEmpLeavingTransactionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblEmpLeavingTransactionViewModel();
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

                Client.DeleteEmpLeavingTransactionCompleted += (s, ev) =>
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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetEmpLeavingTransactionAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                                MessageBox.Show(strings.AllowAddMsg);
                                return;
                            }
                            Loading = true;
                            Client.DeleteEmpLeavingTransactionAsync(
                                (EmpLeavingTransaction)new EmpLeavingTransaction().InjectFrom(row), MainRowList.IndexOf(row));
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
            var newrow = new TblEmpLeavingTransactionViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        #region Prop

        private SortableCollectionView<TblEmpLeavingTransactionViewModel> _mainRowList;

        public SortableCollectionView<TblEmpLeavingTransactionViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblEmpLeavingTransactionViewModel> _selectedMainRows;

        public ObservableCollection<TblEmpLeavingTransactionViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblEmpLeavingTransactionViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _empLeavingTransactionForManagmentGroupList;

        public ObservableCollection<GenericTable> EmpLeavingTransactionForManagmentGroupList
        {
            get { return _empLeavingTransactionForManagmentGroupList ?? (_empLeavingTransactionForManagmentGroupList = new ObservableCollection<GenericTable>()); }
            set { _empLeavingTransactionForManagmentGroupList = value; RaisePropertyChanged("EmpLeavingTransactionForManagmentGroupList"); }
        }

        private int _empLeavingTransactionForManagmentGroup;

        public int EmpLeavingTransactionForManagmentGroup
        {
            get { return _empLeavingTransactionForManagmentGroup; }
            set { _empLeavingTransactionForManagmentGroup = value; RaisePropertyChanged("EmpLeavingTransactionForManagmentGroup"); }
        }

        private TblEmpLeavingTransactionViewModel _selectedMainRow;

        public TblEmpLeavingTransactionViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}