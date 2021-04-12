using System;
using System.Windows;
using CCWFM.Helpers.Enums;
using System.ComponentModel;
using CCWFM.BankDepositService;
using CCWFM.Helpers.AuthenticationHelpers;
using System.Collections.ObjectModel;
using Omu.ValueInjecter.Silverlight;
using System.Linq;
using CCWFM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCWFM.ViewModel.GenericViewModels;
using GalaSoft.MvvmLight.Command;
using Os.Controls.DataGrid.Events;
using CCWFM.Helpers.LocalizationHelpers;

namespace CCWFM.BankDepositService
{
    public partial class TblStoreCommission
    {
        public string code
        {
            get { return TblStore1?.code; }
        }
        public string ENAME
        {
            get { return TblStore1?.ENAME; }
        }
    }

}
namespace CCWFM.ViewModel.Gl
{
    public class StoreCommissionViewModel : ViewModelStructuredBase
    {
        BankDepositServiceClient BankDepositClient = Helpers.Services.Instance.GetBankDepositServiceClient();
        public StoreCommissionViewModel() : base(PermissionItemName.StoreCommission)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new ObservableCollection<TblStoreCommission>();

                BankDepositClient.GetLookUpStoreCompleted += (s, e) =>
                {
                    StoreList = e.Result;
                    foreach (var item in MainRowList)
                    {
                        item.TblStore1 = StoreList.FirstOrDefault(st => st.iserial == item.Tblstore);
                    }
                };

                GetComboData();
                BankDepositClient.GetStoreCommissionCompleted += (s, sv) =>
                {
                    MainRowList.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStoreCommission();

                        newrow.InjectFrom(row);
                        newrow.TblStore1 = StoreList.FirstOrDefault(st => st.iserial == row.Tblstore);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    if (SearchWindow != null)
                    {
                        SearchWindow.Loading = false;
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                BankDepositClient.UpdateOrInsertStoreCommissionCompleted += (s, x) => SaveCompleted(x.Result, x.Error);
                FilterCommand = new RelayCommand<FilterEvent>((e) => {
                    string filter;
                    Dictionary<string, object> valueObjecttemp;
                    GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
                    Filter = filter;
                    ValuesObjects = valueObjecttemp;
                    GetMaindata();
                });
                GetMaindata();
                //AddNewMainRow(false);
            }
        }

        private void SaveCompleted(ObservableCollection<TblStoreCommission> Result, Exception Error)
        {
            if (Error != null)
            {
                MessageBox.Show(Helper.GetInnerExceptionMessage(Error));
                Loading = false;
                return;
            }
            else
                MessageBox.Show(strings.SavedMessage);
            foreach (var item in Result)
            {
                TblStoreCommission savedRow = new TblStoreCommission();
                //if (outindex >= 0 && MainRowList.Count > outindex)
                //    savedRow = MainRowList.ElementAt(outindex);
                savedRow = MainRowList.FirstOrDefault(s => s.Tblstore == item.Tblstore);
                if (savedRow != null)
                {
                    savedRow.InjectFrom(Result);
                    savedRow.TblStore1 = StoreList.FirstOrDefault(b => b.iserial == savedRow.Tblstore);
                }
            }
            DeleteCommand.RaiseCanExecuteChanged();
            IsNewChanged();
            Loading = false;
        }

        #region Methods
        
        public bool ValidHeaderData()
        {
            if (SelectedMainRow.Tblstore <= 0)
            {
                MessageBox.Show(strings.ReqStore);
                return false;
            }
            return true;
        }

        #endregion

        #region Operations

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Tblstore";
            BankDepositClient.GetStoreCommissionAsync(SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                //var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                //    MessageBoxButton.OKCancel);
                //if (res == MessageBoxResult.OK)
                //{
                //    //BankDepositClient.DeleteTblStoreCommissionAsync((TblTblStoreCommission)new
                //    //    TblTblStoreCommission().InjectFrom(SelectedMainRow),
                //    //    MainRowList.IndexOf(SelectedMainRow));
                //}
            }
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                SelectedMainRow = new TblStoreCommission();
                MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
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
                    var saveRow = new TblStoreCommission();
                    saveRow.InjectFrom(SelectedMainRow);

