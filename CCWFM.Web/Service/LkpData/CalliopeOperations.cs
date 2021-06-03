using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using CCWFM.Web.Service.Operations;
using System;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
      [OperationContract]
      private TblStore GetUserDefaultStore(int user,string company, int tblStore)
      {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                //var ccnewUserStore = context.TblUserWFMappings.FirstOrDefault(x => x.WFTblAuthUser == user).TblUser1.ActiveStore;
                var activeStore = context.TblStores.FirstOrDefault(x => x.iserial == tblStore);
                return activeStore;
            }
      }

        [OperationContract]
        public tblCalliopeStoresDailySale SaveStoreDailySales(int user, string company, tblCalliopeStoresDailySale newrow)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                //if(newrow.Iserial == 0)
                //{
                    newrow.CreatedBy = user;
                    newrow.CreationDate = DateTime.Now;
                    context.tblCalliopeStoresDailySales.AddObject(newrow);
                //}
                //else
                //{
                //    var oldRow = context.tblCalliopeStoresDailySales.FirstOrDefault(x => x.Iserial == newrow.Iserial);
                //    oldRow.SalesAmount = newrow.SalesAmount;
                //    oldRow.LastUpdatedBy = user;
                //    oldRow.LastUpdatedDate = DateTime.Now;
                //    context.tblCalliopeStoresDailySales.AddObject(newrow);
                //}
                context.SaveChanges();
                var res = context.tblCalliopeStoresDailySales.FirstOrDefault(x => x.Iserial == newrow.Iserial);
                return res;
            }
        }


    }
}