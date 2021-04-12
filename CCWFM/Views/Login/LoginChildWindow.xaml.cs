using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Defaults;

using System.Text.RegularExpressions;
using CCWFM.LoginService;
using System.IO;

namespace CCWFM.Views.Login
{
    public partial class LoginChildWindow
    {
        private readonly LoginMainWindow _loginMain;
        public object UserSerial { get; private set; }
        public int LogedInUserSerial { get; set; }

        public LoginChildWindow(LoginMainWindow loginMainWindow)
        {
            InitializeComponent();
            _loginMain = loginMainWindow;
            grdUpdatePassword.Visibility = Visibility.Collapsed;
            pnlName.Text = "Login";
            LoginServiceClient _client = new LoginServiceClient();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            LgnProgress.Visibility = Visibility.Visible;
            OkButton.IsEnabled = false;

            var client = new LoginService.LoginServiceClient();
            client.AuthenticateLoginExpirationCompleted += _client_AuthenticateLoginExpirationCompleted;
            client.AuthenticateLoginExpirationAsync(UserNameTextBox.Text.Trim(), PasswordTextBox.Password, txtCompany.Text.Trim());

        }

        private void _client_AuthenticateLoginExpirationCompleted(object sender, AuthenticateLoginExpirationCompletedEventArgs e)
        {
            if (e.Result.ToString() == "0")
            {
                var client = new LoginService.LoginServiceClient();
                client.AuthenticateCompleted += _client_AuthenticateCompleted;
                client.AuthenticateAsync(UserNameTextBox.Text.Trim(), PasswordTextBox.Password, txtCompany.Text.Trim());
            }
            else if (e.Result.ToString().Contains("1,"))
            {
                string _UserSerial = e.Result.ToString().Split(',')[1].ToString();
                OkButton.IsEnabled = true;

                LoginRoot.Visibility = Visibility.Collapsed;
                grdUpdatePassword.Visibility = Visibility.Visible;
                pnlName.Text = "Update Expired Password";

                LogedInUserSerial = int.Parse(_UserSerial);

                //var childWindow = new UpdatePassword(UserNameTextBox.Text.Trim(),UserSerial);
                //childWindow.Show();
            }
            else if (e.Result.ToString() == "InvalidComapny")
            {
                MessagesTextBlock.Text = "Invalid Company Name!";
                LgnProgress.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;
            }
            else if (e.Result.ToString() == "multiUsers")
            {
                MessagesTextBlock.Text = "Please Enter Company Name!";
                LgnProgress.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;
            }
            else if (e.Result.ToString() == "2")
            {
                MessagesTextBlock.Text = "Incorrect User Name,Password or Company!";
                LgnProgress.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;
            }
        }

        private void _client_ChangePasswordCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var client = new LoginService.LoginServiceClient();
            client.AuthenticateCompleted += _client_AuthenticateCompleted;
            client.AuthenticateAsync(UserNameTextBox.Text.Trim(), txtPassword.Password, txtCompany.Text.Trim());
        }

        private void _client_AuthenticateCompleted(object sender, LoginService.AuthenticateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message + Environment.NewLine + e.Error.InnerException);
                MessagesTextBlock.Text = "Error connection to server!";
                LgnProgress.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;

