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
    #region ViewModels

    public class TblCurrencyDailyExchangeViewModel : PropertiesViewModelBase
    {
        private int _year;

        public int Year
        {
            get { return _year; }
            set { _year = value; RaisePropertyChanged("Year"); }
        }

        private int? _tblCurrency;

        private int _votglserial;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
        public int? TblCurrency
        {
            get
            {
                return _tblCurrency;
            }
            set
            {
                if ((ReferenceEquals(_tblCurrency, value) != true))
                {
                    //  Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Aname" });

                    _tblCurrency = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public int votglserial
        {
            get
            {
                return _votglserial;
            }
            set
            {
                if ((_votglserial.Equals(value) != true))
                {
                    _votglserial = value;
                    RaisePropertyChanged("votglserial");
                }
            }
        }

        private DateTime _docDate;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime DocDate
        {
            get
            {
                return _docDate;
            }
            set
            {
                // Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "TechSpec" });
                _docDate = value;
                RaisePropertyChanged("DocDate");
            }
        }

        private float? _exchRate;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqExchRate")]
        public float? ExchRate
        {
            get
            {
                return _exchRate;
            }
            set
            {
                if ((_exchRate.Equals(value) != true))
                {
                    //  Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "YearOfProduct" });
                    _exchRate = value;
                    RaisePropertyChanged("ExchRate");
                }
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private bool _pending;

        public bool Pending
        {
            get { return _pending; }
            set { _pending = value; RaisePropertyChanged("Pending"); }
        }

        private SortableCollectionView<TblCurrencyDailyExchangeViewModel> _detailslist;

        public SortableCollectionView<TblCurrencyDailyExchangeViewModel> DetailsList
        {
            get { return _detailslist ?? (_detailslist = new SortableCollectionView<TblCurrencyDailyExchangeViewModel>()); }
            set
            {
                _detailslist = value;

                RaisePropertyChanged("DetailsList");
            }
        }
    }

    #endregion ViewModels

    public class CurrencyDailyExchangeViewModel : ViewModelBase
    {
        public CurrencyDailyExchangeViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.CurrencyDailyExchangeForm.ToString());

                SelectedMainRow.PropertyChanged += SelectedMainRow_PropertyChanged;
                Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                Client.GetTblCurrencyTestAsync(LoggedUserInfo.DatabasEname);
                Client.GetTblCurrencyTestCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Client.UpdateOrInsertTblCurrencyDailyExchangeCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.GenerateCurrencyDailyExchangeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var savedRow =
                            SelectedMainRow.DetailsList.FirstOrDefault(
                                x => x.DocDate == row.DocDate && x.TblCurrency == row.TblCurrency);

                        if (savedRow != null) savedRow.InjectFrom(row);
                    }
                };
                Client.GetTblCurrencyDailyExchangeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.DocDate == row.DocDate);
                        if (oldrow != null) oldrow.InjectFrom(row);
                    }

                    Loading = false;
                };
                Client.DeleteTblCurrencyDailyExchangeCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.votglserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };
            }
        }

        private ObservableCollection<TblCurrencyTest> _currecyList;

        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currecyList; }
            set { _currecyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private void SelectedMainRow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TblCurrency")
            {
                GetDetailData();
            }
            else if (e.PropertyName == "Year")
            {
                GetDetailData();
            }
        }

        internal void GetDetailData()
        {
            if (SelectedMainRow.Year == 0 || SelectedMainRow.TblCurrency == null)
            {
                return;
            }
            SelectedMainRow.DetailsList.Clear();
            var thisYear = new DateTime(SelectedMainRow.Year, 1, 1);
            for (int i = 0; i < GetDaysInYear(SelectedMainRow.Year); i++)
            {
                SelectedMainRow.DetailsList.Add(new TblCurrencyDailyExchangeViewModel
                {
                    DocDate = thisYear.AddDays(i),
                    TblCurrency = SelectedMainRow.TblCurrency,
                });
            }

            Loading = true;
            if (DetailSortBy == null)
            {
                DetailSortBy = "it.votglserial";
            }
            foreach (var row in SelectedMainRow.DetailsList)
            {
                row.ExchRate = 0;
            }
            Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
            Client.GetTblCurrencyDailyExchangeAsync(SelectedMainRow.DetailsList.Count(x => x.votglserial != 0), int.MaxValue, SelectedMainRow.Year, (int)SelectedMainRow.TblCurrency, DetailSortBy,
                DetailFilter, DetailValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteMainRow()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                if (SelectedDetailRow.votglserial != 0)
                {
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;
                    }
                    Loading = true;
                    Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                    Client.DeleteTblCurrencyDailyExchangeAsync(
                        (tblcurrencydailyexchange)new tblcurrencydailyexchange().InjectFrom(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
                else
                {
                    SelectedDetailRow = new TblCurrencyDailyExchangeViewModel();
                }
            }
        }

        public static int GetDaysInYear(int year)
        {
            var thisYear = new DateTime(year, 1, 1);
            var nextYear = new DateTime(year + 1, 1, 1);

            return (nextYear - thisYear).Days;
        }

        public void SaveMainRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.votglserial == 0;
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }

                    var saveRow = new tblcurrencydailyexchange();

                    saveRow.InjectFrom(SelectedDetailRow);
                    Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                    Client.UpdateOrInsertTblCurrencyDailyExchangeAsync(saveRow, save, 0, LoggedUserInfo.DatabasEname);
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        #region Prop

        private TblCurrencyDailyExchangeViewModel _selectedMainRow;

        public TblCurrencyDailyExchangeViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblCurrencyDailyExchangeViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblCurrencyDailyExchangeViewModel _selectedDetailrow;

        public TblCurrencyDailyExchangeViewModel SelectedDetailRow
        {
            get { return _selectedDetailrow ?? (_selectedDetailrow = new TblCurrencyDailyExchangeViewModel()); }
            set { _selectedDetailrow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblCurrencyDailyExchangeViewModel> _selectedDetailrows;

        public ObservableCollection<TblCurrencyDailyExchangeViewModel> SelectedDetailRows
        {
            get { return _selectedDetailrows ?? (_selectedDetailrows = new ObservableCollection<TblCurrencyDailyExchangeViewModel>()); }
            set { _selectedDetailrows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private DateTime? _toDate;

        public DateTime? ToDate
        {
            get { return _toDate; }
            set { _toDate = value; RaisePropertyChanged("ToDate"); }
        }

        private DateTime? _fromDate;

        public DateTime? FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChanged("FromDate"); }
        }

        private float? _exchRate;

        public float? ExchRate
        {
            get
            {
                return _exchRate;
            }
            set
            {
                if ((_exchRate.Equals(value) != true))
                {
                    _exchRate = value;
                    RaisePropertyChanged("ExchRate");
                }
            }
        }

        #endregion Prop

        public void Report()
        {
            var reportName = "CurrencyDailyExchangeReport";

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }

        public void GenerateCurrencyDailyExchange(DateTime selectedDate, DateTime dateTime, float Rate)
        {
            if (SelectedMainRow.TblCurrency != null)
            {
                Client.ServerConnectionsAsync(LoggedUserInfo.Ip, LoggedUserInfo.Port, LoggedUserInfo.DatabasEname);
                Client.GenerateCurrencyDailyExchangeAsync(selectedDate, dateTime, Rate, (int)SelectedMainRow.TblCurrency, LoggedUserInfo.DatabasEname);
            }
        }
    }
}