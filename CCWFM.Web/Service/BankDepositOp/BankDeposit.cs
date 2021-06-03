using CCWFM.Models.Gl;
using CCWFM.Web.Model;
using CCWFM.Web.Service.AssistanceOp;
using CCWFM.Web.Service.Operations;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;

namespace CCWFM.Web.Service.BankDepositOp
{
    public partial class BankDepositService
    {
        Operations.GlOperations.GlService service = new Operations.GlOperations.GlService();
        [OperationContract]
        private List<TblCashDepositHeader> GetCashDepositHeader(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, string company,
            out int fullCount)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                context.CommandTimeout = 0;
                var defaultQuery = context.TblCashDepositHeaders.Include(
                    nameof(TblCashDepositHeader.TblStore1)).Include(
                    nameof(TblCashDepositHeader.TblBank1)).Include(string.Format(
                        "{0}.{1}", nameof(TblCashDepositHeader.TblTenderType1),
                        nameof(TblTenderType.TblCashDepositTypes)));
                IQueryable<TblCashDepositHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = defaultQuery.Where(filter, parameterCollection.ToArray()).Count();
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort);
                }
                else
                {
                    fullCount = defaultQuery.Count();
                    query = defaultQuery.OrderBy(sort);
                }
                var Result = query.Skip(skip).Take(take).ToList();
                return Result;
            }
        }

        [OperationContract]
        private TblCashDepositHeader GetCashDepositHeaderByIserial(int headerIserial, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                return context.TblCashDepositHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
            }
        }
        [OperationContract]
        private TblCashDepositHeader UpdateOrInsertCashDepositHeader(TblCashDepositHeader newRow,
            string company, int index, int userIserial, out int outindex)
        {
            var approved = newRow.Approved;
            newRow.Approved = false;
            newRow.ApproveDate = null;
            var tblCashDepositHeader = newRow;
            bool isNew = false, isAmountUpdated = false;//, isApproved = false;
            try
            {
                //using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(600)))
                {
                    outindex = index;
                    using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
                    {
                        context.CommandTimeout = 0;
                        newRow.TblBank1 = null;
                        newRow.TblStore1 = null;
                        //newRow.DocDate = newRow.DocDate.Value.Date;//.AddTicks(DateTime.Now.TimeOfDay.Ticks);
                        var oldRow = context.TblCashDepositHeaders.Include(nameof(TblCashDepositHeader.TblCashDepositDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);
                        if (oldRow != null)// الهيدر موجود قبل كده كده هو هيعمل ابروف بس
                        {
                            newRow.TblTenderType1 = null;
                            if (newRow.Amount != oldRow.Amount)
                                isAmountUpdated = true;
                            newRow.OldAmount = oldRow.Amount;
                            if (newRow.Canceled)
                            {
                                newRow.CanceledBy = userIserial;
                                newRow.CanceledDate = DateTime.Now;
                            }
                            else
                            {
                                foreach (var item in newRow.TblCashDepositAmountDetails.ToArray())
                                {
                                    // هشوف بقى الى اتعدل والجديد
                                    int temp, headeriserial;
                                    headeriserial = item.TblCashDepositHeader;
                                    item.TblCashDepositHeader1 = null;
                                    item.TblCashDepositHeader = headeriserial;
                                    UpdateOrInsertCashDepositAmountDetail(item, 1, userIserial, company, out temp);
                                    item.TblCashDepositHeader1Reference = null;
                                }
                                if (oldRow.Approved != newRow.Approved)// كده لسه معموله ابروف
                                {
                                    //newRow.ApproveDate = DateTime.Now;
                                    //newRow.ApprovedBy = userIserial;
                                    foreach (var item in newRow.TblCashDepositDetails.ToArray())
                                    {
                                        // هشوف بقى الى اتعدل والجديد
                                        int temp, headeriserial;
                                        headeriserial = item.TblCashDepositHeader;
                                        item.TblCashDepositHeader1 = null;
                                        item.TblCashDepositHeader = headeriserial;
                                        UpdateOrInsertCashDepositDetail(item, 1, userIserial, company, out temp);
                                        //item.TblCashDepositHeader1 = newRow;
                                    }
                                    //isApproved = true;
                                }
                            }
                            //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                            var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                            if (result.Count() > 0)
                            {
                                oldRow.LastChangeDate = DateTime.Now.Date;
                                oldRow.LastChangeUser = userIserial;
                            }
                        }
                        else// الهيدر ده جديد
                        {
                            if (newRow.Canceled) return newRow;
                            isNew = true;
                            int sequence = context.TblCashDepositTypes.FirstOrDefault(cdt =>
                            cdt.TblTenderTypes.Any(d => d.ISerial == newRow.TblTenderType)).TblSequence;
                            var seq = context.TblSequences.FirstOrDefault(s => s.Iserial == sequence);

                            newRow.TblTenderType1 = null;
                            newRow.Sequance = SharedOperation.HandelSequence(seq, company, 0, 0, 0);
                            newRow.OldAmount = newRow.Amount;
                            newRow.CreatedBy = userIserial;
                            newRow.CreationDate = DateTime.Now;
                            newRow.LastChangeDate = DateTime.Now;
                            newRow.LastChangeUser = userIserial;

                            context.TblCashDepositHeaders.AddObject(newRow);
                        }

                        if (isNew && !newRow.Canceled)
                        {
                            //emailSubject = string.Format(subjectTemplate, "جديد");
                            //emailBody = string.Format(bodyTemplate, newRow.Sequance, "اضافة");
                        }
                        if (isAmountUpdated && !newRow.Canceled)
                        {
                            //emailSubject = string.Format(subjectTemplate, "معدل");
                            //emailBody = string.Format(bodyTemplate, newRow.Sequance, "تعديل");
                        }
                        //if (isApproved && !newRow.Canceled)
                        //{
                        //    AddLeadger(context, newRow.Iserial, company, userIserial);                           
                        //}
                        context.SaveChanges();
                        //scope.Complete();
                    }
                }
                if (approved)
                    ApproveCashDepositHeader(newRow, company, index, userIserial, out outindex);
            }
            catch (Exception ex) { throw ex; }
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var iserial = newRow.Iserial;
                return context.TblCashDepositHeaders.Include(
                            nameof(TblCashDepositHeader.TblStore1)).Include(
                            nameof(TblCashDepositHeader.TblBank1)).Include(
                            nameof(TblCashDepositHeader.TblCashDepositDetails)).Include(
                            nameof(TblCashDepositHeader.TblCashDepositAmountDetails)).Include(string.Format(
                                "{0}.{1}", nameof(TblCashDepositHeader.TblTenderType1),
                                nameof(TblTenderType.TblCashDepositTypes))).FirstOrDefault(h => h.Iserial == iserial);
            }
        }

        private TblCashDepositHeader ApproveCashDepositHeader(TblCashDepositHeader newRow, string company,
           int index, int userIserial, out int outindex)
        {
            newRow.Approved = true;
            newRow.ApproveDate = DateTime.Now;
            bool isApproved = false;
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(600)))
                {
                    outindex = index;
                    using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
                    {
                        context.CommandTimeout = 0;
                        newRow.TblBank1 = null;
                        newRow.TblStore1 = null;
                        var oldRow = context.TblCashDepositHeaders.Include(nameof(TblCashDepositHeader.TblCashDepositDetails))
                            .Include(nameof(TblCashDepositHeader.TblCashDepositAmountDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);
                        if (oldRow != null)// الهيدر موجود قبل كده كده هو هيعمل ابروف بس
                        {
                            newRow.TblTenderType1 = null;
                            newRow.OldAmount = oldRow.Amount;
                            if (newRow.Canceled)
                            {
                                newRow.CanceledBy = userIserial;
                                newRow.CanceledDate = DateTime.Now;
                            }
                            else
                            {
                                isApproved = newRow.Approved;
                                //if (oldRow.Approved != newRow.Approved)// كده لسه معموله ابروف
                                //{
                                newRow.ApproveDate = DateTime.Now;
                                newRow.ApprovedBy = userIserial;
                                //}
                            }
                            var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                            if (result.Count() > 0)
                            {
                                oldRow.LastChangeDate = DateTime.Now.Date;
                                oldRow.LastChangeUser = userIserial;
                            }
                            if (oldRow.Amount != oldRow.TblCashDepositAmountDetails.Sum(sd => sd.Amount) ||
                                oldRow.Amount != oldRow.TblCashDepositDetails.Sum(sd => sd.Amount))
                                throw new InvalidOperationException("Amount not match");
                        }
                        else
                        if (newRow.Amount != newRow.TblCashDepositAmountDetails.Sum(sd => sd.Amount) ||
                            newRow.Amount != newRow.TblCashDepositDetails.Sum(sd => sd.Amount))
                            throw new InvalidOperationException("Amount not match");
                        if (isApproved && !newRow.Canceled)
                        {
                            int journalint = Convert.ToInt32(
                                      service.GetRetailChainSetupByCode("CashDepositJournalIserial", company).sSetupValue);// Code = 1
                            var storesSafeSetting = service.GetRetailChainSetupByCode("StoresSafeCode", company);
                            var hasLedger = context.TblLedgerHeaders.Any(lh => lh.TblJournalLink == newRow.Iserial &&
                                   lh.TblTransactionType == 9);
                            if (!hasLedger)
                                AddLeadger(context, newRow.Iserial, userIserial, journalint, storesSafeSetting);
                        }
                        context.SaveChanges();
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var iserial = newRow.Iserial;
                return context.TblCashDepositHeaders.Include(
                            nameof(TblCashDepositHeader.TblStore1)).Include(
                            nameof(TblCashDepositHeader.TblBank1)).Include(
                            nameof(TblCashDepositHeader.TblCashDepositDetails)).Include(
                            nameof(TblCashDepositHeader.TblCashDepositAmountDetails)).Include(string.Format(
                                "{0}.{1}", nameof(TblCashDepositHeader.TblTenderType1),
                                nameof(TblTenderType.TblCashDepositTypes))).FirstOrDefault(h => h.Iserial == iserial);
            }
        }
        //private void testStoreMail()
        //{
        //    new Thread(delegate ()
        //    {
        //        SharedOperation.SendEmail(null, "Retail@ccausal.loc", new List<string>() {
        //            "Mohammed.Soliman@cc-egypt.com" }, "Test", "Teeeeeeeeeeeeeesssssssssssssst");
        //    }).Start();
        //    //SharedOperation.SendMailReport("CashDepositeDocument", string.Format("إيصال إستلام {0}",
        //    //            DateTime.Now.ToString("dd-MM-yyyy")), new List<ParameterValue>() {
        //    //                    new ParameterValue() {
        //    //                        Name = "headerIserial",Value = "1" } },
        //    //                    "CCNEW", "Mohammed.Soliman@cc-egypt.com",
        //    //                    "Test", "Teeeeeeeeeeeeeesssssssssssssst");
        //}
        [OperationContract]
        private void SendApproveMail(int headerIserial, int storeIserial, DateTime date, string company,
           string Sequance, int userIserial)
        {
            string emailSubject, emailBody;
            var service = new Operations.GlOperations.GlService();
            string subjectTemplate = service.GetRetailChainSetupByCode(
                "CashDepositMailSubject", company).sSetupValue;
            string bodyTemplate = service.GetRetailChainSetupByCode(
                "CashDepositMailBody", company).sSetupValue;
            emailSubject = string.Format(subjectTemplate, "نهائى");
            emailBody = string.Format(bodyTemplate, Sequance, "تاكيد");

            Logger logger = LogManager.GetLogger(Global.InfoLoggerName);
            logger.Info(string.Format(
                "Start SendApproveMail- headerIserial = {0}, storeIserial = {1}, company = {2}, userIserial = {3}",
                headerIserial, storeIserial, company, userIserial));
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                try
                {
                    var store = context.TblStores.FirstOrDefault(s => s.iserial == storeIserial);
                    if (store != null)//&& SharedOperation.IsValidEmail(store.Address))
                    {
                        //logger.Info("Store Is not null");
                        string storeMail = store.Address;
                        SharedOperation.SendMailReport("CashDepositeDocument", string.Format("إيصال إستلام {0}",
                           date.ToString("dd-MM-yyyy")), new List<ParameterValue>() {
                                new ParameterValue() {
                                    Name = "headerIserial",Value = headerIserial.ToString() } },
                                   company, storeMail, emailSubject, emailBody);
                    }
                }
                catch (Exception ex)
                {
                    new AssistanceService().SaveLog(JsonConvert.SerializeObject(ex), userIserial);
                }
            }
        }

        private void AddLeadger(ccnewEntities context, int rowIserial, int userIserial,
            int journalint, tblChainSetupTest storesSafeSetting)
        {
            var newRow = context.TblCashDepositHeaders.Include(nameof(TblCashDepositHeader.TblCashDepositDetails)).FirstOrDefault(h =>
                h.Iserial == rowIserial);

            var detailList = newRow.TblCashDepositDetails.Select(d => new
            {
                TblBank = d.TblBank ?? 0,
                TblJournalAccountType = d.TblJournalAccountType ?? 0,
                EntityAccount = d.EntityAccount ?? 0,
                MachineId = d.MachineId
            });
            // القيد
            var headerType = context.TblCashDepositTypes.FirstOrDefault(cd =>
                  cd.TblTenderTypes.Any(r => r.ISerial == newRow.TblTenderType));
            if (headerType.Iserial == (int)CashDepositType.Discount) return;

            var newLedgerHeaderRow = new TblLedgerHeader
            {
                CreatedBy = userIserial,
                CreationDate = DateTime.Now,
                Description = newRow.LedgerDescription ?? "",
                DocDate = newRow.DocDate,
                TblJournal = journalint,
                TblTransactionType = 9,
                TblJournalLink = newRow.Iserial
            };
            int tmp;

            #region Bank Cash
            List<Entity> bankEntities = new List<Entity>();
            List<Entity> bankExpencesEntities = new List<Entity>();
            Entity subSafeEntity = new Entity();
            if (headerType.Iserial == (int)CashDepositType.Cheque || headerType.Iserial == (int)CashDepositType.Cash || headerType.Iserial == (int)CashDepositType.TFKDiscount15  ||  headerType.Iserial == (int)CashDepositType.TFKDiscount15 ||  headerType.Iserial == (int)CashDepositType.TFKCash)
            {
                var enumerable = detailList.Select(d => d.TblBank).Distinct().ToList();
                bankEntities = context.Entities.Where(e => e.scope == 0 &&
                                   enumerable.Any(item => e.Iserial == item) &&
                                    e.TblJournalAccountType == 6).Distinct().ToList();
                if (newRow.IsSubSafe)
                {
                    subSafeEntity = context.Entities.FirstOrDefault(e => e.scope == 0 &&
                                      e.Code == storesSafeSetting.sSetupValue &&
                                      e.TblJournalAccountType == 0);
                }
                var EntityAccountList = detailList.Select(item => item.EntityAccount).Distinct().ToList();
                var JournalAccountTypeList = detailList.Select(item => item.TblJournalAccountType).Distinct().ToList();
                bankExpencesEntities = context.Entities.Where(e => e.scope == 0 &&
                    JournalAccountTypeList.Any(item => e.TblJournalAccountType == item) &&
                    EntityAccountList.Any(item => e.Iserial == item)).Distinct().ToList();
            }
            #endregion

            #region Expences
            List<Entity> ExpencesEntities = new List<Entity>();
            if (headerType.Iserial == (int)CashDepositType.Expences)
            {
                var EntityAccountList = detailList.Select(item => item.EntityAccount).Distinct().ToList();
                var JournalAccountTypeList = detailList.Select(item => item.TblJournalAccountType).Distinct().ToList();
                ExpencesEntities = context.Entities.Where(e => e.scope == 0 &&
                    //detailList.Any(item => e.Iserial == item.EntityAccount &&
                    //e.TblJournalAccountType == item.TblJournalAccountType)
                    JournalAccountTypeList.Any(item => e.TblJournalAccountType == item) &&
                    EntityAccountList.Any(item => e.Iserial == item)).Distinct().ToList();
            }
            #endregion

            #region Visa & Premium
            List<TblVisaMachine> machines = new List<TblVisaMachine>();
            List<Entity> visaExpensesEntities = new List<Entity>();
            if (headerType.Iserial == (int)CashDepositType.Visa ||
                headerType.DepositeTypeGroup == (int)CashDepositType.PremiumCard
                )


            {
                var bankList = detailList.Select(d => d.TblBank).Distinct().ToList();
                bankEntities = context.Entities.Where(e => e.scope == 0 &&
                                   bankList.Any(item => e.Iserial == item) &&
                                    e.TblJournalAccountType == 6).Distinct().ToList();

                var machineList = detailList.Select(d => d.MachineId).Distinct().ToList();

                machines = context.TblVisaMachines.Where(vm => machineList.Any(item =>
                  vm.MachineId == item)).Distinct().ToList();
                //if (headerType.Iserial == (int)CashDepositType.PremiumCard  )
                if (headerType.DepositeTypeGroup == (int)CashDepositType.PremiumCard)
                    machines = context.TblVisaMachines.Where(vm => bankList.Any(item =>
                        vm.TblBank == item)).Distinct().ToList();
                var list1 = machines.Select(m => m.EntityAccount).Distinct().ToList();
                visaExpensesEntities = context.Entities.Where(e => e.scope == 0 &&
                    list1.Any(machine => e.Iserial == machine) &&
                    e.TblJournalAccountType == 15).Distinct().ToList();
            }
            #endregion


            #region Visa & Premium

            #endregion

            newLedgerHeaderRow = service.UpdateOrInsertTblLedgerHeaders(context, newLedgerHeaderRow, true, 0, out tmp, userIserial);
            var storeEntity = context.Entities.FirstOrDefault(e => e.scope == 0 &&
                               e.Iserial == newRow.TblStore && e.TblJournalAccountType == 8);

            #region Store Entity               
            var storeLedgerDetail = new TblLedgerMainDetail();
            storeLedgerDetail = new TblLedgerMainDetail
            {
                Amount = newRow.Amount,
                Description = newRow.LedgerDescription ?? "",
                ExchangeRate = 1,
                TblCurrency = 1,
                TransDate = newRow.DocDate,
                TblJournalAccountType = 8,
                EntityAccount = storeEntity.Iserial,//Entity Iserial
                GlAccount = storeEntity.AccountIserial,// Account Iserial
                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                PaymentRef = newRow.Sequance,
                DrOrCr = false// true debit or false for credit
            };
            service.UpdateOrInsertTblLedgerMainDetails(context, storeLedgerDetail, true, 0, out tmp,
                  userIserial);
            #endregion

            var CashDepositSetting = context.TblCashDepositSettings.ToList();
            // recivable and expences debit store cridit
            //if (headerType.Iserial == (int)CashDepositType.PremiumCard )
            if (headerType.DepositeTypeGroup == (int)CashDepositType.PremiumCard)
            {
                #region Premium Card
                // Bank Entity
                var bankEntity = bankEntities.FirstOrDefault(e => e.Iserial == newRow.TblBank);
                var machine = machines.FirstOrDefault(vm => vm.TblBank == newRow.TblBank);
                var amount = newRow.Amount;
                if (machine!=null)
                {
                      amount = newRow.Amount * (1 - (machine.DiscountPercent / 100));
                }
             
                var bankLedgerDetail = getLedgerDetail(newRow.DocDate, newRow.Sequance, true,
                    newLedgerHeaderRow.Iserial, newRow, bankEntity, 6, amount);
                service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp,
                    userIserial);

                // Expenses Entity                   
                var visaExpensesEntity = visaExpensesEntities.FirstOrDefault(e =>
                    e.Iserial == machine.EntityAccount);

                var visaExpensesLedgerDetail = getLedgerDetail(newRow.DocDate, newRow.Sequance, true,
                    newLedgerHeaderRow.Iserial, newRow, visaExpensesEntity, 15, newRow.Amount - amount);
                service.UpdateOrInsertTblLedgerMainDetails(context, visaExpensesLedgerDetail, true, 0, out tmp,
                    userIserial);

                // Cost Center
                var storeCostcenter = new TblGlRuleDetail();
                storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                   context); // company,
                service.CreateTblLedgerDetailCostCenter(context, visaExpensesLedgerDetail.Amount ?? 0,
                    visaExpensesLedgerDetail, storeCostcenter);//company,

                #endregion
            }
            else
                foreach (var item in newRow.TblCashDepositDetails)
                {
                    var @ref = item.BatchNo;//item.BatchDate.Value.ToString("yyyyMMdd")+item.Amount.ToString("#");
                    // bank debit store cridit
                    if (headerType.Iserial == (int)CashDepositType.Cheque || headerType.Iserial == (int)CashDepositType.Cash ||  headerType.Iserial == (int)CashDepositType.TFKDiscount15 || headerType.Iserial == (int)CashDepositType.TFKCash)
                    {
                        #region cash & Bank
                        // Bank Entity
                        var bankEntity = bankEntities.FirstOrDefault(e => e.Iserial == item.TblBank);

                        var bankLedgerDetail = new TblLedgerMainDetail();
                        if (bankEntity != null)
                            bankLedgerDetail = getLedgerDetail(item.BatchDate, @ref, true,
                                newLedgerHeaderRow.Iserial, item, bankEntity, 6, item.Amount);
                        if (newRow.IsSubSafe)
                        {
                            bankEntity = subSafeEntity;
                            bankLedgerDetail = getLedgerDetail(item.BatchDate, @ref, true,
                                newLedgerHeaderRow.Iserial, item, bankEntity, 0, item.Amount);
                        }
                        else if (item.TblJournalAccountType != null && item.EntityAccount != null)
                        {
                            bankEntity = bankExpencesEntities.FirstOrDefault(e =>
                                e.Iserial == item.EntityAccount && e.TblJournalAccountType == item.TblJournalAccountType);
                            bankLedgerDetail = getLedgerDetail(item.BatchDate, @ref, true,
                                newLedgerHeaderRow.Iserial, item, bankEntity, item.TblJournalAccountType, item.Amount);
                        }

                        service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp,
                              userIserial);

                        // Cost Center
                        if (item.TblJournalAccountType != null && item.EntityAccount != null)
                        {
                            var storeCostcenter = new TblGlRuleDetail();
                            storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                                context);
                            service.CreateTblLedgerDetailCostCenter(context, item.Amount,
                                bankLedgerDetail, storeCostcenter);
                        }
                        #endregion
                    }
                    // payable or expenses debit store cridit
                    if (headerType.Iserial == (int)CashDepositType.Expences)
                    {
                        // payable or expenses Entity
                        #region payable or expenses

                        var entity = ExpencesEntities.FirstOrDefault(e =>
                            e.Iserial == item.EntityAccount && e.TblJournalAccountType == item.TblJournalAccountType);

                        var ledgerDetail = getLedgerDetail(item.BatchDate, newRow.Sequance, true,
                            newLedgerHeaderRow.Iserial, item, entity, item.TblJournalAccountType, item.Amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, ledgerDetail, true, 0, out tmp, userIserial);

                        // Cost Center
                        if (item.TblJournalAccountType != 14)
                        {
                            var storeCostcenter = new TblGlRuleDetail();
                            storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                                context);
                            service.CreateTblLedgerDetailCostCenter(context, item.Amount,
                                ledgerDetail, storeCostcenter);
                        }
                        #endregion
                    }
                    // bank and expenses debit store cridit
                    if (headerType.Iserial == (int)CashDepositType.Visa)
                    {
                        #region Visa
                        // Bank Entity
                        var entity = bankEntities.FirstOrDefault(e => e.Iserial == item.TblBank);
                        var machine = machines.FirstOrDefault(vm => vm.MachineId == item.MachineId);
                        if (machine == null) throw new ArgumentNullException(paramName: "machineId", message: "No machine found");
                        item.MachineId = machine.MachineId;
                        item.DiscountPercent = machine.DiscountPercent;
                        var amount = item.Amount * (1 - (machine.DiscountPercent / 100));
                        var _ref = item.MachineId + "-" + @ref;
                        decimal x = 0;
                        if (decimal.TryParse(@ref, out x))
                            _ref = item.MachineId + "-" + Convert.ToDecimal(@ref).ToString("000000");
                        var bankLedgerDetail = getLedgerDetail(item.BatchDate, _ref, true,
                            newLedgerHeaderRow.Iserial, item, entity, 6, amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp, userIserial);

                        // Expenses Entity                
                        var visaExpensesEntity = visaExpensesEntities.FirstOrDefault(e =>
                            e.Iserial == machine.EntityAccount);

                        var visaExpensesLedgerDetail = getLedgerDetail(item.BatchDate, _ref, true,
                            newLedgerHeaderRow.Iserial, item, visaExpensesEntity, 15, item.Amount - amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, visaExpensesLedgerDetail, true, 0, out tmp, userIserial);

                        // Cost Center
                        var storeCostcenter = new TblGlRuleDetail();
                        storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                            context);
                        service.CreateTblLedgerDetailCostCenter(context, visaExpensesLedgerDetail.Amount ?? 0,
                            visaExpensesLedgerDetail, storeCostcenter);
                        #endregion
                    }

                    if (headerType.Iserial == (int)CashDepositType.DSquaresCIB)
                    {
                        #region DSquaresCIB
                        // Bank Entity
                        var entity = context.Entities.FirstOrDefault(e => e.Iserial == item.EntityAccount && e.TblJournalAccountType == item.TblJournalAccountType && e.scope == 0);
                        decimal itemDiscountPercent = 1;
                        if (item.TblJournalAccountType == 6)
                        {
                            var discountpercentage = item.DiscountPercent ?? 0;
                            itemDiscountPercent = (discountpercentage / 100);
                            var amountLedger = item.Amount * itemDiscountPercent;

                            var setting=CashDepositSetting.FirstOrDefault(w => w.TblJournalAccountType == item.TblJournalAccountType && w.EntityAccount == item.EntityAccount);

                            //var discountpercentage = setting.DiscountPercent??0;
                            //itemDiscountPercent = (discountpercentage / 100);
                            //var amountLedger = item.Amount * itemDiscountPercent;


                            if (setting!=null)
                            {
                                var disquareEntity = context.Entities.FirstOrDefault(e => e.Iserial == setting.DiscountEntityAccount && e.TblJournalAccountType == setting.DiscountJournalAccountType && e.scope == 0);
                                if (disquareEntity!=null)
                                {
                                    var LedgerDetailAccount = getLedgerDetail(item.BatchDate, "", true,
                                                         newLedgerHeaderRow.Iserial, item, disquareEntity, disquareEntity.TblJournalAccountType, amountLedger);

                                    service.UpdateOrInsertTblLedgerMainDetails(context, LedgerDetailAccount, true, 0, out tmp, userIserial);

                                    var storeCostcenter = new TblGlRuleDetail();
                                    storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                                        context);
                                    service.CreateTblLedgerDetailCostCenter(context, LedgerDetailAccount.Amount ?? 0,
                                        LedgerDetailAccount, storeCostcenter);
                                }
                            }
                            itemDiscountPercent = 1 - (itemDiscountPercent);                            
                        }
                        var amount = item.Amount * itemDiscountPercent;

                        var bankLedgerDetail = getLedgerDetail(item.BatchDate, "", true,
                            newLedgerHeaderRow.Iserial, item, entity, item.TblJournalAccountType, amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp, userIserial);
                  
                        // Expenses Entity                
                        //var visaExpensesEntity = visaExpensesEntities.FirstOrDefault(e =>
                        //    e.Iserial == machine.EntityAccount);

                        //var visaExpensesLedgerDetail = getLedgerDetail(item.BatchDate, _ref, true,
                        //    newLedgerHeaderRow.Iserial, item, visaExpensesEntity, 15, item.Amount - amount);
                        //service.UpdateOrInsertTblLedgerMainDetails(context, visaExpensesLedgerDetail, true, 0, out tmp, userIserial);

                        // Cost Center

                        #endregion
                    }

                    if (headerType.Iserial == (int)CashDepositType.DsquaresLuckyWallet)
                    {
                        #region DsquaresLuckyWallet
                        // Bank Entity
                        var entity = context.Entities.FirstOrDefault(e => e.Iserial == item.EntityAccount && e.TblJournalAccountType == item.TblJournalAccountType && e.scope == 0);
                        decimal itemDiscountPercent = 1;
                        if (item.TblJournalAccountType == 6)
                        {
                            var discountpercentage = item.DiscountPercent ?? 0;
                            itemDiscountPercent = (discountpercentage / 100);
                            var amountLedger = item.Amount * itemDiscountPercent;

                            var setting = CashDepositSetting.FirstOrDefault(w => w.TblJournalAccountType == item.TblJournalAccountType && w.EntityAccount == item.EntityAccount);

                            //var discountpercentage = setting.DiscountPercent??0;
                            //itemDiscountPercent = (discountpercentage / 100);
                            //var amountLedger = item.Amount * itemDiscountPercent;


                            if (setting != null)
                            {
                                var disquareEntity = context.Entities.FirstOrDefault(e => e.Iserial == setting.DiscountEntityAccount && e.TblJournalAccountType == setting.DiscountJournalAccountType && e.scope == 0);
                                if (disquareEntity != null)
                                {
                                    var LedgerDetailAccount = getLedgerDetail(item.BatchDate, "", true,
                                                         newLedgerHeaderRow.Iserial, item, disquareEntity, disquareEntity.TblJournalAccountType, amountLedger);

                                    service.UpdateOrInsertTblLedgerMainDetails(context, LedgerDetailAccount, true, 0, out tmp, userIserial);

                                    var storeCostcenter = new TblGlRuleDetail();
                                    storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                                        context);
                                    service.CreateTblLedgerDetailCostCenter(context, LedgerDetailAccount.Amount ?? 0,
                                        LedgerDetailAccount, storeCostcenter);
                                }
                            }
                            itemDiscountPercent = 1 - (itemDiscountPercent);
                        }
                        var amount = item.Amount * itemDiscountPercent;

                        var bankLedgerDetail = getLedgerDetail(item.BatchDate, "", true,
                            newLedgerHeaderRow.Iserial, item, entity, item.TblJournalAccountType, amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp, userIserial);

                        // Expenses Entity                
                        //var visaExpensesEntity = visaExpensesEntities.FirstOrDefault(e =>
                        //    e.Iserial == machine.EntityAccount);

                        //var visaExpensesLedgerDetail = getLedgerDetail(item.BatchDate, _ref, true,
                        //    newLedgerHeaderRow.Iserial, item, visaExpensesEntity, 15, item.Amount - amount);
                        //service.UpdateOrInsertTblLedgerMainDetails(context, visaExpensesLedgerDetail, true, 0, out tmp, userIserial);

                        // Cost Center

                        #endregion
                    }

                    if (headerType.Iserial == (int)CashDepositType.TFKCourier)
                    {
                        #region TFKCourier
                        // Bank Entity
                        var entity = context.Entities.FirstOrDefault(e => e.Iserial == item.EntityAccount && e.TblJournalAccountType == item.TblJournalAccountType && e.scope == 0);
                        decimal itemDiscountPercent = 1;
                        if (item.TblJournalAccountType == 6)
                        {
                            var discountpercentage = item.DiscountPercent ?? 0;
                            itemDiscountPercent = (discountpercentage / 100);
                            var amountLedger = item.Amount * itemDiscountPercent;

                            var setting = CashDepositSetting.FirstOrDefault(w => w.TblJournalAccountType == item.TblJournalAccountType
                                                                            && w.EntityAccount == item.EntityAccount);

                            //var discountpercentage = setting.DiscountPercent??0;
                            //itemDiscountPercent = (discountpercentage / 100);
                            //var amountLedger = item.Amount * itemDiscountPercent;


                            if (setting != null)
                            {
                                var disquareEntity = context.Entities.FirstOrDefault(e => e.Iserial == setting.DiscountEntityAccount
                                                                                        && e.TblJournalAccountType == setting.DiscountJournalAccountType
                                                                                        && e.scope == 0);
                                if (disquareEntity != null)
                                {
                                    var LedgerDetailAccount = getLedgerDetail(item.BatchDate, "", true,
                                                         newLedgerHeaderRow.Iserial, item, disquareEntity, disquareEntity.TblJournalAccountType, amountLedger);

                                    service.UpdateOrInsertTblLedgerMainDetails(context, LedgerDetailAccount, true, 0, out tmp, userIserial);

                                    var storeCostcenter = new TblGlRuleDetail();
                                    storeCostcenter = service.FindCostCenterByType(storeCostcenter, 8, newRow.TblStore,
                                        context);
                                    service.CreateTblLedgerDetailCostCenter(context, LedgerDetailAccount.Amount ?? 0,
                                        LedgerDetailAccount, storeCostcenter);
                                }
                            }
                            itemDiscountPercent = 1 - (itemDiscountPercent);
                        }
                        var amount = item.Amount * itemDiscountPercent;

                        var bankLedgerDetail = getLedgerDetail(item.BatchDate, "", true,
                            newLedgerHeaderRow.Iserial, item, entity, item.TblJournalAccountType, amount);
                        service.UpdateOrInsertTblLedgerMainDetails(context, bankLedgerDetail, true, 0, out tmp, userIserial);

                        // Expenses Entity                
                        //var visaExpensesEntity = visaExpensesEntities.FirstOrDefault(e =>
                        //    e.Iserial == machine.EntityAccount);

                        //var visaExpensesLedgerDetail = getLedgerDetail(item.BatchDate, _ref, true,
                        //    newLedgerHeaderRow.Iserial, item, visaExpensesEntity, 15, item.Amount - amount);
                        //service.UpdateOrInsertTblLedgerMainDetails(context, visaExpensesLedgerDetail, true, 0, out tmp, userIserial);

                        // Cost Center

                        #endregion
                    }

                   
                }
        }
        private TblLedgerMainDetail getLedgerDetail(DateTime? DocDate, string PaymentRef, bool DOrC,
            int LedgerHeaderIserial, TblCashDepositDetail item, Entity entity, int? JournalAccountType,
            decimal Amount)
        {
            TblLedgerMainDetail storeLedgerDetail;
            storeLedgerDetail = new TblLedgerMainDetail
            {
                Amount = Amount,
                Description = item.LedgerDescription ?? "",
                ExchangeRate = 1,
                TblCurrency = 1,
                TransDate = DocDate,
                TblJournalAccountType = JournalAccountType,
                EntityAccount = entity.Iserial,//Entity Iserial
                GlAccount = entity.AccountIserial,// Account Iserial
                TblLedgerHeader = LedgerHeaderIserial,
                PaymentRef = PaymentRef,
                DrOrCr = DOrC// true debit or false for credit
            };
            return storeLedgerDetail;
        }
        private TblLedgerMainDetail getLedgerDetail(DateTime? DocDate, string PaymentRef, bool DOrC,
                   int LedgerHeaderIserial, TblCashDepositHeader item, Entity entity, int? JournalAccountType, decimal Amount)
        {
            TblLedgerMainDetail storeLedgerDetail;
            storeLedgerDetail = new TblLedgerMainDetail
            {
                Amount = Amount,
                Description = item.LedgerDescription ?? "",
                ExchangeRate = 1,
                TblCurrency = 1,
                TransDate = DocDate,

                TblJournalAccountType = JournalAccountType,
                EntityAccount = entity.Iserial,//Entity Iserial
                GlAccount = entity.AccountIserial,// Account Iserial
                TblLedgerHeader = LedgerHeaderIserial,
                PaymentRef = PaymentRef,
                DrOrCr = DOrC// true debit or false for credit
            };
            return storeLedgerDetail;
        }
        //[OperationContract]
        private int DeleteCashDepositHeader(TblCashDepositHeader row, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCashDepositHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblCashDepositAmountDetail> GetCashDepositAmountDetail(int skip, int take, int headerId, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var query = context.TblCashDepositAmountDetails.Where(v =>
                    v.TblCashDepositHeader == headerId).OrderBy(x => x.Iserial).Skip(skip).Take(take);
                var result = query.ToList();
                return result;
            }
        }

        [OperationContract]
        private TblCashDepositAmountDetail UpdateOrInsertCashDepositAmountDetail(TblCashDepositAmountDetail newRow,
            int index, int userIserial, string company, out int outindex)
        {
            outindex = index;
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCashDepositAmountDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {//context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblCashDepositAmountDetails.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private List<TblCashDepositDetail> GetCashDepositDetail(int skip, int take, int headerId,
            string company, out List<Entity> EntityAccounts)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var query = context.TblCashDepositDetails.Include(nameof(TblCashDepositDetail.TblCashDepositHeader1
                    )).Include(nameof(TblCashDepositDetail.TblBank1)).Where(v =>
                    v.TblCashDepositHeader == headerId).OrderBy(x => x.Iserial).Skip(skip).Take(take);
                var result = query.ToList();
                List<int?> intList = query.Select(x => x.EntityAccount).ToList();
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                EntityAccounts = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                return result;
            }
        }

        [OperationContract]
        private TblCashDepositDetail UpdateOrInsertCashDepositDetail(TblCashDepositDetail newRow,
            int index, int userIserial, string company, out int outindex)
        {
            outindex = index;
            newRow.TblBank1 = null;
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCashDepositDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                    if (result.Count() > 0)
                    {
                        oldRow.LastChangeDate = DateTime.Now.Date;
                        oldRow.LastChangeUser = userIserial;
                    }
                }
                else
                {
                    newRow.LastChangeDate = DateTime.Now.Date;
                    newRow.LastChangeUser = userIserial;
                    context.TblCashDepositDetails.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }
        [OperationContract]
        private List<TblCashDepositDetail> UpdateOrInsertCashDepositDetails(
            List<TblCashDepositDetail> newRow, int userIserial, string company)
        {
            var result = new List<TblCashDepositDetail>();
            foreach (var item in newRow)
            {
                int x = 0;
                result.Add(UpdateOrInsertCashDepositDetail(item, 0, userIserial, company, out x));
            }
            return result;
        }

        [OperationContract]
        private TblCashDepositDetail DeleteCashDepositDetail(TblCashDepositDetail row, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblCashDepositDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private TblVisaMachine GetMachineId(int Store, int Bank, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                
                 var result = context.TblStoreVisaMachines.Include(nameof(TblStoreVisaMachine.TblVisaMachine1)).FirstOrDefault(r => r.TblStore == Store &&
                   r.TblVisaMachine1.TblBank == Bank);
                if (result != null && result.TblVisaMachine1 != null)
                    return result.TblVisaMachine1;
                else return new TblVisaMachine();
            }
        }

        [OperationContract]
        private int GetPremiumBankIserial(string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                int iserial = 0;
                var result = context.tblChainSetupTests.FirstOrDefault(r => r.sGlobalSettingCode == "PremiumBankIserial");
                if (result != null && result.sSetupValue != null)
                    int.TryParse(result.sSetupValue, out iserial);
                return iserial;
            }
        }

        [OperationContract]
        private int GetPremium2030BankIserial(string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                int iserial = 0;
                var result = context.tblChainSetupTests.FirstOrDefault(r => r.sGlobalSettingCode == "PremiumBank2030Iserial");
                if (result != null && result.sSetupValue != null)
                    int.TryParse(result.sSetupValue, out iserial);
                return iserial;
            }
        }

        [OperationContract]
        private int GetTFKDiscountBankIserial(string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                int iserial = 0;
                var result = context.tblChainSetupTests.FirstOrDefault(r => r.sGlobalSettingCode == "TFKDiscount15BankIserial");
                if (result != null && result.sSetupValue != null)
                    int.TryParse(result.sSetupValue, out iserial);
                return iserial;
            }
        }

        [OperationContract]
        private List<TblCashDepositSetting> GetCashDepositSetting(string company, out List<Entity> entityList)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var query = context.TblCashDepositSettings.ToList();
                List<int?> intList = query.Select(x => x.EntityAccount).ToList();
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                entityList = context.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                return query;
            }
        }
    }
}