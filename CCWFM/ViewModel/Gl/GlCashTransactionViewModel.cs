using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using GenericTable = CCWFM.GlService.GenericTable;
using TblCurrencyTest = CCWFM.GlService.TblCurrencyTest;


namespace CCWFM.ViewModel.Gl
{
    public class GlCashTransactionViewModel : ViewModelBase
    {
        public GlCashTransactionViewModel(TblGlCashTypeSetting settings, ObservableCollection<Entity> entityList)
        {
            if (!IsDesignTime)
            {
                Client.GetChainSetupCompleted += (s, sv) =>
                {
                    DefaultBank = new GenericTable
                    {
                        Iserial =
                        Convert.ToInt32(sv.Result.FirstOrDefault(w => w.sGlobalSettingCode == "DefaultBank").sSetupValue)
                    };
                };
                EntityList = entityList;
                TblGlCashTypeSetting = settings;
                GetItemPermissions(settings.Code);
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                GetCustomePermissions(settings.Code);
                var costDimSetupTypeClient = new GlServiceClient();
                costDimSetupTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                costDimSetupTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient = new GlServiceClient();
                Glclient.GetTblRetailCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Glclient.GetTblRetailCurrencyAsync(0, int.MaxValue, "It.Iserial", null, null,
                    LoggedUserInfo.DatabasEname);
                Glclient.GetTblPeriodLineAsync(0, int.MaxValue, 0
                , "It.Iserial", null, null, LoggedUserInfo.DatabasEname);

                Glclient.GetTblPeriodLineCompleted += (s, sv) => {
                    PeriodLines = sv.Result;
                };
                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    TblJournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new ObservableCollection<TblGlCashTransactionHeaderViewModel>();
                SelectedMainRow = new TblGlCashTransactionHeaderViewModel();
                Glclient.GetTblGlCashTransactionHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlCashTransactionHeaderViewModel();
                        newrow.InjectFrom(row);

                        //  newrow.JournalPerRow = row.TblJournal1;
                        newrow.TblJournalAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType != null)
                            newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                        newrow.TblCurrency1PerRow = row.TblCurrency1;
                        newrow.EntityPerRow =
                            sv.entityList.FirstOrDefault(
                                x => x.TblJournalAccountType == row.TblJournalAccountType
                                     && x.Iserial == row.EntityAccount);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                };

                Glclient.GetTblGlCashTransactionDetailCompleted += (s, sv) =>
                    {
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblGlCashTransactionDetailViewModel();
                            newrow.InjectFrom(row);

                            newrow.TblJournalAccountTypePerRow = new GenericTable();
                            if (row.TblJournalAccountType1 != null)
                                newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);

                            newrow.TblJournalAccountType = row.TblJournalAccountType;

                            newrow.EntityPerRow =
                        sv.entityList.FirstOrDefault(
                             x => x.TblJournalAccountType == row.TblJournalAccountType
                             && x.Iserial == row.EntityAccount);


                            newrow.Saved = true;
                            SelectedMainRow.DetailsList.Add(newrow);
                        }

