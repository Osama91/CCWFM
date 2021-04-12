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
    public class TblPaymentScheduleViewModel : GenericViewModel
    {
        private SortableCollectionView<TblPaymentScheduleDetailViewModel> _tblPaymentScheduleDetail;

        public SortableCollectionView<TblPaymentScheduleDetailViewModel> DetailsList
        {
            get
            {
                return _tblPaymentScheduleDetail ?? (_tblPaymentScheduleDetail = new SortableCollectionView<TblPaymentScheduleDetailViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblPaymentScheduleDetail, value) != true))
                {
                    _tblPaymentScheduleDetail = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        private string _axMethodOfPaymentCodeField;

        public string AxMethodOfPaymentCode
        {
            get
            {
                return _axMethodOfPaymentCodeField;
            }
            set
            {
                if ((ReferenceEquals(_axMethodOfPaymentCodeField, value) != true))
                {
                    _axMethodOfPaymentCodeField = value;
                    RaisePropertyChanged("AxMethodOfPaymentCode");
                }
            }
        }
    }

    public class TblPaymentScheduleDetailViewModel : PropertiesViewModelBase
    {
        private double? _installmentCountsField;

        private double? _installmentIntervalField;

        private int _iserialField;

        private double? _percentageField;

        private double? _startingDaysField;

        private int _tblPaymentScheduleettingsField;

        private TblPaymentScheduleSetting _settingPaymentPerRow;

        [ReadOnly(true)]
        public TblPaymentScheduleSetting SettingPaymentPerRow
        {
            get { return _settingPaymentPerRow; }
            set { _settingPaymentPerRow = value; RaisePropertyChanged("SettingPaymentPerRow"); }
        }

        public double? InstallmentCounts
        {
            get
            {
                return _installmentCountsField;
            }
            set
            {
                if ((_installmentCountsField.Equals(value) != true))
                {
                    _installmentCountsField = value;
                    RaisePropertyChanged("InstallmentCounts");
                }
            }
        }

        public double? InstallmentInterval
        {
            get
            {
                return _installmentIntervalField;
            }
            set
            {
                if ((_installmentIntervalField.Equals(value) != true))
                {
                    _installmentIntervalField = value;
                    RaisePropertyChanged("InstallmentInterval");
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

        public double? Percentage
        {
            get
            {
                return _percentageField;
            }
            set
            {
                if ((_percentageField.Equals(value) != true))
                {
                    _percentageField = value;
                    RaisePropertyChanged("Percentage");
                }
            }
        }

        public double? StartingDays
        {
            get
            {
                return _startingDaysField;
            }
            set
            {
                if ((_startingDaysField.Equals(value) != true))
                {
                    _startingDaysField = value;
                    RaisePropertyChanged("StartingDays");
                }
            }
        }

        public int TblPaymentSchedule
        {
            get
            {
                return _tblPaymentScheduleettingsField;
            }
            set
            {
                if ((_tblPaymentScheduleettingsField.Equals(value) != true))
                {
                    _tblPaymentScheduleettingsField = value;
                    RaisePropertyChanged("TblPaymentSchedule");
                }
            }
        }

        private int _tblPaymentScheduleSettings;

        public int TblPaymentScheduleSettings
        {
            get { return _tblPaymentScheduleSettings; }
            set { _tblPaymentScheduleSettings = value; RaisePropertyChanged("TblPaymentScheduleSettings"); }
        }
    }

    public class PaymentScheduleViewModel : ViewModelBase
    {
        public PaymentScheduleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client.GetVendPayModeAsync("CCM");
                Client.GetVendPayModeCompleted += (s, sv) =>
                {
                    VendPayModeList = sv.Result;
                };
                MainRowList = new SortableCollectionView<TblPaymentScheduleViewModel>();
                SelectedMainRow = new TblPaymentScheduleViewModel();
                SelectedDetailRow = new TblPaymentScheduleDetailViewModel();
                
                Client.GetTblPaymentScheduleSettingsAsync(0, 50, "it.Iserial", null, null);
                Client.GetTblPaymentScheduleSettingsCompleted += (s, sv) =>
                {
                    SchedulesSettingList = sv.Result;
                };
                Client.GetTblPaymentScheduleCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPaymentScheduleViewModel();
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
                Client.GetTblPaymentScheduleDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPaymentScheduleDetailViewModel
                        {
                            SettingPaymentPerRow = row.TblPaymentScheduleSetting
                        };
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                };
                Client.UpdateOrInsertTblPaymentScheduleCompleted += (s, x) =>
                {
                    var savedRow = (TblPaymentScheduleViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.DeleteTblPaymentScheduleCompleted += (s, ev) =>
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
            Client.GetTblPaymentScheduleAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                        Client.DeleteTblPaymentScheduleAsync(
                            (TblPaymentSchedule)new TblPaymentSchedule().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblPaymentScheduleViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (SelectedMainRow.DetailsList.Sum(x => x.Percentage) != 100)
                {
                    isvalid = false;
                    MessageBox.Show("Total Percentage Should Be !00%");
                }

                if (isvalid)
                {
                    var saveRow = new TblPaymentSchedule();
                    var save = SelectedMainRow.Iserial == 0;
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblPaymentScheduleDetails = new ObservableCollection<TblPaymentScheduleDetail>();
                    GenericMapper.InjectFromObCollection(saveRow.TblPaymentScheduleDetails, SelectedMainRow.DetailsList);
                    Client.UpdateOrInsertTblPaymentScheduleAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            if (SelectedMainRow != null)
                Client.GetTblPaymentScheduleDetailAsync(SelectedMainRow.DetailsList.Count, 50, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
        }

        #region Prop

        private ObservableCollection<TblPaymentScheduleSetting> _schedulesSettingList;

        public ObservableCollection<TblPaymentScheduleSetting> SchedulesSettingList
        {
            get { return _schedulesSettingList ?? (_schedulesSettingList = new ObservableCollection<TblPaymentScheduleSetting>()); }
            set { _schedulesSettingList = value; RaisePropertyChanged("SchedulesSettingList"); }
        }

        private ObservableCollection<VENDPAYMMODETABLE> _vendPayMode;

        public ObservableCollection<VENDPAYMMODETABLE> VendPayModeList
        {
            get { return _vendPayMode ?? (_vendPayMode = new ObservableCollection<VENDPAYMMODETABLE>()); }
            set { _vendPayMode = value; RaisePropertyChanged("VendPayModeList"); }
        }

        private SortableCollectionView<TblPaymentScheduleViewModel> _mainRowList;

        public SortableCollectionView<TblPaymentScheduleViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblPaymentScheduleViewModel> _selectedMainRows;

        public ObservableCollection<TblPaymentScheduleViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPaymentScheduleViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblPaymentScheduleViewModel _selectedMainRow;

        public TblPaymentScheduleViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblPaymentScheduleDetailViewModel _selectedDetailRow;

        public TblPaymentScheduleDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblPaymentScheduleDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblPaymentScheduleDetailViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPaymentScheduleDetailViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}