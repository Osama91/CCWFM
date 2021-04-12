using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System;

namespace CCWFM.Web.Service.Operations.PurchasePlan
{
    public partial class PurchasePlan
    {
        [OperationContract]
        private List<TblPurchaseOrderHeaderRequestPayment> GetTblPurchaseOrderHeaderRequestPayment(int Iserial)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblPurchaseOrderHeaderRequestPayment> query;

                query = entity.TblPurchaseOrderHeaderRequestPayments
                    .Where(w => w.TblPurchaseOrderHeaderRequest == Iserial);
                
                return query.ToList();
            }
        }

        [OperationContract]
        private double? GetTblPruchaseOrderTotal(int Iserial)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var total = entity.TblPurchaseOrderDetailRequests.Where(w => w.TblPurchaseOrderHeaderRequest == Iserial).Sum(w => w.Qty * w.Price);
                return total;
            }
        }

        [OperationContract]
        private List<TblPurchaseOrderHeaderRequestPayment> UpdateOrInsertTblPurchaseOrderHeaderRequestPayments(List<TblPurchaseOrderHeaderRequestPayment> ListToSave)
        {
            
            using (var entity = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in ListToSave)
                {
                    if (newRow.Iserial==0)
                    {
                        entity.TblPurchaseOrderHeaderRequestPayments.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in entity.TblPurchaseOrderHeaderRequestPayments
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            SharedOperation.GenericUpdate(oldRow, newRow, entity);
                        }
                    }

                }
              
                entity.SaveChanges();
                return ListToSave;
            }
        }

        [OperationContract]
        private int DeleteTblPurchaseOrderHeaderRequestPayment(TblPurchaseOrderHeaderRequestPayment row, int index)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblPurchaseOrderHeaderRequestPayments
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
        [OperationContract]
        public void GeneratePurchaseOrderHeaderRequestPayment(int TblPurchaseOrderHeaderRequest, DateTime startingDate, float percentage, float InstallmentCounts, float InstallmentInterval, float amount, string Description,int setting)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                //var AmountAfterFirstPayment = amount - (amount * percentage / 100);


                //var downPayment = new TblPurchaseOrderHeaderRequestPayment()
                //{
                //    TblPurchaseOrderHeaderRequest = TblPurchaseOrderHeaderRequest,
                //    Amount = Convert.ToDecimal((amount * percentage / 100)),
                //    DueDate = startingDate,
                //    Description = Description,
                //    Status = 0
                //};

                //entity.TblPurchaseOrderHeaderRequestPayments.AddObject(downPayment);


                for (int i = 0; i < InstallmentCounts; i++)
                {
                    var row = new TblPurchaseOrderHeaderRequestPayment()
                    {
                        TblPaymentScheduleSettings=setting,
                        TblPurchaseOrderHeaderRequest = TblPurchaseOrderHeaderRequest,
                        Amount = Convert.ToDecimal((amount* percentage / 100) / InstallmentCounts ),
                        DueDate = startingDate.AddDays(InstallmentInterval * (i)),
                        Description = Description,
                        Status = 0
                    };
                    entity.TblPurchaseOrderHeaderRequestPayments.AddObject(row);
                }
                entity.SaveChanges();
            }
        }
    }
}