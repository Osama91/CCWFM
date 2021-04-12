using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
//using _Model = CCWFM.Web.Model;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
    
        [OperationContract]
        private int DeleteTblStyleTNAColorDetail(TblStyleTNAColorDetail row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                
                var oldRow = (from e in context.TblStyleTNAColorDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStyleTNAColorDetail> GetTblStyleTNAColorDetail( int TblStyleTNAHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblStyleTNAColorDetail> query;
                    query = context.TblStyleTNAColorDetails.Include("TblColor")
                        .Where(x => x.TblStyleTNAHeader == TblStyleTNAHeader );
                return query.ToList();
            }
        }

     
        [OperationContract]
        private TblStyleTNAColorDetail UpdateOrInsertTblStyleTNAColorDetail(TblStyleTNAColorDetail newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (save)
                    {                     
                        context.TblStyleTNAColorDetails.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblStyleTNAColorDetails
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            SharedOperation.GenericUpdate(oldRow, newRow, context);
                           
                        }
                    }
                    context.SaveChanges();
                    return newRow ;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}