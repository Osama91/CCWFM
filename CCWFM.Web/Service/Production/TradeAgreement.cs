using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using LinqKit;
using Omu.ValueInjecter;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        [OperationContract]
        public List<TblTradeAgreementTransaction> GetTblTradeAgreementTransaction(int skip,
            int take, string sort, string filter, Dictionary<string, object> valuesObjects,
            out int fullCount)
        {
            string itemCode = null;
            if (filter != null && valuesObjects != null && filter.Contains("it." + nameof(TblLkpSeason.Code)))
            {
                var val = valuesObjects.FirstOrDefault(k => k.Key.StartsWith(nameof(TblLkpSeason.Code)));
                if (val.Value != null)
                    itemCode = val.Value.ToString().Replace("%", "");
                var startIndex = filter.IndexOf(string.Format("it.{0} LIKE(", nameof(TblLkpSeason.Code)));
                var lastIndex = filter.IndexOf(")", startIndex);
                filter = filter.Remove(startIndex, lastIndex - startIndex + 1).Trim().Replace("  ", " ").Replace("and and", "and").Trim();
                if (filter.StartsWith("and"))
                    filter = filter.Remove(0, 3);
                if (filter.EndsWith("and"))
                    filter = filter.Remove(filter.Length - 3, 3);
                valuesObjects.Remove(val.Key);
                //valuesObjects.Add(val.Key, "%%");
            }
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementTransaction> query = context.TblTradeAgreementTransactions.Include(
                    nameof(TblTradeAgreementTransaction.TblLkpSeason1));
                if (string.IsNullOrWhiteSpace(itemCode))
                    query.Where(t => t.TblTradeAgreementHeaders.Any(h => h.TblTradeAgreementDetails.Any(d =>
                            context.FabricAccSearches.Any(f => f.Iserial == d.ItemCode && f.Code == itemCode))));
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblTradeAgreementTransactions.Where(filter, parameterCollection.ToArray()).Count();
                    query = query.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblTradeAgreementTransactions.Count();
                    query = query.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        //[OperationContract]
        public List<TblTradeAgreementHeader> GetTblTradeAgreementHeaderList(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblTradeAgreementHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblTradeAgreementHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblTradeAgreementHeaders.Count();
                    query = context.TblTradeAgreementHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblTradeAgreementDetail> GetTblTradeAgreementDetailListFabricView(int skip, int take, int TransactionIserial,
            string item, string color, string size, string vendor, string itemtype,
            out List<ItemsDto> itemsList, out List<Vendor> VendorList, out Dictionary<int?, bool> TransactionExist)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.CommandTimeout = 0;
                IQueryable<TblTradeAgreementDetail> temp =
                    context.TblTradeAgreementDetails.Include(nameof(TblTradeAgreementDetail.TblVendorPurchaseGroup1)).Include(
                        nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1)).Include(string.Format("{0}.{1}",
                        nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1), nameof(TblTradeAgreementHeader.TblTradeAgreementTransaction1))).Include(
                        nameof(TblTradeAgreementDetail.TblColor1));

                var predicate = PredicateBuilder.True<TblTradeAgreementDetail>();
                predicate = predicate.And(d =>
                        d.TblTradeAgreementHeader1.TblTradeAgreementTransaction == TransactionIserial);
                if (!string.IsNullOrWhiteSpace(item))
                {
                    IQueryable<int> ItemIserial;// = new List<int>();
                    try
                    {
                        ItemIserial = context.FabricAccSearches.Where(fas =>
                        fas.Code.StartsWith(item) && fas.ItemGroup != "FP").Distinct().Select(d => d.Iserial);
                        predicate = predicate.And(i => ItemIserial.Any(d => i.ItemCode == d));
                    }
                    catch (Exception ex) { }
                }
                if (!string.IsNullOrWhiteSpace(itemtype))
                    predicate = predicate.And(i => i.ItemType.Contains(itemtype));
                if (color != null)
                    predicate = predicate.And(i => i.TblColor1.Code.StartsWith(color));
                if (size != null)
                    predicate = predicate.And(i => i.AccSize.StartsWith(size));
                if (vendor != null)
                    predicate = predicate.And(i => i.TblTradeAgreementHeader1.Vendor.StartsWith(vendor));

                var query = temp.AsExpandable().Where(predicate).OrderByDescending(w =>
                        w.Iserial).Skip(skip).Take(take).ToList();

                List<int> fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.ItemCode).Distinct().ToList();
                List<int> accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.ItemCode).Distinct().ToList();
                var vendors = query.Select(x => x.TblTradeAgreementHeader1.Vendor).Distinct().ToList();
                //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                var itemsquery = context.tbl_FabricAttriputes.Include(nameof(tbl_FabricAttriputes.tbl_lkp_UoM)).Include(
                    nameof(tbl_FabricAttriputes.tbl_lkp_FabricMaterials))
                                  .Where(x => fabricsIserial.Contains(x.Iserial)).
                                  Select(x => new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).ToList();


                var tempQuery = context.tbl_AccessoryAttributesHeader.Include(nameof(tbl_AccessoryAttributesHeader.tbl_lkp_UoM))
                     .Where(x => accIserial.Contains(x.Iserial)).

                                     Select(x => new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = x.tbl_lkp_UoM.Ename,
                                     }).ToList();

                itemsquery.AddRange(tempQuery);
                itemsList = itemsquery;

                VendorList = context.Vendors.Where(x => vendors.Contains(x.vendor_code) && x.DATAAREAID == "CCM").ToList();
                var listofiserial = query.Select(x => x.Iserial).ToList();
                TransactionExist = context.TblBOMStyleColorEstimateds.Where(x => listofiserial.Contains(x.TblTradeAgreementDetail??0))
                    .GroupBy(x => x.TblTradeAgreementDetail).ToDictionary(t => t.Key, t => true);

                return query.ToList();
            }
        }

        [OperationContract]
        public List<ItemsDto> SearchItemForTradeAgreement(string item)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                             where (
                          x.FabricDescription.StartsWith(item) || x.FabricDescriptionAR.StartsWith(item)
                          || x.FabricID.StartsWith(item))
                             select new ItemsDto
                             {
                                 Iserial = x.Iserial,
                                 Code = x.FabricID,
                                 Name = x.FabricDescription,
                                 ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                 Unit = x.tbl_lkp_UoM.Ename
                             }).Take(20).ToList();

                query.AddRange((from x in context.tbl_AccessoryAttributesHeader
                                where (
                             x.Descreption.StartsWith(item) || x.Code.StartsWith(item))
                                //            let comList = x.tbl_AccessoryAttributesDetails.Where(s => s.Code == x.Code)
                                //            let accList = context.TblColors.Where(r => comList.All(l => r.Code == l.Configuration) && r.TblLkpColorGroup == 24)
                                select new ItemsDto
                                {
                                    Iserial = x.Iserial,
                                    Code = x.Code,
                                    Name = x.Descreption,
                                    ItemGroup = "Accessories"
                                    ,
                                    IsSizeIncluded = x.IsSizeIncludedInHeader,
                                    Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                    //         CombinationList = comList,
                                    //            AccConfigList = accList
                                }).Take(20).ToList());

                query.AddRange((from s in context.TblServices
                                where (s.Code.StartsWith(item) || s.Aname.StartsWith(item) || s.Ename.StartsWith(item))
                                select new ItemsDto
                                {
                                    Iserial = s.Iserial,
                                    Code = s.Code,
                                    Name = s.Ename,
                                    ItemGroup = s.ServiceGroup
                                }
                                    ));
                return query;
            }
        }

        [OperationContract]
        public List<TblTradeAgreementDetail> SaveTradeAgreement(TblTradeAgreementTransaction header,
            out TblTradeAgreementTransaction savedHeader)
        {
            header.TblLkpSeason1 = null;
            savedHeader = header;
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var oldTransaction = entities.TblTradeAgreementTransactions.FirstOrDefault(s =>
                s.Iserial == header.Iserial);
                if (oldTransaction != null)
                {
                    SharedOperation.GenericUpdate(oldTransaction, header, entities);

                    foreach (var headerItem in header.TblTradeAgreementHeaders)
                    {
                        headerItem.TblTradeAgreementTransaction = oldTransaction.Iserial;
                        var oldRow = entities.TblTradeAgreementHeaders.FirstOrDefault(
                            x => x.Vendor == headerItem.Vendor && x.FromDate == headerItem.FromDate && x.ToDate == headerItem.ToDate);
                        if (oldRow != null)
                        {
                            headerItem.Iserial = oldRow.Iserial;
                            SharedOperation.GenericUpdate(oldRow, headerItem, entities);
                            foreach (var detailItem in headerItem.TblTradeAgreementDetails)
                            {
                                detailItem.TblTradeAgreementHeader = headerItem.Iserial;
                                if (detailItem.Iserial != 0)
                                {
                                    var olddetailRow = entities.TblTradeAgreementDetails.SingleOrDefault(
                                                  s => s.Iserial == detailItem.Iserial);
                                    SharedOperation.GenericUpdate(olddetailRow, detailItem, entities);
                                }
                                else
                                {
                                    var tempDetail = new TblTradeAgreementDetail();
                                    tempDetail = detailItem.Clone();
                                    tempDetail.TblTradeAgreementHeader1 = null;
                                    entities.TblTradeAgreementDetails.AddObject(tempDetail);
                                }
                            }
                        }
                        else
                        {
                            var tempHeader = new TblTradeAgreementHeader();
                            tempHeader = headerItem.Clone();
                            tempHeader.TblTradeAgreementTransaction1 = null;
                            entities.TblTradeAgreementHeaders.AddObject(tempHeader);
                        }
                    }
                }
                else
                {
                    header.Serial = entities.TblTradeAgreementTransactions.Where(t =>
                        t.TblLkpSeason == header.TblLkpSeason
                        ).Select(t => t.Serial).DefaultIfEmpty(0).Max() + 1;
                    entities.TblTradeAgreementTransactions.AddObject(header);
                }

                entities.SaveChanges();

                List<TblTradeAgreementDetail> result = new List<TblTradeAgreementDetail>();
                header.TblTradeAgreementHeaders.ForEach(h => h.TblTradeAgreementDetails.ForEach(d => result.Add(d)));
                return result;
            }
        }

        [OperationContract]
        public int DeleteTradeAgreementHeader(int tblTradeAgreementHeaderIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var oldHeader = (from s in entities.TblTradeAgreementHeaders
                                 where s.Iserial == tblTradeAgreementHeaderIserial
                                 select s).SingleOrDefault();
                if (oldHeader != null) entities.DeleteObject(oldHeader);
                entities.SaveChanges();
                return oldHeader.Iserial;
            }
        }

        [OperationContract]
        public List<TblTradeAgreementDetail> GetTblTradeAgreementDetailList(int skip, int take, int tradeAgreementTransaction,
            string sort, string filter, Dictionary<string, object> valuesObjects,
            out int fullCount, out List<ItemsDto> itemsList, out List<Vendor> vendorsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblTradeAgreementHeader.TblTradeAgreementTransaction LIKE(@TblTradeAgreementTransaction0)";
                    valuesObjects.Add("TblTradeAgreementTransaction0", tradeAgreementTransaction);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblTradeAgreementDetails.Include(string.Format("{0}.{1}",
                        nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1), nameof(TblTradeAgreementHeader.TblTradeAgreementTransaction1)))
                    .Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblTradeAgreementDetails.Include(nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1)).Include(
                        string.Format("{0}.{1}", nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1), nameof(
                        TblTradeAgreementHeader.TblTradeAgreementTransaction1))).Include(nameof(TblTradeAgreementDetail.TblColor1)).Where(
                        filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblTradeAgreementDetails.Count(x => x.TblTradeAgreementHeader1.TblTradeAgreementTransaction == tradeAgreementTransaction);
                    query = context.TblTradeAgreementDetails.Include(nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1)).Include(
                        string.Format("{0}.{1}", nameof(TblTradeAgreementDetail.TblTradeAgreementHeader1), nameof(
                            TblTradeAgreementHeader.TblTradeAgreementTransaction1))).Include(nameof(TblTradeAgreementDetail.TblColor1)).Where(
                        x => x.TblTradeAgreementHeader1.TblTradeAgreementTransaction == tradeAgreementTransaction).OrderBy(sort).Skip(skip).Take(take);
                }

                var fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.ItemCode);

                var accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.ItemCode);
                //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                var itemsquery = (from x in context.tbl_FabricAttriputes.Include(nameof(tbl_FabricAttriputes.tbl_lkp_UoM)).Include(
                    nameof(tbl_FabricAttriputes.tbl_lkp_FabricMaterials))
                                  where (fabricsIserial.Any(l => x.Iserial == l))
                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).ToList();

                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader
                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories",
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,
                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).ToList());
                itemsList = itemsquery;
                vendorsList = context.Vendors.Where(v => query.Any(q => q.TblTradeAgreementHeader1.Vendor == v.vendor_code)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblTradeAgreementDetail UpdateOrInsertTblTradeAgreementDetail(TblTradeAgreementDetail newRow,
            int index, out int outindex, int User)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var header = newRow.TblTradeAgreementHeader1;
                header.TblTradeAgreementTransaction1 = null;
                newRow.TblColor1 = null;
                newRow.TblColor1Reference = null;
                newRow.TblLkpSeason1 = null;
                newRow.TblLkpSeason1Reference = null;
                newRow.TblVendorPurchaseGroup1 = null;
                newRow.TblVendorPurchaseGroup1Reference = null;
                if (context.TblTradeAgreementHeaders.Any(
                  x => x.Vendor == header.Vendor && x.FromDate == header.FromDate && x.ToDate == header.ToDate))
                {
                    newRow.TblTradeAgreementHeader1 = null;
                    newRow.TblTradeAgreementHeader = context.TblTradeAgreementHeaders.FirstOrDefault(
                        x => x.Vendor == header.Vendor && x.FromDate == header.FromDate && x.ToDate == header.ToDate).Iserial;
                    var oldRow = context.TblTradeAgreementDetails.
                                  SingleOrDefault(e => e.Iserial == newRow.Iserial);
                    if (oldRow != null)
                    {
                        newRow.CreatedBy = oldRow.CreatedBy;
                        newRow.CreationDate = oldRow.CreationDate;

                        newRow.LastUpdatedBy = User;
                        newRow.LastUpdatedDate = DateTime.Now;
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        newRow.CreatedBy = User;
                        newRow.LastUpdatedBy = User;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastUpdatedDate = DateTime.Now;
                        context.TblTradeAgreementDetails.AddObject(newRow);
                    }
                }
                else
                {
                    context.TblTradeAgreementHeaders.AddObject(newRow.TblTradeAgreementHeader1);
                    //newRow.TblTradeAgreementHeader1 = null;
                    var oldRow = context.TblTradeAgreementDetails.SingleOrDefault(e => e.Iserial == newRow.Iserial);
                    if (oldRow != null)
                    {
                        context.TblTradeAgreementDetails.DeleteObject(oldRow);
                    }
                    else
                    {
                        newRow.LastUpdatedBy = User;
                        newRow.LastUpdatedDate = DateTime.Now;
                        newRow.CreatedBy = User;
                        newRow.CreationDate = DateTime.Now;
                        context.TblTradeAgreementDetails.AddObject(newRow);
                    }
                    newRow.Iserial = 0;
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        public int DeleteTblTradeAgreementDetail(int rowIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var oldHeader = (from s in entities.TblTradeAgreementDetails
                                 where s.Iserial == rowIserial
                                 select s).SingleOrDefault();
                if (oldHeader != null) entities.DeleteObject(oldHeader);
                entities.SaveChanges();
                return oldHeader.Iserial;
            }
        }
    }
}