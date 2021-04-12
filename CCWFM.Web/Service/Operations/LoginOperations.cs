using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Web;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public TblAuthUser Authenticate(string _USerName, string _USerPassrword, out TblStore store)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                var authQuery = context.TblAuthUsers.Include("TblCompany1").Include("TblCompany2").Include("BarcodeDisplaySettingsHeader").Include("TblUserBrandSections.TblUserBrandSectionPermissions").FirstOrDefault(x => x.UserName.ToLower() == _USerName && x.UserPassword == _USerPassrword);
                if (authQuery != null)
                {
                    Main.Iserial = authQuery.Iserial;
                    Main.UserWinLogin = authQuery.User_Win_Login;
                    Main.Domain = authQuery.User_Domain;

                    if (authQuery.TblCompany1 != null && authQuery.TblCompany1.Ip != null)
                    {
                        if (authQuery.TblCompany1.Ip != null) Main.Ip = authQuery.TblCompany1.Ip;
                        if (authQuery.TblCompany1.Port != null) Main.Port = authQuery.TblCompany1.Port;
                        if (authQuery.TblCompany1.DbName != null) Main.DatabaseName = authQuery.TblCompany1.DbName;
                        if (authQuery.Code != null) Main.Code = authQuery.Code;
                        if (authQuery.TblCompany1.Code != "HQ")
                        {
                            using (var db = new ccnewEntities())
                            {
                                if (authQuery.ActiveStore == null || authQuery.ActiveStore == 0)
                                {
                                    store = null;
                                }
                                else
                                {
                                    var user = Convert.ToInt32(authQuery.ActiveStore);

                                    store = db.TblStores.FirstOrDefault(x => x.iserial == user);
                                }
                            }
                        }
                    }
                }
                store = null;
                return authQuery;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int GetItemsPermissions(string _ItemName)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return (from z in context.TblAuthPermissions
                        where z.Code == _ItemName
                        select z.Iserial).FirstOrDefault();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthPermission> GetItemsPermissionsByParent(string _ParentCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblAuthPermissions
                             where x.ParentPerm == (from z in context.TblAuthPermissions
                                                    where z.Code == _ParentCode
                                                    select z.Iserial).FirstOrDefault()
                             select x).ToList();

                foreach (var tblAuthPermission in query.ToList())
                {
                    var subquery = (from x in context.TblAuthPermissions
                                    where x.ParentPerm == tblAuthPermission.Iserial
                                    select x).ToList();

                    query.AddRange(subquery);

                    foreach (var authPermission in subquery.ToList())
                    {
                        var subQuery = (from x in context.TblAuthPermissions
                                        where x.ParentPerm == authPermission.Iserial
                                        select x).ToList();
                        query.AddRange(subQuery);

                        //authPermission.
                    }
                }

                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int GetUserJob(int _UserID, string _UserName)
        {
            return Operations.SharedOperation.GetUserJob(_UserID, _UserName);
        }

        public bool GetExistPermByUser(int authuser, string perm)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var job = GetUserJob(authuser, "");
                return (from x in context.TblAuthJobPermissions
                        where x.Tbljob == job && x.TblAuthPermission.Code == perm
                        select x).Any();
            }

        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public ObservableCollection<TblAuthJobPermission> GetUserJobPermissions(int userJob)
        {
            ObservableCollection<TblAuthJobPermission> returnResult;
            using (var context = new WorkFlowManagerDBEntities())
            {
                returnResult = new ObservableCollection<TblAuthJobPermission>
                    (
                    (from x in context.TblAuthJobPermissions
                     where x.Tbljob == userJob
                     select x).ToList());
            }
            return returnResult;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public ObservableCollection<TblAuthPermission> GetUserMenuesPermissions(int userJob)
        {
            ObservableCollection<TblAuthPermission> returnResult;
            using (var context = new WorkFlowManagerDBEntities())
            {
                returnResult = new ObservableCollection<TblAuthPermission>
                    (
                    (from z in context.TblAuthPermissions
                     where (z.PermissionTyp.ToUpper() == "M" || z.PermissionTyp.ToUpper() == "F" || z.PermissionTyp.ToUpper() == "R")
                     && (from x in context.TblAuthJobPermissions
                         where x.Tbljob == userJob
                         select x.TblPermission).Contains(z.Iserial)
                     select z).ToList());
            }
            return returnResult;
        }
    }
}