using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblStyleStatusViewModel : PropertiesViewModelBase
    {
        private string _anameField;

        private string _codeField;

        private string _enameField;

        private int _iserialField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get
            {
                return _anameField;
            }
            set
            {
                if ((ReferenceEquals(_anameField, value) != true))
                {
                    _anameField = value;
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get
            {
                return _enameField;
            }
            set
            {
                if ((ReferenceEquals(_enameField, value) != true))
                {
                    _enameField = value;
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
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
    }

    public class StyleStatusViewModel : ViewModelBase
    {
        public StyleStatusViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblStyleStatusViewModel>();
                SelectedMainRow = new TblStyleStatusViewModel();

                Client.GetTblStyleStatusCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStyleStatusViewModel();

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

                Client.UpdateOrInsertTblStyleStatusCompleted += (s, x) =>
                {
                    var savedRow = (TblStyleStatusViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblStyleStatusCompleted += (s, ev) =>
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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblStyleStatusAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblStyleStatusAsync(
                                (TblStyleStatu)new TblStyleStatu().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblStyleStatusViewModel());
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
                    var saveRow = new TblStyleStatu();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblStyleStatusAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblStyleStatusViewModel> _mainRowList;

        public SortableCollectionView<TblStyleStatusViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblStyleStatusViewModel> _selectedMainRows;

        public ObservableCollection<TblStyleStatusViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStyleStatusViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblStyleStatusViewModel _selectedMainRow;

        public TblStyleStatusViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}