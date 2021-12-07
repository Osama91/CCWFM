using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.Transactions;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations.GlOperations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private TblRecInvHeaderProd PostInv(TblRecInvHeaderProd row, int user, string company)
        {
            using (var scope = new TransactionScope())
            {
                short? Lang = 0;
                using (var entity = new WorkFlowManagerDBEntities())
                {
                    var firstOrDefault = entity.TblAuthUsers.FirstOrDefault(x => x.Iserial == user);
                    if (firstOrDefault != null)
                        Lang = firstOrDefault.CurrLang;
                    entity.CommandTimeout = 0;
                    var query = entity.TblRecInvHeaderProds.FirstOrDefault(x => x.Iserial == row.Iserial);

                    string desc = "Purchase TransNo " + row.SupplierInv;

                    if (row.TblRecInvHeaderTypeProd == 2)
                    {
                        desc = "Return Purchase TransNo " + row.SupplierInv;
                    }
                    if (Lang == 0)
                    {
                        desc = "فاتورة المشتريات رقم " + row.SupplierInv;
                        if (row.TblRecInvHeaderTypeProd == 2)
                        {
                            desc = " فاتورة مرتجع المشتريات رقم " + row.SupplierInv;
                        }
                    }
                    var markuptrans =
                        entity.TblMarkupTransProds.Include("TblMarkupProd1.TblMarkupGroupProd1")
                            .Where(x => x.TblRecInv == row.Iserial && x.Type == 0);

                    var cost = entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == row.Iserial).Sum(w => w.Cost * w.Qty);

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
                                entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == row.Iserial).ToList();
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
                            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
                            {
                                var journal = db.tblChainSetupTests.FirstOrDefault(x => x.sGlobalSettingCode == "GLPurchaseJournal").sSetupValue;
                                var tablename = db.tblChainSetupTests.FirstOrDefault(
                                        x => x.sGlobalSettingCode == "GlItemGroupTableName").sSetupValue;

                                int journalint = db.TblJournals.FirstOrDefault(x => x.Code == journal).Iserial;

                                var newLedgerHeaderProdRow = new TblLedgerHeader
                                {
                                    CreatedBy = user,
                                    CreationDate = DateTime.Now,
                                    Description = desc,
                                    DocDate = row.TransDate,
                                    TblJournal = journalint,
                                    TblTransactionType = 100,
                                    TblJournalLink = query.Iserial
                                };
                                int temp;
                                var glserive = new GlService();
                                glserive.UpdateOrInsertTblLedgerHeaders(newLedgerHeaderProdRow, true, 0, out temp, user, company);

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
                                var list = entity.ExecuteStoreQuery<GlGroupsDtp>("exec GlRecinvPostingToGl @Table_Name, @Iserial",
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
                                    var markupdes = rr.TblMarkupProd1.Ename + row.SupplierInv;
                                    if (Lang == 0)
                                    {
                                        markupdes = rr.TblMarkupProd1.Aname + row.SupplierInv;
                                    }
                                    decimal totalModified = (decimal)total;
                                    if (total < 0)
                                    {
                                        totalModified = (decimal)(total * -1);
                                    }

                                    if (!rr.TblMarkupProd1.ItemEffect)
                                    {
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
                                            TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                            PaymentRef = query.SupplierInv,
                                            DrOrCr = !drorCr
                                        };

                                        if (row.TblRecInvHeaderTypeProd == 2)
                                        {
                                            markupVendorDiscount.DrOrCr = !markupVendorDiscount.DrOrCr;
                                        }
                                        glserive.UpdateOrInsertTblLedgerMainDetails(markupVendorDiscount, true, 000, out temp, company,
                                             user);
                                    }
                                    if (glAccount != 0)
                                    {

                                        var markupVendor = new TblLedgerMainDetail
                                        {
                                            Amount = totalModified,
                                            Description = markupdes,
                                            ExchangeRate = rr.ExchangeRate,
                                            TblCurrency = rr.TblCurrency,
                                            TransDate = row.TransDate,
                                            TblJournalAccountType = rr.TblJournalAccountType,
                                            EntityAccount = vendorAccountMarkUp.Iserial,
                                            GlAccount = vendorAccountMarkUp.AccountIserial,
                                            TblLedgerHeader = newLedgerHeaderProdRow.Iserial,
                                            PaymentRef = query.SupplierInv,
                                            DrOrCr = drorCr
                                        };
                                        if (row.TblRecInvHeaderTypeProd == 2)
                                        {
                                            markupVendor.DrOrCr = !markupVendor.DrOrCr;
                                        }
                                        glserive.UpdateOrInsertTblLedgerMainDetails(markupVendor, true, 000, out temp, company, user);

                                        foreach (var variable in list)
                                        {
                                            var costcenter = new TblGlRuleDetail();
                                            costcenter = glserive.FindCostCenterByType(costcenter, 0, (int)variable.GroupName,
                                                company);

                                            var markupVendorLedgerCostCenter = new TblLedgerDetailCostCenter
                                            {
                                                Ratio = 0,
                                                TblLedgerMainDetail = markupVendor.Iserial,
                                                Amount = (double)(markupVendor.Amount * variable.CostPercentage),
                                                TblCostCenter = costcenter.TblCostCenter,
                                                TblCostCenterType = costcenter.TblCostCenter1.TblCostCenterType,
                                            };
                                            glserive.UpdateOrInsertTblLedgerDetailCostCenters(markupVendorLedgerCostCenter, true, 000,
                                                 out temp, user,company);
                                        }
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

        [OperationContract]
        private List<TblRecInvHeaderProd> GetTblRecInvHeaderProd(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRecInvHeaderProd> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvHeaderProds.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblRecInvHeaderProds.Include("TblRecInvHeaderTypeProd1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblRecInvHeaderProds.Count();
                    query = entity.TblRecInvHeaderProds.Include("TblRecInvHeaderTypeProd1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblRecInvMainDetailProd> GetTblRecInvMainDetailProd(int skip, int take, int recInvHeaderProd, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out double TotalQty, out double TotalAmount, out List<ItemsDto> itemsList)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRecInvMainDetailProd> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeaderProd ==(@Group0)";
                    valuesObjects.Add("Group0", recInvHeaderProd);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvMainDetailProds.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblRecInvMainDetailProds.Include("TblColor1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblRecInvMainDetailProds.Count(v => v.TblRecInvHeaderProd == recInvHeaderProd);
                    query = entity.TblRecInvMainDetailProds.Include("TblColor1").OrderBy(sort).Where(v => v.TblRecInvHeaderProd == recInvHeaderProd).Skip(skip).Take(take);
                }
                try
                {
                    TotalQty = entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == recInvHeaderProd).Sum(x => x.Qty);

                }
                catch (Exception)
                {

                    TotalQty = 0;
                }

                try
                {
                    TotalAmount = entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == recInvHeaderProd).Sum(x => x.Qty * x.Cost);
                }
                catch (Exception)
                {

                    TotalAmount = 0;
                }


                var fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.TblItem);
                var accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.TblItem);
                var itemsquery = (from x in entity.Fabric_UnitID
                                  where (fabricsIserial.Any(l => x.Iserial == l))
                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.Fabric_Code,
                                      Name = x.Fabric_Ename,
                                      ItemGroup = x.Type,
                                      Unit = x.UnitID
                                  }).Take(20).ToList();
                itemsquery.AddRange((from x in entity.tbl_AccessoryAttributesHeader

                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = entity.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(20).ToList());
                itemsList = itemsquery;
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblRecInvDetailProd> GetTblRecInvDetailProd(int skip, int take, int recInvHeaderProd, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRecInvDetailProd> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblRecInvHeaderProd ==(@Group0)";
                    valuesObjects.Add("Group0", recInvHeaderProd);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblRecInvDetailProds.Where(filter, parameterCollection).Count();
                    query = entity.TblRecInvDetailProds.Include("TblCurrency1").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblRecInvDetailProds.Count(v => v.TblRecInvHeaderProd == recInvHeaderProd);
                    query = entity.TblRecInvDetailProds.Include("TblCurrency1").OrderBy(sort).Where(v => v.TblRecInvHeaderProd == recInvHeaderProd).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblRecInvHeaderProd UpdateOrInsertTblRecInvHeaderProds(TblRecInvHeaderProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblRecInvHeaderProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvHeaderProds
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
        private TblRecInvMainDetailProd UpdateOrInsertTblRecInvMainDetailProd(TblRecInvMainDetailProd newRow, bool save, int index, out int outindex, string company, out double TotalQty, out double TotalAmount)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblRecInvMainDetailProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvMainDetailProds
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                        var row =
                            entity.TblRecInvDetailProds.Where(
                                x => x.TblRecInvHeaderProd == newRow.TblRecInvHeaderProd && x.Tblitem == newRow.TblItem && x.ItemType == newRow.ItemType
                                &&x.BatchNo==newRow.BatchNo && x.SizeCode==newRow.SizeCode&&x.TblColor==newRow.TblColor
                                ).ToList();

                        foreach (var TblRecInvMainDetailProd in row)
                        {
                            TblRecInvMainDetailProd.Cost = newRow.Cost;
                            TblRecInvMainDetailProd.ExchangeRate = newRow.ExchangeRate;


                        }
                    }
                }

                entity.SaveChanges();
                TotalQty = entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == newRow.TblRecInvHeaderProd).Sum(x => x.Qty);
                TotalAmount = entity.TblRecInvMainDetailProds.Where(x => x.TblRecInvHeaderProd == newRow.TblRecInvHeaderProd).Sum(x => x.Qty * x.Cost);

                return newRow;
            }
        }

        [OperationContract]
        private TblRecInvDetailProd UpdateOrInsertTblRecInvDetailProds(TblRecInvDetailProd newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblRecInvDetailProds.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblRecInvDetailProds
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
        private int DeleteTblRecInvHeaderProd(TblRecInvHeaderProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblRecInvHeaderProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                entity.CommandTimeout = 0;
                if (query != null)
                {
                    entity.DeleteObject(query);

                    using (var context = new ccnewEntities(GetSqlConnectionString(company)))
                    {
                        var ledgerheader = context.TblLedgerHeaders.FirstOrDefault(w => w.TblJournalLink == query.Iserial && w.TblTransactionType == 100);

                        if (ledgerheader != null)
                        {
                            context.DeleteObject(ledgerheader);
                        }
                    }

                }
                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private int DeleteTblRecInvMainDetailProd(TblRecInvMainDetailProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblRecInvMainDetailProds
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblRecInvDetailProd DeleteTblRecInvDetailProd(TblRecInvDetailProd row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblRecInvDetailProds
                             where e.Glserial == row.Glserial
                                 && e.Dserial == row.Dserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        private List<TblPurchaseReceiveHeader> GetTblRecieveHeaderProd(int skip, int take, int type, string supplier, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var iserials =
                    entity.TblRecInvHeaderLinkProds.Where(x => x.TblRecInvHeaderTypeProd == type && x.TblPurchaseReceiveHeader1.Vendor == supplier)
                        .Select(x => x.TblPurchaseReceiveHeader);
                IQueryable<TblPurchaseReceiveHeader> query;

                if (filter != null)
                {
                    foreach (var variable in iserials)
                    {
                        filter = filter + " and it.Iserial !=(@Group" + variable + ")";
                        valuesObjects.Add("Group" + variable + "", variable);
                    }
                    filter = filter + " and it.Vendor ==(@Sup)";
                    valuesObjects.Add("Sup", supplier);
                    filter = filter + " and it.TblInventType ==(@s)";
                    valuesObjects.Add("s", type);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = entity.TblPurchaseReceiveHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        entity.TblPurchaseReceiveHeaders.Include("TblWarehouse1")
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount =
                        entity.TblPurchaseReceiveHeaders.Count(
                            x => !iserials.Contains(x.Iserial) && x.Vendor == supplier && x.TblInventType == type);
                    query =
                        entity.TblPurchaseReceiveHeaders.Include("TblWarehouse1")
                            .OrderBy(sort)
                            .Where(x => !iserials.Contains(x.Iserial) && x.Vendor == supplier && x.TblInventType == type)
                            .Skip(skip)
                            .Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblPurchaseReceiveHeader> GetTblRecieveHeaderProdFromTo(DateTime From, DateTime To, int type, string supplier, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                To = To.AddDays(1);
                var iserials =
                      entity.TblRecInvHeaderLinkProds.Where(x => x.TblRecInvHeaderTypeProd == type && x.TblPurchaseReceiveHeader1.Vendor == supplier)
                          .Select(x => x.TblPurchaseReceiveHeader);
                IQueryable<TblPurchaseReceiveHeader> query;

                query =
                    entity.TblPurchaseReceiveHeaders.Include("TblWarehouse1")
                        .Where(x => !iserials.Contains(x.Iserial) && x.Vendor == supplier
                        && x.DocDate >= From && x.DocDate < To);

                return query.ToList();
            }
        }

        public bool Find(string tableName, string code, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = "SELECT Code FROM dbo." + tableName + " Where Code ={0}";

                var result = context.ExecuteStoreQuery<string>(query, code);  //<>("");
                return result.Any();
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
                    if (Find("TblRecInvHeader", code, company))
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
        private TblRecInvHeaderProd GetTblRecieveDetail(List<int?> headerProds, TblRecInvHeaderProd tblRecInvHeaderProd, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                tblRecInvHeaderProd.Code = HandelSequence(tblRecInvHeaderProd.Code, company);
                tblRecInvHeaderProd.CreationDate = DateTime.Now;
                entity.TblRecInvHeaderProds.AddObject(tblRecInvHeaderProd);
                entity.SaveChanges();
                foreach (var headerProd in headerProds)
                {
                    entity.TblRecInvHeaderLinkProds.AddObject(new TblRecInvHeaderLinkProd
                    {
                        TblRecInvHeaderProd = tblRecInvHeaderProd.Iserial,
                        TblRecInvHeaderTypeProd = tblRecInvHeaderProd.TblRecInvHeaderTypeProd,
                        TblPurchaseReceiveHeader = (int)headerProd
                    });
                }
                string comand = "select tblPurchaseReceiveDetail.BatchNo,TblPurchaseOrderDetailRequest.ItemId Style,TblPurchaseOrderDetailRequest.FabricColor TblColor,TblPurchaseOrderDetailRequest.Size SizeCode,TblPurchaseOrderHeaderRequest.TblCurrency Currency" +
",CAST( isnull(SUM(TblPurchaseReceiveDetail.Qty*TblPurchaseReceiveDetail.Cost)/SUM(TblPurchaseReceiveDetail.Qty),0) AS DECIMAL(19,4)) Cost" +
",CAST(sum(TblPurchaseReceiveDetail.Qty) AS DECIMAL(19,4)) Quantity  from TblPurchaseReceiveDetail" +
" inner join TblPurchaseOrderDetailRequest on TblPurchaseOrderDetailRequest.Iserial= TblPurchaseReceiveDetail.TblPurchaseOrderDetailRequest" +
" inner join TblPurchaseOrderHeaderRequest on TblPurchaseOrderDetailRequest.TblPurchaseOrderHeaderRequest=TblPurchaseOrderHeaderRequest.Iserial" +
" where  TblPurchaseReceiveDetail.TblPurchaseReceiveHeader in ({0}) " +
 " group by TblPurchaseReceiveDetail.BatchNo,TblPurchaseOrderDetailRequest.ItemId,TblPurchaseOrderDetailRequest.FabricColor,Size,TblPurchaseOrderHeaderRequest.TblCurrency ";
                comand = comand.Replace("{0}", string.Join(",", headerProds));
                List<RecInvDataTable> List = entity.ExecuteStoreQuery<RecInvDataTable>(comand).ToList();
                foreach (var row in List)
                {
                    var firstOrDefault = entity.Fabric_UnitID.FirstOrDefault(x => x.Fabric_Code == row.Style);
                    if (firstOrDefault != null)
                    {
                        var styleIserial = firstOrDefault;
                        var newRow = new TblRecInvMainDetailProd
                        {
                            ItemType = firstOrDefault.Type,
                            Cost = (double)row.Cost,
                            TblCurrency = row.Currency,
                            Qty = (double)row.Quantity,
                            TblItem = styleIserial.Iserial,
                            TblRecInvHeaderProd = tblRecInvHeaderProd.Iserial,
                            SizeCode = row.SizeCode,
                            TblColor = row.TblColor,
                            BatchNo = row.BatchNo,
                            ExchangeRate=1
                        };
                        entity.TblRecInvMainDetailProds.AddObject(newRow);
                    }
                }
                IQueryable<TblPurchaseReceiveDetail> query = entity.TblPurchaseReceiveDetails.Include("TblPurchaseOrderDetailRequest1").Where(x => headerProds.Contains(x.TblPurchaseReceiveHeader));
                foreach (var row in query)
                {
                    var firstOrDefault = entity.Fabric_UnitID.FirstOrDefault(x => x.Fabric_Code == row.TblPurchaseOrderDetailRequest1.ItemId);
                    if (firstOrDefault != null)
                    {
                        var recDetail = new TblRecInvDetailProd
                        {
                            Cost = row.Cost,
                            TblRecInvHeaderProd = tblRecInvHeaderProd.Iserial,
                            Dserial = row.Iserial,
                            Flg = 0,
                            Glserial = (int)row.TblPurchaseReceiveHeader,
                            Tblitem = firstOrDefault.Iserial,
                            ItemType = firstOrDefault.Type,
                            TblColor = row.TblPurchaseOrderDetailRequest1.FabricColor,
                            BatchNo = row.BatchNo,
                            SizeCode = row.TblPurchaseOrderDetailRequest1.Size,
                            ExchangeRate = 1
                        };
                        entity.TblRecInvDetailProds.AddObject(recDetail);
                    }
                }
                entity.SaveChanges();
            }
            return tblRecInvHeaderProd;
        }
    }
}