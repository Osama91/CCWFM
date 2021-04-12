using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
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
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_AccessoryAttributesHeader AddAllNewAccessoriesAttributes(tbl_AccessoryAttributesHeader header
            , List<tbl_AccessoryAttributesDetails> details, bool isSizeInHeader, int userIserial)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (!context.tbl_AccessoryAttributesHeader.Any(x => x.Code == header.Code))
                    {
                        context.tbl_AccessoryAttributesHeader.AddObject(header);
                    }
                    //       var colorGroup = context.TblLkpColorGroups.FirstOrDefault(x => x.Code == "Acc").Iserial;

                    if (SharedOperation.UseAx())
                    {

                        try
                        {
                            InsertAllNewAccessoryToAx(details, header, isSizeInHeader, userIserial);
                        }
                        catch (Exception)
                        {


                        }
                    }
                    foreach (var item in details)
                    {
                        context.tbl_AccessoryAttributesDetails.AddObject(item);

                    }

                    context.SaveChanges();

                    return context.tbl_AccessoryAttributesHeader.Include(nameof(tbl_AccessoryAttributesHeader.tbl_AccessoryAttributesDetails)).FirstOrDefault(x => x.Code == header.Code);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_AccessoryAttributesHeader UpdateAccessoriesAttributes(
            tbl_AccessoryAttributesHeader header
            , List<tbl_AccessoryAttributesDetails> detailsToAdd
            , List<tbl_AccessoryAttributesDetails> detailsToUpdate, int userIserial)
        {
            try
            {
                //InsertAllNewAccessoryToAx(detailsToAdd, header, true);
                using (var context = new WorkFlowManagerDBEntities())
                {
                    //       var htemp = context.tbl_AccessoryAttributesHeader.FirstOrDefault(x => x.Code == header.Code);
                    //     GenericUpdate(htemp, header, context);
                    //      var colorGroup = context.TblLkpColorGroups.FirstOrDefault(x => x.Code == "Acc").Iserial;
                    foreach (var item in detailsToUpdate)
                    {
                        var dtemp =
                            context.tbl_AccessoryAttributesDetails.FirstOrDefault(
                                x => x.Iserial == item.Iserial);
                        GenericUpdate(dtemp, item, context);
                    }

                    if (SharedOperation.UseAx())
                    {

                        try
                        {
                            UpdateAccDetailsInAX(detailsToUpdate, userIserial);
                            InsertAllNewAccessoryToAx(detailsToUpdate, header, false, userIserial);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    foreach (var item in detailsToAdd)
                    {
                        context.tbl_AccessoryAttributesDetails.AddObject(item);

                    }
                    if (SharedOperation.UseAx())
                    {
                        try
                        {
                            UpdateAccDetailsInAX(detailsToAdd, userIserial);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    context.SaveChanges();

                    return context.tbl_AccessoryAttributesHeader.Include(nameof(tbl_AccessoryAttributesHeader.tbl_AccessoryAttributesDetails)).FirstOrDefault(x => x.Code == header.Code);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_AccessoryAttributesDetails> GetAllAccessoriesAttributesDetails(int skip, int take, string accCode, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_AccessoryAttributesDetails> query;
                if (filter != null)
                {
                    filter = filter + " and it.Code LIKE(@Code0)";
                    valuesObjects.Add("Code0", accCode);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.tbl_AccessoryAttributesDetails
                    .Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tbl_AccessoryAttributesDetails
                        .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_AccessoryAttributesDetails.Count(x => x.Code == accCode);
                    query = context.tbl_AccessoryAttributesDetails.Where(x => x.Code == accCode).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_AccessoryAttributesHeader> GetAllAccessoriesHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out Dictionary<string, int> TransactionExist)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_AccessoryAttributesHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    var tempQuery = context.tbl_AccessoryAttributesHeader.Include(nameof(tbl_AccessoryAttributesHeader.tbl_AccessoriesSubGroup));
                    fullCount = tempQuery.Where(filter, parameterCollection.ToArray()).Count();
                    query = tempQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_AccessoryAttributesHeader.Count();
                    query = context.tbl_AccessoryAttributesHeader.Include(nameof(tbl_AccessoryAttributesHeader.tbl_AccessoriesSubGroup)).OrderBy(sort).Skip(skip).Take(take);
                }
                var listOfFabrics = query.Select(x => x.Code);
                int temp2;

                TransactionExist = context.tbl_AccessoryAttributesDetails.Where(x => listOfFabrics.Any(l => x.Code == l))
              .GroupBy(x => x.Code).ToDictionary(t => t.Key, grouping => grouping.Select(x => x.Configuration).ToList().Select(n => int.TryParse(n, out temp2) ? temp2 : 0).Max());
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAccSubGroupSizeLink> GetSuptGroupSizes(int supGroupId)
        {
            try
            {
                List<TblAccSubGroupSizeLink> temp;
                using (var context = new WorkFlowManagerDBEntities())
                {
                    temp = context.TblAccSubGroupSizeLinks.Include("TblAccSize1").Where(x => x.tbl_AccessoriesSubGroup == supGroupId).ToList();
                }
                if (temp.Count > 0)
                {
                    return temp;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_AccessoryAttributesHeader GetAccessoryHeaderOrDefault(string accCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return
                    context
                        .tbl_AccessoryAttributesHeader.FirstOrDefault(x => x.Code.ToLower() == accCode.ToLower());
            }
        }

        [OperationContract]
        private int DeleteAccDetail(tbl_AccessoryAttributesDetails row, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tbl_AccessoryAttributesDetails.Include(nameof(tbl_AccessoryAttributesDetails.tbl_AccessoryAttributesHeader))
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {

                    if (SharedOperation.UseAx())
                    {
                        if (!DeletedAxComination(oldRow, userIserial))
                        {
                            return 0;
                        }
                    }

                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
            }
            return row.Iserial;
        }

        #region [ Posting to AX Logic ]

        private void InsertAllNewAccessoryToAx(IEnumerable<tbl_AccessoryAttributesDetails> listObjToPost
            , tbl_AccessoryAttributesHeader objToPost, bool isSizeInCode, int userIserial)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var axapta = new Axapta();//Ready To be Dependent from Ax;

                    var credential = new NetworkCredential("bcproxy", "around1");

                    TblAuthUser userToLogin;
                    using (var model = new WorkFlowManagerDBEntities())
                    {
                        userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                    }
                    axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
                    var itemId = objToPost.Code.Trim();
                    var tableName = "InventTable";

                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();
                    var tblLkpItemGroupType = context.tbl_lkp_ItemGroupType.FirstOrDefault(x => x.Iserial ==
                                                                                                context.tbl_lkp_AccessoryGroup
                                                                                                    .FirstOrDefault(
                                                                                                        g =>
                                                                                                            g.Iserial ==
                                                                                                            objToPost.AccGroup)
                                                                                                    .tbl_lkp_ItemGroupType);
                    if (tblLkpItemGroupType != null)
                    {
                        var itmGroup = tblLkpItemGroupType.Code;
                        axaptaRecord.set_Field("ItemGroupId", itmGroup);
                    }
                    else
                    {
                        axaptaRecord.set_Field("ItemGroupId", "ACCESSORIES");
                    }
                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ItemName", objToPost.Descreption ?? itemId);
                    axaptaRecord.set_Field("ModelGroupID", "STD");
                    axaptaRecord.set_Field("ItemType", 0);
                    axaptaRecord.set_Field("DimGroupId", "ACC");
                    //Commit the record to the database.
                    axaptaRecord.Insert();
                    tableName = "InventTableModule";
                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 0);
                    if (objToPost.UoMID != null)
                    {
                        axaptaRecord.set_Field
                            ("UnitId",
                                context.tbl_lkp_UoM
                                .Where(x => x.Iserial == objToPost.UoMID)
                                .Select(x => x.Ename)
                                .SingleOrDefault()
                            );
                    }
                    //
                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 1);

                    //
                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 2);

                    //
                    axaptaRecord.Insert();

                    tableName = "InventItemLocation";

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("InventDIMID", "AllBlank");

                    // Commit the record to the database.
                    axaptaRecord.Insert();
                    foreach (var item in listObjToPost)
                    {
                        try
                        {
                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                            importNew.Call("CreateConfig", listObjToPost.Select(x => x.Code).FirstOrDefault(), item);
                        }
                        catch (Exception)
                        {
                        }
                        //    if (!isSizeInCode)
                        //      {
                        try
                        {
                            var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                            importNew.Call("CreateSize", listObjToPost.Select(x => x.Code).FirstOrDefault(), item);
                        }
                        catch (Exception)
                        {
                        }

                        //--------------------------------------------//
                        //--------------------------------------------//
                        //--------------------------------------------//
                        try
                        {
                            tableName = "InventDimCombination";
                            axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            axaptaRecord.set_Field("ItemId", itemId);
                            axaptaRecord.set_Field("InventSizeID", item.Size);
                            axaptaRecord.set_Field("ConfigId", item.Configuration);
                            //Commit the record to the database.
                            axaptaRecord.Insert();
                        }
                        catch (Exception)
                        {
                        }

                        //--------------------------------------------//
                        //--------------------------------------------//
                        //--------------------------------------------//
                        //      }
                    }
                    axapta.Logoff();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateAccDetailsInAX(List<tbl_AccessoryAttributesDetails> detailsToUpdate, int userIserial)
        {
            var axapta = new Axapta();//Ready To be Dependent from Ax

            var credential = new NetworkCredential("bcproxy", "around1");

            TblAuthUser userToLogin;
            using (var model = new WorkFlowManagerDBEntities())
            {
                userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
            }
            axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);

            foreach (var item in detailsToUpdate.Select(x => x.Configuration).Distinct())
            {
                try
                {
                    var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                    importNew.Call("CreateConfig", detailsToUpdate.Select(x => x.Code).FirstOrDefault(), item);
                }
                catch (Exception)
                {
                }
            }

            foreach (var item in detailsToUpdate.Select(x => x.Size).Distinct())
            {
                try
                {
                    var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                    importNew.Call("CreateSize", detailsToUpdate.Select(x => x.Code).FirstOrDefault(), item);
                }
                catch (Exception)
                {
                }
                //--------------------------------------------//
                //--------------------------------------------//
                //--------------------------------------------//C:\Users\GDE\Desktop\CCWFM\CCWFM.Web\Service\Operations\AccessoriesOperations.cs
            }

            foreach (var item in detailsToUpdate)
            {
                try
                {
                    const string tableName = "InventDimCombination";
                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", item.Code);
                    axaptaRecord.set_Field("InventSizeID", item.Size);
                    axaptaRecord.set_Field("ConfigId", item.Configuration);
                    //Commit the record to the database.
                    axaptaRecord.Insert();
                }
                catch (Exception)
                {
                }
            }

            axapta.Logoff();
        }

        private bool DeletedAxComination(tbl_AccessoryAttributesDetails detailsToUpdate, int userIserial)
        {
            var axapta = new Axapta();//Ready To be Dependent from Ax;

            var credential = new NetworkCredential("bcproxy", "around1"); //Ready To be Dependent from Ax
            bool result = false;
            TblAuthUser userToLogin;
            using (var model = new WorkFlowManagerDBEntities())
            {
                userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
            }
            axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, "Ccm", null, null, null);
            try
            {
                var importNew = axapta.CreateAxaptaObject("CreateProductionJournals");
                result = (bool)importNew.Call("DeleteInventDimCombination", detailsToUpdate.tbl_AccessoryAttributesHeader.Code, detailsToUpdate.Configuration, detailsToUpdate.Size);
            }
            catch (Exception)
            {
            }

            axapta.Logoff();
            return result;
        }

        #endregion [ Posting to AX Logic ]
    }
}