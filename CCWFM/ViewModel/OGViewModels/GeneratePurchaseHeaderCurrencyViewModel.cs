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
    public class TblGeneratePurchaseHeaderCurrenciesViewModel : PropertiesViewModelBase
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

        private double _customerExRate;

        public double CustomerExRate
        {
            get { return _customerExRate; }
            set { _customerExRate = value; RaisePropertyChanged("CustomerExRate"); }
        }

        private double _vendorExRate;

        public double VendorExRate
        {
            get { return _vendorExRate; }
            set { _vendorExRate = value; RaisePropertyChanged("VendorExRate"); }
        }

        private int? _tblGeneratePurchaseHeader;

        public int? TblGeneratePurchaseHeader
        {
            get
            {
                return _tblGeneratePurchaseHeader;
            }
            set
            {
                if ((_tblGeneratePurchaseHeader.Equals(value) != true))
                {
                    if (_tblGeneratePurchaseHeader == value)
                    {
                        return;
                    }
                    _tblGeneratePurchaseHeader = value;
                    RaisePropertyChanged("TblGeneratePurchaseHeader");
                }
            }
        }
    }

    public class GeneratePurchaseHeaderCurrenciesViewModel : ViewModelBase
    {
        private readonly int _generatepurchasehaderIserial;

        public GeneratePurchaseHeaderCurrenciesViewModel(int generatePurchaseHeader)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                _generatepurchasehaderIserial = generatePurchaseHeader;
                GetItemPermissions(PermissionItemName.GeneratePurchase.ToString());
                MainRowList = new SortableCollectionView<TblGeneratePurchaseHeaderCurrenciesViewModel>();
                SelectedMainRow = new TblGeneratePurchaseHeaderCurrenciesViewModel();
                Client.GetTblCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Client.GetTblCurrencyAsync(0, int.MaxValue, "it.Iserial", null, null);

                GetMaindata();
                Client.TblGeneratePurchaseHeaderCurrencyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGeneratePurchaseHeaderCurrenciesViewModel();
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

                Client.UpdateOrInsertTblGeneratePurchaseHeaderCurrencyCompleted += (s, x) =>
                {
                    var savedRow = (TblGeneratePurchaseHeaderCurrenciesViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
                Client.DeleteTblGeneratePurchaseHeaderCurrencyCompleted += (s, ev) =>
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
            {
                SortBy = "it.Iserial";
            }
          
            if (Filter != null)
            {
                
                Filter = Filter + "it.TblGeneratePurchaseHeader==(@b)";
            }
            else
            {
                ValuesObjects = new Dictionary<string, object>();

                Filter = "it.TblGeneratePurchaseHeader==(@b)";
            }
            ValuesObjects.Add("b", _generatepurchasehaderIserial);
            Loading = true;
            Client.TblGeneratePurchaseHeaderCurrencyAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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

                            Client.DeleteTblGeneratePurchaseHeaderCurrencyAsync(
                                (TblGeneratePurchaseHeaderCurrency)new TblGeneratePurchaseHeaderCurrency().InjectFrom(row), MainRowList.IndexOf(row));
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
            var newrow = new TblGeneratePurchaseHeaderCurrenciesViewModel
            {
                TblGeneratePurchaseHeader = _generatepurchasehaderIserial
            };
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
                    var saveRow = new TblGeneratePurchaseHeaderCurrency();
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblGeneratePurchaseHeader = _generatepurchasehaderIserial;
                    Loading = true;
                    Client.UpdateOrInsertTblGeneratePurchaseHeaderCurrencyAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblGeneratePurchaseHeaderCurrenciesViewModel> _mainRowList;

        public SortableCollectionView<TblGeneratePurchaseHeaderCurrenciesViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGeneratePurchaseHeaderCurrenciesViewModel> _selectedMainRows;

        public ObservableCollection<TblGeneratePurchaseHeaderCurrenciesViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGeneratePurchaseHeaderCurrenciesViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<TblCurrency> _currencyList;

        public ObservableCollection<TblCurrency> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private TblGeneratePurchaseHeaderCurrenciesViewModel _selectedMainRow;

        public TblGeneratePurchaseHeaderCurrenciesViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}