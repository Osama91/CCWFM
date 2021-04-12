using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using LinqKit;
using System.Transactions;
using System.Data.Entity;

namespace CCWFM.Web.Service.ContractOp
{
    [SilverlightFaultBehavior, ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class ContractService
    {
        [OperationContract]
        private List<TblContractHeader> GetContractHeader(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, int userIserial, out int fullCount, string company)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.CommandTimeout = 0;
                
                var defaultQuery = context.TblContractHeaders.Include(
                    nameof(TblContractHeader.TblLkpSeason1)).Include(
                    nameof(TblContractHeader.TblLkpBrandSection1)).Include(
                    nameof(TblContractHeader.TblSubContractor1));
                IQueryable<TblContractHeader> query;
                if (valuesObjects == null) valuesObjects = new Dictionary<string, object>();
                var styleCodeFilter = valuesObjects.Where(o => o.Key.StartsWith("TblContractDetails_TblSalesOrderColor1_TblSalesOrder1_TblStyle1_StyleCode"));
                string styleCode = string.Empty;
                if (styleCodeFilter.Count() > 0)
                {
                    styleCode = styleCodeFilter.ElementAt(0).Value.ToString().Replace("%", "");
                    styleCodeFilter.ToArray().ForEach(f =>
                    {
                        valuesObjects.Remove(f.Key);
                        filter = filter.Replace(
                       string.Format("it.TblContractDetails.TblSalesOrderColor1.TblSalesOrder1.TblStyle1.StyleCode LIKE(@{0})",
                       f.Key), "");
                    });
                    filter = filter.Trim();
                    if (filter.ToLower().StartsWith("and")) filter = filter.Remove(0, 3);
                    if (filter.ToLower().EndsWith("and")) filter = filter.Remove(filter.Length - 3 - 1, 3);
                }
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = SharedOperation.GetBrandPerUser(filter, valuesObjects, userIserial, context)
                        .Replace("it.Brand", "it.BrandCode").Replace("it.BrandCodeCode", "it.BrandCode");
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    //var parameterCollection = valuesObjects.Select(valuesObject => new System.Data.Objects.ObjectParameter(valuesObject.Key, valuesObject.Value)).ToList();
                    fullCount = defaultQuery.Where(filter, parameterCollection.ToArray()).Count();
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort);
                }
                else
                {
                    fullCount = defaultQuery.Count(x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.BrandCode));
                    query = defaultQuery.OrderBy(sort).Where(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.BrandCode));
                }

                List<TblContractHeader> result;
                if (!string.IsNullOrWhiteSpace(styleCode))
                {
                   

                    query = query.Where(r =>
                                         r.TblContractDetails.Any(d => d.TblSalesOrderColor1.TblSalesOrder1.TblStyle1.StyleCode.Contains(styleCode)));
                    fullCount = query.Count();
                    try
                    {

                    //    var sql = ((System.Data.Objects.ObjectQuery)query).ToTraceString();

                        //result = query.AsNoTracking().ToList();
                        result = query.ToList();

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                 
                }
                else
                {
                    result = query.Skip(skip).Take(take).ToList();
                }
                var db = new ccnewEntities(SharedOperation.GetSqlConnectionString(company));
                foreach (var item in result)
                {
                    item.Supplier = db.TBLsuppliers.FirstOrDefault(s => s.Iserial == item.SupplierIserial);
                    item.Currency = db.TblCurrencyTests.FirstOrDefault(c => c.Iserial == item.TblCurrency);
                }
                return result;
            }
        }

        [OperationContract]
        private TblContractHeader UpdateOrInsertContractHeader(TblContractHeader newRow, int index, int userIserial, out int outindex,string company)
        {
            throw new Exception("Save Contracts On Stitch");

            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    newRow.TblRetailOrderProductionType1 = null;
                    newRow.TblSubContractor1 = null;
                    newRow.TblLkpSeason1 = null;
                    newRow.TblLkpBrandSection1 = null;
                    var brandCode = newRow.BrandCode;
                    var report = context.TblBrandContractReports.FirstOrDefault(bcr => bcr.BrandCode == brandCode);
                    if (report != null)
                        newRow.ContractReport = report.ReportName;
                    var oldRow = context.TblContractHeaders.FirstOrDefault(th => th.Iserial == newRow.Iserial);
                    newRow.Amount = newRow.TblContractDetails.Sum(r => r.Total);

                    

                    var budgetAmount = CalculateBudget(newRow, context);

                    var contractsAmount = context.TblContractDetails
                        .Include(nameof(TblContractDetail.TblContractHeader1)).Where(r =>
                        r.TblContractHeader1.BrandCode == newRow.BrandCode && r.TblLkpBrandSection == newRow.TblLkpBrandSection &&
                        r.TblContractHeader1.TblLkpSeason == newRow.TblLkpSeason).ToList()
                    .Select(r => new Tuple<int?, decimal>(r.TblLkpBrandSection,
                     r.TblContractHeader1.TblRetailOrderProductionType == 1 ?
                              (r.Qty * r.Cost) : r.TblContractHeader1.TblRetailOrderProductionType == 2 ?
                              (r.Qty * (r.Cost - r.AccCost)) : r.TblContractHeader1.TblRetailOrderProductionType == 3 ?
                              (r.Qty * (r.Cost - r.AccCost - r.FabricCost)) : (r.Qty * r.Cost)
                    )).GroupBy(r => r.Item1).Select(r => new Tuple<int?, decimal>(r.Key, r.Sum(d => d.Item2))).ToList();
                    
                    var tblContractDetail = new List<TblContractDetail>();
                    newRow.TblContractDetails.ToArray().ForEach(d => tblContractDetail.Add(d));
                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        CheckBudget(newRow, oldRow, budgetAmount, contractsAmount);
                        
                        if (!oldRow.Approved && newRow.Approved)// كده لسه معموله ابروف
                        {
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        foreach (var item in newRow.TblContractDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp;
                            //headeriserial = item.TblContractHeader;
                            item.TblContractHeader1 = null;
                            item.TblContractHeader = newRow.Iserial;
                            UpdateOrInsertContractDetail(item, userIserial, 1, out temp);
                            item.TblContractHeader1Reference = null;
                        }
                        foreach (var item in newRow.TblContractPaymentByPeriods.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp;
                            item.TblContractHeader1 = null;
                            item.TblContractHeader = newRow.Iserial;
                            UpdateOrInsertContractPaymentDetail(item, userIserial, 1, out temp);
                            item.TblContractHeader1Reference = null;
                        }
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                        if (result.Count() > 0)
                        {
                            newRow.LastChangeUser = userIserial;
                            newRow.LastChangeDate = DateTime.Now;
                        }
                    }
                    else// الهيدر ده جديد
                    {
                        CheckBudget(newRow, null, budgetAmount, contractsAmount);
                        
                        var seqTo = 443;//context.tblChainSetups.FirstOrDefault(s => s.sGlobalSettingCode == "ContractApproveEmailTo");
                        var seqRow = context.TblSequenceProductions.FirstOrDefault(x => x.Iserial == seqTo);
                        newRow.Code = SharedOperation.HandelSequence(seqRow);
                        if (newRow.Approved)// كده معموله ابروف
                        {
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        newRow.TblContractDetails.ForEach(cd => {
                            cd.TblColor1 = null; cd.TblSalesOrderColor1 = null;
                            cd.CreatedBy = userIserial; cd.CreationDate = DateTime.Now;
                        });
                        newRow.TblContractPaymentByPeriods.ForEach(cd => {
                            cd.CreatedBy = userIserial; cd.CreationDate = DateTime.Now;
                        });

                        if (newRow.TblContractDetails.FirstOrDefault() != null)
                            newRow.TblLkpBrandSection = newRow.TblContractDetails.FirstOrDefault().TblLkpBrandSection;
                       
                        //   newRow.TblLkpBrandSection = null;
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastChangeDate = DateTime.Now;
                        newRow.LastChangeUser = userIserial;

                        context.TblContractHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex) { throw ex; }
                TblContractHeader rowTemp = newRow.Clone();
                try
                {
                    if (newRow.Approved)
                        try
                        {
                            var emailTo = context.tblChainSetups.FirstOrDefault(s => s.sGlobalSettingCode == "ContractApproveEmailTo");
                            var emailFrom = context.tblChainSetups.FirstOrDefault(s => s.sGlobalSettingCode == "ContractApproveEmailFrom");
                            var emailBody = context.tblChainSetups.FirstOrDefault(s => s.sGlobalSettingCode == "ContractApproveEmailBody");
                            var emailSubject = context.tblChainSetups.FirstOrDefault(s => s.sGlobalSettingCode == "ContractApproveEmailSubject");
                            if (emailTo != null && emailFrom != null && emailBody != null &&
                                emailSubject != null &&
                                //!emailTo.sSetupValue.Split(';').Any(e => !SharedOperation.IsValidEmail(e)) &&
                                SharedOperation.IsValidEmail(emailFrom.sSetupValue))
                            {
                                string storeMail = emailTo.sSetupValue;
                                SharedOperation.SendEmail(null, emailFrom.sSetupValue,
                                    emailTo.sSetupValue.Split(';').ToList(), string.Format(emailSubject.sSetupValue,
                                   newRow.Code), string.Format(emailBody.sSetupValue, newRow.Code));
                            }
                        }
                        catch (Exception ex) { }
                    foreach (var item in GetContractDetail(0, int.MaxValue, newRow.Iserial))
                    {
                        var tblContractDetail = item.Clone();
                        tblContractDetail.TblLkpBrandSection1 = item.TblLkpBrandSection1;
                        tblContractDetail.TblSalesOrderColor1 = item.TblSalesOrderColor1;
                        rowTemp.TblContractDetails.Add(tblContractDetail);
                    }
                    foreach (var item in GetContractPaymentDetail(0, int.MaxValue, newRow.Iserial))
                    {
                        var tblContractPaymentByPeriod = item.Clone();
                        rowTemp.TblContractPaymentByPeriods.Add(tblContractPaymentByPeriod);
                    }
                    using (var db = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
                    {
                        rowTemp.Supplier = db.TBLsuppliers.FirstOrDefault(s => s.Iserial == newRow.SupplierIserial);
                        rowTemp.Currency = db.TblCurrencyTests.FirstOrDefault(c => c.Iserial == rowTemp.TblCurrency);
                    }
                }
                catch { }
                return rowTemp;
            }
        }

        private static void CheckBudget(TblContractHeader newRow, TblContractHeader oldRow, List<Tuple<int, int>> budgetAmount,
            List<Tuple<int?, decimal>> contractsAmount)
        {
            foreach (var item in budgetAmount)
            {
                var contractsTotal = contractsAmount.Where(c => c.Item1 == item.Item1).Sum(c => c.Item2);
                var newRowTotal = newRow.TblContractDetails.Where(c => c.TblLkpBrandSection == item.Item1).Sum(c => c.Total);
                decimal oldRowTotal = 0;
                if (oldRow != null)
                    oldRowTotal = oldRow.TblContractDetails.Where(c => c.TblLkpBrandSection == item.Item1).Sum(c => c.Total);

                if (oldRow != null)
                    if (item.Item2 < (contractsTotal + newRowTotal - oldRowTotal))
                        throw new InvalidOperationException(string.Format(
                                @"Amount must be within brand section budget brand section budget : {0} used : {1} required : {2}",
        item.Item2.ToString("0.#"), contractsTotal.ToString("0.#"), (newRowTotal - oldRowTotal).ToString("0.#")));
                    else
                                  if (item.Item2 < (contractsTotal + newRowTotal))
                        throw new InvalidOperationException(string.Format(
        @"Amount must be within brand section budget brand section budget : {0} used : {1} required : {2}",
    item.Item2.ToString("0.#"), contractsTotal.ToString("0.#"), newRowTotal.ToString("0.#")));
            }           
        }

        private static List<Tuple<int, int>> CalculateBudget(TblContractHeader newRow, WorkFlowManagerDBEntities context)
        {
            List<Tuple<int, int>> budgetAmount = new List<Tuple<int, int>>();
            var newSeason = context.TblLkpSeasons.FirstOrDefault(s => s.Iserial == newRow.TblLkpSeason);
            var budgetSeason = context.TblLkpSeasons.FirstOrDefault(s =>
                s.ShortCode == newSeason.ShortCode && s.IsMaster == true);
            var sections = newRow.TblContractDetails.Select(d => d.TblLkpBrandSection ?? 0).Distinct();
            budgetAmount = context.TblPurchaseBudgetDetails.Where(r =>
                       r.Brand == newRow.BrandCode && r.TblLkpSeason == budgetSeason.Iserial &&
                      sections.Any(d => d == r.TblLkpBrandSection)).ToList()
                       .Select(r => new Tuple<int, int>(r.TblLkpBrandSection, r.Amount)).ToList();
            //if (budget != null)
            //    budgetAmount = budget.Amount;
            return budgetAmount;
        }

        [OperationContract]
        private int DeleteContractHeader(TblContractHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var oldRow = (from e in context.TblContractHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
        
        [OperationContract]
        private List<TblContractDetail> GetContractDetail(int skip, int take, int headerId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblContractDetails.Include(
                    nameof(TblContractDetail.TblContractHeader1)).Include(
                    nameof(TblContractDetail.TblColor1)).Include(
                    nameof(TblContractDetail.TblLkpBrandSection1)).Include(
                    string.Format("{0}.{1}", nameof(TblContractDetail.TblSalesOrderColor1),
                    nameof(TblSalesOrderColor.TblSalesOrder1))).Include(
                    string.Format("{0}.{1}.{2}", nameof(TblContractDetail.TblSalesOrderColor1),
                    nameof(TblSalesOrderColor.TblSalesOrder1), nameof(TblSalesOrder.TblStyle1)))
                  .Where(v =>
                    v.TblContractHeader == headerId).OrderBy(x =>
                    x.Iserial);//.Skip(skip).Take(take);
                var result = query.ToList();
                return result;
            }
        }
       
        //[OperationContract]
        private TblContractDetail UpdateOrInsertContractDetail(
            TblContractDetail newRow, int userIserial, int index, out int outindex)
        {
            throw new Exception("Save Contracts On Stitch");

            outindex = index;
            newRow.TblContractHeader1 = null;
            newRow.TblSalesOrderColor1 = null;
            newRow.TblColor1 = null;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblContractDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                    if (result.Count() > 0)
                    {
                        newRow.LastChangeUser = userIserial;
                        newRow.LastChangeDate = DateTime.Now;
                    }
                }
                else
                {
                    newRow.CreatedBy = userIserial;
                    newRow.LastChangeUser = userIserial;
                    newRow.CreationDate = DateTime.Now;
                    newRow.LastChangeDate = DateTime.Now;
                    context.TblContractDetails.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        private TblContractPaymentByPeriod UpdateOrInsertContractPaymentDetail(
                  TblContractPaymentByPeriod newRow,int userIserial, int index, out int outindex)
        {
            outindex = index;
            newRow.TblContractHeader1 = null;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblContractPaymentByPeriods
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                    if (result.Count() > 0)
                    {
                        oldRow.LastChangeDate = DateTime.Now;
                        oldRow.LastChangeUser = userIserial;
                    }
                }
                else
                {
                    newRow.CreatedBy = userIserial;
                    newRow.LastChangeUser = userIserial;
                    newRow.CreationDate = DateTime.Now;
                    newRow.LastChangeDate = DateTime.Now;
                    context.TblContractPaymentByPeriods.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblContractDetail DeleteContractDetail(TblContractDetail row, int userIserial)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(600)))
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var oldRow = (from e in context.TblContractDetails
                                  where e.Iserial == row.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        var headerId = oldRow.TblContractHeader;
                        context.TblContractDetailDels.AddObject(new TblContractDetailDel()
                        {
                            TblLkpBrandSection = oldRow.TblLkpBrandSection,
                            AccCost = oldRow.AccCost,
                            Cost = oldRow.Cost,
                            CreatedBy = userIserial,
                            CreationDate = DateTime.Now,
                            DeliveryDate = oldRow.DeliveryDate,
                            FabricCost = oldRow.FabricCost,
                            Material = oldRow.Material,
                            OperationCost = oldRow.OperationCost,
                            Qty = oldRow.Qty,
                            TblColor = oldRow.TblColor,
                            TblContractHeader = oldRow.TblContractHeader,
                            TblSalesOrderColor = oldRow.TblSalesOrderColor,
                            LastChangeDate=oldRow.LastChangeDate,
                            LastChangeUser=oldRow.LastChangeUser,
                        });
                        context.DeleteObject(oldRow);
                        var header = context.TblContractHeaders.FirstOrDefault(h => h.Iserial == headerId);
                        header.Amount = header.TblContractDetails.Sum(r => r.Total);
                    }
                    context.SaveChanges();
                }
                scope.Complete();
            }          
            return row;
        }
        
        [OperationContract]
        private List<TblContractPaymentByPeriod> GetContractPaymentDetail(int skip, int take, int headerId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblContractPaymentByPeriods.Include(
                    nameof(TblContractPaymentByPeriod.TblContractHeader1))
                  .Where(v =>
                    v.TblContractHeader == headerId).OrderBy(x =>
                    x.Iserial);//.Skip(skip).Take(take);
                var result = query.ToList();
                return result;
            }
        }
       
        [OperationContract]
        private TblContractPaymentByPeriod DeleteContractPaymentDetail(TblContractPaymentByPeriod row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblContractPaymentByPeriods
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }


        [OperationContract]
        private List<TblSalesOrderColor> GetStyles(int SupplierIserial, string BrandCode, List<int> BrandSection,
           List<int> Season,List<int> currencies, int RetailOrderProductionType, DateTime? from, DateTime? to)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //var contractHeader = context.TblContractHeaders.FirstOrDefault(ch => ch.Iserial == ContractIserial);
                var defaultQuery = context.TblSalesOrderColors.Include(nameof(TblSalesOrderColor.TblColor1)).Include(
                    nameof(TblSalesOrderColor.TblSalesOrder1)).Include(
                    string.Format("{0}.{1}", nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1))).Include(
                    string.Format("{0}.{1}.{2}", nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1),nameof(TblStyle.TblLkpBrandSection1)));

               

                var predicate = PredicateBuilder.True<TblSalesOrderColor>();

                if (from != null)
                    predicate = predicate.And(i => i.TblSalesOrder1.DeliveryDate >= from);
                if (to != null)
                    predicate = predicate.And(i => i.TblSalesOrder1.DeliveryDate <= to);
              

                #region CheckTNA BeforeContract
                /*
                try
                {
                    var _TNAContractStyles = defaultQuery.AsExpandable().Where(predicate)
                  .Where(v =>
                  v.TblSalesOrder1.TblStyle1.Canceled != true &&
                 (v.TblSalesOrder1.Status == 1 || v.TblSalesOrder1.Status == 5) &&
                  v.TblSalesOrder1.SalesOrderType == 1 &&
                  v.TblSalesOrder1.TblSupplier == SupplierIserial &&
                  v.TblSalesOrder1.TblStyle1.Brand == BrandCode &&
                  BrandSection.Any(bs => v.TblSalesOrder1.TblStyle1.TblLkpBrandSection == bs) &&
                  Season.Any(s => s == v.TblSalesOrder1.TblStyle1.TblLkpSeason) &&
                  currencies.Any(c => c == v.TblCurrency)
                  && !context.TblContractDetails.Any(cd =>
                    cd.TblSalesOrderColor == v.Iserial)
                  ).ToList().GroupBy(p => p.TblSalesOrder1.TblStyle1.Iserial).Select(g => g.First());

                    foreach (var item in _TNAContractStyles)
                    {
                        var SalesOrders = context.TblSalesOrders.Where(r =>  r.TblStyle == item.TblSalesOrder1.TblStyle &&
                                                                             r.TblStyle1.Canceled != true &&
                                                                             (r.Status == 1 || r.Status == 5) &&
                                                                              r.SalesOrderType == 1);
                        var linkedTNA = SalesOrders.Where(r => r.TblStyleTNAHeader != null && r.TblStyleTNAHeader != 0).FirstOrDefault();

                        if (linkedTNA != null)
                        {
                            foreach (var SalesOrdersItem in SalesOrders)
                            {
                                if (
                                    SalesOrdersItem.TblSupplier == linkedTNA.TblSupplier
                                    && SalesOrdersItem.DeliveryDate == linkedTNA.DeliveryDate
                                    //&& 
                                   )
                                {
                                    SalesOrdersItem.TblStyleTNAHeader = linkedTNA.TblStyleTNAHeader;
                                    SalesOrdersItem.TblRetailOrderProductionType = 1;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)  {
                    string cc = ex.Message;
                }
               */
                #endregion

                var query = defaultQuery.AsExpandable().Where(predicate)
                    .Where(v =>
                    v.TblSalesOrder1.TblStyle1.Canceled != true &&
                   (v.TblSalesOrder1.Status == 1 || v.TblSalesOrder1.Status == 5) &&
                    v.TblSalesOrder1.SalesOrderType == 1 &&
                    v.TblSalesOrder1.TblSupplier == SupplierIserial &&
                    v.TblSalesOrder1.TblStyle1.Brand == BrandCode &&
                    BrandSection.Any(bs => v.TblSalesOrder1.TblStyle1.TblLkpBrandSection == bs) &&
                    Season.Any(s => s == v.TblSalesOrder1.TblStyle1.TblLkpSeason) &&
                    currencies.Any(c => c == v.TblCurrency) &&
                    v.TblSalesOrder1.TblRetailOrderProductionType == RetailOrderProductionType && !context.TblContractDetails.Any(cd =>
                    cd.TblSalesOrderColor == v.Iserial)).OrderBy(r =>r.TblSalesOrder1.TblStyle1.Iserial);
                var result = query.ToList();
                return result;
            }
        }


        [OperationContract]
        private List<TblSalesOrderColor> GetSinleStyle(int SupplierIserial, string BrandCode, List<int> BrandSection,
         List<int> Season, List<int> currencies, int RetailOrderProductionType, DateTime? from, DateTime? to,string StyleCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //var contractHeader = context.TblContractHeaders.FirstOrDefault(ch => ch.Iserial == ContractIserial);
                var defaultQuery = context.TblSalesOrderColors.Include(nameof(TblSalesOrderColor.TblColor1)).Include(
                    nameof(TblSalesOrderColor.TblSalesOrder1)).Include(
                    string.Format("{0}.{1}", nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1))).Include(
                    string.Format("{0}.{1}.{2}", nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1), nameof(TblStyle.TblLkpBrandSection1)));
                var predicate = PredicateBuilder.True<TblSalesOrderColor>();

                if (from != null)
                    predicate = predicate.And(i => i.TblSalesOrder1.DeliveryDate >= from);
                if (to != null)
                    predicate = predicate.And(i => i.TblSalesOrder1.DeliveryDate <= to);

                var query = defaultQuery.AsExpandable().Where(predicate).Where(v =>
                v.TblSalesOrder1.TblStyle1.Canceled != true &&

                    (v.TblSalesOrder1.Status == 1 || v.TblSalesOrder1.Status == 5) &&
                    v.TblSalesOrder1.SalesOrderType == 1 &&
                   // v.TblSalesOrder1.TblSupplier == SupplierIserial &&
                    v.TblSalesOrder1.TblStyle1.Brand == BrandCode &&
                    BrandSection.Any(bs => v.TblSalesOrder1.TblStyle1.TblLkpBrandSection == bs) &&
                    Season.Any(s => s == v.TblSalesOrder1.TblStyle1.TblLkpSeason) &&
                    currencies.Any(c => c == v.TblCurrency) &&
                    v.TblSalesOrder1.TblRetailOrderProductionType == RetailOrderProductionType
                    && v.TblSalesOrder1.TblStyle1.StyleCode == StyleCode);
                var result = query.ToList();
                return result;
            }
        }


        [OperationContract]
        private List<TblBrandContractReport> GetBrandContractReports()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblBrandContractReports;
                var result = query.ToList();
                return result;
            }
        }

        [OperationContract]
        private List<TblCurrencyTest> GetLookUpCurrency(string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblCurrencyTest> query;
                query = context.TblCurrencyTests;

                return query.ToList();
            }
        }
    }
}