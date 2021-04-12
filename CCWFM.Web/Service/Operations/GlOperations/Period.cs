using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using LinqKit;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblGlPeriod> GetTblPeriod(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlPeriod> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlPeriods.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlPeriods.Include(nameof(TblGlPeriod.TblAccount1)).
                        Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlPeriods.Count();
                    query = entity.TblGlPeriods.Include(nameof(TblGlPeriod.TblAccount1)).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlPeriod UpdateOrInsertTblperiod(TblGlPeriod newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlPeriods.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlPeriods
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
        private void GeneratePeriodLines(TblGlPeriod period, DateTime fromDate, DateTime toDate, string company)
        {
            try
            {
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var days = toDate.Subtract(fromDate).TotalDays;
                    var month = ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month;
                    var year = toDate.Year - fromDate.Year;
                    var periodLine = from s in entity.TblGlPeriodLines
                                     where s.TblPeriod == period.Iserial
                                     select s;

                    foreach (var item in periodLine)
                    {
                        entity.DeleteObject(item);
                    }
                    entity.SaveChanges();
                    TblGlPeriodLine tblPeriodLine;

                    if (period.PeriodUnit == 0)
                    {
                        for (var i = 0; i <= days; i++)
                        {
                            tblPeriodLine = new TblGlPeriodLine
                            {
                                TblPeriod = period.Iserial,

                                FromDate = fromDate.AddDays(i),
                                ToDate = fromDate.AddDays(i),
                                Ename =
                                    fromDate.AddDays(i).ToString("dddd", new System.Globalization.CultureInfo("en-US")),
                                Aname =
                                    fromDate.AddDays(i).ToString("dddd", new System.Globalization.CultureInfo("ar-EG")),
                            };

                            entity.TblGlPeriodLines.AddObject(tblPeriodLine);
                        }
                    }
                    else if (period.PeriodUnit == 1)
                    {
                        for (var i = 0; i <= month; i++)
                        {
                            tblPeriodLine = new TblGlPeriodLine
                            {
                                TblPeriod = period.Iserial,

                                FromDate = fromDate.AddMonths(i),
                                ToDate = i == month ? toDate : fromDate.AddMonths(i + 1).Subtract(TimeSpan.FromDays(1)),
                                Ename = fromDate.AddMonths(i).ToString("MMMM", new System.Globalization.CultureInfo("en-US")),
                                Aname = fromDate.AddMonths(i).ToString("MMM",
                                new System.Globalization.CultureInfo("ar-EG"))
                            };

                            entity.TblGlPeriodLines.AddObject(tblPeriodLine);
                        }
                    }
                    else
                    {
                        for (var i = 0; i <= year; i++)
                        {
                            tblPeriodLine = new TblGlPeriodLine
                            {
                                TblPeriod = period.Iserial,

                                FromDate = fromDate.AddYears(i),
                                ToDate = i == year ? toDate : fromDate.AddYears(i + 1).Subtract(TimeSpan.FromDays(1)),
                                Ename = fromDate.AddYears(i).Year.ToString(),
                                Aname = fromDate.AddYears(i).Year.ToString()
                            };

                            entity.TblGlPeriodLines.AddObject(tblPeriodLine);
                        }
                    }

                    entity.SaveChanges();
                }
            }
            catch
            {
            }
        }

        [OperationContract]
        private int DeleteTblPeriod(TblGlPeriod row, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblGlPeriods.SingleOrDefault(e => e.Iserial == row.Iserial);
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlPeriodLine> GetTblPeriodLine(int skip, int take, int periodId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlPeriodLine> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    if (periodId != 0)
                    {
                        filter = filter + " and it.TblPeriod ==(@TblPeriod0)";
                        valuesObjects.Add("TblPeriod0", periodId);
                    }

                    fullCount = entity.TblGlPeriodLines.Where(filter, parameterCollection).Count();
                    query = entity.TblGlPeriodLines.OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlPeriodLines.Count(v => v.TblPeriod == periodId);
                    query = entity.TblGlPeriodLines.Where(x => x.TblPeriod == periodId || periodId == 0).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlPeriodLine UpdateOrInsertCspPeriodLine(TblGlPeriodLine newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlPeriodLines.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlPeriodLines
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
        private int DeleteCspPeriodLine(TblGlPeriodLine row, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblGlPeriodLines.SingleOrDefault(e => e.Iserial == row.Iserial);
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
        [OperationContract]
        private TblGlPeriod ClosePeriod(int Iserial, string company, int user)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var period = entity.TblGlPeriods.FirstOrDefault(w => w.Iserial == Iserial);
                period.Closed = true;
                period.ClosedDate = DateTime.Now;
                var periodLines = entity.TblGlPeriodLines.Where(w => w.TblPeriod == period.Iserial).ToList();

                var fromDate = periodLines.OrderBy(w => w.FromDate).FirstOrDefault().FromDate;
                var ToDate = periodLines.OrderByDescending(w => w.ToDate).FirstOrDefault().ToDate;
                int temp = 0;
                var accounts = entity.TblAccounts.Where(wde => wde.Code.StartsWith("4")).Select(t => t.Iserial).ToList();

                var ToDateMod = ToDate.AddDays(1);
                var oldLedgers =
                           entity.TblLedgerHeaders.Where(
                               x => x.TblTransactionType == 13 && x.DocDate.Value == ToDateMod).ToList();
                foreach (var variable in oldLedgers)
                {
                    entity.TblLedgerHeaders.DeleteObject(variable);
                }
                entity.SaveChanges();

                var ClosingPeriod =
                           entity.tblChainSetupTests.FirstOrDefault(
                               x => x.sGlobalSettingCode == "GlClosingPeriod")
                               .sSetupValue;
                var journalint = entity.TblJournals.FirstOrDefault(x => x.Code == ClosingPeriod).Iserial;

                var CostOfGoodSoldAccount = entity.TblInventPostings.FirstOrDefault(
                        x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 73).TblAccount;
                var LedgerDetails = entity.TblLedgerMainDetails.Where(w => (w.TblJournalAccountType == 15 ||
                (w.TblJournalAccountType == 0 && w.EntityAccount == CostOfGoodSoldAccount) ||
                (w.TblJournalAccountType == 0 && accounts.Contains(w.EntityAccount ?? 0))

                ) && w.TblLedgerHeader1.DocDate >= fromDate && w.TblLedgerHeader1.DocDate <= ToDate)
                    .GroupBy(x => new { x.EntityAccount, x.GlAccount, x.TblCurrency, x.TblJournalAccountType })
                    .ToList();

                var LedgerDetailTotal = entity.TblLedgerMainDetails.Where(w => (w.TblJournalAccountType == 15 ||
             (w.TblJournalAccountType == 0 && w.EntityAccount == CostOfGoodSoldAccount) ||
             (w.TblJournalAccountType == 0 && accounts.Contains(w.EntityAccount ?? 0))

             ) && w.TblLedgerHeader1.DocDate >= fromDate && w.TblLedgerHeader1.DocDate <= ToDate)
                 .GroupBy(x => new { x.TblCurrency })
                 .ToList();

                var ListOfLedgerMainDetails = new List<TblLedgerMainDetail>();
                var newLedgerHeaderRow = new TblLedgerHeader
                {
                    CreatedBy = user,
                    CreationDate = DateTime.Now,
                    Description = "Closing",
                    DocDate = ToDateMod,
                    TblJournal = journalint,
                    TblTransactionType = 13,
                    TblJournalLink = 0,
                };
                UpdateOrInsertTblLedgerHeaders(entity, newLedgerHeaderRow, true, 0, out temp, user);
                var drorcr = true;
                decimal? amount = 0;
                foreach (var item in LedgerDetails)
                {
                    //var drAmount = item.Where(w => w.DrOrCr == true).Sum(w => w.Amount * (decimal)w.ExchangeRate) ?? 0;
                    //var CrAmount = item.Where(w => w.DrOrCr == false).Sum(w => w.Amount * (decimal)w.ExchangeRate) ?? 0;
                    var drAmount = item.Where(w => w.DrOrCr == true).Sum(w => w.Amount) ?? 0;
                    var CrAmount = item.Where(w => w.DrOrCr == false).Sum(w => w.Amount) ?? 0;

                    var drExAmount = item.Sum(w => (double)w.Amount * w.ExchangeRate) / (double)item.Sum(w => w.Amount);
                    //var CrExAmount = item.Where(w => w.DrOrCr == false).Sum(w => w.Amount * (decimal)w.ExchangeRate) ?? 0 / item.Where(w => w.DrOrCr == false).Sum(w => w.Amount);


                    if (drAmount >= CrAmount)
                    {
                        drorcr = false;
                        amount = drAmount - CrAmount;
                    }
                    else
                    {
                        drorcr = true;
                        amount = CrAmount - drAmount;
                    }

                    var Testrow = new TblLedgerMainDetail()
                    {
                        Amount = amount,
                        ExchangeRate = drExAmount,
                        Description = "",
                        DrOrCr = drorcr,
                        GlAccount = item.Key.GlAccount,
                        TransDate = ToDate,
                        TblJournalAccountType = item.Key.TblJournalAccountType,
                        EntityAccount = item.Key.EntityAccount,
                        TblCurrency = item.Key.TblCurrency,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };

                    ListOfLedgerMainDetails.Add(Testrow);
                }
                foreach (var item in ListOfLedgerMainDetails)
                {
                    UpdateOrInsertTblLedgerMainDetails(item, true, 000, out temp, company,
                                              user);
                }

                foreach (var item in LedgerDetailTotal)
                {

                    var drAmount = item.Where(w => w.DrOrCr == true).Sum(w => w.Amount) ?? 0;
                    var CrAmount = item.Where(w => w.DrOrCr == false).Sum(w => w.Amount) ?? 0;

                    var drExAmount = item.Sum(w => (double)w.Amount * w.ExchangeRate) / (double)item.Sum(w => w.Amount);

                    if (drAmount >= CrAmount)
                    {
                        drorcr = true;
                        amount = drAmount - CrAmount;
                    }
                    else
                    {
                        drorcr = false;
                        amount = CrAmount - drAmount;
                    }

                    var LedgerDetailRow = new TblLedgerMainDetail()
                    {
                        Amount = amount,
                        ExchangeRate = drExAmount,
                        Description = "",
                        DrOrCr = drorcr,
                        GlAccount = period.TblAccount ?? 0,
                        TransDate = ToDate,
                        TblJournalAccountType = 0,
                        EntityAccount = period.TblAccount ?? 0,
                        TblCurrency = DefaultCurrency(company),
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };
                    UpdateOrInsertTblLedgerMainDetails(LedgerDetailRow, true, 000, out temp, company, user);
                }

                return period;
            }
        }

        [OperationContract]
        private TblGlPeriodLine ClosePeriodline(int iserial, string company, int user)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.CommandTimeout = 0;
                var periodLines = entity.TblGlPeriodLines.FirstOrDefault(w => w.Iserial == iserial);
                var fromDate = periodLines.FromDate;
                var ToDate = periodLines.ToDate;

                var oldLedgers =
                           entity.TblLedgerHeaders.Where(
                               x => x.TblTransactionType == 14 && x.DocDate.Value == ToDate).ToList();
                foreach (var variable in oldLedgers)
                {
                    entity.TblLedgerHeaders.DeleteObject(variable);
                }
                entity.SaveChanges();
                var DefaultCostCenter = entity.TblCostCenters.FirstOrDefault(w => w.Code == "2001");
                var ClosingPeriod =
                  entity.tblChainSetupTests.FirstOrDefault(
                      x => x.sGlobalSettingCode == "GlClosingPeriod")
                      .sSetupValue;
                var journalint = entity.TblJournals.FirstOrDefault(x => x.Code == ClosingPeriod).Iserial;
                ClosingMonthly(user, fromDate, ToDate, journalint, entity, company, DefaultCostCenter);
                MaterialsJournal(user, fromDate, ToDate, journalint, entity, company, DefaultCostCenter);
                ExternalVendorJournal(user, fromDate, ToDate, journalint, entity, company, DefaultCostCenter);
                OtherExpenses(user, fromDate, ToDate, journalint, entity, company, DefaultCostCenter);
                ReportAsAFinish(user, fromDate, ToDate, journalint, entity, company, DefaultCostCenter);
                return periodLines;
            }
        }
        private void ClosingMonthly(int user, DateTime fromDate, DateTime ToDate, int journalint, ccnewEntities entity, string company, TblCostCenter DefaultCostCenter)
        {
            var ListOfLedgerMainDetails = new List<TblLedgerMainDetail>();
            int temp = 0;
            var CostCenterOrganizationUnitList = entity.TblCostCenterOrganizationUnits.ToList();
            var CostCenterRouteGroupList = entity.TblCostCenterRouteGroups.ToList();
            var OrganizationUnitList = CostCenterOrganizationUnitList.Select(w => w.TblOrganizationUnit).Distinct().ToList();
            var SalaryTermEntityAccountList = entity.TblSalaryTermEntityAccounts.ToList();

            var newLedgerHeaderRow = new TblLedgerHeader
            {
                CreatedBy = user,
                CreationDate = DateTime.Now,
                Description = "Closing Monthly Payroll",
                DocDate = ToDate,
                TblJournal = journalint,
                TblTransactionType = 14,
                TblJournalLink = 0,
            };
            UpdateOrInsertTblLedgerHeaders(entity, newLedgerHeaderRow, true, 0, out temp, user);

            using (var context = new PayrollEntities())
            {
                context.CommandTimeout = 0;
                var PaySlipList = context.TblPayslips.Where(w => w.TransDate >= fromDate
                  && w.TransDate <= ToDate
                  && OrganizationUnitList.Contains(w.TblOrganizationUnit))
                  .GroupBy(w => new { w.TblOrganizationUnit, w.TblSalaryTerm })
                  .ToList();
                var InsuranceAccount = entity.Entities.FirstOrDefault(w => w.Code == "320701" && w.TblJournalAccountType == 15 && w.scope == 0);
                using (var WorkFlowcontext = new WorkFlowManagerDBEntities())
                {
                    WorkFlowcontext.CommandTimeout = 0;
                    WorkFlowcontext.FIXAvgCostInventTrans();
                    double totalAmount = 0;
                    foreach (var PaySlipRecord in PaySlipList)
                    {
                        totalAmount += PaySlipRecord.Sum(w => w.Amount);
                    }

                    var OldRouteCardDetailActualCost =
                 WorkFlowcontext.RouteCardDetailActualCosts.Where(
                     w => w.TransDate >= fromDate && w.TransDate <= ToDate
                     ).ToList();
                    foreach (var variable in OldRouteCardDetailActualCost)
                    {
                        WorkFlowcontext.RouteCardDetailActualCosts.DeleteObject(variable);
                    }

                    var SalesOrderOperations = WorkFlowcontext.TblSalesOrderOperations.ToList();
                    var predicate = PredicateBuilder.True<RouteCardDetail>();
                    predicate = predicate.And(i => i.RouteCardHeader.Vendor == "000");
                    predicate = predicate.And(w =>
                              w.RouteCardHeader.DocDate >= fromDate && w.RouteCardHeader.DocDate <= ToDate
                              && w.RouteCardHeader.Direction == 1 && w.RouteCardHeader.RouteType == 5 &&
                              w.SizeQuantity > 0 && w.RouteCardHeader.RouteIncluded == true);
                    var EstimatedList = (from p in WorkFlowcontext.RouteCardDetails.AsExpandable().Where(predicate)
                         .GroupBy(e =>
                                 new
                                 {
                                     e.Iserial,
                                     e.RouteCardHeaderIserial,
                                     e.RouteCardHeader.RoutGroupID,
                                     e.RouteCardHeader.TransID,
                                     e.TblSalesOrder,
                                     e.TblColor,
                                     e.RouteCardHeader.SupplierInv,
                                     e.RouteCardHeader.DocDate
                                 })
                                         let Cost = WorkFlowcontext.TblSalesOrderOperations.FirstOrDefault(y => y.TblOperation == p.Key.RoutGroupID && y.TblSalesOrder == p.Key.TblSalesOrder).OprCost * p.Where(w => w.TblColor == p.Key.TblColor).Sum(v => v.SizeQuantity)
                                         select new
                                         {
                                             Iserial = p.Key.Iserial,
                                             RouteCardHeaderIserial = p.Key.RouteCardHeaderIserial,
                                             TblSalesOrder = p.Key.TblSalesOrder,
                                             TblColor = p.Key.TblColor,
                                             TblRouteGroup = p.Key.RoutGroupID,
                                             Qty = p.Sum(v => v.SizeQuantity),
                                             Cost = Cost,
                                             DocDate = p.Key.DocDate
                                         }).ToList();
                    var CostByCostCenter = new Dictionary<int, double>();

                    foreach (var itemRow in CostCenterOrganizationUnitList)
                    {
                        var detailItems = PaySlipList.Where(w => w.Key.TblOrganizationUnit == itemRow.TblOrganizationUnit).ToList();
                        foreach (var Details in detailItems)
                        {

                            if (CostByCostCenter.ContainsKey(itemRow.TblCostCenter))
                            {
                                CostByCostCenter[itemRow.TblCostCenter] += Details.Sum(w => w.Amount);
                            }
                            else
                            {
                                CostByCostCenter.Add(itemRow.TblCostCenter, Details.Sum(w => w.Amount));
                            }
                        }
                    }

                    foreach (var CostCenter in CostByCostCenter)
                    {
                        var RouteGroupList = CostCenterRouteGroupList.Where(w => w.TblCostCenter == CostCenter.Key).Select(w => w.TblRouteGroup).Distinct();
                        var EstimatedCost = EstimatedList.Where(w => RouteGroupList.Contains(w.TblRouteGroup)).ToList();
                        var SumTotal = EstimatedList.Where(w => RouteGroupList.Contains(w.TblRouteGroup)).Sum(w => w.Cost);
                        foreach (var item in EstimatedCost.Where(w => w.Cost != null && w.Cost != 0))
                        {
                            var unitCost = item.Cost;
                            var ActualOprCost = ((float)CostCenter.Value / (float)(SumTotal)) * unitCost;

                            var OperationRow = SalesOrderOperations.FirstOrDefault(w => w.TblSalesOrder == item.TblSalesOrder && w.TblOperation == item.TblRouteGroup);
                            var RouteCardDetailActualCostRow = new RouteCardDetailActualCost()
                            {
                                TblCostCenter = CostCenter.Key,
                                RouteCardHeader = item.RouteCardHeaderIserial,
                                TblSalesOrder = item.TblSalesOrder,
                                TblColor = item.TblColor,
                                RouteCardDetails = item.Iserial,
                                ActualOperationCost = ActualOprCost,
                                TransDate = item.DocDate,
                                RouteCardDetailActualCostType = 1
                            };
                            WorkFlowcontext.RouteCardDetailActualCosts.AddObject(RouteCardDetailActualCostRow);
                        }
                    }
                    WorkFlowcontext.SaveChanges();
                    var RouteCardDetailActualCosts = WorkFlowcontext.RouteCardDetailActualCosts.Where(w => w.TransDate >= fromDate && w.TransDate <= ToDate).ToList();


                    var salesorders = RouteCardDetailActualCosts.Select(w => w.TblSalesOrder).ToList();
                    foreach (var CostCenter in CostByCostCenter)
                    {
                        var ActualCostTotal = RouteCardDetailActualCosts.Where(w => w.TblCostCenter == CostCenter.Key).Sum(w => w.ActualOperationCost);
                        var DifAmount = CostCenter.Value - ActualCostTotal;
                        var EstimateCost = EstimatedList.Where(w => salesorders.Contains(w.TblSalesOrder)).ToList();

                        foreach (var item in EstimateCost.GroupBy(w => new { w.TblSalesOrder, w.TblColor }))
                        {
                            var SalesOrderCost = item.Sum(wde => wde.Cost);
                            var ActualOprCost = ((float)DifAmount / (float)(EstimateCost.Sum(w => w.Cost))) * SalesOrderCost;

                            //var OperationRow = SalesOrderOperations.FirstOrDefault(w => w.TblSalesOrder == item.TblSalesOrder && w.TblOperation == item.TblRouteGroup);
                            var RouteCardDetailActualCostRow = new RouteCardDetailActualCost()
                            {
                                TblCostCenter = CostCenter.Key,
                                TblSalesOrder = item.Key.TblSalesOrder,
                                TblColor = item.Key.TblColor,
                                ActualOperationCost = ActualOprCost,
                                TransDate = ToDate,
                                RouteCardDetailActualCostType = 1
                            };
                            WorkFlowcontext.RouteCardDetailActualCosts.AddObject(RouteCardDetailActualCostRow);
                        }
                    }
                    WorkFlowcontext.SaveChanges();
                }

                foreach (var item in PaySlipList.GroupBy(w => w.Key.TblSalaryTerm))
                {
                    var account = SalaryTermEntityAccountList.FirstOrDefault(w => w.TblSalaryTerm == item.Key);
                    var journaltype = account.TbljournalAccountType;
                    var entityIserial = account.EntityAccount;
                    var listToSum = PaySlipList.Where(w => w.Key.TblSalaryTerm == item.Key);
                    var GlAccount = entity.Entities.FirstOrDefault(w => w.TblJournalAccountType == journaltype && w.Iserial == entityIserial).AccountIserial;

                    decimal amount = 0;
                    decimal EmpInsurance = 0;
                    decimal CompInsurance = 0;
                    foreach (var PaySlip in listToSum)
                    {
                        amount = amount + Convert.ToDecimal(PaySlip.Sum(w => w.Amount));

                        EmpInsurance = EmpInsurance + Convert.ToDecimal(PaySlip.Sum(w => w.ConstInsurane + w.VarInsurane));
                        CompInsurance = CompInsurance + Convert.ToDecimal(PaySlip.Sum(w => w.ConstInsuraneComp + w.VARInsuraneComp));
                    }
                    if (CompInsurance != 0)
                    {
                        var CompInsuranceRow = new TblLedgerMainDetail()
                        {
                            Amount = CompInsurance * -1,
                            ExchangeRate = 1,
                            Description = "",
                            DrOrCr = true,
                            GlAccount = InsuranceAccount.AccountIserial,
                            TransDate = ToDate,
                            TblJournalAccountType = InsuranceAccount.TblJournalAccountType,
                            EntityAccount = InsuranceAccount.Iserial,
                            TblCurrency = DefaultCurrency(company),
                            TblLedgerHeader = newLedgerHeaderRow.Iserial
                        };
                        ListOfLedgerMainDetails.Add(CompInsuranceRow);

                        CompInsuranceRow.TblLedgerDetailCostCenters = new System.Data.Objects.DataClasses.EntityCollection<TblLedgerDetailCostCenter>();

                        foreach (var PaySlip in PaySlipList.Where(w => w.Key.TblSalaryTerm == item.Key))
                        {
                            var CostCenterAmount = Math.Abs(PaySlip.Sum(w => w.Amount));
                            foreach (var CostCenter in FindCostCenterByOrganizationUnit(PaySlip.Key.TblOrganizationUnit, CostCenterOrganizationUnitList, entity))
                            {
                                var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                                {
                                    Ratio = 0,
                                    Amount = CostCenterAmount,
                                    TblCostCenter = CostCenter.Iserial,
                                    TblCostCenterType = CostCenter.TblCostCenterType,
                                };
                                CompInsuranceRow.TblLedgerDetailCostCenters.Add(markupVendorLedgerCostCenter);
                            }
                        }
                    }

                    if (EmpInsurance != 0)
                    {
                        var EmpInsuranceRow = new TblLedgerMainDetail()
                        {
                            Amount = CompInsurance * -1,
                            ExchangeRate = 1,
                            Description = "",
                            DrOrCr = false,
                            GlAccount = GlAccount,
                            TransDate = ToDate,
                            TblJournalAccountType = journaltype,
                            EntityAccount = entityIserial,
                            TblCurrency = DefaultCurrency(company),
                            TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        };
                        ListOfLedgerMainDetails.Add(EmpInsuranceRow);

                    }

                    var drOrCr = true;
                    if (amount < 0)
                    {
                        drOrCr = false;
                        amount = amount * -1;
                    }

                    var Testrow = new TblLedgerMainDetail()
                    {
                        Amount = amount,
                        ExchangeRate = 1,
                        Description = "",
                        DrOrCr = drOrCr,
                        GlAccount = GlAccount,
                        TransDate = ToDate,
                        TblJournalAccountType = journaltype,
                        EntityAccount = entityIserial,
                        TblCurrency = DefaultCurrency(company),
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };
                    Testrow.TblLedgerDetailCostCenters = new System.Data.Objects.DataClasses.EntityCollection<TblLedgerDetailCostCenter>();
                    foreach (var PaySlip in PaySlipList.Where(w => w.Key.TblSalaryTerm == item.Key))
                    {
                        var CostCenterAmount = Math.Abs(PaySlip.Sum(w => w.Amount));
                        foreach (var CostCenter in FindCostCenterByOrganizationUnit(PaySlip.Key.TblOrganizationUnit, CostCenterOrganizationUnitList, entity))
                        {
                            var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                            {
                                Ratio = 0,
                                Amount = CostCenterAmount,
                                TblCostCenter = CostCenter.Iserial,
                                TblCostCenterType = CostCenter.TblCostCenterType,
                            };
                            Testrow.TblLedgerDetailCostCenters.Add(markupVendorLedgerCostCenter);
                        }
                    }

                    ListOfLedgerMainDetails.Add(Testrow);
                }

                var TotalDrAmount = ListOfLedgerMainDetails.Where(w => w.DrOrCr == true).Sum(w => w.Amount);
                var TotalCrAmount = ListOfLedgerMainDetails.Where(w => w.DrOrCr == false).Sum(w => w.Amount);
                var TotalAccount = entity.Entities.FirstOrDefault(w => w.Code == "220701" && w.TblJournalAccountType == 14 && w.scope == 0);
                var totalRecord = new TblLedgerMainDetail()
                {
                    Amount = TotalDrAmount - TotalCrAmount,
                    ExchangeRate = 1,
                    Description = "",
                    DrOrCr = false,
                    GlAccount = TotalAccount.AccountIserial,
                    TransDate = ToDate,
                    TblJournalAccountType = TotalAccount.TblJournalAccountType,
                    EntityAccount = TotalAccount.Iserial,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                };
                ListOfLedgerMainDetails.Add(totalRecord);
                foreach (var item in ListOfLedgerMainDetails.GroupBy(w => new { w.EntityAccount, w.GlAccount, w.TblJournalAccountType, w.DrOrCr }))
                {
                    var NewRow = new TblLedgerMainDetail()
                    {
                        Amount = item.Sum(r => r.Amount),
                        ExchangeRate = 1,
                        Description = "",
                        DrOrCr = item.Key.DrOrCr,
                        GlAccount = item.Key.GlAccount,
                        TransDate = ToDate,
                        TblJournalAccountType = item.Key.TblJournalAccountType,
                        EntityAccount = item.Key.EntityAccount,
                        TblCurrency = DefaultCurrency(company),
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };
                    UpdateOrInsertTblLedgerMainDetails(NewRow, true, 000, out temp, company,
                                              user);
                    if (item.Key.TblJournalAccountType == 15)
                    {
                        var NewLedgerCostCenterList = new List<TblLedgerDetailCostCenter>();
                        foreach (var CostCenter in item.Select(w => w.TblLedgerDetailCostCenters).ToList())
                        {
                            foreach (var CostCenterRow in CostCenter)
                            {
                                var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                                {
                                    Ratio = 0,
                                    TblLedgerMainDetail = NewRow.Iserial,
                                    Amount = CostCenterRow.Amount,
                                    TblCostCenter = CostCenterRow.TblCostCenter,
                                    TblCostCenterType = CostCenterRow.TblCostCenterType,
                                };
                                NewLedgerCostCenterList.Add(markupVendorLedgerCostCenter);
                            }

                        }
                        foreach (var NewLedgerCostCenter in NewLedgerCostCenterList.GroupBy(w => w.TblCostCenter))
                        {
                            var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                            {
                                Ratio = 0,
                                TblLedgerMainDetail = NewRow.Iserial,
                                Amount = NewLedgerCostCenter.Sum(w => w.Amount),
                                TblCostCenter = NewLedgerCostCenter.Key,
                                TblCostCenterType = NewLedgerCostCenter.FirstOrDefault().TblCostCenterType,
                            };
                            UpdateOrInsertTblLedgerDetailCostCenters(markupVendorLedgerCostCenter, true, 000,
                                                        out temp,user, company);
                        }
                        var DefaultCostAmount = (double)NewRow.Amount;
                        var DefaultLedgerCostCenter = new TblLedgerDetailCostCenter
                        {
                            Ratio = 0,
                            TblLedgerMainDetail = NewRow.Iserial,
                            Amount = DefaultCostAmount,
                            TblCostCenter = DefaultCostCenter.Iserial,
                            TblCostCenterType = DefaultCostCenter.TblCostCenterType,
                        };
                        UpdateOrInsertTblLedgerDetailCostCenters(DefaultLedgerCostCenter, true, 000,
                                                    out temp, user, company);
                    }
                }
            }
        }
        private void MaterialsJournal(int user, DateTime fromDate, DateTime ToDate, int journalint, ccnewEntities entity, string company, TblCostCenter DefaultCostCenter)
        {
            var ListOfLedgerMainDetails = new List<TblLedgerMainDetail>();
            int temp = 0;
            var CostCenterOrganizationUnitList = entity.TblCostCenterOrganizationUnits.ToList();
            var CostCenterRouteGroupList = entity.TblCostCenterRouteGroups.ToList();
            var OrganizationUnitList = CostCenterOrganizationUnitList.Select(w => w.TblOrganizationUnit).Distinct().ToList();
            var SalaryTermEntityAccountList = entity.TblSalaryTermEntityAccounts.ToList();

            var newLedgerHeaderRow = new TblLedgerHeader
            {
                CreatedBy = user,
                CreationDate = DateTime.Now,
                Description = "Closing Monthly Materials",
                DocDate = ToDate,
                TblJournal = journalint,
                TblTransactionType = 14,
                TblJournalLink = 0,
            };
            UpdateOrInsertTblLedgerHeaders(entity, newLedgerHeaderRow, true, 0, out temp, user);
            var types = new List<int?>();

            types.Add(25);
            types.Add(26);
            var TotalAccount = entity.TblAccounts.FirstOrDefault(w => w.Code == "120105");

            using (var WorkFlowcontext = new WorkFlowManagerDBEntities())
            {
                WorkFlowcontext.CommandTimeout = 0;
                var FabricUnit = WorkFlowcontext.Fabric_UnitID.Select(wde => new { wde.GroupIserial, wde.Type }).Distinct().ToList();
                var InventTransList = WorkFlowcontext.TblInventTrans.Include(nameof(TblInventTran.TblItemDim))
                      .Join(WorkFlowcontext.RouteCardFabrics, // the source table of the inner join
      InventTrans => InventTrans.VotGlSerial,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
      RouteCardFabric => RouteCardFabric.Iserial,   // Select the foreign key (the second part of the "on" clause)
     (InventTrans, RouteCardFabric) => new { InventTrans = InventTrans, RouteCardFabric = RouteCardFabric }) // selection
                    .Where(w => w.InventTrans.DocDate >= fromDate
                    && w.InventTrans.DocDate <= ToDate && types.Contains(w.InventTrans.TblInventType) && w.InventTrans.LastStoreAvgCost != 0).GroupBy(e =>
                                  new
                                  {
                                      e.InventTrans.VotGlSerial,
                                      e.InventTrans.DocDate,
                                      e.InventTrans.TblItemDim,
                                      e.RouteCardFabric.TblSalesOrder,
                                      e.RouteCardFabric.StyleColor,
                                      e.RouteCardFabric.ItemGroup
                                      //e.InventTrans.InventTrans.GroupIserial,
                                  })
                                 .Select(p => new
                                 {
                                     TblSalesOrder = p.Key.TblSalesOrder,
                                     StyleColor = p.Key.StyleColor,
                                     Qty = p.Sum(v => v.InventTrans.Qty * v.InventTrans.LastStoreAvgCost),
                                     Cost = p.Sum(v => v.InventTrans.Qty * v.InventTrans.LastStoreAvgCost),
                                     DocDate = p.Key.DocDate,
                                     ItemGroup = p.Key.ItemGroup,
                                     //GroupIserial = p.Key.GroupIserial,
                                 }).ToList();

                foreach (var item in InventTransList)
                {
                    var Cost = (double)item.Cost;
                    var RouteCardDetailActualCostRow = new RouteCardDetailActualCost()
                    {
                        TblCostCenter = DefaultCostCenter.Iserial,
                        TblSalesOrder = item.TblSalesOrder,
                        TblColor = item.StyleColor,
                        ActualOperationCost = Cost,
                        TransDate = ToDate,
                        RouteCardDetailActualCostType = 2
                    };
                    var GroupIserial = FabricUnit.FirstOrDefault(w => w.Type == item.ItemGroup).GroupIserial;
                    WorkFlowcontext.RouteCardDetailActualCosts.AddObject(RouteCardDetailActualCostRow);
                    var groupAccount =
                entity.TblInventPostings.FirstOrDefault(
                    x => x.ItemScopeRelation == GroupIserial && x.TblInventAccountType == 60);
                    if (groupAccount == null)
                    {
                        groupAccount =
                        entity.TblInventPostings.FirstOrDefault(x => x.ItemScopeRelation == -1 && x.TblInventAccountType == 60);
                    }

                    var totalRecord = new TblLedgerMainDetail()
                    {
                        Amount = item.Qty * item.Cost,
                        ExchangeRate = 1,
                        Description = "",
                        DrOrCr = false,
                        GlAccount = groupAccount.TblAccount,
                        TransDate = ToDate,
                        TblJournalAccountType = 0,
                        EntityAccount = groupAccount.TblAccount,
                        TblCurrency = DefaultCurrency(company),
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };
                    ListOfLedgerMainDetails.Add(totalRecord);

                    totalRecord = new TblLedgerMainDetail()
                    {
                        Amount = item.Qty * item.Cost,
                        ExchangeRate = 1,
                        Description = "",
                        DrOrCr = true,
                        GlAccount = TotalAccount.Iserial,
                        TransDate = ToDate,
                        TblJournalAccountType = 0,
                        EntityAccount = TotalAccount.Iserial,
                        TblCurrency = DefaultCurrency(company),
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    };
                    ListOfLedgerMainDetails.Add(totalRecord);



                }
                WorkFlowcontext.SaveChanges();
            }
            GroupLedgerMainDetail(ListOfLedgerMainDetails, company, newLedgerHeaderRow.Iserial, user);

            entity.SaveChanges();
        }
        private void ReportAsAFinish(int user, DateTime fromDate, DateTime ToDate, int journalint, ccnewEntities entity, string company, TblCostCenter DefaultCostCenter)
        {
            int temp = 0;

            var newLedgerHeaderRow = new TblLedgerHeader
            {
                CreatedBy = user,
                CreationDate = DateTime.Now,
                Description = "Closing Report As a Finish",
                DocDate = ToDate,
                TblJournal = journalint,
                TblTransactionType = 14,
                TblJournalLink = 0,
            };
            UpdateOrInsertTblLedgerHeaders(entity, newLedgerHeaderRow, true, 0, out temp, user);
            //مخزون إنتاج تام تحت التشغيل
            var UnderProduction = entity.TblAccounts.FirstOrDefault(w => w.Code == "120105").Iserial;
            //مخزون الانتاج التام
            var Production = entity.TblAccounts.FirstOrDefault(w => w.Code == "120107").Iserial;
            using (var WorkFlowcontext = new WorkFlowManagerDBEntities())
            {
                WorkFlowcontext.CommandTimeout = 0;
                var Tempsalesorders = WorkFlowcontext.RouteCardDetails.Include("TblSalesOrder1").Where(w => w.RouteCardHeader.RouteType == 3 && w.RouteCardHeader.DocDate >= fromDate
                   && w.RouteCardHeader.DocDate <= ToDate).GroupBy(e =>
                                    new
                                    {
                                        e.TblSalesOrder,
                                        e.TblSalesOrder1.TblStyle,
                                        e.TblColor
                                    }).Select(w => new { RoutGroupID = w.Key.TblStyle, TblSalesOrder = w.Key.TblSalesOrder, TblColor = w.Key.TblColor, SizeQuantity = w.Sum(e => e.SizeQuantity) });
                //var salesorders = new List<RouteCardDetail>();

                //Dictionary<int, int> SalesOrderDic = new Dictionary<int, int>();
                //foreach (var item in Tempsalesorders)
                //{
                //    salesorders.Add(new RouteCardDetail()
                //    {
                //        TblSalesOrder = item.TblSalesOrder,
                //        RoutGroupID = item.RoutGroupID,
                //        TblColor = item.TblColor,
                //        SizeQuantity = item.SizeQuantity,
                //    });

                //    if (!SalesOrderDic.Keys.Contains(item.TblSalesOrder??0))
                //    {
                //        SalesOrderDic.Add(item.TblSalesOrder??0, item.TblColor);
                //    }
                //}
                var RouteCardDetailActualCostList = WorkFlowcontext.RouteCardDetailActualCosts.Where(w =>
                   Tempsalesorders.Any(a => a.TblSalesOrder == w.TblSalesOrder && a.TblColor == w.TblColor)
                   && w.RouteCardDetailActualCostType != 4 &&
                 w.Status == 0).ToList().GroupBy(e =>
                                    new
                                    {
                                        e.TblSalesOrder,
                                        e.TblColor
                                    })
                                 .Select(p => new
                                 {
                                     TblSalesOrder = p.Key.TblSalesOrder,
                                     StyleColor = p.Key.TblColor,
                                     Cost = p.Sum(w => w.ActualOperationCost)
                                 }).ToList();

                var itemDims = WorkFlowcontext.TblItemDims.Where(w => w.ItemType == "FP").ToList();
                var TotalCost = Convert.ToDecimal(RouteCardDetailActualCostList.Sum(w => w.Cost));
                var salesorders = Tempsalesorders.ToList();
                var totalRecord = new TblLedgerMainDetail()
                {
                    Amount = TotalCost,
                    ExchangeRate = 1,
                    Description = "",
                    DrOrCr = false,
                    GlAccount = UnderProduction,
                    TransDate = ToDate,
                    TblJournalAccountType = 0,
                    EntityAccount = UnderProduction,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                };
                UpdateOrInsertTblLedgerMainDetails(entity, totalRecord, true, 0, out temp, user);

                var testRecord = new TblLedgerMainDetail()
                {
                    Amount = TotalCost,
                    ExchangeRate = 1,
                    Description = "",
                    DrOrCr = true,
                    GlAccount = Production,
                    TransDate = ToDate,
                    TblJournalAccountType = 0,
                    EntityAccount = Production,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                };
                UpdateOrInsertTblLedgerMainDetails(entity, testRecord, true, 0, out temp, user);
                foreach (var item in RouteCardDetailActualCostList)
                {
                    var SalesOrderRecord = salesorders.FirstOrDefault(w => w.TblSalesOrder == item.TblSalesOrder && w.TblColor == item.StyleColor);
                    var ItemCost = (decimal)item.Cost / SalesOrderRecord.SizeQuantity.Value;
                    var styleItemDims = itemDims.Where(w => w.ItemIserial == SalesOrderRecord.RoutGroupID && w.TblColor == SalesOrderRecord.TblColor).Select(w => w.Iserial).Distinct().ToList();
                    UpdateInventTrans(styleItemDims, ItemCost, WorkFlowcontext);
                    foreach (var RouteCardDetailActualCost in WorkFlowcontext.RouteCardDetailActualCosts.Where(e => e.TblSalesOrder == item.TblSalesOrder && e.TblColor == item.StyleColor))
                    {
                        RouteCardDetailActualCost.Status = 1;
                    }
                }
                WorkFlowcontext.SaveChanges();
            }
            entity.SaveChanges();
        }
        private void ExternalVendorJournal(int user, DateTime fromDate, DateTime ToDate, int journalint, ccnewEntities entity, string company, TblCostCenter DefaultCostCenter)
        {
            int temp = 0;

            var newLedgerHeaderRow = new TblLedgerHeader
            {
                CreatedBy = user,
                CreationDate = DateTime.Now,
                Description = "Closing External Vendor",
                DocDate = ToDate,
                TblJournal = journalint,
                TblTransactionType = 14,
                TblJournalLink = 0,
            };
            UpdateOrInsertTblLedgerHeaders(entity, newLedgerHeaderRow, true, 0, out temp, user);
            //مخزون إنتاج تام تحت التشغيل
            var UnderProduction = entity.TblAccounts.FirstOrDefault(w => w.Code == "120105").Iserial;
            //خدمة  خارجي
            var Production = entity.TblAccounts.FirstOrDefault(w => w.Code == "120111").Iserial;
            using (var WorkFlowcontext = new WorkFlowManagerDBEntities())
            {
                WorkFlowcontext.CommandTimeout = 0;
                var Tempsalesorders = WorkFlowcontext.RouteCardInvoiceDetails.Include("TblSalesOrder1").Where(w => w.RouteCardInvoiceHeader1.Status == 1 &&
                w.RouteCardInvoiceHeader1.DocDate >= fromDate
                   && w.RouteCardInvoiceHeader1.DocDate <= ToDate).GroupBy(e =>
                                    new
                                    {
                                        e.TblSalesOrder,
                                        e.TblSalesOrder1.TblStyle,
                                        e.TblColor,
                                        e.Cost
                                    }).Select(w => new { RoutGroupID = w.Key.TblStyle, TblSalesOrder = w.Key.TblSalesOrder, TblColor = w.Key.TblColor, TotalCost = w.Sum(e => e.Qty * e.Cost), cost = w.Key.Cost }).ToList();
                foreach (var item in Tempsalesorders)
                {
                    var Cost = item.TotalCost;
                    var RouteCardDetailActualCostRow = new RouteCardDetailActualCost()
                    {
                        TblCostCenter = DefaultCostCenter.Iserial,
                        TblSalesOrder = item.TblSalesOrder,
                        TblColor = item.TblColor,
                        ActualOperationCost = Cost,
                        TransDate = ToDate,
                        RouteCardDetailActualCostType = 3
                    };
                    WorkFlowcontext.RouteCardDetailActualCosts.AddObject(RouteCardDetailActualCostRow);
                }
                var TotalCost = Convert.ToDecimal(Tempsalesorders.Sum(w => w.TotalCost));
                var totalRecord = new TblLedgerMainDetail()
                {
                    Amount = TotalCost,
                    ExchangeRate = 1,
                    Description = "",
                    DrOrCr = true,
                    GlAccount = UnderProduction,
                    TransDate = ToDate,
                    TblJournalAccountType = 0,
                    EntityAccount = UnderProduction,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                };
                UpdateOrInsertTblLedgerMainDetails(entity, totalRecord, true, 0, out temp, user);

                var testRecord = new TblLedgerMainDetail()
                {
                    Amount = TotalCost,
                    ExchangeRate = 1,
                    Description = "",
                    DrOrCr = false,
                    GlAccount = Production,
                    TransDate = ToDate,
                    TblJournalAccountType = 0,
                    EntityAccount = Production,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                };
                UpdateOrInsertTblLedgerMainDetails(entity, testRecord, true, 0, out temp, user);
                WorkFlowcontext.SaveChanges();
            }
        }

        private void OtherExpenses(int user, DateTime fromDate, DateTime ToDate, int journalint, ccnewEntities entity, string company, TblCostCenter DefaultCostCenter)
        {

            var CostCenters = new List<int>();
            CostCenters.Add(1076);
            CostCenters.Add(1077);
            using (var WorkFlowcontext = new WorkFlowManagerDBEntities())
            {
                WorkFlowcontext.CommandTimeout = 0;
                var TotalCost = entity.TblLedgerDetailCostCenters.Include("TblLedgerMainDetail1.TblLedgerHeader1").Where(w =>
                CostCenters.Contains(w.TblCostCenter) &&
                w.TblLedgerMainDetail1.TblLedgerHeader1.TblTransactionType != 14 && w.TblLedgerMainDetail1.TblLedgerHeader1.balanced == true &&
                w.TblLedgerMainDetail1.TblLedgerHeader1.DocDate >= fromDate
                   && w.TblLedgerMainDetail1.TblLedgerHeader1.DocDate <= ToDate).Sum(w => w.Amount * w.TblLedgerMainDetail1.ExchangeRate);


                var Tempsalesorders = WorkFlowcontext.RouteCardDetailActualCosts.Where(w => w.TransDate >= fromDate && w.TransDate <= ToDate).
                    GroupBy(w => new { w.TblSalesOrder, w.TblColor })
                    .ToList();
                var SumTotal = 0.0;
                foreach (var item in Tempsalesorders)
                {
                    SumTotal += item.Sum(w => w.ActualOperationCost) ?? 0;
                }

                foreach (var item in Tempsalesorders)
                {
                    var unitCost = item.Sum(w => w.ActualOperationCost);

                    //  var unitCost = item.Cost;
                    var ActualOprCost = ((float)TotalCost / (float)(SumTotal)) * unitCost;
                    var RouteCardDetailActualCostRow = new RouteCardDetailActualCost()
                    {
                        TblCostCenter = DefaultCostCenter.Iserial,
                        TblSalesOrder = item.Key.TblSalesOrder,
                        TblColor = item.Key.TblColor,
                        ActualOperationCost = ActualOprCost,
                        TransDate = ToDate,
                        RouteCardDetailActualCostType = 4
                    };
                    WorkFlowcontext.RouteCardDetailActualCosts.AddObject(RouteCardDetailActualCostRow);
                }


                WorkFlowcontext.SaveChanges();
            }
        }
        private void UpdateInventTrans(List<int> styleItemDims, decimal itemCost, WorkFlowManagerDBEntities workFlowcontext)
        {
            foreach (var item in workFlowcontext.TblInventTrans.Where(w => styleItemDims.Contains(w.TblItemDim)).ToList())
            {
                item.ItemCost = itemCost;
                item.LastStoreAvgCost = itemCost;
            }
        }
        private void GroupLedgerMainDetail(List<TblLedgerMainDetail> listOfLedgerMainDetails, string company, int TblLedgerHeaderIserial, int user)
        {
            int temp = 0;
            foreach (var item in listOfLedgerMainDetails.GroupBy(w => new { w.EntityAccount, w.GlAccount, w.TblJournalAccountType, w.DrOrCr, w.ExchangeRate, w.TransDate }))
            {
                var NewRow = new TblLedgerMainDetail()
                {
                    Amount = item.Sum(r => r.Amount),
                    ExchangeRate = item.Key.ExchangeRate,
                    Description = "",
                    DrOrCr = item.Key.DrOrCr,
                    GlAccount = item.Key.GlAccount,
                    TransDate = item.Key.TransDate,
                    TblJournalAccountType = item.Key.TblJournalAccountType,
                    EntityAccount = item.Key.EntityAccount,
                    TblCurrency = DefaultCurrency(company),
                    TblLedgerHeader = TblLedgerHeaderIserial,
                };
                UpdateOrInsertTblLedgerMainDetails(NewRow, true, 000, out temp, company,
                                          user);
            }
        }
    }
}