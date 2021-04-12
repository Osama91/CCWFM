using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ReportsChildWindow
    {
        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        private readonly int _iserial;
        private readonly string _formName = null;
        public ReportsChildWindow(string formName,int iserial)
        {
            InitializeComponent();
            _client.GetReportAsync(formName);
            _client.GetReportCompleted += (s, sv) =>
            {
                Reports.ItemsSource = sv.Result;
            };
            _iserial = iserial;
            _formName = formName;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var link = sender as HyperlinkButton;
            if (link != null)
            {
                var report = link.DataContext as TblReport;
                if (report != null)
                {
                    var reportName = report.Code;

                    //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
                    //{ reportName = "FabricInspectionar"; }

                    var para = new ObservableCollection<string> { LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture) };
                    if (_formName=="SalesOrder")
                    {
                        para = new ObservableCollection<string> { _iserial.ToString(CultureInfo.InvariantCulture) };
                    }
                    if (_formName == "StyleFamilyReport")
                    {
                        para = new ObservableCollection<string> { _iserial.ToString(CultureInfo.InvariantCulture) };
                    }
                    if (_formName == "Marker")
                    {
                        para = new ObservableCollection<string> { _iserial.ToString(CultureInfo.InvariantCulture) };
                    }
                    if (_formName == "RouteCardForm")
                    {
                        para = new ObservableCollection<string> { _iserial.ToString(CultureInfo.InvariantCulture) };
                    
                    }
                    if (_formName == "LedgerHeader")
                    {
                        para = new ObservableCollection<string>
                        {
                            _iserial.ToString(CultureInfo.InvariantCulture),
                            LoggedUserInfo.Ip + LoggedUserInfo.Port,
                            LoggedUserInfo.DatabasEname
                        };
                        string item;
                        if (    LoggedUserInfo.CurrLang == 0)
                        {
                            item = reportName + "Ar";
                            var reportViewmodel1 = new GenericReportViewModel();
                            reportViewmodel1.GenerateReport(item, para);
                            return;
                        }
                    }
                    if (_formName == "StyleCodingForm")
                    {
                        if (link.Content.ToString() == "TechPack")
                        {
                            para = new ObservableCollection<string>
                            { 
                                _iserial.ToString(CultureInfo.InvariantCulture),
                                LoggedUserInfo.Ip + LoggedUserInfo.Port,
                                LoggedUserInfo.DatabasEname
                           };
                        }
                    }
                    var reportViewmodel = new GenericReportViewModel();
                    reportViewmodel.GenerateReport(reportName, para);
                }
            }
        }
    }
}