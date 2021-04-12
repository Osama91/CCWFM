using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Microsoft.Dynamics.BusinessConnectorNet;
using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;
using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    [DataContract]
    public class SizesWithGroups
    {
        [DataMember]
        public string SizeCode { get; set; }
        [DataMember]
        public string SizeGroup { get; set; }
        [DataMember]
        public string StyleCode { get; set; }
    }
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_PurchaseOrderHeader> GetPurchaseOrderHeaders(int? _Skip, int? _Take)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (_Skip != null)
                    return context.tbl_PurchaseOrderHeader.Skip((int)_Skip).Take(_Take ?? 0).ToList();
                else
                    return context.tbl_PurchaseOrderHeader.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<SizesWithGroups> GetStyleSizesFromRetail(List<string> styleCodes)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.v_RetailItemsWithSizeGroup.Where(x => styleCodes.Contains(x.Style))
                                    .Select(x => new { x.SizeCode , x.SizeGroupCode,x.Style })
                                    .Distinct().ToList();
                return temp.Select(sg => new SizesWithGroups {SizeCode = sg.SizeCode, SizeGroup = sg.SizeGroupCode,StyleCode = sg.Style}).ToList();
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<v_PurchaseOrderDetails> GetPurchaseOrderDetails(int? _Skip, int? _Take, int _CardHeaderID, int Direction, int RouteGroupID)
        {
            var temp = new List<v_PurchaseOrderDetails>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (_Skip != null)
                {
                    temp = context.v_PurchaseOrderDetails
                        .Where(x =>
                           x.Trans_TransactionHeader == _CardHeaderID)
                           .OrderBy(x => x.OrderingNumber).Skip((int)_Skip).Take((int)_Take).ToList();
                }
                else
                {
                    temp = context.v_PurchaseOrderDetails.Where(x =>
                        x.Trans_TransactionHeader == _CardHeaderID)
                        .OrderBy(x => x.OrderingNumber).ToList();
                }
            }
            return temp;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<v_PurchaseOrderDetails_SizeInfo> GetPurchaseOrderDetailsSizeInfo(int? _Skip, int? _Take
            , int _CardHeaderID, int Direction, int RouteGroupID, string _Color, string _ObjectIndex)
        {

            try
            {
                using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
                {
                    if (_Skip != null)
                    {
                        return context.v_PurchaseOrderDetails_SizeInfo
                            .Where(x =>
                            x.Trans_TransactionHeader == _CardHeaderID
                            && x.ObjectIndex == _ObjectIndex
                            && x.Color.ToLower() == _Color.ToLower()).Skip((int)_Skip).Take((int)_Take).ToList();
                    }
                    else
                    {
                        List<v_PurchaseOrderDetails_SizeInfo> temp = new List<v_PurchaseOrderDetails_SizeInfo>();

                        temp = (from x in context.v_PurchaseOrderDetails_SizeInfo
                                where x.Trans_TransactionHeader == _CardHeaderID
                                    && x.ObjectIndex == _ObjectIndex
                                    && x.Color.ToLower() == _Color.ToLower()
                                select x).ToList<v_PurchaseOrderDetails_SizeInfo>();
                        return temp.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void AddPurchaseOrder(tbl_PurchaseOrderHeader _Header, List<tbl_PurchaseOrderDetails> _Details)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    context.tbl_PurchaseOrderHeader.AddObject(_Header);
                    foreach (var item in _Details)
                    {
                        context.tbl_PurchaseOrderDetails.AddObject(item);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdatePurchaseOrder(tbl_PurchaseOrderHeader _Header, List<tbl_PurchaseOrderDetails> _Details)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                tbl_PurchaseOrderHeader rch;
                try
                {
                    rch = (from x in context.tbl_PurchaseOrderHeader.ToList()
                           where x.TransID == _Header.TransID
                           select x).FirstOrDefault();

                    rch.RecieveDate = _Header.RecieveDate;
                    rch.DelivaryDate = _Header.DelivaryDate;
                    rch.Vendor = _Header.Vendor;

                    foreach (var item in context.RouteCardDetails
                        .Where
                        (
                            x => x.Trans_TransactionHeader == rch.TransID
                        ).ToList())
                    {
                        context.DeleteObject(item);
                    }

                    foreach (var item in _Details)
                    {
                        context.tbl_PurchaseOrderDetails.AddObject(item);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeletePurchaseOrder(tbl_PurchaseOrderHeader _Header, List<tbl_PurchaseOrderDetails> _Details)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                tbl_PurchaseOrderHeader rch;
                try
                {
                    rch = (from x in context.tbl_PurchaseOrderHeader.ToList()
                           where x.TransID == _Header.TransID
                           select x).FirstOrDefault();

                    context.DeleteObject(rch);

                    foreach (var item in context.tbl_PurchaseOrderDetails
                        .Where
                        (
                            x => x.Trans_TransactionHeader == rch.TransID
                        ).ToList())
                    {
                        context.DeleteObject(item);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
      
        [OperationContract]
        public int GetMaxPurchaseOrderID(int RouteGroupID, int Direction)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    int x = context.tbl_PurchaseOrderHeader.Max(c => c.TransID);
                    return x;
                }
                catch
                {
                    return 0; ;
                }
            }
        }
    }
}