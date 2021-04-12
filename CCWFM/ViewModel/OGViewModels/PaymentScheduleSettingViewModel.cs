using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblPaymentScheduleSettingViewModel : GenericViewModel
    {
        private bool _installmentCountsIncludedField;

        private bool _installmentIntervalIncludedField;

        private bool _percentageIncludedField;

        private bool _startingDaysIncludedField;

        public bool InstallmentCountsIncluded
        {
            get
            {
                return _installmentCountsIncludedField;
            }
            set
            {
                if ((_installmentCountsIncludedField.Equals(value) != true))
                {
                    _installmentCountsIncludedField = value;
                    RaisePropertyChanged("InstallmentCountsIncluded");
                }
            }
        }

        public bool InstallmentIntervalIncluded
        {
            get
            {
                return _installmentIntervalIncludedField;
            }
            set
            {
                if ((_installmentIntervalIncludedField.Equals(value) != true))
                {
                    _installmentIntervalIncludedField = value;
                    RaisePropertyChanged("InstallmentIntervalIncluded");
                }
            }
        }

        public bool PercentageIncluded
        {
            get
            {
                return _percentageIncludedField;
            }
            set
            {
                if ((_percentageIncludedField.Equals(value) != true))
                {
                    _percentageIncludedField = value;
                    RaisePropertyChanged("PercentageIncluded");
                }
            }
        }

        public bool StartingDaysIncluded
        {
            get
            {
                return _startingDaysIncludedField;
            }
            set
            {
                if ((_startingDaysIncludedField.Equals(value) != true))
                {
                    _startingDaysIncludedField = value;
                    RaisePropertyChanged("StartingDaysIncluded");
                }
            }
        }
    }

    public class PaymentScheduleSettingViewModel : ViewModelBase
    {
        public PaymentScheduleSettingViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblPaymentScheduleSettingViewModel>();
                SelectedMainRow = new TblPaymentScheduleSettingViewModel();

                

                Client.GetTblPaymentScheduleSettingsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPaymentScheduleSettingViewModel();
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

                Client.UpdateOrInsertTblPaymentScheduleSettingsCompleted += (s, x) =>
                {
                    var savedRow = (TblPaymentScheduleSettingViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblPaymentScheduleSettingsCompleted += (s, ev) =>
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

            Client.GetTblPaymentScheduleSettingsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblPaymentScheduleSettingsAsync(
                                (TblPaymentScheduleSetting)new TblPaymentScheduleSetting().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblPaymentScheduleSettingViewModel());
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
                    var saveRow = new TblPaymentScheduleSetting();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblPaymentScheduleSettingsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblPaymentScheduleSettingViewModel> _mainRowList;

        public SortableCollectionView<TblPaymentScheduleSettingViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPaymentScheduleSettingViewModel> _selectedMainRows;

        public ObservableCollection<TblPaymentScheduleSettingViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPaymentScheduleSettingViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPaymentScheduleSettingViewModel _selectedMainRow;

        public TblPaymentScheduleSettingViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}