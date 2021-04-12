using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.Transactions;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {       
        [OperationContract]
        private TblRecInvHeader PostInv(TblRecInvHeader row, int user, string company)
        {
            
            using (var scope = new TransactionScope())
            {
                short? Lang = 0;
                using (var entity = new WorkFlowManagerDBEntities())
                {
                    Lang = entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user).CurrLang;
                }

                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    entity.CommandTimeout = 0;
                    var query = entity.TblRecInvHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Purchase TransNo " + row.SupplierInv;

                    if (row.TblRecInvHeaderType == 2)
                    {
                        desc = "Return Purchase TransNo " + row.SupplierInv;
                    }
                    if (Lang == 0)
                    {
                        desc = "فاتورة المشتريات رقم " + row.SupplierInv;

                        if (row.TblRecInvHeaderType == 2)
                        {
                            desc = " فاتورة مرتجع المشتريات رقم " + row.SupplierInv;
                        }
                    }
                    var markuptrans =
                        entity.TblMarkupTrans.Include("TblMarkup1.TblMarkupGroup1")
                            .Include("TBLsupplier1")
                            .Where(x => x.TblRecInv == row.Iserial && x.Type == 0);

                    var cost =
                        entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == row.Iserial).Sum(w => w.Cost * w.Qty);

                    decimal totalWithItemEffect = 0;
                    decimal totalWithoutItemEffect = 0;
                    foreach (var variable in markuptrans)
                    {
                        if (variable.TblMarkup1.ItemEffect == false)
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithoutItemEffect =
                                  (totalWithoutItemEffect + (cost * ((decimal)variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithoutItemEffect = (totalWithoutItemEffect + (decimal)variable.MiscValue);
                            }
                        }
                        else
                        {
                            if (variable.MiscValueType == 0)
                            {
                                totalWithItemEffect = (totalWithItemEffect + (cost * ((decimal)variable.MiscValue / 100)));
                            }
                            else
                            {
                                totalWithItemEffect = (totalWithItemEffect + (decimal)variable.MiscValue);
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
                                entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == row.Iserial).ToList();
                            if (queryDetail.All(w=>w.Misc==0))
                            {
                                foreach (var variable in queryDetail)
                                {
                                    variable.Misc = (variable.Cost / cost) * totalWithItemEffect;
                                }
                            }
                         
                        }
                        entity.SaveChanges();
                        if (query != null)
                        {
                            query.Status = 1;
                            query.TblUser = user;
                            query.PostDate = DateTime.Now;

                            // var ledgercode = entity.tblChainSetupTests.FirstOrDefault(x => x.sSetupValue == "GLSalesInventory").sSetupValue;
                            var journal =
                                entity.tblChainSetupTests.FirstOrDefault(
                                    x => x.sGlobalSettingCode == "GLPurchaseJournal").sSetupValue;
                            var tablename =
                                entity.tblChainSetupTests.FirstOrDefault(
                                    x => x.sGlobalSettingCode == "GlItemGroupTableName").sSetupValue;
                            if (company=="MAN"|| company == "Sw" || company=="CA" || company=="NC" || company == "vintex")
                            {
                                tablename = "TblItemDownLoadDef";
                            }
                            if (tablename == "TblItemDownLoadDef")
                            {
                                tablename = "ItemStoreGroup";
                            }
                            int journalint = entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                            var newLedgerHeaderRow = new TblLedgerHeader
                            {
                                CreatedBy = user,
                                CreationDate = DateTime.Now,
                                Description = desc,
                                DocDate = row.TransDate,
                                TblJournal = journalint,
                                TblTransactionType = 1,
                                TblJournalLink = query.Iserial
                            };
                            int temp;

                            UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 0, out temp, user, company);

                            var sqlParam = new List<SqlParameter>
                            {
                                new SqlParameter
                                {
                                    ParameterName = "Table_Name",
                                    Value = tablename,
                                    SqlDbType = SqlDbType.NVarChar
                                },

                                new SqlParameter
                                {
                                    ParameterName = "Iserial",
                                    Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
                                    SqlDbType = SqlDbType.NVarChar
                                },
                            };
                            var list =
                                entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlRecinvPostingToGl @Table_Name, @Iserial",
                                    sqlParam.ToArray()).ToList();
                            //var store =
                            //    entity.Entities.FirstOrDefault(
                            //        x => x.Iserial == query.TblStore && x.TblJournalAccountType == 8);

                            //var storeCostcenter = new TblGlRuleDetail();
                            //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, (int)store.Iserial, company);
                            #region MarkupTrans
                            foreach (var rr in markuptrans)
                            {
                                var glAccount =
                                    entity.Entities.FirstOrDefault(
                                        x => x.Iserial == rr.TblMarkup && x.TblJournalAccountType == 9).AccountIserial;
                                var vendorAccountMarkUp =
                                    entity.Entities.FirstOrDefault(
                                        x => x.Iserial == rr.TblSupplier && x.TblJournalAccountType == 2);

                                var drorCr = true;
                                decimal? total = 0;
                                if (rr.MiscValueType == 0)
                                {
                                    total = (total + cost * ((decimal)rr.MiscValue / 100)) * rr.TblMarkup1.TblMarkupGroup1.Direction;
                                }
                                else
                                {
                                    total = (total + (decimal)rr.MiscValue) * rr.TblMarkup1.TblMarkupGroup1.Direction;
                                }
                                if (total > 0)
                                {
                                    drorCr = false;
                                }
                                var markupdes = rr.TblMarkup1.Ename +" "+ row.SupplierInv + " " + vendorAccountMarkUp.Ename;
                                if (Lang == 0)
                                {
                                    markupdes = rr.TblMarkup1.Aname + " " +  row.SupplierInv + " " + vendorAccountMarkUp.Aname;
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
                                    ExchangeRate = rr.ExchangeRate,
                                    TblCurrency = rr.TblCurrency,
                                    TransDate = row.TransDate,
                                    TblJournalAccountType = 0,
                                    EntityAccount = glAccount,
                                    GlAccount = glAccount,
                                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                    PaymentRef = query.SupplierInv,
                                    DrOrCr = !drorCr
                                };

                                if (row.TblRecInvHeaderType == 2)
                                {
                                    markupVendorDiscount.DrOrCr = !markupVendorDiscount.DrOrCr;
                                }
                                UpdateOrInsertTblLedgerMainDetails(markupVendorDiscount, true, 000, out temp, company,
                                    user);

                                //            CreateTblLedgerDetailCostCenter(company, (decimal)total, markupVendorDiscount, storeCostcenter);

                                if (glAccount != 0)
                                {
                                    var markupVendor = new TblLedgerMainDetail
                                    {
                                        Amount = totalModified,
                                        Description = markupdes,
                                        ExchangeRate = rr.ExchangeRate,
                                        TblCurrency = rr.TblCurrency,
                                        TransDate = row.TransDate,
                                        TblJournalAccountType = 2,
                                        EntityAccount = vendorAccountMarkUp.Iserial,
                                        GlAccount = vendorAccountMarkUp.AccountIserial,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = query.SupplierInv,
                                        DrOrCr = drorCr
                                    };
                                    if (row.TblRecInvHeaderType == 2)
                                    {
                                        markupVendor.DrOrCr = !markupVendor.DrOrCr;
                                    }
                                    UpdateOrInsertTblLedgerMainDetails(markupVendor, true, 000, out temp, company, user);

                                    //CreateTblLedgerDetailCostCenter(company, (decimal)total, markupVendor, storeCostcenter);

                                    foreach (var variable in list)
                                    {
                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName,
                                            company);

                                        if (costcenter==null)
                                        {
                                            throw new Exception("Cost Center Not Linked");
                                        }

                                        var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                                        {
                                            Ratio = 0,
                                            TblLedgerMainDetail = markupVendor.Iserial,
                                            Amount = (double)(markupVendor.Amount * variable.CostPercentage),
                                            TblCostCenter = costcenter.TblCostCenter,
                                            TblCostCenterType = costcenter.TblCostCenter1.TblCostCenterType,
                                        };
                                        UpdateOrInsertTblLedgerDetailCostCenters(markupVendorLedgerCostCenter, true, 000,
                                            out temp,user, company);
                                    }
                                }
                            }
                            #endregion

                            foreach (var rr in list.GroupBy(x => x.GroupName))
                            {
                                
                                PostInvPurchaseAndTax(query, newLedgerHeaderRow, rr, company, user, list, desc);
                            }
                            CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                        }

                        entity.SaveChanges();
                        scope.Complete();
                        return query;
                    }
                    return null;
                }
            }
        }

        public void PostInvPurchaseAndTax(TblRecInvHeader query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                //Inventory, receipt
                var groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == rr.Key && x.TblInventAccountType == 60);
                if (groupAccount == null)
                {
                    groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                }

                //Purchase, packing slip tax
                var temproww = entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == rr.Key && x.TblInventAccountType == 87);
                var salesgroupAccount = 0;
                if (temproww != null)
                {
                    salesgroupAccount = temproww.TblAccount;
                }

                if (salesgroupAccount == 0)
                {
                    salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                           x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 87).TblAccount;
                }
                int temp = 0;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Iserial == query.TblSupplier && x.TblJournalAccountType == 2).AccountIserial;

                var Acc = 0;
                if (groupAccount!=null)
                {
                    Acc = groupAccount.TblAccount;
                }

                if (query.TblAccount!=null)
                {
                    Acc = query.TblAccount??0;
                }

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = (decimal?)rr.Sum(x => x.CostWithoutMisc),
                    Description = desc,
                    ExchangeRate = 1,
                    TblCurrency = DefaultCurrency(company),
                    TransDate = query.TransDate,
                    TblJournalAccountType = 0,
                    EntityAccount = Acc,
                    GlAccount = Acc,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = true
                };

                if (query.TblRecInvHeaderType == 2)
                {
                    newledgerDetailrow.DrOrCr = false;
                }
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                //string tax = "Tax";
                //if (Lang == 0)
                //{
                //    tax = "ضريبة";
                //}
                //var salestax = entity.TblSTaxes.FirstOrDefault().TaxPres / 100;
                //var newledgerDetailrowTax = new TblLedgerMainDetail
                //{
                //    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) * salestax),
                //    Description = tax,
                //    ExchangeRate = 1,
                //    TblCurrency = rr.FirstOrDefault().TblCurrency,
                //    TransDate = query.TransDate,
                //    TblJournalAccountType = 0,
                //    EntityAccount = salesgroupAccount,
                //    GlAccount = salesgroupAccount,
                //    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                //    PaymentRef = query.SupplierInv,
                //    DrOrCr = true
                //};
                //if (query.TblRecInvHeaderType == 2)
                //{
                //    newledgerDetailrowTax.DrOrCr = false;
                //}
                //UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowTax, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc)),
                    Description = desc,
                    ExchangeRate = 1,
                    TblCurrency = DefaultCurrency(company),
                    TransDate = query.TransDate,
                    TblJournalAccountType = 2,
                    EntityAccount = query.TblSupplier,
                    GlAccount = vendorAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = false
                };

                if (query.TblRecInvHeaderType == 2)
                {
                    vendorLedgerDetail.DrOrCr = true;
                }
                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                {
                    //var storeCostcenter = new TblGlRuleDetail();
                    //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, (int)query.TblStore, company);
                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, storeCostcenter);

                    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                    //  storeCostcenter);

                    // 20/3/2016
                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                    //    costcenter);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, storeCostcenter);
                    // 20/3/2016
                    // CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, costcenter);
                }
            }
        }

        public void PostInvPurchaseAndTax(TblRecInvHeaderProd query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                decimal amount = 0;
                if (query.TblRecInvHeaderTypeProd == 2)
                {
                    amount = rr.Sum(x => x.Cost) * -1;
                }
                else
                {
                    amount = rr.Sum(x => x.Cost);
                }

                decimal amountWithoutMisc = 0;
                if (query.TblRecInvHeaderTypeProd == 2)
                {
                    amountWithoutMisc = rr.Sum(x => x.CostWithoutMisc) * -1;
                }
                else
                {
                    amountWithoutMisc = rr.Sum(x => x.CostWithoutMisc);
                }
                //Inventory, receipt
                var groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == rr.Key && x.TblInventAccountType == 60);
                if (groupAccount == null)
                {
                    groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                }

                int temp = 0;
                var currencyIserial = rr.FirstOrDefault().TblCurrency;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Code == query.TblSupplier && x.TblJournalAccountType == 2);
                var currencyrow = entity.TblCurrencyTests.FirstOrDefault(w => w.Iserial == currencyIserial);
                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = amount,
                    Description = desc,
                    ExchangeRate = rr.FirstOrDefault().ExchangeRate,
                    TblCurrency = rr.FirstOrDefault().TblCurrency,
                    TransDate = query.TransDate,
                    TblJournalAccountType = 0,
                    EntityAccount = groupAccount.TblAccount,
                    GlAccount = groupAccount.TblAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = true
                };

                if (query.TblRecInvHeaderTypeProd == 2)
                {
                    newledgerDetailrow.DrOrCr = false;
                }
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)amountWithoutMisc,
                    Description = desc,
                    ExchangeRate = list.FirstOrDefault().ExchangeRate,
                    TblCurrency = list.FirstOrDefault().TblCurrency,
                    TransDate = query.TransDate,
                    TblJournalAccountType = 2,
                    EntityAccount = vendorAccount.Iserial,
                    GlAccount = vendorAccount.AccountIserial,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = false
                };

                if (query.TblRecInvHeaderTypeProd == 2)
                {
                    vendorLedgerDetail.DrOrCr = true;
                }
                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                {
                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);

                    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);
                }
            }
        }

        public void PostInvPurchaseAndTax(RouteCardInvoiceHeader query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {

                //Inventory, receipt
                var groupAccount =
                    entity.TblInventPostings.FirstOrDefault(x => x.ItemScopeRelation == 4000 + rr.Key && x.TblInventAccountType == 60);
                if (groupAccount == null)
                {
                    groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                }

                var currencyIserial = rr.FirstOrDefault().TblCurrency;
                var currencyrow = entity.TblCurrencyTests.FirstOrDefault(w => w.Iserial == currencyIserial);


                int temp = 0;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Code == query.Vendor && x.TblJournalAccountType == 2);

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = (decimal?)rr.Sum(x => x.Cost),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = rr.FirstOrDefault().TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 0,
                    EntityAccount = groupAccount.TblAccount,
                    GlAccount = groupAccount.TblAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = true
                };

                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc)),
                    Description = desc,
                    ExchangeRate = 1,
                    TblCurrency = list.FirstOrDefault().TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 2,
                    EntityAccount = vendorAccount.Iserial,
                    GlAccount = vendorAccount.AccountIserial,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = false
                };

                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                {
                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);

                    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);
                }
            }
        }

        public void PostInvPurchaseAndTax(TblDyeingOrderInvoiceHeader query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                //Inventory, receipt
                var groupAccount =
                    entity.TblInventPostings.FirstOrDefault(x => x.ItemScopeRelation == rr.Key && x.TblInventAccountType == 60);
                if (groupAccount == null)
                {
                    groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                }
                int temp = 0;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Code == query.Vendor && x.TblJournalAccountType == 2);

                var currencyIserial = rr.FirstOrDefault().TblCurrency;
                var currencyrow = entity.TblCurrencyTests.FirstOrDefault(w => w.Iserial == currencyIserial);


                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = (decimal?)rr.Sum(x => x.Cost),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = rr.FirstOrDefault().TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 0,
                    EntityAccount = groupAccount.TblAccount,
                    GlAccount = groupAccount.TblAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = true
                };

                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc)),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = list.FirstOrDefault().TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 2,
                    EntityAccount = vendorAccount.Iserial,
                    GlAccount = vendorAccount.AccountIserial,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = false
                };

                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                {
                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);
                    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);
                }
            }
        }

        public void PostInvPurchaseAndTax(TblSalesOrderRequestInvoiceHeader query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var currencyrow = entity.TblCurrencyTests.First(w => w.Iserial == query.TblCurrency);

                var groupAccount = 0;


                if (groupAccount == 0)
                {
                    groupAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 74).TblAccount;
                }

                int temp = 0;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Iserial == query.EntityAccount && x.TblJournalAccountType == query.TblJournalAccountType);

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = (decimal?)rr.Sum(x => x.Cost),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = query.TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 0,
                    EntityAccount = groupAccount,
                    GlAccount = groupAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = false
                };

                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                //string tax = "Tax";
                //if (Lang == 0)
                //{
                //    tax = "ضريبة";
                //}
                //var salestax = entity.TblSTaxes.FirstOrDefault().TaxPres / 100;
                //var newledgerDetailrowTax = new TblLedgerMainDetail
                //{
                //    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) * salestax),
                //    Description = tax,
                //    ExchangeRate = 1,
                //    TblCurrency = rr.FirstOrDefault().TblCurrency,
                //    TransDate = query.TransDate,
                //    TblJournalAccountType = 0,
                //    EntityAccount = salesgroupAccount,
                //    GlAccount = salesgroupAccount,
                //    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                //    PaymentRef = query.SupplierInv,
                //    DrOrCr = true
                //};
                //if (query.TblRecInvHeaderType == 2)
                //{
                //    newledgerDetailrowTax.DrOrCr = false;
                //}
                //UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowTax, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc)),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = query.TblCurrency,
                    TransDate = query.DocDate,
                    TblJournalAccountType = query.TblJournalAccountType,
                    EntityAccount = vendorAccount.Iserial,
                    GlAccount = vendorAccount.AccountIserial,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.Code,
                    DrOrCr = true
                };

                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                //foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                //{
                //    //var storeCostcenter = new TblGlRuleDetail();
                //    //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, (int)query.TblStore, company);
                //    var costcenter = new TblGlRuleDetail();
                //    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);

                //    //CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, storeCostcenter);

                //    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);

                //    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                //    //  storeCostcenter);

                //    // 20/3/2016
                //    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                //    //    costcenter);

                //    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, storeCostcenter);
                //    // 20/3/2016
                //    // CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, costcenter);
                //}
            }
        }

        public void PostInvPurchaseAndTax(TblProductionOrderInvoiceHeader query, TblLedgerHeader newLedgerHeaderRow, IGrouping<int?, GlGroupsDtp> rr, string company, int user, List<GlGroupsDtp> list, string desc)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                //Inventory, receipt
                var groupAccount =
                    entity.TblInventPostings.FirstOrDefault(x => x.ItemScopeRelation == 3000 + rr.Key && x.TblInventAccountType == 60);
                if (groupAccount == null)
                {
                    groupAccount =
                    entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                }


                var currencyrow = entity.TblCurrencyTests.FirstOrDefault(w => w.Iserial == query.TblCurrency);
                int temp = 0;
                var vendorAccount = entity.Entities.FirstOrDefault(x => x.Iserial == query.EntityAccount && x.TblJournalAccountType == query.TblJournalAccountType).AccountIserial;

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = (decimal?)rr.Sum(x => x.Cost),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = currencyrow.Iserial,
                    TransDate = query.DocDate,
                    TblJournalAccountType = 0,
                    EntityAccount = groupAccount.TblAccount,
                    GlAccount = groupAccount.TblAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = true
                };

                //if (query.TblRecInvHeaderType == 2)
                //{
                //    newledgerDetailrow.DrOrCr = false;
                //}
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                //string tax = "Tax";
                //if (Lang == 0)
                //{
                //    tax = "ضريبة";
                //}
                //var salestax = entity.TblSTaxes.FirstOrDefault().TaxPres / 100;
                //var newledgerDetailrowTax = new TblLedgerMainDetail
                //{
                //    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) * salestax),
                //    Description = tax,
                //    ExchangeRate = 1,
                //    TblCurrency = rr.FirstOrDefault().TblCurrency,
                //    TransDate = query.TransDate,
                //    TblJournalAccountType = 0,
                //    EntityAccount = salesgroupAccount,
                //    GlAccount = salesgroupAccount,
                //    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                //    PaymentRef = query.SupplierInv,
                //    DrOrCr = true
                //};
                //if (query.TblRecInvHeaderType == 2)
                //{
                //    newledgerDetailrowTax.DrOrCr = false;
                //}
                //UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowTax, true, 000, out temp, company, user);
                var vendorLedgerDetail = new TblLedgerMainDetail
                {
                    // 20/3/2016
                    //Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc) + ((rr.Sum(x => x.CostWithoutMisc) * salestax))),
                    Amount = (decimal?)(rr.Sum(x => x.CostWithoutMisc)),
                    Description = desc,
                    ExchangeRate = currencyrow.ExchangeRate,
                    TblCurrency = currencyrow.Iserial,
                    TransDate = query.DocDate,
                    TblJournalAccountType = query.TblJournalAccountType,
                    EntityAccount = query.EntityAccount,
                    GlAccount = vendorAccount,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = query.SupplierInv,
                    DrOrCr = false
                };

                //if (query.TblRecInvHeaderType == 2)
                //{
                //    vendorLedgerDetail.DrOrCr = true;
                //}
                UpdateOrInsertTblLedgerMainDetails(vendorLedgerDetail, true, 000, out temp, company, user);

                foreach (var variable in list.Where(x => x.GroupName == rr.Key))
                {
                    //var storeCostcenter = new TblGlRuleDetail();
                    //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, (int)query.TblStore, company);
                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, (int)variable.GroupName, company);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, storeCostcenter);

                    CreateTblLedgerDetailCostCenter(company, (decimal)variable.Cost, newledgerDetailrow, costcenter);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                    //  storeCostcenter);

                    // 20/3/2016
                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc * salestax), newledgerDetailrowTax,
                    //    costcenter);

                    //CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, storeCostcenter);
                    // 20/3/2016
                    // CreateTblLedgerDetailCostCenter(company, (decimal)(variable.CostWithoutMisc + (variable.CostWithoutMisc * salestax)), vendorLedgerDetail, costcenter);
                }
            }
        }
        [OperationContract]
        private List<TblRecInvHeader> GetTblRecInvHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblRecInvHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblRecInvHeaders.Include("TblAccount1").Include("TblStore1").Include("TblRecInvHeaderType1").Include("TBLsupplier1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblRecInvHeaders.Count();
                    query = entity.TblRecInvHeaders.Include("TblAccount1").Include("TblStore1").Include("TblRecInvHeaderType1").Include("TBLsupplier1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblRecInvMainDetail> GetTblRecInvMainDetail(int skip, int take, int RecInvHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company,out List<RecInvItem> Items
            , out decimal TotalQty, out decimal TotalAmount)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                Items = new List<RecInvItem>();
              
                List<TblRecInvMainDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeader ==(@Group0)";
                    valuesObjects.Add("Group0", RecInvHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvMainDetails.Where(filter, parameterCollection.ToArray()).Count();
                    if (company == "MAN"|| company== "Sw" || company == "CA" ||company== "NC" || company == "vintex")
                    {
                        query = entity.TblRecInvMainDetails.Include("TblCurrency1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take).ToList();
                    }
                    else
                    {
                        query = entity.TblRecInvMainDetails.Include("TBLITEMprice.TblColor1").Include("TBLITEMprice.TblSize1").Include("TblCurrency1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take).ToList();
                    }
                }
                else
                {
                    fullCount = entity.TblRecInvMainDetails.Count(v => v.TblRecInvHeader == RecInvHeader);
                    if (company == "MAN" || company == "Sw" || company == "CA" || company == "NC" || company == "vintex")
                    {
                        query = entity.TblRecInvMainDetails.Include("TblCurrency1").OrderBy(sort).Where(v => v.TblRecInvHeader == RecInvHeader).Skip(skip).Take(take).ToList();
                        var queryItem = query.Select(w => w.TblItem).ToList();
                        Items = entity.RecInvItems.Where(x => queryItem.Any(l => x.iserial == l)).ToList();
                    

                    }
                    else
                    {
                        query = entity.TblRecInvMainDetails.Include("TBLITEMprice.TblColor1").Include("TBLITEMprice.TblSize1").Include("TblCurrency1").OrderBy(sort).Where(v => v.TblRecInvHeader == RecInvHeader).Skip(skip).Take(take).ToList();

                    }
                }
                TotalQty = entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == RecInvHeader).Sum(x => x.Qty);
                TotalAmount = entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == RecInvHeader).Sum(x => x.Qty * x.Cost);

                return query;
            }
        }

        [OperationContract]
        private List<TblRecInvDetail> GetTblRecInvDetail(int skip, int take, int RecInvHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblRecInvDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeader ==(@Group0)";
                    valuesObjects.Add("Group0", RecInvHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvDetails.Where(filter, parameterCollection).Count();
                    query = entity.TblRecInvDetails.Include("TblCostCenter1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblRecInvDetails.Count(v => v.TblRecInvHeader == RecInvHeader);
                    query = entity.TblRecInvDetails.Include("TblCostCenter1").Include("TblCurrency1").Include("TblJournalAccountType1").Include("TblJournalAccountType2").OrderBy(sort).Where(v => v.TblRecInvHeader == RecInvHeader).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblRecInvHeader UpdateOrInsertTblRecInvHeaders(TblRecInvHeader newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblRecInvHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvHeaders
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
        private TblRecInvMainDetail UpdateOrInsertTblRecInvMainDetail(TblRecInvMainDetail newRow, bool save, int index, out int outindex, string company, out decimal TotalQty, out decimal TotalAmount)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblRecInvMainDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvMainDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                        var row =
                            entity.TblRecInvDetails.Where(
                                x => x.TblRecInvHeader == newRow.TblRecInvHeader && x.TBLITEMprice.ISerial == newRow.TblItem).ToList();

                        foreach (var tblRecInvMainDetail in row)
                        {
                            tblRecInvMainDetail.Cost = newRow.Cost;
                            tblRecInvMainDetail.Misc = newRow.Misc;
                        }
                    }
                }

                entity.SaveChanges();
                TotalQty = entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == newRow.TblRecInvHeader).Sum(x => x.Qty);
                TotalAmount = entity.TblRecInvMainDetails.Where(x => x.TblRecInvHeader == newRow.TblRecInvHeader).Sum(x => x.Qty * x.Cost);

                return newRow;
            }
        }

        [OperationContract]
        private TblRecInvDetail UpdateOrInsertTblRecInvDetails(TblRecInvDetail newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblRecInvDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvDetails
                                  where e.Glserial == newRow.Glserial
                                  && e.Dserial == newRow.Dserial
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
        private int DeleteTblRecInvHeader(TblRecInvHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblRecInvHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                entity.CommandTimeout = 0;
                if (query != null)
                {
                    entity.deleteRecInvHeader(query.Iserial);
                }
            }
            return row.Iserial;
        }

        [OperationContract]
        private int DeleteTblRecInvMainDetail(TblRecInvMainDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblRecInvMainDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblRecInvDetail DeleteTblRecInvDetail(TblRecInvDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblRecInvDetails
                             where e.Glserial == row.Glserial
                                 && e.Dserial == row.Dserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private List<RecieveView> GetTblRecieveHeader(int skip, int take, int type, int supplier, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var iserials =
                    entity.TblRecInvHeaderLinks.Where(x => x.TblRecInvHeader1.TblRecInvHeaderType == type && x.TblRecInvHeader1.TblSupplier == supplier)
                        .Select(x => x.tblrecieveHeader);
                IQueryable<RecieveView> query;

                if (filter != null)
                {
                    foreach (var variable in iserials)
                    {
                        filter = filter + " and it.glserial !=(@Group" + variable + ")";
                        valuesObjects.Add("Group" + variable + "", variable);
                    }
                    filter = filter + " and it.tblsupplier ==(@Sup)";
                    valuesObjects.Add("Sup", supplier);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = entity.RecieveViews.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        entity.RecieveViews
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount =
                        entity.RecieveViews.Count(
                            x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier);
                    query =
                        entity.RecieveViews
                            .OrderBy(sort)
                            .Where(x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier)
                            .Skip(skip)
                            .Take(take);
                }
                return query.ToList();
                    
            }
        }

        [OperationContract]
        private List<ReturnRecieveView> GetTblReturnHeader(int skip, int take, int type, int supplier, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var iserials =
                    entity.TblRecInvHeaderLinks.Where(x => x.TblRecInvHeader1.TblRecInvHeaderType == type)
                        .Select(x => x.tblrecieveHeader);
                IQueryable<ReturnRecieveView> query;

                if (filter != null)
                {
                    foreach (var variable in iserials)
                    {
                        filter = filter + " and it.glserial !=(@Group" + variable + ")";
                        valuesObjects.Add("Group" + variable + "", variable);
                    }
                    filter = filter + " and it.tblsupplier ==(@Sup)";
                    valuesObjects.Add("Sup", supplier);
                    //filter = filter + " and it.tblStore ==(@Store)";
                    //valuesObjects.Add("Store", tblStore);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = entity.ReturnRecieveViews.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        entity.ReturnRecieveViews
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount =
                        entity.ReturnRecieveViews.Count(
                            x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier);
                    query =
                        entity.ReturnRecieveViews
                            .OrderBy(sort)
                            .Where(x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier)
                            .Skip(skip)
                            .Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<RecieveView> GetTblRecieveHeaderFromTo(DateTime From, DateTime To, int type, int supplier, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                To = To.AddDays(1);
                var iserials =
                    entity.TblRecInvHeaderLinks.Where(x => x.TblRecInvHeader1.TblRecInvHeaderType == type)
                        .Select(x => x.tblrecieveHeader);
                IQueryable<RecieveView> query;

                query =
                    entity.RecieveViews

                        .Where(x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier
                        && x.votdate >= From && x.votdate < To);

                return query.ToList();
            }
        }

        [OperationContract]
        private List<ReturnRecieveView> GetTblReturnHeaderFromTo(DateTime From, DateTime To, int type, int supplier, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                To = To.AddDays(1);
                var iserials =
                    entity.TblRecInvHeaderLinks.Where(x => x.TblRecInvHeader1.TblRecInvHeaderType == type)
                        .Select(x => x.tblrecieveHeader);
                IQueryable<ReturnRecieveView> query;

                query =
                    entity.ReturnRecieveViews

                        .Where(x => !iserials.Contains(x.glserial) && x.tblsupplier == supplier
                         && x.votdate >= From && x.votdate < To);

                return query.ToList();
            }
        }

        private string HandelSequence(string code, string company)
        {
            var temp = "";
            var tempFormat = "";
            const char aa = '0';

            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var chainSeq = entity.tblChainSetupTests.FirstOrDefault(x => x.sSetupValue == "InvoiceSequence");
                var seq = entity.TblSequences.FirstOrDefault(x => x.Code == chainSeq.sSetupValue);

                if (seq.Manual && !string.IsNullOrEmpty(code))
                {
                    if (Find("TblRecInvHeader", code, entity))
                    {
                        throw new Exception("Already Exists");
                    }
                }
                else
                {
                    for (var i = 0; i < seq.NumberOfInt; i++)
                    {
                        tempFormat = tempFormat + aa;
                    }
                    temp = seq.NextRec.ToString(tempFormat) + seq.Format;

                    if (seq != null) seq.NextRec = seq.NextRec + 1;
                }

                entity.SaveChanges();
            }
            return temp;
        }

        [OperationContract]
        private TblRecInvHeader GetTblRecieveDetail(List<int> headers, TblRecInvHeader tblRecInvHeader, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                tblRecInvHeader.Code = HandelSequence(tblRecInvHeader.Code, company);
                tblRecInvHeader.CreationDate = DateTime.Now;
                entity.TblRecInvHeaders.AddObject(tblRecInvHeader);
                entity.SaveChanges();
                foreach (var header in headers)
                {
                    entity.TblRecInvHeaderLinks.AddObject(new TblRecInvHeaderLink
                    {
                        TblRecInvHeader = tblRecInvHeader.Iserial,
                        tblrecieveHeader = header,
                        TblRecInvHeaderType = tblRecInvHeader.TblRecInvHeaderType
                    });
                }
                var tblChainSetting = entity.TblChainSettings.FirstOrDefault();
                if (tblChainSetting != null)
                {
                    if (tblChainSetting.CurrCurrency != null)
                    {
                        var currency = (int)tblChainSetting.CurrCurrency;

                        string comand = "SELECT '' BatchNo,Style,ccitemview.TblColorCode ColorCode,ccitemview.TblColorEname ColorName,TblsizeCode SizeCode,cast(SUM(TBLrecieveDetail.Quantity* TBLrecieveDetail.ucostwot)/SUM(TBLrecieveDetail.Quantity) as decimal(19,4)) Cost , cast(SUM(TBLrecieveDetail.Quantity) as decimal(19,4))" +
                                      " Quantity,ccitemview.Iserial FROM TBLrecieveDetail inner join ccitemview on ccitemview.ISerial= TBLrecieveDetail.tblitem  where TBLrecieveDetail.glserial in" +
                                      " ({0}) group by ccitemview.Style,TblColorCode,TblColorEname,ccitemview.TblsizeCode,ccitemview.Iserial";

                        comand = comand.Replace("{0}", string.Join(",", headers));

                        List<RecInvDataTable> List = entity.ExecuteStoreQuery<RecInvDataTable>(comand).ToList();
                        foreach (var row in List)
                        {                        
                                var newRow = new TblRecInvMainDetail
                                {
                                    Cost = row.Cost,
                                    Misc=0,
                                    TblCurrency = currency,
                                    Qty = row.Quantity,
                                    TblItem = row.Iserial,
                                    TblRecInvHeader = tblRecInvHeader.Iserial,
                                };
                                entity.TblRecInvMainDetails.AddObject(newRow);                         
                        }                       
                    }
                    entity.RecieveDetailViews.MergeOption = MergeOption.NoTracking;
                    var query = entity.RecieveDetailViews.Where(x => headers.Contains(x.glserial)).ToList();

                    foreach (var row in query)
                    {
                        var recDetail = new TblRecInvDetail
                        {
                            Cost = row.ucostwot,
                            TblRecInvHeader = tblRecInvHeader.Iserial,
                            Dserial = (int)row.Dserial,
                            Flg = 0,
                            Misc=0,
                            Glserial = row.glserial,
                            Tblitem = (int)row.tblitem
                        };
                        entity.TblRecInvDetails.AddObject(recDetail);
                    }
                    entity.SaveChanges();
                }
            }
            return tblRecInvHeader;
        }

        [OperationContract]
        private TblRecInvHeader GetTblReturnDetail(List<int> headers, TblRecInvHeader tblRecInvHeader, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                tblRecInvHeader.Code = HandelSequence(tblRecInvHeader.Code, company);
                tblRecInvHeader.CreationDate = DateTime.Now;
                entity.TblRecInvHeaders.AddObject(tblRecInvHeader);
                entity.SaveChanges();
                foreach (var header in headers)
                {
                    entity.TblRecInvHeaderLinks.AddObject(new TblRecInvHeaderLink
                    {
                        TblRecInvHeader = tblRecInvHeader.Iserial,
                        tblrecieveHeader = header,
                        TblRecInvHeaderType = tblRecInvHeader.TblRecInvHeaderType
                    });
                }
                var tblChainSetting = entity.TblChainSettings.FirstOrDefault();
                if (tblChainSetting != null)
                {
                    if (tblChainSetting.CurrCurrency != null)
                    {
                        var currency = (int)tblChainSetting.CurrCurrency;

                        string comand = "SELECT '' BatchNo,Style,ccitemview.TblColorCode ColorCode,ccitemview.TblColorEname ColorName,TblsizeCode SizeCode,cast(SUM(TblReturnDetail.Quantity* TblReturnDetail.ucostwot)/SUM(TblReturnDetail.Quantity) as decimal(19,4)) Cost ,cast( SUM(TblReturnDetail.Quantity) as decimal(19,4))" +
                                      " Quantity,ccitemview.Iserial FROM TblReturnDetail inner join ccitemview on ccitemview.ISerial= TblReturnDetail.tblitem  where TblReturnDetail.glserial in" +
                                      " ({0}) group by ccitemview.Style,TblColorCode,TblColorEname,ccitemview.TblsizeCode,ccitemview.Iserial";

                        comand = comand.Replace("{0}", string.Join(",", headers));

                        List<RecInvDataTable> List = entity.ExecuteStoreQuery<RecInvDataTable>(comand).ToList();
                        foreach (var row in List)
                        {                                                           
                                var newRow = new TblRecInvMainDetail
                                {
                                    Cost = row.Cost,
                                    TblCurrency = currency,
                                    Qty = row.Quantity,
                                    TblItem = row.Iserial,
                                    TblRecInvHeader = tblRecInvHeader.Iserial,
                                };
                                entity.TblRecInvMainDetails.AddObject(newRow);                            
                        }
                    }
                    entity.ReturnDetailViews.MergeOption = MergeOption.NoTracking;
                    var query = entity.ReturnDetailViews.Where(x => headers.Contains(x.glserial));

                    foreach (var row in query)
                    {
                        var recDetail = new TblRecInvDetail
                        {
                            Cost = row.ucostwot,
                            TblRecInvHeader = tblRecInvHeader.Iserial,
                            Dserial = (int)row.Dserial,
                            Flg = 0,
                            Glserial = row.glserial,
                            Tblitem = (int)row.tblitem
                        };
                        entity.TblRecInvDetails.AddObject(recDetail);
                    }
                    entity.SaveChanges();
                }
            }
            return tblRecInvHeader;
        }

        [OperationContract]
        private List<RecInvStyleColor> GetRecInvStyleColor(int skip, int take, int recInvHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.RecInvStyleColors.MergeOption = MergeOption.NoTracking;
                IQueryable<RecInvStyleColor> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeader ==(@Group0)";
                    valuesObjects.Add("Group0", recInvHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.RecInvStyleColors.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.RecInvStyleColors.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.RecInvStyleColors.Count(v => v.TblRecInvHeader == recInvHeader);
                    query = entity.RecInvStyleColors.OrderBy(sort).Where(v => v.TblRecInvHeader == recInvHeader).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<RecInvStyle> GetRecInvStyle(int skip, int take, int recInvHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.RecInvStyles.MergeOption = MergeOption.NoTracking;

                IQueryable<RecInvStyle> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeader ==(@Group0)";
                    valuesObjects.Add("Group0", recInvHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.RecInvStyles.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.RecInvStyles.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.RecInvStyles.Count(v => v.TblRecInvHeader == recInvHeader);
                    query = entity.RecInvStyles.OrderBy(sort).Where(v => v.TblRecInvHeader == recInvHeader).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private void UpdateCostInRecInv(RecInvStyle styleRow, RecInvStyleColor colorRow, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (styleRow != null)
                {
                    var row =
                        entity.TblRecInvMainDetails.Where(
                            x => x.TblRecInvHeader == styleRow.TblRecInvHeader && x.TBLITEMprice.Style == styleRow.Style).ToList();

                    foreach (var tblRecInvMainDetail in row)
                    {
                        tblRecInvMainDetail.Cost = styleRow.Cost??0;
                        tblRecInvMainDetail.Misc = styleRow.Misc;
                    }

                    var rowDetil =
                       entity.TblRecInvDetails.Where(
                           x => x.TblRecInvHeader == styleRow.TblRecInvHeader && x.TBLITEMprice.Style == styleRow.Style).ToList();
                    foreach (var tblRecInvMainDetail in rowDetil)
                    {
                        tblRecInvMainDetail.Cost = styleRow.Cost;
                        tblRecInvMainDetail.Misc = styleRow.Misc;
                    }
                }
                else
                {
                    var row =
                     entity.TblRecInvMainDetails.Where(
                         x => x.TblRecInvHeader == colorRow.TblRecInvHeader && x.TBLITEMprice.Style == colorRow.Style && x.TBLITEMprice.TblColor1.Code == colorRow.ColorCode).ToList();

                    foreach (var tblRecInvMainDetail in row)
                    {
                        tblRecInvMainDetail.Cost = colorRow.Cost??0;
                        tblRecInvMainDetail.Misc = colorRow.Misc;
                    }

                    var rowDetil =
                   entity.TblRecInvDetails.Where(
                       x => x.TblRecInvHeader == colorRow.TblRecInvHeader && x.TBLITEMprice.Style == colorRow.Style && x.TBLITEMprice.TblColor1.Code == colorRow.ColorCode).ToList();
                    foreach (var tblRecInvMainDetail in rowDetil)
                    {
                        tblRecInvMainDetail.Cost = colorRow.Cost;
                        tblRecInvMainDetail.Misc = colorRow.Misc;
                    }
                }
                entity.SaveChanges();
            }
        }
    }
}