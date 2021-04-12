using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthPermission> GetAllPermissions()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthPermissions.Where(x => x.PermissionTyp != "P").OrderBy(x => x.PermOrder).ToList();
            }
        }

        [OperationContract]
        public List<TblAuthPermission> GetUserSpectialPermissions()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthPermissions.Where(x => x.PermissionTyp == "P").OrderBy(x => x.PermOrder).ToList();
            }
        }

        [OperationContract]
        public void SaveJobPerm(List<TblAuthJobPermission> listToSave, List<TblAuthJobPermission> listToDelete, int tblJob)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newPerm in listToSave)
                {
                    var rowToSave =
                   context.TblAuthJobPermissions.FirstOrDefault(x => x.Tbljob == tblJob && x.TblPermission == newPerm.TblPermission);
                    if (rowToSave == null)
                    {
                        context.TblAuthJobPermissions.AddObject(newPerm);
                        context.SaveChanges();
                    }
                    else
                    {
                        GenerUpdate(rowToSave, newPerm, context);
                        context.SaveChanges();
                    }
                }

                foreach (var newPerm in listToDelete)
                {
                    var rowToSave =
                        context.TblAuthJobPermissions.FirstOrDefault(x => x.Tbljob == tblJob && x.TblPermission == newPerm.TblPermission);
                    if (rowToSave != null)
                    {
                        context.TblAuthJobPermissions.DeleteObject(rowToSave);
                        context.SaveChanges();
                    }
                }
            }
        }

        [OperationContract]
        public void CopyPermission(int Job)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var perm = context.TblAuthJobPermissions.Where(x => x.Tbljob == Job).ToList();
                var job = context.TblAuthJobs.FirstOrDefault(w => w.Iserial == Job);
                var newjob = new TblAuthJob()
                {
                    Aname = "Copy " + job.Aname,
                    Ename = "Copy " + job.Ename,
                    Code = "Copy " + job.Code,
                };
                context.TblAuthJobs.AddObject(newjob);
                context.SaveChanges();
                foreach (var item in perm)
                {
                    var newAuthJobPermission = new TblAuthJobPermission()
                    {
                        AllowDelete = item.AllowDelete,
                        AllowNew = item.AllowNew,
                        AllowUpdate = item.AllowUpdate,
                        Value = item.Value,
                        WFM_DOMAINID = item.WFM_DOMAINID,
                        TblPermission = item.TblPermission,
                        Tbljob = newjob.Iserial
                    };
                    context.TblAuthJobPermissions.AddObject(newAuthJobPermission);
                }
                context.SaveChanges();
            }
        }

    }
}