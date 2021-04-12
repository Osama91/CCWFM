using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblTradeAgreementHeader> GetTblTradeAgreementHeaderList(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
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
        public List<TblTradeAgreementDetail> GetTblTradeAgreementDetailList(int skip, int take, int tradeAgreementHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblTradeAgreementHeader LIKE(@TblTradeAgreementHeader0)";
                    valuesObjects.Add("TblTradeAgreementHeader0", tradeAgreementHeader);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblTradeAgreementDetails
                    .Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblTradeAgreementDetails.Include("TblColor1")
                        .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblTradeAgreementDetails.Count(x => x.TblTradeAgreementHeader == tradeAgreementHeader);
                    query = context.TblTradeAgreementDetails.Include("TblColor1").Where(x => x.TblTradeAgreementHeader == tradeAgreementHeader).OrderBy(sort).Skip(skip).Take(take);
                }

                var fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.ItemCode);

                var accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.ItemCode);
                //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                var itemsquery = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                  where (fabricsIserial.Any(l => x.Iserial == l))

                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).Take(20).ToList();

                itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

                                     where (accIserial.Any(l => x.Iserial == l))
                                     select new ItemsDto
                                     {
                                         Iserial = x.Iserial,
                                         Code = x.Code,
                                         Name = x.Descreption,
                                         ItemGroup = "Accessories"
                                         ,
                                         IsSizeIncluded = x.IsSizeIncludedInHeader,

                                         Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
                                     }).Take(20).ToList());
                itemsList = itemsquery;
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblTradeAgreementDetail> GetTblTradeAgreementDetailListFabricView(int skip, int take, int item, string color, string size, string vendor, string itemtype, out List<ItemsDto> itemsList, out List<Vendor> VendorList, out Dictionary<int?, bool> TransactionExist)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblTradeAgreementDetail> query = context.TblTradeAgreementDetails.Include("TblVendorPurchaseGroup1").Include("TblTradeAgreementHeader1").Include("TblColor1").Where(x => x.ItemCode == item && x.ItemType == itemtype && (x.TblColor1.Code.StartsWith(color) || color == null) && (x.AccSize.StartsWith(size) || size == null) && (x.TblTradeAgreementHeader1.Vendor.StartsWith(vendor) || vendor == null)).OrderByDescending(w => w.Iserial).Skip(skip).Take(take);
                var fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.ItemCode);
                var accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.ItemCode);
                var vendors = query.Select(x => x.TblTradeAgreementHeader1.Vendor);
                //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
                var itemsquery = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
                                  where (fabricsIserial.Any(l => x.Iserial == l))
                                  select new ItemsDto
                                  {
                                      Iserial = x.Iserial,
                                      Code = x.FabricID,
                                      Name = x.FabricDescription,
                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
                                      Unit = x.tbl_lkp_UoM.Ename
                                  }).Take(20).ToList();

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
                                     }).Take(20).ToList());
                itemsList = itemsquery;

                VendorList = context.Vendors.Where(x => vendors.Contains(x.vendor_code) && x.DATAAREAID == "CCM").ToList();
                var listofiserial = query.Select(x => x.Iserial);
                TransactionExist = context.TblBOMStyleColorEstimateds.Where(x => listofiserial.Any(l => x.TblTradeAgreementDetail == l))
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
        public List<TblTradeAgreementDetail> SaveTradeAgreement(TblTradeAgreementHeader header)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (entities.TblTradeAgreementHeaders.Any(x => x.Vendor == header.Vendor && x.FromDate == header.FromDate && x.ToDate == header.ToDate))
                {
                    var oldHeader = (from s in entities.TblTradeAgreementHeaders
                                     where s.Iserial == header.Iserial
                                     select s).SingleOrDefault();
                    GenericUpdate(oldHeader, header, entities);
                    foreach (var detail in header.TblTradeAgreementDetails)
                    {
                        detail.TblTradeAgreementHeader = oldHeader.Iserial;
                        if (detail.Iserial != 0)
                        {
                            var oldRow = (from s in entities.TblTradeAgreementDetails
                                          where s.Iserial == detail.Iserial
                                          select s).SingleOrDefault();
                            GenericUpdate(oldRow, detail, entities);
                        }
                        else
                        {
                            entities.TblTradeAgreementDetails.AddObject(detail);
                        }
                    }
                }
                else
                {
                    entities.TblTradeAgreementHeaders.AddObject(header);
                }

                entities.SaveChanges();
                return header.TblTradeAgreementDetails.ToList();
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
        private TblTradeAgreementDetail UpdateOrInsertTblTradeAgreementDetail(TblTradeAgreementDetail newRow, int index, out int outindex, int User)
        {
            outindex = index;

            using (var context = new WorkFlowManagerDBEntities())
            {
                var header = newRow.TblTradeAgreementHeader1;
                if (
              context.TblTradeAgreementHeaders.Any(
                  x => x.Vendor == header.Vendor && x.FromDate == header.FromDate && x.ToDate == header.ToDate))
                {
                    newRow.TblTradeAgreementHeader1 = null;
                    newRow.TblTradeAgreementHeader = context.TblTradeAgreementHeaders.FirstOrDefault(
                        x => x.Vendor == header.Vendor && x.FromDate == header.FromDate && x.ToDate == header.ToDate)
                        .Iserial;
                    var oldRow = (from e in context.TblTradeAgreementDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        newRow.CreatedBy = oldRow.CreatedBy;
                        newRow.CreationDate = oldRow.CreationDate;

                        newRow.LastUpdatedBy = User;
                        newRow.LastUpdatedDate = DateTime.Now;
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        newRow.CreatedBy = User;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastUpdatedDate = DateTime.Now;
                        context.TblTradeAgreementDetails.AddObject(newRow);
                    }
                }
                else
                {
                    var oldRow = (from e in context.TblTradeAgreementDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        context.TblTradeAgreementDetails.DeleteObject(oldRow);
                    }
                    newRow.Iserial = 0;
                    newRow.TblTradeAgreementHeader = 0;
                    newRow.TblTradeAgreementHeader1.Iserial = 0;
                    newRow.CreatedBy = User;
                    newRow.CreationDate = DateTime.Now;
                    newRow.LastUpdatedDate = DateTime.Now;
                    context.TblTradeAgreementDetails.AddObject(newRow);
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