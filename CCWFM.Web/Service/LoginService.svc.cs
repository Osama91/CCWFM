using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using CCWFM.Web.Service.Operations;
using System.DirectoryServices.AccountManagement;
using System.ServiceModel.Activation;

namespace CCWFM.Web.Service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]

    public class LoginService
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

        #region WindowsAuthontication

        private bool GetUserInformation(string _USerName, string _USerPassrword, string _DomainAddress)
        {
            SearchResult rs = null;
            rs = SearchUserByUserName(GetDirectorySearcher(_USerName, _USerPassrword, _DomainAddress), _USerName);
            if (rs != null)
            { return true; }
            else
                return false;
        }
        private SearchResult SearchUserByUserName(DirectorySearcher ds, string username)
        {
            ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))";
            ds.SearchScope = SearchScope.Subtree;
            ds.ServerTimeLimit = TimeSpan.FromSeconds(90);
            SearchResult userObject = ds.FindOne();
            if (userObject != null)
                return userObject;
            else
                return null;
        }
        private DirectorySearcher GetDirectorySearcher(string username, string passowrd, string domain)
        {
            DirectorySearcher dirSearch = new DirectorySearcher();
            try
            {
                dirSearch = new DirectorySearcher(
                            new DirectoryEntry("LDAP://" + domain, username, passowrd));
            }
            catch (DirectoryServicesCOMException e)
            {
                e.Message.ToString();
            }
            return dirSearch;
        }

        #endregion

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthUser> GetAllLoginUsersData()
        {
            var authQuery = new List<TblAuthUser>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                authQuery = context.TblAuthUsers.Include("TblCompany1").Include("TblCompany2")
                   .Include("BarcodeDisplaySettingsHeader").
                   Include("TblUserBrandSections.TblUserBrandSectionPermissions").ToList();
            }
            return authQuery;
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
                    (
                    
                    from x in context.TblAuthJobPermissions
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

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthUser> GetAllLoginUsers()
        {
            List<TblAuthUser> returnResult;
            using (var context = new WorkFlowManagerDBEntities())
            {
                returnResult = context.TblAuthUsers.Include("TblCompany1").Include("TblCompany2")
                    .Include("BarcodeDisplaySettingsHeader")
                    .Include("TblUserBrandSections.TblUserBrandSectionPermissions").ToList();

            }
            return returnResult;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int SaveNewUserRequest(string _NewUserID, string _NewUserCompanyID, string _LikeUserID, string _LikeUserCompanyID, string _IsSalesPerson, string _IsRetailPerson, string _UserName, string _Comment, string _UserCreateRequestID)
        {

            TblAddUserRequest newRow = new TblAddUserRequest();
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (_UserCreateRequestID != "0")
                {
                    newRow.NewUserID = _NewUserID;
                    newRow.NewUserCompanyID = _NewUserCompanyID;
                    newRow.LikeUserID = _LikeUserID;
                    newRow.LikeUserCompanyID = _LikeUserCompanyID;
                    newRow.UserCreateRequestID = _UserCreateRequestID;
                    newRow.IsSalesPerson = _IsSalesPerson;
                    newRow.IsRetailPerson = _IsRetailPerson;
                    newRow.UserName = _UserName;
                    newRow.UserPassword = "654321";
                    newRow.Comment = _Comment;
                    newRow.Approved = "0";
                    newRow.Rejected = "0";
                    newRow.RequestDate = DateTime.Now;
                    context.TblAddUserRequests.AddObject(newRow);
                }

                return context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<NewUserRequests> GetAllNewUsersRequest()
        {
            List<NewUserRequests> returnResult = new List<NewUserRequests>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                List<TblAddUserRequest> Requests = context.TblAddUserRequests.Where(x => x.Approved == "0" && x.Rejected == "0").ToList();
                foreach (var item in Requests)
                {
                    try
                    {
                        NewUserRequests r = new NewUserRequests();

                        //User Create the Request
                        r.UserCreateRequestID = item.UserCreateRequestID;
                        int UserCreateRequestID = int.Parse(r.UserCreateRequestID);

                        r.UserCreateRequestName = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == UserCreateRequestID).Ename.ToString();

                        //New User Data
                        r.NewUserID = item.NewUserID;
                        r.UserName = item.UserName;
                        r.NewUserCompanyID = item.NewUserCompanyID;
                        int _NewUserCompanyID = int.Parse(r.NewUserCompanyID);
                        r.NewUserCompanyName = context.TblCompanies.FirstOrDefault(x => x.Iserial == _NewUserCompanyID).Code;

                        //Like User Data
                        r.LikeUserID = item.LikeUserID;
                        r.LikeUserName = context.Employees.FirstOrDefault(x => x.EMPLID == r.LikeUserID).name;
                        r.LikeUserCompanyID = item.NewUserCompanyID;
                        int _LikeUserCompanyID = int.Parse(r.LikeUserCompanyID);
                        r.LikeUserCompanyName = context.TblCompanies.FirstOrDefault(x => x.Iserial == _LikeUserCompanyID).Code;
                        r.Comment = item.Comment;
                        r.RequestDate = item.RequestDate.Value;

                        r.IsSalesPerson = item.IsSalesPerson;
                        returnResult.Add(r);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }

            return returnResult;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int UpdateNewUsersRequest(NewUserRequests UserRequest)
        {

            int Result = 0;
            string dbName = "";

            TblAuthUser LikeUser = new TblAuthUser();

            using (var context = new WorkFlowManagerDBEntities())
            {
                var currentUserRequest = context.TblAddUserRequests.FirstOrDefault
                    (x => x.UserCreateRequestID == UserRequest.UserCreateRequestID
                    && x.NewUserID == UserRequest.NewUserID & x.NewUserCompanyID == UserRequest.NewUserCompanyID);

                currentUserRequest.Approved = UserRequest.Approved;
                currentUserRequest.Rejected = UserRequest.Rejected;
                currentUserRequest.ApproveDate = DateTime.Now;

                /*************************************Approved User*********************************/
                if (currentUserRequest.Approved == "1")
                {
                    LikeUser = context.TblAuthUsers.FirstOrDefault(x => x.PayrollCode == UserRequest.LikeUserID);

                    int _NewUserCompanyID = int.Parse(UserRequest.NewUserCompanyID);
                    dbName = context.TblCompanies.FirstOrDefault(x => x.Iserial == _NewUserCompanyID).DbName;

                    //Create New User Login

                    TblAuthUser CreateNewUser = new TblAuthUser();
                    CreateNewUser.UserName = currentUserRequest.UserName;
                    CreateNewUser.UserPassword = currentUserRequest.UserPassword;
                    CreateNewUser.Code = string.Format(@"{0}{1}", currentUserRequest.NewUserID, currentUserRequest.NewUserCompanyID);
                    CreateNewUser.TblCompany = int.Parse(currentUserRequest.NewUserCompanyID);
                    CreateNewUser.PayrollCode = currentUserRequest.NewUserID;
                    CreateNewUser.Ename = currentUserRequest.UserName;
                    CreateNewUser.Aname = currentUserRequest.UserName;
                    CreateNewUser.TblJob = LikeUser.TblJob;
                    CreateNewUser.CurrLang = LikeUser.CurrLang;
                    context.TblAuthUsers.AddObject(CreateNewUser);
                }

                context.SaveChanges();
            }

            if (UserRequest.Approved == "1" && UserRequest.IsSalesPerson == "1")
            {
                Operations.GlOperations.GlService service = new Operations.GlOperations.GlService();
                using (var newContext = new ccnewEntities(service.GetSqlConnectionString(dbName)))
                {
                    TBLSalesPerson newRow = new TBLSalesPerson();

                    int NextSerial = newContext.TBLSalesPersons.Select(x => x.ISerial).Max();
                    newRow.ISerial = NextSerial + 1;
                    newRow.Ename = UserRequest.UserName;
                    newRow.Aname = UserRequest.UserName;
                    newRow.Code = UserRequest.NewUserID;
                    newContext.TBLSalesPersons.AddObject(newRow);
                    if (UserRequest.IsRetailPerson == "1")
                    {
                        TblUser newUser = new TblUser();
                        TblUser oldUser = newContext.TblUsers.FirstOrDefault(x => x.Code == UserRequest.LikeUserID);

                        try
                        {
                            int NextUserSerial = newContext.TblUsers.Select(x => x.iserial).Max();
                            newUser.iserial = NextUserSerial + 1;
                            newUser.Ename = UserRequest.UserName;
                            newUser.Aname = UserRequest.UserName;
                            newUser.Code = UserRequest.NewUserID;
                            newUser.UserPassword = UserRequest.UserPassword;
                            newUser.TblJob = oldUser.TblJob;
                            newUser.Active = oldUser.Active;
                            newUser.CurrLang = oldUser.CurrLang;
                            newUser.ReportFont = oldUser.ReportFont;
                            newUser.ActiveCashMach = oldUser.ActiveCashMach;
                            newUser.ActiveStore = oldUser.ActiveStore;
                            newUser.RegNo = oldUser.RegNo;
                            newUser.AllowedStores = oldUser.AllowedStores;
                            newUser.AllowedStoresTo = oldUser.AllowedStoresTo;
                            newContext.TblUsers.AddObject(newUser);
                        }
                        catch { }
                    }
                    newContext.SaveChanges();
                }

                AddUserToActiveDirectory(UserRequest);
            }

            return Result;
        }

        private void AddUserToActiveDirectory(NewUserRequests UserRequest)
        {
            try
            {
                string AdminUserName = "";
                string AdminPwd = "";
                string NewUserNameOnDomain = UserRequest.UserName;
                string NewUserNamePwdOnDomain = UserRequest.UserPassword;
                string DomainUrl = "CN=Users,DC=ccasual,DC=loc";
                string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                PrincipalContext PrincipalContext4 = new PrincipalContext(ContextType.Domain,
                                                                            stringDomainName,
                                                                            DomainUrl,
                                                                           ContextOptions.SimpleBind,
                                                                            AdminUserName,
                                                                            AdminPwd);

                UserPrincipal UserPrincipal1 = new UserPrincipal(PrincipalContext4,
                                                                 NewUserNameOnDomain,
                                                                 NewUserNamePwdOnDomain, true);

                //UserPrincipal1.UserPrincipalName = textboxSamAccountName.Text;
                //UserPrincipal1.Name = textboxName.Text;
                //UserPrincipal1.GivenName = textboxGivenName.Text;
                //UserPrincipal1.Surname = textboxSurname.Text;
                //UserPrincipal1.DisplayName = textboxDisplayName.Text;
                //UserPrincipal1.Description = textboxDescription.Text;

                UserPrincipal1.Enabled = true;
                UserPrincipal1.Save();
            }
            catch { }
        }

        [OperationContract]
        public string AuthenticateLoginExpiration(string _USerName, string _USerPassrword, string _UserCompany)
        {
            string Result = "0";
            using (var context = new WorkFlowManagerDBEntities())
            {
                int Users;
                //Get User Company
                if (string.IsNullOrEmpty(_UserCompany))
                {
                    Users = context.TblAuthUsers.Where(x => x.UserName == _USerName && x.UserPassword == _USerPassrword).Count();
                }
                else
                {
                    int CompanyId = 0;
                    try
                    {
                        CompanyId = context.TblCompanies.FirstOrDefault(x => x.Code.ToLower() == _UserCompany.ToLower()).Iserial;
                    }
                    catch
                    {
                        return "InvalidComapny";
                    }

                    Users = context.TblAuthUsers.Where(x => x.UserName == _USerName && x.UserPassword == _USerPassrword && x.TblCompany == CompanyId).Count();
                }
                if (Users <= 1)
                {
                    string UserLoginExpiredPeriod = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode.ToLower() == "UserLoginExpiredPeriod")
                        .sSetupValue.ToString();
                    //var authQuery = context.TblAuthUsers.Include("TblCompany1")
                    //    .Include("TblCompany2").Include("BarcodeDisplaySettingsHeader").
                    //    Include("TblUserBrandSections.TblUserBrandSectionPermissions").
                    //    FirstOrDefault(x => x.UserName.ToLower() == _USerName && x.UserPassword == _USerPassrword);

                    try
                    {
                        var xx = context.TblAuthUsers.Where(x => x.UserName.ToLower() == _USerName).ToList();
                        var authQuerye = context.TblAuthUsers.Where(x => x.UserName.ToLower() == _USerName && x.UserPassword == _USerPassrword).FirstOrDefault();
                    }
                    catch (Exception ex) { }

                    var authQuery = context.TblAuthUsers.Where(x => x.UserName.ToLower() == _USerName && x.UserPassword == _USerPassrword).FirstOrDefault();


                    if (authQuery != null)
                    {
                        int ExpiredDays = 0;
                        int.TryParse(UserLoginExpiredPeriod, out ExpiredDays);
                        var UserExpiryData = context.TblUserExpiries.FirstOrDefault(x => x.Tbluser == authQuery.Iserial);
                        if (UserExpiryData == null)
                        {
                            return Result;
                        }

                        DateTime dt = UserExpiryData.LastChangeDate.Value.AddDays(ExpiredDays);
                        if (dt < DateTime.Now)
                        {
                            Result = "1" + "," + authQuery.Iserial; //Expired Login
                        }
                    }
                    else
                    {
                        Result = "2"; //Invalid User Login & Password 
                    }
                }
                else
                {
                    Result = "multiUsers";
                }
            }
            return Result; // Valid Login
        }

        [OperationContract]
        public void ChangePassword(string username, string newPassword, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var row = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);
                row.UserPassword = newPassword;
                row.UserName = username;
                context.SaveChanges();
            }
        }
    }

    public class NewUserRequests
    {
        public string NewUserID { set; get; }
        public string NewUserCompanyID { set; get; }
        public string NewUserCompanyName { set; get; }
        public string LikeUserID { set; get; }
        public string LikeUserName { set; get; }
        public string LikeUserCompanyID { set; get; }
        public string LikeUserCompanyName { set; get; }
        public string IsSalesPerson { set; get; }
        public string IsRetailPerson { set; get; }
        public string UserName { set; get; }
        public string UserPassword { set; get; }
        public string Comment { set; get; }
        public string UserCreateRequestID { set; get; }
        public string UserCreateRequestName { get; set; }
        public DateTime RequestDate { set; get; }
        public DateTime ApproveDate { set; get; }
        public string Approved { set; get; }
        public string Rejected { set; get; }
    }

}