                    //var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    //if (mainRowIndex < 0)
                    //{
                    //    MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    //}
                    BankDepositClient.UpdateOrInsertStoreCommissionAsync(MainRowList, //mainRowIndex,
                        LoggedUserInfo.DatabasEname);
                    Loading = true;
                }
            }
        }
        public void GetComboData()
        {            
            BankDepositClient.GetLookUpStoreAsync(LoggedUserInfo.DatabasEname, false);
        }

        #endregion

        #region Properties

        private ObservableCollection<TblStoreCommission> _mainRowList;
        public ObservableCollection<TblStoreCommission> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }
        private ObservableCollection<TblStoreCommission> _selectedMainRows;
        public ObservableCollection<TblStoreCommission> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStoreCommission>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private TblStoreCommission _selectedMainRow;
        public TblStoreCommission SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new TblStoreCommission());
            }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                IsNewChanged();
            }
        }
        
        #region Combo Data

        ObservableCollection<TblStore> _storeList = new ObservableCollection<TblStore>();
        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set { _storeList = value; RaisePropertyChanged(nameof(StoreList)); }
        }

        RelayCommand<FilterEvent> filterCommand;
        public RelayCommand<FilterEvent> FilterCommand
        {
            get { return filterCommand; }
            set { filterCommand = value; RaisePropertyChanged(nameof(FilterCommand)); }
        }
        #endregion

        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
        }
        public override bool CanSaveRecord()
        {
            var result = base.CanSaveRecord();
            return result;
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData();// && ValidDetailData();
        }
        public override void Search()
        {
            //MainRowList.Clear();
            //GetMaindata();
            //if (SearchWindow == null)
            //    SearchWindow = new GenericSearchWindow(GetSearchModel());
            //GenericSearchViewModel<TblStoreCommission> vm =
            //    new GenericSearchViewModel<TblStoreCommission>() { Title = "Cash Deposit Search" };
            //vm.FilteredItemsList = MainRowList;
            //vm.ItemsList = MainRowList;
            //vm.ResultItemsList.CollectionChanged += (s, e) => {
            //    if (e.Action == NotifyCollectionChangedAction.Add)
            //        SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
            //};
            //vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
            //    Filter = vm.Filter;
            //    ValuesObjects = vm.ValuesObjects;
            //    GetMaindata();
            //},
            //(o) => {
            //    return true;//هنا الصلاحيات
            //});
            //SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header = strings.Store,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblStoreCommission.TblStore1),nameof(TblStore.ENAME)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblStoreCommission.TblStore1),nameof(TblStore.ENAME)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ManagerComm,
                        PropertyPath=nameof(TblStoreCommission.ManagerComm),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.AssistantComm,
                        PropertyPath=nameof(TblStoreCommission.AssistantComm),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.SalesManComm,
                        PropertyPath=nameof(TblStoreCommission.SalesManComm),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Active,
                        PropertyPath=nameof(TblStoreCommission.IsActive),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ManagerTax,
                        PropertyPath=nameof(TblStoreCommission.ManagerTax),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.AssistantTax,
                        PropertyPath=nameof(TblStoreCommission.AssistantTax),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.SalesManTax,
                        PropertyPath=nameof(TblStoreCommission.SalesManTax),
                        StringFormat="0.#",
                    },
                };
        }
        public override void DeleteRecord()
        {
            this.DeleteMainRow();
            AddNewMainRow(false);
            base.DeleteRecord();
        }
        public override bool CanDeleteRecord()
        {
            return false;
        }
        public override void Cancel()
        {
            GetMaindata();
            base.Cancel();
        }
     
        #endregion

    }
}