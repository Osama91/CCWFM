using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblUserCheckListViewModel : PropertiesViewModelBase
    {
        private int _tblAuthUser;

        public int TblAuthUser
        {
            get { return _tblAuthUser; }
            set { _tblAuthUser = value; RaisePropertyChanged("TblAuthUser"); }
        }

        private int _tblCheckListGroup;

        public int TblCheckListGroup
        {
            get { return _tblCheckListGroup; }
            set { _tblCheckListGroup = value; RaisePropertyChanged("TblCheckListGroup"); }
        }

        private GenericTable _TblCheckListGroup;

        public GenericTable CheckListGroupPerRow
        {
            get { return _TblCheckListGroup; }
            set { _TblCheckListGroup = value; RaisePropertyChanged("CheckListGroupPerRow"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _sendTo;

        public string SendTo
        {
            get { return _sendTo; }
            set { _sendTo = value; RaisePropertyChanged("SendTo"); }
        }
    }

    #endregion ViewModels

    public class UserCheckListViewModel : ViewModelBase
    {
        public UserCheckListViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblUserCheckListViewModel>();
                SelectedMainRow = new TblUserCheckListViewModel();

                
                Client.GetGenericAsync("TblCheckListGroup", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetGenericCompleted += (s, sv) =>
                {
                    CheckListGroupList = new ObservableCollection<GenericTable>();
                    foreach (var variable in sv.Result)
                    {
                        CheckListGroupList.Add(variable);
                    }
                };

                Client.GetTblUserCheckListCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblUserCheckListViewModel();

                        newrow.InjectFrom(row);
                        newrow.CheckListGroupPerRow = new GenericTable();

                        newrow.CheckListGroupPerRow.InjectFrom(row.TblCheckListGroup1);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;

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

                Client.UpdateOrInsertTblUserCheckListCompleted += (s, x) =>
                {
                    var savedRow = (TblUserCheckListViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.DeleteTblUserCheckListCompleted += (s, ev) =>
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
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblUserCheckListAsync(LoggedUserInfo.Iserial);
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
                            Loading = true;
                            Client.DeleteTblUserCheckListAsync(
                                (TblUserCheckList)new TblUserCheckList().InjectFrom(row), MainRowList.IndexOf(row));
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

            var newrow = new TblUserCheckListViewModel { TblAuthUser = LoggedUserInfo.Iserial };

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;

                    var saveRow = new TblUserCheckList();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblUserCheckListAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        private SortableCollectionView<TblUserCheckListViewModel> _mainRowList;

        public SortableCollectionView<TblUserCheckListViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<GenericTable> _checkListGroupList;

        public ObservableCollection<GenericTable> CheckListGroupList
        {
            get { return _checkListGroupList; }
            set { _checkListGroupList = value; RaisePropertyChanged("CheckListGroupList"); }
        }

        private ObservableCollection<TblUserCheckListViewModel> _selectedMainRows;

        public ObservableCollection<TblUserCheckListViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblUserCheckListViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblUserCheckListViewModel _selectedMainRow;

        public TblUserCheckListViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }
    }
}