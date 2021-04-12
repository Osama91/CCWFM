using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblIssueJournalHeader> GetTblIssueJournalHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblIssueJournalHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblIssueJournalHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblIssueJournalHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblIssueJournalHeaders.Count();
                    query = context.TblIssueJournalHeaders.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIssueJournalHeader UpdateOrInsertTblIssueJournalHeader(TblIssueJournalHeader newRow, bool save, int index, int userIserial, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblIssueJournalHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblIssueJournalHeaders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                //  PostIssueHeader(newRow.Iserial, userIserial);
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblIssueJournalHeader(TblIssueJournalHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblIssueJournalHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblIssueJournalDetail> GetTblIssueJournalDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<ItemsDto> itemsList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblIssueJournalDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblIssueJournalHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblIssueJournalDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        context.TblIssueJournalDetails.Include("TblColor1")
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    fullCount = context.TblIssueJournalDetails.Count(v => v.TblIssueJournalHeader == groupId);
                    query =
                        context.TblIssueJournalDetails.Include("TblColor1")
                            .OrderBy(sort)
                            .Where(v => v.TblIssueJournalHeader == groupId)
                            .Skip(skip)
                            .Take(take);
                }

                var fabricsIserial = query.Where(x => x.ItemType != "Accessories").Select(x => x.ItemCode);

                var accIserial = query.Where(x => x.ItemType == "Accessories").Select(x => x.ItemCode);

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
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIssueJournalDetail UpdateOrInsertTblIssueJournalDetail(TblIssueJournalDetail newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblIssueJournalDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblIssueJournalDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblIssueJournalDetail(TblIssueJournalDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblIssueJournalDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblIssueJournalReceiveHeader> GetTblIssueJournalReceiveHeader(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblIssueJournalReceiveHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblIssueJournalHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblIssueJournalReceiveHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblIssueJournalReceiveHeaders.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblIssueJournalReceiveHeaders.Count(v => v.TblIssueJournalHeader == groupId);
                    query = context.TblIssueJournalReceiveHeaders.OrderBy(sort).Where(v => v.TblIssueJournalHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIssueJournalReceiveHeader UpdateOrInsertTblIssueJournalReceiveHeader(TblIssueJournalReceiveHeader newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblIssueJournalReceiveHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblIssueJournalReceiveHeaders
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblIssueJournalReceiveHeader(TblIssueJournalReceiveHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblIssueJournalReceiveHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblIssueJournalReceiveDetail> GetTblIssueJournalReceiveDetail(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblIssueJournalReceiveDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblIssueJournalReceiveHeader ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblIssueJournalReceiveDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblIssueJournalReceiveDetails.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblIssueJournalReceiveDetails.Count(v => v.TblIssueJournalReceiveHeader == groupId);
                    query = context.TblIssueJournalReceiveDetails.OrderBy(sort).Where(v => v.TblIssueJournalReceiveHeader == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblIssueJournalReceiveDetail UpdateOrInsertTblIssueJournalReceiveDetail(TblIssueJournalReceiveDetail newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblIssueJournalReceiveDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblIssueJournalReceiveDetails
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblIssueJournalReceiveDetail(TblIssueJournalReceiveDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblIssueJournalReceiveDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void IssueJournalTransfer(TblIssueJournalHeader row, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                if (SharedOperation.UseAx())
                {


                    var transactionGuid = Guid.NewGuid().ToString();
                    var vendorWmsLocation = entities.GetWmsLocations.SingleOrDefault(x => x.VENDID == row.Vendor);
                    const string tableName = "PRODCONNECTION";
                    var vendorLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                    var axapta = new Axapta();
                    var credential = new NetworkCredential("bcproxy", "around1");
                    var detail = entities.TblIssueJournalDetails.Where(w => w.TblIssueJournalHeader == row.Iserial).ToList();

                    var userToLogin = entities.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                    axapta.CallStaticClassMethod("SysFlushAOD", "doFlush");
                    foreach (var item in detail)
                    {
                        var locationLoc = entities.GetLocations.SingleOrDefault(x => x.INVENTLOCATIONID == item.Location);

                        AxaptaRecord AxaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        AxaptaRecord.Clear();
                        AxaptaRecord.InitValue();

                        //Transfer To Vendor's Location
                        var fabriccode = entities.Fabric_UnitID.FirstOrDefault(w => w.Iserial == item.ItemCode);
                        AxaptaRecord.set_Field("TRANSID", row.Iserial);
                        AxaptaRecord.set_Field("RAWID", fabriccode.Fabric_Code);
                        AxaptaRecord.set_Field("RAWQTY", item.Qty);
                        AxaptaRecord.set_Field("UNITID", fabriccode.UnitID);
                        if (locationLoc != null) AxaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                        AxaptaRecord.set_Field("FROMLOCATION", item.Location);
                        AxaptaRecord.set_Field("FROMWAREHOUSE", item.Location);
                        AxaptaRecord.set_Field("FROMBATCH", item.Size ?? "Free");
                        AxaptaRecord.set_Field("FROMCONFIG",
                            entities.TblColors.FirstOrDefault(x => x.Iserial == item.TblColor).Code);
                        if (vendorLoc != null) AxaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                        if (vendorWmsLocation != null)
                        {
                            AxaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                            AxaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                        }
                        AxaptaRecord.set_Field("TOBATCH", item.Size ?? item.BatchNo ?? "Free");
                        AxaptaRecord.set_Field("TOCONFIG",
                            entities.TblColors.FirstOrDefault(x => x.Iserial == item.TblColor).Code);
                        AxaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                        AxaptaRecord.set_Field("JOURNALLINKID", row.Iserial);
                        AxaptaRecord.set_Field("TransactionGuid", transactionGuid);
                        AxaptaRecord.Insert();
                    }
                    var import = axapta.CreateAxaptaObject("CLEDyeProcesse");
                    try
                    {
                        var transfer = import.Call("run", row.Iserial, row.Iserial, 0, "Name", 1, DateTime.UtcNow.ToUniversalTime());
                        row.AxTransaction = transfer.ToString();
                        row.IsPosted = true;
                        entities.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    SharedOperation.ClearAxTable("PRODCONNECTION", axapta, transactionGuid);
                    axapta.Logoff();
                }
                else
                {
                    row.AxTransaction = "1111";
                    row.IsPosted = true;
                    entities.SaveChanges();
                }

            }
        }

        [OperationContract]
        public List<TblIssueJournalReceiveDetailService> GetTblIssueJournalReceiveDetailServices(int headerIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                return entities.TblIssueJournalReceiveDetailServices.Where(x => x.TblIssueJournalReceiveDetail == headerIserial).ToList();
            }
        }

        public ItemsDto GetItemCode(int iserial, string itemType)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                ItemsDto code;
                if (!itemType.Contains("Acc"))
                {
                    code = (from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM")
                            where (x.Iserial == iserial)
                            select new ItemsDto
                            {
                                Unit = x.tbl_lkp_UoM.Code,
                                Code = x.FabricID
                            }).FirstOrDefault();
                }
                else
                {
                    code = (from x in context.tbl_AccessoryAttributesHeader.Include("tbl_lkp_UoM")
                            where (x.Iserial == iserial)
                            select new ItemsDto
                            {
                                Unit = x.tbl_lkp_UoM.Code,
                                Code = x.Code
                            }).FirstOrDefault();
                }
                return code;
            }
        }

        public void PostIssueHeader(int iserial, int userIserial)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");

                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);
                }
                var header = entities.TblIssueJournalHeaders.Include("TblIssueJournalDetails").FirstOrDefault(x => x.Iserial == iserial);

                var vendorWmsLocation = entities.GetWmsLocations.FirstOrDefault(x => x.VENDID == header.Vendor);
                var vendorLoc = entities.GetLocations.FirstOrDefault(x => x.INVENTLOCATIONID == vendorWmsLocation.INVENTLOCATIONID);
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                const string tableName = "PRODCONNECTION";
                var transactionGuid = Guid.NewGuid().ToString();

                if (header != null)
                {
                    foreach (var item in header.TblIssueJournalDetails)
                    {
                        var locationLoc = entities.GetLocations.FirstOrDefault(x => x.INVENTLOCATIONID == item.Location);

                        AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();
                        //Transfer To Vendor's Location

                        var itemcode = GetItemCode(item.ItemCode, item.ItemType);
                        axaptaRecord.set_Field("DYEDITEM", itemcode.Code);
                        axaptaRecord.set_Field("TRANSID", item.TblIssueJournalHeader);
                        axaptaRecord.set_Field("RAWID", itemcode.Code);
                        axaptaRecord.set_Field("RAWQTY", item.Qty);
                        axaptaRecord.set_Field("DYEDQTY", item.Qty);
                        axaptaRecord.set_Field("UNITID", itemcode.Unit);
                        axaptaRecord.set_Field("FROMSITE", locationLoc.INVENTSITEID);
                        axaptaRecord.set_Field("FROMLOCATION", item.Location);
                        axaptaRecord.set_Field("FROMWAREHOUSE", item.Location);
                        axaptaRecord.set_Field("FROMBATCH", item.BatchNo);
                        axaptaRecord.set_Field("FROMCONFIG", item.TblColor1.Code);
                        axaptaRecord.set_Field("TOSITE", vendorLoc.INVENTSITEID);
                        axaptaRecord.set_Field("TOLOCATION", vendorWmsLocation.WMSLOCATIONID);
                        axaptaRecord.set_Field("TOWAREHOUSE", vendorWmsLocation.INVENTLOCATIONID);
                        axaptaRecord.set_Field("TOBATCH", item.BatchNo);
                        axaptaRecord.set_Field("TOCONFIG", item.TblColor1.Code);
                        axaptaRecord.set_Field("TRANSTYPE", Convert.ToInt64(0));
                        axaptaRecord.set_Field("JOURNALLINKID", item.TblIssueJournalHeader);
                        axaptaRecord.set_Field("TransactionGuid", transactionGuid);
                        axaptaRecord.Insert();
                    }
                }
            }
        }
    }
}