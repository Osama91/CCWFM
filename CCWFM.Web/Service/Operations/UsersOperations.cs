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
        public List<tbl_Auth_User> GetAllUsers()
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_Auth_User.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void SaveUser(int iserial, int? TblJob, string Code, string Aname, string Ename, string UserName, string Password, string Tel1, string Tel2, string Address, string UserDomain, string WinLogin, short? CurLang, string AxId, string AxName, string Comment,int? PrintCode)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                tbl_Auth_User _User = new tbl_Auth_User();
                if (iserial!=0)
                {
                    _User = (from s in context.tbl_Auth_User
                                 where s.iserial == iserial
                                 select s).SingleOrDefault(); 
                }
                         
                _User.TblJob = TblJob;
                _User.Code = Code;
                _User.Aname = Aname;
                _User.Ename = Ename;
                _User.UserName = UserName;
                _User.UserPassword = Password;
                _User.Tel1 = Tel1;
                _User.Tel2 = Tel2;
                _User.Address = Address;
                _User.User_Domain = UserDomain;
                _User.User_Win_Login = WinLogin;
                _User.AxId = AxId;
                _User.AxName = AxName;
                _User.CurrLang = CurLang;
                _User.Comment = Comment;
                _User.PrintingCode = PrintCode;
                if (iserial == 0)
                {
                    context.AddTotbl_Auth_User(_User);
                }

                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteUser(int Iserial)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                tbl_Auth_User user = context.tbl_Auth_User.Where(x => x.iserial == Iserial).Single();

                context.DeleteObject(user);
                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_Auth_Job> GetAllJobs()
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_Auth_Job.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<User> GetAxUser()
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                return context.Users.ToList();
            }
        }
    }
}