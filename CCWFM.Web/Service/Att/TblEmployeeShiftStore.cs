using CCWFM.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace CCWFM.Web.Service.Att
{
    public partial class AttService
    {
        [OperationContract]
        private List<TblEmployeeShiftStore> GetTblEmployeeShiftStore(int store,string Company)
        {
            using (var context = new TimeAttEntities())
            {   
                                                      

                 var    query = context.TblEmployeeShiftStores.Include("TblEmployeeShiftLookup1").Where(w=>w.TblStore==store&& w.Company==Company).ToList();                
                return query;
            }
        }

        [OperationContract]
        private TblEmployeeShiftStore UpdateOrInsertTblEmployeeShiftStore(TblEmployeeShiftStore newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.TblEmployeeShiftStores.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblEmployeeShiftStores
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        Operations.SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblEmployeeShiftStore(TblEmployeeShiftStore row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblEmployeeShiftStores
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}