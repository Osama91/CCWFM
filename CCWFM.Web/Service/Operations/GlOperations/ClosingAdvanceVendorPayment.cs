using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System;
using System.Data.Entity;
namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblGlChequeTransactionDetail> GetTblChequeTransactionDetailNotLinked(string vendor, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                List<int> ChequeStatusList = new List<int>();
                ChequeStatusList.Add(1);
                ChequeStatusList.Add(2);
                IQueryable<TblGlChequeTransactionDetail> query;
                var entityRecord = entity.Entities.FirstOrDefault(w => w.Code == vendor && w.TblJournalAccountType == 3).Iserial;

                var cheque = entity.TblClosingAdvanceVendorPayments.Include(nameof(TblClosingAdvanceVendorPayment.TblGlChequeTransactionDetail)).Select(w => w.TblGlChequeTransactionDetail1.TblBankCheque).ToList();
                query = entity.TblGlChequeTransactionDetails.Where(x => 
                x.EntityDetail1 == entityRecord 
                &&x.TblGlChequeTransactionHeader1.TblGlChequeTypeSetting== 3
                &&  x.EntityDetail1TblJournalAccountType == 3 
                   && ChequeStatusList.Contains(x.TblBankCheque1.TblGlChequeStatus)
                   && x.TblGlChequeTransactionHeader1.Approved == true
                   && ChequeStatusList.Contains(x.TblGlChequeStatus ?? 0) && !cheque.Contains(x.TblBankCheque)
                );

                return query.ToList();
            }
        }
        [OperationContract]
        private void UpdateorInsertClosingAdvanceVendorPayments(DateTime PostDate,List<int> Iserials, int CreatedBy, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var tblChainSetupTest =
                     entity.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GLAdvanceVendor");
                var sequenceIserial = entity.TblJournals.FirstOrDefault(e => e.Code == tblChainSetupTest.sSetupValue).Iserial;
                entity.CommandTimeout = 0;         
                var items = entity.TblGlChequeTransactionDetails.Include(w => w.TblGlChequeTransactionHeader1).Where(w => Iserials.Contains(w.Iserial)).ToList();
                var entityID = items.FirstOrDefault().EntityDetail1;
                var entityRecord = entity.Entities.FirstOrDefault(w => w.Iserial == entityID && w.TblJournalAccountType == 3);
              
                foreach (var item in items)
                {
                    var newLedgerHeaderRow = new TblLedgerHeader
                    {
                        CreatedBy = CreatedBy,
                        CreationDate = DateTime.Now,
                        Description = " تسوية دفعة مورد" + entityRecord.Code + " " + entityRecord.Ename,
                        DocDate = PostDate,
                        TblJournal = sequenceIserial,
                        TblTransactionType = 7,
                        TblJournalLink = 1
                    };
                    var newrow = new TblClosingAdvanceVendorPayment() { TblGlChequeTransactionDetail = item.Iserial };
                    entity.TblClosingAdvanceVendorPayments.AddObject(newrow);

                    var tempheader = 0;
                    UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, (int)newLedgerHeaderRow.CreatedBy,
                              company);

                    var newledgerDetailrowh1 = new TblLedgerMainDetail
                    {
                        Amount = (decimal?)item.Amount,
                        Description = " تسوية دفعة مورد" + entityRecord.Ename + " رقم شيك" + item.ChequeNo,
                        ExchangeRate = items.FirstOrDefault().TblGlChequeTransactionHeader1.ExchangeRate,
                        TblCurrency = items.FirstOrDefault().TblGlChequeTransactionHeader1.TblCurrency,
                        TransDate = DateTime.Now,
                        TblJournalAccountType = entityRecord.TblJournalAccountType,
                        EntityAccount = entityRecord.Iserial,
                        GlAccount = entityRecord.AccountIserial,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = item.ChequeNo,
                        DrOrCr = false,
                        // TblBankCheque = row.TblBankCheque,
                    };
                    var vendorRecord = entity.Entities.FirstOrDefault(w => w.Code == entityRecord.Code && w.TblJournalAccountType == 2);

                    var newledgerDetail = new TblLedgerMainDetail
                    {
                        Amount = (decimal?)item.Amount,
                        Description = " تسوية دفعة مورد" + entityRecord.Ename + " رقم شيك" + item.ChequeNo,
                        ExchangeRate = items.FirstOrDefault().TblGlChequeTransactionHeader1.ExchangeRate,
                        TblCurrency = items.FirstOrDefault().TblGlChequeTransactionHeader1.TblCurrency,
                        TransDate = DateTime.Now,
                        TblJournalAccountType = vendorRecord.TblJournalAccountType,
                        EntityAccount = vendorRecord.Iserial,
                        GlAccount = vendorRecord.AccountIserial,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = item.ChequeNo,
                        DrOrCr = true,
                        // TblBankCheque = row.TblBankCheque,
                    };

                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company,
                     (int)newLedgerHeaderRow.CreatedBy);

                    UpdateOrInsertTblLedgerMainDetails(newledgerDetail, true, 000, out tempheader, company,
                 (int)newLedgerHeaderRow.CreatedBy);
                }
                entity.SaveChanges();
                //return query.ToList();
            }
        }
    }
}