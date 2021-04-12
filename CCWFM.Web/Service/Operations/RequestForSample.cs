using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblRequestForSample> GetTblRequestForSample(int skip, int take, int style, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, out List<TBLsupplier> SupplierList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRequestForSample> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblStyle ==(@style0)";
                    valuesObjects.Add("style0", style);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblRequestForSamples.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblRequestForSamples.Include("TblColor1").Include("TblRequestForSampleStatu").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblRequestForSamples.Count(x => x.TblStyle == style);
                    query = context.TblRequestForSamples.Include("TblColor1").Include("TblRequestForSampleStatu").Where(x => x.TblStyle == style).OrderBy(sort).Skip(skip).Take(take);
                }

                var listOfSuppliers = query.Select(x => x.TblSupplier).Where(x => x > 0).Distinct().ToArray();
                using (var entity = new ccnewEntities())
                {
                    entity.TBLsuppliers.MergeOption = MergeOption.NoTracking;
                    SupplierList = listOfSuppliers.Any() ? entity.TBLsuppliers.Where(x => listOfSuppliers.Any(l => x.Iserial == l)).ToList() : null;
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblRequestForSample UpdateOrInsertTblRequestForSample(TblRequestForSample newRow, int UserIserial, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var serial = GetMaxSample(newRow.TblStyle, newRow.TblSupplier);
                    newRow.Code = newRow.Code + "-" + serial;
                    newRow.SerialNo = serial;
                    newRow.TblRequestForSampleStatus =
                   context.TblRequestForSampleStatus.FirstOrDefault(x => x.Code == "N/A").Iserial;
                    context.TblRequestForSamples.AddObject(newRow);
                    context.SaveChanges();
                    var newevent = new TblRequestForSampleEvent
                    {
                        ApprovedBy = UserIserial,
                        RequestDate = newRow.CreationDate,
                        DeliveryDate = newRow.EstimatedDeliveryDate,
                        TblRequestForSampleStatus = newRow.TblRequestForSampleStatus,
                        TblSalesOrder = newRow.Iserial
                    };
                    context.TblRequestForSampleEvents.AddObject(newevent);
                }
                else
                {
                    var oldRow = (from e in context.TblRequestForSamples
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        if (oldRow.TblSupplier != newRow.TblSupplier)
                        {
                            var serial = GetMaxSample(newRow.TblStyle, newRow.TblSupplier);
                            newRow.Code = newRow.Code + "-" + serial;
                            newRow.SerialNo = serial;
                        }
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        private string GetMaxSample(int tblStyle, int tblSupplier)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                string serialNoString = "";
                try
                {
                    int? serialNo =
             context.TblRequestForSamples.Where(
                 x => x.TblStyle == tblStyle && x.TblSupplier == tblSupplier)
                 .Select(x => x.SerialNo).Cast<int?>().Max();

                    if (serialNo != null)
                    {
                        serialNo++;

                        var serial = (int)serialNo;
                        serialNoString = serial.ToString("000");
                    }
                    else
                    {
                        serialNoString = "001";
                    }
                }
                catch (Exception)
                {
                    serialNoString = "001";
                }

                return serialNoString;
            }
        }

        [OperationContract]
        private int DeleteTblRequestForSample(TblRequestForSample row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRequestForSamples
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblRequestForSampleStatu> GetTblRequestForSampleStatus()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblRequestForSampleStatu> query = context.TblRequestForSampleStatus;

                return query.ToList();
            }
        }
    }
}