using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using System.Globalization;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.ViewModel.Gl
{
    public class LedgerHeaderViewModel : ViewModelBase
    {
       
        public LedgerHeaderViewModel()
        {
            if (!IsDesignTime)
            {

                GetItemPermissions(PermissionItemName.LedgerHeader.ToString());
                GetCustomePermissions(PermissionItemName.LedgerHeader.ToString());
                Glclient = new GlServiceClient();
                Glclient.PostTotalLedger1Completed += (s, sv) =>
                 {
                     PrintLedgerHeader(sv.newTransactionIserial);
                     try
                     {
                         SelectedMainRow.DetailsList.Clear();
                     }
                     catch (Exception)
                     {                        
                     }
                     GetDetailData(false);

                 };
                Glclient.PrintLedgerHeaderCompleted += (s, sv) =>
                {
                    var row = MainRowList.FirstOrDefault(w => w.Iserial == sv.Result.Iserial);
                    if (row != null)
                    {
                        row.balanced = sv.Result.balanced;
                        if (row.balanced == false)
                        {
                            MessageBox.Show("Cannot Print Unbalanced Transaction");
                            return;
                        }

                        if (row.JournalPerRow != null)
                        {
                            if (row.JournalPerRow.Report != null && !string.IsNullOrEmpty(row.JournalPerRow.Report))
                            {
                                var para = new ObservableCollection<string> { row.Iserial.ToString(CultureInfo.InvariantCulture) };
                                para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
                                para.Add(LoggedUserInfo.DatabasEname);
                                var reportViewmodel = new GenericReportViewModel();
                                reportViewmodel.GenerateReport(row.JournalPerRow.Report, para);
                                return;
                            }
                        }
                        var reportChild = new ReportsChildWindow(PermissionItemName.LedgerHeader.ToString(), row.Iserial);
                        reportChild.Show();
                    }
                };

                Glclient.TestingMethodAsync(LoggedUserInfo.DatabasEname);
                Client.GetChainSetupAsync("Ledger");
                Client.GetChainSetupCompleted += (s, sv) =>
                {
                    DefaultCostCenterTypeIserial = Convert.ToInt32(sv.Result.FirstOrDefault(x => x.sGlobalSettingCode == "DefaultCostCenterType").sSetupValue);
                };
                MainRowList = new SortableCollectionView<TblLedgerHeaderViewModel>();
                MainRowListUnPosted = new SortableCollectionView<TblLedgerHeaderViewModel>();
                SelectedMainRow = new TblLedgerHeaderViewModel();
                var glRuleTypeClient = new GlServiceClient();
                glRuleTypeClient.GetGenericCompleted += (s, sv) => { CostCenterTypeList = sv.Result; };
                glRuleTypeClient.GetGenericAsync("TblCostCenterType", "%%", "%%", "%%", "Iserial", "ASC",
                    LoggedUserInfo.DatabasEname);

                Glclient.GetTblRetailCurrencyAsync(0, int.MaxValue, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);
                Glclient.GetTblRetailCurrencyCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };

                Glclient.ImportLedgerMainDetailsCompleted += (s, sv) => MessageBox.Show(sv.Error == null ? "Completed" : sv.Error.Message);

                Glclient.GetRetailChainSettingAsync(LoggedUserInfo.DatabasEname);

                Glclient.GetRetailChainSettingCompleted += (s, sv) =>
                {
                    DefaultCurrencyIserial = sv.Result.CurrCurrency;
                };
                //var journalAccountTypeClient = new GlServiceClient();
                //journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                //{
                //    JournalAccountTypeList = sv.Result;
                //};
                //journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetJournalAccountTypeSettingAsync(LoggedUserInfo.Iserial,LoggedUserInfo.DatabasEname);
                Glclient.GetJournalAccountTypeSettingCompleted += (s, sv) => {
                    JournalAccountTypeList = new ObservableCollection<GenericTable>();
                    foreach (var item in sv.Result)
                    {
                        JournalAccountTypeList.Add(new GenericTable().InjectFrom(item) as GenericTable);
                    }
                };
                
                var bankTransactionTypeClient = new GlServiceClient();
                bankTransactionTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    BankTransactionTypeList = sv.Result;
                };
                Glclient.GetTblMethodOfPaymentAsync(0, int.MaxValue, "It.Iserial", null, null, LoggedUserInfo.DatabasEname);
                Glclient.GetTblMethodOfPaymentCompleted += (s, sv) =>
                {
                    MethodOfPaymentList = sv.Result;
                };
                bankTransactionTypeClient.GetGenericAsync("TblBankTransactionType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetTblLedgerHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerHeaderViewModel();
                        newrow.InjectFrom(row);
                        newrow.JournalPerRow = row.TblJournal1;

                        MainRowList.Add(newrow);
                    }

                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false,true);
                    }
                    Loading = false;
                };

                Glclient.GetTblLedgerHeader1Completed += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerHeaderViewModel();
                        newrow.InjectFrom(row);
                        newrow.JournalPerRow = row.TblJournal1;

                        MainRowListUnPosted.Add(newrow);
                    }

                    FullCount = sv.fullCount;
                    if (MainRowListUnPosted.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowListUnPosted.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowListUnPosted.Count == 0)
                    {
                        AddNewMainRow(false,false);
                    }
                    Loading = false;
                };


                Glclient.UpdateOrInsertTblLedgerHeadersCompleted += (s, ev) =>
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
                    Loading = false;
                };


                Glclient.UpdateOrInsertTblLedgerHeader1sCompleted += (s, ev) =>
                {

                    Loading = false;
                    if (ev.Error != null)
                    {
                      
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        if (ev.newTransactionIserial!=0)
                        {
                            PrintLedgerHeader(ev.newTransactionIserial);
                        }
            
                        MainRowListUnPosted.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                   
                };

                Glclient.DeleteTblLedgerHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false,true);
                    }
                    Loading = false;
                };

                Glclient.DeleteTblLedgerHeader1Completed += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowListUnPosted.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowListUnPosted.Remove(oldrow);
                    if (!MainRowListUnPosted.Any())
                    {
                        AddNewMainRow(false, false);
                    }
                    Loading = false;
                };
                Glclient.GetTblLedgerMainDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerMainDetailViewModel { CurrencyPerRow = new TblCurrencyTest() };
                        newrow.NotLoaded = true;
                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        if (row.TblBankTransactionType1 != null)
                        {
                            newrow.BankTransactionTypePerRow = new GenericTable();
                            newrow.BankTransactionTypePerRow.InjectFrom(row.TblBankTransactionType1);
                        }
                        newrow.MethodOfPaymentPerRow = row.TblMethodOfPayment1;
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        newrow.OffsetAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        if (row.TblJournalAccountType2 != null)
                            newrow.OffsetAccountTypePerRow.InjectFrom(row.TblJournalAccountType2);
                        newrow.EntityPerRow =
                            sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                     x.Iserial == row.EntityAccount);
                        newrow.OffsetEntityPerRow =
                           sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.OffsetAccountType && x.Iserial == row.OffsetEntityAccount);
                        newrow.InjectFrom(row);
                        if (newrow.DrOrCr == true)
                        {
                            if (row.Amount != null) newrow.DrAmount = row.Amount;
                        }
                        else
                        {
                            if (row.Amount != null) newrow.CrAmount = row.Amount;
                        }
                        if (row.TblBankCheque1 != null) newrow.ChequePerRow = row.TblBankCheque1;
                        newrow.TransactionExists = sv.TransactionExist.FirstOrDefault(x => x.Key == newrow.Iserial).Value;

                        if (row.TblAccount != null)
                        {
                            newrow.AccountPerRow = new TblAccount
                            {
                                Code = row.TblAccount.Code,
                                Iserial = row.TblAccount.Iserial,
                                Ename = row.TblAccount.Ename,
                                Aname = row.TblAccount.Aname
                            };
                        }
                        newrow.NotLoaded = false;
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    AddMode = false;

                    //if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count(x => x.Iserial >= 0) == 0)
                    //{
                    //    AddMode = true;
                    //    AddNewDetailRow(false,true);
                    //}
                    if (SelectedMainRow.balanced == false || SelectedMainRow.DetailsList.Sum(w => w.CrAmount) == 0
                  || SelectedMainRow.DetailsList.Sum(w => w.DrAmount) == 0)
                    {
                        AddMode = true;
                    }
                    if (Export)
                    {
                        AllowExport = true;
                        Export = false;
                    }
                };

                Glclient.GetTblLedgerMainDetail1Completed += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerMainDetailViewModel { CurrencyPerRow = new TblCurrencyTest() };
                        newrow.NotLoaded = true;
                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        if (row.TblBankTransactionType1 != null)
                        {
                            newrow.BankTransactionTypePerRow = new GenericTable();
                            newrow.BankTransactionTypePerRow.InjectFrom(row.TblBankTransactionType1);
                        }
                        newrow.MethodOfPaymentPerRow = row.TblMethodOfPayment1;
                        newrow.JournalAccountTypePerRow = new GenericTable();
                        newrow.OffsetAccountTypePerRow = new GenericTable();
                        if (row.TblJournalAccountType1 != null)
                            newrow.JournalAccountTypePerRow.InjectFrom(row.TblJournalAccountType1);
                        if (row.TblJournalAccountType2 != null)
                            newrow.OffsetAccountTypePerRow.InjectFrom(row.TblJournalAccountType2);
                        newrow.EntityPerRow =
                            sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                     x.Iserial == row.EntityAccount);
                        newrow.OffsetEntityPerRow =
                           sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.OffsetAccountType && x.Iserial == row.OffsetEntityAccount);
                        newrow.InjectFrom(row);
                        if (newrow.DrOrCr == true)
                        {
                            if (row.Amount != null) newrow.DrAmount = row.Amount;
                        }
                        else
                        {
                            if (row.Amount != null) newrow.CrAmount = row.Amount;
                        }
                        if (row.TblBankCheque1 != null) newrow.ChequePerRow = row.TblBankCheque1;
                        newrow.TransactionExists = sv.TransactionExist.FirstOrDefault(x => x.Key == newrow.Iserial).Value;

                        if (row.TblAccount != null)
                        {
                            newrow.AccountPerRow = new TblAccount
                            {
                                Code = row.TblAccount.Code,
                                Iserial = row.TblAccount.Iserial,
                                Ename = row.TblAccount.Ename,
                                Aname = row.TblAccount.Aname
                            };
                        }
                        newrow.NotLoaded = false;
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    AddMode = false;

                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count(x => x.Iserial >= 0) == 0)
                    {
                        AddMode = true;
                        AddNewDetailRow(false, false);
                    }
                    if (SelectedMainRow.balanced == false || SelectedMainRow.DetailsList.Sum(w => w.CrAmount) == 0
                  || SelectedMainRow.DetailsList.Sum(w => w.DrAmount) == 0)
                    {
                        AddMode = true;
                    }
                    if (Export)
                    {
                        AllowExport = true;
                        Export = false;
                    }
                };



                Glclient.UpdateOrInsertTblLedgerMainDetailsCompleted += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblLedgerMainDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    SaveMainRow(true);
                };
                Glclient.UpdateOrInsertTblLedgerMainDetail1Completed += (s, x) =>
                {
                    Loading = false;
                    var savedRow = (TblLedgerMainDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    savedRow.TblLedgerHeader = x.Result.TblLedgerHeader1;
                    SaveMainRow(false);
                };

                Glclient.DeleteTblLedgerMainDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    Loading = false;
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    //if (!SelectedMainRow.DetailsList.Any())
                    //{
                    //    AddNewDetailRow(false,true);
                    //}
                };

                Glclient.DeleteTblLedgerMainDetail1Completed += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    Loading = false;
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false,false);
                    }
                };



                Glclient.GetTblPeriodLineAsync(0, int.MaxValue, 0
                    , "It.Iserial", null, null, LoggedUserInfo.DatabasEname);

                Glclient.GetTblPeriodLineCompleted += (s, sv) =>
                {
                    PeriodLines = sv.Result;
                };
                Glclient.GetTblLedgerDetailCostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerMainDetailCostCenterViewModel
                        {
                            CostCenterPerRow = row.TblCostCenter1,
                            CostCenterTypePerRow = new GenericTable(),
                            EntityPerRow = SelectedDetailRow.EntityPerRow,
                            JournalAccountTypePerRow = SelectedDetailRow.JournalAccountTypePerRow
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
                    if (SelectedSubDetailRow!=null)
                    {
                        ExportGrid.ScrollIntoView(SelectedSubDetailRow, ExportGrid.Columns[1]);
                        ExportGrid.CurrentColumn = ExportGrid.Columns[1];
                        ExportGrid.Focus();
                    }
                 
                };
                Glclient.GetTblLedgerDetail1CostCenterCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLedgerMainDetailCostCenterViewModel
                        {
                            CostCenterPerRow = row.TblCostCenter1,
                            CostCenterTypePerRow = new GenericTable(),
                            EntityPerRow = SelectedDetailRow.EntityPerRow,
                            JournalAccountTypePerRow = SelectedDetailRow.JournalAccountTypePerRow
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
                    if (SelectedSubDetailRow != null)
                    {
                        ExportGrid.ScrollIntoView(SelectedSubDetailRow, ExportGrid.Columns[1]);
                        ExportGrid.CurrentColumn = ExportGrid.Columns[1];
                        ExportGrid.Focus();
                    }

                };
                
                Glclient.UpdateOrInsertTblLedgerDetailCostCentersCompleted += (s, x) =>
                {
                    Loading = false;
                    SelectedDetailRow.TransactionExists = true;
                    var savedRow = SelectedDetailRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                };
                Glclient.UpdateOrInsertTblLedgerDetail1CostCentersCompleted += (s, x) =>
                {
                    Loading = false;
                    SelectedDetailRow.TransactionExists = true;
                    var savedRow = SelectedDetailRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                };

                
                         Glclient.DeleteTblLedgerDetail1CostCenterCompleted += (s, ev) =>
                         {
                             if (ev.Error != null)
                             {
                                 MessageBox.Show(ev.Error.Message);
                             }
                             if (SelectedDetailRow.DetailsList.Any())
                             {
                                 SelectedDetailRow.TransactionExists = false;
                             }

                             Loading = false;
                             var oldrow = SelectedDetailRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                             if (oldrow != null) SelectedDetailRow.DetailsList.Remove(oldrow);
                             if (!SelectedDetailRow.DetailsList.Any())
                             {
                                 AddNewSubDetailRow(false);
                             }
                         };

                Glclient.DeleteTblLedgerDetailCostCenterCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    if (SelectedDetailRow.DetailsList.Any())
                    {
                        SelectedDetailRow.TransactionExists = false;
                    }

                    Loading = false;
                    var oldrow = SelectedDetailRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.DetailsList.Remove(oldrow);
                    if (!SelectedDetailRow.DetailsList.Any())
                    {
                        AddNewSubDetailRow(false);
                    }
                };

                GetMaindata(true);

                GetMaindata(false);
            }
        }

        public void Reverse()
        {
            Glclient.ReverseLedgerTransactionsAsync( SelectedMainRow.TblJournalLink.Value,
                                                     SelectedMainRow.TblTransactionType.Value,
                                                     "", "", "", LoggedUserInfo.WFM_UserID.Value.ToString(),
                                                     LoggedUserInfo.DatabasEname);

            Glclient.ReverseLedgerTransactionsCompleted += (s, x) =>
            {

            };

           
        }

        public int DefaultCostCenterTypeIserial { get; set; }

        public int? DefaultCurrencyIserial { get; set; }

        public void PrintLedgerHeader(int iserial)
        {

            Glclient.PrintLedgerHeaderAsync(iserial,
                LoggedUserInfo.DatabasEname);
        }


        public void PosTLedgerTotal()
        {
            Glclient.PostTotalLedger1Async(SelectedMainRow.Iserial, LoggedUserInfo.Iserial,
             LoggedUserInfo.DatabasEname);
        }
        public void GetMaindata(bool Posted)
        {
            if (SortBy == null)
                SortBy = "it.DocDate desc";
            Loading = true;
            if (Posted)
            {
                Glclient.GetTblLedgerHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial,
               LoggedUserInfo.DatabasEname);
            }
            else
            {
                Glclient.GetTblLedgerHeader1Async(MainRowListUnPosted.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial,
                      LoggedUserInfo.DatabasEname);
            }
           
        }

        public void AddNewMainRow(bool checkLastRow,bool posted )
        {
            if (posted)
            {
                MessageBox.Show("Cannot Add Posted Ledger");
                return;
         
            }
            else {
                var currentRowIndex = (MainRowListUnPosted.IndexOf(SelectedMainRow));

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
                if (AllowAdd != true)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }
                var newrow = new TblLedgerHeaderViewModel { Approved = false, Posted = false };
                MainRowListUnPosted.Insert(currentRowIndex + 1, newrow);
                SelectedMainRow = newrow;
            }
            ExportGrid.BeginEdit();
        }

        public void SaveMainRow(bool posted)
        {
            if (posted)
            {
                //if (SelectedMainRow != null)
                //{
                //    var valiationCollection = new List<ValidationResult>();

                //    var isvalid = Validator.TryValidateObject(SelectedMainRow,
                //        new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                //    if (isvalid)
                //    {
                //        var save = SelectedMainRow.Iserial == 0;
                //        //if (AllowUpdate != true && !save)
                //        //{

                //        //    return;
                //        //}
                //        var saveRow = new TblLedgerHeader();
                //        saveRow.InjectFrom(SelectedMainRow);
                //        if (!Loading)
                //        {
                //            Loading = true;

                //            Glclient.UpdateOrInsertTblLedgerHeadersAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial,
                //                LoggedUserInfo.DatabasEname, true);
                //        }
                //    }
                //}
            }
            else {
                if (SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow,
                        new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = SelectedMainRow.Iserial == 0;
                        //if (AllowUpdate != true && !save)
                        //{

                        //    return;
                        //}
                        var saveRow = new TblLedgerHeader1();
                        saveRow.InjectFrom(SelectedMainRow);
                        if (!Loading)
                        {
                            Loading = true;

                            Glclient.UpdateOrInsertTblLedgerHeader1sAsync(saveRow, save, MainRowListUnPosted.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial,
                                LoggedUserInfo.DatabasEname, true);
                        }
                    }
                }
            }
        }

        public void SaveOldRow(TblLedgerHeaderViewModel oldRow)
        {

            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        return;
                    }
                    var saveRow = new TblLedgerHeader1();
                    saveRow.InjectFrom(oldRow);
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblLedgerHeader1sAsync(saveRow, save, MainRowListUnPosted.IndexOf(oldRow), LoggedUserInfo.Iserial,
                            LoggedUserInfo.DatabasEname, true);
                    }
                }
            }

            //if (oldRow != null)
            //{
            //    var valiationCollection = new List<ValidationResult>();

            //    var isvalid = Validator.TryValidateObject(oldRow,
            //        new ValidationContext(oldRow, null, null), valiationCollection, true);

            //    if (isvalid)
            //    {
            //        var save = oldRow.Iserial == 0;
            //        if (AllowUpdate != true && !save)
            //        {
            //            return;
            //        }
            //        var saveRow = new TblLedgerHeader();
            //        saveRow.InjectFrom(oldRow);
            //        if (!Loading)
            //        {
            //            Loading = true;

            //            Glclient.UpdateOrInsertTblLedgerHeadersAsync(saveRow, save, MainRowList.IndexOf(oldRow), LoggedUserInfo.Iserial,
            //                LoggedUserInfo.DatabasEname, true);
            //        }
            //    }
            //}
        }

        public void DeleteMainRow(bool Posted)
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

                            if (Posted)
                            {
                     //           Glclient.DeleteTblLedgerHeaderAsync((TblLedgerHeader)new TblLedgerHeader().InjectFrom(row),
                     //MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
                            }
                            else
                            {
                                Glclient.DeleteTblLedgerHeader1Async((TblLedgerHeader1)new TblLedgerHeader1().InjectFrom(row),
                                                  MainRowListUnPosted.IndexOf(row), LoggedUserInfo.DatabasEname);
                            }
                 
                        }
                        else
                        {
                            if (Posted)
                            {
                                MainRowList.Remove(row);
                                if (!MainRowList.Any())
                                {
                                    AddNewMainRow(false, true);
                                }
                            }
                            else
                            {
                                MainRowListUnPosted.Remove(row);
                                if (!MainRowListUnPosted.Any())
                                {
                                    AddNewMainRow(false, false);
                                }

                            }
                         
                        }
                    }
                }
            }
        }

        public void GetDetailData( bool Posted)
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            if (Posted)
            {

            
            if (SelectedMainRow != null)
                Glclient.GetTblLedgerMainDetailAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
        }
            else
            {
                if (SelectedMainRow != null)
                    Glclient.GetTblLedgerMainDetail1Async(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                        DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
            }
        }

        public void GetFullDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            Export = true;
            if (SelectedMainRow != null)
            {

                if (PostedMode)
                {


                    if (SelectedMainRow != null)
                        Glclient.GetTblLedgerMainDetailAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                            DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
                }
                else
                {
                    if (SelectedMainRow != null)
                        Glclient.GetTblLedgerMainDetail1Async(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial,
                            DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
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
                            if (AllowDelete != true && AddMode == false)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteTblLedgerMainDetail1Async(
                                (TblLedgerMainDetail1)new TblLedgerMainDetail1().InjectFrom(row),
                                SelectedMainRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                            if (!SelectedMainRow.DetailsList.Any(x => x.Iserial >= 0))
                            {
                                AddNewDetailRow(false,true);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow,bool posted)
        {

            if (posted)
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
                if (AllowUpdate != true && AddMode == false)
                {

                    return;
                }

                var currency = CurrencyList.FirstOrDefault(x => x.Iserial == DefaultCurrencyIserial);
                if (SelectedMainRow.JournalPerRow.TblCurrency != null)
                {
                    if (SelectedMainRow.JournalPerRow.TblCurrency1 != null)
                        currency = SelectedMainRow.JournalPerRow.TblCurrency1;
                }
                var journalaccType = JournalAccountTypeList.FirstOrDefault(x => SelectedMainRow.EntityPerRow != null && x.Iserial == SelectedMainRow.EntityPerRow.TblJournalAccountType);

                var newrow = new TblLedgerMainDetailViewModel
                {
                    CurrencyPerRow = currency,
                    TransDate = SelectedMainRow.DocDate,
                    TblLedgerHeader = SelectedMainRow.Iserial,
                    Description = SelectedMainRow.Description,
                    JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(),
                    ExchangeRateEnabled = false,
                };

                if (SelectedMainRow.DetailsList.Any(x => x.Iserial > -1))
                {
                    SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
                }
                else
                {
                    if (currentRowIndex == -1)
                    {
                        currentRowIndex = 0;
                    }
                    SelectedMainRow.DetailsList.Insert(currentRowIndex, newrow);
                }

                SelectedDetailRow = newrow;
            }
            else {

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
                if (AllowUpdate != true && AddMode == false)
                {

                    return;
                }

                var currency = CurrencyList.FirstOrDefault(x => x.Iserial == DefaultCurrencyIserial);
                if (SelectedMainRow.JournalPerRow.TblCurrency != null)
                {
                    if (SelectedMainRow.JournalPerRow.TblCurrency1 != null)
                        currency = SelectedMainRow.JournalPerRow.TblCurrency1;
                }
                var journalaccType = JournalAccountTypeList.FirstOrDefault(x => SelectedMainRow.EntityPerRow != null && x.Iserial == SelectedMainRow.EntityPerRow.TblJournalAccountType);

                var newrow = new TblLedgerMainDetailViewModel
                {
                    CurrencyPerRow = currency,
                    TransDate = SelectedMainRow.DocDate,
                    TblLedgerHeader = SelectedMainRow.Iserial,
                    Description = SelectedMainRow.Description,
                    JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(),
                    ExchangeRateEnabled = false,
                };

                if (SelectedMainRow.DetailsList.Any(x => x.Iserial > -1))
                {
                    SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
                }
                else
                {
                    if (currentRowIndex == -1)
                    {
                        currentRowIndex = 0;
                    }
                    SelectedMainRow.DetailsList.Insert(currentRowIndex, newrow);
                }

                SelectedDetailRow = newrow;
            }
            ExportGrid.BeginEdit();
        }


        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    if (SelectedDetailRow.TblLedgerHeader == 0)
                    {
                        SelectedDetailRow.TblLedgerHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && AddMode == false)
                    {

                        return;
                    }
                    var rowToSave = new TblLedgerMainDetail1();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    rowToSave.TblLedgerHeader1 = SelectedMainRow.Iserial;
                    rowToSave.Amount = rowToSave.DrOrCr ? SelectedDetailRow.DrAmount : SelectedDetailRow.CrAmount;
                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblLedgerMainDetail1Async(rowToSave, save,
                            SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname,
                            LoggedUserInfo.Iserial, true);
                    }
                }
            }
        }

        public void SaveOldDetailRow(TblLedgerMainDetailViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (oldRow.TblLedgerHeader == 0)
                    {
                        oldRow.TblLedgerHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && AddMode == false)
                    {

                        return;
                    }
                    var rowToSave = new TblLedgerMainDetail1();
                    rowToSave.InjectFrom(oldRow);
                    rowToSave.TblLedgerHeader1 = SelectedMainRow.Iserial;

                    rowToSave.Amount = rowToSave.DrOrCr ? oldRow.DrAmount : oldRow.CrAmount;
                    if (!Loading)
                    {
                        if (SelectedMainRow != null)
                        {
                            if (SelectedMainRow.DetailsList.IndexOf(oldRow) != -1)
                            {
                                Loading = true;
                                Glclient.UpdateOrInsertTblLedgerMainDetail1Async(rowToSave, save,
                                SelectedMainRow.DetailsList.IndexOf(oldRow), LoggedUserInfo.DatabasEname,
                                LoggedUserInfo.Iserial, true);
                            }
                        }

                    }
                }
            }
        }

        public void GetSubDetailData()
        {
            if (DetailSubSortBy == null)
                DetailSubSortBy = "it.Iserial";
            Loading = true;

            if (PostedMode)
            {


                if (SelectedMainRow != null)
                    Glclient.GetTblLedgerDetailCostCenterAsync(SelectedDetailRow.DetailsList.Count, int.MaxValue, SelectedDetailRow.Iserial,
                  DetailSubSortBy, DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
            }
            else
            {
                if (SelectedMainRow != null)
                    Glclient.GetTblLedgerDetail1CostCenterAsync(SelectedDetailRow.DetailsList.Count, int.MaxValue, SelectedDetailRow.Iserial,
                        DetailSubSortBy, DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
            }

       
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
                            if (AllowDelete != true && AddMode == false)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;


                            if (PostedMode)
                            {
                                Glclient.DeleteTblLedgerDetailCostCenterAsync(
                              (TblLedgerDetailCostCenter)new TblLedgerDetailCostCenter().InjectFrom(row),
                              SelectedDetailRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                            }
                            else
                            {
                                Glclient.DeleteTblLedgerDetail1CostCenterAsync(
                          (TblLedgerDetail1CostCenter)new TblLedgerDetail1CostCenter().InjectFrom(row),
                          SelectedDetailRow.DetailsList.IndexOf(row), LoggedUserInfo.DatabasEname);
                            

                            }
                      
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


            if (SelectedMainRow.balanced == false || SelectedMainRow.DetailsList.Sum(w => w.CrAmount) == 0
                     || SelectedMainRow.DetailsList.Sum(w => w.DrAmount) == 0
                     )
            {

            }
            else
            {
                if (AllowUpdate != true && AddMode == false)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }

            }

            var amount = SelectedDetailRow.CrAmount;

            if (SelectedDetailRow.CrAmount == null || SelectedDetailRow.CrAmount == 0)
            {
                amount = SelectedDetailRow.DrAmount;
            }
            if (amount==null)
            {
                amount = 0;
            }
            var newrow = new TblLedgerMainDetailCostCenterViewModel
            {
                TblLedgerMainDetail = SelectedDetailRow.Iserial,
                EntityPerRow = SelectedDetailRow.EntityPerRow,
                JournalAccountTypePerRow = SelectedDetailRow.JournalAccountTypePerRow,
                CostCenterTypePerRow = CostCenterTypeList.FirstOrDefault(x => x.Iserial == DefaultCostCenterTypeIserial),
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
            ExportGrid.BeginEdit();
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
                    //if (SelectedDetailRow.DetailsList.Where(x => x.TblCostCenterType == SelectedSubDetailRow.TblCostCenterType).Sum(x => x.Amount) > SelectedDetailRow.dr)
                    //{
                    //    MessageBox.Show("Cost Center Amount Cannot Be More Than The Transaction Amount");
                    //    return;
                    //}
                    var save = SelectedSubDetailRow.Iserial == 0;
                    if (SelectedSubDetailRow.TblLedgerMainDetail == 0)
                    {
                        SelectedSubDetailRow.TblLedgerMainDetail = SelectedDetailRow.Iserial;
                    }

                    if (SelectedMainRow.balanced == false || SelectedMainRow.DetailsList.Sum(w => w.CrAmount) == 0
                    || SelectedMainRow.DetailsList.Sum(w => w.DrAmount) == 0)
                    {

                    }
                    else
                    {
                        if (AllowUpdate != true && AddMode == false)
                        {

                            return;
                        }

                    }


                    var rowToSave = new TblLedgerDetailCostCenter();
                    rowToSave.InjectFrom(SelectedSubDetailRow);
                    if (!Loading)
                    {
                        Loading = true;

                        if (PostedMode)
                        {
                            Glclient.UpdateOrInsertTblLedgerDetailCostCentersAsync(rowToSave, save,
                                                      SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, true);
                        }
                        else
                        {
                            var rowToSaveNew = new TblLedgerDetail1CostCenter();
                            rowToSaveNew.InjectFrom(SelectedSubDetailRow);
                            rowToSaveNew.TblLedgerMainDetail1 = SelectedDetailRow.Iserial;
                            Glclient.UpdateOrInsertTblLedgerDetail1CostCentersAsync(rowToSaveNew, save,
                                               SelectedDetailRow.DetailsList.IndexOf(SelectedSubDetailRow), LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, true);

                        }
                      
                    }
                }
            }
        }

        #region Prop

        private ObservableCollection<TblCurrencyTest> _currencyList;

        public ObservableCollection<TblCurrencyTest> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
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

        private ObservableCollection<GenericTable> _journalAccountTypeList;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<GenericTable> _bankTransactionTypeList;

        public ObservableCollection<GenericTable> BankTransactionTypeList
        {
            get { return _bankTransactionTypeList; }
            set { _bankTransactionTypeList = value; RaisePropertyChanged("BankTransactionTypeList"); }
        }

        private ObservableCollection<TblMethodOfPayment> _methodOfPaymentList;

        public ObservableCollection<TblMethodOfPayment> MethodOfPaymentList
        {
            get { return _methodOfPaymentList; }
            set { _methodOfPaymentList = value; RaisePropertyChanged("MethodOfPaymentList"); }
        }

        private SortableCollectionView<TblLedgerHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblLedgerHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private SortableCollectionView<TblLedgerHeaderViewModel> _MainRowListUnPosted;

        public SortableCollectionView<TblLedgerHeaderViewModel> MainRowListUnPosted
        {
            get { return _MainRowListUnPosted; }
            set
            {
                _MainRowListUnPosted = value;
                RaisePropertyChanged("MainRowListUnPosted");
            }
        }




        private ObservableCollection<TblLedgerHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblLedgerHeaderViewModel> SelectedMainRows
        {
            get
            {
                return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblLedgerHeaderViewModel>());
            }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }


    

        private TblLedgerHeaderViewModel _selectedMainRow;

        public TblLedgerHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private bool     _PostedMode;

        public bool PostedMode
        {
            get { return _PostedMode; }
            set
            {
                _PostedMode = value;
                RaisePropertyChanged("PostedMode");
            }
        }


  

        private TblLedgerMainDetailViewModel _selectedDetailRow;

        public TblLedgerMainDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblLedgerMainDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblLedgerMainDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblLedgerMainDetailViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private ObservableCollection<TblLedgerMainDetailCostCenterViewModel> _selectedSubDetailRows;

        public ObservableCollection<TblLedgerMainDetailCostCenterViewModel> SelectedSubDetailRows
        {
            get { return _selectedSubDetailRows ?? (_selectedSubDetailRows = new ObservableCollection<TblLedgerMainDetailCostCenterViewModel>()); }
            set
            {
                _selectedSubDetailRows = value;
                RaisePropertyChanged("SelectedSubDetailRows");
            }
        }

        private TblLedgerMainDetailCostCenterViewModel _selectedSubDetailRow;

        public TblLedgerMainDetailCostCenterViewModel SelectedSubDetailRow
        {
            get { return _selectedSubDetailRow; }
            set
            {
                _selectedSubDetailRow = value;
                RaisePropertyChanged("SelectedSubDetailRow");
            }
        }
        private bool _AddMode;

        public bool AddMode
        {
            get { return _AddMode; }
            set
            {
                _AddMode = value;
                RaisePropertyChanged("AddMode");
            }
        }

        public ObservableCollection<TblGlPeriodLine> PeriodLines { get; private set; }


        #endregion Prop

        public void Post()
        {
            if (Loading)
            {
                return;
            }
            if (!SelectedMainRow.balanced)
            {
                SelectedMainRow.Posted = false;
                MessageBox.Show("Cannot Post Un balanced Journal");
                return;
            }
            if (SelectedMainRow.Posted)
            {
                if (CustomePermissions.SingleOrDefault(x => x.Code == "LedgerUnpost") != null)
                {
                    SelectedMainRow.Posted = false;
                    
                }
            }
            else
            {
                if (SelectedMainRow.Approved)
                {
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithApproval") != null)
                    {
                        SelectedMainRow.Posted = true;
                        
                    }
                }
                else
                {
                    
                    if (CustomePermissions.SingleOrDefault(x => x.Code == "LedgerPostWithoutApproval") != null)
                    {
                        SelectedMainRow.Posted = true;
                        
                        
                    }
                }
            }


            if (SelectedMainRow.Posted)
            {
                SaveMainRow(false);
                return;
            }
            MessageBox.Show("Permission Required");
        }

        public void Approve()
        {
            if (!SelectedMainRow.balanced)
            {
                SelectedMainRow.Approved = false;
                MessageBox.Show("Cannot UnApprove Un balanced Journal");
            }
        }

        public void InsertImportedLedgerMainDetail(ObservableCollection<TblLedgerMainDetail1> ledgerMainList)
        {
            if (AllowUpdate != true && AddMode == false)
            {
                MessageBox.Show(strings.AllowUpdateMsg);
                return;
            }
            Glclient.ImportLedgerMainDetailsAsync(ledgerMainList, LoggedUserInfo.DatabasEname);
        }
    }

    #region ViewModels

    public class TblLedgerHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _sequence;

        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; RaisePropertyChanged("Sequence"); }
        }

        private int _tblSequence;

        public int TblSequence
        {
            get { return _tblSequence; }
            set { _tblSequence = value; RaisePropertyChanged("TblSequence"); }
        }

        private int? _tblTransactionType;

        public int? TblTransactionType
        {
            get { return _tblTransactionType; }
            set { _tblTransactionType = value; }
        }

        private int? _tblJournalLink;

        public int? TblJournalLink
        {
            get { return _tblJournalLink; }
            set { _tblJournalLink = value; RaisePropertyChanged("TblJournalLink"); }
        }

        private GlServiceClient client = new GlServiceClient();

        public TblLedgerHeaderViewModel()
        {
            client.FindEntityCompleted += (s, sv) =>
            {
                EntityPerRow = sv.Result;
            };
        }

        private Entity _entityPerRow;

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set { _entityPerRow = value; RaisePropertyChanged("EntityPerRow"); }
        }

        private DateTime? _approveDate;
        private bool _approvedField;
        private string _codeField;
        private string _descriptionField;

        private DateTime? _docDate;
        private int _iserialField;
        private TblJournal _journalPerRow;
        private DateTime? _postDate;
        private bool _postedField;
        private int? _tblJournal;
        private SortableCollectionView<TblLedgerMainDetailViewModel> _detailsList;
        public SortableCollectionView<TblLedgerMainDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new SortableCollectionView<TblLedgerMainDetailViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournal")]
        public int? TblJournal
        {
            get { return _tblJournal; }
            set
            {
                _tblJournal = value;
                RaisePropertyChanged("TblJournal");
            }
        }

        [ReadOnly(true)]
        public string Code
        {
            get { return _codeField; }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public string Description
        {
            get { return _descriptionField; }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private Visibility _visPosted;

        public Visibility VisPosted
        {
            get { return _visPosted; }
            set { _visPosted = value; RaisePropertyChanged("VisPosted"); }
        }

        private Visibility _visApproved;

        public Visibility VisApproved
        {
            get { return _visApproved; }
            set { _visApproved = value; RaisePropertyChanged("VisApproved"); }
        }

        private bool _balanced;

        public bool balanced
        {
            get { return _balanced; }
            set { _balanced = value; RaisePropertyChanged("balanced"); }
        }

        public bool Posted
        {
            get { return _postedField; }
            set
            {
                _postedField = value;
                RaisePropertyChanged("Posted");
                VisPosted = Posted ? Visibility.Visible : Visibility.Collapsed;
            }
        }

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

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [ReadOnly(true)]
        public DateTime? PostDate
        {
            get { return _postDate; }
            set
            {
                _postDate = value;
                RaisePropertyChanged("PostDate");
            }
        }

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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
        public DateTime? DocDate
        {
            get { return _docDate; }
            set
            {
                _docDate = value;
                RaisePropertyChanged("DocDate");
            }
        }



        private Visibility _JournalTotalAccount;

        public Visibility JournalTotalAccount
        {
            get { return _JournalTotalAccount; }
            set { _JournalTotalAccount = value; RaisePropertyChanged("JournalTotalAccount"); }
        }


        public TblJournal JournalPerRow
        {
            get { return _journalPerRow; }
            set
            {
                _journalPerRow = value;
                RaisePropertyChanged("JournalPerRow");
                JournalTotalAccount = Visibility.Collapsed;
                if (JournalPerRow != null)
                {
                    TblJournal = _journalPerRow.Iserial;

                    if (JournalPerRow.TblJournalAccountType != null)
                    {

                        if (JournalPerRow.Entity != null)
                        {
                            JournalTotalAccount = Visibility.Visible;
                            client.FindEntityAsync((int)JournalPerRow.TblJournalAccountType, (int)JournalPerRow.Entity, 0, LoggedUserInfo.DatabasEname, true);
                        }
                    }
                }
            }
        }
    }

    public class TblLedgerMainDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {

        public bool NotLoaded { get; set; }
        public TblLedgerMainDetailViewModel()
        {
            PreventPerRow = new GenericTable { Iserial = 1 };
        }

        public GenericTable PreventPerRow { get; set; }

        private bool _bankDeposit;

        public bool BankDeposit
        {
            get { return _bankDeposit; }
            set { _bankDeposit = value; RaisePropertyChanged("BankDeposit"); }
        }

        private int? _glAccount;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAccount")]
        public int? GlAccount
        {
            get { return _glAccount; }
            set { _glAccount = value; RaisePropertyChanged("GlAccount"); }
        }

        private TblAccount _accountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _accountPerRow; }
            set
            {
                if ((ReferenceEquals(_accountPerRow, value) != true))
                {
                    _accountPerRow = value;
                    RaisePropertyChanged("AccountPerRow");
                    GlAccount = _accountPerRow.Iserial;
                }
            }
        }

        private int? _offsetGlAccount;

        public int? OffsetGlAccount
        {
            get { return _offsetGlAccount; }
            set { _offsetGlAccount = value; RaisePropertyChanged("OffsetGlAccount"); }
        }

        private ObservableCollection<TblLedgerMainDetailCostCenterViewModel> _detailsList;

        public ObservableCollection<TblLedgerMainDetailCostCenterViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new ObservableCollection<TblLedgerMainDetailCostCenterViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        private bool? _drOrCr;

        public bool? DrOrCr
        {
            get { return _drOrCr; }
            set
            {
                _drOrCr = value; RaisePropertyChanged("DrOrCr");

                if (DrOrCr == null)
                {
                    DrEnabled = true;
                    CrEnabled = true;
                }
                if (DrOrCr == true)
                {
                    DrEnabled = true;
                    CrEnabled = false;
                }
                if (DrOrCr == false)
                {
                    DrEnabled = false;
                    CrEnabled = true;
                }
            }
        }

        private bool _exchangeRateEnabled;

        public bool ExchangeRateEnabled
        {
            get { return _exchangeRateEnabled; }
            set { _exchangeRateEnabled = value; RaisePropertyChanged("ExchangeRateEnabled"); }
        }

        private bool _drEnabled;

        public bool DrEnabled
        {
            get { return _drEnabled; }
            set { _drEnabled = value; RaisePropertyChanged("DrEnabled"); }
        }

        private bool _crEnabled;

        public bool CrEnabled
        {
            get { return _crEnabled; }
            set { _crEnabled = value; RaisePropertyChanged("CrEnabled"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _codeField;

        [ReadOnly(true)]
        public string Code
        {
            get { return _codeField; }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        private decimal? _drAmount;

        private string _descriptionField;
        private int? _entityAccountField;
        private double _exchangeRate;
        private GenericTable _journalAccountTypePerRow;
        private int _line;
        private int? _offsetEntityAccount;
        private int? _tblCostDimHeaderField;
        private TblCurrencyTest _tblcurrency;
        private int? _tblCurrencyField;
        private int? _tblJournalAccountTypeField;
        private int _tblLedgerHeader;
        private DateTime? _transDate;
        private Entity _entityPerRow;
        private GenericTable _offsetAccountTypePerRow;
        private Entity _offsetEntityPerRow;

        public TblCurrencyTest CurrencyPerRow
        {
            get { return _tblcurrency; }
            set
            {
                if ((ReferenceEquals(_tblcurrency, value) != true))
                {
                    _tblcurrency = value;
                    RaisePropertyChanged("CurrencyPerRow");
                    if (CurrencyPerRow != null)
                    {
                        TblCurrency = CurrencyPerRow.Iserial;
                        if (CurrencyPerRow.ExchangeRate != null) ExchangeRate = (double)CurrencyPerRow.ExchangeRate;
                    }
                }
            }
        }

        public GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    if (JournalAccountTypePerRow != null) TblJournalAccountType = JournalAccountTypePerRow.Iserial;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                }
            }
        }

        public Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                if ((ReferenceEquals(_entityPerRow, value) != true))
                {
                    _entityPerRow = value;
                    RaisePropertyChanged("EntityPerRow");
                    if (EntityPerRow != null)
                    {
                        EntityAccount = EntityPerRow.Iserial;
                        if (EntityPerRow.AccountIserial != 0)
                        {
                            if (!NotLoaded)
                            {
                                GlAccount = EntityPerRow.AccountIserial;
                                AccountPerRow = new TblAccount
                                {
                                    Iserial = EntityPerRow.AccountIserial,
                                    Code = EntityPerRow.AccountCode,
                                    Ename = EntityPerRow.AccountEname,
                                    Aname = EntityPerRow.AccountAname
                                };
                            }
                        }
                    }
                }
            }
        }

        public GenericTable OffsetAccountTypePerRow
        {
            get { return _offsetAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_offsetAccountTypePerRow, value) != true))
                {
                    _offsetAccountTypePerRow = value;
                    if (OffsetAccountTypePerRow != null) OffsetAccountType = _offsetAccountTypePerRow.Iserial;
                    RaisePropertyChanged("OffsetAccountTypePerRow");
                }
            }
        }

        private int? _offsetAccountType;

        public int? OffsetAccountType
        {
            get { return _offsetAccountType; }
            set
            {
                if ((_offsetAccountType.Equals(value) != true))
                {
                    _offsetAccountType = value;
                    RaisePropertyChanged("OffsetAccountType");
                }
            }
        }

        public Entity OffsetEntityPerRow
        {
            get { return _offsetEntityPerRow; }
            set
            {
                if ((ReferenceEquals(_offsetEntityPerRow, value) != true))
                {
                    _offsetEntityPerRow = value;
                    RaisePropertyChanged("OffsetEntityPerRow");
                    if (OffsetEntityPerRow != null)
                    {

                        OffsetEntityAccount = OffsetEntityPerRow.Iserial;
                        if (OffsetEntityPerRow.AccountIserial != 0)
                        {
                            if (!NotLoaded)
                            {
                                OffsetGlAccount = OffsetEntityPerRow.AccountIserial;
                            }
                        }
                        if (OffsetEntityPerRow.AccountIserial == 0)
                        {
                            OffsetEntityPerRow = null;
                            MessageBox.Show("The Offset Entity Doesn't have Account");
                        }
                    }
                    else
                    {
                        OffsetEntityAccount = null;
                        OffsetGlAccount = null;
                    }
                }
            }
        }

        public int TblLedgerHeader
        {
            get { return _tblLedgerHeader; }
            set
            {
                _tblLedgerHeader = value;
                RaisePropertyChanged("TblLedgerHeader");
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
        public int? TblCurrency
        {
            get { return _tblCurrencyField; }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public int? TblCostDimHeader
        {
            get { return _tblCostDimHeaderField; }
            set
            {
                if ((_tblCostDimHeaderField.Equals(value) != true))
                {
                    _tblCostDimHeaderField = value;
                    RaisePropertyChanged("TblCostDimHeader");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get { return _tblJournalAccountTypeField; }
            set
            {
                if ((_tblJournalAccountTypeField.Equals(value) != true))
                {
                    _tblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAccount")]
        public int? EntityAccount
        {
            get { return _entityAccountField; }
            set
            {
                if ((_entityAccountField.Equals(value) != true))
                {
                    _entityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }

        public int LineNumber
        {
            get { return _line; }
            set
            {
                _line = value;
                RaisePropertyChanged("LineNumber");
            }
        }

        public decimal? DrAmount
        {
            get { return _drAmount; }
            set
            {
                _drAmount = value;
                RaisePropertyChanged("DrAmount");
            }
        }

        private decimal? _crAmount;

        public decimal? CrAmount
        {
            get { return _crAmount; }
            set
            {
                _crAmount = value;
                RaisePropertyChanged("CrAmount");
            }
        }

        public double ExchangeRate
        {
            get { return _exchangeRate; }
            set
            {
                _exchangeRate = value;
                RaisePropertyChanged("ExchangeRate");
            }
        }

        public DateTime? TransDate
        {
            get { return _transDate; }
            set
            {
                _transDate = value;
                RaisePropertyChanged("TransDate");
            }
        }

        public int? OffsetEntityAccount
        {
            get { return _offsetEntityAccount; }
            set
            {
                if ((_offsetEntityAccount.Equals(value) != true))
                {
                    _offsetEntityAccount = value;
                    RaisePropertyChanged("OffsetEntityAccount");
                }
            }
        }

        public string Description
        {
            get { return _descriptionField; }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private bool _transactionExists;

        public bool TransactionExists
        {
            get { return _transactionExists; }
            set
            {
                _transactionExists = value;
                RaisePropertyChanged("TransactionExists");
            }
        }

        private string _paymentRef;

        public string PaymentRef
        {
            get { return _paymentRef; }
            set { _paymentRef = value; RaisePropertyChanged("PaymentRef"); }
        }

        private int? _tblBankTransactionType;

        public int? TblBankTransactionType
        {
            get
            {
                return _tblBankTransactionType;
            }
            set
            {
                if ((_tblBankTransactionType.Equals(value) != true))
                {
                    _tblBankTransactionType = value;
                    RaisePropertyChanged("TblBankTransactionType");
                }
            }
        }

        private GenericTable _bankTransactionTypePerRow;

        public GenericTable BankTransactionTypePerRow
        {
            get { return _bankTransactionTypePerRow; }
            set
            {
                if ((ReferenceEquals(_bankTransactionTypePerRow, value) != true))
                {
                    _bankTransactionTypePerRow = value;
                    RaisePropertyChanged("BankTransactionTypePerRow");
                    if (BankTransactionTypePerRow != null) TblBankTransactionType = BankTransactionTypePerRow.Iserial;
                }
            }
        }

        private TblMethodOfPayment _methodOfPaymentPerRow;

        public TblMethodOfPayment MethodOfPaymentPerRow
        {
            get { return _methodOfPaymentPerRow; }
            set
            {
                if ((ReferenceEquals(_methodOfPaymentPerRow, value) != true))
                {
                    _methodOfPaymentPerRow = value;
                    RaisePropertyChanged("MethodOfPaymentPerRow");
                    if (MethodOfPaymentPerRow != null)
                    {
                        TblMethodOfPayment = MethodOfPaymentPerRow.Iserial;
                        if (MethodOfPaymentPerRow.TblBankTransactionType1 != null)
                        {
                            BankTransactionTypePerRow = new GenericTable();
                            var row = new GenericTable();
                            row.InjectFrom(MethodOfPaymentPerRow.TblBankTransactionType1);
                            BankTransactionTypePerRow = row;
                        }
                    }
                }
            }
        }

        private int? _tblMethodOfPayment;

        public int? TblMethodOfPayment
        {
            get { return _tblMethodOfPayment; }
            set { _tblMethodOfPayment = value; RaisePropertyChanged("TblMethodOfPayment"); }
        }

        private int? _tblBankCheque;

        public int? TblBankCheque
        {
            get { return _tblBankCheque; }
            set { _tblBankCheque = value; RaisePropertyChanged("TblBankCheque"); }
        }

        private TblBankCheque _chequePerRow;

        public TblBankCheque ChequePerRow
        {
            get { return _chequePerRow; }
            set
            {
                _chequePerRow = value; RaisePropertyChanged("ChequePerRow");
                if (ChequePerRow != null)
                {
                    TblBankCheque = ChequePerRow.Iserial;
                }
                else
                {
                    TblBankCheque = null;
                }
            }
        }
    }

    public class TblLedgerMainDetailCostCenterViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private bool _Calculated;

        public bool Calculated
        {
            get { return _Calculated; }
            set { _Calculated = value; RaisePropertyChanged("Calculated"); }
        }

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

        private int _tblLedgerMainDetail;

        public int TblLedgerMainDetail
        {
            get { return _tblLedgerMainDetail; }
            set { _tblLedgerMainDetail = value; RaisePropertyChanged("TblLedgerMainDetail"); }
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