using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Dto_s;
using Microsoft.Dynamics.BusinessConnectorNet;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public void PostPOWithPurchIDToAx(TblSalesOrder po, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var headerObjToPost = context.TblSalesOrders.Include("TblStyle1").SingleOrDefault(x => x.Iserial == po.Iserial);
                var detailsObjToPost = context.TblSalesOrderColors.Include("TblSalesOrderSizeRatios").Include("TblSalesOrderSizeRatios").Where(x => x.TblSalesOrder == po.Iserial).ToList();
                var axapta = new Axapta();

                var credential = new NetworkCredential("bcproxy", "around1");

                TblAuthUser userToLogin;
                using (var model = new WorkFlowManagerDBEntities())
                {
                    userToLogin = model.TblAuthUsers.SingleOrDefault(x => x.Iserial == userIserial);
                }
                string dataarea = "CCM";
                if (headerObjToPost.SalesOrderType == (int)Enums.SalesOrderType.RetailPo)
                {
                    dataarea = "CCR";
                }
                axapta.LogonAs(userToLogin.User_Win_Login, userToLogin.User_Domain, credential, dataarea, null, null, null);

                var inventDim = axapta.CreateAxaptaRecord("InventDim");

                try
                {
                    //Start Transaction
                    axapta.TTSBegin();

                    var tableName = "PurchTable";
                    var _PurchID = headerObjToPost.SalesOrderCode;
                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("PurchId", _PurchID);
                    axaptaRecord.set_Field("DeliveryDate", headerObjToPost.DeliveryDate);

                    var header = axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.TblSupplier) as AxaptaRecord;
                    axaptaRecord.Call("initFromVendTable", header);

                    axaptaRecord.Insert();

                    tableName = "PurchLine";
                    foreach (var item in detailsObjToPost)
                    {
                        foreach (var sizeRow in item.TblSalesOrderSizeRatios)
                        {
                            var styleId = headerObjToPost.TblStyle1.StyleCode;

                            axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                            axaptaRecord.Clear();
                            axaptaRecord.InitValue();

                            inventDim.Clear();
                            //  InventDim.set_Field("InventLocationId", headerObjToPost.WareHouseID);
                            inventDim.set_Field("InventColorId", item.TblColor1.Code);
                            inventDim.set_Field("InventSizeId", sizeRow.Size);
                            inventDim = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventDim) as AxaptaRecord;

                            var tempx = inventDim.get_Field("inventDimId").ToString();
                            axaptaRecord.set_Field("InventDimId", tempx);

                            axaptaRecord.set_Field("ItemId", styleId);
                            axaptaRecord.set_Field("purchId", _PurchID);

                            axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(sizeRow.ProductionPerSize.ToString()));
                            axaptaRecord.set_Field("PurchPrice", item.LocalCost + item.AdditionalCost);
                            axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(sizeRow.ProductionPerSize.ToString()));
                            axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(sizeRow.ProductionPerSize.ToString()));
                            axaptaRecord.Call("createLine", true, true, false, true, true, false);
                        }
                    }

                    //No errors occured, Commit!
                    //Axapta.TTSCommit();
                }
                catch (Exception ex)
                {
                    //There was some errors, Abort transaction and Raise error!
                    //Axapta.TTSAbort();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //Finally logoff the Axapta Session
                    axapta.Logoff();
                }
            }
        }

  
    }
}