                        Loading = false;
                        calcTotal();
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    };
                Glclient.UpdateOrInsertTblGlCashTransactionHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedMainRow.InjectFrom(ev.Result);
                        SelectedMainRow.DetailsList.Clear();
                        GetDetailData();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblGlCashTransactionHeaderCompleted += (s, ev) =>
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

                Glclient.DeleteTblGlCashTransactionDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.GetTblGlCashTransactionDetailCostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlCashTransactionDetailCostCenterViewModel
                        {
                            CostCenterPerRow = row.TblCostCenter1,
                            CostCenterTypePerRow = new GenericTable(),
                            EntityPerRow = SelectedDetailRow.EntityPerRow,
                        };
                        if (row.TblCostCenterType1 != null)
                            newrow.CostCenterTypePerRow.InjectFrom(row.TblCostCenterType1);

                        newrow.InjectFrom(row);

                        SelectedDetailRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;

                    if (SelectedDetailRow.DetailsList.Any() &&
                        (SelectedSubDetailRow == null))
                    {
                        SelectedSubDetailRow = SelectedDetailRow.DetailsList.FirstOrDefault();
                    }

                    if (DetailSubFullCount == 0 && SelectedDetailRow.DetailsList.Count(x => x.Iserial >= 0) == 0)
                    {
                        AddNewSubDetailRow(false);
                    }
                    if (ExportGrid != null)
                    {
                        if (SelectedSubDetailRow != null)
                        {
                            if (SelectedDetailRow.DetailsList.Any())
                            {
                                if (SelectedDetailRow.DetailsList.Contains(SelectedSubDetailRow))
                                {
                                    ExportGrid.ScrollIntoView(SelectedSubDetailRow, ExportGrid.Columns[1]);
                                }
                            }
                        }
                        ExportGrid.CurrentColumn = ExportGrid.Columns[1];
                        ExportGrid.Focus();
                    }

                };

                Glclient.UpdateOrInsertTblGlCashTransactionDetailCostCentersCompleted += (s, x) =>
                {
                    var savedRow = SelectedDetailRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblGlCashTransactionDetailCostCenterCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    Loading = false;
                    var oldrow = SelectedDetailRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.DetailsList.Remove(oldrow);
                    if (!SelectedDetailRow.DetailsList.Any())
                    {
                        AddNewSubDetailRow(false);
                    }
                };
                GetMaindata();
            }
        }

        internal void calcTotal()
        {
            try
            {
                Total = SelectedMainRow.DetailsList.Sum(w => w.Amount);
            }
            catch (Exception)
            {

                Total = 0;
            }

        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc";
            Loading = true;
            Glclient.GetTblGlCashTransactionHeaderAsync(MainRowList.Count, PageSize, TblGlCashTypeSetting.Iserial, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetDetailData()
        {
            Loading = true;
            Glclient.GetTblGlCashTransactionDetailAsync(SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblGlCashTransactionHeaderViewModel
            {
                CreatedBy = LoggedUserInfo.Iserial,
                TblGlCashTypeSetting = TblGlCashTypeSetting.Iserial,
                DocDate = DateTime.Now,
            };

            if (TblGlCashTypeSetting.TblCurrency != null)
            {
                newrow.TblCurrency = TblGlCashTypeSetting.TblCurrencyDefault;
                newrow.TblCurrency1PerRow = TblGlCashTypeSetting.TblCurrency;
                newrow.ExchangeRate = TblGlCashTypeSetting.TblCurrency.ExchangeRate;
            }
            if (TblGlCashTypeSetting.TblJournalAccountType != null)
            {
                if (TblGlCashTypeSetting.DefaultHeaderEntity1 != null)
                {
                    newrow.EntityPerRow = EntityList.FirstOrDefault(
             x => x.TblJournalAccountType == TblGlCashTypeSetting.EntityHeader1TblJournalAccountType &&
                  x.Iserial == TblGlCashTypeSetting.DefaultHeaderEntity1);
                }
                newrow.TblJournalAccountType = TblGlCashTypeSetting.EntityHeader1TblJournalAccountType;
                newrow.TblJournalAccountTypePerRow = new GenericTable();
                newrow.TblJournalAccountTypePerRow.InjectFrom(TblGlCashTypeSetting.TblJournalAccountType);
            }
            //if (TblGlCashTypeSetting.EntityHeader2TblJournalAccountType != null)
            //{
            //    newrow.EntityHeader2TblJournalAccountType = TblGlCashTypeSetting.EntityHeader2TblJournalAccountType;
            //    newrow.TblJournalAccountType1PerRow = new GenericTable();
            //    newrow.TblJournalAccountType1PerRow.InjectFrom(TblGlCashTypeSetting.TblJournalAccountType1);

            //    if (TblGlCashTypeSetting.DefaultHeaderEntity2 != null)
            //    {
            //        newrow.OffsetEntityPerRow = EntityList.FirstOrDefault(
            // x => x.TblJournalAccountType == TblGlCashTypeSetting.EntityHeader2TblJournalAccountType &&
            //      x.Iserial == TblGlCashTypeSetting.DefaultHeaderEntity2);
            //    }
            //}

            //if (TblGlCashTypeSetting.EntityHeader1TblAccount != null && TblGlCashTypeSetting.EntityHeader1TblAccount != 0)
            //{
            //    newrow.EntityHeader1TblAccount = TblGlCashTypeSetting.EntityHeader1TblAccount;
            //}            
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
            AddNewDetailRow(false);
        }

        public void SaveMainRow(bool approved = false)
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();
                var valiationCollectionDetail = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                if (isvalid && SelectedMainRow.DetailsList != null)
                {
                    if (SelectedMainRow.DetailsList.Any())
                    {
                        var save = SelectedMainRow.Iserial == 0;
                        var saveRow = new TblGlCashTransactionHeader();
                        saveRow.InjectFrom(SelectedMainRow);
                        saveRow.TblGlCashTransactionDetails = new ObservableCollection<TblGlCashTransactionDetail>();
                        if (!Loading)
                        {
                            foreach (var tblGlCashTransactionHeaderViewModel in SelectedMainRow.DetailsList)
                            {
                                var isvalidDetail = Validator.TryValidateObject(tblGlCashTransactionHeaderViewModel,
                                    new ValidationContext(tblGlCashTransactionHeaderViewModel, null, null), valiationCollectionDetail, true);
                                if (!isvalidDetail)
                                {
                                    MessageBox.Show("Data is not Valid");
                                    return;
                                }
                                var newdetailrow = new TblGlCashTransactionDetail();
                                newdetailrow.InjectFrom(tblGlCashTransactionHeaderViewModel);
                                newdetailrow.TblGlCashTransactionDetailCostCenters = new ObservableCollection<TblGlCashTransactionDetailCostCenter>();

                                foreach (var item in tblGlCashTransactionHeaderViewModel.DetailsList)
                                {
                                    isvalidDetail = Validator.TryValidateObject(item,
                             new ValidationContext(item, null, null), valiationCollectionDetail, true);
                                    if (!isvalidDetail)
                                    {
                                        MessageBox.Show("Data is not Valid");
                                        return;
                                    }
                                    var newSubdetailrow = new TblGlCashTransactionDetailCostCenter();
                                    newSubdetailrow.InjectFrom(item);

                                    newdetailrow.TblGlCashTransactionDetailCostCenters.Add(newSubdetailrow);
                                }
                                saveRow.TblGlCashTransactionDetails.Add(newdetailrow);
                            }
                            Loading = true;
                            if (approved)
                            {
                                SelectedMainRow.Approved = approved;
                            }
                        
                            Glclient.UpdateOrInsertTblGlCashTransactionHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial, approved, LoggedUserInfo.DatabasEname);
                        }
                    }
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
                            Glclient.DeleteTblGlCashTransactionHeaderAsync((TblGlCashTransactionHeader)new TblGlCashTransactionHeader().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {

                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Glclient.DeleteTblGlCashTransactionDetailAsync((TblGlCashTransactionDetail)new TblGlCashTransactionDetail().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {

            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (SelectedDetailRow == null)
            {
                currentRowIndex = 0;
            }
            if (checkLastRow)
            {
                var lastrow = SelectedMainRow.DetailsList.ElementAtOrDefault(currentRowIndex);
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(lastrow,
                    new ValidationContext(lastrow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var newrow = new TblGlCashTransactionDetailViewModel
            {
                Description = SelectedMainRow.Description
            };
            //if (TblGlCashTypeSetting.EntityDetail1TblJournalAccountType != null)
            //{
            //    newrow.EntityDetail1TblJournalAccountType = TblGlCashTypeSetting.EntityDetail1TblJournalAccountType;
            //    newrow.TblJournalAccountTypePerRow = new GenericTable();
            //    newrow.TblJournalAccountTypePerRow.InjectFrom(TblGlCashTypeSetting.TblJournalAccountType2);
            //}          
            newrow.Saved = true;
            SelectedMainRow.DetailsList.Add(newrow);
            SelectedDetailRow = newrow;
            //ExportGrid.BeginEdit();
        }

        public void GetSubDetailData()
        {
            if (DetailSubSortBy == null)
                DetailSubSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblGlCashTransactionDetailCostCenterAsync(SelectedDetailRow.DetailsList.Count, int.MaxValue, (int)SelectedDetailRow.Iserial,
                    DetailSubSortBy, DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void DeleteSubDetailRow()
        {
            if (SelectedSubDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedSubDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteTblGlCashTransactionDetailCostCenterAsync(
                                (TblGlCashTransactionDetailCostCenter)new TblGlCashTransactionDetailCostCenter().InjectFrom(row),
                                SelectedDetailRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedDetailRow.DetailsList.Remove(SelectedSubDetailRow);
                            if (!SelectedDetailRow.DetailsList.Any())
                            {
                                AddNewSubDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewSubDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow));

            if (checkLastRow)
            {
                var lastrow = SelectedDetailRow.DetailsList.ElementAtOrDefault(currentRowIndex);
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(lastrow,
                    new ValidationContext(lastrow, null, null), valiationCollection, true);

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

            var amount = SelectedDetailRow.Amount;

            if (SelectedDetailRow.Amount == 0)
            {
                amount = SelectedDetailRow.Amount;
            }
            var newrow = new TblGlCashTransactionDetailCostCenterViewModel
            {
                TblGlCashTransactionDetail = (int)SelectedDetailRow.Iserial,
                EntityPerRow = SelectedDetailRow.EntityPerRow,
                
                Amount = (double)amount
            };
            newrow.CostCenterTypeEnabled = true;
            if (SelectedSubDetailRow != null && SelectedSubDetailRow.TblCostCenterType != null)
            {
                var value = (int)SelectedSubDetailRow.TblCostCenterType;
                var types = SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == value).Select(x => x.TblCostCenterType).Distinct();

                foreach (var type in types)
                {
                    if (SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == type).Sum(x => x.Amount) != amount)
                    {
                        newrow.TblCostCenterType = SelectedSubDetailRow.TblCostCenterType;
                        newrow.CostCenterTypePerRow = SelectedSubDetailRow.CostCenterTypePerRow;
                    }
                }
            }

            SelectedDetailRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedSubDetailRow = newrow;
            if (ExportGrid != null)
            {
                ExportGrid.BeginEdit();
            }

        }

        public void SaveSubDetailRow()
        {
            if (SelectedSubDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedSubDetailRow,
                    new ValidationContext(SelectedSubDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {

                    if (SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == SelectedSubDetailRow.TblCostCenterType).Sum(x => x.Amount) > SelectedDetailRow.Amount)
                    {
                        MessageBox.Show("Cost Center Amount Cannot Be More Than The Transaction Amount");
                        SelectedSubDetailRow.Amount = 0;
                        return;
                    }
                    var save = SelectedSubDetailRow.Iserial == 0;
                    if (SelectedSubDetailRow.TblGlCashTransactionDetail == 0)
                    {
                        SelectedSubDetailRow.TblGlCashTransactionDetail = SelectedDetailRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblGlCashTransactionDetailCostCenter();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblGlCashTransactionDetailCostCentersAsync(rowToSave, save,
                            SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        #region Prop

        public TblGlCashTypeSetting TblGlCashTypeSetting { get; set; }

        private Entity _searchPerRow;

        public Entity EntityPerRow
        {
            get { return _searchPerRow ?? (_searchPerRow = new Entity { Iserial = 0 }); }
            set { _searchPerRow = value; RaisePropertyChanged("EntityPerRow"); }
        }

        private GenericTable _tblJournalAccountTypePerRow;

        public GenericTable TblJournalAccountTypePerRow
        {
            get
            {
                return _tblJournalAccountTypePerRow ?? (_tblJournalAccountTypePerRow = new GenericTable { Iserial = -1 }); ;
            }
            set
            {
                _tblJournalAccountTypePerRow = value;

                RaisePropertyChanged("TblJournalAccountTypePerRow");
            }
        }
        private GenericTable _myVar;

        public GenericTable TblJournalAccountType1PerRow
        {
            get
            {
                return _myVar ?? (_myVar = new GenericTable { Iserial = 6 }); ;
            }
            set
            {
                _myVar = value;

                RaisePropertyChanged("TblJournalAccountType1PerRow");
            }
        }

        private ObservableCollection<GenericTable> _tblJournalAccountTypeList;

        public ObservableCollection<GenericTable> TblJournalAccountTypeList
        {
            get { return _tblJournalAccountTypeList; }
            set { _tblJournalAccountTypeList = value; RaisePropertyChanged("TblJournalAccountTypeList"); }
        }

        public ObservableCollection<TblGlPeriodLine> PeriodLines { get; private set; }
        public GenericTable DefaultBank { get; set; }
        public ObservableCollection<Entity> EntityList { get; set; }



        private ObservableCollection<TblCurrencyTest> _currencyList;

        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }
        private double _Total;

        public double Total
        {
            get { return _Total; }
            set { _Total = value; RaisePropertyChanged("Total"); }
        }
        private ObservableCollection<TblGlCashTransactionHeaderViewModel> _mainRowList;

        public ObservableCollection<TblGlCashTransactionHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGlCashTransactionHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblGlCashTransactionHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlCashTransactionHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblGlCashTransactionHeaderViewModel _selectedMainRow;

        public TblGlCashTransactionHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblGlCashTransactionDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblGlCashTransactionDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblGlCashTransactionDetailViewModel>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private TblGlCashTransactionDetailViewModel _selectedDetailRow;

        public TblGlCashTransactionDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<GenericTable> _costCenterTypeList;

        public ObservableCollection<GenericTable> CostCenterTypeList
        {
            get { return _costCenterTypeList; }
            set
            {
                _costCenterTypeList = value;
                RaisePropertyChanged("CostCenterTypeList");
            }
        }

        private ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel> SelectedSubDetailRows
        {
            get { return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel>()); }
            set
            {
                _selectedSubDetailRows = value;
                RaisePropertyChanged("SelectedSubDetailRows");
            }
        }

        private TblGlCashTransactionDetailCostCenterViewModel _selectedSubDetailRow;

        public TblGlCashTransactionDetailCostCenterViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set
            {
                _selectedSubDetailRow = value;
                RaisePropertyChanged("SelectedSubDetailRow");
            }
        }

        //private ObservableCollection<GenericTable> _costCenterTypeList;

        //public ObservableCollection<GenericTable> CostCenterTypeList
        //{
        //    get { return _costCenterTypeList; }
        //    set
        //    {
        //        _costCenterTypeList = value;
        //        RaisePropertyChanged("CostCenterTypeList");
        //    }
        //}

        #endregion Prop
    }

    #region ViewModels

    public class TblGlCashTransactionHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _TblGlCashTypeSetting;

        public int TblGlCashTypeSetting
        {
            get { return _TblGlCashTypeSetting; }
            set { _TblGlCashTypeSetting = value; RaisePropertyChanged("TblGlCashTypeSetting"); }
        }

        private ObservableCollection<TblGlCashTransactionDetailViewModel> _detailsList;

        public ObservableCollection<TblGlCashTransactionDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblGlCashTransactionDetailViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }
        //private GlServiceClient client = new GlServiceClient();

        private Visibility _visApproved;

        public Visibility VisApproved
        {
            get { return _visApproved; }
            set { _visApproved = value; RaisePropertyChanged("VisApproved"); }
        }


        private bool _approvedField;

        public bool Approved
        {
            get { return _approvedField; }
            set
            {
                _approvedField = value;
                RaisePropertyChanged("Approved");
                VisApproved = Approved ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private int _approvedBy;

        public int ApprovedBy
        {
            get { return _approvedBy; }
            set
            {
                _approvedBy = value;
                RaisePropertyChanged("ApprovedBy");
            }
        }

        private DateTime? _approveDate;

        [ReadOnly(true)]
        public DateTime? ApproveDate
        {
            get { return _approveDate; }
            set
            {
                _approveDate = value;
                RaisePropertyChanged("ApproveDate");
            }
        }

        private string _codeField;

        private int? _createdByField;

        private DateTime? _creationDateField;

        private string _descriptionField;

        private DateTime? _docDateField;

        private double? _exchangeRateField;

        private int _iserialField;

        private int? _tblCurrencyField;

        private TblCurrencyTest _tblCurrency1Field;

        private GenericTable _tblJournalAccountTypeField;

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                _entityPerRow = value; RaisePropertyChanged("EntityPerRow");
                if (EntityPerRow != null) EntityAccount = EntityPerRow.Iserial;
            }
        }
        private int _EntityAccount;

        public int EntityAccount
        {
            get { return _EntityAccount; }
            set { _EntityAccount = value; RaisePropertyChanged("EntityAccount"); }
        }


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

        public int? CreatedBy
        {
            get
            {
                return _createdByField;
            }
            set
            {
                if ((_createdByField.Equals(value) != true))
                {
                    _createdByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public DateTime? DocDate
        {
            get
            {
                return _docDateField;
            }
            set
            {
                if ((_docDateField.Equals(value) != true))
                {
                    _docDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }
        int? _entityHeader1TblJournalAccountTypeField;
        public int? TblJournalAccountType
        {
            get
            {
                return _entityHeader1TblJournalAccountTypeField;
            }
            set
            {
                if ((_entityHeader1TblJournalAccountTypeField.Equals(value) != true))
                {
                    _entityHeader1TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        public double? ExchangeRate
        {
            get
            {
                return _exchangeRateField;
            }
            set
            {
                if ((_exchangeRateField.Equals(value) != true))
                {
                    _exchangeRateField = value;
                    RaisePropertyChanged("ExchangeRate");
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

        public int? TblCurrency
        {
            get
            {
                return _tblCurrencyField;
            }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public TblCurrencyTest TblCurrency1PerRow
        {
            get
            {
                return _tblCurrency1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblCurrency1Field, value) != true))
                {
                    _tblCurrency1Field = value;
                    RaisePropertyChanged("TblCurrency1PerRow");
                    if (TblCurrency1PerRow != null)
                    {
                        TblCurrency = TblCurrency1PerRow.Iserial;
                        ExchangeRate = TblCurrency1PerRow.ExchangeRate;
                    }
                }
            }
        }
        public GenericTable TblJournalAccountTypePerRow
        {
            get
            {
                return _tblJournalAccountTypeField;
            }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountTypeField, value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountTypePerRow");
                    if (TblJournalAccountTypePerRow != null)
                        TblJournalAccountType = TblJournalAccountTypePerRow.Iserial;
                }
            }
        }
    }

    public class TblGlCashTransactionDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel> _detailsList;

        public ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblGlCashTransactionDetailCostCenterViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        private bool _saved;

        public bool Saved
        {
            get { return _saved; }
            set { _saved = value; RaisePropertyChanged("Saved"); }
        }

        private int? _tblGlCashTransactionDetail;

        public int? TblGlCashTransactionDetail1
        {
            get
            {
                return _tblGlCashTransactionDetail;
            }
            set
            {
                if ((_tblGlCashTransactionDetail.Equals(value) != true))
                {
                    _tblGlCashTransactionDetail = value;
                    RaisePropertyChanged("TblGlCashTransactionDetail1");
                }
            }
        }

        private double _amountField;
   
        private string _descriptionField;

        private int? _entityDetail1TblJournalAccountTypeField;

        private int _iserialField;
    


    

        private int _tblGlCashTransactionHeaderField;

    

        private GenericTable _tblJournalAccountTypeField;

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                _entityPerRow = value; RaisePropertyChanged("EntityPerRow");
                if (EntityPerRow != null) EntityAccount = EntityPerRow.Iserial;
            }
        }

        private int _EntityAccount;

        public int EntityAccount
        {
            get { return _EntityAccount; }
            set { _EntityAccount = value; RaisePropertyChanged("EntityAccount"); }
        }



        public double Amount
        {
            get
            {
                return _amountField;
            }
            set
            {
                if ((_amountField.Equals(value) != true))
                {
                    _amountField = value;
                    RaisePropertyChanged("Amount");
                }
            }
        }

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public int? TblJournalAccountType
        {
            get
            {
                return _entityDetail1TblJournalAccountTypeField;
            }
            set
            {
                if ((_entityDetail1TblJournalAccountTypeField.Equals(value) != true))
                {
                    _entityDetail1TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
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

        public int TblGlCashTransactionHeader
        {
            get
            {
                return _tblGlCashTransactionHeaderField;
            }
            set
            {
                if ((_tblGlCashTransactionHeaderField.Equals(value) != true))
                {
                    _tblGlCashTransactionHeaderField = value;
                    RaisePropertyChanged("TblGlCashTransactionHeader");
                }
            }
        }

        public GenericTable TblJournalAccountTypePerRow
        {
            get
            {
                return _tblJournalAccountTypeField;
            }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountTypeField, value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountTypePerRow");
                    if (TblJournalAccountTypePerRow != null)
                        TblJournalAccountType = TblJournalAccountTypePerRow.Iserial;
                }
            }
        }

    }

    public class TblGlCashTransactionDetailCostCenterViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private int? _tblCostCenter;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenter")]
        public int? TblCostCenter
        {
            get { return _tblCostCenter; }
            set { _tblCostCenter = value; }
        }

        private TblCostCenter _costCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _costCenterPerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterPerRow, value) != true))
                {
                    _costCenterPerRow = value;
                    RaisePropertyChanged("CostCenterPerRow");
                    if (_costCenterPerRow != null)
                    {
                        TblCostCenter = _costCenterPerRow.Iserial;
                        CostCenterTypeEnabled = false;
                    }
                    else
                    {
                        CostCenterTypeEnabled = true;
                    }
                }
            }
        }

        private bool _costCenterTypeEnabled;

        public bool CostCenterTypeEnabled
        {
            get { return _costCenterTypeEnabled; }
            set { _costCenterTypeEnabled = value; RaisePropertyChanged("CostCenterTypeEnabled"); }
        }

        private int? _tblCostCenterType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenterType")]
        public int? TblCostCenterType
        {
            get { return _tblCostCenterType; }
            set { _tblCostCenterType = value; RaisePropertyChanged("TblCostCenterType"); }
        }

        private GenericTable _costCenterTypePerRow;

        public GenericTable CostCenterTypePerRow
        {
            get { return _costCenterTypePerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterTypePerRow, value) != true))
                {
                    _costCenterTypePerRow = value;
                    RaisePropertyChanged("CostCenterTypePerRow");
                    if (CostCenterTypePerRow != null) TblCostCenterType = CostCenterTypePerRow.Iserial;
                }
            }
        }

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                if ((ReferenceEquals(_entityPerRow, value) != true))
                {
                    _entityPerRow = value;
                    RaisePropertyChanged("EntityPerRow");
                }
            }
        }

        private int _TblGlCashTransactionDetail;

        public int TblGlCashTransactionDetail
        {
            get { return _TblGlCashTransactionDetail; }
            set { _TblGlCashTransactionDetail = value; RaisePropertyChanged("TblGlCashTransactionDetail"); }
        }

        private double _ratio;

        public double Ratio
        {
            get { return _ratio; }
            set { _ratio = value; RaisePropertyChanged("Ratio"); }
        }

        private double _amount;

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; RaisePropertyChanged("Amount"); }
        }
    }

    #endregion ViewModels
}