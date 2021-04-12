using System.ServiceModel;
using Microsoft.Dynamics.BusinessConnectorNet;
using System.Net;
using CCWFM.Web.Model;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void PostPOWithPurchIDToAx(int purchID)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var headerObjToPost = context.tbl_PurchaseOrderHeader.SingleOrDefault(x => x.TransID == purchID);
                var detailsObjToPost = context.v_PurchaseOrderDetailsWithSizes.Where(x => x.Trans_TransactionHeader == purchID).ToList();
                var axapta = new Axapta();

                var credential = new NetworkCredential("bcproxy", "around1");

                axapta.LogonAs("osama.gamal", "ccasual.loc", credential, "ccr", null, null, null);

                var inventTable = axapta.CreateAxaptaRecord("InventDim");
                //var inventColorTable = axapta.CreateAxaptaRecord("InventColor");
                //var inventSizeTable = axapta.CreateAxaptaRecord("InventSize");

                try
                {
                    //Start Transaction
                    //Axapta.TTSBegin();

                    //string _journalid = "143887_109";
                    var tableName = "PurchTable";
                    var _PurchID = "143887_109";
                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("PurchId", _PurchID);
                    axaptaRecord.set_Field("DeliveryDate", headerObjToPost.DelivaryDate);
                    axaptaRecord.set_Field("PurchId", _PurchID);

                    var header = axapta.CallStaticRecordMethod("VendTable", "find", headerObjToPost.Vendor) as AxaptaRecord;
                    axaptaRecord.Call("initFromVendTable", header);

                    axaptaRecord.Insert();

                    tableName = "PurchLine";
                    foreach (var item in detailsObjToPost)
                    {
                        var styleID = context.StyleHeader_SalesOrder
                            .Where(x => x.SalesOrderID == item.SalesOrder)
                            .Select(x => x.StyleHeader).FirstOrDefault().ToString();

                        axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                        axaptaRecord.Clear();
                        axaptaRecord.InitValue();

                        inventTable.Clear();
                        inventTable.set_Field("InventLocationId", headerObjToPost.WareHouseID);
                        inventTable.set_Field("InventColorId", item.Color);
                        inventTable.set_Field("InventSizeId", item.Size);
                        inventTable = axapta.CallStaticRecordMethod("InventDim", "findOrCreate", inventTable) as AxaptaRecord;

                        var tempx = inventTable.get_Field("inventDimId").ToString();
                        axaptaRecord.set_Field("InventDimId", tempx);

                        axaptaRecord.set_Field("ItemId", styleID);
                        axaptaRecord.set_Field("purchId", _PurchID);
                        axaptaRecord.set_Field("QtyOrdered", Convert.ToDecimal(item.TotalQty.ToString()));
                        axaptaRecord.set_Field("PurchPrice", item.PurchasePrice);
                        axaptaRecord.set_Field("PurchQty", Convert.ToDecimal(item.TotalQty.ToString()));
                        axaptaRecord.set_Field("LineAmount", Convert.ToDecimal(item.TotalQty.ToString()));
                        axaptaRecord.Call("createLine", true, true, true, true, true, true);
                    }
                    //No errors occured, Commit!
                    //Axapta.TTSCommit();
                }
                catch(Exception ex)
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