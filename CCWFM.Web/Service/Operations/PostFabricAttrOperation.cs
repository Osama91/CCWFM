using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void InsertFabItem(tbl_FabricAttriputes objToPost, string categoryName, string fabricType, int userIserial)
        {
            var axapta = new Axapta();
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");
                    var user = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);

                    axapta.LogonAs(user.User_Win_Login, user.User_Domain, credential, "Ccm", null, null, null);

                    axapta.TTSBegin();
                    var itemId = objToPost.FabricID;
                    var tableName = "InventTable";

                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    if (fabricType != null)
                    {
                        axaptaRecord.set_Field("ItemGroupId",
                            fabricType.ToUpper() == "KNITTED"
                                ? "KNITTED-FABRIC"
                                : "FINISHED-FABRIC");
                    }
                    else
                    {
                        axaptaRecord.set_Field("ItemGroupId",
                            categoryName.ToUpper() == "FINISHED"
                                ? "FINISHED-FABRIC"
                                : categoryName.ToUpper() == "RAW"
                                    ? "KNITTED-FABRIC"
                                    : categoryName == "DYINGWOVEN"
                                        ? "FINISHED-FABRIC"
                                        : categoryName == "DYINGKNITED"
                                            ? "FINISHED-FABRIC"
                                            : categoryName == "Accessories"
                                                ? "ACCESSORIES"
                                                : "YARN"
                            );
                    }
                    if (objToPost.Notes != null) axaptaRecord.set_Field("Notes", objToPost.Notes);
                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ItemName", objToPost.FabricDescription);
                    axaptaRecord.set_Field("ModelGroupID", "STD");
                    axaptaRecord.set_Field("ItemType",
                        categoryName.ToUpper() == "FINISHED"
                            ? 0
                            : categoryName.ToUpper() == "RAW"
                                ? 1
                                : categoryName.ToUpper() == "DYINGWOVEN"
                                    ? 1
                                    : categoryName.ToUpper() == "DYINGKNITED"
                                        ? 1
                                        : 0);

                    if (objToPost.DyingClassificationID != null)
                    {
                        //DyedGroup
                        axaptaRecord.set_Field("DyedGroup", objToPost.DyingClassificationID.ToString());
                    }
                    axaptaRecord.set_Field("DimGroupId", "FABRIC");
                    //Commit the record to the database.
                    axaptaRecord.Insert();

                    tableName = "InventTableModule";
                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 0);
                    axaptaRecord.set_Field("Price", 0.00);
                    if (objToPost.UoMID != null)
                    {
                        var tempu = context.tbl_lkp_UoM
                            .Where(x => x.Iserial == objToPost.UoMID)
                            .Select(x => x.Ename)
                            .SingleOrDefault();
                        axaptaRecord.set_Field("UnitId", tempu);
                        //axaptaRecord.set_Field("PriceUnit", tempu);
                    }
                    //
                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 1);
                    axaptaRecord.set_Field("Price", 0.00);
                    if (objToPost.UoMID != null)
                    {
                        var tempu = context.tbl_lkp_UoM
                            .Where(x => x.Iserial == objToPost.UoMID)
                            .Select(x => x.Ename)
                            .SingleOrDefault();
                        axaptaRecord.set_Field("UnitId", tempu);
                        //axaptaRecord.set_Field("PriceUnit", tempu);
                    }
                    //
                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 2);
                    axaptaRecord.set_Field("Price", 0.00);
                    if (objToPost.UoMID != null)
                    {
                        var tempu = context.tbl_lkp_UoM
                            .Where(x => x.Iserial == objToPost.UoMID)
                            .Select(x => x.Ename)
                            .SingleOrDefault();
                        axaptaRecord.set_Field("UnitId", tempu);
                        //axaptaRecord.set_Field("PriceUnit", tempu);
                    }
                    //
                    axaptaRecord.Insert();

                    tableName = "InventItemLocation";

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("InventDIMID", "AllBlank");
                    axaptaRecord.Insert();
                    try
                    {
                        tableName = "ConfigTable";
                        axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        axaptaRecord.set_Field("ItemId", itemId);
                        axaptaRecord.set_Field("ConfigId", "Free");
                        axaptaRecord.set_Field("Name", "Free");
                        //Commit the record to the database.
                        axaptaRecord.Insert();
                    }
                    catch
                    {
                        //if record exists do nothing and continue
                    }

                    // Commit the record to the database.
                    var categ = (from x in context.tbl_FabricCategories
                                 where x.Iserial == objToPost.FabricCategoryID
                                 select x).FirstOrDefault();
                    if (categ != null)
                    {
                        try
                        {
                            axapta.CallStaticRecordMethod("TblFabricCategory", "findOrCreate", categ.Iserial, categ.Code,
                                categ.Ename, categ.Aname);
                        }
                        catch
                        {
                            //if record exists do nothing and continue
                        }
                    }

                    var temp = (from x in context.tbl_FabricAttriputes
                                    .Include("tbl_lkp_FabricDesignes").Include("tbl_lkp_FabricFinish").Include("tbl_lkp_FabricMaterials")
                                    .Include("tbl_lkp_FabricStructure").Include("tbl_lkp_FabricTypes").Include("tbl_lkp_Inch").Include("tbl_lkp_Gauges")
                                    .Include("tbl_lkp_ThreadNumbers").Include("tbl_lkp_YarnSource").Include("tbl_lkp_YarnCount").Include("tbl_lkp_YarnFinish")
                                    .Include("tbl_lkp_YarnStatus")
                                where x.FabricCategoryID == objToPost.FabricCategoryID
                                      && x.FabricID == objToPost.FabricID
                                select x).FirstOrDefault();

                    if (temp != null)
                    {
                        if (temp.tbl_lkp_FabricDesignes != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblFabricDesign", "findOrCreate", temp.tbl_lkp_FabricDesignes.Iserial, temp.tbl_lkp_FabricDesignes.Code,
                                  temp.tbl_lkp_FabricDesignes.Ename, temp.tbl_lkp_FabricDesignes.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_FabricFinish != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblFabricFinish", "findOrCreate", temp.tbl_lkp_FabricFinish.Iserial, temp.tbl_lkp_FabricFinish.Code,
                                  temp.tbl_lkp_FabricFinish.Ename, temp.tbl_lkp_FabricFinish.Aname);
                            }
                            catch
                            {
                            }
                        }
                        if (temp.tbl_lkp_FabricMaterials != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblFabricMaterial", "findOrCreate", temp.tbl_lkp_FabricMaterials.Iserial, temp.tbl_lkp_FabricMaterials.Code,
                                  temp.tbl_lkp_FabricMaterials.Ename, temp.tbl_lkp_FabricMaterials.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_FabricStructure != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblFabricStructure", "findOrCreate", temp.tbl_lkp_FabricStructure.Iserial, temp.tbl_lkp_FabricStructure.Code,
                                  temp.tbl_lkp_FabricStructure.Ename, temp.tbl_lkp_FabricStructure.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_FabricTypes != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblFabricType", "findOrCreate", temp.tbl_lkp_FabricTypes.Iserial, temp.tbl_lkp_FabricTypes.Code,
                                  temp.tbl_lkp_FabricTypes.Ename, temp.tbl_lkp_FabricTypes.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_Inch != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblInch", "findOrCreate", temp.tbl_lkp_Inch.Iserial, temp.tbl_lkp_Inch.Code,
                                  temp.tbl_lkp_Inch.Ename, temp.tbl_lkp_Inch.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_Gauges != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblGuage", "findOrCreate", temp.tbl_lkp_Gauges.Iserial, temp.tbl_lkp_Gauges.Code,
                                  temp.tbl_lkp_Gauges.Ename, temp.tbl_lkp_Gauges.Aname);
                            }
                            catch
                            {
                            }
                        }
                        if (temp.tbl_lkp_ThreadNumbers != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblThread", "findOrCreate", temp.tbl_lkp_ThreadNumbers.Iserial, temp.tbl_lkp_ThreadNumbers.Code,
                                  temp.tbl_lkp_ThreadNumbers.Ename, temp.tbl_lkp_ThreadNumbers.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_YarnSource != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblYarnSource", "findOrCreate", temp.tbl_lkp_YarnSource.Iserial, temp.tbl_lkp_YarnSource.Code,
                                  temp.tbl_lkp_YarnSource.Ename, temp.tbl_lkp_YarnSource.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_YarnCount != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblYarnCount", "findOrCreate", temp.tbl_lkp_YarnCount.Iserial, temp.tbl_lkp_YarnCount.Code,
                                  temp.tbl_lkp_YarnCount.Ename, temp.tbl_lkp_YarnCount.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_YarnFinish != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblYarnFinish", "findOrCreate", temp.tbl_lkp_YarnFinish.Iserial, temp.tbl_lkp_YarnFinish.Code,
                                  temp.tbl_lkp_YarnFinish.Ename, temp.tbl_lkp_YarnFinish.Aname);
                            }
                            catch
                            {
                            }
                        }

                        if (temp.tbl_lkp_YarnStatus != null)
                        {
                            try
                            {
                                axapta.CallStaticRecordMethod("TblYarnStatus", "findOrCreate", temp.tbl_lkp_YarnStatus.Iserial, temp.tbl_lkp_YarnStatus.Code,
                                  temp.tbl_lkp_YarnStatus.Ename, temp.tbl_lkp_YarnStatus.Aname);
                            }
                            catch
                            {
                            }
                        }

                        axapta.CallStaticRecordMethod("TblFabricDetail", "findOrCreate", temp.FabricID, temp.YarnStatusID ?? 0, temp.YarnFinishesID ?? 0, temp.YarnCountID ?? 0, temp.YarnSource ?? 0, temp.ThreadNumbersID ?? 0, temp.GaugesID ?? 0,
                            temp.InchesID ?? 0, temp.FabricTypesID ?? 0, temp.FabricStructuresID ?? 0, temp.FabricMaterialsID ?? 0, temp.FabricFinishesID ?? 0, temp.FabricDesignsID ?? 0, temp.FabricCategoryID
                                 );

                        temp.Status = 2;
                    }

                    context.SaveChanges();
                    axapta.TTSCommit();
                }
            }
            catch (Exception ex)
            {
                axapta.TTSAbort();
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateFabItem(tbl_FabricAttriputes objToPost, int userIserial)
        {
            var axapta = new Axapta();
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");
                    var user = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);

                    axapta.LogonAs(user.User_Win_Login, user.User_Domain, credential, "Ccm", null, null, null);

                    axapta.TTSBegin();
                    var itemId = objToPost.FabricID;
                    var tableName = "InventTable";

                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();
                    var header = axapta.CallStaticRecordMethod("InventTable", "find", itemId,true) as AxaptaRecord;
                    header.set_Field("ItemName", objToPost.FabricDescription);
                 
                    context.SaveChanges();
                    axapta.TTSCommit();
                }
            }
            catch (Exception ex)
            {
                axapta.TTSAbort();
                throw ex;
            }
        }
        public bool DeleteAxFabItem(tbl_FabricAttriputes objToPost, bool deleteColors, int userIserial)
        {
            var axapta = new Axapta();
            try
            {
                var credential = new NetworkCredential("bcproxy", "around1");
                var user = new TblAuthUser();
                using (var context = new WorkFlowManagerDBEntities())
                {
                    user = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);
                }

                axapta.LogonAs(user.User_Win_Login, user.User_Domain, credential, "Ccm", null, null, null);

                axapta.TTSBegin();
                var itemId = objToPost.FabricID;
                AxaptaRecord axaptaRecord;

                var tableName = "InventTable";
                using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                {
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                    axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                    // If the record is found then delete the record.
                    if (axaptaRecord.Found)
                    {
                        // Start a transaction that can be committed.
                        axapta.TTSBegin();
                        if (objToPost.Notes != null) axaptaRecord.set_Field("Notes", objToPost.Notes);
                        // Commit the transaction.
                        axapta.TTSCommit();
                    }
                }

                //tableName = "TblFabricDetail";
                //using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                //{
                //    axaptaRecord.Clear();
                //    axaptaRecord.InitValue();

                //    // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                //    axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                //    // If the record is found then delete the record.
                //    if (axaptaRecord.Found)
                //    {
                //        // Start a transaction that can be committed.
                //        axapta.TTSBegin();
                //        if (objToPost.Notes != null) axaptaRecord.set_Field("Notes", objToPost.Notes);
                //        // Commit the transaction.
                //        axapta.TTSCommit();
                //    }
                //}
                tableName = "INVENTTRANS";
                bool inventTransExists = false;
                using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                {
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                    axaptaRecord.ExecuteStmt("select  * from %1 where %1.ItemId =='" + itemId + "'");
                    // If the record is found then delete the record.
                    if (axaptaRecord.Found)
                    {
                        return inventTransExists = true;
                    }
                }
                if (!inventTransExists)
                {
                    tableName = "InventTableModule";
                    using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                    {
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                        axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                        // If the record is found then delete the record.
                        if (axaptaRecord.Found)
                        {
                            // Start a transaction that can be committed.
                            axapta.TTSBegin();
                            axaptaRecord.Delete();
                            // Commit the transaction.
                            axapta.TTSCommit();
                        }
                    }

                    tableName = "InventItemLocation";

                    using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                    {
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                        axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                        // If the record is found then delete the record.
                        if (axaptaRecord.Found)
                        {
                            // Start a transaction that can be committed.
                            axapta.TTSBegin();
                            axaptaRecord.Delete();
                            // Commit the transaction.
                            axapta.TTSCommit();
                        }
                    }

                    if (deleteColors)
                    {
                        tableName = "ConfigTable";
                        using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                        {
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                            axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                            // If the record is found then delete the record.
                            if (axaptaRecord.Found)
                            {
                                // Start a transaction that can be committed.
                                axapta.TTSBegin();
                                axaptaRecord.Delete();
                                // Commit the transaction.
                                axapta.TTSCommit();
                            }
                        }
                    }

                    tableName = "InventTable";
                    using (axaptaRecord = axapta.CreateAxaptaRecord(tableName))
                    {
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        // Execute a query to retrieve an editable record where the StatGroupName is “High Priority Customer”.
                        axaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.ItemId =='" + itemId + "'");
                        // If the record is found then delete the record.
                        if (axaptaRecord.Found)
                        {
                            // Start a transaction that can be committed.
                            axapta.TTSBegin();
                            axaptaRecord.Delete();
                            // Commit the transaction.
                            axapta.TTSCommit();
                        }
                    }
                    // Commit the record to the database.
                    axapta.TTSCommit();
                }
            }
            catch (Exception ex)
            {
                axapta.TTSAbort();
                throw ex;
            }
            return false;
        }
    }
}