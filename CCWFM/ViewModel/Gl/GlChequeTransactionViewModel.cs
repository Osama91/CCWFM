using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using GenericTable = CCWFM.GlService.GenericTable;
using TblAccount = CCWFM.GlService.TblAccount;
using TblBankCheque = CCWFM.GlService.TblBankCheque;
using TblCurrencyTest = CCWFM.GlService.TblCurrencyTest;
using TblGlChequeStatu = CCWFM.GlService.TblGlChequeStatu;
using TblGlChequeTransactionDetail = CCWFM.GlService.TblGlChequeTransactionDetail;
using TblGlChequeTransactionHeader = CCWFM.GlService.TblGlChequeTransactionHeader;
using TblGlChequeTypeSetting = CCWFM.GlService.TblGlChequeTypeSetting;

namespace CCWFM.ViewModel.Gl
{
    public class GlChequeTransactionViewModel : ViewModelBase
    {
        public GlChequeTransactionViewModel(TblGlChequeTypeSetting settings, ObservableCollection<Entity> entityList)
        {
            if (!IsDesignTime)
            {
                //Client.GetChainSetupCompleted += (s, sv) =>
                //{
                //    DefaultBank = new GenericTable
                //    {
                //        Iserial =
                //        Convert.ToInt32(sv.Result.FirstOrDefault(w => w.sGlobalSettingCode == "DefaultBank").sSetupValue)
                //    };
                //};

             

              


                Glclient.GetTblPeriodLineAsync(0, int.MaxValue, 0
          , "It.Iserial", null, null, LoggedUserInfo.DatabasEname);

                Glclient.GetTblPeriodLineCompleted += (s, sv) => {
                    PeriodLines = sv.Result;
                };
                EntityList = entityList;
                TblGlChequeTypeSetting = settings;
                GetItemPermissions(settings.Code);
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                GetCustomePermissions(settings.Code);

                Glclient = new GlServiceClient();
                Glclient.GetTblRetailCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                Glclient.GetTblRetailCurrencyAsync(0, int.MaxValue, "It.Iserial", null, null,
                    LoggedUserInfo.DatabasEname);

                var costDimSetupTypeClient = new GlServiceClient();
                costDimSetupTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                costDimSetupTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                ChequeStatusPerRow = new GenericTable { Iserial = 4 };
                if (settings.Iserial == 8 || settings.Iserial == 14)
                {
                    ChequeStatusPerRow.Iserial = 0;
                }

                var journalAccountTypeClient = new GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    TblJournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new ObservableCollection<TblGlChequeTransactionHeaderViewModel>();
                SelectedMainRow = new TblGlChequeTransactionHeaderViewModel();
                Glclient.GetTblGlChequeTransactionHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlChequeTransactionHeaderViewModel();
                        newrow.InjectFrom(row);

                        newrow.UseEntityHeader1 = TblGlChequeTypeSetting.UseEntityHeader1;
                        newrow.UseEntityHeader2 = TblGlChequeTypeSetting.UseEntityHeader2;
                        newrow.TblGlChequeTypeSetting1PerRow = row.TblGlChequeTypeSetting1;
                        newrow.TblJournalAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType != null)
                            newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                        newrow.TblJournalAccountType1PerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.TblJournalAccountType1PerRow.InjectFrom(row.TblJournalAccountType1);
                        newrow.TblAccountPerRow = row.TblAccount;
                        newrow.TblAccount1PerRow = row.TblAccount1;
                        newrow.TblCurrency1PerRow = row.TblCurrency1;
                        newrow.EntityPerRow =
                            sv.entityList.FirstOrDefault(
                                x => x.TblJournalAccountType == row.EntityHeader1TblJournalAccountType
                                     && x.Iserial == row.EntityHeader1);

                        newrow.OffsetEntityPerRow =
                            sv.entityList.FirstOrDefault(
                                x => x.TblJournalAccountType == row.EntityHeader2TblJournalAccountType
                                     && x.Iserial == row.EntityHeader2);

                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                };

                Glclient.GetLockupFromPreTransactionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SelectedMainRow.DetailsList.All(x => x.TblGlChequeTransactionDetail1 != row.Iserial))
                        {
                            var newrow = new TblGlChequeTransactionDetailViewModel();
                            newrow.InjectFrom(row);
                            newrow.ChequePerRow = new TblBankCheque();
                            newrow.Iserial = 0;
                            newrow.TblGlChequeTransactionHeader = 0;
                            newrow.TblGlChequeTransactionDetail1 = row.Iserial;
                            if (row.TblBankCheque1 != null) newrow.ChequePerRow.InjectFrom(row.TblBankCheque1);
                            newrow.TblJournalAccountTypePerRow = new GenericTable();
                            if (row.TblJournalAccountType != null)
                                newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                            newrow.TblJournalAccountType1PerRow = new GenericTable();
                            if (row.TblJournalAccountType1 != null)
                                newrow.TblJournalAccountType1PerRow.InjectFrom(row.TblJournalAccountType1);
                            newrow.EntityDetail1TblJournalAccountType = row.EntityDetail1TblJournalAccountType;
                            newrow.EntityDetail2TblJournalAccountType = row.EntityDetail2TblJournalAccountType;
                            newrow.EntityPerRow =
                        sv.entityList.FirstOrDefault(
                             x => x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType
                             && x.Iserial == row.EntityDetail1);

                            newrow.OffsetEntityPerRow =
                                 sv.entityList.FirstOrDefault(
                                    x => x.TblJournalAccountType == row.EntityDetail2TblJournalAccountType
                                    && x.Iserial == row.EntityDetail2);
                            newrow.Saved = true;
                            if (settings.EntityDetail1TblAccount != 0)
                            {
                                newrow.EntityDetail1TblAccount = settings.EntityDetail1TblAccount;
                            }

                            if (settings.EntityDetail2TblAccount != 0)
                            {
                                newrow.EntityDetail2TblAccount = settings.EntityDetail2TblAccount;
                            }

                            SelectedMainRow.DetailsList.Add(newrow);
                        }
                    }

                    Loading = false;

                    SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    calcTotal();
                };
                Glclient.GetTblGlChequeTransactionDetailCompleted += (s, sv) =>
                    {
                        foreach (var row in sv.Result)
                        {
                            var newrow = new TblGlChequeTransactionDetailViewModel();
                            newrow.InjectFrom(row);
                            newrow.ChequePerRow = new TblBankCheque();

                            if (row.TblBankCheque1 != null) newrow.ChequePerRow.InjectFrom(row.TblBankCheque1);
                            newrow.TblJournalAccountTypePerRow = new GenericTable();
                            if (row.TblJournalAccountType != null)
                                newrow.TblJournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType);
                            newrow.TblJournalAccountType1PerRow = new GenericTable();
                            if (row.TblJournalAccountType1 != null)
                                newrow.TblJournalAccountType1PerRow.InjectFrom(row.TblJournalAccountType1);

                            newrow.EntityDetail1TblJournalAccountType = row.EntityDetail1TblJournalAccountType;
                            newrow.EntityDetail2TblJournalAccountType = row.EntityDetail2TblJournalAccountType;

                            newrow.EntityPerRow =
                        sv.entityList.FirstOrDefault(
                             x => x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType
                             && x.Iserial == row.EntityDetail1);

                            newrow.OffsetEntityPerRow =
                                 sv.entityList.FirstOrDefault(
                                    x => x.TblJournalAccountType == row.EntityDetail2TblJournalAccountType
                                    && x.Iserial == row.EntityDetail2);

                            newrow.ContractHeaderPerRow = sv.ContractList.FirstOrDefault(
                             x => x.Iserial == row.TblContractHeader
                             );

                            newrow.Saved = true;
                            SelectedMainRow.DetailsList.Add(newrow);
                        }

                        Loading = false;
                        calcTotal();
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    };
                Glclient.UpdateOrInsertTblGlChequeTransactionHeaderCompleted += (s, ev) =>
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
                Glclient.DeleteTblGlChequeTransactionHeaderCompleted += (s, ev) =>
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

                Glclient.DeleteTblGlChequeTransactionDetailCompleted += (s, ev) =>
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

                Glclient.GetTblBankChequeCostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBankChequeCostCenterViewModel
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

                Glclient.UpdateOrInsertTblBankChequeCostCentersCompleted += (s, x) =>
                {
                    var savedRow = SelectedDetailRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteTblBankChequeCostCenterCompleted += (s, ev) =>
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
            Glclient.GetTblGlChequeTransactionHeaderAsync(MainRowList.Count, PageSize, TblGlChequeTypeSetting.Iserial, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetDetailData()
        {
            Loading = true;
            Glclient.GetTblGlChequeTransactionDetailAsync(SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblGlChequeTransactionHeaderViewModel
            {
                CreatedBy = LoggedUserInfo.Iserial,
                UseEntityHeader1 = TblGlChequeTypeSetting.UseEntityHeader1,
                UseEntityHeader2 = TblGlChequeTypeSetting.UseEntityHeader2,
                TblGlChequeTypeSetting = TblGlChequeTypeSetting.Iserial,
                EntityHeader1PostDr = TblGlChequeTypeSetting.EntityHeader1PostDr,
                EntityHeader2PostDr = TblGlChequeTypeSetting.EntityHeader2PostDr,
                DocDate = DateTime.Now,
            };

            if (TblGlChequeTypeSetting.TblCurrency != null)
            {
                newrow.TblCurrency = TblGlChequeTypeSetting.TblCurrency.Iserial;
                newrow.TblCurrency1PerRow = TblGlChequeTypeSetting.TblCurrency;
                newrow.ExchangeRate = TblGlChequeTypeSetting.TblCurrency.ExchangeRate;
            }
            if (TblGlChequeTypeSetting.EntityHeader1TblJournalAccountType != null)
            {
                if (TblGlChequeTypeSetting.DefaultHeaderEntity1 != null)
                {
                    newrow.EntityPerRow = EntityList.FirstOrDefault(
             x => x.TblJournalAccountType == TblGlChequeTypeSetting.EntityHeader1TblJournalAccountType &&
                  x.Iserial == TblGlChequeTypeSetting.DefaultHeaderEntity1);
                }

                newrow.EntityHeader1TblJournalAccountType = TblGlChequeTypeSetting.EntityHeader1TblJournalAccountType;
                newrow.TblJournalAccountTypePerRow = new GenericTable();
                newrow.TblJournalAccountTypePerRow.InjectFrom(TblGlChequeTypeSetting.TblJournalAccountType);
            }
            if (TblGlChequeTypeSetting.EntityHeader2TblJournalAccountType != null)
            {
                newrow.EntityHeader2TblJournalAccountType = TblGlChequeTypeSetting.EntityHeader2TblJournalAccountType;
                newrow.TblJournalAccountType1PerRow = new GenericTable();
                newrow.TblJournalAccountType1PerRow.InjectFrom(TblGlChequeTypeSetting.TblJournalAccountType1);

                if (TblGlChequeTypeSetting.DefaultHeaderEntity2 != null)
                {
                    newrow.OffsetEntityPerRow = EntityList.FirstOrDefault(x => x.TblJournalAccountType == TblGlChequeTypeSetting.EntityHeader2TblJournalAccountType &&
                  x.Iserial == TblGlChequeTypeSetting.DefaultHeaderEntity2);
                }
            }

            if (TblGlChequeTypeSetting.EntityHeader1TblAccount != null && TblGlChequeTypeSetting.EntityHeader1TblAccount != 0)
            {
                newrow.EntityHeader1TblAccount = TblGlChequeTypeSetting.EntityHeader1TblAccount;
            }
            if (TblGlChequeTypeSetting.EntityHeader2TblAccount != null && TblGlChequeTypeSetting.EntityHeader2TblAccount != 0)
            {
                newrow.EntityHeader2TblAccount = TblGlChequeTypeSetting.EntityHeader2TblAccount;
            }
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
                        var saveRow = new TblGlChequeTransactionHeader();
                        saveRow.InjectFrom(SelectedMainRow);
                        saveRow.TblGlChequeTransactionDetails = new ObservableCollection<TblGlChequeTransactionDetail>();
                        if (!Loading)
                        {
                            foreach (var tblGlChequeTransactionHeaderViewModel in SelectedMainRow.DetailsList)
                            {
                                var isvalidDetail = Validator.TryValidateObject(tblGlChequeTransactionHeaderViewModel,
                                    new ValidationContext(tblGlChequeTransactionHeaderViewModel, null, null), valiationCollectionDetail, true);
                                if (!isvalidDetail)
                                {
                                    MessageBox.Show("Data is not Valid");
                                    return;
                                }
                                var newdetailrow = new TblGlChequeTransactionDetail();
                                newdetailrow.InjectFrom(tblGlChequeTransactionHeaderViewModel);
                                saveRow.TblGlChequeTransactionDetails.Add(newdetailrow);
                            }
                            var cheques = SelectedMainRow.DetailsList.Select(x => x.TblBankCheque).Distinct();
                            foreach (var variable in cheques)
                            {
                                if (SelectedMainRow.DetailsList.Count(w => w.TblBankCheque == variable.Value) > 1)
                                {
                                    MessageBox.Show("Cheque Exists more than once");
                                    return;
                                }
                            }
                            Loading = true;
                            SelectedMainRow.Approved = approved;
                            Glclient.UpdateOrInsertTblGlChequeTransactionHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial, approved, LoggedUserInfo.DatabasEname);
                        }
                    }
                }
            }
        }

        internal void DeleteMainRow()
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
                            Glclient.DeleteTblGlChequeTransactionHeaderAsync((TblGlChequeTransactionHeader)new TblGlChequeTransactionHeader().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        internal void DeleteDetailRow()
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
                            Glclient.DeleteTblGlChequeTransactionDetailAsync((TblGlChequeTransactionDetail)new TblGlChequeTransactionDetail().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        internal void AddNewDetailRow(bool checkLastRow)
        {
            if (TblGlChequeTypeSetting.ChequeLockupFilterOnChequeStatus != null && TblGlChequeTypeSetting.ChequeLockupFilterOnChequeStatus != 0)
            {
                return;
            }
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

            var newrow = new TblGlChequeTransactionDetailViewModel
            {
                UseEntityDetail1 = TblGlChequeTypeSetting.UseEntityDetail1,
                UseEntityDetail2 = TblGlChequeTypeSetting.UseEntityDetail2,
                EntityDetail1PostDr = TblGlChequeTypeSetting.EntityDetail1PostDr,
                EntityDetail2PostDr = TblGlChequeTypeSetting.EntityDetail2PostDr,
                //EntityDetail1TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail1TblJournalAccountType,
                EntityDetail2TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType,
                TblGlChequeStatus = 4,
                Description = SelectedMainRow.Description
            };
            //if (TblGlChequeTypeSetting.EntityDetail1TblJournalAccountType != null)
            //{
            //    newrow.EntityDetail1TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail1TblJournalAccountType;
            //    newrow.TblJournalAccountTypePerRow = new GenericTable();
            //    newrow.TblJournalAccountTypePerRow.InjectFrom(TblGlChequeTypeSetting.TblJournalAccountType2);
            //}
            if (TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType != null)
            {
                newrow.EntityDetail2TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType;
                newrow.TblJournalAccountType1PerRow = new GenericTable();
                newrow.TblJournalAccountType1PerRow.InjectFrom(TblGlChequeTypeSetting.TblJournalAccountType3);
            }

            if (TblGlChequeTypeSetting.EntityDetail1TblAccount != null && TblGlChequeTypeSetting.EntityDetail1TblAccount != 0)
            {
                newrow.EntityDetail1TblAccount = TblGlChequeTypeSetting.EntityDetail1TblAccount;
            }
            if (TblGlChequeTypeSetting.EntityDetail2TblAccount != null && TblGlChequeTypeSetting.EntityDetail2TblAccount != 0)
            {
                newrow.EntityDetail2TblAccount = TblGlChequeTypeSetting.EntityDetail2TblAccount;
            }
            newrow.Saved = true;
            SelectedMainRow.DetailsList.Add(newrow);
            SelectedDetailRow = newrow;
            //ExportGrid.BeginEdit();
        }

        internal void GenerateDetailListFromExcel(List<TblGlChequeTransactionDetailViewModel> Detail)
        {
            SelectedMainRow.DetailsList.Clear();


            foreach (var item in Detail)
            {


                //var newrow = new TblGlChequeTransactionDetailViewModel
                //{
                item.UseEntityDetail1 = TblGlChequeTypeSetting.UseEntityDetail1;
                   item.UseEntityDetail2 = TblGlChequeTypeSetting.UseEntityDetail2;
                item.EntityDetail1PostDr = TblGlChequeTypeSetting.EntityDetail1PostDr;
                item. EntityDetail2PostDr = TblGlChequeTypeSetting.EntityDetail2PostDr;
                //EntityDetail1TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail1TblJournalAccountType,
                item.EntityDetail2TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType;
                item. TblGlChequeStatus = 4;
            

                if (TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType != null)
                {
                    item.EntityDetail2TblJournalAccountType = TblGlChequeTypeSetting.EntityDetail2TblJournalAccountType;
                    item.TblJournalAccountType1PerRow = new GenericTable();
                    item.TblJournalAccountType1PerRow.InjectFrom(TblGlChequeTypeSetting.TblJournalAccountType3);
                }
            
                if (TblGlChequeTypeSetting.EntityDetail1TblAccount != null && TblGlChequeTypeSetting.EntityDetail1TblAccount != 0)
                {
                    item.EntityDetail1TblAccount = TblGlChequeTypeSetting.EntityDetail1TblAccount;
                }
                if (TblGlChequeTypeSetting.EntityDetail2TblAccount != null && TblGlChequeTypeSetting.EntityDetail2TblAccount != 0)
                {
                    item.EntityDetail2TblAccount = TblGlChequeTypeSetting.EntityDetail2TblAccount;
                }
                item.Saved = true;
                
             

                AddExcelRow(item);
            
            }
            calcTotal();
        }

        public void AddExcelRow(TblGlChequeTransactionDetailViewModel newrow) {
            GlServiceClient GlclientLocal = new GlServiceClient();
            string temp = newrow.ChequeNo;
            var valuesObjects = new Dictionary<string, object>();
            const string filter = "it.Cheque ==(@tt)";
            valuesObjects.Add("tt", Convert.ToInt64(temp));

            GlclientLocal.GetTblBankChequeAsync(0, 1, 0, "It.Iserial", filter, valuesObjects,
             LoggedUserInfo.DatabasEname, ChequeStatusPerRow.Iserial);
            GlclientLocal.GetTblBankChequeCompleted += (s, sv) =>
            {
                newrow.TblBankCheque = sv.Result.FirstOrDefault().Iserial;
                newrow.ChequePerRow = sv.Result.FirstOrDefault();
                if (newrow.EntityDetail1 != 0 && newrow.EntityDetail1 != null && newrow.TblBankCheque != null && newrow.TblBankCheque != 0)
                {
                    SelectedMainRow.DetailsList.Add(newrow);
                }
            };
            var ScopePerRow = new GenericTable();

            bool tempPre = false;

            if (!string.IsNullOrEmpty(newrow.EntityCode))
                GlclientLocal.FindEntityByCodeAsync(newrow.EntityDetail1TblJournalAccountType ?? 0, newrow.EntityCode, ScopePerRow.Iserial, LoggedUserInfo.DatabasEname, tempPre);

            GlclientLocal.FindEntityByCodeCompleted += (s, sv) => {
                newrow.EntityDetail1 = sv.Result.Iserial;
                newrow.EntityPerRow = sv.Result;
                if (newrow.EntityDetail1 != 0 && newrow.EntityDetail1 != null && newrow.TblBankCheque != null && newrow.TblBankCheque != 0)
                {
                    SelectedMainRow.DetailsList.Add(newrow);
                }
            };
        }
        public void GetSubDetailData()
        {
            if (DetailSubSortBy == null)
                DetailSubSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblBankChequeCostCenterAsync(SelectedDetailRow.DetailsList.Count, int.MaxValue, (int)SelectedDetailRow.TblBankCheque,
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
                            Glclient.DeleteTblBankChequeCostCenterAsync(
                                (TblBankChequeCostCenter)new TblBankChequeCostCenter().InjectFrom(row),
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

            if (SelectedDetailRow.Amount == null || SelectedDetailRow.Amount == 0)
            {
                amount = SelectedDetailRow.Amount;
            }
            var newrow = new TblBankChequeCostCenterViewModel
            {
                TblBankCheque = (int)SelectedDetailRow.TblBankCheque,
                EntityPerRow = SelectedDetailRow.EntityPerRow,
                JournalAccountTypePerRow = SelectedDetailRow.TblJournalAccountType1PerRow,
                // CostCenterTypePerRow = CostCenterTypeList.FirstOrDefault(x => x.Iserial == DefaultCostCenterTypeIserial),
                Amount = (double)amount
            };
            newrow.CostCenterTypeEnabled = true;
            if (SelectedSubDetailRow != null && SelectedSubDetailRow.TblCostCenterType != null)
            {
                var value = (int)SelectedSubDetailRow.TblCostCenterType;
                var types = SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == value).Select(x => x.TblCostCenterType).Distinct();

                foreach (var type in types)
                {
                    if (
                        SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == type).Sum(x => x.Amount) !=
                        (double)amount)
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
                    if (SelectedSubDetailRow.TblBankCheque == 0)
                    {
                        SelectedSubDetailRow.TblBankCheque = (int)SelectedDetailRow.TblBankCheque;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblBankChequeCostCenter();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblBankChequeCostCentersAsync(rowToSave, save,
                            SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        #region Prop

        private DateTime? _toDate;

        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private DateTime? _fromDate;

        public DateTime? FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChanged("FromDate"); }
        }

        private Entity _searchPerRow;

        public Entity EntityPerRow
        {
            get { return _searchPerRow ?? (_searchPerRow = new Entity { Iserial = 0 }); }
            set { _searchPerRow = value; RaisePropertyChanged("EntityPerRow"); }
        }

        private Entity _offsetEntityPerRow;

        public Entity OffsetEntityPerRow
        {
            get { return _offsetEntityPerRow ?? (_offsetEntityPerRow = new Entity { Iserial = 0 }); }
            set { _offsetEntityPerRow = value; RaisePropertyChanged("OffsetEntityPerRow"); }
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

        private GenericTable _chequeStatusPerRow;

        public GenericTable ChequeStatusPerRow
        {
            get { return _chequeStatusPerRow; }
            set
            {
                _chequeStatusPerRow = value;
                RaisePropertyChanged("ChequeStatusPerRow");
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


        public GenericTable DefaultBank { get; set; }
        public ObservableCollection<Entity> EntityList { get; set; }

        public TblGlChequeTypeSetting TblGlChequeTypeSetting { get; set; }

        private ObservableCollection<TblCurrencyTest> _currencyList;

        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }
        public ObservableCollection<TblGlPeriodLine> PeriodLines { get; private set; }
        private double _Total;

        public double Total
        {
            get { return _Total; }
            set { _Total = value; RaisePropertyChanged("Total"); }
        }

        private ObservableCollection<TblGlChequeTransactionHeaderViewModel> _mainRowList;

        public ObservableCollection<TblGlChequeTransactionHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGlChequeTransactionHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblGlChequeTransactionHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlChequeTransactionHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblGlChequeTransactionHeaderViewModel _selectedMainRow;

        public TblGlChequeTransactionHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblGlChequeTransactionDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblGlChequeTransactionDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblGlChequeTransactionDetailViewModel>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private TblGlChequeTransactionDetailViewModel _selectedDetailRow;

        public TblGlChequeTransactionDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblBankChequeCostCenterViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblBankChequeCostCenterViewModel> SelectedSubDetailRows
        {
            get { return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblBankChequeCostCenterViewModel>()); }
            set
            {
                _selectedSubDetailRows = value;
                RaisePropertyChanged("SelectedSubDetailRows");
            }
        }

        private TblBankChequeCostCenterViewModel _selectedSubDetailRow;

        public TblBankChequeCostCenterViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set
            {
                _selectedSubDetailRow = value;
                RaisePropertyChanged("SelectedSubDetailRow");
            }
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

        #endregion Prop

        public void GetLockupFromPreTransaction()
        {
            if (TblGlChequeTypeSetting.ChequeLockupFilterOnChequeType != null)
            {
                Glclient.GetLockupFromPreTransactionAsync(TblGlChequeTypeSetting, TblJournalAccountTypePerRow.Iserial, EntityPerRow.Iserial, OffsetEntityPerRow.Iserial, Code, FromDate, ToDate, LoggedUserInfo.DatabasEname);
            }
        }
    }

    #region ViewModels

    public class TblGlChequeTransactionHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        
        private ObservableCollection<TblGlChequeTransactionDetailViewModel> _detailsList;

        public ObservableCollection<TblGlChequeTransactionDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblGlChequeTransactionDetailViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

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

        private bool _entityHeader1PostDrField;

        private int? _entityHeader1TblAccountField;

        private int? _entityHeader1TblJournalAccountTypeField;

        private bool _entityHeader2PostDrField;

        private int? _entityHeader2TblAccountField;

        private int? _entityHeader2TblJournalAccountTypeField;

        private double? _exchangeRateField;

        private int _iserialField;

        private int? _tblCurrencyField;

        private int _tblGlChequeTypeSettingField;

        private TblAccount _tblAccountField;

        private TblAccount _tblAccount1Field;

        private TblCurrencyTest _tblCurrency1Field;

        private GenericTable _tblJournalAccountTypeField;

        private GenericTable _tblJournalAccountType1Field;

        private int? _entityHeader1;

        public int? EntityHeader1
        {
            get
            {
                return _entityHeader1;
            }
            set
            {
                if ((_entityHeader1.Equals(value) != true))
                {
                    _entityHeader1 = value;
                    RaisePropertyChanged("EntityHeader1");
                }
            }
        }

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                _entityPerRow = value; RaisePropertyChanged("EntityPerRow");
                if (EntityPerRow != null) EntityHeader1 = EntityPerRow.Iserial;
            }
        }

        private Entity _offsetEntityPerRow;

        public Entity OffsetEntityPerRow
        {
            get { return _offsetEntityPerRow; }
            set
            {

                _offsetEntityPerRow = value; RaisePropertyChanged("OffsetEntityPerRow");

                if (OffsetEntityPerRow != null) EntityHeader2 = OffsetEntityPerRow.Iserial;
            }
        }

        private int? _entityHeader2;

        public int? EntityHeader2
        {
            get
            {
                return _entityHeader2;
            }
            set
            {
                if ((_entityHeader2.Equals(value) != true))
                {
                    _entityHeader2 = value;
                    RaisePropertyChanged("EntityHeader2");
                }
            }
        }

        private bool _useEntityHeader1;

        public bool UseEntityHeader1
        {
            get { return _useEntityHeader1; }
            set { _useEntityHeader1 = value; RaisePropertyChanged("UseEntityHeader1"); }
        }

        private bool _useEntityHeader2;

        public bool UseEntityHeader2
        {
            get { return _useEntityHeader2; }
            set { _useEntityHeader2 = value; RaisePropertyChanged("UseEntityHeader2"); }
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

        public bool EntityHeader1PostDr
        {
            get
            {
                return _entityHeader1PostDrField;
            }
            set
            {
                if ((_entityHeader1PostDrField.Equals(value) != true))
                {
                    _entityHeader1PostDrField = value;
                    RaisePropertyChanged("EntityHeader1PostDr");
                }
            }
        }

        public int? EntityHeader1TblAccount
        {
            get
            {
                return _entityHeader1TblAccountField;
            }
            set
            {
                if ((_entityHeader1TblAccountField.Equals(value) != true))
                {
                    _entityHeader1TblAccountField = value;
                    RaisePropertyChanged("EntityHeader1TblAccount");
                }
            }
        }

        public int? EntityHeader1TblJournalAccountType
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
                    RaisePropertyChanged("EntityHeader1TblJournalAccountType");
                }
            }
        }

        public bool EntityHeader2PostDr
        {
            get
            {
                return _entityHeader2PostDrField;
            }
            set
            {
                if ((_entityHeader2PostDrField.Equals(value) != true))
                {
                    _entityHeader2PostDrField = value;
                    RaisePropertyChanged("EntityHeader2PostDr");
                }
            }
        }

        public int? EntityHeader2TblAccount
        {
            get
            {
                return _entityHeader2TblAccountField;
            }
            set
            {
                if ((_entityHeader2TblAccountField.Equals(value) != true))
                {
                    _entityHeader2TblAccountField = value;
                    RaisePropertyChanged("EntityHeader2TblAccount");
                }
            }
        }

        public int? EntityHeader2TblJournalAccountType
        {
            get
            {
                return _entityHeader2TblJournalAccountTypeField;
            }
            set
            {
                if ((_entityHeader2TblJournalAccountTypeField.Equals(value) != true))
                {
                    _entityHeader2TblJournalAccountTypeField = value;
                    RaisePropertyChanged("EntityHeader2TblJournalAccountType");
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

        public int TblGlChequeTypeSetting
        {
            get
            {
                return _tblGlChequeTypeSettingField;
            }
            set
            {
                if ((_tblGlChequeTypeSettingField.Equals(value) != true))
                {
                    _tblGlChequeTypeSettingField = value;
                    RaisePropertyChanged("TblGlChequeTypeSetting");
                }
            }
        }

        public TblAccount TblAccountPerRow
        {
            get
            {
                return _tblAccountField;
            }
            set
            {
                if ((ReferenceEquals(_tblAccountField, value) != true))
                {
                    _tblAccountField = value;
                    RaisePropertyChanged("TblAccountPerRow");
                    if (TblAccountPerRow != null) EntityHeader1TblAccount = TblAccountPerRow.Iserial;
                }
            }
        }

        public TblAccount TblAccount1PerRow
        {
            get
            {
                return _tblAccount1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblAccount1Field, value) != true))
                {
                    _tblAccount1Field = value;
                    RaisePropertyChanged("TblAccount1PerRow");
                    if (TblAccount1PerRow != null) EntityHeader2TblAccount = TblAccount1PerRow.Iserial;
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

        private TblGlChequeTypeSetting _tblGlChequeTypeSetting1Field;

        public TblGlChequeTypeSetting TblGlChequeTypeSetting1PerRow
        {
            get
            {
                return _tblGlChequeTypeSetting1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblGlChequeTypeSetting1Field, value) != true))
                {
                    _tblGlChequeTypeSetting1Field = value;
                    RaisePropertyChanged("TblGlChequeTypeSetting1PerRow");

                    if (TblGlChequeTypeSetting1PerRow != null) TblGlChequeTypeSetting = TblGlChequeTypeSetting1PerRow.Iserial;
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
                        EntityHeader1TblJournalAccountType = TblJournalAccountTypePerRow.Iserial;
                }
            }
        }

        public GenericTable TblJournalAccountType1PerRow
        {
            get
            {
                return _tblJournalAccountType1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountType1Field, value) != true))
                {
                    _tblJournalAccountType1Field = value;
                    RaisePropertyChanged("TblJournalAccountType1PerRow");
                    if (TblJournalAccountType1PerRow != null && TblJournalAccountType1PerRow.Iserial != 0)
                        EntityHeader2TblJournalAccountType = TblJournalAccountType1PerRow.Iserial;
                }
            }
        }
    }

    public class TblGlChequeTransactionDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {

        private string _EntityCode;

        public string EntityCode
        {
            get { return _EntityCode; }
            set { _EntityCode = value; RaisePropertyChanged("EntityCode"); }
        }

        private ObservableCollection<TblBankChequeCostCenterViewModel> _detailsList;

        public ObservableCollection<TblBankChequeCostCenterViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblBankChequeCostCenterViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }
        private TblContractHeader _ContractHeaderPerRow;

        public TblContractHeader ContractHeaderPerRow
        {
            get { return _ContractHeaderPerRow; }
            set
            {
                _ContractHeaderPerRow = value;

                RaisePropertyChanged("ContractHeaderPerRow");
                if (_ContractHeaderPerRow != null && _ContractHeaderPerRow.Iserial != 0)
                {
                    TblContractHeader = _ContractHeaderPerRow.Iserial;
                }
            }
        }
        int? _TblContractHeaderField;
        public int? TblContractHeader
        {
            get
            {
                return _TblContractHeaderField;
            }
            set
            {
                if ((_TblContractHeaderField.Equals(value) != true))
                {
                    _TblContractHeaderField = value;
                    RaisePropertyChanged("TblContractHeader");
                }
            }
        }
        private bool _saved;

        public bool Saved
        {
            get { return _saved; }
            set { _saved = value; RaisePropertyChanged("Saved"); }
        }

        private int? _tblGlChequeTransactionDetail;

        public int? TblGlChequeTransactionDetail1
        {
            get
            {
                return _tblGlChequeTransactionDetail;
            }
            set
            {
                if ((_tblGlChequeTransactionDetail.Equals(value) != true))
                {
                    _tblGlChequeTransactionDetail = value;
                    RaisePropertyChanged("TblGlChequeTransactionDetail1");
                }
            }
        }

        private string _payTo;

        public string PayTo
        {
            get { return _payTo; }
            set { _payTo = value; RaisePropertyChanged("PayTo"); }
        }

        private bool _useEntityDetail1;

        public bool UseEntityDetail1
        {
            get { return _useEntityDetail1; }
            set { _useEntityDetail1 = value; RaisePropertyChanged("UseEntityDetail1"); }
        }

        private bool _useEntityDetail2;

        public bool UseEntityDetail2
        {
            get { return _useEntityDetail2; }
            set { _useEntityDetail2 = value; RaisePropertyChanged("UseEntityDetail2"); }
        }

        private double _amountField;

        private DateTime? _bankCollectionDateField;

        private string _bankNameField;

        private string _chequeNoField;

        private string _descriptionField;

        private DateTime? _dueDateField;

        private int? _entityDetail1Field;

        private bool _entityDetail1PostDrField;

        private int? _entityDetail1TblAccountField;

        private int? _entityDetail1TblJournalAccountTypeField;

        private int? _entityDetail2Field;

        private bool _entityDetail2PostDrField;

        private int? _entityDetail2TblAccountField;

        private int? _entityDetail2TblJournalAccountTypeField;

        private int _iserialField;
        private int? _tblBankChequeField;

        private TblBankCheque _tblBankCheque1Field;

        private TblGlChequeStatu _tblGlChequeStatuField;

        private int? _tblGlChequeStatusField;

        private int _tblGlChequeTransactionHeaderField;

        private GenericTable _tblJournalAccountType1Field;

        private GenericTable _tblJournalAccountTypeField;

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                _entityPerRow = value; RaisePropertyChanged("EntityPerRow");
                if (EntityPerRow != null) EntityDetail1 = EntityPerRow.Iserial;
            }
        }

        private Entity _offsetEntityPerRow;

        public Entity OffsetEntityPerRow
        {
            get { return _offsetEntityPerRow; }
            set
            {
                _offsetEntityPerRow = value; RaisePropertyChanged("OffsetEntityPerRow");
                if (OffsetEntityPerRow != null) EntityDetail2 = OffsetEntityPerRow.Iserial;
            }
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

        public DateTime? BankCollectionDate
        {
            get
            {
                return _bankCollectionDateField;
            }
            set
            {
                if ((_bankCollectionDateField.Equals(value) != true))
                {
                    _bankCollectionDateField = value;
                    RaisePropertyChanged("BankCollectionDate");
                }
            }
        }

        public string BankName
        {
            get
            {
                return _bankNameField;
            }
            set
            {
                if ((ReferenceEquals(_bankNameField, value) != true))
                {
                    _bankNameField = value;
                    RaisePropertyChanged("BankName");
                }
            }
        }

        public string ChequeNo
        {
            get
            {
                return _chequeNoField;
            }
            set
            {
                if ((ReferenceEquals(_chequeNoField, value) != true))
                {
                    _chequeNoField = value;
                    RaisePropertyChanged("ChequeNo");
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

        public DateTime? DueDate
        {
            get
            {
                return _dueDateField ?? (DueDate = DateTime.Now);
            }
            set
            {
                if ((_dueDateField.Equals(value) != true))
                {
                    _dueDateField = value;
                    RaisePropertyChanged("DueDate");
                }
            }
        }

        public int? EntityDetail1
        {
            get
            {
                return _entityDetail1Field;
            }
            set
            {
                if ((_entityDetail1Field.Equals(value) != true))
                {
                    _entityDetail1Field = value;
                    RaisePropertyChanged("EntityDetail1");
                }
            }
        }

        public bool EntityDetail1PostDr
        {
            get
            {
                return _entityDetail1PostDrField;
            }
            set
            {
                if ((_entityDetail1PostDrField.Equals(value) != true))
                {
                    _entityDetail1PostDrField = value;
                    RaisePropertyChanged("EntityDetail1PostDr");
                }
            }
        }

        public int? EntityDetail1TblAccount
        {
            get
            {
                return _entityDetail1TblAccountField;
            }
            set
            {
                if ((_entityDetail1TblAccountField.Equals(value) != true))
                {
                    _entityDetail1TblAccountField = value;
                    RaisePropertyChanged("EntityDetail1TblAccount");
                }
            }
        }

        public int? EntityDetail1TblJournalAccountType
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
                    RaisePropertyChanged("EntityDetail1TblJournalAccountType");
                }
            }
        }

        public int? EntityDetail2
        {
            get
            {
                return _entityDetail2Field;
            }
            set
            {
                if ((_entityDetail2Field.Equals(value) != true))
                {
                    _entityDetail2Field = value;
                    RaisePropertyChanged("EntityDetail2");
                }
            }
        }

        public bool EntityDetail2PostDr
        {
            get
            {
                return _entityDetail2PostDrField;
            }
            set
            {
                if ((_entityDetail2PostDrField.Equals(value) != true))
                {
                    _entityDetail2PostDrField = value;
                    RaisePropertyChanged("EntityDetail2PostDr");
                }
            }
        }

        public int? EntityDetail2TblAccount
        {
            get
            {
                return _entityDetail2TblAccountField;
            }
            set
            {
                if ((_entityDetail2TblAccountField.Equals(value) != true))
                {
                    _entityDetail2TblAccountField = value;
                    RaisePropertyChanged("EntityDetail2TblAccount");
                }
            }
        }

        public int? EntityDetail2TblJournalAccountType
        {
            get
            {
                return _entityDetail2TblJournalAccountTypeField;
            }
            set
            {
                if ((_entityDetail2TblJournalAccountTypeField.Equals(value) != true))
                {
                    _entityDetail2TblJournalAccountTypeField = value;
                    RaisePropertyChanged("EntityDetail2TblJournalAccountType");
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

        public int? TblBankCheque
        {
            get
            {
                return _tblBankChequeField;
            }
            set
            {
                if ((_tblBankChequeField.Equals(value) != true))
                {
                    _tblBankChequeField = value;
                    RaisePropertyChanged("TblBankCheque");
                }
            }
        }

        [Required]
        public TblBankCheque ChequePerRow
        {
            get
            {
                return _tblBankCheque1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblBankCheque1Field, value) != true))
                {
                    _tblBankCheque1Field = value;
                    RaisePropertyChanged("ChequePerRow");
                    if (ChequePerRow != null && ChequePerRow.Iserial != 0)
                    {
                        TblBankCheque = ChequePerRow.Iserial;
                        if (ChequePerRow.TblBank1 != null)
                        {
                            BankName = ChequePerRow.TblBank1.Ename;
                            ChequeNo = ChequePerRow.Cheque.ToString(CultureInfo.InvariantCulture);
                        }

                        if (EntityDetail1 == 0 || EntityDetail1 == null)
                        {

                            if (!string.IsNullOrWhiteSpace( ChequePerRow.PayTo))
                            {
                                PayTo = ChequePerRow.PayTo;
                            }

                          
                            if (ChequePerRow.Amount != 0)
                            {
                                Amount = ChequePerRow.Amount;
                            }
                       
                        }
                    }
                }
            }
        }

        public TblGlChequeStatu TblGlChequeStatusPerRow
        {
            get
            {
                return _tblGlChequeStatuField;
            }
            set
            {
                if ((ReferenceEquals(_tblGlChequeStatuField, value) != true))
                {
                    _tblGlChequeStatuField = value;
                    RaisePropertyChanged("TblGlChequeStatusPerRow");
                    if (TblGlChequeStatusPerRow != null) TblGlChequeStatus = TblGlChequeStatusPerRow.Iserial;
                }
            }
        }

        public int? TblGlChequeStatus
        {
            get
            {
                return _tblGlChequeStatusField;
            }
            set
            {
                if ((_tblGlChequeStatusField.Equals(value) != true))
                {
                    _tblGlChequeStatusField = value;
                    RaisePropertyChanged("TblGlChequeStatus");
                }
            }
        }

        public int TblGlChequeTransactionHeader
        {
            get
            {
                return _tblGlChequeTransactionHeaderField;
            }
            set
            {
                if ((_tblGlChequeTransactionHeaderField.Equals(value) != true))
                {
                    _tblGlChequeTransactionHeaderField = value;
                    RaisePropertyChanged("TblGlChequeTransactionHeader");
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
                        EntityDetail1TblJournalAccountType = TblJournalAccountTypePerRow.Iserial;
                }
            }
        }

        public GenericTable TblJournalAccountType1PerRow
        {
            get
            {
                return _tblJournalAccountType1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountType1Field, value) != true))
                {
                    _tblJournalAccountType1Field = value;
                    RaisePropertyChanged("TblJournalAccountType1PerRow");
                    if (TblJournalAccountType1PerRow != null && TblJournalAccountType1PerRow.Iserial != 0)
                        EntityDetail2TblJournalAccountType = TblJournalAccountType1PerRow.Iserial;
                }
            }
        }
    }

    public class TblBankChequeCostCenterViewModel : Web.DataLayer.PropertiesViewModelBase
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

        private GenericTable _journalAccountTypePerRow;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    if (JournalAccountTypePerRow != null)
                        RaisePropertyChanged("JournalAccountTypePerRow");
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

        private int _TblBankCheque;

        public int TblBankCheque
        {
            get { return _TblBankCheque; }
            set { _TblBankCheque = value; RaisePropertyChanged("TblBankCheque"); }
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