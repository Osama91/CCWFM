using CCWFM.Models.Gl;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel;

namespace CCWFM.Web.Service.BankStatOp
{
    public partial class BankStatService
    {
        int roundFraction = 2;
        [OperationContract]
        private List<BankStatMatchingModel> GetBankStatDetailForMatching(int headerId, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var query = context.TblBankStatDetails.Where(v =>
                    v.TblBankStatHeader == headerId && !v.TblBankStatHeader1.MatchApproved && !context.TblBankStatDetailTblLedgerDetails.Any(bsd =>
                     bsd.TblBankStatDetail == v.Iserial)).Select(bsd => new BankStatMatchingModel()
                    {
                        Iserial = bsd.Iserial,
                        IsChecked = false,
                        DocDate = bsd.DocDate,
                        Description =bsd.Description,
                        Amount = bsd.Amount,
                        ChequeNo = bsd.ChequeNo,
                        DepositNo = bsd.DepositNo,
                    });

                var result = query.ToList();
                //result.ForEach(r => r.Amount = Math.Round(r.Amount, roundFraction));
                return result;
            }
        }
        [OperationContract]
        private List<BankStatMatchingModel> GetLedgerDetailForMatching(int headerId, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var header = context.TblBankStatHeaders.Include(nameof(TblBankStatHeader.TblCurrency1)).FirstOrDefault(bs => bs.Iserial == headerId && !bs.MatchApproved);
                var query = new List<BankStatMatchingModel>().AsQueryable();
                if (header != null)
                    query = context.TblLedgerMainDetails.Where(v => v.TransDate <= header.ReconcileDate &&
                        (v.TblCurrency == header.TblCurrency || v.TblCurrency1.ExchangeRate == header.TblCurrency1.ExchangeRate) &&
                        v.TblJournalAccountType == 6 &&
                        v.EntityAccount == header.TblBank && !context.TblBankStatDetailTblLedgerDetails.Any(bsdld =>
                            bsdld.TblLedgerMainDetail == v.Iserial)).Select(bsd =>
                            new BankStatMatchingModel()
                            {
                                IsLedger = true,
                                Iserial = bsd.Iserial,
                                IsChecked = false,
                                DocDate = bsd.TransDate ?? bsd.TblLedgerHeader1.DocDate,
                                Description = bsd.Description,
                                Amount = bsd.DrOrCr ? bsd.Amount.Value : (bsd.Amount.Value * -1),
                                ChequeNo = bsd.TblBankCheque1.Cheque,
                                DepositNo = bsd.PaymentRef,
                            });

                var result = query.ToList();
                //result.ForEach(r => r.Amount = Math.Round(r.Amount, roundFraction));
                return result;
            }
        }
        
        [OperationContract]
        private List<BankStatMatchingModel> GetBankStatDetailMatched(int headerId, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var query = context.TblBankStatDetails.Where(v =>
                    v.TblBankStatHeader == headerId && context.TblBankStatDetailTblLedgerDetails.Any(bsd =>
                    bsd.TblBankStatDetail == v.Iserial)).Select(bsd => new BankStatMatchingModel()
                    {
                        Iserial = bsd.Iserial,
                        IsChecked = false,
                        DocDate = bsd.DocDate,
                        Description = bsd.Description,
                        Amount = bsd.Amount,
                        ChequeNo = bsd.ChequeNo,
                        DepositNo = bsd.DepositNo,
                    });

                var result = query.ToList();
                return result;
            }
        }
        [OperationContract]
        private List<BankStatMatchingModel> GetLedgerDetailMatched(int headerId, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var header = context.TblBankStatHeaders.First(bs => bs.Iserial == headerId);
                var query = context.TblLedgerMainDetails.Where(v =>
                    v.TblJournalAccountType == 6 && v.EntityAccount == header.TblBank && context.TblBankStatDetailTblLedgerDetails.Any(bsdld =>
                    bsdld.TblLedgerMainDetail == v.Iserial && bsdld.TblBankStatDetail1.TblBankStatHeader == headerId)).Select(
                    bsd => new BankStatMatchingModel()
                    {
                        IsLedger = true,
                        Iserial = bsd.Iserial,
                        IsChecked = false,
                        DocDate = bsd.TransDate,
                        Description = bsd.Description,
                        Amount = bsd.DrOrCr ? bsd.Amount.Value : (bsd.Amount.Value * -1),
                        ChequeNo = bsd.TblBankCheque1.Cheque,
                        DepositNo = bsd.PaymentRef,
                    });

                var result = query.ToList();
                return result;
            }
        }


        [OperationContract]
        private List<BankStatMatchingModel> GetBankStatDetailMatchedByLedgerDetailId(
            int headerId, int ledgerDetailId, bool isChecked, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var query = context.TblBankStatDetails.Where(v => v.TblBankStatHeader == headerId &&
                    context.TblBankStatDetailTblLedgerDetails.Any(bsd => bsd.TblBankStatDetail == v.Iserial &&
                    bsd.TblLedgerMainDetail == ledgerDetailId)).Select(bsd =>
                      new BankStatMatchingModel()
                      {
                          Iserial = bsd.Iserial,
                          IsChecked = isChecked,
                          DocDate = bsd.DocDate,
                          Description = bsd.Description,
                          Amount = bsd.Amount,
                          ChequeNo = bsd.ChequeNo,
                          DepositNo = bsd.DepositNo,
                      });

                var result = query.ToList();
                return result;
            }
        }
        [OperationContract]
        private List<BankStatMatchingModel> GetLedgerDetailMatchedByBankStatDetailId(
            int headerId, int bankStatDetailId, bool isChecked, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var header = context.TblBankStatHeaders.First(bs => bs.Iserial == headerId);
                var query = context.TblLedgerMainDetails.Where(v => v.TblJournalAccountType == 6 &&
                    v.EntityAccount == header.TblBank &&
                    context.TblBankStatDetailTblLedgerDetails.Any(bsdld =>
                    bsdld.TblLedgerMainDetail == v.Iserial && bsdld.TblBankStatDetail == bankStatDetailId &&
                    bsdld.TblBankStatDetail1.TblBankStatHeader == headerId)).Select(
                    bsd => new BankStatMatchingModel()
                    {
                        IsLedger = true,
                        Iserial = bsd.Iserial,
                        IsChecked = isChecked,
                        DocDate = bsd.TransDate,
                        Description = bsd.Description,
                        Amount = bsd.DrOrCr ? bsd.Amount.Value : (bsd.Amount.Value * -1),
                        ChequeNo = bsd.TblBankCheque1.Cheque,
                        DepositNo = bsd.PaymentRef,
                    });

                var result = query.ToList();
                return result;
            }
        }

        [OperationContract]
        private bool InsertMatchedList(
            List<BankStatMatchingModel> BankStatDetailList,
            List<BankStatMatchingModel> LedgerDetailList,
            bool reload, out bool lastInsert, string company)
        {
            lastInsert = reload;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                try
                {
                    //var result = new List<BankStatMatchingModel>();
                    foreach (var itemBankStat in BankStatDetailList)
                    {
                        foreach (var itemLedger in LedgerDetailList)
                        {
                            //result.Add(itemLedger);
                            var newRec = new TblBankStatDetailTblLedgerDetail()
                            {
                                TblBankStatDetail = itemBankStat.Iserial,
                                TblLedgerMainDetail = itemLedger.Iserial,
                            };
                            newRec.Amount = itemLedger.Amount;
                            if (itemBankStat.Amount > itemLedger.Amount)
                                newRec.Amount = itemLedger.Amount;
                            if (itemLedger.Amount > itemBankStat.Amount)
                                newRec.Amount = itemBankStat.Amount;
                            context.TblBankStatDetailTblLedgerDetails.AddObject(newRec);
                        }
                        //result.Add(itemBankStat);
                    }
                    context.SaveChanges();
                }
                catch (System.Exception)
                {
                    throw;
                }                
                return true;// result;
            }
        }


        [OperationContract]
        private bool RemoveMatchedList(
            List<BankStatMatchingModel> BankStatDetailList,
            List<BankStatMatchingModel> LedgerDetailList,
            bool reload, out bool lastInsert, string company)
        {
            lastInsert = reload;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                foreach (var itemBankStat in BankStatDetailList)
                {
                    foreach (var itemLedger in LedgerDetailList)
                    {
                        var tempRec = context.TblBankStatDetailTblLedgerDetails.FirstOrDefault(dl =>
                            dl.TblBankStatDetail == itemBankStat.Iserial &&
                            dl.TblLedgerMainDetail == itemLedger.Iserial);
                        if (tempRec != null)
                        {
                            context.DeleteObject(tempRec);
                        }
                    }
                }
                return context.SaveChanges() > 0;
            }
        }
    }
}