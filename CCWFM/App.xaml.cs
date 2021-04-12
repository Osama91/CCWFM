using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Views.Login;
using CCWFM.Helpers.AuthenticationHelpers;
using Newtonsoft.Json;
using CCWFM.Views.OGView;
using System.Collections.Generic;

namespace CCWFM
{
    public partial class App
    {
        public AssistanceService.AssistanceServiceClient AssistanceClient;
        public App()
        {
            Startup += Application_Startup;
            UnhandledException += Application_UnhandledException;
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
           //RootVisual = new UserFormLayout();
            string url = App.Current.Host.NavigationState.ToString();
            if (url == "")
            {
                RootVisual = new LoginMainWindow();
            }
            else
            {
                string [] Params = url.Split(',');
                string tblUser = Params[1];
                this.RootVisual = new ChildPage(int.Parse(tblUser), "");
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject.Message.Contains("Specified argument was out of the range of valid values"))
            {
                e.Handled = true;
                return;
            }
            try
            {
                var ex = Models.Helper.GetInnerException(e.ExceptionObject);
                if (AssistanceClient == null)
                    AssistanceClient = new AssistanceService.AssistanceServiceClient();
                AssistanceClient.SaveLogAsync(JsonConvert.SerializeObject(e.ExceptionObject), LoggedUserInfo.Iserial);
                if (!Debugger.IsAttached)
                {
                    e.Handled = true;
                    ChildWindow errorWin = new ErrorWindow(ex);
                    errorWin.Show();
                    e.Handled = true;
                }
                else
                {
                    ChildWindow errorWin = new ErrorWindow(ex);
                    errorWin.Show();
                    e.Handled = true;
                }
            }
            catch { }
        }
    }
}