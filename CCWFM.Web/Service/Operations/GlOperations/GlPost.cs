using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Data.Objects;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        public void CreateTblLedgerDetailCostCenter(string company, decimal netSales, TblLedgerMainDetail Row, TblGlRuleDetail storeCostcenter)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                CreateTblLedgerDetailCostCenter(entity, netSales, Row, storeCostcenter);
            }
        }

        public void CreateTblLedgerDetailCostCenter(ccnewEntities entity, decimal netSales, TblLedgerMainDetail Row, TblGlRuleDetail storeCostcenter)
        {
            //using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            //{
           
            try
            {

                decimal? amount = netSales;
                var newrow = new TblLedgerDetailCostCenter
                {
                    Ratio = 0,
                    TblLedgerMainDetail = Row.Iserial,
                    Amount = (double)amount,
                    TblCostCenter = storeCostcenter.TblCostCenter,
                    TblCostCenterType = storeCostcenter.TblCostCenter1.TblCostCenterType
                };

                    entity.TblLedgerDetailCostCenters.AddObject(newrow);
                    entity.SaveChanges();
                }
                catch (Exception ex)  {
                    throw new Exception(ex.Message);
                }
           // }
               
            //}
        }

        public int DefaultCurrency(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var aa = 0;
                if (company == "MAN")
                {
                    aa = 1;
                }
                return aa;
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        [OperationContract]
        private void GetAccountByEntity(int entity, int tblJournalAccountType, string company, int scope, out int account)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var Entity = context.Entities.FirstOrDefault(
                    x => x.Iserial == entity && x.TblJournalAccountType == tblJournalAccountType && x.scope == scope);

                var entityType = context.TblJournalAccountTypes.FirstOrDefault(x => x.Iserial == tblJournalAccountType);

                account = context.Entities.FirstOrDefault(
                        x => x.Iserial == entity && x.TblJournalAccountType == tblJournalAccountType && x.scope == scope).AccountIserial;
                if (account == 0)
                {
                    throw new Exception("Check Entity Group and Entity in Posting Profile: " + entityType.Ename + "," + Entity.Code + "," + Entity.Ename);
                }
            }
        }

        [OperationContract]
        private void GetPostingTimes(string company, out DateTime? salesDate, out DateTime? transferDate,
            out DateTime? adjustDate, out DateTime? expensesDate, out DateTime? depreciationDate, out DateTime? commissionDate)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                salesDate = entity.TblLedgerHeaders.Where(
                    x => x.TblTransactionType == 2).Max(x => x.DocDate);
                transferDate = entity.TblLedgerHeaders.Where(
                    x => x.TblTransactionType == 3).Max(x => x.DocDate);

                adjustDate = entity.TblLedgerHeaders.Where(
                    x => x.TblTransactionType == 4).Max(x => x.DocDate);

                expensesDate = entity.TblLedgerHeaders.Where(
                    x => x.TblTransactionType == 5).Max(x => x.DocDate);
                depreciationDate = entity.TblLedgerHeaders.Where(
                x => x.TblTransactionType == 8).Max(x => x.DocDate);
                commissionDate = entity.TblLedgerHeaders.Where(
               x => x.TblTransactionType == 10).Max(x => x.DocDate);
            }
        }

        [OperationContract]
        private int GlPost(DateTime fromDate, DateTime toDate, int user, string company, bool sales, bool transfer,
            bool adjustment, bool expenses, bool Depreciation, bool Commission, bool repost, bool CostCenter)
        {
            //using (var scope = new TransactionScope())
            //{
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
        
                if (transfer)
                {
                    if (
                        entity.TblLedgerHeaders.Any(
                            x =>
                                x.TblTransactionType == 3 && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value <= toDate &&
                                repost == false))
                    {
                        return 0;
                    }
                }
                if (sales)
                {
                    if (
                        entity.TblLedgerHeaders.Any(
                            x =>
                                x.TblTransactionType == 2 && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value <= toDate &&
                                repost == false))
                    {
                        return 0;
                    }
                }
                if (adjustment)
                {
                    if (
                        entity.TblLedgerHeaders.Any(
                            x =>
                                x.TblTransactionType == 4 && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value <= toDate &&
                                repost == false))
                    {
                        return 0;
                    }
                }
                if (expenses)
                {
                    if (
                        entity.TblLedgerHeaders.Any(
                            x =>
                                x.TblTransactionType == 5 && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value <= toDate &&
                                repost == false))
                    {
                        return 0;
                    }
                }
                if (Depreciation)
                {
                    if (
                      entity.TblLedgerHeaders.Any(
                          x =>
                              x.TblTransactionType == 8 && x.DocDate.Value >= fromDate &&
                              x.DocDate.Value <= toDate &&
                              repost == false))
                    {
                        return 0;
                    }
                }
                if (Commission)
                {
                    if (
                        entity.TblLedgerHeaders.Any(
                            x =>
                                x.TblTransactionType == 10 && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value <= toDate &&
                                repost == false))
                    {
                        return 0;
                    }
                }
                if (sales)
                {
                   // 

                    if (company == "MAN")
                    {
                        GlManPostSalesTransaction(fromDate, toDate, user, company, sales, transfer, adjustment, repost);
                    }
                    GlPostSalesTransaction(fromDate, toDate, user, company, sales, transfer, adjustment, repost);
                    
                }
                if (transfer)
                {
                    GlPostTransfer(fromDate, toDate, user, company, sales, transfer, adjustment, repost);
                }
                if (adjustment)
                {
                    GlPostAdjustment(fromDate, toDate, user, company, sales, transfer, adjustment, repost);
                }
                if (expenses)
                {
                    GlPostexpenses(fromDate, toDate, user, company, expenses, repost);
                }
                if (Depreciation)
                {

                    GlPostDepreciation(fromDate, toDate, user, company, Depreciation, repost);
                }
                if (Commission)
                {
                    GetDailySalesCommision(0, fromDate, toDate, user, company);
                }




            }
            // scope.Complete();
            return 1;
            //}
        }

        [OperationContract]
        private int CalcCostCenter(string company)
        {
            var count = 0;

            //count = RecalcCostCenter(company);
            //while (count != 0)
            //{
            count = RecalcCostCenter(company);
            //  }
            return count;

        }


        private void GlPostexpenses(DateTime fromDate, DateTime toDate, int user, string company, bool expenses,
            bool repost)
        {
            #region expenses

            if (expenses)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var tblChainSetupTest =
                        entity.tblChainSetupTests.FirstOrDefault(
                            x => x.sGlobalSettingCode == "GlItemGroupTableName");
                    if (tblChainSetupTest != null)
                    {
                        var tablename = tblChainSetupTest
                            .sSetupValue;

                        var field = tablename;

                        if (tablename == "TblItemDownLoadDef")
                        {
                            field = "ItemStoreGroup";
                        }
                        var Type = 5;

                        var oldLedgers =
                            entity.TblLedgerHeaders.Where(
                                x =>
                                    x.TblTransactionType == Type && x.DocDate.Value >= fromDate &&
                                    x.DocDate.Value < toDate);
                        foreach (var variable in oldLedgers)
                        {
                            entity.TblLedgerHeaders.DeleteObject(variable);
                        }
                        entity.SaveChanges();
                        foreach (var day in EachDay(fromDate, toDate))
                        {
                            if (day != toDate)
                            {
                                #region Paramters

                                var sqlParam = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = day,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = day.AddDays(1),
                                        SqlDbType = SqlDbType.Date
                                    },
                                };

                                #endregion Paramters

                                var list =
                                    entity.ExecuteStoreQuery<GlPosting>(
                                        "exec SpGlExpensis @FromDate,@ToDate",
                                        sqlParam.ToArray()).ToList<GlPosting>();

                                var journal =
                                    entity.tblChainSetupTests.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "GLExpensesJournal").sSetupValue;

                                var journalint =
                                    entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = "expenses",
                                    DocDate = day,
                                    TblJournal = journalint,
                                    TblTransactionType = Type,
                                    TblJournalLink = 0,
                                };
                                var temp = 0;
                                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out temp, user, company);

                                var currency = DefaultCurrency(company);

                                foreach (
                                    var group in list.GroupBy(x => x.ItemIserial))
                                {
                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 102);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 102).TblAccount;
                                    }

                                    if (groupAccount == 0)
                                    {
                                    }

                                    foreach (var variable in list)
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }

                                foreach (
                                    var group in list.GroupBy(x => x.AccountTemp))
                                {
                                    //Outlet Expenses Offset Account

                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales),
                                        Description = "expenses",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = group.Key,
                                        GlAccount = group.Key,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = false,
                                    };

                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                                        company, user);

                                    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                        if(storeCostcenter != null)
                                        {
                                            CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow, storeCostcenter);
                                        }
                                        if (costcenter != null)
                                        {
                                            CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow, costcenter);
                                        }
                                    }
                                }

                                foreach (
                                    var group in list.GroupBy(x => x.ItemIserial))
                                {
                                    var groupAccount = entity.Entities.FirstOrDefault(
                                        x => x.Iserial == group.Key && x.TblJournalAccountType == 8 && x.scope == 0)
                                        .AccountIserial;

                                    foreach (var variable in list)
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }
                                foreach (
                                    var group in list.GroupBy(x => x.AccountTemp))
                                {
                                    var newledgerDetailrowOffset = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales),
                                        Description = "expenses",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = group.Key,
                                        GlAccount = @group.Key,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = true,
                                        OffsetAccountType = 0,
                                    };
                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowOffset, true, 000, out temp,
                                        company, user);
                                    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                        if(storeCostcenter != null)
                                        {
                                            CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrowOffset,
                                            storeCostcenter);
                                        }
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrowOffset,
                                            costcenter);
                                    }
                                }
                                CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                            }
                        }
                    }
                }
            }

            #endregion expenses
        }

        private void GlPostDepreciation(DateTime fromDate, DateTime toDate, int user, string company, bool depreciation,
         bool repost)
        {
            #region Depreciation

            if (depreciation)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var Type = 8;
                    var oldLedgers =
                        entity.TblLedgerHeaders.Where(
                            x =>
                                x.TblTransactionType == Type && x.DocDate.Value >= fromDate &&
                                x.DocDate.Value < toDate);
                    foreach (var variable in oldLedgers)
                    {
                        entity.TblLedgerHeaders.DeleteObject(variable);
                    }
                    entity.SaveChanges();
                    var fromdate = Convert.ToDateTime(fromDate.Date).Date;
                    var list = entity.TblDepreciationTransactionDetails.Include("TblDepreciationTransactionHeader1.TblLedgerMainDetail1.TblCurrency1").Where(x => x.DueDate.Value >= fromdate &&
                            x.DueDate.Value < toDate);

                    var journal =
                        entity.tblChainSetupTests.FirstOrDefault(
                            x => x.sGlobalSettingCode == "GLExpensesJournal").sSetupValue;

                    var journalint =
                        entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;
                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                      x => x.TblInventAccountType == 103);
                    var newLedgerHeaderRow = new TblLedgerHeader
                    {
                        CreatedBy = user,
                        CreationDate = DateTime.Now,
                        Description = "Depreciation",
                        DocDate = toDate,
                        TblJournal = journalint,
                        TblTransactionType = Type,
                        TblJournalLink = 0,
                    };
                    var temp = 0;
                    UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out temp, user, company);
                    foreach (var VARIABLE in list)
                    {
                        VARIABLE.Status = 1;
                    }
                    foreach (var group in list.GroupBy(x => x.TblDepreciationTransactionHeader1).ToList())
                    {
                        var asset = entity.TblAssetRetails.Include("TblAssetGroup1").Include("TblAccount1").FirstOrDefault(x => x.Iserial == group.Key.TblLedgerMainDetail1.EntityAccount);

                        var account = asset.TblAccount;

                        if (account == null || account == 0)
                        {
                            account = asset.TblAssetGroup1.TblAccount;
                        }

                        var accountCr = asset.SumAccount;

                        if (accountCr == null || accountCr == 0)
                        {
                            accountCr = asset.TblAssetGroup1.SumAccount;
                        }
                        var newledgerDetailrow = new TblLedgerMainDetail
                        {
                            Amount = (decimal?)@group.Sum(x => x.DepExpense),
                            Description = "Depreciation " + asset.Ename,
                            ExchangeRate = 1 * group.Key.TblLedgerMainDetail1.TblCurrency1.ExchangeRate,
                            TblCurrency = group.Key.TblLedgerMainDetail1.TblCurrency,
                            TransDate = toDate,
                            TblJournalAccountType = 5,
                            EntityAccount = group.Key.TblLedgerMainDetail1.EntityAccount,
                            GlAccount = (int)account,
                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                            PaymentRef = "",
                            DrOrCr = true,
                        };

                        UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);
                        var newledgerDetailrow1 = new TblLedgerMainDetail
                        {
                            Amount = (decimal?)@group.Sum(x => x.DepExpense),
                            Description = "Depreciation " + asset.Ename,
                            ExchangeRate = group.Key.TblLedgerMainDetail1.TblCurrency1.ExchangeRate,
                            TblCurrency = group.Key.TblLedgerMainDetail1.TblCurrency,
                            TransDate = toDate,
                            TblJournalAccountType = 0,
                            EntityAccount = accountCr,
                            GlAccount = (int)accountCr,
                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                            PaymentRef = "",
                            DrOrCr = false,
                        };

                        UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow1, true, 000, out temp, company, user);
                    }

                    CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                }
            }

            #endregion Depreciation
        }

        private void GlPostAdjustment(DateTime fromDate, DateTime toDate, int user, string company, bool sales,
            bool transfer, bool adjustment, bool repost)
        {
            #region Adjustment

            if (adjustment)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var tblChainSetupTest =
                        entity.tblChainSetupTests.FirstOrDefault(
                            x => x.sGlobalSettingCode == "GlItemGroupTableName");
                    if (tblChainSetupTest != null)
                    {
                        var tablename = tblChainSetupTest
                            .sSetupValue;

                        var field = tablename;

                        if (tablename == "TblItemDownLoadDef")
                        {
                            field = "ItemStoreGroup";
                        }
                        var Type = 4;

                        var oldLedgers =
                            entity.TblLedgerHeaders.Where(
                                x =>
                                    x.TblTransactionType == Type && x.DocDate.Value >= fromDate &&
                                    x.DocDate.Value < toDate);
                        foreach (var variable in oldLedgers)
                        {
                            entity.TblLedgerHeaders.DeleteObject(variable);
                        }
                        entity.SaveChanges();
                        foreach (var day in EachDay(fromDate, toDate))
                        {
                            if (day != toDate)
                            {
                                #region Paramters

                                var sqlParam = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = day,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = day.AddDays(1),
                                        SqlDbType = SqlDbType.Date
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableName",
                                        Value = tablename,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableNameField",
                                        Value = field,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                };

                                #endregion Paramters

                                var list =
                                    entity.ExecuteStoreQuery<GlPosting>(
                                        "exec SpGlAdjustCost @FromDate,@ToDate,@TableName,@TableNameField",
                                        sqlParam.ToArray()).ToList<GlPosting>();

                                var journal =
                                    entity.tblChainSetupTests.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "GLAdjustmentJournal").sSetupValue;

                                var journalint =
                                    entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = "Adjustment",
                                    DocDate = day,
                                    TblJournal = journalint,
                                    TblTransactionType = Type,
                                    TblJournalLink = 0,
                                };
                                var temp = 0;
                                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out temp, user, company);

                                var currency = DefaultCurrency(company);
                                foreach (
                                    var group in list.Where(x => x.NetSales > 0).GroupBy(x => x.GroupIserial))
                                {
                                    //Inventory, profit

                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 61);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 61).TblAccount;
                                    }

                                    var InvgroupAccount = 0;
                                    var InvtblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 60);
                                    if (InvtblInventPosting != null)
                                        InvgroupAccount =
                                            InvtblInventPosting.TblAccount;

                                    if (InvgroupAccount == 0)
                                    {
                                        InvgroupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60).TblAccount;
                                    }

                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales),
                                        Description = "Adjustment",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = groupAccount,
                                        GlAccount = groupAccount,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = true,
                                        OffsetAccountType = 0,
                                        OffsetEntityAccount = InvgroupAccount,
                                        OffsetGlAccount = InvgroupAccount
                                    };
                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                                        company, user);

                                    foreach (var rr in list.Where(x => x.GroupIserial == @group.Key && x.NetSales > 0))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                        if(storeCostcenter != null)
                                        { 
                                           CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            storeCostcenter);
                                        }
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            costcenter);
                                    }
                                }

                                foreach (
                                    var group in list.Where(x => x.NetSales < 0).GroupBy(x => x.GroupIserial))
                                {
                                    //Inventory, loss

                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 59);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 59).TblAccount;
                                    }
                                    if (groupAccount == 0)
                                    {
                                    }

                                    var InvgroupAccount = 0;
                                    var InvtblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 60);
                                    if (InvtblInventPosting != null)
                                        InvgroupAccount =
                                            InvtblInventPosting.TblAccount;

                                    if (InvgroupAccount == 0)
                                    {
                                        InvgroupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60).TblAccount;
                                    }
                                    if (InvgroupAccount == 0)
                                    {
                                    }

                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales),
                                        Description = "Adjustment",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = groupAccount,
                                        GlAccount = groupAccount,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = false,

                                        OffsetAccountType = 0,
                                        OffsetEntityAccount = InvgroupAccount,
                                        OffsetGlAccount = InvgroupAccount
                                    };

                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                                        company, user);

                                    foreach (var rr in list.Where(x => x.GroupIserial == @group.Key && x.NetSales < 0))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            storeCostcenter);
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            costcenter);
                                    }
                                }
                                CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                            }
                        }
                    }
                }
            }

            #endregion Adjustment
        }

        [OperationContract]
        public TblGlRuleDetail FindCostCenterByType(TblGlRuleDetail storeCostcenter, int TblJournalAccountType,
           int Group, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return FindCostCenterByType(storeCostcenter, TblJournalAccountType, Group, entity);
            }
        }


        public TblGlRuleDetail FindCostCenterByType(TblGlRuleDetail storeCostcenter, int TblJournalAccountType,
            int Group, ccnewEntities entity)
        {

            var tblInventPostingcostcentertemp = entity.TblGlRuleDetails.Include("TblCostCenter1").FirstOrDefault(
                x => x.GlInventoryGroup == Group && x.TblJournalAccountType == TblJournalAccountType);
            if (tblInventPostingcostcentertemp != null)
            {
                storeCostcenter =
                    tblInventPostingcostcentertemp;
            }
            if (storeCostcenter.Iserial == 0)
            {
                storeCostcenter = entity.TblGlRuleDetails.Include("TblCostCenter1").FirstOrDefault(
                    x =>
                        x.GlInventoryGroup == -1 &&
                        x.TblJournalAccountType == TblJournalAccountType);
            }
            return storeCostcenter;
        }


        public List<TblCostCenter> FindCostCenterByOrganizationUnit(int TblOrganizationUnit, List<TblCostCenterOrganizationUnit> CostCenterOrganizationUnitList, ccnewEntities entity)
        {
            var CostCenterIserial = CostCenterOrganizationUnitList.Where(w => w.TblOrganizationUnit == TblOrganizationUnit).Select(w => w.TblCostCenter).ToList();

            return entity.TblCostCenters.Where(e => CostCenterIserial.Contains(e.Iserial)).ToList();
            //var tblInventPostingcostcentertemp = entity.TblGlRuleDetails.Include("TblCostCenter1").FirstOrDefault(
            //    x => x.GlInventoryGroup == Group && x.TblJournalAccountType == TblJournalAccountType);
            //if (tblInventPostingcostcentertemp != null)
            //{
            //    storeCostcenter =
            //        tblInventPostingcostcentertemp;
            //}
            //if (storeCostcenter.Iserial == 0)
            //{
            //    storeCostcenter = entity.TblGlRuleDetails.Include("TblCostCenter1").FirstOrDefault(
            //        x =>
            //            x.GlInventoryGroup == -1 &&
            //            x.TblJournalAccountType == TblJournalAccountType);
            //}
            //return storeCostcenter;
        }

        private void GlPostTransfer(DateTime fromDate, DateTime toDate, int user, string company, bool sales,
            bool transfer, bool adjustment, bool repost)
        {
            #region Transfer

            if (transfer)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var tblChainSetupTest =
                        entity.tblChainSetupTests.FirstOrDefault(
                            x => x.sGlobalSettingCode == "GlItemGroupTableName");
                    if (tblChainSetupTest != null)
                    {
                        var tablename = tblChainSetupTest
                            .sSetupValue;

                        var field = tablename;

                        if (tablename == "TblItemDownLoadDef")
                        {
                            field = "ItemStoreGroup";
                        }
                        var Type = 3;

                        var oldLedgers =
                            entity.TblLedgerHeaders.Where(
                                x =>
                                    x.TblTransactionType == Type && x.DocDate.Value >= fromDate &&
                                    x.DocDate.Value < toDate);
                        foreach (var variable in oldLedgers)
                        {
                            entity.TblLedgerHeaders.DeleteObject(variable);
                        }
                        entity.SaveChanges();
                        foreach (var day in EachDay(fromDate, toDate))
                        {
                            if (day != toDate)
                            {
                                #region Paramters

                                var sqlParam = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = day,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = day.AddDays(1),
                                        SqlDbType = SqlDbType.Date
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableName",
                                        Value = tablename,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableNameField",
                                        Value = field,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                };

                                #endregion Paramters

                                var list =
                                    entity.ExecuteStoreQuery<GlPosting>(
                                        "exec SpGlTransferCost @FromDate,@ToDate,@TableName,@TableNameField",
                                        sqlParam.ToArray()).ToList<GlPosting>();

                                var journal =
                                    entity.tblChainSetupTests.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "GLTransferJournal").sSetupValue;

                                var journalint =
                                    entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = "Transfer",
                                    DocDate = day,
                                    TblJournal = journalint,
                                    TblTransactionType = Type,
                                    TblJournalLink = 0,
                                };
                                var temp = 0;
                                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out temp, user, company);

                                var currency = DefaultCurrency(company);

                                foreach (
                                    var group in list.Where(x => x.NetSales > 0).GroupBy(x => x.GroupIserial))
                                {
                                    //Inventory, receipt
                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 60);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60).TblAccount;
                                    }
                                    foreach (var variable in list.Where(x => x.NetSales > 0))
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }

                                foreach (var group in list.Where(x => x.NetSales > 0).GroupBy(x => x.AccountTemp))
                                {
                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales),
                                        Description = "Transfer",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = group.Key,
                                        GlAccount = group.Key,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = true
                                    };
                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                                        company, user);

                                    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key && x.NetSales > 0))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            storeCostcenter);
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow,
                                            costcenter);
                                    }
                                }

                                foreach (var group in list.Where(x => x.NetSales < 0).GroupBy(x => x.GroupIserial))
                                {
                                    //Inventory, receipt
                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 60);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60).TblAccount;
                                    }
                                    foreach (var variable in list.Where(x => x.NetSales < 0))
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }
                                foreach (
                                    var group in list.Where(x => x.NetSales < 0).GroupBy(x => x.AccountTemp))
                                {
                                    //Inventory, receipt
                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = @group.Sum(x => x.NetSales) * -1,
                                        Description = "Transfer",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = group.Key,
                                        GlAccount = group.Key,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = false
                                    };

                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                                        company, user);

                                    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key && x.NetSales < 0))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales * -1, newledgerDetailrow,
                                            storeCostcenter);
                                        CreateTblLedgerDetailCostCenter(company, rr.NetSales * -1, newledgerDetailrow,
                                            costcenter);
                                    }
                                }
                                CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                            }
                        }
                    }
                }

                #endregion Transfer
            }
        }


        private void GlPostSalesTransaction(DateTime fromDate, DateTime toDate, int user, string company, bool sales,
        bool transfer, bool adjustment, bool repost)
        {
            #region Sales
            var taxPercentage = 1.14;
            if (sales)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    entity.CommandTimeout = 0;
                    var tblChainSetupTest = "TblItemDownLoadDef";
                    if (tblChainSetupTest != null)
                    {
                        var tablename = "TblItemDownLoadDef";

                        var field = tablename;

                        if (tablename == "TblItemDownLoadDef")
                        {
                            field = "ItemStoreGroup";
                        }

                        // sales
                        var Type = 2;

                        var oldLedgers =
                            entity.TblLedgerHeaders.Where(
                                x => x.TblTransactionType == Type && x.DocDate.Value >= fromDate && x.DocDate.Value <= toDate).ToList();
                        foreach (var variable in oldLedgers)
                        {
                            entity.TblLedgerHeaders.DeleteObject(variable);
                        }
                        entity.SaveChanges();

                        var journal =
                            entity.tblChainSetupTests.FirstOrDefault(
                                x => x.sGlobalSettingCode == "GLSalesJournal")
                                .sSetupValue;
                        var costOfGoodSoldjournal =
                           entity.tblChainSetupTests.FirstOrDefault(
                               x => x.sGlobalSettingCode == "GLCostOfGoodSoldJournal")
                               .sSetupValue;

                        var salestype =
                            entity.tblChainSetupTests.FirstOrDefault(
                                x => x.sGlobalSettingCode == "GLPostCostOfGoodSold")
                                .sSetupValue;

                        var taxpercentageRecrod =
                           entity.tblChainSetupTests.FirstOrDefault(
                               x => x.sGlobalSettingCode == "GlSalesTaxPercentage");

                        if (taxpercentageRecrod != null)
                        {
                            taxPercentage = Convert.ToDouble(taxpercentageRecrod.sSetupValue);
                        }

                        var journalint = entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;
                        var journalCostInt = entity.TblJournals.FirstOrDefault(x => x.Code == costOfGoodSoldjournal).Iserial;
                        var currency = DefaultCurrency(company);
                        if (salestype == "2")
                        {
                            foreach (DateTime day in EachDay(fromDate, toDate))
                            {
                                var newLedgerHeaderRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = "Sales",
                                    DocDate = day,
                                    TblJournal = journalint,
                                    TblTransactionType = Type,
                                    TblJournalLink = 0,
                                };

                                var newLedgerHeaderRowCostOfGoodSold = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = "Cost Of Good Sold",
                                    DocDate = day,
                                    TblJournal = journalCostInt,
                                    TblTransactionType = Type,
                                    TblJournalLink = 0,
                                };
                                var tempheader = 0;
                                if (salestype == "2")
                                {
                                    UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRowCostOfGoodSold, true, 000, out tempheader, user,
                                   company);

                                    UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, user,
                                 company);

                                }
                                
                                #region Paramters

                                var sqlParam = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = day,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = day.AddDays(1),
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "TableName",
                                        Value = tablename,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableNameField",
                                        Value = field,
                                        SqlDbType = SqlDbType.NVarChar
                                    }
                                };
                                var list = entity.ExecuteStoreQuery<GlPosting>(
                                                                 "exec SpGlNetSales @FromDate,@ToDate,@TableName,@TableNameField",
                                                                 sqlParam.ToArray())
                                                                 .ToList<GlPosting>();
                                var temp = 0;

                                #endregion Paramters                                    

                                #region NetSales
                                var customerIserials = list.GroupBy(x => x.CustIserial).Select(e=>e.Key).Distinct();
                                var customers = entity.TBLCustomers.Where(r => r.CustomerGroup !=null&& customerIserials.Contains(r.Iserial)).ToList();
                                foreach (var group in list.GroupBy(x => x.CustIserial))
                                {
                             
                                    var groupAccount = 0;
                                    var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                        x =>
                                            x.SupCustRelation == @group.Key && x.SupCustScope == 0 &&
                                            x.TblInventAccountType == 74);
                                    if (tblInventPosting != null)
                                        groupAccount =
                                            tblInventPosting.TblAccount;

                                    if (groupAccount == 0)
                                    {
                                        groupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 74).TblAccount;
                                    }
                                    

                                    //int groupAccount = 0;
                                    //GetAccountByEntity(@group.Key, 1, company, 0, out groupAccount);
                                    foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }

                                foreach (var group in list.GroupBy(x => x.AccountTemp))
                                {
                                    decimal NetsalesAmount = 0;
                                    NetsalesAmount = TotalSales(company, taxPercentage, list, customers, group, NetsalesAmount);

                                  

                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = NetsalesAmount,
                                        Description = "Sales",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = 0,
                                        EntityAccount = group.Key,
                                        GlAccount = group.Key,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = false
                                    };

                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company,
                                        user);

                                    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key))
                                    {
                                        //decimal NetsalesAmountPerStore = 0;
                                        //NetsalesAmountPerStore = TotalSales(company, taxPercentage, rr, customers, group, NetsalesAmount);
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                        if (taxPercentage > 0)
                                        {
                                            var RemoveTax = false;
                                            //revenue
                                            if (company == "MAN")
                                            {
                                                var customerPerRow = customers.FirstOrDefault(w => w.Iserial == rr.CustIserial);
                                                if (customerPerRow != null)
                                                {
                                                    if (customerPerRow.CustomerGroup == 2)
                                                    {
                                                        RemoveTax = true;
                                                    }

                                                }
                                            }
                                            var amount = rr.NetSales / (decimal)taxPercentage;
                                            if (RemoveTax)
                                            {
                                                amount = rr.NetSales;
                                            }

                                            if (storeCostcenter != null)
                                            {
                                                CreateTblLedgerDetailCostCenter(company, amount,
                                                    newledgerDetailrow, storeCostcenter);
                                            }
                                            CreateTblLedgerDetailCostCenter(company, amount,
                                                newledgerDetailrow, costcenter);
                                        }
                                        else
                                        {
                                            CreateTblLedgerDetailCostCenter(company, rr.NetSales,
                                           newledgerDetailrow, storeCostcenter);
                                            CreateTblLedgerDetailCostCenter(company, rr.NetSales,
                                                newledgerDetailrow, costcenter);
                                        }

                                    }
                                }

                                foreach (var group in list.GroupBy(x => x.CustIserial))
                                {
                                  
                                    var temproww = entity.TblInventPostings.FirstOrDefault(
                                        x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 101);
                                    var salesgroupAccount = 0;
                                    if (temproww != null)
                                    {
                                        salesgroupAccount = temproww.TblAccount;
                                    }

                                    if (salesgroupAccount == 0)
                                    {
                                        salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                                            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 101).TblAccount;
                                    }


                                    foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                                    {
                                        variable.AccountTemp = salesgroupAccount;
                                    }
                                }

                                if (taxPercentage > 0)
                                {
                                    foreach (var group in list.GroupBy(x => x.AccountTemp))
                                    {
                                        decimal NetsalesAmount = 0;
                                        NetsalesAmount = TotalSalesTax(company, taxPercentage, list, customers, group, NetsalesAmount);
                                        //101 SalesTax Account
                                        var newledgerDetailrowtax = new TblLedgerMainDetail
                                        {
                                            Amount = NetsalesAmount,
                                            Description = "Tax",
                                            ExchangeRate = 1,
                                            TblCurrency = currency,
                                            TransDate = day,
                                            TblJournalAccountType = 0,
                                            EntityAccount = group.Key,
                                            GlAccount = group.Key,
                                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                            PaymentRef = "",
                                            DrOrCr = false
                                        };
                                        UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp,
                                            company, user);

                                        foreach (var rr in list.Where(x => x.AccountTemp == @group.Key))
                                        {
                                            var storeCostcenter = new TblGlRuleDetail();
                                            storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                                company);

                                            var costcenter = new TblGlRuleDetail();

                                            costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                                            var RemoveTax = false;
                                            //revenue
                                            if (company == "MAN")
                                            {
                                                var customerPerRow = customers.FirstOrDefault(w => w.Iserial == rr.CustIserial);
                                                if (customerPerRow != null)
                                                {
                                                    if (customerPerRow.CustomerGroup == 2)
                                                    {
                                                        RemoveTax = true;
                                                    }

                                                }
                                            }
                                            var amount = rr.NetSales - rr.NetSales / (decimal)taxPercentage;
                                            if (RemoveTax)
                                            {
                                                amount = 0;
                                            }


                                            if (storeCostcenter != null)
                                            {
                                                CreateTblLedgerDetailCostCenter(company, amount,
                                                newledgerDetailrowtax, storeCostcenter);
                                            }
                                            CreateTblLedgerDetailCostCenter(company, amount,
                                                newledgerDetailrowtax, costcenter);
                                        }
                                    }
                                }

                                #endregion NetSales

                                #region Payment

                                foreach (var group in list.GroupBy(x => x.StoreIserial))
                                {
                                    var groupAccount = 0;
                                    GetAccountByEntity(@group.Key, 8, company, 0, out groupAccount);
                                    foreach (var variable in list.Where(x => x.StoreIserial == group.Key))
                                    {
                                        variable.AccountTemp = groupAccount;
                                    }
                                }

                                var journalType = 8;
                                if (company == "MAN")
                                {
                                    journalType = 1;
                                }
                                foreach (var group in list.GroupBy(x => x.StoreIserial))
                                {
                                    var RemoveTax = false;
                                    //revenue
                                    if (company == "MAN")
                                    {
                                        var customerPerRow = customers.FirstOrDefault(w => w.Iserial == group.FirstOrDefault().CustIserial);
                                        if (customerPerRow != null)
                                        {
                                            if (customerPerRow.CustomerGroup == 2)
                                            {
                                                RemoveTax = true;
                                            }

                                        }
                                    }

                                    var entityAccount = group.Key;
                                    var Amount = @group.Sum(w => w.NetSales);

                                    if (company == "MAN")
                                    {
                                        entityAccount = group.First().CustIserial;
                                        Amount = @group.Sum(x => x.NetSales) / (decimal)taxPercentage;
                                        if (RemoveTax)
                                        {
                                            Amount = @group.Sum(x => x.NetSales);
                                        }
                                    }
                                    var newledgerDetailrow = new TblLedgerMainDetail
                                    {
                                        Amount = Amount,
                                        Description = "Sales",
                                        ExchangeRate = 1,
                                        TblCurrency = currency,
                                        TransDate = day,
                                        TblJournalAccountType = journalType,
                                        EntityAccount = entityAccount,
                                        GlAccount = group.First().AccountTemp,
                                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                        PaymentRef = "",
                                        DrOrCr = true
                                    };
                                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company,
                                        user);

                                    foreach (var rr in list.Where(x => x.StoreIserial == @group.Key))
                                    {
                                        var storeCostcenter = new TblGlRuleDetail();
                                        storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                            company);

                                        var costcenter = new TblGlRuleDetail();
                                        costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                                        if (group.Key != 0)
                                        {
                                            var amount = rr.NetSales;
                                            if (company == "MAN")
                                            {
                                                amount = rr.NetSales - rr.NetSales / (decimal)taxPercentage;

                                                if (RemoveTax)
                                                {
                                                    amount = rr.NetSales;
                                                }
                                            }

                                            CreateTblLedgerDetailCostCenter(company, amount, newledgerDetailrow,
                                                storeCostcenter);
                                            CreateTblLedgerDetailCostCenter(company, amount, newledgerDetailrow,
                                                costcenter);
                                        }
                                    }
                                }

                                if (company == "MAN")
                                {

                                    foreach (var group in list.GroupBy(x => x.StoreIserial))
                                    {
                                        var RemoveTax = false;
                                        if (company == "MAN")
                                        {
                                            var customerPerRow = customers.FirstOrDefault(w => w.Iserial == group.FirstOrDefault().CustIserial);
                                            if (customerPerRow != null)
                                            {
                                                if (customerPerRow.CustomerGroup == 2)
                                                {
                                                    RemoveTax = true;
                                                }

                                            }
                                        }
                                        var entityAccount = group.First().CustIserial;
                                        var custIserial = @group.FirstOrDefault().CustIserial;
                                        var amount = @group.Sum(x => x.NetSales) - @group.Sum(x => x.NetSales) / (decimal)taxPercentage;

                                        if (RemoveTax)
                                        {
                                            amount = 0;
                                        }

                                        //101 SalesTax Account
                                        var newledgerDetailrowtax = new TblLedgerMainDetail
                                        {
                                            Amount = amount,
                                            Description = "Tax",
                                            ExchangeRate = 1,
                                            TblCurrency = currency,
                                            TransDate = day,
                                            TblJournalAccountType = journalType,
                                            EntityAccount = entityAccount,
                                            GlAccount = group.First().AccountTemp,
                                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                            PaymentRef = "",
                                            DrOrCr = true
                                        };
                                        UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp,
                                            company, user);

                                        foreach (var rr in list.Where(x => x.AccountTemp == @group.FirstOrDefault().AccountTemp))
                                        {

                                            if (company == "MAN")
                                            {
                                                var customerPerRow = customers.FirstOrDefault(w => w.Iserial == group.FirstOrDefault().CustIserial);
                                                if (customerPerRow != null)
                                                {
                                                    if (customerPerRow.CustomerGroup == 2)
                                                    {
                                                        RemoveTax = true;
                                                    }

                                                }
                                            }

                                            var storeCostcenter = new TblGlRuleDetail();
                                            storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial,
                                                company);

                                            var costcenter = new TblGlRuleDetail();
                                            var amountStore = rr.NetSales - rr.NetSales / (decimal)taxPercentage;
                                            if (RemoveTax)
                                            {
                                                amountStore =0;
                                            }
                                            costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);
                                            if (storeCostcenter != null)
                                            {
                                                CreateTblLedgerDetailCostCenter(company, amountStore,
                                                newledgerDetailrowtax, storeCostcenter);
                                            }
                                            CreateTblLedgerDetailCostCenter(company, amountStore,
                                                newledgerDetailrowtax, costcenter);



                                        }

                                    }

                                }

                                #endregion Payment

                                if (salestype == "1" || salestype == "2")
                                {
                                    #region CostOfGoodSold

                                    #region Paramters

                                    var sqlParamw = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = day,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = day.AddDays(1),
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "TableName",
                                        Value = tablename,
                                        SqlDbType = SqlDbType.NVarChar
                                    },
                                    new SqlParameter
                                    {
                                        ParameterName = "TableNameField",
                                        Value = field,
                                        SqlDbType = SqlDbType.NVarChar
                                    }
                                };

                                    #endregion Paramters

                                    if (salestype == "1")
                                    {
                                        CostOfGoodSold(entity, currency, newLedgerHeaderRow, day, company, user, sqlParamw);
                                    }

                                    if (salestype == "2")
                                    {
                                        CostOfGoodSold(entity, currency, newLedgerHeaderRowCostOfGoodSold, day, company, user, sqlParamw);
                                    }

                                    #endregion CostOfGoodSold
                                }

                               CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                            }

                        }
                    }
                }

                #endregion Sales
            }
        }

        private static decimal TotalSales(string company, double taxPercentage, List<GlPosting> list, List<TBLCustomer> customers, IGrouping<int, GlPosting> group, decimal NetsalesAmount)
        {
            foreach (var CustIserial in list.GroupBy(x => x.CustIserial))
            {
                decimal amount = 0;
                if (taxPercentage > 0)
                {
                    amount = CustIserial.Sum(x => x.NetSales) / (decimal)taxPercentage;
                }
                else
                {
                    amount = CustIserial.Sum(x => x.NetSales);
                }

                var RemoveTax = false;
                //revenue
                if (company == "MAN")
                {
                    var customerPerRow = customers.FirstOrDefault(w => w.Iserial == CustIserial.Key);
                    if (customerPerRow != null)
                    {
                        if (customerPerRow.CustomerGroup == 2)
                        {
                            RemoveTax = true;
                        }

                    }
                }
                if (RemoveTax)
                {
                    amount = CustIserial.Sum(x => x.NetSales);
                }
                NetsalesAmount = NetsalesAmount + amount;
            }

            return NetsalesAmount;
        }

        private static decimal TotalSalesTax(string company, double taxPercentage, List<GlPosting> list, List<TBLCustomer> customers, IGrouping<int, GlPosting> group, decimal NetsalesAmount)
        {
            foreach (var CustIserial in list.GroupBy(x => x.CustIserial))
            {
                decimal amount = 0;
                if (taxPercentage > 0)
                {
                    //amount = @group.Sum(x => x.NetSales) / (decimal)taxPercentage;
                    //amount = @group.Sum(x => x.NetSales) / (decimal)taxPercentage;
                    amount = CustIserial.Sum(x => x.NetSales) - CustIserial.Sum(x => x.NetSales) / (decimal)taxPercentage;

                }
                var RemoveTax = false;
                //revenue
                if (company == "MAN")
                {
                    var customerPerRow = customers.FirstOrDefault(w => w.Iserial == CustIserial.Key);
                    if (customerPerRow != null)
                    {
                        if (customerPerRow.CustomerGroup == 2)
                        {
                            RemoveTax = true;
                        }

                    }
                }
                if (RemoveTax)
                {
                    amount = 0;
                }
                NetsalesAmount = NetsalesAmount + amount;
            }

            return NetsalesAmount;
        }

        private void GlManPostSalesTransaction(DateTime fromDate, DateTime toDate, int user, string company, bool sales,
                                                bool transfer, bool adjustment, bool repost)
        {
            #region Sales
            var taxPercentage = 1.14;
            if (sales)
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    entity.CommandTimeout = 0;
                    var tblChainSetupTest =
                        entity.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlItemGroupTableName");
                    if (tblChainSetupTest != null)
                    {
                        var tablename = tblChainSetupTest
                            .sSetupValue;

                        var field = tablename;

                        if (tablename == "TblItemDownLoadDef")
                        {
                            field = "ItemStoreGroup";
                        }

                        var Type = 15;
                        
                        var oldLedgers =
                            entity.TblLedgerHeaders.Where(
                                x => x.TblTransactionType == Type && x.DocDate.Value >= fromDate && x.DocDate.Value <= toDate).ToList();
                        foreach (var variable in oldLedgers)
                        {
                            entity.TblLedgerHeaders.DeleteObject(variable);
                        }
                        entity.SaveChanges();

                        var journal =
                            entity.tblChainSetupTests.FirstOrDefault(
                                x => x.sGlobalSettingCode == "GLSalesJournal")
                                .sSetupValue;
                        var costOfGoodSoldjournal =
                           entity.tblChainSetupTests.FirstOrDefault(
                               x => x.sGlobalSettingCode == "GLCostOfGoodSoldJournal")
                               .sSetupValue;

                        var salestype =
                            entity.tblChainSetupTests.FirstOrDefault(
                                x => x.sGlobalSettingCode == "GLPostCostOfGoodSold")
                                .sSetupValue;

                        var taxpercentageRecrod =
                                               entity.tblChainSetupTests.FirstOrDefault(
                                                   x => x.sGlobalSettingCode == "GlSalesTaxPercentage");

                        if (taxpercentageRecrod != null)
                        {
                            taxPercentage = Convert.ToDouble(taxpercentageRecrod.sSetupValue);
                        }


                        var journalint = entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;
                        var journalCostInt = entity.TblJournals.FirstOrDefault(x => x.Code == costOfGoodSoldjournal).Iserial;
                        var currency = DefaultCurrency(company);
                        var newLedgerHeaderRow = new TblLedgerHeader
                        {
                            CreatedBy = user,
                            CreationDate = DateTime.Now,
                            Description = "Sales Man",
                            DocDate = toDate,
                            TblJournal = journalint,
                            TblTransactionType = Type,
                            TblJournalLink = 0,
                        };

                     
                        var tempheader = 0;
                      

                        UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, user,
                            company);

                        #region Paramters

                        var sqlParam = new List<SqlParameter>
                                {
                                    new SqlParameter
                                    {
                                        ParameterName = "FromDate",
                                        Value = fromDate,
                                        SqlDbType = SqlDbType.Date
                                    },

                                    new SqlParameter
                                    {
                                        ParameterName = "ToDate",
                                        Value = toDate,
                                        SqlDbType = SqlDbType.Date
                                    },
                                };
                        var list = entity.ExecuteStoreQuery<GlPosting>(
                                                         "exec SpGlNetSalesMan @FromDate,@ToDate",
                                                         sqlParam.ToArray())
                                                         .ToList<GlPosting>();
                        var temp = 0;
                        #endregion Paramters                                    

                        #region NetSales

                        foreach (var group in list.GroupBy(x => x.CustIserial))
                        {
                            //revenue

                            var groupAccount = 0;
                            var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                                x =>
                                    x.SupCustRelation == -1 && x.SupCustScope == 0 &&
                                    x.TblInventAccountType == 74);
                            if (tblInventPosting != null)
                                groupAccount =
                                    tblInventPosting.TblAccount;

                            if (groupAccount == 0)
                            {
                                groupAccount = entity.TblInventPostings.FirstOrDefault(
                                    x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 74).TblAccount;
                            }
                            //int groupAccount = 0;
                            //GetAccountByEntity(@group.Key, 1, company, 0, out groupAccount);
                            foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                            {
                                variable.AccountTemp = groupAccount;
                            }
                        }

                        foreach (var group in list.GroupBy(x => x.AccountTemp))
                        {
                            var newledgerDetailrow = new TblLedgerMainDetail
                            {
                                Amount = @group.Sum(x => x.NetSales),
                                Description = "Sales Man",
                                ExchangeRate = 1,
                                TblCurrency = currency,
                                TransDate = fromDate,
                                TblJournalAccountType = 0,
                                EntityAccount = group.Key,
                                GlAccount = group.Key,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = false
                            };

                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company,
                                user);

                            foreach (var rr in list.Where(x => x.AccountTemp == @group.Key).GroupBy(w => w.GroupIserial))
                            {
                                var costcenter = new TblGlRuleDetail();
                                costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                                CreateTblLedgerDetailCostCenter(company, rr.Sum(w => w.NetSales),
                                    newledgerDetailrow, costcenter);
                            }
                        }

                        foreach (var group in list.GroupBy(x => x.CustIserial))
                        {
                            var temproww = entity.TblInventPostings.FirstOrDefault(
                                x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 101);
                            var salesgroupAccount = 0;
                            if (temproww != null)
                            {
                                salesgroupAccount = temproww.TblAccount;
                            }

                            if (salesgroupAccount == 0)
                            {
                                salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                                    x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 101).TblAccount;
                            }


                            foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                            {
                                variable.AccountTemp = salesgroupAccount;
                            }
                        }

                        foreach (var group in list.GroupBy(x => x.AccountTemp))
                        {

                            var NetsalesAmount = @group.Sum(x => x.NetSales);
                            if (taxPercentage > 0)
                            {
                                NetsalesAmount = @group.Sum(x => x.NetSales)* Math.Abs( (1-(decimal)taxPercentage));
                            }

                            //101 SalesTax Account
                            var newledgerDetailrowtax = new TblLedgerMainDetail
                            {
                                Amount = NetsalesAmount,
                                Description = "Tax",
                                ExchangeRate = 1,
                                TblCurrency = currency,
                                TransDate = fromDate,
                                TblJournalAccountType = 0,
                                EntityAccount = group.Key,
                                GlAccount = group.Key,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = false
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp,
                                company, user);

                            foreach (var rr in list.Where(x => x.AccountTemp == @group.Key).GroupBy(w => w.GroupIserial))
                            {
                                 var costcenter = new TblGlRuleDetail();

                                costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);
                                
                                CreateTblLedgerDetailCostCenter(company,   rr.Sum(e => e.NetSales) *Math.Abs(1 - (decimal)taxPercentage),
                                    newledgerDetailrowtax, costcenter);
                            }
                        }

                        // 1% tax 2/2/2022
                        //foreach (var group in list.GroupBy(x => x.CustIserial))
                        //{

                        //    var groupAccount = 0;
                        //    GetAccountByEntity(@group.Key, 1, company, 0, out groupAccount);
                        //    var newledgerDetailrow = new TblLedgerMainDetail
                        //    {
                        //        Amount  = @group.Sum(x => x.NetSales)*(decimal)0.01,//* (decimal)taxPercentage) - (@group.Sum(x => x.NetSales) * (decimal)0.01),

                        //        Description = "Sales Man tax",
                        //        ExchangeRate = 1,
                        //        TblCurrency = currency,
                        //        TransDate = fromDate,
                        //        TblJournalAccountType = 1,
                        //        EntityAccount = group.Key,
                        //        GlAccount = groupAccount,
                        //        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        //        PaymentRef = "",
                        //        DrOrCr = false
                        //    };
                        //    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company,
                        //        user);

                        //    foreach (var rr in list.Where(x => x.CustIserial == @group.Key).GroupBy(w => w.GroupIserial))
                        //    {
                        //        var costcenter = new TblGlRuleDetail();
                        //        costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                        //        if (group.Key != 0)
                        //        {
                        //            CreateTblLedgerDetailCostCenter(company, rr.Sum(x => x.NetSales) * (decimal)0.01 //* (decimal)taxPercentage) - (@group.Sum(x => x.NetSales) * (decimal)0.01)
                        //                , newledgerDetailrow,
                        //                costcenter);
                        //        }
                        //    }
                        //}


                        //foreach (var group in list.GroupBy(x => x.CustIserial))
                        //{
                        //    var temproww = entity.TblInventPostings.FirstOrDefault(
                        //        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 104);
                        //    var salesgroupAccount = 0;
                        //    if (temproww != null)
                        //    {
                        //        salesgroupAccount = temproww.TblAccount;
                        //    }

                        //    if (salesgroupAccount == 0)
                        //    {
                        //        salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                        //            x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 104).TblAccount;
                        //    }


                        //    foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                        //    {
                        //        variable.AccountTemp = salesgroupAccount;
                        //    }
                        //}

                        //foreach (var group in list.GroupBy(x => x.AccountTemp))
                        //{
                        //    //101 SalesTax Account
                        //    var newledgerDetailrowtax = new TblLedgerMainDetail
                        //    {
                        //        Amount = @group.Sum(x => x.NetSales) * (decimal)0.01,
                        //        Description = "Tax",
                        //        ExchangeRate = 1,
                        //        TblCurrency = currency,
                        //        TransDate = fromDate,
                        //        TblJournalAccountType = 0,
                        //        EntityAccount = group.Key,
                        //        GlAccount = group.Key,
                        //        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        //        PaymentRef = "",
                        //        DrOrCr = true
                        //    };
                        //    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp,
                        //        company, user);

                        //    foreach (var rr in list.Where(x => x.AccountTemp == @group.Key).GroupBy(w => w.GroupIserial))
                        //    {
                        //        var costcenter = new TblGlRuleDetail();

                        //        costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                        //        CreateTblLedgerDetailCostCenter(company, rr.Sum(e => e.NetSales) * (decimal)0.01,
                        //            newledgerDetailrowtax, costcenter);
                        //    }
                        //}

                        #endregion NetSales

                        #region Payment

                        foreach (var group in list.GroupBy(x => x.CustIserial))
                        {
                            var groupAccount = 0;
                            GetAccountByEntity(@group.Key, 1, company, 0, out groupAccount);
                            foreach (var variable in list.Where(x => x.CustIserial == group.Key))
                            {
                                variable.AccountTemp = groupAccount;
                            }
                        }

                        foreach (var group in list.GroupBy(x => x.CustIserial))
                        {
                            //var amount = (@group.Sum(x => x.NetSales)* (decimal)taxPercentage) - (@group.Sum(x => x.NetSales) * (decimal)0.01);
                            var newledgerDetailrow = new TblLedgerMainDetail
                            {
                                Amount = @group.Sum(x => x.NetSales)  * (decimal)taxPercentage,//* (decimal)taxPercentage) - (@group.Sum(x => x.NetSales) * (decimal)0.01),

                                Description = "Sales Man",
                                ExchangeRate = 1,
                                TblCurrency = currency,
                                TransDate = fromDate,
                                TblJournalAccountType = 1,
                                EntityAccount = group.Key,
                                GlAccount = group.First().AccountTemp,
                                TblLedgerHeader = newLedgerHeaderRow.Iserial,
                                PaymentRef = "",
                                DrOrCr = true
                            };
                            UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company,
                                user);

                            foreach (var rr in list.Where(x => x.CustIserial == @group.Key).GroupBy(w => w.GroupIserial))
                            {
                                var costcenter = new TblGlRuleDetail();
                                costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                                if (group.Key != 0)
                                {
                                    CreateTblLedgerDetailCostCenter(company, (rr.Sum(x => x.NetSales)  * (decimal)taxPercentage) //* (decimal)taxPercentage) - (@group.Sum(x => x.NetSales) * (decimal)0.01)
                                        , newledgerDetailrow,
                                        costcenter);
                                }
                            }
                        }

                        #endregion Payment
                        

                        CorrectLedgerHeaderRouding(newLedgerHeaderRow.Iserial, company, user);
                    }
                }

                #endregion Sales
            }
        }


        private void CostOfGoodSold(ccnewEntities entity, int currency, TblLedgerHeader newLedgerHeaderRow, DateTime day,
            string company, int user, List<SqlParameter> sqlParamw)
        {
            var listCostOfGoodsold =
                entity.ExecuteStoreQuery<GlPosting>(
                    "exec SpGlNetSalesCost @FromDate,@ToDate,@TableName,@TableNameField", sqlParamw.ToArray())
                    .ToList<GlPosting>();
            foreach (var group in listCostOfGoodsold.GroupBy(x => x.GroupIserial))
            {
                var groupAccount = 0;
                var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 73);
                if (tblInventPosting != null)
                    groupAccount =
                        tblInventPosting.TblAccount;

                if (groupAccount == 0)
                {
                    groupAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 73).TblAccount;
                }
                if (groupAccount == 0)
                {
                }
                foreach (var variable in listCostOfGoodsold.Where(x => x.GroupIserial == group.Key))
                {
                    variable.AccountTemp = groupAccount;
                }
            }
            foreach (var group in listCostOfGoodsold.GroupBy(x => x.AccountTemp))
            {
                // Sales order, consumption

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = @group.Sum(x => x.NetSales),
                    Description = "SalesCost",
                    ExchangeRate = 1,
                    TblCurrency = currency,
                    TransDate = day,
                    TblJournalAccountType = 0,
                    EntityAccount = group.Key,
                    GlAccount = group.Key,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = "",
                    DrOrCr = true
                };
                var temp = 0;
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);

                foreach (var rr in listCostOfGoodsold.Where(x => x.AccountTemp == @group.Key))
                {
                    var storeCostcenter = new TblGlRuleDetail();
                    storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial, company);

                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                    CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow, storeCostcenter);
                    CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow, costcenter);
                }
            }
            //Sales order, issue

            foreach (var group in listCostOfGoodsold.GroupBy(x => x.GroupIserial))
            {
                var temproww = entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 72);
                var salesgroupAccount = 0;
                if (temproww != null)
                {
                    salesgroupAccount = temproww.TblAccount;
                }

                if (salesgroupAccount == 0)
                {
                    //Sales order, issue
                    salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 72).TblAccount;
                }
                if (salesgroupAccount == 0)
                {
                }
                foreach (var variable in listCostOfGoodsold.Where(x => x.GroupIserial == group.Key))
                {
                    variable.AccountTemp = salesgroupAccount;
                }
            }

            foreach (var group in listCostOfGoodsold.GroupBy(x => x.AccountTemp))
            {
                var temp = 0;
                var newledgerDetailrowtax = new TblLedgerMainDetail
                {
                    Amount = @group.Sum(x => x.NetSales),
                    Description = "SalesCost",
                    ExchangeRate = 1,
                    TblCurrency = currency,
                    TransDate = day,
                    TblJournalAccountType = 0,
                    EntityAccount = group.Key,
                    GlAccount = group.Key,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = "",
                    DrOrCr = false
                };

                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp, company, user);

                foreach (var rr in listCostOfGoodsold.Where(x => x.AccountTemp == @group.Key))
                {
                    var storeCostcenter = new TblGlRuleDetail();
                    storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial, company);

                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, rr.GroupIserial, company);

                    CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrowtax, storeCostcenter);
                    CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrowtax, costcenter);
                }
            }
        }


        private void CostOfGoodSold(ccnewEntities entity, int currency, TblLedgerHeader newLedgerHeaderRow, DateTime day,
           string company, int user, List<GlPosting> listCostOfGoodsold)
        {
            foreach (var group in listCostOfGoodsold.GroupBy(x => x.GroupIserial))
            {
                var groupAccount = 0;
                var tblInventPosting = entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 73);
                if (tblInventPosting != null)
                    groupAccount =
                        tblInventPosting.TblAccount;

                if (groupAccount == 0)
                {
                    groupAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 73).TblAccount;
                }
                if (groupAccount == 0)
                {
                }
                foreach (var variable in listCostOfGoodsold.Where(x => x.GroupIserial == group.Key))
                {
                    variable.AccountTemp = groupAccount;
                }
            }
            foreach (var group in listCostOfGoodsold.GroupBy(x => x.AccountTemp))
            {
                // Sales order, consumption

                var newledgerDetailrow = new TblLedgerMainDetail
                {
                    Amount = @group.Sum(x => x.DrAmount),
                    Description = "SalesCost",
                    ExchangeRate = 1,
                    TblCurrency = currency,
                    TransDate = day,
                    TblJournalAccountType = 0,
                    EntityAccount = group.Key,
                    GlAccount = group.Key,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = "",
                    DrOrCr = true
                };
                var temp = 0;
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp, company, user);

                foreach (var rr in listCostOfGoodsold.Where(x => x.AccountTemp == @group.Key).GroupBy(w => w.GroupIserial))
                {
                    //var storeCostcenter = new TblGlRuleDetail();
                    //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial, company);

                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                    //CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrow, storeCostcenter);
                    CreateTblLedgerDetailCostCenter(company, rr.Sum(w => w.DrAmount), newledgerDetailrow, costcenter);
                }
            }
            //Sales order, issue

            foreach (var group in listCostOfGoodsold.GroupBy(x => x.GroupIserial))
            {
                var temproww = entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == @group.Key && x.TblInventAccountType == 72);
                var salesgroupAccount = 0;
                if (temproww != null)
                {
                    salesgroupAccount = temproww.TblAccount;
                }

                if (salesgroupAccount == 0)
                {
                    //Sales order, issue
                    salesgroupAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 72).TblAccount;
                }
                if (salesgroupAccount == 0)
                {
                }
                foreach (var variable in listCostOfGoodsold.Where(x => x.GroupIserial == group.Key))
                {
                    variable.AccountTemp = salesgroupAccount;
                }
            }

            foreach (var group in listCostOfGoodsold.GroupBy(x => x.AccountTemp))
            {
                var temp = 0;
                var newledgerDetailrowtax = new TblLedgerMainDetail
                {
                    Amount = @group.Sum(x => x.DrAmount),
                    Description = "SalesCost",
                    ExchangeRate = 1,
                    TblCurrency = currency,
                    TransDate = day,
                    TblJournalAccountType = 0,
                    EntityAccount = group.Key,
                    GlAccount = group.Key,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    PaymentRef = "",
                    DrOrCr = false
                };

                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowtax, true, 000, out temp, company, user);

                foreach (var rr in listCostOfGoodsold.Where(x => x.AccountTemp == @group.Key).GroupBy(w => w.GroupIserial))
                {
                    //var storeCostcenter = new TblGlRuleDetail();
                    //storeCostcenter = FindCostCenterByType(storeCostcenter, 8, rr.StoreIserial, company);

                    var costcenter = new TblGlRuleDetail();
                    costcenter = FindCostCenterByType(costcenter, 0, rr.Key, company);

                    //CreateTblLedgerDetailCostCenter(company, rr.NetSales, newledgerDetailrowtax, storeCostcenter);
                    CreateTblLedgerDetailCostCenter(company, rr.Sum(w => w.DrAmount), newledgerDetailrowtax, costcenter);
                }
            }
        }

        [OperationContract]
        private void GlOpen(DateTime toDate, int user, string company, int tblaccount, bool repost)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var Type = 6;

                var oldLedgers =
                    entity.TblLedgerHeaders.Where(
                        x => x.TblTransactionType == Type && x.DocDate.Value == toDate);
                foreach (var variable in oldLedgers)
                {
                    entity.TblLedgerHeaders.DeleteObject(variable);
                }
                entity.SaveChanges();

                #region Paramters

                var sqlParam = new List<SqlParameter>
                {
                    new SqlParameter
                    {
                        ParameterName = "ToDate",
                        Value = toDate,
                        SqlDbType = SqlDbType.Date
                    },
                };

                #endregion Paramters

                var list =
                    entity.ExecuteStoreQuery<GlPosting>(
                        "exec SpGlOpening @ToDate",
                        sqlParam.ToArray()).ToList<GlPosting>();

                var journal =
                    entity.tblChainSetupTests.FirstOrDefault(
                        x => x.sGlobalSettingCode == "GLOpeningJournal").sSetupValue;

                var journalint =
                    entity.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                var newLedgerHeaderRow = new TblLedgerHeader
                {
                    CreatedBy = user,
                    CreationDate = DateTime.Now,
                    Description = "Opening",
                    DocDate = toDate,
                    TblJournal = journalint,
                    TblTransactionType = Type,
                    TblJournalLink = 0,
                };
                var temp = 0;
                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out temp, user, company);

                var currency = DefaultCurrency(company);

                foreach (
                    var group in list.Where(x => x.DrAmount > 0).GroupBy(x => x.GroupIserial))
                {
                    var newledgerDetailrow = new TblLedgerMainDetail
                    {
                        Amount = @group.FirstOrDefault().DrAmount,
                        Description = "Open",
                        ExchangeRate = 1,
                        TblCurrency = currency,
                        TransDate = toDate,
                        TblJournalAccountType = 0,
                        EntityAccount = group.Key,
                        GlAccount = group.Key,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = "",
                        DrOrCr = false
                    };
                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                        company, user);

                    var newledgerDetailrowOffset = new TblLedgerMainDetail
                    {
                        Amount = @group.FirstOrDefault().DrAmount,
                        Description = "Open",
                        ExchangeRate = 1,
                        TblCurrency = currency,
                        TransDate = toDate,
                        TblJournalAccountType = 0,
                        EntityAccount = tblaccount,
                        GlAccount = tblaccount,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = "",
                        DrOrCr = true
                    };
                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                        company, user);

                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowOffset, true, 000, out temp,
                        company, user);
                    foreach (var rr in list.Where(x => x.DrAmount > 0 && x.GroupIserial == group.Key))
                    {
                        var costcenter = entity.TblCostCenters.FirstOrDefault(x => x.Iserial == rr.TblCostCenter);
                        var newrow = new TblLedgerDetailCostCenter
                        {
                            Ratio = 0,
                            TblLedgerMainDetail = newledgerDetailrow.Iserial,
                            Amount = (double)rr.CostCenterAmount,
                            TblCostCenter = rr.TblCostCenter,
                            TblCostCenterType = costcenter.TblCostCenterType
                        };
                        entity.TblLedgerDetailCostCenters.AddObject(newrow);
                        var newrowOffset = new TblLedgerDetailCostCenter
                        {
                            Ratio = 0,
                            TblLedgerMainDetail = newledgerDetailrowOffset.Iserial,
                            Amount = (double)rr.CostCenterAmount,
                            TblCostCenter = rr.TblCostCenter,
                            TblCostCenterType = costcenter.TblCostCenterType
                        };
                        entity.TblLedgerDetailCostCenters.AddObject(newrowOffset);
                    }
                }

                foreach (
                    var group in list.Where(x => x.CrAmount < 0).GroupBy(x => x.GroupIserial))
                {
                    var newledgerDetailrow = new TblLedgerMainDetail
                    {
                        Amount = @group.FirstOrDefault().CrAmount * -1,
                        Description = "Open",
                        ExchangeRate = 1,
                        TblCurrency = currency,
                        TransDate = toDate,
                        TblJournalAccountType = 0,
                        EntityAccount = group.Key,
                        GlAccount = group.Key,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = "",
                        DrOrCr = true
                    };
                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                        company, user);

                    var newledgerDetailrowOffset = new TblLedgerMainDetail
                    {
                        Amount = @group.FirstOrDefault().CrAmount * -1,
                        Description = "Open",
                        ExchangeRate = 1,
                        TblCurrency = currency,
                        TransDate = toDate,
                        TblJournalAccountType = 0,
                        EntityAccount = tblaccount,
                        GlAccount = tblaccount,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        PaymentRef = "",
                        DrOrCr = true
                    };
                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrow, true, 000, out temp,
                        company, user);

                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowOffset, true, 000, out temp,
                        company, user);
                    foreach (var rr in list.Where(x => x.CrAmount < 0 && x.GroupIserial == group.Key))
                    {
                        var costcenter = entity.TblCostCenters.FirstOrDefault(x => x.Iserial == rr.TblCostCenter);
                        var newrow = new TblLedgerDetailCostCenter
                        {
                            Ratio = 0,
                            TblLedgerMainDetail = newledgerDetailrow.Iserial,
                            Amount = (double)rr.CostCenterAmount * -1,
                            TblCostCenter = rr.TblCostCenter,
                            TblCostCenterType = costcenter.TblCostCenterType
                        };
                        entity.TblLedgerDetailCostCenters.AddObject(newrow);
                        var newrowOffset = new TblLedgerDetailCostCenter
                        {
                            Ratio = 0,
                            TblLedgerMainDetail = newledgerDetailrowOffset.Iserial,
                            Amount = (double)rr.CostCenterAmount * -1,
                            TblCostCenter = rr.TblCostCenter,
                            TblCostCenterType = costcenter.TblCostCenterType
                        };
                        entity.TblLedgerDetailCostCenters.AddObject(newrowOffset);
                    }
                }
            }
        }

        [OperationContract]
        private List<GlPosting> GetIncomeStatmentData(DateTime fromDate, DateTime toDate, int costCenterType, int CostCenter, string company, string code, out List<TblIncomeStatmentDesignDetail> DesignDetailList, out List<TblIncomeTax> IncomeTax)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.CommandTimeout = 0;
                //  var AccountISerial = entity.Entities.Where(w => w.TblJournalAccountType == JournalType).Select(w => w.AccountIserial).Distinct();

                IncomeTax = entity.TblIncomeTaxes.ToList();
                DesignDetailList =
                    entity.TblIncomeStatmentDesignDetails.Where(x => x.TblIncomeStatmentDesignHeader1.Name == code)
                        .ToList();
                var glPosting = new List<GlPosting>();
                foreach (var variable in DesignDetailList.Where(x => x.Type.StartsWith("Account")))
                {
                    var account = entity.TblAccounts.Include("TblAccountIntervals").FirstOrDefault(x => x.Code == variable.Description);
                    if (account != null)
                    {
                        variable.Description = account.Ename;
                        if (account.TblAccountIntervals.Any())
                        {
                            var fromRange = account.TblAccountIntervals.FirstOrDefault().FromRange;
                            var toRange = account.TblAccountIntervals.FirstOrDefault().ToRange;
                            var accounts =
                                entity.TblAccounts.Where(
                                    x => x.Code.CompareTo(fromRange) > 0 &&
                                         x.Code.CompareTo(toRange) < 0
                                    ).Select(w => w.Iserial);
                            var ledgersDr = entity.TblLedgerDetails.Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && accounts.Contains(x.GlAccount) && x.DrOrCr).ToList();

                            foreach (var VARIABLE in ledgersDr)
                            {
                                var glpostingrow = new GlPosting();
                                glpostingrow.GroupIserial = account.Iserial;
                                glpostingrow.DrAmount =
                                    Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                                glpostingrow.Description = account.Ename;
                                glpostingrow.AccountPerRow = account;
                                glPosting.Add(glpostingrow);
                            }
                            var ledgersCr = entity.TblLedgerDetails.Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && accounts.Contains(x.GlAccount) && x.DrOrCr == false).ToList();

                            foreach (var VARIABLE in ledgersCr)
                            {
                                var glpostingrow = new GlPosting();
                                glpostingrow.GroupIserial = account.Iserial;
                                glpostingrow.CrAmount =
                                    Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                                glpostingrow.Description = account.Ename;
                                glpostingrow.AccountPerRow = account;
                                glPosting.Add(glpostingrow);
                            }
                            if (variable.Type == "Account With Subs")
                            {
                                foreach (var VARIABLE in ledgersDr)
                                {
                                    var craccount = entity.TblAccounts.FirstOrDefault(x => x.Iserial == VARIABLE.GlAccount);
                                    var glpostingrow = new GlPosting();
                                    glpostingrow.GroupIserial = VARIABLE.GlAccount;
                                    glpostingrow.DrAmount =
                                        Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                                    glpostingrow.Description = craccount.Ename;
                                    glpostingrow.AccountPerRow = craccount;
                                    glpostingrow.AccountTemp = account.Iserial;
                                    glPosting.Add(glpostingrow);
                                }

                                foreach (var VARIABLE in ledgersCr)
                                {
                                    var craccount = entity.TblAccounts.FirstOrDefault(x => x.Iserial == VARIABLE.GlAccount);
                                    var glpostingrow = new GlPosting();
                                    glpostingrow.GroupIserial = VARIABLE.GlAccount;
                                    glpostingrow.CrAmount =
                                        Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                                    glpostingrow.Description = craccount.Ename;
                                    glpostingrow.AccountPerRow = craccount;
                                    glpostingrow.AccountTemp = account.Iserial;
                                    glPosting.Add(glpostingrow);
                                }
                            }
                        }
                        var ledgersDraa =
                            entity.TblLedgerDetails.Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && x.GlAccount == account.Iserial && x.DrOrCr).ToList();

                        foreach (var VARIABLE in ledgersDraa)
                        {
                            var glpostingrow = new GlPosting();
                            glpostingrow.GroupIserial = account.Iserial;
                            glpostingrow.DrAmount =
                                Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                            glpostingrow.Description = account.Ename;
                            glpostingrow.AccountPerRow = account;
                            glPosting.Add(glpostingrow);
                        }

                        var ledgersCraa =
                            entity.TblLedgerDetails.Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && x.GlAccount == account.Iserial && x.DrOrCr == false).ToList();
                        foreach (var VARIABLE in ledgersCraa)
                        {
                            var glpostingrow = new GlPosting();
                            glpostingrow.GroupIserial = account.Iserial;
                            glpostingrow.CrAmount =
                                Convert.ToDecimal(VARIABLE.Amount * Convert.ToDecimal(VARIABLE.ExchangeRate));
                            glpostingrow.Description = account.Ename;
                            glpostingrow.AccountPerRow = account;
                            glPosting.Add(glpostingrow);
                        }
                    }
                }
                var glPostingReturn = new List<GlPosting>();
                foreach (var variable in glPosting.GroupBy(x => x.GroupIserial))
                {
                    var glpostingrow = new GlPosting
                    {
                        GroupIserial = variable.Key,
                        CrAmount = variable.Sum(x => x.CrAmount),
                        DrAmount = variable.Sum(x => x.DrAmount),
                        Description = variable.FirstOrDefault().Description,
                        AccountPerRow = variable.FirstOrDefault().AccountPerRow
                    };

                    glPostingReturn.Add(glpostingrow);
                }

                return glPostingReturn;
            }
        }

        [OperationContract]
        private List<GlPosting> GetIncomeStatmentDataCostCenter(DateTime fromDate, DateTime toDate, int CostCenter, string company, string code, out List<TblIncomeStatmentDesignDetail> DesignDetailList, out List<TblIncomeTax> IncomeTax)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.CommandTimeout = 0;
                IncomeTax = entity.TblIncomeTaxes.ToList();
                DesignDetailList = entity.TblIncomeStatmentDesignDetails.Where(x => x.TblIncomeStatmentDesignHeader1.Name == code).ToList();
                var glPosting = new List<GlPosting>();
                foreach (var variable in DesignDetailList.Where(x => x.Type.StartsWith("Account")))
                {
                    var account = entity.TblAccounts.Include("TblAccountIntervals").FirstOrDefault(x => x.Code == variable.Description);
                    if (account != null)
                    {
                        variable.Description = account.Ename;
                        if (account.TblAccountIntervals.Any())
                        {
                            var fromRange = account.TblAccountIntervals.FirstOrDefault().FromRange;
                            var toRange = account.TblAccountIntervals.FirstOrDefault().ToRange;
                            var accounts = entity.TblAccounts.Where(x => x.Code.CompareTo(fromRange) > 0 && x.Code.CompareTo(toRange) < 0).Select(w => w.Iserial);
                            var ledgersDr = entity.TblLedgerDetailCostCenters.Include("TblLedgerMainDetail1").Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblCostCenter == CostCenter && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && accounts.Contains(x.TblLedgerMainDetail1.GlAccount) && x.TblLedgerMainDetail1.DrOrCr).ToList();

                            foreach (var VARIABLE in ledgersDr)
                            {
                                var glpostingrow = new GlPosting();
                                glpostingrow.GroupIserial = account.Iserial;
                                glpostingrow.DrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                                glpostingrow.Description = account.Ename;
                                glpostingrow.AccountPerRow = account;
                                glPosting.Add(glpostingrow);
                            }
                            var ledgersCr = entity.TblLedgerDetailCostCenters.Include("TblLedgerMainDetail1").Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblCostCenter == CostCenter && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && accounts.Contains(x.TblLedgerMainDetail1.GlAccount) && x.TblLedgerMainDetail1.DrOrCr == false).ToList();

                            foreach (var VARIABLE in ledgersCr)
                            {
                                var glpostingrow = new GlPosting();
                                glpostingrow.GroupIserial = account.Iserial;
                                glpostingrow.CrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                                glpostingrow.Description = account.Ename;
                                glpostingrow.AccountPerRow = account;
                                glPosting.Add(glpostingrow);
                            }
                            if (variable.Type == "Account With Subs")
                            {
                                foreach (var VARIABLE in ledgersDr)
                                {
                                    var craccount = entity.TblAccounts.FirstOrDefault(x => x.Iserial == VARIABLE.TblLedgerMainDetail1.GlAccount);
                                    var glpostingrow = new GlPosting();
                                    glpostingrow.GroupIserial = VARIABLE.TblLedgerMainDetail1.GlAccount;
                                    glpostingrow.DrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                                    glpostingrow.Description = craccount.Ename;
                                    glpostingrow.AccountPerRow = craccount;
                                    glpostingrow.AccountTemp = account.Iserial;
                                    glPosting.Add(glpostingrow);
                                }

                                foreach (var VARIABLE in ledgersCr)
                                {
                                    var craccount = entity.TblAccounts.FirstOrDefault(x => x.Iserial == VARIABLE.TblLedgerMainDetail1.GlAccount);
                                    var glpostingrow = new GlPosting();
                                    glpostingrow.GroupIserial = VARIABLE.TblLedgerMainDetail1.GlAccount;
                                    glpostingrow.CrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                                    glpostingrow.Description = craccount.Ename;
                                    glpostingrow.AccountPerRow = craccount;
                                    glpostingrow.AccountTemp = account.Iserial;
                                    glPosting.Add(glpostingrow);
                                }
                            }
                        }
                        var ledgersDraa =
                            entity.TblLedgerDetailCostCenters.Include("TblLedgerMainDetail1").Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblCostCenter == CostCenter && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && x.TblLedgerMainDetail1.GlAccount == account.Iserial && x.TblLedgerMainDetail1.DrOrCr).ToList();

                        foreach (var VARIABLE in ledgersDraa)
                        {
                            var glpostingrow = new GlPosting();
                            glpostingrow.GroupIserial = account.Iserial;
                            glpostingrow.DrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                            glpostingrow.Description = account.Ename;
                            glpostingrow.AccountPerRow = account;
                            glPosting.Add(glpostingrow);
                        }

                        var ledgersCraa =
                            entity.TblLedgerDetailCostCenters.Include("TblLedgerMainDetail1").Where(x => x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate && x.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= toDate && x.TblCostCenter == CostCenter && x.TblLedgerMainDetail1.TblLedgerHeader1.balanced && x.TblLedgerMainDetail1.GlAccount == account.Iserial && x.TblLedgerMainDetail1.DrOrCr == false).ToList();
                        foreach (var VARIABLE in ledgersCraa)
                        {
                            var glpostingrow = new GlPosting();
                            glpostingrow.GroupIserial = account.Iserial;
                            glpostingrow.CrAmount = (decimal)VARIABLE.Amount * Convert.ToDecimal(VARIABLE.TblLedgerMainDetail1.ExchangeRate);
                            glpostingrow.Description = account.Ename;
                            glpostingrow.AccountPerRow = account;
                            glPosting.Add(glpostingrow);
                        }
                    }
                }
                var glPostingReturn = new List<GlPosting>();
                foreach (var variable in glPosting.GroupBy(x => x.GroupIserial))
                {
                    var glpostingrow = new GlPosting
                    {
                        GroupIserial = variable.Key,
                        CrAmount = variable.Sum(x => x.CrAmount),
                        DrAmount = variable.Sum(x => x.DrAmount),
                        Description = variable.FirstOrDefault().Description,
                        AccountPerRow = variable.FirstOrDefault().AccountPerRow
                    };
                    glPostingReturn.Add(glpostingrow);
                }
                return glPostingReturn;
            }
        }


        private int RecalcCostCenter(string company)
        {
            int count = 0;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.ViewCostCenterShopAreas.MergeOption = MergeOption.NoTracking;
                entity.CommandTimeout = 0;
                var distinctBrandAll = entity.ViewCostCenterShopAreas.ToList();
                int CostCenterType = Convert.ToInt32(GetRetailChainSetupByCode("ShopAreaCostCenterType", company).sSetupValue);
                // Expenses
                var JournalType = 15;
                //      var ledgers = entity.TblLedgerMainDetails.Include("TblLedgerHeader1").Include("TblLedgerDetailCostCenters").Where(x => x.Iserial == 6077 && x.TblJournalAccountType == JournalType && x.TblLedgerHeader1.balanced && (!x.TblLedgerDetailCostCenters.Any(w => w.TblCostCenterType == CostCenterType))).Take(5).ToList();

                count = entity.TblLedgerMainDetails.Include("TblLedgerHeader1").Include("TblLedgerDetailCostCenters.TblCostCenterType1").Where(x => x.TblJournalAccountType == JournalType && x.TblLedgerHeader1.balanced && (x.TblLedgerDetailCostCenters.All(w => w.Calculated == false))).Count();

                var ledgers = entity.TblLedgerMainDetails.Include("TblLedgerHeader1").Include("TblLedgerDetailCostCenters.TblCostCenterType1").Where(x => x.TblJournalAccountType == JournalType && x.TblLedgerHeader1.balanced && (x.TblLedgerDetailCostCenters.All(w => w.Calculated == false))).Take(1000).ToList();

                //  count = ledgers.Count();

                var costcentertypes = entity.TblCostCenterTypes.Select(w => w.Iserial).ToList();
                foreach (var item in ledgers)
                {
                    var NoOfCostCenterTypes = item.TblLedgerDetailCostCenters.Select(w => w.TblCostCenterType).Distinct().Count();
                    var distinctBrand = distinctBrandAll.Where(w => w.ClosedDate > item.TblLedgerHeader1.DocDate || w.ClosedDate == null).Select(w => w.BrandCostCenter).Distinct();

                    #region NotStore


                    //CostCenter Type Not Base (Not Stores)
                    if (!item.TblLedgerDetailCostCenters.Any(w => w.TblCostCenterType == CostCenterType))
                    {
                        if (item.TblLedgerDetailCostCenters.Any() && item.TblLedgerDetailCostCenters.Any(w => distinctBrand.Contains(w.TblCostCenter)))
                        {
                            foreach (var BrandCostCenter in item.TblLedgerDetailCostCenters.Where(w => w.Calculated == false).ToList())
                            {
                                var ListToCalculate = distinctBrandAll.Where(w => (w.ClosedDate > item.TblLedgerHeader1.DocDate || w.ClosedDate == null) && w.BrandCostCenter == BrandCostCenter.TblCostCenter);
                                var TotalArea = ListToCalculate.Sum(w => w.areab);
                                double amount = 0;
                                // inserting Stores ( Base Cost Center)
                                if (item.TblLedgerDetailCostCenters.Where(w => w.Calculated == false).All(w => w.TblCostCenterType != CostCenterType))
                                {
                                    foreach (var Store in ListToCalculate)
                                    {
                                        amount = (double)(((decimal)BrandCostCenter.Amount * Store.areab) / TotalArea);
                                        var newcostcenter = new TblLedgerDetailCostCenter()
                                        {
                                            TblCostCenter = Store.StoreCostCenter,
                                            TblCostCenterType = CostCenterType,
                                            Calculated = true,
                                            TblLedgerMainDetail = item.Iserial,
                                            Ratio = 0,
                                            Amount = amount
                                        };
                                        entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                                    }
                                }

                                var BrandList = entity.ViewShopAreaBrands.Where(w => w.BrandCostCenter == BrandCostCenter.TblCostCenter && (w.ClosedDate > item.TblLedgerHeader1.DocDate || w.ClosedDate == null)).Select(w => w.BrandIserial).ToList();
                                // inserting Brands 
                                var BrandListToInsert = entity.ViewShopAreaBrands.Where(w => BrandList.Contains(w.BrandIserial)).ToList();
                                if (BrandListToInsert.Any())
                                {
                                    foreach (var BrandItem in BrandListToInsert.Where(w => w.AllowGeneralReDistribution).Select(w => w.BrandCostCenterType).Distinct())
                                    {
                                        if (item.TblLedgerDetailCostCenters.Any() && item.TblLedgerDetailCostCenters.Where(w => w.Calculated == false).All(w => w.TblCostCenterType != BrandItem))
                                        {
                                            TotalArea = BrandListToInsert.Where(w => w.BrandCostCenterType == BrandItem).Sum(w => w.areab);

                                            foreach (var BrandTOInsert in BrandListToInsert.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == BrandItem))
                                            {
                                                // BrandTOInsert.areab;
                                                //amount = (double)item.Amount / BrandListToInsert.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == BrandItem).Count();

                                                amount = (double)(((decimal)BrandCostCenter.Amount * BrandTOInsert.areab) / TotalArea);
                                                var newcostcenter = new TblLedgerDetailCostCenter()
                                                {
                                                    TblCostCenter = BrandTOInsert.BrandCostCenter,
                                                    TblCostCenterType = BrandTOInsert.BrandCostCenterType,
                                                    Calculated = true,
                                                    TblLedgerMainDetail = item.Iserial,
                                                    Ratio = 0,
                                                    Amount = amount
                                                };
                                                entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (var Store in distinctBrandAll.Where(w => w.AllowGeneralReDistribution == false).GroupBy(q => q.BrandCostCenterType))
                            {
                                double amount = 0;

                                if (item.TblLedgerDetailCostCenters.Any() && item.TblLedgerDetailCostCenters.All(w => w.TblCostCenterType != Store.Key))
                                {
                                    int costcenter = Store.FirstOrDefault().DefaultCostCenter ?? 0;
                                    amount = (double)(item.Amount);
                                    var newcostcenter = new TblLedgerDetailCostCenter()
                                    {
                                        TblCostCenter = costcenter,
                                        TblCostCenterType = Store.Key,
                                        Calculated = true,
                                        TblLedgerMainDetail = item.Iserial,
                                        Ratio = 0,
                                        Amount = amount
                                    };
                                    entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                                }
                            }
                        }
                    }
                    #endregion
                    //CostCenter Type Is Base (Stores)
                    #region Base(Stores)


                    if (item.TblLedgerDetailCostCenters.All(w => w.TblCostCenterType == CostCenterType)) //&& NoOfCostCenterTypes != costcentertypes.Count)
                    {
                        foreach (var BrandCostCenter in item.TblLedgerDetailCostCenters.Where(w => w.Calculated == false).ToList())
                        {
                            var ListToCalculate = distinctBrandAll.Where(w => w.AllowGeneralReDistribution && (w.ClosedDate > item.TblLedgerHeader1.DocDate || w.ClosedDate == null) && w.StoreCostCenter == BrandCostCenter.TblCostCenter && w.BrandCostCenterType != CostCenterType);
                            foreach (var LoopOnCostCenterType in ListToCalculate.Select(w => w.BrandCostCenterType).Distinct())
                            {
                                var TotalArea = ListToCalculate.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == LoopOnCostCenterType).Sum(w => w.areab);

                                double amount = 0;
                                foreach (var Store in ListToCalculate.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == LoopOnCostCenterType).ToList())
                                {
                                    amount = (BrandCostCenter.Amount * (double)Store.areab / (double)TotalArea);
                                    //    foreach (var Store in ListToCalculate)
                                    //{
                                    //    amount = (double)(((decimal)BrandCostCenter.Amount * Store.areab) / TotalArea);
                                    var newcostcenter = new TblLedgerDetailCostCenter()
                                    {
                                        TblCostCenter = Store.BrandCostCenter,
                                        TblCostCenterType = Store.BrandCostCenterType,
                                        Calculated = true,
                                        TblLedgerMainDetail = item.Iserial,
                                        Ratio = 0,
                                        Amount = amount
                                    };
                                    entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                                }
                            }
                        }
                        foreach (var Store in distinctBrandAll.Where(w => w.AllowGeneralReDistribution == false).GroupBy(q => q.BrandCostCenterType))
                        {
                            double amount = 0;

                            if (item.TblLedgerDetailCostCenters.Any() && item.TblLedgerDetailCostCenters.All(w => w.TblCostCenterType != Store.Key))
                            {
                                int costcenter = Store.FirstOrDefault().DefaultCostCenter ?? 0;
                                amount = (double)(item.Amount);
                                var newcostcenter = new TblLedgerDetailCostCenter()
                                {
                                    TblCostCenter = costcenter,
                                    TblCostCenterType = Store.Key,
                                    Calculated = true,
                                    TblLedgerMainDetail = item.Iserial,
                                    Ratio = 0,
                                    Amount = amount
                                };
                                entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                            }
                        }
                    }
                    #endregion

                    #region 

                    #endregion
                    if (!item.TblLedgerDetailCostCenters.Any() || item.TblLedgerDetailCostCenters.All(w => w.TblCostCenterType1.AllowGeneralReDistribution == false))
                    {
                        var costcentertypeslist = item.TblLedgerDetailCostCenters.Select(w => w.TblCostCenterType).ToList();
                        var ListToCalculate = distinctBrandAll.Where(w => !costcentertypeslist.Contains(w.BrandCostCenterType) && (w.ClosedDate > item.TblLedgerHeader1.DocDate || w.ClosedDate == null));
                        double amount = 0;
                        foreach (var LoopOnCostCenterType in ListToCalculate.Select(w => w.BrandCostCenterType).Distinct())
                        {
                            var TotalArea = ListToCalculate.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == LoopOnCostCenterType).Sum(w => w.areab);

                            foreach (var Store in ListToCalculate.Where(w => w.AllowGeneralReDistribution && w.BrandCostCenterType == LoopOnCostCenterType).ToList())
                            {
                                amount = (double)(item.Amount * Store.areab / TotalArea);
                                //var newcostcenter = new TblLedgerDetailCostCenter()
                                //{
                                //    TblCostCenter = Store.StoreCostCenter,
                                //    TblCostCenterType = CostCenterType,
                                //    Calculated = true,
                                //    TblLedgerMainDetail = item.Iserial,
                                //    Ratio = 0,
                                //    Amount = amount
                                //};

                                var newcostcenterBrand = new TblLedgerDetailCostCenter()
                                {
                                    TblCostCenter = Store.BrandCostCenter,
                                    TblCostCenterType = Store.BrandCostCenterType,
                                    Calculated = true,
                                    TblLedgerMainDetail = item.Iserial,
                                    Ratio = 0,
                                    Amount = amount
                                };
                                entity.TblLedgerDetailCostCenters.AddObject(newcostcenterBrand);
                                //  entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                            }
                        }

                        foreach (var Store in ListToCalculate.Where(w => w.AllowGeneralReDistribution == false).GroupBy(q => q.BrandCostCenterType))
                        {

                            if (item.TblLedgerDetailCostCenters.Any() && item.TblLedgerDetailCostCenters.All(w => w.TblCostCenterType != Store.Key))
                            {
                                int costcenter = Store.FirstOrDefault().DefaultCostCenter ?? 0;
                                amount = (double)(item.Amount);
                                var newcostcenter = new TblLedgerDetailCostCenter()
                                {
                                    TblCostCenter = costcenter,
                                    TblCostCenterType = Store.Key,
                                    Calculated = true,
                                    TblLedgerMainDetail = item.Iserial,
                                    Ratio = 0,
                                    Amount = amount
                                };
                                entity.TblLedgerDetailCostCenters.AddObject(newcostcenter);
                            }

                        }
                    }
                }
                entity.SaveChanges();
            }
            //  RecalcCostCenterBrand(company);
            return count;
        }

    }
}