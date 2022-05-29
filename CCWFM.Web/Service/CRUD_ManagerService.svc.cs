using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using System.Timers;
using CCWFM.Web.Model;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using MailMessage = System.Net.Mail.MailMessage;

namespace CCWFM.Web.Service
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public partial class CRUD_ManagerService
    {
        private Timer tM = new Timer { Enabled = true };

        private void Dashboard()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var firstOrDefault = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "RefreshDashBoard");
                if (firstOrDefault != null)
                    DashBoardInterval = Convert.ToInt32(firstOrDefault.sSetupValue);
                GetBrandSales();
            }
        }

        [OperationContract]
        private List<string> GetDashBoard(out int dashBoardInterval, out List<string> brandComparison, out List<GlPosting> netSalesList, out List<GlPosting> costOfGoodSoldList)
        {
            if (DashBoardInterval == 0)
            {
                Dashboard();
            }
            else
            {
                var time = new TimeSpan(0, DashBoardInterval, 0).TotalMilliseconds;
                if (time != tM.Interval)
                {
                    tM.Interval = new TimeSpan(0, DashBoardInterval, 0).TotalMilliseconds;
                    tM.Elapsed += (s, sv) => Dashboard();
                }
            }

            dashBoardInterval = DashBoardInterval;
            brandComparison = Brandcomparison;

            netSalesList = NetSalesList;

            costOfGoodSoldList = CostOfGoodSoldList;
            return BrandSales;
        }

        [OperationContract]
        public double ConvertCurrency(string fromCurrency, string toCurrency, double amount)
        {
            var url = string.Format("http://rate-exchange.appspot.com/currency?from={0}&to={1}", fromCurrency, toCurrency);
            var client = new WebClient();
            var response = client.DownloadString(url);
            var regex = new Regex("rhs: \\\"(\\d*.\\d*)");
            var convertedAmount = amount * Convert.ToDouble(regex.Match(response).Groups[1].Value);
            return convertedAmount;
        }

        [OperationContract]
        public void SendMailReport(string EmpCode, string reportName, string Subject, string body)
        {
            string deviceInfo = null;
            string extension = String.Empty;
            string mimeType = String.Empty;
            string encoding = String.Empty;
            Warning[] warnings = null;
            string[] streamIDs = null;
            string historyId = null;

            // Create a Report Execution object
            var rsExec = new ReportExecutionService();
            rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;
            rsExec.Url = ReportServer + "/ReportExecution2005.asmx";

            // Load the report
            ExecutionInfo execInfo = rsExec.LoadReport("/Report/" + PublicReportName, historyId);

            var parameters = new ParameterValue[PublicReportPara.Count];
            foreach (var row in PublicReportPara)
            {
                int index = PublicReportPara.IndexOf(row);
                parameters[0] = new ParameterValue();
                parameters[index].Value = row;
                parameters[index].Name = execInfo.Parameters[index].Name;

                // paramters) { Name = , Value = row } }, "en-us");
            }
            rsExec.SetExecutionParameters(parameters, "en-us");

            // get pdf of report
            Byte[] results = rsExec.Render("PDF", deviceInfo,
            out extension, out encoding,
            out mimeType, out warnings, out streamIDs);

            //Walla...almost no code, it's easy to manage and your done.

            //Take the bytes and add as an attachment to a MailMessage(SMTP):

            var attach = new Attachment(new MemoryStream(results),
                String.Format("{0}.pdf", reportName));

            string emailFrom;
            using (var Model = new WorkFlowManagerDBEntities())
            {
                try
                {
                    emailFrom = Model.Employees.FirstOrDefault(x => x.EMPLID == EmpCode).Email;
                }
                catch (Exception)
                {
                    emailFrom = Model.Employees.FirstOrDefault(x => x.EMPLID == "0555").Email;
                }
            }

            string emailTo;
            using (var Model = new WorkFlowManagerDBEntities())
            {
                emailTo = "osama.gamal@ccasual.loc";
                // emailFrom = Model.Employees.FirstOrDefault(x => x.EMPLID == EmpCode).Email;
            }
            SendEmail(attach, emailFrom, new List<string> { emailTo }, Subject, body);
        }

        //********************************************************************************
        public bool SendEmail(Attachment msg, string emailfrom, List<string> emailTo, string subject, string body)
        {
            //********************************************************************************

            try
            {
                var msga = new MailMessage { Subject = subject, Body = body };
                if (msg != null) msga.Attachments.Add(msg);
                foreach (var variable in emailTo)
                {
                    msga.To.Add(variable);
                }
                msga.From = new MailAddress(emailfrom);
                msga.IsBodyHtml = true;
                var smtp = new SmtpClient("192.168.1.5");
                smtp.Send(msga);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [OperationContract]
        public void ServerConnections(string ip, string port, string databaseName)
        {
            Main.Ip = ip;
            Main.Port = port;
            Main.DatabaseName = databaseName;
        }
        static List<TblCompany> Companies = new List<TblCompany>();

        private static TblCompany GetCompany(string dbName)
        {

            TblCompany company;

            if (Companies.Count == 0)
            {
                using (var context = new WorkFlowManagerDBEntities())
                {

                    Companies = context.TblCompanies.ToList();
                }
            }


            //if (Companies.Count > 0)
            //{
                company = Companies.FirstOrDefault(x => x.DbName.ToLowerInvariant() == dbName.ToLowerInvariant());
                return company;
            //}

         

            //return company;
        }

        public string GetSqlConnectionString(string dbName)
        {
            var sqlBuilder = new SqlConnectionStringBuilder();

            TblCompany company = GetCompany(dbName);
            // Set the properties for the data source.
            if (company != null) sqlBuilder.DataSource = company.Ip + company.Port;
            sqlBuilder.InitialCatalog = dbName;
            sqlBuilder.UserID = "Pts";
            sqlBuilder.Password = "2583094";
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.MultipleActiveResultSets = true;
            // Build the SqlConnection connection string.
            var providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            var entityBuilder = new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = "System.Data.SqlClient";

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/Model.CCNewEntities.csdl|res://*/Model.CCNewEntities.ssdl|res://*/Model.CCNewEntities.msl";

            return entityBuilder.ToString();
        }      

        private static List<ObjectParameter> ConvertToParamters(Dictionary<string, object> valuesObjects)
        {
            return valuesObjects.Select(valuesObject => new ObjectParameter(valuesObject.Key, valuesObject.Value)).ToList();
        }

        private IEnumerable<string> GenericUpdate<T>(T oldValues, T newValues, ObjectContext entities)
        {
            var orgRow = entities.ObjectStateManager.GetObjectStateEntry(oldValues);
            orgRow.ApplyCurrentValues(newValues);
            var modifiedProperties = orgRow.GetModifiedProperties();

            return modifiedProperties;
        }

        public static int NullableTryParseInt32(byte? flag)
        {
            return Convert.ToInt32(flag);
        }

        public static int? NullableTryParseInt32FromStr(string text)
        {
            int value;
            return int.TryParse(text, out value) ? (int?)value : null;
        }

        [OperationContract]
        public List<TblReport> GetReport(string page)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblReports.Where(x => x.FormName == page).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblColor> GetTBLTechPackSeasonalMaterListColors(int TBLStyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblColors.Where(X => X.TblSeasonalMasterLists.Any(c => c.TblStyle == TBLStyle));
                return query.ToList();
            }
        }
        
     

        public static List<string> BrandSales { get; set; }

        public static List<string> Brandcomparison { get; set; }

        public static List<GlPosting> NetSalesList { get; set; }

        public static List<GlPosting> CostOfGoodSoldList { get; set; }

        public static int DashBoardInterval { get; set; }

        [OperationContract]
        public void ViewReport(string reportname, List<string> reportPara)
        {
            PublicReportName = reportname;
            PublicReportPara = reportPara;
            var reportserver = "";
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (context.TblAuthPermissions.Any(x => x.Code == reportname))
                {
                    var tblAuthPermission = context.TblAuthPermissions.FirstOrDefault(x => x.Code == reportname);
                    if (tblAuthPermission != null)
                        reportserver = tblAuthPermission.ReportServer;
                }
                if (string.IsNullOrEmpty(reportserver))
                {
                    var singleOrDefault = context.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "ReportServer");
                    if (singleOrDefault != null)
                        reportserver = singleOrDefault.sSetupValue;
                }
            }
            ReportServer = reportserver;
        }

        static public string PublicReportName { get; set; }

        static public string ReportServer { get; set; }

        static public List<string> PublicReportPara { get; set; }
    }
}