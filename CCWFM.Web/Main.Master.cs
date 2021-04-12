using System;
using CCWFM.Web.Classes;

namespace CCWFM.Web
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GettingUser();
        }

        private void GettingUser()
        {
            LoggedUserInfo.WfmUserName = Page.User.Identity.Name;
            LoggedUserInfo.InitiatePermissions();
        }

        public static string UserWinLogin { get; set; }

        public static string Domain { get; set; }

        public static int Iserial { get; set; }

        public static string Code;

        public static string Ip;

        public static string Port;

        public static string DatabaseName;
        public static string PublicReportName;
    }
}