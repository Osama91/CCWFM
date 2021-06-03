using CCWFM.Web.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        internal void GetDailySalesCommision(
            int store, DateTime from, DateTime to, int userIserial, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int transactionType = 10;
            int journalint = Convert.ToInt32(
                   GetRetailChainSetupByCode("SalesCommissionJournalIserial", company).sSetupValue);// Code = 1
            var commissionAccount =
                  GetRetailChainSetupByCode("SalesCommissionAccount", company);
            var taxAccount =
                   GetRetailChainSetupByCode("EarnTaxAccount", company);

            Entity salesCommissionEntity, taxEntity;
            List<TblLedgerHeader> oldLedgers;
            List<GetSalesDailyCommission_Result> query;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                context.CommandTimeout = 0;
                salesCommissionEntity = context.Entities.FirstOrDefault(e => e.scope == 0 &&
                                                    e.Code == commissionAccount.sSetupValue &&
                                                    e.TblJournalAccountType == 15);
                taxEntity = context.Entities.FirstOrDefault(e => e.scope == 0 &&
                                 e.Code == taxAccount.sSetupValue &&
                                 e.TblJournalAccountType == 0);
                try
                {
                    query = context.GetSalesDailyCommission(store, from, to).ToList();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                //query = context.tblsalesdailycommisions.Where(r =>
                //   r.DocDate >= from && r.DocDate <= to);

                // var dquery = context.GetSalesDailyCommission(store, from, to).ToList().FirstOrDefault(x => x.tblstore == 173);


            }
            //var transactionOptions = new TransactionOptions
            //{
            //    IsolationLevel = IsolationLevel.ReadCommitted,
            //    Timeout = TimeSpan.FromSeconds(1200) //assume 10 min is the timeout time
            //};
            //using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(1200)))
            {
                using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                {
                    context.CommandTimeout = 0;
                    // Old Ledgers to be deleted
                    oldLedgers =
                           context.TblLedgerHeaders.Where(
                               x => x.TblTransactionType == transactionType && x.DocDate.Value >= from && x.DocDate.Value <= to).ToList();
                    // Delete old ledgers
                    foreach (var variable in oldLedgers)
                    {
                        context.TblLedgerHeaders.DeleteObject(variable);
                    }
                    context.SaveChanges();
                }
                foreach (var item in query.GroupBy(r => new { r.docdate }))
                // Parallel.ForEach(query.GroupBy(r => new { r.docdate }), item =>
                {
                    using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                    {
                        context.CommandTimeout = 0;
                        var description = string.Format("اثبات عمولات بيعية" + " {0}", item.Key.docdate.Value.ToString("dd/MM/yyyy"));
                        int sequence = 25;
                        var seq = context.TblSequences.FirstOrDefault(s => s.Iserial == sequence);

                        var newLedgerHeaderRow = new TblLedgerHeader
                        {
                            CreatedBy = userIserial,
                            CreationDate = DateTime.Now,
                            Description = description,
                            DocDate = item.Key.docdate,
                            TblJournal = journalint,
                            TblTransactionType = transactionType,
                            TblJournalLink = 0//item.Iserial
                        };
                        newLedgerHeaderRow.Code = SharedOperation.HandelSequence(seq, company, 0, 0, 0);
                        int tmp;
                        newLedgerHeaderRow = UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 0, out tmp, userIserial, company);//context,

                        decimal taxTotal = 0;
                        foreach (var innerItem in item)
                        {
                            var storeRec = context.TblStores
                                .Include(nameof(TblStore.TblStoreCommission))
                                .FirstOrDefault(s => s.iserial == innerItem.tblstore);

                            decimal taxPercent =
                                storeRec.TblStoreCommission.ManagerComm * (innerItem.MaxCommision ?? 0) / 100 * storeRec.TblStoreCommission.ManagerTax +
                                storeRec.TblStoreCommission.AssistantComm * (innerItem.MaxCommision ?? 0) / 100 * storeRec.TblStoreCommission.AssistantTax +
                                storeRec.TblStoreCommission.SalesManComm * (innerItem.MaxCommision ?? 0) / 100 * storeRec.TblStoreCommission.SalesManTax;
                            decimal commissionPercentTotal =
                                storeRec.TblStoreCommission.ManagerComm * (innerItem.MaxCommision ?? 0) / 100 +
                                storeRec.TblStoreCommission.AssistantComm * (innerItem.MaxCommision ?? 0) / 100 +
                                storeRec.TblStoreCommission.SalesManComm * (innerItem.MaxCommision ?? 0) / 100;
                            //decimal taxPercent =
                            //  storeRec.TblStoreCommission.ManagerComm  / 100 * storeRec.TblStoreCommission.ManagerTax +
                            //  storeRec.TblStoreCommission.AssistantComm  / 100 * storeRec.TblStoreCommission.AssistantTax +
                            //  storeRec.TblStoreCommission.SalesManComm  / 100 * storeRec.TblStoreCommission.SalesManTax;
                            //decimal commissionPercentTotal =
                            //    storeRec.TblStoreCommission.ManagerComm  / 100 +
                            //    storeRec.TblStoreCommission.AssistantComm  / 100 +
                            //    storeRec.TblStoreCommission.SalesManComm  / 100;
                            decimal commissionTotal = (innerItem.NetSalesAfterVAT ?? 0) * commissionPercentTotal;
                            if (commissionTotal == 0)
                            {
                                continue;
                            }
                            // Commission Entity
                            var commissionLedgerDetail = new TblLedgerMainDetail
                            {
                                Amount = commissionTotal,
                                Description = description,
                                ExchangeRate = 1,
                                TblCurrency = 1,
                                TransDate = innerItem.docdate,

                                TblJournalAccountType = 15,
                                EntityAccount = salesCommissionEntity.Iserial,//Entity Iserial
                                GlAccount = salesCommissionEntity.AccountIserial,// Account Iserial
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",//innerItem.Iserial.ToString(),
                                DrOrCr = true// true debit or false for credit
                            };
                            UpdateOrInsertTblLedgerMainDetails(commissionLedgerDetail, true, 0, out tmp, company, userIserial);//context,

                            decimal itemTax = (innerItem.NetSalesAfterVAT ?? 0) * taxPercent;

                            // Store Entity
                            var storeEntity = context.Entities.FirstOrDefault(e => e.scope == 0 &&
                                               e.Code == storeRec.code && e.TblJournalAccountType == 14);
                            if (storeEntity == null)
                            {
                                throw new Exception("store with code " + storeRec.code + " is not linked to Payable ");
                            }

                            var storeLedgerDetail = new TblLedgerMainDetail
                            {
                                Amount = commissionTotal - itemTax,
                                Description = description,
                                ExchangeRate = 1,
                                TblCurrency = 1,
                                TransDate = innerItem.docdate,
                                TblJournalAccountType = 14,
                                EntityAccount = storeEntity.Iserial,//Entity Iserial
                                GlAccount = storeEntity.AccountIserial,// Account Iserial
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",//innerItem.Iserial.ToString(),
                                DrOrCr = false// true debit or false for credit
                            };
                            UpdateOrInsertTblLedgerMainDetails(storeLedgerDetail, true, 0, out tmp, company, userIserial);//context,

                            // Cost Center
                            var storeCostcenter = new TblGlRuleDetail();
                            storeCostcenter = FindCostCenterByType(storeCostcenter, 8, innerItem.tblstore ?? 0, company);//, context
                            CreateTblLedgerDetailCostCenter(company, commissionLedgerDetail.Amount ?? 0,
                                commissionLedgerDetail, storeCostcenter);//, context

                            taxTotal += itemTax;
                        }

                        // Tax Entity
                        var taxLedgerDetail = new TblLedgerMainDetail
                        {
                            Amount = taxTotal,//item.Sum(r => r.Commision) * 0.05M,
                            Description = description,
                            ExchangeRate = 1,
                            TblCurrency = 1,
                            TransDate = item.Key.docdate,
                            TblJournalAccountType = 0,
                            EntityAccount = taxEntity.Iserial,//Entity Iserial
                            GlAccount = taxEntity.AccountIserial,// Account Iserial
                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                            PaymentRef = "0",//innerItem.Iserial.ToString(),
                            DrOrCr = false// true debit or false for credit
                        };
                        UpdateOrInsertTblLedgerMainDetails(taxLedgerDetail, true, 0, out tmp, company, userIserial);//context,

                    }
                }//);
            }
            //scope.Complete();
            stopwatch.Stop();
            Console.Error.WriteLine("Sequential loop time in milliseconds: {0}",
                                     stopwatch.ElapsedMilliseconds);
        }
    }
}