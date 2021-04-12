using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCWFM.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            using (Model.WorkFlowManagerDBEntities db = new Model.WorkFlowManagerDBEntities())
            {
                try
                {
                    LogTransactionsStatus = Convert.ToBoolean(db.tblChainSetups.FirstOrDefault(cs => cs.sGlobalSettingCode == nameof(LogTransactionsStatus)).sSetupValue);
                }
                catch { }
            }
        }
        #region Static
        public static bool LogTransactionsStatus = false;
        public const string TransactionsLoggerName = "Transactions";
        public const string InfoLoggerName = "Info";
        #endregion

        private string Concatenate(string userName, IList<string> roles)
        {
            string result = userName + ";";

            foreach (string role in roles)
                result += role + ";";

            return result;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}