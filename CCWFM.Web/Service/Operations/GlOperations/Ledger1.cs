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
        public TblLedgerHeader1 PrintLedgerHeader1(int iserial, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.SpFixGlBalanced();
                return entity.TblLedgerHeader1.FirstOrDefault(w => w.Iserial == iserial);
            }
        }


        [OperationContract]
        public TblLedgerMainDetail1 PostTotalLedger1(int iserial, int user, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var ledgerheader = entity.TblLedgerHeader1.FirstOrDefault(w => w.Iserial == iserial);

                ledgerheader.balanced = true;
                var TblLedgerMainDetail1 = entity.TblLedgerMainDetail1.Include("TblLedgerHeader11.TblJournal1").Where(w => w.TblLedgerHeader1 == iserial).ToList();

                var LedgerDetailRecord = TblLedgerMainDetail1.FirstOrDefault();
                var Journal = TblLedgerMainDetail1.FirstOrDefault().TblLedgerHeader11.TblJournal1;
                var entityAccount = Journal.Entity;
                var journalType = Journal.TblJournalAccountType;

                var SumDr = TblLedgerMainDetail1.Where(w => w.DrOrCr == true).Sum(w => w.Amount);

                var SumCr = TblLedgerMainDetail1.Where(w => w.DrOrCr == false).Sum(w => w.Amount);
                var Entity = entity.Entities.FirstOrDefault(w => w.Iserial == entityAccount && w.TblJournalAccountType == journalType);
                decimal total = (SumDr - SumCr) ?? 0;
                var amount = Math.Abs(total);
                var newLedgerMainDetail = new TblLedgerMainDetail1()
                {
                    TblLedgerHeader1 = iserial,
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
                UpdateOrInsertTblLedgerMainDetail1(entity, newLedgerMainDetail, true, 0, out temp, user);
                return newLedgerMainDetail;
            }
        }
        [OperationContract]
        public void CorrectLedgerHeader1Rouding(int TblLedgerHeader1, string company, int user)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var ledgerHeader = entity.TblLedgerHeader1.Include("TblLedgerMainDetail1").FirstOrDefault(x => x.Iserial == TblLedgerHeader1);

                var totaldebit = ledgerHeader.TblLedgerMainDetail1.Where(x => x.DrOrCr == true && x.OffsetEntityAccount == null).Sum(x => x.Amount * (decimal)x.ExchangeRate);

                var totalCredit = ledgerHeader.TblLedgerMainDetail1.Where(x => x.DrOrCr == false && x.OffsetEntityAccount == null).Sum(x => x.Amount* (decimal)x.ExchangeRate);

                if (totaldebit - totalCredit != 0)
                {
                    int temp = 0;
                    if (totaldebit > totalCredit)
                    {
                        var tblledgerdetail =
                            ledgerHeader.TblLedgerMainDetail1.LastOrDefault(
                                x => x.DrOrCr == false && x.OffsetEntityAccount == null);
                        tblledgerdetail.Amount = tblledgerdetail.Amount + totaldebit - totalCredit;
                        UpdateOrInsertTblLedgerMainDetail1(tblledgerdetail, false, 00, out temp, company, user);
                    }

                    if (totaldebit < totalCredit)
                    {
                        var tblledgerdetail =
                            ledgerHeader.TblLedgerMainDetail1.LastOrDefault(
                                x => x.DrOrCr == true && x.OffsetEntityAccount == null);
                        tblledgerdetail.Amount = tblledgerdetail.Amount + totalCredit - totaldebit;
                        UpdateOrInsertTblLedgerMainDetail1(tblledgerdetail, false, 00, out temp, company, user);
                    }
                }
            }
        }

        [OperationContract]
        private List<TblLedgerHeader1> GetTblLedgerHeader1(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, int user, string company)
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
                IQueryable<TblLedgerHeader1> query;
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
                    fullCount = entity.TblLedgerHeader1.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblLedgerHeader1.Include("TblJournal1.TblCurrency1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    if (JournalList.Any())
                    {
                        fullCount = entity.TblLedgerHeader1.Count(w => JournalList.Contains(w.TblJournal));
                        query = entity.TblLedgerHeader1.Include("TblJournal1.TblCurrency1").OrderBy(sort).Where(w =>  JournalList.Contains(w.TblJournal)).Skip(skip).Take(take);
                        //query = query.Where(w => JournalList.Contains(w.TblJournal));
                    }
                    else
                    {
                        fullCount = entity.TblLedgerHeader1.Count();
                        query = entity.TblLedgerHeader1.Include("TblJournal1.TblCurrency1").OrderBy(sort).Skip(skip).Take(take);
                    }


                }
                query = query.OrderByDescending(w => w.DocDate);

                return query.ToList();
            }
        }




        [OperationContract]
        public TblLedgerHeader1 UpdateOrInsertTblLedgerHeader1s(TblLedgerHeader1 newRow, bool save, int index, out int outindex, int user, string company, bool validate = false)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return UpdateOrInsertTblLedgerHeader1s(entity, newRow, save, index, out outindex, user, validate);
            }
        }


        public TblLedgerHeader1 UpdateOrInsertTblLedgerHeader1s(ccnewEntities entity, TblLedgerHeader1 newRow, bool save, int index, out int outindex, int user, bool validate = false)
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
                    entity.TblLedgerHeader1.AddObject(newRow);

                } catch (Exception EX ) {
                    string MSG = EX.Message;
                }
               
            }
            else
            {
                var oldRow = (from e in entity.TblLedgerHeader1
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
                    // UpdateOrInsertTblLedgerHeader1s(newRow, save, index, out outindex, user, company);
                }
                else
                {
                    throw ex;
                }

            }

            return newRow;

        }


       

        [OperationContract]
        private int DeleteTblLedgerHeader1(TblLedgerHeader1 row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerHeader1
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        private void InsertIntoLedgerdetail(TblLedgerMainDetail1 row, ccnewEntities entity)
        {
            var query = (from e in entity.TblLedgerDetail1
                         where e.TblLedgerMainDetail1 == row.Iserial
                         select e).ToList();
            foreach (var variable in query)
            {
                entity.DeleteObject(variable);
            }
            if (row.OffsetEntityAccount != null)
            {
                var newrow = new TblLedgerDetail1();
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
                newrow.TblLedgerMainDetail1 = row.Iserial;
                newrow.Iserial = 0;
                entity.TblLedgerDetail1.AddObject(newrow);
            }
            var newRow2 = new TblLedgerDetail1();
            newRow2.InjectFrom(row);

            newRow2.EntityKey = null;
            newRow2.EntityRef = null;

            newRow2.TblLedgerMainDetail1 = row.Iserial;
            newRow2.Iserial = 0;
            entity.TblLedgerDetail1.AddObject(newRow2);
            entity.SaveChanges();
        }

   
     
        [OperationContract]
        private List<TblLedgerMainDetail1> GetTblLedgerMainDetail1(int skip, int take, int ledgerHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList, out Dictionary<int, bool> TransactionExist)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblLedgerMainDetail1> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLedgerHeader1 ==(@Group0)";
                    valuesObjects.Add("Group0", ledgerHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblLedgerMainDetail1.Where(filter, parameterCollection).Count();
                    query = entity.TblLedgerMainDetail1.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblLedgerMainDetail1.Count(v => v.TblLedgerHeader1 == ledgerHeader);
                    query = entity.TblLedgerMainDetail1.Include("TblAccount").Include("TblBankCheque1").Include("TblMethodOfPayment1").Include("TblBankTransactionType1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(v => v.TblLedgerHeader1 == ledgerHeader).Skip(skip).Take(take);
                }
                //query = query.OrderByDescending(x => x.TransDate);
                List<int?> intList = query.Select(x => x.EntityAccount).ToList();

                intList.AddRange(query.Select(x => x.OffsetEntityAccount).ToList());

                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                intTypeList.AddRange(query.Select(x => x.OffsetAccountType).ToList());
                entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                var listOfStyles = query.Select(x => x.Iserial);
                TransactionExist = entity.TblLedgerDetail1CostCenter.Where(x => listOfStyles.Count(l => x.TblLedgerMainDetail1 == l) > 1)
                        .GroupBy(x => x.TblLedgerMainDetail1).ToDictionary(t => t.Key, t => true);
                return query.ToList();
            }
        }

        [OperationContract]
        public TblLedgerMainDetail1 UpdateOrInsertTblLedgerMainDetail1(TblLedgerMainDetail1 newRow, bool save, int index, out int outindex, string company, int user, bool validate = false)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return UpdateOrInsertTblLedgerMainDetail1(entity, newRow, save, index, out outindex, user, validate);
            }
        }
        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerDetail1CostCenter newRow, int user)
        {
            var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);
            if (SettingExist)
            {
                var journal = entity.TblLedgerMainDetail1.Include("TblLedgerHeader11").FirstOrDefault(w => w.Iserial == newRow.TblLedgerMainDetail1).TblLedgerHeader11.TblJournal;
                var Settings = entity.TblJournalSettingCostCenters.Any(w => w.TblCostCenter == newRow.TblCostCenter &&
                 w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user));
                if (!Settings)
                {
                    throw new FaultException("You are not authorized to use this Cost Center");
                }
            }

        }

        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerMainDetail1 newRow, int user)
        {

            var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);
            if (SettingExist)
            {

                var entityRow = entity.Entities.FirstOrDefault(w => w.TblJournalAccountType == newRow.TblJournalAccountType && w.Iserial == newRow.EntityAccount && w.scope == 0);
                var journal = entity.TblLedgerHeader1.FirstOrDefault(w => w.Iserial == newRow.TblLedgerHeader1).TblJournal;
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
        private void ValidateJournalSetting(ccnewEntities entity, TblLedgerHeader1 newRow, int user)
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

        public TblLedgerMainDetail1 UpdateOrInsertTblLedgerMainDetail1(ccnewEntities entity, TblLedgerMainDetail1 newRow, bool save, int index, out int outindex, int user, bool validate = false)
        {
            outindex = index;

            if (validate)
            {
                ValidateJournalSetting(entity, newRow, user);
            }

            if (save)
            {
                var TblLedgerHeader1 = entity.TblLedgerHeader1.FirstOrDefault(x => x.Iserial == newRow.TblLedgerHeader1);
                if (TblLedgerHeader1 != null)
                {
                    var ledgerheader = TblLedgerHeader1.TblJournal;
                    var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == ledgerheader);
                    int temp = 0;
                    newRow.Code = HandelSequence(newRow.Code, journal, "TblLedgerMainDetail", 0, 0, 0, entity, out temp);
                }
                entity.TblLedgerMainDetail1.AddObject(newRow);
            }
            else
            {
                var oldRow = (from e in entity.TblLedgerMainDetail1
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var costCenters = (from e in entity.TblLedgerDetail1CostCenter
                                       where e.TblLedgerMainDetail1 == newRow.Iserial
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
                        //    entity.TblLedgerDetail1CostCenters.DeleteObject(item);
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
       //     UpdateCheckFromTransaction(newRow, entity);
            return newRow;
        }



        private void ReverseTblLedgerHeader1(int iserial, int user, string company, int tblBankCheque, int newHeaderIserial, decimal? amount = 0)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {

                var query = entity.TblLedgerHeader1.FirstOrDefault(x => x.Iserial == iserial);
                int temp;
                foreach (var variable in entity.TblLedgerMainDetail1.Where(x => x.TblLedgerHeader1 == query.Iserial && (x.TblBankCheque == tblBankCheque || tblBankCheque == 0)).ToList())
                {
                    variable.DrOrCr = !variable.DrOrCr;
                    variable.TblLedgerHeader1 = newHeaderIserial;
                    if (amount > 0)
                    {
                        variable.Amount = amount;
                    }
                    var row = new TblLedgerMainDetail1()
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
                        TblLedgerHeader1 = newHeaderIserial
                    };

                    UpdateOrInsertTblLedgerMainDetail1(row, true, 0, out temp, company, user);
                }
            }
        }

        [OperationContract]
        private int DeleteTblLedgerMainDetail1(TblLedgerMainDetail1 row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerMainDetail1
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblLedgerDetail1CostCenter> GetTblLedgerDetail1CostCenter(int skip, int take, int ledgerHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblLedgerDetail1CostCenter> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLedgerMainDetail1 ==(@Group0)";
                    valuesObjects.Add("Group0", ledgerHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblLedgerDetail1CostCenter.Where(filter, parameterCollection).Count();
                    query = entity.TblLedgerDetail1CostCenter.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblLedgerDetail1CostCenter.Count(v => v.TblLedgerMainDetail1 == ledgerHeader);
                    query = entity.TblLedgerDetail1CostCenter.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(v => v.TblLedgerMainDetail1 == ledgerHeader).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public TblLedgerDetail1CostCenter UpdateOrInsertTblLedgerDetail1CostCenters(TblLedgerDetail1CostCenter newRow, bool save, int index, out int outindex, int user, string company, bool validate = false)
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
                    entity.TblLedgerDetail1CostCenter.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblLedgerDetail1CostCenter
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
        private int DeleteTblLedgerDetail1CostCenter(TblLedgerDetail1CostCenter row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblLedgerDetail1CostCenter
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int PostGlLedgerHeader1(int iserial, int user, string company, bool postOrApprove)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var header = entity.TblLedgerHeader1.Include("TblLedgerMainDetail11.TblLedgerDetail1")
                     .FirstOrDefault(x => x.Iserial == iserial);
                var detailsdr = header.TblLedgerMainDetail1.Sum(x => x.TblLedgerDetail1.Where(e => e.DrOrCr == true).Sum(w => w.Amount));

                var detailscr = header.TblLedgerMainDetail1.Sum(x => x.TblLedgerDetail1.Where(e => e.DrOrCr == false).Sum(w => w.Amount));

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
        private void ImportLedgerMainDetail1(List<TblLedgerMainDetail1> list, string company)
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
                    foreach (var variable in row.TblLedgerDetail1CostCenter.Where(x => x.TblCostCenter1.Code != null && x.TblCostCenter1.Code != ""))
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

                    var TblLedgerHeader1 = entity.TblLedgerHeader1.FirstOrDefault(x => x.Iserial == row.TblLedgerHeader1);
                    if (TblLedgerHeader1 != null)
                    {
                        var ledgerheader = TblLedgerHeader1.TblJournal;
                        var journal = entity.TblJournals.Include("TblSequence1").Include("TblSequence").FirstOrDefault(x => x.Iserial == ledgerheader);
                        int temp = 0;
                        row.Code = HandelSequence(row.Code, journal, "TblLedgerMainDetail", company, list.IndexOf(row), 0, 0, out temp);
                    }

                    var newrow = new TblLedgerDetail1();
                    newrow.InjectFrom(row);
                    row.TblLedgerDetail1 = new EntityCollection<TblLedgerDetail1> { newrow };
                    entity.TblLedgerMainDetail1.AddObject(row);
                }
                entity.SaveChanges();
            }
        }

    
    }
}