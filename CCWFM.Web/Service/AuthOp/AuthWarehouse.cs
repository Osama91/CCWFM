using CCWFM.Web.Model;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace CCWFM.Web.Service.AuthOp
{
    public partial class AuthService
    {
        [OperationContract]
        public List<TblAuthWarehousePermissionType> GetLookUpAuthWarehouseTypes()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthWarehousePermissionTypes.ToList();
            }
        }
        [OperationContract]
        public List<Models.Inv.AuthWarehouseModel> GetAuthWarehouses(int userIserial,short PermissionIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblWarehouses.Select(w =>
                    new Models.Inv.AuthWarehouseModel()
                    {
                        WarehouseIserial = w.Iserial,
                        WarehoseEname = w.Ename,
                        WarehouseCode = w.Code,
                        IsGranted = context.TblAuthWarehouses.Any(aw =>
                            aw.AuthUserIserial == userIserial &&
                            aw.PermissionType == PermissionIserial &&
                            aw.WarehouseIserial == w.Iserial)
                    }).ToList();
            }
        }

        [OperationContract]
        public void SaveAuthWarehouses(int userIserial, short PermissionIserial, 
            List<Models.Inv.AuthWarehouseModel> authList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                // Add Items
                var curruntAuthList = context.TblAuthWarehouses.Where(aw =>
                       aw.AuthUserIserial == userIserial &&
                       aw.PermissionType == PermissionIserial).ToList();
                foreach (var item in curruntAuthList)
                {
                    context.DeleteObject(item);
                }
                var itemsToAdd = authList.Where(a => a.IsGranted);// &&         
                foreach (var item in itemsToAdd)
                {
                    context.TblAuthWarehouses.AddObject(new TblAuthWarehouse()
                    {
                        WarehouseIserial = item.WarehouseIserial,
                        AuthUserIserial = userIserial,
                        PermissionType = PermissionIserial
                    });
                }
                context.SaveChanges();
            }
        }
    }
}