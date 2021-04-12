using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Transactions;
using CCWFM.Web.Service.Operations.GlOperations;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using CCWFM.Web.Service.Operations;
using LinqKit;

namespace CCWFM.Web.Service.Dyeing
{
    public partial class DyeingService
    {
        [DataContract]
        private class DyeingOrderInvoiceDetailDto
        {
            [DataMember]
            public string DyedFabric { get; set; }
            [DataMember]
            public string ColorCode { get; set; }
            [DataMember]
            public string SupplierInv { get; set; }
            [DataMember]
            public string ServiceCode { get; set; }
            [DataMember]
            public string ServiceName { get; set; }

            [DataMember]
            public int TransID { get; set; }

            [DataMember]
            public int TblColor { get; set; }

            [DataMember]
            public int TblService { get; set; }

            [DataMember]
            public int TblDyeingOrdersMainDetails { get; set; }

            [DataMember]
            public int DyeingOrderInvoiceHeader { get; set; }

            [DataMember]
            public double Qty { get; set; }
            [DataMember]
            public double? Cost { get; set; }
            [DataMember]
            public int BatchNo { get; set; }
        };

        [OperationContract]
        private List<TblDyeingOrderInvoiceHeader> GetDyeingOrderInvoiceHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<Vendor> Vendors)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingOrderInvoiceHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblDyeingOrderInvoiceHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblDyeingOrderInvoiceHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblDyeingOrderInvoiceHeaders.Count();
                    query = context.TblDyeingOrderInvoiceHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                var codes = query.Select(w => w.Vendor).ToList();
                Vendors = context.Vendors.Where(x => codes.Contains(x.vendor_code)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private List<DyeingOrderInvoiceDetailDto> SearchDyeingOrderInvoice(string vendor, DateTime? fromDate, DateTime? toDate, int transId, string supplierInvoice)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var predicate = PredicateBuilder.True<DyeingOrderDetailsService>();

                if (!string.IsNullOrEmpty(supplierInvoice))
                    predicate = predicate.And(i => i.TblDyeingOrdersDetail.SupplierInv == supplierInvoice);


                if (!string.IsNullOrEmpty(vendor))
                    predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.Vendor == vendor);

