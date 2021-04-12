using System.Collections.Generic;
using System.Linq;
using CCWFM.Web.Model;

namespace CCWFM.Web.Classes
{
    public static class LoggedUserInfo
    {
        public static string WfmUserName { get; set; }

        public static int WfmUserJob { get; private set; }


        private static List<TblAuthJobPermission> WfmUserJobPermissions { get; set; }

        public static void InitiatePermissions()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                WfmUserJob = (from x in context.TblAuthJobs.ToList()
                              where x.Iserial == (from z in context.TblAuthUsers.ToList()
                                                  where z.UserName.ToLower() == WfmUserName.ToLower()
                                                  select z.TblJob).FirstOrDefault()
                              select x.Iserial).FirstOrDefault();

                WfmUserJobPermissions = (from x in context.TblAuthJobPermissions.ToList()
                                         where x.Tbljob == WfmUserJob
                                         select x).ToList();
            }
        }

        public static TblAuthJobPermission GetItemPermissions(string itemName)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int permissions = (from z in context.TblAuthPermissions.ToList()
                                   where z.Code == itemName
                                   select z.Iserial).SingleOrDefault();

                return WfmUserJobPermissions.SingleOrDefault(x => x.TblPermission == permissions);
            }
        }
    }
}