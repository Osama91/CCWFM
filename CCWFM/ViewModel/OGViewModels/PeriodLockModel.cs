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
using CCWFM.AttService;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblPeriodLockModel : PropertiesViewModelBase
    {  

        private int _iserialField;

   

        int _DayToClose;
        public int DayToClose
        {
            get
            {
                return _DayToClose;
            }
            set
            {
                
                    _DayToClose = value;
                    RaisePropertyChanged("DayToClose");
                
            }
        }
     [Display(AutoGenerateField = false)]
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
        string _periodIdField;
          [Display(Name = "Period", ResourceType = typeof(strings))]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPeriod")]
        public string CspPeriodID
        {
            get
            {
                return _periodIdField;
            }
            set
            {
                if ((ReferenceEquals(_periodIdField, value) != true))
                {
                    _periodIdField = value;
                    RaisePropertyChanged("CspPeriodID");
                }
            }
        }
        string _CspPeriodLineIDField;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPeriod")]
        public string CspPeriodLineID
        {
            get
            {
                return _CspPeriodLineIDField;
            }
            set
            {
                if ((ReferenceEquals(_CspPeriodLineIDField, value) != true))
                {
                    _CspPeriodLineIDField = value;
                    RaisePropertyChanged("CspPeriodLineID");
                }
            }
        }
    }

    public class PeriodLockViewModel : ViewModelBase
    {
        AttServiceClient AttService = new AttServiceClient();

        public PeriodLockViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblPeriodLockModel>();
                SelectedMainRow = new TblPeriodLockModel();

                AttService.AxPeriodsCompleted += (s, sv) =>
                {
                    PeriodList = sv.Result;
                };
                AttService.AxPeriodsAsync();   

                AttService.GetTblPeriodLockCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPeriodLockModel();
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

                AttService.UpdateOrInsertTblPeriodLockCompleted += (s, x) =>
                {
                    var savedRow = MainRowList.ElementAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                AttService.DeleteTblPeriodLockCompleted += (s, ev) =>
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

            AttService.GetTblPeriodLockAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            AttService.DeleteTblPeriodLockAsync(
                                (TblPeriodLock)new TblPeriodLock().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblPeriodLockModel());
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
                    var saveRow = new TblPeriodLock();
                    saveRow.InjectFrom(SelectedMainRow);
                    AttService.UpdateOrInsertTblPeriodLockAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private ObservableCollection<ExcuseType> _excuseTypeList;

        public ObservableCollection<ExcuseType> ExcuseTypeList
        {
            get { return _excuseTypeList; }
            set { _excuseTypeList = value; RaisePropertyChanged("ExcuseTypeList"); }
        }

        private ObservableCollection<TblPeriodLockModel> _mainRowList;

        public ObservableCollection<TblPeriodLockModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPeriodLockModel> _selectedMainRows;

        public ObservableCollection<TblPeriodLockModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPeriodLockModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPeriodLockModel _selectedMainRow;

        public TblPeriodLockModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<CSPPERIOD> _periodList;

        public ObservableCollection<CSPPERIOD> PeriodList
        {
            get { return _periodList; }
            set { _periodList = value; RaisePropertyChanged("PeriodList"); }
        }

        #endregion Prop
    }
}