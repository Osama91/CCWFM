using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using Omu.ValueInjecter;
using System.Data.SqlClient;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        public TblLedgerHeader PrintLedgerHeader(int iserial, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.SpFixGlBalanced();
                return entity.TblLedgerHeaders.FirstOrDefault(w => w.Iserial == iserial);
            }
        }


        [OperationContract]
        public TblLedgerMainDetail PostTotalLedger(int iserial, int user, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var ledgerheader = entity.TblLedgerHeaders.FirstOrDefault(w => w.Iserial == iserial);

                ledgerheader.balanced = true;
                var TblLedgerMainDetails = entity.TblLedgerMainDetails.Include("TblLedgerHeader1.TblJournal1").Where(w => w.TblLedgerHeader == iserial).ToList();

                var LedgerDetailRecord = TblLedgerMainDetails.FirstOrDefault();
                var Journal = TblLedgerMainDetails.FirstOrDefault().TblLedgerHeader1.TblJournal1;
                var entityAccount = Journal.Entity;
                var journalType = Journal.TblJournalAccountType;

                var SumDr = TblLedgerMainDetails.Where(w => w.DrOrCr == true).Sum(w => w.Amount);

                var SumCr = TblLedgerMainDetails.Where(w => w.DrOrCr == false).Sum(w => w.Amount);
                var Entity = entity.Entities.FirstOrDefault(w => w.Iserial == entityAccount && w.TblJournalAccountType == journalType);
                decimal total = (SumDr - SumCr) ?? 0;
                var amount = Math.Abs(total);
                var newLedgerMainDetail = new TblLedgerMainDetail()
                {
                    TblLedgerHeader = iserial,
                    TblJournalAccountType = journalType,
                    EntityAccount = entityAccount,
                    TransDate = LedgerDetailRecord.TransDate,
                    TblCurrency = LedgerDetailRecord.TblCurrency,
                    Amount = amount,
                    GlAccount = Entity.AccountIserial,
                    ExchangeRate = LedgerDetailRecord.ExchangeRate,
                };
                if ((SumDr - SumCr) > 0)
                {
                    //cr
                    newLedgerMainDetail.DrOrCr = false;
                }
                else
                {
                    //dr
                    newLedgerMainDetail.DrOrCr = true;
                }
                int temp = 0;
                UpdateOrInsertTblLedgerMainDetails(entity, newLedgerMainDetail, true, 0, out temp, user);
                return newLedgerMainDetail;
            }
        }
        [OperationContract]
        public void CorrectLedgerHeaderRouding(int tblledgerheader, string company, int user)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var ledgerHeader = entity.TblLedgerHeaders.Include("TblLedgerMainDetails").FirstOrDefault(x => x.Iserial == tblledgerheader);

                var totaldebit = ledgerHeader.TblLedgerMainDetails.Where(x => x.DrOrCr == true && x.OffsetEntityAccount == null).Sum(x => x.Amount);

                var totalCredit = ledgerHeader.TblLedgerMainDetails.Where(x => x.DrOrCr == false && x.OffsetEntityAccount == null).Sum(x => x.Amount);

                if (totaldebit - totalCredit != 0)
                {
                    int temp = 0;
                    if (totaldebit > totalCredit)
                    {
                        var tblledgerdetail =
                            ledgerHeader.TblLedgerMainDetails.LastOrDefault(
                                x => x.DrOrCr == false && x.OffsetEntityAccount == null);
                        tblledgerdetail.Amount = tblledgerdetail.Amount + totaldebit - totalCredit;
                        UpdateOrInsertTblLedgerMainDetails(tblledgerdetail, false, 00, out temp, company, user);
                    }

                    if (totaldebit < totalCredit)
                    {
                        var tblledgerdetail =
                            ledgerHeader.TblLedgerMainDetails.LastOrDefault(
                                x => x.DrOrCr == true && x.OffsetEntityAccount == null);
                        tblledgerdetail.Amount = tblledgerdetail.Amount + totalCredit - totaldebit;
                        UpdateOrInsertTblLedgerMainDetails(tblledgerdetail, false, 00, out temp, company, user);
                    }
                }
            }
        }

        [OperationContract]
        private List<TblLedgerHeader> GetTblLedgerHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, int user, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {

                var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);

                var JournalList = new List<int>();

                if (SettingExist)
                {

                    JournalList = entity.TblJournalSettingEntities.Where(w =>
     w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user)).Select(w => w.TblJournal ?? 0).Distinct().ToList();
                }
                IQueryable<TblLedgerHeader> query;
                if (filter != null)
                {
                    if (JournalList.Any())
                    {
                        foreach (var variable in JournalList)
                        {
                            filter = filter + " and it.tbljournal ==(@Iserial" + variable + ")";
                            valuesObjects.Add("Iserial" + variable, variable);
                        }
                    }
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblLedgerHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblLedgerHeaders.Include("TblJournal1.TblCurrency1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    if (JournalList.Any())
                    {
                        fullCount = entity.TblLedgerHeaders.Count(w => JournalList.Contains(w.TblJournal));
                        query = entity.TblLedgerHeaders.Include("TblJournal1.TblCurrency1").OrderBy(sort).Where(w => JournalList.Contains(w.TblJournal)).Skip(skip).Take(take);
                        //query = query.Where(w => JournalList.Contains(w.TblJournal));
                    }
                    else
                    {
                        fullCount = entity.TblLedgerHeaders.Count();
                        query = entity.TblLedgerHeaders.Include("TblJournal1.TblCurrency1").OrderBy(sort).Skip(skip).Take(take);
                    }


                }
                query = query.OrderByDescending(w => w.DocDate);

                return query.ToList();
            }
        }

        //public bool Find(string tableName, string code, string company)
        //{
        //    using (var context = new ccnewEntities(GetSqlConnectionString(company)))
        //    {
        //        return Find(tableName, code, context);
        //    }
        //}

        private static bool Find(string tableName, string code, ccnewEntities context)
        {
            var query = "SELECT Code FROM dbo." + tableName + " Where Code ={0}";

            var result = context.ExecuteStoreQuery<string>(query, code);  //<>("");
            if (result.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private string HandelSequence(string code, TblJournal journal, string table, string company, int no, int month, int year, out int seqq)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return HandelSequence(code, journal, table, no, month, year, entity, out seqq);
            }
        }

        private string HandelSequence(string code, TblJournal journal, string table, int no, int month, int year, ccnewEntities entity, out int seqq)
        {
            seqq = 0;
            var temp = "";
            var tempFormat = "";
            const char aa = '0';
            if (table == "TblLedgerHeader")
            {
                if (journal.TblSequence.UseDateTime)
                {
                    for (var i = 0; i < journal.TblSequence.NumberOfInt; i++)
                    {
                        tempFormat = tempFormat + aa;
                    }
                    var seq = 0;
                    try
                    {
                        var tblLedgerHeader = entity.TblLedgerHeaders.Where(
                            x => x.TblSequence == journal.HeaderSequence && x.DocDate.Value.Month == month && x.DocDate.Value.Year == year).OrderByDescending(x => x.Sequence).FirstOrDefault();
                        if (tblLedgerHeader !=
                            null)
                            seq = tblLedgerHeader.Sequence;
                    }
                    catch (Exception)
                    {
                    }

                    seqq = seq + 1;
                    temp = seqq.ToString(tempFormat) + month.ToString("00") + year.ToString().Substring(2, 2) + journal.TblSequence.Format;
                }
                else
                {
                    if (journal.TblSequence.Manual)
                    {
                        if (Find(table, code, entity))
                        {
                            throw new Exception("Already Exists");
                        }
                    }
                    else
                    {
                        for (var i = 0; i < journal.TblSequence.NumberOfInt; i++)
                        {
                            tempFormat = tempFormat + aa;
                        }
                        temp = journal.TblSequence.NextRec.ToString(tempFormat) + journal.TblSequence.Format;
                        var seq = entity.TblSequences.FirstOrDefault(x => x.Iserial == journal.HeaderSequence);

                        if (seq != null) seq.NextRec = seq.NextRec + 1;
                    }
                }
            }
            else
            {
                if (journal.TblSequence1.Manual)
                {
                    if (Find(table, code, entity))
                    {
                        throw new Exception("Already Exists");
                    }
                }
                else
                {
                    for (var i = 0; i < journal.TblSequence1.NumberOfInt; i++)
                    {
                        tempFormat = tempFormat + aa;
                    }
                    var tempno = journal.TblSequence1.NextRec + no;
                    temp = tempno.ToString(tempFormat) + journal.TblSequence1.Format;
                    var seq = entity.TblSequences.FirstOrDefault(x => x.Iserial == journal.DetailSequence);

                    if (seq != null) seq.NextRec = seq.NextRec + 1 + no;
                }
            }
            try
            {
                entity.SaveChanges();
            }
            catch (Exception e ){ throw new Exception(e.Message); }
            
            return temp;
        }
        [OperationContract]
        public TblLedgerHeader UpdateOrInsertTblLedgerHeaders(TblLedgerHeader newRow, bool save, int index, out int outindex, int user, string company, bool validate = false)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return UpdateOrInsertTblLedgerHeaders(entity, newRow, save, index, out outindex, user, validate);
            }
        }


        public TblLedgerHeader UpdateOrInsertTblLedgerHeaders(ccnewEntities entity, TblLedgerHeader newRow, bool save, int index, out int outindex, int user, bool validate = false)
        {
            outindex = index;
            if (validate)
            {
                ValidateJournalSetting(entity, newRow, user);
            }
            entity.CommandTimeout = 0;

            if (save)
            {
                try
                {
                    var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == newRow.TblJournal);
                    int seq = 0;
                    //newRow.Code = "00011216J";
                    newRow.Code = HandelSequence(newRow.Code, journal, "TblLedgerHeader", 0, newRow.DocDate.Value.Month, newRow.DocDate.Value.Year, entity, out seq);
                    newRow.balanced = true;
                    newRow.Sequence = seq;
                    newRow.TblSequence = journal.HeaderSequence;
                    newRow.CreatedBy = user;
                    newRow.CreationDate = DateTime.Now;
                    entity.TblLedgerHeaders.AddObject(newRow);

                } catch (Exception EX ) {
                    string MSG = EX.Message;
                }
               
            }
            else
            {
                var oldRow = (from e in entity.TblLedgerHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    if (newRow.DocDate.Value.Month != oldRow.DocDate.Value.Month && newRow.DocDate.Value.Year != oldRow.DocDate.Value.Year)
                    {
                        var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == newRow.TblJournal);
                        int seq = 0;

                        newRow.Code = HandelSequence(newRow.Code, journal, "TblLedgerHeader", 0, newRow.DocDate.Value.Month, newRow.DocDate.Value.Year, entity, out seq);
                        newRow.Sequence = seq;
                    }
                    newRow.CreatedBy = oldRow.CreatedBy;
                    newRow.CreationDate = oldRow.CreationDate;
                    GenericUpdate(oldRow, newRow, entity);
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
                    //entity.Detach(newRow);
                    // UpdateOrInsertTblLedgerHeaders(newRow, save, index, out outindex, user, company);
                }
                else
                {
                    throw ex;
                }

            }

            return newRow;

        }


        private bool ExceptionContainsErrorCode(Exception e, int ErrorCode)
        {
            SqlException winEx = e as SqlException;
            if (winEx != null && ErrorCode == winEx.Number)
                return true;

            if (e.InnerException != null)
                return ExceptionContainsErrorCode(e.InnerException, ErrorCode);

            return false;
        }

        [OperationContract]
        private int DeleteTblLedgerHeader(TblLedgerHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        private void InsertIntoLedgerdetail(TblLedgerMainDetail row, ccnewEntities entity)
        {
            var query = (from e in entity.TblLedgerDetails
                         where e.TblLedgerMainDetail == row.Iserial
                         select e).ToList();
            foreach (var variable in query)
            {
                entity.DeleteObject(variable);
            }
            if (row.OffsetEntityAccount != null)
            {
                var newrow = new TblLedgerDetail();
                newrow.InjectFrom(row);
                newrow.DrOrCr = !row.DrOrCr;
                newrow.TblJournalAccountType = row.OffsetAccountType;
                newrow.EntityAccount = row.OffsetEntityAccount;
                newrow.GlAccount = row.OffsetGlAccount;

                newrow.EntityKey = null;
                newrow.EntityRef = null;

                newrow.OffsetAccountType = row.TblJournalAccountType;
                newrow.OffsetEntityAccount = row.EntityAccount;
                newrow.OffsetGlAccount = row.GlAccount;
                newrow.TblLedgerMainDetail = row.Iserial;
                newrow.Iserial = 0;
                entity.TblLedgerDetails.AddObject(newrow);
            }
            var newRow2 = new TblLedgerDetail();
            newRow2.InjectFrom(row);

            newRow2.EntityKey = null;
            newRow2.EntityRef = null;

            newRow2.TblLedgerMainDetail = row.Iserial;
            newRow2.Iserial = 0;
            entity.TblLedgerDetails.AddObject(newRow2);
            entity.SaveChanges();
        }

        private void UpdateCheckFromTransaction(TblLedgerMainDetail row, ccnewEntities entity)
        {

            var query = (from e in entity.TblLedgerMainDetails.Include("TblBankCheque1")
                         where e.Iserial == row.Iserial
                         select e).FirstOrDefault();

            if (query != null && query.TblBankCheque != null && query.TblJournalAccountType != 6 && query.TblJournalAccountType != 0)
            {
                var tblBankCheque1 = (from e in entity.TblBankCheques.Include("TblBankChequeCostCenters")
                                      where e.Iserial == row.TblBankCheque
                                      select e).FirstOrDefault();

                if (tblBankCheque1 != null)
                {
                    if (query.Amount != null) tblBankCheque1.Amount = (double)query.Amount;
                    tblBankCheque1.TblJournalAccountType = query.TblJournalAccountType;
                    tblBankCheque1.EntityAccount = query.EntityAccount;

                    if (tblBankCheque1.TblBankChequeCostCenters != null)
                    {
                        foreach (var variable in tblBankCheque1.TblBankChequeCostCenters)
                        {
                            var costcenterExsist =
                                entity.TblLedgerDetailCostCenters.Any(x => x.TblLedgerMainDetail == query.Iserial);
                            if (!costcenterExsist)
                            {
                                var costCenterrow = new TblLedgerDetailCostCenter
                                {
                                    Ratio = variable.Ratio,
                                    Amount = variable.Amount,
                                    TblCostCenter = variable.TblCostCenter,
                                    TblCostCenterType = variable.TblCostCenterType,
                                    TblLedgerMainDetail = row.Iserial
                                };
                                entity.AddToTblLedgerDetailCostCenters(costCenterrow);
                            }
                        }
                    }
                }

                entity.SaveChanges();
            }
        }

        private void UpdatecheckStatus(TblGlChequeTransactionHeader row, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRowDetailtemp = (from e in entity.TblGlChequeTransactionDetails
                                        where e.TblGlChequeTransactionHeader == row.Iserial
                                        select e).ToList();

                foreach (var variable in oldRowDetailtemp)
                {
                    var oldRowDetail = (from e in entity.TblBankCheques
                                        where e.Iserial == variable.TblBankCheque
                                        select e).SingleOrDefault();
                    oldRowDetail.PayTo = variable.PayTo;
                    oldRowDetail.TblGlChequeStatus = (int)variable.TblGlChequeStatus;
                }

                entity.SaveChanges();
            }
        }

        [OperationContract]
        private List<TblLedgerMainDetail> GetTblLedgerMainDetail(int skip, int take, int ledgerHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList, out Dictionary<int, bool> TransactionExist)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblLedgerMainDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLedgerHeader ==(@Group0)";
                    valuesObjects.Add("Group0", ledgerHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblLedgerMainDetails.Where(filter, parameterCollection).Count();
                    query = entity.TblLedgerMainDetails.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblLedgerMainDetails.Count(v => v.TblLedgerHeader == ledgerHeader);
                    query = entity.TblLedgerMainDetails.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(v => v.TblLedgerHeader == ledgerHeader).Skip(skip).Take(take);
                }
                //query = query.OrderByDescending(x => x.TransDate);
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

        [OperationContract]
        public TblLedgerMainDetail UpdateOrInsertTblLedgerMainDetails(TblLedgerMainDetail newRow, bool save, int index, out int outindex, string company, int user, bool validate = false)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return UpdateOrInsertTblLedgerMainDetails(entity, newRow, save, index, out outindex, user, validate);
            }
        }
        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerDetailCostCenter newRow, int user)
        {
            var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);
            if (SettingExist)
            {
                var journal = entity.TblLedgerMainDetails.Include("TblLedgerHeader1").FirstOrDefault(w => w.Iserial == newRow.TblLedgerMainDetail).TblLedgerHeader1.TblJournal;
                var Settings = entity.TblJournalSettingCostCenters.Any(w => w.TblCostCenter == newRow.TblCostCenter &&
                 w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user));
                if (!Settings)
                {
                    throw new FaultException("You are not authorized to use this Cost Center");
                }
            }

        }

        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerMainDetail newRow, int user)
        {

            var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);
            if (SettingExist)
            {

                var entityRow = entity.Entities.FirstOrDefault(w => w.TblJournalAccountType == newRow.TblJournalAccountType && w.Iserial == newRow.EntityAccount && w.scope == 0);
                var journal = entity.TblLedgerHeaders.FirstOrDefault(w => w.Iserial == newRow.TblLedgerHeader).TblJournal;
                var Settings = entity.TblJournalSettingEntities.Any(w => (w.TblJournalAccountType == newRow.TblJournalAccountType|| w.TblJournalAccountType==null)
                && (
                (w.EntityAccount == newRow.EntityAccount && w.Scope == 0) || (w.Scope == 2) ||
                (w.Scope == 1 && w.EntityAccount == entityRow.GroupIserial))
                 && w.TblJournal == journal
                 &&
                 w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user));
                if (!Settings)
                {
                    throw new FaultException("You are not authorized to use this account");
                }
            }

        }
        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerHeader newRow, int user)
        {
            var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);
            if (SettingExist)
            {
                var journal = newRow.TblJournal;
                var Settings = entity.TblJournalSettingEntities.Any(w => w.TblJournal == journal
                 &&
                 w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user));
                if (!Settings)
                {
                    throw new FaultException("You are not authorized to use this Journal");
                }
            }
        }

        public TblLedgerMainDetail UpdateOrInsertTblLedgerMainDetails(ccnewEntities entity, TblLedgerMainDetail newRow, bool save, int index, out int outindex, int user, bool validate = false)
        {
            outindex = index;

            if (validate)
            {
                ValidateJournalSetting(entity, newRow, user);
            }

            if (save)
            {
                var tblLedgerHeader = entity.TblLedgerHeaders.FirstOrDefault(x => x.Iserial == newRow.TblLedgerHeader);
                if (tblLedgerHeader != null)
                {
                    var ledgerheader = tblLedgerHeader.TblJournal;
                    var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == ledgerheader);
                    int temp = 0;
                    newRow.Code = HandelSequence(newRow.Code, journal, "TblLedgerMainDetail", 0, 0, 0, entity, out temp);
                }
                entity.TblLedgerMainDetails.AddObject(newRow);
            }
            else
            {
                var oldRow = (from e in entity.TblLedgerMainDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var costCenters = (from e in entity.TblLedgerDetailCostCenters
                                       where e.TblLedgerMainDetail == newRow.Iserial
                                       select e);
                    if (newRow.Amount != oldRow.Amount)
                    {
                        // More than 1 Cost Center And Not Calculated   15/3/2017
                        if (costCenters.Count() > 0)
                        {
                            var listToUpdate = costCenters.ToList();
                            foreach (var item in listToUpdate)
                            {
                                if (oldRow.Amount == 0)
                                {
                                    oldRow.Amount = (decimal)item.Amount;
                                }
                                item.Amount = (double)((Convert.ToDecimal(item.Amount) / oldRow.Amount) * newRow.Amount);
                            }
                        }
                        //if (costCenters.Count(w => w.Calculated != true) == 1)
                        //{
                        //    costCenters.FirstOrDefault().Amount = (double)newRow.Amount;
                        //}

                        //foreach (var item in costCenters.Where(w => w.Calculated == true).ToList())
                        //{
                        //    entity.TblLedgerDetailCostCenters.DeleteObject(item);
                        //}
                    }
                    GenericUpdate(oldRow, newRow, entity);
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
                }
                else
                {
                    throw ex;
                }
            }
            InsertIntoLedgerdetail(newRow, entity);
            UpdateCheckFromTransaction(newRow, entity);
            return newRow;
        }



        private void ReverseTblledgerHeader(int iserial, int user, string company, int tblBankCheque, int newHeaderIserial, decimal? amount = 0)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {

                var query = entity.TblLedgerHeaders.FirstOrDefault(x => x.Iserial == iserial);
                int temp;
                foreach (var variable in entity.TblLedgerMainDetails.Where(x => x.TblLedgerHeader == query.Iserial && (x.TblBankCheque == tblBankCheque || tblBankCheque == 0)).ToList())
                {
                    variable.DrOrCr = !variable.DrOrCr;
                    variable.TblLedgerHeader = newHeaderIserial;
                    if (amount > 0)
                    {
                        variable.Amount = amount;
                    }
                    var row = new TblLedgerMainDetail()
                    {
                        Amount = variable.Amount,
                        PaymentRef = variable.PaymentRef,
                        TransDate = variable.TransDate,
                        EntityAccount = variable.EntityAccount,
                        TblBankCheque = variable.TblBankCheque,
                        GlAccount = variable.GlAccount,
                        Description = variable.Description,
                        DueDate = variable.DueDate,
                        DrOrCr = variable.DrOrCr,
                        ExchangeRate = variable.ExchangeRate,
                        TblCurrency = variable.TblCurrency,
                        TblJournalAccountType = variable.TblJournalAccountType,
                        TblLedgerHeader = newHeaderIserial
                    };

                    UpdateOrInsertTblLedgerMainDetails(row, true, 0, out temp, company, user);
                }
            }
        }

        [OperationContract]
        private int DeleteTblLedgerMainDetail(TblLedgerMainDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerMainDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblLedgerDetailCostCenter> GetTblLedgerDetailCostCenter(int skip, int take, int ledgerHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblLedgerDetailCostCenter> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLedgerDetail ==(@Group0)";
                    valuesObjects.Add("Group0", ledgerHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblLedgerDetailCostCenters.Where(filter, parameterCollection).Count();
                    query = entity.TblLedgerDetailCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblLedgerDetailCostCenters.Count(v => v.TblLedgerMainDetail == ledgerHeader);
                    query = entity.TblLedgerDetailCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(v => v.TblLedgerMainDetail == ledgerHeader).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public TblLedgerDetailCostCenter UpdateOrInsertTblLedgerDetailCostCenters(TblLedgerDetailCostCenter newRow, bool save, int index, out int outindex, int user, string company, bool validate = false)
        {


            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (validate)
                {
                    ValidateJournalSetting(entity, newRow, user);
                }
                if (save)
                {
                    entity.TblLedgerDetailCostCenters.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblLedgerDetailCostCenters
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }


        [OperationContract]
        private int DeleteTblLedgerDetailCostCenter(TblLedgerDetailCostCenter row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerDetailCostCenters
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int PostGlLedgerHeader(int iserial, int user, string company, bool postOrApprove)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var header = entity.TblLedgerHeaders.Include("TblLedgerMainDetails.TblLedgerDetails")
                     .FirstOrDefault(x => x.Iserial == iserial);
                var detailsdr = header.TblLedgerMainDetails.Sum(x => x.TblLedgerDetails.Where(e => e.DrOrCr == true).Sum(w => w.Amount));

                var detailscr = header.TblLedgerMainDetails.Sum(x => x.TblLedgerDetails.Where(e => e.DrOrCr == false).Sum(w => w.Amount));

                if (detailsdr == detailscr)
                {
                    if (postOrApprove)
                    {
                        header.PostDate = DateTime.Now;
                        header.PostBy = user;
                        header.Posted = true;
                    }
                    else
                    {
                        header.ApproveDate = DateTime.Now;
                        header.ApprovedBy = user;
                        header.Approved = true;
                    }
                    entity.SaveChanges();
                    return 0;
                }
                return 1;
            }
        }

        [OperationContract]
        private void ImportLedgerMainDetails(List<TblLedgerMainDetail> list, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                foreach (var row in list)
                {
                    if (row.TblCurrency1 != null && !string.IsNullOrWhiteSpace(row.TblCurrency1.Ename))
                    {
                        var currency = entity.TblCurrencyTests.FirstOrDefault(x => x.Ename.ToLower() == row.TblCurrency1.Ename.ToLower());
                        if (currency != null)
                        {
                            row.TblCurrency = currency.Iserial;
                            row.TblCurrency1 = null;

                            if (row.ExchangeRate == 0 || row.ExchangeRate == null)
                            {
                                row.ExchangeRate = currency.ExchangeRate;
                            }
                            else
                            {
                                row.ExchangeRate = row.ExchangeRate;
                            }

                        }
                    }

                    if (row.TblJournalAccountType1 != null && !string.IsNullOrWhiteSpace(row.TblJournalAccountType1.Ename))
                    {
                        try
                        {
                            var journalAccountType = entity.TblJournalAccountTypes.SingleOrDefault(x => x.Ename.ToLower() == row.TblJournalAccountType1.Ename.ToLower() || x.Code.ToLower() == row.TblJournalAccountType1.Ename.ToLower());
                            if (journalAccountType != null)
                            {
                                row.TblJournalAccountType = journalAccountType.Iserial;
                                row.TblJournalAccountType1 = null;
                            }
                            else
                            {
                                throw new Exception("Journal Account Type Not Found :" + row.TblJournalAccountType1.Ename);
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("Something Wrong With Journal Account Type :" + row.TblJournalAccountType1.Ename);
                        }
                   
                    }

                    if (row.TblJournalAccountType2 != null && !string.IsNullOrWhiteSpace(row.TblJournalAccountType2.Code))
                    {
                        try
                        {
                            var journalAccountType = entity.Entities.SingleOrDefault(x => x.scope == 0 && x.Code.ToLower() == row.TblJournalAccountType2.Code.ToLower() && x.TblJournalAccountType == row.TblJournalAccountType);
                        if (journalAccountType != null)
                        {
                            row.EntityAccount = journalAccountType.Iserial;
                            row.TblJournalAccountType2 = null;
                        }
                        else
                        {
                            throw new Exception("Entity Not Found Code:" + row.TblJournalAccountType2.Code + " With Type : " + row.TblJournalAccountType2.Code.ToLower());
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("Something Wrong With Entity :" + row.TblJournalAccountType2.Code + " With Type : " + row.TblJournalAccountType2.Code.ToLower());
                        }
                    }

                    if (row.TblMethodOfPayment1 != null && !string.IsNullOrWhiteSpace(row.TblMethodOfPayment1.Code))
                    {
                        try
                        {
                            var journalAccountType = entity.TblAccounts.SingleOrDefault(x => x.Code.ToLower() == row.TblMethodOfPayment1.Code.ToLower());
                            if (journalAccountType != null)
                            {
                                row.GlAccount = journalAccountType.Iserial;
                                row.TblMethodOfPayment1 = null;
                            }
                            else
                            {
                                throw new Exception("Account Not Found Code:" + row.TblMethodOfPayment1.Code.ToLower());
                            }

                        }
                        catch (Exception)
                        {
                            throw new Exception("Something Wrong With  Account Code :" + row.TblMethodOfPayment1.Code.ToLower());
                            
                        }

                    }
                    foreach (var variable in row.TblLedgerDetailCostCenters.Where(x => x.TblCostCenter1.Code != null && x.TblCostCenter1.Code != ""))
                    {
                        variable.Amount = (double)row.Amount;

                        try
                        {
                            var journalAccountType = entity.TblCostCenterTypes.SingleOrDefault(x => x.Ename.ToLower() == variable.TblCostCenterType1.Ename.ToLower());
                            if (journalAccountType != null)
                            {
                                variable.TblCostCenterType = journalAccountType.Iserial;
                                variable.TblCostCenterType1 = null;
                            }
                            else
                            {
                                throw new Exception("Cost Center Type Not Found Code:" + variable.TblCostCenterType1.Ename.ToLower());
                            }
                        }
                        catch
                        {

                            throw new Exception("something Wrong With Center Type Code:" + variable.TblCostCenterType1.Ename.ToLower());

                        }
                        try
                        {
                            var costCenter = entity.TblCostCenters.SingleOrDefault(x =>x.TblCostCenterType== variable.TblCostCenterType && x.Code.ToLower() == variable.TblCostCenter1.Code.ToLower());
                            if (costCenter != null)
                            {
                                variable.TblCostCenter = costCenter.Iserial;
                                variable.TblCostCenter1 = null;
                            }
                            else
                            {
                                throw new Exception("Cost Center Not Found Code:" + variable.TblCostCenter1.Code.ToLower());
                            }
                        }
                        catch
                        {

                            throw new Exception("Something Wrong with Cost Center Code:" + variable.TblCostCenter1.Code.ToLower());
                        }
                    }

                    var tblLedgerHeader = entity.TblLedgerHeaders.FirstOrDefault(x => x.Iserial == row.TblLedgerHeader);
                    if (tblLedgerHeader != null)
                    {
                        var ledgerheader = tblLedgerHeader.TblJournal;
                        var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == ledgerheader);
                        int temp = 0;
                        row.Code = HandelSequence(row.Code, journal, "TblLedgerMainDetail", company, list.IndexOf(row), 0, 0, out temp);
                    }

                    var newrow = new TblLedgerDetail();
                    newrow.InjectFrom(row);
                    row.TblLedgerDetails = new EntityCollection<TblLedgerDetail> { newrow };
                    entity.TblLedgerMainDetails.AddObject(row);
                }
                entity.SaveChanges();
            }
        }

        [OperationContract]
        private void ReverseLedgerTransactions(int TblJournlLink,int TblTransactionType,string month, string year, string no,string TblUser, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.ReverseLedgerTransactions(TblJournlLink, TblTransactionType, month, year, no, TblUser);
            }
        }
    }
}