                throw e.Error;
            }

            if (e.Result != null)
            {
                LoggedUserInfo.WFM_UserID = e.Result.Iserial;
                LoggedUserInfo.Ename = e.Result.Ename;
                LoggedUserInfo.Code = e.Result.Code;
                LoggedUserInfo.WFM_UserName = e.Result.UserName;
                LoggedUserInfo.PrintingBarcode = e.Result.PrintingCode;
                LoggedUserInfo.User_Domain = e.Result.User_Domain;
                LoggedUserInfo.User_Win_Login = e.Result.User_Win_Login;
                LoggedUserInfo.AxId = e.Result.AxId;
                LoggedUserInfo.Iserial = e.Result.Iserial;
                LoggedUserInfo.BarcodeSettingHeader = e.Result.BarcodeDisplaySettingsHeader;
                LoggedUserInfo.UserBrandSection = e.Result.TblUserBrandSections;
                LoggedUserInfo.Company = e.Result.TblCompany1;
                LoggedUserInfo.SecondaryComapany = e.Result.TblCompany2;
                if (LoggedUserInfo.SecondaryComapany != null)
                {
                    LoggedUserInfo.DatabasEname = e.Result.TblCompany2.DbName;
                    LoggedUserInfo.Ip = e.Result.TblCompany2.Ip;
                    LoggedUserInfo.Port = e.Result.TblCompany2.Port;
                }
                else
                {
                    LoggedUserInfo.DatabasEname = e.Result.TblCompany1.DbName;
                    LoggedUserInfo.Ip = e.Result.TblCompany1.Ip;
                    LoggedUserInfo.Port = e.Result.TblCompany1.Port;
                }

                LoggedUserInfo.Store = e.store;
                LoggedUserInfo.AllowedStores = !string.IsNullOrEmpty(e.Result.AllowedStores) ? e.Result.AllowedStores.Split('|').Select(n => int.Parse(n)).ToList() : null;
                LoggedUserInfo.ActiveStore = e.Result.ActiveStore;
                LoggedUserInfo.CurrLang = e.Result.CurrLang;

                var main = new MainPage();

                if (e.Result.CurrLang == null || e.Result.CurrLang == 0)
                {
                    var culture = new CultureInfo("ar");
                    var datetimeFormatInfo = new DateTimeFormatInfo { LongDatePattern = "dd-MM-yyyy", ShortDatePattern = "dd-MM-yyyy" };
                    culture.DateTimeFormat = datetimeFormatInfo;

                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;

                    Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
                    main.FlowDirection = FlowDirection.RightToLeft;
                }
                else
                {
                    var culture = new CultureInfo("en");
                    var datetimeFormatInfo = new DateTimeFormatInfo { LongDatePattern = "dd-MM-yyyy", ShortDatePattern = "dd-MM-yyyy" };
                    culture.DateTimeFormat = datetimeFormatInfo;
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
                    main.FlowDirection = FlowDirection.LeftToRight;
                }
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                _LoginWindow_SubmitAction(main, _loginMain, this);
            }
            else
            {
                MessagesTextBlock.Text = "Incorrect User name or Password!";
                LgnProgress.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;
                PasswordTextBox.SelectAll();
            }
        }

        private void _LoginWindow_SubmitAction(MainPage main, LoginMainWindow _loginMain, LoginChildWindow loginChildWindow)
        {
            main.LoggedUserTextBlock.Content = LoggedUserInfo.WFM_UserName;
            main.LoggedUserTextBlock.Tag = LoggedUserInfo.Code;
            main.LoggedUserTextBlock.TargetName = LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture);
            if (!LoggedUserInfo.WFM_IsMenuInitiated)
            {
                LoggedUserInfo.InitiatePermissions(main, main.PanelButtons, _loginMain, loginChildWindow);
            }
            DefaultUserSettings.DATAAREAID = new Dictionary<string, string> { { "CCM", "CCM" }, { "CCR", "CCR" } };
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OKButton_Click(null, null);
            }
        }

        private void UpdateOkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool IsValidPassword = PasswordPolicy.IsValid(txtPassword.Password);
                if (IsValidPassword && (txtPassword.Password.Trim() == txtConfirmPassword.Password.Trim()))
                {
                    lblMessageUpdate.Visibility = Visibility.Collapsed;

                    LoginServiceClient _client = new LoginServiceClient();
                    _client.ChangePasswordCompleted += _client_ChangePasswordCompleted;
                    _client.ChangePasswordAsync(UserNameTextBox.Text.Trim(), txtPassword.Password, LogedInUserSerial);

                }
                else
                {
                    if (txtPassword.Password.Trim() != txtConfirmPassword.Password.Trim())
                    {
                        lblMessageUpdate.Visibility = Visibility.Visible;
                        lblMessageUpdate.Content = "Passwords not matched";
                    }
                    else
                    {
                        lblMessageUpdate.Visibility = Visibility.Visible;
                        lblMessageUpdate.Content = "Invalid Password";
                    }
                }
            }
            catch { }
        }

        private void grdUpdatePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateOkButton_Click(null, null);
            }
        }
    }

    public class PasswordPolicy
    {
        private static int Minimum_Length = 8;
        private static int Upper_Case_length = 1;
        private static int Lower_Case_length = 1;
        private static int NonAlpha_length = 1;
        private static int Numeric_length = 1;

        public static bool IsValid(string Password)
        {
            if (Password.Length < Minimum_Length)
                return false;
            if (UpperCaseCount(Password) < Upper_Case_length)
                return false;
            if (LowerCaseCount(Password) < Lower_Case_length)
                return false;
            if (NumericCount(Password) < Numeric_length)
                return false;
            if (NonAlphaCount(Password) < NonAlpha_length)
                return false;
            return true;
        }

        private static int UpperCaseCount(string Password)
        {
            return Regex.Matches(Password, "[A-Z]").Count;
        }

        private static int LowerCaseCount(string Password)
        {
            return Regex.Matches(Password, "[a-z]").Count;
        }
        private static int NumericCount(string Password)
        {
            return Regex.Matches(Password, "[0-9]").Count;
        }
        private static int NonAlphaCount(string Password)
        {
            return Regex.Matches(Password, @"[^0-9a-zA-Z\._]").Count;
        }
    }
}