                if (fromDate != null)
                    predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.TransactionDate >= fromDate);
                if (toDate != null)
                    predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersHeader.TransactionDate <= toDate);
                predicate = predicate.And(i => i.TblDyeingOrdersDetail.TransactionType == 1);
                if (transId != 0)
                    predicate = predicate.And(i => i.TblDyeingOrdersDetail.TblDyeingOrdersMainDetail.Iserial == transId);

                predicate = predicate.And(i => i.TblDyeingOrdersDetail.TransactionType == 1 &&
                 !context.TblDyeingOrderInvoiceDetails.Any(e => e.TblDyeingOrdersMainDetails == i.TblDyeingOrdersDetail.Iserial));
                var query = (from p in context.DyeingOrderDetailsServices.AsExpandable()
                    .Where(predicate)
                    .GroupBy(e =>
                            new
                            {
                                e.TblDyeingOrdersDetail.TblDyeingOrdersMainDetail.Iserial,
                                e.TblDyeingOrdersDetail.Color,
                                e.ServiceCode,
                                e.ServiceName,
                                e.TblDyeingOrdersDetail.SupplierInv,
                                e.TblDyeingOrdersDetail.BatchNo,
                                e.TblDyeingOrdersDetail.DyedFabric
                            })
                             select new DyeingOrderInvoiceDetailDto
                             {
                                 Qty = p.Sum(v => v.TblDyeingOrdersDetail.CalculatedTotalQty),
                                 TblDyeingOrdersMainDetails = p.Key.Iserial,
                                 DyeingOrderInvoiceHeader = 0,
                                 TblColor = context.TblColors.FirstOrDefault(w => w.Code == p.Key.Color && w.TblLkpColorGroup != 24).Iserial,
                                 ColorCode = p.Key.Color,
                                 TblService = context.TblServices.FirstOrDefault(w => w.Code == p.Key.ServiceCode).Iserial,
                                 ServiceCode = p.Key.ServiceCode,
                                 TransID = p.Key.Iserial,
                                 Cost = p.Sum(v => v.Qty),
                                 ServiceName = p.Key.ServiceName,
                                 SupplierInv = p.Key.SupplierInv,
                                 BatchNo = p.Key.BatchNo,
                                 DyedFabric = p.Key.DyedFabric
                             }).ToList();
                return query;
            }
        }

        [OperationContract]
        private TblDyeingOrderInvoiceHeader UpdateOrInsertDyeingOrderInvoiceHeader(TblDyeingOrderInvoiceHeader newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var seqCode = SharedOperation.GetChainSetup("GlDyeingPostSequence");
                    var seqProd = context.TblSequenceProductions.FirstOrDefault(w => w.Code == seqCode);
                    newRow.Code = SharedOperation.HandelSequence(seqProd);
                    context.TblDyeingOrderInvoiceHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblDyeingOrderInvoiceHeaders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteDyeingOrderInvoiceHeader(TblDyeingOrderInvoiceHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblDyeingOrderInvoiceHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblDyeingOrderInvoiceDetail> GetDyeingOrderInvoiceDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblDyeingOrderInvoiceDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblDyeingOrderInvoiceHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblDyeingOrderInvoiceDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblDyeingOrderInvoiceDetails.Include(nameof(TblDyeingOrderInvoiceDetail.TblColor1)).Include((nameof(TblDyeingOrderInvoiceDetail.TblService1))).Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblDyeingOrderInvoiceDetails.Count(v => v.TblDyeingOrderInvoiceHeader == groupId);
                    query = context.TblDyeingOrderInvoiceDetails.Include((nameof(TblDyeingOrderInvoiceDetail.TblColor1))).Include((nameof(TblDyeingOrderInvoiceDetail.TblService1))).OrderBy(sort).Where(v => v.TblDyeingOrderInvoiceHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblDyeingOrderInvoiceDetail UpdateOrInsertDyeingOrderInvoiceDetail(TblDyeingOrderInvoiceDetail newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {

                    context.TblDyeingOrderInvoiceDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblDyeingOrderInvoiceDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteDyeingOrderInvoiceDetail(TblDyeingOrderInvoiceDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblDyeingOrderInvoiceDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblDyeingOrderInvoiceHeader PostDyeingOrderInvoice(TblDyeingOrderInvoiceHeader row, int user, string company)
        {
            using (var scope = new TransactionScope())
            {
                short? lang = 0;
                using (var entity = new WorkFlowManagerDBEntities())
                {
                    var firstOrDefault = entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user);
                    if (firstOrDefault != null)
                        lang = firstOrDefault.CurrLang;

                    entity.CommandTimeout = 0;
                    var query = entity.TblDyeingOrderInvoiceHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Dyeing TransNo " + row.Code;


                    var markuptrans =
                        entity.TblDyeingOrderInvoiceMarkupTransProds.Include("TblMarkupProd1.TblMarkupGroupProd1")
                            .Where(x => x.TblDyeingOrderInvoiceHeader == row.Iserial && x.Type == 0);

                    var cost =
                        entity.TblDyeingOrderInvoiceDetails.Where(x => x.TblDyeingOrderInvoiceHeader == row.Iserial).Sum(w => w.Cost * w.Qty);

                    double totalWithItemEffect = 0;
                    double totalWithoutItemEffect = 0;
                    foreach (var variable in markuptrans)
                    {
                        if (variable.TblMarkupProd1.ItemEffect == false)
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithoutItemEffect = (double)(totalWithoutItemEffect + (cost * (variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithoutItemEffect = (double)(totalWithoutItemEffect + variable.MiscValue);
                            }
                        }
                        else
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithItemEffect = (double)(totalWithItemEffect + (cost * (variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithItemEffect = (double)(totalWithItemEffect + variable.MiscValue);
                            }
                        }
                    }
                    if (query != null)
                    {
                        query.MiscWithoutItemEffect = totalWithoutItemEffect;
                        query.Misc = totalWithItemEffect;
                        if (totalWithItemEffect != 0)
                        {
                            var queryDetail =
                                entity.TblDyeingOrderInvoiceDetails.Where(x => x.TblDyeingOrderInvoiceHeader == row.Iserial).ToList();
                            foreach (var variable in queryDetail)
                            {
                                variable.Misc = (variable.Cost / cost) * totalWithItemEffect;
                            }
                        }
                        entity.SaveChanges();
                        if (query != null)
                        {
                            query.Status = 1;
                            query.TblUser = user;
                            query.PostDate = DateTime.Now;
                            using (var db = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                            {
                                var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlDyeingInvoice").sSetupValue;

                                int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderProdRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = desc,
                                    DocDate = row.DocDate,
                                    TblJournal = journalint,
                                    TblTransactionType = 10,
                                    TblJournalLink = query.Iserial
                                };
                                int temp;
                                var glserive = new GlService();
                                glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);


                                var sqlParam = new List<SqlParameter>
                            {

                                new SqlParameter
                                {
                                    ParameterName = "Iserial",
                                    Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
                                    SqlDbType = SqlDbType.NVarChar
                                },
                            };

                                var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlDyeingOrderInvoicePostingToGl  @Iserial",
                                 sqlParam.ToArray()).ToList();
                                #region MarkUp

                                foreach (var rr in markuptrans)
                                {
                                    var currencyrow = db.TblCurrencyTests.First(w => w.Iserial == rr.TblCurrency);
                                    var glAccount =
                                        db.Entities.FirstOrDefault(
                                            x => x.Iserial == rr.TblMarkupProd && x.scope == 0 && x.TblJournalAccountType == 9).AccountIserial;
                                    var vendorAccountMarkUp =
                                        db.Entities.FirstOrDefault(
                                            x => x.Iserial == rr.EntityAccount && x.scope == 0 && x.TblJournalAccountType == rr.TblJournalAccountType);

                                    var drorCr = true;
                                    double? total = 0;
                                    if (rr.MiscValueType == 0)
                                    {
                                        total = (total + cost * (rr.MiscValue / 100)) * rr.TblMarkupProd1.TblMarkupGroupProd1.Direction;
                                    }
                                    else
                                    {
                                        total = (total + rr.MiscValue) * rr.TblMarkupProd1.TblMarkupGroupProd1.Direction;
                                    }
                                    if (total > 0)
                                    {
                                        drorCr = false;
                                    }
                                    var markupdes = rr.TblMarkupProd1.Ename + query.Code;
                                    if (lang == 0)
                                    {
                                        markupdes = rr.TblMarkupProd1.Aname + query.Code;
                                    }
                                    decimal totalModified = (decimal)total;
                                    if (total < 0)
                                    {
                                        totalModified = (decimal)(total * -1);
                                    }
                                    var markupVendorDiscount = new TblLedgerMainDetail();

                                    markupVendorDiscount = new TblLedgerMainDetail
                                    {
                                        Amount = totalModified,
                                        Description = markupdes,
                                        ExchangeRate = currencyrow.ExchangeRate,
                                        TblCurrency = rr.TblCurrency,
                                        TransDate = row.DocDate,
                                        TblJournalAccountType = 0,
                                        EntityAccount = glAccount,
                                        GlAccount = glAccount,
                                        TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                        PaymentRef = query.Code,
                                        DrOrCr = !drorCr
                                    };

                                    glserive.UpdateOrInsertTblLedgerMainDetails(markupVendorDiscount, true, 000, out temp, company,
                                         user);

                                    if (glAccount != 0)
                                    {
                                        var markupVendor = new TblLedgerMainDetail
                                        {
                                            Amount = totalModified,
                                            Description = markupdes,
                                            ExchangeRate = currencyrow.ExchangeRate,
                                            TblCurrency = rr.TblCurrency,
                                            TransDate = row.DocDate,
                                            TblJournalAccountType = rr.TblJournalAccountType,
                                            EntityAccount = vendorAccountMarkUp.Iserial,
                                            GlAccount = vendorAccountMarkUp.AccountIserial,
                                            TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                            PaymentRef = query.Code,
                                            DrOrCr = drorCr
                                        };

                                        glserive.UpdateOrInsertTblLedgerMainDetails(markupVendor, true, 000, out temp, company, user);
                                    }
                                }

                                #endregion MarkUp

                                foreach (var rr in list.GroupBy(x => x.GroupName))
                                {
                                    glserive.PostInvPurchaseAndTax(query, newLedgerHeaderProdRow, rr, company, user, list, desc);
                                }
                                glserive.CorrectLedgerHeaderRouding(newLedgerHeaderProdRow.Iserial, company, user);
                            }

                            entity.SaveChanges();
                            scope.Complete();
                        }
                        return query;
                    }
                    return null;
                }
            }
        }
    }
}