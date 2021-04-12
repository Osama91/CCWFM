using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;


namespace CCWFM.Web.Report
{
    public partial class IssueFabricReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ReportViewer1.ServerReport.Refresh();
                ReportParameter[] reportParameter = new ReportParameter[3];
                reportParameter[0] = new ReportParameter("TransId", Request.QueryString["TransId"].ToString());
                reportParameter[1] = new ReportParameter("DyeingProductionOrder", Request.QueryString["DyeingProductionOrder"].ToString());
                reportParameter[2] = new ReportParameter("User", Request.QueryString["User"].ToString());

                ReportViewer1.ServerReport.SetParameters(reportParameter);   
            }
          
        }
    }
}