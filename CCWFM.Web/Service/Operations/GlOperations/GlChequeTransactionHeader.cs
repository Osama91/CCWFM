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
        private TblGlChequeTypeSetting GetTblChequeTypeSettings(string code, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblGlChequeTypeSettings.Include("TblJournalAccountType").Include("TblJournalAccountType1").Include("TblJournalAccountType2").Include("TblJournalAccountType3").Include("TblCurrency").FirstOrDefault(x => x.Code == code);

                var intList = new List<int?>();

                // ReSharper disable once PossibleNullReferenceException
                intList.Add(query.DefaultHeaderEntity1);

                intList.Add(query.DefaultHeaderEntity2);

                var intTypeList = new List<int?>();
                intTypeList.Add(query.EntityHeader1TblJournalAccountType);

                intTypeList.Add(query.EntityHeader2TblJournalAccountType);
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                return query;
            }
        }

        [OperationContract]
        private List<TblGlChequeTransactionHeader> GetTblGlChequeTransactionHeader(int skip, int take, int typeSetting, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlChequeTransactionHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlChequeTypeSetting ==(@Group0)";
                    valuesObjects.Add("Group0", typeSetting);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlChequeTransactionHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlChequeTransactionHeaders.Include("TblJournalAccountType").Include("TblJournalAccountType1").Include("TblGlChequeTypeSetting1").Include("TblCurrency1").Include("TblAccount").Include("TblAccount1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlChequeTransactionHeaders.Count(x => x.TblGlChequeTypeSetting == typeSetting);
                    query = entity.TblGlChequeTransactionHeaders.Include("TblJournalAccountType").Include("TblJournalAccountType1").Include("TblGlChequeTypeSetting1").Include("TblCurrency1").Include("TblAccount").Include("TblAccount1").OrderBy(sort).Where(v => v.TblGlChequeTypeSetting == typeSetting).Skip(skip).Take(take);
                }

                List<int?> intList = query.Select(x => x.EntityHeader1).ToList();

                intList.AddRange(query.Select(x => x.EntityHeader2).ToList());

                List<int?> intTypeList = query.Select(x => x.EntityHeader1TblJournalAccountType).ToList();

                intTypeList.AddRange(query.Select(x => x.EntityHeader2TblJournalAccountType).ToList());
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType) && x.scope == 0).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlChequeTransactionHeader UpdateOrInsertTblGlChequeTransactionHeader(
            TblGlChequeTransactionHeader newRow, bool save, int index, int user, bool approve, out int outindex,
            string company)
        {
            using (var scope = new TransactionScope())
            {
                #region Code

                bool ww = DateTime.Now > DateTime.Now.AddDays(1);
                outindex = index;
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var GlChequesetting =
                       entity.TblGlChequeTypeSettings.FirstOrDefault(
                           x => x.Iserial == newRow.TblGlChequeTypeSetting);

                    if (save)
                    {
                        try
                        {
                            var firstrow = newRow.TblGlChequeTransactionDetails.FirstOrDefault();
                            var entitySelected =
                                entity.Entities.FirstOrDefault(
                                    x => x.TblJournalAccountType == firstrow.EntityDetail1TblJournalAccountType &&
                                         x.Iserial == firstrow.EntityDetail1);
                            newRow.Description = entitySelected.Ename;
                        }
                        catch (Exception)
                        {
                        }

                        newRow.CreationDate = DateTime.Now;
                        newRow.CreatedBy = user;
                        var setting =
                            entity.TblGlChequeTypeSettings.FirstOrDefault(
                                x => x.Iserial == newRow.TblGlChequeTypeSetting).TblSequence;
                        var journal = entity.TblSequences.FirstOrDefault(x => x.Iserial == setting);
                        int temp = 0;

                        newRow.Code = SharedOperation.HandelSequence(newRow.Code, journal, "TblLedgerHeader", company, 0,
                            newRow.DocDate.Value.Month, newRow.DocDate.Value.Year, out temp);

                        newRow.CreationDate = DateTime.Now;


                        foreach (var variable in newRow.TblGlChequeTransactionDetails.ToList())
                        {
                            var oldRowDetail = (from e in entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1")
                                                where e.Iserial == variable.TblGlChequeTransactionDetail1
                                                select e).SingleOrDefault();
                            if (oldRowDetail != null)
                            {
                                oldRowDetail.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;
                                newRow.TblCurrency = oldRowDetail.TblGlChequeTransactionHeader1.TblCurrency;
                                newRow.ExchangeRate = oldRowDetail.TblGlChequeTransactionHeader1.ExchangeRate;
                            }
                            variable.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;
                        }
                        entity.TblGlChequeTransactionHeaders.AddObject(newRow);
                    }
                    else
                    {

                        var oldRow = (from e in entity.TblGlChequeTransactionHeaders
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            foreach (var newDetailRow in newRow.TblGlChequeTransactionDetails.ToList())
                            {

                                var oldRowDetailTemp = (from e in entity.TblGlChequeTransactionDetails
                                                        where e.Iserial == newDetailRow.TblGlChequeTransactionDetail1
                                                        select e).SingleOrDefault();
                                if (oldRowDetailTemp != null)
                                {
                                    oldRowDetailTemp.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;
                                }
                                newDetailRow.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;


                                if (newDetailRow.Iserial == 0)
                                {
                                    newDetailRow.TblGlChequeTransactionHeader1 = null;
                                    newDetailRow.TblGlChequeTransactionHeader = oldRow.Iserial;
                                    entity.TblGlChequeTransactionDetails.AddObject(newDetailRow);
                                }
                                else
                                {
                                    var oldRowDetail = (from e in entity.TblGlChequeTransactionDetails
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
                    entity.SaveChanges();
                    //var GlChequesetting =
                    //    entity.TblGlChequeTypeSettings.FirstOrDefault(
                    //        x => x.Iserial == newRow.TblGlChequeTypeSetting);
                    //foreach (var variable in newRow.TblGlChequeTransactionDetails)
                    //{
                    //    var oldRowDetail = (from e in entity.TblGlChequeTransactionDetails
                    //                        where e.Iserial == variable.TblGlChequeTransactionDetail1
                    //                        select e).SingleOrDefault();


                    //    if (oldRowDetail != null)
                    //    {
                    //        oldRowDetail.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;
                    //    }
                    //    else
                    //    {
                    //        var oldRowDetailtemp = (from e in entity.TblGlChequeTransactionDetails
                    //                                where e.Iserial == variable.Iserial
                    //                                select e).SingleOrDefault();

                    //        //if (oldRowDetailtemp != null)
                    //        //{
                    //            oldRowDetailtemp.TblGlChequeStatus = GlChequesetting.ChequeLockupFilterOnUpdateStatus;
                    //        //}
                    //        //else
                    //        //{

                    //        //}
                    //    }
                    //}
                    //entity.SaveChanges();
                    if (approve)
                    {
                        var ledgerheader = entity.TblLedgerHeaders.Any(
                            x => x.TblJournalLink == newRow.Iserial && x.TblTransactionType == 7);
                        if (!ledgerheader)
                        {
                            try
                            {
                                PostTblGlChequeTransactionHeader(newRow.Iserial, user, company, newRow.Code);
                            }
                            catch (Exception)
                            {
                                var ledgerToDelete = entity.TblLedgerHeaders.Where(
                                    x => x.TblJournalLink == newRow.Iserial && x.TblTransactionType == 7).ToList();
                                foreach (var ledgerRow in ledgerToDelete)
                                {
                                    entity.TblLedgerHeaders.DeleteObject(ledgerRow);
                                }
                                throw;
                            }

                            UpdatecheckStatus(newRow, company);
                            newRow.Approved = true;
                        }
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
                            UpdateOrInsertTblGlChequeTransactionHeader(newRow, save, index, user, approve, out outindex, company);
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
        private int DeleteTblGlChequeTransactionHeader(TblGlChequeTransactionHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlChequeTransactionHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlChequeTransactionDetail> GetTblGlChequeTransactionDetail(int header, string company, out List<Entity> entityList, out List<TblContractHeader> ContractList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlChequeTransactionDetail> query;

                query = entity.TblGlChequeTransactionDetails.Include(nameof(TblGlChequeTransactionDetail.TblJournalAccountType)).Include(nameof(TblGlChequeTransactionDetail.TblJournalAccountType1))
                    .Include(nameof(TblGlChequeTransactionDetail.TblGlChequeStatu)).Include(nameof(TblGlChequeTransactionDetail.TblBankCheque1)).Where(x => x.TblGlChequeTransactionHeader == header);

                List<int?> intList = query.Select(x => x.EntityDetail1).ToList();

                intList.AddRange(query.Select(x => x.EntityDetail2).ToList());

                List<int?> intTypeList = query.Select(x => x.EntityDetail1TblJournalAccountType).ToList();

                intTypeList.AddRange(query.Select(x => x.EntityDetail2TblJournalAccountType).ToList());
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType) && x.scope == 0).ToList();
                var contractlistIserial = query.Select(x => x.TblContractHeader).Where(w => w.HasValue).ToList();
                using (var context = new WorkFlowManagerDBEntities())
                {
                    ContractList = context.TblContractHeaders.Where(x => contractlistIserial.Contains(x.Iserial)).ToList();
                }
                return query.ToList();
            }
        }


        [OperationContract]
        private List<TblContractHeader> GetTblContractHeaderForCheque(int TblJournalAccountType, int Entity, string company)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    if (TblJournalAccountType == 2 || TblJournalAccountType == 3)
                    {
                        return context.TblContractHeaders.Where(x => x.SupplierIserial == Entity && x.Approved == true).ToList();
                    }
                    return null;
                }
            }
        }

        [OperationContract]
        private int DeleteTblGlChequeTransactionDetail(TblGlChequeTransactionDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlChequeTransactionDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void PostTblGlChequeTransactionHeader(int iserial, int user, string company, string code)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.CommandTimeout = 0;
                var chequeTransactionHeader =
                    entity.TblGlChequeTransactionHeaders.Include("TblGlChequeTypeSetting1")
                        .Include("TblGlChequeTransactionDetails.TblBankCheque1.TblBank1")
                        .FirstOrDefault(x => x.Iserial == iserial);
                var journalint =
                    entity.TblJournals.FirstOrDefault(
                        x => x.Iserial == chequeTransactionHeader.TblGlChequeTypeSetting1.TblJournal).Iserial;
                var newLedgerHeaderRow = new TblLedgerHeader
                {
                    CreatedBy = chequeTransactionHeader.CreatedBy,
                    CreationDate = DateTime.Now,
                    Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted + " " + code,
                    DocDate = chequeTransactionHeader.DocDate,
                    TblJournal = journalint,
                    TblTransactionType = 7,
                    TblJournalLink = chequeTransactionHeader.Iserial
                };

                var tempheader = 0;
                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
                             company);
                var newLedgerHeaderRowIssue = new TblLedgerHeader();
                if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
                {
                    newLedgerHeaderRowIssue = new TblLedgerHeader
                    {
                        CreatedBy = chequeTransactionHeader.CreatedBy,
                        CreationDate = DateTime.Now,
                        Description = chequeTransactionHeader.TblGlChequeTypeSetting1.MsgPosted,
                        DocDate = chequeTransactionHeader.DocDate,
                        TblJournal = journalint,
                        TblTransactionType = 7,
                        TblJournalLink = chequeTransactionHeader.Iserial
                    };
                    UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRowIssue, true, 000, out tempheader, (int)chequeTransactionHeader.CreatedBy,
                         company);
                }

                if (!chequeTransactionHeader.TblGlChequeTypeSetting1.DetailSummary)
                {
                    foreach (var row in chequeTransactionHeader.TblGlChequeTransactionDetails)
                    {
                        #region إلغاء شيك

                        row.TblGlChequeStatus = 3;
                        if (chequeTransactionHeader.TblGlChequeTypeSetting == 8 || chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                        {
                            if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                            {
                                var bankcheque = entity.TblBankCheques.FirstOrDefault(w => w.Iserial == row.TblBankCheque);

                                var newCheque = new TblBankCheque()
                                {
                                    Amount = 0,
                                    Cheque = bankcheque.Cheque,
                                    TblBank = bankcheque.TblBank,
                                    TblCurrency = bankcheque.TblCurrency,
                                    TblGlChequeStatus = 4,
                                    TransDate = DateTime.Now,
                                };
                                entity.TblBankCheques.AddObject(newCheque);

                                bankcheque.Cheque = bankcheque.Cheque * -1;
                                row.ChequeNo = "-" + row.ChequeNo;
                            }

                            var recCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 4);

                            if (recCheque != null)
                            {
                                if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                                {
                                    recCheque.ChequeNo = "-" + recCheque.ChequeNo;
                                }

                                var recrow =
                                    entity.TblLedgerHeaders.FirstOrDefault(x => x.TblJournalLink == recCheque.TblGlChequeTransactionHeader && x.TblTransactionType == 7);
                                if (recrow != null)
                                {
                                    var amount = recCheque.Amount;
                                    ReverseTblledgerHeader(recrow.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
                                }
                            }

                            var recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 3);

                            if (recVendCheque != null)
                            {
                                if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                                {
                                    recVendCheque.ChequeNo = "-" + recVendCheque.ChequeNo;
                                }

                                var recrow =
                               entity.TblLedgerHeaders.Where(x => x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader && x.TblTransactionType == 7);
                                if (recrow != null)
                                {
                                    var amount = recVendCheque.Amount;
                                    foreach (var VARIABLE in recrow)
                                    {
                                        ReverseTblledgerHeader(VARIABLE.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
                                    }
                                }
                            }

                            recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 2);
                            if (recVendCheque != null)
                            {
                                if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                                {
                                    recVendCheque.ChequeNo = "-" + recVendCheque.ChequeNo;
                                }
                                var recrow =
                                      entity.TblLedgerHeaders.FirstOrDefault(
                                          x =>
                                              x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader &&
                                              x.TblTransactionType == 7);
                                if (recrow != null)
                                {
                                    var amount = recVendCheque.Amount;
                                    ReverseTblledgerHeader(recrow.Iserial, user, company, 0,
                                        newLedgerHeaderRow.Iserial, (decimal?)amount);
                                }
                            }

                            recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 13);
                            if (recVendCheque != null)
                            {
                                if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                                {
                                    recVendCheque.ChequeNo = "-" + recVendCheque.ChequeNo;
                                }
                                var recrow =
                                      entity.TblLedgerHeaders.FirstOrDefault(
                                          x =>
                                              x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader &&
                                              x.TblTransactionType == 7);
                                if (recrow != null)
                                {
                                    var amount = recVendCheque.Amount;
                                    ReverseTblledgerHeader(recrow.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
                                }
                            }

                            recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 12);
                            if (recVendCheque != null)
                            {
                                if (chequeTransactionHeader.TblGlChequeTypeSetting == 14)
                                {
                                    recVendCheque.ChequeNo = "-" + recVendCheque.ChequeNo;
                                }
                                var recrow =
                                      entity.TblLedgerHeaders.FirstOrDefault(
                                          x =>
                                              x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader &&
                                              x.TblTransactionType == 7);
                                if (recrow != null)
                                {
                                    var amount = recVendCheque.Amount;
                                    ReverseTblledgerHeader(recrow.Iserial, user, company, 0,
                                        newLedgerHeaderRow.Iserial, (decimal?)amount);
                                }
                            }
                        }

                        #endregion إلغاء شيك

                        #region تسليم اوراق دفع من الخزنة للمورد

                        if (chequeTransactionHeader.TblGlChequeTypeSetting == 3)
                        {
                            row.TblGlChequeStatus = 2;
                            var newledgerDetailrowh1 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
                                EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
                                GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };
                            var accountDetail1 =
                                entity.Entities.FirstOrDefault(
                                    x =>
                                        x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType &&
                                        x.Iserial == row.EntityDetail1).AccountIserial;

                            var newledgerDetailrowd1 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),

                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = row.EntityDetail1TblJournalAccountType,
                                EntityAccount = row.EntityDetail1,
                                GlAccount = accountDetail1,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };
                            var glchequesetting = entity.TblGlChequeTypeSettings.FirstOrDefault(w => w.Iserial == 3);
                            var newledgerDetailrowh2 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = chequeTransactionHeader.EntityHeader2TblJournalAccountType,
                                EntityAccount = glchequesetting.DefaultHeaderEntity2,
                                GlAccount = (int)glchequesetting.DefaultHeaderEntity2,
                                TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = chequeTransactionHeader.EntityHeader2PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };

                            var newledgerDetailrowd2 = new TblLedgerMainDetail();
                            if (row.DueDate <= chequeTransactionHeader.DocDate.Value.AddDays(chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays)
                                ||
                                (chequeTransactionHeader.TblGlChequeTypeSetting1.TransferChequeToBankBeforeDays == 0
                                && chequeTransactionHeader.DocDate.Value.Month == row.DueDate.Value.Month &&
                                chequeTransactionHeader.DocDate.Value.Year == row.DueDate.Value.Year))
                            {
                                var bank = row.TblBankCheque1.TblBank;
                                var bankAccount =
                              entity.Entities.FirstOrDefault(
                                  x =>
                                      x.TblJournalAccountType == 6 &&
                                      x.Iserial == bank && x.scope == 0).AccountIserial;

                                newledgerDetailrowd2 = new TblLedgerMainDetail
                                {
                                    Amount = (decimal?)row.Amount,
                                    Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                    TblCurrency = chequeTransactionHeader.TblCurrency,
                                    TransDate = chequeTransactionHeader.DocDate,
                                    TblJournalAccountType = 6,
                                    EntityAccount = bank,
                                    GlAccount = bankAccount,
                                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                    PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                    DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
                                    TblBankCheque = row.TblBankCheque,
                                };
                            }
                            else
                            {
                                row.TblGlChequeStatus = 1;
                                newledgerDetailrowd2 = new TblLedgerMainDetail
                                {
                                    Amount = (decimal?)row.Amount,
                                    Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                    ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                    TblCurrency = chequeTransactionHeader.TblCurrency,
                                    TransDate = chequeTransactionHeader.DocDate,
                                    TblJournalAccountType = 0,
                                    EntityAccount = row.EntityDetail2TblAccount,
                                    GlAccount = (int)row.EntityDetail2TblAccount,
                                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                    PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                    DrOrCr = !chequeTransactionHeader.EntityHeader2PostDr,
                                    TblBankCheque = row.TblBankCheque,
                                };
                            }
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
                    (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd1, true, 000, out tempheader, company,
                         (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh2, true, 000, out tempheader, company,
                        (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
                         (int)chequeTransactionHeader.CreatedBy);
                        }

                        #endregion تسليم اوراق دفع من الخزنة للمورد

                        #region تسليم اوراق قبض

                        if (chequeTransactionHeader.TblGlChequeTypeSetting == 13)
                        {
                            var glchequesetting = entity.TblGlChequeTypeSettings.FirstOrDefault(w => w.Iserial == 13);

                            row.TblGlChequeStatus = 2;
                            var newledgerDetailrowh1 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque,
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 0,
                                EntityAccount = glchequesetting.EntityDetail1TblAccount,
                                GlAccount = (int)glchequesetting.EntityDetail1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };
                            var accountDetail1 =
                                entity.Entities.FirstOrDefault(
                                    x =>
                                        x.TblJournalAccountType == row.EntityDetail1TblJournalAccountType &&
                                        x.Iserial == row.EntityDetail1).AccountIserial;

                            var newledgerDetailrowd1 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),

                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = row.EntityDetail1TblJournalAccountType,
                                EntityAccount = row.EntityDetail1,
                                GlAccount = accountDetail1,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };


                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
                    (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd1, true, 000, out tempheader, company,
                         (int)chequeTransactionHeader.CreatedBy);
                        }

                        #endregion تسليم اوراق دفع من الخزنة للمورد

                        #region تسليم اوراق دفع للبنك

                        else if (chequeTransactionHeader.TblGlChequeTypeSetting == 4)
                        {
                            row.TblGlChequeStatus = 2;
                            var newledgerDetailrowh1 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
                                EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
                                GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                                TblBankCheque = row.TblBankCheque,
                            };

                            var bank = row.TblBankCheque1.TblBank;
                            var bankAccount =
                          entity.Entities.FirstOrDefault(
                              x =>
                                  x.TblJournalAccountType == 6 &&
                                  x.Iserial == bank && x.scope == 0).AccountIserial;

                            var newledgerDetailrowd2 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Amount,
                                Description = row.Description + " " + row.TblBankCheque1.Cheque.ToString(),
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 6,
                                EntityAccount = bank,
                                GlAccount = bankAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = row.TblBankCheque1.Cheque.ToString(),
                                TblBankCheque = row.TblBankCheque,
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
                     (int)chequeTransactionHeader.CreatedBy);
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowd2, true, 000, out tempheader, company,
                     (int)chequeTransactionHeader.CreatedBy);
                        }

                        #endregion تسليم اوراق دفع للبنك
                    }
                }
                else
                {
                    if (chequeTransactionHeader.TblGlChequeTypeSetting == 12)
                    {
                        #region استلام اوراق قبض

                        foreach (
                            var row in
                                chequeTransactionHeader.TblGlChequeTransactionDetails.GroupBy(
                                    x => x.TblGlChequeTransactionHeader1.EntityHeader1TblAccount))
                        {
                            var newledgerDetailrow = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
                                EntityAccount = chequeTransactionHeader.EntityHeader1,
                                GlAccount =
                                    entity.Entities.FirstOrDefault(
                                        w =>
                                            w.TblJournalAccountType ==
                                            chequeTransactionHeader.EntityHeader1TblJournalAccountType &&
                                            w.Iserial == chequeTransactionHeader.EntityHeader1).AccountIserial,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                            };

                            var newledgerDetailrow2 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 0,
                                EntityAccount = row.FirstOrDefault().EntityDetail1TblAccount,
                                GlAccount = (int)row.FirstOrDefault().EntityDetail1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out tempheader, company,
                                (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow2, true, 000, out tempheader, company,
                                (int)chequeTransactionHeader.CreatedBy);
                        }

                        #endregion استلام اوراق قبض
                    }
                    else if (chequeTransactionHeader.TblGlChequeTypeSetting == 9) {
                        foreach (var row in chequeTransactionHeader.TblGlChequeTransactionDetails.GroupBy(x => x.TblGlChequeTransactionHeader1.EntityHeader1))
                        {
                            var newledgerDetailrow = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = chequeTransactionHeader.EntityHeader1TblJournalAccountType,
                                EntityAccount = chequeTransactionHeader.EntityHeader1,
                                GlAccount = (int)chequeTransactionHeader.EntityHeader1,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                            };

                            var newledgerDetailrow2 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 0,
                                EntityAccount = row.FirstOrDefault().EntityDetail1TblAccount,
                                GlAccount = (int)row.FirstOrDefault().EntityDetail1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out tempheader, company,
                    (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow2, true, 000, out tempheader, company,
                         (int)chequeTransactionHeader.CreatedBy);
                        }
                    }
                    else
                    {
                        #region طلب اصدار شيكات

                        foreach (var row in chequeTransactionHeader.TblGlChequeTransactionDetails.GroupBy(x => x.TblGlChequeTransactionHeader1.EntityHeader1TblAccount))
                        {
                            var newledgerDetailrow = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 0,
                                EntityAccount = chequeTransactionHeader.EntityHeader1TblAccount,
                                GlAccount = (int)chequeTransactionHeader.EntityHeader1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
                            };

                            var newledgerDetailrow2 = new TblLedgerMainDetail
                            {
                                Amount = (decimal?)row.Sum(x => x.Amount),
                                Description = "Cheque Transaction",
                                ExchangeRate = chequeTransactionHeader.ExchangeRate,
                                TblCurrency = chequeTransactionHeader.TblCurrency,
                                TransDate = chequeTransactionHeader.DocDate,
                                TblJournalAccountType = 0,
                                EntityAccount = row.FirstOrDefault().EntityDetail1TblAccount,
                                GlAccount = (int)row.FirstOrDefault().EntityDetail1TblAccount,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = !chequeTransactionHeader.EntityHeader1PostDr,
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out tempheader, company,
                    (int)chequeTransactionHeader.CreatedBy);

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow2, true, 000, out tempheader, company,
                         (int)chequeTransactionHeader.CreatedBy);
                        }

                        #endregion طلب اصدار شيكات
                    }
                }
                chequeTransactionHeader.ApproveDate = DateTime.Now;
                chequeTransactionHeader.ApprovedBy = user;
                chequeTransactionHeader.Approved = true;
                entity.SaveChanges();
            }
        }

        //[OperationContract]
        //private void PostTblGlChequeTransactionHeaderDesc(int iserial, int user, string company)
        //{
        //    using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
        //    {
        //        var chequeTransactionHeader =
        //            entity.TblGlChequeTransactionHeaders.Include("TblGlChequeTypeSetting1")
        //                .Include("TblGlChequeTransactionDetails.TblBankCheque1.TblBank1")
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
        //            foreach (var row in chequeTransactionHeader.TblGlChequeTransactionDetails)
        //            {
        //                #region إلغاء شيك

        //                row.TblGlChequeStatus = 3;
        //                if (chequeTransactionHeader.TblGlChequeTypeSetting == 8||chequeTransactionHeader.TblGlChequeTypeSetting == 14)
        //                {
        //                    var recCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 4);

        //                    if (recCheque != null)
        //                    {
        //                        var recrow =
        //                            entity.TblLedgerHeaders.FirstOrDefault(x => x.TblJournalLink == recCheque.TblGlChequeTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recCheque.Amount;
        //                            ReverseTblledgerHeader(recrow.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                        }
        //                    }

        //                    var recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 3);

        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                       entity.TblLedgerHeaders.Where(x => x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader && x.TblTransactionType == 7);
        //                        if (recrow != null)
        //                        {
        //                            var amount = recVendCheque.Amount;
        //                            foreach (var VARIABLE in recrow)
        //                            {
        //                                ReverseTblledgerHeader(VARIABLE.Iserial, user, company, (int)row.TblBankCheque, newLedgerHeaderRow.Iserial, (decimal?)amount);
        //                            }
        //                        }
        //                    }

        //                    recVendCheque = entity.TblGlChequeTransactionDetails.Include("TblGlChequeTransactionHeader1.TblCurrency1").FirstOrDefault(x => x.TblBankCheque == row.TblBankCheque && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == 2);
        //                    if (recVendCheque != null)
        //                    {
        //                        var recrow =
        //                              entity.TblLedgerHeaders.FirstOrDefault(
        //                                  x =>
        //                                      x.TblJournalLink == recVendCheque.TblGlChequeTransactionHeader &&
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
        //                        PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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
        //                        PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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
        //                        EntityAccount = chequeTransactionHeader.EntityHeader2TblAccount,
        //                        GlAccount = (int)chequeTransactionHeader.EntityHeader2TblAccount,
        //                        TblLedgerHeader = newLedgerHeaderRowIssue.Iserial,
        //                        PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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
        //                              x.Iserial == bank && x.scope == 0).AccountIserial;

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
        //                            PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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
        //                            PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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
        //                        PaymentRef = row.TblBankCheque1.Cheque.ToString(),
        //                        DrOrCr = chequeTransactionHeader.EntityHeader1PostDr,
        //                        TblBankCheque = row.TblBankCheque,
        //                    };

        //                    var bank = row.TblBankCheque1.TblBank;
        //                    var bankAccount =
        //                  entity.Entities.FirstOrDefault(
        //                      x =>
        //                          x.TblJournalAccountType == 6 &&
        //                          x.Iserial == bank && x.scope == 0).AccountIserial;

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
        //                        PaymentRef = row.TblBankCheque1.Cheque.ToString(),
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

        //            foreach (var row in chequeTransactionHeader.TblGlChequeTransactionDetails.GroupBy(x => x.TblGlChequeTransactionHeader1.EntityHeader1TblAccount))
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

        [OperationContract]
        private List<TblGlChequeTransactionDetail> GetLockupFromPreTransaction(TblGlChequeTypeSetting setting, int tbljournalaccounttype, int vendor, int bank, string code, DateTime? FromDate, DateTime? ToDate, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlChequeTransactionDetail> query;
                query = entity.TblGlChequeTransactionDetails.Include("TblJournalAccountType").Include("TblJournalAccountType1").Include("TblGlChequeStatu").Include("TblBankCheque1").Where(x => (x.EntityDetail1 == vendor || vendor == 0)
                    && (x.TblBankCheque1.TblBank == bank || bank == 0)
                    && (x.EntityDetail1TblJournalAccountType == tbljournalaccounttype || tbljournalaccounttype == -1)
                    && (x.TblGlChequeTransactionHeader1.Code == code || code == null)
                    && (x.DueDate >= FromDate || FromDate == null)
                    && (x.DueDate <= ToDate || ToDate == null)
                    && x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting == setting.ChequeLockupFilterOnChequeType && x.TblBankCheque1.TblGlChequeStatus == setting.ChequeLockupFilterOnChequeStatus);

                List<int?> intList = query.Select(x => x.EntityDetail1).ToList();
                intList.AddRange(query.Select(x => x.EntityDetail2).ToList());
                List<int?> intTypeList = query.Select(x => x.EntityDetail1TblJournalAccountType).ToList();
                intTypeList.AddRange(query.Select(x => x.EntityDetail2TblJournalAccountType).ToList());
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType) && x.scope == 0).ToList();
                return query.ToList();
            }
        }
    }
}