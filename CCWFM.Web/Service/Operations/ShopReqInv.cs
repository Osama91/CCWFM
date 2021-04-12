using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblShopReqInv> GetTblShopReqInv(int TblShopReqHeader, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = context.TblShopReqInvs.Include("TblColor1").Include("TblSize1").Where(s => s.TblShopReqHeader == TblShopReqHeader);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblShopReqInv UpdateOrDeleteTblShopReqInv(TblShopReqInv newRow, bool save, int index, string company, int userIserial, out int outindex)
        {
            outindex = index;
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var headerrow =
      context.TblShopReqHeaders.FirstOrDefault(
          x => x.TblItemDownLoadDef == newRow.TblShopReqHeader1.TblItemDownLoadDef &&
               x.TblStore == newRow.TblShopReqHeader1.TblStore &&
               x.Year == newRow.TblShopReqHeader1.Year && x.Week == newRow.TblShopReqHeader1.Week);

                if (headerrow == null)
                {
                    newRow.TblShopReqHeader1.CreationDate = DateTime.Now;
                    newRow.TblShopReqHeader1.UserIserial = userIserial;

                    context.TblShopReqInvs.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblShopReqInvs
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        newRow.TblShopReqHeader1 = null;
                        newRow.TblShopReqHeader = headerrow.Iserial;

                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        newRow.TblShopReqHeader1 = null;
                        newRow.TblShopReqHeader = headerrow.Iserial;
                        context.TblShopReqInvs.AddObject(newRow);
                    }
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblShopReqInv(int row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblShopReqInvs
                              where e.Iserial == row
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}