using System;
using System.Linq;
using System.Web.UI;
using CCWFM.Web.Model;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;

namespace CCWFM.Web.Report
{
    public partial class GenericReport : Page
    {
        public string ReportPath;
        public string PageName;
        public string ReportServer;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Cookies["reportName"] != null)
                {
                    PageName = Request.Cookies["reportName"].Value;
                }
                // PageName = CRUD_ManagerService.PublicReportName;

                string Ip = string.Empty, Database = string.Empty;//
                if (Request.Cookies["Ip"] != null)//
                {
                    Ip = Request.Cookies["Ip"].Value;//
                }//
                if (Request.Cookies["Database"] != null)//
                {//
                    Database = Request.Cookies["Database"].Value;//
                }//
                    string[] para = null;
                if (Request.Cookies["para"] != null)
                {
                    if (Request.Cookies["para"].Value.Split('+').All(x => x != ""))
                    {
                        para = Request.Cookies["para"].Value.Split('+');
                    }
                }
                ReportPath = "/Report/" + PageName;
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (context.TblAuthPermissions.Any(x => x.Code == PageName))
                    {
                        var tblAuthPermission = context.TblAuthPermissions.FirstOrDefault(x => x.Code == PageName);
                        if (tblAuthPermission != null)
                            ReportServer = tblAuthPermission.ReportServer;
                    }
                    if (string.IsNullOrEmpty(ReportServer))
                    {
                        var singleOrDefault = context.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "ReportServer");
                        if (singleOrDefault != null)
                            ReportServer = singleOrDefault.sSetupValue;
                    }
                }
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(ReportServer);
                ReportViewer1.ServerReport.ReportPath = ReportPath;
                Page.Title = PageName;
                var reportParameters = ReportViewer1.ServerReport.GetParameters();
                if ( (reportParameters.Count > 0))
                {
                    ReportViewer1.ServerReport.Refresh();
                    var count = 0;
                    var reportParameter = new List<ReportParameter>(); //new ReportParameter[para.Count()];
                    foreach (var item in reportParameters)
                    {
                        try
                        {
                            if (item.Name.ToLower().Trim() == "Ip".ToLower().Trim() && !string.IsNullOrWhiteSpace(Ip))//
                            {
                                reportParameter.Add(new ReportParameter(item.Name, Ip));//
                                continue;//
                            }
                            if (item.Name.ToLower().Trim() == "Database".ToLower().Trim() && !string.IsNullOrWhiteSpace(Database))//
                            {
                                reportParameter.Add(new ReportParameter(item.Name, Database));//
                                continue;//
                            }
                            if (para!=null&& para.Any())
                            {
                                reportParameter.Add( new ReportParameter(item.Name, para[count]));
                            }
                            count++;
                        }
                        catch (Exception)
                        {
                        }
                        
                    }
                    ReportViewer1.ServerReport.SetParameters(reportParameter);
                    ReportViewer1.ServerReport.Refresh();
                }
            }
        }
    }
}