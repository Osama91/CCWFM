using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<styleApproval> GetStyleApp(string user)
        {
            using (WorkFlowManagerDBEntities entities = new WorkFlowManagerDBEntities())
            {
                string UserId = (from u in entities.tbl_Auth_User
                                 where u.UserName == user
                                 select u.AxId).SingleOrDefault();

                List<styleApproval> StyleApproval = (from h in entities.styleApprovals
                                                     where h.Employee_ID == UserId
                                                     select h).ToList();

                return StyleApproval;
            }
        }
    }
}