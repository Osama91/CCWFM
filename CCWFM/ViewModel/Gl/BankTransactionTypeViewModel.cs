using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class BankTransactionTypeViewModel : ViewModelBase
    {
        public BankTransactionTypeViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.BankTransactionType.ToString());
                Glclient = new GlServiceClient();

                var currencyClient = new GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) => { BankTransactionTypeGroupList = sv.Result; };
                currencyClient.GetGenericAsync("TblBankTransactionTypeGroup", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblBankTransactionTypeViewModel>();
                SelectedMainRow = new TblBankTransactionTypeViewModel();

                Glclient.GetTblBankTransactionTypeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBankTransactionTypeViewModel
                        {
                            BankTransactionTypeGroupPerRow = new GenericTable()
                        };
                        newrow.InjectFrom(row);
                        if (row.TblBankTransactionTypeGroup1 != null)
                            newrow.BankTransactionTypeGroupPerRow.InjectFrom(row.TblBankTransactionTypeGroup1);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any())
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
                        ExportGrid.ExportExcel("BankTransactionType");
                    }
                };

                Glclient.UpdateOrInsertTblBankTransactionTypesCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                };
                Glclient.DeleteTblBankTransactionTypeCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblBankTransactionTypeAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblBankTransactionTypeViewModel();
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
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblBankTransactionType();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblBankTransactionTypesAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                }
            }
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
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Glclient.DeleteTblBankTransactionTypeAsync((TblBankTransactionType)new TblBankTransactionType().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        #region Prop

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _bankTransactionTypeList;

        public ObservableCollection<GenericTable> BankTransactionTypeList
        {
            get { return _bankTransactionTypeList; }
            set { _bankTransactionTypeList = value; RaisePropertyChanged("BankTransactionTypeList"); }
        }

        private SortableCollectionView<TblBankTransactionTypeViewModel> _mainRowList;

        public SortableCollectionView<TblBankTransactionTypeViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<GenericTable> _bankTransactionTypeGroupList;

        public ObservableCollection<GenericTable> BankTransactionTypeGroupList
        {
            get { return _bankTransactionTypeGroupList ?? (_bankTransactionTypeGroupList = new ObservableCollection<GenericTable>()); }
            set { _bankTransactionTypeGroupList = value; RaisePropertyChanged("BankTransactionTypeGroupList"); }
        }

        private ObservableCollection<TblBankTransactionTypeViewModel> _selectedMainRows;

        public ObservableCollection<TblBankTransactionTypeViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBankTransactionTypeViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBankTransactionTypeViewModel _selectedMainRow;

        public TblBankTransactionTypeViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblBankTransactionTypeViewModel : GenericViewModel
    {
        private int? _bankTransactionTypeGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBankTransactionTypeGroup")]
        public int? TblBankTransactionTypeGroup
        {
            get { return _bankTransactionTypeGroup; }
            set { _bankTransactionTypeGroup = value; RaisePropertyChanged("TblBankTransactionTypeGroup"); }
        }

        private GenericTable _bankTransactionTypeGroupPerRow;

        public GenericTable BankTransactionTypeGroupPerRow
        {
            get { return _bankTransactionTypeGroupPerRow; }
            set
            {
                _bankTransactionTypeGroupPerRow = value; RaisePropertyChanged("BankTransactionTypeGroupPerRow");
                if (BankTransactionTypeGroupPerRow != null)
                {
                    if (BankTransactionTypeGroupPerRow.Iserial != 0)
                    {
                        TblBankTransactionTypeGroup = BankTransactionTypeGroupPerRow.Iserial;
                    }
                }
            }
        }
    }

    #endregion ViewModels
}