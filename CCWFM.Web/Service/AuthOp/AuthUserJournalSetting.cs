using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace CCWFM.Web.Service.AuthOp
{
    public partial class AuthService
    {
        //[OperationContract]
        //public List<TblAuthUserJournalSetting> GetAuthUserJournalSetting(string company)
        //{
        //    using (var entity = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
        //    {
        //        return entity.TblAuthUserJournalSettings.ToList();
        //    }
        //}
        [OperationContract]
        public List<Models.Inv.AuthWarehouseModel> GetAuthJournalSetting(int userIserial,string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                return context.TblJournalSettings.Select(w =>
                    new Models.Inv.AuthWarehouseModel()
                    {
                        WarehouseIserial = w.Iserial,
                        WarehoseEname = w.Ename,
                        WarehouseCode = w.Aname,
                        IsGranted = context.TblAuthUserJournalSettings.Any(aw =>
                            aw.TblAuthUser == userIserial &&                                
                            aw.TblJournalSetting == w.Iserial)
                    }).ToList();
            }
        }

        [OperationContract]
        public void SaveAuthJournalSetting(int userIserial, string company, 
            List<Models.Inv.AuthWarehouseModel> authList)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {                
                var curruntAuthList = context.TblAuthUserJournalSettings.Where(aw =>
                       aw.TblAuthUser == userIserial).ToList();
                foreach (var item in curruntAuthList)
                {
                    context.DeleteObject(item);
                }
                var itemsToAdd = authList.Where(a => a.IsGranted);// &&         
                foreach (var item in itemsToAdd)
                {
                    context.TblAuthUserJournalSettings.AddObject(new TblAuthUserJournalSetting()
                    {
                        TblJournalSetting = item.WarehouseIserial,
                        TblAuthUser = userIserial,                        
                    });
                }
                context.SaveChanges();
            }
        }
    }
}