using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.LoginService;
using CCWFM.Views.Login;

namespace CCWFM.Helpers.AuthenticationHelpers
{
    public static class LoggedUserInfo
    {
        public static bool Financial { get; set; }

        public static bool Retail { get; set; }

        public static bool Technical { get; set; }

        public static int? WFM_UserID { get; set; }

        public static string WFM_UserName { get; set; }

        public static int? WFM_UserJob { get; private set; }

        public static int? CurrLang { get; set; }

        public static bool? WFM_IsAdmin { get; set; }

        public static bool WFM_IsMenuInitiated { get; set; }

        public static int? PrintingBarcode { get; set; }

        public static string User_Win_Login { get; set; }

        public static string User_Domain { get; set; }

        public static string AxId { get; set; }

        public static int Iserial { get; set; }

        public static TblCompany Company { get; set; }

        public static TblCompany SecondaryComapany { get; set; }

        public static BarcodeDisplaySettingsHeader BarcodeSettingHeader { get; set; }

        public static List<TblAuthJobPermission> WFM_UserJobPermissions { get; set; }

        public static ObservableCollection<TblAuthPermission> WFM_MenuesPermissions { get; set; }

        public static ObservableCollection<TblUserBrandSection> UserBrandSection { get; set; }

        public static int? ActiveStore { get; set; }

        public static List<int> AllowedStores { get; set; }

        public static TblStore Store { get; set; }

        public static string DatabasEname { get; set; }

        public static string Ip { get; set; }

        public static string Port { get; set; }

        public static string Code { get; set; }

        public static string Ename { get; set; }
        public static void InitiatePermissions(MainPage main, StackPanel panelButtons, LoginMainWindow loginMain, LoginChildWindow loginChildWindow)
        {
            WFM_UserJobPermissions = new List<TblAuthJobPermission>();
            //var client = new CRUD_ManagerServiceClient();
            var client = new LoginService.LoginServiceClient();

            client.GetItemsPermissionsByParentCompleted += (s, e) =>
            {
                var temp = (from z in e.Result
                            where (from x in WFM_UserJobPermissions
                                   select x.TblPermission).Contains(z.Iserial)
                            select z).ToList();

                if (temp.Any(x => x.Code == "Brandsales"))
                {
                    main.Brandsales.Visibility = Visibility.Visible;
                    main.BrandSalesControl.Visibility = Visibility.Visible;
                    main.BrandSalesBorder.Visibility = Visibility.Visible;
                    main.TodaySales.Visibility = Visibility.Visible;
                }

                if (temp.Any(x => x.Code == "BrandPercentage"))
                {
                    main.BrandPercentage.Visibility = Visibility.Visible;
                    main.BrandPercentageControl.Visibility = Visibility.Visible;
                    main.BrandPercentageBorder.Visibility = Visibility.Visible;
                    main.TodaySales.Visibility = Visibility.Visible;
                }

                if (temp.Any(x => x.Code == "BrandRatio"))
                {
                    main.BrandRatio.Visibility = Visibility.Visible;
                    main.BrandRatioControl.Visibility = Visibility.Visible;
                    main.BrandRatioBorder.Visibility = Visibility.Visible;
                    main.TodaySales.Visibility = Visibility.Visible;
                }

                if (temp.Any(x => x.Code == "BrandMargin"))
                {
                    main.GrossMarginChart.Visibility = Visibility.Visible;
                    main.BrandMarginControl.Visibility = Visibility.Visible;
                    main.BrandMarginBorder.Visibility = Visibility.Visible;
                    main.GlDashBoard.Visibility = Visibility.Visible;
                }
            };

            client.GetUserJobAsync((int)WFM_UserID, WFM_UserName);
            client.GetUserJobCompleted += (s, ev) =>
            {
                WFM_UserJob = ev.Result;

                client.GetUserJobPermissionsAsync((int)WFM_UserJob);

                client.GetUserJobPermissionsCompleted += (s2, ev2) =>
                {
                    WFM_UserJobPermissions.AddRange(ev2.Result.ToList());
                    client.GetItemsPermissionsByParentAsync("Charts");
                    client.GetUserMenuesPermissionsAsync((int)WFM_UserJob);

                    client.GetUserMenuesPermissionsCompleted += (s3, ev3) =>
                    {
                        WFM_MenuesPermissions = new ObservableCollection<TblAuthPermission>(ev3.Result);
                        foreach (var variable in panelButtons.Children)
                        {
                            var btn = variable as Button;
                            if (!WFM_MenuesPermissions.Any(x => x.Code == btn.Name))
                            {
                                btn.Visibility = Visibility.Collapsed;

                            }
                            else
                            {
                                string toolTip = "";
                                if (CurrLang == 0)
                                {
                                    toolTip = WFM_MenuesPermissions.FirstOrDefault(x => x.Code == btn.Name).Aname;
                                }
                                else
                                {
                                    toolTip = WFM_MenuesPermissions.FirstOrDefault(x => x.Code == btn.Name).Ename;
                                }

                                btn.Visibility = Visibility.Visible;
                                ToolTipService.SetToolTip(btn, toolTip);
                            }
                        }

                        var aa = new MenuTestViewModel();
                        if (main.AppMenu.MenuItem != null)
                        {
                            main.AppMenu.MenuItem.Clear();
                            main.AppMenu.MenuItem.MenuItems.Clear();
                        }
                        main.DataContext = aa;

                        //if (menuCollection != null) InitiateMenusPermissions(menuCollection);
                        client.CloseAsync();
                        loginMain.Content = main;
                        loginChildWindow.DialogResult = true;
                    };
                };
                
            };
        }

        public static void GetItemPermissions(string itemName)
        {
            var client = new LoginServiceClient();
            client.GetItemsPermissionsCompleted += (s, ev) => WFM_UserJobPermissions.SingleOrDefault(x => x.TblPermission == ev.Result);
            client.GetItemsPermissionsAsync(itemName);
        }

        internal static void Reset()
        {
            WFM_UserID = null;
            WFM_UserJob = null;
            WFM_UserName = null;
            WFM_IsAdmin = null;
            User_Domain = null;
            User_Win_Login = null;
            AxId = null;
        }
    }
}