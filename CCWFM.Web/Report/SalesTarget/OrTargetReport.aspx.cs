using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using CCWFM.Web.Model;
using System.Globalization;

namespace CCWFM.Web.Report.SalesTarget
{
    public partial class OrTargerReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                List<MonthList> Month = new List<MonthList>();

                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(new MonthList { Number = i.ToString(), Name = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i - 1].ToString() });

                }

                DdlDate.DataValueField = "Number";
                DdlDate.DataTextField = "Name";


                for (int y = 2013; y < 2020; y++)
                {
                    DdlYear.Items.Add(Convert.ToString(y));
                }
                DdlDate.DataSource = Month;
                DdlDate.DataBind();
                DdlDate.SelectedValue = "2";
                DdlYear.Text = DateTime.Now.Year.ToString();



                ReportViewer1.ServerReport.Refresh();
                ReportParameter[] reportParameter = new ReportParameter[2];
                reportParameter[0] = new ReportParameter("Month", DdlDate.SelectedItem.ToString());
                reportParameter[1] = new ReportParameter("Year", DdlYear.SelectedValue);
                ReportViewer1.LocalReport.SetParameters(reportParameter);
                ReportViewer1.LocalReport.Refresh();
            }
        }

        protected void DdlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer1.ServerReport.Refresh();
            ReportParameter[] reportParameter = new ReportParameter[2];
            reportParameter[0] = new ReportParameter("Month", DdlDate.SelectedItem.ToString());
            reportParameter[1] = new ReportParameter("Year", DdlYear.SelectedValue);
            ReportViewer1.LocalReport.SetParameters(reportParameter);
            ReportViewer1.LocalReport.Refresh();
        }

        protected void DdlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer1.ServerReport.Refresh();
            ReportParameter[] reportParameter = new ReportParameter[2];
            reportParameter[0] = new ReportParameter("Month", DdlDate.SelectedItem.ToString());
            reportParameter[1] = new ReportParameter("Year", DdlYear.SelectedValue);
            ReportViewer1.LocalReport.SetParameters(reportParameter);
            ReportViewer1.LocalReport.Refresh();
        }
    }
}