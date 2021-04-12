using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using CCWFM.Web.Model;

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
                return _Skip != null ? context.tbl_PurchaseOrderHeader.Skip((int)_Skip).Take(_Take ?? 0).ToList()
                    : context.tbl_PurchaseOrderHeader.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<SizesWithGroups> GetStyleSizesFromRetail(List<string> styleCodes)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.v_RetailItemsWithSizeGroup.Where(x => styleCodes.Contains(x.Style))
                                    .Select(x => new { x.SizeCode, x.SizeGroupCode, x.Style })
                                    .Distinct().ToList();
                return temp.Select(sg => new SizesWithGroups { SizeCode = sg.SizeCode, SizeGroup = sg.SizeGroupCode, StyleCode = sg.Style }).ToList();
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
            using (var context = new WorkFlowManagerDBEntities())
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
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var rch = (from x in context.tbl_PurchaseOrderHeader.ToList()
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
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var x = context.tbl_PurchaseOrderHeader.Max(c => c.TransID);
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