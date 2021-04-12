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

namespace CCWFM.Web.Service.RouteCard
{
    public partial class RouteCardService
    {
        [DataContract]
        public class RouteCardInvoiceDetailDto
        {
            [DataMember]
            public string ColorCode { get; set; }

            [DataMember]
            public string SalesOrderCode { get; set; }

            [DataMember]
            public int TransID { get; set; }

            [DataMember]
            public int TblSalesOrder { get; set; }

            [DataMember]
            public int TblColor { get; set; }

            [DataMember]
            public int RouteCardInvoiceHeader { get; set; }

            [DataMember]
            public int RouteCardHeader { get; set; }

            [DataMember]
            public string RouteGroupString { get; set; }

            [DataMember]
            public string SupplierInv { get; set; }
         

            [DataMember]
            public int? Qty { get; set; }
            [DataMember]
            public double? Cost { get; set; }
        };

        [OperationContract]
        private List<RouteCardInvoiceHeader> GetRouteCardInvoiceHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<Vendor> Vendors)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<RouteCardInvoiceHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.RouteCardInvoiceHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.RouteCardInvoiceHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.RouteCardInvoiceHeaders.Count();
                    query = context.RouteCardInvoiceHeaders.OrderBy(sort).Skip(skip).Take(take);
                }


                var codes = query.Select(w => w.Vendor).ToList();
                Vendors = context.Vendors.Where(x => codes.Contains(x.vendor_code)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        public List<RouteCardInvoiceDetailDto> SearchRouteCardInvoice(string vendor, DateTime? fromDate, DateTime? toDate, int transId, int salesOrder,string supplierInv)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (string.IsNullOrWhiteSpace(supplierInv))
                {
                    supplierInv = null;
                }
                var predicate = PredicateBuilder.True<RouteCardDetail>();

                if (!string.IsNullOrEmpty(supplierInv))
                    predicate = predicate.And(i => i.RouteCardHeader.SupplierInv == supplierInv);

                if (!string.IsNullOrEmpty(vendor))
                    predicate = predicate.And(i => i.RouteCardHeader.Vendor == vendor);

                if (transId!=0)
                    predicate = predicate.And(i => i.RouteCardHeader.TransID == transId);
                if (salesOrder != 0)
                    predicate = predicate.And(i => i.TblSalesOrder == salesOrder);

                predicate = predicate.And(w =>
                          w.RouteCardHeader.DocDate >= fromDate && w.RouteCardHeader.DocDate <= toDate
                          && w.RouteCardHeader.Direction == 1 && w.RouteCardHeader.RouteType == 5 &&
                          !context.RouteCardInvoiceDetails.Any(e => e.RouteCardHeader == w.RouteCardHeaderIserial) &&
                          w.SizeQuantity > 0 && w.RouteCardHeader.RouteIncluded == true); 

                var query = (from p in context.RouteCardDetails.AsExpandable().Where(predicate)
                    .GroupBy(e =>
                            new
                            {
                                e.RouteCardHeaderIserial,
                                e.TblRouteGroup.Ename,
                                e.RouteCardHeader.TransID,
                                e.TblSalesOrder,
                                e.TblColor,
                                e.RouteCardHeader.SupplierInv
                            })
                             select new RouteCardInvoiceDetailDto
                             {
                                 Qty = p.Sum(v => v.SizeQuantity),
                                 RouteCardHeader = p.Key.RouteCardHeaderIserial,
                                 RouteCardInvoiceHeader = 0,
                                 TblColor = p.Key.TblColor,
                                 ColorCode = p.FirstOrDefault().TblColor1.Code,
                                 TblSalesOrder = (int)p.Key.TblSalesOrder,
                                 SalesOrderCode = p.FirstOrDefault().TblSalesOrder1.SalesOrderCode,
                                 TransID = p.Key.TransID,
                                 RouteGroupString= p.Key.Ename,
                                 SupplierInv= p.Key.SupplierInv,
                                 Cost = context.TblSalesOrderOperations.FirstOrDefault(y => y.TblOperation == p.FirstOrDefault().RoutGroupID && y.TblSalesOrder == p.Key.TblSalesOrder).OprCost
                             }).ToList();




                return query;
            }
        }

        [OperationContract]
        private RouteCardInvoiceHeader UpdateOrInsertRouteCardInvoiceHeader(RouteCardInvoiceHeader newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var seqCode = SharedOperation.GetChainSetup("GlRouteInvoiceSequence");
                    var seqProd = context.TblSequenceProductions.FirstOrDefault(w => w.Code == seqCode);
                    newRow.Code = SharedOperation.HandelSequence(seqProd);
                    context.RouteCardInvoiceHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.RouteCardInvoiceHeaders
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
        private int DeleteRouteCardInvoiceHeader(RouteCardInvoiceHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.RouteCardInvoiceHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<RouteCardInvoiceDetail> GetRouteCardInvoiceDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<RouteCardInvoiceDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.RouteCardInvoiceHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.RouteCardInvoiceDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.RouteCardInvoiceDetails.Include("RouteCardHeader1.TblRouteGroup").Include("TblColor1").Include("TblSalesOrder1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.RouteCardInvoiceDetails.Count(v => v.RouteCardInvoiceHeader == groupId);
                    query = context.RouteCardInvoiceDetails.Include("RouteCardHeader1.TblRouteGroup").Include("TblColor1").Include("TblSalesOrder1").OrderBy(sort).Where(v => v.RouteCardInvoiceHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private RouteCardInvoiceDetail UpdateOrInsertRouteCardInvoiceDetail(RouteCardInvoiceDetail newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.RouteCardInvoiceDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.RouteCardInvoiceDetails
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
        private int DeleteRouteCardInvoiceDetail(RouteCardInvoiceDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.RouteCardInvoiceDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }


        [OperationContract]
        private RouteCardInvoiceHeader PostRouteCardInvoice(RouteCardInvoiceHeader row, int user, string company)
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
                    var query = entity.RouteCardInvoiceHeaders.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Route TransNo " + row.Code;
                    var markuptrans =
                        entity.RouteCardInvoiceMarkupTransProds.Include("TblMarkupProd1.TblMarkupGroupProd1")
                            .Where(x => x.RouteCardInvoiceHeader == row.Iserial && x.Type == 0);

                    var cost =
                        entity.RouteCardInvoiceDetails.Where(x => x.RouteCardInvoiceHeader == row.Iserial).Sum(w => w.Cost * w.Qty);

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
                                entity.RouteCardInvoiceDetails.Where(x => x.RouteCardInvoiceHeader == row.Iserial).ToList();
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
                                var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GlRouteInvoice").sSetupValue;

                                int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderProdRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = desc,
                                    DocDate = row.DocDate,
                                    TblJournal = journalint,
                                    TblTransactionType = 9,
                                    TblJournalLink = query.Iserial
                                };
                                int temp;
                                var glserive = new GlService();
                                glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);
                                var sqlParam = new List<SqlParameter>{
                                new SqlParameter
                                {
                                    ParameterName = "Iserial",
                                    Value = row.Iserial.ToString(CultureInfo.InvariantCulture),
                                    SqlDbType = SqlDbType.NVarChar
                                },
                            };
                                var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlRouteCardInvoicePostingToGl  @Iserial",
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