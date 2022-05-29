using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Dto_s;
using System;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private TblApproval UpdateOrInsertTblApproval(TblApproval newRow, int index, int perm, out int outindex, out decimal? tempprice, out float? tempCost)
        {
            tempprice = 0;
            tempCost = 0;
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                //   context.Connection.ConnectionTimeout = 0;
                var oldRow = (from e in context.TblApprovals
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblApprovals.AddObject(newRow);
                }
                var salesorder = context.TblSalesOrders.Include("TblStyle1.TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1").Include("BOMs").FirstOrDefault(x => x.Iserial == newRow.TblSalesOrder);

                

                if (newRow.ApprovedStatus == (int)Enums.ApprovalStatus.Approved)
                {
                    if (salesorder.SalesOrderType == (int)(Enums.SalesOrderType.SalesOrderPo)) {

                        var StyleBrand = context.TblBrands.FirstOrDefault(wde => wde.Brand_Code == salesorder.TblStyle1.Brand);
                        if (!StyleBrand.CanCreateManualProductionOrder)
                        {
                            ContractColorsCheck(context, salesorder);
                        }
                    }



                    switch (newRow.ApprovalType)
                    {
                        case 1:
                            salesorder.IsRetailApproved = newRow.ApprovedStatus == 1;
                            break;

                        case 2:
                            salesorder.IsFinancialApproved = newRow.ApprovedStatus == 1;
                            break;

                        case 3:
                            salesorder.IsTechnicalApproved = newRow.ApprovedStatus == 1;
                            break;
                    }
                    var premissions = salesorder.TblStyle1.TblLkpBrandSection1.TblBrandSectionPermissions.FirstOrDefault(x => x.TblAuthPermission == perm && x.BrandCode == salesorder.TblStyle1.Brand);
                    if (salesorder.IsRetailApproved == premissions.Retail && salesorder.IsFinancialApproved == premissions.Financial && salesorder.IsTechnicalApproved == premissions.Technical)
                    {
                        if (salesorder.TblStyle1.tbl_FabricAttriputes == 0)
                        {
                            salesorder.TblStyle1.tbl_FabricAttriputes = 270;
                        }
                        context.SaveChanges();
                        if ((int)Enums.SalesOrderType.AdvancedSampleRequest != salesorder.SalesOrderType)
                        {
                            var postPo = (int)Enums.SalesOrderType.RetailPo == salesorder.SalesOrderType;

                            if ((int)Enums.SalesOrderType.SalesOrderPo == salesorder.SalesOrderType&& (salesorder.TblStyle1.Brand== "20" || salesorder.TblStyle1.Brand == "GN"))
                            {
                                PostStyleToPo(salesorder.TblStyle, salesorder.Iserial, postPo, newRow.TblAuthUser, out tempprice, out tempCost);
                            }
                            else if ((int)Enums.SalesOrderType.RetailPo == salesorder.SalesOrderType)
                            {
                                PostStyleToPo(salesorder.TblStyle, salesorder.Iserial, postPo, newRow.TblAuthUser, out tempprice, out tempCost);
                            }               
                          
                         
                            salesorder.Status = (int)Enums.ApprovalStatus.Approved;

                            if ((int)Enums.SalesOrderType.SalesOrderPo == salesorder.SalesOrderType)
                            {
                                PostBomToAx(salesorder, newRow.TblAuthUser);
                                // var IsSample= isSample(salesorder.SalesOrderCode);

                                CreateEstimatedBom(salesorder);
                            }
                        }
                    }
                }
                else if (newRow.ApprovedStatus == (int)Enums.ApprovalStatus.Canceled)
                {
                    salesorder.Status = (int)Enums.ApprovalStatus.Canceled;
                }
                else if (newRow.ApprovedStatus == (int)Enums.ApprovalStatus.FinalCost)
                {
                    salesorder.Status = (int)Enums.ApprovalStatus.FinalCost;
                }
                context.SaveChanges();
                newRow.TblSalesOrder1 = salesorder;
                return newRow;
            }
        }

        private bool ContractColorsCheck(WorkFlowManagerDBEntities context, TblSalesOrder salesorder)
        {
            var salesOrderColorsList = context.TblSalesOrderColors.Where(w => w.TblSalesOrder == salesorder.Iserial).ToList();
            var salesOrderColors = salesOrderColorsList.Select(w => w.TblColor);
          //  var salesOrderColorIserials = salesOrderColorsList.Select(w => w.Iserial);

            int salesOrderType = (int)Enums.SalesOrderType.RetailPo;
            var approvedColors = context.TblSalesOrderColors.Include("TblSalesOrder1").Where(wde => wde.TblSalesOrder1.TblStyle == salesorder.TblStyle && wde.TblSalesOrder1.SalesOrderType == salesOrderType).Select(w => w.TblColor);
            var ContractColors = context.TblContractDetails
                .Include("TblContractSubHeader1").Where(wde => salesOrderColors.Contains(wde.TblSalesOrderColor1.TblColor)&&     wde.TblContractSubHeader1.Approved==true  &&wde.TblContractSubHeader1.TblContractSubHeaderStatus == 2 && wde.Status!=2).Select(w => w.TblColor);


            string errormsg = "";
            if (!salesOrderColors.All(s => approvedColors.Contains(s)))
            {
                foreach (var item in salesOrderColors)
                {
                    if (!approvedColors.Contains(item))
                    {
                        var color = context.TblColors.FirstOrDefault(wde => wde.Iserial == item);
                        errormsg = errormsg + " " + color.Code;
                    }
                }

                if (errormsg != "")
                {
                    errormsg = errormsg + " these colors doesn't have an approved contract";
                    throw new Exception(errormsg);  
                }
            }

            if (!salesOrderColors.All(s => ContractColors.Contains(s)))
            {
                foreach (var item in salesOrderColors)
                {
                    if (!ContractColors.Contains(item))
                    {
                        var color = context.TblColors.FirstOrDefault(wde => wde.Iserial == item);
                        errormsg = errormsg + " " + color.Code;
                    }
                }

                if (errormsg != "")
                {
                    errormsg = errormsg + " these colors doesn't have an approved contract";
                    throw new Exception(errormsg);
                }
            }


            return true;

        }

        [OperationContract]
        private int DeleteTblApproval(TblApproval row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblApprovals
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);
                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblApproval> GetTblApproval(int tblSalesOrder)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var salesorder = context.TblSalesOrders.FirstOrDefault(x => x.Iserial == tblSalesOrder);
                return context.TblApprovals.Include("TblApprovalType").Include("TblAuthUser1").Where(x => x.TblSalesOrder == tblSalesOrder).ToList();
            }
        }
    }
}