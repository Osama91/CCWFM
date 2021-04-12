using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private int DeleteSml(TblSeasonalMasterList row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSeasonalMasterLists
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);




                //Delete from [TblTechPackBOMStyleColor]
                try
                {
                    var oldTechPackBOMStyleColorRow = (from e in context.TblTechPackBOMStyleColors
                                                       where e.TechPackBOM1.TblStyle == row.TblStyle && e.StyleColor == row.TblColor
                                                       select e).ToList();

                    foreach (var item in oldTechPackBOMStyleColorRow)
                    {
                        context.DeleteObject(item);
                    }
                } catch { }
              

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblSeasonalMasterList> GetSml(int skip, int take, int tblStyle, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSeasonalMasterList> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblStyle ==(@style0)";
                    valuesObjects.Add("style0", tblStyle);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSeasonalMasterLists
                        .Where(filter, parameterCollection.ToArray()).Count();

                    query = context.TblSeasonalMasterLists.Include("TblColor1").Include("TblSeasonalMasterListDetails")
                        .Include("TblStyle1").Include("TblStyle1.TblLkpBrandSection1")
                        .Include("TblStyle1.TblFamily1")
                        .Include("TblStyle1.TblSubFamily1")
                        .Include("TblStyle1.TblSizeGroup1")
                        .Include("TblStyle1.TblLkpSeason1")
                        .Include("TblStyle1.TblLkpSeason1.TblSeasonTracks.TblSeasonTrackType1")
                        .OrderBy(sort).Where(filter, parameterCollection.ToArray());
                }
                else
                {
                    fullCount = context.TblSeasonalMasterLists.Count(x => x.TblStyle == tblStyle);
                    query = context.TblSeasonalMasterLists.Include("TblColor1").Include("TblSeasonalMasterListDetails").Include("TblStyle1").Include("TblStyle1.TblLkpBrandSection1").Include("TblStyle1.TblFamily1").Include("TblStyle1.TblSubFamily1").Include("TblStyle1.TblSizeGroup1").Include("TblStyle1.TblLkpSeason1").Include("TblStyle1.TblLkpSeason1.TblSeasonTracks.TblSeasonTrackType1")
                        .OrderBy(sort).Where(x => x.TblStyle == tblStyle).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private TblSeasonalMasterList UpdateOrInsertTblSml(TblSeasonalMasterList newRow, bool save, int index, out int outindex, out double QtyExceeded, out double AmountExceeded, out double QtyAvaliable, out double AmountAvaliable)
        {
            QtyExceeded = 0;
            AmountExceeded = 0;
            AmountAvaliable = 0;
            QtyAvaliable = 0;
            outindex = index;
            //  int? qty = 0;
            //  double? totalCost = 0;          
            using (var context = new WorkFlowManagerDBEntities())
            {
                var style = context.TblStyles.FirstOrDefault(w => w.Iserial == newRow.TblStyle);             
                if (save)
                {
                    context.TblSeasonalMasterLists.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSeasonalMasterLists
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                        foreach (var row in newRow.TblSeasonalMasterListDetails.ToList())
                        {
                            var oldColorRow = (from e in context.TblSeasonalMasterListDetails
                                               where e.Iserial == row.Iserial 
                                               select e).SingleOrDefault();
                            if (oldColorRow != null)
                            {
                                row.TblSeasonalMasterList = newRow.Iserial;

                                row.TblSeasonalMasterList1 = null;
                                GenericUpdate(oldColorRow, row, context);
                            }
                            else
                            {
                                row.TblSeasonalMasterList1 = null;

                                row.TblSeasonalMasterList = newRow.Iserial;
                                context.TblSeasonalMasterListDetails.AddObject(row);
                            }
                        }
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStoreIntialOrder> GetTblStoreIntialOrder(int skip, int take, string tblStyle, string color, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<TblStore> Stores)
        {
            using (var context = new ccnewEntities())
            {
                IQueryable<TblStoreIntialOrder> query;
                if (filter != null)
                {
                    filter = filter + " and it.Style ==(@style0)";
                    valuesObjects.Add("style0", tblStyle);

                    filter = filter + " and it.Color ==(@Color0)";
                    valuesObjects.Add("Color0", color);

                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblStoreIntialOrders
                        .Where(filter, parameterCollection.ToArray()).Count();

                    query = context.TblStoreIntialOrders
                        .OrderBy(sort).Where(filter, parameterCollection.ToArray());
                }
                else
                {
                    fullCount = context.TblStoreIntialOrders.Count(x => x.Style == tblStyle && x.Color == color);
                    query = context.TblStoreIntialOrders
                        .OrderBy(sort).Where(x => x.Style == tblStyle && x.Color == color).Skip(skip).Take(take);
                }
                var listOfStores = query.Select(x => x.Store).ToList();
                Stores =
               context.TblStores.Where(x => listOfStores.Any(l => x.iserial == l)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblStoreIntialOrder UpdateOrInsertTblStoreIntialOrder(TblStoreIntialOrder newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new ccnewEntities())
            {
                if (save)
                {
                    context.TblStoreIntialOrders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblStoreIntialOrders
                                  where e.Style == newRow.Style
                                  && e.Color == newRow.Color
                                  && e.Store == newRow.Store
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblStoreIntialOrder DeleteTblStoreIntialOrder(TblStoreIntialOrder row)
        {
            using (var context = new ccnewEntities())
            {
                var oldRow = (from e in context.TblStoreIntialOrders
                              where e.Style == row.Style
                              && e.Color == row.Color
                              && e.Store == row.Store
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}