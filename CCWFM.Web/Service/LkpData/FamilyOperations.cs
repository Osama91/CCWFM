using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
     
        [OperationContract]
        private List<TblFamily> GetTblFamily(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                IQueryable<TblFamily> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = context.TblFamilies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblFamilies.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblFamilies.Count();
                    query = context.TblFamilies.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblFamily UpdateOrInsertTblFamily(TblFamily newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblFamilies.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblFamilies
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblFamily(TblFamily row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblFamilies
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSubFamily> GetTblSubFamily(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSubFamily> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblFamily ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblSubFamilies.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSubFamilies.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSubFamilies.Count(v => v.TblFamily == groupId);
                    query = context.TblSubFamilies.OrderBy(sort).Where(v => v.TblFamily == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSubFamily UpdateOrInsertTblSubFamily(TblSubFamily newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblSubFamilies.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSubFamilies
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblSubFamily(TblSubFamily row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSubFamilies
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private bool CheckManualCodeing(string _brandCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var IsManualCoding = context.TblBrands.Where(x => x.Brand_Code == _brandCode).FirstOrDefault().ManualCode.Value;
                    return IsManualCoding;
                }
                catch
                {
                    return false;
                }
            }
        }

        [OperationContract]
        private bool CheckTNA(int _TblStyleSalesOrder,int tblUserJob)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var allowed = context.TblAuthJobPermissions.Include("TblAuthPermission").Where(x => x.TblAuthPermission.Code == "ApproveWithoutTNA" && x.Tbljob == tblUserJob).FirstOrDefault();
                if (allowed == null)
                {
                    var _tblSalesOrder = context.TblSalesOrders.Where(x => x.Iserial == _TblStyleSalesOrder).FirstOrDefault();
                    try
                    {
                        if (_tblSalesOrder != null)
                        {
                            var query = context.TblStyleTNAHeaders.Where(x => x.TblStyle == _tblSalesOrder.TblStyle).FirstOrDefault();
                            if (query != null)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }

}