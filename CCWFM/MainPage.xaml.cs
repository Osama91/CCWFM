using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Threading;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Views.Gl;
using CCWFM.Views.Login;
using CCWFM.Views.OGView;
using CCWFM.Views.OGView.ChildWindows;
using CustomControls;
using SilverlightMenu.Library;
using Color = System.Windows.Media.Color;
using GlPosting = CCWFM.Views.Gl.GlPosting;

namespace CCWFM
{
    public partial class MainPage
    {

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public class BrandSalesClass
        {
            public string Brand { get; set; }

            public int SalesValue { get; set; }

            public double SalesRatio { get; set; }

            public int SalesPercentage { get; set; }

            public Brush FavoriteColor { get; set; }
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private List<BrandSalesClass> _newbrandsales;

        public List<BrandSalesClass> NewbrandSalesData
        {
            get { return _newbrandsales ?? (_newbrandsales = new List<BrandSalesClass>()); }
            set { _newbrandsales = value; RaisePropertyChanged("NewbrandSalesData"); }
        }

        private List<BrandSalesClass> _newbrandComparision;

        public List<BrandSalesClass> NewbrandComparision
        {
            get { return _newbrandComparision ?? (_newbrandComparision = new List<BrandSalesClass>()); }
            set { _newbrandComparision = value; RaisePropertyChanged("NewbrandComparision"); }
        }

        private List<BrandSalesClass> _brandGrossMargin;

        public List<BrandSalesClass> BrandGrossMargin
        {
            get { return _brandGrossMargin ?? (_brandGrossMargin = new List<BrandSalesClass>()); }
            set { _brandGrossMargin = value; RaisePropertyChanged("BrandGrossMargin"); }
        }

        private readonly DispatcherTimer _myDispatcherTimer = new DispatcherTimer();

        private int _interval;

        public MainPage()
        {
            InitializeComponent();

            var brushes = new[] {
  Colors.Blue,
  Colors.White,
  Colors.Brown,
  Colors.Green,
  Colors.Red,
  Colors.Yellow,
  Colors.Yellow,
  Colors.Red,
    Colors.Blue,
      Colors.White
};
            AppMenu = new Menu();

            _myDispatcherTimer.Tick += Each_Tick;
            _client.GetDashBoardCompleted += (s, sv) =>
            {
                if (sv.Error != null)
                {
                    return;
                }
                if (sv.Result != null)
                {
                    NewbrandSalesData.Clear();
                }
                try
                {
                    foreach (var row in sv.Result.Where(x => x != sv.Result.FirstOrDefault()))
                    {
                        var x = row.Split(' ');
                        NewbrandSalesData.Add(new BrandSalesClass { Brand = x.FirstOrDefault(), SalesValue = Convert.ToInt32(x.LastOrDefault()), FavoriteColor = new SolidColorBrush { Color = Color.FromArgb((byte)sv.Result.IndexOf(row), 0, 0, (byte)sv.Result.IndexOf(row)) } });
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    Brandsales.DataContext = NewbrandSalesData;
                    if (NewbrandSalesData != null)
                        Brandsales.Title = "Brand Sales " + NewbrandSalesData.Sum(x => x.SalesValue);
                    else
                    {
                        Brandsales.Title = "Brand Sales";
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    _interval = sv.dashBoardInterval;
                    NewbrandComparision.Clear();
                    foreach (var row in sv.brandComparison)
                    {
                        var x = row.Split(' ');
                        var rowcount = sv.brandComparison.IndexOf(row);

                        var testColor = brushes[rowcount];

                        try
                        {
                            NewbrandComparision.Add(new BrandSalesClass { Brand = x.FirstOrDefault(), SalesPercentage = Convert.ToInt32((x.ElementAt(1).Substring(0, 2))), SalesRatio = Convert.ToDouble(x.LastOrDefault()), FavoriteColor = new SolidColorBrush { Color = testColor } });
                        }
                        catch (Exception)
                        {
                            NewbrandComparision.Add(new BrandSalesClass { Brand = x.FirstOrDefault(), SalesPercentage = Convert.ToInt32((x.ElementAt(1).Substring(0, 1))), SalesRatio = Convert.ToDouble(x.LastOrDefault()), FavoriteColor = new SolidColorBrush { Color = testColor } });
                        }
                    }
                }
                catch (Exception)
                {
                }
                var costOfGoodSoldList = new List<BrandSalesClass>();
                BrandGrossMargin.Clear();
                try
                {
                    foreach (var variable in sv.costOfGoodSoldList)
                    {
                        costOfGoodSoldList.Add(new BrandSalesClass
                        {
                            Brand = variable.Brand,
                            SalesValue = (int)variable.NetSales,
                        });
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    foreach (var variable in sv.netSalesList)
                    {
                        var rowcount = sv.netSalesList.IndexOf(variable);

                        var testColor = brushes[rowcount];
                        var row = new BrandSalesClass
                        {
                            Brand = variable.Brand,
                            FavoriteColor = new SolidColorBrush { Color = testColor },
                            SalesValue = (int)variable.NetSales,
                        };
                        var brandSalesClass = costOfGoodSoldList.FirstOrDefault(x => x.Brand == variable.Brand);
                        if (brandSalesClass != null)
                        {
                            var dif = (variable.NetSales -
                                       brandSalesClass.SalesValue);
                            row.SalesPercentage = (int)((dif / variable.NetSales) * 100);
                        }
                        BrandGrossMargin.Add(row);
                    }

                }
                catch (Exception)
                {
                }
                try
                {
                    ((DataPointSeries)BrandRatio.Series[0]).ItemsSource = NewbrandComparision;
                }
                catch (Exception)
                {
                }
                try
                {
                    ((DataPointSeries)BrandPercentage.Series[0]).ItemsSource = NewbrandComparision;
                }
                catch (Exception)
                {


                }
                try
                {
                    ((DataPointSeries)BrandPercentage.Series[0]).ItemsSource = NewbrandComparision;
                }
                catch (Exception)
                {
                }
                try
                {
                    ((DataPointSeries)GrossMarginChart.Series[0]).ItemsSource = BrandGrossMargin;
                }
                catch (Exception)
                {
                }
                try
                {
                    ((DataPointSeries)GrossMarginChart.Series[1]).ItemsSource = BrandGrossMargin;
                }
                catch (Exception)
                {
                }


                _myDispatcherTimer.Interval = new TimeSpan(0, 0, _interval, 0);

                _myDispatcherTimer.Start();
            };
            _client.GetDashBoardAsync();
        }

        public void Each_Tick(object o, EventArgs sender)
        {
            _client.GetDashBoardAsync();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (((HyperlinkButton)sender).Content.ToString().ToLower() == "logout")
            {
                InitiateLogoutProcess();
            }
            else if (((HyperlinkButton)sender).Content.ToString().ToLower() == "login")
            {
                InitiateLoginProcess();
            }
        }

        private void InitiateLogoutProcess()
        {
            var res = MessageBox.Show("press OK to logout, Or cancel if you want to remain logged in", "Logout", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                HtmlPage.Document.Submit();
            }
        }

        private void InitiateLoginProcess()
        {
            Content = new LoginMainWindow();
        }

        private void LoggedUserTextBlock_OnClick(object sender, RoutedEventArgs e)
        {
            var childPassword = new ChangePassword(Convert.ToInt32(LoggedUserTextBlock.TargetName));
            childPassword.Show();
        }

        private void LinkImage_OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var child = new FrameChildWindow();
            switch (btn.Name.ToLower())
            {
                case "stylecodingform":
                    child.Content = new StyleHeader(SalesOrderType.SalesOrderPo, true);
                    //   _client.EndPoAsync(703, LoggedUserInfo.Iserial, "555555");
                    // child.Content = new UserFormLayout();
                    break;

                case "employeeshiftform":

                    child.Content = new EmployeeShift();

                    break;

                case "retailpoform":

                    child.Content = new StyleHeader(SalesOrderType.RetailPo, false);

                    break;

                case "globalretailbusinessbudget":

                    child.LayoutRoot.Children.Add(new GlobalBudget(1));//retail Budget

                    break;

                case "brandbudget":

                    child.LayoutRoot.Children.Add(new BrandBudget(1));//retail Budget

                    break;

                case "asset":

                    child.LayoutRoot.Children.Add(new Asset());

                    break;

                case "glposting":

                    child.LayoutRoot.Children.Add(new GlPosting());

                    break;

                case "recinv":

                    child.LayoutRoot.Children.Add(new RecInv());

                    break;

                case "bank":
                    child.LayoutRoot.Children.Add(new Bank());
                    break;
                case "ledgerheader":
                    child.LayoutRoot.Children.Add(new Ledger());
                    break;
                case "account":
                    child.LayoutRoot.Children.Add(new Account());
                    break;
                case "CostCenterOrganizationUnit":
                    child.LayoutRoot.Children.Add(new CostCenterOrganizationUnit());
                    break;


            }
            child.Title = btn.Tag.ToString();
            if (LoggedUserInfo.CurrLang == 0)
            {
                child.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                child.FlowDirection = FlowDirection.LeftToRight;
            }
            child.Show();
        }

        private void LabeledPieSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrandSalesControl.Content = ((LabeledPieSeries)sender).SelectedItem;
        }

        private void BrandComparision_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrandRatioControl.Content = ((ColumnSeries)sender).SelectedItem;
        }

        private void Aa_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrandPercentageControl.Content = ((ColumnSeries)sender).SelectedItem;
        }

        private void GrossMargin_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrandMarginControl.Content = ((ColumnSeries)sender).SelectedItem;
        }
    }
}