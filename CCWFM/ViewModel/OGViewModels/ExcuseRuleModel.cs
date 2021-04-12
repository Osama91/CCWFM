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
    public class TblExcuseRuleViewModel : PropertiesViewModelBase
    {
        private int _counterPerPeriodLineField;

        private string _excuseIdField;

        private int _iserialField;

        private string _periodIdField;
          [Display(Name = "CounterPerPeriodLine", ResourceType = typeof(strings))]
        public int CounterPerPeriodLine
        {
            get
            {
                return _counterPerPeriodLineField;
            }
            set
            {
                if ((_counterPerPeriodLineField.Equals(value) != true))
                {
                    _counterPerPeriodLineField = value;
                    RaisePropertyChanged("CounterPerPeriodLine");
                }
            }
        }
        
        [Display(Name = "Excuse", ResourceType = typeof(strings))]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqExcuseType")]
        public string ExcuseId
        {
            get
            {
                return _excuseIdField;
            }
            set
            {
                if ((ReferenceEquals(_excuseIdField, value) != true))
                {
                    _excuseIdField = value;
                    RaisePropertyChanged("ExcuseId");
                }
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
          [Display(Name = "Period", ResourceType = typeof(strings))]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPeriod")]
        public string PeriodId
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
                    RaisePropertyChanged("PeriodId");
                }
            }
        }
    }

    public class ExcuseRuleViewModel : ViewModelBase
    {
        AttServiceClient AttService = new AttServiceClient();

        public ExcuseRuleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblExcuseRuleViewModel>();
                SelectedMainRow = new TblExcuseRuleViewModel();

                AttService.AxPeriodsCompleted += (s, sv) =>
                {
                    PeriodList = sv.Result;
                };
                AttService.AxPeriodsAsync();
                AttService.ExcuseTypeCompleted += (s, sv) =>
                {
                    ExcuseTypeList = sv.Result;
                };
                AttService.ExcuseTypeAsync();

                AttService.GetTblExcuseRuleCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblExcuseRuleViewModel();
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

                AttService.UpdateOrInsertTblExcuseRuleCompleted += (s, x) =>
                {
                    var savedRow = (TblExcuseRuleViewModel)MainRowList.ElementAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                AttService.DeleteTblExcuseRuleCompleted += (s, ev) =>
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

            AttService.GetTblExcuseRuleAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            AttService.DeleteTblExcuseRuleAsync(
                                (TblExcuseRule)new TblExcuseRule().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblExcuseRuleViewModel());
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
                    var saveRow = new TblExcuseRule();
                    saveRow.InjectFrom(SelectedMainRow);
                    AttService.UpdateOrInsertTblExcuseRuleAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
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

        private ObservableCollection<TblExcuseRuleViewModel> _mainRowList;

        public ObservableCollection<TblExcuseRuleViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblExcuseRuleViewModel> _selectedMainRows;

        public ObservableCollection<TblExcuseRuleViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblExcuseRuleViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblExcuseRuleViewModel _selectedMainRow;

        public TblExcuseRuleViewModel SelectedMainRow
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