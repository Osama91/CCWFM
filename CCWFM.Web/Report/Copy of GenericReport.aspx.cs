//using System;
//using System.Linq;
//using System.Web.UI;
//using CCWFM.Web.Service;
//using Microsoft.Reporting.WebForms;

//namespace CCWFM.Web.Report
//{
//    public partial class GenericReport : Page
//    {
//        public string ReportPath;
//        public string PageName;
//        public string ReportServer;
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!Page.IsPostBack)
//            {
//                if (Request.Cookies["reportName"] != null)
//                {
//                    PageName = Request.Cookies["reportName"].Value;
//                }
//               // PageName = CRUD_ManagerService.PublicReportName;
                



//                ReportPath = "/Report/" + PageName;
//                ReportServer = CRUD_ManagerService.ReportServer;
//                ReportViewer1.ServerReport.ReportServerUrl = new Uri(ReportServer);
//                ReportViewer1.ServerReport.ReportPath = ReportPath;

//                Page.Title = PageName;

//                if (CRUD_ManagerService.PublicReportPara != null && (ReportViewer1.ServerReport.GetParameters().Count > 0 && CRUD_ManagerService.PublicReportPara.Any()))
//                {
//                    ReportViewer1.ServerReport.Refresh();

//                    var count = 0;
//                    var reportParameter = new ReportParameter[CRUD_ManagerService.PublicReportPara.Count];
//                    foreach (var item in ReportViewer1.ServerReport.GetParameters())
//                    {
//                        try
//                        {
                                
//                                reportParameter[count] = new ReportParameter(item.Name, CRUD_ManagerService.PublicReportPara[count]);
//                        }
//                        catch (Exception)
//                        {
//                        }

//                        count++;
//                    }
//                    CRUD_ManagerService.PublicReportPara.Clear();
//                    ReportViewer1.ServerReport.SetParameters(reportParameter);
//                    ReportViewer1.ServerReport.Refresh();
                    

                    
//                }
//            }
//        }
//    }
//}