using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblShopReqHeader> GetTblShopReqHeader(int tblStore, int tblItemDownLoadDef, int week, int year, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = context.TblShopReqHeaders.Where(s => s.TblStore == tblStore && s.TblItemDownLoadDef == tblItemDownLoadDef
                    && s.Year == year && s.Week == week
                    );

                return query.ToList();
            }
        }

        [OperationContract]
        private TblShopReqHeader UpdateOrDeleteTblShopReqHeader(TblShopReqHeader newRow, bool save, int index, string company, out int outindex)
        {
            outindex = index;
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from s in context.TblShopReqHeaders
                              where s.TblStore == newRow.TblStore && s.TblItemDownLoadDef == newRow.TblItemDownLoadDef
            && s.Year == newRow.Year && s.Week == newRow.Week

                              select s).SingleOrDefault();
                if (oldRow == null)
                {
                    context.TblShopReqHeaders.AddObject(newRow);
                }
                else
                {
                    GenericUpdate(oldRow, newRow, context);
                }

                context.SaveChanges();
                return newRow;
            }
        }
    }
}