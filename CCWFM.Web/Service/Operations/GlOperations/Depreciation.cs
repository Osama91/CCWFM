using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblDepreciationTransactionHeader> GetTblDepreciationTransactionHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblDepreciationTransactionHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblDepreciationTransactionHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblDepreciationTransactionHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblDepreciationTransactionHeaders.Count();
                    query = entity.TblDepreciationTransactionHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblDepreciationTransactionHeader UpdateOrInsertTblDepreciationTransactionHeader(TblDepreciationTransactionHeader newRow, bool save, int index, int user, bool approve, out int outindex,
            string company)
        {
            using (var scope = new TransactionScope())
            {
                #region Code

                outindex = index;
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    if (save)
                    {
                        newRow.CreationDate = DateTime.Now;
                        var seqCode = entity.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlDepreciation").sSetupValue;
                        var journal = entity.TblSequences.FirstOrDefault(x => x.Code == seqCode);
                        int seq;

                        newRow.Code = SharedOperation.HandelSequence(newRow.Code, journal, "TblLedgerHeader", company, 0, newRow.TransDate.Value.Month, newRow.TransDate.Value.Year, out seq);

                        entity.TblDepreciationTransactionHeaders.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in entity.TblDepreciationTransactionHeaders
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            foreach (var newDetailRow in newRow.TblDepreciationTransactionDetails.ToList())
                            {
                                if (newDetailRow.Iserial == 0)
                                {
                                    newDetailRow.TblDepreciationTransactionHeader1 = null;
                                    newDetailRow.TblDepreciationTransactionHeader = oldRow.Iserial;
                                    entity.TblDepreciationTransactionDetails.AddObject(newDetailRow);
                                }
                                else
                                {
                                    var oldRowDetail = (from e in entity.TblDepreciationTransactionDetails
                                                        where e.Iserial == newDetailRow.Iserial
                                                        select e).SingleOrDefault();
                                    if (oldRowDetail != null)
                                    {
                                        GenericUpdate(oldRowDetail, newDetailRow, entity);
                                    }
                                }
                            }
                            GenericUpdate(oldRow, newRow, entity);
                        }
                    }
                    if (approve)
                    {
                        var tblLedgerMainDetail = entity.TblLedgerMainDetails.FirstOrDefault(
                            x => x.Iserial == newRow.TblLedgerMainDetail);
                        var Break = false;
                        if (tblLedgerMainDetail != null)
                        {
                            var depdetails =
                                entity.TblDepreciationTransactionDetails.Where(
                                    x => x.TblDepreciationTransactionHeader == newRow.Iserial);

                            foreach (var VARIABLE in depdetails)
                            {
                                entity.TblDepreciationTransactionDetails.DeleteObject(VARIABLE);
                            }
                            double accDepreciation = 0;
                            var startdate = newRow.StartDate;
                            if (startdate.Value.Day != 1)
                            {
                                startdate = new DateTime(startdate.Value.Year, startdate.Value.Month + 1, 1);
                            }

                            var partialyear = false;
                            if (startdate.Value.Month != 1)
                            {
                                partialyear = true;
                            }

                            var dep = (newRow.BookValue - newRow.SalvageValue) / newRow.DepreciationLife;

                            var bookBalue = newRow.BookValue;

                            for (int y = 0; y < newRow.DepreciationLife; y++)
                            {
                                if (Break)
                                {
                                    break;
                                }
                                var depExpense = dep / 12;

                                if (newRow.TblDepreciationMethod == 1)
                                {
                                    depExpense = (newRow.BookValue - accDepreciation) * (1 / newRow.DepreciationLife * newRow.DepreciationFactor);
                                }

                                var NOOFM = 12;
                                for (int i = 0; i < 12; i++)
                                {
                                    var duedate = startdate.Value.AddMonths(i).AddYears(y);
                                    if (newRow.TblDepreciationMethod == 2)
                                    {
                                        depExpense = dep / 12;
                                        accDepreciation = (double)(accDepreciation + depExpense);
                                    }
                                    if (newRow.TblDepreciationMethod == 1)
                                    {
                                        if (y + 1 == newRow.DepreciationLife && duedate.Month == 1)
                                        {
                                            if (partialyear)
                                            {
                                                NOOFM = (12 - (13 - newRow.StartDate.Value.Month));
                                                var ff = ((decimal)NOOFM / 12);
                                                var factor = (newRow.DepreciationFactor / 10 * (double)ff);
                                                depExpense = (newRow.BookValue - accDepreciation) * (factor);
                                            }
                                        }
                                        accDepreciation = (double)(accDepreciation + (depExpense / NOOFM));
                                    }
                                    double bookValuePerRow = 0;
                                    bookValuePerRow = (double)(bookBalue - accDepreciation);
                                    if (bookValuePerRow <= newRow.SalvageValue.Value)
                                    {
                                        accDepreciation = (double)(accDepreciation - (newRow.SalvageValue - bookValuePerRow));
                                        depExpense = depExpense - (newRow.SalvageValue - bookValuePerRow);
                                        bookValuePerRow = (int)newRow.SalvageValue;
                                    }

                                    var depereciationdetail = new TblDepreciationTransactionDetail
                                    {
                                        TblDepreciationTransactionHeader = newRow.Iserial,
                                        DueDate = duedate,
                                        DepExpense = depExpense,
                                        EndBookValue = bookValuePerRow,
                                        StartBookValue = bookValuePerRow + depExpense,
                                        AccDepreciation = accDepreciation,
                                    };
                                    if (newRow.TblDepreciationMethod == 1)
                                    {
                                        depereciationdetail.StartBookValue = bookValuePerRow + depExpense / NOOFM;
                                        if (bookValuePerRow != newRow.SalvageValue.Value)
                                        {
                                            depereciationdetail.DepExpense = depExpense / NOOFM;
                                        }
                                        else
                                        {
                                            depereciationdetail.DepExpense = depereciationdetail.EndBookValue - depereciationdetail.StartBookValue;
                                        }
                                    }
                                    entity.TblDepreciationTransactionDetails.AddObject(depereciationdetail);
                                    if (bookValuePerRow == newRow.SalvageValue.Value)
                                    {
                                        Break = true;
                                        break;
                                    }
                                }
                            }
                        }
                        newRow.Status = 1;
                    }

                    try
                    {
                        entity.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        if (ExceptionContainsErrorCode(ex, 2627))
                        {
                            entity.Detach(newRow);
                            UpdateOrInsertTblDepreciationTransactionHeader(newRow, save, index, user, approve, out outindex,company);

                            
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    
                }

                #endregion Code

                scope.Complete();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblDepreciationTransactionHeader(TblDepreciationTransactionHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblDepreciationTransactionHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblDepreciationTransactionDetail> GetTblDepreciationTransactionDetail(int header, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblDepreciationTransactionDetail> query;

                query = entity.TblDepreciationTransactionDetails.Where(x => x.TblDepreciationTransactionHeader == header);

                return query.ToList();
            }
        }

        [OperationContract]
        private int DeleteTblDepreciationTransactionDetail(TblDepreciationTransactionDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblDepreciationTransactionDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblLedgerMainDetail> GetTblLedgerDetailForDepreciation(int skip, int take, bool depreciation, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList, out Dictionary<int, bool> TransactionExist)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblLedgerMainDetail> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    var tempQuery =
                        entity.TblLedgerMainDetails.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort)
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Where(
                                v => v.TblJournalAccountType == 5 &&
                                    (!entity.TblDepreciationTransactionHeaders.Select(x => x.TblLedgerMainDetail)
                                        .Contains(v.Iserial) || depreciation == false));

                    fullCount = tempQuery.Count();
                    query = tempQuery.Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblLedgerMainDetails.Count(v => (v.TblJournalAccountType == 5 && !entity.TblDepreciationTransactionHeaders.Select(x => x.TblLedgerMainDetail).Contains(v.Iserial) || depreciation == false));
                    query = entity.TblLedgerMainDetails.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(v => (v.TblJournalAccountType == 5 && !entity.TblDepreciationTransactionHeaders.Select(x => x.TblLedgerMainDetail).Contains(v.Iserial) || depreciation == false)).Skip(skip).Take(take);
                }
                List<int?> intList = query.Select(x => x.EntityAccount).ToList();

                intList.AddRange(query.Select(x => x.OffsetEntityAccount).ToList());

                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                intTypeList.AddRange(query.Select(x => x.OffsetAccountType).ToList());
                entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                var listOfStyles = query.Select(x => x.Iserial);
                TransactionExist = entity.TblLedgerDetailCostCenters.Where(x => listOfStyles.Count(l => x.TblLedgerMainDetail == l) > 1)
                        .GroupBy(x => x.TblLedgerMainDetail).ToDictionary(t => t.Key, t => true);
                return query.ToList();
            }
        }

        //[OperationContract]
        //private void PostTblDepreciationTransactionHeader(int iserial, int user, string company, string code)
        //{
        //    using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
        //    {
        //        var chequeTransactionHeader =
        //            entity.TblDepreciationTransactionHeaders.Include("TblGlChequeTypeSetting1")
        //                .Include("TblDepreciationTransactionDetails.TblBankCheque1.TblBank1")
        //                .FirstOrDefault(x => x.Iserial == iserial);
        //        var journalint =
        //            entity.TblJournals.FirstOrDefault(
        //                x => x.Iserial == chequeTransactionHeader.TblGlChequeTypeSetting1.TblJournal).Iserial;
        //        var newLedgerHeaderRow = new TblLedgerHeader
        //        {
        //            CreatedBy = chequeTransactionHeader.CreatedBy,
        //            CreationDate = DateTime.Now,
        //            Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted + " " + code,
        //            DocDate = chequeTransactionHeader.DocDate,
        //            TblJournal = journalint,
        //            TblTransactionType = 7,
        //            TblJournalLink = chequeTransactionHeader.Iserial
        //        };

        //        var tempheader = 0;
        //        UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
        //                     company);
        //        var newLedgerHeaderRowIssue = new TblLedgerHeader();
        //        if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
        //        {
        //            newLedgerHeaderRowIssue = new TblLedgerHeader
        //            {
        //                CreatedBy = chequeTransactionHeader.CreatedBy,
        //                CreationDate = DateTime.Now,
        //                Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted,
        //                DocDate = chequeTransactionHeader.DocDate,
        //                TblJournal = journalint,
        //                TblTransactionType = 7,
        //                TblJournalLink = chequeTransactionHeader.Iserial
        //            };
        //            UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRowIssue, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
        //                 company);
        //        }

        //        if (!chequeTransactionHeader.TblGlChequeTypeSetting1.DetailSummary)
        //        {
        //            foreach (var row in chequeTransactionHeader.TblDepreciationTransactionDetails)
        //            {
        //                #region إلغاء شيك

        //                row.TblGlChequeStatus = 3;
        //                if (chequeTransactionHeader.TblGlChequeTypeSetting == 8)
        //                {
        //                    var recCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 4);

        //                    if (recCheque != null)
        //                    {
        //                        var recrow =
        //                            entity.TblLedgerHeaders.FirstOrDefault(x => x.TblJournalLink == recCheque.TblDepreciationTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recCheque.Amount;
        //                            ReverseTblledgerHeader(recrow.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                        }
        //                    }

        //                    var recVendCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 3);

        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                       entity.TblLedgerHeaders.Where(x => x.TblJournalLink == recVendCheque.TblDepreciationTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recVendCheque.Amount;
        //                            foreach (var VARIABLE in recrow)
        //                            {
        //                                ReverseTblledgerHeader(VARIABLE.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                            }
        //                        }
        //                    }

        //                    recVendCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 2);
        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                              entity.TblLedgerHeaders.FirstOrDefault(
        //                                  x =>
        //                                      x.TblJournalLink == recVendCheque.TblDepreciationTransactionHeader &&
        //                                      x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recVendCheque.Amount;
        //                            ReverseTblledgerHeader(recrow.Iserial, user, company, 0,
        //                                newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                        }
        //                    }
        //                }

        //                #endregion إلغاء شيك

        //                #region تسليم اوراق دفع من الخزنة للمورد

        //                if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
        //                {
        //                    row.TblGlChequeStatus = 2;
        //                    var newledgerDetailrowh1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };
        //                    var accountDetail1 =
        //                        entity.Entities.FirstOrDefault(
        //                            x =>
        //                                x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType &&
        //                                x.Iserial == row.EntityDetail1).AccountIserial;

        //                    var newledgerDetailrowd1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),

        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = row.EntityDetail1TblJournalAccountType,
        //                        EntityAccount = row.EntityDetail1,
        //                        GlAccount = accountDetail1,
        //                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var newledgerDetailrowh2 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader2TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader2,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader2TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader2PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var newledgerDetailrowd2 = new TblLedgerMainDetail();
        //                    if (row.DueDate <= chequeTransactionHeader.DocDate.Value.AddDays(chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays)
        //                        ||
        //                        (chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays == 0
        //                        && chequeTransactionHeader.DocDate.Value.Month == row.DueDate.Value.Month &&
        //                        chequeTransactionHeader.DocDate.Value.Year == row.DueDate.Value.Year))
        //                    {
        //                        var bank = row.TblBankCheque1.TblBank;
        //                        var bankAccount =
        //                      entity.Entities.FirstOrDefault(
        //                          x =>
        //                              x.TblJournalAccountType == 6 &&
        //                              x.Iserial == bank).AccountIserial;

        //                        newledgerDetailrowd2 = new TblLedgerMainDetail
        //                   {
        //                       Amount = (decimal?)row.Amount,
        //                       Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                       ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                       TblCurrency = chequeTransactionHeader.TblCurrency,
        //                       TransDate = chequeTransactionHeader.DocDate,
        //                       TblJournalAccountType = 6,
        //                       EntityAccount = bank,
        //                       GlAccount = bankAccount,
        //                       TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                       PaymentRef = "",
        //                       DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
        //                       TblBankCheque = row.TblBankCheque,
        //                   };
        //                    }
        //                    else
        //                    {
        //                        row.TblGlChequeStatus = 1;
        //                        newledgerDetailrowd2 = new TblLedgerMainDetail
        //                        {
        //                            Amount = (decimal?)row.Amount,
        //                            Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                            ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                            TblCurrency = chequeTransactionHeader.TblCurrency,
        //                            TransDate = chequeTransactionHeader.DocDate,
        //                            TblJournalAccountType = 0,
        //                            EntityAccount = row.EntityDetail2TblAccount,
        //                            GlAccount = (int)row.EntityDetail2TblAccount,
        //                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                            PaymentRef = "",
        //                            DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
        //                            TblBankCheque = row.TblBankCheque,
        //                        };
        //                    }
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
        //            (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd1, true, 000, out tempheader, company,
        //                 (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh2, true, 000, out tempheader, company,
        //                (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
        //                 (int)chequeTransactionHeader.CreatedBy);
        //                }

        //                #endregion تسليم اوراق دفع من الخزنة للمورد

        //                #region تسليم اوراق دفع للبنك

        //                else if (chequeTransactionHeader.TblGlChequeTypeSetting == 4)
        //                {
        //                    row.TblGlChequeStatus = 2;
        //                    var newledgerDetailrowh1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var bank = row.TblBankCheque1.TblBank;
        //                    var bankAccount =
        //                  entity.Entities.FirstOrDefault(
        //                      x =>
        //                          x.TblJournalAccountType == 6 &&
        //                          x.Iserial == bank).AccountIserial;

        //                    var newledgerDetailrowd2 = new TblLedgerMainDetail
        //                      {
        //                          Amount = (decimal?)row.Amount,
        //                          Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                          ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                          TblCurrency = chequeTransactionHeader.TblCurrency,
        //                          TransDate = chequeTransactionHeader.DocDate,
        //                          TblJournalAccountType = 6,
        //                          EntityAccount = bank,
        //                          GlAccount = bankAccount,
        //                          TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                          PaymentRef = "",
        //                          TblBankCheque = row.TblBankCheque,
        //                          DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                      };
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //                }

        //                #endregion تسليم اوراق دفع للبنك
        //            }
        //        }
        //        else
        //        {
        //            #region طلب اصدار شيكات

        //            foreach (var row in chequeTransactionHeader.TblDepreciationTransactionDetails.GroupBy(x => x.TblDepreciationTransactionHeader1.EntityHeader1TblAccount))
        //            {
        //                var newledgerDetailrow = new TblLedgerMainDetail
        //                {
        //                    Amount = (decimal?)row.Sum(x => x.Amount),
        //                    Description = "Cheque Transaction",
        //                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                    TblCurrency = chequeTransactionHeader.TblCurrency,
        //                    TransDate = chequeTransactionHeader.DocDate,
        //                    TblJournalAccountType = 0,
        //                    EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                    GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                    PaymentRef = "",
        //                    DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                };

        //                var newledgerDetailrow2 = new TblLedgerMainDetail
        //                {
        //                    Amount = (decimal?)row.Sum(x => x.Amount),
        //                    Description = "Cheque Transaction",
        //                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                    TblCurrency = chequeTransactionHeader.TblCurrency,
        //                    TransDate = chequeTransactionHeader.DocDate,
        //                    TblJournalAccountType = 0,
        //                    EntityAccount = row.FirstOrDefault().EntityDetail1TblAccount,
        //                    GlAccount = (int)row.FirstOrDefault().EntityDetail1TblAccount,
        //                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                    PaymentRef = "",
        //                    DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                };
        //                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out tempheader, company,
        //        (int)chequeTransactionHeader.CreatedBy);

        //                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow2, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //            }

        //            #endregion طلب اصدار شيكات
        //        }

        //        chequeTransactionHeader.ApproveDate = DateTime.Now;
        //        chequeTransactionHeader.ApprovedBy = user;
        //        chequeTransactionHeader.Approved = true;
        //        entity.SaveChanges();
        //    }
        //}

        //[OperationContract]
        //private void PostTblDepreciationTransactionHeaderDesc(int iserial, int user, string company)
        //{
        //    using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
        //    {
        //        var chequeTransactionHeader =
        //            entity.TblDepreciationTransactionHeaders.Include("TblGlChequeTypeSetting1")
        //                .Include("TblDepreciationTransactionDetails.TblBankCheque1.TblBank1")
        //                .FirstOrDefault(x => x.Iserial == iserial);
        //        var journalint =
        //            entity.TblJournals.FirstOrDefault(
        //                x => x.Iserial == chequeTransactionHeader.TblGlChequeTypeSetting1.TblJournal).Iserial;
        //        var newLedgerHeaderRow = new TblLedgerHeader
        //        {
        //            CreatedBy = chequeTransactionHeader.CreatedBy,
        //            CreationDate = DateTime.Now,
        //            Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted,
        //            DocDate = chequeTransactionHeader.DocDate,
        //            TblJournal = journalint,
        //            TblTransactionType = 7,
        //            TblJournalLink = chequeTransactionHeader.Iserial
        //        };

        //        var tempheader = 0;
        //        UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
        //                     company);
        //        var newLedgerHeaderRowIssue = new TblLedgerHeader();
        //        if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
        //        {
        //            newLedgerHeaderRowIssue = new TblLedgerHeader
        //            {
        //                CreatedBy = chequeTransactionHeader.CreatedBy,
        //                CreationDate = DateTime.Now,
        //                Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted,
        //                DocDate = chequeTransactionHeader.DocDate,
        //                TblJournal = journalint,
        //                TblTransactionType = 7,
        //                TblJournalLink = chequeTransactionHeader.Iserial
        //            };
        //            UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRowIssue, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
        //                 company);
        //        }

        //        if (!chequeTransactionHeader.TblGlChequeTypeSetting1.DetailSummary)
        //        {
        //            foreach (var row in chequeTransactionHeader.TblDepreciationTransactionDetails)
        //            {
        //                #region إلغاء شيك

        //                row.TblGlChequeStatus = 3;
        //                if (chequeTransactionHeader.TblGlChequeTypeSetting == 8)
        //                {
        //                    var recCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 4);

        //                    if (recCheque != null)
        //                    {
        //                        var recrow =
        //                            entity.TblLedgerHeaders.FirstOrDefault(x => x.TblJournalLink == recCheque.TblDepreciationTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recCheque.Amount;
        //                            ReverseTblledgerHeader(recrow.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                        }
        //                    }

        //                    var recVendCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 3);

        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                       entity.TblLedgerHeaders.Where(x => x.TblJournalLink == recVendCheque.TblDepreciationTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recVendCheque.Amount;
        //                            foreach (var VARIABLE in recrow)
        //                            {
        //                                ReverseTblledgerHeader(VARIABLE.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                            }
        //                        }
        //                    }

        //                    recVendCheque = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblDepreciationTransactionHeader1.TblGlChequeTypeSetting == 2);
        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                              entity.TblLedgerHeaders.FirstOrDefault(
        //                                  x =>
        //                                      x.TblJournalLink == recVendCheque.TblDepreciationTransactionHeader &&
        //                                      x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recVendCheque.Amount;
        //                            ReverseTblledgerHeader(recrow.Iserial, user, company, 0,
        //                                newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                        }
        //                    }
        //                }

        //                #endregion إلغاء شيك

        //                #region تسليم اوراق دفع من الخزنة للمورد

        //                if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
        //                {
        //                    row.TblGlChequeStatus = 2;
        //                    var newledgerDetailrowh1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };
        //                    var accountDetail1 =
        //                        entity.Entities.FirstOrDefault(
        //                            x =>
        //                                x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType &&
        //                                x.Iserial == row.EntityDetail1).AccountIserial;

        //                    var newledgerDetailrowd1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),

        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = row.EntityDetail1TblJournalAccountType,
        //                        EntityAccount = row.EntityDetail1,
        //                        GlAccount = accountDetail1,
        //                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var newledgerDetailrowh2 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader2TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader2,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader2TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader2PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var newledgerDetailrowd2 = new TblLedgerMainDetail();
        //                    if (row.DueDate <= chequeTransactionHeader.DocDate.Value.AddDays(chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays)
        //                        ||
        //                        (chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays == 0
        //                        && chequeTransactionHeader.DocDate.Value.Month == row.DueDate.Value.Month &&
        //                        chequeTransactionHeader.DocDate.Value.Year == row.DueDate.Value.Year))
        //                    {
        //                        var bank = row.TblBankCheque1.TblBank;
        //                        var bankAccount =
        //                      entity.Entities.FirstOrDefault(
        //                          x =>
        //                              x.TblJournalAccountType == 6 &&
        //                              x.Iserial == bank).AccountIserial;

        //                        newledgerDetailrowd2 = new TblLedgerMainDetail
        //                        {
        //                            Amount = (decimal?)row.Amount,
        //                            Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                            ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                            TblCurrency = chequeTransactionHeader.TblCurrency,
        //                            TransDate = chequeTransactionHeader.DocDate,
        //                            TblJournalAccountType = 6,
        //                            EntityAccount = bank,
        //                            GlAccount = bankAccount,
        //                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                            PaymentRef = "",
        //                            DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
        //                            TblBankCheque = row.TblBankCheque,
        //                        };
        //                    }
        //                    else
        //                    {
        //                        row.TblGlChequeStatus = 1;
        //                        newledgerDetailrowd2 = new TblLedgerMainDetail
        //                        {
        //                            Amount = (decimal?)row.Amount,
        //                            Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                            ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                            TblCurrency = chequeTransactionHeader.TblCurrency,
        //                            TransDate = chequeTransactionHeader.DocDate,
        //                            TblJournalAccountType = 0,
        //                            EntityAccount = row.EntityDetail2TblAccount,
        //                            GlAccount = (int)row.EntityDetail2TblAccount,
        //                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                            PaymentRef = "",
        //                            DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
        //                            TblBankCheque = row.TblBankCheque,
        //                        };
        //                    }
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
        //            (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd1, true, 000, out tempheader, company,
        //                 (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh2, true, 000, out tempheader, company,
        //                (int)chequeTransactionHeader.CreatedBy);

        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
        //                 (int)chequeTransactionHeader.CreatedBy);
        //                }

        //                #endregion تسليم اوراق دفع من الخزنة للمورد

        //                #region تسليم اوراق دفع للبنك

        //                else if (chequeTransactionHeader.TblGlChequeTypeSetting == 4)
        //                {
        //                    row.TblGlChequeStatus = 2;
        //                    var newledgerDetailrowh1 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
        //                        EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                        PaymentRef = "",
        //                        DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var bank = row.TblBankCheque1.TblBank;
        //                    var bankAccount =
        //                  entity.Entities.FirstOrDefault(
        //                      x =>
        //                          x.TblJournalAccountType == 6 &&
        //                          x.Iserial == bank).AccountIserial;

        //                    var newledgerDetailrowd2 = new TblLedgerMainDetail
        //                    {
        //                        Amount = (decimal?)row.Amount,
        //                        Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
        //                        ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                        TblCurrency = chequeTransactionHeader.TblCurrency,
        //                        TransDate = chequeTransactionHeader.DocDate,
        //                        TblJournalAccountType = 6,
        //                        EntityAccount = bank,
        //                        GlAccount = bankAccount,
        //                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                        PaymentRef = "",
        //                        TblBankCheque = row.TblBankCheque,
        //                        DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                    };
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //                }

        //                #endregion تسليم اوراق دفع للبنك
        //            }
        //        }
        //        else
        //        {
        //            #region طلب اصدار شيكات

        //            foreach (var row in chequeTransactionHeader.TblDepreciationTransactionDetails.GroupBy(x => x.TblDepreciationTransactionHeader1.EntityHeader1TblAccount))
        //            {
        //                var newledgerDetailrow = new TblLedgerMainDetail
        //                {
        //                    Amount = (decimal?)row.Sum(x => x.Amount),
        //                    Description = "Cheque Transaction",
        //                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                    TblCurrency = chequeTransactionHeader.TblCurrency,
        //                    TransDate = chequeTransactionHeader.DocDate,
        //                    TblJournalAccountType = 0,
        //                    EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
        //                    GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
        //                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                    PaymentRef = "",
        //                    DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                };

        //                var newledgerDetailrow2 = new TblLedgerMainDetail
        //                {
        //                    Amount = (decimal?)row.Sum(x => x.Amount),
        //                    Description = "Cheque Transaction",
        //                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
        //                    TblCurrency = chequeTransactionHeader.TblCurrency,
        //                    TransDate = chequeTransactionHeader.DocDate,
        //                    TblJournalAccountType = 0,
        //                    EntityAccount = row.FirstOrDefault().EntityDetail1TblAccount,
        //                    GlAccount = (int)row.FirstOrDefault().EntityDetail1TblAccount,
        //                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
        //                    PaymentRef = "",
        //                    DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
        //                };
        //                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out tempheader, company,
        //        (int)chequeTransactionHeader.CreatedBy);

        //                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow2, true, 000, out tempheader, company,
        //             (int)chequeTransactionHeader.CreatedBy);
        //            }

        //            #endregion طلب اصدار شيكات
        //        }

        //        chequeTransactionHeader.ApproveDate = DateTime.Now;
        //        chequeTransactionHeader.ApprovedBy = user;
        //        chequeTransactionHeader.Approved = true;
        //        entity.SaveChanges();
        //    }
        //}
    }
}