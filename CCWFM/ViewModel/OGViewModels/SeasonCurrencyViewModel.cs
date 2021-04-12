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

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblSeasonCurrenciesViewModel : PropertiesViewModelBase
    {

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _currencyCode;

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; RaisePropertyChanged("CurrencyCode"); }
        }
        private double _exRate;

        public double ExRate
        {
            get { return _exRate; }
            set { _exRate = value; RaisePropertyChanged("ExRate"); }
        }

        private int? _tblLkpSeasonField;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSeason")]
        public int? TblLkpSeason
        {
            get
            {
                return _tblLkpSeasonField;
            }
            set
            {
                if ((_tblLkpSeasonField.Equals(value) != true))
                {
                    if (_tblLkpSeasonField == value)
                    {
                        return;
                    }
                    _tblLkpSeasonField = value;
                    RaisePropertyChanged("TblLkpSeason");

                }
            }
        }

        private TblLkpSeason _seasonPerRow;
        public TblLkpSeason SeasonPerRow
        {
            get
            {
                return _seasonPerRow;
            }
            set
            {
                if ((ReferenceEquals(_seasonPerRow, value) != true))
                {
                    _seasonPerRow = value;
                    RaisePropertyChanged("SeasonPerRow");
                }
            }
        }
    }

    public class SeasonCurrenciesViewModel : ViewModelBase
    {
        public SeasonCurrenciesViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.SeasonCurrenciesForm.ToString());
                MainRowList = new SortableCollectionView<TblSeasonCurrenciesViewModel>();
                SelectedMainRow = new TblSeasonCurrenciesViewModel();
                Client.GetTblCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Client.GetTblCurrencyAsync(0, int.MaxValue, "it.Iserial", null, null);
                var seasonClient = new CRUD_ManagerServiceClient();
                seasonClient.GetAllSeasonsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SeasonList.All(x => x.Iserial != row.Iserial))
                        {
                            SeasonList.Add(new TblLkpSeason().InjectFrom(row) as TblLkpSeason);
                        }
                    }
                    GetMaindata();
                };
                seasonClient.GetAllSeasonsAsync();

                Client.GetTblSeasonCurrencyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSeasonCurrenciesViewModel();
                        newrow.SeasonPerRow = new TblLkpSeason();
                        newrow.SeasonPerRow = SeasonList.FirstOrDefault(x => x.Iserial == row.TblLkpSeason);
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

                Client.UpdateOrInsertTblSeasonCurrencyCompleted += (s, x) =>
                {
                    var savedRow = (TblSeasonCurrenciesViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
                Client.DeleteTblSeasonCurrencyCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    Loading = false;
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };


            }
        }



        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblSeasonCurrencyAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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

                            Client.DeleteTblSeasonCurrencyAsync(
                                (TblSeasonCurrency)new TblSeasonCurrency().InjectFrom(row), MainRowList.IndexOf(row));
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
            var newrow = new TblSeasonCurrenciesViewModel();

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblSeasonCurrency();
                    saveRow.InjectFrom(SelectedMainRow);
                    Loading = true;
                    Client.UpdateOrInsertTblSeasonCurrencyAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }
        
        #region Prop

        private SortableCollectionView<TblSeasonCurrenciesViewModel> _mainRowList;

        public SortableCollectionView<TblSeasonCurrenciesViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSeasonCurrenciesViewModel> _selectedMainRows;

        public ObservableCollection<TblSeasonCurrenciesViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSeasonCurrenciesViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<TblLkpSeason> _seasonList;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged("SeasonList"); }
        }


        private ObservableCollection<TblCurrency> _currencyList;
        public ObservableCollection<TblCurrency> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private TblSeasonCurrenciesViewModel _selectedMainRow;

        public TblSeasonCurrenciesViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop

    }
}