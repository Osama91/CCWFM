using CCWFM.Helpers.AuthenticationHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Browser;

namespace CCWFM.ViewModel
{
    public class GenericReportViewModel
    {
        public void GenerateReport(string reportName, ObservableCollection<string> para)
        {
            Cookies.DeleteCookie("reportName");
            Cookies.DeleteCookie("para");   
            Cookies.SetCookie("reportName", reportName);
            string paraTemp =null;
            if (para.Any(p => p == (LoggedUserInfo.Ip + LoggedUserInfo.Port)))//
                para.Remove(LoggedUserInfo.Ip + LoggedUserInfo.Port);//
            if (para.Any(p => p == LoggedUserInfo.DatabasEname))//
                para.Remove(LoggedUserInfo.DatabasEname);//
            if (para.Any())
            {
                foreach (var variable in para)
                {

                    if (para.LastOrDefault() == variable)
                    {
                        paraTemp = paraTemp + variable;
                    }
                    else
                    {
                        paraTemp = paraTemp + variable + "+";
                    }

                }
            }
            Cookies.SetCookie("para", paraTemp);
            Cookies.SetCookie("Ip", (LoggedUserInfo.Ip + LoggedUserInfo.Port));//
            Cookies.SetCookie("Database", LoggedUserInfo.DatabasEname);//
            var myUri = new Uri(HtmlPage.Document.DocumentUri, String.Format("report/GenericReport.aspx"));
            HtmlPage.Window.Navigate(myUri, "_blank");

        }
    }
}