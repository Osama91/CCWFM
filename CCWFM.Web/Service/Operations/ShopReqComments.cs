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
        private List<TblShopReqComment> GetTblShopReqComments(int TblShopReqHeader, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = context.TblShopReqComments.Include("TblShopComment").Where(s => s.TblShopReqHeader == TblShopReqHeader);

                return query.ToList();
            }
        }

        [OperationContract]
        private TblShopReqComment UpdateOrDeleteTblShopReqComments(TblShopReqComment newRow, bool save, int index, string company, int userIserial, out int outindex)
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
                    context.TblShopReqComments.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblShopReqComments
                                  where e.TblShopComments == newRow.TblShopComments
                                  && e.TblShopReqHeader == headerrow.Iserial

                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        newRow.TblShopReqHeader1 = null;
                        newRow.TblShopReqHeader = headerrow.Iserial;
                        newRow.Iserial = oldRow.Iserial;
                        GenericUpdate(oldRow, newRow, context);
                    }
                    else
                    {
                        newRow.TblShopReqHeader1 = null;
                        newRow.TblShopReqHeader = headerrow.Iserial;
                        context.TblShopReqComments.AddObject(newRow);
                    }
                }

                context.SaveChanges();
                return newRow;
            }
        }
    }
}